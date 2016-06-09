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
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed class SelectedModelToIndicatorColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartIndicatorModel indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.Brush;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToIndicatorThicknessConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return 0;
			else
				return indicatorModel.Thickness;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			BarThicknessEditViewModel.Item item = (BarThicknessEditViewModel.Item)value;
			return item.Thickness;
		}
	}
	public sealed class SelectedModelToIndicatorLegendTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.LegendText;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToRegressionLineOrMovingAverageValueLevelConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.ValueLevel;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFinIndicatorNumericalArgument1Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is WpfChartIndicatorModel))
				return null;
			var finIndicatorModel = (WpfChartIndicatorModel)value;
			var command = (ChangeFinancialIndicatorArgument1Command)parameter;
			object result = finIndicatorModel.Argument1;
			if (result == null)
				return null;
			if (command.ScaleType == ScaleType.Numerical) {
				double d;
				try { d = System.Convert.ToDouble(result); }
				catch { d = 0.0; }
				return d;
			}
			else if (command.ScaleType == ScaleType.DateTime){
				DateTime d;
				try { d = System.Convert.ToDateTime(result); }
				catch { d = DateTime.Now; }
				return d;
			}
			else 
				return System.Convert.ToString(result);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFinIndicatorNumericalArgument2Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is WpfChartIndicatorModel))
				return null;
			var finIndicatorModel = (WpfChartIndicatorModel)value;
			var command = (ChangeFinancialIndicatorArgument2Command)parameter;
			object result = finIndicatorModel.Argument2;
			if (result == null)
				return null;
			if (command.ScaleType == ScaleType.Numerical) {
				double d;
				try { d = System.Convert.ToDouble(result); }
				catch (FormatException) { d = 0.0; }
				return d;
			}
			else if (command.ScaleType == ScaleType.DateTime) {
				DateTime d;
				try { d = System.Convert.ToDateTime(result); }
				catch (FormatException) { d = DateTime.Now; }
				return d;
			}
			else
				return System.Convert.ToString(result);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToValueLevel1Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.ValueLevel1;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToValueLevel2Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.ValueLevel2;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToTrenLineExtrapolateToInfinityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ExtrapolateToInfinity;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFibonacciShowLevel23_6Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ShowLevel23_6;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFibonacciShowLevel76_4Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ShowLevel76_4;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFibonacciArcsShowLevel100Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ShowLevel100;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFibonacciFansShowLevel0Converter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ShowLevel0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFibonacciRetracementShowAdditionalLevelsConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.ShowAdditionalLevels;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToMovingAveragePointsCountConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.PointsCount;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToMovingAverageEnvelopePercentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			else
				return indicatorModel.EnvelopePercent;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedViewModelToBoolMovingAverageKindConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return false;
			var command = (ChangeMovingAverageKindComand)parameter;
			return command.MovingAverageKind == indicatorModel.MovingAverageKind;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
}
