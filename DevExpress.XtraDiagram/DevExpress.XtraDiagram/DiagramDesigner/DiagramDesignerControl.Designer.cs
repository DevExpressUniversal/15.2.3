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

namespace DevExpress.XtraDiagram.Designer {
	partial class DiagramDesignerControlBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.diagramToolbox = new DevExpress.XtraToolbox.ToolboxControl();
			this.innerSplitter = new DevExpress.XtraEditors.SplitContainerControl();
			this.diagramControl = new DevExpress.XtraDiagram.DiagramControl();
			this.mainSplitter = new DevExpress.XtraEditors.SplitContainerControl();
			this.designerPropertyGridControl = new DevExpress.XtraDiagram.Designer.DiagramDesignerPropertyGridControl();
			((System.ComponentModel.ISupportInitialize)(this.innerSplitter)).BeginInit();
			this.innerSplitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.diagramControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).BeginInit();
			this.mainSplitter.SuspendLayout();
			this.SuspendLayout();
			this.diagramToolbox.Appearance.Toolbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.diagramToolbox.Appearance.Toolbox.Options.UseFont = true;
			this.diagramToolbox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.diagramToolbox.Caption = "Shapes";
			this.diagramToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diagramToolbox.Location = new System.Drawing.Point(0, 0);
			this.diagramToolbox.Name = "diagramToolbox";
			this.diagramToolbox.OptionsView.ItemImageSize = new System.Drawing.Size(32, 32);
			this.diagramToolbox.OptionsView.MenuButtonCaption = "More Shapes";
			this.diagramToolbox.OptionsView.ShowToolboxCaption = true;
			this.diagramToolbox.Size = new System.Drawing.Size(277, 779);
			this.diagramToolbox.TabIndex = 9;
			this.innerSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.innerSplitter.Location = new System.Drawing.Point(0, 0);
			this.innerSplitter.Name = "innerSplitter";
			this.innerSplitter.Panel1.Controls.Add(this.diagramToolbox);
			this.innerSplitter.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.innerSplitter.Panel1.Text = "Panel1";
			this.innerSplitter.Panel2.Controls.Add(this.diagramControl);
			this.innerSplitter.Panel2.Padding = new System.Windows.Forms.Padding(1, 0, 2, 0);
			this.innerSplitter.Panel2.Text = "Panel2";
			this.innerSplitter.Size = new System.Drawing.Size(1181, 779);
			this.innerSplitter.SplitterPosition = 278;
			this.innerSplitter.TabIndex = 10;
			this.diagramControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.diagramControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diagramControl.Location = new System.Drawing.Point(1, 0);
			this.diagramControl.Name = "diagramControl";
			this.diagramControl.OptionsView.PageSize = new System.Drawing.SizeF(800F, 600F);
			this.diagramControl.Size = new System.Drawing.Size(895, 779);
			this.diagramControl.TabIndex = 8;
			this.diagramControl.Text = "diagramControl1";
			this.diagramControl.Toolbox = this.diagramToolbox;
			this.mainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainSplitter.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
			this.mainSplitter.Location = new System.Drawing.Point(0, 0);
			this.mainSplitter.Name = "mainSplitter";
			this.mainSplitter.Panel1.Controls.Add(this.innerSplitter);
			this.mainSplitter.Panel1.Text = "Panel1";
			this.mainSplitter.Panel2.Controls.Add(this.designerPropertyGridControl);
			this.mainSplitter.Panel2.Text = "Panel2";
			this.mainSplitter.Size = new System.Drawing.Size(1463, 779);
			this.mainSplitter.SplitterPosition = 277;
			this.mainSplitter.TabIndex = 11;
			this.designerPropertyGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.designerPropertyGridControl.Location = new System.Drawing.Point(0, 0);
			this.designerPropertyGridControl.Name = "designerPropertyGridControl";
			this.designerPropertyGridControl.Size = new System.Drawing.Size(277, 779);
			this.designerPropertyGridControl.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainSplitter);
			this.Name = "DiagramDesignerControlBase";
			this.Size = new System.Drawing.Size(1463, 779);
			((System.ComponentModel.ISupportInitialize)(this.innerSplitter)).EndInit();
			this.innerSplitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.diagramControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).EndInit();
			this.mainSplitter.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DiagramControl diagramControl;
		private XtraToolbox.ToolboxControl diagramToolbox;
		private XtraEditors.SplitContainerControl innerSplitter;
		private XtraEditors.SplitContainerControl mainSplitter;
		private DiagramDesignerPropertyGridControl designerPropertyGridControl;
	}
}
