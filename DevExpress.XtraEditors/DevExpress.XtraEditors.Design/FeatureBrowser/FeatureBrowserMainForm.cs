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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraEditors.FeatureBrowser {
	[ToolboxItem(false)]
	public abstract class FeatureBrowserMainFrame : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		private FeaturesTreeView trFeatures;
		private System.ComponentModel.Container components = null;
		FeatureBrowserItem root;
		FeatureBrowserFormBase currentForm;
		private DevExpress.XtraEditors.SplitContainerControl pnlBrowserMain;
		ArrayList forms;
		string storedSelectedItemId;
		public FeatureBrowserMainFrame()	{
			InitializeComponent();
			this.trFeatures.KeyDown += new KeyEventHandler(onTreeViewFeaturesKeyDown);
			this.currentForm = null;
			this.forms = new ArrayList();
			this.storedSelectedItemId = string.Empty;
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			FeatureBrowserItem item = trFeatures.SelectedNode != null ? trFeatures.SelectedNode.Tag as FeatureBrowserItem : null;
			if(item != null) {
				localStore.AddProperty("SelectedItemId", item.Id);
			}
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			this.storedSelectedItemId = (string)localStore.RestoreProperty("SelectedItemId", string.Empty);
		}
		public override void DoInitFrame() {
			if(EditingObject == null) return;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(EditingComponent ?? EditingObject);
			FeatureBrowserBaseType fbAttribute = attributes[typeof(FeatureBrowserBaseType)] as FeatureBrowserBaseType;
			FeatureBrowserItemsXmlCreator creator = new FeatureBrowserItemsXmlCreator(XmlResourceFullNames, FeatureBrowserFormBase, fbAttribute == null ? EditingObject.GetType() : fbAttribute.BaseType);
			creator.LoadFromXml();
			this.root = creator.Root;
			CreateTreeItems();
			if(this.storedSelectedItemId != string.Empty)
				trFeatures.SelectedNode = GetTreeNodeByFeatureId(this.storedSelectedItemId);
			if(trFeatures.SelectedNode == null && trFeatures.Nodes.Count > 0)
				trFeatures.SelectedNode = trFeatures.Nodes[0];
		}
		public FeatureBrowserItem Root { get {  return root; } }
		public FeatureBrowserFormBase CurrentForm { get { return currentForm; } }
		public abstract string[] XmlResourceFullNames { get; }
		public abstract Type FeatureBrowserFormBase { get; }
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		void CreateTreeItems() {
			trFeatures.BeginUpdate();
			CreateTreeItems(trFeatures.Nodes, Root);
			trFeatures.EndUpdate();
		}
		void CreateTreeItems(TreeNodeCollection nodes, FeatureBrowserItem item) {
			for(int i = 0; i < item.Count; i ++) {
				TreeNode node = nodes.Add(item[i].Name);
				node.Tag = item[i].ReferenceItem != null ? item[i].ReferenceItem : item[i];
				CreateTreeItems(node.Nodes, item[i]);
			}
		}
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static public extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);
		Control ClientPanel { get { return this.pnlBrowserMain.Panel2; } }
		const int WM_SETREDRAW = 0x000B;
		void ShowModule(FeatureBrowserItem item) {
			if(item == null) return;
			if(item.FeatureSelectorFormType == null) {
				if(item.Count > 0)
					ShowModule(item[0]);
				return;
			}
			ClientPanel.SuspendLayout();
			SendMessage(ClientPanel.Handle, WM_SETREDRAW, 0, 0);
			try {
				CreateFeatureSelectorForm(item.FeatureSelectorFormType);
				CurrentForm.SetFeatureProperties(EditingObject, item);
			}
			finally {
				ClientPanel.ResumeLayout();
				SendMessage(ClientPanel.Handle, WM_SETREDRAW, 1, 0);
				ClientPanel.Refresh();
			}
		}
		void CreateFeatureSelectorForm(Type featureSelectorFormType) {
			if(CurrentForm != null) {
				if(CurrentForm.GetType() == featureSelectorFormType) {
					return;
				}
				else {
					CurrentForm.Visible = false;
				}
			}
			for(int i = 0; i < forms.Count; i ++) {
				if(forms[i].GetType() == featureSelectorFormType) {
					this.currentForm = forms[i] as FeatureBrowserFormBase;
					CurrentForm.Visible = true;
					break;
				}
			}
			ConstructorInfo constructorInfoObj = featureSelectorFormType.GetConstructor(Type.EmptyTypes);
			currentForm =  constructorInfoObj.Invoke(null) as FeatureBrowserFormBase;
			currentForm.Parent = ClientPanel;
			currentForm.Dock = DockStyle.Fill;
			currentForm.FeatureSelected += new FeatureBrowserFormFeatureSelectedEvent(OnFeatureSelected);
			forms.Add(currentForm);
		}
		void OnFeatureSelected(object sender, FeatureBrowserFormFeatureSelectedEventArgs e) {
			TreeNode node = GetTreeNodeByFeatureId(e.FeatureName);
			if(node != null) {
				e.Cancel = false; 
				trFeatures.SelectedNode = node;
			}
		}
		TreeNode GetTreeNodeByFeatureId(string id) {
			return FindNodeById(trFeatures.Nodes, id);
		}
		TreeNode FindNodeById(TreeNodeCollection nodes, string id) {
			for(int i = 0; i < nodes.Count; i ++) {
				TreeNode node = nodes[i];
				FeatureBrowserItem item = (FeatureBrowserItem)node.Tag;
				if(item.Id == id) return node;
				node = FindNodeById(node.Nodes, id);
				if(node != null) return node;
			}
			return null;
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.pnlBrowserMain = new DevExpress.XtraEditors.SplitContainerControl();
			this.trFeatures = new DevExpress.XtraEditors.FeatureBrowser.FeaturesTreeView();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBrowserMain)).BeginInit();
			this.pnlBrowserMain.SuspendLayout();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(592, 42);
			this.pnlMain.Controls.Add(this.pnlBrowserMain);
			this.pnlMain.Location = new System.Drawing.Point(0, 38);
			this.pnlMain.Size = new System.Drawing.Size(592, 434);
			this.horzSplitter.Size = new System.Drawing.Size(592, 9);
			this.pnlBrowserMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBrowserMain.Location = new System.Drawing.Point(0, 0);
			this.pnlBrowserMain.Name = "pnlBrowserMain";
			this.pnlBrowserMain.Panel1.Controls.Add(this.trFeatures);
			this.pnlBrowserMain.Size = new System.Drawing.Size(592, 434);
			this.pnlBrowserMain.SplitterPosition = 170;
			this.pnlBrowserMain.TabIndex = 4;
			this.pnlBrowserMain.Text = "panelControl1";
			this.trFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.trFeatures.DefaultExpandCollapseButtonOffset = 5;
			this.trFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trFeatures.HideSelection = false;
			this.trFeatures.HotTracking = true;
			this.trFeatures.Indent = 15;
			this.trFeatures.Location = new System.Drawing.Point(0, 0);
			this.trFeatures.Name = "trFeatures";
			this.trFeatures.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.trFeatures.ShowLines = false;
			this.trFeatures.Size = new System.Drawing.Size(170, 434);
			this.trFeatures.TabIndex = 0;
			this.trFeatures.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.trFeatures_BeforeSelect);
			this.trFeatures.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trFeatures_AfterSelect);
			this.Name = "FeatureBrowserMainFrame";
			this.Size = new System.Drawing.Size(592, 472);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBrowserMain)).EndInit();
			this.pnlBrowserMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private void trFeatures_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			FeatureBrowserItem item = (FeatureBrowserItem)e.Node.Tag;
			if(item.FeatureSelectorFormType == null) {
				e.Cancel = true;
				if(trFeatures.SelectedNode == e.Node.NextVisibleNode && e.Node.PrevVisibleNode != null) {
					trFeatures.SelectedNode = e.Node.PrevVisibleNode;
				} else {
					if(e.Node.Nodes.Count > 0)
						trFeatures.SelectedNode = e.Node.Nodes[0];
				}
			}
		}
		private void trFeatures_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			MakeNodeAndParentsVisible(e.Node);
			ShowModule((FeatureBrowserItem)e.Node.Tag);
		}
		void MakeNodeAndParentsVisible(TreeNode node) {
			while(node != null) {
				node.EnsureVisible();
				node = node.Parent;
			}
		}
		void onTreeViewFeaturesKeyDown(object sender, KeyEventArgs e) {
			if(e.Alt && e.KeyCode == Keys.I)
				new FeatureBrowserLinkChecker(Root, EditingObject).Run();
		}
		void CheckFeatureItemLinks() {
		}
	}
	[ToolboxItem(false)]
	public class FeaturesTreeView : DXTreeView {
		public FeaturesTreeView() {
		}
		protected override CreateParams CreateParams {
			get {
				const int TVS_NOHSCROLL = 0x8000;
				CreateParams cp = base.CreateParams;
				cp.Style |= TVS_NOHSCROLL;
				return cp;
			}
		}
		protected override bool UseThemes { get { return true; } }
	}
}
