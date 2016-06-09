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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ColorsDestination
	public class ColorsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("indexedColors", OnIndexedColors);
			result.Add("mruColors", OnCustomColors);
			return result;
		}
		static Destination OnIndexedColors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IndexedColorsDestination(importer);
		}
		static Destination OnCustomColors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CustomColorsDestination(importer);
		}
		#endregion
		public ColorsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region IndexedColorsDestination
	public class IndexedColorsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rgbColor", OnRgbColor);
			return result;
		}
		static IndexedColorsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (IndexedColorsDestination)importer.PeekDestination();
		}
		static Destination OnRgbColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Destination result = new RgbColorDestination(importer, GetThis(importer).ColorIndex);
			GetThis(importer).ColorIndex++;
			return result;
		}
		#endregion
		int colorIndex;
		public IndexedColorsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
		#endregion
	}
	#endregion
	#region RgbColorDestination
	public class RgbColorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly int colorIndex;
		#endregion
		public RgbColorDestination(SpreadsheetMLBaseImporter importer, int colorIndex)
			: base(importer) {
			this.colorIndex = colorIndex;
		}
		#region Properties
		protected internal int ColorIndex { get { return colorIndex; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.StyleSheet.Palette[ColorIndex] = Importer.GetWpSTColorValue(reader, "rgb");
		}
	}
	#endregion
	#region CustomColorsDestination
	public class CustomColorsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("color", OnColor);
			return result;
		}
		static Destination OnColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CustomColorDestination(importer);
		}
		#endregion
		public CustomColorsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region CustomColorDestination
	public class CustomColorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public CustomColorDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color customColor = Importer.GetWpSTColorValue(reader, "rgb");
			List<Color> customColors = Importer.DocumentModel.StyleSheet.CustomColors;
			if (!customColors.Contains(customColor))
				customColors.Add(customColor);
		}
	}
	#endregion
}
