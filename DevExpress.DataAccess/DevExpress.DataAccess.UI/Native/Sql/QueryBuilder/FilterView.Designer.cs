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

using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	partial class FilterView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.layoutMain = new DevExpress.XtraLayout.LayoutControl();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.buttonOk = new DevExpress.XtraEditors.SimpleButton();
			this.layoutGroupForm = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemOk = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceBottom = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutItemFilter = new DevExpress.XtraLayout.LayoutControlItem();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			this.SuspendLayout();
			this.filterControl.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.filterControl.Location = new System.Drawing.Point(12, 12);
			this.filterControl.Name = "filterControl";
			this.filterControl.ShowGroupCommandsIcon = true;
			this.filterControl.ShowIsNullOperatorsForStrings = true;
			this.filterControl.ShowOperandTypeIcon = true;
			this.filterControl.Size = new System.Drawing.Size(550, 201);
			this.filterControl.TabIndex = 4;
			this.layoutMain.AllowCustomization = false;
			this.layoutMain.Controls.Add(this.filterControl);
			this.layoutMain.Controls.Add(this.buttonCancel);
			this.layoutMain.Controls.Add(this.buttonOk);
			this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutMain.Location = new System.Drawing.Point(0, 0);
			this.layoutMain.Name = "layoutMain";
			this.layoutMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(815, 209, 655, 350);
			this.layoutMain.Root = this.layoutGroupForm;
			this.layoutMain.Size = new System.Drawing.Size(574, 259);
			this.layoutMain.TabIndex = 0;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(487, 225);
			this.buttonCancel.MaximumSize = new System.Drawing.Size(100, 0);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 22);
			this.buttonCancel.StyleController = this.layoutMain;
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.buttonOk.Location = new System.Drawing.Point(406, 225);
			this.buttonOk.MaximumSize = new System.Drawing.Size(100, 0);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 22);
			this.buttonOk.StyleController = this.layoutMain;
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.btnOk_Click);
			this.layoutGroupForm.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupForm.GroupBordersVisible = false;
			this.layoutGroupForm.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemOk,
			this.layoutItemCancel,
			this.emptySpaceBottom,
			this.layoutItemFilter});
			this.layoutGroupForm.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupForm.Name = "layoutGroupForm";
			this.layoutGroupForm.Size = new System.Drawing.Size(574, 259);
			this.layoutGroupForm.TextVisible = false;
			this.layoutItemOk.Control = this.buttonOk;
			this.layoutItemOk.Location = new System.Drawing.Point(394, 205);
			this.layoutItemOk.Name = "layoutItemOk";
			this.layoutItemOk.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 10, 2);
			this.layoutItemOk.Size = new System.Drawing.Size(80, 34);
			this.layoutItemOk.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemOk.TextVisible = false;
			this.layoutItemCancel.Control = this.buttonCancel;
			this.layoutItemCancel.Location = new System.Drawing.Point(474, 205);
			this.layoutItemCancel.Name = "layoutItemCancel";
			this.layoutItemCancel.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 10, 2);
			this.layoutItemCancel.Size = new System.Drawing.Size(80, 34);
			this.layoutItemCancel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCancel.TextVisible = false;
			this.emptySpaceBottom.AllowHotTrack = false;
			this.emptySpaceBottom.Location = new System.Drawing.Point(0, 205);
			this.emptySpaceBottom.Name = "emptySpaceBottom";
			this.emptySpaceBottom.Size = new System.Drawing.Size(394, 34);
			this.emptySpaceBottom.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemFilter.Control = this.filterControl;
			this.layoutItemFilter.Location = new System.Drawing.Point(0, 0);
			this.layoutItemFilter.Name = "layoutItemFilter";
			this.layoutItemFilter.Size = new System.Drawing.Size(554, 205);
			this.layoutItemFilter.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemFilter.TextVisible = false;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 0;
			this.barManager.ShowScreenTipsInMenus = true;
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(574, 0);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 259);
			this.barDockControlBottom.Size = new System.Drawing.Size(574, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 259);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(574, 0);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 259);
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(574, 259);
			this.Controls.Add(this.layoutMain);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(208, 139);
			this.Name = "FilterView";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Filter Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilterView_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControlGroup layoutGroupForm;
		protected XtraEditors.SimpleButton buttonCancel;
		protected XtraEditors.SimpleButton buttonOk;
		protected XtraLayout.LayoutControlItem layoutItemOk;
		protected XtraLayout.LayoutControlItem layoutItemCancel;
		protected XtraLayout.EmptySpaceItem emptySpaceBottom;
		protected XtraLayout.LayoutControlItem layoutItemFilter;
		protected XtraBars.BarManager barManager;
		protected XtraBars.BarDockControl barDockControlTop;
		protected XtraBars.BarDockControl barDockControlBottom;
		protected XtraBars.BarDockControl barDockControlLeft;
		protected XtraBars.BarDockControl barDockControlRight;
		protected XtraLayout.LayoutControl layoutMain;
		protected FilterControl filterControl;
	}
}
