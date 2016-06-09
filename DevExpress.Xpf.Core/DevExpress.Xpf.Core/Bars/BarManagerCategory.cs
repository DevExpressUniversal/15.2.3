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
using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
namespace DevExpress.Xpf.Bars {
	public class BarManagerCategoryCollection : ObservableCollection<BarManagerCategory> {
		BarManager manager;
		public BarManagerCategoryCollection(BarManager manager) {
			this.manager = manager;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryCollectionManager")]
#endif
public BarManager Manager { get { return manager; } }
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if(e.Action == NotifyCollectionChangedAction.Add) {
				foreach(BarManagerCategory cat in e.NewItems) {
					cat.ScopeNode = Manager;
				}
			}
			else if(e.Action == NotifyCollectionChangedAction.Remove) {
				foreach(BarManagerCategory cat in e.OldItems) {
					cat.ScopeNode = null;
				}
			}
		}
		public BarManagerCategory this[string name] {
			get {
				foreach(BarManagerCategory cat in this) {
					if(cat.Name == name) return cat;
				}
				return null;
			}
		}
		protected override void ClearItems() {
			while (Count > 0)
				RemoveAt(0);
		}
		BarManagerCategory unassignedItems;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryCollectionUnassignedItems")]
#endif
public BarManagerCategory UnassignedItems {
			get {
				if(unassignedItems == null)
					unassignedItems = CreateUnassignedItemsCategory(Manager);	
				return unassignedItems;
			}
		}
		protected internal static BarManagerCategory CreateUnassignedItemsCategory(BarManager manager) {
			BarManagerCategory res = new BarManagerCategory();
			if (manager != null)
				manager.AddLogicalChild(res);
			res.Name = "Unassigned Items";
			res.ScopeNode = manager;
			res.IsSpecialCategory = true;
			res.Caption = BarsLocalizer.GetString(BarsStringId.BarManagerCategory_UnassignedCategoryCaption);
			return res;
		}
		AllItemsBarManagerCategory allItems;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryCollectionAllItems")]
#endif
public AllItemsBarManagerCategory AllItems {
			get {
				if(allItems == null)
					allItems = CreateAllItemsCategory(Manager);
				return allItems;
			}
		}
		protected internal static AllItemsBarManagerCategory CreateAllItemsCategory(BarManager manager) {
			AllItemsBarManagerCategory res = new AllItemsBarManagerCategory();
			if (manager != null)
				manager.AddLogicalChild(res);
			res.Name = "All Items";
			res.ScopeNode = manager;
			res.IsSpecialCategory = true;
			res.Caption = BarsLocalizer.GetString(BarsStringId.BarManagerCategory_AllItemsCategoryCaption);
			return res;
		}
	}
	public class BarManagerCategory : DependencyObject {
		#region static
		public static readonly DependencyProperty NameProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty VisibleProperty;
		static BarManagerCategory() {
			NameProperty = DependencyPropertyManager.Register("Name", typeof(string), typeof(BarManagerCategory), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.None));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), typeof(BarManagerCategory), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
			VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), typeof(BarManagerCategory), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnVisiblePropertyChanged)));			
		}		
		protected static void OnVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarManagerCategory)d).OnVisibleChanged(e);
		}
		#endregion
		public BarManagerCategory() {
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryName")]
#endif
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryVisible")]
#endif
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerCategoryCaption")]
#endif
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		protected internal bool IsSpecialCategory { get; set; }
		protected internal DependencyObject ScopeNode { get; set; }
		public virtual List<BarItem> GetBarItems() {
			List<BarItem> res = new List<BarItem>();
			foreach (BarItem item in BarNameScope.GetService<IElementRegistratorService>(ScopeNode).GetElements<BarItem>()) {
				if (item.CategoryName == Name) res.Add(item);
			}
			return res;
		}		
		protected virtual void UpdateItems() {
			List<BarItem> res = GetBarItems();
			foreach(BarItem item in res)
				item.UpdateProperties();
		}
		protected virtual void OnVisibleChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItems();
		}
	}
	public class AllItemsBarManagerCategory : BarManagerCategory {
		public override List<BarItem> GetBarItems() {
			return BarNameScope.GetService<IElementRegistratorService>(ScopeNode).GetElements<BarItem>().ToList();			
		}
	}
}
