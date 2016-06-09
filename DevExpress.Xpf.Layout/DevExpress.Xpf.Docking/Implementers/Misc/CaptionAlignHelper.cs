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
namespace DevExpress.Xpf.Docking {
	public static class CaptionAlignHelper {
		public static bool HasAffectedItems(LayoutControlItem item, CaptionAlignMode mode) {
			return (item.Parent != null) && ((mode == CaptionAlignMode.AlignInGroup) || (mode == CaptionAlignMode.Default));
		}
		public static double GetActualCaptionWidth(LayoutControlItem item) {
			double result = item.DesiredCaptionWidth;
			if(item.CaptionAlignMode == CaptionAlignMode.AutoSize || item.CaptionLocation == CaptionLocation.Top
				|| item.CaptionLocation == CaptionLocation.Bottom) return result;
			if(item.CaptionAlignMode == CaptionAlignMode.Custom)
				return double.IsNaN(item.CaptionWidth) ? result : item.CaptionWidth;
			if(item.Parent != null)
				return GetActualCaptionWidth(GetAffectedItems(item, item.CaptionAlignMode), result);
			return result;
		}
		internal static LayoutControlItem[] GetAllAffectedItems(LayoutControlItem item, CaptionAlignMode mode) {
			LayoutGroup alignRoot = GetAlignRoot(item, mode == CaptionAlignMode.AlignInGroup);
			List<LayoutControlItem> items = new List<LayoutControlItem>();
			using(IEnumerator<BaseLayoutItem> e = LayoutItemsHelper.GetEnumerator(alignRoot)) {
				while(e.MoveNext()) {
					LayoutControlItem controlItem = e.Current as LayoutControlItem;
					if(controlItem != null && controlItem.IsVisible) items.Add(controlItem);
				}
			}
			return items.ToArray();
		}
		static LayoutControlItem[] GetAffectedItems(LayoutControlItem item, CaptionAlignMode mode) {
			if(!HasAffectedItems(item, mode)) return new LayoutControlItem[0];
			LayoutGroup alignRoot = GetAlignRoot(item, mode == CaptionAlignMode.AlignInGroup);
			List<LayoutControlItem> items = new List<LayoutControlItem>();
			Predicate<BaseLayoutItem> filter = delegate(BaseLayoutItem itemToAlign) {
				return (itemToAlign.CaptionAlignMode == mode);
			};
			using(IEnumerator<BaseLayoutItem> e = LayoutItemsHelper.GetEnumerator(alignRoot, filter)) {
				while(e.MoveNext()) {
					LayoutControlItem controlItem = e.Current as LayoutControlItem;
					if(controlItem != null && controlItem.IsVisible) items.Add(controlItem);
				}
			}
			return items.ToArray();
		}
		static LayoutGroup GetAlignRoot(LayoutControlItem item, bool localAlignment) {
			LayoutGroup alignRoot = localAlignment ? item.Parent : item.GetRoot();
			if(localAlignment) {
				while(alignRoot.Parent != null) {
					if(alignRoot.Parent.CaptionAlignMode != alignRoot.CaptionAlignMode)
						break;
					alignRoot = alignRoot.Parent;
				}
			}
			return alignRoot;
		}
		static double GetActualCaptionWidth(LayoutControlItem[] items, double value) {
			double maxWidth = value;
			for(int i = 0; i < items.Length; i++) {
				LayoutControlItem controlItem = items[i];
				if(!double.IsNaN(controlItem.CaptionWidth))
					maxWidth = Math.Max(maxWidth, controlItem.CaptionWidth);
				if(controlItem.HasDesiredCaptionWidth)
					maxWidth = Math.Max(maxWidth, controlItem.DesiredCaptionWidth);
			}
			return maxWidth;
		}
		public static double GetCaptionWidth(BaseLayoutItem item, double value) {
			if(double.IsNaN(value) && item.Parent != null) {
				return GetCaptionWidth(item.Parent);
			}
			return value;
		}
		internal static double GetCaptionWidth(BaseLayoutItem item) {
			return GetCaptionWidth(item, item.CaptionWidth);
		}
		public static double GetTabCaptionWidth(BaseLayoutItem item, double value) {
			if(double.IsNaN(value) && item.Parent != null) {
				return GetTabCaptionWidth(item.Parent);
			}
			return value;
		}
		internal static double GetTabCaptionWidth(BaseLayoutItem item) {
			return GetTabCaptionWidth(item, item.TabCaptionWidth);
		}
		public static void UpdateAffectedItems(LayoutControlItem item, CaptionAlignMode mode) {
			if(!CaptionAlignHelper.HasAffectedItems(item, mode)) return;
			UpdateAffectedItemsCore(CaptionAlignHelper.GetAllAffectedItems(item, mode));
		}
		public static void UpdateAffectedItems(LayoutControlItem item, CaptionAlignMode oldMode, CaptionAlignMode newMode) {
			LayoutControlItem[] oldAffectedItems = GetAffectedItems(item, oldMode);
			UpdateAffectedItemsCore(oldAffectedItems);
			LayoutControlItem[] newAffectedItems = GetAffectedItems(item, newMode);
			for(int i = 0; i < newAffectedItems.Length; i++) {
				LayoutControlItem newItem = newAffectedItems[i];
				if(Array.IndexOf(oldAffectedItems, newItem) != -1) continue;
				newItem.CoerceValue(LayoutControlItem.ActualCaptionWidthProperty);
			}
		}
		static void UpdateAffectedItemsCore(LayoutControlItem[] affectedItems) {
			for(int i = 0; i < affectedItems.Length; i++) {
				if(affectedItems[i].HasDesiredCaptionWidth)
					affectedItems[i].CoerceValue(LayoutControlItem.ActualCaptionWidthProperty);
			}
		}
	}
}
