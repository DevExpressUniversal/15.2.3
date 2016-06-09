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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraBars.Docking.Paint {
	public enum CaptionButtonStatus { ActiveWindowButton, InactiveWindowButton, ActiveApplicationButton, InactiveApplicationButton }
	public enum EdgePositions { None = 0, Left = 1, Top = 2, Right = 4, Bottom = 8, TopLeft = 16, BottomRight = 32, Rect = 64 }
	public enum EdgesType { Standard, Flat, RaisedInner, RaisedOuter, SunkenInner, SunkenOuter }
	class EdgesPaintHelper {
		public static void DrawColorEdge(GraphicsCache cache, Rectangle rect, Color color, EdgesType edgesType, EdgePositions edgePositions) {
			Color LTCol = color, RBCol = color;
			switch(edgesType) {
				case EdgesType.Flat:
					LTCol = ControlPaint.Dark(color);
					RBCol = ControlPaint.Dark(color);
					break;
				case EdgesType.RaisedOuter:
					LTCol = ControlPaint.LightLight(color);
					RBCol = ControlPaint.DarkDark(color);
					break;
				case EdgesType.RaisedInner:
					LTCol = ControlPaint.Light(color);
					RBCol = ControlPaint.Dark(color);
					break;
				case EdgesType.SunkenOuter:
					LTCol = ControlPaint.DarkDark(color);
					RBCol = ControlPaint.LightLight(color);
					break;
				case EdgesType.SunkenInner:
					LTCol = ControlPaint.Dark(color);
					RBCol = ControlPaint.Light(color);
					break;
			}
			DrawColorEdgeCore(cache.Graphics, rect, cache.GetPen(LTCol), cache.GetPen(RBCol), edgePositions);
		}
		public static void DrawColorEdgeCore(Graphics g, Rectangle rect, Pen penLight, Pen penDark, EdgePositions edgePositions) {
			if((edgePositions & EdgePositions.Left) != 0 || (edgePositions & EdgePositions.TopLeft) != 0 || (edgePositions & EdgePositions.Rect) != 0)
				g.DrawLine(penLight, rect.Left, rect.Bottom - 1, rect.Left, rect.Top);
			if((edgePositions & EdgePositions.Top) != 0 || (edgePositions & EdgePositions.TopLeft) != 0 || (edgePositions & EdgePositions.Rect) != 0)
				g.DrawLine(penLight, rect.Left, rect.Top, rect.Right - 1, rect.Top);
			if((edgePositions & EdgePositions.Right) != 0 || (edgePositions & EdgePositions.BottomRight) != 0 || (edgePositions & EdgePositions.Rect) != 0)
				g.DrawLine(penDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
			if((edgePositions & EdgePositions.Bottom) != 0 || (edgePositions & EdgePositions.BottomRight) != 0 || (edgePositions & EdgePositions.Rect) != 0)
				g.DrawLine(penDark, rect.Right - 1, rect.Bottom - 1, rect.Left, rect.Bottom - 1);
		}
	}
	public abstract class DockElementsPainter {
		BarManagerPaintStyle paintStyle;
		GraphicsInfo gInfo;
		DevExpress.Utils.Paint.Clipping clip;
		protected TabPanelPainter fTabPanelPainter;
		protected ButtonPainter fButtonPainter;
		protected WindowPainter fWindowPainter;
		protected HideBarPainter fHideBarPainter;
		ObjectPainter fButtonsPanelPainter;
		public DockElementsPainter(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
			this.gInfo = new GraphicsInfo();
			this.clip = new DevExpress.Utils.Paint.Clipping();
			this.fButtonsPanelPainter = CreateButtonsPanelPainter();
			CreateElementPainters();
		}
		protected virtual ObjectPainter CreateButtonsPanelPainter() {
			return new Docking2010.ButtonsPanelPainter();
		}
		protected abstract void CreateElementPainters();
		public virtual void DrawWindowCaption(DrawWindowCaptionArgs e) {
			WindowPainter.DrawWindowCaption(e);
		}
		protected virtual CaptionButtonStatus GetWindowButtonStatus(bool active) {
			return (active ? CaptionButtonStatus.ActiveWindowButton : CaptionButtonStatus.InactiveWindowButton);
		}
		public virtual void DrawApplicationCaption(DrawApplicationCaptionArgs args) {
			WindowPainter.DrawApplicationCaption(args);
		}
		protected virtual CaptionButtonStatus GetApplicationButtonStatus(bool active) {
			return (active ? CaptionButtonStatus.ActiveApplicationButton : CaptionButtonStatus.InactiveApplicationButton);
		}
		public virtual void DrawTabPanel(DrawTabPanelArgs args) {
			if(!DockLayoutUtils.CanDraw(null, args.Bounds)) return;
			TabPanelPainter.DrawTabPanel(args.DrawTabPanelButtonArgs);
			GraphicsClipState clipState = args.Cache.ClipInfo.SaveAndSetClip(args.GetClipBounds(TabPanelPainter));
			try {
				for(int i = 0; i < args.TabsCount; i++) {
					if(i == args.ActiveTabIndex) continue;
					DrawTabArgs tabArgs = args.GetTabArgs(i);
					TabPanelPainter.DrawTab(tabArgs);
				}
			}
			finally {
				args.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
			TabPanelPainter.UnderlineTabPanel(args.DrawTabPanelButtonArgs);
			clipState = args.Cache.ClipInfo.SaveAndSetClip(args.GetClipBounds(TabPanelPainter));
			try {
				DrawActiveTab(args.GetTabArgs(args.ActiveTabIndex));
			}
			finally {
				args.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
			if(args.DrawTabButtons) {
				TabPanelPainter.DrawPrevTabButton(args.PrevTabButtonArgs, args.IsVertical);
				TabPanelPainter.DrawNextTabButton(args.NextTabButtonArgs, args.IsVertical);
			}
		}
		protected virtual void DrawActiveTab(DrawTabArgs tabArgs) {
			TabPanelPainter.DrawTab(tabArgs);
		}
		public static Size CalcTextSize(Graphics g, AppearanceObject appearance, string text, int width, bool isVertical) {
			return isVertical ?
				CalcVTextSize(g, appearance, text, width) :
				CalcTextSize(g, appearance, text, width);
		}
		public static Size CalcVTextSize(Graphics g, AppearanceObject appearance, string text, int width) {
			var format = appearance.GetTextOptions().GetStringFormat();
			var savedFlags = format.FormatFlags;
			format.FormatFlags |= StringFormatFlags.DirectionVertical;
			SizeF szf = g.MeasureString(text, appearance.GetFont(), int.MaxValue, format);
			format.FormatFlags = savedFlags;
			return Size.Ceiling(new SizeF(szf.Height, szf.Width));
		}
		public static Size CalcTextSize(Graphics g, AppearanceObject appearance, string text, int width) {
			return Size.Ceiling(appearance.CalcTextSize(g, text, width));
		}
		public static Size CalcTextSize(Graphics g, AppearanceObject appearance, string text) {
			return CalcTextSize(g, appearance, text, 0);
		}
		internal protected int GetMaxHeight(AppearanceObject appearance1, AppearanceObject appearance2) {
			int h1 = GetTextHeight(appearance1);
			int h2 = GetTextHeight(appearance2);
			return Math.Max(h1, h2);
		}
		internal protected AppearanceObject GetMaxHeightAppearance(AppearanceObject appearance1, AppearanceObject appearance2) {
			int h1 = GetTextHeight(appearance1);
			int h2 = GetTextHeight(appearance2);
			return (h1 > h2 ? appearance1 : appearance2);
		}
		public Graphics AddGraphics(Graphics g) { return gInfo.AddGraphics(g); }
		public void ReleaseGraphics() { gInfo.ReleaseGraphics(); }
		public int GetTextHeight(AppearanceObject appearance) {
			Graphics g = AddGraphics(null);
			int result = 0;
			try {
				result = CalcTextSize(g, appearance, "Wg").Height;
			}
			finally {
				ReleaseGraphics();
			}
			return result;
		}
		public virtual int GetCaptionHeight(AppearanceObject appearance, bool floating) {
			return Math.Max(ButtonSize.Height, GetTextHeight(appearance)) + 2 * WindowPainter.CaptionVertIndent;
		}
		protected internal UserLookAndFeel LookAndFeel { get { return PaintStyle.Controller.LookAndFeel; } }
		public virtual Size GetMinCaptionSize(AppearanceObject appearance, bool floating) {
			return new Size(4 * WindowPainter.CaptionButtonInterval + 3 * ButtonPainter.ButtonSize.Width, GetCaptionHeight(appearance, floating));
		}
		public BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public TabPanelPainter TabPanelPainter { get { return fTabPanelPainter; } }
		public HideBarPainter HideBarPainter { get { return fHideBarPainter; } }
		public ButtonPainter ButtonPainter { get { return fButtonPainter; } }
		public WindowPainter WindowPainter { get { return fWindowPainter; } }
		public ObjectPainter ButtonsPanelPainter { get { return fButtonsPanelPainter; } }
		public Size ButtonSize { get { return ButtonPainter.ButtonSize; } }
	}
	public class ElementPainter {
		DockElementsPainter painter;
		protected ElementPainter(DockElementsPainter painter) {
			this.painter = painter;
		}
		protected void FillRectangle(DrawArgs args) {
			FillRectangle(args, args.Bounds);
		}
		protected virtual void FillRectangle(DrawArgs args, Rectangle bounds) {
			args.Graphics.FillRectangle(args.Appearance.GetBackBrush(args.Cache, bounds), bounds);
		}
		static Rectangle rect = LayoutConsts.InvalidRectangle;
		public static void DrawReversibleRectangle(Rectangle bounds) {
			DrawHorzReversibleLine(bounds.Location, bounds.Width);
			DrawHorzReversibleLine(new Point(bounds.Left, bounds.Bottom - DockConsts.DockSelectionFrameWidth), bounds.Width);
			DrawVertReversibleLine(new Point(bounds.Left, bounds.Top + DockConsts.DockSelectionFrameWidth), bounds.Height - 2 * DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(bounds.Right - DockConsts.DockSelectionFrameWidth, bounds.Top + DockConsts.DockSelectionFrameWidth), bounds.Height - 2 * DockConsts.DockSelectionFrameWidth);
		}
		internal static void FillReversibleRectangle(Rectangle rect, Color color) {
			ControlPaint.FillReversibleRectangle(rect, color);
		}
		protected static void DrawHorzReversibleLine(Point start, int width) {
			FillReversibleRectangle(new Rectangle(start.X, start.Y, width, DockConsts.DockSelectionFrameWidth), DockConsts.SelectionColor);
		}
		protected static void DrawVertReversibleLine(Point start, int height) {
			FillReversibleRectangle(new Rectangle(start.X, start.Y, DockConsts.DockSelectionFrameWidth, height), DockConsts.SelectionColor);
		}
		protected UserLookAndFeel LookAndFeel { get { return Painter.LookAndFeel; } }
		protected BaseLookAndFeelPainters LookAndFeelPainters { get { return LookAndFeel.Painter; } }
		protected DockElementsPainter Painter { get { return painter; } }
		protected BarManagerPaintStyle PaintStyle { get { return Painter.PaintStyle; } }
	}
	public abstract class BaseTabPainter : ElementPainter {
		public Size GetImageSize(DockLayout dockLayout) {
			return dockLayout.HasImage ? dockLayout.ImageSize : Size.Empty;
		}
		protected static Size GetImageSize(object images) {
			return ImageCollection.GetImageListSize(images);
		}
		protected static Size GetDefaultImageSize(DockLayout dockLayout) {
			return dockLayout.HasImage ? SystemInformation.SmallIconSize : dockLayout.ImageSize;
		}
		protected BaseTabPainter(DockElementsPainter painter) : base(painter) { }
		public virtual Rectangle UpdateActiveTabBounds(Rectangle bounds, TabsPosition position) {
			return bounds;
		}
		protected bool IsValidImage(DockLayout dockLayout) {
			if(dockLayout == null) return false;
			return dockLayout.HasImage;
		}
		protected virtual void DrawImage(GraphicsCache cache, DockLayout dockLayout, Point location) {
			if(!IsValidImage(dockLayout)) return;
			ImageCollection.DrawImageListImage(cache, dockLayout.Images, dockLayout.ImageIndex, new Rectangle(location, ImageCollection.GetImageListSize(dockLayout.Images)));
		}
		protected virtual void DrawImage(GraphicsCache cache, DockLayout dockLayout, Rectangle bounds) {
			if(!IsValidImage(dockLayout)) return;
			if(dockLayout.HasImageUri || dockLayout.IsImagePropertyChanged) {
				var img = dockLayout.GetActualCaptionImage();
				if(dockLayout.GetAllowGlyphSkinning()) {
					bool isActive = dockLayout.LayoutParent.ActiveChild == dockLayout;
					AppearanceObject appearance = dockLayout.ActiveTabAppearance;
					if(!isActive) {
						DockLayout tabContainer = dockLayout.LayoutParent;
						int tabIndex = tabContainer.IndexOf(dockLayout);
						ObjectState state = ObjectState.Normal;
						if(tabIndex < tabContainer.TabButtons.Count)
							state = ((DockPanelCaptionButton)tabContainer.TabButtons[tabIndex]).ButtonState;
						appearance = (((state & ObjectState.Hot) == ObjectState.Hot) ?
							dockLayout.TabsAppearanceHot : dockLayout.TabsAppearance);
					}
					var attributes = ImageColorizer.GetColoredAttributes(appearance.GetForeColor());
					cache.Graphics.DrawImage(img, bounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
				}
				else cache.Graphics.DrawImage(img, bounds);
			}
			else
				DrawImage(cache, dockLayout, bounds.Location);
		}
		protected virtual void DrawButtonText(DrawTabTextArgs args) {
			if(!DockLayoutUtils.CanDraw(null, args.Bounds)) return;
			if(args.IsVertical)
				args.DrawVerticalString();
			else
				args.DrawString();
		}
	}
	public abstract class TabPanelPainter : BaseTabPainter {
		ObjectPainter scrollButtonPainter;
		public TabPanelPainter(DockElementsPainter painter)
			: base(painter) {
			scrollButtonPainter = LookAndFeelScrollBarButtonPainter.GetPainter(LookAndFeel);
		}
		public int GetTabWidth(DockLayout dockLayout, DockLayout parentLayout) {
			Graphics g = Painter.AddGraphics(null);
			try {
				AppearanceObject appearance = (parentLayout.ActiveChild == dockLayout ? parentLayout.ActiveTabAppearance : parentLayout.TabsAppearance);
				bool isVertical = DockLayoutUtils.IsVerticalPosition(parentLayout.TabsPosition);
				int width = GetTabTextWidth(g, appearance, dockLayout.TabText, isVertical) + 2 * TabHorzTextIndent + 3;
				if(GetImageSize(dockLayout).Width > 0 && IsValidImage(dockLayout))
					width += GetImageSize(dockLayout).Width + TabHorzImageIndent;
				return width;
			}
			finally { Painter.ReleaseGraphics(); }
		}
		protected virtual int GetTabTextWidth(Graphics g, AppearanceObject appearance, string tabText, bool isVertical) {
			return DockElementsPainter.CalcTextSize(g, appearance, tabText, 0, isVertical).Width;
		}
		public virtual int GetTabsHeight(DockLayout dockLayout, AppearanceObject appearance1, AppearanceObject appearance2) {
			return Math.Max(GetMaxImageHeight(dockLayout).Height + TabImage_BorderIndent,
				TabText_EmptySpace + Painter.GetMaxHeight(appearance1, appearance2)) + TabVertBackIndent + TabVertForeIndent;
		}
		Size GetMaxImageHeight(DockLayout dockLayout) {
			if(dockLayout == null) return Size.Empty;
			Size result = Size.Empty;
			foreach(DockLayout layout in dockLayout) {
				if(!layout.ImageSize.IsEmpty && result.Height < layout.ImageSize.Height)
					result = layout.ImageSize;
			}
			return result;
		}
		protected virtual void DrawTabContent(DrawTabArgs args) {
			DirectionRectangle rect = new DirectionRectangle(CalcTabContentBounds(args), !args.IsVertical);
			Rectangle tab = rect.Bounds;
			rect.CutFromHead(TabHorzTextIndent);
			if(IsValidImage(args.TabLayout)) {
				Rectangle img = rect.GetNextRectangle(GetRalativeImageSize(args), 0);
				if(rect.Bounds.Contains(img)) {
					DrawImage(args, args.TabLayout.GetRTLBounds(img, tab));
					rect.CutFromHead((args.IsVertical ? img.Height : img.Width) + TabHorzImageIndent);
				}
			}
			rect.CutFromTail(1);
			DrawButtonText(args.CreateDrawTabTextArgs(args.TabLayout.TabText, args.TabLayout.GetRTLBounds(rect.Bounds, tab), args.IsActive));
		}
		void DrawImage(DrawTabArgs args, Rectangle r) {
			if(args.IsVertical) {
				Size imageSize = GetImageSize(args.TabLayout);
				using(Bitmap buffer = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
					using(Graphics g = Graphics.FromImage(buffer)) {
						g.DrawImage(args.TabLayout.Image, new Rectangle(0, 0, imageSize.Width, imageSize.Height));
					}
					buffer.RotateFlip(RotateFlipType.Rotate90FlipNone);
					args.Cache.Graphics.DrawImageUnscaled(buffer, r);
				}
			}
			else {
				DrawImage(args.Cache, args.TabLayout, r);
			}
		}
		Size GetRalativeImageSize(DrawTabArgs args) {
			Size imageSize = GetImageSize(args.TabLayout);
			return args.IsVertical ? new Size(imageSize.Height, imageSize.Width) : imageSize;
		}
		protected virtual Rectangle CalcTabContentBounds(BaseDrawTabArgs args) { return args.Bounds; }
		public virtual void DrawPrevTabButton(DrawTabButtonArgs args, bool isVertical) {
			DrawTabScrollButton(args, isVertical ? ScrollButton.Up : ScrollButton.Left);
		}
		public virtual void DrawNextTabButton(DrawTabButtonArgs args, bool isVertical) {
			DrawTabScrollButton(args, isVertical ? ScrollButton.Down : ScrollButton.Right);
		}
		protected virtual void DrawTabScrollButton(DrawTabButtonArgs args, ScrollButton scrollButton) {
			ScrollBarButtonInfoArgs btnArgs = new ScrollBarButtonInfoArgs(args.Cache, args.Bounds, args.Appearance, args.Appearance,
				scrollButton, args.TabButton.ButtonState);
			scrollButtonPainter.DrawObject(btnArgs);
		}
		public void DrawTab(DrawTabArgs args) {
			if(args == null) return;
			DrawTabCore(args);
		}
		protected abstract void DrawTabCore(DrawTabArgs args);
		protected internal void DrawTabDockPointer(DockLayout insertLayout, DockLayout parentLayout) {
			int tabWidth = Math.Min(GetTabWidth(insertLayout, parentLayout), parentLayout.ClientBounds.Width - 2 * TabHorzTextIndent);
			DirectionRectangle dBounds = new DirectionRectangle(parentLayout.Panel.RectangleToScreen(parentLayout.ClientBounds), DockLayoutUtils.ConvertToDockingStyle(parentLayout.TabsPosition));
			int tabHeight = GetTabsHeight(parentLayout, parentLayout.TabsAppearance, parentLayout.ActiveTabAppearance) - TabVertBackIndent;
			tabHeight = Math.Max(0, Math.Min(tabHeight, dBounds.Width - tabHeight));
			Rectangle tabs = dBounds.GetSideRectangle(tabHeight);
			dBounds.RemoveSize(tabHeight, false);
			switch(parentLayout.TabsPosition) {
				case TabsPosition.Top: DrawTopTabDockPointer(insertLayout, parentLayout, dBounds.Bounds, tabs, tabWidth); break;
				case TabsPosition.Bottom: DrawBottomTabDockPointer(insertLayout, parentLayout, dBounds.Bounds, tabs, tabWidth); break;
				case TabsPosition.Left: DrawLeftTabDockPointer(insertLayout, parentLayout, dBounds.Bounds, tabs, tabWidth); break;
				case TabsPosition.Right: DrawRightTabDockPointer(insertLayout, parentLayout, dBounds.Bounds, tabs, tabWidth); break;
			}
		}
		void DrawTopTabDockPointer(DockLayout insertLayout, DockLayout parentLayout, Rectangle client, Rectangle tabs, int tabWidth) {
			DrawHorzReversibleLine(new Point(client.Left, client.Bottom - DockConsts.DockSelectionFrameWidth), client.Width - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(client.Left, client.Top), client.Height - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(client.Right - DockConsts.DockSelectionFrameWidth, client.Top), client.Height);
			DrawHorzReversibleLine(new Point(tabs.Left, tabs.Bottom - DockConsts.DockSelectionFrameWidth), TabHorzBackIndent);
			DrawVertReversibleLine(new Point(tabs.Left + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth, tabs.Top + DockConsts.DockSelectionFrameWidth), tabs.Height - 2 * DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth, tabs.Top), tabWidth + DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Left + TabHorzBackIndent + tabWidth - DockConsts.DockSelectionFrameWidth, tabs.Top + DockConsts.DockSelectionFrameWidth), tabs.Height - DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left + TabHorzBackIndent + tabWidth, tabs.Bottom - DockConsts.DockSelectionFrameWidth), tabs.Width - TabHorzBackIndent - tabWidth);
		}
		void DrawBottomTabDockPointer(DockLayout insertLayout, DockLayout parentLayout, Rectangle client, Rectangle tabs, int tabWidth) {
			DrawHorzReversibleLine(client.Location, client.Width - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(client.Left, client.Top + DockConsts.DockSelectionFrameWidth), client.Height - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(client.Right - DockConsts.DockSelectionFrameWidth, client.Top), client.Height);
			DrawHorzReversibleLine(tabs.Location, TabHorzBackIndent);
			DrawVertReversibleLine(new Point(tabs.Left + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth, tabs.Top + DockConsts.DockSelectionFrameWidth), tabs.Height - 2 * DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth, tabs.Top + tabs.Height - DockConsts.DockSelectionFrameWidth), tabWidth + DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Left + TabHorzBackIndent + tabWidth - DockConsts.DockSelectionFrameWidth, tabs.Top), tabs.Height - DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left + TabHorzBackIndent + tabWidth, tabs.Top), tabs.Width - TabHorzBackIndent - tabWidth);
		}
		void DrawLeftTabDockPointer(DockLayout insertLayout, DockLayout parentLayout, Rectangle client, Rectangle tabs, int tabWidth) {
			DrawVertReversibleLine(new Point(client.Right - DockConsts.DockSelectionFrameWidth, client.Top), client.Height - DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(client.Left, client.Top), client.Width - DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(client.Left, client.Bottom - DockConsts.DockSelectionFrameWidth), client.Width);
			DrawVertReversibleLine(new Point(tabs.Right - DockConsts.DockSelectionFrameWidth, tabs.Top), TabHorzBackIndent);
			DrawHorzReversibleLine(new Point(tabs.Left + DockConsts.DockSelectionFrameWidth, tabs.Top + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth), tabs.Width - 2 * DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Left, tabs.Top + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth), tabWidth + DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left + DockConsts.DockSelectionFrameWidth, tabs.Top + TabHorzBackIndent + tabWidth - DockConsts.DockSelectionFrameWidth), tabs.Width - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Right - DockConsts.DockSelectionFrameWidth, tabs.Top + TabHorzBackIndent + tabWidth), tabs.Height - TabHorzBackIndent - tabWidth);
		}
		void DrawRightTabDockPointer(DockLayout insertLayout, DockLayout parentLayout, Rectangle client, Rectangle tabs, int tabWidth) {
			DrawHorzReversibleLine(client.Location, client.Width);
			DrawVertReversibleLine(new Point(client.Left, client.Top + DockConsts.DockSelectionFrameWidth), client.Height - 2 * DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(client.Left, client.Bottom - DockConsts.DockSelectionFrameWidth), client.Width);
			DrawVertReversibleLine(tabs.Location, TabHorzBackIndent);
			DrawHorzReversibleLine(new Point(tabs.Left + DockConsts.DockSelectionFrameWidth, tabs.Top + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth), tabs.Width - 2 * DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Left + tabs.Width - DockConsts.DockSelectionFrameWidth, tabs.Top + TabHorzBackIndent - DockConsts.DockSelectionFrameWidth), tabWidth + DockConsts.DockSelectionFrameWidth);
			DrawHorzReversibleLine(new Point(tabs.Left, tabs.Top + TabHorzBackIndent + tabWidth - DockConsts.DockSelectionFrameWidth), tabs.Width - DockConsts.DockSelectionFrameWidth);
			DrawVertReversibleLine(new Point(tabs.Left, tabs.Top + TabHorzBackIndent + tabWidth), tabs.Height - TabHorzBackIndent - tabWidth);
		}
		public virtual void DrawTabPanel(DrawTabPanelButtonArgs args) {
			DirectionRectangle forePanelRect = new DirectionRectangle(args.Bounds, args.IsVertical), backPanelRect = new DirectionRectangle(args.Bounds, args.IsVertical);
			forePanelRect.SetSize(TabVertForeIndent);
			backPanelRect.RemoveSize(TabVertForeIndent, true);
			if(DockLayoutUtils.IsHead(args.Position)) {
				forePanelRect.SetLocation(backPanelRect.Right);
			}
			else {
				backPanelRect.SetLocation(forePanelRect.Right);
			}
			DrawTabPanelCore(args, forePanelRect.Bounds, backPanelRect.Bounds);
		}
		protected virtual void DrawTabPanelCore(DrawTabPanelButtonArgs args, Rectangle forePanelRect, Rectangle backPanelRect) {
			FillBackPanelRect(args, backPanelRect);
			FillForePanelRect(args, forePanelRect);
		}
		protected internal void UnderlineTabPanel(DrawTabPanelButtonArgs args) {
			if(!CanUnderlineForePanelRect) return;
			DirectionRectangle dBounds = new DirectionRectangle(args.Bounds, args.IsVertical);
			dBounds.SetSize(UnderlineSize);
			dBounds.SetLocation(GetUnderlineBoundsLocation(args));
			UnderlineTabPanelCore(args, dBounds.Bounds);
		}
		protected virtual Point GetUnderlineBoundsLocation(DrawTabPanelButtonArgs args) {
			Point result = Point.Empty;
			result.Y = (args.Position == TabsPosition.Top ? args.ActiveTabBounds.Bottom - 1 : args.ActiveTabBounds.Top);
			result.X = (args.Position == TabsPosition.Left ? args.ActiveTabBounds.Right - 1 : args.ActiveTabBounds.Left);
			return result;
		}
		protected virtual void UnderlineTabPanelCore(DrawTabPanelButtonArgs args, Rectangle bounds) {
			EdgesPaintHelper.DrawColorEdgeCore(args.Graphics, bounds, args.BorderPen, args.BorderPen, args.IsVertical ? EdgePositions.Right : EdgePositions.Bottom);
		}
		protected virtual void FillForePanelRect(DrawTabPanelButtonArgs args, Rectangle forePanelRect) {
			using(Brush br = new SolidBrush(args.ForePanelRectColor)) {
				args.Graphics.FillRectangle(br, forePanelRect);
			}
		}
		protected virtual void FillBackPanelRect(DrawTabPanelButtonArgs args, Rectangle backPanelRect) {
			args.Graphics.FillRectangle(args.TabsAppearance.GetBackBrush(args.Cache, backPanelRect), backPanelRect);
		}
		protected virtual EdgePositions GetActiveTabEdgePositions(TabsPosition position) {
			EdgePositions[] positions = new EdgePositions[] { EdgePositions.TopLeft | EdgePositions.Right, EdgePositions.Left | EdgePositions.BottomRight, 
																EdgePositions.TopLeft | EdgePositions.Bottom, EdgePositions.BottomRight | EdgePositions.Top };
			return positions[DockLayoutUtils.ConvertToIndex(position)];
		}
		protected virtual bool CanUnderlineForePanelRect { get { return true; } }
		protected virtual int TabImage_BorderIndent { get { return 4; } }
		protected virtual int TabText_EmptySpace { get { return 6; } }
		public virtual int TabVertBackIndent { get { return 2; } }
		public virtual int TabVertForeIndent { get { return 2; } }
		public virtual int TabHorzTextIndent { get { return 4; } }
		public virtual int TabHorzImageIndent { get { return 4; } }
		public virtual int TabHorzBackIndent { get { return 3; } }
		public virtual int UnderlineSize { get { return 1; } }
		public virtual Size TabsButtonSize {
			get {
				BorderObjectInfoArgs e = new BorderObjectInfoArgs(null, new Rectangle(Point.Empty, new Size(12, 12)), null);
				e.State = ObjectState.Normal | ObjectState.Hot;
				return LookAndFeelPainters.Border.CalcBoundsByClientRectangle(e).Size;
			}
		}
	}
	public abstract class TabPanelRotatePainter : TabPanelPainter {
		protected const int rotateTabId = 0;
		public TabPanelRotatePainter(DockElementsPainter painter) : base(painter) { }
		protected override void DrawTabCore(DrawTabArgs args) {
			DrawRotatedObject(new DrawTabRotateArgs(args, CalcTabBitmapDestBounds(args),
				CalcTabBitmapSize(args), rotateTabId, args.State));
			DrawTabContent(args);
		}
		protected virtual void DrawRotatedObject(DrawTabRotateArgs args) {
			DirectionSize dSize = new DirectionSize(args.BitmapSize, !args.IsVertical);
			Bitmap bm = new Bitmap(dSize.Width, dSize.Height);
			Graphics bmGraphics = Graphics.FromImage(bm);
			GraphicsCache cache = new GraphicsCache(bmGraphics);
			try {
				DrawObjectCore(cache, new Rectangle(Point.Empty, dSize.DirectSize), args);
				RotateFlipImage(bm, args.Position);
				args.Graphics.DrawImageUnscaled(bm, args.Bounds);
			}
			finally {
				bmGraphics.Dispose();
				cache.Dispose();
				bm.Dispose();
			}
		}
		protected virtual void DrawObjectCore(GraphicsCache cache, Rectangle bounds, DrawTabRotateArgs args) {
			if(args.ElementId == rotateTabId)
				DrawTabCore(cache.Graphics, bounds, (DrawTabArgs)args.NativeArgs);
		}
		protected virtual void RotateFlipImage(Bitmap bm, TabsPosition position) {
			switch(position) {
				case TabsPosition.Bottom:
					bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
					break;
				case TabsPosition.Left:
					bm.RotateFlip(RotateFlipType.Rotate270FlipNone);
					break;
				case TabsPosition.Right:
					bm.RotateFlip(RotateFlipType.Rotate90FlipNone);
					break;
			}
		}
		protected virtual Size CalcTabBitmapSize(DrawTabArgs args) { return args.Bounds.Size; }
		protected virtual Rectangle CalcTabBitmapDestBounds(DrawTabArgs args) { return args.Bounds; }
		protected abstract void DrawTabCore(Graphics bmGraphics, Rectangle bounds, DrawTabArgs args);
	}
	public class HideBarPainter : ObjectPainter {
		protected DockElementsPainter Painter { get; private set; }
		protected HideBarPainter(DockElementsPainter painter) {
			Painter = painter;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawHideBarBackground(e.Cache, (AutoHideContainerViewInfo)e);
			DrawHideBarContent(e.Cache, (AutoHideContainerViewInfo)e);
		}
		protected virtual void DrawHideBarContent(GraphicsCache cache, AutoHideContainerViewInfo info) {
			foreach(AutoHideButtonInfo buttonInfo in info.ButtonInfos) {
				foreach(AutoHideButtonHeaderInfo headerInfo in buttonInfo.Headers) {
					DrawButtonHeaderBackground(cache, headerInfo);
					DrawButtonHeaderContent(cache, headerInfo);
				}
			}
		}
		protected virtual void DrawHideBarBackground(GraphicsCache cache, AutoHideContainerViewInfo info) {
			info.PaintAppearance.DrawBackground(cache, info.Bounds);
		}
		protected RotateFlipType GetRotateFlipType(TabsPosition position) {
			switch(position) {
				case TabsPosition.Top:
					return RotateFlipType.RotateNoneFlipY;
				case TabsPosition.Left:
					return RotateFlipType.Rotate90FlipNone;
				case TabsPosition.Right:
					return RotateFlipType.Rotate90FlipX;
			}
			return RotateFlipType.RotateNoneFlipNone;
		}
		protected virtual EdgesType GetHideBarButtonEdgesType() {
			return EdgesType.Standard;
		}
		protected virtual EdgePositions GetEdges(TabsPosition position) {
			int posIndex = DockLayoutUtils.ConvertToIndex(position);
			EdgePositions[] edges = new EdgePositions[] { 
				EdgePositions.Left | EdgePositions.BottomRight, 
				EdgePositions.TopLeft | EdgePositions.Right,
				EdgePositions.Top | EdgePositions.BottomRight, 
				EdgePositions.TopLeft | EdgePositions.Bottom};
			return edges[posIndex];
		}
		protected virtual void DrawButtonHeaderBackground(GraphicsCache cache, AutoHideButtonHeaderInfo headerInfo) {
			Rectangle r = headerInfo.GetRelativeRect(headerInfo.Bounds);
			headerInfo.PaintAppearance.DrawBackground(cache, r);
			EdgesPaintHelper.DrawColorEdge(cache, r, headerInfo.PaintAppearance.GetBorderColor(),
				GetHideBarButtonEdgesType(), GetEdges(headerInfo.Position));
		}
		protected virtual void DrawButtonHeaderContent(GraphicsCache cache, AutoHideButtonHeaderInfo headerInfo) {
			headerInfo.DrawImage(cache);
			headerInfo.DrawText(cache);
		}
		public virtual int GetHideBarSize(Size imageSize, AppearanceObject appearance) {
			int textHeight = Painter.GetTextHeight(appearance);
			Padding m = GetTabButtonContentMargin();
			return HideBarVertInterval + m.Vertical + Math.Max(textHeight, imageSize.Height);
		}
		protected internal virtual Padding GetTabButtonContentMargin() {
			return new Padding(2);
		}
		public virtual int HideBarVertInterval { get { return 2; } }
		public virtual int HideBarHorzInterval { get { return 4; } }
		public virtual int NextTabHeaderGrow { get { return 1; } }
		public virtual int TabHeaderContentIndent { get { return 4; } }
		public virtual int ImageToTextInterval { get { return 2; } }
	}
	public class ButtonPainter : ElementPainter {
		#region const
		public const int
			MinimizeIndex = 0,
			MaximizeIndex = 1,
			CloseIndex = 2,
			HideIndex = 3,
			PinDownIndex = 4,
			MinimizeHotIndex = 5,
			MaximizeHotIndex = 6,
			CloseHotIndex = 7,
			HideHotIndex = 8,
			PinDownHotIndex = 9;
		public const int
			MaximizeButtonVisibleIndex = 100,
			PinButtonVisibleIndex = 101,
			CloseButtonVisibleIndex = 102;
		public const int HotIndexOffset = 5;
		#endregion
		#region static
		[ThreadStatic]
		static ImageList images;
		static ImageList Images {
			get {
				if(images == null) {
					images = ResourceImageHelper.CreateImageListFromResources(
						"DevExpress.XtraBars.Docking.Paint.ButtonImages.bmp",
						typeof(ButtonPainter).Assembly, new Size(10, 10), Color.Magenta);
				}
				return images;
			}
		}
		#endregion
		public ButtonPainter(DockElementsPainter painter)
			: base(painter) {
		}
		public virtual void DrawButton(DrawApplicationCaptionArgs captionArgs, DockPanelCaptionButton args, int imageIndex) {
			if(args.Bounds.IsEmpty) return;
			args.Selected = captionArgs.ActiveCaption;
			DrawButtonBackground(captionArgs, args);
			DrawButtonImage(captionArgs.Cache, args, imageIndex);
		}
		protected virtual void DrawButtonBackground(DrawApplicationCaptionArgs captionArgs, DockPanelCaptionButton args) {
			DrawButtonBounds(captionArgs.Cache, args);
		}
		protected virtual void DrawButtonImage(GraphicsCache cache, DockPanelCaptionButton args, int imageIndex) {
			Images.Draw(cache.Graphics, GetImageLocation(args.Bounds), imageIndex);
		}
		protected virtual void DrawButtonBounds(GraphicsCache cache, DockPanelCaptionButton args) {
			LookAndFeelPainters.Border.DrawObject(CreateButtonArgs(cache, args));
		}
		protected BorderObjectInfoArgs CreateButtonArgs(GraphicsCache cache, DockPanelCaptionButton args) {
			BorderObjectInfoArgs btnArgs = new BorderObjectInfoArgs(cache);
			btnArgs.Bounds = args.Bounds;
			btnArgs.State = args.ButtonState;
			return btnArgs;
		}
		protected BorderObjectInfoArgs CreateButtonArgs(ObjectState state) {
			BorderObjectInfoArgs btnArgs = new BorderObjectInfoArgs();
			btnArgs.Bounds = new Rectangle(Point.Empty, ImageSize);
			btnArgs.State = state;
			return btnArgs;
		}
		protected static bool IsHotOrPressed(ObjectState state) {
			return ((state & ObjectState.Hot) != 0 || (state & ObjectState.Pressed) != 0);
		}
		protected Point GetImageLocation(Rectangle bounds) {
			return new Point(bounds.Left + (bounds.Width - ImageSize.Width) / 2, bounds.Top + (bounds.Height - ImageSize.Height) / 2);
		}
		protected virtual Size ImageSize {
			get { return Images.ImageSize; }
		}
		public virtual Size ButtonSize {
			get {
				BorderObjectInfoArgs e = CreateButtonArgs(ObjectState.Normal | ObjectState.Hot);
				Rectangle bounds = LookAndFeelPainters.Border.CalcBoundsByClientRectangle(e);
				return bounds.Size;
			}
		}
		protected internal virtual Color GetGlyphSkinningColor(XtraEditors.ButtonPanel.BaseButtonInfo info, DockPanel panel) {
			return (panel.DockLayout.Float || panel.CanDrawCaptionActive) ? Color.White : SystemColors.ControlText;
		}
	}
	public class WindowPainter : ElementPainter {
		public WindowPainter(DockElementsPainter painter) : base(painter) { }
		public virtual void DrawWindowCaption(DrawWindowCaptionArgs e) {
			if(e.Bounds.IsEmpty) return;
			DrawWindowCaptionBackground(e);
			DrawCaptionText(e);
			DrawCaptionImage(e);
		}
		public virtual void DrawApplicationCaption(DrawApplicationCaptionArgs e) {
			if(e.Bounds.IsEmpty) return;
			DrawApplicationCaptionBackgroud(e);
			DrawCaptionText(e);
			DrawCaptionImage(e);
		}
		public virtual Rectangle GetClientRect(Rectangle wndRect) {
			Rectangle rect = BorderPainter.GetObjectClientRectangle(new ObjectInfoArgs(null, wndRect, ObjectState.Normal));
			rect.Inflate(-AdditionalBorderWidth, -AdditionalBorderWidth); 
			return rect;
		}
		public virtual Rectangle UpdateBorderAndClientBounds(Rectangle borderBounds, ref Rectangle clientBounds, bool floating) { return borderBounds; }
		protected virtual void DrawApplicationCaptionBackgroud(DrawApplicationCaptionArgs e) {
			e.DrawBackground();
		}
		protected virtual void DrawWindowCaptionBackground(DrawWindowCaptionArgs e) {
			e.DrawBackground();
		}
		protected virtual void DrawCaptionText(DrawApplicationCaptionArgs e) {
			e.DrawString(e.Caption, e.TextBounds);
		}
		protected virtual void DrawCaptionImage(DrawApplicationCaptionArgs e) {
			if(e.ImageBounds.IsEmpty || !IsValidImage(e)) return;
			if(e.AllowGlyphSkinning) {
				var attributes = ImageColorizer.GetColoredAttributes(e.Appearance.GetForeColor());
				if(e.Image != null)
					e.Cache.Graphics.DrawImage(e.Image, e.ImageBounds, 0, 0, e.Image.Width, e.Image.Height, GraphicsUnit.Pixel, attributes);
				else
					ImageCollection.DrawImageListImage(e.Cache, e.Images, e.ImageIndex, e.ImageBounds, attributes);
			}
			else {
				if(e.Image != null)
					e.Cache.Graphics.DrawImage(e.Image, e.ImageBounds);
				else
					ImageCollection.DrawImageListImage(e.Cache, e.Images, e.ImageIndex, e.ImageBounds);
			}
		}
		bool IsValidImage(DrawApplicationCaptionArgs e) {
			return e.Image != null || ImageCollection.IsImageListImageExists(e.Images, e.ImageIndex); 
		}
		protected internal virtual int GetImageIndex(int index, ObjectState state, CaptionButtonStatus status) {
			return index + ((state & ObjectState.Hot) != 0 ? ButtonPainter.HotIndexOffset : 0);
		}
		public virtual void DrawSelectedComponentFrame(Graphics g, bool active, Rectangle bounds, Color backColor) {
			ControlPaint.DrawSelectionFrame(g, active, bounds, Rectangle.Inflate(bounds, -SelectedComponentFrameWidth, -SelectedComponentFrameWidth), backColor);
		}
		public virtual void DrawBorder(DrawBorderArgs args) {
			if(args.IsFloat)
				DrawApplicationBorder(args);
			else
				DrawWindowBorder(args);
		}
		protected virtual void DrawWindowBorder(DrawBorderArgs args) {
			Rectangle rect = args.Bounds, r;
			args.Paint.DrawRectangle(args.Graphics, args.BorderPen, rect);
			args.BorderArgs.Bounds = Rectangle.Inflate(args.BorderArgs.Bounds, -1, -1);
			DrawWindowBorderCore(args.BorderArgs);
			r = BorderPainter.GetObjectClientRectangle(args.BorderArgs);
			args.BorderArgs.Bounds = rect;
			args.Paint.DrawRectangle(args.Graphics, args.BorderPen, r);
		}
		protected virtual void DrawApplicationBorder(DrawBorderArgs args) {
			Rectangle rect = args.Bounds;
			EdgesPaintHelper.DrawColorEdgeCore(args.Graphics, rect, args.Cache.GetPen(ControlPaint.Light(args.CaptionColor)), args.Cache.GetPen(ControlPaint.Dark(args.CaptionColor)), EdgePositions.Rect);
			rect.Inflate(-1, -1);
			args.Paint.DrawRectangle(args.Graphics, args.CaptionPen, rect);
			rect.Inflate(-1, -1);
			DrawInnerApplicationBorderFrame(args, rect);
		}
		protected virtual void DrawInnerApplicationBorderFrame(DrawBorderArgs args, Rectangle bounds) {
			args.Paint.DrawRectangle(args.Graphics, args.CaptionPen, bounds);
		}
		protected internal void DrawWindowBorderCore(BorderObjectInfoArgs args) {
			BorderPainter.DrawObject(args);
		}
		protected virtual BorderPainter BorderPainter {
			get { return BorderHelper.GetPainter(BorderStyle, LookAndFeel); }
		}
		protected DevExpress.XtraEditors.Controls.BorderStyles BorderStyle { get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; ; } }
		public virtual Size GetSizeByClientSize(Size clientSize) {
			clientSize.Height += AdditionalBorderWidth;
			clientSize.Width += AdditionalBorderWidth;
			return BorderPainter.CalcBoundsByClientRectangle(new ObjectInfoArgs(null, new Rectangle(Point.Empty, clientSize), ObjectState.Normal)).Size;
		}
		public virtual Rectangle GetCaptionClientBounds(Rectangle bounds, bool floating) { return bounds; }
		public virtual void DrawControlContainerClientArea(DrawArgs args) {
			args.DrawBackground();
		}
		public virtual int CaptionButtonInterval { get { return 2; } }
		public virtual int CaptionVertIndent { get { return 2; } }
		public virtual int SelectedComponentFrameWidth { get { return 7; } }
		public Size BorderSize { get { return GetSizeByClientSize(LayoutConsts.InvalidSize); } }
		protected virtual int AdditionalBorderWidth { get { return 2; } }
		public virtual bool CanVerticalCaption { get { return false; } }
		public virtual Size ApplicationBorderSize { get { return SystemInformation.BorderSize; } }
	}
}
