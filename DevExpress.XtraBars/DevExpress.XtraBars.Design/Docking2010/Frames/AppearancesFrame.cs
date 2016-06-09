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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Dragging;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class AppearancesFrame : DevExpress.XtraEditors.Frames.AppearancesDesignerBase {
		private System.Windows.Forms.Label label2;
		private ContainerControl pcDocumentManager;
		public AppearancesFrame() {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(previewView != null) {
					((IAppearanceSelector)previewView).SelectedAppearanceChanged -= OnPreviewSelectedAppearanceChanged;
					previewView.Dispose();
					previewView = null;
				}
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("AppearancePanel", gcAppearances.Width);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			gcAppearances.Width = localStore.RestoreIntProperty("AppearancePanel", gcAppearances.Width);
		}
		#region Init & Ctor
		protected override string DescriptionText {
			get { return "Click on any of the View's elements in the preview pane to display the Appearance objects associated with that element. Then select one or more of the Appearance objects to access and modify their settings via the property grid. Note that specific appearances are ignored when the Skin, WindowsXP or Office2003 paint schemes are applied. "; }
		}
		BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		public override void InitComponent() {
			CreateTabControl();
			CreatePreviewControl();
			InitPreviewControl();
		}
		protected override Image AppearanceImage {
			get { return EditingView.BackgroundImage; }
		}
		protected override DevExpress.XtraTab.XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] { pgMain },
				new string[] { "Properties" });
		}
		DocumentManager previewManager;
		BaseView previewView;
		protected override void CreatePreviewControl() {
			components = new Container();
			previewManager = new PreviewDocumentManager(components);
			previewView = previewManager.CreateView(EditingView.Type);
			((ISupportInitialize)previewManager).BeginInit();
			((ISupportInitialize)previewView).BeginInit();
			previewManager.ContainerControl = this.pcDocumentManager;
			previewManager.ViewCollection.AddRange(new BaseView[] { previewView });
			previewManager.View = previewView;
			((ISupportInitialize)previewView).EndInit();
			((ISupportInitialize)previewManager).EndInit();
		}
		protected override void InitPreviewControl() {
			((IAppearanceSelector)previewView).SelectedAppearanceChanged += OnPreviewSelectedAppearanceChanged;
			PreviewAppearances.AssignInternal(EditingAppearances);
			InitAppearanceList(PreviewAppearances);
		}
		void OnPreviewSelectedAppearanceChanged(object sender, EventArgs e) {
			string[] appearances = ((IAppearanceSelector)previewView).SelectedAppearanceNames;
			if(appearances != null && appearances.Length > 0)
				lbcAppearances.SelectedValue = appearances[0];
		}
		#endregion
		#region Editing
		protected BaseAppearanceCollection EditingAppearances {
			get { return ((IAppearanceCollectionAccessor)EditingView).Appearances; }
		}
		protected BaseAppearanceCollection PreviewAppearances {
			get { return ((IAppearanceCollectionAccessor)previewView).Appearances; }
		}
		protected string[] SelectedObjectNames {
			get {
				if(SelectedObjects == null) return null;
				ArrayList list = new ArrayList();
				for(int i = 0; i < SelectedObjects.Length; i++) {
					string name = (SelectedObjects[i] as AppearanceObject).Name;
					if(list.IndexOf(name) < 0)
						list.Add(name);
				}
				return (string[])list.ToArray(typeof(string));
			}
		}
		protected override void SetSelectedObject() {
			if(Preview == null) return;
			ArrayList arr = new ArrayList();
			if(SelectedObjects != null) {
				foreach(AppearanceObject obj in this.SelectedObjects) {
					AppearanceObject app = EditingAppearances.GetAppearance(obj.Name);
					if(arr.IndexOf(app) < 0)
						arr.Add(app);
				}
				Preview.SetAppearance(arr.ToArray());
			}
			((IAppearanceSelector)previewView).SelectedAppearanceNames = SelectedObjectNames;
		}
		protected override void AddObject(ArrayList ret, string item) {
			object obj1 = PreviewAppearances.GetAppearance(item);
			ret.Add(obj1);
			object obj2 = EditingAppearances.GetAppearance(item);
			ret.Add(obj2);
		}
		protected override void SetDefault() {
			previewView.BeginUpdate();
			EditingView.BeginUpdate();
			base.SetDefault();
			EditingView.EndUpdate();
			previewView.EndUpdate();
		}
		protected override void bpAppearances_ButtonClick(object sender, BaseButtonEventArgs e) {
			if(e.Button.Properties.Tag == null) return;
			switch(e.Button.Properties.Tag.ToString()) {
				case "show":
					InitAppearanceList(PreviewAppearances);
					break;
				case "select":
					SelectAll();
					break;
				case "default":
					SetDefault();
					break;
			}
		}
		protected override void lbcAppearances_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(!e.Control) return;
			if(e.KeyCode == Keys.A) SelectAll();
			if(e.KeyCode == Keys.Z) InitAppearanceList(PreviewAppearances);
			if(e.KeyCode == Keys.D) SetDefault();
		}
		#endregion
		#region Load&Save Layout
		protected override void LoadAppearances(string name) {
			PreviewAppearances.RestoreLayoutFromXml(name);
			EditingAppearances.RestoreLayoutFromXml(name);
		}
		protected override void SaveAppearances(string name) {
			PreviewAppearances.SaveLayoutToXml(name);
		}
		#endregion
	}
	interface IAppearanceSelector {
		string[] SelectedAppearanceNames { get; set; }
		event EventHandler SelectedAppearanceChanged;
	}
	class PreviewDocumentManager : DocumentManager {
		public PreviewDocumentManager(IContainer container)
			: base(container) {
		}
		public override BaseView CreateView(ViewType type) {
			switch(type) {
				case ViewType.Tabbed:
					return new PreviewTabbedView(Container);
				case ViewType.NativeMdi:
					return new PreviewNativeMdiView(Container);
				case ViewType.Widget:
					return new PreviewWidgetView(Container);
				case ViewType.WindowsUI:
					return new PreviewWindowsUIView(Container);
			}
			return base.CreateView(type);
		}
	}
	#region TabbedView Preview
	class PreviewTabbedView : DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView, IAppearanceSelector {
		public PreviewTabbedView(IContainer container)
			: base(container) {
		}
		protected override bool CanUseDocumentSelector() {
			return false;
		}
		protected override void PatchActiveChildren(Point offset) {
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			BeginUpdate();
			Orientation = Orientation.Vertical;
			DocumentGroupProperties.MaxDocuments = 3;
			var group1 = CreateDocumentGroup();
			var group2 = CreateDocumentGroup();
			DocumentGroups.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup[] {
				group1,group2
			});
			Documents.AddRange(CreateDocuments());
			SetActiveDocumentCore(Documents[0]);
			EndUpdate();
			IEnumerable<DevExpress.XtraBars.Docking2010.Views.Tabbed.IDocumentGroupInfo> infos =
				((DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedViewInfo)ViewInfo).GetGroupInfos();
			foreach(DevExpress.XtraBars.Docking2010.Views.Tabbed.IDocumentGroupInfo groupInfo in infos) {
				groupInfo.Tab.ViewInfo.HotTrackedTabPage = groupInfo.Tab.GetTabPage(2);
				break;
			}
		}
		protected DevExpress.XtraBars.Docking2010.Views.BaseDocument[] CreateDocuments() {
			return new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] { 
					CreateDocument("Active"),
					CreateDocument("Normal"),
					CreateDocument("Hot"),
					CreateDocument("Selected"),
					CreateDocument("Disabled")
				};
		}
		string[] selectedAppearanceNameCore;
		public string[] SelectedAppearanceNames {
			get { return selectedAppearanceNameCore; }
			set {
				selectedAppearanceNameCore = value;
				RaiseSelectedAppearanceChanged();
			}
		}
		protected void RaiseSelectedAppearanceChanged() {
			EventHandler handler = (EventHandler)Events[selectedAppearanceChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		object selectedAppearanceChanged = new object();
		event EventHandler IAppearanceSelector.SelectedAppearanceChanged {
			add { Events.AddHandler(selectedAppearanceChanged, value); }
			remove { Events.RemoveHandler(selectedAppearanceChanged, value); }
		}
		protected virtual DevExpress.XtraBars.Docking2010.Views.BaseDocument CreateDocument(string caption) {
			DevExpress.XtraBars.Docking2010.Views.BaseDocument document = new fakeDocument();
			document.Caption = caption;
			return document;
		}
		class fakeDocument : DevExpress.XtraBars.Docking2010.Views.Tabbed.Document {
			protected override bool CalcIsEnabled() { return Caption != "Disabled"; }
			protected override bool CalcIsVisible() { return true; }
		}
		public override bool IsFocused {
			get { return true; }
		}
		protected override bool CanProcessHooks() {
			return false;
		}
		protected override void RegisterListeners(DevExpress.XtraBars.Docking2010.DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new AppearanceSelectorUIInteractionListener());
		}
		class AppearanceSelectorUIInteractionListener : DevExpress.XtraBars.Docking2010.DragEngine.UIInteractionServiceListener {
			IDictionary<string, string> map = new Dictionary<string, string>();
			public AppearanceSelectorUIInteractionListener() {
				map.Add("Active", "HeaderActive");
				map.Add("Normal", "Header");
				map.Add("Hot", "HeaderHotTracked");
				map.Add("Selected", "HeaderSelected");
				map.Add("Disabled", "HeaderDisabled");
			}
			public DevExpress.XtraBars.Docking2010.Dragging.DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			string selectedItemName;
			public override bool OnActiveItemChanging(ILayoutElement element) {
				string currentSelection = "View";
				var documentInfo = GetBaseDocumentInfo(element);
				if(documentInfo != null)
					currentSelection = map[documentInfo.BaseDocument.Caption];
				else {
					var documentGroupInfo = GetDocumentGroupInfo(element);
					if(documentGroupInfo != null) {
						LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, View.Manager.ScreenToClient(Control.MousePosition));
						if(hitInfo.InContent && !hitInfo.InReorderingBounds)
							currentSelection = "PageClient";
						if(currentSelection != selectedItemName) {
							selectedItemName = currentSelection;
							((IAppearanceSelector)View.Manager.View).SelectedAppearanceNames = new string[] { selectedItemName };
						}
						View.Adapter.UIInteractionService.Reset();
						return false;
					}
				}
				if(currentSelection != selectedItemName) {
					selectedItemName = currentSelection;
					return true;
				}
				return false;
			}
			public override bool OnActiveItemChanged(ILayoutElement element) {
				((IAppearanceSelector)View.Manager.View).SelectedAppearanceNames = new string[] { selectedItemName };
				return base.OnActiveItemChanged(element);
			}
			public static IBaseDocumentInfo GetBaseDocumentInfo(ILayoutElement element) {
				return ((DevExpress.XtraBars.Docking2010.Dragging.IDocumentLayoutElement)element).GetElementInfo() as IBaseDocumentInfo;
			}
			public static DevExpress.XtraBars.Docking2010.Views.Tabbed.IDocumentGroupInfo GetDocumentGroupInfo(ILayoutElement element) {
				IBaseElementInfo info = ((IDocumentLayoutElement)element).GetElementInfo();
				var groupInfo = info as DevExpress.XtraBars.Docking2010.Views.Tabbed.IDocumentGroupInfo;
				if(groupInfo != null)
					return groupInfo;
				var docInfo = info as DevExpress.XtraBars.Docking2010.Views.Tabbed.IDocumentInfo;
				if(docInfo != null)
					return docInfo.GroupInfo;
				return null;
			}
		}
	}
	#endregion TabbedView Preview
	#region NativeMdiView Preview
	class PreviewNativeMdiView : DevExpress.XtraBars.Docking2010.Views.NativeMdi.NativeMdiView, IAppearanceSelector {
		public PreviewNativeMdiView(IContainer container)
			: base(container) {
		}
		public override bool IsFocused {
			get { return true; }
		}
		protected override bool CanProcessHooks() {
			return false;
		}
		protected override bool CanUseDocumentSelector() {
			return false;
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			BeginUpdate();
			Documents.AddRange(CreateDocuments());
			SetActiveDocumentCore(Documents[0]);
			EndUpdate();
		}
		protected DevExpress.XtraBars.Docking2010.Views.BaseDocument[] CreateDocuments() {
			return new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] { 
					CreateDocument("Active"),
					CreateDocument("Normal"),
				};
		}
		protected virtual DevExpress.XtraBars.Docking2010.Views.BaseDocument CreateDocument(string caption) {
			DevExpress.XtraBars.Docking2010.Views.BaseDocument document = new fakeDocument();
			document.Caption = caption;
			return document;
		}
		class fakeDocument : DevExpress.XtraBars.Docking2010.Views.NativeMdi.Document {
			protected override bool CalcIsEnabled() { return true; }
			protected override bool CalcIsVisible() { return true; }
			protected override bool Borderless { get { return false; } }
		}
		string[] selectedAppearanceNameCore;
		public string[] SelectedAppearanceNames {
			get { return selectedAppearanceNameCore; }
			set {
				selectedAppearanceNameCore = value;
				RaiseSelectedAppearanceChanged();
			}
		}
		protected void RaiseSelectedAppearanceChanged() {
			EventHandler handler = (EventHandler)Events[selectedAppearanceChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		object selectedAppearanceChanged = new object();
		event EventHandler IAppearanceSelector.SelectedAppearanceChanged {
			add { Events.AddHandler(selectedAppearanceChanged, value); }
			remove { Events.RemoveHandler(selectedAppearanceChanged, value); }
		}
		protected override void RegisterListeners(BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new AppearanceSelectorUIInteractionListener());
		}
		class AppearanceSelectorUIInteractionListener : DevExpress.XtraBars.Docking2010.DragEngine.UIInteractionServiceListener {
			public DevExpress.XtraBars.Docking2010.Dragging.DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			public override bool OnActiveItemChanged(ILayoutElement element) {
				((IAppearanceSelector)View.Manager.View).SelectedAppearanceNames = new string[] { "View" };
				return base.OnActiveItemChanged(element);
			}
		}
	}
	#endregion NativeMdiView Preview
	#region WidgetView Preview
	class PreviewWidgetView : DevExpress.XtraBars.Docking2010.Views.Widget.WidgetView, IAppearanceSelector {
		public PreviewWidgetView(IContainer container)
			: base(container) {
		}
		protected override bool CanUseDocumentSelector() {
			return false;
		}
		protected override void PatchActiveChildren(Point offset) {
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			BeginUpdate();
			Orientation = Orientation.Vertical;
			StackGroupProperties.Capacity = 1;
			var group1 = CreateStackGroup();
			var group2 = CreateStackGroup();
			StackGroups.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.StackGroup[] {
				group1,group2
			});
			Documents.AddRange(CreateDocuments());
			SetActiveDocumentCore(Documents[0]);
			EndUpdate();
		}
		protected DevExpress.XtraBars.Docking2010.Views.BaseDocument[] CreateDocuments() {
			return new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] { 
					CreateDocument("Active"),
					CreateDocument("Normal"),
				};
		}
		string[] selectedAppearanceNameCore;
		public string[] SelectedAppearanceNames {
			get { return selectedAppearanceNameCore; }
			set {
				selectedAppearanceNameCore = value;
				RaiseSelectedAppearanceChanged();
			}
		}
		protected void RaiseSelectedAppearanceChanged() {
			EventHandler handler = (EventHandler)Events[selectedAppearanceChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		object selectedAppearanceChanged = new object();
		event EventHandler IAppearanceSelector.SelectedAppearanceChanged {
			add { Events.AddHandler(selectedAppearanceChanged, value); }
			remove { Events.RemoveHandler(selectedAppearanceChanged, value); }
		}
		protected virtual DevExpress.XtraBars.Docking2010.Views.BaseDocument CreateDocument(string caption) {
			DevExpress.XtraBars.Docking2010.Views.BaseDocument document = new fakeDocument();
			document.Caption = caption;
			return document;
		}
		class fakeDocument : DevExpress.XtraBars.Docking2010.Views.Widget.Document, DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner {
			protected override void OnCreate() {
				base.OnCreate();
				Width = 200;
				Height = 200;
			}
			protected override bool CalcIsEnabled() { return Caption != "Disabled"; }
			protected override bool CalcIsVisible() { return true; }
			bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.IsSelected {
				get { return Caption != "Active"; }
			}
		}
		public override bool IsFocused {
			get { return true; }
		}
		protected override bool CanProcessHooks() {
			return false;
		}
		protected override void RegisterListeners(DevExpress.XtraBars.Docking2010.DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new AppearanceSelectorUIInteractionListener());
		}
		class AppearanceSelectorUIInteractionListener : DevExpress.XtraBars.Docking2010.DragEngine.UIInteractionServiceListener {
			IDictionary<string, string> map = new Dictionary<string, string>();
			public AppearanceSelectorUIInteractionListener() {
				map.Add("Active", "ActiveDocumentCaption");
				map.Add("Normal", "DocumentCaption");
			}
			public DevExpress.XtraBars.Docking2010.Dragging.DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			string selectedItemName;
			public override bool OnActiveItemChanging(ILayoutElement element) {
				var documentInfo = GetBaseDocumentInfo(element);
				if(documentInfo != null)
					selectedItemName = map[documentInfo.BaseDocument.Caption];
				return true;
			}
			public override bool OnActiveItemChanged(ILayoutElement element) {
				((IAppearanceSelector)View.Manager.View).SelectedAppearanceNames = new string[] { selectedItemName };
				return base.OnActiveItemChanged(element);
			}
			public static IBaseDocumentInfo GetBaseDocumentInfo(ILayoutElement element) {
				return ((DevExpress.XtraBars.Docking2010.Dragging.IDocumentLayoutElement)element).GetElementInfo() as IBaseDocumentInfo;
			}
			public static DevExpress.XtraBars.Docking2010.Views.Widget.IStackGroupInfo GetDocumentGroupInfo(ILayoutElement element) {
				IBaseElementInfo info = ((IDocumentLayoutElement)element).GetElementInfo();
				var groupInfo = info as DevExpress.XtraBars.Docking2010.Views.Widget.IStackGroupInfo;
				if(groupInfo != null)
					return groupInfo;
				return null;
			}
		}
	}
	#endregion WidgetView Preview
	#region WindowsUIView Preview
	class PreviewWindowsUIView : DevExpress.XtraBars.Docking2010.Views.WindowsUI.WindowsUIView, IAppearanceSelector {
		public PreviewWindowsUIView(IContainer container)
			: base(container) {
			Caption = "Windows App";
		}
		protected override bool CanUseDocumentSelector() {
			return false;
		}
		int canUseSplash = 0;
		protected override bool CanUseSplashScreen() {
			return canUseSplash > 0;
		}
		protected override void PatchActiveChildren(Point offset) {
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			BeginUpdate();
			DevExpress.XtraBars.Docking2010.Views.WindowsUI.PageGroup group1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.PageGroup();
			group1.Items.AddRange(CreateDocuments());
			ContentContainers.Add(group1);
			Controller.Activate(group1);
			ContentContainerActions.Add(new FakeAction() { Caption = "Context Action", Type = DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionType.Context });
			ContentContainerActions.Add(new FakeAction() { Caption = "Navigation Action", Type = DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionType.Navigation });
			ContentContainerActions.Add(new SplashScreenAction() { Caption = "Show SplashScreen" });
			EndUpdate();
			NavigationBarsShown += new EventHandler(PreviewWindowsUIViewNavigationBarsShown);
		}
		void PreviewWindowsUIViewNavigationBarsShown(object sender, EventArgs e) {
			SelectedAppearanceNames = new string[] { "ActionsBar" };
		}
		protected DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document[] CreateDocuments() {
			return new DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document[] { 
					CreateDocument("Page1"),
					CreateDocument("Page2"),
				};
		}
		[DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionGroup("Delegate", DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionType.Default)]
		class FakeAction : DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseDelegateAction, DevExpress.XtraBars.Docking2010.Views.WindowsUI.IActionLayout {
			public FakeAction() : base(Can, Do) { Edge = DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionEdge.Left; }
			static bool Can(DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainer c) { return true; }
			static void Do(DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainer c) { }
			public DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionEdge Edge { get; set; }
			public DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionType Type { get; set; }
		}
		[DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionGroup("Delegate", DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionType.Context)]
		class SplashScreenAction : DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseDelegateAction {
			public SplashScreenAction() : base(Can, Do) { Behavior = DevExpress.XtraBars.Docking2010.Views.WindowsUI.ActionBehavior.HideBarOnClick; }
			static bool Can(DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainer c) { return true; }
			static void Do(DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainer c) {
				((PreviewWindowsUIView)c.Manager.View).canUseSplash++;
				using(((PreviewWindowsUIView)c.Manager.View).ShowSplashScreen()) {
					System.Threading.Thread.Sleep(1500);
				}
				((PreviewWindowsUIView)c.Manager.View).canUseSplash--;
			}
		}
		string[] selectedAppearanceNameCore;
		public string[] SelectedAppearanceNames {
			get { return selectedAppearanceNameCore; }
			set {
				selectedAppearanceNameCore = value;
				RaiseSelectedAppearanceChanged();
			}
		}
		protected void RaiseSelectedAppearanceChanged() {
			EventHandler handler = (EventHandler)Events[selectedAppearanceChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		object selectedAppearanceChanged = new object();
		event EventHandler IAppearanceSelector.SelectedAppearanceChanged {
			add { Events.AddHandler(selectedAppearanceChanged, value); }
			remove { Events.RemoveHandler(selectedAppearanceChanged, value); }
		}
		protected virtual DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document CreateDocument(string caption) {
			DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document document = new fakeDocument();
			document.Caption = caption;
			return document;
		}
		class fakeDocument : DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document {
			protected override void OnCreate() {
				base.OnCreate();
			}
			protected override bool CalcIsEnabled() { return Caption != "Disabled"; }
			protected override bool CalcIsVisible() { return true; }
		}
		public override bool IsFocused {
			get { return true; }
		}
		protected override bool CanProcessHooks() {
			return false;
		}
		protected override void RegisterListeners(DevExpress.XtraBars.Docking2010.DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new AppearanceSelectorUIInteractionListener());
		}
		class AppearanceSelectorUIInteractionListener : DevExpress.XtraBars.Docking2010.DragEngine.UIInteractionServiceListener {
			public DevExpress.XtraBars.Docking2010.Dragging.DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			string selectedItemName;
			public override bool OnActiveItemChanging(ILayoutElement element) {
				var documentInfo = GetBaseDocumentInfo(element);
				if(documentInfo != null)
					selectedItemName = "View";
				var contentContainerInfo = GetContentContainerInfo(element);
				if(contentContainerInfo != null)
					selectedItemName = "View";
				var contentContainerHeaderInfo = GetContentContainerHeaderInfo(element);
				if(contentContainerHeaderInfo != null)
					selectedItemName = "Caption";
				return true;
			}
			public override bool OnActiveItemChanged(ILayoutElement element) {
				((IAppearanceSelector)View.Manager.View).SelectedAppearanceNames = new string[] { selectedItemName };
				return base.OnActiveItemChanged(element);
			}
			public static IBaseDocumentInfo GetBaseDocumentInfo(ILayoutElement element) {
				return ((DevExpress.XtraBars.Docking2010.Dragging.IDocumentLayoutElement)element).GetElementInfo() as IBaseDocumentInfo;
			}
			public static DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainerInfo GetContentContainerInfo(ILayoutElement element) {
				return ((DevExpress.XtraBars.Docking2010.Dragging.IDocumentLayoutElement)element).GetElementInfo() as DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainerInfo;
			}
			public static DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainerHeaderInfo GetContentContainerHeaderInfo(ILayoutElement element) {
				return ((DevExpress.XtraBars.Docking2010.Dragging.IDocumentLayoutElement)element).GetElementInfo() as DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainerHeaderInfo;
			}
		}
	}
	#endregion WidgetView Preview
}
