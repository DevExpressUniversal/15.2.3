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

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using System.Collections.Generic;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Native;
using System.Linq;
namespace DevExpress.XtraReports.Design
{
	public class BandCollectionEditorForm : ReportsEditorFormBase {
		#region static
		static void SetComponent(TreeListNode node, XRControl control) {
			node.Tag = control;
		}
		static XRControl GetComponent(TreeListNode node) {
			return node != null ? node.Tag as XRControl : null;
		}
		#endregion
		#region fields
		private System.ComponentModel.Container components = null;
		private XtraContextMenu contextMenu;
		private BandCollection editValue;
		private IDesignerHost designerHost;
		ISelectionService selectionService;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraTreeList.Native.XtraTreeView tv;
		private DevExpress.XtraEditors.SimpleButton buttonCancel;
		private DevExpress.XtraEditors.LabelControl bottomLine;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraReports.Design.PropertyGridUserControl propertyGrid;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraEditors.SimpleButton buttonDown;
		private DevExpress.XtraEditors.SimpleButton buttonAdd;
		private DevExpress.XtraEditors.SimpleButton buttonRemove;
		private DevExpress.XtraEditors.SimpleButton buttonUp;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlGroup layoutControlGroup3;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		#endregion
		public BandCollection EditValue {
			get { return editValue; }
			set {
				editValue = value;
				UpdateTreeView();
				foreach(TreeListNode node in EnumNodesRecursive(tv.Nodes)) { 
					if(selectionService.GetComponentSelected(node.Tag))
						SelectTreeNode(node);
				}
				UpdateRemoveButton();
				UpdateUpDownButtons();		   
			}
		}
		BandCollectionEditorForm()
			: base(null) {
			InitializeComponent();
		}
		public BandCollectionEditorForm(IServiceProvider servProvider) : base(servProvider) {
			InitializeComponent();
			PreparePopupMenu();
			tv.StateImageList = ReportExplorerController.ImageCollection;
			tv.KeyDown += new KeyEventHandler(tv_KeyDown);
			this.propertyGrid.PropertyGridControl.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(propertyGrid_PropertyValueChanged);
			propertyGrid.ServiceProvider = servProvider;
			propertyGrid.SetLookAndFeel(servProvider);
			designerHost = servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			selectionService = servProvider.GetService(typeof(ISelectionService)) as ISelectionService;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				tv.StateImageList = null;
				if (designerHost != null)
					designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
				tv.KeyDown -= new KeyEventHandler(tv_KeyDown);
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BandCollectionEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.buttonDown = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonUp = new DevExpress.XtraEditors.SimpleButton();
			this.propertyGrid = new DevExpress.XtraReports.Design.PropertyGridUserControl();
			this.tv = new DevExpress.XtraTreeList.Native.XtraTreeView();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.bottomLine = new DevExpress.XtraEditors.LabelControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.grpButtons2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.buttonDown);
			this.layoutControl1.Controls.Add(this.buttonAdd);
			this.layoutControl1.Controls.Add(this.buttonRemove);
			this.layoutControl1.Controls.Add(this.buttonUp);
			this.layoutControl1.Controls.Add(this.propertyGrid);
			this.layoutControl1.Controls.Add(this.tv);
			this.layoutControl1.Controls.Add(this.buttonCancel);
			this.layoutControl1.Controls.Add(this.bottomLine);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(800, 118, 1006, 727);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.buttonDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonDown.Image")));
			this.buttonDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonDown, "buttonDown");
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.StyleController = this.layoutControl1;
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
			this.buttonAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleRight;
			resources.ApplyResources(this.buttonAdd, "buttonAdd");
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.StyleController = this.layoutControl1;
			this.buttonAdd.Click += new System.EventHandler(this.button_Click);
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.StyleController = this.layoutControl1;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonUp.Image")));
			this.buttonUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonUp, "buttonUp");
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.StyleController = this.layoutControl1;
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			this.propertyGrid.AllowGlyphSkinning = false;
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.SelectedObject = null;
			this.propertyGrid.SelectedObjects = new object[0];
			this.propertyGrid.ServiceProvider = null;
			this.propertyGrid.ShowCategories = true;
			this.propertyGrid.ShowDescription = true;
			this.tv.DraggedNode = null;
			resources.ApplyResources(this.tv, "tv");
			this.tv.Name = "tv";
			this.tv.OptionsBehavior.Editable = false;
			this.tv.OptionsPrint.PrintHorzLines = false;
			this.tv.OptionsPrint.PrintVertLines = false;
			this.tv.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.tv.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
			this.tv.OptionsView.ShowColumns = false;
			this.tv.OptionsView.ShowHorzLines = false;
			this.tv.OptionsView.ShowIndicator = false;
			this.tv.OptionsView.ShowVertLines = false;
			this.tv.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tv_AfterSelect);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.StyleController = this.layoutControl1;
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			resources.ApplyResources(this.bottomLine, "bottomLine");
			this.bottomLine.Name = "bottomLine";
			this.bottomLine.StyleController = this.layoutControl1;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.grpButtons2,
			this.layoutControlItem4,
			this.layoutControlGroup3});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 43.014705882352942D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition12.Width = 2D;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition13.Width = 56.985294117647065D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition11,
			columnDefinition12,
			columnDefinition13});
			rowDefinition7.Height = 100D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition8.Height = 37D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition7,
			rowDefinition8});
			this.layoutControlGroup1.Size = new System.Drawing.Size(575, 385);
			this.layoutControlGroup1.TextVisible = false;
			this.grpButtons2.GroupBordersVisible = false;
			this.grpButtons2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2});
			this.grpButtons2.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons2.Location = new System.Drawing.Point(0, 328);
			this.grpButtons2.Name = "grpButtons2";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 82D;
			this.grpButtons2.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2});
			rowDefinition1.Height = 15D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 22D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons2.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2});
			this.grpButtons2.OptionsTableLayoutItem.ColumnSpan = 3;
			this.grpButtons2.OptionsTableLayoutItem.RowIndex = 1;
			this.grpButtons2.Size = new System.Drawing.Size(555, 37);
			this.layoutControlItem1.Control = this.bottomLine;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
			this.layoutControlItem1.Size = new System.Drawing.Size(555, 15);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.buttonCancel;
			this.layoutControlItem2.Location = new System.Drawing.Point(473, 15);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(82, 22);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem4.Control = this.propertyGrid;
			this.layoutControlItem4.Location = new System.Drawing.Point(240, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem4.Size = new System.Drawing.Size(315, 328);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.grpButtons});
			this.layoutControlGroup3.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition10.Width = 238D;
			this.layoutControlGroup3.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition10});
			rowDefinition4.Height = 100D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition5.Height = 5D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition6.Height = 26D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup3.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4,
			rowDefinition5,
			rowDefinition6});
			this.layoutControlGroup3.Size = new System.Drawing.Size(238, 328);
			this.layoutControlItem3.Control = this.tv;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(238, 297);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem7,
			this.layoutControlItem6,
			this.layoutControlItem8,
			this.layoutControlItem5,
			this.emptySpaceItem1});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 302);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 8;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 80D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 4D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 80D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition6.Width = 10D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 30D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 4D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition9.Width = 30D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition3,
			columnDefinition4,
			columnDefinition5,
			columnDefinition6,
			columnDefinition7,
			columnDefinition8,
			columnDefinition9});
			rowDefinition3.Height = 26D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition3});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(238, 26);
			this.layoutControlItem7.Control = this.buttonAdd;
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem6.Control = this.buttonRemove;
			this.layoutControlItem6.Location = new System.Drawing.Point(84, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem6.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem8.Control = this.buttonDown;
			this.layoutControlItem8.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem8.Location = new System.Drawing.Point(208, 0);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.ColumnIndex = 6;
			this.layoutControlItem8.Size = new System.Drawing.Size(30, 26);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem8.TrimClientAreaToControl = false;
			this.layoutControlItem5.Control = this.buttonUp;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.Location = new System.Drawing.Point(174, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem5.Size = new System.Drawing.Size(30, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(164, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.OptionsTableLayoutItem.ColumnIndex = 3;
			this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ControlBox = false;
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BandCollectionEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Load += new System.EventHandler(this.BandCollectionEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tv)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void PreparePopupMenu() {
			contextMenu = new XtraContextMenu();
			MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TopMargin), XRBitmaps.TopMarginBand, BandCommands.InsertTopMarginBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ReportHeader), XRBitmaps.ReportHeaderBand, BandCommands.InsertReportHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PageHeader), XRBitmaps.PageHeaderBand, BandCommands.InsertPageHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_GroupHeader), XRBitmaps.GroupHeaderBand, BandCommands.InsertGroupHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Detail), XRBitmaps.DetailBand, BandCommands.InsertDetailBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_GroupFooter), XRBitmaps.GroupFooterBand, BandCommands.InsertGroupFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ReportFooter), XRBitmaps.ReportFooterBand, BandCommands.InsertReportFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PageFooter), XRBitmaps.PageFooterBand, BandCommands.InsertPageFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_BottomMargin), XRBitmaps.BottomMarginBand, BandCommands.InsertBottomMarginBand));
			if(designerHost != null) {
				IMenuCreationService serv = designerHost.GetService<IMenuCreationService>();
				if(serv != null)
					serv.ProcessMenuItems(MenuKind.BandCollection, items);
			}
			contextMenu.AddMenuItems(items, null, null);
		}
		IList<Band> GetOrderedBands(XtraReportBase report) {
			return report != null ? new List<Band>(report.OrderedBands) : new List<Band>();
		}
		private void SelectTreeNode(TreeListNode node) {
			try {
				tv.SelectNode(node);
			} catch {}
		}
		private void UpdateTreeView() {
			UpdateComponentsNodes(tv.Nodes, GetOrderedBands(RootReport));
		}
		XtraReportBase RootReport {
			get {
				return editValue.Count > 0 ? editValue[0].RootReport : null;
			}
		}
		private void UpdateComponentsNodes(TreeListNodes nodes, IList<Band> components) {
			for(int i = nodes.Count - 1; i >= 0; i--) {
				TreeListNode node = nodes[i];
				if(!components.Contains(GetComponent(node))) nodes.Remove(node);
			}
			for(int i = 0; i < components.Count; i++) {
				XRControl control = components[i] as XRControl;
				UpdateTreeNode(nodes, i, control);
			}
		}
		private void UpdateTreeNode(TreeListNodes nodes, int index, XRControl control) {
			try {
				XtraListNode node = nodes.FirstOrDefault<TreeListNode>(item => ReferenceEquals(item.Tag, control)) as XtraListNode;
				if(node == null) {
					node = new XtraListNode(control.Site.Name, nodes);
					((IList)nodes).Insert(index, node);
					node.SelectImageIndex = node.StateImageIndex = ReportExplorerController.GetImageIndex(control, false);
					SetComponent(node, control);
				} else if(nodes.IndexOf(node) != index) {
					bool wasSelected = tv.SelectedNode == node;
					SwapNodes(nodes, node, nodes[index]);
					node.Text = control.Site.Name;
					node.Tag = control;
					if(wasSelected)
						tv.SelectNode(node);
				}
				XtraReportBase report = control as XtraReportBase;
				if(report != null)
					UpdateComponentsNodes(node.Nodes, GetOrderedBands(report));
				if(tv.SelectedNode != null && tv.SelectedNode.Tag != null)
					UpdatePropertyGrid();
			}
			catch { }
		}
		void SwapNodes(TreeListNodes nodes, TreeListNode node1, TreeListNode node2) {
			bool node1WasExpanded = node1.Expanded;
			bool node2WasExpanded = node2.Expanded;
			tv.MoveNode(node1, node2);
			tv.MoveNode(node1, nodes.ParentNode);
			node1.Expanded = node1WasExpanded;
			node2.Expanded = node2WasExpanded;
		}
		private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			UpdateTreeView();
			UpdateUpDownButtons();
		}
		private void buttonRemove_Click(object sender, System.EventArgs e) {
			RemoveComponent(tv.SelectedNode);
		}
		private void RemoveComponent(TreeListNode node) {
			XRControl control = GetComponent(node);
			if(CanRemove(control)) {
				propertyGrid.SelectedObject = null;
				SetComponent(node, null);
				if(control != null) RemoveFromContainer(control);
			}
		}
		bool CanRemove(XRControl control) {
			System.Diagnostics.Debug.Assert(control != null);
			IMenuCommandService menuServ = designerHost.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			MenuCommand command = menuServ.FindCommand(StandardCommands.Delete);
			if(command != null && !command.Enabled)
				return false;
			return LockService.GetInstance(designerHost).CanDeleteComponent(control);
		}
		private void buttonUp_Click(object sender, System.EventArgs e) {
			MoveBand(ReorderBandsCommands.MoveUp);
		}
		private void buttonDown_Click(object sender, System.EventArgs e) {
			MoveBand(ReorderBandsCommands.MoveDown);
		}
		void MoveBand(CommandID cmdID) {
			IMenuCommandService menuServ = designerHost.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			ISelectionService selectionServ = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			selectionServ.SetSelectedComponents(new object[] { GetComponent(tv.SelectedNode) });
			menuServ.GlobalInvoke(cmdID);
		}
		private void button_Click(object sender, System.EventArgs e) {
			Control control = sender as Control;
			contextMenu.Show(control, control.PointToScreen(new Point(0, control.Size.Height)), designerHost);
		}
		private void tv_AfterSelect(object sender, NodeEventArgs e) {
			UpdatePropertyGrid();
			SetSelectedComponent(propertyGrid.SelectedObject);
			UpdateRemoveButton();
			UpdateUpDownButtons();
		}
		private void tv_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete)
				RemoveComponent(tv.SelectedNode);
		}
		protected void SetSelectedComponent(object component) {
			if(component != null)
				this.selectionService.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
		}
		protected void UpdatePropertyGrid() {
			try {
				XRControl control = GetComponent(tv.SelectedNode);
				propertyGrid.Enabled = LockService.GetInstance(designerHost).CanChangeComponent(control);
				propertyGrid.SelectedObject = control;
			} catch {
				propertyGrid.SelectedObject = null;
			}
		}
		private void UpdateRemoveButton() {
			buttonRemove.Enabled = tv.Nodes.Count > 0 && CanRemove(GetComponent(tv.SelectedNode));
		}
		private void UpdateUpDownButtons() {
			IMoveableBand movableBand = GetComponent(tv.SelectedNode) as IMoveableBand;
			if(movableBand != null) {
				buttonUp.Enabled = movableBand.CanBeMoved(BandReorderDirection.Up);
				buttonDown.Enabled = movableBand.CanBeMoved(BandReorderDirection.Down);
			} else {
				buttonUp.Enabled = false;
				buttonDown.Enabled = false;
			}
		}
		private void RemoveFromContainer(XRControl control) {
			DesignerTransaction transaction = designerHost.CreateTransaction(DesignSR.Trans_Delete);
			try {
				control.Dispose();
			} finally {
				transaction.Commit();
			}
		}
		private void propertyGrid_PropertyValueChanged(object s, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e) {
			TreeListNode node = EnumNodesRecursive(tv.Nodes).FirstOrDefault(item => ReferenceEquals(item.Tag, propertyGrid.SelectedObject));
			if(node == null)
				return;
			string name = propertyGrid.GetPropertyDescriptor(e.Row).Name;
			if(Comparer.Equals(name, "Name")) {
				((XtraListNode)node).Text = ((IComponent)propertyGrid.SelectedObject).Site.Name;
				tv.RefreshNode(node);
			} else if(Comparer.Equals(name, "Level"))
				UpdateTreeView();
		}
		IEnumerable<TreeListNode> EnumNodesRecursive(TreeListNodes nodes) {
			foreach(TreeListNode node in nodes) {
				yield return node;
				foreach(TreeListNode node2 in EnumNodesRecursive(node.Nodes))
					yield return node2;
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		private void BandCollectionEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeButtonsLayout(buttonAdd, buttonRemove, 0, 2);
			InitializeButtonsLayout(buttonUp, buttonDown, 4, 6);
			grpButtons2.OptionsTableLayoutGroup.ColumnDefinitions[1].Width = Math.Max(buttonCancel.CalcBestSize().Width, buttonCancel.Width);
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
		}
		void InitializeButtonsLayout(SimpleButton firstButton, SimpleButton secondButton, int firstCol, int secondCol) {
			int firstButtonBestWidth = firstButton.CalcBestSize().Width;
			int secondButtonBestWidth = secondButton.CalcBestSize().Width;
			if(firstButtonBestWidth <= firstButton.Width && secondButtonBestWidth <= secondButton.Width)
				return;
			int btnsActualSize = Math.Max(firstButtonBestWidth, secondButtonBestWidth);
			int delta = 2 * btnsActualSize - firstButton.Width - secondButton.Width;
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[firstCol].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[secondCol].Width = btnsActualSize + 2 + 2;
			grpButtons.Width += delta;
		}
	}
}
