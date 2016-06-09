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
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.Export.Web {
	interface INavigationService {
		string GetMouseDownScript(HtmlExportContext htmlExportContext, VisualBrick brick);
	}
	public class WebNavigationService : INavigationService {
		protected string Url {
			get;
			private set;
		}
		protected VisualBrick Brick {
			get;
			private set;
		}
		protected HtmlExportContext Context {
			get;
			private set;
		}
		string INavigationService.GetMouseDownScript(HtmlExportContext htmlExportContext, VisualBrick brick) {
			Url = brick.GetActualUrl();
			Brick = brick;
			Context = htmlExportContext;
			return CreateNavigationScript();
		}
		protected virtual string CreateNavigationScript() {
			if(string.IsNullOrEmpty(Url))
				return string.Empty;
			if(Url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
				return Url;
			if(Brick.NavigationPair == BrickPagePair.Empty) {
				return string.Format("ASPx.xr_NavigateUrl('{0}', '{1}')", DXHttpUtility.UrlEncodeToUnicodeCompatible(Url), Brick.Target);
			} else if(Context.CrossReferenceAvailable) {
				string navigateUrl = HtmlHelper.GetHtmlUrl(Url);
				return string.Format("ASPx.xr_NavigateUrl('{0}', '{1}')", DXHttpUtility.UrlEncodeToUnicodeCompatible(navigateUrl), Brick.Target);
			}
			return string.Empty;
		}
	}
	[Obsolete("Use the INavigationService interface instead.")]
	interface IBookmarkService {
		string GetNavigationScript(int pageIndex, int[] birckIndices);
	}
	public abstract class ImageRepositoryRequest {
		#region events
		ImageEventHandler onRequestImageSource;
		public virtual event ImageEventHandler RequestImageSource {
			add { onRequestImageSource = System.Delegate.Combine(onRequestImageSource, value) as ImageEventHandler; }
			remove { onRequestImageSource = System.Delegate.Remove(onRequestImageSource, value) as ImageEventHandler; }
		}
		protected void RaiseRequestImageSource(Image image) {
			if(onRequestImageSource != null)
				onRequestImageSource(this, new ImageEventArgs(image));
		}
		#endregion
	}
	public abstract class ImageRepositoryBase : ImageRepositoryRequest, IImageRepository {
		public abstract void Dispose();
		protected abstract string FormatImageURL(string imageFileName);
		protected abstract void SaveImage(Image image, string fileName);
		protected abstract void AddImageToTable(Image image, long key, string url, bool autoDisposeImage);
		protected abstract string GetUrl(long key);
		protected abstract void FinalizeImage(Image image, bool autoDisposeImage);
		protected abstract bool ContainsImageKey(long key);
		string IImageRepository.GetImageSource(Image img, bool autoDisposeImage) {
			if(img == null)
				return String.Empty;
			RaiseRequestImageSource(img);
			long key = HtmlImageHelper.GetImageHashCode(img);
			if(ContainsImageKey(key))
				return GetUrl(key);
			try {
				string ext = HtmlImageHelper.GetMimeType(img);
				string fileName = String.Format("{0}.{1}", key, ext);
				SaveImage(img, fileName);
				string url = FormatImageURL(fileName);
				AddImageToTable(img, key, url, autoDisposeImage);
				FinalizeImage(img, autoDisposeImage);
				return url;
			}
			catch {
				return String.Empty;
			}
		}
	}
	public class InMemoryHtmlImageRepository : ImageRepositoryBase {
		Dictionary<long, string> imageUrlTable = new Dictionary<long, string>();
		protected string imagePath = "";
		public InMemoryHtmlImageRepository(string imagePath) {
			this.imagePath = imagePath;
		}
		protected override string FormatImageURL(string imageFileName) {
			return imagePath + "/" + imageFileName;
		}
		protected override void SaveImage(Image image, string fileName) {
		}
		public override void Dispose() {
		}
		protected override bool ContainsImageKey(long key) {
			return imageUrlTable.ContainsKey(key);
		}
		protected override void AddImageToTable(Image image, long key, string url, bool autoDisposeImage) {
			imageUrlTable.Add(key, url);
		}
		protected override string GetUrl(long key) {
			string result;
			if(imageUrlTable.TryGetValue(key, out result))
				return result;
			else
				return String.Empty;
		}
		protected override void FinalizeImage(Image image, bool autoDisposeImage) {
			if(autoDisposeImage)
				image.Dispose();
		}
	}
	public class HtmlImageRepository : InMemoryHtmlImageRepository {
		static void CreateDirectory(string path) {
			if(!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
				File.SetAttributes(path, FileAttributes.Archive);
			}
		}
		string rootPath = "";
		public HtmlImageRepository(string rootPath, string imagePath)
			: base(imagePath) {
			this.rootPath = rootPath;
		}
		protected override void SaveImage(Image image, string fileName) {
			string path = Path.Combine(rootPath, imagePath);
			CreateDirectory(path);
			HtmlImageHelper.SaveImage(image, Path.Combine(path, fileName));
		}
	}
	public class CssImageRepository : ImageRepositoryBase, IImageRepository {
		Dictionary<Image, long> imageKeyTable = new Dictionary<Image, long>();
		Dictionary<long, string> keyBase64Table = new Dictionary<long, string>();
		Dictionary<long, string> keyCssClassTable = new Dictionary<long, string>();
		IScriptContainer scriptContainer;
		ImageConverter imageConverter;
		public IScriptContainer ScriptContainer {
			get { return scriptContainer; }
			set {
				Clear();
				scriptContainer = value;
			}
		}
		ImageConverter ImageConverter {
			get {
				if(imageConverter == null)
					imageConverter = new ImageConverter();
				return imageConverter;
			}
			set { imageConverter = value; }
		}
		public CssImageRepository() {
		}
		void Clear() {
			imageKeyTable.Clear();
			keyCssClassTable.Clear();
			keyBase64Table.Clear();
		}
		byte[] ToBytes(Image image) {
			return (byte[])this.ImageConverter.ConvertTo(image, typeof(byte[]));
		}
		public string GetClassNameByImage(Image image) {
			byte[] bytes = null;
			long key = GetKey(image, ref bytes);
			string cssClass;
			if(!keyCssClassTable.TryGetValue(key, out cssClass) && ScriptContainer != null) {
				string base64 = GetBase64(key, image, bytes);
				cssClass = ScriptContainer.RegisterCssClass(
					string.Format("background-image:url(data:image/{0};base64,{1});background-repeat:no-repeat;",
					HtmlImageHelper.GetMimeType(image), base64));
				keyCssClassTable.Add(key, cssClass);
			}
			return cssClass;
		}
		public string GetWatermarkDataByImage(Image image) {
			byte[] bytes = null;
			long key = GetKey(image, ref bytes);
			return string.Format("data:image/{0};base64,{1}", HtmlImageHelper.GetMimeType(image), GetBase64(key, image, bytes));
		}
		public override void Dispose() {
			ScriptContainer = null;
			ImageConverter = null;
		}
		protected override string FormatImageURL(string imageFileName) {
			return string.Empty;
		}
		protected override void SaveImage(Image image, string fileName) {
			throw new NotSupportedException();
		}
		protected override void AddImageToTable(Image image, long key, string url, bool autoDisposeImage) {
			throw new NotSupportedException();
		}
		protected override string GetUrl(long key) {
			return string.Empty;
		}
		protected override void FinalizeImage(Image image, bool autoDisposeImage) {
			if(autoDisposeImage)
				image.Dispose();
		}
		protected override bool ContainsImageKey(long key) {
			return keyCssClassTable.ContainsKey(key);
		}
		string GetBase64(Image image) {
			byte[] bytes = null;
			long key = GetKey(image, ref bytes);
			return GetBase64(key, image, bytes);
		}
		string GetBase64(long key, Image image, byte[] bytes) {
			string value;
			if(!keyBase64Table.TryGetValue(key, out value)) {
				if(bytes == null)
					bytes = ToBytes(image);
				value = Convert.ToBase64String(bytes);
				keyBase64Table.Add(key, value);
			}
			return value;
		}
		long GetKey(Image image, ref byte[] bytes) {
			long key;
			if(!imageKeyTable.TryGetValue(image, out key)) {
				bytes = ToBytes(image);
				key = GetHashCode(bytes);
				imageKeyTable.Add(image, key);
			}
			return key;
		}
		static long GetHashCode(byte[] bytes) {
			return Adler32.CalculateChecksum(bytes);
		}
		public int GetStreamLength(Image image) {
			return GetBase64(image).Length;
		}
		#region IImageRepository Members
		public string GetImageSource(Image image, bool autoDisposeImage) {
			RaiseRequestImageSource(image);
			return string.Format("data:image/{0};base64,{1}", HtmlImageHelper.GetMimeType(image), GetBase64(image));
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Clear();
			Dispose();
		}
		#endregion
	}
	public class ImageInfo {
		Image image;
		string contentId;
		bool disposeImage;
		public ImageInfo(Image img, string contentId, bool disposeImage) {
			this.image = img;
			this.contentId = contentId;
			this.disposeImage = disposeImage;
		}
		public Image Image { get { return image; } }
		public string ContentId { get { return contentId; } }
		public void FinalizeImage() {
			if(disposeImage && image != null) {
				image.Dispose();
				image = null;
			}
		}
	}
	public class MhtImageRepository : ImageRepositoryBase {
		internal const string ContentIdPrefix = "cid:";
		Dictionary<long, ImageInfo> imageTable = new Dictionary<long, ImageInfo>();
		internal static string GetImageContentId(string url) {
			return url.Substring(ContentIdPrefix.Length);
		}
		public MhtImageRepository() : base() {
		}
		public Dictionary<long, ImageInfo> ImagesTable { get { return imageTable; } }
		public override void Dispose() {
			foreach(ImageInfo info in ImagesTable.Values) {
				info.FinalizeImage();
			}
		}
		protected override string FormatImageURL(string imageFileName) {
			return ContentIdPrefix + imageFileName;
		}
		protected override void SaveImage(Image image, string fileName) {
		}
		protected override bool ContainsImageKey(long key) {
			return imageTable.ContainsKey(key);
		}
		protected override void AddImageToTable(Image image, long key, string url, bool autoDisposeImage) {
			imageTable.Add(key, new ImageInfo(image, GetImageContentId(url), autoDisposeImage));
		}
		protected override string GetUrl(long key) {
			ImageInfo result;
			if(imageTable.TryGetValue(key, out result))
				return ContentIdPrefix + result.ContentId;
			return String.Empty;
		}
		protected override void FinalizeImage(Image image, bool autoDisposeImage) {
		}
	}
	public class MailImageRepository : ImageRepositoryBase {
		internal const string ContentIdPrefix = "cid:";
		Dictionary<long, ImageInfo> imageTable = new Dictionary<long, ImageInfo>();
		internal static string GetImageContentId(string url) {
			return url.Substring(ContentIdPrefix.Length);
		}
		public MailImageRepository()
			: base() {
		}
		public Dictionary<long, ImageInfo> ImagesTable { get { return imageTable; } }
		public override void Dispose() {
			foreach(ImageInfo info in ImagesTable.Values) {
				info.FinalizeImage();
			}
		}
		protected override string FormatImageURL(string imageFileName) {
			return ContentIdPrefix + imageFileName;
		}
		protected override void SaveImage(Image image, string fileName) {
		}
		protected override bool ContainsImageKey(long key) {
			return imageTable.ContainsKey(key);
		}
		protected override void AddImageToTable(Image image, long key, string url, bool autoDisposeImage) {
			bool skipBlankGif = (image.Size.Height == 1 && image.Size.Width == 1);
			if(skipBlankGif)
				return;
			imageTable.Add(key, new ImageInfo(image, GetImageContentId(url), autoDisposeImage));
		}
		protected override string GetUrl(long key) {
			ImageInfo result;
			if(imageTable.TryGetValue(key, out result))
				return ContentIdPrefix + result.ContentId;
			return String.Empty;
		}
		protected override void FinalizeImage(Image image, bool autoDisposeImage) {
		}
	}
	public static class PSHtmlStyleRender {
		public static string GetHtmlStyle(Font font, Color foreColor, Color backColor, Color borderColor, PaddingInfo borders, PaddingInfo padding, BorderDashStyle borderStyle) {
			return String.Format("color:{0};background-color:{1};{2}{3}{4}",
				HtmlConvert.ToHtml(foreColor), HtmlConvert.ToHtml(backColor),
				GetBorderHtml(borderColor, backColor, borders, borderStyle), HtmlStyleRender.GetFontHtmlInPixels(font), GetHtmlPadding(padding));
		}
		static object GetHtmlPadding(PaddingInfo padding) {
			PaddingInfo pixPadding = new PaddingInfo(padding, GraphicsDpi.DeviceIndependentPixel);
			string result = string.Empty;
			if(pixPadding.Top > 0)
				result += "padding-top:" + pixPadding.Top + "px;";
			if(pixPadding.Left > 0)
				result += "padding-left:" + pixPadding.Left + "px;";
			if(pixPadding.Right > 0)
				result += "padding-right:" + pixPadding.Right + "px;";
			if(pixPadding.Bottom > 0)
				result += "padding-bottom:" + pixPadding.Bottom + "px;";
			return result;
		}
		public static string GetBorderHtml(Color borderColor, Color backColor, BorderSide sides, int borderWidth) {
			return GetBorderHtml(borderColor, backColor, GetBorders(sides, borderWidth), BorderDashStyle.Solid);
		}
		public static PaddingInfo GetBorders(BorderSide sides, int borderWidth) {
			return new PaddingInfo(
				(sides & BorderSide.Left) != 0 ? borderWidth : 0,
				(sides & BorderSide.Right) != 0 ? borderWidth : 0,
				(sides & BorderSide.Top) != 0 ? borderWidth : 0,
				(sides & BorderSide.Bottom) != 0 ? borderWidth : 0
			);
		}
		public static PaddingInfo GetBorders(BrickStyle style) {
			return GetBorders(style.Sides, (int)Math.Round(style.BorderWidth));
		}
		public static string GetBorderHtml(Color borderColor, Color backColor, PaddingInfo borders, BorderDashStyle borderStyle) {
			StringBuilder sb = new StringBuilder();
			AppendBorderSide(sb, borderColor, borders, borderStyle, DevExpress.XtraPrinting.BorderSide.Left);
			AppendBorderSide(sb, borderColor, borders, borderStyle, DevExpress.XtraPrinting.BorderSide.Top);
			AppendBorderSide(sb, borderColor, borders, borderStyle, DevExpress.XtraPrinting.BorderSide.Right);
			AppendBorderSide(sb, borderColor, borders, borderStyle, DevExpress.XtraPrinting.BorderSide.Bottom);
			return sb.ToString();
		}
		static void AppendBorderSide(StringBuilder sb, Color borderColor, PaddingInfo borders, BorderDashStyle borderStyle, BorderSide side) {
			int borderWidth = GetBorderWidth(borders, side);
			if(borderWidth == 0)
				sb.AppendFormat("border-{0}-style: none;", GetHtmlBorderSide(side));
			else
				sb.AppendFormat("border-{0}:{1} {2}px {3};", GetHtmlBorderSide(side), HtmlConvert.ToHtml(borderColor), borderWidth, GetHtmlBorderStyle(borderStyle));
		}
		static string GetHtmlBorderSide(BorderSide side) {
			switch(side) {
				case BorderSide.Left: return "left";
				case BorderSide.Top: return "top";
				case BorderSide.Right: return "right";
				case BorderSide.Bottom: return "bottom";
			}
			throw new NotImplementedException();
		}
		static int GetBorderWidth(PaddingInfo borders, BorderSide side) {
			switch(side) {
				case BorderSide.Left: return borders.Left;
				case BorderSide.Top: return borders.Top;
				case BorderSide.Right: return borders.Right;
				case BorderSide.Bottom: return borders.Bottom;
			}
			return 0;
		}
		static string GetHtmlBorderStyle(BorderDashStyle borderStyle) {
			switch(borderStyle) {
				case BorderDashStyle.Dash:
				case BorderDashStyle.DashDot:
				case BorderDashStyle.DashDotDot:
					return "dashed";
				case BorderDashStyle.Dot:
					return "dotted";
				case BorderDashStyle.Double:
					return "double";
				case BorderDashStyle.Solid:
				default:
					return "solid";
			}
		}
		static string GetHtmlImagePosition(ContentAlignment alignment) { 
			switch(alignment) {
				case ContentAlignment.BottomCenter:
					return "center bottom";
				case ContentAlignment.BottomLeft:
					return "left bottom";
				case ContentAlignment.BottomRight:
					return "right bottom";
				case ContentAlignment.MiddleCenter:
					return "center center";
				case ContentAlignment.MiddleLeft:
					return "left center";
				case ContentAlignment.MiddleRight:
					return "right center";
				case ContentAlignment.TopCenter:
					return "center top";
				case ContentAlignment.TopRight:
					return "right top";
				case ContentAlignment.TopLeft:
				default:
					return "left top";
			}
		}
		public static int GetHtmlTextDirection(DevExpress.XtraPrinting.Drawing.DirectionMode textDirection) {
			switch(textDirection) {
				case Drawing.DirectionMode.BackwardDiagonal:
					return 50;
				case Drawing.DirectionMode.ForwardDiagonal:
					return -50;
				case Drawing.DirectionMode.Vertical:
					return -90;
				case Drawing.DirectionMode.Horizontal:
				default:
					return 0;
			}
		}
		public static string GetHtmlWatermarkImageStyle(Size pageSize, Point offset, bool needClipMargins, string imageSrc, DevExpress.XtraPrinting.Drawing.PageWatermark pageWatermark) {
			string style = String.Format("width:{0}px;height:{1}px;position:absolute;", pageSize.Width, pageSize.Height);
			if(needClipMargins)
				style += String.Format("margin-top:{0}px;margin-left:{1}px;", -offset.Y, -offset.X);
			if(!string.IsNullOrEmpty(imageSrc)) {
				style += String.Format("background-image:url('{0}');", imageSrc);
				Image image = pageWatermark.Image;
				bool tiling = pageWatermark.ImageTiling;
				ContentAlignment align = pageWatermark.ImageAlign;
				float scaleX = GraphicsDpi.DeviceIndependentPixel / image.HorizontalResolution;
				float scaleY = GraphicsDpi.DeviceIndependentPixel / image.VerticalResolution;
				bool bgRepeat = false;
				Size bgSize = new Size((int)Math.Round(image.Size.Width * scaleX), (int)Math.Round(image.Size.Height * scaleY));
				string bgAlignment = "left top";
				switch(pageWatermark.ImageViewMode) {
					case Drawing.ImageViewMode.Clip:
						if(tiling)
							bgRepeat = true;
						else
							bgAlignment = GetHtmlImagePosition(align);
						break;
					case Drawing.ImageViewMode.Stretch:
						bgSize = pageSize;
						break;
					case Drawing.ImageViewMode.Zoom:
						float scale = DevExpress.XtraPrinting.Drawing.WatermarkHelper.GetAdjustedScale(pageSize, image.Size);
						bgSize = new Size((int)(image.Size.Width * scale), (int)(image.Size.Height * scale));
						if(tiling)
							bgRepeat = true;
						else {
							ContentAlignment alignment = DevExpress.XtraPrinting.Drawing.WatermarkHelper.GetAdjustedAlignment(pageSize, image.Size, align);
							bgAlignment = GetHtmlImagePosition(align);
						}
						break;
				}
				style += String.Format("background-size:{0}px {1}px;background-position:{2};background-repeat:{3};",
					bgSize.Width.ToString(), bgSize.Height.ToString(), bgAlignment, bgRepeat ? "repeat" : "no-repeat");
				style += GetOpacityStyle(pageWatermark.ImageTransparency);
			}
			return style;
		}
		public static string GetHtmlWatermarkTextStyle(Size pageSize, Point offset, bool needClipMargins, Size textSize, DevExpress.XtraPrinting.Drawing.PageWatermark pageWatermark) {
			string style = String.Format("width:{0}px;height:{1}px;position:absolute;line-height:{1}px;{2}", textSize.Width, textSize.Height, HtmlStyleRender.GetFontHtmlInPixels(pageWatermark.Font));
			style += String.Format("text-align:center;color:{0};", HtmlConvert.ToHtml(pageWatermark.ForeColor));
			int marginTop = (int)((pageSize.Height - textSize.Height) / 2f );
			int marginLeft = (int)((pageSize.Width - textSize.Width) / 2f );
			if(needClipMargins) { 
				marginTop -= offset.Y;
				marginLeft -= offset.X;
			}
			style += String.Format("margin-top:{0}px;margin-left:{1}px;", marginTop, marginLeft);
			style += String.Format("-webkit-transform:rotate({0}deg);transform:rotate({0}deg);", GetHtmlTextDirection(pageWatermark.TextDirection));
			style += GetOpacityStyle(pageWatermark.TextTransparency);
			return style;
		}
		static string GetOpacityStyle(int transparency) {
			float opacity = (255f - transparency) / 255f;
			return String.Format("opacity:{0:F3};filter:alpha(opacity={1:F1});", opacity, opacity * 100f);
		}
	}
}
