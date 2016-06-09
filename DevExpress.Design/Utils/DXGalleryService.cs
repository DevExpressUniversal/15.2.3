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
using System.Linq;
using System.Text;
namespace DevExpress.Utils.Design {
	public interface IDXGalleryService {
		void AddItem(IServiceProvider serviceProvider, string itemName, string templatName);
	}
	[CLSCompliant(false)]
	public class DXGalleryServiceImp : IDXGalleryService {
		internal DXGalleryServiceImp() {
		}
		#region IDXGalleryService
		void IDXGalleryService.AddItem(IServiceProvider serviceProvider, string itemName, string templatName) {
			AddItemCore(serviceProvider, itemName, templatName);
		}
		#endregion
		protected virtual void AddItemCore(IServiceProvider serviceProvider, string itemName, string templatName) {
			string path = GetGalleryTemplatePath(serviceProvider);
			if(string.IsNullOrEmpty(path))
				throw new CannotFindDXGalleryItemTemplatePathException();
			string cmd = string.Format("$additemtemplate={0}, name={1}$", templatName, itemName);
			var project = GetProject(serviceProvider);
			project.ProjectItems.AddFromTemplate(path, cmd);
		}
		protected string GetGalleryTemplatePath(IServiceProvider serviceProvider) {
			var solution = GetSolution(serviceProvider);
			if(solution == null)
				return null;
			string path = string.Empty;
			string name = string.Format("DXItemTemplateGallery.{0}.zip", AssemblyInfo.VSuffixWithoutSeparator);
			try {
				path = solution.GetProjectItemTemplate(name, "CSharp");
			}
			catch { }
			return path;
		}
		protected EnvDTE.Project GetProject(IServiceProvider serviceProvider) {
			EnvDTE.ProjectItem item = serviceProvider.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			return item != null ? item.ContainingProject : null;
		}
		protected EnvDTE80.Solution2 GetSolution(IServiceProvider serviceProvider) {
			EnvDTE.Project project = GetProject(serviceProvider);
			return project != null ? project.DTE.Solution as EnvDTE80.Solution2 : null;
		}
	}
	public class DXGalleryServiceProvider {
		static IDXGalleryService svc = null;
		public static IDXGalleryService Service {
			get {
				if(svc == null) {
					svc = new DXGalleryServiceImp();
				}
				return svc;
			}
		}
	}
	#region Exceptions
	public class CannotFindDXGalleryItemTemplatePathException : Exception {
		public CannotFindDXGalleryItemTemplatePathException() {
		}
	}
	#endregion
}
