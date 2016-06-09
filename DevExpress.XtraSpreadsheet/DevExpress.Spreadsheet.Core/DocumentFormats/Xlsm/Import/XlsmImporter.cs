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
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using System.Globalization;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.Xlsm {
	public class XlsmImporter : OpenXmlImporter {
		public XlsmImporter(IDocumentModel documentModel, XlsmDocumentImporterOptions options)
			: base(documentModel, options) {
		}
		protected internal override void ImportVbaProject() {
			foreach(OpenXmlRelation relation in DocumentRelations) {
				if(relation.Type == OpenXmlExporter.RelsVbaProjectNamespace) {
					string fileName = CalculateRelationTargetCore(relation, DocumentRootFolder, String.Empty);
					PackageFile packageFile = GetPackageFile(fileName);
					if(packageFile != null)
						ImportVbaProject(packageFile.SeekableStream);
					break;
				}
			}
		}
		void ImportVbaProject(Stream stream) {
			using(PackageFileReader packageFileReader = new PackageFileReader(stream)) {
				IEnumerable<string> vbaFiles = packageFileReader.EnumerateFiles(string.Empty, false);
				foreach(string entry in vbaFiles) {
					BinaryReader reader = packageFileReader.GetCachedPackageFileReader(entry);
					if(reader != null) {
						string name = entry.Substring(1);
						byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
						DocumentModel.VbaProjectContent.Items.Add(new VbaProjectItem(name, data));
					}
					else {
						DocumentModel.VbaProjectContent.Clear();
						return;
					}
				}
			}
		}
	}
}
