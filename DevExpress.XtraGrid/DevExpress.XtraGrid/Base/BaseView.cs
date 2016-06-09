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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections;
using DevExpress.Data;
using DevExpress.Data.Details;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Repository;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Tab;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.Data.Async.Helpers;
using DevExpress.XtraGrid.Printing;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Text;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraGrid.Views.Base {
	[Flags]
	public enum GridRowCellState {
		Dirty = 0,
		Default = 0x001,
		Selected = 0x002,
		Focused = 0x004,
		FocusedCell = 0x008,
		Even = 0x010,
		Odd = 0x020,
		GridFocused = 0x100,
		FocusedAndGridFocused = GridFocused | Focused
	}
	public enum ScrollVisibility { Never, Always, Auto };
	[Flags]
	public enum SynchronizationMode { None = 0, Data = 1, Visual = 2, Full = 3 }
	public enum GridAnimationType { Default, AnimateAllContent, AnimateFocusedItem, NeverAnimate }
	[Designer("DevExpress.XtraGrid.Design.BaseViewDesigner, " + AssemblyInfo.SRAssemblyGridDesign), ToolboxItem(false)]
	public abstract class BaseView : Component, ISupportInitialize, IXtraSerializable, ISkinProvider, IXtraSerializableLayout,
		IXtraSerializableLayoutEx, ISupportXtraSerializer, IServiceProvider, IStringImageProvider, ISkinProviderEx, IXtraSerializableChildren {
		Dictionary<string, object> propertyCache;
		internal bool AllowLayoutWithoutHandle = false;
		public static int MaxRowCopyCount = -1;
		protected const int LayoutIdAppearance = 1, LayoutIdColumns = 2, LayoutIdData = 3, LayoutIdOptionsView = 4, LayoutIdFormatRules = 20;
		int inListChangedEvent = 0;
		ViewRepository viewRepository;
		protected int fUpdateSize;
		BaseViewPrintInfo printInfo;
		int lockSynchronization, dataInitializing = 0;
		bool synchronizeClones, allowSynchronization, loadFired = false;
		BorderStyles borderStyle;
		ViewTab tabControl;
		string paintStyleName;
		ViewPaintStyle paintStyle;
		StyleFormatConditionCollection formatConditions;
		BaseViewHandler handler;
		BaseViewPainter painter;
		protected BaseViewInfo fViewInfo;
		internal BaseGridController useClonedDataController = null;
		BaseGridController dataController;
		BaseInfoRegistrator baseInfo;
		ImageCollection htmlImages;
		bool isPainting, deserializing;
		bool finalizingSerialization;
		BaseViewAppearanceCollection appearancePrint;
		SynchronizationMode requireSynchronization;
		string _viewCaption;
		GridControl fGridControl;
		protected int fLockUpdate, fDetailHeight;
		internal BaseView fParentView, fSourceView;
		protected bool fAllowCloseEditor, fViewDisposing;
		DetailInfo parentInfo;
		string levelName;
		string name = "";
		bool isPrintingOnly = false, isPrinting = false;
		TabHeaderLocation detailTabHeaderLocation;
		OptionsLayoutBase optionsLayout;
		ViewPrintOptionsBase optionsPrint;
		object tag;
		bool fForceDoubleClickCore = false;
		protected bool lockEditorUpdatingByMouse = false;
		[Browsable(false),EditorBrowsable( EditorBrowsableState.Never),XtraSerializableProperty( XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
		public bool ForceDoubleClick {
			get { return fForceDoubleClickCore; }
			set { fForceDoubleClickCore = value; }
		}
		private static readonly object invalidValueException = new object();
		private static readonly object dataSourceChanged = new object();
		private static readonly object rowCountChanged = new object();
		private static readonly object layout = new object();
		private static readonly object paintStyleChanged = new object();
		private static readonly object mouseDown = new object(), mouseUp = new object(), mouseWheel = new object(), mouseMove = new object();
		private static readonly object click = new object(), doubleClick = new object(), lostFocus = new object(), gotFocus = new object(), mouseEnter = new object(), mouseLeave = new object();
		private static readonly object keyUp = new object(), keyDown = new object();
		private static readonly object keyPress = new object();
		private static readonly object layoutUpgrade = new object();
		private static readonly object beforeLoadLayout = new object();
		private static readonly object printPrepareProgress = new object();
		private static readonly object printExportProgress = new object();
		private static readonly object printInitialize = new object();
		private static readonly object validatingEditor = new object();
		BaseViewAppearanceCollection appearance = null;
		GridEmbeddedLookAndFeel elementsLookAndFeel;
		IViewController viewController;
		ProgressWindow progressWindow = null;
		public BaseView() {
			GridLocalizer.ActiveChanged += new EventHandler(OnLocalizer_Changed);
			this.elementsLookAndFeel = new GridEmbeddedLookAndFeel(this);
			this.optionsPrint = CreateOptionsPrint();
			this.optionsPrint.Changed += new BaseOptionChangedEventHandler(OnMiscOptionChanged); 
			this.optionsLayout = CreateOptionsLayout();
			this.viewRepository = null;
			this.appearancePrint = CreateAppearancesPrint();
			this.appearance = CreateAppearances();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.borderStyle = BorderStyles.Default;
			this.deserializing = false;
			this._viewCaption = "";
			this.lockSynchronization = 0;
			this.allowSynchronization = false;
			this.requireSynchronization = SynchronizationMode.None;
			this.synchronizeClones = true;
			this.detailTabHeaderLocation = TabHeaderLocation.Top;
			this.tabControl = new ViewTab(this);
			this.paintStyle = null;
			this.paintStyleName = "Default";
			this.baseInfo = null;
			this.fViewInfo = null;
			this.handler = EmptyViewHandler.Default;
			this.painter = null;
			this.levelName = string.Empty;
			this.fSourceView = null;
			this.parentInfo = null;
			this.isPainting = false;
			SetupDataController();
			this.fViewDisposing = false;
			this.fAllowCloseEditor = true;
			this.fDetailHeight = 350;
			this.fParentView = null;
			this.fGridControl = null;
			this.fLockUpdate = 0; 
			this.formatConditions = new StyleFormatConditionCollection(this);
			this.formatConditions.CollectionChanged += new CollectionChangeEventHandler(OnFormatConditionChanged);
		}
		protected internal virtual bool ShouldProcessOuterMouseEvents { get { return false; } }
		internal void SetPrintingOnly(bool allowPrintingProgress) {
			this.isPrintingOnly = true;
			if(allowPrintingProgress) this.allowPrintProgress++;
			SetViewRepository(null);
		}
		internal bool IsPrintingOnly { get { return isPrintingOnly; } }
		protected internal bool IsPrinting { get { return (GridControl != null && GridControl.IsPrinting) || isPrinting; } }
		protected virtual void SetupDataController() {
			this.dataController = CreateDataController();
			this.dataController.BeforeListChanged += new ListChangedEventHandler(OnDataController_BeforeListChanged);
			this.dataController.ListChanged += new ListChangedEventHandler(OnDataController_ListChanged);
			this.dataController.ListSourceChanged += new EventHandler(OnDataController_DataSourceChanged);
			this.dataController.VisibleRowCountChanged += new EventHandler(OnDataController_VisibleRowCountChanged);
			CheckDataControllerOptions(GridControl == null ? null : GridControl.DataSource);
			if(this.dcsettings.GetOption<bool>("incrementDCUpdate")) this.dataController.BeginUpdate();
			this.dataController.ByPassFilter = this.dcsettings.GetOption<bool>("ByPassFilter");
		}
		protected internal bool IsInListChangedEvent { get { return inListChangedEvent != 0; } }
		protected internal virtual void DestroyDataController() {
			if(DataControllerCore != null) {
				this.dataController.ListSourceChanged -= new EventHandler(OnDataController_DataSourceChanged);
				this.dataController.VisibleRowCountChanged -= new EventHandler(OnDataController_VisibleRowCountChanged);
				this.dataController.BeforeListChanged -= new ListChangedEventHandler(OnDataController_BeforeListChanged);
				this.dataController.ListChanged -= new ListChangedEventHandler(OnDataController_ListChanged);
				if(useClonedDataController != this.dataController) {
					this.dataController.Dispose();
				}
				this.dataController = null;
			}
		}
		protected virtual void OnMiscOptionChanged(object sender, BaseOptionChangedEventArgs e) {
		}
		string ISkinProvider.SkinName {
			get {
				if(GridControl == null) return SkinManager.DefaultSkinName;
				return GridControl.LookAndFeel.ActiveLookAndFeel.SkinName;
			}
		}
		protected internal UserLookAndFeel LookAndFeel { get { return GridControl == null ? UserLookAndFeel.Default : GridControl.LookAndFeel; } }
		protected internal IViewController ViewController {
			get { return viewController == null ? GridControl : viewController; }
			set { viewController = value; }
		}
		protected internal Dictionary<string, object> PropertyCache {
			get {
				if(propertyCache == null) propertyCache = new Dictionary<string,object>();
				return propertyCache;
			}	
		}
		protected internal RepositoryItemGridLookUpEditBase LookUpOwner { get { return GridControl == null ? null : GridControl.LookUpOwner; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool WorkAsLookup { get { return LookUpOwner != null; } }
		protected virtual bool IsLookupPopupVisible { get { return WorkAsLookup && LookUpOwner.OwnerEdit != null && LookUpOwner.OwnerEdit.IsPopupOpen; } }
		protected virtual OptionsLayoutBase CreateOptionsLayout() { return new OptionsLayoutBase(); }
		protected abstract ViewPrintOptionsBase CreateOptionsPrint();
		protected internal GridEmbeddedLookAndFeel ElementsLookAndFeel { get { return elementsLookAndFeel; } }
		protected internal virtual bool CanShowHint { get { return IsDefaultState && !ViewRect.IsEmpty; } }
		protected internal virtual ToolTipControlInfo GetToolTipObjectInfo(Point p) {
			ViewInfo.GInfo.AddGraphics(null);
			try {
				return GetToolTipObjectInfoCore(ViewInfo.GInfo.Cache, p);
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected virtual ToolTipControlInfo GetToolTipObjectInfoCore(GraphicsCache cache, Point p) {
			return null;
		}
		protected internal virtual string GetToolTipText(object hintObject, Point p) {
			return "";
		}
		protected virtual BaseHitInfo GetHintObjectInfo() { return ViewInfo.SelectionInfo.HotTrackedInfo; }
		protected virtual void HideHint() {
			if(GridControl != null) GridControl.EditorHelper.HideHint();
		}
		protected virtual void ShowHint(Point position, ToolTipLocation location) {
			if(GridControl == null) return;
			ToolTipControlInfo info = GetToolTipObjectInfo(position);
			if(info == null) return;
			ToolTipControllerShowEventArgs tool = GridControl.EditorHelper.RealToolTipController.CreateShowArgs();
			tool.ToolTip = info.Text;
			tool.SelectedObject = info.Object;
			tool.SelectedControl = GridControl;
			tool.AutoHide = false;
			tool.ToolTipLocation = location;
			GridControl.EditorHelper.RealToolTipController.ShowHint(tool, position);
		}
		protected internal virtual BorderStyles GetDefaultBorderStyle() { return BorderStyles.Default; }
		protected internal virtual void LayoutChangedSynchronized() {
			BeginSynchronization();
			try {
				LayoutChanged();
			}
			finally {
				EndSynchronization();
			}
		}
		protected virtual void BeginSynchronization() {
			this.lockSynchronization++;
		}
		protected virtual void EndSynchronization() {
			this.lockSynchronization--;
		}
		protected internal virtual bool IsUpdateViewRect { get { return fUpdateSize != 0; } }
		protected internal virtual void OnViewPropertiesChanged(SynchronizationMode mode) {
			if(IsLevelDefault) {
				RequireSynchronization |= mode;
				try {
					SynchronizeClonedViews(this);
				}
				finally {
					RequireSynchronization = SynchronizationMode.None;
				}
				return;
			}
			BaseView view = GetLevelDefault();
			if(view == null) return;
			BaseView lch = GetLayoutChangedDestination();
			if(lch == this) lch = null;
			if(lch != null) lch.BeginUpdate();
			try {
				view.Synchronize(this, mode);
				view.RequireSynchronization |= mode;
				try {
					view.SynchronizeClonedViews(this);
				}
				finally {
					view.RequireSynchronization = SynchronizationMode.None;
				}
			}
			finally {
				if(lch != null) lch.EndUpdate();
			}
		}
		protected internal virtual void SynchronizeClonedViews(BaseView excludeView) {
			if(GridControl == null) return;
			if(!SynchronizeClones) return;
			InternalSynchronizeClonedViews(excludeView, false);
			InternalSynchronizeClonedViews(excludeView, true);
		}
		protected virtual void InternalSynchronizeClonedViews(BaseView excludeView, bool forceSyncrhonize) {
			for(int n = GridControl.Views.Count - 1; n >= 0; n--) {
				BaseView view = GridControl.Views[n] as BaseView;
				if(view.SourceView == this && view != excludeView) {
					view.RequireSynchronization |= RequireSynchronization;
					if(view.ViewRect.IsEmpty) continue;
					if(forceSyncrhonize) view.CheckSynchronize();
				}
			}
		}
		public virtual bool Focus() {
			if(GridControl == null || !IsVisible) return false;
			bool ret = GridControl.Focus();
			GridControl.FocusedView = this;
			UpdateNavigator();
			return ret;
		}
		Hashtable links = new Hashtable();
		protected virtual Hashtable Links { get { return links; } }
		public virtual void Connect(object connector) {
			Links[connector] = 1;
		}
		public virtual void Disconnect(object connector) {
			if(Links.ContainsKey(connector)) Links.Remove(connector);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int LinkCount { get { return Links.Count; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewRepository ViewRepository {
			get { return viewRepository; }
		}
		protected internal void SetViewRepository(ViewRepository value) {
			if(ViewRepository == value) return;
			if(ViewRepository != null) ViewRepository.Views.Remove(this);
			viewRepository = value;
			if(ViewRepository != null) ViewRepository.Views.Add(this);
			OnViewRepositoryChanged();
		}
		protected virtual void OnViewRepositoryChanged() { }
		public virtual void RefreshData() {
			DataController.DoRefresh();
		}
		public virtual void SynchronizeData(BaseView viewSource) {
		}
		public virtual void SynchronizeVisual(BaseView viewSource) {
			SyncBaseProperties(viewSource);
		}
		public virtual void Synchronize(BaseView viewSource) {
			BeginSynchronization();
			BeginUpdate();
			try {
				SynchronizeVisual(viewSource);
				SynchronizeData(viewSource);
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		public virtual void Synchronize(BaseView viewSource, SynchronizationMode mode) {
			BeginSynchronization();
			try {
				switch(mode) {
					case SynchronizationMode.Visual:
						SynchronizeVisual(viewSource);
						break;
					case SynchronizationMode.Data:
						SynchronizeData(viewSource);
						break;
					default:
						Synchronize(viewSource);
						break;
				}
			}
			finally {
				EndSynchronization();
			}
		}
		protected virtual BaseView GetLayoutChangedDestination() {
			if(ParentView == null) return this;
			BaseView view = this;
			while(view.ParentView != null) {
				if(view == GridControl.DefaultView) break;
				view = view.ParentView;
			}
			return view;
		}
		protected virtual bool ForceParentLayoutChanged() {
			if(!IsUpdateViewRect && ParentView != null) {
				Rectangle rect = ViewRect;
				BaseView view = this;
				while(view.ParentView != null) {
					view.ClearMasterCache();
					if(view == GridControl.DefaultView) break;
					view = view.ParentView;
				}
				if(view != this) {
					bool prevAllowSynchronization = view.AllowSynchronization;
					view.AllowSynchronization = false;
					try {
						view.LayoutChanged();
					}
					finally {
						view.AllowSynchronization = prevAllowSynchronization;
					}
					if(rect != ViewRect) return true;
				}
			}
			return false;
		}
		protected internal virtual bool AllowSynchronization { get { return allowSynchronization; } set { allowSynchronization = value; } }
		protected internal virtual void CheckSynchronize() {
			if(RequireSynchronization == SynchronizationMode.None) return;
			BaseView view = GetLevelDefault();
			if(view == null) {
				RequireSynchronization = SynchronizationMode.None;
				return;
			}
			SynchronizationMode sync = RequireSynchronization;
			RequireSynchronization = SynchronizationMode.None;
			Synchronize(view, sync);
		}
		protected virtual bool CanSynchronized {
			get {
				if(this.lockSynchronization != 0) return false;
				if(SynchronizeClones && IsLevelDefault) return true;
				if(SynchronizeClones && GetLevelDefault() != null && AllowSynchronization) {
					return true;
				}
				return false;
			}
		}
		protected internal GridLevelNode GetLevelNode() {
			if(GridControl != null && GridControl.MainView == this) return GridControl.LevelTree;
			if(SourceView != null) {
				return SourceView.LevelNode;
			}
			return LevelNode;
		}
		protected internal GridLevelNode LevelNode {
			get {
				if(GridControl == null) return null;
				return GridControl.LevelTree.Find(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsLevelDefault {
			get { return LevelNode != null && !LevelNode.IsRootLevel; }
		}
		protected internal virtual string LevelDefaultName {
			get {
				if(!IsLevelDefault) return string.Empty;
				return LevelNode.RelationName;
			}
		}
		protected internal virtual bool AllowAssignSplitOptions { get { return true; } }
		protected internal virtual bool IsSplitView { get { return GridControl != null && GridControl.IsSplitGrid; } }
		protected internal bool IsSplitVisible { get { return SplitContainer != null && SplitContainer.IsSplitViewVisible; } }
		protected internal bool IsVerticalSplit { get { return IsSplitView ? !SplitContainer.Horizontal : false; }}
		protected bool IsSplitSynchronizeViews { get { return SplitContainer != null ? SplitContainer.GetSynchronizeViews() : false; } }
		protected bool IsSplitSynchronizeScrolling { get { return SplitContainer != null ? SplitContainer.GetSynchronizeScrolling() : false; } }
		protected bool IsSplitSynchronizeFocus { 
			get {
				if(!IsSplitVisible) return false;
				if(!IsSplitSynchronizeViews) return false;
				return SplitContainer.GetSynchronizeFocusedRow();
			}
		}
		protected GridSplitContainer SplitContainer { get { return GridControl == null ? null : GridControl.SplitContainer; } }
		protected internal BaseView SplitOtherView { 
			get {
				if(!IsSplitView) return null;
				if(SplitContainer.View == this) return SplitContainer.SplitChildView;
				if(SplitContainer.SplitChildView == this) return SplitContainer.View;
				return null; }
		}
		protected internal virtual BaseView GetLevelDefault() {
			if(IsSplitView) {
				return SplitOtherView;
			}
			if(SourceView == null || LevelName == "" || GridControl == null) return null;
			if(SourceView.IsLevelDefault) return SourceView;
			return null;
		}
		protected internal virtual void CheckTabPage() {
			if(!IsDetailView) return;
			if(ParentView != null) ParentView.UpdateSelectedTabPage(TabControl, SourceRowHandle);
		}
		protected internal virtual SynchronizationMode RequireSynchronization { get { return requireSynchronization; } set { requireSynchronization = value; } }
		protected internal virtual void PopulateTab() {
			if(ParentView == null) return;
			ParentView.PopulateTabMasterData(TabControl, SourceRowHandle);
		}
		protected virtual void UpdateSelectedTabPage(ViewTab tabControl, int rowHandle) { }
		protected virtual void PopulateTabMasterData(ViewTab tabControl, int rowHandle) { }
		protected internal virtual ViewTab TabControl { get { return tabControl; } }
		protected internal virtual void InitializeNew() { }
		protected abstract BaseViewAppearanceCollection CreateAppearances();
		protected abstract BaseViewAppearanceCollection CreateAppearancesPrint();
		protected abstract bool CanLeaveFocusOnTab(bool moveForward);
		protected abstract void LeaveFocusOnTab(bool moveForward);
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseAppearanceCollection PaintAppearance { get { return ViewInfo == null ? Appearance : ViewInfo.PaintAppearance; } }
		protected internal virtual void CheckRecreateDataController(object dataSource) {
		}
		class DCSettings {
			Dictionary<string, object> options;
			public bool IsSet(string name) { return options != null && options.ContainsKey(name); }
			public T GetOption<T>(string name) {
				if(!IsSet(name)) return default(T);
				return (T)options[name];
			}
			public void SetOption(string name, object value) {
				if(options == null) options = new Dictionary<string, object>();
				options[name] = value;
			}
			public void Reset() { options = null; }
		}
		DCSettings dcsettings = new DCSettings();
		protected internal virtual void RecreateDataController() {
			this.dcsettings.Reset();
			if(DataControllerCore == null) return;
			this.dcsettings.SetOption("incrementDCUpdate", DataControllerCore.IsUpdateLocked);
			this.dcsettings.SetOption("ByPassFilter", DataControllerCore.ByPassFilter);
			DestroyDataController();
			LayoutChangedSynchronized();
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdAppearance)]
		public BaseAppearanceCollection AppearancePrint { get { return appearancePrint; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdAppearance)]
		public BaseAppearanceCollection Appearance { get { return appearance; } }
		protected internal BaseViewAppearanceCollection ReplaceAppearance(BaseViewAppearanceCollection appearance) {
			BaseViewAppearanceCollection result = this.appearance;
			this.appearance = appearance;
			return result;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewViewCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.Appearance), Localizable(true)]
		public string ViewCaption {
			get { return _viewCaption; }
			set {
				if(value == null) value = string.Empty;
				if(ViewCaption == value) return;
				_viewCaption = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewOptionsLayout"),
#endif
 DXCategory(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayoutBase OptionsLayout {
			get { return optionsLayout; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewOptionsPrint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ViewPrintOptionsBase OptionsPrint {
			get { return optionsPrint; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewBorderStyle"),
#endif
 DefaultValue(BorderStyles.Default), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewHtmlImages")
#else
	Description("")
#endif
]
		public virtual ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -= new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
				LayoutChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(GetAllowHtmlDraw()) LayoutChangedSynchronized();
		}
		protected virtual bool GetAllowHtmlDraw() { return false; }
		[Browsable(false)]
		public virtual bool Editable { get { return false; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewSynchronizeClones"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool SynchronizeClones {
			get { return synchronizeClones; }
			set {
				synchronizeClones = value;
			}
		}
		[Browsable(false), DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ActivePaintStyleName { get { return PaintStyle == null ? PaintStyleName : PaintStyle.Name; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewPaintStyleName"),
#endif
 Browsable(true), DefaultValue("Default"), TypeConverter("DevExpress.XtraGrid.Design.PaintStyleNameConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public virtual string PaintStyleName {
			get { return paintStyleName; }
			set {
				if(PaintStyleName == value) return;
				paintStyleName = value;
				if(IsLoading) return;
				if(GridControl != null) GridControl.EditorHelper.DestroyEditorsCache();
				BeginUpdate();
				try {
					CheckInfo();
					RaisePaintStyleChanged();
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal bool IsSkinned { get { return PaintStyle == null ? false : PaintStyle.IsSkin; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewDetailTabHeaderLocation"),
#endif
 DefaultValue(TabHeaderLocation.Top), XtraSerializableProperty()]
		public virtual TabHeaderLocation DetailTabHeaderLocation {
			get { return detailTabHeaderLocation; }
			set {
				if(DetailTabHeaderLocation == value) return;
				detailTabHeaderLocation = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DefaultValue(""), XtraSerializableProperty()]
		public virtual string Name {
			get {
				if(this.Site != null) return this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false),
		 XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public virtual StyleFormatConditionCollection FormatConditions { get { return formatConditions; } }
		internal void XtraClearFormatConditions(XtraItemEventArgs e) {
			FormatConditions.Clear();
		}
		internal object XtraCreateFormatConditionsItem(XtraItemEventArgs e) {
			StyleFormatCondition formatCondition = new StyleFormatCondition();
			FormatConditions.Add(formatCondition);
			return formatCondition;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewDetailHeight"),
#endif
 DefaultValue(350), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public virtual int DetailHeight {
			get { return fDetailHeight; }
			set {
				if(value < 50) value = 50;
				if(DetailHeight == value) return;
				fDetailHeight = value;
				OnPropertiesChanged();
			}
		}
		protected bool IsViewInfoReady { get { return ViewInfo != null && ViewInfo.IsReady; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewTag"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter)),
		XtraSerializableProperty()
		]
		public virtual object Tag {
			get { return tag; }
			set { tag = value; }
		}
		bool ShouldSerializeGridControl() { return GridControl != null && !WorkAsLookup; }
		[Browsable(false)]
		public GridControl GridControl {
			get { return fGridControl; }
			set {
				if(WorkAsLookup) return;
				SetGridControl(value);
			}
		}
		[Browsable(false)]
		public virtual bool IsDraggingState { get { return false; } }
		[Browsable(false)]
		public virtual bool IsSizingState { get { return false; } }
		[Browsable(false)]
		public virtual bool IsDefaultState { get { return true; } }
		[Browsable(false)]
		public virtual bool IsEditing { get { return false; } }
		[Browsable(false)]
		public virtual bool IsDetailView { get { return ParentView != null; } }
		[Browsable(false)]
		public virtual int DetailLevel {
			get {
				int res = 1;
				BaseView view = ParentView;
				if(view == null) return 0;
				while((view = view.ParentView) != null) res++;
				return res;
			}
		}
		protected internal bool IsKeyboardFocused { get { return GridControl != null && GridControl.FocusedView == this; } }
		[Browsable(false)]
		public virtual bool IsFocusedView { get { return false; } }
		[Browsable(false)]
		public abstract bool IsZoomedView { get; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView ParentView {
			get { return fParentView; }
			set { fParentView = value; }
		}
		[Browsable(false)]
		public abstract Rectangle ViewRect {
			get;
		}
		[Browsable(false)]
		public virtual bool IsDisposing {
			get { return ViewDisposing; }
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return GridControl == null || GridControl.IsLoading; } } 
		[Browsable(false)]
		public virtual object DataSource { get { return DataController.ListSource; } }
		[Browsable(false)]
		public virtual DevExpress.XtraEditors.BaseEdit ActiveEditor { get { return null; } }
		[Browsable(false)]
		public virtual bool IsVisible { get { return false; } }
		public BaseHitInfo CalcHitInfo(Point pt) { return CalcHitInfoCore(pt); }
		public BaseHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		protected virtual BaseHitInfo CalcHitInfoCore(Point pt) { return new BaseHitInfo(); }
		protected internal void OnBeginInit() {
			this.loadFired = false;
		}
		public virtual void BeginInit() {
		}
		public virtual void EndInit() {
			OnEndInit();
		}
		protected internal virtual bool EndEditOnLeave() {
			return true;
		}
		public virtual bool UpdateCurrentRow() {
			return true;
		}
		public virtual void ShowEditorByKey(KeyEventArgs e) {
			ShowEditor();
			if(IsEditing) {
				DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null) {
					if(e.KeyCode != Keys.Enter)
						be.SendKey(e);
				}
			}
		}
		public virtual void ShowEditorByKeyPress(KeyPressEventArgs e) {
			ShowEditor();
			if(IsEditing) {
				DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null && e.KeyChar != 13 && e.KeyChar != 9) {
					be.SendKey(GridControl.lastKeyMessage, e);
				}
			}
		}
		public virtual void ShowEditorByMouse() {
			lockEditorUpdatingByMouse = true;
			try {
			ShowEditor();
			if(IsEditing) {
				DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null) {
					if(Control.MouseButtons == MouseButtons.Left) {
						if(CanSendMouseToEditor(be)) be.SendMouse(ActiveEditor.PointToClient(Control.MousePosition), Control.MouseButtons);
					}
				}
			}
		}
			finally {
				lockEditorUpdatingByMouse = false;
			}
		}
		protected virtual bool CanSendMouseToEditor(BaseEdit editor) { return true; }
		protected virtual internal bool PostEditor(bool causeValidation) { return true; }
		protected virtual internal void CloseEditor(bool causeValidation) {
			if(ViewDisposing || IsClosingEditor) return;
			this.closingEditor++;
			try {
				PostEditor(causeValidation);
				HideEditor();
			}
			finally {
				this.closingEditor--;
			}
		}
		public virtual bool ValidateEditor() { return true; }
		public bool PostEditor() { return PostEditor(true); }
		public virtual void ShowEditor() { }
		public virtual void HideEditor() { }
		int closingEditor = 0;
		protected internal bool IsClosingEditor { get { return closingEditor != 0; } }
		public void CloseEditor() { CloseEditor(true); }
		public virtual void LayoutChanged() {
			ViewInfo.PaintAnimatedItems = false;
			if(!ViewRect.IsEmpty) InvalidateRect(ViewRect);
			if(!IsUpdateViewRect)
				RaiseLayout();
			UpdateNavigator();
		}
		public abstract void PopulateColumns();
		void SyncBaseProperties(BaseView sourceView) {
			bool paintStyleEquals = this.paintStyleName == sourceView.PaintStyleName;
			this.tag = sourceView.Tag;
			this.borderStyle = sourceView.BorderStyle;
			this.fDetailHeight = sourceView.DetailHeight;
			this.detailTabHeaderLocation = sourceView.DetailTabHeaderLocation;
			this.name = sourceView.Name;
			this.paintStyleName = sourceView.PaintStyleName;
			this.synchronizeClones = sourceView.SynchronizeClones;
			this._viewCaption = sourceView.ViewCaption;
			this.OptionsLayout.Assign(sourceView.OptionsLayout);
			this.OptionsPrint.Assign(sourceView.OptionsPrint);
			this.OptionsLayout.LayoutVersion = sourceView.OptionsLayout.LayoutVersion;
			this.Appearance.AssignInternal(sourceView.Appearance);
			this.AppearancePrint.AssignInternal(sourceView.AppearancePrint);
			if(ViewInfo != null) ViewInfo.SetPaintAppearanceDirty();
			if(!paintStyleEquals)
				CheckInfo();
		}
		public virtual void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginUpdate();
			try {
				this.SyncBaseProperties(v);
				if(copyEvents) {
					Events.AddHandler(layoutUpgrade, v.Events[layoutUpgrade]);
					Events.AddHandler(beforeLoadLayout, v.Events[beforeLoadLayout]);
					Events.AddHandler(validatingEditor, v.Events[validatingEditor]);
					Events.AddHandler(printInitialize, v.Events[printInitialize]);
					Events.AddHandler(click, v.Events[click]);
					Events.AddHandler(dataSourceChanged, v.Events[dataSourceChanged]);
					Events.AddHandler(rowCountChanged, v.Events[rowCountChanged]);
					Events.AddHandler(layout, v.Events[layout]);
					Events.AddHandler(paintStyleChanged, v.Events[paintStyleChanged]);
					Events.AddHandler(doubleClick, v.Events[doubleClick]);
					Events.AddHandler(gotFocus, v.Events[gotFocus]);
					Events.AddHandler(invalidValueException, v.Events[invalidValueException]);
					Events.AddHandler(keyDown, v.Events[keyDown]);
					Events.AddHandler(keyPress, v.Events[keyPress]);
					Events.AddHandler(keyUp, v.Events[keyUp]);
					Events.AddHandler(lostFocus, v.Events[lostFocus]);
					Events.AddHandler(mouseDown, v.Events[mouseDown]);
					Events.AddHandler(mouseMove, v.Events[mouseMove]);
					Events.AddHandler(mouseUp, v.Events[mouseUp]);
					Events.AddHandler(mouseWheel, v.Events[mouseWheel]);
					Events.AddHandler(mouseEnter, v.Events[mouseEnter]);
					Events.AddHandler(mouseLeave, v.Events[mouseLeave]);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal Cursor GetCursor() {
			if(GridControl == null) return Cursors.Default;
			return GridControl.Cursor;
		}
		protected internal void SetCursor(Cursor cursor) {
			if(IsDesignMode) return;
			if(GridControl == null) return;
			if(cursor == null) cursor = GridControl.GetDefaultCursor();
			if(!GridControl.UseWaitCursor)
				GridControl.SetCursor(cursor);
		}
		protected internal void ResetDefaultCursor() {
			if(!IsScrollingState)
				SetCursor(Cursors.Default);
		}
		public virtual void ResetCursor() {
			if(!IsScrollingState)
				SetCursor(null);
		}
		int lockInvalidate = 0;
		bool invalidateTriggered = false;
		protected bool IsLockInvalidate { get { return lockInvalidate != 0; } }
		protected internal void LockInvalidate() { 
			if(lockInvalidate++ == 0) invalidateTriggered = false; 
		}
		protected internal void UnlockInvalidate() {
			if(--lockInvalidate == 0) {
				if(invalidateTriggered) Invalidate();
			}
		}
		public virtual void InvalidateRect(Rectangle r) {
			if(r.IsEmpty || GridControl==null) return;
			invalidateTriggered = true;
			if(lockInvalidate != 0) return;
			GridControl.Invalidate(r);
		}
		public virtual void Invalidate() {
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewRect);
		}
		protected internal abstract void ZoomView(BaseView prevView);
		public virtual void ZoomView() {
			if(GridControl == null) return;
			ZoomView(GridControl.DefaultView);
		}
		public abstract void NormalView();
		public virtual void BeginSelection() {
			DataController.Selection.BeginSelection();
		}
		public virtual void EndSelection() {
			DataController.Selection.EndSelection();
		}
		public virtual void CancelSelection() {
			DataController.Selection.CancelSelection();
		}
		public virtual void BeginDataUpdate() {
			CheckLoaded();
		}
		protected virtual internal void EndDataUpdateCore(bool sortOnly) {
		}
		public void EndDataUpdate() { EndDataUpdateCore(false); }
		public virtual void BeginUpdate() {
			if(fLockUpdate++ == 0) OnBeginUpdate();
		}
		protected virtual void OnBeginUpdate() { }
		protected virtual void OnEndUpdate() { }
		public virtual void CopyToClipboard() {
			try {
			DataObject data = new DataObject();
			data.SetData(typeof(string), GetText());
			object selData = GetSelectedData();
			if(selData != null) data.SetData(selData);
			Clipboard.SetDataObject(data);
		}
			catch { }
		}
		public void EndUpdate() {
			EndUpdateCore(false);
		}
		protected internal virtual void EndUpdateCore(bool synchronized) {
			if(--fLockUpdate == 0) {
				OnEndUpdate();
				if(synchronized)
					LayoutChangedSynchronized();
				else
					LayoutChanged();
			}
		}
		public virtual void InvalidateHitObject(BaseHitInfo hitInfo) {
			ViewInfo.PaintAnimatedItems = false;
			Invalidate();
		}
		[Browsable(false)]
		public int DataRowCount {
			get { return DataController.VisibleListSourceRowCount; }
		}
		[Browsable(false)]
		public virtual int RowCount {
			get {
				if(DesignMode) return 2;
				return DataController.VisibleCount;
			}
		}
		public void CheckLoaded() {
			if(ViewDisposing) return;
			if(GridControl != null) {
				GridControl.ForceInitialize();
			}
			if(ViewController == null) return;
			if(!IsInitialized) OnLoaded();
		}
		protected const string crlf = "\xd\xa", columnSeparator = "\t";
		protected virtual object GetSelectedData() { return null; }
		protected virtual string GetText() { return string.Empty; }
		protected virtual void DesignerMakeColumnsVisible() {
		}
		protected void OnLookupViewPropertiesChanged() {
			if(LookUpOwner != null && !IsDeserializing) LookUpOwner.OnViewPropertiesChanged();
		}
		protected internal virtual void OnPropertiesChanged() {
			OnLookupViewPropertiesChanged();
			LayoutChanged();
		}
		protected internal virtual bool CheckCanLeaveCurrentRow(bool raiseUpdateCurrentRow) { return true; }
		protected internal virtual bool CheckCanLeaveRow(int currentRowHandle, bool raiseUpdateCurrentRow) { return true; }
		protected internal virtual bool IsInitialized { get { return loadFired; } }
		protected internal virtual void OnDetailInitialized() { }
		protected virtual void InitializeVisualParameters() {
		}
		protected virtual void InitializeDataParameters() {
		}
		protected virtual void InitializeDataController() {
		}
		protected virtual void UpdateNavigator() {
			if(IsPrintingOnly) return;
			if(GridControl != null && DataController.LockUpdate == 0 && !GridControl.IsLockUpdate) GridControl.UpdateNavigator();
		}
		protected bool IsDataInitializing { get { return dataInitializing != 0; } }
		protected internal virtual void OnLoaded() {
			if(IsLoading) return;
			if(ViewController == null) return;
			if(GridControl != null && GridControl.IsLoading) return;
			if(IsLevelDefault)
				this.levelName = LevelDefaultName;
			if(this.loadFired) return;
			this.loadFired = true;
			BeginUpdate();
			try {
				this.dataInitializing++;
				CheckInfo();
				InitializeVisualParameters();
				InitializeDataParameters();
				SynchronizeDataController();
			}
			finally {
				this.dataInitializing--;
				EndUpdate();
			}
			CreateHandles();
		}
		protected internal virtual void OnGridLoadComplete() { 
		}
		protected internal virtual void CreateHandles() {
		}
		protected virtual void SynchronizeDataController() {
			if(!DataController.IsReady) return;
			DataController.BeginUpdate();
			try {
				InitializeDataController();
			}
			finally {
				DataController.EndUpdate();
			}
		}
		protected internal virtual void Reload() {
			this.loadFired = false;
			OnLoaded();
		}
		protected internal virtual bool OnCheckHotTrackMouseMove(BaseHitInfo hitInfo) { return true; }
		protected internal virtual void OnHotTrackChanged(BaseHitInfo oldInfo, BaseHitInfo newInfo) {
		}
		protected internal virtual void OnRowHotTrackChanged(int oldRowHandle, int newRowHandle) {
		}
		protected internal virtual ViewPaintStyle PaintStyle { get { return paintStyle; } }
		protected internal ViewPaintStyle ReplacePaint(ViewPaintStyle paintStyle, BaseViewPainter painter) {
			ViewPaintStyle result = this.paintStyle;
			this.paintStyle = paintStyle;
			this.painter = painter;
			return result;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BaseInfoRegistrator BaseInfo { get { return baseInfo; } }
		protected internal virtual void ApplyLevelDefaults(BaseView newGV, BaseView defaultView, DetailInfo di, string levelName) {
			newGV.fParentView = this;
			newGV.parentInfo = di;
			newGV.levelName = levelName;
			newGV.fSourceView = defaultView;
		}
		protected internal virtual ViewDrawArgs CreateDrawArgs(DXPaintEventArgs e, GraphicsCache cache) {
			if(cache == null) cache = new GraphicsCache(e, Painter.Paint);
			return new ViewDrawArgs(cache, ViewInfo, Rectangle.Empty);
		}
		protected internal virtual void Draw(GraphicsCache e) {
			if(isPainting || fLockUpdate != 0 || !CheckViewInfo()) return;
			this.isPainting = true;
			ViewDrawArgs dra = CreateDrawArgs(e.PaintArgs, e);
			try {
				Painter.Draw(dra);
			}
			finally {
				this.isPainting = false;
			}
		}
		protected internal virtual void SetTopRowIndexDirty() {
		}
		protected internal virtual void AddToGridControl() {
		}
		protected internal virtual void RemoveFromGridControl() {
			HideHint();
			if(GridControl != null) {
				if(GridControl.MouseCaptureOwner == this)
					GridControl.MouseCaptureOwner = null;
				if(IsKeyboardFocused)
					GridControl.FocusedView = null;
				HideEditor();
				if(ParentView != null) ParentView.CollapseDetail(this);
				DataController.SetDataSource(null, null, null);
			}
		}
		protected virtual void DisposeHandler() {
			if(Handler != null) Handler.Dispose();
			this.handler = EmptyViewHandler.Default;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ProgressWindow = null;
				DisposeHandler();
				GridLocalizer.ActiveChanged -= new EventHandler(OnLocalizer_Changed);
				fViewDisposing = true;
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
				if(GridControl != null) {
					HideEditor();
					if(IsKeyboardFocused || IsDetailView)
						GridControl.FocusedView = ParentView;
					if(GridControl.Container != null)
						GridControl.Container.Remove(this);
					GridControl.RemoveView(this);
					if(ParentView != null)
						ParentView.CollapseDetail(this);
					SetViewRepository(null);
					Links.Clear();
				}
				DestroyDataController();
				if(TabControl != null) {
					this.tabControl.Dispose();
					this.tabControl = null;
				}
				SetGridControl(null);
				ElementsLookAndFeel.Dispose();
				if(ViewInfo!=null) ViewInfo.ClearAnimatedItems();
			}
			base.Dispose(disposing);
		}
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			if(ViewInfo != null) ViewInfo.SetPaintAppearanceDirty();
			if(GridControl != null) GridControl.EditorHelper.ClearViewInfoCache();
			FireChanged();
			OnPropertiesChanged();
		}
		protected virtual BaseGridController CreateDataController() {
			return null; 
		}
		protected internal bool ViewDisposing { get { return fViewDisposing; } }
		protected virtual void OnDataController_ListChanged(object sender, ListChangedEventArgs e) {
			this.inListChangedEvent--;
		}
		void OnDataController_BeforeListChanged(object sender, ListChangedEventArgs e) {
			this.inListChangedEvent++;
		}
		protected virtual void OnDataController_VisibleRowCountChanged(object sender, EventArgs e) {
			UpdateNavigator();
			RaiseRowCountChanged();
		}
		internal void FireDataController_DataSourceChanged() { OnDataController_DataSourceChanged(this, EventArgs.Empty); }
		protected virtual void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			SetTopRowIndexDirty();
			UpdateNavigator();
			RaiseDataSourceChanged();
		}
		protected internal virtual void OnDataSourceChanging() {
		}
		protected void StopScrolling() {
			if(GridControl != null) GridControl.StopScroller();
		}
		protected virtual internal void StartScrolling() {
			SetDefaultState();
			if(!IsDefaultState || GridControl == null) return;
			GridControl.Scroller = CreateScroller();
			GridControl.Scroller.Start(GridControl);
		}
		protected internal virtual BaseViewOfficeScroller CreateScroller() { return new BaseViewOfficeScroller(this); }
		protected internal virtual bool IsScrollingState { get { return false; } }
		protected internal virtual void SetScrollingState() { }
		protected internal virtual void SetDefaultState() { }
		protected virtual void OnEndInit() { }
		protected internal virtual void CheckInfo() {
			SetupInfo();
			if(ViewController == null || BaseInfo == null) return;
			if(ViewController != null) ViewController.EditorHelper.ClearViewInfoCache();
			if(ViewInfo != null)
				ViewInfo.SetPaintAppearanceDirty();
		}
		protected virtual void OnGridControlChanged(GridControl prevControl) {
			if(GridControl == null) return;
			if(ViewInfo == null) this.fViewInfo = CreateNullViewInfo();
			CheckRecreateDataController(DataController.ListSource);
			CheckRightToLeft();
			if(IsLoading) return;
			CheckInfo();
		}
		protected virtual void OnLocalizer_Changed(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected internal virtual void SetDataSource(BindingContext context, object dataSource, string dataMember) {
			if(!IsLoading && !IsInitialized) OnLoaded();
			object validSource = null;
			if(dataSource != null) {
				validSource = MasterDetailHelper.GetDataSource(GridControl == null || GridControl.GetIsServerMode(dataSource) ? null : context, dataSource, dataMember);
				if(validSource is ListIEnumerable) dataSource = validSource;
			}
			CheckRecreateDataController(validSource);
			if(useClonedDataController == null) 
				DataController.SetDataSource(context, dataSource, dataMember);
		}
		protected abstract BaseViewInfo CreateNullViewInfo();
		protected virtual void SetupInfo() {
			if(ViewController == null) return;
			baseInfo = ViewController.AvailableViews[ViewName];
			if(BaseInfo == null) return;
			this.paintStyle = BaseInfo.PaintStyleByView(this);
			this.paintStyle.UpdateElementsLookAndFeel(this);
			this.handler = BaseInfo.CreateHandler(this);
			this.painter = BaseInfo.CreatePainter(this);
			this.fViewInfo = BaseInfo.CreateViewInfo(this);
			TabControl.CheckInfo();
		}
		protected virtual void OnFormatConditionChanged(object sender, CollectionChangeEventArgs e) {
			if(IsLoading) return;
			OnPropertiesChanged();
			FireChanged();
		}
		protected virtual void RaiseDataSourceChanged() {
			EventHandler handler = (EventHandler)Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseRowCountChanged() {
			EventHandler handler = (EventHandler)Events[rowCountChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected bool AllowLayoutEvent { get { return GridControl != null && GridControl.AllowLayoutEvent; } }
		protected virtual void RaiseLayout() {
			if(!IsInitialized || !AllowLayoutEvent) return;
			EventHandler handler = (EventHandler)Events[layout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void InternalSetViewRectCore(Rectangle rect) {
			SetViewRect(rect);
		}
		protected internal virtual void ClearMasterCache() {
		}
		protected internal virtual void OnEmbeddedNavigatorSizeChanged() {
		}
		protected abstract void SetViewRect(Rectangle newValue);
		protected internal virtual void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[invalidValueException];
			if(handler != null) handler(this, e);
		}
		protected internal virtual BaseView CreateInstance() {
			if(BaseInfo == null) return null;
			return BaseInfo.CreateView(GridControl);
		}
		protected internal virtual void OnLostCapture() { }
		protected internal virtual void OnGotCapture() { }
		protected internal virtual void OnLostFocus() {
			RaiseLostFocus(EventArgs.Empty);
		}
		protected internal virtual void OnGotFocus() {
			RaiseGotFocus(EventArgs.Empty);
		}
		protected internal virtual void OnLeave() {
			if(GridControl == null) return;
			GridControl grid = GridControl;
			try {
				grid.EditorHelper.BeginAllowHideException();
				CloseEditor();
			}
			catch(HideException) { }
			finally {
				grid.EditorHelper.EndAllowHideException();
			}
		}
		protected void ValidateExpression(CriteriaOperator criterion) {
			DataController.ValidateExpression(criterion);
		}
		protected void ValidateExpressionString(string expression) {
			ValidateExpression(CriteriaOperator.Parse(expression));
		}
		protected internal virtual bool IsExpressionValid(string expression, out Exception exception) {
			exception = null;
			if(string.IsNullOrEmpty(expression)) return false;
			try {
				ValidateExpressionString(expression);
			} catch(Exception e) {
				exception = e;
				return false;
			}
			return true;
		}   
		protected internal abstract void OnChildLayoutChanged(BaseView childView);
		protected internal abstract void OnVisibleChanged();
		protected internal virtual void OnRepositoryItemRemoved(DevExpress.XtraEditors.Repository.RepositoryItem item) {
		}
		protected internal virtual void OnEditorsRepositoryChanged() {
		}
		protected virtual bool CanCalculateLayout {
			get {
				if(IsLoading || ViewInfo == null || fLockUpdate != 0  || ViewDisposing || GridControl == null || GridControl.GridDisposing || IsPainting || !GridControl.IsInitializedCore) return false;
				return true;
			}
		}
		protected internal virtual bool CheckViewInfo() {
			if(!IsInitialized) return false;
			if(IsLockUpdate) return false;
			if(ViewInfo.IsReady) {
				if(ViewInfo.IsDataDirty) return CalculateData();
				return true;
			}
			CalculateLayoutSynchronized();
			return ViewInfo.IsReady;
		}
		protected virtual void SetLayoutDirty() {
			if(ViewInfo == null || !ViewInfo.IsReady || ViewInfo.IsDataDirty) return;
			ViewInfo.SetDataDirty();
			if(ViewRect.IsEmpty) return;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewRect);
		}
		protected virtual bool CalculateData() {
			return CalculateLayout(); 
		}
		protected virtual bool CalculateLayout() { return true; }
		protected virtual void CalculateLayoutSynchronized() {
			BeginSynchronization();
			try {
				CalculateLayout();
			}
			finally {
				EndSynchronization();
			}
		}
		protected internal virtual void FocusCreator() {
		}
		protected internal virtual void DoMoveFocusedRow(int delta, KeyEventArgs e) { }
		protected internal virtual BaseView CanMoveFocusedRow(int delta) {
			return null;
		}
		protected internal virtual void OnStyleChanged(object sender, System.EventArgs e) {
			FireChanged();
		}
		protected internal virtual void CollapseDetail(BaseView view) {
		}
		protected internal virtual void CollapseDetails(int rowHandle) {
		}
		protected internal virtual void CollapseAllDetailsCore() { }
		protected internal virtual int CalcRealViewHeight(Rectangle rect) {
			return DetailHeight;
		}
		protected internal virtual IList GetChildDataView(int rowHandle, int relationIndex) {
			return null;
		}
		protected internal enum DataControllerType { Regular, RegularNoCurrencyManager, ServerMode, AsyncServerMode }
		internal static DataControllerType DetectServerModeType(object dataSource) {
			if(dataSource is IListSource)
				dataSource = ((IListSource)dataSource).GetList();
			if(dataSource is AsyncListServer2DatacontrollerProxy) return DataControllerType.AsyncServerMode;
			if(dataSource is IListServer) return DataControllerType.ServerMode;
			return DataControllerType.Regular;
		}
		protected internal bool IsAsyncServerMode {
			get {
				return IsServerMode && DataControllerCore is AsyncServerModeDataController;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsServerMode {
			get {
				if(GridControl == null) return false;
				return DataControllerCore != null && DataControllerCore.IsServerMode;
			}
		}
		protected internal bool IsLockUpdate { get { return fLockUpdate != 0; } }
		protected internal void CancelUpdate() {
			--fLockUpdate;
		}
		protected internal virtual void FireChanged() {
			if(IsDeserializing) return;
			if(GridControl != null) GridControl.FireChanged(this);
		}
		public virtual DevExpress.XtraGrid.Export.BaseExportLink CreateExportLink(DevExpress.XtraExport.IExportProvider provider) {
			return null;
		}
		protected BaseViewPrintInfo PrintInfo { get { return printInfo; } }
		internal void ResetPrintInfo() {
			printInfo = null;
		}
		protected internal virtual bool IsRecreateOnMarginChanged { get { return false; } }
		protected internal virtual bool IsSupportPrinting { get { return false; } }
		protected void CreatePrintInfo(ViewPrintWrapper printWrapper, IBrickGraphics graph) {
			this.printInfo = CreatePrintInfoCore(new PrintInfoArgs(this, printWrapper, graph, 0));
		}
		protected BaseViewPrintInfo CreatePrintInfoCore(PrintInfoArgs args) {
			this.isPrinting = true;
			try {
				BaseViewPrintInfo pi = CreatePrintInfoInstance(args);
				pi.Initialize();
				return pi;
			}
			finally {
				this.isPrinting = false;
			}
		}
		protected virtual BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) { return new BaseViewPrintInfo(args); }
		protected internal virtual void CreatePrintArea(ViewPrintWrapper printWrapper, string areaName, IBrickGraphics graph) {
			if(areaName == "ReportHeader" || (PrintInfo == null && areaName == "DetailHeader")) {
				CreatePrintInfo(printWrapper, graph);
			}
			if(PrintInfo == null) return;
			switch(areaName) {
				case "ReportHeader": PrintInfo.PrintViewHeader(graph);
					break;
				case "DetailHeader": PrintInfo.PrintHeader(graph);
					break;
				case "Detail": PrintInfo.PrintRows(graph);
					break;
				case "DetailPart": PrintInfo.PrintNextRow(graph);
					break;
				case "DetailFooter": PrintInfo.PrintFooter(graph);
					break;
			}
		}
		protected internal virtual void InitializePrinting() { }
		protected internal virtual int GetDetailCount() { return printInfo != null ? printInfo.GetDetailCount() : 0; }
		protected internal virtual UserControl PrintDesigner { get { return null; } }
		protected virtual string ViewName { get { return "BaseView"; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BaseViewInfo GetViewInfo() { return ViewInfo; }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseGridController DataController { 
			get {
				if(dataController == null) SetupDataController();
				return dataController; 
			} 
		}
		internal BaseGridController DataControllerCore { get { return dataController; } }
		protected internal virtual BaseViewHandler Handler { get { return handler; } }
		protected internal virtual BaseViewPainter Painter { get { return painter; } }
		protected internal virtual BaseViewInfo ViewInfo { get { return fViewInfo; } }
		protected internal bool IsPainting { get { return isPainting; } }
		protected internal virtual bool NeedReLayout { 
			get { return false; }
		}
		protected internal virtual bool IsAutoCollapseDetail { get { return false; } }
		protected internal virtual bool IsAllowZoomDetail { get { return false; } }
		protected internal virtual void SetGridControl(GridControl newControl) {
			GridControl prev = GridControl;
			if(prev == newControl) return;
			this.fGridControl = newControl;
			if(GridControl != null && ViewRepository == null) {
				GridControl.ViewCollection.Add(this);
			}
			OnGridControlChanged(prev);
		}
		public virtual string GetViewCaption() {
			if(GridControl == null || !IsInitialized) return string.Empty;
			string res = ViewCaption;
			if(res == "") res = LevelName;
			if(res == "") {
				if(GridControl.MainView == this) {
					res = "MainView";
				}
				else {
					GridLevelNode node = GridControl.LevelTree.Find(this);
					if(node != null) res = node.RelationName;
				}
			}
			if(res == "") res = BaseInfo.ViewName;
			return res;
		}
		protected internal virtual DetailInfo ParentInfo { get { return parentInfo; } }
		[Browsable(false)]
		public virtual string LevelName { get { return levelName; } }
		internal void SetLevelName(string newName) { this.levelName = newName; }
		[Browsable(false)]
		public virtual BaseView SourceView { get { return fSourceView; } }
		[Browsable(false)]
		public virtual object SourceRow {
			get { return (ParentInfo != null ? ParentInfo.MasterRow.ParentRow : null); }
		}
		[Browsable(false)]
		public virtual int SourceRowHandle {
			get { return (ParentInfo != null ? ParentInfo.MasterRow.ParentControllerRow : GridControl.InvalidRowHandle); }
		}
		protected virtual IDesignerHost DesignerHost {
			get {
				if(Site == null) return null;
				return Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}
		protected void SetDeserializing(bool deserializing) {
			this.deserializing = deserializing;
		}
		DesignerTransaction transaction = null;
		protected virtual void OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			this.deserializing = true;
			this.finalizingSerialization = true;
			this.transaction = (DesignerHost != null ? DesignerHost.CreateTransaction("OnStartDeserializing") : null);
			this.loadFired = false;
			BeginUpdate();
		}
		protected virtual void OnEndDeserializing(string restoredVersion) {
			this.deserializing = false;
			try {
				if(transaction != null) {
					transaction.Commit();
				}
				transaction = null;
				OnLoaded();
				if(restoredVersion != OptionsLayout.LayoutVersion) RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
			}
			finally {
				EndUpdate();
				this.finalizingSerialization = false;
			}
			FireChanged();
		}
		protected internal virtual bool IsDesignMode { get { return DesignMode; } }
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.StoreAllOptions) {
				OptionsPrint.Reset();
			}
		}
		protected internal virtual bool IsDeserializing { get { return deserializing; } }
		internal bool IsFinalizingSerialization { get { return finalizingSerialization; } } 
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		#region Export
		ComponentPrinterBase Printer { 
			get {
				if(GridControl == null) throw new InvalidOperationException("GridControl is not initalized");
				return GridControl.Printer; 
			} 
		}
		protected void ClearDocumentIfNeeded() {
			if(OptionsPrint.AutoResetPrintDocument) {
				if(GridControl != null && GridControl.IsPrinterExist) {
					if(IsServerMode) {
						Printer.ClearDocument();
					}
					else
						Printer.ClearDocument();
				}
			}
		}
		int allowPrintProgress = 0;
		protected void ExecutePrintExport(Action0 method) {
			if(OptionsPrint.AutoResetPrintDocument && IsServerMode) {
				GridControl.PrintWrapper = null;
			}
			this.allowPrintProgress++;
			try {
				CheckPrintWrapper();
				ClearDocumentIfNeeded();
				if(Printer == null) return;
				if(GridControl != null && !GridControl.IsPrintWrapperValid) {
					OnPrintEnd(true);
					return;
				}
				method();
			}
			finally {
				this.allowPrintProgress--;
			}
		}
		protected void CheckPrintWrapper() {
			if(GridControl == null) return;
			GridControl.CheckPrintWrapper();
		}
		public void CreateDocument() { ExecutePrintExport(delegate() { Printer.CreateDocument(); }); }
		public void ClearDocument() {
			if(GridControl == null) return;
			if(IsServerMode) GridControl.PrintWrapper = null;
			GridControl.InternalClearDocument();
		}
		public void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			ExecutePrintExport(delegate() { Printer.Export(target, filePath, options); });
		}
		public void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			ExecutePrintExport(delegate() { Printer.Export(target, stream, options); });
		}
		public void Export(ExportTarget target, string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(target, filePath); });
		}
		public void Export(ExportTarget target, Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(target, stream); });
		}
		public void ShowPrintPreview() {
			ExecutePrintExport(delegate() { Printer.ShowPreview(FindOwnerForm(), ElementsLookAndFeel); });
		}
		public void Print() {
			ExecutePrintExport(delegate() { Printer.Print(); });
		}
		public void PrintDialog() {
			ExecutePrintExport(delegate() { Printer.PrintDialog(); });
		}
		public void ShowRibbonPrintPreview() {
			ExecutePrintExport(delegate() { Printer.ShowRibbonPreview(FindOwnerForm(), ElementsLookAndFeel); });
		}
		public void ExportToXlsx(string filePath) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xlsx, filePath); }); }
		public void ExportToXlsx(Stream stream) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xlsx, stream); }); }
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xlsx, stream, options); }); }
		public void ExportToXlsx(string filePath, XlsxExportOptions options) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xlsx, filePath, options); }); }
		public void ExportToXls(string filePath) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xls, filePath); }); }
		public void ExportToXls(Stream stream) { ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xls, stream); }); }
		[Obsolete("Use the ExportToXls(string filePath, XlsExportOptions options) method instead")]
		public void ExportToXls(string filePath, bool useNativeFormat) {
			XlsExportOptions options = new XlsExportOptions(useNativeFormat ? TextExportMode.Value : TextExportMode.Text);
			ExportToXls(filePath, options);
		}
		[Obsolete("Use the ExportToXls(Stream stream, XlsExportOptions options) method instead")]
		public void ExportToXls(Stream stream, bool useNativeFormat) {
			XlsExportOptions options = new XlsExportOptions(useNativeFormat ? TextExportMode.Value : TextExportMode.Text);
			ExportToXls(stream, options);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xls, stream, options); });
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Xls, filePath, options); });
		}
		public void ExportToRtf(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Rtf, filePath); });
		}
		public void ExportToRtf(Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Rtf, stream); });
		}
		public void ExportToHtml(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Html, filePath); });
		}
		public void ExportToHtml(Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Html, stream); });
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Html, stream, options); });
		}
		public void ExportToHtml(String filePath, HtmlExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Html, filePath, options); });
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet);
			ExportToHtml(filePath, options);
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet, title, compressed);
			ExportToHtml(filePath, options);
		}
		[Obsolete("Use the ExportToHtml(Stream stream, HtmlExportOptions options) method instead")]
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet, title, compressed);
			ExportToHtml(stream, options);
		}
		public void ExportToMht(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Mht, filePath); });
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Mht, filePath, options); });
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Mht, stream, options); });
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet);
			ExportToMht(filePath, options);
		}
		[Obsolete("Use the ExportToMht(string filePath, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			ExportToMht(filePath, options);
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			ExportToMht(stream, options);
		}
		public void ExportToPdf(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Pdf, filePath); });
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Pdf, filePath, options); });
		}
		public void ExportToPdf(Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Pdf, stream); });
		}
		public void ExportToText(Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Text, stream); });
		}
		public void ExportToText(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Text, filePath); });
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Text, filePath, options); });
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Text, stream, options); });
		}
		public void ExportToCsv(Stream stream) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Csv, stream); });
		}
		public void ExportToCsv(string filePath) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Csv, filePath); });
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Csv, filePath, options); });
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			ExecutePrintExport(delegate() { Printer.Export(ExportTarget.Csv, stream, options); });
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator) {
			TextExportOptions options = new TextExportOptions(separator);
			ExportToText(filePath, options);
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators) {
			TextExportOptions options = new TextExportOptions(separator);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExportToText(filePath, options);
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			TextExportOptions options = new TextExportOptions(separator, encoding);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExportToText(filePath, options);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator) {
			TextExportOptions options = new TextExportOptions(separator);
			ExportToText(stream, options);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators) {
			TextExportOptions options = new TextExportOptions(separator);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExportToText(stream, options);
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			TextExportOptions options = new TextExportOptions(separator, encoding);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExportToText(stream, options);
		}
		#endregion
		[Obsolete("Use the ExportToHtml method instead")]
		public void ExportToHtmlOld(string fileName) {
			CreateExportLink(new DevExpress.XtraExport.ExportHtmlProvider(fileName)).ExportTo(true);
		}
		[Obsolete("Use the ExportToXls method instead")]
		public void ExportToExcelOld(string fileName) {
			CreateExportLink(new DevExpress.XtraExport.ExportXlsProvider(fileName)).ExportTo(true);
		}
		[Obsolete("Use the ExportToText method instead")]
		public void ExportToTextOld(string fileName) {
			CreateExportLink(new DevExpress.XtraExport.ExportTxtProvider(fileName)).ExportTo(true);
		}
		public void SaveLayoutToXml(string xmlFile) { SaveLayoutToXml(xmlFile, OptionsLayout); }
		public void RestoreLayoutFromXml(string xmlFile) { RestoreLayoutFromXml(xmlFile, OptionsLayout); }
		public void SaveLayoutToRegistry(string path) { SaveLayoutToRegistry(path, OptionsLayout); }
		public void RestoreLayoutFromRegistry(string path) { RestoreLayoutFromRegistry(path, OptionsLayout); }
		public void SaveLayoutToStream(Stream stream) { SaveLayoutToStream(stream, OptionsLayout); }
		public void RestoreLayoutFromStream(Stream stream) { RestoreLayoutFromStream(stream, OptionsLayout); }
		public void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		public void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(this, stream, "View", options);
			else
				serializer.SerializeObject(this, path.ToString(), "View", options);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, "View", options);
			else
				serializer.DeserializeObject(this, path.ToString(), "View", options);
		}
		protected internal Point PointToScreen(Point point) {
			if(GridControl == null || !GridControl.IsHandleCreated) return point;
			return GridControl.PointToScreen(point);
		}
		protected internal Point PointToClient(Point point) {
			if(IsPrintingOnly) return Point.Empty;
			if(GridControl == null || !GridControl.IsHandleCreated) return point;
			return GridControl.PointToClient(point);
		}
		#region events
		protected internal virtual BaseContainerValidateEditorEventArgs CreateValidateEventArgs(object value) {
			return new BaseContainerValidateEditorEventArgs(value);
		}
		protected internal virtual void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BaseContainerValidateEditorEventHandler handler = (BaseContainerValidateEditorEventHandler)this.Events[validatingEditor];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseDown(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseDown];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseUp(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseUp];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseMove(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseMove];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseWheel(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseWheel];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseEnter(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[mouseEnter];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseMouseLeave(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[mouseLeave];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseClick(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[click];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseDoubleClick(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[doubleClick];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseLostFocus(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[lostFocus];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseGotFocus(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[gotFocus];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseKeyDown(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[keyDown];
			if(handler != null) handler(this, e);
		}
		internal void RaiseKeyUp(KeyEventArgs e) {
			KeyEventHandler handler = (KeyEventHandler)this.Events[keyUp];
			if(handler != null) handler(this, e);
		}
		internal void RaiseKeyPress(KeyPressEventArgs e) {
			KeyPressEventHandler handler = (KeyPressEventHandler)this.Events[keyPress];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePaintStyleChanged() {
			EventHandler handler = (EventHandler)this.Events[paintStyleChanged];
			if(handler != null) handler(this, new EventArgs());
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePrintPrepareProgress(ProgressChangedEventArgs e) {
			ProgressChangedEventHandler handler = (ProgressChangedEventHandler)this.Events[printPrepareProgress];
			if(handler != null) handler(this, e);
		}
		protected internal void OnPrintExportProgress(int overallProgress, bool onlyRefreshProgressWindow) {
			if(onlyRefreshProgressWindow) {
				if(ProgressWindow != null) ProgressWindow.ProcessEvents();
				return;
			}
			if(ProgressWindow != null) ProgressWindow.SetProgress(overallProgress);
			RaisePrintExportProgress(new ProgressChangedEventArgs(overallProgress, null));
		}
		protected internal virtual void RaisePrintExportProgress(ProgressChangedEventArgs e) {
			ProgressChangedEventHandler handler = (ProgressChangedEventHandler)this.Events[printExportProgress];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePrintInitialize(PrintInitializeEventArgs e) {
			PrintInitializeEventHandler handler = (PrintInitializeEventHandler)this.Events[printInitialize];
			if(handler != null) handler(this, e);
		}
		[ DXCategory(CategoryName.Data)]
		internal event ProgressChangedEventHandler PrintPrepareProgress {
			add { this.Events.AddHandler(printPrepareProgress, value); }
			remove { this.Events.RemoveHandler(printPrepareProgress, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewPrintExportProgress"),
#endif
 DXCategory(CategoryName.Data)]
		public event ProgressChangedEventHandler PrintExportProgress {
			add { this.Events.AddHandler(printExportProgress, value); }
			remove { this.Events.RemoveHandler(printExportProgress, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewPrintInitialize"),
#endif
 DXCategory(CategoryName.Data)]
		public event PrintInitializeEventHandler PrintInitialize {
			add { this.Events.AddHandler(printInitialize, value); }
			remove { this.Events.RemoveHandler(printInitialize, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewLayoutUpgrade"),
#endif
 DXCategory(CategoryName.Data)]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewPaintStyleChanged"),
#endif
 DXCategory(CategoryName.Properties), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler PaintStyleChanged {
			add { this.Events.AddHandler(paintStyleChanged, value); }
			remove { this.Events.RemoveHandler(paintStyleChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewBeforeLoadLayout"),
#endif
 DXCategory(CategoryName.Data)]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewKeyDown"),
#endif
 DXCategory(CategoryName.Key)]
		public event KeyEventHandler KeyDown {
			add { this.Events.AddHandler(keyDown, value); }
			remove { this.Events.RemoveHandler(keyDown, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewKeyUp"),
#endif
 DXCategory(CategoryName.Key)]
		public event KeyEventHandler KeyUp {
			add { this.Events.AddHandler(keyUp, value); }
			remove { this.Events.RemoveHandler(keyUp, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewKeyPress"),
#endif
 DXCategory(CategoryName.Key)]
		public event KeyPressEventHandler KeyPress {
			add { this.Events.AddHandler(keyPress, value); }
			remove { this.Events.RemoveHandler(keyPress, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseDown"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event MouseEventHandler MouseDown {
			add { this.Events.AddHandler(mouseDown, value); }
			remove { this.Events.RemoveHandler(mouseDown, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseUp"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event MouseEventHandler MouseUp {
			add { this.Events.AddHandler(mouseUp, value); }
			remove { this.Events.RemoveHandler(mouseUp, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseWheel"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event MouseEventHandler MouseWheel {
			add { this.Events.AddHandler(mouseWheel, value); }
			remove { this.Events.RemoveHandler(mouseWheel, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseMove"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event MouseEventHandler MouseMove {
			add { this.Events.AddHandler(mouseMove, value); }
			remove { this.Events.RemoveHandler(mouseMove, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseEnter"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event EventHandler MouseEnter {
			add { this.Events.AddHandler(mouseEnter, value); }
			remove { this.Events.RemoveHandler(mouseEnter, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewMouseLeave"),
#endif
 DXCategory(CategoryName.Mouse)]
		public event EventHandler MouseLeave {
			add { this.Events.AddHandler(mouseLeave, value); }
			remove { this.Events.RemoveHandler(mouseLeave, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewClick"),
#endif
 DXCategory(CategoryName.Action)]
		public event EventHandler Click {
			add { this.Events.AddHandler(click, value); }
			remove { this.Events.RemoveHandler(click, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewDoubleClick"),
#endif
 DXCategory(CategoryName.Action)]
		public event EventHandler DoubleClick {
			add { this.Events.AddHandler(doubleClick, value); }
			remove { this.Events.RemoveHandler(doubleClick, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewLostFocus"),
#endif
 DXCategory(CategoryName.Focus)]
		public event EventHandler LostFocus {
			add { this.Events.AddHandler(lostFocus, value); }
			remove { this.Events.RemoveHandler(lostFocus, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewGotFocus"),
#endif
 DXCategory(CategoryName.Focus)]
		public event EventHandler GotFocus {
			add { this.Events.AddHandler(gotFocus, value); }
			remove { this.Events.RemoveHandler(gotFocus, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewValidatingEditor"),
#endif
 DXCategory(CategoryName.Editor)]
		public event BaseContainerValidateEditorEventHandler ValidatingEditor {
			add { this.Events.AddHandler(validatingEditor, value); }
			remove { this.Events.RemoveHandler(validatingEditor, value); }
		}
		[DXCategory(CategoryName.PropertyChanged), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewDataSourceChanged")
#else
	Description("")
#endif
]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[DXCategory(CategoryName.PropertyChanged), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewRowCountChanged")
#else
	Description("")
#endif
]
		public event EventHandler RowCountChanged {
			add { this.Events.AddHandler(rowCountChanged, value); }
			remove { this.Events.RemoveHandler(rowCountChanged, value); }
		}
		[DXCategory(CategoryName.Action), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewLayout")
#else
	Description("")
#endif
]
		public event EventHandler Layout {
			add { this.Events.AddHandler(layout, value); }
			remove { this.Events.RemoveHandler(layout, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BaseViewInvalidValueException"),
#endif
 DXCategory(CategoryName.Action)]
		public event InvalidValueExceptionEventHandler InvalidValueException {
			add { this.Events.AddHandler(invalidValueException, value); }
			remove { this.Events.RemoveHandler(invalidValueException, value); }
		}
		#endregion
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected bool HasAccessible { get { return this.dxAccessible != null; } }
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() { return null; }
		public virtual object GetRow(int rowHandle) {
			return null;
		}
		protected internal void OnTabSelectedPageChanging(ViewTab viewTab, DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangingEventArgs e) {
			CloseEditor();
			e.Cancel = !UpdateCurrentRow();
			if(e.Cancel) return;
			if(ParentView != null) ParentView.OnChildViewTabChanging(this, e);
		}
		protected internal void OnTabSelectedPageChanged(ViewTab viewTab, DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangedEventArgs e) {
			if(ParentView != null) ParentView.OnChildViewTabChanged(this, e);
		}
		protected virtual void OnChildViewTabChanged(BaseView childView, DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangedEventArgs e) { }
		protected virtual void OnChildViewTabChanging(BaseView baseView, DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangingEventArgs e) { }
		protected internal virtual void OnEnter() { }
		protected internal virtual void ClearDataProperties() {
		}
		internal void CheckDataControllerOptions(object dataSource) {
			if(GridControl == null || this.dataController == null) return;
			bool serverMode = GridControl.GetIsServerMode(dataSource); 
			bool allowRestore = GridControl.AllowRestoreSelectionAndFocusedRow == DefaultBoolean.True;
			if(!serverMode && GridControl.AllowRestoreSelectionAndFocusedRow == DefaultBoolean.Default) allowRestore = true;
			if(DataController != null) DataController.AllowRestoreSelection = allowRestore;
		}
		protected internal abstract void ResetLookUp(bool sameDataSource);
		protected internal virtual bool OnContextMenuClick() { return false; }
		protected internal virtual bool CanCollapseMasterRow(int rowHandle) {
			return true;
		}
		protected internal virtual void OnDefaultViewStop() {
		}
		internal void ResetEvents() {
			Events.Dispose();
		}
		protected void IncrementPaintScrollCounters() {
			if(GridControl != null) GridControl.IncrementScrollCounters();
		}
		protected internal virtual void ReportProgress(int progress) {
			RaisePrintPrepareProgress(new ProgressChangedEventArgs(progress, null));
		}
		internal bool ProgressWindowCancelPending {
			get { return ProgressWindow != null && ProgressWindow.CancelPending; }
		}
		internal ProgressWindow ProgressWindow {
			get { return progressWindow; }
			set {
				if(progressWindow == value) return;
				if(progressWindow != null) progressWindow.Dispose();
				this.progressWindow = value;
			}
		}
		protected internal virtual void OnPrintInitialize(IPrintingSystem printingSystem, PrintableComponentLinkBase link) {
				RaisePrintInitialize(new PrintInitializeEventArgs(printingSystem, link));
		}
		protected internal virtual bool AllowPrintProgress { get { return OptionsPrint.ShowPrintExportProgress && allowPrintProgress > 0; } }
		ViewPrintWrapper printWrapper = null;
		internal void OnPrintStart(ViewPrintWrapper printWrapper){
			if (!PrepareOnPrintStart(printWrapper)) return;
			ShowProgressForm(GetPrintWrapperActivity());
		}
		internal void OnPrintStart(ViewPrintWrapper printWrapper, PrintingSystemActivity activity){
			if(!PrepareOnPrintStart(printWrapper)) return;
			ShowProgressForm(activity);
		}
		bool PrepareOnPrintStart(ViewPrintWrapper printWrapper){
			if(!IsPrintingOnly && (GridControl == null || !GridControl.IsHandleCreated)) return false;
			this.printWrapper = printWrapper;
			return true;
		}
		internal void ShowMarqueProgressForm(){
			ShowProgressForm(GetPrintWrapperActivity());
			if(ProgressWindow == null) return;
			ProgressWindow.SetMarqueProgress(Localizer.Active.GetLocalizedString(StringId.ProgressLoadingData));
		}
		PrintingSystemActivity GetPrintWrapperActivity(){
			return printWrapper != null ? printWrapper.PrinterActivity : PrintingSystemActivity.Idle;
		}
		internal Form FindOwnerForm() { return GridControl == null ? null : GridControl.FindForm(); }
		internal void ShowProgressForm(PrintingSystemActivity activity) {
#if DEBUGTEST
			if(DevExpress.XtraPrinting.Native.TestEnvironment.IsTestRunning())
				return;
#endif
			if(ProgressWindow != null) {
				if(printWrapper != null) ProgressWindow.SetCaption(activity);
				return;
			}
			if(!AllowPrintProgress) return;
			Form form = FindOwnerForm();
			if(form == null) return;
			ProgressWindow = new ProgressWindow();
			ProgressWindow.LookAndFeel.Assign(ElementsLookAndFeel);
			if(!OptionsPrint.AllowCancelPrintExport) ProgressWindow.DisableCancel();
			ProgressWindow.Cancel += new EventHandler(OnProgressWindowCancel);
			if(printWrapper != null)
				ProgressWindow.SetCaption(activity);
			ProgressWindow.ShowCenter(form);
		}
		void OnProgressWindowCancel(object sender, EventArgs e) {
			if(printWrapper != null) printWrapper.CancelPrint();
		}
		internal void OnPrintEnd(bool canceled) {
			this.printWrapper = null;
			ProgressWindow = null;
			if(AllowPrintProgress) Focus();
		}
		protected internal virtual void SetupChildSplitElements(BaseView master) { }
		protected internal virtual void SetupMasterSplitElements() { }
		protected internal virtual void RestoreMasterSplitElements() { }
		protected internal virtual void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) { }
		protected internal virtual GestureAllowArgs[] CheckAllowGestures(Point point) { return GestureAllowArgs.None; }
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(HtmlImages == null) return null;
			return HtmlImages.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return Site;
			return null;
		}
		#endregion
		internal class ChangeContext : IDisposable {
			BaseView from, to;
			static IDictionary<BaseView, ChangeContext> contexts =
				new Dictionary<BaseView, ChangeContext>();
			ChangeContext(BaseView from, BaseView to) {
				if(from != null)
					contexts.Add(from, this);
				if(to != null)
					contexts.Add(to, this);
				this.from = from;
				this.to = to;
			}
			public static IDisposable Enter(BaseView from, BaseView to) {
				return new ChangeContext(from, to);
			}
			public static bool IsChangingTo(BaseView view) {
				ChangeContext context;
				return contexts.TryGetValue(view, out context) && context.to == view;
			}
			public static bool IsChangingFrom(BaseView view) {
				ChangeContext context;
				return contexts.TryGetValue(view, out context) && context.from == view;
			}
			public void Dispose() {
				if(from != null)
					contexts.Remove(from);
				if(to != null)
					contexts.Remove(to);
				this.from = null;
				this.to = null;
			}
		}
		protected internal virtual void OnRepositoryItemRefreshRequired(RepositoryItem item) {
			LayoutChangedSynchronized();
		}
		protected internal virtual bool ValidateEditing() {
			return true;
		}
		protected internal virtual void OnLookAndFeelChanged() {
			CheckInfo();
		}
		bool ISkinProviderEx.GetTouchUI() {
			return LookAndFeel.GetTouchUI();
		}
		float ISkinProviderEx.GetTouchScaleFactor() {
			return LookAndFeel.GetTouchScaleFactor();
		}
		Color ISkinProviderEx.GetMaskColor() {
			return LookAndFeel.GetMaskColor();
		}
		Color ISkinProviderEx.GetMaskColor2() {
			return LookAndFeel.GetMaskColor2();
		}
		protected internal virtual void OnActionScroll(ScrollNotifyAction action) {
		}
		protected virtual bool CanActionScroll(ScrollNotifyAction action) {
			return true;
		}
		protected internal virtual Color BackColor { get { return SystemColors.Control; } }
		protected internal virtual bool IsAnyDetails { get { return false; } }
		protected internal void CheckRightToLeft() {
			if(this.rightToLeft == ControlRightToLeft) return;
			this.rightToLeft = ControlRightToLeft;
			OnRightToLeftChanged();
		}
		protected virtual void OnRightToLeftChanged() {
			ViewInfo.SetPaintAppearanceDirty();
			LayoutChangedSynchronized();
		}
		bool rightToLeft = false;
		protected internal bool IsRightToLeft { get { return rightToLeft; } }
		protected bool ControlRightToLeft { get { return GridControl != null && GridControl.IsRightToLeft && IsSupportRightToLeft; } }
		protected virtual bool IsSupportRightToLeft { get { return true; } }
	}
	public class ViewPrintOptionsBase : ViewBaseOptions {
		bool showPrintExportProgress, autoResetPrintDocument, allowCancelPrintExport;
		string rtfPageHeader, rtfPageFooter, rtfReportHeader, rtfReportFooter;
		public ViewPrintOptionsBase() {
			this.allowCancelPrintExport = true;
			this.autoResetPrintDocument = true;
			this.showPrintExportProgress = true;
			this.rtfPageHeader = string.Empty;
			this.rtfPageFooter = string.Empty;
			this.rtfReportHeader = string.Empty;
			this.rtfReportFooter = string.Empty;
		}
		[Editor("DevExpress.XtraPrinting.Design.PageHeaderEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.EmptyStringConverter)),
		DefaultValue(""), XtraSerializableProperty()]
		public virtual string RtfPageHeader {
			get { return rtfPageHeader; }
			set {
				if(RtfPageHeader == value) return;
				string prevValue = RtfPageHeader;
				rtfPageHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("InnerPageHeader", prevValue, RtfPageHeader));
			}
		}
		[Editor("DevExpress.XtraPrinting.Design.PageFooterEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.EmptyStringConverter)),
		DefaultValue(""), XtraSerializableProperty()]
		public virtual string RtfPageFooter {
			get { return rtfPageFooter; }
			set {
				if(RtfPageFooter == value) return;
				string prevValue = RtfPageFooter;
				rtfPageFooter = value;
				OnChanged(new BaseOptionChangedEventArgs("InnerPageFooter", prevValue, RtfPageFooter));
			}
		}
		[Editor("DevExpress.XtraPrinting.Design.ReportHeaderEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.EmptyStringConverter)),
		DefaultValue(""), XtraSerializableProperty()]
		public virtual string RtfReportHeader {
			get { return rtfReportHeader; }
			set {
				if(RtfReportHeader == value) return;
				string prevValue = RtfReportHeader;
				rtfReportHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("ReportHeader", prevValue, RtfReportHeader));
			}
		}
		[Editor("DevExpress.XtraPrinting.Design.ReportFooterEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.EmptyStringConverter)),
		DefaultValue(""), XtraSerializableProperty()]
		public virtual string RtfReportFooter {
			get { return rtfReportFooter; }
			set {
				if(RtfReportFooter == value) return;
				string prevValue = RtfReportFooter;
				rtfReportFooter = value;
				OnChanged(new BaseOptionChangedEventArgs("ReportFooter", prevValue, RtfReportFooter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ViewPrintOptionsBaseAutoResetPrintDocument"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoResetPrintDocument {
			get { return autoResetPrintDocument; }
			set {
				if(AutoResetPrintDocument == value) return;
				bool prevValue = AutoResetPrintDocument;
				autoResetPrintDocument = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoResetPrintDocument", prevValue, AutoResetPrintDocument));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ViewPrintOptionsBaseAllowCancelPrintExport"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowCancelPrintExport {
			get { return allowCancelPrintExport; }
			set {
				if(AllowCancelPrintExport == value) return;
				bool prevValue = AllowCancelPrintExport;
				allowCancelPrintExport = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowCancelPrintExport", prevValue, AllowCancelPrintExport));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ViewPrintOptionsBaseShowPrintExportProgress"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowPrintExportProgress {
			get { return showPrintExportProgress; }
			set {
				if(ShowPrintExportProgress == value) return;
				bool prevValue = ShowPrintExportProgress;
				showPrintExportProgress = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowPrintExportProgress", prevValue, ShowPrintExportProgress));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ViewPrintOptionsBase opt = options as ViewPrintOptionsBase;
				if(opt == null) return;
				this.allowCancelPrintExport = opt.AllowCancelPrintExport;
				this.autoResetPrintDocument = opt.AutoResetPrintDocument;
				this.showPrintExportProgress = opt.ShowPrintExportProgress;
				this.rtfPageHeader = opt.RtfPageHeader;
				this.rtfPageFooter = opt.RtfPageFooter;
				this.rtfReportHeader = opt.RtfReportHeader;
				this.rtfReportFooter = opt.RtfReportFooter;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ViewBaseOptions : BaseOptions {
		public ViewBaseOptions() { }
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
	}
	public class BaseViewAppearanceCollection : BaseAppearanceCollection {
		BaseView view;
		public BaseViewAppearanceCollection(BaseView view) {
			this.view = view;
		}
		public override string ToString() {
			return string.Empty;
		}
		protected BaseView View { get { return view; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("BaseViewAppearanceCollectionIsLoading")]
#endif
		public override bool IsLoading { get { return View.IsLoading; } }
		protected override AppearanceObject CreateNullAppearance() { return null; }
	}
}
