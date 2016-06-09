#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting.Native;
using System.IO;
using System.ComponentModel;
namespace DevExpress.XtraPrinting {
	public enum PdfAttachmentRelationship {
		Alternative,
		Data,
		Source,
		Supplement,
		Unspecified
	}
	public class PdfAttachment {
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentCreationDate")]
#endif
		public DateTime? CreationDate { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentModificationDate")]
#endif
		public DateTime? ModificationDate { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentType")]
#endif
		public string Type { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentRelationship")]
#endif
		public PdfAttachmentRelationship Relationship { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentData")]
#endif
		public byte[] Data { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentFileName")]
#endif
		public string FileName { get; set; }
		public string Description { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfAttachmentFilePath")]
#endif
		public string FilePath { get; set; }
	}
}
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfEmbeddedFiles {
		List<PdfFilespec> fileNames;
		bool compressed;
		public PdfArray AFArray {
			get {
				AssertState();
				PdfArray array = new PdfArray();
				foreach(PdfFilespec spec in fileNames)
					array.Add(spec.InnerObject);
				return array;
			}
		}
		public PdfDictionary NamesDictionary {
			get {
				AssertState();
				PdfArray names = new PdfArray();
				foreach(PdfFilespec spec in fileNames) {
					names.Add(new PdfLiteralString(spec.Name));
					names.Add(spec.InnerObject);
				}
				PdfDictionary embeddedFilesDictionary = new PdfDictionary();
				embeddedFilesDictionary.Add("Names", names);
				PdfDictionary namesDictionary = new PdfDictionary();
				namesDictionary.Add("EmbeddedFiles", embeddedFilesDictionary);
				return namesDictionary;
			}
		}
		public PdfEmbeddedFiles(bool compressed) {
			this.compressed = compressed;
		}
		static string GetName(PdfAttachment attachment) {
			if(!string.IsNullOrEmpty(attachment.FileName))
				return attachment.FileName;
			if(!string.IsNullOrEmpty(attachment.FilePath))
				return Path.GetFileName(attachment.FilePath);
			return null;
		}
		static byte[] GetData(PdfAttachment attachment) {
			if(attachment.Data != null)
				return attachment.Data;
			byte[] data = null;
			try {
				data = File.ReadAllBytes(attachment.FilePath);
				attachment.ModificationDate = attachment.ModificationDate ?? File.GetLastWriteTime(attachment.FilePath);
				attachment.CreationDate = attachment.CreationDate ?? File.GetCreationTime(attachment.FilePath);
			} catch { }
			return data;
		}
		public bool Active { get { return fileNames != null && fileNames.Count > 0; } }
		public void Register(PdfXRef xRef) {
			AssertState();
			foreach(PdfFilespec spec in fileNames) {
				spec.Register(xRef);
				spec.EmbeddedFile.Register(xRef);
			}
		}
		public void Write(StreamWriter writer) {
			AssertState();
			foreach(PdfFilespec spec in fileNames) {
				spec.Write(writer);
				spec.EmbeddedFile.Write(writer);
			}
		}
		public void FillUp() {
			AssertState();
			foreach(PdfFilespec spec in fileNames) {
				spec.FillUp();
				spec.EmbeddedFile.FillUp();
			}
		}
		internal void SetAttachments(ICollection<PdfAttachment> attachments) {
			fileNames = new List<PdfFilespec>();
			if(attachments.Count > 0) {
				foreach(PdfAttachment attachment in attachments) {
					string name = GetName(attachment);
					if(string.IsNullOrEmpty(name))
						name = "Attachment";
					byte[] data = GetData(attachment);
					if(data == null)
						continue;
					PdfEmbeddedFile file = new PdfEmbeddedFile(compressed)
					{
						Subtype = string.IsNullOrEmpty(attachment.Type) ? "application/octet-stream" : attachment.Type,
						CreationDate = attachment.CreationDate,
						ModificationDate = attachment.ModificationDate ?? DateTimeHelper.Now,
						Data = data,
					};
					PdfFilespec fileName = new PdfFilespec()
					{
						Name = name,
						Description = attachment.Description,
						Relationship = attachment.Relationship,
						EmbeddedFile = file,
					};
					fileNames.Add(fileName);
				}
			}
		}
		void AssertState() {
			if(fileNames == null) throw new InvalidOperationException();
		}
#if DEBUGTEST
		public List<PdfFilespec> Test_FileNames {
			get { return fileNames; }
		}
#endif
	}
	public class PdfFilespec : PdfDocumentDictionaryObject {
		public string Name { get; set; }
		public string Description { get; set; }
		public PdfAttachmentRelationship Relationship { get; set; }
		public PdfEmbeddedFile EmbeddedFile { get; set; }
		public PdfFilespec()
			: base(false) {
		}
		public override void FillUp() {
			base.FillUp();
			Dictionary.Add("Type", "Filespec");
			Dictionary.Add("F", new PdfLiteralString(Name));
			Dictionary.Add("UF", new PdfTextUnicode(Name));
			if(Description != null)
				Dictionary.Add("Desc", new PdfTextUnicode(Description));
			Dictionary.Add("AFRelationship", Relationship.ToString());
			PdfDictionary ef = new PdfDictionary();
			ef.Add("F", EmbeddedFile.InnerObject);
			ef.Add("UF", EmbeddedFile.InnerObject);
			Dictionary.Add("EF", ef);
		}
	}
	public class PdfEmbeddedFile : PdfDocumentStreamObject {
		public string Subtype { get; set; }
		public DateTime? CreationDate { get; set; }
		public DateTime? ModificationDate { get; set; }
		public byte[] Data { get; set; }
		public PdfEmbeddedFile(bool compressed)
			: base(compressed) {
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("Type", "EmbeddedFile");
			Attributes.Add("Subtype", Subtype);
			PdfDictionary paramsDictionary = new PdfDictionary();
			if(CreationDate.HasValue)
				paramsDictionary.Add("CreationDate", new PdfDate(CreationDate.Value));
			if(ModificationDate.HasValue)
				paramsDictionary.Add("ModDate", new PdfDate(ModificationDate.Value));
			Attributes.Add("Params", paramsDictionary);
			Stream.SetBytes(Data);
		}
	}
}
