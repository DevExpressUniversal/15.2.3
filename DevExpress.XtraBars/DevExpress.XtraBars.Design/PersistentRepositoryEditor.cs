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
using DevExpress.XtraEditors.Design;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Design {
	[ToolboxItem(false)]
	[CLSCompliant(false)]
	public class PersistentRepositoryEditor : DevExpress.XtraEditors.Design.PersistentRepositoryEditor {
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private System.ComponentModel.Container components = null;
		public PersistentRepositoryEditor() : base() {
			InitializeComponent();
			pgMain.BringToFront();
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
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.btnSearch.Location = new System.Drawing.Point(423, 4);
			this.splMain.Size = new System.Drawing.Size(5, 176);
			this.pgMain.Size = new System.Drawing.Size(292, 176);
			this.pnlControl.Controls.Add(this.btnClear);
			this.pnlControl.Size = new System.Drawing.Size(469, 54);
			this.pnlControl.Controls.SetChildIndex(this.btnSearch, 0);
			this.pnlControl.Controls.SetChildIndex(this.btnClear, 0);
			this.pnlControl.Controls.SetChildIndex(this.btRemove, 0);
			this.pnlControl.Controls.SetChildIndex(this.btAdd, 0);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(469, 42);
			this.pnlMain.Size = new System.Drawing.Size(172, 176);
			this.horzSplitter.Size = new System.Drawing.Size(469, 4);
			this.btnClear.Location = new System.Drawing.Point(267, 4);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(150, 30);
			this.btnClear.TabIndex = 1;
			this.btnClear.Text = "Remove &Unused";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.Name = "PersistentRepositoryEditor";
			this.Size = new System.Drawing.Size(469, 276);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override RepositoryItemCollection ItemCollection {
			get {
				if(base.EditingObject is RibbonControl)return ((RibbonControl)base.EditingObject).Manager.RepositoryItems;
				return ((DevExpress.XtraBars.BarManager)base.EditingObject).RepositoryItems;
			}
		}
		public override void InitComponent() {
			base.InitComponent();
		}
		private void btnClear_Click(object sender, System.EventArgs e) {
			RemoveAllItems();
		}
		protected override bool AllowGlobalStore { get { return false; } }
	}
}
