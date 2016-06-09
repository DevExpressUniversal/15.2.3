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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.DashboardWin.Native {
	class DataNodeFilterColumn : FilterColumn {
		readonly DataNode node;
		RepositoryItem repositoryItem;
		DataNodeFilterColumn parent;
		List<IBoundProperty> children = null;
		public override string ColumnCaption {
			get { return node.DisplayName; }
		}
		protected override string ColumnName {
			get { return node.DataMember; }
		}
		public override Type ColumnType {
			get { return node.ActualFieldType.ToType(); }
		}
		public override string FieldName {
			get { return parent == null || string.IsNullOrEmpty(node.DataMember) || string.IsNullOrEmpty(parent.node.DataMember) || !node.DataMember.StartsWith(parent.node.DataMember) ? node.DataMember : node.DataMember.Substring(parent.node.DataMember.Length + 1); }
		}
		public override Image Image {
			get { return null; } 
		}
		public override RepositoryItem ColumnEditor {
			get {
				if(repositoryItem == null) {
					DataField field = node as DataField;
					switch(field.EditorType) {
					case DataFieldFilterEditorType.DateTime:
						repositoryItem = new DataItemFilterRepositoryItemDateEdit();
						break;
					case DataFieldFilterEditorType.ComboBox:
						RepositoryItemComboBox item = new RepositoryItemComboBox();
						item.Items.AddRange(field.EditorValues);
						repositoryItem = item;
						break;
					default:
						repositoryItem = new RepositoryItemTextEdit();
						break;
					}
				}
				return repositoryItem;
			}
		}
		public override bool HasChildren {
			get {
				EnsureChildren();
				return children != null;
			}
		}
		public override bool IsAggregate {
			get {
				EnsureChildren();
				return children != null;
			}
		}
		public override IBoundProperty Parent {
			get { return node.NodeType == DataNodeType.CalculatedDataField ? null : parent; }
			set { }
		}
		public override List<IBoundProperty> Children {
			get {
				EnsureChildren();
				return children;
			}
		}
		public DataNodeFilterColumn(DataNode node) {
			this.node = node;
		}
		bool needEnsure = true;
		void EnsureChildren() {
			if(!needEnsure)
				return;
			needEnsure = false;
			node.Expand((d, e) => { });
			if(node.ChildNodes != null && node.ChildNodes.Count > 0) {
				children = new List<IBoundProperty>();
				foreach(DataNode dataNode in node.ChildNodes)
					children.Add(new DataNodeFilterColumn(dataNode) { parent = this });
			}
		}
	}
	class DataSourcePropertyCollection : FilterColumnCollection, IBoundPropertyCollection {
		public DataSourcePropertyCollection(IDataSourceSchema dataSourceSchema) {
			DataSourceNodeBase node = dataSourceSchema.RootNode;
			if(node == null)
				return;
			node.Expand((d, e) => { });
			foreach(DataNode dataNode in node.ChildNodes)
				if(dataNode.NodeType == DataNodeType.CalculatedFields) {
					foreach(FilterColumn column in new DataNodeFilterColumn(dataNode).Children)
						Add(column);
				} else
					Add(new DataNodeFilterColumn(dataNode));
		}
		IBoundProperty IBoundPropertyCollection.this[string fieldName] {
			get { return Find(fieldName, ((IList)this).Cast<IBoundProperty>().ToList()); }
		}
		IBoundProperty Find(string fieldName, List<IBoundProperty> collection) {
			if(fieldName == null || collection == null)
				return null;
			foreach(IBoundProperty property in collection) {
				if(property.Name == fieldName)
					return property;
				if(property.HasChildren && !string.IsNullOrEmpty(property.Name) && fieldName.StartsWith(property.Name + ".") && fieldName.Length > (property.Name.Length + 1)) {
					IBoundProperty cProperty = Find(fieldName.Substring(property.Name.Length + 1), property.Children);
					if(cProperty != null)
						return cProperty;
				}
				if(property.HasChildren && string.IsNullOrEmpty(property.Name) && !string.IsNullOrEmpty(fieldName)) {
					IBoundProperty cProperty = Find(fieldName, property.Children);
					if(cProperty != null)
						return cProperty;
				}
			}
			return null;
		}
	}
}
