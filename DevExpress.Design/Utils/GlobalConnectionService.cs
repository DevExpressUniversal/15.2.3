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

using DevExpress.Data.Entity;
using DevExpress.Entity.Model;
using EnvDTE;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.Design;
using DevExpress.Data.Native;
using System.Configuration;
using System.IO;
namespace DevExpress.Utils.Design {
	public class InterfaceAcessor {
		InterfaceMapping interfaceMapping;
		object interfaceImplementation;
		protected object InterfaceImplementation {
			get {
				return interfaceImplementation;
			}
		}
		public static Assembly MicrosoftVSDesignerAssembly {
			get {
				return Assembly.Load("Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			}
		}
		public InterfaceAcessor(object interfaceImplementation, string interfaceName) {
			this.interfaceImplementation = interfaceImplementation;
			Type type = interfaceImplementation.GetType();
			Type interfaceType = type.GetInterface(interfaceName);
			interfaceMapping = type.GetInterfaceMap(interfaceType); 
		}
		MethodInfo GetMethod(string methodName) {
			MethodInfo interfaceMethodInfo = interfaceMapping.InterfaceType.GetMethod(methodName);
			int methodIndex = Array.IndexOf(interfaceMapping.InterfaceMethods, interfaceMethodInfo);
			return interfaceMapping.TargetMethods[methodIndex];
		}
		protected object InvokeMethod(string methodName, object[] parameters) {
			return GetMethod(methodName).Invoke(InterfaceImplementation, parameters);
		}
	}
	public class VsDataConnectionsServiceAccessor : InterfaceAcessor {
		public static readonly Guid Guid = new Guid("D8D207DA-64EB-4B46-9740-282F5F458B22");
		public VsDataConnectionsServiceAccessor(object dataConnectionsService)
			: base(dataConnectionsService, "Microsoft.VisualStudio.Data.Interop.IVsDataConnectionsService") {
		}
		public string GetConnectionName(int connectionIndex) {
			return (string)InvokeMethod("GetConnectionName", new object[] { connectionIndex });
		}
		public int GetConnectionIndex(string connectionName) {
			return (int)InvokeMethod("GetConnectionIndex", new object[] { connectionName });
		}
		public int AddConnection(string connectionName, Guid guidProvider, string connectionString, bool encryptedString) {
			return (int)InvokeMethod("AddConnection", new object[] { connectionName, guidProvider, connectionString, encryptedString });
		}
	}
	[System.Runtime.InteropServices.Guid("F3DA2C42-8A1A-4234-BBAE-E6E1FB0D67E9")]
	public interface IGlobalConnectionService {
	}
	public class GlobalConnectionServiceAccessor : InterfaceAcessor {
		public GlobalConnectionServiceAccessor(IServiceProvider provider)
			: base(provider.GetService(typeof(IGlobalConnectionService)), "Microsoft.VSDesigner.VSDesignerPackage.IGlobalConnectionService") {
		}
		[CLSCompliant(false)]
		public DataConnectionAccessor[] GetConnections(IServiceProvider serviceProvider, Project project) {
			Array dataConnections = (Array)InvokeMethod("GetConnections", new object[] { serviceProvider, project });
			List<DataConnectionAccessor> dataConnectionAccessors = new List<DataConnectionAccessor>();
			foreach (object dataConnection in dataConnections)
				dataConnectionAccessors.Add(new DataConnectionAccessor(dataConnection));
			return dataConnectionAccessors.ToArray();
		}
		[CLSCompliant(false)]
		public bool AddConnectionToAppSettings(IServiceProvider serviceProvider, Project project, DataConnectionAccessor connection) {
			return (bool)InvokeMethod("AddConnectionToAppSettings", new object[] { serviceProvider, project, connection.DataConnection });
		}
	}
	public class DataConnectionAccessor : IConnectionStringInfo {
		readonly object dataConnection;
		public DataConnectionAccessor(object dataConnection) {
			this.dataConnection = dataConnection;
		}
		public DataConnectionAccessor(string name, string providerName, string connectionString) {
			this.dataConnection = InvokeCtor(name, providerName, connectionString);
		}
		public object DataConnection { get { return dataConnection; } }
		public string Name {
			get {
				return (string)GetPropertyValue("Name");
			}
		}
		public string RunTimeConnectionString {
			get {
				return (string)GetPropertyValue("RunTimeConnectionString");
			}
		}
		public string ProviderName {
			get {
				return (string)GetPropertyValue("ProviderName");
			}
		}
		public DataConnectionLocation Location { get { return (DataConnectionLocation)GetPropertyValue("Location"); } }
		private object GetPropertyValue(string propertyName) {
			return dataConnection.GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, dataConnection, null);
		}
		private static object InvokeCtor(string name, string providerName, string connectionString) {
			Type type = InterfaceAcessor.MicrosoftVSDesignerAssembly.GetType("Microsoft.VSDesigner.VSDesignerPackage.DataConnection");
			ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string), typeof(string), typeof(string) });
			return ctor.Invoke(new object[] { name, providerName, connectionString });
		}
	}
	public class VSConnectionStringsService : IConnectionStringsProvider {
		public string ConfigFileName { get { return ProjectHelper.IsWebProject(serviceProvider) || ProjectHelper.IsWebApplication(serviceProvider) ? "web.config" : "app.config"; } }
		public string ConfigTemplateName { get { return ProjectHelper.IsWebProject(serviceProvider) || ProjectHelper.IsWebApplication(serviceProvider) ? "WebConfig.zip" : "AppConfig.zip"; } }
		public string GetFileName() {
			Project project = ProjectHelper.GetActiveProject(serviceProvider);
			foreach(ProjectItem item in project.ProjectItems)
				if(item.Name.Equals(ConfigFileName, StringComparison.OrdinalIgnoreCase))
					return (string)item.Properties.Item("FullPath").Value;
			return null;
		}
		IServiceProvider serviceProvider;		
		public VSConnectionStringsService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;			
		}
		public string PatchConnectionString(string connectionString) {
			const string dataDirectoryTag = "|DataDirectory|";
			if(string.IsNullOrEmpty(connectionString) || serviceProvider == null) return connectionString;
			int tagIndex = connectionString.IndexOf(dataDirectoryTag);
			if(tagIndex < 0) return connectionString;
			EnvDTE.Project containingProject = ProjectHelper.GetActiveProject(serviceProvider);
			if(containingProject == null) return connectionString;
			string name = containingProject.FullName;
			int index = name.LastIndexOf('\\');
			if(index >= 0) {
				name = name.Substring(0, index);
				if(ProjectHelper.IsWebProject(containingProject) || ProjectHelper.IsWebApplication(containingProject))
					name += "\\App_Data";
				string s = connectionString.Remove(tagIndex, dataDirectoryTag.Length);
				return s.Insert(tagIndex, name);
			}
			return connectionString;
		}
		public IConnectionStringInfo[] GetConnections() {
			GlobalConnectionServiceAccessor serv = new GlobalConnectionServiceAccessor(serviceProvider);
			return serv.GetConnections(serviceProvider, ProjectHelper.GetActiveProject(serviceProvider)).ToArray();
		}
		public IConnectionStringInfo[] GetConfigFileConnections() {
			string fileName = GetFileName();
			List<ConnectionStringInfo> connectionStrings = new List<ConnectionStringInfo>();
			if(fileName != null) {
				ConnectionStringSettingsCollection collection = OpenConfig(fileName).ConnectionStrings.ConnectionStrings;
				foreach(ConnectionStringSettings item in collection) {
					connectionStrings.Add(new ConnectionStringInfo() { Name = item.Name, RunTimeConnectionString = item.ConnectionString, ProviderName = item.ProviderName });
				}
			}
			return connectionStrings.ToArray();
		}
		public IConnectionStringInfo GetConnectionStringInfo(string connectionStringName) {
			var result = GetConfigFileConnections().Where(ic => ic.Name == connectionStringName).FirstOrDefault();
			if(result == null)
				throw new InvalidOperationException(string.Format("Can't find connection {0} in config file", connectionStringName));
			return result;
		}
		public string GetConnectionString(string connectionStringName) {
			IConnectionStringInfo info = GetConnectionStringInfo(connectionStringName);
			if (info != null)
				return PatchConnectionString(info.RunTimeConnectionString);
			return string.Empty;
		}
		static System.Configuration.Configuration OpenConfig(string configFileName) {
			Guard.ArgumentNotNull(configFileName, "configFileName");
			return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configFileName }, ConfigurationUserLevel.None);
		}		
	}
}
