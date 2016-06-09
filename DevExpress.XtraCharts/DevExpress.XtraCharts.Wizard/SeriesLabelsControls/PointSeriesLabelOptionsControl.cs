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
	internal partial class PointSeriesLabelOptionsControl : ChartUserControl {
		struct PointLabelPositionItem {
			readonly PointLabelPosition position;
			readonly string text;
			public PointLabelPosition Position { get { return position; } }
			public PointLabelPositionItem(PointLabelPosition position) {
				this.position = position;
				switch (position) {
					case PointLabelPosition.Center:
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelPositionCenter);
						break;
					case PointLabelPosition.Outside:
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelPositionOutside);
						break;
					default:
						ChartDebug.Fail("Unknown bubble label position.");
						text = ChartLocalizer.GetString(ChartStringId.WizBubbleLabelPositionCenter);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is PointLabelPositionItem) && position == ((PointLabelPositionItem)obj).position;
			}
			public override int GetHashCode() {
				return position.GetHashCode();
			}
		}
		PointSeriesLabel label;
		MethodInvoker updateMethod;
		public PointSeriesLabelOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(PointSeriesLabel label, MethodInvoker updateMethod) {
			this.label = label;
			this.updateMethod = updateMethod;
			spnAngle.EditValue = label.Angle;
			cbPosition.Properties.Items.Add(new PointLabelPositionItem(PointLabelPosition.Center));
			cbPosition.Properties.Items.Add(new PointLabelPositionItem(PointLabelPosition.Outside));
			cbPosition.SelectedItem = new PointLabelPositionItem(label.Position);
			BubbleSeriesLabel bubbleLabel = label as BubbleSeriesLabel;
			bubbleSeriesLabelOptionsControl.Visible = bubbleLabel != null;
			if (bubbleLabel != null)
				bubbleSeriesLabelOptionsControl.Initialize(bubbleLabel, UpdateControls);
			UpdateControls();
		}
		void spnAngle_EditValueChanged(object sender, EventArgs e) {
			label.Angle = Convert.ToInt32(spnAngle.EditValue);
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			label.Position = ((PointLabelPositionItem)cbPosition.SelectedItem).Position;
			UpdateControls();
		}
		void UpdateControls() {
			BubbleSeriesLabel bubbleLabel = label as BubbleSeriesLabel;
			bool isOutside = label.Position == PointLabelPosition.Outside;
			pnlAngle.Enabled = isOutside;
			if (bubbleLabel != null)
				bubbleSeriesLabelOptionsControl.Enabled = isOutside;
			if(updateMethod != null)
				updateMethod();
		}
	}
}
