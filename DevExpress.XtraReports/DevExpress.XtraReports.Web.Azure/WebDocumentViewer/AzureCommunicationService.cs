#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using Microsoft.ServiceBus.Messaging;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureCommunicationService : IAzureCommunicationService {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		TimeSpan longServerBuildTimeOut = TimeSpan.FromHours(1);
		readonly IAzureEntityStorageManager entityStorageManager;
		readonly IServiceBusItemsProvider serviceBusItemsProvider;
		readonly IAzureFileStorageManager fileStorageManager;
		public AzureCommunicationService(IAzureEntityStorageManager entityStorageManager, IServiceBusItemsProvider serviceBusItemsProvider, IAzureFileStorageManager fileStorageManager) {
			this.entityStorageManager = entityStorageManager;
			this.serviceBusItemsProvider = serviceBusItemsProvider;
			this.fileStorageManager = fileStorageManager;
		}
		#region IBuildTimeRequestListener
		public void ProcessIncomingSessionRequests(string id, Func<bool> stopPredicate, Dictionary<string, TypedAction> sessionActions, Dictionary<string, TypedAction> broadcastActions) {
			var stopWatch = System.Diagnostics.Stopwatch.StartNew();
			if(sessionActions != null && sessionActions.Count > 0)
				Task.Factory.StartNew(() => {
					try {
						var messagingFactory = serviceBusItemsProvider.GetMessagingFactory();
						QueueClient requestSessionQueueClient = messagingFactory.CreateQueueClient(serviceBusItemsProvider.RequestQueueName, ReceiveMode.ReceiveAndDelete);
						var messageSession = requestSessionQueueClient.AcceptMessageSession(id, TimeSpan.FromSeconds(10));
						BrokeredMessage message;
						Action<object, string, string> pushResponseAction = (result, actionName, replyToSessionId) => {
							QueueClient responseSessionQueueClient = messagingFactory.CreateQueueClient(serviceBusItemsProvider.ResponseQueueName, ReceiveMode.ReceiveAndDelete);
							if(result == null)
								return;
							var responseMessage = new BrokeredMessage();
							responseMessage.SessionId = replyToSessionId;
							responseMessage.MessageId = GetNewGuid();
							responseMessage.Label = "Response.";
							responseMessage.Properties[ServiceConstatns.FaultMessagePropertyName] = null;
							if(result is byte[]) {
								var file = (byte[])result;
								string filePath;
								using(var memoryStream = new MemoryStream()) {
									memoryStream.Read(file, 0, file.Length);
									filePath = fileStorageManager.SaveFile(null, memoryStream);
								}
								responseMessage.Properties[ServiceConstatns.BlobPathPropertyName] = filePath;
							} else {
								responseMessage.Properties[ServiceConstatns.JsonArgumentPropertyName] = result;
							}
							responseSessionQueueClient.Send(responseMessage);
						};
						while(stopWatch.Elapsed < longServerBuildTimeOut) {
							if(stopPredicate()) {
								while((message = messageSession.Receive(TimeSpan.FromSeconds(5))) != null)
									ProcessRequest(message, sessionActions, pushResponseAction);
								break;
							}
							message = messageSession.Receive(TimeSpan.FromSeconds(5));
							if(message != null)
								ProcessRequest(message, sessionActions, pushResponseAction);
						}
						messageSession.Close();
					} catch(Exception ex) {
						Logger.Error(ex.ToString());
					}
				}, TaskCreationOptions.LongRunning);
			if(broadcastActions != null && broadcastActions.Count > 0) {
				Task.Factory.StartNew(() => {
					try {
						var topicSubscription = serviceBusItemsProvider.GetTopicSubscription();
						while(stopWatch.Elapsed < longServerBuildTimeOut) {
							if(stopPredicate()) {
								break;
							}
							BrokeredMessage message = null;
							while((message = topicSubscription.Receive(TimeSpan.FromSeconds(10))) != null) {
								ProcessBroadcastRequestCore(message, broadcastActions);
							}
						}
					} catch(Exception ex) {
						Logger.Error(ex.ToString());
					}
				}, TaskCreationOptions.LongRunning);
			}
		}
		public void ProcessIncomingBroadcastMessages(string entityId, Func<bool> stopPredicate, Dictionary<string, TypedAction> broadcastActions) {
			var stopWatch = System.Diagnostics.Stopwatch.StartNew();
			Task.Factory.StartNew(() => {
				try {
					var topicSubscription = serviceBusItemsProvider.GetTopicSubscription();
					while(stopWatch.Elapsed < longServerBuildTimeOut) {
						if(stopPredicate()) {
							break;
						}
						BrokeredMessage message = null;
						while((message = topicSubscription.Receive(TimeSpan.FromSeconds(10))) != null) {
							object entityIdFromMessage;
							if(message.Properties.TryGetValue(ServiceConstatns.EntityId, out entityIdFromMessage)
								&& (string)entityIdFromMessage == entityId) {
								ProcessBroadcastRequestCore(message, broadcastActions);
							}
						}
					}
				} catch(Exception ex) {
					Logger.Error(ex.ToString());
				}
			}, TaskCreationOptions.LongRunning);
		}
		public void SendBroadcastMessage(string id, string actionName, string jsonArgs, TimeSpan? timeToLife) {
			try {
				var topicClient = serviceBusItemsProvider.GetTopicClient();
				var brokeredMessage = new BrokeredMessage();
				brokeredMessage.Properties[ServiceConstatns.EntityId] = id;
				brokeredMessage.Properties[ServiceConstatns.JsonArgumentPropertyName] = jsonArgs;
				brokeredMessage.Properties[ServiceConstatns.ActionNamePropertyName] = actionName;
				if(timeToLife.HasValue)
					brokeredMessage.TimeToLive = timeToLife.Value;
				topicClient.Send(brokeredMessage);
			} catch(Exception e) {
				Logger.Error(e.ToString());
			}
		}
		public void Request(string documentId, string actionName, string jsonArgs, TimeSpan? messageTimeToLife) {
			var messagingFactory = serviceBusItemsProvider.GetMessagingFactory();
			QueueClient requestSessionQueueClient = messagingFactory.CreateQueueClient(serviceBusItemsProvider.RequestQueueName, ReceiveMode.ReceiveAndDelete);
			var requestMessage = new BrokeredMessage() { MessageId = GetNewGuid() };
			if(messageTimeToLife.HasValue)
				requestMessage.TimeToLive = messageTimeToLife.Value;
			requestMessage.SessionId = documentId;
			requestMessage.Properties[ServiceConstatns.JsonArgumentPropertyName] = jsonArgs;
			requestMessage.Properties[ServiceConstatns.ActionNamePropertyName] = actionName;
			requestSessionQueueClient.Send(requestMessage);
		}
		public T Request<T>(string documentId, string actionName, string jsonArgs, int timeoutSeconds, TimeSpan? messageTimeToLife) where T : class {
			var messagingFactory = serviceBusItemsProvider.GetMessagingFactory();
			string replyToSessionId = GetNewGuid();
			QueueClient requestSessionQueueClient = messagingFactory.CreateQueueClient(serviceBusItemsProvider.RequestQueueName, ReceiveMode.ReceiveAndDelete);
			var requestMessage = new BrokeredMessage();
			if(messageTimeToLife.HasValue)
				requestMessage.TimeToLive = messageTimeToLife.Value;
			requestMessage.SessionId = documentId;
			requestMessage.ReplyToSessionId = replyToSessionId;
			requestMessage.Label = "Request. " + actionName;
			requestMessage.Properties[ServiceConstatns.JsonArgumentPropertyName] = jsonArgs;
			requestMessage.Properties[ServiceConstatns.ActionNamePropertyName] = actionName;
			requestMessage.MessageId = GetNewGuid();
			requestSessionQueueClient.Send(requestMessage);
			QueueClient responseSessionQueueClient = messagingFactory.CreateQueueClient(serviceBusItemsProvider.ResponseQueueName, ReceiveMode.ReceiveAndDelete);
			var messagesSession = responseSessionQueueClient.AcceptMessageSession(replyToSessionId, TimeSpan.FromSeconds(10));
			var message = messagesSession.Receive(TimeSpan.FromSeconds(timeoutSeconds));
			if(message == null)
				return null;
			return ProcessResponse<T>(message);
		}
		#endregion
		T ProcessResponse<T>(BrokeredMessage message) where T : class {
			if(typeof(T) == typeof(byte[])) {
				var fileSource = message.Properties[ServiceConstatns.BlobPathPropertyName] as string;
				var file = fileStorageManager.GetFile(fileSource);
				using(var fileStream = new MemoryStream())
					return fileStream.ToArray() as T;
			} else {
				string faultMessage = null;
				if(message.Properties.ContainsKey(ServiceConstatns.FaultMessagePropertyName)) {
					faultMessage = message.Properties[ServiceConstatns.FaultMessagePropertyName] as string;
					if(!string.IsNullOrEmpty(faultMessage))
						throw new Exception(faultMessage);
				}
				var jsonResponse = message.Properties[ServiceConstatns.JsonArgumentPropertyName] as string;
				return ActionHelper.Read<T>(jsonResponse);
			}
		}
		void ProcessRequest(BrokeredMessage message, Dictionary<string, TypedAction> actions, Action<object, string, string> pushResponseAction) {
			object actionNameProp;
			object jsonArgumentsProp;
			string replyToSessionId = message.ReplyToSessionId;
			if(message.Properties.TryGetValue(ServiceConstatns.ActionNamePropertyName, out actionNameProp) && message.Properties.TryGetValue(ServiceConstatns.JsonArgumentPropertyName, out jsonArgumentsProp)) {
				string actionName = actionNameProp as string;
				string jsonArguments = jsonArgumentsProp as string;
				ProcessRequestCore(actionName, jsonArguments, replyToSessionId, actions, pushResponseAction);
			} else {
				pushResponseAction(null, actionNameProp as string, replyToSessionId);
			}
		}
		void ProcessBroadcastRequestCore(BrokeredMessage message, Dictionary<string, TypedAction> actions) {
			try {
				object actionNameProp;
				object jsonArgumentsProp;
				if(message.Properties.TryGetValue(ServiceConstatns.ActionNamePropertyName, out actionNameProp) && message.Properties.TryGetValue(ServiceConstatns.JsonArgumentPropertyName, out jsonArgumentsProp)) {
					string actionName = actionNameProp as string;
					string jsonArguments = jsonArgumentsProp as string;
					ProcessBroadcastCore(actionName, jsonArguments, actions);
				}
			} catch(Exception ex) {
				Logger.Error(ex.ToString());
			}
		}
		void ProcessBroadcastCore(string actionName, string jsonArguments, Dictionary<string, TypedAction> actions) {
			TypedAction managerAction;
			if(!actions.TryGetValue(actionName, out managerAction)) {
				throw new ArgumentException("actionName");
			}
			var actionResult = managerAction.Action(jsonArguments);
			if(actionResult != null) {
				throw new NotSupportedException();
			}
		}
		void ProcessRequestCore(string actionName, string jsonArguments, string replyToSessionId, Dictionary<string, TypedAction> actions, Action<object, string, string> pushResponseAction) {
			object response = null;
			try {
				TypedAction managerAction;
				if(!actions.TryGetValue(actionName.ToLower(), out managerAction)) {
					throw new ArgumentException("actionName");
				}
				if(managerAction.ReturnType != null) {
					var actionResult = managerAction.Action(jsonArguments);
					if(actionResult == null)
						return;
					if(managerAction.ReturnType == typeof(byte[])) {
						response = actionResult;
					} else {
						var serializer = new DataContractJsonSerializer(managerAction.ReturnType);
						using(var stream = new MemoryStream()) {
							serializer.WriteObject(stream, actionResult);
							stream.Position = 0;
							using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
								response = reader.ReadToEnd();
						}
					}
				}
			} finally {
				if(!string.IsNullOrEmpty(replyToSessionId))
					pushResponseAction(response, actionName, replyToSessionId);
				else {
					throw new ArgumentNullException("replyToSessionId");
				}
			}
		}
		string GetNewGuid() {
			return Guid.NewGuid().ToString("N");
		}
	}
}
