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

namespace DevExpress.XtraGrid.Controls {
	partial class FindControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btClear = new DevExpress.XtraEditors.SimpleButton();
			this.teFind = new DevExpress.XtraEditors.MRUEdit();
			this.btFind = new DevExpress.XtraEditors.SimpleButton();
			this.btClose = new DevExpress.XtraEditors.CloseButton();
			this.lcGroupMain = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciCloseButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciFind = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciFindButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciClearButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.autoFilterTimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCloseButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFindButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClearButton)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.btClear);
			this.layoutControl1.Controls.Add(this.teFind);
			this.layoutControl1.Controls.Add(this.btFind);
			this.layoutControl1.Controls.Add(this.btClose);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsView.UseSkinIndents = false;
			this.layoutControl1.Root = this.lcGroupMain;
			this.layoutControl1.Size = new System.Drawing.Size(492, 46);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "lc";
			this.btClear.AutoWidthInLayoutControl = true;
			this.btClear.Location = new System.Drawing.Point(420, 12);
			this.btClear.MinimumSize = new System.Drawing.Size(60, 0);
			this.btClear.Name = "btClear";
			this.btClear.Size = new System.Drawing.Size(60, 22);
			this.btClear.StyleController = this.layoutControl1;
			this.btClear.TabIndex = 7;
			this.btClear.Text = "Clear";
			this.btClear.Click += new System.EventHandler(this.btClear_Click);
			this.teFind.CausesValidation = false;
			this.teFind.Location = new System.Drawing.Point(32, 13);
			this.teFind.MaximumSize = new System.Drawing.Size(350, 0);
			this.teFind.Name = "teFind";
			this.teFind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.teFind.Properties.MaxItemCount = 10;
			this.teFind.Properties.ValidateOnEnterKey = false;
			this.teFind.Size = new System.Drawing.Size(320, 20);
			this.teFind.StyleController = this.layoutControl1;
			this.teFind.TabIndex = 5;
			this.teFind.EditValueChanged += new System.EventHandler(this.teFind_EditValueChanged);
			this.teFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teFind_KeyDown);
			this.btFind.AutoWidthInLayoutControl = true;
			this.btFind.Location = new System.Drawing.Point(356, 12);
			this.btFind.MinimumSize = new System.Drawing.Size(60, 0);
			this.btFind.Name = "btFind";
			this.btFind.Size = new System.Drawing.Size(60, 22);
			this.btFind.StyleController = this.layoutControl1;
			this.btFind.TabIndex = 6;
			this.btFind.Text = "Find";
			this.btFind.Click += new System.EventHandler(this.btFind_Click);
			this.btClose.AllowFocus = false;
			this.btClose.AutoWidthInLayoutControl = true;
			this.btClose.Location = new System.Drawing.Point(12, 15);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(16, 16);
			this.btClose.StyleController = this.layoutControl1;
			this.btClose.TabIndex = 4;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			this.lcGroupMain.CustomizationFormText = "lcGroupMain";
			this.lcGroupMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcGroupMain.GroupBordersVisible = false;
			this.lcGroupMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciCloseButton,
			this.lciFind,
			this.lciFindButton,
			this.lciClearButton});
			this.lcGroupMain.Location = new System.Drawing.Point(0, 0);
			this.lcGroupMain.Name = "lcGroupMain";
			this.lcGroupMain.OptionsItemText.TextToControlDistance = 5;
			this.lcGroupMain.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
			this.lcGroupMain.Size = new System.Drawing.Size(492, 46);
			this.lcGroupMain.TextVisible = false;
			this.lciCloseButton.Control = this.btClose;
			this.lciCloseButton.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.lciCloseButton.CustomizationFormText = "lciCloseButton";
			this.lciCloseButton.FillControlToClientArea = false;
			this.lciCloseButton.Location = new System.Drawing.Point(0, 0);
			this.lciCloseButton.Name = "lciCloseButton";
			this.lciCloseButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lciCloseButton.Size = new System.Drawing.Size(20, 26);
			this.lciCloseButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciCloseButton.TextVisible = false;
			this.lciCloseButton.TrimClientAreaToControl = false;
			this.lciFind.Control = this.teFind;
			this.lciFind.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.lciFind.CustomizationFormText = "lciFind";
			this.lciFind.FillControlToClientArea = false;
			this.lciFind.Location = new System.Drawing.Point(20, 0);
			this.lciFind.Name = "lciFind";
			this.lciFind.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lciFind.Size = new System.Drawing.Size(324, 26);
			this.lciFind.TextSize = new System.Drawing.Size(0, 0);
			this.lciFind.TextVisible = false;
			this.lciFind.TrimClientAreaToControl = false;
			this.lciFindButton.Control = this.btFind;
			this.lciFindButton.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.lciFindButton.CustomizationFormText = "lciFindButton";
			this.lciFindButton.FillControlToClientArea = false;
			this.lciFindButton.Location = new System.Drawing.Point(344, 0);
			this.lciFindButton.Name = "lciFindButton";
			this.lciFindButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lciFindButton.Size = new System.Drawing.Size(64, 26);
			this.lciFindButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciFindButton.TextVisible = false;
			this.lciFindButton.TrimClientAreaToControl = false;
			this.lciClearButton.Control = this.btClear;
			this.lciClearButton.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.lciClearButton.CustomizationFormText = "lciClearButton";
			this.lciClearButton.FillControlToClientArea = false;
			this.lciClearButton.Location = new System.Drawing.Point(408, 0);
			this.lciClearButton.Name = "lciClearButton";
			this.lciClearButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lciClearButton.Size = new System.Drawing.Size(64, 26);
			this.lciClearButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciClearButton.TextVisible = false;
			this.lciClearButton.TrimClientAreaToControl = false;
			this.autoFilterTimer.Interval = 1500;
			this.autoFilterTimer.Tick += new System.EventHandler(this.autoFilterTimer_Tick);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "FindControl";
			this.Size = new System.Drawing.Size(492, 46);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCloseButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFindButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClearButton)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btClear;
		private System.Windows.Forms.Timer autoFilterTimer;
		protected internal DevExpress.XtraLayout.LayoutControlItem lciCloseButton;
		protected DevExpress.XtraEditors.CloseButton btClose;
		protected DevExpress.XtraEditors.MRUEdit teFind;
		protected DevExpress.XtraLayout.LayoutControlItem lciFind;
		protected DevExpress.XtraLayout.LayoutControl layoutControl1;
		protected DevExpress.XtraEditors.SimpleButton btFind;
		protected internal DevExpress.XtraLayout.LayoutControlItem lciFindButton;
		protected internal DevExpress.XtraLayout.LayoutControlItem lciClearButton;
		protected DevExpress.XtraLayout.LayoutControlGroup lcGroupMain;
	}
}
