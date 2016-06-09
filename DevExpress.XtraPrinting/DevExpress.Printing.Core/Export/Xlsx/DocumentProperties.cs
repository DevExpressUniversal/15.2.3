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
using DevExpress.Office;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
using System.Globalization;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		const string documentApplicationPropertiesContentType = @"application/vnd.openxmlformats-officedocument.extended-properties+xml";
		const string documentCorePropertiesContentType = @"application/vnd.openxmlformats-package.core-properties+xml";
		const string extendedPropertiesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
		const string corePropertiesNamespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
		const string customPropertiesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
		const string corePropertiesPrefix = "cp";
		const string dcPropertiesPrefix = "dc";
		const string dcPropertiesNamespace = "http://purl.org/dc/elements/1.1/";
		const string dcTermsPropertiesPrefix = "dcterms";
		const string dcTermsPropertiesNamespace = "http://purl.org/dc/terms/";
		const string xsiPrefix = "xsi";
		const string xsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";
		const string documentCustomPropertiesContentType = @"application/vnd.openxmlformats-officedocument.custom-properties+xml";
		const string variantTypePrefix = "vt";
		const string variantTypeNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";
		#region Application properties
		void AddDocumentApplicationPropertiesContent() {
			if(!ShouldExportDocumentApplicationProperties())
				return;
			BeginWriteXmlContent();
			AddDocumentApplicationPropertiesContentCore();
			AddPackageContent(@"docProps\app.xml", EndWriteXmlContent());
		}
		bool ShouldExportDocumentApplicationProperties() {
			return !String.IsNullOrEmpty(documentProperties.Application) ||
				!String.IsNullOrEmpty(documentProperties.Company) ||
				!String.IsNullOrEmpty(documentProperties.Manager) ||
				!String.IsNullOrEmpty(documentProperties.Version) ||
				documentProperties.Security != XlDocumentSecurity.None;
		}
		void AddDocumentApplicationPropertiesContentCore() {
			WriteStartElement("Properties", extendedPropertiesNamespace);
			try {
				if(!String.IsNullOrEmpty(documentProperties.Application))
					WriteString("Application", extendedPropertiesNamespace, EncodeXmlChars(documentProperties.Application));
				if(documentProperties.Security != XlDocumentSecurity.None)
					WriteString("DocSecurity", extendedPropertiesNamespace, ((int)documentProperties.Security).ToString());
				if(!String.IsNullOrEmpty(documentProperties.Manager))
					WriteString("Manager", extendedPropertiesNamespace, EncodeXmlChars(documentProperties.Manager));
				if(!String.IsNullOrEmpty(documentProperties.Company))
					WriteString("Company", extendedPropertiesNamespace, EncodeXmlChars(documentProperties.Company));
				if(!String.IsNullOrEmpty(documentProperties.Version))
					WriteString("AppVersion", extendedPropertiesNamespace, EncodeXmlChars(documentProperties.Version));
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region Core properties
		void AddDocumentCorePropertiesContent() {
			if(!ShouldExportDocumentCoreProperties())
				return;
			BeginWriteXmlContent();
			AddDocumentCorePropertiesContentCore();
			AddPackageContent(@"docProps\core.xml", EndWriteXmlContent());
		}
		bool ShouldExportDocumentCoreProperties() {
			return true;
		}
		void AddDocumentCorePropertiesContentCore() {
			WriteStartElement(corePropertiesPrefix, "coreProperties", corePropertiesNamespace);
			try {
				WriteStringAttr("xmlns", dcPropertiesPrefix, null, dcPropertiesNamespace);
				WriteStringAttr("xmlns", dcTermsPropertiesPrefix, null, dcTermsPropertiesNamespace);
				WriteStringAttr("xmlns", xsiPrefix, null, xsiNamespace);
				if(!String.IsNullOrEmpty(documentProperties.Title))
					WriteString(dcPropertiesPrefix, "title", EncodeXmlChars(documentProperties.Title), dcPropertiesNamespace, false);
				if(!String.IsNullOrEmpty(documentProperties.Subject))
					WriteString(dcPropertiesPrefix, "subject", EncodeXmlChars(documentProperties.Subject), dcPropertiesNamespace, false);
				if(!String.IsNullOrEmpty(documentProperties.Author))
					WriteString(dcPropertiesPrefix, "creator", EncodeXmlChars(documentProperties.Author), dcPropertiesNamespace, false);
				if(!String.IsNullOrEmpty(documentProperties.Keywords))
					WriteString(corePropertiesPrefix, "keywords", EncodeXmlChars(documentProperties.Keywords), corePropertiesNamespace, false);
				if(!String.IsNullOrEmpty(documentProperties.Description))
					WriteString(dcPropertiesPrefix, "description", EncodeXmlChars(documentProperties.Description), dcPropertiesNamespace, false);
				if(!String.IsNullOrEmpty(documentProperties.Author))
					WriteString(corePropertiesPrefix, "lastModifiedBy", EncodeXmlChars(documentProperties.Author), corePropertiesNamespace, false);
				if(documentProperties.Created != DateTime.MinValue) {
					WriteDcTermsDateTime("created", documentProperties.Created);
					WriteDcTermsDateTime("modified", documentProperties.Created);
				}
				if(!String.IsNullOrEmpty(documentProperties.Category))
					WriteString(corePropertiesPrefix, "category", EncodeXmlChars(documentProperties.Category), corePropertiesNamespace, false);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteDcTermsDateTime(string tag, DateTime dateTime) {
			WriteStartElement(dcTermsPropertiesPrefix, tag, dcTermsPropertiesNamespace);
			try {
				WriteStringAttr(xsiPrefix, "type", null, dcTermsPropertiesPrefix + ":W3CDTF");
				writer.WriteString(DateTimeToStringUniversal(dateTime));
			}
			finally {
				WriteEndElement();
			}
		}
		string DateTimeToStringUniversal(DateTime dateTime) {
			return DateTimeToString(dateTime.ToUniversalTime());
		}
		string DateTimeToString(DateTime dateTime) {
			return dateTime.ToString(CurrentCulture.DateTimeFormat.SortableDateTimePattern) + 'Z';
		}
		#endregion
		#region Custom properties
		void AddDocumentCustomPropertiesContent() {
			if(!ShouldExportDocumentCustomProperties())
				return;
			BeginWriteXmlContent();
			AddDocumentCustomPropertiesContentCore();
			AddPackageContent(@"docProps\custom.xml", EndWriteXmlContent());
		}
		bool ShouldExportDocumentCustomProperties() {
			return documentProperties.Custom.Count > 0;
		}
		void AddDocumentCustomPropertiesContentCore() {
			WriteStartElement("Properties", customPropertiesNamespace);
			try {
				WriteStringAttr("xmlns", variantTypePrefix, null, variantTypeNamespace);
				int index = 2;
				XlDocumentCustomProperties properties = documentProperties.Custom;
				foreach(string name in properties.Names) {
					WriteDocumentCustomProperty(name, properties[name], index);
					index++;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteDocumentCustomProperty(string name, XlCustomPropertyValue propertyValue, int index) {
			if(propertyValue.Type == XlVariantValueType.None)
				return;
			WriteStartElement("property", customPropertiesNamespace);
			try {
				WriteStringValue("fmtid", "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}");
				WriteIntValue("pid", index);
				WriteStringValue("name", name);
				switch(propertyValue.Type) {
					case XlVariantValueType.Numeric:
						if(propertyValue.NumericValue == (double)(int)propertyValue.NumericValue)
							WriteVariantValue("i4", ((int)propertyValue.NumericValue).ToString());
						else
							WriteVariantValue("r8", propertyValue.NumericValue.ToString(CultureInfo.InvariantCulture));
						break;
					case XlVariantValueType.Boolean:
						WriteVariantValue("bool", propertyValue.BooleanValue ? "true" : "false");
						break;
					case XlVariantValueType.DateTime:
						WriteVariantValue("filetime", propertyValue.DateTimeValue.ToUniversalTime().ToString(CurrentCulture.DateTimeFormat.SortableDateTimePattern) + 'Z');
						break;
					case XlVariantValueType.Text:
						WriteVariantValue("lpwstr", propertyValue.TextValue);
						break;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteVariantValue(string tag, string value) {
			WriteString(variantTypePrefix, tag, value, variantTypeNamespace, false);
		}
		#endregion
	}
}
