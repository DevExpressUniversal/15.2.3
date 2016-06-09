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

using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Data;
using System.Linq;
using System;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Flyout;
namespace DevExpress.Xpf.NavBar {
	public enum DisplaySource {
		Items,
		Content
	}
	class ConditionalPrioritizedDataTemplateSelector : DataTemplateSelector {
		List<object> templatesAndSelectors = null;
		Func<DataTemplate, bool> condition = null;
		public ConditionalPrioritizedDataTemplateSelector(Func<DataTemplate, bool> condition) {
			templatesAndSelectors = new List<object>();
			this.condition = condition;
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			DataTemplate template = null;
			foreach (var element in templatesAndSelectors) {
				if (template != null) break;
				if (element is DataTemplate) {
					template = ((DataTemplate)element).If(x => condition(x));
				}
				if (element is DataTemplateSelector) {
					template = ((DataTemplateSelector)element).SelectTemplate(item, container).If(x => condition(x));
				}
			}
			return template ?? base.SelectTemplate(item, container);
		}
		public void Add(DataTemplate template, DataTemplateSelector selector) {
			if (selector != null)
				templatesAndSelectors.Add(selector);
			if (template != null)
				templatesAndSelectors.Add(template);			
		}
	}
	public class ItemForegroundWrapper : Control {
		public Control Owner {
			get { return (Control)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		public Brush OwnerForeground {
			get { return (Brush)GetValue(OwnerForegroundProperty); }
			set { SetValue(OwnerForegroundProperty, value); }
		}
		public static readonly DependencyProperty OwnerForegroundProperty =
			DependencyPropertyManager.Register("OwnerForeground", typeof(Brush), typeof(ItemForegroundWrapper), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty OwnerProperty =
			DependencyPropertyManager.Register("Owner", typeof(Control), typeof(ItemForegroundWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((ItemForegroundWrapper)d).OnOwnerChanged((Control)e.OldValue)));
		protected virtual void OnOwnerChanged(Control oldValue) {
			ClearValue(OwnerForegroundProperty);
			if (Owner != null) {
				var nbic = LayoutHelper.FindParentObject<NavBarItemControl>(this);
				if (nbic == null || nbic.Style == null || !nbic.Style.Setters.Any(x => x is Setter && ((Setter)x).Property == ForegroundProperty)) {
					Owner.SetBinding(Control.ForegroundProperty, new Binding("OwnerForeground") { Source = this, Mode = BindingMode.TwoWay });
				}
			}
		}
	}
	[DefaultProperty("Items"), ContentProperty("Items")]
	public partial class NavBarGroup : DXFrameworkContentElement, INavigationItem, INotifyPropertyChanged {
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty LayoutSettingsProperty;
		public static readonly DependencyProperty ImageSettingsProperty;
		public static readonly DependencyProperty ItemDisplayModeProperty;
		public static readonly DependencyProperty ItemLayoutSettingsProperty;
		public static readonly DependencyProperty ItemImageSettingsProperty;
		public static readonly DependencyProperty ItemFontSettingsProperty;
		public static readonly DependencyProperty FontSettingsProperty;
		public static readonly DependencyPropertyKey ActualDisplayModePropertyKey;
		public static readonly DependencyPropertyKey ActualLayoutSettingsPropertyKey;
		public static readonly DependencyPropertyKey ActualImageSettingsPropertyKey;		
		protected static readonly DependencyPropertyKey ActualFontSettingsPropertyKey;
		public static readonly DependencyProperty ActualFontSettingsProperty;
		public static readonly DependencyProperty ActualDisplayModeProperty;
		public static readonly DependencyProperty ActualLayoutSettingsProperty;
		public static readonly DependencyProperty ActualImageSettingsProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty TemplateProperty;
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		static readonly DependencyPropertyKey ActualHeaderTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty NavPaneGroupButtonTemplateProperty;
		static readonly DependencyPropertyKey ActualNavPaneGroupButtonTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualNavPaneGroupButtonTemplateSelectorProperty;
		public static readonly DependencyProperty NavPaneOverflowGroupTemplateProperty;
		static readonly DependencyPropertyKey ActualNavPaneOverflowGroupTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualNavPaneOverflowGroupTemplateSelectorProperty;
		public static readonly DependencyProperty ItemTemplateProperty;		
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty EmptyItemTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty AttachedItemTemplateSelectorProperty;		
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ActualItemTemplateSelectorProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty DisplaySourceProperty;
		static readonly DependencyPropertyKey IsActivePropertyKey;
		public static readonly DependencyProperty IsActiveProperty;
		static readonly DependencyPropertyKey NavBarPropertyKey;	
		public static readonly DependencyProperty NavBarProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedItemIndexProperty;
		public static readonly DependencyProperty NavigationPaneVisibleProperty;		
		public static readonly DependencyProperty ImageSourceProperty;
		public static readonly DependencyProperty IsCollapsingProperty;
		public static readonly DependencyProperty IsExpandingProperty;
		public static readonly DependencyProperty VisualStyleProperty;
		static readonly DependencyPropertyKey ActualVisualStylePropertyKey;
		public static readonly DependencyProperty ActualVisualStyleProperty;
		public static readonly DependencyProperty ItemVisualStyleProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty NavPaneGroupButtonTemplateSelectorProperty;
		public static readonly DependencyProperty NavPaneOverflowGroupTemplateSelectorProperty;
		public static readonly DependencyProperty GroupHeaderTemplateProperty;
		public static readonly DependencyProperty ActualGroupHeaderTemplateProperty;
		static readonly DependencyPropertyKey ActualScrollModePropertyKey;
		public static readonly DependencyProperty ActualScrollModeProperty;
		public static readonly DependencyProperty GroupScrollModeProperty;
		internal static readonly DependencyProperty ViewScrollModeProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		protected static readonly DependencyPropertyKey ActualIsVisiblePropertyKey;
		public static readonly DependencyProperty ActualIsVisibleProperty;
		internal static readonly DependencyPropertyKey CustomIsVisiblePropertyKey;
		internal static readonly DependencyProperty CustomIsVisibleProperty;
		internal static readonly DependencyProperty UseCustomIsVisibleProperty;
		protected static readonly DependencyPropertyKey AnimateGroupExpansionPropertyKey;
		public static readonly DependencyProperty AnimateGroupExpansionProperty;
		public static readonly DependencyProperty ImageSourceInNavPaneMenuProperty;
		public static readonly DependencyProperty FlowDirectionProperty;
		public static readonly DependencyProperty NavPaneShowModeProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualItemsSourceProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsSourceProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsStyleProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsTemplateProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CollapsedNavPaneItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty CollapsedNavPaneSelectedItemProperty;
		public static readonly DependencyProperty PeekFormTemplateProperty;
		public static readonly DependencyProperty PeekFormTemplateSelectorProperty;
		SynchronizedItemCollection synchronizedItems;
		public SynchronizedItemCollection SynchronizedItems { get { return synchronizedItems; } }
		static readonly DependencyProperty ItemsProperty;
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		protected internal virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualItemsSource();
			NavBar.With(x => x.SelectionStrategy).Do(x => x.UpdateSelectedItem());
		}
		protected virtual void OnNavPaneShowModeChanged(ShowMode oldValue) {
			if (NavBar != null && NavBar.View is NavigationPaneView) {
				NavBar.View.With((x) 
					=> ((NavigationPaneView)x).Panel).With((x) 
						=> ((NavigationPanePanel)x).ActiveGroup as NavPaneActiveGroupControl).With((x) 
							=> ((NavPaneActiveGroupControl)x).CollapsedActiveGroupControl).Do((x) 
								=> ((CollapsedActiveGroupControl)x).UpdateShowMode());
			}
		}
		protected virtual void UpdateActualItemsSource() {
			SetValue(ActualItemsSourceProperty, ItemsSource);
		}
		protected virtual void OnActualItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			NavBar.With(x => x.SelectionStrategy).Do(x => x.Lock());
			ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>.OnItemsSourcePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty,
				EmptyItemTemplateProperty,
				AttachedItemTemplateSelectorProperty,
				ItemStyleProperty,
				navBarGroup => navBarGroup.Items,
				navBarGroup => new NavBarItem(),
				null, item => { }, null, LinkItem);
			NavBar.With(x => x.SelectionStrategy).Do(x => x.Unlock());
		}
		protected virtual void LinkItem(NavBarItem item, object source) {
			item.SourceObject = source;
			NavBar.With(x => x.View).Do(x => x.RaiseItemAdding(item, source));
		}
		protected virtual void OnItemStyleChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			RecreateItems(e);
		}		
		void RecreateItems(System.Windows.DependencyPropertyChangedEventArgs e) {
			NavBar.With(x => x.SelectionStrategy).Do(x => x.Lock());
			ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorProperty);
			NavBar.With(x => x.SelectionStrategy).Do(x => x.Unlock());
		}
		static NavBarGroup() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavBarGroup), new FrameworkPropertyMetadata(typeof(NavBarGroup)));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnContentChanged(e.OldValue)));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d,e)=>((NavBarGroup)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateCanExecute()));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateCanExecute()));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(NavBarGroup), new PropertyMetadata(true, OnIsVisibleChanged));
			TemplateProperty = DependencyPropertyManager.Register("Template", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null));
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(object), typeof(NavBarGroup), new PropertyMetadata(string.Empty, OnHeaderPropertyChanged));
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualHeaderTemplateSelector()));
			ActualHeaderTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			NavPaneGroupButtonTemplateProperty = DependencyPropertyManager.Register("NavPaneGroupButtonTemplate", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualNavPaneGroupButtonTemplateSelector()));
			NavPaneOverflowGroupTemplateProperty = DependencyPropertyManager.Register("NavPaneOverflowGroupTemplate", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualNavPaneOverflowGroupTemplateSelector()));
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateSelectors()));
			ItemTemplateSelectorProperty = DependencyPropertyManager.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateSelectors()));
			ActualItemTemplateSelectorProperty = DependencyProperty.Register("ActualItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(NavBarGroup), new PropertyMetadata(true, (d, e) => ((NavBarGroup)d).OnIsExpandedChanged(), (d, value) => ((NavBarGroup)d).CoerceIsExpanded((bool)value)));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(NavBarGroup), new PropertyMetadata(null));
			DisplaySourceProperty = DependencyPropertyManager.Register("DisplaySource", typeof(DisplaySource), typeof(NavBarGroup), new PropertyMetadata(DisplaySource.Items));
			IsActivePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsActive", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(false, (d, e) => ((NavBarGroup)d).RaiseInternalIsActiveChanged(new EventArgs())));
			IsActiveProperty = IsActivePropertyKey.DependencyProperty;
			NavBarPropertyKey = DependencyPropertyManager.RegisterReadOnly("NavBar", typeof(NavBarControl), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnNavBarChanged((NavBarControl)e.OldValue, (NavBarControl)e.NewValue)));
			NavBarProperty = NavBarPropertyKey.DependencyProperty;
			ActualScrollModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualScrollMode", typeof(ScrollMode), typeof(NavBarGroup), new PropertyMetadata(ScrollMode.Buttons));
			ActualScrollModeProperty = ActualScrollModePropertyKey.DependencyProperty;
			GroupScrollModeProperty = DependencyPropertyManager.Register("GroupScrollMode", typeof(ScrollMode?), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnScrollModeChanged()));
			ViewScrollModeProperty = DependencyPropertyManager.Register("ViewScrollMode", typeof(ScrollMode), typeof(NavBarGroup), new PropertyMetadata(ScrollMode.Buttons, (d, e) => ((NavBarGroup)d).OnScrollModeChanged()));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(NavBarItem), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnSelectedItemChanged((NavBarItem)e.OldValue, (NavBarItem)e.NewValue), (d, value) => ((NavBarGroup)d).CoerceSelectedItem((NavBarItem)value)));
			SelectedItemIndexProperty = DependencyPropertyManager.Register("SelectedItemIndex", typeof(int), typeof(NavBarGroup), new PropertyMetadata(ConstantHelper.InvalidIndex, (d, e) => ((NavBarGroup)d).OnSelectedItemIndexChanged((int)e.OldValue, (int)e.NewValue), (d, value) => ((NavBarGroup)d).CoerceSelectedItemIndex((int)value)));
			NavigationPaneVisibleProperty = DependencyPropertyManager.Register("NavigationPaneVisible", typeof(bool), typeof(NavBarGroup), new PropertyMetadata(true, (d, e) => ((NavBarGroup)d).UpdateNavPaneGroups()));			
			ImageSourceProperty = DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), typeof(NavBarGroup), new PropertyMetadata(null, (d,e)=>((NavBarGroup)d).UpdateMenuGroupItemGlyph()));
			IsCollapsingProperty = DependencyPropertyManager.Register("IsCollapsing", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(false));
			IsExpandingProperty = DependencyPropertyManager.Register("IsExpanding", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(false));
			VisualStyleProperty = DependencyPropertyManager.Register("VisualStyle", typeof(Style), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualVisualStyle()));
			ActualVisualStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualVisualStyle", typeof(Style), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnActualVisualStyleChanged(d, e)));
			ActualVisualStyleProperty = ActualVisualStylePropertyKey.DependencyProperty;
			ItemVisualStyleProperty = DependencyPropertyManager.Register("ItemVisualStyle", typeof(Style), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualItemVisualStyle()));
			HeaderTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualHeaderTemplateSelector()));
			ActualHeaderTemplateSelectorProperty = ActualHeaderTemplateSelectorPropertyKey.DependencyProperty;
			ActualNavPaneGroupButtonTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualNavPaneGroupButtonTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			ActualNavPaneGroupButtonTemplateSelectorProperty = ActualNavPaneGroupButtonTemplateSelectorPropertyKey.DependencyProperty;
			NavPaneOverflowGroupTemplateSelectorProperty = DependencyPropertyManager.Register("NavPaneOverflowGroupTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualNavPaneOverflowGroupTemplateSelector()));
			ActualNavPaneOverflowGroupTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualNavPaneOverflowGroupTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			ActualNavPaneOverflowGroupTemplateSelectorProperty = ActualNavPaneOverflowGroupTemplateSelectorPropertyKey.DependencyProperty;			
			NavPaneGroupButtonTemplateSelectorProperty = DependencyPropertyManager.Register("NavPaneGroupButtonTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateActualNavPaneGroupButtonTemplateSelector()));
			GroupHeaderTemplateProperty = DependencyPropertyManager.Register("GroupHeaderTemplate", typeof(ControlTemplate), typeof(NavBarGroup), new PropertyMetadata(null, (d, e)=> ((NavBarGroup)d).OnGroupHeaderTemplateChanged()));
			ActualGroupHeaderTemplateProperty = DependencyPropertyManager.Register("ActualGroupHeaderTemplate", typeof(ControlTemplate), typeof(NavBarGroup), new PropertyMetadata(null));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(IEnumerable), typeof(NavBarGroup), new PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnItemsSourceChanged(e)));
			UseCustomIsVisibleProperty = DependencyPropertyManager.Register("UseCustomIsVisible", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(false, (d, e) => ((NavBarGroup)d).UpdateActualIsVisible()));
			ActualIsVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsVisible", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(true, (d, e) => ((NavBarGroup)d).OnActualIsVisibleChanged((bool)e.OldValue)));
			CustomIsVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("CustomIsVisible", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(true, (d, e) => ((NavBarGroup)d).UpdateActualIsVisible()));
			CustomIsVisibleProperty = CustomIsVisiblePropertyKey.DependencyProperty;
			ActualIsVisibleProperty = ActualIsVisiblePropertyKey.DependencyProperty;
			AnimateGroupExpansionPropertyKey = DependencyPropertyManager.RegisterReadOnly("AnimateGroupExpansion", typeof(bool), typeof(NavBarGroup), new FrameworkPropertyMetadata(false));
			AnimateGroupExpansionProperty = AnimateGroupExpansionPropertyKey.DependencyProperty;
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DisplayMode), typeof(NavBarGroup), new FrameworkPropertyMetadata(DisplayMode.Default, (d, e) => ((NavBarGroup)d).OnDisplayModeChanged((DisplayMode)e.OldValue)));
			LayoutSettingsProperty = DependencyPropertyManager.Register("LayoutSettings", typeof(LayoutSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnLayoutSettingsChanged((LayoutSettings)e.OldValue)));
			ImageSettingsProperty = DependencyPropertyManager.Register("ImageSettings", typeof(ImageSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnImageSettingsChanged((ImageSettings)e.OldValue)));
			ItemDisplayModeProperty = DependencyPropertyManager.Register("ItemDisplayMode", typeof(DisplayMode), typeof(NavBarGroup), new FrameworkPropertyMetadata(DisplayMode.Default, (d, e) => ((NavBarGroup)d).OnItemDisplayModeChanged((DisplayMode)e.OldValue)));
			ItemLayoutSettingsProperty = DependencyPropertyManager.Register("ItemLayoutSettings", typeof(LayoutSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnItemLayoutSettingsChanged((LayoutSettings)e.OldValue)));
			ItemImageSettingsProperty = DependencyPropertyManager.Register("ItemImageSettings", typeof(ImageSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnItemImageSettingsChanged((ImageSettings)e.OldValue)));
			ItemFontSettingsProperty = DependencyPropertyManager.Register("ItemFontSettings", typeof(FontSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnItemFontSettingsChanged((FontSettings)e.OldValue)));
			FontSettingsProperty = DependencyPropertyManager.Register("FontSettings", typeof(FontSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnFontSettingsChanged((FontSettings)e.OldValue)));
			ActualDisplayModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDisplayMode", typeof(DisplayMode), typeof(NavBarGroup), new FrameworkPropertyMetadata(DisplayMode.Default));
			ActualLayoutSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLayoutSettings", typeof(LayoutSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			ActualImageSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualImageSettings", typeof(ImageSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			ActualFontSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualFontSettings", typeof(FontSettings), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			ActualFontSettingsProperty = ActualFontSettingsPropertyKey.DependencyProperty;
			ActualDisplayModeProperty = ActualDisplayModePropertyKey.DependencyProperty;
			ActualLayoutSettingsProperty = ActualLayoutSettingsPropertyKey.DependencyProperty;
			ActualImageSettingsProperty = ActualImageSettingsPropertyKey.DependencyProperty;	
			ImageSourceInNavPaneMenuProperty = DependencyPropertyManager.Register("ImageSourceInNavPaneMenu", typeof(ImageSource), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).UpdateMenuGroupItemGlyph()));
			NavPaneShowModeProperty = DependencyPropertyManager.Register("NavPaneShowMode", typeof(ShowMode), typeof(NavBarGroup), new FrameworkPropertyMetadata(ShowMode.MaximizedDefaultItem, (d, e) => ((NavBarGroup)d).OnNavPaneShowModeChanged((ShowMode)e.OldValue)));
			FlowDirectionProperty = DependencyPropertyManager.Register("FlowDirection", typeof(FlowDirection), typeof(NavBarGroup), new FrameworkPropertyMetadata(FlowDirection.LeftToRight, (d, e) => ((NavBarGroup)d).OnFlowDirectionChanged((FlowDirection)e.OldValue)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnItemStyleChanged(e)));
			ActualItemsSourceProperty = DependencyProperty.Register("ActualItemsSource", typeof(IEnumerable), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnActualItemsSourceChanged(e)));
			ItemsAttachedBehaviorProperty = DependencyProperty.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null));
			EmptyItemTemplateProperty = DependencyPropertyManager.Register("FakeItemTemplate", typeof(DataTemplate), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			AttachedItemTemplateSelectorProperty = DependencyProperty.Register("AttachedItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null, (d, e) => ((NavBarGroup)d).RecreateItems(e)));
			CollapsedNavPaneItemsSourceProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsSource", typeof(IEnumerable), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnCollapsedNavPaneItemsSourceChanged(e)));
			ItemsProperty = DependencyProperty.Register("Items", typeof(NavBarItemCollection), typeof(NavBarGroup), new System.Windows.PropertyMetadata(null, StartSycnronization));
			CollapsedNavPaneItemsProperty = DependencyPropertyManager.Register("CollapsedNavPaneItems", typeof(ObservableCollection<NavBarItem>), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			CollapsedNavPaneItemsStyleProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsStyle", typeof(Style), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnCollapsedNavPaneItemsPropertiesChanged(e)));
			CollapsedNavPaneItemsTemplateProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsTemplate", typeof(DataTemplate), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnCollapsedNavPaneItemsPropertiesChanged(e)));
			CollapsedNavPaneItemsTemplateSelectorProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroup)d).OnCollapsedNavPaneItemsPropertiesChanged(e)));
			CollapsedNavPaneItemsAttachedBehaviorProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			CollapsedNavPaneSelectedItemProperty = DependencyPropertyManager.Register("CollapsedNavPaneSelectedItem", typeof(object), typeof(NavBarGroup), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((NavBarGroup)d).OnCollapsedNavPaneSelectedItemChanged((object)e.OldValue), CoerceCollapsedNavPaneSelectedItem));
			PeekFormTemplateProperty = DependencyPropertyManager.Register("PeekFormTemplate", typeof(DataTemplate), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
			PeekFormTemplateSelectorProperty = DependencyPropertyManager.Register("PeekFormTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarGroup), new FrameworkPropertyMetadata(null));
		}
		private static object CoerceCollapsedNavPaneSelectedItem(DependencyObject d, object baseValue) {
			if (
				((NavBarGroup)d).NavBar == null ||
				 !(((NavBarGroup)d).NavBar.View is NavigationPaneView) ||
				 !(((NavBarGroup)d).NavBar.View as NavigationPaneView).CollapsedNavPaneSelectionStrategy.IsValidItem(baseValue)
				) {
				return null;
			}
			return baseValue;
		}
		static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroup)d).RaisePropertyChanged("Header");
		}	 
		void OnCollapsedNavPaneItemsPropertiesChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, CollapsedNavPaneItemsAttachedBehaviorProperty);
		}
		void OnCollapsedNavPaneItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<NavBarGroup, NavBarItem>.OnItemsSourcePropertyChanged(this,
				e,
				CollapsedNavPaneItemsAttachedBehaviorProperty,
				CollapsedNavPaneItemsTemplateProperty,
				CollapsedNavPaneItemsTemplateSelectorProperty,
				CollapsedNavPaneItemsStyleProperty,
				navBarGroup => navBarGroup.CollapsedNavPaneItems,
				navBarGroup => new NavBarItem(),
				null, item => { }, null, (it, obj) => { it.SourceObject = obj; it.SetCurrentValueIfDefault(NavBarItem.ContentProperty, obj); });
		}
		static void StartSycnronization(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) { ((NavBarGroup)d).Do(x=>x.synchronizedItems = new SynchronizedItemCollection(x, x.Items)); }
		static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroup)d).CustomIsVisible = (bool)e.NewValue;
			((NavBarGroup)d).UpdateActualIsVisible();
		}
		object sourceObject;
		public IEnumerable CollapsedNavPaneItemsSource {
			get { return (IEnumerable)GetValue(CollapsedNavPaneItemsSourceProperty); }
			set { SetValue(CollapsedNavPaneItemsSourceProperty, value); }
		}
		public ObservableCollection<NavBarItem> CollapsedNavPaneItems {
			get { return (ObservableCollection<NavBarItem>)GetValue(CollapsedNavPaneItemsProperty); }
			set { SetValue(CollapsedNavPaneItemsProperty, value); }
		}
		Binding menuItemDisplayBinding;
		public Binding MenuItemDisplayBinding {
			get { return menuItemDisplayBinding; }
			set { menuItemDisplayBinding = value; }
		}
		public Style CollapsedNavPaneItemsStyle {
			get { return (Style)GetValue(CollapsedNavPaneItemsStyleProperty); }
			set { SetValue(CollapsedNavPaneItemsStyleProperty, value); }
		}
		public DataTemplate CollapsedNavPaneItemsTemplate {
			get { return (DataTemplate)GetValue(CollapsedNavPaneItemsTemplateProperty); }
			set { SetValue(CollapsedNavPaneItemsTemplateProperty, value); }
		}
		public DataTemplateSelector CollapsedNavPaneItemsTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CollapsedNavPaneItemsTemplateSelectorProperty); }
			set { SetValue(CollapsedNavPaneItemsTemplateSelectorProperty, value); }
		}
		public object CollapsedNavPaneSelectedItem {
			get { return (object)GetValue(CollapsedNavPaneSelectedItemProperty); }
			set { SetValue(CollapsedNavPaneSelectedItemProperty, value); }
		}
		protected internal object SourceObject {
			get { return sourceObject; }
			set {
				if (sourceObject == value)
					return;
				sourceObject = value;
				OnSourceObjectChanged();
			}
		}
		protected virtual void OnSourceObjectChanged() {
			CollectionViewGroup group = sourceObject as CollectionViewGroup;
			if (group != null) {
				SetBinding(ItemsSourceProperty, new Binding("Items") { Source = group });
				SetBinding(DataContextProperty, new Binding("Name") { Source = group });
			}
		}
		protected virtual void OnCollapsedNavPaneSelectedItemChanged(object oldValue) {
			if(NavBar == null) return;
			if(!(NavBar.View is NavigationPaneView))
				return;
			if (CollapsedNavPaneSelectedItem == null)
				(NavBar.View as NavigationPaneView).CollapsedNavPaneSelectionStrategy.UnselectItem(this);
			else
				(NavBar.View as NavigationPaneView).CollapsedNavPaneSelectionStrategy.SelectItem(CollapsedNavPaneSelectedItem);
			if (NavBar.ActiveGroup == this)
				(NavBar.View as NavigationPaneView).ActiveGroupCollapsedNavPaneSelectedItem = CollapsedNavPaneSelectedItem;
		}
		protected virtual void OnItemFontSettingsChanged(FontSettings oldValue) {
			UpdateActualItemFontSettings();
		}
		protected virtual void OnFontSettingsChanged(FontSettings oldValue) {
			UpdateActualFontSettings();
		}
		protected virtual void OnItemImageSettingsChanged(ImageSettings oldValue) {
			UpdateActualItemImageSettings();
		}
		protected virtual void OnItemLayoutSettingsChanged(LayoutSettings oldValue) {
			UpdateActualItemLayoutSettings();
		}
		protected virtual void OnItemDisplayModeChanged(DisplayMode oldValue) {
			UpdateActualItemDisplayMode();
		}
		protected virtual void OnImageSettingsChanged(ImageSettings oldValue) {
			UpdateActualImageSettings();
		}
		protected virtual void OnLayoutSettingsChanged(LayoutSettings oldValue) {
			UpdateActualLayoutSettings();
		}
		protected virtual void OnDisplayModeChanged(DisplayMode oldValue) {
			UpdateActualDisplayMode();
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupContent"),
#endif
 Category(Categories.Data)]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupCommand"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupCommandParameter"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupCommandTarget"),
