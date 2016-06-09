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
using DevExpress.ExpressApp.Utils;
using DevExpress.Data.Filtering;
using DevExpress.Workflow.Activities;
namespace DevExpress.ExpressApp.Workflow.Versioning {
	public abstract class WorkflowVersioningEngine {
		private IList<IUserActivityVersionBase> FindActivityVersions(IWorkflowDefinition workflowDefinition, out int newVersion) {
			newVersion = 0;
			IList<IUserActivityVersionBase> result = FindActivityVersions(workflowDefinition);
			foreach(IUserActivityVersionBase activityVersion in result) {
				if(activityVersion.Version > newVersion) {
					newVersion = activityVersion.Version;
				}
			}
			newVersion++;
			return result;
		}
		private string CreateLastVersionXaml(IWorkflowDefinition workflowDefinition, IWorkflowDefinition[] workflowDefinitions) {
			string xaml = workflowDefinition.Xaml;
			foreach(IWorkflowDefinition definition in workflowDefinitions) {
				if(definition.GetUniqueId() != workflowDefinition.GetUniqueId()) {
					if(xaml.Contains(definition.GetActivityTypeName())) {
						xaml = xaml.Replace(definition.GetActivityTypeName(), GetActivityCurrentVersion(definition, workflowDefinitions).GetVersionId());
					}
				}
			}
			return xaml;
		}
		private IUserActivityVersionBase GetActivityCurrentVersion(IWorkflowDefinition definition, IWorkflowDefinition[] workflowDefinitions) {
			string lastVersionXaml = CreateLastVersionXaml(definition, workflowDefinitions);
			int newVersion;
			IList<IUserActivityVersionBase> activityVersions = FindActivityVersions(definition, out newVersion);
			foreach(IUserActivityVersionBase activityVersion in activityVersions) {
				if(activityVersion.Xaml == lastVersionXaml) {
					return activityVersion;
				}
			}
			IUserActivityVersionBase userActivityVersion = CreateActivityVersion(definition.GetUniqueId(), newVersion, lastVersionXaml);
			return userActivityVersion;
		}
		protected abstract IUserActivityVersionBase CreateActivityVersion(string workflowUniqueId, int version, string xaml);
		protected abstract IList<IUserActivityVersionBase> FindActivityVersions(IWorkflowDefinition workflowDefinition);
		public IList<IWorkflowDefinition> GetVersionedDefinitions(IWorkflowDefinition[] workflowDefinitions) {
			List<IWorkflowDefinition> result = new List<IWorkflowDefinition>();
			foreach(IWorkflowDefinition workflowDefinition in workflowDefinitions) {
				IUserActivityVersionBase currentVersion = GetActivityCurrentVersion(workflowDefinition, workflowDefinitions);
				IList<IUserActivityVersionBase> activityVersions = FindActivityVersions(workflowDefinition);
				foreach(IUserActivityVersionBase activityVersion in activityVersions) {
					string uniqueId = activityVersion.GetVersionId();
					string name = string.Format("{0} ({1})", workflowDefinition.Name, activityVersion.GetVersionId());
					bool isCurrentVersion = activityVersion.GetVersionId() == currentVersion.GetVersionId();
					if(isCurrentVersion) {
						uniqueId = workflowDefinition.GetUniqueId();
						name = workflowDefinition.Name;
					}
					WorkflowDefinition versionedWorkflowDefinition = new WorkflowDefinition(uniqueId, activityVersion.GetVersionId(), name, activityVersion.Xaml);
					versionedWorkflowDefinition.CanCompileForDesigner = workflowDefinition.CanCompileForDesigner;
					versionedWorkflowDefinition.CanCompile = workflowDefinition.CanCompile;
					versionedWorkflowDefinition.CanOpenHost = workflowDefinition.CanOpenHost;
					if(isCurrentVersion) {
						versionedWorkflowDefinition.SetConditions(workflowDefinition.GetConditions());
					}
					result.Add(versionedWorkflowDefinition);
				}
			}
			return result;
		}
	}
	public class PersistentWorkflowVersioningEngine<U> : WorkflowVersioningEngine where U : IUserActivityVersionBase {
		private IObjectSpaceProvider objectSpaceProvider;
		protected override IUserActivityVersionBase CreateActivityVersion(string workflowUniqueId, int version, string xaml) {
			IObjectSpace os = objectSpaceProvider.CreateObjectSpace();
			IUserActivityVersionBase result = os.CreateObject<U>();
			result.WorkflowUniqueId = workflowUniqueId;
			result.Xaml = xaml;
			result.Version = version;
			os.CommitChanges();
			return result;
		}		
		protected override IList<IUserActivityVersionBase> FindActivityVersions(IWorkflowDefinition workflowDefinition) {			
			List<IUserActivityVersionBase> result = new List<IUserActivityVersionBase>();
			IObjectSpace os = objectSpaceProvider.CreateObjectSpace();
			foreach(U userActivityVersion in os.GetObjects<U>(new BinaryOperator("WorkflowUniqueId", workflowDefinition.GetUniqueId()))) {
				result.Add(userActivityVersion);
			}
			return result;
		}
		public PersistentWorkflowVersioningEngine(IObjectSpaceProvider objectSpaceProvider) {
			Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
			this.objectSpaceProvider = objectSpaceProvider;
		}
	}
}
