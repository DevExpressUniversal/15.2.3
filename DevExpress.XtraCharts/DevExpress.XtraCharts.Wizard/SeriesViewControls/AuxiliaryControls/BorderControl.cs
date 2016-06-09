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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class BorderControl : ChartUserControl {
		BorderBase border;
		Action0 changedCallback;
		bool inUpdate;
		public BorderControl() {
			InitializeComponent();
		}
		public void Initialize(BorderBase border) {
			Initialize(border, null);
		}
		public void Initialize(BorderBase border, Action0 changedCallback) {
			this.border = border;
			this.changedCallback = changedCallback;
			InitializeControls();
		}
		void InitializeControls() {
			inUpdate = true;
			try {
				CheckEditHelper.SetCheckEditState(chVisible, border.Visibility);
				txtThickness.EditValue = border.Thickness;
				cbColor.EditValue = border.Color;
			}
			finally {
				inUpdate = false;
			}
			UpdateControls();
		}
		void UpdateControls() {
			inUpdate = true;
			try {
				pnlControls.Enabled = CommonUtils.GetActualBorderVisibility(border);
			}
			finally {
				inUpdate = false;
			}
			if (changedCallback != null)
				changedCallback();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				border.Visibility = CheckEditHelper.GetCheckEditState(chVisible);
				UpdateControls();
			}
		}
		void cbColor_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				border.Color = (Color)cbColor.EditValue;
				UpdateControls();
			}
		}
		void txtThickness_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				border.Thickness = Convert.ToInt32(txtThickness.EditValue);
				UpdateControls();
			}
		}
	}
}
