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
	partial class ConfigureEFConnectionStringPageView {
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
			this.checkCustomConnectionString = new DevExpress.XtraEditors.CheckEdit();
			this.checkDefaultConnectionString = new DevExpress.XtraEditors.CheckEdit();
			this.textConnectionName = new DevExpress.XtraEditors.TextEdit();
			this.checkSaveConnection = new DevExpress.XtraEditors.CheckEdit();
			this.memoConnectionString = new DevExpress.XtraEditors.MemoEdit();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemConnectionString = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemConnectionName = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSaveConnection = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCustomConString = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemDefaultConString = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.checkCustomConnectionString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkDefaultConnectionString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textConnectionName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSaveConnection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.memoConnectionString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSaveConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCustomConString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultConString)).BeginInit();
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
			this.layoutControlContent.Controls.Add(this.checkCustomConnectionString);
			this.layoutControlContent.Controls.Add(this.checkDefaultConnectionString);
			this.layoutControlContent.Controls.Add(this.textConnectionName);
			this.layoutControlContent.Controls.Add(this.checkSaveConnection);
			this.layoutControlContent.Controls.Add(this.memoConnectionString);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(994, 285, 714, 489);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 0;
			this.checkCustomConnectionString.EditValue = true;
			this.checkCustomConnectionString.Location = new System.Drawing.Point(45, 40);
			this.checkCustomConnectionString.Name = "checkCustomConnectionString";
			this.checkCustomConnectionString.Properties.AutoWidth = true;
			this.checkCustomConnectionString.Properties.Caption = "Specify a custom connection string";
			this.checkCustomConnectionString.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.checkCustomConnectionString.Properties.RadioGroupIndex = 0;
			this.checkCustomConnectionString.Size = new System.Drawing.Size(188, 19);
			this.checkCustomConnectionString.StyleController = this.layoutControlContent;
			this.checkCustomConnectionString.TabIndex = 6;
			this.checkDefaultConnectionString.Location = new System.Drawing.Point(45, 17);
			this.checkDefaultConnectionString.Name = "checkDefaultConnectionString";
			this.checkDefaultConnectionString.Properties.AutoWidth = true;
			this.checkDefaultConnectionString.Properties.Caption = "Use default connection string";
			this.checkDefaultConnectionString.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.checkDefaultConnectionString.Properties.RadioGroupIndex = 0;
			this.checkDefaultConnectionString.Size = new System.Drawing.Size(162, 19);
			this.checkDefaultConnectionString.StyleController = this.layoutControlContent;
			this.checkDefaultConnectionString.TabIndex = 5;
			this.checkDefaultConnectionString.TabStop = false;
			this.checkDefaultConnectionString.CheckedChanged += new System.EventHandler(this.ceDefaultConnectionString_CheckedChanged);
			this.textConnectionName.Location = new System.Drawing.Point(46, 285);
			this.textConnectionName.Name = "textConnectionName";
			this.textConnectionName.Size = new System.Drawing.Size(514, 20);
			this.textConnectionName.StyleController = this.layoutControlContent;
			this.textConnectionName.TabIndex = 4;
			this.textConnectionName.EditValueChanged += new System.EventHandler(this.textEditConnectionName_EditValueChanged);
			this.checkSaveConnection.EditValue = true;
			this.checkSaveConnection.Location = new System.Drawing.Point(46, 262);
			this.checkSaveConnection.Name = "checkSaveConnection";
			this.checkSaveConnection.Properties.Caption = "Save the connection string to config file as:";
			this.checkSaveConnection.Size = new System.Drawing.Size(514, 19);
			this.checkSaveConnection.StyleController = this.layoutControlContent;
			this.checkSaveConnection.TabIndex = 2;
			this.checkSaveConnection.CheckedChanged += new System.EventHandler(this.checkEditSaveConnection_CheckedChanged);
			this.memoConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.memoConnectionString.Location = new System.Drawing.Point(46, 79);
			this.memoConnectionString.Name = "memoConnectionString";
			this.memoConnectionString.Size = new System.Drawing.Size(514, 179);
			this.memoConnectionString.StyleController = this.layoutControlContent;
			this.memoConnectionString.TabIndex = 3;
			this.memoConnectionString.EditValueChanged += new System.EventHandler(this.memoEditConnectionString_EditValueChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemConnectionString,
			this.layoutItemConnectionName,
			this.layoutItemSaveConnection,
			this.layoutItemCustomConString,
			this.layoutItemDefaultConString});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "Root";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(44, 44, 15, 30);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemConnectionString.Control = this.memoConnectionString;
			this.layoutItemConnectionString.CustomizationFormText = "Connection string: ";
			this.layoutItemConnectionString.Location = new System.Drawing.Point(0, 46);
			this.layoutItemConnectionString.Name = "layoutItemConnectionString";
			this.layoutItemConnectionString.Size = new System.Drawing.Size(518, 199);
			this.layoutItemConnectionString.Text = "Connection string: ";
			this.layoutItemConnectionString.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutItemConnectionString.TextSize = new System.Drawing.Size(91, 13);
			this.layoutItemConnectionName.Control = this.textConnectionName;
			this.layoutItemConnectionName.Location = new System.Drawing.Point(0, 268);
			this.layoutItemConnectionName.Name = "layoutItemConnectionName";
			this.layoutItemConnectionName.Size = new System.Drawing.Size(518, 24);
			this.layoutItemConnectionName.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionName.TextVisible = false;
			this.layoutItemSaveConnection.Control = this.checkSaveConnection;
			this.layoutItemSaveConnection.Location = new System.Drawing.Point(0, 245);
			this.layoutItemSaveConnection.Name = "layoutItemSaveConnection";
			this.layoutItemSaveConnection.Size = new System.Drawing.Size(518, 23);
			this.layoutItemSaveConnection.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSaveConnection.TextVisible = false;
			this.layoutItemCustomConString.Control = this.checkCustomConnectionString;
			this.layoutItemCustomConString.Location = new System.Drawing.Point(0, 23);
			this.layoutItemCustomConString.Name = "layoutItemCustomConString";
			this.layoutItemCustomConString.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 2, 2, 2);
			this.layoutItemCustomConString.Size = new System.Drawing.Size(518, 23);
			this.layoutItemCustomConString.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCustomConString.TextVisible = false;
			this.layoutItemDefaultConString.Control = this.checkDefaultConnectionString;
			this.layoutItemDefaultConString.Location = new System.Drawing.Point(0, 0);
			this.layoutItemDefaultConString.Name = "layoutItemDefaultConString";
			this.layoutItemDefaultConString.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 2, 2, 2);
			this.layoutItemDefaultConString.Size = new System.Drawing.Size(518, 23);
			this.layoutItemDefaultConString.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemDefaultConString.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(768, 162, 768, 162);
			this.Name = "ConfigureEFConnectionStringPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.checkCustomConnectionString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkDefaultConnectionString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textConnectionName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSaveConnection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.memoConnectionString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSaveConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCustomConString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultConString)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraEditors.MemoEdit memoConnectionString;
		protected XtraLayout.LayoutControlItem layoutItemConnectionString;
		protected XtraEditors.CheckEdit checkSaveConnection;
		protected XtraLayout.LayoutControlItem layoutItemSaveConnection;
		protected XtraEditors.TextEdit textConnectionName;
		protected XtraLayout.LayoutControlItem layoutItemConnectionName;
		protected XtraEditors.CheckEdit checkCustomConnectionString;
		protected XtraEditors.CheckEdit checkDefaultConnectionString;
		protected XtraLayout.LayoutControlItem layoutItemCustomConString;
		protected XtraLayout.LayoutControlItem layoutItemDefaultConString;
	}
}
