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

using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
#if SL
using DevExpress.Xpf.Windows.Forms;
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
#else
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
#if !SL
	[
	BrickExporter(typeof(ImageBrickExporter))
	]
#endif
	public class ImageBrick : VisualBrick, IImageBrick, IXtraSupportAfterDeserialize {
		const ImageSizeMode DefaultSizeMode = ImageSizeMode.StretchImage;
		ImageEntry imageEntry = new ImageEntry();
		protected ImageSizeMode fSizeMode = DefaultSizeMode;
		string htmlImageUrl;
		bool useImageResolution;
		bool disposeImage;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageBrickImage"),
#endif
		DefaultValue(null),
		]
		public virtual Image Image { get { return imageEntry.Image; } set { imageEntry.Image = value; } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.Cached),
		]
		public ImageEntry ImageEntry { get { return imageEntry; } set { imageEntry = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageBrickSizeMode"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultSizeMode),
		]
		public ImageSizeMode SizeMode {
			get { return fSizeMode; }
			set { fSizeMode = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageBrickHtmlImageUrl"),
#endif
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public string HtmlImageUrl {
			get { return htmlImageUrl; }
			set {
				if (!string.IsNullOrEmpty(value))
					htmlImageUrl = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ImageBrickUseImageResolution"),
#endif
		XtraSerializableProperty,
		DefaultValue(false),
		]
		public bool UseImageResolution { get { return useImageResolution; } set { useImageResolution = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ImageBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Image; } }
		[
		DefaultValue(ImageAlignment.Default),
		XtraSerializableProperty,
		]
		public virtual ImageAlignment ImageAlignment { get { return ImageAlignmentCore; } set { ImageAlignmentCore = value; } }
		public ImageBrick(BrickStyle style) : base(style) { }
		public ImageBrick()
			: base() {
		}
		public ImageBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor)
			: base(sides, borderWidth, borderColor, backColor) {
		}
		public ImageBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
		}
		internal ImageBrick(ImageBrick imageBrick)
			: base(imageBrick) {
			this.fSizeMode = imageBrick.SizeMode;
			this.htmlImageUrl = imageBrick.HtmlImageUrl;
			this.useImageResolution = imageBrick.UseImageResolution;
			this.disposeImage = imageBrick.DisposeImage;
			this.imageEntry = imageBrick.ImageEntry;
			this.ImageAlignmentCore = imageBrick.ImageAlignment;
		}
		public override void Dispose() {
			if(DisposeImage && imageEntry.Image != null) {
				imageEntry.Image.Dispose();
				imageEntry.Image = null;
			}
			base.Dispose();
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ImageBrickDisposeImage")]
#endif
public bool DisposeImage {
			get { return disposeImage; }
			set { disposeImage = value; }
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.ImageEntry)
				DocumentSerializationOptions.AddImageEntryToCache(e);
		}
		internal bool ShouldSerializeImageEntryInternal() {
			return Image != null;
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.ImageEntry)
				return ShouldSerializeImageEntryInternal();
			return base.ShouldSerializeCore(propertyName);
		}
		#region ICloneable Members
		public override object Clone() {
			return new ImageBrick(this);
		}
		#endregion
		protected override float ValidatePageBottomInternal(float pageBottom, RectangleF brickRect, IPrintingSystemContext context) {
			return pageBottom;
		}
	}
}
