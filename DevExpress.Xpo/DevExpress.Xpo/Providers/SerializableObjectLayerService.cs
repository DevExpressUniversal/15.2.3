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

#if !CF
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using System.Configuration;
using System.Xml.Serialization;
using DevExpress.Xpo.DB.Exceptions;
using System.ServiceModel.Channels;
using System.ComponentModel;
#if !SL
using System.ServiceModel.Activation;
#endif
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo.DB {
	#region SerializableObjectLayerService
	[ServiceKnownType(typeof(IdList))]
	[ServiceContract, XmlSerializerFormat]
	public interface ISerializableObjectLayerService {
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/GetCanLoadCollectionObjects")]
		OperationResult<bool> GetCanLoadCollectionObjects();
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/CommitObjects")]
		OperationResult<CommitObjectStubsResult[]> CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/CreateObjectType")]
		OperationResult<object> CreateObjectType(string assemblyName, string typeName);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/GetObjectsByKey")]
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/LoadCollectionObjects")]
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/LoadObjects")]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(XPObjectStub))]
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/Purge")]
		OperationResult<PurgeResult> Purge();
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/SelectData")]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		OperationResult<object[][]> SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentObjectsToDelete")]
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> GetParentObjectsToDelete();
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentObjectsToSave")]
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> GetParentObjectsToSave();
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentTouchedClassInfos")]
		OperationResult<string[]> GetParentTouchedClassInfos();
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/IsParentObjectToDelete")]
		OperationResult<bool> IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/IsParentObjectToSave")]
		OperationResult<bool> IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/LoadDelayedProperties")]
		OperationResult<SerializableObjectLayerResult<object[]>> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props);
		[OperationContract(Action = "http://tempuri.org/ISerializableObjectLayerService/LoadDelayedProperty")]
		OperationResult<SerializableObjectLayerResult<object[]>> LoadDelayedProperty(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property);
	}
