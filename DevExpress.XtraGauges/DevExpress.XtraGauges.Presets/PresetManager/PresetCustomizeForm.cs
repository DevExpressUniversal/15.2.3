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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public class PresetCustomizeForm : XtraForm, IPlatformTypeProvider {
		PropertyStore storeCore;
		IGaugeContainer editGaugeContainerCore;
		IGaugeContainer designGaugeContainerCore;
		private SimpleButton duplicateButton;
		private SimpleButton renameButton;
		private System.ComponentModel.IContainer components;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private SimpleButton okButton;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		DXPopupMenu menuCore;
		protected string RegistryStorePath {
			get { return @"Software\Developer Express\XtraGauges\Designer\"; }
		}
		protected override bool GetAllowSkin() {
			return (LookAndFeel != null) && LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
		}
		protected PropertyStore Store {
			get { return storeCore; }
		}
		protected void StoreProperties() {
			if(Store != null) {
				Store.AddForm(this);
				Store.AddProperty("DesignSurfaceWidth", designSurfaceItem.Width);
			}
		}
		protected void RestoreProperties() {
			if(Store != null) {
				Store.RestoreForm(this);
				designSurfaceItem.Width = Store.RestoreIntProperty("DesignSurfaceWidth", designSurfaceItem.Width);
			}
		}
		bool isWinCore;
		public PresetCustomizeForm(IGaugeContainer editValue) {
			this.menuCore = new DXPopupMenu();
			this.editGaugeContainerCore = editValue;
			this.designGaugeContainerCore = ControlLoader.CreateGaugeContainer();
			isWinCore = designGaugeContainerCore.GetType().IsInstanceOfType(this.editGaugeContainerCore);
			InitializeComponent();
			AssignLayout(EditGaugeContainer, DesignGaugeContainer);
			UpdateDesigner();
			xtraScrollableControl1.Controls.Add(DesignControl);
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Size = new Size(800, 600);
			this.Text = "Visual Gauge Control Designer";
			this.MinimumSize = new Size(640, 480);
			autoLayout.Checked = DesignGaugeContainer.AutoLayout;
			Structure.SelectionChanged += OnStructureSelectionChanged;
			Structure.MouseUp += OnStructureMouseUp;
			Structure.KeyDown += OnProcessKey;
			Structure.MouseDoubleClick += OnStructureDoubleClick;
			DesignControl.KeyDown += OnProcessKey;
			DesignGaugeContainer.CustomizeManager.SelectionChanged += OnDesignerSelectionChanged;
			DesignGaugeContainer.ModelChanged += DesignGaugeContainer_ModelChanged;
			xtraScrollableControl1.Layout += DesignControlLayout;
			autoLayout.CheckedChanged += OnAutoLayoutChanged;
			okButton.Click += OnOkButtonClick;
			applyButton.Click += OnApplyButtonClick;
			cancelButton.Click += OnCancelButtonClick;
			saveButton.Click += OnSaveButtonClick;
			loadButton.Click += OnLoadButtonClick;
			addButton.Click += OnAddButtonClick;
			removeButton.Click += OnRemoveButtonClick;
			duplicateButton.Click += OnDuplicateButtonClick;
			renameButton.Click += OnRenameButtonClick;
			ImageCollection btnImages = ImageHelper.CreateImageCollectionFromResources(
					"DevExpress.XtraGauges.Presets.Resources.Images.gauge-designer-icons1.png",
					typeof(PresetCustomizeForm).Assembly,
					new Size(16, 16)
				);
			addButton.Image = btnImages.Images[0];
			removeButton.Image = btnImages.Images[1];
			renameButton.Image = btnImages.Images[2];
			duplicateButton.Image = btnImages.Images[3];
			DesignControl.Size = EditGaugeContainer.Bounds.Size;
			DesignGaugeContainer.EnableCustomizationMode = true;
			this.storeCore = new PropertyStore(this.RegistryStorePath);
			Store.Restore();
		}
		public bool IsWin { get { return isWinCore; } }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			RestoreProperties();
		}
		protected override void OnClosed(EventArgs e) {
			if(Store != null) {
				StoreProperties();
				Store.Store();
			}
			base.OnClosed(e);
		}
		void OnProcessKey(object sender, KeyEventArgs e) {
			BaseGaugeStructureNode node = Structure.SelectedNode as BaseGaugeStructureNode;
			if(node == null) return;
			if(IsSelectionLocked) return;
			LockSelection();
			switch(e.KeyCode) {
				case Keys.F5:
				case Keys.Add:
					if(node.AllowDuplicate) {
						node.OnDuplicate();
					}
					break;
				case Keys.F2:
					if(node.AllowRename) {
						node.OnRename();
					}
					break;
				case Keys.Subtract:
				case Keys.Delete:
					if(node.AllowRemove) {
						node.OnRemove();
						DesignGaugeContainer.CustomizeManager.SelectedClient = null;
					}
					break;
				case Keys.Escape:
					if(node.Parent != null) {
						Structure.SelectNode(node.Parent);
						UpdateDesignerSelection();
					}
					break;
			}
			DesignControl.Invalidate();
			UnlockSelection();
		}
		void DesignGaugeContainer_ModelChanged(object sender, EventArgs e) {
			if(IsDesignerUpdateLocked) return;
			UpdateDesigner();
		}
		public void UpdateDesigner() {
			Structure.UpdateTreeView(DesignGaugeContainer);
			Structure.ExpandAll();
			UpdateStructureButtons();
		}
		int lockSelectionCounter = 0;
		protected bool IsSelectionLocked { get { return lockSelectionCounter > 0; } }
		protected void LockSelection() { lockSelectionCounter++; }
		protected void UnlockSelection() { --lockSelectionCounter; }
		int lockDesignerUpdateCounter = 0;
		protected bool IsDesignerUpdateLocked { get { return lockDesignerUpdateCounter > 0; } }
		protected internal void BeginDesignerUpdate() { lockDesignerUpdateCounter++; }
		protected internal void EndDesignerUpdate() { if(--lockDesignerUpdateCounter == 0) UpdateDesigner(); }
		protected void CancelDesignerUpdate() { --lockDesignerUpdateCounter; }
		void OnDesignerSelectionChanged(object sender, EventArgs e) {
			if(IsSelectionLocked) return;
			LockSelection();
			Structure.UpdateSelection(DesignGaugeContainer.CustomizeManager.SelectedClient);
			UnlockSelection();
		}
		void OnStructureSelectionChanged(object sender, EventArgs e) {
			UpdateStructureButtons();
			if(IsSelectionLocked) return;
			LockSelection();
			UpdateDesignerSelection();
			DesignControl.Invalidate();
			UnlockSelection();
		}
		protected void UpdateDesignerSelection() {
			BaseGaugeStructureNode structNode = Structure.SelNode as BaseGaugeStructureNode;
			DesignGaugeContainer.CustomizeManager.SelectedClient = (structNode != null) ?
				structNode.CustomizationFrameClient : null;
		}
		protected void UpdateStructureButtons() {
			BaseGaugeStructureNode structureNode = Structure.SelNode as BaseGaugeStructureNode;
			addButton.Enabled = (structureNode != null) ? structureNode.AllowAdd : false;
			removeButton.Enabled = (structureNode != null) ? structureNode.AllowRemove : false;
			renameButton.Enabled = (structureNode != null) ? structureNode.AllowRename : false;
			duplicateButton.Enabled = (structureNode != null) ? structureNode.AllowDuplicate : false;
		}
		protected void AssignLayout(IGaugeContainer sourceContainer, IGaugeContainer targetContainer) {
			BeginDesignerUpdate();
			LockSelection();
			using(MemoryStream ms = new MemoryStream()) {
				sourceContainer.SaveLayoutToStream(ms);
				ms.Seek(0, SeekOrigin.Begin);
				targetContainer.RestoreLayoutFromStream(ms);
				ms.Close();
			}
			UnlockSelection();
			CancelDesignerUpdate();
		}
		void DesignControlLayout(object sender, LayoutEventArgs e) {
			if(DesignControl == null) return;
			DesignControl.Location = new Point(
					Math.Max(10, (xtraScrollableControl1.ClientSize.Width - DesignControl.Width) / 2),
					Math.Max(10, (xtraScrollableControl1.ClientSize.Height - DesignControl.Height) / 2)
				);
		}
		protected void OnOkButtonClick(object sender, EventArgs e) {
			AssignLayout(DesignGaugeContainer, EditGaugeContainer);
			DialogResult = DialogResult.OK;
		}
		protected void OnApplyButtonClick(object sender, EventArgs e) {
			AssignLayout(DesignGaugeContainer, EditGaugeContainer);
		}
		protected void OnCancelButtonClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		protected void OnAddButtonClick(object sender, EventArgs e) {
			if(IsSelectionLocked) return;
			LockSelection();
			BaseGaugeStructureNode node = Structure.SelectedNode as BaseGaugeStructureNode;
			if(node != null) node.OnAdd();
			DesignControl.Invalidate();
			UnlockSelection();
		}
		protected void OnRemoveButtonClick(object sender, EventArgs e) {
			if(IsSelectionLocked) return;
			LockSelection();
			BaseGaugeStructureNode node = Structure.SelectedNode as BaseGaugeStructureNode;
			if(node != null) node.OnRemove();
			DesignGaugeContainer.CustomizeManager.SelectedClient = null;
			DesignControl.Invalidate();
			UnlockSelection();
		}
		protected void OnDuplicateButtonClick(object sender, EventArgs e) {
			if(IsSelectionLocked) return;
			LockSelection();
			BaseGaugeStructureNode node = Structure.SelectedNode as BaseGaugeStructureNode;
			if(node != null) node.OnDuplicate();
			DesignControl.Invalidate();
			UnlockSelection();
		}
		protected void OnRenameButtonClick(object sender, EventArgs e) {
			if(IsSelectionLocked) return;
			LockSelection();
			BaseGaugeStructureNode node = Structure.SelectedNode as BaseGaugeStructureNode;
			if(node != null) node.OnRename();
			DesignControl.Invalidate();
			UnlockSelection();
		}
		protected void OnSaveButtonClick(object sender, EventArgs e) {
			SaveFileDialog dialog = CreateSaveFileDialog();
			if(DesignGaugeContainer != null) {
				DialogResult result = dialog.ShowDialog(this);
				if(result == DialogResult.OK)
					try {
						DesignGaugeContainer.SaveLayoutToXml(dialog.FileName);
					}
					catch(Exception ex) {
						DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
					}
			}
		}
		protected void OnLoadButtonClick(object sender, EventArgs e) {
			OpenFileDialog dialog = CreateOpenFileDialog();
			if(DesignGaugeContainer != null) {
				DialogResult result = dialog.ShowDialog(this);
				if(result == DialogResult.OK)
					try {
						BeginDesignerUpdate();
						LockSelection();
						DesignGaugeContainer.RestoreLayoutFromXml(dialog.FileName);
						UnlockSelection();
						EndDesignerUpdate();
					}
					catch(Exception ex) {
						DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
					}
			}
		}
		protected virtual SaveFileDialog CreateSaveFileDialog() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			return dialog;
		}
		protected virtual OpenFileDialog CreateOpenFileDialog() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			return dialog;
		}
		public IGaugeContainer EditGaugeContainer {
			get { return editGaugeContainerCore; }
		}
		public IGaugeContainer DesignGaugeContainer {
			get { return designGaugeContainerCore; }
		}
		public Control DesignControl {
			get { return designGaugeContainerCore as Control; }
		}
		public GaugeStructureTree Structure {
			get { return structureTree; }
		}
		protected DXPopupMenu RBMenu {
			get { return menuCore; }
		}
		protected void OnStructureDoubleClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				BaseGaugeStructureNode node = Structure.GetNodeAt(e.Location) as BaseGaugeStructureNode;
				if(node != null && node.CustomizationFrameClient != null) {
					var hitTest = Structure.HitTest(e.Location);
					if(hitTest.Location == TreeViewHitTestLocations.Label || hitTest.Location == TreeViewHitTestLocations.Image) {
						CustomizeActionInfo[] actions = node.CustomizationFrameClient.GetActions();
						if(actions.Length > 0) InvokeMenuAction(node.CustomizationFrameClient, actions[0]);
					}
				}
			}
		}
		protected void OnStructureMouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				BaseGaugeStructureNode node = Structure.GetNodeAt(e.Location) as BaseGaugeStructureNode;
				if(node != null) {
					if(!node.IsSelected) Structure.SelectNode(node);
					var hitTest = Structure.HitTest(e.Location);
					if(hitTest.Location == TreeViewHitTestLocations.Label || hitTest.Location == TreeViewHitTestLocations.Image)
						ShowMenuCore(node, e.Location);
				}
			}
		}
		Dictionary<string, string[]> categories = new Dictionary<string, string[]>();
		protected virtual void ShowMenuCore(BaseGaugeStructureNode node, Point pt) {
			if(node.CustomizationFrameClient == null) return;
			CustomizeActionInfo[] info = node.CustomizationFrameClient.GetActions();
			if(!isWinCore) {
				List<CustomizeActionInfo> infoList = info.ToList();
				infoList.RemoveAll(p => p.ActionName.Equals("Add Image") || p.ActionName.Equals("Add State Image Indicator"));
				info = infoList.ToArray();
			}
			RBMenu.Items.Clear();
			if(categories.Count == 0)
				InitCategories();
			if(categories.ContainsKey(node.Text))
				foreach(string item in categories[node.Text]) {
					AddMenuItem(item, info);
				}
			else
				AddMenuItem(string.Empty, info);
			MenuManagerHelper.ShowMenu(RBMenu, UserLookAndFeel.Default, null, Structure, pt);
		}
		private void InitCategories() {
			categories.Add("Pointers", new string[] { "Pointers" });
			categories.Add("Layers", new string[] { "Layers" });
			categories.Add("Scales", new string[] { "Scales", "Pointers", "Layers", "Other Elements" });
			categories.Add("Other Elements", new string[] { "Other Elements" });
			categories.Add("Labels", new string[] { "Labels" });
			categories.Add("Images", new string[] { "Images" });
			categories.Add("Indicators", new string[] { "Indicators" });
		}
		private void AddMenuItem(string groupIdAction, CustomizeActionInfo[] info) {
			foreach(CustomizeActionInfo tInfo in info) {
				if((tInfo.GroupIdAction == groupIdAction) || string.IsNullOrEmpty(groupIdAction)) {
					DXMenuItem mItem = new DXMenuItem(tInfo.ActionName, OnMenuItemClick, tInfo.Image);
					mItem.Tag = tInfo;
					RBMenu.Items.Add(mItem);
				}
			}
		}
		protected void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem mItem = sender as DXMenuItem;
			if(mItem == null) return;
			object instance =
				(DesignGaugeContainer.CustomizeManager.SelectedClient as BaseGaugeModel != null) ?
				(object)(DesignGaugeContainer.CustomizeManager.SelectedClient as BaseGaugeModel).Owner :
				(object)DesignGaugeContainer.CustomizeManager.SelectedClient;
			InvokeMenuAction(instance ?? mItem.Tag, mItem.Tag as CustomizeActionInfo);
		}
		void InvokeMenuAction(object instance, CustomizeActionInfo cInfo) {
			if(instance == null || cInfo == null) return;
			MethodInfo mi = FindMethod(instance, cInfo);
			bool isGauge = instance is IGauge;
			if(mi != null) {
				if(isGauge && mi.Name.Equals("RunDesigner"))
					BeginInvoke(new MethodInvoker(delegate() { mi.Invoke(instance, new object[] { isWinCore }); }));
				else
					BeginInvoke(new MethodInvoker(delegate() { mi.Invoke(instance, new object[] { }); }));
			}
		}
		static MethodInfo FindMethod(object instance, CustomizeActionInfo cInfo) {
			Type targetType = instance.GetType();
			MethodInfo mi = targetType.GetMethod(cInfo.MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi == null)
				mi = targetType.GetMethod(cInfo.MethodName, BindingFlags.Instance | BindingFlags.Public);
			return mi;
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresetCustomizeForm));
			this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.duplicateButton = new DevExpress.XtraEditors.SimpleButton();
			this.renameButton = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.structureTree = new DevExpress.XtraGauges.Presets.PresetManager.GaugeStructureTree();
			this.removeButton = new DevExpress.XtraEditors.SimpleButton();
			this.addButton = new DevExpress.XtraEditors.SimpleButton();
			this.autoLayout = new DevExpress.XtraEditors.CheckEdit();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.saveButton = new DevExpress.XtraEditors.SimpleButton();
			this.loadButton = new DevExpress.XtraEditors.SimpleButton();
			this.applyButton = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.designSurfaceItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.autoLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.designSurfaceItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			this.SuspendLayout();
			this.xtraScrollableControl1.AutoScrollMargin = new System.Drawing.Size(10, 10);
			this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
			this.xtraScrollableControl1.Name = "xtraScrollableControl1";
			this.xtraScrollableControl1.Size = new System.Drawing.Size(470, 529);
			this.xtraScrollableControl1.TabIndex = 1;
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.okButton);
			this.layoutControl1.Controls.Add(this.duplicateButton);
			this.layoutControl1.Controls.Add(this.renameButton);
			this.layoutControl1.Controls.Add(this.panelControl1);
			this.layoutControl1.Controls.Add(this.removeButton);
			this.layoutControl1.Controls.Add(this.addButton);
			this.layoutControl1.Controls.Add(this.autoLayout);
			this.layoutControl1.Controls.Add(this.cancelButton);
			this.layoutControl1.Controls.Add(this.xtraScrollableControl1);
			this.layoutControl1.Controls.Add(this.saveButton);
			this.layoutControl1.Controls.Add(this.loadButton);
			this.layoutControl1.Controls.Add(this.applyButton);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(792, 573);
			this.layoutControl1.TabIndex = 2;
			this.layoutControl1.Text = "layoutControl1";
			this.okButton.Location = new System.Drawing.Point(530, 541);
			this.okButton.MaximumSize = new System.Drawing.Size(80, 22);
			this.okButton.MinimumSize = new System.Drawing.Size(80, 22);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 22);
			this.okButton.StyleController = this.layoutControl1;
			this.okButton.TabIndex = 15;
			this.okButton.Text = "Ok";
			this.duplicateButton.Location = new System.Drawing.Point(695, 51);
			this.duplicateButton.MaximumSize = new System.Drawing.Size(75, 22);
			this.duplicateButton.MinimumSize = new System.Drawing.Size(75, 22);
			this.duplicateButton.Name = "duplicateButton";
			this.duplicateButton.Size = new System.Drawing.Size(75, 22);
			this.duplicateButton.StyleController = this.layoutControl1;
			this.duplicateButton.TabIndex = 14;
			this.duplicateButton.Text = "Duplicate";
			this.renameButton.Location = new System.Drawing.Point(621, 51);
			this.renameButton.MaximumSize = new System.Drawing.Size(70, 22);
			this.renameButton.MinimumSize = new System.Drawing.Size(70, 22);
			this.renameButton.Name = "renameButton";
			this.renameButton.Size = new System.Drawing.Size(70, 22);
			this.renameButton.StyleController = this.layoutControl1;
			this.renameButton.TabIndex = 13;
			this.renameButton.Text = "Rename";
			this.panelControl1.Controls.Add(this.structureTree);
			this.panelControl1.Location = new System.Drawing.Point(483, 79);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(301, 336);
			this.panelControl1.TabIndex = 12;
			this.structureTree.AllowHScrollBar = true;
			this.structureTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.structureTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.structureTree.ImageIndex = 0;
			this.structureTree.Location = new System.Drawing.Point(2, 2);
			this.structureTree.Name = "structureTree";
			this.structureTree.SelectedImageIndex = 0;
			this.structureTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.structureTree.Size = new System.Drawing.Size(297, 332);
			this.structureTree.TabIndex = 9;
			this.removeButton.Location = new System.Drawing.Point(547, 51);
			this.removeButton.MaximumSize = new System.Drawing.Size(70, 22);
			this.removeButton.MinimumSize = new System.Drawing.Size(70, 22);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(70, 22);
			this.removeButton.StyleController = this.layoutControl1;
			this.removeButton.TabIndex = 11;
			this.removeButton.Text = "Remove";
			this.addButton.Location = new System.Drawing.Point(483, 51);
			this.addButton.MaximumSize = new System.Drawing.Size(60, 22);
			this.addButton.MinimumSize = new System.Drawing.Size(60, 22);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(60, 22);
			this.addButton.StyleController = this.layoutControl1;
			this.addButton.TabIndex = 10;
			this.addButton.Text = "Add";
			this.autoLayout.Location = new System.Drawing.Point(483, 8);
			this.autoLayout.Name = "autoLayout";
			this.autoLayout.Properties.Caption = "Enable Auto Layout";
			this.autoLayout.Size = new System.Drawing.Size(301, 19);
			this.autoLayout.StyleController = this.layoutControl1;
			this.autoLayout.TabIndex = 8;
			this.cancelButton.Location = new System.Drawing.Point(616, 541);
			this.cancelButton.MaximumSize = new System.Drawing.Size(80, 22);
			this.cancelButton.MinimumSize = new System.Drawing.Size(80, 22);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 22);
			this.cancelButton.StyleController = this.layoutControl1;
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			this.saveButton.Location = new System.Drawing.Point(565, 471);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(137, 22);
			this.saveButton.StyleController = this.layoutControl1;
			this.saveButton.TabIndex = 4;
			this.saveButton.Text = "Save Layout";
			this.loadButton.Location = new System.Drawing.Point(565, 443);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(137, 22);
			this.loadButton.StyleController = this.layoutControl1;
			this.loadButton.TabIndex = 5;
			this.loadButton.Text = "Load Layout";
			this.applyButton.Location = new System.Drawing.Point(702, 541);
			this.applyButton.MaximumSize = new System.Drawing.Size(80, 22);
			this.applyButton.MinimumSize = new System.Drawing.Size(80, 22);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(80, 22);
			this.applyButton.StyleController = this.layoutControl1;
			this.applyButton.TabIndex = 6;
			this.applyButton.Text = "Apply";
			this.layoutControlGroup1.AllowDrawBackground = false;
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleSeparator1,
			this.emptySpaceItem1,
			this.layoutControlItem1,
			this.splitterItem1,
			this.designSurfaceItem,
			this.layoutControlGroup2,
			this.layoutControlItem12,
			this.layoutControlItem5});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(792, 573);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.simpleSeparator1.AllowHotTrack = false;
			this.simpleSeparator1.CustomizationFormText = "simpleSeparator1";
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 529);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Size = new System.Drawing.Size(792, 2);
			this.simpleSeparator1.Text = "simpleSeparator1";
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 531);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(527, 42);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.Control = this.applyButton;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(699, 531);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 10, 10, 10);
			this.layoutControlItem1.Size = new System.Drawing.Size(93, 42);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.CustomizationFormText = "splitterItem1";
			this.splitterItem1.Location = new System.Drawing.Point(470, 0);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(5, 529);
			this.designSurfaceItem.Control = this.xtraScrollableControl1;
			this.designSurfaceItem.CustomizationFormText = "layoutControlItem4";
			this.designSurfaceItem.Location = new System.Drawing.Point(0, 0);
			this.designSurfaceItem.Name = "designSurfaceItem";
			this.designSurfaceItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.designSurfaceItem.Size = new System.Drawing.Size(470, 529);
			this.designSurfaceItem.Text = "designSurfaceItem";
			this.designSurfaceItem.TextSize = new System.Drawing.Size(0, 0);
			this.designSurfaceItem.TextToControlDistance = 0;
			this.designSurfaceItem.TextVisible = false;
			this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem6,
			this.layoutControlItem10,
			this.layoutControlItem8,
			this.layoutControlItem9,
			this.layoutControlItem7,
			this.layoutControlItem11,
			this.layoutControlItem2,
			this.emptySpaceItem2,
			this.layoutControlItem3});
			this.layoutControlGroup2.Location = new System.Drawing.Point(475, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(317, 529);
			this.layoutControlGroup2.Text = "layoutControlGroup2";
			this.layoutControlItem6.Control = this.autoLayout;
			this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 8, 16);
			this.layoutControlItem6.Size = new System.Drawing.Size(317, 43);
			this.layoutControlItem6.Text = "layoutControlItem6";
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextToControlDistance = 0;
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem10.Control = this.panelControl1;
			this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
			this.layoutControlItem10.Location = new System.Drawing.Point(0, 76);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 3, 8);
			this.layoutControlItem10.Size = new System.Drawing.Size(317, 347);
			this.layoutControlItem10.Text = "layoutControlItem10";
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextToControlDistance = 0;
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem8.Control = this.addButton;
			this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 43);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 2, 8, 3);
			this.layoutControlItem8.Size = new System.Drawing.Size(70, 33);
			this.layoutControlItem8.Text = "layoutControlItem8";
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextToControlDistance = 0;
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem9.Control = this.removeButton;
			this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
			this.layoutControlItem9.Location = new System.Drawing.Point(70, 43);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 8, 3);
			this.layoutControlItem9.Size = new System.Drawing.Size(74, 33);
			this.layoutControlItem9.Text = "layoutControlItem9";
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextToControlDistance = 0;
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem7.Control = this.renameButton;
			this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
			this.layoutControlItem7.Location = new System.Drawing.Point(144, 43);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 8, 3);
			this.layoutControlItem7.Size = new System.Drawing.Size(74, 33);
			this.layoutControlItem7.Text = "layoutControlItem7";
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextToControlDistance = 0;
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem11.Control = this.duplicateButton;
			this.layoutControlItem11.CustomizationFormText = "layoutControlItem11";
			this.layoutControlItem11.Location = new System.Drawing.Point(218, 43);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 8, 3);
			this.layoutControlItem11.Size = new System.Drawing.Size(79, 33);
			this.layoutControlItem11.Text = "layoutControlItem11";
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextToControlDistance = 0;
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem2.Control = this.saveButton;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 468);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(90, 90, 3, 36);
			this.layoutControlItem2.Size = new System.Drawing.Size(317, 61);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(297, 43);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(20, 33);
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.Control = this.loadButton;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 423);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(90, 90, 20, 3);
			this.layoutControlItem3.Size = new System.Drawing.Size(317, 45);
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem12.Control = this.okButton;
			this.layoutControlItem12.CustomizationFormText = "layoutControlItem12";
			this.layoutControlItem12.Location = new System.Drawing.Point(527, 531);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 10);
			this.layoutControlItem12.Size = new System.Drawing.Size(86, 42);
			this.layoutControlItem12.Text = "layoutControlItem12";
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextToControlDistance = 0;
			this.layoutControlItem12.TextVisible = false;
			this.layoutControlItem5.Control = this.cancelButton;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.Location = new System.Drawing.Point(613, 531);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 10);
			this.layoutControlItem5.Size = new System.Drawing.Size(86, 42);
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 573);
			this.Controls.Add(this.layoutControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PresetCustomizeForm";
			this.Text = "Current";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.autoLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.designSurfaceItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			this.ResumeLayout(false);
		}
		private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
		private SimpleButton saveButton;
		private SimpleButton loadButton;
		private DevExpress.XtraLayout.LayoutControlItem designSurfaceItem;
		private SimpleButton cancelButton;
		private SimpleButton applyButton;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.SplitterItem splitterItem1;
		private CheckEdit autoLayout;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private GaugeStructureTree structureTree;
		private SimpleButton removeButton;
		private SimpleButton addButton;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
		private PanelControl panelControl1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
		void OnAutoLayoutChanged(object sender, EventArgs e) {
			DesignGaugeContainer.AutoLayout = autoLayout.Checked;
		}
	}
}
