#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.Utils.Design {
	public class ProjectItemCreator {
		readonly string templateCategory;
		readonly string templateName;
		public ProjectItemCreator(string templateCategory, string templateName) {
			Guard.ArgumentIsNotNullOrEmpty(templateCategory, "templateCategory");
			Guard.ArgumentIsNotNullOrEmpty(templateName, "templateName");
			this.templateName = templateName;
			this.templateCategory = templateCategory;
		}
		[CLSCompliant(false)]
		public virtual void Create(EnvDTE.DTE dte) {
			if(dte == null) return;
			try {
				EnvDTE.Project prj = DTEHelper.GetActiveProject(dte);
				if(prj == null)
					return;
				IVsAddProjectItemDlg addItemDialog = DTEHelper.Query<IVsAddProjectItemDlg>(dte);
				if(addItemDialog == null)
					return;
				IVsMonitorSelection monitorSelection = DTEHelper.Query<IVsMonitorSelection>(dte);
				if(monitorSelection == null)
					return;
				IntPtr tempVsHierarchy, ppSC;
				uint projectItemId = 0;
				IVsMultiItemSelect mis;
				monitorSelection.GetCurrentSelection(out tempVsHierarchy, out projectItemId, out mis, out ppSC);
				string selectedElementCanonicalName = null;
				IVsHierarchy vsHierarchy = DTEHelper.GetVsHierarchy(prj);
				if(vsHierarchy == null)
					return;
				IVsProject ivsPrj = vsHierarchy as IVsProject;
				vsHierarchy.GetCanonicalName(projectItemId, out selectedElementCanonicalName);
				if(string.IsNullOrEmpty(selectedElementCanonicalName))
					return;
				uint uiFlags = (uint)(__VSADDITEMFLAGS.VSADDITEM_AddNewItems | __VSADDITEMFLAGS.VSADDITEM_SuggestTemplateName | __VSADDITEMFLAGS.VSADDITEM_AllowHiddenTreeView);
				Guid prjGuid = new Guid(prj.Kind);
				string strFilter = String.Empty;
				int iDontShowAgain;
				addItemDialog.AddProjectItemDlg(projectItemId, ref prjGuid, ivsPrj, uiFlags, templateCategory, templateName, ref selectedElementCanonicalName, ref strFilter, out iDontShowAgain);
			}
			catch { }
		}
		public void Create() {
			Create(DTEHelper.GetCurrentDTE());
		}
	}
	class ExecuteCommandCreator {
		readonly string commandStr;
		public ExecuteCommandCreator(string command) {
			this.commandStr = command;
		}
		public void Create(EnvDTE.DTE dte) {
			if(dte == null) return;
			try {
				dte.ExecuteCommand(this.commandStr, string.Empty);
			}
			catch { }
		}
		public void Create() {
			using(new MessageFilter())
				Create(DTEHelper.GetCurrentDTE());
		}
	}
}
