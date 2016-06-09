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
using System.Xml.Linq;
namespace DevExpress.Workflow.Utils {
	internal static class WorkflowNamespace {
		private const string baseNamespace = "urn:schemas-microsoft-com:System.Activities/4.0/properties";
		private static XName bookmarks;
		private static XName exception;
		private static XName keyProvider;
		private static XName lastUpdate;
		private static readonly XNamespace outputNamespace =
			XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties/output");
		private static XName status;
		private static readonly XNamespace variablesNamespace =
			XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties/variables");
		private static XName workflow;
		private static XName workflowHostType;
		private static XName timerExpirationTime;
		private static readonly XNamespace workflowNamespace =
			XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties");
		public static XName Bookmarks {
			get {
				if(bookmarks == null) {
					bookmarks = workflowNamespace.GetName("Bookmarks");
				}
				return bookmarks;
			}
		}
		public static XName Exception {
			get {
				if(exception == null) {
					exception = workflowNamespace.GetName("Exception");
				}
				return exception;
			}
		}
		public static XName KeyProvider {
			get {
				if(keyProvider == null) {
					keyProvider = workflowNamespace.GetName("KeyProvider");
				}
				return keyProvider;
			}
		}
		public static XName LastUpdate {
			get {
				if(lastUpdate == null) {
					lastUpdate = workflowNamespace.GetName("LastUpdate");
				}
				return lastUpdate;
			}
		}
		public static XNamespace OutputPath {
			get { return outputNamespace; }
		}
		public static XName Status {
			get {
				if(status == null) {
					status = workflowNamespace.GetName("Status");
				}
				return status;
			}
		}
		public static XNamespace VariablesPath {
			get { return variablesNamespace; }
		}
		public static XName Workflow {
			get {
				if(workflow == null) {
					workflow = workflowNamespace.GetName("Workflow");
				}
				return workflow;
			}
		}
		public static XName WorkflowHostType {
			get {
				if(workflowHostType == null) {
					workflowHostType = workflowNamespace.GetName("WorkflowHostType");
				}
				return workflowHostType;
			}
		}
		public static XName TimerExpirationTime {
			get {
				if(timerExpirationTime == null) {
					timerExpirationTime = workflowNamespace.GetName("TimerExpirationTime");
				}
				return timerExpirationTime;
			}
		}
	}
#if DebugTest
	public static class WorkflowNamespaceForTest {
		public static XName WorkflowHostType {
			get { return WorkflowNamespace.WorkflowHostType;}
		}
	}
#endif
}
