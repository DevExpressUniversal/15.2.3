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

using DevExpress.Data.PivotGrid;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxPivotGrid.Data {
	public class PivotGridPostBackActionBase {
		ASPxPivotGrid pivotGrid;
		string[] values;
		PostActionParametersBase parameters;
		public PivotGridPostBackActionBase(ASPxPivotGrid pivotGrid, string arguments) {
			this.pivotGrid = pivotGrid;
			this.values = arguments.Split(new char[] { '|' });
			this.parameters = CreateParameters(this.values);
		}
		public PivotGridPostBackActionBase(ASPxPivotGrid pivotGrid, PostActionParametersBase parameters) {
			this.pivotGrid = pivotGrid;
			this.parameters = parameters;
		}		
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		[Obsolete()]
		protected string[] Values { get { return values; } }
		protected PostActionParametersBase Parameters { get { return parameters; } }
		protected virtual PostActionParametersBase CreateParameters(string[] values) {
			return new PostActionParametersBase();
		}
		public virtual bool RequireDataUpdate { get { return true; } }
		public virtual void ApplyBefore() { }
		public virtual void ApplyAfter() { }
		public virtual void RaiseEventHandlers() { }
	}
	public class PostActionParametersBase { }
	public class PivotGridPostbackExpandedItemAction : PivotGridPostBackActionBase {
		public PivotGridPostbackExpandedItemAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) {
		}
		public PivotGridPostbackExpandedItemAction(ASPxPivotGrid pivotGrid, ExpandedItemActionParameters parameters)
			: base(pivotGrid, parameters) {
		}
		protected new ExpandedItemActionParameters Parameters {
			get { return (ExpandedItemActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new ExpandedItemActionParameters(Data, values);
		}
		public override void ApplyAfter() {
			if(Parameters.ExpandItem)
				ExpandItem();
			if(Parameters.ExpandField)
				ExpandField();
		}
		protected virtual void ExpandField() {
			Data.ChangeFieldExpanded(Parameters.Field, Parameters.Expand.Value);
		}
		protected virtual void ExpandItem() {
			if(Parameters.ItemValues != null) 
				Data.ChangeExpanded(Parameters.ValueItem, true);
		}
	}
	public class ExpandedItemActionParameters : PostActionParametersBase {
		int uniqueIndex;
		bool isColumn, expandItem, expandField;
		bool? expand;
		PivotGridWebData data;
		public ExpandedItemActionParameters(PivotGridWebData data, string[] values) {
			this.data = data;
			if(!int.TryParse(values[1], out uniqueIndex))
				uniqueIndex = -1;
			this.isColumn = values[0] == CallbackCommands.ExpandColumnChanged;
			this.expandItem = values.Length == 2;
			this.expandField = values.Length == 3;
			if(this.expandField)
				this.expand = values.Length == 3 && values[2] == CallbackCommands.ExpandFieldChanged;
		}
		public ExpandedItemActionParameters(PivotGridWebData data, int uniqueIndex, bool isColumn, 
				bool expandItem, bool expandField, bool? expand) {
			this.data = data;
			this.uniqueIndex = uniqueIndex;
			this.isColumn = isColumn;
			this.expandItem = expandItem;
			this.expandField = expandField;
			if(this.expandItem && this.expandField)
				throw new ArgumentException("Invalid parameters combination");
			this.expand = this.expandField ? expand : null;
		}
		public PivotGridWebData Data { get { return data; } }
		public int UniqueIndex { get { return uniqueIndex; } }
		public bool IsColumn { get { return isColumn; } }
		public bool ExpandItem { get { return expandItem; } }
		public bool ExpandField { get { return expandField; } }
		public bool? Expand { get { return expand; } }
		public object[] ItemValues {
			get { return Data.VisualItems.GetItemValues(IsColumn, UniqueIndex); }
		}
		public PivotGridFieldBase Field {
			get { return Data.GetField(Data.VisualItems.GetItemField(IsColumn, UniqueIndex)); }
		}
		public PivotFieldValueItem ValueItem {
			get { return Data.VisualItems.GetUnpagedItem(IsColumn, UniqueIndex); }
		}
	}
	public class PivotGridPostBackFieldListDeferAction : PivotGridPostBackActionBase {
		public PivotGridPostBackFieldListDeferAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		string[] values;
		protected override PostActionParametersBase CreateParameters(string[] values) {
			if(values.Length != 6)
				throw new ArgumentException();
			this.values = values;
			return base.CreateParameters(values);
		}
		void SetFieldListFields() {
			if(!string.IsNullOrEmpty(values[5])) {
				string[] fields = values[5].Split(',');
				for(int i = 0; i < fields.Length; i++)
					Data.FieldListFields.HideField(Data.GetFieldItem(ScriptHelper.GetFieldByHeaderID(fields[i])));
			}
			for(int i = 1; i < 5; i++) {
				if(string.IsNullOrEmpty(values[i]))
					continue;
				string[] fields = values[i].Split(',');
				for(int j = 0; j < fields.Length; j++) {
					PivotArea newArea = (PivotArea)(i - 1);
					int areaFieldsCount = Data.FieldListFields[newArea].Count;
					PivotGridField field = ScriptHelper.GetFieldByHeaderID(fields[j]);
					if(Data.FieldListFields[newArea].Contains(Data.GetFieldItem(field)))
						areaFieldsCount--;
					if(Data.OnFieldAreaChanging(field, newArea, Math.Min(areaFieldsCount, j)))
						Data.FieldListFields.MoveField(Data.GetFieldItem(field), newArea, Math.Min(areaFieldsCount, j));
				}
			}
		}
		public override void ApplyBefore() {
			SetFieldListFields();
			Data.FieldListFields.SetFieldsToData();
			base.ApplyBefore();
		}
	}
	public class PivotGridFieldsPostbackActionBase : PivotGridPostBackActionBase {
		public PivotGridFieldsPostbackActionBase(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		public PivotGridFieldsPostbackActionBase(ASPxPivotGrid pivotGrid, FieldsActionParameters parameters)
			: base(pivotGrid, parameters) { }
		protected PivotGridFieldCollection Fields { get { return Data.Fields; } }
		protected new FieldsActionParameters Parameters {
			get { return (FieldsActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new FieldsActionParameters(PivotGrid, values);
		}		
	}
	public class FieldsActionParameters : PostActionParametersBase {
		PivotGridField dragField;
		public FieldsActionParameters(ASPxPivotGrid pivotGrid, string[] values) {
			this.dragField = values.Length >= 2 ? pivotGrid.ScriptHelper.GetFieldByHeaderID(values[1]) : null;
		}
		public FieldsActionParameters(PivotGridField dragField) {
			this.dragField = dragField;
		}
		public PivotGridField DragField { get { return dragField; } }
	}
	public class PivotGridPostBackChangeGroupExpandedAction : PivotGridPostBackActionBase {
		public PivotGridPostBackChangeGroupExpandedAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		protected new ChangeGroupExpandedActionParameters Parameters {
			get { return (ChangeGroupExpandedActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new ChangeGroupExpandedActionParameters(values);
		}
		public override void ApplyBefore() {
			PivotGridField field = Data.Fields[Parameters.FieldIndex];
			field.ExpandedInFieldsGroup = !field.ExpandedInFieldsGroup;
		}
	}
	public class ChangeGroupExpandedActionParameters : PostActionParametersBase {
		int fieldIndex;
		public ChangeGroupExpandedActionParameters(string[] values){
			this.fieldIndex = int.Parse(values[1]);
		}
		public int FieldIndex { get { return fieldIndex; } }
	}
	public class PivotGridPostBackDragFieldAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackDragFieldAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		public PivotGridPostBackDragFieldAction(ASPxPivotGrid pivotGrid, DragFieldActionParameters parameters)
			: base(pivotGrid, parameters) { }
		protected new DragFieldActionParameters Parameters {
			get { return (DragFieldActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new DragFieldActionParameters(PivotGrid, values);
		}
		bool changed;
		public override void ApplyBefore() {
			base.ApplyBefore();
			changed = false;
			PivotGridField dragField = Parameters.DragField;
			bool isVisible = Parameters.IsVisible;
			if(!isVisible) {
				dragField.Visible = isVisible;
				return;
			}
			if(Data.OnFieldAreaChanging(dragField, Parameters.NewArea, Parameters.NewAreaIndex))
				changed = true;
		}
		public override void RaiseEventHandlers() {
			base.RaiseEventHandlers();
			if(changed)
				Data.SetFieldAreaPosition(Parameters.DragField, Parameters.NewArea, Parameters.NewAreaIndex);
		}
	}
	public class DragFieldActionParameters : FieldsActionParameters {
		bool isVisible;
		PivotArea newArea;
		int newAreaIndex;
		public DragFieldActionParameters(ASPxPivotGrid pivotGrid, string[] values)
			: base(pivotGrid, values) {
			this.isVisible = values.Length >= 3 ? !pivotGrid.Data.IsCustomizationFields(values[2]) : true;
			PivotGridField targetField = values.Length >= 3 ? pivotGrid.ScriptHelper.GetFieldByHeaderID(values[2]) : null;
			this.newArea = GetArea(pivotGrid, values, DragField, targetField);
			bool isLeft = values.Length >= 4 ? values[3].ToUpper() == bool.TrueString.ToUpper() : false;
			this.newAreaIndex = GetAreaIndex(pivotGrid.Data, DragField, targetField, isLeft, newArea);
		}
		public DragFieldActionParameters(PivotGridField dragField, bool isVisible, PivotArea newArea, int newAreaIndex) 
			: base(dragField) {
			this.isVisible = isVisible;
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
		}
		public bool IsVisible { get { return isVisible; } }
		public PivotArea NewArea { get { return newArea; } }
		public int NewAreaIndex { get { return newAreaIndex; } }
		protected PivotArea GetArea(ASPxPivotGrid pivotGrid, string[] values, PivotGridField dragField, PivotGridField targetField) {
			if(dragField == null) return PivotArea.FilterArea;
			if(values.Length < 3) return dragField.Area;
			if(targetField != null) return targetField.Area;
			PivotArea area;
			if(pivotGrid.ScriptHelper.GetAreaByID(values[2], out area))
				return area;
			return dragField.Area;
		}
		protected int GetAreaIndex(PivotGridWebData data, PivotGridField dragField, PivotGridField targetField, bool isLeft, PivotArea newArea) {
			if(dragField == null) return 0;
			if(targetField == null) {
				if(isLeft)
					return 0;
				else
					return data.GetFieldsByArea(newArea, true).Count;
			}
			List<PivotGridFieldBase> fields = data.GetFieldsByArea(targetField.Area, true);
			if(targetField == dragField)
				return fields.IndexOf(targetField);
			fields.Remove(dragField);
			fields.Insert(fields.IndexOf(targetField) + (isLeft ? 0 : 1), dragField);
			return fields.IndexOf(dragField);
		}
	}
	public class PivotGridPostBackFieldSortAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackFieldSortAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) {
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			PivotGridField field = Parameters.DragField;
			if(field != null) 
				field.ChangeSortOrder();
		}
	}
	public class PivotGridPostBackSortByColumnAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackSortByColumnAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) {
		}
		public new SortByColumnActionParameters Parameters {
			get { return (SortByColumnActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new SortByColumnActionParameters(PivotGrid, values);
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			if(Parameters.IsRemoveAll)
				ApplyRemoveAll();
			else
				ApplySorting();
		}
		void ApplySorting() {
			PivotGridField field = Parameters.Field;
			field.SetSortBySummary(Parameters.DataField, Parameters.FieldSortConditions, Parameters.CustomTotalSummaryType, !IsSortedByThisSummary(field));
		}
		bool IsSortedByThisSummary(PivotGridFieldBase field) {
			if(Parameters.Index >= 0)
				return Data.VisualItems.IsFieldSortedBySummary(Parameters.IsColumn, Data.GetFieldItem(field), Data.GetFieldItem(Parameters.DataField), Parameters.Index);
			else
				return Data.IsFieldSortedBySummary(field, Parameters.DataField, Parameters.FieldSortConditions);
		}
		void ApplyRemoveAll() {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(Parameters.FieldArea, false);
			for(int i = 0; i < fields.Count; i++) {
				if(IsSortedByThisSummary(fields[i]))
					fields[i].SortBySummaryInfo.Reset();
			}
		}
	}
	public class SortByColumnActionParameters : FieldsActionParameters {
		int visibleIndex;
		int itemIndex;
		bool isColumn;
		PivotGridField field, dataField;
		bool isRemoveAll;
		List<PivotGridFieldSortCondition> fieldSortConditions;
		PivotSummaryType? customTotalSummaryType;
		public SortByColumnActionParameters(ASPxPivotGrid pivotGrid, string[] values)
			: base(pivotGrid, values) {
			PivotGridWebData data = pivotGrid.Data;
			this.isRemoveAll = values[3] == "RemoveAll";
			this.visibleIndex = Int32.Parse(values[2]);
			PivotArea itemArea = data.DataField.Area;
			if(VisibleIndex >= 0) {
				PivotGridField itemField = (PivotGridField)data.Fields.GetFieldByClientID(values[1]);
				itemArea = itemField.Area != PivotArea.DataArea ? itemField.Area : data.DataField.Area;
			}
			if(IsRemoveAll)
				itemArea = (PivotArea)Enum.Parse(typeof(PivotArea), values[5]);
			if(itemArea != PivotArea.ColumnArea && itemArea != PivotArea.RowArea)
				throw new ArgumentException("itemArea");
			if(!IsRemoveAll) {
				this.field = data.Fields[Int32.Parse(values[3])];
				if(VisibleIndex < 0)
					itemArea = (Field.Area == PivotArea.ColumnArea) ? PivotArea.RowArea : PivotArea.ColumnArea;
			}
			this.isColumn = itemArea == PivotArea.ColumnArea;
			int dataIndex = Int32.Parse(values[4]);
			this.dataField = dataIndex >= 0 ? data.Fields[dataIndex] : null;
			this.fieldSortConditions = data.GetFieldSortConditions(IsColumn, VisibleIndex);
			int indexPos = IsRemoveAll ? 6 : 5;
			if(indexPos < values.Length) {
				this.itemIndex = Int32.Parse(values[indexPos]);
				PivotFieldValueItem item = data.VisualItems.GetItem(IsColumn, this.itemIndex);
				if(item.CustomTotal != null)
					this.customTotalSummaryType = item.CustomTotal.SummaryType;
			} else {
				this.itemIndex = -1;
			}
		}
		public bool IsColumn { get { return isColumn; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public int Index { get { return itemIndex; } }
		public List<PivotGridFieldSortCondition> FieldSortConditions { get { return fieldSortConditions; } }
		public bool IsRemoveAll { get { return isRemoveAll; } }
		public PivotArea FieldArea { get { return IsColumn ? PivotArea.RowArea : PivotArea.ColumnArea; } }
		public PivotGridField Field { get { return field; } }
		public PivotGridField DataField { get { return dataField; } }
		public PivotSummaryType? CustomTotalSummaryType { get { return customTotalSummaryType; } }
	}
	public class PivotGridPostBackHideFieldAction : PivotGridFieldsPostbackActionBase {
		public PivotGridPostBackHideFieldAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		public override void ApplyBefore() {
			base.ApplyBefore();
			PivotGridField field = Parameters.DragField;
			if(field != null) 
				field.Visible = false;
		}
	}
	public class PivotGridPostBackFilterFieldAction : PivotGridPostBackActionBase {
		bool changed;
		public PivotGridPostBackFilterFieldAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		protected new FilterFieldActionParameters Parameters {
			get { return (FilterFieldActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new FilterFieldActionParameters(Data, values, false);
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			if(Parameters.FilterItems != null)
				changed = Parameters.ApplyFilter(false);
		}
		public override void RaiseEventHandlers() {
			base.RaiseEventHandlers();
			if(changed)
				if(Data.GetIsGroupFilter(Parameters.Field))
					Data.OnGroupFilteringChanged(Parameters.Field.Group);
				else
				Data.OnFieldFilteringChanged(Parameters.Field);
		}
	}
	public class PivotGridDefereFieldFilterAction : PivotGridPostBackActionBase {
		public PivotGridDefereFieldFilterAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		protected new FilterFieldActionParameters Parameters {
			get { return (FilterFieldActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new FilterFieldActionParameters(Data, values, true);
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			bool defere = Data.GetCustomizationFormFields().DeferUpdates;
			Data.GetCustomizationFormFields().DeferUpdates = true;
			Parameters.ApplyFilter(true);
			Data.GetCustomizationFormFields().DeferUpdates = defere;
		}
		public override void RaiseEventHandlers() {
			base.RaiseEventHandlers();
		}
	}
	public class FilterFieldActionParameters : PostActionParametersBase {
		readonly bool forceApply;
		public FilterFieldActionParameters(PivotGridWebData data, string[] values, bool defere) {
			int fieldIndex;
			if(!int.TryParse(values[3], out fieldIndex))
				return;
			PivotGridField field = data.GetFieldByIndex(fieldIndex);
			if(field != null) {
				FilterItems = data.CreatePivotGridFilterItems(field, defere);
				FilterItems.InitializeVisible(values[2], values[1]); 
				forceApply = (values.Length > 4) ? (values[4] == "T") : false;
			}
		}
		public PivotFilterItemsBase FilterItems { get; private set; }
		public PivotGridField Field { get { return (PivotGridField)FilterItems.Field; } }
		internal bool ApplyFilter(bool deferUpdates) {
			if(FilterItems == null) return false;
			return FilterItems.ApplyFilter(false, deferUpdates, forceApply);
		}
	}
	public class PivotGridCustomPostbackAction : PivotGridPostBackActionBase {
		bool isRaising;
		public PivotGridCustomPostbackAction(ASPxPivotGrid pivot, string arguments)
			: base(pivot, arguments) {
			this.isRaising = false;
		}
		protected bool IsRaising { 
			get { return isRaising; }
		}
		void SetIsRaising() {
			isRaising = true;
		}
		protected new CustomPostbackActionParameters Parameters {
			get { return (CustomPostbackActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new CustomPostbackActionParameters(values);
		}
		public override void ApplyBefore() {
			if(!Data.LockDataRefreshOnCustomCallback)
				return;
			base.ApplyBefore();
			PivotGrid.RaiseCustomCallback(Parameters.Parameter);
		}
		public override void RaiseEventHandlers() {
			if(Data.LockDataRefreshOnCustomCallback || IsRaising)
				return;
			SetIsRaising();
			base.RaiseEventHandlers();
			PivotGrid.RaiseCustomCallback(Parameters.Parameter);
		}
	}
	public class CustomPostbackActionParameters : PostActionParametersBase {
		string parameter;
		public CustomPostbackActionParameters(string[] values) {
			this.parameter = string.Join("|", values);
		}
		public string Parameter { get { return parameter; } }
	}
	public class PivotGridPostBackPagerAction : PivotGridPostBackActionBase {
		public PivotGridPostBackPagerAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		protected new PagerActionParameters Parameters {
			get { return (PagerActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new PagerActionParameters(values);
		}
		public override void ApplyBefore() {
			int oldIndex = Data.OptionsPager.PageIndex;
			int oldSize = Data.OptionsPager.RowsPerPage;
			int newIndex = oldIndex;
			int newSize = oldSize;
			PivotWebVisualItems items = Data.VisualItems;
			string command = Parameters.Argument;
			if(ASPxPagerBase.IsChangePageSizeCommand(command))
				newSize = ASPxPagerBase.GetNewPageSize(command, Data.OptionsPager.RowsPerPage);
			else
				newIndex = ASPxPagerBase.GetNewPageIndex(command, items.GetPageIndex(false), items.GetPageCount(false));
			if(newSize <= 0) {
				newIndex = -1;
				newSize = oldSize;
			}
			else {
				if(oldIndex == -1)
					newIndex = 0;
			}
			if(newIndex != oldIndex && PagerIsValidPageIndex(newIndex))
				Data.OptionsPager.PageIndex = newIndex;
			if(newSize != oldSize && PagerIsValidPageSize(newSize))
				Data.OptionsPager.RowsPerPage = newSize;
		}
		protected bool PagerIsValidPageIndex(int pageIndex) {
			if(pageIndex == -1) {
				if(Data.OptionsPager.Visible)
					return Data.OptionsPager.AllButton.Visible || IsPageSizeAllItemVisible();
				return false;
			}
			return true;
		}
		protected bool PagerIsValidPageSize(int pageSize) {
			if(pageSize == -1)
				return PagerIsValidPageIndex(pageSize);
			return pageSize == PivotGrid.InitialPageSize ||
				   Array.Exists<string>(Data.OptionsPager.PageSizeItemSettings.Items, delegate(string item) { 
					   return item == pageSize.ToString(); 
				   });
		}
		protected internal bool IsPageSizeAllItemVisible() {
			return Data.OptionsPager.PageSizeItemSettings.Visible && Data.OptionsPager.PageSizeItemSettings.ShowAllItem;
		}
	}
	public class PagerActionParameters : PostActionParametersBase {
		string argument;
		public PagerActionParameters(string[] values) {
			this.argument = values[1];
		}
		public string Argument { get { return argument; } }
	}
	public class VirtualScrollingActionParameters : PostActionParametersBase {
		readonly int vertPageIndex;
		readonly int vertPageSize;
		readonly int horzPageIndex;
		readonly int horzPageSize;
		public VirtualScrollingActionParameters(string[] values) {
			this.vertPageIndex = Int32.Parse(values[1]);
			this.vertPageSize = Int32.Parse(values[2]);
			this.horzPageIndex = Int32.Parse(values[3]);
			this.horzPageSize = Int32.Parse(values[4]);
		}
		public int VertPageIndex { get { return vertPageIndex; } }
		public int VertPageSize { get { return vertPageSize; } }
		public int HorzPageIndex { get { return horzPageIndex; } }
		public int HorzPageSize { get { return horzPageSize; } }
	}
	public class PivotGridPostBackVirtualScrollingAction : PivotGridPostBackActionBase {
		public PivotGridPostBackVirtualScrollingAction(ASPxPivotGrid pivotGrid, string arguments)
			: base(pivotGrid, arguments) { }
		protected new VirtualScrollingActionParameters Parameters {
			get { return (VirtualScrollingActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new VirtualScrollingActionParameters(values);
		}
		public override void ApplyBefore() {
			if(PivotGrid.OptionsView.VerticalScrollingMode == PivotScrollingMode.Virtual) {
				PivotGrid.OptionsPager.PageIndex = Parameters.VertPageIndex;
				PivotGrid.OptionsPager.RowsPerPage = Parameters.VertPageSize;
			}
			if(PivotGrid.OptionsView.HorizontalScrollingMode == PivotScrollingMode.Virtual) {
				PivotGrid.OptionsPager.ColumnPageIndex = Parameters.HorzPageIndex;
				PivotGrid.OptionsPager.ColumnsPerPage = Parameters.HorzPageSize;
			}
		}
	}
	public enum PivotGridPrefilterCommand { Show, Hide, Reset, ChangeEnabled, Set };
	public class PivotGridPostBackPrefilterAction : PivotGridPostBackActionBase {
		bool requireDataUpdate;
		public PivotGridPostBackPrefilterAction(ASPxPivotGrid pivot, string arguments)
			: base(pivot, arguments) {
			this.requireDataUpdate = false;
			Process();
		}
		public PivotGridPostBackPrefilterAction(ASPxPivotGrid pivot, PivotGridPrefilterCommand command, string argument)
			: base(pivot, "|" + command.ToString() + "|" + argument) {
			this.requireDataUpdate = false;
			Process();
		}
		public override bool RequireDataUpdate { get { return requireDataUpdate; } }
		protected new PrefilterActionParameters Parameters {
			get { return (PrefilterActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new PrefilterActionParameters(values);
		}
		protected void Process() {
			switch(Parameters.Command) {
				case PivotGridPrefilterCommand.Show:
					PivotGrid.IsPrefilterPopupVisible = true;
					break;
				case PivotGridPrefilterCommand.Hide:
					PivotGrid.IsPrefilterPopupVisible = false;
					break;
				case PivotGridPrefilterCommand.Reset:
				case PivotGridPrefilterCommand.ChangeEnabled:
				case PivotGridPrefilterCommand.Set:
					this.requireDataUpdate = true;
					break;
			}
		}
		public override void ApplyBefore() {
			switch(Parameters.Command) {
				case PivotGridPrefilterCommand.Reset:
					PivotGrid.Prefilter.Clear();
					break;
				case PivotGridPrefilterCommand.ChangeEnabled:
					PivotGrid.Prefilter.Enabled = !PivotGrid.Prefilter.Enabled;
					break;
				case PivotGridPrefilterCommand.Set:
					PivotGrid.Prefilter.SetUserCriteriaString(Parameters.Argument);
					break;
			}
		}
	}
	public class PrefilterActionParameters : PostActionParametersBase {
		PivotGridPrefilterCommand command;
		string argument;
		public PrefilterActionParameters(string[] values) {
			this.command = (PivotGridPrefilterCommand)Enum.Parse(typeof(PivotGridPrefilterCommand), values[1]);
			this.argument = values.Length >= 3 ? values[2] : null;
		}
		public PrefilterActionParameters(PivotGridPrefilterCommand command, string argument) {
			this.command = command;
			this.argument = argument;
		}
		public PivotGridPrefilterCommand Command { get { return command; } }
		public string Argument { get { return argument; } }
	}
	public enum PivotGridSortModeNoneCommand { SortAZ, SortZA, ClearSort };
	public class PivotGridPostBackSortModeNoneAction : PivotGridPostBackActionBase {
		public PivotGridPostBackSortModeNoneAction(ASPxPivotGrid pivot, string arguments)
			: base(pivot, arguments) {
		}
		protected new SortModeNoneActionParameters Parameters {
			get { return (SortModeNoneActionParameters)base.Parameters; }
		}
		protected override PostActionParametersBase CreateParameters(string[] values) {
			return new SortModeNoneActionParameters(PivotGrid, values);
		}
		public override void ApplyBefore() {
			base.ApplyBefore();
			PivotGridField field = Parameters.Field;
			switch(Parameters.Command) {
				case PivotGridSortModeNoneCommand.SortAZ:
					Data.SetFieldSorting(field, PivotSortOrder.Ascending, PivotSortMode.DisplayText, null, true);
					break;
				case PivotGridSortModeNoneCommand.SortZA:
					Data.SetFieldSorting(field, PivotSortOrder.Descending, PivotSortMode.DisplayText, null, true);
					break;
				case PivotGridSortModeNoneCommand.ClearSort:
					Data.SetFieldSorting(field, PivotSortOrder.Ascending, null, PivotSortMode.None, false);
					break;
			}
		}
	}
	public class SortModeNoneActionParameters : PostActionParametersBase {
		PivotGridSortModeNoneCommand command;
		PivotGridField field;
		public SortModeNoneActionParameters(ASPxPivotGrid pivotGrid, string[] values) {
			this.command = ParseCommand(values[0]);
			this.field = pivotGrid.ScriptHelper.GetFieldByHeaderID(values[1]);
		}
		public PivotGridSortModeNoneCommand Command { get { return command; } }
		public PivotGridField Field { get { return field; } }
		public static PivotGridSortModeNoneCommand ParseCommand(string command) {
			switch(command) {
				case CallbackCommands.SortAZ:
					return PivotGridSortModeNoneCommand.SortAZ;
				case CallbackCommands.SortZA:
					return PivotGridSortModeNoneCommand.SortZA;
				case CallbackCommands.ClearSort:
					return PivotGridSortModeNoneCommand.ClearSort;
			}
			throw new ArgumentException("Unknown command(" + command + ")");
		}
	}
}