#if SL
	[ServiceContract, XmlSerializerFormat]
	public interface ISerializableObjectLayerServiceClientAsync {
		[XmlSerializerFormat, OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/GetCanLoadCollectionObjects", ReplyAction = "*")]
		IAsyncResult BeginGetCanLoadCollectionObjects(AsyncCallback callback, object asyncState);
		OperationResult<bool> EndGetCanLoadCollectionObjects(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/CommitObjects", ReplyAction = "*")]
		IAsyncResult BeginCommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption, AsyncCallback callback, object asyncState);
		OperationResult<CommitObjectStubsResult[]> EndCommitObjects(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/CreateObjectType", ReplyAction = "*")]
		IAsyncResult BeginCreateObjectType(string assemblyName, string typeName, AsyncCallback callback, object asyncState);
		OperationResult<object> EndCreateObjectType(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/GetObjectsByKey", ReplyAction = "*")]
		IAsyncResult BeginGetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries, AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> EndGetObjectsByKey(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/LoadCollectionObjects", ReplyAction = "*")]
		IAsyncResult BeginLoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject, AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndLoadCollectionObjects(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/LoadObjects", ReplyAction = "*")]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		[ServiceKnownType(typeof(XPObjectStub))]
		IAsyncResult BeginLoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries, AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> EndLoadObjects(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/Purge", ReplyAction = "*")]
		IAsyncResult BeginPurge(AsyncCallback callback, object asyncState);
		OperationResult<PurgeResult> EndPurge(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/SelectData", ReplyAction = "*")]
		[ServiceKnownType(typeof(DeleteStatement))]
		[ServiceKnownType(typeof(InsertStatement))]
		[ServiceKnownType(typeof(UpdateStatement))]
		[ServiceKnownType(typeof(AggregateOperand))]
		[ServiceKnownType(typeof(BetweenOperator))]
		[ServiceKnownType(typeof(BinaryOperator))]
		[ServiceKnownType(typeof(ContainsOperator))]
		[ServiceKnownType(typeof(FunctionOperator))]
		[ServiceKnownType(typeof(GroupOperator))]
		[ServiceKnownType(typeof(InOperator))]
		[ServiceKnownType(typeof(NotOperator))]
		[ServiceKnownType(typeof(NullOperator))]
		[ServiceKnownType(typeof(OperandProperty))]
		[ServiceKnownType(typeof(OperandValue))]
		[ServiceKnownType(typeof(ConstantValue))]
		[ServiceKnownType(typeof(XPStubOperandValue))]
		[ServiceKnownType(typeof(ParameterValue))]
		[ServiceKnownType(typeof(QueryOperand))]
		[ServiceKnownType(typeof(UnaryOperator))]
		[ServiceKnownType(typeof(JoinOperand))]
		[ServiceKnownType(typeof(OperandParameter))]
		[ServiceKnownType(typeof(QuerySubQueryContainer))]
		IAsyncResult BeginSelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, AsyncCallback callback, object asyncState);
		OperationResult<object[][]> EndSelectData(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentObjectsToDelete", ReplyAction = "*")]
		IAsyncResult BeginGetParentObjectsToDelete(AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndGetParentObjectsToDelete(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentObjectsToSave", ReplyAction = "*")]
		IAsyncResult BeginGetParentObjectsToSave(AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndGetParentObjectsToSave(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/GetParentTouchedClassInfos", ReplyAction = "*")]
		IAsyncResult BeginGetParentTouchedClassInfos(AsyncCallback callback, object asyncState);
		OperationResult<string[]> EndGetParentTouchedClassInfos(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/IsParentObjectToDelete", ReplyAction = "*")]
		IAsyncResult BeginIsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject, AsyncCallback callback, object asyncState);
		OperationResult<bool> EndIsParentObjectToDelete(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/IsParentObjectToSave", ReplyAction = "*")]
		IAsyncResult BeginIsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject, AsyncCallback callback, object asyncState);
		OperationResult<bool> EndIsParentObjectToSave(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/LoadDelayedProperties", ReplyAction = "*")]
		IAsyncResult BeginLoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props, AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<object[]>> EndLoadDelayedProperties(IAsyncResult result);
		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/ISerializableObjectLayerService/LoadDelayedProperty", ReplyAction = "*")]
		IAsyncResult BeginLoadDelayedProperty(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property, AsyncCallback callback, object asyncState);
		OperationResult<SerializableObjectLayerResult<object[]>> EndLoadDelayedProperty(IAsyncResult result);
	}
#endif
#if !SL
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
#endif
	public class SerializableObjectLayerService : ServiceBase, ISerializableObjectLayerService {
		ISerializableObjectLayer serializableObjectLayer;
#if !SL
	[DevExpressXpoLocalizedDescription("SerializableObjectLayerServiceSerializableObjectLayer")]
#endif
public ISerializableObjectLayer SerializableObjectLayer {
			get { return serializableObjectLayer; }
		}
		public SerializableObjectLayerService(ISerializableObjectLayer serializableObjectLayer) {
			this.serializableObjectLayer = serializableObjectLayer;
		}
		public virtual OperationResult<bool> GetCanLoadCollectionObjects() {
			return Execute<bool>(delegate() { return serializableObjectLayer.CanLoadCollectionObjects; });
		}
		public virtual OperationResult<CommitObjectStubsResult[]> CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return Execute<CommitObjectStubsResult[]>(delegate() { return serializableObjectLayer.CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption); });
		}
		public virtual OperationResult<object> CreateObjectType(string assemblyName, string typeName) {
			return Execute<object>(delegate() { serializableObjectLayer.CreateObjectType(assemblyName, typeName); return null; });
		}
		public virtual OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() { return serializableObjectLayer.GetObjectsByKey(dictionary, queries); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return Execute <SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return serializableObjectLayer.LoadCollectionObjects(dictionary, refPropertyName, ownerObject); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return Execute <SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() { return serializableObjectLayer.LoadObjects(dictionary, queries); });
		}
		public virtual OperationResult<PurgeResult> Purge() {
			return Execute<PurgeResult>(delegate() { return serializableObjectLayer.Purge(); });
		}
		public virtual OperationResult<object[][]> SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return Execute<object[][]>(delegate() { return serializableObjectLayer.SelectData(dictionary, query, properties, groupProperties, groupCriteria); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> GetParentObjectsToDelete() {
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).GetParentObjectsToDelete(); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> GetParentObjectsToSave() {
			return Execute<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).GetParentObjectsToSave(); });
		}
		public virtual OperationResult<string[]> GetParentTouchedClassInfos() {
			return Execute<string[]>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).GetParentTouchedClassInfos(); });
		}
		public virtual OperationResult<bool> IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return Execute<bool>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).IsParentObjectToDelete(dictionary, theObject); });
		}
		public virtual OperationResult<bool> IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return Execute<bool>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).IsParentObjectToSave(dictionary, theObject); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<object[]>> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return Execute<SerializableObjectLayerResult<object[]>>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).LoadDelayedProperties(dictionary, theObject, props); });
		}
		public virtual OperationResult<SerializableObjectLayerResult<object[]>> LoadDelayedProperty(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return Execute<SerializableObjectLayerResult<object[]>>(delegate() { return ((ISerializableObjectLayerEx)serializableObjectLayer).LoadDelayedProperties(dictionary, objects, property); });
		}
	}
