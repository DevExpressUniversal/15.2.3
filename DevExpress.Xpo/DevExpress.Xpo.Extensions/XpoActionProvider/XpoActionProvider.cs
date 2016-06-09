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
using System.Linq;
using System.Text;
using System.Data.Services.Providers;
using System.Data.Services;
using System.Reflection;
using System.Collections;
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo {
	public interface IParameterMarshaller {
		object[] Marshall(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameters);
		object[] Marshall(DataServiceOperationContext operationContext, ServiceOperation serviceOperation, object[] parameters);
	}
	public class XpoActionProvider : IDataServiceActionProvider {
		readonly static Dictionary<string, ServiceAction> actionsByName = new Dictionary<string, ServiceAction>();
		readonly Type actionsContextInstanceType;
		readonly object actionsContext;
		readonly XpoDataServiceV3 dataService;
		readonly IParameterMarshaller marshaller;
		public XpoActionProvider(Session session, XpoDataServiceV3 dataService, object actionsContext) {
			this.dataService = dataService;
			this.actionsContext = actionsContext;
			actionsContextInstanceType = actionsContext.GetType();
			marshaller = new XpoParameterMarshaller(dataService, session);
		}
		public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool inFeed, ref Microsoft.Data.OData.ODataAction actionToSerialize) {
			var customState = serviceAction.CustomState as ActionInfo;
			return customState.IsAvailable(actionsContext, resourceInstance, inFeed);
		}
		public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameters) {
			return new ActionInvokable(operationContext, serviceAction, actionsContext, parameters, marshaller);
		}
		public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext) {
			return GetActions(operationContext);
		}
		public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType resourceType) {
			return GetActions(operationContext).Where(a => a.Parameters.Count > 0 && a.Parameters[0].ParameterType == resourceType);
		}
		public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction) {
			lock(actionsByName) {
				if(!actionsByName.TryGetValue(serviceActionName, out serviceAction)) {
					serviceAction = GetActions(operationContext, serviceActionName).SingleOrDefault();
					if(serviceAction != null) {
						actionsByName.Add(serviceActionName, serviceAction);
					}
				}
				return serviceAction != null;
			}
		}
		IEnumerable<ServiceAction> GetActions(DataServiceOperationContext context, string methodName = null) {
			IDataServiceMetadataProvider metadata = context.GetService(typeof(IDataServiceMetadataProvider)) as IDataServiceMetadataProvider;
			return new ActionFactory(metadata).GetActions(actionsContextInstanceType, methodName);
		}
	}
	public class XpoParameterMarshaller : IParameterMarshaller {
		static MethodInfo CastMethodGeneric = typeof(Enumerable).GetMethod("Cast");
		static MethodInfo ToListMethodGeneric = typeof(Enumerable).GetMethod("ToList");
		readonly XpoDataServiceV3 dataService;
		readonly Session session;
		public XpoParameterMarshaller(XpoDataServiceV3 dataService, Session session) {
			this.dataService = dataService;
			this.session = session;
		}
		public object[] Marshall(DataServiceOperationContext operationContext, ServiceAction action, object[] parameters) {
			return action.Parameters.Zip(parameters, (parameter, parameterValue) => GetMarshalledParameter(operationContext, parameter, parameterValue)).ToArray();
		}
		public object[] Marshall(DataServiceOperationContext operationContext, ServiceOperation operation, object[] parameters) {
			return operation.Parameters.Zip(parameters, (parameter, parameterValue) => GetMarshalledParameter(operationContext, parameter, parameterValue)).ToArray();
		}
		private object GetMarshalledParameter(DataServiceOperationContext operationContext, OperationParameter operationParameter, object value) {
			ResourceType parameterType = operationParameter.ParameterType;
			switch(parameterType.ResourceTypeKind) {
				case ResourceTypeKind.EntityType: {
						IDataServiceUpdateProvider2 updateProvider = operationContext.GetService(typeof(IDataServiceUpdateProvider2)) as IDataServiceUpdateProvider2;
						string typeName = parameterType.GetAnnotation().ResourceSet.Name;
						value = updateProvider.GetResource(value as IQueryable, typeName);
					} break;
				case ResourceTypeKind.EntityCollection:
						Type myType = (((EntityCollectionResourceType)operationParameter.ParameterType).ItemType).GetAnnotation().ClassInfo.ClassType;
						IQueryable result = dataService.Context.GetXPQueryCreator(myType).CreateXPQuery(session);				
						return result;
				case ResourceTypeKind.Collection: {
						Type elementType = ((CollectionResourceType)parameterType).ItemType.InstanceType;
						MethodInfo castMethod = CastMethodGeneric.MakeGenericMethod(elementType);
						object marshalledValue = castMethod.Invoke(null, new[] { value });
						MethodInfo toListMethod = ToListMethodGeneric.MakeGenericMethod(elementType);
						value = toListMethod.Invoke(null, new[] { marshalledValue });
					} break;
				default:
					break;
			}
			return value;
		}
	}
}
