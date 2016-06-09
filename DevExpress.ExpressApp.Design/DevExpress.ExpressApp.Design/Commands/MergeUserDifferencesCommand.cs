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
using System.ComponentModel.Design;
using DevExpress.ExpressApp.Design.Core;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
namespace DevExpress.ExpressApp.Design.Commands {
	public class MergeUserDifferencesCommand : VSCommand {
		public MergeUserDifferencesCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		public override string CommandName {
			get { return "MergeUserDifferences"; }
		}
		public override string CommandToolName {
			get { return "Merge User Model"; }
		}
		public override CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.cmdidMergeUserDifferencesCommand); }
		}
		private string GetUserDiffsFileName(ProjectWrapper projectWrapper) {
			string binariesDir = projectWrapper.BinariesDir;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = binariesDir;
			openFileDialog.FileName = ModelStoreBase.UserDiffDefaultName + ".xafml";
			openFileDialog.Filter = "Differences Files (*.xafml)| *.xafml";
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				return openFileDialog.FileName;
			}
			return null;
		}
		public override void InternalExec() {
			Assembly modelEditorAssembly = LoadModelEditorAssembly();
			IMergeDifferences mergeDifferencesHelper = (IMergeDifferences)modelEditorAssembly.CreateInstance("DevExpress.ExpressApp.Design.ModelEditor.MergeDifferencesHelper", true, BindingFlags.CreateInstance, null, new object[] { }, null, null);
			if(mergeDifferencesHelper != null) {
				ProjectWrapper projectWrapper = ProjectWrapper.Create(FindSelectedProject());
				string diffsFileName = GetUserDiffsFileName(projectWrapper);
				if(!string.IsNullOrEmpty(diffsFileName)) {
					mergeDifferencesHelper.MergeDifferences(dte, projectWrapper, diffsFileName);
				}
				mergeDifferencesHelper.Dispose();
			}
		}
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			ProjectWrapper projectWrapper = ProjectWrapper.Create(FindSelectedProject());
			if(projectWrapper is WinApplicationWrapper || projectWrapper is WebApplicationProjectWrapper || projectWrapper is WebApplicationWrapper) {
				return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			else {
				return vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
			}
		}
	}
}
