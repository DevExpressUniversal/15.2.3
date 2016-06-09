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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class MarkerControl : ChartUserControl {
		Marker marker;
		SeriesViewBase view;
		public MarkerControl() {
			InitializeComponent();
		}
		public void Initialize(Marker marker, SeriesViewBase view) {
			this.marker = marker;
			this.view = view;
			chVisible.Checked = CommonUtils.GetActualMarkerVisible(view, marker);
			clrColor.EditValue = marker.Color;
			markerBaseControl.Initialize(marker);
			UpdateControls();
		}
		void UpdateControls() {
			pnlColor.Enabled = CommonUtils.GetActualMarkerVisible(view, marker);
			markerBaseControl.Enabled = CommonUtils.GetActualMarkerVisible(view, marker);
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			SetMarkerVisibility(chVisible.Checked);
			UpdateControls();
		}
		void clrColor_EditValueChanged(object sender, EventArgs e) {
			marker.Color = (Color)clrColor.EditValue;
		}
		void SetMarkerVisibility(bool visible) {
			RangeAreaSeriesView rangeAreaView = view as RangeAreaSeriesView;
			if (rangeAreaView != null) {
				if (marker == rangeAreaView.Marker1)
					rangeAreaView.Marker1Visibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
				if (marker == rangeAreaView.Marker2)
					rangeAreaView.Marker2Visibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
			}
			else {
				LineSeriesView lineVview = view as LineSeriesView;
				if (lineVview != null)
					lineVview.MarkerVisibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
				else {
					RadarLineSeriesView radarView = view as RadarLineSeriesView;
					if (radarView != null)
						radarView.MarkerVisibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
					else {
						RangeBarSeriesView rangeBarView = view as RangeBarSeriesView;
						if (rangeBarView != null) {
							if (marker == rangeBarView.MinValueMarker)
								rangeBarView.MinValueMarkerVisibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
							if (marker == rangeBarView.MaxValueMarker)
								rangeBarView.MaxValueMarkerVisibility = visible ? DefaultBoolean.True : DefaultBoolean.False;
						}
					}
				}
			}
		}
	}
}
