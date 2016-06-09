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
	public class PdfObjectStream : PdfDocumentItem {
		readonly List<object> objects;
		public IList<object> Objects { get { return objects; } }
		public PdfObjectStream(PdfReaderStream stream) : base(stream.Dictionary.Number, stream.Dictionary.Generation) {
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? n = dictionary.GetInteger("N");
			int? firstValue = dictionary.GetInteger("First");
			if (dictionary.GetName(PdfDictionary.DictionaryTypeKey) != "ObjStm" || !n.HasValue || !firstValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			int count = n.Value;
			int first = firstValue.Value;
			if (count <= 0 || first < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			objects = new List<object>(count);
			PdfDocumentParser parser = new PdfDocumentParser(dictionary.Objects, dictionary.Number, dictionary.Generation, new PdfArrayData(stream.GetData(true)), first);
			for (int i = 0; i < count; i++)
				objects.Add(parser.ReadObject(false, false));
		}
	}
}
