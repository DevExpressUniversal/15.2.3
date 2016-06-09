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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Forms;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Rendering;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraSpellChecker;
using System.Text.RegularExpressions;
using System.Reflection;
namespace DevExpress.Web.ASPxHtmlEditor {
	public enum HtmlEditorView {
		Design,
		Html,
		Preview
	}
	public enum HtmlEditorImportFormat {
		Rtf,
		Mht,
		Odt,
		Docx,
		Txt
	}
	public enum HtmlEditorExportFormat {
		Rtf,
		Mht,
		Odt,
		Docx,
		Txt,
		Pdf
	}
	public enum HtmlEditorToolbarMode {
		Menu,
		Ribbon,
		ExternalRibbon,
		None
	}
	internal delegate bool EditorsProcessingProc(ASPxHtmlEditor editor);
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxHtmlEditor"),
	Designer("DevExpress.Web.ASPxHtmlEditor.Design.ASPxHtmlEditorDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull), ToolboxBitmap(typeof(ToolboxBitmapAccess), "ASPxHtmlEditor.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon)
]
	[ValidationProperty("Html")]
	public partial class ASPxHtmlEditor : ASPxWebControl, IRequiresLoadPostDataControl, IParentSkinOwner, IValidationSummaryEditor, IControlDesigner {
		#region Nested types
		static class CallbackResultProperty {
			public const string
				Action = "action",
				Html = "html",
				SpellCheck = "spellcheck",
				SpellCheckLoadControl = "spellcheckerloadcontrol",
				AllowScripts = "allowScripts";
		}
		#endregion
		protected internal const string HtmlEditorResourcePath = "DevExpress.Web.ASPxHtmlEditor.";
		protected internal const string HtmlEditorImagesResourcePath = HtmlEditorResourcePath + "Images.";
		protected internal const string HtmlEditorScriptsResourcePath = HtmlEditorResourcePath + "Scripts.";
		protected internal const string HtmlEditorUIScriptsResourcePath = HtmlEditorScriptsResourcePath + "UI.";
		protected internal const string HtmlEditorCommandsScriptsResourcePath = HtmlEditorScriptsResourcePath + "Commands.";
		protected internal const string HtmlEditorWrappersScriptsResourcePath = HtmlEditorScriptsResourcePath + "Wrappers.";
		protected internal const string HtmlEditorCssResourcePath = HtmlEditorResourcePath + "Css.";
		protected internal const string HtmlEditorDefaultCssResourceName = HtmlEditorCssResourcePath + "default.css";
		protected internal const string HtmlEditorSystemDesignViewCssResourceName = HtmlEditorCssResourcePath + "systemDesignView.css";
		protected internal const string HtmlEditorSpriteCssResourceName = HtmlEditorCssResourcePath + "sprite.css";
		protected internal const string HtmlEditorSystemCssResourceName = HtmlEditorCssResourcePath + "system.css";
		protected internal const string HtmlEditorToolbarSpriteCssResourceName = HtmlEditorCssResourcePath + "ISprite.css";
		protected internal const string HtmlEditorToolbarWhiteSpriteCssResourceName = HtmlEditorCssResourcePath + "WISprite.css";
		protected internal const string HtmlEditorToolbarGrayScaleSpriteCssResourceName = HtmlEditorCssResourcePath + "GISprite.css";
		protected internal const string HtmlEditorToolbarGrayScaleWithWhiteHottrackSpriteCssResourceName = HtmlEditorCssResourcePath + "GWISprite.css";
		protected internal const string HtmlEditorFrameworksCssResourcePath = HtmlEditorCssResourcePath + "Frameworks.";
		protected internal const string HtmlEditorCodeMirrorCssResourceName = HtmlEditorFrameworksCssResourcePath + "codemirror.css";
		protected internal const string HtmlEditorFoldGutterCssResourceName = HtmlEditorFrameworksCssResourcePath + "foldgutter.css";
		protected internal const string HtmlEditorSpriteImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + ImagesBase.SpriteImageName + ".png";
		protected internal const string HtmlEditorIconSpriteImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "ISprite.png";
		protected internal const string HtmlEditorWhiteIconSpriteImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "WISprite.png";
		protected internal const string HtmlEditorGrayScaleIconSpriteImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "GISprite.png";
		protected internal const string HtmlEditorGrayScaleWithWhiteHottrackIconSpriteImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "GWISprite.png";
		protected internal const string HtmlEditorFoldGutterFoldedImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "heFoldGutterFolded.png";
		protected internal const string HtmlEditorFoldGutterOpenImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "heFoldGutterOpen.png";
		protected internal const string HtmlEditorFoldMarkerImageResourceName = ASPxHtmlEditor.HtmlEditorImagesResourcePath + "dxheFoldMarker.png";
		protected internal static string[] DialogNames = new string[] { 
			"insertimagedialog", "insertlinkdialog", "changeimagedialog", "changelinkdialog", "pastefromworddialog", "inserttabledialog", 
			"tablepropertiesdialog", "tablecellpropertiesdialog", "tablecolumnpropertiesdialog", "tablerowpropertiesdialog", 
			"insertflashdialog", "changeflashdialog", "insertvideodialog", "changevideodialog", "insertaudiodialog", "changeaudiodialog", 
			"insertyoutubevideodialog" , "changeyoutubevideodialog", "insertplaceholderdialog", "changeplaceholderdialog", "changeelementpropertiesdialog"
		};
		protected internal static string[] UniqueFormNames = new string[] {
			"InsertImageForm", "InsertLinkForm", "InsertTableForm", "PasteFromWordForm", "TableColumnPropertiesForm", "SelectImageForm",
			"SelectDocumentForm", "InsertFlashForm", "InsertVideoForm", "InsertAudioForm", "InsertYoutubeVideoForm", "InsertPlaceholderForm",
			"ChangeElementPropertiesForm", "TableCellPropertiesForm", "TableRowPropertiesForm"
		};
		protected internal static string[] FormNames = new string[] { 
			UniqueFormNames[0], UniqueFormNames[1], UniqueFormNames[0], UniqueFormNames[1], UniqueFormNames[3], UniqueFormNames[2], 
			UniqueFormNames[2], UniqueFormNames[13], UniqueFormNames[4], UniqueFormNames[14], UniqueFormNames[7], UniqueFormNames[7],
			UniqueFormNames[8], UniqueFormNames[8], UniqueFormNames[9], UniqueFormNames[9], UniqueFormNames[10], UniqueFormNames[10], 
			UniqueFormNames[11], UniqueFormNames[11],UniqueFormNames[12]
		};
		protected internal const string CustomDialogNamePrefix = "cd_";
		protected string internalCallbackArgument = "";
		Dictionary<string, string> dialogFormNameDictionary = new Dictionary<string, string>();
		HtmlEditorClientState clientState;
		ASPxHtmlEditorRenderHelper renderHelper;
		HtmlEditorToolbarCollection toolbars = null;
		HtmlEditorRibbonTabCollection ribbonTabs;
		HtmlEditorRibbonContextTabCategoryCollection ribbonContextTabCategories;
		WebControl containerControl = null;
		bool showErrorFrame = false;
		bool isValid = true;
		Unit widthSetBeforeSkinApplied = Unit.Empty; 
		static ASPxHtmlEditor() {
			SpellingFormType.Outlook.ToString();
		}
		public ASPxHtmlEditor()
			: base() {
			CreateDialogFormNameDictionary();
			this.clientState = new HtmlEditorClientState(this);
			InitializeSettings();
			InitializeStyles();
			PartsRoundPanelInternal = new RoundPanelParts(this);
			ImagesEditors = new HtmlEditorEditorImages(this);
			ImagesFileManager = new HtmlEditorFileManagerImages(this);
			CustomDialogs = CreateCustomDialogs();
			ContextMenuItems = CreateContextMenuItems();
			SettingsValidation = new HtmlEditorValidationSettings(this);
			Document = new HtmlEditorDocument(this);
			Shortcuts = new HtmlEditorShortcutCollection(this);
			Placeholders = new HtmlEditorPlaceholderCollection(this);
			ExternalControlsValidator.AreEditorsValidInContainer += new ExternalEditorsEventHandler(AreEditorsValidInContainer);
			ExternalControlsValidator.ClearEditorsInContainer += new ExternalEditorsEventHandler(ClearEditorsInContainer);
			ExternalControlsValidator.ValidateEditorsInContainer += new ExternalEditorsEventHandler(ValidateEditorsInContainer);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public HtmlEditorClientSideEvents ClientSideEvents {
			get { return (HtmlEditorClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorHtml"),
#endif
		DefaultValue(""), Editor(typeof(MultilineStringEditor),
		typeof(UITypeEditor)), AutoFormatDisable, Bindable(true), Localizable(false)]
		public string Html {
			get { return Document.Html; }
			set { Document.Html = value; }
		}
		protected internal HtmlEditorDocument Document { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorActiveView"),
#endif
		DefaultValue(HtmlEditorView.Design), AutoFormatDisable, Localizable(false)]
		public HtmlEditorView ActiveView {
			get { return GetHtmlEditorView(ActiveViewInternal); }
			set {
				ValidateActiveView(value);
				ActiveViewInternal = value;
			}
		}
		HtmlEditorView ActiveViewInternal {
			get { return (HtmlEditorView)GetEnumProperty("ActiveView", HtmlEditorView.Design); }
			set {
				SetEnumProperty("ActiveView", HtmlEditorView.Design, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorToolbars"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorToolbarCollection Toolbars {
			get {
				if(toolbars == null)
					toolbars = new HtmlEditorToolbarCollection(this);
				return toolbars;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorRibbonTabs"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Category("Ribbon")]
		public HtmlEditorRibbonTabCollection RibbonTabs {
			get {
				if(ribbonTabs == null)
					ribbonTabs = new HtmlEditorRibbonTabCollection(this);
				return ribbonTabs;
			}
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Category("Ribbon")]
		public HtmlEditorRibbonContextTabCategoryCollection RibbonContextTabCategories {
			get {
				if(ribbonContextTabCategories == null)
					ribbonContextTabCategories = new HtmlEditorRibbonContextTabCategoryCollection(this);
				return ribbonContextTabCategories;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorToolbarMode"),
#endif
		DefaultValue(HtmlEditorToolbarMode.Menu), AutoFormatDisable, Localizable(false)]
		public HtmlEditorToolbarMode ToolbarMode {
			get { return (HtmlEditorToolbarMode)GetEnumProperty("ToolbarMode", HtmlEditorToolbarMode.Menu); }
			set {
				SetEnumProperty("ToolbarMode", HtmlEditorToolbarMode.Menu, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAssociatedRibbonID"),
#endif
		DefaultValue(""), AutoFormatEnable, NotifyParentProperty(true), Localizable(false), Category("Ribbon"),
		TypeConverter("DevExpress.Web.Design.RibbonControlIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string AssociatedRibbonID {
			get { return GetStringProperty("AssociatedRibbonID", string.Empty); }
			set { SetStringProperty("AssociatedRibbonID", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorCssFiles"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorCssFileCollection CssFiles { get { return Document.CssFiles; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorCustomDialogs"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorCustomDialogs CustomDialogs { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorContextMenuItems"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorContextMenuItemCollection ContextMenuItems { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorShortcuts"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorShortcutCollection Shortcuts { get; private set; }
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorPlaceholderCollection Placeholders { get; private set; }
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorWidth")]
#endif
		public override Unit Width {
			get {
				return base.Width;
			}
			set {
				OnWidthSet(BeforeSkinApplying(), value); 
				base.Width = value;
			}
		}
		protected virtual bool BeforeSkinApplying() {
			return Page == null; 
		}
		protected void OnWidthSet(bool beforeSkinApplying, Unit width) {
			if(beforeSkinApplying)
				this.widthSetBeforeSkinApplied = width;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			RestoreWidth();
		}
		protected void RestoreWidth() { 
			if(this.widthSetBeforeSkinApplied != Unit.Empty) 
				Width = this.widthSetBeforeSkinApplied;
		}
		[Browsable(false), AutoFormatDisable, Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ErrorText {
			get { return SettingsValidation.ErrorText; }
			set {
				SettingsValidation.ErrorText = value;
				LayoutChanged();
				if(!LocalValidationSummaryUpdateRequestsLocked)
					ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
			}
		}
		[Browsable(false), Bindable(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsValid {
			get { return isValid && !ShowErrorFrame; }
			set {
				isValid = value;
				LayoutChanged();
				if(!LocalValidationSummaryUpdateRequestsLocked)
					ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorRenderIFrameForPopupElements"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatEnable, Themeable(false)]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return RenderIFrameForPopupElementsInternal; }
			set { RenderIFrameForPopupElementsInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorImages Images { get { return (HtmlEditorImages)ImagesInternal; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImagesEditors"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorEditorImages ImagesEditors { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImagesFileManager"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerImages ImagesFileManager { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorPartsRoundPanel"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false),
		Obsolete("Use the StylesRoundPanel property to specify a background color and background image of a round panel.")]
		public RoundPanelParts PartsRoundPanel { get { return PartsRoundPanelInternal; } }
		protected internal RoundPanelParts PartsRoundPanelInternal { get; private set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorClientEnabled"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVisible"),
#endif
		AutoFormatDisable]
		public override bool Visible {
			get { return base.Visible; }
			set {
				bool changingVisible = value != base.Visible;
				base.Visible = value;
				if(changingVisible)
					ValidationSummaryCollection.Instance.OnEditorPropertyAffectingValidationSettingsChanged(this);
			}
		}
		bool ScriptsAllowed {
			get { return SettingsHtmlEditing.AllowScripts || Document.IsHtmlCorrectionHandled(); }
		}
		protected internal HtmlEditorSpellChecker SpellChecker { get; private set; }
		protected internal HtmlEditorControl HtmlEditorControl { get; private set; }
		protected internal WebControl ContainerControl {
			get { return containerControl ?? this; }
		}
		protected internal ASPxHtmlEditorRenderHelper RenderHelper {
			get {
				if(this.renderHelper == null)
					this.renderHelper = new ASPxHtmlEditorRenderHelper(this);
				return this.renderHelper;
			}
		}
		protected Dictionary<string, string> DialogFormNameDictionary {
			get { return dialogFormNameDictionary; }
		}
		protected internal MenuIconSetType GetIconSet() {
			return Images.MenuIconSet;
		}
		HtmlEditorClientState ClientState {
			get { return clientState; }
		}
		public void CreateDefaultToolbars(bool clearExistingToolbars) {
			if(clearExistingToolbars)
				Toolbars.Clear();
			Toolbars.CreateDefaultToolbars(IsRightToLeft());
		}
		public void CreateDefaultRibbonTabs(bool clearExistingRibbonTabs) {
			if(ToolbarMode == HtmlEditorToolbarMode.Ribbon) {
				if(clearExistingRibbonTabs)
					RibbonTabs.Clear();
				RibbonTabs.CreateDefaultRibbonTabs();
			}
			else if(ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon) {
				ASPxRibbon ribbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
				if(ribbon != null)
					RibbonHelper.AddTabCollectionToControl(ribbon, new HtmlEditorDefaultRibbon(this).DefaultRibbonTabs, clearExistingRibbonTabs);
			}
		}
		public void CreateDefaultRibbonContextTabCategories(bool clearExistingContextTabCategories) {
			if(ToolbarMode == HtmlEditorToolbarMode.Ribbon) {
				if(clearExistingContextTabCategories)
					RibbonContextTabCategories.Clear();
				RibbonContextTabCategories.CreateDefaultRibbonContextTabCategories();
			}
			else if(ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon) {
				ASPxRibbon ribbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
				if(ribbon != null)
					RibbonHelper.AddContextCategoriesToControl(ribbon, new HtmlEditorDefaultRibbon(this).DefaultRibbonContextTabCategories, clearExistingContextTabCategories);
			}
		}
		public static string CorrectHtml(string html, bool allowScripts, bool allowIFrames, bool allowFormElements, bool updateBoldItalic, bool updateDeprecatedElements) {
			return CorrectHtml(html, allowScripts, allowIFrames, allowFormElements, updateBoldItalic, updateDeprecatedElements, true, true, AllowedDocumentType.XHTML);
		}
		public static string CorrectHtml(string html, bool allowScripts, bool allowIFrames, bool allowFormElements, bool updateBoldItalic, bool updateDeprecatedElements, 
			bool allowIdAttributes, bool allowStyleAttributes, AllowedDocumentType allowedDocumentType) {
			return CorrectHtml(html, allowScripts, allowIFrames, allowFormElements, updateBoldItalic, updateDeprecatedElements, allowIdAttributes, allowStyleAttributes, 
				allowedDocumentType, false, false, false);
		}
		public static string CorrectHtml(string html, bool allowScripts, bool allowIFrames, bool allowFormElements, bool updateBoldItalic, bool updateDeprecatedElements, 
			bool allowIdAttributes, bool allowStyleAttributes, AllowedDocumentType allowedDocumentType, bool allowHTML5MediaElements, bool allowObjectAndEmbedElements, 
			bool allowYouTubeVideoIFrames) {
			return CorrectHtml(html, allowScripts, allowIFrames, allowFormElements, updateBoldItalic, updateDeprecatedElements, allowIdAttributes, allowStyleAttributes, 
				allowedDocumentType, allowHTML5MediaElements, allowObjectAndEmbedElements, allowYouTubeVideoIFrames, false, null);
		}
		public static string CorrectHtml(string html, bool allowScripts, bool allowIFrames, bool allowFormElements, bool updateBoldItalic, bool updateDeprecatedElements, 
			bool allowIdAttributes, bool allowStyleAttributes, AllowedDocumentType allowedDocumentType, bool allowHTML5MediaElements, bool allowObjectAndEmbedElements, 
			bool allowYouTubeVideoIFrames, bool useAbsoluteResourcePaths, HtmlEditorContentElementFiltering contentElementFiltering) {
			HtmlEditorHtmlEditingSettings settings = new HtmlEditorHtmlEditingSettings();
			settings.UpdateBoldItalic = updateBoldItalic;
			settings.UpdateDeprecatedElements = updateDeprecatedElements;
			settings.AllowScripts = allowScripts;
			settings.AllowIFrames = allowIFrames;
			settings.AllowFormElements = allowFormElements;
			settings.AllowIdAttributes = allowIdAttributes;
			settings.AllowStyleAttributes = allowStyleAttributes;
			settings.AllowedDocumentType = allowedDocumentType;
			settings.AllowHTML5MediaElements = allowHTML5MediaElements;
			settings.AllowObjectAndEmbedElements = allowObjectAndEmbedElements;
			settings.AllowYouTubeVideoIFrames = allowYouTubeVideoIFrames;
			if(useAbsoluteResourcePaths)
				settings.ResourcePathMode = ResourcePathMode.Absolute;
			if(contentElementFiltering != null)
				settings.ContentElementFiltering.Assign(contentElementFiltering);
			return CorrectHtml(html, settings);
		}
		public static string CorrectHtml(string html) {
			return CorrectHtml(html, HtmlEditorHtmlEditingSettings.Default);
		}
		public static string CorrectHtml(string html, HtmlEditorHtmlEditingSettings settings) {
			return HtmlProcessor.ProcessHtml(html, settings);
		}
		public static string ReplacePlaceholders(string html, Dictionary<string, string> placeholders) {
			if(placeholders != null && !string.IsNullOrEmpty(html)) {
				foreach(string key in placeholders.Keys)
					html = ASPxHtmlEditor.ReplacePlaceholder(html, key, placeholders[key]);
			}
			return html;
		}
		public static string ReplacePlaceholders(string html, object placeholders) {
			if(placeholders == null || string.IsNullOrEmpty(html))
				return html;
			Type type = placeholders.GetType();
			foreach(var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				object value = p.GetValue(placeholders, null);
				html = ASPxHtmlEditor.ReplacePlaceholder(html, p.Name, ASPxHtmlEditor.GetPlaceholderReplacement(value));
			}
			return html;
		}
		protected internal static string ReplacePlaceholder(string html, string placeholderName, string replacement) {
			return Regex.Replace(html, @"{((&nbsp;)*|\s*)" + placeholderName + @"((&nbsp;)*|\s*)}", string.IsNullOrEmpty(replacement) ? string.Empty : replacement, RegexOptions.Singleline);
		}
		protected internal static string GetPlaceholderReplacement(object value) {
			if(value == null) 
				return string.Empty;
			if(value.GetType() == typeof(DateTime))
				return ((DateTime)value).ToLongDateString();
			else if(value.GetType() == typeof(Byte[]))
				return string.Format("<img alt='' src='data:image/Bmp;base64,{0}'/>", Convert.ToBase64String((byte[])value));
			return value.ToString();
		}
		string GetProcessedHtmlForSpellChecker(string validHtml) {
			return HtmlProcessor.ProcessHtmlForSpellChecker(validHtml, SettingsHtmlEditing);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				StylesTagInspector,
				StylesToolbars,
				StylesButton,
				StylesContextMenu,
				StylesPasteOptionsBar,
				StylesEditors,
				StylesDialogForm,
				StylesRoundPanel,
				StylesSpellChecker,
				StylesStatusBar,
				StylesFileManager,
				StylesDocument,
				SettingsDialogs,
				Settings,
				SettingsHtmlEditing,
				SettingsText,
				SettingsSpellChecker,
				SettingsResize,
				SettingsValidation,
#pragma warning disable 618
				SettingsForms,
				SettingsDialogFormElements,
#pragma warning restore 618
				Document,
				Toolbars,
				RibbonTabs,
				RibbonContextTabCategories,
				CustomDialogs,
				ContextMenuItems,
				ImagesEditors,
				ImagesFileManager,
				PartsRoundPanelInternal,
				Shortcuts,
				Placeholders
			});
		}
		protected override ImagesBase CreateImages() {
			return new HtmlEditorImages(this);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			HtmlEditorControl = null;
			SpellChecker = null;
			this.containerControl = null;
		}
		protected Control CurrentDialogForm { get; set; }
		protected internal bool ShowViewSwitcher {
			get {
				return ((Settings.AllowDesignViewInternal ? 1 : 0) +
					(Settings.AllowHtmlViewInternal ? 1 : 0) +
					(Settings.AllowPreviewInternal ? 1 : 0) > 1);
			}
		}
		protected internal bool ShowErrorFrame {
			get { return this.showErrorFrame; }
			set {
				this.showErrorFrame = value;
				LayoutChanged();
			}
		}
		protected internal bool IsClientValidationEnabled() {
			return ClientSideEvents.Validation != "" || SettingsValidation.HasValidationPatterns();
		}
		protected internal bool IsValidationEnabledCore() {
			return IsVisible() && IsEnabled() && (!IsValid || IsClientValidationEnabled() || SettingsValidation.HasValidationPatterns());
		}
		protected internal bool IsErrorFrameVisible() {
			return DesignMode ? ShowErrorFrame : false;
		}
		protected internal bool IsErrorFrameRequired() {
			return SettingsValidation.Display;
		}
		protected internal bool IsAdvancedHtmlEditingMode() {
			HtmlEditorHtmlEditingMode mode = Settings.SettingsHtmlView.Mode;
			return mode == HtmlEditorHtmlEditingMode.Advanced ||
				  (mode == HtmlEditorHtmlEditingMode.Auto && !Browser.Platform.IsTouchUI && !IsRightToLeft());
		}
		private bool notifyValidationSummariesToAcceptNewError;
		internal bool NotifyValidationSummariesToAcceptNewError {
			get { return notifyValidationSummariesToAcceptNewError; }
			set { notifyValidationSummariesToAcceptNewError = value; }
		}
		protected override void CreateControlHierarchy() {
			this.containerControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(this.containerControl);
			if(DesignMode) {
				ContainerControl.Width = GetControlStyle().Width;
				ContainerControl.Height = GetControlStyle().Height;
			}
			CreateMainControl();
			base.CreateControlHierarchy();
			CreateDialogBlock();
			if(!DesignMode)
				CreateSpellChecker();
		}
		protected virtual HtmlEditorSpellChecker CreateSpellCheckerInstance() {
			return new HtmlEditorSpellChecker(this);
		}
		protected void CreateMainControl() {
			HtmlEditorControl = new HtmlEditorControl(this);
			ContainerControl.Controls.Add(HtmlEditorControl);
		}
		protected string CurrentDialogName {
			get { return GetClientObjectStateValueString(RenderHelper.CurrentDialogStateKey); }
		}
		protected bool IsCustomDialog {
			get { return !DialogFormNameDictionary.ContainsKey(CurrentDialogName); }
		}
		protected virtual void CreateDialogBlock() {
			if(DesignMode)
				return;
			if(!string.IsNullOrEmpty(CurrentDialogName)) {
				CurrentDialogForm = CreateDialogFormControl(CurrentDialogName, ContainerControl);
				CurrentDialogForm.EnableViewState = false;
				if(Page != null && Page.IsCallback) {
					CurrentDialogForm.DataBind();
					RenderUtils.LoadPostDataRecursive(CurrentDialogForm, Request.Params, true);
				} else
					CurrentDialogForm.Visible = false;
			}
		}
		protected void CreateSpellChecker() {
			SpellChecker = CreateSpellCheckerInstance();
			SpellChecker.Visible = false;
			SpellChecker.ID = GetSpellCheckerID();
			SpellChecker.EnableViewState = false;
			SpellChecker.Culture = SettingsSpellChecker.Culture;
			SpellChecker.LevenshteinDistance = SettingsSpellChecker.LevenshteinDistance;
			SpellChecker.MaximumErrorCountInResponse = SettingsSpellChecker.MaximumErrorCountInResponse;
			SpellChecker.SuggestionCount = SettingsSpellChecker.SuggestionCount;
			SpellChecker.Dictionaries.Assign(SettingsSpellChecker.Dictionaries);
			SpellChecker.SettingsSpelling.Assign(SettingsSpellChecker.SettingsSpelling);
			SpellChecker.SettingsText.Assign(SettingsSpellChecker.SettingsText);
			SpellChecker.SettingsLoadingPanel.Assign(SettingsLoadingPanel);
#pragma warning disable 618
			SpellChecker.SettingsForms.Assign(SettingsForms.SpellCheckerForms);
#pragma warning restore 618
			SpellChecker.SettingsDialogFormElements.Assign(SettingsDialogs.CheckSpellingDialog);
			SpellChecker.WordAdded += new WordAddedEventHandler(SpellChecker_WordAdded);
			ContainerControl.Controls.Add(SpellChecker);
		}
		protected void SpellChecker_WordAdded(object sender, WordAddedEventArgs e) {
			this.RaiseSpellCheckerWordAdded(e);
		}
		protected void CreateDialogFormNameDictionary() {
			for(int i = 0; i < DialogNames.Length; i++)
				DialogFormNameDictionary.Add(DialogNames[i], FormNames[i]);
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected internal Unit GetErrorFrameImageSpacing() {
			return GetErrorFrameStyle().ImageSpacing;
		}
		protected internal string GetErrorFrameCloseButtonOnClick() {
			return string.Format(ErrorFrameCloseButtonClickHandlerName, ClientID);
		}
		protected internal string GetErrorFrameID() {
			return RenderHelper.ErrorFrameID;
		}
		protected internal string GetErrorTextCellID() {
			return RenderHelper.ErrorTextCellID;
		}
		protected internal string GetErrorFrameCloseButtonID() {
			return RenderHelper.ErrorFrameCloseButtonID;
		}
		protected internal string GetErrorFrameCloseButtonImageID() {
			return GetErrorFrameCloseButtonID() + ButtonImageIdPostfix;
		}
		protected string GetSpellCheckerID() {
			return "SC";
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxHtmlEditor), HtmlEditorDefaultCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxHtmlEditor), HtmlEditorSystemCssResourceName);
			if(Settings.AllowHtmlView && IsAdvancedHtmlEditingMode()) {
				ResourceManager.RegisterCssResource(Page, typeof(ASPxHtmlEditor), HtmlEditorCodeMirrorCssResourceName);
				ResourceManager.RegisterCssResource(Page, typeof(ASPxHtmlEditor), HtmlEditorFoldGutterCssResourceName);
			}
		}
		private bool localValidationSummaryUpdateRequestsLocked;
		private bool LocalValidationSummaryUpdateRequestsLocked {
			get { return localValidationSummaryUpdateRequestsLocked; }
			set { localValidationSummaryUpdateRequestsLocked = true; }
		}
		public bool Validate() {
			bool isValid = true;
			string errorText;
			LocalValidationSummaryUpdateRequestsLocked = true;
			try {
				if(SettingsValidation.RequiredField.IsRequired && string.IsNullOrEmpty(Html)) {
					isValid = false;
					errorText = SettingsValidation.RequiredField.GetErrorText();
				} else {
					HtmlEditorValidationEventArgs args = new HtmlEditorValidationEventArgs(Html, SettingsValidation.ErrorText, true);
					OnValidation(args);
					Html = args.Html;
					isValid = args.IsValid;
					errorText = args.ErrorText;
				}
				if(!isValid) {
					this.IsValid = isValid;
					this.ErrorText = errorText;
				} else
					this.IsValid = true;
			} finally {
				LocalValidationSummaryUpdateRequestsLocked = false;
			}
			ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
			return isValid;
		}
		private static bool AreEditorsValidInContainer(Control container, string validationGroup, bool checkInvisibleEditors) {
			return ProcessingEditorsInContainer(delegate(ASPxHtmlEditor editor) {
				return editor.IsValid;
			}, container, validationGroup, checkInvisibleEditors);
		}
		private static bool ClearEditorsInContainer(Control container, string validationGroup, bool clearInvisibleEditors) {
			return ProcessingEditorsInContainer(delegate(ASPxHtmlEditor editor) {
				editor.IsValid = true;
				editor.ErrorText = "";
				return true;
			}, container, validationGroup, clearInvisibleEditors);
		}
		private static bool ValidateEditorsInContainer(Control container, string validationGroup, bool validateInvisibleEditors) {
			return ProcessingEditorsInContainer(delegate(ASPxHtmlEditor editor) {
				return editor.Validate();
			}, container, validationGroup, validateInvisibleEditors);
		}
		private static bool ProcessingEditorsInContainer(EditorsProcessingProc proc, Control container, string validationGroup, bool validateInvisibleEditors) {
			if(container == null)
				throw new ArgumentNullException("Validation container is not specified.");
			bool isSuccess = true;
			ASPxHtmlEditor editor = container as ASPxHtmlEditor;
			if(editor != null) {
				if(string.IsNullOrEmpty(validationGroup) || validationGroup == editor.SettingsValidation.ValidationGroup) {
					if(validateInvisibleEditors || editor.IsVisibleAndClientVisible())
						return proc(editor);
				}
			} else if(validateInvisibleEditors || container.Visible) {
				if(container.HasControls())
					for(int i = 0; i < container.Controls.Count; i++)
						isSuccess = ProcessingEditorsInContainer(proc, container.Controls[i], validationGroup, validateInvisibleEditors) && isSuccess;
			}
			return isSuccess;
		}
		protected override void RegisterCustomSpriteCssFile(string spriteCssFile) {
			base.RegisterCustomSpriteCssFile(spriteCssFile);
			if(Images.MenuIconSet != MenuIconSetType.NotSet)
				Images.IconImages.RegisterIconSpriteCssFile(Page);
		}
		protected override void RegisterDefaultSpriteCssFile() {
			base.RegisterDefaultSpriteCssFile();
			Images.IconImages.RegisterIconSpriteCssFile(Page);
		}
		public override void RegisterStyleSheets() {
			base.RegisterStyleSheets();
			if(SpellChecker != null)
				SpellChecker.RegisterStyleSheets();
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(RenderHelper.CssFilesStateKey, GetClientCssFiles());
			result.Add(RenderHelper.ClientStateKey, ClientState.GetSerializableStateString());
			return result;
		}
		protected void GetCreateHoverState(StringBuilder stb, string localVarName) {
			HtmlEditorErrorFrameCloseButtonStyle style = GetErrorFrameCloseButtonHoverStyle();
			string cssClass = style.CssClass;
			string styleAttributes = style.GetStyleAttributes(Page).Value;
			Hashtable result = new Hashtable();
			result["name"] = ClientID;
			result["element"] = GetErrorFrameCloseButtonID();
			result["className"] = cssClass != null ? new string[] { cssClass } : new string[] { "" };
			result["cssText"] = styleAttributes != null ? new string[] { styleAttributes } : new string[] { "" };
			result["postfixes"] = new string[] { "" };
			result["imageUrls"] = new string[] { ResourceManager.ResolveClientUrl(this, GetErrorFrameCloseButtonImage().UrlHottracked) };
			result["imagePostfixes"] = new string[] { ButtonImageIdPostfix };
			stb.AppendFormat("{0}.templateHoverErrorFrameCloseButton = eval({1});\n", localVarName, HtmlConvertor.ToJSON(result));
		}
		string GetRenderCssFilePath() {
			string renderCssFile = GetCustomRenderCssFilePath();
			if(!string.IsNullOrEmpty(renderCssFile))
				return ResourceManager.GetResourceUrl(Page, renderCssFile);
			return ResourceManager.GetResourceUrl(Page, typeof(ASPxHtmlEditor), HtmlEditorDefaultCssResourceName);
		}
		protected internal List<string> GetClientCssFiles() {
			List<string> result = new List<string>();
			string renderCssFilePath = GetRenderCssFilePath();
			if(!string.IsNullOrEmpty(renderCssFilePath))
				result.Add(renderCssFilePath);
			result.Add(ResourceManager.GetResourceUrl(Page, typeof(ASPxHtmlEditor), HtmlEditorSystemDesignViewCssResourceName));
			foreach(HtmlEditorCssFile cssFile in Document.CssFiles) {
				if(!string.IsNullOrEmpty(cssFile.FilePath))
					result.Add(ResolveClientUrl(cssFile.FilePath));
			}
			return result;
		}
		protected virtual bool IsShowOneWordInTextPreview() {
			return false;
		}
		protected bool AreDictionariesAssigned() {
			for(int i = 0; i < SettingsSpellChecker.Dictionaries.Count; i++)
				if(IsDictionaryAssigned(SettingsSpellChecker.Dictionaries[i]))
					return true;
			return false;
		}
		protected bool IsDictionaryAssigned(WebDictionary dictionary) {
			bool ret = false;
			if(dictionary is ASPxSpellCheckerOpenOfficeDictionary) {
				ASPxSpellCheckerOpenOfficeDictionary curDic = (ASPxSpellCheckerOpenOfficeDictionary)dictionary;
				ret = !string.IsNullOrEmpty(curDic.DictionaryPath) && !string.IsNullOrEmpty(curDic.GrammarPath);
			} else if(dictionary is ASPxSpellCheckerISpellDictionary) {
				ASPxSpellCheckerISpellDictionary curDic = (ASPxSpellCheckerISpellDictionary)dictionary;
				ret = !string.IsNullOrEmpty(curDic.AlphabetPath) && !string.IsNullOrEmpty(curDic.DictionaryPath) &&
					!string.IsNullOrEmpty(curDic.GrammarPath);
			} else if(dictionary is ASPxHunspellDictionary) {
				ASPxHunspellDictionary curDic = (ASPxHunspellDictionary)dictionary;
				ret = !string.IsNullOrEmpty(curDic.DictionaryPath) && !string.IsNullOrEmpty(curDic.GrammarPath);
			} else {
				ASPxSpellCheckerDictionary curDic = (ASPxSpellCheckerDictionary)dictionary;
				ret = !string.IsNullOrEmpty(curDic.AlphabetPath) && !string.IsNullOrEmpty(curDic.DictionaryPath);
			}
			return ret;
		}
		HtmlEditorShortcutCollection fullShortcutsCollection = null;
		protected internal HtmlEditorShortcutCollection FullShortcutsCollection {
			get {
				if(fullShortcutsCollection == null) {
					fullShortcutsCollection = new HtmlEditorShortcutCollection(this);
					fullShortcutsCollection.CreateDefaultItems();
					foreach(HtmlEditorShortcut shortCut in Shortcuts) {
						(fullShortcutsCollection as IList).Add(shortCut);
					}
				}
				return fullShortcutsCollection;
			}
			private set { fullShortcutsCollection = value; }
		}
		protected internal string AddShortcutToTooltip(string Tooltip, string commandName) {
			string shortcut = FullShortcutsCollection.FindShortcutByActionName(commandName);
			if(string.IsNullOrEmpty(shortcut) || Tooltip.Contains(shortcut))
				return Tooltip;
			if(string.IsNullOrEmpty(Tooltip))
				return shortcut;
			return string.Format("{0} ({1})", Tooltip, shortcut);
		}
		protected virtual bool LoadPostedHtml() {
			string html = GetPostedHtml();
			return html != null ? Document.LoadHtml(html) : false;
		}
		protected virtual string GetPostedHtml() {
			return GetClientObjectStateValueString(RenderHelper.ContentHtmlStateKey);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null)
				return false;
			if(ClientState.Load(GetClientObjectStateValueString(RenderHelper.ClientStateKey)))
				SyncClientState();
			if(Page != null && Page.IsCallback && ToolbarMode == HtmlEditorToolbarMode.Ribbon)
				RibbonHelper.SyncRibbonControlCollection(RibbonTabs, HtmlEditorControl.BarDockControl.BarDockRender.RibbonControl, postCollection);
			LoadCssFiles();
			bool dataChanged = LoadPostedHtml();
			if(Visible && !IsDialogCallback()) {
				string clientValidationStateStr = GetClientObjectStateValueString(RenderHelper.ValidationStateKey);
				if(!string.IsNullOrEmpty(clientValidationStateStr)) {
					IsValid = false;
					ErrorText = clientValidationStateStr;
				}
				if(dataChanged)
					OnHtmlChanged(EventArgs.Empty);
				return dataChanged;
			}
			return false;
		}
		void LoadCssFiles() {
			string renderCssFilePath = GetRenderCssFilePath();
			ArrayList cssFiles = GetClientObjectStateValue<ArrayList>(RenderHelper.CssFilesStateKey);
			foreach(string cssFile in cssFiles) {
				if(cssFile == renderCssFilePath)
					continue;
				string cssFilePath = cssFile.StartsWith(@"/") ? UrlUtils.ToAppRelative(cssFile) : cssFile;
				CssFiles.Add(new HtmlEditorCssFile(cssFilePath));
			}
			CssFiles.RemoveDuplicates();
		}
		void SyncClientState() {
			if(!NeedLoadClientState())
				ActiveViewInternal = ClientState.ActiveView;
			if(ClientState.CurrentWidth > 0 && SettingsResize.AllowResize) {
				Width = ClientState.IsPercentWidth
					? Unit.Percentage(ClientState.CurrentWidth)
					: Unit.Pixel(ClientState.CurrentWidth);
			}
			if(ClientState.CurrentHeight > 0 && SettingsResize.AllowResize)
				Height = Unit.Pixel(ClientState.CurrentHeight);
			foreach(HtmlEditorToolbar toolbar in this.Toolbars)
				foreach(HtmlEditorToolbarItem item in toolbar.Items.GetVisibleItems()) {
					ToolbarColorButtonBase colorButton = item as ToolbarColorButtonBase;
					if(colorButton != null) {
						if(item.CommandName == ToolbarFontColorButton.DefaultCommandName && !string.IsNullOrEmpty(ClientState.ForeColorPalette))
							colorButton.ColorNestedControlProperties.DeserializeColorsToCustomColorTableItems(ClientState.ForeColorPalette);
						if(item.CommandName == ToolbarBackColorButton.DefaultCommandName && !string.IsNullOrEmpty(ClientState.BackColorPalette))
							colorButton.ColorNestedControlProperties.DeserializeColorsToCustomColorTableItems(ClientState.BackColorPalette);
					}
				}
		}
		protected override void RaisePostDataChangedEvent() {
			Validate();
		}
		protected override void LoadClientState(string state) {
			if(ClientState.Load(state))
				SyncClientState();
		}
		protected override string SaveClientState() {
			return ClientState.ToString();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal void OnRibbonClientLayout(object sender, ASPxClientLayoutArgs e) {
			if(e.LayoutMode == ClientLayoutMode.Loading)
				e.LayoutData = ClientState.Ribbon;
			else
				ClientState.Ribbon = e.LayoutData;
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override object GetCallbackResult() {
			ASPxHtmlEditorCallbackReader reader = new ASPxHtmlEditorCallbackReader(internalCallbackArgument);
			EnsureChildControls(); 
			if(reader.IsViewModeSwitchCallback) {
				Hashtable callbackResult = new Hashtable();
				switch(ActiveView) {
					case HtmlEditorView.Design:
						callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SwitchToDesignViewCallbackPrefix;
						break;
					case HtmlEditorView.Html:
						callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SwitchToHtmlViewCallbackPrefix;
						break;
					case HtmlEditorView.Preview:
						callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SwitchToPreviewCallbackPrefix;
						break;
				}
				PrepareCallbackResult(callbackResult, Document.Html);
				return callbackResult;
			} else if(reader.IsSpellCheckCallback) {
				Hashtable callbackResult = new Hashtable();
				if(reader.SpellCheck != null)
					callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SpellCheckPrefix;
				else if(reader.SpellCheckLoadControl != null)
					callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SpellCheckLoadControlPrefix;
				else if(reader.SpellCheckerOptions != null)
					callbackResult[CallbackResultProperty.Action] = ASPxHtmlEditorCallbackReader.SpellCheckerOptionsPrefix;
				PrepareCallbackResult(callbackResult, Document.GetProcessedHtml(Document.Html, false, true));
				PrepareSpellCheckerCallbackResult(reader, callbackResult);
				return callbackResult;
			}
			else if(reader.IsDialogFormCallback) {
				return GetDialogFormRenderResult(reader.DialogFormName);
			}
			else if(reader.IsImageUploadCallback) {
				bool isThumbnailImage = reader.ThumbnailImageWidth > 0 && reader.ThumbnailImageHeight > 0;
				return isThumbnailImage ? GetCreateThumbnailImageCallbackResult(reader.ImageUrl,
					reader.ThumbnailImageFileName, reader.ThumbnailImageWidth,
					reader.ThumbnailImageHeight) : GetSaveImageFromUrlCallbackResult(reader.ImageUrl);
			}
			else if(reader.IsFlashUploadCallback)
				return GetSaveFlashFromUrlCallbackResult(reader.FlashUrl);
			else if(reader.IsAudioUploadCallback)
				return GetSaveAudioFromUrlCallbackResult(reader.AudioUrl);
			else if(reader.IsVideoUploadCallback)
				return GetSaveVideoFromUrlCallbackResult(reader.VideoUrl);
			else if(reader.IsFileManagerCallback)
				return GetFileManagerCallbackResult(reader.FileManagerCallbackData, CurrentDialogName);
			else
				throw new InvalidDataException("Invalid callback argument.");
		}
		void PrepareCallbackResult(Hashtable callbackResult, string html) {
			if(html != null) {
				Html = html;
				callbackResult[CallbackResultProperty.Html] = Html;
			}
			if(ScriptsAllowed)
				callbackResult[CallbackResultProperty.AllowScripts] = true;
		}
		void PrepareSpellCheckerCallbackResult(ASPxHtmlEditorCallbackReader reader, Hashtable callbackResult) {
			SpellChecker.Visible = true;
			if(reader.SpellCheckLoadControl != null)
				callbackResult[CallbackResultProperty.SpellCheckLoadControl] = RenderUtils.GetRenderResult(SpellChecker);
			if(reader.SpellCheck != null)
				SpellChecker.CreateCallbackReader(reader.SpellCheck, GetProcessedHtmlForSpellChecker(Html));
			if(reader.SpellCheckLoadControl != null)
				SpellChecker.CreateCallbackReader(reader.SpellCheckLoadControl, GetProcessedHtmlForSpellChecker(Html));
			if(reader.SpellCheckerOptions != null)
				SpellChecker.CreateCallbackReader(reader.SpellCheckerOptions, "");
			SpellChecker.RegisterIncludeScripts();
			callbackResult[CallbackResultProperty.SpellCheck] = SpellChecker.GetSpellCheckResult();
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			this.internalCallbackArgument = eventArgument;
		}
		protected internal virtual Control CreateDialogFormControl(string dialogName, Control parent) {
			if(!IsCustomDialog) {
				string dialogFormName = DialogFormNameDictionary[dialogName];
#pragma warning disable 618
				string userSpecifiedFormPath = SettingsForms.GetFormPath(dialogFormName);
#pragma warning restore 618
				Control userControl;
				if(string.IsNullOrEmpty(userSpecifiedFormPath)) {
					userControl = CreateDefaultForm(dialogFormName);
					PrepareUserControl(userControl, parent, dialogName, true);
				}
				else {
					userControl = CreateUserControl(userSpecifiedFormPath);
					PrepareUserControl(userControl, parent, dialogName, false);
				}
				return userControl;
			}
			else {
				string customDialogName = dialogName.Substring(CustomDialogNamePrefix.Length);
				HtmlEditorCustomDialog dialogInfo = CustomDialogs[customDialogName];
				if(dialogInfo != null) {
					if(string.IsNullOrEmpty(dialogInfo.FormPath))
						throw new Exception(StringResources.HtmlEditorExceptionText_CustomDialogFormNameNotSpecified);
					Control userControl = new CustomDialogsContainer(dialogInfo, CreateUserControl(dialogInfo.FormPath));
					PrepareUserControl(userControl, parent, dialogInfo.Name, false);
					return userControl;
				}
				throw new Exception(StringResources.HtmlEditorExceptionText_CustomDialogNotFound);
			}
		}
		protected object GetFileManagerCallbackResult(string callbackArgs, string dialogName) {
			HtmlEditorFileManager fileManager = GetDialogFileManager(dialogName);
			fileManager.IsHtmlEditorCallback = this.IsCallback;
			return fileManager.GetCallbackResult(callbackArgs);
		}
		protected HtmlEditorFileManager GetDialogFileManager(string dialogName) {
			return FindControlHelper.LookupControlRecursive(
				GetCurrentDialogControl(dialogName),
				"FileManager"
			) as HtmlEditorFileManager;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			string[] parts = eventArgument.Split('_');
			string command = parts[0];
			switch(command) {
				case "Export":
					string format = parts[1];
					Export((HtmlEditorExportFormat)Enum.Parse(typeof(HtmlEditorExportFormat), format));
					break;
				default:
					throw new NotImplementedException();
			}
		}
		protected override string GetSkinControlName() {
			return "HtmlEditor";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Web", "Editors", "SpellChecker" };
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new HtmlEditorClientSideEvents();
		}
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new ASPxHtmlEditorLoadingPanelSettings(this);
		}
		protected virtual HtmlEditorCustomDialogs CreateCustomDialogs() {
			return new HtmlEditorCustomDialogs(this);
		}
		protected virtual HtmlEditorContextMenuItemCollection CreateContextMenuItems() {
			return new HtmlEditorContextMenuItemCollection(this);
		}
		protected internal HeaderButtonImageProperties GetErrorFrameCloseButtonImage() {
			HeaderButtonImageProperties image = new HeaderButtonImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, HtmlEditorImages.ErrorFrameCloseButtonImageName));
			image.CopyFrom(Images.ErrorFrameCloseButton);
			return image;
		}
		public ImageProperties GetInsertImageDialogConstrainProportionsBottom() {
			return GetImageProperties(HtmlEditorImages.ConstrainProportionsBottomRtlImageName, HtmlEditorImages.ConstrainProportionsBottomImageName,
				Images.InsertImageDialogConstrainProportionsBottomRtl, Images.InsertImageDialogConstrainProportionsBottom);
		}
		public ImageProperties GetInsertImageDialogConstrainProportionsTop() {
			return GetImageProperties(HtmlEditorImages.ConstrainProportionsTopRtlImageName, HtmlEditorImages.ConstrainProportionsTopImageName,
				Images.InsertImageDialogConstrainProportionsTopRtl, Images.InsertImageDialogConstrainProportionsTop);
		}
		public ImageProperties GetInsertImageDialogConstrainProportionsMiddleOn() {
			return GetImageProperties(HtmlEditorImages.ConstrainProportionsMiddleOnRtlImageName, HtmlEditorImages.ConstrainProportionsMiddleOnImageName,
				Images.InsertImageDialogConstrainProportionsMiddleOnRtl, Images.InsertImageDialogConstrainProportionsMiddleOn);
		}
		public ImageProperties GetInsertImageDialogConstrainProportionsMiddleOff() {
			return GetImageProperties(HtmlEditorImages.ConstrainProportionsOffRtlImageName, HtmlEditorImages.ConstrainProportionsOffImageName,
				Images.InsertImageDialogConstrainProportionsMiddleOffRtl, Images.InsertImageDialogConstrainProportionsMiddleOff);
		}
		public ImageProperties GetStatusBarSizeGripImageProperties() {
			return GetImageProperties(HtmlEditorImages.SizeGripRtlImageName, HtmlEditorImages.SizeGripImageName, Images.SizeGripRtl, Images.SizeGrip);
		}
		protected internal ImageProperties GetTagInspectorSeparatorImageProperties() {
			return GetImageProperties(HtmlEditorImages.TagInspectorSeparatorImageName, HtmlEditorImages.TagInspectorSeparatorImageName,
				Images.TagInspectorSeparator, Images.TagInspectorSeparator);
		}
		protected internal ImageProperties GetInsertImageDialogResetImageProperties() {
			return GetImageProperties(HtmlEditorImages.InsertImageDialogResetImageName, HtmlEditorImages.InsertImageDialogResetImageName,
				Images.InsertImageDialogResetButton, Images.InsertImageDialogResetButton);
		}
		protected internal ImageProperties GetCompletionEnumItemImageProperties() {
			return GetImageProperties(HtmlEditorImages.EnumImageName, HtmlEditorImages.EnumImageName, Images.EnumImage, Images.EnumImage);
		}
		protected internal ImageProperties GetCompletionEventItemImageProperties() {
			return GetImageProperties(HtmlEditorImages.EventImageName, HtmlEditorImages.EventImageName, Images.EventImage, Images.EventImage);
		}
		protected internal ImageProperties GetCompletionFieldItemImageProperties() {
			return GetImageProperties(HtmlEditorImages.FieldImageName, HtmlEditorImages.FieldImageName, Images.FieldImage, Images.FieldImage);
		}
		protected internal ImageProperties GetCompletionXmlItemImageProperties() {
			return GetImageProperties(HtmlEditorImages.XmlItemImageName, HtmlEditorImages.XmlItemImageName, Images.XmlItemImage, Images.XmlItemImage);
		}
		ImageProperties GetImageProperties(string rtlImgName, string imgName, ImageProperties rtlImageProps, ImageProperties imageProps) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, IsRightToLeft() ? rtlImgName : imgName));
			image.CopyFrom(IsRightToLeft() ? rtlImageProps : imageProps);
			return image;
		}
#pragma warning disable 618
		protected internal void SetFormPath(string formName, string value) {
			SettingsForms.SetFormPath(formName, value);
		}
#pragma warning restore 618
		protected Control CreateDefaultForm(string formName) {
			switch(formName) {
				case "InsertImageForm":
					return CreateInsertImageForm();
				case "InsertLinkForm":
					return CreateInsertLinkForm();
				case "InsertTableForm":
					return CreateInsertTableForm();
				case "PasteFromWordForm":
					return CreatePasteFromWordForm();
				case "TableColumnPropertiesForm":
					return CreateTableColumnPropertiesForm();
				case "TableCellPropertiesForm":
					return CreateTableCellPropertiesForm();
				case "TableRowPropertiesForm":
					return CreateTableRowPropertiesForm();
				case "InsertFlashForm":
					return CreateInsertFlashForm();
				case "InsertVideoForm":
					return CreateInsertVideoForm();
				case "InsertYoutubeVideoForm":
					return CreateInsertYouTubeVideoForm();
				case "InsertAudioForm":
					return CreateInsertAudioForm();
				case "InsertPlaceholderForm":
					return CreateInsertPlaceholderForm();
				case "ChangeElementPropertiesForm":
					return CreateChangeElementPropertiesForm();
				default:
					throw new ArgumentException();
			}
		}
		protected virtual Control CreateChangeElementPropertiesForm() {
			return new ChangeElementPropertiesForm();
		}
		protected virtual Control CreateInsertImageForm() {
			return new InsertImageForm();
		}
		protected virtual Control CreateInsertFlashForm() {
			return new InsertFlashForm();
		}
		protected virtual Control CreateInsertVideoForm() {
			return new InsertVideoForm();
		}
		protected virtual Control CreateInsertYouTubeVideoForm() {
			return new InsertYoutubeVideoForm();
		}
		protected virtual Control CreateInsertAudioForm() {
			return new InsertAudioForm();
		}
		protected virtual Control CreateInsertLinkForm() {
			return new InsertLinkForm();
		}
		protected virtual Control CreateInsertTableForm() {
			return new InsertTableForm();
		}
		protected virtual Control CreatePasteFromWordForm() {
			return new PasteFromWordForm();
		}
		protected virtual Control CreateTableColumnPropertiesForm() {
			return new TableColumnPropertiesForm();
		}
		protected virtual Control CreateTableCellPropertiesForm() {
			return new TableCellPropertiesForm();
		}
		protected virtual Control CreateTableRowPropertiesForm() {
			return new TableRowPropertiesForm();
		}
		protected virtual Control CreateInsertPlaceholderForm() {
			return new InsertPlaceholderForm();
		}
		HtmlEditorView GetHtmlEditorView(HtmlEditorView baseView) {
			if(!Settings.AllowDesignViewInternal && baseView == HtmlEditorView.Design)
				return GetHtmlEditorView(HtmlEditorView.Html);
			if(!Settings.AllowHtmlViewInternal && baseView == HtmlEditorView.Html)
				return GetHtmlEditorView(HtmlEditorView.Preview);
			if(!Settings.AllowPreviewInternal && baseView == HtmlEditorView.Preview)
				return GetHtmlEditorView(HtmlEditorView.Design);
			return baseView;
		}
		void ValidateActiveView(HtmlEditorView activeView) {
			if(!IsValidActiveView(activeView))
				throw new ArgumentException(GetInvalidActiveViewExceptionMessage(activeView));
		}
		bool IsValidActiveView(HtmlEditorView activeView) {
			if(Settings.AllowDesignViewInternal && (activeView == HtmlEditorView.Design) ||
				Settings.AllowHtmlViewInternal && (activeView == HtmlEditorView.Html) ||
				Settings.AllowPreviewInternal && (activeView == HtmlEditorView.Preview))
				return true;
			return false;
		}
		string GetInvalidActiveViewExceptionMessage(HtmlEditorView view) {
			string viewName;
			if(view == HtmlEditorView.Design)
				viewName = StringResources.HtmlEditor_DesignView;
			else if(view == HtmlEditorView.Html)
				viewName = StringResources.HtmlEditor_HtmlView;
			else if(view == HtmlEditorView.Preview)
				viewName = StringResources.HtmlEditor_Preview;
			else
				throw new ArgumentException("view");
			return string.Format(StringResources.HtmlEditor_InvalidActiveViewFormat, viewName);
		}
		protected virtual Control GetCurrentDialogControl(string dialogName) {
			return CurrentDialogForm;
		}
		protected string GetDialogFormRenderResult(string dialogName) {
			return GetControlRenderResult(GetCurrentDialogControl(dialogName));
		}
		protected string GetControlRenderResult(Control control) {
			string content = "";
			BeginRendering();
			try {
				if(control != null)
					content = RenderUtils.GetRenderResult(control);
			}
			finally {
				EndRendering();
			}
			return content;
		}
		protected bool IsDialogCallback() {
			return !string.IsNullOrEmpty(CurrentDialogName);
		}
		protected string GetSaveFlashFromUrlCallbackResult(string url) {
			FlashDownloadHelper helper = new FlashDownloadHelper(this);
			return helper.SaveFile(url);
		}
		protected string GetSaveAudioFromUrlCallbackResult(string url) {
			AudioDownloadHelper helper = new AudioDownloadHelper(this);
			return helper.SaveFile(url);
		}
		protected string GetSaveVideoFromUrlCallbackResult(string url) {
			VideoDownloadHelper helper = new VideoDownloadHelper(this);
			return helper.SaveFile(url);
		}
		protected string GetSaveImageFromUrlCallbackResult(string url) {
			ImageDownloadHelper helper = new ImageDownloadHelper(this);
			return helper.SaveFile(url);
		}
		protected string GetCreateThumbnailImageCallbackResult(string url, string fileName, int thumbnailWidth, int thumbnailHeight) {
			ImageThumbnailDownloadHelper helper = new ImageThumbnailDownloadHelper(this, thumbnailWidth, thumbnailHeight);
			return helper.SaveFile(url, fileName);
		}
		protected internal new void PropertyChanged(string propName) {
			base.PropertyChanged(propName);
		}
		string IValidationSummaryEditor.ValidationGroup {
			get { return SettingsValidation.ValidationGroup; }
			set { SettingsValidation.ValidationGroup = value; }
		}
		bool IValidationSummaryEditor.NotifyValidationSummariesToAcceptNewError {
			get { return NotifyValidationSummariesToAcceptNewError; }
			set { NotifyValidationSummariesToAcceptNewError = value; }
		}
		bool IValidationSummaryEditor.IsValidationEnabled() {
			return IsValidationEnabledCore();
		}
		public override void DataBind() {
			base.DataBind();
			LayoutChanged();
		}
		protected internal new bool IsRightToLeft() {
			return base.IsRightToLeft();
		}
		public void Export(HtmlEditorExportFormat format) {
			Document.Export(format);
		}
		public void Export(HtmlEditorExportFormat format, bool saveAsFile) {
			Document.Export(format, saveAsFile);
		}
		public void Export(HtmlEditorExportFormat format, string fileName) {
			Document.Export(format, fileName);
		}
		public void Export(HtmlEditorExportFormat format, Stream outputStream) {
			Document.Export(outputStream, format);
		}
		public void Import(string filePath) {
			Document.Import(filePath);
		}
		public void Import(string filePath, bool useInlineStyles) {
			Document.Import(filePath, useInlineStyles);
		}
		public void Import(string filePath, string contentFolder) {
			Document.Import(filePath, contentFolder);
		}
		public void Import(string filePath, bool useInlineStyles, string contentFolder) {
			Document.Import(filePath, useInlineStyles, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, string filePath) {
			Document.Import(format, filePath);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles) {
			Document.Import(format, filePath, useInlineStyles);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, string contentFolder) {
			Document.Import(format, filePath, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles, string contentFolder) {
			Document.Import(format, filePath, useInlineStyles, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, Stream inputStream) {
			Document.Import(format, inputStream);
		}
		public void Import(HtmlEditorImportFormat format, Stream inputStream, bool useInlineStyles) {
			Document.Import(format, inputStream, useInlineStyles);
		}
		public void Import(HtmlEditorImportFormat format, Stream inputStream, string contentFolder) {
			Document.Import(format, inputStream, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, Stream inputStream, bool useInlineStyles, string contentFolder) {
			Document.Import(format, inputStream, useInlineStyles, contentFolder);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxHtmlEditor.Design.HtmlEditorCommonFormDesigner"; } }
		protected internal virtual MediaFileSelector CreateMediaFileSelector() {
			return new ASPxHtmlEditorMediaFileSelector();
		}
		protected internal virtual HtmlEditorFileManager CreateFileManager() {
			return new HtmlEditorFileManager();
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public abstract class ASPxEditBaseAccessor : ASPxEditBase {
		protected internal const string EditScriptResourceAccessName = EditScriptResourceName;
	}
}
internal class ToolboxBitmapAccess { }
