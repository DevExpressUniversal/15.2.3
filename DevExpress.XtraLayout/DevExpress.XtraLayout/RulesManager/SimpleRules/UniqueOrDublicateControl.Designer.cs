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

namespace DevExpress.XtraEditors.Frames {
	partial class UniqueOrDuplicateControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {  
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcUniqueOrDuplicate = new DevExpress.XtraLayout.LayoutControl();
			this.cmbUniqueOrDuplicate = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgUniqueOrDuplicate = new DevExpress.XtraLayout.LayoutControlGroup();
			this.sliInfo = new DevExpress.XtraLayout.SimpleLabelItem();
			this.lciUniqueOrDuplicate = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.lcUniqueOrDuplicate)).BeginInit();
			this.lcUniqueOrDuplicate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbUniqueOrDuplicate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUniqueOrDuplicate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUniqueOrDuplicate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.lcUniqueOrDuplicate.AllowCustomization = false;
			this.lcUniqueOrDuplicate.Controls.Add(this.cmbUniqueOrDuplicate);
			this.lcUniqueOrDuplicate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcUniqueOrDuplicate.Location = new System.Drawing.Point(0, 0);
			this.lcUniqueOrDuplicate.Name = "lcUniqueOrDuplicate";
			this.lcUniqueOrDuplicate.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(889, 210, 751, 574);
			this.lcUniqueOrDuplicate.Root = this.lcgUniqueOrDuplicate;
			this.lcUniqueOrDuplicate.Size = new System.Drawing.Size(470, 41);
			this.lcUniqueOrDuplicate.TabIndex = 0;
			this.lcUniqueOrDuplicate.Text = "layoutControl1";
			this.cmbUniqueOrDuplicate.EditValue = "";
			this.cmbUniqueOrDuplicate.Location = new System.Drawing.Point(8, 19);
			this.cmbUniqueOrDuplicate.Name = "cmbUniqueOrDuplicate";
			this.cmbUniqueOrDuplicate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbUniqueOrDuplicate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbUniqueOrDuplicate.Size = new System.Drawing.Size(96, 20);
			this.cmbUniqueOrDuplicate.StyleController = this.lcUniqueOrDuplicate;
			this.cmbUniqueOrDuplicate.TabIndex = 4;
			this.lcgUniqueOrDuplicate.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgUniqueOrDuplicate.GroupBordersVisible = false;
			this.lcgUniqueOrDuplicate.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.sliInfo,
			this.lciUniqueOrDuplicate,
			this.emptySpaceItem1,
			this.emptySpaceItem2});
			this.lcgUniqueOrDuplicate.Location = new System.Drawing.Point(0, 0);
			this.lcgUniqueOrDuplicate.Name = "Root";
			this.lcgUniqueOrDuplicate.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
			this.lcgUniqueOrDuplicate.Size = new System.Drawing.Size(470, 41);
			this.lcgUniqueOrDuplicate.TextVisible = false;
			this.sliInfo.AllowHotTrack = false;
			this.sliInfo.Location = new System.Drawing.Point(0, 0);
			this.sliInfo.Name = "sliInfo";
			this.sliInfo.Size = new System.Drawing.Size(295, 17);
			this.sliInfo.Text = "Format all:";
			this.sliInfo.TextSize = new System.Drawing.Size(135, 13);
			this.lciUniqueOrDuplicate.Control = this.cmbUniqueOrDuplicate;
			this.lciUniqueOrDuplicate.Location = new System.Drawing.Point(0, 17);
			this.lciUniqueOrDuplicate.Name = "lciUniqueOrDuplicate";
			this.lciUniqueOrDuplicate.Size = new System.Drawing.Size(238, 24);
			this.lciUniqueOrDuplicate.Text = "values in the selected range";
			this.lciUniqueOrDuplicate.TextLocation = DevExpress.Utils.Locations.Right;
			this.lciUniqueOrDuplicate.TextSize = new System.Drawing.Size(135, 13);
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(295, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(169, 17);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(238, 17);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(226, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcUniqueOrDuplicate);
			this.Name = "UniqueOrDuplicateControl";
			this.Size = new System.Drawing.Size(470, 41);
			((System.ComponentModel.ISupportInitialize)(this.lcUniqueOrDuplicate)).EndInit();
			this.lcUniqueOrDuplicate.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbUniqueOrDuplicate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUniqueOrDuplicate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUniqueOrDuplicate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcUniqueOrDuplicate;
		private DevExpress.XtraLayout.LayoutControlGroup lcgUniqueOrDuplicate;
		private DevExpress.XtraEditors.ComboBoxEdit cmbUniqueOrDuplicate;
		private DevExpress.XtraLayout.SimpleLabelItem sliInfo;
		private DevExpress.XtraLayout.LayoutControlItem lciUniqueOrDuplicate;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
	}
}
