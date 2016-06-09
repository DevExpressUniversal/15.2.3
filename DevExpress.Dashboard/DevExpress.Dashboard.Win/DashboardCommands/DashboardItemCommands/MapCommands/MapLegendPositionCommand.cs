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
using System.Linq;
using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Bars;
namespace DevExpress.DashboardWin.Commands {
	public abstract class MapLegendPositionCommandBase : DashboardItemCommand<MapDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapLegendPositionCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapLegendPositionDescription; } }
		public override string ImageName { get { return "ChangeLegendPosition"; } }
		protected MapLegendPositionCommandBase(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapLegendPositionCommand : MapLegendPositionCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapLegendPosition; } }
		public MapLegendPositionCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			MapLegendPositionCommandUIState legendPositionGalleryState = state as MapLegendPositionCommandUIState;
			if(legendPositionGalleryState != null) {
				MapDashboardItem mapDashboardItem = DashboardItem;
				if(mapDashboardItem != null && mapDashboardItem.Legend != null)
					legendPositionGalleryState.UpdateVisualState(mapDashboardItem.Legend);
			}
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			MapLegendPositionCommandUIState legendPositionGalleryState = state as MapLegendPositionCommandUIState;
			if(legendPositionGalleryState != null) {
				MapDashboardItem mapDashboardItem = DashboardItem;
				if(mapDashboardItem != null && mapDashboardItem.Legend != null) {
					MapLegendPositionGalleryItem galleryItem = legendPositionGalleryState.SelectedGalleryItem as MapLegendPositionGalleryItem;
					if(galleryItem != null) {
						MapLegendPositionHistoryItem historyItem = new MapLegendPositionHistoryItem(mapDashboardItem, galleryItem.Position, galleryItem.Orientation);
						historyItem.Redo(Control);
						Control.History.Add(historyItem);
					}
				}
			}
		}
	}
	public class WeightedLegendPositionCommand : MapLegendPositionCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.WeightedLegendPosition; } }
		public WeightedLegendPositionCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			WeightedLegendPositionCommandUIState galleryState = state as WeightedLegendPositionCommandUIState;
			if(galleryState != null) {
				MapDashboardItem mapDashboardItem = Control.SelectedDashboardItem as MapDashboardItem;
				if(mapDashboardItem != null && mapDashboardItem.WeightedLegend != null)
					galleryState.UpdateVisualState(mapDashboardItem.WeightedLegend);
			}
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			WeightedLegendPositionCommandUIState galleryState = state as WeightedLegendPositionCommandUIState;
			if(galleryState != null) {
				MapDashboardItem mapDashboardItem = Control.SelectedDashboardItem as MapDashboardItem;
				if(mapDashboardItem != null && mapDashboardItem.WeightedLegend != null) {
					WeightedLegendPositionGalleryItem galleryItem = galleryState.SelectedGalleryItem as WeightedLegendPositionGalleryItem;
					if(galleryItem != null) {
						WeightedLegendPositionHistoryItem historyItem = new WeightedLegendPositionHistoryItem(mapDashboardItem, galleryItem.Position);
						historyItem.Redo(Control);
						Control.History.Add(historyItem);
					}
				}
			}
		}
	}
}
