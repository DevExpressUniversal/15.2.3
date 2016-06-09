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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.IO;
using System.Drawing;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Skins.Info;
using DevExpress.Utils.Design;
namespace DevExpress.XtraTreeList.Design {
	public class xNodesEditor : DevExpress.XtraEditors.XtraForm {
		private Panel tlPreview;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.SimpleButton btnAddRoot;
		private DevExpress.XtraEditors.SimpleButton btnAddChild;
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.GroupControl groupBox1;
		private DevExpress.XtraEditors.GroupControl groupBox2;
		private DevExpress.XtraEditors.SimpleButton btnBestFit;
		private Panel panel1;
		private XtraEditors.ImageComboBoxEdit piState;
		private XtraEditors.LabelControl lbState;
		private XtraEditors.ImageComboBoxEdit piSelected;
		private XtraEditors.LabelControl lbSelected;
		private XtraEditors.ImageComboBoxEdit piImage;
		private XtraEditors.LabelControl lbImage;
		private Panel panel2;
		private XtraEditors.SpinEdit seState;
		private XtraEditors.SpinEdit seSelected;
		private XtraEditors.SpinEdit seImage;
		private XtraEditors.LabelControl label3;
		private XtraEditors.LabelControl label4;
		private XtraEditors.LabelControl label5;
		private TreeListNodes nodes;
		public xNodesEditor(object nodes) {
			this.nodes = nodes as TreeListNodes;
			InitializeComponent();
			InitData();
			InitButtonImages();
			InitStyle();
		}
		protected void InitStyle() {
			SkinManager.EnableFormSkins();
			SkinBlobXmlCreator skinCreator = new SkinBlobXmlCreator("DevExpress Design",
				"DevExpress.Utils.Design.", typeof(XtraDesignForm).Assembly, null);
			SkinManager.Default.RegisterSkin(skinCreator);
			LookAndFeel.SetSkinStyle("DevExpress Design");
		}
		private void InitializeComponent() {
			this.tlPreview = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAddRoot = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddChild = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.piState = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbState = new DevExpress.XtraEditors.LabelControl();
			this.piSelected = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbSelected = new DevExpress.XtraEditors.LabelControl();
			this.piImage = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbImage = new DevExpress.XtraEditors.LabelControl();
			this.groupBox2 = new DevExpress.XtraEditors.GroupControl();
			this.panel2 = new System.Windows.Forms.Panel();
			this.seState = new DevExpress.XtraEditors.SpinEdit();
			this.seSelected = new DevExpress.XtraEditors.SpinEdit();
			this.seImage = new DevExpress.XtraEditors.SpinEdit();
			this.label3 = new DevExpress.XtraEditors.LabelControl();
			this.label4 = new DevExpress.XtraEditors.LabelControl();
			this.label5 = new DevExpress.XtraEditors.LabelControl();
			this.btnBestFit = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.piState.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.piSelected.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.piImage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seState.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seSelected.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seImage.Properties)).BeginInit();
			this.SuspendLayout();
			this.tlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tlPreview.Location = new System.Drawing.Point(8, 55);
			this.tlPreview.Name = "tlPreview";
			this.tlPreview.Size = new System.Drawing.Size(534, 194);
			this.tlPreview.TabIndex = 0;
			this.label1.Location = new System.Drawing.Point(5, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select node to edit:";
			this.btnAddRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAddRoot.ImageIndex = 0;
			this.btnAddRoot.Location = new System.Drawing.Point(8, 264);
			this.btnAddRoot.Name = "btnAddRoot";
			this.btnAddRoot.Size = new System.Drawing.Size(108, 28);
			this.btnAddRoot.TabIndex = 1;
			this.btnAddRoot.Text = "Add Root";
			this.btnAddRoot.Click += new System.EventHandler(this.btnAddRoot_Click);
			this.btnAddChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAddChild.ImageIndex = 1;
			this.btnAddChild.Location = new System.Drawing.Point(128, 264);
			this.btnAddChild.Name = "btnAddChild";
			this.btnAddChild.Size = new System.Drawing.Size(108, 28);
			this.btnAddChild.TabIndex = 2;
			this.btnAddChild.Text = "Add Child";
			this.btnAddChild.Click += new System.EventHandler(this.btnAddChild_Click);
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDelete.ImageIndex = 2;
			this.btnDelete.Location = new System.Drawing.Point(248, 264);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(108, 28);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(362, 493);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(84, 28);
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(458, 493);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(84, 28);
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "Cancel";
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Location = new System.Drawing.Point(8, 304);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(534, 84);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.Text = "Image Indexes";
			this.panel1.Controls.Add(this.piState);
			this.panel1.Controls.Add(this.lbState);
			this.panel1.Controls.Add(this.piSelected);
			this.panel1.Controls.Add(this.lbSelected);
			this.panel1.Controls.Add(this.piImage);
			this.panel1.Controls.Add(this.lbImage);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(2, 21);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(530, 61);
			this.panel1.TabIndex = 7;
			this.piState.Location = new System.Drawing.Point(226, 23);
			this.piState.Name = "piState";
			this.piState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.piState.Size = new System.Drawing.Size(96, 20);
			this.piState.TabIndex = 12;
			this.piState.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.piState_ValueChanging);
			this.lbState.Location = new System.Drawing.Point(226, 7);
			this.lbState.Name = "lbState";
			this.lbState.Size = new System.Drawing.Size(63, 13);
			this.lbState.TabIndex = 9;
			this.lbState.Text = "State Image:";
			this.piSelected.Location = new System.Drawing.Point(118, 23);
			this.piSelected.Name = "piSelected";
			this.piSelected.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.piSelected.Size = new System.Drawing.Size(96, 20);
			this.piSelected.TabIndex = 11;
			this.piSelected.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.piSelected_ValueChanging);
			this.lbSelected.Location = new System.Drawing.Point(118, 7);
			this.lbSelected.Name = "lbSelected";
			this.lbSelected.Size = new System.Drawing.Size(78, 13);
			this.lbSelected.TabIndex = 8;
			this.lbSelected.Text = "Selected Image:";
			this.piImage.Location = new System.Drawing.Point(10, 23);
			this.piImage.Name = "piImage";
			this.piImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.piImage.Size = new System.Drawing.Size(96, 20);
			this.piImage.TabIndex = 10;
			this.piImage.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.piImage_ValueChanging);
			this.lbImage.Location = new System.Drawing.Point(10, 7);
			this.lbImage.Name = "lbImage";
			this.lbImage.Size = new System.Drawing.Size(34, 13);
			this.lbImage.TabIndex = 7;
			this.lbImage.Text = "Image:";
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.panel2);
			this.groupBox2.Location = new System.Drawing.Point(8, 397);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(534, 84);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.Text = "Default Image Indexes";
			this.panel2.Controls.Add(this.seState);
			this.panel2.Controls.Add(this.seSelected);
			this.panel2.Controls.Add(this.seImage);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(2, 21);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(530, 61);
			this.panel2.TabIndex = 8;
			this.seState.EditValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.seState.Location = new System.Drawing.Point(226, 22);
			this.seState.Name = "seState";
			this.seState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seState.Properties.IsFloatValue = false;
			this.seState.Properties.Mask.EditMask = "N00";
			this.seState.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seState.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.seState.Size = new System.Drawing.Size(96, 20);
			this.seState.TabIndex = 13;
			this.seSelected.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seSelected.Location = new System.Drawing.Point(118, 22);
			this.seSelected.Name = "seSelected";
			this.seSelected.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seSelected.Properties.IsFloatValue = false;
			this.seSelected.Properties.Mask.EditMask = "N00";
			this.seSelected.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seSelected.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.seSelected.Size = new System.Drawing.Size(96, 20);
			this.seSelected.TabIndex = 12;
			this.seImage.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seImage.Location = new System.Drawing.Point(10, 22);
			this.seImage.Name = "seImage";
			this.seImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seImage.Properties.IsFloatValue = false;
			this.seImage.Properties.Mask.EditMask = "N00";
			this.seImage.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.seImage.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.seImage.Size = new System.Drawing.Size(96, 20);
			this.seImage.TabIndex = 11;
			this.label3.Location = new System.Drawing.Point(226, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "State Image:";
			this.label4.Location = new System.Drawing.Point(118, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Selected Image:";
			this.label5.Location = new System.Drawing.Point(10, 6);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(34, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Image:";
			this.btnBestFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBestFit.ImageIndex = 3;
			this.btnBestFit.Location = new System.Drawing.Point(398, 10);
			this.btnBestFit.Name = "btnBestFit";
			this.btnBestFit.Size = new System.Drawing.Size(144, 28);
			this.btnBestFit.TabIndex = 12;
			this.btnBestFit.Text = "Best Fit (all columns)";
			this.btnBestFit.Click += new System.EventHandler(this.btnBestFit_Click);
			this.ClientSize = new System.Drawing.Size(550, 530);
			this.ControlBox = false;
			this.Controls.Add(this.btnBestFit);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAddChild);
			this.Controls.Add(this.btnAddRoot);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tlPreview);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(348, 329);
			this.Name = "xNodesEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Nodes Editor";
			this.Load += new System.EventHandler(this.xNodesEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.piState.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.piSelected.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.piImage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.seState.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seSelected.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seImage.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		private void InitButtonImages() {
			ImageCollection iml = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Design.Editors.Images.bmp", typeof(xNodesEditor).Assembly, new Size(16, 16));
			btnAddRoot.Image = iml.Images[0];
			btnAddChild.Image = iml.Images[1];
			btnDelete.Image = iml.Images[2];
			btnBestFit.Image = iml.Images[3];
		}
		private void NodeChanged() {
			bool b = FocusedNode != null;
			btnAddChild.Enabled = btnDelete.Enabled = b;
			piImage.Enabled = lbImage.Enabled = b && treeListPreview.SelectImageList != null;
			piSelected.Enabled = lbSelected.Enabled = b && treeListPreview.SelectImageList != null;
			piState.Enabled = lbState.Enabled = b && treeListPreview.StateImageList != null;
			if(b) {
				piImage.EditValue = FocusedNode.ImageIndex;
				piSelected.EditValue = FocusedNode.SelectImageIndex;
				piState.EditValue = FocusedNode.StateImageIndex;
			} else 
				piImage.EditValue = piSelected.EditValue = piState.EditValue = -1;
		}
		private void treeListPreview_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e) {
			NodeChanged();			
		}
		private void SetNodeImageIndexes(TreeListNode node) {
			try {
				node.ImageIndex = Convert.ToInt32(seImage.EditValue);
				node.SelectImageIndex = Convert.ToInt32(seSelected.EditValue);
				node.StateImageIndex = Convert.ToInt32(seState.EditValue);
			} catch {}
		}
		private void btnAddRoot_Click(object sender, System.EventArgs e) {
			treeListPreview.FocusedNode = treeListPreview.AppendNode(null, null);
			SetNodeImageIndexes(treeListPreview.FocusedNode);
			NodeChanged();
		}
		private void btnAddChild_Click(object sender, System.EventArgs e) {
			treeListPreview.FocusedNode = treeListPreview.AppendNode(null, FocusedNode);
			SetNodeImageIndexes(treeListPreview.FocusedNode);
			NodeChanged();
		}
		private void btnDelete_Click(object sender, System.EventArgs e) {
			treeListPreview.DeleteNode(FocusedNode);
			NodeChanged();
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			SetData();
		}
		public TreeList TreeList { get { return nodes.TreeList; } }
		private TreeListNode FocusedNode { get { return treeListPreview.FocusedNode; } }
		private static object[] GetNodeData(TreeListNode node, int dim) {
			object[] ret = new object[dim];
			for(int i = 0; i < dim; i++)
				ret.SetValue(node.GetValue(i), i);
			return ret;
		}
		private static TreeListNode CreateNewNode(TreeListNode node, TreeList tl, TreeListNode parentNode) {
			TreeListNode newNode = tl.AppendNode(GetNodeData(node, tl.Columns.Count), parentNode); 
			newNode.ImageIndex = node.ImageIndex;
			newNode.SelectImageIndex = node.SelectImageIndex;
			newNode.StateImageIndex = node.StateImageIndex;
			newNode.CheckState = node.CheckState;
			return newNode;
		}
		internal static void AddNodes(TreeListNodes fromNodes, TreeListNodes toNodes, TreeListNode parent) {
			TreeListNode newNode = null;
			foreach(TreeListNode node in fromNodes) {
				newNode = CreateNewNode(node, toNodes.TreeList, parent);
				AddNodes(node.Nodes, newNode.Nodes, newNode);
			}
		}
		private void InitData() {
			if(TreeList == null) return;
			SetView();
			SetExView(); 
			treeListPreview.BeginUnboundLoad();
			AddNodes(TreeList.Nodes, treeListPreview.Nodes, null);
			treeListPreview.EndUnboundLoad();
			treeListPreview.OptionsView.AutoWidth = false;
			treeListPreview.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Single;
			treeListPreview.ExpandAll();
			InitImages();
			NodeChanged();
		}
		private void AddPickImageItems(DevExpress.XtraEditors.Controls.ImageComboBoxItemCollection items, int count) {
			for(int i = -1; i < count; i++) {
				items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(i == -1 ? "(none)" : i.ToString() + " image", i, i));
			}
		}
		private bool init = false;
		private void InitImages() {
			piImage.Properties.SmallImages = treeListPreview.SelectImageList;
			piSelected.Properties.SmallImages = treeListPreview.SelectImageList;
			piState.Properties.SmallImages = treeListPreview.StateImageList;
			int n = 0;
			if(treeListPreview.SelectImageList != null) n = ImageCollection.GetImageListImageCount(treeListPreview.SelectImageList);
			AddPickImageItems(piImage.Properties.Items, n);
			AddPickImageItems(piSelected.Properties.Items, n);
			n = 0; 
			if(treeListPreview.StateImageList != null) n = ImageCollection.GetImageListImageCount(treeListPreview.StateImageList);
			AddPickImageItems(piState.Properties.Items, n);
			init = true;
			piImage.EditValue = piSelected.EditValue = piState.EditValue = -2;
			init = false;
			int maxH = 25;
			if(piImage.Height > maxH) {
				piImage.Properties.AutoHeight = piSelected.Properties.AutoHeight = false;
				piImage.Height = piSelected.Height = maxH;
			}
			if(piState.Height > maxH) {
				piState.Properties.AutoHeight = false;
				piState.Height = maxH;
			}
		}
		DevExpress.XtraTreeList.TreeList treeListPreview;
		private void SetView() {
			treeListPreview = DevExpress.XtraTreeList.Design.TreeListAssign.CreateTreeListControlAssign(TreeList);
			treeListPreview.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeListPreview_FocusedNodeChanged);
			tlPreview.Controls.Add(treeListPreview);
		}
		private void SetExView() {
			foreach(DevExpress.XtraTreeList.Columns.TreeListColumn col in treeListPreview.Columns) {
				col.OptionsColumn.ReadOnly = false;
				col.OptionsColumn.AllowEdit = true;
			}
			treeListPreview.OptionsView.ShowColumns = true;
			treeListPreview.OptionsBehavior.Editable = true;
		}
		private void SetData() {
			if(TreeList == null) return;
			TreeList.ClearNodes();
			TreeList.BeginUnboundLoad();
			AddNodes(treeListPreview.Nodes, TreeList.Nodes, null); 
			TreeList.EndUnboundLoad();
			TreeList.FireChanged();
		}
		private void piImage_ValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if(FocusedNode != null && !init && e.NewValue is int)
				FocusedNode.ImageIndex = (int)e.NewValue;
		}
		private void piSelected_ValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if(FocusedNode != null  && !init && e.NewValue is int)
				FocusedNode.SelectImageIndex = (int)e.NewValue;
		}
		private void piState_ValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if(FocusedNode != null  && !init && e.NewValue is int)
				FocusedNode.StateImageIndex = (int)e.NewValue;
		}
		private void btnBestFit_Click(object sender, System.EventArgs e) {
			treeListPreview.BestFitColumns(true);
		}
		private void xNodesEditor_Load(object sender, System.EventArgs e) {
			treeListPreview.ForceInitialize();
			treeListPreview.BestFitColumns();
			int width = 0;
			foreach(DevExpress.XtraTreeList.Columns.TreeListColumn column in treeListPreview.Columns)
				if(column.VisibleIndex >=0) width += column.Width;
			treeListPreview.BestFitColumns(width < treeListPreview.Width);
		}
	}
	public class NodesEditor : UITypeEditor {
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(context != null && context.Instance != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				LookAndFeel.DesignService.ILookAndFeelService lookAndFeelService = provider.GetService(typeof(LookAndFeel.DesignService.ILookAndFeelService)) as LookAndFeel.DesignService.ILookAndFeelService;
				if(edSvc != null) {
					try {
						xNodesEditor editor = new xNodesEditor(objValue);
						if(editor != null && !RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin)
							lookAndFeelService.InitializeRootLookAndFeel(editor.LookAndFeel);
						else
							editor.LookAndFeel.SetSkinStyle("DevExpress Design");
						if(!editor.TreeList.IsUnboundMode) {
							MessageBox.Show("The DataSource property is not null...\r\nPlease set it to '(none)' before editing the Nodes collection.", "Warning");
						}
						else {
							if(edSvc.ShowDialog(editor) == DialogResult.OK) {
								TreeList tl = editor.TreeList;
								tl.FireChanged();
							}
						}
					}
					catch(Exception e) {
						MessageBox.Show(e.Message);
					}
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
