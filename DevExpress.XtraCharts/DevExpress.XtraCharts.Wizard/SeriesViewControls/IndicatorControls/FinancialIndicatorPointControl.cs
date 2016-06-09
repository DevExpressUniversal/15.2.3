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
using System.ComponentModel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class FinancialIndicatorPointControl : ValidateControl {
		FinancialIndicatorPoint point;
		FinancialIndicator financialIndicator;
		public FinancialIndicatorPointControl() {
			InitializeComponent();
		}
		public void Initialize(FinancialIndicatorPoint point, FinancialIndicator financialIndicator) {
			this.point = point;
			this.financialIndicator = financialIndicator;
			txtArgument.EditValue = point.Argument;
			cmbValueLevel.Properties.Items.Clear();
			IViewArgumentValueOptions View = financialIndicator.View as IViewArgumentValueOptions;
			if (View != null) {
				foreach (ValueLevel valueLevel in View.SupportedValueLevels)
					cmbValueLevel.Properties.Items.Add(new ValueLevelItem(valueLevel));
				cmbValueLevel.SelectedItem = new ValueLevelItem(point.ValueLevel);
			}
		}
		void txtArgument_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !ValidArgument(txtArgument.EditValue);
			SetInvalidState();
		}
		void txtArgument_Validated(object sender, EventArgs e) {
			point.Argument = txtArgument.EditValue;
			txtArgument.EditValue = point.Argument;
			SetValidState();
		}
		bool ValidArgument(object value) {
			try {
				if (point.Argument is double) {
					Convert.ToDouble(txtArgument.EditValue);
					return true;
				}
				else if (point.Argument is DateTime) {
					Convert.ToDateTime(txtArgument.EditValue);
					return true;
				}
				else if (point.Argument is string)
					return true;
			}
			catch {
				return false;
			}
			return false;
		}
		void cmbValueLevel_SelectedIndexChanged(object sender, EventArgs e) {
			point.ValueLevel = ((ValueLevelItem)cmbValueLevel.SelectedItem).ValueLevel;
		}
	}
}
