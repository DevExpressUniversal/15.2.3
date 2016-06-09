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
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
namespace DevExpress.XtraVerticalGrid.Frames {
	public class RowPropertiesDesigner : DevExpress.XtraVerticalGrid.Frames.RowDesigner {
		private DevExpress.XtraEditors.SimpleButton sbRows;
		private System.ComponentModel.IContainer components = null;
		public RowPropertiesDesigner() {
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
			this.sbRows = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.treeView1.LineColor = System.Drawing.Color.Black;
			this.btnFields.Location = new System.Drawing.Point(648, 4);
			this.btnFields.Visible = false;
			this.pnlControl.Controls.Add(this.sbRows);
			this.pnlControl.Controls.SetChildIndex(this.sbRows, 0);
			this.pnlControl.Controls.SetChildIndex(this.btnFields, 0);
			this.sbRows.Location = new System.Drawing.Point(432, 4);
			this.sbRows.Name = "sbRows";
			this.sbRows.Size = new System.Drawing.Size(134, 30);
			this.sbRows.TabIndex = 6;
			this.sbRows.Text = "R&etrieve Rows";
			this.sbRows.Click += new System.EventHandler(this.sbRows_Click);
			this.Name = "RowPropertiesDesigner";
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public override void InitComponent() {
			base.InitComponent();
			sbRows.Enabled = EditingVGrid is PropertyGridControl && ((PropertyGridControl)EditingVGrid).SelectedObject != null;
		}
		private void sbRows_Click(object sender, System.EventArgs e) {
			RetrieveFields();
			OnChangeRows();
		}
	}
}
