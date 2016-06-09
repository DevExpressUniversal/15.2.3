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

namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	partial class JoinEditorView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.cmbJoinType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.joinEditorControl = new DevExpress.DataAccess.UI.Native.Sql.QueryBuilder.JoinEditorControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciEditorControl = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciJoinType = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbJoinType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditorControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciJoinType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.cmbJoinType);
			this.layoutControl1.Controls.Add(this.joinEditorControl);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(710, 182, 450, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(574, 259);
			this.layoutControl1.TabIndex = 0;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(470, 225);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(92, 22);
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOk.Location = new System.Drawing.Point(373, 225);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(91, 22);
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.cmbJoinType.EditValue = "";
			this.cmbJoinType.Location = new System.Drawing.Point(63, 12);
			this.cmbJoinType.Name = "cmbJoinType";
			this.cmbJoinType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbJoinType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbJoinType.Size = new System.Drawing.Size(129, 20);
			this.cmbJoinType.StyleController = this.layoutControl1;
			this.cmbJoinType.TabIndex = 5;
			this.joinEditorControl.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
			this.joinEditorControl.Appearance.Options.UseBackColor = true;
			this.joinEditorControl.Location = new System.Drawing.Point(12, 44);
			this.joinEditorControl.Name = "joinEditorControl";
			this.joinEditorControl.Padding = new System.Windows.Forms.Padding(1);
			this.joinEditorControl.Size = new System.Drawing.Size(550, 169);
			this.joinEditorControl.TabIndex = 4;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciEditorControl,
			this.lciJoinType,
			this.layoutControlItem1,
			this.emptySpaceItem1,
			this.layoutControlItem2,
			this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(574, 259);
			this.layoutControlGroup1.TextVisible = false;
			this.lciEditorControl.Control = this.joinEditorControl;
			this.lciEditorControl.Location = new System.Drawing.Point(0, 32);
			this.lciEditorControl.Name = "lciEditorControl";
			this.lciEditorControl.Size = new System.Drawing.Size(554, 173);
			this.lciEditorControl.TextSize = new System.Drawing.Size(0, 0);
			this.lciEditorControl.TextVisible = false;
			this.lciJoinType.Control = this.cmbJoinType;
			this.lciJoinType.Location = new System.Drawing.Point(0, 0);
			this.lciJoinType.Name = "lciJoinType";
			this.lciJoinType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
			this.lciJoinType.Size = new System.Drawing.Size(184, 32);
			this.lciJoinType.Text = "Join type:";
			this.lciJoinType.TextSize = new System.Drawing.Size(48, 13);
			this.layoutControlItem1.Control = this.btnOk;
			this.layoutControlItem1.Location = new System.Drawing.Point(361, 205);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 10, 2);
			this.layoutControlItem1.Size = new System.Drawing.Size(96, 34);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 205);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(361, 34);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.btnCancel;
			this.layoutControlItem2.Location = new System.Drawing.Point(457, 205);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 10, 2);
			this.layoutControlItem2.Size = new System.Drawing.Size(97, 34);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(184, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(370, 32);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(574, 259);
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(239, 176);
			this.Name = "JoinEditorView";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Join Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JoinEditorView_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbJoinType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditorControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciJoinType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl1;
		private XtraEditors.ComboBoxEdit cmbJoinType;
		private JoinEditorControl joinEditorControl;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem lciEditorControl;
		private XtraLayout.LayoutControlItem lciJoinType;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
	}
}
