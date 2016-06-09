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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using DevExpress.Entity.ProjectModel;
using System.Xml;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public abstract class DescendantBuilderProvider{
		public abstract bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider);
		public abstract DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider);		
	}
	public class DefaultDescendantBuilderProvider : DescendantBuilderProvider {
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			return !EntityFrameworkModelBase.IsAtLeastEF6(dbContext);
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			return new DefaultDescendantBuilder(typesCollector);
		}
	}
	public abstract class EF60DbDescendantBuilderProviderBase : DescendantBuilderProvider {
		protected abstract string ExpectedProviderName { get; }
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			EdmxResource resource = EdmxResource.GetEdmxResource(dbDescendant);
			bool containsExpectedProvider = true;
			if(resource != null) {
				containsExpectedProvider = string.IsNullOrEmpty(ExpectedProviderName) ||  resource.GetProviderName() == ExpectedProviderName;
			}
			return containsExpectedProvider && EntityFrameworkModelBase.IsAtLeastEF6(dbContext);
		}
		protected IDXAssemblyInfo GetServicesAssembly(IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider, string servicesAssemblyName) {
			IDXAssemblyInfo assemblyInfo = typesProvider.ActiveProjectTypes.Assemblies.FirstOrDefault(x => x.Name == servicesAssemblyName);
			if (assemblyInfo != null)
				return assemblyInfo;
			if (!dbDescendant.IsSolutionType || dbDescendant.Assembly.IsProjectAssembly)
				return null;
			IProjectTypes projectTypes = typesProvider.GetProjectTypes(dbDescendant.Assembly.AssemblyFullName);
			if(projectTypes == null)
				projectTypes = typesProvider.ActiveProjectTypes;
			return projectTypes.Assemblies.FirstOrDefault(x => x.Name == servicesAssemblyName);
		}
	}   
	public class SqlClientDescendantBuilderProvider : EF60DbDescendantBuilderProviderBase {
		protected override string ExpectedProviderName { get { return "System.Data.SqlClient"; } }		
		public SqlClientDescendantBuilderProvider()
			: this(true, true, "11.0") {
		}
		public SqlClientDescendantBuilderProvider(bool isSqlExpressInstalled, bool isLocalDbInstalled, string localDbVersion) {
			IsSqlExpressInstalled = isSqlExpressInstalled;
			IsLocalDbInstalled = isLocalDbInstalled;
			LocalDbVersion = localDbVersion;
		}
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			if (!IsLocalDbInstalled && !IsSqlExpressInstalled)
				return false;
			return base.Available(dbContext, dbDescendant, typesProvider) && GetServicesAssembly(dbDescendant, typesProvider, Constants.EntityFrameworkSqlClientAssemblyName) != null;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			IDXAssemblyInfo assemblyInfo = GetServicesAssembly(typesCollector.DbDescendantInfo, typesProvider, Constants.EntityFrameworkSqlClientAssemblyName);
			if (IsLocalDbInstalled)
				return new LocalDbDescendantBuilder(typesCollector, assemblyInfo, LocalDbVersion);
			return new SqlExpressDescendantBuilder(typesCollector, assemblyInfo);
		}
		protected bool IsSqlExpressInstalled { get; set; }
		protected bool IsLocalDbInstalled { get; set; }
		protected string LocalDbVersion { get; set; }
	}
	public class SqlCeDescendantBuilderProvider : EF60DbDescendantBuilderProviderBase {
		protected override string ExpectedProviderName { get { return null; } }
		public SqlCeDescendantBuilderProvider()
			: this(false) {
		}
		public SqlCeDescendantBuilderProvider(bool isSqlCE40Installed) {
			IsSqlCE40Installed = isSqlCE40Installed;
		}		
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			return base.Available(dbContext, dbDescendant, typesProvider) && GetServicesAssembly(dbDescendant, typesProvider, Constants.EntityFrameworkSqlCeAssemblyName) != null && IsSqlCE40Installed;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			IDXAssemblyInfo assemblyInfo = GetServicesAssembly(typesCollector.DbDescendantInfo, typesProvider, Constants.EntityFrameworkSqlCeAssemblyName);
			return new SqlCeDescendantBuilder(typesCollector, assemblyInfo);
		}
		bool IsSqlCE40Installed { get; set; }
	}
}
