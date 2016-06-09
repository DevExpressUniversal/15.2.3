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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native{
	public class CreateChoroplethMapHistoryItem : DashboardItemHistoryItem<ChoroplethMapDashboardItem> {
		const int NewGroupIndex = -1;
		readonly ChoroplethMapDragSection section;
		readonly int groupIndex;
		readonly ChoroplethMap initialColumn;
		readonly ChoroplethMap currentColumn;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemMapLayerOptions; } }
		ChoroplethMapDashboardItem MapDashboardItem { get { return (ChoroplethMapDashboardItem)DashboardItem; } }
		public CreateChoroplethMapHistoryItem(ChoroplethMapDragSection section, int groupIndex, ChoroplethMapDashboardItem mapDashboardItem,
			ChoroplethMap initialMap, ChoroplethMap currentMap)
			: base(mapDashboardItem) {
			this.section = section;
			this.initialColumn = initialMap;
			this.groupIndex = groupIndex;
			this.currentColumn = currentMap;
		}
		void UpdateGroup(ChoroplethMap removeColumn, ChoroplethMap addColumn) {
			IElementContainer elementContainer = MapDashboardItem.ElementContainer;
			int elementIndex = elementContainer.SelectedElementIndex;
			MapDashboardItem.Dashboard.BeginUpdate();
			try {
				section.RemoveGroup(removeColumn);
				section.InsertGroup(groupIndex, addColumn);
				elementContainer.SelectedElementIndex = elementIndex;
			}
			finally {
				MapDashboardItem.Dashboard.EndUpdate();
			}
		}
		protected override void PerformUndo() {
			UpdateGroup(currentColumn, initialColumn);
		}
		protected override void PerformRedo() {
			UpdateGroup(initialColumn, currentColumn);
		}
	}
	public abstract class ChangeChoroplethMapHistoryItemBase : DashboardItemHistoryItem<ChoroplethMapDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemMapLayerOptions; } }
		protected ChangeChoroplethMapHistoryItemBase(ChoroplethMapDashboardItem mapDashboardItem)
			: base(mapDashboardItem) {
		}
		protected abstract void UndoOptions();
		protected abstract void RedoOptions();
		protected override void PerformUndo() {
			UndoOptions();
		}
		protected override void PerformRedo() {
			RedoOptions();
		}
	}
	public class ChangeValueMapHistoryItem : ChangeChoroplethMapHistoryItemBase {
		readonly ValueMap valueMap;
		readonly MapPalette initialPalette;
		readonly MapPalette currentPalette;
		readonly MapScale inititalScale;
		readonly MapScale currentScale;
		public ChangeValueMapHistoryItem(ChoroplethMapDashboardItem mapDashboardItem, ValueMap valueMap, MapPalette currentPalette, MapScale currentScale)
			: base(mapDashboardItem) {
			this.valueMap = valueMap;
			this.initialPalette = valueMap.Palette;
			this.currentPalette = currentPalette;
			this.inititalScale = valueMap.Scale;
			this.currentScale = currentScale;
		}
		protected override void UndoOptions() {
			DashboardItem.LockChanging();
			valueMap.Palette = initialPalette;
			valueMap.Scale = inititalScale;
			DashboardItem.UnlockChanging();
			DashboardItem.OnChanged(ChangeReason.View);
		}
		protected override void RedoOptions() {
			DashboardItem.LockChanging();
			valueMap.Palette = currentPalette;
			valueMap.Scale = currentScale;
			DashboardItem.UnlockChanging();
			DashboardItem.OnChanged(ChangeReason.View);
		}
	}
	public class ChangeDeltaMapHistoryItem : ChangeChoroplethMapHistoryItemBase {
		readonly DeltaMap valueMap;
		readonly DeltaOptions initialOptions;
		readonly DeltaOptions currentOptions;
		public ChangeDeltaMapHistoryItem(ChoroplethMapDashboardItem mapDashboardItem, DeltaMap valueMap, DeltaOptions currentOptions)
			: base(mapDashboardItem) {
			this.valueMap = valueMap;
			this.initialOptions = new DeltaOptions();
			this.initialOptions.Assign(valueMap.DeltaOptions);
			this.currentOptions = currentOptions;
		}
		protected override void UndoOptions() {
			valueMap.DeltaOptions.Assign(initialOptions);
		}
		protected override void RedoOptions() {
			valueMap.DeltaOptions.Assign(currentOptions);
		}
	}
}
