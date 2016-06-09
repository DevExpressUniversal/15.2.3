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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.TabControlAutomation {
	internal class DXTabControlAutomationStringProvider {
		internal const string cannotScrollToDirection = "Can not scroll to unsupported direcetion";
		internal const string scrollPercentIsOutOfRange = "Scroll percent is out of range";
		internal const string tabControlDoesNotAllowMultiSelection = "DXTabControl does not allow multiselection";
		internal const string attemptToRemoveFromSelection = "You cannot remove item from selection because it is required for DXTabControl";
		internal static bool disableAnimationCore = false;
	}
	public class DXTabControlAutomationPeer :DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IScrollProvider, ISelectionProvider {
		DXTabControl TabControl { get { return base.Owner as DXTabControl; } }
		TabControlViewBase View { get { return TabControl.With(x => x.View); } }
		TabPanelScrollView ScrollPanel { get { return View.With(x => x.ScrollView).With(x => x.ScrollPanel); } }
		public DXTabControlAutomationPeer(DXTabControl ownerCore) : base(ownerCore) {
			if (TabControl != null) {
				TabControl.SelectionChanged += new TabControlSelectionChangedEventHandler(OnSelectionChanged);
				TabControl.ItemsChanged += OnItemsChanged;
			}
		}
		void OnItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
#if !SL
			ResetChildrenCache();
#endif
			RaiseAutomationEvent(AutomationEvents.StructureChanged);
		}
		void OnSelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			DXTabItem oldItem = null;
			DXTabItem newItem = null;
			if(TabControl != null) {
				oldItem = e.OldSelectedItem == null ? null : TabControl.ItemContainerGenerator.ContainerFromItem(e.OldSelectedItem) as DXTabItem;
				newItem = e.NewSelectedItem == null ? null : TabControl.ItemContainerGenerator.ContainerFromItem(e.NewSelectedItem) as DXTabItem;
			}
			DXTabItemAutomationPeer oldPeer = null;
			DXTabItemAutomationPeer newPeer = null;
			if(oldItem != null)
				oldPeer = CreatePeerForElement(oldItem) as DXTabItemAutomationPeer;
			if(newItem != null)
				newPeer = CreatePeerForElement(newItem) as DXTabItemAutomationPeer;
			if(oldPeer != null) {
				oldPeer.RaiseAutomationEvent(AutomationEvents.StructureChanged);				
			}
			if(newPeer != null) {
				newPeer.RaiseAutomationEvent(AutomationEvents.StructureChanged);				
			}
			RaiseAutomationEvent(AutomationEvents.StructureChanged);
			if(oldPeer!=newPeer){
				if(newPeer != null) {
					newPeer.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
				}
				if(oldPeer != null) {
					oldPeer.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
				}
#if !SL                
				IRawElementProviderSimple newProvider = null;				
				if(newPeer != null)
					newProvider = ProviderFromPeer(newPeer);
				if(newProvider!= null)
					AutomationInteropProvider.RaiseAutomationEvent(SelectionItemPatternIdentifiers.ElementSelectedEvent, newProvider, new AutomationEventArgs(SelectionItemPatternIdentifiers.ElementSelectedEvent));				
#endif
			}
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			for(int i = 0; i<TabControl.Items.Count; i++){
				object obj = TabControl.ItemContainerGenerator.ContainerFromIndex(i);
				if(obj as FrameworkElement != null) {
					AutomationPeer peer = CreatePeerForElement(obj as FrameworkElement);
					if(peer != null)
						children.Add(peer);
				}
			}
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "DXTabControl" + Convert.ToString(TabControl.Name);
		}
		protected override string GetClassNameCore() {
			return typeof(DXTabControl).Name;
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);			
			return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(TabControl, this, useAttachedValue? (string)value : null);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Tab;
		}
		#region IScrollProvider        
		bool IsHorizontallyOriented { get { return !IsVerticallyOriented; } }
		bool IsVerticallyOriented { get { return View.HeaderLocation == HeaderLocation.Left || View.HeaderLocation == HeaderLocation.Right; } }
		bool CanScrollCore { get { return ScrollPanel.Return(x => x.CanScroll && x.MaxOffset != 0, () => false); } }
		double ScrollPercent { get { return CanScrollCore ? ((ScrollPanel.Offset / ScrollPanel.MaxOffset) * 100) : -1d; } }
		double ViewSizeCore {
			get { return CanScrollCore && ScrollPanel.PanelLength != 0 ? ((ScrollPanel.MaxOffset / ScrollPanel.PanelLength) * 100) : 100; }
		}
		public double HorizontalScrollPercent {
			get { return IsHorizontallyOriented ? ScrollPercent : -1d; }
		}
		public double HorizontalViewSize {
			get { return IsHorizontallyOriented ? ViewSizeCore : 100; }
		}
		public bool HorizontallyScrollable {
			get { return IsHorizontallyOriented && CanScrollCore; }
		}
		public double VerticalScrollPercent {
			get { return IsVerticallyOriented ? ScrollPercent : -1d; }
		}
		public double VerticalViewSize {
			get { return IsVerticallyOriented ? ViewSizeCore : 100; }
		}
		public bool VerticallyScrollable {
			get { return IsVerticallyOriented && CanScrollCore; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if((!VerticallyScrollable && verticalAmount != ScrollAmount.NoAmount)
				|| (!HorizontallyScrollable && horizontalAmount != ScrollAmount.NoAmount))
				throw new InvalidOperationException(DXTabControlAutomationStringProvider.cannotScrollToDirection);
			if(!CanScrollCore) return;
			double oldPercent = ScrollPercent;
			ScrollAmount amount = HorizontallyScrollable ? horizontalAmount : verticalAmount;
			int index = ScrollPanel.GetFirstFullVisibleItemIndex();
			if(index == -1) return;
			switch(amount) {
				case ScrollAmount.NoAmount: return;
				case ScrollAmount.SmallDecrement:
					if(index<=0) return;
					double decOffset = ScrollPanel.GetHeaderOffset(index) - ScrollPanel.GetHeaderOffset(index - 1);
					DecreaseOffset(decOffset);
					break;
				case ScrollAmount.SmallIncrement:
					if(index >= TabControl.Items.Count - 1) return;
					double incOffset = ScrollPanel.GetHeaderOffset(index + 1) - ScrollPanel.Offset;
					IncreaseOffset(incOffset);
					break;
				case ScrollAmount.LargeIncrement:
					if(index >= TabControl.Items.Count - 1) return;
					IncreaseOffset(ScrollPanel.VisibleLength);
					break;
				case ScrollAmount.LargeDecrement:
					if(index<=0) return;
					DecreaseOffset(ScrollPanel.VisibleLength);					
					break;
			}
			if(oldPercent != ScrollPercent)
				RaisePropertyChangedEvent(HorizontallyScrollable ? ScrollPatternIdentifiers.HorizontalScrollPercentProperty : ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldPercent, ScrollPercent);
		}
		private void IncreaseOffset(double offset) {
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = true;
			ScrollPanel.Offset
				= ScrollPanel.MaxOffset > ScrollPanel.Offset + offset
				? ScrollPanel.Offset + offset
				: ScrollPanel.MaxOffset;
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = false;
		}
		private void DecreaseOffset(double offset) {
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = true;
			ScrollPanel.Offset
				= ScrollPanel.MinOffset <= ScrollPanel.Offset - offset
				? ScrollPanel.Offset - offset
				: ScrollPanel.MinOffset;
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = false;
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if((!VerticallyScrollable && !(verticalPercent == 0 || verticalPercent == -1))
				|| (!HorizontallyScrollable && !(horizontalPercent == 0 || horizontalPercent == -1)))
				throw new InvalidOperationException(DXTabControlAutomationStringProvider.cannotScrollToDirection);
			if((VerticallyScrollable && ((verticalPercent < 0 && verticalPercent != -1)
				|| verticalPercent > 100)) || (HorizontallyScrollable && ((horizontalPercent < 0 && horizontalPercent != -1) || horizontalPercent > 100)))
				throw new IndexOutOfRangeException(DXTabControlAutomationStringProvider.scrollPercentIsOutOfRange);
			if(!VerticallyScrollable && !HorizontallyScrollable) return;
			double oldPercent = HorizontallyScrollable ? HorizontalScrollPercent : VerticalScrollPercent;
			double percent = HorizontallyScrollable ? horizontalPercent : verticalPercent;
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = true;
			ScrollPanel.Offset = (percent / 100) * ScrollPanel.MaxOffset;
			if(DXTabControlAutomationStringProvider.disableAnimationCore)
				ScrollPanel.disableAnimation = false;
			RaisePropertyChangedEvent(HorizontallyScrollable ? ScrollPatternIdentifiers.HorizontalScrollPercentProperty : ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldPercent, percent);
		}		
		#endregion
		#region ISelectionProvider
		public bool CanSelectMultiple {
			get { return false; }
		}
		public IRawElementProviderSimple[] GetSelection() {
			if(TabControl.SelectedContainer == null) return null;
			AutomationPeer selectedTabItemPeer = CreatePeerForElement(TabControl.SelectedContainer);
			if(selectedTabItemPeer == null) return null;
			IRawElementProviderSimple provider = ProviderFromPeer(selectedTabItemPeer);
			if(provider == null) return null;
			return new IRawElementProviderSimple[] { provider };
		}
		public bool IsSelectionRequired {
#if !SL
			get { return TabControl.HasItems; }
#else
			get { return OwnerCore.Items.Count!=0; }
#endif 
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Selection)
				return this;
			if(patternInterface == PatternInterface.Scroll)
				return this;
			return null;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>TabControl
			};
		}
	}
	public class DXTabItemAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IScrollItemProvider, ISelectionItemProvider {
		DXTabItem TabItem { get { return Owner as DXTabItem; } }
		DXTabControl TabControl { get { return TabItem.With(x => x.Owner); } }
		TabControlViewBase View { get { return TabControl.With(x => x.View); } }
		TabPanelScrollView ScrollPanel { get { return View.With(x => x.ScrollView).With(x => x.ScrollPanel); } }
		public DXTabItemAutomationPeer(DXTabItem ownerCore) : base(ownerCore) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			if (TabItem.IsSelected) {
				DependencyObject visualObject = null;
				if (TabControl.TabContentCacheMode == TabContentCacheMode.None) {
					if(TabControl==null || TabControl.ContentPresenter==null) return children;
					int childrenCount = VisualTreeHelper.GetChildrenCount(TabControl.ContentPresenter);
					DependencyObject child = childrenCount==0 ? null : VisualTreeHelper.GetChild(TabControl.ContentPresenter,0);
					visualObject = child == null ? TabControl.ContentPresenter : child;
				} else {
					if (TabControl == null || TabControl.FastRenderPanel == null || TabControl.FastRenderPanel.SelectedItem == null)
						return children;
					int childrenCount = VisualTreeHelper.GetChildrenCount(TabControl.FastRenderPanel.SelectedItem);
					DependencyObject child = childrenCount == 0 ? null : VisualTreeHelper.GetChild(TabControl.FastRenderPanel.SelectedItem, 0);
					visualObject = child == null ? TabControl.FastRenderPanel.SelectedItem : child;
				}
				if (visualObject == null) return children;
				DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.GetChildrenRecursive(children, visualObject as DependencyObject);
			}
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "DXTabItem" + Convert.ToString(TabItem.Header);
		}
		protected override string GetClassNameCore() {
			return typeof(DXTabItem).Name;
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(TabItem, this, useAttachedValue ? (string)value : null);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TabItem;
		}
		#region IScrollItemProvider
		public void ScrollIntoView() {
			if(TabControl == null || ScrollPanel == null || !ScrollPanel.CanScroll) return;
			int index = TabControl.IndexOf(TabItem);
			if(index != -1 && ScrollPanel.CanScrollTo(index))
				ScrollPanel.ScrollTo(index);
		}
		#endregion
		#region ISelectionItemProvider
		public void AddToSelection() {
			if(TabControl == null) return;
			AutomationPeer peer = CreatePeerForElement(TabControl);
			if(peer != null) {
				ISelectionProvider provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
				if(provider != null) {
					if(!provider.CanSelectMultiple && TabControl.SelectedContainer != null)
						throw new InvalidOperationException(DXTabControlAutomationStringProvider.tabControlDoesNotAllowMultiSelection);
				}
			}
			SelectCore(AutomationEvents.SelectionItemPatternOnElementSelected);			
		}
		public bool IsSelected {
			get { return TabControl == null ? false : TabControl.SelectedItem == TabItem; }
		}
		public void RemoveFromSelection() {			
			bool isSelectedCore = IsSelected;
			if(TabControl == null) return;
			AutomationPeer peer = CreatePeerForElement(TabControl);
			if(peer != null) {
				ISelectionProvider provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
				if(provider != null) {
					if(provider.IsSelectionRequired)
						throw new ArgumentException(DXTabControlAutomationStringProvider.attemptToRemoveFromSelection);
				}
			}			
		}
		public void SelectCore(AutomationEvents rEvent) {
			if(TabControl == null) return;
			bool isSelectedCore = IsSelected;
			TabControl.SelectedItem = TabItem;			
		}
		public void Select() {
			SelectCore(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
		}
		public IRawElementProviderSimple SelectionContainer {
			get {
				if(TabControl == null) return null;
				AutomationPeer peer = CreatePeerForElement(TabControl);
				if(peer == null) return null;
				return ProviderFromPeer(peer);
			}
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			if(patternInterface == PatternInterface.ScrollItem)
				return this;
			return null;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>{
					if(TabControl==null || TabControl.ItemContainerGenerator==null) return null;
					return TabControl.ItemContainerGenerator.ItemFromContainer(TabItem) as DependencyObject;					
				},
				()=>TabItem
			};
		}
	}
}
