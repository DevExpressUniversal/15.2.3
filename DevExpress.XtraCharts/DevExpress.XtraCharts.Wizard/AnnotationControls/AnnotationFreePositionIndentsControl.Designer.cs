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
	partial class AnnotationFreePositionIndentsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationFreePositionIndentsControl));
			this.grpOuterIndents = new DevExpress.XtraEditors.GroupControl();
			this.outerIndentsControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.sepInnerIndents = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpInnerIndents = new DevExpress.XtraEditors.GroupControl();
			this.innerIndentsControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			((System.ComponentModel.ISupportInitialize)(this.grpOuterIndents)).BeginInit();
			this.grpOuterIndents.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepInnerIndents)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpInnerIndents)).BeginInit();
			this.grpInnerIndents.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.grpOuterIndents, "grpOuterIndents");
			this.grpOuterIndents.Controls.Add(this.outerIndentsControl);
			this.grpOuterIndents.Name = "grpOuterIndents";
			resources.ApplyResources(this.outerIndentsControl, "outerIndentsControl");
			this.outerIndentsControl.Name = "outerIndentsControl";
			this.sepInnerIndents.BackColor = System.Drawing.Color.Transparent;
			this.sepInnerIndents.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepInnerIndents, "sepInnerIndents");
			this.sepInnerIndents.Name = "sepInnerIndents";
			resources.ApplyResources(this.grpInnerIndents, "grpInnerIndents");
			this.grpInnerIndents.Controls.Add(this.innerIndentsControl);
			this.grpInnerIndents.Name = "grpInnerIndents";
			resources.ApplyResources(this.innerIndentsControl, "innerIndentsControl");
			this.innerIndentsControl.Name = "innerIndentsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpOuterIndents);
			this.Controls.Add(this.sepInnerIndents);
			this.Controls.Add(this.grpInnerIndents);
			this.Name = "AnnotationFreePositionIndentsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpOuterIndents)).EndInit();
			this.grpOuterIndents.ResumeLayout(false);
			this.grpOuterIndents.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepInnerIndents)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpInnerIndents)).EndInit();
			this.grpInnerIndents.ResumeLayout(false);
			this.grpInnerIndents.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpOuterIndents;
		private RectangleIndentsControl outerIndentsControl;
		private ChartPanelControl sepInnerIndents;
		private DevExpress.XtraEditors.GroupControl grpInnerIndents;
		private RectangleIndentsControl innerIndentsControl;
	}
}
