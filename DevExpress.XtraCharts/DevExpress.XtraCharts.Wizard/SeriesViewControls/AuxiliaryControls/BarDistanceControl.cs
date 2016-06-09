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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class BarDistanceControl : ChartUserControl {
		BarSeriesView view;
		public BarDistanceControl() {
			InitializeComponent();
		}
		public void Initialize(BarSeriesView view) {
			this.view = view;
			txtWidth.EditValue = view.BarWidth;
			ISideBySideBarSeriesView sideBySideView = view as ISideBySideBarSeriesView;
			if (sideBySideView == null) {
				panelEqualBarWidth.Visible = false;
				panelDistance.Visible = false;
				panelDistanceFixed.Visible = false;
			}
			else {
				panelEqualBarWidth.Visible = true;
				panelDistance.Visible = true;
				panelDistanceFixed.Visible = true;
				chEqualBarWidth.Checked = sideBySideView.EqualBarWidth;
				txtDistance.EditValue = sideBySideView.BarDistance;
				txtDistanceFixed.EditValue = sideBySideView.BarDistanceFixed;
			}
		}
		void txtWidth_EditValueChanged(object sender, EventArgs e) {
			view.BarWidth = Convert.ToDouble(txtWidth.EditValue);
		}
		void chEqualBarWidth_CheckedChanged(object sender, EventArgs e) {
			ISideBySideBarSeriesView sideBySideView = view as ISideBySideBarSeriesView;
			if (sideBySideView != null)
				sideBySideView.EqualBarWidth = chEqualBarWidth.Checked;
		}
		void txtDistance_EditValueChanged(object sender, EventArgs e) {
			ISideBySideBarSeriesView sideBySideView = view as ISideBySideBarSeriesView;
			if (sideBySideView != null)
				sideBySideView.BarDistance = Convert.ToDouble(txtDistance.EditValue);
		}
		void txtDistanceFixed_EditValueChanged(object sender, EventArgs e) {
			ISideBySideBarSeriesView sideBySideView = view as ISideBySideBarSeriesView;
			if (sideBySideView != null)
				sideBySideView.BarDistanceFixed = Convert.ToInt32(txtDistanceFixed.EditValue);
		}
	}
}
