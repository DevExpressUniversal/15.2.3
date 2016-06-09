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
		const string DocumentApplicationPropertiesContentType = @"application/vnd.openxmlformats-officedocument.extended-properties+xml";
		protected internal virtual void AddDocumentApplicationPropertiesContent() {
			if (ShouldExportDocumentApplicationProperties())
				AddPackageContent(@"docProps/app.xml", ExportDocumentApplicationProperties());
		}
		protected internal virtual bool ShouldExportDocumentApplicationProperties() {
			ModelDocumentApplicationProperties properties = Workbook.DocumentApplicationProperties;
			return !String.IsNullOrEmpty(properties.Application) ||
				!String.IsNullOrEmpty(properties.Company) ||
				!String.IsNullOrEmpty(properties.Manager) ||
				!String.IsNullOrEmpty(properties.Version) ||
				properties.Security != ModelDocumentSecurity.None;
		}
		protected internal virtual CompressedStream ExportDocumentApplicationProperties() {
			return CreateXmlContent(GenerateDocumentApplicationPropertiesXmlContent);
		}
		protected internal virtual void GenerateDocumentApplicationPropertiesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateDocumentApplicationPropertiesContent();
		}
		protected internal virtual void GenerateDocumentApplicationPropertiesContent() {
			WriteStartElement("Properties", ExtendedPropertiesNamespace);
			try {
				ModelDocumentApplicationProperties properties = Workbook.DocumentApplicationProperties;
				if (!String.IsNullOrEmpty(properties.Application))
					WriteString("Application", ExtendedPropertiesNamespace, EncodeXmlChars(properties.Application));
				if (properties.Security != ModelDocumentSecurity.None)
					WriteString("DocSecurity", ExtendedPropertiesNamespace, ((int)properties.Security).ToString());
				if (!String.IsNullOrEmpty(properties.Manager))
					WriteString("Manager", ExtendedPropertiesNamespace, EncodeXmlChars(properties.Manager));
				if (!String.IsNullOrEmpty(properties.Company))
					WriteString("Company", ExtendedPropertiesNamespace, EncodeXmlChars(properties.Company));
				if (!String.IsNullOrEmpty(properties.Version))
					WriteString("AppVersion", ExtendedPropertiesNamespace, EncodeXmlChars(properties.Version));
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
