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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextTabStopListDestination
	public class DrawingTextTabStopListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tab", OnTabStop);
			return result;
		}
		static DrawingTextTabStopListDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextTabStopListDestination)importer.PeekDestination();
		}
		static Destination OnTabStop(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextTabStopDestination(importer, GetThis(importer).collection);
		}
		#endregion
		readonly DrawingTextTabStopCollection collection;
		public DrawingTextTabStopListDestination(SpreadsheetMLBaseImporter importer, DrawingTextTabStopCollection collection)
			: base(importer) {
			this.collection = collection;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DrawingTextTabStopCollection Collection { get { return collection; } }
	}
	#endregion
	#region DrawingTextTabStopDestination
	public class DrawingTextTabStopDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextTabStopCollection collection;
		public DrawingTextTabStopDestination(SpreadsheetMLBaseImporter importer, DrawingTextTabStopCollection collection)
			: base(importer) {
			this.collection = collection;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			collection.Add(CreateDrawingTextTabStop(reader));
		}
		DrawingTextTabStop CreateDrawingTextTabStop(XmlReader reader) {
			int position = Importer.GetIntegerValue(reader, "pos", Int32.MinValue);
			DrawingTextTabAlignmentType? type = Importer.GetWpEnumOnOffNullValue(reader, "algn", OpenXmlExporter.DrawingTextTabAlignmentTypeTable);
			if (type.HasValue && position != Int32.MinValue)
				return new DrawingTextTabStop(type.Value, position);
			if (type.HasValue && position == Int32.MinValue)
				return new DrawingTextTabStop(type.Value);
			return new DrawingTextTabStop();
		}
	}
	#endregion
}
