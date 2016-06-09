#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DataAccess.UI.Wizard;
using System;
using System.ComponentModel;
namespace DevExpress.DashboardWin {
	public class DashboardDataSourceWizardSettings : SqlWizardSettings {
		internal static readonly DashboardDataSourceWizardSettings DefaultRuntime;
		internal static readonly DashboardDataSourceWizardSettings DesignTime;
		static DashboardDataSourceWizardSettings() {
			DefaultRuntime = new DashboardDataSourceWizardSettings {
				AvailableDataSourceTypes = DashboardDesignerDataSourceType.Default,
				AvailableSqlDataProviders = DashboardSqlDataProvider.All
			};
			DesignTime = new DashboardDataSourceWizardSettings {
				AvailableDataSourceTypes = DashboardDesignerDataSourceType.All,
				AvailableSqlDataProviders = DashboardSqlDataProvider.All,
				ShowConnectionsFromAppConfig = true,
				EnableCustomSql = true
			};
		}
		[
		DefaultValue(false)
		]
		public bool ShowDataSourceNamePage { get; set; }
		[
		DefaultValue(DashboardDesignerDataSourceType.Default)
		]
		public DashboardDesignerDataSourceType AvailableDataSourceTypes { get; set; }
		[
		DefaultValue(false)
		]
		public bool ShowConnectionsFromAppConfig { get; set; }
		[
		DefaultValue(DashboardSqlDataProvider.All)
		]
		public DashboardSqlDataProvider AvailableSqlDataProviders { get; set; }
		public override void Reset() {
			base.Reset();
			ShowDataSourceNamePage = false;
			AvailableDataSourceTypes = DashboardDesignerDataSourceType.Default;
			ShowConnectionsFromAppConfig = false;
			AvailableSqlDataProviders = DashboardSqlDataProvider.All;
		}
		public override bool Equals(object obj) {
			var other = obj as DashboardDataSourceWizardSettings;
			if (other == null)
				return false;
			return ShowDataSourceNamePage == other.ShowDataSourceNamePage && AvailableDataSourceTypes == other.AvailableDataSourceTypes &&
				ShowConnectionsFromAppConfig == other.ShowConnectionsFromAppConfig && AvailableSqlDataProviders == other.AvailableSqlDataProviders;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[Flags]
	public enum DashboardDesignerDataSourceType { Sql = 1, Olap = 2, Object = 4, EF = 8, Excel = 16, Default = Sql | Excel | Olap, All = Default | Object | EF };
	[Flags]
	public enum DashboardSqlDataProvider {
		CustomConnectionString = 0x00000001,
		MSSqlServer = 0x00000002,
		MSSqlServerCE = 0x20000,
		Access = 0x00000004,
		Oracle = 0x00000008,
		Redshift = 0x00000010,
		BigQuery = 0x00000020,
		Teradata = 0x00000040,
		Firebird = 0x00000080,
		DB2 = 0x0000000100,
		MySql = 0x0000000200,
		Pervasive = 0x0000000400,
		Postgres = 0x0000000800,
		Advantage = 0x0000001000,
		Ase = 0x0000002000,
		Asa = 0x0000004000,
		SQLite = 0x0000008000,
		VistaDB = 0x0000010000,
		XmlFile = 0x0000020000,
		All = CustomConnectionString | MSSqlServer | MSSqlServerCE | Access | Oracle | Redshift | BigQuery |
		Teradata | Firebird | DB2 | MySql | Pervasive | Postgres | Advantage | Ase | Asa | SQLite | VistaDB | XmlFile
	};
}
