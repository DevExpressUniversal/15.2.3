#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.LookAndFeel;
namespace DevExpress.DashboardWin.Native {
	public partial class DashboardTitleForm : DashboardForm {
		readonly Locker locker = new Locker();
		readonly DashboardTitleFormModel itemModel;
		readonly OpenFileDialog dialog = new OpenFileDialog() { Filter = ImageHelper.GetImageFilterString(), FilterIndex = 9 };
		string imageUrl;
		byte[] imageData;
		public DashboardTitleForm() {
			InitializeComponent();
		}
		public DashboardTitleForm(DashboardTitleFormModel itemModel, UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
			InitializeComponent();
			this.itemModel = itemModel;
			locker.Lock();
			layoutControl.BeginUpdate();
			try {
				DashboardTitleState state = itemModel.DefaultState;
				ceVisibleTitle.Checked = state.Visible;
				ceIncludeMasterFilterState.Checked = state.IncludeMasterFilterState;
				ceLeft.Checked = state.Alignment == DashboardTitleAlignment.Left;
				ceCenter.Checked = !ceLeft.Checked;
				teText.Text = state.Text;
				if(state.ImageData != null) {
					try {
						ImportImage(state.ImageData);
					}
					catch {
						imageData = state.ImageData = null;
						DashboardWinHelper.ShowWarningMessage(LookAndFeel, this, DashboardWinLocalizer.GetString(DashboardWinStringId.ImageCorruptedMessage));
					}
				}
				else if(!string.IsNullOrEmpty(state.ImageUrl)) {
					try {
						LoadImage(state.ImageUrl);
					}
					catch {
						imageUrl = state.ImageUrl = null;
						DashboardWinHelper.ShowWarningMessage(LookAndFeel, this, DashboardWinLocalizer.GetString(DashboardWinStringId.ImageCorruptedMessage));
					}
				}
				else
					SetDefaultImage();
				SetControlsEnabledState();
			}
			finally {
				layoutControl.EndUpdate();
				locker.Unlock();
			}
		}
		protected override void DisposeInternal(bool disposing) {
			base.DisposeInternal(disposing);
			if(disposing) {
				if(ieImage.Image != null)
					ieImage.Image.Dispose();
				dialog.Dispose();
			}
		}
		void LoadImage(string path) {
			imageUrl = path;
			ieImage.Image = new ImageLoader().Load(path);
		}
		void ImportImage(byte[] data) {
			ieImage.Image = ImageLoader.FromData(data);
			imageData = data;
		}
		void SetDefaultImage() {
			ieImage.Image = null;
			btnRemove.Enabled = false;
		}
		void SetControlsEnabledState() {
			bool isControlsEnabled = ceVisibleTitle.Checked;
			ceIncludeMasterFilterState.Enabled = isControlsEnabled;
			ceLeft.Enabled = ceCenter.Enabled = isControlsEnabled;
			btnLoadImage.Enabled = btnImport.Enabled = isControlsEnabled;
			btnRemove.Enabled = isControlsEnabled && (imageData != null || imageUrl != null);
			ieImage.Enabled = teText.Enabled = isControlsEnabled;
		}
		void OnLoadImageButtonClick(object sender, EventArgs e) {
			if(dialog.ShowDialog(this) == DialogResult.OK) {
				try {
					LoadImage(dialog.FileName);
					btnRemove.Enabled = true;
					imageData = null;
				}
				catch {
					imageUrl = null;
					btnRemove.Enabled = false;
					DashboardWinHelper.ShowWarningMessage(LookAndFeel, this, DashboardWinLocalizer.GetString(DashboardWinStringId.ImageWrongFormatMessage));
				}
				finally {
					UpdateController();
				}
			}
		}
		void OnImportImageButtonClick(object sender, EventArgs e) {
			if(dialog.ShowDialog(this) == DialogResult.OK) {
				try {
					ImportImage(File.ReadAllBytes(dialog.FileName));
					btnRemove.Enabled = true;
					imageUrl = null;
				}
				catch {
					imageData = null;
					btnRemove.Enabled = false;
					DashboardWinHelper.ShowWarningMessage(LookAndFeel, this, string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.ImageFileCorruptedMessage), dialog.FileName));
				}
				finally {
					UpdateController();
				}
			}
		}
		void OnRemoveImageButtonClick(object sender, EventArgs e) {
			SetDefaultImage();
			imageData = null;
			imageUrl = null;
			btnRemove.Enabled = false;
			UpdateController();
		}
		void OnBtnOKClick(object sender, EventArgs e) {
			itemModel.Apply();
			DialogResult = DialogResult.OK;
		}
		void OnBtnCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void OnBtnApplyClick(object sender, EventArgs e) {
			itemModel.Apply();
			btnApply.Enabled = false;
		}
		void OnVisibleTitleChanged(object sender, EventArgs e) {
			SetControlsEnabledState();
			UpdateController();
		}
		void OnIncludeMasterFilterStateChanged(object sender, EventArgs e) {
			UpdateController();
		}
		void OnAlignmentButttonCheckedChanged(object sender, EventArgs e) {
			UpdateController();
		}
		void OnTextEditChanged(object sender, EventArgs e) {
			UpdateController();
		}
		void UpdateController() {
			if(!locker.IsLocked) {
				itemModel.Update(new DashboardTitleState() {
					Visible = ceVisibleTitle.Checked,
					Alignment = ceLeft.Checked ? DashboardTitleAlignment.Left : DashboardTitleAlignment.Center,
					Text = teText.Text,
					ImageData = imageData,
					ImageUrl = imageUrl,
					IncludeMasterFilterState = ceIncludeMasterFilterState.Checked
				});
				btnApply.Enabled = itemModel.Changed;
			}
		}
	}
}
