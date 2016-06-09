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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils.Text;
using DevExpress.XtraSpreadsheet.Drawing;
using GuideValues = System.Collections.Generic.Dictionary<string, double>;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ShapeLayoutInfo
	public class ShapeLayoutInfo {
		public PathInfo[] Paths { get; set; }
		public RectangleF TextRectangle { get; set; }
		public Pen Pen { get; set; }
		public Brush DefaultBrush { get; set; }
	}
	#endregion
	#region PathInfo
	public class PathInfo {
		#region Properties
		public GraphicsPath GraphicsPath { get; private set; }
		public Brush Fill { get; private set; }
		public bool Stroke { get; private set; }
		public PathInfo(GraphicsPath graphicsPath, Brush fill, bool stroke) {
			GraphicsPath = graphicsPath;
			Fill = fill;
			Stroke = stroke;
		}
		#endregion
	}
	#endregion
	#region SimpleParagraphInfo
	public class SimpleParagraphInfo {
		public Font Font { get; private set; }
		public string PlainText { get; private set; }
		public Color Color { get; private set; }
		public StringFormat StringFormat { get; private set; }
		public Size Size { get; private set; }
		public SimpleParagraphInfo(Font font, string plainText, Color color, StringFormat stringFormat, Size size) {
			this.Font = font;
			this.PlainText = plainText;
			this.Color = color;
			this.StringFormat = stringFormat;
			this.Size = size;
		}
	}
	#endregion
	#region IShapeRenderService
	public interface IShapeRenderService {
		void RenderShape(ModelShape shape, Graphics graphics, Rectangle bounds);
	}
	#endregion
	#region DrawingShapeController
	public class DrawingShapeController : IShapeRenderService {
#if !SL && !DXPORTABLE
		static IWordBreakProvider wordBreakProvider = new SpreadsheetWordBreakProvider();
#endif
		public void RenderShape(ModelShape shape, Graphics graphics, Rectangle bounds) {
			ModelShapeCustomGeometry customGeometry = GetShapeGeometry(shape);
			RenderCustomGeometry(shape, customGeometry, graphics, bounds);
		}
		static ModelShapeCustomGeometry GetShapeGeometry(ModelShape shape) {
			if(shape.ShapeProperties.ShapeType == ShapeType.None) {
				return shape.ShapeProperties.CustomGeometry;
			}
			else {
				return ShapesPresetGeometry.GetCustomGeometryByPreset(shape.ShapeProperties.ShapeType);
			}
		}
		static void RenderCustomGeometry(ModelShape shape, ModelShapeCustomGeometry geometry, Graphics graphics, Rectangle bounds) {
			ShapeLayoutInfo shapeLayoutInfo = CalculateGeometry(shape, geometry, bounds);
			Draw(shapeLayoutInfo, graphics);
			DrawText(shape, shapeLayoutInfo.TextRectangle, graphics);
		}
		static SimpleParagraphInfo[] GenerateParagraphInfos(ModelShape shape, Graphics graphics, Rectangle textRect) {
			DocumentModel documentModel = shape.DocumentModel;
			TextProperties textProperties = shape.TextProperties;
			XlFontSchemeStyles fontReferenceIdx = shape.ShapeStyle.FontReferenceIdx;
			SimpleParagraphInfo[] result = new SimpleParagraphInfo[textProperties.Paragraphs.Count];
			DrawingTextBodyProperties bodyProperties = textProperties.BodyProperties;
			for(int i = 0; i < result.Length; ++i) {
				DrawingTextParagraph paragraph = textProperties.Paragraphs[i];
				DrawingTextCharacterProperties runProperties;
				if(paragraph.Runs.Count != 0)
					runProperties = paragraph.Runs[0].RunProperties;
				else {
					if(paragraph.ApplyEndRunProperties)
						runProperties = paragraph.EndRunProperties;
					else
						continue;
				}
				string typeface = runProperties.Latin.Typeface;
				if(string.IsNullOrEmpty(typeface))
					typeface = documentModel.OfficeTheme.FontScheme.GetTypeface(fontReferenceIdx, runProperties.Language);
				if(typeface.StartsWith("+mn"))
					typeface = documentModel.OfficeTheme.FontScheme.GetTypeface(fontReferenceIdx, documentModel.Culture);
				FontStyle style = FontStyle.Regular;
				if(runProperties.Italic)
					style |= FontStyle.Italic;
				if(runProperties.Bold)
					style |= FontStyle.Bold;
				if(runProperties.Underline != DrawingTextUnderlineType.None)
					style |= FontStyle.Underline;
				if(runProperties.Strikethrough != DrawingTextStrikeType.None)
					style |= FontStyle.Strikeout;
				float fontSize = runProperties.Options.HasFontSize ? runProperties.FontSize / 100f : 11;
				Font font = new Font(typeface, fontSize, style);
				Color textColor = GetTextColor(shape, runProperties);
				StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
				switch(paragraph.ParagraphProperties.TextAlignment) {
					case DrawingTextAlignmentType.Left:
						stringFormat.Alignment = StringAlignment.Near;
						break;
					case DrawingTextAlignmentType.Center:
						stringFormat.Alignment = StringAlignment.Center;
						break;
					case DrawingTextAlignmentType.Right:
						stringFormat.Alignment = StringAlignment.Far;
						break;
					case DrawingTextAlignmentType.Justified:
						break;
					case DrawingTextAlignmentType.JustifiedLow:
						break;
					case DrawingTextAlignmentType.Distributed:
						break;
					case DrawingTextAlignmentType.ThaiDistributed:
						break;
				}
				stringFormat.Trimming = StringTrimming.Character;
				bool noWrap = bodyProperties.TextWrapping == DrawingTextWrappingType.None;
				if(noWrap)
					stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
				bool noCLip = bodyProperties.HorizontalOverflow == DrawingTextHorizontalOverflowType.Overflow;
				if(noCLip)
					stringFormat.FormatFlags |= StringFormatFlags.NoClip;
				else
					stringFormat.FormatFlags &= ~StringFormatFlags.NoClip;
				string plainText = paragraph.GetPlainText();
				Size size;
				if(!noWrap && noCLip) {
#if SL || DXPORTABLE
					size = TextUtils.GetStringSize(graphics, plainText, font, stringFormat, textRect.Width);
#else
					size = TextUtils.GetStringSize(graphics, plainText, font, stringFormat, textRect.Width, wordBreakProvider);
#endif
				}
				else {
#if SL || DXPORTABLE
					size = TextUtils.GetStringSize(graphics, plainText, font, stringFormat, textRect.Width, textRect.Height);
#else
					size = TextUtils.GetStringSize(graphics, plainText, font, stringFormat, textRect.Width, textRect.Height, wordBreakProvider);
#endif
				}
				result[i] = new SimpleParagraphInfo(font, plainText, textColor, stringFormat, size);
			}
			return result;
		}
		static void DrawText(ModelShape shape, RectangleF textRectangle, Graphics graphics) {
			DocumentModel documentModel = shape.DocumentModel;
			TextProperties textProperties = shape.TextProperties;
			DrawingTextBodyProperties bodyProperties = textProperties.BodyProperties;
			DrawingTextInset inset = bodyProperties.Inset;
			DocumentModelUnitToLayoutUnitConverter layoutUnitConverter = documentModel.ToDocumentLayoutUnitConverter;
			Rectangle textRect = CalculateTextRect(textRectangle, layoutUnitConverter, inset);
			SimpleParagraphInfo[] simpleParagraphInfoArray = GenerateParagraphInfos(shape, graphics, textRect);
			int textHeight = 0;
			for(int i = 0; i < simpleParagraphInfoArray.Length; ++i) {
				SimpleParagraphInfo simpleParagraphInfo = simpleParagraphInfoArray[i];
				textHeight += simpleParagraphInfo.Size.Height;
			}
			int offsetY = 0;
			switch(bodyProperties.Anchor) {
				case DrawingTextAnchoringType.Bottom:
					offsetY = textRect.Height - textHeight;
					break;
				case DrawingTextAnchoringType.Center:
					offsetY = textRect.Height / 2 - textHeight / 2;
					break;
				case DrawingTextAnchoringType.Distributed:
					break;
				case DrawingTextAnchoringType.Justified:
					break;
				case DrawingTextAnchoringType.Top:
					break;
			}
			DrawTextCore(graphics, textProperties, simpleParagraphInfoArray, textRect, offsetY);
		}
		static Rectangle CalculateTextRect(RectangleF textRectangle, DocumentModelUnitToLayoutUnitConverter layoutUnitConverter, DrawingTextInset inset) {
			int left = (int) textRectangle.Left + layoutUnitConverter.ToLayoutUnits(inset.Left);
			int top = (int) textRectangle.Top + layoutUnitConverter.ToLayoutUnits(inset.Top);
			int right = (int) textRectangle.Right - layoutUnitConverter.ToLayoutUnits(inset.Right);
			int bottom = (int) textRectangle.Bottom - layoutUnitConverter.ToLayoutUnits(inset.Bottom);
			if(left > right) {
				int temp = left;
				left = right;
				right = temp;
			}
			if(top > bottom) {
				int temp = top;
				top = bottom;
				bottom = temp;
			}
			Rectangle textRect = Rectangle.FromLTRB(left, top, right, bottom);
			return textRect;
		}
		static void DrawTextCore(Graphics graphics, TextProperties textProperties, SimpleParagraphInfo[] simpleParagraphInfoArray, Rectangle textRect, int offsetY) {
			for(int i = 0; i < textProperties.Paragraphs.Count; ++i) {
				SimpleParagraphInfo simpleParagraphInfo = simpleParagraphInfoArray[i];
				Font font = simpleParagraphInfo.Font;
				string plainText = simpleParagraphInfo.PlainText;
				StringFormat stringFormat = simpleParagraphInfo.StringFormat;
				int paragraphHeight = simpleParagraphInfo.Size.Height;
				bool noClip = (stringFormat.FormatFlags & StringFormatFlags.NoClip) == StringFormatFlags.NoClip;
				int paragraphY = textRect.Top + offsetY;
				Rectangle drawBounds = new Rectangle(textRect.Left, paragraphY, textRect.Width, noClip ? paragraphHeight : Math.Min(paragraphHeight, textRect.Height + textRect.Top - paragraphY));
				offsetY += paragraphHeight;
				if(noClip && (stringFormat.FormatFlags & StringFormatFlags.NoWrap) == StringFormatFlags.NoWrap) {
					TextUtils.DrawString(graphics, plainText, font, simpleParagraphInfo.Color, new Point(drawBounds.Left, drawBounds.Top), stringFormat);
				}
				else {
#if SL || DXPORTABLE
					TextUtils.DrawString(graphics, plainText, font, simpleParagraphInfo.Color, drawBounds, Rectangle.Empty, stringFormat);
#else
					TextUtils.DrawString(graphics, plainText, font, simpleParagraphInfo.Color, drawBounds, Rectangle.Empty, stringFormat, null, wordBreakProvider);
#endif
				}
			}
		}
		static Color GetTextColor(ModelShape shape, DrawingTextCharacterProperties runProperties) {
			Color defaultColor = shape.ShapeStyle.FontColor.FinalColor;
			if(runProperties.Fill.FillType != DrawingFillType.Solid)
				return defaultColor;
			DrawingSolidFill solidFill = runProperties.Fill as DrawingSolidFill;
			return solidFill == null ? defaultColor : solidFill.Color.FinalColor;
		}
		static ShapeLayoutInfo CalculateGeometry(ModelShape shape, ModelShapeCustomGeometry geometry, Rectangle bounds) {
			ShapeLayoutInfo result = new ShapeLayoutInfo();
			int heightEMU = shape.DocumentModel.UnitConverter.ModelUnitsToEmuF(shape.Height);
			int widthEMU = shape.DocumentModel.UnitConverter.ModelUnitsToEmuF(shape.Width);
			ShapeGuideCalculator calculator = new ShapeGuideCalculator(geometry, widthEMU, heightEMU, shape.ShapeProperties.PresetAdjustList);
			result.DefaultBrush = GetBrush(shape, bounds);
			result.Paths = CalculateGeometryCore(geometry, calculator, shape, bounds, result.DefaultBrush);
			result.TextRectangle = CalculateTextRect(geometry.ShapeTextRectangle, calculator, shape.DocumentModel, bounds);
			result.Pen = GetPen(shape);
			return result;
		}
		internal static Brush GetBrush(ModelShape shape, Rectangle bounds) {
			if(shape.ShapeProperties.Fill == null)
				return null;
			Brush defaultBrush = null;
			if(!shape.ShapeStyle.FillColor.IsEmpty && shape.ShapeStyle.FillReferenceIdx != 0) {
				defaultBrush = new SolidBrush(shape.ShapeStyle.FillColor.FinalColor);
			}
			Color styleColor = shape.ShapeStyle.FillColor.FinalColor;
			IDrawingFill fill;
			if (shape.ShapeProperties.Fill.FillType == DrawingFillType.Automatic && shape.ShapeStyle.FillReferenceIdx > 0) {
				fill = shape.DocumentModel.OfficeTheme.FormatScheme.GetFill((StyleMatrixElementType) Math.Min(3, shape.ShapeStyle.FillReferenceIdx));
			}
			else {
				fill = shape.ShapeProperties.Fill;
				styleColor = Color.Empty;
			}
			ShapeFillRenderVisitor visitor = new ShapeFillRenderVisitor(styleColor, defaultBrush, bounds);
			fill.Visit(visitor);
			return visitor.Result;
		}
		static RectangleF CalculateTextRect(AdjustableRect shapeTextRectangle, ShapeGuideCalculator calculator, DocumentModel workbook, Rectangle bounds) {
			if(shapeTextRectangle == null || shapeTextRectangle.IsEmpty())
				return bounds;
			DocumentModelUnitToLayoutUnitConverter layoutUnitConverter = workbook.ToDocumentLayoutUnitConverter;
			DocumentModelUnitConverter unitConverter = workbook.UnitConverter;
			float top = bounds.Top + layoutUnitConverter.ToLayoutUnits(unitConverter.EmuToModelUnitsL((long) shapeTextRectangle.Top.Evaluate(calculator)));
			float left = bounds.Left + layoutUnitConverter.ToLayoutUnits(unitConverter.EmuToModelUnitsL((long) shapeTextRectangle.Left.Evaluate(calculator)));
			float bottom = bounds.Top + layoutUnitConverter.ToLayoutUnits(unitConverter.EmuToModelUnitsL((long) shapeTextRectangle.Bottom.Evaluate(calculator)));
			float right = bounds.Left + layoutUnitConverter.ToLayoutUnits(unitConverter.EmuToModelUnitsL((long) shapeTextRectangle.Right.Evaluate(calculator)));
			return RectangleF.FromLTRB(left, top, right, bottom);
		}
		static PathInfo[] CalculateGeometryCore(ModelShapeCustomGeometry geometry, ShapeGuideCalculator calculator, ModelShape shape, Rectangle bounds, Brush defaultBrush) {
			PathInfo[] pathInfos = new PathInfo[geometry.Paths.Count];
			for(int index = 0; index < geometry.Paths.Count; index++) {
				ModelShapePath path = geometry.Paths[index];
				pathInfos[index] = CalculatePath(shape, bounds, path, calculator, defaultBrush);
			}
			return pathInfos;
		}
		static void Draw(ShapeLayoutInfo shapeLayoutInfo, Graphics graphics) {
			foreach(PathInfo pathInfo in shapeLayoutInfo.Paths) {
				if(pathInfo.Fill == null)
					continue;
				graphics.FillPath(pathInfo.Fill, pathInfo.GraphicsPath);
			}
			foreach(PathInfo pathInfo in shapeLayoutInfo.Paths) {
				if(!pathInfo.Stroke)
					continue;
				graphics.DrawPath(shapeLayoutInfo.Pen, pathInfo.GraphicsPath);
			}
		}
		static PathInfo CalculatePath(ModelShape shape, Rectangle bounds, ModelShapePath path, ShapeGuideCalculator calculator, Brush defaultBrush) {
			float workbookDefaultScale = shape.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(shape.DocumentModel.UnitConverter.EmuToModelUnitsF(1));
			try {
				DrawingShapePathWalker walker = new DrawingShapePathWalker(path, bounds, calculator, workbookDefaultScale);
				walker.Walk();
				Brush fillBrush = GetPathFillBrush(shape, path, defaultBrush);
				return new PathInfo(walker.GraphicsPath, fillBrush, path.Stroke);
			}
			catch {
				return new PathInfo(new GraphicsPath(), null, false);
			}
		}
		public static Pen GetPen(DocumentModel documentModel, ShapeStyle shapeStyle, ShapeProperties shapeProperties) {
			Outline outline = shapeProperties.Outline;
			DocumentModelUnitConverter unitConverter = documentModel.UnitConverter;
			float penWidth;
			if (outline.IsDefault || outline.Width == 0) {
				if (shapeStyle.LineReferenceIdx == 0 && outline.IsDefault) {
					return new Pen(Color.Empty, 0);
				}
				int validLineReferenceIdx = shapeStyle.LineReferenceIdx < 1 ? 1 : shapeStyle.LineReferenceIdx > 3 ? 3 : shapeStyle.LineReferenceIdx;
				penWidth = documentModel.OfficeTheme.FormatScheme.GetOutline((StyleMatrixElementType) validLineReferenceIdx).Width;
				penWidth = Math.Max(1, unitConverter.ModelUnitsToPixelsF(penWidth, DocumentModel.Dpi));
			}
			else {
				penWidth = Math.Max(1, unitConverter.ModelUnitsToPixelsF(outline.Width, DocumentModel.Dpi));
			}
			Color penColor = outline.Fill.FillType == DrawingFillType.Automatic ? shapeStyle.LineColor.FinalColor : shapeProperties.OutlineColor.FinalColor;
			Pen result = new Pen(penColor, penWidth);
			switch(outline.Dashing) {
				case OutlineDashing.Solid:
					result.DashStyle = DashStyle.Solid;
					break;
				case OutlineDashing.Dash:
				case OutlineDashing.LongDash:
				case OutlineDashing.SystemDash:
					result.DashStyle = DashStyle.Dash;
					break;
				case OutlineDashing.DashDot:
				case OutlineDashing.LongDashDot:
				case OutlineDashing.SystemDashDot:
					result.DashStyle = DashStyle.DashDot;
					break;
				case OutlineDashing.Dot:
				case OutlineDashing.SystemDot:
					result.DashStyle = DashStyle.Dot;
					break;
				case OutlineDashing.SystemDashDotDot:
				case OutlineDashing.LongDashDotDot:
					result.DashStyle = DashStyle.DashDotDot;
					break;
			}
			return result;
		}
		internal static Pen GetPen(ModelShape shape) {
			return GetPen(shape.DocumentModel, shape.ShapeStyle, shape.ShapeProperties);
		}
		static Brush GetPathFillBrush(ModelShape shape, ModelShapePath path, Brush defaultBrush) {
			if(shape.ShapeProperties.Fill.FillType == DrawingFillType.None || path.FillMode == PathFillMode.None)
				return null;
			SolidBrush solidBrush = defaultBrush as SolidBrush;
			if(solidBrush == null)
				return defaultBrush;
			Color brushColor = ModelShape.GetPathFillColorCore(solidBrush.Color, path.FillMode);
			return new SolidBrush(brushColor);
		}
	}
	#endregion
	#region ShapeGuideCalculator
	public class ShapeGuideCalculator {
		#region static
		static readonly ModelShapeGuideList builtinGuides = PopulateBuiltInGuides();
		static ModelShapeGuideList PopulateBuiltInGuides() {
			ModelShapeGuideList result = new ModelShapeGuideList(FakeDocumentModel.Instance);
			result.Add(new ModelShapeGuide("hc", "*/ w 1 2"));
			result.Add(new ModelShapeGuide("hd2", "*/ h 1 2"));
			result.Add(new ModelShapeGuide("hd3", "*/ h 1 3"));
			result.Add(new ModelShapeGuide("hd4", "*/ h 1 4"));
			result.Add(new ModelShapeGuide("hd5", "*/ h 1 5"));
			result.Add(new ModelShapeGuide("hd6", "*/ h 1 6"));
			result.Add(new ModelShapeGuide("hd8", "*/ h 1 8"));
			result.Add(new ModelShapeGuide("ls", "max w h"));
			result.Add(new ModelShapeGuide("ss", "min w h"));
			result.Add(new ModelShapeGuide("ssd2", "*/ ss 1 2"));
			result.Add(new ModelShapeGuide("ssd4", "*/ ss 1 4"));
			result.Add(new ModelShapeGuide("ssd6", "*/ ss 1 6"));
			result.Add(new ModelShapeGuide("ssd8", "*/ ss 1 8"));
			result.Add(new ModelShapeGuide("ssd16", "*/ ss 1 16"));
			result.Add(new ModelShapeGuide("ssd32", "*/ ss 1 32"));
			result.Add(new ModelShapeGuide("vc", "*/ h 1 2"));
			result.Add(new ModelShapeGuide("wd2", "*/ w 1 2"));
			result.Add(new ModelShapeGuide("wd3", "*/ w 1 3"));
			result.Add(new ModelShapeGuide("wd4", "*/ w 1 4"));
			result.Add(new ModelShapeGuide("wd5", "*/ w 1 5"));
			result.Add(new ModelShapeGuide("wd6", "*/ w 1 6"));
			result.Add(new ModelShapeGuide("wd8", "*/ w 1 8"));
			result.Add(new ModelShapeGuide("wd10", "*/ w 1 10"));
			result.Add(new ModelShapeGuide("wd32", "*/ w 1 32"));
			return result;
		}
		#endregion
		#region Fields
		readonly GuideValues guides;
		readonly ModelShapeCustomGeometry geometry;
		#endregion
		#region Properties
		GuideValues Guides { get { return guides; } }
		ModelShapeCustomGeometry Geometry { get { return geometry; } }
		#endregion
		public ShapeGuideCalculator(ModelShapeCustomGeometry geometry, int widthEMU, int heightEMU, ModelShapeGuideList presetAdjustList) {
			this.guides = new GuideValues();
			this.geometry = geometry;
			PopulateConstantGuides();
			Guides.Add("h", heightEMU);
			Guides.Add("w", widthEMU);
			Guides.Add("b", heightEMU);
			Guides.Add("r", widthEMU);
			CalculateGuides(presetAdjustList);
		}
		void CalculateGuides(ModelShapeGuideList presetAdjustList) {
			foreach(ModelShapeGuide builtinGuide in builtinGuides) {
				CalculateGuide(builtinGuide);
			}
			foreach(ModelShapeGuide guide in Geometry.AdjustValues) {
				CalculateGuide(guide);
			}
			foreach(ModelShapeGuide guide in presetAdjustList) {
				CalculateGuide(guide);
			}
			foreach(ModelShapeGuide guide in Geometry.Guides) {
				CalculateGuide(guide);
			}
		}
		internal void CalculateGuide(ModelShapeGuide guide) {
			double result = 0;
			try {
				string[] tokens = guide.Formula.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string formulaType = tokens[0];
				string xs = tokens.Length >= 2 ? tokens[1] : String.Empty;
				string ys = tokens.Length >= 3 ? tokens[2] : String.Empty;
				string zs = tokens.Length >= 4 ? tokens[3] : String.Empty;
				double x = GetGuideValue(xs);
				double y = GetGuideValue(ys);
				double z = GetGuideValue(zs);
				switch(formulaType) {
					case "*/":
						result = z == 0 ? x * y : (x * y) / z;
						break;
					case "+-":
						result = x + y - z;
						break;
					case "+/":
						result = z == 0 ? x + y : (x + y) / z;
						break;
					case "?:":
						result = x > 0 ? y : z;
						break;
					case "abs":
						result = Math.Abs(x);
						break;
					case "at2":
						double atan = x == 0 ? Math.Atan(y) : Math.Atan2(y, x);
						result = RadianToEMUDegree(atan);
						break;
					case "cat2":
						atan = y == 0 ? Math.Atan(z) : Math.Atan2(z, y);
						result = x * Math.Cos(atan);
						break;
					case "cos":
						result = (x * Math.Cos(EMUDegreeToRadian(y)));
						break;
					case "max":
						result = Math.Max(x, y);
						break;
					case "min":
						result = Math.Min(x, y);
						break;
					case "mod":
						result = Math.Sqrt(x * x + y * y + z * z);
						break;
					case "pin":
						result = y < x ? x : y > z ? z : y;
						break;
					case "sat2":
						atan = y == 0 ? Math.Atan(z) : Math.Atan2(z, y);
						result = x * Math.Sin(atan);
						break;
					case "sin":
						result = (x * Math.Sin(EMUDegreeToRadian(y)));
						break;
					case "sqrt":
						result = Math.Sqrt(Math.Abs(x));
						break;
					case "tan":
						result = (x * Math.Tan(EMUDegreeToRadian(y)));
						break;
					case "val":
						result = x;
						break;
				}
			}
			catch {
				result = 0;
			}
			if(Guides.ContainsKey(guide.Name))
				Guides.Remove(guide.Name);
			Guides.Add(guide.Name, result);
		}
		void PopulateConstantGuides() {
			Guides.Add("3cd4", 16200000);
			Guides.Add("3cd8", 8100000);
			Guides.Add("5cd8", 13500000);
			Guides.Add("7cd8", 18900000);
			Guides.Add("cd2", 10800000);
			Guides.Add("cd4", 5400000);
			Guides.Add("cd8", 2700000);
			Guides.Add("l", 0);
			Guides.Add("t", 0);
		}
		public double GetGuideValue(string guideName) {
			if(String.IsNullOrEmpty(guideName))
				return 0;
			double result;
			if(double.TryParse(guideName, out result)) {
				return result;
			}
			if(Guides.TryGetValue(guideName, out result))
				return result;
			return 0;
		}
		internal static double EMUDegreeToRadian(double degree) {
			return degree / 60000d / 180d * Math.PI;
		}
		internal static double RadianToEMUDegree(double radian) {
			return radian / Math.PI * 180d * 60000d;
		}
	}
	#endregion
	#region DrawingShapePathWalker
	internal class DrawingShapePathWalker : IPathInstructionWalker {
		#region Fields
		readonly ModelShapePath path;
		readonly ShapeGuideCalculator calculator;
		GraphicsPath graphicsPath;
		readonly float scaleX;
		readonly float scaleY;
		readonly int left;
		readonly int top;
		double lastX;
		double lastY;
		#endregion
		public DrawingShapePathWalker(ModelShapePath path, Rectangle bounding, ShapeGuideCalculator calculator, float workbookDefaultScale) {
			this.path = path;
			this.calculator = calculator;
			scaleX = path.Width == 0 ? workbookDefaultScale : bounding.Width / (float) path.Width;
			scaleY = path.Height == 0 ? workbookDefaultScale : bounding.Height / (float) path.Height;
			left = bounding.Left;
			top = bounding.Top;
		}
		public GraphicsPath GraphicsPath { get { return graphicsPath; } }
		public void Walk() {
			graphicsPath = new GraphicsPath();
			foreach(IPathInstruction instruction in path.Instructions) {
				instruction.Visit(this);
			}
		}
		#region Implementation of IPathIncstructionWalker
		public void Visit(PathArc pathArc) {
			double hrEMU = pathArc.HeightRadius.Evaluate(calculator);
			double wrEMU = pathArc.WidthRadius.Evaluate(calculator);
			double stAngEMU = pathArc.StartAngle.Evaluate(calculator);
			double swAngEMU = pathArc.SwingAngle.Evaluate(calculator);
			double hR = hrEMU;
			double wR = wrEMU;
			double stAng = AdjAngleToDegree(stAngEMU);
			double swAng = AdjAngleToDegree(swAngEMU);
			double stAngRadian = DegreeToRadian(stAng);
			double swAngRadian = DegreeToRadian(swAng);
			double cos = Math.Cos(stAngRadian);
			double sin = Math.Sin(stAngRadian);
			double radius = (hR * wR) / Math.Sqrt(hR * hR * cos * cos + wR * wR * sin * sin);
			if(double.IsNaN(radius))
				return;
			double x0 = lastX - radius * cos;
			double y0 = lastY - radius * sin;
			double normalizeStartAngle = NormalizeAngle(stAng);
			double normalizedSwingAngle = NormalizeAngle(stAng + swAng) - normalizeStartAngle;
			GraphicsPath.AddArc(left + EMUToLayoutUnitsX(x0 - wR), top + EMUToLayoutUnitsY(y0 - hR), EMUToLayoutUnitsX(wR * 2), EMUToLayoutUnitsY(hR * 2), (float) normalizeStartAngle, (float) normalizedSwingAngle);
			double endCos = Math.Cos(stAngRadian + swAngRadian);
			double endSin = Math.Sin(stAngRadian + swAngRadian);
			double endRadius = (hR * wR) / Math.Sqrt(hR * hR * endCos * endCos + wR * wR * endSin * endSin);
			if(double.IsNaN(endRadius))
				endRadius = 0;
			lastX = x0 + endRadius * endCos;
			lastY = y0 + endRadius * endSin;
		}
		public void Visit(PathClose value) {
			GraphicsPath.CloseFigure();
			GraphicsPath.StartFigure();
		}
		public void Visit(PathCubicBezier value) {
			double[] x = new double[4];
			double[] y = new double[4];
			x[0] = lastX;
			y[0] = lastY;
			for(int i = 1; i < 4; i++) {
				x[i] = value.Points[i - 1].X.Evaluate(calculator);
				y[i] = value.Points[i - 1].Y.Evaluate(calculator);
			}
			GraphicsPath.AddBezier(left + EMUToLayoutUnitsX(x[0]), top + EMUToLayoutUnitsY(y[0]), left + EMUToLayoutUnitsX(x[1]), top + EMUToLayoutUnitsY(y[1]), left + EMUToLayoutUnitsX(x[2]), top + EMUToLayoutUnitsY(y[2]), left + EMUToLayoutUnitsX(x[3]),top + EMUToLayoutUnitsY(y[3]));
			lastX = x[3];
			lastY = y[3];
		}
		public void Visit(PathLine pathLine) {
			double xEMU = pathLine.Point.X.Evaluate(calculator);
			double yEMU = pathLine.Point.Y.Evaluate(calculator);
			GraphicsPath.AddLine(left + EMUToLayoutUnitsX(lastX), top + EMUToLayoutUnitsY(lastY), left + EMUToLayoutUnitsX(xEMU), top + EMUToLayoutUnitsY(yEMU));
			lastX = xEMU;
			lastY = yEMU;
		}
		public void Visit(PathMove pathMove) {
			double xEMU = pathMove.Point.X.Evaluate(calculator);
			double yEMU = pathMove.Point.Y.Evaluate(calculator);
			lastX = xEMU;
			lastY = yEMU;
			GraphicsPath.StartFigure();
		}
		public void Visit(PathQuadraticBezier value) {
			double[] qx = new double[3];
			double[] qy = new double[3];
			double[] cx = new double[4];
			double[] cy = new double[4];
			qx[0] = lastX;
			qy[0] = lastY;
			for(int i = 1; i < 3; i++) {
				qx[i] = value.Points[i - 1].X.Evaluate(calculator);
				qy[i] = value.Points[i - 1].Y.Evaluate(calculator);
			}
			cx[0] = qx[0];
			cy[0] = qy[0];
			cx[1] = qx[0] + 2 * (qx[1] - qx[0]) / 3;
			cy[1] = qy[0] + 2 * (qy[1] - qy[0]) / 3;
			cx[2] = qx[2] + 2 * (qx[1] - qx[2]) / 3;
			cy[2] = qy[2] + 2 * (qy[1] - qy[2]) / 3;
			cx[3] = qx[2];
			cy[3] = qy[2];
			lastX = cx[3];
			lastY = cy[3];
			GraphicsPath.AddBezier(left + EMUToLayoutUnitsX(cx[0]), top + EMUToLayoutUnitsY(cy[0]), left + EMUToLayoutUnitsX(cx[1]), top + EMUToLayoutUnitsY(cy[1]), left + EMUToLayoutUnitsX(cx[2]), top + EMUToLayoutUnitsY(cy[2]), left + EMUToLayoutUnitsX(cx[3]), top + EMUToLayoutUnitsY(cy[3]));
		}
		#endregion
		static float EMUToLayoutUnits(double emu, float delta) {
			return (float) (emu * delta);
		}
		float EMUToLayoutUnitsX(double emu) {
			return EMUToLayoutUnits(emu, scaleX);
		}
		float EMUToLayoutUnitsY(double emu) {
			return EMUToLayoutUnits(emu, scaleY);
		}
		double AdjAngleToDegree(double value) {
			return value / 60000d;
		}
		static double DegreeToRadian(double degree) {
			return degree / 180d * Math.PI;
		}
		static double RadianToDegree(double radian) {
			return radian / Math.PI * 180;
		}
		double NormalizeAngle(double angle) {
			int quadrant = 0;
			while(angle < 0) {
				angle += 90;
				quadrant--;
			}
			while(angle > 90) {
				angle -= 90;
				quadrant++;
			}
			double t = Math.Tan(DegreeToRadian(angle));
			t = t * scaleX / scaleY;
			double r = RadianToDegree(Math.Atan(t));
			r += quadrant * 90;
			return r;
		}
	}
	#endregion
	#region ShapeFillRenderVisitor
	internal class ShapeFillRenderVisitor : IDrawingFillVisitor {
		#region Static members
		readonly Dictionary<DrawingPatternType, HatchStyle> hatchStyleTable = CreateHatchStyleTable();
		static Dictionary<DrawingPatternType, HatchStyle> CreateHatchStyleTable() {
			Dictionary<DrawingPatternType, HatchStyle> result = new Dictionary<DrawingPatternType, HatchStyle>();
			result.Add(DrawingPatternType.Cross, HatchStyle.Cross);
			result.Add(DrawingPatternType.DashedDownwardDiagonal, HatchStyle.DashedDownwardDiagonal);
			result.Add(DrawingPatternType.DashedHorizontal, HatchStyle.DashedHorizontal);
			result.Add(DrawingPatternType.DashedUpwardDiagonal, HatchStyle.DashedUpwardDiagonal);
			result.Add(DrawingPatternType.DashedVertical, HatchStyle.DashedVertical);
			result.Add(DrawingPatternType.DiagonalBrick, HatchStyle.DiagonalBrick);
			result.Add(DrawingPatternType.DiagonalCross, HatchStyle.DiagonalCross);
			result.Add(DrawingPatternType.Divot, HatchStyle.Divot);
			result.Add(DrawingPatternType.DarkDownwardDiagonal, HatchStyle.DarkDownwardDiagonal);
			result.Add(DrawingPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(DrawingPatternType.DarkUpwardDiagonal, HatchStyle.DarkUpwardDiagonal);
			result.Add(DrawingPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(DrawingPatternType.DownwardDiagonal, HatchStyle.WideDownwardDiagonal);
			result.Add(DrawingPatternType.DottedDiamond, HatchStyle.DottedDiamond);
			result.Add(DrawingPatternType.DottedGrid, HatchStyle.DottedGrid);
			result.Add(DrawingPatternType.Horizontal, HatchStyle.Horizontal);
			result.Add(DrawingPatternType.HorizontalBrick, HatchStyle.HorizontalBrick);
			result.Add(DrawingPatternType.LargeCheckerBoard, HatchStyle.LargeCheckerBoard);
			result.Add(DrawingPatternType.LargeConfetti, HatchStyle.LargeConfetti);
			result.Add(DrawingPatternType.LargeGrid, HatchStyle.LargeGrid);
			result.Add(DrawingPatternType.LightDownwardDiagonal, HatchStyle.LightDownwardDiagonal);
			result.Add(DrawingPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(DrawingPatternType.LightUpwardDiagonal, HatchStyle.LightUpwardDiagonal);
			result.Add(DrawingPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(DrawingPatternType.NarrowHorizontal, HatchStyle.NarrowHorizontal);
			result.Add(DrawingPatternType.NarrowVertical, HatchStyle.NarrowVertical);
			result.Add(DrawingPatternType.OpenDiamond, HatchStyle.OutlinedDiamond);
			result.Add(DrawingPatternType.Percent10, HatchStyle.Percent10);
			result.Add(DrawingPatternType.Percent20, HatchStyle.Percent20);
			result.Add(DrawingPatternType.Percent25, HatchStyle.Percent25);
			result.Add(DrawingPatternType.Percent30, HatchStyle.Percent30);
			result.Add(DrawingPatternType.Percent40, HatchStyle.Percent40);
			result.Add(DrawingPatternType.Percent5, HatchStyle.Percent05);
			result.Add(DrawingPatternType.Percent50, HatchStyle.Percent50);
			result.Add(DrawingPatternType.Percent60, HatchStyle.Percent60);
			result.Add(DrawingPatternType.Percent70, HatchStyle.Percent70);
			result.Add(DrawingPatternType.Percent75, HatchStyle.Percent75);
			result.Add(DrawingPatternType.Percent80, HatchStyle.Percent80);
			result.Add(DrawingPatternType.Percent90, HatchStyle.Percent90);
			result.Add(DrawingPatternType.Plaid, HatchStyle.Plaid);
			result.Add(DrawingPatternType.Shingle, HatchStyle.Shingle);
			result.Add(DrawingPatternType.SmallCheckerBoard, HatchStyle.SmallCheckerBoard);
			result.Add(DrawingPatternType.SmallConfetti, HatchStyle.SmallConfetti);
			result.Add(DrawingPatternType.SmallGrid, HatchStyle.SmallGrid);
			result.Add(DrawingPatternType.SolidDiamond, HatchStyle.SolidDiamond);
			result.Add(DrawingPatternType.Sphere, HatchStyle.Sphere);
			result.Add(DrawingPatternType.Trellis, HatchStyle.Trellis);
			result.Add(DrawingPatternType.UpwardDiagonal, HatchStyle.WideUpwardDiagonal);
			result.Add(DrawingPatternType.Vertical, HatchStyle.Vertical);
			result.Add(DrawingPatternType.Wave, HatchStyle.Wave);
			result.Add(DrawingPatternType.WideDownwardDiagonal, HatchStyle.WideDownwardDiagonal);
			result.Add(DrawingPatternType.WideUpwardDiagonal, HatchStyle.WideUpwardDiagonal);
			result.Add(DrawingPatternType.Weave, HatchStyle.Weave);
			result.Add(DrawingPatternType.ZigZag, HatchStyle.ZigZag);
			return result;
		}
		#endregion
		readonly Color styleColor;
		bool StyleColorEmpty {
			get { return styleColor == Color.Empty; }
		}
		public Brush Result { get; private set; }
		Brush DefaultBrush { get; set; }
		Rectangle Bounds { get; set; }
		public ShapeFillRenderVisitor(Color styleColor, Brush defaultBrush, Rectangle bounds) {
			this.styleColor = styleColor;
			DefaultBrush = defaultBrush;
			Bounds = bounds;
		}
		#region Implementation of IDrawingFillVisitor
		public void Visit(DrawingFill fill) {
			Result = DefaultBrush;
		}
		public void Visit(DrawingSolidFill fill) {
			Color finalColor = GetColorFromStyle(fill.Color);
			Result = new SolidBrush(finalColor);
		}
		public void Visit(DrawingPatternFill fill) {
			HatchStyle hatchStyle = hatchStyleTable[fill.PatternType];
			HatchBrush result = new HatchBrush(hatchStyle, GetColorFromStyle(fill.ForegroundColor), GetColorFromStyle(fill.BackgroundColor));
			Result = result;
		}
		Rectangle GetRectOffset(Rectangle original, RectangleOffset offset) {
			int tileLeft = (int) (original.Left + original.Width * offset.LeftOffset / (float) DrawingValueConstants.ThousandthOfPercentage);
			int tileTop = (int) (original.Top + original.Height * offset.TopOffset / (float) DrawingValueConstants.ThousandthOfPercentage);
			int tileRight = (int) (original.Right - original.Width * offset.RightOffset / (float) DrawingValueConstants.ThousandthOfPercentage);
			int tileBottom = (int) (original.Bottom - original.Height * offset.BottomOffset / (float) DrawingValueConstants.ThousandthOfPercentage);
			Rectangle result = Rectangle.FromLTRB(tileLeft, tileTop, tileRight, tileBottom);
			return result;
		}
		ColorBlend InverseBlend(ColorBlend value) {
			int count = value.Colors.Length;
			ColorBlend result = new ColorBlend(count);
			for(int i = 0; i < count; i++) {
				result.Colors[i] = value.Colors[count - 1 - i];
				result.Positions[i] = 1 - value.Positions[count - 1 - i];
			}
			return result;
		}
		Color GetColorFromStyle(DrawingColor drawingColor) {
			if (StyleColorEmpty)
				return drawingColor.FinalColor;
			DrawingColor styleDrawingColor = DrawingColor.Create(FakeDocumentModel.Instance, styleColor);
			styleDrawingColor.CopyFrom(drawingColor);
			styleDrawingColor.Info.SchemeColor = SchemeColorValues.Style;
			return styleDrawingColor.ToRgb(styleColor);
		}
		public void Visit(DrawingGradientFill fill) {
			int count = fill.GradientStops.Count;
			bool appendFirstStop = fill.GradientStops[0].Position > 0;
			bool appendLastStop = fill.GradientStops[count - 1].Position < DrawingValueConstants.ThousandthOfPercentage;
			int colorBlendCount = count;
			if (appendFirstStop)
				colorBlendCount++;
			if (appendLastStop)
				colorBlendCount++;
			int colorBlendIndex = 0;
			ColorBlend colorBlend = new ColorBlend(colorBlendCount);
			if (appendFirstStop) {
				colorBlend.Colors[colorBlendIndex] = GetColorFromStyle(fill.GradientStops[0].Color);
				colorBlend.Positions[colorBlendIndex] = 0.0f;
				colorBlendIndex++;
			}
			for(int i = 0; i < count; i++) {
				colorBlend.Colors[colorBlendIndex] = GetColorFromStyle(fill.GradientStops[i].Color);
				colorBlend.Positions[colorBlendIndex] = fill.GradientStops[i].Position / (float) DrawingValueConstants.ThousandthOfPercentage;
				colorBlendIndex++;
			}
			if (appendLastStop) {
				colorBlend.Colors[colorBlendIndex] = GetColorFromStyle(fill.GradientStops[count - 1].Color);
				colorBlend.Positions[colorBlendIndex] = 1.0f;
			}
			Rectangle tileRect = GetRectOffset(Bounds, fill.TileRect);
			switch(fill.GradientType) {
				case GradientType.Linear:
					LinearGradientBrush linearGradientBrush = new LinearGradientBrush(tileRect, Color.White, Color.White, fill.DocumentModel.UnitConverter.ModelUnitsToDegreeF(fill.Angle), fill.RotateWithShape);
					linearGradientBrush.InterpolationColors = colorBlend;
					Result = linearGradientBrush;
					break;
				case GradientType.Rectangle:
					GraphicsPath rectanglePath = new GraphicsPath();
					rectanglePath.AddRectangle(tileRect);
					PathGradientBrush rectangleBrush = new PathGradientBrush(rectanglePath);
					rectangleBrush.InterpolationColors = InverseBlend(colorBlend);
					Result = rectangleBrush;
					break;
				case GradientType.Circle:
				case GradientType.Shape:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void Visit(DrawingBlipFill fill) {
			Result = DefaultBrush;
		}
		#endregion
	}
	#endregion
}
