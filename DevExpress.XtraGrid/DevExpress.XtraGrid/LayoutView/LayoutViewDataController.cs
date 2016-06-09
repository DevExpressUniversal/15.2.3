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
using System.Text;
using DevExpress.Data.Selection;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout;
using System.ComponentModel;
namespace DevExpress.XtraGrid.Views.Layout {
	public interface ILayoutViewDataController {
		CardDifferencesController CardDifferences { get;}
		CardSelectionController Selection { get; }
		bool IsArrangeLocked { get;}
	}
	public class LayoutViewServerModeDataController : CardViewServerModeDataController, ILayoutViewDataController {
		int lockCounter = 0;
		CardDifferencesController differencesControllerCore = null;
		public LayoutViewServerModeDataController()
			: base() {
			differencesControllerCore = CreateDifferencesController();
		}
		bool ILayoutViewDataController.IsArrangeLocked { get { return lockCounter>0; } }
		CardDifferencesController ILayoutViewDataController.CardDifferences { get { return differencesControllerCore; } }
		CardSelectionController ILayoutViewDataController.Selection { get { return base.Selection as CardSelectionController; } }
		protected virtual CardDifferencesController CreateDifferencesController() { return new CardDifferencesController(this); }
	}
	public delegate void DelayedOperation();
	public class LayoutViewDataControllerRegularNoCurrencyManager : GridDataController, ILayoutViewDataController {
		int lockCounter = 0;
		CardDifferencesController differencesControllerCore;
		public LayoutViewDataControllerRegularNoCurrencyManager() {
			differencesControllerCore = CreateDifferencesController();
		}
		protected override SelectionController CreateSelectionController() {
			return new CardSelectionController(this);
		}
		bool ILayoutViewDataController.IsArrangeLocked { get { return lockCounter>0; } }
		CardDifferencesController ILayoutViewDataController.CardDifferences { get { return differencesControllerCore; } }
		CardSelectionController ILayoutViewDataController.Selection { get { return base.Selection as CardSelectionController; } }
		protected virtual CardDifferencesController CreateDifferencesController() { return new CardDifferencesController(this); }
		protected override void OnBindingListChangedCore(System.ComponentModel.ListChangedEventArgs e) {
			lockCounter++;
			base.OnBindingListChangedCore(e);
			--lockCounter;
		}
	}
	public class LayoutViewDataController : CardViewDataController, ILayoutViewDataController {
		int lockCounter = 0;
		CardDifferencesController differencesControllerCore = null;
		public LayoutViewDataController()
			: base() {
			differencesControllerCore = CreateDifferencesController();
		}
		bool ILayoutViewDataController.IsArrangeLocked { get { return lockCounter > 0; } }
		CardDifferencesController ILayoutViewDataController.CardDifferences { get { return differencesControllerCore; } }
		CardSelectionController ILayoutViewDataController.Selection { get { return base.Selection as CardSelectionController; } }
		protected virtual CardDifferencesController CreateDifferencesController() { return new CardDifferencesController(this); }
		protected override void OnBindingListChangedCore(System.ComponentModel.ListChangedEventArgs e) {
			lockCounter++;
			base.OnBindingListChangedCore(e);
			--lockCounter;
		}
	}
	public class CardDifferencesController : CurrencySelectionController {
		CardDifferencesCollection rowDifferences;
		public CardDifferencesController(DataController controller)
			: base(controller) {
			this.rowDifferences = new CardDifferencesCollection(this);
		}
		public LayoutViewCardDifferences GetCardDifferences(int rowHandle) {
			return rowDifferences.GetRowSelectedObject(rowHandle) as LayoutViewCardDifferences;
		}
		public void CheckCardDifferences(LayoutViewCard card, LayoutViewCard template) {
			LayoutViewCardDifferences s = CardDifferenceController.CreateNormalizedDifferences(card, template);
			if(s.HasDifferences) SetCardDifferences(card.RowHandle, s);
			else ResetCardDifferences(card.RowHandle);
		}
		public void SetCardDifferences(int rowHandle, LayoutViewCardDifferences diffs) {
			rowDifferences.SetRowSelected(rowHandle, false, null);
			rowDifferences.SetRowSelected(rowHandle, true, diffs);
		}
		public void ResetCardDifferences(int rowHandle) {
			rowDifferences.SetRowSelected(rowHandle, false, null);
		}
		public void ResetAllDifferences() {
			rowDifferences.Clear();
		}
	}
	public class CardDifferencesCollection : SelectedRowsCollection {
		public CardDifferencesCollection(CardDifferencesController differenceController) : base(differenceController) { }
	}
	public enum LayoutItemDifferenceType { SelectedTabIndex, GroupExpanded, ItemVisibility }
	public class LayoutItemDifferences {
		Dictionary<LayoutItemDifferenceType, object> itemDifferencePairsCore;
		public LayoutItemDifferences() {
			itemDifferencePairsCore = new Dictionary<LayoutItemDifferenceType, object>();
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutItemDifferencesPairs")]
#endif
		public Dictionary<LayoutItemDifferenceType, object> Pairs {
			get { return itemDifferencePairsCore; }
		}
		public bool IsEqual(LayoutItemDifferences differences) {
			bool fEqual = (differences.Pairs.Count==Pairs.Count);
			if(fEqual) {
				foreach(LayoutItemDifferenceType differenceType in Pairs.Keys) {
					switch(differenceType) {
						case LayoutItemDifferenceType.GroupExpanded:
						case LayoutItemDifferenceType.ItemVisibility:
							fEqual &= ((bool)Pairs[differenceType] == (bool)differences.Pairs[differenceType]);
							break;
						case LayoutItemDifferenceType.SelectedTabIndex:
							fEqual &= ((int)Pairs[differenceType] == (int)differences.Pairs[differenceType]);
							break;
					}
				}
			}
			return fEqual;
		}
		public object this[LayoutItemDifferenceType type] {
			get { return Pairs.ContainsKey(type) ? Pairs[type] : null; }
			set {
				if(Pairs.ContainsKey(type)) Pairs[type] = value;
				else Pairs.Add(type, value);
			}
		}
	}
	public class LayoutViewCardDifferences{
		Dictionary<string, LayoutItemDifferences> differencesCore = null;
		public LayoutViewCardDifferences() {
			this.differencesCore = new Dictionary<string, LayoutItemDifferences>();
		}
		public LayoutViewCardDifferences(LayoutViewCard card)
			: this() {
			card.Accept(new DifferencesBuilder(this));
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewCardDifferencesDifferences")]
#endif
public Dictionary<string, LayoutItemDifferences> Differences {
			get { return differencesCore; }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewCardDifferencesHasDifferences")]
#endif
public bool HasDifferences { get { return Differences.Count > 0; } }
		public void ClearDifferences() {
			Differences.Clear();
		}
		public bool IsEqual(LayoutViewCardDifferences d) {
			return AreDifferencesEqual(d);
		}
		protected bool AreDifferencesEqual(LayoutViewCardDifferences diffs) {
			if(diffs.Differences.Count!=Differences.Count) return false;
			bool result = true;
			foreach(string itemName in Differences.Keys) {
				if(diffs.Differences.ContainsKey(itemName)) {
					result &= Differences[itemName].IsEqual(diffs.Differences[itemName]);
				}
				else return false;
			}
			return result;
		}
		public LayoutItemDifferences this[string name] {
			get { return Differences.ContainsKey(name) ? Differences[name] : null; }
		}
		protected void AddItemDifferences(string itemName) {
			if(!Differences.ContainsKey(itemName)) Differences.Add(itemName, new LayoutItemDifferences());
		}
		public void AddItemDifference(string itemName, LayoutItemDifferenceType type,object value) {
			if(this[itemName]==null) AddItemDifferences(itemName);
			this[itemName][type] = value;
		}
		public void RemoveItemDifference(string itemName, LayoutItemDifferenceType type) {
			if(this[itemName]==null || !this[itemName].Pairs.ContainsKey(type)) return;
			this[itemName].Pairs.Remove(type);
		}
		public void RemoveItemDifferences(string itemName) {
			if(Differences.ContainsKey(itemName)) Differences.Remove(itemName);
		}
	}
	public class CardDifferenceController {
		public static LayoutViewCardDifferences CreateTemplateDifferences(LayoutViewCard template) {
			return new LayoutViewCardDifferences(template);
		}
		protected static LayoutViewCardDifferences CheckDifferences(LayoutViewCard card, LayoutViewCard template) {
			LayoutViewCardDifferences diffs = new LayoutViewCardDifferences(card);
			LayoutViewCardDifferences templateDiffs = CreateTemplateDifferences(template);
			foreach(string itemName in templateDiffs.Differences.Keys) {
				if(diffs.Differences[itemName].IsEqual(templateDiffs.Differences[itemName]))
					diffs.RemoveItemDifferences(itemName);
			}
			return diffs;
		}
		public static LayoutViewCardDifferences CreateNormalizedDifferences(LayoutViewCard card, LayoutViewCard template) {
			return CheckDifferences(card, template);
		}
		public static void ApplyDifferences(LayoutViewCard card, LayoutViewCardDifferences diffs) {
			if(diffs!=null) card.Accept(new DifferencesApplier(diffs));
		}
		public static void ResetDifferences(LayoutViewCard card, LayoutViewCard template) {
			LayoutViewCardDifferences templateDifferences = CreateTemplateDifferences(template);
			ApplyDifferences(card, templateDifferences);
		}
	}
	public class DifferencesBuilder : BaseVisitor {
		LayoutViewCardDifferences differencesCore = null;
		public DifferencesBuilder(LayoutViewCardDifferences diffs)
			: base() {
			this.differencesCore = diffs;
			Differences.ClearDifferences();
		}
		protected LayoutViewCardDifferences Differences { get { return differencesCore; } }
		public override void Visit(BaseLayoutItem item) {
			base.Visit(item);
			LayoutItemDifferences differences  = new LayoutItemDifferences();
			differences.Pairs.Add(LayoutItemDifferenceType.ItemVisibility, (item.Visibility != LayoutVisibility.Never));
			LayoutControlGroup group = item as LayoutControlGroup;
			if(group!=null) {
				differences.Pairs.Add(LayoutItemDifferenceType.GroupExpanded, group.Expanded);
			}
			TabbedControlGroup tab = item as TabbedControlGroup;
			if(tab!=null) {
				differences.Pairs.Add(LayoutItemDifferenceType.SelectedTabIndex, tab.SelectedTabPageIndex);
			}
			if (Differences.Differences.ContainsKey(item.Name)) {
				throw new NotImplementedException("Internal problem");
			}
			Differences.Differences.Add(item.Name, differences);
		}
	}
	public class DifferencesApplier : BaseVisitor {
		LayoutViewCardDifferences differencesCore = null;
		public DifferencesApplier(LayoutViewCardDifferences diffs)
			: base() {
			this.differencesCore = diffs;
		}
		protected LayoutViewCardDifferences Differences { get { return differencesCore; } }
		public override void Visit(BaseLayoutItem item) {
			base.Visit(item);
			if(Differences.Differences.ContainsKey(item.Name)) {
				LayoutItemDifferences differences  = Differences.Differences[item.Name];
				LayoutControlGroup group = item as LayoutControlGroup;
				TabbedControlGroup tab = item as TabbedControlGroup;
				foreach(LayoutItemDifferenceType differenceType in differences.Pairs.Keys) {
					switch(differenceType) {
						case LayoutItemDifferenceType.GroupExpanded:
							if(group!=null) {
								bool expanded = (bool)differences.Pairs[differenceType];
								if(group.Expanded!=expanded) group.Expanded = expanded;
							}
							break;
						case LayoutItemDifferenceType.SelectedTabIndex:
							if(tab!=null) {
								int index = (int)differences.Pairs[differenceType];
								if(tab.SelectedTabPageIndex !=index) tab.SelectedTabPageIndex=index;
							}
							break;
						case LayoutItemDifferenceType.ItemVisibility:
							LayoutVisibility visibility = ((bool)differences.Pairs[differenceType]) ?
								LayoutVisibility.Always : LayoutVisibility.Never;
							if(item.Visibility != visibility) item.Visibility = visibility;
							break;
					}
				}
			}
		}
	}
}
