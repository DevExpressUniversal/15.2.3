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
using DevExpress.Workflow;
using DevExpress.Workflow.Activities;
using System.Activities;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Activities.XamlIntegration;
using System.IO;
namespace DevExpress.ExpressApp.Workflow.Server {
	public class HostManagerActivityProvider : WorkflowServerService {
		private void Manager_HostOpening(object sender, EventArgs e) {
			List<UserActivityDescription> activityDefinitions = new List<UserActivityDescription>();
			string currentAssemblyName = UserActivityBuilderHelper.GetCurrentAssemblyName();
			IList<IWorkflowDefinition> definitions = GetService<IWorkflowDefinitionProvider>().GetDefinitions();
			foreach(IWorkflowDefinition definition in definitions) {
				if(definition.CanCompile) {
					activityDefinitions.Add(new UserActivityDescription(definition.GetActivityTypeName(), definition.Xaml.Replace("assembly=UserActivities", "assembly=" + currentAssemblyName), definition.Name));
				}
			}
			UserActivityBuilderHelper.CreateUserActivityTypes(activityDefinitions, UserActivityBuilderHelper.GetCurrentAssemblyName());
			Dictionary<string, Activity> activities = new Dictionary<string, Activity>();
			foreach(IWorkflowDefinition definition in definitions) {
				if(definition.CanOpenHost) {
					Activity activity = ActivityXamlServices.Load(new StringReader(definition.Xaml.Replace("assembly=UserActivities", "assembly=" + currentAssemblyName)));
					activity.DisplayName = definition.GetActivityTypeName();
					activities.Add(definition.GetUniqueId(), activity);
				}
			}
			HostManager.RefreshHosts(activities);
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			HostManager.HostsOpening += new EventHandler(Manager_HostOpening);
		}
	}
}
