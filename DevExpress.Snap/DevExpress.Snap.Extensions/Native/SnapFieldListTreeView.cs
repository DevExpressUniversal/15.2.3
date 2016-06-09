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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Design;
using DevExpress.DataAccess;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.UI.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Painter;
using DevExpress.DataAccess.Excel;
namespace DevExpress.Snap.Extensions.Native {
	[DXToolboxItem(false)]
	public class SnapFieldListTreeView : DataSourceNativeTreeList, IToolTipControlClient, IDesignControl {
		readonly TreeListHelper treeListHelper;
		ImageCollection mailMergeStateImageList;
		public SnapFieldListTreeView() : this(new SnapTreeListPickManager(new DataContextOptions(false, true))) { }
		protected SnapFieldListTreeView(TreeListPickManager pickManager)
			: base(pickManager) {
			treeListHelper = new TreeListHelper(this);
			OptionsSelection.MultiSelect = true;
			OptionsBehavior.KeepSelectedOnClick = false;
			this.mailMergeStateImageList = ImageHelper.CreateImageCollectionFromResources("DevExpress.Snap.Extensions.Images.DataPickerImages_2.png", typeof(ResFinder).Assembly, new Size(16, 16));
		}
		protected internal ImageCollection MailMergeStateImageList { get { return mailMergeStateImageList; } }
		public SnapControl SnapControl {
			get { return treeListHelper.SnapControl; }
			set {
				if (!DesignMode) {
					UnsubscribeDocumentModelEvents();
					treeListHelper.SnapControl = value;
					serviceProvider = value;
					SubscribeDocumentModelEvents();
				}
			}
		}
		protected override TreeListHandler CreateHandler() {
			return new SnapTreeListHandler(this);
		}
		protected class SnapDataMemberContextMenuHelper : ContextMenuHelper {
			public SnapDataMemberContextMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription());
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_AddCalculatedField), null, FieldListCommands.AddCalculatedField));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_EditCalculatedFields), null, FieldListCommands.EditCalculatedFields));
				items.Add(new MenuItemDescription(Node, UtilsUILocalizer.GetString(UtilsUIStringId.Cmd_ClearCalculatedFields), null, FieldListCommands.ClearCalculatedFields));
				return items;
			}
		}
		protected class SnapDataSourceContextMenuHelper : SnapDataMemberContextMenuHelper {
			readonly SnapControl control;
			public SnapDataSourceContextMenuHelper(SnapControl control, DataMemberListNodeBase node, bool isMailMergeDataSourceChecked)
				: base(node) {
				this.control = control;
				IsMailMergeDataSourceChecked = isMailMergeDataSourceChecked;
			}
			protected bool IsMailMergeDataSourceChecked { get; private set; }
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection result = base.CreateMenuItems();
				DataSourceInfo info = this.control.DocumentModel.DataSources.GetInfo(Node.DataSource);
				if (info != null) {
					IDataComponent dataComponent = info.DataSource as IDataComponent;
					if (dataComponent != null)
						result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.UpdateDataSource), null, DataExplorerCommands.UpdateDataSource));
					result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.RemoveDataSource), null, DataExplorerCommands.RemoveDataSource));
					if (dataComponent != null)
						CreateEditDataSourceMenuItems(result, dataComponent);
				}
				result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.AddDataSource), null, DataExplorerCommands.AddDataSource));
				if (CanBeMailMergeDataSource(Node)) {
					result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeDataSource), null, DataExplorerCommands.MailMergeDataSource, true, IsMailMergeDataSourceChecked));
					result.Insert(1, new MenuItemDescription());
				}
				return result;
			}
			void CreateEditDataSourceMenuItems(MenuItemDescriptionCollection menu, IDataComponent component) {
				SqlDataSource sqlDataSource = component as SqlDataSource;
				if (sqlDataSource != null) {
					bool connSet = sqlDataSource.ConnectionName != null || sqlDataSource.ConnectionParameters != null;
					if (!connSet) return;
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.RebuildResultSchema), null, DataExplorerCommands.RebuildResultSchema));
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ManageQueries), null, DataExplorerCommands.ManageQueries));
					if (sqlDataSource.Queries.Count > 1)
						menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ManageRelations), null, DataExplorerCommands.ManageRelations));
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.ConfigureConnection), null, DataExplorerCommands.ConfigureConnection));
					return;
				}
				EFDataSource efDataSource = component as EFDataSource;
				if (efDataSource != null) {
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Configure), null, DataExplorerCommands.Configure));
					return;
				}
				ObjectDataSource objectDataSource = component as ObjectDataSource;
				if (objectDataSource != null) {
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.EditDataSource), null, DataExplorerCommands.EditObjectDataSource));
					return;
				}
				ExcelDataSource excelDataSource = component as ExcelDataSource;
				if (excelDataSource != null) {
					menu.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.EditDataSource), null, DataExplorerCommands.EditExcelDataSource));
				}
			}
		}
		protected class SnapDetailDataMemberContextMenuHelper : DataMemberContextMenuHelper {
			public SnapDetailDataMemberContextMenuHelper(DataMemberListNodeBase node, bool isMailMergeDataSourceChecked)
				: base(node) {
				IsMailMergeDataSourceChecked = isMailMergeDataSourceChecked;
			}
			protected bool IsMailMergeDataSourceChecked { get; private set; }
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection result = base.CreateMenuItems();
				if (CanBeMailMergeDataSource(Node)) {
					result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeDataSource), null, DataExplorerCommands.MailMergeDataSource, true, IsMailMergeDataSourceChecked));
					result.Insert(1, new MenuItemDescription());
				}
				return result;
			}
		}
		protected class NoNodeDataSourceContextMenuHelper : ContextMenuHelper {
			public NoNodeDataSourceContextMenuHelper(DataMemberListNodeBase node) : base(node) { }
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection result = new MenuItemDescriptionCollection();
				result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.AddDataSource), null, DataExplorerCommands.AddDataSource));
				return result;
			}
		}
		protected class UserDataSourceContextMenuHelper : SnapDataMemberContextMenuHelper {
			public UserDataSourceContextMenuHelper(DataMemberListNodeBase node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection result = base.CreateMenuItems();
				result.Insert(0, new MenuItemDescription(Node, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.AddDataSource), null, DataExplorerCommands.AddDataSource));
				result.Insert(2, new MenuItemDescription());
				return result;
			}
		}
		protected override MenuItemDescriptionCollection CreateMenuItems(DataMemberListNodeBase node) {
			if (node == null)
				return new NoNodeDataSourceContextMenuHelper(node).CreateMenuItems();
			bool isMailMergeDataSourceChecked = IsMailMergeDataSource(node);
			if (node is SnapDataSourceListNode)
				return new SnapDataSourceContextMenuHelper(SnapControl, node, isMailMergeDataSourceChecked).CreateMenuItems();
			NodeType nodeType = ContextMenuHelper.IdentifyNode(node);
			if (nodeType == NodeType.DataMemberNode)
				return new SnapDetailDataMemberContextMenuHelper(node, isMailMergeDataSourceChecked).CreateMenuItems();
			NodeType nodeTypeParent = ContextMenuHelper.IdentifyNodeParent(node);
			if (nodeType == NodeType.FieldNode && nodeTypeParent == NodeType.FieldNode)
				return new MenuItemDescriptionCollection();
			if (nodeType == NodeType.ParameterTableNode || nodeType == NodeType.ParameterNode || !node.IsDataSourceNode)
				return base.CreateMenuItems(node);
			return new UserDataSourceContextMenuHelper(node).CreateMenuItems();
		}
		public static bool CanBeMailMergeDataSource(DataMemberListNodeBase node) {
			if (node == null)
				return false;
			NodeType nodeType = ContextMenuHelper.IdentifyNode(node);
			if (nodeType == NodeType.DataMemberNode && !node.IsDataSourceNode && IsDataSet(node.Parent as DataMemberListNodeBase))
				return true;
			if (nodeType == NodeType.ParameterTableNode || nodeType == NodeType.ParameterNode || !node.IsDataSourceNode)
				return false;
			return !IsDataSet(node);
		}
		static bool IsDataSet(DataMemberListNodeBase node) {
			if (node == null || !node.IsDataSourceNode)
				return false;
			if (node.DataSource is DataSet)
				return true;
			BindingSource bs = node.DataSource as BindingSource;
			if (bs != null)
				return bs.DataSource is DataSet && string.IsNullOrEmpty(bs.DataMember);
			SqlDataSource sqlDataSource = node.DataSource as SqlDataSource;
			if (sqlDataSource != null)
				return string.IsNullOrEmpty(node.DataMember);
			return false;
		}
		public bool IsMailMergeDataSource(DataMemberListNodeBase node) {
			SnapMailMergeVisualOptions options = SnapControl.Options.SnapMailMergeVisualOptions;
			return (node.DataSource != null) && FieldListCommandExecutor.CompareNodeAndOptions(node, options);
		}
		protected internal virtual void SubscribeDocumentModelEvents() {
			if (SnapControl != null)
				SnapControl.DocumentModel.SnapMailMergeDataSourceChanged += OnDocumentModelSnapMailMergeDataSourceChanged;
		}
		protected internal virtual void UnsubscribeDocumentModelEvents() {
			if (SnapControl != null)
				SnapControl.DocumentModel.SnapMailMergeDataSourceChanged -= OnDocumentModelSnapMailMergeDataSourceChanged;
		}
		void OnDocumentModelSnapMailMergeDataSourceChanged(object sender, EventArgs e) {
			Invalidate();
		}
		protected override XtraContextMenuBase CreateContextMenu() {
			return new SNContextMenu(SnapControl);
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return null;
		}
		bool IToolTipControlClient.ShowToolTips {
			get {
				return false;
			}
		}
		public static SNDataInfo GetFieldSNDataInfoByTreeListNode(TreeListNode node) {
			NodeType nodeType = ContextMenuHelper.IdentifyNode((DataMemberListNodeBase)node);
			IDataInfoContainer dataInfoContainer = node as IDataInfoContainer;
			if (dataInfoContainer != null && nodeType != NodeType.ParameterTableNode) {
				List<string> result = new List<string>();
				List<string> escResult = new List<string>();
				CalculateResLists(node, result, escResult);
				DataInfo[] infos = dataInfoContainer.GetData();
				if (infos.Length == 1)
					return new SNDataInfo(infos[0].Source, result.ToArray(), escResult.ToArray(), infos[0].DisplayName);
			}
			return null;
		}
		static void CalculateResLists(TreeListNode node, List<string> result, List<string> escResult) {
			if (!(node is INode))
				return;
			INode currentNode = (INode)node;
			if (currentNode.IsDataSourceNode)
				return;
			List<string> buffer = new List<string>();
			string dataMember = currentNode.DataMember;
			string listDataMember = dataMember;
			while (currentNode != null) {
				if (currentNode.IsList) {
					if (listDataMember != currentNode.DataMember) {
						int lenght = listDataMember.Length - currentNode.DataMember.Length - 1;
						result.Insert(0, dataMember.Substring(currentNode.DataMember.Length + 1, lenght));
						escResult.Insert(0, String.Join(".", buffer));
						buffer.Clear();
						listDataMember = currentNode.DataMember;
					}
				}
				INode parent = (INode)currentNode.Parent;
				string piece = (parent == null || parent.DataMember == null) ? currentNode.DataMember : currentNode.DataMember.Substring(parent.DataMember.Length + 1);
				if (piece != null)
					buffer.Insert(0, EncodePath(piece));
				currentNode = parent;
			}
			result.Insert(0, listDataMember);
			escResult.Insert(0, String.Join(".", buffer));
		}
		protected override void OnDoubleClick(EventArgs e) {
			MouseEventArgs args = (MouseEventArgs)e;
			if (SnapControl == null || !IsClickLocationCorrespondsToSelection(args.Location))
				return;
			SnapControl.DocumentModel.BeginUpdate();
			try {
				OnDoubleClickCore();
			}
			finally {
				SnapControl.DocumentModel.EndUpdate();
			}
		}
		internal void OnDoubleClickCore() {
			SelectionHelper.ClearSelection(SnapControl.DocumentModel.Selection);
			PasteDataInfoTemplate();
			if (SnapControl != null)
				SnapControl.Focus();
		}
		bool IsClickLocationCorrespondsToSelection(Point location) {
			foreach (XtraListNode node in Selection)
				if (node.Bounds.Contains(location))
					return true;
			return false;
		}
		void PasteDataInfoTemplate() {
			PasteDataInfoTemplateCommand command = new PasteDataInfoTemplateCommand(SnapControl);
			command.PasteSource = new DataObjectPasteSource(GetDataObject());
			command.Execute();
		}
		protected internal virtual IDataObject GetDataObject() {
			return new DataObject(SnapDataFormats.SNDataInfoFullName, CalculateDataInfosBySelection().ToArray());
		}
		protected override void OnItemDrag(object sender, ItemDragEventArgs e) {
			IDataInfoContainer dataContainer = e.Item as IDataInfoContainer;
			if (dataContainer != null) {
				List<SNDataInfo> dataInfos = CalculateDataInfosBySelection();
				DragDropEffects effects = dataInfos.Count > 0 ? DragDropEffects.Copy : DragDropEffects.None;
				DoDragDrop(dataInfos.ToArray(), effects);
			}
		}
		internal List<SNDataInfo> CalculateDataInfosBySelection() {
			return CalculateDataInfosByNodesList(Selection);
		}
		List<SNDataInfo> CalculateDataInfosByNodesList(IList nodes) {
			List<SNDataInfo> dataInfos = new List<SNDataInfo>();
			foreach (TreeListNode item in nodes)
				dataInfos.AddRange(CalculateDataInfosByNode(item));
			return dataInfos;
		}
		List<SNDataInfo> CalculateDataInfosByNode(TreeListNode item) {
			List<SNDataInfo> itemInfos = new List<SNDataInfo>();
			NodeType nodeType = ContextMenuHelper.IdentifyNode((DataMemberListNodeBase)item);
			IDataInfoContainer dataInfoContainer = item as IDataInfoContainer;
			INode node = (INode)item;
			if (dataInfoContainer == null || nodeType == NodeType.ParameterTableNode)
				return itemInfos;
			List<string> result = new List<string>();
			List<string> escResult = new List<string>();
			CalculateResLists(item, result, escResult);
			DataInfo[] infos = dataInfoContainer.GetData();
			if (node.IsList)
				itemInfos.AddRange(CalculateDataInfosForListNode(infos, result, escResult));
			else if (node.IsComplex)
				itemInfos.AddRange(CalculateDataInfosForComplexNode(infos, result, escResult));
			else if (node.IsDataSourceNode)
				itemInfos.AddRange(CalculateDataInfosByNodesList(node.ChildNodes));
			else
				for (int i = 0; i < infos.Length; i++) {
					DataInfo info = infos[i];
					itemInfos.Add(new SNDataInfo(info.Source, result.ToArray(), escResult.ToArray(), info.DisplayName));
				}
			List<SNDataInfo> relatedData = CalculateRelatedDataInfos(item);
			if (relatedData != null)
				itemInfos.ForEach(dataInfo => dataInfo.RelatedData = relatedData.ToArray());
			return itemInfos;
		}
		List<SNDataInfo> CalculateDataInfosForListNode(DataInfo[] infos, List<string> result, List<string> escResult) {
			List<SNDataInfo> itemInfos = new List<SNDataInfo>();
			foreach (DataInfo dataInfo in infos)
				if (!string.IsNullOrEmpty(dataInfo.Member) && dataInfo.Member.Contains(".")) {
					int index = dataInfo.Member.LastIndexOf(".");
					string columnName = dataInfo.Member.Substring(index + 1);
					string[] dataPath = new string[result.Count + 1];
					string[] escDataPath = new string[escResult.Count + 1];
					Array.Copy(result.ToArray(), 0, dataPath, 0, dataPath.Length - 1);
					Array.Copy(escResult.ToArray(), 0, escDataPath, 0, escDataPath.Length - 1);
					dataPath[dataPath.Length - 1] = columnName;
					escDataPath[escDataPath.Length - 1] = EncodePath(columnName);
					itemInfos.Add(new SNDataInfo(dataInfo.Source, dataPath, escDataPath, dataInfo.DisplayName));
				}
			return itemInfos;
		}
		List<SNDataInfo> CalculateDataInfosForComplexNode(DataInfo[] infos, List<string> result, List<string> escResult) {
			List<SNDataInfo> itemInfos = new List<SNDataInfo>();
			foreach (DataInfo dataInfo in infos)
				if (!string.IsNullOrEmpty(dataInfo.Member)) {
					int index = dataInfo.Member.LastIndexOf(".");
					string columnName = index > 0 ? dataInfo.Member.Substring(index + 1) : dataInfo.Member;
					string[] dataPath = result.ToArray();
					string[] escDataPath = escResult.ToArray();
					dataPath[dataPath.Length - 1] += "." + columnName;
					escDataPath[escDataPath.Length - 1] += "." + EncodePath(columnName);
					itemInfos.Add(new SNDataInfo(dataInfo.Source, dataPath, escDataPath, dataInfo.DisplayName));
				}
			return itemInfos;
		}
		List<SNDataInfo> CalculateRelatedDataInfos(TreeListNode node) {
			DataMemberListNode dataMemberNode = node as DataMemberListNode;
			if (dataMemberNode == null)
				return null;
			List<string> dataMembers = DataRelationHelper.GetRelatedDataMembers(dataMemberNode.DataSource, dataMemberNode.DataMember);
			if (dataMembers == null || dataMembers.Count == 0)
				return null;
			List<SNDataInfo> result = new List<SNDataInfo>();
			foreach (string dataMember in dataMembers) {
				DataMemberListNodeBase relatedNode = PickManager.FindDataMemberNode(Nodes, dataMemberNode.DataSource, dataMember) as DataMemberListNodeBase;
				if (relatedNode == null)
					continue;
				List<string> dataPaths = new List<string>();
				List<string> escResult = new List<string>();
				CalculateResLists(relatedNode, dataPaths, escResult);
				IDataInfoContainer dataInfoContainer = relatedNode as IDataInfoContainer;
				if (dataInfoContainer == null)
					continue;
				DataInfo[] dataInfos = dataInfoContainer.GetData();
				for (int i = 0; i < dataInfos.Length; i++) {
					DataInfo info = dataInfos[i];
					SNDataInfo dataInfo = new SNDataInfo(info.Source, dataPaths.ToArray(), escResult.ToArray(), info.DisplayName);
					result.Add(dataInfo);
				}
			}
			return result;
		}
		static string EncodePath(string path) {
			return FieldPathService.EncodePath(path);
		}
		protected override TreeListPainter CreatePainter() {
			return new SnapTreeListPainter();
		}
	}
	public class TreeListHelper {
		readonly TreeListController controller;
		public TreeListHelper(SnapFieldListTreeView control) {
			controller = new TreeListController(control);
		}
		public SnapControl SnapControl {
			get { return controller.SnapControl; }
			set { controller.SnapControl = value; }
		}
	}
	public class TreeListController {
		readonly SnapFieldListTreeView treeList;
		SnapControl snapControl;
		public TreeListController(SnapFieldListTreeView treeList) {
			Guard.ArgumentNotNull(treeList, "treeList");
			this.treeList = treeList;
			this.treeList.MouseUp += tv_MouseUp;
		}
		public SnapControl SnapControl {
			get { return snapControl; }
			set {
				if (snapControl != value) {
					if (snapControl != null)
						UnsubscribeFromSnapControlEvents();
					snapControl = value;
					if (SnapControl != null) {
						SubscribeToSnapControlEvents();
						RefreshTreeList();
					}
				}
			}
		}
		void SubscribeToSnapControlEvents() {
			SnapControl.DataSourceChanged += snapControl_DataSourceChanged;
			SnapControl.SelectionChanged += OnSelectionChanged;
		}
		void UnsubscribeFromSnapControlEvents() {
			SnapControl.DataSourceChanged -= snapControl_DataSourceChanged;
			SnapControl.SelectionChanged -= OnSelectionChanged;
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField(SnapControl.DocumentModel);
			if (fieldInfo == null || fieldInfo.Field == null)
				return;
			SnapDocumentModel documentModel = SnapControl.DocumentModel;
			SnapPieceTable pieceTable = documentModel.ActivePieceTable;
			DesignBinding binding = FieldsHelper.GetFieldDesignBindingEx(documentModel.DataSourceDispatcher, new SnapFieldInfo(pieceTable, fieldInfo.Field));
			if (treeList.Selection.Count <= 1 && IsMatch(binding, treeList.SelectedNode))
				return;
			TreeListNode node = FindNode(binding);
			if (node != null)
				SetSelectedNode(node);
		}
		void SetSelectedNode(TreeListNode node) {
			treeList.BeginUpdate();
			try {
				treeList.Selection.Clear();
				node.Selected = true;
				treeList.FocusedNode = node;
			}
			finally {
				treeList.EndUpdate();
			}
		}
		TreeListNode FindNode(DesignBinding binding) {
			if (String.IsNullOrEmpty(binding.DataMember))
				return (TreeListNode)treeList.PickManager.FindSourceNode((IList)treeList.Nodes, binding.DataSource);
			else
				return (TreeListNode)treeList.PickManager.FindDataMemberNode((IList)treeList.Nodes, binding.DataSource, binding.DataMember);
		}
		bool IsMatch(DesignBinding binding, DataMemberListNodeBase node) {
			if (String.IsNullOrEmpty(binding.DataMember) && String.IsNullOrEmpty(node.DataMember))
				return node.HasDataSource(binding.DataSource);
			return node.HasDataSource(binding.DataSource) && String.Equals(binding.DataMember, node.DataMember, StringComparison.OrdinalIgnoreCase);
		}
		bool IsMatch(DesignBinding binding, TreeListNode node) {
			DataMemberListNodeBase dataMemberNode = node as DataMemberListNodeBase;
			if (dataMemberNode == null)
				return false;
			return IsMatch(binding, dataMemberNode);
		}
		void snapControl_DataSourceChanged(object sender, EventArgs e) {
			RefreshTreeList();
		}
		void RefreshTreeList() {
			IDataSourceCollectorService dataSourceCollector = snapControl.GetService<IDataSourceCollectorService>();
			if (dataSourceCollector != null && !snapControl.IsDesignMode)
				treeList.UpdateDataSource(snapControl, dataSourceCollector.GetDataSources());
		}
		void tv_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Right)
				return;
			DataMemberListNodeBase node = treeList.GetNodeAt(e.X, e.Y) as DataMemberListNodeBase;
			treeList.SelectNode(node);
			treeList.ShowContextMenu(node, new Point(e.X, e.Y));
		}
	}
	public class SnapPropertiesProvider : DataSortedPropertiesNativeProvider {
		public SnapPropertiesProvider(DataContext dataContext, IDataContextService serv, TypeSpecificsService typeSpecificsService)
			: base(dataContext, serv, typeSpecificsService) {
		}
		protected override PropertyDescriptor[] FilterProperties(ICollection properties, object dataSource, string dataMember) {
			return new SnapPropertyAggregator().Aggregate(properties, dataSource, dataMember);
		}
	}
	public class SnapTreeListPickManager : TreeListPickManager {
		public SnapTreeListPickManager(DataContextOptions options)
			: base(options) {
		}
		protected override bool ShouldCreateDataInfo(IPropertyDescriptor propertyDescriptor) {
			return base.ShouldCreateDataInfo(propertyDescriptor) && !propertyDescriptor.IsComplex;
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			PropertyDescriptor propertyDescriptor = ((FakedPropertyDescriptor)property).RealProperty;
			TypeSpecifics specifics = GetTypeSpecificsService().GetPropertyTypeSpecifics(propertyDescriptor);
			int index = ColumnImageProvider.Instance.GetColumnImageIndex(propertyDescriptor, specifics);
			return isList || property.IsComplex ? new SnapDataMemberListNode(dataSource, dataMember, displayName, this, (TreeListNodes)owner, propertyDescriptor, index, isList, property.IsComplex) :
				new DataMemberListNode(dataSource, dataMember, displayName, null, (TreeListNodes)owner, propertyDescriptor, index);
		}
		protected override Collection<Pair<object, string>> FilterDataSources(Collection<Pair<object, string>> dataSources) {
			Collection<Pair<object, string>> filterValues = new Collection<Pair<object, string>>();
			foreach (Pair<object, string> pair in dataSources)
				if (ListTypeHelper.IsListType(pair.First.GetType()))
					filterValues.Add(pair);
			return filterValues;
		}
		protected override INode CreateDataSourceNodeCore(object dataSource, string name, TreeListNodes owner, int index) {
			DataSourceInfo dataSourceInfo = ((SnapControl)this.ServiceProvider).DocumentModel.DataSourceDispatcher.GetInfo(dataSource);
			if (dataSourceInfo == null)
				return base.CreateDataSourceNodeCore(dataSource, name, owner, index);
			return new SnapDataSourceListNode(dataSource, name, this, (TreeListNodes)owner, index);
		}
		protected override IPropertiesProvider CreateProvider() {
			IDataContextService service = GetDataContextService();
			return service != null ? new SnapPropertiesProvider(service.CreateDataContext(options), service, GetTypeSpecificsService()) :
				base.CreateProvider();
		}
		public override void FindDataMemberNode(IList nodes, string dataMember, Action<INode> callback) {
			FindDataMemberNodeCore(nodes, dataMember, 0, callback, true);
		}
	}
	class SnapDataMemberListNode : DataMemberListNode {
		readonly bool isList;
		readonly bool isComplex;
		public SnapDataMemberListNode(object dataSource, string dataMember, string text, PickManager pickManager, TreeListNodes owner, PropertyDescriptor property, int imageIndex, bool isList, bool isComplex)
			: base(dataSource, dataMember, text, pickManager, owner, property, imageIndex) {
			this.isList = isList;
			this.isComplex = isComplex;
		}
		public override bool IsList { get { return isList; } }
		public override bool IsComplex { get { return isComplex; } }
	}
	class SnapDataSourceListNode : DataSourceListNode {
		public SnapDataSourceListNode(object dataSource, string name, TreeListPickManager pickManager, TreeListNodes owner, int imageIndex)
			: base(dataSource, name, pickManager, owner, imageIndex) {
		}
	}
	public class SnapTreeListPainter : TreeListPainter {
		public override ITreeListPaintHelper CreatePaintHelper() {
			return new SnapTreeListPaintHelper();
		}
	}
	public class SnapTreeListPaintHelper : TreeListPaintHelper {
		public override void DrawNodeStateImage(CustomDrawNodeImagesEventArgs e) {
			if (!IsMailMergeNode(e.Node))
				base.DrawNodeStateImage(e);
			else
				DrawNodeImageCore(e, e.SelectRect, e.StateImageLocation, ((SnapFieldListTreeView)e.Node.TreeList).MailMergeStateImageList, e.StateImageIndex);
		}
		bool IsMailMergeNode(TreeListNode node) {
			DataMemberListNodeBase dataMemberNode = node as DataMemberListNodeBase;
			if (dataMemberNode == null)
				return false;
			SnapMailMergeVisualOptions options = ((SnapFieldListTreeView)dataMemberNode.TreeList).SnapControl.Options.SnapMailMergeVisualOptions;
			if (dataMemberNode.DataSource != null && FieldListCommandExecutor.CompareNodeAndOptions(dataMemberNode, options))
				return true;
			return IsMailMergeNode(node.ParentNode);
		}
	}
	public class SnapTreeListHandler : TreeListHandler {
		public SnapTreeListHandler(SnapFieldListTreeView snapTreeView)
			: base(snapTreeView) {
		}
		protected override TreeListControlState CreateState(TreeListState state) {
			if(state == TreeListState.Regular)
				return new SnapRegularState(this);
			return base.CreateState(state);
		}
	}
	public class SnapRegularState : TreeListHandler.RegularState {
		public SnapRegularState(TreeListHandler handler)
			: base(handler) {
		}
		public override void MouseDown(MouseEventArgs e, TreeListHitTest ht) {
			Data.DownHitTest = ht;
			if(IsCellSelected && ShouldProcessSelection(ht.Node))
				return;
			base.MouseDown(e, ht);
		}
		public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
			if(ShouldProcessSelection(ht.Node)) {
				Data.DownHitTest = ht;
				if(IsCellSelected && e.Clicks <= 1 && (TreeList.OptionsBehavior.ImmediateEditor || TreeList.OptionsBehavior.ShowEditorOnMouseUp)) {
					OnPressNode();
					if(e.Button.IsLeft() && ht.HitInfoType == HitInfoType.RowIndicator && TreeList.OptionsSelection.UseIndicatorForSelection) {
						if(TreeList.OptionsSelection.MultiSelect) SetState(TreeListState.MultiSelection);
					}
				}
			}
			base.MouseUp(e, ht);
		}
		bool ShouldProcessSelection(TreeListNode node) {
			return TreeList.Selection.Count > 1 && TreeList.Selection.Contains(node);
		}
		bool IsCellSelected {
			get {
				return Data.DownHitTest.HitInfoType == HitInfoType.Row ||
				Data.DownHitTest.HitInfoType == HitInfoType.RowIndent ||
				Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator ||
				Data.DownHitTest.HitInfoType == HitInfoType.RowPreview ||
				Data.DownHitTest.HitInfoType == HitInfoType.Cell ||
				Data.DownHitTest.HitInfoType == HitInfoType.SelectImage ||
				Data.DownHitTest.HitInfoType == HitInfoType.StateImage ||
				Data.DownHitTest.HitInfoType == HitInfoType.RowIndicatorEdge && !CanResizeNodes;
			}
		}
	}
}
