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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
namespace DevExpress.XtraBars.Customization {
	public class AddItem : XtraForm {
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public TextEdit teItemName;
		public DevExpress.XtraEditors.ComboBoxEdit cbCategories;
		public TextEdit teItemCaption;
		private SimpleButton btOk;
		private SimpleButton btCancel;
		private DevExpress.XtraEditors.ImageComboBoxEdit cbBarItems;
		public DevExpress.XtraEditors.ComboBoxEdit cbEditors;
		public AddItem() {
			InitializeComponent();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			cbBarItemsRect = cbBarItems.Bounds;
		}
		CustomizationForm customizationForm;
		protected internal void Init(CustomizationForm customizationForm) {
			this.LookAndFeel.ParentLookAndFeel = customizationForm.Manager.PaintStyle.CustomizationLookAndFeel;
			this.customizationForm = customizationForm;
			cbEditors.Properties.Items.Clear();
			foreach(DevExpress.XtraEditors.Registrator.EditorClassInfo item in EditorRegistrationInfo.Default.Editors) {
				if(!item.DesignTimeVisible || item.AllowInplaceEditing == ShowInContainerDesigner.Never) continue;
				cbEditors.Properties.Items.Add(item.Name);
			}
			cbEditors.SelectedIndex = 0;
			cbBarItems.Properties.SmallImages = customizationForm.Manager.BarItemsImages;
			cbBarItems.Properties.Items.Clear();
			foreach(BarItemInfo iInfo in customizationForm.Manager.PaintStyle.ItemInfoCollection) {
				if(!iInfo.DesignTimeVisible) continue;
				cbBarItems.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(iInfo.GetCaption(), iInfo, iInfo.ImageIndex));
			}
			cbCategories.Properties.Items.Clear();
			foreach(object cat in customizationForm.LManager.lbCategories.Items) {
				if(cat == BarManagerCategory.TotalCategory) continue;
				cbCategories.Properties.Items.Add(cat);
			}
			cbBarItems.CreateControl();
			cbBarItems.SelectedIndex = 0;
			cbCategories.SelectedItem = customizationForm.LManager.lbCategories.SelectedItem;
			if(cbCategories.SelectedItem == null) cbCategories.SelectedIndex = 0;
			cbBarItems_SelectedIndexChanged(null, null);
		}
		protected internal BarItem CreateItem() {
			BarItemInfo itemInfo = (cbBarItems.SelectedItem != null ? (cbBarItems.SelectedItem as ImageComboBoxItem).Value as BarItemInfo : null);
			BarItem barItem = Activator.CreateInstance(itemInfo.ItemType) as BarItem;
			try {
				if(customizationForm.Manager.IsDesignMode && customizationForm.Manager.Container != null)
					customizationForm.Manager.Container.Add(barItem, teItemName.Text);
			} catch(Exception e) {
				XtraMessageBox.Show(e.Message);
				return null;
			}
			barItem.Caption = teItemCaption.Text;
			barItem.Manager = customizationForm.Manager;
			barItem.Category = cbCategories.SelectedItem as BarManagerCategory;
			barItem.OnItemCreated(itemInfo.ItemType.Equals(typeof(BarEditItem)) ? cbEditors.SelectedItem : null); 
			barItem.UpdateId();
			return barItem;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		Rectangle cbBarItemsRect;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.teItemCaption = new DevExpress.XtraEditors.TextEdit();
			this.label3 = new System.Windows.Forms.Label();
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOk = new DevExpress.XtraEditors.SimpleButton();
			this.teItemName = new DevExpress.XtraEditors.TextEdit();
			this.label4 = new System.Windows.Forms.Label();
			this.cbCategories = new DevExpress.XtraEditors.ComboBoxEdit();
			this.label2 = new System.Windows.Forms.Label();
			this.cbEditors = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbBarItems = new DevExpress.XtraEditors.ImageComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.teItemCaption.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teItemName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCategories.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbEditors.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBarItems.Properties)).BeginInit();
			this.SuspendLayout();
			this.label1.Location = new System.Drawing.Point(9, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			this.teItemCaption.EditValue = "textBox2";
			this.teItemCaption.Location = new System.Drawing.Point(84, 128);
			this.teItemCaption.Name = "teItemCaption";
			this.teItemCaption.Size = new System.Drawing.Size(216, 21);
			this.teItemCaption.TabIndex = 5;
			this.label3.Location = new System.Drawing.Point(9, 89);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(66, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Name:";
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(225, 168);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 24);
			this.btCancel.TabIndex = 7;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOk.Location = new System.Drawing.Point(141, 168);
			this.btOk.Name = "btOk";
			this.btOk.Size = new System.Drawing.Size(75, 24);
			this.btOk.TabIndex = 6;
			this.btOk.Text = "OK";
			this.teItemName.EditValue = "textBox1";
			this.teItemName.Location = new System.Drawing.Point(84, 89);
			this.teItemName.Name = "teItemName";
			this.teItemName.Size = new System.Drawing.Size(216, 21);
			this.teItemName.TabIndex = 4;
			this.label4.Location = new System.Drawing.Point(9, 126);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "Caption";
			this.cbCategories.Location = new System.Drawing.Point(84, 49);
			this.cbCategories.Name = "cbCategories";
			this.cbCategories.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																												 new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbCategories.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbCategories.Size = new System.Drawing.Size(216, 21);
			this.cbCategories.TabIndex = 3;
			this.label2.Location = new System.Drawing.Point(9, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 30);
			this.label2.TabIndex = 1;
			this.label2.Text = "Category:";
			this.cbEditors.Location = new System.Drawing.Point(187, 10);
			this.cbEditors.Name = "cbEditors";
			this.cbEditors.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbEditors.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																											  new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbEditors.Size = new System.Drawing.Size(113, 23);
			this.cbEditors.TabIndex = 2;
			this.cbEditors.Visible = false;
			this.cbBarItems.Location = new System.Drawing.Point(84, 10);
			this.cbBarItems.Name = "cbBarItems";
			this.cbBarItems.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																											   new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbBarItems.Size = new System.Drawing.Size(216, 21);
			this.cbBarItems.TabIndex = 1;
			this.cbBarItems.SelectedIndexChanged += new System.EventHandler(this.cbBarItems_SelectedIndexChanged);
			this.AcceptButton = this.btOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btCancel;
			this.ClientSize = new System.Drawing.Size(314, 200);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cbBarItems,
																		  this.cbEditors,
																		  this.btCancel,
																		  this.btOk,
																		  this.teItemCaption,
																		  this.teItemName,
																		  this.cbCategories,
																		  this.label4,
																		  this.label3,
																		  this.label2,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.Name = "AddItem";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add New BarItem";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.AddItem_Closing);
			((System.ComponentModel.ISupportInitialize)(this.teItemCaption.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teItemName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCategories.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbEditors.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBarItems.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void cbBarItems_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(!cbBarItemsRect.IsEmpty)
				cbBarItems.Bounds = cbBarItemsRect;
			cbEditors.Visible = false;
			ImageComboBoxItem selItem = cbBarItems.SelectedItem as ImageComboBoxItem;
			if(selItem != null) {
				BarItemInfo itemInfo = selItem.Value as BarItemInfo;
				teItemName.Text = GenerateValidName(itemInfo.Name);
				if(itemInfo.ItemType.Equals(typeof(BarEditItem))) {
					cbBarItems.Width = (cbEditors.Left - cbBarItems.Left) - 5;
					cbEditors.Visible = true;
				}
			}
			teItemCaption.Text = teItemName.Text;
		}
		string GenerateValidName(string sample) {
			if(sample.Length > 0) sample = Char.ToLower(sample[0], System.Globalization.CultureInfo.InvariantCulture) + sample.Substring(1);
			string res;
			for(int n = 1;;n++) {
				res = sample + n.ToString();
				if(!customizationForm.Manager.IsDesignMode) break;
				if(customizationForm.Manager.Site == null) break; 
				IContainer cont = customizationForm.Manager.Site.Container;
				bool found = false;
				foreach(IComponent comp in cont.Components) {
					if(comp.Site != null && comp.Site.Name == res) {
						found = true;
					}
				}
				if(!found) break;
			}
			return res;
		}
		private void btCancel_Click(object sender, System.EventArgs e) {
			Close();
		}
		private void AddItem_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(this.DialogResult == DialogResult.OK) {
				if(CreateItem() == null) {
					e.Cancel = true;
					return;
				}
			}
		}
	}
}
