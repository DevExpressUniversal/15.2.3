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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Docking {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DockManagerAppearances : IDisposable, IAppearanceOwner {
		AppearanceObject panel, panelCaptionActive, panelCaption, floatFormCaptionActive, floatFormCaption, tabs, activeTab, hideContainer, hidePanelButtonActive, hidePanelButton;
		int lockUpdate;
		BarAndDockingController controller;
		public DockManagerAppearances() : this(null) {}
		public DockManagerAppearances(BarAndDockingController controller) {
			this.panel = CreateAppearance();
			this.panelCaptionActive = CreateAppearance();
			this.panelCaption = CreateAppearance();
			this.floatFormCaptionActive = CreateAppearance();
			this.floatFormCaption = CreateAppearance();
			this.tabs = CreateAppearance();
			this.activeTab = CreateAppearance();
			this.hideContainer = CreateAppearance();
			this.hidePanelButtonActive = CreateAppearance();
			this.hidePanelButton = CreateAppearance();
			this.lockUpdate = 0;
			this.controller = controller;
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				ResetPanel();
				ResetPanelCaptionActive();
				ResetPanelCaption();
				ResetFloatFormCaptionActive();
				ResetFloatFormCaption();
				ResetTabs();
				ResetActiveTab();
				ResetHideContainer();
				ResetHidePanelButtonActive();
				ResetHidePanelButton();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void ResetPanel() { Panel.Reset(); }
		protected virtual void ResetPanelCaptionActive() { PanelCaptionActive.Reset(); }
		protected virtual void ResetPanelCaption() { PanelCaption.Reset(); }
		protected virtual void ResetFloatFormCaptionActive() { FloatFormCaptionActive.Reset(); }
		protected virtual void ResetFloatFormCaption() { FloatFormCaption.Reset(); }
		protected virtual void ResetTabs() { Tabs.Reset(); }
		protected virtual void ResetActiveTab() { ActiveTab.Reset(); }
		protected virtual void ResetHideContainer() { HideContainer.Reset(); }
		protected virtual void ResetHidePanelButtonActive() { HidePanelButtonActive.Reset(); }
		protected virtual void ResetHidePanelButton() { HidePanelButton.Reset(); }
		bool IAppearanceOwner.IsLoading { get { return Controller == null || Controller.IsLoading; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarAndDockingController Controller { get { return controller; } }
		public virtual void Dispose() {
			DestroyAppearance(this.panel);
			DestroyAppearance(this.panelCaptionActive);
			DestroyAppearance(this.panelCaption);
			DestroyAppearance(this.floatFormCaptionActive);
			DestroyAppearance(this.floatFormCaption);
			DestroyAppearance(this.tabs);
			DestroyAppearance(this.activeTab);
			DestroyAppearance(this.hideContainer);
			DestroyAppearance(this.hidePanelButtonActive);
			DestroyAppearance(this.hidePanelButton);
		}
		public void BeginUpdate() {
			this.lockUpdate ++;
		}
		public void EndUpdate() {
			if(--this.lockUpdate == 0) {
				OnAppearanceChanged(this, EventArgs.Empty);
			}
		}
		bool ShouldSerializePanel() { return Panel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Panel { get { return panel; } }
		bool ShouldSerializePanelCaptionActive() { return PanelCaptionActive.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesPanelCaptionActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PanelCaptionActive { get { return panelCaptionActive; } }
		bool ShouldSerializePanelCaption() { return PanelCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesPanelCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PanelCaption { get { return panelCaption; } }
		bool ShouldSerializeFloatFormCaptionActive() { return FloatFormCaptionActive.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesFloatFormCaptionActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FloatFormCaptionActive { get { return floatFormCaptionActive; } }
		bool ShouldSerializeFloatFormCaption() { return FloatFormCaption.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesFloatFormCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject FloatFormCaption { get { return floatFormCaption; } }
		bool ShouldSerializeTabs() { return Tabs.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesTabs"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Tabs { get { return tabs; } }
		bool ShouldSerializeActiveTab() { return ActiveTab.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesActiveTab"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ActiveTab { get { return activeTab; } }
		bool ShouldSerializeHideContainer() { return HideContainer.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesHideContainer"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HideContainer { get { return hideContainer; } }
		bool ShouldSerializeHidePanelButtonActive() { return HidePanelButtonActive.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesHidePanelButtonActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HidePanelButtonActive { get { return hidePanelButtonActive; } }
		bool ShouldSerializeHidePanelButton() { return HidePanelButton.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAppearancesHidePanelButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HidePanelButton { get { return hidePanelButton; } }
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.Dispose();
		}
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject appearance = new AppearanceObject(this, true);
			appearance.Changed += new EventHandler(OnAppearanceChanged);
			return appearance;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			if(this.lockUpdate != 0 || Controller == null || Controller.IsLoading) return;
			Controller.OnChanged();
		}
	}
	public class DockElementsParameters  {
		public void InitPanelAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, AppearanceObject defaultAppearance, bool isFloat) {
			AppearanceDefault apDefault = CreateAppearanceDefault(
					isFloat ? FloatFormForeColor : PanelForeColor,
					isFloat ? FloatFormBackColor : PanelBackColor,
					isFloat ? FloatFormBackColor2 : PanelBackColor2,
					isFloat ? FloatFormGradientMode : PanelGradientMode, Color.Empty
				);
			InitControlAppearance(controlAppearance, mainAppearance, defaultAppearance, apDefault);
		}
		public void InitWindowCaptionAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, bool active) {
			Color textColor = (active ? PanelCaptionActiveForeColor : PanelCaptionForeColor);
			Color backColor = (active ? PanelCaptionActiveBackColor : PanelCaptionBackColor);
			AppearanceDefault captionAppearanceDefault = CreateAppearanceDefault(textColor, backColor, Color.Empty, LinearGradientMode.Horizontal, Color.Empty);
			InitCaptionAppearance(controlAppearance, mainAppearance, captionAppearanceDefault, false, active);
		}
		public void InitApplicationCaptionAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, bool active) {
			Color textColor = (active ? FloatFormCaptionActiveForeColor : FloatFormCaptionForeColor);
			Color backColor = (active ? FloatFormCaptionActiveBackColor : FloatFormCaptionBackColor);
			AppearanceDefault captionAppearanceDefault = CreateAppearanceDefault(textColor, backColor, Color.Empty, LinearGradientMode.Horizontal, Color.Empty);
			InitCaptionAppearance(controlAppearance, mainAppearance, captionAppearanceDefault, true, active);
		}
		public void InitTabsAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, TabsForeColor, TabsBackColor, TabsBackColor2, TabsBorderColor, position);
		}
		public void InitTabsAppearanceHot(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, TabsForeColorHot, TabsBackColor, TabsBackColor2, TabsBorderColor, position);
		}
		public void InitActiveTabAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, ActiveTabForeColor, ActiveTabBackColor, ActiveTabBackColor2, TabsBorderColor, position);
		}
		public void InitHideContainerAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, HideContainerForeColor, HideContainerBackColor, Color.Empty, Color.Empty, position);
		}
		public void InitHidePanelButtonActiveAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, HidePanelButtonActiveForeColor, HidePanelButtonActiveBackColor, HidePanelButtonActiveBackColor2, HidePanelButtonBorderColor, position);
		}
		public void InitHidePanelButtonAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, TabsPosition position) {
			InitTabAppearance(controlAppearance, mainAppearance, HidePanelButtonForeColor, HidePanelButtonBackColor, HidePanelButtonBackColor2, HidePanelButtonBorderColor, position);
		}
		void InitTabAppearance(AppearanceObject tabAppearance, AppearanceObject mainAppearance, Color textColor, Color backColor1, Color backColor2, Color borderColor, TabsPosition position) {
			if(DockLayoutUtils.IsHead(position) && !backColor2.IsEmpty) {
				Color tmp = backColor1;
				backColor1 = backColor2;
				backColor2 = tmp;
			}
			AppearanceDefault apDefault = CreateAppearanceDefaultTab(textColor, backColor1, backColor2, borderColor, position);
			InitControlAppearance(tabAppearance, mainAppearance, tabAppearance, apDefault);
		}
		protected virtual AppearanceDefault CreateAppearanceDefaultTab(Color textColor, Color backColor1, Color backColor2, Color borderColor, TabsPosition position) {
			return CreateAppearanceDefault(textColor, backColor1, backColor2, DockLayoutUtils.IsVerticalPosition(position) ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical, borderColor);
		}
		protected virtual void InitCaptionAppearance(AppearanceObject captionAppearance, AppearanceObject mainAppearance, AppearanceDefault appearanceDefault, bool application, bool active) {
			InitControlAppearance(captionAppearance, mainAppearance, captionAppearance, appearanceDefault);
		}
		protected virtual void InitControlAppearance(AppearanceObject controlAppearance, AppearanceObject mainAppearance, AppearanceObject defaultAppearance, AppearanceDefault apDefault) {
			AppearanceHelper.Combine(controlAppearance, new AppearanceObject[]  {mainAppearance, defaultAppearance}, apDefault);
		}
		protected AppearanceDefault CreateAppearanceDefault(Color textColor, Color backColor, Color backColor2, LinearGradientMode gradientMode, Color borderColor) {
			return new AppearanceDefault(textColor, backColor, borderColor, backColor2, gradientMode);
		}
		public virtual Color GetBorderPen() {
			return PanelBackColor;
		}
		protected virtual Color PanelBackColor { get { return SystemColors.Control; } }
		protected virtual Color PanelForeColor { get { return SystemColors.WindowText; } }
		protected virtual Color PanelBackColor2 { get { return Color.Empty; } }
		protected virtual LinearGradientMode PanelGradientMode { get { return LinearGradientMode.Vertical; } }
		protected virtual Color FloatFormBackColor { get { return SystemColors.Control; } }
		protected virtual Color FloatFormForeColor { get { return SystemColors.WindowText; } }
		protected virtual Color FloatFormBackColor2 { get { return Color.Empty; } }
		protected virtual LinearGradientMode FloatFormGradientMode { get { return LinearGradientMode.Vertical; } }
		protected virtual Color PanelCaptionActiveBackColor { get { return FloatFormCaptionActiveBackColor; } }
		protected virtual Color PanelCaptionActiveForeColor { get { return FloatFormCaptionActiveForeColor; } }
		protected virtual Color PanelCaptionBackColor { get { return FloatFormCaptionBackColor; } }
		protected virtual Color PanelCaptionForeColor { get { return FloatFormCaptionForeColor; } }
		protected virtual Color FloatFormCaptionActiveBackColor { get { return SystemColors.ActiveCaption; } }
		protected virtual Color FloatFormCaptionActiveForeColor { get { return SystemColors.ActiveCaptionText; } }
		protected virtual Color FloatFormCaptionBackColor { get { return SystemColors.InactiveCaption; } }
		protected virtual Color FloatFormCaptionForeColor { get { return SystemColors.InactiveCaptionText; } }
		protected virtual Color TabsBackColor { get { return SystemColors.Control; } }
		protected virtual Color TabsBackColor2 { get { return Color.Empty; } }
		protected virtual Color TabsForeColor { get { return SystemColors.WindowText; } }
		protected virtual Color TabsForeColorHot { get { return SystemColors.WindowText; } }
		protected virtual Color TabsBorderColor { get { return Color.Empty; } }
		protected virtual Color ActiveTabBackColor { get { return SystemColors.Control; } }
		protected virtual Color ActiveTabBackColor2 { get { return Color.Empty; } }
		protected virtual Color ActiveTabForeColor { get { return SystemColors.WindowText; } }
		protected virtual Color HideContainerBackColor { get { return TabsBackColor; } }
		protected virtual Color HideContainerForeColor { get { return TabsForeColor; } }
		protected virtual Color HidePanelButtonActiveBackColor { get { return ActiveTabBackColor; } }
		protected virtual Color HidePanelButtonActiveBackColor2 { get { return ActiveTabBackColor2; } }
		protected virtual Color HidePanelButtonActiveForeColor { get { return ActiveTabForeColor; } }
		protected virtual Color HidePanelButtonBorderColor { get { return TabsBorderColor; } }
		protected virtual Color HidePanelButtonBackColor { get { return HidePanelButtonActiveBackColor; } }
		protected virtual Color HidePanelButtonBackColor2 { get { return HidePanelButtonActiveBackColor2; } }
		protected virtual Color HidePanelButtonForeColor { get { return HidePanelButtonActiveForeColor; } }
	}
	public class DockElementsOffice2000Parameters : DockElementsParameters {
		protected override Color PanelCaptionBackColor { get { return SystemColors.Control; } }
		protected override Color PanelCaptionForeColor { get { return SystemColors.WindowText; } }
		protected override Color TabsBorderColor { get { return Color.Black; } }
		protected override Color TabsBackColor { get { return DevExpress.Utils.ColorUtils.FlatTabBackColor; } }
		protected override Color TabsForeColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color ActiveTabForeColor { get { return SystemColors.ControlText; } }
		protected override Color HidePanelButtonActiveForeColor { get { return SystemColors.ControlDarkDark; } }
		protected override Color HidePanelButtonBorderColor { get { return SystemColors.ControlDark; } }
	}
	public class DockElementsOffice2003Parameters : DockElementsParameters {
		protected override Color PanelBackColor { get { return Office2003Colors.Default[Office2003Color.Button1]; } }
		protected override Color PanelBackColor2 { get { return Office2003Colors.Default[Office2003Color.Header]; } }
		protected override Color PanelCaptionBackColor { get { return FloatFormCaptionBackColor; } }
		protected override Color TabsForeColor { get { return SystemColors.ControlText; } }
		protected override Color TabsBackColor { get { return Office2003Colors.Default[Office2003Color.TabBackColor1]; } }
		protected override Color TabsBackColor2 { get { return Office2003Colors.Default[Office2003Color.TabBackColor2]; } }
		protected override Color TabsBorderColor { get { return Office2003Colors.Default[Office2003Color.Border]; } }
		protected override Color ActiveTabForeColor { get { return SystemColors.ControlText; } }
		protected override Color ActiveTabBackColor { get { return Office2003Colors.Default[Office2003Color.TabPageBackColor1]; } }
		protected override Color ActiveTabBackColor2 { get { return Office2003Colors.Default[Office2003Color.TabPageBackColor2]; } }
		protected override Color HidePanelButtonBackColor { get { return Office2003Colors.Default[Office2003Color.Button1]; } }
		protected override Color HidePanelButtonBackColor2 { get { return Office2003Colors.Default[Office2003Color.Button2]; } }
	}
	public class DockElementsOfficeXPParameters : DockElementsOffice2000Parameters {}
	public class DockElementsWhidbeyParameters : DockElementsOffice2000Parameters {
		protected override Color ActiveTabBackColor { get { return SystemColors.Window; } }
		protected override Color TabsBorderColor { get { return SystemColors.ControlDark; } }
		protected override Color PanelCaptionBackColor { get { return FloatFormCaptionBackColor; } }
		protected override Color PanelCaptionForeColor { get { return FloatFormCaptionForeColor; } }
	}
	public class DockElementsWindowsXPParameters : DockElementsParameters {
		protected override Color PanelBackColor { get { return SystemColors.Window; } }
		protected override Color PanelCaptionActiveForeColor { get { return SystemColors.WindowText; } }
		protected override Color PanelCaptionForeColor { get { return PanelCaptionActiveForeColor; } }
		protected override Color HidePanelButtonBorderColor { get { return WXPPainter.Default.GetThemeColor(new WXPPainterArgs("tab", XPConstants.TABP_TABITEMRIGHTEDGE, XPConstants.TIRES_NORMAL), WXPPainter.XP_TMT_EDGESHADOWCOLOR); } }
	}
	public class DockElementsSkinParameters : DockElementsParameters {
		ISkinProvider skinProvider;
		public DockElementsSkinParameters(ISkinProvider skinProvider) { this.skinProvider = skinProvider; }
		protected override Color PanelBackColor { get { return SkinColor(DockingSkins.SkinDockWindowBorder).GetBackColor(); } }
		protected override Color PanelForeColor { get { return SkinColor(DockingSkins.SkinDockWindowBorder).GetForeColor(); } }
		protected override Color PanelBackColor2 { get { return SkinColor(DockingSkins.SkinDockWindowBorder).GetBackColor2(); } }
		protected override LinearGradientMode PanelGradientMode { get { return SkinColor(DockingSkins.SkinDockWindowBorder).GradientMode; } }
		protected override Color FloatFormBackColor { get { return SkinColor(DockingSkins.SkinFloatingWindowBorder).GetBackColor(); } }
		protected override Color FloatFormForeColor { get { return SkinColor(DockingSkins.SkinFloatingWindowBorder).GetForeColor(); } }
		protected override Color FloatFormBackColor2 { get { return SkinColor(DockingSkins.SkinFloatingWindowBorder).GetBackColor2(); } }
		protected override LinearGradientMode FloatFormGradientMode { get { return SkinColor(DockingSkins.SkinFloatingWindowBorder).GradientMode; } }
		protected override Color PanelCaptionActiveForeColor { get { return SkinColor(DockingSkins.SkinDockWindow).GetForeColor(); } }
		protected override Color PanelCaptionForeColor { get { return SkinColor(DockingSkins.SkinDockWindow).GetForeColor(); } }
		protected override Color FloatFormCaptionActiveForeColor { get { return SkinColor(DockingSkins.SkinFloatingWindow).GetForeColor(); } }
		protected override Color FloatFormCaptionForeColor { get { return SkinColor(DockingSkins.SkinFloatingWindow).GetForeColor(); } }
		protected override Color TabsForeColor { get { return SkinTabForeColor(TabSkinProperties.TabHeaderTextColor); } }
		protected override Color TabsForeColorHot { get { return SkinTabForeColor(TabSkinProperties.TabHeaderTextColorHot); } }
		protected override Color ActiveTabForeColor { get { return SkinTabForeColor(TabSkinProperties.TabHeaderTextColorActive); } }
		protected override Color HidePanelButtonForeColor {
			get { return SkinHideBarForeColor(DockingSkins.HideBarTextColor, TabSkinProperties.TabHeaderTextColor, base.HidePanelButtonForeColor); }
		}
		protected override Color HidePanelButtonActiveForeColor {
			get { return SkinHideBarForeColor(DockingSkins.HideBarTextColorActive, TabSkinProperties.TabHeaderTextColorActive, base.HidePanelButtonActiveForeColor); }
		}
		protected virtual ISkinProvider SkinProvider { get { return skinProvider; } }
		protected Skin Skin { get { return DockingSkins.GetSkin(SkinProvider); } }
		protected SkinColor SkinColor(string skinName) { return Skin[skinName].Color; }
		protected Color SkinTabForeColor(string skinName) { return Skin.Colors[skinName]; }
		protected Color SkinHideBarForeColor(string skinName, string tabSkinName, Color defaultColor) {
			return Skin.Colors.GetColor(skinName, tabSkinName, defaultColor);
		}
		protected override void InitCaptionAppearance(AppearanceObject captionAppearance, AppearanceObject mainAppearance, AppearanceDefault captionAppearanceDefault, bool application, bool active) {
			SkinElement element = application ? GetApplicationSkin() : GetWindowSkin();
			if(element != null) {
				element.Apply(captionAppearanceDefault);
				if(!active && mainAppearance.ForeColor == Color.Empty) {
					Color unfocusColor = GetUnfocusCaptionColor();
					if(unfocusColor != Color.Empty) 
						captionAppearanceDefault.ForeColor = unfocusColor;
				}
			}
			base.InitCaptionAppearance(captionAppearance, mainAppearance, captionAppearanceDefault, application, active);
		}
		protected override AppearanceDefault CreateAppearanceDefaultTab(Color textColor, Color backColor1, Color backColor2, Color borderColor, TabsPosition position) {
			AppearanceDefault apDefault = base.CreateAppearanceDefaultTab(textColor, backColor1, backColor2, borderColor, position);
			SkinElement element = GetTabSkin();
			if(element != null)
				element.ApplyFontSize(apDefault);
			return apDefault;
		}
		protected virtual Color GetUnfocusCaptionColor() {
			SkinElement element = GetWindowSkin();
			if(element != null)
				return element.Properties.GetColor(DockingSkins.SkinUnFocusCaptionColor);
			return Color.Empty;
		}
		SkinElement GetWindowSkin() { return Skin[DockingSkins.SkinDockWindow]; }
		SkinElement GetApplicationSkin() { return Skin[DockingSkins.SkinFloatingWindow]; }
		SkinElement GetTabSkin() { return Skin[DockingSkins.SkinTabHeader]; }
	}
}
