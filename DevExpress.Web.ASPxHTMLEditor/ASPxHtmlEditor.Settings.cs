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
using System.Web.UI;
namespace DevExpress.Web.ASPxHtmlEditor {
	public partial class ASPxHtmlEditor {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettings"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorSettings Settings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxHtmlEditorLoadingPanelSettings SettingsLoadingPanel {
			get { return (ASPxHtmlEditorLoadingPanelSettings)base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsHtmlEditing"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorHtmlEditingSettings SettingsHtmlEditing { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsText"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorTextSettings SettingsText { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsSpellChecker"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorSpellCheckerSettings SettingsSpellChecker { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsResize"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorResizeSettings SettingsResize { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsValidation"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorValidationSettings SettingsValidation { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsDialogs"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), Category("Settings"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public HtmlEditorDefaultDialogSettings SettingsDialogs { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsForms"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public HtmlEditorFormsSettings SettingsForms { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsDialogFormElements"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs property instead.")]
		public HtmlEditorDialogFormElementSettings SettingsDialogFormElements { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsImageSelector"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector property instead.")]
		public HtmlEditorImageSelectorSettings SettingsImageSelector {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsFlashSelector"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog.SettingsFlashSelector property instead.")]
		public HtmlEditorFlashSelectorSettings SettingsFlashSelector {
			get { return SettingsDialogs.InsertFlashDialog.SettingsFlashSelector; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsVideoSelector"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog.SettingsVideoSelector property instead.")]
		public HtmlEditorVideoSelectorSettings SettingsVideoSelector {
			get { return SettingsDialogs.InsertVideoDialog.SettingsVideoSelector; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAudioSelector"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog.SettingsAudioSelector property instead.")]
		public HtmlEditorAudioSelectorSettings SettingsAudioSelector {
			get { return SettingsDialogs.InsertAudioDialog.SettingsAudioSelector; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsImageUpload"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog.SettingsImageUpload property instead.")]
		public ASPxHtmlEditorImageUploadSettings SettingsImageUpload {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageUpload; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsFlashUpload"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog.SettingsFlashUpload property instead.")]
		public ASPxHtmlEditorFlashUploadSettings SettingsFlashUpload {
			get { return SettingsDialogs.InsertFlashDialog.SettingsFlashUpload; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsVideoUpload"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog.SettingsVideoUpload property instead.")]
		public ASPxHtmlEditorVideoUploadSettings SettingsVideoUpload {
			get { return SettingsDialogs.InsertVideoDialog.SettingsVideoUpload; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAudioUpload"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SettingsDialogs.InsertAudioDialog.SettingsAudioUpload property instead.")]
		public ASPxHtmlEditorAudioUploadSettings SettingsAudioUpload {
			get { return SettingsDialogs.InsertAudioDialog.SettingsAudioUpload; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsDocumentSelector"),
#endif
		Category("Settings"), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector property instead.")]
		public HtmlEditorDocumentSelectorSettings SettingsDocumentSelector {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector; }
		}
		protected void InitializeSettings() {
			Settings = new ASPxHtmlEditorSettings(this);
			SettingsHtmlEditing = new ASPxHtmlEditorHtmlEditingSettings(this);
			SettingsText = new ASPxHtmlEditorTextSettings(this);
			SettingsSpellChecker = new ASPxHtmlEditorSpellCheckerSettings(this);
			SettingsResize = new HtmlEditorResizeSettings(this);
			SettingsDialogs = CreateSettingsDialogs();
#pragma warning disable 618
			SettingsForms = CreateSettingsForms();
			SettingsDialogFormElements = CreateSettingsDialogFormElement();
#pragma warning restore 618
		}
		protected virtual HtmlEditorDefaultDialogSettings CreateSettingsDialogs() {
			return new HtmlEditorDefaultDialogSettings(this);
		}
		protected virtual HtmlEditorFormsSettings CreateSettingsForms() {
			return new HtmlEditorFormsSettings(this);
		}
		protected virtual HtmlEditorDialogFormElementSettings CreateSettingsDialogFormElement() {
			return new HtmlEditorDialogFormElementSettings(this);
		}
	}
}
