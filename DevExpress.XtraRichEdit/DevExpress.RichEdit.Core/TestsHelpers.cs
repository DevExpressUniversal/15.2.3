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

using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Export {
	using DevExpress.Office.Internal;
	using DevExpress.XtraRichEdit.Localization;
	using DevExpress.XtraRichEdit.Export;
	using DevExpress.Office.Export;
	using DevExpress.Office;
	using System.IO;
	using DevExpress.XtraRichEdit.Model;
	using DevExpress.XtraRichEdit.Export.Mht;
	using DevExpress.XtraRichEdit.Export.OpenXml;
	using System;
	using DevExpress.Office.Utils;
	using DevExpress.Utils.Zip;
	using DevExpress.XtraRichEdit.Internal;
	#region TemplateComparerTestsMhtDocumentExporter (stub class)
	public class TemplateComparerTestMhtDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_MhtFiles), "mht");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Mht; } }
		public IExporterOptions SetupSaving() {
			return new MhtDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.RaiseBeforeExport(DocumentFormat.Mht, options);
			MhtDocumentExporterOptions mhtOptions = (MhtDocumentExporterOptions)options;
			TemplateComparerTestMhtExporter exporter = PrepareMhtExport(model, mhtOptions);
			StreamWriter writer = new StreamWriter(stream, mhtOptions.Encoding);
			exporter.Export(writer);
			return true;
		}
		TemplateComparerTestMhtExporter PrepareMhtExport(DocumentModel documentModel, MhtDocumentExporterOptions options) {
			if (options == null) {
				options = new MhtDocumentExporterOptions();
				DocumentExporterOptions defaultOptions = documentModel.DocumentExportOptions.GetOptions(options.Format);
				if (defaultOptions != null)
					options.CopyFrom(defaultOptions);
			}
			documentModel.PreprocessContentBeforeExport(DocumentFormat.Mht);
			return new TemplateComparerTestMhtExporter(documentModel, options);
		}
		#endregion
	}
	#endregion
	#region TemplateComparerTestMhtExporter (stub class)
	public class TemplateComparerTestMhtExporter : MhtExporter {
		public static readonly string TestPackageId = "2bb67866-7e38-48c5-bc44-08782586803f";
		public TemplateComparerTestMhtExporter(DocumentModel documentModel, MhtDocumentExporterOptions options)
			: base(documentModel, options) {
		}
		protected internal override string CalculatePackageId() {
			return TestPackageId;
		}
	}
	#endregion
	#region TemplateComparerTestOpenXmlDocumentExporter (stub class)
	public class TemplateComparerTestOpenXmlDocumentExporter : IExporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_OpenXmlFiles), "docx");
		#region IDocumentExporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		public IExporterOptions SetupSaving() {
			return new OpenXmlDocumentExporterOptions();
		}
		public bool SaveDocument(IDocumentModel documentModel, Stream stream, IExporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			OpenXmlDocumentExporterOptions openXmlOptions = (OpenXmlDocumentExporterOptions)options;
			model.RaiseBeforeExport(DocumentFormat.OpenXml, openXmlOptions);
			TemplateComparerTestOpenXmlExporter exporter = new TemplateComparerTestOpenXmlExporter(model, openXmlOptions);
			exporter.Export(stream);
			return true;
		}
		#endregion
	}
	#endregion
	#region TemplateComparerTestOpenXmlExporter (stub class)
	public class TemplateComparerTestOpenXmlExporter : OpenXmlExporter {
		public static readonly string TestDocumentRelationId = "R4802085B";
		public static readonly DateTime LeninsBirthday = new DateTime(1870, 4, 22);
		public TemplateComparerTestOpenXmlExporter(DocumentModel documentModel, OpenXmlDocumentExporterOptions options)
			: base(documentModel, options) {
		}
		protected internal override string CalcDocumentRelationId() {
			return TestDocumentRelationId;
		}
		protected internal override void AddPackageImage(string fileName, OfficeImage image) {
			byte[] imageBytes = GetImageBytes(image);
			Package.Add(fileName, LeninsBirthday, imageBytes);
		}
		protected internal override void AddPackageContent(string fileName, Stream content) {
			Package.Add(fileName, LeninsBirthday, content);
		}
		protected internal override void AddPackageContent(string fileName, byte[] content) {
			Package.Add(fileName, LeninsBirthday, content);
		}
		protected internal override void AddCompressedPackageContent(string fileName, CompressedStream content) {
			Package.AddCompressed(fileName, LeninsBirthday, content);
		}
	}
	#endregion
}
#if DEBUGTEST
namespace DevExpress.XtraRichEdit.Tests {
	public static class DocumentModelCreator {
		public static DocumentModel Create() {
			return new DocumentModel(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		}
		public static DocumentModel Create(bool addDefaultsList, bool changeDefaultTableStyle) {
			return new DocumentModel(addDefaultsList, changeDefaultTableStyle, RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		}
	}
}
#endif
