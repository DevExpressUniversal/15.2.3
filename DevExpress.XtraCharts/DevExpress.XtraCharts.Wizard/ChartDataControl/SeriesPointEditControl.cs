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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesPointEditControl : InternalWizardControlBase {
		Chart chart;
		string wizardTitle = String.Empty;
		Series currentSeries = null;
		public SeriesPointEditControl() {
			InitializeComponent();
			lvSeries.AllowChangeSeries(false);
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			this.chart = form.Chart;
			lvSeries.Initialize(form.Chart, form.OriginalChart);
			lvSeries.SetSelectedIndex(0);
		}
		public override void CompleteChanges() {
			pointsGrid.ValidateCollection();
		}
		public override bool ValidateContent() {
			return pointsGrid.FinishEditOperation();
		}
		private void lvSeries_SelectedIndexChanged(object sender, EventArgs e) {
			currentSeries = lvSeries.SelectedSeries as Series;
			if (currentSeries != null) {
				lvSeries.UpdateControls();
				pointsGrid.DataSource = currentSeries.Points;
				pointsGrid.ReadOnly = !CommonUtils.IsUnboundMode(currentSeries);
			}
		}
	}
}
