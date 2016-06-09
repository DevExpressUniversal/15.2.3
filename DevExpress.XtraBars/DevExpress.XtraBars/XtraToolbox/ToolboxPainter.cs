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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraToolbox {
	public class ToolboxPainter : BaseControlPainter {
		public ToolboxPainter() { }
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			ToolboxViewInfo vi = (ToolboxViewInfo)info.ViewInfo;
			DrawBackground(info, vi);
			DrawHeader(info, vi);
			DrawExpandButton(info, vi);
			if(vi.IsInAnimation) return;
			DrawMenuButton(info, vi);
			DrawGroups(info, vi);
			DrawSplitter(info, vi);
			DrawItems(info, vi);
			DrawMoreItemsButton(info, vi);
			DrawScrollButtons(info, vi);
			DrawFadeOutBitmap(info, vi);
		}
		protected virtual void DrawMenuButton(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanShowMenuButton()) return;
			DrawElementCore(e, vi.MenuButton);
			DrawMenuButtonArrow(e, vi);
		}
		protected virtual void DrawMenuButtonArrow(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.MenuButton.GetArrowSkinElementInfo());
		}
		protected virtual void DrawMoreItemsButton(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanShowMoreItemsButton()) return;
			DrawElementCore(e, vi.MoreItemsButton);
		}
		protected virtual void DrawScrollButtons(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanShowMinimizedScrollButtons()) return;
			DrawElementCore(e, vi.ScrollButtonUp);
			DrawElementCore(e, vi.ScrollButtonDown);
		}
		protected virtual void DrawBackground(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			e.Cache.FillRectangle(new SolidBrush(vi.GetBackColor()), vi.Bounds);
		}
		protected virtual void DrawHeader(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(vi.Rects.HeaderRect.Height == 0) return;
			if(vi.LookAndFeel.UseDefaultLookAndFeel) {
				SkinElementInfo caption = new SkinElementInfo(NavPaneSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinCaption], vi.Rects.HeaderRect);
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, caption);
			}
			DrawHeaderCaption(e, vi);
		}
		protected virtual void DrawHeaderCaption(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanShowHeaderCaption()) return;
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, vi.Toolbox.Appearance.Toolbox, vi.PaintAppearance);
			appearance.DrawString(e.Cache, vi.Toolbox.Caption, vi.Rects.HeaderCaptionRect);
		}
		protected virtual void DrawExpandButton(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanShowExpandButton()) return;
			DrawElementCore(e, vi.ExpandButton);
		}
		protected virtual void DrawSplitter(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanDrawSplitter()) return;
			SkinElementInfo elem = new SkinElementInfo(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabelLine], vi.CalcSplitterBounds());
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elem);
		}
		protected virtual void DrawSeparator(ControlGraphicsInfoArgs e, ToolboxViewInfo vi, ToolboxItemInfo itemInfo) {
			if(!itemInfo.Item.BeginGroup) return;
			DrawSeparatorLine(e, vi, itemInfo);
			DrawSeparatorText(e, vi, itemInfo);
		}
		protected virtual void DrawSeparatorText(ControlGraphicsInfoArgs e, ToolboxViewInfo vi, ToolboxItemInfo itemInfo) {
			if(!vi.CanDrawSeparatorText()) return;
			Rectangle textBounds = vi.CalcSeparatorTextBounds(itemInfo);
			vi.PaintAppearance.DrawString(e.Cache, itemInfo.Item.BeginGroupCaption, textBounds);
		}
		protected virtual void DrawSeparatorLine(ControlGraphicsInfoArgs e, ToolboxViewInfo vi, ToolboxItemInfo itemInfo) {
			if(!vi.CanDrawSeparatorLine(itemInfo.Item)) return;
			Rectangle bounds = vi.CalcSeparatorLineBounds(itemInfo);
			SkinElementInfo element = new SkinElementInfo(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabelLine], bounds);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, element);
		}
		protected virtual void DrawFadeOutBitmap(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(vi.PrevToolboxBitmap == null || vi.PrevToolboxBitmapOpacity == 0) return;
			Rectangle rect = vi.Rects.ItemsClientRect;
			ImageAttributes attr = ToolboxHelper.GetImageAttributes(vi.PrevToolboxBitmapOpacity);
			e.Graphics.DrawImage(vi.PrevToolboxBitmap, rect, rect.X, rect.Y, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
		}
		protected virtual void DrawGroups(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanDrawGroups()) return;
			GraphicsClipState clip = e.Cache.ClipInfo.SaveAndSetClip(vi.Rects.GroupsContentRect);
			try {
				foreach(ToolboxGroupInfo groupInfo in vi.Groups.Values) {
					DrawGroup(e, groupInfo);
					DrawDesignTimeElementSelection(e, groupInfo);
				}
			}
			finally {
				e.Cache.ClipInfo.RestoreClip(clip);
			}
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs e, ToolboxViewInfo vi) {
			if(!vi.CanDrawItems()) return;
			GraphicsClipState clip = e.Cache.ClipInfo.SaveAndSetClip(vi.Rects.ItemsContentRect);
			try {
				foreach(ToolboxItemInfo itemInfo in vi.VisibleItems.Values) {
					DrawSeparator(e, vi, itemInfo);
					DrawItem(e, itemInfo);
					DrawDesignTimeElementSelection(e, itemInfo);
				}
			}
			finally {
				e.Cache.ClipInfo.RestoreClip(clip);
			}
		}
		protected virtual void DrawGroup(ControlGraphicsInfoArgs e, ToolboxGroupInfo info) {
			DrawElementCore(e, info);
		}
		protected virtual void DrawItem(ControlGraphicsInfoArgs e, ToolboxItemInfo info) {
			DrawElementCore(e, info);
		}
		protected virtual void DrawElementCore(ControlGraphicsInfoArgs e, ToolboxElementInfoBase info) {
			ToolboxViewInfo vi = info.ViewInfo;
			vi.ElementPainter.Draw(new ObjectInfoArgs(e.Cache), info);
		}
		protected virtual void DrawDesignTimeElementSelection(ControlGraphicsInfoArgs e, ToolboxElementInfoBase elementInfo) {
			ToolboxViewInfo viewInfo = (ToolboxViewInfo)e.ViewInfo;
			if(!viewInfo.IsDesignTime) return;
			if(!viewInfo.Toolbox.DesignManager.IsComponentSelected(elementInfo.Element)) return;
			viewInfo.Toolbox.DesignManager.DrawSelection(e.Cache, elementInfo.Bounds, elementInfo.PaintAppearance.BackColor);
		}
	}
	public class ToolboxElementPainter : ObjectPainter {
		public void Draw(ObjectInfoArgs e, ToolboxElementInfoBase info) {
			DrawBackground(e, info);
			DrawImage(e, info);
			DrawCaption(e, info);
		}
		protected virtual void DrawBackground(ObjectInfoArgs e, ToolboxElementInfoBase info) {
			if(info.ViewInfo.LookAndFeel.UseDefaultLookAndFeel)
				DrawObject(e.Cache, SkinElementPainter.Default, info.GetSkinElementInfo());
			else {
				AppearanceObject appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, info.PaintAppearance, info.ViewInfo.GetBaseElementAppearance(info.State, info.Element));
				appearance.DrawBackground(e.Cache, info.Bounds);
			}
		}
		protected virtual void DrawImage(ObjectInfoArgs e, ToolboxElementInfoBase info) {
			if(!info.HasImage) return;
			DrawImageCore(e, info.Element.Image, info.ImageBounds);
		}
		protected virtual void DrawImageCore(ObjectInfoArgs e, Image img, Rectangle bounds) {
			e.Paint.DrawImage(e.Graphics, img, bounds);
		}
		protected virtual void DrawCaption(ObjectInfoArgs e, ToolboxElementInfoBase info) {
			if(!info.HasCaption) return;
			if(info.WrappedText == null) DrawCaptionCore(e, info, info.Element.Caption, info.CaptionBounds);
			else DrawWrappedCaption(e, info);
		}
		protected virtual void DrawCaptionCore(ObjectInfoArgs e, ToolboxElementInfoBase info, string text, Rectangle bounds) {
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, info.PaintAppearance, info.ViewInfo.GetBaseElementAppearance(info.State, info.Element));
			appearance.DrawString(e.Cache, text, bounds);
		}
		protected virtual void DrawWrappedCaption(ObjectInfoArgs e, ToolboxElementInfoBase info) {
			Rectangle rect = info.CaptionBounds;
			rect.Height = info.CaptionBounds.Height / info.WrappedText.Length;
			for(int i = 0; i < info.WrappedText.Length; i++) {
				DrawCaptionCore(e, info, info.WrappedText[i], rect);
				rect.Offset(0, rect.Height);
			}
		}
	}
}
