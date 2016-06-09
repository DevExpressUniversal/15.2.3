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

namespace DevExpress.XtraSpreadsheet.Model {
	#region InsertRemoveRangeNotificationContextBase (abstract class)
	public abstract class InsertRemoveRangeNotificationContextBase {
		CellRange range;
		bool suppressTableChecks;
		RemovedDefaultRPNVisitor visitor;
		public CellRange Range {
			get { return range; }
			set {
				range = value;
				visitor = GetVisitor(value);
			}
		}
		public bool SuppressTableChecks { get { return suppressTableChecks; } set { suppressTableChecks = value; } }
		public RemovedDefaultRPNVisitor Visitor { get { return visitor; } }
		protected internal bool CheckEqualSheetStartDefinition(WorkbookDataContext dataContext, int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = dataContext.GetSheetDefinition(sheetDefinitionIndex);
			return sheetDefinition.GetSheetStart(dataContext) == Range.Worksheet;
		}
		protected internal bool CheckEqualSheetId(WorkbookDataContext dataContext) {
			return dataContext.CurrentWorksheet.SheetId == range.SheetId;
		}
		protected abstract RemovedDefaultRPNVisitor GetVisitor(CellRange range);
	}
	#endregion
	#region InsertRangeNotificationContext
	public class InsertRangeNotificationContext : InsertRemoveRangeNotificationContextBase {
		#region Static members
		public static InsertRangeNotificationContext Create(InsertCellMode mode, bool suppressTableChecks, InsertCellsFormatMode formatMode) {
			InsertRangeNotificationContext result = new InsertRangeNotificationContext();
			result.Mode = mode;
			result.SuppressTableChecks = suppressTableChecks;
			result.FormatMode = formatMode;
			return result;
		}
		#endregion
		#region Fields
		InsertCellMode mode;
		InsertCellsFormatMode formatMode;
		#endregion
		#region Properties
		public InsertCellMode Mode { get { return mode; } set { mode = value; } }
		public InsertCellsFormatMode FormatMode { get { return formatMode; } set { formatMode = value; } }
		#endregion
		protected override RemovedDefaultRPNVisitor GetVisitor(CellRange range) {
			return RemovedDefaultRPNVisitor.GetInsertWalker(this, ((Worksheet)range.Worksheet).DataContext);
		}
	}
	#endregion
	#region RemoveRangeNotificationContext
	public class RemoveRangeNotificationContext : InsertRemoveRangeNotificationContextBase {
		#region Static members
		public static RemoveRangeNotificationContext Create(RemoveCellMode mode, bool suppressTableChecks, bool clearFormat, bool clearTableColumnsTotalsRow) {
			RemoveRangeNotificationContext result = new RemoveRangeNotificationContext();
			result.Mode = mode;
			result.SuppressTableChecks = suppressTableChecks;
			result.ClearFormat = clearFormat;
			result.clearTableColumnsTotalsRow = clearTableColumnsTotalsRow;
			return result;
		}
		#endregion
		#region Fields
		RemoveCellMode mode;
		bool clearFormat;
		bool clearTableColumnsTotalsRow;
		bool suppressDataValidationSplit = false;
		bool shouldClearCells = true;
		#endregion
		#region Properties
		public RemoveCellMode Mode { get { return mode; } set { mode = value; } }
		public bool ClearFormat { get { return clearFormat; } set { clearFormat = value; } }
		public bool ClearTableColumnsTotalsRow { get { return clearTableColumnsTotalsRow; } }
		public bool SuppressDataValidationSplit { get { return suppressDataValidationSplit; } set { suppressDataValidationSplit = value; } }
		public bool ShouldClearCells { get { return shouldClearCells; } set { shouldClearCells = value; } }
		#endregion
		protected override RemovedDefaultRPNVisitor GetVisitor(CellRange range) {
			return RemovedDefaultRPNVisitor.GetWalker(this, ((Worksheet)range.Worksheet).DataContext);
		}
	}
	#endregion
}
