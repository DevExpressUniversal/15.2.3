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

using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Snap.Core.Commands {
	public static class SnapCommandId {
		const int commandStartId = 10000;
		public static readonly RichEditCommandId SortFieldAscending = new RichEditCommandId(commandStartId);
		public static readonly RichEditCommandId SortFieldDescending = new RichEditCommandId(commandStartId + 1);
		public static readonly RichEditCommandId InsertGroupHeader = new RichEditCommandId(commandStartId + 2);
		public static readonly RichEditCommandId InsertGroupFooter = new RichEditCommandId(commandStartId + 3);
		public static readonly RichEditCommandId RemoveGroupHeader = new RichEditCommandId(commandStartId + 4);
		public static readonly RichEditCommandId RemoveGroupFooter = new RichEditCommandId(commandStartId + 5);
		public static readonly RichEditCommandId GroupHeader = new RichEditCommandId(commandStartId + 6);
		public static readonly RichEditCommandId GroupFooter = new RichEditCommandId(commandStartId + 7);
		public static readonly RichEditCommandId GroupByField = new RichEditCommandId(commandStartId + 8);
		public static readonly RichEditCommandId InsertListHeader = new RichEditCommandId(commandStartId + 9);
		public static readonly RichEditCommandId InsertListFooter = new RichEditCommandId(commandStartId + 10);
		public static readonly RichEditCommandId RemoveListHeader = new RichEditCommandId(commandStartId + 11);
		public static readonly RichEditCommandId RemoveListFooter = new RichEditCommandId(commandStartId + 12);
		public static readonly RichEditCommandId ListHeader = new RichEditCommandId(commandStartId + 13);
		public static readonly RichEditCommandId ListFooter = new RichEditCommandId(commandStartId + 14);
		public static readonly RichEditCommandId FilterList = new RichEditCommandId(commandStartId + 15);
		public static readonly RichEditCommandId FilterField = new RichEditCommandId(commandStartId + 16);
		public static readonly RichEditCommandId Properties = new RichEditCommandId(commandStartId + 17);
		public static readonly RichEditCommandId InsertBarCode = new RichEditCommandId(commandStartId + 18);
		public static readonly RichEditCommandId InsertCheckBox = new RichEditCommandId(commandStartId + 19);
		public static readonly RichEditCommandId InsertChart = new RichEditCommandId(commandStartId + 20);
		public static readonly RichEditCommandId ConvertToParagraphs = new RichEditCommandId(commandStartId + 21);
		public static readonly RichEditCommandId PasteSnxCommand = new RichEditCommandId(commandStartId + 22);
		public static readonly RichEditCommandId SelectWholeListGroup = new RichEditCommandId(commandStartId + 23);
		public static readonly RichEditCommandId InsertGroupSeparator = new RichEditCommandId(commandStartId + 24);
		public static readonly RichEditCommandId InsertPageBreakGroupSeparator = new RichEditCommandId(commandStartId + 25);
		public static readonly RichEditCommandId InsertNoneGroupSeparator = new RichEditCommandId(commandStartId + 26);
		public static readonly RichEditCommandId InsertEmptyParagraphGroupSeparator = new RichEditCommandId(commandStartId + 27);
		public static readonly RichEditCommandId InsertEmptyRowGroupSeparator = new RichEditCommandId(commandStartId + 28);
		public static readonly RichEditCommandId InsertSectionBreakNextPageGroupSeparator = new RichEditCommandId(commandStartId + 29);
		public static readonly RichEditCommandId InsertSectionBreakEvenPageGroupSeparator = new RichEditCommandId(commandStartId + 30);
		public static readonly RichEditCommandId InsertSectionBreakOddPageGroupSeparator = new RichEditCommandId(commandStartId + 31);
		public static readonly RichEditCommandId InsertDataRowSeparator = new RichEditCommandId(commandStartId + 32);
		public static readonly RichEditCommandId InsertPageBreakDataRowSeparator = new RichEditCommandId(commandStartId + 33);
		public static readonly RichEditCommandId InsertNoneDataRowSeparator = new RichEditCommandId(commandStartId + 34);
		public static readonly RichEditCommandId InsertEmptyParagraphDataRowSeparator = new RichEditCommandId(commandStartId + 35);
		public static readonly RichEditCommandId InsertEmptyRowDataRowSeparator = new RichEditCommandId(commandStartId + 36);
		public static readonly RichEditCommandId InsertSectionBreakNextPageDataRowSeparator = new RichEditCommandId(commandStartId + 37);
		public static readonly RichEditCommandId InsertSectionBreakEvenPageDataRowSeparator = new RichEditCommandId(commandStartId + 38);
		public static readonly RichEditCommandId InsertSectionBreakOddPageDataRowSeparator = new RichEditCommandId(commandStartId + 39);
		public static readonly RichEditCommandId ChangeTheme = new RichEditCommandId(commandStartId + 40);
		public static readonly RichEditCommandId ToggleFieldHighlighting = new RichEditCommandId(commandStartId + 41);
		public static readonly RichEditCommandId CreateFieldForTemplate = new RichEditCommandId(commandStartId + 42);
		public static readonly RichEditCommandId SummaryByField = new RichEditCommandId(commandStartId + 43);
		public static readonly RichEditCommandId SummarySum = new RichEditCommandId(commandStartId + 44);
		public static readonly RichEditCommandId SummaryCount = new RichEditCommandId(commandStartId + 45);
		public static readonly RichEditCommandId SummaryAverage = new RichEditCommandId(commandStartId + 46);
		public static readonly RichEditCommandId SummaryMin = new RichEditCommandId(commandStartId + 47);
		public static readonly RichEditCommandId SummaryMax = new RichEditCommandId(commandStartId + 48);
		public static readonly RichEditCommandId ChangeEditorRowLimit = new RichEditCommandId(commandStartId + 49);
		public static readonly RichEditCommandId FilterFieldPlaceHolder = new RichEditCommandId(commandStartId + 50);
		public static readonly RichEditCommandId HighlightActiveElement = new RichEditCommandId(commandStartId + 51);
		public static readonly RichEditCommandId GroupFieldsCollection = new RichEditCommandId(commandStartId + 52);
		public static readonly RichEditCommandId DeleteList = new RichEditCommandId(commandStartId + 53);
		public static readonly RichEditCommandId ExportDocument = new RichEditCommandId(commandStartId + 54);
		public static readonly RichEditCommandId ShowReportStructureEditorForm = new RichEditCommandId(commandStartId + 55);
		public static readonly RichEditCommandId NewDataSource = new RichEditCommandId(commandStartId + 57);
		public static readonly RichEditCommandId ChangeTableCellStyle = new RichEditCommandId(commandStartId + 58);
		public static readonly RichEditCommandId SaveTheme = new RichEditCommandId(commandStartId + 59);
		public static readonly RichEditCommandId LoadTheme = new RichEditCommandId(commandStartId + 60);
		public static readonly RichEditCommandId MailMergeDataSource = new RichEditCommandId(commandStartId + 61);
		public static readonly RichEditCommandId MailMergeFilters = new RichEditCommandId(commandStartId + 62);
		public static readonly RichEditCommandId MailMergeSorting = new RichEditCommandId(commandStartId + 63);
		public static readonly RichEditCommandId InsertSparkline = new RichEditCommandId(commandStartId + 64);
		public static readonly RichEditCommandId MailMergeCurrentRecord = new RichEditCommandId(commandStartId + 65);
		public static readonly RichEditCommandId InsertIndex = new RichEditCommandId(commandStartId + 66);
		public static readonly RichEditCommandId SnapMailMergePrintPreview = new RichEditCommandId(commandStartId + 67);
		public static readonly RichEditCommandId SnapMailMergePrint = new RichEditCommandId(commandStartId + 68);
		public static readonly RichEditCommandId SnapMailMergeQuickPrint = new RichEditCommandId(commandStartId + 69);
		public static readonly RichEditCommandId SnapMailMergeExportDocument = new RichEditCommandId(commandStartId + 70);
		public static readonly RichEditCommandId FinishAndMerge = new RichEditCommandId(commandStartId + 71);
		public static readonly RichEditCommandId MailMergeFilterField = new RichEditCommandId(commandStartId + 72);
		public static readonly RichEditCommandId SnapFilterField = new RichEditCommandId(commandStartId + 73);
		public static readonly RichEditCommandId MailMergeSortFieldAscending = new RichEditCommandId(commandStartId + 74);
		public static readonly RichEditCommandId MailMergeSortFieldDescending = new RichEditCommandId(commandStartId + 75);
		public static readonly RichEditCommandId SnapSortFieldAscending = new RichEditCommandId(commandStartId + 76);
		public static readonly RichEditCommandId SnapSortFieldDescending = new RichEditCommandId(commandStartId + 77);
	}
}
