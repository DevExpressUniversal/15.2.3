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

using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxHtmlEditor;
	[ToolboxItem(false)]
	public class MVCxHtmlEditorFileManager : HtmlEditorFileManager {
		public const string ImageSelectorDialogName = "insertimagedialog";
		public const string DocumentSelectorDialogName = "insertlinkdialog";
		public const string InsertAudioDialogName = "insertaudiodialog";
		public const string InsertFlashDialogName = "inserflashdialog";
		public const string InsertVideoDialogName = "inservideodialog";
		public MVCxHtmlEditorFileManager()
			: base() { }
		public override bool IsLoading() {
			return false;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxHtmlEditorFileManager), Utils.FileManagerScriptResourceName);
		}
		protected internal override ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new MVCxHtmlEditorFileManagerUploadControl(CurrentDialogName);
		}
		protected override FileManagerSettingsUpload CreateSettingsUpload() {
			return new MVCxHtmlEditorFileManagerUploadSettings(OwnerControl);
		}
		protected string CurrentDialogName {
			get {
				if(TemplateControl is HtmlEditorInsertMediaDialogBase) {
					var sourceSelectControl = TemplateControl as HtmlEditorInsertMediaDialogBase;
					switch(sourceSelectControl.InsertMediaSourceType) {
						case InsertMediaSourceType.Audio:
							return InsertAudioDialogName;
						case InsertMediaSourceType.Flash:
							return InsertFlashDialogName;
						case InsertMediaSourceType.Video:
							return InsertVideoDialogName;
						case InsertMediaSourceType.Image:
							return ImageSelectorDialogName;
					}
				}
				return DocumentSelectorDialogName;
			}
		}
		internal void UploadFile() {
			(Helper.UploadControl as MVCxUploadControl).EnsureUploaded();
		}
	}
	[ToolboxItem(false)]
	public class MVCxHtmlEditorFileManagerUploadControl : MVCxHtmlEditorUploadControl, IDialogFormElementRequiresLoad {
		protected string DialogName { get; set; }
		protected internal MVCxHtmlEditorFileManagerUploadControl(string dialogName)
			: base() {
			DialogName = dialogName;
		}
		protected override string GetClientObjectClassName() {
			return "MVCx.FileManagerUploadControl";
		}
		protected override object GetCallbackRouteValues(MVCxHtmlEditor htmlEditor) {
			switch(DialogName) {
				case MVCxHtmlEditorFileManager.InsertAudioDialogName:
					return htmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.UploadCallbackRouteValues;
				case MVCxHtmlEditorFileManager.InsertFlashDialogName:
					return htmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.UploadCallbackRouteValues;
				case MVCxHtmlEditorFileManager.InsertVideoDialogName:
					return htmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.UploadCallbackRouteValues;
				case MVCxHtmlEditorFileManager.ImageSelectorDialogName:
					return htmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.UploadCallbackRouteValues;
				default:
					return htmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.UploadCallbackRouteValues;
			}
		}
		protected override UploadControlValidationSettings GetValidationSettings(MVCxHtmlEditor htmlEditor) {
			switch(DialogName) {
				case MVCxHtmlEditorFileManager.InsertAudioDialogName:
					return htmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.UploadSettings.ValidationSettings;
				case MVCxHtmlEditorFileManager.InsertFlashDialogName:
					return htmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.UploadSettings.ValidationSettings;
				case	MVCxHtmlEditorFileManager.InsertVideoDialogName:
					return htmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.UploadSettings.ValidationSettings;
				case MVCxHtmlEditorFileManager.ImageSelectorDialogName:
					return htmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.UploadSettings.ValidationSettings;
				default:
					return htmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.UploadSettings.ValidationSettings;
			}
		}
	}
}
