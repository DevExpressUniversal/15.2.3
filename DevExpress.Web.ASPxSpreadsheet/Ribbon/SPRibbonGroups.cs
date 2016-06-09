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

using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Office.Localization;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFileCommonGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FileNew;
			}
		}
		protected override string DefaultName {
			get {
				return "FileCommon";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCommon);
			}
		}
	}
	public class SRViewGroup : SRGroup {
		protected override string DefaultImage {
			get {
				return "FullScreenLarge";
			}
		}
		protected override string DefaultName {
			get {
				return "View";
			}
		}
		protected override string DefaultText {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ViewGroup_Title);
			}
		}
	}
	public class SRUndoGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FileUndo;
			}
		}
		protected override string DefaultName {
			get {
				return "Undo";
			}
		}
		protected override string DefaultText {
			get {
				return OfficeLocalizer.GetString(OfficeStringId.MenuCmd_Undo);
			}
		}
	}
	public class SRClipboardGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PasteSelection;
			}
		}
		protected override string DefaultName {
			get {
				return "Clipboard";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupClipboard);
			}
		}
	}
	public class SRFontGroup : SRGroup {
		protected override string DefaultImage {
			get {
				return "FontGroupLarge";
			}
		}
		protected override string DefaultName {
			get {
				return "Font";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFont);
			}
		}
	}
	public class SRAlignmentGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentCenter;
			}
		}
		protected override string DefaultName {
			get {
				return "Alignment";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupAlignment);
			}
		}
	}
	public class SRNumberGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberPercent;
			}
		}
		protected override string DefaultName {
			get {
				return "Number";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupNumber);
			}
		}
	}
	public class SRCellsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Cells";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCells);
			}
		}
	}
	public class SREditingGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Editing";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupEditing);
			}
		}
	}
	public class SRIllustrationsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertPicture;
			}
		}
		protected override string DefaultName {
			get {
				return "Illustration";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupIllustrations);
			}
		}
	}
	public class SRChartsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Charts";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCharts);
			}
		}
	}
	public class SRLinksGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertHyperlink;
			}
		}
		protected override string DefaultName {
			get {
				return "Link";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupLinks);
			}
		}
	}
	public class SRTablesGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertTable;
			}
		}
		protected override string DefaultName {
			get {
				return "Tables";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTables);
			}
		}
	}
	public class SRPageSetupGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupOrientationCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "PageSetup";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPageSetup);
			}
		}
	}
	public class SRShowGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupPrintAreaCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Show";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupShow);
			}
		}
	}
	public class SRPrintGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FileQuickPrint;
			}
		}
		protected override string DefaultName {
			get {
				return "Print";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupPrint);
			}
		}
	}
	public class SRArrangeGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ArrangeBringForwardCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Arrange";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange);
			}
		}
	}
	public class SRFunctionLibraryGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingAutoSumCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "FunctionLibrary";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFunctionLibrary);
			}
		}
	}
	public class SRCalculationGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormulasCalculationOptionsCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Calculation";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFormulaCalculation);
			}
		}
	}
	public class SRDataSortAndFilterGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.DataSortAscending;
			}
		}
		protected override string DefaultName {
			get {
				return "Sort & Filter";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupSortAndFilter);
			}
		}
	}
	public class SRDataToolsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.DataToolsDataValidation;
			}
		}
		protected override string DefaultName {
			get {
				return "Data Validation";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupDataTools);
			}
		}
	}
	public class SRWindowGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewFreezePanesCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Window";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupWindow);
			}
		}
	}
	public class SRTablePropertiesGroup : SRGroup {
		protected override string DefaultName {
			get {
				return "Properties";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableProperties);
			}
		}
	}
	public class SRTableToolsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.TableToolsConvertToRange;
			}
		}
		protected override string DefaultName {
			get {
				return "Tools";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableTools);
			}
		}
	}
	public class SRTableStyleOptionsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertTable;
			}
		}
		protected override string DefaultName {
			get {
				return "Table Style Properties";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions);
			}
		}
	}
	public class SRTableStyleGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAsTable;
			}
		}
		protected override string DefaultName {
			get {
				return "Table Style";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions);
			}
		}
	}
	public class SRStylesGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAsTable;
			}
		}
		protected override string DefaultName {
			get {
				return "Styles";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupStyles);
			}
		}
	}
	public class SRPictureArrangeGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ArrangeBringForwardCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Arrange";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange);
			}
		}
	}
	public class SRChartTypeGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartChangeType;
			}
		}
		protected override string DefaultName {
			get {
				return "Type";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignType);
			}
		}
	}
	public class SRChartDataGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartSelectData;
			}
		}
		protected override string DefaultName {
			get {
				return "Data";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsDesignData);
			}
		}
	}
	public class SRChartLabelsGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartTitleCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Labels";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutLabels);
			}
		}
	}
	public class SRChartAxesGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ChartAxesCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Axes";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAxes);
			}
		}
	}
	public class SRChartArrangeGroup : SRGroup {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ArrangeBringForwardCommandGroup;
			}
		}
		protected override string DefaultName {
			get {
				return "Arrange";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange);
			}
		}
	}
}
