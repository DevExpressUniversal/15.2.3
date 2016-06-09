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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfEditorSettings {
		protected static Color ToGDIPlusColor(PdfColor color) {
			PdfRGBColor rgbColor = PdfRGBColor.FromColor(color);
			return Color.FromArgb(255, Convert.ToByte(rgbColor.R * 255), Convert.ToByte(rgbColor.G * 255), Convert.ToByte(rgbColor.B * 255));
		}
		readonly PdfDocumentArea documentArea;
		readonly Color backgroundColor;
		readonly Color fontColor;
		readonly PdfTextJustification textJustification;
		readonly PdfEditableFontData fontData;
		readonly double fontSize;
		readonly bool usePasswordChar;
		readonly PdfEditorBorder border;
		readonly bool readOnly;
		public PdfDocumentArea DocumentArea { get { return documentArea; } }
		public Color BackgroundColor { get { return backgroundColor; } }
		public Color FontColor { get { return fontColor; } }
		public PdfTextJustification TextJustification { get { return textJustification; } }
		public PdfEditableFontData FontData { get { return fontData; } }
		public double FontSize { get { return fontSize; } }
		public bool UsePasswordChar { get { return usePasswordChar; } }
		public PdfEditorBorder Border { get { return border; } }
		public abstract PdfEditorType EditorType { get; }
		public abstract object EditValue { get; }
		public bool ReadOnly { get { return readOnly; } }
		protected PdfEditorSettings(PdfAnnotationState state, PdfInteractiveFormField field, PdfEditableFontData fontData, double fontSize, bool readOnly) {
			documentArea = new PdfDocumentArea(state.PageIndex + 1, state.Rectangle);
			if (field != null) {
				PdfWidgetAnnotation widget = field.Widget;
				PdfColor pdfBackgroundColor = widget.BackgroundColor;
				backgroundColor = pdfBackgroundColor == null ? Color.White : ToGDIPlusColor(pdfBackgroundColor);
				border = new PdfEditorBorder(widget);
				textJustification = field.TextJustification;
				IEnumerable<PdfCommand> appearanceCommands = field.AppearanceCommands;
				fontColor = Color.Black;
				if (appearanceCommands != null) {
					foreach (PdfCommand command in appearanceCommands) {
						PdfSetColorCommand setColorCommand = command as PdfSetColorCommand;
						if (setColorCommand != null) {
							fontColor = ToGDIPlusColor(new PdfColor((double[])setColorCommand.Components.Clone()));
							break;
						}
						PdfSetRGBColorSpaceCommand rgbCommand = command as PdfSetRGBColorSpaceCommand;
						if (rgbCommand != null) {
							fontColor = ToGDIPlusColor(new PdfColor(rgbCommand.R, rgbCommand.G, rgbCommand.B));
							break;
						}
						PdfSetGrayColorSpaceCommand grayCommand = command as PdfSetGrayColorSpaceCommand;
						if (grayCommand != null) {
							fontColor = ToGDIPlusColor(new PdfColor(grayCommand.Gray));
							break;
						}
						PdfSetCMYKColorSpaceCommand cmykCommand = command as PdfSetCMYKColorSpaceCommand;
						if (grayCommand != null) {
							fontColor = ToGDIPlusColor(new PdfColor(cmykCommand.C, cmykCommand.M, cmykCommand.Y, cmykCommand.K));
							break;
						}
					}
				}
				this.fontData = fontData;
				this.fontSize = fontSize;
				usePasswordChar = field.Flags.HasFlag(PdfInteractiveFormFieldFlags.Password);
				this.readOnly = readOnly || field.Flags.HasFlag(PdfInteractiveFormFieldFlags.ReadOnly) || (field.Widget != null && field.Widget.Flags.HasFlag(PdfAnnotationFlags.ReadOnly));
			}
		}
	}
}
