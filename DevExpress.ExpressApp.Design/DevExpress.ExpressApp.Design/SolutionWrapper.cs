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
using System.Text;
using EnvDTE;
using EnvDTE80;
namespace DevExpress.ExpressApp.Design.Core {
	public interface ISolutionWrapper {
		bool Build();
		IList<IProjectWrapper> XafProjects { get; }
	}
	public class SolutionWrapper : ISolutionWrapper {
		private Solution solution;
		private List<IProjectWrapper> xafProjects = new List<IProjectWrapper>();		
		private void CollectXafProjects() {
			xafProjects.Clear();
			foreach(BuildDependency buildDependency in solution.SolutionBuild.BuildDependencies) {
				try {
					ProjectWrapper projectWrapper = ProjectWrapper.Create(buildDependency.Project);
					if(projectWrapper.IsExpressAppProject) {
						xafProjects.Add(projectWrapper);
					}
				}
				catch(NotImplementedException) {
				}
			}
		}
		public SolutionWrapper(Solution solution) {
			this.solution = solution;
			CollectXafProjects();
		}
		public bool Build() {
			((DTE2)solution.SolutionBuild.DTE).ToolWindows.OutputWindow.Parent.Activate();
			EnvDTE.Properties projectsAndSolutionProperties = solution.DTE.get_Properties("Environment", "ProjectsAndSolution");
			Property buildOnlyStartup = projectsAndSolutionProperties.Item("OnlySaveStartupProjectsAndDependencies");
			bool oldBuildOnlyStartupValue = (bool)buildOnlyStartup.Value;
			buildOnlyStartup.Value = false;
			try {
				solution.SolutionBuild.Build(true);
			}
			finally {
				buildOnlyStartup.Value = oldBuildOnlyStartupValue;
			}
			bool result = solution.SolutionBuild.LastBuildInfo == 0;
			if(result) {
				CollectXafProjects();
			}
			return result;
		}
		public IList<IProjectWrapper> XafProjects {
			get { return xafProjects; }
		}
	}	
}
