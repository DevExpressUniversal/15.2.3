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
	partial class FormatRuleControlManager {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatRuleControlManager));
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.ddbAddRule = new DevExpress.XtraEditors.DropDownButton();
			this.popupMenuAddRule = new DevExpress.XtraBars.PopupMenu(this.components);
			this.gridRules = new DevExpress.XtraGrid.GridControl();
			this.gridViewRules = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnEnabled = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repItemEnabled = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.gridColumnCaption = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnCalculateBy = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnApplyTo = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repItemApplyTo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.gridColumnStopIfTrue = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repItemStopIfTrue = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
			this.cbCalculateBy = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbFilterBy = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgRoot = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgDescription = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgNewRule = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciAddRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCalculateBy = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciButtonEdit = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciButtonDelete = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciFilterBy = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciButtonUp = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciButtonDown = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciRules = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupMenuAddRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridRules)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewRules)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemEnabled)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemApplyTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemStopIfTrue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCalculateBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgDescription)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgNewRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAddRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCalculateBy)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonDelete)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFilterBy)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciRules)).BeginInit();
			this.SuspendLayout();
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barManager.Controller = this.barAndDockingController;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.layoutControl.AllowCustomization = false;
			resources.ApplyResources(this.layoutControl, "layoutControl");
			this.layoutControl.Controls.Add(this.ddbAddRule);
			this.layoutControl.Controls.Add(this.gridRules);
			this.layoutControl.Controls.Add(this.btnDown);
			this.layoutControl.Controls.Add(this.btnUp);
			this.layoutControl.Controls.Add(this.btnDelete);
			this.layoutControl.Controls.Add(this.btnEdit);
			this.layoutControl.Controls.Add(this.cbCalculateBy);
			this.layoutControl.Controls.Add(this.cbFilterBy);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1024, 42, 884, 793);
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControl.Root = this.lcgRoot;
			this.ddbAddRule.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
			this.ddbAddRule.DropDownControl = this.popupMenuAddRule;
			resources.ApplyResources(this.ddbAddRule, "ddbAddRule");
			this.ddbAddRule.MenuManager = this.barManager;
			this.ddbAddRule.Name = "ddbAddRule";
			this.ddbAddRule.StyleController = this.layoutControl;
			this.popupMenuAddRule.Manager = this.barManager;
			this.popupMenuAddRule.Name = "popupMenuAddRule";
			resources.ApplyResources(this.gridRules, "gridRules");
			this.gridRules.MainView = this.gridViewRules;
			this.gridRules.Name = "gridRules";
			this.gridRules.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repItemEnabled,
			this.repItemApplyTo,
			this.repItemStopIfTrue});
			this.gridRules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewRules});
			this.gridRules.DoubleClick += new System.EventHandler(this.OnGridRulesDoubleClick);
			this.gridViewRules.Appearance.Row.Font = ((System.Drawing.Font)(resources.GetObject("gridViewRules.Appearance.Row.Font")));
			this.gridViewRules.Appearance.Row.Options.UseFont = true;
			this.gridViewRules.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnEnabled,
			this.gridColumnCaption,
			this.gridColumnCalculateBy,
			this.gridColumnApplyTo,
			this.gridColumnStopIfTrue});
			this.gridViewRules.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this.gridViewRules.GridControl = this.gridRules;
			this.gridViewRules.Name = "gridViewRules";
			this.gridViewRules.OptionsCustomization.AllowColumnMoving = false;
			this.gridViewRules.OptionsCustomization.AllowFilter = false;
			this.gridViewRules.OptionsCustomization.AllowGroup = false;
			this.gridViewRules.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridViewRules.OptionsCustomization.AllowSort = false;
			this.gridViewRules.OptionsMenu.EnableColumnMenu = false;
			this.gridViewRules.OptionsMenu.EnableFooterMenu = false;
			this.gridViewRules.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridViewRules.OptionsMenu.ShowAutoFilterRowItem = false;
			this.gridViewRules.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
			this.gridViewRules.OptionsMenu.ShowGroupSortSummaryItems = false;
			this.gridViewRules.OptionsMenu.ShowSplitItem = false;
			this.gridViewRules.OptionsView.ShowGroupPanel = false;
			this.gridViewRules.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewRules.OptionsView.ShowIndicator = false;
			this.gridViewRules.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
			this.gridViewRules.RowHeight = 30;
			this.gridViewRules.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewRules_FocusedRowChanged);
			this.gridColumnEnabled.ColumnEdit = this.repItemEnabled;
			resources.ApplyResources(this.gridColumnEnabled, "gridColumnEnabled");
			this.gridColumnEnabled.Name = "gridColumnEnabled";
			this.gridColumnEnabled.OptionsColumn.FixedWidth = true;
			this.gridColumnEnabled.OptionsColumn.ShowCaption = false;
			resources.ApplyResources(this.repItemEnabled, "repItemEnabled");
			this.repItemEnabled.Name = "repItemEnabled";
			this.gridColumnCaption.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnCaption.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			resources.ApplyResources(this.gridColumnCaption, "gridColumnCaption");
			this.gridColumnCaption.Name = "gridColumnCaption";
			this.gridColumnCaption.OptionsColumn.AllowEdit = false;
			this.gridColumnCalculateBy.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnCalculateBy.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			resources.ApplyResources(this.gridColumnCalculateBy, "gridColumnCalculateBy");
			this.gridColumnCalculateBy.Name = "gridColumnCalculateBy";
			this.gridColumnCalculateBy.OptionsColumn.AllowEdit = false;
			this.gridColumnCalculateBy.OptionsColumn.ReadOnly = true;
			this.gridColumnApplyTo.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnApplyTo.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			resources.ApplyResources(this.gridColumnApplyTo, "gridColumnApplyTo");
			this.gridColumnApplyTo.ColumnEdit = this.repItemApplyTo;
			this.gridColumnApplyTo.Name = "gridColumnApplyTo";
			resources.ApplyResources(this.repItemApplyTo, "repItemApplyTo");
			this.repItemApplyTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repItemApplyTo.Buttons"))))});
			this.repItemApplyTo.Name = "repItemApplyTo";
			resources.ApplyResources(this.gridColumnStopIfTrue, "gridColumnStopIfTrue");
			this.gridColumnStopIfTrue.ColumnEdit = this.repItemStopIfTrue;
			this.gridColumnStopIfTrue.Name = "gridColumnStopIfTrue";
			this.gridColumnStopIfTrue.OptionsColumn.FixedWidth = true;
			resources.ApplyResources(this.repItemStopIfTrue, "repItemStopIfTrue");
			this.repItemStopIfTrue.Name = "repItemStopIfTrue";
			this.btnDown.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDown.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("btnDown.Appearance.Font")));
			this.btnDown.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.StyleController = this.layoutControl;
			this.btnDown.Click += new System.EventHandler(this.OnDownClick);
			this.btnUp.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnUp.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("btnUp.Appearance.Font")));
			this.btnUp.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.StyleController = this.layoutControl;
			this.btnUp.Click += new System.EventHandler(this.OnUpClick);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.StyleController = this.layoutControl;
			this.btnDelete.Click += new System.EventHandler(this.OnDeleteClick);
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.StyleController = this.layoutControl;
			this.btnEdit.Click += new System.EventHandler(this.OnEditClick);
			resources.ApplyResources(this.cbCalculateBy, "cbCalculateBy");
			this.cbCalculateBy.Name = "cbCalculateBy";
			this.cbCalculateBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbCalculateBy.Properties.Buttons"))))});
			this.cbCalculateBy.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbCalculateBy.StyleController = this.layoutControl;
			resources.ApplyResources(this.cbFilterBy, "cbFilterBy");
			this.cbFilterBy.Name = "cbFilterBy";
			this.cbFilterBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilterBy.Properties.Buttons"))))});
			this.cbFilterBy.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbFilterBy.StyleController = this.layoutControl;
			this.lcgRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgRoot.GroupBordersVisible = false;
			this.lcgRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgDescription,
			this.lcgNewRule,
			this.lciButtonEdit,
			this.lciButtonDelete,
			this.lciFilterBy,
			this.lciButtonUp,
			this.lciButtonDown,
			this.lciRules});
			this.lcgRoot.Location = new System.Drawing.Point(0, 0);
			this.lcgRoot.Name = "Root";
			this.lcgRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lcgRoot.Size = new System.Drawing.Size(545, 267);
			this.lcgRoot.TextVisible = false;
			this.lcgDescription.GroupBordersVisible = false;
			this.lcgDescription.Location = new System.Drawing.Point(0, 0);
			this.lcgDescription.Name = "lcgDescription";
			this.lcgDescription.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgDescription.Size = new System.Drawing.Size(541, 1);
			this.lcgDescription.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgNewRule.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgNewRule.GroupBordersVisible = false;
			this.lcgNewRule.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciAddRule,
			this.lciCalculateBy});
			this.lcgNewRule.Location = new System.Drawing.Point(0, 231);
			this.lcgNewRule.Name = "lcgNewRule";
			this.lcgNewRule.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 6, 0);
			this.lcgNewRule.Size = new System.Drawing.Size(541, 32);
			this.lcgNewRule.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgNewRule.TextVisible = false;
			this.lciAddRule.Control = this.ddbAddRule;
			this.lciAddRule.Location = new System.Drawing.Point(0, 0);
			this.lciAddRule.Name = "lciAddRule";
			this.lciAddRule.Size = new System.Drawing.Size(74, 26);
			this.lciAddRule.TextSize = new System.Drawing.Size(0, 0);
			this.lciAddRule.TextVisible = false;
			this.lciCalculateBy.Control = this.cbCalculateBy;
			resources.ApplyResources(this.lciCalculateBy, "lciCalculateBy");
			this.lciCalculateBy.Location = new System.Drawing.Point(74, 0);
			this.lciCalculateBy.Name = "lciCalculateBy";
			this.lciCalculateBy.Size = new System.Drawing.Size(467, 26);
			this.lciCalculateBy.TextLocation = DevExpress.Utils.Locations.Left;
			this.lciCalculateBy.TextSize = new System.Drawing.Size(63, 13);
			this.lciButtonEdit.Control = this.btnEdit;
			this.lciButtonEdit.Location = new System.Drawing.Point(0, 1);
			this.lciButtonEdit.Name = "lciButtonEdit";
			this.lciButtonEdit.Size = new System.Drawing.Size(74, 26);
			this.lciButtonEdit.TextSize = new System.Drawing.Size(0, 0);
			this.lciButtonEdit.TextVisible = false;
			this.lciButtonDelete.Control = this.btnDelete;
			this.lciButtonDelete.Location = new System.Drawing.Point(74, 1);
			this.lciButtonDelete.Name = "lciButtonDelete";
			this.lciButtonDelete.Size = new System.Drawing.Size(74, 26);
			this.lciButtonDelete.TextSize = new System.Drawing.Size(0, 0);
			this.lciButtonDelete.TextVisible = false;
			this.lciFilterBy.Control = this.cbFilterBy;
			this.lciFilterBy.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			resources.ApplyResources(this.lciFilterBy, "lciFilterBy");
			this.lciFilterBy.Location = new System.Drawing.Point(188, 1);
			this.lciFilterBy.Name = "lciFilterBy";
			this.lciFilterBy.Padding = new DevExpress.XtraLayout.Utils.Padding(100, 2, 2, 2);
			this.lciFilterBy.Size = new System.Drawing.Size(353, 26);
			this.lciFilterBy.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.lciFilterBy.TextLocation = DevExpress.Utils.Locations.Left;
			this.lciFilterBy.TextSize = new System.Drawing.Size(43, 13);
			this.lciFilterBy.TextToControlDistance = 8;
			this.lciFilterBy.TrimClientAreaToControl = false;
			this.lciButtonUp.Control = this.btnUp;
			this.lciButtonUp.Location = new System.Drawing.Point(148, 1);
			this.lciButtonUp.Name = "lciButtonUp";
			this.lciButtonUp.Size = new System.Drawing.Size(20, 26);
			this.lciButtonUp.TextSize = new System.Drawing.Size(0, 0);
			this.lciButtonUp.TextVisible = false;
			this.lciButtonDown.Control = this.btnDown;
			this.lciButtonDown.Location = new System.Drawing.Point(168, 1);
			this.lciButtonDown.Name = "lciButtonDown";
			this.lciButtonDown.Size = new System.Drawing.Size(20, 26);
			this.lciButtonDown.TextSize = new System.Drawing.Size(0, 0);
			this.lciButtonDown.TextVisible = false;
			this.lciRules.Control = this.gridRules;
			this.lciRules.Location = new System.Drawing.Point(0, 27);
			this.lciRules.Name = "lciRules";
			this.lciRules.Size = new System.Drawing.Size(541, 204);
			this.lciRules.TextSize = new System.Drawing.Size(0, 0);
			this.lciRules.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "FormatRuleControlManager";
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupMenuAddRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridRules)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewRules)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemEnabled)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemApplyTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemStopIfTrue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCalculateBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgDescription)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgNewRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAddRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCalculateBy)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonDelete)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFilterBy)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciRules)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraBars.BarManager barManager;
		private XtraBars.BarAndDockingController barAndDockingController;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraEditors.ComboBoxEdit cbFilterBy;
		private XtraLayout.LayoutControlItem lciFilterBy;
		private XtraLayout.LayoutControlGroup lcgDescription;
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup lcgRoot;
		private XtraLayout.LayoutControlGroup lcgNewRule;
		private XtraEditors.SimpleButton btnEdit;
		private XtraLayout.LayoutControlItem lciButtonEdit;
		private XtraEditors.SimpleButton btnDelete;
		private XtraLayout.LayoutControlItem lciButtonDelete;
		private XtraGrid.GridControl gridRules;
		private XtraGrid.Views.Grid.GridView gridViewRules;
		private XtraGrid.Columns.GridColumn gridColumnEnabled;
		private XtraGrid.Columns.GridColumn gridColumnCaption;
		private XtraGrid.Columns.GridColumn gridColumnCalculateBy;
		private XtraGrid.Columns.GridColumn gridColumnApplyTo;
		private XtraLayout.LayoutControlItem lciRules;
		private XtraEditors.SimpleButton btnUp;
		private XtraLayout.LayoutControlItem lciButtonUp;
		private XtraEditors.SimpleButton btnDown;
		private XtraLayout.LayoutControlItem lciButtonDown;
		private XtraEditors.Repository.RepositoryItemCheckEdit repItemEnabled;
		private XtraEditors.Repository.RepositoryItemComboBox repItemApplyTo;
		private XtraGrid.Columns.GridColumn gridColumnStopIfTrue;
		private XtraEditors.Repository.RepositoryItemCheckEdit repItemStopIfTrue;
		private XtraEditors.DropDownButton ddbAddRule;
		private XtraLayout.LayoutControlItem lciAddRule;
		private XtraEditors.ComboBoxEdit cbCalculateBy;
		private XtraLayout.LayoutControlItem lciCalculateBy;
		private XtraBars.PopupMenu popupMenuAddRule;
	}
}
