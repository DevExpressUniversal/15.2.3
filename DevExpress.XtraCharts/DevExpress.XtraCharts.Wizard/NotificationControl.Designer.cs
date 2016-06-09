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
	partial class NotificationControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationControl));
			this.dcText = new DevExpress.XtraEditors.GroupControl();
			this.txtText = new DevExpress.XtraEditors.MemoEdit();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grAppearance = new DevExpress.XtraEditors.GroupControl();
			this.textAppearanceControl = new DevExpress.XtraCharts.Wizard.TextAppearanceControl();
			((System.ComponentModel.ISupportInitialize)(this.dcText)).BeginInit();
			this.dcText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grAppearance)).BeginInit();
			this.grAppearance.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.dcText, "dcText");
			this.dcText.Controls.Add(this.txtText);
			this.dcText.Name = "dcText";
			resources.ApplyResources(this.txtText, "txtText");
			this.txtText.Name = "txtText";
			this.txtText.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtText.Properties.WordWrap = false;
			this.txtText.UseOptimizedRendering = true;
			this.txtText.EditValueChanged += new System.EventHandler(this.txtText_EditValueChanged);
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.grAppearance, "grAppearance");
			this.grAppearance.Controls.Add(this.textAppearanceControl);
			this.grAppearance.Name = "grAppearance";
			resources.ApplyResources(this.textAppearanceControl, "textAppearanceControl");
			this.textAppearanceControl.Name = "textAppearanceControl";
			resources.ApplyResources(this, "$this");
			this.CausesValidation = false;
			this.Controls.Add(this.dcText);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.grAppearance);
			this.Name = "NotificationControl";
			((System.ComponentModel.ISupportInitialize)(this.dcText)).EndInit();
			this.dcText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grAppearance)).EndInit();
			this.grAppearance.ResumeLayout(false);
			this.grAppearance.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl dcText;
		private DevExpress.XtraEditors.MemoEdit txtText;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelControl2;
		private DevExpress.XtraEditors.GroupControl grAppearance;
		private TextAppearanceControl textAppearanceControl;
	}
}
