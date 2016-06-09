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

using System.Collections;
using DevExpress.XtraRichEdit;
using DevExpress.Web.Office;
using DevExpress.Web.Office.Internal;
using System;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Model;
using System.IO;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class SaveAsDocumentCommand : InitSessionCommand {
		public SaveAsDocumentCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.SaveAsDocument; } }
		protected override bool IsEnabled() { return Client.RichEditBehaviorOptions.SaveAsAllowed; }
		public override void SwitchSession() {
			string workDirectory = GetWorkDirectory();
			string fileName = Parameters["fileName"].ToString();
			string folderPath = Parameters["folderPath"].ToString();
			string fileExtension = Parameters["fileExtension"].ToString();
			if (IsNotAllowedFileExtension(fileExtension))
				throw new DocumentCannotBeSavedException();
			string filePath = DevExpress.Web.Internal.UrlUtils.ResolvePhysicalPath(workDirectory + "\\" + folderPath + "\\" + fileName + fileExtension);
			MultiUserConflict multiUserConflict = WorkSessions.DetectMultiUserSavingConflict(Manager.WorkSession.ID, filePath);
			DocumentSavingEventArgs args;
			try {
				args = Manager.Control.RaiseSaving(filePath, multiUserConflict);
			}
			catch (Exception exc) {
				throw new DocumentCannotBeSavedException(exc);
			}
			if (!args.Handled) {
				bool cantSaveAs = args.MultiUserConflict == Office.MultiUserConflict.OtherUserDocumentOverride && args.MultiUserConflictResolve == Office.MultiUserConflictResolve.Persist;
				if (cantSaveAs) {
					throw new CantSaveToAlreadyOpenedFileException();
				}
				else {
					var documentContentContainer = new DocumentContentContainer(filePath);
					Guid newGuid;
					try {
						newGuid = WorkSessions.SaveAsWorkSession(Manager.WorkSession.ID, documentContentContainer);
					}
					catch(PathTooLongException exc) {
						throw exc;
					}
					catch (Exception exc) {
						throw new DocumentCannotBeSavedException(exc);
					}
					Manager.SwitchDocument(newGuid);
				}
			}
			else {
				Manager.SwitchDocument(Manager.Control.CurrentSession.ID);
			}
		}
		protected override bool AllowedExecute { get { return !string.IsNullOrEmpty(GetWorkDirectory()); } }
		public override bool IsNewDocument { get { return false; } }
		protected bool IsNotAllowedFileExtension(string fileExtension) {
			return Array.IndexOf<string>(Manager.Control.SettingsDocumentSelector.CommonSettings.AllowedFileExtensions, fileExtension) == -1;
		}
	}
}
