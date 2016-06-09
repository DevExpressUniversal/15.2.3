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
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class ScaleBreakAppearanceControl : ChartUserControl {
		struct ScaleBreakStyleItem {
			ScaleBreakStyle style;
			string text;
			public ScaleBreakStyle Style { get { return style; } }
			public ScaleBreakStyleItem(ScaleBreakStyle style) {
				this.style = style;
				switch(style) {
					case ScaleBreakStyle.Ragged:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleBreakStyleRagged);
						break;
					case ScaleBreakStyle.Straight:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleBreakStyleStraight);
						break;
					case ScaleBreakStyle.Waved:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleBreakStyleWaved);
						break;
					default:
						ChartDebug.Fail("Unknown scale break style.");
						text = ChartLocalizer.GetString(ChartStringId.WizScaleBreakStyleRagged);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is ScaleBreakStyleItem) && style == ((ScaleBreakStyleItem)obj).style;
			}
			public override int GetHashCode() {
				return style.GetHashCode();
			}
		}
		ScaleBreakOptions options;
		public ScaleBreakAppearanceControl() {
			InitializeComponent();
		}
		public void Initialize(ScaleBreakOptions options) {
			this.options = options;
			cbStyle.Properties.Items.Clear();
			cbStyle.Properties.Items.Add(new ScaleBreakStyleItem(ScaleBreakStyle.Straight));
			cbStyle.Properties.Items.Add(new ScaleBreakStyleItem(ScaleBreakStyle.Ragged));
			cbStyle.Properties.Items.Add(new ScaleBreakStyleItem(ScaleBreakStyle.Waved));
			cbStyle.SelectedItem = new ScaleBreakStyleItem(options.Style);
			spnSize.EditValue = options.SizeInPixels;
			ceColor.EditValue = options.Color;
		}
		void cbStyle_SelectedIndexChanged(object sender, EventArgs e) {
			options.Style = ((ScaleBreakStyleItem)cbStyle.SelectedItem).Style;
		}
		void spnSize_EditValueChanged(object sender, EventArgs e) {
			options.SizeInPixels = Convert.ToInt32(spnSize.EditValue);
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			options.Color = (Color)ceColor.EditValue;
		}
	}
}
