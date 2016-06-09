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
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	internal class SpreadsheetCommandHelper {
		internal const string CommandIDParamName = "CommandID";
		internal const string FullScreenModeParamName = "FullScreenMode";
		internal const string ReselectAfterCommandParamName = "ReselectAfterCommand";
		private NameValueCollection CommandContext { get; set; }
		private string CommandId { get; set; }
		public SpreadsheetCommandHelper(NameValueCollection commandContext) {
			CommandContext = commandContext;
			CommandId = ParseCommandId(commandContext);
		}
		private string ParseCommandId(NameValueCollection commandContext) {
			if(commandContext != null && commandContext.Count > 0)
				return commandContext[CommandIDParamName];
			return string.Empty;
		}
		public bool HistoryCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.HistoryCommands);
		}
		public bool ActiveSheetChangeCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.ActiveSheetChangeCommands);
		}
		public bool SheetCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.SheetCommands);
		}
		public bool RefreshRibbonCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.RefreshRibbonCommands);
		}
		public bool CellSizeCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.CellSizeCommands);
		}
		public bool CalculationSheetCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.CalculationSheetCommands);
		}
		public bool PageMarginsCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.PageMarginsCommands);
		}
		public bool PaperOrientationCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.PaperOrientationCommands);
		}
		public bool PaperKindCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.PaperKindCommands);
		}
		public bool PagerCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.PagerCommands);
		}
		public bool ScrollCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.ScrollCommands);
		}
		public bool FullScreenCommandExecuted() {
			if(CommandContext != null && CommandContext.Count > 0 && !string.IsNullOrEmpty(CommandContext[FullScreenModeParamName]))
				return bool.Parse(CommandContext[FullScreenModeParamName]);
			return false;
		}
		public bool ReselectAfterCommand() {
			if(CommandContext != null && CommandContext.Count > 0 && !string.IsNullOrEmpty(CommandContext[ReselectAfterCommandParamName]))
				return bool.Parse(CommandContext[ReselectAfterCommandParamName]);
			return false;
		}
		public bool PrintCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.PrintCommands);
		}
		public bool ResizeCommandExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.ResizeCommands);
		}
		public bool UpdateCellCommandsExecuted(){
			return CommandCompaper(SpreadsheetCommandIdCollections.UpdateCellCommands);
		}
		public bool FreezeCommandsExecuted() {
			return CommandCompaper(SpreadsheetCommandIdCollections.FreezeCommands);
		}
		public bool IsTabControlUpdateRequired() {
			return SheetCommandExecuted() || PagerCommandExecuted();
		}
		public bool IsRibbonControlUpdateRequired() {
			return RefreshRibbonCommandExecuted() || HistoryCommandExecuted() || ActiveSheetChangeCommandExecuted();
		}
		public bool IsDefaultCellSizeUpdateRequired() {
			return SheetCommandExecuted() || CellSizeCommandExecuted() || HistoryCommandExecuted();
		}
		public bool IsCalculationModeUpdateRequired() {
			return SheetCommandExecuted() || HistoryCommandExecuted() || CalculationSheetCommandExecuted();
		}
		public bool IsSheetLoading() {
			return SheetCommandExecuted() || ActiveSheetChangeCommandExecuted() || FreezeCommandsExecuted();
		}
		public bool IsPageMarginsUpdateRequired() {
			return IsSheetLoading() || PageMarginsCommandExecuted() || HistoryCommandExecuted();
		}
		public bool IsPaperOrientationUpdateRequired() {
			return IsSheetLoading() || PaperOrientationCommandExecuted() || HistoryCommandExecuted();
		}
		public bool IsPaperKindUpdateRequired() {
			return IsSheetLoading() || PaperKindCommandExecuted() || HistoryCommandExecuted();
		}
		public bool IsCommandCanChangeScrollPosition() {
			return IsSheetLoading() || ScrollCommandExecuted() || FreezeCommandsExecuted();
		}
		public bool IsCommandCanChangeScrollPositionOrVisibleWindowSize() {
			return IsCommandCanChangeScrollPosition() || FullScreenCommandExecuted();
		}
		public bool IsPrintOptionsUpdateRequired() {
			return IsSheetLoading() || PrintCommandExecuted() || HistoryCommandExecuted();
		}
		public bool IsGridCacheUpdateRequired() {
			return ResizeCommandExecuted() || UpdateCellCommandsExecuted();
		}
		private bool CommandCompaper(List<string> commandCollection) {
			return commandCollection.Contains(CommandId);
		}
	}
	static class SpreadsheetCommandIdCollections {
		static object locker = new object();
		static List<string> historyCommands;
		static List<string> activeSheetChangeCommands;
		static List<string> sheetCommands;
		static List<string> refreshRibbonCommands;
		static List<string> cellSizeCommands;
		static List<string> calculateSheetCommands;
		static List<string> pageMarginsCommands;
		static List<string> paperOrientationCommands;
		static List<string> paperKindCommands;
		static List<string> pagerCommands;
		static List<string> scrollCommands;
		static List<string> printCommands;
		static List<string> resizeCommands;
		static List<string> updateCellCommands;
		static List<string> freezeCommands;
		public static List<string> HistoryCommands {
			get {
				lock(locker) {
					if(historyCommands == null)
						historyCommands = CreateHistoryCommands();
					return historyCommands;
				}
			}
		}
		public static List<string> ActiveSheetChangeCommands {
			get {
				lock(locker) {
					if(activeSheetChangeCommands == null)
						activeSheetChangeCommands = CreateActiveSheetChangeCommands();
					return activeSheetChangeCommands;
				}
			}
		}
		public static List<string> SheetCommands {
			get {
				lock(locker) {
					if(sheetCommands == null)
						sheetCommands = CreateSheetCommands();
					return sheetCommands;
				}
			}
		}
		public static List<string> RefreshRibbonCommands {
			get {
				lock(locker) {
					if(refreshRibbonCommands == null)
						refreshRibbonCommands = CreateRefreshRibbonCommands();
					return refreshRibbonCommands;
				}
			}
		}
		public static List<string> CellSizeCommands {
			get {
				lock(locker) {
					if(cellSizeCommands == null)
						cellSizeCommands = CreateCellSizeCommands();
					return cellSizeCommands;
				}
			}
		}
		public static List<string> CalculationSheetCommands {
			get {
				lock(locker) {
					if(calculateSheetCommands == null)
						calculateSheetCommands = CreateCalculationSheetCommands();
					return calculateSheetCommands;
				}
			}
		}
		public static List<string> PageMarginsCommands {
			get {
				lock(locker) {
					if(pageMarginsCommands == null)
						pageMarginsCommands = CreatePageMarginsCommands();
					return pageMarginsCommands;
				}
			}
		}
		public static List<string> PaperOrientationCommands {
			get {
				lock(locker) {
					if(paperOrientationCommands == null)
						paperOrientationCommands = CreatePaperOrientationCommands();
					return paperOrientationCommands;
				}
			}
		}
		public static List<string> PaperKindCommands {
			get {
				lock(locker) {
					if(paperKindCommands == null)
						paperKindCommands = CreatePaperKindCommands();
					return paperKindCommands;
				}
			}
		}
		public static List<string> PagerCommands {
			get {
				lock(locker) {
					if(pagerCommands == null)
						pagerCommands = CreatePagerCommands();
					return pagerCommands;
				}
			}
		}
		public static List<string> ScrollCommands {
			get {
				lock(locker) {
					if(scrollCommands == null)
						scrollCommands = CreateScrollCommands();
					return scrollCommands;
				}
			}
		}
		public static List<string> PrintCommands {
			get {
				lock(locker) {
					if(printCommands == null)
						printCommands = CreatePrintCommands();
					return printCommands;
				}
			}
		}
		public static List<string> ResizeCommands {
			get {
				lock(locker) {
					if(resizeCommands == null)
						resizeCommands = CreateResizeCommands();
					return resizeCommands;
				}
			}
		}
		public static List<string> UpdateCellCommands {
			get {
				lock(locker) {
					if(updateCellCommands == null)
						updateCellCommands = CreateUpdateCellCommands();
					return updateCellCommands;
				}
			}
		}
		public static List<string> FreezeCommands {
			get {
				lock(locker) {
					if(freezeCommands == null)
						freezeCommands = CreateFreezeCommands();
					return freezeCommands;
				}
			}
		}
		private static List<string> CreateHistoryCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.FileUndo.ToString());
			commands.Add(SpreadsheetCommandId.FileRedo.ToString());
			return commands;
		}
		private static List<string> CreateActiveSheetChangeCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.UnhideSheet));
			commands.Add(SpreadsheetCommandId.HideSheet.ToString());
			commands.Add(SpreadsheetCommandId.InsertSheet.ToString());
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.RemoveSheet));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.MoveOrCopySheetWebCommand));
			return commands;
		}
		private static List<string> CreateSheetCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.LoadSheet));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FileNew));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FileOpen));
			return commands;
		}
		private static List<string> CreateRefreshRibbonCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.FormatAlignmentTop.ToString());
			commands.Add(SpreadsheetCommandId.FormatAlignmentBottom.ToString());
			commands.Add(SpreadsheetCommandId.FormatAlignmentCenter.ToString());
			commands.Add(SpreadsheetCommandId.FormatAlignmentMiddle.ToString());
			commands.Add(SpreadsheetCommandId.FormatAlignmentRight.ToString());
			commands.Add(SpreadsheetCommandId.FormatAlignmentLeft.ToString());
			commands.Add(SpreadsheetCommandId.FormatIncreaseFontSize.ToString());
			commands.Add(SpreadsheetCommandId.FormatDecreaseFontSize.ToString());
			commands.Add(SpreadsheetCommandId.FormatFontStrikeout.ToString());
			commands.Add(SpreadsheetCommandId.FormatFontUnderline.ToString());
			commands.Add(SpreadsheetCommandId.FormatFontBold.ToString());
			commands.Add(SpreadsheetCommandId.FormatFontItalic.ToString());
			return commands;
		}
		private static List<string> CreateCellSizeCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FormatDefaultColumnWidth));
			return commands;
		}
		private static List<string> CreateCalculationSheetCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.FormulasCalculationModeAutomatic.ToString());
			commands.Add(SpreadsheetCommandId.FormulasCalculationModeManual.ToString());
			return commands;
		}
		private static List<string> CreatePageMarginsCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.PageSetupMarginsNormal.ToString());
			commands.Add(SpreadsheetCommandId.PageSetupMarginsNarrow.ToString());
			commands.Add(SpreadsheetCommandId.PageSetupMarginsWide.ToString());
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.ApplyPageSetupSettings)); 
			return commands;
		}
		private static List<string> CreatePaperOrientationCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.PageSetupOrientationPortrait.ToString());
			commands.Add(SpreadsheetCommandId.PageSetupOrientationLandscape.ToString());
			return commands;
		}
		private static List<string> CreatePaperKindCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.SetPaperKind));
			return commands;
		}
		private static List<string> CreatePagerCommands() {
			List<string> commands = new List<string>();
			commands.AddRange(ActiveSheetChangeCommands);
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.RenameSheet));
			return commands;
		}
		private static List<string> CreateScrollCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.ScrollTo));
			return commands;
		}
		private static List<string> CreatePrintCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.PageSetupPrintGridlines.ToString());
			commands.Add(SpreadsheetCommandId.PageSetupSetPaperKind.ToString());
			return commands;
		}
		private static List<string> CreateResizeCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.ResizeColumn));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.ResizeRow));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FormatRowHeight));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FormatColumnWidth));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FormatDefaultColumnWidth));
			commands.Add(SpreadsheetCommandId.FormatRowHeight.ToString());
			commands.Add(SpreadsheetCommandId.FormatColumnWidth.ToString());
			commands.Add(SpreadsheetCommandId.FormatDefaultColumnWidth.ToString());
			commands.Add(SpreadsheetCommandId.FormatRowHeightContextMenuItem.ToString());
			commands.Add(SpreadsheetCommandId.FormatColumnWidthContextMenuItem.ToString());			
			return commands;
		}
		private static List<string> CreateUpdateCellCommands() {
			List<string> commands = new List<string>();
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.UpdateCell));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.MoveRangeWebCommand));
			commands.Add(SpreadsheetCommandId.PasteSelection.ToString());
			commands.Add(SpreadsheetCommandId.FormatClearAll.ToString());
			commands.Add(SpreadsheetCommandId.FormatClearContents.ToString());
			commands.Add(SpreadsheetCommandId.FormatClearFormats.ToString());
			commands.Add(SpreadsheetCommandId.FormatClearContentsContextMenuItem.ToString());
			return commands;
		}
		private static List<string> CreateFreezeCommands() {
			List<string> commands = new List<string>();
			commands.Add(SpreadsheetCommandId.ViewUnfreezePanes.ToString());
			commands.Add(SpreadsheetCommandId.ViewFreezeFirstColumn.ToString());
			commands.Add(SpreadsheetCommandId.ViewFreezePanes.ToString());
			commands.Add(SpreadsheetCommandId.ViewFreezeTopRow.ToString());
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FreezePanesWebCommand));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FreezeColumnWebCommand));
			commands.Add(WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.FreezeRowWebCommand));
			return commands;
		}
	}
}
