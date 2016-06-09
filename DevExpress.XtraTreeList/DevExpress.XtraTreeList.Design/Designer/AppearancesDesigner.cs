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
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	public class AppearancesDesigner : DevExpress.XtraEditors.Frames.AppearancesDesignerSimple {
		private DevExpress.XtraEditors.PanelControl pcPaintStyle;
		private DevExpress.XtraEditors.CheckEdit ceOdd;
		private DevExpress.XtraEditors.CheckEdit ceEven;
		private System.ComponentModel.Container components = null;
		public AppearancesDesigner() {
			InitializeComponent();
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
			this.pcPaintStyle = new DevExpress.XtraEditors.PanelControl();
			this.ceOdd = new DevExpress.XtraEditors.CheckEdit();
			this.ceEven = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPaintStyle)).BeginInit();
			this.pcPaintStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceOdd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEven.Properties)).BeginInit();
			this.SuspendLayout();
			this.pgMain.Size = new System.Drawing.Size(422, 336);
			this.lbCaption.Size = new System.Drawing.Size(588, 42);
			this.horzSplitter.Size = new System.Drawing.Size(588, 4);
			this.pcPaintStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcPaintStyle.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.ceOdd,
																					   this.ceEven});
			this.pcPaintStyle.Location = new System.Drawing.Point(192, 102);
			this.pcPaintStyle.Name = "pcPaintStyle";
			this.pcPaintStyle.Size = new System.Drawing.Size(204, 192);
			this.pcPaintStyle.TabIndex = 5;
			this.pcPaintStyle.Text = "panelControl1";
			this.ceOdd.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.ceOdd.Location = new System.Drawing.Point(8, 36);
			this.ceOdd.Name = "ceOdd";
			this.ceOdd.Properties.Caption = "Enable Appearance OddRow";
			this.ceOdd.Size = new System.Drawing.Size(188, 19);
			this.ceOdd.TabIndex = 7;
			this.ceOdd.CheckedChanged += new System.EventHandler(this.ceOdd_CheckedChanged);
			this.ceEven.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.ceEven.Location = new System.Drawing.Point(8, 12);
			this.ceEven.Name = "ceEven";
			this.ceEven.Properties.Caption = "Enable Appearance EvenRow";
			this.ceEven.Size = new System.Drawing.Size(188, 19);
			this.ceEven.TabIndex = 6;
			this.ceEven.CheckedChanged += new System.EventHandler(this.ceEven_CheckedChanged);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pcPaintStyle,
																		  this.pgMain,
																		  this.splMain,
																		  this.pnlMain,
																		  this.horzSplitter,
																		  this.pnlControl,
																		  this.lbCaption});
			this.Name = "AppearancesDesigner";
			this.Size = new System.Drawing.Size(588, 396);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPaintStyle)).EndInit();
			this.pcPaintStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceOdd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEven.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private TreeList EditingTreeList { get { return EditingObject as TreeList; } }
		protected override BaseAppearanceCollection Appearances { get { return EditingTreeList.Appearance; } }
		protected override Image AppearanceImage { get { return EditingTreeList.BackgroundImage; } }
		public override void InitComponent() {
			base.InitComponent();
			ceEven.Checked = EditingTreeList.OptionsView.EnableAppearanceEvenRow;
			ceOdd.Checked = EditingTreeList.OptionsView.EnableAppearanceOddRow;
		}
		protected override XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] {pgMain, pcPaintStyle}, new string[] {"Properties", "Odd/Even Row Appearances"});
		}
		protected override void SetSelectedObject() {
			ArrayList arr = new ArrayList();
			foreach(AppearanceObject obj in this.SelectedObjects) {
				arr.Add(EditingTreeList.ViewInfo.PaintAppearance.GetAppearance(obj.Name));
			}
			Preview.SetAppearance(arr.ToArray());
		}
		protected override void SetDefault() {
			EditingTreeList.BeginUpdate();
			base.SetDefault();
			EditingTreeList.EndUpdate();
		}
		private void ceEven_CheckedChanged(object sender, System.EventArgs e) {
			EditingTreeList.OptionsView.EnableAppearanceEvenRow = ceEven.Checked;
		}
		private void ceOdd_CheckedChanged(object sender, System.EventArgs e) {
			EditingTreeList.OptionsView.EnableAppearanceOddRow = ceOdd.Checked;
		}
	}
}
