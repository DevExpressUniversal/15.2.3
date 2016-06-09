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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core {
	public enum BestFitMode { Default, AllRows, VisibleRows, DistinctValues, Smart } ;	
}
namespace DevExpress.Xpf.Core.Native {
	public interface IBestFitColumn {
		BestFitMode BestFitMode { get; }
		int BestFitMaxRowCount { get; }
	}
	public abstract class BestFitCalculatorBase {
		public const int DefaultBestFitMaxRowCount = -1;
		#region inner classes
		protected delegate void CalcBestFitDelegate(FrameworkElement bestFitControl, IBestFitColumn column, ref double result);
		protected abstract class RowsBestFitCalculatorBase {
			readonly BestFitCalculatorBase owner;
			readonly IEnumerable<int> rows;
			public RowsBestFitCalculatorBase(BestFitCalculatorBase owner, IEnumerable<int> rows) {
				this.owner = owner;
				this.rows = rows;
			}
			protected BestFitCalculatorBase Owner { get { return owner; } }
			protected IEnumerable<int> Rows { get { return rows; } }
			public virtual void CalcRowsBestFit(FrameworkElement bestFitControl, IBestFitColumn column, ref double result) {
				foreach(int rowHandle in rows) {
					if(!IsValidRowHandle(rowHandle)) continue;
					UpdateBestFitControl(bestFitControl, column, rowHandle);
					owner.UpdateBestFitResult(bestFitControl, ref result);
				}
			}
			protected abstract void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, int rowHandle);
			protected virtual bool IsFocusedCell(IBestFitColumn column, int rowHandle) { return false; }
			protected virtual bool IsValidRowHandle(int rowHandle) { return true; }
		}
		#endregion
		public const int SmartModeRowCountThreshold = 3000;
		protected abstract object[] GetUniqueValues(IBestFitColumn column);
		public virtual double CalcColumnBestFitWidth(IBestFitColumn column) {
			try {
				return Math.Ceiling(CalcColumnBestFitWidthCore(column));
			} finally {
				SetBestFitElement(null);
			}
		}
		protected virtual double CalcColumnBestFitWidthCore(IBestFitColumn column) {
			double result = 0;
			CalcDataBestFit(column, ref result);
			return result;
		}
		protected abstract int GetRowCount(IBestFitColumn column);
		protected virtual int VisibleRowCount { get { return 30; } }
		protected virtual bool IsServerMode { get { return false; } }
		protected virtual int GetBestFitMaxRowCount(IBestFitColumn column) {
			return column.BestFitMaxRowCount;
		}
		protected virtual RowsRange CalcBestFitRowsRange(int rowCount) {
			return new RowsRange(0, rowCount);
		}
		protected abstract void SetBestFitElement(FrameworkElement bestFitElement);		   
		protected abstract FrameworkElement CreateBestFitControl(IBestFitColumn column);
		protected abstract RowsBestFitCalculatorBase CreateBestFitCalculator(IEnumerable<int> rows);
		protected abstract void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, object cellValue);
		protected virtual BestFitMode GetBestFitMode(IBestFitColumn column) { return column.BestFitMode; }
		protected void CalcDataBestFit(IBestFitColumn column, ref double result) {
			FrameworkElement bestFitControl = CreateBestFitControl(column);
			SetBestFitElement(bestFitControl);
			BestFitMode bestFitMode = GetBestFitMode(column);
			GetCalcBestFitDelegate(bestFitMode, column)(bestFitControl, column, ref result);
		}
		protected virtual CalcBestFitDelegate GetCalcBestFitDelegate(BestFitMode bestFitMode, IBestFitColumn column) {
			switch(bestFitMode) {
				case BestFitMode.Default:
				case BestFitMode.Smart:
					return GetSmartModeCalcBestFitDelegate(column);
				case BestFitMode.AllRows:
					return CreateBestFitCalculator(CalcBestFitRowsRange(column)).CalcRowsBestFit;
				case BestFitMode.DistinctValues:
					return CalcDistinctValuesBestFit;
				case BestFitMode.VisibleRows:
					return CreateBestFitCalculator(CalcBestFitRowsRange(VisibleRowCount)).CalcRowsBestFit;
				default:
					throw new Exception();
			}
		}
		protected virtual CalcBestFitDelegate GetSmartModeCalcBestFitDelegate(IBestFitColumn column) {
			BestFitMode smartBestFitMode = GetSmartBestFitMode(column);
			return GetCalcBestFitDelegate(smartBestFitMode, column);
		}
		protected BestFitMode GetSmartBestFitMode(IBestFitColumn column) {
			RowsRange allRowsRange = CalcBestFitRowsRange(column);
			if(allRowsRange.RowCount < SmartModeRowCountThreshold)
				return BestFitMode.AllRows;
			else
				return BestFitMode.DistinctValues;
		}
		protected virtual void CalcDistinctValuesBestFit(FrameworkElement bestFitControl, IBestFitColumn column, ref double result) {
			object[] values = GetUniqueValues(column);
			if(values == null)
				return;
			foreach(object cellValue in values) {
				UpdateBestFitControl(bestFitControl, column, cellValue);
				UpdateBestFitResult(bestFitControl, ref result);
			}
		}		
		public RowsRange CalcBestFitRowsRange(IBestFitColumn column) {
			int rowCount = GetRowCount(column);
			int bestFitMaxRowCount = GetBestFitMaxRowCount(column);
			if(bestFitMaxRowCount != -1)
				rowCount = Math.Min(bestFitMaxRowCount, rowCount);
			if(IsServerMode)
				rowCount = VisibleRowCount;
			return CalcBestFitRowsRange(rowCount);
		}		
		protected void UpdateBestFitResult(FrameworkElement bestFitElement, ref double result) {
			UpdateBestFitResult(bestFitElement, ref result, 0);
		}
		protected void UpdateBestFitResult(FrameworkElement bestFitElement, ref double result, double correction) {
			DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.DoLockedAction(bestFitElement.UpdateLayout);
			double desigredSize = GetDesiredSize(bestFitElement) - correction;
			desigredSize = Math.Min(10000, desigredSize);
			result = Math.Max(result, desigredSize);
		}
		protected virtual double GetDesiredSize(FrameworkElement bestFitElement) {
			double result = bestFitElement.DesiredSize.Width;
#if !SL
			if(bestFitElement.UseLayoutRounding)
				result++;
#endif
			return result;
		}
	}
	public class RowsRange : IEnumerable<int> {
		public RowsRange(int topRowHandle, int rowCount) {
			this.TopRowHandle = topRowHandle;
			this.RowCount = rowCount;
		}
		public int TopRowHandle { get; private set; }
		public int RowCount { get; private set; }
		#region IEnumerable<int> Members
		IEnumerator<int> IEnumerable<int>.GetEnumerator() {
			for(int rowHandle = TopRowHandle; rowHandle < TopRowHandle + RowCount; rowHandle++) {
				yield return rowHandle;
			}
		}
		IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<int>)this).GetEnumerator();
		}
		#endregion
	}	
}
