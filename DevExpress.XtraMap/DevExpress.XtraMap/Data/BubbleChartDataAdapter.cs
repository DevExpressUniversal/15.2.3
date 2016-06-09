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
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class BubbleChartDataAdapter : ChartDataSourceAdapter, ITemplateGeometryProvider {
		MarkerType markerType = MarkerType.Default;
		[Category(SRCategoryNames.Data), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXtraMapLocalizedDescription("BubbleChartDataAdapterMappings")
#else
	Description("")
#endif
]
		public new MapBubbleMappingInfo Mappings { get { return (MapBubbleMappingInfo)base.Mappings; } }
		[Category(SRCategoryNames.Data), 
		DefaultValue(""),
#if !SL
	DevExpressXtraMapLocalizedDescription("BubbleChartDataAdapterBubbleItemDataMember")
#else
	Description("")
#endif
]
		public string BubbleItemDataMember { get { return base.ChartItemDataMember; } set { base.ChartItemDataMember = value; } }
		[Category(SRCategoryNames.Appearance), 
		DefaultValue(MarkerType.Default),
#if !SL
	DevExpressXtraMapLocalizedDescription("BubbleChartDataAdapterMarkerType")
#else
	Description("")
#endif
]
		public MarkerType MarkerType {
			get { return markerType; }
			set {
				if(markerType == value)
					return;
				markerType = value;
				OnStyleChanged();
			}
		}
		#region ITemplateGeometryProvider 
		TemplateGeometryType ITemplateGeometryProvider.TemplateType {
			get {
				return MapUtils.ToTemplateGeometryType(MarkerType);
			}
		}
		#endregion
		protected void OnStyleChanged() {
			NotifyDataChanged(MapUpdateType.Style | MapUpdateType.ViewInfo);   
		}
		protected override MapItemMappingInfoBase CreateMappings(LayerDataManager dataManager) {
			return new MapBubbleMappingInfo(dataManager);
		}
		protected override IMapDataEnumerator CreateAggregatedDataEnumerator(MapDataController controller) {
			return new AggregatedDataEnumerator(controller, null);
		}
		protected internal override MapItemType GetDefaultMapItemType() {
			return MapItemType.Bubble;
		}		
		protected internal override GroupInfo[] CreateAggregationGroups() {
			return new GroupInfo[] { new GroupInfo(BubbleItemDataMember) };
		}
		public override string ToString() {
			return "(BubbleChartDataAdapter)";
		}
	}
}
