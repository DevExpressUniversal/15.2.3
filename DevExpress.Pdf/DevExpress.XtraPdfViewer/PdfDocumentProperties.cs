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
using System.IO;
using DevExpress.Pdf;
namespace DevExpress.XtraPdfViewer {
	public class PdfDocumentProperties { 
		readonly string filePath = String.Empty;
		readonly string fileName = String.Empty;
		readonly string fileLocation = String.Empty;
		readonly long fileSize;
		readonly string version;
		readonly string title;
		readonly string author;
		readonly string subject;
		readonly string keywords;
		readonly string application;
		readonly string producer;
		readonly DateTimeOffset? creationDate;
		readonly DateTimeOffset? modificationDate;
		public string FilePath { get { return filePath; } }
		public string FileName { get { return fileName; } }
		public string FileLocation { get { return fileLocation; } }
		public long FileSize { get { return fileSize; } }
		public string Version { get { return version; } }
		public string Title { get { return title; } }
		public string Author { get { return author; } }
		public string Subject { get { return subject; } }
		public string Keywords { get { return keywords; } }
		public string Application { get { return application; } }
		public string Producer { get { return producer; } }
		public DateTimeOffset? CreationDate { get { return creationDate; } }
		public DateTimeOffset? ModificationDate { get { return modificationDate; } }
		internal PdfDocumentProperties(string filePath, long fileSize, PdfDocument document) {
			if (!String.IsNullOrEmpty(filePath)) {
				this.filePath = filePath;
				fileName = Path.GetFileName(filePath);
				fileLocation = Path.GetDirectoryName(filePath);
			}
			this.fileSize = fileSize;
			switch (document.Version) {
				case PdfFileVersion.Pdf_1_0:
					version = "1.0";
					break;
				case PdfFileVersion.Pdf_1_1:
					version = "1.1";
					break;
				case PdfFileVersion.Pdf_1_2:
					version = "1.2";
					break;
				case PdfFileVersion.Pdf_1_3:
					version = "1.3";
					break;
				case PdfFileVersion.Pdf_1_4:
					version = "1.4";
					break;
				case PdfFileVersion.Pdf_1_5:
					version = "1.5";
					break;
				case PdfFileVersion.Pdf_1_6:
					version = "1.6";
					break;
				case PdfFileVersion.Pdf_1_7:
					version = "1.7";
					break;
			}
			title = document.Title;
			author = document.Author;
			subject = document.Subject;
			keywords = document.Keywords;
			application = document.Creator;
			producer = document.Producer;
			creationDate = document.CreationDate;
			modificationDate = document.ModDate;
		}
	}
}
