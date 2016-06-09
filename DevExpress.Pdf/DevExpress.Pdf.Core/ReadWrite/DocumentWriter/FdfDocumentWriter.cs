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

using System.IO;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	internal class FdfDocumentWriter : PdfObjectWriter {
		const string fdfVersion_1_2 = "1.2";
		const string fdfDictionaryKey = "FDF";
		const string versionDictionaryKey = "Version";
		public static void Write(Stream stream, PdfFormData formData) {
			BufferedStream bufferedStream = new BufferedStream(stream);
			new FdfDocumentWriter(bufferedStream, formData).Write();
			bufferedStream.Flush();
		}
		readonly PdfObjectCollection objects;
		readonly PdfFormData formData;
		FdfDocumentWriter(Stream stream, PdfFormData formData) : base(stream) {
			objects = new PdfObjectCollection(Stream);
			this.formData = formData;
		}
		void Write() {
			Stream.WriteString("%FDF-" + fdfVersion_1_2 + EndOfLine);
			PdfWriterDictionary fdfCatalog = new PdfWriterDictionary(objects);
			fdfCatalog.Add(fdfDictionaryKey, formData.CreateRootDictionary(objects));
			fdfCatalog.AddName(versionDictionaryKey, fdfVersion_1_2);
			PdfObjectReference rootRef = objects.AddDictionary(fdfCatalog);
			using (IEnumerator<PdfObjectContainer> enumerator = objects.EnumeratorOfContainers)
				while (enumerator.MoveNext())
					WriteIndirectObject(enumerator.Current);
			Stream.WriteString("trailer" + EndOfLine);
			PdfDictionary trailerDictionary = new PdfDictionary();
			trailerDictionary.Add(PdfObjectCollection.TrailerRootKey, rootRef);
			Stream.WriteObject(trailerDictionary, PdfObject.DirectObjectNumber);
			Stream.WriteString(EndOfLine + "%%EOF");
		}
	}
}
