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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Export.Rtf {
	public abstract class RtfDocumentProviderBase : RtfExportProviderBase, IRtfExportProvider {
		public struct PrintingParagraphAppearance {
			public Rectangle Bounds;
			public BorderSide BorderSides;
			public int BorderWidth;
			public int BorderColorIndex;
			public int BackgroundColorIndex;
			public PrintingParagraphAppearance(Rectangle bounds, BorderSide borderSides, int borderWidth, int borderColorIndex, int backgroundColorIndex) {
				this.Bounds = bounds;
				this.BorderSides = borderSides;
				this.BorderWidth = borderWidth;
				this.BorderColorIndex = borderColorIndex;
				this.BackgroundColorIndex = backgroundColorIndex;
			}
		}
		#region Fields & Properties
		readonly static string newLine = new string(new char[] { '\r', '\n' });
		readonly RtfExportContext rtfExportContext;
		string pageNumberingInfo;
		protected string PageNumberingInfo {
			get { return pageNumberingInfo; }
		}
		public ExportContext ExportContext {
			get { return rtfExportContext; }
		}
		public abstract BrickViewData CurrentData {
			get;
		}
		protected BrickStyle CurrentStyle {
			get { return CurrentData.Style; }
		}
		protected float BorderWidth {
			get { return ConvertToTwips(CurrentStyle.BorderWidth, GraphicsDpi.DeviceIndependentPixel); }
		}
		protected BorderDashStyle BorderStyle {
			get { return CurrentStyle.BorderDashStyle; }
		}
		protected bool RightToLeft {
			get { return CurrentStyle.StringFormat.RightToLeft; }
		}
		#endregion
		#region Constructors
		protected RtfDocumentProviderBase(Stream stream, RtfExportContext rtfExportContext)
			: base(stream, rtfExportContext.RtfExportHelper) {
			this.rtfExportContext = rtfExportContext;
		}
		#endregion
		#region Methods
		public virtual Rectangle GetFrameBounds() {
			return Rectangle.Empty;
		}
		public virtual PrintingParagraphAppearance GetParagraphAppearance() {
			return new PrintingParagraphAppearance();
		}
		public static string GetRtfImageContent(Image image, PaddingInfo padding) {
			return GetRtfImageContent(image, image.RawFormat, padding);
		}
		public static string GetRtfImageContent(Image image) {
			return GetRtfImageContent(image, image.RawFormat);
		}
		public static string GetRtfImageContent(Image image, ImageFormat imageFormat) {
			return GetRtfImageContent(image, imageFormat, PaddingInfo.Empty);
		}
		public virtual void CreateDocument() {
			GetContent();
			Commit();
		}
		public abstract void SetCellStyle();
		internal static bool IsSpecialSymbol(char ch) {
			return ch == '{'
				|| ch == '}'
				|| ch == '\\';
		}
		protected virtual void OverrideFontSizeToPreventDissapearBottomBorder() {
			const int minimumHeight = 6;
			if(CurrentData.Bounds.Height <= minimumHeight)
				SetContent(String.Format(RtfTags.FontSize, 1));
		}
		protected void OverrideFontSizeToPreventIncorrectImageAlignment() {
			SetContent(String.Format(RtfTags.FontSize, 1));
		}
		protected void SetDirection() { 
			if(RightToLeft)
				SetContent(RtfTags.RightToLeftParagraph);
		}
		protected string GetFontString() {
			Font font = CurrentStyle.Font;
			string fontString = String.Empty;
			if(font.Strikeout) fontString += RtfTags.Strikeout;
			if(font.Underline) fontString += RtfTags.Underline;
			if(font.Italic) fontString += RtfTags.Italic;
			if(font.Bold) fontString += RtfTags.Bold;
			fontString += String.Format(RtfTags.FontWithSize, rtfExportHelper.GetFontNameIndex(font.Name), GetFontSize(font));
			return fontString;
		}
		protected void SetCurrentCell() {
			if(CurrentStyle != null)
				SetCellStyle();
			OverrideFontSizeToPreventDissapearBottomBorder();
			var brickExporter = (BrickExporter)ExportContext.PrintingSystem.ExportersFactory.GetExporter(CurrentData.TableCell);
			brickExporter.FillRtfTableCell(this);
			SetCellUnion();
		}
		protected static int ConvertToTwips(float value, float fromDpi) {
			return (int)GraphicsUnitConverter.Convert(value, fromDpi, GraphicsDpi.Twips);
		}
		protected static string MakeHyperlink(string content, string hyperlink) {
			if(!Uri.IsWellFormedUriString(hyperlink, UriKind.Absolute))
				return String.Format(RtfTags.LocalHyperlink, hyperlink, content);
			return String.Format(RtfTags.Hyperlink, hyperlink, content);
		}
		protected override abstract void WriteContent();
		protected abstract void GetContent();
		protected abstract void WritePageBounds();
		protected abstract void WriteMargins();
		protected abstract void SetCellUnion();
		protected abstract void SetContent(string content);
		protected abstract void SetImageContent(string content);
		protected abstract void SetFrameText(string text);
		static int GetFontSize(Font font) {
			return (int)Math.Floor(FontSizeHelper.GetSizeInPoints(font) * 2f);
		}
		static public void MakeLineBreaks(ref string stringData) {
			stringData = stringData.Replace(newLine, RtfTags.EndOfLine);
		}
		static string GetRtfImageContent(Image image, ImageFormat imageFormat, PaddingInfo padding) {
			if(image == null)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			byte[] buffer = PSConvert.ImageToArray(image, imageFormat);
			if(buffer.Length == 0)
				return string.Empty;
			int imageWidth = GraphicsUnitConverter.Convert(image.Width, image.HorizontalResolution, GraphicsDpi.Twips);
			int imageHeight = GraphicsUnitConverter.Convert(image.Height, image.VerticalResolution, GraphicsDpi.Twips);
			int paddingLeft = GraphicsUnitConverter.Convert(padding.Left, image.HorizontalResolution, GraphicsDpi.Twips);
			int paddingRight = GraphicsUnitConverter.Convert(padding.Right, image.HorizontalResolution, GraphicsDpi.Twips);
			int paddingTop = GraphicsUnitConverter.Convert(padding.Top, image.VerticalResolution, GraphicsDpi.Twips);
			int paddingBottom = GraphicsUnitConverter.Convert(padding.Bottom, image.VerticalResolution, GraphicsDpi.Twips);
			WriteTagsToStringBuilder(imageWidth, imageHeight, paddingTop, paddingBottom, paddingLeft, paddingRight, sb, imageFormat);
			WriteImageToStringBuilder(buffer, sb);
			return sb.ToString();
		}
		static public void MakeStringUnicodeCompatible(ref string stringData) {
			int length = 0;
			while(length < stringData.Length) {
				char ch = stringData[length];
				ushort code = Convert.ToUInt16(ch);
				if(code > 127) {
					string str = String.Format(RtfTags.UnicodeCharacter + "  ", code);
					stringData = stringData.Insert(length + 1, str);
					stringData = stringData.Remove(length, 1);
					length += str.Length;
					continue;
				} else {
					if(IsSpecialSymbol(ch)) {
						stringData = stringData.Insert(length, @"\");
						length++;
					}
				}
				length++;
			}
		}
		static void WriteTagsToStringBuilder(int picwgoal, int pichgoal, int piccropt, int piccropb, int piccropl, int piccropr, StringBuilder sb, ImageFormat imageFormat) {
			string imageTag = (imageFormat == ImageFormat.Wmf) ? String.Format(RtfTags.WindowsMetafile, 8) : RtfTags.PngPictType;
			sb.Append(String.Format("{{" + RtfTags.Picture + RtfTags.NewLine, imageTag));
			sb.Append(String.Format(RtfTags.PictureDesiredWidth, picwgoal));
			sb.Append(String.Format(RtfTags.PictureDesiredHeight, pichgoal));
			AppendNonZeroValue(sb, RtfTags.PictureCropTop, piccropt);
			AppendNonZeroValue(sb, RtfTags.PictureCropBottom, piccropb);
			AppendNonZeroValue(sb, RtfTags.PictureCropLeft, piccropl);
			AppendNonZeroValue(sb, RtfTags.PictureCropRight, piccropr);
			sb.Append(RtfTags.NewLine);
		}
		static void AppendNonZeroValue(StringBuilder sb, string tag, int value) {
			if(value != 0)
				sb.Append(String.Format(tag, value));
		}
		static void WriteImageToStringBuilder(byte[] buffer, StringBuilder sb) {
			if(buffer == null)
				return;
			StringBuilder tempStringBuilder = new StringBuilder(256);
			int length = buffer.Length;
			int byteIndex = 0;
			int rowIndex = 0;
			int colIndex = 0;
			for(rowIndex = 0; (rowIndex < length); rowIndex += 64) {
				for(colIndex = 0; ((colIndex < 64) && ((rowIndex + colIndex) < length)); ++colIndex) {
					tempStringBuilder.AppendFormat("{0:X2}", buffer[byteIndex]);
					++byteIndex;
				}
				sb.Append(tempStringBuilder.ToString());
				tempStringBuilder.Length = 0;
				sb.Append(RtfTags.NewLine);
			}
			sb.Append('}');
		}
		#region IRtfExportProvider Members
		public RtfExportContext RtfExportContext {
			get { return rtfExportContext; }
		}
		void IRtfExportProvider.SetContent(string content) {
			SetContent(content);
		}
		void IRtfExportProvider.SetAnchor(string anchorName) {
			if(string.IsNullOrEmpty(anchorName))
				return;
			SetContent(String.Format(RtfTags.Bookmark, anchorName));
		}
		void IRtfExportProvider.SetPageInfo(PageInfo pageInfo, int startPageNumber, string textValue, string hyperLink) {
			switch(pageInfo) {
				case PageInfo.Number:
				case PageInfo.RomHiNumber:
				case PageInfo.RomLowNumber:
					SetContent(String.Format(RtfTags.FieldInstructionTemplate, RtfTags.FieldInstructionPageNumber, "#"));
					if(pageInfo == PageInfo.RomHiNumber)
						pageNumberingInfo = @"\pgnucrm";
					else if(pageInfo == PageInfo.RomLowNumber)
						pageNumberingInfo = @"\pgnlcrm";
					else
						pageNumberingInfo = @"\pgndec";
					if(startPageNumber > 1)
						pageNumberingInfo += string.Format(@"\pgnstarts{0}\pgnrestart\", startPageNumber);
					break;
				case PageInfo.Total:
					SetContent(String.Format(RtfTags.FieldInstructionTemplate, RtfTags.FieldInstructionPageCount, "#"));
					break;
				case PageInfo.NumberOfTotal:
					SetContent(String.Format(RtfTags.FieldInstructionTemplate, RtfTags.FieldInstructionPageNumber, "#") + "{/}" +
						String.Format(RtfTags.FieldInstructionTemplate, RtfTags.FieldInstructionPageCount, "#"));
					break;
				case PageInfo.UserName:
					SetContent(String.Format(RtfTags.FieldInstructionTemplate, RtfTags.FieldInstructionUserName, textValue));
					break;
				default:
					SetCellText(textValue, hyperLink);
					break;
			}
		}
		#endregion
		#region ITableExportProvider Members
		public void SetCellText(object textValue, string hyperLink) {
			string text = textValue as string;
			if(!String.IsNullOrEmpty(text)) {
				if(!CurrentStyle.StringFormat.FormatFlags.HasFlag(StringFormatFlags.MeasureTrailingSpaces))
					text = text.TrimEnd();
				text = HotkeyPrefixHelper.PreprocessHotkeyPrefixesInString(text, CurrentStyle);
				MakeStringUnicodeCompatible(ref text);
				if(!string.IsNullOrEmpty(hyperLink)) {
					MakeStringUnicodeCompatible(ref hyperLink);
					text = MakeHyperlink(text, hyperLink);
				}
				MakeLineBreaks(ref text);
			}
			SetFrameText(text);
		}
		public void SetCellImage(Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imageSize, PaddingInfo padding, string hyperLink) {
			if(image == null)
				return;
			string content = GetRtfImageContent(image, padding);
			if(!string.IsNullOrEmpty(hyperLink))
				content = MakeHyperlink(content, hyperLink);
			OverrideFontSizeToPreventIncorrectImageAlignment();
			SetImageContent(content);
		}
		void ITableExportProvider.SetCellShape(Color lineColor, XtraReports.UI.LineDirection lineDirection, System.Drawing.Drawing2D.DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink) {
		}
		#endregion
		#endregion
	}
}
