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
	public class PdfInkAnnotation : PdfMarkupAnnotation {
		internal const string Type = "Ink";
		const string inksDictionaryKey = "InkList";
		readonly IList<PdfPoint[]> inks = new List<PdfPoint[]>();
		readonly PdfAnnotationBorderStyle borderStyle;
		public IList<PdfPoint[]> Inks { get { return inks; } }
		public PdfAnnotationBorderStyle BorderStyle { get { return borderStyle; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfInkAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			IList<object> inkList = dictionary.GetArray(inksDictionaryKey);
			if (inkList != null)
				foreach (object ink in inkList) {
					IList<object> array = ink as IList<object>;
					if (array == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					inks.Add(PdfDocumentReader.CreatePointArray(array));
				}
			borderStyle = PdfAnnotationBorderStyle.Parse(dictionary);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.AddList<PdfPoint[]>(inksDictionaryKey, inks, value => new PdfWritablePointsArray(value));
			dictionary.Add(PdfAnnotationBorderStyle.DictionaryKey, borderStyle);
			return dictionary;
		}
	}
}
