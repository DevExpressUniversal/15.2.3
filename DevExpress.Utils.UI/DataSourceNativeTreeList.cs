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
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
using System.ComponentModel;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Services.Internal;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList;
using System.Windows.Forms;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.Utils.UI.Localization;
using System.Drawing;
using System.Security;
namespace DevExpress.XtraReports.Native
{
	[ToolboxItem(false)]
	public class DataSourceNativeTreeList : XtraTreeView {
		#region inner classes
		protected enum NodeType {
			DataSourceNode,
			DataMemberNode,
			FieldNode,
			CalculatedFieldNode,
			ParameterTableNode,
			ParameterNode,
		}
		protected class ContextMenuHelper {
			#region static
			protected static Image CreateImageFromResource(string resource) {
				return ResLoaderBase.LoadBitmap("TlbrImages." + resource, typeof(DevExpress.Utils.UI.ResFinder), Color.Magenta);
			}
			public static ContextMenuHelper CreateInstance(DataMemberListNodeBase node) {
				NodeType typeNode = IdentifyNode(node);
				NodeType typeNodeParent = IdentifyNodeParent(node);
				if (typeNode == NodeType.DataMemberNode)
					return new DataMemberContextMenuHelper(node);
				if (typeNode == NodeType.CalculatedFieldNode)
					return new CalculatedFieldMenuHelper(node);
				if (typeNode == NodeType.ParameterNode)
					return new ParameterPropertyMenuHelper(node);
				if (typeNode == NodeType.ParameterTableNode)
					return new ParameterTableMenuHelper(node);
				if (typeNode == NodeType.FieldNode)
					if (typeNodeParent == NodeType.FieldNode)
						return new NestedPropertyMenuHelper(node);
					else
						return new FieldMenuHelper(node);
				return new ContextMenuHelper(node);
			}
			public static NodeType IdentifyNode(DataMemberListNodeBase node) {
				if (node.IsDataSourceNode) {
					if (node.DataSource is ParametersDataSource)
						return NodeType.ParameterTableNode;
					if (node.DataSource is IList)
						return NodeType.DataMemberNode;
					if (node.DataSource is IListSource)
						return NodeType.DataMemberNode;
					return NodeType.DataSourceNode;
				}
				if (node.IsDataMemberNode && ((DataMemberListNode)node).IsList)
					return NodeType.DataMemberNode;
				if (node.Property is CalculatedPropertyDescriptorBase)
					return NodeType.CalculatedFieldNode;
				if (node.Property is ParameterPropertyDescriptor)
					return NodeType.ParameterNode;
				return NodeType.FieldNode;
			}
			internal static NodeType IdentifyNodeParent(DataMemberListNodeBase node) {
				DataMemberListNodeBase parentNode = node.Parent as DataMemberListNodeBase;
				return parentNode != null ? IdentifyNode(parentNode) : NodeType.FieldNode;
			}
			#endregion
			DataMemberListNodeBase node;
			protected DataMemberListNodeBase Node { get { return node; } }
			public ContextMenuHelper(DataMemberListNodeBase node) {
				this.node = node;
			}
			public virtual MenuItemDescriptionCollection CreateMenuItems() {
				return null;
			}
		}
		protected class DataMemberContextMenuHelper : ContextMenuHelper {
			public DataMemberContextMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_AddCalculatedField), null, FieldListCommands.AddCalculatedField));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditCalculatedFields), null, FieldListCommands.EditCalculatedFields));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_ClearCalculatedFields), null, FieldListCommands.ClearCalculatedFields));
				return items;
			}
		}
		protected class FieldMenuHelper : DataMemberContextMenuHelper {
			public FieldMenuHelper(DataMemberListNodeBase node)
				: base(node.ParentNode as DataMemberListNodeBase) {
			}
		}
		class NestedPropertyMenuHelper : ContextMenuHelper {
			public NestedPropertyMenuHelper(DataMemberListNodeBase node)
				: base(node.ParentNode as DataMemberListNodeBase) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditCalculatedFields), null, FieldListCommands.EditCalculatedFields));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_ClearCalculatedFields), null, FieldListCommands.ClearCalculatedFields));
				return items;
			}
		}
		class ParameterTableMenuHelper : ContextMenuHelper {
			public ParameterTableMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_AddParameter), null, FieldListCommands.AddParameter));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditParameters), null, FieldListCommands.EditParameters));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_ClearParameters), null, FieldListCommands.ClearParameters));
				return items;
			}
		}
		class ParameterPropertyMenuHelper : ContextMenuHelper {
			public ParameterPropertyMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				if(Node.ParentNode as DataMemberListNodeBase != null) {
					items.Add(new MenuItemDescription(Node.ParentNode, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_AddParameter), null, FieldListCommands.AddParameter));
				}
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditParameters), null, FieldListCommands.EditParameters));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_DeleteParameter), CreateImageFromResource("Delete.png"), FieldListCommands.DeleteParameter));
				return items;
			}
		}
		class CalculatedFieldMenuHelper : ContextMenuHelper {
			public CalculatedFieldMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				if(Node.ParentNode as DataMemberListNodeBase != null) {
					items.Add(new MenuItemDescription(Node.ParentNode, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_AddCalculatedField), null, FieldListCommands.AddCalculatedField));
					items.Add(new MenuItemDescription(Node.ParentNode, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditCalculatedFields), null, FieldListCommands.EditCalculatedFields));
				}
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditExpression), null, FieldListCommands.EditExpressionCalculatedField));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_DeleteCalculatedField), CreateImageFromResource("Delete.png"), FieldListCommands.DeleteCalculatedField));
				return items;
			}
		}
		#endregion
		[System.Security.SecuritySafeCritical]
		public static bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow) { return ShowScrollBar_(hWnd, wBar, bShow); }
		[DllImport("user32.dll", EntryPoint = "ShowScrollBar")]
		static extern bool ShowScrollBar_(IntPtr hWnd, int wBar, bool bShow);
		TreeListPickManager pickManager;
		protected IServiceProvider serviceProvider;
		XtraContextMenuBase fContextMenu;
		bool showParametersNode = true;
		protected bool ContextMenuVisible { get { return fContextMenu != null && fContextMenu.Visible; } }
		public TreeListPickManager PickManager {
			get { return pickManager; }
		}
		public DataMemberListNodeBase DataMemberNode {
			get { return this.Selection[0] as DataMemberListNodeBase; }
		}
		public bool ShowParametersNode {
			get { return showParametersNode; }
			set {
				showParametersNode = value;
				UpdateParametersVisibility();
			}
		}
		public DataSourceNativeTreeList(TreeListPickManager pickManager) {
			if (pickManager == null)
				throw new ArgumentException("pickManager");
			this.pickManager = pickManager;
			this.StateImageList = ColumnImageProvider.Instance.CreateImageCollection();
			this.OptionsBehavior.AllowIncrementalSearch = true;
			this.ItemDrag += new ItemDragEventHandler(OnItemDrag);
		}
		public DataSourceNativeTreeList()
			: this(new TreeListPickManager()) {
		}
		public void ShowContextMenu(DataMemberListNodeBase node, Point point) {
			if (CanShowContextMenu()) {
				if (fContextMenu != null)
					fContextMenu.Dispose();
				IXRMenuCommandService menuComandService = serviceProvider.GetService(typeof(IXRMenuCommandService)) as IXRMenuCommandService;
				if(menuComandService == null)
					return;
				fContextMenu = CreateContextMenu();
				if(fContextMenu == null)
					return;
				IList<MenuItemDescription> items = CreateMenuItems(node);
				if(items == null || items.Count == 0)
					return;
				fContextMenu.AddMenuItems(items, null, null);
				Point screanPoint = this.PointToScreen(point);
				menuComandService.ShowContextMenu(fContextMenu, screanPoint.X, screanPoint.Y);
			}
		}
		protected virtual MenuItemDescriptionCollection CreateMenuItems(DataMemberListNodeBase node) {
			ContextMenuHelper helper = ContextMenuHelper.CreateInstance(node);
			return helper.CreateMenuItems();
		}
		protected virtual XtraContextMenuBase CreateContextMenu() {
			return null;
		}
		public void UpdateDataSource(IServiceProvider serviceProvider, object[] dataSources) {
			Stop();
			BeginUpdate();
			if (serviceProvider != null) {
				this.serviceProvider = serviceProvider;
				this.PickManager.ServiceProvider = serviceProvider;
				this.PickManager.FillContent(Nodes, dataSources, dataSources.Length == 0);
			}
			else
				Nodes.Clear();
			EndUpdate();
			Start();
			UpdateParametersVisibility();
		}
		void UpdateParametersVisibility() {
			if (ParametersNode != null)
				ParametersNode.Visible = showParametersNode;
		}
		internal XtraListNode ParametersNode {
			get {
				foreach (DataMemberListNodeBase node in Nodes)
					if (ContextMenuHelper.IdentifyNode(node) == NodeType.ParameterTableNode)
						return node;
				return null;
			}
		}
		bool CanShowContextMenu() {
			if (Parent != null && Parent.Parent != null)
				return Parent.Parent.ContextMenuStrip == null && Parent.Parent.ContextMenu == null;
			return true;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.ItemDrag -= new ItemDragEventHandler(OnItemDrag);
				if (fContextMenu != null) {
					fContextMenu.Dispose();
					fContextMenu = null;
				}
			}
			base.Dispose(disposing);
		}
		public void Start() {
			BeforeExpand += new BeforeExpandEventHandler(OnNodeExpand);
			BeforeCollapse += new BeforeCollapseEventHandler(OnNodeCollapse);
		}
		public void Stop() {
			BeforeExpand -= new BeforeExpandEventHandler(OnNodeExpand);
			BeforeCollapse -= new BeforeCollapseEventHandler(OnNodeCollapse);
		}
		public void SelectDataMemberNode(object dataSource, string dataMember) {
			DataMemberListNode node = (DataMemberListNode)pickManager.FindDataMemberNode((IList)this.Nodes, dataSource, dataMember);
			if (node != null) {
				this.SelectNode(node);
			}
		}
		public void SelectNoneNode() {
			foreach (XtraListNode node in Nodes)
				if (node.GetDisplayText(0) == NoneNodeText) {
					this.SelectNode(node);
					return;
				}
		}
		protected virtual string NoneNodeText {
			get { return "(None)"; }
		}
		protected virtual void OnItemDrag(object sender, ItemDragEventArgs e) {
		}
		[SecuritySafeCritical]
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			const int SB_HORZ = 0;
			ShowScrollBar(Handle, SB_HORZ, false);
		}
		protected virtual void OnNodeExpand(object sender, BeforeExpandEventArgs e) {
			DataSourceListNode node = (DataSourceListNode)pickManager.GetDataSourceNode((INode)e.Node);
			if (node != null) {
				this.SelectNode((XtraListNode)e.Node);
				pickManager.OnNodeExpand(node.DataSource, (INode)e.Node);
			}
		}
		protected virtual void OnNodeCollapse(object sender, BeforeCollapseEventArgs e) {
		}
	}
}
