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
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.Drawing;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public class MultilineText {
		public static MultilineText Empty { get { return new MultilineText(string.Empty, string.Empty); } }
		string firstLineText = string.Empty;
		string secondLineText = string.Empty;
		public MultilineText(string firstLineText, string secondLineText) {
			this.firstLineText = firstLineText;
			this.secondLineText = secondLineText;
		}
		public string FirstLineText { get { return firstLineText; } }
		public string SecondLineText { get { return secondLineText; } }
	}
	#region TextInfoPreliminaryLayoutResult
	public class TextInfoPreliminaryLayoutResult {
		double scale;
		double textWidth;
		double textHeight;
		int headerHeight;
		Rectangle availableTextBounds;
		SizeF firstLineSize;
		SizeF secondLineSize;
		string firstLineText;
		string secondLineText;
		public double Scale { get { return scale; } set { scale = value; } }
		public double TextWidth { get { return textWidth; } set { textWidth = value; } }
		public double TextHeight { get { return textHeight; } set { textHeight = value; } }
		public int HeaderHeight { get { return headerHeight; } set { headerHeight = value; } }
		public Rectangle AvailableTextBounds { get { return availableTextBounds; } set { availableTextBounds = value; } }
		public string FirstLineText { get { return firstLineText; } set { firstLineText = value; } }
		public string SecondLineText { get { return secondLineText; } set { secondLineText = value; } }
		public SizeF FirstLineSize { get { return firstLineSize; } set { firstLineSize = value; } }
		public SizeF SecondLineSize { get { return secondLineSize; } set { secondLineSize = value; } }
	}
	#endregion
	#region TextInfoLayoutCalculatorParameters
	public abstract class TextInfoLayoutCalculatorParameters {
		Rectangle printBounds;
		string firstLineText;
		string secondLineText;
		float secondLineTextSizeMultiplier;
		public Rectangle PrintBounds { get { return printBounds; } set { printBounds = value; } }
		public string FirstLineText { get { return firstLineText; } set { firstLineText = value; } }
		public string SecondLineText { get { return secondLineText; } set { secondLineText = value; } }
		public float SecondLineTextSizeMultiplier { get { return secondLineTextSizeMultiplier; } set { secondLineTextSizeMultiplier = value; } }
	}
	#endregion
	public class TimeIntervalLayoutCalculatorParameters : TextInfoLayoutCalculatorParameters {
		TimeInterval interval;
		public TimeInterval Interval { get { return interval; } set { interval = value; } }
	}
	public class ResourceLayoutCalculatorParameters : TextInfoLayoutCalculatorParameters {
		ResourceBaseCollection resources;
		public ResourceBaseCollection Resources { get { return resources; } set { resources = value; }  }
	}
	#region TextInfoLayoutCalculator
	public class TextInfoLayoutCalculator {
		const float headerMinHeight = 1 / 10F;
		const float headerMaxHeight = 1 / 4F;
		const float headerDefaultHeight = (headerMinHeight + headerMaxHeight) / 2;
		const int textVerticalMargin = 2;
		const int calendarVerticalMargin = 2;
		const int textHorizontalMargin = 2;
		Font font;
		Color foreColor;
		Graphics graphics;
		bool autoScaleFont;
		public TextInfoLayoutCalculator(Graphics graphics, Font font, Color foreColor, bool autoScaleFont) {
			this.graphics = graphics;
			this.font = font;
			this.foreColor = foreColor;
			this.autoScaleFont = autoScaleFont;
		}
		public bool AutoScaleFont { get { return autoScaleFont; } }
		public virtual TextInfoPreliminaryLayoutResult CalculatePreliminaryLayout(TextInfoLayoutCalculatorParameters calculatorParameters) {
			TextInfoPreliminaryLayoutResult preliminaryResult = new TextInfoPreliminaryLayoutResult();
			preliminaryResult.FirstLineText = calculatorParameters.FirstLineText;
			preliminaryResult.SecondLineText = calculatorParameters.SecondLineText;
			Rectangle availableTextBounds = calculatorParameters.PrintBounds;
			availableTextBounds.Inflate(-textHorizontalMargin, -textVerticalMargin);
			preliminaryResult.AvailableTextBounds = availableTextBounds;
			preliminaryResult.FirstLineSize = graphics.MeasureString(calculatorParameters.FirstLineText, font);
			SizeF secondLineSize = graphics.MeasureString(calculatorParameters.SecondLineText, font);
			preliminaryResult.SecondLineSize = new SizeF(secondLineSize.Width * calculatorParameters.SecondLineTextSizeMultiplier, secondLineSize.Height * calculatorParameters.SecondLineTextSizeMultiplier);
			CalculateTextScaleAndActualSize(preliminaryResult);
			return preliminaryResult;
		}
		public virtual TextInfoControlPrintInfo Calculate(TextInfoControlBase control, TextInfoPreliminaryLayoutResult preliminaryHeaderPrintInfo, Rectangle pageBounds, float secondLineTextSizeMultiplier) {
			HeaderTextInfo firstTextLine = null;
			HeaderTextInfo secondTextLine = null;
			double scale = preliminaryHeaderPrintInfo.Scale;
			if (scale > 0) {
				Rectangle actualTextBounds = CalculateActualTextBounds(preliminaryHeaderPrintInfo);
				Rectangle firstLineBounds = CalculateFirstLineBounds(preliminaryHeaderPrintInfo.FirstLineSize, preliminaryHeaderPrintInfo.Scale, actualTextBounds);
				Rectangle secondLineBounds = CalculateSecondLineBounds(preliminaryHeaderPrintInfo.SecondLineSize, preliminaryHeaderPrintInfo.Scale, actualTextBounds, firstLineBounds);
				firstTextLine = new HeaderTextInfo(preliminaryHeaderPrintInfo.FirstLineText, firstLineBounds, scale, font, foreColor, 1.0f);
				secondTextLine = new HeaderTextInfo(preliminaryHeaderPrintInfo.SecondLineText, secondLineBounds, scale, font, foreColor, secondLineTextSizeMultiplier);
			}
			return new TextInfoControlPrintInfo(control, firstTextLine, secondTextLine);
		}
		private Rectangle CalculateFirstLineBounds(SizeF firstLineSize, double scale, Rectangle actualTextBounds) {
			return new Rectangle(actualTextBounds.Left, actualTextBounds.Top, (int)(firstLineSize.Width * scale), (int)(firstLineSize.Height * scale));
		}
		private Rectangle CalculateSecondLineBounds(SizeF secondLineSize, double scale, Rectangle actualTextBounds, Rectangle firstLineBounds) {
			int width = (int)(secondLineSize.Width * scale);
			int height = (int)(secondLineSize.Height * scale);
			int top = AutoScaleFont ? actualTextBounds.Bottom - height : firstLineBounds.Bottom + textVerticalMargin;
			return new Rectangle(actualTextBounds.Left, top, width, height);
		}
		void CalculateTextScaleAndActualSize(TextInfoPreliminaryLayoutResult preliminaryHeaderPrintInfo) {
			Rectangle availableTextBounds = preliminaryHeaderPrintInfo.AvailableTextBounds;
			SizeF firstLineSize = preliminaryHeaderPrintInfo.FirstLineSize;
			SizeF secondLineSize = preliminaryHeaderPrintInfo.SecondLineSize;
			if (AutoScaleFont) {
				double textWidth = Math.Max(firstLineSize.Width, secondLineSize.Width);
				double textHeight = firstLineSize.Height + secondLineSize.Height;
				double scaleX = availableTextBounds.Width / textWidth;
				double scaleY = availableTextBounds.Height / textHeight;
				double scale = Math.Min(scaleX, scaleY);
				preliminaryHeaderPrintInfo.TextWidth = textWidth * scale;
				preliminaryHeaderPrintInfo.TextHeight = textHeight * scale;
				preliminaryHeaderPrintInfo.Scale = scale;
			} else {
				preliminaryHeaderPrintInfo.TextWidth = availableTextBounds.Width;
				preliminaryHeaderPrintInfo.TextHeight = availableTextBounds.Height;
				preliminaryHeaderPrintInfo.Scale = 1;
			}
		}
		private Rectangle CalculateActualTextBounds(TextInfoPreliminaryLayoutResult preliminaryHeaderPrintInfo) {
			double textWidth = preliminaryHeaderPrintInfo.TextWidth;
			double textHeight = preliminaryHeaderPrintInfo.TextHeight;
			Rectangle availableTextBounds = preliminaryHeaderPrintInfo.AvailableTextBounds;
			int top = availableTextBounds.Top;
			if (AutoScaleFont)
				top = availableTextBounds.Top + (int)((availableTextBounds.Height - textHeight) / 2);
			return new Rectangle(availableTextBounds.Left, top, (int)textWidth, (int)textHeight);
		}
	}
	#endregion
}
