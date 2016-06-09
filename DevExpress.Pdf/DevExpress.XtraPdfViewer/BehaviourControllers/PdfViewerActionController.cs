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
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using System.Windows.Forms;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfViewerActionController : IPdfViewerActionController {
		PdfViewer viewer;
		public PdfViewerActionController(PdfViewer viewer) {
			this.viewer = viewer;
		}
		string IPdfViewerActionController.SaveFileAttachment(PdfFileAttachment attachment) {
			using (SaveFileDialog sfd = new SaveFileDialog()) {
				sfd.FileName = attachment.FileName;
				if (sfd.ShowDialog() == DialogResult.OK)
					return sfd.FileName;
			}
			return null;
		}
		bool IPdfViewerActionController.OpenFileAttachment(PdfFileAttachment attachment) {
			PdfFileAttachmentOpeningEventArgs args = new PdfFileAttachmentOpeningEventArgs(attachment);
			viewer.RaiseFileAttachmentOpening(args);
			return !args.Cancel;
		}
		PdfActionRequestResult IPdfViewerActionController.UriAccessRequest(Uri uri) {
			PdfUriOpeningEventArgs args = new PdfUriOpeningEventArgs(uri);
			viewer.RaiseUriOpening(args);
			return args.Cancel ? PdfActionRequestResult.Cancel : PdfActionRequestResult.Ok;
		}
		void IPdfViewerActionController.RunPrintDialog() {
			viewer.Print();
		}
		void IPdfViewerActionController.SelectionStarted(PdfDocumentPosition position) {
			viewer.RaiseSelectionStarted(position);
		}
		void IPdfViewerActionController.SelectionContinued(PdfDocumentPosition position) {
			viewer.RaiseSelectionContinued(position);
		}
		void IPdfViewerActionController.SelectionEnded(PdfDocumentPosition position) {
			viewer.RaiseSelectionEnded(position);
		}
		PdfActionRequestResult IPdfViewerActionController.OpenDocument(string filePath, bool openInNewWindow, PdfTarget target) {
			PdfDocument documentInStart = viewer.Document;
			PdfReferencedDocumentOpeningEventArgs args = new PdfReferencedDocumentOpeningEventArgs(GetFilePath(filePath), openInNewWindow);
			viewer.OnReferencedDocumentOpening(args);
			if (args.Cancel)
				return PdfActionRequestResult.Cancel;
			PdfActionRequestResult result = PdfActionRequestResult.Cancel;
			viewer.HistoryController.PerformLockedOperation(() => {
				if (!openInNewWindow) {
					viewer.LoadDocument(args.DocumentFilePath);
					if (!object.ReferenceEquals(documentInStart, viewer.Document)) {
						viewer.ShowDocumentPosition(target);
						result = PdfActionRequestResult.Ok;
					}
				}
				else
					PdfProcessorLauncher.Launch(args.DocumentFilePath);
			});
			viewer.RegisterCurrentDocumentViewState(PdfNavigationMode.ReferencedDocumentOpening);
			return result;
		}
		string GetFilePath(string fileSpecificationFileName) {
			if (String.IsNullOrWhiteSpace(fileSpecificationFileName) || String.IsNullOrEmpty(viewer.DocumentFilePath))
				return fileSpecificationFileName;
			if (File.Exists(fileSpecificationFileName))
				return Path.GetFullPath(fileSpecificationFileName);
			string fileName = fileSpecificationFileName;
			if (!Path.IsPathRooted(fileName)) {
				string currentDirectory = Path.GetDirectoryName(viewer.DocumentFilePath);
				if (!String.IsNullOrEmpty(currentDirectory)) {
					try {
						string filePath = Path.GetFullPath(currentDirectory + "\\" + fileSpecificationFileName);
						if (File.Exists(filePath))
							return filePath;
						filePath = Path.GetFullPath(currentDirectory + "\\" + Path.GetFileName(fileName));
						if (File.Exists(filePath))
							return filePath;
					}
					catch { }
				}
			}
			return fileName;
		}
	}
}
