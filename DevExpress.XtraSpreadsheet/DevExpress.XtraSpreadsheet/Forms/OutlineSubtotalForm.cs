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

using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class OutlineSubtotalForm :XtraForm {
		#region fields
		OutlineSubtotalViewModel viewModel;
		#endregion
		public OutlineSubtotalForm() {
			InitializeComponent();
		}
		public OutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
			this.viewModel = viewModel;
			InitializeComponent();
			InitializeViewModelParemeters();
		}
		void InitializeViewModelParemeters() {
			cbChangeIn.Properties.Items.AddRange(viewModel.ColumnNames);
			cbChangeIn.SelectedIndex = 0;
			foreach (string columnName in viewModel.ColumnNames)
				clSubtotalTo.Items.Add(columnName);
			ceReplaceCurrent.Checked = viewModel.ReplaceCurrentSubtotals;
			cePageBreak.Checked = viewModel.PageBreakBeetwenGroups;
			ceSummaryBelow.Checked = viewModel.ShowRowSumsBelow;
			cbUseFunction.Properties.Items.AddRange(viewModel.FunctionItems);
			cbUseFunction.SelectedIndex = viewModel.FunctionIndex;
			clSubtotalTo.SetItemChecked(viewModel.ChangedColumnIndex, true);
		}
		void clSubtotalTo_ItemCheck(object sender, XtraEditors.Controls.ItemCheckEventArgs e) {
			sbOK.Enabled = clSubtotalTo.CheckedIndices.Count != 0;
		}
		void sbRemoveAll_Click(object sender, System.EventArgs e) {
			viewModel.RemoveAll();
			this.Close();
		}
		void sbOK_Click(object sender, System.EventArgs e) {
			ApplyFormProperties();
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}
		void ApplyFormProperties() {
			viewModel.ChangedColumnIndex = cbChangeIn.SelectedIndex;
			viewModel.FunctionIndex = (cbUseFunction.SelectedItem as SubtotalFunctionItem).Index;
			viewModel.FunctionText = (cbUseFunction.SelectedItem as SubtotalFunctionItem).DisplayName;
			viewModel.ReplaceCurrentSubtotals = ceReplaceCurrent.Checked;
			viewModel.PageBreakBeetwenGroups = cePageBreak.Checked;
			viewModel.ShowRowSumsBelow = ceSummaryBelow.Checked;
			viewModel.SubtotalColumnIndices.Clear();
			foreach (int columnIndex in clSubtotalTo.CheckedIndices)
				viewModel.SubtotalColumnIndices.Add(columnIndex);
		}
		void ceReplaceCurrent_CheckedChanged(object sender, System.EventArgs e) {
			ceSummaryBelow.Enabled = ceReplaceCurrent.Checked;
		}
	}
}
