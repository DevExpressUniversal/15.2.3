#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin {
	partial class DashboardDesigner {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardDesigner));
			this.splitParent = new DevExpress.XtraEditors.SplitContainerControl();
			this.splitChild = new DevExpress.XtraEditors.SplitContainerControl();
			this.dashboardViewer = new DevExpress.DashboardWin.DashboardViewer(this.components);
			this.dashboardBarAndDockingController = new DevExpress.DashboardWin.Native.DashboardBarAndDockingController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitParent)).BeginInit();
			this.splitParent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitChild)).BeginInit();
			this.splitChild.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dashboardViewer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dashboardBarAndDockingController)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splitParent, "splitParent");
			this.splitParent.Name = "splitParent";
			resources.ApplyResources(this.splitParent.Panel1, "splitParent.Panel1");
			this.splitParent.Panel2.Controls.Add(this.splitChild);
			resources.ApplyResources(this.splitParent.Panel2, "splitParent.Panel2");
			resources.ApplyResources(this.splitChild, "splitChild");
			this.splitChild.Name = "splitChild";
			resources.ApplyResources(this.splitChild.Panel1, "splitChild.Panel1");
			this.splitChild.Panel2.Controls.Add(this.dashboardViewer);
			resources.ApplyResources(this.splitChild.Panel2, "splitChild.Panel2");
			resources.ApplyResources(this.dashboardViewer, "dashboardViewer");
			this.dashboardViewer.Name = "dashboardViewer";
			this.dashboardViewer.PrintingOptions.DocumentContentOptions.FilterState = DevExpress.DashboardWin.DashboardPrintingFilterState.SeparatePage;
			this.dashboardBarAndDockingController.PropertiesBar.AllowLinkLighting = false;
			this.dashboardBarAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.dashboardBarAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitParent);
			this.Name = "DashboardDesigner";
			((System.ComponentModel.ISupportInitialize)(this.splitParent)).EndInit();
			this.splitParent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitChild)).EndInit();
			this.splitChild.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dashboardViewer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dashboardBarAndDockingController)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.DashboardWin.Native.DashboardBarAndDockingController dashboardBarAndDockingController;
		private DevExpress.XtraEditors.SplitContainerControl splitParent;
		private DashboardViewer dashboardViewer;
		private DevExpress.XtraEditors.SplitContainerControl splitChild;
	}
}
