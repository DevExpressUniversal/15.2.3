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
using System.IO;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	internal class PdfDocumentWriter : PdfObjectWriter {
		public static byte[] headerComment = new byte[] { 0x25, 0xA2, 0xA3, 0x8F, 0x93, 0x0D, 0x0A };
		public static PdfObjectCollection Write(Stream stream, PdfDocument document, PdfSaveOptions options) {
			BufferedStream bufferedStream = new BufferedStream(stream);
			PdfObjectCollection result = new PdfDocumentWriter(bufferedStream, document, options).Write();
			return result;
		}
		public static PdfObjectCollection Write(Stream stream, PdfDocument document) {
			return Write(stream, document, new PdfSaveOptions());
		}
		readonly PdfObjectCollection objects;
		readonly PdfDocument document;
		readonly PdfSignature signature;
		SortedDictionary<int, long> xref = new SortedDictionary<int, long>();
		internal PdfObjectCollection Objects { get { return objects; } }
		internal PdfDocumentWriter(Stream stream, PdfDocument document, PdfSaveOptions options) : base(stream) {
			this.document = document;
			this.signature = options.Signature;
			objects = new PdfObjectCollection(Stream, WriteIndirectObject);
			objects.PrepareToWrite(document.DocumentCatalog);
			objects.EncryptionInfo = PdfEncryptionInfo.Create(document.ID, options.EncryptionOptions);
			objects.AddFreeObject(0, 65535);
			Stream.WriteString("%PDF-1.7" + EndOfLine);
			Stream.WriteBytes(headerComment);
		}
		internal PdfObjectCollection Write() {
			if (signature != null)
				PdfSignatureFormField.CreateSignatureFormField(document.DocumentCatalog, signature);
			PdfObjectReference[] references = document.Write(objects);
			objects.FinalizeWritingAndClearWriteParameters();
			WriteIndirectObjects();
			long xrefPosition = WriteCrossReferenceTable();
			WriteTrailer(references[0], references[1]);
			Stream.WriteString(EndOfLine + "startxref" + EndOfLine);
			Stream.WriteString(xrefPosition.ToString(CultureInfo.InvariantCulture));
			Stream.WriteString(EndOfLine + "%%EOF");
			Stream.PatchSignature();
			if (document.DocumentCatalog.Objects.IsStreamDetached)
				objects.ResolveAllSlots();
			Stream.Flush();
			return objects;
		}
		void WriteIndirectObjects() {
			using (IEnumerator<PdfObjectContainer> enumerator = objects.EnumeratorOfContainers)
				while (enumerator.MoveNext())
					WriteIndirectObject(enumerator.Current);
		}
		long WriteCrossReferenceTable() {
			long xrefPosition = Stream.Position;
			CultureInfo culture = CultureInfo.InvariantCulture;
			Stream.WriteString("xref" + EndOfLine);
			using (IEnumerator<KeyValuePair<int, long>> enumerator = xref.GetEnumerator()) {
				int previousNumber = 0;
				int sectionStartNumber = 0;
				List<string> references = new List<string>() { "0000000000 00000 f\r\n" };
				while (enumerator.MoveNext()) {
					KeyValuePair<int, long> current = enumerator.Current;
					if (++previousNumber != current.Key) {
						WriteReferencesSection(sectionStartNumber, references);
						previousNumber = current.Key;
						sectionStartNumber = current.Key;
						references.Clear();
					}
					references.Add(String.Format(culture, "{0:0000000000} 00000 n\r\n", current.Value));
				}
				WriteReferencesSection(sectionStartNumber, references);
			}
			return xrefPosition;
		}
		void WriteReferencesSection(int firstNumber, List<string> references) {
			Stream.WriteStringFormat("{0} {1}" + EndOfLine, firstNumber, references.Count);
			foreach (string reference in references)
				Stream.WriteString(reference);
		}
		void WriteTrailer(PdfObjectReference info, PdfObjectReference catalog) {
			Stream.WriteString("trailer" + EndOfLine);
			PdfDictionary trailerDictionary = new PdfDictionary();
			trailerDictionary.Add(PdfObjectCollection.TrailerSizeKey, xref.Count);
			if (info != null)
				trailerDictionary.Add(PdfObjectCollection.TrailerInfoKey, info);
			PdfEncryptionInfo encryptionInfo = objects.EncryptionInfo;
			trailerDictionary.Add("ID", document.ID);
			if (encryptionInfo != null) {
				trailerDictionary.Add("Encrypt", encryptionInfo.CreateDictionary(objects));
			}
			trailerDictionary.Add(PdfObjectCollection.TrailerRootKey, catalog);
			Stream.WriteObject(trailerDictionary, PdfObject.DirectObjectNumber);
		}
		public override PdfObjectSlot WriteIndirectObject(PdfObjectContainer container) {
			PdfObjectSlot slot = base.WriteIndirectObject(container);
			xref.Add(slot.ObjectNumber, slot.Offset);
			objects.AddItem(slot, true);
			return slot;
		}
	}
}
