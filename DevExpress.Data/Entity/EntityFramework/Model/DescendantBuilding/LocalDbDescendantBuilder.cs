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
namespace DevExpress.Entity.Model.DescendantBuilding {
	public class LocalDbDescendantBuilder : SqlExpressDescendantBuilder {
		public LocalDbDescendantBuilder(TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly, string dbVersion)
			: base(typesCollector, servicesAssembly) {
			LocalDbVersion = dbVersion;
		}
		public string LocalDbVersion { get; set; }
		object CreateLocalDbConnection(Type dbContextType, TypesCollector typesCollector) {
			Type factoryType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.LocalDbConnectionFactory");
			ConstructorInfo ctor = factoryType.GetConstructor(new Type[] { typeof(string) });
			object factory = ctor.Invoke(new object[] { LocalDbVersion });
			string dbFilePath = Path.Combine(TempFolder, Constants.DatabaseFileName);
			MethodInfo createConnection = factoryType.GetMethod("CreateConnection", BindingFlags.Public | BindingFlags.Instance);
			return createConnection.Invoke(factory, new object[] { GetConnectionString(dbFilePath) });
		}
		protected override string GetConnectionString(string dbFilePath) {
			return String.Format("Server=(localdb)\\{0};Integrated Security=SSPI;AttachDbFileName={1};", LocalDbVersion, dbFilePath);
		}
	}
}
