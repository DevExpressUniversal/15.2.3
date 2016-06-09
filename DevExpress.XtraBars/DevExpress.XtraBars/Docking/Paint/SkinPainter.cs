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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Styles;
namespace DevExpress.XtraBars.Docking.Paint {
	public class DockElementsSkinPainter : DockElementsPainter {
		TabSkinPaintHelper paintHelper;
		public DockElementsSkinPainter(SkinBarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override void CreateElementPainters() {
			fHideBarPainter = new HideBarSkinPainter(this);
			fTabPanelPainter = new TabPanelSkinPainter(this);
			fButtonPainter = new ButtonSkinPainter(this);
			fWindowPainter = new WindowSkinPainter(this);
			this.paintHelper = new TabSkinPaintHelper(this);
		}
		protected override ObjectPainter CreateButtonsPanelPainter() {
			return new Docking2010.ButtonsPanelSkinPainter(PaintStyle as ISkinProvider);
		}
		public override int GetCaptionHeight(AppearanceObject appearance, bool floating) {
			return ((WindowSkinPainter)WindowPainter).GetCaptionBoundsByClientRectangle(
				new Rectangle(0, 0, 0, base.GetCaptionHeight(appearance, floating)), floating).Height;
		}
		public TabSkinPaintHelper PaintHelper { get { return paintHelper; } }
	}
	public class HideBarSkinPainter : HideBarPainter {
		public HideBarSkinPainter(DockElementsSkinPainter painter)
			: base(painter) {
		}
		protected virtual Skin GetSkin() {
			return DockingSkins.GetSkin((ISkinProvider)Painter.PaintStyle);
		}
		protected virtual SkinElement GetHeaderElement() {
			return GetSkin()[DockingSkins.SkinTabHeaderHideBar];
		}
		protected virtual SkinElement GetHideBarElement(TabsPosition position) {
			String hideBarName = DockingSkins.SkinHideBar;
			switch(position) {
				case TabsPosition.Bottom:
					hideBarName = DockingSkins.SkinHideBarBottom;
					break;
				case TabsPosition.Left:
					hideBarName = DockingSkins.SkinHideBarLeft;
					break;
				case TabsPosition.Right:
					hideBarName = DockingSkins.SkinHideBarRight;
					break;
			}
			return GetSkin()[hideBarName];
		}
		protected override void DrawHideBarBackground(GraphicsCache cache, AutoHideContainerViewInfo info) {
			TabsPosition position = info.Owner.Position;
			SkinElementInfo skinInfo = new SkinElementInfo(GetHideBarElement(position), info.Bounds);
			skinInfo.RightToLeft = info.IsRightToLeft;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinInfo);
		}
		protected override void DrawButtonHeaderBackground(GraphicsCache cache, AutoHideButtonHeaderInfo headerInfo) {
			Rectangle bounds = headerInfo.GetRelativeRect(headerInfo.Bounds);
			SkinElementInfo skinInfo = new SkinElementInfo(GetHeaderElement(), bounds);
			skinInfo.RightToLeft = headerInfo.Header.IsRightToLeft;
			skinInfo.BackAppearance = headerInfo.PaintAppearance;
			skinInfo.State |= (headerInfo.IsHot ? ObjectState.Selected : ObjectState.Normal);
			skinInfo.ImageIndex += (headerInfo.IsHot ? 2 : 0);
			new RotateObjectPaintHelper().DrawRotated(cache, skinInfo, SkinElementPainter.Default,
				GetRotateFlipType(headerInfo.Position));
		}
		protected internal override Padding GetTabButtonContentMargin() {
			SkinElementInfo info = new SkinElementInfo(GetHeaderElement());
			Rectangle client = new Rectangle(0, 0, 100, 20);
			Rectangle bounds = SkinElementPainter.Default.CalcBoundsByClientRectangle(info, client);
			return new Padding(client.Left - bounds.Left, client.Top - bounds.Top,
				bounds.Right - client.Right, bounds.Bottom - client.Bottom);
		}
		public override int NextTabHeaderGrow { get { return 0; } }
	}
	public class TabPanelSkinPainter : TabPanelRotatePainter {
		protected const int rotateTabHeaderLineId = 1;
		public TabPanelSkinPainter(DockElementsSkinPainter painter) : base(painter) { }
		protected override int GetTabTextWidth(Graphics g, AppearanceObject appearance, string tabText, bool isVertical) {
			return PaintHelper.CalcBoundsSizeByClientSize(new Size(base.GetTabTextWidth(g, appearance, tabText, isVertical), 10), DockingSkins.SkinTabHeader).Width;
		}
		protected override void DrawTabCore(Graphics bmGraphics, Rectangle bounds, DrawTabArgs args) {
			PaintHelper.DrawTab(bmGraphics, args, bounds);
		}
		protected override void DrawTabPanelCore(DrawTabPanelButtonArgs args, Rectangle forePanelRect, Rectangle backPanelRect) {
			PaintHelper.DrawTabPane(args, args.Bounds);
			UnderlineTabPanel(args);
		}
		protected override void DrawTabContent(DrawTabArgs args) {
			if(args.State == ObjectState.Hot && args.ActiveChildIndex != args.TabIndex) {
				args = new DrawTabArgs(args.Cache, args.Bounds, args.TabPanelBounds, args.TabLayout.TabsAppearanceHot, args.TabContainer, args.TabIndex, args.State);
			}
			base.DrawTabContent(args);
		}
		protected override Point GetUnderlineBoundsLocation(DrawTabPanelButtonArgs args) {
			Point result = Point.Empty;
			result.Y = (args.Position == TabsPosition.Top ? args.Bounds.Bottom - UnderlineSize : args.Bounds.Top);
			result.X = (args.Position == TabsPosition.Left ? args.Bounds.Right - UnderlineSize : args.Bounds.Left);
			return result;
		}
		protected override void UnderlineTabPanelCore(DrawTabPanelButtonArgs args, Rectangle bounds) {
			if(args.IsVertical)
				bounds.Inflate(0, PaintHelper.TabHeaderLineHGrow);
			else
				bounds.Inflate(PaintHelper.TabHeaderLineHGrow, 0);
			DrawRotatedObject(new DrawTabRotateArgs(args, bounds, bounds.Size, rotateTabHeaderLineId, ObjectState.Normal));
		}
		protected override void DrawObjectCore(GraphicsCache cache, Rectangle bounds, DrawTabRotateArgs args) {
			if(args.ElementId == rotateTabHeaderLineId)
				PaintHelper.UnderlineTabPanel(cache, bounds);
			else
				base.DrawObjectCore(cache, bounds, args);
		}
		protected override Size CalcTabBitmapSize(DrawTabArgs args) {
			DirectionSize dSize = new DirectionSize(base.CalcTabBitmapSize(args), args.IsVertical);
			if(args.IsActive)
				dSize.Width += PaintHelper.HeaderDownGrow;
			else
				dSize.Width += ClipTabsIndent;
			return dSize.Size;
		}
		protected override Rectangle CalcTabBitmapDestBounds(DrawTabArgs args) {
			DirectionRectangle dBounds = new DirectionRectangle(base.CalcTabBitmapDestBounds(args), !args.IsVertical);
			if(!DockLayoutUtils.IsHead(args.Position)) {
				if(args.IsActive)
					dBounds.IncreaseY(-PaintHelper.HeaderDownGrow);
				else
					dBounds.IncreaseY(-ClipTabsIndent);
			}
			return dBounds.Bounds;
		}
		public override Rectangle UpdateActiveTabBounds(Rectangle bounds, TabsPosition position) {
			return PaintHelper.UpdateActiveTabBounds(bounds, position);
		}
		protected override Rectangle CalcTabContentBounds(BaseDrawTabArgs args) {
			return PaintHelper.CalcTabContentBounds(args, DockingSkins.SkinTabHeader);
		}
		public override int GetTabsHeight(DockLayout dockLayout, AppearanceObject appearance1, AppearanceObject appearance2) {
			Size size = new Size(10, base.GetTabsHeight(dockLayout, appearance1, appearance2));
			return PaintHelper.CalcBoundsSizeByClientSize(size, DockingSkins.SkinTabHeader).Height;
		}
		protected override void FillForePanelRect(DrawTabPanelButtonArgs args, Rectangle forePanelRect) { }
		protected TabSkinPaintHelper PaintHelper { get { return (Painter as DockElementsSkinPainter).PaintHelper; } }
		protected virtual int ClipTabsIndent { get { return 3; } }
		public override int TabVertBackIndent { get { return PaintHelper.HeaderUpGrow + 1; } }
		public override int TabVertForeIndent { get { return UnderlineSize; } }
		public override int TabHorzTextIndent { get { return 2; } }
		public override int UnderlineSize { get { return PaintHelper.UnderlineWidth; } }
	}
	public class ButtonSkinPainter : ButtonPainter {
		public ButtonSkinPainter(DockElementsSkinPainter painter)
			: base(painter) {
		}
		protected Skin GetSkin() {
			return DockingSkins.GetSkin((SkinBarManagerPaintStyle)PaintStyle);
		}
		protected SkinElementInfo GetSkinElementInfo(string name) {
			return new SkinElementInfo(GetSkin()[name]);
		}
		protected SkinElementInfo GetSkinElementInfo(string name, Rectangle bounds) {
			return new SkinElementInfo(GetSkin()[name], bounds);
		}
		protected override void DrawButtonBackground(DrawApplicationCaptionArgs captionArgs, DockPanelCaptionButton args) {
			SkinElementInfo info = GetSkinElementInfo(DockingSkins.SkinDockWindowButton, args.Bounds);
			info.ImageIndex = (captionArgs.ActiveCaption ? -1 : 4);
			info.State = args.ButtonState;
			if(info.State != ObjectState.Normal) info.ImageIndex = -1;
			ObjectPainter.DrawObject(captionArgs.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawButtonImage(GraphicsCache cache, DockPanelCaptionButton args, int imageIndex) {
			ImageCollection.DrawImageListImage(cache, GetImages(args), imageIndex % 5, new Rectangle(GetImageLocation(args.Bounds), ImageSize));
		}
		public override Size ButtonSize {
			get {
				return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default,
					GetSkinElementInfo(DockingSkins.SkinDockWindowButton),
					new Rectangle(Point.Empty, ImageSize)).Size;
			}
		}
		protected override Size ImageSize {
			get {
				var images = GetImages(null);
				if(images == null) return Size.Empty;
				return images.ImageSize;
			}
		}
		ImageCollection GetImages(DockPanelCaptionButton args) {
			SkinImage image = GetDockWindowButtonsSkinElement(args).Image;
			return (image == null) ? null : image.GetImages();
		}
		SkinElement GetDockWindowButtonsSkinElement(DockPanelCaptionButton args) {
			string imageName = BarSkins.SkinDockWindowButtons;
			if(args != null) {
				if(args.Selected && GetBarSkinElement(BarSkins.SkinDockWindowButtonsSelected) != null)
					imageName = BarSkins.SkinDockWindowButtonsSelected;
				if(args.Hot) imageName = BarSkins.SkinDockWindowButtonsHot;
				if(args.Pressed) imageName = BarSkins.SkinDockWindowButtonsPressed;
			}
			return GetBarSkinElement(imageName);
		}
		SkinElement GetBarSkinElement(string elementName) {
			return BarSkins.GetSkin((SkinBarManagerPaintStyle)PaintStyle)[elementName];
		}
		protected internal override Color GetGlyphSkinningColor(XtraEditors.ButtonPanel.BaseButtonInfo info, DockPanel panel) {
			DockPanelCaptionButton args = new DockPanelCaptionButton();
			args.ButtonState = info.State;
			if(panel.CanDrawCaptionActive || (panel.FloatForm != null && panel.FloatForm == Form.ActiveForm))
				args.ButtonState |= ObjectState.Selected;
			SkinElement element = GetDockWindowButtonsSkinElement(args);
			if(element != null) {
				Color skinColor = element.GetForeColor(ObjectState.Normal);
				if(!skinColor.IsEmpty)
					return skinColor;
				skinColor = GetDockWindowButtonsForeColor();
				if(!skinColor.IsEmpty)
					return skinColor;
			}
			return panel.GetCaptionAppearance().GetForeColor();
		}
		protected virtual Color GetDockWindowButtonsForeColor() {
			return BarSkins.GetSkin((ISkinProvider)PaintStyle).Colors.GetColor(BarSkins.DockWindowButtonsForeColor, Color.Empty);
		}
	}
	public class WindowSkinPainter : WindowPainter {
		public WindowSkinPainter(DockElementsSkinPainter painter) : base(painter) { }
		public override void DrawControlContainerClientArea(DrawArgs args) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinGroupPanelNoBorder], args.Bounds);
			info.BackAppearance = args.Appearance;
			info.Cache = args.Cache;
			DefaultSkinPainter.DrawObject(info);
		}
		protected override void DrawWindowCaptionBackground(DrawWindowCaptionArgs args) {
			SkinElementInfo info = new SkinElementInfo(Skin[DockingSkins.SkinDockWindow], args.Bounds);
			info.ImageIndex = (args.ActiveCaption ? 0 : 1);
			ObjectPainter.DrawObject(args.Cache, DefaultSkinPainter, info);
		}
		protected override void DrawCaptionText(DrawApplicationCaptionArgs e) {
			e.DrawString(e.Caption, e.TextBounds);
		}
		protected override void DrawApplicationCaptionBackgroud(DrawApplicationCaptionArgs args) {
			SkinElementInfo info = new SkinElementInfo(Skin[DockingSkins.SkinFloatingWindow], args.Bounds);
			info.ImageIndex = (args.ActiveCaption ? 0 : 1);
			ObjectPainter.DrawObject(args.Cache, DefaultSkinPainter, info);
		}
		protected override void DrawWindowBorder(DrawBorderArgs args) {
			SkinElementInfo info = new SkinElementInfo(Skin[DockingSkins.SkinDockWindowBorder], args.Bounds);
			ObjectPainter.DrawObject(args.Cache, DefaultSkinPainter, info);
		}
		protected override void DrawApplicationBorder(DrawBorderArgs args) {
			SkinElementInfo info = new SkinElementInfo(Skin[DockingSkins.SkinFloatingWindowBorder], args.Bounds);
			ObjectPainter.DrawObject(args.Cache, DefaultSkinPainter, info);
		}
		public override Rectangle GetClientRect(Rectangle wndRect) { return wndRect; }
		public override Rectangle UpdateBorderAndClientBounds(Rectangle borderBounds, ref Rectangle clientBounds, bool floating) {
			borderBounds = clientBounds;
			SkinElementInfo info = new SkinElementInfo(Skin[floating ? DockingSkins.SkinFloatingWindowBorder : DockingSkins.SkinDockWindowBorder], clientBounds);
			clientBounds = ObjectPainter.GetObjectClientRectangle(null, DefaultSkinPainter, info);
			return borderBounds;
		}
		protected internal Rectangle GetCaptionBoundsByClientRectangle(Rectangle bounds, bool floating) {
			return ObjectPainter.CalcBoundsByClientRectangle(null, DefaultSkinPainter,
				new SkinElementInfo(Skin[floating ? DockingSkins.SkinFloatingWindow : DockingSkins.SkinDockWindow]),
				bounds);
		}
		public override Rectangle GetCaptionClientBounds(Rectangle bounds, bool floating) {
			return ObjectPainter.GetObjectClientRectangle(null, DefaultSkinPainter,
				new SkinElementInfo(Skin[floating ? DockingSkins.SkinFloatingWindow : DockingSkins.SkinDockWindow],
				bounds));
		}
		public override int CaptionVertIndent { get { return 0; } }
		protected Skin Skin { get { return ((DockElementsSkinPainter)Painter).PaintHelper.Skin; } }
		protected SkinElementPainter DefaultSkinPainter { get { return SkinElementPainter.Default; } }
	}
	public class TabSkinPaintHelper {
		DockElementsSkinPainter painter;
		public TabSkinPaintHelper(DockElementsSkinPainter painter) {
			this.painter = painter;
		}
		public void DrawTabPane(DrawArgs args, Rectangle bounds) {
			SkinElementInfo info = CreateSkinElementInfo(DockingSkins.SkinTabHeaderBackground, bounds);
			info.Cache = args.Cache;
			DefaultSkinPainter.DrawObject(info);
		}
		public void DrawTab(Graphics g, BaseDrawTabArgs args, Rectangle bounds) {
			DrawTabCore(g, args, bounds, TabSkinHeaderName);
		}
		public void DrawHideBarTab(Graphics g, BaseDrawTabArgs args, Rectangle bounds, string hideBarSkinElementName) {
			DrawTabCore(g, args, bounds, hideBarSkinElementName);
		}
		protected int GetImageIndex(BaseDrawTabArgs args) {
			int result = 0;
			if(args.State == ObjectState.Hot) result++;
			result += args.IsActive ? 2 : 0;
			return result;
		}
		void DrawTabCore(Graphics g, BaseDrawTabArgs args, Rectangle bounds, string skinName) {
			SkinElementInfo info = new SkinElementInfo(Skin[skinName], bounds);
			info.Graphics = g;
			info.BackAppearance = args.Appearance;
			info.State = args.State;
			if(args.IsActive) info.State |= ObjectState.Selected;
			info.ImageIndex = GetImageIndex(args);
			DefaultSkinPainter.DrawObject(info);
		}
		public Size CalcBoundsSizeByClientSize(Size clientSize, string skinName) {
			return DefaultSkinPainter.CalcBoundsByClientRectangle(CreateSkinElementInfo(skinName, new Rectangle(Point.Empty, clientSize))).Size;
		}
		public Rectangle CalcTabContentBounds(BaseDrawTabArgs args, string skinName) {
			Rectangle bounds = args.Bounds;
			if(args.IsVertical) bounds = DockLayoutUtils.FlipRectangle(bounds);
			Rectangle result = DefaultSkinPainter.GetObjectClientRectangle(CreateSkinElementInfo(skinName, bounds));
			if(!DockLayoutUtils.IsHead(args.Position))
				result.Y = bounds.Top + (bounds.Bottom - result.Bottom);
			if(args.IsVertical) result = DockLayoutUtils.FlipRectangle(result);
			if(args.IsActive && skinName == DockingSkins.SkinTabHeader) {
				if(args.IsVertical) result.Y += HeaderHGrow;
				else result.X += HeaderHGrow;
			}
			return result;
		}
		SkinElementInfo CreateSkinElementInfo(string skinName, Rectangle bounds) {
			return new SkinElementInfo(Skin[skinName], bounds);
		}
		public Rectangle UpdateActiveTabBounds(Rectangle bounds, TabsPosition position) {
			DirectionRectangle dBounds = new DirectionRectangle(bounds, !DockLayoutUtils.IsVerticalPosition(position));
			int increaseWidth = HeaderHGrow;
			int increaseTop = HeaderUpGrow;
			dBounds.Inflate(increaseWidth);
			dBounds.IncreaseHeight(increaseTop);
			if(DockLayoutUtils.IsHead(position))
				dBounds.IncreaseY(-increaseTop);
			return dBounds.Bounds;
		}
		public void UnderlineTabPanel(GraphicsCache cache, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, DefaultSkinPainter, CreateSkinElementInfo(DockingSkins.SkinTabHeaderLine, bounds));
		}
		protected int HeaderHGrow { get { return Skin.Properties.GetInteger(TabSkinProperties.SelectedHeaderHGrow); } }
		public int HeaderUpGrow { get { return Skin.Properties.GetInteger(TabSkinProperties.SelectedHeaderUpGrow); } }
		public int HeaderDownGrow { get { return Skin.Properties.GetInteger(TabSkinProperties.SelectedHeaderDownGrow); } }
		public int TabHeaderLineHGrow { get { return Skin.Properties.GetInteger(DockingSkins.TabHeaderLineHGrow); } }
		public ISkinProvider SkinProvider { get { return (SkinBarManagerPaintStyle)Painter.PaintStyle; } }
		protected UserLookAndFeel LookAndFeel { get { return Painter.LookAndFeel; } }
		protected SkinElementPainter DefaultSkinPainter { get { return SkinElementPainter.Default; } }
		public Skin Skin { get { return DockingSkins.GetSkin(SkinProvider); } }
		public int UnderlineWidth { get { return ObjectPainter.CalcObjectMinBounds(null, DefaultSkinPainter, CreateSkinElementInfo(DockingSkins.SkinTabHeaderLine, Rectangle.Empty)).Height; } }
		string TabSkinHeaderName { get { return DockingSkins.SkinTabHeader; } }
		DockElementsSkinPainter Painter { get { return painter; } }
	}
}
