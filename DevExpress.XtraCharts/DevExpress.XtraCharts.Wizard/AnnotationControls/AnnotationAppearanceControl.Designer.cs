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
	partial class AnnotationAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationAppearanceControl));
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.sepBackground = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpShape = new DevExpress.XtraEditors.GroupControl();
			this.pnlShapeFillet = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnShapeFillet = new DevExpress.XtraEditors.SpinEdit();
			this.lblShapeFillet = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepShapeKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlShapeKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbShapeKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblShapeKind = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepShape = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpConnector = new DevExpress.XtraEditors.GroupControl();
			this.pnlConnectorStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbConnectorStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblConnectorStyle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.sepBackground)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpShape)).BeginInit();
			this.grpShape.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapeFillet)).BeginInit();
			this.pnlShapeFillet.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnShapeFillet.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShapeKind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapeKind)).BeginInit();
			this.pnlShapeKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbShapeKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShape)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpConnector)).BeginInit();
			this.grpConnector.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlConnectorStyle)).BeginInit();
			this.pnlConnectorStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbConnectorStyle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.sepBackground.BackColor = System.Drawing.Color.Transparent;
			this.sepBackground.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepBackground, "sepBackground");
			this.sepBackground.Name = "sepBackground";
			resources.ApplyResources(this.grpShape, "grpShape");
			this.grpShape.Controls.Add(this.pnlShapeFillet);
			this.grpShape.Controls.Add(this.sepShapeKind);
			this.grpShape.Controls.Add(this.pnlShapeKind);
			this.grpShape.Name = "grpShape";
			resources.ApplyResources(this.pnlShapeFillet, "pnlShapeFillet");
			this.pnlShapeFillet.BackColor = System.Drawing.Color.Transparent;
			this.pnlShapeFillet.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlShapeFillet.Controls.Add(this.spnShapeFillet);
			this.pnlShapeFillet.Controls.Add(this.lblShapeFillet);
			this.pnlShapeFillet.Name = "pnlShapeFillet";
			resources.ApplyResources(this.spnShapeFillet, "spnShapeFillet");
			this.spnShapeFillet.Name = "spnShapeFillet";
			this.spnShapeFillet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnShapeFillet.Properties.IsFloatValue = false;
			this.spnShapeFillet.Properties.Mask.EditMask = resources.GetString("spnShapeFillet.Properties.Mask.EditMask");
			this.spnShapeFillet.Properties.MaxValue = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.spnShapeFillet.EditValueChanged += new System.EventHandler(this.spnShapeFillet_EditValueChanged);
			resources.ApplyResources(this.lblShapeFillet, "lblShapeFillet");
			this.lblShapeFillet.Name = "lblShapeFillet";
			this.sepShapeKind.BackColor = System.Drawing.Color.Transparent;
			this.sepShapeKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepShapeKind, "sepShapeKind");
			this.sepShapeKind.Name = "sepShapeKind";
			resources.ApplyResources(this.pnlShapeKind, "pnlShapeKind");
			this.pnlShapeKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlShapeKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlShapeKind.Controls.Add(this.cbShapeKind);
			this.pnlShapeKind.Controls.Add(this.lblShapeKind);
			this.pnlShapeKind.Name = "pnlShapeKind";
			resources.ApplyResources(this.cbShapeKind, "cbShapeKind");
			this.cbShapeKind.Name = "cbShapeKind";
			this.cbShapeKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbShapeKind.Properties.Buttons"))))});
			this.cbShapeKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbShapeKind.SelectedIndexChanged += new System.EventHandler(this.cbShapeKind_SelectedIndexChanged);
			resources.ApplyResources(this.lblShapeKind, "lblShapeKind");
			this.lblShapeKind.Name = "lblShapeKind";
			this.sepShape.BackColor = System.Drawing.Color.Transparent;
			this.sepShape.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepShape, "sepShape");
			this.sepShape.Name = "sepShape";
			resources.ApplyResources(this.grpConnector, "grpConnector");
			this.grpConnector.Controls.Add(this.pnlConnectorStyle);
			this.grpConnector.Name = "grpConnector";
			resources.ApplyResources(this.pnlConnectorStyle, "pnlConnectorStyle");
			this.pnlConnectorStyle.BackColor = System.Drawing.Color.Transparent;
			this.pnlConnectorStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlConnectorStyle.Controls.Add(this.cbConnectorStyle);
			this.pnlConnectorStyle.Controls.Add(this.lblConnectorStyle);
			this.pnlConnectorStyle.Name = "pnlConnectorStyle";
			resources.ApplyResources(this.cbConnectorStyle, "cbConnectorStyle");
			this.cbConnectorStyle.Name = "cbConnectorStyle";
			this.cbConnectorStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbConnectorStyle.Properties.Buttons"))))});
			this.cbConnectorStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbConnectorStyle.SelectedIndexChanged += new System.EventHandler(this.cbConnectorStyle_SelectedIndexChanged);
			resources.ApplyResources(this.lblConnectorStyle, "lblConnectorStyle");
			this.lblConnectorStyle.Name = "lblConnectorStyle";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpConnector);
			this.Controls.Add(this.sepShape);
			this.Controls.Add(this.grpShape);
			this.Controls.Add(this.sepBackground);
			this.Controls.Add(this.backgroundControl);
			this.Name = "AnnotationAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.sepBackground)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpShape)).EndInit();
			this.grpShape.ResumeLayout(false);
			this.grpShape.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapeFillet)).EndInit();
			this.pnlShapeFillet.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnShapeFillet.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShapeKind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapeKind)).EndInit();
			this.pnlShapeKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbShapeKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepShape)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpConnector)).EndInit();
			this.grpConnector.ResumeLayout(false);
			this.grpConnector.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlConnectorStyle)).EndInit();
			this.pnlConnectorStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbConnectorStyle.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private BackgroundControl backgroundControl;
		private ChartPanelControl sepBackground;
		private DevExpress.XtraEditors.GroupControl grpShape;
		private ChartPanelControl pnlShapeKind;
		private DevExpress.XtraEditors.ComboBoxEdit cbShapeKind;
		private ChartLabelControl lblShapeKind;
		private ChartPanelControl pnlShapeFillet;
		private DevExpress.XtraEditors.SpinEdit spnShapeFillet;
		private ChartLabelControl lblShapeFillet;
		private ChartPanelControl sepShapeKind;
		private ChartPanelControl sepShape;
		private DevExpress.XtraEditors.GroupControl grpConnector;
		private ChartPanelControl pnlConnectorStyle;
		private DevExpress.XtraEditors.ComboBoxEdit cbConnectorStyle;
		private ChartLabelControl lblConnectorStyle;
	}
}
