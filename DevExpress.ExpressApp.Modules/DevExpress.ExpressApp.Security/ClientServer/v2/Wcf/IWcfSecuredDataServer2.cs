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
using System.ServiceModel;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.MiddleTier;
using System.Reflection;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer.Wcf {
	[ServiceContract]
	[ServiceKnownType("GetKnownTypes", typeof(WcfDataServerHelper))]
	public interface IWcfSecuredDataServer2 {
		#region ISecuredSerializableObjectLayer2
		[OperationContract]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(XPObjectStub))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[SetMaxItemsInObjectGraph]
		SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, ObjectStubsQuery[] queries);
		[OperationContract]
		[ServiceKnownType(typeof(XPObjectStub))]
		[SetMaxItemsInObjectGraph]
		CommitObjectStubsResult[] CommitObjects(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption);
		[OperationContract]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(XPObjectStub))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[SetMaxItemsInObjectGraph]
		SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries);
		[OperationContract]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[SetMaxItemsInObjectGraph]
		object[][] SelectData(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria);
		[OperationContract]
		[SetMaxItemsInObjectGraph]
		bool CanLoadCollectionObjects(IClientInfo clientInfo);
		[OperationContract]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(XPObjectStub))]
		[SetMaxItemsInObjectGraph]
		SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject);
		[SetMaxItemsInObjectGraph]
		[OperationContract(Name = "LoadDelayedPropertiesAnother")]
		SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		bool IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		bool IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		[ServiceKnownType(typeof(XPObjectStub))]
		SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		[ServiceKnownType(typeof(XPObjectStub))]
		SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState);
		[OperationContract]
		[SetMaxItemsInObjectGraph]
		string[] GetParentTouchedClassInfos(IClientInfo clientInfo);
		[SetMaxItemsInObjectGraph]
		[OperationContract]
		void CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName);
		[OperationContract]
		[SetMaxItemsInObjectGraph]
		object Do(IClientInfo clientInfo, Dictionary<string, object> inputState, out Dictionary<string, object> outState, string command, object args);
		#endregion
		#region IServerSecurity
		[OperationContract]
		void Logon(IClientInfo clientInfo);
		[OperationContract]
		void Logoff(IClientInfo clientInfo);
		[OperationContract(Name = "IsGranted")]
		[ServiceKnownType(typeof(ModelOperationPermissionRequest))]
		[ServiceKnownType(typeof(SerializablePermissionRequest))]
		bool IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest);
		[OperationContract(Name = "IsGrantedFromList")]
		[ServiceKnownType(typeof(ModelOperationPermissionRequest))]
		[ServiceKnownType(typeof(SerializablePermissionRequest))]
		IList<bool> IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests);
		[OperationContract]
		object GetUserId(IClientInfo clientInfo);
		[OperationContract]
		string GetUserName(IClientInfo clientInfo);
		[OperationContract]
		object GetLogonParameters(IClientInfo clientInfo); 
		[OperationContract]
		bool GetNeedLogonParameters(IClientInfo clientInfo);
		[OperationContract]
		bool GetIsLogoffEnabled(IClientInfo clientInfo);
		[OperationContract]
		string GetUserTypeName(IClientInfo clientInfo);
		[OperationContract]
		string GetRoleTypeName(IClientInfo clientInfo);
		#endregion
	}
}
