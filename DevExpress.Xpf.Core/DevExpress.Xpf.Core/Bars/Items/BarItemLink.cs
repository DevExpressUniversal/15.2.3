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
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Customization;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Bars {
	[Flags]
	public enum RibbonItemStyles { Default = 0, Large = 1, SmallWithText = 2, SmallWithoutText = 4, All = Large | SmallWithText | SmallWithoutText }
	public enum BarItemDisplayMode { Default, Content, ContentAndGlyph }
	public enum BarItemMergeType { Default, Add, Replace, MergeItems, Remove }
	public enum BarItemAlignment { Default, Near, Far }
	public class BarItemLinkInfoReferenceCollection : ObservableCollection<BarItemLinkInfo> {
	}
	public abstract class BarItemLinkBase : FrameworkContentElement, IBarItem, IBarManagerControllerAction, ICloneable{
		#region static
		public static readonly DependencyProperty ActualIsVisibleProperty;
		static readonly DependencyPropertyKey ActualIsVisiblePropertyKey;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty MergeTypeProperty;
		public static readonly DependencyProperty MergeOrderProperty;
		public static readonly DependencyProperty CustomResourcesProperty;
		public static readonly DependencyProperty AlignmentProperty;
		public static readonly DependencyProperty HoldersIsVisibleProperty;
		public static readonly DependencyProperty OverrideItemDataContextProperty;
		public static readonly DependencyProperty VerticalAlignmentProperty;
		public static readonly RoutedEvent IsRemovedChangedEvent;
		public static readonly DependencyProperty SectorIndexProperty;
		public static readonly DependencyProperty IsRemovedProperty;
		public static readonly DependencyProperty IndexProperty;		
		static BarItemLinkBase() {
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(0));
			IsRemovedProperty = DependencyPropertyManager.Register("IsRemoved", typeof(bool), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(false, (d, e) => ((BarItemLinkBase)d).OnIsRemovedChanged()));		
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsVisiblePropertyChanged)));
			ActualIsVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsVisible", typeof(bool), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnActualIsVisiblePropertyChanged)));
			ActualIsVisibleProperty = ActualIsVisiblePropertyKey.DependencyProperty;
			MergeTypeProperty = DependencyPropertyManager.Register("MergeType", typeof(BarItemMergeType), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(BarItemMergeType.Default));
			MergeOrderProperty = DependencyPropertyManager.Register("MergeOrder", typeof(int), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(-1));
			SectorIndexProperty = DependencyPropertyManager.Register("SectorIndex", typeof(int), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(-1, (d,e)=>((BarItemLinkBase)d).OnSectorIndexChanged()));
			CustomResourcesProperty = DependencyPropertyManager.Register("CustomResources", typeof(ResourceDictionary), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCustomResourcesPropertyChanged)));
			AlignmentProperty = DependencyPropertyManager.Register("Alignment", typeof(BarItemAlignment), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(BarItemAlignment.Default, new PropertyChangedCallback(OnAlignmentPropertyChanged)));
			HoldersIsVisibleProperty = DependencyPropertyManager.Register("HoldersIsVisible", typeof(bool?), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHoldersIsVisiblePropertyChanged)));
			OverrideItemDataContextProperty = DependencyPropertyManager.Register("OverrideItemDataContext", typeof(bool), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnOverrideItemDataContextPropertyChanged)));
			NameProperty.OverrideMetadata(typeof(BarItemLinkBase), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnNamePropertyChanged)));
			VerticalAlignmentProperty = DependencyPropertyManager.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(BarItemLinkBase), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, new PropertyChangedCallback(OnVerticalAlignmentPropertyChanged)));
			IsRemovedChangedEvent = EventManager.RegisterRoutedEvent("IsRemovedChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BarItemLinkBase));
		}
		protected static void OnNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnNameChanged(e.NewValue as string, e.OldValue as string);
		}
		protected static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnAlignmentChanged(e);
		}
		protected static void OnCustomResourcesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnCustomResourcesChanged(e);
		}
		protected static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnIsEnabledChanged(d,e);
		}
		protected static void OnActualIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnActualIsVisibleChanged(e);
		}
		protected static void OnIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnIsVisibleChanged(e);
		}
		protected static void OnHoldersIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnHoldersIsVisibleChanged((bool?)e.OldValue);
		}
		protected static void OnOverrideItemDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnOverrideItemDataContextChanged((bool)e.OldValue);
		}
		protected static void OnVerticalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkBase)d).OnVerticalAlignmentChanged((VerticalAlignment)e.OldValue);
		}
		#endregion
		BarItemLinkInfoReferenceCollection linkInfos;
		protected BarItemLinkBase() {
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		}		
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CreatedByCustomizationDialog { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsRemoved {
			get { return (bool)GetValue(IsRemovedProperty); }
			set { SetValue(IsRemovedProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarItemLinkInfoReferenceCollection LinkInfos {
			get {
				if(linkInfos == null)
					linkInfos = new BarItemLinkInfoReferenceCollection();
				return linkInfos;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool? HoldersIsVisible {
			get { return (bool?)GetValue(HoldersIsVisibleProperty); }
			set { SetValue(HoldersIsVisibleProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseActualIsVisible")]
#endif
		public bool ActualIsVisible {
			get { return (bool)GetValue(ActualIsVisibleProperty); }
			internal set { this.SetValue(ActualIsVisiblePropertyKey, value); }
		}
		protected internal int ActualMergeOrder {
			get {
				BarItemLink barItemLink = this as BarItemLink;
				if(barItemLink == null || barItemLink.Item == null || MergeOrder != -1)
					return MergeOrder;
				return barItemLink.Item.MergeOrder;
			}
		}
		protected internal BarItemMergeType ActualMergeType {
			get {
				BarItemLink barItemLink = this as BarItemLink;
				if(barItemLink == null || barItemLink.Item == null) {
					return MergeType == BarItemMergeType.Default ? BarItemMergeType.Add : MergeType;
				}
				if(barItemLink.MergeType != BarItemMergeType.Default)
					return barItemLink.MergeType;
				return barItemLink.Item.MergeType == BarItemMergeType.Default ? BarItemMergeType.Add : barItemLink.Item.MergeType;
			}
		}
		bool isPrivate = false;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CommonBarItemCollectionLink { get; set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseIsPrivate")]
#endif
		public virtual bool IsPrivate { get { return isPrivate; } protected internal set { isPrivate = value; } }
		protected internal virtual bool IsPrivateLinkInCustomizationMode { get; set; }		
		public bool OverrideItemDataContext {
			get { return (bool)GetValue(OverrideItemDataContextProperty); }
			set { SetValue(OverrideItemDataContextProperty, value); }
		}		
		bool allowShowCustomizationMenu = true;
		protected internal virtual bool AllowShowCustomizationMenu { get { return allowShowCustomizationMenu; } set { allowShowCustomizationMenu = value; } }
		bool isLinkEnabledCore = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(true)]
		public bool IsLinkEnabled {
			get {
				return isLinkEnabledCore;
			}
			set {
				isLinkEnabledCore = value;
				CoerceIsEnabledProperty();
			}
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseLinkTypeName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string LinkTypeName {
			get { return GetType().FullName; }
			set { }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseMergeType")]
#endif
		public BarItemMergeType MergeType {
			get { return (BarItemMergeType)GetValue(MergeTypeProperty); }
			set { SetValue(MergeTypeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseMergeOrder")]
#endif
		public int MergeOrder {
			get { return (int)GetValue(MergeOrderProperty); }
			set { SetValue(MergeOrderProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseSectorIndex")]
#endif
		public int SectorIndex {
			get { return (int)GetValue(SectorIndexProperty); }
			set { SetValue(SectorIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseCustomResources")]
#endif
		public ResourceDictionary CustomResources {
			get { return (ResourceDictionary)GetValue(CustomResourcesProperty); }
			set { SetValue(CustomResourcesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkBaseLinks"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		private BarItemLinkCollection links;
		public BarItemLinkCollection Links {
			get { return links; }
			internal set {
				if (Equals(links, value))
					return;
				links = value;
				OnLinksChanged();
			}
		}		
		public BarItemAlignment Alignment {
			get { return (BarItemAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		public VerticalAlignment VerticalAlignment {
			get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
			set { SetValue(VerticalAlignmentProperty, value); }
		}
		public event RoutedEventHandler IsRemovedChanged {
			add { this.AddHandler(IsRemovedChangedEvent, value); }
			remove { this.RemoveHandler(IsRemovedChangedEvent, value); }
		}
		protected internal virtual BarItemAlignment ActualAlignment { get { return Alignment; } }				
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Initialize();
		}
		protected internal virtual void OnLinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) { }
		protected internal abstract void Initialize();
		protected internal virtual void UpdateActualIsVisible() {
			ActualIsVisible = GetActualIsVisible();
		}
		protected virtual bool GetActualIsVisible() {
			return IsVisible && HoldersIsVisible.Return((e) => e.Value, () => true) && !IsRemoved;
		}
		protected internal virtual void UpdateProperties() {
			UpdateActualIsVisible();
		}
		protected internal virtual BarItemLinkControlBase CreateBarItemLinkControl() {
			return BarItemLinkControlCreator.Default.Create(GetType(), this);
		}
		protected virtual void OnIsRemovedChanged() {
			UpdateActualIsVisible();
			this.RaiseEvent(new RoutedEventArgs(IsRemovedChangedEvent, this));
		}
		protected virtual void OnActualIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVisibility());
			if(Links != null && Links.Holder != null)
				UpdateSeparatorsVisibility(Links.Holder, false);
		}
		protected virtual void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualIsVisible();
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVisibility());
		}
		protected virtual void OnHoldersIsVisibleChanged(bool? oldValue) {
			UpdateActualIsVisible();
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVisibility());
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateIsEnabled());
		}
		protected virtual void OnCustomResourcesChanged(DependencyPropertyChangedEventArgs e) {
			foreach(BarItemLinkInfo linkInfo in LinkInfos) {
				if(linkInfo.LinkControl != null)
					linkInfo.LinkControl.Resources = CustomResources;
			}
		}		
		protected internal virtual void OnItemIsEnabledChanged() {
			UpdateByItemIsEnabled();
		}
		protected virtual void CoerceIsEnabledProperty() {
			CoerceValue(UIElement.IsEnabledProperty);
		}
		protected internal virtual void UpdateByItemIsEnabled() {
			CoerceIsEnabledProperty();
			ExecuteActionOnBaseLinkControls(lc => lc.UpdateIsEnabled());
		}
		protected virtual void OnLinksChanged() {
			UpdateSeparatorsVisibility();
		}
		protected internal virtual void OnItemContentChanged() {
		}
		protected internal virtual void UpdateLinkControlsGlyphParams() {
			foreach(BarItemLinkInfo info in LinkInfos) {
				BarItemLink link = info.LinkBase as BarItemLink;
				if(link == null) continue;
				link.UpdateLinkControlGlyphParams(info.LinkControl as BarItemLinkControl);
			}
		}
		internal bool CalculateVisibility(BarItemLinkInfo linkInfo) { 
			bool res = ActualIsVisible;
			if(linkInfo == null) return res;
			return res;
		}
		internal bool CalculateIsEnabled(BarItemLinkInfo linkInfo) {
			if (BarManagerCustomizationHelper.IsInCustomizationMode(this))
				return true;
			bool res = IsEnabled;
			if (linkInfo == null) return res;
			return res;
		}
		protected virtual void UpdateEditContentMargin() {
		}
		protected internal void ExecuteActionOnLinkControls<T>(LinkControlAction<T> action) where T:BarItemLinkControlBase {
			foreach(BarItemLinkInfo info in LinkInfos.ToList()) {
				(info.LinkControl as T).Do(lc=>action(lc));
			}
		}
		protected internal void ExecuteActionOnLinkControls(LinkControlAction<BarItemLinkControl> action) {
			ExecuteActionOnLinkControls<BarItemLinkControl>(action);
		}
		protected internal void ExecuteActionOnBaseLinkControls(LinkControlAction<BarItemLinkControlBase> action) {
			ExecuteActionOnLinkControls<BarItemLinkControlBase>(action);			
		}
		internal bool lockUpdateLinkControl = false;
		protected internal static void UpdateSeparatorsVisibility(ILinksHolder holder, bool force = false) {
			if (holder == null)
				return;
			if (force)
				UpdateSeparatorsVisibilityImpl(holder);
			else {
				holder.ImmediateActionsManager.EnqueueAction(new UpdateSeparatorsVisibilityAction(holder));
			}
		}
		class UpdateSeparatorsVisibilityAction : IAggregateAction {
			ILinksHolder holder;
			public UpdateSeparatorsVisibilityAction(ILinksHolder holder) {
				this.holder = holder;
			}
			public bool CanAggregate(IAction action) {
				return action is UpdateSeparatorsVisibilityAction;
			}
			public void Execute() {
				UpdateSeparatorsVisibilityImpl(holder);
			}
		}		
		static void UpdateSeparatorsVisibilityImpl(ILinksHolder holder) {
			var links = holder.With(x => x.ActualLinks);
			if(links == null)
				return;
			List<int> separatorIndexes = new List<int>();
			List<bool> separatorVisibilities = new List<bool>();
			for (int i = 0; i < links.Count; i++)
				if (links[i] != null && (links[i] is BarItemLinkSeparator || (links[i] as BarItemLink).With(x => x.Item as BarItemSeparator) != null)) {
					separatorIndexes.Add(i);
					separatorVisibilities.Add(links[i].GetActualIsVisible());
				}
			int visibleItemsCount = 0;
			for (int i = 0; i < links.Count; i++) {
				if (separatorIndexes.Contains(i)) {
					int separatorVisiblityIndex = separatorIndexes.IndexOf(i);
					if (!separatorVisibilities[separatorVisiblityIndex])
						continue;
					separatorVisibilities[separatorVisiblityIndex] = visibleItemsCount != 0;
					visibleItemsCount = 0;
					continue;
				}
				if (links[i].ActualIsVisible)
					visibleItemsCount++;
			}
			if (visibleItemsCount == 0 && separatorVisibilities.Count > 0)
				separatorVisibilities[separatorVisibilities.Count - 1] = false;
			for(int i = 0; i < separatorIndexes.Count; i++) {
				bool isHeaderNear = false;
				if(separatorIndexes[i] - 1 >= 0)
					isHeaderNear |= (links[separatorIndexes[i] - 1] is BarItemLinkMenuHeader);
				if(separatorIndexes[i] + 1 < links.Count)
					isHeaderNear |= (links[separatorIndexes[i] + 1] is BarItemLinkMenuHeader);
				separatorVisibilities[i] &= !isHeaderNear;
			}
			for(int i = 0; i < separatorIndexes.Count; i++)
				links[separatorIndexes[i]].ActualIsVisible = separatorVisibilities[i] && links[separatorIndexes[i]].IsVisible;
		}
		protected virtual void UpdateSeparatorsVisibility() {
			if (Links != null)
				UpdateSeparatorsVisibility(Links.Holder);
		}
		protected void OnAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateAlignment();
		}
		protected internal virtual void UpdateAlignment() {
		}
		protected virtual void OnNameChanged(string newValue, string oldValue) {
		}
		protected virtual void OnSectorIndexChanged() {
			ExecuteActionOnLinkControls((lc) => lc.OnSourceSectorIndexChanged());
		}				
		List<WeakReference> clonedLinks;
		protected List<WeakReference> ClonedLinks {
			get {
				if(clonedLinks == null)
					clonedLinks = new List<WeakReference>();
				return clonedLinks;
			}
		}
		BarItemLinkBase sourceLink;
		protected internal BarItemLinkBase SourceLink {
			get { return sourceLink; }
			set {
				if(sourceLink != value) {
					var oldValue = sourceLink;
					sourceLink = value;
					OnSourceLinkChanged(oldValue);
				}
			}
		}
		protected virtual void OnSourceLinkChanged(BarItemLinkBase oldValue) {
			if(oldValue != null) {
				RemoveFromClonedLinks(oldValue);
			}
			if(SourceLink != null)
				sourceLink.ClonedLinks.Add(new WeakReference(this));
		}
		protected void RemoveFromClonedLinks(BarItemLinkBase sourceLink) {
			foreach(WeakReference linkRef in sourceLink.ClonedLinks)
				if(linkRef.IsAlive && linkRef.Target == this) {
					sourceLink.ClonedLinks.Remove(linkRef);
					break;
				}
		}
		public virtual void Assign(BarItemLinkBase link) {
			SourceLink = link;
			IsPrivate = link.IsPrivate;
			IsLinkEnabled = link.IsLinkEnabled;
			IsVisible = link.IsVisible;
			CommonBarItemCollectionLink = link.CommonBarItemCollectionLink;
			BindingOperations.SetBinding(this, BarItemLinkBase.HoldersIsVisibleProperty, new Binding() { Source = link, Path = new PropertyPath("HoldersIsVisible") });
			BindingOperations.SetBinding(this, BarItemLinkBase.IsEnabledProperty, new Binding() { Source = link, Path = new PropertyPath("IsEnabled") });
		}
		protected bool IsInitializationSuppressed { get; private set; } 
		void SuppressInitialization() {
			IsInitializationSuppressed = true;
		}
		void UnsuppressInitialization() {
			IsInitializationSuppressed = false;
			Initialize();
		}
		public void UpdateLinkControlsActualGlyph() {
			ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyph());
		}
		protected virtual void OnOverrideItemDataContextChanged(bool oldValue) {
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateDataContext());
		}
		protected virtual void OnVerticalAlignmentChanged(VerticalAlignment oldValue) {
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVerticalAlignment());
		}
		#region IBarManagerControllerAction Members
		IActionContainer IControllerAction.Container { get; set; }
		void IControllerAction.Execute(DependencyObject context) {
			var controller = ((IControllerAction)this).Container;
			if (controller != null) {
				CollectionActionHelper.Execute(new CollectionActionWrapper(this, controller, context));
			}
		}
		object IBarManagerControllerAction.GetObject() {
			return this;
		}
		#endregion
		#region ICloneable Members
		protected internal string BasedOn { get; private set; }		
		object ICloneable.Clone() {
			BarItemLinkBase linkBase = (BarItemLinkBase)this.GetType().GetConstructor(new Type[]{}).Invoke(new object[] {});
			linkBase.SetBinding(ItemsAttachedBehaviorProperties.SourceProperty, new Binding() { Path = new PropertyPath(ItemsAttachedBehaviorProperties.SourceProperty), Source = this });
			linkBase.SuppressInitialization();
			linkBase.Assign(this);
			linkBase.UnsuppressInitialization();
			linkBase.BasedOn = Name;
			linkBase.Name = CloneNameHelper.GetCloneName(this, linkBase);
			return linkBase;
		}
		#endregion
		public virtual void Clear() {
			Clear(true);
		}		
		protected internal virtual void Clear(bool clearItem) {
			Links = null;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
	}	
	public class BarItemLink : BarItemLinkBase, IMultipleElementRegistratorSupport {
		#region        
		public static readonly DependencyProperty UserGlyphSizeProperty;
		public static readonly DependencyProperty BarItemNameProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty UserContentProperty;
		public static readonly DependencyProperty ActualContentProperty;
		static readonly DependencyPropertyKey ActualContentPropertyKey;
		public static readonly DependencyProperty ActualDescriptionProperty;
		static readonly DependencyPropertyKey ActualDescriptionPropertyKey;
		public static readonly DependencyProperty ActualContentTemplateProperty;
		static readonly DependencyPropertyKey ActualContentTemplatePropertyKey;
		public static readonly DependencyProperty ActualSuperTipProperty;
		static readonly DependencyPropertyKey ActualSuperTipPropertyKey;
		public static readonly DependencyProperty ActualHintProperty;
		static readonly DependencyPropertyKey ActualHintPropertyKey;
		public static readonly DependencyProperty UserGlyphAlignmentProperty; 
		public static readonly DependencyProperty BarItemDisplayModeProperty;
		public static readonly DependencyProperty KeyGestureTextProperty;
		static readonly DependencyPropertyKey KeyGestureTextPropertyKey;
		public static readonly DependencyProperty HasKeyGestureProperty;
		static readonly DependencyPropertyKey HasKeyGesturePropertyKey;
		public static readonly DependencyProperty HasHintProperty;
		static readonly DependencyPropertyKey HasHintPropertyKey;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty KeyTipProperty;
		public static readonly DependencyProperty KeyTipDropDownProperty;
		public static readonly DependencyProperty ShowScreenTipProperty;
		public static readonly DependencyProperty ShowKeyGestureProperty;								
		static BarItemLink() {
			BarManager.ToolbarGlyphSizeProperty.OverrideMetadata(typeof(BarItemLink), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.Inherits, (o, e) => ((BarItemLink)o).OnToolbarGlyphSizeChanged((GlyphSize)e.OldValue, (GlyphSize)e.NewValue)));
			ShowKeyGestureProperty = DependencyPropertyManager.Register("ShowKeyGesture", typeof(bool?), typeof(BarItemLink), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLink)d).ExecuteActionOnLinkControls(x => x.UpdateShowKeyGesture())));
			BarItemNameProperty = DependencyPropertyManager.Register("BarItemName", typeof(string), typeof(BarItemLink), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnBarItemNamePropertyChanged)));
			UserContentProperty = DependencyPropertyManager.Register("UserContent",typeof(object),typeof(BarItemLink), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLink)d).UpdateProperties()));
			ActualContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent", typeof(object), typeof(BarItemLink), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualContentPropertyKeyChanged)));
			ActualContentProperty = ActualContentPropertyKey.DependencyProperty;
			HasHintPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasHint", typeof(bool), typeof(BarItemLink), new FrameworkPropertyMetadata(false));
			HasHintProperty = HasHintPropertyKey.DependencyProperty;
			HasKeyGesturePropertyKey = DependencyPropertyManager.RegisterReadOnly("HasKeyGesture", typeof(bool), typeof(BarItemLink), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasKeyGesturePropertyChanged)));
			HasKeyGestureProperty = HasKeyGesturePropertyKey.DependencyProperty;
			ActualContentTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContentTemplate", typeof(DataTemplate), typeof(BarItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnActualContentTemplatePropertyChanged)));
			ActualContentTemplateProperty = ActualContentTemplatePropertyKey.DependencyProperty;
			ActualDescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescription", typeof(string), typeof(BarItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnActualDescriptionPropertyChanged)));
			ActualDescriptionProperty = ActualDescriptionPropertyKey.DependencyProperty;
			ActualSuperTipPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualSuperTip", typeof(SuperTip), typeof(BarItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnActualSuperTipPropertyChanged)));
			ActualSuperTipProperty = ActualSuperTipPropertyKey.DependencyProperty;
			ActualHintPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHint", typeof(object), typeof(BarItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnActualHintPropertyChanged)));
			ActualHintProperty = ActualHintPropertyKey.DependencyProperty;
			UserGlyphSizeProperty = DependencyPropertyManager.Register("UserGlyphSize", typeof(GlyphSize), typeof(BarItemLink), new FrameworkPropertyMetadata(GlyphSize.Default, new PropertyChangedCallback(OnGlyphSizePropertyChanged)));
			BarItemDisplayModeProperty = DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(BarItemLink), new FrameworkPropertyMetadata(BarItemDisplayMode.Default, new PropertyChangedCallback(OnBarItemDisplayModePropertyChanged)));
			KeyGestureTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("KeyGestureText", typeof(string), typeof(BarItemLink), new FrameworkPropertyMetadata(string.Empty));
			KeyGestureTextProperty = KeyGestureTextPropertyKey.DependencyProperty;
			UserGlyphAlignmentProperty = DependencyPropertyManager.Register("UserGlyphAlignment", typeof(Dock?), typeof(BarItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUserGlyphAlignmentChanged)));
			RibbonStyleProperty = DependencyPropertyManager.Register("RibbonStyle", typeof(RibbonItemStyles), typeof(BarItemLink), new FrameworkPropertyMetadata(RibbonItemStyles.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRibbonStylePropertyChanged)));
			KeyTipProperty = DependencyPropertyManager.Register("KeyTip", typeof(string), typeof(BarItemLink), new FrameworkPropertyMetadata(string.Empty));
			KeyTipDropDownProperty = DependencyPropertyManager.Register("KeyTipDropDown", typeof(string), typeof(BarItemLink), new FrameworkPropertyMetadata(string.Empty));
			ShowScreenTipProperty = DependencyPropertyManager.Register("ShowScreenTip", typeof(DevExpress.Utils.DefaultBoolean), typeof(BarItemLink), new FrameworkPropertyMetadata(DevExpress.Utils.DefaultBoolean.Default));			
		}
		protected static void OnActualContentPropertyKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnActualContentPropertyKeyChanged(e);
		}
		protected static void OnActualContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnActualDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnActualSuperTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnActualHintPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnHasKeyGesturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnHasKeyGestureChanged(e);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).ribbonStyleInternal = (RibbonItemStyles)e.NewValue;
			((BarItemLink)d).OnRibbonStyleChanged(e);
		}
		protected static void OnGlyphSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnGlyphSizeChanged(e);
		}
		protected static void OnUserGlyphAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnUserGlyphAlignmentChanged(e.OldValue as Dock?);
		}
		protected static void OnBarItemDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnBarItemDisplayModeChanged(e);
		}
		static void OnBarItemNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLink)d).OnBarItemNameChanged(e);
		}
		public static BarItemLinkBase XtraCreateItemLinksItemCore(BarManager manager, XtraCreateCollectionItemEventArgs e) {
			BarItemLinkBase linkBase = (BarItemLinkBase)ItemLinkFactory.Default.CreateObject((string)e.Item.ChildProperties["LinkTypeName"].Value);			
			BarItemLink link = linkBase as BarItemLink;
			bool value;
			bool.TryParse(e.Item.ChildProperties["CommonBarItemCollectionLink"].Value as string, out value);
			link.CommonBarItemCollectionLink = value;
			var barItemNameProperty = e.Item.ChildProperties["BarItemName"];
			if(link != null && barItemNameProperty != null)
				link.BarItemName = (string)barItemNameProperty.Value;
			linkBase.Initialize();
			return linkBase;
		}
		#endregion
		LockableValueStorage<BarItem> item;
		bool hasStrongLinkedItem = false;
		protected internal bool HasStrongLinkedItem { get { return hasStrongLinkedItem; } }
		public BarItemLink() {
			item = new LockableValueStorage<BarItem>();
			item.ValueChanging += OnItemPropertyChanging;
			item.ValueChanged += OnItemValueChanged;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBarItemName")]
#endif
		public string BarItemName {
			get { return (string)GetValue(BarItemNameProperty); }
			set { SetValue(BarItemNameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkUserGlyphSize")]
#endif
		public GlyphSize UserGlyphSize {
			get { return (GlyphSize)GetValue(UserGlyphSizeProperty); }
			set { SetValue(UserGlyphSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkUserContent"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object UserContent {
			get { return GetValue(UserContentProperty); }
			set { SetValue(UserContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinkUserGlyphAlignment"),
#endif
 TypeConverter(typeof(NullableDockConverter))]
		public Dock? UserGlyphAlignment {
			get { return (Dock?)GetValue(UserGlyphAlignmentProperty); }
			set { SetValue(UserGlyphAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActualContent")]
#endif
		public object ActualContent {
			get { return GetValue(ActualContentProperty); }
			private set { this.SetValue(ActualContentPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActualContentTemplate")]
#endif
		public DataTemplate ActualContentTemplate {
			get { return (DataTemplate)GetValue(ActualContentTemplateProperty); }
			private set { this.SetValue(ActualContentTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActualDescription")]
#endif
		public string ActualDescription {
			get { return (string)GetValue(ActualDescriptionProperty); }
			private set { this.SetValue(ActualDescriptionPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActualSuperTip")]
#endif
		public SuperTip ActualSuperTip {
			get { return (SuperTip)GetValue(ActualSuperTipProperty); }
			private set { this.SetValue(ActualSuperTipPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActualHint")]
#endif
		public object ActualHint {
			get { return GetValue(ActualHintProperty); }
			private set { this.SetValue(ActualHintPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkKeyGestureText")]
#endif
		public string KeyGestureText {
			get { return (string)GetValue(KeyGestureTextProperty); }
			private set { this.SetValue(KeyGestureTextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkHasHint")]
#endif
		public bool HasHint {
			get { return (bool)GetValue(HasHintProperty); }
			private set { this.SetValue(HasHintPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkHasKeyGesture")]
#endif
		public bool HasKeyGesture {
			get { return (bool)GetValue(HasKeyGestureProperty); }
			private set { this.SetValue(HasKeyGesturePropertyKey, value); }
		}
		protected internal RibbonItemStyles ActualRibbonStyle {
			get { return RibbonStyle == RibbonItemStyles.Default ? (Item != null ? Item.RibbonStyle : RibbonItemStyles.Default) : RibbonStyle; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkRibbonStyle")]
#endif
		public RibbonItemStyles RibbonStyle {
			get { return ribbonStyleInternal; }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public bool? ShowKeyGesture {
			get { return (bool?)GetValue(ShowKeyGestureProperty); }
			set { SetValue(ShowKeyGestureProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkShowScreenTip")]
#endif
		public DevExpress.Utils.DefaultBoolean ShowScreenTip {
			get { return (DevExpress.Utils.DefaultBoolean)GetValue(ShowScreenTipProperty); }
			set { SetValue(ShowScreenTipProperty, value); }
		}						
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkKeyTip")]
#endif
public string KeyTip {
			get { return (string)GetValue(KeyTipProperty); }
			set { SetValue(KeyTipProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkKeyTipDropDown")]
#endif
public string KeyTipDropDown {
			get { return (string)GetValue(KeyTipDropDownProperty); }
			set { SetValue(KeyTipDropDownProperty, value); }
		}
		RibbonItemStyles ribbonStyleInternal = RibbonItemStyles.Default;
		protected virtual void OnRibbonStyleChanged(DependencyPropertyChangedEventArgs e) {			
			ExecuteActionOnLinkControls((lc) => lc.InitializeRibbonStyle());
		}
		protected virtual bool IsLinkInHolder(LinksHolderType holderType) {
			if(Links == null || Links.Holder == null || !(Links.Holder is DependencyObject))
				return false;
			return Links.Holder.HolderType == holderType;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInStatusBar")]
#endif
		public virtual bool IsLinkInStatusBar {
			get {
				return IsLinkInHolder(LinksHolderType.RibbonStatusBarLeft) || IsLinkInHolder(LinksHolderType.RibbonStatusBarRight);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInMenu")]
#endif
		public virtual bool IsLinkInMenu {
			get {
				return IsLinkInHolder(LinksHolderType.PopupMenu);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInDropDownGallery")]
#endif
		public virtual bool IsLinkInDropDownGallery {
			get {
				return IsLinkInHolder(LinksHolderType.RibbonGalleryBarItem);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInBarButtonGroup")]
#endif
		public virtual bool IsLinkInBarButtonGroup {
			get {
				return IsLinkInHolder(LinksHolderType.BarButtonGroup);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInRibbonPageGroup")]
#endif
		public virtual bool IsLinkInRibbonPageGroup {
			get {
				return IsLinkInHolder(LinksHolderType.RibbonPageGroup);
			}
		}
		public virtual bool IsLinkInBar {
			get {
				return IsLinkInHolder(LinksHolderType.Bar);
			}
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInApplicationMenu")]
#endif
		public virtual bool IsLinkInApplicationMenu {
			get {
				return IsLinkInHolder(LinksHolderType.ApplicationMenu);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInRibbonQuickAccessToolbar")]
#endif
		public virtual bool IsLinkInRibbonQuickAccessToolbar {
			get {
				return IsLinkInHolder(LinksHolderType.RibbonQuickAccessToolbar);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInRibbonPageHeader")]
#endif
		public virtual bool IsLinkInRibbonPageHeader {
			get {
				return IsLinkInHolder(LinksHolderType.RibbonPageHeader);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkIsLinkInRibbon")]
#endif
		public virtual bool IsLinkInRibbon {
			get {
				if(Links == null || Links.Holder == null)
					return false;
				switch(Links.Holder.HolderType) {
					case LinksHolderType.RibbonPageGroup:
					case LinksHolderType.BarButtonGroup:
					case LinksHolderType.RibbonQuickAccessToolbar:
					case LinksHolderType.RibbonPageHeader:
					case LinksHolderType.ApplicationMenu:
					case LinksHolderType.RibbonStatusBarLeft:
					case LinksHolderType.RibbonStatusBarRight:
					case LinksHolderType.RibbonGalleryBarItem:
						return true;
				}
				return false;
			}
		}
		protected internal override BarItemAlignment ActualAlignment {
			get {
				if(Alignment != BarItemAlignment.Default)
					return Alignment;
				if(Item == null)
					return BarItemAlignment.Default;
				return Item.Alignment;
			}
		}		
		protected internal override void OnLinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) {
			base.OnLinkControlLoaded(sender, e);
			if (Item != null)
				Item.OnLinkControlLoaded(sender, e);
		}
		protected internal override void UpdateActualIsVisible() {
			ActualIsVisible = GetActualIsVisible();
			foreach(var info in LinkInfos) {
				info.With(x => x.OwnerCollection).Do(x => x.UpdateVisibleInfos());
			}
		}
		protected override bool GetActualIsVisible() {
			bool res = IsVisible;
			if (Item != null) {
				res = res && Item.IsVisible;
				if (Item.Category != null)
					res = res && Item.Category.Visible;
				if (HoldersIsVisible != null)
					res = res && (bool)HoldersIsVisible;
			}
			return res && !IsRemoved;
		}
		protected virtual void UpdateActualContent() {
			ActualContent = UserContent != null ? UserContent : Item.GetContent();
			if(ActualContent is UIElement)
				throw new ArgumentException("UIElement can't be set as content for BarItemLink");
			ExecuteActionOnLinkControls((lc) => lc.OnSourceContentChanged());
		}
		protected virtual internal void UpdateActualContentTemplate() {
			ActualContentTemplate = Item.GetContentTemplate();
		}
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			if(Item == null)
				return;
			UpdateActualContentTemplate();
			ActualDescription = Item.GetDescription();
			ActualSuperTip = Item.GetSuperTip();
			ActualHint = ToolTip ?? Item.GetHint();
			if(Item != null && Item.KeyGesture != null)
				KeyGestureText = Item.KeyGesture.GetDisplayStringForCulture(System.Globalization.CultureInfo.CurrentCulture);
			else 
				KeyGestureText = string.Empty;
			CoerceIsEnabledProperty();			
			HasHint = Item.GetHint() != null;
			HasKeyGesture = KeyGestureText != null && KeyGestureText != "";
			if(!IsPrivate)
				IsPrivate = Item.IsPrivate;			
			ExecuteActionOnLinkControls((lc)=>lc.UpdateActualGlyph());
			UpdateActualContent();
		}
		protected override bool IsEnabledCore {
			get {
				return CalcIsEnabledFromItem();
			}
		}
		protected override void CoerceIsEnabledProperty() {
			base.CoerceIsEnabledProperty();
		}
		protected virtual bool CalcIsEnabledFromItem() {
			if(Item == null)
				return true;
			bool retValue = Item.IsEnabled && Item.CanExecute;
			retValue = retValue && IsLinkEnabled;			
			return retValue;
		}
		protected internal override void OnItemIsEnabledChanged() {
			CoerceIsEnabledProperty();
			ExecuteActionOnLinkControls(lc => lc.UpdateIsEnabled());
		}
		protected internal override void OnItemContentChanged() {
			UpdateActualContent();
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkBarItemDisplayMode")]
#endif
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}
		[Browsable(false)]
		public BarItem Item {
			get { return item.GetValue(); }
			protected internal set { item.SetValue(value); }
		}
		void OnItemPropertyChanging(object sender, ValueChangedEventArgs<BarItem> e) {
			OnItemChanging();
		}
		void OnItemValueChanged(object sender, ValueChangedEventArgs<BarItem> e) {
			LogBase.Add(this, e.NewValue, "itemChanged");
			linkLocker.DoIfNotLocked(() => hasStrongLinkedItem = e.NewValue != null, () => hasStrongLinkedItem = false);
			OnItemChanged(e.OldValue);
			if (Item != null) {
				BarItemName = Item.Name;
				UpdateProperties();
				InitializeFrom(Item);
			}
		}
		const string clonedNamePlaceHolder = "01EA29AFAF78424D8157655DA674EB34";
		string clonedName = clonedNamePlaceHolder;
		protected virtual void OnItemChanged(BarItem oldValue) {
			if (String.IsNullOrEmpty(Name) || Name == clonedName) {
				SetCurrentValue(NameProperty, (clonedName = CloneNameHelper.GetCloneName(Item, this)));				
			}
			ItemsAttachedBehaviorProperties.SetSource(this, Item.With(ItemsAttachedBehaviorProperties.GetSource));
			foreach(BarItemLinkInfo info in LinkInfos.ToArray()) {
				info.OnItemChanged(oldValue);
			}
			if(Item != null && !Item.Links.Contains(this)) {
				Item.Links.AllowModifyCollection = true;
				try {
					Item.Links.Add(this);
				} finally { Item.Links.AllowModifyCollection = false; }				
			}
			UpdateNSName();
			UpdateSeparatorsVisibility();
		}
		protected virtual void OnItemChanging() {
			if(Item != null) {
				Item.Links.AllowModifyCollection = true;
				try {
					Item.Links.Remove(this);					
				}
				finally { Item.Links.AllowModifyCollection = false; }
			}
		}		
		protected virtual void UpdateItemForClonedLinks() {
			WeakReference[] links = ClonedLinks.ToArray();
			foreach(WeakReference link in links) {
				if(link.IsAlive) {
					BarItemLink itemLink = link.Target as BarItemLink;
					if(itemLink != null)
						itemLink.Link(Item);
				} else
					ClonedLinks.Remove(link);
			}
		}
		public event BarItemLinkControlLoadedEventHandler LinkControlLoaded;
		protected internal void RaiseLinkControlLoaded() {
			if(LinkControlLoaded == null) return;
			BarItemLinkControlLoadedEventArgs e = new BarItemLinkControlLoadedEventArgs(Item, this, LinkControl);
			LinkControlLoaded(this, e);
		}
		protected override void OnNameChanged(string newValue, string oldValue) {
			base.OnNameChanged(newValue, oldValue);
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, newValue);
		}
		protected internal void OnBarItemNameChanged(DependencyPropertyChangedEventArgs e) {
			UpdateBarItem();
			UpdateNSName();
		}
		void UpdateNSName() {
			NSName = String.IsNullOrEmpty(BarItemName) ? (hasStrongLinkedItem ? (object)Item : null) : BarItemName;
		}
		protected internal void EnsureBarItemName() {
			if (hasStrongLinkedItem && Item != null)
				BarItemName = Item.Name;
		}
		protected internal override void Clear(bool clearItem) {
			base.Clear(clearItem);
			if (!IsPrivate && (clearItem || CommonBarItemCollectionLink))
				Unlink();
		}		
		protected internal override BarItemLinkControlBase CreateBarItemLinkControl() {
			BarItemLinkControlBase ctrl = base.CreateBarItemLinkControl();
			if(ctrl == null && Item != null)
				return Item.CreateBarItemLinkControl(this);
			return ctrl;
		}
		protected internal override void Initialize() {
			if (Item == null)
				return;
			UpdateProperties();
			InitializeFrom(Item);
			UpdateSeparatorsVisibility();
		}		
		protected internal virtual void OnClick() {
			if(Item != null)
				Item.OnItemClick(this);
		}		
		protected internal virtual void OnDoubleClick() {
			if(Item != null)
				Item.OnItemDoubleClick(this);
		}
		protected virtual void InitializeFrom(BarItem item) {
		}
		protected virtual void UpdateShowGlyph(BarItemLinkInfo linkInfo) {
			if(linkInfo.LinkControl is BarItemLinkControl)
				((BarItemLinkControl)linkInfo.LinkControl).UpdateActualShowGlyph();
		}
		protected internal override void UpdateAlignment() {
			TryMeasurePanel();
		}
		protected virtual void UpdateLinkControlPropertiesInRibbon(BarItemLinkInfo linkInfo) {			
			BarItemLinkControl lc = linkInfo.LinkControl as BarItemLinkControl;
			if(lc == null)
				return;
			lc.UpdateActualGlyph();
			lc.UpdateShowDescription();
		}
		protected virtual void OnBarItemDisplayModeChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls((lc) => lc.OnSourceBarItemDisplayModeChanged());
		}
		protected virtual void OnGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyph());
			UpdateLinkControlsGlyphParams();
		}
		protected virtual void OnUserGlyphAlignmentChanged(Dock? oldValue) {
			ExecuteActionOnLinkControls((lc) => lc.OnSourceGlyphAlignmentChanged());
		}
		protected virtual void OnActualContentPropertyKeyChanged(DependencyPropertyChangedEventArgs e) {
			UpdateEditContentMargin();
		}
		protected virtual void OnToolbarGlyphSizeChanged(GlyphSize oldValue, GlyphSize newValue) { ExecuteActionOnLinkControls(x => x.UpdateActualGlyph()); }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkLinkControl")]
#endif
		public BarItemLinkControl LinkControl { get { return LinkInfos.Count > 0 ? LinkInfos[LinkInfos.Count - 1].LinkControl as BarItemLinkControl : null; } }
		protected virtual void TryMeasurePanel() {
			foreach(BarItemLinkInfo linkInfo in LinkInfos) {
				if(linkInfo.LinkControl == null)
					continue;				
				if(linkInfo.LinkControl.LinksControl != null)
					linkInfo.LinkControl.LinksControl.InvalidateMeasurePanel();
			}
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarItemLink l = link as BarItemLink;
			if(l == null) return;
			Alignment = l.Alignment;
			BarItemDisplayMode = l.BarItemDisplayMode;
			KeyGestureText = l.KeyGestureText;
			UserContent = l.UserContent;
			UserGlyphAlignment = l.UserGlyphAlignment;
			UserGlyphSize = l.UserGlyphSize;
			if(!CommonBarItemCollectionLink)
				Link(l.Item);
			else
				item.SetValue(l.Item, true);
			BarItemName = l.BarItemName;
			RibbonStyle = l.RibbonStyle;
			KeyTip = l.KeyTip;
			KeyTipDropDown = l.KeyTipDropDown;
		}
		readonly Locker linkLocker = new Locker();
		protected internal void Link(BarItem item) {
			linkLocker.DoLockedAction(() => { Item = item; });
		}
		protected internal void Unlink() {
			linkLocker.DoLockedAction(() => { Item = null; });
		}
		protected internal void Unlink(BarItem item) {
			if (Item == item && !hasStrongLinkedItem)
				Unlink();
		}
		protected internal virtual void UpdateLinkControlGlyphParams(BarItemLinkControl barItemLinkControl) {
			if(barItemLinkControl != null) barItemLinkControl.UpdateGlyphParams();
		}
		protected virtual void OnHasKeyGestureChanged(DependencyPropertyChangedEventArgs e) { }
		protected internal void UpdateBarItem() {
			if (Item != null && Item.Name == BarItemName)
				return;
			if (string.IsNullOrEmpty(BarItemName))
				Unlink();
		}
		object nsName;
		object NSName {
			get { return nsName; }
			set {
				if (Equals(nsName, value))
					return;
				var oldValue = nsName;
				nsName = value;
				BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(BarItemLink), oldValue, value);
			}
		}
		#region IMultipleElementRegistratorSupport Members
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(BarItemLink), typeof(IFrameworkInputElement) }.Concat(GetRegistratorKeys()).Distinct(); }
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(registratorKey, typeof(IFrameworkInputElement)))
				return Name;
			if (Equals(registratorKey, typeof(BarItemLink)))
				return NSName;
			return GetRegistratorName(registratorKey);
		}
		protected virtual object GetRegistratorName(object registratorKey) { throw new ArgumentException(); }
		protected virtual IEnumerable<object> GetRegistratorKeys() { return Enumerable.Empty<object>(); }
		protected internal void LockItem() {
			item.Lock();
		}
		protected internal void UnlockItem() {
			item.Unlock();
		}
		#endregion
	}
}
