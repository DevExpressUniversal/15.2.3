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
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		const string DocumentCorePropertiesContentType = @"application/vnd.openxmlformats-package.core-properties+xml";
		protected internal virtual void AddDocumentCorePropertiesContent() {
			if (ShouldExportDocumentCoreProperties())
				AddPackageContent(@"docProps/core.xml", ExportDocumentCoreProperties());
		}
		protected internal virtual bool ShouldExportDocumentCoreProperties() {
			return true;
		}
		protected internal virtual CompressedStream ExportDocumentCoreProperties() {
			return CreateXmlContent(GenerateDocumentCorePropertiesXmlContent);
		}
		protected internal virtual void GenerateDocumentCorePropertiesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateDocumentCorePropertiesContent();
		}
		protected internal virtual void GenerateDocumentCorePropertiesContent() {
			WriteStartElement(CorePropertiesPrefix, "coreProperties", CorePropertiesNamespace);
			try {
				WriteStringAttr("xmlns", DcPropertiesPrefix, null, DcPropertiesNamespace);
				WriteStringAttr("xmlns", DcTermsPropertiesPrefix, null, DcTermsPropertiesNamespace);
				WriteStringAttr("xmlns", XsiPrefix, null, XsiNamespace);
				ModelDocumentCoreProperties properties = Workbook.DocumentCoreProperties;
				if (!String.IsNullOrEmpty(properties.Title))
					WriteString(DcPropertiesPrefix, "title", EncodeXmlChars(properties.Title), DcPropertiesNamespace, false);
				if (!String.IsNullOrEmpty(properties.Subject))
					WriteString(DcPropertiesPrefix, "subject", EncodeXmlChars(properties.Subject), DcPropertiesNamespace, false);
				if (!String.IsNullOrEmpty(properties.Creator))
					WriteString(DcPropertiesPrefix, "creator", EncodeXmlChars(properties.Creator), DcPropertiesNamespace, false);
				if (!String.IsNullOrEmpty(properties.Keywords))
					WriteString(CorePropertiesPrefix, "keywords", EncodeXmlChars(properties.Keywords), CorePropertiesNamespace, false);
				if (!String.IsNullOrEmpty(properties.Description))
					WriteString(DcPropertiesPrefix, "description", EncodeXmlChars(properties.Description), DcPropertiesNamespace, false);
				if (!String.IsNullOrEmpty(properties.LastModifiedBy))
					WriteString(CorePropertiesPrefix, "lastModifiedBy", EncodeXmlChars(properties.LastModifiedBy), CorePropertiesNamespace, false);
				if (properties.LastPrinted != DateTime.MinValue)
					WriteString(CorePropertiesPrefix, "lastPrinted", DateTimeToStringUniversal(properties.LastPrinted), CorePropertiesNamespace, false);
				if (properties.Created != DateTime.MinValue)
					WriteDcTermsDateTime("created", properties.Created);
				if (properties.Modified != DateTime.MinValue)
					WriteDcTermsDateTime("modified", properties.Modified);
				if (!String.IsNullOrEmpty(properties.Category))
					WriteString(CorePropertiesPrefix, "category", EncodeXmlChars(properties.Category), CorePropertiesNamespace, false);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteDcTermsDateTime(string tag, DateTime dateTime) {
			WriteStartElement(DcTermsPropertiesPrefix, tag, DcTermsPropertiesNamespace);
			try {
				WriteStringAttr(XsiPrefix, "type", null, DcTermsPropertiesPrefix + ":W3CDTF");
				DocumentContentWriter.WriteString(DateTimeToStringUniversal(dateTime));
			}
			finally {
				WriteEndElement();
			}
		}
		string DateTimeToStringUniversal(DateTime dateTime) {
			return DateTimeToString(dateTime.ToUniversalTime());
		}
		string DateTimeToString(DateTime dateTime) {
			return dateTime.ToString(Workbook.Culture.DateTimeFormat.SortableDateTimePattern) + 'Z';
		}
	}
}
