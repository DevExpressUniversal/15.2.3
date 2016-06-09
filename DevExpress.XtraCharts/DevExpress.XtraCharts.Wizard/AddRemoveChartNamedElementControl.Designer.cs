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
	partial class AddRemoveChartNamedElementControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddRemoveChartNamedElementControl));
			this.cbRemove = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAdd = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbElement = new DevExpress.XtraCharts.Wizard.ChartComboBox();
			this.panelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbElement.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
			this.panelControl6.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.cbRemove, "cbRemove");
			this.cbRemove.Name = "cbRemove";
			this.cbRemove.Click += new System.EventHandler(this.cbRemove_Click);
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.cbAdd);
			this.panelControl2.Controls.Add(this.panelControl1);
			this.panelControl2.Controls.Add(this.cbRemove);
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.cbAdd, "cbAdd");
			this.cbAdd.Name = "cbAdd";
			this.cbAdd.Click += new System.EventHandler(this.cbAdd_Click);
			this.panelControl1.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.cbElement, "cbElement");
			this.cbElement.Name = "cbElement";
			this.cbElement.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbElement.Properties.Buttons"))))});
			this.cbElement.Properties.DropDownRows = 10;
			this.cbElement.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbElement.SelectedIndexChanged += new System.EventHandler(this.cbAxes_SelectedIndexChanged);
			this.panelControl6.BackColor = System.Drawing.Color.Transparent;
			this.panelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl6.Controls.Add(this.labelControl1);
			resources.ApplyResources(this.panelControl6, "panelControl6");
			this.panelControl6.Name = "panelControl6";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.cbElement);
			this.Controls.Add(this.panelControl6);
			this.Controls.Add(this.panelControl2);
			this.Name = "AddRemoveChartNamedElementControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbElement.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
			this.panelControl6.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton cbRemove;
		private ChartPanelControl panelControl2;
		protected ChartComboBox cbElement;
		private ChartPanelControl panelControl1;
		public ChartLabelControl labelControl1;
		public ChartPanelControl panelControl6;
		protected DevExpress.XtraEditors.SimpleButton cbAdd;
	}
}
