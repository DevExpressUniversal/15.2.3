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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Reflection;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraTreeList.Native;
using System.Collections.Generic;
using DevExpress.Design.TypePickEditor;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraReports.Design.LightSwitch {
	public class DataSourceEditorLS : DataSourceEditorVS {
		#region inner classes
		protected class LightSwitchDataSourceTypePickerNode : TypePickerNode {
			string dataSourceName;
			public LightSwitchDataSourceTypePickerNode(string collectionName, string dataSourceName, Type type, TreeListNodes owner)
				: base(collectionName, type, owner) {
				this.dataSourceName = dataSourceName;
			}
			public string CollectionName {
				get {
					return Text;
				}
			}
			public string DataSourceName {
				get {
					return dataSourceName;
				}
			}
			public override IComponent GetInstance(IServiceProvider provider) {
				LightSwitchDataSource lightSwitchDataSource = new LightSwitchDataSource();
				lightSwitchDataSource.DataSource = this.Type;
				lightSwitchDataSource.CollectionName = CollectionName;
				lightSwitchDataSource.DataSourceName = DataSourceName;
				return lightSwitchDataSource;
			}
		}
		protected class LightSwitchQueryDataSourceTypePickerNode : LightSwitchDataSourceTypePickerNode {
			readonly LightSwitchDataSource.QueryParameterCollection parameters = new LightSwitchDataSource.QueryParameterCollection();
			public LightSwitchDataSource.QueryParameterCollection Parameters { get { return parameters; } }
			public LightSwitchQueryDataSourceTypePickerNode(string queryName, string dataSourceName, Type type, TreeListNodes owner)
				: base(queryName, dataSourceName, type, owner) {
			}
			public override IComponent GetInstance(IServiceProvider provider) {
				LightSwitchDataSource dataSource = (LightSwitchDataSource)base.GetInstance(provider);
				Parameter[] parameters = new Parameter[Parameters.Count];
				Parameters.CopyTo(parameters, 0);
				dataSource.QueryParameters.AddRange(parameters);
				dataSource.IsQuery = true;
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				foreach(Parameter parameter in dataSource.QueryParameters) {
					DesignToolHelper.AddToContainer(designerHost, parameter, parameter.Name);					
				}
				return dataSource;
			}
		}
		class DataSourcePickerTreeViewLS : DataSourcePickerTreeViewVS {
			public DataSourcePickerTreeViewLS(Type type)
				: base(type) {
			}
			protected override PickerNode CreateBindingSourcePickerNode(Type type) {
				return new LightSwitchDataSourceTypePickerNode(type.Name, "", type, this.Nodes) { StateImageIndex = 5 };
			}
			public override void Start(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
				base.Start(context, provider, currentValue);
				if (context != null && context.Instance is LightSwitchDataSource) {
					for (int i = 0; i < Nodes.Count; i++) {
						if (Nodes[i] is NonePickerNode)
							Nodes.RemoveAt(i);
					}
				}
			}
			protected override void FillNodeInternal(IDesignerHost designerHost) {
				Type type = designerHost.GetType(LSConstants.DataWorkspaceTypeName);
				if(type == null)
					return;
				PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				foreach(PropertyInfo item in propertyInfo) {
					if(!item.Name.Contains(LSConstants.XtraReportsDomainServicePropertyName)) {
						XtraListNode node = new XtraListNode(item.Name, Nodes) { StateImageIndex = 1 };
						PropertyInfo[] propertyInfos = item.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
						foreach(PropertyInfo item1 in propertyInfos) {
							if(item1.PropertyType.IsGenericType) {
								Type entityType = item1.PropertyType.GetGenericArguments()[0];
								TypePickerNode collectionNode = new LightSwitchDataSourceTypePickerNode(item1.Name, item.Name, entityType, node.Nodes) { StateImageIndex = 5 };
								node.Nodes.Add(collectionNode);
								MethodInfo[] methodInfos = item.PropertyType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
								foreach(MethodInfo methodInfo in methodInfos) {
									bool isReturnOneRecordQuery = methodInfo.ReturnType == entityType;
									bool isReturnManyRecordsQuery = methodInfo.ReturnType.IsGenericType && 
																	typeof(IEnumerable).IsAssignableFrom(methodInfo.ReturnType) && 
																	methodInfo.ReturnType.GetGenericArguments()[0] == entityType && 
																	!methodInfo.IsSpecialName;
									if(isReturnOneRecordQuery || isReturnManyRecordsQuery) {
										LightSwitchQueryDataSourceTypePickerNode queryNode = new LightSwitchQueryDataSourceTypePickerNode(methodInfo.Name, item.Name, entityType, collectionNode.Nodes) { StateImageIndex = 3 };
										QueryParameter[] queryParameters = QueryParametersHelper.GetParameters(methodInfo);
										queryNode.Parameters.AddRange(queryParameters);
										collectionNode.Nodes.Add(queryNode);
									}
								}
							}
						}
						Nodes.Add(node);
					}
				}
				foreach(XtraListNode item in Nodes) {
					item.ExpandAll();
				}
			}
			protected override string GetDisplayName(IComponent comp) {
				string getDisplayNameBaseResult = base.GetDisplayName(comp);
				if(comp is DevExpress.Data.EntityBindingSource && ((DevExpress.Data.EntityBindingSource)comp).DataSource is Type)
					return string.Format("{0} - {1}", getDisplayNameBaseResult, ((Type)((DevExpress.Data.EntityBindingSource)comp).DataSource).Name);
				return getDisplayNameBaseResult;
			}
		}
		#endregion
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new DataSourcePickerTreeViewLS(typeof(System.Data.DataSet));
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new TypePickerPanel(picker);
		}
	}
	public static class QueryParametersHelper {
		public static QueryParameter[] GetParameters(IDesignerHost designerHost, string dataSourceName, string queryName) {
			Type type = designerHost.GetType(LSConstants.DataWorkspaceTypeName);
			if (type == null)
				return null;
			List<QueryParameter> result = new List<QueryParameter>();
			PropertyInfo propertyInfo = type.GetProperty(dataSourceName);
			if (propertyInfo == null)
				throw new InvalidOperationException("Query parameters cannot be updated because the data source name has been changed. Please specify a correct data source name by using the DataSource editor.");
			MethodInfo methodInfo = propertyInfo.PropertyType.GetMethod(queryName);
			if (methodInfo == null)
				throw new InvalidOperationException("Query parameters cannot be updated because the query name has been changed. Please specify a correct query name by using the DataSource editor.");
			return GetParameters(methodInfo);
		}
		public static QueryParameter[] GetParameters(MethodInfo queryMethod) {
			List<QueryParameter> result = new List<QueryParameter>();
			foreach (System.Reflection.ParameterInfo parameterInfo in queryMethod.GetParameters()) {
				QueryParameter parameter = new QueryParameter();
				parameter.Name = parameterInfo.Name;
				parameter.Description = parameterInfo.Name;
				if (parameterInfo.ParameterType.IsGenericType)
					parameter.Type = parameterInfo.ParameterType.GetGenericArguments()[0];
				else
					parameter.Type = parameterInfo.ParameterType;
				result.Add(parameter);
			}
			return result.ToArray();
		}
		public static void RemoveQueryParameters(LightSwitchDataSource dataSource, IDesignerHost designerHost) {
			if (dataSource == null)
				return;
			while (dataSource.QueryParameters.Count > 0)
				DesignToolHelper.RemoveFromContainer(designerHost, dataSource.QueryParameters[0]);
		}
	}
	public class EntityDataSourceEditor : DataSourceEditorLS {
		protected override object GetEditValue(PickerNode selectedPickerNode, ITypeDescriptorContext context, IServiceProvider provider) {
			LightSwitchDataSourceTypePickerNode node = selectedPickerNode as LightSwitchDataSourceTypePickerNode;
			LightSwitchDataSource dataSource = context.Instance as LightSwitchDataSource;
			if(node != null && dataSource != null) {
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				DesignerTransaction transaction = designerHost.CreateTransaction("Set up dataSource properties");
				try {
					XRAccessor.SetProperty(dataSource, "DataSourceName", node.DataSourceName);
					XRAccessor.SetProperty(dataSource, "CollectionName", node.CollectionName);
					IComponentChangeService componentChangeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
					componentChangeService.OnComponentChanging(dataSource, null);
					QueryParametersHelper.RemoveQueryParameters(dataSource, designerHost);
					componentChangeService.OnComponentChanged(dataSource, null, null, null);
					if(node is LightSwitchQueryDataSourceTypePickerNode) {
						LightSwitchQueryDataSourceTypePickerNode queryNode = (LightSwitchQueryDataSourceTypePickerNode)node;
						componentChangeService.OnComponentChanging(dataSource, null);
						QueryParameter[] parameters = new QueryParameter[queryNode.Parameters.Count];
						queryNode.Parameters.CopyTo(parameters, 0);
						dataSource.QueryParameters.AddRange(parameters);
						foreach (Parameter parameter in dataSource.QueryParameters) {
							DesignToolHelper.AddToContainer(designerHost, parameter, parameter.Name);
						} 
						componentChangeService.OnComponentChanged(dataSource, null, null, null);
					}
					XRAccessor.SetProperty(dataSource, "IsQuery", node is LightSwitchQueryDataSourceTypePickerNode);
					transaction.Commit();
					TypeDescriptor.Refresh(dataSource);
				} catch {
					transaction.Cancel();
				}
				return node.Type;
			}			
			return null;
		}
	}
}
