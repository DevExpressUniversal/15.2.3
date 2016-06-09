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
	partial class AnnotationFreePositionControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationFreePositionControl));
			this.tbcPages = new DevExpress.XtraTab.XtraTabControl();
			this.tbDocking = new DevExpress.XtraTab.XtraTabPage();
			this.dockingControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationFreePositionDockingControl();
			this.tbIndents = new DevExpress.XtraTab.XtraTabPage();
			this.indentsControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AnnotationFreePositionIndentsControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcPages)).BeginInit();
			this.tbcPages.SuspendLayout();
			this.tbDocking.SuspendLayout();
			this.tbIndents.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcPages, "tbcPages");
			this.tbcPages.Name = "tbcPages";
			this.tbcPages.SelectedTabPage = this.tbDocking;
			this.tbcPages.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbDocking,
			this.tbIndents});
			this.tbDocking.Controls.Add(this.dockingControl);
			this.tbDocking.Name = "tbDocking";
			resources.ApplyResources(this.tbDocking, "tbDocking");
			resources.ApplyResources(this.dockingControl, "dockingControl");
			this.dockingControl.Name = "dockingControl";
			this.tbIndents.Controls.Add(this.indentsControl);
			this.tbIndents.Name = "tbIndents";
			resources.ApplyResources(this.tbIndents, "tbIndents");
			resources.ApplyResources(this.indentsControl, "indentsControl");
			this.indentsControl.Name = "indentsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcPages);
			this.Name = "AnnotationFreePositionControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcPages)).EndInit();
			this.tbcPages.ResumeLayout(false);
			this.tbDocking.ResumeLayout(false);
			this.tbDocking.PerformLayout();
			this.tbIndents.ResumeLayout(false);
			this.tbIndents.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcPages;
		private DevExpress.XtraTab.XtraTabPage tbIndents;
		private DevExpress.XtraTab.XtraTabPage tbDocking;
		private AnnotationFreePositionDockingControl dockingControl;
		private AnnotationFreePositionIndentsControl indentsControl;
	}
}
