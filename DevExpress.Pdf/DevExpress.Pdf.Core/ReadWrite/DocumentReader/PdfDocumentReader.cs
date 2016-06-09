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
using System.Text;
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Pdf.Native {
	internal class PdfDocumentReader : PdfDocumentStructureReader {
		const string signature = "%PDF-";
		static readonly PdfTokenDescription xrefToken = new PdfTokenDescription(new byte[] { (byte)'x', (byte)'r', (byte)'e', (byte)'f' });
		static readonly PdfTokenDescription startxrefToken = new PdfTokenDescription(new byte[] { (byte)'s', (byte)'t', (byte)'a', (byte)'r', (byte)'t', (byte)'x', (byte)'r', (byte)'e', (byte)'f' });
		static readonly Dictionary<string, PdfFileVersion> versionMapping = new Dictionary<string, PdfFileVersion>() {
			{ "1.7", PdfFileVersion.Pdf_1_7 },
			{ "1.6", PdfFileVersion.Pdf_1_6 },
			{ "1.5", PdfFileVersion.Pdf_1_5 },
			{ "1.4", PdfFileVersion.Pdf_1_4 },
			{ "1.3", PdfFileVersion.Pdf_1_3 },
			{ "1.2", PdfFileVersion.Pdf_1_2 },
			{ "1.1", PdfFileVersion.Pdf_1_1 },
			{ "1.0", PdfFileVersion.Pdf_1_0 }
		};
		internal static double ConvertToDouble(object value) {
			if (value is double)
				return (double)value;
			if (!(value is int))
				ThrowIncorrectDataException();
			return (int)value;
		}
		internal static bool IsUnicode(byte[] value) {
			int length = value.Length;
			return length >= 2 && value[0] == 254 && value[1] == 255;
		}
		internal static string ConvertToUnicodeString(byte[] value) {
			return Encoding.BigEndianUnicode.GetString(value, 0, value.Length).Substring(1);
		}
		internal static string ConvertToString(byte[] value) {
			return IsUnicode(value) ? ConvertToUnicodeString(value) : DXEncoding.GetEncoding("Windows-1252").GetString(value, 0, value.Length);
		}
		internal static PdfPoint CreatePoint(IList<object> array, int index) {
			double x = PdfDocumentReader.ConvertToDouble(array[index++]);
			double y = PdfDocumentReader.ConvertToDouble(array[index]);
			return new PdfPoint(x, y);
		}
		internal static PdfPoint[] CreatePointArray(IList<object> array) {
			if (array == null)
				return null;
			int length = array.Count;
			if (length % 2 != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			length /= 2;
			PdfPoint[] points = new PdfPoint[length];
			for (int i = 0, index = 0; i < length; i++, index += 2)
				points[i] = CreatePoint(array, index);
			return points;
		}
		internal static PdfRange CreateDomain(IList<object> array, int index) {
			double min = PdfDocumentReader.ConvertToDouble(array[index++]);
			double max = PdfDocumentReader.ConvertToDouble(array[index]);
			if (max < min)
				ThrowIncorrectDataException();
			return new PdfRange(min, max);
		}
		internal static IList<PdfRange> CreateDomain(IList<object> array) {
			if (array == null)
				ThrowIncorrectDataException();
			int count = array.Count;
			if (count == 0 || count % 2 > 0)
				ThrowIncorrectDataException();
			count /= 2;
			List<PdfRange> result = new List<PdfRange>();
			for (int i = 0, index = 0; i < count; i++, index += 2)
				result.Add(CreateDomain(array, index));
			return result;
		}
		internal static PdfFileVersion FindVersion(string versionString) {
			foreach (KeyValuePair<string, PdfFileVersion> pair in versionMapping)
				if (versionString == pair.Key)
					return pair.Value;
			ThrowIncorrectDataException();
			return PdfFileVersion.Pdf_1_7;
		}
		internal static PdfDocument Read(Stream stream, bool detachStreamAfterLoadComplete, PdfGetPasswordAction getPasswordAction = null) {
			BufferedStream bufferedStream = new BufferedStream(stream);
			int i = 0;
			int signatureLength = signature.Length;
			long streamLength = stream.Length;
			while (bufferedStream.Position < streamLength) {
				int streamByte = bufferedStream.ReadByte();
				i = streamByte == signature[i] ? i + 1 : 0;
				if (i == signatureLength) {
					bufferedStream.Position -= signatureLength;
					break;
				}
			}
			try {
				long pos = bufferedStream.Position;
				if (pos >= streamLength)
					bufferedStream.Position = 0;
				try {
					return new PdfDocumentReader(new PdfDocumentStream(bufferedStream), getPasswordAction ?? new PdfGetPasswordAction((n) => null), false).Read(detachStreamAfterLoadComplete);
				}
				catch (PdfIncorrectPasswordException) {
					throw;
				}
				catch {
					bufferedStream.Position = pos >= streamLength ? 0 : pos;
					return new PdfDocumentReader(new PdfDocumentStream(bufferedStream), getPasswordAction ?? new PdfGetPasswordAction((n) => null), true).Read(detachStreamAfterLoadComplete);
				}
			}
			catch {
				if (bufferedStream != null)
					bufferedStream = null;
				throw;
			}
		}
		internal static void FindObjects(PdfObjectCollection objects, PdfDocumentStream streamReader) {
			objects.RemoveCorruptedObjects();
			streamReader.Position = 0;
			long streamLength = streamReader.Length;
			long position = streamReader.Position;
			long startxrefPosition = -1;
			for (int data = streamReader.ReadByte(); data >= 0; position = streamReader.Position, data = streamReader.ReadByte()) {
				if (PdfObjectParser.IsSpaceSymbol((byte)data))
					continue;
				if (data == Comment) {
					streamReader.ReadString();
					continue;
				}
				streamReader.Position = position;
				if (streamReader.ReadToken(xrefToken)) {
					startxrefPosition = streamReader.Position;
					if(!streamReader.FindToken(EofToken))
						break;
					continue;
				}
				streamReader.Position = position;
				if (streamReader.ReadToken(startxrefToken)) {
					if (streamReader.ReadNumber() != 0 && streamReader.FindToken(EofToken))
						startxrefPosition = position;
					continue;
				}
				else {
					try {
						PdfObjectSlot slot = streamReader.ReadObject(position);
						objects.AddItem(slot, true);
					}
					catch {
						if (startxrefPosition >= 0)
							break;
						else
							continue;
					}
				}
			}
			streamReader.Position = startxrefPosition;
		}
		static PdfFileVersion FindPdfVersion(string versionString) {
			foreach (KeyValuePair<string, PdfFileVersion> pair in versionMapping)
				if (versionString.StartsWith(pair.Key))
					return pair.Value;
			ThrowIncorrectDataException();
			return PdfFileVersion.Pdf_1_7;
		}
		readonly PdfGetPasswordAction getPasswordAction;
		object encryptValue;
		PdfFileVersion version = PdfFileVersion.Pdf_1_7;
		PdfObjectReference[] idObjects;
		object documentInfoValue = null;
		byte[][] id;
		PdfDocumentReader(PdfDocumentStream documentStream, PdfGetPasswordAction getPasswordAction, bool isInvalid) : base(documentStream) {
			this.getPasswordAction = getPasswordAction;
			version = GetVersion(documentStream);
			PdfObjectCollection objects = Objects;
			long length = documentStream.Length;
			try {
				int startxref = GetStartXRef(documentStream);
				bool updateTrailer = true;
				do {
					PdfReaderDictionary trailerDictionary = ReadTrailer(startxref, true);
					if (updateTrailer)
						UpdateTrailer(trailerDictionary, objects);
					updateTrailer = false;
					startxref = trailerDictionary.GetInteger("Prev") ?? -1;
				} while (startxref != -1);
				if (isInvalid)
					ReadCorruptedDocument(objects);
			}
			catch {
				ReadCorruptedDocument(objects);
			}
		}
		protected override void UpdateTrailer(PdfReaderDictionary trailerDictionary, PdfObjectCollection objects) {
			base.UpdateTrailer(trailerDictionary, objects);
			if (!trailerDictionary.TryGetValue("Encrypt", out encryptValue))
				encryptValue = null;
			trailerDictionary.TryGetValue(PdfObjectCollection.TrailerInfoKey, out documentInfoValue);
			IList<object> array = trailerDictionary.GetArray("ID");
			if (array != null) {
				if (array.Count != 2)
					ThrowIncorrectDataException();
				byte[] id0 = array[0] as byte[];
				if (id0 != null) {
					id = new byte[2][];
					id[0] = id0;
					byte[] id1 = array[1] as byte[];
					if (id1 == null)
						ThrowIncorrectDataException();
					id[1] = id1;
				}
				else {
					PdfObjectReference ref1 = array[0] as PdfObjectReference;
					PdfObjectReference ref2 = array[1] as PdfObjectReference;
					if (ref1 == null || ref2 == null)
						ThrowIncorrectDataException();
					idObjects = new PdfObjectReference[] { ref1, ref1 };
				}
			}
		}
		void ReadCorruptedDocument(PdfObjectCollection objects) {
			PdfDocumentStream streamReader = DocumentStream;
			FindObjects(objects, streamReader);
			if (streamReader.ReadToken(startxrefToken)) {
				int trailerOffset = streamReader.ReadNumber();
				if (trailerOffset != 0) {
					long position = streamReader.Position;
					UpdateTrailer(ReadTrailer(trailerOffset, false), objects);
					streamReader.Position = position;
					streamReader.SkipEmptySpaces();
				}
				if (!streamReader.ReadToken(EofToken))
					ThrowIncorrectDataException();
			}
			if (streamReader.FindToken(TrailerToken))
				UpdateTrailer(PdfDocumentParser.ParseDictionary(objects, 0, 0, new PdfArrayData(ReadTrailerData())), objects);
		}
		PdfFileVersion GetVersion(PdfDocumentStream streamReader) {
			string header = streamReader.ReadString();
			while (!header.StartsWith(signature)) {
				if (header.Length == 0 || header[0] != Comment)
					ThrowIncorrectDataException();
				header = streamReader.ReadString();
			}
			return FindPdfVersion(header.Substring(signature.Length));
		}
		long FindLastEndOfFile(PdfDocumentStream streamReader) {
			long startPosition = streamReader.Position;
			streamReader.SetPositionFromEnd(30);
			long result = streamReader.FindLastToken(EofToken, false);
			if (result == -1) {
				streamReader.Position = startPosition;
				result = streamReader.FindLastToken(EofToken);
			}
			return result;
		}
		int GetStartXRef(PdfDocumentStream streamReader) {
			const int requilredSpace = 50;
			long readoffset = FindLastEndOfFile(streamReader) - requilredSpace;
			if (readoffset < 0)
				ThrowIncorrectDataException();
			streamReader.Position = readoffset;
			byte[] buffer = streamReader.ReadBytes(requilredSpace);
			int? startxref = PdfObjectParser.ParseStartXRef(buffer);
			if (startxref == null)
				ThrowIncorrectDataException();
			return startxref.Value;
		}
		byte[] ReadTrailerData() {
			PdfDocumentStream streamReader = DocumentStream;
			List<byte> data = new List<byte>();
			PdfTokenDescription token = PdfTokenDescription.BeginCompare(startxrefToken);
			for (;;) {
				int next = streamReader.ReadByte();
				if (next == -1)
					return data.ToArray();
				byte symbol = (byte)next;
				data.Add(symbol);
				if (token.Compare(symbol)) {
					int tokenLength = token.Length;
					data.RemoveRange(data.Count - tokenLength, tokenLength);
					return data.ToArray();
				}
			}
		}
		PdfReaderDictionary ReadTrailer(long offset, bool fillCrossReferenceTable) {
			PdfObjectCollection objects = Objects;
			PdfDocumentStream streamReader = DocumentStream;
			streamReader.Position = offset;
			if (streamReader.ReadToken(xrefToken)) {
				if (fillCrossReferenceTable) {
					for (;;) {
						long position = streamReader.Position;
						int next = streamReader.SkipSpaces();
						if (next == -1 || !PdfObjectParser.IsDigitSymbol((byte)next)) {
							streamReader.Position = position;
							break;
						}
						streamReader.Position--;
						int number = streamReader.ReadNumber();
						int count = streamReader.ReadNumber();
						for (int i = 0; i < count; i++) {
							long objectOffset = streamReader.ReadNumber(10);
							next = streamReader.ReadByte();
							if (next != ' ')
								ThrowIncorrectDataException();
							int generation = (int)streamReader.ReadNumber(5);
							next = streamReader.ReadByte();
							if (next != ' ')
								ThrowIncorrectDataException();
							next = streamReader.ReadByte();
							switch (next) {
								case 'n':
									objects.AddItem(new PdfObjectSlot(number, generation, objectOffset), false);
									break;
								case 'f':
									objects.AddFreeObject(number, generation);
									break;
								default:
									ThrowIncorrectDataException();
									break;
							}
							next = streamReader.ReadByte();
							switch (next) {
								case ' ':
								case CarriageReturn:
									next = streamReader.ReadByte();
									if (next != LineFeed && next != CarriageReturn)
										ThrowIncorrectDataException();
									break;
								case LineFeed:
									break;
								default:
									ThrowIncorrectDataException();
									break;
							}
							number++;
						}
					}
					if (!streamReader.ReadToken(TrailerToken))
						ThrowIncorrectDataException();
				}
				else if (!streamReader.FindToken(TrailerToken))
					ThrowIncorrectDataException();
				return PdfDocumentParser.ParseDictionary(objects, 0, 0, new PdfArrayData(ReadTrailerData()));
			}
			else {
				PdfReaderStream pdfStream = PdfDocumentParser.ParseStream(objects, 0, 0, streamReader.ReadIndirectObject(offset).Data);
				PdfReaderDictionary trailerDictionary = pdfStream.Dictionary;
				IList<object> array = trailerDictionary.GetArray("W");
				if (array == null || array.Count != 3 || trailerDictionary.GetName(PdfDictionary.DictionaryTypeKey) != "XRef")
					ThrowIncorrectDataException();
				if (fillCrossReferenceTable) {
					object typeWeight = array[0];
					object offsetWeight = array[1];
					object generationWeight = array[2];
					if (!(typeWeight is int) || !(offsetWeight is int) || !(generationWeight is int))
						ThrowIncorrectDataException();
					List<PdfIndexDescription> indices = new List<PdfIndexDescription>();
					array = trailerDictionary.GetArray("Index");
					if (array == null) {
						int? sizeValue = trailerDictionary.GetInteger(PdfObjectCollection.TrailerSizeKey);
						if (!sizeValue.HasValue)
							ThrowIncorrectDataException();
						int size = sizeValue.Value;
						if (size < 1)
							ThrowIncorrectDataException();
						indices.Add(new PdfIndexDescription(0, size));
					}
					else {
						int count = array.Count;
						if (count % 2 == 1)
							ThrowIncorrectDataException();
						count /= 2;
						for (int i = 0, index = 0; i < count; i++) {
							object startValue = array[index++];
							object valuesCount = array[index++];
							if (!(startValue is int) || !(valuesCount is int))
								ThrowIncorrectDataException();
							indices.Add(new PdfIndexDescription((int)startValue, (int)valuesCount));
						}
					}
					PdfCrossReferenceDecoder.Decode(pdfStream.GetData(true), indices, objects, (int)typeWeight, (int)offsetWeight, (int)generationWeight);
				}
				return trailerDictionary;
			}
		}
		PdfDocument Read(bool detachStreamAfterLoadComplete) {
			PdfObjectCollection objects = Objects;
			if (detachStreamAfterLoadComplete)
				objects.ResolveAllSlots();
			PdfEncryptionInfo encryptionInfo;
			if (encryptValue == null)
				encryptionInfo = null;
			else {
				if (id == null && idObjects != null) {
					byte[] id0 = objects.GetObjectData(idObjects[0].Number) as byte[];
					byte[] id1 = objects.GetObjectData(idObjects[1].Number) as byte[];
					if (id0 == null || id1 == null)
						ThrowIncorrectDataException();
					id = new byte[2][];
					id[0] = id0;
					id[1] = id1;
				}
				encryptionInfo = objects.EnsureEncryptionInfo(encryptValue, id, getPasswordAction);
			}
			PdfReaderDictionary documentCatalogDictionary = objects.GetDictionary(RootObjectNumber);
			if (documentCatalogDictionary == null)
				ThrowIncorrectDataException();
			PdfDocumentCatalog documentCatalog = new PdfDocumentCatalog(documentCatalogDictionary);
			string versionString = documentCatalog.Version;
			if (!String.IsNullOrEmpty(versionString)) {
				PdfFileVersion updatedVersion = FindVersion(versionString);
				if (updatedVersion > version)
					version = updatedVersion;
			}
			PdfDocumentInfo documentInfo = null;
			if (documentInfoValue != null) {
				PdfReaderDictionary documentInfoDictionary = objects.TryResolve(documentInfoValue) as PdfReaderDictionary;
				if (documentInfoDictionary != null)
					documentInfo = new PdfDocumentInfo(documentInfoDictionary);
			}
			return new PdfDocument(version, documentInfo ?? new PdfDocumentInfo(), documentCatalog, encryptionInfo, id);
		}
	}
}
