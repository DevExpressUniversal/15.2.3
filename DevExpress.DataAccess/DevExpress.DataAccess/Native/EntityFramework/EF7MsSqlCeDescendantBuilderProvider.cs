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
using System.Reflection;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF7MsSqlCeDescendantBuilderProvider : EF7DescendantBuilderProviderBase {
		DbDescendantBuilder builder;
		protected override string ServiceAssemblyName { get { return Constants.EntityFramework7MsSqlCeAssemblyName; } }
		public EF7MsSqlCeDescendantBuilderProvider() {
		}
		public EF7MsSqlCeDescendantBuilderProvider(DataAccessEntityFrameworkModel model)
			: base(model) {
		}
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			if(!EntityFrameworkModelBase.IsAtLeastEF7(dbContext))
				return false;
			AssemblyName assemblyName = dbDescendant.ResolveType().Assembly.GetReferencedAssemblies().FirstOrDefault(n => n.Name.StartsWith(ServiceAssemblyName));
			if(assemblyName == null)
				return false;
			this.serviceAssemblyInfo = typesProvider.GetAssembly(assemblyName.Name);
			return this.serviceAssemblyInfo != null;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			return this.builder ?? (this.builder = new EF7MsSqlCEDescendantBuilder(Model, typesCollector, this.serviceAssemblyInfo));
		}
	}
	public class EF7MsSqlCEDescendantBuilder : EF7DescendantBuilderBase {
		protected override string DbContextOptionsExtensionsTypeName { get { return "Microsoft.Data.Entity.SqlCeDbContextOptionsExtensions"; } }
		protected override string UseServerExtensionName { get { return "UseSqlCe"; } }
		public EF7MsSqlCEDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo dxAssemblyInfo)
			: base(model, typesCollector, dxAssemblyInfo) {
		}
	}
}
