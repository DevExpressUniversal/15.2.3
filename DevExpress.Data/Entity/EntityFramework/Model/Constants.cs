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
namespace DevExpress.Entity.Model {
	public sealed class Constants {
	   static Dictionary<string, string> typeNameToKeyword;
		private Constants() {
		}
		public static string TypeNameToKeyword(string typeName) {
			if (String.IsNullOrEmpty(typeName))
				return typeName;
			InitTypeNames();
			if (!typeNameToKeyword.ContainsKey(typeName))
				return typeName;
			return typeNameToKeyword[typeName];
		}
		static void InitTypeNames() {
			if (typeNameToKeyword != null)
				return;
			typeNameToKeyword = new Dictionary<string, string>();
			typeNameToKeyword["Boolean"] = "bool";
			typeNameToKeyword["Byte"] = "byte";
			typeNameToKeyword["SByte"] = "sbyte";
			typeNameToKeyword["Char"] = "char";
			typeNameToKeyword["Decimal"] = "decimal";
			typeNameToKeyword["Double"] = "double";
			typeNameToKeyword["Single"] = "float";
			typeNameToKeyword["Int32"] = "int";
			typeNameToKeyword["UInt32"] = "uint";
			typeNameToKeyword["Int64"] = "long";
			typeNameToKeyword["UInt64"] = "ulong";
			typeNameToKeyword["Object"] = "object";
			typeNameToKeyword["Int16"] = "short";
			typeNameToKeyword["UInt16"] = "ushort";
			typeNameToKeyword["String"] = "string";
			typeNameToKeyword["Void"] = "void";
		}
		public const string ServicesClientAssemblyName = "Microsoft.Data.Services.Client";
		public const string ServiceContextTypeName = "System.Data.Services.Client.DataServiceContext";
		public const string DbContextTypeName = "System.Data.Entity.DbContext";
		public const string DbModelBuilderTypeName = "System.Data.Entity.DbModelBuilder";
		public const string MetadataHelperTypeName = "System.Data.Common.Utils.MetadataHelper";
		public const string DbSetTypeName = "System.Data.Entity.DbSet`1";
		public const string DbConnectionTypeName = "System.Data.Common.DbConnection";
		public const string IObjectContextAdapterTypeName = "System.Data.Entity.Infrastructure.IObjectContextAdapter";
		public const string SqlCeConnectionTypeName = "System.Data.SqlServerCe.SqlCeConnection";
		public const string EntityStoreSchemaGeneratorTypeAttributeName = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Type";
		public const string EntityStoreSchemaGeneratorTypeAttributeValueIsViews = "Views";
		public const string DatabaseFileName = "db.sdf";
		public const string EntityFrameworkAssemblyName = "EntityFramework";
		public const string SystemDataEntityAssemblyName = "System.Data.Entity";
		public const string Sql35ProviderName = "System.Data.SqlServerCe.3.5";
		public const string Sql40ProviderName = "System.Data.SqlServerCe.4.0";
		public const string CE35RegistryKey = @"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v3.5";
		public const string CE40RegistryKey = @"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v4.0";
		public const string SystemDataSqlCe350AssemblyFullName = @"System.Data.SqlServerCe, Version=3.5.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
		public const string SystemDataSqlCe351AssemblyFullName = @"System.Data.SqlServerCe, Version=3.5.1.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
		public const string SystemDataSqlCe40AssemblyFullName = @"System.Data.SqlServerCe, Version=4.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
		public const string EntityFrameworkMySqlClientAssemblyName = "MySql.Data.Entity.EF6";
		public const string EntityFrameworkSqliteAssemblyName = "System.Data.SQLite.EF6";
		public const string SqliteAssemblyName = "System.Data.SQLite";
		public const string EntityFrameworkSqlClientAssemblyName = "EntityFramework.SqlServer";
		public const string EntityFrameworkSqlCeAssemblyName = "EntityFramework.SqlServerCompact";
		public const string DbContextEF7TypeName = "Microsoft.Data.Entity.DbContext";
		public const string EntityFramework7MsSqlAssemblyName = "EntityFramework.MicrosoftSqlServer";
		public const string EntityFramework7MsSqlCeAssemblyName = "EntityFramework.SqlServerCompact";
		public const string EntityFramework7SqliteAssemblyName = "EntityFramework.Sqlite";
		public const string EntityFramework7NpgsqlAssemblyName = "EntityFramework7.Npgsql";
	}
}
