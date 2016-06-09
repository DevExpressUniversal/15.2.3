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

using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelCommand
	public abstract class ModelCommand {
		DevExpress.Office.IDocumentModelPart documentModelPart;
		protected ModelCommand(DevExpress.Office.IDocumentModelPart documentModelPart) {
			Guard.ArgumentNotNull(documentModelPart, "documentModel");
			this.documentModelPart = documentModelPart;
		}
		public IDocumentModel DocumentModel { get { return documentModelPart.DocumentModel; } }
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		public object Result { get; protected set; }
		public bool Execute() {
			if (!Validate())
				return false;
			try {
				BeginExecute();
				ExecuteCore();
			}
			finally {
				EndExecute();
			}
			return true;
		}
		protected internal virtual void BeginExecute() {
			DocumentModel.BeginUpdate();
		}
		protected internal virtual void EndExecute() {
			ApplyChanges();
			DocumentModel.EndUpdate();
		}
		protected internal virtual bool Validate() {
			return true;
		}
		protected internal abstract void ExecuteCore();
		protected internal virtual void ApplyChanges() {
		}
	}
	#endregion
	#region SpreadsheetModelCommand
	public abstract class SpreadsheetModelCommand : ModelCommand {
		protected SpreadsheetModelCommand(DevExpress.Office.IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		public Worksheet Worksheet { [System.Diagnostics.DebuggerStepThrough] get { return (Worksheet)DocumentModelPart; } }
		public new DocumentModel DocumentModel { [System.Diagnostics.DebuggerStepThrough] get { return (DocumentModel)base.DocumentModel; } }
		public WorkbookDataContext DataContext { [System.Diagnostics.DebuggerStepThrough] get { return DocumentModel.DataContext; } }
	}
	#endregion
	#region ErrorHandledWorksheetCommand
	public abstract class ErrorHandledWorksheetCommand : SpreadsheetModelCommand {
		readonly IErrorHandler errorHandler;
		protected ErrorHandledWorksheetCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler)
			: base(documentModelPart) {
			this.errorHandler = errorHandler;
		}
		protected IErrorHandler ErrorHandler { get { return errorHandler; } }
		protected bool HandleError(IModelErrorInfo error) {
			if (error != null) {
				if (errorHandler.HandleError(error) == ErrorHandlingResult.Abort)
					return false;
			}
			return true;
		}
	}
	#endregion
	#region DrawingObjectsDimensionCalculator
	public class DrawingObjectsDimensionCalculator {
		bool lockAspectRatio;
		int pictureWidth;
		int pictureHeight;
		float desiredWidth;
		float desiredHeight;
		float resultHeight;
		float resultWidth;
		public DrawingObjectsDimensionCalculator(int pictureWidth, int pictureHeight, float desiredWidth, float desiredHeight, bool lockAspectRatio) {
			this.lockAspectRatio = lockAspectRatio;
			this.pictureWidth = pictureWidth;
			this.pictureHeight = pictureHeight;
			this.desiredWidth = desiredWidth;
			this.desiredHeight = desiredHeight;
		}
		public float ResultHeight { get { return resultHeight; } }
		public float ResultWidth { get { return resultWidth; } }
		public void Calculate() {
			if (desiredHeight == 0 && desiredWidth == 0) {
				this.resultHeight = pictureHeight;
				this.resultWidth = pictureWidth;
				return;
			}
			if (lockAspectRatio) {
				float kx = desiredWidth / pictureWidth;
				float ky = desiredHeight / pictureHeight;
				if (kx > ky) {
					this.resultWidth = pictureWidth * ky;
					this.resultHeight = desiredHeight;
				}
				else {
					this.resultHeight = pictureHeight * kx;
					this.resultWidth = desiredWidth;
				}
			}
			else {
				this.resultHeight = desiredHeight;
				this.resultWidth = desiredWidth;
			}
		}
	}
	#endregion
	#region ExternalWorkbook commands
	public class ExternalWorkbookRemoveCommand : ErrorHandledWorksheetCommand {
		readonly ExternalLink link;
		readonly bool modifyReferences;
		int index;
		public ExternalWorkbookRemoveCommand(ExternalLink link, IDocumentModelPart documentModelPart, IErrorHandler errorHandler, bool modifyReferences)
			: base(documentModelPart, errorHandler) {
			this.link = link;
			this.modifyReferences = modifyReferences;
		}
		protected internal override bool Validate() {
			index = DocumentModel.ExternalLinks.IndexOf(link);
			if (index < 0)
				return HandleError(new ModelErrorInfo(ModelErrorType.UsingInvalidObject));
			return base.Validate();
		}
		protected internal override void ExecuteCore() {
			if (modifyReferences) {
				ExternalLinkRemovedFormulaWalker walker = new ExternalLinkRemovedFormulaWalker(index, DataContext);
				walker.Walk(DocumentModel);
			}
			DocumentModel.ExternalLinks.RemoveAt(index, modifyReferences);
			DocumentModel.InternalAPI.RaiseExternalLinkRemoved(link);
		}
	}
	public class ExternalWorkbookAddCommand : ErrorHandledWorksheetCommand {
		readonly ExternalLink link;
		public ExternalWorkbookAddCommand(ExternalLink link, IDocumentModelPart documentModelPart, IErrorHandler errorHandler)
			: base(documentModelPart, errorHandler) {
			this.link = link;
		}
		protected internal override void ExecuteCore() {
			DocumentModel.ExternalLinks.AddCore(link, DocumentModel.ExternalLinks.Count, false);
			DocumentModel.InternalAPI.RaiseExternalLinkAdded(link);
		}
	}
	public class ExternalWorkbookCollectionClearCommand : ErrorHandledWorksheetCommand {
		public ExternalWorkbookCollectionClearCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler)
			: base(documentModelPart, errorHandler) {
		}
		protected internal override void ExecuteCore() {
			DocumentModel.ExternalLinks.Clear();
			DocumentModel.InternalAPI.RaiseExternalLinksCollectionClear();
		}
	}
	#endregion
	#region HideUnhideCommadBase
	public abstract class HideUnhideCommadBase : SpreadsheetModelCommand {
		protected delegate void HideUnhideDelegate(int index);
		readonly int startIndex;
		readonly int endIndex;
		readonly bool needUpdateOutline;
		int maxOutlineLevel = 0;
		protected HideUnhideCommadBase(Worksheet sheet, int startIndex, int endIndex, bool needUpdateOutline)
			: base(sheet) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.needUpdateOutline = needUpdateOutline;
			this.maxOutlineLevel = 0;
		}
		protected int StartIndex { get { return startIndex; } }
		protected int EndIndex { get { return endIndex; } }
		protected int MaxOutlineLevel { get { return maxOutlineLevel; } set { maxOutlineLevel = value; } }
		protected bool NeedUpdateOutline { get { return needUpdateOutline; } }
	}
	#endregion
	#region HideColumnsCommand
	public class HideColumnsCommand : HideUnhideCommadBase {
		protected delegate void HideUnhideColumnDelegate(Column column);
		public HideColumnsCommand(Worksheet sheet, int startIndex, int endIndex, bool needUpdateOutline)
			: base(sheet, startIndex, endIndex, needUpdateOutline) {
		}
		protected internal override void ExecuteCore() {
			HideColumns(StartIndex, EndIndex);
		}
		void HideColumns(int startIndex, int endIndex) {
			MaxOutlineLevel = 0;
			Worksheet.NeedColumnUnhideNotificated = false;
			HideUnhideColumnsCore(startIndex, endIndex, HideColumn);
			Worksheet.NeedColumnUnhideNotificated = true;
			if (NeedUpdateOutline && MaxOutlineLevel > 0)
				UpdateColumnOutlineGroup(startIndex, endIndex, MaxOutlineLevel, true);
			Worksheet.RaiseHideUnhideColumns(startIndex, endIndex, true);
		}
		protected void HideUnhideColumnsCore(int startIndex, int endIndex, HideUnhideColumnDelegate action) {
			HideUnhideCore(startIndex, endIndex, action);
			CellIntervalRange affectedRange = CellIntervalRange.CreateColumnInterval(Worksheet, startIndex, PositionType.Absolute, endIndex, PositionType.Absolute);
			Worksheet.WebRanges.ChangeRange(affectedRange);
			DocumentModel.CalculationChain.MarkupDependentsForRecalculationWithHistory(affectedRange);
			Worksheet.Tables.MarkupTablesForInvalidateFormatCache(affectedRange);
			Worksheet.PivotTables.MarkupPivotTablesForInvalidateFormatCache(affectedRange);
		}
		protected void HideUnhideCore(int startIndex, int endIndex, HideUnhideColumnDelegate action) {
			IList<Column> columns = Worksheet.Columns.GetColumnRangesEnsureExist(startIndex, endIndex);
			foreach (Column column in columns) {
				action(column);
			}
		}
		void HideColumn(Column column) {
			column.IsHidden = true;
			MaxOutlineLevel = System.Math.Max(MaxOutlineLevel, column.OutlineLevel);
			ModelWorksheetView worksheetView = Worksheet.ActiveView;
			if (worksheetView.TopLeftCell.Column == column.StartIndex && !worksheetView.IsFrozen())
				worksheetView.TopLeftCell = new CellPosition(column.StartIndex + 1, worksheetView.TopLeftCell.Row);
		}
		protected void UpdateColumnOutlineGroup(int firstIndex, int lastIndex, int outlineLevel, bool collapse) {
			GroupList groups = new ColumnGroupCollector(Worksheet).StrongCollectGroups(firstIndex, lastIndex);
			foreach (GroupItem group in groups.Groups)
				if (group.Level <= outlineLevel && (group.Start >= firstIndex && group.End <= lastIndex || !collapse))
					Worksheet.Columns.GetIsolatedColumn(group.ButtonPosition).IsCollapsed = collapse;
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region UnhideColumnsCommand
	public class UnhideColumnsCommand : HideColumnsCommand {
		public UnhideColumnsCommand(Worksheet sheet, int startIndex, int endIndex, bool needUpdateOutline)
			: base(sheet, startIndex, endIndex, needUpdateOutline) {
		}
		protected internal override void ExecuteCore() {
			UnhideColumns(StartIndex, EndIndex);
		}
		public void UnhideColumns(int startIndex, int endIndex) {
			MaxOutlineLevel = 0;
			Worksheet.NeedColumnUnhideNotificated = false;
			HideUnhideColumnsCore(startIndex, endIndex, UnhideColumn);
			Worksheet.NeedColumnUnhideNotificated = true;
			if (NeedUpdateOutline && MaxOutlineLevel > 0)
				UpdateColumnOutlineGroup(startIndex, endIndex, MaxOutlineLevel, false);
			Worksheet.RaiseHideUnhideColumns(startIndex, endIndex, false);
		}
		protected internal void UnhideColumn(Column column) {
			MaxOutlineLevel = System.Math.Max(MaxOutlineLevel, column.OutlineLevel);
			if (column.IsCustomWidth && column.Width == 0) {
				column.BeginUpdate();
				try {
					column.IsCustomWidth = false;
					column.IsHidden = false;
				}
				finally {
					column.EndUpdate();
				}
			}
			else
				column.IsHidden = false;
		}
	}
	#endregion
	#region HideRowsCommand
	public class HideRowsCommand : HideUnhideCommadBase {
		public HideRowsCommand(Worksheet sheet, int startIndex, int endIndex, bool needUpdateOutline)
			: base(sheet, startIndex, endIndex, needUpdateOutline) {
		}
		protected internal override void ExecuteCore() {
			HideRows(StartIndex, EndIndex);
		}
		void HideRows(int startIndex, int endIndex) {
			Worksheet.NeedRowUnhideNotificated = false;
			HideUnhideRowsCore(startIndex, endIndex, HideRow);
			Worksheet.NeedRowUnhideNotificated = true;
			if (NeedUpdateOutline && MaxOutlineLevel > 0)
				UpdateRowOutlineGroup(startIndex, endIndex, MaxOutlineLevel, true);
			Worksheet.RaiseHideUnhideRows(startIndex, endIndex, true);
		}
		void HideRow(int index) {
			Row row = Worksheet.Rows[index];
			row.IsHidden = true;
			MaxOutlineLevel = System.Math.Max(MaxOutlineLevel, row.OutlineLevel);
			ModelWorksheetView worksheetView = Worksheet.ActiveView;
			if (worksheetView.TopLeftCell.Row == index && !worksheetView.IsFrozen())
				worksheetView.TopLeftCell = new CellPosition(worksheetView.TopLeftCell.Column, index + 1);
		}
		protected void HideUnhideRowsCore(int startIndex, int endIndex, HideUnhideDelegate action) {
			HideUnhideCore(startIndex, endIndex, action);
			CellIntervalRange affectedRange = CellIntervalRange.CreateRowInterval(Worksheet, startIndex, PositionType.Absolute, endIndex, PositionType.Absolute);
			Worksheet.WebRanges.ChangeRange(affectedRange);
			DocumentModel.CalculationChain.MarkupDependentsForRecalculationWithHistory(affectedRange);
			Worksheet.Tables.MarkupTablesForInvalidateFormatCache(affectedRange);
			Worksheet.PivotTables.MarkupPivotTablesForInvalidateFormatCache(affectedRange);
		}
		void HideUnhideCore(int startIndex, int endIndex, HideUnhideDelegate action) {
			for (int i = startIndex; i <= endIndex; i++)
				action(i);
		}
		protected void UpdateRowOutlineGroup(int firstIndex, int lastIndex, int outlineLevel, bool collapse) {
			GroupList groups = new RowGroupCollector(Worksheet).StrongCollectGroups(firstIndex, lastIndex);
			foreach (GroupItem group in groups.Groups)
				if (group.Level <= outlineLevel && (group.Start >= firstIndex && group.End <= lastIndex || !collapse))
					Worksheet.Rows[group.ButtonPosition].IsCollapsed = collapse;
			Worksheet.RowGroupCache = null;
		}
	}
	#endregion
	#region UnhideRowsCommand
	public class UnhideRowsCommand : HideRowsCommand {
		public UnhideRowsCommand(Worksheet sheet, int startIndex, int endIndex, bool needUpdateOutline)
			: base(sheet, startIndex, endIndex, needUpdateOutline) {
		}
		protected internal override void ExecuteCore() {
			UnhideRows(StartIndex, EndIndex);
		}
		void UnhideRows(int startIndex, int endIndex) {
			MaxOutlineLevel = 0;
			Worksheet.NeedRowUnhideNotificated = false;
			HideUnhideRowsCore(startIndex, endIndex, UnhideRow);
			Worksheet.NeedRowUnhideNotificated = true;
			if (NeedUpdateOutline && MaxOutlineLevel > 0)
				UpdateRowOutlineGroup(startIndex, endIndex, MaxOutlineLevel, false);
			Worksheet.RaiseHideUnhideRows(startIndex, endIndex, false);
			Worksheet.Rows.CheckForEmptyRows(startIndex, endIndex);
		}
		void UnhideRow(int index) {
			Row row = Worksheet.Rows.TryGetRow(index);
			if (row == null)
				return;
			MaxOutlineLevel = System.Math.Max(MaxOutlineLevel, row.OutlineLevel);
			if (row.IsCustomHeight && row.Height == 0) {
				row.BeginUpdate();
				try {
					row.IsCustomHeight = false;
					row.IsHidden = false;
				}
				finally {
					row.EndUpdate();
				}
			}
			else
				row.IsHidden = false;
		}
	}
	#endregion
	#region RemoveRangeCommand
	public class RemoveRangeCommand : ErrorHandledWorksheetCommand {
		#region Fields
		CellRangeBase rangeBase;
		RemoveCellMode mode;
		bool suppressTableChecks;
		bool clearFormat;
		#endregion
		public RemoveRangeCommand(Worksheet sheet, CellRangeBase rangeBase, RemoveCellMode mode, bool suppressTableChecks, bool clearFormat, IErrorHandler errorHandler)
			: base(sheet, errorHandler) {
			Guard.ArgumentNotNull(rangeBase, "rangeBase");
			System.Diagnostics.Debug.Assert(rangeBase.Worksheet == null || rangeBase.Worksheet.SheetId == sheet.SheetId);
			this.rangeBase = rangeBase;
			this.mode = mode;
			this.suppressTableChecks = suppressTableChecks;
			this.clearFormat = clearFormat;
		}
		#region Properties
		public CellRangeBase RangeBase { get { return rangeBase; } }
		public RemoveCellMode Mode { get { return mode; } }
		public bool SuppressTableChecks { get { return suppressTableChecks; } }
		public bool ClearFormat { get { return clearFormat; } }
		public int SheetId { get { return Worksheet.SheetId; } }
		public WorksheetCollection Sheets { get { return DocumentModel.Sheets; } }
		public Table ActualTable { get; set; }
		public bool SuppressDataValidationSplit { get; set; }
		public bool SuppressPivotTableChecks { get; set; }
		#endregion
		protected internal override bool Validate() {
			FixRangeBase();
			if (rangeBase == null)
				return false;
			NotificationChecks checks = NotificationChecks.All & ~NotificationChecks.ProtectedCells;
			if (suppressTableChecks)
				checks &= ~NotificationChecks.Table;
			if (SuppressPivotTableChecks)
				checks &= ~NotificationChecks.PivotTable;
			IModelErrorInfo error = DocumentModel.CanRangeRemove(rangeBase, mode, checks);
			if (error == null || error is ClarificationErrorInfo)
				return true;
			return HandleError(error);
		}
		protected virtual void FixRangeBase() {
			if (rangeBase.Worksheet == null)
				rangeBase.Worksheet = Worksheet;
			if (rangeBase.RangeType == CellRangeType.UnionRange)
				rangeBase = GetSortedUnionRanges();
		}
		protected internal override void BeginExecute() {
			BeginExecuteCore();
			base.BeginExecute();
		}
		protected virtual void BeginExecuteCore() {
			if (mode == RemoveCellMode.ShiftCellsLeft)
				DocumentModel.InternalAPI.OnShiftCellsLeft(Worksheet, rangeBase.TopLeft.Column, rangeBase.TopLeft.Row, rangeBase.BottomRight.Row);
			else
				DocumentModel.InternalAPI.OnShiftCellsUp(Worksheet, rangeBase.TopLeft.Column, rangeBase.BottomRight.Column, rangeBase.TopLeft.Row);
		}
		protected internal override void ExecuteCore() {
			RemoveRangeNotificationContext context = RemoveRangeNotificationContext.Create(mode, suppressTableChecks, clearFormat, true);
			context.SuppressDataValidationSplit = SuppressDataValidationSplit;
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				List<CellRangeBase> innerRanges = GetSortedUnionRanges().InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++) {
					context.Range = innerRanges[i] as CellRange;
					Worksheet.OnSingleRangeRemoving(context);
				}
			} else {
				context.Range = rangeBase as CellRange;
				Worksheet.OnSingleRangeRemoving(context);
			}
			DocumentModel.OnAfterRangeRemoving(SheetId, rangeBase, mode);
			if (mode != RemoveCellMode.Default)
				DocumentModel.ApplyChanges(DocumentModelChangeActions.ResetInvalidDataCircles | DocumentModelChangeActions.Redraw);
		}
		CellUnion GetSortedUnionRanges() {
			return (rangeBase as CellUnion).GetUnionWithSortedInnerRanges(mode);
		}
	}
	#endregion
	#region RemoveRowsColumnsCommandBase (abstract class)
	public abstract class RemoveRowsColumnsCommandBase : RemoveRangeCommand {
		protected RemoveRowsColumnsCommandBase(Worksheet sheet, CellIntervalRange range, RemoveCellMode mode, IErrorHandler errorHandler)
			: base(sheet, range, mode, false, false, errorHandler) {
		}
		#region Properties
		protected abstract int StartIndex { get; }
		protected abstract int Count { get; }
		#endregion
		protected override void FixRangeBase() {
		}
		protected override void BeginExecuteCore() {
			int count = StartIndex + Count;
			for (int i = StartIndex; i < count; i++)
				PerformInternalAPIAction(i);
		}
		protected abstract void PerformInternalAPIAction(int index);
	}
	#endregion
	#region RemoveRowsCommand
	public class RemoveRowsCommand : RemoveRowsColumnsCommandBase {
		#region Static Members
		static CellIntervalRange CreateDeletedRange(Worksheet sheet, int startIndex, int count) {
			Guard.ArgumentPositive(count, "count");
			return CellIntervalRange.CreateRowInterval(sheet, startIndex, PositionType.Absolute, startIndex + count - 1, PositionType.Absolute); 
		}
		#endregion
		public RemoveRowsCommand(Worksheet sheet, int startIndex, int count, IErrorHandler errorHandler)
			: base(sheet, CreateDeletedRange(sheet, startIndex, count), RemoveCellMode.ShiftCellsUp, errorHandler) {
		}
		protected override int StartIndex { get { return ((CellIntervalRange)RangeBase).TopRowIndex; } }
		protected override int Count { get { return ((CellIntervalRange)RangeBase).Height; } }
		protected override void PerformInternalAPIAction(int index) {
			DocumentModel.InternalAPI.OnBeforeRowRemoved(Worksheet, index, Count);
		}
	}
	#endregion
	#region RemoveColumnsCommand
	public class RemoveColumnsCommand : RemoveRowsColumnsCommandBase {
		#region Static Members
		static CellIntervalRange CreateDeletedRange(Worksheet sheet, int startIndex, int count) {
			Guard.ArgumentPositive(count, "count");
			return CellIntervalRange.CreateColumnInterval(sheet, startIndex, PositionType.Absolute, startIndex + count - 1, PositionType.Absolute); 
		}
		#endregion
		public RemoveColumnsCommand(Worksheet sheet, int startIndex, int count, IErrorHandler errorHandler)
			: base(sheet, CreateDeletedRange(sheet, startIndex, count), RemoveCellMode.ShiftCellsLeft, errorHandler) {
		}
		protected override int StartIndex { get { return ((CellIntervalRange)RangeBase).LeftColumnIndex; } }
		protected override int Count { get { return ((CellIntervalRange)RangeBase).Width; } }
		protected override void PerformInternalAPIAction(int index) {
			DocumentModel.InternalAPI.OnColumnRemoved(Worksheet, index);
		}
	}
	#endregion
	#region InsertRangeCommand
	public class InsertRangeCommand : ErrorHandledWorksheetCommand {
		#region Fields
		CellRangeBase rangeBase;
		InsertCellMode mode;
		bool suppressTableChecks;
		InsertCellsFormatMode formatMode;
		#endregion
		public InsertRangeCommand(Worksheet sheet, CellRangeBase rangeBase, InsertCellMode mode, bool suppressTableChecks, InsertCellsFormatMode formatMode, IErrorHandler errorHandler)
			: base(sheet, errorHandler) {
			Guard.ArgumentNotNull(rangeBase, "rangeBase");
			System.Diagnostics.Debug.Assert(rangeBase.Worksheet == null || rangeBase.Worksheet.SheetId == sheet.SheetId);
			this.rangeBase = rangeBase;
			this.mode = mode;
			this.suppressTableChecks = suppressTableChecks;
			this.formatMode = formatMode;
		}
		protected internal override bool Validate() {
			FixRangeBase();
			if (rangeBase == null)
				return false;
			NotificationChecks checks = NotificationChecks.All & ~NotificationChecks.ProtectedCells;
			if (suppressTableChecks)
				checks &= ~NotificationChecks.Table;
			IModelErrorInfo error = DocumentModel.CanRangeInsert(rangeBase, mode, formatMode, checks);
			return HandleError(error);
		}
		protected virtual void FixRangeBase() {
			if (rangeBase.Worksheet == null)
				rangeBase.Worksheet = Worksheet;
			if (rangeBase.RangeType == CellRangeType.UnionRange)
				rangeBase = (rangeBase as CellUnion).GetUnionWithSortedInnerRanges(mode);
		}
		protected internal override void ExecuteCore() {
			InsertRangeNotificationContext context = InsertRangeNotificationContext.Create(mode, suppressTableChecks, formatMode);
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				List<CellRangeBase> innerRanges = (rangeBase as CellUnion).InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++) {
					context.Range = innerRanges[i] as CellRange;
					DocumentModel.OnSingleRangeInserting(context);
				}
			}
			else {
				context.Range = rangeBase as CellRange;
				DocumentModel.OnSingleRangeInserting(context);
			}
			DocumentModel.OnAfterRangeInserting(Worksheet.SheetId, rangeBase, mode);
			DocumentModel.ApplyChanges(DocumentModelChangeActions.ResetInvalidDataCircles | DocumentModelChangeActions.Redraw);
		}
	}
	#endregion
}
