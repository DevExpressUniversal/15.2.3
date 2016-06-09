﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class ChartChangeVerticalAxisTitleDialog : SpreadsheetDialogWithRoundPanel {
		protected const string ChartVerticalAxisTitleTextBoxId = "txbChartVerticalAxisTitle";
		protected ASPxTextBox ChartVerticalAxisTitle { get; private set; }
		protected ASPxLabel Caption { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.ChartChangeVerticalAxisTitleDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxChartChangeVerticalAxisTitleForm";
		}
		protected override string GetRoundPanelID() {
			return "rpChartChangeVerticalAxisTitle";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = DialogUtils.CreateCaptionCell();
			tableRow.Cells.Add(tableCell);
			Caption = new ASPxLabel() { ID = "lblChartChangeTitle", AssociatedControlID = ChartVerticalAxisTitleTextBoxId };
			tableCell.Controls.Add(Caption);
			tableCell = DialogUtils.CreateInputCell();
			tableRow.Cells.Add(tableCell);
			ChartVerticalAxisTitle = DialogUtils.CreateTextBoxWithErrorFrame(ChartVerticalAxisTitleTextBoxId,  GetControlClientInstanceName("_dxChartVerticalAxisTitle"), "_dxChartChangeVerticalAxisTitleGroup");
			tableCell.Controls.Add(ChartVerticalAxisTitle);
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			Caption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChartChangeVerticalAxisTitle_Caption) + DialogUtils.LabelEndMark;
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { ChartVerticalAxisTitle };
		}
	}
}
