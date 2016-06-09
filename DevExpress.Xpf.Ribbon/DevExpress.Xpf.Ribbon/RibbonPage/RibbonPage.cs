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
using System.Windows.Media;
using DevExpress.Xpf.Ribbon;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using System.Collections.ObjectModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Bars.Customization;
namespace DevExpress.Xpf.Ribbon {
	[ContentProperty("Groups")]
	public class RibbonPage : FrameworkContentElement, ICloneable, IMultipleElementRegistratorSupport, IBarManagerControllerAction {
		#region static
		public static readonly DependencyProperty ActualIsVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsVisiblePropertyKey;
		public static readonly DependencyProperty ActualColorProperty;
		protected static readonly DependencyPropertyKey ActualColorPropertyKey;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty CaptionTemplateProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty PageCategoryProperty;
		protected internal static readonly DependencyPropertyKey PageCategoryPropertyKey;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty KeyTipProperty;
		public static readonly DependencyProperty CaptionMinWidthProperty;
		public static readonly DependencyProperty GroupsProperty;
		public static readonly DependencyProperty GroupsSourceProperty;
		protected static readonly DependencyPropertyKey GroupsPropertyKey;
		public static readonly DependencyProperty GroupTemplateProperty;
		public static readonly DependencyProperty GroupTemplateSelectorProperty;
		public static readonly DependencyProperty GroupStyleProperty;
		public static readonly DependencyProperty AllowRemoveFromParentDuringCustomizationProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty GroupsAttachedBehaviorProperty;
		public static readonly DependencyProperty MergeOrderProperty;
		public static readonly DependencyProperty MergeTypeProperty;
		public static readonly DependencyProperty IndexProperty;	  
		public static readonly DependencyProperty IsRemovedProperty;
		static RibbonPage() {
			Type ribbonPageType = typeof(RibbonPage);
			IsRemovedProperty = DependencyPropertyManager.Register("IsRemoved", typeof(bool), typeof(RibbonPage), new FrameworkPropertyMetadata(false, (d, e) => ((RibbonPage)d).OnIsRemovedChanged()));
			AllowRemoveFromParentDuringCustomizationProperty = DependencyPropertyManager.Register("AllowRemoveFromParentDuringCustomization", typeof(bool), ribbonPageType, new FrameworkPropertyMetadata(true));
			ActualIsVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsVisible", typeof(bool), ribbonPageType, new FrameworkPropertyMetadata(true, (d, e) => ((RibbonPage)d).OnActualIsVisibleChanged((bool)e.OldValue, (bool)e.NewValue)));
			ActualIsVisibleProperty = ActualIsVisiblePropertyKey.DependencyProperty;
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RibbonPage), new FrameworkPropertyMetadata(0));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(object), ribbonPageType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCaptionPropertyChanged)));
			CaptionTemplateProperty = DependencyPropertyManager.Register("CaptionTemplate", typeof(DataTemplate), ribbonPageType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCaptionTemplatePropertyChanged)));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), ribbonPageType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsSelectedPropertyChanged), new CoerceValueCallback(OnIsSelectedPropertyCoerce)));
			PageCategoryPropertyKey = DependencyPropertyManager.RegisterReadOnly("PageCategory", typeof(RibbonPageCategoryBase), ribbonPageType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPageCategoryPropertyChanged)));
			PageCategoryProperty = PageCategoryPropertyKey.DependencyProperty;
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), ribbonPageType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnIsVisiblePropertyChanged)));
			KeyTipProperty = DependencyPropertyManager.Register("KeyTip", typeof(string), ribbonPageType, new UIPropertyMetadata(string.Empty));
			CaptionMinWidthProperty = DependencyPropertyManager.Register("CaptionMinWidth", typeof(double), ribbonPageType, new PropertyMetadata(0d));
			GroupsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Groups", typeof(RibbonPageGroupCollection), ribbonPageType, new PropertyMetadata(null));
			GroupsProperty = GroupsPropertyKey.DependencyProperty;
			GroupsSourceProperty = DependencyProperty.Register("GroupsSource", typeof(object), ribbonPageType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupsSourcePropertyChanged)));
			GroupTemplateProperty = DependencyProperty.Register("GroupTemplate", typeof(DataTemplate), ribbonPageType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupsTemplatePropertyChanged)));
			GroupTemplateSelectorProperty = DependencyProperty.Register("GroupTemplateSelector", typeof(DataTemplateSelector), ribbonPageType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupsTemplatePropertyChanged)));
			GroupStyleProperty = DependencyProperty.Register("GroupStyle", typeof(Style), ribbonPageType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupsTemplatePropertyChanged)));
			GroupsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("GroupsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonPage, RibbonPageGroup>), ribbonPageType, new UIPropertyMetadata(null));
			ActualColorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualColor", typeof(Color), ribbonPageType, new FrameworkPropertyMetadata(Colors.Transparent));
			ActualColorProperty = ActualColorPropertyKey.DependencyProperty;
			MergeOrderProperty = DependencyPropertyManager.Register("MergeOrder", typeof(int), ribbonPageType, new FrameworkPropertyMetadata(-1));
			MergeTypeProperty = DependencyPropertyManager.Register("MergeType", typeof(RibbonMergeType), ribbonPageType, new FrameworkPropertyMetadata(RibbonMergeType.MergeItems));
			NameProperty.OverrideMetadata(ribbonPageType, new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) => ((RibbonPage)d).OnNameChanged(e.OldValue as string))));		 
		}
		protected virtual void OnIsRemovedChanged() {
			this.UpdateActualIsVisible();
			this.Ribbon.SelectFirstVisiblePage();
		}
		protected virtual void OnNameChanged(string oldValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, Name);			
		}				
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		protected static void OnIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnIsVisibleChanged((bool)e.OldValue);
		}
		protected static void OnPageCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnPageCategoryChanged(e.OldValue as RibbonPageCategoryBase);			
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnIsSelectedChanged((bool)e.OldValue);
		}
		protected static object OnIsSelectedPropertyCoerce(DependencyObject d, object value) {
			return ((bool)value) & ((RibbonPage)d).IsSelectable;
		}
		protected static void OnCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnCaptionChanged(e);
		}
		protected static void OnCaptionTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnCaptionTemplateChanged(e);
		}
		protected static void OnGroupsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnGroupsSourceChanged(e);
		}
		protected static void OnGroupsTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonPage)d).OnGroupsTemplateChanged(e);
		}		
		#endregion
		#region static prop defs
		public bool ActualIsVisible {
			get { return (bool)GetValue(ActualIsVisibleProperty); }
			protected set { this.SetValue(ActualIsVisiblePropertyKey, value); }
		}
		public double CaptionMinWidth {
			get { return (double)GetValue(CaptionMinWidthProperty); }
			set { SetValue(CaptionMinWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageAllowRemoveFromParentDuringCustomization")]
#endif
		public bool AllowRemoveFromParentDuringCustomization {
			get { return (bool)GetValue(AllowRemoveFromParentDuringCustomizationProperty); }
			set { SetValue(AllowRemoveFromParentDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageKeyTip")]
#endif
		public string KeyTip {
			get { return (string)GetValue(KeyTipProperty); }
			set { SetValue(KeyTipProperty, value); }
		}
		[TypeConverter(typeof(ObjectConverter)), 
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonPageCaption")
#else
	Description("")
#endif
]
		public object Caption {
			get { return GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public DataTemplate CaptionTemplate {
			get { return (DataTemplate)GetValue(CaptionTemplateProperty); }
			set { SetValue(CaptionTemplateProperty, value); }
		}
		public RibbonPageCategoryBase PageCategory {
			get { return (RibbonPageCategoryBase)GetValue(PageCategoryProperty); }
			protected internal set { this.SetValue(PageCategoryPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageIsSelected")]
#endif
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public bool IsRemoved {
			get { return (bool)GetValue(IsRemovedProperty); }
			set { SetValue(IsRemovedProperty, value); }
		}		
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageGroups")]
#endif
		public RibbonPageGroupCollection Groups {
			get { return (RibbonPageGroupCollection)GetValue(GroupsProperty); }			
		}
		public object GroupsSource {
			get { return GetValue(GroupsSourceProperty); }
			set { SetValue(GroupsSourceProperty, value); }
		}
		public DataTemplate GroupTemplate {
			get { return (DataTemplate)GetValue(GroupTemplateProperty); }
			set { SetValue(GroupTemplateProperty, value); }
		}
		public DataTemplateSelector GroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupTemplateSelectorProperty); }
			set { SetValue(GroupTemplateSelectorProperty, value); }
		}
		public Style GroupStyle {
			get { return (Style)GetValue(GroupStyleProperty); }
			set { SetValue(GroupStyleProperty, value); }
		}
		public Color ActualColor {
			get { return (Color)GetValue(ActualColorProperty); }
			protected set { this.SetValue(ActualColorPropertyKey, value); }
		}
		#endregion
		readonly MultiDictionary<RibbonPage, RibbonPageGroup> mergedGroups;
		public RibbonPage() {
			ActualGroupsCore = new ObservableCollection<RibbonPageGroup>();
			ActualGroups = new ReadOnlyObservableCollection<RibbonPageGroup>(ActualGroupsCore);
			if(this.IsInDesignTool())
				SetCurrentValue(CaptionProperty, "Ribbon Page");
			this.SetValue(GroupsPropertyKey, CreatePageGroupCollection());
			InitBindings();
			DXSerializer.SetSerializationIDDefault(this, "RibbonPage");
			mergedGroups = new MultiDictionary<RibbonPage, RibbonPageGroup>();
		}
		protected internal event EventHandler IsSelectedChanged;
		protected internal event EventHandler IsVisibleChangedWhenSelected;
		protected internal event EventHandler IsVisibleChanged;
		protected virtual RibbonPageGroupCollection CreatePageGroupCollection() {
			return new RibbonPageGroupCollection(this);
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}		  
		public int MergeOrder {
			get { return (int)GetValue(MergeOrderProperty); }
			set { SetValue(MergeOrderProperty, value); }
		}
		public RibbonMergeType MergeType {
			get { return (RibbonMergeType)GetValue(MergeTypeProperty); }
			set { SetValue(MergeTypeProperty, value); }
		}
		public bool IsSelectable { get { return GetIsSelectable(); } }
		public RibbonControl Ribbon { get { return PageCategory == null ? null : PageCategory.Ribbon; } }
		protected internal ObservableCollection<RibbonPageGroup> ActualGroupsCore { get; set; }
		public ReadOnlyObservableCollection<RibbonPageGroup> ActualGroups { get; protected set; }
		internal List<RibbonPageHeaderControl> PageHeaderControls = new List<RibbonPageHeaderControl>();
		internal List<RibbonPageGroupsControl> PageGroupsControls = new List<RibbonPageGroupsControl>();
		protected delegate void PageHeaderControlAction(RibbonPageHeaderControl headerControl);		
		protected void ExecutePageHeaderControlAction(PageHeaderControlAction action) {
			foreach(RibbonPageHeaderControl headerControl in PageHeaderControls) {
				action(headerControl);
			}
		}
		protected virtual bool GetIsSelectable() {
			return ActualIsVisible;
		}		
		protected virtual void OnCaptionTemplateChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnCaptionChanged(DependencyPropertyChangedEventArgs e) {
		}
		bool suppressIsSelectedLogic = false;
		protected internal void SetIsSelectedSealed(bool value) {
			suppressIsSelectedLogic = true;
			SetCurrentValue(IsSelectedProperty, value);
			suppressIsSelectedLogic = false;
		}
		protected virtual void OnActualIsVisibleChanged(bool oldValue, bool newValue) {
			ExecutePageHeaderControlAction(new PageHeaderControlAction(ph => {
				if (ph.Ribbon != null && ph.Ribbon.CategoriesPane != null && ph.Ribbon.CategoriesPane.RibbonItemsPanel!=null)
					ph.Ribbon.CategoriesPane.RibbonItemsPanel.InvalidateMeasure();
			}));
		}
		protected virtual void OnIsVisibleChanged(bool oldValue) {
			UpdateActualIsVisible();
			bool oldIsSelected = IsSelected;
			if(!IsVisible)
				this.SetCurrentValue(IsSelectedProperty, false);
			if(IsVisibleChangedWhenSelected != null && oldIsSelected)
				IsVisibleChangedWhenSelected(this, new EventArgs());			
			if(!IsVisible && IsSelected) {
				Ribbon.SelectFirstVisiblePage();
			}
			if(!oldValue && IsVisible && Ribbon != null && Ribbon.SelectedPage == null)
				Ribbon.SelectFirstVisiblePage(true);
			RaiseIsVisibleChanged();
		}
		protected internal void UpdateActualIsVisible() {
			if (IsRemoved) {
				ActualIsVisible = false;
				return;
			} 
			if (PageCategory != null)
				ActualIsVisible = PageCategory.IsVisible && IsVisible;
			else
				ActualIsVisible = IsVisible;
			foreach(RibbonPageGroup group in Groups) {
				group.UpdateActualIsVisible();
			}
			if (!ActualIsVisible && IsSelected)
				if (Ribbon != null && Ribbon.MergedParent != null)
					Ribbon.MergedParent.SelectFirstVisiblePage();
		}
		protected virtual void RaiseIsVisibleChanged() {
			if(IsVisibleChanged != null) {
				IsVisibleChanged(this, new EventArgs());
			}
		}
		internal bool IsSelectedChangedProcessing { get; set; }
		protected virtual void OnIsSelectedChanged(bool oldValue) {
			if(suppressIsSelectedLogic || Ribbon==null)
				return;
			IsSelectedChangedProcessing = true;
			if(IsSelectedChanged != null)
				IsSelectedChanged(this, new EventArgs());
			if(MergedParentCategory != null)
				MergedParentCategory.OnPageIsSelectedCoreChanged(this);
			else if(MergedParent != null) {
				MergedParent.SetCurrentValue(RibbonPage.IsSelectedProperty, IsSelected);
			} else if(PageCategory != null && PageCategory.MergedParentRibbon != null)
				PageCategory.MergedParentRibbon.OnPageIsSelectedChanged(this, new EventArgs());
			IsSelectedChangedProcessing = false;
		}
		protected virtual void OnPageCategoryChanged(RibbonPageCategoryBase oldValue) {
			UpdateActualColor();
			if(oldValue != null) {
				if(oldValue.Pages.Contains(this)) {
					throw new InvalidOperationException("Set a new PageCategory property for the page failed, because this page was not removed from Pages collection of another PageCategory.");
				}
			}
			UpdateActualIsVisible();
		}
		internal RibbonPageGroup GetGroupByCaption(string caption) {
			foreach(RibbonPageGroup pageGroup in ActualGroupsCore) {
				if(pageGroup.Caption == caption)
					return pageGroup;
			}
			return null;
		}
		protected virtual void OnGroupsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<RibbonPage, RibbonPageGroup>.OnItemsSourcePropertyChanged(
					this,
					e,
					GroupsAttachedBehaviorProperty,
					GroupTemplateProperty,
					GroupTemplateSelectorProperty,
					GroupStyleProperty,
					page => page.Groups,
					page => new RibbonPageGroup(), useDefaultTemplateSelector: true);
				if(Ribbon != null)
					Ribbon.AddOrRemoveObjectWithItemsSourcePropertyUsed(this, GroupsSource != null);
			} else {
				if(e.NewValue != null) {
					PopulateItemsHelper.GenerateItems(e, () => {
						RibbonPageGroup group = Groups.FindOrCreateNew(x => string.IsNullOrEmpty(x.Caption), () => new RibbonPageGroup());
						return new SingleGroupGenerator(new BarItemsGenerator(group, ImageType.Colored)); 
					});
				}
			}
		}
		private void OnGroupsTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonPage, RibbonPageGroup>.OnItemsGeneratorTemplatePropertyChanged(
				this,
				e,
				GroupsAttachedBehaviorProperty);
		}
		protected internal virtual void UpdateActualColor() {
			if(MergedParentCategory != null)
				ActualColor = MergedParentCategory.Color;
			else if(PageCategory != null)
				ActualColor = PageCategory.Color;
		}
		void InitBindings() {
		}
		protected internal virtual void OnPageGroupInserted(RibbonPageGroup group, int index) {
			if (group.Page == this && group.Parent==null)
				AddLogicalChild(group);
			if(PageCategory != null)
				PageCategory.OnPageGroupInserted(group, index);
			ActualGroupsCore.Insert(index, group);
		}
		protected internal virtual void OnPageGroupRemoved(RibbonPageGroup group, int index) {
			RemoveLogicalChild(group);
			if(PageCategory != null)
				PageCategory.OnPageGroupRemoved(group, index);
			ActualGroupsCore.Remove(group);
		}
		protected override IEnumerator LogicalChildren {
			get { return Groups.Where(x=>x.Parent==this).ToList().GetEnumerator(); }
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			RibbonPage res = new RibbonPage();
			res.AssignPropertiesFromSource(this);
			res.AssignGroupsFromSource(this);
			return res;
		}
		#endregion
		protected virtual void AssignPropertiesFromSource(RibbonPage source) {
			CaptionTemplate = source.CaptionTemplate;
			Caption = source.Caption;
			CaptionMinWidth = source.CaptionMinWidth;
			MergeType = source.MergeType;
			MergeOrder = source.MergeOrder;
		}
		protected virtual void AssignGroupsFromSource(RibbonPage source) {
			foreach(RibbonPageGroup group in source.Groups) {
				Groups.Add((RibbonPageGroup)((ICloneable)group).Clone());
			}
		}
		#region merging
		internal int ActualMergeOrder {
			get {
				return MergedChildren.Count == 0 ? MergeOrder : MergedChildren[MergedChildren.Count - 1].MergeOrder;
			}
		}
		internal RibbonPage MergedParent { get; set; }
		internal RibbonPage ReplacedPage { get; set; }
		List<RibbonPage> MergedChildren = new List<RibbonPage>();
		private RibbonPageCategoryBase mergedParentCategoryCore = null;
		internal RibbonPageCategoryBase MergedParentCategory {
			get { return mergedParentCategoryCore; }
			set {
				if(mergedParentCategoryCore == value) return;
				RibbonPageCategoryBase oldValue = mergedParentCategoryCore;
				mergedParentCategoryCore = value;
				OnMergedParentCategoryChanged(oldValue);
			}
		}
		protected virtual void OnMergedParentCategoryChanged(RibbonPageCategoryBase oldValue) {
			UpdateActualColor();
		}
		internal void Merge(RibbonPage childPage) {
			if(MergedChildren.Contains(childPage))
				return;
			MergedChildren.Add(childPage);
			foreach(RibbonPageGroup group in childPage.Groups) {
				mergedGroups.Add(childPage, group);
				if(group.MergeType == RibbonMergeType.Remove)
					continue;
				RibbonPageGroup sameCaptionGroup = GetGroupByCaption(group.Caption);
				if(group.MergeType == RibbonMergeType.Add || sameCaptionGroup == null) {
					ActualGroupsCore.Add(group);
				} else if(group.MergeType == RibbonMergeType.Replace) {
					ActualGroupsCore[ActualGroupsCore.IndexOf(sameCaptionGroup)] = group;
					group.ReplacedPageGroup = sameCaptionGroup;
				} else {
					sameCaptionGroup.Merge(group);
					group.MergedParent = sameCaptionGroup;
				}
			}
			childPage.MergedParent = this;
		}
		internal void UnMerge(RibbonPage childPage) {
			if(!MergedChildren.Contains(childPage))
				return;
			MergedChildren.Remove(childPage);
			var groups = mergedGroups[childPage];
			foreach(RibbonPageGroup group in groups) {
				if(group.MergedParent != null)
					(group.MergedParent).UnMerge(group);
				if(group.ReplacedPageGroup != null) {
					ActualGroupsCore[ActualGroupsCore.IndexOf(group)] = group.ReplacedPageGroup;
					group.ReplacedPageGroup = null;
				} 
				ActualGroupsCore.Remove(group);
			}
			mergedGroups.Remove(childPage);
			childPage.MergedParent = null;
		}
		#endregion
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { yield return typeof(IFrameworkInputElement); }
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			return Name;
		}
		IActionContainer IControllerAction.Container { get; set; }
		protected internal bool CreatedByCustomizationDialog { get; set; }
		object IBarManagerControllerAction.GetObject() { return this; }
		void IControllerAction.Execute(DependencyObject context) { CollectionActionHelper.Execute(this); }
	}
}
