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
	internal partial class FunnelSeriesLabelOptionsControl : ChartUserControl {
		struct FunnelSeriesLabelPositionItem {
			readonly FunnelSeriesLabelPosition position;
			readonly string text;
			public FunnelSeriesLabelPosition Position { get { return position; } }
			public FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition position) {
				this.position = position;
				switch(position) {
					case FunnelSeriesLabelPosition.Center:
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionCenter);
						break;
					case FunnelSeriesLabelPosition.Left:
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionLeft);
						break;
					case FunnelSeriesLabelPosition.LeftColumn:
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionLeftColumn);
						break;
					case FunnelSeriesLabelPosition.Right:
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionRight);
						break;
					case FunnelSeriesLabelPosition.RightColumn:
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionRightColumn);
						break;
					default:
						ChartDebug.Fail("Unknown funnel series label position.");
						text = ChartLocalizer.GetString(ChartStringId.WizFunnelSeriesLabelPositionRight);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is FunnelSeriesLabelPositionItem) && position == ((FunnelSeriesLabelPositionItem)obj).position;
			}
			public override int GetHashCode() {
				return position.GetHashCode();
			}
		}
		FunnelSeriesLabel label;
		MethodInvoker updateMethod;
		public FunnelSeriesLabelOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(FunnelSeriesLabel label, MethodInvoker updateMethod) {
			this.label = label;
			this.updateMethod = updateMethod;
			cbPosition.Properties.Items.Add(new FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition.LeftColumn));
			cbPosition.Properties.Items.Add(new FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition.Left));
			cbPosition.Properties.Items.Add(new FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition.Center));
			cbPosition.Properties.Items.Add(new FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition.Right));
			cbPosition.Properties.Items.Add(new FunnelSeriesLabelPositionItem(FunnelSeriesLabelPosition.RightColumn));
			cbPosition.SelectedItem = new FunnelSeriesLabelPositionItem(label.Position);
			UpdateControls();
		}
		void UpdateControls() {
			if(updateMethod != null)
				updateMethod();
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			label.Position = ((FunnelSeriesLabelPositionItem)cbPosition.SelectedItem).Position;
			UpdateControls();
		}
	}
}
