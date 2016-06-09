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
using  DevExpress.Utils.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design
{
	[ComImport, Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IPictureDisp {
		IntPtr Handle { get; }
		IntPtr HPal { get; }
		short PictureType { get; }
		int Width { get; }
		int Height { get; }
		void Render(IntPtr hdc, int x, int y, int cx, int cy, int xSrc, int ySrc, int cxSrc, int cySrc);
	}
	[ComVisible(true), ComImport(), Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleServiceProvider {
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int QueryService(
			[In]
			ref Guid guidService,
			[In]
			ref Guid riid,
			out IntPtr ppvObject);
	}
	public class DataConnectionsServiceWrapper
	{
		static readonly Guid SqlServerDataProviderGuid = new Guid("{91510608-8809-4020-8897-FBA057E22D54}");
		static readonly Guid OleDBDataProviderGuid = new Guid("{7F041D59-D76A-44ed-9AA2-FBF6B0548B80}");
		VsDataConnectionsServiceAccessor dataConnectionsService;
		IServiceProvider serviceProvider = null;
		const BindingFlags invokeMethodAttr = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		static System.Collections.Hashtable providers = new System.Collections.Hashtable();
		static DataConnectionsServiceWrapper() {
			providers["System.Data.SqlClient"] = "Provider=SQLOLEDB;";
			providers["System.Data.OracleClient"] = "Provider=MSDAORA;";
			providers["System.Data.Odbc"] = "Provider=MSDASQL;";
		}
		public DataConnectionsServiceWrapper(IServiceProvider serviceProvider, object connectionService) {
			this.serviceProvider = serviceProvider;
			dataConnectionsService = new VsDataConnectionsServiceAccessor(connectionService);
		}
		public int PromptAndAddConnection() {
			string connectionString = Native.ConnectionStringHelper.GetConnectionString();
			if(connectionString == string.Empty) return -1;
			string connectionName = Native.ConnectionStringHelper.GetConnectionName(connectionString);
			return AddConnection(connectionName, connectionString);
		}
		public string GetConnectionName(int connectionIndex) {
			return dataConnectionsService.GetConnectionName(connectionIndex);
		}
		public string GetConnectionString(int connectionIndex) {
			DataConnectionAccessor[] connections = GetConnections();
			return GetConnectionStringWithProvider(connections[connectionIndex]);
		}
		static string GetConnectionStringWithProvider(DataConnectionAccessor dataConnection) {
			if(dataConnection.Name.Contains("MSDASQL"))
				return dataConnection.Name;
			string connectionString = dataConnection.RunTimeConnectionString;
			return connectionString.IndexOf("Provider=") < 0 ? connectionString.Insert(0, GetProvider(dataConnection.ProviderName)) : connectionString;
		}
		static string GetProvider(string providerName) {
			string provider = (string)providers[providerName];
			return provider != null ? provider : string.Empty;
		}
		public int GetConnectionIndex(string connectionName) {
			return dataConnectionsService.GetConnectionIndex(connectionName);
		}
		public string GetConnectionNameUsingConnectionString(string connectionString)
		{
			DataConnectionAccessor[] connections = GetConnections();
			foreach (DataConnectionAccessor dataConnection in connections) {
				if (GetConnectionStringWithProvider(dataConnection).Equals(connectionString))
					return dataConnection.Name;
			}
			return string.Empty;
		}
		int AddConnection(string connectionName, string connectionString) {
			Guid guidProvider;
			if(ConnectionStringHelper.GetConnectionType(connectionString) == ConnectionType.OleDB) {
				guidProvider = OleDBDataProviderGuid;
			} else {
				connectionString = ConnectionStringHelper.RemoveProviderFromConnectionString(connectionString);
				guidProvider = SqlServerDataProviderGuid;
			}
			return dataConnectionsService.AddConnection(connectionName, guidProvider, connectionString, false);
		}
		DataConnectionAccessor[] GetConnections() {
			GlobalConnectionServiceAccessor serv = new GlobalConnectionServiceAccessor(serviceProvider);
			EnvDTE.Project project = ((EnvDTE.ProjectItem)serviceProvider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			return serv.GetConnections(serviceProvider, project);
		}
		public StringCollection GetConnectionNames() {
			DataConnectionAccessor[] connections = GetConnections();
			StringCollection connectionNames = new StringCollection();
			foreach (DataConnectionAccessor item in connections)
				connectionNames.Add(item.Name);
			return connectionNames;
		}
	}
	[System.Runtime.InteropServices.Guid("7494682C-37A0-11D2-A273-00C04F8EF4FF"), System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVsServerExplorer {
	}
	class VsServerExplorerAccessor : InterfaceAcessor {
		public VsServerExplorerAccessor(IServiceProvider provider)
			: base(provider.GetService(typeof(IVsServerExplorer)), "Microsoft.VSDesigner.ServerExplorer.IVsServerExplorer") {
		}
		public void Init() {
			InvokeMethod("Init", new object[0]);
		}
		public object GetRootService(ref Guid guid) {
			return InvokeMethod("GetRootService", new object[] { guid });
		}
	}	
	class DbObjectType {
		static Type type;
		public static System.Type Type {
			get {
				if(type == null) {
					type = InterfaceAcessor.MicrosoftVSDesignerAssembly.GetType("Microsoft.VSDesigner.Data.DbObjectType");
				}
				return type;
			}
		}
		public static int Table {
			get {
				return (int)Enum.Parse(Type, "Table");
			}
		}
	}
}
