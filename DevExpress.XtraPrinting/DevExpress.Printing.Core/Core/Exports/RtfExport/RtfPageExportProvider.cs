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

using DevExpress.Printing.Native;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.TextRotation;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
namespace DevExpress.XtraPrinting.Export.Rtf {
	public class RtfPageExportProvider : RtfDocumentProviderBase, IRtfExportProvider {
		#region Inner Clases
		abstract class WatermarkRtfExportProviderBase {
			readonly Page page;
			readonly StringBuilder content = new StringBuilder();
			protected PageWatermark Watermark {
				get { return page.ActualWatermark; }
			}
			protected Page Page {
				get { return page; }
			}
			protected Size PageSize {
				get {
					var pageSize = (Page.PageData.Landscape) ? new Size(Page.PageData.Size.Height, Page.PageData.Size.Width) : Page.PageData.Size;
					return GraphicsUnitConverter.Convert(pageSize, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Pixel);
				}
			}
			protected StringBuilder Content {
				get { return content; }
			}
			public WatermarkRtfExportProviderBase(Page page) {
				this.page = page;
			}
			public string GetRtfContent(ExportContext exportContext) {
				SetShapeHeader();
				AppendBounds(GetBounds(exportContext.Measurer));
				SetIndividualWatermarkSettings();
				SetBehindWatermak();
				return Content.ToString();
			}
			protected abstract void SetShapeType();
			protected abstract Rectangle GetBounds(Measurer measurer);
			protected abstract void SetIndividualWatermarkSettings();
			protected void SetBehindWatermak() {
				Content.AppendFormat(RtfTags.ShapeBehind, Watermark.ShowBehind ? 1 : 0);
				Content.Append("}}");
			}
			protected void SetShapeHeader() {
				Content.Append(RtfTags.Shape);
				Content.Append(RtfTags.ShapeInstall);
				Content.Append(RtfTags.NoneWrapShape);
				Content.Append(RtfTags.NoShapeBorderLine);
				SetShapeType();
			}
			protected void AppendBounds(Rectangle bounds) {
				Content.AppendFormat(RtfTags.ShapeBounds, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
			}
		}
		class TextWatermarkRtfExportProvider : WatermarkRtfExportProviderBase {
			float angle;
			public TextWatermarkRtfExportProvider(Page page)
				: base(page) {
			}
			protected override void SetIndividualWatermarkSettings() {
				const int twoByteScale = 65536;
				const int byteScale = 256;
				Content.AppendFormat(RtfTags.ShapeRotation, (int)(angle * twoByteScale));
				Content.AppendFormat(RtfTags.UnicodeShapeString, GetUnicodeString());
				Content.AppendFormat(RtfTags.ShapeFont, Watermark.Font.Name);
				Content.AppendFormat(RtfTags.ShapeColor, ColorTranslator.ToWin32(Watermark.ForeColor));
				Content.AppendFormat(RtfTags.ShapeBoldText, Watermark.Font.Bold ? 1 : 0);
				Content.AppendFormat(RtfTags.ShapeItalicText, Watermark.Font.Italic ? 1 : 0);
				Content.AppendFormat(RtfTags.ShapeOpacity, twoByteScale - (Watermark.TextTransparency * byteScale));
			}
			protected override void SetShapeType() {
				Content.Append(RtfTags.TextShape);
			}
			protected override Rectangle GetBounds(Measurer measurer) {
				const int rightBottomTextRotateAngle = 45;
				const int leftBottomTextRotateAngle = 135;
				const int leftTopTextRotateAngle = 225;
				const int rightTopTextRotateAngle = 315;
				const float xScaleCorrector = 0.92F;
				const float yScaleCorrector = 0.75F;
				CalculateAngle();
				using(RotatedTextHelper helper = new RotatedTextHelper(Watermark.Font, Watermark.StringFormat, Watermark.Text)) {
					Rectangle bounds = helper.GetBounds(Point.Empty, 0f, TextRotation.LeftBottom, 0f, GraphicsUnit.Pixel, measurer);
					bounds.Width = (int)(bounds.Width * xScaleCorrector);
					bounds.Height = (int)(bounds.Height * yScaleCorrector);
					if((angle > rightBottomTextRotateAngle && angle < leftBottomTextRotateAngle) || (angle > leftTopTextRotateAngle && angle < rightTopTextRotateAngle)) {
						int width = bounds.Width;
						bounds.Width = bounds.Height;
						bounds.Height = width;
					}
					bounds.Offset((int)((PageSize.Width - bounds.Width) / 2), (int)((PageSize.Height - bounds.Height) / 2));
					bounds.Offset(-Page.Margins.Left, -Page.Margins.Top);
					bounds = GraphicsUnitConverter.Convert(bounds, GraphicsDpi.Pixel, GraphicsDpi.Twips);
					return bounds;
				}
			}
			string GetUnicodeString() {
				string unString = Watermark.Text;
				MakeStringUnicodeCompatible(ref unString);
				return unString;
			}
			void CalculateAngle() {
				switch(Watermark.TextDirection) {
					case DirectionMode.BackwardDiagonal:
						angle = 50;
						break;
					case DirectionMode.ForwardDiagonal:
						angle = 310;
						break;
					case DirectionMode.Horizontal:
						angle = 0;
						break;
					case DirectionMode.Vertical:
						angle = 270;
						break;
				}
			}
		}
		class ImageWatermarkRtfExportProvider : WatermarkRtfExportProviderBase {
			Size pictureSizeInPixels;
			Point pictureLocationInPixels;
			Size PictureSizeInTwips {
				get { return GraphicsUnitConverter.Convert(pictureSizeInPixels, GraphicsDpi.Pixel, GraphicsDpi.Twips); }
			}
			Point PictureLocationInTwips {
				get { return GraphicsUnitConverter.Convert(pictureLocationInPixels, GraphicsDpi.Pixel, GraphicsDpi.Twips); }
			}
			Point BottomRightLocation {
				get { return new Point(PageSize.Width - pictureSizeInPixels.Width, PageSize.Height - pictureSizeInPixels.Height); }
			}
			public ImageWatermarkRtfExportProvider(Page page)
				: base(page) {
			}
			protected override void SetIndividualWatermarkSettings() {
				using(Image image = DrawImage()) {
					Content.AppendFormat(RtfTags.ShapePicture, GetRtfImageContent(image));
				}
			}
			protected override void SetShapeType() {
				Content.Append(RtfTags.ImageShape);
			}
			protected override Rectangle GetBounds(Measurer measurer) {
				pictureLocationInPixels = Point.Empty;
				if(Watermark.ImageTiling || Watermark.ImageViewMode == ImageViewMode.Stretch)
					pictureSizeInPixels = PageSize;
				else
					SetNoStretchBounds();
				pictureLocationInPixels.Offset(-Page.Margins.Left, -Page.Margins.Top);
				return new Rectangle(PictureLocationInTwips, PictureSizeInTwips);
			}
			void SetNoStretchBounds() {
				if(Watermark.ImageViewMode == ImageViewMode.Clip)
					GetClipingBounds();
				else
					GetZoomBounds();
			}
			void GetZoomBounds() {
				pictureSizeInPixels = GetZoomSize();
				int x = (pictureSizeInPixels.Height != PageSize.Height) ? 0 : BottomRightLocation.X / 2;
				int y = (pictureSizeInPixels.Width != PageSize.Width) ? 0 : BottomRightLocation.Y / 2;
				pictureLocationInPixels = new Point(x, y);
			}
			void GetClipingBounds() {
				pictureSizeInPixels = Watermark.Image.Size;
				var rect = new Rectangle(pictureLocationInPixels, pictureSizeInPixels);
				var baseRect = new Rectangle(Point.Empty, PageSize);
				pictureLocationInPixels = RectHelper.AlignRectangle(rect, baseRect, Watermark.ImageAlign).Location;
			}
			Size GetZoomSize() {
				float xScale = (float)PageSize.Width / (float)Watermark.Image.Width;
				float yScale = (float)PageSize.Height / (float)Watermark.Image.Height;
				if(Watermark.Image.Size.Width * yScale <= PageSize.Width)
					return new Size((int)(Watermark.Image.Size.Width * yScale), PageSize.Height);
				return new Size(PageSize.Width, (int)(Watermark.Image.Size.Height * xScale));
			}
			Image DrawImage() {
				return (!Watermark.ImageTiling || Watermark.ImageViewMode == ImageViewMode.Stretch)
					? (Image)Watermark.Image.Clone()
					: GetTilingImage();
			}
			Image GetTilingImage() {
				var image = new Bitmap(pictureSizeInPixels.Width, pictureSizeInPixels.Height);
				using(Graphics gr = Graphics.FromImage(image)) {
					Size size = (Watermark.ImageViewMode == ImageViewMode.Clip) ? Watermark.Image.Size : GetZoomSize();
					for(int x = 0; x <= (PageSize.Width / size.Width); x++) {
						for(int y = 0; y <= (PageSize.Height / size.Height); y++) {
							gr.DrawImage(Watermark.Image, new Rectangle(new Point(x * size.Width, y * size.Height), size));
						}
					}
				}
				return image;
			}
		}
		class BoundsBorderCorrector {
			const int RtfBorderOutSetCompensator = 30;
			readonly float width;
			RectangleF bounds;
			public RectangleF Bounds {
				get { return bounds; }
			}
			public BoundsBorderCorrector(BorderSide sides, RectangleF bounds, float width) {
				this.bounds = bounds;
				this.width = width;
				ProcessBorderSides(sides);
			}
			void ProcessBorderSides(BorderSide sides) {
				if((sides & BorderSide.Left) > 0) { 
					float offset = RtfBorderOutSetCompensator + width;
					bounds.Offset(offset, 0);
					bounds.Width -= offset;
				}
				if((sides & BorderSide.Right) > 0) {
					bounds.Width -= RtfBorderOutSetCompensator + width;
				}
			}
		}
		#endregion
		#region Fields & Properties
		readonly bool exportWatermarks;
		readonly int[] pageIndices;
		readonly StringBuilder stringBuilder = new StringBuilder();
		Page currentPage;
		ILayoutControl currentInfo;
		public override BrickViewData CurrentData {
			get { return (BrickViewData)currentInfo; }
		}
		Margins TwipsMargins {
			get { return GraphicsUnitConverter.Convert(currentPage.Margins, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Twips); }
		}
#if DEBUGTEST
		internal StringBuilder Test_Content {
			get { return stringBuilder; }
		}
		internal Page Test_CurrentPage {
			get { return currentPage; }
		}
#endif
		#endregion
		#region Constructors
		public RtfPageExportProvider(Stream stream, Document document, RtfExportOptions options)
			: base(stream, new RtfPageExportContext(document.PrintingSystem, new RtfExportHelper())) {
			pageIndices = PageRangeParser.GetIndices(options.PageRange, document.PageCount);
			exportWatermarks = options.ExportWatermarks;
		}
		#endregion
		#region Methods
		public override void SetCellStyle() {
			SetFrameHeader();
			SetBorders();
			SetFrameColor();
		}
		protected override void GetContent() {
			RtfExportContext.ProgressReflector.SetProgressRanges(new float[] { 1f });
			try {
				RtfExportContext.ProgressReflector.InitializeRange(pageIndices.Length);
				foreach(int pageIndex in pageIndices) {
					currentPage = RtfExportContext.PrintingSystem.Document.Pages[pageIndex];
					using(var layoutBuilder = new RtfPageLayoutBuilder(currentPage, RtfExportContext)) {
						LayoutControlCollection layoutControls = layoutBuilder.BuildLayoutControls();
						WritePageBounds();
						if(exportWatermarks && RtfExportContext.PrintingSystem.Watermark.NeedDraw(pageIndex, pageIndices.Length))
							WriteWaterMark();
						for(int index = 0; index < layoutControls.Count; index++) {
							currentInfo = layoutControls[index];
							SetCurrentCell();
						}
						SetContent(RtfTags.SectionEnd);
					}
					RtfExportContext.ProgressReflector.RangeValue++;
				}
			} finally {
				RtfExportContext.ProgressReflector.MaximizeRange();
			}
		}
		protected override void SetContent(string content) {
			stringBuilder.Append(content);
		}
		protected override void SetImageContent(string content) {
			SetContent(content);
		}
		protected override void SetCellUnion() {
			if(!ContentEndsWith(RtfTags.ParagraphEnd))
				SetContent(RtfTags.ParagraphEnd);
			SetContent("}");
		}
		protected override void SetFrameText(string Text) {
			SetCellFont();
			if(Text == string.Empty)
				OverrideFontSizeToPreventDissapearBottomBorder();
			SetContent(" ");
			SetContent(Text);
		}
		protected override void WriteContent() {
			writer.WriteLine(stringBuilder.ToString());
		}
		protected override void WritePageBounds() {
			SetContent(RtfTags.SectionDefault);
			if(currentPage.PageData.Landscape)
				SetContent(RtfTags.PageLandscape);
			int width = ConvertToTwips(currentPage.PageData.Bounds.Width, GraphicsDpi.HundredthsOfAnInch);
			int height = ConvertToTwips(currentPage.PageData.Bounds.Height, GraphicsDpi.HundredthsOfAnInch);
			SetContent(String.Format(RtfTags.PageSize, width, height));
			WriteMargins();
		}
		protected override void WriteMargins() {
			SetContent(String.Format(RtfTags.Margins, TwipsMargins.Left, TwipsMargins.Right, TwipsMargins.Top, TwipsMargins.Bottom));
		}
		protected bool ContentEndsWith(string rtfTag) {
			return stringBuilder.Length >= rtfTag.Length
				&& stringBuilder.ToString(stringBuilder.Length - rtfTag.Length, rtfTag.Length) == rtfTag;
		}
		#region Borders processing
		protected void SetBorders() {
			if(CurrentStyle == null || CurrentStyle.Sides == BorderSide.None || CurrentStyle.BorderWidth == 0)
				return;
			BorderSide sides = CurrentStyle.Sides;
			if((sides & BorderSide.Top) > 0) {
				SetBorder(RtfTags.TopBorder);
			}
			if((sides & BorderSide.Bottom) > 0) {
				SetBorder(RtfTags.BottomBorder);
			}
			if((sides & BorderSide.Left) > 0) {
				SetBorder(RtfTags.LeftBorder);
			}
			if((sides & BorderSide.Right) > 0) {
				SetBorder(RtfTags.RightBorder);
			}
		}
		void SetBorder(string border) {
			SetContent(border);
			SetBorderStyle();
		}
		void SetBorderStyle() {
			SetContent(RtfExportBorderHelper.GetFullBorderStyle(BorderStyle, (int)BorderWidth, rtfExportHelper.GetColorIndex(CurrentStyle.BorderColor)));
		}
		#endregion
		void WriteWaterMark() {
			if(currentPage.ActualWatermark.Image != null) {
				ImageWatermarkRtfExportProvider watermarkProvider = new ImageWatermarkRtfExportProvider(currentPage);
				SetContent(watermarkProvider.GetRtfContent(ExportContext));
			}
			if(currentPage.ActualWatermark.Text != string.Empty) {
				TextWatermarkRtfExportProvider watermarkProvider = new TextWatermarkRtfExportProvider(currentPage);
				SetContent(watermarkProvider.GetRtfContent(ExportContext));
			}
		}
		void SetFrameColor() {
			SetContent(String.Format(RtfTags.BackgroundPatternColor, rtfExportHelper.GetColorIndex(CurrentStyle.BackColor)));
		}
		void SetFrameHeader() {
			SetContent("{");
			SetContent(RtfTags.ParagraphDefault);
			SetContent(RtfTags.TextUnderneath);
			Rectangle bounds = GetFrameBounds();
			SetContent(String.Format(RtfTags.ObjectBounds, bounds.Left, bounds.Top, bounds.Width, bounds.Height));
			SetFrame();
			SetDirection(); 
			SetCellGAlign();
		}
		void SetCellGAlign() {
			SetContent(RtfAlignmentConverter.ToHorzRtfAlignment(CurrentStyle.TextAlignment));
		}
		public override Rectangle GetFrameBounds() {
			RectangleF boundsF = GraphicsUnitConverter.Convert(CurrentData.BoundsF, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Twips);
			boundsF = BorderWidth != 0f ? new BoundsBorderCorrector(CurrentStyle.Sides, boundsF, BorderWidth).Bounds : boundsF;
			Rectangle bounds = Rectangle.Round(boundsF);
			return new Rectangle(bounds.Left - TwipsMargins.Left, bounds.Top, bounds.Width, -bounds.Height);
		}
		public override PrintingParagraphAppearance GetParagraphAppearance() { 
			int borderColor = rtfExportHelper.GetColorIndex(CurrentStyle.BorderColor);
			int frameColor = rtfExportHelper.GetColorIndex(CurrentStyle.BackColor);
			return new PrintingParagraphAppearance(GetFrameBounds(), CurrentStyle.Sides, (int)BorderWidth, borderColor, frameColor);
		}
		void SetCellFont() {
			SetContent(String.Format(RtfTags.Color, rtfExportHelper.GetColorIndex(CurrentStyle.ForeColor)));
			SetContent(GetFontString());
		}
		void SetFrame() {
			SetContent(RtfTags.DefaultFrame);
			SetContent(RtfTags.RelativeFrameToPage);
		}
		#endregion
	}
}
