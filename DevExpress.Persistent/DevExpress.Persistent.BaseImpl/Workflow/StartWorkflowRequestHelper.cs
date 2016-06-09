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
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
namespace DevExpress.Persistent.BaseImpl.Workflow {
	public class StartWorkflowRequestHelper {
		public static IList<StartWorkflowRequestType> CreateStartWorkflowRequestWhenObjectIsCreated<StartWorkflowRequestType, WorkflowDefinitionType>(object targetObject, Session session)
			where StartWorkflowRequestType : DevExpress.ExpressApp.Workflow.DC.IDCStartWorkflowRequest
			where WorkflowDefinitionType : IWorkflowDefinition {
			List<StartWorkflowRequestType> result = new List<StartWorkflowRequestType>();
			if(targetObject != null && session != null && !(session is NestedUnitOfWork) && session.IsNewObject(targetObject)) {
				XPCollection<WorkflowDefinitionType> definitions = new XPCollection<WorkflowDefinitionType>(session);
				definitions.Criteria = new BinaryOperator("IsActive", true);
				foreach(WorkflowDefinitionType definition in definitions) {
					foreach(IStartWorkflowCondition condition in definition.GetConditions()) {
						ObjectCreatedStartWorkflowCondition objectCreatedCondition = condition as ObjectCreatedStartWorkflowCondition;
						if(objectCreatedCondition != null && objectCreatedCondition.TargetObjectType.IsAssignableFrom(targetObject.GetType())) {
							StartWorkflowRequestType request = (StartWorkflowRequestType)session.GetClassInfo<StartWorkflowRequestType>().CreateNewObject(session);
							request.TargetObjectKey = session.GetKeyValue(targetObject);
							request.TargetWorkflowUniqueId = objectCreatedCondition.TargetWorkflowUniqueId;
							result.Add(request);
						}
					}
				}
			}
			return result;
		}
	}
}
