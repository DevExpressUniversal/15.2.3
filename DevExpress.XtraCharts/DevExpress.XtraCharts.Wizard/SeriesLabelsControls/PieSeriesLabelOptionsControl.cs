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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class PieSeriesLabelOptionsControl : ChartUserControl {
		struct PieSeriesLabelPositionItem {
			readonly PieSeriesLabelPosition position;
			readonly string text;
			public PieSeriesLabelPosition Position { get { return position; } }
			public PieSeriesLabelPositionItem(PieSeriesLabelPosition position) {
				this.position = position;
				switch(position) {
					case PieSeriesLabelPosition.Inside:
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionInside);
						break;
					case PieSeriesLabelPosition.Outside:
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionOutside);
						break;
					case PieSeriesLabelPosition.Radial:
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionRadial);
						break;
					case PieSeriesLabelPosition.Tangent:
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionTangent);
						break;
					case PieSeriesLabelPosition.TwoColumns:
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionTwoColumns);
						break;
					default:
						ChartDebug.Fail("Unknown pie series label position.");
						text = ChartLocalizer.GetString(ChartStringId.WizPieSeriesLabelPositionOutside);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is PieSeriesLabelPositionItem) && position == ((PieSeriesLabelPositionItem)obj).position;
			}
			public override int GetHashCode() {
				return position.GetHashCode();
			}
		}
		PieSeriesLabel label;
		SeriesBase series;
		Chart chart;
		MethodInvoker updateMethod;
		public PieSeriesLabelOptionsControl() {
			InitializeComponent();
		}
		void UpdateControls() {
			pnlIndent.Enabled = label.Position == PieSeriesLabelPosition.TwoColumns;
			pnlPosition.Enabled = !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, series);
			if(updateMethod != null)
				updateMethod();
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			label.Position = ((PieSeriesLabelPositionItem)cbPosition.SelectedItem).Position;
			UpdateControls();
		}
		void spnIndent_EditValueChanged(object sender, EventArgs e) {
			label.ColumnIndent = Convert.ToInt32(spnIndent.EditValue);
		}
		public void Initialize(PieSeriesLabel label, SeriesBase series, Chart chart, MethodInvoker updateMethod) {
			this.label = label;
			this.series = series;
			this.chart = chart;
			this.updateMethod = updateMethod;
			INestedDoughnutSeriesView nestedView = series.View as INestedDoughnutSeriesView;
			bool supportsOutsidePosition = nestedView == null || nestedView.IsOutside.Value;
			cbPosition.Properties.Items.Add(new PieSeriesLabelPositionItem(PieSeriesLabelPosition.Inside));
			if (supportsOutsidePosition) {
				cbPosition.Properties.Items.Add(new PieSeriesLabelPositionItem(PieSeriesLabelPosition.Outside));
				cbPosition.Properties.Items.Add(new PieSeriesLabelPositionItem(PieSeriesLabelPosition.TwoColumns));
			}
			cbPosition.Properties.Items.Add(new PieSeriesLabelPositionItem(PieSeriesLabelPosition.Radial));
			cbPosition.Properties.Items.Add(new PieSeriesLabelPositionItem(PieSeriesLabelPosition.Tangent));
			cbPosition.SelectedItem = new PieSeriesLabelPositionItem(label.Position);
			spnIndent.EditValue = label.ColumnIndent;
			pnlIndent.Visible = supportsOutsidePosition;
			sepIndent.Visible = supportsOutsidePosition;
			UpdateControls();
		}
	}
}
