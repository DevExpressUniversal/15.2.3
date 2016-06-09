#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Utils;
using Microsoft.CSharp;
namespace DevExpress.XtraLayout.Customization.Templates {
	public class TemplateManager :IXtraSerializable {
		protected bool CanRestoreTemplate(string path) {
			if(string.IsNullOrEmpty(path)) return false;
			if(Assembly.GetExecutingAssembly().GetManifestResourceStream(path) != null)
				return Assembly.GetExecutingAssembly().GetManifestResourceStream(path).CanRead;
			if(File.Exists(path)) return true;
			return false;
		}
		public static bool CanMakeTemplate(List<BaseLayoutItem> items) {
			if(items == null || items.Count == 0) return false;
			if(!CheckRectangleArea(items)) return false;
			return true;
		}
		static bool CheckRectangleArea(List<BaseLayoutItem> items) {
			List<BaseLayoutItem> listBLI = new List<BaseLayoutItem>();
			foreach(BaseLayoutItem bli in items) {
				if(bli.ParentName == items[0].ParentName && bli.Parent == items[0].Parent)
					listBLI.Add(bli);
			}
			Point leftPoint = new Point(int.MaxValue, int.MaxValue);
			Point rightPoint = new Point(int.MinValue, int.MinValue);
			foreach(BaseLayoutItem item in listBLI) {
				if(item.Location.X <= leftPoint.X && item.Location.Y <= leftPoint.Y) leftPoint = item.Location;
				if(item.Location.X + item.Size.Width >= rightPoint.X) rightPoint.X = item.Location.X + item.Size.Width;
				if(item.Location.Y + item.Size.Height >= rightPoint.Y) rightPoint.Y = item.Location.Y + item.Size.Height;
			}
			double areaRectangular = (rightPoint.X - leftPoint.X) * (rightPoint.Y - leftPoint.Y);
			foreach(BaseLayoutItem item in listBLI) {
				areaRectangular -= item.Size.Height * item.Size.Width;
			}
			if(areaRectangular != 0) return false;
			else return true;
		}
		public TemplateManager() {
			Items = new List<BaseLayoutItem>();
			ControlsInfo = new ControlData();
		}
		public static List<BaseLayoutItem> GetSelectedItemsWithChildren(List<BaseLayoutItem> items) {
			List<BaseLayoutItem> result = new List<BaseLayoutItem>();
			FlatItemsList fil = new FlatItemsList();
			foreach(BaseLayoutItem item in items) {
				if(item.Selected) result.AddRange(fil.GetItemsList(item));
			}
			return result;
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, true)]
		public List<BaseLayoutItem> Items { get; set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ControlData ControlsInfo { get; set; }
		[XtraSerializableProperty]
		public string Name { get; set; }
		internal bool IsStandartTemplate = false;
		public static BaseItemCollection GetTemplateCollection() {
			BaseItemCollection bic = new BaseItemCollection();
			List<string> fNames = GetPathList();
			TemplateManager tManager;
			foreach(string templatePath in fNames) {
				tManager = new TemplateManager();
				tManager.RestoreTemplate(templatePath, null, null);
				if(tManager != null) {
					bic.Add(new LayoutControlItem(null) { Text = tManager.Name, Tag = tManager,Name = templatePath });
				}
			}
			return bic;
		}
		internal static int GetDefaultTemplateCount() {
			string[] names = typeof(TemplateManager).Assembly.GetManifestResourceNames();
			int value = 0;
			foreach(string name in names) {
				if(name.StartsWith(TemplateString.EmbeddedResourceXML)) {
					value++;
				}
			}
			return value;
		}
		static List<string> GetPathList() {
			string[] names = typeof(TemplateManager).Assembly.GetManifestResourceNames();
			List<string> fNames = new List<string>();
			foreach(string name in names) {
				if(name.StartsWith(TemplateString.EmbeddedResourceXML)) {
					fNames.Add(name);
				}
			}
			string dirName = TemplateString.PathToTemplate;
			if(Directory.Exists(dirName)) {
				names = Directory.GetFiles(dirName, "*.xml", SearchOption.TopDirectoryOnly);
				fNames.AddRange(names);
			}
			return fNames;
		}
		public void CreateTemplate(LayoutControl layout) {
			TemplateMangerAskNameForm askNameForm = new TemplateMangerAskNameForm(this, layout);
			askNameForm.ShowDialog();
		}
		public void CreateTemplate(string name, List<BaseLayoutItem> items, ControlData control, string path) {
			CreateTemplate(name, items, control, path, true);
		}
		public void CreateTemplate(string name, List<BaseLayoutItem> items, ControlData control, string path, bool checkRectangleArea) {
			if(CanMakeTemplate(items) || !checkRectangleArea) {
				List<BaseLayoutItem> selectedItems = GetSelectedItemsWithChildren(items);
				string templateName = name;
				using(FileStream fs = new FileStream(path, FileMode.Create)) {
					MakeTemplateCore(templateName, selectedItems, control, fs);
				}
			}
		}
		public void RestoreTemplatePreview(string path, LayoutControl layoutControl, LayoutItemDragController dragController, bool debugInfo = false) {
			RestoreTemplateCore(path, layoutControl, dragController, true, debugInfo);
		}
		public void RestoreTemplate(string path, LayoutControl layoutControl, LayoutItemDragController dragController) {
			RestoreTemplateCore(path, layoutControl, dragController, false, false);
		}
		void RestoreTemplateCore(string path, LayoutControl restoreLayoutControl, LayoutItemDragController dragController, bool previewTemplate, bool debugInfo) {
			if(CanRestoreTemplate(path) && Assembly.GetExecutingAssembly().GetManifestResourceStream(path) != null) {
				using(Stream fs = Assembly.GetExecutingAssembly().GetManifestResourceStream(path)) {
					RestoreTemplateCore(fs);
				}
				IsStandartTemplate = true;
			}
			if(CanRestoreTemplate(path) && File.Exists(path)) {
				using(FileStream fs = new FileStream(path, FileMode.Open)) {
					RestoreTemplateCore(fs);
				}
			}
			if(dragController != null && Items.Count > 0) {
				LayoutControlGroup lg = restoreLayoutControl.Root.AddGroup();
				lg.GroupBordersVisible = false;
				lg.Tag = TemplateString.LayoutGroupForRestoreName;
				List<BaseLayoutItem> listLayoutItem = new List<BaseLayoutItem>();
				ChangeAssemblyVersion(ControlsInfo);
				foreach(BaseLayoutItem tempItem in Items) listLayoutItem.Add(tempItem.Clone(null, null));
				string parentName = listLayoutItem[0].ParentName;
				Point leftTopPoint = FindLeftTopPoint(parentName);
				if(restoreLayoutControl != null) AssignControlToItems(listLayoutItem, restoreLayoutControl, previewTemplate, debugInfo);
				PatchItemLocation(listLayoutItem, leftTopPoint, parentName);
				LayoutItemDragController dragControllerNew = new LayoutItemDragController(lg, dragController);
				dragControllerNew.Drag();
				if(!previewTemplate) PatchRestoreItemsNames(restoreLayoutControl, listLayoutItem);
				lg.BeginUpdate();
				FillItemToGroup(lg, listLayoutItem, parentName);
				lg.EndUpdate();
				lg.Selected = true;
				if(previewTemplate) lg.Parent.UngroupSelected();
			}
		}
		void PatchRestoreItemsNames(LayoutControl restoreLayoutControl, List<BaseLayoutItem> listLayoutItem) {
			if(restoreLayoutControl == null) return;
			List<string> names = new List<string>();
			List<RenameGroupHelper> renameGroupList = new List<RenameGroupHelper>();
			foreach(BaseLayoutItem item in listLayoutItem) {
				PatchName(restoreLayoutControl, item, names, renameGroupList);
			}
			foreach(BaseLayoutItem item in listLayoutItem) {
				PatchParentName(item, renameGroupList);
			}
		}
		void PatchName(LayoutControl layoutControl, BaseLayoutItem item, List<string> names, List<RenameGroupHelper> renameGroupList) {
			if(item is TabbedGroup) {
				PatchGroup(layoutControl, item, names, renameGroupList);
				return;
			}
			if(item is LayoutGroup) {
				PatchTabbedParentName(renameGroupList, item as LayoutGroup);
				PatchGroup(layoutControl, item, names, renameGroupList);
				return;
			}
			item.Name = PatchItemName(layoutControl, item.Name, false, names);
			names.Add(item.Name);
			if(item is LayoutControlItem && (item as LayoutControlItem).Control != null) PatchControlName(layoutControl, item, names);
		}
		static void PatchTabbedParentName(List<RenameGroupHelper> renameGroupList, LayoutGroup group) {
			if(!String.IsNullOrEmpty(group.TabbedGroupParentName)) {
				foreach(RenameGroupHelper gHelp in renameGroupList) {
					if(group.TabbedGroupParentName == gHelp.PreviousName) {
						group.TabbedGroupParentName = gHelp.RealName;
						break;
					}
				}
			}
		}
		static void PatchParentName(BaseLayoutItem item, List<RenameGroupHelper> renameGroupList) {
			if(!String.IsNullOrEmpty(item.ParentName)) {
				foreach(RenameGroupHelper gHelp in renameGroupList) {
					if(item.ParentName == gHelp.PreviousName) {
						item.ParentName = gHelp.RealName;
						break;
					}
				}
			}
			if(item is LayoutGroup && (item as LayoutGroup).TabbedGroupParentName != null) {
				foreach(RenameGroupHelper gHelp in renameGroupList) {
					if(gHelp.PreviousName == (item as LayoutGroup).TabbedGroupParentName || gHelp.RealName == (item as LayoutGroup).TabbedGroupParentName) return;
				}
				(item as LayoutGroup).TabbedGroupParentName = "";
			}
		}
		static void PatchGroup(LayoutControl layoutControl, BaseLayoutItem item, List<string> names, List<RenameGroupHelper> renameGroupList) {
			string prevName = item.Name;
			item.Name = PatchItemName(layoutControl, item.Name, false, names);
			names.Add(item.Name);
			renameGroupList.Add(new RenameGroupHelper(prevName, item.Name));
		}
		static void PatchControlName(LayoutControl layoutControl, BaseLayoutItem item, List<string> names) {
			string tryName = (item as LayoutControlItem).Control.Name;
			(item as LayoutControlItem).Control.Name = PatchItemName(layoutControl, tryName == String.Empty ? item.Name : tryName, true, names);
			names.Add((item as LayoutControlItem).Control.Name);
		}
		static void FillItemToGroup(LayoutControlGroup lg, List<BaseLayoutItem> listLayoutItem, string parentName) {
			listLayoutItem.Add(lg);
			lg.Name = TemplateString.LayoutGroupForRestoreName;
			for(int i = 0; i < listLayoutItem.Count; i++) {
				if(listLayoutItem[i].ParentName == parentName && listLayoutItem[i].Tag as string != lg.Name) {
					listLayoutItem[i].ParentName = lg.Name;
				}
				LayoutGroup group = listLayoutItem[i] as LayoutGroup;
				TabbedGroup tgroup = listLayoutItem[i] as TabbedGroup;
				if(group != null) {
					if(!CheckGroupOnParentTabbedGroup(listLayoutItem, group) && !CheckGroupOnParentGroup(listLayoutItem, group) && i != listLayoutItem.Count - 1) {
						group.ParentName = lg.Name;
						group.TabbedGroupParentName = null;
					}
					group.RestoreChildren(new BaseItemCollection(listLayoutItem.ToArray()));
				}
				if(tgroup != null) {
					tgroup.RestoreChildren(new BaseItemCollection(listLayoutItem.ToArray()));
				}
			}
		}
		static bool CheckGroupOnParentGroup(List<BaseLayoutItem> listLayoutItem, LayoutGroup group) {
			bool groupOnParentGroup = false;
			foreach(BaseLayoutItem item in listLayoutItem) {
				if(group.ParentName == item.Name) groupOnParentGroup = true;
			}
			return groupOnParentGroup;
		}
		static bool CheckGroupOnParentTabbedGroup(List<BaseLayoutItem> listLayoutItem, LayoutGroup group) {
			bool tabbedGroupParent = false;
			foreach(BaseLayoutItem item in listLayoutItem) {
				TabbedGroup tabgroup = item as TabbedGroup;
				if(tabgroup != null) {
					if(group.TabbedGroupParentName == tabgroup.Name) {
						tabbedGroupParent = true;
					}
				}
			}
			return tabbedGroupParent;
		}
		void AssignControlToItems(List<BaseLayoutItem> listLayoutItem, LayoutControl layoutControl, bool preview, bool debugInfo) {
			if(ControlsInfo.Assembly == null) return;
			CreateControl(ControlsInfo, preview, layoutControl, listLayoutItem, debugInfo);
		}
		void CreateControl(ControlData controlData, bool previewControl, LayoutControl layoutControl, List<BaseLayoutItem> listLayoutItem, bool debugInfo) {
			IComponent currentControl = new Control();
			CodeDomProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters();
			List<string> assemblyUserPath = new List<string>();
			foreach(string assemblyName in controlData.Assembly) {
				try {
					Assembly userAssembly = Assembly.Load(assemblyName);
					assemblyUserPath.Add(userAssembly.Location);
				} catch { }
			}
			parameters.ReferencedAssemblies.AddRange(assemblyUserPath.ToArray());
			Type initializeMethodProviderType;
			IComponent initializeMethodProvider;
			CompilerResults results = TryCompileTemplate(controlData, provider, parameters, debugInfo);
			try {
			initializeMethodProviderType = results.CompiledAssembly.GetTypes()[0];
			} catch(Exception tExc) {
				if(tExc.InnerException != null) throw new TemplateException(tExc.InnerException.ToString());
				else throw new TemplateException(tExc.ToString());
			}
			currentControl = Activator.CreateInstance(initializeMethodProviderType.BaseType) as IComponent;
			MethodInfo mi = initializeMethodProviderType.GetMethod("InitializeInstance", BindingFlags.Public | BindingFlags.Instance);
			initializeMethodProvider = Activator.CreateInstance(initializeMethodProviderType) as IComponent;
			try {
				if(mi != null) mi.Invoke(initializeMethodProvider, new object[] { currentControl, controlData.Resources });
			} catch(Exception tExc) {
				WriteDebugInfo(controlData, tExc.ToString());
				if(tExc.InnerException != null) throw new TemplateException(tExc.InnerException.ToString());
				else throw new TemplateException(tExc.ToString());
			}
			ILayoutControl controlOwner = layoutControl as ILayoutControl;
			List<IComponent> componentsToAdd = new List<IComponent>();
			foreach(FieldInfo fi in initializeMethodProviderType.GetFields(BindingFlags.Instance | BindingFlags.Public)) {
				IComponent currentField = fi.GetValue(initializeMethodProvider) as IComponent;
				if(currentField != null) componentsToAdd.Add(currentField);
				var controlToLayout = currentField as Control;
				if(controlToLayout != null) {
					foreach(BaseLayoutItem item in listLayoutItem) {
						var lci = item as LayoutControlItem;
						if(lci == null) continue;
						if(lci.ControlName == controlToLayout.Name) {
							controlToLayout.DataBindings.Clear();
							lci.Control = controlToLayout;
						}
					}
				}
			}
			if(!previewControl) {
				AddNewReferenceAssemblyToProject(controlData, controlOwner);
				AddNewResourceToProject(controlData, controlOwner);
				foreach(IComponent component in componentsToAdd) {
					AddControlToDesignSurface(component, controlOwner);
				}
			}
		}
		private void AddNewResourceToProject(ControlData controlData, ILayoutControl controlOwner) {
			ResXResourceWriter resXResourceWriter = GetResourceWriter(controlOwner.Site.Container);
			if(resXResourceWriter == null) return;
			if(controlData.Resources == null) return;
			DXTemplateResourceManager resManager = new DXTemplateResourceManager(controlData.Resources);
			foreach(DictionaryEntry d in resManager.GetReader()) {
				resXResourceWriter.AddResource(d.Key.ToString(), d.Value);
			}
		}
		public static ResXResourceWriter GetResourceWriter(IContainer container) {
			IDesignerHost iDesignerHost = DevExpress.XtraDataLayout.LayoutCreator.GetIDesignerHost(container);
			if(iDesignerHost == null) return null;
			IResourceService iService = iDesignerHost.GetService(typeof(IResourceService)) as IResourceService;
			if(iService == null) return null;
			IResourceWriter iWriter = iService.GetResourceWriter(System.Globalization.CultureInfo.InvariantCulture);
			if(iWriter == null) return null;
			return iWriter as ResXResourceWriter;
		}
		static void AddNewReferenceAssemblyToProject(ControlData controlData, ILayoutControl controlOwner) {
			if(controlOwner == null || controlOwner.Site == null || controlOwner.Site.Container == null) return;
			IDesignerHost iDesignerHost = DevExpress.XtraDataLayout.LayoutCreator.GetIDesignerHost(controlOwner.Site.Container);
			if(iDesignerHost == null) return;
			ITypeResolutionService typeResolutionService = (ITypeResolutionService)iDesignerHost.GetService(typeof(ITypeResolutionService));
			if(typeResolutionService == null) return;
			foreach(string assemblyName in controlData.Assembly) {
				try {
					Assembly userAssembly = Assembly.Load(assemblyName);
					typeResolutionService.ReferenceAssembly(userAssembly.GetName());
				} catch { }
			}
		}
		string PatchAssemblyName(string assemblyName) {
			if(assemblyName.StartsWith(TemplateString.DevExpress)) {
				AssemblyName aName = Assembly.GetExecutingAssembly().GetName();
				string returnString = String.Empty;
				if(assemblyName.Contains(TemplateString.ReplaceVersion)) {
					returnString = assemblyName.Replace(TemplateString.ReplaceVersion, aName.Version.Major + "." + aName.Version.Minor);
				} else {
					returnString = ChangeAssembly(assemblyName, GetStartIndexVersion(assemblyName, TemplateString.LongVersion), AssemblyInfo.Version);
					returnString = ChangeAssembly(assemblyName, GetStartIndexVersion(assemblyName, TemplateString.ShortVersion), AssemblyInfo.VersionShort);
				}
				string publicKey = GetPublicKey(aName);
				returnString = returnString.Remove(returnString.LastIndexOf(TemplateString.PublicKey));
				returnString += publicKey;
				return returnString;
			}
			return assemblyName;
		}
		static string GetPublicKey(AssemblyName aName) {
			int FirstIndex = aName.FullName.LastIndexOf(TemplateString.PublicKey);
			string publicKey = aName.FullName.Remove(0, FirstIndex);
			return publicKey;
		}
		CompilerResults TryCompileTemplate(ControlData controlData, CodeDomProvider provider, CompilerParameters parameters, bool debuginfo) {
			int watchDog = -1;
			CompilerResults results;
			CodeSnippetCompileUnit compileUnit;
			while(true) {
				compileUnit = new CodeSnippetCompileUnit(controlData.Data);
				results = provider.CompileAssemblyFromDom(parameters, compileUnit);
				watchDog++;
				if(results.Errors.HasErrors) {
					controlData.Data = GetCodeWithoutErrorLine(controlData.Data, results.Errors);
					if(watchDog > 30) {
						if(debuginfo) WriteDebugInfo(controlData, GetErrorString(results));
						throw new TemplateException(results.Errors[0].ErrorText);
					}
				} else break;
			}
			return results;
		}
		private static string GetErrorString(CompilerResults results) {
			string temp = "";
			foreach(CompilerError error in results.Errors) {
				temp += error.ToString() + Environment.NewLine;
			}
			return temp;
		}
		private void WriteDebugInfo(ControlData controlData, string Error) {
			if(!Directory.Exists(TemplateString.PathToDebugInfo)) Directory.CreateDirectory(TemplateString.PathToDebugInfo);
			string fullPath = TemplateMangerAskNameForm.GetUniqueFileName(TemplateString.PathToDebugInfo, Name);
			using(FileStream fs = new FileStream(fullPath, FileMode.Create)) {
				MakeTemplateCore(Name, Items, controlData, fs);
			}
			using(StreamWriter sw = File.AppendText(fullPath)) {
				sw.Write(Environment.NewLine);
				sw.Write(Error);
			}
		}
		private static bool AddLine(CompilerErrorCollection errors, int i) {
			foreach(CompilerError error in errors) {
				if(error.Line - 1 == i) return false;
			}
			return true;
		}
		string GetCodeWithoutErrorLine(string codeString, CompilerErrorCollection errors) {
			var lines = Regex.Split(codeString, "\r\n|\r|\n").ToArray();
			string temp = "";
			for(int i = 0; i < lines.Count(); i++) {
				if(AddLine(errors, i)) temp += lines[i] + Environment.NewLine;
				else temp += String.Format("//{0}{1}", lines[i], Environment.NewLine);
			}
			return temp;
		}
		void ChangeAssemblyVersion(ControlData controlData) {
			if(controlData == null) return;
			if(controlData.Assembly == null) return;
			for(int i = 0; i < controlData.Assembly.Count; i++) {
				string tempAssembly = controlData.Assembly[i];
				tempAssembly = PatchAssemblyName(tempAssembly);
				if(tempAssembly.StartsWith(TemplateString.Nunit)) {
					controlData.Assembly.Remove(controlData.Assembly[i]);
					i--;
					continue;
				}
				controlData.Assembly[i] = tempAssembly;
			}
			ControlsInfo = controlData;
		}
		static string ChangeAssembly(string assembly, int startindex, string Version) {
			string oldVersion = assembly.Substring(startindex, Version.Count());
			assembly = assembly.Replace(oldVersion, Version);
			return assembly;
		}
		static int GetStartIndexVersion(string assembly, string value) {
			int startindex = assembly.IndexOf(value) + value.Count(); ;
			return startindex;
		}
		void UpdateTemplateFileFromPath(string path) {
			using(FileStream fs = new FileStream(path, FileMode.Open)) {
				fs.SetLength(0);
				MakeTemplateCore(Name, Items, ControlsInfo, fs);
			}
		}
		static void AddControlToDesignSurface(IComponent currentcontrol, ILayoutControl controlOwner) {
			if(currentcontrol != null) {
				if(controlOwner.Site != null) {
					ArrayList components = new ArrayList(controlOwner.Site.Container.Components);
					if(!components.Contains(currentcontrol) && ((ILayoutControl)controlOwner).AllowManageDesignSurfaceComponents && AllowAdd(controlOwner.Site.Container.Components, currentcontrol)) {
						PatchPictureEdit(currentcontrol);
						controlOwner.Site.Container.Add(currentcontrol);
					}
				}
			}
		}
		static bool AllowAdd(ComponentCollection components, IComponent currentcontrol) {
			if(currentcontrol is BindingSource) return false;
			return true;
		}
		static void PatchPictureEdit(IComponent currentcontrol) {
			if(currentcontrol is PictureEdit)
				(currentcontrol as PictureEdit).Cursor = null;
		}
		public static string PatchItemName(LayoutControl layout, string tryName, bool control, List<string> listPrevNames) {
			if(tryName == null) return "item";
			Hashtable hashTableNames = FillExistNames(layout, control, listPrevNames);
			string nameCore = GetTitleCase(tryName);
			nameCore = FindUniqueName(hashTableNames, nameCore);
			return nameCore;
		}
		static string FindUniqueName(Hashtable hashTableNames, string nameCore) {
			int count = 1;
			if(nameCore != null)
				while(hashTableNames.ContainsKey(nameCore)) {
					string qw = "1234567890:?";
					nameCore = String.Format("{0}{1}", nameCore.TrimEnd(qw.ToCharArray()), count);
					count++;
				}
			return nameCore;
		}
		static string GetTitleCase(string tryName) {
			string nameCore;
			if(tryName.Contains(" ")) nameCore = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tryName.ToLower());
			else nameCore = tryName;
			return nameCore.Replace(" ", String.Empty); ;
		}
		static Hashtable FillExistNames(LayoutControl layout, bool control, List<string> listPrevNames) {
			FlatItemsList helper = new FlatItemsList();
			Hashtable hashTableNames = new Hashtable();
			List<BaseLayoutItem> listBLI = helper.GetItemsList(layout.Root);
			foreach(string prevNames in listPrevNames) hashTableNames.Add(prevNames, null);
			foreach(BaseLayoutItem item in listBLI) {
				if(control) {
					if(item is LayoutControlItem) {
						if((item as LayoutControlItem).ControlName != null && !hashTableNames.ContainsKey((item as LayoutControlItem).ControlName))
							hashTableNames.Add((item as LayoutControlItem).ControlName, null);
					}
				} else
					if(!hashTableNames.ContainsKey(item.Name))
						hashTableNames.Add(item.Name, null);
			}
			return hashTableNames;
		}
		static void PatchItemLocation(List<BaseLayoutItem> listLayoutItem, Point leftTopPoint, string parentName) {
			foreach(BaseLayoutItem item in listLayoutItem) {
				if(item.ParentName == parentName)
					item.Location = new Point(item.Location.X - leftTopPoint.X, item.Location.Y - leftTopPoint.Y);
			}
		}
		Point FindLeftTopPoint(string parentName) {
			Point minPoint = new Point(int.MaxValue, int.MaxValue);
			foreach(BaseLayoutItem item in Items) if(item.ParentName == parentName && item.Location.X <= minPoint.X && item.Location.Y <= minPoint.Y) minPoint = item.Location;
			return minPoint;
		}
		void MakeTemplateCore(string name, List<BaseLayoutItem> choosenItems, ControlData choosenControl, Stream stream) {
			ControlsInfo = choosenControl;
			Items = choosenItems;
			Name = name;
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, Stream stream) {
			serializer.SerializeObject(this, stream, GetType().Name);
		}
		protected virtual void RestoreTemplateCore(Stream stream) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			Items = new List<BaseLayoutItem>();
			ControlsInfo = new ControlData();
			serializer.DeserializeObject(this, stream, GetType().Name);
		}
		internal object XtraFindItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			string typeStr = (string)infoType.Value;
			BaseLayoutItem newItem = null;
			switch(typeStr) {
				case "EmptySpaceItem":
					newItem = CreateEmptySpaceItem(null);
					break;
				case "SplitterItem":
					newItem = new SplitterItem();
					break;
				case "LayoutRepositoryItem":
					newItem = new LayoutRepositoryItem();
					break;
				case "LayoutControlItem":
					newItem = CreateLayoutItem(null);
					break;
				case "SimpleSeparator":
					newItem = new SimpleSeparator();
					break;
				case "SimpleLabelItem":
					newItem = new SimpleLabelItem();
					break;
				case "TabbedControlGroup":
				case "TabbedGroup":
					newItem = CreateTabbedGroup(null);
					break;
				case "LayoutControlGroup":
				case "LayoutGroup":
					newItem = CreateLayoutGroup(null);
					break;
			}
			Items.Add(newItem);
			return newItem;
		}
		protected virtual LayoutGroup CreateLayoutGroup(LayoutGroup parent) {
			return new LayoutControlGroup(parent);
		}
		protected virtual BaseLayoutItem CreateLayoutItem(LayoutGroup parent) {
			return new LayoutControlItem((LayoutControlGroup)parent);
		}
		protected virtual TabbedGroup CreateTabbedGroup(LayoutGroup parent) {
			return new TabbedControlGroup(parent);
		}
		protected virtual EmptySpaceItem CreateEmptySpaceItem(LayoutGroup parent) {
			return new EmptySpaceItem((LayoutControlGroup)parent);
		}
		protected virtual SplitterItem CreateSplitterItem(LayoutGroup parent) {
			return new SplitterItem((LayoutControlGroup)parent);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnStartSerializing() {
		}
	}
	public class ControlData {
		[XtraSerializableProperty]
		public string Name { get; set; }
		[XtraSerializableProperty]
		public string Data { get; set; }
		[XtraSerializableProperty]
		public string Resources { get; set; }
		[XtraSerializableProperty]
		public List<string> Assembly { get; set; }
	}
	internal class RenameGroupHelper {
		public string PreviousName { get; set; }
		public string RealName { get; set; }
		public RenameGroupHelper(string prevName, string realName) {
			PreviousName = prevName;
			RealName = realName;
		}
	}
	public class TemplateException :Exception {
		public TemplateException() : base() { }
		public TemplateException(string message) : base(message) { }
	}
	public static class TemplateString {
		public static string ReplaceVersion = "#ReplaceThisVersion";
		public static string PublicKey = "PublicKeyToken=";
		public static string LongVersion = "Version=";
		public static string ShortVersion = ".v";
		public static string DevExpress = "DevExpress";
		public static string Nunit = "nunit";
		public static string Designer = "Designer.cs";
		public static string PathToCopyTemplate {
			get {
				if(UseCustomTemplatePath) return TemplatePath + @"CopyTemplates\";
				return String.Format("{0}{1}", pathToCommonDocuments, @"\DevExpress\XtraLayout\UserTemplates\CopyTemplates\");
			}
		}
		public static string PathToTemplate {
			get {
				if(UseCustomTemplatePath) return TemplatePath;
				return String.Format("{0}{1}", pathToCommonDocuments, @"\DevExpress\XtraLayout\UserTemplates\");
			}
		}
		public static string PathToDebugInfo{
			get {
				if(UseCustomTemplatePath) return TemplatePath + @"DebugInfo\";
				return String.Format("{0}{1}", pathToCommonDocuments, @"\DevExpress\XtraLayout\UserTemplates\DebugInfo\");
			}
		}
		public static string CopyTemplateName = "DevExpressCopyPasteTemplate";
		public static string EmbeddedResourceXML = "DevExpress.XtraLayout.Customization.Templates.XML";
		public static string LayoutGroupForRestoreName = "LayoutRootGroupForRestore";
		internal static string pathToCommonDocuments { get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments); } }
		const string XtraTemplateRegistryPath = "Software\\Developer Express\\XtraLayout\\";
		const string XtraTemplatePathRegistryEntry = "PathToTemplate";
		const string XtraUseTemplatePathRegistryEntry = "UsePathToTemplate";
		public static void SetPathToTemplate(string path) {
			if(!Directory.Exists(path)) throw new DirectoryNotFoundException("Directory not found: " + path); 
			PropertyStore store = new PropertyStore(XtraTemplateRegistryPath);
			if(store == null) throw new Exception("PropertyStore is Empty.");
			store.AddProperty(XtraTemplatePathRegistryEntry, path);
			store.AddProperty(XtraUseTemplatePathRegistryEntry, true);
			store.Store();
		}
		public static void RemovePathToTemplate(){
			PropertyStore store = new PropertyStore(XtraTemplateRegistryPath);
			if(store == null) return;
			store.AddProperty(XtraTemplatePathRegistryEntry, "");
			store.AddProperty(XtraUseTemplatePathRegistryEntry, false);
			store.Store(); 
		}
		static string TemplatePath {
			get {																									  
				PropertyStore store = new PropertyStore(XtraTemplateRegistryPath);
				if(store == null) return pathToCommonDocuments;
				store.Restore();
				string result = (string)store.RestoreProperty(XtraTemplatePathRegistryEntry, null);
				if(result.Last() != '\\')
					result += @"\";
				return result;
			}
		}
		static bool UseCustomTemplatePath {
			get {																									  
				PropertyStore store = new PropertyStore(XtraTemplateRegistryPath);
				if(store == null)
					return false;
				store.Restore();
				return store.RestoreBoolProperty(XtraUseTemplatePathRegistryEntry, false) && store.RestoreProperty(XtraTemplatePathRegistryEntry,null) != null;
			}
		}
	}
	public class DXTemplateResourceManager {
		public string ResourceString { get; set; }
		public DXTemplateResourceManager(string resources) {
			ResourceString = resources;
			InitializeReader(resources);
		}
		void InitializeReader(string resources) {
			if(resources != "") {
				resXResourceReader = new ResXResourceReader(new StringReader(resources));
			}
		}
		internal ResXResourceReader GetReader() {
			return resXResourceReader;
		}
		ResXResourceReader resXResourceReader;
		public object GetObject(string name) {
			if(resXResourceReader == null) return null;
			foreach(DictionaryEntry d in resXResourceReader) {
				if(name.Contains(d.Key.ToString())) {
					return d.Value;
				}
			}
			return null;
		}
	}
}
