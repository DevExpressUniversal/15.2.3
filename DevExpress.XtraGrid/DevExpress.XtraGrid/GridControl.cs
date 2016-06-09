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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Configuration;
using System.Timers;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Win;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Dragging;
using DevExpress.Accessibility;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Internal;
using DevExpress.XtraPrintingLinks;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Printing;
using System.Reflection;
using DevExpress.Utils.Gesture;
using System.Diagnostics;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraExport;
using DevExpress.Export;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid {
	[DXToolboxItem(true), Designer("DevExpress.XtraGrid.Design.GridControlDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(IDesigner)),
	 Docking(DockingBehavior.Ask),
	 ComplexBindingProperties("DataSource", "DataMember"),
	 Description("A data-aware control that displays data in one of several views, enables editing data, provides data filtering, sorting, grouping and summary calculation features."),
	 ToolboxTabName(AssemblyInfo.DXTabData)
]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "All")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "GridControl")]
	public class GridControl : EditorContainer,
		IPrintableEx, IPrintHeaderFooter,
		INavigatableControl, IToolTipControlClient, ISupportLookAndFeel, DevExpress.Utils.Menu.IDXManagerPopupMenu, IViewController, IFilteredComponent, IBoundControl, ISupportXtraSerializer, IGestureClient, IGuideDescription, IFilteredComponentColumnsClient, ILogicalOwner {
		bool useEmbeddedNavigator;
		public const int InvalidRowHandle = DevExpress.Data.DataController.InvalidRow;
		public const int AutoFilterRowHandle = BaseListSourceDataController.FilterRow;
		public const int NewItemRowHandle = DevExpress.Data.CurrencyDataController.NewItemRow;
		UserLookAndFeel lookAndFeel;
		NavigatableControlHelper navigatableHelper = new NavigatableControlHelper();
		ViewRepository viewRepository;
		GridLevelTree levelTree;
		object dataSource;
		string dataMember;
		DefaultBoolean allowRestoreSelectionAndFocusedRow = DefaultBoolean.Default;
		GridControlViewCollection views;
		BaseView defaultView;
		int lockUpdate, lockFireChanged = 0;
		BaseView mouseCaptureOwner, focusedView;
		bool gridDisposing;
		bool showOnlyPredefinedDetails;
		MouseButtons lastMouseButtons;
		InfoCollection availableViews;
		static IPopupServiceControl popupServiceControl = new DevExpress.XtraEditors.Controls.HookPopupController();
		private static readonly object processGridKey = new object();
		private static readonly object focusedViewChanged = new object();
		private static readonly object dataSourceChanged = new object();
		private static readonly object defaultViewChanged = new object();
		private static readonly object viewRegistered = new object();
		private static readonly object viewRemoved = new object();
		private static readonly object load = new object();
		DragController _dragController;
		ControlNavigator _embeddedNavigator;
		bool formsUseDefaultLookAndFeel = false; 
		static GridControl() {
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinGrid));
		}
		public GridControl() {
			Permissions.Request();
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
			RepositoryItemGridLookUpEdit.RegisterGridLookUpEdit();
			RepositoryItemSearchLookUpEdit.RegisterSearchLookUpEdit();
			this.showOnlyPredefinedDetails = false;
			this.levelTree = new GridLevelTree(this);
			this.viewRepository = new ViewRepository(this);
			this.viewRepository.Changed += new CollectionChangeEventHandler(OnViewRepository_Changed);
			this.useEmbeddedNavigator = false;
			this._dragController = new DragController(this);
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.availableViews = new InfoCollection();
			RegisterAvailableViews();
			this.lastMouseButtons = MouseButtons.None;
			this.gridDisposing = false;
			this.mouseCaptureOwner = null;
			this.focusedView = null;
			this.dataSource = null;
			this.dataMember = "";
			this.views = new GridControlViewCollection();
			this.defaultView = focusedView = null;
			this.BackColor = SystemColors.Control;
			SetStyle(DefaultGridControlStyles, true);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		const ControlStyles DefaultGridControlStyles =
			ControlStyles.UserMouse | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick |
			ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.SupportsTransparentBackColor;
		Cursor userCursor = null;
		bool inSetCursor = false;
		internal Cursor GetDefaultCursor() { return userCursor == null ? Cursors.Default : userCursor; }
		internal void SetCursor(Cursor cursor) {
			this.inSetCursor = true;
			try {
				if(cursor == null) cursor = GetDefaultCursor();
				Cursor = cursor == null ? Cursors.Default : cursor;
			}
			finally {
				this.inSetCursor = false;
			}
		}
		public override DockStyle Dock {
			get { return base.Dock; }
			set {
				if(IsSplitGrid) value = DockStyle.Fill;
				base.Dock = value;
			}
		}
		DevExpress.XtraLayout.LayoutControlItem GetOwnerItem(Control control) {
			DevExpress.XtraLayout.LayoutControl lc = control.Parent as DevExpress.XtraLayout.LayoutControl;
			if(lc != null) return lc.GetItemByControl(control);
			return null;
		}
		public GridSplitContainer CreateSplitContainer() {
			if(IsSplitGrid) return null;
			GridSplitContainer split = new GridSplitContainer();
			DevExpress.XtraLayout.LayoutControlItem lci = GetOwnerItem(this);
			bool prevAllowRemove = true;
			bool prevTextVisible = false;
			if(lci != null && lci.Owner != null) {
				prevAllowRemove = ((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent;
				prevTextVisible = lci.TextVisible;
				((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent = false;
				lci.BeginInit();
			}
			split.Grid = this;
			split.Anchor = this.Anchor;
			split.Bounds = this.Bounds;
			split.Dock = this.Dock;
			int zindex = this.Parent == null ? 0 : Parent.Controls.GetChildIndex(this);
			Control parent = this.Parent;
			BindingContext context = BindingContext;
			split.Panel1.Controls.Add(this);
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.Dock = DockStyle.Fill;
			if(parent != null) {
				parent.Controls.Add(split);
				parent.Controls.SetChildIndex(split, zindex);
			}
			if(lci != null && lci.Owner != null) {
				lci.Control = split;
				((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent = prevAllowRemove;
				lci.TextVisible = prevTextVisible;
				lci.EndInit();
			}
			return split;
		}
		public void RemoveSplitContainer() {
			if(!IsSplitGrid) return;
			GridSplitContainer split = SplitContainer;
			Control parent = split.Parent;
			int zindex = parent == null ? 0 : parent.Controls.GetChildIndex(split);
			DevExpress.XtraLayout.LayoutControlItem lci = GetOwnerItem(this);
			bool prevAllowRemove = true;
			bool prevTextVisible = false;
			if(lci != null && lci.Owner != null) {
				prevAllowRemove = ((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent;
				prevTextVisible = lci.TextVisible;
				((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent = false;
				lci.BeginInit();
			}
			AnchorStyles anchor = split.Anchor;
			Rectangle bounds = split.Bounds;
			DockStyle dock = split.Dock;
			BindingContext context = BindingContext;
			if(parent != null) {
				parent.Controls.Add(this);
			}
			split.Grid = null;
			split.Dispose();
			this.Anchor = anchor;
			this.Dock = dock;
			this.Bounds = bounds;
			if(parent != null) {
				parent.Controls.SetChildIndex(this, zindex);
			}
			if(lci != null && lci.Owner != null) {
				lci.Control = this;
				((DevExpress.XtraLayout.ILayoutDesignerMethods)lci.Owner).AllowHandleControlRemovedEvent = prevAllowRemove;
				lci.TextVisible = prevTextVisible;
				lci.EndInit();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsSplitGrid {
			get {
				if(SplitContainer != null) return true;
				return false;
			}
		}
		protected internal GridSplitContainer SplitContainer {
			get {
				SplitGroupPanel panel = Parent as SplitGroupPanel;
				if(panel != null) return panel.Parent as GridSplitContainer;
				return null;
			}
		}
		public override Cursor Cursor {
			get { return base.Cursor; }
			set {
				base.Cursor = value;
				if(!inSetCursor && !UseWaitCursor)
					this.userCursor = Cursor;
			}
		}
		protected override void OnCursorChanged(EventArgs e) {
			base.OnCursorChanged(e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.searchControl = null;
				this.userCursor = null;
				this.gridDisposing = true;
				if(_embeddedNavigator != null) {
					_embeddedNavigator.SizeChanged -= new EventHandler(OnEmbeddedNavigator_SizeChanged);
					_embeddedNavigator.Dispose();
					this._embeddedNavigator = null;
				}
				while(Views.Count > 0)
					RemoveView(Views[0]);
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					LookAndFeel.Dispose();
				}
				LevelTree.Dispose();
				DestroyAvailableViews();
				ViewRepository.Changed -= new CollectionChangeEventHandler(OnViewRepository_Changed);
				ViewRepository.Dispose();
				DestroyDragArrows();
				DestroyPrinting();
			}
			base.Dispose(disposing);
		}
		protected void DestroyPrinting() {
			PrintWrapper = null;
			if(printer != null) {
				printer.Dispose();
				printer = null;
			}
		}
#if DXWhidbey
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			BaseGridAccessibleObject view = (DefaultView != null) ? DefaultView.DXAccessible as BaseGridAccessibleObject : null;
			if(view == null) return base.GetAccessibilityObjectById(objectId);
			return view.GetAccessibleObjectById(objectId, -1);
		}
#endif
		public void AccessibleNotifyClients(AccessibleEvents accEvents, int childID) {
#if DXWhidbey
			AccessibilityNotifyClients(accEvents, 10, childID);
#endif
		}
		protected virtual ControlNavigator CreateEmbeddedNavigator() {
			GridControlNavigator nav = new GridControlNavigator(this);
			nav.SizeChanged += new EventHandler(OnEmbeddedNavigator_SizeChanged);
			return nav;
		}
		protected virtual ViewRepository ViewRepository { get { return viewRepository; } }
		protected internal virtual DragController DragController { get { return _dragController; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ViewRepositoryCollection ViewCollection { get { return ViewRepository.Views; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlUseEmbeddedNavigator"),
#endif
 Browsable(true), DefaultValue(false),
		DXCategory(CategoryName.Appearance)]
		public virtual bool UseEmbeddedNavigator {
			get { return useEmbeddedNavigator; }
			set {
				if(UseEmbeddedNavigator == value) return;
				useEmbeddedNavigator = value;
				if(IsLoading || DefaultView == null) return;
				DefaultView.LayoutChangedSynchronized();
			}
		}
		bool useDisabledStatePainterCore = true;
		[DefaultValue(true), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlUseDisabledStatePainter"),
#endif
 DXCategory(CategoryName.Appearance)]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlShowOnlyPredefinedDetails"),
#endif
 Browsable(true), DefaultValue(false), DXCategory(CategoryName.Behavior)]
		public virtual bool ShowOnlyPredefinedDetails {
			get { return showOnlyPredefinedDetails; }
			set {
				if(ShowOnlyPredefinedDetails == value) return;
				showOnlyPredefinedDetails = value;
				if(IsLoading || DefaultView == null) return;
				NormalView(MainView);
				MainView.CollapseAllDetailsCore();
			}
		}
		[Browsable(false), DefaultValue(false)]
		public bool FormsUseDefaultLookAndFeel { get { return formsUseDefaultLookAndFeel; } set { formsUseDefaultLookAndFeel = value; } } 
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public GridLevelTree LevelTree { get { return levelTree; } }
		[Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlEmbeddedNavigator"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXCategory(CategoryName.Appearance)]
		public virtual ControlNavigator EmbeddedNavigator {
			get {
				if(_embeddedNavigator == null) {
					this._embeddedNavigator = CreateEmbeddedNavigator();
					this.Controls.Add(_embeddedNavigator);
				}
				return _embeddedNavigator;
			}
		}
		[Browsable(false)]
		public bool GridDisposing { get { return gridDisposing; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void LockFireChanged() {
			this.lockFireChanged++;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void UnlockFireChanged() {
			this.lockFireChanged--;
		}
		internal bool IsLockFireChanged { get { return lockFireChanged != 0; } }
		protected internal virtual void FireChanged(object component) {
			if(this.lockFireChanged != 0 || IsLoading) return;
			if(LookUpOwner != null && LookUpOwner.IsDesignMode) LookUpOwner.FireChangedCore();
			if(!DesignMode) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null)
				srv.OnComponentChanged(component, null, null, null);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void FireChanged() {
			FireChanged(this);
		}
		protected internal void ZoomView(BaseView view) {
			if(DefaultView == view) return;
			if(string.IsNullOrEmpty(view.LevelName) && DesignMode) return;
			BaseView parentView = view.ParentView;
			if(parentView == null && DefaultView != null) {
				DefaultView.InternalSetViewRectCore(Rectangle.Empty);
			}
			while(parentView != null) {
				try {
					parentView.InternalSetViewRectCore(Rectangle.Empty);
				}
				finally {
				}
				parentView = parentView.ParentView;
			}
			SetDefaultView(view);
			UpdateViewCollection();
			DefaultView.InternalSetViewRectCore(ClientRectangle);
		}
		public override void BeginInit() {
			foreach(BaseView view in ViewCollection) { view.OnBeginInit(); }
			base.BeginInit();
		}
		void OnViewRepository_Changed(object sender, CollectionChangeEventArgs e) {
			BaseView view = e.Element as BaseView;
			if(e.Action == CollectionChangeAction.Add) {
				if(view != null) {
					view.SetGridControl(this);
					view.OnLoaded();
				}
			}
		}
		protected virtual void UpdateViewCollection() {
			if(!DesignMode) return;
			while(Views.Count > 1) Views.RemoveAtCore(1);
			if(LevelTree.Contains(DefaultView)) Views.AddCore(DefaultView);
		}
		protected internal void NormalView(BaseView view) {
			if(GridDisposing) return;
			if(view != null && (!views.Contains(view) || view.ViewDisposing))
				view = null;
			if(view == null) view = MainView;
			if(DefaultView == view) return;
			if(view == null) {
				SetDefaultView(null);
				return;
			}
			DefaultView.InternalSetViewRectCore(Rectangle.Empty);
			SetDefaultView(view);
			DefaultView.InternalSetViewRectCore(ClientRectangle);
			UpdateViewCollection();
		}
		GridEditorContainerHelper IViewController.EditorHelper { get { return EditorHelper; } }
		protected new internal GridEditorContainerHelper EditorHelper { get { return base.EditorHelper as GridEditorContainerHelper; } }
		protected override EditorContainerHelper CreateHelper() {
			return new GridEditorContainerHelper(this);
		}
		protected virtual void UpdateViewStyles() {
			BeginUpdate();
			try {
				for(int n = 0; n < ViewCollection.Count; n++) {
					ViewCollection[n].OnLookAndFeelChanged();
				}
				for(int n = 0; n < Views.Count; n++) {
					Views[n].CheckInfo();
				}
			}
			finally {
				EndUpdate(true);
			}
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			CreateMainView();
		}
		protected virtual void CreateMainView() {
			if(MainView == null) {
				MainView = CreateDefaultView();
			}
			else {
				if(defaultView != null && focusedView != null) return;
			}
			this.defaultView = MainView;
			this.focusedView = MainView;
		}
		public virtual void ForceInitialize() {
			OnLoaded();
			ActivateDataSource();
		}
		internal bool IsInitializedCore { get { return IsInitialized; } }
		bool allowLayoutEvent = true;
		protected internal bool AllowLayoutEvent { get { return allowLayoutEvent; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridControlBindingContext")]
#endif
		public override BindingContext BindingContext {
			get {
				BindingContext con = base.BindingContext;
				if(con == null && IsDesignMode) {
					con = new BindingContext();
					base.BindingContext = con;
				}
				return con;
			}
		}
		bool isLoadedCore;
		[Browsable(false)]
		public bool IsLoaded {
			get { return isLoadedCore; }
			private set { isLoadedCore = value; }
		}
		protected override void OnLoaded() {
			if(IsLoading || IsInitialized) return;
			LockFireChanged();
			try {
				this.allowLayoutEvent = false;
				CreateMainView();
				BeginUpdate();
				try {
					ForceViewInitialize();
					DefaultView.Handler.ProcessEvent(EventType.Resize, ClientRectangle);
					base.OnLoaded();
					if(IsDesignMode) {
						if(BindingContext == null) BindingContext = new BindingContext();
						ActivateDataSource();
					}
					else {
						if(Created) ActivateDataSource();
					}
				}
				finally {
					EndUpdate();
				}
			}
			finally {
				this.allowLayoutEvent = true;
				UnlockFireChanged();
			}
			RaiseLoad();
			OnDataSourceChanged(); 
			IsLoaded = true;
			RaiseViewGridLoadComplete();
		}
		protected override bool FireOnLoadOnPaint { get { return true; } }
		internal XtraGrid.Blending.XtraGridBlending OldAlphaBlending = null;
		protected virtual void ForceViewInitialize() {
			foreach(BaseView view in Views) {
				view.OnLoaded();
			}
			foreach(BaseView view in ViewCollection) {
				view.OnLoaded();
			}
		}
		protected virtual void RaiseViewGridLoadComplete() {
			foreach(BaseView view in Views) {
				view.OnGridLoadComplete();
			}
			foreach(BaseView view in ViewCollection) {
				view.OnGridLoadComplete();
			}
		}
		BaseView mainView = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public BaseView MainView {
			get { return mainView; }
			set {
				if(value == MainView || value == null) return;
				ViewCollection.Add(value);
				value.SetGridControl(this);
				if(value.GridControl != this) return;
				if(this.mainView != null) this.mainView.Disconnect(this);
				while(Views.Count > 0)
					RemoveView(Views[0], false);
				this.mainView = value;
				this.mainView.BeginUpdate();
				try {
					this.mainView.Connect(this);
					this.mainView.AddToGridControl();
					RegisterView(value);
					SetDefaultView(MainView);
					this.focusedView = MainView;
					RaiseOnFocusedViewChanged(new ViewFocusEventArgs(null, MainView));
					if(!IsLoading) {
						DefaultView.OnLoaded();
						DefaultView.Handler.ProcessEvent(EventType.Resize, ClientRectangle);
						DefaultView.OnVisibleChanged();
						DefaultView.LayoutChangedSynchronized();
					}
					ActivateDataSource();
				}
				finally {
					this.mainView.EndUpdateCore(true);
				}
				OnDataSourceChanged();
			}
		}
		[Browsable(false)]
		public BaseView DefaultView { get { return defaultView; } }
		[Browsable(false)]
		public virtual bool IsFocused { get { return Focused || (FocusedView != null && FocusedView.IsFocusedView); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView FocusedView {
			get {
				if(!views.Contains(focusedView))
					focusedView = null;
				return focusedView;
			}
			set {
				if(FocusedView == value) return;
				BaseView prevFocused = FocusedView;
				if(FocusedView != null) {
					FocusedView.OnLostFocus();
					FocusedView.CloseEditor();
				}
				focusedView = value;
				if(FocusedView != null) {
					FocusedView.OnGotFocus();
				}
				RaiseOnFocusedViewChanged(new ViewFocusEventArgs(prevFocused, FocusedView));
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete(ObsoleteText.SRGridControl_KeyboardFocusView)]
		public BaseView KeyboardFocusView { get { return FocusedView; } set { FocusedView = value; } }
		protected internal virtual void FocusViewByMouse(BaseView view) {
			if(view == FocusedView) return;
			if(FocusedView != null) {
				if(!FocusedView.CheckCanLeaveCurrentRow(true)) return;
			}
			FocusedView = view;
		}
		private void DataSetChangedEvent(object sender, CollectionChangeEventArgs e) {
			if(e.Element is DataTable) {
				string name = (e.Element as DataTable).TableName;
				if(name == DataMember && !IsLoading && IsInitialized)
					ActivateDataSource();
			}
		}
		private void RemoveDataSetEvents() {
			DataSet data = GetDataSet();
			if(data != null)
				data.Tables.CollectionChanged -= new CollectionChangeEventHandler(DataSetChangedEvent);
		}
		private void AddDataSetEvents() {
			DataSet data = GetDataSet();
			if(data != null) {
				data.Tables.CollectionChanged += new CollectionChangeEventHandler(DataSetChangedEvent);
			}
		}
		private DataSet GetDataSet() {
			object value = DataSource;
			if(value is IDXCloneable) return null; 
			if(!(value is DataSet) && (value is IListSource))
				value = (value as IListSource).GetList();
			if(value is DataViewManager)
				value = (value as DataViewManager).DataSet;
			return value as DataSet;
		}
		protected bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			if(dataSource is DataSet) return true;
			if(dataSource is System.Data.DataView) {
				System.Data.DataView dv = dataSource as System.Data.DataView;
				if(dv.Table == null) return false;
				return true;
			}
			if(dataSource is System.Data.DataTable) return true;
			if(dataSource is IEnumerable) return true;
			return false;
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DefaultView == null || DefaultView.DXAccessible == null) return base.CreateAccessibilityInstance();
			DefaultView.DXAccessible.ParentCore = null;
			return DefaultView.DXAccessible.Accessible;
		}
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		internal bool allowFastScroll = true;
		protected override void OnBackgroundImageChanged(EventArgs e) {
			this.allowFastScroll = BackgroundImage == null;
			base.OnBackgroundImageChanged(e);
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			if(IsInitialized && DataSource != null) {
				ActivateDataSource();
			}
			base.OnBindingContextChanged(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null)
					FocusedView.OnLostFocus();
				base.OnLostFocus(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			if(IsAllowInvalidateOnFocus())
				Invalidate();
		}
		protected override void OnGotFocus(EventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null)
					FocusedView.OnGotFocus();
				base.OnGotFocus(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			if(IsAllowInvalidateOnFocus())
				Invalidate();
		}
		object lastPaintedFocusState = null;
		bool IsAllowInvalidateOnFocus() {
			return ((lastPaintedFocusState == null || (bool)lastPaintedFocusState != IsFocused) &&
				(EditorHelper != null && EditorHelper.InternalFocusLock == 0));
		}
		protected override void OnContainerEnter(EventArgs e) {
			base.OnContainerEnter(e);
		}
		protected override void OnContainerLeave(EventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null)
					FocusedView.OnLeave();
				base.OnContainerLeave(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			Invalidate();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			if(!Enabled && FocusedView != null) FocusedView.HideEditor();
		}
		protected override void OnValidating(CancelEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null && FocusedView.Editable) {
					if(FocusedView.IsClosingEditor) return;
					if(!Enabled) FocusedView.HideEditor();
					if(!e.Cancel && !FocusedView.ValidateEditing()) 
						e.Cancel = true;
				}
			}
			catch(HideException) {
				e.Cancel = true;
			}
			finally {
				EditorHelper.EndAllowHideException();
			}
			base.OnValidating(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(dragArrows != null) dragArrows.Reset();
			EditorHelper.HideHint();
			if(MainView != null)
				DefaultView.OnVisibleChanged();
		}
		internal void CreateHandleCore() {
			if(IsHandleCreated) return;
			CreateHandle();
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
		}
		internal object lastKeyMessage = null;
		GestureHelper touchHelper;
		protected override void WndProc(ref Message m) {
			if(touchHelper == null) touchHelper = new GestureHelper(this);
			if(DevExpress.Utils.Drawing.Helpers.SmartDoubleBufferPainter.Default.ProcessMessage(this, ref m)) return;
			lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			if(DevExpress.XtraEditors.Senders.BaseSender.RequireShowEditor(ref m)) MainView.ShowEditor();
			if(NativeMethods.IsKeyboardContextMenuMessage(m)) {
				if(FocusedView != null) {
					if(FocusedView.OnContextMenuClick()) return;
				}
			}
			if(touchHelper.WndProc(ref m)) return;
			if(!CheckProcessMsg(ref m)) {
				base.WndProc(ref m);
			}
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		ControlPaintHelper paintHelper = null;
		ControlPaintHelper PaintHelper {
			get {
				if(paintHelper == null) paintHelper = new ControlPaintHelper(this, OnPaint, OnPaintBackground, true, GetStyle(ControlStyles.Opaque));
				return paintHelper;
			}
		}
		protected virtual bool CheckProcessMsg(ref Message m) {
			if(m.Msg == MSG.WM_PAINT) {
				PaintHelper.ProcessWMPaint(ref m);
				return true;
			}
			return false;
		}
		protected virtual bool IsPaintAnimateItems() {
			for(int i = 0; i < views.Count; i++) {
				if(!IsVisibleView(views[i])) continue;
				if(!views[i].ViewInfo.PaintAnimatedItems) return false;
			}
			return true;
		}
		int scrollCounter = 0;
		internal void IncrementScrollCounters() {
			scrollCounter++;
		}
		internal DateTime lastPaintTime = DateTime.MinValue;
		protected override void OnPaint(PaintEventArgs e) {
			this.scrollCounter = 0;
			this.lastPaintTime = DateTime.Now;
			base.OnPaint(e);
			bool paintAnimatedItems = IsPaintAnimateItems();
			lock(this) {
				GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e, Handle));
				cache.Cache.CheckCache();
				try {
					DevExpress.Utils.Paint.XPaint.Graphics.BeginPaint(e.Graphics);
					foreach(BaseView gp in views) {
						if(!IsVisibleView(gp)) continue;
						if(e.ClipRectangle.IntersectsWith(gp.ViewRect)) {
							gp.ViewInfo.PaintAnimatedItems = paintAnimatedItems;
							gp.Draw(cache);
						}
					}
					if(!Enabled && UseDisabledStatePainter) BackgroundPaintHelper.PaintDisabledControl(MainView != null && MainView.IsSkinned ? MainView.ElementsLookAndFeel : null, cache, ClientRectangle);
				}
				finally {
					DevExpress.Utils.Paint.XPaint.Graphics.EndPaint(e.Graphics);
				}
			}
			RaisePaintEvent(this, e);
		}
		protected override void OnMouseCaptureChanged(EventArgs e) {
			base.OnMouseCaptureChanged(e);
		}
		[Browsable(false)]
		public override Color BackColor {
			get {
				if(MainView != null && MainView.BackColor != Color.Empty) return MainView.BackColor;
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		protected override void OnResize(EventArgs e) {
			if(IsLoading || MainView == null || !IsInitialized) return;
			try {
				if(FocusedView != DefaultView && FocusedView != null) FocusedView.SetDefaultState();
				if(!ClientRectangle.IsEmpty && DefaultView.Handler != null) DefaultView.Handler.ProcessEvent(EventType.Resize, ClientRectangle);
			}
			catch(HideException) { }
			base.OnResize(e);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(!IsLoading) {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null && host.Loading) return; 
				CreateMainView();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(MainView != null) MainView.CreateHandles();
		}
		protected override void RaiseEditorKeyDown(KeyEventArgs e) {
			if(ProcessGridKeys(e, false)) return;
			base.RaiseEditorKeyDown(e);
			if(e.Handled) return;
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null) {
					e.Handled = (FocusedView.Handler.ProcessEvent(EventType.ProcessKey, e) == EventResult.Handled);
				}
			}
			catch(HideException) {
				e.Handled = true;
			}
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected virtual bool ProcessGridKeys(KeyEventArgs keys, bool onlyEvent) {
			try {
				EditorHelper.BeginAllowHideException();
				if(FocusedView != null) {
					KeyEventHandler handler = (KeyEventHandler)this.Events[processGridKey];
					if(handler != null) {
						handler(this, keys);
						if(keys.Handled) return true;
					}
					if(onlyEvent) {
						if(IsTabPressed(keys.KeyData))
							if(IsControlPressed(keys.KeyData)) return ProcessControlTab(!IsShiftPressed(keys.KeyData));
						return false;
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			return false;
		}
		internal void SetSelectable(bool value) {
			SetStyle(ControlStyles.Selectable, value);
		}
		protected internal bool IsNeededKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(FocusedView == null) return false;
			if(key == Keys.Escape || key == Keys.Enter || key == Keys.Tab) {
				if(key == Keys.Escape && FocusedView.Handler.NeedKey(NeedKeyType.Escape)) return true;
				if(key == Keys.Enter && FocusedView.Handler.NeedKey(NeedKeyType.Enter)) return true;
				if(key == Keys.Tab && FocusedView.Handler.NeedKey(NeedKeyType.Tab)) return true;
				return false;
			}
			return FocusedView.Handler.NeedKey(NeedKeyType.Dialog);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Escape || key == Keys.Enter || key == Keys.Tab) {
				if(key == Keys.Escape && (FocusedView != null && FocusedView.Handler.NeedKey(NeedKeyType.Escape))) return false;
				if(key == Keys.Enter && (FocusedView != null && FocusedView.Handler.NeedKey(NeedKeyType.Enter))) return false;
				if(IsTabPressed(keyData)) {
					if(IsControlPressed(keyData)) return ProcessControlTab(!IsShiftPressed(keyData));
					if(FocusedView != null && FocusedView.Handler.NeedKey(NeedKeyType.Tab)) return false;
				}
			}
			else {
				if((FocusedView != null && FocusedView.Handler.NeedKey(NeedKeyType.Dialog))) return false;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected internal virtual bool ProcessControlTab(bool moveForward) {
			EditorHelper.BeginAllowHideException();
			try {
				if(FocusedView != null) FocusedView.CloseEditor();
				if(moveForward) base.ProcessDialogKey(Keys.Tab);
				else base.ProcessDialogKey(Keys.Tab | Keys.Shift);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
			return true;
		}
		protected override bool IsInputKey(Keys keyData) {
			bool result = base.IsInputKey(keyData);
			if(result) return true;
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Tab && (FocusedView == null || (FocusedView != null && !FocusedView.Handler.NeedKey(NeedKeyType.Tab)))) return false;
			if(key == Keys.Escape && (FocusedView != null || (FocusedView != null && !FocusedView.Handler.NeedKey(NeedKeyType.Escape)))) return false;
			if(key == Keys.Enter && (FocusedView != null || (FocusedView != null && !FocusedView.Handler.NeedKey(NeedKeyType.Enter)))) return false;
			return true;
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyPress(e);
				if(e.Handled) return;
				if(FocusedView != null) {
					FocusedView.Handler.ProcessEvent(EventType.KeyPress, e);
					return;
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected bool IsShiftPressed(Keys keys) {
			return (keys & Keys.Shift) == Keys.Shift;
		}
		protected bool IsControlPressed(Keys keys) {
			return (keys & Keys.Control) == Keys.Control;
		}
		protected bool IsTabPressed(Keys keys) {
			return (keys & Keys.KeyCode) == Keys.Tab;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(ProcessGridKeys(e, true)) return;
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyDown(e);
				if(e.Handled) return;
				if(IsTabPressed(e.KeyData) && IsControlPressed(e.KeyData)) {
					ProcessControlTab(!IsShiftPressed(e.KeyData));
					return;
				}
				if(FocusedView != null) FocusedView.Handler.ProcessEvent(EventType.KeyDown, e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyUp(e);
				if(e.Handled) return;
				if(FocusedView != null) FocusedView.Handler.ProcessEvent(EventType.KeyUp, e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected BaseView[] GetMouseViews(MouseEventArgs e) {
			BaseView view = GetMouseDestination(e);
			if(view != null) return new BaseView[] { view };
			ArrayList list = new ArrayList();
			for(int n = Views.Count - 1; n >= 0; n--) {
				BaseView bv = Views[n] as BaseView;
				if(!IsVisibleView(bv) || MouseCaptureOwner != null) continue;
				if(bv.ViewRect.Contains(e.X, e.Y)) {
					list.Add(bv);
				}
			}
			return list.ToArray(typeof(BaseView)) as BaseView[];
		}
		BaseView GetMouseDestination(MouseEventArgs e) {
			BaseView res = GetMouseDestinationCore(e);
			if(res != null && res.ViewDisposing) res = null;
			return res;
		}
		BaseView GetMouseDestinationCore(MouseEventArgs e) {
			if(MouseCaptureOwner != null) return MouseCaptureOwner;
			int count = views.Count;
			for(int n = count - 1; n >= 0; n--) {
				BaseView gv = views[n] as BaseView;
				if(!IsVisibleView(gv)) continue;
				if(gv.Handler.RequireMouse(e)) return gv;
			}
			return null;
		}
		DXMouseEventArgs GetClickEventArgs(EventArgs ev, bool isDoubleClick) {
			MouseEventArgs mouse = ev as MouseEventArgs;
			if(mouse == null) {
				Point pos = PointToClient(Control.MousePosition);
				mouse = new MouseEventArgs(lastMouseButtons, isDoubleClick ? 2 : 1, pos.X, pos.Y, 0);
			}
			return DXMouseEventArgs.GetMouseArgs(mouse);
		}
		protected override void OnClick(EventArgs ev) {
			try {
				EditorHelper.BeginAllowHideException();
				DXMouseEventArgs e = GetClickEventArgs(ev, false);
				base.OnClick(e);
				if(e.Handled) return;
				BaseView mouseDest = GetMouseDestination(e);
				if(mouseDest != null) {
					mouseDest.Handler.ProcessEvent(EventType.Click, e);
					return;
				}
				int count = views.Count;
				for(int n = count - 1; n >= 0; n--) {
					BaseView gv = views[n] as BaseView;
					if(!IsVisibleView(gv) || MouseCaptureOwner != null) continue;
					if(gv.ViewRect.Contains(e.X, e.Y)) {
						gv.Handler.ProcessEvent(EventType.Click, ev);
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnDoubleClick(EventArgs ev) {
			try {
				EditorHelper.BeginAllowHideException();
				DXMouseEventArgs ee = GetClickEventArgs(ev, true);
				base.OnDoubleClick(ee);
				if(ee.Handled) return;
				BaseView mouseDest = GetMouseDestination(ee);
				if(mouseDest != null) {
					DoubleClickChecker.Lock();
					mouseDest.Handler.ProcessEvent(EventType.DoubleClick, ee);
					mouseDest.Handler.ProcessEvent(EventType.MouseDown, ee);
					DoubleClickChecker.Unlock();
					return;
				}
				int count = views.Count;
				for(int n = count - 1; n >= 0; n--) {
					BaseView gv = views[n] as BaseView;
					if(!IsVisibleView(gv) || MouseCaptureOwner != null) continue;
					if(gv.ViewRect.Contains(ee.X, ee.Y)) {
						DoubleClickChecker.Lock();
						gv.Handler.ProcessEvent(EventType.DoubleClick, ee);
						EventResult res = gv.Handler.ProcessEvent(EventType.MouseDown, ee);
						DoubleClickChecker.Unlock();
						if(res == EventResult.Handled) break;
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		public BaseView GetViewAt(Point point) {
			for(int n = Views.Count - 1; n >= 0; n--) {
				BaseView view = Views[n];
				if(view.IsVisible && view.ViewRect.Contains(point)) return view;
			}
			return null;
		}
		protected override void OnMouseDown(MouseEventArgs ev) {
			lastMouseButtons = ev.Button;
			try {
				EditorHelper.BeginAllowHideException();
				DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
				base.OnMouseDown(e);
				if(e.Handled || e.Clicks > 1) return;
				BaseView mouseDest = GetMouseDestination(e);
				if(mouseDest != null) {
					mouseDest.Handler.ProcessEvent(EventType.MouseDown, e);
					return;
				}
				int count = views.Count;
				for(int n = count - 1; n >= 0; n--) {
					BaseView gv = views[n] as BaseView;
					if(!IsVisibleView(gv) || MouseCaptureOwner != null) continue;
					if(gv.ViewRect.Contains(e.X, e.Y) || gv.ShouldProcessOuterMouseEvents) {
						if(gv.Handler.ProcessEvent(EventType.MouseDown, e) == EventResult.Handled) break;
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnLostCapture() {
			base.OnLostCapture();
			MouseCaptureOwner = null;
		}
		protected override void OnMouseUp(MouseEventArgs ev) {
			try {
				EditorHelper.BeginAllowHideException();
				this.lastMouseButtons = ev.Button;
				DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
				base.OnMouseUp(e);
				if(DragController.IsDragging) {
					EditorHelper.LockHideException();
					try {
						DragController.EndDrag(e);
					}
					finally {
						EditorHelper.UnlockHideException();
					}
					return;
				}
				if(e.Handled) return;
				BaseView mouseDest = GetMouseDestination(e);
				if(mouseDest != null) {
					mouseDest.Handler.ProcessEvent(EventType.MouseUp, e);
					MouseCaptureOwner = null;
					return;
				}
				int count = views.Count;
				for(int n = count - 1; n >= 0; n--) {
					BaseView gv = views[n] as BaseView;
					if(!IsVisibleView(gv) || MouseCaptureOwner != null) continue;
					if(gv.ViewRect.Contains(e.X, e.Y) || gv.ShouldProcessOuterMouseEvents) {
						if(gv.Handler.ProcessEvent(EventType.MouseUp, e) == EventResult.Handled) break;
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				MouseCaptureOwner = null;
			}
		}
		protected internal virtual BaseView ViewByPoint(Point pt) {
			for(int n = views.Count - 1; n >= 0; n--) {
				BaseView gv = views[n] as BaseView;
				if(!IsVisibleView(gv)) continue;
				if(gv.ViewRect.Contains(pt.X, pt.Y)) return gv;
			}
			return null;
		}
		protected void CheckMousePos(Point p) {
			foreach(BaseView gv in views) {
				if(p.X == BaseViewInfo.EmptyPoint.X || !gv.ViewRect.Contains(p))
					gv.Handler.UpdateMouseHere(p, false);
				else
					gv.Handler.UpdateMouseHere(p, true);
			}
		}
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			if(DefaultView != null) DefaultView.OnEnter();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			OnLoaded();
			CheckMousePos(PointToClient(Control.MousePosition));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			CheckMousePos(BaseViewInfo.EmptyPoint);
			this.lastMouseEventArgs = new MouseEventArgs(MouseButtons.None, 0, -10000, -10000, 0);
			if(!IsDesignMode)
				SetCursor(null);
		}
		MouseEventArgs lastMouseEventArgs = new MouseEventArgs(MouseButtons.None, 0, -10000, -10000, 0);
		protected override void OnMouseMove(MouseEventArgs ev) {
			if(ev.X == lastMouseEventArgs.X && ev.Y == lastMouseEventArgs.Y && ev.Button != MouseButtons.None) return;
			lastMouseEventArgs = ev;
			OnLoaded();
			try {
				EditorHelper.BeginAllowHideException();
				DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
				base.OnMouseMove(e);
				if(e.Handled) return;
				CheckMousePos(new Point(e.X, e.Y));
				if(DragController.IsDragging) {
					DragController.DoDragging(e);
					return;
				}
				BaseView mouseDest = GetMouseDestination(e);
				if(mouseDest != null) {
					mouseDest.Handler.ProcessEvent(EventType.MouseMove, e);
					return;
				}
				int count = views.Count;
				for(int n = 0; n < count; n++) {
					BaseView gv = views[n] as BaseView;
					if(!IsVisibleView(gv) || MouseCaptureOwner != null) continue;
					if(gv.ViewRect.Contains(e.X, e.Y) || gv.ShouldProcessOuterMouseEvents) {
						gv.Handler.ProcessEvent(EventType.MouseMove, e);
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected internal void StopScroller() { Scroller = null; }
		BaseViewOfficeScroller scroller = null;
		protected internal BaseViewOfficeScroller Scroller {
			get { return scroller; }
			set {
				if(scroller != null) scroller.Dispose();
				if(DesignMode) return;
				scroller = value;
			}
		}
		protected override void OnMouseWheelCore(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
				base.OnMouseWheelCore(ee);
				if(FocusedView != null) {
					FocusedView.Handler.ProcessEvent(EventType.MouseWheel, ee);
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected virtual BaseView CreateDefaultView() {
			return CreateView("GridView");
		}
		protected virtual void SetDefaultView(BaseView view) {
			if(DefaultView == view) return;
			if(defaultView != null) defaultView.OnDefaultViewStop();
			DestroyPrinting();
			defaultView = view;
			RaiseOnDefaultViewChanged();
			if(view != null)
				FocusedView = view;
			UpdateAccessibility();
		}
		private void UpdateAccessibility() {
			FieldInfo propAccessibilityField = typeof(Control).GetField("PropAccessibility", BindingFlags.NonPublic | BindingFlags.Static);
			int propAccessibility = (int)propAccessibilityField.GetValue(this);
			PropertyInfo propertiesInfo = typeof(Control).GetProperty("Properties", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo setProperty = propertiesInfo.PropertyType.GetMethod("SetObject", BindingFlags.Public | BindingFlags.Instance);
			setProperty.Invoke(propertiesInfo.GetValue(this, null), new object[] { propAccessibility, null});
		}
		protected internal BaseView MouseCaptureOwner {
			get {
				if(!views.Contains(mouseCaptureOwner) && !LevelTree.Contains(mouseCaptureOwner))
					mouseCaptureOwner = null;
				return mouseCaptureOwner;
			}
			set {
				if(MouseCaptureOwner == value) return;
				BaseView prevCapture = MouseCaptureOwner;
				mouseCaptureOwner = value;
				Capture = (value != null);
				if(prevCapture != null) prevCapture.OnLostCapture();
				if(MouseCaptureOwner != null) MouseCaptureOwner.OnGotCapture();
			}
		}
		protected override bool IsInputChar(char charCode) {
			return true;
		}
		protected internal bool IsLockUpdate {
			get {
				return lockUpdate != 0;
			}
		}
		protected override void OnSystemColorsChanged(EventArgs e) {
			base.OnSystemColorsChanged(e);
			if(InvokeRequired)
				Invoke(new MethodInvoker(OnSystemColorsChanged));
			else
				OnSystemColorsChanged();
		}
		protected void OnSystemColorsChanged() {
			DevExpress.Utils.WXPaint.Painter.ThemeChanged();
			AvailableViews.UpdateTheme();
			if(IsLoading) return;
			BeginUpdate();
			EndUpdate();
		}
		public virtual void RefreshDataSource() {
			if(IsLoading || MainView == null) return;
			MainView.DataController.RefreshData();
		}
		public virtual void BeginUpdate() {
			if(lockUpdate++ == 0) {
				foreach(BaseView bv in Views) {
					bv.BeginUpdate();
				}
			}
		}
		protected virtual void CancelUpdate() {
			if(--lockUpdate == 0) {
				Exception e = null;
				foreach(BaseView bv in Views) {
					try {
						bv.CancelUpdate();
					}
					catch(Exception exc) {
						e = exc;
					}
				}
				if(e != null) {
					throw new Exception("Error occurs", e);
				}
			}
		}
		protected virtual void EndUpdate(bool synchronized) {
			if(--lockUpdate == 0) {
				Exception e = null;
				foreach(BaseView bv in Views) {
					try {
						bv.EndUpdateCore(synchronized);
					}
					catch(Exception exc) {
						e = exc;
					}
				}
				if(e != null) {
					throw new Exception("Error occurs", e);
				}
			}
		}
		public virtual void EndUpdate() {
			EndUpdate(false);
		}
		public object InternalGetService(Type service) {
			if(service != null && MainView != null) {
				if(service.Equals(typeof(DataView))) return MainView.DataSource as DataView;
				if(service.Equals(typeof(ITypedList))) return MainView.DataSource as ITypedList;
				if(service.Equals(typeof(IXtraList))) return MainView.DataSource as IXtraList;
				if(service.Equals(typeof(IRelationList))) return MainView.DataSource as IRelationList;
			}
			return GetService(service);
		}
		DragArrowsHelper dragArrows;
		void DestroyDragArrows() {
			if(dragArrows != null) {
				dragArrows.Dispose();
				this.dragArrows = null;
			}
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			DestroyDragArrows();
		}
		internal DragArrowsHelper CreateArrowsHelper() {
			if(dragArrows != null) return dragArrows;
			dragArrows = new DragArrowsHelper(LookAndFeel, this);
			return dragArrows;
		}
		public virtual BaseView CreateView(string name) {
			BaseInfoRegistrator reg = AvailableViews[name];
			BaseView view = null;
			if(reg != null) view = reg.CreateView(this);
			else
				return null;
			if(view != null && Container != null)
				Container.Add(view);
			if(view != null) view.InitializeNew();
			return view;
		}
		protected internal virtual void RegisterView(BaseView gv) {
			views.AddCore(gv);
			RaiseOnRegisterView(gv);
			gv.CheckRightToLeft();
		}
		protected internal void RemoveView(BaseView gv) { RemoveView(gv, true); }
		protected internal void RemoveView(BaseView gv, bool disposeView) {
			if(views.Contains(gv)) {
				views.RemoveCore(gv);
				bool shouldChangeDefaultView = (gv == DefaultView && views.Count > 0), shouldChangeKeyboard = (gv == FocusedView);
				RaiseOnRemoveView(gv);
				if(disposeView) gv.Dispose();
				else {
					gv.RemoveFromGridControl();
				}
				if(this.mainView == gv) this.mainView = null;
				if(views.Count == 0) {
					if(this.defaultView != null) this.defaultView.OnDefaultViewStop();
					this.defaultView = null;
					this.focusedView = null;
					return;
				}
				if(GridDisposing) return;
				if(shouldChangeDefaultView)
					NormalView(MainView);
				if(shouldChangeKeyboard) {
					focusedView = DefaultView;
					RaiseOnFocusedViewChanged(new ViewFocusEventArgs(null, FocusedView));
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridControlViewCollection Views { get { return views; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlDataSource"),
#endif
#if DXWhidbey
 AttributeProvider(typeof(IListSource)),
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
#endif
 DefaultValue(null), DXCategory(CategoryName.Data)
		]
		public virtual object DataSource {
			get {
				WeakReference wr = dataSource as WeakReference;
				if(wr != null) {
					if(wr.IsAlive) return wr.Target;
					dataSource = null;
				}
				return dataSource; 
			}
			set {
				if(value == DBNull.Value) value = null;
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(IsValidDataSource(value)) {
					EnforceValidDataMember(value);
					RemoveDataSetEvents();
					if(dataSourceWeakReference) {
						dataSource = new WeakReference(value);
					}
					else {
					dataSource = value;
					}
					OnDataSourceChanging();
					ActivateDataSource();
					RaiseDataSourceChanged();
				}
			}
		}
		internal bool dataSourceWeakReference = false;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlDataMember"),
#endif
 DefaultValue(""), Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor)), Localizable(true), DXCategory(CategoryName.Data)]
		public virtual string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				ActivateDataSource();
				RaiseDataSourceChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlAllowRestoreSelectionAndFocusedRow"),
#endif
 DefaultValue(DefaultBoolean.Default), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DXCategory(CategoryName.Data)]
		public DefaultBoolean AllowRestoreSelectionAndFocusedRow {
			get { return allowRestoreSelectionAndFocusedRow; }
			set {
				if(AllowRestoreSelectionAndFocusedRow == value) return;
				allowRestoreSelectionAndFocusedRow = value;
				if(MainView != null) MainView.CheckDataControllerOptions(DataSource);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlServerMode"),
#endif
 DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DXCategory(CategoryName.Data),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridControl_ServerMode)]
		public bool ServerMode {
			get { return MainView != null && MainView.IsServerMode; }
			set {
			}
		}
		internal bool GetIsServerMode(object dataSource) {
			object ds = MasterDetailHelper.GetDataSource(null, dataSource, DataMember);
			if(BaseView.DetectServerModeType(ds) != BaseView.DataControllerType.Regular) return true;
			return false;
		}
		protected void EnforceValidDataMember(object dataSource) {
			if(dataSource == null) this.dataMember = "";
			if(DataMember == "" || BindingContext == null || dataSource == null || (IsDesignMode && IsLoading) || IsLoading) return;
			try {
				BindingManagerBase bm = BindingContext[dataSource, DataMember];
			}
			catch {
				this.dataMember = "";
			}
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXCategory(CategoryName.Appearance)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		bool lockActivateDataSource = false;
		protected void ActivateDataSource() {
			if(this.lockActivateDataSource || IsLoading || MainView == null || MainView.IsLoading) return;
			try {
				this.lockActivateDataSource = true;
				if(DataSource is DataView) {
					if((DataSource as DataView).Table == null) return;
				}
				LockFireChanged();
				try {
					RemoveDataSetEvents();
					object dataSource = DataSource;
					object ds = null;
					if(!GetIsServerMode(DataSource)) {
						ds = MasterDetailHelper.GetDataSource(BindingContext, DataSource, DataMember);
						if(ds is ListIEnumerable) {
							dataSource = ds;
						}
					}
					MainView.SetDataSource(BindingContext, dataSource, DataMember);
					AddDataSetEvents();
				}
				finally {
					UnlockFireChanged();
				}
			}
			finally {
				this.lockActivateDataSource = false;
			}
		}
		public bool IsVisibleView(BaseView gv) { return gv != null && gv.IsVisible && !gv.ViewDisposing; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual InfoCollection AvailableViews { get { return availableViews; } }
		protected void RegisterAvailableViews() {
			RegisterAvailableViewsCore(AvailableViews);
		}
		internal static void RegisterDefaultBorders(InfoCollection collection) {
			collection.Borders.Clear();
			collection.Borders.Add(new BorderInfo("None", new EmptyBorderPainter()));
			collection.Borders.Add(new BorderInfo("Flat", new FlatBorderPainter()));
			collection.Borders.Add(new BorderInfo("FlatSunken", new FlatSunkenBorderPainter()));
			collection.Borders.Add(new BorderInfo("Simple", new SimpleBorderPainter()));
			collection.Borders.Add(new BorderInfo("Raised", new Border3DRaisedPainter()));
			collection.Borders.Add(new BorderInfo("Sunken", new Border3DSunkenPainter()));
		}
		internal static void RegisterDefaultAvailableViews(InfoCollection collection) {
			collection.Clear();
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new CardInfoRegistrator(); }, "CardView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new GridInfoRegistrator(); }, "GridView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new BandedGridInfoRegistrator(); }, "BandedGridView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new AdvBandedGridInfoRegistrator(); }, "AdvBandedGridView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new LayoutViewInfoRegistrator(); }, "LayoutView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate {return new WinExplorerViewInfoRegistrator(); }, "WinExplorerView"));
			collection.Add(new FakeBaseInfoRegistrator(delegate { return new TileViewInfoRegistrator(); }, "TileView"));
		}
		protected virtual void RegisterAvailableViewsCore(InfoCollection collection) {
			RegisterDefaultBorders(collection);
			RegisterDefaultAvailableViews(collection);
		}
		protected virtual void RegisterBorders(InfoCollection collection) {
			RegisterDefaultBorders(collection);
		}
		protected virtual void DestroyAvailableViews() {
			AvailableViews.Clear();
		}
		string lastActiveStyle = null;
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			if(IsLoading) return;
			CheckRightToLeft();
			string activeStyle = LookAndFeel.ActiveStyle.ToString() + "->" + LookAndFeel.ActiveSkinName + "->" + LookAndFeel.GetTouchUI() + "->" + LookAndFeel.GetTouchScaleFactor();
			if(!LookAndFeelHelper.IsForcedLookAndFeelChange && (!LookAndFeel.IsColorized && activeStyle == lastActiveStyle)) return;
			this.lastActiveStyle = activeStyle;
			ResourceCache.DefaultCache.ClearPartial();
			EditorHelper.DestroyEditorsCache();
			UpdateViewStyles();
			DestroyDragArrows();
		}
		protected virtual void OnEmbeddedNavigator_SizeChanged(object sender, EventArgs e) {
			if(UseEmbeddedNavigator && DefaultView != null)
				DefaultView.OnEmbeddedNavigatorSizeChanged();
		}
		public virtual void SwitchPaintStyle(string newPaintStyleName) {
			bool changed = false;
			BeginUpdate();
			try {
				foreach(BaseView view in ViewCollection) {
					if(view.PaintStyleName != newPaintStyleName) {
						changed = true;
						view.PaintStyleName = newPaintStyleName;
					}
				}
				foreach(BaseView view in Views) {
					if(view.PaintStyleName != newPaintStyleName) {
						changed = true;
						view.PaintStyleName = newPaintStyleName;
					}
				}
			}
			finally {
				if(changed)
					EndUpdate(true);
				else
					CancelUpdate();
			}
		}
		protected virtual ISelectionService GetSelectionService() {
			if(!IsDesignMode) return null;
			return GetService(typeof(ISelectionService)) as ISelectionService;
		}
		protected internal virtual bool GetComponentSelected(object obj) {
			ISelectionService srv = GetSelectionService();
			if(srv == null) return false;
			return srv.GetComponentSelected(obj);
		}
		protected internal virtual ICollection GetSelectedObjects() {
			ISelectionService srv = GetSelectionService();
			if(srv == null) return null;
			return srv.GetSelectedComponents();
		}
		protected internal virtual void SetComponentsSelected(object[] obj) {
			ISelectionService srv = GetSelectionService();
			srv.SetSelectedComponents(obj, ControlConstants.SelectionClick);
		}
		#region Export
		public void ExportToXlsx(string filePath) {
			DefaultView.ExportToXlsx(filePath);
		}
		public void ExportToXlsx(Stream stream) {
			DefaultView.ExportToXlsx(stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			DefaultView.ExportToXlsx(stream, options);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			DefaultView.ExportToXlsx(filePath, options);
		}
		public void ExportToXls(string filePath) {
			DefaultView.ExportToXls(filePath);
		}
		public void ExportToXls(Stream stream) {
			DefaultView.ExportToXls(stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			DefaultView.ExportToXls(stream, options);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			DefaultView.ExportToXls(filePath, options);
		}
		[Obsolete("Use the ExportToXls(String filePath, XlsExportOptions options) method instead")]
		public void ExportToXls(string filePath, bool useNativeFormat) {
			DefaultView.ExportToXls(filePath, useNativeFormat);
		}
		[Obsolete("Use the ExportToXls(Stream stream, XlsExportOptions options) method instead")]
		public void ExportToXls(Stream stream, bool useNativeFormat) {
			DefaultView.ExportToXls(stream, useNativeFormat);
		}
		public void ExportToRtf(string filePath) {
			DefaultView.ExportToRtf(filePath);
		}
		public void ExportToRtf(Stream stream) {
			DefaultView.ExportToRtf(stream);
		}
		public void ExportToHtml(string filePath) {
			DefaultView.ExportToHtml(filePath);
		}
		public void ExportToHtml(Stream stream) {
			DefaultView.ExportToHtml(stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			DefaultView.ExportToHtml(stream, options);
		}
		public void ExportToHtml(String filePath, HtmlExportOptions options) {
			DefaultView.ExportToHtml(filePath, options);
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet) {
			DefaultView.ExportToHtml(filePath, htmlCharSet);
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			DefaultView.ExportToHtml(filePath, htmlCharSet, title, compressed);
		}
		[Obsolete("Use the ExportToHtml(Stream stream, HtmlExportOptions options) method instead")]
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			DefaultView.ExportToHtml(stream, htmlCharSet, title, compressed);
		}
		public void ExportToMht(string filePath) {
			DefaultView.ExportToMht(filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			DefaultView.ExportToMht(filePath, options);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			DefaultView.ExportToMht(stream, options);
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet) {
			DefaultView.ExportToMht(filePath, htmlCharSet);
		}
		[Obsolete("Use the ExportToMht(string filePath, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			DefaultView.ExportToMht(filePath, htmlCharSet, title, compressed);
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			DefaultView.ExportToMht(stream, htmlCharSet, title, compressed);
		}
		public void ExportToPdf(string filePath) {
			DefaultView.ExportToPdf(filePath);
		}
		public void ExportToPdf(Stream stream) {
			DefaultView.ExportToPdf(stream);
		}
		public void ExportToText(Stream stream) {
			DefaultView.ExportToText(stream);
		}
		public void ExportToText(string filePath) {
			DefaultView.ExportToText(filePath);
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			DefaultView.ExportToText(filePath, options);
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			DefaultView.ExportToText(stream, options);
		}
		public void ExportToCsv(Stream stream) {
			DefaultView.ExportToCsv(stream);
		}
		public void ExportToCsv(string filePath) {
			DefaultView.ExportToCsv(filePath);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			DefaultView.ExportToCsv(filePath, options);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			DefaultView.ExportToCsv(stream, options);
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator) {
			DefaultView.ExportToText(filePath, separator);
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators) {
			DefaultView.ExportToText(filePath, separator, quoteStringsWithSeparators);
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			DefaultView.ExportToText(filePath, separator, quoteStringsWithSeparators, encoding);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator) {
			DefaultView.ExportToText(stream, separator);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators) {
			DefaultView.ExportToText(stream, separator, quoteStringsWithSeparators);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			DefaultView.ExportToText(stream, separator, quoteStringsWithSeparators, encoding);
		}
		#endregion
		[Obsolete("Use the ExportToHtml method instead")]
		public void ExportToHtmlOld(string fileName) {
			DefaultView.ExportToHtmlOld(fileName);
		}
		[Obsolete("Use the ExportToXls method instead")]
		public void ExportToExcelOld(string fileName) {
			DefaultView.ExportToExcelOld(fileName);
		}
		[Obsolete("Use the ExportToText method instead")]
		public void ExportToTextOld(string fileName) {
			DefaultView.ExportToTextOld(fileName);
		}
		IDisposable printer;
		protected internal bool IsPrinterExist { get { return printer != null; } }
		protected internal ComponentPrinterBase Printer {
			get {
				if(printer == null) printer = CreateComponentPrinter();
				if(printer == null) return null;
				UpdateComponentPrinter(true);
				return (ComponentPrinterBase)printer;
			}
		}
		internal void InternalClearDocument() {
			if(printer == null) return;
			((ComponentPrinterBase)printer).ClearDocument();
		}
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new XtraGridComponentPrinter(PrintWrapper);
		}
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		[Obsolete("Use ShowPrintPreview instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ShowPreview() { ShowPrintPreview(); }
		public void ShowPrintPreview() {
			if(DefaultView != null) DefaultView.ShowPrintPreview();
		}
		public void ShowRibbonPrintPreview() {
			if(DefaultView != null) DefaultView.ShowRibbonPrintPreview();
		}
		public void Print() {
			if(DefaultView != null) DefaultView.Print();
		}
		public void PrintDialog() {
			if(DefaultView != null) DefaultView.PrintDialog();
		}
		#region events
		protected void RaiseLoad() {
			EventHandler handler = (EventHandler)this.Events[load];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseDataSourceChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
			OnDataSourceChanged(); 
		}
		protected void RaiseOnDefaultViewChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[defaultViewChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseOnFocusedViewChanged(ViewFocusEventArgs e) {
			if(IsLoading) return;
			UpdateNavigator();
			ViewFocusEventHandler handler = (ViewFocusEventHandler)this.Events[focusedViewChanged];
			if(handler != null) handler(this, e);
		}
		protected void RaiseOnRegisterView(BaseView gv) {
			if(IsLoading) return;
			ViewOperationEventHandler handler = (ViewOperationEventHandler)this.Events[viewRegistered];
			if(handler != null) handler(this, new ViewOperationEventArgs(gv));
		}
		protected virtual void RaiseOnRemoveView(BaseView gv) {
			if(IsLoading) return;
			ViewOperationEventHandler handler = (ViewOperationEventHandler)this.Events[viewRemoved];
			if(handler != null) handler(this, new ViewOperationEventArgs(gv));
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ControlCollection Controls { get { return base.Controls; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlDataSourceChanged"),
#endif
 DXCategory(CategoryName.Grid)]
		public event EventHandler DataSourceChanged {
			add { Events.AddHandler(dataSourceChanged, value); }
			remove { Events.RemoveHandler(dataSourceChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlDefaultViewChanged"),
#endif
 DXCategory(CategoryName.Grid)]
		public event EventHandler DefaultViewChanged {
			add { Events.AddHandler(defaultViewChanged, value); }
			remove { Events.RemoveHandler(defaultViewChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlLoad"),
#endif
 DXCategory(CategoryName.Grid)]
		public event EventHandler Load {
			add { Events.AddHandler(load, value); }
			remove { Events.RemoveHandler(load, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlViewRegistered"),
#endif
 DXCategory(CategoryName.Grid)]
		public event ViewOperationEventHandler ViewRegistered {
			add { Events.AddHandler(viewRegistered, value); }
			remove { Events.RemoveHandler(viewRegistered, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlViewRemoved"),
#endif
 DXCategory(CategoryName.Grid)]
		public event ViewOperationEventHandler ViewRemoved {
			add { Events.AddHandler(viewRemoved, value); }
			remove { Events.RemoveHandler(viewRemoved, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlProcessGridKey"),
#endif
 DXCategory(CategoryName.Grid)]
		public event KeyEventHandler ProcessGridKey {
			add { Events.AddHandler(processGridKey, value); }
			remove { Events.RemoveHandler(processGridKey, value); }
		}
		[Browsable(false), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlKeyboardFocusViewChanged"),
#endif
 DXCategory(CategoryName.Grid), Obsolete(ObsoleteText.SRGridControl_FocusedViewChanged)]
		public event ViewFocusEventHandler KeyboardFocusViewChanged {
			add { Events.AddHandler(focusedViewChanged, value); }
			remove { Events.RemoveHandler(focusedViewChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridControlFocusedViewChanged"),
#endif
 DXCategory(CategoryName.Grid)]
		public event ViewFocusEventHandler FocusedViewChanged {
			add { Events.AddHandler(focusedViewChanged, value); }
			remove { Events.RemoveHandler(focusedViewChanged, value); }
		}
		#endregion
		#region IToolTipControlClient
		GridToolTipInfo lastToolTipInfo = null;
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			BaseView[] views = GetMouseViews(new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0));
			if(views == null) return null;
			foreach(BaseView bv in views) {
				if(!bv.CanShowHint) {
					if(this.lastToolTipInfo != null && this.lastToolTipInfo.View == bv) this.lastToolTipInfo = null;
					continue;
				}
				ToolTipControlInfo info = bv.GetToolTipObjectInfo(point);
				if(info == null) {
					if(this.lastToolTipInfo != null && this.lastToolTipInfo.View == bv)
						this.lastToolTipInfo = null;
					continue;
				}
				if(this.lastToolTipInfo == null || this.lastToolTipInfo.View != bv || !Object.Equals(this.lastToolTipInfo.Object, info.Object)) {
					this.lastToolTipInfo = new GridToolTipInfo(bv, info.Object);
				}
				info.Object = this.lastToolTipInfo;
				return info;
			}
			this.lastToolTipInfo = null;
			return null;
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		#endregion
		ViewPrintWrapper printWrapper;
		protected internal ViewPrintWrapper PrintWrapper {
			get {
				if(printWrapper == null) {
					printWrapper = CreateViewPrintWrapper();
					printWrapper.AfterCreated();
					UpdateComponentPrinter(false);
				}
				return printWrapper;
			}
			set {
				if(printWrapper == value) return;
				if(printWrapper != null) printWrapper.Dispose();
				printWrapper = value;
			}
		}
		void UpdateComponentPrinter(bool checkPrintWrapper) {
			if(printer == null) return;
			if(checkPrintWrapper && PrintWrapper == null) return;
			if(printWrapper == null) return;
			((ComponentPrinterBase)printer).Component = PrintWrapper;
		}
		protected internal bool IsPrintWrapperValid { get { return printWrapper == null || printWrapper.IsValid; } }
		protected internal void CheckPrintWrapper() {
			if(!IsPrintWrapperValid) PrintWrapper = null;
		}
		protected virtual ViewPrintWrapper CreateViewPrintWrapper() {
			return new ViewPrintWrapper(DefaultView, true);
		}
		#region IPrintable
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(MainView == null && printWrapper == null) return;
			((IBasePrintable)PrintWrapper).Finalize(ps, link);
		}
		bool IPrintable.CreatesIntersectedBricks { get { return ((IPrintable)PrintWrapper).CreatesIntersectedBricks; } }
		bool IPrintable.HasPropertyEditor() { return ((IPrintable)PrintWrapper).HasPropertyEditor(); }
		UserControl IPrintable.PropertyEditorControl { get { return ((IPrintable)PrintWrapper).PropertyEditorControl; } }
		bool IPrintable.SupportsHelp() { return ((IPrintable)PrintWrapper).SupportsHelp(); }
		void IPrintable.ShowHelp() { ((IPrintable)PrintWrapper).ShowHelp(); }
		void IPrintable.AcceptChanges() { ((IPrintable)PrintWrapper).AcceptChanges(); }
		void IPrintable.RejectChanges() { ((IPrintable)PrintWrapper).RejectChanges(); }
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			if(printWrapper == null) ForceInitialize();
			((IPrintable)PrintWrapper).Initialize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			((IBasePrintable)PrintWrapper).CreateArea(areaName, graph);
		}
		void IPrintableEx.OnEndActivity() { ((IPrintableEx)PrintWrapper).OnEndActivity(); }
		void IPrintableEx.OnStartActivity() { ((IPrintableEx)PrintWrapper).OnStartActivity(); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("")]
		public bool IsPrinting { get { return printWrapper != null && PrintWrapper.IsPrinting; } }
		#endregion
		#region IPrintHeaderFooter
		string IPrintHeaderFooter.InnerPageHeader { get { return ((IPrintHeaderFooter)PrintWrapper).InnerPageHeader; } }
		string IPrintHeaderFooter.InnerPageFooter { get { return ((IPrintHeaderFooter)PrintWrapper).InnerPageFooter; } }
		string IPrintHeaderFooter.ReportHeader { get { return ((IPrintHeaderFooter)PrintWrapper).ReportHeader; } }
		string IPrintHeaderFooter.ReportFooter { get { return ((IPrintHeaderFooter)PrintWrapper).ReportFooter; } }
		#endregion
		#region INavigatableControl
		protected virtual ColumnView NavView { get { return FocusedView as ColumnView; } }
		protected internal virtual NavigatableControlHelper NavigatableHelper { get { return navigatableHelper; } }
		void INavigatableControl.AddNavigator(INavigatorOwner owner) { NavigatableHelper.AddNavigator(owner); }
		void INavigatableControl.RemoveNavigator(INavigatorOwner owner) { NavigatableHelper.RemoveNavigator(owner); }
		int INavigatableControl.RecordCount {
			get {
				if(NavView == null) return 0;
				return NavView.NavigatorRowCount;
			}
		}
		int INavigatableControl.Position {
			get {
				if(NavView == null) return 0;
				return NavView.NavigatorPosition;
			}
		}
		bool INavigatableControl.IsActionEnabled(NavigatorButtonType type) {
			if(IsDesignMode) return false;
			if(NavView == null) return false;
			return NavView.IsNavigatorActionEnabled(type);
		}
		void INavigatableControl.DoAction(NavigatorButtonType type) {
			if(IsDesignMode) return;
			if(NavView == null) return;
			try {
				EditorHelper.BeginAllowHideException();
				NavView.DoNavigatorAction(type);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				UpdateNavigator();
			}
		}
		protected internal virtual void UpdateNavigator() {
			NavigatableHelper.UpdateButtons();
		}
		#endregion
		RepositoryItemGridLookUpEditBase lookUpOwner;
		protected internal RepositoryItemGridLookUpEditBase LookUpOwner { get { return lookUpOwner; } }
		protected internal virtual void SetupLookUp(RepositoryItemGridLookUpEditBase lookUp, BaseView view) {
			this.lookUpOwner = lookUp;
			UpdateRepositoryOwner();
			MainView = view;
			DevExpress.XtraGrid.Views.Grid.IGridLookUp gl = view as DevExpress.XtraGrid.Views.Grid.IGridLookUp;
			if(gl != null) gl.Setup();
		}
		protected internal void UpdateRepositoryOwner() {
			if(LookUpOwner != null) {
				((PersistentRepository)EditorHelper.InternalRepository).SetParentComponent(LookUpOwner.OwnerEdit != null ? (Component)LookUpOwner.OwnerEdit : (Component)LookUpOwner);
			}
		}
		#region IFilterControl Members
		ColumnView FilterView {
			get {
				return this.MainView as ColumnView;
			}
		}
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return new DevExpress.XtraGrid.FilterEditor.ViewFilterColumnCollection(FilterView);
		}
		DevExpress.Data.Filtering.CriteriaOperator IFilteredComponentBase.RowCriteria {
			get {
				if(FilterView == null) return string.Empty;
				return FilterView.ActiveFilterCriteria;
			}
			set {
				if(FilterView == null) return;
				FilterView.ActiveFilterCriteria = value;
			}
		}
		EventHandler filterChanged;
		internal void OnFilterChanged() {
			if(filterChanged != null) filterChanged(this, EventArgs.Empty);
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { filterChanged += value; }
			remove { filterChanged -= value; }
		}
		EventHandler filterDataSourceChanged;
		internal void OnDataSourceChanged() {
			if(IsLoading) return;
			if(filterDataSourceChanged != null) filterDataSourceChanged(this, EventArgs.Empty);
		}
		protected internal virtual void OnDataSourceChanging() {
			if(MainView == null) return;
			MainView.OnDataSourceChanging();
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { filterDataSourceChanged += value; }
			remove { filterDataSourceChanged -= value; }
		}
		#endregion
		internal void ResetLookUp(object newDataSource) {
			bool sameDataSource = object.ReferenceEquals(DataSource, newDataSource);
			if(!sameDataSource) {
				if(DataSource == null) return;
				DataSource = null;
			}
			if(MainView != null) {
				MainView.ResetLookUp(sameDataSource);
			}
		}
		#region ISupportXtraSerializer Members
		void ISupportXtraSerializer.RestoreLayoutFromRegistry(string path) {
			if(MainView != null) MainView.RestoreLayoutFromRegistry(path);
		}
		void ISupportXtraSerializer.RestoreLayoutFromStream(Stream stream) {
			if(MainView != null) MainView.RestoreLayoutFromStream(stream);
		}
		void ISupportXtraSerializer.RestoreLayoutFromXml(string xmlFile) {
			if(MainView != null) MainView.RestoreLayoutFromXml(xmlFile);
		}
		void ISupportXtraSerializer.SaveLayoutToRegistry(string path) {
			if(MainView != null) MainView.SaveLayoutToRegistry(path);
		}
		void ISupportXtraSerializer.SaveLayoutToStream(Stream stream) {
			if(MainView != null) MainView.SaveLayoutToStream(stream);
		}
		void ISupportXtraSerializer.SaveLayoutToXml(string xmlFile) {
			if(MainView != null) MainView.SaveLayoutToXml(xmlFile);
		}
		#endregion
		#region ContextMenuStrip
		public override ContextMenuStrip ContextMenuStrip {
			get {
				return base.ContextMenuStrip;
			}
			set {
				if(Object.Equals(base.ContextMenuStrip, value)) return;
				if(base.ContextMenuStrip != null)
					base.ContextMenuStrip.Opening -= new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);
				base.ContextMenuStrip = value;
				if(base.ContextMenuStrip != null)
					base.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);
			}
		}
		void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
			if(FocusedView == null || FocusedView.ActiveEditor == null) return;
			PictureEdit edit = FocusedView.ActiveEditor as PictureEdit;
			if(edit == null) return;
			if(edit.Properties.ShowMenu)
				e.Cancel = true;
		}
		#endregion
		#region ITouchClient Members
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		IntPtr IGestureClient.Handle { get { return Handle; } }
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); }  }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(touchClient == null) UpdateTouchClient(info);
			TouchClient.OnTouchScroll(info, delta, ref overPan);
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) { 
			touchClient = GetViewAt(point);
			return TouchClient.CheckAllowGestures(point);
		}
		void UpdateTouchClient(GestureArgs info) {
			touchClient = GetViewAt(info.Start.Point);
		}
		BaseView touchClient = null;
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			UpdateTouchClient(info);
		}
		protected BaseView TouchClient { get { return touchClient == null ? DefaultView : touchClient; } }
		#endregion
		internal static object GetClonedDataSource(object dataSource) {
			IDXCloneable dataCloneable = dataSource as IDXCloneable;
			if(dataCloneable == null) {
				IListSource source = dataSource as IListSource;
				if(source != null) dataCloneable = source.GetList() as IDXCloneable;
			}
			if(dataCloneable != null) {
				return dataCloneable.DXClone();
			}
			return null;
		}
		#region IGuideDescription Members
		string IGuideDescription.SubType {
			get {
				if(DefaultView == null) return null;
				return DefaultView.GetType().Name;
			}
		}
		#endregion
		#region ISearchClient Members
		ISearchControl searchControl;
		[Browsable(false)]	
		public bool IsAttachedToSearchControl { get { return searchControl != null; } }
		SearchControl.IColumnViewSearchClient SearchColumnViewClient {
			get { return MainView as SearchControl.IColumnViewSearchClient; }
		}
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase args) {
			if(SearchColumnViewClient != null)
				SearchColumnViewClient.ApplyFindFilter(args);
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			this.searchControl = searchControl;
			if(SearchColumnViewClient == null) return;
			SearchColumnViewClient.ApplyFindFilter(null);
			SearchColumnViewClient.FindPanelVisibilityChange();
		}
		internal void SetFocusSearchControl() {
			if(IsAttachedToSearchControl)
				searchControl.Focus();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new SearchControl.GridColumnsProvider(this);
		}
		IEnumerable IFilteredComponentColumns.Columns { get { return ((IFilteredComponent)this).CreateFilterColumnCollection(); } }
		#endregion
		#region ILogicalOwner Members
		System.Collections.Generic.IEnumerable<Component> ILogicalOwner.GetLogicalChildren() {
			if(Views == null) yield break;
			foreach(BaseView view in Views) {
				yield return view;
			}
		}
		#endregion
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			EditorHelper.DestroyEditorsCache();
			BeginUpdate();
			try {
				if(DefaultView != null) DefaultView.CheckRightToLeft();
				foreach(BaseView view in Views) view.CheckRightToLeft();
			}
			finally {
				EndUpdate();
			}
		}
		protected new internal bool IsRightToLeft { get { return base.IsRightToLeft; } }
	}
	public class GridToolTipInfo : ISupportLookAndFeel {
		BaseView view;
		object _object;
		public GridToolTipInfo(BaseView view, object _object) {
			this.view = view;
			this._object = _object;
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return View.ElementsLookAndFeel; } }
		public GridControl Grid { get { return View.GridControl; } }
		public BaseView View { get { return view; } }
		public object Object { get { return _object; } }
	}
	public class ViewFocusEventArgs : EventArgs {
		BaseView previousView, view;
		public ViewFocusEventArgs(BaseView previousView, BaseView view) {
			this.previousView = previousView;
			this.view = view;
		}
		public BaseView View { get { return view; } }
		public BaseView PreviousView { get { return previousView; } }
	}
	public delegate void ViewFocusEventHandler(object sender, ViewFocusEventArgs e);
	public class ViewOperationEventArgs : EventArgs {
		private BaseView view;
		internal ViewOperationEventArgs(BaseView view) {
			this.view = view;
		}
		public BaseView View { get { return view; } }
	}
	public delegate void ViewOperationEventHandler(object sender, ViewOperationEventArgs e);
	internal class ObsoleteText {
		internal const string SRObsoleteMenuItem = "You should use the DXMenuItem property instead of MenuItem";
		internal const string SRObsoleteCreateColumnEdit = "You should add editors via RepositoryItems";
		internal const string SRObsoleteDefaultEdit = "Check documentation";
		internal const string SRObsoleteColumnFormatType = "You should use the 'DisplayFormat.FormatType' instead of FormatType";
		internal const string SRObsoleteEditorsRepository = "You should use the 'RepositoryItems' or 'ExternalRepository' property";
		internal const string SRObsoleteColumnAdd = "Use the AddField method instead.";
		internal const string SRUnboundEventRowHandle = "To identify a row, use the e.Row or e.ListSourceRowIndex parameter. See the documentation, to learn more.";
		internal const string
			SRGridOptionsDetail_SmartDetailExpandButton = "Use the OptionsDetail.SmartDetailExpandButtonMode property instead.",
			SRGridView_OptionsBehaviorAllowOnlyOneMasterRowExpanded = "Use the OptionsDetail.AllowOnlyOneMasterRowExpanded property instead.",
			SRColumnView_ShowButtonMode = "Use the OptionsView.ShowButtonMode property instead.",
			SRColumnView_OptionsViewShowFilterPanel = "Use the OptionsView.ShowFilterPanelMode property instead.",
			SRColumnView_OptionsBehaviorUseNewCustomFilterDialog = "Use the OptionsFilter.UseNewCustomFilterDialog property instead.",
			SRColumnView_OptionsBehaviorShowAllTableValuesInFilterPopup = "Use the OptionsFilter.ShowAllTableValuesInFilterPopup property instead.",
			SRColumnView_OptionsBehaviorShowEditorOnMouseUp = "Use the OptionsBehavior.EditorShowMode property instead.",
			SRColumnView_FilterPopupMaxRecordsCount = "Use the ColumnFilterPopupMaxRecordsCount property of the view'fieldCaption OptionsFilter object instead.",
			SRColumnView_FilterPopupRowCount = "Use the ColumnFilterPopupRowCount property of the view'fieldCaption OptionsFilter object instead.",
			SRBaseView_GetStyleByViewName = "Use a corresponding property of the view'fieldCaption Appearance object.",
			SRCardOptionsPrint_PrintSelectedCardOnly = "Use the PrintSelectedCardsOnly property instead.",
			SRCardView_ViewStylesInfo = "Use the view'fieldCaption Appearance property instead.",
			SRGridView_ViewStylesInfo = "Use the view'fieldCaption Appearance property instead.",
			SRGridView_GroupFooterShowMode = "Use the OptionsView.GroupFooterShowMode property instead",
			SRGridView_CustomizationRowCount = "The Customization form can be resized at runtime. Use the CustomizationFormBounds property to specify the size of the form, in pixels.",
			SRColumn_Options = "Use the OptionsColumn property instead.",
			SRColumn_StyleName = "Use the column'fieldCaption AppearanceCell property instead.",
			SRColumn_Style = "Use the column'fieldCaption AppearanceCell property instead.",
			SRColumn_HeaderStyleName = "Use the column'fieldCaption AppearanceHeader property instead.",
			SRColumn_HeaderStyle = "Use the column'fieldCaption AppearanceHeader property instead.",
			SRColumn_IsOption = "Use the column'fieldCaption OptionsColumn property instead.",
			SRGridOptionsView_ShowNewItemRow = "Use the NewItemRowPosition property of the view'fieldCaption OptionsView object instead.",
			SRGridControl_KeyboardFocusView = "Use the FocusedView property instead.",
			SRGridControl_LevelDefaults = "Use the LevelTree property instead.",
			SRGridControl_FocusedViewChanged = "Use the FocusedViewChanged event instead.",
			SRGridControl_ServerMode = "You don't need ServerMode property anymore.",
			SRRowCellStyleEventArgs_CellStyle = "Use the Appearance property instead.",
			SRCustomDrawEventArgs_Style = "Use the Appearance property instead.",
			SRStyleFormatCondition_StyleName = "Use the Appearance property instead.",
			SRBand_Options = "Use the OptionsBand property instead.",
			SRBand_IsOption = "Use the OptionsBand property instead.",
			SRBand_HeaderStyle = "Use the AppearanceHeader property instead.",
			SRBand_HeaderStyleName = "Use the AppearanceHeader property instead.",
			SRGridView_OptionsViewShowHorzLines = "Use the ShowHorizontalLines property instead.",
			SRGridView_OptionsViewShowVertLines = "Use the ShowVerticalLines property instead.",
			SRGridView_OptionsViewShowPreviewLines = "Use the ShowPreviewRowLines property instead.",
			SRGridView_CopyToClipboardWithColumnHeaders = "Use the OptionsClipboard.CopyColumnHeaders property instead.";
	}
	public class GridControlViewCollection : ReadOnlyCollectionBase, System.Collections.Generic.IEnumerable<BaseView> {
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridControlViewCollectionItem")]
#endif
		public BaseView this[int index] { get { return InnerList[index] as BaseView; } }
		internal void AddCore(BaseView view) {
			InnerList.Add(view);
		}
		internal void RemoveAtCore(int index) { InnerList.RemoveAt(index); }
		internal void RemoveCore(BaseView view) { InnerList.Remove(view); }
		public virtual bool Contains(BaseView view) { return InnerList.Contains(view); }
		internal int VisibleCount {
			get {
				int c = 0;
				foreach(BaseView view in this) {
					if(view.IsVisible && !view.ViewRect.IsEmpty) c++;
				}
				return c;
			}
		}
		System.Collections.Generic.IEnumerator<BaseView> System.Collections.Generic.IEnumerable<BaseView>.GetEnumerator() {
			foreach(BaseView view in InnerList)
				yield return view;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class XtraGridComponentPrinter : ComponentPrinter {
		public XtraGridComponentPrinter(IPrintable component) : base(component) { }
		public XtraGridComponentPrinter(IPrintable component, PrintingSystemBase printingSystem) : base(component, printingSystem) { }
		protected DevExpress.XtraGrid.Views.Grid.GridView GetView() {
			var temp = Component as ViewPrintWrapper;
			if(temp != null) return temp.View as DevExpress.XtraGrid.Views.Grid.GridView;
			return null;
		}
		public override void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			ExportCore(target, stream, options, (exporter, fp) => { exporter.Export((Stream)fp); }, (targ, fp, opt) => { base.Export(targ, (Stream)fp, opt); });
		}
		public override void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			ExportCore(target, filePath, options, (exporter, fp) => { exporter.Export((string)fp); }, (targ, fp, opt) => { base.Export(targ, (string)fp, opt); });
		}
		public override void Export(ExportTarget target, string filePath) {
			ExportCore(target, filePath, (exporter, fp) => { exporter.Export((string)fp); }, (targ, fp) => { base.Export(targ, (string)fp); });
		}
		public override void Export(ExportTarget target, Stream stream) {
			ExportCore(target, stream, (exporter, fp) => { exporter.Export((Stream)fp); }, (targ, fp) => { base.Export(targ, (Stream)fp); });
		}
		protected virtual IDataAwareExportOptions InitExporterOptions(ExportTarget target, ExportOptionsBase options) {
			XlsExportOptionsBase xlsEO = options as XlsExportOptionsBase;
			IDataAwareExportOptions result = DataAwareExportOptionsFactory.Create(target, options as IDataAwareExportOptions);
			DevExpress.XtraGrid.Views.Grid.GridView view = GetView();
			result.AllowCellMerge = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowCellMerge, view.OptionsView.AllowCellMerge);
			result.ShowTotalSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowTotalSummaries, view.OptionsPrint.PrintFooter);
			result.ShowGroupSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowGroupSummaries, view.OptionsPrint.PrintGroupFooter);
			result.ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.ShowColumnHeaders, view.OptionsPrint.PrintHeader);
			result.AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowHorzLines, view.OptionsPrint.PrintHorzLines);
			result.AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowVertLines, view.OptionsPrint.PrintVertLines);
			result.RightToLeftDocument = view.GridControl.IsRightToLeft ? DefaultBoolean.True : DefaultBoolean.False;
			result.AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowHyperLinks, xlsEO != null ? xlsEO.ExportHyperlinks : true);
			result.SheetName = xlsEO != null ? xlsEO.SheetName : view.ViewCaption;
			if(target == ExportTarget.Csv) {
				CsvExportOptions csvEO = options as CsvExportOptions;
				if(csvEO != null) {
					result.CSVEncoding = csvEO.Encoding;
					result.CSVSeparator = csvEO.Separator;
				}
			}
			result.ExportTarget = target;
			result.InitDefaults();
			return result;
		}
		private void ExportCore(ExportTarget target, object filePath, Action2<GridViewExcelExporter<ColumnImplementer, DataRowImplementer>, object> action, Action2<ExportTarget, object> baseAction) {
			if(!DoExportCore(target, filePath, null, action)) baseAction(target, filePath);
		}
		private void ExportCore(ExportTarget target, object filePath, ExportOptionsBase options, Action2<GridViewExcelExporter<ColumnImplementer, DataRowImplementer>, object> action, Action3<ExportTarget, object, ExportOptionsBase> baseAction) {
			if(!DoExportCore(target, filePath, options, action)) baseAction(target, filePath, options);
		}
		protected virtual bool DoExportCore(ExportTarget target, object filePath, ExportOptionsBase options, Action2<GridViewExcelExporter<ColumnImplementer, DataRowImplementer>, object> action){
			if(ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions, target) && GetView() != null){
				IDataAwareExportOptions exporterOptions = null;
				try{
					GetView().OnPrintStart(Component as ViewPrintWrapper, PrintingSystemActivity.Exporting);
					exporterOptions = InitExporterOptions(target, options);
					exporterOptions.ExportProgress += OnExportProgress;
					var exporter = new GridViewExcelExporter<ColumnImplementer, DataRowImplementer>( new GridViewImplementer<ColumnImplementer, DataRowImplementer>(GetView(), target), exporterOptions);
					action(exporter, filePath);
				}
				finally{
					GetView().OnPrintEnd(false);
					exporterOptions.ExportProgress -= OnExportProgress;
				}
				return true;
			}
			return false;
		}
		void OnExportProgress(ProgressChangedEventArgs ea) {
			var view = GetView();
			if(view == null) return;
			view.OnPrintExportProgress(ea.ProgressPercentage, ea.ProgressPercentage < 0 ? true : false);
		}
	}
	[ToolboxItem(false)
#if DXWhidbey
, System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraEditors.Design.RepositoryItemCodeDomSerializer, " + AssemblyInfo.SRAssemblyEditorsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
#endif
]
	public class GridControlNavigator : ControlNavigator {
		GridControl grid;
		public GridControlNavigator(GridControl control) {
			this.grid = control;
			SetStyle(ControlStyles.Selectable, false);
			this.TabStop = false;
			this.Visible = false;
			this.ShowToolTips = true;
			this.TextLocation = NavigatorButtonsTextLocation.Center;
			base.NavigatableControl = control;
			this.RightToLeft = RightToLeft.No;
		}
		internal void UpdateLayout() {
			if(ViewInfo.IsReady) return;
			ViewInfo.CalcViewInfo(null, System.Windows.Forms.MouseButtons.None, Point.Empty, ClientRectangle);
		}
		protected GridControl Grid { get { return grid; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft {
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name { get { return base.Name; } set { base.Name = value; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				base.NavigatableControl = null;
			}
			base.Dispose(disposing);
		}
		protected override EditorButtonPainter ButtonPainterCore {
			get {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003 && DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled)
					return EditorButtonHelper.GetWindowsXPPainter();
				return base.ButtonPainterCore;
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				if(!Grid.ContainsFocus) Grid.Focus();
			}
			base.OnMouseDown(e);
		}
		bool lockSizeChanged = false;
		protected override void LayoutChanged() {
			base.LayoutChanged();
			if(this.lockSizeChanged || (Grid != null && !Grid.UseEmbeddedNavigator)) return;
			this.lockSizeChanged = true;
			try {
				if(Size.Width != MinSize.Width) {
					OnSizeChanged(EventArgs.Empty);
				}
			}
			finally {
				lockSizeChanged = false;
			}
		}
		protected override bool GetValidationCanceledCore() {
			if(!base.GetValidationCanceledCore()) return false;
			Control c = GetValidationCanceledSource(this);
			if(c == this || c == Grid) return false;
			return true;
		}
		[DefaultValue(NavigatorButtonsTextLocation.Center)]
		public override NavigatorButtonsTextLocation TextLocation {
			get { return base.TextLocation; }
			set { base.TextLocation = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles BorderStyle {
			get { return BorderStyles.NoBorder; }
			set {
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override UserLookAndFeel LookAndFeel { get { return base.LookAndFeel; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override INavigatableControl NavigatableControl {
			get { return base.NavigatableControl; }
			set { base.NavigatableControl = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[DefaultValue(true)]
		public override bool ShowToolTips {
			get { return base.ShowToolTips; }
			set { base.ShowToolTips = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { base.TabIndex = value; }
		}
	}
	public class GridEditorContainerHelper : EditorContainerHelper {
		public GridEditorContainerHelper(GridControl owner)
			: base(owner) {
		}
		protected new GridControl Owner { get { return base.Owner as GridControl; } }
		protected override void OnRepositoryItemRemoved(RepositoryItem item) {
			if(IsLoading) return;
			foreach(BaseView view in Owner.Views) {
				view.OnRepositoryItemRemoved(item);
			}
			BaseView[] views = Owner.LevelTree.GetTemplates();
			foreach(BaseView view in views) {
				view.OnRepositoryItemRemoved(item);
			}
		}
		protected override void OnRepositoryRefreshRequired(RepositoryItem item) {
			if(Owner.DefaultView != null) Owner.DefaultView.OnRepositoryItemRefreshRequired(item);
			base.OnRepositoryRefreshRequired(item);
		}
		protected override void OnRepositoryItemChanged(RepositoryItem item) {
			if(Owner.DefaultView != null)
				Owner.DefaultView.OnPropertiesChanged();
			base.OnRepositoryItemChanged(item);
		}
		protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			if(Owner.FocusedView != null)
				Owner.FocusedView.RaiseInvalidValueException(e);
		}
		protected override BaseContainerValidateEditorEventArgs CreateValidateEventArgs(object fValue) {
			if(Owner.FocusedView != null)
				return Owner.FocusedView.CreateValidateEventArgs(fValue);
			return base.CreateValidateEventArgs(fValue);
		}
		protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs va) {
			if(Owner.FocusedView != null)
				Owner.FocusedView.RaiseValidatingEditor(va);
		}
	}
	internal class GridDesignTimeHints {
		public static string DTColumn = "Click to select a column in the property grid;\xd\xamove it to a new position via drag&drop";
		public static string DTBand = "Click to select a band in the property grid;\xd\xamove it to a new position via drag&drop";
	}
}
