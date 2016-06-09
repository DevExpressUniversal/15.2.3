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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Core {
	public enum HeaderOrientation { Default, Horizontal, Vertical }
	[Obsolete("Use the ScrollButtonShowMode type.")]
	public enum ButtonShowMode { Always, Never, WhenNeeded }
	public enum ScrollButtonShowMode { Always, Never, AutoHideBothButtons, AutoHideEachButton }
	public class TabControlScrollView : TabControlViewBase {
		#region Properties
		public static readonly DependencyProperty HeaderOrientationProperty = DependencyProperty.Register("HeaderOrientation", typeof(HeaderOrientation), typeof(TabControlScrollView),
		   new PropertyMetadata(HeaderOrientation.Default, (d, e) => ((TabControlScrollView)d).UpdateViewProperties()));
		public static readonly DependencyProperty HeaderAutoFillProperty = DependencyProperty.Register("HeaderAutoFill", typeof(bool), typeof(TabControlScrollView), new PropertyMetadata(false));
		public static readonly DependencyProperty AllowAnimationProperty = DependencyProperty.Register("AllowAnimation", typeof(bool), typeof(TabControlScrollView), new PropertyMetadata(true));
		public static readonly DependencyProperty ScrollButtonShowModeProperty = DependencyProperty.Register("ScrollButtonShowMode", typeof(ScrollButtonShowMode), typeof(TabControlScrollView),
			new PropertyMetadata(ScrollButtonShowMode.AutoHideBothButtons, (d, e) => ((TabControlScrollView)d).UpdateScrollButtonsVisibility()));
		static readonly DependencyPropertyKey IsScrollPrevButtonVisiblePropertyKey =
			DependencyProperty.RegisterReadOnly("IsScrollPrevButtonVisible", typeof(bool), typeof(TabControlScrollView), new PropertyMetadata(false));
		static readonly DependencyPropertyKey IsScrollNextButtonVisiblePropertyKey =
			DependencyProperty.RegisterReadOnly("IsScrollNextButtonVisible", typeof(bool), typeof(TabControlScrollView), new PropertyMetadata(false));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty IsScrollPrevButtonVisibleProperty = IsScrollPrevButtonVisiblePropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty IsScrollNextButtonVisibleProperty = IsScrollNextButtonVisiblePropertyKey.DependencyProperty;
		public static readonly DependencyProperty AllowScrollOnMouseWheelProperty =
			DependencyProperty.Register("AllowScrollOnMouseWheel", typeof(bool), typeof(TabControlScrollView), new PropertyMetadata(true));
		public ScrollButtonShowMode ScrollButtonShowMode { get { return (ScrollButtonShowMode)GetValue(ScrollButtonShowModeProperty); } set { SetValue(ScrollButtonShowModeProperty, value); } }
		public bool AllowScrollOnMouseWheel { get { return (bool)GetValue(AllowScrollOnMouseWheelProperty); } set { SetValue(AllowScrollOnMouseWheelProperty, value); } }
		[Obsolete("Use the ScrollButtonShowMode property.")]
		public ButtonShowMode ScrollButtonsShowMode {
			get {
#pragma warning disable
				switch(ScrollButtonShowMode) {
					case ScrollButtonShowMode.Always: return ButtonShowMode.Always;
					case ScrollButtonShowMode.AutoHideBothButtons:
					case ScrollButtonShowMode.AutoHideEachButton: return ButtonShowMode.WhenNeeded;
					case ScrollButtonShowMode.Never:
					default: return ButtonShowMode.Never;
				}
#pragma warning restore
			}
			set {
#pragma warning disable
				switch(value) {
					case ButtonShowMode.Always: 
						ScrollButtonShowMode = ScrollButtonShowMode.Always;
						break;
					case ButtonShowMode.WhenNeeded:
						ScrollButtonShowMode = AutoHideScrollButtons ? ScrollButtonShowMode.AutoHideEachButton : ScrollButtonShowMode.AutoHideBothButtons;
						break;
					case ButtonShowMode.Never:
					default:
						ScrollButtonShowMode = ScrollButtonShowMode.Never;
						break;
				}
#pragma warning restore
			}
		}
		[Obsolete("Use the ScrollButtonShowMode property.")]
		public bool AutoHideScrollButtons {
			get { return ScrollButtonShowMode == ScrollButtonShowMode.AutoHideEachButton; }
			set {
#pragma warning disable
				ScrollButtonsShowMode = ScrollButtonsShowMode; 
#pragma warning restore
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewHeaderOrientation")]
#endif
		public HeaderOrientation HeaderOrientation { get { return (HeaderOrientation)GetValue(HeaderOrientationProperty); } set { SetValue(HeaderOrientationProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewHeaderAutoFill")]
#endif
		public bool HeaderAutoFill { get { return (bool)GetValue(HeaderAutoFillProperty); } set { SetValue(HeaderAutoFillProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewAllowAnimation")]
#endif
		public bool AllowAnimation { get { return (bool)GetValue(AllowAnimationProperty); } set { SetValue(AllowAnimationProperty, value); } }
		public DelegateCommand ScrollPrevCommand { get; private set; }
		public DelegateCommand ScrollNextCommand { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DelegateCommand<bool> ScrollToSelectedCommand { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DelegateCommand UpdateScrollButtonsCommand { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DelegateCommand<MouseWheelEventArgs> ScrollOnMouseWheelCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewCanScroll")]
#endif
		public bool CanScroll { get { return ScrollPanel.Return(x => x.CanScroll, () => false); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewCanScrollPrev")]
#endif
		public bool CanScrollPrev { get { return ScrollPanel.Return(x => x.CanScrollPrev, () => false); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewCanScrollNext")]
#endif
		public bool CanScrollNext { get { return ScrollPanel.Return(x => x.CanScrollNext, () => false); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlScrollViewCanScrollToSelectedTabItem")]
#endif
		public bool CanScrollToSelectedTabItem { get { return ScrollPanel.Return(x => x.CanScrollTo(Owner.SelectedContainer), () => false); } }
		protected internal TabPanelScrollView ScrollPanel { get { return Owner.With(x => x.TabPanel).With(x => x.Panel as TabPanelScrollView); } }
		#endregion Properties
		public TabControlScrollView() {
			ScrollPrevCommand = new DelegateCommand(ScrollPrev, () => CanScrollPrev, false);
			ScrollNextCommand = new DelegateCommand(ScrollNext, () => CanScrollNext, false);
			ScrollToSelectedCommand = new DelegateCommand<bool>(ScrollToSelectedTabItem, false);
			UpdateScrollButtonsCommand = new DelegateCommand(UpdateScrollCommands, false);
			ScrollOnMouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(x => {
				TabViewInfo vInfo = new TabViewInfo(Owner);
				if(x.Delta > 0 ^ vInfo.Orientation == Orientation.Vertical)
					ScrollNext();
				if(x.Delta < 0 ^ vInfo.Orientation == Orientation.Vertical)
					ScrollPrev();
			}, x => {
				TabViewInfo vInfo = new TabViewInfo(Owner);
				if(x.Delta > 0 ^ vInfo.Orientation == Orientation.Vertical)
					return CanScrollNext;
				if(x.Delta < 0 ^ vInfo.Orientation == Orientation.Vertical)
					return CanScrollPrev;
				return false;
			}, false);
			UpdateScrollCommands();
		}
		public virtual void ScrollFirst() {
			ScrollPanel.Do(x => x.ScrollToBegin());
			UpdateScrollCommands();
		}
		public virtual void ScrollLast() {
			ScrollPanel.Do(x => x.ScrollToEnd());
			UpdateScrollCommands();
		}
		public void ScrollPrev() {
			ScrollPanel.Do(x => x.ScrollPrev());
			UpdateScrollCommands();
		}
		public void ScrollNext() {
			ScrollPanel.Do(x => x.ScrollNext());
			UpdateScrollCommands();
		}
		public void ScrollToSelectedTabItem(bool useAnimation = true) {
			ScrollPanel.Do(x => x.ScrollTo(Owner.SelectedContainer, useAnimation));
			UpdateScrollCommands();
		}
		protected internal override void UpdateViewPropertiesCore() {
			base.UpdateViewPropertiesCore();
			ScrollToSelectedTabItem();
			UpdateScrollButtonsVisibility();
		}
		void UpdateScrollButtonsVisibility() {
			switch(ScrollButtonShowMode) {
				case ScrollButtonShowMode.Always:
					SetValue(IsScrollPrevButtonVisiblePropertyKey, true);
					SetValue(IsScrollNextButtonVisiblePropertyKey, true);
					return;
				case ScrollButtonShowMode.AutoHideBothButtons:
					SetValue(IsScrollPrevButtonVisiblePropertyKey, CanScroll);
					SetValue(IsScrollNextButtonVisiblePropertyKey, CanScroll);
					return;
				case ScrollButtonShowMode.AutoHideEachButton:
					SetValue(IsScrollPrevButtonVisiblePropertyKey, CanScrollPrev);
					SetValue(IsScrollNextButtonVisiblePropertyKey, CanScrollNext);
					return;
				case ScrollButtonShowMode.Never:
					SetValue(IsScrollPrevButtonVisiblePropertyKey, false);
					SetValue(IsScrollNextButtonVisiblePropertyKey, false);
					return;
			}
		}
		protected virtual void UpdateScrollCommands() {
			ScrollPrevCommand.RaiseCanExecuteChanged();
			ScrollNextCommand.RaiseCanExecuteChanged();
			ScrollToSelectedCommand.RaiseCanExecuteChanged();
			UpdateScrollButtonsVisibility();
		}
	}
}
