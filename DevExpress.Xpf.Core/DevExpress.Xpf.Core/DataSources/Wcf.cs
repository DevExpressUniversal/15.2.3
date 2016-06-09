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
using System.Linq;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.DataSources {
	public interface IWcfDataSource : IDataSource {
		Uri ServiceRoot { get; }
	}
	public interface IWcfStandartDataSource : IWcfDataSource {
	}
	public interface IWcfServerModeDataSource : IWcfDataSource {
		object DataServiceContext { get; set; }
		IQueryable Query { get; set; }
	}
	public class GenericPropertyDataSourceStrategy : DevExpress.Xpf.Core.DataSources.DataSourceStrategyBase {
		public GenericPropertyDataSourceStrategy(IDataSource owner) : base(owner) { }
		public override Type GetDataObjectType() {
			return OwnerDataMemberType.IsGenericType ? OwnerDataMemberType.GetGenericArguments()[0] : null;
		}
	}
	class EF5_DataSourceStrategy : GenericPropertyDataSourceStrategy {
		const string EF5_EntityExtension = "System.Data.Entity.DbExtensions, EntityFramework";
		const string EF6_EntityExtension = "System.Data.Entity.QueryableExtensions, EntityFramework";
		Type GetLoadMethodExtensionType() {
			Type ef5_type = Type.GetType(EF5_EntityExtension);
			if(ef5_type != null)
				return ef5_type;
			return Type.GetType(EF6_EntityExtension);
		}
		public EF5_DataSourceStrategy(IDataSource owner) : base(owner) { }
		public override object CreateData(object value) {
			Type dbExtensionsType = GetLoadMethodExtensionType();
			dbExtensionsType.GetMethod("Load", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[1] { value });
			return value.GetType().GetProperty("Local").GetValue(value, null);
		}
	}
	public abstract class WcfDataSourceStrategyBase : GenericPropertyDataSourceStrategy {
		public WcfDataSourceStrategyBase(IWcfDataSource owner) : base(owner) { }
		private IWcfDataSource Owner { get { return (IWcfDataSource)this.owner; } }
		public override bool CanUpdateData() {
			return base.CanUpdateData() && Owner.ServiceRoot != null;
		}
		public override object CreateContextIstance() {
			return Activator.CreateInstance(Owner.ContextType, new object[] { Owner.ServiceRoot });
		}
	}
	public class WcfDataSourceStrategyStandart : WcfDataSourceStrategyBase {
		public WcfDataSourceStrategyStandart(IWcfDataSource owner) : base(owner) { }
		private IWcfStandartDataSource Owner { get { return (IWcfStandartDataSource)this.owner; } }
	}
	public class WcfServerModeDataSourceStrategy : WcfDataSourceStrategyBase {
		public WcfServerModeDataSourceStrategy(IWcfServerModeDataSource owner) : base(owner) { }
		private IWcfServerModeDataSource Owner { get { return (IWcfServerModeDataSource)this.owner; } }
		public override object CreateData(object value) {
			Owner.DataServiceContext = Owner.ContextInstance;
			Owner.Query = value as IQueryable;
			return Owner.Data;
		}
	}
	#region WCF
	public class WcfCollectionViewSource : CollectionViewDataSourceBase, IWcfStandartDataSource {
		public static readonly DependencyProperty ServiceRootProperty;
		static WcfCollectionViewSource() {
			Type ownerType = typeof(WcfCollectionViewSource);
			ServiceRootProperty = DependencyPropertyManager.Register("ServiceRoot", typeof(Uri), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((WcfCollectionViewSource)d).UpdateData()));
		}
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new WcfDataSourceStrategyStandart(this);
		}
		public Uri ServiceRoot {
			get { return (Uri)GetValue(ServiceRootProperty); }
			set { SetValue(ServiceRootProperty, value); }
		}
	}
	public class WcfSimpleDataSource : SimpleDataSource, IWcfStandartDataSource {
		public static readonly DependencyProperty ServiceRootProperty;
		static WcfSimpleDataSource() {
			Type ownerType = typeof(WcfSimpleDataSource);
			ServiceRootProperty = DependencyPropertyManager.Register("ServiceRoot", typeof(Uri), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((WcfSimpleDataSource)d).UpdateData()));
		}
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new WcfDataSourceStrategyStandart(this);
		}
		public Uri ServiceRoot {
			get { return (Uri)GetValue(ServiceRootProperty); }
			set { SetValue(ServiceRootProperty, value); }
		}
	}
	#endregion
}
