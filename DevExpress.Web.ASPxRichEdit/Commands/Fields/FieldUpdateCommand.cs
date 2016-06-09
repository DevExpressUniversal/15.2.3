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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class FieldUpdateCommand : WebRichEditLoadModelCommandBase {
		public FieldUpdateCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.FieldUpdate; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			if(Manager.Control == null)
				throw new Exception("FieldUpdateCommand could be called only on callback");
			result.Add("subDocID", (int)Parameters["subDocID"]);
			Hashtable info = (Hashtable)Parameters["info"];
			Manager.Control.MailMergeActiveRecordIndex = (int)Parameters["activeRecord"];
			Hashtable resultInfo = new Hashtable();
			foreach(DictionaryEntry pair in info) {
				Hashtable requestDataAndType = (Hashtable)pair.Value;
				switch((ServerUpdateFieldType)requestDataAndType["type"]) {
					case ServerUpdateFieldType.DocVariable:
						resultInfo.Add(pair.Key, this.UpdateDocVariableField((Hashtable)requestDataAndType["data"]));
						break;
					case ServerUpdateFieldType.MergeField:
						resultInfo.Add(pair.Key, this.UpdateMergeField((Hashtable)requestDataAndType["data"]));
						break;
				}
			}
			result.Add("info", resultInfo);
		}
		Hashtable UpdateDocVariableField(Hashtable data) {
			Hashtable result = new Hashtable();
			ArrayList parameters = (ArrayList)data["params"];
			String fieldName = (String)data["name"];
			ArgumentCollection argCollection = new ArgumentCollection();
			for (int i = 0; i < parameters.Count; i++) {
				Hashtable paramHashtable = (Hashtable)parameters[i];
				String textRepresentation = (String)paramHashtable["pureText"];
				int startPosition = (int)paramHashtable["intervalStart"];
				int endPosition = (int)paramHashtable["intervalEnd"];
				DocumentLogPosition logPosition = new DocumentLogPosition(startPosition);
				Token token = new Token(TokenKind.Simple, logPosition, textRepresentation, endPosition - startPosition);
				Argument arg = new Argument(token);
				argCollection.Add(arg);
			}
			CalculateDocumentVariableEventArgs eventArg = new CalculateDocumentVariableEventArgs(fieldName, argCollection, null, null);
			try {
				Manager.Control.RaiseCalculateDocumentVariable(eventArg);
			}
			catch(Exception exc) {
				throw new CalculateDocumentVariableException(exc.Message, exc);
			}
			if (eventArg.Handled) {
				Document userDocument = eventArg.Value as Document;
				if (userDocument != null) {
					CollectDocumentText(result, eventArg.KeepLastParagraph, userDocument);
				}
				else {
					String userString = eventArg.Value as String;
					if (userString == null)
						userString = eventArg.Value.ToString();
					result.Add("text", userString);
				}
			}
			else {
				object variable = this.DocumentModel.Variables.GetVariableValue(eventArg.VariableName, argCollection);
				if (variable != null)
					result.Add("text", variable.ToString());
				else
					result.Add("text", "");
			}
			return result;
		}
		Hashtable UpdateMergeField(Hashtable data) {
			Hashtable result = new Hashtable();
			String fieldName = (String)data["name"];
			object value = null;
			value = Manager.Control.GetMergeFieldValue(fieldName);
			if (value != null)
				result.Add("text", value.ToString());
			else
				result.Add("text", "");
			return result;
		}
		void CollectDocumentText(Hashtable result, bool keepLastParagraph, Document document) {
			DocumentModel doc = ((DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocument)document).DocumentModel;
			int docLength = doc.ActivePieceTable.DocumentEndLogPosition - DocumentLogPosition.Zero + 1;
			result["docLength"] = docLength - (keepLastParagraph ? 0 : 1);
			result["document"] = new LoadDocumentCommand(this, doc, true).Execute();
			result["imageCorrespondence"] = GetCorrespondenceTableForPictures(doc, DocumentModel);
		}
		Hashtable GetCorrespondenceTableForPictures(DocumentModel source, DocumentModel target) {
			var result = new Hashtable();
			foreach(var pieceTable in source.GetPieceTables(false)) {
				pieceTable.Runs.ForEach(run => {
					var inlinePictureRun = run as InlinePictureRun;
					if(inlinePictureRun != null) {
						var image = inlinePictureRun.Image.Clone(DocumentModel);
						DocumentModel.ImageCache.RegisterImage(image.EncapsulatedOfficeNativeImage);
						result.Add(inlinePictureRun.Image.ImageCacheKey, image.ImageCacheKey);
					}
				});
			}
			return result;
		}
	}
	public class CalculateDocumentVariableException : Exception {
		public CalculateDocumentVariableException(string message, Exception exc)
			:base(message, exc) { }
	}
}
