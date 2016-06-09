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

using System.Collections.Generic;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartDataPointDestination
	public class ChartDataPointDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("idx", OnIndex);
			result.Add("invertIfNegative", OnInvertIfNegative);
			result.Add("marker", OnMarker);
			result.Add("bubble3D", OnBubble3d);
			result.Add("explosion", OnExplosion);
			result.Add("spPr", OnShapeProperties);
			result.Add("pictureOptions", OnPictureOptions);
			return result;
		}
		static ChartDataPointDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartDataPointDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DataPoint dataPoint;
		#endregion
		public ChartDataPointDestination(SpreadsheetMLBaseImporter importer, DataPoint dataPoint)
			: base(importer) {
			this.dataPoint = dataPoint;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).dataPoint.ShapeProperties);
		}
		static Destination OnInvertIfNegative(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataPoint.SetInvertIfNegativeCore(value); },
				"val", true);
		}
		static Destination OnIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new IntegerValueDestination(importer,
				delegate(int value) { dataPoint.SetIndexCore(value); },
				"val", 0);
		}
		static Destination OnPictureOptions(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new PictureOptionsDestination(importer, dataPoint.PictureOptions);
		}
		static Destination OnBubble3d(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataPoint.SetBubble3DCore(value); },
				"val", true);
		}
		static Destination OnExplosion(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new IntegerValueDestination(importer,
				delegate(int value) { dataPoint.SetExplosionCore(value); },
				"val", 0);
		}
		static Destination OnMarker(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataPoint dataPoint = GetThis(importer).dataPoint;
			return new ChartMarkerDestination(importer, dataPoint.Marker);
		}
		#endregion
	}
	#endregion
	#region ChartMarkerDestination
	public class ChartMarkerDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, MarkerStyle> markerStyleTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.MarkerStyleTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spPr", OnShapeProperties);
			result.Add("symbol", OnSymbol);
			result.Add("size", OnSize);
			return result;
		}
		static ChartMarkerDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartMarkerDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Marker marker;
		#endregion
		public ChartMarkerDestination(SpreadsheetMLBaseImporter importer, Marker marker)
			: base(importer) {
			this.marker = marker;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).marker.ShapeProperties);
		}
		static Destination OnSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Marker marker = GetThis(importer).marker;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 2 || value > 72)
						importer.ThrowInvalidFile();
					marker.SetSizeCore(value);
				},
				"val", 5);
		}
		static Destination OnSymbol(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Marker marker = GetThis(importer).marker;
			return new EnumValueDestination<MarkerStyle>(importer,
				markerStyleTable,
				delegate(MarkerStyle value) { marker.SetSymbolCore(value); },
				"val",
				MarkerStyle.None);
		}
		#endregion
	}
	#endregion
}
