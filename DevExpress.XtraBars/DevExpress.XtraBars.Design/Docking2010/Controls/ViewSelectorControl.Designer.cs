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

namespace DevExpress.XtraBars.Design {
	partial class ViewSelectorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSelectorControl));
			this.documentManagerLink = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			this.viewComboBox = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.viewListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.addViewButton = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.viewsIcon = new System.Windows.Forms.ImageList();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewComboBox.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			this.panelControl4.SuspendLayout();
			this.SuspendLayout();
			this.documentManagerLink.Appearance.ForeColor = System.Drawing.Color.Blue;
			this.documentManagerLink.Cursor = System.Windows.Forms.Cursors.Hand;
			this.documentManagerLink.Dock = System.Windows.Forms.DockStyle.Top;
			this.documentManagerLink.Location = new System.Drawing.Point(0, 10);
			this.documentManagerLink.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.documentManagerLink.Name = "documentManagerLink";
			this.documentManagerLink.Padding = new System.Windows.Forms.Padding(0, 2, 3, 5);
			this.documentManagerLink.Size = new System.Drawing.Size(93, 20);
			this.documentManagerLink.TabIndex = 0;
			this.documentManagerLink.Text = "DocumentManager";
			this.documentManagerLink.MouseEnter += new System.EventHandler(this.hyperLink_MouseEnter);
			this.documentManagerLink.MouseLeave += new System.EventHandler(this.hyperLink_MouseLeave);
			this.panelControl1.AutoSize = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.panelControl3);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl1.Location = new System.Drawing.Point(0, 30);
			this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Padding = new System.Windows.Forms.Padding(0, 10, 10, 10);
			this.panelControl1.Size = new System.Drawing.Size(346, 44);
			this.panelControl1.TabIndex = 2;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.Add(this.viewComboBox);
			this.panelControl3.Controls.Add(this.labelControl3);
			this.panelControl3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl3.Location = new System.Drawing.Point(0, 10);
			this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.panelControl3.Name = "panelControl3";
			this.panelControl3.Size = new System.Drawing.Size(336, 24);
			this.panelControl3.TabIndex = 4;
			this.viewComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewComboBox.Location = new System.Drawing.Point(67, 0);
			this.viewComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
			this.viewComboBox.Name = "viewComboBox";
			this.viewComboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.viewComboBox.Size = new System.Drawing.Size(269, 20);
			this.viewComboBox.TabIndex = 2;
			this.labelControl3.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelControl3.Location = new System.Drawing.Point(0, 0);
			this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Padding = new System.Windows.Forms.Padding(0, 2, 5, 0);
			this.labelControl3.Size = new System.Drawing.Size(67, 15);
			this.labelControl3.TabIndex = 3;
			this.labelControl3.Text = "Current View";
			this.panelControl2.AutoSize = true;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.viewListBox);
			this.panelControl2.Controls.Add(this.labelControl4);
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl2.Location = new System.Drawing.Point(0, 74);
			this.panelControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Padding = new System.Windows.Forms.Padding(0, 0, 10, 10);
			this.panelControl2.Size = new System.Drawing.Size(346, 411);
			this.panelControl2.TabIndex = 3;
			this.viewListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewListBox.Location = new System.Drawing.Point(0, 23);
			this.viewListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.viewListBox.Name = "viewListBox";
			this.viewListBox.Size = new System.Drawing.Size(336, 378);
			this.viewListBox.TabIndex = 3;
			this.viewListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.viewListBoxKeyDown);
			this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.labelControl4.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelControl4.Location = new System.Drawing.Point(0, 0);
			this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.labelControl4.Size = new System.Drawing.Size(87, 23);
			this.labelControl4.TabIndex = 2;
			this.labelControl4.Text = "View Collection:";
			this.addViewButton.Location = new System.Drawing.Point(0, 10);
			this.addViewButton.Name = "addViewButton";
			this.addViewButton.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
			this.addViewButton.Size = new System.Drawing.Size(79, 30);
			this.addViewButton.TabIndex = 4;
			this.addViewButton.Text = "Add View";
			this.addViewButton.Click += new System.EventHandler(this.AddView);
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Controls.Add(this.simpleButton1);
			this.panelControl4.Controls.Add(this.addViewButton);
			this.panelControl4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl4.Location = new System.Drawing.Point(0, 485);
			this.panelControl4.Name = "panelControl4";
			this.panelControl4.Padding = new System.Windows.Forms.Padding(0, 10, 10, 10);
			this.panelControl4.Size = new System.Drawing.Size(346, 50);
			this.panelControl4.TabIndex = 4;
			this.simpleButton1.Location = new System.Drawing.Point(85, 10);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
			this.simpleButton1.Size = new System.Drawing.Size(79, 30);
			this.simpleButton1.TabIndex = 5;
			this.simpleButton1.Text = "Remove View";
			this.simpleButton1.Click += new System.EventHandler(this.RemoveView);
			this.viewsIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("viewsIcon.ImageStream")));
			this.viewsIcon.TransparentColor = System.Drawing.Color.Transparent;
			this.viewsIcon.Images.SetKeyName(0, "TabbedView_16x16.png");
			this.viewsIcon.Images.SetKeyName(1, "NativeMDIView_16x16.png");
			this.viewsIcon.Images.SetKeyName(2, "MetroView_16x16.png");
			this.viewsIcon.Images.SetKeyName(3, "WidgetdView_16x16.png");
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.panelControl4);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.documentManagerLink);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "ViewSelectorControl";
			this.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
			this.Size = new System.Drawing.Size(346, 545);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			this.panelControl3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewComboBox.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			this.panelControl2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.viewListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			this.panelControl4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl documentManagerLink;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private DevExpress.XtraEditors.LabelControl labelControl4;
		private DevExpress.XtraEditors.ListBoxControl viewListBox;
		private DevExpress.XtraEditors.PanelControl panelControl3;
		private DevExpress.XtraEditors.ImageComboBoxEdit viewComboBox;
		private DevExpress.XtraEditors.LabelControl labelControl3;
		private DevExpress.XtraEditors.SimpleButton addViewButton;
		private DevExpress.XtraEditors.PanelControl panelControl4;
		private DevExpress.XtraEditors.SimpleButton simpleButton1;
		private System.Windows.Forms.ImageList viewsIcon;
	}
}
