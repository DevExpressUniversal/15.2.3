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
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Zip;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		const string DocumentCustomPropertiesContentType = @"application/vnd.openxmlformats-officedocument.custom-properties+xml";
		const string VariantTypePrefix = "vt";
		const string VariantTypeNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";
		protected internal virtual void AddDocumentCustomPropertiesContent() {
			if (ShouldExportDocumentCustomProperties())
				AddPackageContent(@"docProps/custom.xml", ExportDocumentCustomProperties());
		}
		protected internal virtual bool ShouldExportDocumentCustomProperties() {
			ModelDocumentCustomProperties properties = Workbook.DocumentCustomProperties;
			return properties.Count > 0;
		}
		protected internal virtual CompressedStream ExportDocumentCustomProperties() {
			return CreateXmlContent(GenerateDocumentCustomPropertiesXmlContent);
		}
		protected internal virtual void GenerateDocumentCustomPropertiesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateDocumentCustomPropertiesContent();
		}
		protected internal virtual void GenerateDocumentCustomPropertiesContent() {
			WriteStartElement("Properties", CustomPropertiesNamespace);
			try {
				WriteStringAttr("xmlns", VariantTypePrefix, null, VariantTypeNamespace);
				int index = 2;
				ModelDocumentCustomProperties properties = Workbook.DocumentCustomProperties;
				foreach (string name in properties.Names) {
					WriteDocumentCustomProperty(name, properties[name], index);
					index++;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteDocumentCustomProperty(string name, CellValue cellValue, int index) {
			if (cellValue.Type == CellValueType.Error || cellValue.Type == CellValueType.None || cellValue.Type == CellValueType.Unknown)
				return;
			WriteStartElement("property", CustomPropertiesNamespace);
			try {
				WriteStringValue("fmtid", "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}");
				WriteIntValue("pid", index);
				WriteStringValue("name", name);
				switch (cellValue.Type) {
					case CellValueType.Numeric:
						if (cellValue.NumericValue == (double)(int)cellValue.NumericValue)
							WriteVariantValue("i4", ((int)cellValue.NumericValue).ToString());
						else
							WriteVariantValue("r8", cellValue.NumericValue.ToString(Workbook.Culture));
						break;
					case CellValueType.Boolean:
						WriteVariantValue("bool", cellValue.BooleanValue ? "true" : "false");
						break;
					case CellValueType.DateTime:
						WriteVariantValue("filetime", cellValue.DateTimeValue.ToUniversalTime().ToString(Workbook.Culture.DateTimeFormat.SortableDateTimePattern) + 'Z');
						break;
					case CellValueType.Text:
						WriteVariantValue("lpwstr", cellValue.TextValue);
						break;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteVariantValue(string tag, string value) {
			WriteString(VariantTypePrefix, tag, value, VariantTypeNamespace, false);
		}
	}
}
