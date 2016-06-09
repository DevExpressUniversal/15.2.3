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
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class RangeBarLabelSettings : ChartUserControl {
		RangeBarSeriesLabel label;
		public RangeBarLabelSettings() {
			InitializeComponent();
		}
		public void Initialize(RangeBarSeriesLabel label) {
			this.label = label;
			cbKind.SelectedIndex = (int)label.Kind;
			cbPosition.SelectedIndex = (int)label.Position;
			txtIndent.EditValue = label.Indent;
			UpdateControls();
		}
		void UpdateControls() {
			pnlPosition.Enabled = label.Kind != RangeBarLabelKind.OneLabel;
			pnlIndent.Enabled = label.Kind != RangeBarLabelKind.OneLabel && label.Position != RangeBarLabelPosition.Center;
		}
		void txtIndent_EditValueChanged(object sender, EventArgs e) {
			label.Indent = Convert.ToInt32(txtIndent.EditValue);
		}
		void cbKind_SelectedIndexChanged(object sender, EventArgs e) {
			label.Kind = (RangeBarLabelKind)cbKind.SelectedIndex;
			UpdateControls();
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			label.Position = (RangeBarLabelPosition)this.cbPosition.SelectedIndex;
			UpdateControls();
		}
	}
}
