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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public abstract class DataServiceOperationEventArgs : EventArgs, IDisposable {
		protected void OnCompleted() {
			if(Completed != null) {
				Completed(this, EventArgs.Empty);
			}
		}
		public virtual void Dispose() {
			Completed = null;
		}
		public event EventHandler<EventArgs> Completed;
		public IObjectSpace ObjectSpace { get; protected set; }
	}
	public interface IApplicationDataService {
		event EventHandler<DataServiceOperationEventArgs> Committing;
	}
	internal class UnitOfWorkDataServiceOperationEventArgs : DataServiceOperationEventArgs {
		private UnitOfWork uow;
		private EventHandler raiseStartedHandler;
		void uow_BeforeCommitTransaction(object sender, SessionManipulationEventArgs e) {
			if(raiseStartedHandler != null) {
				raiseStartedHandler(this, EventArgs.Empty);
			}
		}
		void uow_AfterCommitTransaction(object sender, SessionManipulationEventArgs e) {
			OnCompleted();
		}
		public UnitOfWorkDataServiceOperationEventArgs(IObjectSpace objectSpace, UnitOfWork uow, EventHandler raiseStartedHandler) {
			this.raiseStartedHandler = raiseStartedHandler;
			this.uow = uow;
			if(uow != null) {
				uow.BeforeCommitTransaction += new SessionManipulationEventHandler(uow_BeforeCommitTransaction);
				uow.AfterCommitTransaction += new SessionManipulationEventHandler(uow_AfterCommitTransaction);
			}
			this.ObjectSpace = objectSpace;
		}
		public override void Dispose() {
			raiseStartedHandler = null;
			if(uow != null) {
				uow.BeforeCommitTransaction -= new SessionManipulationEventHandler(uow_BeforeCommitTransaction);
				uow.AfterCommitTransaction -= new SessionManipulationEventHandler(uow_AfterCommitTransaction);
				uow = null;
			}
			ObjectSpace = null;
		}
	}
	internal class ObjectSpaceWrapper : XPObjectSpace {
		public ObjectSpaceWrapper(DevExpress.ExpressApp.DC.ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, CreateUnitOfWorkHandler createUnitOfWorkDelegate)
			: base(typesInfo, xpoTypeInfoSource, createUnitOfWorkDelegate) {
		}
		public override void Dispose() {
			SetSession(null);
			base.Dispose();
		}
	}
	public abstract class SecuredSerializableObjectLayerBase : ISecuredSerializableObjectLayer, IApplicationDataService {
		protected abstract SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo, out UnitOfWork session);
		protected abstract void FinalizeSession(IClientInfo clientInfo); 
		protected object[][] SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return GetSerializableObjectLayer(clientInfo).SelectData(dictionary, query, properties, groupProperties, groupCriteria);
		}
		protected SerializableObjectLayerResult<XPObjectStubCollection[]> LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return GetSerializableObjectLayer(clientInfo).LoadObjects(dictionary, queries);
		}
		protected SerializableObjectLayerResult<XPObjectStubCollection[]> GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return GetSerializableObjectLayer(clientInfo).GetObjectsByKey(dictionary, queries);
		}
		protected SerializableObjectLayerResult<XPObjectStubCollection> LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return GetSerializableObjectLayer(clientInfo).LoadCollectionObjects(dictionary, refPropertyName, ownerObject);
		}
		protected SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return GetSerializableObjectLayer(clientInfo).LoadDelayedProperties(dictionary, theObject, props);
		}
		protected SerializableObjectLayerResult<object[]> LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return GetSerializableObjectLayer(clientInfo).LoadDelayedProperties(dictionary, objects, property);
		}
		protected SerializableObjectLayer GetSerializableObjectLayer(IClientInfo clientInfo) {
			UnitOfWork session;
			return GetSerializableObjectLayer(clientInfo, out session);
		}
		#region ISecuredSerializableObjectLayer2
		SerializableObjectLayerResult<XPObjectStubCollection[]> ISecuredSerializableObjectLayer.LoadObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery[] queries) {
			return LoadObjects(clientInfo, dictionary, queries);
		}
		CommitObjectStubsResult[] ISecuredSerializableObjectLayer.CommitObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption) {
			UnitOfWork session;
			SerializableObjectLayer ol = GetSerializableObjectLayer(clientInfo, out session);
			CommitObjectStubsResult[] result;
			if(Committing != null) {
				if(XafTypesInfo.Instance == null) {
					throw new InvalidOperationException("XafTypesInfo.Instance is null.");
				}
				XpoTypeInfoSource xpoTypeInfoSource = (XpoTypeInfoSource)((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(XpoTypeInfoSource));
				if(xpoTypeInfoSource == null) {
					throw new InvalidOperationException("XafTypesInfo.PersistentEntityStore is null.");
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
					catch {
						FinalizeSession(clientInfo);
						throw;
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
			return result;
		}
		private static CommitObjectStubsResult[] InternalCommitChanges(XPDictionaryStub dictionary, XPObjectStubCollection objectsForDelete, XPObjectStubCollection objectsForSave, LockingOption lockingOption, UnitOfWork session, SerializableObjectLayer ol) {
			CommitObjectStubsResult[] result;
			result = ol.CommitObjects(dictionary, objectsForDelete, objectsForSave, lockingOption);
			session.CommitChanges();
			return result;
		}
		SerializableObjectLayerResult<XPObjectStubCollection[]> ISecuredSerializableObjectLayer.GetObjectsByKey(IClientInfo clientInfo, XPDictionaryStub dictionary, GetObjectStubsByKeyQuery[] queries) {
			return GetObjectsByKey(clientInfo, dictionary, queries);
		}
		object[][] ISecuredSerializableObjectLayer.SelectData(IClientInfo clientInfo, XPDictionaryStub dictionary, ObjectStubsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return SelectData(clientInfo, dictionary, query, properties, groupProperties, groupCriteria);
		}
		bool ISecuredSerializableObjectLayer.CanLoadCollectionObjects(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).CanLoadCollectionObjects;
		}
		SerializableObjectLayerResult<XPObjectStubCollection> ISecuredSerializableObjectLayer.LoadCollectionObjects(IClientInfo clientInfo, XPDictionaryStub dictionary, string refPropertyName, XPObjectStub ownerObject) {
			return LoadCollectionObjects(clientInfo, dictionary, refPropertyName, ownerObject);
		}
		PurgeResult ISecuredSerializableObjectLayer.Purge(IClientInfo clientInfo) {
			throw new InvalidOperationException();
		}
		SerializableObjectLayerResult<object[]> ISecuredSerializableObjectLayer.LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject, string[] props) {
			return LoadDelayedProperties(clientInfo, dictionary, theObject, props);
		}
		SerializableObjectLayerResult<object[]> ISecuredSerializableObjectLayer.LoadDelayedProperties(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStubCollection objects, string property) {
			return LoadDelayedProperties(clientInfo, dictionary, objects, property);
		}
		bool ISecuredSerializableObjectLayer.IsParentObjectToSave(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetSerializableObjectLayer(clientInfo).IsParentObjectToSave(dictionary, theObject);
		}
		bool ISecuredSerializableObjectLayer.IsParentObjectToDelete(IClientInfo clientInfo, XPDictionaryStub dictionary, XPObjectStub theObject) {
			return GetSerializableObjectLayer(clientInfo).IsParentObjectToDelete(dictionary, theObject);
		}
		SerializableObjectLayerResult<XPObjectStubCollection> ISecuredSerializableObjectLayer.GetParentObjectsToSave(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).GetParentObjectsToSave();
		}
		SerializableObjectLayerResult<XPObjectStubCollection> ISecuredSerializableObjectLayer.GetParentObjectsToDelete(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).GetParentObjectsToDelete();
		}
		string[] ISecuredSerializableObjectLayer.GetParentTouchedClassInfos(IClientInfo clientInfo) {
			return GetSerializableObjectLayer(clientInfo).GetParentTouchedClassInfos();
		}
		void ISecuredSerializableObjectLayer.CreateObjectType(IClientInfo clientInfo, string assemblyName, string typeName) {
			GetSerializableObjectLayer(clientInfo).CreateObjectType(assemblyName, typeName);
		}
		object ISecuredSerializableObjectLayer.Do(IClientInfo clientInfo, string command, object args) {
			return ((ICommandChannel)GetSerializableObjectLayer(clientInfo)).Do(command, args);
		}
		void ISecuredSerializableObjectLayer.FinalizeSession(IClientInfo clientInfo) {
			FinalizeSession(clientInfo);
		}
		#endregion
		public event EventHandler<DataServiceOperationEventArgs> Committing;
	}
}
