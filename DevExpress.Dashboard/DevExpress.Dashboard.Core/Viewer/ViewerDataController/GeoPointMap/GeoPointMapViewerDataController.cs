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

using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class GeoPointMapViewerDataController {
		public const string LatitudeSelection = "LatitudeSelection";
		public const string LongitudeSelection = "LongitudeSelection";
		public const string DisplayTextPostFix = "_DisplayText";
		readonly GeoPointMapDashboardItemViewModelBase viewModel;
		readonly MultiDimensionalData data;
		public GeoPointMapViewerDataController(GeoPointMapDashboardItemViewModelBase viewModel, MultiDimensionalData data) {
			this.viewModel = viewModel;
			this.data = data;
		}
		public GeoPointMapMultiDimensionalDataSourceBase GetDataSource() {
			switch(viewModel.MapKind) {
				case GeoPointMapKind.GeoPoint:
					return new GeoPointMapMultiDimensionalDataSource((GeoPointMapDashboardItemViewModel)viewModel, data);
				case GeoPointMapKind.BubbleMap:
					return new BubbleMapMultiDimensionalDataSource((BubbleMapDashboardItemViewModel)viewModel, data);
				case GeoPointMapKind.PieMap:
					return new PieMapMultiDimensionalDataSource((PieMapDashboardItemViewModel)viewModel, data);
				default:
					return null;
			}
		}
	}
}
