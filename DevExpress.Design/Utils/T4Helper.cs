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
namespace DevExpress.Utils.Design {
	public static class T4Helper {
		public static string GetOutputDir(object host) {
			IServiceProvider hostServiceProvider = (IServiceProvider)host;
			if(hostServiceProvider == null)
				throw new Exception("Host property returned unexpected value (null)");
			var dte = (EnvDTE.DTE)hostServiceProvider.GetService(typeof(EnvDTE.DTE));
			if(dte == null)
				throw new Exception("Unable to retrieve EnvDTE.DTE");
			var activeSolutionProjects = (Array)dte.ActiveSolutionProjects;
			if(activeSolutionProjects == null)
				throw new Exception("DTE.ActiveSolutionProjects returned null");
			var dteProject = (EnvDTE.Project)activeSolutionProjects.GetValue(0);
			if(dteProject == null)
				throw new Exception("DTE.ActiveSolutionProjects[0] returned null");
			string outputPath = dteProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
			string fullPath = dteProject.Properties.Item("FullPath").Value.ToString();
			return Path.Combine(fullPath, outputPath);
		}
	}
}
