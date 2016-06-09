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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Globalization;
using System.Activities;
namespace DevExpress.Workflow {
	public class WorkflowCreationKnownTypesProvider {
		private static List<Type> knownTypes = new List<Type>();
		private static bool isFixed = false;
		public static IEnumerable<Type> GetKnownTypes(System.Reflection.ICustomAttributeProvider provider) {
			isFixed = true;
			return knownTypes;
		}
		public static void AddKnownType(Type type) {
			if(isFixed) {
				throw new InvalidOperationException();
			}
			knownTypes.Add(type);
		}
	}
	[ServiceContract(Name = "IWorkflowCreation")]
	[ServiceKnownType("GetKnownTypes", typeof(WorkflowCreationKnownTypesProvider))]
	public interface IWorkflowCreation {
		[OperationContract(Name = "Create")]
		Guid Create(IDictionary<string, object> inputs);
		[OperationContract(Name = "CreateSuspended")]
		Guid CreateSuspended(IDictionary<string, object> inputs);
		[OperationContract(Name = "ResumeBookmark", IsOneWay = true)]
		void ResumeBookmark(Guid instanceId, string bookmarkName, object data);
	}
	public class CreationEndpoint : WorkflowHostingEndpoint {
		private const string CreationSuffix = "Creation";
		static Uri defaultBaseUri;
		static Uri DefaultBaseUri {
			get {
				if(defaultBaseUri == null) {
					defaultBaseUri = new Uri(string.Format(CultureInfo.InvariantCulture, "net.pipe://localhost/workflowCreationEndpoint/{0}/{1}", Process.GetCurrentProcess().Id, AppDomain.CurrentDomain.Id));
				}
				return defaultBaseUri;
			}
		}
		private void InitializeCreationContext(WorkflowCreationContext creationContext, WorkflowHostingResponseContext responseContext, Guid instanceId, object[] inputs) {
			Dictionary<string, object> arguments = (Dictionary<string, object>)inputs[0];
			if(arguments != null && arguments.Count > 0) {
				foreach(KeyValuePair<string, object> pair in arguments) {
					creationContext.WorkflowArguments.Add(pair.Key, pair.Value);
				}
			}
			responseContext.SendResponse(instanceId, null);
		}
		protected override Guid OnGetInstanceId(object[] inputs, OperationContext operationContext) {
			if(operationContext.IncomingMessageHeaders.Action.EndsWith("Create") || operationContext.IncomingMessageHeaders.Action.EndsWith("CreateSuspended")) {
				return new Guid();
			}
			else if(operationContext.IncomingMessageHeaders.Action.EndsWith("ResumeBookmark")) {
				return (Guid)inputs[0];
			}
			else {
				throw new InvalidOperationException("Invalid Action: " + operationContext.IncomingMessageHeaders.Action);
			}
		}
		protected override WorkflowCreationContext OnGetCreationContext(object[] inputs, OperationContext operationContext, Guid instanceId, WorkflowHostingResponseContext responseContext) {
			WorkflowCreationContext creationContext = new WorkflowCreationContext();
			if(operationContext.IncomingMessageHeaders.Action.EndsWith("Create")) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("CreationEndpoint.Create");
				InitializeCreationContext(creationContext, responseContext, instanceId, inputs);
			}
			else if(operationContext.IncomingMessageHeaders.Action.EndsWith("CreateSuspended")) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText("CreationEndpoint.CreateSuspended");
				creationContext.CreateOnly = true;
				InitializeCreationContext(creationContext, responseContext, instanceId, inputs);
			}
			else {
				throw new InvalidOperationException("Invalid Action: " + operationContext.IncomingMessageHeaders.Action);
			}
			return creationContext;
		}
		protected override System.Activities.Bookmark OnResolveBookmark(object[] inputs, OperationContext operationContext, WorkflowHostingResponseContext responseContext, out object value) {
			Bookmark bookmark = null;
			value = null;
			if(operationContext.IncomingMessageHeaders.Action.EndsWith("ResumeBookmark")) {
				bookmark = new Bookmark((string)inputs[1]);
				value = inputs[2];
			}
			else {
				throw new NotImplementedException(operationContext.IncomingMessageHeaders.Action);
			}
			return bookmark;
		}
		public CreationEndpoint(Binding binding, EndpointAddress address) : base(typeof(IWorkflowCreation), binding, address) { }
		public CreationEndpoint()
			: this(GetDefaultBinding(), new EndpointAddress(new Uri(DefaultBaseUri, new Uri(Guid.NewGuid().ToString(), UriKind.Relative)))) {
		}
		public static Binding GetDefaultBinding() {
			return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None) { TransactionFlow = true };
		}
		public static CreationEndpoint Create(string workflowHostUri) {
			return new CreationEndpoint(new BasicHttpBinding(), new EndpointAddress(new Uri(workflowHostUri + "/" + CreationSuffix)));
		}
		public static IWorkflowCreation GetCreationClient(string workflowHostUri) {
			CreationEndpoint creationEndpoint = new CreationEndpoint(new BasicHttpBinding(), new EndpointAddress(new Uri(workflowHostUri + "/" + CreationSuffix)));
			return new ChannelFactory<IWorkflowCreation>(creationEndpoint.Binding, creationEndpoint.Address).CreateChannel();
		}
	}
}
