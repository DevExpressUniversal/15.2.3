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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public abstract class ValidationSummaryControl : ASPxInternalWebControl {
		#region Nested Types
		protected class ErrorContainer {
			private WebControl errorContainerControl;
			private WebControl errorTextContainerControl;
			private HyperLink link;
			public ErrorContainer(WebControl errorContainerControl, WebControl errorTextContainerControl) {
				this.errorContainerControl = errorContainerControl;
				this.errorTextContainerControl = errorTextContainerControl;
			}
			public WebControl ErrorContainerControl {
				get { return errorContainerControl; }
			}
			public WebControl ErrorTextContainerControl {
				get { return errorTextContainerControl; }
			}
			public HyperLink Link {
				get { return link; }
				set { link = value; }
			}
		}
		#endregion
		private ASPxValidationSummary validationSummary;
		private Table rootTable;
		private TableCell rootCell;
		private ValidationSummaryHeaderControl headerControl;
		private WebControl errorsContainer;
		private List<ErrorContainer> errorContainers = new List<ErrorContainer>();
		public ValidationSummaryControl(ASPxValidationSummary validationSummary)
			: base() {
			this.validationSummary = validationSummary;
		}
		protected ASPxValidationSummary ValidationSummary {
			get { return validationSummary; }
		}
		protected bool IsRightToLeft {
			get { return (ValidationSummary as ISkinOwner).IsRightToLeft(); }
		}
		protected Table RootTable {
			get { return rootTable; }
		}
		protected TableCell RootCell {
			get { return rootCell; }
		}
		protected ValidationSummaryHeaderControl HeaderControl {
			get { return headerControl; }
		}
		protected WebControl ErrorsContainer {
			get { return errorsContainer; }
		}
		protected List<ErrorContainer> ErrorContainers {
			get { return errorContainers; }
		}
		protected override void ClearControlFields() {
			this.rootTable = null;
			this.rootCell = null;
			this.headerControl = null;
			ErrorContainers.Clear();
			this.errorsContainer = null;
		}
		protected override void CreateControlHierarchy() {
			CreateRootTable();
			if(ValidationSummary.HasHeader)
				CreateHeader();
			CreateErrorsContainer();
			if(DesignMode)
				CreateErrorContainers();
		}
		private void CreateRootTable() {
			this.rootTable = RenderUtils.CreateTable();
			Controls.Add(RootTable);
			TableRow row = RenderUtils.CreateTableRow();
			RootTable.Rows.Add(row);
			this.rootCell = RenderUtils.CreateTableCell();
			row.Cells.Add(RootCell);
		}
		private void CreateHeader() {
			this.headerControl = new ValidationSummaryHeaderControl(ValidationSummary);
			RootCell.Controls.Add(HeaderControl);
		}
		private void CreateErrorsContainer() {
			this.errorsContainer = CreateErrorsContainerCore();
			RootCell.Controls.Add(ErrorsContainer);
		}
		private void CreateErrorContainers() {
			foreach(ValidationSummaryError error in ValidationSummary.Errors) {
				ErrorContainer errorContainer = AddErrorTo(ErrorsContainer);
				ErrorContainers.Add(errorContainer);
				string errorText = ValidationSummary.HtmlEncode(error.ErrorText);
				AddErrorTextAndLinkTo(errorContainer, errorText);
			}
		}
		private void AddErrorTextAndLinkTo(ErrorContainer errorContainer, string errorText) {
			WebControl errorTextContainer = errorContainer.ErrorTextContainerControl;
			if(ValidationSummary.ShowErrorAsLink) {
				HyperLink link = errorContainer.Link = RenderUtils.CreateHyperLink();
				errorTextContainer.Controls.Add(link);
				link.Text = errorText;
				link.NavigateUrl = RenderUtils.AccessibilityEmptyUrl;
			} else
				errorTextContainer.Controls.Add(new LiteralControl(errorText));
		}
		protected abstract WebControl CreateErrorsContainerCore();
		protected abstract ErrorContainer AddErrorTo(WebControl errorsContainer);
		protected override void PrepareControlHierarchy() {
			PrepareRootTable();
			PrepareErrorsContainer();
			if(DesignMode) {
				foreach(ErrorContainer errorContainer in ErrorContainers)
					PrepareErrorContainer(errorContainer);
			}
		}
		private void PrepareRootTable() {
			RenderUtils.AssignAttributes(ValidationSummary, RootTable);
			AppearanceStyleBase rootTableStyle = ValidationSummary.GetRootTableStyle();
			rootTableStyle.AssignToControl(RootTable);
			AppearanceStyle rootCellStyle = ValidationSummary.GetRootCellStyle();
			rootCellStyle.AssignToControl(RootCell, true);
		}
		protected virtual void PrepareErrorsContainer() {
		}
		protected virtual void PrepareErrorContainer(ErrorContainer errorContainer) {
			ValidationSummaryErrorStyle errorStyle = ValidationSummary.GetErrorStyle();
			errorStyle.AssignToControl(errorContainer.ErrorContainerControl);
			errorContainer.ErrorContainerControl.Height = errorStyle.Height;
			if(errorContainer.Link != null) {
				AppearanceStyleBase linkStyle = ValidationSummary.GetLinkStyle();
				linkStyle.AssignToControl(errorContainer.Link);
			}
		}
		public string GetSampleErrorContainerRenderResult() {
			WebControl dummyErrorsContainer = CreateErrorsContainerCore();
			ErrorContainer errorContainer = AddErrorTo(dummyErrorsContainer);
			AddErrorTextAndLinkTo(errorContainer, "&nbsp;");
			PrepareErrorContainer(errorContainer);
			return RenderUtils.GetRenderResult(errorContainer.ErrorContainerControl);
		}
		protected void AddCssClassTo(WebControl control, string cssClass) {
			control.CssClass = RenderUtils.CombineCssClasses(control.CssClass, cssClass);
		}
	}
	public class ValidationSummaryTableControl : ValidationSummaryControl {
		public ValidationSummaryTableControl(ASPxValidationSummary validationSummary)
			: base(validationSummary) {
		}
		protected override WebControl CreateErrorsContainerCore() {
			Table table = RenderUtils.CreateTable(true);
			if(!DesignMode) {
				TableRow fakeRow = RenderUtils.CreateTableRow();
				fakeRow.ID = ASPxValidationSummary.FakeItemID;
				table.Rows.Add(fakeRow);
				fakeRow.Cells.Add(RenderUtils.CreateTableCell());
			}
			return table;
		}
		protected override ErrorContainer AddErrorTo(WebControl errorsContainer) {
			Table table = (Table)errorsContainer;
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return new ErrorContainer(row, cell);
		}
		protected override void PrepareErrorsContainer() {
			base.PrepareErrorsContainer();
			ErrorsContainer.Width = Unit.Percentage(100);
			AppearanceStyleBase style = ValidationSummary.GetTableErrorContainerStyle();
			style.AssignToControl(ErrorsContainer);
		}
		protected override void PrepareErrorContainer(ValidationSummaryControl.ErrorContainer errorContainer) {
			base.PrepareErrorContainer(errorContainer);
			AppearanceStyleBase style = ValidationSummary.GetErrorTextCellStyle();
			style.AssignToControl(errorContainer.ErrorTextContainerControl, true);
		}
	}
	public class ValidationSummaryListControl : ValidationSummaryControl {
		private ListType listType;
		public ValidationSummaryListControl(ASPxValidationSummary validationSummary, ValidationSummaryRenderMode renderMode)
			: base(validationSummary) {
			this.listType = renderMode == ValidationSummaryRenderMode.BulletedList ? ListType.Bulleted : ListType.Ordered;
		}
		private ListType ListType {
			get { return listType; }
		}
		protected override WebControl CreateErrorsContainerCore() {
			WebControl list = RenderUtils.CreateList(ListType);
			if(!DesignMode) {
				WebControl fakeItem = RenderUtils.CreateListItem();
				fakeItem.ID = ASPxValidationSummary.FakeItemID;
				list.Controls.Add(fakeItem);
			}
			return list;
		}
		protected override ErrorContainer AddErrorTo(WebControl errorsContainer) {
			WebControl listItem = RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
			errorsContainer.Controls.Add(listItem);
			return new ErrorContainer(listItem, listItem);
		}
		protected override void PrepareErrorsContainer() {
			base.PrepareErrorsContainer();
			Unit margin = ValidationSummary.GetBulletIndent();
			RenderUtils.SetMargins(ErrorsContainer, IsRightToLeft ? 0 : margin, 0, IsRightToLeft ? margin : 0, 0);
			AppearanceStyleBase style = ValidationSummary.GetListErrorContainerStyle();
			style.AssignToControl(ErrorsContainer, true);
		}
	}
	public class ValidationSummaryHeaderControl : ASPxInternalWebControl {
		private ASPxValidationSummary validationSummary;
		private Table table;
		private TableCell cell;
		public ValidationSummaryHeaderControl(ASPxValidationSummary validationSummary) {
			this.validationSummary = validationSummary;
		}
		protected ASPxValidationSummary ValidationSummary {
			get { return validationSummary; }
		}
		protected Table Table {
			get { return table; }
		}
		protected TableCell Cell {
			get { return cell; }
		}
		protected string HeaderText {
			get { return ValidationSummary.HtmlEncode(ValidationSummary.HeaderText); }
		}
		protected override void ClearControlFields() {
			this.table = null;
			this.cell = null;
		}
		protected override void CreateControlHierarchy() {
			this.table = RenderUtils.CreateTable();
			Controls.Add(Table);
			TableRow row = RenderUtils.CreateTableRow();
			Table.Rows.Add(row);
			this.cell = RenderUtils.CreateTableCell();
			row.Cells.Add(Cell);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase headerTableStyle = ValidationSummary.GetHeaderTableStyle();
			headerTableStyle.AssignToControl(Table);
			AppearanceStyle headerStyle = ValidationSummary.GetHeaderStyle();
			headerStyle.AssignToControl(Cell, true);
			Cell.Controls.Add(new LiteralControl(HeaderText));
		}
	}
}
