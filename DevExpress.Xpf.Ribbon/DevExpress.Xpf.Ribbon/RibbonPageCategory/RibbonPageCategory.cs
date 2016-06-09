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
using System.Windows.Markup;
using DevExpress.Xpf.Utils;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Ribbon.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCategoryBase : FrameworkContentElement, ICloneable, IMultipleElementRegistratorSupport, IBarManagerControllerAction {
		#region static
		public static readonly DependencyProperty CaptionProperty;
		protected internal static readonly DependencyPropertyKey IsDefaultPropertyKey;
		public static readonly DependencyProperty IsDefaultProperty;
		public static readonly DependencyProperty PagesSourceProperty;
		public static readonly DependencyProperty RibbonProperty;
		protected internal static readonly DependencyPropertyKey RibbonPropertyKey;
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		protected internal static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty PagesProperty;
		protected static readonly DependencyPropertyKey PagesPropertyKey;
		public static readonly DependencyProperty PageTemplateProperty;
		public static readonly DependencyProperty PageTemplateSelectorProperty;
		public static readonly DependencyProperty PageStyleProperty;
		public static readonly DependencyProperty IndexProperty;		
		public static readonly DependencyProperty AllowRemoveFromParentDuringCustomizationProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PagesAttachedBehaviorProperty;
		public static readonly DependencyProperty MergeOrderProperty;
		public static readonly DependencyProperty MergeTypeProperty;
		static RibbonPageCategoryBase() {
			Type pageCategoryBaseType = typeof(RibbonPageCategoryBase);
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RibbonPageCategoryBase), new FrameworkPropertyMetadata(0));
			RibbonPropertyKey = DependencyPropertyManager.RegisterReadOnly("Ribbon", typeof(RibbonControl), pageCategoryBaseType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRibbonPropertyChanged)));
			RibbonProperty = RibbonPropertyKey.DependencyProperty;
			AllowRemoveFromParentDuringCustomizationProperty = DependencyPropertyManager.Register("AllowRemoveFromParentDuringCustomization", typeof(bool), pageCategoryBaseType, new FrameworkPropertyMetadata(true));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), pageCategoryBaseType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnCaptionPropertyChanged)));
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), pageCategoryBaseType, new FrameworkPropertyMetadata(Color.FromArgb(0xFF, 0x9e, 0xc5, 0x7e), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnColorPropertyChanged)));
			IsDefaultPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsDefault", typeof(bool), pageCategoryBaseType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnIsDefaultPropertyChanged), new CoerceValueCallback(OnIsDefaultPropertyCoerce)));
			IsDefaultProperty = IsDefaultPropertyKey.DependencyProperty;
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), pageCategoryBaseType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnIsVisiblePropertyChanged), new CoerceValueCallback(OnIsVisiblePropertyCoerce)));
			IsSelectedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSelected", typeof(bool), pageCategoryBaseType,
				new PropertyMetadata(false, OnIsSelectedPropertyChanged));
			IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
			MergeOrderProperty = DependencyPropertyManager.Register("MergeOrder", typeof(int), pageCategoryBaseType, new PropertyMetadata(-1));
			MergeTypeProperty = DependencyPropertyManager.Register("MergeType", typeof(RibbonMergeType), pageCategoryBaseType, new PropertyMetadata(RibbonMergeType.MergeItems));
			PagesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Pages", typeof(RibbonPageCollection), pageCategoryBaseType, new PropertyMetadata(null));
			PagesProperty = PagesPropertyKey.DependencyProperty;
			PagesSourceProperty = DependencyProperty.Register("PagesSource", typeof(object), pageCategoryBaseType, new PropertyMetadata(null, new PropertyChangedCallback(OnPagesSourcePropertyChanged)));
			PageTemplateProperty = DependencyProperty.Register("PageTemplate", typeof(DataTemplate), pageCategoryBaseType, new PropertyMetadata(null, new PropertyChangedCallback(OnPagesTemplatePropertyChanged)));
			PageTemplateSelectorProperty = DependencyProperty.Register("PageTemplateSelector", typeof(DataTemplateSelector), pageCategoryBaseType, new PropertyMetadata(null, new PropertyChangedCallback(OnPagesTemplatePropertyChanged)));
			PageStyleProperty = DependencyProperty.Register("PageStyle", typeof(Style), pageCategoryBaseType, new PropertyMetadata(null, new PropertyChangedCallback(OnPagesTemplatePropertyChanged)));
			PagesAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("PagesAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonPageCategoryBase, RibbonPage>), pageCategoryBaseType, new UIPropertyMetadata(null));
			NameProperty.OverrideMetadata(pageCategoryBaseType, new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) => ((RibbonPageCategoryBase)d).OnNameChanged(e.OldValue as string))));			
		}						
		protected virtual void OnNameChanged(string oldValue) {
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, Name);
		}				
		protected static void OnPagesSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnPagesSourceChanged(e);
		}
		protected static void OnPagesTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnPagesTemplateChanged(e);
		}		
		protected static void OnIsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnIsVisibleChanged((bool)e.OldValue);
		}
		protected static object OnIsVisiblePropertyCoerce(DependencyObject d, object value) {
			bool newValue = (bool)value;
			((RibbonPageCategoryBase)d).IsVisibleChanging(ref newValue);
			return newValue;
		}
		protected static void OnRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnRibbonChanged(e.OldValue as RibbonControl);
		}
		protected static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnColorChanged((Color)e.OldValue);
		}
		protected static void OnCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnCaptionChanged((string)e.OldValue);
		}
		protected static void OnIsDefaultPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnIsDefaultChanged((bool)e.OldValue);
		}
		protected static object OnIsDefaultPropertyCoerce(DependencyObject d, object value) {
			bool newValue = (bool)value;
			((RibbonPageCategoryBase)d).IsDefaultChanging(ref newValue);
			return newValue;
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryBase)d).OnIsSelectedChanged((bool)e.OldValue);
		}
		#endregion
		protected internal event EventHandler PageIsSelectedChanged;
		protected internal event RibbonPageInsertedEventHandler PageInserted;
		protected internal event RibbonPageRemovedEventHandler PageRemoved;
		protected internal event RibbonPageGroupInsertedEventHandler PageGroupInserted;
		protected internal event RibbonPageGroupRemovedEventHandler PageGroupRemoved;
		readonly MultiDictionary<RibbonPageCategoryBase, RibbonPage> mergedPages;				
		public RibbonPageCategoryBase() {
			ActualPagesCore = new ObservableCollection<RibbonPage>();
			ActualPages = new ReadOnlyObservableCollection<RibbonPage>(ActualPagesCore);
			RaisePageIsSelectedChanged = true;
			Pages = CreatePagesCollection();
			InitBindings();
			DXSerializer.SetSerializationIDDefault(this, "RibbonPageCategoryBase");
			mergedPages = new MultiDictionary<RibbonPageCategoryBase, RibbonPage>();
		}
		protected virtual void InitBindings() {
		}
		protected virtual RibbonPageCollection CreatePagesCollection() {
			return new RibbonPageCollection(this);
		}
		#region dep props
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			protected internal set { SetValue(RibbonPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageCategoryBaseColor")]
#endif
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageCategoryBaseCaption")]
#endif
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageCategoryBaseIsDefault")]
#endif
		public bool IsDefault {
			get { return (bool)GetValue(IsDefaultProperty); }
			protected internal set { SetValue(IsDefaultPropertyKey, value); }
		}		
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			protected internal set { SetValue(IsSelectedPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageCategoryBaseAllowRemoveFromParentDuringCustomization")]
#endif
		public bool AllowRemoveFromParentDuringCustomization {
			get { return (bool)GetValue(AllowRemoveFromParentDuringCustomizationProperty); }
			set { SetValue(AllowRemoveFromParentDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonPageCategoryBasePages")]
#endif
		public RibbonPageCollection Pages {
			get { return (RibbonPageCollection)GetValue(PagesProperty); }
			protected set { SetValue(PagesPropertyKey, value); }
		}
		public object PagesSource {
			get { return GetValue(PagesSourceProperty); }
			set { SetValue(PagesSourceProperty, value); }
		}
		public DataTemplate PageTemplate {
			get { return (DataTemplate)GetValue(PageTemplateProperty); }
			set { SetValue(PageTemplateProperty, value); }
		}
		public DataTemplateSelector PageTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PageTemplateSelectorProperty); }
			set { SetValue(PageTemplateSelectorProperty, value); }
		}
		public Style PageStyle {
			get { return (Style)GetValue(PageStyleProperty); }
			set { SetValue(PageStyleProperty, value); }
		}
		#endregion              
		protected internal ObservableCollection<RibbonPage> ActualPagesCore { get; protected set; }
		public ReadOnlyObservableCollection<RibbonPage> ActualPages { get; protected set; }
		public int MergeOrder {
			get { return (int)GetValue(MergeOrderProperty); }
			set { SetValue(MergeOrderProperty, value); }
		}
		public RibbonMergeType MergeType {
			get { return (RibbonMergeType)GetValue(MergeTypeProperty); }
			set { SetValue(MergeTypeProperty, value); }
		}		
		protected internal RibbonPage GetLastSelectedPage() {
			RibbonPage lastSelectedPage = null;
			foreach (RibbonPage page in Pages) {
				if (page.IsSelected)
					lastSelectedPage = page;
			}
			return lastSelectedPage;
		}
		protected virtual void OnColorChanged(Color oldValue) {
			foreach (RibbonPage page in ActualPagesCore)
				page.UpdateActualColor();
		}
		protected virtual void OnCaptionChanged(string oldValue) {
		}
		protected virtual void OnIsDefaultChanged(bool oldValue) {
		}
		internal List<RibbonPageCategoryHeaderControl> CategoryHeaderControls = new List<RibbonPageCategoryHeaderControl>();
		internal List<RibbonPageCategoryControl> CategoryControls = new List<RibbonPageCategoryControl>();
		private void OnPagesTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonPageCategoryBase, RibbonPage>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				PagesAttachedBehaviorProperty);
		}
		protected virtual void OnPagesSourceChanged(DependencyPropertyChangedEventArgs e) {
			if (e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<RibbonPageCategoryBase, RibbonPage>.OnItemsSourcePropertyChanged(this,
					e,
					PagesAttachedBehaviorProperty,
					PageTemplateProperty,
					PageTemplateSelectorProperty,
					PageStyleProperty,
					category => category.Pages,
					category => new RibbonPage(), useDefaultTemplateSelector: true);
				if (Ribbon != null)
					Ribbon.AddOrRemoveObjectWithItemsSourcePropertyUsed(this, PagesSource != null);
			} else {
				PopulateItemsHelper.GenerateItems(e, () => new PagesGenerator(this));
			}
		}			 
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			if (oldValue != null) {
				oldValue.IsEnabledChanged -= OnRibbonIsEnabledChanged;
			}
			if (Ribbon != null) {
				Ribbon.IsEnabledChanged += OnRibbonIsEnabledChanged;
			}
			if (Ribbon == null)
				return;
			foreach (RibbonPage page in Pages) {
				if (page.IsSelected) {
					Ribbon.SelectedPage = page;
					break;
				}
			}	   
		}		
		void OnRibbonIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CoerceValue(IsEnabledProperty);
		}
		protected delegate void CategoryHeaderControlAction(RibbonPageCategoryHeaderControl control);
		protected delegate void CategoryControlAction(RibbonPageCategoryControl control);
		protected void ExecuteActionOnCategoryHeaderControls(CategoryHeaderControlAction action) {
			foreach (RibbonPageCategoryHeaderControl ctrl in CategoryHeaderControls) {
				action(ctrl);
			}
		}
		protected void ExecuteActionOnCategoryControls(CategoryControlAction action) {
			foreach (RibbonPageCategoryControl ctrl in CategoryControls) {
				action(ctrl);
			}
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {
		}
		public RibbonPage GetFirstSelectablePage() {
			return ActualPages.OrderBy(page => page.ActualMergeOrder).FirstOrDefault(page => page.IsSelectable);
		}
		protected virtual void OnIsVisibleChanged(bool oldValue) {
			foreach (RibbonPage page in Pages) {
				page.UpdateActualIsVisible();
			}
			if (IsVisible == false) {
				foreach (RibbonPage page in ActualPages) {
					page.SetCurrentValue(RibbonPage.IsSelectedProperty, false);
				}
				if (Ribbon != null && Ribbon.SelectedPage == null) {
					RibbonPage page = Ribbon.GetFirstSelectablePage();
					if (page != null)
						page.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
				}
			}
		}
		protected virtual void IsVisibleChanging(ref bool newValue) {
		}
		protected virtual void IsDefaultChanging(ref bool newValue) {
		}		
		protected internal void OnPageRemoved(RibbonPage page, int oldIndex) {
			RemoveLogicalChild(page);
			if (PageRemoved != null)
				PageRemoved(page, new RibbonPageRemovedEventArgs(page, oldIndex));
			if (page.IsSelected) {
				page.SetCurrentValue(RibbonPage.IsSelectedProperty, false);
				RibbonPage nextPage = RibbonSelectionHelper.SwitchPagesSelection(page, oldIndex);
				if (nextPage != null) nextPage.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
			}
			ActualPagesCore.Remove(page);
		}
		protected internal void OnPageInserted(RibbonPage page, int newIndex) {
			if (page.Parent == null)
				AddLogicalChild(page);
			ActualPagesCore.Insert(newIndex, page);
			if (PageInserted != null)
				PageInserted(page, new RibbonPageInsertedEventArgs(page, newIndex));
			if (page.IsSelected) {
				OnPageIsSelectedCoreChanged(page);
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		protected internal void OnPageGroupInserted(RibbonPageGroup group, int index) {
			if (PageGroupInserted != null)
				PageGroupInserted(group, new RibbonPageGroupInsertedEventArgs(group, index));
		}
		protected internal void OnPageGroupRemoved(RibbonPageGroup group, int index) {
			if (PageGroupRemoved != null)
				PageGroupRemoved(group, new RibbonPageGroupRemovedEventArgs(group, index));
		}
		protected internal void OnSelectedPageIsVisibleCoreChanged(RibbonPage page) {
			if (page.IsSelected == false && page.IsVisible == false) {
				RibbonPage nextPage = RibbonSelectionHelper.SwitchPagesSelection(page, Pages.IndexOf(page));
				if (nextPage != null) nextPage.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
			}
		}
		protected internal bool RaisePageIsSelectedChanged { get; set; }
		protected internal void OnPageIsSelectedCoreChanged(RibbonPage page) {
			if (page.IsSelected && Ribbon == null) {
				foreach (RibbonPage pg in Pages) {
					pg.SetCurrentValue(RibbonPage.IsSelectedProperty, pg == page);
				}
			}
			if (PageIsSelectedChanged != null && RaisePageIsSelectedChanged)
				PageIsSelectedChanged(page, new EventArgs());
		}
		protected override IEnumerator LogicalChildren {
			get {
				return Pages.Where(x=>x.Parent==this).ToList().GetEnumerator();
			}
		}
		protected virtual RibbonPageCategoryBase CreateEmptyClone() {
			return new RibbonPageCategoryBase();
		}
		protected virtual void AssignPropertiesFromSource(RibbonPageCategoryBase sourceCategory) {
			Caption = sourceCategory.Caption;
			Color = sourceCategory.Color;
			IsDefault = sourceCategory.IsDefault;
			MergeType = !sourceCategory.IsDefault || sourceCategory.MergeType != RibbonMergeType.Add ? sourceCategory.MergeType : RibbonMergeType.MergeItems;
			MergeOrder = sourceCategory.MergeOrder;
		}
		protected virtual void ClonePagesFromSource(RibbonPageCategoryBase sourceCategory) {
			foreach (RibbonPage page in sourceCategory.Pages) {
				Pages.Add((RibbonPage)((ICloneable)page).Clone());
			}
		}
		internal RibbonPage GetPageByCaption(string caption) {
			foreach (RibbonPage page in ActualPagesCore) {
				if((page.Caption as string) == caption)
					return page;
			}
			return null;
		}
		protected internal RibbonPage GetSelectedPage() {
			foreach (RibbonPage page in ActualPagesCore) {
				if (page.IsSelected)
					return page;
			}
			return null;
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			RibbonPageCategoryBase res = CreateEmptyClone();
			res.AssignPropertiesFromSource(this);
			res.ClonePagesFromSource(this);
			return res;
		}
		#endregion
		#region Merging
		internal int ActualMergeOrder {
			get {
				if (MergedChildren.Count == 0)
					return MergeOrder;
				return MergedChildren[MergedChildren.Count - 1].MergeOrder;
			}
		}		
		internal RibbonControl MergedParentRibbon = null;
		List<RibbonPageCategoryBase> MergedChildren = new List<RibbonPageCategoryBase>();
		internal RibbonPageCategoryBase MergedParent = null;
		internal RibbonPageCategoryBase ReplacedCategory = null;
		void AddMergingPage(RibbonPage page) {
			ActualPagesCore.Add(page);
			page.MergedParentCategory = this;
		}
		void RemoveMergedPage(RibbonPage page) {
			ActualPagesCore.Remove(page);
			page.MergedParentCategory = null;
		}
		internal void RemoveUnusedPagesForMerging() {
			foreach (RibbonPage page in Pages) {
				if (page.MergeType == RibbonMergeType.Remove)
					ActualPagesCore.Remove(page);
			}
		}
		internal void RestoreUnusedPagesForMerging() {
			int index = 0;
			foreach (RibbonPage page in Pages) {
				if (page.MergeType == RibbonMergeType.Remove)
					ActualPagesCore.Insert(index, page);
				index++;
			}
		}
		internal void Merge(RibbonPageCategoryBase childCategory) {
			if (MergedChildren.Contains(childCategory))
				return;
			MergedChildren.Add(childCategory);
			foreach (RibbonPage childPage in childCategory.Pages) {
				mergedPages.Add(childCategory, childPage);
				if (childPage.MergeType == RibbonMergeType.Remove)
					continue;
				RibbonPage sameCaptionPage = GetPageByCaption(Convert.ToString(childPage.Caption));
				if (childPage.MergeType == RibbonMergeType.Add || sameCaptionPage == null) {
					AddMergingPage(childPage);
				} else if (childPage.MergeType == RibbonMergeType.Replace) {
					ActualPagesCore[ActualPagesCore.IndexOf(sameCaptionPage)] = childPage;
					childPage.ReplacedPage = sameCaptionPage;
					childPage.MergedParentCategory = this;
				} else
					sameCaptionPage.Merge(childPage);
			}
			childCategory.MergedParent = this;
		}
		internal void UnMerge(RibbonPageCategoryBase cat) {
			if(!MergedChildren.Contains(cat))
				return;
			MergedChildren.Remove(cat);
			var pages = mergedPages[cat];
			foreach (RibbonPage page in pages) {
				if (page.MergedParent != null)
					page.MergedParent.UnMerge(page);
				if (page.ReplacedPage != null) {
					ActualPagesCore[ActualPagesCore.IndexOf(page)] = page.ReplacedPage;
					page.ReplacedPage = null;
					page.MergedParentCategory = null;
				}
				RemoveMergedPage(page);
			}
			mergedPages.Remove(cat);
			cat.MergedParent = null;
		}
		#endregion
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { yield return typeof(IFrameworkInputElement); }
		}		
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			return Name;
		}
		IActionContainer IControllerAction.Container { get; set; }
		object IBarManagerControllerAction.GetObject() { return this; }
		void IControllerAction.Execute(DependencyObject context) { CollectionActionHelper.Execute(this); }
	}
	[ContentProperty("Pages")]
	public class RibbonPageCategory : RibbonPageCategoryBase {
		#region static
		public static readonly DependencyProperty SelectedPageOnCategoryShowingProperty;
		static RibbonPageCategory() {
			SelectedPageOnCategoryShowingProperty = DependencyPropertyManager.Register("SelectedPageOnCategoryShowing", typeof(SelectedPageOnCategoryShowing), typeof(RibbonPageCategory), new FrameworkPropertyMetadata(SelectedPageOnCategoryShowing.FirstPage));
		}
		#endregion
		#region dep props
		public SelectedPageOnCategoryShowing SelectedPageOnCategoryShowing {
			get { return (SelectedPageOnCategoryShowing)GetValue(SelectedPageOnCategoryShowingProperty); }
			set { SetValue(SelectedPageOnCategoryShowingProperty, value); }
		}
		#endregion
		protected override RibbonPageCategoryBase CreateEmptyClone() {
			return new RibbonPageCategory();
		}
		protected override void OnIsVisibleChanged(bool oldValue) {
			base.OnIsVisibleChanged(oldValue);
			if (IsVisible) {
				if (SelectedPageOnCategoryShowing == SelectedPageOnCategoryShowing.FirstPage) {
					RibbonPage firstSelectablePage = GetFirstSelectablePage();
					if (firstSelectablePage != null)
						firstSelectablePage.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
				}
			}
		}
	}
	[ContentProperty("Pages")]
	public class RibbonDefaultPageCategory : RibbonPageCategoryBase {
		public RibbonDefaultPageCategory() {
			IsDefault = true;
		}
		protected override void IsVisibleChanging(ref bool newValue) {
			newValue = newValue || MergedParent == null;
		}
		protected override void IsDefaultChanging(ref bool newValue) {
			newValue = true;
		}
		protected override RibbonPageCategoryBase CreateEmptyClone() {
			return new RibbonDefaultPageCategory();			
		}
	}
	public enum SelectedPageOnCategoryShowing { FirstPage, None }
	public class ActualCategoriesConverterExtension : MarkupExtension, System.Windows.Data.IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
