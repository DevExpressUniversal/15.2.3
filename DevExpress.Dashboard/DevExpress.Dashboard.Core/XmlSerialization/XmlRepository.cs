#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Native {
	public static class XmlRepository {
		static readonly XmlRepository<DataItem> dataItemRepository = new XmlRepository<DataItem>();
		static readonly XmlRepository<IDashboardDataSource> dataSourceRepository = new XmlRepository<IDashboardDataSource>();
		static readonly XmlRepository<DashboardItem> dashboardItemRepository = new XmlRepository<DashboardItem>();
		static readonly XmlRepository<ChartSeries> chartSeriesRepository = new XmlRepository<ChartSeries>();
		static readonly XmlRepository<GridColumnBase> gridColumnRepository = new XmlRepository<GridColumnBase>();
		static readonly XmlRepository<KpiElement> kpiElementRepository = new XmlRepository<KpiElement>();
#if !DXPORTABLE
		static readonly XmlRepository<IDataProvider> dataProviderRepository = new XmlRepository<IDataProvider>();
#endif
		static readonly XmlRepository<DataConnectionBase> dataConnectionRepository = new XmlRepository<DataConnectionBase>();
		static readonly XmlRepository<ChoroplethMap> choroplethMapRepository = new XmlRepository<ChoroplethMap>();
		static readonly XmlRepository<MapPalette> mapPaletteRepository = new XmlRepository<MapPalette>();
		static readonly XmlRepository<MapScale> mapScaleRepository = new XmlRepository<MapScale>();
		static readonly XmlRepository<IXmlSerializableElement> conditionalFormattingRepository = new XmlRepository<IXmlSerializableElement>();
		public static XmlRepository<DataItem> DataItemRepository { get { return dataItemRepository; } }
		public static XmlRepository<IDashboardDataSource> DataSourceRepository { get { return dataSourceRepository; } }
		public static XmlRepository<DashboardItem> DashboardItemRepository { get { return dashboardItemRepository; } }
		public static XmlRepository<ChartSeries> ChartSeriesRepository { get { return chartSeriesRepository; } }
		public static XmlRepository<GridColumnBase> GridColumnRepository { get { return gridColumnRepository; } }
		public static XmlRepository<KpiElement> KpiElementRepository { get { return kpiElementRepository; } }
#if !DXPORTABLE
		public static XmlRepository<IDataProvider> DataProviderRepository { get { return dataProviderRepository; } }
#endif
		public static XmlRepository<DataConnectionBase> DataConnectionRepository { get { return dataConnectionRepository; } }
		public static XmlRepository<ChoroplethMap> ChoroplethMapRepository { get { return choroplethMapRepository; } }
		public static XmlRepository<MapPalette> MapPaletteRepository { get { return mapPaletteRepository; } }
		public static XmlRepository<MapScale> MapScaleRepository { get { return mapScaleRepository; } }
		public static XmlRepository<IXmlSerializableElement> ConditionalFormattingRepository { get { return conditionalFormattingRepository; } }
		static XmlRepository() {
			DataItemRepository.RegisterSerializer<DataItemXmlSerializer<Dimension>>("Dimension");
			DataItemRepository.RegisterSerializer<DataItemXmlSerializer<Measure>>("Measure");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<PivotDashboardItem>>("Pivot");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<GridDashboardItem>>("Grid");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ChartDashboardItem>>("Chart");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ScatterChartDashboardItem>>("ScatterChart");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<PieDashboardItem>>("Pie");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<GaugeDashboardItem>>("Gauge");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<CardDashboardItem>>("Card");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ImageDashboardItem>>("Image");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<TextBoxDashboardItem>>("TextBox");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<RangeFilterDashboardItem>>("RangeFilter");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ChoroplethMapDashboardItem>>("ChoroplethMap");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<GeoPointMapDashboardItem>>("GeoPointMap");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<BubbleMapDashboardItem>>("BubbleMap");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<PieMapDashboardItem>>("PieMap");
			ChartSeriesRepository.RegisterSerializer<ChartSeriesXmlSerializer<SimpleSeries>>("Simple");
			ChartSeriesRepository.RegisterSerializer<ChartSeriesXmlSerializer<RangeSeries>>("Range");
			ChartSeriesRepository.RegisterSerializer<ChartSeriesXmlSerializer<WeightedSeries>>("Weighted");
			ChartSeriesRepository.RegisterSerializer<ChartSeriesXmlSerializer<HighLowCloseSeries>>("HighLowClose");
			ChartSeriesRepository.RegisterSerializer<ChartSeriesXmlSerializer<OpenHighLowCloseSeries>>("OpenHighLowClose");
			GridColumnRepository.RegisterSerializer<GridColumnXmlSerializer<GridDimensionColumn>>("GridDimensionColumn");
			GridColumnRepository.RegisterSerializer<GridColumnXmlSerializer<GridMeasureColumn>>("GridMeasureColumn");
			GridColumnRepository.RegisterSerializer<GridColumnXmlSerializer<GridDeltaColumn>>("GridDeltaColumn");
			GridColumnRepository.RegisterSerializer<GridColumnXmlSerializer<GridSparklineColumn>>("GridSparklineColumn");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ComboBoxDashboardItem>>("ComboBox");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<ListBoxDashboardItem>>("ListBox");
			DashboardItemRepository.RegisterSerializer<DashboardItemXmlSerializer<TreeViewDashboardItem>>("TreeView");
			KpiElementRepository.RegisterSerializer<KpiElementXmlSerializer<Card>>("Card");
			KpiElementRepository.RegisterSerializer<KpiElementXmlSerializer<Gauge>>("GaugeElement");
#if !DXPORTABLE
#pragma warning disable 612, 618
			DataProviderRepository.RegisterSerializer<DataProviderXmlSerializer<SqlDataProvider>>("DataProvider");
			DataProviderRepository.RegisterSerializer<DataProviderXmlSerializer<OlapDataProvider>>("OlapDataProvider");
#pragma warning restore 612, 618
#endif
#if DEBUGTEST && !DXPORTABLE
			DataProviderRepository.RegisterSerializer<DataProviderXmlSerializer<DevExpress.DashboardCommon.Tests.TestDataProvider>>("TestDataProvider");
#endif
			DataConnectionRepository.RegisterSerializer<DataConnectionXmlSerializer<DataConnection>>("DataConnection");
#if !DXPORTABLE
			DataConnectionRepository.RegisterSerializer<DataConnectionXmlSerializer<OlapDataConnection>>("OlapDataConnection");
#endif
			ChoroplethMapRepository.RegisterSerializer<ChoroplethMapLayerXmlSerializer<ValueMap>>("ValueMap");
			ChoroplethMapRepository.RegisterSerializer<ChoroplethMapLayerXmlSerializer<DeltaMap>>("DeltaMap");
			MapPaletteRepository.RegisterSerializer<MapPaletteXmlSerializer<GradientPalette>>("GradientPalette");
			MapPaletteRepository.RegisterSerializer<MapPaletteXmlSerializer<CustomPalette>>("CustomPalette");
			MapScaleRepository.RegisterSerializer<MapScaleXmlSerializer<UniformScale>>("UniformScale");
			MapScaleRepository.RegisterSerializer<MapScaleXmlSerializer<CustomScale>>("CustomScale");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<GridItemFormatRule>>("GridItemFormatRule");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<PivotItemFormatRule>>("PivotItemFormatRule");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<PivotItemFormatRuleLevel>>("PivotItemFormatRuleLevel");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionValue>>("FormatConditionValue");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionAverage>>("FormatConditionAverage");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionExpression>>("FormatConditionExpression");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionTopBottom>>("FormatConditionTopBottom");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionDateOccuring>>("FormatConditionDateOccuring");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionRangeSet>>("FormatConditionRangeSet");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionRangeGradient>>("FormatConditionRangeGradient");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionBar>>("FormatConditionBar");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionColorRangeBar>>("FormatConditionColorRangeBar");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<FormatConditionGradientRangeBar>>("FormatConditionGradientRangeBar");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<AppearanceSettings>>("AppearanceSettings");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<IconSettings>>("IconSettings");
			ConditionalFormattingRepository.RegisterSerializer<ConditionalFormattingXmlSerializer<BarStyleSettings>>("BarStyleSettings");
#if !DXPORTABLE
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DashboardOlapDataSource>>("OLAPDataSource");
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DashboardEFDataSource>>("EFDataSource");
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DashboardObjectDataSource>>("ObjectDataSource");
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DashboardExcelDataSource>>("ExcelDataSource");
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<ExtractFileDataSource>>("ExtractFileDataSource");
#endif
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DashboardSqlDataSource>>("SqlDataSource");
#if !DXPORTABLE
#pragma warning disable 0618
			DataSourceRepository.RegisterSerializer<DataSourceXmlSerializer<DataSource>>("DataSource");
#pragma warning restore 0618
#endif
		}
	}
	public class XmlRepository<TObject> {
		readonly List<XmlSerializer<TObject>> serializers = new List<XmlSerializer<TObject>>();
		XmlSerializer<TObject> GetSerializer(Type type) {
			return serializers.Find(serializer => serializer.GetType() == type);
		}
		void CheckSerializer(string name, Type type) {
			if(GetSerializer(name) != null)
				throw new ArgumentException(String.Format("Repository already contains the '{0}' name", name));
			if(GetSerializer(type) != null)
				throw new ArgumentException(String.Format("Repository already contains the '{0}' type", type.Name));
		}
		void AddSerializer(XmlSerializer<TObject> serializer) {
			serializers.Add(serializer);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public void RegisterSerializer<TSerializer>(string name) where TSerializer : XmlSerializer<TObject> {
			CheckSerializer(name, typeof(TSerializer));
			XmlSerializer<TObject> serializer = (XmlSerializer<TObject>)Activator.CreateInstance(typeof(TSerializer), name);
			AddSerializer(serializer);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
		public void UnregisterSerializer<TSerializer>() where TSerializer : XmlSerializer<TObject> {
			Type type = typeof(TSerializer);
			XmlSerializer<TObject> serializer = GetSerializer(type);
			if(serializer != null)
				serializers.Remove(serializer);
			else
				throw new ArgumentException(String.Format("Repository doesn't contain the '{0}' type", type.Name));
		}
		public XmlSerializer<TObject> GetSerializer(string name) {
			return serializers.Find(serializer => serializer.Name == name);
		}
		public XmlSerializer<TObject> GetSerializer(TObject obj) {
			foreach(XmlSerializer<TObject> serializer in serializers)
				if(serializer.Check(obj))
					return serializer;
			return null;
		}
	}
}
