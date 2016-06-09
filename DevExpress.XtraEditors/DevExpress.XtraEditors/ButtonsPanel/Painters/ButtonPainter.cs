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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
namespace DevExpress.XtraEditors.ButtonPanel {
	public class TabButtonSkinPainter : BaseButtonSkinPainter {
		public TabButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetBackground() {
			return TabSkins.GetSkin(SkinProvider)[TabSkins.SkinTabPageButton];
		}
		protected override Skin GetSkin() {
			return TabSkins.GetSkin(SkinProvider);
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(GetTabButtonsPanelState(info)), info.Bounds);
			elementInfo.State = info.State;
			elementInfo.Cache = cache;
			elementInfo.ImageIndex = CalcImageIndexCore(info.State, elementInfo);
			elementInfo.GlyphIndex = CalcGlyphIndex(info.State, info);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		protected TabButtonsPanelState GetTabButtonsPanelState(BaseButtonInfo info) {
			if(info.Selected) return TabButtonsPanelState.Selected;
			IButtonsPanel panel = null;
			if(info.Button is BaseButton)
				panel = ((BaseButton)info.Button).GetOwner();
			if(panel != null && panel.Owner is XtraTab.ViewInfo.BaseTabPageViewInfo && (panel.Owner as XtraTab.ViewInfo.BaseTabPageViewInfo).IsActiveState && !info.Selected)
				return TabButtonsPanelState.Active;
			return TabButtonsPanelState.Normal;
		}
		protected int CalcGlyphIndex(ObjectState state, BaseButtonInfo info) {
			int offsetIndex = 0;
			if(info.Button is BasePinButton)
				offsetIndex = info.Button.Properties.Checked ? 4 : 8;
			ObjectState tempState = state & (~ObjectState.Selected);
			if(tempState == ObjectState.Disabled) return offsetIndex + 3;
			if((tempState & ObjectState.Pressed) != 0) return offsetIndex + 2;
			if((tempState & ObjectState.Hot) != 0) return offsetIndex + 1;
			return offsetIndex;
		}
		static int ButtonMinSize = 14;
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElementInfo ee = new SkinElementInfo(GetBackground());
			if(ee.Element == null) return Rectangle.Empty;
			Size minSize = ee.Element.Size.MinSize;
			Size imageSize = GetImageMinSize(ee.Element.Image);
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			imageSize = GetImageMinSize(ee.Element.Glyph);
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			minSize.Width = Math.Max(minSize.Width, ButtonMinSize);
			minSize.Height = Math.Max(minSize.Height, ButtonMinSize);
			return new Rectangle(Point.Empty, minSize);
		}
		Size GetImageMinSize(SkinImage image) {
			if(image == null) return Size.Empty;
			Size res = image.GetImageBounds(0).Size;
			if(res.IsEmpty || image.Stretch != SkinImageStretch.NoResize) return Size.Empty;
			return res;
		}
		protected virtual int CalcImageIndexCore(ObjectState state, SkinElementInfo info) {
			if(state == ObjectState.Disabled) return 3;
			if((state & ObjectState.Pressed) != 0) return 2;
			if((state & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		protected override ImageCollection GetGlyphs(IBaseButton Button) {
			return null;
		}
	}
	public class TabButtonDefaultPainter : BaseButtonPainter {
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Hot)
				DrawHot(cache, info);
			if(info.Pressed)
				DrawPressed(cache, info);
			if(info.State == ObjectState.Normal)
				DrawNormal(cache, info);
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			int imageIndex = 0;
			if((info.Button is BasePinButton || info.Button is BaseMaximizeButton)) {
				imageIndex += info.Button.Properties.Checked ? 2 : 0;
			}
			return imageIndex;
		}
		protected virtual void DrawNormal(GraphicsCache cache, BaseButtonInfo info) {
			BBrushes brushes = new BBrushes(cache, info.PaintAppearance);
			Rectangle r = info.Bounds;
			DrawFlatBounds(cache, r, brushes.LightLight, brushes.Dark);
			r.Inflate(-1, -1);
			cache.Paint.FillRectangle(cache.Graphics, brushes.Light, new Rectangle(r.X, r.Y, 1, r.Height));
			cache.Paint.FillRectangle(cache.Graphics, brushes.Light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			r.X++;
			r.Y++;
			r.Width--;
			r.Height--;
			DrawButtonBackground(info, info.PaintAppearance, r);
		}
		public static void DrawFlatBounds(GraphicsCache cache, Rectangle r, Brush light, Brush dark) {
			cache.Paint.FillRectangle(cache.Graphics, light, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			cache.Paint.FillRectangle(cache.Graphics, light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			cache.Paint.FillRectangle(cache.Graphics, dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			cache.Paint.FillRectangle(cache.Graphics, dark, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
		}
		protected virtual void DrawHot(GraphicsCache cache, BaseButtonInfo info) { }
		protected virtual void DrawPressed(GraphicsCache cache, BaseButtonInfo info) { }
		public void DrawBounds(GraphicsCache cache, Rectangle r, Brush light, Brush dark) {
			cache.Paint.FillRectangle(cache.Graphics, light, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			cache.Paint.FillRectangle(cache.Graphics, light, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
			cache.Paint.FillRectangle(cache.Graphics, dark, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			cache.Paint.FillRectangle(cache.Graphics, dark, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
		}
		protected virtual void DrawButtonBackground(ObjectInfoArgs e, AppearanceObject style, Rectangle r) {
			if(style.BackColor == Color.Empty) {
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(DefaultAppearance.BackColor), r);
			}
			else
				style.FillRectangle(e.Cache, r);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 2, 2);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -2, -2);
		}
	}
	public class TabButtonStyle3DPainter : TabButtonDefaultPainter {
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Pressed)
				DrawPressed(cache, info);
			else
				DrawNormal(cache, info);
		}
		protected override void DrawNormal(GraphicsCache cache, BaseButtonInfo info) {
			BBrushes brushes = new BBrushes(cache, info.PaintAppearance);
			Rectangle r = info.Bounds;
			ButtonObjectPainter.DrawBounds(info, r, brushes.LightLight, brushes.DarkDark);
			r.Inflate(-1, -1);
			ButtonObjectPainter.DrawBounds(info, r, brushes.Light, brushes.Dark);
			r.Inflate(-1, -1);
			DrawButtonBackground(info, info.PaintAppearance, r);
		}
		protected override void DrawPressed(GraphicsCache cache, BaseButtonInfo info) {
			BBrushes brushes = new BBrushes(cache, info.PaintAppearance);
			Rectangle r = info.Bounds;
			DrawBounds(cache, r, brushes.Dark, brushes.LightLight);
			r.Inflate(-1, -1);
			DrawBounds(cache, r, brushes.DarkDark, brushes.Light);
			r.Inflate(-1, -1);
			DrawButtonBackground(info, info.PaintAppearance, r);
		}
	}
	public class TabButtonFlatPainter : TabButtonDefaultPainter {
		protected override void DrawNormal(GraphicsCache cache, BaseButtonInfo info) {
			Rectangle r = info.Bounds;
			r.X++; r.Y++; r.Width--; r.Height--;
			r.X++; r.Y++; r.Width--; r.Height--;
			r.Width--; r.Height--;
			DrawButtonBackground(info, info.PaintAppearance, r);
		}
		protected override void DrawHot(GraphicsCache cache, BaseButtonInfo info) {
			base.DrawNormal(cache, info);
		}
		protected override void DrawPressed(GraphicsCache cache, BaseButtonInfo info) {
			BBrushes brushes = new BBrushes(cache, info.PaintAppearance); ;
			Rectangle r = info.Bounds;
			DrawLines(cache, r, brushes.DarkDark);
			r.X++; r.Y++; r.Width--; r.Height--;
			DrawLines(cache, r, brushes.Dark);
			r.X++; r.Y++; r.Width--; r.Height--;
			cache.Paint.FillRectangle(cache.Graphics, brushes.LightLight, new Rectangle(r.X, r.Bottom - 1, r.Width - 1, 1));
			cache.Paint.FillRectangle(cache.Graphics, brushes.LightLight, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			r.Width--; r.Height--;
			DrawButtonBackground(info, info.PaintAppearance, r);
		}
		protected void DrawLines(GraphicsCache cache, Rectangle r, Brush brush) {
			cache.Paint.FillRectangle(cache.Graphics, brush, new Rectangle(r.X, r.Y, 1, r.Height - 1));
			cache.Paint.FillRectangle(cache.Graphics, brush, new Rectangle(r.X + 1, r.Y, r.Width - 1, 1));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 3, 3);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -3, -3);
		}
	}
	public class TabButtonOffice2003Painter : ButtonOffice2003Painter {
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Disabled) {
				info.PaintAppearance.BackColor = Office2003Colors.Default[Office2003Color.ButtonDisabled];
				info.PaintAppearance.DrawBackground(info.Cache, info.Bounds);
				return;
			}
			Brush backBrush = Brushes.Transparent;
			if(info.State == ObjectState.Normal || info.State == ObjectState.Selected)
				backBrush = GetNormalBackBrush(info);
			if(info.Pressed && info.Hot)
				backBrush = cache.GetGradientBrush(info.Bounds, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical);
			else if(info.Hot)
				backBrush = GetHotBackBrush(info);
			cache.Graphics.FillRectangle(backBrush, info.Bounds);
			DrawBorder(cache, info);
		}
		protected override void DrawBorder(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Disabled) return;
			Brush borderBrish = cache.GetSolidBrush(GetBorderColor(info));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Top, info.Bounds.Width, 1));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Bottom - 1, info.Bounds.Width, 1));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Top, 1, info.Bounds.Height));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Right - 1, info.Bounds.Top, 1, info.Bounds.Height));
		}
		protected virtual Color GetBorderColor(BaseButtonInfo info) {
			if(info.State == ObjectState.Normal || info.State == ObjectState.Selected) return SystemColors.Control;
			Color clr = info.PaintAppearance.GetBorderColor();
			if(clr == SystemColors.Control) clr = Color.Empty;
			return clr != Color.Empty ? clr : Office2003Colors.Default[Office2003Color.Border];
		}
		protected virtual Brush GetHotBackBrush(BaseButtonInfo info) {
			Color c1, c2;
			c1 = info.Pressed ? Office2003Colors.Default[Office2003Color.Button1Pressed] : Office2003Colors.Default[Office2003Color.Button1Hot];
			c2 = info.Pressed ? Office2003Colors.Default[Office2003Color.Button2Pressed] : Office2003Colors.Default[Office2003Color.Button2Hot];
			AppearanceObject obj = info.PaintAppearance;
			if(obj.GetBackColor() != Color.Empty && obj.GetBackColor2() != Color.Empty) {
				c1 = obj.GetBackColor2();
				c2 = obj.GetBackColor();
				if(info.Pressed) {
					c1 = ControlPaint.Dark(c1);
				}
			}
			return info.Cache.GetGradientBrush(info.Bounds, c1, c2, LinearGradientMode.Vertical);
		}
		protected virtual Brush GetNormalBackBrush(BaseButtonInfo info) {
			if(info.State == ObjectState.Disabled) {
				return info.Cache.GetSolidBrush(SystemColors.Control);
			}
			Color c1, c2;
			c1 = Office2003Colors.Default[Office2003Color.Button1];
			c2 = Office2003Colors.Default[Office2003Color.Button2];
			AppearanceObject obj = info.PaintAppearance;
			if(obj.GetBackColor() != Color.Empty && obj.GetBackColor2() != Color.Empty) {
				c1 = obj.GetBackColor();
				c2 = obj.GetBackColor2();
			}
			return info.Cache.GetGradientBrush(info.Bounds, c1, c2, LinearGradientMode.Vertical);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 3, 2);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -3, -2);
		}
	}
	public class TabButtonWXPPainter : TabButtonStyle3DPainter {
		XtraTab.IXtraTab tabControlCore;
		Drawing.EditorButtonObjectInfoArgs infoArgs;
		public TabButtonWXPPainter(XtraTab.IXtraTab tabControl) {
			tabControlCore = tabControl;
			infoArgs = new Drawing.EditorButtonObjectInfoArgs(new DevExpress.XtraEditors.Controls.EditorButton(Controls.ButtonPredefines.Close), null);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			var info = e as BaseButtonInfo;
			if(info != null && info.Button is TabCloseButton) {
				DrawCloseButton(e.Cache, info);
				return;
			}
			base.DrawObject(e);
		}
		protected virtual void DrawCloseButton(GraphicsCache cache, BaseButtonInfo info) {
			var painter = tabControlCore.ViewInfo.CloseButtonHelper.ButtonPainter;
			infoArgs.Bounds = info.Bounds;
			infoArgs.State = info.State;
			ObjectPainter.DrawObject(cache, painter, infoArgs);
		}
	}
}
