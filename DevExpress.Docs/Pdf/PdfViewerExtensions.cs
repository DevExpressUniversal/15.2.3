#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public static class PdfViewerExtensions {
		public static void SaveDocument(this IPdfViewer viewer, Stream stream, PdfSaveOptions options) {
			PdfDocumentProcessorHelper helper = viewer.GetDocumentProcessorHelper();
			helper.Save(stream, options);
		}
		public static void SaveDocument(this IPdfViewer viewer, string path, PdfSaveOptions options) {
			PdfDocumentProcessorHelper helper = viewer.GetDocumentProcessorHelper();
			helper.Save(path, options);
		}
		public static void Export(this IPdfViewer viewer, Stream stream, PdfFormDataFormat format) {
			PdfDocumentProcessorHelper helper = viewer.GetDocumentProcessorHelper();
			if (helper != null) {
				PdfFormData formData = helper.GetFormData();
				if (formData == null)
					throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
				formData.Save(stream, format);
			}
		}
		public static void Export(this IPdfViewer viewer, string fileName, PdfFormDataFormat format) {
			using (Stream stream = File.Create(fileName))
				viewer.Export(stream, format);
		}
		public static void Import(this IPdfViewer viewer, Stream stream, PdfFormDataFormat format) {
			DoImport(viewer, new PdfFormData(stream, format));
		}
		public static void Import(this IPdfViewer viewer, string fileName, PdfFormDataFormat format) {
			DoImport(viewer, new PdfFormData(fileName, format));
		}
		public static void Import(this IPdfViewer viewer, Stream stream) {
			DoImport(viewer, new PdfFormData(stream));
		}
		public static void Import(this IPdfViewer viewer, string fileName) {
			DoImport(viewer, new PdfFormData(fileName));
		}
		static void DoImport(IPdfViewer viewer, PdfFormData source) {
			PdfDocumentProcessorHelper helper = viewer.GetDocumentProcessorHelper();
			if (helper != null) {
				PdfFormData formData = helper.GetFormData();
				if (formData == null)
					throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
				formData.Apply(source);
			}
		}
	}
}
