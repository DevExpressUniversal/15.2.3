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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Persistent.Base;
using DevExpress.Workflow.EF;
namespace DevExpress.ExpressApp.Workflow.EF {
	[System.ComponentModel.DisplayName("Running Workflow Instance Info")]
	public class EFRunningWorkflowInstanceInfo : EFWBaseObject, IRunningWorkflowInstanceInfo, IObjectSpaceLink {
		private IObjectSpace objectSpace;
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		[Browsable(false)]
		public int Id { get; protected set; }
		[FieldSize(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string WorkflowName { get; set; }
		[FieldSize(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string WorkflowUniqueId { get; set; }
		[FieldSize(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		public string TargetObjectHandle { get; set; }
		public Guid ActivityInstanceId { get; set; }
		public string State { get; set; }		
		[NotMapped]
		public virtual IList<EFTrackingRecord> TrackingRecords {
			get {
				IList<EFTrackingRecord> result = objectSpace.GetObjects<EFTrackingRecord>(new BinaryOperator("InstanceId", ActivityInstanceId),  false);  
				return result;
			}
		}
		public void SetTrackingRecordVisualizationInfo(ITrackingRecordVisualizationInfo info) {
			TrackingRecordVisualizationInfo = info;
		}
		[NotMapped, VisibleInListView(false), VisibleInLookupListView(false)]
		public ITrackingRecordVisualizationInfo TrackingRecordVisualizationInfo { get; private set; }
	}
}
