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
using System.Reflection.Emit;
using System.Collections.Generic;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public class SqlExpressDescendantBuilder : EF60DbDescendantBuilderBase {
		protected override bool SupressExceptions { get { return true; } }
		public SqlExpressDescendantBuilder(TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly)
			: base(typesCollector, servicesAssembly) {
		}
		public override string ProviderName {
			get { return "System.Data.SqlClient"; }
		}
		public override string SqlProviderServicesTypeName{
			get { return "System.Data.Entity.SqlServer.SqlProviderServices";  }
		}
		public override string ProviderManifestToken {
			get { return "2008"; } 
		}
		protected override string GetConnectionString(string dbFilePath) {
			Guid name = Guid.NewGuid();
			bool useUserInstance = Internal.SqlExpressDescendantBuilderConfig.UseUserInstance;
			return string.Format(@"Server=.\SQLEXPRESS;AttachDbFileName={0};Database={1};{2}Integrated Security=SSPI;MultipleActiveResultSets=True;Application Name=EntityFrameworkMUE", dbFilePath, name, (useUserInstance ? "User Instance=true;" : string .Empty));
		}
		protected override object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector) {
			Type sqlConnectionFactoryType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.SqlConnectionFactory");
			ConstructorInfo ctor = sqlConnectionFactoryType.GetConstructor(new Type[0]);
			object factory = ctor.Invoke(new object[0]);
			MethodInfo createConnection = sqlConnectionFactoryType.GetMethod("CreateConnection", BindingFlags.Public | BindingFlags.Instance);
			return createConnection.Invoke(factory, new object[] { GetConnectionString(Path.Combine(TempFolder, Constants.DatabaseFileName)) });
		}
	}
}
namespace DevExpress.Entity.Model.DescendantBuilding.Internal {
	public class SqlExpressDescendantBuilderConfig {
		public static bool UseUserInstance { get; set; }
	}
}
