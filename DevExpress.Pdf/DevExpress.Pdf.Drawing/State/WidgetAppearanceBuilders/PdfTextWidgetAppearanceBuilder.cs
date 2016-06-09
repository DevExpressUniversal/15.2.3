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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfTextWidgetAppearanceBuilder : PdfWidgetAppearanceBuilder {
		readonly PdfTextFormField textField;
		public override PdfEditorSettings GetEditorAppearanceSettings(PdfAnnotationState annotationState, bool readOnly) {
			return new PdfTextEditSettings(annotationState, textField, FontData, ActualFontSize, readOnly);
		}
		public PdfTextWidgetAppearanceBuilder(PdfTextFormField field, PdfDocumentState documentState)
			: base(field, documentState) {
			textField = field;
			CalculateActualFontSize(Annotation.ContentRectangle, field.Text);
		}
		protected override void FillMarkedContent(IList<PdfCommand> markedContentCommands) {
			PdfRectangle contentRectangle = Annotation.ContentRectangle;
			CalculateActualFontSize(contentRectangle, textField.Text);
			if (textField.Text != null) {
				int maxLen = textField.MaxLen.HasValue ? textField.MaxLen.Value : 0;
				if (textField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Comb) && maxLen != 0) {
					double yOffset = CalculateCenteredLineYOffset(contentRectangle);
					StartDrawTextBox(markedContentCommands, null);
					double step = contentRectangle.Width / maxLen;
					char[] chars = textField.Text.ToCharArray();
					int length = chars.Length;
					double previousCharWidth = 0;
					for (int i = 0; i < maxLen && i < length; i++) {
						string ch = chars[i].ToString();
						double charWidth = CalculateWidth(ActualFontSize, ch);
						if (i == 0)
							DrawTextBoxText(markedContentCommands, (step - charWidth) / 2, yOffset, ch);
						else
							DrawTextBoxText(markedContentCommands, step + (previousCharWidth - charWidth) / 2, 0, ch);
						previousCharWidth = charWidth;
					}
					EndDrawTextBox(markedContentCommands);
				}
				else {
					PdfEditableFontData fontData = FontData;
					PdfFont font = fontData.PdfFont;
					RemoveFontFromStorage(font);
					double lineSpacing = fontData.Metrics.GetLineSpacing(ActualFontSize);
					PdfCommandConstructor constructor = new PdfCommandConstructor(markedContentCommands, textField.Widget.DefaultForm.Resources);
					PdfWidgetStringPainter painter = new PdfWidgetStringPainter(constructor, fontData.Mapper.MapCharacter(' '));
					PdfInteractiveFormFieldTextState textState = textField.TextState;
					double actualFontSize = ActualFontSize;
					if (textState != null) {
						painter.SetCharSpacing(textState.CharacterSpacing);
						painter.SetHorizontalScaling(textState.HorizontalScaling);
						painter.SetWordSpacing(textState.WordSpacing);
						textState.FillCommands(markedContentCommands);
					}
					PdfStringFormatter formatter = new PdfStringFormatter(fontData, actualFontSize);
					bool singleLine = !textField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Multiline);
					PdfStringFormat format = GetStringFormat(singleLine);
					const double offsetFactor = 0.55;
					if (!singleLine)
						contentRectangle = new PdfRectangle(contentRectangle.Left, contentRectangle.Bottom, contentRectangle.Right, contentRectangle.Top - lineSpacing * offsetFactor);
					IList<PdfStringGlyphRun> lines = formatter.FormatString(textField.Text, contentRectangle, format, true);
					if (singleLine && lines.Count > 1)
						lines = new List<PdfStringGlyphRun>() { lines[0] };
					painter.DrawLines(lines, fontData, actualFontSize, contentRectangle, format, true);
					fontData.UpdateFont();
				}
			}
		}
		PdfStringFormat GetStringFormat(bool singleLine) {
			PdfStringFormat format = new PdfStringFormat(PdfStringFormatFlags.NoClip);
			format.LeadingMarginFactor = 0;
			format.TrailingMarginFactor = 0;
			format.Trimming = PdfStringTrimming.None;
			switch (textField.TextJustification) {
				case PdfTextJustification.Centered:
					format.Alignment = PdfStringAlignment.Center;
					break;
				case PdfTextJustification.LeftJustified:
					format.Alignment = PdfStringAlignment.Near;
					break;
				case PdfTextJustification.RightJustified:
					format.Alignment = PdfStringAlignment.Far;
					break;
			}
			if (singleLine) {
				format.FormatFlags |= PdfStringFormatFlags.NoWrap;
				format.LineAlignment = PdfStringAlignment.Center;
			}
			return format;
		}
	}
}
