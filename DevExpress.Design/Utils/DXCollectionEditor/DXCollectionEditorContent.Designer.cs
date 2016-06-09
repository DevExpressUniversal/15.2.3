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

namespace DevExpress.Utils.Design {
	partial class DXCollectionEditorContent {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
			this.multiColumnListView.SelectedItemChanged -= ListView_SelectedItemChanged;
			this.multiColumnListView.MultiColumnListBox.MouseDown -= this.MultiColumnListView_MouseDown;
			removeItem.Click -= new System.EventHandler(RemoveButton_Click);
			foreach(System.IDisposable element in disposableElements) {
				element.Dispose();
			}
			disposableElements.Clear();
			disposableElements = null;
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.addItemDropDownButton = new DevExpress.XtraEditors.DropDownButton();
			this.removeItemButton = new DevExpress.XtraEditors.SimpleButton();
			this.upItemButton = new DevExpress.XtraEditors.SimpleButton();
			this.downItemButton = new DevExpress.XtraEditors.SimpleButton();
			this.propertyGrid = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			this.previewSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.itemsGroupControl = new DevExpress.XtraEditors.GroupControl();
			this.itemsSearchControl = new DevExpress.XtraEditors.SearchControl();
			this.itemsListSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.addRemoveStackPanel = new DevExpress.XtraEditors.Internal.StackPanelControl();
			this.searchButton = new DevExpress.XtraEditors.CheckButton();
			this.otherButtonsStackPanel = new DevExpress.XtraEditors.Internal.StackPanelControl();
			this.manageButtonsStackPanel = new DevExpress.XtraEditors.Internal.StackPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.previewSplitContainerControl)).BeginInit();
			this.previewSplitContainerControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.itemsGroupControl)).BeginInit();
			this.itemsGroupControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.itemsSearchControl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsListSplitContainerControl)).BeginInit();
			this.itemsListSplitContainerControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.addRemoveStackPanel)).BeginInit();
			this.addRemoveStackPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.otherButtonsStackPanel)).BeginInit();
			this.otherButtonsStackPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.manageButtonsStackPanel)).BeginInit();
			this.manageButtonsStackPanel.SuspendLayout();
			this.SuspendLayout();
			this.addItemDropDownButton.Location = new System.Drawing.Point(0, 0);
			this.addItemDropDownButton.Name = "addItemDropDownButton";
			this.addItemDropDownButton.Size = new System.Drawing.Size(135, 30);
			this.addItemDropDownButton.TabIndex = 6;
			this.addItemDropDownButton.Text = "Add";
			this.addItemDropDownButton.ArrowButtonClick += new System.EventHandler(this.AddItemDropDownButton_ArrowButtonClick);
			this.addItemDropDownButton.Click += new System.EventHandler(this.AddItemDropDownButton_Click);
			this.removeItemButton.Enabled = false;
			this.removeItemButton.Location = new System.Drawing.Point(139, 0);
			this.removeItemButton.Name = "removeItemButton";
			this.removeItemButton.Size = new System.Drawing.Size(75, 30);
			this.removeItemButton.TabIndex = 1;
			this.removeItemButton.Text = "Remove";
			this.removeItemButton.Click += new System.EventHandler(this.RemoveButton_Click);
			this.upItemButton.Enabled = false;
			this.upItemButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.upItemButton.Location = new System.Drawing.Point(34, 0);
			this.upItemButton.Name = "upItemButton";
			this.upItemButton.Size = new System.Drawing.Size(30, 30);
			this.upItemButton.TabIndex = 4;
			this.upItemButton.Text = "U";
			this.upItemButton.Click += new System.EventHandler(this.UpItemButton_Click);
			this.downItemButton.Enabled = false;
			this.downItemButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.downItemButton.Location = new System.Drawing.Point(0, 0);
			this.downItemButton.Name = "downItemButton";
			this.downItemButton.Size = new System.Drawing.Size(30, 30);
			this.downItemButton.TabIndex = 5;
			this.downItemButton.Text = "D";
			this.downItemButton.Click += new System.EventHandler(this.DownItemButton_Click);
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.DrawFlat = false;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ShowSearchPanel = true;
			this.propertyGrid.Size = new System.Drawing.Size(157, 293);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PgMain_PropertyValueChanged);
			this.previewSplitContainerControl.CaptionImageUri.Uri = "";
			this.previewSplitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewSplitContainerControl.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
			this.previewSplitContainerControl.Location = new System.Drawing.Point(0, 0);
			this.previewSplitContainerControl.Margin = new System.Windows.Forms.Padding(0);
			this.previewSplitContainerControl.Name = "previewSplitContainerControl";
			this.previewSplitContainerControl.Panel1.Text = "Panel1";
			this.previewSplitContainerControl.Panel2.Controls.Add(this.propertyGrid);
			this.previewSplitContainerControl.Panel2.Text = "Panel2";
			this.previewSplitContainerControl.Size = new System.Drawing.Size(524, 293);
			this.previewSplitContainerControl.SplitterPosition = 157;
			this.previewSplitContainerControl.TabIndex = 0;
			this.previewSplitContainerControl.Text = "splitContainerControl4";
			this.itemsGroupControl.CaptionImageUri.Uri = "";
			this.itemsGroupControl.Controls.Add(this.itemsSearchControl);
			this.itemsGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemsGroupControl.Location = new System.Drawing.Point(0, 0);
			this.itemsGroupControl.Name = "itemsGroupControl";
			this.itemsGroupControl.Size = new System.Drawing.Size(188, 293);
			this.itemsGroupControl.TabIndex = 0;
			this.itemsGroupControl.Text = "Items";
			this.itemsSearchControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.itemsSearchControl.Location = new System.Drawing.Point(2, 20);
			this.itemsSearchControl.Name = "itemsSearchControl";
			this.itemsSearchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.itemsSearchControl.Properties.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.ItemsSearchControl_Properties_EditValueChanging);
			this.itemsSearchControl.Size = new System.Drawing.Size(184, 20);
			this.itemsSearchControl.TabIndex = 0;
			this.itemsSearchControl.Visible = false;
			this.itemsListSplitContainerControl.CaptionImageUri.Uri = "";
			this.itemsListSplitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemsListSplitContainerControl.Location = new System.Drawing.Point(8, 56);
			this.itemsListSplitContainerControl.Name = "itemsListSplitContainerControl";
			this.itemsListSplitContainerControl.Panel1.Controls.Add(this.itemsGroupControl);
			this.itemsListSplitContainerControl.Panel1.Text = "Panel1";
			this.itemsListSplitContainerControl.Panel2.Controls.Add(this.previewSplitContainerControl);
			this.itemsListSplitContainerControl.Panel2.Text = "Panel2";
			this.itemsListSplitContainerControl.Size = new System.Drawing.Size(717, 293);
			this.itemsListSplitContainerControl.SplitterPosition = 188;
			this.itemsListSplitContainerControl.TabIndex = 0;
			this.itemsListSplitContainerControl.Text = "splitContainerControl2";
			this.addRemoveStackPanel.Controls.Add(this.addItemDropDownButton);
			this.addRemoveStackPanel.Controls.Add(this.removeItemButton);
			this.addRemoveStackPanel.ItemIndent = 4;
			this.addRemoveStackPanel.Location = new System.Drawing.Point(0, 0);
			this.addRemoveStackPanel.Name = "addRemoveStackPanel";
			this.addRemoveStackPanel.Size = new System.Drawing.Size(214, 30);
			this.addRemoveStackPanel.TabIndex = 0;
			this.searchButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.searchButton.Location = new System.Drawing.Point(68, 0);
			this.searchButton.Name = "searchButton";
			this.searchButton.Size = new System.Drawing.Size(30, 30);
			this.searchButton.TabIndex = 7;
			this.searchButton.Text = "S";
			this.searchButton.CheckedChanged += new System.EventHandler(this.SearchButton_CheckedChanged);
			this.otherButtonsStackPanel.Controls.Add(this.downItemButton);
			this.otherButtonsStackPanel.Controls.Add(this.upItemButton);
			this.otherButtonsStackPanel.Controls.Add(this.searchButton);
			this.otherButtonsStackPanel.ItemIndent = 4;
			this.otherButtonsStackPanel.Location = new System.Drawing.Point(239, 0);
			this.otherButtonsStackPanel.Name = "otherButtonsStackPanel";
			this.otherButtonsStackPanel.Size = new System.Drawing.Size(98, 30);
			this.otherButtonsStackPanel.TabIndex = 1;
			this.manageButtonsStackPanel.Controls.Add(this.addRemoveStackPanel);
			this.manageButtonsStackPanel.Controls.Add(this.otherButtonsStackPanel);
			this.manageButtonsStackPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.manageButtonsStackPanel.ItemIndent = 25;
			this.manageButtonsStackPanel.Location = new System.Drawing.Point(8, 8);
			this.manageButtonsStackPanel.Name = "manageButtonsStackPanel";
			this.manageButtonsStackPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 18);
			this.manageButtonsStackPanel.Size = new System.Drawing.Size(717, 48);
			this.manageButtonsStackPanel.TabIndex = 11;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.itemsListSplitContainerControl);
			this.Controls.Add(this.manageButtonsStackPanel);
			this.Name = "DXCollectionEditorContent";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.Size = new System.Drawing.Size(733, 357);
			((System.ComponentModel.ISupportInitialize)(this.previewSplitContainerControl)).EndInit();
			this.previewSplitContainerControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.itemsGroupControl)).EndInit();
			this.itemsGroupControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.itemsSearchControl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsListSplitContainerControl)).EndInit();
			this.itemsListSplitContainerControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.addRemoveStackPanel)).EndInit();
			this.addRemoveStackPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.otherButtonsStackPanel)).EndInit();
			this.otherButtonsStackPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.manageButtonsStackPanel)).EndInit();
			this.manageButtonsStackPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SplitContainerControl itemsListSplitContainerControl;
		private DevExpress.XtraEditors.GroupControl itemsGroupControl;
		private DevExpress.XtraEditors.SplitContainerControl previewSplitContainerControl;
		private DevExpress.XtraEditors.SimpleButton downItemButton;
		private DevExpress.XtraEditors.SimpleButton upItemButton;
		private DevExpress.XtraEditors.DropDownButton addItemDropDownButton;
		private DevExpress.XtraEditors.SimpleButton removeItemButton;
		private DevExpress.XtraEditors.Internal.StackPanelControl addRemoveStackPanel;
		private DevExpress.XtraEditors.Internal.StackPanelControl otherButtonsStackPanel;
		private DevExpress.XtraEditors.Internal.StackPanelControl manageButtonsStackPanel;
		private DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx propertyGrid;
		private DevExpress.XtraEditors.SearchControl itemsSearchControl;
		private DevExpress.XtraEditors.CheckButton searchButton;
	}
}
