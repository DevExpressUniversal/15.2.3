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
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimeRulerMinutesScaleCalculatorHelper : TimeRulerCalculatorHelperBase {
		Size largeHourSize;
		public TimeRulerMinutesScaleCalculatorHelper(GraphicsCache cache, TimeRulerPainter painter)
			: base(cache, painter) {
		}
		protected internal override void PrepareLayout(TimeRulerViewInfo ruler) {
			this.largeHourSize = CalculateLargeHourSize(ruler.Ruler, ruler);
		}	  
		protected internal override int CalculateTimeRulerWidthCore(TimeRuler ruler, TimeRulerViewInfo viewInfo) {
			TimeFormatInfo formatInfo = viewInfo.FormatInfo;
			AppearanceObject appearance = viewInfo.BackgroundAppearance;
			Size largeHourSize = CalculateLargeHourSize(ruler, viewInfo);
			int smallWidth = CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 0, 0, 0), formatInfo);
			smallWidth = Math.Max(smallWidth, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 12, 0, 0), formatInfo));
			smallWidth = Math.Max(smallWidth, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 0, 59, 0), formatInfo));
			smallWidth = Math.Max(smallWidth, CalcMaxDateStringWidth(appearance, new DateTime(1, 1, 1, 12, 59, 0), formatInfo));
			return largeHourSize.Width + Painter.GapBetweenHourAndMinutes + smallWidth;
		}
		protected internal virtual Size CalculateLargeHourSize(TimeRuler ruler, TimeRulerViewInfo viewInfo) {
			TimeFormatInfo formatInfo = viewInfo.FormatInfo;
			AppearanceObject appearance = viewInfo.LargeHourAppearance;
			Size result = CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 0, 0, 0), formatInfo);
			result = UnionSize(result, CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 0, 0, 0), formatInfo));
			result = UnionSize(result, CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 11, 0, 0), formatInfo));
			result = UnionSize(result, CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 12, 0, 0), formatInfo));
			result = UnionSize(result, CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 13, 0, 0), formatInfo));
			result = UnionSize(result, CalcHourOnlyStringSize(appearance, new DateTime(1, 1, 1, 22, 0, 0), formatInfo));
			return result;
		}
		protected internal virtual Size UnionSize(Size size1, Size size2) {
			return new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
		}
		protected internal virtual Size CalcHourOnlyStringSize(AppearanceObject appearance, DateTime date, TimeFormatInfo formatInfo) {
			string str = date.ToString(formatInfo.HourOnlyFormat);
			return Size.Ceiling(appearance.CalcTextSize(Cache, str, Int32.MaxValue));
		}
		protected internal override string ChooseFormat(DateTime time, bool useTimeDesignator, TimeFormatInfo formatInfo) {
			return ScaleFormatHelper.ChooseMinutesFormat(time, useTimeDesignator, formatInfo);
		}
		protected internal override ViewInfoItemCollection CreateTimeSeparatorLines(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			Rectangle clientBounds = ruler.ClientBounds;
			Rectangle hourLineBounds = new Rectangle(clientBounds.X + Painter.HourSeparatorLineOffset, 0, clientBounds.Width - 2 * Painter.HourSeparatorLineOffset, 1);
			Rectangle minuteLineBounds = Rectangle.FromLTRB(clientBounds.X + this.largeHourSize.Width + Painter.GapBetweenHourAndMinutes, 0, clientBounds.Right - Painter.HourSeparatorLineOffset, 1);
			ViewInfoItemCollection result = new ViewInfoItemCollection();
			MinutesScaleHourTimeSeparatorLineItemsCalculator hourCalc = new MinutesScaleHourTimeSeparatorLineItemsCalculator(ruler, actualTimes, hourLineBounds);
			hourCalc.CalculateItems(rowsBounds);
			result.AddRange(hourCalc.Result);
			MinutesScaleMinutesTimeSeparatorLineItemsCalculator minutesCalc = new MinutesScaleMinutesTimeSeparatorLineItemsCalculator(ruler, actualTimes, minuteLineBounds);
			minutesCalc.CalculateItems(rowsBounds);
			result.AddRange(minutesCalc.Result);
			return result;
		}
		protected internal override ViewInfoItemCollection CreateTimeCaptions(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			XtraSchedulerDebug.Assert(actualTimes.Length == rowsBounds.Length);
			ViewInfoItemCollection result = new ViewInfoItemCollection();
			result.AddRange(CreateHourCaptions(ruler, actualTimes, rowsBounds));
			result.AddRange(CreateMinutesCaptions(ruler, actualTimes, rowsBounds));
			return result;
		}
		protected internal virtual ViewInfoItemCollection CreateHourCaptions(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			MinutesScaleHourCaptionItemsCalculator calc = new MinutesScaleHourCaptionItemsCalculator(ruler, actualTimes, largeHourSize);
			calc.CalculateItems(rowsBounds);
			return calc.Result;
		}
		protected internal virtual ViewInfoItemCollection CreateMinutesCaptions(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds) {
			MinutesScaleMinuteCaptionItemsCalculator calc = CreateMinueCaptionItemcCalculator(ruler, actualTimes);
			calc.CalculateItems(rowsBounds);
			return calc.Result;
		}
		protected internal virtual MinutesScaleMinuteCaptionItemsCalculator CreateMinueCaptionItemcCalculator(TimeRulerViewInfo ruler, DateTime[] actualTimes) {
			if (ruler.Ruler.ShowMinutes)
				return new MinutesScaleAllMinutesCaptionItemsCalculator(ruler, actualTimes);
			else
				return new MinutesScaleMinuteCaptionItemsCalculator(ruler, actualTimes);
		}
	}
}
