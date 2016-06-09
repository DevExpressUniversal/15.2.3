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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	#region TimeRulerHourScaleCalculatorHelper
	public class TimeRulerHourScaleCalculatorHelper : TimeRulerCalculatorHelperBase {
		public TimeRulerHourScaleCalculatorHelper(GraphicsCache cache, TimeRulerPainter painter)
			: base(cache, painter) {
		}
		protected internal override void PrepareLayout(TimeRulerViewInfo ruler) {
		}
		protected internal override int CalculateTimeRulerWidthCore(TimeRuler ruler, TimeRulerViewInfo viewInfo) {
			TimeFormatInfo formatInfo = viewInfo.FormatInfo;
			AppearanceObject appearance = viewInfo.BackgroundAppearance;
			int width = CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 0, 0, 0), formatInfo);
			width = Math.Max(width, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 11, 0, 0), formatInfo));
			width = Math.Max(width, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 12, 0, 0), formatInfo));
			width = Math.Max(width, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 13, 0, 0), formatInfo));
			width = Math.Max(width, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 23, 0, 0), formatInfo));
			return width;
		}
		protected internal override string ChooseFormat(DateTime time, bool useTimeDesignator, TimeFormatInfo formatInfo) {
			return ScaleFormatHelper.ChooseHourFormat(time, useTimeDesignator, formatInfo);
		}
		protected internal override ViewInfoItemCollection CreateTimeCaptions(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			HourScaleTimeCaptionItemsCalculator calc = new HourScaleTimeCaptionItemsCalculator(ruler, actualTimes);
			calc.CalculateItems(rowsBounds);
			return calc.Result;
		}
		protected internal override ViewInfoItemCollection CreateTimeSeparatorLines(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			Rectangle separatorBounds = new Rectangle(ruler.ClientBounds.X + Painter.HourSeparatorLineOffset, 0, ruler.ClientBounds.Width - 2 * Painter.HourSeparatorLineOffset, 1);
			HourScaleTimeSeparatorLineItemsCalculator calc = new HourScaleTimeSeparatorLineItemsCalculator(ruler, actualTimes, separatorBounds);
			calc.CalculateItems(rowsBounds);
			return calc.Result;
		}
	}
	#endregion
}
