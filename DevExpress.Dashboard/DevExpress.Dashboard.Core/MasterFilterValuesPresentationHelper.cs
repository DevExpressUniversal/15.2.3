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
using System.Collections.Generic;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
namespace DevExpress.DashboardCommon {
	class FormattableValueComparer : EqualityComparer<FormattableValue> {
		public override bool Equals(FormattableValue x, FormattableValue y) {
			return
				(x.Value == null && y.Value == null || x.Value != null && y.Value != null && x.Value.Equals(y.Value)) &&
				(x.RangeLeft == null && y.RangeLeft == null || x.RangeLeft != null && y.RangeLeft != null && x.RangeLeft.Equals(y.Value)) &&
				(x.RangeRight == null && y.RangeRight == null || x.RangeRight != null && y.RangeRight != null && x.RangeRight.Equals(y.Value)) &&
				x.Format.EqualsViewModel(y.Format);
		}
		public override int GetHashCode(FormattableValue x) {
			return
				x.Value != null ? x.Value.GetHashCode() :
				x.RangeLeft != null ? x.RangeLeft.GetHashCode() :
				x.RangeRight != null ? x.RangeRight.GetHashCode() :
				0;
		}
	}
	static class MasterFilterValuesPresentationHelper {
		internal const int MaxFilterValues = 20;
		internal static IList<DimensionFilterValues> GetUniqueMasterFilterValues(IEnumerable<DimensionFilterValues> masterFilterValues, bool truncate) {
			List<DimensionFilterValues> result = new List<DimensionFilterValues>();
			DimensionFilterValues resultFilterValues;
			Dictionary<FormattableValue, bool> uniqueValues;
			FormattableValueComparer comparer = new FormattableValueComparer();
			foreach(DimensionFilterValues filterValues in masterFilterValues) {
				resultFilterValues = new DimensionFilterValues();
				result.Add(resultFilterValues);
				resultFilterValues.Name = filterValues.Name;
				resultFilterValues.Truncated = filterValues.Truncated;
				uniqueValues = new Dictionary<FormattableValue, bool>(comparer);
				foreach(FormattableValue value in filterValues.Values) {
					uniqueValues[value] = true;
					if(truncate && uniqueValues.Count > MaxFilterValues) {
						uniqueValues.Remove(value);
						resultFilterValues.Truncated = true;
						break;
					}
				}
				resultFilterValues.Values = new List<FormattableValue>(uniqueValues.Keys);
			}
			return result;
		}
	}
}
