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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum LargeImageLoadMode { Direct, OnPageLoad, OnFirstShow }
	public enum ZoomWindowPosition { Right, Bottom, Left, Top, Inside }
	public enum MouseBoxOpacityMode { Inside, Outside }
	[DXWebToolboxItem(true), DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxImageZoom"),
	Designer("DevExpress.Web.Design.ASPxImageZoomDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxImageZoom.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	public class ASPxImageZoom : ASPxWebControl {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "ImageZoom.js";
		private ImageZoomZoomModeSettings settingsZoomMode = null;
		private ImageZoomZoomWindowStyles stylesZoomWindow = null;
		private ImageZoomExpandWindowStyles stylesExpandWindow = null;
		private ImageZoomZoomWindowImages imagesZoomWindow = null;
		private ImageZoomExpandWindowImages imagesExpandWindow = null;
		private ASPxImageZoomNavigator associatedImageZoomNavigator = null;
		private bool isChangedImageZoomNavigator = false;
		public ASPxImageZoom()
			: base() {
		}
		public ASPxImageZoom(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected new internal string GetClientInstanceName() {
			return base.GetClientInstanceName();
		}
		protected override void CreateControlHierarchy() {
			if(HasImageZoomNavigator()) {
				ImageUrl = string.Empty;
				LargeImageUrl = string.Empty;
				ExpandWindowText = string.Empty;
				ZoomWindowText = string.Empty;
			}
			Controls.Add(CreateImageZoomControl());
			base.CreateControlHierarchy();
		}
		protected ImageZoomControlBase CreateImageZoomControl() {
			return DesignMode ? new ImageZoomControlDesignMode(this) as ImageZoomControlBase : new ImageZoomControl(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomImageUrl"),
#endif
		DefaultValue(""), Localizable(false), Bindable(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string ImageUrl {
			get { return GetStringProperty("ImageUrl", string.Empty); }
			set {
				SetStringProperty("ImageUrl", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomLargeImageUrl"),
#endif
		DefaultValue(""), Localizable(false), Bindable(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string LargeImageUrl {
			get { return GetStringProperty("LargeImageUrl", string.Empty); }
			set {
				SetStringProperty("LargeImageUrl", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomLargeImageLoadMode"),
#endif
		DefaultValue(LargeImageLoadMode.Direct), Localizable(false), AutoFormatDisable]
		public LargeImageLoadMode LargeImageLoadMode {
			get { return (LargeImageLoadMode)GetEnumProperty("LargeImageLoadMode", LargeImageLoadMode.Direct); }
			set { SetEnumProperty("LargeImageLoadMode", LargeImageLoadMode.Direct, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomZoomWindowText"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty, Localizable(false)]
		public string ZoomWindowText {
			get { return GetStringProperty("ZoomWindowText", string.Empty); }
			set {
				SetStringProperty("ZoomWindowText", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomExpandWindowText"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty, Localizable(false)]
		public string ExpandWindowText {
			get { return GetStringProperty("ExpandWindowText", string.Empty); }
			set {
				SetStringProperty("ExpandWindowText", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomShowHint"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty]
		public bool ShowHint {
			get { return GetBoolProperty("ShowHint", true); }
			set {
				SetBoolProperty("ShowHint", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomShowHintText"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty]
		public bool ShowHintText {
			get { return GetBoolProperty("ShowHintText", true); }
			set {
				SetBoolProperty("ShowHintText", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomHintText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string HintText {
			get { return GetStringProperty("HintText", ""); }
			set { SetStringProperty("HintText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomEnableZoomMode"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty]
		public bool EnableZoomMode {
			get { return GetBoolProperty("EnableZoomMode", true); }
			set {
				SetBoolProperty("EnableZoomMode", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomEnableExpandMode"),
#endif
		DefaultValue(true), AutoFormatEnable, AutoFormatCannotBeEmpty]
		public bool EnableExpandMode {
			get { return GetBoolProperty("EnableExpandMode", true); }
			set {
				SetBoolProperty("EnableExpandMode", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomSettingsZoomMode"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ImageZoomZoomModeSettings SettingsZoomMode {
			get {
				if(settingsZoomMode == null)
					settingsZoomMode = new ImageZoomZoomModeSettings(this);
				return settingsZoomMode;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomStylesZoomWindow"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageZoomZoomWindowStyles StylesZoomWindow {
			get {
				if(stylesZoomWindow == null)
					stylesZoomWindow = new ImageZoomZoomWindowStyles(this);
				return stylesZoomWindow;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomStylesExpandWindow"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageZoomExpandWindowStyles StylesExpandWindow {
			get {
				if(stylesExpandWindow == null)
					stylesExpandWindow = new ImageZoomExpandWindowStyles(this);
				return stylesExpandWindow;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageZoomStyles Styles {
			get { return StylesInternal as ImageZoomStyles; }
		}
		protected override StylesBase CreateStyles() {
			return new ImageZoomStyles(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get {
				return base.DisabledStyle;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get {
				return base.BackgroundImage;
			}
		}
		protected internal virtual string GetClipPanelCssClassName() {
			string className = ImageZoomStyles.ClipPanelClassName;
			if(SettingsZoomMode.ZoomWindowPosition == ZoomWindowPosition.Inside)
				className += " " + ImageZoomStyles.InsideClipPanelClassName;
			return className;
		}
		protected internal ButtonStyle GetDefaultCloseButtonStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(StylesExpandWindow.GetDefaultCloseButtonStyle());
			style.CopyFrom(StylesExpandWindow.CloseButton);
			return style;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomAlternateText"),
#endif
		DefaultValue(""), Category("Accessibility"), AutoFormatDisable, Localizable(true)]
		public string AlternateText {
			get { return GetStringProperty("AlternateText", String.Empty); }
			set { 
				SetStringProperty("AlternateText", String.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomAssociatedImageZoomNavigatorID"),
#endif
		Category("Accessibility"), Themeable(false), TypeConverter(typeof(AssociatedControlConverter)),
		IDReferenceProperty, DefaultValue(""), Localizable(false), AutoFormatDisable]
		public virtual string AssociatedImageZoomNavigatorID {
			get { return GetStringProperty("AssociatedImageZoomNavigatorID", ""); }
			set {
				SetStringProperty("AssociatedImageZoomNavigatorID", "", value);
				LayoutChanged();
				isChangedImageZoomNavigator = true;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomImagesZoomWindow"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageZoomZoomWindowImages ImagesZoomWindow {
			get {
				if(imagesZoomWindow == null)
					imagesZoomWindow = new ImageZoomZoomWindowImages(this);
				return imagesZoomWindow;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomImagesExpandWindow"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageZoomExpandWindowImages ImagesExpandWindow {
			get {
				if(imagesExpandWindow == null)
					imagesExpandWindow = new ImageZoomExpandWindowImages(this);
				return imagesExpandWindow;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageZoomImages Images {
			get { return ImagesInternal as ImageZoomImages; }
		}
		protected override ImagesBase CreateImages() {
			return new ImageZoomImages(this);
		}
		protected virtual internal ImageProperties GetHintImage() {
			ImageProperties sprite = new ImageProperties();
			sprite.MergeWith(Images.GetImageProperties(Page, ImageZoomImages.HintImageName));
			sprite.MergeWith(Images.Hint);
			return sprite;
		}
		protected virtual internal ButtonImageProperties GetCloseButtonImage() {
			ButtonImageProperties sprite = new ButtonImageProperties();
			sprite.MergeWith(ImagesExpandWindow.GetImageProperties(Page, ImageZoomExpandWindowImages.ExpandWindowCloseButtonImageName));
			sprite.MergeWith(ImagesExpandWindow.CloseButton);
			return sprite;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { SettingsZoomMode, StylesExpandWindow, ImagesExpandWindow, StylesZoomWindow, ImagesZoomWindow }));
			return list.ToArray();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			if(EnableExpandMode)
				helper.AddStyle(GetDefaultCloseButtonStyle().HoverStyle, ImageZoomContstants.CloseButtonIDForScript, new string[0],
					GetCloseButtonImage().GetHottrackedScriptObject(Page), string.Empty, IsEnabled());
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			if(EnableExpandMode)
				helper.AddStyle(GetDefaultCloseButtonStyle().PressedStyle, ImageZoomContstants.CloseButtonIDForScript, new string[0],
					GetCloseButtonImage().GetPressedScriptObject(Page), string.Empty, IsEnabled());
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			if(EnableExpandMode)
				helper.AddStyle(GetDefaultCloseButtonStyle().DisabledStyle, ImageZoomContstants.CloseButtonIDForScript, new string[0],
					GetCloseButtonImage().GetDisabledScriptObject(Page), string.Empty, IsEnabled());
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageZoomClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ClientSideEvents ClientSideEvents {
			get { return (ClientSideEvents)ClientSideEventsInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientImageZoom";
		}
		protected override bool HasHoverScripts() {
			return EnableExpandMode;
		}
		protected override bool HasPressedScripts() {
			return EnableExpandMode;
		}
		protected override bool HasDisabledScripts() {
			return EnableExpandMode;
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override bool IsAnimationScriptNeeded() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CanCreateLargeImageOnClient()) {
				stb.AppendFormat("{0}.largeImageLoadMode = {1};\n", localVarName, (int)LargeImageLoadMode);
				stb.AppendFormat("{0}.largeImageUrl = '{1}';\n", localVarName, ResolveUrl(LargeImageUrl));
			}
			if(EnableZoomMode) {
				if(SettingsZoomMode.MouseBoxOpacityMode == MouseBoxOpacityMode.Outside)
					stb.AppendFormat("{0}.mouseBoxOpacityMode = {1};\n", localVarName, (int)SettingsZoomMode.MouseBoxOpacityMode);
				Unit DefaultPersentValue = new Unit(ImageZoomZoomModeSettings.DefaultZoomAreaSize);
				if(!SettingsZoomMode.ZoomWindowWidth.IsEmpty && SettingsZoomMode.ZoomWindowWidth != DefaultPersentValue)
					stb.AppendFormat("{0}.zoomWindowWidth = {1};\n", localVarName, ConvertUnitValueToClientValue(SettingsZoomMode.ZoomWindowWidth));
				if(!SettingsZoomMode.ZoomWindowHeight.IsEmpty && SettingsZoomMode.ZoomWindowHeight != DefaultPersentValue)
					stb.AppendFormat("{0}.zoomWindowHeight = {1};\n", localVarName, ConvertUnitValueToClientValue(SettingsZoomMode.ZoomWindowHeight));
				if(SettingsZoomMode.ZoomWindowPosition == ZoomWindowPosition.Inside)
					stb.AppendFormat("{0}.zoomWindowPosition = 0;\n", localVarName);
			} else
				stb.AppendFormat("{0}.showZoomWindow = false;\n", localVarName);
			if(!EnableExpandMode)
				stb.AppendFormat("{0}.showExpandWindow = false;\n", localVarName);
			if(!ShowHint)
				stb.AppendFormat("{0}.showHint = false;\n", localVarName);
			if(HasImageZoomNavigator())
				stb.AppendFormat("{0}.imageZoomNavigatorName = '{1}';\n", localVarName, GetAssociatedImageZoomNavigatorID());
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterImageControlUtilsScript();
			RegisterIncludeScript(typeof(ASPxImageZoom), ScriptResourceName);
		}
		protected override bool HasContent() {
			return DesignMode || HasImageZoomNavigator() || !string.IsNullOrEmpty(ImageUrl);
		}
		protected string GetAssociatedImageZoomNavigatorID() {
			return IsMvcRender() ? AssociatedImageZoomNavigatorID : associatedImageZoomNavigator.ClientID;
		}
		protected internal bool HasImageZoomNavigator() {
			if(string.IsNullOrEmpty(AssociatedImageZoomNavigatorID))
				return false;
			if(isChangedImageZoomNavigator) {
				associatedImageZoomNavigator = (ASPxImageZoomNavigator)RenderUtils.GetReferentControl(this, AssociatedImageZoomNavigatorID, null);
				isChangedImageZoomNavigator = false;
			}
			return associatedImageZoomNavigator != null && associatedImageZoomNavigator.Visible;
		}
		protected internal bool CanCreateLargeImageOnClient() {
			return !string.IsNullOrEmpty(LargeImageUrl) && LargeImageLoadMode != LargeImageLoadMode.Direct;
		}
		protected override bool HasLoadingPanel() {
			return !DesignMode;
		}
		protected string ConvertUnitValueToClientValue(Unit value) {
			int val = (int)value.Value;
			return value.Type == UnitType.Percentage ? string.Format("'{0}%'", val) : val.ToString();
		}
		protected internal string GetHintText() {
			if(!string.IsNullOrEmpty(HintText))
				return HintText;
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.ImageZoom_HintText);
		}
	}
	internal class ImageZoomContstants {
		internal const string ImageID = "I";
		internal const string ZoomWindowID = "ZW";
		internal const string ExpandWindowID = "EW";
		internal const string CloseButtonID = "CB";
		static internal string CloseButtonIDForScript {
			get { return ExpandWindowID + "_" + CloseButtonID; }
		}
	}
}
