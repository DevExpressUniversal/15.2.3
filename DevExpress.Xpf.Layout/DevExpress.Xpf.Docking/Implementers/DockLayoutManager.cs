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

using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Customization;
using DevExpress.Xpf.Docking.Images;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CoreDock = System.Windows.Controls.Dock;
using LK = DevExpress.Xpf.Docking.Platform.FloatingWindowLock.LockerKey;
using MergedEnumerator = DevExpress.Xpf.Docking.Base.MergedEnumerator;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	[SupportDXTheme(TypeInAssembly = typeof(DockLayoutManager))]
	[DXToolboxBrowsableAttribute]
	[ContentProperty("LayoutRoot")]
	[TemplatePart(Name = "PART_LeftAutoHideTray", Type = typeof(AutoHideTray))]
	[TemplatePart(Name = "PART_RightAutoHideTray", Type = typeof(AutoHideTray))]
	[TemplatePart(Name = "PART_TopAutoHideTray", Type = typeof(AutoHideTray))]
	[TemplatePart(Name = "PART_BottomAutoHideTray", Type = typeof(AutoHideTray))]
	[TemplatePart(Name = "PART_LeftAutoHideTrayPanel", Type = typeof(AutoHidePane))]
	[TemplatePart(Name = "PART_RightAutoHideTrayPanel", Type = typeof(AutoHidePane))]
	[TemplatePart(Name = "PART_TopAutoHideTrayPanel", Type = typeof(AutoHidePane))]
	[TemplatePart(Name = "PART_BottomAutoHideTrayPanel", Type = typeof(AutoHidePane))]
	[TemplatePart(Name = "PART_ClosedItemsPanel", Type = typeof(ClosedItemsPanel))]
	public class DockLayoutManager : psvControl, IWeakEventListener, IUIElement, IDisposable, ISupportBatchUpdate, ILogicalOwner {
		#region static
		public static readonly DependencyProperty DockLayoutManagerProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		public static readonly DependencyProperty UIScopeProperty;
		public static readonly DependencyProperty LayoutRootProperty;
		public static readonly DependencyProperty FloatingModeProperty;
		public static readonly DependencyProperty ClosedPanelsBarVisibilityProperty;
		public static readonly DependencyProperty ActiveDockItemProperty;
		public static readonly DependencyProperty ActiveLayoutItemProperty;
		public static readonly DependencyProperty ActiveMDIItemProperty;
		public static readonly DependencyProperty IsCustomizationProperty;
		static readonly DependencyPropertyKey IsCustomizationPropertyKey;
		public static readonly DependencyProperty IsRenamingProperty;
		static readonly DependencyPropertyKey IsRenamingPropertyKey;
		public static readonly DependencyProperty AllowDockItemRenameProperty;
		public static readonly DependencyProperty AllowLayoutItemRenameProperty;
		public static readonly DependencyProperty AllowCustomizationProperty;
		public static readonly DependencyProperty AllowDocumentSelectorProperty;
		public static readonly DependencyProperty AutoHideExpandModeProperty;
		public static readonly DependencyProperty AutoHideModeProperty;
		public static readonly DependencyProperty AllowMergingAutoHidePanelsProperty;
		public static readonly DependencyProperty ShowInvisibleItemsProperty;
		public static readonly DependencyProperty ShowInvisibleItemsInCustomizationFormProperty;
		public static readonly DependencyProperty DestroyLastDocumentGroupProperty;
		public static readonly DependencyProperty ClosedPanelsBarPositionProperty;
		public static readonly DependencyProperty DefaultTabPageCaptionImageProperty;
		public static readonly DependencyProperty DefaultAutoHidePanelCaptionImageProperty;
		public static readonly RoutedEvent RequestUniqueNameEvent;
		public static readonly RoutedEvent ShowingMenuEvent;
		public static readonly RoutedEvent DockItemActivatedEvent;
		public static readonly RoutedEvent LayoutItemActivatedEvent;
		public static readonly RoutedEvent MDIItemActivatedEvent;
		public static readonly RoutedEvent DockItemActivatingEvent;
		public static readonly RoutedEvent DockItemStartDockingEvent;
		public static readonly RoutedEvent DockItemDockingEvent;
		public static readonly RoutedEvent DockItemEndDockingEvent;
		public static readonly RoutedEvent DockItemClosingEvent;
		public static readonly RoutedEvent DockItemClosedEvent;
		public static readonly RoutedEvent DockItemHidingEvent;
		public static readonly RoutedEvent DockItemHiddenEvent;
		public static readonly RoutedEvent DockItemRestoringEvent;
		public static readonly RoutedEvent DockItemRestoredEvent;
		public static readonly RoutedEvent DockItemDraggingEvent;
		public static readonly RoutedEvent DockItemExpandedEvent;
		public static readonly RoutedEvent DockItemCollapsedEvent;
		public static readonly RoutedEvent LayoutItemActivatingEvent;
		public static readonly RoutedEvent LayoutItemSelectionChangingEvent;
		public static readonly RoutedEvent LayoutItemSelectionChangedEvent;
		public static readonly RoutedEvent LayoutItemSizeChangedEvent;
		public static readonly RoutedEvent LayoutItemHiddenEvent;
		public static readonly RoutedEvent LayoutItemRestoredEvent;
		public static readonly RoutedEvent LayoutItemMovedEvent;
		public static readonly RoutedEvent IsCustomizationChangedEvent;
		public static readonly RoutedEvent CustomizationFormVisibleChangedEvent;
		public static readonly RoutedEvent LayoutItemStartRenamingEvent;
		public static readonly RoutedEvent LayoutItemEndRenamingEvent;
		public static readonly RoutedEvent MDIItemActivatingEvent;
		public static readonly RoutedEvent ShowInvisibleItemsChangedEvent;
		public static readonly RoutedEvent ItemIsVisibleChangedEvent;
		public static readonly RoutedEvent ShowingDockHintsEvent;
		public static readonly RoutedEvent MergeEvent;
		public static readonly RoutedEvent UnMergeEvent;
		public static readonly RoutedEvent BeforeItemAddedEvent;
		public static readonly RoutedEvent DockOperationStartingEvent;
		public static readonly RoutedEvent DockOperationCompletedEvent;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty OwnerWindowTitleProperty;
		public static readonly DependencyProperty ShowMaximizedDocumentCaptionInWindowTitleProperty;
		public static readonly DependencyProperty WindowTitleFormatProperty;
		public static readonly DependencyProperty DisposeOnWindowClosingProperty;
		public static readonly DependencyProperty AllowFloatGroupTransparencyProperty;
		public static readonly DependencyProperty EnableWin32CompatibilityProperty;
		public static readonly DependencyProperty ShowFloatWindowsInTaskbarProperty;
		public static readonly DependencyProperty OwnsFloatWindowsProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		public static readonly DependencyProperty MDIMergeStyleProperty;
		public static readonly DependencyProperty RedrawContentWhenResizingProperty;
		public static readonly DependencyProperty DockingStyleProperty;
		public static readonly DependencyProperty FloatingDocumentContainerProperty;
		public static readonly DependencyProperty AllowAeroSnapProperty;
		static DockLayoutManager() {
#if !SILVERLIGHT
			DevExpress.Mvvm.UI.ViewInjection.DocumentGroupStrategy.RegisterStrategy();
#endif
			var dProp = new DependencyPropertyRegistrator<DockLayoutManager>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideUIMetadata(DXSerializer.SerializationProviderProperty, (SerializationProvider)new DXDockingSerializationProvider());
			dProp.OverrideUIMetadata(DXSerializer.SerializationIDDefaultProperty, (string)Docking.SerializationController.DefaultID);
			dProp.Register("OwnerWindowTitle", ref OwnerWindowTitleProperty, (string)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnOwnerWindowTitleChanged());
			dProp.Register("ShowMaximizedDocumentCaptionInWindowTitle", ref ShowMaximizedDocumentCaptionInWindowTitleProperty, (bool)true);
			dProp.Register("WindowTitleFormat", ref WindowTitleFormatProperty, (string)null, null,
				(dObj, value) => ((DockLayoutManager)dObj).CoerceWindowTitleFormat((string)value));
			dProp.Register("DisposeOnWindowClosing", ref DisposeOnWindowClosingProperty, true);
			dProp.Register("AllowFloatGroupTransparency", ref AllowFloatGroupTransparencyProperty, true,
				(d, e) => ((DockLayoutManager)d).UpdateFloatingPaneResources());
			dProp.Register("EnableWin32Compatibility", ref EnableWin32CompatibilityProperty, false,
				(d, e) => ((DockLayoutManager)d).UpdateFloatingPaneResources());
			dProp.Register("ShowFloatWindowsInTaskbar", ref ShowFloatWindowsInTaskbarProperty, false);
			dProp.Register("OwnsFloatWindows", ref OwnsFloatWindowsProperty, true);
			dProp.Register("AllowAeroSnap", ref AllowAeroSnapProperty, true);
			dProp.RegisterDirectEvent<RequestUniqueNameEventHandler>("RequestUniqueName", ref RequestUniqueNameEvent);
			dProp.RegisterDirectEvent<ShowingMenuEventHandler>("ShowingMenu", ref ShowingMenuEvent);
			dProp.RegisterDirectEvent<DockItemCancelEventHandler>("DockItemActivating", ref DockItemActivatingEvent);
			dProp.RegisterDirectEvent<LayoutItemCancelEventHandler>("LayoutItemActivating", ref LayoutItemActivatingEvent);
			dProp.RegisterDirectEvent<MDIItemCancelEventHandler>("MDIItemActivating", ref MDIItemActivatingEvent);
			dProp.RegisterDirectEvent<DockItemActivatedEventHandler>("DockItemActivated", ref DockItemActivatedEvent);
			dProp.RegisterDirectEvent<LayoutItemActivatedEventHandler>("LayoutItemActivated", ref LayoutItemActivatedEvent);
			dProp.RegisterDirectEvent<MDIItemActivatedEventHandler>("MDIItemActivated", ref MDIItemActivatedEvent);
			dProp.RegisterDirectEvent<DockItemCancelEventHandler>("DockItemClosing", ref DockItemClosingEvent);
			dProp.RegisterDirectEvent<DockItemCancelEventHandler>("DockItemHiding", ref DockItemHidingEvent);
			dProp.RegisterDirectEvent<DockItemCancelEventHandler>("DockItemRestoring", ref DockItemRestoringEvent);
			dProp.RegisterDirectEvent<DockItemClosedEventHandler>("DockItemClosed", ref DockItemClosedEvent);
			dProp.RegisterDirectEvent<DockItemEventHandler>("DockItemHidden", ref DockItemHiddenEvent);
			dProp.RegisterDirectEvent<DockItemEventHandler>("DockItemRestored", ref DockItemRestoredEvent);
			dProp.RegisterDirectEvent<DockItemCancelEventHandler>("DockItemStartDocking", ref DockItemStartDockingEvent);
			dProp.RegisterDirectEvent<DockItemDockingEventHandler>("DockItemDocking", ref DockItemDockingEvent);
			dProp.RegisterDirectEvent<DockItemDockingEventHandler>("DockItemEndDocking", ref DockItemEndDockingEvent);
			dProp.RegisterDirectEvent<LayoutItemSelectionChangingEventHandler>("LayoutItemSelectionChanging", ref LayoutItemSelectionChangingEvent);
			dProp.RegisterDirectEvent<LayoutItemSelectionChangedEventHandler>("LayoutItemSelectionChanged", ref LayoutItemSelectionChangedEvent);
			dProp.RegisterDirectEvent<LayoutItemSizeChangedEventHandler>("LayoutItemSizeChanged", ref LayoutItemSizeChangedEvent);
			dProp.RegisterDirectEvent<LayoutItemHiddenEventHandler>("LayoutItemHidden", ref LayoutItemHiddenEvent);
			dProp.RegisterDirectEvent<LayoutItemRestoredEventHandler>("LayoutItemRestored", ref LayoutItemRestoredEvent);
			dProp.RegisterDirectEvent<LayoutItemMovedEventHandler>("LayoutItemMoved", ref LayoutItemMovedEvent);
			dProp.RegisterDirectEvent<LayoutItemCancelEventHandler>("LayoutItemStartRenaming", ref LayoutItemStartRenamingEvent);
			dProp.RegisterDirectEvent<LayoutItemEndRenamingEventHandler>("LayoutItemEndRenaming", ref LayoutItemEndRenamingEvent);
			dProp.RegisterDirectEvent<IsCustomizationChangedEventHandler>("IsCustomizationChanged", ref IsCustomizationChangedEvent);
			dProp.RegisterDirectEvent<CustomizationFormVisibleChangedEventHandler>("CustomizationFormVisibleChanged", ref CustomizationFormVisibleChangedEvent);
			dProp.RegisterDirectEvent<ShowInvisibleItemsChangedEventHandler>("ShowInvisibleItemsChanged", ref ShowInvisibleItemsChangedEvent);
			dProp.RegisterDirectEvent<ItemIsVisibleChangedEventHandler>("ItemIsVisibleChanged", ref ItemIsVisibleChangedEvent);
			dProp.RegisterDirectEvent<DockItemDraggingEventHandler>("DockItemDragging", ref DockItemDraggingEvent);
			dProp.RegisterDirectEvent<DockItemExpandedEventHandler>("DockItemExpanded", ref DockItemExpandedEvent);
			dProp.RegisterDirectEvent<DockItemCollapsedEventHandler>("DockItemCollapsed", ref DockItemCollapsedEvent);
			dProp.RegisterDirectEvent<ShowingDockHintsEventHandler>("ShowingDockHints", ref ShowingDockHintsEvent);
			dProp.RegisterDirectEvent<BarMergeEventHandler>("Merge", ref MergeEvent);
			dProp.RegisterDirectEvent<BarMergeEventHandler>("UnMerge", ref UnMergeEvent);
			dProp.RegisterDirectEvent<BeforeItemAddedEventHandler>("BeforeItemAdded", ref BeforeItemAddedEvent);
			dProp.RegisterDirectEvent<DockOperationStartingEventHandler>("DockOpeartionStarting", ref DockOperationStartingEvent);
			dProp.RegisterDirectEvent<DockOperationCompletedEventHandler>("DockOpearationCompleted", ref DockOperationCompletedEvent);
			dProp.RegisterAttachedInherited("DockLayoutManager", ref DockLayoutManagerProperty, (DockLayoutManager)null, OnDockLayoutManagerChanged);
			dProp.RegisterAttachedInherited("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null, OnLayoutItemChanged, CoerceLayoutItem);
			dProp.RegisterAttached("UIScope", ref UIScopeProperty, (IUIElement)null, OnUIScopeChanged);
			dProp.Register("LayoutRoot", ref LayoutRootProperty, (LayoutGroup)null, OnLayoutRootChanged);
			dProp.Register("FloatingMode", ref FloatingModeProperty, FloatingMode.Window);
			dProp.Register("ClosedPanelsBarVisibility", ref ClosedPanelsBarVisibilityProperty, ClosedPanelsBarVisibility.Default,
				(dObj, e) => ((DockLayoutManager)dObj).OnClosedItemsBarVisibilityChanged((ClosedPanelsBarVisibility)e.NewValue));
			dProp.Register("AutoHideExpandMode", ref AutoHideExpandModeProperty, AutoHideExpandMode.Default);
			dProp.RegisterAttachedInherited("AllowMergingAutoHidePanels", ref AllowMergingAutoHidePanelsProperty, false, OnAllowMergingAutoHidePanelsChanged);
			dProp.Register("AutoHideMode", ref AutoHideModeProperty, AutoHideMode.Default,
			   (d, e) => ((DockLayoutManager)d).OnAutoHideModeChanged((AutoHideMode)e.OldValue, (AutoHideMode)e.NewValue));
			dProp.Register("ActiveDockItem", ref ActiveDockItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnActiveDockItemChanged((BaseLayoutItem)e.OldValue, (BaseLayoutItem)e.NewValue));
			dProp.Register("ActiveLayoutItem", ref ActiveLayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnActiveLayoutItemChanged((BaseLayoutItem)e.OldValue, (BaseLayoutItem)e.NewValue));
			dProp.Register("ActiveMDIItem", ref ActiveMDIItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnActiveMDIItemChanged((BaseLayoutItem)e.OldValue, (BaseLayoutItem)e.NewValue));
			dProp.Register("AllowCustomization", ref AllowCustomizationProperty, true,
				(dObj, e) => ((DockLayoutManager)dObj).OnAllowCustomizationChanged((bool)e.NewValue));
			dProp.Register("AllowDocumentSelector", ref AllowDocumentSelectorProperty, true,
				(dObj, e) => ((DockLayoutManager)dObj).OnAllowDocumentSelectorChanged((bool)e.NewValue));
			dProp.RegisterReadonly("IsCustomization", ref IsCustomizationPropertyKey, ref IsCustomizationProperty, false,
				(dObj, e) => ((DockLayoutManager)dObj).OnIsCustomizationChanged((bool)e.NewValue),
				(dObj, value) => ((DockLayoutManager)dObj).CoerceIsCustomization((bool)value));
			dProp.RegisterReadonly("IsRenaming", ref IsRenamingPropertyKey, ref IsRenamingProperty, false, null,
				(dObj, value) => ((DockLayoutManager)dObj).CoerceIsRenaming((bool)value));
			dProp.Register("AllowDockItemRename", ref AllowDockItemRenameProperty, (bool?)null);
			dProp.Register("AllowLayoutItemRename", ref AllowLayoutItemRenameProperty, (bool?)null);
			dProp.Register("ShowInvisibleItems", ref ShowInvisibleItemsProperty, (bool?)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnShowInvisibleItemsChanged((bool?)e.NewValue));
			dProp.Register("ShowInvisibleItemsInCustomizationForm", ref ShowInvisibleItemsInCustomizationFormProperty, (bool)true,
				(dObj, e) => ((DockLayoutManager)dObj).OnShowInvisibleItemsInCustomizationFormChanged((bool)e.NewValue));
			dProp.Register("DestroyLastDocumentGroup", ref DestroyLastDocumentGroupProperty, false);
			dProp.Register("ClosedPanelsBarPosition", ref ClosedPanelsBarPositionProperty, CoreDock.Top);
			dProp.Register("MDIMergeStyle", ref MDIMergeStyleProperty, MDIMergeStyle.Default,
				(dObj, e) => ((DockLayoutManager)dObj).OnMDIMergeStyleChanged((MDIMergeStyle)e.OldValue, (MDIMergeStyle)e.NewValue));
			dProp.Register("DockingStyle", ref DockingStyleProperty, DockingStyle.Default);
			dProp.Register("RedrawContentWhenResizing", ref RedrawContentWhenResizingProperty, true);
			dProp.Register("FloatingDocumentContainer", ref FloatingDocumentContainerProperty, FloatingDocumentContainer.Default);
			dProp.Register("ItemsSource", ref ItemsSourceProperty, (IEnumerable)null,
				(dObj, e) => ((DockLayoutManager)dObj).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue, e));
			dProp.Register("ItemTemplate", ref ItemTemplateProperty, (DataTemplate)null);
			dProp.Register("ItemTemplateSelector", ref ItemTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.Register("IsSynchronizedWithCurrentItem", ref IsSynchronizedWithCurrentItemProperty, false,
				(dObj, e) => ((DockLayoutManager)dObj).OnIsSynchronizedWithCurrentItemChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("DefaultTabPageCaptionImage", ref DefaultTabPageCaptionImageProperty, (ImageSource)Images.ImageHelper.GetImage(DefaultImages.TabPageCaption));
			dProp.Register("DefaultAutoHidePanelCaptionImage", ref DefaultAutoHidePanelCaptionImageProperty, (ImageSource)Images.ImageHelper.GetImage(DefaultImages.AutoHidePanelCaption));
			dProp.RegisterClassCommandBinding(DockControllerCommand.Activate, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.Close, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.CloseActive, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.Dock, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.Float, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.Hide, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(DockControllerCommand.Restore, DockControllerCommand.Executed, DockControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(SerializationControllerCommand.SaveLayout, SerializationControllerCommand.Executed, SerializationControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(SerializationControllerCommand.RestoreLayout, SerializationControllerCommand.Executed, SerializationControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(CustomizationControllerCommand.ShowClosedItems, CustomizationControllerCommand.Executed, CustomizationControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(CustomizationControllerCommand.HideClosedItems, CustomizationControllerCommand.Executed, CustomizationControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaption, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowControl, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionImageBeforeText, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionImageAfterText, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionOnLeft, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionOnRight, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionAtTop, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(LayoutControllerCommand.ShowCaptionAtBottom, LayoutControllerCommand.Executed, LayoutControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.Minimize, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.Maximize, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.Restore, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.Cascade, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.TileHorizontal, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.TileVertical, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.ArrangeIcons, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
			dProp.RegisterClassCommandBinding(MDIControllerCommand.ChangeMDIStyle, MDIControllerCommand.Executed, MDIControllerCommand.CanExecute);
		}
		internal static DockLayoutManager Ensure(DependencyObject dObj, bool forceFind = false) {
			DockLayoutManager manager = (dObj as DockLayoutManager) ?? DockLayoutManager.GetDockLayoutManager(dObj);
			if(manager == null || forceFind)
				manager = FindManager(dObj);
			return EnsureManager(dObj, manager);
		}
		internal static DockLayoutManager FindManager(DependencyObject dObj) {
			DockLayoutManager manager = dObj as DockLayoutManager;
			if(manager == null) {
				IUIElement uiElement = dObj as IUIElement;
				if(uiElement == null)
					uiElement = dObj.GetParentIUIElement();
				if(uiElement != null) {
					manager = uiElement.GetManager() as DockLayoutManager;
				}
			}
			return manager;
		}
		static DockLayoutManager EnsureManager(DependencyObject dObj, DockLayoutManager manager) {
			dObj.SetValue(DockLayoutManagerProperty, manager);
			return manager;
		}
		internal static void Release(DependencyObject dObj) {
			if(dObj == null) return;
			dObj.ClearValue(DockLayoutManagerProperty);
			dObj.ClearValue(LayoutItemProperty);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static DockLayoutManager GetDockLayoutManager(DependencyObject obj) {
			return (DockLayoutManager)obj.GetValue(DockLayoutManagerProperty);
		}
		public static void SetDockLayoutManager(DependencyObject obj, DockLayoutManager value) {
			obj.SetValue(DockLayoutManagerProperty, value);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static BaseLayoutItem GetLayoutItem(DependencyObject obj) {
			return (BaseLayoutItem)obj.GetValue(LayoutItemProperty);
		}
		public static void SetLayoutItem(DependencyObject obj, BaseLayoutItem value) {
			obj.SetValue(LayoutItemProperty, value);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static IUIElement GetUIScope(DependencyObject obj) {
			return (IUIElement)obj.GetValue(UIScopeProperty);
		}
		public static void SetUIScope(DependencyObject obj, IUIElement value) {
			obj.SetValue(UIScopeProperty, value);
		}
		static BaseLayoutItem FindLayoutItem(DependencyObject dObj) {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(dObj);
			if(item == null) {
				psvContentControl contentControl = dObj as psvContentControl;
				if(contentControl != null)
					item = contentControl.Content as BaseLayoutItem;
			}
			return item;
		}
		static void OnLayoutRootChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			DockLayoutManager manager = dObj as DockLayoutManager;
			if(manager != null) {
				manager.InvalidateMeasure();
				manager.InvalidateArrange();
			}
			LayoutGroup group = e.NewValue as LayoutGroup;
			if(group != null && group.ItemType != LayoutItemType.Group)
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.WrongLayoutRoot));
			if(group != null) {
				group.BeginLayoutChange();
				manager.AddLogicalChild(group);
				group.Manager = manager;
				group.SelectTemplate();
				group.IsRootGroup = true;
				group.EndLayoutChange();
			}
			group = e.OldValue as LayoutGroup;
			if(group != null) {
				group.BeginLayoutChange();
				group.IsRootGroup = false;
				manager.RemoveLogicalChild(group);
				group.ClearTemplate();
				group.EndLayoutChange();
			}
		}
		static void OnDockLayoutManagerChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			if(dObj is IDockLayoutManagerListener)
				NotifyListener((IDockLayoutManagerListener)dObj, e);
			if(e.NewValue == null)
				ClearItemsControlPanel(dObj as UIElement);
			IUIElement element = dObj as IUIElement;
			if(element != null) {
				DockLayoutManager manager = null;
				if(e.OldValue != null) {
					manager = ((DockLayoutManager)e.OldValue);
					manager.ResetView(element);
					manager.UnRegisterView(element);
					if(!(dObj is BaseLayoutItem)) {
						dObj.ClearValue(UIScopeProperty);
						BaseLayoutItem item = FindLayoutItem(dObj);
						if(item != null) {
							item.UIElements.Remove(element);
						}
					}
				}
				if(e.NewValue != null) {
					manager = ((DockLayoutManager)e.NewValue);
					IUIElement scope = dObj is BaseLayoutItem ? null : dObj.FindUIScope();
					if(scope is DockLayoutManager)
						manager.RegisterView(element);
					if(!(dObj is BaseLayoutItem)) {
						dObj.SetValue(UIScopeProperty, scope);
						manager.ResetView(element);
					}
				}
			}
		}
		static object CoerceLayoutItem(DependencyObject dObj, object value) {
			if(dObj is DockLayoutManager) return null;
			return value;
		}
		static void OnLayoutItemChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			BaseLayoutItem oldValue = e.OldValue as BaseLayoutItem;
			BaseLayoutItem newValue = e.NewValue as BaseLayoutItem;
			OnLayoutItemChanged(dObj, oldValue, newValue);
		}
		static void OnLayoutItemChanged(DependencyObject dObj, BaseLayoutItem oldValue, BaseLayoutItem newValue) {
			if(dObj is IUIElement) {
				if(dObj is Splitter) return;
				BaseLayoutItem item = null;
				DockLayoutManager manager = FindManager(dObj);
				if(oldValue != null) {
					item = oldValue;
					item.UIElements.Remove((IUIElement)dObj);
					if(item.Manager != manager) {
						if(((IUIElement)item).Scope == null)
							item.Manager = null;
					}
				}
				if(newValue != null) {
					item = newValue;
					BaseLayoutItem[] items = manager.GetItems();
					if(Array.IndexOf(items, item) != -1) {
						item.UIElements.Add((IUIElement)dObj);
						item.Manager = manager;
					}
				}
				if(manager != null)
					manager.ResetView((IUIElement)dObj);
			}
		}
		static void OnUIScopeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			IUIElement element = dObj as IUIElement;
			if(element != null) {
				if(e.OldValue != null)
					((IUIElement)e.OldValue).Children.Remove(element);
				if(e.NewValue != null)
					((IUIElement)e.NewValue).Children.Add(element);
			}
		}
		static void NotifyListener(IDockLayoutManagerListener listener, DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null)
				listener.Unsubscribe((DockLayoutManager)e.OldValue);
			if(e.NewValue != null)
				listener.Subscribe((DockLayoutManager)e.NewValue);
		}
		static void ClearItemsControlPanel(UIElement uiElement) {
			BaseHeadersPanel tPanel = uiElement as BaseHeadersPanel;
			if(tPanel != null) {
				UIElement[] children = new UIElement[tPanel.Children.Count];
				tPanel.Children.CopyTo(children, 0);
				tPanel.IsItemsHost = false;
				tPanel.Children.Clear();
				for(int i = 0; i < children.Length; i++) {
					children[i].ClearValue(DockLayoutManager.DockLayoutManagerProperty);
				}
			}
			DockBarContainerControl barContainerControl = uiElement as DockBarContainerControl;
			if(barContainerControl != null) {
				BarManagerPropertyHelper.ClearBarManager(barContainerControl);
				BarContainerControlPropertyAccessor.GetBars(barContainerControl).Clear();
			}
		}
		private static void OnAllowMergingAutoHidePanelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DockLayoutManager container = d as DockLayoutManager;
			container.Do(x => x.OnAllowMergingAutoHidePanelsChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		internal static void AddToVisualTree(DockLayoutManager container, FrameworkElement child) {
			if(container == null) return;
			container.AddVisualChild(child);
		}
		internal static void RemoveFromVisualTree(DockLayoutManager container, FrameworkElement child) {
			if(container == null) return;
			container.RemoveVisualChild(child);
		}
		static UIElement[] GetUIElements(BaseLayoutItem[] items) {
			List<UIElement> elements = new List<UIElement>();
			for(int i = 0; i < items.Length; i++) {
				items[i].Accept(
					delegate(BaseLayoutItem itemToLock) {
						UIElement element = itemToLock.GetUIElement();
						if(element != null) elements.Add(element);
						if(itemToLock != null) elements.Add(itemToLock);
					}
				);
			}
			return elements.ToArray();
		}
		protected internal LogicalContainer<UIElement> DockHintsContainer { get; private set; }
		protected internal LogicalContainer<DependencyObject> InternalElementsContainer { get; private set; }
		internal void PurgeLogicalChildren() {
			var toDelete = logicalChildrenCore.Except(this.GetItems().AsEnumerable()).Where(x => x is BaseLayoutItem).ToArray();
			foreach(var item in toDelete) {
				RemoveLogicalChild(item);
			}
		}
		List<LogicalTreeLocker> logicalTreeLocks = new List<LogicalTreeLocker>();
		internal void LockLogicalTreeChanging(LogicalTreeLocker logicalLocker) {
			if(!logicalTreeLocks.Contains(logicalLocker)) {
				logicalTreeLocks.Add(logicalLocker);
			}
		}
		internal void UnlockLogicalTreeChanging(LogicalTreeLocker logicalLocker) {
			logicalTreeLocks.Remove(logicalLocker);
			PurgeLogicalChildren();
		}
		bool IsLogicalTreeChangingLocked(DependencyObject element) {
			return logicalTreeLocks.FirstOrDefault(x => x.IsLocked(element)) != null;
		}
		internal static void AddLogicalChild(DockLayoutManager container, DependencyObject child) {
			if(child == null || container == null) return;
			if(LogicalTreeHelper.GetParent(child) == null)
				container.AddLogicalChild(child);
		}
		internal static void RemoveLogicalChild(DockLayoutManager container, DependencyObject child) {
			if(child == null || container == null) return;
			container.RemoveLogicalChild(child);
		}
		#endregion static
		FloatGroupCollection floatGroupsCore;
		AutoHideGroupCollection autoHideGroupsCore;
		ClosedPanelCollection closedPanelsCore;
		IViewAdapter viewAdapterCore;
		IDockController dockControllerCore;
		ILayoutController layoutControllerCore;
		IMDIController mdiControllerCore;
		ICustomizationController customizationControllerCore;
		ISerializationController serializationControllerCore;
		RenameHelper renameHelperCore;
		DelayedActionsHelper loadedActionsHelper;
		DelayedActionsHelper initilizedActionsHelper;
		LayoutItemStateHelper layoutItemStateHelper;
		ContainerGenerator containerGenerator;
		LayoutGroupCollection decomposedItems;
		VisualCollection visualChildrenCore;
		ThemeTreeWalkerHelper treeWalkerHelper;
		DisableFloatingPanelTransparencyBehavior disableTransparencyBehavior;
		Win32AdornerWindowProvider adornerWindowProvider;
		Win32DragService win32DragService;
		public DockLayoutManager() {
			CheckLicense();
			CoerceValue(WindowTitleFormatProperty);
			ShouldRestoreOnActivate = false;
			DockHintsContainer = new LogicalContainer<UIElement>();
			InternalElementsContainer = new LogicalContainer<DependencyObject>();
			DragAdorner = CreateDragAdorner();
			floatGroupsCore = CreateFloatGroupsCollection();
			autoHideGroupsCore = CreateAutoHideGroupsCollection();
			autoHideGroupsCore.CollectionChanged += OnAutoHideGroupsCollectionChanged;
			closedPanelsCore = CreateClosedPanelsCollection();
			dockControllerCore = CreateDockController();
			layoutControllerCore = CreateLayoutController();
			mdiControllerCore = CreateMDIController();
			customizationControllerCore = CreateCustomizationController();
			serializationControllerCore = CreateSerializationController();
			renameHelperCore = CreateRenameHelper();
			viewAdapterCore = CreateViewAdapter();
			loadedActionsHelper = new DelayedActionsHelper();
			initilizedActionsHelper = new DelayedActionsHelper();
			layoutItemStateHelper = new LayoutItemStateHelper(this);
			containerGenerator = new ContainerGenerator();
			IsVisibleChanged += OnIsVisibleChanged;
			RegisterInputBindings();
			itemsInternal = new DockLayoutManagerItemsCollection(this);
			decomposedItems = new DecomposedItemsCollection(this);
			visualChildrenCore = new VisualCollection(this);
			DockLayoutManager.Ensure(this);
			ThemeChangedEventManager.AddListener(this, this);
			BarNameScope.SetIsScopeOwner(this, true);
			MergingProperties.SetElementMergingBehavior(this, ElementMergingBehavior.InternalWithExternal);
		}
		protected DockLayoutManagerItemsCollection itemsInternal;
		protected virtual void CheckLicense() {
		}
		protected override void OnDispose() {
			Platform.HitTestHelper.ResetCache();
			ThemeChangedEventManager.RemoveListener(this, this);
			IsVisibleChanged -= OnIsVisibleChanged;
			UnsubscribeSystemEvents();
			if(OwnerWindow != null) {
				ResetMDIChildrenTitle();
				UnSubscribeOwnerWindowEvents();
			}
			if(VisualRoot != null)
				UnsubscribeVisualRootEvents();
			VisualRoot = null;
			Ref.Dispose(ref itemsInternal);
			DisposeFloatContainers();
			DisposeAutoHideTrays();
			DisposeClosedPanel();
			DisposeLayoutLayer();
			DisposeFloatingLayer();
			Ref.Dispose(ref serializationControllerCore);
			Ref.Dispose(ref customizationControllerCore);
			Ref.Dispose(ref dockControllerCore);
			Ref.Dispose(ref layoutControllerCore);
			Ref.Dispose(ref mdiControllerCore);
			Ref.Dispose(ref closedPanelsCore);
			Ref.Dispose(ref mergingHelper);
			Ref.Dispose(ref floatGroupsCore);
			autoHideGroupsCore.CollectionChanged -= OnAutoHideGroupsCollectionChanged;
			Ref.Dispose(ref autoHideGroupsCore);
			Ref.Dispose(ref viewAdapterCore);
			Ref.Dispose(ref renameHelperCore);
			Ref.Dispose(ref loadedActionsHelper);
			Ref.Dispose(ref initilizedActionsHelper);
			Ref.Dispose(ref adornerWindowProvider);
			DisposeVisualTreeHost();
			base.OnDispose();
		}
		protected void DisposeLayoutLayer() {
			if(LayoutLayer != null) {
				LayoutLayer.ClearValue(DXContentPresenter.ContentProperty);
				LayoutLayer = null;
			}
			if(LayoutRoot != null) LayoutRoot.ClearTemplate();
		}
		protected void DisposeFloatingLayer() {
			if(FloatingLayer != null) {
				FloatingLayer.Dispose();
				FloatingLayer = null;
			}
		}
		protected void DisposeClosedPanel() {
			if(ClosedItemsPanel != null) {
				ClosedItemsPanel.Dispose();
				ClosedItemsPanel = null;
			}
		}
		protected internal void DisposeFloatContainers() {
			FloatGroup[] groups = FloatGroups.ToArray();
			foreach(FloatGroup floatGroup in groups) {
				FloatPanePresenter floatPresenter = floatGroup.UIElements.GetElement<FloatPanePresenter>();
				if(floatPresenter != null) floatPresenter.Dispose();
			}
		}
		protected void DisposeAutoHideTrays() {
			string[] sides = new string[] { "Left", "Top", "Right", "Bottom" };
			for(int i = 0; i < sides.Length; i++) {
				AutoHideTray tray = GetTemplateChild("PART_" + sides[i] + "AutoHideTray") as AutoHideTray;
				AutoHidePane trayPanel = GetTemplateChild("PART_" + sides[i] + "AutoHideTrayPanel") as AutoHidePane;
				Ref.Dispose(ref trayPanel);
				Ref.Dispose(ref tray);
			}
			autoHideTrayCollection.Clear();
		}
		protected void DisposeVisualTreeHost() {
			visualChildrenCore.Clear();
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return null; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		protected internal System.Windows.Window OwnerWindow { get; private set; }
		protected UIElement VisualRoot { get; private set; }
		protected bool ShouldRestoreCustomizationForm { get; set; }
		protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			bool visible = (bool)e.NewValue;
			ShouldRestoreOnIsVisibleChanged = visible;
			if(visible) {
				loadedActionsHelper.AddDelayedAction(RestoreFloatingWindows);
				VisualizerAdornerHelper.EnsureAdornerActivated(DragAdorner);
				InvokeHelper.BeginInvoke(this, new Action(RestoreCustomization), InvokeHelper.Priority.Render);
				if(ScreenHelper.IsAttachedToPresentationSource(this) && IsLoaded)
					loadedActionsHelper.DoDelayedActions();
			}
			else {
				CustomizationController.HideDocumentSelectorForm();
				HideFloatingWindows();
				HideCustomization();
				ResetAdorners();
			}
		}
		protected internal virtual void HideCustomization() {
			if(IsCustomization) {
				ViewAdapter.ProcessAction(ViewAction.HideSelection);
				ShouldRestoreCustomizationForm = CustomizationController.CustomizationForm != null && CustomizationController.CustomizationForm.IsOpen;
				CustomizationController.HideCustomizationForm();
				RenameHelper.CancelRenaming();
			}
		}
		protected internal virtual void RestoreCustomization() {
			if(IsCustomization) {
				ViewAdapter.ProcessAction(ViewAction.ShowSelection);
				if(ShouldRestoreCustomizationForm)
					CustomizationController.ShowCustomizationForm();
			}
		}
		protected override void OnInitialized() {
			base.OnInitialized();
	  		mergingHelper = new DockLayoutManagerMergingHelper(this);
			AddLogicalChild(mergingHelper);
			BarNameScope.EnsureRegistrator(this);
			EnsureLogicalTree();
			initilizedActionsHelper.DoDelayedActions();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			EnsureLayoutRoot();
			if(!WindowHelper.IsXBAP) {
				EnsureSystemEventsSubscriptions();
				EnsureOwnerWindowSubscriptions();
				EnsureIsCustomizationInDesignTime();
				SaveRestoreOffset();
			}
			if(ScreenHelper.IsAttachedToPresentationSource(this))
				loadedActionsHelper.DoDelayedActions();
		}
		void OnSystemEventsDisplaySettingsChanged(object sender, EventArgs e) {
			foreach(FloatGroup fg in FloatGroups) {
				fg.CoerceValue(FloatGroup.FloatSizeProperty);
			}
		}
		protected virtual void OwnerWindowClosed(object sender) {
			if(DisposeOnWindowClosing) {
				Window ownerWindow = Window.GetWindow(this);
				if(ownerWindow == sender)
					Dispose();
			}
		}
		void EnsureIsCustomizationInDesignTime() {
			if(IsInDesignTime) {
				InvokeHelper.BeginInvoke(this,
					new Action(EnsureIsCustomizationInDesignTimeCore), InvokeHelper.Priority.Render);
			}
		}
		void EnsureIsCustomizationInDesignTimeCore() {
			((Customization.CustomizationController)CustomizationController).CoerceValue(
				Customization.CustomizationController.IsCustomizationProperty);
		}
		InputBinding closeMDIItemCommandBinding;
		InputBinding CloseMDIItemCommandBinding {
			get {
				if(closeMDIItemCommandBinding == null) {
					closeMDIItemCommandBinding = new InputBinding(DockControllerCommand.CloseActive, new KeyGesture(Key.F4, ModifierKeys.Control));
					BindingHelper.SetBinding(closeMDIItemCommandBinding, InputBinding.CommandParameterProperty, this, DockLayoutManager.ActiveMDIItemProperty);
					BindingHelper.SetBinding(closeMDIItemCommandBinding, InputBinding.CommandTargetProperty, this, DockLayoutManager.DockLayoutManagerProperty);
				}
				return closeMDIItemCommandBinding;
			}
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowMaximizedDocumentCaptionInWindowTitle")]
#endif
		public bool ShowMaximizedDocumentCaptionInWindowTitle {
			get { return (bool)GetValue(ShowMaximizedDocumentCaptionInWindowTitleProperty); }
			set { SetValue(ShowMaximizedDocumentCaptionInWindowTitleProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerWindowTitleFormat")]
#endif
		public string WindowTitleFormat {
			get { return (string)GetValue(WindowTitleFormatProperty); }
			set { SetValue(WindowTitleFormatProperty, value); }
		}
		public bool DisposeOnWindowClosing {
			get { return (bool)GetValue(DisposeOnWindowClosingProperty); }
			set { SetValue(DisposeOnWindowClosingProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty]
		public bool AllowFloatGroupTransparency {
			get { return (bool)GetValue(AllowFloatGroupTransparencyProperty); }
			set { SetValue(AllowFloatGroupTransparencyProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Advanced), XtraSerializableProperty]
		public bool EnableWin32Compatibility {
			get { return (bool)GetValue(EnableWin32CompatibilityProperty); }
			set { SetValue(EnableWin32CompatibilityProperty, value); }
		}
		internal bool IsTransparencyDisabled {
			get { return EnableWin32Compatibility || !AllowFloatGroupTransparency; }
		}
		[XtraSerializableProperty]
		public bool ShowFloatWindowsInTaskbar {
			get { return (bool)GetValue(ShowFloatWindowsInTaskbarProperty); }
			set { SetValue(ShowFloatWindowsInTaskbarProperty, value); }
		}
		[XtraSerializableProperty]
		public bool OwnsFloatWindows {
			get { return (bool)GetValue(OwnsFloatWindowsProperty); }
			set { SetValue(OwnsFloatWindowsProperty, value); }
		}
		[XtraSerializableProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool AllowAeroSnap {
			get { return (bool)GetValue(AllowAeroSnapProperty); }
			set { SetValue(AllowAeroSnapProperty, value); }
		}
		internal bool EnableNativeDragging {
			get { return AllowAeroSnap && GetRealFloatingMode() == Core.FloatingMode.Window && EnvironmentHelper.IsWinSevenOrLater; }
		}
		DisableFloatingPanelTransparencyBehavior DisableTransparencyBehavior {
			get {
				if(disableTransparencyBehavior == null)
					disableTransparencyBehavior = new DisableFloatingPanelTransparencyBehavior();
				return disableTransparencyBehavior;
			}
		}
		internal Win32AdornerWindowProvider Win32AdornerWindowProvider {
			get {
				if(adornerWindowProvider == null)
					adornerWindowProvider = new Win32AdornerWindowProvider();
				return adornerWindowProvider;
			}
		}
		internal Win32DragService Win32DragService {
			get {
				if(win32DragService == null)
					win32DragService = new Win32DragService();
				return win32DragService;
			}
		}
		string OwnerWindowTitle;
		Window topLevelWindow;
		Window TopLevelWindow {
			get { return topLevelWindow; }
			set {
				if(topLevelWindow == value) return;
				topLevelWindow = value;
				if(topLevelWindow != null)
					OnTopLevelWindowChanged();
			}
		}
		void OnTopLevelWindowChanged() {
			VisualizerAdornerHelper.EnsureAdornerActivated(DragAdorner);
		}
		void EnsureSystemEventsSubscriptions() {
			UnsubscribeSystemEvents();
			SubscribeSystemEvents();
		}
		void SubscribeSystemEvents() {
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnSystemEventsDisplaySettingsChanged;
		}
		void UnsubscribeSystemEvents() {
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= OnSystemEventsDisplaySettingsChanged;
		}
		void EnsureOwnerWindowSubscriptions() {
			if(OwnerWindow != null)
				UnSubscribeOwnerWindowEvents();
			OwnerWindow = Window.GetWindow(this);
			TopLevelWindow = LayoutHelper.FindRoot(this) as Window;
			if(OwnerWindow != null) {
				OwnerWindowTitle = OwnerWindow.Title;
				SubscribeOwnerWindowEvents();
			}
			if(VisualRoot != null)
				UnsubscribeVisualRootEvents();
			VisualRoot = OwnerWindow ?? LayoutHelper.GetTopLevelVisual(this);
			if(VisualRoot != null)
				SubscribeVisualRootEvents();
		}
		Locker windowTitleLocker = new Locker();
		int lockOwnerWindowTitleChanged;
		protected internal void SetMDIChildrenTitle(string title) {
			lockOwnerWindowTitleChanged++;
			if(OwnerWindow != null && OwnerWindow.GetBindingExpression(Window.TitleProperty) == null) {
				windowTitleLocker.LockOnce();
				OwnerWindow.Title = string.Format(WindowTitleFormat, OwnerWindowTitle, title);
			}
			lockOwnerWindowTitleChanged--;
		}
		protected internal void ResetMDIChildrenTitle() {
			lockOwnerWindowTitleChanged++;
			if(OwnerWindow != null) {
				if(windowTitleLocker && OwnerWindow.GetBindingExpression(Window.TitleProperty) == null) {
					OwnerWindow.Title = OwnerWindowTitle;
				}
			}
			windowTitleLocker.Unlock();
			lockOwnerWindowTitleChanged--;
		}
		protected virtual void OnOwnerWindowTitleChanged() {
			if(lockOwnerWindowTitleChanged > 0) return;
			lockOwnerWindowTitleChanged++;
			if(OwnerWindow != null)
				OwnerWindowTitle = OwnerWindow.Title;
			else OwnerWindowTitle = null;
			lockOwnerWindowTitleChanged--;
		}
		protected virtual string CoerceWindowTitleFormat(string format) {
			if(!string.IsNullOrEmpty(format)) return format;
			return DockLayoutManagerParameters.WindowTitleFormat;
		}
		protected virtual void RegisterInputBindings() {
			if(!InputBindings.Contains(CloseMDIItemCommandBinding))
				InputBindings.Add(CloseMDIItemCommandBinding);
		}
		protected virtual void UnRegisterInputBindings() {
			InputBindings.Remove(CloseMDIItemCommandBinding);
		}
		protected override void OnUnloaded() {
			if(OwnerWindow != null)
				UnSubscribeOwnerWindowEvents();
			if(VisualRoot != null)
				UnsubscribeVisualRootEvents();
			VisualRoot = null;
			UnRegisterInputBindings();
			UnsubscribeSystemEvents();
			base.OnUnloaded();
		}
		protected virtual IViewAdapter CreateViewAdapter() {
			return new DockLayoutManagerViewAdapter(this);
		}
		internal int isDockItemActivation = 0;
		internal int isLayoutItemActivation = 0;
		internal int isMDIItemActivation = 0;
		protected virtual void OnActiveDockItemChanged(BaseLayoutItem oldItem, BaseLayoutItem activeItem) {
			DockController.ActiveItem = activeItem;
			CheckCustomizationRoot(activeItem);
			SynchronizeWithCurrentItem(activeItem);
		}
		protected virtual void OnActiveLayoutItemChanged(BaseLayoutItem oldItem, BaseLayoutItem activeItem) {
			LayoutController.ActiveItem = activeItem;
			CheckCustomizationRoot(activeItem);
			SynchronizeWithCurrentItem(activeItem);
		}
		protected virtual void OnActiveMDIItemChanged(BaseLayoutItem oldItem, BaseLayoutItem activeItem) {
			MDIController.ActiveItem = activeItem;
			CheckCustomizationRoot(activeItem);
			SynchronizeWithCurrentItem(activeItem);
		}
		void CheckCustomizationRoot(BaseLayoutItem item) {
			CustomizationController controller = (CustomizationController)CustomizationController;
			if(controller.CustomizationRoot != item.GetRoot())
				controller.CoerceValue(DevExpress.Xpf.Docking.Customization.CustomizationController.CustomizationRootProperty);
		}
		protected virtual void OnAllowCustomizationChanged(bool allow) {
			if(!allow)
				CustomizationController.EndCustomization();
		}
		protected virtual void OnAllowDocumentSelectorChanged(bool allow) {
			if(!allow)
				CustomizationController.HideDocumentSelectorForm();
		}
		protected virtual void OnIsCustomizationChanged(bool newValue) {
			RenameHelper.CancelRenamingAndResetClickedState();
			ViewAdapter.ProcessAction(newValue ? ViewAction.ShowSelection : ViewAction.HideSelection);
			BaseLayoutItem[] items = this.GetItems();
			foreach(BaseLayoutItem item in items) {
				item.CoerceValue(BaseLayoutItem.VisibilityProperty);
				if(!IsInDesignTime) {
					if(item is LayoutGroup)
						item.CoerceValue(LayoutGroup.AllowSplittersProperty);
				}
				if(item is LayoutSplitter)
					item.CoerceValue(LayoutSplitter.IsEnabledProperty);
			}
			Update();
		}
		protected virtual object CoerceIsCustomization(bool baseValue) {
			return LayoutController.IsCustomization;
		}
		protected virtual object CoerceIsRenaming(bool newValue) {
			foreach(LayoutView view in ViewAdapter.Views) {
				RenameController controller = view.AdornerHelper.GetRenameController();
				if(controller != null && controller.IsRenamingStarted)
					return true;
			}
			return false;
		}
		protected virtual void OnShowInvisibleItemsChanged(bool? newValue) {
			UpdateItemsVisibility();
			RaiseEvent(new ShowInvisibleItemsChangedEventArgs(newValue) { Source = this });
		}
		protected virtual void OnShowInvisibleItemsInCustomizationFormChanged(bool value) {
			UpdateItemsVisibility();
		}
		protected virtual void OnClosedItemsBarVisibilityChanged(ClosedPanelsBarVisibility visibility) {
			CustomizationController.ClosedPanelsBarVisibility = visibility;
		}
		internal void ProcessKey(KeyEventArgs e) {
			LayoutView view = ViewAdapter.DragService.DragSource as LayoutView;
			if(view != null)
				view.RootUIElementKeyDown(this, e);
		}
		protected virtual void OwnerWindowPreviewKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Tab && ((Keyboard.Modifiers & ModifierKeys.Control) != 0)) {
				DependencyObject eventProcessor = LayoutHelper.FindLayoutOrVisualParentObject(e.OriginalSource as DependencyObject, (fe) => fe is DockLayoutManager && ((DockLayoutManager)fe).AllowDocumentSelector);
				if(CanProcessKey(eventProcessor)) {
					if(CanShowDocumentSelector()) {
						CustomizationController.ShowDocumentSelectorForm();
						e.Handled = eventProcessor == null && CustomizationController.IsDocumentSelectorVisible;
					}
				}
				return;
			}
			if(e.Key == Key.Enter) {
				if(RenameHelper.EndRenaming()) return;
			}
			if(e.Key == Key.Escape) {
				if(RenameHelper.CancelRenamingAndResetClickedState()) return;
				ExtendSelectionToParent();
				return;
			}
			if(e.Key == Key.Delete) {
				if(!IsRenaming)
					LayoutController.HideSelectedItems();
				return;
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Tab && ((Keyboard.Modifiers & ModifierKeys.Control) != 0))
				if(!AllowDocumentSelector) e.Handled = Navigate(!KeyHelper.IsShiftPressed);
			base.OnKeyDown(e);
		}
		protected virtual void OwnerWindowPreviewKeyUp(object sender, KeyEventArgs e) {
			if(KeyboardHelper.IsControlPressed) {
				if(CustomizationController.IsDocumentSelectorVisible)
					CustomizationController.HideDocumentSelectorForm();
				return;
			}
		}
		bool Navigate(bool forward) {
			DependencyObject focused = KeyboardFocusHelper.FocusedElement;
			BaseLayoutItem item = null;
			if(focused != null) {
				item = DockLayoutManager.GetLayoutItem(focused);
			}
			return this.ProcessPanelNavigation(item, forward);
		}
		bool CanProcessKey(DependencyObject eventProcessor) {
			return ViewAdapter.DragService.OperationType == OperationType.Regular && (eventProcessor == this || (eventProcessor == null && this.IsInVisualTree()));
		}
		bool CanShowDocumentSelector() {
			return AllowDocumentSelector && !CustomizationController.IsDocumentSelectorVisible;
		}
		protected void EnsureLayoutRoot() {
			if(LayoutRoot == null)
				LayoutRoot = CreateLayoutGroup();
		}
		protected internal void EnsureLogicalTree() {
			base.AddLogicalChild(DockHintsContainer);
			base.AddLogicalChild(InternalElementsContainer);
			Array.ForEach(this.GetItems(), (item) => {
				item.Manager = this;
			});
		}
		protected virtual void AddVisualChild(FrameworkElement child) {
			if(child.Parent == null && !visualChildrenCore.Contains(child)) {
				visualChildrenCore.Add(child);
			}
		}
		protected virtual void RemoveVisualChild(FrameworkElement child) {
			if (visualChildrenCore.Contains(child)) {
				visualChildrenCore.Remove(child);
			}
		}
		protected void SubscribeVisualRootEvents() {
			VisualRootPreviewKeyDownEventManager.AddListener(VisualRoot, this);
			VisualRootPreviewKeyUpEventManager.AddListener(VisualRoot, this);
		}
		protected void UnsubscribeVisualRootEvents() {
			VisualRootPreviewKeyDownEventManager.RemoveListener(VisualRoot, this);
			VisualRootPreviewKeyUpEventManager.RemoveListener(VisualRoot, this);
		}
		protected virtual void SubscribeOwnerWindowEvents() {
			BindingHelper.SetBinding(this, OwnerWindowTitleProperty, OwnerWindow, "Title");
			OwnerWindowClosedEventManager.AddListener(OwnerWindow, this);
			OwnerWindowBoundsChangedEventManager.AddListener(OwnerWindow, this);
			if(TopLevelWindow != null && TopLevelWindow != OwnerWindow)
				OwnerWindowBoundsChangedEventManager.AddListener(TopLevelWindow, this);
		}
		protected virtual void UnSubscribeOwnerWindowEvents() {
			ResetMDIChildrenTitle();
			BindingHelper.ClearBinding(this, OwnerWindowTitleProperty);
			OwnerWindowClosedEventManager.RemoveListener(OwnerWindow, this);
			OwnerWindowBoundsChangedEventManager.RemoveListener(OwnerWindow, this);
			OwnerWindow = null;
			if(TopLevelWindow != null) {
				OwnerWindowBoundsChangedEventManager.RemoveListener(TopLevelWindow, this);
				TopLevelWindow = null;
			}
		}
		Locker themeChangedLocker = new Locker();
		internal bool IsThemeChangedLocked {
			get { return themeChangedLocker.IsLocked; }
		}
		protected internal virtual void OnThemeChanged() {
			PrepareLayoutForModification();
			if(IsDesktopFloatingMode && (CanGetRestoreOffset() || effectiveRestoreOffset.HasValue)) {
				themeChangedLocker.Lock();
				Point restoreOffset = GetRestoreOffset();
				Dispatcher.BeginInvoke(new Action<IEnumerable<FloatGroup>, Point>((x, y) => OnThemeChangedAsync(x, y)), System.Windows.Threading.DispatcherPriority.Render, FloatGroups.ToArray(), restoreOffset);
			}
			UpdateFloatingPaneResources();
		}
		void OnThemeChangedAsync(IEnumerable<FloatGroup> affectedItems, Point prevOffset) {
			themeChangedLocker.Unlock();
			if(CanGetRestoreOffset()) {
				var newOffset = GetRestoreOffset();
				foreach(var fg in affectedItems) {
					fg.EnsureFloatLocation(prevOffset, newOffset);
				}
				SaveRestoreOffset();
			}
		}
		void PrepareItems() {
			if(SerializationController.IsDeserializing || TreeWalkerHelper.IsThemeChanging())
				Array.ForEach(this.GetItems(), (item) => item.PrepareForModification(SerializationController.IsDeserializing));
		}
		protected internal void PrepareLayoutForModification() {
			if(SerializationController.IsDeserializing)
				EnsureLayoutRoot();
			PrepareViews();
			PrepareItems();
		}
		void PrepareViews() {
			foreach(IView view in ViewAdapter.Views) {
				if(view.Type == HostType.AutoHide) {
					ViewAdapter.ProcessAction(view, ViewAction.Hide);
				}
				view.Invalidate();
			}
		}
		internal void OnLayoutRestored() {
			EnsureLogicalTree();
			UpdateClosedPanelsBar();
			Update();
			foreach(IView view in ViewAdapter.Views) {
				view.Invalidate();
			}
			Action checkFloatGroupRestoreBoundsAction = new Action(RequestCheckFloatGroupRestoreBounds);
			Window ownerWindow = Window.GetWindow(this);
			if(ScreenHelper.IsAttachedToPresentationSource(this) && IsLoaded) RestoreFloatingWindows();
			else loadedActionsHelper.AddDelayedAction(RestoreFloatingWindows);
			if(ownerWindow is DXWindow) {
				Dispatcher.BeginInvoke(checkFloatGroupRestoreBoundsAction);
			}
			else checkFloatGroupRestoreBoundsAction();
			Array.ForEach(this.GetItems(), (item) => item.UnlockLogicalTree());
			InvokeHelper.BeginInvoke(this, new Action(() => RestoreFloatGroupOrder()), InvokeHelper.Priority.Loaded);
		}
		private void RestoreFloatGroupOrder() {
			if(IsDisposing) return;
			var floatGroups = FloatGroups.OrderBy(x => x.GetSerializableZOrder());
			foreach(var fg in floatGroups) {
				this.BringToFront(fg);
			}
		}
		List<AutoHideTray> autoHideTrayCollection = new List<AutoHideTray>();
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Platform.HitTestHelper.ResetCache();
			autoHideTrayCollection.Clear();
			SetAutoHideTrayAffinity(SWC.Dock.Left, ref PartLeftAutoHideTray, ref PartLeftAutoHidePane);
			SetAutoHideTrayAffinity(SWC.Dock.Right, ref PartRightAutoHideTray, ref PartRightAutoHidePane);
			SetAutoHideTrayAffinity(SWC.Dock.Top, ref PartTopAutoHideTray, ref PartTopAutoHidePane);
			SetAutoHideTrayAffinity(SWC.Dock.Bottom, ref PartBottomAutoHideTray, ref PartBottomAutoHidePane);
			SetClosedItemsPanelAffinity();
			SetLayoutLayerAffinity();
			SetFloatingLayerAffinity();
			EnsureCustomization();
			if(IsInDesignTime) {
				EnsureLogicalTree();
			}
		}
		WeakReference walker;
		void EnsureCustomization() {
			string theme = null;
			if(walker != null) {
				ThemeTreeWalker w = walker.Target as ThemeTreeWalker;
				if(w != null)
					theme = w.ThemeName;
			}
			ThemeTreeWalker current = ThemeManager.GetTreeWalker(this);
			if(current != null && current.ThemeName != theme) {
				if(walker != null)
					EndCustomization();
				walker = new WeakReference(current);
			}
			else walker = null;
		}
		protected virtual void UpdateFloatingPaneResources() {
			DisableTransparencyBehavior.Detach();
			if(EnableWin32Compatibility || !AllowFloatGroupTransparency)
				DisableTransparencyBehavior.Attach(this);
		}
		protected internal ClosedItemsPanel ClosedItemsPanel { get; private set; }
		protected AutoHideTray PartLeftAutoHideTray;
		protected AutoHideTray PartTopAutoHideTray;
		protected AutoHideTray PartRightAutoHideTray;
		protected AutoHideTray PartBottomAutoHideTray;
		protected AutoHidePane PartLeftAutoHidePane;
		protected AutoHidePane PartTopAutoHidePane;
		protected AutoHidePane PartRightAutoHidePane;
		protected AutoHidePane PartBottomAutoHidePane;
		void SetClosedItemsPanelAffinity() {
			if(ClosedItemsPanel != null && !LayoutItemsHelper.IsTemplateChild(ClosedItemsPanel, this))
				ClosedItemsPanel.Dispose();
			ClosedItemsPanel = GetTemplateChild("PART_ClosedItemsPanel") as ClosedItemsPanel;
		}
		protected internal DXContentPresenter LayoutLayer { get; private set; }
		protected internal LayoutItemsControl FloatingLayer { get; private set; }
		protected internal FrameworkElement AutoHideLayer { get; private set; }
		void SetLayoutLayerAffinity() {
			if(LayoutLayer != null && !LayoutItemsHelper.IsTemplateChild(LayoutLayer, this)) {
				DisposeLayoutLayer();
			}
			LayoutLayer = GetTemplateChild("LayoutLayer") as DXContentPresenter;
			if(LayoutRoot != null) LayoutRoot.SelectTemplate();
		}
		void SetFloatingLayerAffinity() {
			if(FloatingLayer != null && !LayoutItemsHelper.IsTemplateChild(FloatingLayer, this)) {
				FloatingLayer.Dispose();
			}
			FloatingLayer = GetTemplateChild("FloatingLayer") as LayoutItemsControl;
		}		
		void SetAutoHideTrayAffinity(SWC.Dock dock, ref AutoHideTray partAutoHideTray, ref AutoHidePane partTrayPanel) {
			string side = dock.ToString();
			if(partAutoHideTray != null && !LayoutItemsHelper.IsTemplateChild(partAutoHideTray, this)) {
				partAutoHideTray.Dispose();
			}
			if(partTrayPanel != null && !LayoutItemsHelper.IsTemplateChild(partTrayPanel, this)) {
				partTrayPanel.Dispose();
			}
			partAutoHideTray = GetTemplateChild("PART_" + side + "AutoHideTray") as AutoHideTray;
			if(partAutoHideTray != null) {
				partTrayPanel = GetTemplateChild("PART_" + side + "AutoHideTrayPanel") as AutoHidePane;
				if(partTrayPanel != null) {
					SetAffinity(partAutoHideTray, partTrayPanel, dock);
				}
				autoHideTrayCollection.Add(partAutoHideTray);
			}
			AutoHideLayer = GetTemplateChild("AutoHideLayer") as FrameworkElement;
		}
		void SetAffinity(AutoHideTray tray, AutoHidePane panel, SWC.Dock dock) {
			tray.DockType = dock;
			panel.DockType = dock;
			tray.EnsureAutoHidePanel(panel);
		}
		protected void OwnerWindowBoundsChanged() {
			FloatingWindowsReposition();
			SaveRestoreOffset();
		}
		protected internal bool ShouldRestoreOnActivate { get; set; }
		protected bool ShouldRestoreOnIsVisibleChanged { get; set; }
		protected FloatGroup[] GetFloatGroups(bool getSorted) {
			FloatGroup[] groups = new FloatGroup[FloatGroups.Count];
			if(groups.Length > 0) {
				FloatGroups.CopyTo(groups, 0);
				if(getSorted)
					Array.Sort(groups, this.GetFloatGroupComparison());
			}
			return groups;
		}
#if DEBUGTEST
		internal bool TestIsVisible = true;
#endif
		protected internal virtual void RestoreFloatingWindows() {
#if DEBUGTEST
			if(!TestIsVisible) return;
#else
			if(!IsVisible) return;
#endif
			if(ShouldRestoreOnActivate || ShouldRestoreOnIsVisibleChanged) {
				FloatGroup[] groups = GetFloatGroups(true);
				for(int i = 0; i < groups.Length; i++) {
					FloatGroup floatGroup = groups[i];
					if(floatGroup.ShouldRestoreOnActivate) {
						floatGroup.ShouldRestoreOnActivate = false;
						if(!floatGroup.IsOpen) {
							floatGroup.IsOpen = true;
						}
					}
				}
				Array.Reverse(groups);
				for(int i = 0; i < groups.Length; i++) {
					IView view = this.GetView(groups[i]);
				}
			}
		}
		protected internal virtual void HideFloatingWindows() {
			CustomizationController.CloseMenu();
			foreach(FloatGroup floatGroup in FloatGroups) {
				if(floatGroup.IsOpen) {
					floatGroup.ShouldRestoreOnActivate = true;
					if(floatGroup.IsOpen) floatGroup.IsOpen = false;
				}
			}
		}
		protected void ResetAdorners() {
			InvokeHelper.BeginInvoke(this,
				new Action(() => { if(!this.IsVisible() && DragAdorner.IsActivated) DragAdorner.Deactivate(); }),
				InvokeHelper.Priority.Normal, null);
			foreach(LayoutView view in ViewAdapter.Views) {
				if(view != null && view.IsAdornerHelperInitialized)
					view.AdornerHelper.Reset();
			}
		}
		protected virtual void FloatingWindowsReposition() {
			if(IsDesktopFloatingMode) return;
			foreach(FloatGroup floatGroup in FloatGroups) {
				if(floatGroup.IsOpen) {
					FloatPanePresenter floatPresenter = floatGroup.UIElements.GetElement<FloatPanePresenter>();
					if(floatPresenter != null) floatPresenter.CheckBoundsInContainer();
				}
				floatGroup.UpdateMaximizedBounds();
			}
		}
		public bool BringToFront(BaseLayoutItem item) {
			FloatGroup fGroup = (item != null) ? item.GetRoot() as FloatGroup : null;
			if(fGroup == null || !fGroup.IsOpen) return false;
			if(fGroup.UIElements.Count == 0)
				UpdateLayout();
			IView view = this.GetView(fGroup.UIElements.GetElement<FloatPanePresenter>());
			if(view != null)
				ViewAdapter.UIInteractionService.Activate(view);
			return view != null;
		}
		public void Activate(BaseLayoutItem item) {
			ActivateCore(item);
		}
		protected internal LayoutGroup ActivateCore(BaseLayoutItem itemToActivate, bool focus) {
			if(itemToActivate == null) return null;
			LayoutGroup activationRoot = itemToActivate.GetRoot();
			bool fIsLayoutItem = LayoutItemsHelper.IsLayoutItem(itemToActivate) || itemToActivate.ItemType == LayoutItemType.Group;
			if(fIsLayoutItem && (activationRoot != null) && activationRoot.IsLayoutRoot) {
				if(activationRoot.ParentPanel != null) {
					DocumentPanel document = GetDocument(activationRoot.ParentPanel);
					if((document == null) || TryActivate(MDIController, document, false)) {
						if(!TryActivate(DockController, activationRoot.ParentPanel, false)) {
							return activationRoot;
						}
					}
				}
				LayoutController.Activate(itemToActivate, focus);
			}
			else if(!(itemToActivate is AutoHideGroup)) {
				DocumentPanel document = GetDocument(itemToActivate);
				if((document == null) || TryActivate(MDIController, document, false))
					DockController.Activate(itemToActivate, focus);
			}
			return activationRoot;
		}
		protected internal LayoutGroup ActivateCore(BaseLayoutItem itemToActivate) {
			return ActivateCore(itemToActivate, true);
		}
		bool TryActivate(IActiveItemOwner controller, BaseLayoutItem item, bool focus = true) {
			controller.Activate(item, focus);
			return controller.ActiveItem == item;
		}
		static DocumentPanel GetDocument(BaseLayoutItem item) {
			DocumentPanel document = item as DocumentPanel;
			if(item is DocumentGroup)
				document = ((DocumentGroup)item).SelectedItem as DocumentPanel;
			if(item is FloatGroup && ((FloatGroup)item).HasItems)
				document = ((FloatGroup)item)[0] as DocumentPanel;
			return document != null && document.IsMDIChild ? document : null;
		}
		public bool Collapse(bool immediately) {
			bool result = false;
			foreach(IView view in ViewAdapter.Views) {
				if(view.Type != HostType.AutoHide) continue;
				ViewAdapter.ActionService.Hide(view, immediately);
				result = true;
			}
			return result;
		}
		protected internal bool CanAutoHideOnMouseDown {
			get { return AutoHideExpandMode == Base.AutoHideExpandMode.MouseDown || AutoHideMode == Base.AutoHideMode.Inline; }
		}
		public void ShowItemSelectorMenu(UIElement source, BaseLayoutItem[] items) {
			CustomizationController.ShowItemSelectorMenu(source, items);
		}
		public void ShowContextMenu(BaseLayoutItem item) {
			CustomizationController.ShowContextMenu(item);
		}
		public void CloseMenu() {
			CustomizationController.CloseMenu();
		}
		public bool SelectItem(BaseLayoutItem item) {
			return SelectItem(item, Layout.Core.SelectionMode.SingleItem);
		}
		public bool SelectItem(BaseLayoutItem item, Layout.Core.SelectionMode mode) {
			if(IsDisposing || !IsCustomization || item == null) return false;
			LayoutGroup root = item.GetRoot();
			IView view = this.GetView(root);
			bool result = false;
			if(view != null) {
				IUIElement element = item;
				if(element != null) {
					ILayoutElement viewElement = view.GetElement(element);
					ViewAdapter.SelectionService.Select(view, viewElement, mode);
				}
				ActiveLayoutItem = item;
				CustomizationController.CustomizationRoot = root;
				result = true;
			}
			return result;
		}
		public bool ExtendSelectionToParent() {
			if(IsDisposing || !IsCustomization || ActiveLayoutItem == null) return false;
			IView view = this.GetView(ActiveLayoutItem.GetRoot());
			return ViewAdapter.SelectionService.ExtendSelectionToParent(view);
		}
		int activationLockCount;
		internal bool IsActivationLocked { get { return activationLockCount > 0; } }
		internal void LockActivation() {
			activationLockCount++;
			if(DockController is ILockOwner)
				((ILockOwner)DockController).Lock();
			if(MDIController is ILockOwner)
				((ILockOwner)MDIController).Lock();
		}
		internal void UnlockActivation() {
			if(DockController is ILockOwner)
				((ILockOwner)DockController).Unlock();
			if(MDIController is ILockOwner)
				((ILockOwner)MDIController).Unlock();
			activationLockCount--;
		}
		protected internal event PropertyChangedEventHandler MDIMergeStyleChanged = delegate { };
		protected internal event PropertyChangedEventHandler AutoHideDisplayModeChanged = delegate { };
		protected void RaiseAutoHideDisplayModeChanged() {
			AutoHideDisplayModeChanged(this, new PropertyChangedEventArgs("AutoHideDisplayModeChanged"));
		}
		protected void RaiseMDIStyleChanged() {
			if(MDIMergeStyleChanged != null)
				MDIMergeStyleChanged(this, new PropertyChangedEventArgs("MDIMergeStyle"));
		}
		protected virtual void OnMDIMergeStyleChanged(MDIMergeStyle oldValue, MDIMergeStyle newValue) {
			RaiseMDIStyleChanged();
		}
		Locker closedPanelsLocker = new Locker();
		internal bool IsClosedPanelsVisibilityLocked{
			get {
				return closedPanelsLocker.IsLocked;
			}
		}
		internal void LockClosedPanelsVisibility(){
			closedPanelsLocker.Lock();
		}
		internal void UnlockClosedPanelsVisibility() {
			closedPanelsLocker.Unlock();
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerIsCustomizationFormVisible")]
#endif
		public bool IsCustomizationFormVisible {
			get { return CustomizationController.IsCustomizationFormVisible; }
		}
		public void ShowCustomizationForm() {
			CustomizationController.ShowCustomizationForm();
		}
		public void HideCustomizationForm() {
			CustomizationController.HideCustomizationForm();
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerClosedPanelsBarVisibility"),
#endif
		XtraSerializableProperty]
		public ClosedPanelsBarVisibility ClosedPanelsBarVisibility {
			get { return (ClosedPanelsBarVisibility)GetValue(ClosedPanelsBarVisibilityProperty); }
			set { SetValue(ClosedPanelsBarVisibilityProperty, value); }
		}
		bool _ManualClosedPanelsBarVisibility;
		[XtraSerializableProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool ManualClosedPanelsBarVisibility {
			get { return CustomizationController.ClosedPanelsVisibility; }
			set { _ManualClosedPanelsBarVisibility = value; }
		}
		internal bool GetClosedPanelsVisibility() { return _ManualClosedPanelsBarVisibility; }
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerFloatingMode"),
#endif
		XtraSerializableProperty]
		public FloatingMode FloatingMode {
			get { return (FloatingMode)GetValue(FloatingModeProperty); }
			set { SetValue(FloatingModeProperty, value); }
		}
		protected internal bool IsDesktopFloatingMode { get { return GetRealFloatingMode() == Core.FloatingMode.Window; } }
		protected internal DevExpress.Xpf.Core.FloatingMode GetRealFloatingMode() {
			return WindowHelper.IsXBAP || DesignerProperties.GetIsInDesignMode(this) ?
				 DevExpress.Xpf.Core.FloatingMode.Adorner : (DevExpress.Xpf.Core.FloatingMode)(int)FloatingMode;
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerActiveDockItem")]
#endif
		public BaseLayoutItem ActiveDockItem {
			get { return (BaseLayoutItem)GetValue(ActiveDockItemProperty); }
			set { SetValue(ActiveDockItemProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerActiveLayoutItem")]
#endif
		public BaseLayoutItem ActiveLayoutItem {
			get { return (BaseLayoutItem)GetValue(ActiveLayoutItemProperty); }
			set { SetValue(ActiveLayoutItemProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerActiveMDIItem")]
#endif
		public BaseLayoutItem ActiveMDIItem {
			get { return (BaseLayoutItem)GetValue(ActiveMDIItemProperty); }
			set { SetValue(ActiveMDIItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAllowCustomization"),
#endif
 XtraSerializableProperty]
		public bool AllowCustomization {
			get { return (bool)GetValue(AllowCustomizationProperty); }
			set { SetValue(AllowCustomizationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAllowDocumentSelector"),
#endif
 XtraSerializableProperty]
		public bool AllowDocumentSelector {
			get { return (bool)GetValue(AllowDocumentSelectorProperty); }
			set { SetValue(AllowDocumentSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerIsCustomization")]
#endif
		public bool IsCustomization {
			get { return (bool)GetValue(IsCustomizationProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAllowDockItemRename")]
#endif
		public bool? AllowDockItemRename {
			get { return (bool?)GetValue(AllowDockItemRenameProperty); }
			set { SetValue(AllowDockItemRenameProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAllowLayoutItemRename")]
#endif
		public bool? AllowLayoutItemRename {
			get { return (bool?)GetValue(AllowLayoutItemRenameProperty); }
			set { SetValue(AllowLayoutItemRenameProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerIsRenaming")]
#endif
		public bool IsRenaming {
			get { return (bool)GetValue(IsRenamingProperty); }
		}
		public bool Rename(BaseLayoutItem item) {
			return RenameHelper.Rename(item);
		}
		public void BeginCustomization() {
			CustomizationController.BeginCustomization();
		}
		public void EndCustomization() {
			CustomizationController.EndCustomization();
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutRoot")]
#endif
		public LayoutGroup LayoutRoot {
			get { return (LayoutGroup)GetValue(LayoutRootProperty); }
			set { SetValue(LayoutRootProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerFloatGroups"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FloatGroupCollection FloatGroups {
			get { return floatGroupsCore; }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAutoHideGroups"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AutoHideGroupCollection AutoHideGroups {
			get { return autoHideGroupsCore; }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerClosedPanels"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ClosedPanelCollection ClosedPanels {
			get { return closedPanelsCore; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HiddenItemsCollection HiddenItems {
			get { return LayoutController.HiddenItems; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupCollection DecomposedItems {
			get { return decomposedItems; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDefaultTabPageCaptionImage")]
#endif
		public ImageSource DefaultTabPageCaptionImage {
			get { return (ImageSource)GetValue(DefaultTabPageCaptionImageProperty); }
			set { SetValue(DefaultTabPageCaptionImageProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDefaultAutoHidePanelCaptionImage")]
#endif
		public ImageSource DefaultAutoHidePanelCaptionImage {
			get { return (ImageSource)GetValue(DefaultAutoHidePanelCaptionImageProperty); }
			set { SetValue(DefaultAutoHidePanelCaptionImageProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false)]
		public SerializableItemCollection Items {
			get { return SerializationController.Items; }
			set { SerializationController.Items = value; }
		}
		protected internal IViewAdapter ViewAdapter {
			get { return viewAdapterCore; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockController")]
#endif
		public IDockController DockController {
			get { return dockControllerCore; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutController")]
#endif
		public ILayoutController LayoutController {
			get { return layoutControllerCore; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerMDIController")]
#endif
		public IMDIController MDIController {
			get { return mdiControllerCore; }
		}
		protected internal RenameHelper RenameHelper {
			get { return renameHelperCore; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowInvisibleItems")]
#endif
		public bool? ShowInvisibleItems {
			get { return (bool?)GetValue(ShowInvisibleItemsProperty); }
			set { SetValue(ShowInvisibleItemsProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowInvisibleItemsInCustomizationForm")]
#endif
		public bool ShowInvisibleItemsInCustomizationForm {
			get { return (bool)GetValue(ShowInvisibleItemsInCustomizationFormProperty); }
			set { SetValue(ShowInvisibleItemsInCustomizationFormProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public bool DestroyLastDocumentGroup {
			get { return (bool)GetValue(DestroyLastDocumentGroupProperty); }
			set { SetValue(DestroyLastDocumentGroupProperty, value); }
		}
		public CoreDock ClosedPanelsBarPosition {
			get { return (CoreDock)GetValue(ClosedPanelsBarPositionProperty); }
			set { SetValue(ClosedPanelsBarPositionProperty, value); }
		}
		internal bool IsInDesignTime {
			get { return DesignerProperties.GetIsInDesignMode(this); }
		}
		internal bool AllowSelection {
			get { return IsCustomization && !IsInDesignTime; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerClosingBehavior")]
#endif
		public ClosingBehavior ClosingBehavior { get; set; }
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerAutoHideExpandMode")]
#endif
		public AutoHideExpandMode AutoHideExpandMode {
			get { return (AutoHideExpandMode)GetValue(AutoHideExpandModeProperty); }
			set { SetValue(AutoHideExpandModeProperty, value); }
		}
		public AutoHideMode AutoHideMode {
			get { return (AutoHideMode)GetValue(AutoHideModeProperty); }
			set { SetValue(AutoHideModeProperty, value); }
		}
		public bool AllowMergingAutoHidePanels {
			get { return (bool)GetValue(AllowMergingAutoHidePanelsProperty); }
			set { SetValue(AllowMergingAutoHidePanelsProperty, value); }
		}
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DockLayoutManagerIsSynchronizedWithCurrentItem"),
#endif
		XtraSerializableProperty]
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}
		[XtraSerializableProperty]
		public MDIMergeStyle MDIMergeStyle {
			get { return (MDIMergeStyle)GetValue(MDIMergeStyleProperty); }
			set { SetValue(MDIMergeStyleProperty, value); }
		}
		[XtraSerializableProperty]
		public bool RedrawContentWhenResizing {
			get { return (bool)GetValue(RedrawContentWhenResizingProperty); }
			set { SetValue(RedrawContentWhenResizingProperty, value); }
		}
		[XtraSerializableProperty]
		public DockingStyle DockingStyle {
			get { return (DockingStyle)GetValue(DockingStyleProperty); }
			set { SetValue(DockingStyleProperty, value); }
		}
		[XtraSerializableProperty]
		public FloatingDocumentContainer FloatingDocumentContainer {
			get { return (FloatingDocumentContainer)GetValue(FloatingDocumentContainerProperty); }
			set { SetValue(FloatingDocumentContainerProperty, value); }
		}
		ThemeTreeWalkerHelper TreeWalkerHelper {
			get {
				if(treeWalkerHelper == null)
					treeWalkerHelper = new ThemeTreeWalkerHelper(this);
				return treeWalkerHelper;
			}
		}
		protected internal ICustomizationController CustomizationController {
			get { return customizationControllerCore; }
		}
		protected internal ISerializationController SerializationController {
			get { return serializationControllerCore; }
		}
		protected internal DragAdorner DragAdorner { get; private set; }
		protected virtual RenameHelper CreateRenameHelper() {
			return new RenameHelper(this);
		}
		protected virtual IDockController CreateDockController() {
			return new DockController(this);
		}
		protected virtual IMDIController CreateMDIController() {
			return new MDIController(this);
		}
		protected virtual ILayoutController CreateLayoutController() {
			return new LayoutController(this);
		}
		protected virtual ISerializationController CreateSerializationController() {
			return new SerializationController(this);
		}
		protected virtual ICustomizationController CreateCustomizationController() {
			return new CustomizationController(this);
		}
		int floatingCounter = 0;
		protected internal void BeginFloating() {
			floatingCounter++;
		}
		protected internal void EndFloating() {
			floatingCounter--;
		}
		protected internal bool IsFloating {
			get { return floatingCounter > 0; }
		}
		internal bool IsDragging {
			get { return !IsDisposing && ViewAdapter.DragService.DragItem != null; }
		}
		int lockUpdateCounter;
		int updatesCount;
		protected bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		protected void LockUpdate() {
			if(!IsUpdateLocked)
				updatesCount = 0;
			lockUpdateCounter++;
		}
		protected void UnlockUpdate() {
			if(--lockUpdateCounter == 0) {
				if(updatesCount > 0) Update();
			}
		}
		protected virtual void BeginUpdate() {
		}
		protected virtual void EndUpdate() {
		}
		protected internal void Update(bool sholdUpdateLayout) {
			updatesCount++;
			Platform.HitTestHelper.ResetCache();
			if(!IsInitialized) return;
			if(IsUpdateLocked || SerializationController.IsDeserializing) return;
			lockUpdateCounter++;
			try {
				BeginUpdate();
				if(sholdUpdateLayout)
					UpdateLayout();
				this.InvalidateView(LayoutRoot);
				using(var rootEnumerator = new IUIElementEnumerator(this)) {
					while(rootEnumerator.MoveNext()) {
						FloatPanePresenter presenter = rootEnumerator.Current as FloatPanePresenter;
						LayoutGroup pane = rootEnumerator.Current as LayoutGroup;
						if(pane != null && pane.ItemType == LayoutItemType.Group)
							UpdateGroupPane(pane, sholdUpdateLayout);
						if(presenter != null)
							UpdateGroupPane(presenter.Element, sholdUpdateLayout);
					}
				}
				if(LayoutLayer != null && LayoutLayer.Parent != null)
					((UIElement)LayoutLayer.Parent).InvalidateMeasure();
				CheckCustomizationRoot(LayoutRoot);
				UpdateSelection();
				UpdateActiveItems();
				DecomposedItems.Purge();
				PurgeLogicalChildren();
			}
			finally {
				lockUpdateCounter--;
				updatesCount = 0;
				EndUpdate();
			}
		}
		void UpdateActiveItems() {
			var items = this.GetItems();
			var dockItem = ActiveDockItem;
			if(dockItem != null && !items.Contains(dockItem)) this.Deactivate(dockItem);
			var mdiItem = ActiveMDIItem;
			if(mdiItem != null && !items.Contains(mdiItem)) this.Deactivate(mdiItem);
			var layoutItem = ActiveLayoutItem;
			if(layoutItem != null && !items.Contains(layoutItem)) this.Deactivate(layoutItem);
		}
		protected internal void Update() {
			Update(true);
		}
		internal static void UpdateGroupPane(DependencyObject pane, bool shouldUpdateLayout = true) {
			IEnumerator ve = new VisualTreeEnumerator(pane);
			while(ve.MoveNext()) {
				GroupPanel groupPanel = ve.Current as GroupPanel;
				if(groupPanel != null) {
					groupPanel.RebuildLayout();
					break;
				}
			}
		}
		void UpdateSelection() {
			if(ViewAdapter == null || !IsCustomization) return;
			ViewAdapter.ProcessAction(ViewAction.ShowSelection);
		}
		void UpdateItemsVisibility() {
			BaseLayoutItem[] items = this.GetItems();
			foreach(BaseLayoutItem item in items) {
				item.CoerceValue(BaseLayoutItem.VisibilityProperty);
			}
		}
		internal void RegisterViewIfNeeded(IUIElement element) {
			if(IsDisposing) return;
			IView existingView = ViewAdapter.GetView(element);
			if(existingView != null) return;
			RegisterView(element);
		}
		void RegisterView(IUIElement element) {
			if(IsDisposing) return;
			IView existingView = ViewAdapter.GetView(element);
			if(existingView != null) {
				ViewAdapter.Views.Remove(existingView);
				Ref.Dispose(ref existingView);
			}
			IView view = CreateView(element);
			if(view != null)
				ViewAdapter.Views.Add(view);
		}
		internal void UnRegisterView(IUIElement element) {
			IView view = (ViewAdapter != null) ? ViewAdapter.GetView(element) : null;
			if(view != null) {
				ViewAdapter.Views.Remove(view);
				Ref.Dispose(ref view);
			}
		}
		internal void ResetView(IUIElement element) {
			if(IsDisposing) return;
			IUIElement viewElement = element.GetRootUIScope();
			IView view = (ViewAdapter != null) ? ViewAdapter.GetView(viewElement) : null;
			if(view != null) view.Invalidate();
		}
		bool IsViewCreated(IUIElement element) {
			return ViewAdapter.GetView(element) != null;
		}
		IView CreateView(IUIElement element) {
			if(element is AutoHideTray)
				return CreateAutoHideView(element);
			if(element is FloatPanePresenter)
				return CreateFloatingView(element);
			if((element is LayoutGroup && ((LayoutGroup)element).ItemType == LayoutItemType.Group) || element is GroupPane)
				return CreateLayoutView(element);
			if(element is CustomizationControl)
				return CreateCustomizationView(element);
			return null;
		}
		protected virtual IView CreateLayoutView(IUIElement element) {
			return new LayoutView(element);
		}
		protected virtual IView CreateFloatingView(IUIElement element) {
			return new FloatingView(element);
		}
		protected virtual IView CreateAutoHideView(IUIElement element) {
			return new AutoHideView(element);
		}
		protected virtual IView CreateCustomizationView(IUIElement element) {
			return new CustomizationView(element);
		}
		protected virtual DragAdorner CreateDragAdorner() {
			return new DragAdorner(this);
		}
		protected virtual FloatGroupCollection CreateFloatGroupsCollection() {
			return new FloatGroupCollection();
		}
		protected virtual AutoHideGroupCollection CreateAutoHideGroupsCollection() {
			return new AutoHideGroupCollection();
		}
		protected virtual ClosedPanelCollection CreateClosedPanelsCollection() {
			return new ClosedPanelCollection(this);
		}
		protected internal virtual LayoutControlItem CreateLayoutControlItem() {
			return new LayoutControlItem();
		}
		protected internal virtual LayoutSplitter CreateLayoutSplitter() {
			return new LayoutSplitter();
		}
		protected internal virtual SeparatorItem CreateSeparatorItem() {
			return new SeparatorItem();
		}
		protected internal virtual LabelItem CreateLabelItem() {
			return new LabelItem();
		}
		protected internal virtual EmptySpaceItem CreateEmptySpaceItem() {
			return new EmptySpaceItem();
		}
		protected internal virtual LayoutPanel CreateLayoutPanel() {
			return new LayoutPanel();
		}
		protected internal virtual LayoutGroup CreateLayoutGroup() {
			return new LayoutGroup();
		}
		protected internal virtual LayoutGroup CreateLayoutGroup(Orientation orientation) {
			LayoutGroup group = CreateLayoutGroup();
			((ISupportInitialize)group).BeginInit();
			group.Orientation = orientation;
			((ISupportInitialize)group).EndInit();
			return group;
		}
		protected internal virtual FloatGroup CreateFloatGroup() {
			return new FloatGroup();
		}
		protected internal virtual AutoHideGroup CreateAutoHideGroup() {
			return new AutoHideGroup();
		}
		protected internal virtual TabbedGroup CreateTabbedGroup() {
			return new TabbedGroup();
		}
		protected internal virtual DocumentGroup CreateDocumentGroup() {
			return new DocumentGroup();
		}
		protected internal virtual DocumentPanel CreateDocumentPanel() {
			return new DocumentPanel();
		}
		internal bool CheckAutoHideExpandState(LayoutPanel panel) {
			if(panel == null || !panel.IsAutoHidden || IsDisposing) return false;
			AutoHideTray tray = ((IUIElement)panel.Parent).GetRootUIScope() as AutoHideTray;
			if(tray == null) return false;
			switch(panel.AutoHideExpandState) {
				case Base.AutoHideExpandState.Visible:
					tray.DoRestore(panel);
					break;
				case Base.AutoHideExpandState.Hidden:
					if(tray.HotItem == panel)
						tray.DoCollapseIfPossible(true);
					break;
				case Base.AutoHideExpandState.Expanded:
					tray.DoMaximize(panel);
					break;
			}
			return true;
		}
		internal void CollapseOtherViews(BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			if(panel == null || !panel.IsAutoHidden || IsDisposing) return;
			AutoHideTray currentView = ((IUIElement)panel.Parent).GetRootUIScope() as AutoHideTray;
			if(currentView != null) {
				foreach(var view in autoHideTrayCollection) {
					if(currentView == view) continue;
					view.DoCollapse();
				}
			}
		}
		internal void CheckClosedState(BaseLayoutItem item) {
			CheckLayoutItemState(item, LayoutItemState.Close);
		}
		internal void CheckAutoHiddenState(BaseLayoutItem item) {
			CheckLayoutItemState(item, LayoutItemState.AutoHide);
		}
		void CheckLayoutItemState(BaseLayoutItem item, LayoutItemState state){
			if(IsDisposing || item == null) return;
			layoutItemStateHelper.QueueCheckLayoutItemState(item, state);
		}
		internal void UpdateClosedPanelsBar() {
			if(IsDisposing) return;
			if(ClosedPanelsBarVisibility == ClosedPanelsBarVisibility.Manual)
				CustomizationController.ClosedPanelsVisibility = GetClosedPanelsVisibility();
		}
		Point GetRestoreOffsetCore() {
			return Platform.WindowHelper.GetScreenLocation(this);
		}
		Point CalcRestoreOffset() {
			return CalcRestoreOffset(RestoreLayoutOptions.GetFloatPanelsRestoreOffset(this));
		}
		Point CalcRestoreOffset(Point savedOffset) {
			Point offset = new Point(0, 0);
			if(IsDesktopFloatingMode) {
				if(!double.IsNaN(savedOffset.X) && !double.IsNaN(savedOffset.Y)) {
					Point currentOffset = GetRestoreOffsetCore();
					offset = new Point(savedOffset.X - currentOffset.X, savedOffset.Y - currentOffset.Y);
				}
			}
			return offset;
		}
		Point CheckFloatGroupRestoreBounds(Rect restorebounds, Point offset) {
			Point result = restorebounds.Location();
			if(!Platform.WindowHelper.IsXBAP) {
				Rect bounds = restorebounds;
				int direction = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
				bounds = new Rect(bounds.X + direction * offset.X, bounds.Y + offset.Y, bounds.Width, bounds.Height);
				bounds = Platform.WindowHelper.CheckScreenBounds(this, bounds);
				result = new Point(bounds.Left, bounds.Top);
			}
			return result;
		}
		void CheckFloatGroupRestoreBounds() {
			Point restoreOffset = CalcRestoreOffset();
			foreach(FloatGroup fGroup in FloatGroups) {
				fGroup.With(x => x.FloatingWindowLock).Do(x => x.Lock(LK.CheckFloatBounds));
				try {
					Point checkPoint = CheckFloatGroupRestoreBounds(fGroup.FloatBounds, restoreOffset);
					if(fGroup.FloatLocation == checkPoint) fGroup.FloatLocation = new Point(fGroup.FloatLocation.X + 1, fGroup.FloatLocation.Y); ; 
					fGroup.FloatLocation = checkPoint;
				}
				finally {
					fGroup.With(x => x.FloatingWindowLock).Do(x => x.Unlock(LK.CheckFloatBounds));
				}
				Rect bounds = DocumentPanel.GetRestoreBounds(fGroup);
				Point restoreLocation = CheckFloatGroupRestoreBounds(bounds, restoreOffset);
				DocumentPanel.SetRestoreBounds(fGroup, new Rect(restoreLocation, bounds.Size()));
				fGroup.MaximizationLockHelper.Unlock();
			}
		}
		internal void CheckFloatGroupRestoreBounds(FloatGroup floatGroup, Point savedOffset) {
			Point offset = CalcRestoreOffset(savedOffset);
			floatGroup.FloatLocation = CheckFloatGroupRestoreBounds(floatGroup.FloatBounds, offset);
		}
		protected virtual void AddDelayedAction(Action action) {
			AddDelayedAction(action, DelayedActionPriority.Default);
		}
		void AddDelayedAction(Action action, DelayedActionPriority priority = DelayedActionPriority.Default) {
			if(IsLoaded && ScreenHelper.IsAttachedToPresentationSource(this))
				action();
			else
				loadedActionsHelper.AddDelayedAction(action, priority);
		}
		protected internal virtual void RequestCheckFloatGroupRestoreBounds() {
			if(IsDisposing) return;
			FloatGroups.Accept((x) => x.MaximizationLockHelper.Lock());
			Window ownerWindow = Window.GetWindow(this);
			AddDelayedAction(CheckFloatGroupRestoreBounds, ownerWindow is DevExpress.Xpf.Ribbon.IRibbonWindow ? DelayedActionPriority.Delayed : DelayedActionPriority.Default);
		}
		internal Point GetRestoreOffset() {
			return !CanGetRestoreOffset() ? SavedRestoreOffset : GetRestoreOffsetCore();
		}
		Point? effectiveRestoreOffset;
		Point SavedRestoreOffset {
			get { return effectiveRestoreOffset.HasValue ? effectiveRestoreOffset.Value : new Point(); }
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			SaveRestoreOffset();
		}
		bool CanGetRestoreOffset() {
			return !IsDisposing && ScreenHelper.IsAttachedToPresentationSource(this) && (OwnerWindow == null || OwnerWindow.WindowState != WindowState.Minimized) && IsMeasureValid;
		}
		void SaveRestoreOffset() {
			if(CanGetRestoreOffset())
				effectiveRestoreOffset = GetRestoreOffset();
		}
		void OnAllowMergingAutoHidePanelsChanged(bool oldValue, bool newValue) {
			MergingHelper.Do(x => x.Changed());
		}
		private void OnAutoHideModeChanged(Base.AutoHideMode oldValue, Base.AutoHideMode newValue) {
			RaiseAutoHideDisplayModeChanged();
		}
		protected override Visual GetVisualChild(int index) {
			if(index < base.VisualChildrenCount)
				return base.GetVisualChild(index);
			return visualChildrenCore[index - base.VisualChildrenCount];
		}
		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount + visualChildrenCore.Count;
			}
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				if(IsDisposing)
					return Enumerable.Empty<object>().GetEnumerator();
				else {
					return new MergedEnumerator(
							new IEnumerator[] { 
								new CustomizationEnumerator(this),
								new LogicalElementsEnumerator(this),
								logicalChildrenCore.GetEnumerator()
							});
				}
			}
		}
		#region Serialization
		public void SaveLayoutToStream(Stream stream) {
			SerializationController.SaveLayout(stream);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(stream);
		}
		public void SaveLayoutToXml(string path) {
			SerializationController.SaveLayout(path);
		}
		public void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(path);
		}
		void RestoreLayoutCore(object path) {
			using(new NotificationBatch(this)) {
				SerializationController.RestoreLayout(path);
				NotificationBatch.Action(this, this);
			}
		}
		#endregion Serialization
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerContextMenuCustomizations")]
#endif
		public BarManagerActionCollection ContextMenuCustomizations {
			get { return CustomizationController.ItemContextMenuController.ActionContainer.Actions; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerItemSelectorMenuCustomizations")]
#endif
		public BarManagerActionCollection ItemSelectorMenuCustomizations {
			get { return CustomizationController.ItemsSelectorMenuController.ActionContainer.Actions; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutControlItemContextMenuCustomizations")]
#endif
		public BarManagerActionCollection LayoutControlItemContextMenuCustomizations {
			get { return CustomizationController.LayoutControlItemContextMenuController.ActionContainer.Actions; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutControlItemCustomizationMenuCustomizations")]
#endif
		public BarManagerActionCollection LayoutControlItemCustomizationMenuCustomizations {
			get { return CustomizationController.LayoutControlItemCustomizationMenuController.ActionContainer.Actions; }
		}
		#region End-User Events
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerRequestUniqueName")]
#endif
		public event RequestUniqueNameEventHandler RequestUniqueName {
			add { base.AddHandler(RequestUniqueNameEvent, value); }
			remove { base.RemoveHandler(RequestUniqueNameEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowingMenu")]
#endif
		public event ShowingMenuEventHandler ShowingMenu {
			add { base.AddHandler(ShowingMenuEvent, value); }
			remove { base.RemoveHandler(ShowingMenuEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemActivating")]
#endif
		public event DockItemCancelEventHandler DockItemActivating {
			add { base.AddHandler(DockItemActivatingEvent, value); }
			remove { base.RemoveHandler(DockItemActivatingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemActivated")]
#endif
		public event DockItemActivatedEventHandler DockItemActivated {
			add { base.AddHandler(DockItemActivatedEvent, value); }
			remove { base.RemoveHandler(DockItemActivatedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemStartDocking")]
#endif
		public event DockItemCancelEventHandler DockItemStartDocking {
			add { base.AddHandler(DockItemStartDockingEvent, value); }
			remove { base.RemoveHandler(DockItemStartDockingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemDocking")]
#endif
		public event DockItemDockingEventHandler DockItemDocking {
			add { base.AddHandler(DockItemDockingEvent, value); }
			remove { base.RemoveHandler(DockItemDockingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemEndDocking")]
#endif
		public event DockItemDockingEventHandler DockItemEndDocking {
			add { base.AddHandler(DockItemEndDockingEvent, value); }
			remove { base.RemoveHandler(DockItemEndDockingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemHiding")]
#endif
		public event DockItemCancelEventHandler DockItemHiding {
			add { base.AddHandler(DockItemHidingEvent, value); }
			remove { base.RemoveHandler(DockItemHidingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemClosing")]
#endif
		public event DockItemCancelEventHandler DockItemClosing {
			add { base.AddHandler(DockItemClosingEvent, value); }
			remove { base.RemoveHandler(DockItemClosingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemRestoring")]
#endif
		public event DockItemCancelEventHandler DockItemRestoring {
			add { base.AddHandler(DockItemRestoringEvent, value); }
			remove { base.RemoveHandler(DockItemRestoringEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemHidden")]
#endif
		public event DockItemEventHandler DockItemHidden {
			add { base.AddHandler(DockItemHiddenEvent, value); }
			remove { base.RemoveHandler(DockItemHiddenEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemClosed")]
#endif
		public event DockItemClosedEventHandler DockItemClosed {
			add { base.AddHandler(DockItemClosedEvent, value); }
			remove { base.RemoveHandler(DockItemClosedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemRestored")]
#endif
		public event DockItemEventHandler DockItemRestored {
			add { base.AddHandler(DockItemRestoredEvent, value); }
			remove { base.RemoveHandler(DockItemRestoredEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemDragging")]
#endif
		public event DockItemDraggingEventHandler DockItemDragging {
			add { base.AddHandler(DockItemDraggingEvent, value); }
			remove { base.RemoveHandler(DockItemDraggingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemExpanded")]
#endif
		public event DockItemExpandedEventHandler DockItemExpanded {
			add { base.AddHandler(DockItemExpandedEvent, value); }
			remove { base.RemoveHandler(DockItemExpandedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerDockItemCollapsed")]
#endif
		public event DockItemCollapsedEventHandler DockItemCollapsed {
			add { base.AddHandler(DockItemCollapsedEvent, value); }
			remove { base.RemoveHandler(DockItemCollapsedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerIsCustomizationChanged")]
#endif
		public event IsCustomizationChangedEventHandler IsCustomizationChanged {
			add { base.AddHandler(IsCustomizationChangedEvent, value); }
			remove { base.RemoveHandler(IsCustomizationChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerCustomizationFormVisibleChanged")]
#endif
		public event CustomizationFormVisibleChangedEventHandler CustomizationFormVisibleChanged {
			add { base.AddHandler(CustomizationFormVisibleChangedEvent, value); }
			remove { base.RemoveHandler(CustomizationFormVisibleChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemActivating")]
#endif
		public event LayoutItemCancelEventHandler LayoutItemActivating {
			add { base.AddHandler(LayoutItemActivatingEvent, value); }
			remove { base.RemoveHandler(LayoutItemActivatingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemActivated")]
#endif
		public event LayoutItemActivatedEventHandler LayoutItemActivated {
			add { base.AddHandler(LayoutItemActivatedEvent, value); }
			remove { base.RemoveHandler(LayoutItemActivatedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemSelectionChanging")]
#endif
		public event LayoutItemSelectionChangingEventHandler LayoutItemSelectionChanging {
			add { base.AddHandler(LayoutItemSelectionChangingEvent, value); }
			remove { base.RemoveHandler(LayoutItemSelectionChangingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemSelectionChanged")]
#endif
		public event LayoutItemSelectionChangedEventHandler LayoutItemSelectionChanged {
			add { base.AddHandler(LayoutItemSelectionChangedEvent, value); }
			remove { base.RemoveHandler(LayoutItemSelectionChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemSizeChanged")]
#endif
		public event LayoutItemSizeChangedEventHandler LayoutItemSizeChanged {
			add { base.AddHandler(LayoutItemSizeChangedEvent, value); }
			remove { base.RemoveHandler(LayoutItemSizeChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemHidden")]
#endif
		public event LayoutItemHiddenEventHandler LayoutItemHidden {
			add { base.AddHandler(LayoutItemHiddenEvent, value); }
			remove { base.RemoveHandler(LayoutItemHiddenEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemRestored")]
#endif
		public event LayoutItemRestoredEventHandler LayoutItemRestored {
			add { base.AddHandler(LayoutItemRestoredEvent, value); }
			remove { base.RemoveHandler(LayoutItemRestoredEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemMoved")]
#endif
		public event LayoutItemMovedEventHandler LayoutItemMoved {
			add { base.AddHandler(LayoutItemMovedEvent, value); }
			remove { base.RemoveHandler(LayoutItemMovedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemStartRenaming")]
#endif
		public event LayoutItemCancelEventHandler LayoutItemStartRenaming {
			add { base.AddHandler(LayoutItemStartRenamingEvent, value); }
			remove { base.RemoveHandler(LayoutItemStartRenamingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerLayoutItemEndRenaming")]
#endif
		public event LayoutItemEndRenamingEventHandler LayoutItemEndRenaming {
			add { base.AddHandler(LayoutItemEndRenamingEvent, value); }
			remove { base.RemoveHandler(LayoutItemEndRenamingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerMDIItemActivating")]
#endif
		public event MDIItemCancelEventHandler MDIItemActivating {
			add { base.AddHandler(MDIItemActivatingEvent, value); }
			remove { base.RemoveHandler(MDIItemActivatingEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerMDIItemActivated")]
#endif
		public event MDIItemActivatedEventHandler MDIItemActivated {
			add { base.AddHandler(MDIItemActivatedEvent, value); }
			remove { base.RemoveHandler(MDIItemActivatedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowInvisibleItemsChanged")]
#endif
		public event ShowInvisibleItemsChangedEventHandler ShowInvisibleItemsChanged {
			add { base.AddHandler(ShowInvisibleItemsChangedEvent, value); }
			remove { base.RemoveHandler(ShowInvisibleItemsChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerItemIsVisibleChanged")]
#endif
		public event ItemIsVisibleChangedEventHandler ItemIsVisibleChanged {
			add { base.AddHandler(ItemIsVisibleChangedEvent, value); }
			remove { base.RemoveHandler(ItemIsVisibleChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerShowingDockHints")]
#endif
		public event ShowingDockHintsEventHandler ShowingDockHints {
			add { base.AddHandler(ShowingDockHintsEvent, value); }
			remove { base.RemoveHandler(ShowingDockHintsEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerMerge")]
#endif
		public event BarMergeEventHandler Merge {
			add { base.AddHandler(MergeEvent, value); }
			remove { base.RemoveHandler(MergeEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerUnMerge")]
#endif
		public event BarMergeEventHandler UnMerge {
			add { base.AddHandler(UnMergeEvent, value); }
			remove { base.RemoveHandler(UnMergeEvent, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DockLayoutManagerBeforeItemAdded")]
#endif
		public event BeforeItemAddedEventHandler BeforeItemAdded {
			add { base.AddHandler(BeforeItemAddedEvent, value); }
			remove { base.RemoveHandler(BeforeItemAddedEvent, value); }
		}
		public event DockOperationStartingEventHandler DockOperationStarting {
			add { base.AddHandler(DockOperationStartingEvent, value); }
			remove { base.RemoveHandler(DockOperationStartingEvent, value); }
		}
		public event DockOperationCompletedEventHandler DockOperationCompleted {
			add { base.AddHandler(DockOperationCompletedEvent, value); }
			remove { base.RemoveHandler(DockOperationCompletedEvent, value); }
		}
		#endregion Events
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(OwnerWindowBoundsChangedEventManager)) {
				OwnerWindowBoundsChanged();
				return true;
			}
			if(managerType == typeof(OwnerWindowClosedEventManager)) {
				OwnerWindowClosed(sender);
				return true;
			}
			if(managerType == typeof(VisualRootPreviewKeyDownEventManager)) {
				OwnerWindowPreviewKeyDown(sender, e as KeyEventArgs);
				return true;
			}
			if(managerType == typeof(VisualRootPreviewKeyUpEventManager)) {
				OwnerWindowPreviewKeyUp(sender, e as KeyEventArgs);
				return true;
			}
			return false;
		}
		#endregion
		#region ItemsSource
		protected virtual ILayoutAdapter ResolveLayoutAdapter() {
			return MVVMHelper.GetLayoutAdapter(this) ?? LayoutAdapter.Instance;
		}
		void PrepareItemCore(object obj) {
			ILayoutAdapter adapter = ResolveLayoutAdapter();
			string targetName = adapter.Resolve(this, obj);
			(obj as INotifyPropertyChanged).Do(x => x.PropertyChanged += OnItemTargetNameChanged);
			BaseLayoutItem target = this.RaiseBeforeItemAddedEvent(obj, string.IsNullOrEmpty(targetName) ? null : this.GetItem(targetName));
			GetContainerForItem(obj, target);
		}
		void ClearItemCore(object obj) {
			ClearContainerForItem(obj);
			(obj as INotifyPropertyChanged).Do(x => x.PropertyChanged -= OnItemTargetNameChanged);
		}
		void OnItemTargetNameChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "TargetName") {
				ClearItemCore(sender);
				PrepareItemCore(sender);
			}
		}
		protected internal virtual void OnAddToItemsSource(IEnumerable newItems, int startingIndex = 0) {
			if(newItems == null) return;
			foreach(object obj in newItems) {
				PrepareItemCore(obj);
			}
		}
		protected internal virtual void OnRemoveFromItemsSource(IEnumerable oldItems) {
			if(oldItems != null) {
				foreach(object obj in oldItems) {
					ClearItemCore(obj);
				}
			}
		}
		protected internal virtual void OnItemReplacedInItemsSource(IList oldItems, IList newItems, int newStartingIndex) {
			containerGenerator.LinkContainerToItem(oldItems[0], newItems[0], newStartingIndex, ItemTemplate, ItemTemplateSelector);
		}
		protected virtual object GetContainerForItem(object item, BaseLayoutItem target) {
			return containerGenerator.GetContainerForItem(target as IGeneratorHost, item, ItemTemplate, ItemTemplateSelector);
		}
		protected virtual void ClearContainerForItem(object item) {
			containerGenerator.ClearContainerForItem(item);
		}
		protected internal virtual void OnResetItemsSource(IEnumerable source) {
			using(new UpdateBatch(this)) {
				ResetItemsSource();
				OnAddToItemsSource(source);
			}
		}
		protected virtual void ResetItemsSource() {
			containerGenerator.Reset();
		}
		protected internal virtual void OnCurrentChanged(object sender) {
			var view = sender as ICollectionView;
			OnCurrentCollectionItemChanged(view.CurrentItem);
		}
		protected virtual void OnCurrentCollectionItemChanged(object value) {
			if(!IsSynchronizedWithCurrentItem) return;
			if(value != null) {
				BaseLayoutItem item = containerGenerator.GetContainerForItem(value) as BaseLayoutItem;
				Activate(item);
			}
		}
		protected virtual void OnIsSynchronizedWithCurrentItemChanged(bool oldValue, bool newValue) {
			OnCurrentCollectionItemChanged(itemsInternal.CurrentItem);
		}
		protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue, DependencyPropertyChangedEventArgs e) {
			if(IsDisposing) return;
			using(new UpdateBatch(this)) {
				ResetItemsSource();
				if(IsInitialized || newValue == null)
					itemsInternal.SetItemsSource(newValue);
				else
					initilizedActionsHelper.AddDelayedAction(new Action(() => itemsInternal.SetItemsSource(ItemsSource)));
			}
		}
		void SynchronizeWithCurrentItem(BaseLayoutItem item) {
			if(item == null) return;
			if(IsSynchronizedWithCurrentItem) {
				object content = containerGenerator.GetItemForContainer(item);
				if(content != null) itemsInternal.MoveCurrentTo(content);
			}
		}
		#endregion
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.Docking.UIAutomation.DockLayoutManagerAutomationPeer(this);
		}
		#endregion UIAutomation
		#region ILogicalTreeOwner Members
		List<object> logicalChildrenCore = new List<object>();
		protected new void AddLogicalChild(object child) {
			DependencyObject dObj = child as DependencyObject;
			if(LogicalTreeHelper.GetParent(dObj) != null) return;
			BaseLayoutItem item = child as BaseLayoutItem;
			if(item != null) {
				if(!logicalChildrenCore.Contains(item))
					logicalChildrenCore.Add(item);
				base.AddLogicalChild(child);
			}
			else InternalElementsContainer.Add(dObj);
		}
		protected new void RemoveLogicalChild(object child) {
			DependencyObject dObj = child as DependencyObject;
			logicalChildrenCore.Remove(child);
			InternalElementsContainer.Remove(dObj);
			base.RemoveLogicalChild(child);
		}
		#endregion
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		#endregion
		#region ISupportBatchUpdate Members
		void ISupportBatchUpdate.BeginUpdate() {
			LockUpdate();
		}
		void ISupportBatchUpdate.EndUpdate() {
			UnlockUpdate();
		}
		bool ISupportBatchUpdate.IsUpdatedLocked {
			get { return IsUpdateLocked; }
		}
		#endregion
		#region internal
		enum LayoutItemState { Close, AutoHide }
		class ThemeTreeWalkerHelper {
			DependencyObject Owner;
			WeakReference walker;
			public ThemeTreeWalkerHelper(DependencyObject owner) {
				Owner = owner;
			}
			public bool IsThemeChanging() {
				bool isThemeChanging = false;
				string theme = null;
				if(walker != null) {
					ThemeTreeWalker w = walker.Target as ThemeTreeWalker;
					if(w != null)
						theme = w.ThemeName;
				}
				ThemeTreeWalker current = ThemeManager.GetTreeWalker(Owner);
				if(current != null) {
					if(current.ThemeName != theme) {
						isThemeChanging = true;
						walker = new WeakReference(current);
					}
				}
				else {
					walker = null;
				}
				return isThemeChanging;
			}
		}
		class LayoutItemStateHelper {
			DockLayoutManager Owner;
			public LayoutItemStateHelper(DockLayoutManager owner) {
				Owner = owner;
			}
			public void QueueCheckLayoutItemState(BaseLayoutItem item, LayoutItemState state) {
				Action<BaseLayoutItem, LayoutItemState> closeAction = new Action<BaseLayoutItem, LayoutItemState>((x, y) => UpdateLayoutItemState(x, y));
				if(item.IsLayoutChangeInProgress) Owner.Dispatcher.BeginInvoke(closeAction, item, state);
				else closeAction(item, state);
			}
			void UpdateClosedState(BaseLayoutItem item) {
				if(item.Closed) {
					if(item.Parent != null)
						item.Closed = Owner.DockController.Close(item);
				}
				else
					Owner.DockController.Restore(item);
			}
			void UpdateAutoHiddenState(BaseLayoutItem item) {
				LayoutPanel panel = item as LayoutPanel;
				if(panel == null) return;
				if(panel.AutoHidden) {
					if(item.Parent != null)
						Owner.DockController.Hide(item);
				}
				else
					Owner.DockController.Dock(item);
				item.SetAutoHidden(item.IsAutoHidden);
			}
			void UpdateLayoutItemState(BaseLayoutItem item, LayoutItemState state) {
				switch(state){
					case LayoutItemState.Close:
						UpdateClosedState(item);
						break;
					case LayoutItemState.AutoHide:
						UpdateAutoHiddenState(item);
						break;
				}
			}
		}
		class ContainerGenerator {
			Dictionary<object, DependencyObject> itemsHash = new Dictionary<object, DependencyObject>();
			Dictionary<DependencyObject, IGeneratorHost> containerHash = new Dictionary<DependencyObject, IGeneratorHost>();
			public DependencyObject GetContainerForItem(IGeneratorHost generatorHost, object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
				if(generatorHost == null) return null;
				DependencyObject container = generatorHost.GenerateContainerForItem(item, itemTemplate, itemTemplateSelector);
				itemsHash.Add(item, container);
				containerHash.Add(container, generatorHost);
				return container;
			}
			public DependencyObject LinkContainerToItem(object oldValue, object newValue, int newStartingIndex, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
				DependencyObject container = itemsHash[oldValue];
				IGeneratorHost generatorHost;
				if(containerHash.ContainsKey(container)) {
					generatorHost = containerHash[container];
					if(generatorHost != null) {
						containerHash.Remove(container);
						itemsHash.Remove(oldValue);
						DependencyObject newContainer = generatorHost.LinkContainerToItem(container, newValue, itemTemplate, itemTemplateSelector);
						itemsHash.Add(newValue, newContainer);
						containerHash.Add(newContainer, generatorHost);
						return newContainer;
					}
				}
				return null;
			}
			public virtual void ClearContainerForItem(object item) {
				if(itemsHash.ContainsKey(item)) {
					DependencyObject container = itemsHash[item];
					if(containerHash.ContainsKey(container)) {
						IGeneratorHost generatorHost = containerHash[container];
						if(generatorHost != null) generatorHost.ClearContainer(container, item);
						containerHash.Remove(container);
					}
					itemsHash.Remove(item);
				}
			}
			public void Reset() {
				var items = itemsHash.Keys.ToArray();
				foreach(object obj in items) {
					ClearContainerForItem(obj);
				}
				itemsHash.Clear();
				containerHash.Clear();
			}
			public DependencyObject GetContainerForItem(object item) {
				DependencyObject container;
				itemsHash.TryGetValue(item, out container);
				return container;
			}
			public object GetItemForContainer(DependencyObject container) {
				return itemsHash.ContainsValue(container) ? itemsHash.GetKeyByValue(container) : null;
			}
		}
		class DecomposedItemsCollection : LayoutGroupCollection {
			DockLayoutManager Owner;
			public DecomposedItemsCollection(DockLayoutManager owner) {
				Owner = owner;
			}
		}
		class DisableFloatingPanelTransparencyBehavior : DevExpress.Mvvm.UI.Interactivity.Behavior<DockLayoutManager> {
			ResourceDictionary floatPaneStyles;
			ResourceDictionary FloatPaneStyles {
				get {
					if(floatPaneStyles == null) floatPaneStyles = new ResourceDictionary();
					return floatPaneStyles;
				}
			}
			protected override void OnAttached() {
				base.OnAttached();
				AssociatedObject.SetValue(DockLayoutManager.AllowFloatGroupTransparencyProperty, false);
				var walker = ThemeManager.GetTreeWalker(AssociatedObject);
				string path;
				if(walker == null || string.IsNullOrEmpty(walker.ThemeName) || walker.ThemeName == Theme.DeepBlueName) {
					path = string.Format(@"/{0};component\Themes\DeepBlue\FloatPane.WinFormsHost.xaml", AssemblyInfo.SRAssemblyXpfDocking);
				}
				else {
					string themeName = walker.ThemeName;
					string assemblyThemeName = GetThemeAssemblyName(themeName);
					path = String.Format(@"/{0};component\DevExpress.Xpf.Docking\{1}\FloatPane.WinFormsHost.xaml", assemblyThemeName, themeName);
				}
				Uri resourceUri = new Uri(path, UriKind.RelativeOrAbsolute);
				FloatPaneStyles.Source = resourceUri;
				if(!AssociatedObject.Resources.MergedDictionaries.Contains(FloatPaneStyles))
					AssociatedObject.Resources.MergedDictionaries.Add(FloatPaneStyles);
			}
			string GetThemeAssemblyName(string themeName) {
				var theme = Theme.FindTheme(themeName);
				return theme == null ? null : theme.AssemblyName;
			}
			protected override void OnDetaching() {
				if(AssociatedObject != null)
					AssociatedObject.Resources.MergedDictionaries.Remove(FloatPaneStyles);
				base.OnDetaching();
			}
		}
		#endregion
		DockLayoutManagerMergingHelper mergingHelper;
		DockLayoutManagerMergingHelper MergingHelper { get { return mergingHelper; } }
		WeakList<DockLayoutManager> _Linked = new WeakList<DockLayoutManager>();
		internal WeakList<DockLayoutManager> Linked {
			get { return _Linked; }
		}
		internal IDragService LinkedDragService { get; set; }
		internal void OnMerge(DockLayoutManager child) {
			if(IsDisposing) return;
			DockLayoutManagerLinker.Link(this, child);
			child.MergedParent = this;
			if(AllowMergingAutoHidePanels && child.AllowMergingAutoHidePanels) {
				AutoHideGroups.Merge(child, child.AutoHideGroups);
			}
		}
		internal void OnUnmerge(DockLayoutManager child) {
			if(IsDisposing) return;
			DockLayoutManagerLinker.Unlink(this, child);
			AutoHideGroups.Unmerge(child, child.AutoHideGroups);
		}
		void OnAutoHideGroupsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(AllowMergingAutoHidePanels && MergedParent.Return(x => x.AllowMergingAutoHidePanels, () => false)) {
				MergedParent.With(x => x.AutoHideGroups).Do(x => x.Merge(this, AutoHideGroups));
			}
		}
		DockLayoutManager MergedParent { get; set; }
	}
	class DockLayoutManagerMergingHelper : FrameworkContentElement, IMergingSupport, IDisposable {
		readonly DockLayoutManager owner;
		readonly ObservableCollection<DockLayoutManagerMergingHelper> mergedChildren;
		internal DockLayoutManagerMergingHelper(DockLayoutManager owner) {
			this.owner = owner;
			mergedChildren = new ObservableCollection<DockLayoutManagerMergingHelper>();
			mergedChildren.CollectionChanged += OnMergedChildrenCollectionChanged;
			BarNameScope.SetIsScopeOwner(this, false);
			Changed();
		}
		bool isDisposed;
		public void Dispose() {
			if(!isDisposed) {
				isDisposed = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			((IMergingSupport)MergedParent).Do(x => x.Unmerge(this));
		}
		ObservableCollection<DockLayoutManagerMergingHelper> MergedChildren { get { return mergedChildren; } }
		DockLayoutManager Owner { get { return owner; } }
		internal DockLayoutManagerMergingHelper MergedParent { get; private set; }
		bool IMergingSupport.CanMerge(IMergingSupport second) { return true; }
		bool IMergingSupport.IsAutomaticallyMerged { get { return true; } set { } }
		bool IMergingSupport.IsMerged { get { return MergedParent != null; } } 
		bool IMergingSupport.IsMergedParent(IMergingSupport second) {
			return this.MergedParent == second;
		}
		void IMergingSupport.Merge(IMergingSupport second) {
			var dlm = second as DockLayoutManagerMergingHelper;
			if(dlm == null) return;
			if(!MergedChildren.Contains(dlm))
				MergedChildren.Add(dlm);
		}
		object IMergingSupport.MergingKey { get { return typeof(DockLayoutManager); } }
		void IMergingSupport.Unmerge(IMergingSupport second) {
			var dlm = second as DockLayoutManagerMergingHelper;
			if(dlm == null) return;
			MergedChildren.Remove(dlm);
		}
		void OnMergedChildrenCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(e.OldItems != null) {
				foreach(DockLayoutManagerMergingHelper element in e.OldItems) {
					element.MergedParent = null;
					Owner.OnUnmerge(element.Owner);
				}
			}
			if(e.NewItems != null) {
				foreach(DockLayoutManagerMergingHelper element in e.NewItems) {
					element.MergedParent = this;
					Owner.OnMerge(element.Owner);
				}
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			MergingPropertiesHelper.OnPropertyChanged(this, e, MergingID);
		}
		public void Changed() {
			MergingProperties.SetElementMergingBehavior(this, Owner.AllowMergingAutoHidePanels ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.InternalWithInternal);
		}
		#region IMultipleElementRegistratorSupport Members
		const string MergingID = "0C082948-C6A4-4274-BF91-A3039EF0092F";
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {			
			if(Equals(registratorKey,typeof(IMergingSupport)))
				return MergingID;
			throw new ArgumentException("registratorKey");
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { 
				yield return typeof(IMergingSupport);
			}
		}
		#endregion    
	}
	public static class DockLayoutManagerLinker {
		public static void Link(DockLayoutManager first, DockLayoutManager second) {
			if(first == null || second == null || first == second) return;
			if(!first.Linked.Contains(second)) first.Linked.Add(second);
			if(!second.Linked.Contains(first)) second.Linked.Add(first);
		}
		public static void Unlink(DockLayoutManager first, DockLayoutManager second) {
			if(first == null || second == null || first == second) return;
			if(first.Linked.Contains(second)) first.Linked.Remove(second);
			if(second.Linked.Contains(first)) second.Linked.Remove(first);
		}
	}
}
