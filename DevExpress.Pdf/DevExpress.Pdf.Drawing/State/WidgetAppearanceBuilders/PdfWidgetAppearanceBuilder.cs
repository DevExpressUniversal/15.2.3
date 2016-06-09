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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfWidgetAppearanceBuilder : PdfDisposableObject {
		const double minFontSize = 4;
		static PdfWidgetAppearanceBuilder CreateBuilder(PdfInteractiveFormField field, PdfDocumentState documentState) {
			PdfChoiceFormField choice = field as PdfChoiceFormField;
			if (choice != null)
				return new PdfChoiceWidgetAppearanceBuilder(choice, documentState);
			PdfTextFormField text = field as PdfTextFormField;
			if (text != null)
				return new PdfTextWidgetAppearanceBuilder(text, documentState);
			return null;
		}
		static PdfFormData Create(PdfInteractiveFormField field, IDictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder> controllers, PdfDocumentState documentState, PdfFormData parentFormData) {
			PdfButtonFormField button = field as PdfButtonFormField;
			if (button != null && button.Flags.HasFlag(PdfInteractiveFormFieldFlags.PushButton))
				return null;
			if (field.Name == null) {
				PdfWidgetAppearanceBuilder controller = CreateBuilder(field, documentState);
				if (controller != null) {
					if (!controllers.ContainsKey(field.Widget))
						controllers.Add(field.Widget, controller);
					if (parentFormData != null)
						parentFormData.AddAppearanceBuilder(controller.RebuildAppearance);
				}
				return null;
			}
			PdfFormData formData = new PdfFormData(field);
			if (field.Kids != null && button == null) {
				foreach (PdfInteractiveFormField child in field.Kids) {
					PdfFormData childFormData = Create(child, controllers, documentState, formData);
					if (childFormData != null)
						formData.AddKid(child.Name, childFormData);
				}
			}
			else {
				PdfWidgetAppearanceBuilder controller = CreateBuilder(field, documentState);
				if (controller != null) {
					if (!controllers.ContainsKey(field.Widget))
						controllers.Add(field.Widget, controller);
					formData.AddAppearanceBuilder(controller.RebuildAppearance);
				}
			}
			return formData;
		}
		public static PdfFormData CreateFormDataWithControllers(PdfDocumentState documentState, IDictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder> controllers) {
			PdfDocument document = documentState.Document;
			if (document.AcroForm == null)
				return new PdfFormData(false);
			PdfFormData root = new PdfFormData();
			foreach (PdfInteractiveFormField field in document.AcroForm.Fields) {
				PdfFormData formData = Create(field, controllers, documentState, null);
				if (formData != null)
					root[field.Name] = formData;
			}
			root.AllowAddNewKids = false;
			return root;
		}
		readonly PdfDocumentState documentState;
		readonly PdfInteractiveFormField field;
		readonly PdfEmbeddedEditableFontData fontData;
		readonly double fontSize;
		double actualFontSize;
		protected PdfWidgetAnnotation Annotation { get { return field.Widget; } }
		protected PdfInteractiveFormField FormField { get { return Annotation.InteractiveFormField; } }
		protected PdfEmbeddedEditableFontData FontData { get { return fontData; } }
		protected double FontSize { get { return fontSize; } }
		protected double ActualFontSize {
			get { return actualFontSize; }
			set { actualFontSize = value; }
		}
		public void RebuildAppearance() {
			PdfWidgetAnnotation widget = Annotation;
			PdfForm defaultAppearanceForm = widget.DefaultForm;
			widget.UpdateTransformation();
			IList<PdfCommand> commands = defaultAppearanceForm.Commands;
			commands.Clear();
			commands.Add(new PdfSaveGraphicsStateCommand());
			FillRectangle(commands, new PdfRectangle(0, 0, widget.Rect.Width, widget.Rect.Height), widget.BackgroundColor);
			DrawBorder(commands);
			commands.Add(new PdfRestoreGraphicsStateCommand());
			PdfMarkedContentCommand markedContentCommand = new PdfMarkedContentCommand("Tx");
			IList<PdfCommand> markedContentCommands = markedContentCommand.Children;
			markedContentCommands.Clear();
			markedContentCommands.Add(new PdfSaveGraphicsStateCommand());
			PdfRectangle contentRectangle = widget.ContentRectangle;
			markedContentCommands.Add(new PdfAppendRectangleCommand(contentRectangle.Left, contentRectangle.Bottom, contentRectangle.Width, contentRectangle.Height));
			markedContentCommands.Add(new PdfModifyClippingPathUsingNonzeroWindingNumberRuleCommand());
			markedContentCommands.Add(new PdfEndPathWithoutFillingAndStrokingCommand());
			FillMarkedContent(markedContentCommands);
			markedContentCommands.Add(new PdfRestoreGraphicsStateCommand());
			commands.Add(markedContentCommand);
			defaultAppearanceForm.InvalidateStream();
		}
		public void EnsureAppearance() {
			PdfWidgetAnnotation widget = Annotation;
			PdfAnnotationAppearances appearance = widget == null ? null : widget.Appearance;
			bool needAppearances = field != null && field.Form.NeedAppearances;
			if (needAppearances || appearance == null || appearance.Normal == null || appearance.Normal.DefaultForm == null || appearance.Normal.DefaultForm.Commands.Count == 0)
				RebuildAppearance();
		}
		public abstract PdfEditorSettings GetEditorAppearanceSettings(PdfAnnotationState annotationState, bool readOnly);
		protected PdfWidgetAppearanceBuilder(PdfInteractiveFormField field, PdfDocumentState documentState) {
			this.documentState = documentState;
			this.field = field;
			PdfFontSearch fontSearch = documentState == null ? new PdfFontSearch() : documentState.FontSearch;
			try {
				PdfSetTextFontCommand fontCommand = FormField.TextState.FontCommand;
				fontSize = FormField.TextState.FontSize;
				if (fontCommand != null)
					fontData = fontSearch.Search(fontCommand.Font);
				else
					fontData = fontSearch.Search(null);
				actualFontSize = fontSize == 0 ? PdfInteractiveFormFieldTextState.DefaultFontSize : fontSize;
			}
			finally {
				if (fontSearch != null && documentState.FontSearch == null)
					fontSearch.Dispose();
			}
		}
		protected double CalculateCenteredLineYOffset(PdfRectangle clipRect) {
			PdfEditableFontData fontData = FontData;
			double fontSize = ActualFontSize;
			PdfFontMetrics metrics = fontData.Metrics;
			double baseLine = metrics.GetDescent(fontSize);
			double yOffset = (clipRect.Height - metrics.GetLineSpacing(fontSize)) / 2 + baseLine;
			return clipRect.Bottom + yOffset;
		}
		protected double CalculateWidth(double fontSize, string text) {
			if (fontSize <= 0)
				return 0;
			PdfInteractiveFormFieldTextState textState = FormField.TextState;
			PdfGraphicsMeasuringContext context = new PdfGraphicsMeasuringContext(fontData, fontSize);
			context.SetCharSpacing((float)textState.CharacterSpacing);
			context.SetWordSpacing((float)textState.WordSpacing);
			context.SetHorizontalScaling((int)(textState.HorizontalScaling));
			return context.GetTextWidth(text ?? String.Empty);
		}
		protected double CalculateFirstLineXOffset(PdfRectangle clipRect, string text) {
			double textWidth = CalculateWidth(actualFontSize, text);
			switch (FormField.TextJustification) {
				case PdfTextJustification.LeftJustified:
					return clipRect.Left + 1;
				case PdfTextJustification.Centered:
					return clipRect.Left + clipRect.Width / 2 - textWidth / 2;
				case PdfTextJustification.RightJustified:
					return clipRect.Right - textWidth - 1;
			}
			return 0;
		}
		protected void DrawTextBoxText(IList<PdfCommand> commands, double xOffset, double yOffset, string text) {
			commands.Add(new PdfStartTextLineWithOffsetsCommand(xOffset, yOffset));
			PdfGlyphMappingResult mapping = fontData.Mapper.MapString(text, false);
			fontData.RegisterString(mapping.Mapping);
			commands.Add(new PdfShowTextCommand(mapping.GlyphRun.TextData));
			fontData.UpdateFont();
		}
		protected void EndDrawTextBox(IList<PdfCommand> commands) {
			commands.Add(new PdfEndTextCommand());
		}
		protected void RemoveFontFromStorage(PdfFont font) {
			documentState.FontStorage.DeleteFont(font);
		}
		protected void StartDrawTextBox(IList<PdfCommand> commands, PdfColor foreColor) {
			PdfEditableFontData fontData = FontData;
			PdfFont font = fontData.PdfFont;
			Annotation.DefaultForm.Resources.AddFont(font);
			RemoveFontFromStorage(font);
			commands.Add(new PdfBeginTextCommand());
			if (foreColor != null) {
				double[] components = (double[])foreColor.Components.Clone();
				switch (components.Length) {
					case 1:
						commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(components[0]));
						break;
					case 3:
						commands.Add(new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(components[0], components[1], components[2]));
						break;
					case 4:
						commands.Add(new PdfSetCMYKColorSpaceForNonStrokingOperationsCommand(components[0], components[1], components[2], components[3]));
						break;
				}
			}
			else {
				FormField.TextState.FillCommands(commands);
			}
			commands.Add(new PdfSetTextFontCommand(font, actualFontSize));
		}
		protected void DrawTextBox(IList<PdfCommand> commands, PdfRectangle clipRect, string text, PdfColor forecolor) {
			CalculateActualFontSize(clipRect, text);
			StartDrawTextBox(commands, forecolor);
			double xOffset = CalculateFirstLineXOffset(clipRect, text);
			double yOffset = CalculateCenteredLineYOffset(clipRect);
			DrawTextBoxText(commands, xOffset, yOffset, text);
			EndDrawTextBox(commands);
		}
		protected void DrawTextBox(IList<PdfCommand> commands, PdfRectangle clipRect, string text) {
			DrawTextBox(commands, clipRect, text, null);
		}
		protected void FillRectangle(IList<PdfCommand> commands, PdfRectangle rect, PdfColor color) {
			if (color != null) {
				PdfRGBColor rgbColor = PdfRGBColor.FromColor(color);
				commands.Add(new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(rgbColor.R, rgbColor.G, rgbColor.B));
				commands.Add(new PdfAppendRectangleCommand(rect.Left, rect.Bottom, rect.Width, rect.Height));
				commands.Add(new PdfFillPathUsingNonzeroWindingNumberRuleCommand());
			}
		}
		protected void CalculateActualFontSize(PdfRectangle rect, string text) {
			const double heightMultiplier = 0.75; 
			if (FontSize == 0) {
				double calculatedFontSize = rect.Height * heightMultiplier;
				double width = CalculateWidth(calculatedFontSize, text);
				double step = rect.Height / 2;
				if (width > rect.Width)
					while (width > rect.Width || width < rect.Width - 2) {
						if (width > rect.Width)
							calculatedFontSize -= step;
						if (width < rect.Width)
							calculatedFontSize += step;
						step /= 2;
						width = CalculateWidth(calculatedFontSize, text);
					}
				if (calculatedFontSize < minFontSize)
					ActualFontSize = minFontSize;
				else
					ActualFontSize = calculatedFontSize;
			}
			else
				ActualFontSize = FontSize;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				fontData.Dispose();
			}
		}
		protected abstract void FillMarkedContent(IList<PdfCommand> markedContentCommands);
		void AppendBorderRectangle(IList<PdfCommand> commands, double borderWidth) {
			PdfWidgetAnnotation widget = Annotation;
			double halfWidth = borderWidth / 2;
			commands.Add(new PdfAppendRectangleCommand(halfWidth, halfWidth, widget.Rect.Width - borderWidth, widget.Rect.Height - borderWidth));
			commands.Add(new PdfCloseAndStrokePathCommand());
		}
		void AppendUpperLeftStroke(IList<PdfCommand> commands, double borderWidth) {
			double doubleBorderWidth = 2 * borderWidth;
			PdfRectangle rect = Annotation.Rect;
			commands.Add(new PdfBeginPathCommand(new PdfPoint(borderWidth, borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(borderWidth, rect.Height - borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - borderWidth, rect.Height - borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - doubleBorderWidth, rect.Height - doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(doubleBorderWidth, rect.Height - doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(doubleBorderWidth, doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(borderWidth, borderWidth)));
			commands.Add(new PdfFillPathUsingNonzeroWindingNumberRuleCommand());
		}
		void AppendBottomRightStroke(IList<PdfCommand> commands, double borderWidth) {
			double doubleBorderWidth = 2 * borderWidth;
			PdfRectangle rect = Annotation.Rect;
			commands.Add(new PdfBeginPathCommand(new PdfPoint(borderWidth, borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - borderWidth, borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - borderWidth, rect.Height - borderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - doubleBorderWidth, rect.Height - doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width - doubleBorderWidth, doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(doubleBorderWidth, doubleBorderWidth)));
			commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(borderWidth, borderWidth)));
			commands.Add(new PdfFillPathUsingNonzeroWindingNumberRuleCommand());
		}
		void DrawBorder(IList<PdfCommand> commands) {
			PdfWidgetAnnotation widget = Annotation;
			PdfColor borderColor = null;
			if (widget.AppearanceCharacteristics != null) {
				borderColor = widget.AppearanceCharacteristics.BorderColor;
				if (borderColor != null) {
					PdfRGBColor rgbBorderColor = PdfRGBColor.FromColor(borderColor);
					commands.Add(new PdfSetRGBColorSpaceForStrokingOperationsCommand(rgbBorderColor.R, rgbBorderColor.G, rgbBorderColor.B));
					if (widget.BorderStyle != null) {
						PdfAnnotationBorderStyle borderStyle = widget.BorderStyle;
						double borderWidth = borderStyle.Width;
						double halfBorderWidth = borderWidth / 2;
						commands.Add(new PdfSetLineWidthCommand(borderWidth));
						PdfRectangle rect = widget.Rect;
						switch (borderStyle.StyleName) {
							case "D":
								commands.Add(new PdfSetLineStyleCommand(borderStyle.LineStyle));
								AppendBorderRectangle(commands, borderWidth);
								break;
							case "B":
								AppendBorderRectangle(commands, borderWidth);
								commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(1.0));
								AppendUpperLeftStroke(commands, borderWidth);
								commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(0.5));
								AppendBottomRightStroke(commands, borderWidth);
								break;
							case "I":
								AppendBorderRectangle(commands, borderWidth);
								commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(0.5));
								AppendUpperLeftStroke(commands, borderWidth);
								commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(0.75));
								AppendBottomRightStroke(commands, borderWidth);
								break;
							case "U":
								commands.Add(new PdfBeginPathCommand(new PdfPoint(0, halfBorderWidth)));
								commands.Add(new PdfAppendLineSegmentCommand(new PdfPoint(rect.Width, halfBorderWidth)));
								commands.Add(new PdfStrokePathCommand());
								break;
							default:
								AppendBorderRectangle(commands, borderWidth);
								break;
						}
					}
					else if (widget.Border != null) {
						PdfAnnotationBorder border = widget.Border;
						double lineWidth = border.LineWidth;
						commands.Add(new PdfSetLineWidthCommand(lineWidth));
						if (border.LineStyle != null)
							commands.Add(new PdfSetLineStyleCommand(border.LineStyle));
						AppendBorderRectangle(commands, lineWidth);
					}
				}
			}
		}
	}
}
