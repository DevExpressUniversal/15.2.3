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
	partial class SummaryEditorsContainer {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutEditors = new DevExpress.XtraLayout.LayoutControl();
			this.mainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.summaryOptionsHeader1 = new DevExpress.XtraReports.Wizards3.Views.SummaryOptionsHeader();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.editorsContainerGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			((System.ComponentModel.ISupportInitialize)(this.layoutEditors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mainGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editorsContainerGroup)).BeginInit();
			this.SuspendLayout();
			this.layoutEditors.AllowCustomization = false;
			this.layoutEditors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutEditors.Location = new System.Drawing.Point(0, 36);
			this.layoutEditors.Margin = new System.Windows.Forms.Padding(0);
			this.layoutEditors.Name = "layoutEditors";
			this.layoutEditors.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(703, 229, 450, 350);
			this.layoutEditors.Padding = new System.Windows.Forms.Padding(30);
			this.layoutEditors.Root = this.mainGroup;
			this.layoutEditors.Size = new System.Drawing.Size(399, 82);
			this.layoutEditors.TabIndex = 1;
			this.layoutEditors.Text = "layoutControl1";
			this.mainGroup.CustomizationFormText = "editorsContainer";
			this.mainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.mainGroup.GroupBordersVisible = false;
			this.mainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem1,
			this.editorsContainerGroup});
			this.mainGroup.Location = new System.Drawing.Point(0, 0);
			this.mainGroup.Name = "mainGroup";
			this.mainGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.mainGroup.Size = new System.Drawing.Size(399, 82);
			this.mainGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 6);
			this.mainGroup.Text = "mainGroup";
			this.mainGroup.TextVisible = false;
			this.summaryOptionsHeader1.BackColor = System.Drawing.Color.Transparent;
			this.summaryOptionsHeader1.Dock = System.Windows.Forms.DockStyle.Top;
			this.summaryOptionsHeader1.Location = new System.Drawing.Point(0, 0);
			this.summaryOptionsHeader1.Margin = new System.Windows.Forms.Padding(0);
			this.summaryOptionsHeader1.MaximumSize = new System.Drawing.Size(0, 36);
			this.summaryOptionsHeader1.MinimumSize = new System.Drawing.Size(0, 36);
			this.summaryOptionsHeader1.Name = "summaryOptionsHeader1";
			this.summaryOptionsHeader1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.summaryOptionsHeader1.Size = new System.Drawing.Size(399, 36);
			this.summaryOptionsHeader1.TabIndex = 4;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 75);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 1);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(399, 1);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.editorsContainerGroup.CustomizationFormText = "editorsContainerGroup";
			this.editorsContainerGroup.GroupBordersVisible = false;
			this.editorsContainerGroup.Location = new System.Drawing.Point(0, 0);
			this.editorsContainerGroup.Name = "editorsContainerGroup";
			this.editorsContainerGroup.Size = new System.Drawing.Size(399, 75);
			this.editorsContainerGroup.Text = "editorsContainerGroup";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.layoutEditors);
			this.Controls.Add(this.summaryOptionsHeader1);
			this.Name = "SummaryEditorsContainer";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
			this.Size = new System.Drawing.Size(399, 124);
			((System.ComponentModel.ISupportInitialize)(this.layoutEditors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mainGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editorsContainerGroup)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutEditors;
		private XtraLayout.LayoutControlGroup mainGroup;
		private SummaryOptionsHeader summaryOptionsHeader1;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.LayoutControlGroup editorsContainerGroup;
	}
}
