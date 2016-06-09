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

namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	partial class AnnotationTextContentControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationTextContentControl));
			this.grpAppearance = new DevExpress.XtraEditors.GroupControl();
			this.textAppearanceControl = new DevExpress.XtraCharts.Wizard.TextAppearanceControl();
			this.sepPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpPosition = new DevExpress.XtraEditors.GroupControl();
			this.pnlAlignment = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAlignment = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblTextAlignment = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepText = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpText = new DevExpress.XtraEditors.GroupControl();
			this.memoText = new DevExpress.XtraEditors.MemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpAppearance)).BeginInit();
			this.grpAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPosition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPosition)).BeginInit();
			this.grpPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).BeginInit();
			this.pnlAlignment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpText)).BeginInit();
			this.grpText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.memoText.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpAppearance, "grpAppearance");
			this.grpAppearance.Controls.Add(this.textAppearanceControl);
			this.grpAppearance.Name = "grpAppearance";
			resources.ApplyResources(this.textAppearanceControl, "textAppearanceControl");
			this.textAppearanceControl.Name = "textAppearanceControl";
			this.sepPosition.BackColor = System.Drawing.Color.Transparent;
			this.sepPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepPosition, "sepPosition");
			this.sepPosition.Name = "sepPosition";
			resources.ApplyResources(this.grpPosition, "grpPosition");
			this.grpPosition.Controls.Add(this.pnlAlignment);
			this.grpPosition.Name = "grpPosition";
			resources.ApplyResources(this.pnlAlignment, "pnlAlignment");
			this.pnlAlignment.BackColor = System.Drawing.Color.Transparent;
			this.pnlAlignment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAlignment.Controls.Add(this.cbAlignment);
			this.pnlAlignment.Controls.Add(this.lblTextAlignment);
			this.pnlAlignment.Name = "pnlAlignment";
			resources.ApplyResources(this.cbAlignment, "cbAlignment");
			this.cbAlignment.Name = "cbAlignment";
			this.cbAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAlignment.Properties.Buttons"))))});
			this.cbAlignment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAlignment.SelectedIndexChanged += new System.EventHandler(this.cbAlignment_SelectedIndexChanged);
			resources.ApplyResources(this.lblTextAlignment, "lblTextAlignment");
			this.lblTextAlignment.Name = "lblTextAlignment";
			this.sepText.BackColor = System.Drawing.Color.Transparent;
			this.sepText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepText, "sepText");
			this.sepText.Name = "sepText";
			this.grpText.Controls.Add(this.memoText);
			resources.ApplyResources(this.grpText, "grpText");
			this.grpText.Name = "grpText";
			resources.ApplyResources(this.memoText, "memoText");
			this.memoText.Name = "memoText";
			this.memoText.UseOptimizedRendering = true;
			this.memoText.EditValueChanged += new System.EventHandler(this.memoText_EditValueChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpText);
			this.Controls.Add(this.sepText);
			this.Controls.Add(this.grpPosition);
			this.Controls.Add(this.sepPosition);
			this.Controls.Add(this.grpAppearance);
			this.Name = "AnnotationTextContentControl";
			((System.ComponentModel.ISupportInitialize)(this.grpAppearance)).EndInit();
			this.grpAppearance.ResumeLayout(false);
			this.grpAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPosition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPosition)).EndInit();
			this.grpPosition.ResumeLayout(false);
			this.grpPosition.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlAlignment)).EndInit();
			this.pnlAlignment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpText)).EndInit();
			this.grpText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.memoText.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpAppearance;
		private TextAppearanceControl textAppearanceControl;
		private ChartPanelControl sepPosition;
		private DevExpress.XtraEditors.GroupControl grpPosition;
		private ChartPanelControl pnlAlignment;
		private DevExpress.XtraEditors.ComboBoxEdit cbAlignment;
		private ChartLabelControl lblTextAlignment;
		private ChartPanelControl sepText;
		private DevExpress.XtraEditors.GroupControl grpText;
		private DevExpress.XtraEditors.MemoEdit memoText;
	}
}
