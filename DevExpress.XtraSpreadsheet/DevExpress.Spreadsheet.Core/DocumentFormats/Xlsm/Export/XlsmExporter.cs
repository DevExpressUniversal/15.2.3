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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Export;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xlsm {
	#region XlsmExporter
	public class XlsmExporter : OpenXmlExporter {
		public XlsmExporter(DocumentModel workbook, XlsmDocumentExporterOptions options) 
			: base(workbook, options) {
		}
		protected internal override string GetMainDocumentContentType() {
			return "application/vnd.ms-excel.sheet.macroEnabled.main+xml";
		}
		protected internal override bool ShouldExportVbaProject() {
			return !Workbook.VbaProjectContent.IsEmpty;
		}
		protected internal override Stream ExportVbaProject() {
			List<VbaProjectEntry> vbaProjectEntries = new List<VbaProjectEntry>();
			foreach(VbaProjectItem item in Workbook.VbaProjectContent.Items)
				VbaProjectExportHelper.ExportVbaProjectItem(vbaProjectEntries, item.StreamName, item.Data);
			using(MemoryStream ms = new MemoryStream()) {
				StructuredStorageWriter structuredStorageWriter = new StructuredStorageWriter();
				foreach(VbaProjectEntry entry in vbaProjectEntries)
					StructuredStorageHelper.AddStreamDirectoryEntry(structuredStorageWriter, entry.StreamName, entry.Writer);
				structuredStorageWriter.Write(ms);
				return new MemoryStream(ms.GetBuffer(), 0, (int)ms.Length);
			}
		}
		protected internal override bool ShouldExportDefinedName(DefinedName definedName) {
			return true;
		}
	}
	#endregion
	#region XltmExporter
	public class XltmExporter : XlsmExporter {
		public XltmExporter(DocumentModel workbook, XltmDocumentExporterOptions options)
			: base(workbook, options) {
		}
		protected internal override string GetMainDocumentContentType() {
			return "application/vnd.ms-excel.template.macroEnabled.main+xml";
		}
	}
	#endregion
}
