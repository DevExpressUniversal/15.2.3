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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.Design.SmartTags {
	public interface IEnvDteService {
		string GetActiveProjectFullPath();
		string GetActiveProjectAssemblyName();
		string GetActiveProjectItemRelativePath();
		void EnsureAssemblyReferencedByEnvDte(string simpleNameOrPath, bool forceCopyLocal);
	}
	public class XpfEnvDteService : IEnvDteService {
		public string GetActiveProjectFullPath() {
			return ProtectedDTEInvocation(dte => dte.ActiveDocument.ProjectItem.ContainingProject.Properties.Item("FullPath").Value.ToString());
		}
		public string GetActiveProjectAssemblyName() {
			return ProtectedDTEInvocation(dte => dte.ActiveDocument.ProjectItem.ContainingProject.Properties.Item("AssemblyName").Value.ToString());
		}
		public string GetActiveProjectItemRelativePath() {
			return ProtectedDTEInvocation(dte => {
				List<string> pathParts = new List<string>();
				for(EnvDTE.ProjectItem item = dte.ActiveDocument.ProjectItem; item != null; item = item.Collection.Parent as EnvDTE.ProjectItem) {
					if(item.FileCount == 0) break;
					pathParts.Add(Path.GetFileName(item.get_FileNames(0).TrimEnd('/', '\\')));
				}
				return Path.Combine(pathParts.AsEnumerable().Reverse().ToArray());
			});
		}
		public void EnsureAssemblyReferencedByEnvDte(string simpleNameOrPath, bool forceCopyLocal) {
			ProtectedDTEInvocation<object>(dte => {
				VSLangProj.VSProject project = DTEHelper.GetCurrentProject().Object as VSLangProj.VSProject;
				if(project == null) throw new InvalidOperationException();
				VSLangProj.Reference reference = project.References.Cast<VSLangProj.Reference>().Where(r => r.Name.StartsWith(AssemblyHelper.GetPartialName(simpleNameOrPath))).FirstOrDefault();
				if(reference == null)
					reference = project.References.Add(simpleNameOrPath);
				if(forceCopyLocal) {
					reference.CopyLocal = false;
					reference.CopyLocal = true;
				}
				return null;
			});
		}
		static T ProtectedDTEInvocation<T>(Func<EnvDTE.DTE, T> func) {
			using(new MessageFilter()) {
				EnvDTE.DTE dte = DTEHelper.GetCurrentDTE();
				return func(dte);
			}
		}
	}
}
