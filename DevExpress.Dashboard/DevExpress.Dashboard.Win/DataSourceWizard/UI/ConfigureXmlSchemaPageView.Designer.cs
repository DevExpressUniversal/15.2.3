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

using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.DataSourceWizard {
	partial class ConfigureXmlSchemaPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureXmlSchemaPageView));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.xmlSchemaEditorControl = new DevExpress.Utils.UI.XmlSchemaEditorControl();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			resources.ApplyResources(this.layoutControlBase, "layoutControlBase");
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutGroupBase.Size = new System.Drawing.Size(610, 424);
			resources.ApplyResources(this.buttonNext, "buttonNext");
			resources.ApplyResources(this.buttonFinish, "buttonFinish");
			this.layoutItemFinishButton.Location = new System.Drawing.Point(515, 376);
			this.layoutItemNextButton.Location = new System.Drawing.Point(424, 376);
			this.panelBaseContent.Controls.Add(this.xmlSchemaEditorControl);
			this.panelBaseContent.Controls.Add(this.labelControl1);
			resources.ApplyResources(this.panelBaseContent, "panelBaseContent");
			this.layoutItemBaseContentPanel.Size = new System.Drawing.Size(610, 336);
			this.layoutItemAdditionalButtons.Location = new System.Drawing.Point(0, 376);
			resources.ApplyResources(this.panelAdditionalButtons, "panelAdditionalButtons");
			this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.xmlSchemaEditorControl, "xmlSchemaEditorControl");
			this.xmlSchemaEditorControl.Name = "xmlSchemaEditorControl";
			resources.ApplyResources(this, "$this");
			this.Name = "ConfigureXmlSchemaPageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XmlSchemaEditorControl xmlSchemaEditorControl;
	}
}
