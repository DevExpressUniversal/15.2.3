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
namespace DevExpress.Xpf.Bars.Automation {
	class GalleryControlAutomationPeer : BaseNavigationAutomationPeer, IScrollProvider, ISelectionProvider 
		,IRawElementProviderSimple 
		{
		public GalleryControlAutomationPeer(GalleryControl control) : base(control) {
		}
		ScrollBar scrollBar;
		AutomationPeer captionPeer;
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : null);
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "Gallery" + (Owner as GalleryControl).With(x => x.Gallery).Return(x => x.Name, () => "");
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			GalleryControl control = Owner as GalleryControl;
			scrollBar = control.VerticalScrollBar;
			if (EnsureHasCaptionPeer())
				children.Add(captionPeer);
			if (control.GroupsControl != null) {
				foreach (object item in control.GroupsControl.Items) {
					children.Add(CreatePeerForElement(control.GroupsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement));
				}
			}			
			return children;
		}
		bool EnsureHasCaptionPeer() {
			if (captionPeer != null)
				return true;
			var caption = (Owner as GalleryControl).With(x => x.Caption);
			if ((captionPeer = caption.With(CreatePeerForElement)).ReturnSuccess()) {
				caption.Checked += new RoutedEventHandler(OnCaptionChecked);
				caption.Unchecked += new RoutedEventHandler(OnCaptionUnchecked);
			}
			return captionPeer != null;
		}
		void OnCaptionUnchecked(object sender, RoutedEventArgs e) {
			(Owner as GalleryControl).OnCaptionClick(sender, e);
		}
		void OnCaptionChecked(object sender, RoutedEventArgs e) {
			(Owner as GalleryControl).OnCaptionClick(sender, e);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Scroll) return this;
			if(patternInterface == PatternInterface.Selection && (Owner as GalleryControl).Gallery.ItemCheckMode != GalleryItemCheckMode.None) return this;
			return null;
		}
		#region IScrollProvider
		public double HorizontalScrollPercent {
			get { return -1d; }
		}
		public double HorizontalViewSize {
			get { return 100d; }
		}
		public bool HorizontallyScrollable {
			get { return false; }
		}
		static double SmallChange = 100;
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(horizontalAmount != ScrollAmount.NoAmount)
				throw new InvalidOperationException();
			GalleryControl control = Owner as GalleryControl;
			double oldValue = VerticalScrollPercent;
				switch(verticalAmount){
					case ScrollAmount.LargeDecrement:
						control.ScrollToVerticalOffset(control.ContentVerticalOffset - control.ViewportSize.Height);
						RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldValue, VerticalScrollPercent);
						break;
					case ScrollAmount.LargeIncrement:
						control.ScrollToVerticalOffset(control.ContentVerticalOffset + control.ViewportSize.Height);
						RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldValue, VerticalScrollPercent);
						break;
					case ScrollAmount.SmallDecrement:
						control.ScrollToVerticalOffset(control.ContentVerticalOffset - SmallChange);
						RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldValue, VerticalScrollPercent);
						break;
					case ScrollAmount.SmallIncrement:
						control.ScrollToVerticalOffset(control.ContentVerticalOffset + SmallChange);
						RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldValue, VerticalScrollPercent);
						break;
				}
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {			
			if(horizontalPercent != 0 && horizontalPercent != -1)
				throw new InvalidOperationException();
			if((verticalPercent < 0 || verticalPercent > 100) && verticalPercent != -1)
				throw new IndexOutOfRangeException();
			else {
				double oldValue = VerticalScrollPercent;
				(Owner as GalleryControl).ScrollToVerticalOffset((((Owner as GalleryControl).ScrollableSize.Height - (Owner as GalleryControl).ViewportSize.Height) * verticalPercent) / 100);
				RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, oldValue, VerticalScrollPercent);
			}
		}
		public double VerticalScrollPercent {
			get { return (((Owner as GalleryControl).ContentVerticalOffset) / ((Owner as GalleryControl).ScrollableSize.Height - (Owner as GalleryControl).ViewportSize.Height)) * 100; }
		}
		public double VerticalViewSize {
			get { return VerticallyScrollable ? ((Owner as GalleryControl).ViewportSize.Height / ((Owner as GalleryControl).ScrollableSize.Height - (Owner as GalleryControl).ViewportSize.Height)) * 100 : 100; }
		}
		public bool VerticallyScrollable {
			get { return (Owner as GalleryControl).ViewportSize.Height < (Owner as GalleryControl).ScrollableSize.Height; }
		}
		#endregion
		#region ISelectionProvider
		public bool CanSelectMultiple {
			get { return (Owner as GalleryControl).Gallery.ItemCheckMode == GalleryItemCheckMode.Multiple || (Owner as GalleryControl).Gallery.ItemCheckMode == GalleryItemCheckMode.SingleInGroup; }
		}
		public IRawElementProviderSimple[] GetSelection() {
			GalleryControl control = (Owner as GalleryControl);			
			List<GalleryItem> items = control.Gallery.GetCheckedItems();
			List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();
			foreach(GalleryItem item in items) {
				providers.Add(ProviderFromPeer
					(CreatePeerForElement(
					(control.GroupsControl.ItemContainerGenerator
					.ContainerFromItem(item.Group) as GalleryItemGroupControl).ItemContainerGenerator
					.ContainerFromItem(item) as GalleryItemControl)));
			}
			return providers.ToArray<IRawElementProviderSimple>();
		}
		public bool IsSelectionRequired {
			get { return false; }
		}
		#endregion
		#region IRawElementProviderSimple
		public object GetPatternProvider(int patternId) {
			if(patternId == ScrollPatternIdentifiers.Pattern.Id) return this;
			if(patternId == SelectionPatternIdentifiers.Pattern.Id) return this;
			return null;
		}	   
		public IRawElementProviderSimple HostRawElementProvider {			
			get { 
				WindowInteropHelper helper = new WindowInteropHelper(Application.Current.MainWindow);
				return AutomationInteropProvider.HostProviderFromHandle(helper.Handle);
			}
		}
		public ProviderOptions ProviderOptions {
			get { return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider; }
		}
		public object GetPropertyValue(int propertyId) {
			throw new NotImplementedException();
		}
		#endregion
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[] {
				()=>(Owner as GalleryControl).Gallery,
				()=>Owner
			};
		}
	}
	class GalleryItemGroupControlAutomationPeer : BaseNavigationAutomationPeer, IScrollItemProvider {
		public GalleryItemGroupControlAutomationPeer(GalleryItemGroupControl control) : base(control) {
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : null);
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return Convert.ToString((Owner as GalleryItemGroupControl).Group == null ? "" : (Owner as GalleryItemGroupControl).Group.Caption);
		}
		public void ScrollIntoView() {
			(Owner as GalleryItemGroupControl).GroupsControl.GalleryControl.ScrollToGroupByIndex((Owner as GalleryItemGroupControl).GroupsControl.Items.IndexOf((Owner as GalleryItemGroupControl).Group));
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ScrollItem) return this;
			return null;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>(Owner as GalleryItemGroupControl).Group,
				()=>Owner
			};
		}
	}
	class GalleryItemControlAutomationPeer : BaseNavigationAutomationPeer, IScrollItemProvider, IInvokeProvider, ISelectionItemProvider
		, IRawElementProviderSimple 
		{
		public GalleryItemControlAutomationPeer(GalleryItemControl control) : base(control) {
			control.Item.Click += new EventHandler(OnItemClick);
		}		
		protected override void SetFocusCore() {
			(Owner as GalleryItemControl).Focus();
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.AutomationIdProperty, out useAttachedValue);
			return BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : null);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			GalleryItemControl control = (Owner as GalleryItemControl);
			return children;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			if((Owner as GalleryItemControl).Item == null) return "";
			string caption = (Owner as GalleryItemControl).Item.Caption == null ? "" : Convert.ToString((Owner as GalleryItemControl));
			string description = (Owner as GalleryItemControl).Item.Description == null ? "" : Convert.ToString((Owner as GalleryItemControl).Item.Description);
			return caption + "(" + description+")";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ListItem;
		}
		#region IScrollItemProvider
		public void ScrollIntoView() {
			(Owner as GalleryItemControl).GroupControl.GroupsControl.GalleryControl.ScrollToItem((Owner as GalleryItemControl).Item);
		}
		#endregion        
		#region IInvokeProvider
		void OnItemClick(object sender, EventArgs e) {
			RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
		}
		public void Invoke() {
			Dispatcher.BeginInvoke(new Action(() => (Owner as GalleryItemControl).Item.OnClick(Owner as GalleryItemControl)));
		}
		#endregion
		#region ISelectionItemProvider
		public void AddToSelection() {
			if((Owner as GalleryItemControl).Item.IsChecked) return;
			ToggleIsSelected();
		}
		private void ToggleIsSelected() {
			bool oldValue = IsSelected;
			(Owner as GalleryItemControl).OnMouseLeftButtonUpCore();
			RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
			RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, IsSelected);
		}
		public bool IsSelected {
			get { return (Owner as GalleryItemControl).Item.IsChecked; }
		}
		public void RemoveFromSelection() {
			if(!((Owner as GalleryItemControl).Item.IsChecked)) return;
			ToggleIsSelected();
		}
		public void Select() {
			if((Owner as GalleryItemControl).Item.IsChecked) return;
			ToggleIsSelected();
		}
		public IRawElementProviderSimple SelectionContainer {
			get { return ProviderFromPeer(GetParent().GetParent()); }
		}
		#endregion
		#region IRawElementProviderSimple
		public object GetPatternProvider(int patternId) {
			if(patternId == ScrollItemPatternIdentifiers.Pattern.Id) return this;
			if(patternId == InvokePatternIdentifiers.Pattern.Id) return this;
			if(patternId == SelectionItemPatternIdentifiers.Pattern.Id && (Owner as GalleryItemControl).Gallery.ItemCheckMode != GalleryItemCheckMode.None) return this;
			return null;
		}
		public IRawElementProviderSimple HostRawElementProvider {
			get { return (PeerFromProvider(SelectionContainer) as GalleryControlAutomationPeer).HostRawElementProvider; }
		}
		public ProviderOptions ProviderOptions {
			get { return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider; }
		}
		public object GetPropertyValue(int propertyId) {
			throw new NotImplementedException();
		}
		#endregion
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>(Owner as GalleryItemControl).Item,
				()=>Owner
		};
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ScrollItem) return this;
			if(patternInterface == PatternInterface.Invoke) return this;
			if(patternInterface == PatternInterface.SelectionItem && (Owner as GalleryItemControl).Gallery.ItemCheckMode != GalleryItemCheckMode.None) return this;
			return null;
		}	   
	}   
}
