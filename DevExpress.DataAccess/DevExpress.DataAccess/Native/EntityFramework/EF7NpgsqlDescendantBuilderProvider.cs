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

using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF7NpgsqlDescendantBuilderProvider : EF7DescendantBuilderProviderBase {
		DbDescendantBuilder builder;
		protected override string ServiceAssemblyName { get { return Constants.EntityFramework7NpgsqlAssemblyName; } }
		public EF7NpgsqlDescendantBuilderProvider() {
		}
		public EF7NpgsqlDescendantBuilderProvider(DataAccessEntityFrameworkModel model)
			: base(model) {
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			return this.builder ?? (this.builder = new EF7NpgsqlDescendantBuilder(Model, typesCollector, this.serviceAssemblyInfo));
		}
	}
	public class EF7NpgsqlDescendantBuilder : EF7DescendantBuilderBase {
		protected override string DbContextOptionsExtensionsTypeName { get { return "Microsoft.Data.Entity.NpgsqlDbContextOptionsExtensions"; } }
		protected override string UseServerExtensionName { get { return "UseNpgsql"; } }
		public EF7NpgsqlDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo dxAssemblyInfo)
			: base(model, typesCollector, dxAssemblyInfo) {
		}
	}
}
