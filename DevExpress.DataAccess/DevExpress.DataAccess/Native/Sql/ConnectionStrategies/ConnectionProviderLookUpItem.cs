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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.DataAccess.Localization;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class ProviderLookupItem {
		public static ReadOnlyCollection<ProviderLookupItem> GetPredefinedItems() { return GetPredefinedItems(DataAccessLocalizer.GetString(DataAccessStringId.WizardCustomConnectionString)); }
		public static ReadOnlyCollection<ProviderLookupItem> GetPredefinedItems(string customConnectionProviderName) {
			return new List<ProviderLookupItem>() {
				new ProviderLookupItem("MSSqlServer", "Microsoft SQL Server", new MsSqlServerStrategy()),
				new ProviderLookupItem("Access97", "Microsoft Access 97", new Access97Strategy()),
				new ProviderLookupItem("Access2007", "Microsoft Access 2007", new Access2007Strategy()),
				new ProviderLookupItem("MSSqlServerCE", "Microsoft SQL Server CE", new MsSqlServerCeStrategy()),
				new ProviderLookupItem("Oracle", "Oracle", new OracleStrategy()),
				new ProviderLookupItem("Amazon Redshift", "Amazon Redshift", new AmazonRedshiftStrategy()),
				new ProviderLookupItem("BigQuery", "Google BigQuery", new BigQueryStrategy()),
				new ProviderLookupItem("Teradata", "Teradata", new TeradataStrategy()),
				new ProviderLookupItem("Firebird", "Firebird", new FirebirdStrategy()),
				new ProviderLookupItem("DB2", "IBM DB2", new Db2Strategy()),
				new ProviderLookupItem("MySql", "MySQL", new MySqlStrategy()),
				new ProviderLookupItem("Pervasive", "Pervasive PSQL", new PervasiveStrategy()),
				new ProviderLookupItem("Postgres", "PostgreSQL", new PostgreStrategy()),
				new ProviderLookupItem("Advantage", "SAP Sybase Advantage", new AdvantageStrategy()),
				new ProviderLookupItem("Ase", "SAP Sybase ASE", new AseStrategy()),
				new ProviderLookupItem("Asa", "SAP Sybase SQL Anywhere", new AsaStrategy()),
				new ProviderLookupItem("SQLite", "SQLite", new SQLiteStrategy()),
				new ProviderLookupItem("VistaDB", "VistaDB", new VistaDbStrategy()),
				new ProviderLookupItem("VistaDB5", "VistaDB5", new VistaDb5Strategy()),
				new ProviderLookupItem("InMemorySetFull", "XML file", new XmlFileStrategy()),
				new ProviderLookupItem(DataConnectionParametersRepository.CustomConStrTag, customConnectionProviderName, new CustomConnectionStrategy())
			}.AsReadOnly();
		}
		public ProviderLookupItem(string providerKey, string providerName, IConnectionParametersStrategy strategy) {
			ProviderKey = providerKey;
			ProviderName = providerName;
			Strategy = strategy;
		}
		public string ProviderKey { get; private set; }
		public string ProviderName { get; private set; }
		public IConnectionParametersStrategy Strategy { get; private set; }
		#region Overrides of Object
		public override string ToString() { return ProviderName; }
		#endregion
	}
}
