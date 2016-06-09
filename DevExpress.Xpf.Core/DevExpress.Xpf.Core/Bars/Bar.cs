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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	public class SLBarItemLinkHolderBase : FrameworkContentElement {
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
	}
	[ContentProperty("Items")]
	public class BarItemLinkHolderBase : SLBarItemLinkHolderBase, ILinksHolder, ILogicalChildrenContainer {		
		#region static 
		public static readonly DependencyProperty VisibleProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BarItemsAttachedBehaviorProperty;		 
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;				
		static BarItemLinkHolderBase() {
			VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), typeof(BarItemLinkHolderBase), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnVisiblePropertyChanged)));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(object), typeof(BarItemLinkHolderBase), new PropertyMetadata(null, new PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BarItemLinkHolderBase), new PropertyMetadata(null, new PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarItemLinkHolderBase), new PropertyMetadata(null, new PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(BarItemLinkHolderBase), new PropertyMetadata(null, OnItemLinksTemplatePropertyChanged));
			BarItemsAttachedBehaviorProperty = DependencyProperty.RegisterAttached("BarItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<BarItemLinkHolderBase, BarItem>), typeof(BarItemLinkHolderBase), new PropertyMetadata(null));
			NameProperty.OverrideMetadata(typeof(BarItemLinkHolderBase), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnNamePropertyChanged)));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), typeof(BarItemLinkHolderBase), new FrameworkPropertyMetadata(false, (d, e) => ((BarItemLinkHolderBase)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue)));
		}		
		protected static void OnVisiblePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkHolderBase)obj).OnVisibleChanged(e);
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkHolderBase)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkHolderBase)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkHolderBase)d).OnNameChanged(e.NewValue as string, e.OldValue as string);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkHolderBase)d).OnItemLinksTemplateSelectorChanged(e);
		}		
		#endregion
		BarItemLinkCollection links;
		BarItemLinkCollection mergedLinks;
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		public BarItemLinkHolderBase() {
			MergedLinksHoldersVisibility = new Dictionary<BarItemLinkHolderBase, bool>();
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			foreach(BarItemLinkBase linkBase in ItemLinks) {
				linkBase.CoerceValue(IsEnabledProperty);
				linkBase.UpdateProperties();
			}
		}		
		BarManager manager = null;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkHolderBaseManager"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager Manager {
			get { return manager; }
			set {
				if(value == manager) return;
				var oldValue = manager;
				OnManagerChanging(oldValue);
				manager = value;
				OnManagerChanged(manager);				
			}
		}
		protected virtual void OnManagerChanging(BarManager oldManager) {
		}
		protected virtual void OnManagerChanged(BarManager newManager) {			
		}
		protected internal ObservableCollection<ILinksHolder> MergedLinksHolders { 
			get {
				if(mergedLinksHolders == null) {
					mergedLinksHolders = new ObservableCollection<ILinksHolder>();
					mergedLinksHolders.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMergedLinksHoldersChanged);
				}
				return mergedLinksHolders;
			} 
		}
		protected virtual void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
		}
		protected override IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkHolderBaseVisible")]
