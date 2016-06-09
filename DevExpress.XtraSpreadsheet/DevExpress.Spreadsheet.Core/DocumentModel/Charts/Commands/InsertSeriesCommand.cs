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

using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region InsertSeriesCommandBase (absract class)
	public abstract class InsertSeriesCommandBase : ErrorHandledWorksheetCommand {
		readonly IChart chart;
		protected InsertSeriesCommandBase(IErrorHandler errorHandler, IChart chart)
			: base(chart.DocumentModel, errorHandler) {
			Guard.ArgumentNotNull(chart, "chart");
			this.chart = chart;
		}
		protected IChart Chart { get { return chart; } }
		protected ChartViewCollection Views { get { return chart.Views; } }
		protected IModelErrorInfo ValidateRange(CellRangeBase range) {
			if (range == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			if (range.RangeType == CellRangeType.UnionRange)
				return new ModelErrorInfo(ModelErrorType.UnionRangeNotAllowed);
			if (range.Width != 1 && range.Height != 1)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			return null;
		}
	}
	#endregion
	#region InsertSeriesCommand
	public class InsertSeriesCommand : InsertSeriesCommandBase {
		#region Fields
		readonly IChartView view;
		readonly IDataReference arguments;
		readonly IDataReference values;
		readonly IChartText seriesName;
		#endregion
		public InsertSeriesCommand(IErrorHandler errorHandler, IChartView view, IDataReference arguments, IDataReference values, IChartText seriesName)
			: base(errorHandler, view.Parent) {
			Guard.ArgumentNotNull(arguments, "arguments");
			Guard.ArgumentNotNull(arguments, "values");
			this.view = view;
			this.arguments = arguments;
			this.values = values;
			this.seriesName = seriesName;
		}
		protected internal override void ExecuteCore() {
			ChangeAllSeriesOrdersCommand.Execute(ErrorHandler, view.Parent);
			view.Series.Add(CreateSeries());
		}
		protected internal override bool Validate() {
			if (!view.IsContained)
				return false;
			ChartDataReference valuesReference = values as ChartDataReference;
			if (arguments == null)
				return true;
			VariantValue value = valuesReference.CachedValue;
			if (value.IsCellRange)
				return HandleError(ValidateRange(value.CellRangeValue));
			return true;
		}
		ISeries CreateSeries() {
			int seriesCount = Views.GetSeriesCount();
			if (seriesCount == 0)
				ChartAxisHelper.CheckArgumentAxis(view, arguments);
			ISeries result = view.CreateSeriesInstance();
			result.Index = Views.GetMaxSeriesIndex() + 1;
			result.Order = seriesCount;
			result.Arguments = arguments;
			result.Values = values;
			if (seriesName != null)
				result.Text = seriesName;
			return result;
		}
	}
	#endregion
	#region ChangeSeriesDirectionCommand
	public class ChangeSeriesDirectionCommand : InsertSeriesCommandBase {
		readonly CellRangeBase cellRange;
		public ChangeSeriesDirectionCommand(IErrorHandler errorHandler, IChart chart, CellRangeBase cellRange)
			: base(errorHandler, chart) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			this.cellRange = cellRange;
		}
		protected internal override void ExecuteCore() {
			Chart.SeriesDirection = cellRange.Width == 1 ? ChartViewSeriesDirection.Vertical : ChartViewSeriesDirection.Horizontal;
		}
		protected internal override void BeginExecute() {
		}
		protected internal override void EndExecute() {
		}
		protected internal override bool Validate() {
			if (Views.Count == 0)
				return false;
			return HandleError(ValidateRange(cellRange));
		}
	}
	#endregion
}
