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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ASPxSpellChecker.Native;
using DevExpress.Utils;
using DevExpress.Web.ASPxSpellChecker.Forms;
using DevExpress.Web.ASPxSpellChecker.Internal;
using DevExpress.Web.ASPxSpellChecker.Native;
using DevExpress.Web.Internal;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.Web.Design;
using System.Collections.Specialized;
namespace DevExpress.Web.ASPxSpellChecker {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxSpellChecker"),
	Designer("DevExpress.Web.ASPxSpellChecker.Design.ASPxSpellCheckerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), "ASPxSpellChecker.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabComponents)
	]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724")]
	public class ASPxSpellChecker : ASPxWebControl, IParentSkinOwner, IControlDesigner {
		private static readonly object EventCheckedElementResolve = new object();
		protected internal string[] ChildControlNames = new string[] { "Web", "Editors" };
		protected internal static string[] FormNames = new string[] { SpellCheckFormName, SpellCheckOptionsFormName };
		protected internal const string SpellCheckerScriptResourceName = "Scripts.SpellChecker.js";
		protected internal const string SpellCheckerDialogsScriptResourceName = "Scripts.Dialogs.js";
		protected internal const string SpellCheckerCssResourceName = "Css.Default.css";
		protected internal const string SpellCheckerSystemCssResourceName = "Css.system.css";
		protected internal const string SpellCheckerImagesResourcePath = "Images.";
		protected internal const string SpellCheckerCheckTextPrefix = "Check";
		protected internal const string SpellCheckerStartIndexPrefix = "StartIndex";
		protected internal const string SpellCheckerAddWordPrefix = "Word";
		protected internal const string SpellCheckFormId = "SpellCheckForm";
		protected internal const string SpellCheckFormRenderResultPostFix = ":SCFPR:";
		protected internal const string SpellCheckOptionsFormId = "SpellCheckOptionsForm";
		protected internal const string SpellCheckFormName = SpellCheckFormId;
		protected internal const string SpellCheckOptionsFormName = SpellCheckOptionsFormId;
		protected internal const string SpellCheckSpanID = "SpellCheckSpan";
		protected internal const string SpellCheckSpanPostFix = ":SpellCheckSpan";
		protected internal const string SpellCheckSettingStateKey = "settings";
		protected internal const string SpellCheckSettingIgnoreUpperCaseWordsStateKey = "ignoreUpperCaseWords";
		protected internal const string SpellCheckSettingIgnoreMixedCaseWordsStateKey = "ignoreMixedCaseWords";
		protected internal const string SpellCheckSettingIgnoreWordsWithNumberStateKey = "ignoreWordsWithNumber";
		protected internal const string SpellCheckSettingIgnoreEmailsStateKey = "ignoreEmails";
		protected internal const string SpellCheckSettingIgnoreUrlsStateKey = "ignoreUrls";
		protected internal const string SpellCheckSettingIgnoreMarkupTagsStateKey = "ignoreMarkupTags";
		protected internal const string SpellCheckSettingCultureStateKey = "culture";
		WebSpellChecker spellChecker;
		ASPxSpellCheckerSpellingSettings settingsSpelling;
		ASPxSpellCheckerTextSettings settingsText;
		SpellCheckerFormsSettings settingsForms;
		SpellCheckerDialogSettings settingsDialogFormElements;
		List<int> startErrorPositionArray = null;
		List<string[]> suggestionsArray = null;
		List<int> wrongWordLengthArray = null;
		int errorCount = 0;
		bool spellCheckingHasBeenBroken;
		int textOffset = 0;
		SpellCheckerDictionaryCollection dictionaries;
		SpellCheckerCallbackReader callBackReader = null;
		WebControl errorWordSpan = null;
		WebControl mainElement = null;
		SpellCheckerPopupControl spellCheckForm = null;
		SpellCheckerPopupControl spellCheckOptionsForm = null;
		SpellCheckerEditorStyles stylesEditors = null;
		SpellCheckerEditorImages imagesEditors;
		SpellCheckerButtonStyles stylesButton = null;
		SpellCheckerDialogFormStyles stylesDialogForm = null;
		public ASPxSpellChecker()
			: base() {
			InitializeProperties();
		}
		protected ASPxSpellChecker(ASPxWebControl ownerControl)
			: base(ownerControl) {
			InitializeProperties();
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public SpellCheckerClientSideEvents ClientSideEvents {
			get { return (SpellCheckerClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerLevenshteinDistance"),
#endif
		Category("Settings"), DefaultValue(4), AutoFormatDisable]
		public int LevenshteinDistance {
			get { return GetIntProperty("LevenshteinDistance", 4); }
			set {
				CommonUtils.CheckNegativeValue(value, "LevenshteinDistance");
				SetIntProperty("LevenshteinDistance", 4, value);
			}
		}
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerMaximumErrorCountInResponse")]
#endif
		[Category("Settings"), DefaultValue(100), AutoFormatDisable]
		public int MaximumErrorCountInResponse {
			get { return GetIntProperty("MaximumErrorCountInResponse", 100); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "MaximumErrorCountInResponse");
				SetIntProperty("MaximumErrorCountInResponse", 100, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSuggestionCount"),
#endif
		Category("Settings"), DefaultValue(5), AutoFormatDisable]
		public int SuggestionCount {
			get { return GetIntProperty("SuggestionCount", 5); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "SuggestionCount");
				SetIntProperty("SuggestionCount", 5, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSettingsSpelling"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSpellCheckerSpellingSettings SettingsSpelling {
			get { return this.settingsSpelling; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSettingsText"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSpellCheckerTextSettings SettingsText {
			get { return settingsText; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSettingsForms"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerFormsSettings SettingsForms {
			get { return settingsForms; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSettingsDialogFormElements"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerDialogSettings SettingsDialogFormElements {
			get { return settingsDialogFormElements; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerDictionaries"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public SpellCheckerDictionaryCollection Dictionaries {
			get { return dictionaries; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCulture"),
#endif
		Category("Settings"), DefaultValue(null), AutoFormatDisable]
		public CultureInfo Culture {
			get { return (CultureInfo)GetObjectProperty("Culture", null); }
			set { SetObjectProperty("Culture", null, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCheckedElementID"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string CheckedElementID {
			get { return GetStringProperty("CheckedElementID", ""); }
			set { SetStringProperty("CheckedElementID", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft
		{
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AccessKey {
			get { return base.AccessKey; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color BackColor {
			get { return base.BackColor; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border {
			get { return base.Border; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom {
			get { return base.BorderBottom; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft {
			get { return base.BorderLeft; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight {
			get { return base.BorderRight; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop {
			get { return base.BorderTop; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssClass {
			get { return base.CssClass; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override short TabIndex {
			get { return base.TabIndex; }
		}
		[Browsable(false), Localizable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ToolTip {
			get { return base.ToolTip; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible {
			get { return base.Visible; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerDialogFormCloseButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties DialogFormCloseButtonImage {
			get { return ((SpellCheckerImages)ImagesInternal).DialogFormCloseButton; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerEditorImages ImagesEditors { get { return imagesEditors; } }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCheckedTextContainerStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public CheckedTextContainerStyle CheckedTextContainerStyle {
			get { return Styles.CheckedTextContainer; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerErrorWordStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ErrorWordStyle ErrorWordStyle {
			get { return Styles.ErrorWord; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return Styles.LoadingPanel; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerEditorStyles StylesEditors { get { return stylesEditors; } }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerStylesButton"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerButtonStyles StylesButton { get { return stylesButton; } }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerStylesDialogForm"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpellCheckerDialogFormStyles StylesDialogForm { get { return stylesDialogForm; } }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCheckedElementResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> CheckedElementResolve
		{
			add { Events.AddHandler(EventCheckedElementResolve, value); }
			remove { Events.RemoveHandler(EventCheckedElementResolve, value); }
		}
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		protected SpellCheckerStyles Styles {
			get { return (SpellCheckerStyles)StylesInternal; }
		}
		protected internal SpellCheckerStyles RenderStyles {
			get { return (SpellCheckerStyles)RenderStylesInternal; }
		}
		protected WebControl ErrorWordSpan {
			get { return errorWordSpan; }
		}
		protected WebControl MainElement {
			get { return mainElement; }
		}
		protected SpellCheckerPopupControl SpellCheckForm {
			get { return spellCheckForm; }
		}
		protected SpellCheckerPopupControl SpellCheckOptionsForm {
			get { return spellCheckOptionsForm; }
		}
		protected WebSpellChecker SpellChecker {
			get {
				if(spellChecker == null)
					spellChecker = CreateSpellChecker();
				return spellChecker;
			}
		}
		protected SpellCheckerCallbackReader CallBackReader {
			get { return callBackReader; }
			set { callBackReader = value; }
		}
		protected int ErrorCount {
			get { return errorCount; }
			set { errorCount = value; }
		}
		protected int TextOffset {
			get { return textOffset; }
			set { textOffset = value; }
		}
		public override void Dispose() {
			if(spellChecker != null)
				spellChecker.Dispose();
			base.Dispose();
		}
		public virtual CultureInfo GetCulture() {
			if(Culture == null)
				return CultureInfo.InvariantCulture;
			return Culture;
		}
		public SpellCheckerCustomDictionary GetCustomDictionary() {
			foreach(WebDictionary dictionary in Dictionaries)
				if(dictionary.IsCustomDictionary &&
					(CultureInfo.Equals(GetCulture(), dictionary.GetCulture()) || CultureInfo.InvariantCulture.Equals(dictionary.GetCulture()))) {
					SpellCheckerCustomDictionaryProvider provider = (SpellCheckerCustomDictionaryProvider)GetDictionaryProvider(dictionary);
					if(!provider.IsDictionaryStored) {
						ASPxSpellCheckerCustomDictionary dict = (ASPxSpellCheckerCustomDictionary)dictionary;
						CustomDictionaryLoadingEventArgs args = new CustomDictionaryLoadingEventArgs(dict);
						OnCustomDictionaryLoading(args);
					}
					return (SpellCheckerCustomDictionary)GetDictionaryProvider(dictionary).GetDictionary();
				}
			return null;
		}
		protected void InitializeProperties() {
			this.dictionaries = CreateDictionaryCollection();
			this.settingsSpelling = CreateSettingsSpelling();
			this.settingsText = CreateSettingsText();
			this.settingsForms = CreateSettingsForms();
			this.settingsDialogFormElements = new SpellCheckerDialogSettings(this);
			this.stylesEditors = new SpellCheckerEditorStyles(this);
			this.stylesButton = new SpellCheckerButtonStyles(this);
			this.stylesDialogForm = new SpellCheckerDialogFormStyles(this);
			this.imagesEditors = new SpellCheckerEditorImages(this);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			LoadSpellCheckSetting();
			return false;
		}
		protected virtual void LoadSpellCheckSetting() {
			Hashtable settings = GetClientObjectStateValue<Hashtable>(SpellCheckSettingStateKey);
			if(settings != null)
				SetSettingsByDictionary(settings);
		}
		protected override object GetCallbackResult() {
			AssignSpellCheckerSettings();
			IDictionary callBackResult = new Hashtable();
			callBackResult[SpellCheckerCallbackResultProperties.Settings] = GetSettingsDictionary();
			if(!CallBackReader.IsOptionsFormCallBack) {
				BeforeCheckText();
				if(!string.IsNullOrEmpty(CallBackReader.WordToAdd)) {
					SpellCheckerCustomDictionary dictionary = GetCustomDictionary();
					if(dictionary != null) {
						dictionary.AddWord(CallBackReader.WordToAdd);
						RaiseWordAdded(new WordAddedEventArgs(CallBackReader.WordToAdd));
					}
				}
				if(CallBackReader.DialogFormName == SpellCheckFormName) {
					callBackResult[SpellCheckerCallbackResultProperties.DialogContent] =
						GetDialogContentRenderResult(SpellCheckFormName);
				}
				GenerateCheckTextCallbackResult(CallBackReader.TextToCheck);
				if(errorCount > 0) {
					callBackResult[SpellCheckerCallbackResultProperties.ErrorCount] = errorCount;
					callBackResult[SpellCheckerCallbackResultProperties.StartErrorWordPositionArray] = startErrorPositionArray.ToArray();
					callBackResult[SpellCheckerCallbackResultProperties.SuggestionsArray] = suggestionsArray.ToArray();
					callBackResult[SpellCheckerCallbackResultProperties.WrongWordLengthArray] = wrongWordLengthArray.ToArray();
					callBackResult[SpellCheckerCallbackResultProperties.CheckComplete] = !this.spellCheckingHasBeenBroken; 
				}
			}
			else
				callBackResult[SpellCheckerCallbackResultProperties.DialogContent] =
					GetDialogContentRenderResult(CallBackReader.DialogFormName);
			return callBackResult;
		}
		protected virtual void GenerateCheckTextCallbackResult(string textToCheck) {
			errorCount = 0;
			if(!string.IsNullOrEmpty(textToCheck))
				CheckText(textToCheck);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			this.callBackReader = new SpellCheckerCallbackReader(eventArgument);
		}
		protected internal virtual void CheckText(string text) {
			TextOffset = 0;
			this.startErrorPositionArray = new List<int>();
			this.wrongWordLengthArray = new List<int>();
			this.suggestionsArray = new List<string[]>();
			this.spellCheckingHasBeenBroken = false;
			SpellChecker.NotInDictionaryWordFound += OnNotInDictionaryWordFound;
			SpellChecker.PrepareSuggestions += OnPrepareSuggestions;
			try {
				CheckTextCore(text);
			}
			finally {
				SpellChecker.PrepareSuggestions -= OnPrepareSuggestions;
				SpellChecker.NotInDictionaryWordFound -= OnNotInDictionaryWordFound;
			}
		}
		protected void CheckTextCore(string text) {
			SpellChecker.Check(text, CallBackReader.StartIndex);
		}
		protected virtual void SetSpellCheckerOptions() {
			SpellChecker.OptionsSpelling.IgnoreRepeatedWords = DevExpress.Utils.DefaultBoolean.True;
			AssignSpellCheckerOptions(SettingsSpelling);
		}
		protected virtual SpellCheckerDictionaryProvider GetDictionaryProvider(WebDictionary dictionary) {
			return dictionary.IsCustomDictionary ? new SpellCheckerCustomDictionaryProvider(dictionary) :
				new SpellCheckerDictionaryProvider(dictionary);
		}
		protected virtual void SetSpellCheckerDictionaries() {
			foreach(WebDictionary dictionary in Dictionaries) {
				SpellCheckerDictionaryProvider provider = GetDictionaryProvider(dictionary);
				if(dictionary.IsCustomDictionary && !provider.IsDictionaryStored) {
					ASPxSpellCheckerCustomDictionary dict = (ASPxSpellCheckerCustomDictionary)dictionary;
					CustomDictionaryLoadingEventArgs args = new CustomDictionaryLoadingEventArgs(dict);
					OnCustomDictionaryLoading(args);
				}
				SpellChecker.Dictionaries.Add(provider.GetDictionary());
			}
		}
		protected virtual void BeforeCheckText() {
			SetSpellCheckerDictionaries();
			SetSpellCheckerOptions();
			AssignSpellCheckerProperties();
		}
		void OnPrepareSuggestions(object sender, PrepareSuggestionsEventArgs e) {
			OnPrepareSuggestions(e);
		}
		void OnNotInDictionaryWordFound(object sender, NotInDictionaryWordFoundEventArgs e) {
			OnNotInDictionaryWordFound(e);
			if (e.Handled)
				return;
			if (this.errorCount < MaximumErrorCountInResponse) {
				GenerateMisspelledWordResult(e);
				this.errorCount++;
			}
			else {
				e.Handled = true;
				e.Result = SpellCheckOperation.Cancel;
				this.spellCheckingHasBeenBroken = true;
			}
		}
		protected virtual void GenerateMisspelledWordResult(NotInDictionaryWordFoundEventArgs e) {
			startErrorPositionArray.Add(((IntPosition)e.StartPosition).ActualIntPosition + TextOffset);
			wrongWordLengthArray.Add(((IntPosition)e.Length).ActualIntPosition);
			SuggestionCollection suggestions = SpellChecker.GetSuggestions(e.Word);
			suggestionsArray.Add(suggestions.ToStringArray());
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDialogUtilsScripts();
			RegisterIncludeScript(typeof(ASPxSpellChecker), SpellCheckerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpellChecker), SpellCheckerDialogsScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSpellChecker";
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(CheckedElementID))
				stb.AppendFormat("{0}.checkedElementID='{1}';\n", localVarName,
								 RenderUtils.GetReferentControlClientID(this, CheckedElementID, OnCheckedElementResolve));
			stb.AppendFormat("{0}.finishSpellChecking='{1}';\n", localVarName, HtmlConvertor.EscapeString(SettingsText.GetFinishSpellChecking()));
			stb.AppendFormat("{0}.spellCheckFormCaption='{1}';\n", localVarName, HtmlConvertor.EscapeString(SettingsText.GetSpellCheckFormCaption()));
			stb.AppendFormat("{0}.optionsFormCaption='{1}';\n", localVarName, HtmlConvertor.EscapeString(SettingsText.GetOptionsFormCaption()));
			stb.AppendFormat("{0}.noSuggestionsText='{1}';\n", localVarName, HtmlConvertor.EscapeString(SettingsText.GetNoSuggestionsText()));
			if(IsShowOneWordInTextPreview())
				stb.AppendFormat("{0}.showOneWordInTextPreview=true;\n", localVarName);
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(SpellCheckSettingStateKey, GetSettingsDictionary());
			return result;
		}
		protected virtual bool IsShowOneWordInTextPreview() {
			return false;
		}
		protected virtual ASPxSpellCheckerSpellingSettings CreateSettingsSpelling() {
			return new ASPxSpellCheckerSpellingSettings(this);
		}
		protected virtual ASPxSpellCheckerTextSettings CreateSettingsText() {
			return new ASPxSpellCheckerTextSettings(this);
		}
		protected virtual SpellCheckerFormsSettings CreateSettingsForms() {
			return new SpellCheckerFormsSettings(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SpellCheckerClientSideEvents();
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected virtual SpellCheckerDictionaryCollection CreateDictionaryCollection() {
			return new SpellCheckerDictionaryCollection(this);
		}
		protected virtual WebSpellChecker CreateSpellChecker() {
			return new WebSpellChecker();
		}
		protected virtual void AssignSpellCheckerProperties() {
			SpellChecker.LevenshteinDistance = this.LevenshteinDistance;
			SpellChecker.SuggestionCount = this.SuggestionCount;
			SpellChecker.Culture = GetCulture();
		}
		protected virtual void SetSettingsByDictionary(IDictionary dictionary) {
			SettingsSpelling.IgnoreUpperCaseWords = (bool)dictionary[SpellCheckSettingIgnoreUpperCaseWordsStateKey];
			SettingsSpelling.IgnoreMixedCaseWords = (bool)dictionary[SpellCheckSettingIgnoreMixedCaseWordsStateKey];
			SettingsSpelling.IgnoreWordsWithNumber = (bool)dictionary[SpellCheckSettingIgnoreWordsWithNumberStateKey];
			SettingsSpelling.IgnoreEmails = (bool)dictionary[SpellCheckSettingIgnoreEmailsStateKey];
			SettingsSpelling.IgnoreUrls = (bool)dictionary[SpellCheckSettingIgnoreUrlsStateKey];
			SettingsSpelling.IgnoreMarkupTags = (bool)dictionary[SpellCheckSettingIgnoreMarkupTagsStateKey];
			string currentCulture = (string)dictionary[SpellCheckSettingCultureStateKey];
			foreach(WebDictionary dic in Dictionaries)
				if(CultureInfo.Equals(dic.GetCulture().DisplayName, currentCulture))
					Culture = dic.GetCulture();
		}
		protected virtual IDictionary GetSettingsDictionary() {
			Hashtable result = new Hashtable();
			result[SpellCheckSettingIgnoreUpperCaseWordsStateKey] = SettingsSpelling.IgnoreUpperCaseWords;
			result[SpellCheckSettingIgnoreMixedCaseWordsStateKey] = SettingsSpelling.IgnoreMixedCaseWords;
			result[SpellCheckSettingIgnoreWordsWithNumberStateKey] = SettingsSpelling.IgnoreWordsWithNumber;
			result[SpellCheckSettingIgnoreEmailsStateKey] = SettingsSpelling.IgnoreEmails;
			result[SpellCheckSettingIgnoreUrlsStateKey] = SettingsSpelling.IgnoreUrls;
			result[SpellCheckSettingIgnoreMarkupTagsStateKey] = SettingsSpelling.IgnoreMarkupTags;
			result[SpellCheckSettingCultureStateKey] = GetCulture().DisplayName;
			return result;
		}
		protected virtual void AssignSpellCheckerOptions(ASPxSpellCheckerSpellingSettings settings) {
			OptionsSpellingBase options = new OptionsSpellingBase();
			options.IgnoreEmails = settings.IgnoreEmails ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreMixedCaseWords = settings.IgnoreMixedCaseWords ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreUpperCaseWords = settings.IgnoreUpperCaseWords ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreUri = settings.IgnoreUrls ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreWordsWithNumbers = settings.IgnoreWordsWithNumber ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreMarkupTags = settings.IgnoreMarkupTags ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			options.IgnoreRepeatedWords = DevExpress.Utils.DefaultBoolean.True;
			SpellChecker.OptionsSpelling.Assign(options);
		}
		protected void OnCheckedElementResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventCheckedElementResolve];
			if(handler != null)
				handler(this, e);
		}
		protected override bool HasContent() {
			return !DesignMode || IsAutoFormatPreview;
		}
		protected override void ClearControlFields() {
			this.mainElement = null;
			this.errorWordSpan = null;
			this.spellCheckForm = null;
			this.spellCheckOptionsForm = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			if(IsAutoFormatPreview) {
				CreateSpellCheckForm(this);
			}
			else {
				CreateMainElement();
				CreateSpellCheckForm(MainElement);
				CreateSpellCheckOptionsForm(MainElement);
				CreateSpan();
				base.CreateControlHierarchy();
			}
		}
		protected virtual void CreateMainElement() {
			this.mainElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(MainElement);
		}
		private void CreateSpan() {
			this.errorWordSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			ErrorWordSpan.ID = SpellCheckSpanID;
			Controls.Add(ErrorWordSpan);
			PrepareErrorWordSpan(); 
		}
		protected void CreateSpellCheckForm(WebControl parent) {
			this.spellCheckForm = new SpellCheckerPopupControl(this);
			SpellCheckForm.ID = SpellCheckFormId;
			SpellCheckForm.ParentStyles = GetDialogFormStyles();
			parent.Controls.Add(SpellCheckForm);
		}
		protected void CreateSpellCheckOptionsForm(WebControl parent) {
			this.spellCheckOptionsForm = new SpellCheckerPopupControl(this);
			SpellCheckOptionsForm.ID = SpellCheckOptionsFormId;
			SpellCheckOptionsForm.ParentStyles = GetDialogFormStyles();
			SpellCheckOptionsForm.PopupHorizontalAlign = PopupHorizontalAlign.Center;
			SpellCheckOptionsForm.PopupVerticalAlign = PopupVerticalAlign.Middle;
			parent.Controls.Add(SpellCheckOptionsForm);
		}
		protected internal virtual Control CreateDialogControl(string controlName, Control parent) {
			string userSpecifiedFormPath = SettingsForms.GetFormPath(controlName);
			Control userControl;
			if(string.IsNullOrEmpty(userSpecifiedFormPath)) {
				userControl = CreateDefaultForm(controlName);
				PrepareUserControl(userControl, parent, controlName, true);
			}
			else {
				userControl = CreateUserControl(userSpecifiedFormPath);
				PrepareUserControl(userControl, parent, controlName, false);
			}
			return userControl;
		}
		protected virtual Control GetControlForAutoFormatPreview() {
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker));
			Stream stream = asm.GetManifestResourceStream("Forms.CS.SpellCheckForm.ascx");
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
			stream.Position = 0;
			StreamReader reader = new StreamReader(stream);
			Control result = null;
			try {
				string userControlText = reader.ReadToEnd();
				result = Page.ParseControl(userControlText);
			}
			finally {
				reader.Close();
				stream.Close();
			}
			return result;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareMainElement();
			PrepareSpellCheckForm();
			PrepareSpellCheckOptionsForm();
			PrepareErrorWordSpan();
		}
		private void PrepareMainElement() {
			if(MainElement != null) {
				RenderUtils.SetStringAttribute(MainElement, "id", this.ID);
				RenderUtils.AppendDefaultDXClassName(MainElement, SpellCheckerStyles.MainElementCssClass);
				if(IsRightToLeft())
					RenderUtils.AppendDefaultDXClassName(MainElement, SpellCheckerStyles.RightToLeftCssClass);
			}
		}
		protected virtual void PrepareErrorWordSpan() {
			if(ErrorWordSpan != null) {
				ErrorWordStyle style = GetErrorWordStyle();
				ErrorWordSpan.MergeStyle(style);
				RenderUtils.SetVisibility(ErrorWordSpan, false, true);
			}
		}
		protected virtual void PreparePopupControl(DevExpress.Web.ASPxPopupControl popupControl) {
			popupControl.AllowDragging = true;
			popupControl.Modal = true;
			popupControl.PopupAnimationType = AnimationType.Fade;
			popupControl.CloseButtonImage.Assign(GetDialogFormCloseButtonImage());
		}
		protected virtual void PrepareSpellCheckForm() {
			if(SpellCheckForm != null) 
				PrepareSpellCheckPopupControl(SpellCheckForm);
		}
		protected virtual void PrepareSpellCheckPopupControl(SpellCheckerPopupControl popupControl) {
			PreparePopupControl(popupControl);
			popupControl.PopupVerticalAlign = PopupVerticalAlign.Below;
			popupControl.CloseAction = CloseAction.CloseButton;
			popupControl.CloseOnEscape = true;
			popupControl.Width = 0;
		}
		private void PrepareSpellCheckOptionsForm() {
			if(SpellCheckOptionsForm != null)
				PrepareOptionsPopupControl(SpellCheckOptionsForm);
		}
		protected virtual void PrepareOptionsPopupControl(SpellCheckerPopupControl popupControl) {
			PreparePopupControl(popupControl);
			popupControl.PopupVerticalAlign = PopupVerticalAlign.Middle;
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.Center;
			popupControl.CloseAction = CloseAction.None;
			popupControl.ShowCloseButton = false;
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected internal void SetFormPath(string formName, string value) {
			SettingsForms.SetFormPath(formName, value);
		}
		protected virtual string GetDialogContentRenderResult(string name) {
			return RenderUtils.GetRenderResult(CreateDialogControl(name, this));
		}
		protected Control CreateDefaultForm(string formName) {
			switch(formName) {
				case "SpellCheckForm":
					return CreateSpellCheckForm();
				case "SpellCheckOptionsForm":
					return CreateSpellCheckOptionsForm();
				default:
					throw new ArgumentException();
			}
		}
		protected virtual Control CreateSpellCheckForm() {
			return new SpellCheckForm();
		}
		protected virtual Control CreateSpellCheckOptionsForm() {
			return new SpellCheckOptionsForm();
		}
		protected internal void AssignSpellCheckerSettings() {
			AssignSpellCheckerProperties();
			AssignSpellCheckerOptions(SettingsSpelling);
		}
		protected override void LoadClientState(string state) {
			Hashtable dict = HtmlConvertor.FromJSON<Hashtable>(state);
			SetSettingsByDictionary(dict);
		}
		protected override string SaveClientState() {
			return HtmlConvertor.ToJSON(GetSettingsDictionary(), false, false, true);
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected virtual internal EditorImages GetEditorsImages() {
			return ImagesEditors;
		}
		protected virtual HeaderButtonImageProperties GetDialogFormCloseButtonImage() {
			return DialogFormCloseButtonImage;
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxSpellChecker), SpellCheckerCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxSpellChecker), SpellCheckerSystemCssResourceName);
		}
		protected override StylesBase CreateStyles() {
			return new SpellCheckerStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new SpellCheckerImages(this);
		}
		protected override string GetSkinControlName() {
			return "SpellChecker";
		}
		protected override string[] GetChildControlNames() {
			return ChildControlNames;
		}
		protected override bool HasSpriteCssFile() {
			return false;
		}
		public CheckedTextContainerStyle GetCheckedTextContainerStyle() {
			CheckedTextContainerStyle style = new CheckedTextContainerStyle();
			style.CopyFrom(Styles.GetDefaultCheckedTextContainerStyle());
			style.CopyFrom(RenderStyles.CheckedTextContainer);
			return style;
		}
		protected internal ErrorWordStyle GetErrorWordStyle() {
			ErrorWordStyle style = new ErrorWordStyle();
			style.CopyFrom(Styles.GetDefaultErrorWordStyle());
			style.CopyFrom(RenderStyles.ErrorWord);
			return style;
		}
		protected virtual internal ButtonControlStyles GetButtonStyles() {
			return StylesButton;
		}
		protected virtual internal DevExpress.Web.PopupControlStyles GetDialogFormStyles() {
			return StylesDialogForm;
		}
		protected virtual internal EditorStyles GetEditorsStyles() {
			return StylesEditors;
		}
		private static readonly object prepareSuggestions = new object(); 
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerPrepareSuggestions")]
#endif
		public event PrepareSuggestionsEventHandler PrepareSuggestions {
			add { Events.AddHandler(prepareSuggestions, value); }
			remove { Events.RemoveHandler(prepareSuggestions, value); }
		}
		protected internal virtual void OnPrepareSuggestions(PrepareSuggestionsEventArgs e) {
			RaisePrepareSuggestions(e);
		}
		protected virtual void RaisePrepareSuggestions(PrepareSuggestionsEventArgs e) {
			PrepareSuggestionsEventHandler handler = (PrepareSuggestionsEventHandler)this.Events[prepareSuggestions];
			if(handler != null)
				handler(this, e);
		}
		private static readonly object notInDictionaryWordFound = new object(); 
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerNotInDictionaryWordFound")]
#endif
		public event NotInDictionaryWordFoundEventHandler NotInDictionaryWordFound {
			add { Events.AddHandler(notInDictionaryWordFound, value); }
			remove { Events.RemoveHandler(notInDictionaryWordFound, value); }
		}
		protected internal virtual void OnNotInDictionaryWordFound(NotInDictionaryWordFoundEventArgs e) {
			RaiseNotInDictionaryWordFound(e);
		}
		protected virtual void RaiseNotInDictionaryWordFound(NotInDictionaryWordFoundEventArgs e) {
			NotInDictionaryWordFoundEventHandler handler = (NotInDictionaryWordFoundEventHandler)this.Events[notInDictionaryWordFound];
			if(handler != null)
				handler(this, e);
		}
		private static readonly object wordAdded = new object(); 
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerWordAdded")]
#endif
		public event WordAddedEventHandler WordAdded {
			add { Events.AddHandler(wordAdded, value); }
			remove { Events.RemoveHandler(wordAdded, value); }
		}
		protected internal virtual void OnWordAdded(object sender, WordAddedEventArgs e) {
			RaiseWordAdded(e);
		}
		protected virtual void RaiseWordAdded(WordAddedEventArgs e) {
			WordAddedEventHandler handler = (WordAddedEventHandler)this.Events[wordAdded];
			if(handler != null)
				handler(this, e);
		}
		private static readonly object customDictionaryLoading = new object();
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCustomDictionaryLoading")]
#endif
		public event CustomDictionaryLoadingEventHandler CustomDictionaryLoading {
			add { Events.AddHandler(customDictionaryLoading, value); }
			remove { Events.RemoveHandler(customDictionaryLoading, value); }
		}
		protected internal virtual void OnCustomDictionaryLoading(CustomDictionaryLoadingEventArgs e) {
			RaiseCustomDictionaryLoading(e);
		}
		protected virtual void RaiseCustomDictionaryLoading(CustomDictionaryLoadingEventArgs e) {
			CustomDictionaryLoadingEventHandler handler = (CustomDictionaryLoadingEventHandler)this.Events[customDictionaryLoading];
			if(handler != null)
				handler(this, e);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				SettingsText,
				SettingsSpelling,
				SettingsForms,
				SettingsDialogFormElements,
				Dictionaries,
				StylesButton,
				StylesEditors,
				StylesDialogForm,
				ImagesEditors
			});
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxSpellChecker.Design.SpellCheckerCommonFormDesigner"; } }
	}
	public class CustomDictionaryLoadingEventArgs : EventArgs {
		ASPxSpellCheckerCustomDictionary customDictionary;
		public CustomDictionaryLoadingEventArgs(ASPxSpellCheckerCustomDictionary customDictionary) {
			this.customDictionary = customDictionary;
		}
		public ASPxSpellCheckerCustomDictionary CustomDictionary {
			get { return this.customDictionary; }
		}
	}
	public delegate void CustomDictionaryLoadingEventHandler(object sender, CustomDictionaryLoadingEventArgs e);
}
internal class ToolboxBitmapAccess {
}
