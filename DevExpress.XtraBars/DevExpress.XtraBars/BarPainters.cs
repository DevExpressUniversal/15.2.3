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
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars.Painters {
	public class BarBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			BarBackgroundObjectInfoArgs info = e as BarBackgroundObjectInfoArgs;
			if(info.Painter == null || info.ViewInfo == null) return;
			info.Painter.DrawBackgroundSurface(new GraphicsInfoArgs(e.Cache, e.Bounds), info.ViewInfo, info.SourceInfo);
		}
	}
	public class BarBackgroundObjectInfoArgs : ObjectInfoArgs {
		BarControlViewInfo viewInfo;
		object sourceInfo;
		BarPainter painter;
		public BarBackgroundObjectInfoArgs(BarPainter painter, BarControlViewInfo viewInfo, object sourceInfo) {
			this.painter = painter;
			this.viewInfo = viewInfo;
			this.sourceInfo = sourceInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public BarControlViewInfo ViewInfo { get { return viewInfo; } }
		public object SourceInfo { get { return sourceInfo; } }
		public BarPainter Painter { get { return painter; } }
	}
	public class BarSubmenuBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			BarSubmenuBackgroundObjectInfoArgs info = e as BarSubmenuBackgroundObjectInfoArgs;
			if(info.Painter == null || info.ViewInfo == null) return;
			info.Painter.DrawMenuBackground(new GraphicsInfoArgs(e.Cache, e.Bounds), info.ViewInfo);
		}
	}
	public class BarSubmenuBackgroundObjectInfoArgs : ObjectInfoArgs {
		CustomSubMenuBarControlViewInfo viewInfo;
		BarSubMenuPainter painter;
		public BarSubmenuBackgroundObjectInfoArgs(BarSubMenuPainter painter, CustomSubMenuBarControlViewInfo viewInfo) {
			this.painter = painter;
			this.viewInfo = viewInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public CustomSubMenuBarControlViewInfo ViewInfo { get { return viewInfo; } }
		public BarSubMenuPainter Painter { get { return painter; } }
	}
	public class BarLinkObjectPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			BarLinkObjectInfoArgs linkInfo = e as BarLinkObjectInfoArgs;
			linkInfo.LinkInfo.Painter.Draw(new GraphicsInfoArgs(e.Cache, e.Bounds), linkInfo.LinkInfo, null);
		}
	}
	public class BarLinkObjectInfoArgs : ObjectInfoArgs {
		BarLinkViewInfo linkInfo;
		public BarLinkObjectInfoArgs(BarLinkViewInfo linkInfo) {
			this.linkInfo = linkInfo;
			this.Bounds = linkInfo.Bounds;
			if(!linkInfo.IsLinkInMenu) {
				foreach(RectInfo info in linkInfo.Rects.ExtRectangles) {
					if(info.Type == RectInfoType.HorzSeparator || info.Type == RectInfoType.VertSeparator)
						this.Bounds = Rectangle.Union(this.Bounds, info.Rect);
				}
			}
		}
		public BarLinkViewInfo LinkInfo { get { return linkInfo; } }
	}
	public class CustomPainter {
		BarManagerPaintStyle paintStyle;
		public CustomPainter(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
		}
		public virtual BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public BarDrawParameters DrawParameters { get { return PaintStyle.DrawParameters; } }
		public PrimitivesPainter PPainter { get { return PaintStyle.PrimitivesPainter; } }
		public static bool IsNeedDraw(PaintEventArgs e, Rectangle r) {
			if(e.ClipRectangle.IsEmpty) return true;
			if(e.ClipRectangle.IntersectsWith(r)) return true;
			return false;
		}
		protected BarManager GetManager(CustomViewInfo info) {
			return info.Manager;
		}
		public virtual void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
		}
		protected internal virtual bool DrawEditorBackground(Control ownerEdit, CustomControlViewInfo ViewInfo, GraphicsCache cache) {
			return false;	
		}
	}
	public class BarDockControlPainter : CustomPainter {
		public BarDockControlPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			DockControlViewInfo vi = info as DockControlViewInfo;
			vi.Appearance.Normal.DrawBackground(e.Graphics, e.Cache, vi.Bounds);
			DrawDesignTimeSelection(e, vi);
		}
		protected virtual void DrawDesignTimeSelection(GraphicsInfoArgs e, DockControlViewInfo info) {
			if(!info.Manager.IsDesignMode) return;
			if(info.Manager.Helper.CustomizationManager.GetObjectSelected(info.BarControl)) {
				info.Manager.Helper.CustomizationManager.DrawDesignTimeSelection(e, info.Bounds, 150);
			}
		}
		protected internal override bool DrawEditorBackground(Control ownerEdit, CustomControlViewInfo info, GraphicsCache cache) {
			DockControlViewInfo vi = info as DockControlViewInfo;
			vi.Appearance.Normal.DrawBackground(cache.Graphics, cache, ownerEdit.ClientRectangle);
			return true;
		}
	}
	public class BarPainter : CustomPainter {
		public BarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public virtual int CalcIndent(BarIndent indent, BarControlViewInfo viewInfo) {
			int res = 0;
			bool isVertical = viewInfo.IsVertical;
			switch(indent) {
				case BarIndent.SizeGrip: 
					if(viewInfo.DrawSizeGrip) res += viewInfo.CalcSizeGripSize().Width; 
					break;
				case BarIndent.DragBorder: 
					if(viewInfo.DrawDragBorder) res += DrawParameters.Constants.DragBorderSize; 
					break;
				case BarIndent.Left : res += (isVertical ? viewInfo.VertIndent : viewInfo.HorzIndent); break;
				case BarIndent.Right : res += (isVertical ? viewInfo.VertIndent  : viewInfo.HorzIndent); break;
				case BarIndent.Top : res += (isVertical ? viewInfo.HorzIndent : viewInfo.VertIndent); break;
				case BarIndent.Bottom :res += (isVertical ? viewInfo.HorzIndent : viewInfo.VertIndent); break;
			}
			return res;
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			BarControlViewInfo vi = (BarControlViewInfo)info;
			DrawBackground(e, vi, sourceInfo);
			foreach(BarControlRowViewInfo br in vi.Rows) {
				foreach(BarLinkViewInfo item in br.Links) {
					DrawLink(e, vi, item);
				}
			}
			DrawDesignTimeSelection(e, vi);
		}
		protected virtual void DrawLink(GraphicsInfoArgs e, BarControlViewInfo viewInfo, BarLinkViewInfo item) {
			XtraAnimator.Current.DrawAnimationHelper(e.Cache, viewInfo.BarControl, item,
				new BarLinkObjectPainter(), new BarLinkObjectInfoArgs(item),
				new DrawTextInvoker(DrawLinkText), item);
		}
		void DrawLinkText(GraphicsCache cache, object info) {
			BarLinkViewInfo item = (BarLinkViewInfo)info;
			item.Painter.DrawLinkTextOnly(new BarLinkPaintArgs(cache, item, null));
		}
		protected virtual void DrawDesignTimeSelection(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(!info.Manager.IsDesignMode) return;
			if(info.Manager.Helper.CustomizationManager.GetObjectSelected(info.Bar)) {
				info.Manager.Helper.CustomizationManager.DrawDesignTimeSelection(e, info.Bounds, 100);
			}
		}
		protected void DrawSizeGrip(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(info.SizeGrip.IsEmpty) return;
			SizeGripObjectInfoArgs args = new SizeGripObjectInfoArgs(info.SizeGrip);
			args.SetAppearance(info.Appearance.Normal);
			args.Appearance.BackColor = Color.Transparent;
			args.Appearance.BackColor2 = Color.Transparent;
			ObjectPainter.DrawObject(e.Cache, info.SizeGripPainter, args);
		}
		const int DragBorderRealWidth = 3;
		protected virtual void DrawDragBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(!info.DragBorder.IsEmpty) {
				int end = !info.IsVertical ? info.DragBorder.Height : info.DragBorder.Width;
				Brush brush;
				Rectangle r;
				for(int n = 0; n < end; n++) {
					brush = SystemBrushes.ControlDark;
					Brush backBrush = info.Appearance.Normal.GetBackBrush(e.Cache);
					if(n < 3 || end - n < 3 || (n % 2) == 1) continue; 
					r = info.DragBorder;
					if(info.IsVertical) {
						r.Width = 1;
						r.X += n;
						r.Height = (info.DragBorder.Height - DragBorderRealWidth) / 2;
						r.Y = r.Bottom + DragBorderRealWidth;
						r.Height = DragBorderRealWidth;
						r.Y -= r.Height;
					} else {
						r.Y += n;
						r.Height = 1;
						r.Width = (info.DragBorder.Width - DragBorderRealWidth) / 2;
						r.X = r.Right + DragBorderRealWidth;
						r.Width = DragBorderRealWidth;
						r.X -= r.Width;
					}
					PaintHelper.FillRectangle(e.Graphics, brush, r);
				}
			}
		}
		public virtual void DrawBackgroundSurface(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			if(info.Bar == null || (info.Bar.OptionsBar.DrawBorder && !info.Bar.IsFloating))
				DrawBackgroundSurfaceCore(e, info, sourceInfo);
		}
		public virtual void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			info.Appearance.Normal.DrawBackground(e.Graphics, e.Cache, info.Bounds);
		}
		public virtual void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
			Region linksReg = info.GetBarLinksRegion(null);
			if(!info.DragBorder.IsEmpty) {
				DrawDragBorder(e, info);
				if(linksReg != null) linksReg.Exclude(info.DragBorder);
			}
			if(!info.SizeGrip.IsEmpty) {
				DrawSizeGrip(e, info);
				if(linksReg != null) linksReg.Exclude(info.SizeGrip);
			}
			if(linksReg != null) {
				linksReg.Dispose();
			}
		}
	}
	public class FloatingBarPainter : BarPainter { 
		public FloatingBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
	}
	public class PopupControlContainerBarPainter : BarSubMenuPainter {
		public PopupControlContainerBarPainter(BarManagerPaintStyle paintStyle)
			: base(paintStyle) {
			this.sizeGripPainter = new SizeGripObjectPainter();
		}
		SizeGripObjectPainter sizeGripPainter;
		public SizeGripObjectPainter SizeGripPainer { get { return sizeGripPainter; } }
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			PopupContainerControlViewInfo vi = info as PopupContainerControlViewInfo;
			if(vi == null) {
				base.Draw(e, info, sourceInfo);
				return;
			}
			DrawBackground(e, vi);
			DrawNavigationBackground(e, vi);
			DrawNavigationItems(e, vi);
			if(vi.ShowSizeGrip)
				DrawSizeGrip(e, vi);
		}
		protected virtual void DrawSizeGrip(GraphicsInfoArgs e, PopupContainerControlViewInfo vi) {
			vi.SizeGripInfo.SetAppearance(vi.Appearance.Normal);
			vi.SizeGripInfo.Cache = e.Cache;
			SizeGripPainer.DrawObject(vi.SizeGripInfo);
		}
		protected virtual void DrawBackground(GraphicsInfoArgs e, PopupContainerControlViewInfo vi) {
			PaintHelper.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(vi.BarControl.FindForm().BackColor), vi.ButtonsRect);
		}
	}
	public class BarSubMenuPainter : CustomPainter {
		public BarSubMenuPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public virtual void DrawMenuBackground(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			vi.Appearance.Normal.DrawBackground(e.Graphics, e.Cache, vi.Bounds);
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			CustomSubMenuBarControlViewInfo vi = (CustomSubMenuBarControlViewInfo)info;
			if(vi == null) return;
			DrawMenuBackground(e, vi);
			DrawMenuCaption(e, vi);
			DrawNavigationBackground(e, vi);
			DrawNavigationItems(e, vi);
			foreach(SubMenuLinkInfo linfo in vi.BarLinksViewInfo) {
				DrawLink(e, vi, linfo.LinkInfo);
			}
			if(!vi.MenuBar.IsEmpty) {
				BarCustomDrawEventArgs mb = new BarCustomDrawEventArgs(e.Graphics, vi.MenuBar);
				CustomPopupBarControl cpb = vi.BarControl as CustomPopupBarControl;
				if(cpb != null) cpb.RaisePaintMenuBar(mb);
				if(!mb.Handled)
					vi.AppearanceMenu.MenuBar.DrawBackground(e.Graphics, e.Cache, vi.MenuBar);
			}
		}
		protected virtual void DrawNavigationBackground(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
		}
		protected virtual void DrawNavigationItems(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			if(!vi.ShowNavigationHeader)
				return;
			if(vi.NavigationItems.Count > 1) {
				DrawBackArrow(e, vi);
			}
			foreach(MenuNavigationItem item in vi.NavigationItems) {
				DrawNavigationItems(e, vi, item);
			}
		}
		protected virtual void DrawNavigationItems(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi, MenuNavigationItem item) {
			vi.GetNavigationItemAppearance(item).DrawString(e.Cache, item.Text, item.Bounds);
		}
		protected virtual void DrawBackArrow(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
		}
		protected virtual void DrawLink(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo viewInfo, BarLinkViewInfo item) {
			if(!e.Cache.IsNeedDrawRect(item.Bounds)) return;
			if(XtraAnimator.Current.DrawFrame(e.Cache, viewInfo.BarControl, item)) return;
			item.Painter.Draw(e, item, viewInfo);
			XtraAnimator.Current.DrawPostFrame(e.Cache, viewInfo.BarControl, item);
		}
		public virtual Size CalcCaptionSize(Graphics g, CustomSubMenuBarControlViewInfo vi, string caption) {
			Size res = vi.AppearanceMenu.MenuCaption.CalcTextSize(g, caption, 0).ToSize();
			res.Height += 8; res.Width += 10;
			return res;
		}
		public virtual void DrawMenuCaption(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			if(vi.MenuCaptionBounds.IsEmpty) return;
			Rectangle r = vi.MenuCaptionBounds;
			vi.AppearanceMenu.MenuCaption.DrawBackground(e.Cache, r);
			r.Y = r.Bottom - 2; r.Height = 1;
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(vi.AppearanceMenu.MenuCaption.BorderColor), r);
			r = vi.MenuCaptionBounds;
			r.Inflate(-5, -3);
			vi.AppearanceMenu.MenuCaption.DrawString(e.Cache, vi.MenuCaption, r);
		}
	}
	public enum BarLinkEmptyBorder { Down, Up, Left, Right, None, All};
}
