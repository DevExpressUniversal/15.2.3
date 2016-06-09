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
using DevExpress.Office;
using System.Collections;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SparklinesModifyRangeBase
	public abstract class SparklinesModifyRangeBase : ErrorHandledWorksheetCommand {
		#region Static Members
		public static ModelErrorType GetDataRangeError(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange)
				return ModelErrorType.UnionRangeNotAllowed;
			if (range.Height != 1 && range.Width != 1)
				return ModelErrorType.RangeMustConsistOfASingleRowOrColumn;
			return ModelErrorType.None;
		}
		#endregion
		#region Fields
		readonly CellRangeBase targetRange;
		readonly CellRange sourceRange;
		ParsedExpression expression;
		#endregion
		protected SparklinesModifyRangeBase(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, CellRangeBase targetRange, CellRange sourceRange) 
			: base(documentModelPart, errorHandler) {
			this.targetRange = targetRange;
			this.sourceRange = sourceRange;
			this.expression = new ParsedExpression();
		}
		#region Properties
		protected CellRangeBase TargetRange { get { return targetRange; } }
		protected ParsedExpression Expression { get { return expression; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (targetRange != null) {
				string formula = targetRange.ToString(true);
				expression = DataContext.ParseExpression(formula, OperandDataType.None, true);
			}
		}
		protected internal override bool Validate() {
			if (targetRange == null)
				return true;
			if (sourceRange != null && sourceRange.Equals(targetRange))
				return false;
			ModelErrorType errorType = GetDataRangeError(targetRange);
			if (errorType != ModelErrorType.None)
				return HandleError(new ModelErrorInfo(errorType));
			return true;
		}
	}
	#endregion
	#region SparklineGroupModifyDateRangeCommand
	public class SparklineGroupModifyDateRangeCommand : SparklinesModifyRangeBase {
		readonly SparklineGroup sparklineGroup;
		public SparklineGroupModifyDateRangeCommand(SparklineGroup sparklineGroup, IErrorHandler errorHandler, CellRangeBase targetRange)
			: base(sparklineGroup.DocumentModelPart, errorHandler, targetRange, sparklineGroup.DateRange) {
			this.sparklineGroup = sparklineGroup;
		}
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			sparklineGroup.UseDateAxis = TargetRange != null;
			sparklineGroup.Expression = Expression;
		}
	}
	#endregion
	#region SparklineModifySourceRangeCommand
	public class SparklineModifyDataRangeCommand : SparklinesModifyRangeBase {
		readonly Sparkline sparkline;
		public SparklineModifyDataRangeCommand(Sparkline sparkline, IErrorHandler errorHandler, CellRangeBase targetRange)
			: base(sparkline.Sheet, errorHandler, targetRange, sparkline.SourceDataRange) {
			this.sparkline = sparkline;
		}
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			sparkline.Expression = Expression;
		}
	}
	#endregion
	#region SparklineAddCommand
	public class SparklineAddCommand : SparklinesModifyRangeBase {
		readonly SparklineCollection collection;
		readonly CellPosition position;
		readonly bool skipValidation;
		public SparklineAddCommand(SparklineCollection collection, IErrorHandler errorHandler, CellRangeBase targetRange, CellPosition position)
			: this(collection, errorHandler, targetRange, position, false) {
		}
		public SparklineAddCommand(SparklineCollection collection, IErrorHandler errorHandler, CellRangeBase targetRange, CellPosition position, bool skipValidation)
			: base(collection.DocumentModelPart, errorHandler, targetRange, null) {
			this.collection = collection;
			this.position = position;
			this.skipValidation = skipValidation;
		}
		protected internal override void ExecuteCore() {
			Sparkline sparkline = new Sparkline(Worksheet);
			sparkline.Sheet.Workbook.BeginUpdate();
			try {
				base.ExecuteCore();
				sparkline.Position = position;
				sparkline.Expression = Expression;
			}
			finally {
				sparkline.Sheet.Workbook.EndUpdate();
			}
			collection.Add(sparkline);
		}
		protected internal override bool Validate() {
			if (skipValidation)
				return true;
			return base.Validate();
		}
	}
	#endregion
	#region SparklineGroupAddCommand
	public class SparklineGroupAddCommand : ErrorHandledWorksheetCommand {
		#region Fields
		readonly CellRangeBase dataRange;
		readonly CellRangeBase positionRange;
		readonly SparklineGroupType type;
		List<CellRangeBase> dataRangesList = new List<CellRangeBase>();
		List<CellPosition> positionsList = new List<CellPosition>();
		#endregion
		public SparklineGroupAddCommand(Worksheet sheet, IErrorHandler errorHandler, CellRangeBase dataRange, CellRangeBase positionRange, SparklineGroupType type)
			: base(sheet, errorHandler) {
			this.dataRange = dataRange;
			this.positionRange = positionRange;
			this.type = type;
		}
		long PositionCount { get { return positionsList.Count; } }
		protected internal override void ExecuteCore() {
			SparklineGroup sparklineGroup = new SparklineGroup(Worksheet);
			sparklineGroup.BeginUpdate();
			try {
				sparklineGroup.Type = type;
				for (int i = 0; i < PositionCount; i++)
					CreateSparkline(sparklineGroup.Sparklines, i);
			}
			finally {
				sparklineGroup.EndUpdate();
			}
			Worksheet.SparklineGroups.Add(sparklineGroup);
		}
		void CreateSparkline(SparklineCollection sparklines, int index) {
			SparklineAddCommand command = new SparklineAddCommand(sparklines, ErrorHandler, dataRangesList[index], positionsList[index], true);
			command.Execute();
		}
		#region Validate
		protected internal override bool Validate() {
			ModelErrorType errorType = ProcessRanges(positionRange, dataRange);
			if (errorType != ModelErrorType.None)
				return HandleError(new ModelErrorInfo(errorType));
			return true;
		}
		#region ProcessRanges
		ModelErrorType ProcessRanges(CellRangeBase positionRange, CellRangeBase dataRange) {
			if (positionRange == null || dataRange == null)
				return ModelErrorType.SparklinePositionOrDataRangeIsInvalid;
			ModelErrorType errorType = GetPositionRangeError(positionRange);
			if (errorType != ModelErrorType.None)
				return errorType;
			List<CellRange> positionAreaList = new List<CellRange>(positionRange.GetAreasEnumerable());
			for (int i = 0; i < positionAreaList.Count; i++) {
				CellRange current = positionAreaList[i];
				int lastIndex = positionAreaList.FindLastIndex(current.EqualsPosition);
				if (lastIndex > 0 && lastIndex != i)
					return ModelErrorType.SparklinePositionsMustBeUnique;
				AddPositionsToList(current);
			}
			if (dataRange.AreasCount == 1)
				return ProcessRangesCore(GetSingleRange(dataRange));
			return ProcessRangesCore(((CellUnion)dataRange).InnerCellRanges);
		}
		void AddPositionsToList(CellRange range) {
			CellPositionEnumerator enumerator = range.GetAllPositionsEnumerator();
			while (enumerator.MoveNext())
				positionsList.Add(enumerator.Current);
		}
		void AddDataRangesToList() {
			foreach (CellRange range in dataRange.GetAreasEnumerable()) {
				CellPositionEnumerator enumerator = range.GetAllPositionsEnumerator();
				while (enumerator.MoveNext())
					dataRangesList.Add(new CellRange(range.Worksheet, enumerator.Current, enumerator.Current));
			}
		}
		ModelErrorType ProcessRangesCore(CellRangeBase dataRange) {
			ModelErrorType errorType = GetDataRangeError(dataRange);
			if (errorType != ModelErrorType.None)
				return errorType;
			if (positionRange.CellCount == 1) {
				dataRangesList.Add(dataRange);
				return ModelErrorType.None;
			}
			if (positionRange.CellCount == dataRange.CellCount) {
				AddDataRangesToList();
				return ModelErrorType.None;
			}
			return ModelErrorType.SparklinePositionOrDataRangeIsInvalid;
		}
		ModelErrorType ProcessRangesCore(List<CellRangeBase> dataRanges) {
			ModelErrorType errorType = GetUnionRangeError(dataRanges, GetDataRangeError);
			if (errorType != ModelErrorType.None)
				return errorType;
			if (positionRange.CellCount == dataRanges.Count) {
				dataRangesList.AddRange(dataRanges);
				return ModelErrorType.None;
			}
			if (DateRangesAreSingleCells(dataRanges)) {
				foreach (CellRangeBase dataRange in dataRanges)
					dataRangesList.AddRange(dataRange.GetAreasEnumerable());
				return ModelErrorType.None;
			}
			return ModelErrorType.SparklinePositionOrDataRangeIsInvalid;
		}
		#endregion
		#region GetRangeError
		delegate ModelErrorType GetSingleRangeError(CellRangeBase range);
		ModelErrorType GetDataRangeError(CellRangeBase range) {
			return SparklinesModifyRangeBase.GetDataRangeError(range);
		}
		ModelErrorType GetPositionRangeError(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange)
				return GetUnionRangeError(((CellUnion)range).InnerCellRanges, GetPositionRangeErrorCore);
			return GetPositionRangeErrorCore(range);
		}
		ModelErrorType GetPositionRangeErrorCore(CellRangeBase range) {
			if (!Object.ReferenceEquals(range.Worksheet, Worksheet))
				return ModelErrorType.ErrorUseRangeFromAnotherWorksheet;
			return GetDataRangeError(range);
		}
		ModelErrorType GetUnionRangeError(List<CellRangeBase> innerRanges, GetSingleRangeError getError) {
			foreach (CellRangeBase range in innerRanges) {
				ModelErrorType errorType = getError(range);
				if (errorType != ModelErrorType.None)
					return errorType;
			}
			return ModelErrorType.None;
		}
		#endregion
		CellRangeBase GetSingleRange(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange)
				return ((CellUnion)range).InnerCellRanges[0];
			return range;
		}
		bool DateRangesAreSingleCells(List<CellRangeBase> ranges) {
			long result = 0;
			foreach (CellRangeBase range in ranges) {
				if (range.CellCount != 1)
					return false;
				result += range.CellCount;
			}
			return result == PositionCount;
		}
		#endregion
	}
	#endregion
}
