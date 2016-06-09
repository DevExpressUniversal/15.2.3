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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Utils.VisualEffects {
	public class BadgeViewInfo : AdornerElementViewInfo {
		public BadgeViewInfo(Badge badge) : base(badge) { }
		public Rectangle TextBounds { get; protected set; }
		public Rectangle ImageBounds { get; protected set; }
		protected Badge Badge { get { return Element as Badge; } }
		protected virtual Size GetImageSize() {
			Image im = GetImage();
			if(im == null) return Size.Empty;
			return im.Size;
		}
		protected virtual Size GetTextSize(Graphics g) {
			if(string.IsNullOrEmpty(Text)) return Size.Empty;
			SizeF size = g.MeasureString(Text, PaintAppearance.GetFont(), 0, PaintAppearance.GetStringFormat());
			return RoundTextSize(size);
		}
		protected Size RoundTextSize(SizeF size) {
			int width = (int)size.Width;
			int height = (int)size.Height;
			if(size.Width > width)
				width++;
			if(size.Height > height)
				height++;
			return new Size(width, height);
		}
		Size CombineSize(Size defaultSize, IEnumerable<Size> sources) {
			if(sources == null) return defaultSize;
			Size size = defaultSize;
			foreach(Size source in sources) {
				if(source.IsEmpty) continue;
				if(source.Width > size.Width) size.Width = source.Width;
				if(source.Height > size.Height) size.Height = source.Height;
			}
			return size;
		}
		protected override Size CalcMinSizeCore(Graphics g, AdornerElementPainter painter) {
			Size defaultSize = base.CalcMinSizeCore(g, painter);
			Size imageSize = GetImageSize();
			Size textSize = GetTextSize(g);
			Padding textMargin = Badge.Properties.ActualTextMargin;
			textSize.Width += textMargin.Horizontal;
			textSize.Height += textMargin.Vertical;
			return CombineSize(defaultSize, new Size[] { imageSize, textSize });
		}
		protected override void CalcContent(Graphics g, Rectangle bounds) {
			if(bounds.Size.IsEmpty) return;
			Size textSize = GetTextSize(g);
			if(!textSize.IsEmpty) { 
				Padding textMargin = Badge.Properties.ActualTextMargin;
				TextBounds = new Rectangle(bounds.X + textMargin.Left, bounds.Y + textMargin.Top, bounds.Width - textMargin.Horizontal, bounds.Height - textMargin.Vertical);
			}
			Size imageSize = GetImageSize();
			if(!imageSize.IsEmpty)
				ImageBounds = CanStretchImage ? bounds : PlacementHelper.Arrange(imageSize, bounds, ContentAlignment.MiddleCenter);
		}
		protected override Rectangle CalcBounds(Graphics g, Rectangle targetElementBounds) {
			if(targetElementBounds.IsEmpty) return targetElementBounds;
			Size minSize = CalcMinSize(g);
			if(minSize.IsEmpty) return Rectangle.Empty;
			Rectangle maxBounds = Rectangle.Inflate(targetElementBounds, minSize.Width / 2, minSize.Height / 2);
			if(maxBounds.IsEmpty) return Rectangle.Empty;
			Rectangle bounds = PlacementHelper.Arrange(minSize, maxBounds, Badge.Properties.ActualLocation);
			Point offset = Badge.Properties.ActualOffset;
			Point location = new Point(bounds.X + offset.X, bounds.Y + offset.Y);
			return new Rectangle(location, bounds.Size);
		}
		public Image Image { get { return GetImage(); } }
		public string Text { get { return Badge.Properties.ActualText; } }
		public bool AllowGlyphSkinning {
			get {
				if(Badge.Properties.ActualImage == null) return true;
				return Badge.Properties.IsGlyphSkinningEnabled;
			}
		}
		public Padding ImageStretchMargins {
			get {
				if(Badge.Properties.ActualImage == null) return DefaultImageStretchMargins;
				return Badge.Properties.ActualImageStretchMargins;
			}
		}
		public bool CanStretchImage { get { return Badge.Properties.CanStretchImage; } }
		protected Image GetImage() {
			if(!Badge.Properties.CanUseImage) return null;
			return Badge.Properties.ActualImage ?? DefaultImage;
		}
		static Image defaultImage;
		protected static Image DefaultImage {
			get {
				if(defaultImage == null)
					defaultImage = ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images.Badge.png", typeof(BadgePainter).Assembly);
				return defaultImage;
			}
		}
		protected static System.Windows.Forms.Padding DefaultImageStretchMargins { get { return new System.Windows.Forms.Padding(9); } }
	}
}