#endif
		Bindable(true)]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupGroupHeaderTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate GroupHeaderTemplate {
			get { return (ControlTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualGroupHeaderTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate ActualGroupHeaderTemplate {
			get { return (ControlTemplate)GetValue(ActualGroupHeaderTemplateProperty); }
			set { SetValue(ActualGroupHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupIsVisible"),
#endif
 Category(Categories.Appearance)]
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		protected internal bool UseCustomIsVisible {
			get { return (bool)GetValue(UseCustomIsVisibleProperty); }
			set { SetValue(UseCustomIsVisibleProperty, value); }
		}
		protected internal bool CustomIsVisible {
			get { return (bool)GetValue(CustomIsVisibleProperty); }
			set { this.SetValue(CustomIsVisiblePropertyKey, value); }
		}
		public bool ActualIsVisible {
			get { return (bool)GetValue(ActualIsVisibleProperty); }
			protected internal set { this.SetValue(ActualIsVisiblePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupHeader"),
#endif
 TypeConverter(typeof(ObjectConverter)), Category(Categories.Data)]
		public object Header {
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupHeaderTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualHeaderTemplateSelector")]
#endif
		public DataTemplateSelector ActualHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualHeaderTemplateSelectorProperty); }
			private set { this.SetValue(ActualHeaderTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavPaneGroupButtonTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate NavPaneGroupButtonTemplate {
			get { return (DataTemplate)GetValue(NavPaneGroupButtonTemplateProperty); }
			set { SetValue(NavPaneGroupButtonTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualNavPaneGroupButtonTemplateSelector")]
#endif
		public DataTemplateSelector ActualNavPaneGroupButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualNavPaneGroupButtonTemplateSelectorProperty); }
			private set { this.SetValue(ActualNavPaneGroupButtonTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavPaneOverflowGroupTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate NavPaneOverflowGroupTemplate {
			get { return (DataTemplate)GetValue(NavPaneOverflowGroupTemplateProperty); }
			set { SetValue(NavPaneOverflowGroupTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualNavPaneOverflowGroupTemplateSelector")]
#endif
		public DataTemplateSelector ActualNavPaneOverflowGroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualNavPaneOverflowGroupTemplateSelectorProperty); }
			private set { this.SetValue(ActualNavPaneOverflowGroupTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupItemTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupItemTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualItemTemplateSelector")]
#endif
		public DataTemplateSelector ActualItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualItemTemplateSelectorProperty); }
			private set { SetValue(ActualItemTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupIsExpanded"),
#endif
 Category(Categories.Appearance)]
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool IsEnabled {
			get { return base.IsEnabled; }
			set { base.IsEnabled = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool AnimateGroupExpansion {
			get { return (bool)GetValue(AnimateGroupExpansionProperty); }
			protected internal set { this.SetValue(AnimateGroupExpansionPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupContentTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupDisplaySource"),
#endif
 Category(Categories.Appearance)]
		public DisplaySource DisplaySource {
			get { return (DisplaySource)GetValue(DisplaySourceProperty); }
			set { SetValue(DisplaySourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupVisualStyle"),
#endif
 Category(Categories.Appearance)]
		public Style VisualStyle {
			get { return (Style)GetValue(VisualStyleProperty); }
			set { SetValue(VisualStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualVisualStyle")]
#endif
		public Style ActualVisualStyle {
			get { return (Style)GetValue(ActualVisualStyleProperty); }
			private set { this.SetValue(ActualVisualStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupItemVisualStyle"),
#endif
 Category(Categories.Appearance)]
		public Style ItemVisualStyle {
			get { return (Style)GetValue(ItemVisualStyleProperty); }
			set { SetValue(ItemVisualStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupIsActive")]
#endif
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }			
			internal set { this.SetValue(IsActivePropertyKey, value); }			
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavBar")]
#endif
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); } 
			internal set { this.SetValue(NavBarPropertyKey, value); } 
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupSelectedItem")]
#endif
		public NavBarItem SelectedItem {
			get { return selectedItem; }
			set { SetValue(SelectedItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupSelectedItemIndex"),
#endif
 Category(Categories.Data)]
		public int SelectedItemIndex {
			get { return selectedItemIndex; }
			set { SetValue(SelectedItemIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavigationPaneVisible"),
#endif
 Category(Categories.Appearance)]
		public bool NavigationPaneVisible {
			get { return (bool)GetValue(NavigationPaneVisibleProperty); }
			set { SetValue(NavigationPaneVisibleProperty, value); }
		}		
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupImageSource"),
#endif
 Category(Categories.Data)]
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupImageSourceInNavPaneMenu"),
#endif
 Category(Categories.Data)]
		public ImageSource ImageSourceInNavPaneMenu {
			get { return (ImageSource)GetValue(ImageSourceInNavPaneMenuProperty); }
			set { SetValue(ImageSourceInNavPaneMenuProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupIsCollapsing")]
#endif
		public bool IsCollapsing {
			get { return (bool)GetValue(IsCollapsingProperty); }
			internal set { SetValue(IsCollapsingProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupIsExpanding")]
#endif
		public bool IsExpanding {
			get { return (bool)GetValue(IsExpandingProperty); }
			internal set { SetValue(IsExpandingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupHeaderTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavPaneGroupButtonTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector NavPaneGroupButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(NavPaneGroupButtonTemplateSelectorProperty); }
			set { SetValue(NavPaneGroupButtonTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupNavPaneOverflowGroupTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector NavPaneOverflowGroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(NavPaneOverflowGroupTemplateSelectorProperty); }
			set { SetValue(NavPaneOverflowGroupTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupActualScrollMode")]
#endif
		public ScrollMode ActualScrollMode {
			get { return (ScrollMode)GetValue(ActualScrollModeProperty); }
			private set { this.SetValue(ActualScrollModePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupGroupScrollMode"),
#endif
 Category(Categories.Appearance)]
		public ScrollMode? GroupScrollMode {
			get { return (ScrollMode?)GetValue(GroupScrollModeProperty); }
			set { SetValue(GroupScrollModeProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarGroupItemsSource")]
#endif
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		internal ScrollMode ViewScrollMode {
			get { return (ScrollMode)GetValue(ViewScrollModeProperty); }
			set { SetValue(ViewScrollModeProperty, value); }
		}
		public FontSettings FontSettings {
			get { return (FontSettings)GetValue(FontSettingsProperty); }
			set { SetValue(FontSettingsProperty, value); }
		}
		public ImageSettings ImageSettings {
			get { return (ImageSettings)GetValue(ImageSettingsProperty); }
			set { SetValue(ImageSettingsProperty, value); }
		}
		public LayoutSettings LayoutSettings {
			get { return (LayoutSettings)GetValue(LayoutSettingsProperty); }
			set { SetValue(LayoutSettingsProperty, value); }
		}
		public DisplayMode DisplayMode {
			get { return (DisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public FontSettings ItemFontSettings {
			get { return (FontSettings)GetValue(ItemFontSettingsProperty); }
			set { SetValue(ItemFontSettingsProperty, value); }
		}
		public ImageSettings ItemImageSettings {
			get { return (ImageSettings)GetValue(ItemImageSettingsProperty); }
			set { SetValue(ItemImageSettingsProperty, value); }
		}
		public LayoutSettings ItemLayoutSettings {
			get { return (LayoutSettings)GetValue(ItemLayoutSettingsProperty); }
			set { SetValue(ItemLayoutSettingsProperty, value); }
		}
		public DisplayMode ItemDisplayMode {
			get { return (DisplayMode)GetValue(ItemDisplayModeProperty); }
			set { SetValue(ItemDisplayModeProperty, value); }
		}
		public FontSettings ActualFontSettings {
			get { return (FontSettings)GetValue(ActualFontSettingsProperty); }
			protected internal set { this.SetValue(ActualFontSettingsPropertyKey, value); }
		}
		public ImageSettings ActualImageSettings {
			get { return (ImageSettings)GetValue(ActualImageSettingsProperty); }
			protected internal set { this.SetValue(ActualImageSettingsPropertyKey, value); }
		}
		public LayoutSettings ActualLayoutSettings {
			get { return (LayoutSettings)GetValue(ActualLayoutSettingsProperty); }
			protected internal set { this.SetValue(ActualLayoutSettingsPropertyKey, value); }
		}
		public DisplayMode ActualDisplayMode {
			get { return (DisplayMode)GetValue(ActualDisplayModeProperty); }
			protected internal set { this.SetValue(ActualDisplayModePropertyKey, value); }
		}
		public ShowMode NavPaneShowMode {
			get { return (ShowMode)GetValue(NavPaneShowModeProperty); }
			set { SetValue(NavPaneShowModeProperty, value); }
		}
		public FlowDirection FlowDirection {
			get { return (FlowDirection)GetValue(FlowDirectionProperty); }
			set { SetValue(FlowDirectionProperty, value); }
		}
		public DataTemplate PeekFormTemplate {
			get { return (DataTemplate)GetValue(PeekFormTemplateProperty); }
			set { SetValue(PeekFormTemplateProperty, value); }
		}
		public DataTemplateSelector PeekFormTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PeekFormTemplateSelectorProperty); }
			set { SetValue(PeekFormTemplateSelectorProperty, value); }
		}
		WeakReference groupControlWR = null;
		public NavBarGroupControl GroupControl {
			get { return groupControlWR == null ? null : groupControlWR.Target as NavBarGroupControl; }
			set {
				NavBarGroupControl oldValue = GroupControl;
				groupControlWR = new WeakReference(value);
				if(value != oldValue)
					OnGroupControlChanged(oldValue);
			}
		}
		protected internal bool HasPeekFormTemplate { get { return PeekFormTemplate.ReturnSuccess() || PeekFormTemplateSelector.ReturnSuccess(); } }
		protected internal DataTemplate GetActualPeekFormTemplate() {
			return PeekFormTemplate ?? PeekFormTemplateSelector.With(x => x.SelectTemplate(this, this.GroupControl));
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarGroupItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data)]
		public NavBarItemCollection Items { get { return (NavBarItemCollection)GetValue(ItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsNavPaneGroup { get { return ActualIsVisible && NavigationPaneVisible; } }
		private object CoerceSelectedItem(NavBarItem navBarItem) {
			if (NavBar != null)
				return NavBar.SelectionStrategy.CoerceSelection(this, navBarItem);
			else
				return navBarItem;
		}
		object CoerceSelectedItemIndex(int newIndex) {
			if (NavBar != null)
				return NavBar.SelectionStrategy.CoerceSelection(this, newIndex);
			else
				return newIndex;
		}
		int selectedItemIndex = -1;
		protected virtual void OnSelectedItemIndexChanged(int oldIndex, int newIndex) {
			selectedItemIndex = newIndex;
			NavBar.SelectionStrategy.SelectItem(this, SelectedItemIndex);
		}
		NavBarItem selectedItem = null;
		protected virtual void OnSelectedItemChanged(NavBarItem oldItem, NavBarItem newItem) {
			if (NavBar != null) {
				selectedItem = newItem;
				NavBar.SelectionStrategy.SelectItem(SelectedItem, this);
				NavBar.SelectionStrategy.UpdateSelectedItemIndexByItem(this, SelectedItem);
			}
		}
		public event EventHandler Activate;
		public event EventHandler Click;
		public NavBarGroupCollection OwnerCollection;
		public NavBarGroup() {
			onCanExecuteChangedEventHandler = new Lazy<EventHandler>(() => new EventHandler(OnCanExecuteChanged));
			SetValue(ItemsProperty, new NavBarItemCollection(this));
			CollapsedNavPaneItems = new ObservableCollection<NavBarItem>();
			CollapsedNavPaneItems.CollectionChanged += OnCollapsedNavPaneItemsCollectionChanged;
		}
		void OnCollapsedNavPaneItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (NavBar != null && NavBar.View is NavigationPaneView)
				(NavBar.View as NavigationPaneView).CollapsedNavPaneSelectionStrategy.UpdateSelectedItem(this);
		}
		Lazy<EventHandler> onCanExecuteChangedEventHandler;
		EventHandler OnCanExecuteChangedEventHandler {
			get { return onCanExecuteChangedEventHandler.Value; }
		}
		protected virtual void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			if(oldCommand != null)
				oldCommand.CanExecuteChanged -= OnCanExecuteChangedEventHandler;
			if(newCommand != null)
				newCommand.CanExecuteChanged += OnCanExecuteChangedEventHandler;
			UpdateCanExecute();
		}
		void OnActualVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}		
		protected virtual void OnContentChanged(object oldValue) {
			if(oldValue != null) {
				RemoveChild(oldValue);
			}
			if(Content != null) {
				AddChild(Content);
			}
		}
		protected virtual void OnGroupControlChanged(NavBarGroupControl oldValue) { }		
		void OnGroupHeaderTemplateChanged() {
			UpdateActualGroupHeaderTemplate();
		}
		protected virtual void OnActualIsVisibleChanged(bool oldValue) {
			UpdateNavPaneGroups();
		}				
		protected virtual void OnFlowDirectionChanged(FlowDirection oldValue) {
			FrameworkElement.SetFlowDirection(this, FlowDirection);
		}
		protected internal virtual void UpdateActualIsVisible() {
			ActualIsVisible = UseCustomIsVisible ? CustomIsVisible : IsVisible;
		}
		void UpdateNavPaneGroups() {			
			if(NavBar != null && NavBar.View != null)
				NavBar.View.UpdateViewForce();
		}
		internal void UpdateActualVisualStyle() {
			ActualVisualStyle = VisualStyle == null && NavBar != null && NavBar.View != null ? NavBar.View.GroupVisualStyle : VisualStyle;
		}
		internal void UpdateUseCustomIsVisible() {
			UseCustomIsVisible = NavBar != null && NavBar.View is NavigationPaneView;
		}
		internal void UpdateScrollMode() {
			if (NavBar == null)
				return;
			BindingOperations.SetBinding(this, ViewScrollModeProperty, new Binding("(0)") { Path = new PropertyPath(ScrollingSettings.ScrollModeProperty), Source = NavBar.View });
		}
		internal void UpdateActualItemVisualStyle() {
			UpdateItems((w) => w.UpdateActualVisualStyle());
		}						
		internal void ChangeGroupExpanded() {
			IsExpanded = !IsExpanded;
		}
		void OnIsExpandedChanged() {
			if(NavBar != null && NavBar.View != null)
				NavBar.View.RaiseEvent(new NavBarGroupExpandedChangedEventArgs(this, IsExpanded) { RoutedEvent = ExplorerBarView.GroupExpandedChangedEvent });
		}
		void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		bool CoerceIsExpanded(bool newValue) {
			return CoerceIsExpandedCore(newValue, IsExpanded);
		}
		bool CoerceIsExpandedCore(bool newValue, bool oldValue) {
			if (NavBar != null && NavBar.View != null && oldValue != newValue) {
				NavBarGroupExpandedChangingEventArgs e = new NavBarGroupExpandedChangingEventArgs(this, newValue);
				e.RoutedEvent = ExplorerBarView.GroupExpandedChangingEvent;
				NavBar.View.RaiseEvent(e);
				if (e.Cancel)
					return oldValue;
			}
			return newValue;
		}
		void OnNavBarChanged(NavBarControl oldControl, NavBarControl newControl) {
			UpdateActualHeaderTemplateSelector();
			UpdateSelectors();
			UpdateActualNavPaneGroupButtonTemplateSelector();
			UpdateActualNavPaneOverflowGroupTemplateSelector();
			UpdateActualVisualStyle();
			UpdateActualGroupHeaderTemplate();
			UpdateActualItemVisualStyle();
			UpdateScrollMode();
			UpdateUseCustomIsVisible();
			UpdateActualAnimateGroupExpansion();
			UpdateActualItemDisplayMode();
			UpdateActualItemImageSettings();
			UpdateActualItemLayoutSettings();
			UpdateActualItemFontSettings();
			UpdateActualLayoutSettings();
			UpdateActualDisplayMode();
			UpdateActualImageSettings();
			UpdateActualFontSettings();
			if(oldControl != null)
				oldControl.RemoveChild(this);
			if(newControl != null)
				newControl.AddChild(this);
			if (newControl != null) {
				SetBinding(FlowDirectionProperty, new Binding("FlowDirection") { Source = newControl });
			}
		}
		void OnScrollModeChanged() {
			ActualScrollMode = GroupScrollMode ?? ViewScrollMode;
		}				
		internal bool isNavBarInitialized = false;		
		void UpdateCanExecute() {
			IsEnabled = CanExecute();
		}
		bool CanExecute() {
			if(Command == null) return true;
			if(Command is RoutedCommand) {
				if(CommandTarget == null) CommandTarget = NavBar as IInputElement;
				return ((RoutedCommand)Command).CanExecute(CommandParameter, CommandTarget);
			} else return Command.CanExecute(CommandParameter);
		}
		internal void UpdateActualHeaderTemplateSelector() {
			ActualHeaderTemplateSelector = GetActualTemplateSelector(NavBarGroup.HeaderTemplateProperty,
				NavBarGroup.HeaderTemplateSelectorProperty, NavigationPaneView.HeaderTemplateProperty, NavigationPaneView.HeaderTemplateSelectorProperty);
		}
		internal void UpdateActualNavPaneGroupButtonTemplateSelector() {
			ActualNavPaneGroupButtonTemplateSelector = GetActualTemplateSelector(NavBarGroup.NavPaneGroupButtonTemplateProperty,
				NavBarGroup.NavPaneGroupButtonTemplateSelectorProperty, NavigationPaneView.GroupButtonTemplateProperty, NavigationPaneView.GroupButtonTemplateSelectorProperty);
		}
		internal void UpdateActualNavPaneOverflowGroupTemplateSelector() {			
			ActualNavPaneOverflowGroupTemplateSelector = GetActualTemplateSelector(NavBarGroup.NavPaneOverflowGroupTemplateProperty,
				NavBarGroup.NavPaneOverflowGroupTemplateSelectorProperty, NavigationPaneView.OverflowGroupTemplateProperty, NavigationPaneView.OverflowGroupTemplateSelectorProperty);
		}
		internal void UpdateSelectors() {
			var attachedSelector = new ConditionalPrioritizedDataTemplateSelector((t) => { var value = NavBarItemsAttachedBehaviorTemplateConsistencyChecker<NavBarItem>.CheckTemplate(t); return value.HasValue ? value.Value : true; });
			var actualSelector = new ConditionalPrioritizedDataTemplateSelector((t) => { var value = NavBarItemsAttachedBehaviorTemplateConsistencyChecker<NavBarItem>.CheckTemplate(t); return value.HasValue ? !value.Value : true; });
			var viewItemTemplate = NavBar.With(x => x.View).With(x => x.ItemTemplate);
			var viewItemTemplateSelector = NavBar.With(x => x.View).With(x => x.ItemTemplateSelector);
			attachedSelector.Add(ItemTemplate, ItemTemplateSelector);
			attachedSelector.Add(viewItemTemplate, viewItemTemplateSelector);
			actualSelector.Add(ItemTemplate, ItemTemplateSelector);
			actualSelector.Add(viewItemTemplate, viewItemTemplateSelector);
			SetValue(NavBarGroup.AttachedItemTemplateSelectorProperty, attachedSelector);
			SetValue(NavBarGroup.ActualItemTemplateSelectorProperty, actualSelector);			
		}	   
		internal void UpdateActualGroupHeaderTemplate() {
			ActualGroupHeaderTemplate = GroupHeaderTemplate == null && NavBar != null && NavBar.View != null ? NavBar.View.GroupHeaderTemplate : GroupHeaderTemplate;
		}
		internal void UpdateActualAnimateGroupExpansion() {
			AnimateGroupExpansion = NavBar != null && NavBar.View != null ? NavBar.View.AnimateGroupExpansion : true;
		}
		protected internal virtual void UpdateActualItemImageSettings() {
			UpdateItems(item => item.UpdateActualImageSettings());
		}
		protected internal virtual void UpdateActualItemLayoutSettings() {
			UpdateItems(item => item.UpdateActualLayoutSettings());
		}
		protected internal virtual void UpdateActualItemDisplayMode() {
			UpdateItems(item => item.UpdateActualDisplayMode());
		}
		protected internal virtual void UpdateActualItemFontSettings() {
			UpdateItems(item => item.UpdateActualFontSettings());
		}
		protected internal virtual void UpdateActualDisplayMode() {
			if(DisplayMode != DisplayMode.Default) {
				ActualDisplayMode = DisplayMode;
				return;
			}
			if(NavBar == null || NavBar.View == null || NavBar.View.GroupDisplayMode == DisplayMode.Default) {
				ActualDisplayMode = DisplayMode.ImageAndText;
				return;
			}
			ActualDisplayMode = NavBar.View.GroupDisplayMode;
		}
		protected internal virtual void UpdateActualLayoutSettings() {
			if(LayoutSettings != null) {
				ActualLayoutSettings = LayoutSettings;
				return;
			}
			if(NavBar == null || NavBar.View == null || NavBar.View.GroupLayoutSettings == null) {
				ActualLayoutSettings = LayoutSettings.Default;
				return;
			}
			ActualLayoutSettings = NavBar.View.GroupLayoutSettings;
		}
		protected internal virtual void UpdateActualImageSettings() {
			if(ImageSettings != null) {
				ActualImageSettings = ImageSettings;
				return;
			}
			if(NavBar == null || NavBar.View == null || NavBar.View.GroupImageSettings == null) {
				ActualImageSettings = ImageSettings.GroupDefault;
				return;
			}
			ActualImageSettings = NavBar.View.GroupImageSettings;
		}
		protected internal virtual void UpdateActualFontSettings() {
			if(FontSettings != null) {
				ActualFontSettings = FontSettings;
				return;
			}
			if(NavBar == null || NavBar.View == null || NavBar.View.GroupFontSettings == null) {
				ActualFontSettings = FontSettings.Default;
				return;
			}
			ActualFontSettings = NavBar.View.GroupFontSettings;
		}
		internal void RaiseActivateEvent() {
			if(Activate != null)
				Activate(this, new EventArgs());
		}
		internal void RaiseClickEvent() {
			bool isEnabled = IsEnabledCore;
			if(!isEnabled) return;
			if(Click != null)
				Click(this, new EventArgs());
			bool canExectuteCommand = ((INavigatorClient)NavBar).Return(x => !x.IsAttached, () => true);
			if(canExectuteCommand && Command != null) {
				if(Command is RoutedCommand)
					((RoutedCommand)Command).Execute(CommandParameter, CommandTarget);
				else Command.Execute(CommandParameter);
			}
		}
		DataTemplateSelector GetActualTemplateSelector(DependencyProperty groupTemplateProperty, DependencyProperty groupTemplateSelectorProperty, DependencyProperty viewTemplateProperty, DependencyProperty viewTemplateSelectorProperty) {
			DataTemplate groupTemplate = (DataTemplate)GetValue(groupTemplateProperty);
			DataTemplateSelector groupTemplateSelector = (DataTemplateSelector)GetValue(groupTemplateSelectorProperty);
			DataTemplate viewTemplate = NavBar != null && NavBar.View != null ? (DataTemplate)NavBar.View.GetValue(viewTemplateProperty) : null;
			DataTemplateSelector viewTemplateSelector = NavBar != null && NavBar.View != null ? (DataTemplateSelector)NavBar.View.GetValue(viewTemplateSelectorProperty) : null;
			bool IsGroupTemplatesAssigned = groupTemplate != null || groupTemplateSelector != null;
			DataTemplate actualTemplate = IsGroupTemplatesAssigned ? groupTemplate : viewTemplate;
			DataTemplateSelector actualTemplateSelector = IsGroupTemplatesAssigned ? groupTemplateSelector : viewTemplateSelector;
			return new ActualTemplateSelectorWrapper(actualTemplateSelector, actualTemplate);
		}
		void UpdateItems(Action<NavBarItem> action) {
			foreach(NavBarItem item in SynchronizedItems)
				action(item);
		}
		Bars.BarButtonItem menuGroupItem;
		protected internal virtual Bars.BarButtonItem MenuGroupItem {
			get {
				if (menuGroupItem == null)
					menuGroupItem = CreateMenuGroupItem();
				return menuGroupItem;
			}
		}
		DevExpress.Xpf.Bars.Native.WeakList<EventHandler> handlersInternalIsActiveChanged = new Bars.Native.WeakList<EventHandler>();
		internal event EventHandler InternalIsActiveChanged {
			add { handlersInternalIsActiveChanged.Add(value); }
			remove { handlersInternalIsActiveChanged.Remove(value); }
		}
		void RaiseInternalIsActiveChanged(EventArgs args) {
			foreach(EventHandler e in handlersInternalIsActiveChanged)
				e(this, args);
		}		
		protected virtual DevExpress.Xpf.Bars.BarButtonItem CreateMenuGroupItem() {
			DevExpress.Xpf.Bars.BarButtonItem bItem = new Bars.BarButtonItem();
			bItem.GlyphSize = Bars.GlyphSize.Small;			
			BindingOperations.SetBinding(bItem, Bars.BarButtonItem.ContentProperty, new Binding("Header") { Source = this, Converter = new DevExpress.Xpf.Core.ObjectToStringConverter() });
			BindingOperations.SetBinding(bItem, Bars.BarButtonItem.IsVisibleProperty, new Binding("ActualIsVisible") { Source = this });
			var showCommand = DelegateCommandFactory.Create(new Action(() => { NavBar.SelectionStrategy.SelectGroup(this); }), new Func<bool>(() => true), false);
			bItem.Command = new DelegateCommand(() => { showCommand.Execute(bItem.CommandParameter); if (Command != null) Command.Execute(CommandParameter); });
			return bItem;
		}
		bool isVisibleInOverflowPanel;
		bool isItemsControlGroup;
		protected internal bool IsOverflowGroup {
			get { return isItemsControlGroup; }
			set {
				if (value == isItemsControlGroup) return;
				bool oldValue = isItemsControlGroup;
				isItemsControlGroup = value;
				UpdateMenuGroupItem();
			}
		}
		protected internal bool IsVisibleInOverflowPanel {
			get { return isVisibleInOverflowPanel; }
			set {
				if (value == isVisibleInOverflowPanel) return;
				bool oldValue = isVisibleInOverflowPanel;
				isVisibleInOverflowPanel = value;
				UpdateMenuGroupItem();
			}
		}
		protected internal virtual void UpdateMenuGroupItem() {
			MenuGroupItem.IsVisible = ActualIsVisible && IsOverflowGroup && !IsVisibleInOverflowPanel;
		}
		protected internal virtual void UpdateMenuGroupItemGlyph() {
			MenuGroupItem.Glyph = ImageSourceInNavPaneMenu ?? ImageSource;
		}
		string INavigationItem.Header {
			get { return Header.ToString(); }
		}
		DataTemplate INavigationItem.PeekFormTemplate {
			get { return this.GetActualPeekFormTemplate(); }
		}
		protected void RaisePropertyChanged(string propertyName) {
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		event PropertyChangedEventHandler propertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
	}
	class NavBarItemsAttachedBehaviorTemplateConsistencyChecker<T> where T : class {
		static Delegate getChildTypeFromChildIndexDelegate;
		static Delegate GetChildTypeFromChildIndexDelegate {
			get {
				if (getChildTypeFromChildIndexDelegate == null)
					getChildTypeFromChildIndexDelegate = CreateGetter(typeof(DataTemplate), "ChildTypeFromChildIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				return getChildTypeFromChildIndexDelegate;
			}
		}
		static Delegate CreateGetter(Type entityType, string propertyName, System.Reflection.BindingFlags bindingFlags) {
			var propertyInfo = entityType.GetProperty(propertyName, bindingFlags);
			var instance = System.Linq.Expressions.Expression.Parameter(propertyInfo.DeclaringType);
			var body = System.Linq.Expressions.Expression.Property(instance, propertyName);
			var getterExpression = System.Linq.Expressions.Expression.Lambda(body, instance);
			return getterExpression.Compile();
		}
		static Dictionary<int, Type> GetChildTypeFromChildIndex(DataTemplate template) {
			return GetChildTypeFromChildIndexDelegate.DynamicInvoke(template) as Dictionary<int, Type>;
		}
		public static bool? CheckTemplate(DataTemplate template) {
			if (template == null) return null;
			var types = GetChildTypeFromChildIndex(template);
			var count = types.Count;
			if (count < 1) return null;
			if (types[1].IsAssignableFrom(typeof(T)))
				return true;
			if (count < 2) return false;
			if (types[1].IsAssignableFrom(typeof(ContentControl)) || types[1].IsAssignableFrom(typeof(ContentPresenter)))
				if (types[2].IsAssignableFrom(typeof(T)))
					return true;
			return false;
		}
	}
}
