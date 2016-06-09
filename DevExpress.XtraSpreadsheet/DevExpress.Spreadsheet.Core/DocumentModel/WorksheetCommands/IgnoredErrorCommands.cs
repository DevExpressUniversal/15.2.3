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
namespace DevExpress.XtraSpreadsheet.Model {
	#region IgnoredErrorCommandBase
	public abstract class IgnoredErrorCommandBase : ErrorHandledWorksheetCommand {
		CellRangeBase range;
		protected IgnoredErrorCommandBase(Worksheet sheet, IErrorHandler errorHandler, CellRangeBase range)
			: base(sheet, errorHandler) {
			this.range = range;
		}
		#region Properties
		protected CellRangeBase Range { get { return range; } set { range = value; } }
		protected IgnoredErrorCollection Collection { get { return Worksheet.IgnoredErrors; } }
		int Count { get { return Collection.Count; } }
		#endregion
		protected internal override void ExecuteCore() {
			for (int i = Count - 1; i >= 0; i--)
				ProcessExistingError(Collection[i]);
		}
		protected abstract void ProcessExistingError(IgnoredError existingError);
		#region OptimizeRange
		protected void OptimizeRange() {
			if (range.RangeType != CellRangeType.UnionRange || range.AreasCount == 1)
				return;
			CellUnion union = range as CellUnion;
			if (!union.HasIntersectedRanges())
				return;
			range = GetNonIntersectingRangesUnion();
		}
		CellUnion GetNonIntersectingRangesUnion() {
			List<CellRangeBase> rangesList = GetRangeSortedByCellCount();
			for (int i = 0; i < rangesList.Count; i++) {
				CellRangeBase current = rangesList[i];
				for (int j = i + 1; j < rangesList.Count; j++) {
					CellRangeBase next = rangesList[j];
					if (!current.Intersects(next))
						continue;
					if (CurrentContainsNext(current, next)) {
						rangesList.Remove(next);
						j--;
						continue;
					}
					CellRangeBase complementedRange = next.ExcludeRange(current);
					if (complementedRange != null)
						rangesList[j] = complementedRange;
				}
			}
			return new CellUnion(Worksheet, rangesList);
		}
		List<CellRangeBase> GetRangeSortedByCellCount() {
			List<CellRangeBase> result = new List<CellRangeBase>(range.GetAreasEnumerable());
			result.Sort(new CellCountComparer());
			return result;
		}
		bool CurrentContainsNext(CellRangeBase current, CellRangeBase next) {
			if (current.RangeType == CellRangeType.UnionRange || next.RangeType == CellRangeType.UnionRange)
				return false;
			return ((CellRange)current).ContainsRange((CellRange)next);
		}
		#region CellCountComparer (inner class)
		class CellCountComparer : IComparer<CellRangeBase> {
			public int Compare(CellRangeBase x, CellRangeBase y) {
				return x.CellCount > y.CellCount ? -1 : 1;
			}
		}
		#endregion
		#endregion
		#region Validate
		protected internal override bool Validate() {
			return HandleError(GetErrorInfo());
		}
		ModelErrorInfo GetErrorInfo() {
			if (range == null)
				return new ModelErrorInfo(ModelErrorType.ErrorInvalidRange);
			if (!object.ReferenceEquals(Worksheet, range.Worksheet))
				return new ModelErrorInfo(ModelErrorType.ErrorUseRangeFromAnotherWorksheet);
			return null;
		}
		#endregion
	}
	#endregion
	#region IgnoredErrorAddCommand
	public class IgnoredErrorAddCommand : IgnoredErrorCommandBase {
		#region Fields
		readonly IgnoredErrorType errorType;
		bool errorExists;
		#endregion
		public IgnoredErrorAddCommand(Worksheet sheet, IErrorHandler errorHandler, CellRangeBase range, IgnoredErrorType errorType)
			: base(sheet, errorHandler, range) {
			this.errorType = errorType;
		}
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			if (!errorExists) {
				OptimizeRange();
				Collection.Add(IgnoredError.Create(Range, errorType));
			}
		}
		#region ProcessExistingError
		protected override void ProcessExistingError(IgnoredError existingError) {
			if (existingError.GetErrorType() == errorType)
				ModifyRange(existingError);
			else
				existingError.TryClearRange(Range);
		}
		void ModifyRange(IgnoredError existingError) {
			Range = Range.MergeWithRange(existingError.Range);
			OptimizeRange();
			existingError.Range = Range;
			errorExists = true;
		}
		#endregion
		protected internal override bool Validate() {
			if (errorType == IgnoredErrorType.None)
				return false;
			return base.Validate();
		}
	}
	#endregion
	#region IgnoredErrorClearCommand
	public class IgnoredErrorClearCommand : IgnoredErrorCommandBase {
		public IgnoredErrorClearCommand(Worksheet sheet, IErrorHandler errorHandler, CellRangeBase range)
			: base(sheet, errorHandler, range) {
		}
		protected override void ProcessExistingError(IgnoredError existingError) {
			existingError.TryClearRange(Range);
		}
	}
	#endregion
}
