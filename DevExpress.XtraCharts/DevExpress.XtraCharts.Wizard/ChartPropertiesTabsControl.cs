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
using System.Collections;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartPropertiesTabsControl : FilterTabsControl {		
		Chart chart;
		Chart originalChart;
		public override XtraTabControl TabControl { get { return tbcPages; } }
		public ChartPropertiesTabsControl() {
			InitializeComponent();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			chart.Border.Visibility = CheckEditHelper.GetCheckEditState(chVisible);
			bool enabled = CommonUtils.GetActualBorderVisibility(chart.Border);
			lblColor.Enabled = enabled;
			ceBorderColor.Enabled = enabled;
			lblThickness.Enabled = enabled;
			txtThickness.Enabled = enabled;
		}
		void ceBorderColor_EditValueChanged(object sender, EventArgs e) {
			chart.Border.Color = (Color)ceBorderColor.EditValue;
		}
		void txtThickness_EditValueChanged(object sender, EventArgs e) {
			chart.Border.Thickness = Convert.ToInt32(txtThickness.EditValue);
		}
		void chAutoLayout_CheckedChanged(object sender, EventArgs e) {
			chart.AutoLayout = ((CheckEdit)sender).Checked;
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			backgroundControl.Initialize(chart, originalChart);
			CheckEditHelper.SetCheckEditState(chVisible, chart.Border.Visibility);
			ceBorderColor.EditValue = chart.Border.Color;
			txtThickness.EditValue = chart.Border.Thickness;
			if (chart.Container.ControlType == ChartContainerType.XRControl)
				tbcPages.TabPages.Remove(tbBorder);
			paddingControl.Initialize(chart.Padding);
			emptyChartTextControl.Initialize(chart.EmptyChartText);
			smallChartTextControl.Initialize(chart.SmallChartText);
			chAutoLayout.Checked = chart.AutoLayout;
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = ChartPageTab.General;
			tbBorder.Tag = ChartPageTab.Border;
			tbPadding.Tag = ChartPageTab.Padding;
			tbcEmptyChartText.Tag = ChartPageTab.EmptyChartText;
			tbcSmallChartText.Tag = ChartPageTab.SmallChartText;
		}
		public void Initialize(Chart chart, Chart originalChart, UserLookAndFeel lookAndFeel, CollectionBase filter) {
			this.chart = chart;
			this.originalChart = originalChart;
			InitializeCore(lookAndFeel, filter, null);
		}
	}
}
