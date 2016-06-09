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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraSpreadsheet {
	[DXToolboxItem(false)]
	public class SpreadsheetFieldListTreeView : DataSourceNativeTreeList, IToolTipControlClient, ISpreadsheetFieldList {
		#region Fields
		SpreadsheetFieldListController controller;
		#endregion
		#region Properties
		object EffectiveDataSource {
			get {
				if(SpreadsheetControl != null) {
					if(SpreadsheetControl.Document.MailMergeDataSource != null)
						return SpreadsheetControl.Document.MailMergeDataSource;
				}
				return DataSource;
			}
		}
		public void CreateController(ISpreadsheetFieldList spreadsheetFieldList) {
			controller = new SpreadsheetFieldListController(this);
		}
		string EffectiveDataMember {
			get { return DataMember ?? String.Empty; }
		}
		public bool ShowComplexChildren {
			get {
				SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
				return pickManager != null && pickManager.ShowComplexChildren;
			}
			set {
				SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
				if(pickManager == null)
					return;
				pickManager.ShowComplexChildren = value;
			}
		}
		public bool OnlyDataMembersMode {
			get {
				SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
				return pickManager != null && pickManager.OnlyDataMembersMode;
			}
			set {
				SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
				if(pickManager == null)
					return;
				pickManager.OnlyDataMembersMode = value;
			}
		}
		public bool NeedDoubleClick { get; set; }
		public ISpreadsheetControl SpreadsheetControl {
			get { return controller.SpreadsheetControl; }
			set {
				if(!DesignMode && SpreadsheetControl != value) {
					UnsubscribeDocumentModelEvents();
					serviceProvider = value;
					controller.SpreadsheetControl = value;
					RefreshTreeList();
					SubscribeDocumentModelEvents();
				}
			}
		}
		#endregion
		#region Static Members
		static void CalculateResLists(TreeListNode treeListNode, List<string> result) {
			INode node = treeListNode as INode;
			if(node == null)
				return;
			INode currentNode = node;
			string dataMember = node.DataMember;
			string listDataMebmer = dataMember;
			while(currentNode != null) {
				if(currentNode.IsList) {
					if(listDataMebmer != currentNode.DataMember) {
						int lenght = listDataMebmer.Length - currentNode.DataMember.Length - 1;
						result.Insert(0, dataMember.Substring(currentNode.DataMember.Length + 1, lenght));
						listDataMebmer = currentNode.DataMember;
					}
				}
				INode parent = (INode)currentNode.Parent;
				currentNode = parent;
			}
			result.Insert(0, listDataMebmer);
		}
		#endregion
		public SpreadsheetFieldListTreeView() : this(false){}
		public SpreadsheetFieldListTreeView(bool onlyDataMembersMode) : this(new SpreadsheetTreeListPickManager(new DataContextOptions(false, false))) {
			OnlyDataMembersMode = onlyDataMembersMode;
			ShowComplexChildren = true;
			CreateController(this);
		}
		protected SpreadsheetFieldListTreeView(TreeListPickManager pickManager)
			: base(pickManager) {
			OptionsSelection.MultiSelect = true;
			NeedDoubleClick = true;
		}
		#region ContextMenu
		#endregion
		public void RefreshTreeList() {
			if(SpreadsheetControl == null)
				return;
			this.serviceProvider = SpreadsheetControl;
			PickManager.ServiceProvider = SpreadsheetControl;
			SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
			string dataMember = GetRootDataMemberForFilter(EffectiveDataMember);
			if(pickManager != null) {
				pickManager.DataMemberFilter = dataMember;
			}
			if(dataMember==null) {
				RefreshTreeListCore(null);
			}
			else {
				RefreshTreeListCore(EffectiveDataSource);
				if(Nodes == null || Nodes.Count == 0 ||
				   (!Nodes[0].HasChildren && PickManager.FindNoneNode(Nodes) == null))
					RefreshTreeListCore(null);
			}
		}
		void RefreshTreeListCore(object dataSource) {
			Stop();
			BeginUpdate();
			if(serviceProvider != null) {
				Collection<Pair<object, string>> dataSourcesPairs = new Collection<Pair<object, string>>();
				if(dataSource != null) {
					SpreadsheetTreeListPickManager pickManager = PickManager as SpreadsheetTreeListPickManager;
					if(pickManager != null) {
						dataSourcesPairs.Add(new Pair<object, string>(dataSource, pickManager.DataMemberFilter));
					}
				}
				PickManager.FillContent(Nodes, dataSourcesPairs, dataSourcesPairs.Count == 0);
			}
			else
				Nodes.Clear();
			EndUpdate();
			Start();
		}
		string GetRootDataMemberForFilter(string dataMemberFilter) {
			if(String.IsNullOrEmpty(dataMemberFilter))
				return String.Empty;
			return TryFullDataMember(dataMemberFilter)
					   ? dataMemberFilter
					   : GetRootDataMemberForFilterForce(SpreadsheetControl.Document.MailMergeDataMember, dataMemberFilter);
		}
		bool TryFullDataMember(string dataMemberFilter) {
			IDataContextService service = new DataContextServiceBase();
			PropertiesProvider provider = new PropertiesProvider(service.CreateDataContext(new DataContextOptions(false, false)), null);
			IPropertyDescriptor[] properties = new IPropertyDescriptor[] { };
			provider.GetItemProperties(EffectiveDataSource, dataMemberFilter, (s, e) => properties = e.Properties);
			provider.Dispose();
			return properties.Length > 0;
		}
		IPropertyDescriptor[] GetListItemProperties(object dataSource, string member) {
			IDataContextService service = new DataContextServiceBase();
			SpreadsheetDataMembersOnlyPropertiesProvider provider = new SpreadsheetDataMembersOnlyPropertiesProvider(service.CreateDataContext(new DataContextOptions(false, false)), service);
			IPropertyDescriptor[] properties = new IPropertyDescriptor[] { };
			provider.GetListItemProperties(dataSource, member, (s, e) => properties = e.Properties);
			provider.Dispose();
			return properties;
		}
		string GetRootDataMemberForFilterForce(string rootMember, string dataMemberFilter) {
			if(String.IsNullOrEmpty(dataMemberFilter))
				return String.Empty;
			Queue<IPropertyDescriptor> propertiesQueue = new Queue<IPropertyDescriptor>();
			Queue<string> parentQueue = new Queue<string>();
			Queue<int> levelQueue = new Queue<int>();
			string[] filter = dataMemberFilter.Split('.');
			IPropertyDescriptor[] properties = GetListItemProperties(EffectiveDataSource, rootMember);
			for(int i = 0; i < properties.Length; i++) {
				propertiesQueue.Enqueue(properties[i]);
				levelQueue.Enqueue(0);
				parentQueue.Enqueue(rootMember);
			}
			while(propertiesQueue.Count > 0) {
				IPropertyDescriptor current = propertiesQueue.Dequeue();
				int level = levelQueue.Dequeue();
				string parent = parentQueue.Dequeue();
				parent = String.IsNullOrEmpty(parent) ? current.Name : parent + "." + current.Name;
				string[] name = current.Name.Split('.');
				bool match = true;
				for(int i = 0; i < name.Length; i++) {
					if(i + level < filter.Length && name[i] == filter[level + i])
						continue;
					match = false;
					break;
				}
				if(match) {
					level += name.Length;
					if(level == filter.Length) {
						return parent;
					}
				}
				else
					level = 0;
				properties = GetListItemProperties(EffectiveDataSource, parent);
				for (int i = 0; i < properties.Length; i++) {
					propertiesQueue.Enqueue(properties[i]);
					levelQueue.Enqueue(level);
					parentQueue.Enqueue(parent);
				}				
			}
			return null;
		}
		protected internal virtual void SubscribeDocumentModelEvents() {
		}
		protected internal virtual void UnsubscribeDocumentModelEvents() {
		}
		#region ToolTips
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return null;
		}
		bool IToolTipControlClient.ShowToolTips {
			get {
				return false;
			}
		}
		#endregion
		protected override void OnDoubleClick(EventArgs e) {
			if(!NeedDoubleClick)
				return;
			MouseEventArgs args = (MouseEventArgs)e;
			if (SpreadsheetControl == null || !IsClickLocationCorrespondsToSelection(args.Location))
				return;
			PasteDataInfoInCell();
		}
		protected override void OnItemDrag(object sender, ItemDragEventArgs e) {
			IDataInfoContainer dataContainer = e.Item as IDataInfoContainer;
			if (dataContainer != null) {
				List<PasteFieldListInfoHelper> fields = CalculatePasteFieldListInfoHelpersBySelection();
				DragDropEffects effects = fields.Count > 0 ? DragDropEffects.Copy : DragDropEffects.None;
				DoDragDrop(fields.ToArray(), effects);
			}
		}
		bool IsClickLocationCorrespondsToSelection(Point location) {
			foreach (XtraListNode node in Selection)
				if (node.Bounds.Contains(location))
					return true;
			return false;
		}
		void PasteDataInfoInCell() {
			if (SpreadsheetControl.ActiveCell == null)
				return;
			List<PasteFieldListInfoHelper> fields = CalculatePasteFieldListInfoHelpersBySelection();
			if(fields.Count == 0)
				return;
			PasteFieldListInfoCommand command = new PasteFieldListInfoCommand(SpreadsheetControl, SpreadsheetControl.InnerControl.DocumentModel.ActiveSheet.Selection.AsRange(), fields.ToArray());
			command.Execute();
		}
		public string[] SelectedNodes() {
			List<string> result = new List<string>();
			string[] filter = EffectiveDataMember.Split('.');
			foreach (TreeListNode node in Selection) {
				List<string> resultPath = new List<string>();
				CalculateResLists(node, resultPath);
				string name = string.Join(".", resultPath);
				if(String.IsNullOrEmpty(name))
					continue;
				name = PasteFieldListInfoCommand.GetFilteredFieldName(filter, name);
				result.Add(name);
			}
			return result.ToArray();
		}
		public string GetSelectedNodeShortName() {
			return GetShortName(GetSelectedNodeName());
		}
		public string GetSelectedNodeName() {
			List<string> resultPath = new List<string>();
			TreeListNode node = SelectedNode;
			if (node == null)
				return String.Empty;
			CalculateResLists(node, resultPath);
			string name = string.Join(".", resultPath);
			return name;
		}
		public static string GetShortName(string fullFieldName) {
			int lastPointIndex = fullFieldName.LastIndexOf('.') + 1;
			return fullFieldName.Substring(lastPointIndex);
		}
		List<PasteFieldListInfoHelper> CalculatePasteFieldListInfoHelpersBySelection() {
			List<PasteFieldListInfoHelper> pasteFieldListInfoHelpers = new List<PasteFieldListInfoHelper>();
			foreach (DataMemberListNodeBase node in Selection) {
				NodeType nodeType = ContextMenuHelper.IdentifyNode(node);
				if(node == null || nodeType == NodeType.ParameterTableNode || node.IsDataSourceNode)
					continue;
				if (node.IsList) {
					IPropertyDescriptor[] properties = null;
					IDataContextService service = new DataContextServiceBase();
					PropertiesProvider provider = new PropertiesProvider(service.CreateDataContext(new DataContextOptions(false, false)), null);
					provider.GetListItemProperties(node.DataSource, node.DataMember, (s, e) => properties = e.Properties);
					provider.Dispose();
					foreach (IPropertyDescriptor property in properties) {
						if (property.IsListType || property.IsComplex)
							continue;
						string dataMember = !String.IsNullOrEmpty(node.DataMember) ? String.Concat(node.DataMember, ".", property.Name) : property.Name;
						bool isFieldPicture = property.Specifics == TypeSpecifics.Array;
						PasteFieldListInfoHelper result = new PasteFieldListInfoHelper(isFieldPicture, dataMember);
						pasteFieldListInfoHelpers.Add(result);
					}
				}
				else {
					bool isFieldPicture = node.Property.PropertyType == typeof(Byte[]) || typeof(Image).IsAssignableFrom(node.Property.PropertyType);
					string dataMember = node.DataMember;
					PasteFieldListInfoHelper result = new PasteFieldListInfoHelper(isFieldPicture, dataMember);
					pasteFieldListInfoHelpers.Add(result);
				}
			}
			return pasteFieldListInfoHelpers;
		}
		public void SelectNode(string fieldName) {			
			TreeListNode node = FindNode(fieldName);
			if(node == null)
				return;
			Selection.Clear();
			node.Selected = true;
			FocusedNode = node;
		}
		public TreeListNode FindNode(string fieldName) {
			string fullName = fieldName;
			if (!String.IsNullOrEmpty(EffectiveDataMember))
				fullName  = EffectiveDataMember + "." + fieldName;
			TreeListNode result;
			if (String.IsNullOrEmpty(fullName))
				result =  (TreeListNode)PickManager.FindSourceNode(Nodes, EffectiveDataSource);
			else
				result = (TreeListNode)PickManager.FindDataMemberNode(Nodes, EffectiveDataSource, fullName);
			return result ?? (ForceFindNode(fieldName));
		}
		TreeListNode ForceFindNode(string fieldName) {
			Queue<XtraListNode> nodesQueue = new Queue<XtraListNode>();
			foreach(XtraListNode node in Nodes) {
				nodesQueue.Enqueue(node);
			}
			while(nodesQueue.Count > 0) {
				XtraListNode current = nodesQueue.Dequeue();
				if(current.Text == fieldName) {
					return current;
				}
				if(current.Level > 10)
					continue;
				current.Expand();
				foreach(XtraListNode node in current.Nodes) {
					nodesQueue.Enqueue(node);
				}
				current.Collapse();
			}
			return null;
		}
		public override void ExpandAll() {
			ExpandToLevel(10);
		}
	}
	internal class SpreadsheetTreeListPickManager : TreeListPickManager {
		public bool OnlyDataMembersMode { get; set; }
		public string DataMemberFilter { get; set; }
		public bool ShowComplexChildren { get; set; }
		public SpreadsheetTreeListPickManager(DataContextOptions options)
			: base(options) {
			OnlyDataMembersMode = false;
			ShowComplexChildren = true;
			DataMemberFilter = String.Empty;
		}
		protected override bool ShouldCreateDataInfo(IPropertyDescriptor propertyDescriptor) {
			return base.ShouldCreateDataInfo(propertyDescriptor) && !propertyDescriptor.IsComplex;
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			PropertyDescriptor propertyDescriptor = ((FakedPropertyDescriptor) property).RealProperty;
			TypeSpecifics specifics = GetTypeSpecificsService().GetPropertyTypeSpecifics(propertyDescriptor);
			if(specifics == TypeSpecifics.Default && typeof(Image).IsAssignableFrom(propertyDescriptor.PropertyType)) {  
				specifics = TypeSpecifics.Array;
			}
			int index = ColumnImageProvider.Instance.GetColumnImageIndex(propertyDescriptor, specifics);
			TreeListNodes treeListNodes = (TreeListNodes) owner;
			return isList || property.IsComplex
					   ? new SpreadsheetDataMemberListNode(dataSource, dataMember, displayName, this, treeListNodes, propertyDescriptor, index, isList, property.IsComplex)
					   : new DataMemberListNode(dataSource, dataMember, displayName, null, treeListNodes, propertyDescriptor, index);
		}
		protected override Collection<Pair<object, string>> FilterDataSources(Collection<Pair<object, string>> dataSources) {
			Collection<Pair<object, string>> filterValues = new Collection<Pair<object, string>>();
			foreach(Pair<object, string> pair in dataSources)
				if(ListTypeHelper.IsListType(pair.First.GetType()))
					filterValues.Add(pair);
			return filterValues;
		}
		public override void FindDataMemberNode(IList nodes, string dataMember, Action<INode> callback) {
			FindDataMemberNodeCore(nodes, dataMember, 0, callback, true);
		}
		protected override IPropertiesProvider CreateProvider() {
			IDataContextService service = new DataContextServiceBase();
			return OnlyDataMembersMode || !ShowComplexChildren 
				? new SpreadsheetDataMembersOnlyPropertiesProvider(service.CreateDataContext(options), service) {DataMembersOnly = OnlyDataMembersMode, ShowComplexChildren = ShowComplexChildren} 
				: base.CreateProvider();
		}
		public override void GetDataSourceName(object dataSource, string dataMember, IPropertiesProvider provider,
											   EventHandler<GetDataSourceDisplayNameEventArgs> callback) {
			if(String.IsNullOrEmpty(DataMemberFilter))
				base.GetDataSourceName(dataSource, String.IsNullOrEmpty(DataMemberFilter) ? dataMember : DataMemberFilter, provider, callback);
			else
				callback(this, new GetDataSourceDisplayNameEventArgs(SpreadsheetFieldListTreeView.GetShortName(DataMemberFilter)));
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			if (String.IsNullOrEmpty(dataMember))
				return base.CreateDataSourceNode(dataSource, dataMember, name, owner);
			TypeSpecifics specifics = TypeSpecifics.List;
			int index = ColumnImageProvider.Instance.GetDataSourceImageIndex(dataSource, specifics);
			return CreateDataSourceNodeCore(dataSource, name, (TreeListNodes)owner, index);
		}
	}
	class SpreadsheetDataMemberListNode : DataMemberListNode {
		readonly bool isList;
		readonly bool isComplex;
		public SpreadsheetDataMemberListNode(object dataSource, string dataMember, string text, PickManager pickManager, TreeListNodes owner, PropertyDescriptor property, int imageIndex, bool isList, bool isComplex)
			: base(dataSource, dataMember, text, pickManager, owner, property, imageIndex) {
			this.isList = isList;
			this.isComplex = isComplex;
		}
		public override bool IsList { get { return isList; } }
		public override bool IsComplex { get { return isComplex; } }
	}
	class SpreadsheetDataMembersOnlyPropertiesProvider : DataSortedPropertiesNativeProvider {
		#region Properties
		public bool DataMembersOnly { get; set; }
		public bool ShowComplexChildren { get; set; }
		#endregion
		public SpreadsheetDataMembersOnlyPropertiesProvider(DataContext dataContext, IDataContextService serv) :
			base(dataContext, serv, new XRTypeSpecificService()) {
			DataMembersOnly = true;
			ShowComplexChildren = true;
		}
		protected override bool CanProcessProperty(IPropertyDescriptor property) {
			if (!base.CanProcessProperty(property))
				return false;
			Type propertyType = GetPropertyType(property);
			bool showComplexProperty = ShowComplexChildren || ((FakedPropertyDescriptor) property).RealProperty.GetType().FullName != "System.Data.DataRelationPropertyDescriptor";
			bool showDataMemberOnly = !DataMembersOnly || ((typeof(IList).IsAssignableFrom(propertyType) && !propertyType.IsArray) || DataContext.IsComplexType(propertyType));
			return showDataMemberOnly && showComplexProperty;
		}
	}
}
