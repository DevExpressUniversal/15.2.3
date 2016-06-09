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

using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class AvailableItemData {
		public enum NodeType {
			None,
			Table,
			View,
			Column
		};
		public class ColumnData {
			public ColumnData() { }
			public ColumnData(string name, string type) {
				Name = name;
				Type = type;
			}
			public string Name { get; private set; }
			public string Type { get; private set; }
		}
		public class List : ItemDataList<AvailableItemData> {
			static readonly PropertyDescriptor shadowedPropertyDescriptor =
				TypeDescriptor.GetProperties(typeof(AvailableItemData))["Shadowed"];
			readonly Dictionary<string, AvailableItemData> glossary;
			public List(QueryBuilderViewModel owner) : base(owner) {
				this.glossary = new Dictionary<string, AvailableItemData>();
			}
			public void Initialize(IEnumerable<string> tables, IEnumerable<string> views) {
				foreach(string table in tables)
					Add(CreateTableNode(NodeType.Table, table, Owner.GetShadowed(table)));
				foreach(string view in views)
					Add(CreateTableNode(NodeType.View, view, Owner.GetShadowed(view)));
			}
			public AvailableItemData this[string tableName] { get { return this.glossary[tableName]; } }
			internal void SetShadowed(AvailableItemData item, bool value) {
				SetItemProperty(item, value, shadowedPropertyDescriptor,
					data => data.Shadowed,
					(data, b) => data.shadowed = b);
			}
			AvailableItemData CreateTableNode(NodeType type, string name, bool shadowed) {
				AvailableItemData result = new AvailableItemData(type, name, shadowed);
				result.children.Add(new ColumnData());
				this.glossary.Add(name, result);
				return result;
			}
		}
		readonly BindingList<ColumnData> children;
		readonly NodeType type;
		readonly string name;
		bool shadowed;
		AvailableItemData(NodeType type, string name, bool shadowed) {
			this.children = new BindingList<ColumnData>();
			this.type = type;
			this.name = name;
			this.shadowed = shadowed;
		}
		public BindingList<ColumnData> Children { get { return this.children; } }
		public NodeType Type { get { return this.type; } }
		public string Name { get { return this.name; } }
		public bool Shadowed { get { return this.shadowed; } }
	}
}
