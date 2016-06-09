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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Native;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Data.Browsing.Design {
	#region TreeNodes
	public class DataMemberNodeBase : TreeNode, INode {
		public virtual string DataMember { get { return null; } }
		public virtual object DataSource { get { return null; } }
		public bool IsEmpty { get { return DataSource == null && DataMember == null; } }
		public DataMemberNodeBase() {
		}
		public DataMemberNodeBase(string text, int imageIndex, int selectedImageIndex)
			: base(text, imageIndex, selectedImageIndex) {
		}
		#region INode Members
		public virtual bool IsDummyNode { get { return false; } }
		public virtual bool IsDataMemberNode { get { return false; } }
		public virtual bool IsDataSourceNode { get { return false; } }
		public virtual bool IsList { get { return false; } }
		public virtual bool IsComplex { get { return false; } }
		public IList ChildNodes { get { return this.Nodes; } }
		public new object Parent { get { return base.Parent; } }
		public bool HasDataSource(object dataSource) {
			return this.DataSource != null && this.DataSource.Equals(dataSource);
		}
		public void AddChildNode(object node) {
			this.Nodes.Add((TreeNode)node);
		}
		public void Expand(EventHandler callback) {
			base.Expand();
			if(callback != null)
				callback(this, EventArgs.Empty);
		}
		#endregion
	}
	public class DataMemberNode : DataMemberNodeBase, IDataInfoContainer {
		private object dataSource = null;
		private string dataMember = "";
		PickManager pickManager;
		public override bool IsDataMemberNode { get { return true; } }
		public override string DataMember { get { return dataMember; } }
		public override object DataSource {  get { return dataSource; } }
		public override bool IsList { get { return pickManager != null; } }
		public DataMemberNode(object dataSource, string dataMember, string text, PickManager pickManager)
			: base(text, -1, -1) {
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			ImageIndex = SelectedImageIndex = GetImageIndex();
			this.pickManager = pickManager;
		}
		private int GetImageIndex() {
			return IsList ? DataSourceTreeView.TABLE_IMAGE : DataSourceTreeView.COLUMN_IMAGE;
		}
		DataInfo[] IDataInfoContainer.GetData() {
			return pickManager != null ? pickManager.GetData(DataSource, DataMember) : 
				new DataInfo[] { new DataInfo(DataSource, DataMember, Text) };
		}
	}
	class DataSourceNode : DataMemberNodeBase, IDataInfoContainer {
		PickManager pickManager;
		private object dataSource;
		public override object DataSource { get { return dataSource; } }
		public override bool IsDataSourceNode { get { return true; } }
		public DataSourceNode(object dataSource, string name, PickManager pickManager)
			: base(name, -1, -1) {
			this.dataSource = dataSource;
			this.ImageIndex = SelectedImageIndex = GetImageIndex();
			this.pickManager = pickManager;
		}
		private int GetImageIndex() {
			return (dataSource is DataSet) ? DataSourceTreeView.DATASET_IMAGE : DataSourceTreeView.TABLE_IMAGE;
		}
		DataInfo[] IDataInfoContainer.GetData() {
			return pickManager.GetData(dataSource, "");
		}
	}
	class DummyNode : DataMemberNodeBase {
		public override bool IsDummyNode { get { return true; } }
		public DummyNode()
			: base() {
		}
	}
	#endregion
	public class TreeViewPickManager : PickManager {
		public TreeViewPickManager() {
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			return new DataSourceNode(dataSource, name, this);
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			return new DataMemberNode(dataSource, dataMember, displayName, isList ? this : null);
		}
		protected override INode CreateDummyNode(object owner) {
			return new DummyNode();
		}
		protected override object CreateNoneNode(object owner) {
			return new DataMemberNodeBase(PreviewLocalizer.GetString(PreviewStringId.NoneString) , DataSourceTreeView.NONE_IMAGE, DataSourceTreeView.NONE_IMAGE);
		}
		protected override bool NodeIsEmpty(INode node) {
			return node is DataMemberNodeBase && node.IsEmpty;
		}
	}
	[ToolboxItem(false)]
	public class DataSourceTreeView : TreeView {
		public const int COLUMN_IMAGE = 1, NONE_IMAGE = 2, DATASET_IMAGE = 3, TABLE_IMAGE = 0;
		[DllImport("user32.dll")] 
		public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
		PickManager pickManager;
		protected virtual string DataMemberNoneString { get { return PreviewLocalizer.GetString(PreviewStringId.NoneString); } }
		public PickManager PickManager { get { return pickManager; }
		}
		public DataMemberNodeBase DataMemberNode { 
			get { return SelectedNode as DataMemberNodeBase; }
		}
		public DataSourceTreeView(PickManager pickManager) {
			if (pickManager == null)
				throw new ArgumentException("pickManager");
			this.pickManager = pickManager;
			Initialize();
		}		
		public DataSourceTreeView() {
			this.pickManager = CreatePickManager();
			Initialize();
		}
		protected virtual PickManager CreatePickManager() {
			return new TreeViewPickManager();
		}
		protected void Initialize() {
			ImageList = new ImageList();
			PickManager.FillDataSourceImageList(ImageList);
		}
		public void Start() {
			BeforeExpand += new TreeViewCancelEventHandler(OnNodeExpand);
		}
		public void Stop() {
			BeforeExpand -= new TreeViewCancelEventHandler(OnNodeExpand);
		}
		public virtual void SelectDataMemberNode(object dataSource, string dataMember) {
			DataMemberNode node = (DataMemberNode)pickManager.FindDataMemberNode(Nodes, dataSource, dataMember);
			if(node != null)
				SelectedNode = node;
		}
		public TreeNode NoneNode {
			get {
				foreach(TreeNode node in Nodes) {
				if (node.Text == DataMemberNoneString) {
						return node;
					}
				}
				return null;
			}
				}
		public void SelectNoneNode() {
			if(NoneNode != null) SelectedNode = NoneNode;
		}
		[System.Security.SecuritySafeCritical]
		protected override void InitLayout() { 
			base.InitLayout(); 
			const int SB_HORZ = 0; 
			ShowScrollBar(Handle, SB_HORZ, false); 
		}
		protected virtual void OnNodeExpand(object sender, TreeViewCancelEventArgs e) {
			DataSourceNode node = (DataSourceNode)pickManager.GetDataSourceNode((INode)e.Node);
			if (node != null) { 
				SelectedNode = e.Node;
				pickManager.OnNodeExpand(node.DataSource, (INode)e.Node); 
			}
		}
	}
	#region ColumnNameUITypeEditor
	public abstract class ColumnNameEditor : System.Drawing.Design.UITypeEditor {
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider == null)
				return value;
			IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(editorService == null)
				return value;
			object dataSource = GetDataSource(context);
			if(dataSource == null)
				return dataSource;
			using(ColumnNameEditorPicker picker = CreatePicker(dataSource, value as string, provider)) {
				editorService.DropDownControl(picker);
				string res = picker.SelectedColumnName;
				if(string.IsNullOrEmpty(res) && !picker.IsNoneNodeSelected) return value;
				return res;
			}
		}
		public override bool IsDropDownResizable { get { return true; } }
		protected virtual ColumnNameEditorPicker CreatePicker(object dataSource, string columnName, IServiceProvider serviceProvider) {
			return new ColumnNameEditorPicker(dataSource, columnName, serviceProvider);
		}
		protected abstract object GetDataSource(ITypeDescriptorContext context);
	}
	[ToolboxItem(false)]
	public class ColumnNameEditorPicker : DataSourceTreeView {
		readonly object dataSource;
		readonly IServiceProvider serviceProvider;
		IWindowsFormsEditorService editorService;
		bool selectNodeOnHandleCreated;
		object selectNodeDataSource;
		string selectNodeDataMember;
		public ColumnNameEditorPicker(object dataSource, string columnName, IServiceProvider serviceProvider) {
			this.dataSource = dataSource;
			this.serviceProvider = serviceProvider;
			this.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(OnNodeMouseDoubleClick);
			Start();
			FillContent(dataSource);
			SelectedColumnName = columnName;
		}
		protected virtual void FillContent(object dataSource) {
			PickManager.FillContent(Nodes, dataSource, true);
		}
		protected virtual void SetSize() {
			Height = Math.Min(400, ItemHeight * ExpandedNodesCount(Nodes) * 4 / 3);
			Width = Math.Min(400, GetMaxNodeRight(Nodes) + SystemInformation.VerticalScrollBarWidth + 10);
		}
		protected int GetMaxNodeRight(TreeNodeCollection nodes) {
			int maxNodeRight = 0;
			for(int i = 0; i < nodes.Count; i++) {
				if(nodes[i].Bounds.Right > maxNodeRight)
					maxNodeRight = nodes[i].Bounds.Right;
				if(nodes[i].Nodes.Count > 0) {
					int childMaxRight = GetMaxNodeRight(nodes[i].Nodes);
					if(childMaxRight > maxNodeRight)
						maxNodeRight = childMaxRight;
				}
			}
			return maxNodeRight;
		}
		protected int ExpandedNodesCount(TreeNodeCollection nodes) {
			int count = nodes.Count;
			for(int i = 0; i < nodes.Count; i++) {
				if(nodes[i].IsExpanded)
					count += ExpandedNodesCount(nodes[i].Nodes);
			}
			return count;
		}
		protected new ColumnNameEditorPickManager PickManager { get { return (ColumnNameEditorPickManager)base.PickManager; } }
		protected object DataSource { get { return this.dataSource; } }
		protected IServiceProvider ServiceProvider { get { return this.serviceProvider; } }
		protected IWindowsFormsEditorService EditorService {
			get {
				if(this.editorService == null && ServiceProvider != null)
					this.editorService = (IWindowsFormsEditorService)ServiceProvider.GetService(typeof(IWindowsFormsEditorService));
				return this.editorService;
			}
		}
		public bool IsNoneNodeSelected { get { return SelectedNode == NoneNode && SelectedNode != null; } }
		public string SelectedColumnName {
			get {
				DevExpress.Data.Browsing.Design.DataMemberNode node = SelectedNode as DevExpress.Data.Browsing.Design.DataMemberNode;
				return node == null ? String.Empty : node.DataMember;
			}
			set {
				if(!string.IsNullOrEmpty(value))
					SelectDataMemberNode(DataSource, value);
				else
					SelectNoneNode();
			}
		}
		protected virtual void OnNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
			if(e.Node.Nodes.Count == 0)
				CloseDropDown();
		}
		protected virtual void CloseDropDown() {
			Stop();
			if(EditorService != null)
				EditorService.CloseDropDown();
		}
		protected override DevExpress.Data.Browsing.Design.PickManager CreatePickManager() {
			return new ColumnNameEditorPickManager();
		}
		public override void SelectDataMemberNode(object dataSource, string dataMember) {
			if(!IsHandleCreated) {
				this.selectNodeOnHandleCreated = true;
				this.selectNodeDataSource = dataSource;
				this.selectNodeDataMember = dataMember;
				return;
			}
			base.SelectDataMemberNode(dataSource, dataMember);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(this.selectNodeOnHandleCreated) {
				SelectDataMemberNode(this.selectNodeDataSource, this.selectNodeDataMember);
				this.selectNodeOnHandleCreated = false;
			}
			SetSize();
		}
	}
	public class ColumnNameEditorPickManager : TreeViewPickManager {
		protected class ColumnNameProvider : PropertiesProvider {
			public override void GetListItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> action) {
				Type memberType = DataContext.GetPropertyType(dataSource, dataMember);
				if(memberType != null && (memberType.IsPrimitive || memberType == typeof(string) || memberType == typeof(DateTime)))
					action(this, CreatePropertiesEventArgs(new IPropertyDescriptor[0]));
				else
					GetItemProperties(dataSource, dataMember, action);
			}
		}
		protected override IPropertiesProvider CreateProvider() {
			return new ColumnNameProvider();
		}
		public void FillContent(IList nodes, object dataSource, bool addNoneNode) {
			nodes.Clear();
			FillNodes(dataSource, string.Empty, nodes);
			if(addNoneNode)
				nodes.Add(CreateNoneNode(nodes));
			if(nodes.Count > 0)
				((INode)nodes[0]).Expand(null);
		}
	}
	#endregion
}
