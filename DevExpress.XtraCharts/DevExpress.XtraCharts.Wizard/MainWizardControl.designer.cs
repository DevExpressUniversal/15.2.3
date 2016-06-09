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

using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard {
	partial class MainWizardControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWizardControl));
			this.nbWizard = new DevExpress.XtraNavBar.NavBarControl();
			this.grWizardParentControl = new DevExpress.XtraEditors.GroupControl();
			this.pnlParent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.panelCaption = new DevExpress.XtraEditors.PanelControl();
			this.labelCaption = new DevExpress.XtraEditors.LabelControl();
			this.pnlHintOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlHintContainer = new DevExpress.XtraEditors.PanelControl();
			this.pnlHint = new DevExpress.XtraCharts.Wizard.ChartHintPanel();
			this.panelNavigation = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.workPanel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.nbWizard)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grWizardParentControl)).BeginInit();
			this.grWizardParentControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlParent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelCaption)).BeginInit();
			this.panelCaption.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlHintOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlHintContainer)).BeginInit();
			this.pnlHintContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelNavigation)).BeginInit();
			this.panelNavigation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.workPanel)).BeginInit();
			this.workPanel.SuspendLayout();
			this.SuspendLayout();
			this.nbWizard.ActiveGroup = null;
			this.nbWizard.LinkSelectionMode = XtraNavBar.LinkSelectionModeType.OneInControl;
			this.nbWizard.ContentButtonHint = null;
			resources.ApplyResources(this.nbWizard, "nbWizard");
			this.nbWizard.DragDropFlags = DevExpress.XtraNavBar.NavBarDragDrop.None;
			this.nbWizard.Name = "nbWizard";
			this.nbWizard.OptionsNavPane.ExpandedWidth = ((int)(resources.GetObject("resource.ExpandedWidth")));
			this.nbWizard.StoreDefaultPaintStyleName = true;
			this.nbWizard.SelectedLinkChanged += new DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventHandler(this.nbWizard_SelectedLinkChanged);
			this.nbWizard.LinkPressed += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.nbWizard_LinkPressed);
			this.grWizardParentControl.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("grWizardParentControl.AppearanceCaption.Font")));
			this.grWizardParentControl.AppearanceCaption.Options.UseBorderColor = true;
			this.grWizardParentControl.AppearanceCaption.Options.UseFont = true;
			this.grWizardParentControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.grWizardParentControl.Controls.Add(this.pnlParent);
			this.grWizardParentControl.Controls.Add(this.chartPanelControl1);
			this.grWizardParentControl.Controls.Add(this.labelControl1);
			this.grWizardParentControl.Controls.Add(this.panelCaption);
			this.grWizardParentControl.Controls.Add(this.pnlHintOffset);
			this.grWizardParentControl.Controls.Add(this.pnlHintContainer);
			resources.ApplyResources(this.grWizardParentControl, "grWizardParentControl");
			this.grWizardParentControl.Name = "grWizardParentControl";
			this.pnlParent.BackColor = System.Drawing.Color.Transparent;
			this.pnlParent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlParent, "pnlParent");
			this.pnlParent.Name = "pnlParent";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.panelCaption, "panelCaption");
			this.panelCaption.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelCaption.Controls.Add(this.labelCaption);
			this.panelCaption.Name = "panelCaption";
			this.labelCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelCaption.Appearance.Font")));
			resources.ApplyResources(this.labelCaption, "labelCaption");
			this.labelCaption.Name = "labelCaption";
			this.pnlHintOffset.BackColor = System.Drawing.Color.Transparent;
			this.pnlHintOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlHintOffset, "pnlHintOffset");
			this.pnlHintOffset.Name = "pnlHintOffset";
			resources.ApplyResources(this.pnlHintContainer, "pnlHintContainer");
			this.pnlHintContainer.Controls.Add(this.pnlHint);
			this.pnlHintContainer.Name = "pnlHintContainer";
			resources.ApplyResources(this.pnlHint, "pnlHint");
			this.pnlHint.ForeColor = System.Drawing.Color.Black;
			this.pnlHint.Name = "pnlHint";
			this.pnlHint.ParentAutoHeight = true;
			this.pnlHint.TabStop = false;
			this.panelNavigation.BackColor = System.Drawing.Color.Transparent;
			this.panelNavigation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelNavigation.Controls.Add(this.nbWizard);
			resources.ApplyResources(this.panelNavigation, "panelNavigation");
			this.panelNavigation.Name = "panelNavigation";
			this.workPanel.BackColor = System.Drawing.Color.Transparent;
			this.workPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.workPanel.Controls.Add(this.grWizardParentControl);
			resources.ApplyResources(this.workPanel, "workPanel");
			this.workPanel.Name = "workPanel";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.workPanel);
			this.Controls.Add(this.panelNavigation);
			this.Name = "MainWizardControl";
			((System.ComponentModel.ISupportInitialize)(this.nbWizard)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grWizardParentControl)).EndInit();
			this.grWizardParentControl.ResumeLayout(false);
			this.grWizardParentControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlParent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelCaption)).EndInit();
			this.panelCaption.ResumeLayout(false);
			this.panelCaption.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlHintOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlHintContainer)).EndInit();
			this.pnlHintContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelNavigation)).EndInit();
			this.panelNavigation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.workPanel)).EndInit();
			this.workPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraNavBar.NavBarControl nbWizard;
		private ChartPanelControl panelNavigation;
		public DevExpress.XtraEditors.GroupControl grWizardParentControl;
		public ChartPanelControl pnlParent;
		public ChartPanelControl pnlHintOffset;
		private PanelControl pnlHintContainer;
		public ChartHintPanel pnlHint;
		public ChartPanelControl workPanel;
		public PanelControl panelCaption;
		public LabelControl labelCaption;
		private LabelControl labelControl1;
		public ChartPanelControl chartPanelControl1;
	}
}
