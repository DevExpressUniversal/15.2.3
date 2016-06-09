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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartSeriesRefererenceAccessorType
	public enum ChartSeriesRefererenceAccessorType {
		Values,
		Arguments,
		BubbleSize
	}
	#endregion
	#region IChartSeriesRefererenceAccessor
	public interface IChartSeriesRefererenceAccessor {
		void SetReference(ISeries series, IDataReference value);
	}
	#endregion
	#region ChartSeriesValuesAccessor
	public class ChartSeriesValuesAccessor : IChartSeriesRefererenceAccessor {
		public void SetReference(ISeries series, IDataReference value) {
			series.Values = value;
		}
	}
	#endregion
	#region ChartSeriesArgumentsAccessor
	public class ChartSeriesArgumentsAccessor : IChartSeriesRefererenceAccessor {
		public void SetReference(ISeries series, IDataReference value) {
			series.Arguments = value;
		}
	}
	#endregion
	#region ChartSeriesBubbleSizeAccessor
	public class ChartSeriesBubbleSizeAccessor : IChartSeriesRefererenceAccessor {
		public void SetReference(ISeries series, IDataReference value) {
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries != null)
				bubbleSeries.BubbleSize = value;
		}
	}
	#endregion
	#region ModifySeriesRangesCommand
	public class ModifySeriesRangesCommand : ErrorHandledWorksheetCommand {
		#region Static Members
		static Dictionary<ChartSeriesRefererenceAccessorType, IChartSeriesRefererenceAccessor> accessorTable = GetAccessorTable();
		static Dictionary<ChartSeriesRefererenceAccessorType, IChartSeriesRefererenceAccessor> GetAccessorTable() {
			Dictionary<ChartSeriesRefererenceAccessorType, IChartSeriesRefererenceAccessor> result = new Dictionary<ChartSeriesRefererenceAccessorType, IChartSeriesRefererenceAccessor>();
			result.Add(ChartSeriesRefererenceAccessorType.Arguments, new ChartSeriesArgumentsAccessor());
			result.Add(ChartSeriesRefererenceAccessorType.Values, new ChartSeriesValuesAccessor());
			result.Add(ChartSeriesRefererenceAccessorType.BubbleSize, new ChartSeriesBubbleSizeAccessor());
			return result;
		}
		static IChartSeriesRefererenceAccessor GetAccessor(ChartSeriesRefererenceAccessorType type) {
			return accessorTable[type];
		}
		#endregion
		#region Fields
		readonly IChartSeriesRefererenceAccessor accessor;
		readonly ISeries series;
		readonly IDataReference reference;
		#endregion
		public ModifySeriesRangesCommand(IErrorHandler errorHandler, ISeries series, ChartSeriesRefererenceAccessorType accessorType, IDataReference reference)
			: base(series.View.Parent.DocumentModel, errorHandler) {
			this.series = series;
			this.reference = reference;
			this.accessor = GetAccessor(accessorType);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				accessor.SetReference(series, reference);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#region Validate
		protected internal override bool Validate() {
			if (reference == DataReference.Empty)
				return true;
			IModelErrorInfo error = ValidateReference(reference);
			return HandleError(error);
		}
		IModelErrorInfo ValidateReference(IDataReference reference) {
			if (reference == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			ChartDataReference chartDataReference = reference as ChartDataReference;
			if (chartDataReference != null)
				return ValidateVariantValue(chartDataReference.CachedValue);
			return null;
		}
		IModelErrorInfo ValidateVariantValue(VariantValue cachedValue) { 
			if (cachedValue.IsArray)
				return ValidateArray(cachedValue.ArrayValue);
			return ValidateRange(cachedValue.CellRangeValue);
		}
		IModelErrorInfo ValidateArray(IVariantArray array) {
			Guard.ArgumentNotNull(array, "array");
			if (array.Height == 1 || array.Width == 1)
				return null;
			return new ModelErrorInfo(ModelErrorType.InvalidReference);
		}
		IModelErrorInfo ValidateRange(CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			if (range.RangeType == CellRangeType.UnionRange)
				return new ModelErrorInfo(ModelErrorType.UnionRangeNotAllowed);
			if (range.Height == 1 || range.Width == 1)
				return null;
			return new ModelErrorInfo(ModelErrorType.InvalidReference);
		}
		#endregion
	}
	#endregion
}
