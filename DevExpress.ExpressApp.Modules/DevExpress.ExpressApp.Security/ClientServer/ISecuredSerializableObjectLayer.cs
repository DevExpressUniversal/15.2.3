﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public interface ISecuredSerializableObjectLayer {
		SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries);
		CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption);
		SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries);
		object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria);
		bool CanLoadCollectionObjects(IClientInfo clientInfo);
		SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject);
		PurgeResult Purge(IClientInfo clientInfo);
		SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props);
		SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property);
		bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject);
		bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject);
		SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo);
		SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo);
		string[] GetParentTouchedClassInfos(IClientInfo clientInfo);
		void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName);
		object Do(IClientInfo clientInfo, string command, object args);
		void FinalizeSession(IClientInfo clientInfo); 
	}
}
