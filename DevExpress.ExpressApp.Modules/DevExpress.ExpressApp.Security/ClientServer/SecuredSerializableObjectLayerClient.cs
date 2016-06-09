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
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public class SecuredSerializableObjectLayerClient : ISerializableObjectLayer, ISerializableObjectLayerProvider, ISerializableObjectLayerEx, ICommandChannel {
		private ISecuredSerializableObjectLayer securedObjectLayer;
		private IClientInfo clientInfo;
		public SecuredSerializableObjectLayerClient(IClientInfo clientInfo, ISecuredSerializableObjectLayer securedObjectLayer) {
			Guard.ArgumentNotNull(clientInfo, "clientInfo");
			Guard.ArgumentNotNull(securedObjectLayer, "securedObjectLayer");
			this.clientInfo = clientInfo;
			this.securedObjectLayer = securedObjectLayer;
		}
		#region ISerializableObjectLayer Members
		public bool CanLoadCollectionObjects {
			get { return securedObjectLayer.CanLoadCollectionObjects(clientInfo); }
		}
		public CommitObjectStubsResult[] CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return securedObjectLayer.CommitObjects(clientInfo, dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public void CreateObjectType(string assemblyName, string typeName) {
			securedObjectLayer.CreateObjectType(clientInfo, assemblyName, typeName);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return securedObjectLayer.GetObjectsByKey(clientInfo, dictionary, queries);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return securedObjectLayer.LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return securedObjectLayer.LoadObjects(clientInfo, dictionary, queries);
		}
		public DevExpress.Xpo.Helpers.PurgeResult Purge() {
			return securedObjectLayer.Purge(clientInfo);
		}
		public object[][] SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, DevExpress.Data.Filtering.CriteriaOperatorCollection groupProperties, DevExpress.Data.Filtering.CriteriaOperator groupCriteria) {
			return securedObjectLayer.SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
		}
		public ISerializableObjectLayer ObjectLayer {
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region ISerializableObjectLayerEx Members
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete() {
			return securedObjectLayer.GetParentObjectsToDelete(clientInfo);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave() {
			return securedObjectLayer.GetParentObjectsToSave(clientInfo);
		}
		public string[] GetParentTouchedClassInfos() {
			return securedObjectLayer.GetParentTouchedClassInfos(clientInfo);
		}
		public bool IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return securedObjectLayer.IsParentObjectToDelete(clientInfo, dictionary, theObject);
		}
		public bool IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return securedObjectLayer.IsParentObjectToSave(clientInfo, dictionary, theObject);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return securedObjectLayer.LoadDelayedProperties(clientInfo, dictionary, objects, property);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return securedObjectLayer.LoadDelayedProperties(clientInfo, dictionary, theObject, props);
		}
		#endregion
		#region ICommandChannel Members
		public object Do(string command, object args) {
			return securedObjectLayer.Do(clientInfo, command, args);
		}
		#endregion
	}
}
