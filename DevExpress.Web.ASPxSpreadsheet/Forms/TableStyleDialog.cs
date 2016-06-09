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

using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class ModifyTableStyleDialog : SpreadsheetDialogWithRoundPanel {
		protected ASPxPageControl TableStyles { get; private set; }
		protected ASPxHiddenField HiddenField { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxssDlgModifyTableStyle";
		}
		protected override string GetContentTableID() {
			return "dxModifyTableStyle";
		}
		protected override string GetRoundPanelID() {
			return "rpModifyTableStyle";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			TableStyles = new ASPxPageControl();
			TableStyles.TabPosition = TabPosition.Left;
			TableStyles.TabStyle.HorizontalAlign = HorizontalAlign.Left;
			TableStyles.ID = "pcTableStyles";
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(TableStyles);
			TableStyles.TabPages.Add(new LightTableStyleTabPage(Spreadsheet));
			TableStyles.TabPages.Add(new MediumTableStyleTabPage(Spreadsheet));
			TableStyles.TabPages.Add(new DarkTableStyleTabPage(Spreadsheet));
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			HiddenField = DialogUtils.CreateHiddenField("dxHiddenField");
			HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
			Controls.Add(HiddenField);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RenderUtils.AppendDefaultDXClassName(TableStyles, "dxssDlgPageControl");
		}
	}
}
