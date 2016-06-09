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
using DevExpress.ExpressApp.Workflow.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.Workflow.Xpo {
	[System.ComponentModel.DisplayName("Workflow Instance Control Command Request")]
	public class XpoWorkflowInstanceControlCommandRequest : WFBaseObject, IDCWorkflowInstanceControlCommandRequest {
		public XpoWorkflowInstanceControlCommandRequest(Session session) : base(session) { }
		public string TargetWorkflowUniqueId { get { return GetPropertyValue<string>("TargetWorkflowUniqueId"); } set { SetPropertyValue<string>("TargetWorkflowUniqueId", value); } }
		public Guid TargetActivityInstanceId { get { return GetPropertyValue<Guid>("TargetActivityInstanceId"); } set { SetPropertyValue<Guid>("TargetActivityInstanceId", value); } }
		public WorkflowControlCommand Command { get { return GetPropertyValue<WorkflowControlCommand>("Command"); } set { SetPropertyValue<WorkflowControlCommand>("Command", value); } }
		public DateTime CreatedOn { get { return GetPropertyValue<DateTime>("CreatedOn"); } set { SetPropertyValue<DateTime>("CreatedOn", value); } }
		[Size(SizeAttribute.Unlimited)]
		public string Result { get { return GetPropertyValue<string>("Result"); } set { SetPropertyValue<string>("Result", value); } }
	}
}
