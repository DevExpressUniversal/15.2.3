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
	partial class AverageControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcAverage = new DevExpress.XtraLayout.LayoutControl();
			this.cmbAverage = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgAverage = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciAverage = new DevExpress.XtraLayout.LayoutControlItem();
			this.sliInfo = new DevExpress.XtraLayout.SimpleLabelItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.lcAverage)).BeginInit();
			this.lcAverage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbAverage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAverage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAverage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.lcAverage.AllowCustomization = false;
			this.lcAverage.Controls.Add(this.cmbAverage);
			this.lcAverage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcAverage.Location = new System.Drawing.Point(0, 0);
			this.lcAverage.Name = "lcAverage";
			this.lcAverage.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(841, 155, 913, 787);
			this.lcAverage.Root = this.lcgAverage;
			this.lcAverage.Size = new System.Drawing.Size(470, 41);
			this.lcAverage.TabIndex = 0;
			this.lcAverage.Text = "layoutControl1";
			this.cmbAverage.EditValue = "";
			this.cmbAverage.Location = new System.Drawing.Point(8, 19);
			this.cmbAverage.Name = "cmbAverage";
			this.cmbAverage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbAverage.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbAverage.Size = new System.Drawing.Size(96, 20);
			this.cmbAverage.StyleController = this.lcAverage;
			this.cmbAverage.TabIndex = 4;
			this.lcgAverage.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgAverage.GroupBordersVisible = false;
			this.lcgAverage.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciAverage,
			this.sliInfo,
			this.emptySpaceItem2});
			this.lcgAverage.Location = new System.Drawing.Point(0, 0);
			this.lcgAverage.Name = "Root";
			this.lcgAverage.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
			this.lcgAverage.Size = new System.Drawing.Size(470, 41);
			this.lcgAverage.TextVisible = false;
			this.lciAverage.Control = this.cmbAverage;
			this.lciAverage.CustomizationFormText = "the average of the column\'s cell values";
			this.lciAverage.Location = new System.Drawing.Point(0, 17);
			this.lciAverage.Name = "lciAverage";
			this.lciAverage.Size = new System.Drawing.Size(272, 24);
			this.lciAverage.Text = "the average for the selected range";
			this.lciAverage.TextLocation = DevExpress.Utils.Locations.Right;
			this.lciAverage.TextSize = new System.Drawing.Size(169, 13);
			this.sliInfo.AllowHotTrack = false;
			this.sliInfo.Location = new System.Drawing.Point(0, 0);
			this.sliInfo.Name = "sliInfo";
			this.sliInfo.Size = new System.Drawing.Size(464, 17);
			this.sliInfo.Text = "Format values that are:";
			this.sliInfo.TextSize = new System.Drawing.Size(169, 13);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(272, 17);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(192, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcAverage);
			this.Name = "AverageControl";
			this.Size = new System.Drawing.Size(470, 41);
			((System.ComponentModel.ISupportInitialize)(this.lcAverage)).EndInit();
			this.lcAverage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbAverage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAverage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAverage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcAverage;
		private DevExpress.XtraLayout.LayoutControlGroup lcgAverage;
		private DevExpress.XtraEditors.ComboBoxEdit cmbAverage;
		private DevExpress.XtraLayout.LayoutControlItem lciAverage;
		private DevExpress.XtraLayout.SimpleLabelItem sliInfo;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
	}
}