#endif
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items {
			get {
				if(itemsCore == null)
					itemsCore = new CommonBarItemCollection(this);
				return itemsCore;
			}
		}		
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkHolderBaseItemLinks"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]		
		public BarItemLinkCollection ItemLinks {
			get {
				if(links == null) links = CreateItemLinksCollection();
				return links;
			}
		}
		public object ItemLinksSource {
			get { return GetValue(ItemLinksSourceProperty); }
			set { SetValue(ItemLinksSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		protected BarItemGeneratorHelper<BarItemLinkHolderBase> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<BarItemLinkHolderBase>(this, BarItemsAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return itemGeneratorHelper;
			}
		}
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			return new BarItemLinkCollection(this);
		}
		protected virtual void OnVisibleChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void UpdateLinksControl() { 
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnItemLinksSourceChanged(new DependencyPropertyChangedEventArgs(ItemLinksSourceProperty, ItemLinksSource, ItemLinksSource));
		}
		protected virtual void OnItemLinksTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarItemLinkHolderBase, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, BarItemsAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksTemplateSelectorChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarItemLinkHolderBase, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, BarItemsAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			BarNameScope.EnsureRegistrator(this);
		}
		protected virtual void OnNameChanged(string newValue, string oldValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(ILinksHolder), oldValue, newValue);
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, newValue);
		}
		#region ILinksHolder Members
		Dictionary<BarItemLinkHolderBase, bool> MergedLinksHoldersVisibility;
		BarItemLinkCollection ILinksHolder.Links {
			get { return ItemLinks; }
		}
		IEnumerable ILinksHolder.ItemsSource { get { return ItemLinksSource as IEnumerable; } }
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		bool ILinksHolder.ShowDescription { get { return false; } }
		protected virtual BarItemLinkCollection CreateMergedLinks() { return new MergedItemLinkCollection(this); }
		BarItemLinkCollection ILinksHolder.MergedLinks {
			get {
				if(mergedLinks == null)
					mergedLinks = CreateMergedLinks();
				return mergedLinks;
			}
		}
		readonly ImmediateActionsManager immediateActionsManager = new ImmediateActionsManager();
		ImmediateActionsManager ILinksHolder.ImmediateActionsManager {
			get { return immediateActionsManager; }
		}
		BarItemLinkCollection ILinksHolder.ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		protected virtual void MergeCore(ILinksHolder holder) {
			BarItemLinkHolderBase bar = holder as BarItemLinkHolderBase;
			(bar as IMergingSupport).Do(x => x.IsAutomaticallyMerged &= MergingPropertiesHelper.IsAutomaticMergingInProcess(x));
			if(bar != null)
				holder.MergedParent = this;
			if(MergedLinksHolders.Contains(holder))
				return;
			MergedLinksHolders.Add(holder);
			BarItemLinkHolderBase barItemLinkHolderBase = holder as BarItemLinkHolderBase;
			if(barItemLinkHolderBase == null) return;
			MergedLinksHoldersVisibility.Add(barItemLinkHolderBase, barItemLinkHolderBase.Visible);
			barItemLinkHolderBase.Visible = false;
		}
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		void ILinksHolder.Merge(ILinksHolder holder) {
			MergeCore(holder);
		}
		protected virtual void UnMergeCore(ILinksHolder holder) {
			BarItemLinkHolderBase bar = holder as BarItemLinkHolderBase;
			if(bar != null)
				((ILinksHolder)bar).MergedParent = null;
			(bar as IMergingSupport).Do(x => x.IsAutomaticallyMerged = true);
			MergedLinksHolders.Remove(holder);
			BarItemLinkHolderBase barItemLinkHolderBase = holder as BarItemLinkHolderBase;
			if(barItemLinkHolderBase == null || !MergedLinksHoldersVisibility.Keys.Contains(barItemLinkHolderBase)) return;
			barItemLinkHolderBase.Visible = MergedLinksHoldersVisibility[barItemLinkHolderBase];
			MergedLinksHoldersVisibility.Remove(barItemLinkHolderBase);
		}
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			UnMergeCore(holder);
		}
		protected void UnMergeCore() {
			while(MergedLinksHolders.Count != 0) {
				BarItemLinkHolderBase bar = MergedLinksHolders[MergedLinksHolders.Count - 1] as BarItemLinkHolderBase;
				if(bar != null)
					((ILinksHolder)bar).MergedParent = null;
				MergedLinksHolders.RemoveAt(MergedLinksHolders.Count - 1);
			}			
			foreach(BarItemLinkHolderBase holder in MergedLinksHoldersVisibility.Keys)
				holder.Visible = MergedLinksHoldersVisibility[holder];
			MergedLinksHoldersVisibility.Clear();
		}
		void ILinksHolder.UnMerge() {
			UnMergeCore();
		}
		GlyphSize ILinksHolder.ItemsGlyphSize { get { return GlyphSize.Small; } }
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) { return GlyphSize.Default; }
		IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if (LogicalTreeHelper.GetParent(link) == null)
				AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {
			RemoveLogicalChild(link);
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.None; } }
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkHolderBaseItemLinksSourceElementGeneratesUniqueBarItem")]
#endif
		public bool ItemLinksSourceElementGeneratesUniqueBarItem {
			get { return (bool)GetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty); }
			set { SetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty, value); }
		}
		BarItemGeneratorHelper<BarItemLinkHolderBase> itemGeneratorHelper;
		#region ILogicalChildrenContainer
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion        
		#region IMultipleElementRegistratorSupport Members
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(ILinksHolder), typeof(IFrameworkInputElement) }.Concat(GetRegistratorKeys()).Distinct(); }
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(registratorKey, typeof(IFrameworkInputElement)))
				return Name;
			if (Equals(registratorKey, typeof(ILinksHolder)))
				return Name;
			return GetRegistratorName(registratorKey);
		}
		protected virtual object GetRegistratorName(object registratorKey) { throw new ArgumentException(); }
		protected virtual IEnumerable<object> GetRegistratorKeys() { return Enumerable.Empty<object>(); }
		#endregion
	}
	public class BarControlLoadedEventArgs : EventArgs {
		public Bar Bar { get; private set; }
		public BarControl BarControl { get; private set; }
		public BarControlLoadedEventArgs(Bar bar, BarControl barControl) {
			Bar = bar;
			BarControl = barControl;
		}
	}
	public delegate void BarControlLoadedEventHandler(object sender, BarControlLoadedEventArgs e);
	public class BarVisibileChangedEventArgs : BarControlLoadedEventArgs {
		public bool OldValue { get; private set; }
		public bool NewValue { get; private set; }
		public BarVisibileChangedEventArgs(Bar bar, BarControl barControl, bool oldValue, bool newValue) : base(bar, barControl) {
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
	public delegate void BarVisibleChangedEventHandler(object sender, BarVisibileChangedEventArgs e);
	public class Bar : BarItemLinkHolderBase, ILinksHolder, IBarManagerControllerAction, IMergingSupport, IBar {		
		#region static
		public static readonly DependencyProperty UseWholeRowProperty;
		public static readonly DependencyProperty IsMultiLineProperty;
		public static readonly DependencyProperty BarItemHorzIndentProperty;
		public static readonly DependencyProperty BarItemVertIndentProperty;
		public static readonly DependencyProperty AllowQuickCustomizationProperty;
		public static readonly DependencyProperty ShowDragWidgetProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty AllowCollapseProperty;
		public static readonly DependencyProperty IsMainMenuProperty;
		public static readonly DependencyProperty IsStatusBarProperty;
		public static readonly DependencyProperty ShowSizeGripProperty;
		public static readonly DependencyProperty GlyphSizeProperty;
		public static readonly DependencyProperty RotateWhenVerticalProperty;
		public static readonly DependencyProperty IsCollapsedProperty;
		public static readonly DependencyProperty AllowRenameProperty;
		public static readonly DependencyProperty AllowHideProperty;
		public static readonly DependencyProperty BarItemsAlignmentProperty;
		public static readonly DependencyProperty AllowCustomizationMenuProperty;
		public static readonly DependencyProperty BarItemDisplayModeProperty;
		public static readonly DependencyProperty HideWhenEmptyProperty;		
		static Bar() {
			MergingProperties.ToolBarMergeStyleProperty.OverrideMetadata(typeof(Bar), new FrameworkPropertyMetadata((d, e) => ((Bar)d).DockInfo.BarControl.Do(x => x.UpdateVisibility())));
			HideWhenEmptyProperty = DependencyPropertyManager.Register("HideWhenEmpty", typeof(bool), typeof(Bar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((Bar)d).OnHideWhenEmptyChanged((bool)e.OldValue))));
			BarItemDisplayModeProperty = DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(Bar), new FrameworkPropertyMetadata(BarItemDisplayMode.Default));
			UseWholeRowProperty = DependencyPropertyManager.Register("UseWholeRow", typeof(DefaultBoolean), typeof(Bar),
				new FrameworkPropertyMetadata(DefaultBoolean.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUseWholeRowPropertyChanged)));
			AllowRenameProperty = DependencyPropertyManager.Register("AllowRename", typeof(bool), typeof(Bar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			IsMultiLineProperty = DependencyPropertyManager.Register("IsMultiLine", typeof(bool), typeof(Bar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			BarItemHorzIndentProperty = DependencyPropertyManager.Register("BarItemHorzIndent", typeof(double), typeof(Bar),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			BarItemVertIndentProperty = DependencyPropertyManager.Register("BarItemVertIndent", typeof(double), typeof(Bar),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			AllowQuickCustomizationProperty = DependencyPropertyManager.Register("AllowQuickCustomization", typeof(DefaultBoolean), typeof(Bar),
				new FrameworkPropertyMetadata(DefaultBoolean.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnAllowQuickCuztomizationPropertyChanged)));
			ShowDragWidgetProperty = DependencyPropertyManager.Register("ShowDragWidget", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowDragWidgetPropertyChanged)));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), typeof(Bar), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure));
			AllowCollapseProperty = DependencyPropertyManager.Register("AllowCollapse", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnAllowCollapsePropertyChanged),
					new CoerceValueCallback(OnAllowCollapsePropertyCoerce)));
			IsMainMenuProperty = DependencyPropertyManager.Register("IsMainMenu", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsMainMenuChanged)));
			IsStatusBarProperty = DependencyPropertyManager.Register("IsStatusBar", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsStatusBarChanged)));
			ShowSizeGripProperty = DependencyPropertyManager.Register("ShowSizeGrip", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowSizeGripChanged)));
			GlyphSizeProperty = DependencyPropertyManager.Register("GlyphSize", typeof(GlyphSize), typeof(Bar),
				new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnGlyphSizePropertyChanged)));
			IsCollapsedProperty = DependencyPropertyManager.Register("IsCollapsed", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsCollapsedPropertyChanged),
					new CoerceValueCallback(OnIsCollapsedPropertyCoerce)));
			RotateWhenVerticalProperty = DependencyPropertyManager.Register("RotateWhenVertical", typeof(bool), typeof(Bar),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRotateWhenVericalPropertyChanged)));
			AllowHideProperty = DependencyPropertyManager.Register("AllowHide", typeof(DefaultBoolean), typeof(Bar),
				new FrameworkPropertyMetadata(DefaultBoolean.Default, new PropertyChangedCallback(OnDisableClosePropertyChanged)));
			AllowCustomizationMenuProperty = DependencyPropertyManager.Register("AllowCustomizationMenu", typeof(bool), typeof(Bar), new FrameworkPropertyMetadata(true));
			BarItemsAlignmentProperty = DependencyPropertyManager.Register("BarItemsAlignment", typeof(BarItemAlignment), typeof(Bar),
				new FrameworkPropertyMetadata(BarItemAlignment.Default, new PropertyChangedCallback(OnAlignmentPropertyChanged)));
			MergingProperties.HideElementsProperty.OverrideMetadata(typeof(Bar), new FrameworkPropertyMetadata((d, e) => ((Bar)d).DockInfo.BarControl.Do(x => x.OnBarVisibleChanged())));
		}
		protected static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Bar)d).OnAlignmentChanged(e);
		}
		protected static object OnAllowCollapsePropertyCoerce(DependencyObject obj, object baseValue) {
			return ((Bar)obj).OnAllowCollapseCoerce(baseValue);
		}
		protected static void OnAllowCollapsePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnAllowCollapseChanged(e);
		}
		protected static object OnIsCollapsedPropertyCoerce(DependencyObject obj, object baseValue) {
			return ((Bar)obj).OnIsCollapsedCoerce(baseValue);
		}
		protected static void OnIsCollapsedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnIsCollapsedChanged(e);
		}
		protected static void OnUseWholeRowPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnUseWholeRowChanged(e);
		}
		protected static void OnGlyphSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnGlyphSizeChanged(e);
		}
		protected static void OnIsMainMenuChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnIsMainMenuChanged(e);
		}
		protected static void OnIsStatusBarChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnIsStatusBarChanged(e);
		}
		protected static void OnShowSizeGripChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnShowSizeGripChanged(e);
		}
		protected static void OnShowDragWidgetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnShowDragWidgetChanged(e);
		}
		protected static void OnAllowQuickCuztomizationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnAllowQuickCuztomizationChanged(e);
		}
		protected static void OnRotateWhenVericalPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnRotateWhenVerticalChanged(e);
		}
		protected static void OnDisableClosePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((Bar)obj).OnDisableCloseChanged(e);
		}
		static void OnBarCustomizationControlTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			Bar bar = (Bar)obj;
			if (e.NewValue == null) bar.CustomizationControl = null;
			BarCustomizationControl customizationControl = ((FrameworkTemplate)e.NewValue).LoadContent() as BarCustomizationControl;
			if (customizationControl == null)
				throw new ArgumentException("BarCustomizationControl");
			bar.CustomizationControl = customizationControl;
		}
		#endregion
		protected internal bool ShouldOnNewRow = false;
		BarDockInfo dockInfo;
		public Bar() {
			((IBar)this).ShowInOriginContainer = true;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			((IMergingSupport)this).IsAutomaticallyMerged = true;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			HideIfFloating();
		}		
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			DockInfo.MakeBarFloating(false);
			RefreshPopup();
		}
		void RefreshPopup() {
			HideIfFloating();
			ShowIfFloating();
		}
		public void HideIfFloating() {
			(DockInfo.Container as FloatingBarContainerControl).With(x => x.OwnerPopup).Do(x => x.IsOpen = false);
		}
		public void ShowIfFloating() {
			(DockInfo.Container as FloatingBarContainerControl).With(x => x.OwnerPopup).Do(x => x.IsOpen = true);
		}
		protected override void OnManagerChanging(BarManager oldManager) {			
		}
		protected override void OnManagerChanged(BarManager newManager) {
			base.OnManagerChanged(newManager);
			if (newManager != null) {
				if (IsMainMenu)
					newManager.MainMenu = this;
				if (IsStatusBar)
					newManager.StatusBar = this;				
			}
		}		
		protected virtual BarDockInfo CreateDockInfo() {
			return new BarDockInfo();
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarDockInfo"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ]
		public BarDockInfo DockInfo {
			get {
				if (dockInfo == null)
					DockInfo = CreateDockInfo();
				return dockInfo;
			}
			set {
				if (dockInfo == value) return;
				BarDockInfo prevValue = dockInfo;
				dockInfo = value;
				OnDockInfoChanged(prevValue);
			}
		}
		protected internal void Remerge() {
			OnMergedLinksHoldersChanged(MergedLinksHolders, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		protected override void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
			helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.UpdateItemsSource(((ILinksHolder)this).ActualLinks);
		}
		protected internal virtual bool ShowWhenBarManagerIsMerged { get { return false; } }
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, new SingleObjectEnumerator(DockInfo)); }
		}
		protected override void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnIsEnabledChanged(sender, e);
			if (DockInfo != null && DockInfo.BarControl != null)
				DockInfo.BarControl.IsEnabled = IsEnabled;
		}
		protected virtual void OnDockInfoChanged(BarDockInfo prevValue) {
			if (prevValue != null) {
				prevValue.Bar = null;
				RemoveLogicalChild(prevValue);
			}
			if (DockInfo != null) {
				AddLogicalChild(DockInfo);
				DockInfo.Bar = this;
			}
		}
		void IBar.Merge(IBar bar) {
			Bar _bar = bar as Bar ?? (bar as ToolBarControlBase).With(x => x.Bar);
			if (_bar == null)
				return;
			Merge(_bar);
		}
		void IBar.Unmerge(IBar bar) {
			Bar _bar = bar as Bar ?? (bar as ToolBarControlBase).With(x => x.Bar);
			if (_bar == null)
				return;
			UnMerge(_bar);
		}
		void IBar.Unmerge() {
			UnMerge();
		}
		public void Merge(Bar bar) {
			MergeCore(bar);
		}
