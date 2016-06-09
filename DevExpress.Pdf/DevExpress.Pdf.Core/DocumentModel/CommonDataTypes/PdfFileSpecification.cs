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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfAssociatedFileRelationship.Source)]
	public enum PdfAssociatedFileRelationship { Source, Data, Alternative, Supplement, EncryptedPayload }
	public class PdfFileSpecification : PdfObject {
		const string dictionaryType = "Filespec";
		const string fileSystemDictionaryKey = "FS";
		const string fileNameDictionaryKey = "F";
		const string unicodeFileNameDictionaryKey = "UF";
		const string embeddedFileDictionaryKey = "EF";
		const string descriptionDictionaryKey = "Desc";
		const string collectionItemDictionaryKey = "CI";
		const string relationshipDictionaryKey = "AFRelationship";
		const string parametersDictionaryKey = "Params";
		const string sizeDictionaryKey = "Size";
		const string creationDateDictionaryKey = "CreationDate";
		const string modificationDateDictionaryKey = "ModDate";
		const string indexDictionaryKey = "Index";
		internal static PdfFileSpecification Parse(PdfObjectCollection collection, object value) {
			PdfReaderDictionary fileSpecificationDictionary = value as PdfReaderDictionary;
			if (fileSpecificationDictionary != null)
				return new PdfFileSpecification(fileSpecificationDictionary);
			byte[] fileName = value as byte[];
			if (fileName == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfFileSpecification(PdfDocumentReader.ConvertToString(fileName));
		}
		internal static PdfFileSpecification Parse(PdfReaderDictionary dictionary, string key, bool required) {
			object value;
			if (!dictionary.TryGetValue(key, out value)) {
				if (required)
					PdfDocumentReader.ThrowIncorrectDataException();
				return null;
			}
			PdfObjectReference reference = value as PdfObjectReference;
			return reference == null ? Parse(dictionary.Objects, value) : dictionary.Objects.GetFileSpecification(reference);
		}
		readonly string fileSystem;
		readonly int index;
		string fileName;
		string mimeType;
		DateTimeOffset? creationDate;
		DateTimeOffset? modificationDate;
		PdfReaderStream compressedFileData;
		int? readedfileSize;
		byte[] fileData;
		string description;
		PdfAssociatedFileRelationship relationship;
		PdfFileAttachment attachment;
		public string FileSystem { get { return fileSystem; } }
		public int Index { get { return index; } }
		public string FileName {
			get { return fileName; }
			internal set { fileName = value; }
		}
		public string MimeType {
			get { return mimeType; }
			internal set { mimeType = value; }
		}
		public DateTimeOffset? CreationDate {
			get { return creationDate; }
			internal set { creationDate = value; }
		}
		public DateTimeOffset? ModificationDate {
			get { return modificationDate; }
			internal set { modificationDate = value; }
		}
		public byte[] FileData {
			get {
				if (fileData == null && compressedFileData != null) {
					fileData = compressedFileData.GetData(true);
					compressedFileData = null;
				}
				return fileData;
			}
			internal set {
				compressedFileData = null;
				fileData = value;
			}
		}
		public string Description {
			get { return description; }
			internal set { description = value; }
		}
		public PdfAssociatedFileRelationship Relationship {
			get { return relationship; }
			internal set { relationship = value; }
		}
		internal PdfFileAttachment Attachment {
			get {
				if (attachment == null)
					attachment = new PdfFileAttachment(this);
				return attachment;
			}
			set { attachment = value; }
		}
		PdfFileSpecification(PdfReaderDictionary dictionary)
			: base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			bool isTypeAbsent = String.IsNullOrEmpty(type);
			if (!isTypeAbsent && (type != dictionaryType && type != "FileSpec" && type != "F"))
				PdfDocumentReader.ThrowIncorrectDataException();
			fileSystem = dictionary.GetName(fileSystemDictionaryKey);
			fileName = dictionary.GetString(unicodeFileNameDictionaryKey) ??
				dictionary.GetString(fileNameDictionaryKey) ?? dictionary.GetString("DOS") ?? dictionary.GetString("Mac") ?? dictionary.GetString("Unix");
			PdfReaderDictionary embeddedFileDictionary = dictionary.GetDictionary(embeddedFileDictionaryKey);
			if (embeddedFileDictionary != null) {
				if (isTypeAbsent)
					PdfDocumentReader.ThrowIncorrectDataException();
				compressedFileData = embeddedFileDictionary.GetStream(fileNameDictionaryKey) ?? embeddedFileDictionary.GetStream("DOS") ?? embeddedFileDictionary.GetStream("Unix");
				if (compressedFileData == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfReaderDictionary embeddedStreamDictionary = compressedFileData.Dictionary;
				mimeType = embeddedStreamDictionary.GetName(PdfReaderDictionary.DictionarySubtypeKey);
				PdfReaderDictionary parametersDictionary = embeddedStreamDictionary.GetDictionary(parametersDictionaryKey);
				if (parametersDictionary != null) {
					readedfileSize = parametersDictionary.GetInteger(sizeDictionaryKey);
					creationDate = parametersDictionary.GetDate(creationDateDictionaryKey);
					modificationDate = parametersDictionary.GetDate(modificationDateDictionaryKey);
				}
			}
			description = dictionary.GetString(descriptionDictionaryKey);
			PdfReaderDictionary collectionItemDictionary = dictionary.GetDictionary(collectionItemDictionaryKey);
			if (collectionItemDictionary != null)
				index = collectionItemDictionary.GetInteger(indexDictionaryKey) ?? 0;
			relationship = dictionary.GetEnumName<PdfAssociatedFileRelationship>(relationshipDictionaryKey);
		}
		internal PdfFileSpecification(string fileName) {
			this.fileName = fileName;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, dictionaryType);
			dictionary.AddName(fileSystemDictionaryKey, fileSystem);
			dictionary.Add(fileNameDictionaryKey, Encoding.UTF8.GetBytes(fileName));
			dictionary.Add(unicodeFileNameDictionaryKey, fileName);
			if (compressedFileData != null) {
				object filters = null;
				object filterParams = null;
				if (compressedFileData.Dictionary.TryGetValue(PdfReaderStream.FilterDictionaryKey, out filters))
					compressedFileData.Dictionary.TryGetValue(PdfReaderStream.DecodeParametersDictionaryKey, out filterParams);
				dictionary.Add(embeddedFileDictionaryKey, WriteEmbeddedStream(objects, readedfileSize, compressedFileData.DecryptedData, filters, filterParams));
			}
			else if (fileData != null)
				dictionary.Add(embeddedFileDictionaryKey, WriteEmbeddedStream(objects, fileData.Length, fileData, null, null));
			dictionary.AddIfPresent(descriptionDictionaryKey, description);
			if (index != 0) {
				PdfDictionary collectionItemDictionary = new PdfDictionary();
				collectionItemDictionary.Add(indexDictionaryKey, index);
				dictionary.Add(collectionItemDictionaryKey, collectionItemDictionary);
			}
			dictionary.AddEnumName(relationshipDictionaryKey, relationship);
			return dictionary;
		}
		PdfWriterDictionary WriteEmbeddedStream(PdfObjectCollection objects, int? size, byte[] data, object filters, object filterParams) {
			PdfWriterDictionary embededStreamDictionary = new PdfWriterDictionary(objects);
			embededStreamDictionary.AddName(PdfReaderDictionary.DictionarySubtypeKey, mimeType);
			PdfWriterDictionary parametersDictionary = new PdfWriterDictionary(objects);
			parametersDictionary.AddIfPresent(sizeDictionaryKey, size);
			parametersDictionary.AddIfPresent(creationDateDictionaryKey, creationDate);
			parametersDictionary.AddIfPresent(modificationDateDictionaryKey, modificationDate);
			if(parametersDictionary.Count > 0)
				embededStreamDictionary.Add(parametersDictionaryKey, parametersDictionary);
			PdfWriterDictionary embeddedDictionary = new PdfWriterDictionary(objects);
			embededStreamDictionary.AddIfPresent(PdfReaderStream.FilterDictionaryKey, filters);
			embededStreamDictionary.AddIfPresent(PdfReaderStream.DecodeParametersDictionaryKey, filterParams);
			embeddedDictionary.Add(fileNameDictionaryKey, objects.AddStream(new PdfStream(embededStreamDictionary, data)));
			return embeddedDictionary;
		}
	}
}
