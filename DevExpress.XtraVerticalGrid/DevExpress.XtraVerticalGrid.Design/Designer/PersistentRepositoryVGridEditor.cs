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
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraVerticalGrid.Frames {
	[ToolboxItem(false)]
	public class PersistentRepositoryVGridEditor : DevExpress.XtraEditors.Design.PersistentRepositoryEditor {
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private System.ComponentModel.Container components = null;
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		protected override bool AllowGlobalStore { get { return false; } }
		#region Component Designer generated code
		private void InitializeComponent() {
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.btAdd.Location = new System.Drawing.Point(0, 4);
			this.btRemove.Location = new System.Drawing.Point(178, 4);
			this.btRemove.Size = new System.Drawing.Size(79, 30);
			this.listView1.MultiColumnListBox.Size = new System.Drawing.Size(160, 258);
			this.pnlControl.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.btnClear});
			this.pnlMain.Location = new System.Drawing.Point(0, 56);
			this.pnlMain.Size = new System.Drawing.Size(172, 284);
			this.horzSplitter.Location = new System.Drawing.Point(172, 56);
			this.horzSplitter.Size = new System.Drawing.Size(512, 4);
			this.btnClear.Location = new System.Drawing.Point(263, 4);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(115, 30);
			this.btnClear.TabIndex = 1;
			this.btnClear.Text = "Remove &Unused";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pgMain,
																		  this.splMain,
																		  this.horzSplitter,
																		  this.pnlMain,
																		  this.pnlControl,
																		  this.lbCaption});
			this.Name = "PersistentRepositoryVGridEditor";
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init
		protected override string DescriptionText { get { return "Here you can manage the editors used for in-place editing within cells (add, remove and modify their settings). To assign editors to grid rows, switch to the Rows page. Editors that are not used can be deleted via the Remove Unused button."; } }
		public PersistentRepositoryVGridEditor() : base() {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override RepositoryItemCollection ItemCollection {
			get {
				return ((VGridControlBase)base.EditingObject).RepositoryItems;
			}
		}
		public override void InitComponent() {
			base.InitComponent();
		}
		#endregion
		#region Editing
		private void btnClear_Click(object sender, System.EventArgs e) {
			RemoveAllItems();
		}
		#endregion
	}
}
