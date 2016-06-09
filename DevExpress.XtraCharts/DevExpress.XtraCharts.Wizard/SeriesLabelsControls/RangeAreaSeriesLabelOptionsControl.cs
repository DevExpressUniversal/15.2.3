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
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class RangeAreaSeriesLabelOptionsControl : ChartUserControl {
		RangeAreaSeriesLabel label;
		MethodInvoker updateMethod;
		public RangeAreaSeriesLabelOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(RangeAreaSeriesLabel label, MethodInvoker updateMethod) {
			this.label = label;
			this.updateMethod = updateMethod;
			cbKind.SelectedIndex = (int)label.Kind;
			spnMaxValueAngle.EditValue = label.MaxValueAngle;
			spnMinValueAngle.EditValue = label.MinValueAngle;
			RangeArea3DSeriesLabel rangeArea3DLabel = label as RangeArea3DSeriesLabel;
			if (rangeArea3DLabel != null) 
				pnlMinMaxAngles.Visible = false;
			UpdateControls();
		}
		void UpdateControls() {
			pnlMaxValueAngle.Enabled = label.Kind != RangeAreaLabelKind.OneLabel && label.Kind != RangeAreaLabelKind.MinValueLabel;
			pnlMinValueAngle.Enabled = label.Kind != RangeAreaLabelKind.OneLabel && label.Kind != RangeAreaLabelKind.MaxValueLabel;
			if (updateMethod != null)
				updateMethod();
		}
		void cbKind_SelectedIndexChanged(object sender, EventArgs e) {
			label.Kind = (RangeAreaLabelKind)cbKind.SelectedIndex;
			UpdateControls();
		}
		void spnMaxValueAngle_EditValueChanged(object sender, EventArgs e) {
			label.MaxValueAngle = Convert.ToInt32(spnMaxValueAngle.EditValue);
		}
		void spnMinValueAngle_EditValueChanged(object sender, EventArgs e) {
			label.MinValueAngle = Convert.ToInt32(spnMinValueAngle.EditValue);
		}
	}
}
