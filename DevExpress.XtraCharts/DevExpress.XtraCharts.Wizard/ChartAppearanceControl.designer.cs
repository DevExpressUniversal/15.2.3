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
	partial class ChartAppearanceControl {
		protected override void Dispose(bool disposing) {
			if (chart != null) {
				panelControl1.Controls.Remove(designControl);
				chart = null;
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
				private void InitializeComponent()
				{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartAppearanceControl));
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.paletteEditContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.styleEditContainer = new DevExpress.XtraEditors.PopupContainerControl();
			this.stylesContainer = new DevExpress.XtraCharts.Design.StylesContainerControl();
			this.paletteEditControl = new DevExpress.XtraCharts.Design.PaletteEditControl();
			this.paletteEdit = new DevExpress.XtraCharts.Design.PalettePopupContainerEdit();
			this.styleEdit = new DevExpress.XtraEditors.PopupContainerEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.paletteEditContainer)).BeginInit();
			this.paletteEditContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.styleEditContainer)).BeginInit();
			this.styleEditContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.paletteEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.styleEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			this.SuspendLayout();
			this.panelControl1.Controls.Add(this.paletteEditContainer);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			this.paletteEditContainer.Controls.Add(this.styleEditContainer);
			this.paletteEditContainer.Controls.Add(this.paletteEditControl);
			resources.ApplyResources(this.paletteEditContainer, "paletteEditContainer");
			this.paletteEditContainer.Name = "paletteEditContainer";
			this.styleEditContainer.Controls.Add(this.stylesContainer);
			resources.ApplyResources(this.styleEditContainer, "styleEditContainer");
			this.styleEditContainer.Name = "styleEditContainer";
			this.stylesContainer.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("stylesContainer.Appearance.BackColor")));
			this.stylesContainer.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.stylesContainer, "stylesContainer");
			this.stylesContainer.Name = "stylesContainer";
			this.stylesContainer.OnEditValueChanged += new System.EventHandler(this.stylesContainer_OnEditValueChanged);
			this.stylesContainer.OnNeedClose += new System.EventHandler(this.stylesContainer_OnNeedClose);
			resources.ApplyResources(this.paletteEditControl, "paletteEditControl");
			this.paletteEditControl.Name = "paletteEditControl";
			this.paletteEditControl.OnPaletteChanged += new System.EventHandler(this.paletteEditControl_OnPaletteChanged);
			this.paletteEditControl.OnNeedClose += new System.EventHandler(this.paletteEditControl_OnNeedClose);
			resources.ApplyResources(this.paletteEdit, "paletteEdit");
			this.paletteEdit.Name = "paletteEdit";
			this.paletteEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("paletteEdit.Properties.Buttons"))))});
			this.paletteEdit.Properties.CloseOnLostFocus = false;
			this.paletteEdit.Properties.CloseOnOuterMouseClick = false;
			this.paletteEdit.Properties.PopupControl = this.paletteEditContainer;
			this.paletteEdit.Properties.UsePopupControlMinSize = true;
			this.paletteEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.paletteEdit_QueryPopUp);
			this.paletteEdit.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.paletteEdit_Closed);
			this.paletteEdit.EditValueChanged += new System.EventHandler(this.paletteEdit_EditValueChanged);
			resources.ApplyResources(this.styleEdit, "styleEdit");
			this.styleEdit.Name = "styleEdit";
			this.styleEdit.Properties.AutoHeight = ((bool)(resources.GetObject("styleEdit.Properties.AutoHeight")));
			this.styleEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("styleEdit.Properties.Buttons"))))});
			this.styleEdit.Properties.PopupControl = this.styleEditContainer;
			this.styleEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.styleEdit_QueryPopUp);
			this.styleEdit.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(this.styleEdit_CloseUp);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.panelControl2.Controls.Add(this.labelControl2);
			this.panelControl2.Controls.Add(this.labelControl1);
			this.panelControl2.Controls.Add(this.styleEdit);
			this.panelControl2.Controls.Add(this.paletteEdit);
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.panelControl2);
			this.Name = "ChartAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.paletteEditContainer)).EndInit();
			this.paletteEditContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.styleEditContainer)).EndInit();
			this.styleEditContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.paletteEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.styleEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			this.panelControl2.PerformLayout();
			this.ResumeLayout(false);
				}
		#endregion
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.PopupContainerControl paletteEditContainer;
		private DevExpress.XtraCharts.Design.PaletteEditControl paletteEditControl;
		private DevExpress.XtraEditors.PopupContainerControl styleEditContainer;
		private DevExpress.XtraCharts.Design.StylesContainerControl stylesContainer;
		private DevExpress.XtraCharts.Design.PalettePopupContainerEdit paletteEdit;
		private DevExpress.XtraEditors.PopupContainerEdit styleEdit;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraEditors.PanelControl panelControl2;
	}
}
