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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.Utils;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.Map.Design {
	internal class RegisterHelper {
		public static readonly Type[] InformationDataProviderTypes = new Type[] {
			typeof(BingGeocodeDataProvider), 
			typeof(BingSearchDataProvider),
			typeof(BingRouteDataProvider)
		};
		public static readonly Type[] MapDataProviderTypes = new Type[] {
			typeof(BingMapDataProvider), 
			typeof(OpenStreetMapDataProvider)
		};
		public static readonly Type[] MapColorizerTypes = new Type[] {
			typeof(ChoroplethColorizer),
			typeof(GraphColorizer),
			typeof(KeyColorColorizer)
		};
		public static readonly Type[] ChoroplethColorizerValueProviderTypes = new Type[] {
			typeof(ShapeAttributeValueProvider),
			typeof(MapBubbleValueProvider)
		};
		public static readonly Type[] KeyValueColorizerValueProviderTypes = new Type[] {
			typeof(IdItemKeyProvider),
			typeof(AttributeItemKeyProvider)
		};
		public static readonly Type[] LegendTypes = new Type[] {
			typeof(ColorScaleLegend),
			typeof(ColorListLegend),
			typeof(SizeLegend)
		};
		public static readonly Type[] CoordinateSystemTypes = new Type[] {
			typeof(GeoMapCoordinateSystem),
			typeof(CartesianMapCoordinateSystem)
		};
		public static readonly Type[] SourceCoordinateSystemTypes = new Type[] {
			typeof(CartesianSourceCoordinateSystem),
			typeof(GeoSourceCoordinateSystem)
		};
		public static readonly Type[] RangeDistributionTypes = new Type[] {
			typeof(LinearRangeDistribution),
			typeof(ExponentialRangeDistribution),
			typeof(LogarithmicRangeDistribution)
		};
		public static readonly Type[] DataAdapterTypes = new Type[] {
			typeof(MapItemStorage),
			typeof(ListSourceDataAdapter),
			typeof(ShapefileDataAdapter),
			typeof(KmlFileDataAdapter),
			typeof(BubbleChartDataAdapter),
			typeof(PieChartDataAdapter)
		};
		public static readonly Type[] PointTypes = new Type[] {
			typeof(CartesianPoint),
			typeof(GeoPoint)
		};
		public static readonly Type[] MapItemSettingsTypes = new Type[] {
			typeof(MapCustomElementSettings),
			typeof(MapPushpinSettings),
			typeof(MapDotSettings),
			typeof(MapEllipseSettings),
			typeof(MapLineSettings),
			typeof(MapPathSettings),
			typeof(MapPolygonSettings),
			typeof(MapPolylineSettings),
			typeof(MapRectangleSettings)
		};
		public static readonly Type[] MiniMapLayerTypes = new Type[] {
			typeof(MiniMapImageTilesLayer), 
			typeof(MiniMapVectorLayer)
		};
		public static void PrepareAttributeTable(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(MapControl), MapControl.LayersProperty.GetName(),
				new NewItemTypesAttribute(typeof(ImageTilesLayer), typeof(VectorLayer), typeof(InformationLayer)));
			builder.AddCustomAttributes(typeof(ImageTilesLayer), ImageTilesLayer.DataProviderProperty.GetName(),
			   new NewItemTypesAttribute(MapDataProviderTypes));
			builder.AddCustomAttributes(typeof(MapItemStorage), MapItemStorage.ItemsProperty.GetName(),
				new NewItemTypesAttribute(typeof(MapCustomElement), typeof(MapLine), typeof(MapPolyline),
					typeof(MapPolygon), typeof(MapRectangle), typeof(MapEllipse), typeof(MapDot), typeof(MapPushpin),
					typeof(MapPath), typeof(MapBubble), typeof(MapPie)));
			builder.AddCustomAttributes(typeof(InformationLayer), InformationLayer.DataProviderProperty.GetName(),
				new NewItemTypesAttribute(InformationDataProviderTypes));
			builder.AddCustomAttributes(typeof(VectorLayer), VectorLayer.ColorizerProperty.GetName(),
				new NewItemTypesAttribute(MapColorizerTypes));
			builder.AddCustomAttributes(typeof(ChoroplethColorizer), ChoroplethColorizer.RangeDistributionProperty.GetName(),
				new NewItemTypesAttribute(RangeDistributionTypes));
			builder.AddCustomAttributes(typeof(ChoroplethColorizer), ChoroplethColorizer.ValueProviderProperty.GetName(),
				new NewItemTypesAttribute(ChoroplethColorizerValueProviderTypes));
			builder.AddCustomAttributes(typeof(KeyColorColorizer), KeyColorColorizer.ItemKeyProviderProperty.GetName(),
				new NewItemTypesAttribute(KeyValueColorizerValueProviderTypes));
			builder.AddCustomAttributes(typeof(VectorLayer), VectorLayer.DataProperty.GetName(),
				new NewItemTypesAttribute(DataAdapterTypes));
			builder.AddCustomAttributes(typeof(MeasureRules), MeasureRules.RangeDistributionProperty.GetName(),
				new NewItemTypesAttribute(RangeDistributionTypes));
			builder.AddCustomAttributes(typeof(MeasureRules), MeasureRules.ValueProviderProperty.GetName(),
				new NewItemTypesAttribute(typeof(ItemAttributeValueProvider)));
			builder.AddCustomAttributes(typeof(ListSourceDataAdapter), ListSourceDataAdapter.ItemSettingsProperty.GetName(),
				new NewItemTypesAttribute(MapItemSettingsTypes));
			builder.AddCustomAttributes(typeof(MapControl), MapControl.LegendsProperty.GetName(),
				new NewItemTypesAttribute(LegendTypes));
			builder.AddCustomAttributes(typeof(MapControl), MapControl.CoordinateSystemProperty.GetName(),
				new NewItemTypesAttribute(CoordinateSystemTypes));
			builder.AddCustomAttributes(typeof(CoordinateSystemDataAdapterBase), CoordinateSystemDataAdapterBase.SourceCoordinateSystemProperty.GetName(),
				new NewItemTypesAttribute(SourceCoordinateSystemTypes));
			builder.AddCustomAttributes(typeof(MapControl), new FeatureAttribute(typeof(MapStructureAdornerProvider)));
			builder.AddCustomAttributes(typeof(MiniMap), MiniMap.LayersProperty.GetName(),
				new NewItemTypesAttribute(MiniMapLayerTypes));
			builder.AddCustomAttributes(typeof(MapControl), MapControl.CenterPointProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapChartItemBase), MapChartItemBase.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapCustomElement), MapCustomElement.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapPushpin), MapPushpin.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapDot), MapDot.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapLine), MapLine.Point1Property.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapLine), MapLine.Point2Property.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapEllipse), MapEllipse.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapRectangleGeometry), MapRectangleGeometry.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapEllipseGeometry), MapEllipseGeometry.CenterProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapPathFigure), MapPathFigure.StartPointProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(MapRectangle), MapRectangle.LocationProperty.GetName(),
				new NewItemTypesAttribute(PointTypes));
			builder.AddCustomAttributes(typeof(CartesianMapCoordinateSystem), CartesianMapCoordinateSystem.MeasureUnitProperty.GetName(),
				PropertyValueEditor.CreateEditorAttribute(typeof(MeasureUnitEditor)));
		}
	}
	internal class RegisterMetadata : MetadataProviderBase {
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			RegisterHelper.PrepareAttributeTable(builder);
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new MapControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new VectorLayerPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new ImageTilesLayerPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new InformationLayerPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new ShapefileDataAdapterPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new KmlFileDataAdapterPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DataSourceDataAdapterPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BingMapDataProviderPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new OpenStreetMapDataProviderPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BingGeocodeDataProviderPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BingSearchDataProviderPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BingRouteDataProviderPropertyLinesProvider());
			TypeDescriptor.AddAttributes(typeof(MapDataAdapterBase), new DesignTimeParentAttribute(typeof(VectorLayer)));
		}
		protected override Assembly RuntimeAssembly { get { return typeof(MapControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameData; } }
		protected override string ToolboxTabPrefix { get { return "DX: "; } }
	}
}
