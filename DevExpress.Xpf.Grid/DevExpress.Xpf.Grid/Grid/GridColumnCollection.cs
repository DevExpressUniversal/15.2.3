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
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.Linq;
namespace DevExpress.Xpf.Grid {
	public class GridColumnCollection : ColumnCollectionBase<GridColumn> {
		public GridColumnCollection(GridControl grid)
			: base(grid) {
		}
		GridControl grid { get { return ((GridControl)((IColumnCollection)this).Owner); } }
		protected override void OnRemoveItem(GridColumn column) {
			if(!grid.IsDeserializing) {
				if(column.IsGrouped)
					grid.ActualGroupCountCore = Math.Max(0, grid.ActualGroupCountCore - 1);
				grid.UngroupBy(column);
				if(grid.SortInfo[column.FieldName] != null)
					grid.SortInfo.Remove(grid.SortInfo[column.FieldName]);
				var actualSortInfo = grid.ActualSortInfoCore.FirstOrDefault(info => info.FieldName == column.FieldName);
				if(actualSortInfo != null)
					grid.ActualSortInfoCore.Remove(actualSortInfo);
			}
			base.OnRemoveItem(column);
		}
		protected override void ClearItems() {
			if(!grid.IsDeserializing) grid.GroupCount = 0;
			base.ClearItems();
		}
	}
}
