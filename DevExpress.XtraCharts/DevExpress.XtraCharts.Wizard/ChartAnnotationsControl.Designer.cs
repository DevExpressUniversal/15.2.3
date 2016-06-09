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

namespace DevExpress.XtraCharts.Wizard {
	partial class ChartAnnotationsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartAnnotationsControl));
			this.panel1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.annotationListRedactControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationListRedactControl();
			this.panel2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.separator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.annotationControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationControl();
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel1)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chartPanel, "chartPanel");
			this.splitter.Panel2.Controls.Add(this.annotationControl);
			this.splitter.Panel2.Controls.Add(this.separator);
			this.splitter.Panel2.Controls.Add(this.panel1);
			resources.ApplyResources(this.splitter, "splitter");
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel1.Controls.Add(this.annotationListRedactControl);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Name = "panel1";
			resources.ApplyResources(this.annotationListRedactControl, "annotationListRedactControl");
			this.annotationListRedactControl.Name = "annotationListRedactControl";
			this.annotationListRedactControl.SelectedElementChanged += new DevExpress.XtraCharts.Wizard.SelectedElementChangedEventHandler(this.annotationListRedactControl_SelectedElementChanged);
			this.panel2.BackColor = System.Drawing.Color.Transparent;
			this.panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			this.separator.BackColor = System.Drawing.Color.Transparent;
			this.separator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator, "separator");
			this.separator.Name = "separator";
			resources.ApplyResources(this.annotationControl, "annotationControl");
			this.annotationControl.Name = "annotationControl";
			resources.ApplyResources(this, "$this");
			this.Name = "ChartAnnotationsControl";
			((System.ComponentModel.ISupportInitialize)(this.chartPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panel1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl separator;
		private ChartPanelControl panel1;
		private ChartPanelControl panel2;
		private DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationListRedactControl annotationListRedactControl;
		private DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationControl annotationControl;
	}
}
