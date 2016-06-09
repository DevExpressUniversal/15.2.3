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

using System.Collections;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Gauges.Digital {
	public abstract class BaseDigitalGaugeModelWin : BaseDigitalGaugeModel {
		public BaseDigitalGaugeModelWin(DigitalGauge gauge)
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
		protected override CustomizeActionInfo[] GetActionsCore() {
			ArrayList list = new ArrayList(
				new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run Digital Gauge Designer", "Run Designer", UIHelper.GaugeTypeImages[3]),
					new CustomizeActionInfo("ChangeStyle", "Show Style Chooser", "Change Style", UIHelper.ChangeStyleImage),
					new CustomizeActionInfo("AddDigit", "Add digit", "Add Digit", UIHelper.DigitalGaugesMenu[0]),
					new CustomizeActionInfo("RemoveDigit", "Remove digit", "Remove Digit", UIHelper.DigitalGaugesMenu[1]),
					new CustomizeActionInfo("AddBackgroundLayer", "Add default Background Layer", "Add Background Layer", UIHelper.LinearGaugeElementImages[1],"Layers"),
					new CustomizeActionInfo("AddEffectLayer", "Add default Effect Layer", "Add Effect Layer", UIHelper.LinearGaugeElementImages[5],"Layers"),
				});
			list.AddRange(base.GetActionsCore());
			if(DigitalGauge.BackgroundLayers.Count == 0) list.Add(defaultInitialization);
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
	}
	public class DigitalGaugeModel_S7 : BaseDigitalGaugeModelWin {
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
	public class DigitalGaugeModel_S14 : BaseDigitalGaugeModelWin {
		public DigitalGaugeModel_S14(DigitalGauge gauge)
			: base(gauge) { }
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_S14();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_S14(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
	public class DigitalGaugeModel_M5x8 : BaseDigitalGaugeModelWin {
		public DigitalGaugeModel_M5x8(DigitalGauge gauge)
			: base(gauge) { }
		protected override BaseSegmentsCalculator CreateSegmentsCalculator() {
			return new SegmentsCalculator_M5x8();
		}
		protected override BaseDigit CreateDigit() {
			return new Digit_M5x8(DigitalGauge.AppearanceOn, DigitalGauge.AppearanceOff);
		}
	}
	public class DigitalGaugeModel_M8x14 : BaseDigitalGaugeModelWin {
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
