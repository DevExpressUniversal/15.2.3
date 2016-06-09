#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using System.ServiceModel;
using DevExpress.Persistent.Base;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer.Wcf {
	[System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed),
	ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,
		MaxItemsInObjectGraph = Int32.MaxValue)]
	public class WcfSecuredDataServer : IWcfSecuredDataServer, IErrorHandler, IServiceBehavior {
		readonly IDataServer dataServer;
		public WcfSecuredDataServer(IDataServer dataServer) {
			this.dataServer = dataServer;
		}
		#region ISecuredSerializableObjectLayer2
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return dataServer.LoadObjects(clientInfo, dictionary, queries);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return dataServer.CommitObjects(clientInfo, dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return dataServer.GetObjectsByKey(clientInfo, dictionary, queries);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return dataServer.SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return dataServer.CanLoadCollectionObjects(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return dataServer.LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public PurgeResult Purge(IClientInfo clientInfo) {
			return dataServer.Purge(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return dataServer.LoadDelayedProperties(clientInfo, dictionary, theObject, props);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return dataServer.LoadDelayedProperties(clientInfo, dictionary, objects, property);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return dataServer.IsParentObjectToSave(clientInfo, dictionary, theObject);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return dataServer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo) {
			return dataServer.GetParentObjectsToSave(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo) {
			return dataServer.GetParentObjectsToDelete(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return dataServer.GetParentTouchedClassInfos(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			dataServer.CreateObjectType(clientInfo, assemblyName, typeName);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public object Do(IClientInfo clientInfo, string command, object args) {
			return dataServer.Do(clientInfo, command, args);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public void FinalizeSession(IClientInfo clientInfo) {
			dataServer.FinalizeSession(clientInfo);
		}
		#endregion
		#region IServerSecurity
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public void Logon(IClientInfo clientInfo) {
			dataServer.Logon(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public void Logoff(IClientInfo clientInfo) {
			dataServer.Logoff(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest) {
			return dataServer.IsGranted(clientInfo, permissionRequest);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public IList<bool> IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests) {
			return dataServer.IsGranted(clientInfo, permissionRequests);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public object GetUserId(IClientInfo clientInfo) {
			return dataServer.GetUserId(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public string GetUserName(IClientInfo clientInfo) {
			return dataServer.GetUserName(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public object GetLogonParameters(IClientInfo clientInfo) {
			return dataServer.GetLogonParameters(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool GetNeedLogonParameters(IClientInfo clientInfo) {
			return dataServer.GetNeedLogonParameters(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public bool GetIsLogoffEnabled(IClientInfo clientInfo) {
			return dataServer.GetIsLogoffEnabled(clientInfo);
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public string GetUserTypeName(IClientInfo clientInfo) {
			Type t = dataServer.GetUserType(clientInfo);
			return (t != null) ? t.AssemblyQualifiedName : null;
		}
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public string GetRoleTypeName(IClientInfo clientInfo) {
			Type t = dataServer.GetRoleType(clientInfo);
			return (t != null) ? t.AssemblyQualifiedName : null;
		}
		#endregion
		#region IServiceBehavior Members
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) {
		}
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
			foreach(ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers) {
				dispatcher.ErrorHandlers.Add(this);
			}
		}
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
		}
		#endregion
		#region IErrorHandler
		public virtual bool HandleError(Exception error) {
			return false;
		}
		public virtual void ProvideFault(Exception error, MessageVersion version, ref Message fault) {
			ExceptionDetail exceptionDetail = new ExceptionDetail(error);
			FaultException<ExceptionDetail> faultException = new FaultException<ExceptionDetail>(exceptionDetail, exceptionDetail.Message);
			MessageFault messageFault = faultException.CreateMessageFault();
			fault = Message.CreateMessage(version, messageFault, faultException.Action);
		}
		#endregion
	}
}
