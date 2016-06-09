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

namespace DevExpress.DataAccess.UI.Native {
	partial class ParametersGridFormBase {
		System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		void InitializeComponent() {
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.separatorBottom = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
			this.parametersGrid = new DevExpress.DataAccess.UI.Native.ParametersGrid.ParametersGrid();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemParametersGrid = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemPreviewButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutItemOkButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparatorBottom = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemAddButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemRemoveButton = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOkButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancelButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).BeginInit();
			this.SuspendLayout();
			this.btnRemove.Location = new System.Drawing.Point(188, 352);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(83, 22);
			this.btnRemove.StyleController = this.layoutControl1;
			this.btnRemove.TabIndex = 5;
			this.btnRemove.Text = "&Remove";
			this.layoutControl1.Controls.Add(this.separatorBottom);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnRemove);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.btnAdd);
			this.layoutControl1.Controls.Add(this.btnPreview);
			this.layoutControl1.Controls.Add(this.parametersGrid);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2059, 164, 897, 622);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(594, 376);
			this.layoutControl1.TabIndex = 1;
			this.separatorBottom.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.separatorBottom.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.separatorBottom.LineVisible = true;
			this.separatorBottom.Location = new System.Drawing.Point(0, 339);
			this.separatorBottom.Margin = new System.Windows.Forms.Padding(0);
			this.separatorBottom.Name = "separatorBottom";
			this.separatorBottom.Padding = new System.Windows.Forms.Padding(0, 0, 0, 13);
			this.separatorBottom.Size = new System.Drawing.Size(594, 13);
			this.separatorBottom.StyleController = this.layoutControl1;
			this.separatorBottom.TabIndex = 6;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(499, 352);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(83, 22);
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "&Cancel";
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(410, 352);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(83, 22);
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "&OK";
			this.btnAdd.Location = new System.Drawing.Point(99, 352);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(83, 22);
			this.btnAdd.StyleController = this.layoutControl1;
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "&Add";
			this.btnPreview.Location = new System.Drawing.Point(12, 352);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(81, 22);
			this.btnPreview.StyleController = this.layoutControl1;
			this.btnPreview.TabIndex = 3;
			this.btnPreview.Text = "&Preview...";
			this.parametersGrid.Location = new System.Drawing.Point(0, 0);
			this.parametersGrid.Margin = new System.Windows.Forms.Padding(0);
			this.parametersGrid.Name = "parametersGrid";
			this.parametersGrid.Padding = new System.Windows.Forms.Padding(2);
			this.parametersGrid.Size = new System.Drawing.Size(594, 339);
			this.parametersGrid.TabIndex = 2;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemParametersGrid,
			this.layoutItemPreviewButton,
			this.emptySpaceItem4,
			this.layoutItemOkButton,
			this.layoutItemCancelButton,
			this.layoutItemSeparatorBottom,
			this.layoutItemAddButton,
			this.layoutItemRemoveButton});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(594, 376);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutItemParametersGrid.Control = this.parametersGrid;
			this.layoutItemParametersGrid.Location = new System.Drawing.Point(0, 0);
			this.layoutItemParametersGrid.Name = "layoutItemParametersGrid";
			this.layoutItemParametersGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemParametersGrid.Size = new System.Drawing.Size(594, 339);
			this.layoutItemParametersGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemParametersGrid.TextVisible = false;
			this.layoutItemPreviewButton.Control = this.btnPreview;
			this.layoutItemPreviewButton.Location = new System.Drawing.Point(0, 352);
			this.layoutItemPreviewButton.Name = "layoutItemPreviewButton";
			this.layoutItemPreviewButton.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 0, 0, 2);
			this.layoutItemPreviewButton.Size = new System.Drawing.Size(93, 24);
			this.layoutItemPreviewButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreviewButton.TextVisible = false;
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(271, 352);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem4.Size = new System.Drawing.Size(139, 24);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemOkButton.Control = this.btnOk;
			this.layoutItemOkButton.Location = new System.Drawing.Point(410, 352);
			this.layoutItemOkButton.Name = "layoutItemOkButton";
			this.layoutItemOkButton.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 6, 0, 2);
			this.layoutItemOkButton.Size = new System.Drawing.Size(89, 24);
			this.layoutItemOkButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemOkButton.TextVisible = false;
			this.layoutItemCancelButton.Control = this.btnCancel;
			this.layoutItemCancelButton.Location = new System.Drawing.Point(499, 352);
			this.layoutItemCancelButton.Name = "layoutItemCancelButton";
			this.layoutItemCancelButton.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 12, 0, 2);
			this.layoutItemCancelButton.Size = new System.Drawing.Size(95, 24);
			this.layoutItemCancelButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCancelButton.TextVisible = false;
			this.layoutItemSeparatorBottom.Control = this.separatorBottom;
			this.layoutItemSeparatorBottom.Location = new System.Drawing.Point(0, 339);
			this.layoutItemSeparatorBottom.MinSize = new System.Drawing.Size(1, 13);
			this.layoutItemSeparatorBottom.Name = "layoutItemSeparatorBottom";
			this.layoutItemSeparatorBottom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemSeparatorBottom.Size = new System.Drawing.Size(594, 13);
			this.layoutItemSeparatorBottom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemSeparatorBottom.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparatorBottom.TextVisible = false;
			this.layoutItemAddButton.Control = this.btnAdd;
			this.layoutItemAddButton.Location = new System.Drawing.Point(93, 352);
			this.layoutItemAddButton.Name = "layoutItemAddButton";
			this.layoutItemAddButton.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 2);
			this.layoutItemAddButton.Size = new System.Drawing.Size(89, 24);
			this.layoutItemAddButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAddButton.TextVisible = false;
			this.layoutItemRemoveButton.Control = this.btnRemove;
			this.layoutItemRemoveButton.Location = new System.Drawing.Point(182, 352);
			this.layoutItemRemoveButton.Name = "layoutItemRemoveButton";
			this.layoutItemRemoveButton.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 2);
			this.layoutItemRemoveButton.Size = new System.Drawing.Size(89, 24);
			this.layoutItemRemoveButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemRemoveButton.TextVisible = false;
			this.AcceptButton = this.btnCancel;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 386);
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 600);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(610, 424);
			this.Name = "ParametersGridFormBase";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Query Parameters";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOkButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancelButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected ParametersGrid.ParametersGrid parametersGrid;
		protected XtraEditors.SimpleButton btnCancel;
		protected XtraEditors.SimpleButton btnOk;
		protected XtraEditors.SimpleButton btnRemove;
		protected XtraEditors.SimpleButton btnAdd;
		protected XtraEditors.SimpleButton btnPreview;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutItemParametersGrid;
		private XtraLayout.LayoutControlItem layoutItemPreviewButton;
		private XtraLayout.LayoutControlItem layoutItemAddButton;
		private XtraLayout.EmptySpaceItem emptySpaceItem4;
		private XtraLayout.LayoutControlItem layoutItemOkButton;
		private XtraLayout.LayoutControlItem layoutItemCancelButton;
		private XtraEditors.LabelControl separatorBottom;
		private XtraLayout.LayoutControlItem layoutItemSeparatorBottom;
		private XtraLayout.LayoutControlItem layoutItemRemoveButton;
	}
}
