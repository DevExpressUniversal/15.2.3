#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public partial class GaugeOptionsForm : DashboardForm {
		IServiceProvider serviceProvider;
		readonly DataDashboardItem dashboardItem;
		readonly Gauge gauge;
		public GaugeOptionsForm() {
			InitializeComponent();
		}
		public GaugeOptionsForm(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, Gauge gauge)
			: this() {
			this.serviceProvider = serviceProvider;
			this.dashboardItem = dashboardItem;
			this.gauge = gauge;
			ceAutomaticMinimum.Checked = gauge.Minimum == null;
			ceAutomaticMaximum.Checked = gauge.Maximum == null;
			UpdateMinimum();
			UpdateMaximum();
			deltaOptionsControl.PrepareOptions(gauge.DeltaOptions);
			deltaOptionsControl.Enabled = gauge.ActualValue != null && gauge.TargetValue != null;
		}
		double GaugeMin {
			get {
				IGaugeMinMaxProvider gaugeMinMaxProvider = serviceProvider.RequestServiceStrictly<IGaugeMinMaxProvider>();
				return !String.IsNullOrEmpty(gauge.UniqueName) ? gaugeMinMaxProvider.GetRangeMin(dashboardItem.ComponentName, gauge.UniqueName) : 0;
			}
		}
		double GaugeMax {
			get {
				IGaugeMinMaxProvider gaugeMinMaxProvider = serviceProvider.RequestServiceStrictly<IGaugeMinMaxProvider>();
				return !String.IsNullOrEmpty(gauge.UniqueName) ? gaugeMinMaxProvider.GetRangeMax(dashboardItem.ComponentName, gauge.UniqueName) : 0;
			}
		}
		void UpdateMinimum() {
			seMinimum.Value = Convert.ToDecimal(gauge.Minimum ?? GaugeMin);
		}
		void UpdateMaximum() {
			seMaximum.Value = Convert.ToDecimal(gauge.Maximum ?? GaugeMax);
		}
		void UpdateElementsEnabled() {
			bool manualMinimum = !ceAutomaticMinimum.Checked;
			labelMinimum.Enabled = manualMinimum;
			seMinimum.Enabled = manualMinimum;
			bool manualMaximum = !ceAutomaticMaximum.Checked;
			labelMaximum.Enabled = manualMaximum;
			seMaximum.Enabled = manualMaximum;
		}
		void OnAutomaticMinimumCheckedChanged(object sender, EventArgs e) {
			UpdateElementsEnabled();
			UpdateMinimum();
		}
		void OnAutomaticMaximumCheckedChanged(object sender, EventArgs e) {
			UpdateElementsEnabled();
			UpdateMaximum();
		}
		void OnOkClick(object sender, EventArgs e) {
			IDashboardDesignerSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			GaugeDashboardItem gaugeDashboardItem = selectionService.SelectedDashboardItem as GaugeDashboardItem;
			if (gaugeDashboardItem != null) {
				GaugeOptionsHistoryItem historyItem = new GaugeOptionsHistoryItem(gaugeDashboardItem, gauge, deltaOptionsControl.DeltaOptions,
					ceAutomaticMinimum.Checked ? null : (double?)Convert.ToDouble(seMinimum.Value), ceAutomaticMaximum.Checked ? null : (double?)Convert.ToDouble(seMaximum.Value));
				IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
				historyService.RedoAndAdd(historyItem);
			}
			DialogResult = DialogResult.OK;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			UpdateElementsEnabled();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				serviceProvider = null;
			}
			base.Dispose(disposing);
		}
	}
}
