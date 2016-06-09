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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeRulerLayoutCalculatorBase {
		TimeRulerPainter painter;
		ISupportTimeRuler timeRulerSupport;
		TimeRulerCalculatorHelperBase helper;
		GraphicsCache cache;
		protected TimeRulerLayoutCalculatorBase(GraphicsCache cache, ISupportTimeRuler timeRulerSupport, TimeRulerPainter painter) {
			this.cache = cache;
			this.timeRulerSupport = timeRulerSupport;
			this.painter = painter;
			this.helper = CreateCalculatorHelper();
		}
		#region Properties
		protected internal ISupportTimeRuler TimeRulerSupport { get { return timeRulerSupport; } }
		protected internal TimeRulerPainter Painter { get { return painter; } }
		public TimeRulerCalculatorHelperBase Helper { get { return helper; } }
		public GraphicsCache Cache { get { return cache; } }
		#endregion
		protected internal virtual TimeRulerCalculatorHelperBase CreateCalculatorHelper() {
			if (IsHourScale(TimeRulerSupport.TimeScale))
				return new TimeRulerHourScaleCalculatorHelper(Cache, Painter);
			else
				return new TimeRulerMinutesScaleCalculatorHelper(Cache, Painter);
		}
		protected internal virtual bool IsHourScale(TimeSpan scale) {
			if (scale < DateTimeHelper.HourSpan)
				return false;
			double totalHours = scale.TotalHours;
			double remainder = totalHours % (int)totalHours;
			return remainder == 0;
		}
		protected internal virtual TimeRulerViewInfo CalcRulerPreliminaryLayout(TimeRuler ruler, Rectangle availableBounds, ITimeRulerFormatStringService formatStringProvider) {
			TimeFormatInfo formatInfo = new TimeFormatInfo();
			formatInfo.Initialize(ruler, formatStringProvider);
			TimeRulerViewInfo viewInfo = CreateRulerViewInfo(ruler, formatInfo);
			int width = CalculateTimeRulerWidth(ruler, availableBounds, viewInfo);
			viewInfo.Bounds = new Rectangle(availableBounds.X, availableBounds.Y, width, availableBounds.Height);
			return viewInfo;
		}
		protected internal virtual int CalculateTimeRulerWidth(TimeRuler ruler, Rectangle availableBounds, TimeRulerViewInfo viewInfo) {
			return Helper.CalculateTimeRulerWidth(ruler, viewInfo);
		}
		protected internal virtual TimeRulerViewInfo CreateRulerViewInfo(TimeRuler ruler, TimeFormatInfo formatInfo) {
			return new TimeRulerViewInfo(ruler, TimeRulerSupport.PaintAppearance, formatInfo);
		}
		protected internal virtual void CalcRulerLayoutCore(TimeRulerViewInfo ruler, Rectangle[] rowsBounds, DateTime[] actualTimes, bool isFirstRuler) {
			Helper.PrepareLayout(ruler);
			if (rowsBounds.Length <= 0) {
				ruler.HeaderBounds = ruler.Bounds;
				return;
			}
			ruler.IsFirst = isFirstRuler;
			ruler.ContentBounds = CalcRulerContentBounds(ruler, rowsBounds);
			ruler.HeaderBounds = CalcRulerHeaderBounds(ruler, rowsBounds);
			ruler.HasTopBorder = true;
			ruler.HasBottomBorder = false;
			ruler.HasLeftBorder = !ruler.IsFirst;
			ruler.HasRightBorder = false;
			ruler.CalcBorderBounds(Painter);
			ruler.ClientBounds = CalcRulerClientBounds(ruler);
			ruler.Items.AddRange(Helper.CreateTimeCaptions(ruler, actualTimes, rowsBounds));
			ruler.Items.AddRange(Helper.CreateTimeSeparatorLines(ruler, actualTimes, rowsBounds));
			ruler.HeaderCaptionItem = CreateHeaderCaptionItem(ruler);
		}
		protected internal virtual DateTime CalcActualDate(TimeSpan time, TimeSpan currentUtcOffset, TimeSpan targetUtcOffset) {
			TimeSpan utcTime = time + DateTimeHelper.DaySpan - currentUtcOffset;
			return new DateTime((utcTime + targetUtcOffset).Ticks);
		}
		protected internal virtual DateTime CalcActualDate(DateTime time, TimeSpan currentUtcOffset, TimeSpan targetUtcOffset) {
			TimeSpan timeSpan = TimeSpan.FromTicks(time.Ticks);
			return CalcActualDate(timeSpan, currentUtcOffset, targetUtcOffset);
		}
		protected internal virtual TimeSpan CalcTargetUtcOffset(TimeRuler ruler, DateTime date) {
			TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById(ruler.TimeZoneId);
			if (tzInfo == null)
				tzInfo = TimeZoneEngine.Local;
			if (ruler.AdjustForDaylightSavingTime)
				return tzInfo.GetUtcOffset(date);
			return GetTimeZoneUtcOffsetWithoutDST(date, tzInfo);
		}
		protected internal virtual Rectangle CalcRulerContentBounds(TimeRulerViewInfo ruler, Rectangle[] rowsBounds) {
			int topRowY = rowsBounds[0].Y - 1;
			Rectangle bounds = ruler.Bounds;
			Rectangle result = new Rectangle(bounds.X, topRowY, bounds.Width, bounds.Bottom - topRowY);
			if (!ruler.IsFirst) {
				result.X -= Painter.HorizontalOverlap;
				result.Width += Painter.HorizontalOverlap;
			}
			return result;
		}
		protected internal virtual Rectangle CalcRulerHeaderBounds(TimeRulerViewInfo ruler, Rectangle[] rowsBounds) {
			int topRowY = rowsBounds[0].Y - 1;
			Rectangle bounds = ruler.Bounds;
			Rectangle result = new Rectangle(bounds.X, bounds.Y, bounds.Width, topRowY - bounds.Y);
			result.Height += Painter.VerticalOverlap;
			if (!ruler.IsFirst) {
				result.X -= Painter.HorizontalOverlap;
				result.Width += Painter.HorizontalOverlap;
			}
			return result;
		}
		protected internal virtual Rectangle CalcRulerClientBounds(TimeRulerViewInfo ruler) {
			Rectangle result = Painter.CalcClientBounds(Cache, ruler.ContentBounds);
			result.Inflate(-Painter.ContentSpan, 0);
			return result;
		}
		protected internal virtual ViewInfoTextItem CreateHeaderCaptionItem(TimeRulerViewInfo ruler) {
			string caption = ruler.Ruler.Caption;
			if (caption == null)
				return null;
			caption = caption.Trim();
			if (caption.Length <= 0)
				return null;
			ViewInfoTextItem item = new ViewInfoTextItem();
			item.Bounds = Painter.CalcHeaderContentBounds(Cache, ruler);
			AppearanceHelper.Combine(item.Appearance, new AppearanceObject[] { ruler.BackgroundAppearance });
			item.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
			item.Text = caption;
			return item;
		}
		TimeSpan GetTimeZoneUtcOffsetWithoutDST(DateTime date, TimeZoneInfo timeZone) {
			if (!timeZone.IsDaylightSavingTime(date))
				return timeZone.GetUtcOffset(date);
			TimeSpan result = timeZone.GetUtcOffset(date);
			foreach (System.TimeZoneInfo.AdjustmentRule rule in timeZone.GetAdjustmentRules()) {
				if (rule.DateStart <= date && date <= rule.DateEnd) {
					if (rule.DaylightDelta == TimeSpan.Zero)
						return result;
					DateTime startDaylightTime = TimeZoneInfoUtils.CalculateDateTimeFromTransitionTime(date.Year, rule.DaylightTransitionStart);
					DateTime endDaylightTime = TimeZoneInfoUtils.CalculateDateTimeFromTransitionTime(date.Year, rule.DaylightTransitionEnd);
					if (startDaylightTime <= date && date <= endDaylightTime) {
						result = result - rule.DaylightDelta;
						break;
					}
				}
			}
			return result;
		}
	}
}
