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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public abstract class DataAccessDescendantBuilderProvider : EF60DbDescendantBuilderProviderBase {
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			EdmxResource resource = EdmxResource.GetEdmxResource(dbDescendant);
			bool containsExpectedProvider = false;
			if(resource != null) {
				containsExpectedProvider = string.IsNullOrEmpty(ExpectedProviderName) || resource.GetProviderName() == ExpectedProviderName;
			}
			return containsExpectedProvider && EntityFrameworkModelBase.IsAtLeastEF6(dbContext);
		}
	}
	public class RuntimeDefaultDescendantBuilderProvider : EF60DbDescendantBuilderProviderBase {
		protected override string ExpectedProviderName { get { return ""; } }
		DbDescendantBuilder builder;
		readonly DataAccessEntityFrameworkModel model;
		public RuntimeDefaultDescendantBuilderProvider() {
		}
		public RuntimeDefaultDescendantBuilderProvider(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			return true;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			if(this.builder == null)
				this.builder = new RuntimeDefaultDescendantBuilder(this.model, typesCollector);
			return this.builder;
		}
	}
	public class RuntimeDefaultDescendantBuilder : RuntimeDescendantBuilder {
		public override string ProviderName {
			get { return ""; }
		}
		public override string SqlProviderServicesTypeName {
			get { return ""; }
		}
		public override string ProviderManifestToken {
			get { return ""; } 
		}
		public RuntimeDefaultDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector)
			: base(model, typesCollector, null) {
		}
		public override object Build() {
			IDXTypeInfo typeInfo = TypesCollector.DbDescendantInfo;
			if(typeInfo == null)
				return null;
			if(!CheckDefaultConstructors(typeInfo)) {
				DescendantInstance = null;
				EdmxResource edmxResource = EdmxResource.GetEdmxResource(typeInfo);
				bool isModelFirst = edmxResource != null;
				try {
					ModuleBuilder mb = CreateDynamicAssembly();
					Tuple<ConstructorInfo, Type[]> ctorTuple = GetDbContextConstructor(Model.ConnectionString, this.dbContext);
					if(ctorTuple == null)
						return null;
					Type resultType = EmitDbDescendant(TypesCollector, ctorTuple, mb, isModelFirst, null);
					DescendantInstance = Activator.CreateInstance(resultType, Model.ConnectionString);
				} catch(TargetInvocationException tiex) {
					if(SupressExceptions)
						return null;
					if(tiex.InnerException != null)
						throw tiex.InnerException;
					throw;
				} catch(Exception) {
					if(SupressExceptions)
						return null;
					throw;
				}
			}		 
			Model.ContextInstance = DescendantInstance;
			return DescendantInstance;
		}
	}
}
