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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Workflow.CommonServices {
	public interface IWorkflowDefinitionProvider {
		IList<IWorkflowDefinition> GetDefinitions();
		IWorkflowDefinition FindDefinition(string uniqueId);
	}
	public class WorkflowDefinitionProvider : ServiceBase, IWorkflowDefinitionProvider {
		private IObjectSpaceProvider objectSpaceProvider;
		public Type WorkflowDefinitionType { get; private set; }
		public virtual IList<IWorkflowDefinition> GetDefinitions() {
			List<IWorkflowDefinition> result = new List<IWorkflowDefinition>();
			IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace();
			foreach(IWorkflowDefinition obj in objectSpace.GetObjects(WorkflowDefinitionType)) {
				result.Add(obj);
			}
			return result;
		}
		public virtual IWorkflowDefinition FindDefinition(string uniqueId) {
			IWorkflowDefinition definition = null;
			foreach(IWorkflowDefinition item in GetDefinitions()) {
				if(item.GetUniqueId() == uniqueId) {
					definition = item;
				}
			}
			return definition;
		}
		public WorkflowDefinitionProvider(Type workflowDefinitionType) : this(workflowDefinitionType, null) { }
		public WorkflowDefinitionProvider(Type workflowDefinitionType, IObjectSpaceProvider objectSpaceProvider) {
			Guard.ArgumentNotNull(workflowDefinitionType, "workflowDefinitionType");
			this.objectSpaceProvider = objectSpaceProvider;
			this.WorkflowDefinitionType = workflowDefinitionType;
		}
		public IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProvider ?? GetService<IObjectSpaceProvider>(); }
			set { objectSpaceProvider = value; }
		}
	}
	public class WorkflowDefinitionProvider<T> : WorkflowDefinitionProvider where T : IWorkflowDefinition {
		public WorkflowDefinitionProvider() : base(typeof(T)) { }
		public WorkflowDefinitionProvider(IObjectSpaceProvider objectSpaceProvider) : base(typeof(T), objectSpaceProvider) { }
	}
}
