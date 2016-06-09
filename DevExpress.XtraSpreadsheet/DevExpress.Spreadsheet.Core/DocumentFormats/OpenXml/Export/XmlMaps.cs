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
using System.Xml;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void AddXmlMapsPackageContent() {
			if (!ShouldExportXmlMaps())
				return;
			AddPackageContent(@"xl\xmlMaps.xml", ExportXmlMaps());
		}
		protected internal virtual bool ShouldExportXmlMaps() {
			return !String.IsNullOrEmpty(Workbook.XmlMapsContent);
		}
		protected internal virtual CompressedStream ExportXmlMaps() {
			XmlWriterSettings xmlWriterSettings = CreateXmlMapsWriterSettings();
			return CreateXmlContent(GenerateXmlMapsXmlContent, xmlWriterSettings);
		}
		protected internal XmlWriterSettings CreateXmlMapsWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = false;
			settings.Encoding = DXEncoding.UTF8NoByteOrderMarks;
			settings.CheckCharacters = true;
			settings.OmitXmlDeclaration = true;
			return settings;
		}
		protected internal virtual void GenerateXmlMapsXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateXmlMapsContent();
		}
		protected internal virtual void GenerateXmlMapsContent() {
			string content = Workbook.XmlMapsContent;
			DocumentContentWriter.WriteRaw(content);
		}
	}
}
