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

using System.Collections.Generic;
using DevExpress.DashboardCommon.Service;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public class StateModel {
		public IEnumerable<ItemFilterModel> Filters { get; set; }
		public IEnumerable<DashboardParameterModel> Parameters { get; set; }
		public IEnumerable<object> DrilldownValues { get; set; }
		public int SelectedElementIndex { get; set; }						
		public MapClusterizationRequestInfo ClusterizationInfo { get; set; } 
		public ExpandingModel ExpandingModel { get; set; }				   
	}
	public class DashboardParameterModel : DashboardParameterInfo {
		public DashboardParameterModel(string name, object value) {
			Name = name;
			Value = value;
		}
	}
	public class ItemFilterModel {
		public IEnumerable<DimensionFilterModel> DimensionFilters { get; set; }
		public bool IsExcludingAllFilter { get; set; }
	}
	public class DimensionModel {
		public string ID { get; set; }
		public string DataMember { get; set; }
		public DateTimeGroupInterval DateTimeGroupInterval { get; set; }
		public Dimension CreateInstance() {
			return new Dimension(ID, DataMember, DateTimeGroupInterval);
		}
	}
	public class DimensionFilterModel {
		public DimensionModel Model { get; set; }
		public IList<object> FilterValues { get; set; }
		public MasterFilterRange Range { get; set; }
	}
	public class ExpandingModel {
		public PivotDashboardItemExpandCollapseState PivotExpandCollapseState { get; set; }
		public bool IsColumn { get; set; }
		public object[] Expand { get; set; }
	}
}
