#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.DashboardCommon.DB;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using System.Linq.Expressions;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.Native {
	public class PickManager : PickManagerBase, IDataSourceSchema {
		readonly IDashboardDataSource dataSource;
		DataSourceNodeBase rootNode;
		string dataSourceNodeName;
		string dataMember;
		public DataSourceNodeBase RootNode { get { return rootNode; } }
		public IDashboardDataSource DataSource { get { return dataSource; } }
		public string DataSourceDisplayName { get { return dataSourceNodeName; } set { dataSourceNodeName = value; } }
		public string DataSourceName { get { return dataSource.ComponentName; } }
		public IEnumerable<CalculatedField> CalculatedFields { get { return DataSource.CalculatedFields.Where(field => field.DataMember == dataMember); } }
		public PickManager(string name, IDashboardDataSource dataSource, string dataMember) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			this.dataSourceNodeName = name;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		DataNode SearchSecondLevelNodes(IList nodes, string dataMember) {
			foreach (DataNode node in nodes) {
				if (node.ChildNodes.Count > 0) {
					if (node.ChildNodes.Count == 1 && ((DataNode)node.ChildNodes[0]).IsDummyNode)
					   node.Expand((sender, e) => { });
					foreach (DataNode childNode in node.ChildNodes)
						if (childNode.DataMember == dataMember)
							return childNode;
				}
			}
			return null;
		}
		DataField SearchLastLevelNodes(IList nodes, string dataMember) {
			foreach(DataNode node in nodes) {
				if (node.ChildNodes.Count > 0) {
					if (node.ChildNodes.Count == 1 && ((DataNode)node.ChildNodes[0]).IsDummyNode)
						node.Expand((sender, e) => { });
					DataField foundField = SearchLastLevelNodes(node.ChildNodes, dataMember);
					if (foundField != null)
						return foundField;
				}
				else {
					if (node.DataMember == dataMember)
						return node as DataField;
					OlapHierarchyDataField hierarchyDataField = node as OlapHierarchyDataField;
					if (hierarchyDataField != null)
						foreach (string hierarchyDataMember in hierarchyDataField.GroupDataMembers)
							if (hierarchyDataMember == dataMember)
								return hierarchyDataField;
				}
			}
			return null;
		}
		protected override bool NodeIsEmpty(INode node) {
			return node.IsEmpty;
		}
		protected override object CreateNoneNode(object owner) {
			throw new NotSupportedException();
		}
		protected override INode CreateDummyNode(object owner) {
			return new DataNode(this);
		}
		protected override IPropertiesProvider CreateProvider() {
			return new PropertiesProvider(new DashboardDataContext(), null);
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			return new DataSourceNode(this);
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			if (!property.IsComplex && !property.IsListType) {
				FakedPropertyDescriptor fakedPropertyDescriptor = (FakedPropertyDescriptor)property;
				SchemaColumnPropertyDescriptor columnDescriptor = fakedPropertyDescriptor.RealProperty as SchemaColumnPropertyDescriptor;
				string actualDataMember = (columnDescriptor == null || columnDescriptor.IncludeTableNameInDataMember) ? dataMember: displayName;
				return new DataField(this, actualDataMember, GetDisplayName(property, displayName), fakedPropertyDescriptor.RealProperty.PropertyType);
			}
			return new DataMemberNode(this, dataMember, displayName, isList);
		}
		string GetDisplayName(IPropertyDescriptor property, string displayName) {
			string caption;
			if (displayName != property.Name || !DevExpress.Data.Helpers.MasterDetailHelper.TryGetDataTableDataColumnCaption(((FakedPropertyDescriptor)property).RealProperty, out caption))
				return displayName;
			return caption;
		}
		protected override bool ShouldAddDummyNode(IPropertyDescriptor property) {
			return property.IsComplex && !property.IsListType;
		}
		public void ConstructTree(string dataSourceNodeName) {
			this.dataSourceNodeName = dataSourceNodeName;
			List<DataNode> nodes = new List<DataNode>();
			PivotCustomizationFieldsTreeBase olapNodes = dataSource.GetDataSchema(dataSourceNodeName) as PivotCustomizationFieldsTreeBase;
			if(olapNodes == null)
				FillNodes(dataSource.GetDataSchema(dataSourceNodeName), string.Empty, nodes);
			else
				FillOlapNodes(olapNodes, nodes);
			if (nodes.Count != 1)
				throw new Exception("Single root node is expected");
			if(olapNodes == null && nodes[0].ChildNodes.Count > 0 && DataSource != null && CalculatedFields.Count() > 0)
				nodes[0].ChildNodes.Add(new CalculatedFieldsNode(this));
			rootNode = (DataSourceNodeBase)nodes[0];
		}
		void FillOlapNodes(PivotCustomizationFieldsTreeBase tree, List<DataNode> nodes) {
			IEnumerator enumerator = ((IEnumerable)tree).GetEnumerator();
			enumerator.MoveNext();
			OlapDataSourceNode node = new OlapDataSourceNode(this);
			nodes.Add(node);
			PopulateOlapTree(node, (ICustomizationTreeItem)enumerator.Current);
		}
		void PopulateOlapTree(DataNode node, ICustomizationTreeItem item) {
			foreach(ICustomizationTreeItem childItem in item.EnumerateChildren()) {
				DataNode childNode = OlapDataNodesCreator.CreateOlapNode(this, childItem);
				if(childNode != null) {
					node.ChildNodes.Add(childNode);
					PopulateOlapTree(childNode, childItem);
				}
			}
		}		   
		public void ExpandNode(DataNode node) {
			Guard.ArgumentNotNull(node, "node");
			Guard.ArgumentNotNull(rootNode, "rootNode");
			OnNodeExpand(dataSource.GetDataSchema(dataSourceNodeName), node);
		}
		public DataNode FindNode(string dataMember) {
			DataNode foundNode = null;
			OlapDataSourceNode olapRootNode = rootNode as OlapDataSourceNode;
			if (olapRootNode != null) {
				foundNode = SearchLastLevelNodes(rootNode.ChildNodes, dataMember);
			}
			else
				if (rootNode != null) {
					FindDataMemberNodeCore(rootNode.ChildNodes, dataMember, 0, node => foundNode = (DataNode)node, true);
					if (foundNode == null)
						foundNode = SearchSecondLevelNodes(rootNode.ChildNodes, dataMember);
				}
			return foundNode;
		}
		public DataField FindDataField(string dataMember) {
			DataField field = FindNode(dataMember) as DataField;
			if(field != null || dataSource == null || (dataSource.CalculatedFields != null && !dataSource.CalculatedFields.ContainsName(dataMember)) || rootNode == null || rootNode.ChildNodes.Count == 0)
				return field;
			CalculatedFieldsNode node = rootNode.ChildNodes[rootNode.ChildNodes.Count - 1] as CalculatedFieldsNode;
			if(node == null)
				return null;
			FindDataMemberNodeCore(node.ChildNodes, dataMember, 0, fnode => field = fnode as DataField, true);			
			if (field == null) {
				field = SearchSecondLevelNodes(rootNode.ChildNodes, dataMember) as DataField;
			}
			return field;
		}
		public IList GetHierarchyDataNodes(OlapHierarchyDataField field) {
			List<OlapHierarchyDataFieldItem> nodes = new List<OlapHierarchyDataFieldItem>();
			for(int i = 0; i < field.GroupDataMembers.Count; i++)
				nodes.Add(new OlapHierarchyDataFieldItem(this, field, i));
			return nodes;
		}
		public string GetFieldCaption(string dataMember) {
			DataField dataField = FindNode(dataMember) as DataField;
			return dataField != null ? dataField.Caption : dataMember;
		}
		public string GetDataMemberCaption(string dataMember) {
			DataField dataField = FindNode(dataMember) as DataField;
			OlapHierarchyDataField hierarchyDataField = dataField as OlapHierarchyDataField;
			if(hierarchyDataField != null)
				return hierarchyDataField.GroupCaptions[hierarchyDataField.GroupDataMembers.IndexOf(dataMember)];
			return dataField != null ? dataField.Caption : dataMember;
		}
		public bool IsOlapHierarchyDataField(string dataMember) {
			return FindNode(dataMember) is OlapHierarchyDataField;
		}
		public void Clear() {
			rootNode = null;
		}
		public DataField GetField(string dataMember) {
			return String.IsNullOrEmpty(dataMember) ? null : FindDataField(dataMember);
		}
		public virtual DataFieldType GetFieldType(string fieldName) {
			return DataBindingHelper.GetDataFieldType(GetFieldSourceType(fieldName));
		}
		public virtual Type GetFieldSourceType(string fieldName) {
			DataField field = GetField(fieldName);
			return field != null ? field.SourceType : typeof(object);
		}
		public bool ContainsField(string dataMember) {
			return GetField(dataMember) != null;
		}
		public List<string> GetDataMembers() {
			if(RootNode != null) {
				IList<DataField> dataFields = RootNode.GetAllDataFields();
				dataFields = dataFields.Where<DataField>(field => {
					return field.NodeType != DataNodeType.CalculatedDataField;
				}).ToList();
				return dataFields.Select<DataField, string>(field => {
					return field.DataMember;
				}).ToList();
			}
			return new List<string>();
		}
		public virtual string GetUniqueNamePropertyName(string propertyName) {
			return propertyName;
		}
		public virtual string GetDataItemDefinitionDisplayText(string dataMember) {
			return dataMember;
		}
		public virtual bool GetIsOlap() {
			return false;
		}
		public virtual string ConstructName(string baseName, string nameSuffix) {
			return string.IsNullOrEmpty(nameSuffix) ? baseName :
				string.Format(DashboardLocalizer.GetString(DashboardStringId.FormatStringDataItemName), baseName, nameSuffix);
		}
		public virtual bool IsAggregateCalcField(string dataMember) { return false; }
	}
	class DashboardDataContext : DataContext {
		public DashboardDataContext()
			: base(true) {
		}
	}
	public class PickManagerWithCalcFields : PickManager {		
		CalculatedFieldsController calculatedFieldsController;
		public PickManagerWithCalcFields(string name, IDashboardDataSource dataSource, string dataMember, CalculatedFieldsController calculatedFieldsController)
			: base(name, dataSource, dataMember) {			
			this.calculatedFieldsController = calculatedFieldsController;
		}
		public override Type GetFieldSourceType(string dataMember) {
			DataField field = GetField(dataMember);
			if(field != null)
				return field.SourceType;
			CalculatedField calculatedField = calculatedFieldsController.CalculatedFields[dataMember];
			if(calculatedField != null)
				return calculatedField.DataType.ToType();
			return typeof(object);
		}
		public override bool IsAggregateCalcField(string dataMember) {
			CalculatedField calcField = calculatedFieldsController.CalculatedFields[dataMember];
			return calcField != null && calcField.CheckHasAggregate(calculatedFieldsController.CalculatedFields);
		}
	}
}
