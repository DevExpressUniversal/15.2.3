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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Details;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Tab;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using System.Collections.Generic;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Registrator;
namespace DevExpress.XtraGrid.Views.Base {
	public abstract class BaseViewPopupActivator : IDisposable {
		BaseView view;
		RepositoryItemPopupBase item;
		PopupEditActivator popupActivator;
		bool isInitialized = false;
		public BaseViewPopupActivator(BaseView view) {
			this.view = view;
			this.item = null;
		}
		public virtual void Dispose() {
			if(this.item != null) this.item.Dispose();
			if(this.popupActivator != null) {
				this.popupActivator.CloseUp -= new DevExpress.XtraEditors.Controls.CloseUpEventHandler(OnActivator_CloseUp);
				this.popupActivator.DestroyPopup();
				this.popupActivator.Dispose();
			}
		}
		public BaseView View { get { return view; } }
		protected abstract RepositoryItemPopupBase CreateRepositoryItem();
		protected virtual PopupEditActivator PopupActivator { get { return popupActivator; } }
		public RepositoryItemPopupBase Item { get { return item; } }
		protected virtual Control PopupOwner { get { return View.GridControl; } }
		protected bool IsInitialized { get { return isInitialized; } }
		public virtual void Init() {
			if(IsInitialized) return;
			this.isInitialized = true;
			this.item = CreateRepositoryItem();
			if(Item == null) return;
			this.popupActivator = new PopupEditActivator(View.WorkAsLookup, true);
			this.popupActivator.Owner = PopupOwner;
			this.popupActivator.PopupItem = Item;
			this.popupActivator.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(OnActivator_CloseUp);
		}
		public virtual bool CanShow { get { return Item != null; } }
		public virtual void Show(Rectangle ownerBounds) {
			if(!CanShow) return;
			PopupActivator.ShowPopup(ownerBounds);
		}
		protected virtual void OnActivator_CloseUp(object sender, CloseUpEventArgs e) {
		}
		public bool IsFocused { get { return PopupActivator.ContainsFocus; } }
	}
	public class MRUFilterPopup : BaseViewPopupActivator {
		class ViewFilterItem {
			public readonly string DisplayText;
			public readonly ViewFilter ViewFilter;
			public ViewFilterItem(string displayText, ViewFilter viewFilter) {
				this.ViewFilter = viewFilter;
				this.DisplayText = displayText;
			}
			public override string ToString() {
				return DisplayText;
			}
		}
		public MRUFilterPopup(ColumnView view) : base(view) {
		}
		public new ColumnView View { get { return base.View as ColumnView; } }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			if(View.MRUFilters.Count == 0) return null;
			RepositoryItemComboBox comboBox = new RepositoryItemMruFilterCombo(this);
			foreach(ViewFilter filter in View.MRUFilters) {
				if(Equals(filter.Criteria, View.ActiveFilter.Criteria)) continue;
				comboBox.Items.Add(new ViewFilterItem(View.GetFilterDisplayText(filter), filter));
			}
			if(comboBox.Items.Count == 0) return null;
			comboBox.DropDownRows = View.OptionsFilter.MRUFilterListPopupCount;
			comboBox.KeyDown += new KeyEventHandler(OnMruComboKeyDown);
			return comboBox;
		}
		class RepositoryItemMruFilterCombo : RepositoryItemComboBox {
			MRUFilterPopup filter;
			public RepositoryItemMruFilterCombo(MRUFilterPopup filter) {
				this.filter = filter;
			}
			public override BaseEdit CreateEditor() {
				return new MruFilterCombo(filter);
			}
		}
		class MruFilterCombo : ComboBoxEdit {
			MRUFilterPopup filter;
			public MruFilterCombo(MRUFilterPopup filter) {
				this.filter = filter;
			}
			protected override void OnActionItemClick(ListItemActionInfo action) {
				ViewFilterItem item = action.Item as ViewFilterItem;
				if(!SilentRemove(item)) return;
				filter.View.MRUFilters.Remove(item.ViewFilter);
				if(IsPopupOpen) RefreshPopup();
				if(!CanShowPopup) ClosePopup(PopupCloseMode.Cancel);
			}
			protected override bool HasItemActions { get { return true; } }
			protected override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
				itemInfo.ActionInfo = new ListItemActionCollection();
				itemInfo.ActionInfo.Add(new ListItemDeleteActionInfo(itemInfo));
			}
		}
		void OnMruComboKeyDown(object sender, KeyEventArgs e) {
			IPopupControl pc = sender as IPopupControl;
			PopupListBoxForm popupForm = pc == null ? null : pc.PopupWindow as PopupListBoxForm;
			if(e.KeyData == Keys.Delete) {
				ComboBoxEdit combo = sender as ComboBoxEdit;
				if(popupForm != null && popupForm.ListBox != null && popupForm.ListBox.SelectedItem is ViewFilterItem) {
					ViewFilterItem vi = popupForm.ListBox.SelectedItem as ViewFilterItem;
					PopupActivator.CancelPopup();
					try {
						View.MRUFilters.Remove(vi.ViewFilter);
					}
					catch { }
				}
			}
		}
		protected override void OnActivator_CloseUp(object sender, CloseUpEventArgs e) {
			if(e.AcceptValue && (e.Value is ViewFilterItem)) {
				View.ActiveFilter.Assign(((ViewFilterItem)e.Value).ViewFilter, null);
			}
			if(View != null)
				View.MRUFilterPopup = null;
		}
	}
	public abstract class FilterPopup : BaseViewPopupActivator {
		GridColumn column;
		Control owner;
		object creator;
		bool hasBlank = false;
		public FilterPopup(ColumnView view, GridColumn column, Control owner, object creator) : base(view) {
			this.owner = owner;
			this.column = column;
			this.creator = creator;
		}
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			return null;
		}
		public virtual void ShowPopup(Rectangle ownerBounds) {
			Show(ownerBounds);
		}
		internal bool HasBlank { get { return hasBlank; } set { hasBlank = value; } }
		protected override Control PopupOwner { get { return owner; } }
		public object Creator { get { return creator; } set { creator = value; } }
		public new ColumnView View { get { return base.View as ColumnView; } }
		public virtual GridColumn Column { get { return column; } }
		public virtual void InitData(object[] values) {
			bool smartUpdate = IsInitialized;
			if(smartUpdate) {
				Item.BeginUpdate();
				ResetItems();
			}
			Init();
			InitMRUItems();
			InitDefaultItems();
			if(values == null) return;
			InitValues(values);
			if(smartUpdate) {
				Item.CancelUpdate();
				RefreshOpenedPopup();
			}
		}
		protected virtual void RefreshOpenedPopup() {
			if(PopupActivator != null)
				PopupActivator.RefreshPopup();
		}
		protected virtual void ResetItems() {
		}
		protected virtual void InitValues(object[] values) {
			BaseEditViewInfo bev = View.CreateColumnEditViewInfo(column, GridControl.InvalidRowHandle, true);
			Item.BeginUpdate();
			try {
				if(values.Length == 1 && AsyncServerModeDataController.IsNoValue(values[0])) {
					AddLoadingItem();
				}
				else {
					for(int n = 0; n < values.Length; n++) {
						object obj = values[n];
						if(obj == null) continue;
						FilterItem item;
						if(bev != null && Column.GetFilterMode() == ColumnFilterMode.Value) {
							View.ViewInfo.UpdateEditViewInfo(bev, column, DevExpress.Data.DataController.InvalidRow);
							bev.EditValue = obj;
							bev.SetDisplayText(View.RaiseCustomColumnDisplayText(DevExpress.Data.DataController.InvalidRow, column, obj, bev.DisplayText, false));
							item = new FilterItem(bev.DisplayText, obj);
						}
						else
							item = new FilterItem(obj.ToString(), obj);
						values[n] = item;
					}
					if(column.GetFilterMode() == ColumnFilterMode.DisplayText || column.GetSortMode() == ColumnSortMode.DisplayText) {
						Array.Sort(values, new FilterItemComparer());
					}
					AddItems(values);
				}
			}
			finally {
				Item.EndUpdate();
			}
		}
		protected virtual void InitMRUItems() { }
		protected virtual void InitDefaultItems() { }
		public virtual void AddItems(object[] values) { }
		protected virtual void AddLoadingItem() { }
		internal static bool IsNullValue(object obj) {
			return obj == null || obj == DBNull.Value;
		}
		protected virtual bool AllowBlankItems {
			get {
				return Column.OptionsFilter.ShowBlanksFilterItems != DefaultBoolean.False;
			}
		}
		protected virtual bool ForceAddBlankItems {
			get {
				return Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.True;
			}
		}
	}
	public class DateFilterPopup : FilterPopup, IPopupOutlookDateFilterOwner {
		public DateFilterPopup(ColumnView view, GridColumn column, Control owner, object creator)
			: base(view, column, owner, creator) { 
		}
		DateFilterInfoCache cache = null;
		bool isAsyncLoadingState = false;
		protected internal bool IsAsyncLoadingState { get { return isAsyncLoadingState; } }
		public override void InitData(object[] values) {
		   this.isAsyncLoadingState = false;
			if(values != null && values.Length == 1 && AsyncServerModeDataController.IsNoValue(values[0])) {
				this.isAsyncLoadingState = true;
			}
			ColumnDateFilterInfo info = View.GetPrevDateFilterInfo(Column);
			if(info != null && info.Cache != null) {
				this.cache = info.Cache;
			}
			else {
				this.cache = new DateFilterInfoCache();
				this.cache.Init(values);
			}
			base.InitData(values);
		}
		protected override void RefreshOpenedPopup() {
			RepositoryItemDateFilterPopupEdit item = Item as RepositoryItemDateFilterPopupEdit;
			if(item != null) {
				item.popupFilter.Cache = cache;
				item.Init(View.ActiveFilter[Column], View.GetPrevDateFilterInfo(Column), true);
			}
			base.RefreshOpenedPopup();
		}
		protected override void InitValues(object[] values) { }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			RepositoryItemDateFilterPopupEdit item = new RepositoryItemDateFilterPopupEdit(this);
			item.popupFilter.Field = new GridFilterColumn(Column);
			item.popupFilter.UseAltFilter = Column.GetFilterPopupMode() == FilterPopupMode.DateAlt;
			item.popupFilter.ShowEmptyFilter = Column.OptionsFilter.ShowEmptyDateFilter;
			item.popupFilter.Cache = this.cache;
			item.LookAndFeel.Assign(View.ElementsLookAndFeel);
			item.Init(GetActiveFilter(), View.GetPrevDateFilterInfo(Column), false);
			return item;
		}
		ViewColumnFilterInfo GetActiveFilter() { 
			ViewColumnFilterInfo vfi = View.ActiveFilter[Column];
			if(vfi == null)
				vfi = View.ActiveFilter[base.Column];
			return vfi;
		}
		[ToolboxItem(false)]
		class RepositoryItemDateFilterPopupEdit : RepositoryItemPopupContainerEdit {
			PopupContainerControl containerControl = new PopupContainerControl();
			DateFilterPopup owner;
			LoadingAnimator loadingAnimator;
			internal PopupOutlookDateFilterControl popupFilter;
			public RepositoryItemDateFilterPopupEdit(DateFilterPopup owner) {
				this.owner = owner;
				this.popupFilter = new PopupOutlookDateFilterControl(owner);
				PopupSizeable = false;
				CloseActAsOkButton = true;
			}
			public override BaseEdit CreateEditor() {
				PopupContainerEdit edit = base.CreateEditor() as PopupContainerEdit;
				this.popupFilter.OwnerEdit = edit;
				return edit;
			}
			protected LoadingAnimator LoadingAnimator {
				get {
					if(loadingAnimator == null) loadingAnimator = new LoadingAnimator(containerControl, DevExpress.XtraEditors.Drawing.LoadingAnimator.LoadingImageLine);
					return loadingAnimator;
				}
			}
			public void Init(ViewColumnFilterInfo activeFilter, ColumnDateFilterInfo prevFilter, bool update) {
				this.originalClientSize = Size.Empty;
				CriteriaOperator currentFilter = null;
				if(activeFilter != null && activeFilter.Filter != null) currentFilter = activeFilter.Filter.FilterCriteria;
				if(update) containerControl.Controls.Clear();
				popupFilter.ElementsLookAndFeel = LookAndFeel;
				popupFilter.Init(prevFilter == null ? null : prevFilter.LastFilter, currentFilter);
				popupFilter.CreateControls();
				popupFilter.Dock = DockStyle.Fill;
				PopupControl = containerControl;
				if(owner.IsAsyncLoadingState) {
					containerControl.Paint += new PaintEventHandler(containerControl_Paint);
				}
				else {
					if(loadingAnimator != null) loadingAnimator.StopAnimation();
					containerControl.Paint -= new PaintEventHandler(containerControl_Paint);
					containerControl.ClientSize = popupFilter.Size;
					containerControl.Controls.Add(popupFilter);
				}
				if(!update) 
					containerControl.ClientSizeChanged += new EventHandler(OnContainerControlClientSizeChanged);
				this.originalClientSize = containerControl.ClientSize;
			}
			void containerControl_Paint(object sender, PaintEventArgs e) {
				using(GraphicsCache cache = new GraphicsCache(e)) {
					LoadingAnimator.DrawAnimatedItem(cache, containerControl.ClientRectangle);
				}
			}
			Size originalClientSize = Size.Empty;
			void OnContainerControlClientSizeChanged(object sender, EventArgs e) {
				Control control = ((Control)sender);
				if(control.FindForm() != null && control.ClientSize.Width < originalClientSize.Width) {
					control.FindForm().MinimumSize = new Size(originalClientSize.Width + SystemInformation.VerticalScrollBarWidth * 2, 50);
				}
			}
			public DateFilterResult FinalizeResult() {
				if(popupFilter.Result != null) return popupFilter.Result;
				popupFilter.ApplyFilter();
				return popupFilter.Result;
			}
			public DateFilterResult CalcResult() {
				return popupFilter.CalcFilterResult();
			}
			protected override void Dispose(bool disposing) {
				if(loadingAnimator != null) loadingAnimator.Dispose();
				popupFilter.Dispose();
				containerControl.Dispose();
				base.Dispose(disposing);
			}
		}
		protected override void OnActivator_CloseUp(object sender, CloseUpEventArgs e) {
			base.OnActivator_CloseUp(sender, e);
			if(Column != null) {
				if(e.AcceptValue) { 
					ApplyFilter(true);
				}
			}
			View.OnFilterPopupCloseUp(Column);
			View.DestroyFilterPopup();
		}
		public override GridColumn Column {
			get {
				return View.GetColumnFieldNameSortGroup(base.Column);
			}
		}
		void ApplyFilter(bool final) {
			RepositoryItemDateFilterPopupEdit item = (RepositoryItemDateFilterPopupEdit)Item;
			DateFilterResult result = final ? item.FinalizeResult() : item.CalcResult();
			if(result == null) {
				if(!final) 
					result = new DateFilterResult();
				else
					return;
			}
			GridColumn column = View.GetParentColumnFieldNameSortGroup(Column);
			if(result.FilterType == FilterDateType.None) {
				View.ActiveFilter.Remove(column);
			}
			else {
				ColumnFilterInfo colFilter = new ColumnFilterInfo(ColumnFilterType.Custom, null, result.FilterCriteria, result.FilterDisplayText);
				View.ActiveFilter.Add(column, colFilter);
			}
			if(!IsAsyncLoadingState)
				View.OnDateFilterChanged(new ColumnDateFilterInfo(column, cache, result));
		}
		bool GetAllowUpdateFilterOnCheck() {
			if(Column.OptionsFilter.ImmediateUpdatePopupDateFilterOnCheck == DefaultBoolean.False) return false;
			if(Column.OptionsFilter.ImmediateUpdatePopupDateFilterOnCheck == DefaultBoolean.True) return true;
			if(View.IsAsyncServerMode) return true;
			if(View.DataController.ListSourceRowCount > 10000) return false;
			return true;
		}
		bool GetAllowUpdateFilterOnDateChanged() {
			if(Column.OptionsFilter.ImmediateUpdatePopupDateFilterOnDateChange == DefaultBoolean.False) return false;
			if(Column.OptionsFilter.ImmediateUpdatePopupDateFilterOnDateChange == DefaultBoolean.True) return true;
			if(View.IsServerMode) {
				if(View.IsAsyncServerMode) return true;
				return false;
			}
			if(View.DataController.ListSourceRowCount > 500) return false;
			return true;
		}
		void IPopupOutlookDateFilterOwner.OnCheckedChanged() {
			if(GetAllowUpdateFilterOnCheck())
				ApplyFilter(false);
		}
		void IPopupOutlookDateFilterOwner.RaiseFilterListUpdate(List<FilterDateElement> list) {
			View.RaiseFilterPopupDate(this, list);
		}
		void IPopupOutlookDateFilterOwner.OnDateModified() {
			if(GetAllowUpdateFilterOnDateChanged())
				ApplyFilter(false);
		}
	}
	public class ColumnDateFilterInfo {
		public GridColumn Column;
		public DateFilterInfoCache Cache;
		public DateFilterResult LastFilter;
		public ColumnDateFilterInfo(GridColumn column, DateFilterInfoCache cache, DateFilterResult lastFilter) {
			this.Column = column;
			this.Cache = cache;
			this.LastFilter = lastFilter;
		}
	}
	public class CheckedColumnFilterPopup : FilterPopup {
		[ToolboxItem(false)]
		public class RepositoryItemFilterComboBox : RepositoryItemCheckedComboBoxEdit {
			static RepositoryItemFilterComboBox() { RegisterFilterCheckedComboBoxEdit(); }
			internal static void RegisterFilterCheckedComboBoxEdit() {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("GridFilterCheckedComboBoxEdit", typeof(FilterComboBox), typeof(RepositoryItemFilterComboBox), typeof(DevExpress.XtraEditors.ViewInfo.PopupContainerEditViewInfo), new ButtonEditPainter(), false, null, typeof(DevExpress.Accessibility.ComboBoxEditAccessible)));
			}
			public override string EditorTypeName { get { return "GridFilterCheckedComboBoxEdit"; } }
			bool isListBoxModified = false;
			public RepositoryItemFilterComboBox() : this(false, false) { }
			public RepositoryItemFilterComboBox(bool incrementalSearch, bool allowMultiSelect) {
				this.SeparatorChar = '\n';
				DropDownRows = 15;
				this.IncrementalSearch = incrementalSearch;
				this.AllowMultiSelect = allowMultiSelect;
			}
			string values = string.Empty;
			protected override bool IgnoreSeparatorSpace { get { return true; }}
			public override Size PopupFormSize {
				get {
					object size = this.PropertyStore["BlobSize"];
					if(size == null) return Size.Empty;
					return (Size)size;
				}
				set {
					this.PropertyStore["BlobSize"] = value;
				}
			}
			public string FilterValues {
				get { return values; }
				set { 
					values = value;
					if(OwnerEdit != null) OwnerEdit.EditValue = FilterValues;
				}
			}
			public void Sync() {
				SynchronizeItemsWithEditValue();
			}
			public override BaseEdit CreateEditor() {
				BaseEdit edit = new FilterComboBox();
				edit.EditValue = FilterValues;
				return edit;
			}
			internal bool IsListBoxModified {
				get { return isListBoxModified; }
				set {
					if(isListBoxModified != value)
						isListBoxModified = value;
				}
			}
		}
		[ToolboxItem(false)]
		public class FilterComboBox : CheckedComboBoxEdit {
			public override string EditorTypeName { get { return "GridFilterCheckedComboBoxEdit"; } }
			public new RepositoryItemFilterComboBox Properties {
				get { return base.Properties as RepositoryItemFilterComboBox; }
			}
			protected override PopupBaseForm CreatePopupForm() {
				if(Properties.PopupControl == null) return null;
				return new FilterCheckedPopupContainerForm(this);
			}
			protected override void WndProc(ref Message msg) {
				base.WndProc(ref msg);
				CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
			}
			protected override void OnListBoxKeyDown(object sender, KeyEventArgs e) {
				if(e.KeyCode == Keys.Enter) {
					Properties.IsListBoxModified = true;
					DoClosePopup(XtraEditors.PopupCloseMode.Normal);
				} else base.OnListBoxKeyDown(sender, e);
			}
			protected override void DoShowPopup() {
				base.DoShowPopup();
				Properties.IsListBoxModified = false;
			}
			protected override void OnItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
				base.OnItemCheck(sender, e);
				Properties.IsListBoxModified = true;
			}
		}
		[ToolboxItem(false)]
		public class FilterCheckedPopupContainerForm : CheckedPopupContainerForm {
			public FilterCheckedPopupContainerForm(PopupContainerEdit ownerEdit)
				: base(ownerEdit) {
			}
			public override void HidePopupForm() {
				Size tempSize = ViewInfo.Bounds.Size;
				base.HidePopupForm();
				if(FormResized)
					SetPropertyStore(tempSize);
			}
			RepositoryItemFilterComboBox FilterProperties {
				get {
					FilterComboBox edit = this.OwnerEdit as FilterComboBox;
					if(edit != null) return edit.Properties;
					return null;
				}
			}
			protected override void OnOkButtonClick() {
				if(FilterProperties != null) 
					FilterProperties.IsListBoxModified = true;
				base.OnOkButtonClick();
			}
	}
		public CheckedColumnFilterPopup(ColumnView view, GridColumn column, Control owner, object creator) : base(view, column, owner, creator) { }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			RepositoryItemFilterComboBox comboBox = new RepositoryItemFilterComboBox(this.View.OptionsFilter.AllowFilterIncrementalSearch, this.View.OptionsFilter.AllowMultiSelectInCheckedFilterPopup);
			comboBox.LookAndFeel.Assign(View.ElementsLookAndFeel);
			return comboBox;
		}
		public new RepositoryItemFilterComboBox Item { get { return base.Item as RepositoryItemFilterComboBox; } }
		public override void ShowPopup(Rectangle ownerBounds) {
			Size columnCheckedFilterSize = Column.CheckedFilterPopupSize;
			if(Item != null) {
				if(columnCheckedFilterSize != Size.Empty)
					Item.PopupFormSize = columnCheckedFilterSize;
			}
			base.ShowPopup(ownerBounds);
		}
		static string GetStringByValue(IEnumerable items, CriteriaOperator op) {
			OperandValue value = op as OperandValue;
			if(ReferenceEquals(value, null))
				return null;
			foreach(object obj in items) {
				FilterItem item = obj as FilterItem;
				if(item == null)
					continue;
				if(Equals(item.Value, value.Value)) {
					return item.Text;
				}
			}
			return null;
		}
		static List<DateTime> GetDateTimeList(IEnumerable items) {
			List<DateTime> ret = new List<DateTime>();
			foreach(object obj in items) {
				FilterItem item = obj as FilterItem;
				if(item == null || !(item.Value is DateTime))
					continue;
				ret.Add(Convert.ToDateTime(item.Value));
			}
			return ret;
		}
		static string DateTimeToString(DateTime dt, GridColumn column) {
			if(column.DisplayFormat.FormatType != FormatType.DateTime || string.IsNullOrEmpty(column.DisplayFormat.FormatString))
			return string.Format("{0:d}", dt);
			return string.Format(column.DisplayFormat.GetFormatString(), dt);
		}
		internal static bool AllowCheckedDateList(GridColumn column) {
			return column.ColumnType == typeof(DateTime) && column.RealColumnEdit is RepositoryItemDateEdit && column.FilterMode == ColumnFilterMode.Value;
		}
		static string[] GetStringsByCriteria(IEnumerable items, CriteriaOperator op, GridColumn column) {
			if(AllowCheckedDateList(column) && !ReferenceEquals(op, null)) {
				IEnumerable<DateTime> list = MultiselectRoundedDateTimeFilterHelper.GetCheckedDates(op, column.FieldName, GetDateTimeList(items));
				return list.Select(dt => DateTimeToString(dt, column)).ToArray();
			} else {
				ArrayList rv = new ArrayList();
				if(ReferenceEquals(op, null))
					return new string[0];
				GroupOperator grop = op as GroupOperator;
				if(!ReferenceEquals(grop, null)) {
					if(grop.OperatorType != GroupOperatorType.Or && grop.Operands.Count > 1)
						return null;
					foreach(CriteriaOperator nop in grop.Operands) {
						IList nres = GetStringsByCriteria(items, nop, column);
						if(nres == null)
							return null;
						rv.AddRange(nres);
					}
				} else {
					BinaryOperator bop = op as BinaryOperator;
					if(!ReferenceEquals(bop, null)) {
						if(bop.OperatorType != BinaryOperatorType.Equal)
							return null;
						string toAdd = GetStringByValue(items, bop.RightOperand);
						if(toAdd != null)
							rv.Add(toAdd);
					} else {
						InOperator iop = op as InOperator;
						if(!ReferenceEquals(iop, null)) {
							foreach(CriteriaOperator rop in iop.Operands) {
								string toAdd = GetStringByValue(items, rop);
								if(toAdd != null)
									rv.Add(toAdd);
							}
						} else
							return null;
					}
				}
				return (string[])rv.ToArray(typeof(string));
			}
		}
		protected override void ResetItems() {
			Item.Items.Clear();
		}
		protected override void AddLoadingItem() {
			Item.Items.Add(AsyncServerModeDataController.NoValue);
		}
		public override void AddItems(object[] values) {
			if(ForceAddBlankItems && !HasBlank) {
				AddBlankItem();
				HasBlank = true;
			}
			foreach(object obj in values) {
				FilterItem item = obj as FilterItem;
				if(item == null) continue;
				if(!AllowBlankItems && IsNullValue(item.Value)) continue;
				if(AllowBlankItems && (IsNullValue(item.Value) || string.Empty.Equals(item.Value))) {
					if(!HasBlank)
						AddBlankItem();
					HasBlank = true;
					continue;
				}
				Item.Items.Add(item);
			}
			CriteriaOperator op = null;
			bool hasNull = IsNullOrEmptyEliminator.Eliminate(Column.FilterInfo.FilterCriteria, Column.FieldName, out op);
			string[] filterValues = GetStringsByCriteria(values, hasNull ? op : Column.FilterInfo.FilterCriteria, Column);
			if(filterValues != null && filterValues.Length > 0)
				Item.FilterValues = string.Join("\n", filterValues); 
			else Item.FilterValues = "<XtraFilter: empty_filter_string>"; 
			if(hasNull) Item.FilterValues += string.Format("\n{0}", GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterBlanks));
			Item.Sync();
		}
		void AddBlankItem() {
			Item.Items.Add(new FilterItem(GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterBlanks), null));
		}
		protected override void OnActivator_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
			List<Object> checkedValues = Item.Items.GetCheckedValues();
			View.OnFilterPopupCloseUp(Column);
			View.DestroyFilterPopup();
			base.OnActivator_CloseUp(sender, e);
			if(Column != null) {
				Size popupSize = Item.PopupFormSize;
				if(e.AcceptValue && e.CloseMode != PopupCloseMode.Immediate && Item.IsListBoxModified)
					View.ApplyCheckedColumnFilter(Column, e.Value, checkedValues);
				Column.UpdateCheckedFilterInfo(popupSize);
			}
		}
	}
	public class ColumnFilterPopup : FilterPopup {
		internal const string MRUDivider = "-";
		[ToolboxItem(false)]
		public class RepositoryItemFilterComboBox : RepositoryItemComboBox {
			ColumnFilterPopup columnFilter;
			public RepositoryItemFilterComboBox(ColumnFilterPopup columnFilter) {
				this.columnFilter = columnFilter;
				this.PopupFormMinSize = new Size(100, 25);
			}
			public override Size PopupFormSize {
				get {
					object size = this.PropertyStore["ComboPopupSize"];
					if(size == null) return Size.Empty;
					return (Size)size;
				} 
				set {
					this.PropertyStore["ComboPopupSize"] = value;
				}
			}
			public override BaseEdit CreateEditor() {
				return new FilterComboBox(columnFilter);
			}
		}
		[ToolboxItem(false)]
		public class FilterComboBox : ComboBoxEdit {
			ColumnFilterPopup columnFilter;
			public FilterComboBox(ColumnFilterPopup columnFilter) {
				this.columnFilter = columnFilter;
			}
			protected override void RaiseDropDownCustomDrawItem(ListBoxDrawItemEventArgs e) {
				base.RaiseDropDownCustomDrawItem(e);
				if(e.Handled) return;
				if(object.Equals(e.Item, MRUDivider)) {
					DrawMruDivider(e);
					return;
				}
				CheckHighlightPopupAutoSearchText(e);
			}
			bool IncrementalSearch { get { return columnFilter.View.OptionsFilter.AllowFilterIncrementalSearch; } }
			protected override void ProcessFindItem(KeyPressHelper helper, char pressedKey) {
				if(IncrementalSearch)
					base.ProcessFindItem(helper, pressedKey);
				else {
					if(PopupForm == null) return;
					int index = PopupForm.ListBox.FindString(AutoSearchText, PopupForm.ListBox.SelectedIndex + 1);
					if(index < 0) index = PopupForm.ListBox.FindString(AutoSearchText, -1);
					PopupForm.ListBox.SelectedIndex = index;
					AutoSearchText = string.Empty;
				}
			}
			protected override void OnPopupShown() {
				base.OnPopupShown();
				if(PopupForm != null) PopupForm.ListBox.HotTrackSelectMode = HotTrackSelectMode.SelectItemOnHotTrackEx;
			}
			protected override void OnPopupSelectedIndexChanged() {
				AutoSearchText = string.Empty;
				base.OnPopupSelectedIndexChanged();
			}
			void DrawMruDivider(ListBoxDrawItemEventArgs e) {
				Rectangle r = e.Bounds;
				r.Inflate(-1, 0);
				r.Height = 1;
				r.Y += e.Bounds.Height / 2 - 2;
				Color clr = SystemColors.GrayText;
				e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(clr), r);
				r.Y += 2;
				e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(clr), r);
				e.Handled = true;
			}
			protected override void OnActionItemClick(ListItemActionInfo action) {
				if(!SilentRemove(action.Item)) return;
				if(Properties.Items.Count > 0 && Object.Equals(Properties.Items[0], MRUDivider)) {
					SilentRemove(MRUDivider);
				}
				FilterItem item = action.Item as FilterItem;
				if(item == null) return;
				ColumnFilterInfo fi = item.Value as ColumnFilterInfo;
				columnFilter.Column.MRUFilters.Remove(fi);
				if(IsPopupOpen) RefreshPopup();
				if(!CanShowPopup) ClosePopup(PopupCloseMode.Cancel);
			}
			protected override bool HasItemActions { get { return true; } }
			protected override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
				FilterItem item = itemInfo.Item as FilterItem;
				if(item == null) return;
				ColumnFilterInfo fi = item.Value as ColumnFilterInfo;
				if(columnFilter.Column.MRUFilters.Contains(fi)) {
					itemInfo.ActionInfo = new ListItemActionCollection();
					itemInfo.ActionInfo.Add(new ListItemDeleteActionInfo(itemInfo));
				}
			}
			protected override void WndProc(ref Message msg) {
				base.WndProc(ref msg);
				CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
			}
		}
		public ColumnFilterPopup(ColumnView view, GridColumn column, Control owner, object creator) : base(view, column, owner, creator) { 
		}
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			RepositoryItemFilterComboBox comboBox = new RepositoryItemFilterComboBox(this);
			comboBox.LookAndFeel.Assign(View.ElementsLookAndFeel);
			comboBox.PopupSizeable = true;
			comboBox.DropDownRows = Math.Max(2, View.OptionsFilter.ColumnFilterPopupRowCount);
			return comboBox;
		}
		public new RepositoryItemFilterComboBox Item { get { return base.Item as RepositoryItemFilterComboBox; } }
		public override void ShowPopup(Rectangle ownerBounds) {
			int columnFilterWidth = Column.FilterPopupWidth;
			if(Item != null) {
				if(columnFilterWidth > 0) 
					Item.PopupFormSize = new Size(columnFilterWidth, 0);
			}
			base.ShowPopup(ownerBounds);
		}
		protected override void InitMRUItems() {
			if(!View.OptionsFilter.AllowColumnMRUFilterList) return;
			ColumnFilterInfo active = Column.FilterInfo;
			int count = 0;
			foreach(ColumnFilterInfo mru in Column.MRUFilters) {
				if(mru.Equals(active)) continue;
				string dispText;
				if(mru.Value != null) {
					dispText = Column.View.GetFilterDisplayTextByColumn(Column, mru.Value);
				} else if(mru.DisplayText != null && mru.DisplayText.Length > 0) {
					dispText = mru.DisplayText;
				} else {
					dispText = Column.View.GetFilterDisplayText(mru.FilterCriteria);
				}
				Item.Items.Add(new FilterItem(dispText, mru));
				if(++count >= View.OptionsFilter.MRUColumnFilterListCount) break;
			}
			if(count > 0) Item.Items.Add(MRUDivider);
		}
		public override void InitData(object[] values) {
			if(AllowBlankItems) {
				if(View.IsServerMode || values == null || Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.True) HasBlank = true;
				else {
					foreach(object obj in values) {
						if(IsNullValue(obj)) {
							HasBlank = true;
							break;
						}
					}
				}
			} else HasBlank = false;
			base.InitData(values);
		}
		protected override void InitDefaultItems() {
			if(Column.FilterInfo.Type != ColumnFilterType.None)
				Item.Items.Add(new FilterItem(GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterAll), new FilterItem("", 0)));
			Item.Items.Add(new FilterItem(GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterCustom), new FilterItem("", 1)));
			if(HasBlank) {
				Item.Items.Add(new FilterItem(GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterBlanks), new FilterItem("", 2)));
				Item.Items.Add(new FilterItem(GridLocalizer.Active.GetLocalizedString(GridStringId.PopupFilterNonBlanks), new FilterItem("", 3)));
			}
		}
		protected override void ResetItems() {
			Item.Items.Clear();
		}
		public override void AddItems(object[] values) {
			foreach(object obj in values) {
				FilterItem item = obj as FilterItem;
				if(item == null || IsNullValue(item.Value)) continue;
				Item.Items.Add(item);
			}
		}
		protected override void AddLoadingItem() {
			Item.Items.Add(AsyncServerModeDataController.NoValue);
		}
		protected override void OnActivator_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
			View.OnFilterPopupCloseUp(Column);
			View.DestroyFilterPopup();
			base.OnActivator_CloseUp(sender, e);
			if(Column != null && Column.View != null) {
				int popupWidth = Item.PopupFormSize.Width;
				if(e.AcceptValue && e.CloseMode != PopupCloseMode.Immediate)
					View.ApplyColumnFilter(Column, e.Value as FilterItem);
				Column.UpdateFilterInfo(popupWidth);
			}
		}
	}
	class FilterItemComparer : IComparer {
		int IComparer.Compare(object a, object b) {
			FilterItem item1 = a as FilterItem, item2 = b as FilterItem;
			if(item1 == item2) return 0;
			if(item1 == null) return -1;
			if(item2 == null) return 1;
			return Comparer.Default.Compare(item1.Text, item2.Text);
		}
	}
	[Serializable, TypeConverter(typeof(DevExpress.Utils.Design.BinaryTypeConverter))]
	public class ViewColumnFilterInfo : ISerializable {
		[NonSerialized]
		internal ViewFilter viewFilter = null;
		[NonSerialized]
		GridColumn column;
		[NonSerialized]
		ColumnFilterInfo filter;
		[NonSerialized]
		string columnName;
		public ViewColumnFilterInfo(GridColumn column, ColumnFilterInfo filter) {
			this.column = column;
			this.filter = filter;
		}
		public GridColumn Column { get { return column; } }
		public ColumnFilterInfo Filter { 
			get { return filter; } 
			set {
				if(value == null || value.Equals(ColumnFilterInfo.Empty)) value = ColumnFilterInfo.Empty;
				if(Filter == value || Filter.Equals(value)) return;
				filter = value;
				OnFilterChanged();
			}
		}
		public bool IsEmpty { get { return Column == null || Filter.Type == ColumnFilterType.None; } }
		protected internal ViewFilter ViewFilter { get { return viewFilter; } }
		protected void OnFilterChanged() {
			if(ViewFilter != null) ViewFilter.OnFilterChanged(this);
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			si.AddValue("Filter", Filter);
			si.AddValue("Column", Column == null ? this.columnName : Column.Name);
			si.AssemblyName = this.GetType().Assembly.GetName().Name;
		}
		internal bool RestoreColumn(GridColumnCollection columns) {
			if(this.columnName == null || this.columnName == string.Empty || columns == null) return false;
			GridColumn col = columns.ColumnByName(this.columnName);
			if(col != null) {
				this.columnName = string.Empty;
				this.column = col;
				return true;
			}
			return false;
		}
		internal ViewColumnFilterInfo(SerializationInfo si, StreamingContext context) : this(null, null) {
			foreach (SerializationEntry entry in si) {
				switch (entry.Name) {
					case "Filter":
						this.filter = (ColumnFilterInfo)entry.Value;
						break;
					case "Column":
						this.column = null;
						this.columnName = (string)entry.Value;
						break;
				}
			}
		}
	}
	[Serializable, ListBindable(false)]
	public class ViewFilter : CollectionBase, ICloneable {
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterIsEmpty")]
#endif
		public bool IsEmpty { get { return Count == 0 && ReferenceEquals(NonColumnFilterCriteria, null); } }
		CriteriaOperator fNonColumnFilterCriteria;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ViewFilterNonColumnFilterCriteria"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public CriteriaOperator NonColumnFilterCriteria {
			get {
				return fNonColumnFilterCriteria;
			}
			set {
				if(ReferenceEquals(value, NonColumnFilterCriteria))
					return;
				fNonColumnFilterCriteria = value;
				OnViewFilterChanged();
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterNonColumnFilter")]
#endif
		public string NonColumnFilter {
			get {
				return CriteriaOperator.ToString(NonColumnFilterCriteria);
			}
			set {
				NonColumnFilterCriteria = CriteriaOperator.TryParse(value);
			}
		}
		[NonSerialized]
		int lockUpdate = 0;
		[NonSerialized]
		EventHandler changed;
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterChanged")]
#endif
		public event EventHandler Changed { 
			add { changed += value; }
			remove { changed -= value; }
		}
		public ViewFilter() {
		}
		public object Clone() {
			ViewFilter res = new ViewFilter();
			res.Assign(this, null);
			return res;
		}
		public void BeginUpdate() { lockUpdate ++; }
		internal void CancelUpdate() { lockUpdate --; }
		public void EndUpdate() {
			if(--this.lockUpdate == 0) 
				OnViewFilterChanged();
		}
		public void AddRange(ViewColumnFilterInfo[] info) {
			foreach(ViewColumnFilterInfo fi in info) { Add(fi); }
		}
		public int Add(ViewColumnFilterInfo info) {
			if(info == null || info.IsEmpty) return -1;
			try {
				return List.Add(info);
			} catch(ArgumentOutOfRangeException) {
				return -1;
			}
		}
		public int Add(GridColumn column, ColumnFilterInfo filter) {
			ViewColumnFilterInfo info = this[column];
			try {
				filter.UpdateValueFilterIfNeeded(column);
				if(info == null) {
					if(filter.Type == ColumnFilterType.None) return -1;
					return List.Add(new ViewColumnFilterInfo(column, filter));
				}
				info.Filter = filter;
			} catch(ArgumentOutOfRangeException) {
				return -1;
			}
			return IndexOf(info);
		}
		public void Remove(GridColumn column) {
			ViewColumnFilterInfo info = this[column];
			if(info != null) Remove(info);
		}
		public int IndexOf(ViewColumnFilterInfo info) { return List.IndexOf(info); }
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterItem")]
#endif
		public ViewColumnFilterInfo this[GridColumn column] {
			get {
				for(int n = Count - 1; n >= 0; n --) {
					if(this[n].Column == column) return this[n];
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterExpression")]
#endif
		public string Expression {
			get {
				return CriteriaOperator.ToString(Criteria);
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterFilterExpression")]
#endif
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Criteria property instead")]
		public CriteriaOperator FilterExpression {
			get {
				return this.Criteria;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterCriteria")]
#endif
		public CriteriaOperator Criteria {
			get {
				CriteriaOperator totalFilter = NonColumnFilterCriteria;
				for(int i = 0; i < Count; ++i) {
					totalFilter = GroupOperator.And(totalFilter, this[i].Filter.FilterCriteria);
				}
				return totalFilter;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterDisplayText")]
#endif
		[Obsolete("Use the ColumnView.FilterPanelText property or ColumnView.GetFilterDisplayText method instead")]
		public string DisplayText {
			get {
				string displayText = NonColumnFilter;
				if(Count == 0) return displayText;
				foreach(ViewColumnFilterInfo info in this) {
					if(displayText != "") displayText += " AND ";
					displayText += string.Format("({0})", info.Filter.GetDisplayText());
				}
				return displayText;
			}
		}
		protected internal void OnDeserialize(GridColumnCollection columns) {
			for(int n = Count - 1; n >= 0; n--) {
				if(!this[n].RestoreColumn(columns)) RemoveAt(n);
			}
		}
		protected internal void Assign(ViewFilter filter, GridColumnCollection columns) {
			Assign(filter, columns, true);
		}
		protected internal void Assign(ViewFilter filter, GridColumnCollection columns, bool allowClone) {
			if(filter.IsEmpty && this.IsEmpty)
				return;
			ViewFilter clone = null;
			if(allowClone) clone = Clone() as ViewFilter;
			BeginUpdate();
			Clear();
			this.fNonColumnFilterCriteria = filter.NonColumnFilterCriteria;
			try {
				foreach(ViewColumnFilterInfo info in filter) {
					GridColumn col = columns == null ? info.Column : columns.Find(info.Column);
					if(col != null && col.View != null) {
						col = col.View.GetParentColumnFieldNameSortGroup(col);
						Add(col, info.Filter.Clone() as ColumnFilterInfo);
					}
				}
			}
			catch {
				if(clone != null) {
					Assign(clone, columns, false);
					throw;
				}
			}
			finally {
				EndUpdate();
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("ViewFilterItem")]
#endif
		public ViewColumnFilterInfo this[int index] { get { return List[index] as ViewColumnFilterInfo; } }
		public void Remove(ViewColumnFilterInfo info) { List.Remove(info); }
		protected override void OnInsertComplete(int position, object item) { 
			ViewColumnFilterInfo info = (ViewColumnFilterInfo)item;
			info.viewFilter = this;
			OnViewFilterChanged();
		}
		protected override void OnRemoveComplete(int position, object item) { 
			ViewColumnFilterInfo info = (ViewColumnFilterInfo)item;
			info.viewFilter = null;
			OnViewFilterChanged();
		}
		protected override void OnClear() {
			fNonColumnFilterCriteria = null;
			foreach(ViewColumnFilterInfo info in this) {
				info.viewFilter = null;
			}
			InnerList.Clear();
			OnViewFilterChanged();
		}
		protected internal void OnFilterChanged(ViewColumnFilterInfo info) {
			if(info.Filter.Equals(ColumnFilterInfo.Empty) || info.Filter.Type == ColumnFilterType.None) {
				Remove(info);
			} else {
				OnViewFilterChanged();
			}
		}
		protected virtual void OnViewFilterChanged() {
			if(this.lockUpdate != 0) return;
			if(changed != null) changed(this, EventArgs.Empty);
		}
		public ViewFilter(ColumnView view, CriteriaOperator filter) {
			foreach(KeyValuePair<OperandProperty, CriteriaOperator> affinity
				in CriteriaColumnAffinityResolver.SplitByColumns(filter)) {
				string propertyName = affinity.Key.PropertyName;
				GridColumn gc = !string.IsNullOrEmpty(propertyName) ? view.Columns.ColumnByFilterField(propertyName) : null;
				if(gc == null) {
					this.fNonColumnFilterCriteria = GroupOperator.And(this.fNonColumnFilterCriteria, affinity.Value);
				} else {
					ColumnFilterInfo fi = new ColumnFilterInfo(affinity.Value);
					fi.SetFilterKind(ColumnFilterKind.Predefined);
					Add(gc, fi);
				}
			}
		}
		internal bool IsLegacyFilterDisplayTextMode() {
			foreach(ViewColumnFilterInfo vcfi in this) {
				if(vcfi.Filter.DisplayText != null && vcfi.Filter.DisplayText.Length > 0)
					return true;
			}
			return false;
		}
	}
	[Serializable]
	public class MRUViewFilterCollection : ViewFilterCollection {
		protected override bool CanAdd(ViewFilter filter) {
			return base.CanAdd(filter) && Find(filter) == -1;
		}
	}
	[Serializable, ListBindable(false)]
	public class ViewFilterCollection : CollectionBase, IEnumerable<ViewFilter> {
		public ViewFilter this[int index] { get { return List[index] as ViewFilter; } }
		public int Add(ViewFilter filter) { 
			if(!CanAdd(filter)) return -1;
			return List.Add(filter); 
		}
		public void Insert(int index, ViewFilter filter) { 
			if(!CanAdd(filter)) return;
			List.Insert(index, filter); 
		}
		protected virtual bool CanAdd(ViewFilter filter) {
			return filter != null && !filter.IsEmpty;
		}
		internal bool CanShowMRU(ViewFilter activeFilter) {
			if(Count == 0) return false;
			if(Count > 1) return true;
			return !Equals(this[0].Criteria, activeFilter.Criteria);
		}
		internal void InsertMRU(ViewFilter filter, int maxCount) {
			if(filter == null || filter.IsEmpty)
				return;
			int index = Find(filter);
			if(index != -1) RemoveAt(index);
			Insert(0, filter.Clone() as ViewFilter);
			CheckCount(maxCount);
		}
		internal int Find(ViewFilter filter) {
			CriteriaOperator filterExpression = filter.Criteria;
			for(int n = 0; n < Count; n++) {
				if(Equals(this[n].Criteria, filterExpression))
					return n;
			}
			return -1;
		}
		public void Remove(ViewFilter filter) { List.Remove(filter); }
		protected internal void Remove(GridColumn column) {
			for(int n = Count - 1; n >= 0; n --) {
				ViewFilter filter = this[n];
				filter.Remove(column);
				if(filter.IsEmpty)
					RemoveAt(n);
			}
		}
		protected internal void CheckCount(int maxCount) {
			maxCount = Math.Max(0, maxCount);
			while(Count > maxCount) RemoveAt(Count - 1);
		}
		protected internal void OnDeserialize(GridColumnCollection columns) {
			for(int n = Count - 1; n >= 0; n--) {
				ViewFilter filter = this[n];
				filter.OnDeserialize(columns);
				if(filter.IsEmpty)
					RemoveAt(n);
			}
		}
		protected internal void Assign(ViewFilterCollection filters) {
			Clear();
			for(int n = 0; n < filters.Count; n ++) {
				Add(filters[n].Clone() as ViewFilter);
			}
		}
		IEnumerator<ViewFilter> IEnumerable<ViewFilter>.GetEnumerator() {
			foreach(ViewFilter viewFilter in InnerList)
				yield return viewFilter;
		}
	}
}
