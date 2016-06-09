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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class HeaderPrintInfoCalculator {
		const float headerMinHeight = 1 / 10F;
		const float headerMaxHeight = 1 / 4F;
		const float headerDefaultHeight = (headerMinHeight + headerMaxHeight) / 2;
		const int textVerticalMargin = 2;
		const int calendarVerticalMargin = 2;
		const int textHorizontalMargin = 2;
		const int calendarHorizontalMargin = 2;
		const float calendarMaxWidth = 2 / 3F;
		const float dayOfWeekFontSizeMultiplier = 0.61803F;
		Font font;
		Graphics graphics;
		Color foreColor = Color.Black;
		public HeaderPrintInfoCalculator(Graphics graphics, Font font, Color foreColor) {
			this.graphics = graphics;
			this.font = font;
			this.foreColor = foreColor;
		}
		public HeaderPrintInfoCalculator(Graphics graphics, Font font)
			: this(graphics, font, Color.Black) {
		}
		protected Color ForeColor { get { return foreColor; } }
		public virtual PreliminaryHeaderPrintInfo CalculatePreliminaryHeaderInfo(HeaderPrintInfoCalculatorParameters calculatorParameters) {
			Rectangle pageBounds = calculatorParameters.PageBounds;
			PreliminaryHeaderPrintInfo preliminaryResult = new PreliminaryHeaderPrintInfo();
			DateNavigator dateNavigator = CreateDateNavigator(calculatorParameters.Control, calculatorParameters.Interval.Start.Date);
			AdjustDateNavigatorSize(dateNavigator, pageBounds, calculatorParameters.Layout, preliminaryResult);
			int monthCalendarCount = preliminaryResult.MaxMonthCalendarCount;
			Size singleMonthSize = preliminaryResult.SingleMonthSize;
			Rectangle headerBounds = Rectangle.Empty;
			preliminaryResult.FirstLineText = calculatorParameters.FirstLineText;
			preliminaryResult.SecondLineText = calculatorParameters.SecondLineText;
			while (true) {
				dateNavigator.Size = new Size(singleMonthSize.Width * monthCalendarCount, singleMonthSize.Height);
				int headerHeight = CalculateHeaderHeight(pageBounds, dateNavigator);
				headerBounds = new Rectangle(pageBounds.X, pageBounds.Y, pageBounds.Width, headerHeight);
				Rectangle availableTextBounds = CalculateAvailableTextBounds(headerBounds, dateNavigator, calculatorParameters.Layout);
				availableTextBounds.Inflate(-textHorizontalMargin, -textVerticalMargin);
				preliminaryResult.AvailableTextBounds = availableTextBounds;
				preliminaryResult.FirstLineSize = graphics.MeasureString(calculatorParameters.FirstLineText, font);
				SizeF secondLineSize = graphics.MeasureString(calculatorParameters.SecondLineText, font);
				preliminaryResult.SecondLineSize = new SizeF(secondLineSize.Width * calculatorParameters.SecondLineTextSizeMultiplier, secondLineSize.Height * calculatorParameters.SecondLineTextSizeMultiplier);
				CalculateTextScaleAndActualSize(preliminaryResult, calculatorParameters.AutoScaleHeadingsFont);
				if (headerHeight / preliminaryResult.TextHeight < 2 || monthCalendarCount == 0) {
					preliminaryResult.HeaderHeight = headerHeight;
					break;
				}
				monthCalendarCount--;
			}
			preliminaryResult.MonthCalendarCount = monthCalendarCount;
			preliminaryResult.DateNavigator = dateNavigator;
			return preliminaryResult;
		}
		public virtual HeaderPrintInfo Calculate(PreliminaryHeaderPrintInfo preliminaryHeaderPrintInfo, Rectangle pageBounds, ViewPart viewPart, float secondLineTextSizeMultiplier) {
			Rectangle headerBounds = new Rectangle(pageBounds.X, pageBounds.Y, pageBounds.Width, preliminaryHeaderPrintInfo.HeaderHeight);
			HeaderTextInfo firstTextLine = null;
			HeaderTextInfo secondTextLine = null;
			bool printText = viewPart == ViewPart.Left || viewPart == ViewPart.Both;
			double scale = preliminaryHeaderPrintInfo.Scale;
			if (printText && scale > 0) {
				Rectangle actualTextBounds = CalculateActualTextBounds(preliminaryHeaderPrintInfo);
				Rectangle firstLineBounds = CalculateFirstLineBounds(preliminaryHeaderPrintInfo.FirstLineSize, preliminaryHeaderPrintInfo.Scale, actualTextBounds);
				Rectangle secondLineBounds = CalculateSecondLineBounds(preliminaryHeaderPrintInfo.SecondLineSize, preliminaryHeaderPrintInfo.Scale, actualTextBounds);
				firstTextLine = new HeaderTextInfo(preliminaryHeaderPrintInfo.FirstLineText, firstLineBounds, scale, font, ForeColor, 1.0f);
				secondTextLine = new HeaderTextInfo(preliminaryHeaderPrintInfo.SecondLineText, secondLineBounds, scale, font, ForeColor, secondLineTextSizeMultiplier);
			}
			CalendarHeaderPrintInfo calendarInfo = null;
			bool printCalendar = viewPart == ViewPart.Right || viewPart == ViewPart.Both;
			if (printCalendar && preliminaryHeaderPrintInfo.MonthCalendarCount != 0) {
				DateNavigator dateNavigator = preliminaryHeaderPrintInfo.DateNavigator;
				Rectangle dateNavigatorBounds = CalculateDateNavigatorBounds(dateNavigator, headerBounds);
				calendarInfo = new CalendarHeaderPrintInfo(dateNavigator, dateNavigatorBounds);
			}
			return new HeaderPrintInfo(headerBounds, firstTextLine, secondTextLine, calendarInfo);
		}
		Rectangle CalculateDateNavigatorBounds(DateNavigator dateNavigator, Rectangle headerBounds) {
			return new Rectangle(headerBounds.Right - dateNavigator.Size.Width - calendarHorizontalMargin, headerBounds.Top, dateNavigator.Size.Width, headerBounds.Height);
		}
		Rectangle CalculateFirstLineBounds(SizeF firstLineSize, double scale, Rectangle actualTextBounds) {
			return new Rectangle(actualTextBounds.Left, actualTextBounds.Top, (int)(firstLineSize.Width * scale), (int)(firstLineSize.Height * scale));
		}
		Rectangle CalculateSecondLineBounds(SizeF secondLineSize, double scale, Rectangle actualTextBounds) {
			return new Rectangle(actualTextBounds.Left, (int)(actualTextBounds.Bottom - secondLineSize.Height * scale), (int)(secondLineSize.Width * scale), (int)(secondLineSize.Height * scale));
		}
		Rectangle CalculateAvailableTextBounds(Rectangle headerBounds, DateNavigator dateNavigator, PageLayout layout) {
			if (layout == PageLayout.OnePage)
				return new Rectangle(headerBounds.X, headerBounds.Y, headerBounds.Width - dateNavigator.Size.Width, headerBounds.Height);
			else
				return headerBounds;
		}
		void CalculateTextScaleAndActualSize(PreliminaryHeaderPrintInfo preliminaryHeaderPrintInfo, bool autoScaleHeadingsFont) {
			Rectangle availableTextBounds = preliminaryHeaderPrintInfo.AvailableTextBounds;
			SizeF firstLineSize = preliminaryHeaderPrintInfo.FirstLineSize;
			SizeF secondLineSize = preliminaryHeaderPrintInfo.SecondLineSize;
			double textWidth = Math.Max(firstLineSize.Width, secondLineSize.Width);
			double textHeight = firstLineSize.Height + secondLineSize.Height;
			double scaleX = availableTextBounds.Width / textWidth;
			double scaleY = availableTextBounds.Height / textHeight;
			double scale = Math.Min(scaleX, scaleY);
			if (!autoScaleHeadingsFont)
				scale = Math.Min(scale, 1);
			preliminaryHeaderPrintInfo.Scale = scale;
			preliminaryHeaderPrintInfo.TextWidth = textWidth * scale;
			preliminaryHeaderPrintInfo.TextHeight = textHeight * scale;
		}
		Rectangle CalculateActualTextBounds(PreliminaryHeaderPrintInfo preliminaryHeaderPrintInfo) {
			double textWidth = preliminaryHeaderPrintInfo.TextWidth;
			double textHeight = preliminaryHeaderPrintInfo.TextHeight;
			Rectangle availableTextBounds = preliminaryHeaderPrintInfo.AvailableTextBounds;
			int top = availableTextBounds.Top + (int)((availableTextBounds.Height - textHeight) / 2);
			return new Rectangle(availableTextBounds.Left, top, (int)textWidth, (int)textHeight);
		}
		int CalculateHeaderHeight(Rectangle pageBounds, DateNavigator dateNavigator) {
			if (dateNavigator != null && dateNavigator.Size.Height != 0)
				return dateNavigator.Size.Height + 2 * textVerticalMargin;
			else
				return (int)(pageBounds.Height * headerDefaultHeight) + 2 * calendarVerticalMargin;
		}
		int AdjustDateNavigatorSize(DateNavigator dateNavigator, Rectangle pageBounds, PageLayout layout, PreliminaryHeaderPrintInfo preliminaryHeaderPrintInfo) {
			int maxMonthCalendarCount = 2;
			Size singleMonthSize = AdjustDateNavigatorHeight(dateNavigator, pageBounds);
			if (singleMonthSize == Size.Empty) {
				dateNavigator.Size = Size.Empty;
				return 0;
			}
			int dateNavigatorMaxWidth = CalculateDateNavigatorMaxWidth(pageBounds, layout);
			for (int i = maxMonthCalendarCount; i >= 0; i--) {
				int width = singleMonthSize.Width * i;
				if (width + 2 * calendarHorizontalMargin <= dateNavigatorMaxWidth) {
					dateNavigator.Size = new Size(width, singleMonthSize.Height);
					preliminaryHeaderPrintInfo.SingleMonthSize = singleMonthSize;
					preliminaryHeaderPrintInfo.MaxMonthCalendarCount = i;
					return i;
				}
			}
			return 0;
		}
		int CalculateDateNavigatorMaxWidth(Rectangle maxBounds, PageLayout layout) {
			if (layout == PageLayout.TwoPage)
				return maxBounds.Width;
			else
				return (int)(calendarMaxWidth * maxBounds.Width);
		}
		Size AdjustDateNavigatorHeight(DateNavigator dateNavigator, Rectangle pageBounds) {
			int dateNavigatorMaxHeight = (int)(pageBounds.Height * headerMaxHeight);
			int dateNavigatorMinHeight = (int)(pageBounds.Height * headerMinHeight);
			Size singleMonthSize = dateNavigator.CalcBestSize();
			if (singleMonthSize.Height >= dateNavigatorMinHeight && singleMonthSize.Height <= dateNavigatorMaxHeight)
				return singleMonthSize;
			float minFontSize = CalcualteDateNavigatorMinFontSize(dateNavigator, dateNavigatorMinHeight);
			if (minFontSize == 0f)
				return Size.Empty;
			float maxFontSize = CalcualteDateNavigatorMaxFontSize(dateNavigator, dateNavigatorMaxHeight);
			return AdjustDateNavigatorFontSize(dateNavigator, dateNavigatorMinHeight, dateNavigatorMaxHeight, minFontSize, maxFontSize);
		}
		float CalcualteDateNavigatorMinFontSize(DateNavigator dateNavigator, int dateNavigatorMinHeight) {
			float fontSize = dateNavigator.Font.Size;
			while (fontSize > 0 && dateNavigator.CalcBestSize().Height > dateNavigatorMinHeight) {
				fontSize /= 2.0f;
				if (fontSize == 0f) {
					return 0f;
				}
				SetDateNavigatorFontSize(dateNavigator, fontSize);
			}
			return fontSize;
		}
		float CalcualteDateNavigatorMaxFontSize(DateNavigator dateNavigator, int dateNavigatorMaxHeight) {
			float fontSize = dateNavigator.Font.Size;
			while (dateNavigator.CalcBestSize().Height < dateNavigatorMaxHeight) {
				fontSize *= 2;
				SetDateNavigatorFontSize(dateNavigator, fontSize);
			}
			return fontSize;
		}
		Size AdjustDateNavigatorFontSize(DateNavigator dateNavigator, int dateNavigatorMinHeight, int dateNavigatorMaxHeight, float minFontSize, float maxFontSize) {
			Size singleMonthSize;
			do {
				float fontSize = (minFontSize + maxFontSize) / 2;
				SetDateNavigatorFontSize(dateNavigator, fontSize);
				singleMonthSize = dateNavigator.CalcBestSize();
				if (singleMonthSize.Height < dateNavigatorMinHeight)
					minFontSize = fontSize;
				if (singleMonthSize.Height > dateNavigatorMaxHeight)
					maxFontSize = fontSize;
			} while (singleMonthSize.Height < dateNavigatorMinHeight || singleMonthSize.Height > dateNavigatorMaxHeight);
			return singleMonthSize;
		}
		DateNavigator CreateDateNavigator(SchedulerControl control, DateTime date) {
			DateNavigator dateNavigator = new DateNavigator();
			dateNavigator.CalendarView = XtraEditors.Repository.CalendarView.Classic;
			dateNavigator.PrintNavigator = true;
			dateNavigator.WeekDayAbbreviationLength = 1;
			dateNavigator.ClientSize = Size.Empty;
			dateNavigator.PrintNavigator = true;
			dateNavigator.ShowWeekNumbers = false;
			dateNavigator.ShowTodayButton = false;
			dateNavigator.DateTime = date;
			dateNavigator.SchedulerControl = control;
			dateNavigator.ShowHeader = false;
			dateNavigator.ShowFooter = false;
			dateNavigator.HighlightHolidays = false;
			dateNavigator.CellPadding = new System.Windows.Forms.Padding(1);
			dateNavigator.CreateControl();
			return dateNavigator;
		}
		void SetDateNavigatorFontSize(DateNavigator dateNavigator, float fontSize) {
			Font dateNavigatorFont = dateNavigator.Font;
			Font newDateNavigatorFont = new Font(dateNavigatorFont.FontFamily, fontSize,
				dateNavigatorFont.Style, dateNavigatorFont.Unit, dateNavigatorFont.GdiCharSet);
			dateNavigator.Font = newDateNavigatorFont;
			dateNavigator.CalendarAppearance.DayCell.Font = newDateNavigatorFont;
			dateNavigator.CalendarAppearance.Header.Font = newDateNavigatorFont;
		}
	}
}
