#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Collections.Generic;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.Controls {
	[ToolboxItem(false)] 
	public class ErrorInfoControl : Panel { 
		private string errorImageName;
		private Table table;
		private ASPxCheckBox ignoreError;
		private void ignoreError_CheckedChanged(object sender, EventArgs e) {
			ErrorInfo error = ErrorHandling.Instance.GetPageError();
			if(error != null) {
				error.IgnoreError = ((ASPxCheckBox)sender).Checked;
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			this.Style.Clear();
			table.CssClass = "ErrorMessage";
			if(DesignMode) {
				SetupTableForDesignMode(table);
			}
			else {
				CreateErrorRow(table);
				ErrorHandling.Instance.ClearPageError();
			}
			base.Render(writer);
		}
		private void CreateErrorRow(Table table) {
			ErrorInfo error = ErrorHandling.Instance.GetPageError();
			if(error != null && Visible) {
				var validationException = error.Exception as ValidationException;
				if(validationException != null) {
					ProcessValidationException(validationException);
				}
				else {
					CreateErrorRow(table, ErrorImageName, error.GetErrorMessage());
				}
			}
			else {
				table.Visible = false;
			}
		}
		private void SetupTableForDesignMode(Table table) {
			TableRow row = new TableRow();
			table.Rows.Add(row);
			row.Cells.Add(new TableCell());
			row.Cells[0].Text = "Error message";
		}
		private void ProcessValidationException(ValidationException validationException) {
			TableRow row = new TableRow();
			table.Rows.Add(row);
			row.Cells.Add(new TableCell());
			row.Cells[0].Text = validationException.MessageHeader;
			row.Cells[0].ColumnSpan = 2;
			row.Cells[0].Attributes["id"] = ClientID + "_Header";
			row.Cells[0].Attributes[EasyTestTagHelper.TestField] = "ErrorInfo";
			row.Cells[0].Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;
			foreach(var validationResult in new[] { ValidationResultType.Error, ValidationResultType.Warning, ValidationResultType.Information }) {
				string errorMessage = validationException.GetMessages(validationResult);
				if(!string.IsNullOrEmpty(errorMessage)) {
					CreateErrorRow(table, validationResult.ToString(), errorMessage, "Validation" + validationResult);
				}
			}
			if(validationException.Result.ValidationOutcome == ValidationOutcome.Warning) {
				TableRow row2 = new TableRow();
				row2.Cells.Add(new TableCell());
				var cell = new TableCell();
				cell.ColumnSpan = 2;
				row2.Cells.Add(cell);
				table.Rows.Add(row2);
				cell.Controls.Add(ignoreError);
				ignoreError.Visible = true;
				ignoreError.Text = CaptionHelper.GetLocalizedText("Texts", "IgnoreWarning");
				ignoreError.Attributes[EasyTestTagHelper.TestField] = ignoreError.Text;
				ignoreError.Attributes[EasyTestTagHelper.TestControlClassName] = JSASPxCheckBoxTestControl.ClassName;
			}
		}
		protected void CreateErrorRow(Table table, string imageName, string message, string testTag = "ErrorInfo") {
			TableRow row = new TableRow();
			table.Rows.Add(row);
			if(!WebApplicationStyleManager.IsNewStyle) {
				var imageCell = new TableCell();
				if(!string.IsNullOrEmpty(imageName)) {
					Image image = new Image();
					image.ImageUrl = ImageLoader.Instance.GetImageInfo(imageName).ImageUrl;
					imageCell.CssClass = "ErrorImage";
					imageCell.Controls.Add(image);
					imageCell.VerticalAlign = VerticalAlign.Top;
					imageCell.HorizontalAlign = HorizontalAlign.Left;
				}
				row.Cells.Add(imageCell);
			}
			var labelCell = new TableCell();
			labelCell.Attributes["id"] = ClientID + "_" + testTag;
			labelCell.Attributes[EasyTestTagHelper.TestField] = testTag;
			labelCell.Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;
			labelCell.CssClass = "ErrorLabel";
			Literal label = new Literal();
			labelCell.Controls.Add(label);
			row.Cells.Add(labelCell);
			string formattedMessage = System.Web.HttpUtility.HtmlEncode(message.Trim());
			string[] lines = formattedMessage.Split('\n');
			formattedMessage = string.Join("<br>", lines);
			label.Text = formattedMessage;
		}
		public ErrorInfoControl() {
			table = new Table();
			Controls.Add(table);
			ErrorImageName = "Error";
			ignoreError = new ASPxCheckBox();
			Controls.Add(ignoreError);
			ignoreError.ID = "Ch";
			ignoreError.CheckedChanged += new EventHandler(ignoreError_CheckedChanged);
			ignoreError.Visible = false;
		}
		[DefaultValue("Error")]
		public string ErrorImageName {
			get { return errorImageName; }
			set { errorImageName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TableRowCollection Rows {
			get { return table.Rows; }
		}
#if DebugTest
		public void DebugTest_CreateErrorRow(Table table) {
			CreateErrorRow(table);
		}
#endif
	}
}
