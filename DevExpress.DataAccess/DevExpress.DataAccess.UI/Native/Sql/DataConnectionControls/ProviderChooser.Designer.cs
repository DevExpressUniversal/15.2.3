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

using System.ComponentModel;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	partial class ProviderChooser {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
				OnDispose(disposing);
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProviderChooser));
			this.providerPanelControl = new DevExpress.XtraEditors.PanelControl();
			this.providersListLabel = new DevExpress.XtraEditors.LabelControl();
			this.providersList = new DevExpress.XtraEditors.ComboBoxEdit();
			this.contentPanelControl = new DevExpress.XtraEditors.PanelControl();
			this.connectionNameControl = new ConnectionNameControl();
			((System.ComponentModel.ISupportInitialize)(this.providerPanelControl)).BeginInit();
			this.providerPanelControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.providersList.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.contentPanelControl)).BeginInit();
			this.contentPanelControl.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.providerPanelControl, "providerPanelControl");
			this.providerPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.providerPanelControl.Controls.Add(this.providersListLabel);
			this.providerPanelControl.Controls.Add(this.providersList);
			this.providerPanelControl.Name = "providerPanelControl";
			resources.ApplyResources(this.providersListLabel, "providersListLabel");
			this.providersListLabel.Name = "providersListLabel";
			resources.ApplyResources(this.providersList, "providersList");
			this.providersList.Name = "providersList";
			this.providersList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("providersList.Properties.Buttons"))))});
			this.providersList.Properties.DropDownRows = 10;
			this.providersList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.providersList.SelectedValueChanged += new System.EventHandler(this.providersList_SelectedValueChanged);
			resources.ApplyResources(this.contentPanelControl, "contentPanelControl");
			this.contentPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.contentPanelControl.Controls.Add(this.connectionNameControl);
			this.contentPanelControl.Name = "contentPanelControl";
			this.connectionNameControl.ConnectionName = "";
			this.connectionNameControl.IsConnectionNameChanged = false;
			resources.ApplyResources(this.connectionNameControl, "connectionNameControl");
			this.connectionNameControl.Name = "connectionNameControl";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.contentPanelControl);
			this.Controls.Add(this.providerPanelControl);
			this.Name = "ProviderChooser";
			((System.ComponentModel.ISupportInitialize)(this.providerPanelControl)).EndInit();
			this.providerPanelControl.ResumeLayout(false);
			this.providerPanelControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.providersList.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.contentPanelControl)).EndInit();
			this.contentPanelControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private PanelControl providerPanelControl;
		private LabelControl providersListLabel;
		private ComboBoxEdit providersList;
		private PanelControl contentPanelControl;
		private ConnectionNameControl connectionNameControl;
	}
}