#pragma warning disable 3005
		public void UnMerge(Bar bar) {
			UnMergeCore(bar);
		}
		public void UnMerge() {
			UnMergeCore();
		}
#pragma warning restore 3005
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarAllowRename")]
#endif
		public bool AllowRename {
			get { return (bool)GetValue(AllowRenameProperty); }
			set { SetValue(AllowRenameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarAllowCollapse")]
#endif
		public bool AllowCollapse {
			get { return (bool)GetValue(AllowCollapseProperty); }
			set { SetValue(AllowCollapseProperty, value); }
		}
		public bool HideWhenEmpty {
			get { return (bool)GetValue(HideWhenEmptyProperty); }
			set { SetValue(HideWhenEmptyProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarAllowCustomizationMenu")]
#endif
		public bool AllowCustomizationMenu {
			get { return (bool)GetValue(AllowCustomizationMenuProperty); }
			set { SetValue(AllowCustomizationMenuProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarUseWholeRow")]
#endif
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean UseWholeRow {
			get { return (DefaultBoolean)GetValue(UseWholeRowProperty); }
			set { SetValue(UseWholeRowProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsUseWholeRow")]
#endif
		public bool IsUseWholeRow {
			get {
				if (UseWholeRow == DefaultBoolean.True) return true;
				if (UseWholeRow == DefaultBoolean.False) return false;
				if (IsStatusBar || IsMainMenu) return true;
				return false;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsMultiLine")]
#endif
		public bool IsMultiLine {
			get { return (bool)GetValue(IsMultiLineProperty); }
			set { SetValue(IsMultiLineProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarAllowQuickCustomization")]
#endif
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowQuickCustomization {
			get { return (DefaultBoolean)GetValue(AllowQuickCustomizationProperty); }
			set { SetValue(AllowQuickCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsAllowQuickCustomization")]
#endif
		public bool IsAllowQuickCustomization {
			get {
				if (AllowQuickCustomization == DefaultBoolean.False) return false;
				if (AllowQuickCustomization == DefaultBoolean.True) return true;
				if (IsMainMenu || IsStatusBar) return false;
				return true;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarShowDragWidget")]
#endif
		public bool ShowDragWidget {
			get { return (bool)GetValue(ShowDragWidgetProperty); }
			set { SetValue(ShowDragWidgetProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarBarItemHorzIndent")]
#endif
		public double BarItemHorzIndent {
			get { return (double)GetValue(BarItemHorzIndentProperty); }
			set { SetValue(BarItemHorzIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarBarItemVertIndent")]
#endif
		public double BarItemVertIndent {
			get { return (double)GetValue(BarItemVertIndentProperty); }
			set { SetValue(BarItemVertIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCaption")]
#endif
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsMainMenu")]
#endif
		public bool IsMainMenu {
			get { return (bool)GetValue(IsMainMenuProperty); }
			set { SetValue(IsMainMenuProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsStatusBar")]
#endif
		public bool IsStatusBar {
			get { return (bool)GetValue(IsStatusBarProperty); }
			set { SetValue(IsStatusBarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarShowSizeGrip")]
#endif
		public bool ShowSizeGrip {
			get { return (bool)GetValue(ShowSizeGripProperty); }
			set { SetValue(ShowSizeGripProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarGlyphSize")]
#endif
		public GlyphSize GlyphSize {
			get { return (GlyphSize)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarRotateWhenVertical")]
#endif
		public bool RotateWhenVertical {
			get { return (bool)GetValue(RotateWhenVerticalProperty); }
			set { SetValue(RotateWhenVerticalProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsCollapsed")]
#endif
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarAllowHide")]
#endif
		public DefaultBoolean AllowHide {
			get { return (DefaultBoolean)GetValue(AllowHideProperty); }
			set { SetValue(AllowHideProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarIsAllowHide")]
#endif
		public bool IsAllowHide {
			get {
				if (AllowHide == DefaultBoolean.True) return true;
				if (AllowHide == DefaultBoolean.False) return false;
				if (IsMainMenu || IsStatusBar) return false;
				return true;
			}
		}
		public BarItemAlignment BarItemsAlignment {
			get { return (BarItemAlignment)GetValue(BarItemsAlignmentProperty); }
			set { SetValue(BarItemsAlignmentProperty, value); }
		}
		Size defaultBarSize;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size DefaultBarSize {
			get { return defaultBarSize; }
			set { defaultBarSize = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CreatedByCustomizationDialog { get; set; }
		bool isRemoved;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsRemoved {
			get { return isRemoved; }
			set {
				if (isRemoved == value)
					return;
				isRemoved = value;
				OnIsRemovedChanged();
			}
		}
		internal bool IsPrivate {
			get { return isPrivate; }
			set {
				if (IsPrivate == value) return;
				isPrivate = value;
				OnIsPrivateChanged();
			}
		}
		internal bool ActualAllowCustomizationMenu {
			get { return AllowCustomizationMenu && !IsPrivate; }
		}
		public event BarVisibleChangedEventHandler VisibleChanged;
		public event BarControlLoadedEventHandler BarControlLoaded;
		protected virtual void OnIsRemovedChanged() {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.OnBarIsRemovedChanged();
		}
		protected virtual void RaiseVisibleChangedEvent() {
			if (VisibleChanged == null) return;
			BarControl barControl = DockInfo != null ? DockInfo.BarControl : null;
			BarVisibileChangedEventArgs e = new BarVisibileChangedEventArgs(this, barControl, !Visible, Visible);
			VisibleChanged(this, e);
		}
		protected internal virtual void RaiseBarControlLoadedEvent() {
			if (BarControlLoaded == null) return;
			BarControl barControl = DockInfo != null ? DockInfo.BarControl : null;
			BarControlLoadedEventArgs e = new BarControlLoadedEventArgs(this, barControl);
			BarControlLoaded(this, e);
		}
		protected internal bool GetIsMultiLine() {
			return IsMultiLine || (DockInfo.Container != null && DockInfo.Container.IsFloating);
		}
		void OnRotateWhenVerticalChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl == null) return;
			foreach (BarItemLinkInfo info in DockInfo.BarControl.Items) {
				info.LinkControl.UpdateOrientation();
			}
		}
		protected override void OnVisibleChanged(DependencyPropertyChangedEventArgs e) {
			DockInfo.BarControl.Do(x => x.OnLayoutPropertyChanged());
			RaiseVisibleChangedEvent();
		}
		protected virtual void OnAllowQuickCuztomizationChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.UpdateBarControlProperties();
		}
		protected virtual void OnShowDragWidgetChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.UpdateBarControlProperties();
		}
		protected virtual void OnIsStatusBarChanged(DependencyPropertyChangedEventArgs e) {
			if (IsStatusBar)
				IsMainMenu = false;			
			if (Manager != null) {
				if (IsStatusBar)
					Manager.StatusBar = this;
				else if (Manager.StatusBar == this)
					Manager.StatusBar = null;
			}
			if (DockInfo.BarControl != null) {
				DockInfo.BarControl.UpdateBarControlProperties();
				DockInfo.BarControl.UpdateVisualState();
			}
		}
		protected virtual void OnShowSizeGripChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.UpdateBarControlProperties();
		}
		protected virtual void OnIsMainMenuChanged(DependencyPropertyChangedEventArgs e) {
			if (IsMainMenu)
				IsStatusBar = false;			
			if (Manager != null) {
				if (IsMainMenu)
					Manager.MainMenu = this;
				else if (Manager.MainMenu == this)
					Manager.MainMenu = null;
			}
			if (DockInfo.BarControl != null) {
				DockInfo.BarControl.UpdateBarControlProperties();
				DockInfo.BarControl.UpdateVisualState();
			}
		}
		protected virtual void OnDisableCloseChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo != null) DockInfo.OnShowCloseButtonInFloatBarChanged();
		}		
		protected virtual void OnAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.InvalidateMeasurePanel();
		}
		protected virtual void OnUseWholeRowChanged(DependencyPropertyChangedEventArgs e) {
			CoerceValue(AllowCollapseProperty);
			DockInfo.BarControl.Do(x => x.OnLayoutPropertyChanged());
		}
		protected virtual object OnAllowCollapseCoerce(object baseValue) {
			if (IsUseWholeRow) return false;
			return baseValue;
		}
		protected void OnAllowCollapseChanged(DependencyPropertyChangedEventArgs e) {
			CoerceValue(IsCollapsedProperty);
		}
		protected virtual void OnHideWhenEmptyChanged(bool oldValue) {
			DockInfo.With(x=>x.BarControl).Do(x=>x.UpdateVisibility());
		}
		protected virtual object OnIsCollapsedCoerce(object baseValue) {
			bool isCollapsed = (bool)baseValue;
			return AllowCollapse && isCollapsed;
		}
		protected void OnIsCollapsedChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.ActualShowContent = !IsCollapsed;
		}
		#region ILinksHolder Members        
		GlyphSize ILinksHolder.ItemsGlyphSize {
			get { return GlyphSize; }
		}
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return (GlyphSize)GetValue(BarManager.ToolbarGlyphSizeProperty);			
		}
		LinksHolderType ILinksHolder.HolderType {
			get { return LinksHolderType.Bar; }
		}		
		#endregion
		void OnGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			if (DockInfo.BarControl != null)
				DockInfo.BarControl.OnGlyphSizeChanged(e);
		}		
		#region Customization
		BarCustomizationControl customizationControl;
		protected BarCustomizationControl CustomizationControl {
			get { return customizationControl; }
			set {
				if (CustomizationControl == value) return;
				if (CustomizationControl != null)
					RemoveItems(CustomizationControl);
				customizationControl = value;
				if (CustomizationControl != null)
					AddItems(CustomizationControl);
			}
		}
		protected virtual void AddItems(BarCustomizationControl newValue) {
			foreach (BarItem item in newValue.Items)
				Manager.Items.Add(item);
			ItemLinks.Clear();
			foreach (BarItemLinkBase link in newValue.ItemLinks)
				ItemLinks.Add(link);
		}
		protected virtual void RemoveItems(BarCustomizationControl oldValue) {
			List<BarItem> items = new List<BarItem>(Items.OfType<BarItem>());
			foreach (BarItem item in items) {
				if (oldValue.Items.Contains(item))
					Items.Remove(item);
			}
			List<BarItemLinkBase> links = new List<BarItemLinkBase>(ItemLinks);
			foreach (BarItemLinkBase link in links) {
				if (oldValue.ItemLinks.Contains(link))
					ItemLinks.Remove(link);
			}
			BarDragProvider.RemoveUnnesessarySeparators(ItemLinks);
		}
		#endregion
		#region IBarManagerControllerAction Members
		IActionContainer IControllerAction.Container { get; set; }
		void IControllerAction.Execute(DependencyObject context) {
			var manager = context as BarManager;
			int index = InsertBarAction.GetBarIndex(this);
			var controller = ((IControllerAction)this).Container;
			if (controller != null) {
				manager = manager ?? controller.GetBarManager();
				if (manager != null) {
					if (!manager.Bars.Contains(this)) {
						if (index != -1)
							manager.Bars.Insert(index, this);
						else
							manager.Bars.Add(this);
					}
				}
			}
		}
		object IBarManagerControllerAction.GetObject() {
			return this;
		}
		#endregion
		void OnIsPrivateChanged() {
		}
		bool isPrivate;		
		#region IMergingSupport Members
		bool IMergingSupport.IsMerged { get { return ((ILinksHolder)this).MergedParent != null; } }
		bool IMergingSupport.IsAutomaticallyMerged { get; set; }
		object IMergingSupport.MergingKey { get { return typeof(Bar); } }
		BarDockInfo IBar.DockInfo { get { return DockInfo; } }
		BarContainerControl IBar.OriginContainer { get; set; }
		protected internal ToolBarControlBase ToolBar { get; set; }
		bool showInOriginContainer;
		bool IBar.ShowInOriginContainer {
			get { return showInOriginContainer; }
			set {
				if (value == showInOriginContainer) return;
				showInOriginContainer = value;
				if (ToolBar != null) {
					if (value) {
						ToolBar.Visibility = Visibility.Visible;
						DockInfo.Container = ((IBar)this).OriginContainer;
						DockInfo.BarControl = ToolBar.BarControl;						
					} else
						ToolBar.Visibility = Visibility.Collapsed;
				}
			}
		}
		bool IBar.CanBind(BarContainerControl container, object binderKey) {
			if (!String.IsNullOrEmpty(DockInfo.ContainerName) && DockInfo.ContainerName != BarDockInfo.FloatingContainerName && binderKey == BarRegistratorKeys.BarTypeKey)
				return false;
			bool canBindContainer = DockInfo.Container == null;
			if(container!=null && DockInfo.Container != null) {
				var oldLevel = ScopeTree.GetLevel(BarNameScope.GetScope(this), BarNameScope.GetScope(DockInfo.Container), ScopeSearchSettings.Descendants | ScopeSearchSettings.Local);
				var newLevel = ScopeTree.GetLevel(BarNameScope.GetScope(this), BarNameScope.GetScope(container), ScopeSearchSettings.Descendants | ScopeSearchSettings.Local);
				canBindContainer = newLevel > 0 && newLevel < oldLevel || newLevel == oldLevel && DockInfo.Container.IsFloating != container.IsFloating;
			}
			return !DockInfo.IsLocked && canBindContainer && BarManager.GetBarManager(this).If(x => x.Bars.Contains(this)).ReturnSuccess();
		}
		int IBar.Index { get; set; }
		bool IMergingSupport.CanMerge(IMergingSupport second) { return CanMerge((IMergingSupport)second); }
		bool CanMerge(IMergingSupport bar) {
			return (bar is Bar) && (CanBeMerged() && IsMainMenu == ((Bar)bar).IsMainMenu && IsStatusBar == ((Bar)bar).IsStatusBar);
		}
		protected internal bool CanBeMerged() {
			var mergeStyle = MergingProperties.GetToolBarMergeStyle(this);
			if (IsMainMenu && !mergeStyle.HasFlag(ToolBarMergeStyle.MainMenu))
				return false;
			else if (IsStatusBar && !mergeStyle.HasFlag(ToolBarMergeStyle.StatusBar))
				return false;
			else if (!IsMainMenu && !IsStatusBar && !mergeStyle.HasFlag(ToolBarMergeStyle.ToolBars))
				return false;			
			return true;
		}
		void IMergingSupport.Merge(IMergingSupport second) {
			((ILinksHolder)this).Merge((Bar)second);
		}
		void IMergingSupport.Unmerge(IMergingSupport second) {
			((ILinksHolder)this).UnMerge((Bar)second);
		}
		bool IMergingSupport.IsMergedParent(IMergingSupport second) {
			return Equals(((ILinksHolder)this).MergedParent, second);
		}
		#endregion
		protected override IEnumerable<object> GetRegistratorKeys() {
			foreach (var key in base.GetRegistratorKeys())
				yield return key;
			yield return typeof(IMergingSupport);
			yield return BarRegistratorKeys.BarNameKey;
			yield return BarRegistratorKeys.BarTypeKey;
		}
		protected override object GetRegistratorName(object registratorKey) {
			if (Equals(typeof(IMergingSupport), registratorKey)) {
				return MergingProperties.GetName(this).WithString(x => x) ?? GetSpecialName() ?? Caption;
			}
			if (Equals(BarRegistratorKeys.BarNameKey, registratorKey))
				return DockInfo.ContainerName;
			if (Equals(BarRegistratorKeys.BarTypeKey, registratorKey))
				return DockInfo.ContainerType;
			return base.GetRegistratorName(registratorKey);
		}
		string GetSpecialName() {
			if (IsMainMenu)
				return MergingPropertiesHelper.MainMenuID;
			if (IsStatusBar)
				return MergingPropertiesHelper.StatusBarID;
			return null;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (Equals(e.Property, BarManager.BarManagerProperty)) {
				BarNameScope.GetService<IElementRegistratorService>(this).Changed(this, BarRegistratorKeys.BarTypeKey);
				BarNameScope.GetService<IElementRegistratorService>(this).Changed(this, BarRegistratorKeys.BarNameKey);
			}
			MergingPropertiesHelper.OnPropertyChanged(this, e, nameProperty: CaptionProperty);
			object oldName = MergingProperties.GetName(this).WithString(x => x) ?? Caption;
			object newName = newName = oldName;
			bool any = false;
			if (Equals(IsStatusBarProperty, e.Property)) {
				if ((bool)e.NewValue)
					newName = MergingPropertiesHelper.StatusBarID;
				else
					oldName = MergingPropertiesHelper.StatusBarID;
			}
			if (Equals(IsMainMenuProperty, e.Property)) {
				if ((bool)e.NewValue)
					newName = MergingPropertiesHelper.MainMenuID;
				else
					oldName = MergingPropertiesHelper.MainMenuID;
			}
			if (any)
				BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IMergingSupport), oldName, newName, true);
		}
	}
	public class BarCustomizationControl : FrameworkElement {
		ItemCollection<BarItem> barItems;
		ItemCollection<BarItemLinkBase> barItemLinks;
		public BarCustomizationControl() {
			barItems = new ItemCollection<BarItem>();
			barItemLinks = new ItemCollection<BarItemLinkBase>();
		}
		public ItemCollection<BarItem> Items { get { return barItems; } }
		public ItemCollection<BarItemLinkBase> ItemLinks { get { return barItemLinks; } }
	}
	public class ItemCollection<T> : ObservableCollection<T> { }
	public class BarItemGeneratorHelper<T> where T : DependencyObject {
		public T Owner { get; private set; }
		protected bool ItemLinksSourceElementGeneratesUniqueBarItem { get; private set; }
		protected CommonBarItemCollection TargetCollection { get; private set; }
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected readonly DependencyProperty AtatachedBehaviorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected readonly DependencyProperty ItemStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected readonly DependencyProperty ItemTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected DependencyProperty ItemsSourceProperty;
		public BarItemGeneratorHelper(
			T owner,
			DependencyProperty atatachedBehaviorProperty,
			DependencyProperty itemStyleProperty,
			DependencyProperty itemTemplateProperty,
			CommonBarItemCollection targetCollection,
			DependencyProperty itemTemplateSelectorProperty,
			bool itemLinksSourceElementGeneratesUniqueBarItem = false) {
			Owner = owner;
			AtatachedBehaviorProperty = atatachedBehaviorProperty;
			ItemStyleProperty = itemStyleProperty;
			ItemTemplateProperty = itemTemplateProperty;
			TargetCollection = targetCollection;
			ItemTemplateSelectorProperty = itemTemplateSelectorProperty;
			ItemLinksSourceElementGeneratesUniqueBarItem = itemLinksSourceElementGeneratesUniqueBarItem;
			if (!ItemLinksSourceElementGeneratesUniqueBarItem) {
				((IFrameworkInputElement)Owner).AddHandler(BarNameScope.ScopeChangedEvent, new RoutedEventHandler(OnScopeChanged));
			}
		}
		void OnScopeChanged(object sender, RoutedEventArgs e) {
			var lastparent = TreeHelper.GetParents(Owner, false, true).LastOrDefault();
			if (lastparent == null || PresentationSource.FromDependencyObject(lastparent) == null)
				return;
			if (ItemsSourceProperty != null)
				OnItemsSourceChanged(new DependencyPropertyChangedEventArgs(ItemsSourceProperty, Enumerable.Empty<object>(), null));
			BarNameScopeTreeWalker.DoWhenUnlocked(Recreate);			
		}		
		void Recreate() {
			OnItemsSourceChanged(new DependencyPropertyChangedEventArgs(ItemsSourceProperty, null, Owner.GetValue(ItemsSourceProperty)));
		}
		public void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			ItemsSourceProperty = e.Property;
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<T, BarItem>.OnItemsSourcePropertyChanged(Owner,
				   e,
				   AtatachedBehaviorProperty,
				   ItemTemplateProperty,
				   ItemTemplateSelectorProperty,
				   ItemStyleProperty,
				   owner => TargetCollection,
				   owner => new BarButtonItem(),
				   (index, item) => InsertItemAction(index, item),
				   useDefaultTemplateSelector: true,
				   customClear: i => true);
			} else if(Owner is ILinksHolder) {
				PopulateItemsHelper.GenerateItems(e, () => new SingleGroupGenerator(new BarItemsGenerator((ILinksHolder)Owner, BarManager.GetBarManager(Owner).If(x => x.AllowGlyphTheming).Return(x => ImageType.GrayScaled, () => ImageType.Colored))));
			}
		}
		void InsertItemAction(int index, object item) {
			var items = BarNameScope
				.GetService<IElementRegistratorService>(Owner)
				.GetElements<IFrameworkInputElement>()
				.OfType<BarItemLink>()
				.Select(x => x.Item)
				.Distinct()
				.Where(x => x != null);
			Style itemStyle = Owner.GetValue(ItemStyleProperty) as Style;
			DataTemplate itemTemplate = Owner.GetValue(ItemTemplateProperty) as DataTemplate;
			BarItemLinkBase link = null;
			BarItem barItem = item as BarItem;
			if(!ItemLinksSourceElementGeneratesUniqueBarItem) {
				barItem.SetItemSourceData(itemStyle, itemTemplate);
				link = items.FirstOrDefault(it => it.CompareWithItemCreatedFromSource(barItem)).With(x => x.CreateLink());
			}
			barItem.SetItemSourceData(itemStyle, itemTemplate);
			barItem.IsPrivate = true;
			if(index < 0 || index >= TargetCollection.Count) {
				if(link == null)
					TargetCollection.Add(barItem);
				else
					TargetCollection.Add(link);
			} else {
				if(link == null)
					TargetCollection.Insert(index, barItem);
				else
					TargetCollection.Insert(index, link);
			}
		}
	}
}