#if !SL
	[ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.Single)]
	public class SerializableObjectLayerSingletonService : SerializableObjectLayerService {
		public SerializableObjectLayerSingletonService(ISerializableObjectLayer serializableObjectLayer) : base(serializableObjectLayer) { }
	}
#endif
	public class SerializableObjectLayerServiceClient: 
#if SL
	SerializableObjectLayerServiceClientBase<ISerializableObjectLayerServiceClientAsync>
#else
	SerializableObjectLayerServiceClientBase<ISerializableObjectLayerService>
#endif
	{
		public SerializableObjectLayerServiceClient(string confName) : base(confName) { }
		public SerializableObjectLayerServiceClient(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
	}
	public class SerializableObjectLayerServiceClientBase<IContractType> : ClientBase<IContractType>, ISerializableObjectLayer, ISerializableObjectLayerEx
#if SL
	where IContractType : class, ISerializableObjectLayerServiceClientAsync
#else
	where IContractType : class, ISerializableObjectLayerService
#endif
	{
		public SerializableObjectLayerServiceClientBase(string confName) : base(confName) { }
		public SerializableObjectLayerServiceClientBase(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
		public event ClientChannelCreatedHandler ClientChannelCreated;
		public static event ClientChannelCreatedHandler GlobalObjectClientChannelCreated;
		protected virtual void OnClientChannelCreated(object channel) {
			if(ClientChannelCreated != null) {
				ClientChannelCreated(this, new ClientChannelCreatedEventArgs(channel));
			}
			if(GlobalObjectClientChannelCreated != null) {
				GlobalObjectClientChannelCreated(this, new ClientChannelCreatedEventArgs(channel));
			}
		}
#if SL
		protected override IContractType CreateChannel() {
			object channel = new SerializableObjectLayerServiceClientChannel(this);
			OnClientChannelCreated(channel);
			return (IContractType)channel;
		}
		protected new IContractType Channel {
			get {
				return (IContractType)base.Channel;
			}
		}
		public bool CanLoadCollectionObjects {
			get {
				return OperationResult.ExecuteClient<bool>(() => {
					IAsyncResult res = Channel.BeginGetCanLoadCollectionObjects(null, null);
					return Channel.EndGetCanLoadCollectionObjects(res);
				}).HandleError();
			}
		}
		public CommitObjectStubsResult[] CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return OperationResult.ExecuteClient<CommitObjectStubsResult[]>(() => {
				IAsyncResult res = Channel.BeginCommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption, null, null);
				return Channel.EndCommitObjects(res);
			}).HandleError();
		}
		public void CreateObjectType(string assemblyName, string typeName) {
			OperationResult.ExecuteClient<object>(() => {
				IAsyncResult res = Channel.BeginCreateObjectType(assemblyName, typeName, null, null);
				return Channel.EndCreateObjectType(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection[]>>(() => {
				IAsyncResult res = Channel.BeginGetObjectsByKey(dictionary, queries, null, null);
				return Channel.EndGetObjectsByKey(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(() => {
				IAsyncResult res = Channel.BeginLoadCollectionObjects(dictionary, refPropertyName, ownerObject, null, null);
				return Channel.EndLoadCollectionObjects(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection[]>>(() => {
				IAsyncResult res = Channel.BeginLoadObjects(dictionary, queries, null, null);
				return Channel.EndLoadObjects(res);
			}).HandleError();
		}
		public PurgeResult Purge() {
			return OperationResult.ExecuteClient<PurgeResult>(() => {
				IAsyncResult res = Channel.BeginPurge(null, null);
				return Channel.EndPurge(res);
			}).HandleError();
		}
		public object[][] SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return OperationResult.ExecuteClient<object[][]>(() => {
				IAsyncResult res = Channel.BeginSelectData(dictionary, query, properties, groupProperties, groupCriteria, null, null);
				return Channel.EndSelectData(res);
			}).HandleError();
		}
		public ISerializableObjectLayer ObjectLayer {
			get { return this; }
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete() {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(() => {
				IAsyncResult res = Channel.BeginGetParentObjectsToDelete(null, null);
				return Channel.EndGetParentObjectsToDelete(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave() {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(() => {
				IAsyncResult res = Channel.BeginGetParentObjectsToSave(null, null);
				return Channel.EndGetParentObjectsToSave(res);
			}).HandleError();
		}
		public string[] GetParentTouchedClassInfos() {
			return OperationResult.ExecuteClient<string[]>(() => {
				IAsyncResult res = Channel.BeginGetParentTouchedClassInfos(null, null);
				return Channel.EndGetParentTouchedClassInfos(res);
			}).HandleError();
		}
		public bool IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return OperationResult.ExecuteClient<bool>(() => {
				IAsyncResult res = Channel.BeginIsParentObjectToDelete(dictionary, theObject, null, null);
				return Channel.EndIsParentObjectToDelete(res);
			}).HandleError();
		}
		public bool IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return OperationResult.ExecuteClient<bool>(() => {
				IAsyncResult res = Channel.BeginIsParentObjectToSave(dictionary, theObject, null, null);
				return Channel.EndIsParentObjectToSave(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<object[]>>(() => {
				IAsyncResult res = Channel.BeginLoadDelayedProperty(dictionary, objects, property, null, null);
				return Channel.EndLoadDelayedProperty(res);
			}).HandleError();
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return OperationResult.ExecuteClient<SerializableObjectLayerResult<object[]>>(() => {
				IAsyncResult res = Channel.BeginLoadDelayedProperties(dictionary, theObject, props, null, null);
				return Channel.EndLoadDelayedProperties(res);
			}).HandleError();
		}
		class SerializableObjectLayerServiceClientChannel : SerializableObjectLayerServiceClientChannelBase<IContractType> {
			public SerializableObjectLayerServiceClientChannel(ClientBase<IContractType> client)
				: base(client) {
			}
		}
		protected class SerializableObjectLayerServiceClientChannelBase<IAsyncContractType> : ChannelBase<IAsyncContractType>, ISerializableObjectLayerServiceClientAsync
			where IAsyncContractType : class, ISerializableObjectLayerServiceClientAsync
		{
			public SerializableObjectLayerServiceClientChannelBase(ClientBase<IAsyncContractType> client)
				: base(client) {
			}
			public IAsyncResult BeginGetCanLoadCollectionObjects(AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetCanLoadCollectionObjects", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginCommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption, AsyncCallback callback, object asyncState) {
				return BeginInvoke("CommitObjects", new object[] { dictionary, objectsForDelete, objectsForSave, lockingOption }, callback, asyncState);
			}
			public IAsyncResult BeginCreateObjectType(string assemblyName, string typeName, AsyncCallback callback, object asyncState) {
				return BeginInvoke("CreateObjectType", new object[] { assemblyName, typeName }, callback, asyncState);
			}
			public IAsyncResult BeginGetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries, AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetObjectsByKey", new object[] { dictionary, queries }, callback, asyncState);
			}
			public IAsyncResult BeginLoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject, AsyncCallback callback, object asyncState) {
				return BeginInvoke("LoadCollectionObjects", new object[] { dictionary, refPropertyName, ownerObject }, callback, asyncState);
			}
			public IAsyncResult BeginLoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries, AsyncCallback callback, object asyncState) {
				return BeginInvoke("LoadObjects", new object[] { dictionary, queries }, callback, asyncState);
			}
			public IAsyncResult BeginPurge(AsyncCallback callback, object asyncState) {
				return BeginInvoke("Purge", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginSelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, AsyncCallback callback, object asyncState) {
				return BeginInvoke("SelectData", new object[] { dictionary, query, properties, groupProperties, groupCriteria }, callback, asyncState);
			}
			public IAsyncResult BeginGetParentObjectsToDelete(AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetParentObjectsToDelete", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginGetParentObjectsToSave(AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetParentObjectsToSave", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginGetParentTouchedClassInfos(AsyncCallback callback, object asyncState) {
				return BeginInvoke("GetParentTouchedClassInfos", emptyArray, callback, asyncState);
			}
			public IAsyncResult BeginIsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject, AsyncCallback callback, object asyncState) {
				return BeginInvoke("IsParentObjectToDelete", new object[] { dictionary, theObject }, callback, asyncState);
			}
			public IAsyncResult BeginIsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject, AsyncCallback callback, object asyncState) {
				return BeginInvoke("IsParentObjectToSave", new object[] { dictionary, theObject }, callback, asyncState);
			}
			public IAsyncResult BeginLoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props, AsyncCallback callback, object asyncState) {
				return BeginInvoke("LoadDelayedProperties", new object[] { dictionary, theObject, props }, callback, asyncState);
			}
			public IAsyncResult BeginLoadDelayedProperty(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property, AsyncCallback callback, object asyncState) {
				return BeginInvoke("LoadDelayedProperty", new object[] { dictionary, objects, property }, callback, asyncState);
			}
			static object[] emptyArray = new object[0];
			public OperationResult<bool> EndGetCanLoadCollectionObjects(IAsyncResult result) {
				return (OperationResult<bool>)EndInvoke("GetCanLoadCollectionObjects", emptyArray, result);
			}
			public OperationResult<CommitObjectStubsResult[]> EndCommitObjects(IAsyncResult result) {
				return (OperationResult<CommitObjectStubsResult[]>)EndInvoke("CommitObjects", emptyArray, result);
			}
			public OperationResult<object> EndCreateObjectType(IAsyncResult result) {
				return (OperationResult<object>)EndInvoke("CreateObjectType", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> EndGetObjectsByKey(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>>)EndInvoke("GetObjectsByKey", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndLoadCollectionObjects(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>>)EndInvoke("LoadCollectionObjects", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>> EndLoadObjects(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<XPObjectStubCollection[]>>)EndInvoke("LoadObjects", emptyArray, result);
			}
			public OperationResult<PurgeResult> EndPurge(IAsyncResult result) {
				return (OperationResult<PurgeResult>)EndInvoke("Purge", emptyArray, result);
			}
			public OperationResult<object[][]> EndSelectData(IAsyncResult result) {
				return (OperationResult<object[][]>)EndInvoke("SelectData", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndGetParentObjectsToDelete(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>>)EndInvoke("GetParentObjectsToDelete", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>> EndGetParentObjectsToSave(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<XPObjectStubCollection>>)EndInvoke("GetParentObjectsToSave", emptyArray, result);
			}
			public OperationResult<string[]> EndGetParentTouchedClassInfos(IAsyncResult result) {
				return (OperationResult<string[]>)EndInvoke("GetParentTouchedClassInfos", emptyArray, result);
			}
			public OperationResult<bool> EndIsParentObjectToDelete(IAsyncResult result) {
				return (OperationResult<bool>)EndInvoke("IsParentObjectToDelete", emptyArray, result);
			}
			public OperationResult<bool> EndIsParentObjectToSave(IAsyncResult result) {
				return (OperationResult<bool>)EndInvoke("IsParentObjectToSave", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<object[]>> EndLoadDelayedProperties(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<object[]>>)EndInvoke("LoadDelayedProperties", emptyArray, result);
			}
			public OperationResult<SerializableObjectLayerResult<object[]>> EndLoadDelayedProperty(IAsyncResult result) {
				return (OperationResult<SerializableObjectLayerResult<object[]>>)EndInvoke("LoadDelayedProperty", emptyArray, result);
			}
		}
#else
		IContractType channel;
		protected new IContractType Channel {
			get {
				IContractType currentChannel = channel;
				if(currentChannel == null) {
					currentChannel = CreateChannel();
					channel = currentChannel;
					OnClientChannelCreated(currentChannel);
				}
				return currentChannel;
			}
		}
		public bool CanLoadCollectionObjects {
			get { return ExecuteClient<bool>(delegate() { return Channel.GetCanLoadCollectionObjects(); }).HandleError(); }
		}
		public CommitObjectStubsResult[] CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return ExecuteClient<CommitObjectStubsResult[]>(delegate() { return Channel.CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption); }).HandleError();
		}
		public void CreateObjectType(string assemblyName, string typeName) {
			ExecuteClient<object>(delegate() { return Channel.CreateObjectType(assemblyName, typeName); }).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() { return Channel.GetObjectsByKey(dictionary, queries); }).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return Channel.LoadCollectionObjects(dictionary, refPropertyName, ownerObject); }).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection[]>>(delegate() { return Channel.LoadObjects(dictionary, queries); }).HandleError();
		}
		public PurgeResult Purge() {
			return ExecuteClient<PurgeResult>(delegate() { return Channel.Purge(); }).HandleError();
		}
		public object[][] SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return ExecuteClient<object[][]>(delegate() { return Channel.SelectData(dictionary, query, properties, groupProperties, groupCriteria); }).HandleError();
		}
		public ISerializableObjectLayer ObjectLayer {
			get { return this; }
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete() {
			return ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return Channel.GetParentObjectsToDelete(); }).HandleError();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave() {
			return ExecuteClient<SerializableObjectLayerResult<XPObjectStubCollection>>(delegate() { return Channel.GetParentObjectsToSave(); }).HandleError();
		}
		public string[] GetParentTouchedClassInfos() {
			return ExecuteClient<string[]>(delegate() { return Channel.GetParentTouchedClassInfos(); }).HandleError();
		}
		public bool IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return ExecuteClient<bool>(delegate() { return Channel.IsParentObjectToDelete(dictionary, theObject); }).HandleError();
		}
		public bool IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return ExecuteClient<bool>(delegate() { return Channel.IsParentObjectToSave(dictionary, theObject); }).HandleError();
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return ExecuteClient<SerializableObjectLayerResult<object[]>>(delegate() { return Channel.LoadDelayedProperty(dictionary, objects, property); }).HandleError();
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return ExecuteClient<SerializableObjectLayerResult<object[]>>(delegate() { return Channel.LoadDelayedProperties(dictionary, theObject, props); }).HandleError();
		}
		public OperationResult<R> ExecuteClient<R>(OperationResultChannelPredicate<R> predicate) {
			return OperationResult.ExecuteClient<R, IContractType>(predicate, ref channel);
		}
#endif
	}
	#endregion
#if !SL
	public class SerializableObjectLayerMarshalByRefObject : MarshalByRefObject, ISerializableObjectLayer, ISerializableObjectLayerEx {
		public static ISerializableObjectLayer SerializableObjectLayer;
		public bool CanLoadCollectionObjects {
			get { return SerializableObjectLayer.CanLoadCollectionObjects; }
		}
		public CommitObjectStubsResult[] CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return SerializableObjectLayer.CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public void CreateObjectType(string assemblyName, string typeName) {
			SerializableObjectLayer.CreateObjectType(assemblyName, typeName);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return SerializableObjectLayer.GetObjectsByKey(dictionary, queries);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return SerializableObjectLayer.LoadCollectionObjects(dictionary, refPropertyName, ownerObject);
		}
		public SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return SerializableObjectLayer.LoadObjects(dictionary, queries);
		}
		public PurgeResult Purge() {
			return SerializableObjectLayer.Purge();
		}
		public object[][] SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return SerializableObjectLayer.SelectData(dictionary, query, properties, groupProperties, groupCriteria);
		}
		public ISerializableObjectLayer ObjectLayer {
			get { return this; }
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete() {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).GetParentObjectsToDelete();
		}
		public SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave() {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).GetParentObjectsToSave();
		}
		public string[] GetParentTouchedClassInfos() {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).GetParentTouchedClassInfos();
		}
		public bool IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).IsParentObjectToDelete(dictionary, theObject);
		}
		public bool IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).IsParentObjectToSave(dictionary, theObject);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).LoadDelayedProperties(dictionary, objects, property);
		}
		public SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return ((ISerializableObjectLayerEx)SerializableObjectLayer).LoadDelayedProperties(dictionary, theObject, props);
		}
	}
