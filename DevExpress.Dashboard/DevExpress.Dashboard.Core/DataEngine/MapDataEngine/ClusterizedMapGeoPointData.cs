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
using DevExpress.DashboardCommon.Data;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Native {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class ClusterizedMapGeoPointData : ReadOnlyTypedList {
		readonly List<object[]> data;
		Dictionary<GeoPointEx, List<object>> olapUniqueCoordinates;
		public Dictionary<GeoPointEx, List<object>> OlapUniqueCoordinates { get { return olapUniqueCoordinates; } }
		public ClusterizedMapGeoPointData(List<object[]> data, Dictionary<GeoPointEx, List<object>> olapUniqueCoordinates, PropertiesRepository propertiesRepository) {
			this.data = data;
			this.olapUniqueCoordinates = olapUniqueCoordinates;
			foreach(DashboardItemDataArrayPropertyDescriptor pd in propertiesRepository)
				Properties.Add(pd);
		}
		protected override int GetItemsCount() {
			return data != null ? data.Count : 0;
		}
		protected override object GetItemValue(int index) {
			return data != null ? data[index] : null;
		}
	}
}
