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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Helpers;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid.Data {
	public interface IPivotGridEventsImplementor : IPivotGridEventsImplementorBase {
		bool ShowingCustomizationForm(Form customizationForm, ref Control parentControl);
		void ShowCustomizationForm();
		void HideCustomizationForm();
		void OnPopupMenuShowing(PopupMenuShowingEventArgs e);
		void OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e);		
		int GetCustomRowHeight(PivotFieldValueItem item, int height);
		int GetCustomColumnWidth(PivotFieldValueItem item, int width);		
		int FieldValueImageIndex(PivotFieldValueItem item);
		void CellDoubleClick(PivotCellViewInfo cellViewInfo);
		void CellClick(PivotCellViewInfo cellViewInfo);
		bool CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw);
		bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw);
		bool CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw);
		bool CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw);
		bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw);
		void CustomAppearance(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds);
		void FocusedCellChanged();
		void CellSelectionChanged();
		object CustomEditValue(object value, PivotGridCellItem cellItem);
		RepositoryItem GetCellEdit(PivotGridCellItem cellItem);
		void LeftTopCellChanged(Point oldValue, Point newValue);
		void AsyncOperationStarting();
		void AsyncOperationCompleted();
	}
	public interface IPivotGridDataOwner {
		void FireChanged(object[] changedObjects);
	}
	class PivotClipboardAccessor : IPivotClipboardAccessor {
		public void SetDataObject(string clipbloarObject) {
			try {
				Clipboard.SetDataObject(clipbloarObject, true, 3, 100);
			} catch(System.Runtime.InteropServices.ExternalException) { }
		}
	}
	public class PivotGridViewInfoData : PivotGridDataAsync, IViewInfoControl, IPrefilterOwner, IFilteredComponent, IPivotGridOptionsPrintOwner {
		CurrencyManager parentListCurrencyManager = null;
		internal PivotGridViewInfo viewInfo;
		UserLookAndFeel controlLookAndFeel;
		Rectangle customizationFormBounds;
		CustomizationForm customizationForm;
		PivotGridOptionsHint optionsHint;
		PivotGridOptionsMenu optionsMenu;
		IDXMenuManager menuManager;
		IViewInfoControl control;
		PivotGridCells cells;
		PivotEventArgsHelper eventArgsHelper;
		public PivotGridViewInfoData() : this(null) { }
		public PivotGridViewInfoData(IViewInfoControl control) {
			this.control = control;
			EventsImplementor = control as IPivotGridEventsImplementor;
			this.appearance = new PivotGridAppearances(this);
			Appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.appearancePrint = CreatePivotGridAppearancesPrint();
			AppearancePrint.Changed += new EventHandler(OnAppearancePrintChanged);
			this.paintAppearance = new PivotGridAppearances(this);
			this.paintAppearancePrint = new PivotGridAppearancesPrint(this);
			this.paintAppearanceDirty = true;
			this.paintAppearancePrintDirty = true;
			this.defaultAppearance = null;
			this.borderStyle = BorderStyles.Default;
			this.viewInfo = CreateViewInfo();
			this.eventArgsHelper = CreateEventArgsHelper();
			this.controlLookAndFeel = null;
			this.customizationFormBounds = Rectangle.Empty;
			this.customizationForm = null;
			this.optionsHint = new PivotGridOptionsHint();
			this.optionsMenu = new PivotGridOptionsMenu();
			this.menuManager = null;
			this.cells = new PivotGridCells(this);
			this.formatConditions = CreateFormatConditionCollection();
			this.formatConditions.Data = this;
			this.formatConditions.CollectionChanged += OnFormatRuleChanged;
			this.formatRules = new PivotGridFormatRuleCollection(this);
			this.formatRules.CollectionChanged += OnFormatRuleChanged;
#if DEBUGTEST
			DevExpress.XtraPivotGrid.Tests.SafeFixture.AddDisposable(this);
#endif
		}
		protected virtual PivotGridFormatConditionCollection CreateFormatConditionCollection() {
			return new PivotGridFormatConditionCollection();
		}
		protected override IPivotClipboardAccessor CreateClipboardAccessor() {
			return new PivotClipboardAccessor();
		}
		protected virtual PivotGridAppearancesPrint CreatePivotGridAppearancesPrint() {
			return new PivotGridAppearancesPrint(this);
		}
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new PivotVisualItems(this);
		}
		protected override void Dispose(bool disposing) {
			if(this.customizationForm != null) {
				this.customizationForm.Dispose();
				this.customizationForm = null;
			}
			if(disposing && this.viewInfo != null) {
				this.eventArgsHelper = null;
				this.viewInfo.Dispose();
				this.viewInfo = null;
			}
			if(disposing) {
				Appearance.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearance.Dispose();
				this.appearance = null;
				AppearancePrint.Changed -= new EventHandler(OnAppearancePrintChanged);
				this.appearancePrint.Dispose();
				this.appearancePrint = null;
				this.ParentListCurrencyManager = null;
			}
			base.Dispose(disposing);
		}
		protected override PivotGridData CreateEmptyInstance() {
			return PivotGrid.CreateEmptyData();
		}
		protected internal virtual PivotGridDragManager CreateDragManager(PivotFieldItem field, DragCompletedCallback callback) {
			return new PivotGridDragManager(this, field, callback);
		}
		protected virtual PivotGridViewInfo CreateViewInfo() {
			return new PivotGridViewInfo(this);
		}
		PivotEventArgsHelper CreateEventArgsHelper() {
			return new PivotEventArgsHelper(this, (IThreadSafeAccessible)VisualItemsInternal);
		}
		protected override PrefilterBase CreatePrefilter() {
			return new Prefilter(this);
		}
		protected override PivotChartDataSourceBase CreateChartDataSource() {
			return new PivotWinChartDataSource(this);
		}
		protected override DevExpress.XtraPivotGrid.Events.PivotGridEventRaiserBase CreateEventRaiser(IPivotGridEventsImplementorBase eventsImplementor) {
			return eventsImplementor == null ? null : new PivotGridEventRaiser(eventsImplementor);
		}
		public bool IsEnabled {
			get { return ControlOwner != null ? ControlOwner.Enabled : true; }
		}
		public bool UseDisabledStatePainter {
			get { return PivotGrid != null ? PivotGrid.UseDisabledStatePainter && !IsInProcessing : true; }
		}
		public override bool IsDesignMode {
			get { return Control != null ? Control.IsDesignMode : false; }
		}
		public virtual bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(PivotGrid); } }
		public new PivotGridFieldCollection Fields { get { return (PivotGridFieldCollection)base.Fields; } }
		public PivotGridControl PivotGrid { get { return Control as PivotGridControl; } }
		protected override PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollection(this);
		}
		public void SetListSource(BindingContext context, object dataSource, string dataMember) {
			ListSource = MasterDetailHelper.GetDataSource(context, dataSource, dataMember);
		}
		public override void SetControlDataSource(IList ds) {
			if(PivotGrid != null) {
				PivotGrid.DataSource = ds;
			}
		}
		[Browsable(false)]
		public PivotChartDataSourceBase ChartDataSource { get { return ChartDataSourceInternal; } }
		public PivotGridViewInfo ViewInfo { 
			get {
				if(viewInfo != null && !Disposing)
					viewInfo.EnsureIsCalculated();
				return viewInfo; 
			} 
		}
		public PivotEventArgsHelper EventArgsHelper {
			get { return eventArgsHelper; }
		}
		public UserLookAndFeel ControlLookAndFeel {
			get { return controlLookAndFeel; }
			set { controlLookAndFeel = value; }
		}
		public UserLookAndFeel ActiveLookAndFeel { get { return ControlLookAndFeel != null ? ControlLookAndFeel : UserLookAndFeel.Default; } }
		public new Prefilter Prefilter { get { return (Prefilter)base.Prefilter; } }
		public new PivotVisualItems VisualItems { get { return (PivotVisualItems)base.VisualItems; } }
		protected internal new PivotVisualItems VisualItemsInternal { get { return (PivotVisualItems)base.VisualItemsInternal; } }
		public new PivotGridOptionsBehavior OptionsBehavior { get { return (PivotGridOptionsBehavior)base.OptionsBehavior; } }
		protected override PivotGridOptionsBehaviorBase CreateOptionsBehavior() {
			return new PivotGridOptionsBehavior(OnOptionsBehaviorChanged);
		}
		protected override PivotGridOptionsFilterBase CreateOptionsFilter() { return new PivotGridOptionsFilterPopup(OnOptionsFilterChanged); }
		public PivotGridOptionsFilterPopup OptionsFilterPopup { get { return (PivotGridOptionsFilterPopup)base.OptionsFilter; } }
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridOptionsChartDataSource(this);
		}
		public new PivotGridOptionsChartDataSource OptionsChartDataSource { get { return (PivotGridOptionsChartDataSource)base.OptionsChartDataSource; } }		
		public PivotGridOptionsHint OptionsHint { get { return optionsHint; } }
		public PivotGridOptionsMenu OptionsMenu { get { return optionsMenu; } }		
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		public PivotGridCells Cells { get { return cells; } }
		protected override PivotGridFieldBase CreateDataField() {
			return new PivotGridField(this);
		}
		public new PivotGridField DataField { get { return (PivotGridField)base.DataField; } }
		protected override PivotGridOptionsViewBase CreateOptionsView() { return new PivotGridOptionsView(new EventHandler(OnOptionsViewChanged)); }
		public new PivotGridOptionsView OptionsView { get { return (PivotGridOptionsView)base.OptionsView; } }
		protected override PivotGridOptionsCustomization CreateOptionsCustomization() { return new PivotGridOptionsCustomizationEx(new EventHandler(OnOptionsChanged)); }
		public new PivotGridOptionsCustomizationEx OptionsCustomization { get { return (PivotGridOptionsCustomizationEx)base.OptionsCustomization; } }
		public new PivotGridOptionsDataField OptionsDataField { get { return base.OptionsDataField; } }
		public CustomizationForm CustomizationForm { get { return customizationForm; } }
		public Rectangle CustomizationFormBounds { get { return customizationFormBounds; } set { customizationFormBounds = value; } }
		public override bool IsLoading {
			get {
				if(IsDeserializing) return true;
				if(ControlOwner is IComponentLoading)
					return ((IComponentLoading)ControlOwner).IsLoading;
				if(Control is IComponentLoading)
					return ((IComponentLoading)Control).IsLoading;
				return false;
			}
		}
		public override bool IsControlReady {
			get {
				if(PivotGrid != null)
					return !PivotGrid.IsUpdating;
				return false;
			}
		}
		public void BestFit(PivotGridField field) {
			if(ViewInfo != null)
				ViewInfo.BestFit((PivotFieldItem)GetFieldItem(field));
		}		
		#region CurrencyDataController's code
		protected CurrencyManager ParentListCurrencyManager {
			get { return parentListCurrencyManager; }
			set {
				if(ParentListCurrencyManager == value)
					return;
				if(ParentListCurrencyManager != null) {
					ParentListCurrencyManager.ItemChanged -= OnCurrencyManager_ItemChanged;
				}
				parentListCurrencyManager = value;
				if(ParentListCurrencyManager != null) {
					ParentListCurrencyManager.ItemChanged += OnCurrencyManager_ItemChanged;
				}
			}
		}
		void OnCurrencyManager_ItemChanged(object sender, EventArgs e) {
			if(PivotGrid == null)
				return;
			IList listSource = MasterDetailHelper.GetDataSource(PivotGrid.BindingContext, PivotGrid.DataSource, PivotGrid.DataMember);
			if(ListSource != listSource)
				ListSource = listSource;
		}
		public void SetDataSource(BindingContext bindingContext, object dataSource, string dataMember) {
			ListSource = MasterDetailHelper.GetDataSource(bindingContext, dataSource, dataMember);
			CurrencyManager currencyManager = GetCurrencyManager(bindingContext, dataSource, dataMember);
			CurrencyManagerEndEdit(currencyManager);
			if(currencyManager != null)
				currencyManager.PositionChanged += (d,e) => CurrencyManagerEndEdit(currencyManager);
			if(!string.IsNullOrEmpty(dataMember))
				ParentListCurrencyManager = GetCurrencyManager(bindingContext, dataSource, "");
		}
		protected static void CurrencyManagerEndEdit(CurrencyManager currencyManager) {
			if(currencyManager != null && currencyManager.Position >= 0) 
				currencyManager.EndCurrentEdit();
		}
		protected CurrencyManager GetCurrencyManager(BindingContext bindingContext, object dataSource, string dataMember) {
			if(bindingContext == null || dataSource == null) return null;
			return bindingContext[dataSource, dataMember] as CurrencyManager;
		}
		#endregion CurrencyDataController's code
		public override bool AllowHideFields {
			get {
				if(OptionsCustomization.AllowHideFields == AllowHideFieldsType.WhenCustomizationFormVisible)
					return CustomizationForm != null;
				return OptionsCustomization.AllowHideFields == AllowHideFieldsType.Always ? true : false;
			}
		}
		public override void OnPopupMenuShowing(object e) {
			if(EventsImplementor != null)
				EventsImplementor.OnPopupMenuShowing((PopupMenuShowingEventArgs)e);
		}
		public override void OnPopupMenuItemClick(object e) {
			if(EventsImplementor != null)
				EventsImplementor.OnPopupMenuItemClick((PivotGridMenuItemClickEventArgs)e);
		}
		public override void FocusedCellChanged(Point oldValue, Point newValue) {
			base.FocusedCellChanged(oldValue, newValue);
			if(OptionsBehavior.RepaintGridOnFocusedCellChanged)
				Invalidate(ViewInfo.Bounds);
			else
				InvalidateFocusedCell(oldValue, newValue);
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.FocusedCellChanged();
		}		
		public override void CellSelectionChanged() {
			base.CellSelectionChanged();
			Invalidate(ViewInfo.Bounds);
			if(EventsImplementor != null && !Disposing)
				EventsImplementor.CellSelectionChanged();
		}
		public override void MakeCellVisible(Point cell) {
			ViewInfo.Scroller.ScrollToCell(cell);
		}
		public int GetFieldValueSeparator(PivotFieldValueItem pivotFieldValueItem) {
			PivotGridOptionsSeparator optionsSeparator = pivotFieldValueItem.IsColumn ? OptionsView.ColumnValueSeparator : OptionsView.RowValueSeparator;
			if(optionsSeparator.AutoLevelCount == 0 || pivotFieldValueItem.Level < optionsSeparator.AutoLevelCount)
				return optionsSeparator.Width;
			return 0;
		}
		public PivotFieldsAreaViewInfoBase GetAreaViewInfo(bool isColumn) {
			return isColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
		}
		public int GetVisibleIndex(PivotGridField field, int lastLevelIndex) {
			PivotFieldsAreaViewInfoBase viewInfo = field.IsColumn ? ViewInfo.ColumnAreaFields : ViewInfo.RowAreaFields;
			if(lastLevelIndex < 0 || lastLevelIndex >= viewInfo.LastLevelItemCount) return -1;
			for(int i = viewInfo.GetLastLevelViewInfo(lastLevelIndex).VisibleIndex; i >= 0; i--)
				if(GetObjectLevel(field.IsColumn, i) == field.AreaIndex) return i;
			return -1;
		}
		public int GetPivotFieldImageIndex(PivotFieldValueItem item) {
			if(EventsImplementor != null)
				return EventsImplementor.FieldValueImageIndex(item);
			else return -1;
		}		
		public int GetCustomRowHeight(PivotFieldValueItem item, int height) {
			if(EventsImplementor != null)
				return EventsImplementor.GetCustomRowHeight(item, height);
			else
				return height;
		}
		public int GetCustomColumnWidth(PivotFieldValueItem item, int width) {
			if(EventsImplementor != null)
				return EventsImplementor.GetCustomColumnWidth(item, width);
			else
				return width;
		}
		public override object GetCustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
			else
				return base.GetCustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		public RepositoryItem GetCellEdit(PivotGridCellItem cellItem) {
			return EventsImplementor != null ? EventsImplementor.GetCellEdit(cellItem) : null;
		}
		public void CellDoubleClick(PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CellDoubleClick(cellViewInfo);
		}
		public void CellClick(PivotCellViewInfo cellViewInfo) {
			if(EventsImplementor != null)
				EventsImplementor.CellClick(cellViewInfo);
		}
		public bool CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawHeaderArea(headersViewInfo, paintArgs, bounds, defaultDraw);
			else return false;
		}
		public bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawEmptyArea(appearanceOwner, paintArgs, bounds, defaultDraw);
			else return false;
		}
		public bool CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawFieldHeader(headerViewInfo, paintArgs, painter, defaultDraw);
			else return false;
		}
		public bool CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawFieldValue(paintArgs, fieldCellViewInfo, info, painter, defaultDraw);
			else return false;
		}
		public bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			if(EventsImplementor != null)
				return EventsImplementor.CustomDrawCell(paintArgs, ref appearance, cellViewInfo, defaultDraw);
			else return false;
		}
		public void CustomAppearance(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			if(EventsImplementor != null)
				EventsImplementor.CustomAppearance(ref appearance, cellItem, bounds);
		}
		public Point CustomizationFormDefaultLocation {
			get { return new Point(CustomizationFormBase.DefaultLocation, CustomizationFormBase.DefaultLocation); }
		}
		public void ChangeFieldsCustomizationVisible() {
			if(IsFieldCustomizationShowing) DestroyCustomization();
			else FieldsCustomization();
		}
		public virtual void FieldsCustomization(Control parentControl, Point showPoint, CustomizationFormStyle style) {
			DestroyCustomization();
			if(ControlOwner == null) return;
			customizationForm = CreateCustomizationForm(style);
			CustomizationForm.LookAndFeel.ParentLookAndFeel = ActiveLookAndFeel;
			if(!OnShowingCustomizationForm(ref parentControl)) {
				CustomizationForm.ShowCustomization(parentControl, showPoint);
				OnShowCustomizationForm();
			} else {
				DestroyCustomizationCore(false);
			}
			InvalidateFieldItems();
		}
		public void FieldsCustomization(Control parentControl) {
			FieldsCustomization(parentControl, CustomizationFormDefaultLocation, OptionsCustomization.CustomizationFormStyle);
		}
		public void FieldsCustomization(Point showPoint) {
			FieldsCustomization(null, showPoint, OptionsCustomization.CustomizationFormStyle);
		}
		public void FieldsCustomization() {
			FieldsCustomization(CustomizationFormDefaultLocation);
		}
		public void DestroyCustomization() {
			DestroyCustomizationCore(true);
			InvalidateFieldItems();
		}
		void DestroyCustomizationCore(bool fireEvent) {
			if(customizationForm != null) {
				this.customizationFormBounds = CustomizationForm.Bounds;
				customizationForm.Dispose();
				customizationForm = null;
				if(fireEvent)
					OnHideCustomizationForm();
			}
		}
		public override bool IsFieldCustomizationShowing { get { return this.customizationForm != null; } }
		protected virtual CustomizationForm CreateCustomizationForm(CustomizationFormStyle style) {
			return new CustomizationForm(this, style);
		}
		public void ChangePrefilterVisible() {
			Prefilter.ChangePrefilterVisible();
		}
		public bool IsPrefilterFormShowing { get { return Prefilter.IsPrefilterFormShowing; } }
		bool IPrefilterFormOwner.ShowOperandTypeIcon {
			get { return Prefilter.ShowOperandTypeIcon; }
		}
		public override void FireChanged(object[] objs) {
			if(IsLockUpdate || IsLoading || Disposing) return;
			IPivotGridDataOwner dataOwner = (IPivotGridDataOwner)ControlOwner;
			if(dataOwner != null)
				dataOwner.FireChanged(objs);
		}
		public override void OnFieldSizeChanged(PivotGridFieldBase field, bool widthChanged, bool heightChanged) {
			base.OnFieldSizeChanged(field, widthChanged, heightChanged);
			Invalidate();
		}
		public void InvalidateViewInfo() {
			if(this.viewInfo != null)
				viewInfo.Clear();
		}
		public new PivotGridField GetField(PivotFieldItemBase item) {
			return (PivotGridField)base.GetField(item);
		}
		public new PivotFieldItem GetFieldItem(PivotGridFieldBase field) {
			return (PivotFieldItem)base.GetFieldItem(field);
		}
		protected override void InvokeInMainThread(AsyncCompletedInternal internalCompleted) {
			if(PivotGrid == null) return;
			if(!IsInMainThread)
				PivotGrid.Invoke((MethodInvoker)delegate() {
					InvokeInMainThreadBase(internalCompleted);
				});
			else
				InvokeInMainThreadBase(internalCompleted);
		}
		protected override bool IsInMainThread { 
			get {
				if(PivotGrid == null) return true;
				return !PivotGrid.InvokeRequired;
			}
		}
		protected override void DoBeforeAsyncProcessStarted () {
			base.DoBeforeAsyncProcessStarted();
			ViewInfo.ClearBeforeAsyncOperation();
			if(Control != null)
				Control.Update();
		}
		protected override void AsyncProcessStarting() {
			base.AsyncProcessStarting();
			SetControlEnabled(false);
			if(EventsImplementor != null)
				EventsImplementor.AsyncOperationStarting();
		}
		protected override void ShowLoadingPanelInternal() {
			base.ShowLoadingPanelInternal();
			Invalidate();
		}
		void SetControlEnabled(bool enabled) {
			if(ControlOwner == null)
				return;
			WinApiProvider.SetControlEnabled(ControlOwner, enabled);
			if(Control != null)
				Control.EnableScrollBars(enabled);
			if(CustomizationForm != null)
				WinApiProvider.SetControlEnabled(CustomizationForm, enabled);
		}
		protected override void AsyncProcessFinishing() {
			base.AsyncProcessFinishing();
			SetControlEnabled(true);
			if(EventsImplementor != null)
				EventsImplementor.AsyncOperationCompleted();
		}
		protected override void HideLoadingPanelInternal() {
			base.HideLoadingPanelInternal();
			Invalidate();
		}
		void InvokeInMainThreadBase(AsyncCompletedInternal internalCompleted) {
			base.InvokeInMainThread(internalCompleted);
		}
		public override void Invalidate() {
			Invalidate(ClientRectangle);
		}
		public virtual void Invalidate(Rectangle bounds) {
			if(!IsLockUpdate)
				InvalidateControl(bounds);
		}
		public void InvalidateCell(Point cell) {
			if(ViewInfo == null) return;
			ViewInfo.CellsArea.InvalidatedCell(cell);
		}
		public void InvalidateFocusedCell() {
			if(!IsLocked) {
				InvalidateCell(VisualItemsInternal.FocusedCell);
			}
		}
		public void InvalidateFocusedCell(Point oldValue, Point newValue) {
			PivotCellViewInfo oldViewInfo = ViewInfo.CellsArea.GetCellViewInfoByCoord(oldValue.X, oldValue.Y),
				newViewInfo = ViewInfo.CellsArea.GetCellViewInfoByCoord(newValue.X, newValue.Y);
			if(oldViewInfo != null && newViewInfo != null) {
				Invalidate(new Rectangle(
					Math.Min(oldViewInfo.PaintBounds.Left, newViewInfo.PaintBounds.Left),
					Math.Min(oldViewInfo.PaintBounds.Top, newViewInfo.PaintBounds.Top),
					Math.Max(oldViewInfo.PaintBounds.Right, newViewInfo.PaintBounds.Right),
					Math.Max(oldViewInfo.PaintBounds.Bottom, newViewInfo.PaintBounds.Bottom)
				));
			}
			if(newViewInfo != null)
				Invalidate(newViewInfo.PaintBounds);
			if(oldViewInfo != null)
				Invalidate(oldViewInfo.PaintBounds);
		}
		public override void OnFieldVisibleChanged(PivotGridFieldBase field, int oldAreaIndex, bool doRefresh) {
			base.OnFieldVisibleChanged(field, oldAreaIndex, doRefresh);
			PopupateCustomizationFormFields();			
		}
		protected internal void PopupateCustomizationFormFields() {
			if(!IsLocked && CustomizationForm != null && !IsDeserializing && !IsLockUpdate)
				CustomizationForm.Populate();
		}
		public override void OnGroupsChanged() {
			base.OnGroupsChanged();
			PopupateCustomizationFormFields();
		}
		public override void OnColumnInsert(PivotGridFieldBase field) {
			base.OnColumnInsert(field);
			FireChanged(field);
		}
		public override void EndUpdate() {
			base.EndUpdate();
			PopupateCustomizationFormFields();
			FireChanged();
		}
		protected internal IViewInfoControl Control { get { return control; } }
		protected virtual void InvalidateControl(Rectangle bounds) {
			if(Control != null) Control.Invalidate(bounds);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new IPivotGridEventsImplementor EventsImplementor {
			get { return (IPivotGridEventsImplementor)base.EventsImplementor; }
			set { base.EventsImplementor = value; } 
		}
		protected virtual bool OnShowingCustomizationForm(ref Control parentControl) {
			if(EventsImplementor != null)
				return EventsImplementor.ShowingCustomizationForm(CustomizationForm, ref parentControl);
			return false;
		}
		protected virtual void OnShowCustomizationForm() {
			if(EventsImplementor != null)
				EventsImplementor.ShowCustomizationForm();
		}
		protected virtual void OnHideCustomizationForm() {
			if(EventsImplementor != null)
				EventsImplementor.HideCustomizationForm();
		}
		public virtual void OnLeftTopCellChanged(Point oldValue, Point newValue) {
			if(EventsImplementor != null)
				EventsImplementor.LeftTopCellChanged(oldValue, newValue);
		}
		public Control ControlOwner { get { return Control != null ? Control.ControlOwner : null; } }
		internal virtual IContainer GetComponentContainer() {
			if(ControlOwner is Component)
				return ((Component)ControlOwner).Container;
			if(Control is Component)
				return ((Component)Control).Container;
			return null;
		}
		public virtual Rectangle ClientRectangle {
			get {
				return Control != null ? Control.ClientRectangle : Rectangle.Empty;
			}
		}
		public virtual Rectangle ClientRectangleAccordingBounds {
			get {
				Rectangle bounds = ClientRectangle;
				if(bounds.IsEmpty) return bounds;
				BorderObjectInfoArgs infoArgs = new BorderObjectInfoArgs();
				infoArgs.Bounds = bounds;
				BorderPainter painter = BorderHelper.GetGridPainter(BorderStyle, ActiveLookAndFeel);
				bounds = painter.GetObjectClientRectangle(infoArgs);
				return bounds;
			}
		}
		protected virtual void OnOptionsBehaviorChanged(object sender, EventArgs e) {
			ViewInfo.BoundsOffset = Size.Empty;
			ViewInfo.LeftTopCoord = Point.Empty;
			base.OnOptionsChanged(sender, e);
		}
		#region IViewInfoControl
		Rectangle IViewInfoControl.ClientRectangle { get { return ClientRectangle; } }
		void IViewInfoControl.Invalidate(Rectangle bounds) {
			Invalidate(bounds);
		}
		Control IViewInfoControl.ControlOwner { get { return ControlOwner; } }
		void IViewInfoControl.UpdateScrollBars() {
			if(Control != null) Control.UpdateScrollBars();
		}
		void IViewInfoControl.EnableScrollBars(bool enabled) {
			if(Control != null) Control.EnableScrollBars(enabled);
		}
		void IViewInfoControl.InvalidateScrollBars() {
			if(Control != null) Control.InvalidateScrollBars();
		}
		void IViewInfoControl.Update() {
			if(Control != null) Control.Update();
		}
		bool IViewInfoControl.ScrollBarOverlap {
			get {
				if(Control != null) return Control.ScrollBarOverlap;
				return false;
			}
		}
		#endregion
		#region FormatConditions
		PivotGridFormatRuleCollection formatRules;
		public PivotGridFormatRuleCollection FormatRules { get { return formatRules; } }
		PivotGridFormatConditionCollection formatConditions;
		public PivotGridFormatConditionCollection FormatConditions { get { return formatConditions; } }
		void OnFormatRuleChanged(object sender, EventArgs e) {
			if(GetAggregations(true).Count > 0)
				DoRefresh();
			LayoutChanged();
			FireChanged();
		}
		#endregion
		#region Appearance
		PivotGridAppearances appearance;
		PivotGridAppearances paintAppearance;
		AppearanceDefaultInfo[] defaultAppearance;
		PivotGridAppearancesPrint appearancePrint;
		PivotGridAppearancesPrint paintAppearancePrint;
		bool paintAppearanceDirty;
		bool paintAppearancePrintDirty;
		BorderStyles borderStyle;
		public PivotGridAppearances Appearance { get { return appearance; } }
		public PivotGridAppearancesPrint AppearancePrint { get { return appearancePrint; } }
		public PivotGridAppearances PaintAppearance {
			get {
				if(this.paintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		public PivotGridAppearancesPrint PaintAppearancePrint {
			get {
				if(this.paintAppearancePrintDirty) UpdatePaintAppearancePrint();
				return paintAppearancePrint;
			}
		}
		void UpdatePaintAppearance() {
			this.paintAppearanceDirty = false;
			paintAppearance.Combine(Appearance, DefaultAppearance);
			paintAppearance.UpdateRightToLeft(IsRightToLeft);
			Appearance.LoadFieldValueExtraProperties(DefaultAppearance);
		}
		void UpdatePaintAppearancePrint() {
			this.paintAppearancePrintDirty = false;
			paintAppearancePrint.Combine(AppearancePrint, AppearancePrint.GetAppearanceDefaultInfo());
			paintAppearancePrint.UpdateRightToLeft(IsRightToLeft);
		}
		AppearanceDefaultInfo[] DefaultAppearance {
			get {
				if(this.defaultAppearance == null)
					this.defaultAppearance = Appearance.GetAppearanceDefaultInfo(ActiveLookAndFeel);
				return this.defaultAppearance;
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			paintAppearanceDirty = true;
			LayoutChanged();
			FireChanged();
		}
		void OnAppearancePrintChanged(object sender, EventArgs e) {
			paintAppearancePrintDirty = true;
			FireChanged();
		}
		[DefaultValue(BorderStyles.Default), XtraSerializableProperty()]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		public void LookAndFeelChanged() {
			this.paintAppearanceDirty = true;
			this.defaultAppearance = null;
			LayoutChanged();
		}
		#endregion		
		internal UserAction UserAction {
			get {
				if(PivotGrid != null)
					return PivotGrid.UserAction;
				return UserAction.None;
			}
			set {
				if(PivotGrid != null)
					PivotGrid.UserAction = value;
			}
		}
		#region IPrefilterOwner Members
		Control IPrefilterFormOwner.ControlOwner { get { return ControlOwner; } }
		UserLookAndFeel IPrefilterOwner.ActiveLookAndFeel { get { return ActiveLookAndFeel; } }
		IFilteredComponent IPrefilterFormOwner.FilteredComponent { get { return this; } }
		void IPrefilterFormOwner.SetPrefilterVisible(bool visible) {
			UserAction = visible ? UserAction.Prefilter : UserAction.None;
		}
		#endregion
		#region IFilteredComponent Members
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return new FieldFilterColumnCollection(Fields, Prefilter.PrefilterColumnNames, MenuManager);
		}
		#endregion
		public object CustomEditValue(object value, PivotGridCellItem cellItem) {
			if(EventsImplementor == null)
				return value;
			return EventsImplementor.CustomEditValue(value, cellItem);
		}
		public bool IsFieldReadOnly(PivotGridField field) {
			if(field == null)
				return true;
			return field.Options.ReadOnly || PivotDataSource.IsFieldReadOnly(field);
		}
		public override CustomizationFormFields GetCustomizationFormFields() {
			return CustomizationForm != null && CustomizationForm.CustomizationFields != null ? CustomizationForm.CustomizationFields : base.GetCustomizationFormFields();
		}
		public override IList<AggregationLevel> GetAggregations(bool datasourceLevel) {
			if(formatRules == null)
				return new List<AggregationLevel>();
			return FormatRules.GetSummaries().Where((ruleInfo) => {
								  PivotGridFormatRule rule = FormatRules[ruleInfo.Column];
								  return rule != null && rule.IsValid;
					 })
					  .Select((b) =>
						  new {
							  DataField = FormatRules[b.Column].Measure,
							  DataIndex = GetFieldsByArea(PivotArea.DataArea, false).IndexOf(FormatRules[b.Column].Measure),
							  ApplySettings = FormatRules[b.Column].Settings as FormatRuleFieldIntersectionSettings,
							  FormataRule = FormatRules[b.Column],
							  Rule = FormatRules[b.Column].Rule,
							  SummaryType = b.SummaryType,
							  SummaryArgument = b.SummaryArgument
						  })
						  .Where((c) => c.DataIndex >= 0 && (datasourceLevel == (c.DataField.SummaryDisplayType == DevExpress.Data.PivotGrid.PivotSummaryDisplayType.Default)))
						  .GroupBy((c) =>
							  new {
								  Column = GetFieldsByArea(PivotArea.ColumnArea, false).IndexOf(c.ApplySettings.Column),
								  Row = GetFieldsByArea(PivotArea.RowArea, false).IndexOf(c.ApplySettings.Row)
							  })
							  .Select(
									(d) => new AggregationLevel(
										d.GroupBy((aa) => aa.DataIndex).Select((e) => new AggregationCalculation(
															e.Select((f) => new AggregationItemValue(
																						   f.SummaryType,
																						   f.SummaryArgument,
																						   (value) => f.FormataRule.ValueProvider.RuleState.SetValue(
																												   FormatRuleSummaryInfoCollection.QueryKindFromSummaryType(f.SummaryType).ToString(), value))),
															e.Key)
													  ),
										d.Key.Row,
										d.Key.Column)).ToList();
		}
	}
}
