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
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.UI;
	public class UploadControlExtension : ExtensionBase {
		public UploadControlExtension(UploadControlSettings settings)
			: base(settings) {
		}
		public UploadControlExtension(UploadControlSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxUploadControl Control {
			get { return (MVCxUploadControl)base.Control; }
		}
		protected internal new UploadControlSettings Settings {
			get { return (UploadControlSettings)base.Settings; }
		}
		public static UploadedFile[] GetUploadedFiles(string name) {
			string[] errorTexts;
			return GetUploadedFiles(name, null, out errorTexts, null, null);
		}
		public static UploadedFile[] GetUploadedFiles(string name, UploadControlValidationSettings validationSettings) {
			string[] errorTexts;
			return GetUploadedFiles(name, validationSettings, out errorTexts, null, null);
		}
		public static UploadedFile[] GetUploadedFiles(string name, UploadControlValidationSettings validationSettings, out string[] errorTexts) {
			return GetUploadedFiles(name, validationSettings, out errorTexts, null, null);
		}
		public static UploadedFile[] GetUploadedFiles(string name, UploadControlValidationSettings validationSettings, EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate) {
			string[] errorTexts;
			return GetUploadedFiles(name, validationSettings, out errorTexts, fileUploadCompleteDelegate, null);
		}
		public static UploadedFile[] GetUploadedFiles(string name, UploadControlValidationSettings validationSettings, out string[] errorTexts, EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate) {
			return GetUploadedFiles(name, validationSettings, out errorTexts, fileUploadCompleteDelegate, null);
		}
		public static UploadedFile[] GetUploadedFiles(string name, UploadControlValidationSettings validationSettings, out string[] errorTexts, 
			EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate, EventHandler<FilesUploadCompleteEventArgs> filesUploadCompleteDelegate) {
			UploadControlSettings settings = new UploadControlSettings { Name = name };
			if(validationSettings != null)
				settings.ValidationSettings.Assign(validationSettings);
			return GetUploadedFiles(settings, out errorTexts, fileUploadCompleteDelegate, filesUploadCompleteDelegate);
		}
		public static UploadedFile[] GetUploadedFiles(UploadControlSettings settings) {
			string[] errorTexts;
			return GetUploadedFiles(settings, out errorTexts, null, null);
		}
		public static UploadedFile[] GetUploadedFiles(UploadControlSettings settings, EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate) {
			string[] errorTexts;
			return GetUploadedFiles(settings, out errorTexts, fileUploadCompleteDelegate, null);
		}
		public static UploadedFile[] GetUploadedFiles(UploadControlSettings settings, EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate, 
			EventHandler<FilesUploadCompleteEventArgs> filesUploadCompleteDelegate) {
			string[] errorTexts;
			return GetUploadedFiles(settings, out errorTexts, fileUploadCompleteDelegate, filesUploadCompleteDelegate);
		}
		public static UploadedFile[] GetUploadedFiles(UploadControlSettings settings, out string[] errorTexts,
			EventHandler<FileUploadCompleteEventArgs> fileUploadCompleteDelegate, EventHandler<FilesUploadCompleteEventArgs> filesUploadCompleteDelegate) {
			UploadControlExtension extension = new UploadControlExtension(settings);
			if(fileUploadCompleteDelegate != null)
				extension.Control.FileUploadComplete += fileUploadCompleteDelegate;
			if(filesUploadCompleteDelegate != null)
				extension.Control.FilesUploadComplete += filesUploadCompleteDelegate;
			extension.Control.EnsureUploaded();
			errorTexts = extension.Control.ErrorTexts;
			return extension.Control.UploadedFiles;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AddButton.Assign(Settings.AddButton);
			Control.AddUploadButtonsHorizontalPosition = Settings.AddUploadButtonsHorizontalPosition;
			Control.AddUploadButtonsSpacing = Settings.AddUploadButtonsSpacing;
			Control.ButtonSpacing = Settings.ButtonSpacing;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CancelButton.Assign(Settings.CancelButton);
			Control.ClearFileSelectionImage.Assign(Settings.ClearFileSelectionImage);
			Control.CancelButtonHorizontalPosition = Settings.CancelButtonHorizontalPosition;
			Control.CancelButtonSpacing = Settings.CancelButtonSpacing;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.FileInputSpacing = Settings.FileInputSpacing;
			Control.ShowAddRemoveButtons = Settings.ShowAddRemoveButtons;
			Control.UploadMode = Settings.UploadMode;
			Control.UploadStorage = Settings.UploadStorage;
			Control.AdvancedModeSettings.Assign(Settings.AdvancedModeSettings);
			Control.FileInputCount = Settings.FileInputCount;
			Control.ProgressBarSettings.Assign(Settings.ProgressBarSettings);
			Control.RemoveButton.Assign(Settings.RemoveButton);
			Control.RemoveButtonSpacing = Settings.RemoveButtonSpacing;
			Control.ShowClearFileSelectionButton = Settings.ShowClearFileSelectionButton;
			Control.ShowProgressPanel = Settings.ShowProgressPanel;
			Control.ShowUploadButton = Settings.ShowUploadButton;
			Control.ShowTextBox = Settings.ShowTextBox;
			Control.Size = Settings.Size;
			Control.NullText = Settings.NullText;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.UploadButton.Assign(Settings.UploadButton);
			Control.ValidationSettings.Assign(Settings.ValidationSettings);
			Control.BrowseButton.Assign(Settings.BrowseButton);
			Control.AzureSettings.Assign(Settings.AzureSettings);
			Control.AmazonSettings.Assign(Settings.AmazonSettings);
			Control.DropboxSettings.Assign(Settings.DropboxSettings);
			Control.FileSystemSettings.Assign(Settings.FileSystemSettings);
			Control.AutoStartUpload = Settings.AutoStartUpload;
			Control.DialogTriggerID = Settings.DialogTriggerID;
			MainUploadControlHiddenUI.SetShowUIMode(Control, Settings.ShowUI);
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureUploaded();
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxUploadControl(ViewContext);
		}
	}
}
