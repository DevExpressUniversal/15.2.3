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
	internal partial class MovingAverageControl : SubsetBasedIndicatorControl {
		new MovingAverage Indicator { get { return (MovingAverage)base.Indicator; } }
		public MovingAverageControl() {
			InitializeComponent();
		}
		public override void Initialize(Indicator indicator) {
			base.Initialize(indicator);
			MovingAverage movingAverage = Indicator;
			switch (movingAverage.Kind) {
				case MovingAverageKind.Envelope:
					cbKind.SelectedIndex = 1;
					break;
				case MovingAverageKind.MovingAverageAndEnvelope:
					cbKind.SelectedIndex = 2;
					break;
				default:
					cbKind.SelectedIndex = 0;
					break;
			}
			txtEnvelopePercent.EditValue = movingAverage.EnvelopePercent;
			ceColor.Color = movingAverage.Color;
			lineStyleControl.Initialize(movingAverage.LineStyle);
			ceEnvelopeColor.Color = movingAverage.EnvelopeColor;
			envelopeLineStyleControl.Initialize(movingAverage.EnvelopeLineStyle);
			UpdateEnvelopeControls();
		}
		protected override void UpdateControls() {
			base.UpdateControls();
			bool enabled = Indicator.Visible;
			panelColor.Enabled = enabled;
			lineStyleControl.Enabled = enabled;
			UpdateEnvelopeControls();
		}
		void UpdateEnvelopeControls() {
			bool enabled = Indicator.Visible && Indicator.Kind != MovingAverageKind.MovingAverage;
			panelEnvelopePercent.Enabled = enabled;
			panelEnvelopeColor.Enabled = enabled;
			envelopeLineStyleControl.Enabled = enabled;
		}
		void cbKind_SelectedIndexChanged(object sender, EventArgs e) {
			switch (cbKind.SelectedIndex) {
				case 1:
					Indicator.Kind = MovingAverageKind.Envelope;
					break;
				case 2:
					Indicator.Kind = MovingAverageKind.MovingAverageAndEnvelope;
					break;
				default:
					Indicator.Kind = MovingAverageKind.MovingAverage;
					break;
			}
			UpdateEnvelopeControls();
		}
		void txtEnvelopePercent_EditValueChanged(object sender, EventArgs e) {
			Indicator.EnvelopePercent = Convert.ToDouble(txtEnvelopePercent.EditValue);
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			Indicator.Color = (Color)ceColor.EditValue;
		}
		void ceEnvelopeColor_EditValueChanged(object sender, EventArgs e) {
			Indicator.EnvelopeColor = (Color)ceEnvelopeColor.EditValue;
		}
	}
}
