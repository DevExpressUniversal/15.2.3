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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class BubbleViewControl : ChartUserControl {
		struct BubbleLabelValueToDisplayItem {
			readonly BubbleLabelValueToDisplay valueToDisplay;
			readonly string text;
			public BubbleLabelValueToDisplay ValueToDisplay { get { return valueToDisplay; } }
			public BubbleLabelValueToDisplayItem(BubbleLabelValueToDisplay valueToDisplay) {
				this.valueToDisplay = valueToDisplay;
				switch(valueToDisplay) {
					case BubbleLabelValueToDisplay.Value:
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelValueToDisplayValue);
						break;
					case BubbleLabelValueToDisplay.Weight:
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelValueToDisplayWeight);
						break;
					case BubbleLabelValueToDisplay.ValueAndWeight:
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelValueToDisplayValueAndWeight);
						break;
					default:
						ChartDebug.Fail("Unknown bubble label value to display.");
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelValueToDisplayValue);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is BubbleLabelValueToDisplayItem) && valueToDisplay == ((BubbleLabelValueToDisplayItem)obj).valueToDisplay;
			}
			public override int GetHashCode() {
				return valueToDisplay.GetHashCode();
			}
		}
		BubbleSeriesLabel label;
		MethodInvoker updateMethod;
		public BubbleViewControl() {
			InitializeComponent();
		}
		public void Initialize(BubbleSeriesLabel label, MethodInvoker updateMethod) {
			this.label = label;
			this.updateMethod = updateMethod;
			cbValue.Properties.Items.Add(new BubbleLabelValueToDisplayItem(BubbleLabelValueToDisplay.Weight));
			cbValue.Properties.Items.Add(new BubbleLabelValueToDisplayItem(BubbleLabelValueToDisplay.Value));
			cbValue.Properties.Items.Add(new BubbleLabelValueToDisplayItem(BubbleLabelValueToDisplay.ValueAndWeight));
			UpdateControls();
		}
		void cbValue_SelectedIndexChanged(object sender, EventArgs e) {
		}
		void UpdateControls() {
			if(updateMethod != null)
				updateMethod();
		}
	}
}
