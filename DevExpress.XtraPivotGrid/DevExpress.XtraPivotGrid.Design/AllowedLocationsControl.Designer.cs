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

namespace DevExpress.XtraPivotGrid.Design {
	partial class AllowedLocationsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.gcPivotGrid = new DevExpress.XtraEditors.GroupControl();
			this.pcDataArea = new DevExpress.XtraEditors.PanelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.pcColumnArea = new DevExpress.XtraEditors.PanelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pcRowArea = new DevExpress.XtraEditors.PanelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.pcFilterArea = new DevExpress.XtraEditors.PanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.gcHiddenFields = new DevExpress.XtraEditors.GroupControl();
			this.pcHiddenFields = new DevExpress.XtraEditors.PanelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.gcPivotGrid)).BeginInit();
			this.gcPivotGrid.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcDataArea)).BeginInit();
			this.pcDataArea.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcColumnArea)).BeginInit();
			this.pcColumnArea.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcRowArea)).BeginInit();
			this.pcRowArea.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcFilterArea)).BeginInit();
			this.pcFilterArea.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcHiddenFields)).BeginInit();
			this.gcHiddenFields.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcHiddenFields)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			this.SuspendLayout();
			this.gcPivotGrid.Controls.Add(this.pcDataArea);
			this.gcPivotGrid.Controls.Add(this.pcColumnArea);
			this.gcPivotGrid.Controls.Add(this.panel1);
			this.gcPivotGrid.Controls.Add(this.pcFilterArea);
			this.gcPivotGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gcPivotGrid.Location = new System.Drawing.Point(2, 2);
			this.gcPivotGrid.Name = "gcPivotGrid";
			this.gcPivotGrid.Size = new System.Drawing.Size(297, 202);
			this.gcPivotGrid.TabIndex = 3;
			this.gcPivotGrid.Text = "Pivot Grid";
			this.pcDataArea.Appearance.Options.UseBackColor = true;
			this.pcDataArea.Controls.Add(this.labelControl4);
			this.pcDataArea.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pcDataArea.Location = new System.Drawing.Point(99, 77);
			this.pcDataArea.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.pcDataArea.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pcDataArea.Name = "pcDataArea";
			this.pcDataArea.Size = new System.Drawing.Size(196, 123);
			this.pcDataArea.TabIndex = 3;
			this.pcDataArea.Click += new System.EventHandler(this.pcDataArea_Click);
			this.labelControl4.Location = new System.Drawing.Point(6, 6);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Size = new System.Drawing.Size(49, 13);
			this.labelControl4.TabIndex = 0;
			this.labelControl4.Text = "Data Area";
			this.pcColumnArea.Appearance.Options.UseBackColor = true;
			this.pcColumnArea.Controls.Add(this.labelControl2);
			this.pcColumnArea.Dock = System.Windows.Forms.DockStyle.Top;
			this.pcColumnArea.Location = new System.Drawing.Point(99, 49);
			this.pcColumnArea.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.pcColumnArea.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pcColumnArea.Name = "pcColumnArea";
			this.pcColumnArea.Size = new System.Drawing.Size(196, 28);
			this.pcColumnArea.TabIndex = 2;
			this.pcColumnArea.Click += new System.EventHandler(this.pcColumnArea_Click);
			this.labelControl2.Location = new System.Drawing.Point(8, 8);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(61, 13);
			this.labelControl2.TabIndex = 1;
			this.labelControl2.Text = "Column Area";
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.pcRowArea);
			this.panel1.Controls.Add(this.panelControl2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(2, 49);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(97, 151);
			this.panel1.TabIndex = 1;
			this.pcRowArea.Appearance.Options.UseBackColor = true;
			this.pcRowArea.Controls.Add(this.labelControl3);
			this.pcRowArea.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pcRowArea.Location = new System.Drawing.Point(0, 28);
			this.pcRowArea.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.pcRowArea.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pcRowArea.Name = "pcRowArea";
			this.pcRowArea.Size = new System.Drawing.Size(97, 123);
			this.pcRowArea.TabIndex = 1;
			this.pcRowArea.Click += new System.EventHandler(this.pcRowArea_Click);
			this.labelControl3.Location = new System.Drawing.Point(6, 6);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(47, 13);
			this.labelControl3.TabIndex = 0;
			this.labelControl3.Text = "Row Area";
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl2.Location = new System.Drawing.Point(0, 0);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Size = new System.Drawing.Size(97, 28);
			this.panelControl2.TabIndex = 0;
			this.pcFilterArea.Appearance.Options.UseBackColor = true;
			this.pcFilterArea.Controls.Add(this.labelControl1);
			this.pcFilterArea.Dock = System.Windows.Forms.DockStyle.Top;
			this.pcFilterArea.Location = new System.Drawing.Point(2, 21);
			this.pcFilterArea.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
			this.pcFilterArea.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pcFilterArea.Name = "pcFilterArea";
			this.pcFilterArea.Size = new System.Drawing.Size(293, 28);
			this.pcFilterArea.TabIndex = 0;
			this.pcFilterArea.Click += new System.EventHandler(this.pcFilterArea_Click);
			this.labelControl1.Location = new System.Drawing.Point(6, 6);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(50, 13);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "Filter Area";
			this.gcHiddenFields.Controls.Add(this.pcHiddenFields);
			this.gcHiddenFields.Dock = System.Windows.Forms.DockStyle.Right;
			this.gcHiddenFields.Location = new System.Drawing.Point(299, 2);
			this.gcHiddenFields.Name = "gcHiddenFields";
			this.gcHiddenFields.Size = new System.Drawing.Size(99, 202);
			this.gcHiddenFields.TabIndex = 2;
			this.gcHiddenFields.Text = "Hidden Fields";
			this.pcHiddenFields.Appearance.Options.UseBackColor = true;
			this.pcHiddenFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pcHiddenFields.Location = new System.Drawing.Point(2, 21);
			this.pcHiddenFields.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			this.pcHiddenFields.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pcHiddenFields.Name = "pcHiddenFields";
			this.pcHiddenFields.Size = new System.Drawing.Size(95, 179);
			this.pcHiddenFields.TabIndex = 0;
			this.pcHiddenFields.Click += new System.EventHandler(this.pcHiddenFields_Click);
			this.panelControl1.Controls.Add(this.btnCancel);
			this.panelControl1.Controls.Add(this.btnOK);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(0, 206);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(400, 44);
			this.panelControl1.TabIndex = 4;
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(304, 11);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 22);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(209, 11);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(85, 22);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.panelControl3.Controls.Add(this.gcPivotGrid);
			this.panelControl3.Controls.Add(this.gcHiddenFields);
			this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl3.Location = new System.Drawing.Point(0, 0);
			this.panelControl3.Name = "panelControl3";
			this.panelControl3.Size = new System.Drawing.Size(400, 206);
			this.panelControl3.TabIndex = 5;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl3);
			this.Controls.Add(this.panelControl1);
			this.Name = "AllowedLocationsControl";
			this.Size = new System.Drawing.Size(400, 250);
			((System.ComponentModel.ISupportInitialize)(this.gcPivotGrid)).EndInit();
			this.gcPivotGrid.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcDataArea)).EndInit();
			this.pcDataArea.ResumeLayout(false);
			this.pcDataArea.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcColumnArea)).EndInit();
			this.pcColumnArea.ResumeLayout(false);
			this.pcColumnArea.PerformLayout();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcRowArea)).EndInit();
			this.pcRowArea.ResumeLayout(false);
			this.pcRowArea.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcFilterArea)).EndInit();
			this.pcFilterArea.ResumeLayout(false);
			this.pcFilterArea.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcHiddenFields)).EndInit();
			this.gcHiddenFields.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcHiddenFields)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.GroupControl gcPivotGrid;
		private XtraEditors.PanelControl pcDataArea;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.PanelControl pcColumnArea;
		private XtraEditors.LabelControl labelControl2;
		private System.Windows.Forms.Panel panel1;
		private XtraEditors.PanelControl pcRowArea;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.PanelControl panelControl2;
		private XtraEditors.PanelControl pcFilterArea;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.GroupControl gcHiddenFields;
		private XtraEditors.PanelControl pcHiddenFields;
		private XtraEditors.PanelControl panelControl1;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.PanelControl panelControl3;
	}
}
