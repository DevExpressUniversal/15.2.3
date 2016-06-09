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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using System.Reflection;
using System.ComponentModel;
namespace DevExpress.XtraWizard {
	public enum WizardHitTest { None, PageClient, PrevButton, NextButton, CancelButton, HelpButton, NavigationPanel }
	public class WizardHitInfo {
		WizardHitTest hitTest;
		Point hitPoint;
		public WizardHitInfo() {
			Clear();
		}
		public virtual void Clear() {
			this.hitPoint = new Point(-10000, -10000);
			this.hitTest = WizardHitTest.None;
		}
		public bool IsValid { get { return HitPoint.X != -10000; } }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public WizardHitTest HitTest { get { return hitTest; } set { hitTest = value; } }
	}
	public class WizardBaseConsts {
		public const int DefaultDividerSize = 2;
		public const int CommandAreaHeight = 44;
		public const int DefaultCommandButtonVerticalIndent = 8;
	}
	public class Wizard97Consts : WizardBaseConsts {
		public const int ContentMargin = 16;
		public const int ImageWidth = 185;
		public const int InteriorHeaderTitleHeight = 65;
		public const int HeaderImageSize = 61;
		public const int CommandButtonSpacingMargin = 11;
	}
	public class WizardAeroConsts : WizardBaseConsts {
		public const int ContentLeftMargin = 38;
		public const int ContentRightMargin = 22;
		public const int ContentBottomMargin = 19;
		public const int TitleBarHeight = 38;
		public const int TitleBarIconSize = 16;
		public const int TitleBarElementSpacing = 4; 
		public const int HeaderMargin = 19;
		public const int CommandButtonSpacing = 7;
	}
	public class ButtonInfo {
		WizardButton button;
		Point location;
		public ButtonInfo(WizardButton button) {
			this.button = button;
			this.location = Point.Empty;
		}
		public Point Location { get { return location; } set { location = value; } }
		public Size Size { get { return button.Size; } }
		public string Text { get { return button.Text; } set { button.Text = value; } }
		public bool Visible { get { return button.Visible; } set { button.Visible = value; } }
		public Image Image { get { return button.Image; } set { button.Image = value; } }
		public WizardButton Button { get { return button; } }
		internal bool Enabled { get { return button.Enabled; } set { button.Enabled = value; } }
		internal void SetLocation() {
			Button.Location = Location;
		}
	}
	public class WizardViewInfo : IDisposable {
		WizardControl wizardControl;
		ObjectPainter backgroundPainter;
		WizardAppearanceCollection paintAppearance;
		WizardModelBase wizardModel;
		ButtonInfo helpButtonInfo, cancelButtonInfo, nextButtonInfo, prevButtonInfo, finishButtonInfo;
		bool paintAppearanceDirty = true;
		internal bool lockFocus;
		public WizardViewInfo(WizardControl control) {
			this.wizardControl = control;
			this.backgroundPainter = null;
			this.paintAppearance = new WizardAppearanceCollection(wizardControl);
			CreateButtonsInfo();
			CreateWizardModel();
		}
		protected virtual void CreateButtonsInfo() {
			this.helpButtonInfo = new ButtonInfo(WizardControl.btnHelp);
			this.cancelButtonInfo = new ButtonInfo(WizardControl.btnCancel);
			this.nextButtonInfo = new ButtonInfo(WizardControl.btnNext);
			this.prevButtonInfo = new ButtonInfo(WizardControl.btnPrevious);
			this.finishButtonInfo = new ButtonInfo(WizardControl.btnFinish);
		}
		public WizardAppearanceCollection PaintAppearance {
			get {
				UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		public void SetAppearanceDirty() { this.paintAppearanceDirty = true; }
		protected virtual void UpdatePaintAppearance() {
			if(!this.paintAppearanceDirty) return;
			this.paintAppearanceDirty = false;
			PaintAppearance.Combine(WizardControl.Appearance, Model.GetAppearanceDefault());
			if(PaintAppearance.ExteriorPageTitle.TextOptions.WordWrap == WordWrap.Default)
				PaintAppearance.ExteriorPageTitle.TextOptions.WordWrap = WordWrap.Wrap;
			if(PaintAppearance.ExteriorPage.TextOptions.WordWrap == WordWrap.Default)
				PaintAppearance.ExteriorPage.TextOptions.WordWrap = WordWrap.Wrap;
			if(PaintAppearance.Page.TextOptions.WordWrap == WordWrap.Default)
				PaintAppearance.Page.TextOptions.WordWrap = WordWrap.Wrap;
		}
		internal void CreateWizardModel() {
			this.wizardModel = CreateWizardModelCore(WizardControl.WizardStyle);
			SetAppearanceDirty();
		}
		protected virtual WizardModelBase CreateWizardModelCore(WizardStyle style) {
			if(style == WizardStyle.WizardAero)
				return new WizardAeroModel(this);
			return new Wizard97Model(this);
		}
		protected internal ObjectPainter BackgroundPainter {
			get {
				if(backgroundPainter == null)
					backgroundPainter = CreateBackgroundPainter();
				return backgroundPainter;
			}
		}
		protected virtual ObjectPainter CreateBackgroundPainter() {
			UserLookAndFeel lookAndFeel = WizardControl.LookAndFeel;
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new ControlSkinPainter(lookAndFeel);
			return new ControlPainter();
		}
		protected internal WizardModelBase Model { get { return wizardModel; } }
		protected internal WizardControl WizardControl { get { return wizardControl; } }
		protected internal UserLookAndFeel LookAndFeel { get { return WizardControl.LookAndFeel; } }
		internal bool IsWizardAeroStyle { get { return Model is WizardAeroModel; } }
		protected virtual internal bool IsAeroEnabled() {
			if(!(NativeVista.IsVista && NativeVista.IsCompositionEnabled())) return false;
			Form form = WizardControl.Parent as Form;
			if(form == null || form.IsMdiChild || form.FormBorderStyle == FormBorderStyle.None || !form.ControlBox) return false;
			if(form.Controls.Count > 1) return false;
			XtraForm xtraForm = form as XtraForm;
			if(xtraForm != null && GetAllowFormSkin(xtraForm)) return false;
			return true;
		}
		bool GetAllowFormSkin(XtraForm form) {
			MethodInfo mi = form.GetType().GetMethod("GetAllowSkin", BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi != null) return (bool)mi.Invoke(form, null);
			return form.AllowFormSkin && SkinManager.AllowFormSkins;
		}
		protected internal string GetWizardTitleText() {
			return WizardControl.Text;
		}
		protected internal string GetPageTitleText() {
			if(SelectedPage != null) {
				return SelectedPage.Text;
			}
			return string.Empty;
		}
		protected internal string GetPageDescriptionText() {
			if(SelectedPage != null) {
				WizardPage page = SelectedPage as WizardPage;
				if(page != null)
					return page.DescriptionText;
			}
			return string.Empty;
		}
		internal ButtonInfo HelpButton { get { return helpButtonInfo; } }
		internal ButtonInfo CancelButton { get { return cancelButtonInfo; } }
		internal ButtonInfo NextButton { get { return nextButtonInfo; } }
		internal ButtonInfo PrevButton { get { return prevButtonInfo; } }
		internal ButtonInfo FinishButton { get { return finishButtonInfo; } }
		BackButton BackButton { get { return WizardControl.btnBack; } }
		public bool IsLastPage { 
			get { 
				BaseWizardPage nextPage = WizardControl.GetNextPage(WizardControl.SelectedPageIndex);
				if(nextPage == null) return true;
				return false;
			} 
		}
		public bool IsInteriorPage { get { return SelectedPage is WizardPage; } }
		protected BaseWizardPage SelectedPage { get { return WizardControl.SelectedPage; } }
		protected internal Image Image { get { return WizardControl.Image; } }
		protected internal Image HeaderImage { get { return WizardControl.HeaderImage; } }
		protected internal virtual void UpdateButtonsState() {
			BackButton.Visible = IsWizardAeroStyle;
			if(BackButton.Visible)
				BackButton.Size = BackButton.CalcBestSize();
			PrevButton.Enabled = BackButton.Enabled = WizardControl.SelectedPageIndex > 0 && (SelectedPage.AllowBack || WizardControl.IsDesignMode);
			PrevButton.Visible = !IsWizardAeroStyle;
			NextButton.Enabled = WizardControl.SelectedPageIndex < WizardControl.Pages.Count - 1 && (SelectedPage.AllowNext || WizardControl.IsDesignMode);
			NextButton.Visible = !IsLastPage || WizardControl.IsDesignMode;
			FinishButton.Visible = IsLastPage && !WizardControl.IsDesignMode;
			FinishButton.Enabled = SelectedPage != null && SelectedPage.AllowFinish;
			CancelButton.Enabled = WizardControl.SelectedPage != null && SelectedPage.AllowCancel;
			HelpButton.Visible = WizardControl.HelpVisible;
			PrevButton.Button.SetText(WizardControl.PreviousText);
			NextButton.Button.SetText(WizardControl.NextText);
			FinishButton.Button.SetText(WizardControl.FinishText);
			CancelButton.Button.SetText(WizardControl.CancelText);
			HelpButton.Button.SetText(WizardControl.HelpText);
		}
		public void UpdateSelectionPage() {
			UpdatePagesBounds();
			UpdateButtons();
		}
		protected virtual void UpdateButtons() {
			UpdateButtonsState();
			Model.UpdateButtonsLocation();
			CustomizeButtons();
			if(!lockFocus && !PrevButton.Enabled) NextButton.Button.Focus();
			if(!lockFocus && !NextButton.Enabled) FinishButton.Button.Focus();
			WizardControl.UpdateAcceptCancelButtons();
		}
		protected virtual void UpdatePagesBounds() {
			foreach(BaseWizardPage page in WizardControl.Pages) {
				if(page != WizardControl.SelectedPage)
					page.Bounds = new Rectangle(new Point(-10000, -10000), GetPageBounds(page).Size);
				page.VisibleInternal = (page == WizardControl.SelectedPage);
			}
			if(WizardControl.SelectedPage == null) return;
			WizardControl.SelectedPage.Bounds = GetPageBounds(WizardControl.SelectedPage);
		}
		Rectangle GetPageBounds(BaseWizardPage page) {
			if(page is WizardPage)
				return Model.GetInteriorPageBounds(page);
			return Model.GetExteriorPageBounds(page);
		}
		public Rectangle GetClientRect() {
			return Model.GetClientRectangle();
		}
		public Rectangle GetContentBounds() {
			return Model.GetContentBounds(); 
		}
		public virtual WizardHitInfo CalcHitInfo(Point pt) {
			WizardHitInfo hitInfo = new WizardHitInfo();
			hitInfo.HitPoint = pt;
			if(pt.Y > WizardControl.Height - CancelButton.Size.Height - 2 * Wizard97Consts.CommandButtonSpacingMargin)
				hitInfo.HitTest = WizardHitTest.NavigationPanel;
			Control pointControl = WizardControl.GetChildAtPoint(pt);
			if(pointControl == NextButton.Button) hitInfo.HitTest = WizardHitTest.NextButton;
			if(pointControl == PrevButton.Button || pointControl == BackButton) hitInfo.HitTest = WizardHitTest.PrevButton;
			if(pointControl == CancelButton.Button) hitInfo.HitTest = WizardHitTest.CancelButton;
			if(pointControl == HelpButton.Button) hitInfo.HitTest = WizardHitTest.HelpButton;
			if(SelectedPage != null && pointControl == SelectedPage) hitInfo.HitTest = WizardHitTest.PageClient;
			return hitInfo;
		}
		public virtual void UpdatePainters() {
			this.backgroundPainter = null;
		}
		public void Dispose() {
			PaintAppearance.Dispose();
		}
		protected internal int GetDividerSize() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinLabelLine];
				if(element == null) return WizardBaseConsts.DefaultDividerSize;
				return element.Size.MinSize.Height;
			}
			return WizardBaseConsts.DefaultDividerSize;
		}
		internal void CustomizeButtons() {
			WizardControl.RaiseCustomizeCommandButtons(new CustomizeCommandButtonsEventArgs(this, SelectedPage));
			HelpButton.SetLocation();
			CancelButton.SetLocation();
			PrevButton.SetLocation();
			NextButton.SetLocation();
			FinishButton.SetLocation();
		}
		public abstract class WizardModelBase {
			WizardViewInfo viewInfo;
			public WizardModelBase(WizardViewInfo viewInfo) {
				this.viewInfo = viewInfo;
			}
			protected WizardViewInfo ViewInfo { get { return viewInfo; } }
			protected WizardControl Wizard { get { return ViewInfo.WizardControl; } }
			public virtual int CommandAreaHeight { get { return Math.Max(ViewInfo.GetMaxButtonHeight() + WizardBaseConsts.DefaultCommandButtonVerticalIndent, WizardBaseConsts.CommandAreaHeight); } }
			public abstract void UpdateButtonsLocation();
			public abstract Rectangle GetContentBounds();
			public abstract Rectangle GetClientRectangle();
			public abstract Rectangle GetInteriorPageBounds(BaseWizardPage page);
			public abstract Rectangle GetExteriorPageBounds(BaseWizardPage page);
			public abstract AppearanceDefaultInfo[] GetAppearanceDefault();
			protected Size CalcTextSize(string text, AppearanceObject appearance, int width) {
				GraphicsInfo gInfo = new GraphicsInfo();
				gInfo.AddGraphics(null);
				SizeF size = SizeF.Empty;
				try {
					size = appearance.CalcTextSize(gInfo.Graphics, text, width);
				}
				finally {
					gInfo.ReleaseGraphics();
				}
				return size.ToSize();
			}
			protected Color GetSystemColor(Color color) {
				return LookAndFeelHelper.GetSystemColor(Wizard.LookAndFeel, color);
			}
			protected Color GetSkinPropertyColor(string propertyName, Color defaultColor) {
				Skin skin = SkinManager.Default.GetSkin(SkinProductId.Common, ViewInfo.LookAndFeel);
				if(skin == null)
					return defaultColor;
				Color color = skin.Properties.GetColor(propertyName);
				if(color == Color.Empty)
					return defaultColor;
				return color;
			}
		}
		public class Wizard97Model : WizardModelBase {
			public Wizard97Model(WizardViewInfo viewInfo)
				: base(viewInfo) {
			}
			public override AppearanceDefaultInfo[] GetAppearanceDefault() {
				Color windowColor = GetSystemColor(SystemColors.Window), windowTextColor = GetSystemColor(SystemColors.WindowText), controlText = GetSystemColor(SystemColors.WindowText);
				AppearanceDefaultInfo[] result = new AppearanceDefaultInfo[] {
					new AppearanceDefaultInfo(AppearanceName.ExteriorPage, new AppearanceDefault(windowTextColor, windowColor)),
					new AppearanceDefaultInfo(AppearanceName.ExteriorPageTitle, new AppearanceDefault(controlText, windowColor, HorzAlignment.Default, VertAlignment.Center, GetDefaultPageFont(14, FontStyle.Bold))),
					new AppearanceDefaultInfo(AppearanceName.PageTitle, new AppearanceDefault(windowTextColor, windowColor, GetDefaultPageTitleFont(8, FontStyle.Bold))),
					new AppearanceDefaultInfo(AppearanceName.Page, new AppearanceDefault(windowTextColor, windowColor)),
					new AppearanceDefaultInfo(AppearanceName.AeroWizardTitle, new AppearanceDefault(controlText, Color.Transparent, GetDefaultPageFont(12, FontStyle.Regular)))
				};
				return result;
			}
			Font GetDefaultPageTitleFont(float size, FontStyle style) {
				try {
					return new Font(SystemFonts.DefaultFont.FontFamily, size, style);
				}
				catch {}
				return new Font(SystemFonts.DefaultFont.FontFamily, size);
			}
			Font GetDefaultPageFont(float size, FontStyle style) {
				try {
					return new Font("Verdana", size, style);
				}
				catch { }
				return new Font(SystemFonts.DefaultFont.FontFamily, size);
			}
			public override Rectangle GetClientRectangle() {
				return Wizard.ClientRectangle;
			}
			protected int ImageWidth { get { return Wizard.ImageWidth; } }
			public override Rectangle GetExteriorPageBounds(BaseWizardPage page) {
				Rectangle rect = GetContentBounds();
				rect.X += ImageWidth;
				rect.Width -= ImageWidth;
				int height = GetExteriorTitleBounds(page).Height;
				rect.Y += height;
				rect.Height -= height;
				if(Wizard.AllowPagePadding) {
					rect.Inflate(-Wizard97Consts.ContentMargin, -Wizard97Consts.ContentMargin);
				}
				else {
					rect.Y += Wizard97Consts.ContentMargin;
					rect.Height -= Wizard97Consts.ContentMargin;
				}
				return rect;
			}
			public override Rectangle GetContentBounds() {
				Rectangle rect = GetClientRectangle();
				rect.Height -= Wizard.GetScaleHeight(CommandAreaHeight) + ViewInfo.GetDividerSize();
				return rect;
			}
			public override Rectangle GetInteriorPageBounds(BaseWizardPage page) {
				Rectangle rect = GetContentBounds();
				int h = ViewInfo.WizardControl.GetScaleHeight(Wizard97Consts.InteriorHeaderTitleHeight) + ViewInfo.GetDividerSize();
				rect.Y += h;
				rect.Height -= h;
				if(Wizard.AllowPagePadding)
					rect.Inflate(-Wizard97Consts.ContentMargin, -Wizard97Consts.ContentMargin);
				return rect;
			}
			public virtual Rectangle GetInteriorHeaderBounds() {
				Rectangle rect = GetContentBounds();
				rect.Height = ViewInfo.WizardControl.GetScaleHeight(Wizard97Consts.InteriorHeaderTitleHeight);
				return rect;
			}
			public virtual Rectangle GetInteriorHeaderImageBounds() {
				Rectangle rect = GetInteriorHeaderBounds();
				int padding = Wizard97Consts.InteriorHeaderTitleHeight - Wizard97Consts.HeaderImageSize;
				rect.X = rect.Right - Wizard97Consts.ContentMargin - Wizard97Consts.HeaderImageSize;
				rect.Y = rect.Top + padding / 2;
				rect.Size = new Size(Wizard97Consts.HeaderImageSize, Wizard97Consts.HeaderImageSize);
				return rect;
			}
			public virtual Rectangle GetPageTitleBounds() {
				Rectangle rect = GetInteriorHeaderBounds();
				rect.Inflate(-Wizard97Consts.ContentMargin, -Wizard97Consts.ContentMargin);
				rect.Width -= Wizard97Consts.HeaderImageSize + Wizard97Consts.ContentMargin;
				return rect;
			}
			public virtual Rectangle GetExteriorImageBounds() {
				Rectangle bounds = GetContentBounds();
				bounds.Width = ImageWidth;
				return bounds;
			}
			public virtual Rectangle GetExteriorTitleBounds() {
				return GetExteriorTitleBounds(Wizard.SelectedPage);
			}
			public virtual Rectangle GetExteriorTitleBounds(BaseWizardPage page) {
				Rectangle bounds = GetContentBounds();
				bounds.X += ImageWidth;
				bounds.Width -= ImageWidth;
				string text = (page == null ? "" : page.Text);
				int height = CalcTextSize(text, ViewInfo.PaintAppearance.ExteriorPageTitle, bounds.Width).Height;
				height += 2 * Wizard97Consts.ContentMargin;
				bounds.Height = height;
				bounds.Inflate(-Wizard97Consts.ContentMargin, 0);
				return bounds;
			}
			public virtual Rectangle GetExteriorHeaderImageBounds() {
				Rectangle bounds = GetExteriorImageBounds();
				bounds.Y += Wizard97Consts.ContentMargin;
				bounds.X = bounds.Right - Wizard97Consts.ContentMargin - Wizard97Consts.HeaderImageSize;
				bounds.Size = new Size(Wizard97Consts.HeaderImageSize, Wizard97Consts.HeaderImageSize);
				return bounds;
			}
			protected int GetCommandButtonTopLocation(ButtonInfo button) {
				return Wizard.ClientRectangle.Bottom - Wizard.GetScaleHeight(CommandAreaHeight) / 2 - button.Size.Height / 2;
			}
			public override void UpdateButtonsLocation() {
				ViewInfo.HelpButton.Location = new Point(Wizard.ClientRectangle.Width - Wizard97Consts.CommandButtonSpacingMargin - ViewInfo.HelpButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.HelpButton));
				ViewInfo.CancelButton.Location = new Point(Wizard.ClientRectangle.Width - Wizard97Consts.CommandButtonSpacingMargin - ViewInfo.CancelButton.Size.Width - (ViewInfo.HelpButton.Visible ? ViewInfo.HelpButton.Size.Width + Wizard97Consts.CommandButtonSpacingMargin : 0), GetCommandButtonTopLocation(ViewInfo.CancelButton));
				ViewInfo.NextButton.Location = new Point(ViewInfo.CancelButton.Location.X - Wizard97Consts.CommandButtonSpacingMargin - ViewInfo.NextButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.NextButton));
				ViewInfo.FinishButton.Location = new Point(ViewInfo.CancelButton.Location.X - Wizard97Consts.CommandButtonSpacingMargin - ViewInfo.FinishButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.FinishButton));
				ViewInfo.PrevButton.Location = new Point(GetNextButtonLocationX() - ViewInfo.PrevButton.Size.Width - 4, GetCommandButtonTopLocation(ViewInfo.PrevButton));
			}
			int GetNextButtonLocationX() {
				if(ViewInfo.NextButton.Visible) return ViewInfo.NextButton.Location.X;
				return ViewInfo.FinishButton.Location.X;
			}
		}
		public class WizardAeroModel : WizardModelBase {
			public WizardAeroModel(WizardViewInfo viewInfo)
				: base(viewInfo) {
			}
			public override AppearanceDefaultInfo[] GetAppearanceDefault() {
				Color windowColor = GetSystemColor(SystemColors.Window);
				Color titleTextColor = GetTitleForeColor();
				AppearanceDefaultInfo[] result = new AppearanceDefaultInfo[] {
					new AppearanceDefaultInfo(AppearanceName.PageTitle, new AppearanceDefault(GetAeroWizardTitleForeColor(Color.FromArgb(19, 112, 171)), windowColor, HorzAlignment.Default, VertAlignment.Center, GetDefaultTitleFont(12, FontStyle.Regular))),
					new AppearanceDefaultInfo(AppearanceName.Page, new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetAeroWizardPageBackColor(windowColor))),
					new AppearanceDefaultInfo(AppearanceName.AeroWizardTitle, new AppearanceDefault(titleTextColor, Color.Transparent, GetDefaultTitleFont(9, FontStyle.Regular)))
				};
				return result;
			}
			protected internal virtual int TitleBarHeight { get { return Math.Max(ViewInfo.GetBackButtonHeight() + WizardBaseConsts.DefaultCommandButtonVerticalIndent, WizardAeroConsts.CommandAreaHeight); } }
			protected internal virtual int TitleBarButtonSize { get { return ViewInfo.GetBackButtonHeight();  } }
			Font GetDefaultTitleFont(float size, FontStyle style) {
				try {
					return new Font(SystemFonts.CaptionFont.FontFamily, size, style);
				}
				catch { }
				return new Font(SystemFonts.DefaultFont.FontFamily, size);
			}
			protected Color GetTitleForeColor() {
				Color titleTextColor = GetSystemColor(SystemColors.ControlText);
				SkinElement element = CommonSkins.GetSkin(Wizard.LookAndFeel)[CommonSkins.SkinLabel];
				if(element != null) {
					titleTextColor = element.GetAppearanceDefault().ForeColor;
					if(titleTextColor == Color.Transparent)
						titleTextColor = LookAndFeelHelper.GetTransparentForeColor(Wizard.LookAndFeel, Wizard);
				}
				return titleTextColor;
			}
			protected Color GetAeroWizardTitleForeColor(Color defaultColor) {
				return GetSkinPropertyColor("AeroWizardTitle", defaultColor);
			}
			protected Color GetAeroWizardPageBackColor(Color defaultColor) {
				return GetSkinPropertyColor("WizardPageColor", defaultColor);
			}
			public override Rectangle GetClientRectangle() {
				Rectangle rect = Wizard.ClientRectangle;
				int divSize = ViewInfo.IsAeroEnabled() || Wizard.IsDesignMode ? 0 : ViewInfo.GetDividerSize();
				int h = TitleBarHeight + divSize;
				rect.Y += h;
				rect.Height -= h;
				return rect;
			}
			public override Rectangle GetContentBounds() {
				Rectangle bounds = GetClientRectangle();
				bounds.Height -= Wizard.GetScaleHeight(CommandAreaHeight) + ViewInfo.GetDividerSize();
				return bounds;
			}
			public override Rectangle GetExteriorPageBounds(BaseWizardPage page) {
				return GetInteriorPageBounds(page);
			}
			public override Rectangle GetInteriorPageBounds(BaseWizardPage page) {
				Rectangle rect = GetContentBounds();
				if(Wizard.AllowPagePadding) {
					rect.X += WizardAeroConsts.ContentLeftMargin;
					rect.Width -= WizardAeroConsts.ContentLeftMargin + WizardAeroConsts.ContentRightMargin;
				}
				int height = GetHeaderTitleBounds(page).Height;
				rect.Y += height;
				rect.Height -= height + (Wizard.AllowPagePadding ? WizardAeroConsts.ContentBottomMargin : 0);
				return rect;
			}
			public virtual Rectangle GetHeaderTitleBounds() {
				return GetHeaderTitleBounds(Wizard.SelectedPage);
			}
			public virtual Rectangle GetHeaderTitleBounds(BaseWizardPage page) {
				Rectangle rect = GetContentBounds();
				rect.X += WizardAeroConsts.ContentLeftMargin;
				rect.Width -= WizardAeroConsts.ContentLeftMargin + WizardAeroConsts.ContentRightMargin;
				string text = (page == null ? "" : page.Text);
				int height = CalcTextSize(text, ViewInfo.PaintAppearance.PageTitle, rect.Width).Height;
				height += 2 * WizardAeroConsts.HeaderMargin;
				rect.Height = height;
				return rect;
			}
			public virtual Rectangle GetTitleBarBounds() {
				Rectangle rect = ViewInfo.WizardControl.ClientRectangle;
				rect.Height = TitleBarHeight;
				return rect;
			}
			public virtual Rectangle GetBackButtonBounds() {
				Rectangle rect = GetTitleBarBounds();
				int v = TitleBarHeight - TitleBarButtonSize;
				int h = GetHorzOffset(v);
				rect.Width = TitleBarButtonSize + h;
				rect.Inflate(-h / 2, -v / 2);
				return rect;
			}
			int GetHorzOffset(int defOffset) {
				if(ViewInfo.IsAeroEnabled() || ViewInfo.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return defOffset;
				SkinElement element = CommonSkins.GetSkin(ViewInfo.LookAndFeel)[CommonSkins.SkinBackButton];
				if(element == null) return defOffset;
				return element.Offset.Offset.X;
			}
			public virtual Rectangle GetTitleBarIconBounds() {
				Rectangle rect = GetTitleBarBounds();
				rect.X = GetBackButtonBounds().Right + WizardAeroConsts.TitleBarElementSpacing;
				rect.Width = WizardAeroConsts.TitleBarIconSize;
				int d = TitleBarHeight - WizardAeroConsts.TitleBarIconSize;
				rect.Inflate(0, -d / 2);
				return rect;
			}
			public virtual Rectangle GetTitleBarTitleTextBounds(bool iconVisible) {
				Rectangle rect = GetTitleBarBounds();
				Rectangle res = rect;
				int right = iconVisible ? GetTitleBarIconBounds().Right : GetBackButtonBounds().Right;
				res.X = right + WizardAeroConsts.TitleBarElementSpacing;
				res.Width = rect.Right - res.X;
				return res;
			}
			int GetCommandButtonTopLocation(ButtonInfo button) {
				return ViewInfo.WizardControl.ClientRectangle.Bottom - Wizard.GetScaleHeight(CommandAreaHeight) / 2 - button.Size.Height / 2;
			}
			public override void UpdateButtonsLocation() {
				ViewInfo.HelpButton.Location = new Point(ViewInfo.WizardControl.ClientRectangle.Width - WizardAeroConsts.ContentRightMargin - ViewInfo.HelpButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.HelpButton));
				ViewInfo.CancelButton.Location = new Point(ViewInfo.WizardControl.ClientRectangle.Width - WizardAeroConsts.ContentRightMargin - ViewInfo.CancelButton.Size.Width - (ViewInfo.HelpButton.Visible ? ViewInfo.HelpButton.Size.Width + WizardAeroConsts.CommandButtonSpacing : 0), GetCommandButtonTopLocation(ViewInfo.CancelButton));
				ViewInfo.NextButton.Location = new Point(ViewInfo.CancelButton.Location.X - WizardAeroConsts.CommandButtonSpacing - ViewInfo.NextButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.NextButton));
				ViewInfo.FinishButton.Location = new Point(ViewInfo.CancelButton.Location.X - WizardAeroConsts.CommandButtonSpacing - ViewInfo.FinishButton.Size.Width, GetCommandButtonTopLocation(ViewInfo.FinishButton));
				ViewInfo.BackButton.Bounds = GetBackButtonBounds();
			}
		}
		public virtual bool DrawHtmlText { get { return WizardControl.AllowHtmlText; } }
		protected internal int GetMaxButtonHeight() {
			int maxButtonHeight = NextButton.Size.Height;
			maxButtonHeight = Math.Max(maxButtonHeight, PrevButton.Size.Height);
			maxButtonHeight = Math.Max(maxButtonHeight, CancelButton.Size.Height);
			maxButtonHeight = Math.Max(maxButtonHeight, FinishButton.Size.Height);
			return maxButtonHeight;
		}
		protected internal int GetBackButtonHeight() {
			return BackButton.Size.Height;
		}
	}
	public class AppearanceName {
		public static readonly string ExteriorPageTitle = "ExteriorPageTitle";
		public static readonly string ExteriorPage = "ExteriorPage";
		public static readonly string PageTitle = "PageTitle";
		public static readonly string Page = "Page";
		public static readonly string AeroWizardTitle = "AeroWizardTitle";
	}
	public class WizardAppearanceCollection : BaseAppearanceCollection {
		AppearanceObject exteriorPageTitle, exteriorPage, pageTitle, page, aeroWizardTitle;
		IAppearanceOwner wizardControl;
		protected internal WizardAppearanceCollection(IAppearanceOwner wizardControl) {
			this.wizardControl = wizardControl;
		}
		public override bool IsLoading { get { return wizardControl.IsLoading; } }
		protected override AppearanceObject CreateNullAppearance() { return null; }
		protected override void CreateAppearances() {
			this.exteriorPageTitle = CreateAppearance(AppearanceName.ExteriorPageTitle);
			this.exteriorPage = CreateAppearance(AppearanceName.ExteriorPage);
			this.pageTitle = CreateAppearance(AppearanceName.PageTitle);
			this.page = CreateAppearance(AppearanceName.Page);
			this.aeroWizardTitle = CreateAppearance(AppearanceName.AeroWizardTitle);
		}
		void ResetExteriorPageTitle() { ExteriorPageTitle.Reset(); }
		bool ShouldSerializeExteriorPageTitle() { return ExteriorPageTitle.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ExteriorPageTitle { get { return exteriorPageTitle; } }
		void ResetExteriorPage() { ExteriorPage.Reset(); }
		bool ShouldSerializeExteriorPage() { return ExteriorPage.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ExteriorPage { get { return exteriorPage; } }
		void ResetPageTitle() { PageTitle.Reset(); }
		bool ShouldSerializePageTitle() { return PageTitle.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PageTitle { get { return pageTitle; } }
		void ResetPage() { Page.Reset(); }
		bool ShouldSerializePage() { return Page.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Page { get { return page; } }
		void ResetAeroWizardTitle() { AeroWizardTitle.Reset(); }
		bool ShouldSerializeAeroWizardTitle() { return AeroWizardTitle.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AeroWizardTitle { get { return aeroWizardTitle; } }
	}
}
