#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
namespace DevExpress.DashboardCommon.Viewer {
	public interface ICardStringMeasurer {
		int GetStringHeight(string text, Font font, int width, StringFormat format);
		bool IsStringFit(string text, Font font, int width, StringFormat format);
	}
	public class CardPresenter : IDisposable {
		public static Rectangle RecalculateRect(Rectangle bounds, RectangleF rect) {
			int width = bounds.Width;
			int height = bounds.Height;
			return new Rectangle(
				(int)(bounds.Left + width * rect.Left),
				(int)(bounds.Top + height * rect.Top),
				(int)(width * rect.Width),
				(int)(height * rect.Height)
			);
		}
		readonly static RectangleF MainTitleTemplate = new RectangleF(0F, 0.02F, 1F, 0.3F);
		readonly static RectangleF SubTitleTemplate = new RectangleF(0F, 0.3F, 0.6F, 0.21F);
		readonly static RectangleF SubTitleAdditionalTemplate = new RectangleF(0F, 0.475F, 0.6F, 0.21F);
		readonly static RectangleF SubValue1Template = new RectangleF(0.6F, 0.3F, 0.4F, 0.21F);
		readonly static RectangleF SubValue2Template = new RectangleF(0.6F, 0.475F, 0.4F, 0.21F);
		readonly static RectangleF MainValueTemplate = new RectangleF(0.175F, 0.6F, 0.825F, 0.35F);
		readonly static RectangleF ImageTemplate = new RectangleF(0F, 0.7F, 0.165F, 0.2475F);
		Rectangle imageBox;
		IndicatorPresenter indicatorPresenter;
		TextPropertiesCreator mainTitleProps = new TextPropertiesCreator();
		TextPropertiesCreator subTitleProps = new TextPropertiesCreator();
		TextPropertiesCreator subValue1Props = new TextPropertiesCreator();
		TextPropertiesCreator subValue2Props = new TextPropertiesCreator();
		TextPropertiesCreator mainValueProps = new TextPropertiesCreator();
		Color mainTextColor;
		Color subTextColor;
		Color neutralColor;
		Color goodColor;
		Color badColor;
		Color warningColor;
		Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();
		public void Initialize(CardStyleProperties styleProperties) {
			indicatorPresenter = new IndicatorPresenter(styleProperties.Good, styleProperties.Bad, styleProperties.Warning);
			DisposeBrushes();
			mainTextColor = styleProperties.ActualValueColor;
			subTextColor = styleProperties.SubTextColor;
			neutralColor = styleProperties.Neutral;
			goodColor = styleProperties.Good;
			warningColor = styleProperties.Warning;
			badColor = styleProperties.Bad;
			Color[] colors = new[] { mainTextColor, subTextColor, neutralColor, goodColor, warningColor, badColor };
			foreach(Color color in colors) {
				if(!brushes.ContainsKey(color))
					brushes.Add(color, new SolidBrush(color));
			}
			StringFormat stringFormatNear = new StringFormat();
			stringFormatNear.Alignment = StringAlignment.Near;
			stringFormatNear.FormatFlags = StringFormatFlags.NoWrap;
			stringFormatNear.LineAlignment = StringAlignment.Center;
			stringFormatNear.Trimming = StringTrimming.EllipsisCharacter;
			StringFormat stringFormatFar = new StringFormat(stringFormatNear);
			stringFormatFar.Alignment = StringAlignment.Far;
			mainTitleProps.Initialize(MainTitleTemplate, styleProperties.MainTitleHeight, stringFormatNear);
			subTitleProps.Initialize(SubTitleTemplate, styleProperties.SubTitleHeight, stringFormatNear, SubTitleAdditionalTemplate);
			subValue1Props.Initialize(SubValue1Template, styleProperties.SubValue1Height, stringFormatFar);
			subValue2Props.Initialize(SubValue2Template, styleProperties.SubValue2Height, stringFormatFar);
			mainValueProps.Initialize(MainValueTemplate, styleProperties.MainValueHeight, stringFormatFar);
		}
		public CardDrawProperties GetDrawPropertiesCollection(ICardStringMeasurer measurer, Rectangle bounds, CardModel card) {
			return GetDrawPropertiesCollection(measurer, bounds, card, null); 
		}
		public CardDrawProperties GetDrawPropertiesCollection(ICardStringMeasurer measurer, Rectangle bounds, CardModel card, string fontName) {
			CardData data = card.Data;
			CardViewModel cardViewModel = card.ViewModel;
			Color variationColor = GetVariationBrush(data.IndicatorType, data.IsGood);
			CardDrawProperties cardDrawProperties = new CardDrawProperties();
			cardDrawProperties.TitleProperties = mainTitleProps.CreateTextDrawProperties(measurer, card.Title, mainTextColor, bounds, fontName);
			cardDrawProperties.SubTitleProperties = subTitleProps.CreateTextDrawProperties(measurer, card.SubTitle, subTextColor, bounds, fontName);
			cardDrawProperties.SubValue1Properties = subValue1Props.CreateTextDrawProperties(measurer, card.SubValue1, cardViewModel.IgnoreSubValue1DeltaColor ? mainTextColor : variationColor, data.SingleValue ? Rectangle.Empty : bounds, fontName);
			cardDrawProperties.SubValue2Properties = subValue2Props.CreateTextDrawProperties(measurer, card.SubValue2, cardViewModel.IgnoreSubValue2DeltaColor ? mainTextColor : variationColor, data.SingleValue ? Rectangle.Empty : bounds, fontName);
			cardDrawProperties.MainValueProperties = mainValueProps.CreateTextDrawProperties(measurer, card.MainValue, cardViewModel.IgnoreDeltaColor ? mainTextColor : variationColor, bounds, fontName);
			imageBox = RecalculateRect(bounds, ImageTemplate);
			Image image = GetVariationImage(imageBox.Size, data.IndicatorType, data.IsGood);
			if(image != null) {
				imageBox.Width = image.Width;
				imageBox.Height = image.Height;
				cardDrawProperties.ImageDrawProperties = new ImageDrawProperties(image, imageBox);
			}
			return cardDrawProperties;
		}
		public Brush GetBrush(Color color) {
			return brushes[color];
		}
		void DisposeBrushes() {
			brushes.Clear();
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				DisposeBrushes();
				if(mainTitleProps != null) {
					mainTitleProps.Dispose();
					mainTitleProps = null;
				}
				if(subTitleProps != null) {
					subTitleProps.Dispose();
					subTitleProps = null;
				}
				if(subValue1Props != null) {
					subValue1Props.Dispose();
					subValue1Props = null;
				}
				if(subValue2Props != null) {
					subValue2Props.Dispose();
					subValue2Props = null;
				}
				if(mainValueProps != null) {
					mainValueProps.Dispose();
					mainValueProps = null;
				}
			}
		}
		Color GetVariationBrush(IndicatorType indicatorType, bool isGood) {
			switch(indicatorType) {
				case IndicatorType.Warning:
					return warningColor;
				case IndicatorType.DownArrow:
				case IndicatorType.UpArrow:
					return isGood ? goodColor : badColor;
				default:
					return neutralColor;
			}
		}
		Image GetVariationImage(Size size, IndicatorType type, bool isGood) {
			if(size.Width == 0 || size.Height == 0 || type == IndicatorType.None)
				return null;
			return indicatorPresenter.GetImage(size, type, isGood);
		}
	}
	class TextPropertiesCreator : IDisposable {
		string baseFontName = AppearanceObject.DefaultFont.FontFamily.Name;
		Font font = null;
		StringFormat format = new StringFormat();
		RectangleF templateRect;
		RectangleF additionalTemplateRect;
		public void Initialize(RectangleF template, float height, StringFormat format) {
			Initialize(template, height, format, RectangleF.Empty);
		}
		public void Initialize(RectangleF template, float height, StringFormat format, RectangleF additionalTemplateRect) {
			this.templateRect = template;
			this.templateRect.Height = height;
			this.format = format;
			this.additionalTemplateRect = additionalTemplateRect;
		}
		Font GetFont(int fontSize) {
			if(font != null){
				if(font.Size == fontSize)
					return font;
				font.Dispose();
			}
			return new Font(baseFontName, fontSize > 0 ? fontSize : 1, FontStyle.Regular);
		}
		public TextDrawProperties CreateTextDrawProperties(ICardStringMeasurer measurer, string text, Color color, Rectangle parentBounds, string fontName) {
			if(parentBounds.IsEmpty)
				return null;
			if(!String.IsNullOrWhiteSpace(fontName))
				baseFontName = fontName;
			Rectangle bounds = CardPresenter.RecalculateRect(parentBounds, templateRect);
			int fontSize = DashboardStringHelper.GetFontSizeByLineHeight(baseFontName, bounds.Height);
			Font font = GetFont(fontSize);
			bounds = CorrectBounds(bounds, font, text, format, measurer);
			if(!additionalTemplateRect.IsEmpty) {
				List<string> subTitleStrings = WrapString(text, font, bounds.Width, format, measurer);
				if(subTitleStrings.Count > 1) {
					Rectangle additionalBounds = CardPresenter.RecalculateRect(parentBounds, additionalTemplateRect);
					additionalBounds = CorrectBounds(additionalBounds, font, subTitleStrings[1], format, measurer);
					TextDrawProperties additionalProperties = new TextDrawProperties(subTitleStrings[1], additionalBounds, font, color, format);
					StringFormat noTrimFormat = new StringFormat(format);
					noTrimFormat.Trimming = StringTrimming.None;
					return new TextDrawProperties(subTitleStrings[0], bounds, font, color, noTrimFormat, additionalProperties);
				}
			}
			return new TextDrawProperties(text, bounds, font, color, format);
		}
		Rectangle CorrectBounds(Rectangle bounds, Font font, string text, StringFormat format, ICardStringMeasurer measurer) {
			int newHeight = measurer.GetStringHeight(text, font, bounds.Width, format);
			bounds.Y -= (newHeight - bounds.Height) / 2;
			bounds.Height = newHeight;
			return bounds;
		}
		List<string> WrapString(string text, Font font, int rectWidth, StringFormat format, ICardStringMeasurer measurer) {
			List<string> resultLines = new List<string>();
			char[] wordDelimeters = { ' ' };
			string[] words = text.Split(wordDelimeters);
			int wordsCount = words.Length;
			int i = 0;
			string measuredString = String.Empty;
			do {
				string currentString = measuredString;
				currentString = !String.IsNullOrWhiteSpace(currentString) ?  String.Join(" ", currentString, words[i]) : words[i] ;
				bool isFit = measurer.IsStringFit(currentString, font, rectWidth, format);
				if(!isFit)
					break;
				measuredString = currentString;
				i++;
			}
			while(i < wordsCount);
			if(String.IsNullOrWhiteSpace(measuredString)) {
				resultLines.Add(text);
				return resultLines;
			}
			resultLines.Add(measuredString);
			string additionalString = String.Join(" ", words, i, wordsCount - i);
			if(!String.IsNullOrWhiteSpace(additionalString))
				resultLines.Add(additionalString);
			return resultLines;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(format != null) {
					format.Dispose();
					format = null;
				}
			}
		}
	}
}
