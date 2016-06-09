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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public abstract class ChooseObjectPageBase<T, TItemsSource> : DataSourceWizardPage where T : class {
		readonly Func<T, bool> isHighlightedFunc;
		readonly Func<IEnumerable<T>, TItemsSource> getItemsSourceFunc;
		protected ChooseObjectPageBase(Func<T, bool> isHighlightedFunc, Func<IEnumerable<T>, TItemsSource> getItemsSourceFunc, DataSourceWizardModelBase model)
			: base(model) {
			this.isHighlightedFunc = isHighlightedFunc;
			this.getItemsSourceFunc = getItemsSourceFunc;
		}
		public virtual T SelectedItem { get; set; }
		protected void OnSelectedItemChanged() {
			if(SelectedItem != null) {
				SelectedObject = SelectedItem;
				Result = SelectedItem;
			}
			RaiseChanged();
		}
		public virtual object SelectedObject { get; set; }
		protected void OnSelectedObjectChanged() {
			SelectedItem = SelectedObject as T;
		}
		public virtual T Result { get; set; }
		protected void OnResultChanged() {
			SelectedItem = Result;
		}
		[RaiseChanged]
		public virtual bool ShowAll { get; set; }
		protected void OnShowAllChanged() {
			UpdateItems();
		}
		public virtual bool EnableShowAll { get; protected set; }
		public virtual TItemsSource Items { get; protected set; }
		void UpdateItems() {
			Items = ShowAll ? allItems : highlightedItems;
		}
		TItemsSource allItems;
		TItemsSource highlightedItems;
		public void Initialize(IEnumerable<T> items, bool showAll) {
			SetData(items, showAll);
		}
		public void SetData(IEnumerable<T> items, bool showAll) {
			var allItemsList = items.ToList().AsReadOnly();
			allItems = getItemsSourceFunc(allItemsList);
			var highlightedItemsList = allItemsList.Where(isHighlightedFunc).ToList().AsReadOnly();
			highlightedItems = getItemsSourceFunc(highlightedItemsList);
			if(!highlightedItemsList.Any()) {
				EnableShowAll = false;
				ShowAll = true;
			} else {
				EnableShowAll = highlightedItemsList.Count != allItemsList.Count;
				ShowAll = !EnableShowAll || showAll;
			}
			UpdateItems();
		}
	}
}
