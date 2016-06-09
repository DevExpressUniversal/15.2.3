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
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Bars;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Bars.Automation;
using System.Windows.Interop;
namespace DevExpress.Xpf.Ribbon.Automation {
	public static class RibbonAutomationHelper {		
	}
	public class RibbonControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IExpandCollapseProvider {
		public RibbonControlAutomationPeer(RibbonControl control)
			: base(control) {
		}		
		private RibbonTabsAutomationPeer tabsPeer;
		protected override void SetFocusCore() {
			(Owner as RibbonControl).Focus();
		}				
		protected override List<AutomationPeer> GetChildrenCore() {
			RibbonControl ribbon = Owner as RibbonControl;
			List<AutomationPeer> children = new List<AutomationPeer>();
			if(ribbon.ApplicationButton != null) {
				AutomationPeer applicationButtonPeer = CreatePeerForElement(ribbon.ApplicationButton);
				if(applicationButtonPeer != null)
					children.Add(applicationButtonPeer);
			}
			if(ribbon.Toolbar != null && ribbon.Toolbar.Control != null) {
				AutomationPeer ribbonToolbarControlAutomationPeer = CreatePeerForElement(ribbon.Toolbar.Control);
				if(ribbonToolbarControlAutomationPeer != null)
					children.Add(ribbonToolbarControlAutomationPeer);
			}
			if(ribbon.MinimizationButton != null && ribbon.MinimizationButton.IsVisible == true) {
				AutomationPeer minimizationButtonAutomationPeer = CreatePeerForElement(ribbon.MinimizationButton);
				if(minimizationButtonAutomationPeer != null) {
					children.Add(minimizationButtonAutomationPeer);
					(children[children.Count - 1] as RibbonCheckedBorderControlAutomationPeer).parent = ribbon;
					(children[children.Count - 1] as RibbonCheckedBorderControlAutomationPeer).role = RibbonCheckedBorderRole.expander;
				}
			}
			if(ribbon.PageHeaderItemLinks != null && ribbon.PageHeaderItemLinks.Count != 0 && ribbon.PageHeaderLinksControlContainer!=null) {
				List<AutomationPeer> listPeer = new FrameworkElementAutomationPeer(ribbon.PageHeaderLinksControlContainer).GetChildren();
				if(listPeer != null)
					children.AddRange(listPeer);
			}
			RibbonTabsAutomationPeer tabsPeer = GetTabsScrollViewer(ribbon);
			if(tabsPeer != null) children.Add(tabsPeer);
			if(!(ribbon.IsMinimized) && ribbon.SelectedPageControl != null) {
				AutomationPeer ribbonSelectedPageControlAutomationPeer = CreatePeerForElement(ribbon.SelectedPageControl);
				if(ribbonSelectedPageControlAutomationPeer != null)
					children.Add(ribbonSelectedPageControlAutomationPeer);
			}
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "RibbonControl";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		private RibbonTabsAutomationPeer GetTabsScrollViewer(RibbonControl ribbon) {
			if(tabsPeer != null) 
				return tabsPeer;
			var categoriesPane = ribbon.CategoriesPane;
			if (categoriesPane == null)
				return null;
			ScrollViewer scrollViewer = categoriesPane.ScrollHost;
			RibbonTabsAutomationPeer peer = new RibbonTabsAutomationPeer(scrollViewer);
			peer.leftPeer = new RepeatButtonAutomationPeer(categoriesPane.LeftButton as RepeatButton);
			peer.rightPeer = new RepeatButtonAutomationPeer(categoriesPane.RightButton as RepeatButton);
			tabsPeer = peer;
			peer.Ribbon = ribbon;
			return peer;
		}	
		#region IExpandCollapseProvider
		public void Collapse() {
			if((Owner as RibbonControl).IsMinimized == false) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				(Owner as RibbonControl).IsMinimized = true;
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
			}
		}
		public void Expand() {
			if((Owner as RibbonControl).IsMinimized == true) {
				ExpandCollapseState oldValue = ExpandCollapseState;
				(Owner as RibbonControl).IsMinimized = false;
				RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
			}
		}
		public ExpandCollapseState ExpandCollapseState {
			get { return (Owner as RibbonControl).IsMinimized ? ExpandCollapseState.Collapsed : System.Windows.Automation.ExpandCollapseState.Expanded; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.ExpandCollapse ? this : null;
		}
	}
	public class RibbonPageCategoryControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {		
		public RibbonPageCategoryControlAutomationPeer(RibbonPageCategoryControl control) : base(control) {
			isDefault = control.PageCategory is RibbonDefaultPageCategory;
		}
		internal bool isDefault;
		protected override void SetFocusCore() {
			(Owner as RibbonPageCategoryControl).Focus();
		}		
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			RibbonPageCategoryBase category = (Owner as RibbonPageCategoryControl).PageCategory;
			if(category == null) return String.Empty;
			if(category is RibbonDefaultPageCategory)
				return "DefaultPageCategory";
			return String.IsNullOrEmpty(category.Caption) ? "PageCategory" + Convert.ToString(category.Ribbon.Categories.IndexOf(category)) : category.Caption;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = base.GetChildrenCore();
			if(children == null)
				return new List<AutomationPeer>(); 
			children.RemoveAll(i => !(i is RibbonPageHeaderControlAutomationPeer));
			return children;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>(Owner as RibbonPageCategoryControl).PageCategory,
				()=>Owner
			};
		}
	}
	public class RibbonPageHeaderControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, ISelectionItemProvider, IScrollItemProvider {		
		public RibbonPageHeaderControlAutomationPeer(RibbonPageHeaderControl control) : base(control) {
			if (control != null) {
				control.WeakPageIsSelectedChanged += PageIsSelectedChanged;
			}
		}
		private DevExpress.Xpf.Core.ValueChangedEventHandler<bool> pageIsSelectedChanged;
		public DevExpress.Xpf.Core.ValueChangedEventHandler<bool> PageIsSelectedChanged {
			get {
				if (pageIsSelectedChanged == null) {
					pageIsSelectedChanged = new DevExpress.Xpf.Core.ValueChangedEventHandler<bool>(OnOwnerWeakPageIsSelectedChanged);
				}
				return pageIsSelectedChanged;
			}
			set { pageIsSelectedChanged = value; }
		}
		void OnOwnerWeakPageIsSelectedChanged(object sender, Core.ValueChangedEventArgs<bool> e) {
			if (e.NewValue)
				RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);			
		}
		internal ScrollViewer scrollViewer;
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			RibbonPageHeaderControl header = (RibbonPageHeaderControl)Owner;
			if(header.Page == null) return String.Empty;
			string caption = Convert.ToString(header.Page.Caption);
			return String.IsNullOrEmpty(caption) ? "Page" + Convert.ToString(header.Page.PageCategory.Pages.IndexOf(header.Page)) : caption;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TabItem;
		}
		protected override bool IsOffscreenCore() {
			if(GetParent() != null)
				return !(GetBoundingRectangleCore().IntersectsWith(GetParent().GetBoundingRectangle()));
			else
				return true;
		}
		protected override void SetFocusCore() {
			(Owner as RibbonPageHeaderControl).Focus();
		}
		#region ISelectionItemProvider
		public void AddToSelection() {
			Select();			
		}
		public bool IsSelected {
			get { return (Owner as RibbonPageHeaderControl).Page == null ? false : (Owner as RibbonPageHeaderControl).Page.IsSelected; }
		}
		public void RemoveFromSelection() {			
		}
		public void Select() {
			(Owner as RibbonPageHeaderControl).OnMouseLeftButtonSingleClick();
			RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
		}
		public IRawElementProviderSimple SelectionContainer {
			get {
				if(GetParent() == null) return (IRawElementProviderSimple)null;
				return ProviderFromPeer((GetParent() is RibbonPageCategoryControlAutomationPeer) ? GetParent().GetParent() : GetParent());
			}
		}
		#endregion
		#region IScrollItemProvider
		public void ScrollIntoView() {
		}
		#endregion     
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem) return this;
			if(patternInterface == PatternInterface.ScrollItem) return this;
			return null;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>(Owner as RibbonPageHeaderControl).Page,
				()=>Owner
			};
		} 
	}
	public class RibbonPageGroupControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IScrollItemProvider {
		public RibbonPageGroupControlAutomationPeer(RibbonPageGroupControl control) : base(control) {
		}
		internal bool isOffscreen { get; set; }
		internal RibbonSelectedPageControl lowerRibbon = null;
		internal RibbonCheckedBorderControlAutomationPeer captionButtonPeer;
		protected override void SetFocusCore() {
			(Owner as RibbonPageGroupControl).Focus();
		}		
		protected override bool IsOffscreenCore() {
			return isOffscreen;
		}		
		protected override Rect GetBoundingRectangleCore() {
			return isOffscreen? new Rect() : base.GetBoundingRectangleCore();
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			if((Owner as RibbonPageGroupControl).PageGroup == null) return String.Empty;
			return String.IsNullOrEmpty((Owner as RibbonPageGroupControl).PageGroup.Caption) ? "PageGroup" + (Owner as RibbonPageGroupControl).PageGroup.Page.Groups.IndexOf((Owner as RibbonPageGroupControl).PageGroup) : (Owner as RibbonPageGroupControl).PageGroup.Caption;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = base.GetChildrenCore();
			if(children == null)
				return new List<AutomationPeer>();
			if(children.Count(i => i is RibbonCheckedBorderControlAutomationPeer) != 0) {
				RibbonCheckedBorderControlAutomationPeer captionButton = children.Find(i => i is RibbonCheckedBorderControlAutomationPeer) as RibbonCheckedBorderControlAutomationPeer;
				if(captionButton != null) {
					captionButton.parent = (Owner as RibbonPageGroupControl).PageGroup;
					captionButtonPeer = captionButton;
				}
			}
			if((Owner as RibbonPageGroupControl).PageGroup != null) {
				if((!((Owner as RibbonPageGroupControl).PageGroup.ShowCaptionButton)) && (children.Count(i => i is RibbonCheckedBorderControlAutomationPeer) != 0))
					children.Remove(children.Find(i => i is RibbonCheckedBorderControlAutomationPeer));
			}
			return children;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected internal virtual void SetIsOffscreen(bool value) {
			bool oldValue = isOffscreen;
			isOffscreen = value;
			RaisePropertyChangedEvent(AutomationElementIdentifiers.IsOffscreenProperty, oldValue, value);
		}
		#region IScrollItemProvider
		public void ScrollIntoView() {
			if((Owner as RibbonPageGroupControl).PageGroup==null) return;
			if((Owner as RibbonPageGroupControl).PageGroup.IsVisible == false)
				throw new InvalidOperationException("Item cannot be scrolled into view");
			if(lowerRibbon == null || lowerRibbon.SelectedPage == null || lowerRibbon.SelectedPage.Groups == null) return;
			int index = lowerRibbon.SelectedPage.Groups.IndexOf((Owner as RibbonPageGroupControl).PageGroup);
			double offset = lowerRibbon.GetGroupHorizontalOffsetInScrollViewer(index);
			if(lowerRibbon.ScrollViewer == null) return;
			lowerRibbon.ScrollViewer.ScrollToHorizontalOffset(offset);
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.ScrollItem ? this : null;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>(Owner as RibbonPageGroupControl).PageGroup,
				()=>Owner
			};
		}
	}
	internal enum RibbonCheckedBorderRole { customization, expander };
	public class RibbonCheckedBorderControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IInvokeProvider, IExpandCollapseProvider, IToggleProvider {
		public RibbonCheckedBorderControlAutomationPeer(RibbonCheckedBorderControl control) : base(control) {
		}
		internal object parent;
		internal RibbonCheckedBorderRole role;
		protected override void SetFocusCore() {
			(Owner as RibbonCheckedBorderControl).Focus();
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;			
			if(parent is RibbonPageGroup) {
				RibbonPageGroupControlAutomationPeer pgPeer = GetParent() as RibbonPageGroupControlAutomationPeer;
				if(pgPeer != null
					&& pgPeer.Owner as RibbonPageGroupControl != null
					&& (pgPeer.Owner as RibbonPageGroupControl).PageGroup != null) {
					RibbonPageGroup group = (pgPeer.Owner as RibbonPageGroupControl).PageGroup;
					return (String.IsNullOrEmpty(group.Caption) ? pgPeer.GetName() : group.Caption) + " CaptionButton";
				}
			}
			if(parent is RibbonQuickAccessToolbarControl) {
				if(role == RibbonCheckedBorderRole.customization)
					return "Customize Toolbar";
				if(role == RibbonCheckedBorderRole.expander) return "DropDownButton";
			}
			if(parent is RibbonControl) {
				return "Minimize Ribbon";
			}
			if(parent is RibbonGalleryBarItemLinkControlAutomationPeer)
				return "Expand Gallery";
			if (parent is BackstageViewControlAutomationPeer)
				return "Hide Backstage View";
			return "";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}		
		#region IInvokeProvider
		public void Invoke() {
			if(parent is RibbonPageGroup) {
				if((parent as RibbonPageGroup).FirstPageGroupControl == null) return;
				(parent as RibbonPageGroup).FirstPageGroupControl.OnCaptionButtonClick();
			}
			RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
		#endregion        
		#region IExpandCollapseProvider
		public void Collapse() {
			if(parent is RibbonQuickAccessToolbarControl) {
				if((role == RibbonCheckedBorderRole.customization && (parent as RibbonQuickAccessToolbarControl).CustomizationMenu!=null && (parent as RibbonQuickAccessToolbarControl).CustomizationMenu.IsOpen == true) ||
					role == RibbonCheckedBorderRole.expander && (parent as RibbonQuickAccessToolbarControl).DropDownButton!=null && (parent as RibbonQuickAccessToolbarControl).DropDownButton.IsChecked == true) {
					ExpandCollapseState oldValue = ExpandCollapseState;
					RibbonQuickAccessToolbarControl toolbar = parent as RibbonQuickAccessToolbarControl;
					if(toolbar == null) return;
					if(role == RibbonCheckedBorderRole.customization)
						toolbar.CloseCustomizationMenu();
					if(role == RibbonCheckedBorderRole.expander) {
						toolbar.DropDownButton.IsChecked = false;
						toolbar.IsPopupOpened = false;
					}
					RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
				}
			}
		}
		public void Expand() {
			if(parent is RibbonQuickAccessToolbarControl) {
				if((role == RibbonCheckedBorderRole.customization && (parent as RibbonQuickAccessToolbarControl).CustomizationMenu != null && (parent as RibbonQuickAccessToolbarControl).CustomizationMenu.IsOpen == false) ||
					(role == RibbonCheckedBorderRole.expander && (parent as RibbonQuickAccessToolbarControl).DropDownButton != null && (parent as RibbonQuickAccessToolbarControl).DropDownButton.IsChecked != true)) {
					ExpandCollapseState oldValue = ExpandCollapseState;
					RibbonQuickAccessToolbarControl toolbar = parent as RibbonQuickAccessToolbarControl;
					if(toolbar == null) return;
					if(role == RibbonCheckedBorderRole.customization) {
						toolbar.CustomizationButton.IsChecked = true;
						toolbar.ShowCustomizationMenu();
					}
					if(role == RibbonCheckedBorderRole.expander) {
						toolbar.DropDownButton.IsChecked = true;
						toolbar.IsPopupOpened = true;
					}
					RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
				}
			}
		}
		public ExpandCollapseState ExpandCollapseState {
			get {
				if((parent as RibbonQuickAccessToolbarControl).CustomizationButton == null) return System.Windows.Automation.ExpandCollapseState.LeafNode;
				return (parent as RibbonQuickAccessToolbarControl).CustomizationButton.IsChecked == true ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed; }
		}
		#endregion
		#region IToggleProvider
		public void Toggle() {
			ToggleState oldValue = ToggleState;
			if(parent is RibbonControl) {
				(parent as RibbonControl).IsMinimized = !(parent as RibbonControl).IsMinimized;				
			}
			if (parent is BackstageViewControlAutomationPeer) {
				var backstage = ((parent as BackstageViewControlAutomationPeer).Owner as BackstageViewControl);
				if (backstage == null) return;
				backstage.IsOpen = !backstage.IsOpen;
			}
				RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldValue, ToggleState);
		}
		public ToggleState ToggleState {
			get {
				if (parent is RibbonControl)
					return (parent as RibbonControl).IsMinimized ? ToggleState.On : ToggleState.Off;
				if (parent is BackstageViewControlAutomationPeer) {
					var backstage = ((parent as BackstageViewControlAutomationPeer).Owner as BackstageViewControl);
					if (backstage == null) return ToggleState.Indeterminate;
					return backstage.IsOpen ? ToggleState.On : ToggleState.Off;
				}
				return ToggleState.Indeterminate;
			}
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			switch(patternInterface) {
				case PatternInterface.Invoke:
					return parent is RibbonPageGroup ? this : null;
				case PatternInterface.ExpandCollapse:
					return parent is RibbonQuickAccessToolbarControl ? this : parent is RibbonGalleryBarItemLinkControlAutomationPeer ? parent as IExpandCollapseProvider : null;
				case PatternInterface.Toggle:
					return parent is RibbonControl || parent is BackstageViewControlAutomationPeer ? this : null;
				default:
					return null;
			}
		}
	}	
	public class QuickAccessToolbarAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public QuickAccessToolbarAutomationPeer(RibbonQuickAccessToolbarControl control) : base(control) {
		}
		RibbonCheckedBorderControlAutomationPeer customizationButton = null;
		RibbonCheckedBorderControlAutomationPeer dropdownButton = null;
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ToolBar;
		}
		protected override void SetFocusCore() {
			(Owner as RibbonQuickAccessToolbarControl).Focus();
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			RibbonQuickAccessToolbarControl toolbar = Owner as RibbonQuickAccessToolbarControl;
			List<AutomationPeer> children = new List<AutomationPeer>();
			foreach(BarItemLinkBase linkBase in toolbar.ItemLinks) {
				BarItemLink link = linkBase as BarItemLink;
				if(link != null && link.LinkControl != null) {
					AutomationPeer linkLinkControlAutomationPeer = CreatePeerForElement(link.LinkControl);
					if(linkLinkControlAutomationPeer != null)
						children.Add(linkLinkControlAutomationPeer);
				}
			}
			if(customizationButton == null && toolbar.CustomizationButton!=null) customizationButton = new RibbonCheckedBorderControlAutomationPeer(toolbar.CustomizationButton) { parent = toolbar, role= RibbonCheckedBorderRole.customization};
			RibbonCheckedBorderControl dropdown = (Owner as RibbonQuickAccessToolbarControl).DropDownButton;
			if(dropdown != null && toolbar.IsDropDownButtonVisible) {
				if(dropdownButton == null) {
					dropdownButton = CreatePeerForElement(dropdown) as RibbonCheckedBorderControlAutomationPeer;
					dropdownButton.parent = toolbar;
					dropdownButton.role =  RibbonCheckedBorderRole.expander;
				}
				if(dropdownButton != null)
					children.Add(dropdownButton);
			}
			if(customizationButton != null)
				children.Add(customizationButton);
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Quick Access Toolbar";
		}		
	}
	public class LowerRibbonAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IScrollProvider {
		DevExpress.Data.Utils.WeakEventHandler<LowerRibbonAutomationPeer, RibbonPropertyChangedEventArgs, RibbonPropertyChangedEventHandler> selectedPageChangedWeakHandler;
		public LowerRibbonAutomationPeer(RibbonSelectedPageControl control) : base(control) {
			control.Ribbon.SelectedPageChanged += (selectedPageChangedWeakHandler = new Data.Utils.WeakEventHandler<LowerRibbonAutomationPeer, RibbonPropertyChangedEventArgs, RibbonPropertyChangedEventHandler>(
				this,
				(owner, sender, args) => ((LowerRibbonAutomationPeer)owner).OnSelectedPageChanged(),
				(wh, sender) => ((RibbonControl)sender).SelectedPageChanged -= wh.Handler,
				wh => wh.OnEvent
				)).Handler;
		}
		protected virtual void OnSelectedPageChanged() {
			ResetChildrenCache();
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Pane;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			if((Owner as RibbonSelectedPageControl).SelectedPage!=null && (Owner as RibbonSelectedPageControl).SelectedPage.Groups!=null)
			foreach(RibbonPageGroup group in (Owner as RibbonSelectedPageControl).SelectedPage.Groups) {
				if(group.FirstPageGroupControl != null) {
					AutomationPeer groupPageGroupControlAutomationPeer = CreatePeerForElement(group.FirstPageGroupControl);
					if(groupPageGroupControlAutomationPeer != null) {
						children.Add(CreatePeerForElement(group.FirstPageGroupControl));
						if(children[children.Count - 1] as RibbonPageGroupControlAutomationPeer != null)
							(children[children.Count - 1] as RibbonPageGroupControlAutomationPeer).lowerRibbon = Owner as RibbonSelectedPageControl;
					}
				}
			}			
			return children;
		}
		protected override void SetFocusCore() {
			(Owner as RibbonSelectedPageControl).Focus();
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Lower Ribbon";
		}		
		#region IScrollProvider
		public double HorizontalScrollPercent {
			get {
				if((Owner as RibbonSelectedPageControl).ScrollViewer == null) return -1;
				return HorizontallyScrollable ? ((Owner as RibbonSelectedPageControl).ScrollViewer.HorizontalOffset / (Owner as RibbonSelectedPageControl).ScrollViewer.ScrollableWidth)*100 : -1; }
		}
		public double HorizontalViewSize {
			get {
				if((Owner as RibbonSelectedPageControl).ScrollViewer == null) return 100;
				return HorizontallyScrollable? ((Owner as RibbonSelectedPageControl).ScrollViewer.ActualWidth/(Owner as RibbonSelectedPageControl).ScrollViewer.ScrollableWidth)*100 : 100; }
		}
		public bool HorizontallyScrollable {
			get {
				if((Owner as RibbonSelectedPageControl).ScrollViewer == null) return false;
				return ((Owner as RibbonSelectedPageControl).ScrollViewer.ActualWidth < (Owner as RibbonSelectedPageControl).ScrollViewer.ScrollableWidth) && (Owner as RibbonSelectedPageControl).ScrollViewer.ScrollableWidth != 0d; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(verticalAmount != ScrollAmount.NoAmount) {
				throw new InvalidOperationException();
			}
			ScrollViewer scrollViewer = (Owner as RibbonSelectedPageControl).ScrollViewer;
			if(scrollViewer == null) return;
			switch(horizontalAmount) {
				case ScrollAmount.LargeIncrement:
					if(scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
						scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollViewer.ActualWidth);
					break;
				case ScrollAmount.LargeDecrement:
					if(scrollViewer.HorizontalOffset > 0)
						scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - scrollViewer.ActualWidth);
					break;
				case ScrollAmount.SmallDecrement:
					if(scrollViewer.HorizontalOffset > 0)
						((RibbonSelectedPageControl)Owner).ScrollLeft();
					break;
				case ScrollAmount.SmallIncrement:
					if(scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
						((RibbonSelectedPageControl)Owner).ScrollRight();
					break;
			}
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if(HorizontallyScrollable == false)
				throw new InvalidOperationException("Lower Ribbon is not scrollable");
			if(verticalPercent != 0d && verticalPercent != -1d)
				throw new InvalidOperationException();
			if(horizontalPercent > 100d || (horizontalPercent < 0d && horizontalPercent != -1d)) {
				throw new ArgumentOutOfRangeException("Value must be greater than 0 or less than 100 (except -1)");
			}
			if(horizontalPercent == -1d)
				return;
			if((Owner as RibbonSelectedPageControl).ScrollViewer == null) return;
			(Owner as RibbonSelectedPageControl).ScrollViewer.ScrollToHorizontalOffset((Owner as RibbonSelectedPageControl).ScrollViewer.ScrollableWidth * horizontalPercent/100);
		}
		public double VerticalScrollPercent {
			get { return -1; }
		}
		public double VerticalViewSize {
			get { return 100; }
		}
		public bool VerticallyScrollable {
			get { return false; }
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.Scroll ? this : null;
		}
	}  
	public class RibbonTabsAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, ISelectionProvider, IScrollProvider {		
		public RibbonTabsAutomationPeer(ScrollViewer viewer) : base(viewer) {
			scrollViewer = viewer;
		}
		internal RibbonControl Ribbon;
		internal RepeatButtonAutomationPeer leftPeer;
		internal RepeatButtonAutomationPeer rightPeer;
		internal ScrollViewer scrollViewer;
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = null;
			children = base.GetChildrenCore();
			if(children == null)
				return new List<AutomationPeer>();
			if(children.Count(i => !(i is RibbonPageCategoryControlAutomationPeer)) != 0)
				children.RemoveAll(i => !(i is RibbonPageCategoryControlAutomationPeer));
			AutomationPeer defaultcategoryControlAutomationPeer = null;
			if(children.Count(i => ((i is RibbonPageCategoryControlAutomationPeer) && (i as RibbonPageCategoryControlAutomationPeer).isDefault)) != 0)
				defaultcategoryControlAutomationPeer = children.Find(i => ((i is RibbonPageCategoryControlAutomationPeer) && (i as RibbonPageCategoryControlAutomationPeer).isDefault));
			if(defaultcategoryControlAutomationPeer != null) {
				List<AutomationPeer> listPeer = defaultcategoryControlAutomationPeer.GetChildren();
				if(listPeer != null)
					children.AddRange(listPeer);
				children.Remove(defaultcategoryControlAutomationPeer);
			}
			SetScrollViewer(children);
			return children;
		}
		protected override void SetFocusCore() {
			scrollViewer.Focus();
		}		
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Tab;
		}		
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Ribbon Tabs";
		}
		private void SetScrollViewer(List<AutomationPeer> children) {
			foreach(AutomationPeer peer in children) {
				if(peer is RibbonPageHeaderControlAutomationPeer) (peer as RibbonPageHeaderControlAutomationPeer).scrollViewer = scrollViewer;
				if(peer is RibbonPageCategoryControlAutomationPeer) {
					List<AutomationPeer> children2 = peer.GetChildren();
					if(children2 != null)
						SetScrollViewer(children2);
				}
			}
		}
		#region ISelectionProvider
		public bool CanSelectMultiple {
			get { return false; }
		}
		public IRawElementProviderSimple[] GetSelection() {
			RibbonPageHeaderControlAutomationPeer selectedPageHeader= null;
			foreach(AutomationPeer peer in GetChildren()) {
				if((peer is RibbonPageHeaderControlAutomationPeer) && ((peer as RibbonPageHeaderControlAutomationPeer).IsSelected))
					selectedPageHeader = peer as RibbonPageHeaderControlAutomationPeer;
				if(peer is RibbonPageCategoryControlAutomationPeer) {
					List<AutomationPeer> listPeer = peer.GetChildren();
					if(listPeer == null)
						continue;
					foreach(AutomationPeer pagePeer in listPeer) {
						if((pagePeer is RibbonPageHeaderControlAutomationPeer) && ((pagePeer as RibbonPageHeaderControlAutomationPeer).IsSelected))
							selectedPageHeader = pagePeer as RibbonPageHeaderControlAutomationPeer;
					}
				}
			}
			if(selectedPageHeader == null)
				return new IRawElementProviderSimple[] { };
			return new IRawElementProviderSimple[] { ProviderFromPeer(selectedPageHeader) };
		}		
		public bool IsSelectionRequired {
			get { return false; }
		}
		#endregion
		#region IScrollProvider
		public double HorizontalScrollPercent {
			get {
				if(scrollViewer == null) return -1;
				return HorizontallyScrollable ? (scrollViewer.HorizontalOffset / scrollViewer.ScrollableWidth)*100 : -1; }
		}
		public double HorizontalViewSize {			
			get {
				if(scrollViewer == null) return 100;
				return HorizontallyScrollable ? (scrollViewer.ActualWidth/scrollViewer.ScrollableWidth)*100 : 100; }
		}
		public bool HorizontallyScrollable {
			get { return (scrollViewer.ActualWidth < scrollViewer.ScrollableWidth) && scrollViewer.ScrollableWidth != 0d; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(verticalAmount != ScrollAmount.NoAmount) {
				throw new InvalidOperationException();
			}
			if(scrollViewer == null) return;
			switch(horizontalAmount) {
				case ScrollAmount.LargeIncrement:
					if(scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
						scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollViewer.ActualWidth);
					break;
				case ScrollAmount.LargeDecrement:
					if(scrollViewer.HorizontalOffset > 0)
						scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - scrollViewer.ActualWidth);
					break;
				case ScrollAmount.SmallDecrement:
					if(scrollViewer.HorizontalOffset > 0)
						(leftPeer as IInvokeProvider).Invoke();
					break;
				case ScrollAmount.SmallIncrement:
					if(scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
						(rightPeer as IInvokeProvider).Invoke();
					break;
			}
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if(HorizontallyScrollable == false)
				throw new InvalidOperationException("Ribbon tabs are not scrollable");
			if(verticalPercent != 0d && verticalPercent != -1d)
				throw new InvalidOperationException();
			if(horizontalPercent > 100d || (horizontalPercent < 0d && horizontalPercent != -1d)) {
				throw new ArgumentOutOfRangeException("Value must be greater than 0 or less than 100 (except -1)");
			}
			if(horizontalPercent == -1d)
				return;
			if(scrollViewer == null) return;
			scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth * horizontalPercent / 100);
		}
		public double VerticalScrollPercent {
			get { return -1; }
		}
		public double VerticalViewSize {
			get { return 100; }
		}
		public bool VerticallyScrollable {
			get { return false; }
		}
		#endregion      
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Scroll) return this;
			if(patternInterface == PatternInterface.Selection) return this;
			return null;
		}
	}
	public class RibbonApplicationButtonControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer, IInvokeProvider {
		public RibbonApplicationButtonControlAutomationPeer(RibbonApplicationButtonControl control) : base(control) {
			control.Click += new EventHandler(OnClick);
		}		
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Button;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Menu";
		}
		protected override void SetFocusCore() {
			(Owner as RibbonApplicationButtonControl).Focus();
		}		
		#region IInvokeProvider
		public void Invoke() {
			RibbonApplicationButtonControl control = (Owner as RibbonApplicationButtonControl);
			control.OnClick();
		}
		void OnClick(object sender, EventArgs e) {
			RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
			RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.Invoke ? this : null;
		}
	}
	public class GalleryDropDownControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public GalleryDropDownControlAutomationPeer(GalleryDropDownControl control) : base(control) {
		}		
	}
	public class RibbonGalleryBarItemLinkControlAutomationPeer : BarItemLinkControlAutomationPeer, IExpandCollapseProvider {
		GalleryScrollButtonAutomationPeer up;
		GalleryScrollButtonAutomationPeer down;
		RibbonCheckedBorderControlAutomationPeer expand;
		public RibbonGalleryBarItemLinkControlAutomationPeer(RibbonGalleryBarItemLinkControl control)
			: base(control) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			if(up == null && ((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_Up") as Button)!=null) up = new GalleryScrollButtonAutomationPeer((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_Up") as Button, "Scroll Up");
			if(down == null && ((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_Down") as Button) != null) down = new GalleryScrollButtonAutomationPeer((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_Down") as Button, "Scroll Down");
			if(expand == null && ((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_DropDown") as RibbonCheckedBorderControl) != null) expand = new RibbonCheckedBorderControlAutomationPeer((Owner as RibbonGalleryBarItemLinkControl).GetTemplateChildCore("PART_DropDown") as RibbonCheckedBorderControl) { parent = this };
			AutomationPeer galleryControlAutomationPeer = null;
			if((Owner as RibbonGalleryBarItemLinkControl).GalleryControl != null)
				galleryControlAutomationPeer = CreatePeerForElement((Owner as RibbonGalleryBarItemLinkControl).GalleryControl);
			List<AutomationPeer> retValue = new List<AutomationPeer>();
			foreach(AutomationPeer peer in new AutomationPeer[] { galleryControlAutomationPeer, up, down, expand }) {
				if(peer != null)
					retValue.Add(peer);
			}
			return retValue;
		}
		#region IExpandCollapseProvider
		public void Collapse() {
			if((Owner as RibbonGalleryBarItemLinkControl).DropDownButton==null || (Owner as RibbonGalleryBarItemLinkControl).DropDownButton.IsChecked == false) return;
			ExpandCollapseState oldValue = ExpandCollapseState;
			(Owner as RibbonGalleryBarItemLinkControl).ShowDropDownGallery();
			RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);			
		}
		public void Expand() {
			if((Owner as RibbonGalleryBarItemLinkControl).DropDownButton == null || (Owner as RibbonGalleryBarItemLinkControl).DropDownButton.IsChecked == true) return;
			ExpandCollapseState oldValue = ExpandCollapseState;
			(Owner as RibbonGalleryBarItemLinkControl).ShowDropDownGallery();
			RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, ExpandCollapseState);
		}
		public ExpandCollapseState ExpandCollapseState {
			get {
				if((Owner as RibbonGalleryBarItemLinkControl).DropDownButton == null) return System.Windows.Automation.ExpandCollapseState.LeafNode;
				return ((Owner as RibbonGalleryBarItemLinkControl).DropDownButton.IsChecked==true) ? ExpandCollapseState.Expanded : System.Windows.Automation.ExpandCollapseState.Collapsed; }
		}
		#endregion
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override string GetNameCore() {
			string content = null;
			if((Owner as RibbonGalleryBarItemLinkControl) != null && (Owner as RibbonGalleryBarItemLinkControl).GalleryLink != null && (Owner as RibbonGalleryBarItemLinkControl).GalleryLink.GalleryItem != null)
				content = Convert.ToString((Owner as RibbonGalleryBarItemLinkControl).GalleryLink.GalleryItem.Content);
			return String.IsNullOrEmpty(content) ? "Ribbon Gallery Item" : content;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			switch(patternInterface){
				case PatternInterface.ExpandCollapse:
					return this;
				case PatternInterface.Scroll:
					return GetChildrenCore() == null ? null : GetChildrenCore()[0];
				default:
					return null;
			}
		}
	}
	public class BarButtonGroupLinkControlAutomationPeer : BarItemLinkControlAutomationPeer {
		public BarButtonGroupLinkControlAutomationPeer(BarButtonGroupLinkControl control) : base(control) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
	}
	public class GalleryScrollButtonAutomationPeer : ButtonAutomationPeer, IInvokeProvider {
		string name;
		public GalleryScrollButtonAutomationPeer(Button button, string name) : base(button) {
			this.name = name;
		}
		protected override string GetNameCore() {
			return name;
		}
		protected override void SetFocusCore() {
			(Owner as Button).Focus();
		}
		public void Invoke() {
			if((Owner as Button).IsEnabled == false)
				throw new ElementNotEnabledException();
			else
				if((base.GetPattern(PatternInterface.Invoke) as IInvokeProvider) != null)
					(base.GetPattern(PatternInterface.Invoke) as IInvokeProvider).Invoke();
		}
		public override object GetPattern(PatternInterface patternInterface) {
			object obj = base.GetPattern(patternInterface);
			if(patternInterface == PatternInterface.Invoke) return this;
			return obj;
		}
	}
}
