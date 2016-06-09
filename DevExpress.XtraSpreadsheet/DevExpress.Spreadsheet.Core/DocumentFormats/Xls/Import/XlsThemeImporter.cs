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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using System.Globalization;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.Office.Services;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsThemeImporter : OpenXmlImporter {
		const string rootRelationsPath = "_rels/.rels";
		const string themeManagerFileName = "themeManager.xml";
		public XlsThemeImporter(IDocumentModel documentModel)
			: base(documentModel, new OpenXmlDocumentImporterOptions()) {
		}
		public override void Import(Stream stream) {
			try {
				if (stream != null)
					stream.Seek(0, SeekOrigin.Begin);
				CreateObjects();
				OpenPackage(stream);
				ImportTheme();
			} catch { }
		}
		void CreateObjects() {
			CreateDocumentRelationsStack();
			CreatePackageFileStreams();
			CreatePackageImages();
		}
		protected internal override void ImportTheme() {
			OpenXmlRelationCollection themeRelations = GetThemeRelations();
			DocumentRelationsStack.Push(themeRelations);
			base.ImportTheme();
		}
		OpenXmlRelationCollection GetThemeRelations() {
			OpenXmlRelationCollection rootRelations = ImportRelations(rootRelationsPath);
			string themeManagerPath = LookupRelationTargetByType(rootRelations, OfficeDocumentNamespace, String.Empty, themeManagerFileName);
			DocumentRootFolder = Path.GetDirectoryName(themeManagerPath);
			string themeManagerRelationsPath =  DocumentRootFolder + "/_rels/" + Path.GetFileName(themeManagerPath) + ".rels";
			return ImportRelations(themeManagerRelationsPath);
		}
	}
}
