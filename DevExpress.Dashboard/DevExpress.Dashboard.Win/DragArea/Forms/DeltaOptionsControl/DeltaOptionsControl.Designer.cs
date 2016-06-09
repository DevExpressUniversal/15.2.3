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

namespace DevExpress.DashboardWin.Native {
	partial class DeltaOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeltaOptionsControl));
			this.cbValueType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelValueType = new DevExpress.XtraEditors.LabelControl();
			this.labelResultIndication = new DevExpress.XtraEditors.LabelControl();
			this.cbResultIndication = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelThresholdType = new DevExpress.XtraEditors.LabelControl();
			this.editThresholdValue = new DevExpress.XtraEditors.SpinEdit();
			this.labelThresholdValue = new DevExpress.XtraEditors.LabelControl();
			this.cbThresholdType = new DevExpress.XtraEditors.ComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbValueType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResultIndication.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editThresholdValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbThresholdType.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cbValueType, "cbValueType");
			this.cbValueType.Name = "cbValueType";
			this.cbValueType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbValueType.Properties.Buttons"))))});
			this.cbValueType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbValueType.SelectedValueChanged += new System.EventHandler(this.OnValueTypeChanged);
			resources.ApplyResources(this.labelValueType, "labelValueType");
			this.labelValueType.Name = "labelValueType";
			resources.ApplyResources(this.labelResultIndication, "labelResultIndication");
			this.labelResultIndication.Name = "labelResultIndication";
			resources.ApplyResources(this.cbResultIndication, "cbResultIndication");
			this.cbResultIndication.Name = "cbResultIndication";
			this.cbResultIndication.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbResultIndication.Properties.Buttons"))))});
			this.cbResultIndication.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbResultIndication.SelectedValueChanged += new System.EventHandler(this.OnResultIndicationChanged);
			resources.ApplyResources(this.labelThresholdType, "labelThresholdType");
			this.labelThresholdType.Name = "labelThresholdType";
			resources.ApplyResources(this.editThresholdValue, "editThresholdValue");
			this.editThresholdValue.Name = "editThresholdValue";
			this.editThresholdValue.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.editThresholdValue.EditValueChanged += new System.EventHandler(this.OnThresholdValueChanged);
			resources.ApplyResources(this.labelThresholdValue, "labelThresholdValue");
			this.labelThresholdValue.Name = "labelThresholdValue";
			resources.ApplyResources(this.cbThresholdType, "cbThresholdType");
			this.cbThresholdType.Name = "cbThresholdType";
			this.cbThresholdType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbThresholdType.Properties.Buttons"))))});
			this.cbThresholdType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbThresholdType.SelectedValueChanged += new System.EventHandler(this.OnThresholdTypeChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbThresholdType);
			this.Controls.Add(this.labelThresholdValue);
			this.Controls.Add(this.editThresholdValue);
			this.Controls.Add(this.cbResultIndication);
			this.Controls.Add(this.labelThresholdType);
			this.Controls.Add(this.labelResultIndication);
			this.Controls.Add(this.cbValueType);
			this.Controls.Add(this.labelValueType);
			this.Name = "DeltaOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.cbValueType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbResultIndication.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editThresholdValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbThresholdType.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.ComboBoxEdit cbValueType;
		private DevExpress.XtraEditors.LabelControl labelValueType;
		private DevExpress.XtraEditors.LabelControl labelResultIndication;
		private DevExpress.XtraEditors.ComboBoxEdit cbResultIndication;
		private DevExpress.XtraEditors.LabelControl labelThresholdType;
		private DevExpress.XtraEditors.SpinEdit editThresholdValue;
		private DevExpress.XtraEditors.LabelControl labelThresholdValue;
		private DevExpress.XtraEditors.ComboBoxEdit cbThresholdType;
	}
}
