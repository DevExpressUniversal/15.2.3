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

using System.Collections;
using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.OLAP {
	class OLAPByDisplayTextMemberComparer : BaseComparer, IComparer<QueryMember> {
		public OLAPByDisplayTextMemberComparer() {
		}
		int IComparer<QueryMember>.Compare(QueryMember x, QueryMember y) {
			int res = Comparer<string>.Default.Compare(x.Caption, y.Caption);
			if(res != 0)
				return res;
			return Comparer.Default.Compare(x.UniqueLevelValue, y.UniqueLevelValue);
		}
	}
	class OLAPByMemberAttributeValueComparer<TType> : BaseComparer, IComparer<QueryMember> {
		OLAPMetadataColumn column;
		string attrName;
		public OLAPByMemberAttributeValueComparer(OLAPCubeColumn column, string attrName) {
			this.column = column.Metadata;
			this.attrName = attrName;
		}
		int IComparer<QueryMember>.Compare(QueryMember x, QueryMember y) {
			try {
				OLAPMember a = (OLAPMember)x;
				OLAPMember b = (OLAPMember)y;
				DevExpress.XtraPivotGrid.OLAPMemberProperty ap;
				if(a.AutoPopulatedProperties == null || !a.AutoPopulatedProperties.TryGetValue(attrName, out ap))
					ap = null;
				DevExpress.XtraPivotGrid.OLAPMemberProperty bp;
				if(b.AutoPopulatedProperties == null || !b.AutoPopulatedProperties.TryGetValue(attrName, out bp))
					bp = null;
				ITypedValue<TType> apt = ap as ITypedValue<TType>;
				ITypedValue<TType> bpt = bp as ITypedValue<TType>;
				if(apt == null) {
					if(bpt == null)
						return 0;
					return 1;
				}
				if(bpt == null)
					return -1;
				int res = Comparer<TType>.Default.Compare(apt.Value, bpt.Value);
				if(res != 0)
					return res;
				res = Comparer.Default.Compare(x.Value, y.Value);
				if(res != 0)
					return res;
				return Comparer<string>.Default.Compare(a.UniqueName, b.UniqueName);
			} catch {
				int value = Comparer<bool>.Default.Compare(column.GetIsCalculatedMember((OLAPMember)x), column.GetIsCalculatedMember((OLAPMember)y));
				if(value != 0)
					return value;
				int result = Comparer<bool>.Default.Compare(x.GetType().IsValueType(), y.GetType().IsValueType());
				if(result != 0)
					return -result;
				return Comparer<string>.Default.Compare(x.GetType().FullName, y.GetType().FullName);
			}
		}
	}
	class OLAPByMemberValueComparer : BaseComparer, IComparer<QueryMember> {
		OLAPMetadataColumn column;
		public OLAPByMemberValueComparer(OLAPCubeColumn column) {
			this.column = column.Metadata;
		}
		int IComparer<QueryMember>.Compare(QueryMember x, QueryMember y) {
			try {
				int res = Comparer.Default.Compare(x.Value, y.Value);
				if(res != 0)
					return res;
				return Comparer.Default.Compare(x.UniqueLevelValue, y.UniqueLevelValue);
			} catch {
				int value = Comparer<bool>.Default.Compare(column.GetIsCalculatedMember((OLAPMember)x), column.GetIsCalculatedMember((OLAPMember)y));
				if(value != 0)
					return value;
				int result = Comparer<bool>.Default.Compare(x.GetType().IsValueType(), y.GetType().IsValueType());
				if(result != 0)
					return -result;
				return Comparer<string>.Default.Compare(x.GetType().FullName, y.GetType().FullName);
			}
		}
	}
}
