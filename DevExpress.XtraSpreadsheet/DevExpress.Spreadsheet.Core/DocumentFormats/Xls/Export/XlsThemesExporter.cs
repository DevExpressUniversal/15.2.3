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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsThemesExporter : OpenXmlExporter {
		#region Fields
		const string defaultThemeManagerRelationId = "rId1";
		const string ThemeManagerContentType = "application/vnd.openxmlformats-officedocument.themeManager+xml";
		readonly BinaryWriter streamWriter;
		#endregion
		public XlsThemesExporter(BinaryWriter streamWriter, DocumentModel workbook)
			: base(workbook, new OpenXmlDocumentExporterOptions()) {
			Guard.ArgumentNotNull(streamWriter, "streamWriter");
			this.streamWriter = streamWriter;
		}
		protected override string RootImagePath { get { return "theme"; } }
		public override void Export() {
			if (Workbook.Properties.IsDefaultThemeVersion)
				return;
			using (MemoryStream stream = new MemoryStream()) {
				CreateInternalZipArchive(stream);
				ExportTheme(stream.ToArray());
			}
		}
		void ExportTheme(byte[] themeData) {
			if (themeData.Length == 0)
				return;
			XlsCommandTheme firstCommand = new XlsCommandTheme();
			firstCommand.ThemeVersion = Workbook.Properties.DefaultThemeVersion;
			XlsCommandContinueFrt12 continueCommand = new XlsCommandContinueFrt12();
			using (XlsChunkWriter writer = new XlsChunkWriter(streamWriter, firstCommand, continueCommand)) {
				writer.Write(themeData);
			}
		}
		protected override void AddPackageContents() {
			ExportOfficeThemes();
			AddPackageContent(@"[Content_Types].xml", ExportContentTypes());
		}
		protected internal override void InitializeExport() {
			Builder.UsedContentTypes.Clear();
			Builder.OverriddenContentTypes.Clear();
			PopulateUsedContentTypesTable();
			PopulateOverriddenContentTypesTable();
			CreateExportedImages();
			CreateExportedImageTable();
		}
		protected internal override void ExportOfficeThemesCore() {
			AddPackageContent(@"_rels\.rels", ExportPackageRelations());
			AddPackageContent(@"theme\theme\themeManager.xml", ExportThemeManager());
			AddPackageContent(@"theme\theme\_rels\themeManager.xml.rels", ExportThemeManagerRelations());
			AddPackageContent(@"theme\theme\theme1.xml", ExportThemes());
			if (CurrentRelations.Count > 0)
				AddRelationsPackageContent(@"theme\theme\_rels\theme1.xml.rels", CurrentRelations);
		}
		protected internal override void PopulateUsedContentTypesTable() {
			Builder.UsedContentTypes.Add("rels", RelsContentType);
			Builder.UsedContentTypes.Add("xml", XmlContentType);
		}
		protected internal override void PopulateOverriddenContentTypesTable() {
			Builder.OverriddenContentTypes.Add("/theme/theme/theme1.xml", ThemeContentType);
			Builder.OverriddenContentTypes.Add("/theme/theme/themeManager.xml", ThemeManagerContentType);
		}
		protected internal override void GeneratePackageRelationsContent(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, new OpenXmlRelation(defaultThemeManagerRelationId, "theme/theme/themeManager.xml", OfficeDocumentNamespace));
		}
		#region ExportThemeManagerRelations
		CompressedStream ExportThemeManagerRelations() {
			return CreateXmlContent(GeneratePackageThemeRelationsContent);
		}
		void GeneratePackageThemeRelationsContent(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, new OpenXmlRelation(defaultThemeManagerRelationId, "theme1.xml", RelsThemeNamespace));
		}
		#endregion
		#region ExportThemeManager
		CompressedStream ExportThemeManager() {
			return CreateXmlContent(GenerateThemeManagerContent);
		}
		void GenerateThemeManagerContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateThemeManagerContent();
		}
		void GenerateThemeManagerContent() {
			WriteStartElement("a", "themeManager", DrawingMLNamespace);
			try {
				WriteStringAttr("xmlns", "a", null, DrawingMLNamespace);
			} finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
