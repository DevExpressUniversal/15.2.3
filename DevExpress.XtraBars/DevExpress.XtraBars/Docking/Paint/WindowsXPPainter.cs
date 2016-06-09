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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Styles;
namespace DevExpress.XtraBars.Docking.Paint {
	public class DockElementsWindowsXPPainter : DockElementsPainter {
		public DockElementsWindowsXPPainter(BarManagerWindowsXPPaintStyle paintStyle) : base(paintStyle) { }
		protected override void CreateElementPainters() {
			fHideBarPainter = new HideBarWindowsXPPainter(this);
			fTabPanelPainter = new TabPanelWindowsXPPainter(this);
			fButtonPainter = new ButtonWindowsXPPainter(this);
			fWindowPainter = new WindowWindowsXPPainter(this);
		}
		public override int GetCaptionHeight(AppearanceObject appearance, bool floating) {
			const int frameWidth = 1;
			return base.GetCaptionHeight(appearance, floating) + 2 * frameWidth;
		}
		protected override ObjectPainter CreateButtonsPanelPainter() {
			return new XtraEditors.ButtonPanel.BaseButtonsPanelWindowsXpPainter();
		}
		protected override void DrawActiveTab(DrawTabArgs tabArgs) {
			if(tabArgs == null) return;
			DirectionRectangle dBounds = new DirectionRectangle(tabArgs.TabPanelBounds, DockLayoutUtils.ConvertToOppositeDockingStyle(tabArgs.Position));
			dBounds.SetLocation(dBounds.Left + (DockLayoutUtils.IsHead(tabArgs.Position) ? 1 : -1));
			WindowPainter.DrawWindowBorderCore(new BorderObjectInfoArgs(tabArgs.Cache, dBounds.GetSideRectangle(2), null));
			tabArgs = new DrawTabArgs(tabArgs, Rectangle.Inflate(tabArgs.Bounds, 1, 1), ObjectState.Normal);
			base.DrawActiveTab(tabArgs);
		}
	}
	public class HideBarWindowsXPPainter : HideBarPainter {
		public HideBarWindowsXPPainter(DockElementsWindowsXPPainter painter)
			: base(painter) {
		}
		protected override void DrawHideBarBackground(GraphicsCache cache, AutoHideContainerViewInfo info) {
			WXPPainterArgs wa = new WXPPainterArgs("tab", XPConstants.TABP_BODY, 0);
			wa.Bounds = info.Bounds;
			WXPPainter.Default.DrawTheme(wa, cache.Graphics, null);
		}
		protected override void DrawButtonHeaderBackground(GraphicsCache cache, AutoHideButtonHeaderInfo headerInfo) {
			XPButtonHeaderArgs infoArgs = new XPButtonHeaderArgs(headerInfo);
			new RotateObjectPaintHelper().DrawRotated(cache, infoArgs,
				XPButtonHeaderPainter.Default, GetRotateFlipType(headerInfo.Position));
		}
		class XPButtonHeaderArgs : ObjectInfoArgs {
			public XPButtonHeaderArgs(AutoHideButtonHeaderInfo headerInfo) {
				IsHot = headerInfo.IsHot;
				Bounds = headerInfo.GetRelativeRect(headerInfo.Bounds);
			}
			public bool IsHot { get; private set; }
		}
		class XPButtonHeaderPainter : ObjectPainter {
			public static XPButtonHeaderPainter Default = new XPButtonHeaderPainter();
			public override void DrawObject(ObjectInfoArgs e) {
				base.DrawObject(e);
				WXPPainterArgs wa = new WXPPainterArgs("tab", XPConstants.TABP_TOPTABITEM, ((XPButtonHeaderArgs)e).IsHot ?
					XPConstants.TTIBES_HOT : XPConstants.TTIS_NORMAL);
				wa.Bounds = e.Bounds;
				WXPPainter.Default.DrawTheme(wa, e.Cache.Graphics, null);
			}
		}
	}
	public class TabPanelWindowsXPPainter : TabPanelRotatePainter {
		public TabPanelWindowsXPPainter(DockElementsWindowsXPPainter painter) : base(painter) { }
		protected override void DrawTabCore(Graphics bmGraphics, Rectangle bounds, DrawTabArgs args) {
			WXPPainterArgs wa = new WXPPainterArgs("tab", XPConstants.TABP_TOPTABITEM, args.IsActive ? XPConstants.TTIS_SELECTED : XPConstants.TTIS_NORMAL);
			wa.Bounds = bounds;
			WXPPainter.Default.DrawTheme(wa, bmGraphics, null);
		}
		protected override void DrawTabPanelCore(DrawTabPanelButtonArgs args, Rectangle forePanelRect, Rectangle backPanelRect) {
			WXPPainterArgs wa = new WXPPainterArgs("tab", XPConstants.TABP_BODY, 0);
			wa.Bounds = args.Bounds;
			WXPPainter.Default.DrawTheme(wa, args.Graphics, null);
		}
		protected virtual int UnderlineStrokeWidth { get { return 2; } }
		protected override int TabImage_BorderIndent { get { return base.TabImage_BorderIndent + UnderlineStrokeWidth + 1; } }
		public override int TabVertForeIndent { get { return 0; } }
	}
	public class ButtonWindowsXPPainter : ButtonPainter {
		public ButtonWindowsXPPainter(DockElementsWindowsXPPainter painter) : base(painter) { }
		protected override void DrawButtonBackground(DrawApplicationCaptionArgs captionArgs, DockPanelCaptionButton args) { }
		protected override void DrawButtonImage(GraphicsCache cache, DockPanelCaptionButton args, int imageIndex) {
			WXPPainterArgs wa = GetWXPPainterArgs(imageIndex, args.ButtonState);
			wa.Bounds = args.Bounds;
			if(ShouldRotateImage(imageIndex))
				DrawRotatedImage(wa, cache.Graphics, imageIndex);
			else
				WXPPainter.Default.DrawTheme(wa, cache.Graphics, null);
		}
		protected virtual WXPPainterArgs GetWXPPainterArgs(int imageIndex, ObjectState buttonState) {
			WXPPainterArgs wa = new WXPPainterArgs("window", -1, -1);
			switch(imageIndex) {
				case CloseIndex:
				case CloseHotIndex:
					wa.PartId = XPConstants.WP_SMALLCLOSEBUTTON;
					wa.StateId = GetCloseButtonStateId(buttonState);
					break;
				case MaximizeIndex:
				case MaximizeHotIndex:
					wa.PartId = XPConstants.WP_MAXBUTTON;
					wa.StateId = GetMaxButtonStateId(buttonState);
					break;
				case MinimizeIndex:
				case MinimizeHotIndex:
					wa.PartId = XPConstants.WP_RESTOREBUTTON;
					wa.StateId = GetMinButtonStateId(buttonState);
					break;
				case HideIndex:
				case HideHotIndex:
				case PinDownIndex:
				case PinDownHotIndex:
					wa.ThemeName = "explorerbar";
					wa.PartId = 3;
					wa.StateId = GetPinButtonStateId(buttonState);
					break;
			}
			return wa;
		}
		protected virtual void DrawRotatedImage(WXPPainterArgs wa, Graphics destGraphics, int imageIndex) {
			Rectangle destBounds = wa.Bounds;
			Size sz = WXPPainter.Default.GetThemeSize(wa, true);
			if(sz == LayoutConsts.InvalidSize) return;
			wa.Bounds = new Rectangle(Point.Empty, sz);
			Bitmap bm = new Bitmap(wa.Bounds.Width, wa.Bounds.Height);
			Graphics bmGraphics = Graphics.FromImage(bm);
			try {
				WXPPainter.Default.DrawTheme(wa, bmGraphics, null);
				bm.RotateFlip(GetRotateFlipTypeByImageIndex(imageIndex));
				destGraphics.DrawImage(bm, destBounds);
			}
			finally {
				bmGraphics.Dispose();
				bm.Dispose();
			}
		}
		protected virtual RotateFlipType GetRotateFlipTypeByImageIndex(int imageIndex) {
			if(imageIndex == HideIndex || imageIndex == HideHotIndex)
				return RotateFlipType.Rotate270FlipNone;
			return RotateFlipType.RotateNoneFlipNone;
		}
		protected virtual bool ShouldRotateImage(int imageIndex) {
			return (imageIndex == HideIndex || imageIndex == HideHotIndex ||
				imageIndex == PinDownIndex || imageIndex == PinDownHotIndex);
		}
		private int GetMaxButtonStateId(ObjectState state) {
			if((state & ObjectState.Hot) != 0) {
				return ((state & ObjectState.Pressed) != 0 ? XPConstants.MAXBS_PUSHED : XPConstants.MAXBS_HOT);
			}
			return XPConstants.MAXBS_NORMAL;
		}
		private int GetMinButtonStateId(ObjectState state) {
			if((state & ObjectState.Hot) != 0) {
				return ((state & ObjectState.Pressed) != 0 ? XPConstants.RBS_PUSHED : XPConstants.RBS_HOT);
			}
			return XPConstants.RBS_NORMAL;
		}
		private int GetPinButtonStateId(ObjectState state) {
			return ((state & ObjectState.Hot) != 0 ? 2 : 1);
		}
		private int GetCloseButtonStateId(ObjectState state) {
			if((state & ObjectState.Hot) != 0) {
				return ((state & ObjectState.Pressed) != 0 ? XPConstants.CBS_PUSHED : XPConstants.CBS_HOT);
			}
			return XPConstants.CBS_NORMAL;
		}
		public override Size ButtonSize { get { return new Size(14, 14); } }
	}
	public class WindowWindowsXPPainter : WindowPainter {
		public WindowWindowsXPPainter(DockElementsWindowsXPPainter painter) : base(painter) { }
		protected override void DrawWindowCaptionBackground(DrawWindowCaptionArgs e) {
			WXPPainterArgs wa = new WXPPainterArgs(e.ActiveCaption ? "explorerbar" : "tab", e.ActiveCaption ? 5 : XPConstants.TABP_BODY, 0);
			wa.Bounds = e.Bounds;
			WXPPainter.Default.DrawTheme(wa, e.Graphics, null);
			wa = new WXPPainterArgs("button", XPConstants.BP_GROUPBOX, 0);
			wa.Bounds = e.Bounds;
			WXPPainter.Default.DrawTheme(wa, e.Graphics, null);
		}
		public override int CaptionVertIndent { get { return 1; } }
	}
}
