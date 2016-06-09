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

using DevExpress.XtraReports.Design;
namespace DevExpress.DataAccess.UI.Native {
	partial class PropertyGridForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.propertyGridControl = new DevExpress.XtraReports.Design.PropertyGridUserControl();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).BeginInit();
			this.panelContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).BeginInit();
			this.SuspendLayout();
			this.panelContent.Controls.Add(this.propertyGridControl);
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Controls.SetChildIndex(this.btnOK, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.btnCancel, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelContent, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelControlAdditionalButtons, 0);
			this.propertyGridControl.AllowGlyphSkinning = false;
			this.propertyGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridControl.Location = new System.Drawing.Point(0, 0);
			this.propertyGridControl.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGridControl.Name = "propertyGridControl";
			this.propertyGridControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.propertyGridControl.SelectedObject = null;
			this.propertyGridControl.SelectedObjects = new object[0];
			this.propertyGridControl.ServiceProvider = null;
			this.propertyGridControl.ShowCategories = true;
			this.propertyGridControl.ShowDescription = true;
			this.propertyGridControl.Size = new System.Drawing.Size(280, 222);
			this.propertyGridControl.TabIndex = 5;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(333, 345);
			this.Name = "PropertyGridForm";
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).EndInit();
			this.panelContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private PropertyGridUserControl propertyGridControl;
	}
}
