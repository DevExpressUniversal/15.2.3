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
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.DurableInstancing;
using System.Runtime.Serialization;
using System.ServiceModel.Activities;
using System.Xml;
using DevExpress.Workflow.Utils;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Xml.Linq;
using System.Timers;
using System.Activities;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Workflow.Store {
	internal class Owner {
		private Dictionary<XName, InstanceValue> ownerMetadata = new Dictionary<XName, InstanceValue>();
		public Owner(XName name, Guid id) {
			if(id == null || id == Guid.Empty) {
				throw new ArgumentException("Owner 'id' is null or empty");
			}
			OwnerId = id;
			Name = name;
		}
		public XName Name { get; private set; }
		public Guid OwnerId { get; private set; }
		public IDictionary<XName, InstanceValue> OwnerMetadata { get { return ownerMetadata; } }
		public void FillMetadata(IDictionary<XName, InstanceValue> sourceMetadata) {
			ownerMetadata.Clear();
			foreach(XName name in sourceMetadata.Keys) {
				ownerMetadata[name] = sourceMetadata[name];
			}
		}
		public string OwnerIdString {
			get {
				if(OwnerMetadata.ContainsKey(WorkflowNamespace.WorkflowHostType)) {
					return (OwnerMetadata[WorkflowNamespace.WorkflowHostType].Value as XName).LocalName;
				}
				return string.Empty;
			}
		}
		public Dictionary<XName, InstanceValue> CreateInstanceMetadata() {
			Dictionary<XName, InstanceValue> instanceMetadata = new Dictionary<XName, InstanceValue>();
			if(OwnerMetadata.ContainsKey(WorkflowNamespace.WorkflowHostType)) {
				instanceMetadata.Add(WorkflowNamespace.WorkflowHostType, OwnerMetadata[WorkflowNamespace.WorkflowHostType]);
			}
			return instanceMetadata;
		}
	}
	public class WorkflowInstanceStore : InstanceStore {
		private const string CanNotLoadUninitializedInstance = "An uninitialized instance is passed while it is not allowed: '{0}'. For more details, see the AcceptUninitializedInstance property of the System.Activities.DurableInstancing.LoadWorkflowCommand class.";
		private static readonly object LockObj = new object();
		private Type workflowInstanceKeyType;
		private Type workflowInstanceType;
		private IObjectSpaceProvider objectSpaceProvider;
		private InstanceCompletionAction instanceCompletionAction = InstanceCompletionAction.DeleteAll; 
		private TimeSpan runnableInstancesDetectionPeriod = new TimeSpan(0, 0, 30); 
		private Timer timer;
		private Dictionary<Guid, Owner> owners = new Dictionary<Guid, Owner>();
		private List<Guid> runningInstances = new List<Guid>();
		private NetDataContractSerializer CreateNetDataContractSerializer() {
			NetDataContractSerializer s = new NetDataContractSerializer();
			return s;
		}
		private string SerializeInstanceValues(IDictionary<System.Xml.Linq.XName, InstanceValue> instanceValues) {
			XmlDocument document = new XmlDocument();
			document.LoadXml("<InstanceValues/>");
			foreach(KeyValuePair<System.Xml.Linq.XName, InstanceValue> value in instanceValues) {
				try {
					XmlElement instanceValue = document.CreateElement("InstanceValue");
					XmlElement key = SerializeValue("key", value.Key, document);
					instanceValue.AppendChild(key);
					XmlElement newValue = SerializeValue("value", value.Value.Value, document);
					instanceValue.AppendChild(newValue);
					document.DocumentElement.AppendChild(instanceValue);
				}
				catch(Exception) { }
			}
			return document.InnerXml;
		}
		private XmlElement SerializeValue(string elementName, object o, XmlDocument doc) {
			NetDataContractSerializer serializer = CreateNetDataContractSerializer();
			XmlElement element = doc.CreateElement(elementName);
			using(MemoryStream stream = new MemoryStream()) {
				serializer.Serialize(stream, o);
				stream.Position = 0;
				StreamReader reader = new StreamReader(stream);
				element.InnerXml = reader.ReadToEnd();
			}
			return element;
		}
		private IDictionary<System.Xml.Linq.XName, InstanceValue> DeserializeValues(string serializedValues) {
			IDictionary<System.Xml.Linq.XName, InstanceValue> data = new Dictionary<System.Xml.Linq.XName, InstanceValue>();
			if(!string.IsNullOrEmpty(serializedValues)) {
				NetDataContractSerializer serializer = CreateNetDataContractSerializer();
				XmlDocument document = new XmlDocument();
				document.Load(new StringReader(serializedValues));
				XmlNodeList values = document.GetElementsByTagName("InstanceValue");
				foreach(XmlElement valuePair in values) {
					XmlElement keyElement = (XmlElement)valuePair.SelectSingleNode("descendant::key");
					System.Xml.Linq.XName keyValue = (System.Xml.Linq.XName)DeserializeValue(serializer, keyElement);
					XmlElement valueElement = (XmlElement)valuePair.SelectSingleNode("descendant::value");
					object value = DeserializeValue(serializer, valueElement);
					InstanceValue instanceValue = new InstanceValue(value);
					data.Add(keyValue, instanceValue);
				}
			}
			return data;
		}
		private object DeserializeValue(NetDataContractSerializer serializer, XmlElement element) {
			MemoryStream stream = new MemoryStream();
			XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);
			element.WriteContentTo(writer);
			writer.Flush();
			stream.Position = 0;
			return serializer.Deserialize(stream);
		}
		private IInstanceKey FindWorkflowInstanceKey(IObjectSpace objectSpace, Guid keyID) {
			return (IInstanceKey)objectSpace.FindObject(workflowInstanceKeyType, new BinaryOperator("KeyId", keyID));
		}
		private IList<IInstanceKey> FindWorkflowInstanceKeys(IObjectSpace objectSpace, Guid instanceId) {
			List<IInstanceKey> result = new List<IInstanceKey>();
			foreach(object obj in objectSpace.GetObjects(workflowInstanceKeyType, new BinaryOperator("InstanceId", instanceId))) {
				result.Add((IInstanceKey)obj);
			}
			return result;
		}
		private IInstanceKey CreateWorkflowInstanceKey(IObjectSpace objectSpace) {
			return (IInstanceKey)objectSpace.CreateObject(workflowInstanceKeyType);
		}
		private void FreeInstanceKey(IObjectSpace objectSpace, Guid keyId) {
			object instanceKey = FindWorkflowInstanceKey(objectSpace, keyId);
			if(instanceKey != null) {
				objectSpace.Delete(instanceKey);
			}
		}
		private IWorkflowInstance FindWorkflowInstance(IObjectSpace objectSpace, Guid instanceID) {
			return (IWorkflowInstance)objectSpace.FindObject(workflowInstanceType, new BinaryOperator("InstanceId", instanceID));
		}
		private IWorkflowInstance FindRunnableInstance(IObjectSpace objectSpace, Guid ownerId) {
			Owner owner = owners[ownerId];
			string ownerString = owner.OwnerIdString;
			GroupOperator criteria = new GroupOperator(new BinaryOperator("Owner", ownerString),
				new BinaryOperator("Status", ActivityInstanceState.Executing),
				new GroupOperator(GroupOperatorType.Or,
					new BinaryOperator("ExpirationDateTime", DateTime.Now.ToUniversalTime(), BinaryOperatorType.LessOrEqual),
					new NullOperator("ExpirationDateTime")));
			if(runningInstances != null && runningInstances.Count > 0) {
				criteria.Operands.Add(new NotOperator(new InOperator("InstanceId", runningInstances)));
			}
			return (IWorkflowInstance)objectSpace.FindObject(workflowInstanceType, criteria);
		}
		private IWorkflowInstance CreateWorkflowInstance(IObjectSpace objectSpace) {
			return (IWorkflowInstance)objectSpace.CreateObject(workflowInstanceType);
		}
		private void timer_Elapsed(object sender, ElapsedEventArgs e) {
			RaiseInstancePersistenceEvents();
		}
		private void RaiseInstancePersistenceEvents() {
			timer.Enabled = false;
			foreach(InstanceOwner owner in GetInstanceOwners()) {
				foreach(InstancePersistenceEvent persistenceEvent in this.GetEvents(owner)) {
					if(persistenceEvent.Equals(HasRunnableWorkflowEvent.Value)) {
						bool hasRunnableEvents = false;
						using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
							hasRunnableEvents = FindRunnableInstance(objectSpace, owner.InstanceOwnerId) != null;
						}
						if(hasRunnableEvents) {
							this.SignalEvent(persistenceEvent, owner);
						}
						else {
							this.ResetEvent(persistenceEvent, owner);
							timer.Enabled = true;
						}
					}
				}
			}
		}
		private bool CheckIsSuspendWithNoException(IDictionary<XName, InstanceValue> instanceMetadataChanges) {
			XName suspendExceptionName = XNamespace.Get("urn:schemas-microsoft-com:System.ServiceModel.Activities/4.0/properties").GetName("SuspendException");
			if(instanceMetadataChanges != null && instanceMetadataChanges.ContainsKey(suspendExceptionName)) {
				return !(instanceMetadataChanges[suspendExceptionName].Value != null && typeof(System.Exception).IsAssignableFrom(instanceMetadataChanges[suspendExceptionName].Value.GetType()));
			}
			return true;
		}
		private ActivityInstanceState GetWorkflowStatus(IDictionary<XName, InstanceValue> data) {
			if(data.ContainsKey(WorkflowNamespace.Status)) {
				try {
					return (ActivityInstanceState)Enum.Parse(typeof(ActivityInstanceState), data[WorkflowNamespace.Status].Value.ToString());
				}
				catch(Exception) {
					return ActivityInstanceState.Closed;
				}
			}
			return ActivityInstanceState.Closed;
		}
		private void OnInstanceComplete(Guid id, ActivityInstanceState status, IDictionary<XName, InstanceValue> data) {
			if(InstanceComplete != null) {
				InstanceComplete(this, new InstanceEventArgs(id, status, data));
			}
		}
		protected override bool TryCommand(InstancePersistenceContext context, InstancePersistenceCommand command, TimeSpan timeout) {
			return EndTryCommand(BeginTryCommand(context, command, timeout, null, null));
		}
		protected override IAsyncResult BeginTryCommand(InstancePersistenceContext context, InstancePersistenceCommand command, TimeSpan timeout, AsyncCallback callback, object state) {
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(">WorkflowInstanceStore.BeginTryCommand");
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("command.GetType()", command.GetType());
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("context.InstanceView", context.InstanceView);
			if(context.InstanceView != null) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("context.InstanceView.InstanceId", context.InstanceView.InstanceId);
			}
			CreateWorkflowOwnerCommand createOwner = command as CreateWorkflowOwnerCommand;
			if(createOwner != null) {
				ProcessCreateWorkflowOwnerCommand(context, createOwner);
			}
			DeleteWorkflowOwnerCommand deleteOwner = command as DeleteWorkflowOwnerCommand;
			if(deleteOwner != null) {
				ProcessDeleteWorkflowOwnerCommand(context, deleteOwner);
			}
			LoadWorkflowCommand loadWorkflow = command as LoadWorkflowCommand;
			if(loadWorkflow != null) {
				ProcessLoadWorkflowCommand(context, loadWorkflow);
			}
			SaveWorkflowCommand saveCommand = command as SaveWorkflowCommand;
			if(saveCommand != null) {
				ProcessSaveWorkflowCommand(context, saveCommand);
			}
			TryLoadRunnableWorkflowCommand tryLoadRunnable = command as TryLoadRunnableWorkflowCommand;
			if(tryLoadRunnable != null) {
				ProcessTryLoadRunnableWorkflowCommand(context, tryLoadRunnable);
			}
			LoadWorkflowByInstanceKeyCommand loadByInstance = command as LoadWorkflowByInstanceKeyCommand;
			if(loadByInstance != null) {
				ProcessLoadWorkflowByInstanceCommand(context, loadByInstance);
			}
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("<WorkflowInstanceStore.BeginTryCommand");
			return new WorkflowStoreCompletedAsyncResult(callback, state, command);
		}
		private void ProcessCreateWorkflowOwnerCommand(InstancePersistenceContext context, CreateWorkflowOwnerCommand createWorkflowOwnerCommand) {
			Owner owner = new Owner(createWorkflowOwnerCommand.Name, Guid.NewGuid());
			owner.FillMetadata(createWorkflowOwnerCommand.InstanceOwnerMetadata);
			owners.Add(owner.OwnerId, owner);
			context.BindInstanceOwner(owner.OwnerId, owner.OwnerId);
			context.BindEvent(HasRunnableWorkflowEvent.Value);
			if(!timer.Enabled) timer.Enabled = true;
		}
		private void ProcessDeleteWorkflowOwnerCommand(InstancePersistenceContext context, DeleteWorkflowOwnerCommand deleteWorkflowOwnerCommand) {
			Guid ownerId = context.InstanceView.InstanceOwner.InstanceOwnerId;
			owners.Remove(ownerId);
			context.InstanceHandle.Free();
		}
		private void ProcessLoadWorkflowCommand(InstancePersistenceContext context, LoadWorkflowCommand loadWorkflowCommand) {
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(">WorkflowInstanceStore.ProcessLoadWorkflowCommand");
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("AcceptUninitializedInstance", loadWorkflowCommand.AcceptUninitializedInstance);
			lock(LockObj) {
				IDictionary<System.Xml.Linq.XName, InstanceValue> data = null;
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
					IWorkflowInstance workflowInstance = FindWorkflowInstance(objectSpace, context.InstanceView.InstanceId);
					DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("workflowInstance", workflowInstance);
					InstanceState instanceState = InstanceState.Uninitialized;
					IDictionary<XName, InstanceValue> instanceMetadata = null;
					if(workflowInstance != null) {
						data = DeserializeValues(workflowInstance.Content);
						if(!string.IsNullOrEmpty(workflowInstance.Metadata)) {
							instanceMetadata = DeserializeValues(workflowInstance.Metadata);
						}
						instanceState = InstanceState.Initialized;
					}
					if(data == null && !loadWorkflowCommand.AcceptUninitializedInstance) {
						throw new InvalidOperationException(string.Format(CanNotLoadUninitializedInstance, context.InstanceView.InstanceId));
					}
					context.BindAcquiredLock(0);
					Owner owner = owners[context.InstanceView.InstanceOwner.InstanceOwnerId];
					Guard.ArgumentNotNull(owner, "owner");
					if(instanceMetadata == null || instanceMetadata.Count == 0) {
						instanceMetadata = owner.CreateInstanceMetadata();
					}
					var associatedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
					var completedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
					foreach(IInstanceKey keyEntry in FindWorkflowInstanceKeys(objectSpace, context.InstanceView.InstanceId)) {
						associatedKeys.Add(keyEntry.KeyId, DeserializeValues(keyEntry.Properties));
					}
					DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("associatedKeys.Count", associatedKeys.Count);
					DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("instanceState", instanceState);
					context.LoadedInstance(instanceState, data, instanceMetadata, associatedKeys, completedKeys);
					runningInstances.Add(context.InstanceView.InstanceId);
				}
			}
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("<WorkflowInstanceStore.ProcessLoadWorkflowCommand");
		}
		private void ProcessSaveWorkflowCommand(InstancePersistenceContext context, SaveWorkflowCommand saveWorkflowCommand) {
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(">WorkflowInstanceStore.ProcessSaveWorkflowCommand");
			lock(LockObj) {
				IDictionary<System.Xml.Linq.XName, InstanceValue> data = null;
				data = saveWorkflowCommand.InstanceData;
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
					IWorkflowInstance workflowInstance = FindWorkflowInstance(objectSpace, context.InstanceView.InstanceId);
					if(workflowInstance == null) {
						workflowInstance = CreateWorkflowInstance(objectSpace);
						workflowInstance.InstanceId = context.InstanceView.InstanceId;
						Owner owner = owners[context.InstanceView.InstanceOwner.InstanceOwnerId];
						workflowInstance.Owner = owner.OwnerIdString;
					}
					if(!context.InstanceView.IsBoundToLock) {
						context.BindAcquiredLock(0);
					}
					if(saveWorkflowCommand.InstanceData.Count > 0) {
						context.PersistedInstance(saveWorkflowCommand.InstanceData);
					}
					foreach(var keyEntry in saveWorkflowCommand.InstanceKeysToAssociate) {
						IInstanceKey instanceKey = FindWorkflowInstanceKey(objectSpace, keyEntry.Key);
						if(instanceKey != null) {
							if(instanceKey.InstanceId != context.InstanceView.InstanceId) {
								throw new InstanceKeyCollisionException(saveWorkflowCommand.Name, context.InstanceView.InstanceId,
																		new InstanceKey(keyEntry.Key), instanceKey.InstanceId);
							}
						}
						else {
							instanceKey = CreateWorkflowInstanceKey(objectSpace);
							instanceKey.KeyId = keyEntry.Key;
							instanceKey.InstanceId = context.InstanceView.InstanceId;
							instanceKey.Properties = SerializeInstanceValues(keyEntry.Value);
						}
						context.AssociatedInstanceKey(keyEntry.Key);
						if(keyEntry.Value != null) {
							foreach(var property in keyEntry.Value) {
								context.WroteInstanceKeyMetadataValue(keyEntry.Key, property.Key, property.Value);
							}
						}
					}
					foreach(var keyGuid in saveWorkflowCommand.InstanceKeysToComplete) {
						context.CompletedInstanceKey(keyGuid);
					}
					foreach(var keyGuid in saveWorkflowCommand.InstanceKeysToFree) {
						FreeInstanceKey(objectSpace, keyGuid);
						context.UnassociatedInstanceKey(keyGuid);
					}
					foreach(var keyEntry in saveWorkflowCommand.InstanceKeyMetadataChanges) {
						if(keyEntry.Value != null) {
							foreach(var property in keyEntry.Value) {
								context.WroteInstanceKeyMetadataValue(keyEntry.Key, property.Key, property.Value);
							}
						}
					}
					foreach(var property in saveWorkflowCommand.InstanceMetadataChanges) {
						context.WroteInstanceMetadataValue(property.Key, property.Value);
					}
					workflowInstance.Status = CheckIsSuspendWithNoException(saveWorkflowCommand.InstanceMetadataChanges) ? ActivityInstanceState.Executing : ActivityInstanceState.Faulted;
					if(!saveWorkflowCommand.UnlockInstance || saveWorkflowCommand.InstanceData.Count > 0) {
						workflowInstance.Content = SerializeInstanceValues(data);
						workflowInstance.Metadata = SerializeInstanceValues(saveWorkflowCommand.InstanceMetadataChanges);
						workflowInstance.ExpirationDateTime = GetExpirationDateTime(saveWorkflowCommand);
					}
					if(saveWorkflowCommand.CompleteInstance) {
						foreach(var keyEntry in context.InstanceView.InstanceKeys) {
							if(keyEntry.Value.InstanceKeyState == InstanceKeyState.Associated) {
								FreeInstanceKey(objectSpace, keyEntry.Key); 
								context.CompletedInstanceKey(keyEntry.Key);
							}
						}
						context.CompletedInstance();
						workflowInstance.Status = GetWorkflowStatus(data);
						if(InstanceCompletionAction == System.Activities.DurableInstancing.InstanceCompletionAction.DeleteAll) {
							objectSpace.Delete(workflowInstance);
						}
					}
					if(saveWorkflowCommand.UnlockInstance) {
						context.InstanceHandle.Free();
						runningInstances.Remove(context.InstanceView.InstanceId);
					}
					objectSpace.CommitChanges();
					if(saveWorkflowCommand.CompleteInstance) {
						OnInstanceComplete(context.InstanceView.InstanceId, workflowInstance.Status, data);
					}
				}
			}
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("<WorkflowInstanceStore.ProcessSaveWorkflowCommand");
		}
		private DateTime? GetExpirationDateTime(SaveWorkflowCommand saveWorkflowCommand) {
			InstanceValue result;
			if(saveWorkflowCommand.InstanceData.TryGetValue(WorkflowNamespace.TimerExpirationTime, out result)) {
				return ((DateTime)result.Value).ToUniversalTime();
			}
			return null;
		}
		private void ProcessTryLoadRunnableWorkflowCommand(InstancePersistenceContext context, TryLoadRunnableWorkflowCommand tryLoadRunnableWorkflowCommand) {
			lock(LockObj) {
				IDictionary<System.Xml.Linq.XName, InstanceValue> data = null;
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
					IWorkflowInstance workflowInstance = FindRunnableInstance(objectSpace, context.InstanceView.InstanceOwner.InstanceOwnerId);
					if(workflowInstance != null) {
						timer.Enabled = false;
						data = DeserializeValues(workflowInstance.Content);
						if(!context.InstanceView.IsBoundToInstance) {
							context.BindInstance(workflowInstance.InstanceId);
						}
						if(!context.InstanceView.IsBoundToInstanceOwner) {
							context.BindInstanceOwner(context.InstanceView.InstanceOwner.InstanceOwnerId, context.InstanceView.InstanceOwner.InstanceOwnerId);
						}
						if(!context.InstanceView.IsBoundToLock) {
							context.BindAcquiredLock(0);
						}
						Owner owner = owners[context.InstanceView.InstanceOwner.InstanceOwnerId];
						Guard.ArgumentNotNull(owner, "owner");
						Dictionary<XName, InstanceValue> instanceMetadata = owner.CreateInstanceMetadata();
						var associatedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
						var completedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
						foreach(IInstanceKey keyEntry in FindWorkflowInstanceKeys(objectSpace, workflowInstance.InstanceId)) {
							associatedKeys.Add(keyEntry.KeyId, DeserializeValues(keyEntry.Properties));
						}
						context.LoadedInstance(InstanceState.Initialized, data, instanceMetadata, associatedKeys, completedKeys);
						runningInstances.Add(context.InstanceView.InstanceId);
					}
					else {
						ResetEvent(HasRunnableWorkflowEvent.Value, context.InstanceView.InstanceOwner);
						timer.Enabled = true;
					}
				}
			}
		}
		private void ProcessLoadWorkflowByInstanceCommand(InstancePersistenceContext context, LoadWorkflowByInstanceKeyCommand loadWorkflowByInstanceCommand) {
			lock(LockObj) {
				IDictionary<System.Xml.Linq.XName, InstanceValue> data = null;
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
					Guid instanceId = loadWorkflowByInstanceCommand.LookupInstanceKey;
					IInstanceKey instanceKey = FindWorkflowInstanceKey(objectSpace, loadWorkflowByInstanceCommand.LookupInstanceKey);
					if(instanceKey != null) {
						instanceId = instanceKey.InstanceId;
					}
					if(!runningInstances.Contains(instanceId)) {
						IWorkflowInstance workflowInstance = FindWorkflowInstance(objectSpace, instanceId);
						if(workflowInstance != null) {
							data = DeserializeValues(workflowInstance.Content);
						}
					}
					if(data == null && !loadWorkflowByInstanceCommand.AcceptUninitializedInstance) {
						throw new InvalidOperationException(string.Format(CanNotLoadUninitializedInstance, instanceId));
					}
					var associatedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
					var completedKeys = new Dictionary<Guid, IDictionary<XName, InstanceValue>>();
					foreach(IInstanceKey keyEntry in FindWorkflowInstanceKeys(objectSpace, instanceId)) {
						associatedKeys.Add(keyEntry.KeyId, DeserializeValues(keyEntry.Properties));
					}
					if(!context.InstanceView.IsBoundToInstance) {
						context.BindInstance(instanceId);
					}
					if(!context.InstanceView.IsBoundToInstanceOwner) {
						context.BindInstanceOwner(context.InstanceView.InstanceOwner.InstanceOwnerId, context.InstanceView.InstanceOwner.InstanceOwnerId);
					}
					if(!context.InstanceView.IsBoundToLock) {
						context.BindAcquiredLock(0);
					}
					runningInstances.Add(context.InstanceView.InstanceId);
					Owner owner = owners[context.InstanceView.InstanceOwner.InstanceOwnerId];
					Guard.ArgumentNotNull(owner, "owner");
					Dictionary<XName, InstanceValue> instanceMetadata = owner.CreateInstanceMetadata();
					context.LoadedInstance(data != null ? InstanceState.Initialized : InstanceState.Uninitialized, data, instanceMetadata, associatedKeys, completedKeys);
				}
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace()) {
					foreach(KeyValuePair<Guid, IDictionary<XName, InstanceValue>> keyEntry in loadWorkflowByInstanceCommand.InstanceKeysToAssociate) {
						IInstanceKey instanceKey = FindWorkflowInstanceKey(objectSpace, keyEntry.Key);
						if(instanceKey != null) {
							if(instanceKey.InstanceId != context.InstanceView.InstanceId) {
								throw new InstanceKeyCollisionException(loadWorkflowByInstanceCommand.Name, context.InstanceView.InstanceId,
																		new InstanceKey(keyEntry.Key), instanceKey.InstanceId);
							}
						}
						else {
							instanceKey = CreateWorkflowInstanceKey(objectSpace);
						}
						instanceKey.KeyId = keyEntry.Key;
						instanceKey.InstanceId = context.InstanceView.InstanceId;
						instanceKey.Properties = SerializeInstanceValues(keyEntry.Value);
						context.AssociatedInstanceKey(keyEntry.Key);
						foreach(KeyValuePair<XName, InstanceValue> property in keyEntry.Value) {
							context.WroteInstanceKeyMetadataValue(keyEntry.Key, property.Key, property.Value);
						}
					}
					objectSpace.CommitChanges();
				}
			}
		}
		protected override bool EndTryCommand(IAsyncResult result) {
			DevExpress.Utils.Guard.ArgumentNotNull(result, "result");
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(">EndTryCommand");
			WorkflowStoreCompletedAsyncResult wfResult = (WorkflowStoreCompletedAsyncResult)result;
			wfResult.End();
			if(wfResult.Command != null) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseValue("command.GetType()", wfResult.Command);
			}
			DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("<EndTryCommand");
			return true;
		}
		[Obsolete("Use the 'WorkflowInstanceStore(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider)' constructor instead.", true)]
		public WorkflowInstanceStore(IObjectSpaceProvider objectSpaceProvider) : this(null, null, objectSpaceProvider) { }
		public WorkflowInstanceStore(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider) {
			Guard.ArgumentNotNull(workflowInstanceType, "workflowInstanceType");
			if(!typeof(IWorkflowInstance).IsAssignableFrom(workflowInstanceType)) {
				throw new ArgumentException("workflowInstanceType is not IWorkflowInstance");
			}
			this.workflowInstanceType = workflowInstanceType;
			Guard.ArgumentNotNull(workflowInstanceKeyType, "workflowInstanceKeyType");
			if(!typeof(IInstanceKey).IsAssignableFrom(workflowInstanceKeyType)) {
				throw new ArgumentException("workflowInstanceKeyType is not IInstanceKey");
			}
			this.workflowInstanceKeyType = workflowInstanceKeyType;
			Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
			this.objectSpaceProvider = objectSpaceProvider;
			objectSpaceProvider.TypesInfo.RegisterEntity(workflowInstanceType);
			objectSpaceProvider.TypesInfo.RegisterEntity(workflowInstanceKeyType);
			timer = new Timer();
			timer.AutoReset = true;
			timer.Interval = RunnableInstancesDetectionPeriod.TotalMilliseconds;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Enabled = true;
		}
		[DefaultValue(InstanceCompletionAction.DeleteAll)]
		public InstanceCompletionAction InstanceCompletionAction {
			get { return instanceCompletionAction; }
			set { instanceCompletionAction = value; }
		}
		[DefaultValue("00:00:30")]
		public TimeSpan RunnableInstancesDetectionPeriod {
			get { return runnableInstancesDetectionPeriod; }
			set {
				if(runnableInstancesDetectionPeriod != value) {
					timer.Enabled = false;
					runnableInstancesDetectionPeriod = value;
					timer.Interval = runnableInstancesDetectionPeriod.TotalMilliseconds;
					timer.Enabled = true;
				}
			}
		}
		public event EventHandler<InstanceEventArgs> InstanceComplete;
#if DebugTest
	public IObjectSpaceProvider DebugTest_ObjectSpaceProvider { get { return objectSpaceProvider; } }
#endif
	}
	public class InstanceEventArgs : EventArgs {
		public InstanceEventArgs(Guid id, ActivityInstanceState status, IDictionary<XName, InstanceValue> data) {
			Id = id;
			Status = status;
			Data = data;
		}
		public Guid Id { get; private set; }
		public ActivityInstanceState Status { get; private set; }
		public IDictionary<XName, InstanceValue> Data { get; private set; }
	}
}
