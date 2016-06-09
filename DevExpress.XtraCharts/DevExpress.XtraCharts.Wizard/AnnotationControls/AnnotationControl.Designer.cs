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
	partial class AnnotationControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationControl));
			this.tbcPagesControl = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.annotationGeneralControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationGeneralControl();
			this.tbAnchorPoint = new DevExpress.XtraTab.XtraTabPage();
			this.annotationAnchorPointControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationAnchorPointControl();
			this.tbShapePosition = new DevExpress.XtraTab.XtraTabPage();
			this.annotationShapePositionControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationShapePositionControl();
			this.tbContent = new DevExpress.XtraTab.XtraTabPage();
			this.tbPadding = new DevExpress.XtraTab.XtraTabPage();
			this.paddingControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.annotationAppearanceControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationAppearanceControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.borderControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).BeginInit();
			this.tbcPagesControl.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			this.tbAnchorPoint.SuspendLayout();
			this.tbShapePosition.SuspendLayout();
			this.tbPadding.SuspendLayout();
			this.tbAppearance.SuspendLayout();
			this.tbBorder.SuspendLayout();
			this.tbShadow.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcPagesControl, "tbcPagesControl");
			this.tbcPagesControl.Name = "tbcPagesControl";
			this.tbcPagesControl.SelectedTabPage = this.tbGeneral;
			this.tbcPagesControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAnchorPoint,
			this.tbShapePosition,
			this.tbContent,
			this.tbPadding,
			this.tbAppearance,
			this.tbBorder,
			this.tbShadow});
			this.tbGeneral.Controls.Add(this.annotationGeneralControl);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.annotationGeneralControl, "annotationGeneralControl");
			this.annotationGeneralControl.Name = "annotationGeneralControl";
			this.annotationGeneralControl.UpdateLayoutCallback = null;
			this.tbAnchorPoint.Controls.Add(this.annotationAnchorPointControl);
			this.tbAnchorPoint.Name = "tbAnchorPoint";
			resources.ApplyResources(this.tbAnchorPoint, "tbAnchorPoint");
			resources.ApplyResources(this.annotationAnchorPointControl, "annotationAnchorPointControl");
			this.annotationAnchorPointControl.Name = "annotationAnchorPointControl";
			this.tbShapePosition.Controls.Add(this.annotationShapePositionControl);
			this.tbShapePosition.Name = "tbShapePosition";
			resources.ApplyResources(this.tbShapePosition, "tbShapePosition");
			resources.ApplyResources(this.annotationShapePositionControl, "annotationShapePositionControl");
			this.annotationShapePositionControl.Name = "annotationShapePositionControl";
			this.tbContent.Name = "tbContent";
			resources.ApplyResources(this.tbContent, "tbContent");
			this.tbPadding.Controls.Add(this.paddingControl);
			this.tbPadding.Name = "tbPadding";
			resources.ApplyResources(this.tbPadding, "tbPadding");
			resources.ApplyResources(this.paddingControl, "paddingControl");
			this.paddingControl.Name = "paddingControl";
			this.tbAppearance.Controls.Add(this.annotationAppearanceControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.annotationAppearanceControl, "annotationAppearanceControl");
			this.annotationAppearanceControl.Name = "annotationAppearanceControl";
			this.tbBorder.Controls.Add(this.borderControl);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			resources.ApplyResources(this.borderControl, "borderControl");
			this.borderControl.Name = "borderControl";
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.Name = "shadowControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcPagesControl);
			this.Name = "AnnotationControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPagesControl)).EndInit();
			this.tbcPagesControl.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			this.tbAnchorPoint.ResumeLayout(false);
			this.tbShapePosition.ResumeLayout(false);
			this.tbShapePosition.PerformLayout();
			this.tbPadding.ResumeLayout(false);
			this.tbPadding.PerformLayout();
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			this.tbBorder.PerformLayout();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcPagesControl;
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private DevExpress.XtraTab.XtraTabPage tbAnchorPoint;
		private DevExpress.XtraTab.XtraTabPage tbShapePosition;
		private DevExpress.XtraTab.XtraTabPage tbContent;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private AnnotationGeneralControl annotationGeneralControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl borderControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl shadowControl;
		private DevExpress.XtraTab.XtraTabPage tbPadding;
		private RectangleIndentsControl paddingControl;
		private AnnotationAppearanceControl annotationAppearanceControl;
		private AnnotationShapePositionControl annotationShapePositionControl;
		private AnnotationAnchorPointControl annotationAnchorPointControl;
	}
}
