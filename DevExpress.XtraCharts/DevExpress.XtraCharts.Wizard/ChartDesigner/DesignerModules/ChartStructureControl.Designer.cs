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
using DevExpress.XtraCharts.Designer.Native;
namespace DevExpress.XtraCharts.Designer
{
	public partial class ChartStructureControl 
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartStructureControl));
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
			this.tlChartTree = new DevExpress.XtraCharts.Designer.Native.ChartTreeList();
			this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.defaultButtonEdit = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditPlus = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditDelete = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditEye = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditEyeDelete = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditDisabledEye = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.buttonEditDisabledEyeDelete = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImageButtonEdit();
			this.popupContainerEdit = new DevExpress.XtraCharts.Designer.Native.RepositoryItemImagePopupContainerEdit();
			this.popupContainerControl = new DevExpress.XtraEditors.PopupContainerControl();
			this.liTreeImages = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.tlChartTree)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.defaultButtonEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditPlus)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDelete)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditEye)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditEyeDelete)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDisabledEye)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDisabledEyeDelete)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerControl)).BeginInit();
			this.SuspendLayout();
			this.tlChartTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.treeListColumn1});
			resources.ApplyResources(this.tlChartTree, "tlChartTree");
			this.tlChartTree.Name = "tlChartTree";
			this.tlChartTree.BeginUnboundLoad();
			this.tlChartTree.AppendNode(new object[] {
			"Chart"}, -1);
			this.tlChartTree.AppendNode(new object[] {
			"Diagram"}, 0);
			this.tlChartTree.EndUnboundLoad();
			this.tlChartTree.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.tlChartTree.OptionsSelection.MultiSelect = true;
			this.tlChartTree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
			this.tlChartTree.OptionsView.ShowColumns = false;
			this.tlChartTree.OptionsView.ShowHorzLines = false;
			this.tlChartTree.OptionsView.ShowIndicator = false;
			this.tlChartTree.OptionsView.ShowVertLines = false;
			this.tlChartTree.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.buttonEditPlus,
			this.buttonEditDelete,
			this.defaultButtonEdit,
			this.buttonEditEye,
			this.buttonEditEyeDelete,
			this.buttonEditDisabledEye,
			this.buttonEditDisabledEyeDelete,
			this.popupContainerEdit});
			this.tlChartTree.StateImageList = this.liTreeImages;
			this.tlChartTree.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.OnChartTreeCustomNodeCellEdit);
			this.tlChartTree.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.OnChartTreeNodeCellStyle);
			this.tlChartTree.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.OnChartTreeFocusedNodeChanged);
			this.tlChartTree.SelectionChanged += new System.EventHandler(this.OnChartTreeSelectionChanged);
			this.tlChartTree.MouseLeave += new System.EventHandler(this.OnChartTreeMouseLeave);
			this.tlChartTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnChartTreeMouseMove);
			resources.ApplyResources(this.treeListColumn1, "treeListColumn1");
			this.treeListColumn1.ColumnEdit = this.defaultButtonEdit;
			this.treeListColumn1.FieldName = "treeListColumn1";
			this.treeListColumn1.Name = "treeListColumn1";
			this.defaultButtonEdit.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.defaultButtonEdit, "defaultButtonEdit");
			this.defaultButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("defaultButtonEdit.Buttons"))), resources.GetString("defaultButtonEdit.Buttons1"), ((int)(resources.GetObject("defaultButtonEdit.Buttons2"))), ((bool)(resources.GetObject("defaultButtonEdit.Buttons3"))), ((bool)(resources.GetObject("defaultButtonEdit.Buttons4"))), ((bool)(resources.GetObject("defaultButtonEdit.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("defaultButtonEdit.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("defaultButtonEdit.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, resources.GetString("defaultButtonEdit.Buttons8"), ((object)(resources.GetObject("defaultButtonEdit.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("defaultButtonEdit.Buttons10"))), ((bool)(resources.GetObject("defaultButtonEdit.Buttons11"))))});
			this.defaultButtonEdit.Name = "defaultButtonEdit";
			this.defaultButtonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.defaultButtonEdit.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.defaultButtonEdit.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditPlus.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditPlus, "buttonEditPlus");
			this.buttonEditPlus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditPlus.Buttons"))), resources.GetString("buttonEditPlus.Buttons1"), ((int)(resources.GetObject("buttonEditPlus.Buttons2"))), ((bool)(resources.GetObject("buttonEditPlus.Buttons3"))), ((bool)(resources.GetObject("buttonEditPlus.Buttons4"))), ((bool)(resources.GetObject("buttonEditPlus.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditPlus.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditPlus.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, resources.GetString("buttonEditPlus.Buttons8"), ((object)(resources.GetObject("buttonEditPlus.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditPlus.Buttons10"))), ((bool)(resources.GetObject("buttonEditPlus.Buttons11"))))});
			this.buttonEditPlus.Name = "buttonEditPlus";
			this.buttonEditPlus.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditPlus.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditPlusPressed);
			this.buttonEditPlus.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditPlus.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditDelete, "buttonEditDelete");
			this.buttonEditDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditDelete.Buttons"))), resources.GetString("buttonEditDelete.Buttons1"), ((int)(resources.GetObject("buttonEditDelete.Buttons2"))), ((bool)(resources.GetObject("buttonEditDelete.Buttons3"))), ((bool)(resources.GetObject("buttonEditDelete.Buttons4"))), ((bool)(resources.GetObject("buttonEditDelete.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditDelete.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditDelete.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, resources.GetString("buttonEditDelete.Buttons8"), ((object)(resources.GetObject("buttonEditDelete.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditDelete.Buttons10"))), ((bool)(resources.GetObject("buttonEditDelete.Buttons11"))))});
			this.buttonEditDelete.Name = "buttonEditDelete";
			this.buttonEditDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditDelete.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditEyeDeletePressed);
			this.buttonEditDelete.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditDelete.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditEye.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditEye, "buttonEditEye");
			this.buttonEditEye.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditEye.Buttons"))), resources.GetString("buttonEditEye.Buttons1"), ((int)(resources.GetObject("buttonEditEye.Buttons2"))), ((bool)(resources.GetObject("buttonEditEye.Buttons3"))), ((bool)(resources.GetObject("buttonEditEye.Buttons4"))), ((bool)(resources.GetObject("buttonEditEye.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditEye.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditEye.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject4, resources.GetString("buttonEditEye.Buttons8"), ((object)(resources.GetObject("buttonEditEye.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditEye.Buttons10"))), ((bool)(resources.GetObject("buttonEditEye.Buttons11"))))});
			this.buttonEditEye.Name = "buttonEditEye";
			this.buttonEditEye.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditEye.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditEyeDeletePressed);
			this.buttonEditEye.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditEye.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditEyeDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditEyeDelete, "buttonEditEyeDelete");
			this.buttonEditEyeDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditEyeDelete.Buttons"))), resources.GetString("buttonEditEyeDelete.Buttons1"), ((int)(resources.GetObject("buttonEditEyeDelete.Buttons2"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons3"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons4"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditEyeDelete.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditEyeDelete.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, resources.GetString("buttonEditEyeDelete.Buttons8"), ((object)(resources.GetObject("buttonEditEyeDelete.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditEyeDelete.Buttons10"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons11")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditEyeDelete.Buttons12"))), resources.GetString("buttonEditEyeDelete.Buttons13"), ((int)(resources.GetObject("buttonEditEyeDelete.Buttons14"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons15"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons16"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons17"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditEyeDelete.Buttons18"))), ((System.Drawing.Image)(resources.GetObject("buttonEditEyeDelete.Buttons19"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject6, resources.GetString("buttonEditEyeDelete.Buttons20"), resources.GetString("buttonEditEyeDelete.Buttons21"), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditEyeDelete.Buttons22"))), ((bool)(resources.GetObject("buttonEditEyeDelete.Buttons23"))))});
			this.buttonEditEyeDelete.Name = "buttonEditEyeDelete";
			this.buttonEditEyeDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditEyeDelete.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditEyeDeletePressed);
			this.buttonEditEyeDelete.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditEyeDelete.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditDisabledEye.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditDisabledEye, "buttonEditDisabledEye");
			this.buttonEditDisabledEye.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditDisabledEye.Buttons"))), resources.GetString("buttonEditDisabledEye.Buttons1"), ((int)(resources.GetObject("buttonEditDisabledEye.Buttons2"))), ((bool)(resources.GetObject("buttonEditDisabledEye.Buttons3"))), ((bool)(resources.GetObject("buttonEditDisabledEye.Buttons4"))), ((bool)(resources.GetObject("buttonEditDisabledEye.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditDisabledEye.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditDisabledEye.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject7, resources.GetString("buttonEditDisabledEye.Buttons8"), resources.GetString("buttonEditDisabledEye.Buttons9"), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditDisabledEye.Buttons10"))), ((bool)(resources.GetObject("buttonEditDisabledEye.Buttons11"))))});
			this.buttonEditDisabledEye.Name = "buttonEditDisabledEye";
			this.buttonEditDisabledEye.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditDisabledEye.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditEyeDeletePressed);
			this.buttonEditDisabledEye.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditDisabledEye.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.buttonEditDisabledEyeDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.buttonEditDisabledEyeDelete, "buttonEditDisabledEyeDelete");
			this.buttonEditDisabledEyeDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons"))), resources.GetString("buttonEditDisabledEyeDelete.Buttons1"), ((int)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons2"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons3"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons4"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject8, resources.GetString("buttonEditDisabledEyeDelete.Buttons8"), resources.GetString("buttonEditDisabledEyeDelete.Buttons9"), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons10"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons11")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons12"))), resources.GetString("buttonEditDisabledEyeDelete.Buttons13"), ((int)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons14"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons15"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons16"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons17"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons18"))), ((System.Drawing.Image)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons19"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, resources.GetString("buttonEditDisabledEyeDelete.Buttons20"), resources.GetString("buttonEditDisabledEyeDelete.Buttons21"), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons22"))), ((bool)(resources.GetObject("buttonEditDisabledEyeDelete.Buttons23"))))});
			this.buttonEditDisabledEyeDelete.Name = "buttonEditDisabledEyeDelete";
			this.buttonEditDisabledEyeDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.buttonEditDisabledEyeDelete.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnButtonEditEyeDeletePressed);
			this.buttonEditDisabledEyeDelete.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.buttonEditDisabledEyeDelete.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			this.popupContainerEdit.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.popupContainerEdit, "popupContainerEdit");
			this.popupContainerEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("popupContainerEdit.Buttons"))), resources.GetString("popupContainerEdit.Buttons1"), ((int)(resources.GetObject("popupContainerEdit.Buttons2"))), ((bool)(resources.GetObject("popupContainerEdit.Buttons3"))), ((bool)(resources.GetObject("popupContainerEdit.Buttons4"))), ((bool)(resources.GetObject("popupContainerEdit.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("popupContainerEdit.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("popupContainerEdit.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject10, resources.GetString("popupContainerEdit.Buttons8"), ((object)(resources.GetObject("popupContainerEdit.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("popupContainerEdit.Buttons10"))), ((bool)(resources.GetObject("popupContainerEdit.Buttons11"))))});
			this.popupContainerEdit.CloseOnLostFocus = false;
			this.popupContainerEdit.Name = "popupContainerEdit";
			this.popupContainerEdit.PopupControl = this.popupContainerControl;
			this.popupContainerEdit.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
			this.popupContainerEdit.ShowPopupCloseButton = false;
			this.popupContainerEdit.QueryResultValue += new DevExpress.XtraEditors.Controls.QueryResultValueEventHandler(this.popupContainerEdit_QueryResultValue);
			this.popupContainerEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.popupContainerEdit_QueryPopUp);
			this.popupContainerEdit.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.popupContainerEdit_Closed);
			this.popupContainerEdit.Popup += new System.EventHandler(this.popupContainerEdit_Popup);
			this.popupContainerEdit.EditValueChanged += new System.EventHandler(this.popupContainerEdit_EditValueChanged);
			this.popupContainerEdit.Click += new System.EventHandler(this.OnChartTreeButtonEditClick);
			this.popupContainerEdit.DoubleClick += new System.EventHandler(this.OnChartTreeButtonDoubleClick);
			resources.ApplyResources(this.popupContainerControl, "popupContainerControl");
			this.popupContainerControl.Name = "popupContainerControl";
			this.liTreeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.liTreeImages, "liTreeImages");
			this.liTreeImages.TransparentColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.popupContainerControl);
			this.Controls.Add(this.tlChartTree);
			this.Name = "ChartStructureControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.tlChartTree)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.defaultButtonEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditPlus)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDelete)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditEye)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditEyeDelete)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDisabledEye)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonEditDisabledEyeDelete)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion        
		private DevExpress.XtraCharts.Designer.Native.ChartTreeList tlChartTree;
		private XtraTreeList.Columns.TreeListColumn treeListColumn1;
		private RepositoryItemImageButtonEdit buttonEditPlus;
		private System.Windows.Forms.ImageList liTreeImages;
		private RepositoryItemImageButtonEdit buttonEditDelete;
		private RepositoryItemImageButtonEdit defaultButtonEdit;
		private RepositoryItemImageButtonEdit buttonEditEye;
		private RepositoryItemImageButtonEdit buttonEditEyeDelete;
		private RepositoryItemImageButtonEdit buttonEditDisabledEye;
		private RepositoryItemImageButtonEdit buttonEditDisabledEyeDelete;
		private RepositoryItemImagePopupContainerEdit popupContainerEdit;
		private XtraEditors.PopupContainerControl popupContainerControl;
	}
}
