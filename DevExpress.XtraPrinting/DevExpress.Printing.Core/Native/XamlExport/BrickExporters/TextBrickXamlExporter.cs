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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	class TextBrickXamlExporter : VisualBrickXamlExporter {
		protected override void WriteBrickToXamlCore(XamlWriter writer, VisualBrick brick, XamlExportContext exportContext) {
			TextBrick textBrick = brick as TextBrick;
			if(textBrick == null) {
				throw new ArgumentException("brick");
			}
			writer.WriteStartElement(XamlTag.TextBlock);
			Page page = exportContext.Page;
			PrintingSystemBase ps = (page != null) ? page.Document.PrintingSystem : null;
			writer.WriteAttribute(XamlAttribute.Text, GetText(textBrick, ps, page, exportContext.TextMeasurementSystem));
			string textBlockStyleName = exportContext.ResourceMap.TextBlockStylesDictionary[textBrick];
			writer.WriteAttribute(XamlAttribute.Style, string.Format(staticResourceFormat, textBlockStyleName));
#if !SL
			var labelBrick = textBrick as LabelBrick;
			if(labelBrick != null) {
				if(exportContext.TextMeasurementSystem == TextMeasurementSystem.GdiPlus) {
					writer.WriteAttribute(XamlAttribute.TextWrapping, (labelBrick.Angle != 0f && labelBrick.Style.StringFormat.WordWrap) ? XamlNames.TextWrappingWrap : XamlNames.TextWrappingNoWrap);
				}
				if(exportContext.Compatibility == XamlCompatibility.WPF && labelBrick.Angle != 0) {
					writer.WriteAttribute(XamlAttribute.HorizontalAlignment, MapTextAlignmentToHorizontalAlignment(brick.Style.TextAlignment));
					writer.WriteStartElement(XamlTag.TextBlockLayoutTransform);
					writer.WriteStartElement(XamlTag.TransformGroup);
					writer.WriteStartElement(XamlTag.RotateTransform);
					writer.WriteAttribute(XamlAttribute.Angle, 360 - labelBrick.Angle);
					writer.WriteEndElement(); 
					writer.WriteEndElement(); 
					writer.WriteEndElement(); 
				}
			}
#endif
			writer.WriteEndElement(); 
		}
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.Content;
		}
#if !SL
		static string MapTextAlignmentToHorizontalAlignment(TextAlignment textAlignment) {
			switch(textAlignment) {
				case TextAlignment.BottomLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.TopLeft:
				case TextAlignment.BottomJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.TopJustify:
					return XamlNames.AlignmentLeft;
				case TextAlignment.BottomCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.TopCenter:
					return XamlNames.AlignmentCenter;
				case TextAlignment.BottomRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.TopRight:
					return XamlNames.AlignmentRight;
			}
			throw new ArgumentException("textAlignment");
		}
#endif
		protected virtual string GetText(TextBrick brick, PrintingSystemBase ps, Page drawingPage, TextMeasurementSystem textMeasurementSystem) {
			string text = brick.Text;
#if !SL
			if(textMeasurementSystem == TextMeasurementSystem.GdiPlus) {
				if(brick is LabelBrick && ((LabelBrick)brick).Angle != 0) {
					return (string.IsNullOrEmpty(text) || brick.StringFormat.FormatFlags.HasFlag(StringFormatFlags.MeasureTrailingSpaces))
						? text
						: text.TrimEnd(' ');
				}
				float usableWidth = Math.Max(0, brick.Padding.DeflateWidth(brick.Width, GraphicsDpi.Document));
				using(var measurer = new GdiPlusMeasurer(ps)) {
					var textFormatter = new TextFormatter(GraphicsUnit.Document, measurer);
					string[] multilineText = textFormatter.FormatMultilineText(text, brick.Font, usableWidth, float.MaxValue, brick.StringFormat.Value);
					return string.Join(Environment.NewLine, multilineText);
				}
			}
#endif
			return text;
		}
	}
}
