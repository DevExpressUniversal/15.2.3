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
namespace DevExpress.Charts.Native {
	public abstract class MeasureUnitsCalculatorBase {
		public static double CalculateAutoMeasureUnit(double wholeRange, double visualRange, double visualRangeLength, int pixelsPerUnit) {
			if (GeometricUtils.IsValidDouble(wholeRange) && GeometricUtils.IsValidDouble(visualRange) && visualRangeLength > 0 && pixelsPerUnit > 0 && wholeRange != 0 && visualRange != 0) {
				double wholeLength = (visualRangeLength * wholeRange) / visualRange;
				double unitsCount = Math.Max(1, Math.Floor(wholeLength / pixelsPerUnit));
				return wholeRange / unitsCount;
			}
			return double.NaN;
		}
		public abstract bool UpdateAutomaticMeasureUnit(double axisLength, IEnumerable<ISeries> seriesList);
	}
	public abstract class MeasureUnitsCalculatorBase<TMeasureUnit, TGridAlignment> : MeasureUnitsCalculatorBase where TMeasureUnit : struct {
		class MeasureUnitCalculationInfo {
			public double MeasureUnit { get; private set; }
			public double MinMeasureUnit { get; private set; }
			public double WholeRange { get; private set; }
			public MeasureUnitCalculationInfo(double measureUnit, double minMeasureUnit, double wholeRange) {
				MeasureUnit = measureUnit;
				MinMeasureUnit = minMeasureUnit;
				WholeRange = wholeRange;
			}
		}
		readonly IAxisData axis;
		readonly UnitSelector<TMeasureUnit, TGridAlignment> unitSelector;
		double minMeasureUnit;
		double currentMeasureUnit;
		protected IAxisData Axis { get { return axis; } }
		protected UnitSelector<TMeasureUnit, TGridAlignment> UnitSelector { get { return unitSelector; } }
		protected double CurrentMeasureUnit { get { return currentMeasureUnit; } }
		protected abstract IScaleOptionsBase<TMeasureUnit> ScaleOptions { get; }
		protected abstract bool UseMinMeasureUnit { get; }
		public double MinMeasureUnit { get { return minMeasureUnit; } }
		public IList<TMeasureUnit> MeasureUnitsList { get { return unitSelector.SelectActiveMeasureUnits(); } }
		public MeasureUnitsCalculatorBase(IAxisData axis) {
			this.axis = axis;
			this.minMeasureUnit = double.NaN;
			this.unitSelector = CreateUnitSelector();
		}
		int GetPixelsPerUnit(IEnumerable<ISeries> seriesList) {
			int pixelsPerUnit = 0;
			foreach (ISeries series in seriesList) {
				IXYSeriesView view = series.SeriesView as IXYSeriesView;
				if (view != null && (pixelsPerUnit == 0 || pixelsPerUnit < view.PixelsPerArgument))
					pixelsPerUnit = view.PixelsPerArgument;
			}
			return pixelsPerUnit;
		}
		MeasureUnitCalculationInfo CalculateMeasureUnitInfo(double axisLength, int pixelsPerUnit, IEnumerable<ISeries> seriesList) {
			int pointsOnRange = 0;
			foreach (ISeries series in seriesList) {
				if (series.ActualPoints.Count > pointsOnRange)
					pointsOnRange = series.ActualPoints.Count;
			}
			double wholeRange = axis.WholeRange.RefinedMax - axis.WholeRange.RefinedMin;
			double visualRange = axis.VisualRange.Auto ? wholeRange : axis.VisualRange.RefinedMax - axis.VisualRange.RefinedMin;
			if (visualRange > wholeRange)
				wholeRange = visualRange;
			if (wholeRange != 0 && visualRange != 0) {
				double measureUnit = CalculateAutoMeasureUnit(wholeRange, visualRange, axisLength, pixelsPerUnit);
				if (GeometricUtils.IsValidDouble(measureUnit) && measureUnit >= 0 && GeometricUtils.IsValidDouble(wholeRange) && wholeRange > 0) {
					minMeasureUnit = UseMinMeasureUnit && pointsOnRange > 1 ? wholeRange / (pointsOnRange - 1) : measureUnit;
					return new MeasureUnitCalculationInfo(measureUnit, minMeasureUnit, wholeRange);
				}
			}
			return null;
		}
		TMeasureUnit? CalculateAutoMeasureUnit(IEnumerable<ISeries> seriesList, double axisLength, int pixelsPerUnit) {
			if (pixelsPerUnit > 0) {
				MeasureUnitCalculationInfo measureUnitInfo = CalculateMeasureUnitInfo(axisLength, pixelsPerUnit, seriesList);
				if (measureUnitInfo != null) {
					minMeasureUnit = measureUnitInfo.MinMeasureUnit;
					double measureUnit = measureUnitInfo.MeasureUnit;
					unitSelector.UpdateActiveUnits(minMeasureUnit, measureUnitInfo.WholeRange);
					TMeasureUnit selectedMeasureUnit = unitSelector.SelectMeasureUnit(Math.Max(measureUnit, minMeasureUnit), currentMeasureUnit);
					currentMeasureUnit = measureUnit;
					return selectedMeasureUnit;
				}
			}
			return null;
		}
		TMeasureUnit CalculateCustomMeasureUnit(IEnumerable<ISeries> seriesList, double axisLength, int pixelsPerUnit) {
			IAxisRangeData visualRange = axis.VisualRange;
			IAxisRangeData wholeRange = axis.WholeRange;
			return ScaleOptions.CalculateCustomMeasureUnit(seriesList, axisLength, pixelsPerUnit, visualRange.RefinedMin, visualRange.RefinedMax, wholeRange.RefinedMin, wholeRange.RefinedMax);
		}
		protected abstract bool UpdateAutomaticUnits(TMeasureUnit measureUnit, int pixelsPerUnit);
		protected abstract void UpdateAutomaticGrid(TMeasureUnit measureUnit, double pixelsPerUnit);
		protected abstract UnitSelector<TMeasureUnit, TGridAlignment> CreateUnitSelector();
		public override bool UpdateAutomaticMeasureUnit(double axisLength, IEnumerable<ISeries> seriesList) {
			int pixelsPerUnit = GetPixelsPerUnit(seriesList);
			TMeasureUnit? measureUnit;
			if (ScaleOptions.UseCustomMeasureUnit) {
				measureUnit = CalculateCustomMeasureUnit(seriesList, axisLength, pixelsPerUnit);
				currentMeasureUnit = Math.Min(currentMeasureUnit, 1.0);
			}
			else
				measureUnit = CalculateAutoMeasureUnit(seriesList, axisLength, pixelsPerUnit);
			return measureUnit.HasValue && UpdateAutomaticUnits(measureUnit.Value, pixelsPerUnit);
		}
		public void UpdateAutomaticGrid(double axisLength, IEnumerable<ISeries> seriesList) {
			int pixelsPerUnit = GetPixelsPerUnit(seriesList);
			TMeasureUnit? measureUnit;
			if (ScaleOptions.UseCustomMeasureUnit)
				measureUnit = CalculateCustomMeasureUnit(seriesList, axisLength, pixelsPerUnit);
			else
				measureUnit = CalculateAutoMeasureUnit(seriesList, axisLength, pixelsPerUnit);
			if (measureUnit.HasValue)
				UpdateAutomaticGrid(measureUnit.Value, pixelsPerUnit);
		}
	}
}
