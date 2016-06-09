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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public class TreeListDataHeaderInnerControl : TreeListTableBase {
		TreeListDataColumn dataColumn;
		TableCell titleCell, imageCell;
		Image image;
		public TreeListDataHeaderInnerControl(TreeListRenderHelper helper, TreeListDataColumn dataColumn)
			: base(helper) {
			if(dataColumn == null)
				throw new ArgumentNullException("dataColumn");
			this.dataColumn = dataColumn;
		}
		protected TreeListDataColumn DataColumn { get { return dataColumn; } }
		protected TableCell TitleCell { get { return titleCell; } }
		protected TableCell ImageCell { get { return imageCell; } }
		protected Image Image { get { return image; } }
		protected override void CreateControlHierarchy() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			this.titleCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TitleCell);
			RenderHelper.CreateHeaderCaption(TitleCell, DataColumn);
			if(DataColumn.IsSorted()) {
				this.imageCell = RenderUtils.CreateTableCell();
				row.Cells.Add(ImageCell);
				this.image = RenderUtils.CreateImage();
				ImageCell.Controls.Add(Image);
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderStyle(DataColumn).AssignToControl(TitleCell, AttributesRange.Cell | AttributesRange.Font);
			Width = Unit.Percentage(100);
			if(Image != null) {
				string name = DataColumn.SortOrder == ColumnSortOrder.Ascending
					? TreeListImages.SortAscendingName
					: TreeListImages.SortDescendingName;
				RenderHelper.GetImage(name).AssignToControl(Image, DesignMode);
			}			
			if(ImageCell != null) {
				TreeListStyles.AppendDefaultClassName(ImageCell);
				ImageCell.Style[HtmlTextWriterStyle.TextAlign] = RenderHelper.IsRightToLeft ? "left" : "right";
				ImageCell.Width = 1;
				if(Image != null)
					RenderUtils.SetStyleUnitAttribute(Image, RenderHelper.IsRightToLeft ? "maring-right" : "margin-left", RenderHelper.GetHeaderSortImageSpacing(DataColumn));
			}
			TreeListStyles.AppendDefaultClassName(this, TitleCell);
		}
	}
	public class TreeListEditFormTable : TreeListTableBase {
		int editRowIndex;
		Unit editCellWidth;
		TreeListEditFormLayout layout;
		bool renderButtons;
		public TreeListEditFormTable(TreeListRenderHelper helper, int editRowIndex, bool renderButtons)
			: base(helper) {
			this.editRowIndex = editRowIndex;
			this.layout = new TreeListEditFormLayoutCalculator(TreeList).Calculate();
			this.editCellWidth = Unit.Percentage(100 / Layout.ColumnCount);
			this.renderButtons = renderButtons;
		}
		protected ASPxTreeList TreeList { get { return RenderHelper.TreeList; } }
		protected int EditRowIndex { get { return editRowIndex; } }
		protected Unit EditorCellWidth { get { return editCellWidth; } }
		protected TreeListEditFormLayout Layout { get { return layout; } }
		protected override void CreateControlHierarchy() {
			for(int i = 0; i < layout.RowCount; i++)
				CreateRow(i);
			if(this.renderButtons)
				CreateUpdateCancelRow();
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			CellSpacing = 0;
		}
		void CreateRow(int layoutRowIndex) {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			for(int i = 0; i < Layout.ColumnCount; i++) {
				int columnIndex = layout[layoutRowIndex, i];
				if(columnIndex < 0)
					continue;
				TreeListDataColumn column = TreeList.Columns[columnIndex] as TreeListDataColumn;
				if(column.EditFormSettings.CaptionLocation == TreeListColumnEditCaptionLocation.Near)
					row.Cells.Add(new TreeListEditFormCaptionCell(RenderHelper, column));
				row.Cells.Add(new TreeListEditFormEditorCell(RenderHelper, EditRowIndex, column, EditorCellWidth));
			}
		}
		void CreateUpdateCancelRow() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			TreeListCommandCell cell = new TreeListCommandCell(RenderHelper, -1, TreeListUtils.FindCommandColumnForEditForm(TreeList));
			row.Cells.Add(cell);
			cell.ColumnSpan = TreeListRenderHelper.FilterTableSpanValue(2 * Layout.ColumnCount);
			cell.HorizontalAlign = RenderHelper.IsRightToLeft ? HorizontalAlign.Left : HorizontalAlign.Right;			
		}
	}
	public class TreeListEditFormEditorCell : TreeListEditorCellBase {
		public TreeListEditFormEditorCell(TreeListRenderHelper helper, int rowIndex, TreeListDataColumn column, Unit width)
			: base(helper, rowIndex, column) {
			Width = width;
		}
		protected override EditorInplaceMode EditMode { get { return EditorInplaceMode.EditForm; } }
		protected override bool NeedTopCaption { get { return Column.EditFormSettings.CaptionLocation == TreeListColumnEditCaptionLocation.Top; } }
		protected override bool NeedBorderClassName { get { return false; } }
		protected override bool BorderBottomVisible { get { return false; } }
		protected TreeListColumnEditFormSettings FormSettings { get { return Column.EditFormSettings; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetEditFormEditCellStyle(Column).AssignToControl(this, true);
			RowSpan = TreeListRenderHelper.FilterTableSpanValue(FormSettings.RowSpan);
			int span = 2 * FormSettings.ColumnSpan;
			if(FormSettings.CaptionLocation == TreeListColumnEditCaptionLocation.Near)
				span -= 1;
			ColumnSpan = span;
		}
	}
	public class TreeListEditFormCaptionCell : TreeListCellBase {
		TreeListDataColumn column;		
		public TreeListEditFormCaptionCell(TreeListRenderHelper helper, TreeListDataColumn column)
			: base(helper) {
			this.column = column;
		}
		protected TreeListDataColumn Column { get { return column; } }
		protected override bool NeedBorderClassName { get { return false; } }
		protected override bool BorderRightVisible { get { return false; } }
		protected override bool BorderLeftVisible { get { return false; } }
		protected override bool BorderTopVisible { get { return false; } }
		protected override bool BorderBottomVisible { get { return false; } }
		protected override void CreateControlHierarchy() {
			WebControl label = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
			Controls.Add(label);
			string text = String.Format(TreeListRenderHelper.EditCaptionFormat, Column.GetCaption());
			label.Controls.Add(RenderUtils.CreateLiteralControl(text));
		}
		protected override void PrepareControlHierarchy() {			
			IAssociatedControlID assocControl = FindControl(RenderHelper.GetEditorId(Column)) as IAssociatedControlID;
			if(assocControl != null)
				(Controls[0] as WebControl).Attributes["for"] = assocControl.ClientID();
			RowSpan = Column.EditFormSettings.RowSpan;			
			RenderHelper.GetEditFormCaptionStyle(Column).AssignToControl(this, true);
		}
	}
	public class TreeListButtonInfo {
		protected TreeListButtonInfo() {
		}
		public static TreeListButtonInfo Create(TreeListRenderHelper helper, TreeListCommandColumn column, TreeListCommandColumnButtonType type, string nodeKey) {
			TreeListButtonInfo info = new TreeListButtonInfo();
			info.RenderHelper = helper;
			info.ButtonID = GenerateButtonID(column, GetNodeRowIndex(helper, nodeKey), (int)type);
			info.ButtonText = helper.GetCommandButtonText(column, type);
			info.ButtonClickArguments = helper.GetCommandButtonClickArguments(type, nodeKey);
			info.ButtonType = column != null ? column.ButtonType : ButtonType.Link;
			if(column != null){
				info.ButtonImage = helper.GetCommandColumnButton(column, type).Image;
				info.ButtonStyles = helper.GetCommandColumnButton(column, type).Styles;
			}
			return info;
		}
		public static TreeListButtonInfo Create(TreeListRenderHelper helper, TreeListCommandColumn column, int customButtonIndex, string nodeKey) {
			TreeListButtonInfo info = new TreeListButtonInfo();
			info.RenderHelper = helper;
			info.ButtonID = GenerateButtonID(column, GetNodeRowIndex(helper, nodeKey), - (customButtonIndex + 1));
			TreeListCommandColumnCustomButton button = column.CustomButtons[customButtonIndex];
			info.ButtonClickArguments = helper.GetCustomButtonClickArguments(nodeKey, customButtonIndex, button.GetID());
			info.ButtonText = button.GetText();
			info.ButtonImage = button.Image;
			info.ButtonStyles = button.Styles;
			info.ButtonType = column.ButtonType;
			return info;
		}
		static string GenerateButtonID(TreeListCommandColumn column, int rowIndex, int buttonType) {
			int columnIndex = column != null ? column.Index : -1;
			return string.Format("DXCBtn_{0}_{1}_{2}", rowIndex, columnIndex, buttonType);
		}
		static int GetNodeRowIndex(TreeListRenderHelper helper, string nodeKey) {
			TreeListRowInfo foundRowInfo = helper.TreeDataHelper.Rows.FirstOrDefault(r => string.Equals(r.NodeKey, nodeKey));
			return helper.TreeDataHelper.Rows.IndexOf(foundRowInfo);
		}
		protected TreeListRenderHelper RenderHelper { get; private set; }
		protected ButtonType ButtonType { get; private set; }
		protected string ButtonText { get; private set; }
		protected string ButtonID { get; private set; }
		protected object[] ButtonClickArguments { get; private set; }
		protected ImageProperties ButtonImage { get; private set; }
		protected ButtonControlStyles ButtonStyles { get; private set; }
		protected TreeListCommandButtonsHelper CommandButtonsHelper { get { return RenderHelper.TreeList.CommandButtonsHelper; } }
		public ASPxButton CreateControl() {
			ASPxCommandButton button = new ASPxCommandButton();
			button.ID = ButtonID;
			button.AutoPostBack = false;
			button.CausesValidation = false;
			button.EnableViewState = false;
			button.EncodeHtml = false;
			button.UseSubmitBehavior = false;
			button.RenderMode = ButtonType == ButtonType.Button ? ButtonRenderMode.Button : ButtonRenderMode.Link;
			button.ParentSkinOwner = RenderHelper.TreeList;
			if(ButtonType != ButtonType.Image)
				button.Text = ButtonText;
			button.AddClickArguments(ButtonClickArguments);
			AssignImageAndStylesToButton(button);
			CommandButtonsHelper.Register(button);
			return button;
		}
		void AssignImageAndStylesToButton(ASPxCommandButton button) {
			button.Image.Assign(ButtonImage);
			if(ButtonStyles != null) {
				button.FocusRectBorder.CopyFrom(ButtonStyles.FocusRectStyle.Border);
				button.FocusRectPaddings.CopyFrom(ButtonStyles.FocusRectStyle.Paddings);
				RenderHelper.GetCommandButtonStyle(ButtonStyles.Style).AssignToControl(button);
			}
		}
	}
	[ToolboxItem(false)]
	public class TreeListCommandCellSpacer : WebControl {
		public TreeListCommandCellSpacer(Unit width)
			: base(HtmlTextWriterTag.Span) {
			RenderUtils.SetHorizontalMargins(this, Unit.Empty, width);
			Font.Size = FontUnit.Parse("1px");
			Controls.Add(TreeListRenderHelper.CreateNbsp());
		}
	}
}
