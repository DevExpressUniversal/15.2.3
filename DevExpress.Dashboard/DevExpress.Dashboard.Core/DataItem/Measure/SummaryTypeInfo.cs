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
using DevExpress.Data.PivotGrid;
namespace DevExpress.DashboardCommon.Native {
	public class SummaryTypeInfo {
		public static SummaryTypeInfo Number {
			get {
				return new SummaryTypeInfo {
					TextSummaryTypes = CountSummaryType,
					BooleanSummaryTypes = CountSummaryType,
					DateTimeSummaryTypes = CountSummaryType,
					NumericSummaryTypes = GetAllSummaryTypes(),
					ConvertibleSummaryTypes = GetAllSummaryTypes(),
					ComparableSummaryTypes = CountSummaryType,
					ConvertibleComparableSummaryTypes = GetAllSummaryTypes(),
					ObjectSummaryTypes = CountSummaryType
				};
			}
		}
		public static SummaryTypeInfo Text {
			get {
				return new SummaryTypeInfo {
					TextSummaryTypes = CountMinMaxSummaryTypes,
					BooleanSummaryTypes = CountMinMaxSummaryTypes,
					DateTimeSummaryTypes = CountMinMaxSummaryTypes,
					NumericSummaryTypes = GetAllSummaryTypes(),
					ConvertibleSummaryTypes = GetAllSummaryTypes(SummaryType.Min, SummaryType.Max),
					ComparableSummaryTypes = CountMinMaxSummaryTypes,
					ConvertibleComparableSummaryTypes = GetAllSummaryTypes(),
					ObjectSummaryTypes = CountSummaryType,
				};
			}
		}
		public static SummaryTypeInfo Hidden {
			get {
				return new SummaryTypeInfo {
					TextSummaryTypes = CountMinMaxSummaryTypes,
					BooleanSummaryTypes = CountMinMaxSummaryTypes,
					DateTimeSummaryTypes = CountMinMaxSummaryTypes,
					NumericSummaryTypes = GetAllSummaryTypes(),
					ConvertibleSummaryTypes = GetAllSummaryTypes(),
					ComparableSummaryTypes = CountMinMaxSummaryTypes,
					ConvertibleComparableSummaryTypes = GetAllSummaryTypes(),
					ObjectSummaryTypes = CountSummaryType,
				};
			}
		}
		static readonly IList<SummaryType> CountSummaryType = new SummaryType[] { SummaryType.Count, SummaryType.CountDistinct };
		static readonly IList<SummaryType> CountMinMaxSummaryTypes = new SummaryType[] { SummaryType.Count, SummaryType.CountDistinct, SummaryType.Min, SummaryType.Max };
		static IList<SummaryType> GetAllSummaryTypes(params SummaryType[] excludedSummaryTypes) {
			List<SummaryType> summaryTypes = new List<SummaryType>((IEnumerable<SummaryType>)Enum.GetValues(typeof(SummaryType)));
			summaryTypes.Remove(SummaryType.CountDistinct);
			summaryTypes.Insert(summaryTypes.IndexOf(SummaryType.Count) + 1, SummaryType.CountDistinct);
			foreach(SummaryType excludedSummaryType in excludedSummaryTypes)
				summaryTypes.Remove(excludedSummaryType);
			return summaryTypes;
		}
		public IList<SummaryType> TextSummaryTypes { get; private set; }
		public IList<SummaryType> BooleanSummaryTypes { get; private set; }
		public IList<SummaryType> DateTimeSummaryTypes { get; private set; }
		public IList<SummaryType> NumericSummaryTypes { get; private set; }
		public IList<SummaryType> ConvertibleSummaryTypes { get; private set; }
		public IList<SummaryType> ComparableSummaryTypes { get; private set; }
		public IList<SummaryType> ConvertibleComparableSummaryTypes { get; private set; }
		public IList<SummaryType> ObjectSummaryTypes { get; private set; }
	}
}
