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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DefinedNamesDestination
	public class ExternalDefinedNamesDestination : DefinedNamesDestinationBase<ExternalDefinedName> {
		public ExternalDefinedNamesDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workbook)
			: base(importer, workbook) {
		}
		protected internal override Destination GetDefinedNameDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workbook) {
			return new ExternalDefinedNameDestination(importer, workbook);
		}
	}
	#endregion
	public class ExternalDefinedNameDestination : DefinedNameDestinationBase<ExternalDefinedName> {
		public ExternalDefinedNameDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workBook)
			: base(importer, workBook) {
		}
		protected internal override ExternalDefinedName GetDefinedNameInstance(string name, IModelWorkbook workbook, int sheetId) {
			return new ExternalDefinedName(name, (ExternalWorkbook)workbook, string.Empty, sheetId);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = Importer.ReadAttribute(reader, "name");
			if (String.IsNullOrEmpty(name))
				return;
			string refersTo = Importer.ReadAttribute(reader, "refersTo");
			int sheetId = Importer.GetIntegerValue(reader, "sheetId", Int32.MinValue);
			DefinedName = (sheetId == Int32.MinValue)
				? CreateWorkBookScoped(name)
				: CreateSheetScoped(name, sheetId);
			DefinedName.SetReferenceCore(refersTo);
		}
	}
}
