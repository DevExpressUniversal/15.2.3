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

using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class OpenFileForm : RichEditDialogBase {
		protected ASPxFileManager FileManager { get; set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			FileManager = CreateFileManager();
			group.Items.CreateItem("", FileManager).ShowCaption = Utils.DefaultBoolean.False;
		}
		protected virtual ASPxFileManager CreateFileManager() {
			return new RichEditFileManager();
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			FileManager.ID = "FileManager";
			FileManager.ClientInstanceName = "FileManager";
			FileManager.Border.BorderStyle = BorderStyle.None;
			FileManager.Styles.FolderContainer.Width = new Unit(250D, UnitType.Pixel);
			FileManager.Styles.File.Margins.Margin = new Unit(1D, UnitType.Pixel);
			PrepareFileManager();
		}
		protected void PrepareFileManager() {
			FileManager.Styles.CopyFrom(RichEdit.StylesFileManager);
			FileManager.ControlStyle.CopyFrom(RichEdit.StylesFileManager.Control);
			FileManager.Settings.Assign(RichEdit.SettingsDocumentSelector.CommonSettings);
			FileManager.SettingsEditing.Assign(RichEdit.SettingsDocumentSelector.EditingSettings);
			FileManager.SettingsFolders.Assign(RichEdit.SettingsDocumentSelector.FoldersSettings);
			FileManager.SettingsToolbar.Assign(RichEdit.SettingsDocumentSelector.ToolbarSettings);
			FileManager.SettingsUpload.Assign(RichEdit.SettingsDocumentSelector.UploadSettings);
			FileManager.SettingsPermissions.Assign(RichEdit.SettingsDocumentSelector.PermissionSettings);
			FileManager.SettingsFileList.Assign(RichEdit.SettingsDocumentSelector.FileListSettings);
			FileManager.Settings.RootFolder = RichEdit.GetWorkDirectory();
			FileManager.ClientSideEvents.CustomCommand = RichEdit.SettingsDocumentSelector.CustomCommand;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgOpenFileForm";
		}
	}
}
