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
namespace DevExpress.Pdf.Native {
	public class PdfFileAttachmentList : IEnumerable<PdfFileAttachment> {
		readonly PdfDocumentCatalog documentCatalog;
		readonly PdfDeferredSortedDictionary<string, PdfFileSpecification> embeddedFiles;
		List<PdfFileAttachmentAnnotation> fileAttachmentAnnotations;
		internal PdfProgressChangedEventHandler SearchAttachmentProgressChanged;
		public PdfFileAttachmentList(PdfDocumentCatalog documentCatalog) {
			this.documentCatalog = documentCatalog;
			embeddedFiles = (PdfDeferredSortedDictionary<string, PdfFileSpecification>)documentCatalog.Names.EmbeddedFiles;
		}
		public void Add(PdfFileAttachment item) {
			embeddedFiles.Add(PdfNames.NewKey(embeddedFiles), item.FileSpecification);
		}
		public bool Delete(PdfFileAttachment item) {
			SearchFileAttachmentAnnotation();
			string key = null;
			foreach (KeyValuePair<string, PdfFileSpecification> pair in embeddedFiles)
				if (pair.Value != null && pair.Value.Attachment == item) {
					key = pair.Key;
					break;
				}
			if (!String.IsNullOrEmpty(key))
				return embeddedFiles.Remove(key);
			PdfFileAttachmentAnnotation fileAttachmentAnnotation = null;
			foreach (PdfFileAttachmentAnnotation annotation in fileAttachmentAnnotations)
				if (annotation.FileSpecification.Attachment == item) {
					fileAttachmentAnnotation = annotation;
					break;
				}
			if (fileAttachmentAnnotation != null) {
				fileAttachmentAnnotation.Page.Annotations.Remove(fileAttachmentAnnotation);
				return fileAttachmentAnnotations.Remove(fileAttachmentAnnotation);
			}
			return false;
		}
		public void InvalidateSearchedFileAnnotations() {
			fileAttachmentAnnotations = null;
		}
		public IEnumerator<PdfFileAttachment> GetEnumerator() {
			SearchFileAttachmentAnnotation();
			foreach (PdfFileAttachmentAnnotation annotation in fileAttachmentAnnotations)
				yield return annotation.FileSpecification.Attachment;
			foreach (PdfFileSpecification fileSpecification in embeddedFiles.Values)
				yield return fileSpecification.Attachment;
		}
		void SearchFileAttachmentAnnotation() {
			if (fileAttachmentAnnotations == null) {
				List<PdfFileAttachmentAnnotation> result = new List<PdfFileAttachmentAnnotation>();
				int currentPageNumber = 1;
				foreach (PdfPage page in documentCatalog.Pages) {
					foreach (PdfAnnotation annotation in page.Annotations) {
						PdfFileAttachmentAnnotation fileAttachmentAnnotation = annotation as PdfFileAttachmentAnnotation;
						if (fileAttachmentAnnotation != null)
							result.Add(fileAttachmentAnnotation);
					}
					if (SearchAttachmentProgressChanged != null)
						SearchAttachmentProgressChanged(page, new PdfProgressChangedEventArgs(currentPageNumber++));
				}
				fileAttachmentAnnotations = result;
			}
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
