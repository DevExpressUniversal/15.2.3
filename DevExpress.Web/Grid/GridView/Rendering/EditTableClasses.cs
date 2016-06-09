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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
namespace DevExpress.Web.Rendering {
	public class GridViewTableInlineEditRow : GridViewTableRow {
		public GridViewTableInlineEditRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter) 
			: base(renderHelper, visibleIndex, hasGroupFooter) { 
		}
		public override GridViewRowType RowType { get { return GridViewRowType.InlineEdit; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EditingRowID;
			CreateLeftIndentCells();
			for(int i = 0; i < LeafColumns.Count; i++) {
				TableCell cell = RenderHelper.CreateInlineEditorCell(LeafColumns[i].Column, i, VisibleIndex);
				Cells.Add(cell);
			}
			CreateRightIndentCells();
			RenderHelper.AddHorzScrollExtraCell(this);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetInlineEditRowStyle().AssignToControl(this, false);
			Grid.RaiseHtmlRowPrepared(this); 
		}
		protected override TableCell CreateAdaptiveDetailButtonCell() {
			return new GridViewTableAdaptiveDetailButtonCell(RenderHelper, VisibleIndex);
		}
		public override bool RemoveExtraIndentBottomBorder() {
			return HasGroupFooter;
		}
	}	
	public class GridViewTableEditFormRow : GridViewTableRow {
		public GridViewTableEditFormRow(GridViewRenderHelper renderHelper, int visibleIndex, bool hasGroupFooter)
			: base(renderHelper, visibleIndex, hasGroupFooter) {
		}
		public override GridViewRowType RowType { get { return GridViewRowType.EditForm; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EditingRowID;
			CreateLeftIndentCells();
			Cells.Add(new GridViewTableEditFormCell(RenderHelper, VisibleIndex));
			CreateRightIndentCells();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetEditFormRowStyle().AssignToControl(this, AttributesRange.Common | AttributesRange.Font);
			Grid.RaiseHtmlRowPrepared(this);
		}
		public override bool RemoveExtraIndentBottomBorder() {
			return HasGroupFooter;
		}
	}
	public class GridViewTableEditFormCell : GridTableCell {
		int visibleIndex;
		public GridViewTableEditFormCell(GridViewRenderHelper renderHelper, int visibleIndex)
			: base(renderHelper) {
			this.visibleIndex = visibleIndex;
		}
		protected internal new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EditFormCellID;
			ColumnSpan = RenderHelper.ColumnSpanCount;
			if(!RenderHelper.AddEditFormTemplateControl(this, VisibleIndex)) {
				Controls.Add(CreateMainTable());
			}
			RenderHelper.Grid.RaiseHtmlEditFormCreated(this);
		}
		protected Table CreateMainTable() {
			return new GridViewEditFormTable(RenderHelper, VisibleIndex);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.AppendGridCssClassName(this);
			RenderHelper.GetEditFormRowCellStyle().AssignToControl(this, true);
		}
	}
	public class GridViewEditFormTable : InternalTable {
		int visibleIndex;
		GridViewRenderHelper renderHelper;
		bool renderUpdateCancelButtons;
		public GridViewEditFormTable(GridViewRenderHelper renderHelper, int visibleIndex) : this(renderHelper, visibleIndex, true) { }
		public GridViewEditFormTable(GridViewRenderHelper renderHelper, int visibleIndex, bool renderUpdateCancelButtons) {
			this.renderHelper = renderHelper;
			this.visibleIndex = visibleIndex;
			this.renderUpdateCancelButtons = renderUpdateCancelButtons;
		}
		protected GridViewRenderHelper RenderHelper { get { return renderHelper; } }
		protected ASPxGridView Grid { get { return RenderHelper.Grid; } }
		protected int VisibleIndex { get { return visibleIndex; } }
		protected bool RenderUpdateCancelButtons { get { return renderUpdateCancelButtons; } }
		protected WebDataProxy DataProxy { get { return RenderHelper.DataProxy; } }
		protected override void CreateControlHierarchy() {
			ID = GridViewRenderHelper.EditFormTableID;
			if(Grid.SettingsEditing.UseFormLayout) {
				TableRow row = RenderUtils.CreateTableRow();
				GridViewFormLayoutContainerCell cell = new GridViewFormLayoutContainerCell(RenderHelper, VisibleIndex, renderUpdateCancelButtons);
				row.Cells.Add(cell);
				Rows.Add(row);
			}
			else
				CreateStandardEditForm();
		}
		protected void CreateStandardEditForm() {
			int totalCellCount = 1;
			var layout = GridViewEditFormLayout.CreateLayout(Grid);
			foreach(var level in layout) {
				TableRow row = RenderUtils.CreateTableRow();
				Rows.Add(row);
				foreach(var item in level) {
					if(item.Type == GridViewEditFormLayoutItemType.Empty) {
						var cell = RenderUtils.CreateTableCell();
						cell.ColumnSpan = item.ColSpan;
						row.Cells.Add(cell);
					}
					if(item.Type == GridViewEditFormLayoutItemType.Caption)
						row.Cells.Add(new GridViewTableEditFormEditorCaptionCell(RenderHelper, item.Column));
					if(item.Type == GridViewEditFormLayoutItemType.Editor) {
						var cell = new GridViewTableEditFormEditorCell(RenderHelper, item.Column, VisibleIndex, item.CaptionLocation == ASPxColumnCaptionLocation.Top);
						row.Cells.Add(cell);
						cell.ColumnSpan = item.ColSpan;
						cell.RowSpan = item.RowSpan;
						cell.Width = item.Width;
					}
				}
			}
			if(layout.Count > 0) {
				var firstLevel = layout[0];
				totalCellCount = firstLevel.Sum(i => {
					if(i.Type == GridViewEditFormLayoutItemType.Editor || i.Type == GridViewEditFormLayoutItemType.Empty)
						return i.ColSpan;
					return 1;
				});
			}
			if(RenderUpdateCancelButtons)
				CreateUpdateCancelRow(totalCellCount);
		}
		protected void CreateUpdateCancelRow(int columnSpan) {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			var commandItems = new GridCommandButtonType[] { GridCommandButtonType.Update, GridCommandButtonType.Cancel };
			TableCell cell = new GridViewCommandItemsCell(RenderHelper, commandItems, true);
			row.Cells.Add(cell);
			cell.ColumnSpan = columnSpan;
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			GridViewEditFormTableStyle style = RenderHelper.GetEditFormTableStyle();
			style.AssignToControl(this, true);
			CellSpacing = (int)style.Spacing.Value;
		}
	}
	public class GridViewFormLayoutContainerCell : GridTableCell {
		int visibleIndex;
		bool renderUpdateCancelButtons;
		public GridViewFormLayoutContainerCell(GridViewRenderHelper renderHelper, int visibleIndex, bool renderUpdateCancelButtons) : base(renderHelper) {
			this.visibleIndex = visibleIndex;
			this.renderUpdateCancelButtons = renderUpdateCancelButtons;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected GridViewFormLayoutProperties EditFormLayoutProperties {
			get { return RenderHelper.Grid.EditFormLayoutProperties; }
		}
		protected ASPxFormLayout CreateFormLayout() {
			EditFormLayoutProperties.ValidateLayoutItemColumnNames();
			ASPxFormLayout formLayout = new ASPxFormLayout();
			formLayout.ID = GridRenderHelper.EditFormLayoutID;
			formLayout.ParentSkinOwner = RenderHelper.Grid;
			formLayout.Width = Unit.Percentage(100);
			formLayout.EnableViewState = false;
			formLayout.Properties.Assign(EditFormLayoutProperties);
			formLayout.Properties.DataOwner = EditFormLayoutProperties.DataOwner;
			if(formLayout.Items.IsEmpty) {
				var prop = RenderHelper.Grid.GenerateDefaultLayout(false);
				formLayout.ColCount = prop.ColCount;
				formLayout.Items.Assign(prop.Items);
			}
			if(!this.renderUpdateCancelButtons)
				HideCommandLayoutItems(formLayout);
			return formLayout;
		}
		protected void HideCommandLayoutItems(ASPxFormLayout formLayout) {
			formLayout.ForEach(delegate(LayoutItemBase item) {
				if(item is EditModeCommandLayoutItem)
					item.Visible = false;
			});
		}
		protected override void CreateControlHierarchy() {
			ASPxFormLayout formLayout = CreateFormLayout();
			Controls.Add(formLayout);
			int requiredFieldDefaultCount = formLayout.GetRequiredFieldCount();
			formLayout.ForEach(delegate(LayoutItemBase item) {
				ContentPlaceholderLayoutItem placeHolderItem = item as ContentPlaceholderLayoutItem;
				if(placeHolderItem != null)
					CreateItemContent(placeHolderItem);
			});
			if(requiredFieldDefaultCount != formLayout.GetRequiredFieldCount())
				formLayout.ResetControlHierarchy();
		}
		protected void CreateItemContent(ContentPlaceholderLayoutItem layoutItem) {
			if(!layoutItem.IsVisible())
				return;
			if(layoutItem is GridViewColumnLayoutItem)
				CreateColumnLayoutItemContent(layoutItem as GridViewColumnLayoutItem);
			if(layoutItem is EditModeCommandLayoutItem)
				CreateCommandLayoutItemContent(layoutItem as EditModeCommandLayoutItem);
		}
		protected void CreateColumnLayoutItemContent(GridViewColumnLayoutItem layoutItem) {
			GridViewDataColumn column = layoutItem.Column as GridViewDataColumn;
			if(RenderHelper.AddEditFormLayoutItemTemplateControl(layoutItem, visibleIndex, column))
				return;
			if(column == null)
				return;
			if(RenderHelper.AddEditItemTemplateControl(visibleIndex, column, layoutItem.NestedControlContainer)) {
				RenderHelper.EnsureNestedControlContainerRecursive(layoutItem.NestedControlContainer);
				return;
			}
			ASPxEditBase editor = CreateEditor(column, layoutItem);
			if(editor.Height == Unit.Percentage(100))
				layoutItem.Height = Unit.Percentage(100);
		}
		protected ASPxEditBase CreateEditor(GridViewDataColumn column, ColumnLayoutItem layoutItem) {
			return RenderHelper.CreateEditor(visibleIndex, column, layoutItem.NestedControlContainer,
					EditorInplaceMode.EditForm, layoutItem.GetRowSpan(), true, () => { 
						ASPxEditBase result = layoutItem.GetNestedControl() as ASPxEditBase;
						if(result != null)
							result.Value = RenderHelper.Grid.DataProxy.GetEditingRowValue(visibleIndex, column.FieldName);
						return result;
					});
		}
		protected void CreateCommandLayoutItemContent(EditModeCommandLayoutItem layoutItem) {
			if(layoutItem.ShowUpdateButton)
				GridViewCommandItemsCell.CreateUpdateButton(layoutItem.NestedControlContainer, RenderHelper.Grid, !RenderHelper.RequireRenderEditFormPopup);
			if(layoutItem.ShowCancelButton)
				GridViewCommandItemsCell.CreateCancelButton(layoutItem.NestedControlContainer, RenderHelper.Grid, !RenderHelper.RequireRenderEditFormPopup);
		}
	}
	public class GridViewTableEditFormEditorCaptionCell : GridTableCell {
		GridViewDataColumn column;
		public GridViewTableEditFormEditorCaptionCell(GridViewRenderHelper helper, GridViewDataColumn column)
			: base(helper) {
			this.column = column;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected GridViewDataColumn Column { get { return column; } }
		protected override void CreateControlHierarchy() {
			WebControl label = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
			Controls.Add(label);
			label.Controls.Add(RenderUtils.CreateLiteralControl(Column.EditFormCaption));
		}
		protected override void PrepareControlHierarchy() {
			IAssociatedControlID assocControl = FindControl(RenderHelper.GetEditorId(Column)) as IAssociatedControlID;
			if(assocControl != null)
				(Controls[0] as WebControl).Attributes["for"] = assocControl.ClientID();
			RenderHelper.GetEditFormEditorCaptionStyle(Column).AssignToControl(this, true);
		}
	}
	public abstract class GridViewTableEditorCellBase : GridViewTableBaseCell {
		WebControl caption;
		ASPxEditBase editor;
		bool hasTopCaption;
		public GridViewTableEditorCellBase(GridViewRenderHelper renderHelper, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: this(renderHelper, column, visibleIndex, removeLeftBorder, removeRightBorder, false) { }
		public GridViewTableEditorCellBase(GridViewRenderHelper renderHelper, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder, bool hasTopCaption)
			: base(renderHelper, null, column, visibleIndex, removeLeftBorder, removeRightBorder) {
			this.hasTopCaption = hasTopCaption;
		}
		protected new GridViewDataColumn Column { get { return base.Column as GridViewDataColumn; } }
		protected abstract EditorInplaceMode GetEditMode();
		protected bool HasTopCaption { get { return hasTopCaption; } }
		protected WebControl Caption { get { return caption; } }
		protected override void CreateControlHierarchy() {
			if(HasTopCaption) {
				this.caption = RenderUtils.CreateDiv();
				Controls.Add(Caption);
				WebControl label = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
				Caption.Controls.Add(label);
				label.Controls.Add(RenderUtils.CreateLiteralControl(Column.EditFormCaption));
			}
			if(!RenderHelper.AddEditItemTemplateControl(VisibleIndex, Column, this))
				this.editor = RenderHelper.CreateEditor(VisibleIndex, Column, this, GetEditMode(), RowSpan);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(HasTopCaption)
				PrepareCaption();
		}
		protected virtual void PrepareCaption() {
			IAssociatedControlID assocControl = FindControl(RenderHelper.GetEditorId(Column)) as IAssociatedControlID;
			if(assocControl != null)
				(Caption.Controls[0] as WebControl).Attributes["for"] = assocControl.ClientID();
			GetCaptionStyle().AssignToControl(Caption, true);
		}
		protected virtual AppearanceStyle GetCaptionStyle() {
			GridViewEditFormCaptionStyle style = new GridViewEditFormCaptionStyle();
			style.Paddings.Assign(new Paddings(0));
			style.CopyFrom(RenderHelper.GetEditFormEditorCaptionStyle(Column));
			return style;
		}
	}
	public class GridViewTableEditFormEditorCell : GridViewTableEditorCellBase {
		public GridViewTableEditFormEditorCell(GridViewRenderHelper renderHelper, GridViewDataColumn column, int visibleIndex, bool hasTopCaption)
			: base(renderHelper, column, visibleIndex, false, false, hasTopCaption) {
		}
		protected override EditorInplaceMode GetEditMode() { return EditorInplaceMode.EditForm; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetEditFormEditorCellStyle(Column).AssignToControl(this, true);
			base.PrepareControlHierarchy();
		}
	}
	public class GridViewTableInlineEditorCell : GridViewTableEditorCellBase {
		public GridViewTableInlineEditorCell(GridViewRenderHelper renderHelper, GridViewDataColumn column, int visibleIndex, bool removeLeftBorder, bool removeRightBorder)
			: base(renderHelper, column, visibleIndex, removeLeftBorder, removeRightBorder) { }
		protected override EditorInplaceMode GetEditMode() { return EditorInplaceMode.Inplace; }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetInlineEditCellStyle(Column).AssignToControl(this, true);
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
			RenderHelper.SetCellWidthIfRequired(Column, this, VisibleIndex);
		}
	}
	public class GridViewTableEditingErrorRow : GridViewTableRow {
		bool isStyledRow;
		public GridViewTableEditingErrorRow(GridViewRenderHelper renderHelper, bool isStyledRow) 
			: this(renderHelper, isStyledRow, false) { }
		public GridViewTableEditingErrorRow(GridViewRenderHelper renderHelper, bool isStyledRow, bool hasGroupFooter)
			: base(renderHelper, -1, hasGroupFooter) {
			this.isStyledRow = isStyledRow;
		}
		protected bool IsStyledRow { get { return isStyledRow; } }
		public override GridViewRowType RowType { get { return GridViewRowType.EditingErrorRow; } }
		protected override void CreateControlHierarchy() {
			if(!IsStyledRow && RenderHelper.HasEditingError) {
				ID = GridViewRenderHelper.EditingErrorItemID;
			}
			CreateLeftIndentCells();
			Cells.Add(new GridViewTableEditingErrorCell(RenderHelper, IsStyledRow));
			CreateRightIndentCells();
		}
		protected override TableCell CreateIndentTableCell() {
			return new GridViewTableIndentCell(RenderHelper);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetEditingErrorRowStyle().AssignToControl(this, false);
			base.PrepareControlHierarchy();
		}
		public override bool RemoveExtraIndentBottomBorder() {
			return HasGroupFooter;
		}
	}
	public class GridViewTableEditingErrorCell : GridTableCell {
		bool isStyledCell;
		public GridViewTableEditingErrorCell(GridViewRenderHelper renderHelper, bool isStyledCell)
			: base(renderHelper, false, true) {
			this.isStyledCell = isStyledCell;
		}
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected bool IsStyledCell { get { return isStyledCell; } }
		protected override void CreateControlHierarchy() {
			if(IsStyledCell) {
				RenderUtils.SetStringAttribute(this, "data-colSpan", RenderHelper.ColumnSpanCount.ToString());
			} else {
				ColumnSpan = RenderHelper.ColumnSpanCount;
				Text = RenderHelper.EditingErrorText;
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.AppendGridCssClassName(this);
			base.PrepareControlHierarchy();
		}
	}
}
