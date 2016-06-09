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
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region MailMergeRangeMouseStateBase
	public abstract class MailMergeRangeMouseStateBase : DragRangeManuallyMouseHandlerState {
		protected enum ProcessingMailMergeRange {
			Header,
			Footer,
			Detail,
			DetailLevel,
			GroupHeader,
			GroupFooter
		}
		#region Fields
		readonly CellRange originalRange;
		readonly string mailMergeDefinedName;
		readonly ProcessingMailMergeRange processing;
		readonly MailMergeOptions options;
		#endregion
		#region Properties
		protected override CellRange OriginalRange { get { return originalRange; } }
		protected ProcessingMailMergeRange Processing { get { return processing; } }
		protected MailMergeOptions Options { get { return options; } }
		#endregion
		protected MailMergeRangeMouseStateBase(SpreadsheetMouseHandler mouseHandler,
											   SpreadsheetHitTestResult hitTestResult, string mailMergeDefinedName)
			: base(mouseHandler, hitTestResult) {
			this.mailMergeDefinedName = mailMergeDefinedName;
			options = new MailMergeOptions(DocumentModel);
			if (mailMergeDefinedName == MailMergeDefinedNames.HeaderRange && Options.HeaderRange != null) {
				originalRange = (CellRange)Options.HeaderRange;
				processing = ProcessingMailMergeRange.Header;
			}
			else if (mailMergeDefinedName == MailMergeDefinedNames.FooterRange && Options.FooterRange != null) {
				originalRange = (CellRange)Options.FooterRange;
				processing = ProcessingMailMergeRange.Footer;
			}
			else if (mailMergeDefinedName == MailMergeDefinedNames.DetailRange && Options.DetailRange != null) {
				originalRange = (CellRange)Options.DetailRange;
				processing = ProcessingMailMergeRange.Detail;
			}
			else if (mailMergeDefinedName.IndexOf(MailMergeDefinedNames.DetailLevel) == 0) {
				string stringIndex = mailMergeDefinedName.Substring(MailMergeDefinedNames.DetailLevel.Length);
				int index;
				if (int.TryParse(stringIndex, out index)) {
					if (index < Options.DetailLevels.Count) {
						originalRange = (CellRange)Options.DetailLevels[index];
						processing = ProcessingMailMergeRange.DetailLevel;
					}
				}
			}
			else if (mailMergeDefinedName.IndexOf(MailMergeDefinedNames.GroupHeader) == 0) {
				if (Options.HasGroup) {
					originalRange = Options.GetRangeByDefinedName(mailMergeDefinedName) as CellRange;
					processing = ProcessingMailMergeRange.GroupHeader;
				}
			}
			else if (mailMergeDefinedName.IndexOf(MailMergeDefinedNames.GroupFooter) == 0) {
				if (Options.HasGroup) {
					originalRange = Options.GetRangeByDefinedName(mailMergeDefinedName) as CellRange;
					processing = ProcessingMailMergeRange.GroupFooter;
				}
			}
		}
		protected internal override bool CommitDrag(CellPosition commitPosition,
													IDataObject dataObject) {
			DocumentModel documentModel = Control.InnerControl.DocumentModel;
			CellRange targetRange = GetNewRange(commitPosition);
			BeforeComitDrag(targetRange);
			ChangeMailMergeRange(documentModel, targetRange);
			LastFeedbackBounds = Rectangle.Empty;
			return true;
		}
		protected internal override Rectangle CalculateCellBounds(SpreadsheetHitTestResult hitTestResult) {
			PageGrid gridRows = hitTestResult.Page.GridRows;
			PageGrid gridColumns = hitTestResult.Page.GridColumns;
			CellRange targetRange = GetNewRange(hitTestResult.CellPosition);
			int left = gridColumns.LookupNearItem(targetRange.TopLeft.Column);
			int top = gridRows.LookupNearItem(targetRange.TopLeft.Row);
			int right = gridColumns.LookupFarItem(targetRange.BottomRight.Column);
			int bottom = gridRows.LookupFarItem(targetRange.BottomRight.Row);
			int leftCoordinate = left < 0 || left >= gridColumns.Count
									 ? hitTestResult.Page.Bounds.Left
									 : gridColumns[left].Near;
			int topCoordinate = top < 0 || top >= gridRows.Count ? hitTestResult.Page.Bounds.Top : gridRows[top].Near;
			int rightCoordinate = right < 0 || right >= gridColumns.Count
									  ? hitTestResult.Page.Bounds.Right
									  : gridColumns[right].Far;
			int bottomCoordinate = bottom < 0 || bottom >= gridRows.Count
									   ? hitTestResult.Page.Bounds.Bottom
									   : gridRows[bottom].Far;
			return Normalize(Rectangle.FromLTRB(leftCoordinate, topCoordinate, rightCoordinate, bottomCoordinate));
		}
		protected CellRange FixIntersectsWithMergedCells(CellRange range) {
			CellRange result = range;
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			bool haveIntersections;
			do {
				haveIntersections = false;
				List<CellRange> list = sheet.MergedCells.GetMergedCellRangesIntersectsRange(result);
				if(list.Count > 0) {
					int targetRangeFirstCol = result.TopLeft.Column;
					int targetRangeFirstRow = result.TopLeft.Row;
					int targetRangeLastCol = result.BottomRight.Column;
					int targetRangeLastRow = result.BottomRight.Row;
					foreach(CellRange cellRange in list) {
						if(targetRangeFirstCol > cellRange.TopLeft.Column) {
							targetRangeFirstCol = cellRange.TopLeft.Column;
							haveIntersections = true;
						}
						if(targetRangeFirstRow > cellRange.TopLeft.Row) {
							targetRangeFirstRow = cellRange.TopLeft.Row;
							haveIntersections = true;
						}
						if(targetRangeLastCol < cellRange.BottomRight.Column) {
							targetRangeLastCol = cellRange.BottomRight.Column;
							haveIntersections = true;
						}
						if(targetRangeLastRow < cellRange.BottomRight.Row) {
							targetRangeLastRow = cellRange.BottomRight.Row;
							haveIntersections = true;
						}
					}
					CellPosition topLeft = new CellPosition(targetRangeFirstCol, targetRangeFirstRow);
					CellPosition bottomRight = new CellPosition(targetRangeLastCol, targetRangeLastRow);
					result = new CellRange(sheet, topLeft, bottomRight);
				}
			} while(haveIntersections);
			return result;
		}
		protected void ChangeMailMergeRangeCore(DocumentModel documentModel, CellRange newRange, string definedName) {
			documentModel.BeginUpdate();
			string newRangeString = GetAbsolutePositionRange(newRange).ToString();
			DefinedNameCollection definedNames = documentModel.ActiveSheet.DefinedNames;
			if(definedNames.Contains(definedName))
				definedNames[definedName].SetReference(newRangeString);
			else
				definedNames.Add(new DefinedName(documentModel, definedName, newRangeString,
												 documentModel.ActiveSheet.SheetId));
			documentModel.ApplyChanges(DocumentModelChangeActions.Redraw);
			documentModel.ApplyChanges(DocumentModelChangeActions.RaiseUpdateUI);
			documentModel.EndUpdate();
		}
		protected void ChangeMailMergeRange(DocumentModel documentModel, CellRange newRange) {
			ChangeMailMergeRangeCore(documentModel, newRange, mailMergeDefinedName);
		}
		CellRange GetAbsolutePositionRange(CellRange range) {
			CellPosition topLeft = new CellPosition(range.TopLeft.Column, range.TopLeft.Row, PositionType.Absolute,
													PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(range.BottomRight.Column, range.BottomRight.Row,
														PositionType.Absolute, PositionType.Absolute);
			return new CellRange(range.Worksheet, topLeft, bottomRight);
		}
		protected virtual CellRange ProcessHeader(CellPosition cellPosition) {
			return CalculateTargetRange(cellPosition);
		}
		protected virtual CellRange ProcessFooter(CellPosition cellPosition) {
			return CalculateTargetRange(cellPosition);
		}
		protected virtual CellRange ProcessDetailRange(CellPosition cellPosition) {
			CellRange range = CalculateTargetRange(cellPosition);
			int x1 = range.TopLeft.Column;
			int x2 = range.BottomRight.Column;
			int y1 = range.TopLeft.Row;
			int y2 = range.BottomRight.Row;
			foreach(CellRangeBase cellRangeBase in Options.DetailLevels) {
				x1 = Math.Min(x1, cellRangeBase.TopLeft.Column);
				x2 = Math.Max(x2, cellRangeBase.BottomRight.Column);
				y1 = Math.Min(y1, cellRangeBase.TopLeft.Row);
				y2 = Math.Max(y2, cellRangeBase.BottomRight.Row);
			}
			return new CellRange(range.Worksheet, new CellPosition(x1, y1), new CellPosition(x2, y2));
		}
		protected virtual CellRange ProcessDetailLevel(CellPosition cellPosition) {
			return CalculateTargetRange(cellPosition);
		}
		protected bool CheckInside(CellRange range, CellRange parentRange) {
			if(parentRange == null)
				return true;
			int rx1 = range.TopLeft.Column;
			int rx2 = range.BottomRight.Column;
			int ry1 = range.TopLeft.Row;
			int ry2 = range.BottomRight.Row;
			int px1 = parentRange.TopLeft.Column;
			int px2 = parentRange.BottomRight.Column;
			int py1 = parentRange.TopLeft.Row;
			int py2 = parentRange.BottomRight.Row;
			return (px1 <= rx1 && rx1 <= px2) && (px1 <= rx2 && rx2 <= px2) && (py1 <= ry1 && ry1 <= py2) &&
				   (py1 <= ry2 && ry2 <= py2);
		}
		CellRange GetNewRange(CellPosition cellPosition) {
			switch(Processing) {
				case ProcessingMailMergeRange.Header:
					return ProcessHeader(cellPosition);
				case ProcessingMailMergeRange.Footer:
					return ProcessFooter(cellPosition);
				case ProcessingMailMergeRange.Detail:
					return ProcessDetailRange(cellPosition);
				case ProcessingMailMergeRange.DetailLevel:
					return ProcessDetailLevel(cellPosition);
				case ProcessingMailMergeRange.GroupHeader:
					return ProcessDetailLevel(cellPosition);
				case ProcessingMailMergeRange.GroupFooter:
					return ProcessDetailLevel(cellPosition);
			}
			return CalculateTargetRange(cellPosition);
		}
		protected abstract CellRange CalculateTargetRange(CellPosition cellPosition);
		protected virtual void BeforeComitDrag(CellRange targetRange) {
		}
	}
	#endregion
	#region MailMergeMoveRangeMouseState
	public class MailMergeMoveRangeMouseState : MailMergeRangeMouseStateBase {
		#region Fields
		CellPosition hitCellPosition;
		#endregion
		public MailMergeMoveRangeMouseState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult,
											string mailMergeDefinedName)
			: base(mouseHandler, hitTestResult, mailMergeDefinedName) {
			hitCellPosition = hitTestResult.CellPosition;
		}
		protected override CellRange ProcessDetailRange(CellPosition commitPosition) {
			return CalculateTargetRange(commitPosition);
		}
		protected override CellRange ProcessDetailLevel(CellPosition commitPosition) {
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			CellRangeBase parent = Options.DetailRange;
			if(parent == null)
				return CalculateTargetRange(commitPosition);
			int dx = hitCellPosition.Column - OriginalRange.TopLeft.Column;
			int dy = hitCellPosition.Row - OriginalRange.TopLeft.Row;
			int targetRangeFirstCol = Math.Max(parent.TopLeft.Column, commitPosition.Column - dx);
			int targetRangeFirstRow = Math.Max(parent.TopLeft.Row, commitPosition.Row - dy);
			int targetRangeLastCol = Math.Min(parent.BottomRight.Column, targetRangeFirstCol + OriginalRange.Width - 1);
			int targetRangeLastRow = Math.Min(parent.BottomRight.Row, targetRangeFirstRow + OriginalRange.Height - 1);
			targetRangeFirstCol = Math.Max(parent.TopLeft.Column, targetRangeLastCol - (OriginalRange.Width - 1));
			targetRangeFirstRow = Math.Max(parent.TopLeft.Row, targetRangeLastRow - (OriginalRange.Height - 1));
			CellRange targetRange = new CellRange(sheet, new CellPosition(targetRangeFirstCol, targetRangeFirstRow),
												  new CellPosition(targetRangeLastCol, targetRangeLastRow));
			targetRange = FixIntersectsWithMergedCells(targetRange);
			return targetRange;
		}
		protected override CellRange CalculateTargetRange(CellPosition commitPosition) {
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			int dx = hitCellPosition.Column - OriginalRange.TopLeft.Column;
			int dy = hitCellPosition.Row - OriginalRange.TopLeft.Row;
			int targetRangeFirstCol = Math.Max(0, commitPosition.Column - dx);
			int targetRangeFirstRow = Math.Max(0, commitPosition.Row - dy);
			int targetRangeLastCol = Math.Min(sheet.MaxColumnCount, targetRangeFirstCol + OriginalRange.Width - 1);
			int targetRangeLastRow = Math.Min(sheet.MaxRowCount, targetRangeFirstRow + OriginalRange.Height - 1);
			CellRange targetRange = new CellRange(sheet, new CellPosition(targetRangeFirstCol, targetRangeFirstRow),
												  new CellPosition(targetRangeLastCol, targetRangeLastRow));
			targetRange = FixIntersectsWithMergedCells(targetRange);
			return targetRange;
		}
		protected override void BeforeComitDrag(CellRange targetRange) {
			DocumentModel documentModel = this.Control.InnerControl.DocumentModel;
			if(Processing == ProcessingMailMergeRange.Detail) {
				int dx = targetRange.TopLeft.Column - OriginalRange.TopLeft.Column;
				int dy = targetRange.TopLeft.Row - OriginalRange.TopLeft.Row;
				for(int i = 0; i < Options.DetailLevels.Count; i++) {
					CellRange detailLevel = (CellRange)Options.DetailLevels[i];
					CellPosition newTopLeft = new CellPosition(detailLevel.TopLeft.Column + dx,
															   detailLevel.TopLeft.Row + dy);
					CellPosition newBottomRight = new CellPosition(detailLevel.BottomRight.Column + dx,
																   detailLevel.BottomRight.Row + dy);
					CellRange newRange = new CellRange(documentModel.ActiveSheet, newTopLeft, newBottomRight);
					ChangeMailMergeRangeCore(documentModel, newRange, MailMergeDefinedNames.DetailLevel + i);
				}
			}
		}
	}
	#endregion
	#region MailMergeResizeRangeMouseState
	public class MailMergeResizeRangeMouseState : MailMergeRangeMouseStateBase {
		#region Fields
		readonly bool top;
		readonly bool left;
		#endregion
		public MailMergeResizeRangeMouseState(SpreadsheetMouseHandler mouseHandler,
											  SpreadsheetHitTestResult hitTestResult, bool top, bool left,
											  string mailMergeDefinedName)
			: base(mouseHandler, hitTestResult, mailMergeDefinedName) {
			CellOffset = Point.Empty;
			OriginalRangeSize = new Size(1, 1);
			this.top = top;
			this.left = left;
		}
		protected override CellRange ProcessDetailLevel(CellPosition commitPosition) {
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			CellRangeBase parent = Options.DetailRange;
			CellRange range = CalculateTargetRange(commitPosition);
			if(parent == null)
				return range;
			if(CheckInside(range, (CellRange)parent))
				return range;
			int x1 = Math.Max(range.TopLeft.Column, parent.TopLeft.Column);
			int y1 = Math.Max(range.TopLeft.Row, parent.TopLeft.Row);
			int x2 = Math.Min(range.BottomRight.Column, parent.BottomRight.Column);
			int y2 = Math.Min(range.BottomRight.Row, parent.BottomRight.Row);
			range = new CellRange(sheet, new CellPosition(x1, y1), new CellPosition(x2, y2));
			return range;
		}
		protected override CellRange CalculateTargetRange(CellPosition commitPosition) {
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			PageGrid gridRows = LastPage.GridRows;
			PageGrid gridColumns = LastPage.GridColumns;
			int resultPositionRow = gridRows[gridRows.LookupFarItem(commitPosition.Row)].ModelIndex;
			int resultPositionColumn = gridColumns[gridColumns.LookupFarItem(commitPosition.Column)].ModelIndex;
			int targetRangeFirstCol = OriginalRange.TopLeft.Column;
			int targetRangeLastCol = OriginalRange.BottomRight.Column;
			int targetRangeFirstRow = OriginalRange.TopLeft.Row;
			int targetRangeLastRow = OriginalRange.BottomRight.Row;
			if(top)
				targetRangeFirstRow = resultPositionRow;
			else
				targetRangeLastRow = resultPositionRow;
			if(left)
				targetRangeFirstCol = resultPositionColumn;
			else
				targetRangeLastCol = resultPositionColumn;
			CellRange targetRange = new CellRange(sheet, new CellPosition(targetRangeFirstCol, targetRangeFirstRow), new CellPosition(targetRangeLastCol, targetRangeLastRow));
			targetRange = FixIntersectsWithMergedCells(targetRange);
			return targetRange;
		}
	}
	#endregion
}
