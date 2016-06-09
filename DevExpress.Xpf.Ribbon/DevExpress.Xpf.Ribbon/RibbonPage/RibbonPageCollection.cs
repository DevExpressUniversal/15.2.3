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
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCollection : BaseDependencyObjectCollection<RibbonPage> {
		public RibbonPageCollection(RibbonPageCategoryBase pageCategory) {
			PageCategory = pageCategory;
		}		
		public RibbonControl Ribbon { get { return PageCategory == null ? null : PageCategory.Ribbon; } }
		RibbonPageCategoryBase pageCategory;
		public RibbonPageCategoryBase PageCategory {
			get { return pageCategory; }
			private set {
				RibbonPageCategoryBase oldValue = pageCategory;
				pageCategory = value;
				if(oldValue != value)
					OnPageCategoryChanged(oldValue, value);					
			}
		}
		void OnPageCategoryChanged(RibbonPageCategoryBase oldValue, RibbonPageCategoryBase newValue) {
		}
		protected override void OnReplaceItem(int index, RibbonPage newItem, RibbonPage oldItem) {
			bool OldItemIsSelectedValue = oldItem.IsSelected;
			OnRemoveItem(index, oldItem);
			newItem.SetCurrentValue(RibbonPage.IsSelectedProperty, OldItemIsSelectedValue || newItem.IsSelected);
			OnInsertItem(index, newItem);			
		}
		protected override void OnRemoveItem(int oldIndex, RibbonPage page) {
			if(page.PageCategory != null)
				page.PageCategory.OnPageRemoved(page, oldIndex);
			page.IsSelectedChanged -= OnPageIsSelectedCoreChanged;
			page.IsVisibleChangedWhenSelected -= OnPageIsVisibleCoreChanged;
			page.PageCategory = null;
		}
		protected override void OnInsertItem(int newIndex, RibbonPage page) {
			page.PageCategory = PageCategory;
			page.IsSelectedChanged += new EventHandler(OnPageIsSelectedCoreChanged);
			page.IsVisibleChangedWhenSelected += OnPageIsVisibleCoreChanged;
			if(page.PageCategory != null) {
				page.PageCategory.OnPageInserted(page, newIndex);		 
			}
			page.Index = newIndex;
		}		
		internal void OnPageIsSelectedCoreChanged(object sender, EventArgs e) {
			RibbonPage page = sender as RibbonPage;
			if(page.PageCategory != null) {
				page.PageCategory.OnPageIsSelectedCoreChanged(page);
			}			
		}
		internal void OnPageIsVisibleCoreChanged(object sender, EventArgs e) {
			RibbonPage page = sender as RibbonPage;
			if(page.PageCategory != null) {
				page.PageCategory.OnSelectedPageIsVisibleCoreChanged(page);
			}			
		}
	}
}
