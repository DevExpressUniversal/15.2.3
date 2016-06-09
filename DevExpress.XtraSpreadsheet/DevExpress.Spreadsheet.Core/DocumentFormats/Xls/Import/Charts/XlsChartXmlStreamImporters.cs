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
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region ShapePropStream importer
	public class XlsChartShapePropStreamImporter : OpenXmlImporter {
		#region Fields
		ShapeProperties shapeProperties;
		#endregion
		public XlsChartShapePropStreamImporter(ShapeProperties shapeProperties)
			: base(shapeProperties.DocumentModel, new OpenXmlDocumentImporterOptions()) {
				this.shapeProperties = shapeProperties;
		}
		public override void Import(Stream stream) {
			if (stream != null)
				stream.Seek(0, SeekOrigin.Begin);
			CreateDocumentRelationsStack();
			ImportShapeProperties(stream);
		}
		public override OfficeImage LookupImageByRelationId(IDocumentModel documentModel, string relationId, string rootFolder) {
			return null;
		}
		void ImportShapeProperties(Stream stream) {
			XmlReader reader = CreateXmlReader(stream);
			if (reader != null && ReadToRootElement(reader, "spPr", DrawingMLNamespace)) {
				Destination destination = new ChartShapePropertiesDestination(this, this.shapeProperties);
				destination.ProcessElementOpen(reader);
				ImportContent(reader, destination);
			}
		}
	}
	#endregion
	#region TextPropStream importer
	public class XlsChartTextPropStreamImporter : OpenXmlImporter {
		#region Fields
		TextProperties textProperties;
		#endregion
		public XlsChartTextPropStreamImporter(TextProperties textProperties)
			: base(textProperties.DocumentModel, new OpenXmlDocumentImporterOptions()) {
			this.textProperties = textProperties;
		}
		public override void Import(Stream stream) {
			if (stream != null)
				stream.Seek(0, SeekOrigin.Begin);
			CreateDocumentRelationsStack();
			ImportTextProperties(stream);
		}
		public override OfficeImage LookupImageByRelationId(IDocumentModel documentModel, string relationId, string rootFolder) {
			return null;
		}
		void ImportTextProperties(Stream stream) {
			XmlReader reader = CreateXmlReader(stream);
			if (reader != null && ReadToRootElement(reader, "txPr", DrawingMLNamespace)) {
				Destination destination = new TextPropertiesDestination(this, this.textProperties);
				destination.ProcessElementOpen(reader);
				ImportContent(reader, destination);
			}
		}
	}
	#endregion
}
