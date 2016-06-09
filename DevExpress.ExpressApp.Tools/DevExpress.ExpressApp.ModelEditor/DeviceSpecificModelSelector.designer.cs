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

namespace DevExpress.ExpressApp.ModelEditor {
	partial class DeviceSpecificModelSelector {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.loadModelButton = new DevExpress.XtraEditors.SimpleButton();
			this.printDocument1 = new System.Drawing.Printing.PrintDocument();
			this.bottomPanel = new DevExpress.XtraLayout.LayoutControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
			((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
			this.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(239, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Choose the XAFML file to open:";
			this.loadModelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.loadModelButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.loadModelButton.Location = new System.Drawing.Point(297, 122);
			this.loadModelButton.Name = "loadModelButton";
			this.loadModelButton.Size = new System.Drawing.Size(75, 23);
			this.loadModelButton.TabIndex = 7;
			this.loadModelButton.Text = "Open";
			this.loadModelButton.Click += new System.EventHandler(this.loadModelButton_Click);
			this.bottomPanel.AllowCustomization = false;
			this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.bottomPanel.Location = new System.Drawing.Point(0, 100);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.OptionsCustomizationForm.ShowLoadButton = false;
			this.bottomPanel.OptionsCustomizationForm.ShowSaveButton = false;
			this.bottomPanel.OptionsView.AllowItemSkinning = false;
			this.bottomPanel.OptionsView.EnableIndentsInGroupsWithoutBorders = true;
			this.bottomPanel.OptionsView.UseSkinIndents = false;
			this.bottomPanel.Root = this.layoutControlGroup1;
			this.bottomPanel.Size = new System.Drawing.Size(380, 10);
			this.bottomPanel.TabIndex = 8;
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleSeparator1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 5;
			this.layoutControlGroup1.Size = new System.Drawing.Size(380, 10);
			this.layoutControlGroup1.TextVisible = false;
			this.simpleSeparator1.AllowHotTrack = false;
			this.simpleSeparator1.CustomizationFormText = "simpleSeparator1";
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 0);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Size = new System.Drawing.Size(380, 10);
			this.radioGroup1.Location = new System.Drawing.Point(15, 22);
			this.radioGroup1.Name = "radioGroup1";
			this.radioGroup1.Size = new System.Drawing.Size(357, 72);
			this.radioGroup1.TabIndex = 9;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 157);
			this.Controls.Add(this.radioGroup1);
			this.Controls.Add(this.loadModelButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bottomPanel);
			this.MaximumSize = new System.Drawing.Size(1500, 196);
			this.MinimumSize = new System.Drawing.Size(400, 196);
			this.Name = "OpenModelForm";
			this.Text = "Open Model";
			((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.SimpleButton loadModelButton;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private DevExpress.XtraLayout.LayoutControl bottomPanel;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
		private XtraEditors.RadioGroup radioGroup1;
	}
}
