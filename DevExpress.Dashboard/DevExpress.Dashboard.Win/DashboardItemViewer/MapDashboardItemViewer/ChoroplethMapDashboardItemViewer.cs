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
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraMap;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class ChoroplethMapDashboardItemViewer : MapDashboardItemViewer {
		ChoroplethMapDashboardItemViewControl ChoroplethViewControl { get { return (ChoroplethMapDashboardItemViewControl)MapViewControl; } }
		protected override MapDashboardItemViewControl CreateViewControl(DashboardMapControl mapControl) {
			return new ChoroplethMapDashboardItemViewControl(mapControl);
		}
		protected override bool CancelSelection(MapItem item) {
			return !ChoroplethViewControl.IsSelectionAllowed(item);
		}
		protected override void Update(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			ChoroplethViewControl.Update((ChoroplethMapDashboardItemViewModel)viewModel, data, dataChanged);
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			DataPointInfo dataPoint = new DataPointInfo();
			MapHitInfo hitInfo = MapControl.CalcHitInfo(location);
			if(hitInfo.InMapPath) {
				MapPath path = hitInfo.MapPath;
				MapItemAttributeCollection attributes = path.Attributes;
				if(attributes != null && attributes.Count > 0) {
					MapItemAttribute selectionAttribute = attributes[ChoroplethMapDashboardItemViewControl.SelectionAttributeName];
					if(selectionAttribute != null) {
						dataPoint.DimensionValues.Add(DashboardDataAxisNames.DefaultAxis, new List<object>() { selectionAttribute.Value });
						FillMeasureIds(dataPoint);
						return dataPoint;
					}
				}
			}
			return null;
		}
		void FillMeasureIds(DataPointInfo dataPoint) {
			ChoroplethMapDashboardItemViewModel mapItemViewModel = ViewModel as ChoroplethMapDashboardItemViewModel;
			if(mapItemViewModel != null) {
				ChoroplethColorizerViewModel colorizerViewModel = mapItemViewModel.ChoroplethColorizer;
				if(colorizerViewModel != null) {
					MapDeltaColorizerViewModel deltaViewModel = colorizerViewModel as MapDeltaColorizerViewModel;
					if(deltaViewModel != null) {
						dataPoint.Deltas.Add(deltaViewModel.DeltaValueId);
						return;
					}
					MapValueColorizerViewModel valueViewModel = colorizerViewModel as MapValueColorizerViewModel;
					if(valueViewModel != null) {
						dataPoint.Measures.Add(valueViewModel.ValueId);
						return;
					}
				}
			}
		}
	}
}
