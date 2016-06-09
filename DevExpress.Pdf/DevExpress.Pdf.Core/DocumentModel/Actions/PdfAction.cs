#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfAction : PdfObject {
		internal const string DictionaryType = "Action";
		internal const string ActionTypeDictionaryKey = "S";
		const string nextActionDictionaryKey = "Next";
		internal static PdfAction Parse(PdfReaderDictionary actionDictionary) {
			if (actionDictionary != null) {
				string type = actionDictionary.GetName(PdfDictionary.DictionaryTypeKey);
				string actionType = actionDictionary.GetName(ActionTypeDictionaryKey);
				if ((type != null && type != DictionaryType && type != "A") || actionType == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				switch (actionType) {
					case PdfGoToAction.Name:
						return new PdfGoToAction(actionDictionary);
					case PdfRemoteGoToAction.Name:
						return new PdfRemoteGoToAction(actionDictionary);
					case PdfEmbeddedGoToAction.Name:
						return new PdfEmbeddedGoToAction(actionDictionary);
					case PdfLaunchAction.Name:
						return new PdfLaunchAction(actionDictionary);
					case PdfThreadAction.Name:
						return new PdfThreadAction(actionDictionary);
					case PdfUriAction.Name:
						return new PdfUriAction(actionDictionary);
					case PdfSoundAction.Name:
						return new PdfSoundAction(actionDictionary);
					case PdfMovieAction.Name:
						return new PdfMovieAction(actionDictionary);
					case PdfHideAction.Name:
						return new PdfHideAction(actionDictionary);
					case PdfNamedAction.Name:
						return new PdfNamedAction(actionDictionary);
					case PdfSubmitFormAction.Name:
						return new PdfSubmitFormAction(actionDictionary);
					case PdfResetFormAction.Name:
						return new PdfResetFormAction(actionDictionary);
					case PdfImportDataAction.Name:
						return new PdfImportDataAction(actionDictionary);
					case PdfJavaScriptAction.Name:
						return new PdfJavaScriptAction(actionDictionary);
					case PdfSetOcgStateAction.Name:
						return new PdfSetOcgStateAction(actionDictionary);
					case PdfRenditionAction.Name:
						return new PdfRenditionAction(actionDictionary);
					case PdfTransitionAction.Name:
						return new PdfTransitionAction(actionDictionary);
					case PdfGoTo3dViewAction.Name:
						return new PdfGoTo3dViewAction(actionDictionary);
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						break;
				}
			}
			return null;
		}
		readonly PdfDocumentCatalog documentCatalog;
		List<PdfAction> next = null;
		object nextValue;
		protected PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		protected abstract string ActionType { get; }
		public IEnumerable<PdfAction> Next {
			get {
				FillNextActions();
				return next;
			}
		}
		protected PdfAction(PdfDocumentCatalog documentCatalog) {
			this.documentCatalog = documentCatalog;
		}
		protected PdfAction(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			documentCatalog = dictionary.Objects.DocumentCatalog;
			if (!dictionary.TryGetValue(nextActionDictionaryKey, out nextValue))
				nextValue = null;
		}
		void FillNextActions() {
			if (nextValue != null) {
				PdfObjectCollection objects = documentCatalog.Objects;
				object value = nextValue;
				nextValue = null;
				IList<object> actionArray = objects.TryResolve(value) as IList<object>;
				next = new List<PdfAction>(actionArray == null ? 1 : actionArray.Count);
				if (actionArray != null)
					foreach (object action in actionArray)
						next.Add(objects.GetAction(action));
				else
					next.Add(objects.GetAction(value));
			}
		}
		protected internal virtual void Execute(IPdfInteractiveOperationController interactiveOperationController, IList<PdfPage> pages) {
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			FillNextActions();
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(DictionaryType));
			dictionary.Add(ActionTypeDictionaryKey, new PdfName(ActionType));
			if (next != null && next.Count == 1)
				dictionary.Add(nextActionDictionaryKey, next[0]);
			else
				dictionary.AddList(nextActionDictionaryKey, next);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
	}
}
