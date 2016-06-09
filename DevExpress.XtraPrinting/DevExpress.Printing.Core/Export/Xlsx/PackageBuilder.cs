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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraExport.Xlsx {
	#region XlsxPackageBuilder
	public class XlsxPackageBuilder {
		#region Fields
		public const string RelsPrefix = "r";
		public const string RelsContentType = "application/vnd.openxmlformats-package.relationships+xml";
		public const string XmlContentType = "application/xml";
		public const string WorksheetContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";
		public const string WorkbookContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
		public const string SharedStringsContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";
		public const string StylesContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";
		public const string ContentTypesNamespaceConst = "http://schemas.openxmlformats.org/package/2006/content-types";
		public const string SpreadsheetNamespaceConst = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
		public const string RelsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
		public const string RelsWorksheetNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
		public const string OfficeDocumentNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
		public const string PackageRelsNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";
		public const string RelsSharedStringsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";
		public const string RelsStylesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
		readonly Stream outputStream;
		InternalZipArchive documentPackage;
		DateTime now;
		OpenXmlRelationCollection workbookRelations;
		Dictionary<string, string> usedContentTypes;
		Dictionary<string, string> overriddenContentTypes;
		#endregion
		public XlsxPackageBuilder(Stream outputStream) {
			Guard.ArgumentNotNull(outputStream, "outputStream");
			this.outputStream = outputStream;
		}
		protected XlsxPackageBuilder() {
		}
		public InternalZipArchive Package { get { return documentPackage; } }
		public DateTime Now { get { return now; } }
		public OpenXmlRelationCollection WorkbookRelations { get { return workbookRelations; } }
		public Dictionary<string, string> UsedContentTypes { get { return usedContentTypes; } }
		public Dictionary<string, string> OverriddenContentTypes { get { return overriddenContentTypes; } }
		public void BeginExport() {
			this.now = DevExpress.XtraPrinting.Native.DateTimeHelper.Now;
			this.documentPackage = CreateDocumentPackage(outputStream);
			InitializeExport();
		}
		protected virtual InternalZipArchive CreateDocumentPackage(Stream stream) {
			if(stream == null)
				return null;
			return new InternalZipArchive(stream);
		}
		void InitializeExport() {
			this.workbookRelations = new OpenXmlRelationCollection();
			this.usedContentTypes = new Dictionary<string, string>();
			this.overriddenContentTypes = new Dictionary<string, string>();
		}
		public void EndExport() {
			if (documentPackage != null) {
				IDisposable disposable = documentPackage;
				disposable.Dispose();
			}
		}
		#region Content Types
		public void GenerateContentTypesContent(XmlWriter writer) {
			writer.WriteStartElement("Types", ContentTypesNamespaceConst);
			try {
				GenerateUsedContentTypes(writer);
				GenerateOverriddenContentTypes(writer);
			}
			finally {
				writer.WriteEndElement();
			}
		}
		protected internal virtual void GenerateUsedContentTypes(XmlWriter writer) {
			foreach (string extension in UsedContentTypes.Keys) {
				string contentType = UsedContentTypes[extension];
				writer.WriteStartElement("Default");
				try {
					writer.WriteAttributeString("Extension", extension);
					writer.WriteAttributeString("ContentType", contentType);
				}
				finally {
					writer.WriteEndElement();
				}
			}
		}
		protected internal virtual void GenerateOverriddenContentTypes(XmlWriter writer) {
			foreach (string partName in OverriddenContentTypes.Keys) {
				writer.WriteStartElement("Override");
				try {
					writer.WriteAttributeString("PartName", partName);
					writer.WriteAttributeString("ContentType", OverriddenContentTypes[partName]);
				}
				finally {
					writer.WriteEndElement();
				}
			}
		}
		#endregion
		#region Relations
		public virtual void GenerateRelationsContent(XmlWriter writer, OpenXmlRelationCollection relations) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			try {
				GenerateRelationsContentCore(writer, relations);
			}
			finally {
				writer.WriteEndElement();
			}
		}
		public virtual void GenerateRelationsContent(XmlWriter writer, OpenXmlRelation relation) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			try {
				WriteRelationshipTag(writer, relation);
			}
			finally {
				writer.WriteEndElement();
			}
		}
		protected internal void GenerateRelationsContentCore(XmlWriter writer, OpenXmlRelationCollection relations) {
			foreach (OpenXmlRelation relation in relations) {
				WriteRelationshipTag(writer, relation);
			}
		}
		void WriteRelationshipTag(XmlWriter writer, OpenXmlRelation relation) {
			writer.WriteStartElement("Relationship");
			try {
				writer.WriteAttributeString("Id", relation.Id);
				writer.WriteAttributeString("Type", relation.Type);
				writer.WriteAttributeString("Target", DevExpress.XtraPrinting.HtmlExport.Native.DXHttpUtility.UrlEncodeSpaces(relation.Target));
				if (!String.IsNullOrEmpty(relation.TargetMode))
					writer.WriteAttributeString("TargetMode", relation.TargetMode);
			}
			finally {
				writer.WriteEndElement();
			}
		}
		#endregion
	}
	#endregion
}
