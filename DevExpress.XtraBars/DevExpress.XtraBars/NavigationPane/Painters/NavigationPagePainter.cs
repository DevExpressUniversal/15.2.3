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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationPagePainter : ObjectPainter {
		static Font DefaultCaptionFont = Docking2010.Views.WindowsUI.SegoeUIFontsCache.GetFont("Segoe UI", 12f);
		AppearanceDefault defaultAppearanceCaption;
		public AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaption == null)
					defaultAppearanceCaption = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaption;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPageViewInfo info = e as NavigationPageViewInfo;
			DrawCaptionBackground(info.Graphics, info);
			DrawBackground(info.Graphics, info);
			DrawCustomHeaderButtons(info.Graphics, info);
			DrawText(info.Graphics, info);
		}
		protected virtual void DrawBackground(Graphics graphics, NavigationPageViewInfo info) {
			graphics.FillRectangle(Brushes.Red, info.BackgroundBounds);
		}
		protected virtual void DrawText(Graphics graphics, NavigationPageViewInfo info) {
			if(info.Owner is NavigationPage) {
				if(!(info.Owner as NavigationPage).Properties.CanHtmlDraw)
					info.Cache.DrawString(info.Owner.Caption, info.PaintAppearance.GetFont(),
					  info.PaintAppearance.GetForeBrush(info.Cache), info.TextBounds, info.PaintAppearance.GetStringFormat());
				else
					StringPainter.Default.DrawString(info.Cache, info.PaintAppearance, info.Owner.Caption, info.TextBounds, info.PaintAppearance.TextOptions, info.Owner);
			}
		}
		protected virtual void DrawCustomHeaderButtons(Graphics graphics, NavigationPageViewInfo info) {
			INavigationPage page = info.Owner as INavigationPage;
			if(page != null)
				ObjectPainter.DrawObject(info.Cache, (info.Owner as IButtonsPanelOwner).GetPainter(), page.ButtonsPanel.ViewInfo as ObjectInfoArgs);
		}
		protected virtual void DrawCaptionBackground(Graphics graphics, NavigationPageViewInfo info) { }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public virtual Rectangle GetBackgroundObjectClientRectangle(Rectangle bounds) {
			return bounds;
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			return new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultCaptionFont);
		}
		public virtual Padding Padding { get { return Padding.Empty; } }
		public virtual Padding SizingMargins { get { return Padding.Empty; } }
	}
	public class NavigationPageSkinPainter : NavigationPagePainter {
		ISkinProvider providerCore;
		public NavigationPageSkinPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		protected ISkinProvider SkinProvider {
			get { return providerCore; }
		}
		protected override AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, Color.Empty);
			(GetCaptionBackground()).ApplyForeColorAndFont(appearance);
			return appearance;
		}
		protected override void DrawCaptionBackground(Graphics graphics, NavigationPageViewInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetCaptionBackground(), info.CaptionBounds);
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, elementInfo);
		}
		protected override void DrawBackground(Graphics graphics, NavigationPageViewInfo info) {
			if(info.IsAllowedBackgroundSkinning) {
				SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.BackgroundBounds);
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, elementInfo);
			}
			else {
				NavigationPageBase infoOwner = info.Owner as NavigationPageBase;
				if(infoOwner != null && infoOwner.Owner is NavigationFrame) {
					SolidBrush brush = new SolidBrush(CommonSkins.GetSkin((infoOwner.Owner as NavigationFrame).LookAndFeel).Colors.GetColor("Control"));
					graphics.FillRectangle(brush, info.BackgroundBounds);
				}
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement element = GetCaptionBackground();
			if(element != null)
				return element.ContentMargins.Inflate(client);
			return base.CalcBoundsByClientRectangle(e, client);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinElement element = GetCaptionBackground();
			if(element != null)
				return element.ContentMargins.Deflate(e.Bounds);
			return base.GetObjectClientRectangle(e);
		}
		public override Rectangle GetBackgroundObjectClientRectangle(Rectangle bounds) {
			SkinElement element = GetBackground();
			if(element != null)
				return element.ContentMargins.Deflate(bounds);
			return bounds;
		}
		public override Padding SizingMargins {
			get {
				SkinElement element = GetBackground();
				if(element != null && element.Image != null)
					return new Padding(element.Image.SizingMargins.Left, element.Image.SizingMargins.Top, element.Image.SizingMargins.Right, element.Image.SizingMargins.Bottom);
				return base.SizingMargins;
			}
		}
		public override Padding Padding {
			get {
				SkinElement element = GetBackground();
				if(element != null)
					return new Padding(element.ContentMargins.Left, element.ContentMargins.Top, element.ContentMargins.Right, element.ContentMargins.Bottom);
				return base.Padding;
			}
		}
		protected virtual Skin GetNavigationSkin() {
			return NavigationPaneSkins.GetSkin(SkinProvider);
		}
		protected virtual Skin GetSkin() {
			return DockingSkins.GetSkin(SkinProvider);
		}
		protected virtual SkinElement GetBackground() {
			return GetNavigationSkin()[NavigationPaneSkins.SkinPageBackground] ?? GetSkin()[DockingSkins.SkinDockWindowBorder];
		}
		protected virtual SkinElement GetCaptionBackground() {
			return GetNavigationSkin()[NavigationPaneSkins.SkinPageCaption] ?? GetSkin()[DockingSkins.SkinDockWindow];
		}
	}
}
