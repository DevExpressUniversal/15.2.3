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
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class ColumnsEditorForm : RichEditDialogUserControl {
		protected PanelContent Content { get; private set; }
		protected override void CreateChildControls() {
			base.CreateChildControls();
			ASPxPanel panel = new ASPxPanel();
			Content = new PanelContent();
			panel.PanelCollection.Add(Content);
			CreateEditors(Convert.ToInt32(RichEdit.CurrentColumnsCount));
			Controls.Add(panel);
		}
		protected void CreateEditors(int ColumnsCount) {
			InternalTable mainTable = RenderUtils.CreateTable();
			mainTable.CssClass = "dxreDlgColumnsEditorMainTable";
			Content.Controls.Add(mainTable);
			TableRow headerRow = CreateHeaderRow();
			mainTable.Controls.Add(headerRow);
			for(int index = 0; index < ColumnsCount; index++) {
				TableRow row = RenderUtils.CreateTableRow();
				mainTable.Controls.Add(row);
				row.Controls.Add(CreateTextBoxCell(index));
				row.Controls.Add(CreateSpinEditCell(index, "Width"));
				row.Controls.Add(CreateSpinEditCell(index, "Spacing"));
			}
		}
		TableRow CreateHeaderRow() {
			TableRow headerRow = RenderUtils.CreateTableRow();
			string[] titles = { ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_Col),
							ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_Width),
							ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_Spacing)
		};
			for(int i = 0; i < titles.Length; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				headerRow.Controls.Add(cell);
				ASPxLabel label = new ASPxLabel();
				label.Text = titles[i];
				cell.Controls.Add(label);
			}
			return headerRow;
		}
		TableCell CreateTextBoxCell(int index) {
			TableCell texBoxCell = RenderUtils.CreateTableCell();
			ASPxTextBox textBox = new ASPxTextBox();
			textBox.Text = (index + 1).ToString() + ":";
			textBox.ReadOnly = true;
			textBox.CssClass = "dxreDlgTextBox";
			texBoxCell.Controls.Add(textBox);
			return texBoxCell;
		}
		TableCell CreateSpinEditCell(int index, string name) {
			TableCell spinEditCell = RenderUtils.CreateTableCell();
			ASPxSpinEdit spinEdit = new ASPxSpinEdit();
			spinEdit.SetupDefaultSettings(0, 1000, UnitFormatString);
			spinEdit.ClientInstanceName = "dxreDialog_SpnColumns" + name + index.ToString();
			spinEdit.CssClass = "dxreDlgSpinEdit";
			spinEdit.ID = "SpnColumns" + name + " " + index.ToString();
			spinEditCell.Controls.Add(spinEdit);
			return spinEditCell;
		}
	}
}
