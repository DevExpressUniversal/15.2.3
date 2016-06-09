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
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class UploadControlSettings : SettingsBase {
		AddButtonProperties addButton;
		BrowseButtonProperties browseButton;
		CancelButtonProperties cancelButton;
		ImagePropertiesEx clearFileSelectionImage;
		UploadAdvancedModeSettings advancedModeSettings;
		UploadProgressBarSettings progressBarSettings;
		RemoveButtonProperties removeButton;
		UploadButtonProperties uploadButton;
		UploadControlValidationSettings validationSettings;
		UploadControlAzureSettings azureSettings;
		UploadControlAmazonSettings amazonSettings;
		UploadControlDropboxSettings dropboxSettings;
		UploadControlFileSystemSettings fileSystemSettings;
		public UploadControlSettings() {
			this.addButton = new AddButtonProperties(null);
			this.browseButton = new BrowseButtonProperties(null);
			this.cancelButton = new CancelButtonProperties(null);
			this.clearFileSelectionImage = new ImagePropertiesEx();
			this.advancedModeSettings = new UploadAdvancedModeSettings(null);
			this.progressBarSettings = new UploadProgressBarSettings(null);
			this.removeButton = new RemoveButtonProperties(null);
			this.uploadButton = new UploadButtonProperties(null);
			this.validationSettings = new UploadControlValidationSettings(null);
			this.azureSettings = new UploadControlAzureSettings(null);
			this.amazonSettings = new UploadControlAmazonSettings(null);
			this.dropboxSettings = new UploadControlDropboxSettings(null);
			this.fileSystemSettings = new UploadControlFileSystemSettings(null);
			FileInputCount = 1;
			ShowClearFileSelectionButton = true;
			CancelButtonHorizontalPosition = CancelButtonHorizontalPosition.Center;
			ShowUI = true;
			ShowTextBox = true;
		}
		public AddButtonProperties AddButton { get { return addButton; } }
		public BrowseButtonProperties BrowseButton { get { return browseButton; } }
		public AddUploadButtonsHorizontalPosition AddUploadButtonsHorizontalPosition { get; set; }
		public Unit AddUploadButtonsSpacing { get; set; }
		public Unit ButtonSpacing { get; set; }
		public object CallbackRouteValues { get; set; }
		public CancelButtonProperties CancelButton { get { return cancelButton; } }
		public ImagePropertiesEx ClearFileSelectionImage { get { return clearFileSelectionImage; } }
		public CancelButtonHorizontalPosition CancelButtonHorizontalPosition { get; set; }
		public Unit CancelButtonSpacing { get; set; }
		public UploadControlClientSideEvents ClientSideEvents { get { return (UploadControlClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public int FileInputCount { get; set; }
		public Unit FileInputSpacing { get; set; }
		public UploadControlUploadMode UploadMode { get; set; }
		public UploadControlUploadStorage UploadStorage { get; set; }
		public UploadAdvancedModeSettings AdvancedModeSettings { get { return advancedModeSettings; } }
		public UploadProgressBarSettings ProgressBarSettings { get { return progressBarSettings; } }
		public RemoveButtonProperties RemoveButton { get { return removeButton; } }
		public Unit RemoveButtonSpacing { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowAddRemoveButtons { get; set; }
		public bool ShowClearFileSelectionButton { get; set; }
		public bool ShowProgressPanel { get; set; }
		public bool ShowUploadButton { get; set; }
		public bool ShowTextBox { get; set; }
		protected internal bool ShowUI { get; set; }
		public int Size { get; set; }
		public string NullText { get; set; }
		public UploadControlStyles Styles { get { return (UploadControlStyles)StylesInternal; } }
		public UploadButtonProperties UploadButton { get { return uploadButton; } }
		public UploadControlValidationSettings ValidationSettings { get { return validationSettings; } }
		public UploadControlAzureSettings AzureSettings { get { return azureSettings; } }
		public UploadControlAmazonSettings AmazonSettings { get { return amazonSettings; } }
		public UploadControlDropboxSettings DropboxSettings { get { return dropboxSettings; } }
		public UploadControlFileSystemSettings FileSystemSettings { get { return fileSystemSettings; } }
		public bool AutoStartUpload { get; set; }
		public string DialogTriggerID { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new UploadControlClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return new UploadControlStyles(null);
		}
	}
}
