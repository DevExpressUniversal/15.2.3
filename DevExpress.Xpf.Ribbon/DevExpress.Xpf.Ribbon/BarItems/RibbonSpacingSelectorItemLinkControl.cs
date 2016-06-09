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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon.Internal;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonSpacingSelectorItemLinkControl : BarSubItemLinkControl {
		public static readonly DependencyProperty ActualRibbonProperty;
		public static readonly DependencyProperty SelectedSpacingModeProperty;
		private BarItemMenuHeader header;
		private BarCheckItem mouseModeItem;
		private BarCheckItem touchModeItem;
		static RibbonSpacingSelectorItemLinkControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSpacingSelectorItemLinkControl), new FrameworkPropertyMetadata(typeof(RibbonSpacingSelectorItemLinkControl)));
			ActualRibbonProperty = DependencyPropertyManager.Register("ActualRibbon", typeof(RibbonControl), typeof(RibbonSpacingSelectorItemLinkControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonSpacingSelectorItemLinkControl)d).OnActualRibbonChanged((RibbonControl)e.OldValue)));
			SelectedSpacingModeProperty = DependencyPropertyManager.Register("SelectedSpacingMode", typeof(SpacingMode), typeof(RibbonSpacingSelectorItemLinkControl), new FrameworkPropertyMetadata(SpacingMode.Mouse, (d, e) => ((RibbonSpacingSelectorItemLinkControl)d).OnSelectedSpacingModeChanged((SpacingMode)e.OldValue)));
		}
		public RibbonSpacingSelectorItemLinkControl(RibbonSpacingSelectorItemLink link) : base(link) {
			ThemeManager.AddThemeChangedHandler(this, OnThemeChanged);
			ActivateMouseModeCommand = new DelegateCommand(new Action(ActivateMouseMode), false);
			ActivateTouchModeCommand = new DelegateCommand(new Action(ActivateTouchMode), false);
			UpdateActualRibbon();
		}
		void OnThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			UpdateModeChecks();
			UpdateVisibilityByThemeTreeWalker(ThemeManager.GetTreeWalker(this));
		}
		public RibbonSpacingSelectorItemLinkControl() : this(null) { }
		public BarItemMenuHeader Header {
			get {
				if (header == null) {
					header = new BarItemMenuHeader();
					header.Content = RibbonControlLocalizer.Active.GetLocalizedString(RibbonControlStringId.SpacingModeStrings_MenuHeaderCaption);
					header.IsPrivate = true;
				}
				return header;
			}
		}
		public BarCheckItem MouseModeItem {
			get {
				if (mouseModeItem == null) {
					mouseModeItem = new BarCheckItem();
					mouseModeItem.Content = RibbonControlLocalizer.Active.GetLocalizedString(RibbonControlStringId.SpacingModeStrings_MouseModeContent);
					mouseModeItem.Description = RibbonControlLocalizer.Active.GetLocalizedString(RibbonControlStringId.SpacingModeStrings_MouseModeDescription);
					mouseModeItem.IsPrivate = true;
					mouseModeItem.GroupIndex = GetHashCode();
					mouseModeItem.Command = ActivateMouseModeCommand;
					mouseModeItem.Glyph = ImageHelper.GetImage("arrow_32x32.png");
					mouseModeItem.GlyphSize = GlyphSize.Large;
					mouseModeItem.AllowGlyphTheming = true;
				}
				return mouseModeItem;
			}
		}
		public BarCheckItem TouchModeItem {
			get {
				if (touchModeItem == null) {
					touchModeItem = new BarCheckItem();
					touchModeItem.Content = RibbonControlLocalizer.Active.GetLocalizedString(RibbonControlStringId.SpacingModeStrings_TouchModeContent);
					touchModeItem.Description = RibbonControlLocalizer.Active.GetLocalizedString(RibbonControlStringId.SpacingModeStrings_TouchModeDescription);
					touchModeItem.IsPrivate = true;
					touchModeItem.GroupIndex = GetHashCode();
					touchModeItem.Command = ActivateTouchModeCommand;
					touchModeItem.Glyph = ImageHelper.GetImage("touch_32x32.png");
					touchModeItem.GlyphSize = GlyphSize.Large;
					touchModeItem.AllowGlyphTheming = true;
				}
				return touchModeItem;
			}
		}
		public SpacingMode SelectedSpacingMode {
			get { return (SpacingMode)GetValue(SelectedSpacingModeProperty); }
			set { SetValue(SelectedSpacingModeProperty, value); }
		}
		public RibbonControl ActualRibbon {
			get { return (RibbonControl)GetValue(ActualRibbonProperty); }
			set { SetValue(ActualRibbonProperty, value); }
		}
		protected ICommand ActivateMouseModeCommand { get; private set; }
		protected ICommand ActivateTouchModeCommand { get; private set; }
		public BarManagerMenuController MenuController { get; set; }
		protected RibbonSpacingSelectorItem SpacingSelectorItem { get { return Link.With(x => x.Item) as RibbonSpacingSelectorItem; } }
		protected RibbonSpacingSelectorItemLink SpacingSelectorLink { get { return Link as RibbonSpacingSelectorItemLink; } }
		#region events
		protected virtual void OnActualRibbonChanged(RibbonControl oldValue) { }
		protected virtual void OnSelectedSpacingModeChanged(SpacingMode oldValue) {
			UpdateModeChecks();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateVisibilityByThemeTreeWalker(ThemeManager.GetTreeWalker(this));
			UpdateActualRibbon();
		}
		protected override void OnPopupChanged(PopupMenuBase oldValue) {
			base.OnPopupChanged(oldValue);
			var popup = Popup as PopupMenu;
			popup.ItemsDisplayMode = PopupMenuItemsDisplayMode.LargeImagesTextDescription;
			popup.ItemLinks.Clear();
			oldValue.With(Interaction.GetBehaviors).Do(x => x.Remove(ControllerBehavior));
			popup.With(Interaction.GetBehaviors).Do(x => x.Add(ControllerBehavior));			
		}
		ControllerBehavior controllerBehavior;
		ControllerBehavior ControllerBehavior {
			get { return controllerBehavior ?? (controllerBehavior = CreateControllerBehavior()); }
		}
		ControllerBehavior CreateControllerBehavior() {
			ControllerBehavior behavior = new ControllerBehavior();
			behavior.ExecutionMode = ActionExecutionMode.OnAssociatedObjectChanged;
			FillMenuItems(behavior.Actions);
			return behavior;
		}
		protected virtual void FillMenuItems(IList<IControllerAction> actions) {
			actions.Add(Header);
			actions.Add(MouseModeItem);
			actions.Add(TouchModeItem);
		}
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(MenuController)); } }
		protected override void RaisePopup() {
			UpdateModeChecks();
			base.RaisePopup();
		}
		#endregion
		void UpdateModeChecks() {
			var themeTreeWalker = ThemeManager.GetTreeWalker(this);
			MouseModeItem.IsChecked = themeTreeWalker != null && !themeTreeWalker.IsTouch;
			TouchModeItem.IsChecked = themeTreeWalker != null && themeTreeWalker.IsTouch;
		}
		protected override void AssignPopupContentControlLinksHolder() {
			ItemsOwner.LinksHolder = Popup as ILinksHolder;
		}		
		protected virtual void ActivateMouseMode() {
			RibbonSpacingModeHelper.UpdateThemeName(ThemeManager.GetTreeWalker(ActualRibbon), Bars.SpacingMode.Mouse);
		}
		protected virtual void ActivateTouchMode() {
			RibbonSpacingModeHelper.UpdateThemeName(ThemeManager.GetTreeWalker(ActualRibbon), Bars.SpacingMode.Touch);
		}
		public virtual void UpdateActualRibbon() {
			ActualRibbon = SpacingSelectorLink.With(x => x.Ribbon ?? RibbonControl.GetRibbon(x)) ?? SpacingSelectorItem.With(x => x.Ribbon ?? RibbonControl.GetRibbon(x))
				?? RibbonControl.GetRibbon(this) ?? LayoutHelper.FindParentObject<RibbonControl>(this) ?? (Window.GetWindow(this) as DXRibbonWindow).With(x => x.Ribbon);
		}
		protected virtual void UpdateVisibilityByThemeTreeWalker(ThemeTreeWalker themeTreeWalker) {
			bool isVisible = RibbonSpacingModeHelper.GetIsTouchSupported(themeTreeWalker);
			if (Item != null)
				Item.IsVisible = isVisible;
		}
#if DEBUGTEST
		public PopupMenu GetPopup() {
			return Popup as PopupMenu;
		}
#endif
	}
}
