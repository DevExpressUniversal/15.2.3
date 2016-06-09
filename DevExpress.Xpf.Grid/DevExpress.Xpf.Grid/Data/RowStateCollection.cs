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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Grid.Native;
using System.Collections.Generic;
namespace DevExpress.Xpf.Data {
	public class RowStateCollection : SelectedRowsCollection {
		public RowStateCollection(RowStateController selectionController) : base(selectionController) { }
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) { }
	}
	public class DetailInfoCollection : RowStateCollection {
		public DetailInfoCollection(RowStateController selectionController) : base(selectionController) { }
		protected override void SetSelectionObject(object row, object selectionObject) {
			base.SetSelectionObject(row, selectionObject);
			RowDetailContainer rowDetailContainer = selectionObject as RowDetailContainer;
			if(rowDetailContainer != null) {
				rowDetailContainer.MasterListIndex = (int)row;
			}
		}
		public IEnumerable<int> GetRowListIndicesWithExpandedDetails() {
			foreach(int index in Rows.Keys) {
				RowDetailContainer container = (RowDetailContainer)Rows[index];
				if(container.RootDetailInfo.IsExpanded)
					yield return index;
			}
		}
		public IEnumerable<RowDetailContainer> GetContainers() {
			return Rows.Keys.Cast<int>().Select(x => (RowDetailContainer)Rows[x]);
		}
		protected override void OnItemDeleted(int listSourceRow) {
			RowDetailContainer container = GetSelectedObject(listSourceRow) as RowDetailContainer;
			if(container != null)
				container.Detach();
			base.OnItemDeleted(listSourceRow);
		}
	}
}
