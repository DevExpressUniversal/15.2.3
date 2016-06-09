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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Localization;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
#if SL
using IBindingList = DevExpress.Data.Browsing.IBindingList;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid {
	[DXToolboxBrowsable]
	[TemplateVisualState(Name = MouseEnterState, GroupName = ShowLayoutListGroup)]
	[TemplateVisualState(Name = MouseLeaveState, GroupName = ShowLayoutListGroup)]
	public class PivotExcelFieldListControl : FieldListControlBase {
		internal const string ShowLayoutListButtonName = "ShowLayoutList", UpdateButtonName = "Update", LayoutSelectorName = "LayoutSelector",
			DeferUpdatesCheckName = "DefereUpdatesCheck", ShowLayoutListPanelName = "ShowLayoutListPanel", MouseEnterState = "MouseEnter",
			MouseLeaveState = "MouseLeave", ShowLayoutListGroup = "ShowLayoutListGroup";
		public PivotExcelFieldListControl() {
			this.SetDefaultStyleKey(typeof(PivotExcelFieldListControl));
			this.DataContext = this;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
#if !SL
		protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters) {
			System.Windows.Media.HitTestResult result = base.HitTestCore(hitTestParameters);
			Point pt = hitTestParameters.HitPoint;
			if(result == null && !(pt.X < 0 || pt.Y < 0 || pt.X > ActualWidth || pt.Y > ActualHeight)) {
				result = new System.Windows.Media.PointHitTestResult(this, hitTestParameters.HitPoint);
			}
			return result;
		}
#endif
		protected internal SimplePanel ShowLayoutListPanel { get; private set; }
#if !DEBUGTEST
		protected internal 
#else
		public
#endif
			Button 
			UpdateButton { get; private set; }
		protected internal Button ShowLayoutListButton1 { get; private set; }
		protected internal Button ShowLayoutListButton2 { get; private set; }
		protected virtual void OnShowLayoutListClick(object sender, RoutedEventArgs e) {
#if !SL
			BarPopupBase.ShowElementContextMenu(sender);
#else
			Point pt = HitTestHelper.TransformPointToRoot((UIElement)sender, new Point(0, ShowLayoutListButton1.ActualHeight));
			ContextMenuManager.ShowElementContextMenu(sender, pt); 
#endif
		}
		protected virtual void OnUpdateClick(object sender, RoutedEventArgs e) {
			if(PivotGrid == null) return;
			PivotGrid.Data.FieldListFields.SetFieldsToData();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeEvents();
			SubscribeEvents();
		}
		void SubscribeEvents() {
			ShowLayoutListButton1 = GetTemplateChild(ShowLayoutListButtonName) as Button;
			if(ShowLayoutListButton1 != null)
				ShowLayoutListButton1.Click += OnShowLayoutListClick;
			ShowLayoutListButton2 = GetTemplateChild(ShowLayoutListButtonName + "2") as Button;
			if(ShowLayoutListButton2 != null)
				ShowLayoutListButton2.Click += OnShowLayoutListClick;
			UpdateButton = GetTemplateChild(UpdateButtonName) as Button;
			if(UpdateButton != null)
				UpdateButton.Click += OnUpdateClick;
			ShowLayoutListPanel = GetTemplateChild(ShowLayoutListPanelName) as SimplePanel;
			if(ShowLayoutListPanel != null) {
				ShowLayoutListPanel.MouseEnter += ShowLayoutListPanelOnMouseEnter;
				ShowLayoutListPanel.MouseLeave += ShowLayoutListPanelOnMouseLeave;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			SubscribeEvents();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeEvents();
		}
		void UnsubscribeEvents() {
			if(ShowLayoutListButton1 != null)
				ShowLayoutListButton1.Click -= OnShowLayoutListClick;
			if(ShowLayoutListButton2 != null)
				ShowLayoutListButton2.Click -= OnShowLayoutListClick;
			if(UpdateButton != null)
				UpdateButton.Click -= OnUpdateClick;
			if(ShowLayoutListPanel != null) {
				ShowLayoutListPanel.MouseEnter -= ShowLayoutListPanelOnMouseEnter;
				ShowLayoutListPanel.MouseLeave -= ShowLayoutListPanelOnMouseLeave;
			}
		}
		void ShowLayoutListPanelOnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			VisualStateManager.GoToState(this, MouseLeaveState, true);
		}
		void ShowLayoutListPanelOnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			VisualStateManager.GoToState(this, MouseEnterState, true);
		}
		internal void MoveField(PivotGridField field, PivotArea areaTo, int index) {
			if(PivotGrid == null) return;
			PivotGrid.Data.FieldListFields.MoveField(field.FieldItem, areaTo, index);
		}
		internal void HideField(PivotGridField field) {
			if(PivotGrid == null) 
				return;
			PivotFieldItem item = field.FieldItem;
			if(item == null || !item.Visible && !PivotGrid.DeferredUpdates) return;
			PivotGrid.Data.FieldListFields.HideField(item);
		}
		internal int GetFieldIndex(PivotGridField movedToField, PivotGridField movedField, FieldListArea fieldListArea) {
			FieldArea toArea = fieldListArea.ToFieldArea();
			if(PivotGrid == null) return -1;
			PivotFieldsReadOnlyObservableCollection movedToFields = PivotGrid.Data.FieldListFields[toArea];
			int index = movedToField == null ? movedToFields.Count : movedToFields.IndexOf(movedToField);
			if(PivotGrid.Data.FieldListFields.IsFieldInArea(movedField, toArea) && index >= movedToFields.IndexOf(movedField))
				return index - 1;
			else
				return index;
		}
		protected override void OnOwnerChanged(ILogicalOwner oldOwner, ILogicalOwner newOwner) {
			OnPivotGridChanged((PivotGridControl)oldOwner, (PivotGridControl)newOwner);
		}
	}
	public enum FieldListLayout {
		StackedDefault = 0,
		StackedSideBySide = 1,
		TopPanelOnly = 2,
		BottomPanelOnly2by2 = 3,
		BottomPanelOnly1by4 = 4
	}
	public enum FieldListStyle {
		Simple,
		Excel2007
	}
	[Flags]
	public enum FieldListAllowedLayouts {
		All = 31,
		StackedDefault = 1,
		StackedSideBySide = 2,
		TopPanelOnly = 4,
		BottomPanelOnly2by2 = 8,
		BottomPanelOnly1by4 = 16
	}
	internal static class CustomizationFormEnumExtensions {
		internal static PivotGridStringId GetStringId(this FieldListLayout layout) {
			switch(layout) {
				case FieldListLayout.StackedDefault:
					return PivotGridStringId.CustomizationFormStackedDefault;
				case FieldListLayout.StackedSideBySide:
					return PivotGridStringId.CustomizationFormStackedSideBySide;
				case FieldListLayout.TopPanelOnly:
					return PivotGridStringId.CustomizationFormTopPanelOnly;
				case FieldListLayout.BottomPanelOnly2by2:
					return PivotGridStringId.CustomizationFormBottomPanelOnly2by2;
				case FieldListLayout.BottomPanelOnly1by4:
					return PivotGridStringId.CustomizationFormBottomPanelOnly1by4;
				default:
					throw new ArgumentException("FieldListLayout");
			}
		}
		public static bool IsLayoutAllowed(FieldListAllowedLayouts allowedLayouts, FieldListLayout layout) {
			if(allowedLayouts == FieldListAllowedLayouts.All)
				return true;
			int test = (int)Math.Pow(2, (int)layout);
			return (test & (int)allowedLayouts) != 0;
		}
	}
}
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class ExcelMenuBarButtonItem : BarButtonItem {
		public static readonly DependencyProperty PivotGridProperty;
		static ExcelMenuBarButtonItem() {
			Type typeOfThis = typeof(ExcelMenuBarButtonItem);
			PivotGridProperty = DependencyPropertyManager.Register("PivotGrid", typeof(PivotGridControl), typeOfThis);
		}
		public ExcelMenuBarButtonItem() {
			Command = DelegateCommandFactory.Create<FieldListLayout?>(SetLayout, CanSetLayout, false);
		}
		public PivotGridControl PivotGrid {
			get { return (PivotGridControl)GetValue(PivotGridProperty); }
			set { SetValue(PivotGridProperty,  value); }
		}
		void SetLayout(FieldListLayout? layout) {
			if(PivotGrid == null || layout == null)
				return;
			PivotGrid.FieldListLayout = (FieldListLayout)layout;
		}
		bool CanSetLayout(FieldListLayout? layout) {
			return PivotGrid != null && layout != null &&  layout != PivotGrid.FieldListLayout;
		}
	}
}
