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

namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	partial class ChooseEFStoredProcedureForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.listBoxControlMain = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).BeginInit();
			this.panelContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControlMain)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.Location = new System.Drawing.Point(232, 312);
			this.btnCancel.Size = new System.Drawing.Size(90, 22);
			this.btnOK.Location = new System.Drawing.Point(137, 312);
			this.btnOK.Size = new System.Drawing.Size(89, 22);
			this.panelContent.Controls.Add(this.listBoxControlMain);
			this.panelContent.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.panelContent.Size = new System.Drawing.Size(329, 306);
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Size = new System.Drawing.Size(333, 345);
			this.layoutControlMain.Controls.SetChildIndex(this.btnOK, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.btnCancel, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelContent, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelControlAdditionalButtons, 0);
			this.layoutControlGroupMain.Size = new System.Drawing.Size(333, 345);
			this.layoutItemContentPanel.Size = new System.Drawing.Size(333, 310);
			this.layoutItemButtonOk.Size = new System.Drawing.Size(104, 35);
			this.layoutItemButtonCancel.Location = new System.Drawing.Point(104, 0);
			this.layoutItemButtonCancel.Size = new System.Drawing.Size(104, 35);
			this.layoutControlGroupOkCancel.Location = new System.Drawing.Point(125, 0);
			this.layoutControlGroupOkCancel.Size = new System.Drawing.Size(208, 35);
			this.panelControlAdditionalButtons.Location = new System.Drawing.Point(2, 312);
			this.panelControlAdditionalButtons.Size = new System.Drawing.Size(121, 31);
			this.layoutControlItemAdditionalButtons.Size = new System.Drawing.Size(125, 35);
			this.listBoxControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxControlMain.Location = new System.Drawing.Point(0, 0);
			this.listBoxControlMain.Name = "listBoxControlMain";
			this.listBoxControlMain.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxControlMain.Size = new System.Drawing.Size(329, 296);
			this.listBoxControlMain.TabIndex = 5;
			this.listBoxControlMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxControl1_MouseDoubleClick);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(333, 345);
			this.MaximumSize = new System.Drawing.Size(800, 600);
			this.Name = "ChooseEFStoredProcedureForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select stored procedures to add";
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).EndInit();
			this.panelContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControlMain)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.ListBoxControl listBoxControlMain;
	}
}
