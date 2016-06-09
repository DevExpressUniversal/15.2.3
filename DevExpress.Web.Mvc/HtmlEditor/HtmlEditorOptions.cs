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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web.ASPxHtmlEditor;
namespace DevExpress.Web.Mvc {
	public class MVCxHtmlEditorAudioUploadSettings : ASPxHtmlEditorAudioUploadSettings {
		HtmlEditorAudioSelectorSettings associatedSettings;
		public MVCxHtmlEditorAudioUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorAudioUploadSettings(IPropertiesOwner owner, HtmlEditorAudioSelectorSettings associatedSettings)
			: base(owner) {
			this.associatedSettings = associatedSettings;
		}
		public object UploadCallbackRouteValues { get; set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorAudioUploadSettings src = source as MVCxHtmlEditorAudioUploadSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorSelectorSettings GetSelectorSettings() {
			return this.associatedSettings ?? base.GetSelectorSettings();
		}
	}
	public class MVCxHtmlEditorFlashUploadSettings : ASPxHtmlEditorFlashUploadSettings {
		HtmlEditorFlashSelectorSettings associatedSettings;
		public MVCxHtmlEditorFlashUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorFlashUploadSettings(IPropertiesOwner owner, HtmlEditorFlashSelectorSettings associatedSettings)
			: base(owner) {
			this.associatedSettings = associatedSettings;
		}
		public object UploadCallbackRouteValues { get; set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorFlashUploadSettings src = source as MVCxHtmlEditorFlashUploadSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorSelectorSettings GetSelectorSettings() {
			return this.associatedSettings ?? base.GetSelectorSettings();
		}
	}
	public class MVCxHtmlEditorVideoUploadSettings : ASPxHtmlEditorVideoUploadSettings {
		HtmlEditorVideoSelectorSettings associatedSettings;
		public MVCxHtmlEditorVideoUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorVideoUploadSettings(IPropertiesOwner owner, HtmlEditorVideoSelectorSettings associatedSettings)
			: base(owner) {
			this.associatedSettings = associatedSettings;
		}
		public object UploadCallbackRouteValues { get; set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorVideoUploadSettings src = source as MVCxHtmlEditorVideoUploadSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorSelectorSettings GetSelectorSettings() {
			return this.associatedSettings ?? base.GetSelectorSettings();
		}
	}
	public class MVCxHtmlEditorImageUploadSettings : ASPxHtmlEditorImageUploadSettings {
		HtmlEditorImageSelectorSettings associatedSettings;
		public MVCxHtmlEditorImageUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorImageUploadSettings(IPropertiesOwner owner, HtmlEditorImageSelectorSettings associatedSettings)
			: base(owner) {
			this.associatedSettings = associatedSettings;
		}
		public object UploadCallbackRouteValues { get; set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorImageUploadSettings src = source as MVCxHtmlEditorImageUploadSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorSelectorSettings GetSelectorSettings() {
			return this.associatedSettings ?? base.GetSelectorSettings();
		}
	}
	public class MVCxHtmlEditorFormsSettings : HtmlEditorFormsSettings {
		MVCxHtmlEditorDefaultDialogSettings associatedSettings;
		public MVCxHtmlEditorFormsSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorFormsSettings(MVCxHtmlEditorDefaultDialogSettings associatedSettings)
			: base(null) {
			this.associatedSettings = associatedSettings;
		}
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertAudioFormPath { get { return base.InsertAudioFormPath; } set { base.InsertAudioFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertFlashFormPath { get { return base.InsertFlashFormPath; } set { base.InsertFlashFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertImageFormPath { get { return base.InsertImageFormPath; } set { base.InsertImageFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertLinkFormPath { get { return base.InsertLinkFormPath; } set { base.InsertLinkFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertTableFormPath { get { return base.InsertTableFormPath; } set { base.InsertTableFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertVideoFormPath { get { return base.InsertVideoFormPath; } set { base.InsertVideoFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string InsertYouTubeVideoFormPath { get { return base.InsertYouTubeVideoFormPath; } set { base.InsertYouTubeVideoFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string PasteFromWordFormPath { get { return base.PasteFromWordFormPath; } set { base.PasteFromWordFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string TableColumnPropertiesFormPath { get { return base.TableColumnPropertiesFormPath; } set { base.TableColumnPropertiesFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxSpellCheckerFormsSettings SpellCheckerForms {
			get { return (MVCxSpellCheckerFormsSettings)base.SpellCheckerForms; }
		}
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertAudioFormAction { get { return base.InsertAudioFormPath; } set { base.InsertAudioFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertFlashFormAction { get { return base.InsertFlashFormPath; } set { base.InsertFlashFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertImageFormAction { get { return base.InsertImageFormPath; } set { base.InsertImageFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertLinkFormAction { get { return base.InsertLinkFormPath; } set { base.InsertLinkFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertTableFormAction { get { return base.InsertTableFormPath; } set { base.InsertTableFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertVideoFormAction { get { return base.InsertVideoFormPath; } set { base.InsertVideoFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string InsertYouTubeVideoFormAction { get { return base.InsertYouTubeVideoFormPath; } set { base.InsertYouTubeVideoFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string PasteFromWordFormAction { get { return base.PasteFromWordFormPath; } set { base.PasteFromWordFormPath = value; } }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string TableColumnPropertiesFormAction { get { return base.TableColumnPropertiesFormPath; } set { base.TableColumnPropertiesFormPath = value; } }
		protected internal string GetFormAction(string formName) {
			return GetFormPath(formName);
		}
		protected override SpellCheckerFormsSettings CreateSpellCheckerFormsSettings() {
			return new MVCxSpellCheckerFormsSettings(this);
		}
		protected override HtmlEditorDefaultDialogSettings GetDefaultDialogsSettings() {
			return this.associatedSettings ?? base.GetDefaultDialogsSettings();
		}
	}
	public class MVCxHtmlEditorAudioSelectorSettings : HtmlEditorAudioSelectorSettings {
		public MVCxHtmlEditorAudioSelectorSettings() : this(null) { }
		public MVCxHtmlEditorAudioSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		public object UploadCallbackRouteValues { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public FileSavingEventHandler AudioFileSaving { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public new MVCxHtmlEditorFileManagerUploadSettings UploadSettings {
			get { return (MVCxHtmlEditorFileManagerUploadSettings)base.UploadSettings; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorAudioSelectorSettings src = source as MVCxHtmlEditorAudioSelectorSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
					FolderCreating = src.FolderCreating;
					ItemRenaming = src.ItemRenaming;
					ItemCopying = src.ItemCopying;
					ItemDeleting = src.ItemDeleting;
					ItemMoving = src.ItemMoving;
					FileUploading = src.FileUploading;
					CustomThumbnail = src.CustomThumbnail;
					UploadSettings.ValidationSettings.AllowedFileExtensions = src.CommonSettings.AllowedFileExtensions;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new MVCxHtmlEditorAudioSelectorUploadSettings(Owner);
		}
	}
	public class MVCxHtmlEditorFlashSelectorSettings : HtmlEditorFlashSelectorSettings {
		public MVCxHtmlEditorFlashSelectorSettings() : this(null) { }
		public MVCxHtmlEditorFlashSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		public object UploadCallbackRouteValues { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public FileSavingEventHandler FlashFileSaving { get; set; }
		public new MVCxHtmlEditorFlashSelectorUploadSettings UploadSettings {
			get { return (MVCxHtmlEditorFlashSelectorUploadSettings)base.UploadSettings; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorFlashSelectorSettings src = source as MVCxHtmlEditorFlashSelectorSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
					FolderCreating = src.FolderCreating;
					ItemRenaming = src.ItemRenaming;
					ItemCopying = src.ItemCopying;
					ItemDeleting = src.ItemDeleting;
					ItemMoving = src.ItemMoving;
					CustomThumbnail = src.CustomThumbnail;
					FileUploading = src.FileUploading;
					FlashFileSaving = src.FlashFileSaving;
					UploadSettings.ValidationSettings.AllowedFileExtensions = src.CommonSettings.AllowedFileExtensions;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new MVCxHtmlEditorFlashSelectorUploadSettings(Owner);
		}
	}
	public class MVCxHtmlEditorVideoSelectorSettings : HtmlEditorVideoSelectorSettings {
		public MVCxHtmlEditorVideoSelectorSettings() : this(null) { }
		public MVCxHtmlEditorVideoSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		public object UploadCallbackRouteValues { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public FileSavingEventHandler VideoFileSaving { get; set; }
		public new MVCxHtmlEditorVideoSelectorUploadSettings UploadSettings {
			get { return (MVCxHtmlEditorVideoSelectorUploadSettings)base.UploadSettings; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorVideoSelectorSettings src = source as MVCxHtmlEditorVideoSelectorSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
					FolderCreating = src.FolderCreating;
					ItemRenaming = src.ItemRenaming;
					ItemCopying = src.ItemCopying;
					ItemDeleting = src.ItemDeleting;
					ItemMoving = src.ItemMoving;
					CustomThumbnail = src.CustomThumbnail;
					FileUploading = src.FileUploading;
					VideoFileSaving = src.VideoFileSaving;
					UploadSettings.ValidationSettings.AllowedFileExtensions = src.CommonSettings.AllowedFileExtensions;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new MVCxHtmlEditorVideoSelectorUploadSettings(Owner);
		}
	}
	public class MVCxHtmlEditorImageSelectorSettings : HtmlEditorImageSelectorSettings {
		public MVCxHtmlEditorImageSelectorSettings() : this(null) { }
		public MVCxHtmlEditorImageSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		public object UploadCallbackRouteValues { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public FileSavingEventHandler ImageFileSaving { get; set; }
		public new MVCxHtmlEditorImageSelectorUploadSettings UploadSettings {
			get { return (MVCxHtmlEditorImageSelectorUploadSettings)base.UploadSettings; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				MVCxHtmlEditorImageSelectorSettings src = source as MVCxHtmlEditorImageSelectorSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
					FolderCreating = src.FolderCreating;
					ItemRenaming = src.ItemRenaming;
					ItemCopying = src.ItemCopying;
					ItemDeleting = src.ItemDeleting;
					ItemMoving = src.ItemMoving;
					CustomThumbnail = src.CustomThumbnail;
					FileUploading = src.FileUploading;
					ImageFileSaving = src.ImageFileSaving;
					UploadSettings.ValidationSettings.AllowedFileExtensions = src.CommonSettings.AllowedFileExtensions;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new MVCxHtmlEditorImageSelectorUploadSettings(Owner);
		}
	}
	public class MVCxHtmlEditorDocumentSelectorSettings : HtmlEditorDocumentSelectorSettings {
		public MVCxHtmlEditorDocumentSelectorSettings() : this(null) { }
		public MVCxHtmlEditorDocumentSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		public object UploadCallbackRouteValues { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public new MVCxHtmlEditorDocumentSelectorUploadSettings UploadSettings {
			get { return (MVCxHtmlEditorDocumentSelectorUploadSettings)base.UploadSettings; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as MVCxHtmlEditorDocumentSelectorSettings;
				if(src != null) {
					UploadCallbackRouteValues = src.UploadCallbackRouteValues;
					FolderCreating = src.FolderCreating;
					ItemRenaming = src.ItemRenaming;
					ItemCopying = src.ItemCopying;
					ItemDeleting = src.ItemDeleting;
					ItemMoving = src.ItemMoving;
					CustomThumbnail = src.CustomThumbnail;
					FileUploading = src.FileUploading;
					UploadSettings.ValidationSettings.AllowedFileExtensions = src.CommonSettings.AllowedFileExtensions;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new MVCxHtmlEditorDocumentSelectorUploadSettings(Owner);
		}
	}
	public class MVCxHtmlEditorFileManagerUploadSettings : HtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorFileManagerUploadSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxHtmlEditorFileManagerValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal as MVCxHtmlEditorFileManagerValidationSettings; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new MVCxHtmlEditorFileManagerValidationSettings(Owner);
		}
	}
	public class MVCxHtmlEditorImageSelectorUploadSettings : MVCxHtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorImageSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxHtmlEditorFileManagerValidationSettings ValidationSettings {
			get { return (MVCxHtmlEditorFileManagerValidationSettings)ValidationSettingsInternal; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new MVCxHtmlEditorFileManagerValidationSettings(Owner);
		}
	}
	public class MVCxHtmlEditorAudioSelectorUploadSettings : MVCxHtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorAudioSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxHtmlEditorFileManagerValidationSettings ValidationSettings {
			get { return (MVCxHtmlEditorFileManagerValidationSettings)ValidationSettingsInternal; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new MVCxHtmlEditorFileManagerValidationSettings(Owner);
		}
	}
	public class MVCxHtmlEditorFlashSelectorUploadSettings : MVCxHtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorFlashSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxHtmlEditorFileManagerValidationSettings ValidationSettings {
			get { return (MVCxHtmlEditorFileManagerValidationSettings)ValidationSettingsInternal; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new MVCxHtmlEditorFileManagerValidationSettings(Owner);
		}
	}
	public class MVCxHtmlEditorVideoSelectorUploadSettings : MVCxHtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorVideoSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxHtmlEditorFileManagerValidationSettings ValidationSettings {
			get { return (MVCxHtmlEditorFileManagerValidationSettings)ValidationSettingsInternal; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new MVCxHtmlEditorFileManagerValidationSettings(Owner);
		}
	}
	public class MVCxHtmlEditorDocumentSelectorUploadSettings : MVCxHtmlEditorFileManagerUploadSettings {
		public MVCxHtmlEditorDocumentSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(true)]
		public override bool ShowUploadPanel {
			get { return base.ShowUploadPanel; }
			set { base.ShowUploadPanel = value; }
		}
		protected override bool GetDefaultShowUploadPanel() {
			return true;
		}
	}
	public class MVCxHtmlEditorFileManagerValidationSettings : MVCxFileManagerValidationSettings {
		public MVCxHtmlEditorFileManagerValidationSettings(IPropertiesOwner owner) : base(owner) { }
		public override long MaxFileSize {
			get { return base.MaxFileSize; }
			set { base.MaxFileSize = value; }
		}
		protected override long GetMaxFileSizeDefaultValue() {
			return 30 * 1024 * 1024;
		}
	}
}
