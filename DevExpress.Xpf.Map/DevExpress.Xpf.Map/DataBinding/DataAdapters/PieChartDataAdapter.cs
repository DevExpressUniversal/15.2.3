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

using System.ComponentModel;
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[CustomBindingProperty("Mappings.Latitude", DXMapStrings.LatitudeDataMemberCaption, DXMapStrings.LatitudeDataMemberDescription)]
	[CustomBindingProperty("Mappings.Longitude", DXMapStrings.LongitudeDataMemberCaption, DXMapStrings.LongitudeDataMemberDescription)]
	[CustomBindingProperty("ItemIdDataMember", DXMapStrings.ItemIdDataMemberCaption, DXMapStrings.ItemIdDataMemberDescription)]
	[CustomBindingProperty("Mappings.SegmentId", DXMapStrings.SegmentIdDataMemberCaption, DXMapStrings.SegmentIdDataMemberDescription)]
	[CustomBindingProperty("Mappings.SegmentValue", DXMapStrings.SegmentValueDataMemberCaption, DXMapStrings.SegmentValueDataMemberDescription)]
	public class PieSourceCustomBindingPropertiesAttribute : CustomBindingPropertiesAttribute {
	}
	[PieSourceCustomBindingPropertiesAttribute]
	public class PieChartDataAdapter : ChartDataSourceAdapter {
		public static readonly DependencyProperty MappingsProperty = DependencyPropertyManager.Register("Mappings",
			typeof(MapPieMappingInfo), typeof(PieChartDataAdapter), new PropertyMetadata(null, MappingsPropertyChanged));
		public static readonly DependencyProperty PieSettingsProperty = DependencyPropertyManager.Register("PieSettings",
			typeof(MapPieSettings), typeof(PieChartDataAdapter), new PropertyMetadata(null, ItemSettingsPropertyChanged));
		[Category(Categories.Data)]
		public MapPieMappingInfo Mappings {
			get { return (MapPieMappingInfo)GetValue(MappingsProperty); }
			set { SetValue(MappingsProperty, value); }
		}
		[Category(Categories.Data)]
		public MapPieSettings PieSettings {
			get { return (MapPieSettings)GetValue(PieSettingsProperty); }
			set { SetValue(PieSettingsProperty, value); }
		}
		static readonly MapPieSettings defaultSettings = new MapPieSettings();
		protected override string ValueDataMember { get { return Mappings.SegmentValue; } }
		protected internal override MapItemSettingsBase ActualItemSettings {
			get { return PieSettings ?? defaultSettings; }
		}
		protected internal override MapItemMappingInfoBase DataMappings { get { return this.Mappings; } }
		IPieSegmentDataLoader CreatePieSegmentDataLoader(MapDataController controller) {
			return new MapChartDataItemSegmentDataLoader(controller);
		}
		protected override IMapDataEnumerator CreateAggregatedDataEnumerator(MapDataController controller) {
			IPieSegmentDataLoader loader = CreatePieSegmentDataLoader(controller);
			return new AggregatedDataEnumerator(controller, loader);
		}
		protected internal override bool CanAggregateData() {
			return base.CanAggregateData() && Mappings != null && !string.IsNullOrEmpty(Mappings.SegmentId);
		}
		protected internal override GroupInfo[] CreateAggregationGroups() {
			return new GroupInfo[] { new GroupInfo(ItemIdDataMember), new GroupInfo(Mappings.SegmentId) };
		}
		protected override MapDependencyObject CreateObject() {
			return new PieChartDataAdapter();
		}
	}
}
