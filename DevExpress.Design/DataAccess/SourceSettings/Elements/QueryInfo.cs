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

namespace DevExpress.Design.DataAccess {
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	sealed class QueryInfo : DataSourceElementInfo, IQueryInfo {
		PropertyDescriptor[] properties;
		public QueryInfo(IComponentDataAccessTechnologyInfoItem componentItem, string queryName)
			: base(componentItem.Type, queryName) {
			this.Component = componentItem.Component;
			using(var dataContext = new DevExpress.Data.Browsing.DataContextBase()) {
				var pdc = dataContext.GetItemProperties(Component, queryName);
				this.properties = new PropertyDescriptor[pdc.Count];
				pdc.CopyTo(properties, 0);
			}
		}
		protected override IEnumerable<string> GetFieldsCore() {
			return properties.Select(p => p.Name);
		}
		public object Component { get; private set; }
		public static IEnumerable<IQueryInfo> GetQueries(IDataAccessTechnologyInfoItem item) {
			IComponentDataAccessTechnologyInfoItem componentItem = item as IComponentDataAccessTechnologyInfoItem;
			var pQueries = componentItem.Type.GetProperty("Queries");
			if(pQueries != null && pQueries.CanRead) {
				var queries = pQueries.GetValue(componentItem.Component, new object[] { }) as IEnumerable;
				if(queries != null) {
					foreach(object query in queries) {
						var pName = query.GetType().GetProperty("Name");
						if(pName != null && pName.CanRead) {
							string name = pName.GetValue(query, new object[] { }) as string;
							yield return new QueryInfo(componentItem, name);
						}
					}
				}
			}
		}
	}
}
