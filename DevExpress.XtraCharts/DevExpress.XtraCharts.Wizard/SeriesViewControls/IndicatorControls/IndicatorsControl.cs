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
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class IndicatorsControl : ChartUserControl {
		readonly Dictionary<Type, IndicatorControlBase> controls = new Dictionary<Type, IndicatorControlBase>();
		Chart chart;
		IndicatorControlBase currentControl;
		static Type GetWizardIndicatorControlType(Type indicatorType) {
			if (indicatorType == typeof(RegressionLine))
				return typeof(SingleLevelIndicatorControl);
			if (indicatorType == typeof(TrendLine))
				return typeof(TrendLineControl);
			if (indicatorType == typeof(FibonacciIndicator))
				return typeof(FibonacciIndicatorControl);
			if (indicatorType == typeof(ExponentialMovingAverage) || indicatorType == typeof(SimpleMovingAverage) ||
				indicatorType == typeof(TriangularMovingAverage) || indicatorType == typeof(WeightedMovingAverage))
				return typeof(MovingAverageControl);
			return null;
		}
		void indicatorsSelectionControl_SelectedElementChanged() {
			SuspendLayout();
			try {
				Indicator indicator = indicatorsSelectionControl.CurrentElement as Indicator;
				if (indicator == null) {
					panelType.Visible = false;
					panelIndicator.Controls.Clear();
					currentControl = null;
				}
				else {
					chart.SelectHitElement(indicator);
					panelType.Visible = true;
					labelIndicatorType.Text = indicator.IndicatorName;
					Type type = GetWizardIndicatorControlType(indicator.GetType());
					if (type != null) {
						IndicatorControlBase control;
						if (!controls.TryGetValue(type, out control)) {
							control = Activator.CreateInstance(type) as IndicatorControlBase;
							if (control != null)
								controls.Add(type, control);
						}
						if (control == null)
							panelIndicator.Controls.Clear();
						else {
							if (!Object.ReferenceEquals(currentControl, control)) {
								if (currentControl != null)
									currentControl.NameChanged -= new MethodInvoker(indicatorsSelectionControl.UpdateList);
								panelIndicator.Controls.Clear();
								panelIndicator.Controls.Add(control);
								control.Dock = DockStyle.Fill;
								control.NameChanged += new MethodInvoker(indicatorsSelectionControl.UpdateList);
								currentControl = control;
							}
							control.Initialize(indicator);
						}
					}
				}
			}
			finally {
				ResumeLayout();
			}
		}
		public IndicatorsControl() {
			InitializeComponent();
		}
		public void Initialize(IndicatorCollection indicators, XYDiagram2DSeriesViewBase view, Chart chart) {
			this.chart = chart;
			indicatorsSelectionControl.Initialize(indicators, view);
		}
		public void SelectIndicator(Indicator indicator) {
			indicatorsSelectionControl.CurrentElement = indicator;
		}
	}
}
