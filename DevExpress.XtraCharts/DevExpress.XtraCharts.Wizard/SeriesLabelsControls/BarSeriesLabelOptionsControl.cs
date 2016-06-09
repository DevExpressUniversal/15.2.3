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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class BarSeriesLabelOptionsControl : ChartUserControl {
		struct BarSeriesLabelPositionItem {
			readonly BarSeriesLabelPosition position;
			readonly string text;
			public BarSeriesLabelPosition Position { get { return position; } }
			public BarSeriesLabelPositionItem(BarSeriesLabelPosition position) {
				this.position = position;
				switch(position) {
					case BarSeriesLabelPosition.Auto:
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionAuto);
						break;
					case BarSeriesLabelPosition.Top:
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionTop);
						break;
					case BarSeriesLabelPosition.TopInside:
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionTopInside);
						break;
					case BarSeriesLabelPosition.Center:
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionCenter);
						break;
					case BarSeriesLabelPosition.BottomInside:
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionBottomInside);
						break;
					default:
						ChartDebug.Fail("Unknown bar series label position.");
						text = ChartLocalizer.GetString(ChartStringId.WizBarSeriesLabelPositionTop);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is BarSeriesLabelPositionItem) && position == ((BarSeriesLabelPositionItem)obj).position;
			}
			public override int GetHashCode() {
				return position.GetHashCode();
			}
		}
		BarSeriesLabel label;
		MethodInvoker updateMethod;
		public BarSeriesLabelOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(BarSeriesLabel label, MethodInvoker updateMethod) {
			this.label = label;
			this.updateMethod = updateMethod;
			chShowForZeroValues.Checked = label.ShowForZeroValues;
			if (label is SideBySideBarSeriesLabel || label is StackedBarSeriesLabel) {
				if (label is SideBySideBarSeriesLabel) {
					cbPosition.Properties.Items.Add(new BarSeriesLabelPositionItem(BarSeriesLabelPosition.Auto));
					cbPosition.Properties.Items.Add(new BarSeriesLabelPositionItem(BarSeriesLabelPosition.Top));
				}
				cbPosition.Properties.Items.Add(new BarSeriesLabelPositionItem(BarSeriesLabelPosition.TopInside));
				cbPosition.Properties.Items.Add(new BarSeriesLabelPositionItem(BarSeriesLabelPosition.Center));
				cbPosition.Properties.Items.Add(new BarSeriesLabelPositionItem(BarSeriesLabelPosition.BottomInside));
				cbPosition.SelectedItem = new BarSeriesLabelPositionItem(label.Position);
				spnIndent.EditValue = label.Indent;
			}
			else {
				pnlPosition.Visible = false;
				pnlIndent.Visible = false;
			}
			UpdateControls();
		}
		void chShowForZeroValues_CheckedChanged(object sender, EventArgs e) {
			label.ShowForZeroValues = chShowForZeroValues.Checked;
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			if (label is SideBySideBarSeriesLabel || label is StackedBarSeriesLabel) {
				label.Position = ((BarSeriesLabelPositionItem)cbPosition.SelectedItem).Position;
				UpdateControls();
			}
		}
		void spnIndent_EditValueChanged(object sender, EventArgs e) {
			label.Indent = Convert.ToInt32(spnIndent.EditValue);
		}
		void UpdateControls() {
			pnlIndent.Enabled = (label.Position == BarSeriesLabelPosition.TopInside || label.Position == BarSeriesLabelPosition.BottomInside);
			if (updateMethod != null)
				updateMethod();
		}
	}
}
