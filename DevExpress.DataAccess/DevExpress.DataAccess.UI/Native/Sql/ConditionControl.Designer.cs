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

namespace DevExpress.DataAccess.UI.Native.Sql {
	partial class ConditionControl {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.labelRightColumn = new DevExpress.XtraEditors.LabelControl();
			this.labelRightTable = new DevExpress.XtraEditors.LabelControl();
			this.labelLeftColumn = new DevExpress.XtraEditors.LabelControl();
			this.labelLeftTable = new DevExpress.XtraEditors.LabelControl();
			this.labelLeftPoint = new DevExpress.XtraEditors.LabelControl();
			this.labelRightPoint = new DevExpress.XtraEditors.LabelControl();
			this.SuspendLayout();
			this.labelRightColumn.Cursor = System.Windows.Forms.Cursors.Hand;
			this.labelRightColumn.Location = new System.Drawing.Point(414, 3);
			this.labelRightColumn.Name = "labelRightColumn";
			this.labelRightColumn.Size = new System.Drawing.Size(60, 13);
			this.labelRightColumn.TabIndex = 5;
			this.labelRightColumn.Text = "[ManagerId]";
			this.labelRightColumn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelRightColumn_MouseClick);
			this.labelRightColumn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.labelRightColumn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UnderlineLabel_MouseMove);
			this.labelRightTable.Cursor = System.Windows.Forms.Cursors.Hand;
			this.labelRightTable.Location = new System.Drawing.Point(267, 3);
			this.labelRightTable.Name = "labelRightTable";
			this.labelRightTable.Size = new System.Drawing.Size(141, 13);
			this.labelRightTable.TabIndex = 3;
			this.labelRightTable.Text = "[HumanResources.Employee]";
			this.labelRightTable.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelRightTable_MouseClick);
			this.labelRightTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.labelRightTable.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UnderlineLabel_MouseMove);
			this.labelLeftColumn.Cursor = System.Windows.Forms.Cursors.Hand;
			this.labelLeftColumn.Location = new System.Drawing.Point(161, 3);
			this.labelLeftColumn.Name = "labelLeftColumn";
			this.labelLeftColumn.Size = new System.Drawing.Size(64, 13);
			this.labelLeftColumn.TabIndex = 1;
			this.labelLeftColumn.Text = "[EmployeeId]";
			this.labelLeftColumn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelLeftColumn_MouseClick);
			this.labelLeftColumn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.labelLeftColumn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UnderlineLabel_MouseMove);
			this.labelLeftTable.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.labelLeftTable.Location = new System.Drawing.Point(2, 3);
			this.labelLeftTable.Name = "labelLeftTable";
			this.labelLeftTable.Size = new System.Drawing.Size(153, 13);
			this.labelLeftTable.TabIndex = 2;
			this.labelLeftTable.Text = "[HumanResources.Employee_1]";
			this.labelLeftTable.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelLeftTable_MouseClick);
			this.labelLeftTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.labelLeftTable.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UnderlineLabel_MouseMove);
			this.labelLeftPoint.Location = new System.Drawing.Point(155, 3);
			this.labelLeftPoint.Name = "labelLeftPoint";
			this.labelLeftPoint.Size = new System.Drawing.Size(4, 13);
			this.labelLeftPoint.TabIndex = 6;
			this.labelLeftPoint.Text = ".";
			this.labelLeftPoint.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.labelRightPoint.Location = new System.Drawing.Point(408, 3);
			this.labelRightPoint.Name = "labelRightPoint";
			this.labelRightPoint.Size = new System.Drawing.Size(4, 13);
			this.labelRightPoint.TabIndex = 7;
			this.labelRightPoint.Text = ".";
			this.labelRightPoint.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnderlineControl_MouseDown);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.labelRightPoint);
			this.Controls.Add(this.labelLeftPoint);
			this.Controls.Add(this.labelRightColumn);
			this.Controls.Add(this.labelRightTable);
			this.Controls.Add(this.labelLeftColumn);
			this.Controls.Add(this.labelLeftTable);
			this.Name = "ConditionControl";
			this.Size = new System.Drawing.Size(477, 19);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl labelRightColumn;
		private DevExpress.XtraEditors.LabelControl labelRightTable;
		private DevExpress.XtraEditors.LabelControl labelLeftColumn;
		private DevExpress.XtraEditors.LabelControl labelLeftTable;
		private DevExpress.XtraEditors.LabelControl labelLeftPoint;
		private DevExpress.XtraEditors.LabelControl labelRightPoint;
	}
}
