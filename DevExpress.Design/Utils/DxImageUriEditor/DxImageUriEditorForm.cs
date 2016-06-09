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
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Design {
	public partial class DxImageUriEditorForm : XtraForm {
		DxImageUri editValue;
		static DxImageUriEditorForm() {
			SkinManager.EnableFormSkins();
		}
		public DxImageUriEditorForm() {
			InitializeComponent();
			WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
			LookAndFeel.SetSkinStyle(UserLookAndFeel.Default.ActiveSkinName);
		}
		public void SetEditValue(DxImageUri editValue) {
			if(editValue == null) {
				this.editValue = new DxImageUri();
			}
			else {
				this.editValue = editValue;
			}
		}
		protected void UpdateEditValue(string val) {
			this.editValue.Uri = val;
			UpdateUI();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitializeGallery(activeGroup => activeGroup.Items.AddRange(GetItems()));
			UpdateUI();
		}
		void UpdateUI() {
			this.btnOk.Enabled = this.editValue.IsInitialized;
		}
		protected GalleryItem[] GetItems() {
			List<GalleryItem> list = new List<GalleryItem>();
			string[] baseImages = DxImageAssemblyUtil.ImageProvider.GetBaseImages();
			for(int i = 0; i < baseImages.Length; i++) {
				list.Add(GetItems(baseImages[i]));
			}
			return list.ToArray();
		}
		protected GalleryItem GetItems(string itemName) {
			GalleryItem item = new GalleryItem();
			item.Hint = itemName;
			item.Tag = itemName;
			item.Checked = GetIsChecked(itemName);
			item.Image = DxImageAssemblyUtil.ImageProvider.GetImage(itemName, ImageSize.Size32x32, DxImageUriUtils.GetImageType(LookAndFeel));
			return item;
		}
		protected bool GetIsChecked(string itemName) {
			if(!editValue.IsInitialized) return false;
			return editValue.IsMatch(itemName);
		}
		public DxImageUri GetResult() {
			return editValue;
		}
		void InitializeGallery(Action<GalleryItemGroup> handler) {
			Debug.Assert(this.gallery.Gallery.Groups.Count == 1);
			this.gallery.Gallery.BeginUpdate();
			try {
				GalleryItemGroup group = this.gallery.Gallery.Groups.First();
				handler(group);
			}
			finally {
				this.gallery.Gallery.EndUpdate();
			}
		}
		void OnOkButtonClick(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		void OnGalleryControlItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			DialogResult = DialogResult.OK;
		}
		void OnGalleryControlGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			string val = (string)e.Item.Tag;
			UpdateEditValue(val);
		}
	}
}
