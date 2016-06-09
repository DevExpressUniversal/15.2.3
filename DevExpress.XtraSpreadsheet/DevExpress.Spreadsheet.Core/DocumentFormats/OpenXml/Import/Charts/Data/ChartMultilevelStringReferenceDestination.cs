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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartMultilevelStringReferenceDestination
	public class ChartMultilevelStringReferenceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("multiLvlStrCache", OnStringCache);
			return result;
		}
		static ChartMultilevelStringReferenceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartMultilevelStringReferenceDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlChartMLDataReference reference;
		#endregion
		public ChartMultilevelStringReferenceDestination(SpreadsheetMLBaseImporter importer, OpenXmlChartMLDataReference reference)
			: base(importer) {
			this.reference = reference;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OpenXmlChartMLDataReference reference = GetThis(importer).reference;
			return new StringValueTagDestination(importer, delegate(string value) { reference.FormulaBody = value; return true; });
		}
		static Destination OnStringCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartMultilevelStringReferenceDestination thisDestination = GetThis(importer);
			return new ChartMultilevelStringCacheDestination(importer, thisDestination.reference);
		}
		#endregion
	}
	#endregion
	#region ChartMultilevelStringCacheDestination
	public class ChartMultilevelStringCacheDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("lvl", OnLevel);
			return result;
		}
		static ChartMultilevelStringCacheDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartMultilevelStringCacheDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OpenXmlChartMLDataReference reference;
		int levelCounter;
		#endregion
		public ChartMultilevelStringCacheDestination(SpreadsheetMLBaseImporter importer, OpenXmlChartMLDataReference reference)
			: base(importer) {
			this.reference = reference;
			this.levelCounter = 0;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnLevel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartMultilevelStringCacheDestination thisDestination = GetThis(importer);
			OpenXmlChartMLReferenceLevel levelPoints = new OpenXmlChartMLReferenceLevel();
			thisDestination.reference.Levels.Add(levelPoints);
			thisDestination.levelCounter++;
			return new ChartMultilevelStringCacheLevelDestination(importer, levelPoints.Points);
		}
		#endregion
	}
	#endregion
	#region ChartMultilevelStringCacheLevelDestination
	public class ChartMultilevelStringCacheLevelDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		static ChartMultilevelStringCacheLevelDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartMultilevelStringCacheLevelDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		List<StringPoint> points;
		#endregion
		public ChartMultilevelStringCacheLevelDestination(SpreadsheetMLBaseImporter importer, List<StringPoint> points)
			: base(importer) {
			this.points = points;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StringPoint point = new StringPoint();
			GetThis(importer).points.Add(point);
			return new ChartStringPointDestination(importer, point);
		}
		#endregion
	}
	#endregion
}
