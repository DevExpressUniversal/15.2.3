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
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.DC;
using DevExpress.Workflow.Xpo;
namespace DevExpress.ExpressApp.Workflow.Xpo {
	[Persistent]
	[System.ComponentModel.DisplayName("Running Workflow Instance Info")]
	public class XpoRunningWorkflowInstanceInfo : WFBaseObject, IDCRunningWorkflowInstanceInfo {
		public const string ActivityInstanceIdPropertyName = "ActivityInstanceId";
		public XpoRunningWorkflowInstanceInfo(Session session) : base(session) { }
		[Size(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string WorkflowName {
			get { return GetPropertyValue<string>("WorkflowName"); }
			set { SetPropertyValue<string>("WorkflowName", value); }
		}
		[Size(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string WorkflowUniqueId {
			get { return GetPropertyValue<string>("WorkflowUniqueId"); }
			set { SetPropertyValue<string>("WorkflowUniqueId", value); }
		}
		[Size(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string TargetObjectHandle {
			get { return GetPropertyValue<string>("TargetObjectHandle"); }
			set { SetPropertyValue<string>("TargetObjectHandle", value); }
		}
		public Guid ActivityInstanceId {
			get { return GetPropertyValue<Guid>(ActivityInstanceIdPropertyName); }
			set { SetPropertyValue<Guid>(ActivityInstanceIdPropertyName, value); }
		}
		public string State {
			get { return GetPropertyValue<string>("State"); }
			set { SetPropertyValue<string>("State", value); }
		}
		public XPCollection<XpoTrackingRecord> TrackingRecords {
			get {
				XPCollection<XpoTrackingRecord> result = new XPCollection<XpoTrackingRecord>(Session, new BinaryOperator("InstanceId", ActivityInstanceId));
				result.BindingBehavior = CollectionBindingBehavior.AllowNone;
				return result; 
			}
		}
		public void SetTrackingRecordVisualizationInfo(ITrackingRecordVisualizationInfo info) {
			TrackingRecordVisualizationInfo = info;
		}
		[NonPersistent, VisibleInListView(false), VisibleInLookupListView(false)]
		public ITrackingRecordVisualizationInfo TrackingRecordVisualizationInfo { get; private set; }
	}
}
