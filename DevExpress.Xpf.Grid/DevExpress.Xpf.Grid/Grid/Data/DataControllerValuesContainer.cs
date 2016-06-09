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
using DevExpress.Xpf.Data;
namespace DevExpress.Xpf.Grid {
	public struct DataControllerLazyValuesContainer {
		GridViewBase view;
		int? rowHandle;
		int? listSourceIndex;
		GridControl Grid { get { return view.Grid; } }
		public int RowHandle {
			get {
				if(rowHandle == null)
					rowHandle = Grid.GetRowHandleByListIndex(ListSourceIndex);
				return rowHandle.Value;
			}
		}
		public int ListSourceIndex {
			get {
				if(listSourceIndex == null)
					listSourceIndex = RowHandle == GridControl.InvalidRowHandle ? GridControl.InvalidRowHandle : Grid.GetListIndexByRowHandle(RowHandle);
				return listSourceIndex.Value;
			}
		}
		public DataControllerLazyValuesContainer(GridViewBase view, int? rowHandle, int? listSourceIndex) {
			this.view = view;
			if((rowHandle.HasValue && listSourceIndex.HasValue) || (!rowHandle.HasValue && !listSourceIndex.HasValue))
				throw new ArgumentException("rowHandle, listSourceIndex");
			this.rowHandle = rowHandle;
			this.listSourceIndex = listSourceIndex;
		}
	}
}
