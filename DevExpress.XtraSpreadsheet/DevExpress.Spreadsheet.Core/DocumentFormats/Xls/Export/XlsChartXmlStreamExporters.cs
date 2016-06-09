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
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xlsx;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsChartPackageBuilder : XlsxPackageBuilder {
		public XlsChartPackageBuilder(Stream outputStream)
			: base(outputStream) {
		}
		protected override InternalZipArchive CreateDocumentPackage(Stream stream) {
			return null;
		}
	}
	public class XlsChartShapePropStreamExporter : OpenXmlExporter {
		#region Fields
		ShapeProperties shapeProperties;
		#endregion
		public XlsChartShapePropStreamExporter(ShapeProperties shapeProperties)
			: base(shapeProperties.DocumentModel, new OpenXmlDocumentExporterOptions()) {
			this.shapeProperties = shapeProperties;
		}
		public bool WriteAutomatic { get; set; }
		public override void Export(Stream outputStream) {
			if (!ShouldExportChartShapeProperties(shapeProperties) && !WriteAutomatic)
				return;
			BeginExport(outputStream);
			try {
				InitializeExport();
				WriteContent(outputStream);
			}
			finally {
				EndExport();
			}
		}
		protected override XlsxPackageBuilder CreatePackageBuilder(Stream stream) {
			return new XlsChartPackageBuilder(stream);
		}
		void WriteContent(Stream outputStream) {
			using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, CreateXmlWriterSettings())) {
				DocumentContentWriter = xmlWriter;
				WriteStartElement("a", "spPr", DrawingMLNamespace);
				try {
					if (ShouldExportChartShapeProperties(shapeProperties)) {
						GenerateDrawingFillContent(shapeProperties.Fill);
						GenerateOutlineContent(shapeProperties.Outline, "ln");
						ExportDrawingEffectStyle(shapeProperties.EffectStyle);
					}
				}
				finally {
					WriteEndElement();
				}
			}
		}
		protected override bool ShouldExportChartShapeProperties(ShapeProperties properties) {
			bool shouldExportFill = properties.Fill.FillType != DrawingFillType.Automatic &&
				properties.Fill.FillType != DrawingFillType.Picture; 
			return shouldExportFill || properties.Outline.Fill.FillType != DrawingFillType.Automatic || !properties.EffectStyle.IsDefault;
		}
		protected internal override void GenerateDrawingFillContent(IDrawingFill fill) {
			if (fill.FillType == DrawingFillType.Picture)
				return; 
			base.GenerateDrawingFillContent(fill);
		}
	}
}
