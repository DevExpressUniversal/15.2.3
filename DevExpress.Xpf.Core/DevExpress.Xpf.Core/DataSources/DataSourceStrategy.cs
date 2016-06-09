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
using System.Reflection;
using DevExpress.Xpf.Core.ServerMode;
using System.Linq;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using System.Data;
namespace DevExpress.Xpf.Core.DataSources {
	public class DataSourceStrategyBase {
		protected readonly IDataSource owner;
		protected virtual Type OwnerDataMemberType { get { return ((PropertyInfo)GetDataMemberInfo()).PropertyType; } }
		public DataSourceStrategyBase(IDataSource owner) {
			this.owner = owner;
		}
		public virtual bool CanGetDesignData() {
			return this.owner.ContextType != null && !string.IsNullOrEmpty(this.owner.Path);
		}
		public virtual bool CanUpdateData() {
			return CanGetDesignData();
		}
		public virtual object CreateContextIstance() {
			return Activator.CreateInstance(this.owner.ContextType);
		}
		public virtual object CreateData(object value) {
			return value;
		}
		public virtual object GetDataMemberValue(object contextInstance) {
			return ((PropertyInfo)GetDataMemberInfo()).GetValue(contextInstance, null);
		}
		public MemberInfo GetDataMemberInfo() {
			if(!CanGetDesignData()) return null;
			return GetDataMemberInfoCore();
		}
		protected virtual MemberInfo GetDataMemberInfoCore() {
			return this.owner.ContextType.GetProperty(this.owner.Path);
		}
		public virtual Type GetDataObjectType() {
			return OwnerDataMemberType;
		}
		public virtual List<DesignTimePropertyInfo> GetDesignTimeProperties() { return null; }
	}
	class TypedDataSourceStrategy : DataSourceStrategyBase {
		public TypedDataSourceStrategy(ITypedDataSource owner) : base(owner) { }
		private ITypedDataSource Owner { get { return (ITypedDataSource)this.owner; } }
		public override bool CanUpdateData() {
			return base.CanUpdateData() && Owner.AdapterType != null;
		}
		public override List<DesignTimePropertyInfo> GetDesignTimeProperties() {
			object instance = base.CreateContextIstance();
			PropertyInfo info = instance.GetType().GetProperties().FirstOrDefault((p) => {
				return p.Name == "Tables";
			});
			if(info == null) return null;
			object value = info.GetValue(instance, null);
			var dc = value as DataTableCollection;
			if(dc == null || dc.Count != 1) return null;
			List<DesignTimePropertyInfo> DesignTimeProperties = new List<DesignTimePropertyInfo>();
			foreach(DataColumn c in dc[0].Columns) {
				DesignTimeProperties.Add(new DesignTimePropertyInfo(c.ColumnName, c.DataType, c.ReadOnly));
			}
			return DesignTimeProperties;
		}
		public override Type GetDataObjectType() {
			return OwnerDataMemberType.BaseType.GetGenericArguments()[0];
		}
		public override object CreateContextIstance() {
			object instance = base.CreateContextIstance();
			object adapter = Activator.CreateInstance(Owner.AdapterType);
			MethodInfo fillMethod = Owner.AdapterType.GetMethods().First((m) => {
				ParameterInfo[] parameters = m.GetParameters();
				return m.Name == "Fill" && m.ReturnType == typeof(int) && parameters.Count() == 1 && parameters[0].ParameterType == OwnerDataMemberType;
			});
			fillMethod.Invoke(adapter, new object[1] { GetDataMemberValue(instance) });
			return instance;
		}
		public override object CreateData(object value) {
			return value.GetType().GetProperty("DefaultView").GetValue(value, null);
		}
	}
	class QueryableServerModeDataSourceStrategy : GenericPropertyDataSourceStrategy {
		public QueryableServerModeDataSourceStrategy(IQueryableServerModeDataSource owner) : base(owner) { }
		private IQueryableServerModeDataSource Owner { get { return (IQueryableServerModeDataSource)this.owner; } }
		public override object CreateData(object value) {
			Owner.QueryableSource = value as IQueryable;
			return Owner.Data;
		}
	}
	public class BaseDataSourceStrategySelector {
		public virtual DataSourceStrategyBase SelectStrategy(IDataSource dataSource, DataSourceStrategyBase currentStrategy) {
			return currentStrategy;
		}
	}
	class EntityFrameworkStrategySelector : BaseDataSourceStrategySelector {
		public override DataSourceStrategyBase SelectStrategy(IDataSource dataSource, DataSourceStrategyBase currentStrategy) {
			if(dataSource == null || dataSource.ContextType == null) return currentStrategy;
			if(dataSource.ContextType.BaseType.FullName == "System.Data.Entity.DbContext") return new EF5_DataSourceStrategy(dataSource);
			return new GenericPropertyDataSourceStrategy(dataSource);
		}
	}
}
