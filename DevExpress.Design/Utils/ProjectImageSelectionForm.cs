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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils.Design.ProjectImagePicker {
	[CLSCompliant(false)]
	public partial class ProjectImageSelectionForm : XtraForm {
		IProjectImagePickerOwner owner;
		ProjectImageResourceResearcher researcher;
		static ProjectImageSelectionForm() {
			SkinManager.EnableFormSkins();
		}
		public ProjectImageSelectionForm(IProjectImagePickerOwner owner) {
			EnvDTE.Project project = ProjectHelper.GetActiveProject(owner.ServiceProvider);
			this.researcher = new ProjectImageResourceResearcher(project, owner.ServiceProvider);
			this.owner = owner;
			InitializeComponent();
			InitResourceFileList();
		}
		protected virtual void InitResourceFileList() {
			lbImage.Items.Clear();
			cbResourceFile.Properties.Items.Clear();
			researcher.Refresh(null);
			ProjectResourceCollection res = this.researcher.ImageResourceCollection;
			UpdateFormState(res.Count != 0);
			if(res.Count == 0)
				return;
			foreach(ProjectResourceInfo resourceInfo in res) cbResourceFile.Properties.Items.Add(resourceInfo);
			cbResourceFile.SelectedIndex = 0;
		}
		protected virtual void InitImageList(ProjectResourceInfo resourceInfo) {
			lbImage.Items.Clear();
			lbImage.Enabled = (resourceInfo.Count != 0);
			if(resourceInfo.Count == 0) return;
			foreach(ProjectImage imageInfo in resourceInfo) {
				CheckedListBoxItem item = new CheckedListBoxItem(imageInfo);
				if(this.owner.IsSelected(imageInfo)) item.CheckState = CheckState.Checked;
				lbImage.Items.Add(item);
			}
		}
		void UpdateFormState(bool active) {
			labelNotResFound.Visible = !active;
			cbResourceFile.Enabled = lbImage.Enabled = pePreview.Enabled = btnSelect.Enabled = active;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode == Keys.A && e.Control) {
				if(this.lbImage.Focused)
					this.lbImage.SelectAll();
			}
		}
		#region Event Handlers
		void ResourceFile_SelectedIndexChanged(object sender, EventArgs e) {
			ProjectResourceInfo resourceInfo = (ProjectResourceInfo)cbResourceFile.SelectedItem;
			InitImageList(resourceInfo);
		}
		void ImageList_SelectedIndexChanged(object sender, EventArgs e) {
			BaseListBoxControl.SelectedItemCollection items = lbImage.SelectedItems;
			if(items.Count == 0) {
				pePreview.Image = null;
				return;
			}
			if(items.Count != 1) return;
			pePreview.Image = GetImage(0).Image;
		}
		void btnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		ProjectImage GetImage(int pos) {
			CheckedListBoxItem item = (CheckedListBoxItem)lbImage.SelectedItems[pos];
			return (ProjectImage)item.Value;
		}
		#endregion
		[CLSCompliant(false)]
		public virtual ProjectResourceInfo GetValues() {
			if(!IsValidSelection) return null;
			ProjectResourceInfo res = new ProjectResourceInfo(ProjectResourceName);
			foreach(CheckedListBoxItem item in lbImage.Items) {
				if(IsAcceptAsResultValue(item)) res.Add((ProjectImage)item.Value);
			}
			return res;
		}
		protected virtual bool IsAcceptAsResultValue(CheckedListBoxItem item) {
			return item.CheckState == CheckState.Checked;
		}
		string ProjectResourceName {
			get {
				var info = cbResourceFile.SelectedItem as ProjectResourceInfo;
				return info.ResourceName;
			}
		}
		bool IsValidSelection {
			get {
				if(cbResourceFile.SelectedItem == null)
					return false;
				if(lbImage.SelectedItems == null || lbImage.SelectedItems.Count == 0)
					return false;
				return true;
			}
		}
		protected CheckedListBoxControl ImageListBox { get { return lbImage; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) components.Dispose();
				components = null;
				if(this.researcher != null) this.researcher.Dispose();
				this.researcher = null;
			}
			base.Dispose(disposing);
		}
	}
	[CLSCompliant(false)]
	public class AddOnlyProjectImageSelectionForm : ProjectImageSelectionForm {
		public AddOnlyProjectImageSelectionForm(IProjectImagePickerOwner owner) : base(owner) {
			ImageListBox.ItemChecking += OnImageListBoxItemChecking;
		}
		void OnImageListBoxItemChecking(object sender, ItemCheckingEventArgs e) {
			CheckedListBoxItem item = ((CheckedListBoxControl)sender).Items[e.Index];
			if(!item.Enabled) e.Cancel = true;
		}
		protected override void InitImageList(ProjectResourceInfo resourceInfo) {
			base.InitImageList(resourceInfo);
			foreach(CheckedListBoxItem item in ImageListBox.Items) {
				if(item.CheckState == CheckState.Checked) item.Enabled = false;
			}
		}
		protected override bool IsAcceptAsResultValue(CheckedListBoxItem item) {
			return base.IsAcceptAsResultValue(item) && item.Enabled;
		}
	}
	[CLSCompliant(false)]
	public interface IProjectImagePickerOwner {
		IServiceProvider ServiceProvider { get; }
		bool IsSelected(ProjectImage img);
	}
}
