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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)] 
	public class PaintStyleFrame : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		private DevExpress.XtraBars.Design.Frames.PaintStyle paintStyle1;
		private DevExpress.XtraEditors.SimpleButton simpleButton1;
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.IContainer components = null;
		protected override string DescriptionText { get { return "Select the paint scheme, the result is displayed in the preview section .To apply the selected paint style, press the apply button."; } }
		public PaintStyleFrame() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.paintStyle1 = new DevExpress.XtraBars.Design.Frames.PaintStyle();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(591, 38);
			this.pnlMain.Controls.Add(this.paintStyle1);
			this.pnlMain.Location = new System.Drawing.Point(0, 38);
			this.pnlMain.Size = new System.Drawing.Size(467, 370);
			this.horzSplitter.Size = new System.Drawing.Size(467, 4);
			this.horzSplitter.Visible = false;
			this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.simpleButton1.Location = new System.Drawing.Point(8, 330);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(108, 30);
			this.simpleButton1.TabIndex = 12;
			this.simpleButton1.Text = "&Apply";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.panel1.Controls.Add(this.simpleButton1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(467, 38);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(124, 370);
			this.panel1.TabIndex = 13;
			this.paintStyle1.AutoScroll = true;
			this.paintStyle1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paintStyle1.Location = new System.Drawing.Point(0, 0);
			this.paintStyle1.Name = "paintStyle1";
			this.paintStyle1.Size = new System.Drawing.Size(467, 370);
			this.paintStyle1.TabIndex = 0;
			this.Controls.Add(this.panel1);
			this.Name = "PaintStyleFrame";
			this.Size = new System.Drawing.Size(591, 408);
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		[Browsable(false)]
		public BarManager Manager {	get { return EditingObject as BarManager; }	}
		private BarAndDockingController GetController(bool forceCreateController) {
			if(Manager == null) return null;
			if(!forceCreateController) return Manager.GetController();
			if(Manager.Controller != null) return Manager.Controller;
			IDesignerHost host = Manager.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return Manager.GetController();
			DefaultBarAndDockingController defController = FindComponent(host, typeof(DefaultBarAndDockingController)) as DefaultBarAndDockingController;
			if(defController != null) return defController.Controller;
			BarAndDockingController controller = FindComponent(host, typeof(BarAndDockingController)) as BarAndDockingController;
			if(controller == null) {
				controller = host.CreateComponent(typeof(BarAndDockingController)) as BarAndDockingController;
			}
			if(controller != null) {
				Manager.Controller = controller;
			}
			Manager.Controller = controller;
			return Manager.GetController();
		}
		object FindComponent(IDesignerHost host, Type type) {
			if(host == null || type == null || host.Container == null) return null;
			foreach(object obj in host.Container.Components) {
				if(obj.GetType().Equals(type) || obj.GetType().IsSubclassOf(type)) return obj;
			}
			return null;
		}
		public override void DoInitFrame() {
			paintStyle1.InitStyle(GetController(false).PaintStyleName);
			paintStyle1.SetImages("DevExpress.XtraBars.Design.Frames.BarStyles.gif");
		}
		private void simpleButton1_Click(object sender, System.EventArgs e) {
			GetController(true).PaintStyleName = paintStyle1.PaintStyleName;
		}
	}
}
