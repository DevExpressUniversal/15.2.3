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
	public abstract class PdfUnclosedPathAnnotation : PdfPathAnnotation {
		const string lineEndingDictionaryKey = "LE";
		static PdfAnnotationLineEndingStyle ParseLineEnding(object value) {
			PdfName name = value as PdfName;
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfEnumToStringConverter.Parse<PdfAnnotationLineEndingStyle>(name.Name, false);
		}
		readonly PdfAnnotationLineEndingStyle startLineEnding = PdfAnnotationLineEndingStyle.None;
		readonly PdfAnnotationLineEndingStyle finishLineEnding = PdfAnnotationLineEndingStyle.None;
		public PdfAnnotationLineEndingStyle StartLineEnding { get { return startLineEnding; } }
		public PdfAnnotationLineEndingStyle FinishLineEnding { get { return finishLineEnding; } }
		protected PdfUnclosedPathAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			IList<object> lineEndingList = dictionary.GetArray(lineEndingDictionaryKey);
			if (lineEndingList != null) {
				if (lineEndingList.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				startLineEnding = ParseLineEnding(lineEndingList[0]);
				finishLineEnding = ParseLineEnding(lineEndingList[1]);
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			if (startLineEnding != PdfAnnotationLineEndingStyle.None || finishLineEnding != PdfAnnotationLineEndingStyle.None)
				dictionary[lineEndingDictionaryKey] = new object[] { new PdfName(PdfEnumToStringConverter.Convert<PdfAnnotationLineEndingStyle>(startLineEnding, false)), 
																	 new PdfName(PdfEnumToStringConverter.Convert<PdfAnnotationLineEndingStyle>(finishLineEnding, false)) };
			return dictionary;
		}
	}
}
