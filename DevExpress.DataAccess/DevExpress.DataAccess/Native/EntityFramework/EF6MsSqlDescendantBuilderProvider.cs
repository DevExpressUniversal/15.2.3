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
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF6MsSqlDescendantBuilderProvider : DataAccessDescendantBuilderProvider {
		protected override string ExpectedProviderName { get { return "System.Data.SqlClient"; } }		
		DbDescendantBuilder builder;
		readonly DataAccessEntityFrameworkModel model;
		public EF6MsSqlDescendantBuilderProvider() {
		}
		public EF6MsSqlDescendantBuilderProvider(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			IDXAssemblyInfo assemblyInfo = typesProvider.GetAssembly(Constants.EntityFrameworkSqlClientAssemblyName);
			if(assemblyInfo == null) {
				this.model.Exceptions.Add(new ApplicationException(string.Format("Cannot load the specified assembly: \"{0}\".", Constants.EntityFrameworkSqlClientAssemblyName)));
			}
			this.builder = new RuntimeEF6MsSqlDescendantBuilder(this.model, typesCollector, assemblyInfo);
			return this.builder;
		}
	}
	public class RuntimeEF6MsSqlDescendantBuilder : RuntimeDescendantBuilder {
		public override string ProviderName {
			get { return "System.Data.SqlClient"; }
		}
		public override string SqlProviderServicesTypeName {
			get { return "System.Data.Entity.SqlServer.SqlProviderServices"; }
		}
		public override string ProviderManifestToken {
			get { return "2008"; } 
		}
		public RuntimeEF6MsSqlDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo dxAssemblyInfo)
			: base(model, typesCollector, dxAssemblyInfo) {		   
		}		 
	}
}
