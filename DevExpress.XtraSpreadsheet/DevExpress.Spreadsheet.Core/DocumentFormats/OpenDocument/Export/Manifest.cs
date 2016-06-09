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

#if OPENDOCUMENT
using DevExpress.Utils.Zip;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenDocument {
	public partial class OpenDocumentExporter {
		protected internal virtual CompressedStream ExportManifest() {
			return CreateXmlContent(GenerateManifestContent);
		}
		protected internal virtual void GenerateManifestContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateManifestContent();
		}
		protected internal virtual void GenerateManifestContent() {
			DocumentContentWriter.WriteStartElement("manifest", "manifest", ManifestNamespace);
			try {
				WriteNs("manifest", ManifestNamespace);
				GenerateManifestElementContent("application/vnd.oasis.opendocument.spreadsheet", "/");
				GenerateManifestElementContent("text/xml", "styles.xml");
				GenerateManifestElementContent("text/xml", "content.xml");
				GenerateManifestElementContent("text/xml", "meta.xml");
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateManifestElementContent(string mediaType, string fullPath) {
			WriteStartElement("file-entry", ManifestNamespace);
			try {
				WriteStringAttr("media-type", ManifestNamespace, mediaType);
				WriteStringAttr("full-path", ManifestNamespace, fullPath);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
#endif
