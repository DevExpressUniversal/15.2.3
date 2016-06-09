#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.ExpressApp.Design {
	partial class DesignErrorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing){
				BuildProjectClicked = null;
				if(Parent != null) {
					Parent.Controls.Remove(this);
				}
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.mainPanel = new System.Windows.Forms.Panel();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.controlsPanel = new System.Windows.Forms.Panel();
			this.buildProjectLink = new System.Windows.Forms.LinkLabel();
			this.showGeneratedCodeLink = new System.Windows.Forms.LinkLabel();
			this.callStackPanel = new System.Windows.Forms.Panel();
			this.callStackTextBox = new System.Windows.Forms.TextBox();
			this.showCallStackLinkPanel = new System.Windows.Forms.Panel();
			this.showCallStackLink = new System.Windows.Forms.LinkLabel();
			this.errorMessageTextBox = new System.Windows.Forms.TextBox();
			this.topPanel = new System.Windows.Forms.Panel();
			this.headerTextBox = new System.Windows.Forms.TextBox();
			this.iconPanel = new System.Windows.Forms.Panel();
			this.mainPanel.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			this.controlsPanel.SuspendLayout();
			this.callStackPanel.SuspendLayout();
			this.showCallStackLinkPanel.SuspendLayout();
			this.topPanel.SuspendLayout();
			this.SuspendLayout();
			this.mainPanel.BackColor = System.Drawing.Color.GhostWhite;
			this.mainPanel.Controls.Add(this.bottomPanel);
			this.mainPanel.Controls.Add(this.topPanel);
			this.mainPanel.Location = new System.Drawing.Point(10, 10);
			this.mainPanel.MinimumSize = new System.Drawing.Size(300, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Padding = new System.Windows.Forms.Padding(1);
			this.mainPanel.Size = new System.Drawing.Size(492, 225);
			this.mainPanel.TabIndex = 0;
			this.bottomPanel.Controls.Add(this.controlsPanel);
			this.bottomPanel.Controls.Add(this.callStackPanel);
			this.bottomPanel.Controls.Add(this.errorMessageTextBox);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bottomPanel.Location = new System.Drawing.Point(1, 42);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(490, 182);
			this.bottomPanel.TabIndex = 1;
			this.controlsPanel.BackColor = System.Drawing.Color.GhostWhite;
			this.controlsPanel.Controls.Add(this.buildProjectLink);
			this.controlsPanel.Controls.Add(this.showGeneratedCodeLink);
			this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.controlsPanel.Location = new System.Drawing.Point(0, 134);
			this.controlsPanel.Name = "controlsPanel";
			this.controlsPanel.Size = new System.Drawing.Size(490, 40);
			this.controlsPanel.TabIndex = 5;
			this.buildProjectLink.AutoSize = true;
			this.buildProjectLink.Location = new System.Drawing.Point(3, 20);
			this.buildProjectLink.Name = "buildProjectLink";
			this.buildProjectLink.Size = new System.Drawing.Size(197, 13);
			this.buildProjectLink.TabIndex = 3;
			this.buildProjectLink.TabStop = true;
			this.buildProjectLink.Text = "Build the project and reload the designer";
			this.buildProjectLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buildProjectLink_LinkClicked);
			this.showGeneratedCodeLink.AutoSize = true;
			this.showGeneratedCodeLink.Location = new System.Drawing.Point(3, 3);
			this.showGeneratedCodeLink.Name = "showMoreDetail";
			this.showGeneratedCodeLink.Size = new System.Drawing.Size(197, 13);
			this.showGeneratedCodeLink.TabIndex = 4;
			this.showGeneratedCodeLink.TabStop = true;
			this.showGeneratedCodeLink.Text = "Show generated code";
			this.showGeneratedCodeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showGeneratedCode_LinkClicked);
			this.callStackPanel.BackColor = System.Drawing.Color.Lavender;
			this.callStackPanel.Controls.Add(this.callStackTextBox);
			this.callStackPanel.Controls.Add(this.showCallStackLinkPanel);
			this.callStackPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.callStackPanel.Location = new System.Drawing.Point(0, 87);
			this.callStackPanel.Name = "callStackPanel";
			this.callStackPanel.Size = new System.Drawing.Size(490, 47);
			this.callStackPanel.TabIndex = 4;
			this.callStackTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.callStackTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.callStackTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.callStackTextBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.callStackTextBox.Location = new System.Drawing.Point(0, 26);
			this.callStackTextBox.MaximumSize = new System.Drawing.Size(0, 300);
			this.callStackTextBox.Multiline = true;
			this.callStackTextBox.Name = "callStackTextBox";
			this.callStackTextBox.ReadOnly = true;
			this.callStackTextBox.Size = new System.Drawing.Size(490, 28);
			this.callStackTextBox.TabIndex = 7;
			this.callStackTextBox.TabStop = false;
			this.showCallStackLinkPanel.BackColor = System.Drawing.SystemColors.Window;
			this.showCallStackLinkPanel.Controls.Add(this.showCallStackLink);
			this.showCallStackLinkPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.showCallStackLinkPanel.Location = new System.Drawing.Point(0, 0);
			this.showCallStackLinkPanel.Name = "showCallStackLinkPanel";
			this.showCallStackLinkPanel.Size = new System.Drawing.Size(490, 26);
			this.showCallStackLinkPanel.TabIndex = 6;
			this.showCallStackLink.AutoSize = true;
			this.showCallStackLink.Location = new System.Drawing.Point(3, 7);
			this.showCallStackLink.Name = "showCallStackLink";
			this.showCallStackLink.Size = new System.Drawing.Size(39, 13);
			this.showCallStackLink.TabIndex = 3;
			this.showCallStackLink.TabStop = true;
			this.showCallStackLink.Text = "Details";
			this.showCallStackLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.showCallStackLink_LinkClicked);
			this.errorMessageTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.errorMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.errorMessageTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.errorMessageTextBox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.errorMessageTextBox.Location = new System.Drawing.Point(0, 0);
			this.errorMessageTextBox.MaximumSize = new System.Drawing.Size(0, 200);
			this.errorMessageTextBox.MinimumSize = new System.Drawing.Size(0, 30);
			this.errorMessageTextBox.Multiline = true;
			this.errorMessageTextBox.Name = "errorMessageTextBox";
			this.errorMessageTextBox.ReadOnly = true;
			this.errorMessageTextBox.Size = new System.Drawing.Size(490, 87);
			this.errorMessageTextBox.TabIndex = 3;
			this.errorMessageTextBox.TabStop = false;
			this.topPanel.Controls.Add(this.headerTextBox);
			this.topPanel.Controls.Add(this.iconPanel);
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(1, 1);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(490, 41);
			this.topPanel.TabIndex = 0;
			this.headerTextBox.BackColor = System.Drawing.Color.GhostWhite;
			this.headerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.headerTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headerTextBox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.headerTextBox.Location = new System.Drawing.Point(41, 0);
			this.headerTextBox.Multiline = true;
			this.headerTextBox.Name = "headerTextBox";
			this.headerTextBox.ReadOnly = true;
			this.headerTextBox.Size = new System.Drawing.Size(449, 41);
			this.headerTextBox.TabIndex = 3;
			this.headerTextBox.TabStop = false;
			this.iconPanel.BackColor = System.Drawing.Color.GhostWhite;
			this.iconPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.iconPanel.Location = new System.Drawing.Point(0, 0);
			this.iconPanel.Name = "iconPanel";
			this.iconPanel.Size = new System.Drawing.Size(41, 41);
			this.iconPanel.TabIndex = 0;
			this.iconPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.iconPanel_Paint);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.mainPanel);
			this.Name = "DesignErrorControl";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.Size = new System.Drawing.Size(512, 243);
			this.mainPanel.ResumeLayout(false);
			this.bottomPanel.ResumeLayout(false);
			this.bottomPanel.PerformLayout();
			this.controlsPanel.ResumeLayout(false);
			this.controlsPanel.PerformLayout();
			this.callStackPanel.ResumeLayout(false);
			this.callStackPanel.PerformLayout();
			this.showCallStackLinkPanel.ResumeLayout(false);
			this.showCallStackLinkPanel.PerformLayout();
			this.topPanel.ResumeLayout(false);
			this.topPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.Panel iconPanel;
		private System.Windows.Forms.TextBox headerTextBox;
		private System.Windows.Forms.TextBox errorMessageTextBox;
		private System.Windows.Forms.Panel controlsPanel;
		private System.Windows.Forms.LinkLabel buildProjectLink;
		private System.Windows.Forms.Panel callStackPanel;
		private System.Windows.Forms.TextBox callStackTextBox;
		private System.Windows.Forms.Panel showCallStackLinkPanel;
		private System.Windows.Forms.LinkLabel showCallStackLink;
		private System.Windows.Forms.LinkLabel showGeneratedCodeLink;
	}
}
