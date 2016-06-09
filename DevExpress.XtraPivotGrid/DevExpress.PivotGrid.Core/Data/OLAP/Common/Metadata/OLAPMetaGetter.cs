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
using System.Collections.Generic;
using System.Data;
using DevExpress.PivotGrid.OLAP;
using Microsoft.Win32;
using DevExpress.Utils;
namespace DevExpress.XtraPivotGrid.Data {
#if DXPORTABLE
	public class OLAPMetaGetter : IOLAPMetaGetter {
		public bool Connected { get; set; }
		public string ConnectionString { get; set; }
		public void Dispose() {
		}
		public List<string> GetCatalogs() {
			return new List<string>();
		}
		public List<string> GetCubes(string catalogName) {
			return new List<string>();
		}
		public static List<string> GetProviders() {
			return new List<string>();
		}
	}
#else
	using System.Data.OleDb;
	public class OLAPMetaGetter : IOLAPMetaGetter {
		OleDbConnection olapConnection;
		protected OleDbConnection OlapConnection { get { return olapConnection; } }
		string connectionString;
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString != value || !Connected) {
					connectionString = value;
					Connect();
				}
			}
		}
		public bool Connected {
			get { return olapConnection != null; }
			set {
				if(Connected == value) return;
				if(value) Connect();
				else Disconnect();
			}
		}
		DataTable GetSchema(Guid guid, object[] restrictions) {
			if(!Connected) return null;
			return OlapConnection.GetOleDbSchemaTable(guid, restrictions);
		}
		public List<string> GetCatalogs() {
			if(!Connected) return null;
			DataTable catalogsTable = GetSchema(OLAPSchemaGuid.Catalogs, null);
			if(catalogsTable == null) return null;
			List<string> catalogs = new List<string>(catalogsTable.Rows.Count);
			for(int i = 0; i < catalogsTable.Rows.Count; i++)
				catalogs.Add((string)catalogsTable.Rows[i]["CATALOG_NAME"]);
			return catalogs;
		}
		public List<string> GetCubes(string catalogName) {
			if(!Connected || string.IsNullOrEmpty(catalogName)) return null;
			OlapConnection.ChangeDatabase(catalogName);
			DataTable cubesTable = GetSchema(OLAPSchemaGuid.Cubes, new object[] { catalogName });
			if(cubesTable == null) return null;
			List<string> cubes = new List<string>(cubesTable.Rows.Count);
			for(int i = 0; i < cubesTable.Rows.Count; i++)
				cubes.Add((string)cubesTable.Rows[i]["CUBE_NAME"]);
			return cubes;
		}
		public static List<string> GetProviders() {
			List<string> result = new List<string>();
			OleDbDataReader enumerator = OleDbEnumerator.GetRootEnumerator();
			while(enumerator.Read()) {
				if(((string)enumerator.GetValue(0)).ToLowerInvariant().StartsWith("msolap")) {
					RegistryKey progId = Registry.ClassesRoot.OpenSubKey("CLSID\\" + enumerator.GetValue(5).ToString() + "\\ProgID", false);
					if(progId != null) {
						string provider = progId.GetValue("") as string;
						if(!string.IsNullOrEmpty(provider) && !result.Contains(provider))
							result.Add(provider);
					}
				}
			}
			if(result.Count > 0 && !result.Contains("MSOLAP"))
				result.Insert(0, "MSOLAP");
			return result;
		}
		public static bool IsProviderAvailable { get { return GetProviders().Count > 0; } }
		string connectionException;
		public string ConnectionException { get { return connectionException; } }
		void Connect() {
			if(Connected) Disconnect();
			if(ConnectionString == null) return;
			try {
				olapConnection = new OleDbConnection(RemoveCubeName(ConnectionString));
				olapConnection.Open();
				if(olapConnection.State != ConnectionState.Open)
					Disconnect();
				connectionException = "";
			} catch(Exception ex) {
				connectionException = ex.Message;
				Disconnect();
			}
		}
		void Disconnect() {
			if(!Connected) return;
			olapConnection.Dispose();
			olapConnection = null;
		}
		string RemoveCubeName(string str) {
			if(str.IndexOfInvariantCultureIgnoreCase("Cube Name") < 0)
				return str;
			OLAPConnectionStringBuilder cb = new OLAPConnectionStringBuilder(str);
			return cb.ConnectionString;
		}
	#region IDisposable Members
		public void Dispose() {
			Disconnect();
		}
	#endregion
	}
#endif
}
