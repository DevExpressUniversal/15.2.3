#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class XRControlRenderService : IXRControlRenderService {
		#region IXRControlRenderService
		public byte[] RenderShape(XRShapeLayout shapeContract) {
			float width = shapeContract.Width;
			float height = shapeContract.Height;
			ShapeBase shape = GetShapeElement(shapeContract);
			var colorConverter = PrintingSystemXmlSerializer.ColorConverter.Instance;
			using(var stream = new MemoryStream())
			using(var brick = new ShapeBrick())
			using(var sampleBitmap = new Bitmap((int)width, (int)height)) {
				brick.Angle = shapeContract.Angle;
				brick.LineWidth = XRConvert.Convert(shapeContract.LineWidth, shapeContract.Dpi, GraphicsDpi.Document);
				brick.FillColor = (Color)colorConverter.FromString(shapeContract.FillColor);
				brick.ForeColor = (Color)colorConverter.FromString(shapeContract.ForeColor);
				brick.Stretch = shapeContract.Stretch;
				using(var printingSystem = XtraReport.CreatePrintingSystem())
				using(var gdiGraphics = new ImageGraphics(sampleBitmap, printingSystem))
				using(var gdi = new GdiHashtable()) {
					var clientSize = new SizeF(width - 2, height - 2);
					clientSize = GraphicsUnitConverter.PixelToDoc(clientSize);
					ShapeHelper.DrawShapeContent(shape, gdiGraphics, new RectangleF(new PointF(1, 1), clientSize), brick, gdi);
				}
				sampleBitmap.Save(stream, ImageFormat.Png);
				return stream.ToArray();
			}
		}
		public RenderedXRChart RenderChartAsBase64(XRChartLayout chartContract) {
			using(var chart = DesignerChartWrapper.CreateFromContract(chartContract))
			using(var bitmap = chart.Chart.CreateBitmap(chart.SizeF.ToSize()))
			using(var bitmapStream = new MemoryStream()) {
				bitmap.Save(bitmapStream, ImageFormat.Png);
				var imageBase64 = Convert.ToBase64String(bitmapStream.GetBuffer(), 0, (int)bitmapStream.Length);
				var indexes = chart.GetIndexesOfIncompatibleViews();
				return new RenderedXRChart {
					Image = imageBase64,
					Indexes = indexes
				};
			}
		}
		#endregion
		static ShapeBase GetShapeElement(XRShapeLayout shapeContract) {
			switch(shapeContract.ShapeType) {
				case "Rectangle":
					return new ShapeRectangle { Fillet = shapeContract.Fillet };
				case "Ellipse":
					return new ShapeEllipse();
				case "Polygon":
					return new ShapePolygon {
						NumberOfSides = shapeContract.NumberOfSides,
						Fillet = shapeContract.Fillet
					};
				case "Arrow":
					return new ShapeArrow {
						ArrowHeight = shapeContract.ArrowHeight,
						ArrowWidth = shapeContract.ArrowWidth,
						Fillet = shapeContract.Fillet
					};
				case "Star":
					return new ShapeStar {
						Concavity = shapeContract.Concavity,
						Fillet = shapeContract.Fillet,
						StarPointCount = shapeContract.StarPointCount
					};
				case "Cross":
					return new ShapeCross {
						Fillet = shapeContract.Fillet,
						HorizontalLineWidth = shapeContract.HorizontalLineWidth,
						VerticalLineWidth = shapeContract.VerticalLineWidth
					};
				case "Line":
					return new ShapeLine();
				case "Bracket":
					return new ShapeBracket { TipLength = shapeContract.TipLength };
				case "Brace":
					return new ShapeBrace {
						TailLength = shapeContract.TailLength,
						TipLength = shapeContract.TipLength,
						Fillet = shapeContract.Fillet
					};
				default:
					return new ShapeRectangle();
			}
		}
		public RichEditorResponse RenderRich(RichEditorRequest richRequest) {
			var richText = DesignerRichWrapper.createFromRequest(richRequest);
			var image = richText.ToImage(System.Drawing.Text.TextRenderingHint.AntiAlias);
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				byte[] imageBytes = ms.ToArray();
				string base64String = Convert.ToBase64String(imageBytes);
				return new RichEditorResponse() {
					SerializableRtfString = richText.SerializableRtfString,
					Text = richText.Text,
					Rtf = richText.Rtf,
					Img = base64String
				};
			}
		}
	}
}
