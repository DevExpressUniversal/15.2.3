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
using DevExpress.Office;
using DevExpress.Spreadsheet;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
namespace DevExpress.Spreadsheet {
	public interface SortState {
		IList<SortCondition> Conditions { get; }
		void Sort(int columnIndex, bool descendingOrder);
		void Sort(IList<SortCondition> conditions);
		void Clear();
	}
	public class SortCondition {
		#region Fields
		int columnIndex;
		bool descendingOrder;
		#endregion
		public SortCondition(int columnIndex, bool descendingOrder) {
			this.columnIndex = columnIndex;
			this.descendingOrder = descendingOrder;
		}
		public int ColumnIndex { get { return columnIndex; } }
		public bool Descending { get { return descendingOrder; } }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.XtraSpreadsheet.Localization;
	#region NativeSortState
	partial class NativeSortState : NativeObjectBase, SortState {
		readonly Model.SortState modelSortState;
		readonly Model.CellRange filterRange;
		public NativeSortState(Model.SortState modelSortState, Model.CellRange filterRange) {
			this.modelSortState = modelSortState;
			this.filterRange = filterRange;
		}
		#region Properties
		Model.Worksheet Worksheet { get { return modelSortState.Sheet; } }
		Model.DocumentModel DocumentModel { get { return Worksheet.Workbook; } }
		#region SortState Members
		public IList<SortCondition> Conditions {
			get {
				CheckValid();
				return CreateConditionsList();
			}
		}
		#region Sort
		public void Sort(int columnIndex, bool descending) {
			CheckValid();
			CheckValid(columnIndex);
			List<SortCondition> conditions = new List<SortCondition>();
			conditions.Add(new SortCondition(columnIndex, descending));
			SortCore(conditions);
		}
		public void Sort(IList<SortCondition> conditions) {
			CheckValid();
			CheckValid(conditions);
			SortCore(conditions);
		}
		void CheckValid(IList<SortCondition> conditions) {
			int count = conditions.Count;
			for (int i = 0; i < count; i++)
				CheckValid(conditions[i].ColumnIndex);
		}
		void CheckValid(int columnIndex) {
			if (columnIndex < 0 || columnIndex > filterRange.BottomRight.Column - filterRange.TopLeft.Column)
				HandleError(Model.ModelErrorType.InvalidSortColumnIndex);
			if (Worksheet.MergedCells.RangeContainsMergedCellRangesOfDifferentSize(filterRange))
				HandleError(Model.ModelErrorType.UnableToSortMergedCells);
			if (Worksheet.ArrayFormulaRanges.CheckMultiCellArrayFormulasInRange(filterRange))
				HandleError(Model.ModelErrorType.ErrorChangingPartOfAnArray);
		}
		void SortCore(IList<SortCondition> conditions) {
			DocumentModel.BeginUpdate();
			try {
				modelSortState.Clear();
				int count = conditions.Count;
				for (int i = 0; i < count; i++) {
					Model.SortCondition modelCondition = CreateModelSortCondition(conditions[i]);
					modelSortState.SortConditions.Add(modelCondition);
				}
				modelSortState.SortRange = filterRange.Clone();
				modelSortState.Apply(ApiErrorHandler.Instance);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Clear
		public void Clear() {
			CheckValid();
			modelSortState.Clear();
		}
		#endregion
		#endregion
		#endregion
		#region Internal
		IList<SortCondition> CreateConditionsList() {
			IList<SortCondition> result = new List<SortCondition>();
			Model.SortConditionCollection modelConditions = modelSortState.SortConditions;
			int conditionsCount = modelConditions.Count;
			for (int i = 0; i < conditionsCount; i++)
				result.Add(CreateSortCondition(modelConditions[i]));
			return result;
		}
		SortCondition CreateSortCondition(Model.SortCondition modelCondition) {
			int columnIndex = modelCondition.SortReference.TopLeft.Column - modelSortState.SortRange.TopLeft.Column;
			return new SortCondition(columnIndex, modelCondition.Descending);
		}
		Model.SortCondition CreateModelSortCondition(SortCondition condition) {
			Model.SortCondition result = new Model.SortCondition(Worksheet, GetColumnRange(condition.ColumnIndex));
			result.BeginUpdate();
			try {
				result.Descending = condition.Descending;
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
		Model.CellRange GetColumnRange(int columnIndex) {
			int absoluteColumnIndex = filterRange.TopLeft.Column + columnIndex;
			Model.CellPosition topLeft = new Model.CellPosition(absoluteColumnIndex, filterRange.TopLeft.Row - 1); 
			Model.CellPosition bottomRight = new Model.CellPosition(absoluteColumnIndex, filterRange.BottomRight.Row);
			return new Model.CellRange(modelSortState.Sheet, topLeft, bottomRight);
		}
		void HandleError(Model.ModelErrorType errorType) {
			ApiErrorHandler.Instance.HandleError(new Model.ModelErrorInfo(errorType));
		}
		#endregion
	}
	#endregion
}
