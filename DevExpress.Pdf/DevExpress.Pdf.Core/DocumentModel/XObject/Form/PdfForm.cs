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
	public class PdfForm : PdfXObject {
		internal const string Type = "Form";
		const string bBoxDictionaryKey = "BBox";
		const string matrixDictionaryKey = "Matrix";
		const string resourceDictionaryString = "Resources";
		const string lastModifiedDictionaryKey = "LastModified";
		const string structParentsDictionaryKey = "StructParents";
		internal static PdfForm Create(PdfReaderStream stream, PdfResources parentResources) {
			PdfReaderDictionary group = stream.Dictionary.GetDictionary(PdfGroupForm.DictionaryKey);
			return group == null ? new PdfForm(stream, parentResources) : new PdfGroupForm(stream, group, parentResources);
		}
		readonly PdfResources resources;
		readonly Dictionary<string, PdfPieceInfoEntry> pieceInfo;
		readonly DateTimeOffset? lastModified;
		readonly int? structParents;
		PdfReaderStream stream;
		PdfCommandList commands;
		PdfRectangle bBox;
		PdfTransformationMatrix matrix;
		readonly PdfDocumentCatalog documentCatalog;
		public Dictionary<string, PdfPieceInfoEntry> PieceInfo { get { return pieceInfo; } }
		public DateTimeOffset? LastModified { get { return lastModified; } }
		public int? StructParents { get { return structParents; } }
		public PdfCommandList Commands {
			get {
				if (commands == null)
					commands = stream == null ? new PdfCommandList() : PdfContentStreamParser.GetContent(resources, stream.GetData(true));
				return commands;
			}
		}
		public PdfRectangle BBox {
			get { return bBox; }
			internal set { bBox = value; }
		}
		public PdfTransformationMatrix Matrix {
			get { return matrix; }
			internal set { matrix = value; }
		}
		internal PdfResources Resources { get { return resources; } }
		internal PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		internal PdfForm(PdfDocumentCatalog documentCatalog, PdfRectangle bBox) {
			if (bBox == null)
				throw new ArgumentNullException("bBox");
			this.bBox = bBox;
			resources = new PdfResources(documentCatalog, null, null, true);
			matrix = new PdfTransformationMatrix();
			this.documentCatalog = documentCatalog;
		}
		internal PdfForm(PdfForm form) {
			this.bBox = form.bBox;
			this.resources = form.resources;
			this.pieceInfo = form.pieceInfo;
			this.commands = new PdfCommandList(form.Commands);
			this.matrix = form.matrix;
			this.documentCatalog = form.documentCatalog;
		}
		internal PdfForm(PdfReaderStream stream, PdfResources parentResources) : base(stream.Dictionary) {
			this.stream = stream;
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? formType = dictionary.GetInteger("FormType");
			if (formType.HasValue && formType.Value != 1)
				PdfDocumentReader.ThrowIncorrectDataException();
			resources = dictionary.GetResources(resourceDictionaryString, parentResources, true);
			documentCatalog = dictionary.Objects.DocumentCatalog;
			pieceInfo = PdfPieceInfoEntry.Parse(dictionary);
			lastModified = dictionary.GetDate(lastModifiedDictionaryKey);
			structParents = dictionary.GetInteger(structParentsDictionaryKey);
			bBox = dictionary.GetRectangle(bBoxDictionaryKey) ?? new PdfRectangle(0, 0, 0, 0);
			matrix = new PdfTransformationMatrix(dictionary.GetArray(matrixDictionaryKey));
		}
		internal void InvalidateStream() {
			stream = null;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionarySubtypeKey, Type);
			dictionary.Add(bBoxDictionaryKey, bBox);
			if (!matrix.IsDefault)
				dictionary.Add(matrixDictionaryKey, matrix.Data);
			dictionary.Add(resourceDictionaryString, resources);
			PdfPieceInfoEntry.WritePieceInfo(dictionary, pieceInfo);
			dictionary.AddNullable(lastModifiedDictionaryKey, lastModified);
			dictionary.AddNullable(structParentsDictionaryKey, structParents);
			return dictionary;
		}
		protected override PdfStream CreateStream(PdfObjectCollection objects) {
			byte[] data;
			PdfWriterDictionary dictionary = CreateDictionary(objects);
			if (stream == null) {
				dictionary.Add(PdfReaderStream.FilterDictionaryKey, new PdfName(PdfFlateDecodeFilter.Name));
				data = Commands.ToStream(resources).RawData;
			}
			else {
				object filterValue;
				PdfReaderDictionary streamDictionary = stream.Dictionary;
				if (streamDictionary.TryGetValue(PdfReaderStream.FilterDictionaryKey, out filterValue))
					dictionary.Add(PdfReaderStream.FilterDictionaryKey, filterValue);
				if (streamDictionary.TryGetValue(PdfReaderStream.DecodeParametersDictionaryKey, out filterValue))
					dictionary.Add(PdfReaderStream.DecodeParametersDictionaryKey, filterValue);
				data = stream.DecryptedData;
			}
			return new PdfStream(dictionary, data);
		}
	}
}
