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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.Design.DataAccess.UI {
	public sealed class DataSourceGlyph : UserControl {
		DataSourceGlyphPanel panel;
		public DataSourceGlyph(IDataSourceGlyphProvider provider) {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			panel = new DataSourceGlyphPanel(provider);
			panel.Parent = this;
		}
		public void UpdatePosition() {
			Bounds = panel.GetPosition();
			BringToFront();
		}
		#region static
		static IDictionary<Control, IDataSourceGlyphProvider> providers =
			new Dictionary<Control, IDataSourceGlyphProvider>();
		public static bool IsAttached(Control control) {
			return (control != null) && providers.ContainsKey(control);
		}
		public static void Attach(Control control) {
			Attach(control, ContentAlignment.MiddleCenter, true, true);
		}
		public static void Attach(Control control, bool showText) {
			Attach(control, ContentAlignment.MiddleCenter, showText, true);
		}
		public static void Attach(Control control, ContentAlignment alignment) {
			Attach(control, alignment, true, true);
		}
		public static void Attach(Control control, ContentAlignment alignment, bool showText) {
			Attach(control, alignment, showText, true);
		}
		public static void Attach(Control control, ContentAlignment alignment, bool showText, bool adaptableSize) {
			AssertionException.IsNotNull(control);
			IDataSourceGlyphProvider provider;
			if(!providers.TryGetValue(control, out provider)) {
				provider = new DataSourceGlyphProvider()
				{
					Alignment = alignment,
					ShowText = showText,
					AdaptableSize = adaptableSize
				};
				providers.Add(control, provider);
			}
			else provider.DataSourceChanged();
			provider.DataAwareControl = control;
		}
		public static void Detach(Control control) {
			AssertionException.IsNotNull(control);
			IDataSourceGlyphProvider provider;
			if(providers.TryGetValue(control, out provider)) {
				providers.Remove(control);
				provider.Dispose();
			}
		}
		public static void DataSourceChanged(Control control) {
			AssertionException.IsNotNull(control);
			IDataSourceGlyphProvider provider;
			if(providers.TryGetValue(control, out provider))
				provider.DataSourceChanged();
		}
		#endregion static
	}
	public sealed class DataSourceGlyphPanel : Control {
		IDataSourceGlyphProvider provider;
		public DataSourceGlyphPanel(IDataSourceGlyphProvider provider) {
			this.provider = provider;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.StandardClick, true);
			SetStyle(ControlStyles.Selectable, false);
			this.Dock = DockStyle.Fill;
			this.Cursor = Cursors.Hand;
			painter = new GlyphPainter();
			info = new GlyphInfo(painter);
			gInfo = new GraphicsInfo();
		}
		GlyphInfo info;
		GlyphPainter painter;
		GraphicsInfo gInfo;
		public ContentAlignment Alignment {
			get { return provider.Alignment; }
		}
		ContentAlignment ActualAlignment {
			get {
				if(provider.ConnectedToDataSource)
					return ContentAlignment.BottomLeft;
				return Alignment;
			}
		}
		public Rectangle GetPosition() {
			return GetPosition(provider.DataAwareControl.ClientRectangle);
		}
		Rectangle GetPosition(Rectangle ownerBounds) {
			Graphics g = gInfo.AddGraphics(null);
			try {
				info.ShowText = provider.ShowText;
				info.AdaptableSize = provider.AdaptableSize;
				info.Glyph = (provider != null) && provider.ConnectedToDataSource ? provider.Glyph : null;
				Rectangle clientRect = Rectangle.Inflate(ownerBounds, -2, -2);
				Size minSize = info.CalcMinSize(g, clientRect);
				return PlacementHelper.Arrange(minSize, clientRect, ActualAlignment);
			}
			finally { gInfo.ReleaseGraphics(); }
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				info.Cache = cache;
				info.ShowText = provider.ShowText;
				info.AdaptableSize = provider.AdaptableSize;
				info.IsEnabled = !Utils.Design.DebugInfoDesigner.IsDebugging(provider.DataAwareControl);
				info.IsHot = IsHot;
				info.Calc(e.Graphics, ClientRectangle);
				painter.DrawObject(info);
				info.Cache = null;
			}
		}
		bool isHotCore;
		public bool IsHot {
			get { return isHotCore; }
		}
		void SetIsHot(bool value) {
			if(isHotCore == value) return;
			isHotCore = value;
			Invalidate();
		}
		protected override void OnMouseEnter(System.EventArgs e) {
			SetIsHot(true);
		}
		protected override void OnMouseLeave(System.EventArgs e) {
			SetIsHot(false);
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			provider.RunDataSourceWizard();
		}
		class GlyphPainter : ObjectPainter {
			#region static
			static Font SmallFont = new Font("Segoe UI", DevExpress.Utils.AppearanceObject.DefaultFont.Size);
			static Font DefaultFont = new Font("Segoe UI", DevExpress.Utils.AppearanceObject.DefaultFont.Size + 4);
			static Font LargeFont = new Font("Segoe UI", DevExpress.Utils.AppearanceObject.DefaultFont.Size + 8);
			static System.Drawing.Imaging.ImageAttributes invertAttributes;
			static GlyphPainter() {
				float[][] invertMatrix = new float[][]{ 
					new float[] {-1, 0, 0, 0, 0},
					new float[] {0, -1, 0, 0, 0},
					new float[] {0, 0, -1, 0, 0},
					new float[] {0, 0, 0, 1, 0}, 
					new float[] {1.5f, 1.5f, 1.5f, 0, 1}};
				invertAttributes = new System.Drawing.Imaging.ImageAttributes();
				invertAttributes.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(invertMatrix));
			}
			#endregion static
			public GlyphPainter() {
				font = DefaultFont;
			}
			public sealed override void DrawObject(ObjectInfoArgs e) {
				GlyphInfo args = e as GlyphInfo;
				if(args.Bounds.Size.IsEmpty) return;
				Rectangle imgBounds = new Rectangle(0, 0, args.Bounds.Width, args.Bounds.Height);
				using(Bitmap img = new Bitmap(imgBounds.Width, imgBounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
					using(Graphics g = Graphics.FromImage(img)) {
						using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, imgBounds)) {
							bg.Graphics.TranslateTransform(-args.Bounds.X, -args.Bounds.Y);
							using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
								DrawBackground(bufferedCache, args);
								DrawForeground(bufferedCache, args);
							}
							bg.Render();
						}
						e.Cache.Graphics.DrawImageUnscaled(img, args.Bounds);
					}
				}
			}
			protected virtual void DrawBackground(GraphicsCache cache, GlyphInfo info) {
				info.PaintAppearancePanel.DrawBackground(cache, info.Bounds);
				cache.Graphics.DrawRectangle(info.PaintAppearancePanel.GetBorderPen(cache),
					info.Bounds.X, info.Bounds.Y, info.Bounds.Width - 1, info.Bounds.Height - 1);
			}
			protected virtual void DrawForeground(GraphicsCache cache, GlyphInfo info) {
				if(!info.TextBounds.Size.IsEmpty)
					info.PaintAppearance.DrawString(cache, info.Text, info.TextBounds);
				if(!info.ImageBounds.Size.IsEmpty) {
					if(info.IsEnabled) {
						if(info.IsHot && !UseGlyph)
							cache.Graphics.DrawImage(info.Image, info.ImageBounds, 0, 0, info.ImageBounds.Width, info.ImageBounds.Height, GraphicsUnit.Pixel, invertAttributes);
						else
							cache.Graphics.DrawImage(info.Image, info.ImageBounds, 0, 0, info.ImageBounds.Width, info.ImageBounds.Height, GraphicsUnit.Pixel);
					}
					else cache.Graphics.DrawImage(info.Image, info.ImageBounds, 0, 0, info.ImageBounds.Width, info.ImageBounds.Height, GraphicsUnit.Pixel, DevExpress.Utils.Paint.XPaint.DisabledImageAttr);
				}
			}
			AppearanceDefault appearanceDefaultCore;
			public override AppearanceDefault DefaultAppearance {
				get {
					if(appearanceDefaultCore == null)
						appearanceDefaultCore = CreateDefaultAppearance();
					return appearanceDefaultCore;
				}
			}
			AppearanceDefault appearanceDefaultHotCore;
			public AppearanceDefault DefaultAppearanceHot {
				get {
					if(appearanceDefaultHotCore == null)
						appearanceDefaultHotCore = CreateDefaultAppearanceHot();
					return appearanceDefaultHotCore;
				}
			}
			AppearanceDefault appearanceDefaultPanelCore;
			public AppearanceDefault DefaultAppearancePanel {
				get {
					if(appearanceDefaultPanelCore == null)
						appearanceDefaultPanelCore = CreateDefaultAppearancePanel();
					return appearanceDefaultPanelCore;
				}
			}
			AppearanceDefault appearanceDefaultPanelHotCore;
			public AppearanceDefault DefaultAppearancePanelHot {
				get {
					if(appearanceDefaultPanelHotCore == null)
						appearanceDefaultPanelHotCore = CreateDefaultAppearancePanelHot();
					return appearanceDefaultPanelHotCore;
				}
			}
			protected virtual AppearanceDefault CreateDefaultAppearance() {
				return new AppearanceDefault(Color.FromArgb(0xF7, 0x94, 0x1E), Color.Empty, font);
			}
			protected virtual AppearanceDefault CreateDefaultAppearanceHot() {
				return new AppearanceDefault(Color.White, Color.Empty, font);
			}
			protected virtual AppearanceDefault CreateDefaultAppearancePanel() {
				return new AppearanceDefault(Color.Empty, Color.White, Color.FromArgb(0xD9, 0xD9, 0xD9));
			}
			protected virtual AppearanceDefault CreateDefaultAppearancePanelHot() {
				return new AppearanceDefault(Color.Empty, Color.FromArgb(0xF7, 0x94, 0x1E), Color.FromArgb(0xF7, 0x94, 0x1E));
			}
			public virtual Padding ContentMargin {
				get { return new Padding(12, 4, 12, 4); }
			}
			public virtual Padding ImageMargin {
				get { return new Padding(4); }
			}
			public virtual int ImageToTextDistance {
				get { return 6; }
			}
			Font font;
			public void AdjustFont(GraphicsCache cache, Rectangle bounds, string text) {
				Font result = DefaultFont;
				if(UseGlyph || !AdaptableSize)
					result = SmallFont;
				else {
					if(bounds.Width < GetDefaultWidth(cache, text) * 1.25)
						result = SmallFont;
					else {
						if(bounds.Width < GetLargeWidth(cache, text) * 2) {
							result = DefaultFont;
						}
						else result = LargeFont;
					}
					if(bounds.Height < GetDefaultHeight(cache, text) * 1.5)
						result = SmallFont;
					else {
						if(bounds.Height < GetLargeHeight(cache, text) * 2) {
							if(result.Size > DefaultFont.Size)
								result = DefaultFont;
						}
					}
				}
				if(font != result) {
					appearanceDefaultCore = null;
					appearanceDefaultHotCore = null;
					font = result;
				}
			}
			int defWidth = -1;
			int GetDefaultWidth(GraphicsCache cache, string text) {
				if(defWidth == -1)
					defWidth = (int)(0.5f + cache.CalcTextSize(text, DefaultFont, new StringFormat(), 0).Width) + 32 +
						ImageToTextDistance + ContentMargin.Horizontal * 2;
				return defWidth;
			}
			int largeWidth = -1;
			int GetLargeWidth(GraphicsCache cache, string text) {
				if(largeWidth == -1)
					largeWidth = (int)(0.5f + cache.CalcTextSize(text, LargeFont, new StringFormat(), 0).Width) + 48 +
						ImageToTextDistance + ContentMargin.Horizontal * 2;
				return largeWidth;
			}
			int defHeight = -1;
			int GetDefaultHeight(GraphicsCache cache, string text) {
				if(defHeight == -1)
					defHeight = System.Math.Max((int)(0.5f + cache.CalcTextSize(text, DefaultFont, new StringFormat(), 0).Height), 32) +
						ContentMargin.Vertical * 2;
				return defHeight;
			}
			int largeHeight = -1;
			int GetLargeHeight(GraphicsCache cache, string text) {
				if(largeHeight == -1)
					largeHeight = System.Math.Max((int)(0.5f + cache.CalcTextSize(text, LargeFont, new StringFormat(), 0).Height), 48) +
						 ContentMargin.Vertical * 2;
				return largeHeight;
			}
			public Image GetImage(Rectangle bounds) {
				if(UseGlyph)
					return null;
				if(font == SmallFont)
					return DataSourceGlyphImage.Small;
				if(font == LargeFont)
					return DataSourceGlyphImage.Large;
				return DataSourceGlyphImage.Default;
			}
			public bool UseGlyph { get; set; }
			public bool AdaptableSize { get; set; }
		}
		class GlyphInfo : ObjectInfoArgs {
			public GlyphInfo(GlyphPainter painter) {
				this.Painter = painter;
				this.PaintAppearance = new FrozenAppearance();
				this.PaintAppearancePanel = new FrozenAppearance();
				this.ShowText = true;
				this.AdaptableSize = true;
			}
			public bool IsEnabled { get; set; }
			public bool IsHot { get; set; }
			public AppearanceObject PaintAppearance { get; private set; }
			public AppearanceObject PaintAppearancePanel { get; private set; }
			public GlyphPainter Painter { get; private set; }
			public string Text { get; private set; }
			public Rectangle TextBounds { get; private set; }
			public Image Image { get; private set; }
			public Rectangle ImageBounds { get; private set; }
			public bool ShowText { get; set; }
			public bool AdaptableSize { get; set; }
			public Size CalcMinSize(Graphics g, Rectangle bounds) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					Text = DataAccess.DataAccessLocalizer.GetString(DataAccessLocalizerStringId.CreateDataSourceActionName);
					Painter.AdaptableSize = AdaptableSize;
					Painter.UseGlyph = (Glyph != null);
					Painter.AdjustFont(cache, bounds, Text);
					UpdateAppearances();
					Size textSize = CalcTextSize(cache, Text, PaintAppearance);
					Image = Painter.GetImage(bounds) ?? Glyph;
					Size contentSize = (Image == null) ? textSize : new Size(
						textSize.Width + Image.Width + Painter.ImageToTextDistance,
						System.Math.Max(textSize.Height, Image.Height));
					Padding margin = Painter.ContentMargin;
					bool hasText = ShowText && (Glyph == null);
					if(!hasText || contentSize.Width + margin.Horizontal * 2 > bounds.Width) {
						margin = Painter.ImageMargin;
						contentSize = Image.Size;
					}
					return new Size(contentSize.Width + margin.Horizontal, contentSize.Height + margin.Vertical);
				}
			}
			public void Calc(Graphics g, Rectangle bounds) {
				UpdateAppearances();
				if(bounds == Bounds) return;
				Bounds = bounds;
				Padding margin = Painter.ContentMargin;
				Rectangle content = new Rectangle(
					bounds.Left + margin.Left, bounds.Top + margin.Top,
					bounds.Width - margin.Horizontal, bounds.Height - margin.Vertical);
				using(GraphicsCache cache = new GraphicsCache(g)) {
					Size textSize = CalcTextSize(cache, Text, PaintAppearance);
					if(Glyph != null || bounds.Width == Image.Width + Painter.ImageMargin.Horizontal) {
						margin = Painter.ImageMargin;
						TextBounds = Rectangle.Empty;
						ImageBounds = new Rectangle(
							bounds.Left + margin.Left, bounds.Top + margin.Top,
							bounds.Width - margin.Horizontal, bounds.Height - margin.Vertical);
					}
					else {
						TextBounds = PlacementHelper.Arrange(textSize, content, ContentAlignment.MiddleRight);
						ImageBounds = PlacementHelper.Arrange(Image.Size, content, ContentAlignment.MiddleLeft);
					}
				}
			}
			void UpdateAppearances() {
				PaintAppearance.Assign(IsEnabled && IsHot ? Painter.DefaultAppearanceHot : Painter.DefaultAppearance);
				PaintAppearancePanel.Assign(IsEnabled && IsHot ? Painter.DefaultAppearancePanelHot : Painter.DefaultAppearancePanel);
			}
			protected Size CalcTextSize(GraphicsCache cache, string text, AppearanceObject appearance) {
				return Size.Ceiling(appearance.CalcTextSize(cache, text, 0));
			}
			public Cursor HitTest(Point p) {
				return Bounds.Contains(p) ? Cursors.Hand : null;
			}
			public Image Glyph { get; internal set; }
		}
		static class DataSourceGlyphImage {
			static DataSourceGlyphImage() {
				string prefix = "DevExpress.Design.Images.DataSourceGlyph.";
				Small = ResourceImageHelper.CreateImageFromResources(prefix + "Small.png", typeof(DataSourceGlyphImage).Assembly);
				Default = ResourceImageHelper.CreateImageFromResources(prefix + "Default.png", typeof(DataSourceGlyphImage).Assembly);
				Large = ResourceImageHelper.CreateImageFromResources(prefix + "Large.png", typeof(DataSourceGlyphImage).Assembly);
			}
			public static Image Small { get; private set; }
			public static Image Default { get; private set; }
			public static Image Large { get; private set; }
		}
		static class PlacementHelper {
			public static Rectangle Arrange(Size size, Rectangle targetRect, ContentAlignment alignment) {
				double left = GetLeft(size.Width, targetRect, alignment);
				double top = GetTop(size.Height, targetRect, alignment);
				return new Rectangle(new Point(Round(left), Round(top)), size);
			}
			static int Round(double d) {
				return d > 0 ? (int)(d + 0.5d) : (int)(d - 0.5d);
			}
			static double CenterRange(double targetStart, double targetRange, double range) {
				return targetStart + (targetRange - range) * 0.5;
			}
			static Size GetSize(Rectangle targetRect, ContentAlignment alignment) {
				double width = targetRect.Width;
				double height = targetRect.Height;
				switch(alignment) {
					case ContentAlignment.TopCenter:
					case ContentAlignment.BottomCenter:
						height *= 0.5d;
						break;
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleRight:
						width *= 0.5d;
						break;
				}
				return new Size(Round(width), Round(height));
			}
			static double GetLeft(double width, Rectangle targetRect, ContentAlignment alignment) {
				double left = targetRect.Left;
				switch(alignment) {
					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						left = CenterRange(targetRect.Left, targetRect.Width, width);
						break;
					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						left = targetRect.Right - width;
						break;
				}
				return left;
			}
			static double GetTop(double height, Rectangle targetRect, ContentAlignment alignment) {
				double top = targetRect.Top;
				switch(alignment) {
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleRight:
						top = CenterRange(targetRect.Top, targetRect.Height, height);
						break;
					case ContentAlignment.BottomLeft:
					case ContentAlignment.BottomCenter:
					case ContentAlignment.BottomRight:
						top = targetRect.Bottom - height;
						break;
				}
				return top;
			}
		}
	}
}
