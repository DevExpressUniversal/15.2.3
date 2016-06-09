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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfLinkAnnotation : PdfAnnotation {
		const string destinationDictionaryKey = "Dest";
		const string uriActionDictionaryKey = "PA";
		internal const string Type = "Link";
		PdfAction action;
		PdfDestinationObject destination;
		PdfAnnotationHighlightingMode highlightingMode;
		PdfUriAction uriAction;
		IList<PdfQuadrilateral> region;
		PdfAnnotationBorderStyle borderStyle;
		public PdfAction Action {
			get {
				Ensure();
				return action;
			}
		}
		public PdfDestination Destination { get {
				Ensure();
				return destination == null ? null : destination.GetDestination(DocumentCatalog, true);
			}
		}
		public PdfAnnotationHighlightingMode HighlightingMode {
			get {
				Ensure();
				return highlightingMode;
			}
		}
		public PdfUriAction UriAction {
			get {
				Ensure();
				return uriAction;
			}
		}
		public IList<PdfQuadrilateral> Region {
			get {
				Ensure();
				return region;
			}
		}
		public PdfAnnotationBorderStyle BorderStyle {
			get {
				Ensure();
				return borderStyle;
			}
		}
		protected override string AnnotationType { get { return Type; } }
		internal PdfLinkAnnotation(PdfPage page, PdfRectangle rect, string destinationName) : base(page, rect) {
			destination = new PdfDestinationObject(destinationName);
		}
		internal PdfLinkAnnotation(PdfPage page, PdfRectangle rect, Uri uri) : base(page, rect) {
			action = new PdfUriAction(DocumentCatalog, uri);
		}
		internal PdfLinkAnnotation(PdfPage page, PdfReaderDictionary dictionary)
			: base(page, dictionary) {
		}
		protected override void ResolveDictionary(PdfReaderDictionary dictionary) {
			base.ResolveDictionary(dictionary);
			action = dictionary.GetAction(PdfDictionary.DictionaryActionKey);
			destination = dictionary.GetDestination(destinationDictionaryKey);
			if (action != null && destination != null)
				PdfDocumentReader.ThrowIncorrectDataException();
			highlightingMode = dictionary.GetAnnotationHighlightingMode();
			PdfAction pa = dictionary.GetAction(uriActionDictionaryKey);
			if (pa != null) {
				uriAction = pa as PdfUriAction;
				if (uriAction == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			region = PdfQuadrilateral.ParseArray(dictionary);
			borderStyle = PdfAnnotationBorderStyle.Parse(dictionary);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryActionKey, action);
			if (destination != null)
				dictionary.Add(destinationDictionaryKey, destination.ToWriteableObject(DocumentCatalog, objects, true));
			if (highlightingMode != PdfAnnotationHighlightingMode.Invert)
				dictionary.AddEnumName(PdfDictionary.DictionaryAnnotationHighlightingModeKey, highlightingMode);
			if (uriAction != null)
				dictionary.Add(uriActionDictionaryKey, objects.AddObject(uriAction));
			if (borderStyle != null)
				dictionary.Add(PdfAnnotationBorderStyle.DictionaryKey, objects.AddObject(borderStyle));
			PdfQuadrilateral.Write(dictionary, region);
			return dictionary;
		}
	}
}
