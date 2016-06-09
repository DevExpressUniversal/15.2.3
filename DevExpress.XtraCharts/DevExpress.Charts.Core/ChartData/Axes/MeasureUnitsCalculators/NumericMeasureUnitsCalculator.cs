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
	public class NumericUnitSelector : UnitSelector<double, double> {
		const int autoMeasureUnitStep = 10;
		public static int AutoMeasureUnitStep { get { return autoMeasureUnitStep; } }
		static double RoundMeasureUnit(double measureUnit) {
			if (measureUnit == 1)
				return GeometricUtils.StrongRound(measureUnit);
			else if (measureUnit > 1) {
				int tens = 0;
				while (measureUnit > 10) {
					measureUnit /= 10;
					tens++;
				}
				return GeometricUtils.StrongRound(measureUnit) * Math.Pow(10, tens);
			}
			else if (measureUnit > 0) {
				int tens = 0;
				while (measureUnit < 1) {
					measureUnit *= 10;
					tens++;
				}
				return (1.0 / Math.Pow(10, tens)) * GeometricUtils.StrongRound(measureUnit);
			}
			throw new ArgumentException("Measure unit must be greater than zero.");
		}
		public override void UpdateActiveUnits(double min, double max) {
			double minUnit = RoundMeasureUnit(min);
			double maxUnit = RoundMeasureUnit(max);
			List<UnitContainer.MeasureItem> activeUnits = new List<UnitContainer.MeasureItem>();
			double measureUnit = minUnit;
			activeUnits.Add(new UnitContainer.MeasureItem(measureUnit, measureUnit));
			while (measureUnit < maxUnit) {
				measureUnit *= AutoMeasureUnitStep;
				activeUnits.Add(new UnitContainer.MeasureItem(measureUnit, measureUnit));
			}
			ActiveUnitContainer.UpdateActiveUnits(activeUnits, null);
		}
	}
	public class NumericMeasureUnitsCalculatorCore : MeasureUnitsCalculatorBase<double, double> {
		INumericScaleOptions NumericScaleOptions { get { return Axis.NumericScaleOptions; } }
		protected override bool UseMinMeasureUnit { get { return true; } }
		protected override IScaleOptionsBase<double> ScaleOptions { get { return NumericScaleOptions; } }
		public NumericMeasureUnitsCalculatorCore(IAxisData axis) : base(axis) {
		}
		protected override UnitSelector<double, double> CreateUnitSelector() {
			return new NumericUnitSelector();
		}
		protected override bool UpdateAutomaticUnits(double measureUnit, int pixelsPerUnit) {
			return NumericScaleOptions.UpdateAutomaticUnits(measureUnit, measureUnit);
		}
		protected override void UpdateAutomaticGrid(double measureUnit, double pixelsPerUnit) {
		}
	}
}
