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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors.ButtonPanel;
using System.Drawing.Printing;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardLayoutControlItemPainter : BaseLayoutItemPainter, IPanelControlOwner {
		protected override ObjectPainter CreateBorderPainter(BaseViewInfo e) {
				return new DashboardGroupObjectPainter(this, e.Owner.PaintStyle.LookAndFeel);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			BaseViewInfo vi = e as BaseViewInfo;
			if(vi != null) {
				DashboardLayoutControlItemBase item = vi.Owner as DashboardLayoutControlItemBase;
				if(item != null)
					item.RaisePaintEvent(new System.Windows.Forms.PaintEventArgs(e.Graphics, e.Bounds));
			}
		}
		ObjectPainter backgroundPainter = new DashboardLayoutControlItemBackgroundPainter();
		protected override void DrawBackground(BaseLayoutItemViewInfo e) {backgroundPainter.DrawObject(e);}
		public override void DrawControlsArea(BaseLayoutItemViewInfo e) {
			DashboardLayoutControlItemBase lci = e.Owner as DashboardLayoutControlItemBase;
			if(!lci.ContentVisible) return;
			ObjectPainter.DrawObject(e.Cache, GetBorderPainter(e), e.BorderInfo);
		}
		#region DoNotDraw
		protected override void DrawTextArea(BaseLayoutItemViewInfo e) {
		}
		protected override void DrawSelection(BaseLayoutItemViewInfo e) {
		}
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
		}
		protected override void DrawBorder(BaseViewInfo e) {
		}
		#endregion
		#region IPanelControlOwner Members
		Color IPanelControlOwner.GetForeColor() {
			return Color.Black;
		}
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
		}
		#endregion
	}
	public class DashboardBorderPainter : ObjectPainter {
		public DashboardBorderPainter(UserLookAndFeel lookAndFeel) {
			lookAndFeelCore = lookAndFeel;
		}
		UserLookAndFeel lookAndFeelCore;
		SkinElementInfo backgroundSkinInfo = null;
		protected virtual SkinElementInfo GetSkinLayoutItemBackground(ObjectInfoArgs e) {
			if(backgroundSkinInfo == null) {
				Skin skin = DashboardSkins.GetSkin(lookAndFeelCore);
				if(skin != null) {
					SkinElement sel = skin[DashboardSkins.SkinDashboardItemBackground];
					if(sel != null) {
						backgroundSkinInfo = new SkinElementInfo(sel, Rectangle.Empty);
						return backgroundSkinInfo;
					}
				}
			}
			return backgroundSkinInfo;
		}
		protected void PatchImageIndex(SkinElementInfo info, ObjectInfoArgs e) {
			if(e.State == ObjectState.Selected) {
				info.ImageIndex = 2;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			SkinElementInfo info = GetSkinLayoutItemBackground(e);
			if((info == null) || (e == null)) return;
			info.Bounds = e.Bounds;
			info.ImageIndex = (int)e.State;
			PatchImageIndex(info, e);
			if(info.ImageIndex >= 0) ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class DashboardLayoutControlItemBackgroundPainter : LayoutControlItemSkinPainter {
	   readonly Padding defaultSpacing = new Padding(5);	   
		SkinElementInfo backgroundSkinInfo = null;
		protected override SkinElementInfo GetSkinLayoutItemBackground(BaseViewInfo e) {
			if (backgroundSkinInfo == null) {
				if (e.Owner == null || e.OwnerILayoutControl == null)
					return null;
				Skin skin = DashboardSkins.GetSkin(e.OwnerILayoutControl.LookAndFeel);
				if (skin != null) {
					SkinElement sel = skin[DashboardSkins.SkinDashboardItemBackground];
					if (sel != null) {
						backgroundSkinInfo = new SkinElementInfo(sel, Rectangle.Empty);
						return backgroundSkinInfo;
					}
				}
				backgroundSkinInfo = base.GetSkinLayoutItemBackground(e);
			}
			return backgroundSkinInfo;
		}
		public override Padding GetSpacing(BaseViewInfo e) {
			SkinElementInfo si = GetSkinLayoutItemBackground(e);
			if(si != null && si.Element != null) {
				int skinSpacing = si.Element.Properties.GetInteger("Spacing");
				return new Padding(skinSpacing);
			}
			return defaultSpacing;
		}
		protected override Rectangle GetPaintBounds(BaseViewInfo e) {
			DashboardLayoutControlItemViewInfo vi = e.Owner.ViewInfo as DashboardLayoutControlItemViewInfo;
			DashboardLayoutControlGroupViewInfo vig = e.Owner.ViewInfo as DashboardLayoutControlGroupViewInfo;
			if(vi == null && vig == null) return Rectangle.Empty;
			Rectangle result = vi != null ? vi.BackgroundAreaRelativeToControl : vig.BoundsRelativeToControl;
			if(vi != null && e.Owner is DashboardLayoutControlItemBase && (e.Owner as DashboardLayoutControlItemBase).IsLocked) {
				result = vi.BoundsRelativeToControl;
			}
			Size borderSize = new Size(0, 0);
			SkinElementInfo si = GetSkinLayoutItemBackground(e);
			if(si != null && si.Element != null && vig == null) {
				int skinBorderSize = si.Element.Properties.GetInteger("BorderSize");
				borderSize = new Size(skinBorderSize, skinBorderSize);
			}
			result.Inflate(borderSize);
			return result;
		}
		protected override void PatchImageIndex(SkinElementInfo info, BaseLayoutItemViewInfo e) {
			if (e.State == ObjectState.Selected) {
				info.ImageIndex = 2;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo bliv = e as BaseLayoutItemViewInfo;
			if (bliv.Owner.Selected) bliv.State = ObjectState.Selected;
			base.DrawObject(e);
		}
		#region DoNotDraw
		protected override void DrawSelection(BaseLayoutItemViewInfo e) {
		}
		protected override void DrawTextArea(BaseLayoutItemViewInfo e) {
		}
		#endregion
	}
	public class DashboardGroupObjectInfoArgs : GroupObjectInfoArgs {
		public bool AllowImageColorization { get; set; }
	}
	public class DashboardItemCaptionButtonInfoArgs : StyleObjectInfoArgs {
		public DashboardItemCaptionButtonInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState state, Image glyph)
			: base(cache, bounds, appearance, state) {
			Glyph = glyph;
		}
		public Image Glyph { get; private set; }
	}
	public class DashboardGroupObjectPainter : SkinGroupObjectPainter {
		protected override System.Drawing.Printing.Margins GetCorrectMarginsByButtonsPanelLocation(GroupObjectInfoArgs info, Rectangle caption, System.Drawing.Printing.Margins margins) {
			System.Drawing.Printing.Margins originalMargins = new Margins(margins.Left, margins.Right - 1, margins.Top, margins.Bottom);
			return originalMargins;
		}
		protected override Size CalcCaptionButtonsPanelMinSize(GroupObjectInfoArgs info) {
			if(!info.ShowButtonsPanel)
				return new Size(buttonImageWidth, buttonImageHeight);
			return info.ButtonsPanel.ViewInfo.CalcMinSize(info.Graphics);
		}
		const int buttonImageHeight = 16;
		const int buttonImageWidth = 16;
		static Bitmap ColorBitmap(Image original, Color newColor, float opacity) {
			Bitmap bitmap = new Bitmap(original);
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(opacity >= 0.0f && opacity <= 1.0f ? (int)(color.A * opacity) : 255, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		static float GetOpacityByState(ObjectState state) {
			switch(state) {
				case ObjectState.Disabled:
					return 0.15f;
				case ObjectState.Hot:
					return 1.0f;
				case ObjectState.Normal:
					return 0.65f;
				case ObjectState.Pressed:
					return 0.35f;
				case ObjectState.Selected:
					return 0.80f;
				default:
					return 0.75f;
			}
		}
		public DashboardGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider, string groupPanelSkinElementName)
			: base(owner, provider, groupPanelSkinElementName) {
		}
		public DashboardGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider)
			: base(owner, provider) {
		} 
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info == null) return base.GetBorderPainter(e);
			if (info.BorderStyle == BorderStyles.NoBorder)
				return new SkinGroupEmptyBorderPainter(Provider);
			else if (info.ShowCaption)
				return new DashboardItemTopPainter(this, Provider);
			else
				return new DashboardItemPanelPainter(this, Provider);
		}
		protected override SkinElement GetPanelCaptionSkinElement(GroupObjectInfoArgs info) {
			string name = DashboardSkins.SkinDashboardItemCaptionTop;
			Skin skin = DashboardSkins.GetSkin(Provider);
			SkinElement res = skin != null ? skin[name] : null;
			if(res == null) {
				res = base.GetPanelCaptionSkinElement(info);
			}
			return res;
		}
	}
	public abstract class DashboardItemPainter : SkinGroupBorderPainter {
		abstract protected string SkinElementName { get; }
		protected DashboardItemPainter(SkinGroupObjectPainter groupPainter, ISkinProvider provider)
			: base(groupPainter, provider) {
		}
		protected override SkinElement GetPanelSkinElement(GroupObjectInfoArgs info) {
			Skin skin = DashboardSkins.GetSkin(Provider);			
			SkinElement res = skin != null ? skin[SkinElementName] : null;
			if(res == null) {			 
				return base.GetPanelSkinElement(info);
			}
			return res;																											 
		}
	}
	public class DashboardItemTopPainter : DashboardItemPainter {
		override protected string SkinElementName { get { return DashboardSkins.SkinDashboardItemTop; } }
		public DashboardItemTopPainter(SkinGroupObjectPainter groupPainter, ISkinProvider provider)
			: base(groupPainter, provider) {
		}
	}
	public class DashboardItemPanelPainter : DashboardItemPainter {
		override protected string SkinElementName { get { return DashboardSkins.SkinDashboardItemPanel; } }
		public DashboardItemPanelPainter(SkinGroupObjectPainter groupPainter, ISkinProvider provider)
			: base(groupPainter, provider) {
		}  
	}
	public class DashboardLayoutControlItemInGroupPainter : LayoutControlItemPainter,IPanelControlOwner {
		protected override ObjectPainter CreateBorderPainter(BaseViewInfo e) {
			return new DashboardGroupObjectPainterForItemInGroup(this, e.Owner.PaintStyle.LookAndFeel);
		}
		SkinElementInfo backgroundSkinInfo = null;
		protected virtual SkinElementInfo GetSkinElement(BaseViewInfo e,string skinName) {
			if(backgroundSkinInfo == null) {
				Skin skin = DashboardSkins.GetSkin(e.OwnerILayoutControl.LookAndFeel);
				if(skin != null) {
					SkinElement sel = skin[skinName];
					if(sel != null) {
						backgroundSkinInfo = new SkinElementInfo(sel, Rectangle.Empty);
						return backgroundSkinInfo;
					}
				}
			}
			return backgroundSkinInfo;
		}
		protected void PatchImageIndex(SkinElementInfo info, ObjectInfoArgs e) {
			switch(e.State) {
				case ObjectState.Normal:
					info.ImageIndex = 0;
					break;
				case ObjectState.Hot:
					info.ImageIndex = 1;
					break;
				case ObjectState.Selected:
					info.ImageIndex = 2;
					break;
				default: break;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			if(!(e is LayoutControlItemViewInfo)) return;
			DrawHotOrSelectedState(e as LayoutControlItemViewInfo);
			DrawBorder(e as LayoutControlItemViewInfo);
		}
		private void DrawBorder(LayoutControlItemViewInfo e) {
			if(!e.Owner.Visible) return;
			DashboardLayoutControlItemBase item = e.Owner as DashboardLayoutControlItemBase;
			if(item != null && !item.AllowDrawContentBorder) return;
			Rectangle rect = e.ClientAreaRelativeToControl;
			rect.Inflate(1, 1);
			Skin skin = DashboardSkins.GetSkin(e.Owner.Owner.LookAndFeel);
			Color color = (Color)skin.Properties["ItemInGroupBorderColor"];
			e.Cache.DrawRectangle(new Pen(color),  rect);
		}
		private void DrawHotOrSelectedState(LayoutControlItemViewInfo e) {
			if(!e.Owner.Visible || !((DashboardLayoutControl)(e.Owner.Owner)).AllowSelection) return;
			SkinElementInfo info = GetSkinElement(e as BaseViewInfo, DashboardSkins.SkinDashboardItemBackground);
			PatchImageIndex(info, e);
			Rectangle drawRect = e.ClientAreaRelativeToControl;
			drawRect.Inflate(4, 4);
			info.Bounds = drawRect;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DrawControlsArea(BaseLayoutItemViewInfo e) {
			DashboardLayoutControlItemBase lci = e.Owner as DashboardLayoutControlItemBase;
			if(!lci.ContentVisible) return;
			ObjectPainter.DrawObject(e.Cache, GetBorderPainter(e), e.BorderInfo);
		}
		#region DoNotDraw
		protected override void DrawSelection(BaseLayoutItemViewInfo e) {
		}
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) {
		}
		protected override void DrawBorder(BaseViewInfo e) {
		}
		protected override void DrawTextArea(BaseLayoutItemViewInfo e) { }
		#endregion
		#region IPanelControlOwner Members
		Color IPanelControlOwner.GetForeColor() {
			return Color.Black;
		}
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
		}
		#endregion
	}
	public class DashboardGroupObjectPainterForItemInGroup : DashboardGroupObjectPainter {
		public override int ButtonToBorderDistance { get { return 0; } }
		public DashboardGroupObjectPainterForItemInGroup(IPanelControlOwner owner, ISkinProvider provider, string groupPanelSkinElementName)
			: base(owner, provider, groupPanelSkinElementName) {
		}
		public DashboardGroupObjectPainterForItemInGroup(IPanelControlOwner owner, ISkinProvider provider)
			: base(owner, provider) {
		}
		protected override void CalcContentMargins(GroupObjectInfoArgs info, out int left, out int top, out int right, out int bottom) {
			Skin skin = DashboardSkins.GetSkin(Provider);
			if(skin != null) {
				SkinPaddingEdges padding = skin.Properties["ItemInGroupTitlePadding"] as SkinPaddingEdges;
				if(padding != null) {
					left = padding.Left;
					right = padding.Right;
					top = padding.Top;
					bottom = padding.Bottom;
					return;
				}
			}
			base.CalcContentMargins(info, out left, out top, out right, out bottom);
		}
		#region DoNotDraw
		protected override void DrawCaptionSkinElement(GroupObjectInfoArgs info) {
		}
		protected override void DrawBorder(GroupObjectInfoArgs info) {
		}
		#endregion
	}
	public class DashboardGroupBoxButtonsSkinPainter :GroupBoxButtonsPanelSkinPainter {
		public DashboardGroupBoxButtonsSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new DashboardGroupBoxButtonSkinPainter(Provider);
		}
	}
	public class DashboardGroupBoxButtonSkinPainter :GroupBoxButtonSkinPainter {
		public DashboardGroupBoxButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			SkinElement skinElement = GetPanelCaptionSkinElement(SkinProvider);
			Image image = ColorBitmap(info.Image, skinElement.Color.GetForeColor(), GetOpacityByState(info.State));
			cache.Graphics.DrawImage(image, info.ImageBounds);
		}
		protected SkinElement GetPanelCaptionSkinElement(ISkinProvider Provider) {
			string name = DashboardSkins.SkinDashboardItemCaptionTop;
			Skin skin = DashboardSkins.GetSkin(Provider);
			SkinElement res = skin != null ? skin[name] : null;
			if(res == null) {
				res = GetPanelCaptionSkinElementCore(Provider);
			}
			return res;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		SkinElement GetPanelCaptionSkinElementCore(ISkinProvider Provider) {
			return CommonSkins.GetSkin(Provider)[CommonSkins.SkinGroupPanelCaptionTop];
		}
		static Bitmap ColorBitmap(Image original, Color newColor, float opacity) {
			Bitmap bitmap = new Bitmap(original);
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(opacity >= 0.0f && opacity <= 1.0f ? (int)(color.A * opacity) : 255, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		static float GetOpacityByState(ObjectState state) {
			switch(state) {
				case ObjectState.Disabled:
					return 0.15f;
				case ObjectState.Hot:
					return 1.0f;
				case ObjectState.Normal:
					return 0.65f;
				case ObjectState.Pressed:
					return 0.35f;
				case ObjectState.Selected:
					return 0.80f;
				default:
					return 0.75f;
			}
		}
	}
}
