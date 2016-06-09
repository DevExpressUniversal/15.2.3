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
using System.Linq.Expressions;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Utils.Design;
namespace DevExpress.Xpf.Core.Design.Wizards.Utils {
	public class EmptyItemCreator : IVSObjectsCreator {
		public void CreateDataAccessTechnologyObject(IDataAccessTechnologyInfo info, EnvDTE.DTE dte) { }
	}
	public class ExecuteCommandCreator : IVSObjectsCreator {
		private readonly string commandStr;
		public ExecuteCommandCreator(string command) {
			this.commandStr = command;
		}
		public void CreateDataAccessTechnologyObject(IDataAccessTechnologyInfo info, EnvDTE.DTE dte) {
			try {
				dte.ExecuteCommand(this.commandStr, string.Empty);
			} catch { }
		}
	}
	public class AddNewItemCreator : IVSObjectsCreator {
		private readonly string itemTemplate;
		private readonly string nameFormat;
		string Name { get { return GetNewItemName(this.nameFormat); } }
		public AddNewItemCreator(string itemTemplate, string nameFormat) {
			this.nameFormat = nameFormat;
			this.itemTemplate = itemTemplate;
		}
		public void CreateDataAccessTechnologyObject(IDataAccessTechnologyInfo info, EnvDTE.DTE dte) {
			if(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(this.itemTemplate)) return;
			try {
				EnvDTE.ItemOperations itemOp = dte.ItemOperations;
				itemOp.AddNewItem(this.itemTemplate, Name);
			} catch { }
		}
		private static string GetNewItemName(string fileTemplate) {
			if(string.IsNullOrEmpty(fileTemplate))
				return string.Empty;
			string fileName = fileTemplate.Substring(0, fileTemplate.IndexOf('{'));
			int number = GetCorrectNumberOfItem(fileName);
			if(number == -1)
				return string.Empty;
			return String.Format(fileTemplate, number);
		}
		private static int GetCorrectNumberOfItem(string name) {
			List<string> filesWithCorrectExtension = GetAllProjectFileNames();
			if(filesWithCorrectExtension == null)
				return -1;
			int number = 1;
			while(true) {
				string newItemName = name + number.ToString();
				if(!filesWithCorrectExtension.Contains(newItemName))
					break;
				number++;
			}
			return number;
		}
		private static List<string> GetAllProjectFileNames() {
			EnvDTE.DTE dte = DTEHelper.GetCurrentDTE();
			if(dte == null)
				return null;
			EnvDTE.Project project = dte.ActiveDocument.ProjectItem.ContainingProject;
			List<string> filesWithCorrectExtension = new List<string>();
			foreach(EnvDTE.ProjectItem item in project.ProjectItems) {
				if(item.Name.Contains('.'))
					filesWithCorrectExtension.Add(item.Name.Substring(0, item.Name.LastIndexOf('.')));
			}
			string projectPath = System.IO.Path.GetDirectoryName(project.FullName);
			foreach(string file in System.IO.Directory.GetFiles(projectPath)) {
				string fileName = System.IO.Path.GetFileName(file);
				if(file.Contains('.'))
					filesWithCorrectExtension.Add(fileName.Substring(0, fileName.LastIndexOf('.')));
			}
			return filesWithCorrectExtension;
		}
	}
}
