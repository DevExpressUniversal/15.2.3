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
using System.Drawing;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class TrendLineControl : IndicatorControlBase {
		TrendLine TrendLine { get { return (TrendLine)Indicator; } }
		public TrendLineControl() {
			InitializeComponent();
		}
		public override void Initialize(Indicator indicator) {
			base.Initialize(indicator);
			TrendLine trendLine = TrendLine;
			chkExtrapolate.Checked = trendLine.ExtrapolateToInfinity;
			financialIndicatorControl.Initialize(trendLine);
			ceColor.EditValue = trendLine.Color;
			lineStyleControl.Initialize(trendLine.LineStyle);
		}
		protected override void UpdateControls() {
			base.UpdateControls();
			bool enabled = Indicator.Visible;
			chkExtrapolate.Enabled = enabled;
			xtraTabControl.Enabled = enabled;
		}
		void chkExtrapolate_CheckedChanged(object sender, EventArgs e) {
			TrendLine.ExtrapolateToInfinity = chkExtrapolate.Checked;
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			TrendLine.Color = (Color)ceColor.EditValue;
		}
	}
}
