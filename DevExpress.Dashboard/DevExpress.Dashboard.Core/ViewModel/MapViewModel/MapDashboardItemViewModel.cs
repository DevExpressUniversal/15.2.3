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

using DevExpress.DashboardCommon.Native;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.ViewModel {
	public class TooltipDataItemViewModel {
		public string Caption { get; set; }
		public string DataId { get; set; }
		public string AttributeName { get; set; }
		public TooltipDataItemViewModel() {
		}
		public TooltipDataItemViewModel(string caption, string dataMember) {
			Caption = caption;
			DataId = dataMember;
		}
	}
	public abstract class MapDashboardItemViewModel : DataDashboardItemViewModel {
		public MapViewport Viewport { get; set; }
		public MapShapeItem[] MapItems { get; set; }
		public IList<TooltipDataItemViewModel> TooltipMeasures { get; set; }
		public bool LockNavigation { get; set; }
		public string ShapeTitleAttributeName { get; set; }
		protected virtual List<string> FilteredAttributes { 
			get {
				List<string> filteredAttributes = new List<string>();
				if(ShapeTitleAttributeName != null)
					filteredAttributes.Add(ShapeTitleAttributeName);
				return filteredAttributes; 
			} 
		}
		protected MapDashboardItemViewModel()
			: base() {
		}
		protected MapDashboardItemViewModel(MapDashboardItem dashboardItem, IList<TooltipDataItemViewModel> tooptipMeasuresViewModel)
			: base(dashboardItem) {
			Viewport = dashboardItem.Viewport.Clone();
			TooltipMeasures = tooptipMeasuresViewModel;
			LockNavigation = dashboardItem.LockNavigation;
			ShapeTitleAttributeName = dashboardItem.ShapeTitleAttributeName;
		}
		protected MapShapeItem[] PrepareMapItems(MapShapeItem[] mapItems) {
			if(mapItems == null)
				return null;
			MapShapeItem[] result = new MapShapeItem[mapItems.Length];
			for(int i = 0; i < mapItems.Length; i++)
				result[i] = mapItems[i].CloneWithFilteredAttributes(FilteredAttributes);
			return result;
		}
	}
}
