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

using System.Collections.Generic;
using DevExpress.Data.Summary;
using DevExpress.Data;
using System;
namespace DevExpress.Xpf.Grid {
	public class GridSummaryItemsEditorController : SummaryItemsEditorController {
		public static string GetNameBySummaryType(SummaryItemType summaryType) {
			switch(summaryType) {
				case SummaryItemType.Average: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterAverage));
				case SummaryItemType.Count: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterCount));
				case SummaryItemType.Max: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterMax));
				case SummaryItemType.Min: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterMin));
				case SummaryItemType.Sum: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterSum));
				case SummaryItemType.Custom: return GridControlLocalizer.GetString((GridControlStringId.MenuFooterCustom));
			}
			return string.Empty;
		}
		public static string GetGlobalCountSummaryName() {
			return GridControlLocalizer.GetString((GridControlStringId.MenuFooterRowCount));
		}
		internal ISummaryItemsOwner Owner { get { return ItemsOwner; } }
		public new List<SummaryEditorUIItem> UIItems { get { return base.UIItems; } }
		public GridSummaryItemsEditorController(ISummaryItemsOwner itemsOwner) : base(itemsOwner) { }
		protected override string GetTextBySummaryType(SummaryItemType summaryType) {
			return GetNameBySummaryType(summaryType);
		}
		protected override string GetSummaryItemCaption(ISummaryItem item) {
			return ((IGridSummaryItemsOwner)ItemsOwner).FormatSummaryItemCaption(item, base.GetSummaryItemCaption(item));
		}
		protected override void CreateItemWithCountSummary() { }
		protected override bool TestItemAlignment(ISummaryItem item) {
			return ((IAlignmentItem)item).Alignment == GridSummaryItemAlignment.Default;
		}
		protected override bool IsGroupSummaryItem(ISummaryItem item) {
			return TestItemAlignment(item);
		}
		public virtual bool HasFixedCountSummary() {			
			return FindSummaryItem("", SummaryItemType.Count, Items) != null; 
		}
	}
	public class GridSummaryPanelItemsEditorController : GridSummaryItemsEditorController {
		public GridSummaryPanelItemsEditorController(ISummaryItemsOwner itemsOwner) : base(itemsOwner) { }
		public override void RemoveSummary(string fieldName, SummaryItemType summaryType) {
			if(summaryType != SummaryItemType.Count) {
				base.RemoveSummary(fieldName, summaryType);
				return;
			}
			IList<ISummaryItem> items = FindSummaryItems(summaryType, Items);
			foreach(ISummaryItem item in items) {
				Items.Remove(item);
			}
		}
		public override void AddSummary(string fieldName, SummaryItemType summaryType) {
			if(summaryType != SummaryItemType.Count) {
				base.AddSummary(fieldName, summaryType);
				return;
			}
			if(FindSummaryItems(summaryType, Items).Count == 0) {
				IList<ISummaryItem> items = FindSummaryItems(summaryType, InitialItems);
				foreach(ISummaryItem item in items) {
					Items.Add(item);
				}
				if(items.Count == 0)
					Items.Add(ItemsOwner.CreateItem(fieldName, summaryType));
			}
		}
		IList<ISummaryItem> FindSummaryItems(SummaryItemType summaryType, List<ISummaryItem> list) {
			IList<ISummaryItem> res = new List<ISummaryItem>();
			foreach(IAlignmentItem item in list) {
				if(TestItemAlignment(item) && item.SummaryType == summaryType && IsGroupSummaryItem(item))
					res.Add(item);
			}
			return res;
		}
		protected override bool TestItemAlignment(ISummaryItem item) {
			return ((IAlignmentItem)item).Alignment != GridSummaryItemAlignment.Default;
		}
		public override bool HasFixedCountSummary() {
			return HasSummary(SummaryItemType.Count);
		}
		public override bool HasSummary(SummaryItemType summaryType) {
			foreach(IAlignmentItem item in Items) {
				if(item.SummaryType != summaryType)
					continue;
				if(item.Alignment != GridSummaryItemAlignment.Default)
					return true;
			}
			return false;
		}
	}
}
