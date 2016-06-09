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

using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.ServerMode {
	static class ExpandHelper {
		public static bool CanQueryFullLevel(IList<ServerModeColumn> area, int level) {
			for(int i = 0; i <= level; i++) {
				if(area[i].TopValueCount != 0 && area[i].TopValueMode == TopValueMode.ParentFieldValues || area[i].TopValueShowOthers)
					return false;
				if(i > 0 && area[i - 1].SortBySummary != null)
					return false;
			}
			return true;
		}
		static bool CalculateTotals(IList<ServerModeColumn> area, int level) {
			for(int i = 0; i <= level; i++)
				if(area[i].CalculateTotals)
					return true;
			return false;
		}
		static bool CanQueryFastLevel(IList<ServerModeColumn> area, int level) {
			return CanQueryFullLevel(area, level) && !CalculateTotals(area, level);
		}
		public static int GetUnTotalledExpandLevel(List<ServerModeColumn> area, bool hasData, int unExpandedLevel) {
			if(!hasData)
				return area.Count - 1;
			int level = area.Count - 1;
			while(level > 0) {
				if(CanQueryFastLevel(area, level) || level == unExpandedLevel)
					return level;
				else
					level--;
			}
			return level;
		}
		public static int GetUnTotalledExpandLevel(List<ServerModeColumn> area, bool hasData, AreaFieldValues fieldValues) {
			return GetUnTotalledExpandLevel(area, hasData, fieldValues.GetLastNotExpandedLevel());
		}
	}
}
