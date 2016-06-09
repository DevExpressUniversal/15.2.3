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
using System.Security.Cryptography;
using DevExpress.Utils;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfDocument {
		readonly PdfFileVersion version;
		readonly PdfDocumentInfo documentInfo;
		readonly PdfDocumentCatalog documentCatalog;
		readonly PdfDocumentPermissionsFlags permissionsFlags = PdfDocumentPermissionsFlags.Accessibility | PdfDocumentPermissionsFlags.DataExtraction |
																PdfDocumentPermissionsFlags.DocumentAssembling | PdfDocumentPermissionsFlags.FormFilling |
																PdfDocumentPermissionsFlags.HighQualityPrinting | PdfDocumentPermissionsFlags.Modifying |
																PdfDocumentPermissionsFlags.ModifyingFormFieldsAndAnnotations | PdfDocumentPermissionsFlags.Printing;
		byte[][] id;
		PdfDocumentWriter writer;
		public PdfFileVersion Version { get { return version; } }
		public string Title {
			get { return documentInfo.Title; }
			set {
				documentInfo.Title = value;
				UpdateMetadata();
			}
		}
		public string Author {
			get { return documentInfo.Author; }
			set {
				documentInfo.Author = value;
				UpdateMetadata();
			}
		}
		public string Subject {
			get { return documentInfo.Subject; }
			set {
				documentInfo.Subject = value;
				UpdateMetadata();
			}
		}
		public string Keywords {
			get { return documentInfo.Keywords; }
			set {
				documentInfo.Keywords = value;
				UpdateMetadata();
			}
		}
		public string Creator {
			get { return documentInfo.Creator; }
			set {
				documentInfo.Creator = value;
				UpdateMetadata();
			}
		}
		public string Producer {
			get { return documentInfo.Producer; }
			set {
				documentInfo.Producer = value;
				UpdateMetadata();
			}
		}
		public DateTimeOffset? CreationDate { get { return documentInfo.CreationDate; } }
		public DateTimeOffset? ModDate { get { return documentInfo.ModDate; } }
		public DefaultBoolean Trapped {
			get { return documentInfo.Trapped; }
			set { documentInfo.Trapped = value; }
		}
		public IList<PdfPage> Pages { get { return documentCatalog.Pages; } }
		public IDictionary<int, PdfPageLabel> PageLabels { get { return documentCatalog.PageLabels; } }
		public PdfNames Names { get { return documentCatalog.Names; } }
		public IDictionary<string, PdfDestination> Destinations { get { return documentCatalog.Destinations; } }
		public PdfViewerPreferences ViewerPreferences { get { return documentCatalog.ViewerPreferences; } }
		public PdfPageLayout PageLayout { get { return documentCatalog.PageLayout; } }
		public PdfPageMode PageMode { get { return documentCatalog.PageMode; } }
		public PdfOutlines Outlines { get { return documentCatalog.Outlines; } }
		public IList<PdfBookmark> Bookmarks {
			get { return documentCatalog.Bookmarks; }
			set {
				if (value == null)
					throw new ArgumentNullException("Bookmarks", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectBookmarkListValue));
				documentCatalog.Bookmarks = value;
			}
		}
		public IList<PdfArticleThread> Threads { get { return documentCatalog.Threads; } }
		public PdfDestination OpenDestination { get { return documentCatalog.OpenDestination; } }
		public PdfAction OpenAction { get { return documentCatalog.OpenAction; } }
		public PdfDocumentActions Actions { get { return documentCatalog.Actions; } }
		public PdfInteractiveForm AcroForm { get { return documentCatalog.AcroForm; } }
		public PdfMetadata Metadata { get { return documentCatalog.Metadata; } }
		public PdfLogicalStructure LogicalStructure { get { return documentCatalog.LogicalStructure; } }
		public PdfMarkInfo MarkInfo { get { return documentCatalog.MarkInfo; } }
		public CultureInfo LanguageCulture { get { return documentCatalog.LanguageCulture; } }
		public IList<PdfOutputIntent> OutputIntents { get { return documentCatalog.OutputIntents; } }
		public IDictionary<string, PdfPieceInfoEntry> PieceInfo { get { return documentCatalog.PieceInfo; } }
		public PdfOptionalContentProperties OptionalContentProperties { get { return documentCatalog.OptionalContentProperties; } }
		public bool NeedsRendering { get { return documentCatalog.NeedsRendering; } }
		public bool AllowPrinting { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.Printing); } }
		public bool AllowModifying { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.Modifying); } }
		public bool AllowDataExtraction { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.DataExtraction); } }
		public bool AllowAnnotationsAndFormsModifying { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.ModifyingFormFieldsAndAnnotations); } }
		public bool AllowFormsFilling { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.FormFilling); } }
		public bool AllowAccessibility { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.Accessibility); } }
		public bool AllowDocumentAssembling { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.DocumentAssembling); } }
		public bool AllowHighQualityPrinting { get { return permissionsFlags.HasFlag(PdfDocumentPermissionsFlags.HighQualityPrinting); } }
		public IEnumerable<PdfFileAttachment> FileAttachments { get { return documentCatalog.FileAttachments; } }
		internal PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		internal byte[][] ID {
			get {
				if (id != null)
					return id;
				byte[] uniqueKey = Guid.NewGuid().ToByteArray();
				id = new byte[][] { uniqueKey, uniqueKey };
				return id;
			}
		}
		internal event PdfProgressChangedEventHandler ProgressChanged;
		internal PdfDocument(PdfFileVersion version, PdfDocumentInfo documentInfo, PdfDocumentCatalog documentCatalog, PdfEncryptionInfo encryptionInfo, byte[][] id) {
			this.version = version;
			this.documentInfo = documentInfo;
			this.documentCatalog = documentCatalog;
			this.id = id;
			documentCatalog.ProgressChanged += (o, e) => RaiseProgressChanged(e.ProgressValue);
			if (encryptionInfo != null)
				permissionsFlags = encryptionInfo.PermissionsFlags;
		}
		internal PdfDocument(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			CheckOptions(creationOptions);
			writer = new PdfDocumentWriter(new BufferedStream(stream), this, saveOptions);
			documentInfo = new PdfDocumentInfo();
			PdfObjectCollection objects = writer.Objects;
			documentCatalog = new PdfDocumentCatalog(writer.Objects, creationOptions);
			UpdateMetadata();
		}
		internal PdfDocument(PdfCreationOptions creationOptions) {
			CheckOptions(creationOptions);
			documentInfo = new PdfDocumentInfo();
			documentCatalog = new PdfDocumentCatalog(new PdfObjectCollection(null), creationOptions);
			UpdateMetadata();
		}
		internal PdfDocument() : this(null) {
		}
		void CheckOptions(PdfCreationOptions creationOptions) {
			if (creationOptions != null && creationOptions.Compatibility != PdfCompatibility.Pdf && (creationOptions.DisableEmbeddingAllFonts || (creationOptions.NotEmbeddedFontFamilies != null && creationOptions.NotEmbeddedFontFamilies.Count != 0)))
				throw new NotSupportedException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgShouldEmbedFonts)); 
		}
		internal void UpdateObjects(PdfObjectCollection objects) {
			documentCatalog.Objects = objects;
		}
		internal PdfPage AddPage(PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) {
			return documentCatalog.AddPage(mediaBox, cropBox, rotate);
		}
		internal PdfPage AddPage(PdfRectangle mediaBox) {
			return AddPage(mediaBox, null, 0);
		}
		internal PdfPage InsertPage(int pageNumber, PdfRectangle mediaBox, PdfRectangle cropBox, int rotate) {
			return documentCatalog.AddPage(pageNumber, mediaBox, cropBox, rotate);
		}
		internal PdfPage InsertPage(int pageNumber, PdfRectangle mediaBox) {
			return InsertPage(pageNumber, mediaBox, null, 0);
		}
		internal void DeletePage(int pageNumber) {
			documentCatalog.DeletePage(pageNumber);
		}
		internal void Append(PdfDocument document) {
			documentCatalog.Append(document.documentCatalog);
		}
		internal void AttachFile(PdfFileAttachment attachment) {
			if(documentCatalog.CreationOptions.Compatibility == PdfCompatibility.PdfA2b)
				throw new NotSupportedException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnsupportedFileAttachments)); 
			documentCatalog.FileAttachments.Add(attachment);
		}
		internal bool DeleteAttachment(PdfFileAttachment attachment) {
			return documentCatalog.FileAttachments.Delete(attachment);
		}
		internal void FinalizeDocument() {
			if (writer != null) {
				writer.Write();
				writer = null;
			}
		}
		internal PdfObjectReference[] Write(PdfObjectCollection objects) {
			documentCatalog.ResetProgress();
			DateTimeOffset dateNow = DateTimeOffset.Now;
			documentInfo.CreationDate = dateNow;
			documentInfo.ModDate = dateNow;
			return new PdfObjectReference[] { documentCatalog.CreationOptions.Compatibility == PdfCompatibility.PdfA2b ? null : objects.AddObject(documentInfo), objects.AddObject(documentCatalog) };
		}
		void UpdateMetadata() {
			documentCatalog.Metadata = documentInfo.GetMetadata(documentCatalog.CreationOptions.Compatibility);
		}
		void RaiseProgressChanged(int percentage) {
			if (ProgressChanged != null)
				ProgressChanged(this, new PdfProgressChangedEventArgs(percentage));
		}
	}
}
