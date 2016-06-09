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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class ReductionOptionsControl : ChartUserControl {
		ReductionStockOptions options;
		bool IsValidColor {
			get {
				Color color;
				if (cbColor.EditValue is Color)
					color = (Color)cbColor.EditValue;
				else {
					string text = cbColor.EditValue as string;
					color = String.IsNullOrEmpty(text) ? Color.Empty : Color.FromName(text);
				}
				return !color.IsEmpty;
			}
		}
		public ReductionOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(ReductionStockOptions options) {
			this.options = options;
			InitializeControls();
		}
		void InitializeControls() {
			chVisible.Checked = options.Visible;
			cbColor.EditValue = options.Color;
			cbLevel.SelectedIndex = (int)options.Level;
			UpdateControls();
		}
		void UpdateControls() {
			pnlEditors.Enabled = options.Visible;
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			options.Visible = chVisible.Checked;
			UpdateControls();
		}
		void cbColor_EditValueChanged(object sender, EventArgs e) {
			if (IsValidColor)
				options.Color = (Color)cbColor.EditValue;
		}
		void cbColor_Leave(object sender, EventArgs e) {
			if (!IsValidColor)
				cbColor.EditValue = ReductionStockOptions.DefaultColor;
		}
		void cbLevel_SelectedIndexChanged(object sender, EventArgs e) {
			options.Level = (StockLevel)cbLevel.SelectedIndex;
		}
	}
}
