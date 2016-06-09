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
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	public interface ICustomSortHelper {
		object GetSortByObject(object[] crossValues, bool isColumn);
		object GetValue(object row, object column, PivotGridFieldBase field);
	}
	class EmptyCustomSortHelper : ICustomSortHelper {
		object ICustomSortHelper.GetSortByObject(object[] crossValues, bool isColumn) {
			return null;
		}
		object ICustomSortHelper.GetValue(object row, object column, PivotGridFieldBase field) {
			return null;
		}
	}
	class CustomSortHelper<TColumn, TObject, TData> : ICustomSortHelper where TColumn : QueryColumn {
		ISortContext<TColumn, TObject, TData> context;
		QueryAreas<TColumn> areas;
		IDataSourceHelpersOwner<TColumn> owner;
		public CustomSortHelper(ISortContext<TColumn, TObject, TData> context, QueryAreas<TColumn> areas, IDataSourceHelpersOwner<TColumn> owner) {
			this.context = context;
			this.areas = areas;
			this.owner = owner;
		}
		object ICustomSortHelper.GetSortByObject(object[] crossValues, bool isColumn) {
			List<QueryMember> members = new List<QueryMember>();
			IList<TColumn> area = areas.GetArea(isColumn);
			if(area.Count < crossValues.Length)
				return null;
			for(int i = 0; i < crossValues.Length; i++) {
				QueryMember member = areas.GetMemberByValue(area[i].Metadata, crossValues[i]);
				if(member == null)
					return null;
				members.Add(member);
			}
			return context.GetSortByObject(members, isColumn);
		}
		object ICustomSortHelper.GetValue(object row, object column, PivotGridFieldBase field) {
			if(row == null)
				return null;
			if(column == null)
				return null;
			TColumn dataColumn = owner.CubeColumns[field];
			if(dataColumn == null)
				return null;
			return context.GetValueProvider().GetValue((TObject)column, (TObject)row, context.GetData(dataColumn));
		}
	}
}
