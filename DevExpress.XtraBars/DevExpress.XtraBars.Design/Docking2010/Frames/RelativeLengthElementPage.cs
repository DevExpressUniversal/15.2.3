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
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraBars.Design.Frames {
	public class RelativeLengthElementPage<T, TInsnance> : BaseRelativeLengthElementPage
		where T : BaseComponent {
		DXPropertyGridEx propertyGrid1;
		SplitterControl splitterControl1;
		Panel pnlButton;
		Panel pnlList;
		SimpleButton btnRemove;
		SimpleButton btnAdd;
		BaseMutableListEx<T> collectionCore;
		IContainer components = null;
		DXTreeView treeView1;
		string[] propertiesCore;
		public RelativeLengthElementPage(BaseMutableListEx<T> collection, string[] properties) {
			InitializeComponent();
			collectionCore = collection;
			btnAdd.Image = XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[0];
			btnRemove.Image = XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[2];
			PopulateTreeView();
			if(Properties != null)
				propertyGrid1.SelectedObject = new FilterObject(treeView1.SelectedNode.Tag, Properties);
			propertiesCore = properties;
		}
		void PopulateTreeView() {
			treeView1.SuspendLayout();
			treeView1.Nodes.Clear();
			foreach(var item in collectionCore) {
				var node = new TreeNode(GetNodeText(item)) { Tag = item };
				node.ImageIndex =  item is Document ? 0 : 1;
				treeView1.Nodes.Add(node);
				if(string.Equals(node.Text, selectedNodeText))
					treeView1.SelectedNode = node;
			}
			treeView1.ResumeLayout();
		}
		public override void SetCollectionModify(bool value) {
			pnlButton.Visible = value;
		}
		string GetNodeText(T item) {
			string result = string.Empty;
			if(item is BaseDocument)
				result += (item as BaseDocument).Caption;
			if(item is BaseRelativeLengthElement) {
				if(item is RowDefinition)
					result += string.Format("rowDefinition{0}",collectionCore.IndexOf(item).ToString());
				if(item is ColumnDefinition)
					result +=  string.Format("columnDefinition{0}", collectionCore.IndexOf(item).ToString());
				if(item is StackGroup)
					result +=  string.Format("stackGroup{0}", collectionCore.IndexOf(item).ToString());
				var length = (item as BaseRelativeLengthElement).Length;
				if(length.UnitType == LengthUnitType.Pixel)
					result += string.Format(" ({0}px)", length.UnitValue);
				else
					result += string.Format(" ({0}*)", length.UnitValue);
			}
			return result;
		}
		public string[] Properties {
			get { return propertiesCore; }
		}
		public override void UpdateImages(ImageList images) {
			treeView1.ImageList = images;
		}
		public override void UpdateSelection(object item) {
			for(int i = 0; i < treeView1.Nodes.Count; i++) {
				if(treeView1.Nodes[i].Tag == item) {
					treeView1.SelectedNode = treeView1.Nodes[i];
					if(Properties != null) {
						var element = new FilterObject(treeView1.SelectedNode.Tag, Properties);
						propertyGrid1.SelectedObject = element;
					}
					return;
				}
			}
		}
		string selectedNodeText;
		public override void UpdateTreeView() {
			if(treeView1.SelectedNode != null)
				selectedNodeText = treeView1.SelectedNode.Text;
			PopulateTreeView();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.propertyGrid1 = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
			this.pnlButton = new System.Windows.Forms.Panel();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.pnlList = new System.Windows.Forms.Panel();
			this.treeView1 = new DevExpress.Utils.Design.DXTreeView();
			this.pnlButton.SuspendLayout();
			this.pnlList.SuspendLayout();
			this.SuspendLayout();
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.DrawFlat = false;
			this.propertyGrid1.Location = new System.Drawing.Point(205, 55);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(328, 247);
			this.propertyGrid1.TabIndex = 0;
			this.splitterControl1.Location = new System.Drawing.Point(200, 55);
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.Size = new System.Drawing.Size(5, 247);
			this.splitterControl1.TabIndex = 1;
			this.splitterControl1.TabStop = false;
			this.pnlButton.Controls.Add(this.btnRemove);
			this.pnlButton.Controls.Add(this.btnAdd);
			this.pnlButton.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlButton.Location = new System.Drawing.Point(0, 0);
			this.pnlButton.Name = "pnlButton";
			this.pnlButton.Size = new System.Drawing.Size(533, 55);
			this.pnlButton.TabIndex = 3;
			this.btnRemove.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemove.Location = new System.Drawing.Point(36, 5);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(30, 30);
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			this.btnAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAdd.Location = new System.Drawing.Point(0, 5);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(30, 30);
			this.btnAdd.TabIndex = 3;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.pnlList.Controls.Add(this.treeView1);
			this.pnlList.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlList.Location = new System.Drawing.Point(0, 55);
			this.pnlList.Margin = new System.Windows.Forms.Padding(5);
			this.pnlList.Name = "pnlList";
			this.pnlList.Size = new System.Drawing.Size(200, 247);
			this.pnlList.TabIndex = 5;
			this.treeView1.AllowSkinning = true;
			this.treeView1.DefaultExpandCollapseButtonOffset = 0;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.treeView1.Size = new System.Drawing.Size(200, 247);
			this.treeView1.TabIndex = 2;
			this.treeView1.SelectionChanged += treeView1_SelectionChanged;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.splitterControl1);
			this.Controls.Add(this.pnlList);
			this.Controls.Add(this.pnlButton);
			this.Name = "RelativeLengthElementPage";
			this.Size = new System.Drawing.Size(533, 302);
			this.pnlButton.ResumeLayout(false);
			this.pnlList.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		void treeView1_SelectionChanged(object sender, EventArgs e) {
			if(treeView1.SelCount == 0) return;
			TreeNode[] selNodes = treeView1.SelNodes;
			object[] objs = new object[selNodes.Length];
			for(int n = 0; n < selNodes.Length; n++) {
				var element = selNodes[n].Tag;
				if(Properties != null)
					element = new FilterObject(treeView1.SelectedNode.Tag, Properties);
				object obj = element;
				objs[n] = element;
			}
			propertyGrid1.SelectedObjects = objs;
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			var newInstance = (T)Activator.CreateInstance(typeof(TInsnance));
			collectionCore.Add(newInstance);
			PopulateTreeView();
		}
		void btnRemove_Click(object sender, System.EventArgs e) {
			TreeNode[] selNodes = treeView1.SelNodes;
			foreach(var node in selNodes) {
				var item = (T)node.Tag;
				int index = collectionCore.IndexOf(item);
				collectionCore.Remove(item);
			}
			PopulateTreeView();
		}
	}
	public class BaseRelativeLengthElementPage : UserControl {
		public virtual void SetCollectionModify(bool value) { }
		public virtual void UpdateTreeView() { }
		public virtual void UpdateSelection(object item) { }
		public virtual void UpdateImages(ImageList images) { }
	}
}