#endif
	public abstract class SerializableObjectLayerProxyBase : ISerializableObjectLayer, ISerializableObjectLayerProvider, ISerializableObjectLayerEx, ICommandChannel {
		protected abstract SerializableObjectLayer GetObjectLayer();
#if !SL
	[DevExpressXpoLocalizedDescription("SerializableObjectLayerProxyBaseCanLoadCollectionObjects")]
#endif
		public virtual bool CanLoadCollectionObjects {
			get { return GetObjectLayer().CanLoadCollectionObjects; }
		}
		public virtual CommitObjectStubsResult[] CommitObjects(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			return GetObjectLayer().CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption);
		}
		public virtual void CreateObjectType(string assemblyName, string typeName) {
			GetObjectLayer().CreateObjectType(assemblyName, typeName);
		}
		public virtual SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return GetObjectLayer().GetObjectsByKey(dictionary, queries);
		}
		public virtual SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return GetObjectLayer().LoadCollectionObjects(dictionary, refPropertyName, ownerObject);
		}
		public virtual SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return GetObjectLayer().LoadObjects(dictionary, queries);
		}
		public virtual PurgeResult Purge() {
			return GetObjectLayer().Purge();
		}
		public virtual object[][] SelectData(XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return GetObjectLayer().SelectData(dictionary, query, properties, groupProperties, groupCriteria);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SerializableObjectLayerProxyBaseObjectLayer")]
#endif
		public virtual ISerializableObjectLayer ObjectLayer {
			get { return GetObjectLayer().ObjectLayer; }
		}
		public virtual SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToDelete() {
			return GetObjectLayer().GetParentObjectsToDelete();
		}
		public virtual SerializableObjectLayerResult<XPObjectStubCollection> GetParentObjectsToSave() {
			return GetObjectLayer().GetParentObjectsToSave();
		}
		public virtual string[] GetParentTouchedClassInfos() {
			return GetObjectLayer().GetParentTouchedClassInfos();
		}
		public virtual bool IsParentObjectToDelete(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetObjectLayer().IsParentObjectToDelete(dictionary, theObject);
		}
		public virtual bool IsParentObjectToSave(XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetObjectLayer().IsParentObjectToSave(dictionary, theObject);
		}
		public virtual SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return GetObjectLayer().LoadDelayedProperties(dictionary, objects, property);
		}
		public virtual SerializableObjectLayerResult<object[]> LoadDelayedProperties(XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return GetObjectLayer().LoadDelayedProperties(dictionary, theObject, props);
		}
		public virtual object Do(string command, object args) {
			return ((ICommandChannel)GetObjectLayer()).Do(command, args);
		}
	}
}
#endif
