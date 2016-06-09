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

using DevExpress.Office;
using DevExpress.SpreadsheetSource.Implementation;
using System;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region WorksheetColumnsDestination
	public class WorksheetColumnsDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("col", OnColumn);
			return result;
		}
		#endregion
		public WorksheetColumnsDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnColumn(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new WorksheetColumnDestination(importer);
		}
	}
	#endregion
	#region WorksheetColumnDestination
	public class WorksheetColumnDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		public WorksheetColumnDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int firstIndex = Importer.GetWpSTIntegerValue(reader, "min", 0);
			if (firstIndex <= 0)
				Importer.ThrowInvalidFile("Invalid first column index");
			int lastIndex = Importer.GetWpSTIntegerValue(reader, "max", 0);
			if (lastIndex <= 0)
				Importer.ThrowInvalidFile("Invalid last column index");
			if (lastIndex < firstIndex)
				Importer.ThrowInvalidFile("Last column index less than first");
			firstIndex--;
			lastIndex--;
			XlsxSourceDataReader dataReader = Importer.Source.DataReader;
			if (!dataReader.CanAddColumn(firstIndex, lastIndex))
				return;
			bool isHidden = Importer.GetWpSTOnOffValue(reader, "hidden", false);
			int formatIndex = Importer.GetWpSTIntegerValue(reader, "style", -1);
			ColumnInfo column = new ColumnInfo(firstIndex, lastIndex, isHidden, formatIndex);
			dataReader.Columns.Add(column);
		}
	}
	#endregion
}
