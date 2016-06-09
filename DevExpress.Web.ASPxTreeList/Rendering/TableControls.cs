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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public class TreeListTableBase : InternalTable {
		TreeListRenderHelper renderHelper;
		public TreeListTableBase(TreeListRenderHelper helper) 
			: base() {
			this.renderHelper = helper;			
		}
		protected TreeListRenderHelper RenderHelper { get { return renderHelper; } }
	}
	public abstract class TreeListRowBase : InternalTableRow {
		TreeListRenderHelper renderHelper;
		public TreeListRowBase(TreeListRenderHelper renderHelper) 
			: base() {
			this.renderHelper = renderHelper;			
		}
		protected TreeListRenderHelper RenderHelper { get { return renderHelper; } }
	}
	public abstract class TreeListDataRowBase : TreeListRowBase {
		int rowIndex;		
		public TreeListDataRowBase(TreeListRenderHelper helper, int rowIndex)
			: base(helper) {
			this.rowIndex = rowIndex;			
		}
		public abstract TreeListRowKind Kind { get; }
		public int Level { get { return RenderHelper.GetLevelFromIndentCount(RowInfo.Indents.Count); } }
		protected int RowIndex { get { return rowIndex; } }
		protected internal TreeListRowInfo RowInfo { get { return RenderHelper.GetRowByIndex(RowIndex); } }		
		protected void RaisePrepared() {
			RenderHelper.TreeList.RaiseHtmlRowPrepared(this);
		}
		protected void CreateDataRowIndentCells() {
			int maxIndentIndex = RowInfo.Indents.Count - 1;
			for(int i = 0; i < maxIndentIndex; i++)
				Cells.Add(CreateIndentCell(i, false));
			if(maxIndentIndex > -1)
				Cells.Add(CreateIndentCell(maxIndentIndex, true));
		}
		TableCell CreateIndentCell(int indentIndex, bool leaf) {
			TableCell cell;
			TreeListRowIndentType indent = RowInfo.Indents[indentIndex];
			if(leaf)
				cell = new TreeListLeafCell(RenderHelper, RowIndex, indent);
			else
				cell = new TreeListTreeLineCell(RenderHelper, RowIndex, indent);
			return cell;
		}
	}	
	public abstract class TreeListWideDataRow : TreeListDataRowBase {
		bool isAuxRow;
		public TreeListWideDataRow(TreeListRenderHelper helper, int rowIndex, bool isAuxRow)
			: base(helper, rowIndex) {
			this.isAuxRow = isAuxRow;
		}
		protected bool IsAuxRow { get { return isAuxRow; } }
		protected abstract TableCell CreateCell();
		protected override void CreateControlHierarchy() {
			if(IsAuxRow) {
				foreach(TreeListRowIndentType indent in RowInfo.Indents)
					Cells.Add(new TreeListTreeLineCell(RenderHelper, RowIndex, RenderHelper.FilterAuxRowIndent(indent)));				
			} else {
				CreateDataRowIndentCells();
			}
			Cells.Add(CreateCell());
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {
			RaisePrepared();
		}
	}
	public abstract class TreeListCellBase : InternalTableCell {		
		const int
			BorderLeftValue = 1,
			BorderBottomValue = 2,
			BorderRightValue = 4,
			BorderTopValue = 8;
		TreeListRenderHelper renderHelper;
		public TreeListCellBase(TreeListRenderHelper renderHelper) 
			: base() {
			this.renderHelper = renderHelper;			
		}
		protected TreeListRenderHelper RenderHelper { get { return renderHelper; } }
		protected abstract bool BorderRightVisible { get; }
		protected abstract bool BorderLeftVisible { get; }
		protected abstract bool BorderTopVisible { get; }
		protected abstract bool BorderBottomVisible { get; }
		protected virtual bool NeedBorderClassName { get { return false; } }		
		protected void AppendBorderClassName() {
			if(NeedBorderClassName) {
				int code = GetBorderCode();
				if(code < 15) {
					string name = String.Format("{0}{1:X}", TreeListRenderHelper.BorderClassNamePrefix, code);
					RenderUtils.AppendDefaultDXClassName(this, name);
				}
			}
		}
		protected void AppendSystemIndentClassName() {
			string name;
			if(RenderUtils.Browser.IsIE)
				name = TreeListRenderHelper.IndentCellSystemClassNameIE;
			else
				name = TreeListRenderHelper.IndentCellSystemClassName;
			RenderUtils.AppendDefaultDXClassName(this, name);
		}
		protected int GetBorderCode() {
			int value = 0;
			if(BorderLeftVisible) value += RenderHelper.IsRightToLeft ? BorderRightValue : BorderLeftValue;
			if(BorderBottomVisible) value += BorderBottomValue;
			if(BorderRightVisible) value += RenderHelper.IsRightToLeft ? BorderLeftValue : BorderRightValue;
			if(BorderTopVisible) value += BorderTopValue;
			return value;			
		}
		protected bool IsInLastHtmlRow() {			
			Table table = RenderHelper.TreeList.GetDataTable();
			TableRow row = Parent as TableRow;
			if(table == null || row == null)
				return false;
			int index = table.Rows.Count - 1;
			TableRow lastRow = table.Rows[index];
			if(lastRow == row)
				return true;
			if(lastRow is TreeListErrorRow && index > 0)
				lastRow = table.Rows[index - 1];
			return lastRow == row;
		}		
	}
	public abstract class TreeListDataCellBase : TreeListCellBase {
		int rowIndex;
		public TreeListDataCellBase(TreeListRenderHelper helper, int rowIndex)
			: base(helper) {
			this.rowIndex = rowIndex;
		}
		public int Level { get { return RenderHelper.GetLevelFromIndentCount(RowInfo.Indents.Count); } }
		protected override bool NeedBorderClassName { get { return true; } }		
		protected int RowIndex { get { return rowIndex; } }
		protected internal TreeListRowInfo RowInfo { 
			get {
				if(RowIndex == TreeListRenderHelper.PopupEditFormRowIndex)
					return RenderHelper.TreeDataHelper.GetPopupEditFormRowInfo();
				return RenderHelper.GetRowByIndex(RowIndex);
			} 
		}
		protected TreeListRowInfo PrevRowInfo { get { return RenderHelper.GetRowByIndex(RowIndex - 1); } }
		protected TreeListRowInfo NextRowInfo { get { return RenderHelper.GetRowByIndex(RowIndex + 1); } }
		protected bool GetBorderTopVisibleCore() {
			if(!RenderHelper.HorzGridLinesVisible)
				return false;
			if(RowIndex == 0)
				return !RenderHelper.IsHeaderRowVisible && !RenderHelper.SuppressOuterGridLines;
			TreeListRowInfo prev = PrevRowInfo;
			return prev != null && prev.Indents.Count > RowInfo.Indents.Count;
		}
		protected bool GetBorderBottomVisibleCore(bool isAuxRow) {
			if(!RenderHelper.HorzGridLinesVisible)
				return false;
			if(!isAuxRow && TreeListBuilderHelper.GetAuxRowCount(RenderHelper, RowInfo.NodeKey, true) > 0)
				return true;
			TreeListRowInfo next = NextRowInfo;
			return (next != null && next.Indents.Count >= RowInfo.Indents.Count) || (!RenderHelper.SuppressOuterGridLines && IsInLastHtmlRow());
		}
		protected bool GetBorderLeftVisibleCore(TreeListColumn column) {
			bool first = column == RenderHelper.FirstVisibleColumn;
			if(first && RenderHelper.SuppressOuterGridLines && RowInfo.Indents.Count < 1 && !RenderHelper.IsRowSelectable(RowInfo))
				return false;
			return RenderHelper.GridLines == GridLines.Both || (RenderHelper.VerticalGridLinesVisible && !first);
		}
		protected bool GetBorderRightVisibleCore(TreeListColumn column) {
			return RenderHelper.VerticalGridLinesVisible && column == RenderHelper.LastVisibleColumn && !RenderHelper.SuppressOuterGridLines;
		}
		protected void SetFirstDataCellColSpan() {
			int value = RenderHelper.GetFirstDataColumnSpan(RowInfo.Indents.Count);
			if(RenderHelper.IsSelectionEnabled && !RenderHelper.IsRowSelectable(RowInfo))
				value++;
			ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(value);
		}
	}
	public abstract class TreeListColumnDataCellBase : TreeListDataCellBase {
		TreeListDataColumn column;
		public TreeListColumnDataCellBase(TreeListRenderHelper helper, int rowIndex, TreeListDataColumn column) 
			: base(helper, rowIndex) {
			this.column = column;
		}
		protected ASPxTreeList TreeList { get { return RenderHelper.TreeList; } }
		protected internal TreeListDataColumn Column { get { return column; } }
		protected bool IsFirst { get { return Column == RenderHelper.FirstVisibleColumn; } }
		protected override bool BorderLeftVisible { get { return GetBorderLeftVisibleCore(Column); } }
		protected override bool BorderRightVisible { get { return GetBorderRightVisibleCore(Column); } }
		protected override bool BorderTopVisible { get { return GetBorderTopVisibleCore(); } }		
	}
	public abstract class TreeListWideDataCell : TreeListDataCellBase {
		bool isAuxRow;
		public TreeListWideDataCell(TreeListRenderHelper helper, int rowIndex, bool isAuxRow) 
			: base(helper, rowIndex) {
			this.isAuxRow = isAuxRow;
		}
		protected bool IsAuxRow { get { return isAuxRow; } }
		protected override bool BorderLeftVisible {
			get {
				if(!RenderHelper.IsRootIndentVisible && RenderHelper.SuppressOuterGridLines && RowInfo.Indents.Count < 1)
					return false;
				return RenderHelper.GridLines == GridLines.Both;
			}
		}
		protected override bool BorderRightVisible { 
			get { return RenderHelper.VerticalGridLinesVisible && !RenderHelper.SuppressOuterGridLines; } 
		}
		protected override bool BorderTopVisible { 
			get { return IsAuxRow ? false : GetBorderTopVisibleCore(); } 
		}
		protected override bool BorderBottomVisible { 
			get { return GetBorderBottomVisibleCore(IsAuxRow); } 
		}
		protected abstract AppearanceStyleBase GetStyle();
		protected override void PrepareControlHierarchy() {
			int span = RenderHelper.GetWideDataCellColumnSpan(RowInfo.Indents.Count);
			ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(span);
			GetStyle().AssignToControl(this, true);			
			AppendBorderClassName();
		}
	}
	#region Headers
	public class TreeListHeaderRow : TreeListRowBase {
		public TreeListHeaderRow(TreeListRenderHelper helper)
			: base(helper) {
			ID = TreeListRenderHelper.HeaderSuffix;
		}
		public override TableRowSection TableSection { get { return TableRowSection.TableHeader; } set { } }
		protected override void CreateControlHierarchy() {
			bool rootVisible = RenderHelper.IsRootIndentVisible;
			if(rootVisible)
				Cells.Add(new TreeListHeaderIndentCell(RenderHelper, true));
			if(RenderHelper.IsSelectionEnabled) {
				if(RenderHelper.IsSelectAllCheckVisible)
					Cells.Add(new TreeListHeaderSelectAllCell(RenderHelper));
				else
					Cells.Add(new TreeListHeaderIndentCell(RenderHelper, !rootVisible));
			}
			int count = RenderHelper.VisibleColumns.Count;
			for(int i = 0; i < count; i++) {
				TreeListColumn column = RenderHelper.VisibleColumns[i];
				Cells.Add(new TreeListHeaderCell(RenderHelper, column, i < 1));
			}
			if(count < 1)
				Cells.Add(new TreeListHeaderEmptyCell(RenderHelper));
			RenderHelper.AddHorzScrollExtraCell(this, true);
		}
		protected override void PrepareControlHierarchy() {
			if(RenderUtils.Browser.Family.IsNetscape)
				RenderUtils.SetStyleStringAttribute(this, "-moz-user-select", "none");
			if(RenderUtils.Browser.Platform.IsMSTouchUI)
				RenderUtils.AppendDefaultDXClassName(this, TreeListHeaderStyle.MSTouchDraggableMarkerCssClassName);
		}
	}
	public abstract class TreeListHeaderCellBase : TreeListCellBase {
		public TreeListHeaderCellBase(TreeListRenderHelper helper) 
			: base(helper) {
		}
		protected override HtmlTextWriterTag TagKey { get { return RenderHelper.TreeList.IsAccessibilityCompliantRender() && !HasText ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Th; } }
		protected override bool NeedBorderClassName { get { return true; } }
		protected override bool BorderTopVisible { get { return !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderBottomVisible { get { return true; } }
		protected virtual bool HasText { get { return false; } }
		protected override void PrepareControlHierarchy() {
			TreeListHeaderStyle style = GetStyle();
			style.AssignToControl(this, true);
			RenderUtils.SetWrap(this, style.Wrap);
			AppendBorderClassName();
		}
		protected virtual TreeListHeaderStyle GetStyle() {
			return RenderHelper.GetHeaderStyle(null);
		}
	}
	public class TreeListHeaderIndentCell : TreeListHeaderCellBase {
		bool isLeft;
		public TreeListHeaderIndentCell(TreeListRenderHelper helper, bool isLeft)
			: base(helper) {
			this.isLeft = isLeft;
		}
		protected bool IsLeft { get { return isLeft; } }
		protected override bool BorderLeftVisible { get { return !IsLeft || !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderRightVisible { get { return false; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(TreeListRenderHelper.CreateNbsp());			
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			AppendSystemIndentClassName();
		}
	}
	public class TreeListHeaderSelectAllCell : TreeListHeaderCellBase, IInternalCheckBoxOwner {
		InternalCheckboxControl check;
		public TreeListHeaderSelectAllCell(TreeListRenderHelper helper) 
			: base(helper) {
		}
		protected InternalCheckboxControl Check { get { return check; } }
		protected override bool BorderRightVisible { get { return false; } }
		protected override bool BorderLeftVisible {
			get { return RenderHelper.IsRootIndentVisible || !RenderHelper.SuppressOuterGridLines; }
		}
		protected override void CreateControlHierarchy() {
			this.check = new InternalCheckboxControl(this);
			Controls.Add(Check);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetPadding(this, 0);
			RenderUtils.SetHorizontalAlign(this, HorizontalAlign.Center);
			AppendSystemIndentClassName();
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(RenderUtils.IsHtml5Mode(this) && RenderHelper.TreeList.IsAccessibilityCompliantRender()))
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("checkbox", (this as IInternalCheckBoxOwner).CheckState);
			settings.Add("aria-label", AccessibilityUtils.TreeListSelectAllCheckBoxText);
			return settings;
		}
		#region IInternalCheckBoxOwner
		CheckState IInternalCheckBoxOwner.CheckState {
			get { return RenderHelper.RootRowInfo.CheckState; }
		}
		bool IInternalCheckBoxOwner.ClientEnabled {
			get { return RenderHelper.IsEnabled; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return RenderHelper.GetCheckBoxImage((this as IInternalCheckBoxOwner).CheckState);
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return TreeListRenderHelper.SelectAllCheckID;
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return RenderHelper.GetICBStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
	}
	public class TreeListHeaderEmptyCell : TreeListHeaderCellBase {
		public TreeListHeaderEmptyCell(TreeListRenderHelper helper) 
			: base(helper) {
			ID = TreeListRenderHelper.DragAndDropTargetMark + TreeListRenderHelper.EmptyHeaderSuffix;
		}
		protected override bool BorderRightVisible { get { return !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderLeftVisible {
			get { return !RenderHelper.SuppressOuterGridLines || RenderHelper.IsSelectionEnabled || RenderHelper.IsRootIndentVisible; }
		}
		protected override void CreateControlHierarchy() {
			ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(RenderHelper.HeaderFirstColumnSpan);
			Controls.Add(TreeListRenderHelper.CreateNbsp());
		}
	}
	public class TreeListHeaderCell : TreeListHeaderCellBase {
		TreeListColumn column;
		bool isFirst;
		public TreeListHeaderCell(TreeListRenderHelper helper, TreeListColumn column, bool isFirst) 
			: base(helper) {
			if(column == null)
				throw new ArgumentNullException("column");
			this.column = column;
			this.isFirst = isFirst;
			if(helper.IsColumnClickable(column))				
				ID = RenderHelper.GetHeaderCellId(column.Index);
		}
		protected override bool BorderRightVisible { 
			get { return !Column.Visible || (Column == RenderHelper.LastVisibleColumn && !RenderHelper.SuppressOuterGridLines); } 
		}
		protected override bool BorderTopVisible { 
			get { return !Column.Visible || base.BorderTopVisible; }
		}
		protected override bool BorderLeftVisible {
			get {
				return !Column.Visible || !IsFirst
					|| !RenderHelper.SuppressOuterGridLines
					|| RenderHelper.IsSelectionEnabled
					|| RenderHelper.IsRootIndentVisible;
			}
		}
		protected override bool HasText { get { return !string.IsNullOrEmpty(Column.GetCaption()); } }
		protected TreeListColumn Column { get { return column; } }
		protected bool IsFirst { get { return isFirst; } }	
		protected override void CreateControlHierarchy() {
			TreeListDataColumn dataColumn = Column as TreeListDataColumn;
			if(dataColumn != null && dataColumn.IsSorted()) {
				Controls.Add(new TreeListDataHeaderInnerControl(RenderHelper, dataColumn));
			} else {
				TreeListCommandColumn commandColumn = Column as TreeListCommandColumn;
				if(commandColumn != null && commandColumn.ShowNewButtonInHeader && RenderHelper.SettingsDataSecurity.AllowInsert) {
					Control button = TreeListButtonInfo.Create(RenderHelper, commandColumn, TreeListCommandColumnButtonType.New, String.Empty).CreateControl();		
					Controls.Add(button);
				} else {
					RenderHelper.CreateHeaderCaption(this, Column);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!RenderHelper.RequireFixedTableLayout)
				Width = Column.Width;
			if(RenderHelper.IsColumnClickable(Column)) {
				RenderUtils.SetCursor(this, RenderUtils.GetPointerCursor());
				RenderUtils.SetStringAttribute(this, TouchUtils.TouchMouseDownEventName, RenderHelper.GetHeaderOnMouseDown());
			}
			if(RenderHelper.ContextMenuEventAssigned)
				RenderUtils.SetStringAttribute(this, "oncontextmenu", RenderHelper.GetOnContextMenu("Header", column.Index));
			if(IsFirst)
				ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(RenderHelper.HeaderFirstColumnSpan);
			TreeListStyles.AppendDefaultClassName(this);
			if(RenderHelper.TreeList.IsAccessibilityCompliantRender()) {
				RenderUtils.SetStringAttribute(this, "scope", "col");
				if(!RenderUtils.IsHtml5Mode(this))
					RenderUtils.SetStringAttribute(this, "abbr", Column.GetCaption());
			}
			ToolTip = Column.ToolTip;
			RenderUtils.AllowEllipsisInText(this, Column.GetAllowEllipsisInText());
		}
		protected override TreeListHeaderStyle GetStyle() {
			TreeListHeaderStyle style = RenderHelper.GetHeaderStyle(Column);
			if(RenderHelper.IsRightToLeft) {
				if(style.HorizontalAlign == HorizontalAlign.NotSet)
					style.HorizontalAlign = HorizontalAlign.Right;
			}
			return style;
		}
	}
	#endregion
	#region Indent cells
	public abstract class TreeListIndentCellBase : TreeListDataCellBase {
		public TreeListIndentCellBase(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex) {
		}
		protected override bool NeedBorderClassName { get { return false; } }
		protected override bool BorderTopVisible { get { return false; } }
		protected override bool BorderRightVisible { get { return false; } }
		protected override bool BorderBottomVisible { get { return false; } }
		protected override bool BorderLeftVisible { get { return false; } }
	}
	public class TreeListTreeLineCell : TreeListIndentCellBase {
		TreeListRowIndentType indent;
		public TreeListTreeLineCell(TreeListRenderHelper helper, int rowIndex, TreeListRowIndentType indent)
			: base(helper, rowIndex) {
			this.indent = indent;
		}
		protected TreeListRowIndentType Indent { get { return indent; } }
		protected virtual bool IsLeaf { get { return false; } }
		protected override void PrepareControlHierarchy() {			
			RenderHelper.GetTreeLineCellStyle(RowInfo, Indent, IsLeaf).AssignToControl(this, true);
			if(Controls.Count == 0)
				Text = "&nbsp;";
			AppendSystemIndentClassName();			
		}
	}
	public class TreeListLeafCell : TreeListTreeLineCell {
		Image imageControl;
		HyperLink link;
		public TreeListLeafCell(TreeListRenderHelper helper, int rowIndex, TreeListRowIndentType indent)
			: base(helper, rowIndex, indent) {
		}
		protected override bool IsLeaf { get { return true; } }
		protected bool Accessible { get { return RenderHelper.TreeList.IsAccessibilityCompliantRender(); } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.HasNodeImage(RowInfo)) {
				this.imageControl = RenderUtils.CreateImage();
				if(Accessible && RenderHelper.IsEnabled) {
					this.link = RenderUtils.CreateHyperLink();
					Controls.Add(this.link);
					this.link.Controls.Add(this.imageControl);
				} else {
					Controls.Add(this.imageControl);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(this.imageControl != null) {
				RenderHelper.GetNodeImage(RowInfo).AssignToControl(this.imageControl, DesignMode);
				if(RenderHelper.IsEnabled) {
					string className = RowInfo.Expanded
						? TreeListRenderHelper.CollapseButtonClassName
						: TreeListRenderHelper.ExpandButtonClassName;
					this.imageControl.CssClass = RenderUtils.CombineCssClasses(this.imageControl.CssClass, className);
				}
			}
			if(this.link != null) {
				this.link.NavigateUrl = RenderHelper.GetToggleExpansionUrl(RowInfo);
				if(RenderUtils.IsHtml5Mode(this) && Accessible) {
					if(RenderUtils.Browser.Family.IsWebKit)
						RenderUtils.SetStringAttribute(this.link, "role", "gridcell");
					RenderUtils.SetStringAttribute(this.link, "aria-label", string.Format(AccessibilityUtils.TreeListCollapseExpandButtonFormatString,
						RenderHelper.Rows[RowIndex].Expanded ? AccessibilityUtils.TreeListCollapseText : AccessibilityUtils.TreeListExpandText, RowIndex + 1));
				}
			}
		}
	}
	public class TreeListSelectionCell : TreeListIndentCellBase, IInternalCheckBoxOwner {
		private InternalCheckboxControl check = null;
		public TreeListSelectionCell(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex) {
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(RenderUtils.IsHtml5Mode(this) && RenderHelper.TreeList.IsAccessibilityCompliantRender()))
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("checkbox", (this as IInternalCheckBoxOwner).CheckState);
			string label = string.Format(AccessibilityUtils.TreeListDataCheckBoxDescriptionFormatString, Level, 
				RenderHelper.Rows[RowIndex].Selected ? "" : AccessibilityUtils.Not);
			if(RenderHelper.Rows[RowIndex].HasButton)
				label += RenderHelper.Rows[RowIndex].Expanded ? AccessibilityUtils.TreeListNodeExpandedStateText : AccessibilityUtils.TreeListNodeCollapsedStateText; 
			for(int i = 0; i < RenderHelper.AllDataColumns.Count; i++) {
				if(!RenderHelper.AllDataColumns[i].Visible)
					continue;
				string fieldName = RenderHelper.AllDataColumns[i].FieldName;
				string caption = RenderHelper.AllDataColumns[i].GetCaption();
				string format = RenderHelper.AllDataColumns[i].DisplayFormat;
				string value = string.Format(string.IsNullOrEmpty(format) ? "{0}" : format, RenderHelper.Rows[RowIndex].GetValue(fieldName));
				label += String.Format(AccessibilityUtils.TableItemFormatString, caption, value);
			}
			settings.Add("aria-label", label);
			return settings;
		}
		#region IInternalCheckBoxOwner
		CheckState IInternalCheckBoxOwner.CheckState {
			get { return RowInfo.TreeListData.GetNodeCheckState(RowInfo.NodeKey); }
		}
		bool IInternalCheckBoxOwner.ClientEnabled {
			get { return RenderHelper.IsEnabled; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return RenderHelper.GetCheckBoxImage((this as IInternalCheckBoxOwner).CheckState);
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return string.Empty;
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return RenderHelper.GetICBStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
		protected InternalCheckboxControl Check { get { return check; } }
		protected override bool NeedBorderClassName { get { return true; } }
		protected override bool BorderLeftVisible { 
			get { return RenderHelper.GridLines == GridLines.Both && (RowInfo.Indents.Count > 0 || RenderHelper.IsRootIndentVisible || !RenderHelper.SuppressOuterGridLines); } 
		}
		protected override bool BorderRightVisible { get { return false; } }
		protected override bool BorderTopVisible { get { return GetBorderTopVisibleCore(); } }
		protected override bool BorderBottomVisible { get { return GetBorderBottomVisibleCore(false); } }
		protected override void CreateControlHierarchy() {
			this.check = new InternalCheckboxControl(this);
			Controls.Add(Check);
			Controls.Add(TreeListRenderHelper.CreateNbsp());
		}
		protected override void PrepareControlHierarchy() {			
			RenderHelper.GetSelectionCellStyle().AssignToControl(this, true);
			AppendBorderClassName();
			AppendSystemIndentClassName();			
		}
	}
	#endregion
	#region Data
	public class TreeListDataRow : TreeListDataRowBase {		
		Style savedStyle = null;
		public TreeListDataRow(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex) {						
			ID = helper.GetDataRowId(rowIndex);
		}
		public override TreeListRowKind Kind { get { return TreeListRowKind.Data; } }		
		protected internal Style SavedStyle { 
			get {
				if(savedStyle == null)
					savedStyle = new Style();
				return savedStyle; 
			} 
		}
		protected override void CreateControlHierarchy() {
			CreateDataRowIndentCells();
			if(RenderHelper.IsRowSelectable(RowInfo))
				Cells.Add(new TreeListSelectionCell(RenderHelper, RowIndex));
			IList<TreeListColumn> columns = RenderHelper.VisibleColumns;
			int count = columns.Count;
			for(int i = 0; i < count; i++) {				
				TreeListDataColumn dataColumn = columns[i] as TreeListDataColumn;
				if(dataColumn != null) {
					Cells.Add(new TreeListDataCell(RenderHelper, RowIndex, dataColumn));
				} else {
					TreeListCommandColumn commandColumn = columns[i] as TreeListCommandColumn;
					if(commandColumn != null)
						Cells.Add(new TreeListCommandCell(RenderHelper, RowIndex, commandColumn));
					else
						throw new NotImplementedException();
				}
			}
			if(count < 1)
				Cells.Add(new TreeListEmptyDataCell(RenderHelper, RowIndex));
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {			
			bool editingNode = RowInfo.NodeKey == RenderHelper.EditingNodeKey;
			bool isSelected = RenderHelper.IsRowSelectable(RowInfo) && RowInfo.Selected;
			bool isFocused = RenderHelper.IsFocusedNodeEnabled && RowInfo.Focused;
			if(editingNode) {				
				if(!isSelected && !isFocused)
					RenderHelper.GetEditFormDisplayNodeStyle().AssignToControl(this);
				else
					SavedStyle.CopyFrom(RenderHelper.GetEditFormDisplayNodeStyle());
			} else {
				if(RowIndex % 2 > 0 && RenderHelper.IsAlternatingNodeStyleEnabled)
					RenderHelper.GetAlternatingNodeStyle().AssignToControl(this);
				else
					RenderHelper.GetNodeStyle().AssignToControl(this);
			}
			RaisePrepared();
			if(RenderHelper.NeedExactStyleTable)
				SavedStyle.CopyFrom(ControlStyle);
			if(isFocused)
				RenderHelper.GetFocusedNodeStyle().AssignToControl(this);
			else if(isSelected)
				RenderHelper.GetSelectedNodeStyle().AssignToControl(this);
			if(RenderHelper.ContextMenuEventAssigned)
				RenderUtils.SetStringAttribute(this, "oncontextmenu", RenderHelper.GetOnContextMenu("Node", RowInfo.NodeKey));
		}
	}
	public class TreeListEmptyDataCell : TreeListDataCellBase {
		public TreeListEmptyDataCell(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex) {
		}
		protected override bool BorderLeftVisible { 
			get { 
				return RenderHelper.GridLines == GridLines.Both 
					&& (RenderHelper.IsRowSelectable(RowInfo) || RowInfo.Indents.Count > 0 || !RenderHelper.SuppressOuterGridLines); 
			} 
		}
		protected override bool BorderRightVisible { get { return !RenderHelper.SuppressOuterGridLines && RenderHelper.VerticalGridLinesVisible; } }
		protected override bool BorderTopVisible { get { return GetBorderTopVisibleCore(); } }
		protected override bool BorderBottomVisible { get { return GetBorderBottomVisibleCore(false); } }
		protected override void CreateControlHierarchy() {
			Controls.Add(TreeListRenderHelper.CreateNbsp());
		}
		protected override void PrepareControlHierarchy() {			
			SetFirstDataCellColSpan();
			RenderHelper.GetCellStyle(null).AssignToControl(this, true);
			TreeListStyles.AppendDefaultClassName(this);
			AppendBorderClassName();
		}
	}
	public class TreeListDataCell : TreeListColumnDataCellBase {	
		public TreeListDataCell(TreeListRenderHelper helper, int rowIndex, TreeListDataColumn column)
			: base(helper, rowIndex, column) {			
		}
		protected override bool BorderBottomVisible {
			get { return GetBorderBottomVisibleCore(false); }
		}
		protected override void CreateControlHierarchy() {
			Control control = RenderHelper.CreateDataCellTemplateContainer(this, RowInfo, Column);
			if(control != null)
				control.DataBind();
			else
				Controls.Add(RenderHelper.GetDataCellDisplayControl(RowInfo, Column));
		}
		protected override void PrepareControlHierarchy() {
			if(IsFirst)
				SetFirstDataCellColSpan();			
			RenderHelper.GetCellStyle(Column).AssignToControl(this, true);
			TreeList.RaiseHtmlDataCellPrepared(this);
			TreeListStyles.AppendDefaultClassName(this);
			AppendBorderClassName();
			RenderHelper.SetDefaultDisplayControlAlign(this, Column);
			if(RenderHelper.NeedDataRowPointerCursor(RowInfo))
				RenderUtils.SetCursor(this, RenderUtils.GetPointerCursor());
			if(RowIndex == 0 && !RenderHelper.IsHeaderRowVisible && !RenderHelper.RequireFixedTableLayout)
				Width = Column.Width;
			RenderUtils.AllowEllipsisInText(this, Column.GetAllowEllipsisInText());
		}
	}
	#endregion
	#region Preview
	public class TreeListPreviewRow : TreeListWideDataRow {
		public TreeListPreviewRow(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex, true) {
		}
		public override TreeListRowKind Kind { get { return TreeListRowKind.Preview; } }
		protected override TableCell CreateCell() {
			return new TreeListPreviewCell(RenderHelper, RowIndex);
		}
	}
	public class TreeListPreviewCell : TreeListWideDataCell {
		public TreeListPreviewCell(TreeListRenderHelper helper, int rowIndex)
			: base(helper, rowIndex, true) {
		}		
		protected override void CreateControlHierarchy() {
			Control control = RenderHelper.CreatePreviewTemplateContainer(this, RowInfo);
			if(control != null)
				control.DataBind();
			else
				Text = RenderHelper.GetPreviewText(RowInfo, false);
		}
		protected override AppearanceStyleBase GetStyle() {
			return RenderHelper.GetPreviewStyle();
		}
	}
	#endregion
	#region Summary
	public class TreeListFooterRow : TreeListDataRowBase {
		bool total;
		public TreeListFooterRow(TreeListRenderHelper helper, int rowIndex, bool total) 
			: base(helper, rowIndex) {
			this.total = total;
		}
		public override TreeListRowKind Kind { get { return Total ? TreeListRowKind.Footer : TreeListRowKind.GroupFooter; } }
		protected bool Total { get { return total; } }
		protected override void CreateControlHierarchy() {
			foreach(TreeListRowIndentType indent in RowInfo.Indents)
				Cells.Add(new TreeListTreeLineCell(RenderHelper, RowIndex, RenderHelper.FilterAuxRowIndent(indent)));			
			if(!Total || RenderHelper.IsRootIndentVisible)
				Cells.Add(new TreeListTreeLineCell(RenderHelper, RowIndex, TreeListRowIndentType.None));
			int count = RenderHelper.VisibleColumns.Count;
			for(int i = 0; i < count; i++) {
				TreeListColumn column = RenderHelper.VisibleColumns[i];
				Cells.Add(new TreeListFooterCell(RenderHelper, RowIndex, column, Total));
			}
			if(count < 1)
				Cells.Add(new TreeListFooterCell(RenderHelper, RowIndex, null, Total));
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = Total ? RenderHelper.GetFooterStyle() : RenderHelper.GetGroupFooterStyle();
			style.AssignToControl(this);
			RaisePrepared();
		}
	}
	public class TreeListFooterCell : TreeListDataCellBase {
		bool total;
		TreeListColumn column;
		public TreeListFooterCell(TreeListRenderHelper helper, int rowIndex, TreeListColumn column, bool total)
			: base(helper, rowIndex) {			
			this.total = total;
			this.column = column;
		}
		protected bool IsFirst { get { return Column == null || Column == RenderHelper.FirstVisibleColumn; } }
		protected bool IsLast { get { return Column == null || Column == RenderHelper.LastVisibleColumn; } }
		protected bool Total { get { return total; } }
		protected TreeListColumn Column { get { return column; } }		
		protected override bool BorderLeftVisible { 
			get {
				if(Total && !RenderHelper.IsRootIndentVisible && RenderHelper.SuppressOuterGridLines)
					return false;
				return RenderHelper.GridLines == GridLines.Both && IsFirst; 
			}
		}
		protected override bool BorderRightVisible { get { return RenderHelper.VerticalGridLinesVisible && IsLast && !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderTopVisible { get { return RenderHelper.HorzGridLinesVisible; } }
		protected override bool BorderBottomVisible { get { return !RenderHelper.SuppressOuterGridLines && RenderHelper.HorzGridLinesVisible && IsInLastHtmlRow(); } }
		protected override void CreateControlHierarchy() {
			Control control = RenderHelper.CreateFooterTemplateContainer(this, RowInfo, Column, Total);
			if(control != null)
				control.DataBind();
			else
				Text = RenderHelper.GetFooterText(RowInfo, Column);
			if(IsFirst) {
				int span = RenderHelper.GetFirstDataColumnSpan(1 + RowInfo.Indents.Count);
				if(RenderHelper.IsSelectionEnabled)
					span++;
				if(Total && !RenderHelper.IsRootIndentVisible)
					span++;
				ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(span);
			}
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = Total 
				? RenderHelper.GetFooterCellStyle(Column) 
				: RenderHelper.GetGroupFooterCellStyle(Column);
			style.AssignToControl(this, true);
			TreeListStyles.AppendDefaultClassName(this);
			AppendBorderClassName();
			TreeListDataColumn dataColumn = Column as TreeListDataColumn;
			if(dataColumn != null)
				RenderHelper.SetDefaultDisplayControlAlign(this, dataColumn);
		}
	}
	#endregion
	#region Editing
	public class TreeListInlineEditRow : TreeListDataRowBase {
		public TreeListInlineEditRow(TreeListRenderHelper helper, int rowIndex) 
			: base(helper, rowIndex) {
			ID = helper.GetDataRowId(rowIndex);
		}
		public override TreeListRowKind Kind { get { return TreeListRowKind.InlineEdit; } }
		protected override void CreateControlHierarchy() {
			CreateDataRowIndentCells();
			IList<TreeListColumn> columns = RenderHelper.VisibleColumns;
			int count = columns.Count;
			for(int i = 0; i < count; i++) {
				TreeListDataColumn dataColumn = columns[i] as TreeListDataColumn;
				if(dataColumn != null) {
					Cells.Add(new TreeListInlineEditorCell(RenderHelper, RowIndex, dataColumn));
				} else {
					TreeListCommandColumn commandColumn = columns[i] as TreeListCommandColumn;
					if(commandColumn != null)
						Cells.Add(new TreeListCommandCell(RenderHelper, RowIndex, commandColumn));
					else
						throw new NotImplementedException();
				}
			}
			if(count < 1)
				Cells.Add(new TreeListEmptyDataCell(RenderHelper, RowIndex));
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = RenderHelper.VisibleColumns.Count > 0
				? RenderHelper.GetInlineEditNodeStyle()
				: RenderHelper.GetNodeStyle();
			style.AssignToControl(this);
			RaisePrepared();
		}
	}
	public abstract class TreeListEditorCellBase : TreeListColumnDataCellBase {		
		public TreeListEditorCellBase(TreeListRenderHelper helper, int rowIndex, TreeListDataColumn column) 
			: base(helper, rowIndex, column) {
		}
		protected abstract EditorInplaceMode EditMode { get; }
		protected TreeListDataHelper TreeDataHelper { get { return TreeList.TreeDataHelper; } }
		protected string FieldName { get { return Column.FieldName; } }
		protected virtual bool NeedTopCaption { get { return false; } }
		protected override void CreateControlHierarchy() {
			Control control = RenderHelper.CreateEditCellTemplateContainer(this, RowInfo, Column);
			if(control == null) {
				if(NeedTopCaption) {
					string text = String.Format(TreeListRenderHelper.EditCaptionFormat, Column.GetCaption());
					Controls.Add(RenderUtils.CreateLiteralControl(text));
					Controls.Add(RenderUtils.CreateBr());
				}
				object value = TreeDataHelper.GetEditingValue(FieldName, RowInfo.GetValue(FieldName));
				ASPxEditBase editor = RenderHelper.CreateEditor(Column, value, EditMode, false);
				Controls.Add(editor);
				RenderHelper.PrepareEditor(editor, Column, ColumnSpan);
				TreeList.RaiseCellEditorInitialize(new TreeListColumnEditorEventArgs(RowInfo.NodeKey, Column, editor, value));
			}
		}
	}
	public class TreeListInlineEditorCell : TreeListEditorCellBase {
		public TreeListInlineEditorCell(TreeListRenderHelper helper, int rowIndex, TreeListDataColumn column) 
			: base(helper, rowIndex, column) {
		}
		protected override EditorInplaceMode EditMode { get { return EditorInplaceMode.Inplace; } }
		protected override bool BorderBottomVisible {
			get { return GetBorderBottomVisibleCore(false); }
		}
		protected override void PrepareControlHierarchy() {
			if(IsFirst)
				SetFirstDataCellColSpan();			
			RenderHelper.GetInlineEditCellStyle(Column).AssignToControl(this, true);
			TreeListStyles.AppendDefaultClassName(this);
			AppendBorderClassName();
			RenderHelper.SetDefaultDisplayControlAlign(this, Column);
		}		
	}
	public class TreeListEditFormRow : TreeListWideDataRow {
		public TreeListEditFormRow(TreeListRenderHelper helper, int rowIndex, bool isAuxRow)
			: base(helper, rowIndex, isAuxRow) {
			if(!isAuxRow)
				ID = helper.GetDataRowId(rowIndex);
		}
		public override TreeListRowKind Kind { get { return TreeListRowKind.EditForm; } }
		protected override TableCell CreateCell() {
			return new TreeListEditFormCell(RenderHelper, RowIndex, IsAuxRow);
		}
	}
	public class TreeListEditFormCell : TreeListWideDataCell {
		public TreeListEditFormCell(TreeListRenderHelper helper, int rowIndex, bool isAuxRow) 
			: base(helper, rowIndex, isAuxRow) {
		}
		protected override bool BorderRightVisible { get { return !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderLeftVisible { get { return RowInfo.Indents.Count > 0 || RenderHelper.IsRootIndentVisible || !RenderHelper.SuppressOuterGridLines; } }
		protected override bool BorderBottomVisible { 
			get {
				if(RenderHelper.HorzGridLinesVisible)
					return base.BorderBottomVisible;
				return !RenderHelper.SuppressOuterGridLines || !IsInLastHtmlRow();
			} 
		}
		protected override bool BorderTopVisible {
			get {
				if(RenderHelper.HorzGridLinesVisible)
					return base.BorderTopVisible;
				return RowIndex > 0 
					|| RenderHelper.EditMode == TreeListEditMode.EditFormAndDisplayNode 
					|| !RenderHelper.IsHeaderRowVisible && !RenderHelper.SuppressOuterGridLines;
			}
		}
		protected override void CreateControlHierarchy() {
			Control container = RenderHelper.CreateEditFormTemplateContainer(this, RowIndex);
			if(container == null)
				Controls.Add(new TreeListEditFormTable(RenderHelper, RowIndex, true));
		}
		protected override AppearanceStyleBase GetStyle() {
			return RenderHelper.GetEditFormStyle();
		}
	}
	public class TreeListErrorRow : TreeListWideDataRow {
		public TreeListErrorRow(TreeListRenderHelper helper, int rowIndex) 
			: base(helper, rowIndex, true) {
		}
		public override TreeListRowKind Kind { get { return TreeListRowKind.Error; } }
		protected override TableCell CreateCell() {
			return new TreeListErrorCell(RenderHelper, RowIndex);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetVisibility(this, !String.IsNullOrEmpty(RenderHelper.TreeDataHelper.EditingNodeError), true);
		}
	}
	public class TreeListErrorCell : TreeListWideDataCell {
		public TreeListErrorCell(TreeListRenderHelper helper, int rowIndex) 
			: base(helper, rowIndex, true) {
			ID = TreeListRenderHelper.ErrorCellSuffix;
		}
		protected override bool BorderTopVisible {
			get {
				if(!RenderHelper.HorzGridLinesVisible)
					return base.BorderTopVisible;
				TreeListRowInfo next = NextRowInfo;
				return next != null && next.Indents.Count < RowInfo.Indents.Count;
			}
		}
		protected override AppearanceStyleBase GetStyle() {
			return RenderHelper.GetErrorStyle();
		}
		protected override void CreateControlHierarchy() {
			Text = RenderHelper.TreeDataHelper.EditingNodeError;
		}
	}
	public class TreeListCommandCell : TreeListDataCellBase {
		TreeListCommandColumn column;
		public TreeListCommandCell(TreeListRenderHelper helper, int rowIndex, TreeListCommandColumn column)
			: base(helper, rowIndex) {
			this.column = column;
		}
		protected bool AllowInsert { get { return RenderHelper.SettingsDataSecurity.AllowInsert; } }
		protected bool AllowEdit { get { return RenderHelper.SettingsDataSecurity.AllowEdit; } }
		protected bool AllowDelete { get { return RenderHelper.SettingsDataSecurity.AllowDelete; } }
		public TreeListCommandColumn Column { get { return column; } }
		protected bool IsEditForm { get { return RowIndex < 0; } }
		protected string NodeKey {
			get {
				if(IsEditForm) return RenderHelper.EditingNodeKey;
				return RowInfo.NodeKey;
			}
		}
		protected override bool BorderRightVisible {
			get {
				if(IsEditForm) return false;
				return GetBorderRightVisibleCore(Column);
			}
		}
		protected override bool BorderLeftVisible {
			get {
				if(IsEditForm) return false;
				return GetBorderLeftVisibleCore(Column);
			}
		}
		protected override bool BorderTopVisible {
			get {
				if(IsEditForm) return false;
				return GetBorderTopVisibleCore();
			}
		}
		protected override bool BorderBottomVisible {
			get {
				if(IsEditForm) return false;
				return GetBorderBottomVisibleCore(false);
			}
		}
		protected override void CreateControlHierarchy() {
			if(IsEditForm) {
				CreateUpdateCancelButtons();
			} else {
				bool isEditing = RenderHelper.IsEditingKey(NodeKey);
				if(isEditing) {
					if(RenderHelper.EditMode == TreeListEditMode.Inline) {
						if(Column.NewButton.Visible || Column.EditButton.Visible || RenderHelper.HasSingleCommandColumn() )
							CreateUpdateCancelButtons();
					}
				} else {
					CreateEditNewDeleteButtons();
				}
				CreateCustomButtons(isEditing);
				if(Controls.Count < 1)
					Controls.Add(TreeListRenderHelper.CreateNbsp());
			}
		}
		protected override void PrepareControlHierarchy() {
			if(!IsEditForm) {
				if(!RenderHelper.RequireFixedTableLayout)
					Width = Column.Width;
				if(Column == RenderHelper.FirstVisibleColumn)
					SetFirstDataCellColSpan();
			}
			RenderHelper.GetCommandCellStyle(Column).AssignToControl(this, true);
			RenderHelper.TreeList.RaiseHtmlCommandCellPrepared(this);
			if(!HasVisibleChildren())
				Text = "&nbsp;"; 
			AppendBorderClassName();
			RenderUtils.AppendDefaultDXClassName(this, TreeListRenderHelper.CommandCellClassName);
		}
		void CreateUpdateCancelButtons() {
			if(Column == null)
				return;
			WebControl buttonsContainer = this;
			if(RenderUtils.IsHtml5Mode(this)) {
				buttonsContainer = new WebControl(HtmlTextWriterTag.Span);
				Controls.Add(buttonsContainer);
			}
			CreateButtonControl(TreeListCommandColumnButtonType.Update, Column.UpdateButton.Visible, buttonsContainer);
			CreateButtonControl(TreeListCommandColumnButtonType.Cancel, Column.CancelButton.Visible, buttonsContainer);
		}
		void CreateEditNewDeleteButtons() {
			if(NodeKey == TreeListRenderHelper.NewNodeKey)
				throw new InvalidOperationException();
			CreateButtonControl(TreeListCommandColumnButtonType.Edit, Column.EditButton.Visible);
			CreateButtonControl(TreeListCommandColumnButtonType.New, NodeKey != RenderHelper.NewNodeParentKey && Column.NewButton.Visible);
			CreateButtonControl(TreeListCommandColumnButtonType.Delete, Column.DeleteButton.Visible);
		}
		void CreateCustomButtons(bool isEditing) {
			for(int i = 0; i < Column.CustomButtons.Count; i++) {
				TreeListCommandColumnButtonEventArgs args = new TreeListCommandColumnButtonEventArgs(Column, NodeKey, TreeListCommandColumnButtonType.Custom, i);
				RenderHelper.TreeList.RaiseCommandColumnButtonInitialize(args);
				bool defaultVisible = Column.CustomButtons[i].IsVisible(isEditing);
				if(!IsButtonVisible(defaultVisible, args.Visible)) continue;
				CreateSpacerIfNeeded();
				WebControl control = TreeListButtonInfo.Create(RenderHelper, Column, i, NodeKey).CreateControl();
				if(args.Enabled == DefaultBoolean.False)
					control.Enabled = false;
				Controls.Add(control);
			}
		}
		void CreateButtonControl(TreeListCommandColumnButtonType type, bool visible) {
			CreateButtonControl(type, visible, this);
		}
		void CreateButtonControl(TreeListCommandColumnButtonType type, bool visible, WebControl container) {
			if(type == TreeListCommandColumnButtonType.New && !AllowInsert || type == TreeListCommandColumnButtonType.Edit && !AllowEdit ||
				type == TreeListCommandColumnButtonType.Delete && !AllowDelete)
				return;
			TreeListCommandColumnButtonEventArgs args = new TreeListCommandColumnButtonEventArgs(Column, NodeKey, type, -1);
			RenderHelper.TreeList.RaiseCommandColumnButtonInitialize(args);
			if(!IsButtonVisible(visible, args.Visible)) return;
			CreateSpacerIfNeeded();
			WebControl control = TreeListButtonInfo.Create(RenderHelper, Column, type, NodeKey).CreateControl();
			if(args.Enabled == DefaultBoolean.False)
				control.Enabled = false;
			if(container == null)
				container = this;
			container.Controls.Add(control);
		}
		void CreateSpacerIfNeeded() {
			if(Controls.Count < 1) return;
			Unit spacing = RenderHelper.GetCommandCellStyle(Column).Spacing;
			if(spacing.IsEmpty) return;
			Controls.Add(new TreeListCommandCellSpacer(spacing));
		}
		bool HasVisibleChildren() {
			foreach(Control child in Controls) {
				if(child.Visible) return true;
			}
			return false;
		}
		bool IsButtonVisible(bool defaultVisible, DefaultBoolean eventResult) {
			if(eventResult == DefaultBoolean.True)
				return true;
			if(eventResult == DefaultBoolean.False)
				return false;
			return defaultVisible;
		}
	}
	public class TreeListArmRow : TreeListRowBase {
		bool asHeader;
		public TreeListArmRow(TreeListRenderHelper helper, bool asHeader)
			: base(helper) {
				this.asHeader = asHeader;
		}
		protected bool AsHeader { get { return asHeader; } }
		public override TableRowSection TableSection { 
			get { return AsHeader ? TableRowSection.TableHeader : TableRowSection.TableBody; } 
			set { } 
		}
		protected override void CreateControlHierarchy() {
			var cellCount = RenderHelper.GetTotalCellCount();
			if(RenderHelper.HasHorizontalScrollBar)
				cellCount--;
			for(int i = 0; i < cellCount; i++)
				Cells.Add(CreateCell());
			RenderHelper.AddHorzScrollExtraCell(this, AsHeader);
		}
		protected virtual TableCell CreateCell() {
			bool renderAsTh = AsHeader && !RenderHelper.TreeList.IsAccessibilityCompliantRender();
			return renderAsTh ? new TableHeaderCell() : new TableCell();
		}
		protected override void PrepareControlHierarchy() {
			int indentCount = GetIndentCount();
			for(int i = 0; i < indentCount; i++)
				Cells[i].Width = TreeListRenderHelper.IndentDefaultWidth; 
			for(int i = 0; i < RenderHelper.VisibleColumns.Count; i++)
				Cells[indentCount + i].Width = GetColumnWidth(RenderHelper.VisibleColumns[i]);
		}
		protected virtual int GetIndentCount() {
			var value = RenderHelper.GetTotalCellCount() - RenderHelper.VisibleColumns.Count;
			if(RenderHelper.VisibleColumns.Count < 1)
				value--;
			if(RenderHelper.HasHorizontalScrollBar)
				value--;
			return value;
		}
		Unit GetColumnWidth(TreeListColumn column) {
			Unit width = column.Width;
			if(width.IsEmpty && RenderHelper.HasHorizontalScrollBar)
				width = 100;
			if(!RenderHelper.AllowColumnResizing || width.IsEmpty || width.Type != UnitType.Pixel)
				return width;
			return Math.Max((int)width.Value, RenderHelper.GetColumnMinWidth(column));
		}
	}
	public class TreeListHorzScrollExtraCell : TreeListCellBase {
		const string ExtraCellClassName = "dxtlHSEC";
		bool isHeaderCell;
		public TreeListHorzScrollExtraCell(TreeListRenderHelper helper, bool isHeaderCell)
			: base(helper) {
			this.isHeaderCell = isHeaderCell;
		}
		protected bool IsHeaderCell { get { return isHeaderCell; } }
		protected override HtmlTextWriterTag TagKey { get { return IsHeaderCell ? HtmlTextWriterTag.Th : HtmlTextWriterTag.Td; } }
		protected override bool BorderRightVisible { get { return false; } }
		protected override bool BorderLeftVisible { get { return false; } }
		protected override bool BorderTopVisible { get { return false; } }
		protected override bool BorderBottomVisible { get { return false; } }
		protected override bool NeedBorderClassName { get { return false; } }
		protected override void PrepareControlHierarchy() {
			CssClass = RenderUtils.CombineCssClasses(CssClass, ExtraCellClassName);
		}
	}
	#endregion
}
