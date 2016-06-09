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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class LayoutSettingsControl : ChartUserControl {
		struct SeriesLabelTextOrientationItem {
			readonly TextOrientation textOrientation;
			readonly string text;
			public TextOrientation TextOrientation { get { return textOrientation; } }
			public SeriesLabelTextOrientationItem(TextOrientation textOrientation) {
				this.textOrientation = textOrientation;
				switch (textOrientation) {
					case TextOrientation.Horizontal:
						text = ChartLocalizer.GetString(ChartStringId.WizSeriesLabelTextOrientationHorizontal);
						break;
					case TextOrientation.TopToBottom:
						text = ChartLocalizer.GetString(ChartStringId.WizSeriesLabelTextOrientationTopToBottom);
						break;
					case TextOrientation.BottomToTop:
						text = ChartLocalizer.GetString(ChartStringId.WizSeriesLabelTextOrientationBottomToTop);
						break;
					default:
						ChartDebug.Fail("Unknown series label text orientation.");
						text = ChartLocalizer.GetString(ChartStringId.WizSeriesLabelTextOrientationHorizontal);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is SeriesLabelTextOrientationItem) && textOrientation == ((SeriesLabelTextOrientationItem)obj).textOrientation;
			}
			public override int GetHashCode() {
				return textOrientation.GetHashCode();
			}
		}
		SeriesLabelBase label;
		SeriesBase series;
		Chart chart;
		MethodInvoker updateMethod;
		ChartUserControl optionsControl;
		ILabelBehaviorProvider LabelBehaviorProvider { get { return label; } }
		public LayoutSettingsControl() {
			InitializeComponent();
		}
		public void Initialize(SeriesLabelBase label, SeriesBase series, Chart chart, MethodInvoker updateMethod) {
			this.label = label;
			this.series = series;
			this.chart = chart;
			this.updateMethod = updateMethod;
			if (LabelBehaviorProvider.VerticalRotationSupported) {
				cbTextDirection.Properties.Items.Add(new SeriesLabelTextOrientationItem(TextOrientation.Horizontal));
				cbTextDirection.Properties.Items.Add(new SeriesLabelTextOrientationItem(TextOrientation.TopToBottom));
				cbTextDirection.Properties.Items.Add(new SeriesLabelTextOrientationItem(TextOrientation.BottomToTop));
				cbTextDirection.SelectedItem = new SeriesLabelTextOrientationItem(label.TextOrientation);
			}
			else {
				pnTextDirection.Visible = false;
				separator.Visible = false;
			}
			optionsControl = CreateOptionsControl();
			if (optionsControl != null) {
				optionsControl.Dock = DockStyle.Fill;
				pnOptions.Controls.Add(optionsControl);
			}
			else
				pnOptions.Visible = false;
		}
		ChartUserControl CreateOptionsControl() {
			BarSeriesLabel barLabel = label as BarSeriesLabel;
			if (barLabel != null) {
				BarSeriesLabelOptionsControl barControl = new BarSeriesLabelOptionsControl();
				barControl.Initialize(barLabel, UpdateControls);
				return barControl;
			}
			RangeAreaSeriesLabel rangeAreaLabel = label as RangeAreaSeriesLabel;
			if (rangeAreaLabel != null) {
				RangeAreaSeriesLabelOptionsControl rangeAreaControl = new RangeAreaSeriesLabelOptionsControl();
				rangeAreaControl.Initialize(rangeAreaLabel, UpdateControls);
				return rangeAreaControl;
			}
			PointSeriesLabel pointLabel = label as PointSeriesLabel;
			if (pointLabel != null) {
				PointSeriesLabelOptionsControl pointControl = new PointSeriesLabelOptionsControl();
				pointControl.Initialize(pointLabel, UpdateControls);
				return pointControl;
			}
			PieSeriesLabel pieLabel = label as PieSeriesLabel;
			if (pieLabel != null) {
				PieSeriesLabelOptionsControl pieControl = new PieSeriesLabelOptionsControl();
				pieControl.Initialize(pieLabel, series, chart, UpdateControls);
				return pieControl;
			}
			FunnelSeriesLabel funnelLabel = label as FunnelSeriesLabel;
			if (funnelLabel != null) {
				FunnelSeriesLabelOptionsControl funnelControl = new FunnelSeriesLabelOptionsControl();
				funnelControl.Initialize(funnelLabel, UpdateControls);
				return funnelControl;
			}
			RangeBarSeriesLabel rangeBarLabel = label as RangeBarSeriesLabel;
			if (rangeBarLabel != null) {
				RangeBarLabelSettings rangeBarControl = new RangeBarLabelSettings();
				rangeBarControl.Initialize(rangeBarLabel);
				return rangeBarControl;
			}
			return null;
		}
		void UpdateControls() {
			if(updateMethod != null)
				updateMethod();
		}
		void cbTextDirection_SelectedIndexChanged(object sender, EventArgs e) {
			label.TextOrientation = ((SeriesLabelTextOrientationItem)cbTextDirection.SelectedItem).TextOrientation;
			UpdateControls();
		}
	}
}
