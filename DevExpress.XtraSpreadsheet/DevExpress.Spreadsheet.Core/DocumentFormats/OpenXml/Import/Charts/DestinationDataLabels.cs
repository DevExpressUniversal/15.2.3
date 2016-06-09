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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DataLabelDestinationBase
	public abstract class DataLabelDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, DataLabelPosition> dataLabelPositionTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.DataLabelPositionTable);
		internal static Dictionary<string, BarShape> BarChartShapeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.BarChartShapeTable);
		#endregion
		#region Handler Table
		static protected void AddDataLabelBaseHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("delete", OnDelete);
			table.Add("dLblPos", OnDataLabelPosition);
			table.Add("numFmt", OnNumberFormat);
			table.Add("separator", OnSeparator);
			table.Add("showBubbleSize", OnShowBubbleSize);
			table.Add("showCatName", OnShowCatName);
			table.Add("showLegendKey", OnShowLegendKey);
			table.Add("showPercent", OnShowPercent);
			table.Add("showSerName", OnShowSerName);
			table.Add("showVal", OnShowValue);
			table.Add("txPr", OnTextProperties);
			table.Add("spPr", OnShapeProperties);
		}
		static DataLabelDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (DataLabelDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		protected abstract DataLabelBase DataLabel { get; }
		#endregion
		protected DataLabelDestinationBase(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnDelete(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.Delete = value; },
				"val", true);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).DataLabel.ShapeProperties);
		}
		static Destination OnDataLabelPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new EnumValueDestination<DataLabelPosition>(importer,
				dataLabelPositionTable,
				delegate(DataLabelPosition value) { dataLabel.LabelPosition = value; },
				"val",
				DataLabelPosition.BestFit);
		}
		static Destination OnNumberFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartNumberFormatDestination(importer, GetThis(importer).DataLabel.NumberFormat);
		}
		static Destination OnSeparator(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
#if !SL
			return new DataLabelSeparatorDestination(importer, dataLabel);
#else
			return new StringValueTagDestination(importer, delegate(string value) { dataLabel.SetSeparatorCore(value); return true; });
#endif
		}
		static Destination OnShowBubbleSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowBubbleSize = value; },
				"val", true);
		}
		static Destination OnShowCatName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowCategoryName = value; },
				"val", true);
		}
		static Destination OnShowLegendKey(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowLegendKey = value; },
				"val", true);
		}
		static Destination OnShowPercent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowPercent = value; },
				"val", true);
		}
		static Destination OnShowSerName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowSeriesName = value; },
				"val", true);
		}
		static Destination OnShowValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabel.ShowValue = value; },
				"val", true);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabelBase dataLabel = GetThis(importer).DataLabel;
			return new TextPropertiesDestination(importer, dataLabel.TextProperties);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			DataLabel.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			DataLabel.EndUpdate();
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	#region DataLabelsDestination
	public class DataLabelsDestination : DataLabelDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dLbl", OnDataLabel);
			result.Add("leaderLines", OnLeaderLines);
			result.Add("showLeaderLines", OnShowLeaderLines);
			AddDataLabelBaseHandlers(result);
			return result;
		}
		static DataLabelsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DataLabelsDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DataLabels dataLabels;
		#endregion
		public DataLabelsDestination(SpreadsheetMLBaseImporter importer, DataLabels dataLabels)
			: base(importer) {
			this.dataLabels = dataLabels;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override DataLabelBase DataLabel { get { return dataLabels; } }
		#endregion
		#region Handlers
		static Destination OnDataLabel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabels dataLabels = GetThis(importer).dataLabels;
			DataLabel label = new DataLabel(dataLabels.Parent, 0);
			dataLabels.Labels.AddWithoutHistoryAndNotifications(label);
			return new DataLabelDestination(importer, label);
		}
		static Destination OnLeaderLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new InnerShapePropertiesDestination(importer, GetThis(importer).dataLabels.LeaderLinesProperties);
		}
		static Destination OnShowLeaderLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabels dataLabels = GetThis(importer).dataLabels;
			return new OnOffValueDestination(importer,
				delegate(bool value) { dataLabels.ShowLeaderLines = value; },
				"val", true);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.dataLabels.Apply = true;
		}
	}
	#endregion
	#region DataLabelDestination
	public class DataLabelDestination : DataLabelDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("idx", OnIndex);
			result.Add("layout", OnLayout);
			result.Add("tx", OnChartText);
			AddDataLabelBaseHandlers(result);
			return result;
		}
		static DataLabelDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DataLabelDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DataLabel label;
		#endregion
		public DataLabelDestination(SpreadsheetMLBaseImporter importer, DataLabel label)
			: base(importer) {
			this.label = label;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override DataLabelBase DataLabel { get { return label; } }
		#endregion
		#region Handlers
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).label.Layout);
		}
		static Destination OnChartText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabel label = GetThis(importer).label;
			return new ChartTextDestination(importer, label.Parent, delegate(IChartText value) { label.SetTextCore(value); });
		}
		static Destination OnIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataLabel label = GetThis(importer).label;
			return new IntegerValueDestination(importer,
				delegate(int value) { label.ItemIndex = value; },
				"val", 1);
		}
		#endregion
	}
	#endregion
	#region DataLabelSeparatorDestination
#if !SL
	public class DataLabelSeparatorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly DataLabelBase dataLabel;
		#endregion
		public DataLabelSeparatorDestination(SpreadsheetMLBaseImporter importer, DataLabelBase dataLabel)
			: base(importer) {
			this.dataLabel = dataLabel;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string content = Importer.InternalReader.ReadString();
			dataLabel.SetSeparatorCore(content);
			Importer.PopDestination();
		}
	}
#endif
	#endregion
}
