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
using System.Text;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors.Drawing;
using System.Drawing.Imaging;
using DevExpress.Utils.Text;
namespace DevExpress.XtraWizard {
	public class WizardPainter {
		public void DrawWizardClient(WizardViewInfo viewInfo, PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e))
				CreateClientPainter(viewInfo).Draw(new GraphicsInfoArgs(cache, e.ClipRectangle));
		}
		public void DrawWizardPage(WizardViewInfo viewInfo, BaseWizardPage page, PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e))
				CreatePagePainter(viewInfo, page).Draw(new GraphicsInfoArgs(cache, e.ClipRectangle));
		}
		protected virtual WizardClientPainterBase CreateClientPainter(WizardViewInfo viewInfo) {
			if(viewInfo.IsWizardAeroStyle)
				return new WizardAeroClientPainter(viewInfo);
			if(viewInfo.IsInteriorPage)
				return new WizardClientInteriorPagePainter(viewInfo);
			return new WizardClientExteriorPagePainter(viewInfo);
		}
		protected virtual PagePainterBase CreatePagePainter(WizardViewInfo viewInfo, BaseWizardPage page) {
			if(viewInfo.IsWizardAeroStyle) {
				return new WizardAeroInteriorPagePainter(viewInfo, page);
			}
			if(page is WizardPage)
				return new InteriorPagePainter(viewInfo, page);
			return new ExteriorPagePainter(viewInfo, page);
		}
	}
	public abstract class WizardClientPainterBase {
		WizardViewInfo viewInfo;
		public WizardClientPainterBase(WizardViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected WizardViewInfo ViewInfo { get { return viewInfo; } }
		protected WizardViewInfo.WizardModelBase Model { get { return ViewInfo.Model; } }
		protected Rectangle ClientRect { get { return ViewInfo.GetClientRect(); } }
		protected Rectangle CalcClipRectangle() {
			Rectangle rect = ViewInfo.WizardControl.ClientRectangle;
			rect.Height -= ViewInfo.WizardControl.GetScaleHeight(Model.CommandAreaHeight);
			return rect;
		}
		public void Draw(GraphicsInfoArgs e) {
			DrawContentBackground(e, ClientRect);
			Rectangle clipRect = CalcClipRectangle();
			if(clipRect.Height <= 0) return;
			GraphicsClipState state = e.Cache.ClipInfo.SaveClip();
			e.Cache.ClipInfo.SetClip(clipRect);
			DrawContentRegion(e);
			DrawDividers(e);
			e.Cache.ClipInfo.RestoreClipRelease(state);
		}
		protected virtual void DrawContentBackground(GraphicsInfoArgs e, Rectangle bounds) {
			StyleObjectInfoArgs info = new StyleObjectInfoArgs();
			info.Bounds = bounds;
			ObjectPainter.DrawObject(e.Cache, ViewInfo.BackgroundPainter, info);
		}
		protected virtual void DrawDividers(GraphicsInfoArgs e) {
			int topLocation = ClientRect.Bottom - ViewInfo.WizardControl.GetScaleHeight(Model.CommandAreaHeight) - ViewInfo.GetDividerSize();
			DrawDivider(e, topLocation);
		}
		protected void DrawDivider(GraphicsInfoArgs e, int topLocation) {
			if(ViewInfo.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				PaintHelper.DrawSkinnedDivider(e.Cache, ViewInfo.LookAndFeel, new Rectangle(ClientRect.Left, topLocation, ClientRect.Width, ViewInfo.GetDividerSize()));
			else
				PaintHelper.DrawDivider(e.Cache, new Rectangle(ClientRect.Left, topLocation, ClientRect.Width, 1));
		}
		protected abstract void DrawContentRegion(GraphicsInfoArgs e);
	}
	public class WizardClientExteriorPagePainter : WizardClientPainterBase {
		protected readonly Color DefaultImageBackgroundColor = Color.FromArgb(51, 105, 166);
		public WizardClientExteriorPagePainter(WizardViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override void DrawContentRegion(GraphicsInfoArgs e) {
			DrawContentArea(e);
			DrawImages(e);
		}
		protected new WizardViewInfo.Wizard97Model Model { get { return ViewInfo.Model as WizardViewInfo.Wizard97Model; } }
		protected void DrawContentArea(GraphicsInfoArgs e) {
			AppearanceObject appearance = ViewInfo.PaintAppearance.ExteriorPageTitle;
			appearance.FillRectangle(e.Cache, ViewInfo.GetContentBounds());
			PaintHelper.DrawText(e.Cache, ViewInfo.GetPageTitleText(), appearance, appearance.TextOptions.VAlignment, Model.GetExteriorTitleBounds(), ViewInfo.DrawHtmlText);
		}
		protected void DrawImages(GraphicsInfoArgs e) {
			if(ViewInfo.Image != null) {
				Rectangle rect = Model.GetExteriorImageBounds();
				BackgroundImagePainter.DrawBackgroundImage(e.Graphics, ViewInfo.Image, Color.Empty, ViewInfo.WizardControl.ImageLayout, rect, rect, Point.Empty, RightToLeft.No);
			}
			else {
				e.Cache.FillRectangle(DefaultImageBackgroundColor, Model.GetExteriorImageBounds());
			}
			if(ViewInfo.HeaderImage != null && ViewInfo.WizardControl.ShowHeaderImage)
				e.Graphics.DrawImage(ViewInfo.HeaderImage, Model.GetExteriorHeaderImageBounds());
		}
	}
	public class WizardClientInteriorPagePainter : WizardClientPainterBase {
		public WizardClientInteriorPagePainter(WizardViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected new WizardViewInfo.Wizard97Model Model { get { return ViewInfo.Model as WizardViewInfo.Wizard97Model; } }
		protected override void DrawContentRegion(GraphicsInfoArgs e) {
			DrawHeaderBackground(e, Model.GetInteriorHeaderBounds());
			DrawHeaderText(e);
			DrawHeaderImage(e);
		}
		protected override void DrawDividers(GraphicsInfoArgs e) {
			base.DrawDividers(e);
			DrawDivider(e, Model.GetInteriorHeaderBounds().Bottom);
		}
		protected virtual void DrawHeaderBackground(GraphicsInfoArgs e, Rectangle headerRect) {
			ViewInfo.PaintAppearance.PageTitle.FillRectangle(e.Cache, headerRect);
		}
		protected virtual void DrawHeaderImage(GraphicsInfoArgs e) {
			if(ViewInfo.HeaderImage != null)
				e.Graphics.DrawImage(ViewInfo.HeaderImage, Model.GetInteriorHeaderImageBounds());
		}
		protected virtual void DrawHeaderText(GraphicsInfoArgs e) {
			Rectangle rect = Model.GetPageTitleBounds();
			string titleText = ViewInfo.GetPageTitleText();
			PaintHelper.DrawText(e.Cache, titleText, ViewInfo.PaintAppearance.PageTitle, VertAlignment.Top, rect, ViewInfo.DrawHtmlText);
			SizeF size = ViewInfo.PaintAppearance.PageTitle.CalcTextSize(e.Cache, titleText, rect.Width);
			rect.Y += (int)Math.Ceiling(size.Height) + 3;
			rect.X += Wizard97Consts.ContentMargin * 2;
			rect.Width -= Wizard97Consts.ContentMargin * 2;
			PaintHelper.DrawText(e.Cache, ViewInfo.GetPageDescriptionText(), ViewInfo.PaintAppearance.Page, VertAlignment.Top, rect, ViewInfo.DrawHtmlText);
		}
	}
	public class WizardAeroClientPainter : WizardClientPainterBase {
		public WizardAeroClientPainter(WizardViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected new WizardViewInfo.WizardAeroModel Model { get { return ViewInfo.Model as WizardViewInfo.WizardAeroModel; } }
		protected override void DrawContentRegion(GraphicsInfoArgs e) {
			DrawClientBackground(e, ViewInfo.GetContentBounds());
			DrawTitleBarTitleText(e);
			DrawTitleBarIcon(e);
			DrawHeaderTitleText(e);
		}
		protected virtual void DrawClientBackground(GraphicsInfoArgs e, Rectangle bounds) {
			ViewInfo.PaintAppearance.Page.FillRectangle(e.Cache, bounds);
		}
		protected override void DrawContentBackground(GraphicsInfoArgs e, Rectangle bounds) {
			if(!ViewInfo.IsAeroEnabled() || ViewInfo.WizardControl.IsDesignMode)
				bounds = ViewInfo.WizardControl.ClientRectangle;
			base.DrawContentBackground(e, bounds);
		}
		protected override void DrawDividers(GraphicsInfoArgs e) {
			base.DrawDividers(e);
			if(!ViewInfo.IsAeroEnabled() || ViewInfo.WizardControl.IsDesignMode)
				DrawDivider(e, Model.GetTitleBarBounds().Bottom);
		}
		protected virtual void DrawTitleBarTitleText(GraphicsInfoArgs e) {
			AppearanceObject appearance = ViewInfo.PaintAppearance.AeroWizardTitle;
			Rectangle textRect = Model.GetTitleBarTitleTextBounds(ViewInfo.WizardControl.TitleImage != null);
			if(ViewInfo.IsAeroEnabled() && !ViewInfo.WizardControl.IsDesignMode) {
				const int glowSize = 10;
				string text = StringPainter.Default.RemoveFormat(ViewInfo.GetWizardTitleText());
				Size textSize = CalcSimpleTextSize(e.Cache, appearance, text, textRect.Width - glowSize);
				textRect.Width = textSize.Width + glowSize;
				NativeVista.DrawGlowingText(e.Graphics, text, appearance.Font, textRect, appearance.ForeColor, GetTextFormatFlags(appearance));
				return;
			}
			PaintHelper.DrawText(e.Cache, ViewInfo.GetWizardTitleText(), appearance, appearance.TextOptions.VAlignment, textRect, ViewInfo.DrawHtmlText);
		}
		TextFormatFlags GetTextFormatFlags(AppearanceObject appearance) {
			return TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter;
		}
		Size CalcSimpleTextSize(GraphicsCache cache, AppearanceObject appearance, string text, int maxWidth) {
			using(StringFormat fmt = GetSimpleTextStringFormat(appearance)) {
				return appearance.CalcTextSize(cache.Graphics, fmt, text, maxWidth).ToSize();
			}
		}
		StringFormat GetSimpleTextStringFormat(AppearanceObject appearance) {
			StringFormat fmt = (StringFormat)appearance.GetStringFormat().Clone();
			fmt.LineAlignment = StringAlignment.Center;
			fmt.Alignment = StringAlignment.Near;
			fmt.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;
			fmt.Trimming = StringTrimming.None;
			return fmt;
		}
		protected virtual void DrawTitleBarIcon(GraphicsInfoArgs e) {
			Image image = ViewInfo.WizardControl.TitleImage;
			if(image != null)
				e.Paint.DrawImage(e.Graphics, image, Model.GetTitleBarIconBounds());
		}
		protected virtual void DrawHeaderTitleText(GraphicsInfoArgs e) {
			AppearanceObject appearance = ViewInfo.PaintAppearance.PageTitle;
			PaintHelper.DrawText(e.Cache, ViewInfo.GetPageTitleText(), appearance, appearance.TextOptions.VAlignment, Model.GetHeaderTitleBounds(), ViewInfo.DrawHtmlText);
		}
	}
	public abstract class PagePainterBase {
		BaseWizardPage page;
		WizardViewInfo viewInfo;
		public PagePainterBase(WizardViewInfo viewInfo, BaseWizardPage page) {
			this.page = page;
			this.viewInfo = viewInfo;
		}
		protected BaseWizardPage Page { get { return page; } }
		protected WizardViewInfo ViewInfo { get { return viewInfo; } }
		public void Draw(GraphicsInfoArgs e) {
			DrawBackground(e);
			DrawContentRegion(e, Page.ClientRectangle);
		}
		protected virtual void DrawContentRegion(GraphicsInfoArgs e, Rectangle bounds) { }
		protected abstract void DrawBackground(GraphicsInfoArgs e);
	}
	public class InteriorPagePainter : PagePainterBase {
		public InteriorPagePainter(WizardViewInfo viewInfo, BaseWizardPage page)
			: base(viewInfo, page) {
		}
		protected override void DrawBackground(GraphicsInfoArgs e) {
			if(IsSkinPaintingRequired) {
				StyleObjectInfoArgs info = new StyleObjectInfoArgs();
				info.Bounds = Page.ClientRectangle;
				ObjectPainter.DrawObject(e.Cache, ViewInfo.BackgroundPainter, info);
			}
			else {
				ViewInfo.PaintAppearance.Page.FillRectangle(e.Cache, Page.ClientRectangle);
			}
		}
		bool IsSkinPaintingRequired { 
			get { 
				AppearanceObject app = ViewInfo.WizardControl.Appearance.Page;
				return app.BackColor.IsEmpty || app.BackColor == Color.Transparent;
			} 
		}
	}
	public class WizardAeroInteriorPagePainter : PagePainterBase {
		public WizardAeroInteriorPagePainter(WizardViewInfo viewInfo, BaseWizardPage page)
			: base(viewInfo, page) {
		}
		protected override void DrawBackground(GraphicsInfoArgs e) {
			ViewInfo.PaintAppearance.Page.FillRectangle(e.Cache, Page.ClientRectangle);
		}
	}
	public class WizardAeroExteriorPagePainter : ExteriorPagePainter {
		public WizardAeroExteriorPagePainter(WizardViewInfo viewInfo, BaseWizardPage page)
			: base(viewInfo, page) {
		}
		protected override void DrawContentRegion(GraphicsInfoArgs e, Rectangle bounds) {
			if(!ViewInfo.WizardControl.AllowPagePadding) {
				bounds.X += WizardAeroConsts.ContentLeftMargin;
				bounds.Height -= WizardAeroConsts.ContentBottomMargin;
				bounds.Width -= WizardAeroConsts.ContentRightMargin + WizardAeroConsts.ContentLeftMargin;
			}
			DrawTopText(e, bounds);
			DrawBottomText(e, bounds);
		}
	}
	public class ExteriorPagePainter : PagePainterBase {
		public ExteriorPagePainter(WizardViewInfo viewInfo, BaseWizardPage page)
			: base(viewInfo, page) {
		}
		protected override void DrawBackground(GraphicsInfoArgs e) {
			ViewInfo.PaintAppearance.ExteriorPage.FillRectangle(e.Cache, Page.ClientRectangle);
		}
		protected override void DrawContentRegion(GraphicsInfoArgs e, Rectangle bounds) {
			if(!ViewInfo.WizardControl.AllowPagePadding) {
				bounds.X += Wizard97Consts.ContentMargin;
				bounds.Height -= Wizard97Consts.ContentMargin;
				bounds.Width -= 2 * Wizard97Consts.ContentMargin;
			}
			DrawTopText(e, bounds);
			DrawBottomText(e, bounds);
		}
		protected virtual void DrawTopText(GraphicsInfoArgs e, Rectangle bounds) {
			bounds.Inflate(-2, -2);
			PaintHelper.DrawText(e.Cache, GetTopText(), ViewInfo.PaintAppearance.ExteriorPage, VertAlignment.Top, bounds, ViewInfo.DrawHtmlText);
		}
		protected virtual void DrawBottomText(GraphicsInfoArgs e, Rectangle bounds) {
			bounds.Inflate(-2, -2);
			PaintHelper.DrawText(e.Cache, GetBottomText(), ViewInfo.PaintAppearance.ExteriorPage, VertAlignment.Bottom, bounds, ViewInfo.DrawHtmlText);
		}
		protected WelcomeWizardPage WelcomePage { get { return Page as WelcomeWizardPage; } }
		protected CompletionWizardPage CompletionPage { get { return Page as CompletionWizardPage; } }
		protected virtual string GetTopText() {
			if(WelcomePage != null)
				return WelcomePage.IntroductionText;
			if(CompletionPage != null)
				return CompletionPage.FinishText;
			return string.Empty;
		}
		protected virtual string GetBottomText() {
			BaseWelcomeWizardPage page = Page as BaseWelcomeWizardPage;
			if(page != null)
				return page.ProceedText;
			return string.Empty;
		}
	}
	#region Internal
	public class PaintHelper {
		public static void DrawText(GraphicsCache cache, string text, AppearanceObject appearance, VertAlignment align, Rectangle rect, bool drawHtmlText) {
			TextOptions options = new TextOptions(appearance);
			options.Assign(TextOptions.DefaultOptions);
			options.VAlignment = align;
			if(drawHtmlText) 
				StringPainter.Default.DrawString(cache, appearance, text, rect, options);
			else 
				appearance.DrawString(cache, text, rect, appearance.GetStringFormat(options));
		}
		public static void DrawSkinnedDivider(GraphicsCache cache, UserLookAndFeel lookAndFeel, Rectangle rect) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(lookAndFeel)[CommonSkins.SkinLabelLine], rect);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		public static void DrawDivider(GraphicsCache cache, Rectangle rect) {
			cache.Graphics.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
			cache.Graphics.DrawLine(SystemPens.ControlLightLight, rect.Left, rect.Top, rect.Right, rect.Top);
		}
	}
	public class ControlPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject.ControlAppearance.DrawBackground(e.Cache, e.Bounds);
		}
	}
	public class ControlSkinPainter : SkinCustomPainter {
		public ControlSkinPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinGroupPanelNoBorder], e.Bounds);
			return info;
		}
	}
	#endregion
}
