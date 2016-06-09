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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Wizard;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Wizards3;
using DevExpress.XtraReports.Wizards3.Builder;
using PrintingNative = DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.UserDesigner {
	public enum ReportState { None, Opening, Opened, Changed, Saved, Closing };
	internal delegate void CommandIDEventHandler(object sender, CommandIDEventArgs e);
	internal class CommandIDEventArgs : EventArgs {
		CommandID cmdID;
		public CommandID CommandID {
			get { return cmdID; }
		}
		public CommandIDEventArgs(CommandID cmdID) {
			this.cmdID = cmdID;
		}
	}
	public class ReportStateEventArgs : EventArgs {
		ReportState reportState = ReportState.None;
		public ReportStateEventArgs(ReportState reportState) {
			this.reportState = reportState;
		}
		public ReportState ReportState {
			get { return reportState; }
		}
	}
	public delegate void ReportStateEventHandler(object sender, ReportStateEventArgs e);
	[
	ToolboxItem(false),
	ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRDesignPanel.bmp"),
	Designer("DevExpress.XtraReports.Design.XRDesignPanelDesigner, " + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IDesigner)),
	Description("Displays a report in the End-User Designer."),
	]
	public class XRDesignPanel : PanelControl, IServiceProvider
	{
		#region classes
		class PermanentCommandsHandler : ICommandHandler {
			XRDesignPanel designPanel;
			public PermanentCommandsHandler(XRDesignPanel designPanel) {
				this.designPanel = designPanel;
			}
			public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
				switch(command) {
					case ReportCommand.NewReport:
					case ReportCommand.NewReportWizard:
					case ReportCommand.OpenFile:
					case ReportCommand.Exit:
						useNextHandler = false;
						return true;
					default:
						return false;
				}
			}
			public void HandleCommand(ReportCommand command, object[] args) {
				switch(command) {
					case ReportCommand.Exit:
						designPanel.parentFormHelper.CloseParentForm();
						break;
					case ReportCommand.NewReport:
						designPanel.CreateNewReport();
						break;
					case ReportCommand.NewReportWizard:
						designPanel.CreateNewReportWizard();
						break;
					case ReportCommand.OpenFile:
						new TryCatchHelper(designPanel.LookAndFeel, designPanel).ExecuteAction(() => designPanel.OpenReportFile());
						break;
				}
			}
		}
		class CommandHandler : ICommandHandler {
			XRDesignPanel designPanel;
			bool DesignerVisible {
				get { return designPanel.SelectedTabIndex == TabIndices.Designer; }
			}
			bool BrowserVisible {
				get { return designPanel.SelectedTabIndex == TabIndices.Html; }
			}
			UndoEngineImpl UndoEngine {
				get { return designPanel.undoEngine; }
			}
			ReportTabControl View {
				get { return designPanel.View; }
			}
			public CommandHandler(XRDesignPanel designPanel) {
				this.designPanel = designPanel;
			}
			public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
				if(designPanel.Report == null)
					return false;
				switch(command) {
					case ReportCommand.SaveFileAs:
						useNextHandler = false;
						return true;
					case ReportCommand.SaveFile:
						useNextHandler = false;
						return true;
					case ReportCommand.Closing:
						useNextHandler = false;
						return true;
					case ReportCommand.Close:
						return true;
					case ReportCommand.Undo:
						if(DesignerVisible && (CanUndoInEditingMode() ?? UndoEngine != null && UndoEngine.CanUndo)) {
							useNextHandler = false;
							return true;
						}
						return false;
					case ReportCommand.Redo:
						if(DesignerVisible && (CanRedoInEditingMode() ?? UndoEngine != null && UndoEngine.CanRedo)) {
							useNextHandler = false;
							return true;
						}
						return false;
					case ReportCommand.HtmlBackward:
					case ReportCommand.HtmlForward:
					case ReportCommand.HtmlHome:
					case ReportCommand.HtmlRefresh:
					case ReportCommand.HtmlFind:
						return BrowserVisible;
					default:
						return false;
				}
			}
			public void HandleCommand(ReportCommand command, object[] args) {
				switch(command) {
					case ReportCommand.SaveFile:
						designPanel.SaveReport();
						break;
					case ReportCommand.SaveFileAs:
						designPanel.SaveReportAs();
						break;
					case ReportCommand.Closing:
						if(designPanel.SaveChangedReport() == DialogResult.Cancel && args != null && args.Length > 1 && args[1] is CancelEventArgs)
							((CancelEventArgs)args[1]).Cancel = true;
						break;
					case ReportCommand.Close:
						HandleClose();
						break;
					case ReportCommand.Undo:
						if(!UndoInEditingMode())
							UndoEngine.DoUndo();
						break;
					case ReportCommand.Redo:
						if(!RedoInEditingMode())
							UndoEngine.DoRedo();
						break;
					case ReportCommand.HtmlBackward:
						View.HtmlBackward();
						break;
					case ReportCommand.HtmlForward:
						View.HtmlForward();
						break;
					case ReportCommand.HtmlHome:
						View.HtmlHome();
						break;
					case ReportCommand.HtmlRefresh:
						View.HtmlRefresh();
						break;
					case ReportCommand.HtmlFind:
						View.HtmlFind();
						break;
				}
			}
			void HandleClose() {
				designPanel.CommitPropertyGridData();
				CancelEventArgs e = new CancelEventArgs();
				designPanel.ExecCommand(ReportCommand.Closing, new object[] { null, e, designPanel });
				if(e.Cancel)
					return;
				designPanel.CloseReport();
			}
			InplaceTextEditorBase GetInplacedEditor() {
				var selectionService = designPanel.GetService(typeof(ISelectionService)) as ISelectionService;
				var selectedComponents = selectionService.GetSelectedComponents().OfType<IComponent>();
				foreach(var component in selectedComponents) {
					var textControlDesigner = designPanel.host.GetDesigner(component) as XRTextControlDesigner;
					if(textControlDesigner != null && textControlDesigner.IsInplaceEditingMode)
						return textControlDesigner.Editor;
				}
				return null;
			}
			bool? CanUndoInEditingMode() {
				InplaceTextEditorBase inplaceTextEditor = GetInplacedEditor();
				if(inplaceTextEditor != null)
					return inplaceTextEditor.CanUndo;
				return null;
			}
			bool? CanRedoInEditingMode() {
				InplaceTextEditorBase inplaceTextEditor = GetInplacedEditor();
				if(inplaceTextEditor != null)
					return inplaceTextEditor.CanRedo;
				return null;
			}
			bool UndoInEditingMode() {
				InplaceTextEditorBase inplaceTextEditor = GetInplacedEditor();
				if(inplaceTextEditor != null) {
					inplaceTextEditor.Undo();
					return true;
				}
				return false;
			}
			bool RedoInEditingMode() {
				InplaceTextEditorBase inplaceTextEditor = GetInplacedEditor();
				if(inplaceTextEditor != null) {
					inplaceTextEditor.Redo();
					return true;
				}
				return false;
			}
		}
		class DummyFontService : DevExpress.XtraReports.Design.FontServiceBase {
			public DummyFontService() {
			}
		}
		class ParentFormHelper {
			Form parentForm;
			XRDesignPanel designPanel;
			public ParentFormHelper(XRDesignPanel designPanel) {
				this.designPanel = designPanel;
			}
			public void Initialize() {
				NullParentForm();
				parentForm = designPanel.FindForm();
				if(parentForm != null && !(parentForm is XRDesignPanelForm))
					parentForm.FormClosing += new FormClosingEventHandler(OnFormClosing);
			}
			public void NullParentForm() {
				if(parentForm != null) {
					parentForm.FormClosing -= new FormClosingEventHandler(OnFormClosing);
					parentForm = null;
				}
			}
			public void CloseParentForm() {
				if(parentForm != null)
					parentForm.Close();
			}
			void OnFormClosing(object sender, FormClosingEventArgs e) {
				if(!e.Cancel)
					designPanel.ExecuteClosing(new object[] { sender, e });
			}
		}
		#endregion
		#region static
		internal static string ApplyReportState(string text, ReportState reportState) {
			if(text.Length > 0 && text[text.Length - 1] == '*')
				text = text.Remove(text.Length - 1, 1);
			return reportState == ReportState.Changed ? text + "*" : text;
		}
		public static SaveFileDialog CreateSaveFileDialog(XtraReport report, string fileName) {
			return CreateSaveFileDialog(report, Application.StartupPath, fileName);
		}
		public static SaveFileDialog CreateSaveFileDialog(XtraReport report, string defaultDirectory, string fileName) {
			string realName = PrintingNative.StringHelper.GetNonEmptyValue(fileName, report.GetType().Name);
			return FileDialogCreator.CreateSaveDialog(realName, defaultDirectory, ".repx");
		}
		static bool TechnologySupported(ViewTechnology[] technologies, ViewTechnology requiredTechnology) {
			foreach(ViewTechnology technology in technologies) {
				if(technology == requiredTechnology) {
					return true;
				}
			}
			return false;
		}
		static string GetShortTypeName(string typeName) {
			string[] items = typeName.Split(',');
			return items.Length > 1 ? String.Join(",", items, 0, 2) : typeName;
		}
		#endregion
		private ReportTabControl view;
		private int selectedTabIndex;
		XtraReport report;
		XRToolboxService toolBoxServ;
		DevExpress.XtraReports.Design.FontServiceBase fontService;
		string fileName = "";
		ReportState reportState = ReportState.None;
		XRDesignerHost host;
		ReportCommandService reportCommandService;
		IToolShell reportToolShell;
		Hashtable reportSettings;
		UndoEngineImpl undoEngine;
		ParentFormHelper parentFormHelper;
		ComponentVisibility componentVisibility = ComponentVisibility.ReportExplorer;
		ComponentVisibility stylesNodeVisibility = ComponentVisibility.ReportExplorer;
		ComponentVisibility formattingRulesNodeVisibility = ComponentVisibility.ReportExplorer;
		IServiceProvider parentServiceProvider;
		#region properties
		XRToolboxService ToolboxService {
			get {
				if(toolBoxServ == null)
					toolBoxServ = GetService(typeof(IToolboxService)) as XRToolboxService;
				return toolBoxServ;
			}
		}
		IXRMenuCommandService CommandService {
			get {
				return GetService(typeof(IXRMenuCommandService)) as IXRMenuCommandService;
			}
		}
		[DefaultValue(ComponentVisibility.ReportExplorer)]
		public ComponentVisibility ComponentVisibility {
			get { return componentVisibility; }
			set { componentVisibility = value; }
		}
		[DefaultValue(ComponentVisibility.ReportExplorer)]
		public ComponentVisibility StylesNodeVisibility {
			get { return stylesNodeVisibility; }
			set { stylesNodeVisibility = value; }
		}
		[DefaultValue(ComponentVisibility.ReportExplorer)]
		public ComponentVisibility FormattingRulesNodeVisibility {
			get { return formattingRulesNodeVisibility; }
			set { formattingRulesNodeVisibility = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the ComponentVisibility property instead.")
		]
		public Boolean ShowComponentTray {
			get { return (componentVisibility & ComponentVisibility.ComponentTray) > 0; }
			set {
				ComponentVisibility |= ComponentVisibility.ComponentTray;
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelFileName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		]
		public string FileName {
			get { return fileName; }
			set {
				if(fileName != value) {
					fileName = value;
					OnFileNameChanged(this, EventArgs.Empty);
				}
			}
		}
		[Browsable(false)]
		public XtraReport Report {
			get { return report; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		]
		public ReportState ReportState {
			get { return reportState; }
			set {
				if(reportState != value || value == ReportState.Saved) {
					reportState = value;
					OnReportStateChanged(new ReportStateEventArgs(reportState));
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelBorderStyle"),
#endif
		DefaultValue(DevExpress.XtraEditors.Controls.BorderStyles.NoBorder),
		SRCategory(ReportStringId.CatAppearance),
		]
		public new DevExpress.XtraEditors.Controls.BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelSelectedTabIndex"),
#endif
		DefaultValue(0),
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatAppearance),
		]
		public int SelectedTabIndex {
			get {
				if(view != null)
					selectedTabIndex = view.SelectedIndex;
				return selectedTabIndex;
			}
			set {
				selectedTabIndex = value;
				UpdateTabIndex();
			}
		}
		internal IToolShell ReportToolShell {
			get { return reportToolShell; }
		}
		internal ReportTabControl View {
			get { return view; }
		}
		internal bool FileNameExists {
			get { return !String.IsNullOrEmpty(fileName); }
		}
		internal ReportDesigner ReportDesigner {
			get { return host != null ? (ReportDesigner)host.GetDesigner(host.RootComponent) : null; }
		}
#if DEBUGTEST
		[Browsable(false)]
		public DevExpress.XtraBars.BarManager Test_ViewPrintBarManager { get { return View != null ? View.BarManager : null; } }
		[Browsable(false)]
		public DevExpress.XtraPrinting.Control.PrintControl Test_ViewPrintControl { get { return View != null ? View.PreviewControl : null; } }
		[Browsable(false)]
		public XRDesignerHost Test_DesignerHost { get { return host; } }
#endif
		#endregion
		internal XRDesignPanel(IServiceProvider parentServiceProvider)
			: this() {
			this.parentServiceProvider = parentServiceProvider;
		}
		public XRDesignPanel()
			: base() {
			fontService = new DummyFontService();
			reportSettings = new Hashtable();
			BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			ApplyLookAndFeelPadding();
			reportCommandService = new ReportCommandService();
			reportCommandService.SetCommandVisibility(ReportCommand.PropertiesWindow, CommandVisibility.None);
			reportCommandService.CommandChanged += new ReportCommandEventHandler(OnCommandChanged);
			reportCommandService.AddCommandHandler(new PermanentCommandsHandler(this));
			reportCommandService.AddCommandHandler(new CommandHandler(this));
			parentFormHelper = new ParentFormHelper(this);
		}
#if DEBUGTEST
		public XtraReport Test_GetPreviewReport() {
			ReportTabControl view = this.GetService(typeof(ReportTabControl)) as ReportTabControl;
			return view != null ? view.PreviewReport : null;
		}
		public void Test_ShowPreviewTab(bool buildPagesInBackground) {
			ReportTabControl view = this.GetService(typeof(ReportTabControl)) as ReportTabControl;
			if(view != null)
				view.ShowPreviewTab(buildPagesInBackground);
		}
		public void Test_ShowDesignerTab() {
			ReportTabControl view = GetService(typeof(ReportTabControl)) as ReportTabControl;
			if(view != null)
				view.ShowDesignerTab();
		}
#endif
		public new object GetService(Type serviceType) {
			return host != null ? ((IDesignerHost)host).GetService(serviceType) : parentServiceProvider != null ? parentServiceProvider.GetService(serviceType) : null;
		}
		public void AddService(Type type, object value) {
			host.AddService(type, value);
		}
		public void RemoveService(Type type) {
			host.RemoveService(type);
		}
		#region report commands
		public void AddCommandHandler(ICommandHandler handler) {
			reportCommandService.AddCommandHandler(handler);
		}
		public void RemoveCommandHandler(ICommandHandler handler) {
			reportCommandService.RemoveCommandHandler(handler);
		}
		public void SetCommandVisibility(ReportCommand command, CommandVisibility visibility) {
			reportCommandService.SetCommandVisibility(command, visibility);
		}
		public void SetCommandVisibility(ReportCommand[] commands, CommandVisibility visibility) {
			reportCommandService.SetCommandVisibility(commands, visibility);
		}
		public CommandVisibility GetCommandVisibility(ReportCommand command) {
			return reportCommandService.GetCommandVisibility(command);
		}
		public bool GetCommandEnabled(ReportCommand command) {
			return reportCommandService.GetCommandEnabled(command);
		}
		public void ExecCommand(ReportCommand command, object[] args) {
			if(report == null)
				reportCommandService.HandleCommand(command, args);
			else if(CommandService != null)
				CommandService.GlobalInvoke(CommandIDReportCommandConverter.GetCommandID(command), args);
		}
		public void ExecCommand(ReportCommand command) {
			ExecCommand(command, new object[] { });
		}
		#endregion
		protected void SetRootView(IDesignerHost host) {
			IRootDesigner rootDesigner = host.GetDesigner(host.RootComponent) as IRootDesigner;
			if(rootDesigner == null || !TechnologySupported(rootDesigner.SupportedTechnologies, ControlConstants.ViewTechnologyDefault))
				return;
			if(view != null) {
				DevExpress.XtraReports.Native.NativeMethods.ClearUnvalidatedControl(FindForm());
				Controls.Clear();
				if(!view.IsDisposed) {
					view.SelectedTabIndexChanged -= new EventHandler(OnSelectedTabIndexChanged);
					view.Dispose();
				}
			}
			view = (ReportTabControl)rootDesigner.GetView(ControlConstants.ViewTechnologyDefault);
			view.SuspendLayout();
			view.SelectedTabIndexChanged += new EventHandler(OnSelectedTabIndexChanged);
			view.Dock = DockStyle.Fill;
			view.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			UpdateTabIndex();
			this.Controls.Add(view);
			view.ResumeLayout();
		}
		private void UpdateTabIndex() {
			if(view != null)
				view.SelectedIndex = selectedTabIndex;
		}
		void SubscribeEvents() {
			if(ToolboxService != null) {
				ToolboxService.SelectedItemChanged += new EventHandler(OnSelectedToolboxItemChanged);
				ToolboxService.SelectedItemUsed += new EventHandler(OnSelectedToolboxItemUsed);
			}
			IComponentChangeService comChangeServ = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(comChangeServ != null) {
				comChangeServ.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
				comChangeServ.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				comChangeServ.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				comChangeServ.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
				comChangeServ.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
			}
		}
		void InitializeHost() {
			host = new XRDesignerHost(parentServiceProvider);
			var commandServ = new XRMenuCommandService(host, this);
			host.AddService(typeof(IMenuCommandService), commandServ);
			host.AddService(typeof(IXRMenuCommandService), commandServ);
			host.AddService(typeof(XRDesignPanel), this);
			host.AddService(typeof(DevExpress.Data.Utils.IToolShell), reportToolShell);
			host.AddService(typeof(DevExpress.XtraReports.Design.FontServiceBase), fontService);
			host.AddService(typeof(ReportCommandService), reportCommandService);
			Activate();
			IDesignerEventService designerEventService = ((IDesignerEventService)GetService(typeof(IDesignerEventService)));
			if(designerEventService != null)
				designerEventService.SelectionChanged += new EventHandler(OnSelectionChanged);
			XRFormKeyHandler.RegisterDesignPanel(this);
			host.Activated += new EventHandler(OnActivated);
			host.Deactivated += new EventHandler(OnDeactivated);
			host.LoadComplete += new EventHandler(OnLoadComplete);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				parentFormHelper.NullParentForm();
				DisposeHost();
				reportSettings.Clear();
				reportCommandService.CommandChanged -= new ReportCommandEventHandler(OnCommandChanged);
				reportCommandService.Dispose();
			}
			base.Dispose(disposing);
		}
		void DisposeHost() {
			if(host == null)
				return;
			RemoveAndDispoaeService(typeof(UndoEngine));
			undoEngine = null;
			reportCommandService.DetachFromMenuCommandService();
			host.Deactivate();
			host.RemoveService(typeof(InheritanceHelperService));
			host.RemoveService(typeof(DevExpress.Data.Utils.IToolShell));
			RemoveAndDispoaeService(typeof(DevExpress.XtraReports.Design.FontServiceBase));
			host.RemoveService(typeof(IMenuCommandService));
			host.RemoveService(typeof(IXRMenuCommandService));
			host.RemoveService(typeof(XRDesignPanel));
			host.RemoveService(typeof(ReportCommandService));
			this.toolBoxServ = null;
			XRFormKeyHandler.UnregisterDesignPanel(this);
			IDesignerEventService designerEventService = ((IDesignerEventService)GetService(typeof(IDesignerEventService)));
			if(designerEventService != null)
				designerEventService.SelectionChanged -= new EventHandler(OnSelectionChanged);
			if(toolBoxServ != null) {
				toolBoxServ.SelectedItemChanged -= new EventHandler(OnSelectedToolboxItemChanged);
				toolBoxServ.SelectedItemUsed -= new EventHandler(OnSelectedToolboxItemUsed);
			}
			IComponentChangeService comChangeServ = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(comChangeServ != null) {
				comChangeServ.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
				comChangeServ.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				comChangeServ.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
				comChangeServ.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
				comChangeServ.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
			}
			host.Activated -= new EventHandler(OnActivated);
			host.Deactivated -= new EventHandler(OnDeactivated);
			host.LoadComplete -= new EventHandler(OnLoadComplete);
			host.Dispose();
			host = null;
		}
		void RemoveAndDispoaeService(Type serviceType) {
			IDisposable service = ((IServiceProvider)host).GetService(serviceType) as IDisposable;
			if(service != null)
				service.Dispose();
			host.RemoveService(serviceType);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(m.Msg == PrintingNative.Win32.WM_MOUSEACTIVATE && ShouldFocus) {
				if(!Focused)
					Focus();
			}
		}
		bool ShouldFocus {
			get {
				return SelectedTabIndex == TabIndices.Designer && ReportDesigner != null && !ReportDesigner.IsInplaceEditorActive();
			}
		}
		void SaveNewFile(XtraReport report) {
			PrintingNative.CursorStorage.SetCursor(Cursors.WaitCursor);
			if(report != null) {
				new DesignerHostExtensions(host).CommitInplaceEditors();
				string defaultUrl = PrintingNative.StringHelper.GetNonEmptyValue(fileName, report.DisplayName, report.Name);
				string result = ReportStorageServiceInteractive.SetNewData(report, defaultUrl);
				if(!string.IsNullOrEmpty(result)) {
					this.FileName = result;
					ReportState = ReportState.Saved;
				}
			}
			PrintingNative.CursorStorage.RestoreCursor();
		}
		void SaveFile(XtraReport report) {
			PrintingNative.CursorStorage.SetCursor(Cursors.WaitCursor);
			if(report != null) {
				new DesignerHostExtensions(host).CommitInplaceEditors();
				System.Diagnostics.Debug.Assert(FileNameExists);
				ReportStorageService.SetData(report, fileName);
				ReportState = ReportState.Saved;
			}
			PrintingNative.CursorStorage.RestoreCursor();
		}
		System.Resources.ResourceManager resourceManager;
		void OnReportDeserialize(object sender, XRSerializerEventArgs e) {
			resourceManager = e.ResourceManager;
			if(e.Serializer != null) {
				reportSettings.Clear();
				reportSettings.Add(DesignPropertyNames.ShowExportWarnings, e.Serializer.DeserializeBoolean(DesignPropertyNames.ShowExportWarnings, true));
				reportSettings.Add(DesignPropertyNames.DrawGrid, e.Serializer.DeserializeBoolean(DesignPropertyNames.DrawGrid, true));
				reportSettings.Add(DesignPropertyNames.DrawWatermark, e.Serializer.DeserializeBoolean(DesignPropertyNames.DrawWatermark, false));
				reportSettings.Add(DesignPropertyNames.GridSize, e.Serializer.DeserializeSize(DesignPropertyNames.GridSize, new Size(8, 8)));
				reportSettings.Add(DesignPropertyNames.SnapToGrid, e.Serializer.DeserializeBoolean(DesignPropertyNames.SnapToGrid, true));
				reportSettings.Add(DesignPropertyNames.PreviewRowCount, e.Serializer.DeserializeInteger(DesignPropertyNames.PreviewRowCount, 100));
			}
		}
		void LoadDesignTimeProperties() {
			if(resourceManager != null) {
				try {
					System.Resources.ResourceSet resourceSet = ResLoader.GetResourceSet(resourceManager);
					ResLoader.LoadDesignTimeProperties(report, host, resourceSet);
					NestedComponentEnumerator enemerator = new NestedComponentEnumerator(report.Bands);
					while(enemerator.MoveNext()) {
						ResLoader.LoadDesignTimeProperties(enemerator.Current, host, resourceSet);
					}
				} finally {
					resourceManager = null;
				}
			}
			ReportDesigner repDesigner = host.GetDesigner(host.RootComponent) as ReportDesigner;
			if(repDesigner == null || reportSettings.Count == 0)
				return;
			repDesigner.ShowExportWarnings = (bool)reportSettings[DesignPropertyNames.ShowExportWarnings];
			repDesigner.DrawGrid = (bool)reportSettings[DesignPropertyNames.DrawGrid];
			repDesigner.DrawWatermark = (bool)reportSettings[DesignPropertyNames.DrawWatermark];
			Size size = (Size)reportSettings[DesignPropertyNames.GridSize];
			repDesigner.RootReport.SnapGridSize = XRConvert.Convert((float)size.Width, DevExpress.XtraPrinting.GraphicsDpi.Pixel, repDesigner.RootReport.Dpi);
			repDesigner.SnapToGrid = (bool)reportSettings[DesignPropertyNames.SnapToGrid];
			repDesigner.DetailPrintCount = (int)reportSettings[DesignPropertyNames.PreviewRowCount];
			reportSettings.Clear();
		}
		bool ContainsInternalFocus(Control control) {
			foreach(Control ctrl in control.Controls)
				if(ContainsInternalFocus(ctrl))
					return true;
			return control.Focused;
		}
		internal bool ContainsInternalFocus() {
			if(host == null)
				return false;
			return ContainsInternalFocus(this);
		}
		#region events
		private static readonly object SelectionChangedEvent = new object();
		private static readonly object SelectedToolboxItemChangedEvent = new object();
		private static readonly object SelectedToolboxItemUsedEvent = new object();
		private static readonly object ComponentAddedEvent = new object();
		private static readonly object ComponentRemovedEvent = new object();
		private static readonly object ComponentChangedEvent = new object();
		private static readonly object ReportStateChangedEvent = new object();
		private static readonly object CommandStatusChangedEvent = new object();
		private static readonly object DesignerHostLoadingEvent = new object();
		private static readonly object DesignerHostLoadedEvent = new object();
		private static readonly object ActivatedEvent = new object();
		private static readonly object DeactivatedEvent = new object();
		private static readonly object LoadCompleteEvent = new object();
		private static readonly object CommandExecuteEvent = new object();
		private static readonly object ComponentAddingEvent = new object();
		private static readonly object ComponentRemovingEvent = new object();
		private static readonly object SelectedTabIndexChangedEvent = new object();
		private static readonly object CommandChangedEvent = new object();
		private static readonly object FileNameChangedEvent = new object();
		internal event EventHandler FileNameChanged {
			add { Events.AddHandler(FileNameChangedEvent, value); }
			remove { Events.RemoveHandler(FileNameChangedEvent, value); }
		}
		void OnFileNameChanged(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[FileNameChangedEvent];
			if(handler != null)
				handler(sender, e);
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelDesignerHostLoading")]
#endif
		public event EventHandler DesignerHostLoading {
			add { Events.AddHandler(DesignerHostLoadingEvent, value); }
			remove { Events.RemoveHandler(DesignerHostLoadingEvent, value); }
		}
		public event DesignerLoadedEventHandler DesignerHostLoaded {
			add { Events.AddHandler(DesignerHostLoadedEvent, value); }
			remove { Events.RemoveHandler(DesignerHostLoadedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelReportStateChanged")]
#endif
		public event ReportStateEventHandler ReportStateChanged {
			add { Events.AddHandler(ReportStateChangedEvent, value); }
			remove { Events.RemoveHandler(ReportStateChangedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelComponentRemoved")]
#endif
		public event ComponentEventHandler ComponentRemoved {
			add { Events.AddHandler(ComponentRemovedEvent, value); }
			remove { Events.RemoveHandler(ComponentRemovedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelComponentAdded")]
#endif
		public event ComponentEventHandler ComponentAdded {
			add { Events.AddHandler(ComponentAddedEvent, value); }
			remove { Events.RemoveHandler(ComponentAddedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelComponentChanged")]
#endif
		public event ComponentChangedEventHandler ComponentChanged {
			add { Events.AddHandler(ComponentChangedEvent, value); }
			remove { Events.RemoveHandler(ComponentChangedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelSelectionChanged")]
#endif
		public event EventHandler SelectionChanged {
			add { Events.AddHandler(SelectionChangedEvent, value); }
			remove { Events.RemoveHandler(SelectionChangedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelSelectedToolboxItemChanged")]
#endif
		public event EventHandler SelectedToolboxItemChanged {
			add { Events.AddHandler(SelectedToolboxItemChangedEvent, value); }
			remove { Events.RemoveHandler(SelectedToolboxItemUsedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelSelectedToolboxItemUsed")]
#endif
		public event EventHandler SelectedToolboxItemUsed {
			add { Events.AddHandler(SelectedToolboxItemUsedEvent, value); }
			remove { Events.RemoveHandler(SelectedToolboxItemUsedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelCommandStatusChanged")]
#endif
		public event MenuCommandEventHandler CommandStatusChanged {
			add { Events.AddHandler(CommandStatusChangedEvent, value); }
			remove { Events.RemoveHandler(CommandStatusChangedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelActivated")]
#endif
		public event EventHandler Activated {
			add { Events.AddHandler(ActivatedEvent, value); }
			remove { Events.RemoveHandler(ActivatedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelDeactivated")]
#endif
		public event EventHandler Deactivated {
			add { Events.AddHandler(DeactivatedEvent, value); }
			remove { Events.RemoveHandler(DeactivatedEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelComponentRemoving")]
#endif
		public event ComponentEventHandler ComponentRemoving {
			add { Events.AddHandler(ComponentRemovingEvent, value); }
			remove { Events.RemoveHandler(ComponentRemovingEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelComponentAdding")]
#endif
		public event ComponentEventHandler ComponentAdding {
			add { Events.AddHandler(ComponentAddingEvent, value); }
			remove { Events.RemoveHandler(ComponentAddingEvent, value); }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignPanelSelectedTabIndexChanged"),
#endif
		Category("Property Changed"),
		]
		public event EventHandler SelectedTabIndexChanged {
			add { Events.AddHandler(SelectedTabIndexChangedEvent, value); }
			remove { Events.RemoveHandler(SelectedTabIndexChangedEvent, value); }
		}
		internal event ReportCommandEventHandler CommandChanged {
			add { Events.AddHandler(CommandChangedEvent, value); }
			remove { Events.RemoveHandler(CommandChangedEvent, value); }
		}
		internal event CommandIDEventHandler CommandExecute {
			add { Events.AddHandler(CommandExecuteEvent, value); }
			remove { Events.RemoveHandler(CommandExecuteEvent, value); }
		}
		internal event EventHandler LoadComplete {
			add { Events.AddHandler(LoadCompleteEvent, value); }
			remove { Events.RemoveHandler(LoadCompleteEvent, value); }
		}
		void OnCommandExecute(object sender, CommandIDEventArgs e) {
			CommandIDEventHandler handler = (CommandIDEventHandler)this.Events[CommandExecuteEvent];
			if(handler != null)
				handler(sender, e);
		}
		protected void OnDesignerHostLoading(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[DesignerHostLoadingEvent];
			if(handler != null)
				handler(sender, EventArgs.Empty);
		}
		protected void OnDesignerHostLoaded(object sender, DesignerLoadedEventArgs e) {
			DesignerLoadedEventHandler handler = (DesignerLoadedEventHandler)this.Events[DesignerHostLoadedEvent];
			if(handler != null)
				handler(sender, e);
		}
		protected void OnSelectionChanged(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[SelectionChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnSelectedToolboxItemChanged(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[SelectedToolboxItemChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnSelectedToolboxItemUsed(object sender, EventArgs e) {
			SetFocus(); 
			RaiseSelectedToolboxItemUsed(sender, e);
		}
		protected void RaiseSelectedToolboxItemUsed(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[SelectedToolboxItemUsedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnReportStateChanged(ReportStateEventArgs e) {
			ReportStateEventHandler handler = (ReportStateEventHandler)this.Events[ReportStateChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnComponentAdded(object sender, ComponentEventArgs e) {
			ComponentEventHandler handler = (ComponentEventHandler)this.Events[ComponentAddedEvent];
			if(handler != null)
				handler(this, e);
			ReportState = ReportState.Changed;
		}
		protected void OnComponentRemoved(object sender, ComponentEventArgs e) {
			ComponentEventHandler handler = (ComponentEventHandler)this.Events[ComponentRemovedEvent];
			if(handler != null)
				handler(this, e);
			ReportState = ReportState.Changed;
		}
		protected void OnActivated(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[ActivatedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnDeactivated(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[DeactivatedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnLoadComplete(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[LoadCompleteEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(!EventArgsHelper.AreValidArgs(e))
				return;
			ComponentChangedEventHandler handler = (ComponentChangedEventHandler)this.Events[ComponentChangedEvent];
			if(handler != null)
				handler(this, e);
			ReportState = ReportState.Changed;
		}
		protected void OnCommandStatusChanged(object sender, MenuCommandEventArgs e) {
			MenuCommandEventHandler handler = (MenuCommandEventHandler)this.Events[CommandStatusChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		protected void OnComponentAdding(object sender, ComponentEventArgs e) {
			ComponentEventHandler handler = (ComponentEventHandler)this.Events[ComponentAddingEvent];
			if(handler != null)
				handler(this, e);
			ReportState = ReportState.Changed;
		}
		protected void OnComponentRemoving(object sender, ComponentEventArgs e) {
			ComponentEventHandler handler = (ComponentEventHandler)this.Events[ComponentRemovingEvent];
			if(handler != null)
				handler(this, e);
			ReportState = ReportState.Changed;
		}
		protected virtual void OnSelectedTabIndexChanged(object sender, EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[SelectedTabIndexChangedEvent];
			if(handler != null)
				handler(this, e);
		}
		void OnCommandChanged(object sender, ReportCommandEventArgs e) {
			ReportCommandEventHandler handler = (ReportCommandEventHandler)this.Events[CommandChangedEvent];
			if(handler != null)
				handler(this, e);
			if(e.MenuCommand != null)
				OnCommandStatusChanged(this, e);
		}
		#endregion
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			ApplyLookAndFeelPadding();
		}
		void ApplyLookAndFeelPadding() {
			Padding padding = ReportPaintStyles.GetPaintStyle(this.LookAndFeel).GetDesignPanelPadding(this.LookAndFeel);
			this.DockPadding.Left = padding.Left;
			this.DockPadding.Right = padding.Right;
			this.DockPadding.Top = padding.Top;
			this.DockPadding.Bottom = padding.Bottom;
		}
		public MenuCommand FindMenuCommand(CommandID cmdID) {
			return (CommandService != null) ?
				CommandService.FindCommand(cmdID) :
				null;
		}
		#region obsolete
		[
		Obsolete("The ExecuteCommand method is obsolete now. Use the ExecCommand method instead."),
		]
		public bool ExecuteCommand(CommandID cmdID) {
			return ExecuteCommandCore(cmdID);
		}
		[
		Obsolete("The ExecuteCommand method is obsolete now. Use the ExecCommand method instead."),
		]
		public bool ExecuteCommand(CommandID cmdID, object[] parameters) {
			return ExecuteCommandCore(cmdID, parameters);
		}
#if DEBUGTEST
		public
#else
		internal 
#endif
 bool ExecuteCommandCore(CommandID cmdID) {
			return ExecuteCommandCore(cmdID, null);
		}
		internal bool ExecuteCommandCore(CommandID cmdID, object[] parameters) {
			try {
				OnCommandExecute(this, new CommandIDEventArgs(cmdID));
			} catch {
				return true;
			}
			return (CommandService != null && cmdID != null) ?
				CommandService.GlobalInvoke(cmdID, parameters) : false;
		}
		#endregion
		public object[] GetSelectedComponents() {
			return ReportDesigner != null ? ReportDesigner.GetSelectedComponents() : new object[] { };
		}
		internal void SelectToolboxItem(ToolboxItem toolboxItem) {
			if(ToolboxService != null)
				ToolboxService.SetSelectedToolboxItem(toolboxItem);
		}
		protected internal void ToolPicked(ToolboxItem toolboxItem) {
			if(toolboxItem != null && host != null) {
				IToolboxUser toolboxUser = host.GetDesigner(host.RootComponent) as IToolboxUser;
				if(toolboxUser != null) {
					toolboxUser.ToolPicked(toolboxItem);
					SetFocus();
				}
			}
		}
		void SetFocus() {
			ReportDesigner repDesigner = host.GetDesigner(host.RootComponent) as ReportDesigner;
			if(repDesigner == null) {
				Focus();
				return;
			}
			repDesigner.ReportFrame.Focus();
		}
		public void OpenReportFile() {
			if(SaveChangedReport() == DialogResult.Cancel)
				return;
			string fileName = ReportStorageServiceInteractive.GetNewUrl();
			if(string.IsNullOrEmpty(fileName))
				return;
			ProcessReportInstance(fileName);
		}
		void CreateNewReport() {
			if(SaveChangedReport() == DialogResult.Cancel)
				return;
			this.fileName = string.Empty;
			OpenReport(XtraReport.GetDefaultReport());
		}
		void CreateNewReportWizard() {
			if(report == null)
				OpenReport(new XtraReport());
			IDesignerHost designrHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			XtraReportModel model = new XtraReportModelUI(null).CreateReportModel(host);
			if(model == null)
				return;
			UndoEngine undoEngine = designrHost.GetService<UndoEngine>();
			IWizardCustomizationService serv = GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
			object dataSource; string dataMember;
			serv.CreateDataSourceSafely(model, out dataSource, out dataMember);
			undoEngine.ExecuteWithoutUndo(() => serv.CreateReportSafely(host, model, dataSource, dataMember));
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			parentFormHelper.Initialize();
		}
		void ActivateHost(XtraReport source) {
			ReportState = ReportState.Opening;
			reportToolShell = new ToolShell();
			InitializeHost();
			if(source != null)
				host.AddService(typeof(InheritanceHelperService), new EUDInheritanceHelperService(host));
			OnDesignerHostLoading(this, EventArgs.Empty);
			host.DesignerLoader.BeginLoad(host);
			host.DesignerLoader.LoadReport(report, source);
			SetRootView(host);
			OnDesignerHostLoaded(this, new DesignerLoadedEventArgs(host));
			report.OnDesignerLoaded(new DesignerLoadedEventArgs(host));
			if(parentServiceProvider != null) {
				ReportCommandServiceBase servBase = parentServiceProvider.GetService(typeof(ReportCommandServiceBase)) as ReportCommandServiceBase;
				if(servBase != null) {
					reportCommandService.AddCommandHandlers(servBase);
					reportCommandService.CopyCommands(servBase);
				}
			}
			reportCommandService.AttachToMenuCommandService(host, (MenuCommandHandlerBase)GetService(typeof(MenuCommandHandler)));
			undoEngine = new UndoEngineImpl(host);
			host.DesignerLoader.EndLoad();
			host.AddService(typeof(UndoEngine), undoEngine);
			host.Activate();
			ReportState = ReportState.Opened;
			SubscribeEvents();
			ReportDesigner.ComponentVisibility = ComponentVisibility;
			ReportDesigner.StylesNodeVisibility = StylesNodeVisibility;
			ReportDesigner.FormattingRulesNodeVisibility = FormattingRulesNodeVisibility;
		}
		public void Activate() {
			if(host != null) {
				host.Activate();
				InvalidateDesignSurface();
			}
		}
		internal void InvalidateDesignSurface() {
			IBandViewInfoService service = this.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
			if(service != null) {
				service.InvalidateViewInfo();
				service.Invalidate();
			}
		}
		public void Deactivate() {
			if(host != null)
				host.Deactivate();
		}
		bool ProcessReportInstance(Stream stream) {
			try {
#if !DEBUGTEST
				if(report != null && !IsValidReportType(stream, report.GetType(), true)) {
					string s = ReportLocalizer.GetString(ReportStringId.Msg_CreateReportInstance);
					s = s.Replace("<br/>", "\r\n");
					if(DialogResult.Yes != XtraMessageBox.Show(LookAndFeel, s, ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner), MessageBoxButtons.YesNo, MessageBoxIcon.Question))
						return false;
				}
#endif
				Type reportType = XRSerializer.GetReportType(stream);
				if(reportType == null)
					throw new XRSerializationException(ReportLocalizer.GetString(ReportStringId.Msg_FileContentCorrupted));
				bool forceDataSource = true;
				if(report == null || !report.GetType().Equals(reportType)) {
					ReportTypeService reportTypeService = GetService(typeof(ReportTypeService)) as ReportTypeService;
					if(reportTypeService != null)
						reportType = reportTypeService.GetType(reportType);
					report = Activator.CreateInstance(reportType) as XtraReport;
					if(report == null)
						report = new XtraReport();
					forceDataSource = false;
				}
				DisposeHost();
				PrintingNative.CursorStorage.SetCursor(Cursors.WaitCursor);
				XtraReport source = null;
				try {
					report.Deserialize += new XRSerializerEventHandler(OnReportDeserialize);
					report.LoadLayoutInternal(stream, ref source, forceDataSource);
				} finally {
					report.Deserialize -= new XRSerializerEventHandler(OnReportDeserialize);
					ActivateHost(source);
					LoadDesignTimeProperties();
					ReportToolShell.UpdateToolItems();
					PrintingNative.CursorStorage.RestoreCursor();
				}
			} catch(Exception ex) {
				ReportState = ReportState.None;
				throw ex;
			}
			return true;
		}
#if !DEBUGTEST
		static bool IsValidReportType(Stream stream, Type type, bool ignoreAssemblyVersionInfo) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			XRTypeInfo info = XRTypeInfo.Deserialize(new StreamReader(stream).ReadToEnd());
			return info.TypeEquals(type, ignoreAssemblyVersionInfo);
		}
#endif
		public void OpenReport(Stream stream) {
			ProcessReportInstance(stream);
		}
		public void OpenReport(string fileName) {
			if(!ReportStorageService.IsValidUrl(fileName))
				throw new ArgumentException(ReportLocalizer.GetString(ReportStringId.Msg_FileNotFound));
			ProcessReportInstance(fileName);
		}
		void ProcessReportInstance(string fileName) {
			Guard.ArgumentIsNotNullOrEmpty(fileName, "filename");
			string fileNameStore = this.fileName;
			this.fileName = fileName;
			using(Stream stream = new MemoryStream(ReportStorageService.GetData(fileName))) {
				if(!ProcessReportInstance(stream))
					this.fileName = fileNameStore;
			}
		}
		public void CloseReport() {
			this.ReportState = ReportState.Closing;
			OpenReport((XtraReport)null);
			OnReportStateChanged(new ReportStateEventArgs(this.reportState));
		}
		public void OpenReport(XtraReport report) {
			DisposeHost();
			reportState = ReportState.None;
			this.report = report;
			if(this.report != null)
				this.report.StopPageBuilding();
			if(report != null)
				ActivateHost(null);
		}
		public void SaveReport(string fileName) {
			string oldFileName = this.fileName;
			try {
				this.fileName = fileName;
				SaveReport();
			} finally {
				this.fileName = oldFileName;
			}
		}
		public void SaveReport() {
			if(FileNameExists && ReportStorageService.CanSetData(fileName))
				SaveFile(report);
			else
				SaveNewFile(report);
		}
		internal void EnsureReportSaved() {
			if(ReportState != ReportState.Saved && FileNameExists && ReportStorageService.CanSetData(fileName))
				SaveFile(report);
		}
		public void SaveReportAs() {
			if(report != null)
				SaveNewFile(report);
		}
		public DialogResult SaveChangedReport() {
			DialogResult dialogResult = DialogResult.None;
			if(reportState == ReportState.Changed) {
				dialogResult = ShowSaveChangedReportDialog(LookAndFeel);
				if(dialogResult == DialogResult.Yes) {
					SaveReportUsingCommand();
				}
			}
			return dialogResult;
		}
#if DEBUGTEST
		public static Func<DialogResult> Test_GetSaveChangedReportResult;
#endif
		static DialogResult ShowSaveChangedReportDialog(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
#if DEBUGTEST
			if(Test_GetSaveChangedReportResult != null)
				return Test_GetSaveChangedReportResult();
#endif
			return XtraMessageBox.Show(lookAndFeel, ReportLocalizer.GetString(ReportStringId.UD_Msg_ReportChanged), ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
		}
		internal void SaveReportUsingCommand() {
			bool handled = false;
			if(CommandService != null) {
				if(FileNameExists)
					handled = ExecuteCommandCore(UICommands.SaveFile);
				else
					handled = ExecuteCommandCore(UICommands.SaveFileAs);
			}
			if(!handled)
				SaveReport();
		}
		internal void CommitPropertyGridData() {
			PropertyGridService propGridService = GetService(typeof(PropertyGridService)) as PropertyGridService;
			if(propGridService == null)
				return;
			if(propGridService.PropertyGrid.ContainsFocus)
				Focus();
		}
		internal void ExecuteClosing(object[] args) {
			new DesignerHostExtensions(host).CommitInplaceEditors();
			CommitPropertyGridData();
			ExecCommand(ReportCommand.Closing, args);
		}
#if DEBUGTEST
		public XRToolboxService Test_GetToolBoxService() {
			return ToolboxService;
		}
#endif
	}
}
