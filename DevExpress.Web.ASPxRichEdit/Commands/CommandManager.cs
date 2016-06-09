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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Web.Office.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Model;
using System.IO;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class CommandManager {
		public RichEditWorkSession WorkSession { get; set; }
		public Guid ClientGuid { get; private set; }
		public WorkSessionClient Client { get { return WorkSession.GetClient(ClientGuid); } }
		public ASPxRichEdit Control { get; private set; }
		public CommandManager(RichEditWorkSession workSession, Guid clientGuid, ASPxRichEdit control) {
			WorkSession = workSession;
			ClientGuid = clientGuid;
			Control = control;
		}
		public DocumentHandlerResponse ExecuteCommands(NameValueCollection parameters) {
			var commandsJSON = parameters["commands"];
			var list = new List<string>();
			try {
				var commandsList = HtmlConvertor.FromJSON<ArrayList>(commandsJSON);
				commandsList.Sort(new RequestSortingComparer());
				List<WebRichEditCommand> results = new List<WebRichEditCommand>();
				foreach(Hashtable item in commandsList) {
					var command = CommandFactory.Create(this, item);
					if(command != null)
						results.Add(command);
					else
						throw new Exception("Command Not Found");
				}
				return ExecuteCommands(results, true);
			}
			catch(Exception) {
				return GetErrorResponse(ResponseError.InnerException, null, -1);
			}
		}
		public DocumentHandlerResponse ExecuteCommands(IEnumerable<WebRichEditCommand> commands, bool throwExceptions) {
			var results = new ArrayList();
			var lastProcessedEditCommandId = WorkSession.RichEdit.Model.DocumentModel.LastExecutedEditCommandId;
			var currentIncrementalId = -1;
			try {
				foreach(var command in commands) {
					currentIncrementalId = command.IncrementalId;
					if(command.IsModification && Client.ReadOnly)
						return GetErrorResponse(ResponseError.AuthException, results, currentIncrementalId);
					if(WorkSession.EditorClientGuid != ClientGuid && command.EditIncrementalId > 0) {
						if(command.EditIncrementalId - lastProcessedEditCommandId != 1)
							return GetErrorResponse(ResponseError.ModelIsChanged, results, currentIncrementalId);
						lastProcessedEditCommandId++;
						WorkSession.EditorClientGuid = ClientGuid;
					}
					if (command.IsModification) {
						WorkSession.RefreshLastModifyTime();
						WorkSession.Modified = true;
					}
					results.Add(command.Execute());
				}
			}
			catch(CantSaveToAlreadyOpenedFileException e) {
				if(throwExceptions)
					return GetErrorResponse(ResponseError.CantSaveToAlreadyOpenedFile, results, currentIncrementalId);
				throw e;
			}
			catch(DocumentCannotBeSavedException e) {
				if(throwExceptions)
					return GetErrorResponse(ResponseError.CantSaveDocument, results, currentIncrementalId);
				throw e;
			}
			catch(DocumentCannotBeOpenedException e) {
				if(throwExceptions)
					return GetErrorResponse(ResponseError.CantOpenDocument, results, currentIncrementalId);
				throw e;
			}
			catch(CalculateDocumentVariableException e) {
				if(throwExceptions)
					return GetErrorResponse(ResponseError.CalculateDocumentVariableException, results, currentIncrementalId);
				throw e;
			}
			catch(PathTooLongException e) {
				if(throwExceptions)
					return GetErrorResponse(ResponseError.PathTooLongException, results, currentIncrementalId);
				throw e;
			}
			catch(Exception exc) {
				if(Control == null)
					ASPxRichEdit.RaiseCallbackErrorInternal(WorkSession.DocumentInfo, exc);
				if(throwExceptions)
					return GetErrorResponse(ResponseError.InnerException, results, currentIncrementalId);
				throw exc;
			}
			return GetSuccessResponse(results);
		}
		JSONDocumentHandlerResponse GetErrorResponse(ResponseError error, ArrayList results, int failedIncrementalId) {
			JSONDocumentHandlerResponse errorResponse = new JSONDocumentHandlerResponse();
			var hashtable = new Hashtable();
			hashtable["errorCode"] = (int)error;
			if(results != null)
				hashtable["results"] = results;
			hashtable["failedIncrementalId"] = failedIncrementalId;
			errorResponse.ResponseResult = HtmlConvertor.ToJSON(hashtable);
			errorResponse.ContentType = DocumentRequestManager.DefaultResponseContentType;
			errorResponse.ContentEncoding = Encoding.UTF8;
			return errorResponse;
		}
		JSONDocumentHandlerResponse GetSuccessResponse(ArrayList results) {
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(new Hashtable() { { "commands", results } });
			documentResponse.ContentType = DocumentRequestManager.DefaultResponseContentType;
			documentResponse.ContentEncoding = Encoding.UTF8;
			return documentResponse;
		}
		public void SwitchDocument(string documentPath) {
			DocumentContentContainer documentContentContainer = new DocumentContentContainer(documentPath);
			SwitchDocument(WorkSessions.OpenWorkSession(WorkSession.ID, documentContentContainer, (Guid newGuid) => new RichEditWorkSession(documentContentContainer, newGuid, Client.DocumentCapabilitiesOptions)));
		}
		public void SwitchDocument(Guid newSessinId) {
			WorkSession = (RichEditWorkSession)WorkSessions.Get(newSessinId, true);
			if(Control != null)
				Control.WorkSessionGuid = newSessinId;
		}
	}
	public enum ClientTextRunType {
		Undefined = -1,
		TextRun = 1,
		ParagraphRun = 2,
		LineNumberCommonRun = 3,
		CustomRun = 4,
		DataContainerRun = 5,
		FieldCodeStartRun = 6,
		FieldCodeEndRun = 7,
		FieldResultEndRun = 8,
		LayoutDependentTextRun = 9,
		FootNoteRun = 10,
		EndNoteRun = 11,
		InlineCustomObjectRun = 12,
		InlinePictureRun = 13,
		SectionRun = 14,
		SeparatorTextRun = 15,
		FloatingObjectAnchorRun = 16
	}
	public enum ResponseError { 
		ModelIsChanged = 1,
		InnerException = 2,
		AuthException = 3,
		CantSaveToAlreadyOpenedFile = 4,
		CantSaveDocument = 5,
		CantOpenDocument = 6,
		CalculateDocumentVariableException = 7,
		PathTooLongException = 8
	}
	public enum ServerUpdateFieldType {
		DocVariable = 1,
		MergeField = 2
	}
}
