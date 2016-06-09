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
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using System.Linq;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	class PickManager : PickManagerBase {
		readonly Dictionary<Type, Dictionary<string, int>> cache = new Dictionary<Type, Dictionary<string, int>>();
		readonly object dataSource;
		public PickManager(object dataSource) {
			this.dataSource = dataSource;
		}
		protected override bool NodeIsEmpty(INode node) {
			return node.IsEmpty;
		}
		protected override object CreateNoneNode(object owner) {
			throw new NotSupportedException();
		}
		protected override INode CreateDummyNode(object owner) {
			return new PickManagerNode();
		}
		protected override IPropertiesProvider CreateProvider() {
			return new QueryablePropertiesProvider(cache, new DataContext(), null);
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			return new PickManagerNode();
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			if(!property.IsListType) {
				if(property.IsComplex) {
					PickManagerNode nodecore = new PickManagerNode(dataMember, displayName, isList, true);
					((INode)nodecore).ChildNodes.Add(CreateDummyNode(((INode)nodecore).ChildNodes));
					OnNodeExpand(dataSource, nodecore);
					return nodecore;
				}
				else {
					FakedPropertyDescriptor fakedPropertyDescriptor = (FakedPropertyDescriptor)property;
					return new PickManagerNode(dataMember, GetDisplayName(property, displayName), fakedPropertyDescriptor.RealProperty.PropertyType, isList);
				}
			}
			return new PickManagerNode(dataMember, displayName, isList, false);
		}
		string GetDisplayName(IPropertyDescriptor property, string displayName) {
			string caption;
			if(displayName != property.Name || !DevExpress.Data.Helpers.MasterDetailHelper.TryGetDataTableDataColumnCaption(((FakedPropertyDescriptor)property).RealProperty, out caption))
				return displayName;
			return caption;
		}
		public INode ConstructTree() {
			List<INode> nodes = new List<INode>();
			IQueryable q = dataSource as IQueryable;
			object ds = dataSource;
			if(q != null)
				ds = Activator.CreateInstance(typeof(List<>).MakeGenericType(q.ElementType));
			FillNodes(ds, string.Empty, nodes);
			if(nodes.Count != 1)
				throw new Exception("Single root node is expected");
			return nodes[0];
		}
		public override void FillNodes(object dataSource, string dataMember, System.Collections.IList nodes) {
			base.FillNodes(dataSource, dataMember, nodes);
		}
		class QueryablePropertiesProvider : PropertiesProvider {
			Dictionary<Type, Dictionary<string, int>> cache;
			public QueryablePropertiesProvider(Dictionary<Type, Dictionary<string, int>> cache, DataContext dataContext, ITypeSpecificsService serv) : base(dataContext, serv) { 
				this.cache = cache;
			}
			public override void GetListItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> action) {
				Type type = DataContext.GetPropertyType(dataSource, dataMember);
				int index = dataMember.LastIndexOf('.');
				string dataM;
				if(index > 0)
					dataM = dataMember.Substring(index + 1);
				else
					dataM = dataMember;
				Dictionary<string, int> hash;
				if(!cache.TryGetValue(type, out hash)) {
					hash = new Dictionary<string, int>();
					cache.Add(type, hash);
				}
				int count;
				if(!hash.TryGetValue(dataM, out count)) {
					hash.Add(dataM, 0);
				}
				if(count > 5) 
					return;
				hash[dataM] = count + 1;
				try {
					base.GetListItemProperties(dataSource, dataMember, action);
				} finally {
					hash[dataM] = count - 1;
				}
			}
		}
	}
}
