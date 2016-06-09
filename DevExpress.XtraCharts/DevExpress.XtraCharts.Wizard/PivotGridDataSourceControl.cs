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
using DevExpress.Data.ChartDataSources;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class PivotGridDataSourceControl : InternalWizardControlBase {
		PivotGridDataSourceOptions Options { get { return WizardForm.Chart.DataContainer.PivotGridDataSourceOptions; } }
		public PivotGridDataSourceControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			UpdateControls();
		}
		public void UpdateControls() {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options)) {
				IPivotGrid pivotGrid = PivotGridDataSourceUtils.GetPivotGrid(Options);
				cheRetrieveDataByColumns.Checked = Options.RetrieveDataByColumns;
				cheRetrieveEmptyCells.Checked = Options.RetrieveEmptyCells;
				panelSinglePageOnlySeparator.Visible = pivotGrid.SinglePageSupported;
				cheSinglePageOnly.Visible = pivotGrid.SinglePageSupported;
				cheSinglePageOnly.Checked = Options.SinglePageOnly;
				speMaxSeriesCount.EditValue = Options.MaxAllowedSeriesCount;
				speMaxPointCountInSeries.EditValue = Options.MaxAllowedPointCountInSeries;
				grSelection.Visible = pivotGrid.SelectionSupported;
				cheSelectionOnly.Checked = Options.SelectionOnly;
				panelUpdateDelay.Enabled = Options.SelectionOnly;
				speUpdateDelay.EditValue = Options.UpdateDelay;
				cheRetrieveColumnTotals.Checked = Options.RetrieveColumnTotals;
				cheRetrieveColumnGrandTotals.Checked = Options.RetrieveColumnGrandTotals;
				cheRetrieveColumnCustomTotals.Checked = Options.RetrieveColumnCustomTotals;
				cheRetrieveRowTotals.Checked = Options.RetrieveRowTotals;
				cheRetrieveRowGrandTotals.Checked = Options.RetrieveRowGrandTotals;
				cheRetrieveRowCustomTotals.Checked = Options.RetrieveRowCustomTotals;
			}
		}
		void cheRetrieveDataByColumns_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveDataByColumns = cheRetrieveDataByColumns.Checked;
		}
		void cheRetrieveEmptyCells_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveEmptyCells = cheRetrieveEmptyCells.Checked;
		}
		void cheSinglePageOnly_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.SinglePageOnly = cheSinglePageOnly.Checked;
		}
		void speMaxSeriesCount_EditValueChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.MaxAllowedSeriesCount = Convert.ToInt32(speMaxSeriesCount.EditValue);
		}
		void speMaxPointCountInSeries_EditValueChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.MaxAllowedPointCountInSeries = Convert.ToInt32(speMaxPointCountInSeries.EditValue);
		}
		void cheSelectionOnly_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options)) {
				Options.SelectionOnly = cheSelectionOnly.Checked;
				panelUpdateDelay.Enabled = cheSelectionOnly.Checked;
			}
		}
		void speUpdateDelay_EditValueChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.UpdateDelay = Convert.ToInt32(speUpdateDelay.EditValue);
		}
		void cheRetrieveColumnTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveColumnTotals = cheRetrieveColumnTotals.Checked;
		}
		void cheRetrieveColumnGrandTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveColumnGrandTotals = cheRetrieveColumnGrandTotals.Checked;
		}
		void cheRetrieveColumnCustomTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveColumnCustomTotals = cheRetrieveColumnCustomTotals.Checked;
		}
		void cheRetrieveRowTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveRowTotals = cheRetrieveRowTotals.Checked;
		}
		void cheRetrieveRowGrandTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveRowGrandTotals = cheRetrieveRowGrandTotals.Checked;
		}
		void cheRetrieveRowCustomTotals_CheckedChanged(object sender, EventArgs e) {
			if (PivotGridDataSourceUtils.HasPivotGrid(Options))
				Options.RetrieveRowCustomTotals = cheRetrieveRowCustomTotals.Checked;
		}
	}
}
