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
using DevExpress.DataAccess.Localization;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class DataAccessEntityFrameworkModel : EntityFrameworkModelBase, IDisposable {
		readonly ISolutionTypesProvider solutionTypesProvider;
		readonly List<Exception> exceptionLog = new List<Exception>();
		public bool IsEntityFramework7 { get; set; }
		public List<Exception> Exceptions { get { return this.exceptionLog; } }
		public DataAccessEntityFrameworkModel(ISolutionTypesProvider solutionTypesProvider) {
			this.solutionTypesProvider = solutionTypesProvider;
		}
		protected override ContainerBuilder GetContainerBuilderCore(DbContainerType dbContainerType) {
			if (dbContainerType == DbContainerType.EntityFramework)
				return new DataAccessEFContainerBuilder(this);
			return base.GetContainerBuilderCore(dbContainerType);
		}
		protected override ISolutionTypesProvider TypesProvider { get { return this.solutionTypesProvider; } }
		public object ContextInstance { get; set; }
		public string ConnectionString { get; set; }
		public void Dispose() {
			IDisposable contextDisposable = ContextInstance as IDisposable;
			if(contextDisposable != null) {
				contextDisposable.Dispose();
			}
		}
	}
	public class DataAccessEFContainerBuilder : EFContainerBuilderBase {
		readonly DataAccessEntityFrameworkModel model;
		public DataAccessEFContainerBuilder(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override IDbContainerInfo Build(IDXTypeInfo type, ISolutionTypesProvider typesProvider) {
			try {
				return base.Build(type, typesProvider);
			} catch(ProviderNotSupportedException notSupportedException) {
				throw new NotSupportedException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.ProviderNotSupportedException), notSupportedException.ProviderName));
			}
		}
		protected override IDbContainerInfo BuildCore(IDXTypeInfo type, DbDescendantBuilder descendantBuilder) {
			IDbContainerInfo result = base.BuildCore(type, descendantBuilder);
			if(result != null)
				return result;
			object dbContextInstance = descendantBuilder.Build();
			if(dbContextInstance == null)
				return null;
			IEntityContainerInfo dbSets = GetDbSets(descendantBuilder.TypesCollector, dbContextInstance);
			if(dbSets == null)
				return null;
			return new DbContainerInfo(type.ResolveType(), dbSets);
		}
		protected override DescendantBuilderFactoryBase CreateBuilderProviderFactory() {
			return new DataAccessDescendantBuilderFactory(this.model);
		}
		protected override void LogException(Exception ex, bool display) {
			this.model.Exceptions.Add(ex);
		}
		protected override void CreateSampleQuery(object objectContext) {
		}
		IEntityContainerInfo GetDbSets(TypesCollector typesCollector, object dbContextInstance) {
			try {
				if(typesCollector.DbSetFinder != null) {
					Type dbSetFinderType = typesCollector.DbSetFinder.ResolveType();
					object dbSetFinder = Activator.CreateInstance(dbSetFinderType);
					return new EF7ContainerInfo(dbSetFinder, dbContextInstance);
				}
			} catch(Exception ex) {
				if(ex is TargetInvocationException && ex.InnerException != null)
					LogException(ex.InnerException, true);
				else
					LogException(ex, true);
			}
			return null;
		}
	}
	public class EF7ContainerInfo : RuntimeWrapper, IEntityContainerInfo {
		readonly string name;
		readonly object dbContextInstance;
		List<IEntitySetInfo> entitySets;
		readonly IEnumerable<IEntityFunctionInfo> entityFunctions = Enumerable.Empty<IEntityFunctionInfo>();
		public string Name { get { return this.name; } }
		public IEnumerable<IEntitySetInfo> EntitySets {
			get {
				if(this.entitySets == null)
					InitEntitySets();
				return this.entitySets;
			}
		}
		public IEnumerable<IEntityFunctionInfo> EntityFunctions { get { return this.entityFunctions; } }
		public EF7ContainerInfo(object finder, object dbContextInstance)
			: base("Microsoft.Data.Entity.Internal.DbSetFinder", finder) {
				name = dbContextInstance.GetType().Name;
			this.dbContextInstance = dbContextInstance;
		}
		void InitEntitySets() {
			MethodAccessor findSets = GetMethodAccessor("FindSets");
			IEnumerable sets = findSets.Invoke(Value, () => new[] {this.dbContextInstance}) as IEnumerable;
			if(sets == null)
				return;
			entitySets = new List<IEntitySetInfo>();
			foreach(object set in sets)
				entitySets.Add(new DbSetPropertyInfo(set, this));
		}
	}
	public class DbSetPropertyInfo : RuntimeWrapper, IEntitySetInfo {
		string name;
		readonly EF7ContainerInfo entityContainerInfo;
		public DbSetPropertyInfo(object dbSetProperty, EF7ContainerInfo entityContainerInfo)
			: base("Microsoft.Data.Entity.Internal.DbSetProperty", dbSetProperty) {
			this.entityContainerInfo = entityContainerInfo;
		}
		public IEntityTypeInfo ElementType {
			get {
				Type type = (Type)GetPropertyAccessor("EntityType").Value;
				return new EF7TypeInfo(type);
			}
		}
		public bool IsView { get { return (bool)GetPropertyAccessor("HasSetter").Value; } }
		public bool ReadOnly { get { return (bool)GetPropertyAccessor("HasSetter").Value; } }
		public string Name { get { return this.name ?? (this.name = (string)GetPropertyAccessor("Name").Value); } }
		public IEntityContainerInfo EntityContainerInfo { get { return this.entityContainerInfo; } }
		public IEntitySetAttachedInfo AttachedInfo { get { return null; } }
	}
	public class EF7TypeInfo : IEntityTypeInfo {
		readonly Type type;
		List<IEdmPropertyInfo> allProperties;
		public IEnumerable<IEdmPropertyInfo> AllProperties {
			get {
				if(this.allProperties == null)
					InitProperties();
				return this.allProperties;
			}
		}
		public IEnumerable<IEdmPropertyInfo> KeyMembers { get { return null; } }
		public IEnumerable<IEdmAssociationPropertyInfo> LookupTables { get { return null; } }
		public Type Type { get { return this.type; } }
		void InitProperties() {
			PropertyInfo[] properties = this.type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			this.allProperties = new List<IEdmPropertyInfo>(properties.Select(p => new EF7PropertyInfo(p)));
		}
		public IEdmPropertyInfo GetForeignKey(IEdmPropertyInfo dependentProperty) {
			return null;
		}
		public IEdmPropertyInfo GetDependentProperty(IEdmPropertyInfo foreignKey) {
			return null;
		}
		public EF7TypeInfo(Type type) {
			this.type = type;
		}
	}
	public class EF7PropertyInfo : IEdmPropertyInfo {
		readonly PropertyInfo property;
		public EF7PropertyInfo(PropertyInfo property) {
			this.property = property;
		}
		public string Name { get { return property.Name; } }
		public string DisplayName { get { return property.Name; } }
		public Type PropertyType { get { return property.PropertyType; } }
		public bool IsForeignKey { get { throw new NotSupportedException(); } }
		public bool IsReadOnly { get { throw new NotSupportedException(); } }
		public DataColumnAttributes Attributes { get { throw new NotSupportedException(); } }
		public object ContextObject { get { throw new NotSupportedException(); } }
		public bool IsNavigationProperty { get { throw new NotSupportedException(); } }
		public IEdmPropertyInfo AddAttributes(IEnumerable<Attribute> newAttributes) {
			throw new NotSupportedException();
		}
	}
}
