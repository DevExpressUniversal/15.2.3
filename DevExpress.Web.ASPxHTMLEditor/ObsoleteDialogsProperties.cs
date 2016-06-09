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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorFormsSettings : ASPxHtmlEditorSettingsBase {
		public HtmlEditorFormsSettings(IPropertiesOwner owner)
			: base(owner) {
#pragma warning disable 618
				SpellCheckerForms = CreateSpellCheckerFormsSettings();
#pragma warning restore 618
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertImageFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertImageFormPath {
			get { return GetStringProperty("InsertImageFormPath", ""); }
			set { SetStringProperty("InsertImageFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertAudioFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertAudioFormPath {
			get { return GetStringProperty("InsertAudioFormPath", ""); }
			set { SetStringProperty("InsertAudioFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertFlashFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertFlashFormPath {
			get { return GetStringProperty("InsertFlashFormPath", ""); }
			set { SetStringProperty("InsertFlashFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertVideoFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertVideoFormPath {
			get { return GetStringProperty("InsertVideoFormPath", ""); }
			set { SetStringProperty("InsertVideoFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertYouTubeVideoFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertYouTubeVideoFormPath {
			get { return GetStringProperty("InsertYoutubeVideoFormPath", ""); }
			set { SetStringProperty("InsertYoutubeVideoFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertLinkFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertLinkFormPath {
			get { return GetStringProperty("InsertLinkFormPath", ""); }
			set { SetStringProperty("InsertLinkFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertTableFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string InsertTableFormPath {
			get { return GetStringProperty("InsertTableFormPath", ""); }
			set { SetStringProperty("InsertTableFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsPasteFromWordFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string PasteFromWordFormPath {
			get { return GetStringProperty("PasteFromWordFormPath", ""); }
			set { SetStringProperty("PasteFromWordFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsTableColumnPropertiesFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete.")]
		public string TableColumnPropertiesFormPath {
			get { return GetStringProperty("TableColumnPropertiesFormPath", ""); }
			set { SetStringProperty("TableColumnPropertiesFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsInsertImageCssClassItems"),
#endif
		NotifyParentProperty(true), MergableProperty(false), AutoFormatDisable, Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog.CssClassItems property instead.")]
		public InsertImageCssClassItems InsertImageCssClassItems {
			get { return GetDefaultDialogsSettings().InsertImageDialog.CssClassItems; }
		}
		[
		NotifyParentProperty(true), MergableProperty(false), AutoFormatDisable, Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog.CssClassItems property instead.")]
		public InsertMediaCssClassItems InsertAudioCssClassItems {
			get { return GetDefaultDialogsSettings().InsertAudioDialog.CssClassItems; }
		}
		[
		NotifyParentProperty(true), MergableProperty(false), AutoFormatDisable, Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog.CssClassItems property instead.")]
		public InsertMediaCssClassItems InsertFlashCssClassItems {
			get { return GetDefaultDialogsSettings().InsertFlashDialog.CssClassItems; }
		}
		[
		NotifyParentProperty(true), MergableProperty(false), AutoFormatDisable, Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog.CssClassItems property instead.")]
		public InsertMediaCssClassItems InsertVideoCssClassItems {
			get { return GetDefaultDialogsSettings().InsertVideoDialog.CssClassItems; }
		}
		[
		NotifyParentProperty(true), MergableProperty(false), AutoFormatDisable, Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertYouTubeVideoDialog.CssClassItems property instead.")]
		public InsertMediaCssClassItems InsertYouTubeVideoCssClassItems {
			get { return GetDefaultDialogsSettings().InsertYouTubeVideoDialog.CssClassItems; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFormsSettingsSpellCheckerForms"),
#endif
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable, Browsable(false)]
		public SpellCheckerFormsSettings SpellCheckerForms { get; private set; }
		protected virtual SpellCheckerFormsSettings CreateSpellCheckerFormsSettings() {
			return new SpellCheckerFormsSettings(this);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorFormsSettings src = source as HtmlEditorFormsSettings;
				if(src != null) {
					SpellCheckerForms.Assign(src.SpellCheckerForms);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal string GetFormPath(string formName) {
			return GetStringProperty(string.Format("{0}Path", formName), "");
		}
		protected internal void SetFormPath(string formName, string value) {
			SetStringProperty(string.Format("{0}Path", formName), "", value);
		}
		protected virtual HtmlEditorDefaultDialogSettings GetDefaultDialogsSettings() {
			return HtmlEditor.SettingsDialogs;
		}
#pragma warning disable 618
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { SpellCheckerForms };
		}
#pragma warning restore 618
	}
	public class HtmlEditorDialogFormElementSettings : ASPxHtmlEditorSettingsBase {
		public HtmlEditorDialogFormElementSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertImageDialog"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog property instead.")]
		public HtmlEditorInsertImageDialogSettings InsertImageDialog {
			get { return GetDefaultDialogsSettings().InsertImageDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertFlashDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog property instead.")]
		public HtmlEditorInsertMediaDialogSettings InsertFlashDialog {
			get { return GetDefaultDialogsSettings().InsertFlashDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertAudioDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog property instead.")]
		public HtmlEditorInsertMediaDialogSettings InsertAudioDialog {
			get { return GetDefaultDialogsSettings().InsertAudioDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertVideoDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog property instead.")]
		public HtmlEditorInsertMediaDialogSettings InsertVideoDialog {
			get { return GetDefaultDialogsSettings().InsertVideoDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertYouTubeVideoDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertYouTubeVideoDialog property instead.")]
		public HtmlEditorInsertMediaDialogSettingsBase InsertYouTubeVideoDialog {
			get { return GetDefaultDialogsSettings().InsertYouTubeVideoDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertLinkDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertLinkDialog property instead.")]
		public HtmlEditorInsertLinkDialogSettings InsertLinkDialog {
			get { return GetDefaultDialogsSettings().InsertLinkDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsInsertTableDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertTableDialog property instead.")]
		public HtmlEditorInsertTableDialogSettings InsertTableDialog {
			get { return GetDefaultDialogsSettings().InsertTableDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsPasteFromWordDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.PasteFromWordDialog property instead.")]
		public HtmlEditorPasteFromWordDialogSettings PasteFromWordDialog {
			get { return GetDefaultDialogsSettings().PasteFromWordDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsTableCellPropertiesDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.TableCellPropertiesDialog property instead.")]
		public HtmlEditorTableCellPropertiesDialogSettings TableCellPropertiesDialog {
			get { return GetDefaultDialogsSettings().TableCellPropertiesDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsTableColumnPropertiesDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.TableColumnPropertiesDialog property instead.")]
		public HtmlEditorTableElementPropertiestDialogSettings TableColumnPropertiesDialog {
			get { return GetDefaultDialogsSettings().TableColumnPropertiesDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsTableRowPropertiesDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.TableRowPropertiesDialog property instead.")]
		public HtmlEditorTableElementPropertiestDialogSettings TableRowPropertiesDialog {
			get { return GetDefaultDialogsSettings().TableRowPropertiesDialog; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDialogFormElementSettingsCheckSpellingDialog"),
#endif
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		Obsolete("This property is now obsolete. Use the SettingsDialogs.CheckSpellingDialog property instead.")]
		public SpellCheckerDialogSettings CheckSpellingDialog {
			get { return GetDefaultDialogsSettings().CheckSpellingDialog; }
		}
		protected virtual HtmlEditorDefaultDialogSettings GetDefaultDialogsSettings() {
			return HtmlEditor.SettingsDialogs;
		}
	}
}
