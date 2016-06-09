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
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
namespace DevExpress.XtraPivotGrid {
	public enum AdomdVersion { Version9, Version10, Version11, Version12 };
	public class AdomdMetaGetter : IOLAPMetaGetter {
		const string AdomdClientName = "Microsoft.AnalysisServices.AdomdClient";
		#region static stuff
		static bool? isProviderAvailable;
		static AdomdVersion? availableVersion;
		readonly static Dictionary<AdomdVersion, Assembly> assembliesCache = new Dictionary<AdomdVersion, Assembly>();
		public static bool IsProviderAvailable {
			get {
				if(!isProviderAvailable.HasValue) {
					isProviderAvailable = GetIsProviderAvailable(AdomdVersion.Version12);
					if(isProviderAvailable == false)
						isProviderAvailable = GetIsProviderAvailable(AdomdVersion.Version11);
					if(isProviderAvailable == false)
						isProviderAvailable = GetIsProviderAvailable(AdomdVersion.Version10);
					if(isProviderAvailable == false)
						isProviderAvailable = GetIsProviderAvailable(AdomdVersion.Version9);
				}
				return isProviderAvailable.Value;
			}
		}
		public event EventHandler<AdomdMetaGetterExceptionEventArgs> Exception;
		public static bool GetIsProviderAvailable(AdomdVersion version) {
			try {
				AssemblyName adoName = GetAdomdAssemblyName(version);
				if(adoName == null)
					return false;
				Assembly adoClient = Assembly.ReflectionOnlyLoad(adoName.FullName);
				bool res = adoClient != null;
				if(res) availableVersion = version;
				return res;
			} catch(Exception e) {
				if((e is FileNotFoundException) || (e is FileLoadException) || (e is BadImageFormatException))
					return false;
				else
					throw e;
			} 
		}
		public static AdomdVersion GetNewestAdomdVersion() {
			if(GetIsProviderAvailable(AdomdVersion.Version12))
				return AdomdVersion.Version12;
			if(GetIsProviderAvailable(AdomdVersion.Version11))
				return AdomdVersion.Version11;
			if(GetIsProviderAvailable(AdomdVersion.Version10))
				return AdomdVersion.Version10;
			return AdomdVersion.Version9;
		}
		public static Assembly LoadAdomdAssembly(AdomdVersion version) {
			lock(assembliesCache) {
				Assembly res;
				if(assembliesCache.TryGetValue(version, out res))
					return res;
				AssemblyName name = GetAdomdAssemblyName(version);
				res = Assembly.Load(name);
				assembliesCache.Add(version, res);
				return res;
			}
		}
		static AssemblyName GetAdomdAssemblyName(AdomdVersion version) {
			AssemblyName res = new AssemblyName() { Name = AdomdClientName };
			switch(version) {
				case AdomdVersion.Version9:
					res.Version = new Version(9, 0, 242, 0);
					break;
				case AdomdVersion.Version10:
					res.Version = new Version(10, 0, 0, 0);
					break;
				case AdomdVersion.Version11:
					res.Version = new Version(11, 0, 0, 0);
					break;
				case AdomdVersion.Version12:
					res.Version = new Version(12, 0, 0, 0);
					break;
				default:
					throw new ArgumentException("version");
			}
			res.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
			res.SetPublicKeyToken(new byte[] { 0x89, 0x84, 0x5d, 0xcd, 0x80, 0x80, 0xcc, 0x91 });
			return res;
		}
		#endregion
		AdomdConnection olapConnection;
		string connectionString;
		#region IDisposable Members
		bool disposed;
		public void Dispose() {
			if(!disposed) {
				disposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}
		~AdomdMetaGetter() {
			if(!disposed) {
				Dispose(false);
				disposed = true;
			}
		}
		protected virtual void Dispose(bool disposing) {
			Disconnect();
		}
		#endregion
		#region IOLAPMetaGetter Members
		public bool Connected {
			get { return olapConnection != null; }
			set {
				if(Connected == value) return;
				if(value) Connect();
				else Disconnect();
			}
		}
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString != value || !Connected) {
					connectionString = value;
					Connect();
				}
			}
		}
		public List<string> GetCatalogs() {
			if(!Connected) return null;
			DataSet catalogsSet = olapConnection.GetSchemaDataSet(AdomdSchemaGuid.Catalogs, null);
			if(catalogsSet == null || catalogsSet.Tables.Count == 0) return null;
			DataTable catalogsTable = catalogsSet.Tables[0];
			List<string> catalogs = new List<string>(catalogsTable.Rows.Count);
			for(int i = 0; i < catalogsTable.Rows.Count; i++)
				catalogs.Add((string)catalogsTable.Rows[i]["CATALOG_NAME"]);
			return catalogs;
		}
		public List<string> GetCubes(string catalogName) {
			if(!Connected || string.IsNullOrEmpty(catalogName)) return null;
			olapConnection.ChangeDatabase(catalogName);
			DataSet cubesSet = olapConnection.GetSchemaDataSet(AdomdSchemaGuid.Cubes, new object[] { catalogName });
			if(cubesSet == null || cubesSet.Tables.Count == 0) return null;
			DataTable cubesTable = cubesSet.Tables[0];
			List<string> cubes = new List<string>(cubesTable.Rows.Count);
			for(int i = 0; i < cubesTable.Rows.Count; i++)
				cubes.Add((string)cubesTable.Rows[i]["CUBE_NAME"]);
			return cubes;
		}
		#endregion
		void Connect() {
			if(Connected) Disconnect();
			if(ConnectionString == null) return;
			if(!IsProviderAvailable || !availableVersion.HasValue)
				throw new ArgumentException(AdomdClientName + ".dll is not available");
			olapConnection = new AdomdConnection(availableVersion.Value);
			olapConnection.ConnectionString = RemoveCubeName(ConnectionString);
			try {
				olapConnection.Open();
				if(!olapConnection.IsOpen)
					Disconnect();
			} catch(Exception ex) {
				Disconnect();
				if(Exception != null)
					Exception(this, new AdomdMetaGetterExceptionEventArgs(ex));
			}
		}
		void Disconnect() {
			if(!Connected) return;
			olapConnection.Close(true);
			olapConnection.Dispose();
			olapConnection = null;
		}
		string RemoveCubeName(string str) {
			if(str.IndexOf("Cube Name", StringComparison.InvariantCultureIgnoreCase) < 0)
				return str;
			OLAPConnectionStringBuilder cb = new OLAPConnectionStringBuilder(str);
			return cb.ConnectionString;
		}
	}
	public class AdomdMetaGetterExceptionEventArgs : EventArgs {
		Exception exception;
		public AdomdMetaGetterExceptionEventArgs(Exception exception) {
			this.exception = exception;
		}
		public Exception Exception { get { return exception; } }
	}
}
