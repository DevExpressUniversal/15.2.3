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
	[CustomBindingProperty("Mappings.Value", DXMapStrings.BubbleValueDataMemberCaption, DXMapStrings.BubbleValueDataMemberDescription)]
	public class BubbleSourceCustomBindingPropertiesAttribute : CustomBindingPropertiesAttribute {
	}
	[BubbleSourceCustomBindingPropertiesAttribute]
	public class BubbleChartDataAdapter : ChartDataSourceAdapter {
		public static readonly DependencyProperty MappingsProperty = DependencyPropertyManager.Register("Mappings",
			typeof(MapBubbleMappingInfo), typeof(BubbleChartDataAdapter), new PropertyMetadata(null, MappingsPropertyChanged));
		public static readonly DependencyProperty BubbleSettingsProperty = DependencyPropertyManager.Register("BubbleSettings",
			typeof(MapBubbleSettings), typeof(BubbleChartDataAdapter), new PropertyMetadata(null, ItemSettingsPropertyChanged));
		[Category(Categories.Data)]
		public MapBubbleMappingInfo Mappings {
			get { return (MapBubbleMappingInfo)GetValue(MappingsProperty); }
			set { SetValue(MappingsProperty, value); }
		}
		[Category(Categories.Data)]
		public MapBubbleSettings BubbleSettings {
			get { return (MapBubbleSettings)GetValue(BubbleSettingsProperty); }
			set { SetValue(BubbleSettingsProperty, value); }
		}
		static readonly MapBubbleSettings defaultSettings = new MapBubbleSettings();
		protected override string ValueDataMember { get { return Mappings.Value; } }
		protected internal override MapItemSettingsBase ActualItemSettings {
			get { return BubbleSettings != null ? BubbleSettings : defaultSettings; }
		}
		protected internal override MapItemMappingInfoBase DataMappings { get { return this.Mappings; } }
		protected override IMapDataEnumerator CreateAggregatedDataEnumerator(MapDataController controller) {
			return new AggregatedDataEnumerator(controller, null);
		}
		protected override MarkerType GetMarkerType() {
			return ((MapBubbleSettings)ActualItemSettings).MarkerType;
		}
		protected internal override GroupInfo[] CreateAggregationGroups() {
			return new GroupInfo[] { new GroupInfo(ItemIdDataMember) };
		}
		protected override MapDependencyObject CreateObject() {
			return new BubbleChartDataAdapter();
		}
	}
}
