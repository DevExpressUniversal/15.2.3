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
	public interface IDataServer2 : ISecuredSerializableObjectLayer2, IServerSecurity {
	}
	public delegate IDataServer2 QueryDataServerHandler();
	public class SecuredDataServer2 : IDataServer2 {
		private QueryDataLayerHandler queryDataLayerHandler;
		private string connectionString;
		private XPDictionary dictionary;
		private QueryRequestSecurityStrategyHandler queryRequestSecurityStrategyHandler;
		private EventHandler<DataServiceOperationEventArgs> committingDelegate;
		private IDataLayer GetDataLayer() {
			IDataLayer dataLayer;
			if(queryDataLayerHandler != null) {
				dataLayer = queryDataLayerHandler();
				Guard.ArgumentNotNull(dataLayer, "QueryDataLayerHandler");
			}
			else {
				Guard.ArgumentNotNullOrEmpty(connectionString, "connectionString");
				Guard.ArgumentNotNull(dictionary, "dictionary");
				dataLayer = XpoDefault.GetDataLayer(connectionString, dictionary, AutoCreateOption.SchemaAlreadyExists);
			}
			return dataLayer;
		}
		private ServerSecurity2 CreateServerSecurity() {
			return new ServerSecurity2(GetDataLayer, queryRequestSecurityStrategyHandler);
		}
		private ISecuredSerializableObjectLayer2 SecuredSerializableObjectLayer { 
			get {
				QuerySelectDataSecurityHandler querySelectDataSecurityHandler = delegate(IClientInfo clientInfo) {
					ISelectDataSecurityProvider dataServerSecurity = CreateServerSecurity().GetSecurityAndLogon(clientInfo);
					Guard.ArgumentNotNull(dataServerSecurity, "dataServerSecurity");
					return dataServerSecurity.CreateSelectDataSecurity();
				};
				SecuredSerializableObjectLayer2 securedSerializableObjectLayer = new SecuredSerializableObjectLayer2(GetDataLayer, querySelectDataSecurityHandler);
				securedSerializableObjectLayer.Committing += delegate(object sender, DataServiceOperationEventArgs args) {
					if(committingDelegate != null) {
						committingDelegate(this, args);
					}
				};
				Guard.ArgumentNotNull(securedSerializableObjectLayer, "CreateSecuredSerializableObjectLayer()");
				return securedSerializableObjectLayer;
			}
		}
		private IServerSecurity ServerSecurity {
			get {
				return CreateServerSecurity();
			}
		}
		private SecuredDataServer2(QueryRequestSecurityStrategyHandler queryRequestSecurityStrategyHandler, EventHandler<DataServiceOperationEventArgs> committingDelegate) {
			this.queryRequestSecurityStrategyHandler = queryRequestSecurityStrategyHandler;
			this.committingDelegate = committingDelegate;
		}
		public SecuredDataServer2(QueryDataLayerHandler queryDataLayerHandler, QueryRequestSecurityStrategyHandler queryRequestSecurityStrategyHandler, EventHandler<DataServiceOperationEventArgs> committingDelegate)
			: this(queryRequestSecurityStrategyHandler, committingDelegate) {
			this.queryDataLayerHandler = queryDataLayerHandler;
		}
		public SecuredDataServer2(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler queryRequestSecurityStrategyHandler, EventHandler<DataServiceOperationEventArgs> committingDelegate)
			: this(queryRequestSecurityStrategyHandler, committingDelegate) {
			Guard.ArgumentNotNullOrEmpty(connectionString, "connectionString");
			Guard.ArgumentNotNull(dictionary, "dictionary");
			this.connectionString = connectionString;
			this.dictionary = dictionary;
		}
		#region ISecuredSerializableObjectLayer2
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return SecuredSerializableObjectLayer.LoadObjects(clientInfo, state, dictionary, queries);
		}
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, Dictionary<string, object> parameters, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return SecuredSerializableObjectLayer.CommitObjects(clientInfo, parameters, dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return SecuredSerializableObjectLayer.GetObjectsByKey(clientInfo, state, dictionary, queries);
		}
		public object[][] SelectData(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return SecuredSerializableObjectLayer.SelectData(clientInfo, state, dictionary, query, properties, groupProperties, groupCriteria);
		}
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.CanLoadCollectionObjects(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return SecuredSerializableObjectLayer.LoadCollectionObjects(clientInfo, state, dictionary, refPropertyName, ownerObject);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return SecuredSerializableObjectLayer.LoadDelayedProperties(clientInfo, state, dictionary, theObject, props);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return SecuredSerializableObjectLayer.LoadDelayedProperties(clientInfo, state, dictionary, objects, property);
		}
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return SecuredSerializableObjectLayer.IsParentObjectToSave(clientInfo, dictionary, theObject);
		}
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return SecuredSerializableObjectLayer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo, Dictionary<string, object> state) {
			return SecuredSerializableObjectLayer.GetParentObjectsToSave(clientInfo, state);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo, Dictionary<string, object> state) {
			return SecuredSerializableObjectLayer.GetParentObjectsToDelete(clientInfo, state);
		}
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return SecuredSerializableObjectLayer.GetParentTouchedClassInfos(clientInfo);
		}
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			SecuredSerializableObjectLayer.CreateObjectType(clientInfo, assemblyName, typeName);
		}
		public object Do(IClientInfo clientInfo, Dictionary<string, object> state, string command, object args) {
			return SecuredSerializableObjectLayer.Do(clientInfo, state, command, args);
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
