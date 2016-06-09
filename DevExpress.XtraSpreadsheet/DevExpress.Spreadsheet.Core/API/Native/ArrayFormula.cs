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
using DevExpress.Utils;
using DevExpress.Office;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	public interface ArrayFormula {
		Range Range { get; }
		string Formula { get; }
	}
	public interface ArrayFormulaCollection : ISimpleCollection<ArrayFormula> {
		void RemoveAt(int index);
		void Remove(Range range);
		void Remove(ArrayFormula arrayFormula);
		int IndexOf(ArrayFormula arrayFormula);
		void Clear();
		ArrayFormula Add(Range range, string formula);
		bool Contains(ArrayFormula arrayFormula);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	partial class NativeArrayFormula : ArrayFormula {
		NativeRange range;
		bool isValid;
		public NativeArrayFormula(Model.CellRange arrayFormulaRange, NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(arrayFormulaRange, "arrayFormulaRange");
			this.range = new NativeRange(arrayFormulaRange, nativeWorksheet);
		}
		public Range Range { get { return range; } }
		public NativeRange NativeRange { get { return range; } }
		public string Formula {
			get {
				Model.CellRange modelRange = (Model.CellRange)NativeRange.ModelRange;
				Model.ICell hostCell = modelRange.GetFirstCellUnsafe() as Model.ICell;
				return hostCell.FormulaBody;
			}
		}
		protected internal bool IsValid { get { return isValid; } set { isValid = value; } }
	}
	partial class NativeArrayFormulaCollection : NativeCollectionForUndoableCollectionBase<ArrayFormula, NativeArrayFormula, Model.CellRange>, ArrayFormulaCollection {
		public NativeArrayFormulaCollection(NativeWorksheet worksheet)
			: base(worksheet) {
		}
		protected override void ClearModelObjects() {
			ModelWorksheet.ClearArrayFormulas();
		}
		protected override NativeArrayFormula CreateNativeObject(Model.CellRange range) {
			return new NativeArrayFormula(range, Worksheet);
		}
		public override IEnumerable<Model.CellRange> GetModelItemEnumerable() {
			return ModelWorksheet.ArrayFormulaRanges.InnerList;
		}
		public override int ModelCollectionCount {
			get { return ModelWorksheet.ArrayFormulaRanges.InnerList.Count; }
		}
		protected override void RemoveModelObjectAt(int index) {
			ModelWorksheet.RemoveArrayFormulaAt(index);
		}
		public ArrayFormula Add(Range range, string formula) {
			Model.CellRange otherModelRange = Worksheet.GetModelRange(range).GetFirstInnerCellRange();
			NativeRange otherNativeRange = new NativeRange(otherModelRange, Worksheet);
			otherNativeRange.ArrayFormula = formula;
			return InnerList[Count - 1];
		}
		public void Remove(Range range) {
			Model.CellRangeBase modelRange = Worksheet.GetModelRange(range);
			Model.CellRange cellRange = modelRange.GetFirstInnerCellRange();
			ModelWorksheet.RemoveArrayFormulasAndValuesFrom(cellRange);
		}
		protected override void InvalidateItem(NativeArrayFormula item) {
			item.IsValid = false;
		}
		protected override UndoableCollection<Model.CellRange> GetModelCollection() {
			return ModelWorksheet.ArrayFormulaRanges;
		}
	}
}
