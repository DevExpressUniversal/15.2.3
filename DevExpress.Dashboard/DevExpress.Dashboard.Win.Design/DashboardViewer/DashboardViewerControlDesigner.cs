#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardWin.Native;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using EnvDTE;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
namespace DevExpress.DashboardWin.Design {
	public class DashboardViewerDesigner : BaseControlDesigner, IVSDashboardDesigner {
		DesignerVerbCollection defaultVerbs;
		DesignerActionListCollection actionLists = null;
		string sourceDirectory;
		public override DesignerVerbCollection DXVerbs { get { return defaultVerbs; } }
		public string SourceDirectory {
			get {
				if (sourceDirectory == null) {
					ProjectItem projectItem = GetService(typeof(ProjectItem)) as ProjectItem;
					if (projectItem == null)
						sourceDirectory = String.Empty;
					else {
						try {
							Project project = projectItem.ContainingProject;
							if (project == null)
								sourceDirectory = String.Empty;
							else {
								string projectPath = project.Properties.Item("FullPath").Value as string;
								sourceDirectory = String.IsNullOrEmpty(projectPath) ? String.Empty :
									Path.GetDirectoryName(projectPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
							}
						}
						catch {
							sourceDirectory = String.Empty;
						}
					}
				}
				return sourceDirectory;
			}
		}
		public DashboardViewerDesigner() {
			defaultVerbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About", new EventHandler(OnAboutClick)) });
		}
		void OnAboutClick(object sender, EventArgs e) {
			DashboardViewer.About();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if (designerHost.GetService(typeof(ILookAndFeelService)) == null)
				designerHost.AddService(typeof(ILookAndFeelService), new VSLookAndFeelService(designerHost));
			if (designerHost.GetService(typeof(IDashboardListService)) == null)
				designerHost.AddService(typeof(IDashboardListService), new DashboardListService());
		}
		protected override DesignerActionListCollection CreateActionLists() {
			if (actionLists == null) {
				actionLists = base.CreateActionLists();
				DashboardViewer viewer = Component as DashboardViewer;
				if (viewer != null)
					actionLists.Insert(0, new DashboardViewerActionList(viewer));
			}
			return actionLists;
		}
	}
}
