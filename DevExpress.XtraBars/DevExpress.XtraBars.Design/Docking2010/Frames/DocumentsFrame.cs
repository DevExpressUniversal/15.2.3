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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.Design.CodeGenerator;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class DocumentsFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public DocumentsFrame() {
			InitializeComponent();
			CreateImages();
		}
		void InitEditingView() {
			if(EditingView != null)
				((IDesignTimeSupport)EditingView).Load();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(EditingView != null)
					((IDesignTimeSupport)EditingView).Unload();
				DocumentManagerInfo.SelectionChanged -= OnInfo_SelectionChanged;
			}
			base.Dispose(disposing);
		}
		BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		EditingDocumentManagerInfo documentManagerInfo;
		protected EditingDocumentManagerInfo DocumentManagerInfo {
			get { return documentManagerInfo; }
		}
		public override void InitComponent() {
			base.InitComponent();
			this.documentManagerInfo = InfoObject as EditingDocumentManagerInfo;
			DocumentManagerInfo.SelectionChanged += OnInfo_SelectionChanged;
			btnPopulate.Enabled = NamesTable.Count != 0;
			InitEditingView();
		}
		void OnInfo_SelectionChanged(object sender, EventArgs e) {
			RaiseRefreshWizard("", "ChangedView");
			Populate();
			RefreshPropertyGrid();
		}
		void CreateImages() {
			Tree.ImageList = imageList1;
		}
		public DXTreeView Tree { get { return treeView1; } }
		public override void DoInitFrame() {
			Populate();
		}
		Hashtable expandedItems = new Hashtable();
		protected string GetNodeKey(TreeNode node) {
			int level = 0;
			TreeNode pnode = node;
			while(pnode.Parent != null) {
				level++;
				pnode = pnode.Parent;
			}
			return string.Format("{0}:{1}{2}", level, node.Tag == null ? null : node.Tag.GetType(), node.Tag == null ? 0 : node.Tag.GetHashCode());
		}
		protected virtual void ClearExpandedItems() {
			expandedItems.Clear();
		}
		protected virtual void SaveExpandedItems() {
			ClearExpandedItems();
			SaveExpandedNodes(Tree.Nodes);
		}
		void SaveExpandedNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				if(!node.IsExpanded) continue;
				this.expandedItems[GetNodeKey(node)] = true;
				SaveExpandedNodes(node.Nodes);
			}
		}
		public virtual void Populate() {
			Tree.BeginUpdate();
			try {
				Tree.Nodes.Clear();
				foreach(BaseDocument document in EditingView.Documents) {
					AddDocument(Tree.Nodes, document);
				}
			}
			finally {
				Tree.EndUpdate();
			}
		}
		public virtual void Repopulate() {
			SaveExpandedItems();
			Populate();
		}
		public virtual void RefreshTree() {
			Tree.BeginUpdate();
			try {
				RefreshNodes(Tree.Nodes);
			}
			finally {
				Tree.EndUpdate();
			}
		}
		protected void RefreshNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				node.Text = GetNodeCaption(node, false);
				RefreshNodes(node.Nodes);
			}
		}
		protected virtual string GetNodeCaption(TreeNode node, bool isEditing) {
			BaseDocument document = node.Tag as BaseDocument;
			return GetItemCaption(document, isEditing);
		}
		string GetItemCaption(BaseDocument item, bool isEditing) {
			string caption = !string.IsNullOrEmpty(item.Caption) ? item.Caption : "Document";
			if(item.IsDeferredControlLoad) {
				if(!string.IsNullOrEmpty(item.ControlName))
					return string.Format("{0}<{1}>", caption, item.ControlName);
				if(!string.IsNullOrEmpty(item.ControlTypeName))
					return string.Format("{0}<{1}>", caption, item.ControlTypeName);
			}
			return caption;
		}
		protected virtual void SetNodeCaption(TreeNode node, string label) {
			BaseDocument item = node.Tag as BaseDocument;
			if(item != null) item.Caption = label;
		}
		protected object GetNodeObject(TreeNode node) {
			if(node == null || node.Tag == null) return null;
			return node.Tag as BaseDocument;
		}
		protected virtual bool IsAllowEditObject(object val) {
			return true;
		}
		protected virtual bool IsAllowSelectObject(object val) {
			return true;
		}
		protected virtual void tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(e.Node == null) return;
			if(IsAllowSelectObject(GetNodeObject(e.Node))) {
				pgMain.SelectedObject = GetNodeObject(e.Node);
			}
			else
				pgMain.SelectedObject = null;
			SetDeleteEnabled(e.Node);
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			if(e.ChangedItem == null) return;
			if(e.ChangedItem.Label == "(Name)" || e.ChangedItem.Label == "Name" || e.ChangedItem.Label == "Text" || e.ChangedItem.Label == "Caption" || e.ChangedItem.Label == "UserCaption") {
				RefreshTree();
			}
		}
		protected virtual void tree_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			e.CancelEdit = !IsAllowEditObject(GetNodeObject(e.Node));
		}
		protected virtual void tree_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			if(e.Label != "" && e.Label != null) {
				try {
					SetNodeCaption(e.Node, e.Label);
				}
				catch(Exception ex) {
					e.CancelEdit = true;
					XtraMessageBox.Show(LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				pgMain.Refresh();
			}
			e.CancelEdit = true;
			e.Node.Text = GetNodeCaption(e.Node, false);
		}
		private void AddDocument() {
			string caption = GetDocumentCaption();
			if(caption != null) {
				BaseDocument document = null;
				try {
					string name = string.Concat(
						caption.Split(new char[] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries));
					name = name.ToLowerInvariant();
					document = CreateDocument(caption, name);
				}
				catch(Exception ex) {
					XtraMessageBox.Show(LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				UpdateNode(document);
			}
		}
		protected void UpdateNode(BaseDocument document) {
			Tree.SelectedNode = AddDocument(Tree.Nodes, document);
			Repopulate();
		}
		protected BaseDocument CreateDocument(string caption, string name) {
			return EditingView.AddDocument(caption, name);
		}
		protected virtual string GetDocumentCaption() {
			string caption = "document";
			List<int> listIndex = new List<int>();
			foreach(BaseDocument document in EditingView.Documents) {
				if(document.Caption.Contains(caption)) {
					try {
						int index = Int32.Parse(document.Caption.Replace(caption, string.Empty));
						listIndex.Add(index);
					}
					catch(Exception) { }
				}
			}
			for(int i = 1; i < Int32.MaxValue; i++) {
				if(!listIndex.Contains(i)) {
					caption += i;
					break;
				}
			}
			return caption;
		}
		protected virtual TreeNode AddDocument(TreeNodeCollection nodes, BaseDocument document) {
			TreeNode node = new TreeNode();
			node.Tag = document;
			node.Text = GetNodeCaption(node, false);
			node.SelectedImageIndex = node.ImageIndex = GetDocumentImageIndex(document);
			nodes.Add(node);
			return node;
		}
		int GetDocumentImageIndex(BaseDocument document) { return 0; }
		private void DeleteDocument() {
			if(Tree.SelectedNode == null) return;
			BaseDocument document = Tree.SelectedNode.Tag as BaseDocument;
			if(document != null) {
				if(MessageBox.Show(string.Format("Are you sure you want to delete '{0}' document?", document.Caption),
					"XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
					EditingView.Documents.Remove(document);
					Tree.Nodes.Remove(Tree.SelectedNode);
					document.Dispose();
					SetSelectedNode();
				}
			}
		}
		void SetSelectedNode() {
			SetDeleteEnabled(Tree.SelectedNode);
			pgMain.SelectedObject = null;
			if(Tree.SelectedNode != null && Tree.Nodes.Count != 0) {
				int i = Tree.SelectedNode.Index;
				Tree.SelectedNode = Tree.Nodes[i];
			}
			Repopulate();
		}
		void SetDeleteEnabled(TreeNode node) {
			btnDelete.Enabled = node != null && node.Tag is BaseDocument;
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			AddDocument();
		}
		void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteDocument();
		}
		void btnClear_Click(object sender, System.EventArgs e) {
			ClearDocuments();
		}
		protected virtual void tree_GetNodeEditText(object sender, TreeViewGetNodeEditTextEventArgs e) {
			e.Text = GetNodeCaption(e.Node, true);
		}
		protected virtual void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				DeleteDocument();
			}
		}
		protected virtual void tree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
		}
		protected virtual void tree_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
		}
		protected virtual void tree_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
		}
		protected virtual void tree_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
		}
		protected virtual void tree_DragLeave(object sender, System.EventArgs e) {
		}
		private void btnPopulate_Click(object sender, EventArgs e) {
			CreateDocuments();
		}
		protected void CreateDocuments() {		   
				IList<BaseDocument> populatedDocumentCollection = new List<BaseDocument>();				
				foreach(DictionaryEntry element in NamesTable) {
					EnvDTE.CodeClass codeClass = element.Value as EnvDTE.CodeClass;
					if(!CanAddDocument(codeClass)) continue;
					BaseDocument document = CreateDocument(codeClass.Name, codeClass.Name);
					document.ControlTypeName = codeClass.FullName;
					populatedDocumentCollection.Add(document);
				}				
				Repopulate();
				using(DTEServiceProvider context = new DTEServiceProvider((documentManagerInfo.EditingDocumentManager.ContainerControl ?? documentManagerInfo.EditingDocumentManager.MdiParent).Site)) {
					if(CanCreateQueryControlEvent(context))
						GenerateQueryControlEvent(context, populatedDocumentCollection);
				}
		}
		bool CanCreateQueryControlEvent(DTEServiceProvider context) {
			string eventName = documentManagerInfo.SelectedView.Site.Name + "_QueryControl";
			EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(context);
			return CodeNavigator.GetCodeClassMethod(rootClass, eventName) == null;
		}
		int GenerateQueryControlEvent(DTEServiceProvider context, IEnumerable<BaseDocument> populatedDocumentCollection) {
			string targetViewName = documentManagerInfo.SelectedView.Site.Name;
			string eventName = targetViewName + "_QueryControl";
			DTEEventInfo info = new DTEEventInfo(eventName, "QueryControl")
			{
				Code = CodeGenerator(populatedDocumentCollection),
				Comment = "Assigning a required content for each auto generated Document",
				EventSubscriptionComment = "// Handling the QueryControl event that will populate all automatically generated Documents" + Environment.NewLine
			};
			info.AddParameter("sender", typeof(object));
			info.AddParameter("e", typeof(QueryControlEventArgs));
			return CodeGeneratorHelper.AddEventHandler(context, targetViewName, info);
		}
		string CodeGeneratorIf(string documentName, string controlTypeName) {
			return "if(e.Document == " + documentName + ")" + Environment.NewLine + "e.Control = new " + controlTypeName + "();" + Environment.NewLine;
		}
		string CodeGenerator(IEnumerable<BaseDocument> populatedDocumentCollection) {
			string strCode = string.Empty;
			foreach(BaseDocument document in populatedDocumentCollection)
				strCode += CodeGeneratorIf(document.Site.Name, document.ControlTypeName);
			if(!string.IsNullOrEmpty(strCode))
				strCode += "if(e.Control == null)" + System.Environment.NewLine;
			strCode += "e.Control = new System.Windows.Forms.Control();";
			return strCode;
		}
		bool CanAddDocument(EnvDTE.CodeClass codeClass) {
			foreach(BaseDocument baseDocument in EditingView.Documents) {
				if(baseDocument.ControlTypeName == codeClass.FullName && baseDocument.ControlName == codeClass.Name) {
					return false;
				}
			}
			return true;
		}
		ProjectResearcher researcherCore;
		ProjectResearcher Researcher {
			get {
				if(researcherCore == null)
					researcherCore = new ProjectResearcher(VSServiceHelper.GetDTEProject(EditingView));
				return researcherCore;
			}
		}
		public void ClearDocuments() {
			if(MessageBox.Show("Are you sure you want to remove all documents?", "XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) return;
			BaseDocument[] documents = EditingView.Documents.ToArray();
			EditingView.Documents.RemoveRange(documents);
			for(int i = 0; i < documents.Length; i++) {
				documents[i].Dispose();
			}
			Repopulate();
		}
		public void PopulateDocuments() {
			if(EditingView.Manager == null) return;
			if(EditingView.Manager.MdiParent != null) {
				Researcher.Refresh(typeof(Form));
				PopulateDocuments(Researcher.Controls, EditingView.Manager.MdiParent.Name);
				Researcher.Refresh(typeof(UserControl));
				PopulateDocuments(Researcher.Controls, null);
			}
			if(EditingView.Manager.ContainerControl != null) {
				Researcher.Refresh(typeof(UserControl));
				PopulateDocuments(Researcher.Controls, EditingView.Manager.ContainerControl.Name);
			}
		}
		Hashtable namesTable;
		public Hashtable NamesTable {
			get {
				if(namesTable == null) {
					namesTable = new Hashtable();
					PopulateDocuments();
				}
				return namesTable; }
		}
		void PopulateDocuments(EnvDTE.CodeClass[] codeClasses, string ownerControlName) {
			foreach(EnvDTE.CodeClass codeClass in codeClasses) {
				if(codeClass.IsAbstract) continue;
				string controlName = codeClass.FullName;
				bool hideControl = false;
				foreach(EnvDTE.CodeAttribute codeAttribute in codeClass.Attributes) {
					if(codeAttribute.Name == DevExpress.XtraBars.Docking2010.SkipOnDocumentCreation.Name) hideControl = true;
				}
				if(!NamesTable.Contains(controlName) && codeClass.Name != ownerControlName && !hideControl) {
					NamesTable.Add(codeClass.FullName, codeClass);
				}
				if(NamesTable.Contains(controlName) && hideControl) {
					NamesTable.Remove(codeClass.FullName);
				}
			}
		}
	}
	class VSServiceHelper {
		public static EnvDTE.Project GetDTEProject(IComponent component) {
			EnvDTE.ProjectItem item = (EnvDTE.ProjectItem)component.Site.GetService(typeof(EnvDTE.ProjectItem));
			return item.ContainingProject;
		}
	}
	class ProjectResearcher : ProjectResearcherBase {
		List<EnvDTE.CodeClass> controlsCore;
		public ProjectResearcher(EnvDTE.Project project)
			: base(project) {
			this.controlsCore = new List<EnvDTE.CodeClass>();
		}
		public override void Refresh(object data) {
			this.controlsCore.Clear();
			base.Refresh(data);
		}
		public override void ProcessCodeElement(EnvDTE.CodeElement element, object data) {
			if(element.Kind != EnvDTE.vsCMElement.vsCMElementClass)
				return;
			Type dataType = (Type)data;
			EnvDTE.CodeClass elementClass = (EnvDTE.CodeClass)element;
			if(elementClass.get_IsDerivedFrom(dataType.FullName))
				controlsCore.Add(elementClass);
		}
		protected override void ProcessProjectItemCore(EnvDTE.CodeElements elements, object data) {
			EnvDTE.CodeElement nsCodeElement = FindNamespaceCodeElement(elements);
			if(nsCodeElement == null && elements.Count > 0) {
				ProcessCodeElement(elements.Item(1), data);
				return;
			}
			if(nsCodeElement != null && nsCodeElement.Children.Count > 0)
				foreach(EnvDTE.CodeElement element in nsCodeElement.Children) {
					ProcessCodeElement(element, data);
				}
		}
		protected virtual EnvDTE.CodeElement FindNamespaceCodeElement(EnvDTE.CodeElements elements) {
			foreach(EnvDTE.CodeElement element in elements) {
				if(element.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
					return element;
			}
			return null;
		}
		public EnvDTE.CodeClass[] Controls { get { return this.controlsCore.ToArray(); } }
		public override void Dispose() {
			if(this.controlsCore != null)
				this.controlsCore.Clear();
			this.controlsCore = null;
			base.Dispose();
		}
	}
}
