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
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer.Remoting {
	public class RemoteSecuredDataServer : MarshalByRefObject, IDataServer {
		private static IDataServer dataServer;
		public static void Initialize(IDataServer dataServer) {
			RemoteSecuredDataServer.dataServer = dataServer;
		}
		#region ISecuredSerializableObjectLayer2
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return dataServer.LoadObjects(clientInfo, dictionary, queries);
		}
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return dataServer.CommitObjects(clientInfo, dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return dataServer.GetObjectsByKey(clientInfo, dictionary, queries);
		}
		public object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return dataServer.SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
		}
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return dataServer.CanLoadCollectionObjects(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return dataServer.LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
		}
		public PurgeResult Purge(IClientInfo clientInfo) {
			return dataServer.Purge(clientInfo);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return dataServer.LoadDelayedProperties(clientInfo, dictionary, theObject, props);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return dataServer.LoadDelayedProperties(clientInfo, dictionary, objects, property);
		}
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return dataServer.IsParentObjectToSave(clientInfo, dictionary, theObject);
		}
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return dataServer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo) {
			return dataServer.GetParentObjectsToSave(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo) {
			return dataServer.GetParentObjectsToDelete(clientInfo);
		}
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return dataServer.GetParentTouchedClassInfos(clientInfo);
		}
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			dataServer.CreateObjectType(clientInfo, assemblyName, typeName);
		}
		public object Do(IClientInfo clientInfo, string command, object args) {
			return dataServer.Do(clientInfo, command, args);
		}
		public void FinalizeSession(IClientInfo clientInfo) {
			dataServer.FinalizeSession(clientInfo);
		}
		#endregion
		#region IServerSecurity
		public void Logon(IClientInfo clientInfo) {
			dataServer.Logon(clientInfo);
		}
		public void Logoff(IClientInfo clientInfo) {
			dataServer.Logoff(clientInfo);
		}
		public bool IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest) {
			return dataServer.IsGranted(clientInfo, permissionRequest);
		}
		public IList<bool> IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests) {
			return dataServer.IsGranted(clientInfo, permissionRequests);
		}
		public object GetUserId(IClientInfo clientInfo) {
			return dataServer.GetUserId(clientInfo);
		}
		public string GetUserName(IClientInfo clientInfo) {
			return dataServer.GetUserName(clientInfo);
		}
		public object GetLogonParameters(IClientInfo clientInfo) {
			return dataServer.GetLogonParameters(clientInfo);
		}
		public bool GetNeedLogonParameters(IClientInfo clientInfo) {
			return dataServer.GetNeedLogonParameters(clientInfo);
		}
		public bool GetIsLogoffEnabled(IClientInfo clientInfo) {
			return dataServer.GetIsLogoffEnabled(clientInfo);
		}
		public Type GetUserType(IClientInfo clientInfo) {
			return dataServer.GetUserType(clientInfo);
		}
		public Type GetRoleType(IClientInfo clientInfo) {
			return dataServer.GetRoleType(clientInfo);
		}
		#endregion
	}
}
