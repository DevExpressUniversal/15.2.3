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
namespace DevExpress.Pdf.Native {
	public class PdfAdditionalActions : PdfObject {
		internal const string DictionaryAdditionalActionsKey = "AA";
		static readonly string[] InteractiveFormFieldActionsDictionaryKeys = new string[]{
			PdfInteractiveFormFieldActions.CharacterChangedDictionaryKey,
			PdfInteractiveFormFieldActions.FieldFormattingDictionaryKey,
			PdfInteractiveFormFieldActions.FieldValueChangedDictionaryKey,
			PdfInteractiveFormFieldActions.FieldValueRecalculatingDictionaryKey
		};
		static readonly string[] AnnotationActionsDictionaryKeys = new string[]{
			PdfAnnotationActions.CursorEnteredDictionaryKey,
			PdfAnnotationActions.CursorExitedDictionaryKey,
			PdfAnnotationActions.MouseButtonPressedDictionaryKey,
			PdfAnnotationActions.MouseButtonReleasedDictionaryKey,
			PdfAnnotationActions.InputFocusReceivedDictionaryKey,
			PdfAnnotationActions.InputFocusLostDictionaryKey,
			PdfAnnotationActions.PageOpenedDictionaryKey,
			PdfAnnotationActions.PageClosedDictionaryKey,
			PdfAnnotationActions.PageBecameVisibleDictionaryKey,
			PdfAnnotationActions.PageBecameInvisibleDictionaryKey 
		};
		static bool isDictionaryContainsActions(PdfReaderDictionary dictionary, IEnumerable<string> actionDictionaryKeys) {
			foreach (string key in actionDictionaryKeys)
				if (dictionary.ContainsKey(key))
					return true;
			return false;
		}
		readonly PdfAnnotationActions annotationActions;
		readonly PdfInteractiveFormFieldActions interactiveFormFieldActions;
		public PdfAnnotationActions AnnotationActions { get { return annotationActions; } }
		public PdfInteractiveFormFieldActions InteractiveFormFieldActions { get { return interactiveFormFieldActions; } }
		internal PdfAdditionalActions(PdfAnnotation parent, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			annotationActions = isDictionaryContainsActions(dictionary, AnnotationActionsDictionaryKeys) ? new PdfAnnotationActions(parent, dictionary) : null;
			interactiveFormFieldActions = isDictionaryContainsActions(dictionary, InteractiveFormFieldActionsDictionaryKeys) ? new PdfInteractiveFormFieldActions(dictionary) : null;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			if (annotationActions != null)
				annotationActions.FillDictionary(dictionary);
			if (interactiveFormFieldActions != null)
				interactiveFormFieldActions.FillDictionary(dictionary);
			return dictionary;
		}
	}
}
