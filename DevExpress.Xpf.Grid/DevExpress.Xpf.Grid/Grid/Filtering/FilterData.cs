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

using System;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Grid.Filtering {
	public class FilterData : IEquatable<FilterData> {
		public static FilterData Null {
			get { return new FilterData(string.Empty, null, string.Empty, FilterDateType.None); }
		}
		public FilterData(string caption, CriteriaOperator criteria, string tooltip, FilterDateType filterType) {
			Caption = caption;
			Criteria = criteria;
			Tooltip = tooltip;
			FilterType = filterType;
		}
		public string Caption { get; private set; }
		public CriteriaOperator Criteria { get; private set; }
		public string Tooltip { get; private set; }
		public FilterDateType FilterType { get; private set; }
		public override bool Equals(object obj) {
			var asFilterData = obj as FilterData;
			if(asFilterData == null) return false;
			return Equals(asFilterData);
		}
		public bool Equals(FilterData other) {
			return Caption == other.Caption
				&& CriteriaOperator.CriterionEquals(Criteria, other.Criteria)
				&& Tooltip == other.Tooltip
				&& FilterType == other.FilterType;
		}
		public override int GetHashCode() {
			return FilterType.GetHashCode();
		}
	}
}
