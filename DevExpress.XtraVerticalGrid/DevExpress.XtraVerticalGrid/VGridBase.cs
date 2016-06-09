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

using DevExpress.XtraVerticalGrid.Rows;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraVerticalGrid.Blending;
using DevExpress.XtraEditors.Container;
using System.Collections;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Serializing;
using System;
using System.Linq;
using DevExpress.XtraVerticalGrid.Data;
using System.IO;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Controls;
using System.Windows.Forms;
using DevExpress.XtraVerticalGrid.Events;
using System.ComponentModel.Design;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraVerticalGrid.Editors;
using DevExpress.Accessibility;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.XtraVerticalGrid.Internal;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraVerticalGrid {
	[Docking(DockingBehavior.Ask)]
	public abstract class VGridControlBase : DevExpress.XtraEditors.Container.EditorContainer, IXtraSerializable, IToolTipControlClient, ISupportLookAndFeel, IPrintable, IXtraSerializableLayout, IXtraSerializableLayoutEx, IGestureClient, ISupportXtraSerializer, IFilteredComponentColumnsClient {
		internal static VGridStore GS;
		int lockUpdate;
		int lockGridLayout;
		int lockLoadData;
		int lockPost;
		protected bool fGridDisposing;
		protected bool fFocusedRecordModified;
		VGridAppearanceCollection appearance;
		VGridRows rows;
		internal protected int fFocusedRecord;
		internal protected int fFocusedRecordCellIndex;
		int rowHeaderWidthChangeStep;
		int recordsInterval;
		int bandsInterval;
		WidthsManager widthsManager;
		Rectangle customizationFormBounds;
		object imageList;
		BaseHandler handler;
		VGridRowsIterator rowsIterator;
		VGridCustomizationForm customizationForm;
		UserLookAndFeel lookAndFeel, elementsLookAndFeel;
		VGridMenuBase menu;
		ErrorInfo errorInfo;
		GridRowReadOnlyCollection visibleRows;
		GridRowReadOnlyCollection fixedTopRows;
		GridRowReadOnlyCollection fixedBottomRows;
		GridRowReadOnlyCollection notFixedRows;
		BaseViewInfo viewInfo;
		VGridOptionsFind optionsFind;
		VGridOptionsView optionsView;
		VGridOptionsSelectionAndFocus optionsSelectionAndFocus;
		VGridOptionsBehavior optionsBehavior;
		VGridOptionsLayout optionsLayout;
		VGridOptionsHint optionsHint;
		protected LayoutViewStyle fLayoutStyle;
		TreeButtonStyle treeButtonStyle;
		ShowButtonModeEnum showButtonMode;
		BorderStyles borderStyle;
		protected VGridScroller fScroller;
		VGridPainter painter;
		XtraVertGridBlending blending;
		internal RowValueInfo drawnCell;
		internal VGridScrollBehavior scrollBehavior;
		AutoHeightsStore autoHeights;
		bool useDisabledStatePainter;
		BaseAccessible dxAccessible;
		VGridViewInfoHelper viewInfoHelper;
		protected internal int lastMouseUpX, lastMouseUpY; 
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		IDisposable componentPrinter;
		ComponentPrinterBase ComponentPrinter {
			get {
				if(componentPrinter == null)
					componentPrinter = new ComponentPrinter(this);
				return (ComponentPrinterBase)componentPrinter;
			}
		}
		FocusManager FocusManager { get; set; }
		#region Initing
		public VGridControlBase() {
			InitializeCore();
			this.lockUpdate = this.lockGridLayout = this.lockLoadData = 0;
			this.fGridDisposing = false;
			this.fFocusedRecordModified = false;
			DataManager = CreateDataManager();
			UniqueNames = new UniqueNameStore();
			FocusManager = new FocusManager(this);
			this.errorInfo = new ErrorInfoEx();
			this.errorInfo.Changed += new EventHandler(OnErrorInfo_Changed);
			this.fLayoutStyle = LayoutViewStyle.MultiRecordView;
			this.treeButtonStyle = TreeButtonStyle.Default;
			this.appearance = CreateAppearance();
			this.appearance.Changed += new EventHandler(Appearance_Changed);
			this.optionsLayout = CreateOptionsLayout();
			this.optionsView = CreateOptionsView();
			this.optionsSelectionAndFocus = CreateOptionsSelectionAndFocus();
			this.optionsSelectionAndFocus.Changed += OptionsSelectionAndFocusChanged;
			this.optionsView.Changed += new BaseOptionChangedEventHandler(OnOptionsViewChanged);
			this.optionsBehavior = CreateOptionsBehavior();
			this.optionsBehavior.Changed += new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
			this.optionsHint = CreateoptionsHint();
			this.optionsHint.Changed += new BaseOptionChangedEventHandler(OnOptionsHintChanged);
			this.optionsFind = CreateOptionsFind();
			this.optionsFind.Changed += OptionsFindChanged;
			this.rows = CreateRows();
			this.scrollBehavior = new VGridScrollBehavior();
			this.autoHeights = new AutoHeightsStore();
			this.customizationForm = null;
			this.blending = null;
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			this.elementsLookAndFeel = new DevExpress.LookAndFeel.Helpers.EmbeddedLookAndFeel();
			UpdateElementsLookAndFeel();
			CreatePainter();
			this.rowsIterator = CreateRowsIteratorCore();
			this.viewInfo = CreateViewInfo(false);
			this.handler = CreateHandler();
			this.fScroller = CreateScroller();
			this.drawnCell = null;
			this.showButtonMode = ShowButtonModeEnum.ShowForFocusedCell;
			this.borderStyle = BorderStyles.Default;
			this.fFocusedRecord = -1;
			this.fFocusedRecordCellIndex = 0;
			this.bandsInterval = 2;
			this.widthsManager = new WidthsManager(this);
			this.rowHeaderWidthChangeStep = 2;
			this.recordsInterval = 0;
			this.customizationFormBounds = VGridStore.DefaultCustomizationFormBounds;
			this.imageList = null;
			SetUpViewStyles();
			this.menu = CreateMenu();
			this.useDisabledStatePainter = true;
			this.viewInfoHelper = CreateViewInfoHelper();
		}
		protected virtual VGridDataManager CreateDataManager() {
			return new VGridDataManager();
		}
		void OptionsSelectionAndFocusChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == VGridOptionsSelectionAndFocus.EnableAppearanceFocusedRowName)
				InvalidateUpdate();
		}
		protected virtual VGridRows CreateRows() {
			return new VGridRows(this);
		}
		protected virtual void InitializeCore() {
		}
		static VGridControlBase() {
			InitializeGridStore();
		}
		private static void InitializeGridStore() {
			GS = VGridStore.Instance;
		}
		internal object lastKeyMessage = null;
		protected override void WndProc(ref Message m) {
			lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			if(DevExpress.XtraEditors.Senders.BaseSender.RequireShowEditor(ref m)) ShowEditor();
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper
		{
			get
			{
				if (gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		protected abstract VGridMenuBase CreateMenu();
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fGridDisposing = disposing;
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(LookAndFeel_StyleChanged);
					lookAndFeel.Dispose();
					lookAndFeel = null;
				}
				if(ElementsLookAndFeel != null) {
					ElementsLookAndFeel.Dispose();
					elementsLookAndFeel = null;
				}
				DestroyCustomization();
				Rows.DestroyRows();
				DestroyFindPanel();
				handler.SetControlState(VGridState.Disposed);
				if(viewInfo != null) {
					viewInfo.Dispose();
					viewInfo = null;
				}
				if(optionsView != null) {
					optionsView.Changed -= new BaseOptionChangedEventHandler(OnOptionsViewChanged);
					optionsView = null;
				}
				if(optionsBehavior != null) {
					optionsBehavior.Changed -= new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
					optionsBehavior = null;
				}
				if(componentPrinter != null) {
					componentPrinter.Dispose();
					componentPrinter = null;
				}
				if (optionsFind != null) {
					optionsFind.Changed -= OptionsFindChanged;
					optionsFind = null;
				}
				if (optionsSelectionAndFocus != null) {
					optionsSelectionAndFocus.Changed -= OptionsSelectionAndFocusChanged;
					optionsSelectionAndFocus = null;
				}
			}
			base.Dispose(disposing);
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinVGrid));
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			Scroller.CreateHandles();
		}
		protected override void OnLoaded() {
			if(IsInitialized) return;
			base.OnLoaded();
			OnLoadedCore();
		}
		protected virtual void OnLoadedCore() {
			lockLoadData++;
			BeginUpdate();
			try {
				viewInfo.UpdateCache();
				ActivateDataSource();
			}
			finally {
				lockLoadData--;
				EndUpdate();
			}
			FocusedRow = VisibleRows[0];
		}
		int fDataUpdateLocked;
		protected bool IsDataUpdateLocked { get { return fDataUpdateLocked != 0; } }
		internal bool IsUpdateLocked { get { return lockUpdate != 0; } }
		public virtual void BeginDataUpdate() {
			fDataUpdateLocked++;
			BeginUpdate();
		}
		public virtual void CancelDataUpdate() {
			fDataUpdateLocked--;
			CancelUpdate();
		}
		public virtual void EndDataUpdate() {
			CancelDataUpdate();
			UpdateData();
		}
		public virtual void UpdateData() {
			if(IsDataUpdateLocked)
				return;
			UpdateDataCore();
		}
		protected virtual void UpdateDataCore() {
			if(DataManager != null && !DataManager.IsDisposed)
				RePopulateColumns();
			UpdateCore();
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			ActivateDataSource();
		}
		protected virtual void ActivateDataSource() {
			if(IsLoading || IsDisposed)
				return;
			if (!IsDataSourceInitialized()) {
				SubscribeDataSourceInitializedEvent();
				return;
			}
			ActivateDataSourceInternal();
			LayoutChanged();
			SetFocusAfterDataSourceActivated();
		}
		protected virtual void SubscribeDataSourceInitializedEvent() {
		}
		protected virtual void UnsubscribeDataSourceInitializedEvent(object dataSource) {
		}
		protected virtual bool IsDataSourceInitialized() {
			return true;
		}
		protected virtual void ActivateDataSourceInternal() { }
		protected virtual void SetFocusAfterDataSourceActivated() {
			if(DataManager.CurrentControllerRow < 0 || DataManager.CurrentControllerRow > RecordCount - 1)
				FocusedRecord = Math.Min(0, RecordCount - 1);
			else
				FocusedRecord = DataManager.CurrentControllerRow;
			FocusedRecordCellIndex = (RecordCount == 0 ? -1 : 0);
		}
		protected virtual VGridScroller CreateScroller() { return new VGridScroller(this); }
		protected virtual BaseHandler CreateHandler() { return new BaseHandler(this); }
		protected virtual BaseViewInfo CreateViewInfo(bool isPrinting) {
			switch(LayoutStyle) {
				case LayoutViewStyle.BandsView:
					return new BandsViewInfo(this, isPrinting);
				case LayoutViewStyle.SingleRecordView:
					return new SingleRecordViewInfo(this, isPrinting);
				case LayoutViewStyle.MultiRecordView:
					return new MultiRecordViewInfo(this, isPrinting);
			}
			return null;
		}
		protected virtual VGridPainter CreatePainterCore(PaintEventHelper eventHelper) {
			return new VGridPainter(eventHelper);
		}
		protected virtual VGridRowsIterator CreateRowsIteratorCore() {
			return new VGridRowsIterator(this, true);
		}
		protected virtual GridRowReadOnlyCollection CreateVisibleRows() {
			return new GridRowReadOnlyCollection();
		}
		protected override EditorContainerHelper CreateHelper() {
			return new GridEditorContainerHelper(this);
		}
		protected virtual VGridOptionsLayout CreateOptionsLayout() {
			return new VGridOptionsLayout();
		}
		protected virtual VGridOptionsView CreateOptionsView() {
			return new VGridOptionsView();
		}
		protected virtual VGridOptionsSelectionAndFocus CreateOptionsSelectionAndFocus() {
			return new VGridOptionsSelectionAndFocus();
		}
		protected virtual VGridOptionsBehavior CreateOptionsBehavior() {
			return new VGridOptionsBehavior();
		}
		protected virtual VGridOptionsHint CreateoptionsHint() {
			return new VGridOptionsHint();
		}
		protected virtual VGridOptionsFind CreateOptionsFind() {
			return new VGridOptionsFind();
		}
		#endregion Initing
		#region Properties
		internal protected virtual IComparer VisibleRowsComparer { get { return new PGridRowComparer(false, false); } }
		internal protected AutoHeightsStore AutoHeights { get { return autoHeights; } }
		internal protected UniqueNameStore UniqueNames { get; private set; }
		protected VGridMenuBase Menu { get { return menu; } }
		string filterString;
		CriteriaOperator filterCriteria;
		[DefaultValue(null)]
		public CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				if(object.ReferenceEquals(filterCriteria, value))
					return;
				filterCriteria = value;
				FilterCriteriaChanged();
			}
		}
		[DefaultValue(null)]
		public string FilterString {
			get { return filterString; }
			set {
				if(filterString == value)
					return;
				filterString = value;
				FilterStringChanged();
			}
		}
		int IsEditorLocked { get; set; }
		internal void LockEditor() {
			IsEditorLocked++;
		}
		internal void UnlockEditor() {
			IsEditorLocked--;
		}
		protected virtual void FilterStringChanged() {
			FilterCriteria = CriteriaOperator.TryParse(FilterString);
		}
		protected virtual void FilterCriteriaChanged() {
			DataModeHelper.FilterChanged(this);
		}
		[Browsable(false)]
		public bool GridDisposing { get { return fGridDisposing; } }
		[Browsable(false)]
		public virtual bool CanShowEditor {
			get {
				if(State != VGridState.Regular && State != VGridState.Editing)
					return false;
				if(State == VGridState.Editing && ActiveEditor != null)
					return false;
				if(!OptionsBehavior.Editable) return false;
				if(FocusedRow == null) return false;
				if(FocusedRow is EditorRow && !((EditorRow)FocusedRow).Enabled) return false;
				RepositoryItem rowEdit = GetRowEdit(FocusedRow.GetRowProperties(FocusedRecordCellIndex));
				if(rowEdit != null && !rowEdit.Editable) return false;
				RowProperties p = FocusedRow.GetRowProperties(FocusedRecordCellIndex);
				if(p == null || !p.Bindable) return false;
				bool cancel = false;
				RaiseShowingEditor(ref cancel);
				return !cancel;
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(VGridOptionsLayout.AppearanceLayoutId)]
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public VGridAppearanceCollection Appearance { get { return appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowReadOnlyCollection VisibleRows {
			get {
				if(visibleRows == null) {
					UpdateVisibleRows();
				}
				return visibleRows;
			}
		}
		protected internal void ResetVisibleRows() {
			visibleRows = null;
			fixedTopRows = null;
			fixedBottomRows = null;
			notFixedRows = null;
			if(IsEditorLocked == 0) {
				HideEditor();
			}
			InvalidateUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowReadOnlyCollection FixedTopRows {
			get {
				if(fixedTopRows == null) {
					fixedTopRows = GetFixedRowList(FixedStyle.Top);
				}
				return fixedTopRows;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowReadOnlyCollection FixedBottomRows {
			get {
				if(fixedBottomRows == null) {
					fixedBottomRows = GetFixedRowList(FixedStyle.Bottom);
				}
				return fixedBottomRows;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridRowReadOnlyCollection NotFixedRows {
			get {
				if(notFixedRows == null) {
					notFixedRows = GetFixedRowList(FixedStyle.None);
				}
				return notFixedRows;
			}
		}
		[Browsable(false)]
		public int BandCount {
			get { return fScroller.BandsInfo.Count; }
		}
		[Browsable(false)]
		public int BandWidth {
			get { return RowHeaderWidth + RecordWidth; }
		}
		[Browsable(false)]
		public virtual int BandMinWidth {
			get { return RowHeaderMinWidth + 25; }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseBandsInterval"),
#endif
 DefaultValue(2), Category("Appearance"), XtraSerializableProperty(), Localizable(true)]
		public int BandsInterval {
			get { return bandsInterval; }
			set {
				if(value > 30) value = 30;
				if(value < 0) value = 0;
				if(BandsInterval != value) {
					bandsInterval = value;
					if(LayoutStyle == LayoutViewStyle.BandsView)
						InvalidateUpdate();
				}
			}
		}
		WidthsManager Widths { get { return widthsManager; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRowHeaderWidth"),
#endif
		DefaultValue(100),
		Category("Appearance"),
		XtraSerializableProperty(),
		Localizable(true),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public int RowHeaderWidth { get { return Widths.HeaderWidth; } set { Widths.HeaderWidth = value; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRowHeaderWidthChangeStep"),
#endif
 DefaultValue(2), Category("Behavior"), XtraSerializableProperty(), Localizable(true)]
		public int RowHeaderWidthChangeStep {
			get { return rowHeaderWidthChangeStep; }
			set {
				if(value < 1) value = 1;
				if(value > 10) value = 10;
				if(RowHeaderWidthChangeStep != value) {
					rowHeaderWidthChangeStep = value;
				}
			}
		}
		[Obsolete("Use the OptionsView.MaxRowAutoHeight property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MaxRowAutoHeight {
			get { return OptionsView.MaxRowAutoHeight; }
			set { OptionsView.MaxRowAutoHeight = value; }
		}
		public void BestFit() {
			BeginUpdate();
			UpdateLayout();
			Scroller.Strategy.GetVisibleCountInternal(VisibleRows, 0, int.MaxValue, 1, i => i < VisibleRows.Count);
			CalculateBestWidth bestWidth = new CalculateBestWidth();
			RowsIterator.DoOperation(bestWidth);
			RowHeaderWidth = ScaleRowHeaderWidth(bestWidth.MaxHeaderWidth);
			EndUpdate();
		}
		internal int ScaleRowHeaderWidth(float value) {
			if(IsAutoSize)
				return (int)Math.Ceiling((value * (float)BandWidth / (float)ViewInfo.ViewRects.Client.Width));
			return (int)value;
		}
		[Browsable(false)]
		public virtual int RowHeaderMinWidth { get { return 15; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRecordWidth"),
#endif
		DefaultValue(100),
		Category("Appearance"),
		XtraSerializableProperty(),
		Localizable(true),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public int RecordWidth { get { return Widths.RecordWidth; } set { Widths.RecordWidth = value; } }
		[Browsable(false)]
		public virtual int RecordMinWidth { get { return 15; } }
		[Browsable(false)]
		public virtual bool FocusedRecordModified { get { return fFocusedRecordModified; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(true, true, true)]
		public VGridRows Rows { get { return rows; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected internal UserLookAndFeel ElementsLookAndFeel { get { return elementsLookAndFeel; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseRow FocusedRow { get { return FocusManager.FocusedRow; } set { FocusManager.FocusedRow = value; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseBorderStyle"),
#endif
 Category("Appearance"), DefaultValue(BorderStyles.Default), XtraSerializableProperty(), Localizable(true)]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseShowButtonMode"),
#endif
 DefaultValue(ShowButtonModeEnum.ShowForFocusedCell),
		Category("Appearance"), XtraSerializableProperty(), Localizable(true)]
		public ShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(ShowButtonMode != value) {
					showButtonMode = value;
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Auto), Category("Layout"), XtraSerializableProperty(),
		Localizable(true)]
		public ScrollVisibility ScrollVisibility {
			get { return scrollBehavior.ScrollVisibility; }
			set {
				if(scrollBehavior.ScrollVisibility != value) {
					scrollBehavior.ScrollVisibility = value;
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseScrollsStyle"),
#endif
 TypeConverter(typeof(ExpandableObjectConverter)), Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public VGridScrollStylesController ScrollsStyle {
			get { return fScroller.StylesController; }
		}
		internal protected LayoutViewStyle LayoutStyle {
			get { return fLayoutStyle; }
			set {
				if(LayoutStyle != value) {
					fLayoutStyle = value;
					LayoutViewStyleChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseTreeButtonStyle"),
#endif
 DefaultValue(TreeButtonStyle.Default), Category("Appearance"), Localizable(true), XtraSerializableProperty()]
		public TreeButtonStyle TreeButtonStyle {
			get { return treeButtonStyle; }
			set {
				if(TreeButtonStyle != value) {
					treeButtonStyle = value;
					LayoutChanged();
					InvalidateCustomizationForm();
				}
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseImageList"),
#endif
 Category("Appearance"), DefaultValue(null), Localizable(true),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object ImageList {
			get { return imageList; }
			set {
				if(ImageList != value) {
					imageList = value;
					viewInfo.RC.NeedsUpdate = true;
					LayoutChanged();
				}
			}
		}
		[Browsable(false)]
		public DevExpress.XtraEditors.BaseEdit ActiveEditor { get { return ContainerHelper.ActiveEditor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object EditingValue {
			get { return ContainerHelper.EditingValue; }
			set { ContainerHelper.EditingValue = value; }
		}
		bool ShouldSerializeOptionsFind() { return OptionsFind.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsFind"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsFind OptionsFind { get { return optionsFind; } }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsView"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsView OptionsView { get { return optionsView; } }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsBehavior"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		bool ShouldSerializeOptionsSelectionAndFocus() { return OptionsSelectionAndFocus.ShouldSerializeCore(this); }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsSelectionAndFocus")]
#endif
		[Category("Options")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsSelectionAndFocus OptionsSelectionAndFocus { get { return optionsSelectionAndFocus; } }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsLayout"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public VGridOptionsLayout OptionsLayout { get { return optionsLayout; } }
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerializeCore(this); }
		[Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsMenu")
#else
	Description("")
#endif
]
		public VGridOptionsMenu OptionsMenu { get { return Menu.Options; } }
		bool ShouldSerializeOptionsHint() { return OptionsHint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseOptionsHint"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public VGridOptionsHint OptionsHint { get { return optionsHint; } }
		[Browsable(false)]
		public int RecordCount { get { return DataManager.RecordCount; } }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRecordsInterval"),
#endif
 DefaultValue(0), Category("Appearance"), XtraSerializableProperty()]
		public int RecordsInterval {
			get { return recordsInterval; }
			set {
				if(value > 50) value = 50;
				if(value < 0) value = 0;
				if(RecordsInterval != value) {
					recordsInterval = value;
					if(LayoutStyle == LayoutViewStyle.MultiRecordView)
						LayoutChanged();
				}
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use 'OptionsLayout.LayoutVersion' instead")]
		public virtual string LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
			set { OptionsLayout.LayoutVersion = value; }
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseUseDisabledStatePainter"),
#endif
 Category("Appearance")]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainter; }
			set {
				if(useDisabledStatePainter != value) {
					useDisabledStatePainter = value;
					Invalidate();
				}
			}
		}
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		[Browsable(false)]
		public VGridRowsIterator RowsIterator { get { return rowsIterator; } }
		[Browsable(false)]
		public VGridCustomizationForm CustomizationForm { get { return customizationForm; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FocusedRecord {
			get { return fFocusedRecord; }
			set {
				value = DataModeHelper.CheckValidRecord(value);
				if(FocusedRecord != value && CanFocusRecord(value)) {
					ChangeFocusedRecord(value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FocusedRecordCellIndex {
			get { return fFocusedRecordCellIndex; }
			set {
				int newFocusedRecord = DataModeHelper.Position;
				if(value < 0) {
					if(DataModeHelper.Position == 0) value = 0;
					else {
						newFocusedRecord = Math.Max(0, DataModeHelper.Position - 1);
						value = CurrentNumCells - 1;
					}
				}
				else if(value >= CurrentNumCells && value > 0) {
					if(DataModeHelper.Position == RecordCount - 1) value = CurrentNumCells - 1;
					else {
						newFocusedRecord = Math.Min(DataModeHelper.Position + 1, RecordCount - 1);
						value = 0;
					}
				}
				FocusedRecord = newFocusedRecord;
				if(FocusedRecord == DataModeHelper.CheckValidRecord(newFocusedRecord) && FocusedRecordCellIndex != value) {
					IndexChangedEventArgs e = new IndexChangedEventArgs(value, FocusedRecordCellIndex);
					this.fFocusedRecordCellIndex = value;
					CloseEditor();
					InvalidateRow(FocusedRow);
					RaiseFocusedRecordCellChanged(e);
				}
			}
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopVisibleRowIndex {
			get { return fScroller.TopVisibleRowIndex; }
			set { fScroller.TopVisibleRowIndex = value; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopVisibleRowIndexPixel {
			get { return fScroller.TopVisibleRowIndexPixel; }
			set { fScroller.TopVisibleRowIndexPixel = value; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftVisibleRecord {
			get { return fScroller.LeftVisibleRecord; }
			set { fScroller.LeftVisibleRecord = value; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftVisibleRecordPixel {
			get { return fScroller.LeftVisibleRecordPixel; }
			set { fScroller.LeftVisibleRecordPixel = value; }
		}
		[Browsable(false)]
		[Obsolete("This property is obsolete.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftVisibleBand { get; set; }
		[Browsable(false)]
		public BaseViewInfo ViewInfo { get { return viewInfo; } }
		[Browsable(false)]
		public VGridState State { get { return handler.State; } }
		internal BaseHandler Handler { get { return handler; } }
		protected internal virtual VGridDataManager DataManager { get; protected set; }
		protected internal DataModeHelper DataModeHelper { get { return DataManager.DataModeHelper; } }
		protected internal GridEditorContainerHelper ContainerHelper { get { return base.EditorHelper as GridEditorContainerHelper; } }
		protected internal VGridPainter Painter { get { return painter; } }
		internal int CurrentNumCells {
			get {
				if(FocusedRow == null) return 0;
				if(FocusedRow is MultiEditorRow)
					return ((MultiEditorRow)FocusedRow).PropertiesCollection.Count;
				return 1;
			}
		}
		internal virtual bool IsAutoSize {
			get {
				if(LayoutStyle == LayoutViewStyle.BandsView) return OptionsView.AutoScaleBands;
				return (LayoutStyle == LayoutViewStyle.SingleRecordView);
			}
		}
		bool IsPostingEditor { get { return this.lockPost != 0; } }
		bool Unloading {
			get {
				if(GridDisposing) return true;
				if(DesignMode) {
					IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(host != null) return host.Loading;
				}
				return false;
			}
		}
		protected internal VGridScroller Scroller { get { return fScroller; } }
		internal virtual bool EditingValueModified {
			get {
				if(ActiveEditor != null) return ActiveEditor.IsModified;
				return false;
			}
		}
		internal bool IsAutoScaleBands {
			get { return OptionsView.AutoScaleBands; }
		}
		internal bool HasFocus {
			get { return ContainsFocus || ContainerHelper.InternalFocusLock != 0; }
		}
		internal PaintStyleParams PaintStyleParams { get { return Painter.PaintStyle.GetPaintStyleParams(); } }
		internal XtraVertGridBlending Blending { get { return blending; } set { blending = value; } }
		internal bool CanUpdatePaintAppearanceBlending { get { return (!DesignMode && Blending != null && Blending.Enabled); } }
		#endregion Properties
		#region IXtraSerializable
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			RowXtraDeserializer.StartDeserializing(Rows);
			BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			try {
				if(restoredVersion != OptionsLayout.LayoutVersion)
					RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
			}
			finally {
				RowXtraDeserializer.EndDeserializing(this);
				EndInit();
			}
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, OptionsLayout);
		}
		public virtual void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, OptionsLayout);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile, options);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, OptionsLayout);
		}
		public virtual void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, OptionsLayout);
		}
		public virtual void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, OptionsLayout);
		}
		public virtual void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			SaveLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, OptionsLayout);
		}
		public virtual void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream, options);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(this, stream, this.GetType().Name, options);
			else
				serializer.SerializeObject(this, path.ToString(), this.GetType().Name, options);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			DesignerTransaction trans = null;
			if(host != null)
				trans = host.CreateTransaction(GetType().Name + "Layout");
			BeginUpdate();
			try {
				ResetVisibleRows();
				Stream stream = path as Stream;
				if(stream != null)
					serializer.DeserializeObject(this, stream, this.GetType().Name, options);
				else
					serializer.DeserializeObject(this, path.ToString(), this.GetType().Name, options);
				if(trans != null)
					trans.Commit();
			}
			catch {
				if(trans != null)
					trans.Cancel();
				throw;
			}
			finally {
				EndUpdate();
			}
		}
		#endregion IXtraSerializable
		#region IXtraSerializableLayoutEx Members
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			VGridOptionsLayout vGridOptions = options as VGridOptionsLayout;
			if(vGridOptions != null && vGridOptions.StoreAppearance) {
				Appearance.Reset();
			}
			if(options == OptionsLayoutBase.FullLayout || vGridOptions != null) {
				ResetProperties();
			}
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			VGridOptionsLayout vGridOptions = options as VGridOptionsLayout;
			if(vGridOptions == null)
				return true;
			if(id == VGridOptionsLayout.AppearanceLayoutId)
				return vGridOptions.StoreAppearance;
			return true;
		}
		void ResetProperties() {
			OptionsView.Reset();
			OptionsBehavior.Reset();
			OptionsMenu.Reset();
		}
		#endregion IXtraSerializableLayoutEx Members
		#region IToolTipControlClient
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) { return handler.GetObjectTipInfo(point); }
		#endregion IToolTipControlClient
		#region Styles
		public void RestoreDefaultStyles() {
			BeginUpdate();
			try {
				Appearance.Reset();
				ViewInfo.SetAppearanceDirty();
				ViewInfo.RC.NeedsUpdate = true;
			}
			finally {
				EndUpdate();
			}
		}
		private void SetUpViewStyles() {
			SetUpControlStyles();
			BeginUpdate();
			try {
				RestoreDefaultStyles();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual VGridAppearanceCollection CreateAppearance() { return new VGridAppearanceCollection(this); }
		private void SetUpControlStyles() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
#if DXWhidbey
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
#else
			SetStyle(ControlStyles.DoubleBuffer, true);
#endif
		}
		protected virtual void OnStyleChanged(object sender, EventArgs e) {
			FireChanged();
			viewInfo.RC.NeedsUpdate = true;
			LayoutChanged();
			if(!IsUpdateLocked) {
				InvalidateCustomizationForm();
			}
		}
		protected internal void ClearAutoHeights() {
			AutoHeights.Clear();
		}
		public void FireChanged() {
			if(GridDisposing || lockLoadData != 0) return;
			DevExpress.XtraVerticalGrid.Utils.DesignUtils.FireChanged(this, IsLoading, DesignMode, GetService(typeof(IComponentChangeService)) as IComponentChangeService);
		}
		private void ViewStyles_Changed(object value, EventArgs e) {
			ViewInfo.RC.NeedsUpdate = true;
			FireChanged();
			LayoutChanged();
		}
		void Appearance_Changed(object sender, EventArgs e) {
			ViewInfo.SetAppearanceDirty();
			OnStyleChanged(sender, e);
		}
		protected virtual void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			UpdateElementsLookAndFeel();
			fScroller.UpdateStyle();
			ViewInfo.RC.NeedsUpdate = true;
			ViewInfo.SetAppearanceDirty();
			ViewInfo.UpdateCalcHelper();
			LayoutChanged();
			if(CustomizationForm != null) {
				CustomizationForm.UpdateStyle();
				InvalidateCustomizationForm();
			}
		}
		protected virtual void UpdateElementsLookAndFeel() {
			ElementsLookAndFeel.Assign(LookAndFeel.ActiveLookAndFeel);
			ElementsLookAndFeel.UseDefaultLookAndFeel = false;
		}
		#endregion Styles
		#region Editors
		public void ShowEditor() {
			if(DataModeHelper.Position < 0 || DataModeHelper.Position > RecordCount - 1)
				return;
			if(!CanShowEditor)
				return;
			MakeRecordVisible(DataModeHelper.Position);
			InternalMakeRowVisible(FocusedRow, FocusedRow);
			UpdateLayout();
			RowValueInfo rv = ViewInfo.GetRowValueInfo(FocusedRow, DataModeHelper.Position, fFocusedRecordCellIndex);
			if(rv != null) {
				ActivateEditor(rv);
			}
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			Appearance.UpdateRightToLeft(ViewInfo.IsRightToLeft);
			ViewInfo.SetAppearanceDirty();
			Refresh();
		}
		private void ActivateEditor(RowValueInfo cell) {
			if(cell == null) return;
			Rectangle r = Rectangle.Intersect(cell.EditorViewInfo.Bounds, viewInfo.GetVisibleValuesRect());
			if(!(r.Width > 0 || r.Height == ViewInfo.GetVisibleRowHeight(FocusedRow))) return;
			Scroller.OnAction(ScrollNotifyAction.Hide);
			AppearanceObject appearance = GetEditStyle(cell);
			ContainerHelper.ActivateEditor(cell, new UpdateEditorInfoArgs(ViewInfoHelper.IsReadOnly(cell, this), r, appearance, cell.EditorViewInfo.EditValue, ElementsLookAndFeel, cell.EditorViewInfo.ErrorIconText, cell.EditorViewInfo.ErrorIcon, ViewInfo.IsRightToLeft));
			if(ActiveEditor != null) {
				handler.SetControlState(VGridState.Editing);
				RaiseShownEditor();
			}
		}
		int closingEditor = 0;
		protected internal bool IsClosingEditor { get { return closingEditor != 0; } }
		protected virtual void CloseEditor(bool causeValidation) {
			if(Disposing || IsClosingEditor || IsPostingEditor) return;
			this.closingEditor++;
			try {
				PostEditor(causeValidation);
				HideEditor();
			}
			finally {
				this.closingEditor--;
			}
		}
		public virtual void CloseEditor() {
			CloseEditor(true);
		}
		protected override void OnValidating(CancelEventArgs e) {
			if(GridDisposing)
				return;
			try {
				ContainerHelper.BeginAllowHideException();
				if(IsClosingEditor) return;
				if(!ContainerHelper.ValidateEditor(this)) {
					e.Cancel = true;
				}
				else {
					CloseEditor(false);
					e.Cancel = !UpdateFocusedRecord(true);
				}
			}
			catch(HideException) {
				e.Cancel = true;
			}
			finally {
				ContainerHelper.EndAllowHideException();
			}
			base.OnValidating(e);
		}
		public virtual void HideEditor() {
			if(ActiveEditor == null)
				return;
			ContainerHelper.DeactivateEditor();
			if(FocusedRow != null && !FocusedRow.IsLoadingCore) {
				InvalidateRow(FocusedRow);
				InvalidateRecord(FocusedRecord);
			}
			RaiseHiddenEditor();
			handler.SetControlState(VGridState.Regular);
		}
		protected virtual void OnCellValueChanged(RowProperties props, int recordIndex, object value) {
			int cellIndex = props.CellIndex;
			RaiseCellValueChanged(new CellValueChangedEventArgs(props.Row, recordIndex, cellIndex, value));
			LayoutChangedCore();
		}
		protected virtual bool PostEditor(bool causeValidation) {
			if(ActiveEditor == null || !EditingValueModified || IsPostingEditor)
				return true;
			bool post = PostEditorCore(causeValidation);
			if(post) {
				ProcessEditorAfterPost();
			}
			return post;
		}
		public virtual bool PostEditor() {
			return PostEditor(true);
		}
		protected virtual bool PostEditorCore(bool causeValidation) {
			if(causeValidation && !ContainerHelper.ValidateEditor(this))
				return false;
			if(FocusedRow == null || ContainerHelper.BreakPostOnEquals()) {
				return true;
			}
			object edValue = EditingValue;
			try {
				this.lockPost++;
				SetCellValueCore(FocusedRow.GetRowProperties(FocusedRecordCellIndex), FocusedRecord, edValue, true);
			}
			catch(Exception e) {
				ContainerHelper.OnInvalidValueException(this, new EditorValueException(e, e.Message), edValue);
				return false;
			}
			finally {
				this.lockPost--;
			}
			return true;
		}
		protected internal virtual BaseEditViewInfo CreateEditorViewInfo(RowProperties properties, int recordIndex) {
			RepositoryItem item = GetRowEdit(properties, recordIndex);
			BaseEditViewInfo editViewInfo = (BaseEditViewInfo)item.CreateViewInfo();
			UpdateEditViewInfo(properties, editViewInfo, recordIndex);
			return editViewInfo;
		}
		protected internal virtual void UpdateEditViewInfo(RowProperties properties, BaseEditViewInfo editViewInfo, int recordIndex) {
			if(properties == null || editViewInfo == null)
				return;
			if(editViewInfo.LookAndFeel == null || (editViewInfo.LookAndFeel != ElementsLookAndFeel && editViewInfo.LookAndFeel.UseDefaultLookAndFeel)) {
				editViewInfo.LookAndFeel = ElementsLookAndFeel;
			}
			editViewInfo.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			editViewInfo.AllowDrawFocusRect = false;
			editViewInfo.InplaceType = InplaceType.Grid;
			if(properties.Format.FormatType != FormatType.None)
				editViewInfo.Format = properties.Format;
			editViewInfo.RightToLeft = ViewInfo.IsRightToLeft;
		}
		protected virtual void ProcessEditorAfterPost() {
			if(ActiveEditor != null) {
				ActiveEditor.IsModified = false;
				ActiveEditor.ErrorText = "";
				EditorHelper.RealToolTipController.HideHint();
			}
		}
		internal RepositoryItem GetCustomRowEdit(RowProperties properties, int recordIndex, RepositoryItem item) {
			GetCustomRowCellEditEventArgs e = new GetCustomRowCellEditEventArgs(properties.Row, recordIndex, properties.CellIndex, item);
			BeginUpdate();
			bool readOnly = e.RepositoryItem.ReadOnly;
			RaiseCustomRecordCellEdit(e);
			if(readOnly != e.RepositoryItem.ReadOnly)
				properties.RenderReadOnly = e.RepositoryItem.ReadOnly;
			CancelUpdate();
			return e.RepositoryItem;
		}
		internal RepositoryItem GetCustomRowEditForEditing(RowProperties properties, int recordIndex, RepositoryItem item) {
			GetCustomRowCellEditEventArgs e = new GetCustomRowCellEditEventArgs(properties.Row, recordIndex, properties.CellIndex, item);
			BeginUpdate();
			RaiseCustomRecordCellEditForEditing(e);
			CancelUpdate();
			return e.RepositoryItem;
		}
		protected internal virtual RepositoryItem CreateDefaultRowEdit(RowProperties p) {
			return ContainerHelper.DefaultRepository.GetRepositoryItem(p.RowType);
		}
		protected internal RepositoryItem GetRowEdit(RowProperties properties) {
			return GetRowEdit(properties, FocusedRecord);
		}
		protected internal virtual RepositoryItem GetRowEdit(RowProperties properties, int recordIndex) {
			if(properties == null)
				return (RepositoryItem)ContainerHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			GridCell cell = new GridCell(properties.Row, properties.CellIndex, recordIndex);
			RepositoryItem item;
			if(!ViewInfo.RepositoryItemCache.TryGetValue(cell, out item)) {
				item = CreateRowEdit(properties);
				ConfigureRowEdit(item, properties);
				item = GetCustomRowEdit(properties, recordIndex, item);
				ViewInfo.RepositoryItemCache.Add(cell, item);
			}
			return item;
		}
		protected virtual RepositoryItem CreateRowEdit(RowProperties p) {
			if(p.RowEdit != null)
				return p.RowEdit;
			return CreateDefaultRowEdit(p);
		}
		protected virtual void ConfigureRowEdit(RepositoryItem item, RowProperties p) {
		}
		internal protected void EditorsRepositoryChanged() {
			InvalidateUpdate();
		}
		internal protected virtual void EditorsPropertiesChanged(RepositoryItem item) {
			InvalidateUpdate();
		}
		private AppearanceObject GetEditStyle(RowValueInfo cell) {
			AppearanceObject result = new AppearanceObject();
			result.Assign(cell.Style);
			return result;
		}
		internal protected virtual void OnActiveEditor_ValueChanging(object sender, ChangingEventArgs e) {
			RaiseCellValueChanging(new CellValueChangedEventArgs(FocusedRow, FocusedRecord, fFocusedRecordCellIndex, e.NewValue));
		}
		internal protected virtual void OnActiveEditor_LostFocus(object sender, EventArgs e) {
			if(ContainerHelper.InternalFocusLock != 0 || (ActiveEditor != null && ActiveEditor.EditorContainsFocus) || ActiveEditor is PGPopupContainerEdit) return;
			OnLostFocus(e);
			if(!IsClosingEditor)
				PostEditor(true);
		}
		#endregion Editors
		#region Data
		protected virtual string ColumnSeparator { get { return "\t"; } }
		public virtual void CopyToClipboard() {
			DataObject data = new DataObject();
			data.SetData(typeof(string), GetSelectedText());
			try {
				Clipboard.SetDataObject(data);
			}
			catch { }
		}
		internal protected virtual string GetSelectedText() {
			return GetRowText(FocusedRow);
		}
		string GetRowText(BaseRow row) {
			if(row == null) return string.Empty;
			StringBuilder sb = new StringBuilder();
			if(OptionsBehavior.CopyToClipboardWithRowHeaders) {
				row.AppendHeaderText(sb);
				sb.Append(ColumnSeparator);
			}
			row.AppendValueText(sb, FocusedRecord);
			return sb.ToString();
		}
		internal protected virtual object GetSelectedData() {
			return GetRowData(FocusedRow);
		}
		object GetRowData(BaseRow row) {
			if(row == null) return null;
			return row.GetData(FocusedRecord);
		}
		public virtual void AddNewRecord() {
			HideEditor();
			if(State != VGridState.Regular)
				return;
			DataModeHelper.AddNewRecord();
			fFocusedRecordModified = true;
		}
		public virtual void DeleteRecord(int recordIndex) {
			DataModeHelper.DeleteRecord(recordIndex);
		}
		public virtual object GetRecordObject(int recordIndex) {
			return DataManager.GetRow(recordIndex);
		}
		protected internal object GetCurrentCellValue(BaseRow gridRow) {
			return GetCellValue(gridRow, FocusedRecord);
		}
		public virtual bool IsCellDefaultValue(BaseRow row, int recordIndex) {
			if(row == null) return false;
			return IsCellDefaultValue(row.Properties, recordIndex);
		}
		public virtual object GetCellValue(BaseRow row, int recordIndex) {
			if(row == null) return null;
			return GetCellValue(row.Properties, recordIndex);
		}
		public virtual bool IsCellDefaultValue(RowProperties props, int recordIndex) {
			return DataModeHelper.IsCellDefaultValue(props, recordIndex);
		}
		public virtual object GetCellValue(RowProperties props, int recordIndex) {
			return DataModeHelper.GetCellValue(props, recordIndex);
		}
		public virtual object GetCellValue(MultiEditorRow meRow, int recordIndex, int cellIndex) {
			if(meRow == null) return null;
			return GetCellValue(meRow.PropertiesCollection[cellIndex], recordIndex);
		}
		public string GetCellDisplayText(RowProperties rowProperties, int recordIndex) {
			return GetCellDisplayTextCore(rowProperties, recordIndex);
		}
		public string GetCellDisplayText(BaseRow row, int recordIndex) {
			return GetCellDisplayTextCore(row.Properties, recordIndex);
		}
		public string GetCellDisplayText(MultiEditorRow meRow, int recordIndex, int cellIndex) {
			RowProperties properties = meRow.PropertiesCollection[cellIndex];
			return GetCellDisplayTextCore(properties, recordIndex);
		}
		internal protected virtual string GetCellDisplayTextCore(RowProperties properties, int recordIndex) {
			if(properties == null)
				return null;
			BaseEditViewInfo editViewInfo = CreateEditorViewInfo(properties, recordIndex);
			UpdateEditViewInfoData(editViewInfo, properties, recordIndex);
			editViewInfo.CalcViewInfo(null);
			return editViewInfo.DisplayText;
		}
		protected internal virtual void UpdateEditViewInfoData(BaseEditViewInfo editViewInfo, RowProperties properties, int recordIndex) {
			if(editViewInfo == null)
				return;
			editViewInfo.EditValue = GetCellValue(properties, recordIndex);
		}
		public virtual void SetCellValue(BaseRow gridRow, int recordIndex, object value) {
			if(gridRow == null) return;
			SetCellValue(gridRow.Properties, recordIndex, value);
		}
		public virtual void SetCellValue(MultiEditorRow meRow, int recordIndex, int cellIndex, object value) {
			if(meRow == null) return;
			SetCellValue(meRow.PropertiesCollection[cellIndex], recordIndex, value);
		}
		public virtual void SetCellValue(RowProperties props, int recordIndex, object value) {
			try {
				SetCellValueCore(props, recordIndex, value, false);
			}
			catch(Exception e) {
				ContainerHelper.OnInvalidValueException(this, e, value);
			}
		}
		protected virtual void SetCellValueCore(RowProperties props, int recordIndex, object value, bool endCurrentEdit) {
			DataModeHelper.SetCellValue(props, recordIndex, value, endCurrentEdit);
			if(this.fFocusedRecord == recordIndex)
				this.fFocusedRecordModified = true;
			OnCellValueChanged(props, recordIndex, value);
		}
		protected internal virtual string GetRowError(RowProperties props, int recordDisplayIndex) {
			if(DataModeHelper.GetCorrectRecordIndex(recordDisplayIndex) == FocusedRecord) return GetRowError(props);
			return GetRowDataError(props, recordDisplayIndex);
		}
		protected internal virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetRowErrorType(RowProperties props, int recordIndex) {
			if(DataModeHelper.GetCorrectRecordIndex(recordIndex) == FocusedRecord) return GetRowErrorType(props);
			return GetRowDataErrorType(props, recordIndex);
		}
		internal DevExpress.XtraEditors.DXErrorProvider.ErrorInfo GetRowErrorInfo(RowProperties props, int recordDisplayIndex) {
			string errorText = GetRowError(props, recordDisplayIndex);
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
			if(RowValueInfo.IsValidErrorIconTextInternal(errorText)) {
				errorType = GetRowErrorType(props, recordDisplayIndex);
				if(errorType == DevExpress.XtraEditors.DXErrorProvider.ErrorType.None)
					errorText = null;
			}
			return new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo(errorText, errorType);
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetRowErrorType(RowProperties props) {
			if(FocusedRecord == -1) return DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			if(info[props] != null && info[props].Length > 0) return info.GetErrorType(props);
			return GetRowDataErrorType(props, FocusedRecord);
		}
		public virtual string GetRowError(RowProperties props) {
			if(FocusedRecord == -1) return null;
			string result = ErrorInfo[props];
			if(result != null && result.Length > 0) return result;
			return GetRowDataError(props, FocusedRecord);
		}
		protected virtual string GetRowDataError(RowProperties props, int recordIndex) {
			return DataModeHelper.GetRowDataError(props, recordIndex);
		}
		protected virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetRowDataErrorType(RowProperties props, int recordIndex) {
			return DataModeHelper.GetRowDataErrorType(props, recordIndex);
		}
		public virtual void SetRowError(RowProperties props, string errorText) {
			SetRowError(props, errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
		}
		public virtual void SetRowError(RowProperties props, string errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType) {
			if(FocusedRecord == -1 || props == null) return;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			info.SetError(props, errorText, errorType);
		}
		public virtual void ClearRowErrors() {
			ErrorInfo.ClearErrors();
		}
		protected virtual void OnErrorInfo_Changed(object sender, EventArgs e) {
			InvalidateRecord(FocusedRecordDisplayIndex);
		}
		protected int FocusedRecordDisplayIndex {
			get {
				int record = FocusedRecord;
				if(DataModeHelper.NewItemRecordMode && record == DataModeHelper.NewItemRecord)
					record = RecordCount - 1;
				return record;
			}
		}
		[Browsable(false)]
		public virtual bool HasRowErrors { get { return ErrorInfo.HasErrors; } }
		protected virtual ErrorInfo ErrorInfo { get { return errorInfo; } }
		public virtual void CancelUpdateFocusedRecord() {
			if(!FocusedRecordModified) return;
			DataModeHelper.CancelCurrentRowEdit();
			this.fFocusedRecordModified = false;
			ClearRowErrors();
			InvalidateRecord(FocusedRecordDisplayIndex);
		}
		public object InternalGetService(System.Type service) {
			if(service != null && service.Equals(typeof(DataColumnInfoCollection))) {
				if(DataManager == null) return null;
				return DataManager.Columns;
			}
			return GetService(service);
		}
		private void UpdateDataSource() {
		}
		protected virtual void ChangeFocusedRecord(int curRecord) {
			fFocusedRecord = curRecord;
		}
		protected virtual bool CanFocusRecord(int newRecordIndex) {
			UpdateFocusedRecord();
			if(FocusedRecordModified) {
				if(ContainerHelper.AllowHideException) throw new HideException();
				return false;
			}
			return true;
		}
		public virtual bool UpdateFocusedRecord() {
			return UpdateFocusedRecord(false);
		}
		bool UpdateFocusedRecord(bool force) {
			bool result = DataManager.EndCurrentRowEdit(force);
			if(result) fFocusedRecordModified = false;
			InvalidateRecord(FocusedRecordDisplayIndex);
			return result;
		}
		#endregion Data
		#region Rows
		protected virtual void XtraClearRows(XtraItemEventArgs e) {
			RowXtraDeserializer.ClearRows(e);
		}
		protected virtual object XtraCreateRowsItem(XtraItemEventArgs e) {
			return RowXtraDeserializer.CreateRowsItem(e, this);
		}
		protected virtual object XtraFindRowsItem(XtraItemEventArgs e) {
			return RowXtraDeserializer.FindRowsItem(e);
		}
		protected virtual void XtraSetIndexRowsItem(XtraSetItemIndexEventArgs e) {
			RowXtraDeserializer.SetItemIndex(e);
		}
		internal protected virtual BaseRow GetNearestRowCanFocus(BaseRow supposedRow) {
			if(VisibleRows.Count == 0 || DesignMode || supposedRow == null)
				return null;
			if(supposedRow.OptionsRow.AllowFocus)
				return supposedRow;
			int destIndex = supposedRow.VisibleIndex;
			int step = 0;
			int endIndex = FocusedRow == null ? VisibleRows.Count - 1 : FocusedRow.VisibleIndex;
			if(destIndex == 0) {
				destIndex++;
				step = 1;
			}
			if(destIndex == VisibleRows.Count - 1) {
				destIndex--;
				step = -1;
			}
			if(destIndex < 0 || destIndex > VisibleRows.Count - 1)
				return FocusedRow;
			if(step == 0) {
				step = (destIndex < endIndex ? -1 : 1);
				endIndex = (destIndex < endIndex ? 0 : VisibleRows.Count - 1);
			}
			while(destIndex != endIndex + step && destIndex >= 0) { 
				supposedRow = VisibleRows[destIndex];
				if(supposedRow.OptionsRow.AllowFocus)
					return supposedRow;
				destIndex += step;
			}
			return FocusedRow;
		}
		internal protected virtual void UpdateVisibleRows() {
			this.visibleRows = GetVisibleRows();
			this.visibleRows.Sort(VisibleRowsComparer);
		}
		protected GridRowReadOnlyCollection GetFixedRowList(FixedStyle fixedStyle) {
			if(LayoutStyle == LayoutViewStyle.BandsView && fixedStyle != FixedStyle.None)
				return GridRowReadOnlyCollection.Empty;
			if(LayoutStyle == LayoutViewStyle.BandsView && fixedStyle == FixedStyle.None)
				return new GridRowReadOnlyCollection(VisibleRows);
			GridRowReadOnlyCollection list = new GridRowReadOnlyCollection();
			foreach(BaseRow row in VisibleRows) {
				if(row.Fixed == fixedStyle)
					list.Add(row);
			}
			return list;
		}
		protected virtual GridRowReadOnlyCollection GetVisibleRows() {
			GetVisibleRowsRowOperation operation = CreateGetVisibleRowsOperation();
			operation.ShowCategories = OptionsView.ShowRootCategories;
			RowsIterator.DoOperation(operation);
			return operation.Rows;
		}
		protected virtual GetVisibleRowsRowOperation CreateGetVisibleRowsOperation() {
			return !DataModeHelper.FilterByRows ? new GetVisibleRowsRowOperation(this) : new FilterRowPropertiesOperation(this);
		}
		protected internal virtual bool OnRowChanging(BaseRow row, RowChangeArgs args) {
			RowChangingEventArgs e = new RowChangingEventArgs(row, args.Prop, args.ChangeType, args.Value);
			RaiseRowChanging(e);
			if(e.CanChange) args.Value = e.PropertyValue;
			return e.CanChange;
		}
		protected internal virtual void OnRowChanged(BaseRow row, VGridRows rows, RowProperties prop, RowChangeTypeEnum changeType) {
			if(changeType == RowChangeTypeEnum.RowEdit) {
				EditorsPropertiesChanged(GetRowEdit(prop));
			}
			if(changeType == RowChangeTypeEnum.Add ||
				changeType == RowChangeTypeEnum.Delete ||
				changeType == RowChangeTypeEnum.Expanded ||
				changeType == RowChangeTypeEnum.Visible ||
				changeType == RowChangeTypeEnum.Move ||
				changeType == RowChangeTypeEnum.Fixed) {
				ResetVisibleRows();
			}
			if(changeType == RowChangeTypeEnum.Expanded && !row.Expanded && row.HasAsChild(FocusedRow)) {
				CloseEditor();
				if(row != null)
					FocusedRow = row;
			}
			if(changeType == RowChangeTypeEnum.Caption || changeType == RowChangeTypeEnum.Delete ||
				changeType == RowChangeTypeEnum.Height || changeType == RowChangeTypeEnum.MaxCaptionLineCount ||
				changeType == RowChangeTypeEnum.StyleName || changeType == RowChangeTypeEnum.RowAssigned ||
				changeType == RowChangeTypeEnum.Enabled || changeType == RowChangeTypeEnum.Value ||
				(IsMultiEditorPropertiesEvent(changeType) && row.Height == -1)) {
				autoHeights.Remove(row);
				if(Site != null && Site.DesignMode)
					InvalidateUpdate();
			}
			if(changeType == RowChangeTypeEnum.Delete)
				RowsIterator.DoLocalOperation(new RemoveHeight(autoHeights), row.ChildRows);
			if(changeType == RowChangeTypeEnum.PropertiesAdded && row == FocusedRow && row.RowPropertiesCount == 1) FocusedRecordCellIndex = 0;
			if(!GridDisposing && IsInitialized) {
				if(changeType == RowChangeTypeEnum.Add ||
					changeType == RowChangeTypeEnum.Delete ||
					changeType == RowChangeTypeEnum.UnboundType ||
					changeType == RowChangeTypeEnum.UnboundExpression ||
					changeType == RowChangeTypeEnum.FieldName ||
					changeType == RowChangeTypeEnum.ReadOnly) {
					RePopulateColumns();
				}
				if(changeType == RowChangeTypeEnum.Value) {
					InvalidateRow(row);
				}
				else {
					if(changeType == RowChangeTypeEnum.ReadOnly) {
						HideEditor();
					}
					InvalidateUpdate();
				}
				if(rows != null || row == null || changeType == RowChangeTypeEnum.Visible) {
					InvalidateCustomizationForm();
				}
			}
			if(row != null) {
				if(changeType == RowChangeTypeEnum.Add) {
					FocusManager.Update();
				}
				if(changeType == RowChangeTypeEnum.PropertiesDeleted && row == FocusedRow)
					FocusedRecordCellIndex = Math.Min(FocusedRecordCellIndex, ((MultiEditorRow)row).PropertiesCollection.Count - 1);
				if(changeType == RowChangeTypeEnum.Visible && row == FocusedRow && !row.Visible) {
					FocusedRow = GetNearestRowCanFocus(VisibleRows[0]);
				}
				if(changeType == RowChangeTypeEnum.Delete && row == FocusedRow) {
					FocusManager.Update();
				}
				if(changeType == RowChangeTypeEnum.Move && row == FocusedRow) {
					MakeRowVisible(FocusedRow);
				}
			}
			if(row != null)
				row.FireChanged();
			else
				rows.FireChanged();
			RaiseRowChanged(row, prop, changeType);
		}
		void RePopulateColumns() {
			if(IsDataUpdateLocked)
				return;
			DataManager.RePopulateColumns();
		}
		private bool IsMultiEditorPropertiesEvent(RowChangeTypeEnum changeType) {
			return (changeType == RowChangeTypeEnum.PropertiesAdded || changeType == RowChangeTypeEnum.PropertiesCleared ||
				changeType == RowChangeTypeEnum.PropertiesDeleted || changeType == RowChangeTypeEnum.PropertiesReplaced || changeType == RowChangeTypeEnum.Width);
		}
		internal void RowOptionsChanged(BaseRow row, BaseOptionChangedEventArgs e) {
			if(e.Name == VGridOptionsRow.AllowFocusName && row == FocusedRow
				&& !row.OptionsRow.AllowFocus) FocusedRow = VisibleRows[0];
			if(e.Name == VGridOptionsRow.ShowInCustomizationFormName) {
				InvalidateCustomizationForm();
			}
		}
		public virtual void ExpandAllRows() {
			RowOperation op = new ExpandCollapse(this, true);
			RowsIterator.DoLocalOperation(op, Rows);
		}
		public virtual void CollapseAllRows() {
			RowOperation op = new ExpandCollapse(this, false);
			RowsIterator.DoLocalOperation(op, Rows);
		}
		public virtual void FullExpandRow(BaseRow row) {
			if(row == null || !row.HasChildren) return;
			BeginUpdate();
			try {
				row.Expanded = true;
			}
			finally {
				CancelUpdate();
			}
			RowOperation op = new ExpandCollapse(this, true);
			RowsIterator.DoLocalOperation(op, row.ChildRows);
		}
		public void MakeRowVisible(BaseRow row) {
			InternalMakeRowVisible(row, null);
		}
		public virtual void RetrieveFields() {
			BeginUpdate();
			try {
				Rows.DestroyRows();
				if(DataManager.IsReady) {
					for(int i = 0; i < DataManager.Columns.Count; i++) {
						DataColumnInfo dc = DataManager.Columns[i];
						BaseRow row = new EditorRow(dc.Name);
						row.Properties.Caption = dc.Caption;
						Rows.Add(row);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		public BaseRow GetRowByFieldName(string fieldName) {
			return Rows.GetRowByFieldName(fieldName, true);
		}
		internal void InternalMakeRowVisible(BaseRow row, BaseRow prevFocus) {
			if(row == null || row.Grid != this) return;
			BeginUpdate();
			BaseRow parent = row.Rows.ParentRow;
			while(parent != null) {
				parent.Expanded = true;
				parent.Visible = true;
				parent = parent.Rows.ParentRow;
			}
			row.Visible = true;
			CancelUpdate();
			MakeRowVisibleUIRefresh(row, prevFocus);
		}
		void MakeRowVisibleUIRefresh(BaseRow row, BaseRow prevFocus) {
			if(!IsInitialized || GridDisposing || lockLoadData != 0 || IsUpdateLocked)
				return;
			BeginUpdate();
			bool scrolled = false;
			if(!Scroller.IsScrollAnimationInProgress)
				scrolled = fScroller.SetRowVisible(row);
			bool partialRefresh = !scrolled && !HeightChanged(row) && !HeightChanged(prevFocus) && IsCellCalculated(row);
			if(partialRefresh) {
				CancelUpdate();
				Rectangle r = viewInfo.ChangeFocusRow(row, prevFocus);
				Invalidate(r);
			}
			else {
				EndUpdate();
			}
		}
		bool IsCellCalculated(BaseRow row) {
			BaseRowViewInfo rowViewInfo = ViewInfo[row];
			if(rowViewInfo == null)
				return false;
			if(IsCategoryRow(row))
				return true;
			RowValueInfo valueInfo = rowViewInfo[FocusedRecord, FocusedRecordCellIndex];
			return valueInfo != null;
		}
		bool HeightChanged(BaseRow row) {
			if(row == null || row.Grid != this)
				return false;
			int rowHeight = ViewInfo.GetVisibleRowHeight(row);
			AutoHeights.Remove(row);
			ViewInfo.RepositoryItemCache.Remove(row);
			int newRowHeight = ViewInfo.GetVisibleRowHeight(row);
			return rowHeight != newRowHeight;
		}
		private bool IsRowDisconnected(BaseRow row) { return (row == null || !row.IsConnected); }
		public void MakeRecordVisible(int recordIndex) {
			InternalMakeRecordVisible(recordIndex, recordIndex, false);
		}
		internal void InternalMakeRecordVisible(int recordIndex, int prevRecordIndex, bool refresh) {
			InvalidateMakeRecordVisible(prevRecordIndex, recordIndex, refresh);
			if(IsUpdateLocked)
				return;
			UpdateMakeRecordVisible();
		}
		protected virtual void UpdateMakeRecordVisible() {
		}
		protected class UpdateInfo {
			public int NewRecord { get; set; }
			public int OldRecord { get; set; }
			public bool InvalidateRecords { get; set; }
		}
		protected UpdateInfo RecordsUpdateInfo { get; set; }
		protected virtual void InvalidateMakeRecordVisible(int oldIndex, int newIndex, bool invalidate) {
		}
		internal int GetValidRowHandle(int rowHandle) {
			if(!DataManager.IsColumnValid(rowHandle))
				rowHandle = -1;
			return rowHandle;
		}
		public BaseRow GetRowByHandle(int rowHandle) {
			FindRowByHandle op = new FindRowByHandle(rowHandle);
			RowsIterator.DoOperation(op);
			return op.Row;
		}
		public void MoveRow(BaseRow source, BaseRow dest, RowDragEffect effect) {
			if(!CanMoveRow(source, dest, effect))
				return;
			if(effect == RowDragEffect.InsertBefore)
				MoveRow(source, dest, true);
			if(effect == RowDragEffect.MoveChild || effect == RowDragEffect.MoveToEnd)
				MoveRow(source, dest, false);
			if(effect == RowDragEffect.InsertAfter) {
				BeginUpdate();
				try {
					VGridRows destRows = dest == null ? Rows : dest.Rows;
					VGridRows.Move(source, dest, destRows, false);
				}
				finally {
					EndUpdate();
				}
			}
		}
		public void MoveRow(BaseRow source, BaseRow dest, bool insertBefore) {
			if(!CanMoveRow(source, dest, insertBefore)) return;
			BeginUpdate();
			try {
				VGridRows destRows = dest == null ? Rows : dest.ChildRows;
				if(insertBefore && dest != null)
					destRows = dest.Rows;
				else dest = null;
				VGridRows.Move(source, dest, destRows, true);
			}
			finally {
				EndUpdate();
			}
		}
		public void FocusFirst() {
			FocusedRow = GetFirstVisible();
		}
		public void FocusNext() {
			BaseRow row = GetNextVisible(FocusedRow);
			if(row != null)
				FocusedRow = row;
		}
		public void FocusPrev() {
			BaseRow row = GetPrevVisible(FocusedRow);
			if(row != null)
				FocusedRow = row;
		}
		public void FocusLast() {
			FocusedRow = GetLastVisible();
		}
		internal void MoveFocusedRow(int delta, bool cancelOnNullFocus) {
			int previousIndex = FocusedRow != null ? FocusedRow.VisibleIndex : -1;
			if(previousIndex == -1) {
				if(cancelOnNullFocus)
					return;
				else
					previousIndex = 0;
			}
			int curIndex = previousIndex + delta;
			curIndex = Math.Max(0, Math.Min(curIndex, VisibleRows.Count - 1));
			BaseRow focusedRow = GetNearestRowCanFocus(VisibleRows[curIndex]);
			AnimateKeyboardNavigation(focusedRow);
			FocusedRow = focusedRow;
		}
		const int ScrollKeyThreshold = 300;
		DateTime lastSmoothScrollRow = DateTime.MinValue;
		bool CheckLastKeyboardScroll() {
			DateTime lastScroll = this.lastSmoothScrollRow;
			this.lastSmoothScrollRow = DateTime.Now;
			if(DateTime.Now.Subtract(lastScroll).TotalMilliseconds < ScrollKeyThreshold) {
				return true;
			}
			return false;
		}
		protected virtual void AnimateKeyboardNavigation(BaseRow row) {
			int distance = Scroller.GetScrollDistanceToMakeVisible(row);
			if(distance == 0)
				return;
			if(CheckLastKeyboardScroll()) {
				Scroller.CancelAnimatedScroll();
				Scroller.VertScrollPixel(distance);
			}
			else {
				if(OptionsBehavior.AllowAnimatedScrolling) {
					Scroller.AnimateScroll(distance);
				}
				else {
					Scroller.VertScrollPixel(distance);
				}
			}
		}
		public BaseRow GetFirst() {
			return Rows[0];
		}
		public BaseRow GetFirstVisible() {
			return VisibleRows[0];
		}
		public BaseRow GetNext(BaseRow row) {
			if(row == null || row.Grid != this) return null;
			if(row.HasChildren && row.ChildRows.IsLoaded) {
				return row.ChildRows[0];
			}
			if(row.Index < row.Rows.Count - 1) return row.Rows[row.Index + 1];
			BaseRow parent = row.ParentRow;
			while(parent != null) {
				if(parent.Index < parent.Rows.Count - 1) return parent.Rows[parent.Index + 1];
				else parent = parent.ParentRow;
			}
			return null;
		}
		public BaseRow GetNextVisible(BaseRow visibleRow) {
			if(visibleRow == null || visibleRow.Grid != this || visibleRow.VisibleIndex == -1) return null;
			return VisibleRows[visibleRow.VisibleIndex + 1];
		}
		public BaseRow GetPrev(BaseRow row) {
			if(row == null || row.Grid != this) return null;
			if(row.Index == 0) return row.ParentRow;
			else return GetLocalLast(row.Rows[row.Index - 1]);
		}
		public BaseRow GetPrevVisible(BaseRow visibleRow) {
			if(visibleRow == null || visibleRow.Grid != this || visibleRow.VisibleIndex == -1) return null;
			return VisibleRows[visibleRow.VisibleIndex - 1];
		}
		public BaseRow GetLast() {
			return GetLocalLast(Rows[Rows.Count - 1]);
		}
		public BaseRow GetLastVisible() {
			return VisibleRows[VisibleRows.Count - 1];
		}
		protected internal virtual BaseRow GetLocalLast(BaseRow row) {
			if(row == null) return null;
			if(row.HasChildren && row.ChildRows.IsLoaded) {
				FindLastChild op = new FindLastChild();
				RowsIterator.DoLocalOperation(op, row.ChildRows);
				return op.Row;
			}
			else return row;
		}
		protected internal virtual bool CanMoveRow(BaseRow source, BaseRow dest, RowDragEffect effect) {
			return CanMoveRowCore(source, dest, effect);
		}
		protected internal virtual bool CanMoveRowCore(BaseRow source, BaseRow dest, RowDragEffect effect) {
			if(source == dest)
				return false;
			if(source == null || source.Grid != this)
				return false;
			if(dest == null)
				return effect == RowDragEffect.MoveToEnd;
			if(dest.Fixed != source.Fixed)
				return false;
			if(dest.Grid != this)
				return false;
			if(effect == RowDragEffect.MoveChild && dest.ChildRows.IndexOf(source) != -1)
				return !source.Visible;
			if(source.HasAsChild(dest))
				return false;
			if(effect == RowDragEffect.InsertAfter)
				return true;
			return true;
		}
		protected internal virtual bool CanMoveRow(BaseRow source, BaseRow dest, bool before) {
			if(before) {
				return CanMoveRowCore(source, dest, RowDragEffect.InsertBefore);
			}
			else {
				if(dest == null) {
					return CanMoveRowCore(source, dest, RowDragEffect.MoveToEnd);
				}
				return CanMoveRowCore(source, dest, RowDragEffect.MoveChild);
			}
		}
		internal Bitmap GetRowDragBitmap(BaseRow row, Size size) {
			Bitmap bmp = null;
			Graphics g = null;
			try {
				Rectangle r = new Rectangle(0, 0, size.Width, size.Height);
				BaseRowHeaderInfo rh = row.CreateHeaderInfo();
				rh.Calc(r, viewInfo, null, false, null);
				bmp = new Bitmap(r.Width, r.Height);
				g = Graphics.FromImage(bmp);
				using(GraphicsCache cache = new GraphicsCache(g)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache.Graphics, r)) {
						PaintEventArgs e = new PaintEventArgs(bg.Graphics, rh.HeaderRect);
						painter.DrawRowHeader(e, rh, viewInfo.RC);
						bg.Render();
					}
				}
			}
			catch { }
			finally { if(g != null) g.Dispose(); }
			return bmp;
		}
		public void RowsCustomization() {
			RowsCustomization(VGridStore.DefaultCustomizationFormLocation);
		}
		public virtual void RowsCustomization(Point screenLocation) {
			DestroyCustomization();
			customizationForm = new VGridCustomizationForm(this, handler);
			customizationForm.Disposed += new EventHandler(RaiseHideCustomizationForm);
			Form parentForm = FindForm();
			if(parentForm != null)
				parentForm.AddOwnedForm(CustomizationForm);
			customizationForm.ShowCustomization(screenLocation);
			RaiseShowCustomizationForm();
		}
		public virtual void DestroyCustomization() {
			if(customizationForm != null) {
				customizationForm.Dispose();
				customizationForm = null;
			}
		}
		bool ShouldSerializeCustomizationFormBounds() { return CustomizationFormBounds != VGridStore.DefaultCustomizationFormBounds; }
		[Browsable(false), XtraSerializableProperty()]
		public Rectangle CustomizationFormBounds {
			get { return customizationFormBounds; }
			set {
				value = VGridCustomizationForm.CheckCustomizationFormBounds(value);
				if(CustomizationFormBounds == value) return;
				customizationFormBounds = value;
				if(CustomizationForm != null && value != VGridStore.DefaultCustomizationFormBounds) CustomizationForm.Bounds = CustomizationFormBounds;
			}
		}
		public override void Refresh() {
			ViewInfo.IsReady = false;
			UpdateDataOnRefresh();
			base.Refresh();
		}
		protected virtual void UpdateDataOnRefresh() {
		}
		internal void InvalidateUpdate() {
			if (viewInfo == null)
				return;
			viewInfo.IsReady = false;
			Invalidate();
		}
		bool isDataValid = true;
		public virtual void InvalidateData() {
			this.isDataValid = false;
			InvalidateUpdate();
		}
		public virtual void InvalidateRow(BaseRow row) {
			if(row == null || row.Grid != this)
				return;
			Rectangle r = ViewInfo.UpdateRow(row);
			if(!r.IsEmpty) Invalidate(r);
		}
		public virtual void InvalidateRecord(int recordIndex) {
			AutoHeights.Clear();
			Rectangle r = ViewInfo.UpdateRecord(recordIndex);
			if(!r.IsEmpty) Invalidate(r);
		}
		protected void InvalidateRecords(int recordIndex, int prevRecordIndex) {
			Rectangle r1 = ViewInfo.UpdateRecord(recordIndex);
			Rectangle r2 = ViewInfo.UpdateRecord(prevRecordIndex);
			if(!r1.IsEmpty) Invalidate(r1);
			if(!r2.IsEmpty) Invalidate(r2);
		}
		public virtual void InvalidateRowCells(BaseRow row, int recordIndex) {
			if(row == null || row.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId) return;
			BaseRowViewInfo rInfo = ViewInfo[row];
			if(rInfo != null)
				Invalidate(rInfo.UpdateCells(recordIndex));
		}
		void InvalidateCustomizationForm() {
			if(CustomizationForm != null)
				CustomizationForm.FillRows();
		}
		public bool IsEditorRow(BaseRow row) {
			if(row == null)
				return false;
			return row.XtraRowTypeID == RowTypeIdConsts.EditorRowTypeId;
		}
		public bool IsMultiEditorRow(BaseRow row) {
			if(row == null)
				return false;
			return row.XtraRowTypeID == RowTypeIdConsts.MultiEditorRowTypeId;
		}
		public bool IsCategoryRow(BaseRow row) {
			if(row == null)
				return false;
			return row.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId;
		}
		internal void OpenRightCustomizationTabPage(BaseRow row) {
			if(CustomizationForm == null) return;
			CustomizationForm.SwitchTabPage(row);
		}
		protected virtual void OnDataManager_Reset(object sender, EventArgs e) {
			this.fFocusedRecordModified = false;
			BeginUpdate();
			try {
				if(GenerateRowsOnDataManagerReset())
					RetrieveFields();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual bool GenerateRowsOnDataManagerReset() {
			return Rows.Count == 0;
		}
		#endregion Rows
		#region Layout
		public virtual void BeginUpdate() {
			lockUpdate++;
			FocusManager.BeginUpdate();
		}
		public virtual void CancelUpdate() {
			lockUpdate--;
			FocusManager.CancelUpdate();
		}
		public virtual void EndUpdate() {
			CancelUpdate();
			UpdateCore();
		}
		protected virtual void UpdateCore() {
			if(IsUpdateLocked || GridDisposing || IsDisposed)
				return;
			LayoutChanged();
			FocusManager.Update();
			InvalidateCustomizationForm();
		}
		protected internal void BeginLockGridLayout() { lockGridLayout++; }
		protected internal void CancelLockGridLayout() { lockGridLayout--; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void LayoutChangedCore() {
			if(IsUpdateLocked || !IsInitialized || Unloading || ClientSize.IsEmpty) return;
			viewInfo.IsReady = false;
			UpdateLayout();
			Invalidate();
		}
		public virtual void LayoutChanged() {
			HideEditor();
			LayoutChangedCore();
		}
		internal void UpdateLayoutInternal() { UpdateLayout(); }
		void UpdateLayout() {
			if(viewInfo.IsReady)
				return;
			UpdateDataIfNeeded();
			ClearAutoHeights();
			viewInfo.UpdateCache();
			Scroller.InvalidateBandsInfo();
			ViewInfo.Calc();
			Scroller.InvalidateBandsInfo();
			Scroller.Update();
			UpdateMakeRecordVisible();
			if(!IsGridLayoutLocked)
				RaiseGridLayout();
		}
		protected void UpdateDataIfNeeded() {
			if(this.isDataValid)
				return;
			BeginUpdate();
			UpdateData();
			isDataValid = true;
			CancelUpdate();
		}
		protected void UpdateScrollBar() {
			if(IsUpdateLocked)
				return;
			fScroller.Update();
		}
		public virtual VGridHitInfo CalcHitInfo(Point ptGridClient) {
			VGridHitTest ht = viewInfo.CalcHitTest(ptGridClient);
			return ht.ToHitInfo();
		}
		protected virtual void LayoutViewStyleChanged() {
			ResetVisibleRows();
			viewInfo.Dispose();
			viewInfo = CreateViewInfo(false);
			viewInfo.UpdateCache();
			BeginUpdate();
			try { fScroller.LayoutViewStyleChanged(); }
			finally { EndUpdate(); }
			MakeRowVisible(FocusedRow);
		}
		private void CreatePainter() {
			this.painter = CreatePainterCore(new PaintEventHelper(this));
			if(ViewInfo != null) painter.ResourceCache = ViewInfo.RC;
		}
		public void SetDefaultViewOptions() { OptionsView.Assign(CreateOptionsView()); }
		public void SetDefaultBehaviorOptions() { OptionsBehavior.Assign(CreateOptionsBehavior()); }
		protected virtual void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			viewInfo.RC.NeedsUpdate = true;
			BeginUpdate();
			try {
				if(e.Name == VGridOptionsView.ShowRootCategoriesName) {
					ResetVisibleRows();
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual bool AllowPixelScrolling {
			get { return State != VGridState.RowDragging; }
		}
		public virtual void VertScrollPixel(int pixelCount) {
			fScroller.VertScrollPixel(pixelCount);
		}
		public virtual void HorzScrollPixel(int pixelCount) {
			fScroller.HorzScrollPixel(pixelCount);
		}
		public virtual void VertScroll(int rowsCount) {
			fScroller.VertScroll(rowsCount);
		}
		public virtual void HorzScroll(int recordsCount) {
			fScroller.HorzScroll(recordsCount);
		}
		protected virtual void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
		}
		protected virtual void OnOptionsHintChanged(object sender, BaseOptionChangedEventArgs e) {
		}
		protected virtual void OptionsFindChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == VGridOptionsFind.ShowClearButtonName ||
				e.Name == VGridOptionsFind.ShowCloseButtonName ||
				e.Name == VGridOptionsFind.ShowFindButtonName) {
				FindPanelOwner.InitButtons();
				return;
			}
			if (e.Name == VGridOptionsFind.FindNullPromptName) {
				FindPanelOwner.InitButtons();
				return;
			}
			if (e.Name == VGridOptionsFind.FindModeName) {
				FindPanelOwner.InitFindPanel();
				return;
			}
			if (e.Name == VGridOptionsFind.VisibilityName) {
				if (OptionsFind.Visibility == FindPanelVisibility.Always) {
					FindPanelVisible = true;
					FindPanelOwner.InitButtons();
					return;
				}
				if (OptionsFind.Visibility == FindPanelVisibility.Never) {
					FindPanelVisible = false;
					return;
				}
				if (OptionsFind.Visibility == FindPanelVisibility.Default) {
					FindPanelOwner.InitButtons();
					return;
				}
			}
		}
		protected bool IsGridLayoutLocked { get { return (lockGridLayout != 0); } }
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		#endregion Layout
		#region Internal
		internal AppearanceObject InternalRecordCellStyle(RowValueInfo rv) {
			GetCustomRowCellStyleEventArgs e = new GetCustomRowCellStyleEventArgs(rv.Row, rv.RecordIndex, rv.RowCellIndex, rv.Style);
			RaiseRecordCellStyle(e);
			return e.Appearance;
		}
		internal bool InternalCustomizationFormCreatingCategory(string categoryCaption) {
			CategoryRow row = new CategoryRow(categoryCaption);
			row.Visible = false;
			CustomizationFormCreatingCategoryEventArgs e = new CustomizationFormCreatingCategoryEventArgs(row);
			RaiseCustomizationFormCreatingCategory(e);
			if(e.CanCreate && row.Rows == null) Rows.Add(row);
			return e.CanCreate;
		}
		internal bool InternalCustomizationFormDeletingCategory(CategoryRow row) {
			CustomizationFormDeletingCategoryEventArgs e = new CustomizationFormDeletingCategoryEventArgs(row);
			RaiseCustomizationFormDeletingCategory(e);
			if(e.CanDelete) {
				handler.CheckPreserveChildRows(row, HitInfoTypeEnum.CustomizationForm);
				row.Dispose();
			}
			return e.CanDelete;
		}
		#endregion Internal
		#region Event handling
		protected override void RaiseEditorKeyDown(KeyEventArgs e) {
			base.RaiseEditorKeyDown(e);
			if(e.Handled)
				return;
			e.Handled = handler.ProcessChildControlKey(e);
		}
		protected override void RaiseEditorKeyUp(KeyEventArgs e) {
			base.RaiseEditorKeyUp(e);
			if(e.Handled)
				return;
			e.Handled = handler.ProcessChildControlKeyUp(e);
		}
		protected override bool IsInputKey(Keys key) {
			bool res = base.IsInputKey(key);
			switch(key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
					res = true;
					break;
				case Keys.Tab:
					if(!OptionsBehavior.UseTabKey)
						res = false;
					break;
			}
			return res;
		}
		protected override bool IsInputChar(char charCode) {
			return true;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Down || key == Keys.Up ||
				key == Keys.Left || key == Keys.Right) return false;
			if(key == Keys.Tab) {
				if((Control.ModifierKeys & Keys.Control) != 0) keyData = key;
				else if(OptionsBehavior.UseTabKey) return false;
			}
			if((key == Keys.Enter || key == Keys.Escape) && State == VGridState.Editing) return false;
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnResize(EventArgs e) {
			BeginLockGridLayout();
			try {
				handler.Resize();
			}
			finally { CancelLockGridLayout(); }
			base.OnResize(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			lock (this) {
				if (GridDisposing)
					return;
				UpdateLayout();
				painter.DoDraw(new DevExpress.Utils.Drawing.DXPaintEventArgs(e), viewInfo);
			}
			RaisePaintEvent(this, e);
		}
		protected override void OnSystemColorsChanged(EventArgs e) {
			base.OnSystemColorsChanged(e);
			DevExpress.Utils.WXPaint.Painter.ThemeChanged();
			fScroller.SystemColorsChanged();
			LayoutChanged();
		}
		protected override void OnLostCapture() {
			handler.LostCapture();
		}
		protected internal virtual bool CanGridFocusOnMouseDown(VGridHitTest ht) {
			if(DesignMode) return false;
			return (ht.HitInfoType != HitInfoTypeEnum.CustomizationForm);
		}
		protected internal virtual bool CanResetCursor { get { return !DesignMode; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			try {
				base.OnMouseDown(e);
				handler.MouseDown(e);
			}
			catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			handler.MouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			try {
				base.OnMouseUp(e);
				handler.MouseUp(e);
			}
			catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnDoubleClick(EventArgs e) {
			try {
				base.OnDoubleClick(e);
				handler.DoubleClick(PointToClient(Control.MousePosition));
			}
			catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnMouseWheelCore(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheelCore(ee);
			if (ee.Handled)
				return;
			Handler.OnMouseWheel(ee);
		}
		protected override void OnMouseEnter(EventArgs e) {
			handler.MouseEnter(PointToClient(Control.MousePosition));
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			handler.MouseLeave();
			base.OnMouseLeave(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			try { handler.KeyDown(e); }
			catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled)
				return;
			try {
				handler.KeyUp(e);
			} catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			try { handler.KeyPress(e); }
			catch(Exception err) {
				if(!(err is HideException)) {
					throw;
				}
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			handler.GotFocus();
			base.OnGotFocus(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			try {
				ContainerHelper.BeginAllowHideException();
				handler.LostFocus();
			}
			catch(HideException) { }
			finally {
				ContainerHelper.EndAllowHideException();
			}
			base.OnLostFocus(e);
		}
		#endregion Event handling
		#region Event raising
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRowChanging"),
#endif
 Category("Property Changed")]
		public event RowChangingEventHandler RowChanging {
			add { Events.AddHandler(GS.rowChanging, value); }
			remove { Events.RemoveHandler(GS.rowChanging, value); }
		}
		protected virtual void RaiseRowChanging(RowChangingEventArgs e) {
			RowChangingEventHandler handler = (RowChangingEventHandler)this.Events[GS.rowChanging];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRowChanged"),
#endif
 Category("Property Changed")]
		public event RowChangedEventHandler RowChanged {
			add { Events.AddHandler(GS.rowChanged, value); }
			remove { Events.RemoveHandler(GS.rowChanged, value); }
		}
		protected virtual void RaiseRowChanged(BaseRow gridRow, RowProperties prop, RowChangeTypeEnum changeType) {
			RowChangedEventHandler handler = (RowChangedEventHandler)this.Events[GS.rowChanged];
			if(handler != null) handler(this, new RowChangedEventArgs(gridRow, prop, changeType));
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseFocusedRowChanged"),
#endif
 Category("Property Changed")]
		public event FocusedRowChangedEventHandler FocusedRowChanged {
			add { Events.AddHandler(GS.focusedRowChanged, value); }
			remove { Events.RemoveHandler(GS.focusedRowChanged, value); }
		}
		internal protected virtual void RaiseFocusedRowChanged(FocusedRowChangedEventArgs e) {
			FocusedRowChangedEventHandler handler = (FocusedRowChangedEventHandler)this.Events[GS.focusedRowChanged];
			if(handler != null)
				RaiseIfNotDisposed(delegate { handler(this, e); });
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseFocusedRecordChanged"),
#endif
 Category("Property Changed")]
		public event IndexChangedEventHandler FocusedRecordChanged {
			add { Events.AddHandler(GS.focusedRecordChanged, value); }
			remove { Events.RemoveHandler(GS.focusedRecordChanged, value); }
		}
		protected virtual void RaiseFocusedRecordChanged(IndexChangedEventArgs e) {
			ClearRowErrors();
			IndexChangedEventHandler handler = (IndexChangedEventHandler)this.Events[GS.focusedRecordChanged];
			if(handler != null)
				RaiseIfNotDisposed(delegate { handler(this, e); });
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseFocusedRecordCellChanged"),
#endif
 Category("Property Changed")]
		public event IndexChangedEventHandler FocusedRecordCellChanged {
			add { Events.AddHandler(GS.focusedRecordCellChanged, value); }
			remove { Events.RemoveHandler(GS.focusedRecordCellChanged, value); }
		}
		internal protected virtual void RaiseFocusedRecordCellChanged(IndexChangedEventArgs e) {
			IndexChangedEventHandler handler = (IndexChangedEventHandler)this.Events[GS.focusedRecordCellChanged];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseStateChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler StateChanged {
			add { Events.AddHandler(GS.stateChanged, value); }
			remove { Events.RemoveHandler(GS.stateChanged, value); }
		}
		protected internal virtual void RaiseStateChanged() {
			EventHandler handler = (EventHandler)this.Events[GS.stateChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRecordCellStyle"),
#endif
 Category("Appearance")]
		public event GetCustomRowCellStyleEventHandler RecordCellStyle {
			add { Events.AddHandler(GS.recordCellStyle, value); }
			remove { Events.RemoveHandler(GS.recordCellStyle, value); }
		}
		protected virtual void RaiseRecordCellStyle(GetCustomRowCellStyleEventArgs e) {
			GetCustomRowCellStyleEventHandler handler = (GetCustomRowCellStyleEventHandler)this.Events[GS.recordCellStyle];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomRecordCellEdit"),
#endif
 Category("Behavior")]
		public event GetCustomRowCellEditEventHandler CustomRecordCellEdit {
			add { Events.AddHandler(GS.customRecordCellEdit, value); }
			remove { Events.RemoveHandler(GS.customRecordCellEdit, value); }
		}
		protected internal bool IsCustomRecordCellEditExists {
			get {
				return (GetCustomRowCellEditEventHandler)this.Events[GS.customRecordCellEdit] != null;
			}
		}
		protected virtual void RaiseCustomRecordCellEdit(GetCustomRowCellEditEventArgs e) {
			GetCustomRowCellEditEventHandler handler = (GetCustomRowCellEditEventHandler)this.Events[GS.customRecordCellEdit];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomRecordCellEditForEditing"),
#endif
 Category("Behavior")]
		public event GetCustomRowCellEditEventHandler CustomRecordCellEditForEditing {
			add { Events.AddHandler(GS.customRecordCellEditForEditing, value); }
			remove { Events.RemoveHandler(GS.customRecordCellEditForEditing, value); }
		}
		protected virtual void RaiseCustomRecordCellEditForEditing(GetCustomRowCellEditEventArgs e) {
			GetCustomRowCellEditEventHandler handler = (GetCustomRowCellEditEventHandler)this.Events[GS.customRecordCellEditForEditing];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseGridLayout"),
#endif
 Category("Layout")]
		public event EventHandler GridLayout {
			add { Events.AddHandler(GS.gridLayout, value); }
			remove { Events.RemoveHandler(GS.gridLayout, value); }
		}
		protected virtual void RaiseGridLayout() {
			EventHandler handler = (EventHandler)this.Events[GS.gridLayout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseLayoutUpgrade"),
#endif
 Category("Layout")]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { Events.AddHandler(GS.layoutUpgrade, value); }
			remove { Events.RemoveHandler(GS.layoutUpgrade, value); }
		}
		protected virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)Events[GS.layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseBeforeLoadLayout"),
#endif
 Category("Layout")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(GS.beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(GS.beforeLoadLayout, value); }
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[GS.beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomDrawRowHeaderIndent"),
#endif
 Category("Custom Draw")]
		public event CustomDrawRowHeaderIndentEventHandler CustomDrawRowHeaderIndent {
			add { Events.AddHandler(GS.customDrawRowHeaderIndent, value); }
			remove { Events.RemoveHandler(GS.customDrawRowHeaderIndent, value); }
		}
		protected internal virtual void RaiseCustomDrawRowHeaderIndent(CustomDrawRowHeaderIndentEventArgs e) {
			CustomDrawRowHeaderIndentEventHandler handler = (CustomDrawRowHeaderIndentEventHandler)this.Events[GS.customDrawRowHeaderIndent];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomDrawRowHeaderCell"),
#endif
 Category("Custom Draw")]
		public event CustomDrawRowHeaderCellEventHandler CustomDrawRowHeaderCell {
			add { Events.AddHandler(GS.customDrawRowHeaderCell, value); }
			remove { Events.RemoveHandler(GS.customDrawRowHeaderCell, value); }
		}
		protected internal virtual void RaiseCustomDrawRowHeaderCell(CustomDrawRowHeaderCellEventArgs e) {
			CustomDrawRowHeaderCellEventHandler handler = (CustomDrawRowHeaderCellEventHandler)this.Events[GS.customDrawRowHeaderCell];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomDrawRowValueCell"),
#endif
 Category("Custom Draw")]
		public event CustomDrawRowValueCellEventHandler CustomDrawRowValueCell {
			add { Events.AddHandler(GS.customDrawRowValueCell, value); }
			remove { Events.RemoveHandler(GS.customDrawRowValueCell, value); }
		}
		protected internal virtual void RaiseCustomDrawRowValueCell(CustomDrawRowValueCellEventArgs e) {
			CustomDrawRowValueCellEventHandler handler = (CustomDrawRowValueCellEventHandler)this.Events[GS.customDrawRowValueCell];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomDrawSeparator"),
#endif
 Category("Custom Draw")]
		public event CustomDrawSeparatorEventHandler CustomDrawSeparator {
			add { Events.AddHandler(GS.customDrawSeparator, value); }
			remove { Events.RemoveHandler(GS.customDrawSeparator, value); }
		}
		protected internal virtual void RaiseCustomDrawSeparator(CustomDrawSeparatorEventArgs e) {
			CustomDrawSeparatorEventHandler handler = (CustomDrawSeparatorEventHandler)this.Events[GS.customDrawSeparator];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomDrawTreeButton"),
#endif
 Category("Custom Draw")]
		public event CustomDrawTreeButtonEventHandler CustomDrawTreeButton {
			add { Events.AddHandler(GS.customDrawTreeButton, value); }
			remove { Events.RemoveHandler(GS.customDrawTreeButton, value); }
		}
		protected internal virtual void RaiseCustomDrawTreeButton(CustomDrawTreeButtonEventArgs e) {
			CustomDrawTreeButtonEventHandler handler = (CustomDrawTreeButtonEventHandler)this.Events[GS.customDrawTreeButton];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseShowingEditor"),
#endif
 Category("Editor")]
		public event CancelEventHandler ShowingEditor {
			add { Events.AddHandler(GS.showingEditor, value); }
			remove { Events.RemoveHandler(GS.showingEditor, value); }
		}
		protected virtual void RaiseShowingEditor(ref bool cancel) {
			cancel = false;
			CancelEventHandler handler = (CancelEventHandler)this.Events[GS.showingEditor];
			if(handler != null) {
				CancelEventArgs e = new CancelEventArgs();
				handler(this, e);
				cancel = e.Cancel;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseShownEditor"),
#endif
 Category("Editor")]
		public event EventHandler ShownEditor {
			add { Events.AddHandler(GS.shownEditor, value); }
			remove { Events.RemoveHandler(GS.shownEditor, value); }
		}
		protected virtual void RaiseShownEditor() {
			EventHandler handler = (EventHandler)this.Events[GS.shownEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCellValueChanging"),
#endif
 Category("Property Changed")]
		public event CellValueChangedEventHandler CellValueChanging {
			add { this.Events.AddHandler(GS.cellValueChanging, value); }
			remove { this.Events.RemoveHandler(GS.cellValueChanging, value); }
		}
		protected virtual void RaiseCellValueChanging(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[GS.cellValueChanging];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCellValueChanged"),
#endif
 Category("Property Changed")]
		public event CellValueChangedEventHandler CellValueChanged {
			add { this.Events.AddHandler(GS.cellValueChanged, value); }
			remove { this.Events.RemoveHandler(GS.cellValueChanged, value); }
		}
		protected virtual void RaiseCellValueChanged(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[GS.cellValueChanged];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseHiddenEditor"),
#endif
 Category("Editor")]
		public event EventHandler HiddenEditor {
			add { Events.AddHandler(GS.hiddenEditor, value); }
			remove { Events.RemoveHandler(GS.hiddenEditor, value); }
		}
		protected virtual void RaiseHiddenEditor() {
			EventHandler handler = (EventHandler)this.Events[GS.hiddenEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseValidatingEditor"),
#endif
 Category("Editor")]
		public event BaseContainerValidateEditorEventHandler ValidatingEditor {
			add { Events.AddHandler(GS.validatingEditor, value); }
			remove { Events.RemoveHandler(GS.validatingEditor, value); }
		}
		internal protected virtual void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BaseContainerValidateEditorEventHandler handler = (BaseContainerValidateEditorEventHandler)this.Events[GS.validatingEditor];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseInvalidValueException"),
#endif
 Category("Action")]
		public event InvalidValueExceptionEventHandler InvalidValueException {
			add { Events.AddHandler(GS.invalidValueException, value); }
			remove { Events.RemoveHandler(GS.invalidValueException, value); }
		}
		internal protected virtual void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[GS.invalidValueException];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseShowCustomizationForm"),
#endif
 Category("Behavior")]
		public event EventHandler ShowCustomizationForm {
			add { Events.AddHandler(GS.showCustomizationForm, value); }
			remove { Events.RemoveHandler(GS.showCustomizationForm, value); }
		}
		protected virtual void RaiseShowCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[GS.showCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseHideCustomizationForm"),
#endif
 Category("Behavior")]
		public event EventHandler HideCustomizationForm {
			add { Events.AddHandler(GS.hideCustomizationForm, value); }
			remove { Events.RemoveHandler(GS.hideCustomizationForm, value); }
		}
		protected virtual void RaiseHideCustomizationForm(object sender, EventArgs e) {
			customizationForm.Disposed -= new EventHandler(RaiseHideCustomizationForm);
			this.customizationFormBounds = customizationForm.Bounds;
			EventHandler handler = (EventHandler)this.Events[GS.hideCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
			customizationForm = null;
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomizationFormCreatingCategory"),
#endif
 Category("Behavior")]
		public event CustomizationFormCreatingCategoryEventHandler CustomizationFormCreatingCategory {
			add { Events.AddHandler(GS.customizationFormCreatingCategory, value); }
			remove { Events.RemoveHandler(GS.customizationFormCreatingCategory, value); }
		}
		protected virtual void RaiseCustomizationFormCreatingCategory(CustomizationFormCreatingCategoryEventArgs e) {
			CustomizationFormCreatingCategoryEventHandler handler = (CustomizationFormCreatingCategoryEventHandler)this.Events[GS.customizationFormCreatingCategory];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseCustomizationFormDeletingCategory"),
#endif
 Category("Behavior")]
		public event CustomizationFormDeletingCategoryEventHandler CustomizationFormDeletingCategory {
			add { Events.AddHandler(GS.customizationFormDeletingCategory, value); }
			remove { Events.RemoveHandler(GS.customizationFormDeletingCategory, value); }
		}
		protected virtual void RaiseCustomizationFormDeletingCategory(CustomizationFormDeletingCategoryEventArgs e) {
			CustomizationFormDeletingCategoryEventHandler handler = (CustomizationFormDeletingCategoryEventHandler)this.Events[GS.customizationFormDeletingCategory];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseDataSourceChanged"),
#endif
 Category("DataSource")]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(GS.dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(GS.dataSourceChanged, value); }
		}
		protected virtual void RaiseDataSourceChanged() {
			EventHandler handler = (EventHandler)Events[GS.dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseStartDragRow"),
#endif
 Category("Drag Drop")]
		public event StartDragRowEventHandler StartDragRow {
			add { this.Events.AddHandler(GS.startDragRow, value); }
			remove { this.Events.RemoveHandler(GS.startDragRow, value); }
		}
		protected internal virtual void RaiseStartDragRow(StartDragRowEventArgs e) {
			StartDragRowEventHandler handler = (StartDragRowEventHandler)Events[GS.startDragRow];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseProcessDragRow"),
#endif
 Category("Drag Drop")]
		public event ProcessDragRowEventHandler ProcessDragRow {
			add { this.Events.AddHandler(GS.processDragRow, value); }
			remove { this.Events.RemoveHandler(GS.processDragRow, value); }
		}
		protected internal virtual void RaiseProcessDragRow(DragRowEventArgs e) {
			ProcessDragRowEventHandler handler = (ProcessDragRowEventHandler)Events[GS.processDragRow];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseEndDragRow"),
#endif
 Category("Drag Drop")]
		public event EndDragRowEventHandler EndDragRow {
			add { this.Events.AddHandler(GS.endDragRow, value); }
			remove { this.Events.RemoveHandler(GS.endDragRow, value); }
		}
		protected internal virtual void RaiseEndDragRow(EndDragRowEventArgs e) {
			EndDragRowEventHandler handler = (EndDragRowEventHandler)Events[GS.endDragRow];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRowHeaderWidthChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler RowHeaderWidthChanged {
			add { Events.AddHandler(GS.rowHeaderWidthChanged, value); }
			remove { Events.RemoveHandler(GS.rowHeaderWidthChanged, value); }
		}
		internal virtual void RaiseRowHeaderWidthChanged() {
			EventHandler handler = (EventHandler)this.Events[GS.rowHeaderWidthChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseRecordWidthChanged"),
#endif
 Category("Property Changed")]
		public event EventHandler RecordWidthChanged {
			add { Events.AddHandler(GS.recordWidthChanged, value); }
			remove { Events.RemoveHandler(GS.recordWidthChanged, value); }
		}
		internal virtual void RaiseRecordWidthChanged() {
			EventHandler handler = (EventHandler)this.Events[GS.recordWidthChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseValidateRecord"),
#endif
 Category("Data")]
		public event ValidateRecordEventHandler ValidateRecord {
			add { Events.AddHandler(GS.validateRecord, value); }
			remove { Events.RemoveHandler(GS.validateRecord, value); }
		}
		protected virtual void RaiseValidateRecord(ValidateRecordEventArgs e) {
			ValidateRecordEventHandler handler = (ValidateRecordEventHandler)this.Events[GS.validateRecord];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseInvalidRecordException"),
#endif
 Category("Data")]
		public event InvalidRecordExceptionEventHandler InvalidRecordException {
			add { Events.AddHandler(GS.invalidRecordException, value); }
			remove { Events.RemoveHandler(GS.invalidRecordException, value); }
		}
		protected virtual void RaiseInvalidRecordException(InvalidRecordExceptionEventArgs e) {
			InvalidRecordExceptionEventHandler handler = (InvalidRecordExceptionEventHandler)this.Events[GS.invalidRecordException];
			if(handler != null) handler(this, e);
			if(e.ExceptionMode == ExceptionMode.DisplayError) {
				DialogResult dr = XtraMessageBox.Show(e.ErrorText, e.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if(dr == DialogResult.No)
					e.ExceptionMode = ExceptionMode.Ignore;
			}
			if(e.ExceptionMode == ExceptionMode.Ignore) CancelUpdateFocusedRecord();
			if(e.ExceptionMode == ExceptionMode.ThrowException) {
				throw (e.Exception);
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseInitNewRecord"),
#endif
 Category("Data")]
		public event RecordIndexEventHandler InitNewRecord {
			add { Events.AddHandler(GS.initNewRecord, value); }
			remove { Events.RemoveHandler(GS.initNewRecord, value); }
		}
		protected virtual void RaiseInitNewRecord(int recordIndex) {
			RecordIndexEventHandler handler = (RecordIndexEventHandler)this.Events[GS.initNewRecord];
			if(handler != null) handler(this, new RecordIndexEventArgs(recordIndex));
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBasePopupMenuShowing"),
#endif
		Category("Behavior")]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(GS.onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(GS.onPopupMenuShowing, value); }
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseShowMenu"),
#endif
		Category("Behavior")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use 'PopupMenuShowing' instead", false)]
		public event VGridControlMenuEventHandler ShowMenu {
			add { Events.AddHandler(GS.showMenu, value); }
			remove { Events.RemoveHandler(GS.showMenu, value); }
		}
		internal protected virtual void RaiseShowMenuCore(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[GS.onPopupMenuShowing];
			if(handler != null)
				handler(this, e);
		}
		internal protected virtual void RaiseShowMenu(PopupMenuShowingEventArgs e) {
			RaiseShowMenuCore(e);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			VGridControlMenuEventHandler handler = (VGridControlMenuEventHandler)this.Events[GS.showMenu];
			if(handler != null) {
				VGridControlMenuEventArgs args = new VGridControlMenuEventArgs(e.Menu, e.Row);
				handler(this, args);
			}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseLeftVisibleRecordChanged"),
#endif
 Category("Property Changed")]
		public event IndexChangedEventHandler LeftVisibleRecordChanged {
			add { Events.AddHandler(GS.leftVisibleRecordChanged, value); }
			remove { Events.RemoveHandler(GS.leftVisibleRecordChanged, value); }
		}
		internal protected virtual void RaiseLeftVisibleRecordChanged(int newPixel, int oldPixel, int newIndex, int oldIndex) {
			IndexChangedEventHandler handler = (IndexChangedEventHandler)this.Events[GS.leftVisibleRecordChanged];
			if(handler != null)
				handler(this, new PixelIndexChangedEventArgs(newPixel, oldPixel, newIndex, oldIndex));
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlBaseTopVisibleRowIndexChanged"),
#endif
 Category("Property Changed")]
		public event IndexChangedEventHandler TopVisibleRowIndexChanged {
			add { Events.AddHandler(GS.topVisibleRowIndexChanged, value); }
			remove { Events.RemoveHandler(GS.topVisibleRowIndexChanged, value); }
		}
		internal protected virtual void RaiseTopVisibleRowIndexChanged(int newPixel, int oldPixel, int newIndex, int oldIndex) {
			IndexChangedEventHandler handler = (IndexChangedEventHandler)this.Events[GS.topVisibleRowIndexChanged];
			if(handler != null)
				handler(this, new PixelIndexChangedEventArgs(newPixel, oldPixel, newIndex, oldIndex));
		}
		protected void RaiseIfNotDisposed(Action0 action) {
			if(GridDisposing || IsDisposed)
				return;
			action();
		}
		#endregion Event raising
		#region IPrintable
		IPrintingSystem printingSystem = null;
		internal ILink printingLink = null;
		internal int printIndent = 0;
		internal IBrickGraphics curentPGraph;
		VGridPrinterBase printer;
		protected internal IPrintingSystem PrintingSystem {
			get { return printingSystem; }
			set {
				if(PrintingSystem == value) return;
				if(PrintingSystem != null) {
					PrintingSystem.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystem_AfterChange);
				}
				printingSystem = value;
				if(PrintingSystem != null) {
					PrintingSystem.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystem_AfterChange);
				}
			}
		}
		void OnPrintingSystem_AfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(PrintingSystem == null || this.printingLink == null) return;
			switch(e.EventName) {
				case SR.PageSettingsChanged:
				case SR.AfterMarginsChange:
					((LinkBase)this.printingLink).Margins = ((PrintingSystemBase)PrintingSystem).PageMargins;
					this.printingLink.CreateDocument();
					break;
			}
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return true; }
		}
		bool IPrintable.HasPropertyEditor() {
			return false;
		}
		UserControl IPrintable.PropertyEditorControl {
			get {
				return null;
			}
		}
		bool IPrintable.SupportsHelp() { return false; }
		void IPrintable.ShowHelp() { }
		void IPrintable.AcceptChanges() {
		}
		void AcceptChanges(ControlCollection controls) {
			if(controls == null) return;
		}
		void IPrintable.RejectChanges() { }
		protected virtual void SetCommandsVisibility() {
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportCsv, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportFile, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportMht, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportTxt, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendCsv, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendFile, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendGraphic, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendMht, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendTxt, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
			PrintingSystem.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			OnLoaded();
			this.printIndent = 0;
			PrintingSystem = ps;
			SetCommandsVisibility();
			printer = new VGridPrinterBase(this);
			this.printingLink = link;
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			PrintingSystem = null;
			this.printingLink = null;
		}
		protected internal BaseViewInfo CreatePrintViewInfo(IBrickGraphics graph) {
			BaseViewInfo vi = CreateViewInfo(true);
			curentPGraph = graph;
			vi.Calc();
			return vi;
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if(printer != null) printer.CreatePrintArea(PrintingSystem, areaName, graph);
		}
		#endregion
		#region Export
		void ExecutePrintExport(Action0 action) {
			ComponentPrinter.ClearDocument();
			action();
		}
		public void Export(ExportTarget target, string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(target, filePath);
			});
		}
		public void Export(ExportTarget target, Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(target, stream);
			});
		}
		public void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(target, stream, options);
			});
		}
		public void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(target, filePath, options);
			});
		}
		public void ExportToXlsx(string filePath) {
			ExecutePrintExport(delegate() { 
				ComponentPrinter.Export(ExportTarget.Xlsx, filePath);
			});
		}
		public void ExportToXlsx(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xlsx, stream);
			});
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xlsx, stream, options);
			});
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xlsx, filePath, options);
			});
		}
		public void ExportToXls(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xls, filePath);
			});
		}
		public void ExportToXls(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xls, stream);
			});
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xls, stream, options);
			});
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Xls, filePath, options);
			});
		}
		public void ExportToRtf(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Rtf, filePath);
			});
		}
		public void ExportToRtf(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Rtf, stream);
			});
		}
		public void ExportToHtml(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Html, filePath);
			});
		}
		public void ExportToHtml(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Html, stream);
			});
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Html, stream, options);
			});
		}
		public void ExportToHtml(String filePath, HtmlExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Html, filePath, options);
			});
		}
		public void ExportToMht(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Mht, filePath);
			});
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Mht, filePath, options);
			});
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Mht, stream, options);
			});
		}
		public void ExportToPdf(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Pdf, filePath);
			});
		}
		public void ExportToPdf(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Pdf, stream);
			});
		}
		public void ExportToText(Stream stream) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Text, stream);
			});
		}
		public void ExportToText(string filePath) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Text, filePath);
			});
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Text, filePath, options);
			});
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Export(ExportTarget.Text, stream, options);
			});
		}
		#endregion
		#region Accessibility
		protected internal virtual BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null)
					dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual BaseAccessible CreateAccessibleInstance() { return new DevExpress.XtraVerticalGrid.Accessibility.GridAccessibleObject(this); }
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			BaseGridAccessibleObject gridAccessible = DXAccessible as BaseGridAccessibleObject;
			if(gridAccessible == null)
				return base.GetAccessibilityObjectById(objectId);
			return gridAccessible.GetAccessibleObjectById(objectId, -1);
		}
		public void AccessibleNotifyClients(AccessibleEvents accEvents, int childID) {
			AccessibilityNotifyClients(accEvents, 10, childID);
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null)
				return base.CreateAccessibilityInstance();
			DXAccessible.ParentCore = null;
			return DXAccessible.Accessible;
		}
		#endregion Accessibility
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		public void ShowPrintPreview() {
			ExecutePrintExport(delegate() {
				ComponentPrinter.ShowPreview(this.LookAndFeel);
			});
		}
		public void ShowRibbonPrintPreview() {
			ExecutePrintExport(delegate() {
				ComponentPrinter.ShowRibbonPreview(this.LookAndFeel);
			});
		}
		public void Print() {
			ExecutePrintExport(delegate() {
				ComponentPrinter.Print();
			});
		}
		protected internal virtual void RowDoubleClick(BaseRow row) {
			if(!ViewInfo.IsResizeable(row)) {
				row.Height = -1;
				return;
			}
			if(row.OptionsRow.DblClickExpanding) {
				ChangeExpanded(row);
			}
		}
		internal void ExpandRow(BaseRow row) {
			if(row == null || row.Expanded) return;
			BeginUpdate();
			try {
				row.Expanded = true;
				if(OptionsBehavior.SmartExpand) {
					ViewInfo.IsReady = false;
					UpdateLayout();
					Scroller.SetRowMaxVisible(row);
				}
			}
			finally {
				EndUpdate();
			}
		}
		internal void ChangeExpanded(BaseRow row) {
			string rowName = row.Name;
			CloseEditor();
			BaseRow expandingRow = row.IsConnected ? row : GetRowByName(rowName);
			if(expandingRow.Expanded)
				expandingRow.Expanded = false;
			else
				ExpandRow(expandingRow);
		}
		public BaseRow GetRowByName(string name) {
			FindRowByNameOperation operation = new FindRowByNameOperation(name);
			RowsIterator.DoOperation(operation);
			return operation.Row;
		}
		public BaseRow GetRowByCaption(string caption) {
			FindRowByCaption operation = new FindRowByCaption(caption);
			RowsIterator.DoOperation(operation);
			return operation.Row;
		}
		protected virtual bool CanShowRowMenu(BaseRow row) {
			return row != null;
		}
		internal void ShowContextMenu(Point point, VGridHitTest hitTest) {
			if(CanShowRowMenu(hitTest.Row)) {
				ShowRowPropertiesMenu(point, hitTest.Row, hitTest.Row.GetRowProperties(hitTest.ToHitInfo().CellIndex));
			}
		}
		protected internal virtual void ShowRowPropertiesMenu(Point point, BaseRow row, RowProperties rowProperties) {
			if(row == null)
				return;
			RowProperties properties = rowProperties == null ? row.Properties : rowProperties;
			if(!CanShowRowMenu(row))
				return;
			Menu.ShowMenu(point, properties);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseRow CreateEditorRow() {
			return new EditorRow();
		}
		protected virtual DialogResult GetFormResult(Form frm) {
			if(FindForm() != null)
				return frm.ShowDialog(FindForm());
			return frm.ShowDialog();
		}
		protected virtual internal void DoShowContextMenu(DevExpress.Utils.Menu.DXPopupMenu menu, Point point) {
			DevExpress.Utils.Menu.MenuManagerHelper.ShowMenu(menu, LookAndFeel, MenuManager, this, point);
		}
		internal protected virtual bool IsDefault(RepositoryItem item) {
			return false;
		}
		protected internal VGridViewInfoHelper ViewInfoHelper { get { return viewInfoHelper; } }
		protected virtual internal VGridViewInfoHelper CreateViewInfoHelper() {
			return new VGridViewInfoHelper();
		}
		protected virtual internal void OnClearViewInfo() { }
		#region IGestureClient Members
		Point touchStart = Point.Empty;
		IntPtr IGestureClient.Handle { get { return Handle; } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
				VGridHitInfo hitInfo = CalcHitInfo(point);
				if(hitInfo.Row != null || hitInfo.HitInfoType == HitInfoTypeEnum.Empty) {
					return new GestureAllowArgs[] { new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_ALL }, GestureAllowArgs.PressAndTap };
				}
				return GestureAllowArgs.None; 
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			HideEditor();
			if(delta.Y != 0) {
				int totalDelta = GetDeltaRowCount(info);
				TopVisibleRowIndexPixel = touchStart.Y - totalDelta;
			}
			if(delta.X != 0) {
				int totalDelta = GetDeltaRecordCount(info);
				LeftVisibleRecordPixel = touchStart.X - totalDelta;
			}
		}
		int GetDeltaRecordCount(GestureArgs info) {
			int deltaX = info.Current.X - info.Start.X;
			return deltaX;
		}
		int GetDeltaRowCount(GestureArgs info) {
			int deltaY = info.Current.Y - info.Start.Y;
			return deltaY;
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			touchStart.X = LeftVisibleRecordPixel;
			touchStart.Y = TopVisibleRowIndexPixel;
			CloseEditor();
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		#endregion
		#region FindPanel
		FindPanel findPanel;
		string findFilterText = string.Empty;
		CriteriaOperator findFilterCriteria = null;
		bool findPanelVisible;
		protected internal CriteriaOperator FindFilterCriteria {
			get { return findFilterCriteria; }
			set {
				findFilterCriteria = value;
				FilterCriteriaChanged();
			}
		}
		FindPanelOwner findPanelOwner;
		protected internal FindPanelOwner FindPanelOwner {
			get {
				if(findPanelOwner == null)
					findPanelOwner = CreateFindPanelOwner();
				return findPanelOwner;
			}
		}
		protected internal FindPanel FindPanel { get { return findPanel; } }
		internal bool SearchControlFocus() {
			if(!IsAttachedToSearchControl) return false;
			FindPanelOwner.SearchControl.Focus();
			return true;
		}
		bool IsAttachedToSearchControl { get { return ((ISearchControlClient)this).IsAttachedToSearchControl; } }
		[DefaultValue(false)]
		public bool FindPanelVisible {
			get { return findPanelVisible; }
			set {
				if (FindPanelVisible == value)
					return;
				findPanelVisible = value;
				FindPanelVisibleChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty]
		public string FindFilterText {
			get { return findFilterText; }
			set {
					findFilterText = value;
					FindFilterTextChanged();
			}
		}
		private void FindFilterTextChanged() {
			FindPanelOwner.ApplyFindFilter(FindFilterText);
		}
		void FindPanelVisibleChanged() {
			if (FindPanelVisible && !IsAttachedToSearchControl) {
				if (FindPanel == null) {
					CreateFindPanel();
				}
				FindPanelOwner.InitButtons();
				FindPanelOwner.InitFindPanel();
				FindPanel.Visible = true;
			}
			else {
				if (FindPanel != null)
					FindPanel.Visible = false;
			}
			LayoutChanged();
		}
		public void ShowFindPanel() {
			FindPanelVisible = true;
			if (FindPanel != null) {
				FindPanelOwner.FocusFindEditor();
				FindPanelOwner.ApplyFindFilter(FindFilterText);
			}
		}
		public void HideFindPanel() {
			if (FindPanel != null && FindPanel.ContainsFocus)
				Focus();
			if (OptionsFind.ClearFindOnClose)
				FindPanelOwner.ApplyFindFilter("");
			FindPanelVisible = false;
		}
		protected virtual FindPanel CreateFindPanelCore() {
			return new FindPanel(FindPanelOwner);
		}
		protected virtual FindPanelOwner CreateFindPanelOwner() {
			return new FindPanelOwner(this);
		}
		protected void CreateFindPanel() {
			this.findPanel = CreateFindPanelCore();
			if (FindPanel != null) {
				FindPanelOwner.InitFindPanel();
				Controls.Add(FindPanel);
			}
		}
		protected void DestroyFindPanel() {
			if (FindPanel != null) {
				FindPanel.Dispose();
			}
			this.findPanel = null;
		}
		#endregion
		public BaseRow CreateRow(int rowTypeId) {
			switch (rowTypeId) {
				case RowTypeIdConsts.CategoryRowTypeId:
					return new CategoryRow();
				case RowTypeIdConsts.EditorRowTypeId:
					return CreateEditorRow();
				case RowTypeIdConsts.MultiEditorRowTypeId:
					return new MultiEditorRow();
				default:
					throw new InvalidCastException();
			}
		}
		public virtual IDataColumnInfo[] GetFindColumnNames() {
			if (DataModeHelper.FilterByRows) {
				return new IDataColumnInfo[] { new RowProperties(PGridDataModeHelper.CaptionColumnName) };
			}
			else {
				return VisibleRows.Cast<BaseRow>()
					.SelectMany(row => row.GetRowProperties())
					.Where(p => FindPanelOwner.IsAllowFindColumn(p)).ToArray();
			}
		}
		protected internal virtual bool FindByDisplayText { get { return !DataModeHelper.FilterByRows; } }
		protected internal virtual bool HighlightHeaders { get { return DataModeHelper.FilterByRows; } }
		#region ISearchControlClient Members                
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			if(!string.IsNullOrEmpty(((SearchColumnsInfo)searchInfo).Columns))
				OptionsFind.FindFilterColumns = ((SearchColumnsInfo)searchInfo).Columns;
			FindFilterText = searchInfo.SearchText;			
		}
		IEnumerable IFilteredComponentColumns.Columns { get { return GetExternalSeachColumns(); } }
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new VGridControlBaseProvider(this);
		}
		protected virtual IEnumerable<string> GetExternalSeachColumns() {
			return VisibleRows.Cast<BaseRow>()
				.SelectMany(row => row.GetRowProperties())
				.Where(p => p.Bindable)
				.Select(p => p.FieldName);
		}
		bool ISearchControlClient.IsAttachedToSearchControl {
			get { return FindPanelOwner.SearchControl != null; }
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {			
			FindPanelOwner.SearchControl = searchControl;
			if(!Disposing && !IsDisposed) {
				FindFilterText = null;
				FindPanelVisible = !IsAttachedToSearchControl && OptionsFind.Visibility == FindPanelVisibility.Always;
			}
		}
		#endregion
		protected internal virtual bool CanSkipCategoryRow(CategoryRow categoryRow) {
			return categoryRow.Level == 0;
		}
	}
}
