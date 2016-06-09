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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Core {
	public interface ISupportVisibleIndex {
		int VisibleIndex { get; }
	}
	public interface IOrderPanelElement {
		int VisibleIndex { get; set; }
	}
	public class CachedItemsControl : ItemsControlBase {
		Dictionary<object, FrameworkElement> cache = new Dictionary<object, FrameworkElement>();
		Guid lastCacheVersion = Guid.Empty;
		bool shouldPurgeCache;
		protected override Size MeasureOverride(Size constraint) {
			ValidateCache();
			return base.MeasureOverride(constraint);
		}
		protected override void ValidateVisualTree() {
			if(ItemsSource != null) {
				int count = 0;
				Dictionary<object, FrameworkElement> validKeys = null;
				Dictionary<object, FrameworkElement> oldKeys = null;
				if(shouldPurgeCache) {
					validKeys = new Dictionary<object, FrameworkElement>();
					oldKeys = new Dictionary<object, FrameworkElement>(cache);
				}
				foreach(object item in ItemsSource) {
					FrameworkElement presenter;
					if(cache.TryGetValue(item, out presenter) && CanUseElementFromCache(presenter)) {
						if(shouldPurgeCache)
							validKeys.Add(item, null);
					} else {
						presenter = CreateChild(item);
						cache[item] = presenter;
					}
					if(item is ISupportVisibleIndex)
						OrderPanelBase.SetVisibleIndex(presenter, ((ISupportVisibleIndex)item).VisibleIndex);
					int index = Panel.Children.IndexOf(presenter);
					if(index >= 0)
						Panel.Children.RemoveRange(count, index - count);
					else {
						Panel parent = (Panel)System.Windows.Media.VisualTreeHelper.GetParent(presenter);
						if(parent != null)
							parent.Children.Remove(presenter);
						Panel.Children.Insert(count, presenter);
					}
					count++;
					ValidateElement(presenter, item);
				}
				Panel.Children.RemoveRange(count, Panel.Children.Count - count);
				if(shouldPurgeCache) {
					foreach(object key in oldKeys.Keys) {
						if(!validKeys.ContainsKey(key) && oldKeys.ContainsKey(key))
							cache.Remove(key);
					}
					shouldPurgeCache = false;
				}
			}
		}
		protected virtual bool CanUseElementFromCache(FrameworkElement element) {
			return true;
		}
		protected virtual void ValidateElement(FrameworkElement element, object item) { 
		}
		void ValidateCache() {
			ISupportCacheVersion cacheVersionItemsSource = ItemsSource as ISupportCacheVersion;
			if (cacheVersionItemsSource != null && cacheVersionItemsSource.CacheVersion != lastCacheVersion) {
				ClearCache();
				lastCacheVersion = cacheVersionItemsSource.CacheVersion;
			}
		}
		protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			base.OnItemsSourceChanged(e);
			if(ItemsSource == null || !(ItemsSource is ISupportCacheVersion))
				shouldPurgeCache = true;
		}
		void ClearCacheAndInvalidateMeasure() {
			ClearCache();
			InvalidateMeasure();
		}
		void ClearCache() {
			cache.Clear();
		}
		protected override void OnItemTemplateChanged() {
			base.OnItemTemplateChanged();
			ClearCacheAndInvalidateMeasure();
		}
	}
}
