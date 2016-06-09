#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Utils;
using System.IO;
namespace DevExpress.ExpressApp.ModelEditor {
	public partial class OpenModelForm : XtraForm {
		public OpenModelForm() {
			InitializeComponent();
			this.Tag = "testdialog=";
			Image modelEditorImage = ImageLoader.Instance.GetLargeImageInfo("EditModel").Image;
			if(modelEditorImage != null) {
				this.Icon = Icon.FromHandle(new Bitmap(modelEditorImage).GetHicon());
			}
			UpdateLoadModelButtonState();
			this.CenterToScreen();
		}
		private void browseTargetFilePathButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Configuration Files (*.config)|*.config|Assembly Files (*.dll)|*.dll|All Files|*.*";
			if(openFileDialog.ShowDialog(this) == DialogResult.OK) {
				targetFileNameTextBox.Text = openFileDialog.FileName;
				string fileNameDirectory = Path.GetDirectoryName(openFileDialog.FileName);
				string binDirectory = Path.GetDirectoryName(fileNameDirectory);
				if(Path.GetFileName(binDirectory) == "bin") {
					diffsPathTextBox.Text = Path.GetDirectoryName(binDirectory);
				}
			}
		}
		private void browseDiffsPathButton_Click(object sender, EventArgs e) {
			FolderBrowserDialog folderDialog = new FolderBrowserDialog();
			folderDialog.ShowNewFolderButton = false;
			folderDialog.Description = "Select the model differences path:";
			if(!string.IsNullOrEmpty(targetFileNameTextBox.Text)) {
				folderDialog.SelectedPath = Path.GetDirectoryName(targetFileNameTextBox.Text);
			}
			if(folderDialog.ShowDialog(this) == DialogResult.OK) {
				diffsPathTextBox.Text = folderDialog.SelectedPath;
			}
		}
		private void UpdateLoadModelButtonState() {
			bool isConfigFile = string.Compare(Path.GetExtension(targetFileNameTextBox.Text), ".config", true) == 0;
			if(isConfigFile && diffsPathTextBox.Text != Path.GetDirectoryName(targetFileNameTextBox.Text)) {
				diffsPathTextBox.Text = Path.GetDirectoryName(targetFileNameTextBox.Text);
			}
			diffsPathTextBox.Enabled = !isConfigFile;
			browseDiffsPathButton.Enabled = !isConfigFile;
			if(string.IsNullOrEmpty(targetFileNameTextBox.Text) || string.IsNullOrEmpty(diffsPathTextBox.Text)) {
				loadModelButton.Enabled = false;
			}
			else {
				try {
					FileInfo fileInfo = new FileInfo(targetFileNameTextBox.Text);
					DirectoryInfo directoryInfo = new DirectoryInfo(diffsPathTextBox.Text);
					loadModelButton.Enabled = fileInfo.Exists && directoryInfo.Exists;
				}
				catch {
					loadModelButton.Enabled = false;
				}
			}
		}
		private void targetFileNameTextBox_TextChanged(object sender, EventArgs e) {
			UpdateLoadModelButtonState();
		}
		private void diffsPathTextBox_TextChanged(object sender, EventArgs e) {
			UpdateLoadModelButtonState();
		}
		public string TargetFileName {
			get { return targetFileNameTextBox.Text; }
		}
		public string ModelDifferencesStorePath {
			get { return diffsPathTextBox.Text; }
		}
		public string DeviceSpecificDifferencesStoreName {
			get {
				if(deviceSpecificModelSelector != null && foundModelsDiffs != null) {
					int selectedIndex = deviceSpecificModelSelector.SelectedIndex;
					if(selectedIndex > 0 && foundModelsDiffs.Count > selectedIndex) {
						return foundModelsDiffs[deviceSpecificModelSelector.SelectedIndex];
					}
				}
				return null;
			}
		}
		public string AssembliesPath {
			get { return assembliesPath; }
		}
		private void label1_Click(object sender, EventArgs e) {
		}
		string assembliesPath;
		DeviceSpecificModelSelector deviceSpecificModelSelector;
		List<string> foundModelsDiffs;
		private void loadModelButton_Click(object sender, EventArgs e) {
			bool closeDialog = true;
			string targetFileName = TargetFileName;
			string diffsPath = ModelDifferencesStorePath;
			assembliesPath = System.IO.Path.GetDirectoryName(targetFileName);
			if(assembliesPath == "") {
				assembliesPath = Environment.CurrentDirectory;
			}
			DesignerModelFactory dmf = new DesignerModelFactory();
			if(dmf.IsApplication(targetFileName)) {
				if(string.IsNullOrEmpty(diffsPath)) {
					diffsPath = assembliesPath;
				}
				foundModelsDiffs = new List<string>();
				foreach(string fileName in Directory.GetFiles(diffsPath)) {
					string foundDeviceModel = GetDeviceSpecificModelDiffDefaultName(fileName);
					if(!string.IsNullOrEmpty(foundDeviceModel)) {
						foundModelsDiffs.Add(foundDeviceModel);
					}
				}
				if(foundModelsDiffs.Count > 1) {
					closeDialog = false;
					foundModelsDiffs.Sort();
					deviceSpecificModelSelector = new DeviceSpecificModelSelector();
					deviceSpecificModelSelector.SetItems(foundModelsDiffs);
					this.SuspendLayout();
					this.Controls.Clear();
					this.Controls.Add(deviceSpecificModelSelector);
					this.ResumeLayout(false);
					this.PerformLayout();
				}
			}
			if(closeDialog) {
				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}
		private string GetDeviceSpecificModelDiffDefaultName(string targetDiffFileName) {
			if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultMobileName + ModelDifferenceStore.ModelFileExtension)) {
				return ModelDifferenceStore.AppDiffDefaultMobileName;
			}
			else {
				if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultTabletName + ModelDifferenceStore.ModelFileExtension)) {
					return ModelDifferenceStore.AppDiffDefaultTabletName;
				}
				else {
					if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultDesktopName + ModelDifferenceStore.ModelFileExtension)) {
						return ModelDifferenceStore.AppDiffDefaultDesktopName;
					}
					else {
						if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultName + ModelDifferenceStore.ModelFileExtension)) {
							return ModelDifferenceStore.AppDiffDefaultName;
						}
					}
				}
			}
			return null;
		}
	}
}
