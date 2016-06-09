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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class RotateControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RotateControl));
			this.tbXAxisAngle = new DevExpress.XtraEditors.TrackBarControl();
			this.grAngles = new DevExpress.XtraEditors.GroupControl();
			this.pnlZ = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lbZ = new DevExpress.XtraEditors.LabelControl();
			this.tbZAxisAngle = new DevExpress.XtraEditors.TrackBarControl();
			this.pnlY = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lbY = new DevExpress.XtraEditors.LabelControl();
			this.tbYAxisAngle = new DevExpress.XtraEditors.TrackBarControl();
			this.pnlX = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lbX = new DevExpress.XtraEditors.LabelControl();
			this.sepPlaneTrackBar = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlPlane = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spaceOrder = new DevExpress.XtraEditors.LabelControl();
			this.cbRotationOrder = new DevExpress.XtraEditors.ComboBoxEdit();
			this.groupAngles = new DevExpress.XtraEditors.GroupControl();
			this.rgRotationType = new DevExpress.XtraEditors.RadioGroup();
			this.sepGroupTypeAngles = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepGroupAnglesOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupRotationOptions = new DevExpress.XtraEditors.GroupControl();
			this.chUseTouchDevice = new DevExpress.XtraEditors.CheckEdit();
			this.sepMouseTouch = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chUseMouse = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbXAxisAngle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbXAxisAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grAngles)).BeginInit();
			this.grAngles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlZ)).BeginInit();
			this.pnlZ.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbZAxisAngle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbZAxisAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlY)).BeginInit();
			this.pnlY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbYAxisAngle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbYAxisAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlX)).BeginInit();
			this.pnlX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPlaneTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPlane)).BeginInit();
			this.pnlPlane.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbRotationOrder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupAngles)).BeginInit();
			this.groupAngles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgRotationType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGroupTypeAngles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGroupAnglesOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupRotationOptions)).BeginInit();
			this.groupRotationOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chUseTouchDevice.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMouseTouch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chUseMouse.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tbXAxisAngle, "tbXAxisAngle");
			this.tbXAxisAngle.Name = "tbXAxisAngle";
			this.tbXAxisAngle.Properties.Maximum = 360;
			this.tbXAxisAngle.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
			this.tbXAxisAngle.EditValueChanged += new System.EventHandler(this.tbXAxisAngle_EditValueChanged);
			resources.ApplyResources(this.grAngles, "grAngles");
			this.grAngles.Controls.Add(this.pnlZ);
			this.grAngles.Controls.Add(this.pnlY);
			this.grAngles.Controls.Add(this.pnlX);
			this.grAngles.Controls.Add(this.sepPlaneTrackBar);
			this.grAngles.Controls.Add(this.pnlPlane);
			this.grAngles.Name = "grAngles";
			resources.ApplyResources(this.pnlZ, "pnlZ");
			this.pnlZ.BackColor = System.Drawing.Color.Transparent;
			this.pnlZ.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlZ.Controls.Add(this.lbZ);
			this.pnlZ.Controls.Add(this.tbZAxisAngle);
			this.pnlZ.Name = "pnlZ";
			resources.ApplyResources(this.lbZ, "lbZ");
			this.lbZ.Name = "lbZ";
			resources.ApplyResources(this.tbZAxisAngle, "tbZAxisAngle");
			this.tbZAxisAngle.Name = "tbZAxisAngle";
			this.tbZAxisAngle.Properties.Maximum = 360;
			this.tbZAxisAngle.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
			this.tbZAxisAngle.EditValueChanged += new System.EventHandler(this.tbZAxisAngle_EditValueChanged);
			resources.ApplyResources(this.pnlY, "pnlY");
			this.pnlY.BackColor = System.Drawing.Color.Transparent;
			this.pnlY.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlY.Controls.Add(this.lbY);
			this.pnlY.Controls.Add(this.tbYAxisAngle);
			this.pnlY.Name = "pnlY";
			resources.ApplyResources(this.lbY, "lbY");
			this.lbY.Name = "lbY";
			resources.ApplyResources(this.tbYAxisAngle, "tbYAxisAngle");
			this.tbYAxisAngle.Name = "tbYAxisAngle";
			this.tbYAxisAngle.Properties.Maximum = 360;
			this.tbYAxisAngle.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
			this.tbYAxisAngle.EditValueChanged += new System.EventHandler(this.tbYAxisAngle_EditValueChanged);
			this.pnlX.BackColor = System.Drawing.Color.Transparent;
			this.pnlX.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlX.Controls.Add(this.lbX);
			this.pnlX.Controls.Add(this.tbXAxisAngle);
			resources.ApplyResources(this.pnlX, "pnlX");
			this.pnlX.Name = "pnlX";
			resources.ApplyResources(this.lbX, "lbX");
			this.lbX.Name = "lbX";
			this.sepPlaneTrackBar.BackColor = System.Drawing.Color.Transparent;
			this.sepPlaneTrackBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepPlaneTrackBar, "sepPlaneTrackBar");
			this.sepPlaneTrackBar.Name = "sepPlaneTrackBar";
			resources.ApplyResources(this.pnlPlane, "pnlPlane");
			this.pnlPlane.BackColor = System.Drawing.Color.Transparent;
			this.pnlPlane.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPlane.Controls.Add(this.spaceOrder);
			this.pnlPlane.Controls.Add(this.cbRotationOrder);
			this.pnlPlane.Name = "pnlPlane";
			resources.ApplyResources(this.spaceOrder, "spaceOrder");
			this.spaceOrder.Name = "spaceOrder";
			resources.ApplyResources(this.cbRotationOrder, "cbRotationOrder");
			this.cbRotationOrder.Name = "cbRotationOrder";
			this.cbRotationOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRotationOrder.Properties.Buttons"))))});
			this.cbRotationOrder.Properties.Items.AddRange(new object[] {
			resources.GetString("cbRotationOrder.Properties.Items"),
			resources.GetString("cbRotationOrder.Properties.Items1"),
			resources.GetString("cbRotationOrder.Properties.Items2"),
			resources.GetString("cbRotationOrder.Properties.Items3"),
			resources.GetString("cbRotationOrder.Properties.Items4"),
			resources.GetString("cbRotationOrder.Properties.Items5")});
			this.cbRotationOrder.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbRotationOrder.SelectedIndexChanged += new System.EventHandler(this.cbRotationOrder_SelectedIndexChanged);
			resources.ApplyResources(this.groupAngles, "groupAngles");
			this.groupAngles.Controls.Add(this.rgRotationType);
			this.groupAngles.Name = "groupAngles";
			resources.ApplyResources(this.rgRotationType, "rgRotationType");
			this.rgRotationType.Name = "rgRotationType";
			this.rgRotationType.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgRotationType.Properties.Appearance.BackColor")));
			this.rgRotationType.Properties.Appearance.Options.UseBackColor = true;
			this.rgRotationType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgRotationType.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
			this.rgRotationType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgRotationType.Properties.Items"))), resources.GetString("rgRotationType.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgRotationType.Properties.Items2"))), resources.GetString("rgRotationType.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgRotationType.Properties.Items4"))), resources.GetString("rgRotationType.Properties.Items5"))});
			this.rgRotationType.SelectedIndexChanged += new System.EventHandler(this.rgRotationType_SelectedIndexChanged);
			this.sepGroupTypeAngles.BackColor = System.Drawing.Color.Transparent;
			this.sepGroupTypeAngles.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepGroupTypeAngles, "sepGroupTypeAngles");
			this.sepGroupTypeAngles.Name = "sepGroupTypeAngles";
			this.sepGroupAnglesOptions.BackColor = System.Drawing.Color.Transparent;
			this.sepGroupAnglesOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepGroupAnglesOptions, "sepGroupAnglesOptions");
			this.sepGroupAnglesOptions.Name = "sepGroupAnglesOptions";
			resources.ApplyResources(this.groupRotationOptions, "groupRotationOptions");
			this.groupRotationOptions.Controls.Add(this.chUseTouchDevice);
			this.groupRotationOptions.Controls.Add(this.sepMouseTouch);
			this.groupRotationOptions.Controls.Add(this.chUseMouse);
			this.groupRotationOptions.Name = "groupRotationOptions";
			resources.ApplyResources(this.chUseTouchDevice, "chUseTouchDevice");
			this.chUseTouchDevice.Name = "chUseTouchDevice";
			this.chUseTouchDevice.Properties.Caption = resources.GetString("chUseTouchDevice.Properties.Caption");
			this.chUseTouchDevice.CheckedChanged += new System.EventHandler(this.chUseTouchDevice_CheckedChanged);
			this.sepMouseTouch.BackColor = System.Drawing.Color.Transparent;
			this.sepMouseTouch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepMouseTouch, "sepMouseTouch");
			this.sepMouseTouch.Name = "sepMouseTouch";
			resources.ApplyResources(this.chUseMouse, "chUseMouse");
			this.chUseMouse.Name = "chUseMouse";
			this.chUseMouse.Properties.Caption = resources.GetString("chUseMouse.Properties.Caption");
			this.chUseMouse.CheckedChanged += new System.EventHandler(this.chUseMouse_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupRotationOptions);
			this.Controls.Add(this.sepGroupAnglesOptions);
			this.Controls.Add(this.grAngles);
			this.Controls.Add(this.sepGroupTypeAngles);
			this.Controls.Add(this.groupAngles);
			this.Name = "RotateControl";
			((System.ComponentModel.ISupportInitialize)(this.tbXAxisAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbXAxisAngle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grAngles)).EndInit();
			this.grAngles.ResumeLayout(false);
			this.grAngles.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlZ)).EndInit();
			this.pnlZ.ResumeLayout(false);
			this.pnlZ.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbZAxisAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbZAxisAngle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlY)).EndInit();
			this.pnlY.ResumeLayout(false);
			this.pnlY.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbYAxisAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbYAxisAngle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlX)).EndInit();
			this.pnlX.ResumeLayout(false);
			this.pnlX.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepPlaneTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPlane)).EndInit();
			this.pnlPlane.ResumeLayout(false);
			this.pnlPlane.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbRotationOrder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupAngles)).EndInit();
			this.groupAngles.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rgRotationType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGroupTypeAngles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepGroupAnglesOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupRotationOptions)).EndInit();
			this.groupRotationOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chUseTouchDevice.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMouseTouch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chUseMouse.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.TrackBarControl tbXAxisAngle;
		private DevExpress.XtraEditors.GroupControl grAngles;
		private ChartPanelControl pnlX;
		private ChartPanelControl pnlZ;
		private DevExpress.XtraEditors.TrackBarControl tbZAxisAngle;
		private ChartPanelControl pnlY;
		private DevExpress.XtraEditors.TrackBarControl tbYAxisAngle;
		private DevExpress.XtraEditors.GroupControl groupAngles;
		private ChartPanelControl sepPlaneTrackBar;
		private ChartPanelControl sepGroupTypeAngles;
		private ChartPanelControl pnlPlane;
		private DevExpress.XtraEditors.ComboBoxEdit cbRotationOrder;
		private DevExpress.XtraEditors.LabelControl spaceOrder;
		private DevExpress.XtraEditors.LabelControl lbZ;
		private DevExpress.XtraEditors.LabelControl lbY;
		private ChartPanelControl sepGroupAnglesOptions;
		private DevExpress.XtraEditors.GroupControl groupRotationOptions;
		private DevExpress.XtraEditors.CheckEdit chUseTouchDevice;
		private ChartPanelControl sepMouseTouch;
		private DevExpress.XtraEditors.CheckEdit chUseMouse;
		private DevExpress.XtraEditors.LabelControl lbX;
		private DevExpress.XtraEditors.RadioGroup rgRotationType;
	}
}
