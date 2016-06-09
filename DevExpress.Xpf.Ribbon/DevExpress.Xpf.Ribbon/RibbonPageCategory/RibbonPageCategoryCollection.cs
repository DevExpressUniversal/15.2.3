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
	public class RibbonPageCategoryCollection : ObservableCollection<RibbonPageCategoryBase> {
		RibbonControl ribbon;
		public RibbonControl Ribbon {
			get { return ribbon; }
			private set {
				RibbonControl oldValue = ribbon;
				ribbon = value;
				if(oldValue != value)
					OnRibbonControlChanged(oldValue, value);
			}
		}
		void OnRibbonControlChanged(RibbonControl oldValue, RibbonControl newValue) { }	   
		public RibbonPageCategoryCollection(RibbonControl owner) {
			Ribbon = owner;
		}
		protected override void InsertItem(int index, RibbonPageCategoryBase category) {
			if(Contains(category)) return;
			base.InsertItem(index, category);
			OnInsertItem(category, index);
			category.Index = index;
		}
		protected override void RemoveItem(int index) {
			var item = this[index];
			base.RemoveItem(index);
			OnRemoveItem(item, index);
		}
		protected override void SetItem(int index, RibbonPageCategoryBase item) {
			RibbonPageCategoryBase category = this[index];
			base.SetItem(index, item);
			OnRemoveItem(category, index);
			OnInsertItem(item, index);
		}
		protected override void ClearItems() {
			List<RibbonPageCategoryBase> removedCategories = new List<RibbonPageCategoryBase>();
			foreach(RibbonPageCategoryBase cat in this) {
				removedCategories.Add(cat);
			}
			base.ClearItems();
			for(int i = 0; i < removedCategories.Count; i++) {
				OnRemoveItem(removedCategories[i], i);
			}
		}
		protected virtual void OnRemoveItem(RibbonPageCategoryBase category, int index) {
			if(Ribbon != null)
				Ribbon.OnPageCategoryRemoved(category, index);
		}
		protected virtual void OnInsertItem(RibbonPageCategoryBase category, int index) {
			if(Ribbon != null)
				Ribbon.OnPageCategoryInserted(category, index);
		}				
		public RibbonPageCategoryBase this[string name] {
			get {
				foreach(RibbonPageCategoryBase category in this) {
					if(category.Name == name) return category;
				}
				return null;
			}
		}
	}
}
