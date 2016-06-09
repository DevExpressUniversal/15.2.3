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

using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.SpreadsheetSource.Implementation;
using System;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region DefinedNamesDestination
	public class DefinedNamesDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("definedName", OnDefinedName);
			return result;
		}
		#endregion
		public DefinedNamesDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDefinedName(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new DefinedNameDestination(importer);
		}
	}
	#endregion
	#region DefinedNameDestination
	public class DefinedNameDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Fields
		string name;
		string scope;
		bool isHidden;
		string comment;
		#endregion
		public DefinedNameDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			name = Importer.ReadAttribute(reader, "name");
			if (String.IsNullOrEmpty(name))
				return;
			int sheetId = Importer.GetIntegerValue(reader, "localSheetId", Int32.MinValue);
			scope = sheetId == Int32.MinValue || sheetId >= Importer.Source.Worksheets.Count ? String.Empty : Importer.Source.Worksheets[sheetId].Name;
			isHidden = Importer.GetWpSTOnOffValue(reader, "hidden", false);
			comment = Importer.DecodeXmlChars(Importer.ReadAttribute(reader, "comment"));
		}
		public override bool ProcessText(XmlReader reader) {
			if (String.IsNullOrEmpty(name))
				return true;
			XlCellRange range = XlRangeReferenceParser.Parse(reader.Value, false);
			if (range == null)
				return true;
			DefinedName definedName = new DefinedName(name, scope, isHidden, range, range.ToString());
			if (!String.IsNullOrEmpty(comment))
				definedName.Comment = comment;
			Importer.Source.InnerDefinedNames.Add(definedName);
			return true;
		}
	}
	#endregion
}
