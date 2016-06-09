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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridHeaderTemplateItem {
		PivotGridWebData data;
		PivotFieldItemBase field;
		DefaultBoolean canDragInGroup;
		ISupportsFieldsCustomization control;
		public PivotGridHeaderTemplateItem(PivotGridWebData data, PivotFieldItemBase field, DefaultBoolean canDrag, ISupportsFieldsCustomization control) {
			this.data = data;
			this.field = field;
			this.canDragInGroup = canDrag;
			this.control = control;
		}
		internal ISupportsFieldsCustomization FieldList { get { return control; } }
		protected internal PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		public PivotGridField Field { get { return Data.GetField(FieldItem); } }
		protected internal PivotFieldItem FieldItem { get { return (PivotFieldItem)field; } }
		public string ID { get { return ScriptHelper.GetHeaderID(FieldItem); } }
		public bool IsGroupButtonVisible { 
			get { return Field.Group != null && Field.Group.CanExpandField(Field); } 
		}
		public bool IsFilterButtonVisible { 
			get { return Field.ShowFilterButton && (HasFilter || AllowFilter); } 
		}
		public bool IsSortButtonVisible {
			get { return Field.Visible && Field.ShowSortButton && AllowSort; } 
		}
		public bool CanDrag {
			get {
				bool canDrag = IsFieldListItem ? Field.CanDragInCustomizationForm : Field.CanDrag;
				return canDrag && (Field.Group == null || CanDragInGroup == DefaultBoolean.True) &&
					Data.PivotGrid.IsEnabled();
			}
		}
		public bool CanSort {
			get {
				return PivotGrid.IsEnabled() && Field.CanSort && Field.Visible && AllowSort;
			}
		}
		public string HeaderMouseDownScript {
			get {
				return ScriptHelper.GetHeaderMouseDown();
			}
		}
		public string HeaderClickScript {
			get {
				return ScriptHelper.GetHeaderClick(IsFieldListItem);
			}
		}
		public bool IsAccessibilityCompliant {
			get {
				return PivotGrid.IsAccessibilityCompliantRender();
			}
		}
		public AppearanceStyle HeaderTableStyle {
			get {
				return Data.GetHeaderTableStyle(FieldItem);
			}
		}
		bool HasFilter {
			get { return Field.ShowActiveFilterButton || Field.Visible && control == null; }
		}
		protected internal DefaultBoolean CanDragInGroup {
			get { return canDragInGroup; }
		}
		protected bool AllowSort {
			get { return control != null ? control.AllowSortInForm : true; }
		}
		protected internal bool AllowFilter {
			get { return control != null ? control.AllowFilterInForm : true; }
		}
		protected internal bool IsFieldListItem {
			get { return control != null; }
		}
		public void AddContextMenu() {
			PivotFieldItem fieldItem = FieldItem;
			string fullID = ID;
			if(fieldItem.Area == PivotArea.DataArea && Data.IsDataAreaCollapsed) {
				fullID = ElementNames.DataHeadersPopup + "_" + ID;
			}
			Data.PivotGrid.RenderHelper.AddHeaderContextMenu(fullID, fieldItem);
		}
	}
	public class PivotGridHeaderTemplateContainer : TemplateContainerBase {		
		public PivotGridHeaderTemplateContainer(PivotGridHeaderTemplateItem item)
			: base(item.Field.Index, item.Field) {
			this.item = item;
			this.ID = GetID(item);
		}
		PivotGridHeaderTemplateItem item;
		protected PivotGridHeaderTemplateItem Item { get { return item; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridHeaderTemplateContainerField")]
#endif
		public PivotGridField Field { get { return Item.Field; } }
		public PivotGridHeaderHtmlTable CreateHeader() {
			return new PivotGridHeaderHtmlTable(Item);
		}
		protected string GetID(PivotGridHeaderTemplateItem item) {
			return "ht" + item.ID;
		}
	}
	public class PivotGridHeaderHtmlTable : PivotGridHeaderHtmlTableBase {
		public PivotGridHeaderHtmlTable(PivotGridHeaderTemplateItem templateItem)
			: base(templateItem) {
			CreateHierarchyCore();
			PrepareHierarchyCore(true);
		}
		public new PivotGridField Field { get { return Item.Field; } }
		public bool IsGroupButtonVisible {
			get { return Item.IsGroupButtonVisible; }
		}
		public bool IsSortButtonVisible {
			get { return Item.IsSortButtonVisible; }
		}
		public bool IsFilterButtonVisible {
			get { return Item.IsFilterButtonVisible; }
		}
		public bool IsDragAllowed {
			get { return Item.CanDrag; }
		}
		public object Content {
			get { return HeaderText.Content; }
			set { HeaderText.Content = value; }
		}
		protected override void SetID() {
			ID = Item.ID + "HT";
		}
	}
	public class PivotGridEmptyAreaTemplateContainer : TemplateContainerBase {
		PivotArea area;
		public PivotGridEmptyAreaTemplateContainer(PivotArea area)
			: base((int)area, null) {
			this.area = area;
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridEmptyAreaTemplateContainerArea")]
#endif
		public PivotArea Area { get { return area; } }
	}
	public class PivotGridFieldValueTemplateItem {
		string id;
		PivotGridField field;
		PivotFieldValueItem item;
		List<PivotGridFieldPair> sortedFields;
		public PivotGridFieldValueTemplateItem(string id, PivotGridField field, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields) {
			this.id = id;
			this.field = field;
			this.item = item;
			this.sortedFields = sortedFields;
		}
		protected internal string ID { get { return id; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateItemField")]
#endif
		public PivotGridField Field { get { return field; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateItemValueItem")]
#endif
		public PivotFieldValueItem ValueItem { get { return item; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the CollapsedImage property instead")]
		public ImageProperties CollaspedImage { get { return CollapsedImage; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateItemCollapsedImage")]
#endif
		public ImageProperties CollapsedImage { get { return PivotGrid.RenderHelper.GetFieldValueCollapsedImage(ValueItem.IsCollapsed); } }
		protected internal ImageProperties SortedByImage { get { return PivotGrid.RenderHelper.GetSortByColumnImage(); } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateItemImageOnClick")]
#endif
		public string ImageOnClick { get { return ScriptHelper.GetCollapsedImageOnClick(ValueItem, false); } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateItemIsAnyFieldSortedByThisValue")]
#endif
		public bool IsAnyFieldSortedByThisValue { get { return SortedFields != null && SortedFields.Count > 0; } }		
		protected internal PivotGridWebData Data { get { return (PivotGridWebData)ValueItem.Data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		protected internal List<PivotGridFieldPair> SortedFields { get { return sortedFields; } }
	}
	public class PivotGridFieldValueTemplateContainer : TemplateContainerBase {
		public PivotGridFieldValueTemplateContainer(PivotGridFieldValueTemplateItem item)
			: base(item.ValueItem.Index, item) {
			this.ID = GetID(item);
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerItem")]
#endif
		public PivotGridFieldValueTemplateItem Item {
			get { return DataItem as PivotGridFieldValueTemplateItem; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerField")]
#endif
		public PivotGridField Field { get { return Item.Field; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerValueItem")]
#endif
		public PivotFieldValueItem ValueItem { get { return Item.ValueItem; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerValue")]
#endif
		public object Value { get { return ValueItem.Value; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerText")]
#endif
		public string Text { get { return ValueItem.Text; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerHtmlText")]
#endif
		public string HtmlText { get { return !string.IsNullOrEmpty(Text) ? Text : "&nbsp;"; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerMinIndex")]
#endif
		public int MinIndex { get { return ValueItem.MinLastLevelIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridFieldValueTemplateContainerMaxIndex")]
#endif
		public int MaxIndex { get { return ValueItem.MaxLastLevelIndex; } }
		public PivotGridFieldValueHtmlCell CreateFieldValue() {
			return new PivotGridFieldValueHtmlCell(Item);
		}
		protected string GetID(PivotGridFieldValueTemplateItem item) {
			return "fvt" + item.ID;
		}
	}
	public class PivotGridFieldValueHtmlCell : PivotGridHtmlFieldValueCellBase {
		public PivotGridFieldValueHtmlCell(PivotGridFieldValueTemplateItem item)
			: base(item.Data, item.ValueItem, item.SortedFields) {
			CreateHierarchyCore();
			PrepareHierarchyCore();
		}
		public bool IsCollapsedButtonVisible { get { return Item.ShowCollapsedButton; } }
		public override void RenderBeginTag(HtmlTextWriter writer) {
		}
		public override void RenderEndTag(HtmlTextWriter writer) {
		}
	}
	public class PivotGridCellTemplateItem {
		PivotCellBaseEventArgs args;
		string text;
		public PivotGridCellTemplateItem(PivotGridCellItem cellItem, string text) {
			this.args = new PivotCellBaseEventArgs(cellItem);
			this.text = text;
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemText")]
#endif
		public string Text { get { return text; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemDataField")]
#endif
		public PivotGridField DataField { get { return args.DataField; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemColumnIndex")]
#endif
		public int ColumnIndex { get { return args.ColumnIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemRowIndex")]
#endif
		public int RowIndex { get { return args.RowIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemColumnFieldIndex")]
#endif
		public int ColumnFieldIndex { get { return args.ColumnFieldIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemRowFieldIndex")]
#endif
		public int RowFieldIndex { get { return args.RowFieldIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemColumnField")]
#endif
		public PivotGridField ColumnField { get { return args.ColumnField; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemRowField")]
#endif
		public PivotGridField RowField { get { return args.RowField; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemValue")]
#endif
		public object Value { get { return args.Value; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemSummaryValue")]
#endif
		public PivotSummaryValue SummaryValue { get { return args.SummaryValue; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemColumnValueType")]
#endif
		public PivotGridValueType ColumnValueType { get { return args.ColumnValueType; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateItemRowValueType")]
#endif
		public PivotGridValueType RowValueType { get { return args.RowValueType; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() { return args.CreateDrillDownDataSource(); }		
		public object GetFieldValue(PivotGridField field) { return args.GetFieldValue(field); }
		public bool IsOthersFieldValue(PivotGridField field) { return args.IsOthersFieldValue(field); }
		public bool IsFieldValueExpanded(PivotGridField field) { return args.IsFieldValueExpanded(field); }
		public bool IsFieldValueRetrievable(PivotGridField field) { return args.IsFieldValueRetrievable(field); }
		public PivotGridField[] GetColumnFields() { return args.GetColumnFields(); }
		public PivotGridField[] GetRowFields() { return args.GetRowFields(); }
		public object GetCellValue(PivotGridField dataField) { return args.GetCellValue(dataField); }
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return args.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetColumnGrandTotal(PivotGridField dataField) { return args.GetColumnGrandTotal(dataField); }
		public object GetColumnGrandTotal(object[] rowValues, PivotGridField dataField) { return args.GetColumnGrandTotal(rowValues, dataField); }
		public object GetRowGrandTotal(PivotGridField dataField) { return args.GetRowGrandTotal(dataField); }
		public object GetRowGrandTotal(object[] columnValues, PivotGridField dataField) { return args.GetRowGrandTotal(columnValues, dataField); }
		public object GetGrandTotal(PivotGridField dataField) { return args.GetGrandTotal(dataField); }
	}
	public class PivotGridCellTemplateContainer : TemplateContainerBase {
		public PivotGridCellTemplateContainer(PivotGridCellTemplateItem item) : base(-1, item) { 
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerItem")]
#endif
		public PivotGridCellTemplateItem Item {
			get { return DataItem as PivotGridCellTemplateItem; }
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerDataField")]
#endif
		public PivotGridField DataField { get { return Item.DataField; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerValue")]
#endif
		public object Value { get { return Item.Value; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerText")]
#endif
		public string Text { get { return Item.Text; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerColumnField")]
#endif
		public PivotGridField ColumnField { get { return Item.ColumnField; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("PivotGridCellTemplateContainerRowField")]
#endif
		public PivotGridField RowField { get { return Item.RowField; } }
		public object GetFieldValue(PivotGridField field) { return Item.GetFieldValue(field); }
	}
}
