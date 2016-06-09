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
	partial class ValueMapOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValueMapOptionsControl));
			this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.groupPalette = new DevExpress.XtraEditors.GroupControl();
			this.colorEdit2 = new DevExpress.XtraEditors.ColorEdit();
			this.colorEdit1 = new DevExpress.XtraEditors.ColorEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.autoColorsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.customColorsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.scaleContainer = new DevExpress.XtraEditors.GroupControl();
			this.absoluteLevelsEdit = new DevExpress.XtraEditors.SpinEdit();
			this.percentLevelsEdit = new DevExpress.XtraEditors.SpinEdit();
			this.absoluteScaleCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.percentsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelThresholdType = new DevExpress.XtraEditors.LabelControl();
			this.gridControl = new DevExpress.XtraGrid.GridControl();
			this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnRangeStop = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnColor = new DevExpress.XtraGrid.Columns.GridColumn();
			this.allowEditCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupPalette)).BeginInit();
			this.groupPalette.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.autoColorsCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.customColorsCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.scaleContainer)).BeginInit();
			this.scaleContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.absoluteLevelsEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.percentLevelsEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.absoluteScaleCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.percentsCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.allowEditCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.repositoryItemColorEdit1, "repositoryItemColorEdit1");
			this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorEdit1.Buttons"))))});
			this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
			this.groupPalette.Controls.Add(this.colorEdit2);
			this.groupPalette.Controls.Add(this.colorEdit1);
			this.groupPalette.Controls.Add(this.labelControl2);
			this.groupPalette.Controls.Add(this.labelControl1);
			this.groupPalette.Controls.Add(this.autoColorsCheckEdit);
			this.groupPalette.Controls.Add(this.customColorsCheckEdit);
			resources.ApplyResources(this.groupPalette, "groupPalette");
			this.groupPalette.Name = "groupPalette";
			resources.ApplyResources(this.colorEdit2, "colorEdit2");
			this.colorEdit2.Name = "colorEdit2";
			this.colorEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit2.Properties.Buttons"))))});
			this.colorEdit2.EditValueChanged += new System.EventHandler(this.colorEdit2_EditValueChanged);
			resources.ApplyResources(this.colorEdit1, "colorEdit1");
			this.colorEdit1.Name = "colorEdit1";
			this.colorEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit1.Properties.Buttons"))))});
			this.colorEdit1.EditValueChanged += new System.EventHandler(this.colorEdit1_EditValueChanged);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.autoColorsCheckEdit, "autoColorsCheckEdit");
			this.autoColorsCheckEdit.Name = "autoColorsCheckEdit";
			this.autoColorsCheckEdit.Properties.Caption = resources.GetString("autoColorsCheckEdit.Properties.Caption");
			this.autoColorsCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.autoColorsCheckEdit.Properties.RadioGroupIndex = 0;
			resources.ApplyResources(this.customColorsCheckEdit, "customColorsCheckEdit");
			this.customColorsCheckEdit.Name = "customColorsCheckEdit";
			this.customColorsCheckEdit.Properties.Caption = resources.GetString("customColorsCheckEdit.Properties.Caption");
			this.customColorsCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.customColorsCheckEdit.Properties.RadioGroupIndex = 0;
			this.customColorsCheckEdit.TabStop = false;
			this.customColorsCheckEdit.CheckedChanged += new System.EventHandler(this.customColorsCheckEdit_CheckedChanged);
			this.scaleContainer.Controls.Add(this.absoluteLevelsEdit);
			this.scaleContainer.Controls.Add(this.percentLevelsEdit);
			this.scaleContainer.Controls.Add(this.absoluteScaleCheckEdit);
			this.scaleContainer.Controls.Add(this.percentsCheckEdit);
			this.scaleContainer.Controls.Add(this.labelControl3);
			this.scaleContainer.Controls.Add(this.labelThresholdType);
			resources.ApplyResources(this.scaleContainer, "scaleContainer");
			this.scaleContainer.Name = "scaleContainer";
			resources.ApplyResources(this.absoluteLevelsEdit, "absoluteLevelsEdit");
			this.absoluteLevelsEdit.Name = "absoluteLevelsEdit";
			this.absoluteLevelsEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("absoluteLevelsEdit.Properties.Buttons"))))});
			this.absoluteLevelsEdit.EditValueChanged += new System.EventHandler(this.absoluteLevelsSpinEdit_EditValueChanged);
			resources.ApplyResources(this.percentLevelsEdit, "percentLevelsEdit");
			this.percentLevelsEdit.Name = "percentLevelsEdit";
			this.percentLevelsEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("percentLevelsEdit.Properties.Buttons"))))});
			this.percentLevelsEdit.EditValueChanged += new System.EventHandler(this.levelsEdit_EditValueChanged);
			resources.ApplyResources(this.absoluteScaleCheckEdit, "absoluteScaleCheckEdit");
			this.absoluteScaleCheckEdit.Name = "absoluteScaleCheckEdit";
			this.absoluteScaleCheckEdit.Properties.Caption = resources.GetString("absoluteScaleCheckEdit.Properties.Caption");
			this.absoluteScaleCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.absoluteScaleCheckEdit.Properties.RadioGroupIndex = 0;
			this.absoluteScaleCheckEdit.TabStop = false;
			this.absoluteScaleCheckEdit.CheckedChanged += new System.EventHandler(this.percentsCheckEdit_CheckedChanged);
			resources.ApplyResources(this.percentsCheckEdit, "percentsCheckEdit");
			this.percentsCheckEdit.Name = "percentsCheckEdit";
			this.percentsCheckEdit.Properties.Caption = resources.GetString("percentsCheckEdit.Properties.Caption");
			this.percentsCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.percentsCheckEdit.Properties.RadioGroupIndex = 0;
			this.percentsCheckEdit.CheckedChanged += new System.EventHandler(this.percentsCheckEdit_CheckedChanged);
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.labelThresholdType, "labelThresholdType");
			this.labelThresholdType.Name = "labelThresholdType";
			this.gridControl.EmbeddedNavigator.Buttons.Edit.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.First.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.Last.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.Next.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.NextPage.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.Prev.Visible = false;
			this.gridControl.EmbeddedNavigator.Buttons.PrevPage.Enabled = false;
			this.gridControl.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
			this.gridControl.EmbeddedNavigator.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.gridControl.EmbeddedNavigator.TextLocation = ((DevExpress.XtraEditors.NavigatorButtonsTextLocation)(resources.GetObject("gridControl.EmbeddedNavigator.TextLocation")));
			resources.ApplyResources(this.gridControl, "gridControl");
			this.gridControl.MainView = this.gridView;
			this.gridControl.Name = "gridControl";
			this.gridControl.UseEmbeddedNavigator = true;
			this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView});
			this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnRangeStop,
			this.gridColumnColor});
			this.gridView.GridControl = this.gridControl;
			this.gridView.Name = "gridView";
			this.gridView.OptionsDetail.EnableMasterViewMode = false;
			this.gridView.OptionsFilter.AllowFilterEditor = false;
			this.gridView.OptionsMenu.EnableColumnMenu = false;
			this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridView.OptionsView.BestFitMaxRowCount = 1000;
			this.gridView.OptionsView.ColumnAutoWidth = false;
			this.gridView.OptionsView.ShowGroupPanel = false;
			resources.ApplyResources(this.gridColumnRangeStop, "gridColumnRangeStop");
			this.gridColumnRangeStop.FieldName = "Range";
			this.gridColumnRangeStop.Name = "gridColumnRangeStop";
			resources.ApplyResources(this.gridColumnColor, "gridColumnColor");
			this.gridColumnColor.ColumnEdit = this.repositoryItemColorEdit1;
			this.gridColumnColor.FieldName = "Color";
			this.gridColumnColor.Name = "gridColumnColor";
			resources.ApplyResources(this.allowEditCheckEdit, "allowEditCheckEdit");
			this.allowEditCheckEdit.Name = "allowEditCheckEdit";
			this.allowEditCheckEdit.Properties.Caption = resources.GetString("allowEditCheckEdit.Properties.Caption");
			this.allowEditCheckEdit.CheckedChanged += new System.EventHandler(this.allowEditCheckEdit_CheckedChanged);
			this.groupControl1.Controls.Add(this.allowEditCheckEdit);
			this.groupControl1.Controls.Add(this.gridControl);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.scaleContainer);
			this.Controls.Add(this.groupControl1);
			this.Controls.Add(this.groupPalette);
			this.Name = "ValueMapOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupPalette)).EndInit();
			this.groupPalette.ResumeLayout(false);
			this.groupPalette.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.autoColorsCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.customColorsCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.scaleContainer)).EndInit();
			this.scaleContainer.ResumeLayout(false);
			this.scaleContainer.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.absoluteLevelsEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.percentLevelsEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.absoluteScaleCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.percentsCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.allowEditCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.GroupControl groupPalette;
		private XtraEditors.LabelControl labelThresholdType;
		private XtraEditors.ColorEdit colorEdit2;
		private XtraEditors.ColorEdit colorEdit1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.SpinEdit percentLevelsEdit;
		private XtraEditors.CheckEdit customColorsCheckEdit;
		private XtraGrid.GridControl gridControl;
		private XtraGrid.Views.Grid.GridView gridView;
		private XtraGrid.Columns.GridColumn gridColumnRangeStop;
		private XtraGrid.Columns.GridColumn gridColumnColor;
		private XtraEditors.CheckEdit percentsCheckEdit;
		private XtraEditors.CheckEdit autoColorsCheckEdit;
		private XtraEditors.GroupControl scaleContainer;
		private XtraEditors.CheckEdit allowEditCheckEdit;
		private XtraEditors.SpinEdit absoluteLevelsEdit;
		private XtraEditors.CheckEdit absoluteScaleCheckEdit;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.GroupControl groupControl1;
		private XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
	}
}
