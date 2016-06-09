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

using System.ComponentModel;
namespace DevExpress.XtraBars.Design.Frames {
	partial class MainFrame {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			components = new Container();
			this.viewSelector = new ViewSelectorControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(240, 60);
			this.splMain.Size = new System.Drawing.Size(5, 362);
			this.pgMain.Location = new System.Drawing.Point(245, 60);
			this.pgMain.Size = new System.Drawing.Size(434, 362);
			this.pnlControl.Size = new System.Drawing.Size(679, 32);
			this.pnlControl.Visible = false;
			this.lbCaption.Size = new System.Drawing.Size(679, 42);
			this.pnlMain.Controls.Add(this.viewSelector);
			this.pnlMain.Size = new System.Drawing.Size(240, 362);
			this.horzSplitter.Size = new System.Drawing.Size(679, 4);
			this.viewSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewSelector.Location = new System.Drawing.Point(0, 0);
			this.viewSelector.Name = "viewSelector";
			this.viewSelector.Size = new System.Drawing.Size(240, 362);
			this.viewSelector.TabIndex = 0;
			this.viewSelector.Text = "viewSelector";
			this.Name = "MainFrame";
			this.Size = new System.Drawing.Size(679, 422);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ViewSelectorControl viewSelector;
	}
}
