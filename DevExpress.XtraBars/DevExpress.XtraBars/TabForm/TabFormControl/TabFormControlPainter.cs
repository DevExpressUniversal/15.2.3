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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Paint;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabFormControlPainter : TabFormControlPainterBase {
		public TabFormControlPainter(BarManagerPaintStyle style)
			: base(style) {
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			TabFormControlViewInfo vi = (TabFormControlViewInfo)info;
			DrawFormCaption(e.Cache, vi);
			base.Draw(e, info, sourceInfo);
		}
		public virtual void DrawFormCaption(GraphicsCache cache, TabFormControlViewInfo vi) {
			if(vi.Owner.GetFormPainter() == null)
				return;
			int dx = -vi.Owner.GetFormPainter().Margins.Left;
			if(vi.Owner.TabForm.WindowState == FormWindowState.Maximized) {
				dx -= vi.Owner.GetFormPainter().GetZoomedMarginsCore().Left;
			}
			if(vi.IsRightToLeft) {
				cache.Graphics.TranslateTransform(-dx + vi.Owner.Width, 0);
				cache.Graphics.ScaleTransform(-1f, 1f);
			}
			else {
				cache.Graphics.TranslateTransform(dx, 0);
			}
			vi.Owner.TabForm.GetPainter().DrawCaption(cache);
			cache.ResetMatrix();
			cache.Graphics.ResetTransform();
		}
		public override void DrawPageText(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			TabFormPainter formPainter = ((TabFormControl)ctrl).GetFormPainter();
			if(formPainter == null) {
				base.DrawPageText(cache, ctrl, pageInfo);
				return;
			}
			pageInfo.PaintAppearance.DrawString(cache, pageInfo.Page.Text, pageInfo.GetTextBounds());
		}
	}
	public class TabFormControlPainterBase : CustomPainter {
		public TabFormControlPainterBase(BarManagerPaintStyle style)
			: base(style) {
		}
		public override void Draw(GraphicsInfoArgs e, XtraBars.ViewInfo.CustomViewInfo info, object sourceInfo) {
			TabFormControlViewInfoBase vi = (TabFormControlViewInfoBase)info;
			DrawLinks(e.Cache, vi);
			DrawPages(e.Cache, vi);
			if(vi.Owner.IsDesignMode)
				DrawContainerRects(e.Cache, vi);
		}
		public virtual void DrawLinks(GraphicsCache cache, TabFormControlViewInfoBase info) {
			info.LinkInfoProvider.ForEachLinkInfo(linkInfo => { DrawLink(cache, info, linkInfo); });
		}
		public virtual void DrawLink(GraphicsCache cache, TabFormControlViewInfoBase info, BarLinkViewInfo linkInfo) {
			if(linkInfo.Bounds == Rectangle.Empty)
				return;
			XtraAnimator.Current.DrawAnimationHelper(cache, info.Owner, linkInfo,
				new BarLinkObjectPainter(), new BarLinkObjectInfoArgs(linkInfo),
				new DrawTextInvoker(DrawLinkText), linkInfo);
		}
		void DrawLinkText(GraphicsCache cache, object info) {
			BarLinkViewInfo item = (BarLinkViewInfo)info;
			item.Painter.DrawLinkTextOnly(new BarLinkPaintArgs(cache, item, null));
		}
		public virtual void DrawContainerRects(GraphicsCache cache, TabFormControlViewInfoBase viewInfo) {
			using(Pen pen = new Pen(Color.Red)) {
				pen.DashPattern = new float[] { 5.0f, 5.0f };
				DrawContainerRectCore(cache, viewInfo.LinkInfoProvider.TitleItemLinks, pen);
				DrawContainerRectCore(cache, viewInfo.LinkInfoProvider.TabLeftItemLinks, pen);
				DrawContainerRectCore(cache, viewInfo.LinkInfoProvider.TabRightItemLinks, pen);
			}
		}
		public virtual void DrawContainerRectCore(GraphicsCache cache, TabFormLinkInfoCollection col, Pen pen) {
			Rectangle rect = col.Bounds;
			rect.Width--;
			rect.Height--;
			cache.Graphics.DrawRectangle(pen, rect);
		}
		public virtual void DrawPages(GraphicsCache cache, TabFormControlViewInfoBase vi) {
			TabFormControlBase ctrl = vi.Owner;
			if(vi.ShouldShowAddPage()) DrawAddPage(cache, ctrl, vi.AddPageInfo);
			TabFormPageViewInfo selectedPageInfo = null;
			foreach(TabFormPageViewInfo pageInfo in vi.PageInfos) {
				if(pageInfo.Page == ctrl.SelectedPage) {
					selectedPageInfo = pageInfo;
					continue;
				}
				DrawPage(cache, ctrl, pageInfo);
			}
			DrawSeparator(cache, vi);
			if(selectedPageInfo != null) {
				DrawPage(cache, ctrl, selectedPageInfo);
			}
		}
		public virtual void DrawSeparator(GraphicsCache cache, TabFormControlViewInfoBase vi) {
			if(vi.Owner.LookAndFeel == null) return;
			Color clr = FormSkins.GetSkin(vi.Owner.LookAndFeel.ActiveLookAndFeel).Colors.GetColor("TabFormSeparatorColor", Color.Empty);
			if(clr != Color.Empty)
				cache.Graphics.DrawLine(cache.GetPen(clr), new Point(0, vi.Bounds.Height - 1), new Point(vi.Bounds.Width, vi.Bounds.Height - 1));
		}
		public virtual void DrawAddPage(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			SkinElement elem = ctrl.ViewInfo.GetAddPageSkinElement();
			if(elem == null) return;
			DrawPageBackgroungCore(cache, ctrl, pageInfo, elem);
			if(string.Equals(elem.ElementName, EditorsSkins.SkinNavigatorButton)) {
				Image glyph = EditorsSkins.GetSkin(ctrl.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinNavigator].Image.GetImages().Images[6];
				Point glyphLoc = new Point(pageInfo.CurrentBounds.X + elem.ContentMargins.Left, pageInfo.CurrentBounds.Y + elem.ContentMargins.Top);
				cache.Graphics.DrawImage(glyph, new Rectangle(glyphLoc, glyph.Size));
			}
		}
		public virtual void DrawPage(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			DrawPageBackground(cache, ctrl, pageInfo);
			DrawPageText(cache, ctrl, pageInfo);
			DrawPageImage(cache, ctrl, pageInfo);
			DrawPageCloseButton(cache, ctrl, pageInfo);
		}
		public virtual void DrawPageCloseButton(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			if(!pageInfo.Page.ShouldShowCloseButton()) return;
			Image img = ctrl.ViewInfo.CloseButton;
			int imageIndex = 0;
			if(pageInfo.CloseButtonState == ObjectState.Hot) imageIndex = 1;
			else if(pageInfo.CloseButtonState == ObjectState.Pressed) imageIndex = 2;
			else if(pageInfo.CloseButtonState == ObjectState.Disabled) imageIndex = 3;
			Size imageSize = ctrl.ViewInfo.GetCloseButtonSize();
			ImageAttributes attr = new ImageAttributes();
			attr.SetColorMatrix(DevExpress.XtraBars.Navigation.ColorizedImagePaintHelper.CreateColorMatrix(pageInfo.PaintAppearance.ForeColor));
			cache.Graphics.DrawImage(img, pageInfo.GetCloseButtonBounds(), 0, imageSize.Height * imageIndex, imageSize.Width, imageSize.Height, GraphicsUnit.Pixel, attr);
		}
		protected internal virtual void DrawPageImage(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			Image img = pageInfo.Page.GetImage();
			if(img == null) return;
			Rectangle imageBounds = pageInfo.GetImageBounds();
			if(pageInfo.Page.GetEnabled()) {
				if(pageInfo.Page.GetAllowGlyphSkinning()) {
					ImageAttributes attr = ImageColorizer.GetColoredAttributes(pageInfo.PaintAppearance.ForeColor);
					cache.Graphics.DrawImage(img, imageBounds, 0, 0, ctrl.ViewInfo.ImageSize.Width, ctrl.ViewInfo.ImageSize.Height, GraphicsUnit.Pixel, attr);
				}
				else cache.Graphics.DrawImage(img, imageBounds);
			}
			else cache.Graphics.DrawImage(img, imageBounds, 0, 0, imageBounds.Width, imageBounds.Height, GraphicsUnit.Pixel, XPaint.DisabledImageAttr);
		}
		public virtual void DrawPageBackground(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			SkinElement elem = ctrl.ViewInfo.GetPageSkinElement();
			if(elem == null) return;
			DrawPageBackgroungCore(cache, ctrl, pageInfo, elem);
		}
		protected void DrawPageBackgroungCore(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo, SkinElement elem) {
			SkinElementInfo info = new SkinElementInfo(elem, pageInfo.CurrentBounds);
			info.RightToLeft = ctrl.IsRightToLeft;
			if(pageInfo.Page == ctrl.SelectedPage) info.ImageIndex = 2;
			else if(pageInfo == ctrl.Handler.HotPage) info.ImageIndex = 1;
			else info.ImageIndex = 0;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		public virtual void DrawPageText(GraphicsCache cache, TabFormControlBase ctrl, TabFormPageViewInfo pageInfo) {
			cache.Graphics.DrawString(pageInfo.Page.Text, ctrl.Font, Brushes.Black, pageInfo.GetTextBounds());
		}
	}
	public class TabFormControlBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
		}
	}
	public class TabFormControlBackgroundObjectInfoArgs : ObjectInfoArgs {
		TabFormControlViewInfoBase viewInfo;
		TabFormControlPainterBase painter;
		public TabFormControlBackgroundObjectInfoArgs(TabFormControlPainterBase painter, TabFormControlViewInfoBase viewInfo) {
			this.painter = painter;
			this.viewInfo = viewInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public TabFormControlViewInfoBase ViewInfo { get { return viewInfo; } }
		public TabFormControlPainterBase Painter { get { return painter; } }
	}
}
