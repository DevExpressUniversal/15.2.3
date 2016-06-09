#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.CodeGeneration;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	[ComVisible(true)]
	public sealed class EditorPane : Microsoft.VisualStudio.Shell.WindowPane,
		IOleCommandTarget,
		IVsFindTarget,
		IVsPersistDocData,
		IVsFileChangeEvents,
		IExtensibleObject,
		IEditorNavigation,
		IVsRunningDocTableEvents,
		IPersistFileFormat {
		private const uint WM_KEYUP = 0x0101;
		private const uint WM_KEYDOWN = 0x0100;
		private const uint V_TAB = 0x09;
		private const uint V_CONTROL = 0x11;
		private _DTE dte = null;
		private ProjectWrapper projectWrapper = null;
		private Guid traceGuid = Guid.Empty;
		private string fileName;
		private bool gettingCheckoutStatus;
		private DesignErrorControl error = new DesignErrorControl();
		private ModelEditorControl editorControl;
		private ProjectItem currentProjectItem;
		private IModelEditorController modelEditorController;
		private Form modelForm = null;
		private List<string> openingModelEditorLog = new List<string>(10);
		private bool controlPressed = false;
		private uint _rdtEventsCookie = 0;
		private static Dictionary<ErrorTask, ArrayList> errorTasks = new Dictionary<ErrorTask, ArrayList>();
		private ITypeResolutionService typeResolutionService;
		private Microsoft.VisualStudio.OLE.Interop.IServiceProvider comServiceProvider;
		private IVsFileChangeEx fileChangeExService;
		private uint fileChangeCookie = VSConstants.VSCOOKIE_NIL;
		private Timer fileChangeTimer = new Timer();
		private bool fileChangeTimerSet;
		private IDisposable obj;
		private Form ModelEditorForm {
			get {
				if(modelForm == null) {
					modelForm = new ModelEditorForm();
					modelForm.FormBorderStyle = FormBorderStyle.None;
				}
				return modelForm;
			}
		}
		private void PrivateInit(string fileName, ProjectItem currentProjectItem, IVsHierarchy pvHier) {
			try {
				this.currentProjectItem = currentProjectItem;
				this.dte = currentProjectItem.DTE;
				this.fileName = fileName;
				this.projectWrapper = ProjectWrapper.Create(currentProjectItem.ContainingProject);
				XafDesignerHelper.AddLicx(pvHier, this);
				if(XafDesignerHelper.CheckLicenseExpired()) {
					ModelEditorForm.Controls.Add(XafDesignerHelper.GetLicenseErrorControl());
					return;
				}
				ModelLoader modelLoader = new ModelLoader(projectWrapper, XafTypesInfo.Instance, this);
				ReflectionHelper.Reset();
				XafTypesInfo.HardReset();
				DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.ForceInitialize();
				ReflectionHelper.AssemblyResolve += new AssemblyResolveEventHandler(ReflectionHelper_AssemblyResolve);
				ReflectionHelper.TypeResolve += new TypeResolveEventHandler(ReflectionHelper_TypeResolve);
				DevExpress.ExpressApp.Model.Core.DesignerOnlyCalculator.IsRunFromDesigner = true;
				DynamicTypeService dynamicTypeService = (DynamicTypeService)GetService(typeof(DynamicTypeService));
				ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)dynamicTypeService.GetTypeDiscoveryService(pvHier);
				typeResolutionService = dynamicTypeService.GetTypeResolutionService(pvHier);
				EFDesignTimeTypeInfoHelper.ForceInitialize(typeDiscoveryService);
				modelEditorController = modelLoader.LoadModel(typeDiscoveryService, typeResolutionService, fileName, out obj);
				modelEditorController.DesignMode = true;
				editorControl = new ModelEditorControl(new SettingsStorageOnHashtable(LoadSettings()));
				((ModelEditorViewController)modelEditorController).SetControl(editorControl);
				((ModelEditorViewController)modelEditorController).SetTemplate(null);
				((ModelEditorViewController)modelEditorController).DesignMode = true;
				editorControl.TabIndex = 0;
				editorControl.TabStop = false;
				editorControl.Dock = DockStyle.Fill;
				ModelEditorForm.Controls.Add(editorControl);
				ModelEditorForm.ActiveControl = editorControl.modelTreeList;
				modelEditorController.Modifying += new System.ComponentModel.CancelEventHandler(ModelEditorController_Modifying);
				modelEditorController.CanSaveBoFiles += new EventHandler<FileModelStoreCancelEventArgs>(modelEditorController_CanSaveBoFiles);
				modelEditorController.CurrentNodeChanged += new EventHandler(ModelEditorController_CurrentNodeChanged);
				if(Path.GetFileNameWithoutExtension(fileName).Contains("_")) {
					string onlyFilename = Path.GetFileNameWithoutExtension(fileName);
					string aspect = onlyFilename.Substring(onlyFilename.IndexOf("_") + 1);
					modelEditorController.SetCurrentAspectByName(aspect);
				}
				LookAndFeelUtils.useXPStyle = true;
				LookAndFeelUtils.ApplyStyle(editorControl);
				gettingCheckoutStatus = false;
				setupCommands();
				AdviseOnRDTEvents();
				modelEditorController.LoadSettings();
			}
			catch(Exception e) {
				Tracing.LogError(traceGuid, e);
				error.Dock = DockStyle.Fill;
				ModelEditorForm.Controls.Add(error);
				error.SetErrorMessage(e, "One or more errors encountered while loading the designer. The errors are listed below. Some errors can be fixed may require code or model changes.", true, "Build the project and reload the model editor");
				error.BuildProjectClicked += new EventHandler(error_BuildProjectClicked);
				if(e is CompilerErrorException) {
					string tempFileName = GetGeneratedCodeTempFile(((CompilerErrorException)e).SourceCode);
					error.ShowGeneratedCodeClicked += new EventHandler(delegate {
						ShowGeneratedCode(tempFileName);
					});
				}
			}
		}
		private Type ReflectionHelper_TypeResolve(object sender, TypeResolveEventArgs args) {
			try {
				return typeResolutionService.GetType(args.TypeName, false);
			}
			catch {
				return null;
			}
		}
		private void error_BuildProjectClicked(object sender, EventArgs e) {
			try {
				if(projectWrapper.Build()) {
					projectWrapper.ReOpenProjectItem(currentProjectItem, vsSaveChanges.vsSaveChangesNo);
				}
				else {
					error.SetErrorMessage(new Exception(), "Build failed because of the errors in your code.\r\nThe current and related projects should be built before the Model Editor can be loaded.\r\nFix the errors and then use the link below to build the projects and reload the Model Editor.", true, "Build the project and reload the model editor");
				}
			}
			catch(Exception ex) {
				Tracing.Tracer.LogWarning("Build model editor failed", ex.Message, ex.StackTrace);
				throw;
			}
		}
		private string GetGeneratedCodeTempFile(string sourceCode) {
			string result = "";
			try {
				string tempFileName = Path.GetTempFileName();
				result = tempFileName.Replace(".tmp", ".cs");
				using(FileStream stream = new FileStream(result, FileMode.Create, FileAccess.Write)) {
					StreamWriter sw = new StreamWriter(new MemoryStream());
					sw.WriteLine(sourceCode);
					sw.Flush();
					BinaryReader binaryReader = new BinaryReader(sw.BaseStream, System.Text.Encoding.UTF8);
					sw.BaseStream.Position = 0;
					byte[] bytes = binaryReader.ReadBytes(System.Convert.ToInt32(sw.BaseStream.Length));
					stream.Position = stream.Length;
					stream.Write(bytes, 0, bytes.Length);
				}
			}
			catch { }
			return result;
		}
		private void ShowGeneratedCode(string fileName) {
			try {
				EnvDTE.Window window = dte.OpenFile(EnvDTE.Constants.vsViewKindCode, fileName);
				if(window != null) {
					window.Activate();
				}
			}
			catch { }
		}
		private void AdviseOnRDTEvents() {
			IVsRunningDocumentTable2 pRDT2 = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable2;
			if(pRDT2 == null)
				ErrorHandler.ThrowOnFailure(VSConstants.E_UNEXPECTED);
			((IVsRunningDocumentTable)pRDT2).AdviseRunningDocTableEvents((IVsRunningDocTableEvents)this, out _rdtEventsCookie);
		}
		private Assembly ReflectionHelper_AssemblyResolve(object sender, AssemblyResolveEventArgs args) {
			return typeResolutionService.GetAssembly(args.AssemblyName);
		}
		private void ModelEditorController_CurrentNodeChanged(object sender, EventArgs e) {
			if(FocusChanged != null) {
				FocusChanged(this, EventArgs.Empty);
			}
		}
		private void ModelEditorController_Modifying(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !CanEditFiles(GetEditedFiles());
			SetReadOnly();
		}
		private void modelEditorController_CanSaveBoFiles(object sender, FileModelStoreCancelEventArgs e) {
			e.Cancel = !CanEditFiles(e.BoFiles);
		}
		private void setupCommands() {
			IMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(null != mcs) {
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdForward,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							modelEditorController.Forward();
						}),
					new EventHandler(
						delegate(object sender, EventArgs e) {
							((MenuCommand)sender).Enabled = modelEditorController.CanForward;
						}));
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdBack,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							modelEditorController.Back();
						}),
					new EventHandler(
						delegate(object sender, EventArgs e) {
							((MenuCommand)sender).Enabled = modelEditorController.CanBack;
						}));
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdLanguage,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							OleMenuCmdEventArgs args = (OleMenuCmdEventArgs)e;
							if(args.InValue != null) {
								if(!modelEditorController.ShowCulturesManager((string)args.InValue)) {
									modelEditorController.SetCurrentAspectByName((string)args.InValue);
								}
							}
							if(args.OutValue != IntPtr.Zero) {
								Marshal.GetNativeVariantForObject(modelEditorController.CurrentAspect, args.OutValue);
							}
						}), new EventHandler(
						delegate(object sender, EventArgs e) {
							((MenuCommand)sender).Enabled = modelEditorController.CanChangeAspect;
						}));
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdLanguageGetList,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							List<string> aspectNames = modelEditorController.AllAspectNames;
							string[] items = new string[aspectNames.Count];
							aspectNames.CopyTo(items);
							OleMenuCmdEventArgs args = (OleMenuCmdEventArgs)e;
							Marshal.GetNativeVariantForObject(items, args.OutValue);
						}), null);
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdSearch,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							editorControl.ActivateSearchControl();
						}), null);
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdLocalization,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							DevExpress.LookAndFeel.UserLookAndFeel.Default.UseWindowsXPTheme = true;
							DevExpress.LookAndFeel.UserLookAndFeel.Default.SetWindowsXPStyle();
							modelEditorController.ShowLocalizationForm();
						}), new EventHandler(
						delegate(object sender, EventArgs e) {
							((MenuCommand)sender).Enabled = modelEditorController.CanShowLocalizationForm;
						}));
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdReload,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							modelEditorController.ReloadModel(true, true);
						}), new EventHandler(
						delegate(object sender, EventArgs e) {
							((MenuCommand)sender).Enabled = modelEditorController.IsModified;
						}));
				addCommand(mcs, ConstantList.guidModelEditorCmdSet, (int)CommandIds.icmdShowUnusableData,
					new EventHandler(
						delegate(object sender, EventArgs e) {
							modelEditorController.CalculateUnusableModel();
						}), null);
			}
		}
		private static void addCommand(IMenuCommandService mcs, Guid menuGroup, int cmdID, EventHandler commandEvent, EventHandler queryEvent) {
			CommandID menuCommandID = new CommandID(menuGroup, cmdID);
			OleMenuCommand command = new OleMenuCommand(commandEvent, menuCommandID);
			if(null != queryEvent) {
				command.BeforeQueryStatus += queryEvent;
			}
			mcs.AddCommand(command);
		}
		private bool CanEditFiles(string[] checkFiles) {
			if(gettingCheckoutStatus)
				return false;
			try {
				gettingCheckoutStatus = true;
				return ProjectWrapper.CanEditFile(checkFiles, this);
			}
			finally {
				gettingCheckoutStatus = false;
			}
		}
		private Dictionary<string, string> LoadSettings() {
			Dictionary<string, string> result = new Dictionary<string, string>();
			object modelSettingsProvider = GetService(typeof(DevExpress.ExpressApp.Design.Core.IModelEditorSettings));
			if(modelSettingsProvider != null) {
				MethodInfo loadSettings = DesignCorePackageReflectionHelper.GetMethodInfo(modelSettingsProvider.GetType(), "LoadSettings", new Type[] { typeof(string), typeof(MemoryStream) });
				if(loadSettings != null) {
					MemoryStream settingsStream = new MemoryStream();
					loadSettings.Invoke(modelSettingsProvider, new object[] { fileName, settingsStream });
					if(settingsStream.Length > 0) {
						BinaryFormatter formatter = new BinaryFormatter();
						settingsStream.Seek(0, SeekOrigin.Begin);
						result = formatter.Deserialize(settingsStream) as Dictionary<string, string>;
					}
				}
			}
			return result;
		}
		private void SaveSettings() {
			try {
				if(editorControl != null && !editorControl.IsDisposed) {
					editorControl.OnClosed();
					modelEditorController.SaveSettings();
					SettingsStorageOnHashtable _settings = editorControl.SettingsStorage as SettingsStorageOnHashtable;
					if(_settings != null && _settings.Settings != null) {
						object modelSettingsProvider = GetService(typeof(DevExpress.ExpressApp.Design.Core.IModelEditorSettings));
						if(modelSettingsProvider != null) {
							MethodInfo saveSettings = DesignCorePackageReflectionHelper.GetMethodInfo(modelSettingsProvider.GetType(), "SaveSettings", new Type[] { typeof(string), typeof(MemoryStream) });
							if(saveSettings != null) {
								BinaryFormatter formatter = new BinaryFormatter();
								MemoryStream stream = new MemoryStream();
								stream.Seek(0, SeekOrigin.Begin);
								formatter.Serialize(stream, _settings.Settings);
								saveSettings.Invoke(modelSettingsProvider, new object[] { fileName, stream });
							}
						}
					}
				}
			}
			catch {
			}
		}
		private void SetReadOnly() {
			IVsWindowFrame frame = (IVsWindowFrame)this.GetService(typeof(SVsWindowFrame));
			if(frame != null) {
				ErrorHandler.ThrowOnFailure(frame.SetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, IsEditedFilesReadOnly ? "[Read Only]" : string.Empty));
			}
		}
		private bool IsEditExternalFiles {
			get {
				try {
					string invariantDiffsFileName = projectWrapper.GetInvariantDiffsFileName();
					return Path.GetDirectoryName(invariantDiffsFileName) != Path.GetDirectoryName(fileName);
				}
				catch {
					return false;
				}
			}
		}
		private string[] GetEditedFiles() {
			List<string> result = new List<string>();
			if(IsEditExternalFiles) {
				FileModelStore fileModelStore = new FileModelStore(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
				result.Add(fileName);
				foreach(string aspect in fileModelStore.GetAspects()) {
					string aspectFileName = fileModelStore.GetFileNameForAspect(aspect);
					if(File.Exists(aspectFileName)) {
						result.Add(aspectFileName);
					}
				}
			}
			else {
				foreach(ProjectItem item in projectWrapper.GetDiffsFiles()) {
					result.Add(item.get_FileNames(0));
				}
			}
			return result.ToArray();
		}
		private bool IsEditedFilesReadOnly {
			get {
				foreach(string fileName in GetEditedFiles()) {
					FileAttributes fileAttrs = File.GetAttributes(fileName);
					int isReadOnly = (int)fileAttrs & (int)FileAttributes.ReadOnly;
					if(isReadOnly == 1) {
						return true;
					}
				}
				return false;
			}
		}
		protected override bool PreProcessMessage(ref Message m) {
			if(m.Msg == WM_KEYDOWN) {
				if((int)m.WParam == V_CONTROL) {
					controlPressed = true;
				}
				else if((int)m.WParam == V_TAB && controlPressed) {
					controlPressed = false;
					return false;
				}
			}
			else if(m.Msg == WM_KEYUP) {
				if((int)m.WParam == V_CONTROL) {
					controlPressed = false;
				}
			}
			if(modelEditorController != null) {
				return modelEditorController.PreProcessMessage(m) || base.PreProcessMessage(ref m);
			}
			return base.PreProcessMessage(ref m);
		}
		protected override void Dispose(bool disposing) {
			SetFileChangeNotification(null, false);
			IVsRunningDocumentTable pRDT = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
			if(pRDT != null) {
				if(_rdtEventsCookie != 0) {
					pRDT.UnadviseRunningDocTableEvents(_rdtEventsCookie);
					_rdtEventsCookie = 0;
				}
			}
			SaveSettings();
			FocusChanged = null;
			if(modelEditorController != null) {
				CaptionHelper.RemoveModelApplicationIfNeed(modelEditorController.ModelApplication);
				modelEditorController.CurrentNodeChanged -= new EventHandler(ModelEditorController_CurrentNodeChanged);
				modelEditorController.Modifying -= new System.ComponentModel.CancelEventHandler(ModelEditorController_Modifying);
				modelEditorController.CanSaveBoFiles -= new EventHandler<FileModelStoreCancelEventArgs>(modelEditorController_CanSaveBoFiles);
				modelEditorController.Dispose();
				modelEditorController = null;
			}
			ReflectionHelper.AssemblyResolve -= new AssemblyResolveEventHandler(ReflectionHelper_AssemblyResolve);
			ReflectionHelper.TypeResolve -= new TypeResolveEventHandler(ReflectionHelper_TypeResolve);
			base.Dispose(disposing);
			if(modelForm != null) {
				modelForm.Dispose();
				modelForm = null;
			}
			if(error != null) {
				error.BuildProjectClicked -= new EventHandler(error_BuildProjectClicked);
				error.Dispose();
				error = null;
			}
			currentProjectItem = null;
			if(fileChangeTimer != null) {
				fileChangeTimer.Tick -= new EventHandler(this.OnFileChangeEvent);
				fileChangeTimer.Dispose();
				fileChangeTimer = null;
			}
			if(editorControl != null) {
				editorControl.Dispose();
				editorControl = null;
			}
			dte = null;
			projectWrapper = null;
			openingModelEditorLog = null;
			typeResolutionService = null;
			if(obj != null) {
				obj.Dispose();
				obj = null;
			}
		}
		static EditorPane() {
			System.ComponentModel.LicenseManager.CurrentContext = new XafDesignerDesigntimeLicenseContext();
			XafDesignerHelper.ShowAboutProductsEx();
		}
		public EditorPane(System.IServiceProvider serviceProvider, string fileName, IVsHierarchy pvHier, uint itemid, Microsoft.VisualStudio.OLE.Interop.IServiceProvider comServiceProvider)
			: base(serviceProvider) {
			this.comServiceProvider = comServiceProvider;
			traceGuid = Guid.NewGuid();
			string logFileName = Path.ChangeExtension(fileName, ".log");
			Tracing.Initialize(traceGuid, logFileName);
			object currentProjectItem;
			pvHier.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ExtObject, out currentProjectItem);
			PrivateInit(fileName, (ProjectItem)currentProjectItem, pvHier);
		}
		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, System.IntPtr pCmdText) {
			Debug.Assert(cCmds == 1, "Multiple commands");
			Debug.Assert(prgCmds != null, "NULL argument");
			if((prgCmds == null)) {
				return VSConstants.E_INVALIDARG;
			}
			OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;
			if(pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
				switch(prgCmds[0].cmdID) {
					case (uint)VSConstants.VSStd97CmdID.NewWindow: {
						cmdf |= OLECMDF.OLECMDF_INVISIBLE;
						break;
					}
					default: {
						return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
					}
				}
			}
			else if(pguidCmdGroup == ConstantList.guidModelEditorCmdSet) {
				IOleCommandTarget service = (IOleCommandTarget)this.GetService(typeof(IOleCommandTarget));
				if(service != null) {
					return service.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
				}
				return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
			}
			else {
				return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
			}
			prgCmds[0].cmdf = (uint)cmdf;
			return VSConstants.S_OK;
		}
		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, System.IntPtr pvaIn, System.IntPtr pvaOut) {
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Exec() of: {0}", this.ToString()));
			if(pguidCmdGroup == ConstantList.guidModelEditorCmdSet) {
				IOleCommandTarget service = (IOleCommandTarget)this.GetService(typeof(IOleCommandTarget));
				if(service != null) {
					return service.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
				}
			}
			return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
		}
		public override IWin32Window Window {
			get { return ModelEditorForm; }
		}
		#region IEditorNavigation
		public void NavigateTo(string text) {
		}
		public string GetState() {
			return "";
		}
		public void SetValue(string name, object value) {
			if(modelEditorController.CurrentModelNode != null) {
				if(((ModelNode)modelEditorController.CurrentModelNode.ModelNode).HasValue(name)) {
					if(value != null) {
						((ModelNode)modelEditorController.CurrentModelNode.ModelNode).SetValue(name, value);
					}
					else {
						((ModelNode)modelEditorController.CurrentModelNode.ModelNode).SetValue(name, null);
					}
					modelEditorController.RefreshCurrentAttributeValue();
				}
			}
		}
		public event EventHandler FocusChanged;
		#endregion
		#region IVsFindTarget Members
		private object findState;				   
		bool isNewSearch;
		int IVsFindTarget.Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult) {
			object node = this.editorControl.modelTreeList.FocusedNode;
			SearchNodeOptions options = SearchNodeOptions.None;
			if((grfOptions & (uint)__VSFINDOPTIONS.FR_MatchCase) != 0) {
				options |= SearchNodeOptions.CaseSensitive;
			}
			if((grfOptions & (uint)__VSFINDOPTIONS.FR_WholeWord) != 0) {
				options |= SearchNodeOptions.WholeWord;
			}
			if((grfOptions & (uint)__VSFINDOPTIONS.FR_Backwards) != 0) {
				options |= SearchNodeOptions.SearchUp;
			}
			bool isFound = modelEditorController.FindAndFocusEntry(!isNewSearch, pszSearch, options);
			if(!isFound) {
				if(isNewSearch) {
					pResult = (uint)__VSFINDRESULT.VSFR_NotFound;
				}
				else {
					pResult = (uint)__VSFINDRESULT.VSFR_EndOfDoc;
				}
			}
			else {
				pResult = (uint)__VSFINDRESULT.VSFR_Found;
			}
			isNewSearch = false;
			return VSConstants.S_OK;
		}
		int IVsFindTarget.GetCapabilities(bool[] pfImage, uint[] pgrfOptions) {
			if(pfImage != null) {
				if(pfImage.Length > 0) {
					pfImage[0] = false;
				}
			}
			if(pgrfOptions != null) {
				if(pgrfOptions.Length > 0) {
					pgrfOptions[0] = (uint)(__VSFINDOPTIONS.FR_Find |
						__VSFINDOPTIONS.FR_Document | 
						__VSFINDOPTIONS.FR_Backwards |
						__VSFINDOPTIONS.FR_WholeWord |
						__VSFINDOPTIONS.FR_MatchCase);
				}
			}
			return VSConstants.S_OK;
		}
		int IVsFindTarget.GetCurrentSpan(TextSpan[] pts) {
			return VSConstants.E_NOTIMPL;
		}
		int IVsFindTarget.GetFindState(out object ppunk) {
			ppunk = findState;
			return VSConstants.S_OK;
		}
		int IVsFindTarget.GetMatchRect(RECT[] prc) {
			return VSConstants.E_NOTIMPL;
		}
		int IVsFindTarget.GetProperty(uint propid, out object pvar) {
			pvar = null;
			switch(propid) {
				case (uint)__VSFTPROPID.VSFTPROPID_DocName: {
					pvar = fileName;
					break;
				}
				case (uint)__VSFTPROPID.VSFTPROPID_InitialPattern:
				case (uint)__VSFTPROPID.VSFTPROPID_InitialPatternAggressive: {
					GetInitialSearchString(out pvar);
					isNewSearch = true;
					pvar = null;
					break;
				}
				case (uint)__VSFTPROPID.VSFTPROPID_WindowFrame: {
					pvar = (IVsWindowFrame)GetService(typeof(SVsWindowFrame));
					break;
				}
				case (uint)__VSFTPROPID.VSFTPROPID_IsDiskFile: {
					pvar = true;
					break;
				}
				default: {
					return VSConstants.E_NOTIMPL;
				}
			}
			return VSConstants.S_OK;
		}
		private void GetInitialSearchString(out object pvar) {
			pvar = null;
		}
		int IVsFindTarget.GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage) {
			ppTextImage = null;
			return VSConstants.E_NOTIMPL;
		}
		int IVsFindTarget.MarkSpan(TextSpan[] pts) {
			return VSConstants.E_NOTIMPL;
		}
		int IVsFindTarget.NavigateTo(TextSpan[] pts) {
			throw new Exception("The method or operation is not implemented.");
		}
		int IVsFindTarget.NotifyFindTarget(uint notification) {
			return VSConstants.S_OK;
		}
		int IVsFindTarget.Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced) {
			pfReplaced = 0;
			return VSConstants.E_NOTIMPL;
		}
		int IVsFindTarget.SetFindState(object pUnk) {
			findState = pUnk;
			return VSConstants.S_OK;
		}
		#endregion
		#region IVsPersistDocData Members
		int IVsPersistDocData.Close() {
			this.Dispose(true);
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.GetGuidEditorType(out Guid pClassID) {
			pClassID = ConstantList.guidEditorFactory;
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.IsDocDataDirty(out int pfDirty) {
			pfDirty = 0;
			if((modelEditorController != null) && modelEditorController.IsModified) {
				pfDirty = 1;
			}
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable) {
			pfReloadable = 0;
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.LoadDocData(string pszMkDocument) {
			SetReadOnly();
			SetFileChangeNotification(pszMkDocument, true);
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew) {
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.ReloadDocData(uint grfFlags) {
			if(modelEditorController != null ) {
				modelEditorController.ReloadModel(false, true);
			}
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) {
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled) {
			pbstrMkDocumentNew = fileName;
			pfSaveCanceled = 0;
			BindingHelper.EndCurrentEdit((Control)editorControl.ModelAttributesEditor);
			ModelSaver modelSaver = new ModelSaver(this, projectWrapper, modelEditorController);
			SetFileChangeNotification(null, false);
			modelSaver.Save();
			SetFileChangeNotification(pbstrMkDocumentNew, true);
			return VSConstants.S_OK;
		}
		int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath) {
			return VSConstants.S_OK;
		}
		#endregion
		#region IExtensibleObject Members
		void IExtensibleObject.GetAutomationObject(string Name, IExtensibleObjectSite pParent, out object ppDisp) {
			if(!string.IsNullOrEmpty(Name) && !Name.Equals("Document", StringComparison.CurrentCultureIgnoreCase)) {
				ppDisp = null;
				return;
			}
			ppDisp = (IEditorNavigation)this;
		}
		#endregion
		#region IVsRunningDocTableEvents Members
		int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs) {
			return VSConstants.S_OK;
		}
		int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) {
			return VSConstants.S_OK;
		}
		int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			return VSConstants.S_OK;
		}
		int IVsRunningDocTableEvents.OnAfterSave(uint docCookie) {
			return VSConstants.S_OK;
		}
		int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) {
			return VSConstants.S_OK;
		}
		int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			return VSConstants.S_OK;
		}
		#endregion
		#region IPersistFileFormat Members
		public int GetClassID(out Guid pClassID) {
			throw new NotImplementedException();
		}
		public int GetCurFile(out string ppszFilename, out uint pnFormatIndex) {
			ppszFilename = fileName;
			pnFormatIndex = 0;
			return VSConstants.S_OK;
		}
		public int GetFormatList(out string ppszFormatList) {
			ppszFormatList = "XAFML File (*.xafml)\n*.xafml\n.";
			return VSConstants.S_OK;
		}
		public int InitNew(uint nFormatIndex) {
			return VSConstants.S_OK;
		}
		public int IsDirty(out int pfIsDirty) {
			return ((IVsPersistDocData)this).IsDocDataDirty(out pfIsDirty);
		}
		public int Load(string pszFilename, uint grfMode, int fReadOnly) {
			return VSConstants.S_OK;
		}
		public int Save(string pszFilename, int fRemember, uint nFormatIndex) {
			return VSConstants.S_OK;
		}
		public int SaveCompleted(string pszFilename) {
			return VSConstants.S_OK;
		}
		#endregion
		private int SetFileChangeNotification(string fileName, bool isStart) {
			int result = VSConstants.E_FAIL;
			if(fileChangeExService == null) {
				fileChangeExService = (IVsFileChangeEx)GetService(typeof(SVsFileChangeEx));
			}
			if(fileChangeExService == null)
				return VSConstants.E_UNEXPECTED;
			if(isStart) {
				if(fileChangeCookie == VSConstants.VSCOOKIE_NIL) {
					result = fileChangeExService.AdviseFileChange(fileName,
						(uint)(_VSFILECHANGEFLAGS.VSFILECHG_Attr | _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time),
						this, out fileChangeCookie);
					if(fileChangeCookie == VSConstants.VSCOOKIE_NIL)
						return VSConstants.E_FAIL;
				}
			}
			else {
				if(fileChangeCookie != VSConstants.VSCOOKIE_NIL) {
					result = fileChangeExService.UnadviseFileChange(fileChangeCookie);
					fileChangeCookie = VSConstants.VSCOOKIE_NIL;
				}
			}
			return result;
		}
		private void OnFileChangeEvent(object sender, System.EventArgs e) {
			fileChangeTimer.Enabled = false;
			IVsUIShell vsUiShellService = (IVsUIShell)GetService(typeof(SVsUIShell));
			int result = 0;
			Guid tempGuid = Guid.Empty;
			if(vsUiShellService != null) {
				vsUiShellService.ShowMessageBox(0, ref tempGuid, fileName, "File has been changed outside the environment. Reload the new file?", null, 0,
					OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
					OLEMSGICON.OLEMSGICON_QUERY, 0, out result);
			}
			if(result == (int)DialogResult.Yes) {
				((IVsPersistDocData)this).ReloadDocData(0);
			}
			fileChangeTimerSet = false;
		}
		#region IVsFileChangeEvents Members
		public int DirectoryChanged(string pszDirectory) {
			return VSConstants.S_OK;
		}
		public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange) {
			if(0 == cChanges || null == rgpszFile || null == rggrfChange)
				return VSConstants.E_INVALIDARG;
			for(uint i = 0; i < cChanges; i++) {
				if(!String.IsNullOrEmpty(rgpszFile[i]) && string.Compare(rgpszFile[i], fileName, true) == 0) {
					if((rggrfChange[i] & (int)_VSFILECHANGEFLAGS.VSFILECHG_Attr) != 0) {
						SetReadOnly();
					}
					if((rggrfChange[i] & (int)(_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size)) != 0) {
						if(!fileChangeTimerSet) {
							fileChangeTimer = new Timer();
							fileChangeTimerSet = true;
							fileChangeTimer.Interval = 1000;
							fileChangeTimer.Tick += new EventHandler(this.OnFileChangeEvent);
							fileChangeTimer.Enabled = true;
						}
					}
				}
			}
			return VSConstants.S_OK;
		}
		#endregion
	}
	public class ModelEditorForm : Form, ISupportToolTipsForm {
		#region ISupportToolTipsForm Members
		public bool ShowToolTipsWhenInactive {
			get { return true; }
		}
		public bool ShowToolTipsFor(Form form) { return false; }
		#endregion
	}
}
