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

using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using System;
using System.ComponentModel;
namespace DevExpress.XtraCharts {
	public class ChartRangeControlClientNumericGridOptions : ChartRangeControlClientGridOptions {
		const double minMeasureUnitMagnify = 0.001;
		INumericScaleOptions ScaleOptions { get { return Axis.NumericScaleOptions; } }
		protected override double ChartGridSpacing { get { return ScaleOptions.GridSpacing * ScaleOptions.GridAlignment; } }
		protected override double ChartSnapSpacing {
			get {
				double measureUnit = ScaleOptions.MeasureUnit;
				return double.IsNaN(measureUnit) ? 1.0 : measureUnit;
			}
		}
		protected internal override RangeControlGridUnit GridUnit {
			get { return GetRangeControlUnit(GridSpacing, GridOffset); }
		}
		protected internal override RangeControlGridUnit SnapUnit {
			get { return GetRangeControlUnit(ValidateSpacing(SnapSpacing), SnapOffset); }
		}
		public ChartRangeControlClientNumericGridOptions(ChartElement owner) : base (owner) {
		}
		double ValidateSpacing(double spacing) {
			Axis.UpdateAutoMeasureUnit(false);
			NumericMeasureUnitsCalculatorCore calculator = Axis.NumericScaleOptions.NumericMeasureUnitsCalculatorCore;
			if ((calculator != null) && !double.IsNaN(calculator.MinMeasureUnit))
				return Math.Max(spacing, calculator.MinMeasureUnit * minMeasureUnitMagnify);
			return spacing;
		}
		RangeControlGridUnit GetRangeControlUnit(double spacing, double offset) {
			return new RangeControlGridUnit(spacing, spacing, offset);
		}
		protected override RangeControlClientGridMapping CreateGridMapping() {
			return new NumericRangeControlClientGridMapping();
		}
		protected override IFormatProvider CreateDefaultLabelFormatProvider() {
			return new NumericLabelFormatter();
		}
		protected internal override void UpdateAutoGridSettings(double refinedWholeRange, double scaleLengthInPixels) {
			double gridSpacing = GridSpacingCalculator.Calculate(refinedWholeRange, scaleLengthInPixels, Constants.AxisXGridSpacingFactor * 2);
			UpdateAutoSpacing(gridSpacing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartRangeControlClientNumericGridOptions(null);
		}
	}
}
