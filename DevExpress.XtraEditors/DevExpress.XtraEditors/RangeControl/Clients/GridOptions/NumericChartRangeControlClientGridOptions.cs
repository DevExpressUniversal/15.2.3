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
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Sparkline.Core;
namespace DevExpress.XtraEditors {
	public sealed class NumericChartRangeControlClientGridOptions : ChartRangeControlClientGridOptions {
		#region Nested Classes : NumericLabelFormatter, GridMapping
		sealed class NumericLabelFormatter : IFormatProvider, ICustomFormatter {
			#region IFormatProvider
			object IFormatProvider.GetFormat(Type formatType) {
				if (formatType == typeof(ICustomFormatter))
					return this;
				return null;
			}
			#endregion
			#region ICustomFormatter
			string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider) {
				if (!Equals(formatProvider))
					return null;
				if (string.IsNullOrWhiteSpace(format)) {
					double value = Convert.ToDouble(arg);
					string autoFormat = "G";
					string suffix = "";
					if ((value >= 1000) && (value < 999999)) {
						autoFormat = "F1";
						suffix = "K";
						value /= 1000;
					} else if ((value >= 1000000) && (value < 999999999)) {
						autoFormat = "F1";
						suffix = "M";
						value /= 1000000;
					} else if (value >= 1000000000) {
						autoFormat = "F1";
						suffix = "B";
						value /= 1000000000;
					}
					return value.ToString(autoFormat, CultureInfo.InvariantCulture) + suffix;
				}
				try {
					return String.Format("{0:" + format + "}", arg);
				} catch {
					return String.Format("{0}", arg);
				}
			}
			#endregion
			public override string ToString() {
				return "(NumericLabelFormatter)";
			}
		}
		sealed class NumericGridMapping : IChartCoreClientGridMapping {
			GridUnit selectedUnit;
			public GridUnit SelectedUnit {
				get { return selectedUnit; }
			}
			#region IChartCoreClientGridMapping
			double IChartCoreClientGridMapping.CeilValue(GridUnit unit, double value) {
				return Math.Ceiling(value / unit.Step);
			}
			double IChartCoreClientGridMapping.FloorValue(GridUnit unit, double value) {
				return Math.Floor(value / unit.Step);
			}
			double IChartCoreClientGridMapping.GetGridValue(GridUnit unit, double index) {
				return index * unit.Step;
			}
			GridUnit IChartCoreClientGridMapping.SelectGridUnit(double unit) {
				double log;
				double logBase;
				if (unit < 1) {
					logBase = 0.1;
					log = Math.Ceiling(Math.Log(unit, logBase));
				} else {
					logBase = 10;
					log = Math.Floor(Math.Log(unit, logBase));
				}
				double autoUnit;
				if (SparklineMathUtils.AreDoublesEqual(0.0, log))
					autoUnit = (unit < 1) ? 0.1 : 1.0;
				else
					autoUnit = Math.Pow(logBase, log);
				double autoSpacing = Math.Ceiling(unit / autoUnit);
				selectedUnit = null;
				if (autoSpacing >= 5)
					selectedUnit = new GridUnit(autoUnit * 10, 1);
				else
					selectedUnit = new GridUnit(autoUnit, autoSpacing);
				return selectedUnit;
			}
			#endregion
		}
		#endregion
		NumericLabelFormatter labelFormatter;
		NumericGridMapping gridMapping;
		protected override GridUnit CoreGridUnit {
			get { return new GridUnit(1, GridSpacing); }
		}
		protected override GridUnit CoreSnapUnit {
			get {
				if (Auto && (gridMapping.SelectedUnit != null))
					return gridMapping.SelectedUnit;
				return new GridUnit(1, SnapSpacing); 
			}
		}
		protected override double PixelPerUnit {
			get { return 100; }
		}
		protected override IChartCoreClientGridMapping GridMapping {
			get { return gridMapping; }
		}
		protected override IFormatProvider LabelFormatProviderInternal {
			get { return labelFormatter; }
		}
		public NumericChartRangeControlClientGridOptions() {
			this.gridMapping = new NumericGridMapping();
			this.labelFormatter = new NumericLabelFormatter();
		}
		protected override void PushAutoUnitToProperties() {
			GridUnit selectedUnit = gridMapping.SelectedUnit;
			if (selectedUnit != null) {
				GridSpacing = selectedUnit.Step;
				SnapSpacing = selectedUnit.Step;
			} else {
				GridSpacing = 1.0;
				SnapSpacing = 1.0;
			}
		}
		protected override bool ValidateLabelFormat(string format) {
			if (!string.IsNullOrEmpty(format)) {
				double anyDoubleValue = 10.0d;
				try {
					anyDoubleValue.ToString(format);
				} catch {
					return false;
				}
			}
			return true;
		}
		protected internal override object GetNativeGridValue(double value) {
			return value;
		}
	}
}
