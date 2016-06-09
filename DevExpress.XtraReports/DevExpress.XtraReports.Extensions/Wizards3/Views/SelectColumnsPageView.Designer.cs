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

namespace DevExpress.XtraReports.Wizards3.Views {
	partial class SelectColumnsPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectColumnsPageView));
			this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.availableLabel = new System.Windows.Forms.Label();
			this.selectedLabel = new System.Windows.Forms.Label();
			this.selectedColumnsListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.availableColumnsListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.addButton = new DevExpress.XtraEditors.SimpleButton();
			this.addAllButton = new DevExpress.XtraEditors.SimpleButton();
			this.removeButton = new DevExpress.XtraEditors.SimpleButton();
			this.removeAllButton = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			this.layoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.selectedColumnsListBox)).BeginInit();
			this.contentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.availableColumnsListBox)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font = ((System.Drawing.Font)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.panelBaseContent.Controls.Add(this.layoutPanel);
			resources.ApplyResources(this.panelBaseContent, "panelBaseContent");
			resources.ApplyResources(this.layoutPanel, "layoutPanel");
			this.layoutPanel.Controls.Add(this.availableLabel, 0, 0);
			this.layoutPanel.Controls.Add(this.selectedLabel, 2, 0);
			this.layoutPanel.Controls.Add(this.selectedColumnsListBox, 2, 1);
			this.layoutPanel.Controls.Add(this.contentPanel, 0, 1);
			this.layoutPanel.Controls.Add(this.addButton, 1, 2);
			this.layoutPanel.Controls.Add(this.addAllButton, 1, 3);
			this.layoutPanel.Controls.Add(this.removeButton, 1, 5);
			this.layoutPanel.Controls.Add(this.removeAllButton, 1, 6);
			this.layoutPanel.Name = "layoutPanel";
			resources.ApplyResources(this.availableLabel, "availableLabel");
			this.availableLabel.Name = "availableLabel";
			resources.ApplyResources(this.selectedLabel, "selectedLabel");
			this.selectedLabel.Name = "selectedLabel";
			this.selectedColumnsListBox.DisplayMember = "DisplayName";
			resources.ApplyResources(this.selectedColumnsListBox, "selectedColumnsListBox");
			this.selectedColumnsListBox.IncrementalSearch = true;
			this.selectedColumnsListBox.Name = "selectedColumnsListBox";
			this.layoutPanel.SetRowSpan(this.selectedColumnsListBox, 7);
			this.selectedColumnsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.selectedColumnsListBox.SelectedIndexChanged += new System.EventHandler(this.selectedColumnsListBox_SelectedIndexChanged);
			this.selectedColumnsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.selectedColumnsListBox_MouseDoubleClick);
			this.contentPanel.Controls.Add(this.availableColumnsListBox);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			this.layoutPanel.SetRowSpan(this.contentPanel, 7);
			this.availableColumnsListBox.DisplayMember = "DisplayName";
			resources.ApplyResources(this.availableColumnsListBox, "availableColumnsListBox");
			this.availableColumnsListBox.IncrementalSearch = true;
			this.availableColumnsListBox.Name = "availableColumnsListBox";
			this.availableColumnsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.availableColumnsListBox.SelectedIndexChanged += new System.EventHandler(this.availableColumnsListBox_SelectedIndexChanged);
			this.availableColumnsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableColumnsListBox_MouseDoubleClick);
			resources.ApplyResources(this.addButton, "addButton");
			this.addButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.addButton.Name = "addButton";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			resources.ApplyResources(this.addAllButton, "addAllButton");
			this.addAllButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.addAllButton.Name = "addAllButton";
			this.addAllButton.Click += new System.EventHandler(this.addAllButton_Click);
			resources.ApplyResources(this.removeButton, "removeButton");
			this.removeButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.removeButton.Name = "removeButton";
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			resources.ApplyResources(this.removeAllButton, "removeAllButton");
			this.removeAllButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.removeAllButton.Name = "removeAllButton";
			this.removeAllButton.Click += new System.EventHandler(this.removeAllButton_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.MinimumSize = new System.Drawing.Size(480, 250);
			this.Name = "SelectColumnsPageView";
			this.Tag = "Select the columns you want to display within your report.";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			this.layoutPanel.ResumeLayout(false);
			this.layoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.selectedColumnsListBox)).EndInit();
			this.contentPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.availableColumnsListBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel layoutPanel;
		private System.Windows.Forms.Label availableLabel;
		private System.Windows.Forms.Label selectedLabel;
		private XtraEditors.ListBoxControl availableColumnsListBox;
		private XtraEditors.ListBoxControl selectedColumnsListBox;
		private System.Windows.Forms.Panel contentPanel;
		private XtraEditors.SimpleButton addButton;
		private XtraEditors.SimpleButton addAllButton;
		private XtraEditors.SimpleButton removeButton;
		private XtraEditors.SimpleButton removeAllButton;
	}
}
