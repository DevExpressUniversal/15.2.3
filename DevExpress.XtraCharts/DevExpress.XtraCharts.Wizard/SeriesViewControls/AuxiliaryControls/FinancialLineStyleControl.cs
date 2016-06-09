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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class FinancialLineStyleControl : ChartUserControl {
		FinancialSeriesViewBase view;
		public FinancialLineStyleControl() {
			InitializeComponent();
		}
		public void Initialize(FinancialSeriesViewBase view) {
			this.view = view;
			ceColor.EditValue = this.view.Color;
			txtLength.EditValue = this.view.LevelLineLength;
			txtThickness.EditValue = this.view.LineThickness;
			StockSeriesView stockView = view as StockSeriesView;
			if(stockView != null)
				cbOpenClose.SelectedIndex = (int)stockView.ShowOpenClose;
			else {
				cbOpenClose.Visible = false;
				lblOpenClose.Visible = false;
			}
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			view.Color = (Color)ceColor.EditValue;
		}
		void txtDistance_EditValueChanged(object sender, EventArgs e) {
			view.LineThickness = Convert.ToInt32(txtThickness.EditValue);
		}
		void txtLength_EditValueChanged(object sender, EventArgs e) {
			view.LevelLineLength = Convert.ToDouble(txtLength.EditValue);
		}
		void cbOpenClose_SelectedIndexChanged(object sender, EventArgs e) {
			StockSeriesView stockView = view as StockSeriesView;
			if(stockView != null)
				stockView.ShowOpenClose = (StockType)cbOpenClose.SelectedIndex;
		}
	}
}
