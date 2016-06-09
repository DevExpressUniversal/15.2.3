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
using DevExpress.DashboardCommon.Service;
using System;
namespace DevExpress.DashboardWin.Native {
	public class MapDashboardItemDesigner : DataDashboardItemDesigner {
		MapDashboardItem MapDashboardItem { get { return (MapDashboardItem)DashboardItem; } }
		MapDashboardItemViewer MapViewer { get { return (MapDashboardItemViewer)ItemViewer; } }
		public void OnMapControlViewportChanged(MapViewportState viewportState) {
			MapDashboardItem.LockChanging();
			try {
				MapViewport viewport = new MapViewport(MapDashboardItem);
				viewport.TopLatitude = viewportState.TopLatitude;
				viewport.BottomLatitude = viewportState.BottomLatitude;
				viewport.LeftLongitude = viewportState.LeftLongitude;
				viewport.RightLongitude = viewportState.RightLongitude;
				viewport.CenterPointLatitude = viewportState.CenterPointLatitude;
				viewport.CenterPointLongitude = viewportState.CenterPointLongitude;
				viewport.CreateViewerPaddings = false;
				if (viewport != MapDashboardItem.Viewport) {
					MapViewportHistoryItem historyItem = new MapChangeViewportHistoryItem(MapDashboardItem, viewport);
					historyItem.Redo(DashboardDesigner);
					DashboardDesigner.History.Add(historyItem);
				}
			}
			finally {
				MapDashboardItem.UnlockChanging();
			}
		}
	}
}
