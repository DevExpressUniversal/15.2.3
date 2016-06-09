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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class ChooseConnectionPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.radioGroupCreateNewConnection = new DevExpress.XtraEditors.RadioGroup();
			this.listBoxChooseConnection = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemConnectionList = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCreateConnection = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupCreateNewConnection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxChooseConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCreateConnection)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.radioGroupCreateNewConnection);
			this.layoutControlContent.Controls.Add(this.listBoxChooseConnection);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(476, 222, 794, 554);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 1;
			this.radioGroupCreateNewConnection.Location = new System.Drawing.Point(46, 27);
			this.radioGroupCreateNewConnection.Name = "radioGroupCreateNewConnection";
			this.radioGroupCreateNewConnection.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.radioGroupCreateNewConnection.Properties.Appearance.Options.UseBackColor = true;
			this.radioGroupCreateNewConnection.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroupCreateNewConnection.Properties.Columns = 1;
			this.radioGroupCreateNewConnection.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "No, I\'d like to specify the connection parameters myself"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "Yes, let me choose an existing connection from the list")});
			this.radioGroupCreateNewConnection.Size = new System.Drawing.Size(514, 43);
			this.radioGroupCreateNewConnection.StyleController = this.layoutControlContent;
			this.radioGroupCreateNewConnection.TabIndex = 9;
			this.radioGroupCreateNewConnection.EditValueChanged += new System.EventHandler(this.rgCreateNewConnection_EditValueChanged);
			this.listBoxChooseConnection.Location = new System.Drawing.Point(46, 74);
			this.listBoxChooseConnection.Name = "listBoxChooseConnection";
			this.listBoxChooseConnection.Size = new System.Drawing.Size(514, 237);
			this.listBoxChooseConnection.StyleController = this.layoutControlContent;
			this.listBoxChooseConnection.TabIndex = 8;
			this.listBoxChooseConnection.SelectedIndexChanged += new System.EventHandler(this.chooseConnectionListBox_SelectedIndexChanged);
			this.listBoxChooseConnection.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.chooseConnectionListBox_MouseDoubleClick);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemConnectionList,
			this.layoutItemCreateConnection});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(44, 44, 25, 24);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemConnectionList.Control = this.listBoxChooseConnection;
			this.layoutItemConnectionList.Location = new System.Drawing.Point(0, 47);
			this.layoutItemConnectionList.Name = "layoutItemConnectionList";
			this.layoutItemConnectionList.Size = new System.Drawing.Size(518, 241);
			this.layoutItemConnectionList.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionList.TextVisible = false;
			this.layoutItemCreateConnection.Control = this.radioGroupCreateNewConnection;
			this.layoutItemCreateConnection.Location = new System.Drawing.Point(0, 0);
			this.layoutItemCreateConnection.Name = "layoutItemCreateConnection";
			this.layoutItemCreateConnection.Size = new System.Drawing.Size(518, 47);
			this.layoutItemCreateConnection.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCreateConnection.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseConnectionPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.radioGroupCreateNewConnection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxChooseConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCreateConnection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraEditors.ListBoxControl listBoxChooseConnection;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemConnectionList;
		protected XtraEditors.RadioGroup radioGroupCreateNewConnection;
		protected XtraLayout.LayoutControlItem layoutItemCreateConnection;
	}
}
