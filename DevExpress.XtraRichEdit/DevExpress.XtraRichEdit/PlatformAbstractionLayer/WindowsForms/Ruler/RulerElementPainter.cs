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

using DevExpress.Utils;
using DevExpress.LookAndFeel;
using System.Drawing;
using DevExpress.Skins;
using System;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Utils;
using System.Windows.Forms;
using System.Reflection;
namespace DevExpress.XtraRichEdit.Ruler {
	#region GenerateImageDelegate
	public delegate void GenerateImageDelegate(Graphics gr, Rectangle bounds, Brush brush);
	#endregion
	#region RulerElementPainter (abstract class)
	public abstract class RulerElementPainter : IDisposable {
		#region Fields
		readonly HorizontalRulerControl ruler;
		readonly Dictionary<Type, Bitmap> enabledElementImages;
		readonly Dictionary<Type, Bitmap> disabledElementImages;
		#endregion
		protected RulerElementPainter(HorizontalRulerControl ruler) {
			Guard.ArgumentNotNull(ruler, "ruler");
			this.ruler = ruler;
			this.enabledElementImages = new Dictionary<Type, Bitmap>();
			this.disabledElementImages = new Dictionary<Type, Bitmap>();
		}
		public virtual void Initialize() {
			PopulateEnabledElementImages();
			PopulateDisabledElementImages();
		}
		#region Properties
		protected internal HorizontalRulerControl Ruler { get { return ruler; } }
		protected internal Dictionary<Type, Bitmap> EnabledElementImages { get { return enabledElementImages; } }
		protected internal Dictionary<Type, Bitmap> DisabledElementImages { get { return disabledElementImages; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal Bitmap GenerateImage(Rectangle bounds, Color color, GenerateImageDelegate generateImage) {
			Size size = bounds.Size;
			size.Width = Math.Max(1, size.Width);
			size.Height = Math.Max(1, size.Height);
			bounds = new Rectangle(Point.Empty, size);
			Bitmap bmp = new Bitmap(size.Width, size.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (SolidBrush brush = new SolidBrush(color)) {
					generateImage(gr, bounds, brush);
				}
			}
			return bmp;
		}
		protected internal virtual void DrawHotZone(RulerHotZone hotZone, GraphicsCache cache, Point location) {
			if (!hotZone.Visible)
				return;
			Dictionary<Type, Bitmap> images = hotZone.Enabled ? EnabledElementImages : DisabledElementImages;
			Bitmap bitmap;
			if (images.TryGetValue(hotZone.GetType(), out bitmap)) {
				HorizontalRulerPainter.DrawBitmap(cache.Graphics, bitmap, location.X, location.Y);
			}
		}
		protected internal virtual Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone) {
			Bitmap bitmap;
			if (EnabledElementImages.TryGetValue(hotZone.GetType(), out bitmap))
				return Ruler.PixelsToLayoutUnits(bitmap.Size);
			else
				return Ruler.PixelsToLayoutUnits(new Size(16, 16));
		}
		protected internal abstract void PopulateEnabledElementImages();
		protected internal abstract void PopulateDisabledElementImages();
	}
	#endregion
	#region ColorBasedRulerElementPainter
	public class ColorBasedRulerElementPainter : RulerElementPainter {
		readonly Color enabledElementColor;
		readonly Color disabledElementColor;
		public ColorBasedRulerElementPainter(HorizontalRulerControl ruler, Color elementColor)
			: base(ruler) {
			this.enabledElementColor = elementColor;
			this.disabledElementColor = ControlPaint.LightLight(elementColor);
		}
		protected Color EnabledElementColor { get { return enabledElementColor; } }
		protected Color DisabledElementColor { get { return disabledElementColor; } }
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (EnabledElementImages != null) {
						foreach (Type key in EnabledElementImages.Keys)
							EnabledElementImages[key].Dispose();
						EnabledElementImages.Clear();
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal override void PopulateEnabledElementImages() {
			EnabledElementImages.Add(typeof(LeftTabHotZone), GenerateTabImage(DrawLeftTabImage, EnabledElementColor));
			EnabledElementImages.Add(typeof(RightTabHotZone), GenerateTabImage(DrawRightTabImage, EnabledElementColor));
			EnabledElementImages.Add(typeof(CenterTabHotZone), GenerateTabImage(DrawCenterTabImage, EnabledElementColor));
			EnabledElementImages.Add(typeof(DecimalTabHotZone), GenerateTabImage(DrawDecimalTabImage, EnabledElementColor));
			EnabledElementImages.Add(typeof(LeftIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.LeftIndent.png", Assembly.GetExecutingAssembly()));
			EnabledElementImages.Add(typeof(LeftBottomHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.LeftIndentBottom.png", Assembly.GetExecutingAssembly()));
			EnabledElementImages.Add(typeof(RightIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.RightIndent.png", Assembly.GetExecutingAssembly()));
			EnabledElementImages.Add(typeof(FirstLineIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.FirstLineIndent.png", Assembly.GetExecutingAssembly()));
			EnabledElementImages.Add(typeof(TableLeftBorderHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.TableColumnBorder.png", Assembly.GetExecutingAssembly()));
			EnabledElementImages.Add(typeof(TableHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.TableColumnBorder.png", Assembly.GetExecutingAssembly()));
		}
		protected internal override void PopulateDisabledElementImages() {
			DisabledElementImages.Add(typeof(LeftTabHotZone), GenerateTabImage(DrawLeftTabImage, DisabledElementColor));
			DisabledElementImages.Add(typeof(RightTabHotZone), GenerateTabImage(DrawRightTabImage, DisabledElementColor));
			DisabledElementImages.Add(typeof(CenterTabHotZone), GenerateTabImage(DrawCenterTabImage, DisabledElementColor));
			DisabledElementImages.Add(typeof(DecimalTabHotZone), GenerateTabImage(DrawDecimalTabImage, DisabledElementColor));
			DisabledElementImages.Add(typeof(LeftIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.LeftIndentDisabled.png", Assembly.GetExecutingAssembly()));
			DisabledElementImages.Add(typeof(LeftBottomHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.LeftIndentBottomDisabled.png", Assembly.GetExecutingAssembly()));
			DisabledElementImages.Add(typeof(RightIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.RightIndentDisabled.png", Assembly.GetExecutingAssembly()));
			DisabledElementImages.Add(typeof(FirstLineIndentHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.FirstLineIndentDisabled.png", Assembly.GetExecutingAssembly()));
			DisabledElementImages.Add(typeof(TableLeftBorderHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.TableColumnBorderDisabled.png", Assembly.GetExecutingAssembly()));
			DisabledElementImages.Add(typeof(TableHotZone), ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.Ruler.TableColumnBorderDisabled.png", Assembly.GetExecutingAssembly()));
		}
		#region Elements images generation
		protected internal virtual Bitmap GenerateTabImage(GenerateImageDelegate generateImage, Color color) {
			int height = Math.Max(6, Ruler.LayoutUnitsToPixelsH(2 * Ruler.ViewInfo.ClientBounds.Height / 5));
			return GenerateImage(new Rectangle(0, 0, 2 * height, height), color, generateImage);
		}
		void DrawTabImageBottom(Graphics gr, Rectangle bounds, Brush brush) {
			Rectangle rect = bounds;
			rect.Y = rect.Bottom - 2;
			rect.Height = 2;
			gr.FillRectangle(brush, rect);
		}
		void DrawLeftTabImage(Graphics gr, Rectangle bounds, Brush brush) {
			gr.Clear(DXColor.Transparent);
			Rectangle rect = bounds;
			rect.Width = 2;
			gr.FillRectangle(brush, rect);
			bounds.Width -= bounds.Height;
			DrawTabImageBottom(gr, bounds, brush);
		}
		void DrawRightTabImage(Graphics gr, Rectangle bounds, Brush brush) {
			gr.Clear(DXColor.Transparent);
			Rectangle rect = bounds;
			rect.X = rect.Right - 2;
			rect.Width = 2;
			gr.FillRectangle(brush, rect);
			bounds.X += bounds.Height;
			bounds.Width -= bounds.Height;
			DrawTabImageBottom(gr, bounds, brush);
		}
		void DrawCenterTabImage(Graphics gr, Rectangle bounds, Brush brush) {
			gr.Clear(DXColor.Transparent);
			Rectangle rect = bounds;
			rect.X += rect.Width / 2 - 1;
			rect.Width = 2;
			gr.FillRectangle(brush, rect);
			int width = 2 * bounds.Height - 2;
			bounds.X += (bounds.Width - width) / 2;
			bounds.Width = width;
			DrawTabImageBottom(gr, bounds, brush);
		}
		void DrawDecimalTabImage(Graphics gr, Rectangle bounds, Brush brush) {
			DrawCenterTabImage(gr, bounds, brush);
			Rectangle rect = bounds;
			rect.X += rect.Width / 2 - 1;
			rect.X += 3;
			rect.Y = rect.Bottom - 2;
			rect.Y -= 3;
			rect.Height = 2;
			rect.Width = 2;
			gr.FillRectangle(brush, rect);
		}
		#endregion
	}
	#endregion
	#region RulerElementSkinPainter
	public class RulerElementSkinPainter : RulerElementPainter {
		readonly UserLookAndFeel lookAndFeel;
		public RulerElementSkinPainter(HorizontalRulerControl ruler, UserLookAndFeel lookAndFeel)
			: base(ruler) {
			Guard.ArgumentNotNull(lookAndFeel, "lookAndFeel");
			this.lookAndFeel = lookAndFeel;
		}
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected internal override void PopulateEnabledElementImages() {
			EnabledElementImages.Add(typeof(LeftTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 1));
			EnabledElementImages.Add(typeof(RightTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 0));
			EnabledElementImages.Add(typeof(CenterTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 3));
			EnabledElementImages.Add(typeof(DecimalTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 4));
			EnabledElementImages.Add(typeof(LeftIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 1));
			EnabledElementImages.Add(typeof(RightIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 1));
			EnabledElementImages.Add(typeof(FirstLineIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 0));
			EnabledElementImages.Add(typeof(LeftBottomHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndentBottom, 0));
			EnabledElementImages.Add(typeof(RightColumnResizerHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 1));
			EnabledElementImages.Add(typeof(LeftColumnResizerHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 1));
			EnabledElementImages.Add(typeof(MiddleColumnResizerHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 0));
			EnabledElementImages.Add(typeof(TableHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 0));
			EnabledElementImages.Add(typeof(TableLeftBorderHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 0));
		}
		protected internal override void PopulateDisabledElementImages() {
			DisabledElementImages.Add(typeof(LeftTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 6));
			DisabledElementImages.Add(typeof(RightTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 5));
			DisabledElementImages.Add(typeof(CenterTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 8));
			DisabledElementImages.Add(typeof(DecimalTabHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerTab, 9));
			DisabledElementImages.Add(typeof(LeftIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 3));
			DisabledElementImages.Add(typeof(RightIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 3));
			DisabledElementImages.Add(typeof(FirstLineIndentHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndent, 2));
			DisabledElementImages.Add(typeof(LeftBottomHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerIndentBottom, 1));
			DisabledElementImages.Add(typeof(LeftColumnResizerHotZone), GetRichEditSkinElementImage(RichEditSkins.SkinRulerColumnResizer, 0));
		}
		protected internal virtual Bitmap GetRichEditSkinElementImage(string elementName, int imageIndex) {
			SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[elementName];
			ImageCollection images = element.Image.GetImages();
			return (Bitmap)images.Images[imageIndex];
		}
		protected internal virtual Bitmap GetCurrentReportsSkinElement(string elementName, int imageIndex) {
			SkinElementInfo elementInfo = GetReportsSkinElement(elementName, Rectangle.Empty);
			ImageCollection images = elementInfo.Element.Image.GetImages();
			return (Bitmap)images.Images[imageIndex];
		}
		protected internal virtual SkinElementInfo GetRichEditSkinElement(string elementName, Rectangle bounds) {
			SkinElement element = RichEditSkins.GetSkin(LookAndFeel)[elementName];
			return new SkinElementInfo(element, bounds);
		}
		protected internal virtual SkinElementInfo GetReportsSkinElement(string elementName, Rectangle bounds) {
			SkinElement element = ReportsSkins.GetSkin(LookAndFeel)[elementName];
			return new SkinElementInfo(element, bounds);
		}
	}
	#endregion
	partial class HorizontalRulerPainter {
		internal static void DrawBitmap(Graphics gr, Bitmap bitmap, int x, int y) {
			Rectangle bounds = new Rectangle(x, y, bitmap.Width, bitmap.Height);
			gr.DrawImage(bitmap, bounds);
		}
	}
}
