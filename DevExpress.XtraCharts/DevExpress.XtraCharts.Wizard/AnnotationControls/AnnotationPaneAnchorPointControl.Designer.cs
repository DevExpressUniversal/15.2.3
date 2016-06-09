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
	partial class AnnotationPaneAnchorPointControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationPaneAnchorPointControl));
			this.grpAxisXCoordinate = new DevExpress.XtraEditors.GroupControl();
			this.axisXCoordinateControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AxisCoordinateControl();
			this.sepAxisCoordinates = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpAxisYCoordinate = new DevExpress.XtraEditors.GroupControl();
			this.axisYCoordinateControl = new DevExpress.XtraCharts.Wizard.AnnotationControls.AxisCoordinateControl();
			this.sepPane = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlPaneBack = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlPane = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPane = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPane = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpAxisXCoordinate)).BeginInit();
			this.grpAxisXCoordinate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepAxisCoordinates)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpAxisYCoordinate)).BeginInit();
			this.grpAxisYCoordinate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPane)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPaneBack)).BeginInit();
			this.pnlPaneBack.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPane)).BeginInit();
			this.pnlPane.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPane.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpAxisXCoordinate, "grpAxisXCoordinate");
			this.grpAxisXCoordinate.Controls.Add(this.axisXCoordinateControl);
			this.grpAxisXCoordinate.Name = "grpAxisXCoordinate";
			resources.ApplyResources(this.axisXCoordinateControl, "axisXCoordinateControl");
			this.axisXCoordinateControl.Name = "axisXCoordinateControl";
			this.sepAxisCoordinates.BackColor = System.Drawing.Color.Transparent;
			this.sepAxisCoordinates.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAxisCoordinates, "sepAxisCoordinates");
			this.sepAxisCoordinates.Name = "sepAxisCoordinates";
			resources.ApplyResources(this.grpAxisYCoordinate, "grpAxisYCoordinate");
			this.grpAxisYCoordinate.Controls.Add(this.axisYCoordinateControl);
			this.grpAxisYCoordinate.Name = "grpAxisYCoordinate";
			resources.ApplyResources(this.axisYCoordinateControl, "axisYCoordinateControl");
			this.axisYCoordinateControl.Name = "axisYCoordinateControl";
			this.sepPane.BackColor = System.Drawing.Color.Transparent;
			this.sepPane.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepPane, "sepPane");
			this.sepPane.Name = "sepPane";
			resources.ApplyResources(this.pnlPaneBack, "pnlPaneBack");
			this.pnlPaneBack.BackColor = System.Drawing.Color.Transparent;
			this.pnlPaneBack.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPaneBack.Controls.Add(this.pnlPane);
			this.pnlPaneBack.Name = "pnlPaneBack";
			resources.ApplyResources(this.pnlPane, "pnlPane");
			this.pnlPane.BackColor = System.Drawing.Color.Transparent;
			this.pnlPane.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPane.Controls.Add(this.cbPane);
			this.pnlPane.Controls.Add(this.lblPane);
			this.pnlPane.Name = "pnlPane";
			resources.ApplyResources(this.cbPane, "cbPane");
			this.cbPane.Name = "cbPane";
			this.cbPane.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPane.Properties.Buttons"))))});
			this.cbPane.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPane.SelectedIndexChanged += new System.EventHandler(this.cbPane_SelectedIndexChanged);
			resources.ApplyResources(this.lblPane, "lblPane");
			this.lblPane.Name = "lblPane";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlPaneBack);
			this.Controls.Add(this.sepPane);
			this.Controls.Add(this.grpAxisYCoordinate);
			this.Controls.Add(this.sepAxisCoordinates);
			this.Controls.Add(this.grpAxisXCoordinate);
			this.Name = "AnnotationPaneAnchorPointControl";
			((System.ComponentModel.ISupportInitialize)(this.grpAxisXCoordinate)).EndInit();
			this.grpAxisXCoordinate.ResumeLayout(false);
			this.grpAxisXCoordinate.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepAxisCoordinates)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpAxisYCoordinate)).EndInit();
			this.grpAxisYCoordinate.ResumeLayout(false);
			this.grpAxisYCoordinate.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPane)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPaneBack)).EndInit();
			this.pnlPaneBack.ResumeLayout(false);
			this.pnlPaneBack.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPane)).EndInit();
			this.pnlPane.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPane.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpAxisXCoordinate;
		private AxisCoordinateControl axisXCoordinateControl;
		private ChartPanelControl sepAxisCoordinates;
		private DevExpress.XtraEditors.GroupControl grpAxisYCoordinate;
		private AxisCoordinateControl axisYCoordinateControl;
		private ChartPanelControl sepPane;
		private ChartPanelControl pnlPaneBack;
		private ChartPanelControl pnlPane;
		private DevExpress.XtraEditors.ComboBoxEdit cbPane;
		private ChartLabelControl lblPane;
	}
}
