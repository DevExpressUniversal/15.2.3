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
using DevExpress.Pdf;
using DevExpress.Mvvm;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfDocumentProperties : BindableBase, IPdfDocumentProperties {
		string fileName;
		string title;
		string author;
		string subject;
		string keywords;
		DateTime? created;
		DateTime? modified;
		string application;
		string producer;
		string version;
		string location;
		long fileSize;
		int numberOfPages;
		string pageSize;
		public string FileName { 
			get { return fileName; }
			internal set { SetProperty(ref fileName, value, () => FileName); }
		}
		public string Title {
			get { return title; }
			internal set { SetProperty(ref title, value, () => Title); }
		}
		public string Author {
			get { return author; }
			internal set { SetProperty(ref author, value, () => Author); }
		}
		public string Subject {
			get { return subject; }
			internal set { SetProperty(ref subject, value, () => Subject); }
		}
		public string Keywords {
			get { return keywords; }
			internal set { SetProperty(ref keywords, value, () => Keywords); }
		}
		public DateTime? Created {
			get { return created; }
			internal set { SetProperty(ref created, value, () => Created); }
		}
		public DateTime? Modified {
			get { return modified; }
			internal set { SetProperty(ref modified, value, () => Modified); }
		}
		public string Application {
			get { return application; }
			internal set { SetProperty(ref application, value, () => Application); }
		}
		public string Producer {
			get { return producer; }
			internal set { SetProperty(ref producer, value, () => Producer); }
		}
		public string Version {
			get { return version; }
			internal set { SetProperty(ref version, value, () => Version); }
		}
		public string Location {
			get { return location; }
			internal set { SetProperty(ref location, value, () => Location); }
		}
		public long FileSize {
			get { return fileSize; }
			internal set { SetProperty(ref fileSize, value, () => FileSize); }
		}
		public int NumberOfPages {
			get { return numberOfPages; }
			internal set { SetProperty(ref numberOfPages, value, () => NumberOfPages); }
		}
		public string PageSize {
			get { return pageSize; }
			internal set { SetProperty(ref pageSize, value, () => PageSize); }
		}
		public PdfDocumentProperties(IPdfDocument document) {
			PdfDocumentViewModel doc = (PdfDocumentViewModel)document;
			PdfDocument pdfDoc = doc.PdfDocument;
			string filePath = doc.FilePath;
			if (!string.IsNullOrEmpty(doc.FilePath)) {
				FileName = Path.GetFileName(filePath);
				Location = Path.GetDirectoryName(filePath);
			}
			FileSize = doc.FileSize;
			switch (pdfDoc.Version) {
				case PdfFileVersion.Pdf_1_0:
					Version = "1.0";
					break;
				case PdfFileVersion.Pdf_1_1:
					Version = "1.1";
					break;
				case PdfFileVersion.Pdf_1_2:
					Version = "1.2";
					break;
				case PdfFileVersion.Pdf_1_3:
					Version = "1.3";
					break;
				case PdfFileVersion.Pdf_1_4:
					Version = "1.4";
					break;
				case PdfFileVersion.Pdf_1_5:
					Version = "1.5";
					break;
				case PdfFileVersion.Pdf_1_6:
					Version = "1.6";
					break;
				case PdfFileVersion.Pdf_1_7:
					Version = "1.7";
					break;
			}
			Title = pdfDoc.Title;
			Author = pdfDoc.Author;
			Subject = pdfDoc.Subject;
			Keywords = pdfDoc.Keywords;
			Application = pdfDoc.Creator;
			Producer = pdfDoc.Producer;
			if (pdfDoc.CreationDate.HasValue)
				Created = pdfDoc.CreationDate.Value.DateTime;
			if (pdfDoc.ModDate.HasValue)
				Modified = pdfDoc.ModDate.Value.DateTime;
			NumberOfPages = doc.Pages.Count;
		}
	}
	public class FileSizeConverter : IValueConverter {
		static readonly PdfViewerStringId[] units = new PdfViewerStringId[] { 
			PdfViewerStringId.UnitKiloBytes, 
			PdfViewerStringId.UnitMegaBytes,
			PdfViewerStringId.UnitGigaBytes,
			PdfViewerStringId.UnitTeraBytes,
			PdfViewerStringId.UnitPetaBytes,
			PdfViewerStringId.UnitExaBytes,
			PdfViewerStringId.UnitZettaBytes
		};
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double fileSize = (long)value;
			string fileSizeString = fileSize.ToString("N0");
			if (fileSize < 1024)
				fileSizeString = String.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.FileSizeInBytes), fileSizeString);
			else {
				int i = 0;
				while ((fileSize /= 1024) >= 1024)
					i++;
				fileSizeString = String.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.FileSize), fileSize, PdfViewerLocalizer.GetString(units[i]), fileSizeString);
			}
			return fileSizeString;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
