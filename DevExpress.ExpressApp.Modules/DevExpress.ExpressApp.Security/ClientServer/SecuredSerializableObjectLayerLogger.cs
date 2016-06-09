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
using DevExpress.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public class SecuredSerializableObjectLayerLogger : LoggerBase, ISecuredSerializableObjectLayer {
		#region constants
		public const int CanLoadCollectionObjectsCode = 200;
		public const int CommitObjectsCode = 201;
		public const int CreateObjectTypeCode = 202;
		public const int GetObjectsByKeyCode = 203;
		public const int LoadCollectionObjectsCode = 204;
		public const int LoadObjectsCode = 205;
		public const int PurgeCode = 206;
		public const int SelectDataCode = 207;
		public const int GetParentObjectsToDeleteCode = 208;
		public const int GetParentObjectsToSaveCode = 209;
		public const int GetParentTouchedClassInfosCode = 210;
		public const int IsParentObjectToDeleteCode = 211;
		public const int IsParentObjectToSaveCode = 212;
		public const int LoadDelayedPropertyCode = 213;
		public const int LoadDelayedPropertiesCode = 214;
		public const int DoCode = 215;
		public const int DropCacheCode = 216;
		public const int FinalizeSessionCode = 217;
		#endregion
		private readonly ISecuredSerializableObjectLayer layer;
		private string GetTextDescription(XPObjectStubCollection objectsToDescribe) {
			StringBuilder result = new StringBuilder();
			foreach(XPObjectStub stub in objectsToDescribe) {
				result.Append(stub.ToString());
			}
			return result.ToString();
		}
		private string GetTextDescription(XPDictionaryStub dictionary) {
			if(dictionary != null && dictionary.ClassInfoList != null) {
				StringBuilder result = new StringBuilder();
				for(uint i = 0; i < dictionary.ClassInfoList.Length; ++i) {
					result.Append(dictionary.ClassInfoList[i] + "; ");
				}
				return result.ToString();
			}
			return string.Empty;
		}
		public SecuredSerializableObjectLayerLogger(ISecuredSerializableObjectLayer layer, ILogger logger) : base(logger) {
			Guard.ArgumentNotNull(layer, "layer");
			this.layer = layer;
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).AppendLine(GetTextDescription(dictionary));
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() {
				return layer.LoadObjects(clientInfo, dictionary, queries);
			}, "LoadObjects", LoadObjectsCode, clientInfo, additionalLogData.ToString());
		}
		public CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.ObjectsForDelete)).AppendLine(GetTextDescription(objectsForDelete));
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.ObjectsForSave)).AppendLine(GetTextDescription(objectsForSave));
			return Execute<CommitObjectStubsResult[]>(delegate() {
				return layer.CommitObjects(clientInfo, dictionary, objectsForDelete, objectsForSave, lockingOption);
			}, "CommitObjects", CommitObjectsCode, clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.ObjectTypes)).AppendLine(GetTextDescription(dictionary));
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() {
				return layer.GetObjectsByKey(clientInfo, dictionary, queries);
			}, "GetObjectsByKey", GetObjectsByKeyCode, clientInfo, additionalLogData.ToString());
		}
		public object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).Append(GetTextDescription(dictionary));
			return Execute<object[][]>(delegate() {
				 return layer.SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
			}, "SelectData", SelectDataCode, clientInfo, additionalLogData.ToString());
		}
		public bool CanLoadCollectionObjects(IClientInfo clientInfo) {
			return Execute<bool>(delegate() {
				return layer.CanLoadCollectionObjects(clientInfo);
			}, "CanLoadCollectionObjects", CanLoadCollectionObjectsCode, clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			StringBuilder additionalLogData = new StringBuilder();
			if(dictionary != null) {
				additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).Append(GetTextDescription(dictionary)).Append("; " + ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.RefPropertyName)).Append(refPropertyName);
				if(ownerObject != null) {
					additionalLogData.Append("; " + ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.OwnerObject)).Append(ownerObject.ToString());
				}
			}
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() {
				return layer.LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
			}, "LoadCollectionObjects", 204, clientInfo, additionalLogData.ToString());
		}
		public PurgeResult Purge(IClientInfo clientInfo) {
			return Execute<PurgeResult>(delegate() {
				return layer.Purge(clientInfo);
			}, "Purge", PurgeCode, clientInfo);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).AppendLine(GetTextDescription(dictionary));
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Properties));
			if(theObject != null) {
				additionalLogData.AppendLine(theObject.ToString());
			}
			else {
				additionalLogData.AppendLine();
			}
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Properties));
			for(uint i = 0; i < props.Length; ++i) {
				additionalLogData.Append(props[i]).Append("; ");
			}
			return Execute<SerializableObjectLayerResult<object[]>>(delegate() {
				return layer.LoadDelayedProperties(clientInfo, dictionary, theObject, props);
			}, "LoadDelayedProperties", LoadDelayedPropertiesCode, clientInfo);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).AppendLine(GetTextDescription(dictionary));
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Objects));
			if(objects != null) {
				additionalLogData.AppendLine(GetTextDescription(objects));
			}
			else {
				additionalLogData.AppendLine();
			}
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Property)).AppendLine(property);
			return Execute<SerializableObjectLayerResult<object[]>>(delegate() {
				return layer.LoadDelayedProperties(clientInfo, dictionary, objects, property);
			}, "LoadDelayedProperties", LoadDelayedPropertyCode, clientInfo, additionalLogData.ToString());
		}
		public bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).AppendLine(GetTextDescription(dictionary));
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Object));
			if(theObject != null) {
				additionalLogData.AppendLine(theObject.ToString());
			}
			else {
				additionalLogData.AppendLine();
			}
			return Execute<bool>(delegate() {
				return layer.IsParentObjectToSave(clientInfo, dictionary, theObject);
			}, "IsParentObjectToSave", IsParentObjectToSaveCode, clientInfo);
		}
		public bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			StringBuilder additionalLogData = new StringBuilder();
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Dictionary)).AppendLine(GetTextDescription(dictionary));
			additionalLogData.Append(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.Object));
			if(theObject != null) {
				additionalLogData.AppendLine(theObject.ToString());
			}
			else {
				additionalLogData.AppendLine();
			}
			return Execute<bool>(delegate() {
				return layer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
			}, "IsParentObjectToDelete", IsParentObjectToDeleteCode, clientInfo, additionalLogData.ToString());
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo) {
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() {
				return layer.GetParentObjectsToSave(clientInfo);
			}, "GetParentObjectsToSave", GetParentObjectsToSaveCode, clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo) {
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() {
				return layer.GetParentObjectsToDelete(clientInfo);
			}, "GetParentObjectsToDelete", GetParentObjectsToDeleteCode, clientInfo);
		}
		public string[] GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return Execute<string[]>(delegate() {
				return layer.GetParentTouchedClassInfos(clientInfo);
			}, "GetParentTouchedClassInfos", GetParentTouchedClassInfosCode, clientInfo);
		}
		public void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			Execute<object>(delegate() {
				layer.CreateObjectType(clientInfo, assemblyName, typeName);
				return null;
			}, "CreateObjectType", CreateObjectTypeCode, clientInfo, typeName + "; " + assemblyName);
		}
		public object Do(IClientInfo clientInfo, string command, object args) {
			string infoMessage = "Command: " + command + "; Arguments: " + args;
			return Execute<object>(delegate() {
				return layer.Do(clientInfo, command, args);
			}, "Do", DoCode, clientInfo, infoMessage);
		}
		public void FinalizeSession(IClientInfo clientInfo) {
			Execute<object>(delegate() {
				layer.FinalizeSession(clientInfo);
				return null;
			}, "FinalizeSession", FinalizeSessionCode, clientInfo);
		}
	}
}
