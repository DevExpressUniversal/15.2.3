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

namespace DevExpress.XtraCharts.Wizard.ChartTitleControls {
	partial class TitleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitleControl));
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.tbText = new DevExpress.XtraTab.XtraTabPage();
			this.txtText = new DevExpress.XtraEditors.MemoEdit();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.titleGeneralControl = new DevExpress.XtraCharts.Wizard.ChartTitleControls.TitleGeneralControl();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.tbText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).BeginInit();
			this.tbGeneral.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.tbText;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbText,
			this.tbGeneral});
			this.tbText.Controls.Add(this.txtText);
			this.tbText.Name = "tbText";
			resources.ApplyResources(this.tbText, "tbText");
			resources.ApplyResources(this.txtText, "txtText");
			this.txtText.Name = "txtText";
			this.txtText.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtText.Properties.WordWrap = false;
			this.txtText.UseOptimizedRendering = true;
			this.txtText.EditValueChanged += new System.EventHandler(this.txtText_EditValueChanged);
			this.tbGeneral.Controls.Add(this.titleGeneralControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.titleGeneralControl, "titleGeneralControl");
			this.titleGeneralControl.Name = "titleGeneralControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.xtraTabControl1);
			this.Name = "TitleControl";
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.tbText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).EndInit();
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbText;
		private DevExpress.XtraEditors.MemoEdit txtText;
		protected DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private TitleGeneralControl titleGeneralControl;
	}
}
