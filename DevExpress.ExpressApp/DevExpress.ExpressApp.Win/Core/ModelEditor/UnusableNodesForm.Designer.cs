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

namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	partial class UnusableNodesForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.unusableNodesTab = new DevExpress.XtraTab.XtraTabControl();
			this.defaultPage = new DevExpress.XtraTab.XtraTabPage();
			this.defaultLanguageMemoEdit = new DevExpress.XtraEditors.MemoEdit();
			this.closeButton = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.unusableNodesTab)).BeginInit();
			this.unusableNodesTab.SuspendLayout();
			this.defaultPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.defaultLanguageMemoEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.unusableNodesTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.unusableNodesTab.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.unusableNodesTab.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.unusableNodesTab.Location = new System.Drawing.Point(12, 12);
			this.unusableNodesTab.Name = "unusableNodesTab";
			this.unusableNodesTab.SelectedTabPage = this.defaultPage;
			this.unusableNodesTab.Size = new System.Drawing.Size(483, 294);
			this.unusableNodesTab.TabIndex = 0;
			this.unusableNodesTab.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.defaultPage});
			this.defaultPage.Controls.Add(this.defaultLanguageMemoEdit);
			this.defaultPage.Name = "defaultPage";
			this.defaultPage.Size = new System.Drawing.Size(477, 267);
			this.defaultPage.Text = "Default language";
			this.defaultLanguageMemoEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.defaultLanguageMemoEdit.Location = new System.Drawing.Point(0, 0);
			this.defaultLanguageMemoEdit.Name = "defaultLanguageMemoEdit";
			this.defaultLanguageMemoEdit.Properties.ReadOnly = true;
			this.defaultLanguageMemoEdit.Size = new System.Drawing.Size(477, 267);
			this.defaultLanguageMemoEdit.TabIndex = 0;
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(416, 328);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.layoutControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.layoutControl1.Location = new System.Drawing.Point(-2, 317);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(510, 17);
			this.layoutControl1.TabIndex = 7;
			this.layoutControl1.Text = "layoutControl1";
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleSeparator1,
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(510, 15);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.simpleSeparator1.ControlAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.simpleSeparator1.CustomizationFormText = "simpleSeparator1";
			this.simpleSeparator1.ImageAlignment = System.Drawing.ContentAlignment.TopCenter;
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 0);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Size = new System.Drawing.Size(510, 2);
			this.simpleSeparator1.Text = "simpleSeparator1";
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 2);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(510, 13);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(507, 363);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.unusableNodesTab);
			this.Controls.Add(this.layoutControl1);
			this.MinimizeBox = false;
			this.Name = "UnusableNodesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Unusable Nodes";
			((System.ComponentModel.ISupportInitialize)(this.unusableNodesTab)).EndInit();
			this.unusableNodesTab.ResumeLayout(false);
			this.defaultPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.defaultLanguageMemoEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
	  #endregion
		private DevExpress.XtraTab.XtraTabControl unusableNodesTab;
		private DevExpress.XtraTab.XtraTabPage defaultPage;
		private DevExpress.XtraEditors.SimpleButton closeButton;
		private DevExpress.XtraEditors.MemoEdit defaultLanguageMemoEdit;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
	}
}
