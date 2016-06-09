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
using DevExpress.Utils;
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	class ServiceBusItemsProvider : IServiceBusItemsProvider {
		NamespaceManager namespaceManager;
		MessagingFactory messagingFactory;
		NamespaceManager NamespaceManager {
			get {
				if(namespaceManager == null) {
					namespaceManager = NamespaceManager.CreateFromConnectionString(connectionProvider.ServiceBusConnectionString);
				}
				return namespaceManager;
			}
		}
		MessagingFactory MessagingFactory {
			get {
				if(messagingFactory == null) {
					messagingFactory = MessagingFactory.CreateFromConnectionString(connectionProvider.ServiceBusConnectionString);
					CreateQueuesIfNotExist();
				}
				return messagingFactory;
			}
		}
		readonly AzureConnectionStringProvider connectionProvider;
		SubscriptionClient topicSubscription;
		TopicClient requestsTopicClient;
		public ServiceBusItemsProvider(AzureConnectionStringProvider connectionProvider) {
			Guard.ArgumentNotNull(connectionProvider, "connectionProvider");
			this.connectionProvider = connectionProvider;
		}
		public virtual string RequestQueueName {
			get { return ServiceConstatns.DefaultRequestsQueueName; }
		}
		public virtual string ResponseQueueName {
			get { return ServiceConstatns.DefaultResponsesQueueName; }
		}
		public TopicClient GetTopicClient() {
			if(requestsTopicClient == null) {
				CreateTopicIfNotExists();
				requestsTopicClient = TopicClient.CreateFromConnectionString(connectionProvider.ServiceBusConnectionString, ServiceConstatns.DefaultRequestsTopicName);
			}
			return requestsTopicClient;
		}
		public SubscriptionClient GetTopicSubscription() {
			if(topicSubscription == null) {
				CreateTopicIfNotExists();
				CreateSubscriptionIfNotExists();
				topicSubscription = SubscriptionClient.CreateFromConnectionString(connectionProvider.ServiceBusConnectionString, ServiceConstatns.DefaultRequestsTopicName, ServiceConstatns.DefaultTopicSubscriptionName);
			}
			return topicSubscription;
		}
		public MessagingFactory GetMessagingFactory() {
			return MessagingFactory;
		}
		public void CreateQueuesIfNotExist() {
			if(!NamespaceManager.QueueExists(RequestQueueName)) {
				var requestsQueueDescription = new QueueDescription(RequestQueueName) {
					RequiresSession = true,
					DefaultMessageTimeToLive = TimeSpan.FromMinutes(90)
				};
				NamespaceManager.CreateQueue(requestsQueueDescription);
			}
			if(!NamespaceManager.QueueExists(ResponseQueueName)) {
				var responsesQueueDescription = new QueueDescription(ResponseQueueName) {
					RequiresSession = true,
					DefaultMessageTimeToLive = TimeSpan.FromMinutes(90)
				};
				NamespaceManager.CreateQueue(responsesQueueDescription);
			}
		}
		void CreateTopicIfNotExists() {
			if(!NamespaceManager.TopicExists(ServiceConstatns.DefaultRequestsTopicName)) {
				NamespaceManager.CreateTopic(ServiceConstatns.DefaultRequestsTopicName);
			}
		}
		void CreateSubscriptionIfNotExists() {
			if(!NamespaceManager.SubscriptionExists(ServiceConstatns.DefaultRequestsTopicName, ServiceConstatns.DefaultTopicSubscriptionName)) {
				NamespaceManager.CreateSubscription(ServiceConstatns.DefaultRequestsTopicName, ServiceConstatns.DefaultTopicSubscriptionName);
			}
		}
	}
}
