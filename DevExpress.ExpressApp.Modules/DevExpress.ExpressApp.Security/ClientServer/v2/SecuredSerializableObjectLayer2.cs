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
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public delegate IDataLayer QueryDataLayerHandler();
	public delegate ISelectDataSecurity QuerySelectDataSecurityHandler(IClientInfo clientInfo);
	[Serializable]
	public class DuplicateDictionaryItemException : Exception {
		public DuplicateDictionaryItemException(string key) {
			Key = key;
		}
		public string Key { get; set; }
	}
	public enum DuplicateDictionaryItemMode { Override, KeepExisting, ThrowException }
	public class DictionaryHelper {
		public static void AssignDictionaryItems<TValue>(Dictionary<string, TValue> sourceParameters, Dictionary<string, TValue> targetParameters, DuplicateDictionaryItemMode copyMode) {
			if(sourceParameters != null && targetParameters != null) {
				foreach(string key in sourceParameters.Keys) {
					if(!targetParameters.ContainsKey(key)) { 
						targetParameters[key] = sourceParameters[key];
					}
					else {
						if(copyMode == DuplicateDictionaryItemMode.ThrowException) {
							throw new DuplicateDictionaryItemException(key.ToString());
						}
					}
				}
			}
		}
		public static void AssignDictionaryItems(Dictionary<string, object> sourceParameters, Dictionary<string, object> targetParameters, DuplicateDictionaryItemMode copyMode) {
			AssignDictionaryItems<object>(sourceParameters, targetParameters, copyMode);
		}
		public static void AssignDictionaryItems(Dictionary<string, object> sourceParameters, Dictionary<string, object> targetParameters) {
			AssignDictionaryItems(sourceParameters, targetParameters, DuplicateDictionaryItemMode.KeepExisting);
		}
		public static bool TryGetValue<T>(Dictionary<string, object> dictionary, string key, out T itemValue) {
			object val;
			if(dictionary.TryGetValue(key, out val)) {
				itemValue = (T)val;
				return true;
			}
			itemValue = default(T);
			return false;
		}
	}
	public enum SecurityCannotSaveChangesReason { MemberReadPermissionsChanged }
	[Serializable]
	public class SecurityCannotSaveChangesException : Exception {
		public SecurityCannotSaveChangesException() {
			Reason = SecurityCannotSaveChangesReason.MemberReadPermissionsChanged;
		}
		public SecurityCannotSaveChangesReason Reason { get; set; }
	}
	public class SecuredSerializableObjectLayer2 : ISecuredSerializableObjectLayer2, IApplicationDataService {
		public const string ReadMemberPermissionVersionParameterName = "{5838BFB3-1A27-4355-8A86-93A6F15DD866}"; 
		private readonly QueryDataLayerHandler queryDataLayerHandler;
		private readonly QuerySelectDataSecurityHandler querySelectDataSecurityHandler;
		private static bool TryGetParamValue<T>(Dictionary<string, object> parameters, string parameterName, out T paramValue) {
			object val;
			if(parameters.TryGetValue(parameterName, out val)) {
				paramValue = (T)val;
				return true;
			}
			paramValue = default(T);
			return false;
		}
		private void PutCurrentReadPermissionVersionToOut(Dictionary<string, object> inputState, Dictionary<string, object> outState, ISelectDataSecurity security) {
			IReadPermissionVersionProvider readPermissionVersionProvider = security as IReadPermissionVersionProvider;
			if(readPermissionVersionProvider != null) {
				Dictionary<string, int> readMembersInputState;
				if(!DictionaryHelper.TryGetValue<Dictionary<string, int>>(inputState, ReadMemberPermissionVersionParameterName, out readMembersInputState)) {
					readMembersInputState = new Dictionary<string, int>();
				}
				outState[ReadMemberPermissionVersionParameterName] = readPermissionVersionProvider.CompleteVersion(readMembersInputState);
			}
			else {
				outState[ReadMemberPermissionVersionParameterName] = new Dictionary<string, int>();
			}
		}
		private void CheckReadPermissionVersion(List<string> classNames, Dictionary<string, object> initialState, Dictionary<string, object> outState, ISelectDataSecurity security) {
			IReadPermissionVersionProvider readPermissionVersionProvider = security as IReadPermissionVersionProvider;
			if((readPermissionVersionProvider != null) && (initialState != null) && (initialState.Count > 0) && (outState != null) && (outState.Count > 0)) {
				Dictionary<string, int> initialReadPermissionVersion;
				Dictionary<string, int> currentReadPermissionVersion;
				if(TryGetParamValue(initialState, ReadMemberPermissionVersionParameterName, out initialReadPermissionVersion) 
						&& TryGetParamValue(outState, ReadMemberPermissionVersionParameterName, out currentReadPermissionVersion)) {
					if(!readPermissionVersionProvider.CompareReadPermissionVersion(classNames, initialReadPermissionVersion, currentReadPermissionVersion)) {
						throw new SecurityCannotSaveChangesException();
					}
				}
			}
		}
		private SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo,
				Dictionary<string, object> inputState, Dictionary<string, object> outState,
				out UnitOfWork result_parentSession, out ISelectDataSecurity selectDataSecurity) {
			selectDataSecurity = querySelectDataSecurityHandler(clientInfo);
			Guard.ArgumentNotNull(selectDataSecurity, "querySelectDataSecurityHandler");
			IDataLayer dataLayer = queryDataLayerHandler();
			UnitOfWork directSession = new UnitOfWork(dataLayer);
			SecurityRuleProvider securityRuleProvider = new SecurityRuleProvider(dataLayer.Dictionary, selectDataSecurity);
			SessionObjectLayer sessionObjectLayer = new SessionObjectLayer(directSession, true, null, securityRuleProvider, null);
			result_parentSession = new UnitOfWork(sessionObjectLayer);
			ISecurityRule securityRule = securityRuleProvider.securityRule;
			PutCurrentReadPermissionVersionToOut(inputState, outState, selectDataSecurity);
			return new SerializableObjectLayer(result_parentSession, true);
		}
		private SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo, Dictionary<string, object> inputState, Dictionary<string, object> outState) {
			UnitOfWork result_parentSession;
			ISelectDataSecurity selectDataSecurity;
			return GetSerializableObjectLayer(clientInfo, inputState, outState, out result_parentSession, out selectDataSecurity);
		}
		private SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo, new Dictionary<string, object>(), new Dictionary<string, object>());
		}
		public SecuredSerializableObjectLayer2(QueryDataLayerHandler queryDataLayerHandler, QuerySelectDataSecurityHandler querySelectDataSecurityHandler) {
			Guard.ArgumentNotNull(queryDataLayerHandler, "queryDataLayerHandler");
			Guard.ArgumentNotNull(querySelectDataSecurityHandler, "querySelectDataSecurityHandler");
			this.queryDataLayerHandler = queryDataLayerHandler;
			this.querySelectDataSecurityHandler = querySelectDataSecurityHandler;
		}
		#region ISecuredSerializableObjectLayer2
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, Dictionary<string, object> parameters, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<XPObjectStubCollection[]> result = GetSerializableObjectLayer(clientInfo, parameters, outState).LoadObjects(dictionary, queries);
			DictionaryHelper.AssignDictionaryItems(outState, parameters, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			UnitOfWork session;
			ISelectDataSecurity selectDataSecurity;
			SerializableObjectLayer ol = GetSerializableObjectLayer(clientInfo, state, outState, out session, out selectDataSecurity);
			List<string> classNames = new List<string>();
			foreach(XPObjectStub stub in objectsForSave) {
				if(!classNames.Contains(stub.ClassName)) {
					classNames.Add(stub.ClassName);
				}
			}
			CheckReadPermissionVersion(classNames, state, outState, selectDataSecurity);
			CommitObjectStubsResult[] result;
			if(Committing != null) {
				if(XafTypesInfo.Instance == null) {
					throw new InvalidOperationException("XafTypesInfo.Instance is null.");
				}
				XpoTypeInfoSource xpoTypeInfoSource = (XpoTypeInfoSource)((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(XpoTypeInfoSource));
				if(xpoTypeInfoSource == null) {
					throw new InvalidOperationException("XpoTypeInfoSource is not found.");
				}
				IObjectSpace objectSpace = new ObjectSpaceWrapper(XafTypesInfo.Instance, xpoTypeInfoSource, () => { return session; });
				try {
					EventHandler startHandler = delegate(object sender, EventArgs args) {
						if(Committing != null) {
							Committing(this, (DataServiceOperationEventArgs)sender);
						}
					};
					DataServiceOperationEventArgs operationArgs = new UnitOfWorkDataServiceOperationEventArgs(objectSpace, session, startHandler);
					try {
						result = InternalCommitChanges(dictionary, objectsForDelete, objectsForSave, lockingOption, session, ol);
					}
					finally {
						operationArgs.Dispose();
					}
				}
				finally {
					objectSpace.Dispose();
				}
			}
			else {
				result = InternalCommitChanges(dictionary, objectsForDelete, objectsForSave, lockingOption, session, ol);
			}
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		private static CommitObjectStubsResult[] InternalCommitChanges(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption, UnitOfWork session, SerializableObjectLayer ol) {
			CommitObjectStubsResult[] result;
			result = ol.CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption);
			session.CommitChanges();
			return result;
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<XPObjectStubCollection[]> result = GetSerializableObjectLayer(clientInfo, state, outState).GetObjectsByKey(dictionary, queries);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public object[][] SelectData(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			object[][] result = GetSerializableObjectLayer(clientInfo, state, outState).SelectData(dictionary, query, properties, groupProperties, groupCriteria);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).CanLoadCollectionObjects;
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<XPObjectStubCollection> result = GetSerializableObjectLayer(clientInfo, state, outState).LoadCollectionObjects(dictionary, refPropertyName, ownerObject);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<object[]> result = GetSerializableObjectLayer(clientInfo, state, outState).LoadDelayedProperties(dictionary, theObject, props);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> state, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<object[]> result = GetSerializableObjectLayer(clientInfo, state, outState).LoadDelayedProperties(dictionary, objects, property);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetSerializableObjectLayer(clientInfo).IsParentObjectToSave(dictionary, theObject);
		}
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetSerializableObjectLayer(clientInfo).IsParentObjectToDelete(dictionary, theObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo, Dictionary<string, object> state) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<XPObjectStubCollection> result = GetSerializableObjectLayer(clientInfo, state, outState).GetParentObjectsToSave();
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo, Dictionary<string, object> state) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			SerializableObjectLayerResult<XPObjectStubCollection> result = GetSerializableObjectLayer(clientInfo, state, outState).GetParentObjectsToDelete();
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).GetParentTouchedClassInfos();
		}
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			GetSerializableObjectLayer(clientInfo).CreateObjectType(assemblyName, typeName);
		}
		public object Do(IClientInfo clientInfo, Dictionary<string, object> state, string command, object args) {
			Dictionary<string, object> outState = new Dictionary<string, object>();
			object result = ((ICommandChannel)GetSerializableObjectLayer(clientInfo, state, outState)).Do(command, args);
			DictionaryHelper.AssignDictionaryItems(outState, state, DuplicateDictionaryItemMode.Override);
			return result;
		}
		#endregion
		public event EventHandler<DataServiceOperationEventArgs> Committing;
	}
}
