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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class SelectionStrategy {
		public bool IsLockedBySelectionChanging { get { return selectionLocker.IsLocked; } }
		PropertyGridControl Grid { get; set; }
		RowDataGenerator RowDataGenerator { get { return Grid.RowDataGenerator; } }
		DataViewBase DataView { get { return Grid.DataView; } }
		readonly Locker selectionPublicLocker = new Locker();
		readonly Locker selectionLocker = new Locker();
		readonly Locker selectionGlobalLocker = new Locker();
		public void Lock() {
			selectionGlobalLocker.Lock();
		}
		public void Unlock() {
			selectionGlobalLocker.Unlock();
		}
		public SelectionStrategy(PropertyGridControl grid) {
			Grid = grid;
		}
		public void SelectViaPath(string selectedPropertyPath, bool immediate = false) {
			LogBase.Add(Grid, selectedPropertyPath);
			if (selectionGlobalLocker.IsLocked)
				return;			
			selectionPublicLocker.DoLockedActionIfNotLocked(() => {
				Grid.SelectedPropertyPath = selectedPropertyPath;				
			});
			if (immediate)
				UpdateSelectedValue();		   
			try {
				if (!selectionLocker.IsLocked && !selectionPublicLocker.IsLocked) {
					selectionLocker.Lock();
					RowDataGenerator.SetSelection(selectedPropertyPath, immediate, () => selectionLocker.Unlock());
				}
			} catch (Exception) {
				selectionLocker.Unlock();
				throw;
			}
		}
		public void UpdateSelectedValue() {
			selectionPublicLocker.DoLockedActionIfNotLocked(() => {
				Grid.SelectedPropertyValue = Grid.SelectedPropertyPath.With(DataView.GetHandleByFieldName).With(DataView.GetValue);
			});
		}
		public void SelectViaHandle(RowHandle rowHandle) {
			SelectViaPath(DataView.GetFieldNameByHandle(rowHandle), true);
		}
	}
}
