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
using System.Collections;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data.Browsing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Data;
#if !DXPORTABLE
using System.Data.Common;
using DevExpress.Data.Design;
#endif
namespace DevExpress.Data.Native {
#if DXPORTABLE
	public class BindingHelper {
		public static bool IsList(PropertyDescriptor property) {
			Type bt = typeof(Byte[]);
			return !bt.Equals(property.PropertyType) && ListTypeHelper.IsListType(property.PropertyType);
		}
		public static string GetDataSourceName(IComponent obj) {
			if(obj == null)  
				return "Null";
			DataTable dataTable = obj as DataTable;
			if (dataTable != null && !String.IsNullOrEmpty(dataTable.TableName))
				return dataTable.TableName;
			return obj.Site != null ? obj.Site.Name : obj.GetType().Name;
		}
		public static string JoinStrings(string separator, string s1, string s2) {
			return string.IsNullOrEmpty(s1) ? s2 :
				string.IsNullOrEmpty(s2) ? s1 :
				String.Join(separator, new string[] { s1, s2 });
		}
	}
#else
	public class BindingHelper {
		public static bool IsDataObject(object obj) {
			DataObjectAttribute attrib = TypeDescriptor.GetAttributes(obj)[typeof(DataObjectAttribute)] as DataObjectAttribute;
			return attrib != null && attrib.IsDataObject;
		}
		public static bool IsDataSource(object obj) {
			return IsListSource(obj) && DataSourceConverter.IsListBindable(obj) && !(obj is DevExpress.Data.ChartDataSources.IChartDataSource);
		}
		public static bool IsListSource(object obj) {
			return obj is IListSource || obj is IList;
		}
		public static bool IsList(PropertyDescriptor property) {
			Type bt = typeof(Byte[]);
			return !bt.Equals(property.PropertyType) && ListTypeHelper.IsListType(property.PropertyType);
		}
		public static string GetDataSourceName(IComponent obj) {
			if(obj == null)  
				return "Null";
			DataTable dataTable = obj as DataTable;
			if(dataTable != null && !String.IsNullOrEmpty(dataTable.TableName))
				return dataTable.TableName;
			return obj.Site != null ? obj.Site.Name : obj.GetType().Name;
		}
		public static string JoinStrings(string separator, string s1, string s2) {
			return string.IsNullOrEmpty(s1) ? s2 :
				string.IsNullOrEmpty(s2) ? s1 :
				String.Join(separator, new string[] {s1, s2}); 
		}
		public static IDataAdapter ConvertToIDataAdapter(object obj) {
			if(!IsDataAdapter(obj))
				return null;
			if(obj is IDataAdapter) 
				return (IDataAdapter)obj;
			try {
				return ExtractDataAdapter(obj);
			} catch {
				return null;
			}
		}
		static IDataAdapter ExtractDataAdapter(object obj) {
#if DXWhidbey
			PropertyInfo pi = obj.GetType().GetProperty("Adapter", BindingFlags.Instance | BindingFlags.NonPublic);
			IDataAdapter adapter = pi.GetValue(obj, null) as IDataAdapter;
			if(adapter == null)
				return null;
			pi = obj.GetType().GetProperty("CommandCollection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			PropertyInfo dapi = adapter.GetType().GetProperty("SelectCommand", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
			if(pi != null && dapi != null) {
				Array arr = (Array)pi.GetValue(obj, null);
				if(arr != null) {
					dapi.SetValue(adapter, arr.GetValue(0), null);
					return adapter;
				}
			}
#endif // DXWhidbey
			return null;
		}
		public static bool IsDataAdapter(object obj) {
			if (obj == null)
				return false;
			if(obj is IDataAdapter)
				return true;
#if DXWhidbey
			AttributeCollection attributes = TypeDescriptor.GetAttributes(obj);
			DataObjectAttribute dataObjectAttribute = attributes[typeof(DataObjectAttribute)] as DataObjectAttribute;
			return dataObjectAttribute != null && !dataObjectAttribute.IsDefaultAttribute() && obj.GetType().GetProperty("Adapter", BindingFlags.Instance | BindingFlags.NonPublic) != null;
#else
			return false;
#endif
		}
		public static DataSet ConvertToDataSet(object obj) {
			if (obj == null)
				return null;
			if (obj is DataSet)
				return (DataSet)obj;
			if (obj is DataTable)
				return ((DataTable)obj).DataSet;
			if (obj is DataView && ((DataView)obj).Table != null)
				return ((DataView)obj).Table.DataSet;
#if DXWhidbey
			if (obj is System.Windows.Forms.BindingSource) {
				System.Windows.Forms.BindingSource bindingSource = (System.Windows.Forms.BindingSource)obj;
				return ConvertToDataSet(bindingSource.DataSource);
			}
#endif
			return null;
		}
		public static string ConvertToTableName(object obj, string member) {
			if(obj == null)
				return "";
			if(obj is DataSet)
				return member;
			if(obj is DataTable)
				return ((DataTable)obj).TableName;
			if(obj is DataView)
				return ((DataView)obj).Table.TableName;
#if DXWhidbey
			if(obj is System.Windows.Forms.BindingSource) {
				System.Windows.Forms.BindingSource bindingSource = (System.Windows.Forms.BindingSource)obj;
				return bindingSource.DataMember;
			}
#endif
			return null;
		}
		public static object DetermineDataAdapter(IList typedAdapters, DataTable table) {
			if(table == null)
				return null;
			object typedDataAdapter = DetermineDataAdapter(typedAdapters, table.TableName, table.GetType().Namespace);
			if(typedDataAdapter == null)
				typedDataAdapter = DetermineDataAdapter(typedAdapters, table.TableName, string.Empty);
			return typedDataAdapter;
		}
		static object DetermineDataAdapter(IList typedAdapters, string tableName, string tableNamespace) {
			foreach(object typedAdapter in typedAdapters) {
				IDataAdapter dataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(typedAdapter);
				if(dataAdapter != null)
					foreach(DataTableMapping mapping in dataAdapter.TableMappings) {
						bool skipNamespaces = typedAdapter.GetType().Namespace == null || tableNamespace == null;
						if(tableName.Equals(mapping.DataSetTable) && (skipNamespaces || typedAdapter.GetType().Namespace.StartsWith(tableNamespace)))
							return typedAdapter;
					}
			}
			return null;
		}
		public static object DetermineDataAdapter(IList typedAdapters, string tableName) {
			return DetermineDataAdapter(typedAdapters, tableName, string.Empty);
		}
	}
#if DXWhidbey
	public class VS2005ConnectionStringHelper : IDisposable {
		const string dataDirectoryTag = "|DataDirectory|";
		static Type GetType(string assemblyName, string typeName) {
			Assembly assembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
			return assembly != null ? assembly.GetType(typeName) : null;
		}
		static object GetPropertyValue(Type type, object obj, string propertyName) {
			return type.GetProperty(propertyName).GetValue(obj, null);
		}
		static string GetDataConnectionString(object dataAdapter) {
			object connection;
			PropertyInfo pi = FindConnection(dataAdapter, out connection);
			return pi == null ? null : (string)pi.GetValue(connection, null);
		}
		static void SetDataConnectionString(object obj, string connectionString) {
			object connection;
			PropertyInfo pi = FindConnection(obj, out connection);
			if (pi != null)
				pi.SetValue(connection, connectionString, null);
		}
		static PropertyInfo FindConnection(object obj, out object connection) {
			connection = null;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(obj);
			foreach (Attribute attribute in attributes)
				if (attribute is DataObjectAttribute) {
					PropertyInfo pi = obj.GetType().GetProperty("Connection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
					if (pi != null) {
						try {
						connection = pi.GetValue(obj, null);
						}
						catch { }
						if (connection != null)
							return connection.GetType().GetProperty("ConnectionString");
					}
				}
			return null;
		}
		string connectionString = string.Empty;
		object dataAdapter = null;
		IDesignerHost designerHost = null;
		public VS2005ConnectionStringHelper() {
		}
		public VS2005ConnectionStringHelper(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		bool ShouldPatchConnectionString(object dataAdapter) {
			IComponent component = dataAdapter as IComponent;
			if (component == null || (designerHost == null && component.Site == null))
				return false;
			string connectionString = GetDataConnectionString(dataAdapter);
			return string.IsNullOrEmpty(connectionString) ? false : connectionString.IndexOf(dataDirectoryTag) >= 0;
		}
		public void PatchConnectionString(object dataAdapter) {
			if (!ShouldPatchConnectionString(dataAdapter))
				return;
			this.connectionString = GetDataConnectionString(dataAdapter);
			this.dataAdapter = dataAdapter;
			if (designerHost == null)
				designerHost = (IDesignerHost)((IComponent)dataAdapter).Site.GetService(typeof(IDesignerHost));
			string patchedConnectionString = GetPatchedConnectionString(designerHost, this.connectionString);
			if(patchedConnectionString != null && patchedConnectionString != this.connectionString)
				SetDataConnectionString(dataAdapter, patchedConnectionString);
		}
		public string GetPatchedConnectionString(IDesignerHost designerHost, string connectionString) {
			if(string.IsNullOrEmpty(connectionString) || designerHost == null) return connectionString;
			int tagIndex = connectionString.IndexOf(dataDirectoryTag);
			if(tagIndex < 0) return connectionString;
			Type projectItemType = GetType("EnvDTE", "EnvDTE.ProjectItem");
			object projectItem = designerHost.GetService(projectItemType);
			if(projectItem == null) return connectionString;
			object containingProject = GetPropertyValue(projectItemType, projectItem, "ContainingProject");
			string name = (string)GetPropertyValue(GetType("EnvDTE", "EnvDTE.Project"), containingProject, "FullName");
			int index = name.LastIndexOf('\\');
			if(index >= 0) {
				name = name.Substring(0, index);
				if(IsWebProject(containingProject) || IsWebApplication(containingProject)) name += "\\App_Data";
				string s = connectionString.Remove(tagIndex, dataDirectoryTag.Length);
				return s.Insert(tagIndex, name);
			}
			return connectionString;
		}
		bool IsWebProject(object containingProject) {
			string kind = (string)GetPropertyValue(GetType("EnvDTE", "EnvDTE.Project"), containingProject, "Kind");
			return kind.ToUpper() == "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
		}
		bool IsWebApplication(object containingProject) {
			string[] extenderNames = GetPropertyValue(GetType("EnvDTE", "EnvDTE.Project"), containingProject, "ExtenderNames") as string[];
			return extenderNames != null ? Array.IndexOf(extenderNames, "WebApplication") >= 0 : false;
		}
		void IDisposable.Dispose() {
			if (!string.IsNullOrEmpty(connectionString))
				SetDataConnectionString(dataAdapter, connectionString);
		}
	}
#endif
#endif
}
