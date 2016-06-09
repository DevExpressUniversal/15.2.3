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

using DevExpress.XtraRichEdit.Model;
using System.Collections;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class InitSessionCommand : WebRichEditLoadModelCommandBase {
		public InitSessionCommand(CommandManager commandManager, Hashtable parameters) 
			:base(commandManager, parameters) { }
		protected override void FillHashtable(Hashtable result) {
			if(AllowedExecute) {
				RichEditWorkSession lastSession = Manager.WorkSession;
				SwitchSession();
				if(Manager.WorkSession != lastSession) {
					WorkSessionClient previousClientSettings = lastSession.GetClient(Manager.ClientGuid);
					Manager.WorkSession.AttachClient(Manager.ClientGuid, previousClientSettings);
					lastSession.DetachClient(Manager.ClientGuid);
					Client.Reset();
				}
				result["isNewWorkSession"] = true;
				result["isNewDocument"] = IsNewDocument;
				result["sessionGuid"] = WorkSession.ID.ToString();
				result["fileName"] = !string.IsNullOrEmpty(WorkSession.CurrentFileName) ? System.IO.Path.GetFileName(WorkSession.CurrentFileName) : string.Empty;
				result["lastExecutedEditCommandId"] = DocumentModel.LastExecutedEditCommandId;
				result["emptyImageCacheID"] = WorkSession.EmptyImageCacheID;
				result["subDocumentsCounter"] = DocumentModel.CountIdForPeaceTables;
				result["isModified"] = WorkSession.Modified;
				if(IsNewDocument) {
					result["document"] = new LoadDocumentCommand(this, DocumentModel).Execute();
					ClientFontInfoCacheLength = 0;
				}
			}
		}
		protected string GetWorkDirectory() {
			return Manager.WorkSession.GetClient(Manager.ClientGuid).WorkDirectory;
		}
		protected virtual bool AllowedExecute { get { return true; } }
		public abstract void SwitchSession();
		public abstract bool IsNewDocument { get; }
	}
	public class StartCommand : InitSessionCommand {
		public StartCommand(CommandManager commandManager, DocumentModel documentModel, ASPxRichEditSettings settings) : base(commandManager, new Hashtable()) {
			this.settings = settings;
		}
		public override CommandType Type { get { return CommandType.Start; } }
		protected override bool IsEnabled() { return true; }
		private ASPxRichEditSettings settings;
		public override bool IsNewDocument { get { return true; } }
		public override void SwitchSession() { }
		protected override void FillHashtable(Hashtable result) {
			base.FillHashtable(result);
			XtraRichEdit.Model.DocumentModel doc = Manager.WorkSession.RichEdit.Model.DocumentModel;
			result["options"] = new LoadControlOptionsCommand(this, new Hashtable(), this.settings).Execute();
			result["stringResources"] = new LoadStringResourcesCommand(this, new Hashtable()).Execute();
		}
	}
	public class ReloadDocumentCommand : InitSessionCommand {
		public ReloadDocumentCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ReloadDocument; } }
		protected override bool IsEnabled() { return true; }
		public override bool IsNewDocument { get { return true; } }
		public override void SwitchSession() { }
	}
}
