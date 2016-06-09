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
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public class AppearancesDesignerSimple : AppearancesDesignerBase {
		private System.ComponentModel.Container components = null;
		public AppearancesDesignerSimple() {
			InitializeComponent();
			pgMain.BringToFront();
			bpAppearances.Buttons[0].Properties.Visible = false;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).BeginInit();
			this.pnlAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.scAppearance.Location = new System.Drawing.Point(0, 0);
			this.scAppearance.Size = new System.Drawing.Size(5, 296);
			this.scAppearance.Visible = false;
			this.gcAppearances.Size = new System.Drawing.Size(160, 271);
			this.gcPreview.Dock = System.Windows.Forms.DockStyle.None;
			this.gcPreview.Location = new System.Drawing.Point(132, 0);
			this.gcPreview.Size = new System.Drawing.Size(1, 336);
			this.gcPreview.Visible = false;
			this.lbcAppearances.Size = new System.Drawing.Size(152, 244);
			this.bpAppearances.Location = new System.Drawing.Point(0, 271);
			this.bpAppearances.Size = new System.Drawing.Size(160, 25);
			this.pnlAppearances.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlAppearances.Size = new System.Drawing.Size(160, 296);
			this.pnlPreview.Dock = System.Windows.Forms.DockStyle.None;
			this.pnlPreview.Size = new System.Drawing.Size(0, 276);
			this.pnlPreview.Visible = false;
			this.splMain.Location = new System.Drawing.Point(160, 100);
			this.splMain.Size = new System.Drawing.Size(5, 296);
			this.pgMain.Location = new System.Drawing.Point(165, 100);
			this.pgMain.Size = new System.Drawing.Size(623, 296);
			this.pnlControl.Location = new System.Drawing.Point(0, 46);
			this.pnlControl.Size = new System.Drawing.Size(788, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(788, 42);
			this.pnlMain.Location = new System.Drawing.Point(0, 100);
			this.pnlMain.Size = new System.Drawing.Size(160, 296);
			this.horzSplitter.Location = new System.Drawing.Point(0, 42);
			this.horzSplitter.Size = new System.Drawing.Size(788, 4);
			this.Name = "AppearancesDesignerSimple";
			this.Size = new System.Drawing.Size(788, 396);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).EndInit();
			this.pnlAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string DescriptionText { get { return Properties.Resources.AppearanceDesignerDescription; } }
		protected virtual BaseAppearanceCollection Appearances { get { return null; } }
		public override void InitComponent() {
			CreateTabControl();
			InitAppearanceList(Appearances);
		}
		protected override void AddObject(ArrayList ret, string item) {
			object obj = GetAppearanceObjectByName(Appearances, item);
			ret.Add(obj);
		}
		protected override void bpAppearances_ButtonClick(object sender, XtraBars.Docking2010.BaseButtonEventArgs e) {
			if(e.Button.Properties.Tag == null) return;
			switch(e.Button.Properties.Tag.ToString()) {
				case "select":
					SelectAll();
					break;
				case "default":
					SetDefault();
					break;
			}
		}
		protected override void lbcAppearances_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(!e.Control) return;
			if(e.KeyCode == Keys.A) SelectAll();
			if(e.KeyCode == Keys.D)	SetDefault();
		}
		protected override void LoadAppearances(string name) {
			Appearances.RestoreLayoutFromXml(name);
		}
		protected override void SaveAppearances(string name) {
			Appearances.SaveLayoutToXml(name);
		}
	}
}
