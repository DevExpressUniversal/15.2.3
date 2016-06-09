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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.Model.Metadata;
namespace DevExpress.Entity.Model {
	public abstract class ContainerBuilder {
		EntityTypeInfoFactory entityTypeInfoFactory;
		protected virtual DbContainerInfo GetDbContainerInfo(IDXTypeInfo type, MetadataWorkspaceInfo mw, IMapper mapper) {
			if(mw == null)
				return null;
			object container = mw.GetItems(DataSpace.CSpace).FirstOrDefault(x => RuntimeWrapper.ConvertEnum<BuiltInTypeKind>(PropertyAccessor.GetValue(x, "BuiltInTypeKind")) == BuiltInTypeKind.EntityContainer);
			if(container == null)
				return null;
			IDataColumnAttributesProvider dataColumnAttributesProvider = CreateDataColumnAttributesProvider();
			EntityContainerInfo result = new EntityContainerInfo(container, mapper, dataColumnAttributesProvider, EntityTypeInfoFactory);
			return CreateDbContainerInfo(type, result, mw);
		}
		protected virtual DbContainerInfo CreateDbContainerInfo(IDXTypeInfo type, EntityContainerInfo result, MetadataWorkspaceInfo mw) {
			return new DbContainerInfo(type.ResolveType(), result, mw) { Assembly = type.Assembly };
		}
		public abstract IDbContainerInfo Build(IDXTypeInfo info, ISolutionTypesProvider typesProvider);
		protected virtual IDataColumnAttributesProvider CreateDataColumnAttributesProvider() {
				return new EmptyDataColumnAttributesProvider();
			}
		public abstract DbContainerType BuilderType { get; }
		EntityTypeInfoFactory EntityTypeInfoFactory{
			get{
				if(this.entityTypeInfoFactory == null)
					entityTypeInfoFactory = CreateEntityTypeInfoFactory();
				return entityTypeInfoFactory;
			}
		}
		protected virtual EntityTypeInfoFactory CreateEntityTypeInfoFactory() {
			return new EntityTypeInfoFactory();
		}
	}
	public class ProviderNotSupportedException : Exception {
		public string ProviderName { get; set; }
		public ProviderNotSupportedException(string providerName) {
			ProviderName = providerName;
		}
	}
	public class EFContainerBuilderBase : ContainerBuilder {
		public override DbContainerType BuilderType {
			get { return DbContainerType.EntityFramework; }
		}
		object GetObjectContext(TypesCollector typesCollector, object dbContextInstance) {
			object objectContext = null;
			try {
				if(typesCollector.IObjectContextAdapter == null)
					return null;
				Type iObjectContextAdapterType = typesCollector.IObjectContextAdapter.ResolveType();
				if(iObjectContextAdapterType == null)
					return null;
				PropertyInfo pi = iObjectContextAdapterType.GetProperty("ObjectContext", BindingFlags.Instance | BindingFlags.Public);
				if(pi == null)
					return null;
				objectContext = pi.GetValue(dbContextInstance, null);
			} catch(Exception ex) {
				if(ex is TargetInvocationException && ex.InnerException != null)
					LogException(ex.InnerException, true);
				else
					LogException(ex, true);
			}
			return objectContext;
		}
		DescendantBuilderFactoryBase descendantBuilderFactory;
		protected DescendantBuilderFactoryBase DescendantBuilderFactory {
			get {
				if (descendantBuilderFactory == null) {
					descendantBuilderFactory = CreateBuilderProviderFactory();
					descendantBuilderFactory.Initialize();
				}
				return descendantBuilderFactory;
			}
		}
		public override IDbContainerInfo Build(IDXTypeInfo type, ISolutionTypesProvider typesProvider) {
			try {
				DbDescendantBuilder descendantBuilder = this.DescendantBuilderFactory.GetDbDescendantBuilder(type, typesProvider);
				if(descendantBuilder == null) {
					EdmxResource resource = EdmxResource.GetEdmxResource(type);
					string providerName = "";
					if(resource != null)
						providerName = resource.GetProviderName();
					throw new ProviderNotSupportedException(providerName);
				}
				try {
					return BuildCore(type, descendantBuilder);
				} finally {
					descendantBuilder.Dispose();
				}
			} catch(TargetInvocationException tiex) {
				LogException(tiex.InnerException != null ? tiex.InnerException : tiex, false);
				return null;
			} catch(Exception ex) {
				LogException(ex, false);
				return null;
			}
		}
		protected virtual IDbContainerInfo BuildCore(IDXTypeInfo type, DbDescendantBuilder descendantBuilder) {
			object dbContextInstance = descendantBuilder.Build();
			if(dbContextInstance == null)
				return null;
			object objectContext = GetObjectContext(descendantBuilder.TypesCollector, dbContextInstance);
			if(objectContext == null)
				return null;
			CreateSampleQuery(objectContext);
			MetadataWorkspaceInfo mwInfo = CreateMetadataWorkspaceInfo(objectContext);
			return GetDbContainerInfo(type, mwInfo, GetMapper(descendantBuilder.TypesCollector, mwInfo));
		}
		protected internal virtual Mapper GetMapper(TypesCollector typesCollector, MetadataWorkspaceInfo mwInfo) {
			return new Mapper(mwInfo, typesCollector);
		}
		void DeleteTempFolder(string directoryPath) {
			try {
				if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
					Directory.Delete(directoryPath, true);
			}
			catch { }
		}
		void DeleteDatabase(object dbContextInstance) {
			try {		
				object database = PropertyAccessor.GetValue(dbContextInstance, "Database");
				if (database == null)
					return;
				MethodInfo delete = database.GetType().GetMethod("Delete", new Type[0]);
				if (delete != null)
					delete.Invoke(database, null);
			}
			catch { }
		}
		MetadataWorkspaceInfo CreateMetadataWorkspaceInfo(object objectContext) {
			object mw = PropertyAccessor.GetValue(objectContext, "MetadataWorkspace");
			if (mw == null)
				return null;
			return new MetadataWorkspaceInfo(mw);
		}
		protected virtual void CreateSampleQuery(object objectContext) {
			try {
				MethodInfo createQuery = objectContext.GetType().GetMethod("CreateQuery");
				if (createQuery == null)
					return;
				createQuery = createQuery.MakeGenericMethod(typeof(object));
				if (createQuery == null)
					return;
				ParameterInfo[] parameters = createQuery.GetParameters();
				if (parameters == null || parameters.Length < 2)
					return;
				object p = Activator.CreateInstance(parameters[1].ParameterType, new object[] { (int)0 });
				object query = createQuery.Invoke(objectContext, new object[] { "x", p });
				if (query == null)
					return;
				MethodInfo toTraceString = query.GetType().GetMethod("ToTraceString");
				if (toTraceString != null)
					toTraceString.Invoke(query, null);
			}
			catch { }
		}
		protected virtual void LogException(Exception ex, bool display ) {
		}
		protected virtual DescendantBuilderFactoryBase CreateBuilderProviderFactory() {
			return new DescendantBuilderFactoryBase();
		}
	}
}
