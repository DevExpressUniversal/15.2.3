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
using System.Text;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Core.Base;
namespace DevExpress.Web.ASPxGauges.Gauges.Digital {
	public abstract class BaseDigitalGaugeModelWeb : BaseDigitalGaugeModel {
		public BaseDigitalGaugeModelWeb(DigitalGauge gauge)
			: base(gauge) {
		}
		protected IDigitalGauge DigitalGauge {
			get { return Owner as IDigitalGauge; }
		}
		protected override void OnBackgroundBoundsCalculated(PointF2D start, PointF2D end) {
			if(DigitalGauge.BackgroundLayers.Count > 0) {
				DigitalGauge.BackgroundLayers[0].BeginUpdate();
				DigitalGauge.BackgroundLayers[0].TopLeft = start;
				DigitalGauge.BackgroundLayers[0].BottomRight = end;
				DigitalGauge.BackgroundLayers[0].EndUpdate();
			}
		}
		protected override string GetText() {
			return DigitalGauge.Text;
		}
		protected override int GetDigitCount() {
			return DigitalGauge.DigitCount;
		}
		protected override TextSpacing GetPadding() {
			return DigitalGauge.Padding;
		}
		protected override float GetLetterSpacing() {
			return DigitalGauge.LetterSpacing;
		}
	}
	public class DigitalGaugeModel_S7 : BaseDigitalGaugeModelWeb {
		public DigitalGaugeModel_S7(DigitalGauge gauge)
			: base(gauge) {
		}
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_S7();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_S7(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
	public class DigitalGaugeModel_S14 : BaseDigitalGaugeModelWeb {
		public DigitalGaugeModel_S14(DigitalGauge gauge)
			: base(gauge) { }
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_S14();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_S14(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
	public class DigitalGaugeModel_M5x8 : BaseDigitalGaugeModelWeb {
		public DigitalGaugeModel_M5x8(DigitalGauge gauge)
			: base(gauge) { }
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_M5x8();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_M5x8(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
	public class DigitalGaugeModel_M8x14 : BaseDigitalGaugeModelWeb {
		public DigitalGaugeModel_M8x14(DigitalGauge gauge)
			: base(gauge) { }
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_M8x14();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_M8x14(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
}
