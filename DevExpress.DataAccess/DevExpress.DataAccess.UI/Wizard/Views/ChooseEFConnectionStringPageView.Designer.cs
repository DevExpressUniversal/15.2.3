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
	partial class ChooseEFConnectionStringPageView {
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
			this.checkChooseConnection = new DevExpress.XtraEditors.CheckEdit();
			this.checkCreateNewConnection = new DevExpress.XtraEditors.CheckEdit();
			this.listBoxChooseConnection = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemConnectionsList = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemNewConnection = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemChooseConnection = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.checkChooseConnection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkCreateNewConnection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxChooseConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionsList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNewConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemChooseConnection)).BeginInit();
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
			this.layoutControlContent.Controls.Add(this.checkChooseConnection);
			this.layoutControlContent.Controls.Add(this.checkCreateNewConnection);
			this.layoutControlContent.Controls.Add(this.listBoxChooseConnection);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(949, 238, 794, 554);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 1;
			this.checkChooseConnection.EditValue = true;
			this.checkChooseConnection.Location = new System.Drawing.Point(45, 48);
			this.checkChooseConnection.Name = "checkChooseConnection";
			this.checkChooseConnection.Properties.AutoWidth = true;
			this.checkChooseConnection.Properties.Caption = "Yes, let me choose from list";
			this.checkChooseConnection.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.checkChooseConnection.Properties.RadioGroupIndex = 0;
			this.checkChooseConnection.Size = new System.Drawing.Size(153, 19);
			this.checkChooseConnection.StyleController = this.layoutControlContent;
			this.checkChooseConnection.TabIndex = 5;
			this.checkCreateNewConnection.Location = new System.Drawing.Point(45, 26);
			this.checkCreateNewConnection.Name = "checkCreateNewConnection";
			this.checkCreateNewConnection.Properties.AutoWidth = true;
			this.checkCreateNewConnection.Properties.Caption = "No, specify a custom connection string";
			this.checkCreateNewConnection.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.checkCreateNewConnection.Properties.RadioGroupIndex = 0;
			this.checkCreateNewConnection.Size = new System.Drawing.Size(207, 19);
			this.checkCreateNewConnection.StyleController = this.layoutControlContent;
			this.checkCreateNewConnection.TabIndex = 4;
			this.checkCreateNewConnection.TabStop = false;
			this.checkCreateNewConnection.CheckedChanged += new System.EventHandler(this.ceCreateNewConnection_CheckedChanged);
			this.listBoxChooseConnection.Location = new System.Drawing.Point(46, 71);
			this.listBoxChooseConnection.Name = "listBoxChooseConnection";
			this.listBoxChooseConnection.Size = new System.Drawing.Size(514, 240);
			this.listBoxChooseConnection.StyleController = this.layoutControlContent;
			this.listBoxChooseConnection.TabIndex = 8;
			this.listBoxChooseConnection.SelectedIndexChanged += new System.EventHandler(this.chooseConnectionListBox_SelectedIndexChanged);
			this.listBoxChooseConnection.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.chooseConnectionListBox_MouseDoubleClick);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemConnectionsList,
			this.layoutItemNewConnection,
			this.layoutItemChooseConnection});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "Root";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(44, 44, 25, 24);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemConnectionsList.Control = this.listBoxChooseConnection;
			this.layoutItemConnectionsList.Location = new System.Drawing.Point(0, 44);
			this.layoutItemConnectionsList.Name = "layoutItemConnectionsList";
			this.layoutItemConnectionsList.Size = new System.Drawing.Size(518, 244);
			this.layoutItemConnectionsList.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionsList.TextVisible = false;
			this.layoutItemNewConnection.Control = this.checkCreateNewConnection;
			this.layoutItemNewConnection.Location = new System.Drawing.Point(0, 0);
			this.layoutItemNewConnection.Name = "layoutItemNewConnection";
			this.layoutItemNewConnection.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
			this.layoutItemNewConnection.Size = new System.Drawing.Size(518, 21);
			this.layoutItemNewConnection.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemNewConnection.TextVisible = false;
			this.layoutItemChooseConnection.Control = this.checkChooseConnection;
			this.layoutItemChooseConnection.Location = new System.Drawing.Point(0, 21);
			this.layoutItemChooseConnection.Name = "layoutItemChooseConnection";
			this.layoutItemChooseConnection.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 2, 2, 2);
			this.layoutItemChooseConnection.Size = new System.Drawing.Size(518, 23);
			this.layoutItemChooseConnection.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemChooseConnection.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseEFConnectionStringPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.checkChooseConnection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkCreateNewConnection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxChooseConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionsList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNewConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemChooseConnection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemConnectionsList;
		protected XtraEditors.CheckEdit checkChooseConnection;
		protected XtraLayout.LayoutControlItem layoutItemNewConnection;
		protected XtraLayout.LayoutControlItem layoutItemChooseConnection;
		protected XtraEditors.CheckEdit checkCreateNewConnection;
		protected XtraEditors.ListBoxControl listBoxChooseConnection;
	}
}
