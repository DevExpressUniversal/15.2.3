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
using System.Linq;
namespace DevExpress.Web.ASPxHtmlEditor {
	using DevExpress.Web.ASPxHtmlEditor.Localization;
	public class HtmlEditorUserControl : UserControl, IDialogFormElementRequiresLoad {
		private ASPxHtmlEditor htmlEditor = null;
		protected ASPxHtmlEditor HtmlEditor {
			get {
				if(htmlEditor == null)
					htmlEditor = FindParentHtmlEditor();
				return htmlEditor;
			}
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorUserControlClientID")]
#endif
public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		protected override void OnInit(EventArgs e) {
			ClientIDHelper.UpdateClientIDMode(this);
			base.OnInit(e);
			CreateControlHierarchy();
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			PrepareChildControls();
		}
		protected virtual ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { };
		}
		protected virtual ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { };
		}
		protected virtual ASPxHtmlEditorRoundPanel GetChildDxHtmlEditorRoundPanel() {
			return null;
		}
		protected virtual ASPxHtmlEditorRoundPanel[] GetChildDxHtmlEditorRoundPanels() {
			List<ASPxHtmlEditorRoundPanel> roundPanels = new List<ASPxHtmlEditorRoundPanel>();
			ASPxHtmlEditorRoundPanel firstPanel = GetChildDxHtmlEditorRoundPanel();
			if(firstPanel != null)
				roundPanels.Add(firstPanel);
			return roundPanels.ToArray();
		}
		private ASPxHtmlEditor FindParentHtmlEditor() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxHtmlEditor)
					return curControl as ASPxHtmlEditor;
				curControl = curControl.Parent;
			}
			return null;
		}
		protected virtual void CreateControlHierarchy() {
		}
		protected virtual void PrepareChildControls() {
			ApplyStylesToChildControls(HtmlEditor, GetChildDxEdits(), GetChildDxButtons(), GetChildDxHtmlEditorRoundPanels());
		}
		protected void AddTemplateToControl(Control destinationContainer, ITemplate template) {
			if(template == null || destinationContainer == null)
				return;
			template.InstantiateIn(destinationContainer);
		}
		protected internal static void PrepareChildDXControls(ControlCollection controls, ASPxHtmlEditor htmlEditor) {
			ASPxEditBase[] dxEdits = GetAllChildDXControls<ASPxEditBase>(controls).ToArray();
			ASPxButton[] dxButtons = GetAllChildDXControls<ASPxButton>(controls).ToArray();
			ASPxHtmlEditorRoundPanel[] panels = GetAllChildDXControls<ASPxHtmlEditorRoundPanel>(controls).ToArray();
			ApplyStylesToChildControls(htmlEditor, dxEdits, dxButtons, panels);
		}
		protected internal static void ApplyStylesToChildControls(ASPxHtmlEditor htmlEditor, ASPxEditBase[] dxEdits, ASPxButton[] dxButtons, ASPxHtmlEditorRoundPanel[] panels) {
			foreach(ASPxEditBase edit in dxEdits) {
				if(edit == null)
					continue;
				edit.ParentStyles = htmlEditor.StylesEditors;
				edit.ParentImages = htmlEditor.ImagesEditors;
			}
			foreach(ASPxButton btn in dxButtons) {
				btn.ParentStyles = htmlEditor.StylesButton;
			}
			for(int i = 0; i < panels.Length; i++) {
				panels[i].Parts.Assign(htmlEditor.PartsRoundPanelInternal);
				panels[i].ControlStyle.CopyFrom(htmlEditor.StylesRoundPanel.ControlStyle);
			}
		}
		protected internal static IEnumerable<T> GetAllChildDXControls<T>(ControlCollection controls) {
			List<T> result = new List<T>() { };
			result.AddRange(controls.OfType<T>());
			foreach(Control control in controls) {
				if(control.Controls != null && control.Controls.Count > 0) {
					result.AddRange(GetAllChildDXControls<T>(control.Controls));
				}
			}
			return result;
		}
		void IDialogFormElementRequiresLoad.ForceInit() {
			FrameworkInitialize();
		}
		void IDialogFormElementRequiresLoad.ForceLoad() {
			CreateControlHierarchy();
			OnLoad(EventArgs.Empty);
		}
	}
	public class HtmlEditorCustomDialog : CollectionItem {
		public HtmlEditorCustomDialog() : base() { }
		public HtmlEditorCustomDialog(string name, string caption)
			: this() {
			Name = name;
			Caption = caption;
		}
		public HtmlEditorCustomDialog(string name, string caption, string formPath)
			: this(name, caption) {
			FormPath = formPath;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HtmlEditorCustomDialogs Collection { get { return (HtmlEditorCustomDialogs)base.Collection; } }
		protected ASPxHtmlEditor Owner { get { return Collection != null ? Collection.Owner : null; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogName"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogCaption"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogFormPath"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UserControlFileEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FormPath {
			get { return GetStringProperty("FormPath", ""); }
			set { SetStringProperty("FormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogDefaultButtonID"),
#endif
		Category("Buttons"), DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string DefaultButtonID {
			get { return GetStringProperty("DefaultButtonID", ""); }
			set { SetStringProperty("DefaultButtonID", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogOkButtonVisible"),
#endif
		Category("Buttons"), DefaultValue(true), NotifyParentProperty(true)]
		public bool OkButtonVisible {
			get { return GetBoolProperty("OkButtonVisible", true); }
			set { SetBoolProperty("OkButtonVisible", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogOkButtonText"),
#endif
		Category("Buttons"), DefaultValue(StringResources.HtmlEditorText_Ok), NotifyParentProperty(true), Localizable(true)]
		public string OkButtonText {
			get { return GetStringProperty("OkButtonText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonOk)); }
			set { SetStringProperty("OkButtonText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonOk), value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogCancelButtonVisible"),
#endif
		Category("Buttons"), DefaultValue(true), NotifyParentProperty(true)]
		public bool CancelButtonVisible {
			get { return GetBoolProperty("CancelButtonVisible", true); }
			set { SetBoolProperty("CancelButtonVisible", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogCancelButtonText"),
#endif
		Category("Buttons"), DefaultValue(StringResources.HtmlEditorText_Cancel), NotifyParentProperty(true), Localizable(true)]
		public string CancelButtonText {
			get { return GetStringProperty("CancelButtonText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonCancel)); }
			set { SetStringProperty("CancelButtonText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonCancel), value); }
		}
		protected override bool IsLoading() {
			if(Owner != null)
				return ((IWebControlObject)Owner).IsLoading();
			return base.IsLoading();
		}
		protected override bool IsDesignMode() {
			if(Owner != null)
				return ((IWebControlObject)Owner).IsDesignMode();
			return base.IsDesignMode();
		}
		protected override bool IsRendering() {
			if(Owner != null)
				return ((IWebControlObject)Owner).IsRendering();
			return base.IsRendering();
		}
		protected override void LayoutChanged() {
			if(Owner != null)
				((IWebControlObject)Owner).LayoutChanged();
			else
				base.LayoutChanged();
		}
		public override void Assign(CollectionItem source) {
			HtmlEditorCustomDialog src = source as HtmlEditorCustomDialog;
			if(src != null) {
				Name = src.Name;
				Caption = src.Caption;
				FormPath = src.FormPath;
				DefaultButtonID = src.DefaultButtonID;
				OkButtonVisible = src.OkButtonVisible;
				OkButtonText = src.OkButtonText;
				CancelButtonVisible = src.CancelButtonVisible;
				CancelButtonText = src.CancelButtonText;
			}
			base.Assign(source);
		}
		public override string ToString() {
			return !string.IsNullOrEmpty(Name)
				? Name
				: base.ToString();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorCustomDialogs : Collection<HtmlEditorCustomDialog> {
		protected internal HtmlEditorCustomDialogs()
			: base() {
		}
		public HtmlEditorCustomDialogs(ASPxHtmlEditor owner)
			: base(owner) {
		}
		public HtmlEditorCustomDialog this[string name] {
			get {
				return Find(delegate(HtmlEditorCustomDialog dialog) {
					return dialog.Name == name;
				});
			}
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCustomDialogsOwner")]
#endif
		public new ASPxHtmlEditor Owner { get { return (ASPxHtmlEditor)base.Owner; } }
		public HtmlEditorCustomDialog Add() {
			return AddInternal(new HtmlEditorCustomDialog());
		}
		public HtmlEditorCustomDialog Add(string name, string caption) {
			return AddInternal(new HtmlEditorCustomDialog(name, caption));
		}
		public HtmlEditorCustomDialog Add(string name, string caption, string formPath) {
			return AddInternal(new HtmlEditorCustomDialog(name, caption, formPath));
		}
		protected override void OnChanged() {
			if(Owner != null)
				(Owner as IWebControlObject).LayoutChanged();
		}
	}
	public class HtmlEditorDefaultDialogSettings : ASPxHtmlEditorSettingsBase {
		public HtmlEditorDefaultDialogSettings(IPropertiesOwner owner)
			: base(owner) {
			InsertImageDialog = CreateInsertImageDialogSettings(owner);
			InsertAudioDialog = CreateInsertAudioDialogSettings(owner);
			InsertFlashDialog = CreateInsertFlashDialogSettings(owner);
			InsertVideoDialog = CreateInsertVideoDialogSettings(owner);
			InsertYouTubeVideoDialog = CreateInsertYouTubeVideoDialogSettings(owner);
			InsertLinkDialog = CreateInsertLinkDialogSettings(owner);
			InsertTableDialog = CreateInsertTableDialogSettings(owner);
			PasteFromWordDialog = CreatePasteFromWordDialogSettings(owner);
			TableCellPropertiesDialog = CreateTableCellPropertiesDialogSettings(owner);
			TableColumnPropertiesDialog = CreateTableColumnPropertiesDialogSettings(owner);
			TableRowPropertiesDialog = CreateTableRowPropertiesDialogSettings(owner);
			CheckSpellingDialog = CreateSpellCheckerDialogSettings(owner);
			InsertPlaceholderDialog = CreateInsertPlaceholderDialogSettings(owner);
			ChangeElementPropertiesDialog = CreateChangeElementPropertiesDialogSettings(owner);
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertImageDialogSettings InsertImageDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertFlashDialogSettings InsertFlashDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertAudioDialogSettings InsertAudioDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertVideoDialogSettings InsertVideoDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertMediaDialogSettingsBase InsertYouTubeVideoDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertLinkDialogSettings InsertLinkDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorInsertTableDialogSettings InsertTableDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorPasteFromWordDialogSettings PasteFromWordDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorTableCellPropertiesDialogSettings TableCellPropertiesDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorTableElementPropertiestDialogSettings TableColumnPropertiesDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorTableElementPropertiestDialogSettings TableRowPropertiesDialog { get; private set; }
		[Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorDialogSettingsBase InsertPlaceholderDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HtmlEditorChangeElementPropertiestDialogSettings ChangeElementPropertiesDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpellCheckerDialogSettings CheckSpellingDialog { get; private set; }
		protected virtual HtmlEditorInsertImageDialogSettings CreateInsertImageDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertImageDialogSettings(owner);
		}
		protected virtual HtmlEditorInsertFlashDialogSettings CreateInsertFlashDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertFlashDialogSettings(owner);
		}
		protected virtual HtmlEditorInsertAudioDialogSettings CreateInsertAudioDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertAudioDialogSettings(owner);
		}
		protected virtual HtmlEditorInsertVideoDialogSettings CreateInsertVideoDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertVideoDialogSettings(owner);
		}
		protected virtual HtmlEditorInsertMediaDialogSettingsBase CreateInsertYouTubeVideoDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertMediaDialogSettingsBase(owner);
		}
		protected virtual HtmlEditorInsertLinkDialogSettings CreateInsertLinkDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertLinkDialogSettings(owner);
		}
		protected virtual HtmlEditorInsertTableDialogSettings CreateInsertTableDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorInsertTableDialogSettings(owner);
		}
		protected virtual HtmlEditorPasteFromWordDialogSettings CreatePasteFromWordDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorPasteFromWordDialogSettings(owner);
		}
		protected virtual HtmlEditorTableCellPropertiesDialogSettings CreateTableCellPropertiesDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorTableCellPropertiesDialogSettings(owner);
		}
		protected virtual HtmlEditorTableElementPropertiestDialogSettings CreateTableColumnPropertiesDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorTableElementPropertiestDialogSettings(owner);
		}
		protected virtual HtmlEditorTableElementPropertiestDialogSettings CreateTableRowPropertiesDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorTableElementPropertiestDialogSettings(owner);
		}
		protected virtual SpellCheckerDialogSettings CreateSpellCheckerDialogSettings(IPropertiesOwner owner) {
			return new SpellCheckerDialogSettings(owner);
		}
		protected virtual HtmlEditorDialogSettingsBase CreateInsertPlaceholderDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorDialogSettingsBase(owner);
		}
		protected virtual HtmlEditorChangeElementPropertiestDialogSettings CreateChangeElementPropertiesDialogSettings(IPropertiesOwner owner) {
			return new HtmlEditorChangeElementPropertiestDialogSettings(owner);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settigns = source as HtmlEditorDefaultDialogSettings;
				if(settigns != null) {
					InsertImageDialog.Assign(settigns.InsertImageDialog);
					InsertLinkDialog.Assign(settigns.InsertLinkDialog);
					InsertTableDialog.Assign(settigns.InsertTableDialog);
					PasteFromWordDialog.Assign(settigns.PasteFromWordDialog);
					TableCellPropertiesDialog.Assign(settigns.TableCellPropertiesDialog);
					TableColumnPropertiesDialog.Assign(settigns.TableColumnPropertiesDialog);
					TableRowPropertiesDialog.Assign(settigns.TableRowPropertiesDialog);
					CheckSpellingDialog.Assign(settigns.CheckSpellingDialog);
					InsertAudioDialog.Assign(settigns.InsertAudioDialog);
					InsertVideoDialog.Assign(settigns.InsertVideoDialog);
					InsertFlashDialog.Assign(settigns.InsertFlashDialog);
					InsertYouTubeVideoDialog.Assign(settigns.InsertYouTubeVideoDialog);
					ChangeElementPropertiesDialog.Assign(settigns.ChangeElementPropertiesDialog);
					InsertPlaceholderDialog.Assign(settigns.InsertPlaceholderDialog);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				InsertImageDialog, InsertLinkDialog, InsertTableDialog, PasteFromWordDialog,
				TableCellPropertiesDialog, TableColumnPropertiesDialog, TableRowPropertiesDialog, CheckSpellingDialog,
				InsertAudioDialog, InsertFlashDialog, InsertVideoDialog, InsertYouTubeVideoDialog, InsertPlaceholderDialog,
				ChangeElementPropertiesDialog
			});
		}
	}
	public class HtmlEditorInsertMediaDialogSettingsBase : HtmlEditorDialogSettingsBase {
		ITemplate moreOptionsSectionTemplate;
		InsertMediaCssClassItems cssClassItems;
		ASPxHtmlEditorUploadSettingsBase uploadSettingsInternal;
		HtmlEditorSelectorSettings selectorSettingsInternal;
		public HtmlEditorInsertMediaDialogSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate MoreOptionsSectionTemplate {
			get { return moreOptionsSectionTemplate; }
			set {
				if(moreOptionsSectionTemplate != value)
					moreOptionsSectionTemplate = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertMediaDialogSettingsBaseShowMoreOptionsButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowMoreOptionsButton {
			get { return GetBoolProperty("ShowMoreOptionsButton", true); }
			set { SetBoolProperty("ShowMoreOptionsButton", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertMediaDialogSettingsBaseShowStyleSettingsSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowStyleSettingsSection {
			get { return GetBoolProperty("ShowStyleSettingsSection", true); }
			set { SetBoolProperty("ShowStyleSettingsSection", true, value); }
		}
		protected internal bool ShowSaveFileToServerButtonInternal {
			get { return GetBoolProperty("ShowSaveFileToServerButtonInternal", true); }
			set { SetBoolProperty("ShowSaveFileToServerButtonInternal", true, value); }
		}
		protected internal bool ShowFileUploadSectionInternal {
			get { return GetBoolProperty("ShowFileUploadSectionInternal", true); }
			set { SetBoolProperty("ShowFileUploadSectionInternal", true, value); }
		}
		protected internal bool ShowInsertFromWebSectionInternal {
			get { return GetBoolProperty("ShowInsertFromWebSectionInternal", true); }
			set { SetBoolProperty("ShowInsertFromWebSectionInternal", true, value); }
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public InsertMediaCssClassItems CssClassItems {
			get {
				if(this.cssClassItems == null)
					this.cssClassItems = new InsertMediaCssClassItems(HtmlEditor);
				return this.cssClassItems;
			}
		}
		protected internal ASPxHtmlEditorUploadSettingsBase SettingsUploadInternal {
			get {
				if(uploadSettingsInternal == null)
					uploadSettingsInternal = CreateUploadSettings();
				return uploadSettingsInternal;
			}
		}
		protected internal HtmlEditorSelectorSettings SettingsSelectorInternal {
			get {
				if(selectorSettingsInternal == null)
					selectorSettingsInternal = CreateSelectorSettings();
				return selectorSettingsInternal;
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorInsertMediaDialogSettingsBase;
				if(settings != null) {
					ShowMoreOptionsButton = settings.ShowMoreOptionsButton;
					ShowStyleSettingsSection = settings.ShowStyleSettingsSection;
					MoreOptionsSectionTemplate = settings.MoreOptionsSectionTemplate;
					ShowSaveFileToServerButtonInternal = settings.ShowSaveFileToServerButtonInternal;
					ShowFileUploadSectionInternal = settings.ShowFileUploadSectionInternal;
					ShowInsertFromWebSectionInternal = settings.ShowInsertFromWebSectionInternal;
					CssClassItems.Assign(settings.CssClassItems);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				CssClassItems
			});
		}
		protected virtual ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return null;
		}
		protected virtual HtmlEditorSelectorSettings CreateSelectorSettings() {
			return null;
		}
	}
	public class HtmlEditorInsertMediaDialogSettings : HtmlEditorInsertMediaDialogSettingsBase {
		public HtmlEditorInsertMediaDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertMediaDialogSettingsShowSaveFileToServerButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowSaveFileToServerButton {
			get { return ShowSaveFileToServerButtonInternal; }
			set { ShowSaveFileToServerButtonInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertMediaDialogSettingsShowFileUploadSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFileUploadSection {
			get { return ShowFileUploadSectionInternal; }
			set { ShowFileUploadSectionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertMediaDialogSettingsShowInsertFromWebSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowInsertFromWebSection {
			get { return ShowInsertFromWebSectionInternal; }
			set { ShowInsertFromWebSectionInternal = value; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorInsertMediaDialogSettings;
				if(settings != null) {
					ShowSaveFileToServerButton = settings.ShowSaveFileToServerButton;
					ShowFileUploadSection = settings.ShowFileUploadSection;
					SettingsUploadInternal.Assign(settings.SettingsUploadInternal);
					SettingsSelectorInternal.Assign(settings.SettingsSelectorInternal);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				SettingsUploadInternal, SettingsSelectorInternal
			});
		}
	}
	public class HtmlEditorInsertFlashDialogSettings : HtmlEditorInsertMediaDialogSettings {
		public HtmlEditorInsertFlashDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFlashSelectorSettings SettingsFlashSelector { 
			get { 
				return (HtmlEditorFlashSelectorSettings)SettingsSelectorInternal; 
			} 
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorFlashUploadSettings SettingsFlashUpload {
			get {
				return (ASPxHtmlEditorFlashUploadSettings)SettingsUploadInternal;
			}
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new ASPxHtmlEditorFlashUploadSettings(Owner);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new HtmlEditorFlashSelectorSettings(Owner);
		}
	}
	public class HtmlEditorInsertVideoDialogSettings : HtmlEditorInsertMediaDialogSettings {
		public HtmlEditorInsertVideoDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorVideoSelectorSettings SettingsVideoSelector {
			get {
				return (HtmlEditorVideoSelectorSettings)SettingsSelectorInternal;
			}
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorVideoUploadSettings SettingsVideoUpload {
			get {
				return (ASPxHtmlEditorVideoUploadSettings)SettingsUploadInternal;
			}
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new ASPxHtmlEditorVideoUploadSettings(Owner);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new HtmlEditorVideoSelectorSettings(Owner);
		}
	}
	public class HtmlEditorInsertAudioDialogSettings : HtmlEditorInsertMediaDialogSettings {
		public HtmlEditorInsertAudioDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorAudioSelectorSettings SettingsAudioSelector {
			get {
				return (HtmlEditorAudioSelectorSettings)SettingsSelectorInternal;
			}
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorAudioUploadSettings SettingsAudioUpload {
			get {
				return (ASPxHtmlEditorAudioUploadSettings)SettingsUploadInternal;
			}
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new ASPxHtmlEditorAudioUploadSettings(Owner);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new HtmlEditorAudioSelectorSettings(Owner);
		}
	}
	public class HtmlEditorInsertImageDialogSettings : HtmlEditorInsertMediaDialogSettings {
		InsertImageCssClassItems cssClassItems;
		public HtmlEditorInsertImageDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertImageDialogSettingsShowMoreOptionsButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowMoreOptionsButton {
			get { return base.ShowMoreOptionsButton; }
			set { base.ShowMoreOptionsButton = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertImageDialogSettingsShowSaveFileToServerButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowSaveFileToServerButton {
			get { return base.ShowSaveFileToServerButton; }
			set { base.ShowSaveFileToServerButton = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertImageDialogSettingsShowFileUploadSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowFileUploadSection {
			get { return base.ShowFileUploadSection; }
			set { base.ShowFileUploadSection = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertImageDialogSettingsShowImageStyleSettingsSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true),
		Obsolete("Use the ShowStyleSettingsSection property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowImageStyleSettingsSection {
			get { return ShowStyleSettingsSection; }
			set { ShowStyleSettingsSection = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ITemplate MoreOptionsSectionTemplate {
			get { return base.MoreOptionsSectionTemplate; }
			set { base.MoreOptionsSectionTemplate = value; }
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorImageUploadSettings SettingsImageUpload {
			get {
				return (ASPxHtmlEditorImageUploadSettings)SettingsUploadInternal;
			}
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorImageSelectorSettings SettingsImageSelector {
			get {
				return (HtmlEditorImageSelectorSettings)SettingsSelectorInternal;
			}
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public new InsertImageCssClassItems CssClassItems {
			get {
				if(this.cssClassItems == null)
					this.cssClassItems = new InsertImageCssClassItems(HtmlEditor);
				return this.cssClassItems;
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorInsertImageDialogSettings;
				if(settings != null) {
					CssClassItems.Assign(settings.CssClassItems);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new ASPxHtmlEditorImageUploadSettings(Owner);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new HtmlEditorImageSelectorSettings(Owner);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				CssClassItems
			});
		}
	}
	public class HtmlEditorInsertLinkDialogSettings: HtmlEditorDialogSettingsBase {
		public HtmlEditorInsertLinkDialogSettings(IPropertiesOwner owner)
			: base(owner) {
				SettingsDocumentSelector = CreateSettingsDocumentSelector();
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertLinkDialogSettingsShowOpenInNewWindowButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowOpenInNewWindowButton
		{
			get { return GetBoolProperty("ShowOpenInNewWindowButton", true); }
			set { SetBoolProperty("ShowOpenInNewWindowButton", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertLinkDialogSettingsShowEmailAddressSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowEmailAddressSection
		{
			get { return GetBoolProperty("ShowEmailAddressSection", true); }
			set { SetBoolProperty("ShowEmailAddressSection", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertLinkDialogSettingsShowDisplayPropertiesSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowDisplayPropertiesSection
		{
			get { return GetBoolProperty("ShowDisplayPropertiesSection", true); }
			set { SetBoolProperty("ShowDisplayPropertiesSection", true, value); }
		}
		[NotifyParentProperty(true), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDocumentSelectorSettings SettingsDocumentSelector { get; private set; }
		protected virtual HtmlEditorDocumentSelectorSettings CreateSettingsDocumentSelector() {
			return new HtmlEditorDocumentSelectorSettings(Owner);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorInsertLinkDialogSettings;
				if(settings != null) {
					ShowOpenInNewWindowButton = settings.ShowOpenInNewWindowButton;
					ShowEmailAddressSection = settings.ShowEmailAddressSection;
					ShowDisplayPropertiesSection = settings.ShowDisplayPropertiesSection;
					SettingsDocumentSelector.Assign(settings.SettingsDocumentSelector);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				SettingsDocumentSelector
			});
		}
	}
	public class HtmlEditorInsertTableDialogSettings: HtmlEditorDialogSettingsBase {
		public HtmlEditorInsertTableDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertTableDialogSettingsShowLayoutSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowLayoutSection {
			get { return GetBoolProperty("ShowLayoutSettings", true); }
			set { SetBoolProperty("ShowLayoutSettings", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertTableDialogSettingsShowAppearanceSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowAppearanceSection {
			get { return GetBoolProperty("ShowAppearanceSections", true); }
			set { SetBoolProperty("ShowAppearanceSections", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorInsertTableDialogSettingsShowAccessibilitySection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowAccessibilitySection {
			get { return GetBoolProperty("ShowAccessibilitySection", true); }
			set { SetBoolProperty("ShowAccessibilitySection", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorInsertTableDialogSettings;
				if(settings != null) {
					ShowLayoutSection = settings.ShowLayoutSection;
					ShowAppearanceSection = settings.ShowAppearanceSection;
					ShowAccessibilitySection = settings.ShowAccessibilitySection;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class HtmlEditorPasteFromWordDialogSettings: HtmlEditorDialogSettingsBase {
		public HtmlEditorPasteFromWordDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorPasteFromWordDialogSettingsShowStripFontFamilySelector"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowStripFontFamilySelector {
			get { return GetBoolProperty("ShowStripFontFamilySelector", true); }
			set { SetBoolProperty("ShowStripFontFamilySelector", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as HtmlEditorPasteFromWordDialogSettings;
				if(settings != null)
					ShowStripFontFamilySelector = settings.ShowStripFontFamilySelector;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class HtmlEditorTableCellPropertiesDialogSettings: HtmlEditorTableElementPropertiestDialogSettings {
		public HtmlEditorTableCellPropertiesDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTableCellPropertiesDialogSettingsShowApplyToAllCellsButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowApplyToAllCellsButton
		{
			get { return GetBoolProperty("ShowApplyToAllCellsButton", true); }
			set { SetBoolProperty("ShowApplyToAllCellsButton", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as HtmlEditorTableCellPropertiesDialogSettings;
			if(settings != null)
				ShowApplyToAllCellsButton = settings.ShowApplyToAllCellsButton;
		}
	}
	public class HtmlEditorTableElementPropertiestDialogSettings: ASPxHtmlEditorSettingsBase {
		public HtmlEditorTableElementPropertiestDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTableElementPropertiestDialogSettingsShowAlignmentSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowAlignmentSection {
			get { return GetBoolProperty("ShowAlignmentSection", true); }
			set { SetBoolProperty("ShowAlignmentSection", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorTableElementPropertiestDialogSettingsShowAppearanceSection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowAppearanceSection {
			get { return GetBoolProperty("ShowAppearanceSection", true); }
			set { SetBoolProperty("ShowAppearanceSection", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as HtmlEditorTableElementPropertiestDialogSettings;
			if(settings != null) {
				ShowAlignmentSection = settings.ShowAlignmentSection;
				ShowAppearanceSection = settings.ShowAppearanceSection;
			}
		}
	}
	public class HtmlEditorChangeElementPropertiestDialogSettings : HtmlEditorDialogSettingsBase {
		DialogCssClassItems cssClassItems;
		public HtmlEditorChangeElementPropertiestDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public DialogCssClassItems CssClassItems {
			get {
				if(this.cssClassItems == null)
					this.cssClassItems = new DialogCssClassItems(HtmlEditor);
				return this.cssClassItems;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as HtmlEditorChangeElementPropertiestDialogSettings;
			if(settings != null)
				CssClassItems.Assign(settings.CssClassItems);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				CssClassItems
			});
		}
	}
	public class HtmlEditorDialogSettingsBase: ASPxHtmlEditorSettingsBase {
		ITemplate topAreaTemplate;
		ITemplate bottomAreaTemplate;
		public HtmlEditorDialogSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate TopAreaTemplate {
			get { return topAreaTemplate; }
			set {
				if(TopAreaTemplate == value)
					return;
				topAreaTemplate = value;
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate BottomAreaTemplate {
			get { return bottomAreaTemplate; }
			set {
				if(bottomAreaTemplate == value)
					return;
				bottomAreaTemplate = value;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as HtmlEditorDialogSettingsBase;
			if(settings != null) {
				TopAreaTemplate = settings.TopAreaTemplate;
				BottomAreaTemplate = settings.BottomAreaTemplate;
			}
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	using DevExpress.Utils;
	using DevExpress.Web.ASPxHtmlEditor.Forms;
	using DevExpress.Web.ASPxHtmlEditor.Localization;
	[ToolboxItem(false)]
	public class ASPxHtmlEditorMediaFileSelector : MediaFileSelector {
		public ASPxHtmlEditorMediaFileSelector()
			: base() {
		}
		protected override ASPxFileManager CreateFileManager() {
			return new HtmlEditorFileManager();
		}
		protected override ASPxUploadControl CreateUploadControl() {
			return new ASPxHtmlEditorUploadControl();
		}
	}
	[ToolboxItem(false)]
	sealed class HtmlEditorSystemPopup : DevExpress.Web.ASPxPopupControl {
		class Template : ITemplate {
			readonly Control control;
			public Template(Control control) {
				this.control = control;
			}
			public void InstantiateIn(Control container) {
				container.Controls.Add(this.control);
			}
		}
		const string AdvancedSearchWindowName = "advancedSearch";
		const string QuickSearchWindowName = "quickSearch";
		const string IntelliSenseWindowName = "intelliSense";
		readonly ASPxHtmlEditor HtmlEditor;
		PopupWindow quickSearchWindow = null;
		PopupWindow advancedSearchWindow = null;
		PopupWindow intelliSenseWindow = null;
		QuickSearchPanelControl quickSearchControl = null;
		AdvancedSearchPanelControl advancedSearchControl = null;
		AdvancedSearchButtons advancedSearchButtons = null;
		ASPxListBox intelliSenseListBox = null;
		public HtmlEditorSystemPopup(ASPxHtmlEditor htmlEditor)
			: base(null) {
			HtmlEditor = htmlEditor;
			AllowDragging = true;
			ParentSkinOwner = htmlEditor;
			EnableViewState = false;
			EnableClientSideAPI = true;
			PopupAction = PopupAction.None;
			PopupAlignCorrection = PopupAlignCorrection.Auto;
			PopupHorizontalAlign = PopupHorizontalAlign.RightSides;
			PopupVerticalAlign = PopupVerticalAlign.TopSides;
			CloseOnEscape = true;
			Collapsed = true;
			AutoUpdatePosition = true;
			CloseAction = CloseAction.CloseButton;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxHtmlEditor), ASPxHtmlEditor.HtmlEditorFindManagerResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorClasses.Controls.SystemPopupControl";
		}
		protected override bool LoadWindowsState(string state) { 
			return false;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateQuickSearchWindow();
			CreateAdvancedSearchWindow();
			if(HtmlEditor.Settings.AllowHtmlView && HtmlEditor.IsAdvancedHtmlEditingMode())
				CreateIntelliSenseWindow();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, "dxhe-searchPopup");
			if(this.intelliSenseListBox != null) {
				this.intelliSenseListBox.Width = Unit.Percentage(100);
				this.intelliSenseListBox.Height = Unit.Pixel(170);
			}
			this.ParentStyles = HtmlEditor.StylesDialogForm;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.quickSearchControl = null;
			this.advancedSearchControl = null;
			this.advancedSearchButtons = null;
			this.intelliSenseListBox = null;
		}
		void CreateAdvancedSearchWindow() {
			this.HeaderText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Header);
			this.advancedSearchWindow = new PopupWindow();
			this.advancedSearchWindow.Name = AdvancedSearchWindowName;
			this.advancedSearchControl = new AdvancedSearchPanelControl(HtmlEditor, AdvancedSearchWindowName);
			AddControlToWindow(this.advancedSearchWindow, this.advancedSearchControl);
			this.advancedSearchWindow.PopupElementID = HtmlEditor.RenderHelper.EditAreaCellID;
			this.advancedSearchButtons = new AdvancedSearchButtons(HtmlEditor, AdvancedSearchWindowName);
			this.advancedSearchWindow.FooterTemplate = new Template(this.advancedSearchButtons);
			this.advancedSearchWindow.ShowFooter = DefaultBoolean.True;
			Windows.Add(this.advancedSearchWindow);
		}
		void CreateQuickSearchWindow() {
			this.quickSearchWindow = new PopupWindow();
			this.quickSearchWindow.Name = QuickSearchWindowName;
			this.quickSearchControl = new QuickSearchPanelControl(HtmlEditor, QuickSearchWindowName);
			this.quickSearchWindow.ShowCollapseButton = DefaultBoolean.True;
			this.quickSearchWindow.PopupElementID = HtmlEditor.RenderHelper.EditAreaCellID;
			this.quickSearchWindow.HeaderContentTemplate = new Template(this.quickSearchControl);
			this.quickSearchWindow.ContentStyle.CssClass = "dxhe-hidden";
			this.quickSearchWindow.Collapsed = true;
			Windows.Add(this.quickSearchWindow);
		}
		void CreateIntelliSenseWindow() {
			this.intelliSenseWindow = new PopupWindow();
			this.intelliSenseWindow.Name = IntelliSenseWindowName;
			this.intelliSenseWindow.CloseAction = WindowCloseAction.OuterMouseClick;
			this.intelliSenseListBox = CreateListBox();
			AddControlToWindow(this.intelliSenseWindow, this.intelliSenseListBox);
			this.intelliSenseWindow.ShowHeader = DefaultBoolean.False;
			Windows.Add(this.intelliSenseWindow);
		}
		ASPxListBox CreateListBox() {
			ASPxListBox listBox = new ASPxListBox();
			listBox.ParentSkinOwner = HtmlEditor;
			listBox.ID = HtmlEditor.RenderHelper.IntelliSenseListBoxId;
			listBox.EncodeHtml = true;
			listBox.ClientInstanceName = HtmlEditor.ClientID + "_" + IntelliSenseWindowName + "_" + HtmlEditor.RenderHelper.IntelliSenseListBoxId;
			listBox.EnableClientSideAPI = true;
			listBox.EnableSynchronization = DefaultBoolean.False;
			listBox.ItemImage.Width = Unit.Pixel(16);
			listBox.ItemImage.Height = Unit.Pixel(16);
			return listBox;
		}
		void AddControlToWindow(PopupWindow win, Control control) {
			if(win.ContentCollection.Count == 0)
				win.ContentCollection.Add(new PopupControlContentControl());
			win.ContentCollection[0].Controls.Add(control);
		}
	}
	[ToolboxItem(false)]
	public sealed class HtmlEditorPopupForm : DevExpress.Web.ASPxPopupControl {
		public HtmlEditorPopupForm(ISkinOwner skinOwner)
			: base(null) {
			AllowDragging = true;
			EnableClientSideAPI = true;
			PopupAnimationType = AnimationType.Fade;
			Modal = true;
			CloseAction = CloseAction.CloseButton;
			CloseOnEscape = true;
			ParentSkinOwner = skinOwner;
			PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
		}
		protected override bool HideBodyScrollWhenModal() {
			return !Browser.Family.IsNetscape;
		}
		protected override bool LoadWindowsState(string state) {
			return false;
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorDialogFormStylesLite(this);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, "dxhe-unselectablePopup");
		}
	}
	[ToolboxItem(false)]
	public class CustomDialogsContainer : ASPxPanel {
		const string OkButtonID = "Ok";
		const string CancelButtonID = "Cancel";
		public CustomDialogsContainer(HtmlEditorCustomDialog customDialog, Control customDialogContent)
			: base() {
			CustomDialog = customDialog;
			CustomDialogContent = customDialogContent;
		}
		HtmlEditorCustomDialog CustomDialog { get; set; }
		ASPxHtmlEditor HtmlEditor { get { return CustomDialog.Collection.Owner; } }
		Control CustomDialogContent { get; set; }
		WebControl ContentContainer { get; set; }
		WebControl ButtonsContainer { get; set; }
		TableCell OkButtonCell { get; set; }
		ASPxButton OkButton { get; set; }
		TableCell CancelButtonCell { get; set; }
		ASPxButton CancelButton { get; set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ContentContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(ContentContainer);
			ContentContainer.Controls.Add(CustomDialogContent);
			ButtonsContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(ButtonsContainer);
			Table buttonsTable = RenderUtils.CreateTable();
			ButtonsContainer.Controls.Add(buttonsTable);
			RenderUtils.SetStyleStringAttribute(buttonsTable, "display", "inline-block");
			buttonsTable.Rows.Add(RenderUtils.CreateTableRow());
			OkButtonCell = RenderUtils.CreateTableCell();
			buttonsTable.Rows[0].Cells.Add(OkButtonCell);
			CancelButtonCell = RenderUtils.CreateTableCell();
			buttonsTable.Rows[0].Cells.Add(CancelButtonCell);
			OkButton = CreateDialogButton(
				OkButtonID,
				"ASPxClientHtmlEditor.OnCustomDialogClosing"
			);
			OkButtonCell.Controls.Add(OkButton);
			CancelButton = CreateDialogButton(
				CancelButtonID,
				"ASPxClientHtmlEditor.OnCustomDialogClosing"
			);
			CancelButtonCell.Controls.Add(CancelButton);
		}
		protected ASPxButton CreateDialogButton(string id, string onclick) {
			ASPxButton button = new ASPxButton();
			button.ID = id;
			button.AutoPostBack = false;
			button.CausesValidation = false;
			button.ClientSideEvents.Click = onclick;
			button.Width = Unit.Pixel(74);
			ClientIDHelper.EnableClientIDGeneration(button);
			return button;
		}
		protected override void PrepareControlHierarchy() {
			HtmlEditor.GetCustomDialogStyle(CustomDialog.Name).AssignToControl(this);
			ContentContainer.CssClass = HtmlEditorStyles.CustomDialogContentCssClass;
			ButtonsContainer.CssClass = HtmlEditorStyles.CustomDialogButtonsCssClass;
			OkButton.CssClass = HtmlEditorStyles.CustomDialogOkButtonCssClass;
			CancelButton.CssClass = HtmlEditorStyles.CustomDialogCancelButtonCssClass;
			if(!string.IsNullOrEmpty(CustomDialog.DefaultButtonID))
				DefaultButton = CustomDialog.DefaultButtonID;
			else if(CustomDialog.OkButtonVisible)
				DefaultButton = OkButton.ID;
			else if(CustomDialog.CancelButtonVisible)
				DefaultButton = CancelButton.ID;
			OkButtonCell.Visible = CustomDialog.OkButtonVisible;
			CancelButtonCell.Visible = CustomDialog.CancelButtonVisible;
			ButtonsContainer.Visible = CustomDialog.OkButtonVisible || CustomDialog.CancelButtonVisible;
			OkButton.Text = CustomDialog.OkButtonText;
			CancelButton.Text = CustomDialog.CancelButtonText;
			base.PrepareControlHierarchy();
		}
	}
}
