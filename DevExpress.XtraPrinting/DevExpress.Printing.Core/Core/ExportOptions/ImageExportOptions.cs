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
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Imaging;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using System.Drawing;
using System.Drawing.Imaging;
#else
using System.Windows.Media;
using DevExpress.Xpf.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions")]
	public class ImageExportOptions : PageByPageExportOptionsBase, IXtraSupportShouldSerialize {
#if !SILVERLIGHT
		static readonly Color DefaultPageBorderColor = Color.Black;
#else
		static readonly Color DefaultPageBorderColor = Colors.Black;
#endif
		#region static
		static readonly ImageFormat DefaultImageFormat = ImageFormat.Png;
		const int DefaultResolution = 96;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly Dictionary<string, ImageFormat> ImageFormats = new Dictionary<string, ImageFormat>();
		static ImageExportOptions() {
			ImageFormats[".bmp"] = ImageFormat.Bmp;
			ImageFormats[".gif"] = ImageFormat.Gif;
			ImageFormats[".jpg"] = ImageFormat.Jpeg;
			ImageFormats[".png"] = ImageFormat.Png;
			ImageFormats[".emf"] = ImageFormat.Emf;
			ImageFormats[".wmf"] = ImageFormat.Wmf;
			ImageFormats[".tiff"] = ImageFormat.Tiff;
		}
		internal static bool GetImageFormatSupported(ImageFormat format) {
			return ImageFormats.ContainsValue(format);
		}
		#endregion
		const ImageExportMode DefaultExportMode = ImageExportMode.SingleFile;
		const int DefaultPageBorderWidth = 1;
		ImageExportMode exportMode = DefaultExportMode;
		ImageFormat format = DefaultImageFormat;
		int resolution = DefaultResolution;
		Color pageBorderColor = DefaultPageBorderColor;
		int pageBorderWidth = DefaultPageBorderWidth;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsPageBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.PageBorderColor"),
		TypeConverter(typeof(ImagePageBorderColorConverter)),
		XtraSerializableProperty,
		]
		public Color PageBorderColor { get { return pageBorderColor; } set { pageBorderColor = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsPageBorderWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.PageBorderWidth"),
		DefaultValue(DefaultPageBorderWidth),
		TypeConverter(typeof(ImagePageBorderWidthConverter)),
		XtraSerializableProperty,
		]
		public int PageBorderWidth { get { return pageBorderWidth; } set { pageBorderWidth = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.ExportMode"),
		DefaultValue(DefaultExportMode),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public ImageExportMode ExportMode { get { return exportMode; } set { exportMode = value; } }
		protected internal override bool IsMultiplePaged {
			get { return ExportMode == ImageExportMode.DifferentFiles || ExportMode == ImageExportMode.SingleFilePageByPage; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsPageRange"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.PageRange"),
		TypeConverter(typeof(ImagePageRangeConverter)),
		XtraSerializableProperty,
		]
		public override string PageRange { get { return base.PageRange; } set { base.PageRange = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsFormat"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.Format"),
		TypeConverter(typeof(PSImageFormatConverter)),
		XtraSerializableProperty,
		]
		public ImageFormat Format {
			get { return format; }
			set { format = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageExportOptionsResolution"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.ImageExportOptions.Resolution"),
		DefaultValue(DefaultResolution),
		XtraSerializableProperty,
		]
		public int Resolution { get { return resolution; } set { resolution = value; } }
		public ImageExportOptions() {
		}
		public ImageExportOptions(ImageFormat format) {
			this.format = format;
		}
		ImageExportOptions(ImageExportOptions source)
			: base(source) {
		}
		protected internal override ExportOptionsBase CloneOptions() {
			return new ImageExportOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			ImageExportOptions imageOptionsSource = (ImageExportOptions)source;
			exportMode = imageOptionsSource.ExportMode;
			format = imageOptionsSource.Format;
			resolution = imageOptionsSource.Resolution;
			pageBorderColor = imageOptionsSource.PageBorderColor;
			pageBorderWidth = imageOptionsSource.PageBorderWidth;
		}
		bool ShouldSerializeFormat() {
			return format != DefaultImageFormat;
		}
		bool ShouldSerializePageBorderColor() {
			return PageBorderColor != DefaultPageBorderColor;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeFormat() || ShouldSerializePageBorderColor() || pageBorderWidth != DefaultPageBorderWidth || 
				exportMode != DefaultExportMode || resolution != DefaultResolution || base.ShouldSerialize();
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			switch(propertyName) {
				case "Format":
					return ShouldSerializeFormat();
				case "PageBorderColor":
					return ShouldSerializePageBorderColor();
			}
			return true;
		}
	}
}
