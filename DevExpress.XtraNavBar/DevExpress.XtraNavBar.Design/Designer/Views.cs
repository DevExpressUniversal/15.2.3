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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraNavBar.Design;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraNavBar.Frames {
	[ToolboxItem(false)]
	public class Views : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		private DevExpress.XtraEditors.SimpleButton btApply;
		private DevExpress.XtraEditors.GroupControl groupControl2;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.ListBoxControl lbStyles;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.SplitterControl splitterControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private DevExpress.XtraEditors.PanelControl panelControl3;
		private DevExpress.XtraNavBar.NavBarControl navBarControl1;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
			this.btApply = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.lbStyles = new DevExpress.XtraEditors.ListBoxControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(552, 42);
			this.pnlMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.panelControl3,
																				  this.panelControl2,
																				  this.splitterControl1,
																				  this.panelControl1});
			this.pnlMain.DockPadding.All = 4;
			this.pnlMain.Location = new System.Drawing.Point(0, 24);
			this.pnlMain.Size = new System.Drawing.Size(552, 460);
			this.horzSplitter.Size = new System.Drawing.Size(552, 4);
			this.navBarControl1.ActiveGroup = null;
			this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navBarControl1.Location = new System.Drawing.Point(6, 20);
			this.navBarControl1.Name = "navBarControl1";
			this.navBarControl1.Size = new System.Drawing.Size(180, 426);
			this.navBarControl1.TabIndex = 0;
			this.navBarControl1.Text = "navBarControl1";
			this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.FlatViewInfoRegistrator();
			this.btApply.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btApply.Location = new System.Drawing.Point(244, 12);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(92, 24);
			this.btApply.TabIndex = 4;
			this.btApply.Text = "&Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			this.groupControl2.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.navBarControl1});
			this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl2.DockPadding.All = 2;
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.Size = new System.Drawing.Size(192, 452);
			this.groupControl2.TabIndex = 3;
			this.groupControl2.Text = "Preview:";
			this.groupControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.lbStyles});
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.DockPadding.All = 2;
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(348, 404);
			this.groupControl1.TabIndex = 6;
			this.groupControl1.Text = "View Styles:";
			this.lbStyles.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.lbStyles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbStyles.Location = new System.Drawing.Point(6, 20);
			this.lbStyles.Name = "lbStyles";
			this.lbStyles.Size = new System.Drawing.Size(336, 378);
			this.lbStyles.TabIndex = 5;
			this.lbStyles.SelectedIndexChanged += new System.EventHandler(this.cbStyles_SelectedIndexChanged);
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.groupControl2});
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelControl1.Location = new System.Drawing.Point(4, 4);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(192, 452);
			this.panelControl1.TabIndex = 7;
			this.panelControl1.Text = "panelControl1";
			this.splitterControl1.Location = new System.Drawing.Point(196, 4);
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.Size = new System.Drawing.Size(4, 452);
			this.splitterControl1.TabIndex = 8;
			this.splitterControl1.TabStop = false;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.btApply});
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl2.Location = new System.Drawing.Point(200, 408);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Size = new System.Drawing.Size(348, 48);
			this.panelControl2.TabIndex = 9;
			this.panelControl2.Text = "panelControl2";
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.groupControl1});
			this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl3.Location = new System.Drawing.Point(200, 4);
			this.panelControl3.Name = "panelControl3";
			this.panelControl3.Size = new System.Drawing.Size(348, 404);
			this.panelControl3.TabIndex = 10;
			this.panelControl3.Text = "panelControl3";
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.horzSplitter,
																		  this.pnlMain,
																		  this.lbCaption});
			this.Name = "Views";
			this.Size = new System.Drawing.Size(552, 484);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		public Views() : base(4) {
			InitializeComponent();
		}
		protected override void InitImages() {
			base.InitImages();
			btApply.Image = DesignerImages16.Images[DesignerImages16ApplyIndex];
		}
		private NavBarControl NavBar { get { return EditingObject as NavBarControl; } }
		private int IndexOf(XtraEditors.Controls.ListBoxItemCollection objCollection, string s) {
			for(int i = 0; i < objCollection.Count; i++)
				if(objCollection[i].ToString() == s) return i;
			return -1;
		}
		protected override string DescriptionText { get { return "Select a style from the 'View Styles' list. A preview of this style is displayed in the 'Preview' panel. Press the Apply button to apply this style to the NavBar Control."; } }
		public override void InitComponent() {
			NavBarControlDesigner.PopulateDesignTimeViews(NavBar);
			lbStyles.Items.AddRange(NavBar.AvailableNavBarViews.ToArray(typeof(object)) as object[]);
			lbStyles.SelectedIndex = IndexOf(lbStyles.Items, NavBar.View.ToString());
			XView xv = new XView(navBarControl1);
		}
		private void cbStyles_SelectedIndexChanged(object sender, System.EventArgs e) {
			navBarControl1.View = lbStyles.SelectedItem as DevExpress.XtraNavBar.ViewInfo.BaseViewInfoRegistrator;
			navBarControl1.ResetStyles();
		}
		private void btApply_Click(object sender, System.EventArgs e) {
			NavBar.PaintStyleName = navBarControl1.View.ViewName;
			NavBar.ResetStyles();
		}
	}
}
