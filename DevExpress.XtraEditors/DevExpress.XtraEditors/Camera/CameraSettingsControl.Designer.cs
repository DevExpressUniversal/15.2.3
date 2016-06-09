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

namespace DevExpress.XtraEditors.Camera {
	partial class CameraSettingsControl {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraSettingsControl));
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.mainPanel = new DevExpress.XtraEditors.PanelControl();
			this.simpleButtonDefaults = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButtonClose = new DevExpress.XtraEditors.SimpleButton();
			this.labDesaturate = new DevExpress.XtraEditors.LabelControl();
			this.labActiveDevice = new DevExpress.XtraEditors.LabelControl();
			this.ceDesaturate = new DevExpress.XtraEditors.CheckEdit();
			this.labContrast = new DevExpress.XtraEditors.LabelControl();
			this.tbContrast = new DevExpress.XtraEditors.TrackBarControl();
			this.labBrightness = new DevExpress.XtraEditors.LabelControl();
			this.cbDevices = new DevExpress.XtraEditors.ComboBoxEdit();
			this.tbBrightness = new DevExpress.XtraEditors.TrackBarControl();
			this.cbResolution = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labResolution = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceDesaturate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContrast)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContrast.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDevices.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResolution.Properties)).BeginInit();
			this.SuspendLayout();
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.simpleButtonDefaults);
			this.panelControl1.Controls.Add(this.simpleButtonClose);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			this.mainPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.mainPanel.Controls.Add(this.labResolution);
			this.mainPanel.Controls.Add(this.cbResolution);
			this.mainPanel.Controls.Add(this.labDesaturate);
			this.mainPanel.Controls.Add(this.labActiveDevice);
			this.mainPanel.Controls.Add(this.ceDesaturate);
			this.mainPanel.Controls.Add(this.labContrast);
			this.mainPanel.Controls.Add(this.tbContrast);
			this.mainPanel.Controls.Add(this.labBrightness);
			this.mainPanel.Controls.Add(this.cbDevices);
			this.mainPanel.Controls.Add(this.tbBrightness);
			resources.ApplyResources(this.mainPanel, "mainPanel");
			this.mainPanel.Name = "mainPanel";
			resources.ApplyResources(this.simpleButtonDefaults, "simpleButtonDefaults");
			this.simpleButtonDefaults.Name = "simpleButtonDefaults";
			this.simpleButtonDefaults.Click += new System.EventHandler(this.simpleButtonDefaults_Click);
			resources.ApplyResources(this.simpleButtonClose, "simpleButtonClose");
			this.simpleButtonClose.Name = "simpleButtonClose";
			this.simpleButtonClose.Click += new System.EventHandler(this.simpleButtonClose_Click);
			resources.ApplyResources(this.labDesaturate, "labDesaturate");
			this.labDesaturate.Name = "labDesaturate";
			resources.ApplyResources(this.labActiveDevice, "labActiveDevice");
			this.labActiveDevice.Name = "labActiveDevice";
			resources.ApplyResources(this.ceDesaturate, "ceDesaturate");
			this.ceDesaturate.Name = "ceDesaturate";
			this.ceDesaturate.Properties.Caption = resources.GetString("ceDesaturate.Properties.Caption");
			resources.ApplyResources(this.labContrast, "labContrast");
			this.labContrast.Name = "labContrast";
			resources.ApplyResources(this.tbContrast, "tbContrast");
			this.tbContrast.Name = "tbContrast";
			this.tbContrast.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.tbContrast.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tbContrast.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
			resources.ApplyResources(this.labBrightness, "labBrightness");
			this.labBrightness.Name = "labBrightness";
			resources.ApplyResources(this.cbDevices, "cbDevices");
			this.cbDevices.Name = "cbDevices";
			this.cbDevices.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDevices.Properties.Buttons"))))});
			this.cbDevices.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDevices.EditValueChanged += new System.EventHandler(this.cbDevices_EditValueChanged);
			resources.ApplyResources(this.tbBrightness, "tbBrightness");
			this.tbBrightness.Name = "tbBrightness";
			this.tbBrightness.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.tbBrightness.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tbBrightness.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
			resources.ApplyResources(this.cbResolution, "cbResolution");
			this.cbResolution.Name = "cbResolution";
			this.cbResolution.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbResolution.Properties.Buttons"))))});
			this.cbResolution.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.labResolution, "labResolution");
			this.labResolution.Name = "labResolution";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.panelControl1);
			this.Name = "CameraSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceDesaturate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContrast.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContrast)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDevices.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResolution.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private TrackBarControl tbBrightness;
		private ComboBoxEdit cbDevices;
		private LabelControl labContrast;
		private LabelControl labBrightness;
		private TrackBarControl tbContrast;
		private SimpleButton simpleButtonDefaults;
		private CheckEdit ceDesaturate;
		private SimpleButton simpleButtonClose;
		private PanelControl panelControl1;
		private PanelControl mainPanel;
		private LabelControl labDesaturate;
		private LabelControl labActiveDevice;
		private LabelControl labResolution;
		private ComboBoxEdit cbResolution;
	}
}
