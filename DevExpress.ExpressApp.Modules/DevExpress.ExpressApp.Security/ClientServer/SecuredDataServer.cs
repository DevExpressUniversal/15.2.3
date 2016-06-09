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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public interface IDataServer : ISecuredSerializableObjectLayer, IServerSecurity {
	}
	public delegate IDataServerSecurity QueryRequestSecurityStrategyHandler();
	public interface IDataServerSecurity : ISelectDataSecurityProvider, IRequestSecurityStrategy {
		void SetLogonParameters(object logonParameters);
	}
	public class SecuredDataServer : IDataServer {
		public class RequestSecurityStrategyProvider : IRequestSecurityStrategyProvider {
			private QueryRequestSecurityStrategyHandler handler;
			private IDataLayer dataLayer;
			private UnitOfWork CreateUnitOfWork() {
				return new UnitOfWork(dataLayer);
			}
			public RequestSecurityStrategyProvider(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler handler) {
				Guard.ArgumentNotNull(dataLayer, "dataLayer");
				this.dataLayer = dataLayer;
				this.handler = handler;
			}
			public IDataServerSecurity CreateAndLogonSecurity(IClientInfo clientInfo) {
				if(handler == null) {
					return null;
				}
				else {
					IDataServerSecurity result = handler();
					if(AnonymousLogonParameters.Instance.Equals(clientInfo.LogonParameters)) {
					}
					else {
						result.SetLogonParameters(clientInfo.LogonParameters);
						ITypesInfo typesInfo = XafTypesInfo.Instance;
						XpoTypeInfoSource xpoTypeInfoSource = (XpoTypeInfoSource)((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(XpoTypeInfoSource));
						result.Logon(new XPObjectSpace(typesInfo, xpoTypeInfoSource, CreateUnitOfWork));
					}
					SecuritySystem.SetInstance((ISecurityStrategyBase)result);
					return result;
				}
			}
		}
		private void Initialize(IServerSecurity serverSecurity, ISecuredSerializableObjectLayer securedSerializableObjectLayer) {
			Guard.ArgumentNotNull(serverSecurity, "serverSecurity");
			Guard.ArgumentNotNull(securedSerializableObjectLayer, "securedSerializableObjectLayer");
			this.ServerSecurity = serverSecurity;
			this.SecuredSerializableObjectLayer = securedSerializableObjectLayer;
		}
		protected virtual IServerSecurity CreateDefaultServerSecurity(RequestSecurityStrategyProvider securityStrategyProvider) { 
			return new ServerSecurity(securityStrategyProvider);
		}
		protected virtual ISecuredSerializableObjectLayer CreateDefaultSecuredSerializableObjectLayer(IDataLayer dataLayer, RequestSecurityStrategyProvider securityStrategyProvider, EventHandler<DataServiceOperationEventArgs> committingDelegate, bool allowICommandChannelDoWithSecurityContext) {
			SecuredSerializableObjectLayer objectLayer = new SecuredSerializableObjectLayer(dataLayer, securityStrategyProvider, allowICommandChannelDoWithSecurityContext);
			objectLayer.Committing += delegate(object sender, DataServiceOperationEventArgs args) {
				if(committingDelegate != null) {
					committingDelegate(this, args);
				}
			};
			return objectLayer;
		}
		protected virtual IServerSecurity CreateDefaultServerSecurityLogger(IServerSecurity serverSecurity, ILogger logger) {
			return new ServerSecurityLogger(serverSecurity, logger);
		}
		protected virtual ISecuredSerializableObjectLayer CreateDefaultSecuredSerializableObjectLayerLogger(ISecuredSerializableObjectLayer securedSerializableObjectLayer, ILogger logger) {
			return new SecuredSerializableObjectLayerLogger(securedSerializableObjectLayer, logger);
		}
		public SecuredDataServer(IServerSecurity serverSecurity, ISecuredSerializableObjectLayer securedSerializableObjectLayer) {
			Initialize(serverSecurity, securedSerializableObjectLayer);
		}
		public SecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler, ILogger logger, EventHandler<DataServiceOperationEventArgs> committingDelegate, bool allowICommandChannelDoWithSecurityContext) {
			RequestSecurityStrategyProvider securityStrategyProvider = new RequestSecurityStrategyProvider(dataLayer, querySecurityEnvironmentHandler);
			IServerSecurity serverSecurity = CreateDefaultServerSecurity(securityStrategyProvider);
			ISecuredSerializableObjectLayer objectLayer = CreateDefaultSecuredSerializableObjectLayer(dataLayer, securityStrategyProvider, committingDelegate, allowICommandChannelDoWithSecurityContext);
			if(logger != null) {
				serverSecurity = CreateDefaultServerSecurityLogger(serverSecurity, logger);
				objectLayer = CreateDefaultSecuredSerializableObjectLayerLogger(objectLayer, logger);
			}
			Initialize(serverSecurity, objectLayer);
		}
		public SecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler, ILogger logger) 
			: this(dataLayer, querySecurityEnvironmentHandler, logger, null, false) { }
		public SecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler) 
			: this(dataLayer, querySecurityEnvironmentHandler, null) { }
		public SecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider, ILogger logger, EventHandler<DataServiceOperationEventArgs> committingDelegate)
			: this(new ThreadSafeDataLayer(dictionary, XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists)), securityEnvironmentProvider, logger, committingDelegate, false) {
		}
		public SecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider, ILogger logger)
			: this(connectionString, dictionary, securityEnvironmentProvider, logger, null) {
		}
		public SecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider) 
			: this(connectionString, dictionary, securityEnvironmentProvider, null) { }
		public IServerSecurity ServerSecurity { get; private set; }
		public ISecuredSerializableObjectLayer SecuredSerializableObjectLayer { get; private set; }
		#region ISecuredSerializableObjectLayer2
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return SecuredSerializableObjectLayer.LoadObjects(clientInfo, dictionary, queries);
		}
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return SecuredSerializableObjectLayer.CommitObjects(clientInfo, dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return SecuredSerializableObjectLayer.GetObjectsByKey(clientInfo, dictionary, queries);
		}
		public object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return SecuredSerializableObjectLayer.SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
		}
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.CanLoadCollectionObjects(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return SecuredSerializableObjectLayer.LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
		}
		public PurgeResult Purge(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.Purge(clientInfo);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return SecuredSerializableObjectLayer.LoadDelayedProperties(clientInfo, dictionary, theObject, props);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return SecuredSerializableObjectLayer.LoadDelayedProperties(clientInfo, dictionary, objects, property);
		}
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return SecuredSerializableObjectLayer.IsParentObjectToSave(clientInfo, dictionary, theObject);
		}
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return SecuredSerializableObjectLayer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.GetParentObjectsToSave(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.GetParentObjectsToDelete(clientInfo);
		}
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.GetParentTouchedClassInfos(clientInfo);
		}
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			SecuredSerializableObjectLayer.CreateObjectType(clientInfo, assemblyName, typeName);
		}
		public object Do(IClientInfo clientInfo, string command, object args) {
			return SecuredSerializableObjectLayer.Do(clientInfo, command, args);
		}
		public void FinalizeSession(IClientInfo clientInfo) {
			SecuredSerializableObjectLayer.FinalizeSession(clientInfo);
		}
		#endregion
		#region IServerSecurity
		public void Logon(IClientInfo clientInfo) {
			ServerSecurity.Logon(clientInfo);
		}
		public void Logoff(IClientInfo clientInfo) {
			ServerSecurity.Logoff(clientInfo);
		}
		public bool IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest) {
			return ServerSecurity.IsGranted(clientInfo, permissionRequest);
		}
		public IList<bool> IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests) {
			return ServerSecurity.IsGranted(clientInfo, permissionRequests);
		}
		public object GetUserId(IClientInfo clientInfo) {
			return ServerSecurity.GetUserId(clientInfo);
		}
		public string GetUserName(IClientInfo clientInfo) {
			return ServerSecurity.GetUserName(clientInfo);
		}
		public object GetLogonParameters(IClientInfo clientInfo) {
			return ServerSecurity.GetLogonParameters(clientInfo);
		}
		public bool GetNeedLogonParameters(IClientInfo clientInfo) {
			return ServerSecurity.GetNeedLogonParameters(clientInfo);
		}
		public bool GetIsLogoffEnabled(IClientInfo clientInfo) {
			return ServerSecurity.GetIsLogoffEnabled(clientInfo);
		}
		public Type GetUserType(IClientInfo clientInfo) {
			return ServerSecurity.GetUserType(clientInfo);
		}
		public Type GetRoleType(IClientInfo clientInfo) {
			return ServerSecurity.GetRoleType(clientInfo);
		}
		#endregion
	}
}
