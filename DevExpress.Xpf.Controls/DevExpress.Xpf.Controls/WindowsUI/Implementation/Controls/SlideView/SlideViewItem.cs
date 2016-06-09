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
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using DevExpress.Xpf.Controls.Primitives;
#if SILVERLIGHT
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.WindowsUI {
#if !SILVERLIGHT
#endif
	[TemplatePart(Name = "PART_Header", Type = typeof(SlideViewItemHeader))]
	[TemplatePart(Name = "PART_HeaderPlaceHolder", Type = typeof(FrameworkElement))]
	public class SlideViewItem : veHeaderedContentControl, IClickableControl, ICommandSource {
		#region static
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty IsHeaderInteractiveProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty IsHeaderStickyProperty;
		public static readonly DependencyProperty InteractiveHeaderTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty HeaderPlaceholderHeightProperty;
		static SlideViewItem() {
			var dProp = new DependencyPropertyRegistrator<SlideViewItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Command", ref CommandProperty, (ICommand)null, OnCommandChanged);
			dProp.Register("CommandParameter", ref CommandParameterProperty, (object)null);
			dProp.Register("CommandTarget", ref CommandTargetProperty, (IInputElement)null);
			dProp.Register("IsHeaderInteractive", ref IsHeaderInteractiveProperty, false);
			dProp.Register("IsHeaderSticky", ref IsHeaderStickyProperty, true);
			dProp.Register("InteractiveHeaderTemplate", ref InteractiveHeaderTemplateProperty, (DataTemplate)null);
			dProp.Register("HeaderPlaceholderHeight", ref HeaderPlaceholderHeightProperty, double.NaN);
		}
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SlideViewItem slideViewItem = d as SlideViewItem;
			if(slideViewItem != null) slideViewItem.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		#endregion
#if SILVERLIGHT
		public SlideViewItem() {
			DefaultStyleKey = typeof(SlideViewItem);
		}
		SlideViewItemsPanel splitPanel;
		SlideViewItemsPanel SplitPanel {
			get {
				if(splitPanel == null) {
					return Parent as SlideViewItemsPanel ?? VisualTreeHelper.GetParent(this) as SlideViewItemsPanel;
				}
				return splitPanel;
			}
		}
#endif
		SlideViewItemHeader PartSlideViewItemHeader;
		FrameworkElement PartHeaderPlaceholder;
		protected internal SlideView Owner { get; internal set; }
		protected override void ClearTemplateChildren() {
			if(PartSlideViewItemHeader != null) {
				PartSlideViewItemHeader.Owner = null;
#if !SILVERLIGHT
				PartSlideViewItemHeader.ClearValue(UIElement.IsManipulationEnabledProperty);
#endif
			}
			if(PartHeaderPlaceholder != null)
				PartHeaderPlaceholder.ClearValue(FrameworkElement.HeightProperty);
			base.ClearTemplateChildren();
		}
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartSlideViewItemHeader = GetTemplateChild("PART_Header") as SlideViewItemHeader;
			PartHeaderPlaceholder = GetTemplateChild("PART_HeaderPlaceHolder") as FrameworkElement;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartSlideViewItemHeader != null) {
				PartSlideViewItemHeader.Owner = this;
#if !SILVERLIGHT
				if(Owner != null)
					PartSlideViewItemHeader.SetBinding(UIElement.IsManipulationEnabledProperty, new Binding("IsManipulationEnabled") { Source = Owner });
#endif
				PartSlideViewItemHeader.SetBinding(SlideViewItemHeader.IsStickyProperty, new Binding("IsHeaderSticky") { Source = this });
				PartSlideViewItemHeader.IsEnabled = CanExecute;
			}
			if(PartHeaderPlaceholder != null) {
				PartHeaderPlaceholder.SetBinding(HeightProperty, new Binding("HeaderPlaceholderHeight") { Source = this });
				PartHeaderPlaceholder.SetBinding(VisibilityProperty, new Binding("Orientation") { Source = Owner, Converter = new OrientationToVisibilityConverter() });
			}
		}
#if SILVERLIGHT
		protected override void OnUnloaded() {
			splitPanel = null;
			base.OnUnloaded();
		}
#endif
		internal void OnScrollChanged(double horizontalOffset, double headerOffset) {
			if(PartSlideViewItemHeader == null) return;
#if SILVERLIGHT
			double locationX = GetRelativeLocation().X - horizontalOffset;
#else
			double locationX = horizontalOffset;
#endif
			PartSlideViewItemHeader.SetRelativeLocation(locationX, headerOffset);
		}
#if SILVERLIGHT
		Point GetRelativeLocation() {
			return SplitPanel != null ? SplitPanel.GetChildRect(this).Location() : new Point();
		}
#endif
		protected virtual void OnClick() {
			EventHandler handler = Click;
			if(handler != null)
				handler(this, EventArgs.Empty);
			CommandHelper.ExecuteCommand(this);
		}
		protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue) {
			if(oldValue != null) {
				UnhookCommand(oldValue);
			}
			if(newValue != null) {
				HookCommand(newValue);
			}
		}
		private void UnhookCommand(ICommand command) {
			command.CanExecuteChanged -= OnCanExecuteChanged;
			UpdateCanExecute();
		}
		private void HookCommand(ICommand command) {
			command.CanExecuteChanged += OnCanExecuteChanged;
			UpdateCanExecute();
		}
		private void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		private void UpdateCanExecute() {
			CanExecute = Command != null ? CommandHelper.CanExecuteCommand(this) : true;
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new SlideViewItemAutomationPeer(this);
		}
		#endregion
		bool _CanExecute = true;
		private bool CanExecute {
			get { return _CanExecute; }
			set {
				if(value != _CanExecute) {
					_CanExecute = value;
					PartSlideViewItemHeader.Do(x => x.IsEnabled = value);
				}
			}
		}
		public event EventHandler Click;
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		public bool IsHeaderInteractive {
			get { return (bool)GetValue(IsHeaderInteractiveProperty); }
			set { SetValue(IsHeaderInteractiveProperty, value); }
		}
		internal bool IsHeaderSticky {
			get { return (bool)GetValue(IsHeaderStickyProperty); }
			set { SetValue(IsHeaderStickyProperty, value); }
		}
		public DataTemplate InteractiveHeaderTemplate {
			get { return (DataTemplate)GetValue(InteractiveHeaderTemplateProperty); }
			set { SetValue(InteractiveHeaderTemplateProperty, value); }
		}
		#region ISlideViewItem Members
		void IClickableControl.OnClick() {
			OnClick();
			if(Owner is SlideView) {
				((SlideView)Owner).OnItemClick(this);
			}
		}
		#endregion
	}
}
