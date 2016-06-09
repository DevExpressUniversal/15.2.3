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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class ModifyCollectionHelper {
		PropertyGridView view;
		protected PropertyGridView View { get { return view; } }
		protected PropertyGridControl PropertyGrid { get { return View.PropertyGrid; } }
		protected DataViewBase DataView { get { return PropertyGrid.DataView; } }
		public ModifyCollectionHelper(PropertyGridView view) {
			this.view = view;
		}
		public void OnCollectionButtonClick(RowDataBase data, object clickSource) {
			if (data.IsCollectionRow) {
				if(!View.MenuHelper.ShowCollectionMenu(data, clickSource)) {
					AddCollectionItem(data);
				}
			} else {
				RemoveCollectionItem(data);
			}
		}
		public void RemoveCollectionItem(RowDataBase rowData) {
			if (rowData == null)
				return;
			DataView.SetIsExpanded(rowData.Handle, false);
			if ((View.CellEditorOwner.ActiveEditor as DependencyObject).With(x => PropertyGridHelper.GetRowData(x)) == rowData) {
				View.CellEditorOwner.CurrentCellEditor.HideEditor(true);
			}
			DataView.RemoveCollectionItem(rowData.Handle);
		}
		public void AddCollectionItem(RowDataBase rowData, object value = null) {
			if (rowData == null)
				return;
			var resultValue = value ?? DataView.GetSelectedCollectionNewItem(rowData.Handle);
			DataView.AddCollectionNewItem(rowData.Handle, resultValue);
		}
	}
}
