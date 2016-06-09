#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf {
	public abstract class PdfShapeAnnotation : PdfMarkupAnnotation {
		const string interiorColorDictionaryKey = "IC";
		readonly PdfAnnotationBorderStyle borderStyle;
		readonly PdfColor interiorColor;
		readonly PdfAnnotationBorderEffect borderEffect;
		readonly PdfRectangle padding;
		public PdfAnnotationBorderStyle BorderStyle { get { return borderStyle; } }
		public PdfColor InteriorColor { get { return interiorColor; } }
		public PdfAnnotationBorderEffect BorderEffect { get { return borderEffect; } }
		public PdfRectangle Padding { get { return padding; } }
		protected PdfShapeAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			borderStyle = PdfAnnotationBorderStyle.Parse(dictionary);
			interiorColor = PdfAnnotation.ParseColor(dictionary, interiorColorDictionaryKey);
			borderEffect = PdfAnnotationBorderEffect.Parse(dictionary);
			padding = dictionary.GetPadding(Rect);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(PdfAnnotationBorderStyle.DictionaryKey, borderStyle);
			if (interiorColor != null)
				dictionary.Add(interiorColorDictionaryKey, interiorColor.ToWritableObject());
			if (borderEffect != null)
				dictionary.Add(PdfAnnotationBorderEffect.DictionaryKey, borderEffect.ToWritableObject());
			dictionary.Add(PdfDictionary.DictionaryPaddingKey, padding);
			return dictionary;
		}
		protected override void GenerateAppearanceCommands(IList<PdfCommand> commands) {
			base.GenerateAppearanceCommands(commands);
			bool hasBorder;
			bool hasInterior;
			double opacity = Opacity;
			if (opacity != 1)
				commands.Add(new PdfSetGraphicsStateParametersCommand(new PdfGraphicsStateParameters() { NonStrokingAlphaConstant = opacity, StrokingAlphaConstant = opacity }));
			PdfColor color = Color;
			if (color == null)
				hasBorder = false;
			else {
				hasBorder = true;
				double[] colorComponents = color.Components;
				commands.Add(new PdfSetRGBColorSpaceForStrokingOperationsCommand(colorComponents[0], colorComponents[1], colorComponents[2]));
			}
			color = InteriorColor;
			if (color == null)
				hasInterior = false;
			else {
				hasInterior = true;
				double[] colorComponents = color.Components;
				commands.Add(new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(colorComponents[0], colorComponents[1], colorComponents[2]));
			}
			PdfRectangle rect = Rect;
			double left = 0.5;
			double bottom = 0.5;
			double right = rect.Width - 0.5;
			double top = rect.Height - 0.5;
			PdfRectangle padding = Padding;
			if (padding != null) {
				left += padding.Left;
				bottom += padding.Bottom;
				right -= padding.Right;
				top -= padding.Top;
			}
			if (hasBorder) {
				PdfAnnotationBorderStyle borderStyle = BorderStyle;
				if (borderStyle != null) {
					double borderWidth = borderStyle.Width;
					commands.Add(new PdfSetLineWidthCommand(borderWidth));
					if (borderStyle.StyleName == "D")
						commands.Add(new PdfSetLineStyleCommand(borderStyle.LineStyle));
					if (borderWidth < rect.Width && borderWidth < rect.Height) {
						double borderOffset = borderWidth / 2;
						if (borderOffset > 0) {
							left += borderOffset;
							bottom += borderOffset;
							right -= borderOffset;
							top -= borderOffset;
						}
					}
					else {
						left = right = (left + right) / 2;
						bottom = top = (bottom + top) / 2;
					}
				}
			}
			GenerateShapeCommands(commands, new PdfRectangle(left, bottom, right, top));
			if (hasBorder)
				commands.Add(hasInterior ? (PdfCommand)new PdfFillAndStrokePathUsingNonzeroWindingNumberRuleCommand() : (PdfCommand)new PdfStrokePathCommand());
			else if (hasInterior)
				commands.Add(new PdfFillPathUsingNonzeroWindingNumberRuleCommand());
		}
		protected abstract void GenerateShapeCommands(IList<PdfCommand> commands, PdfRectangle rect);
	}
}
