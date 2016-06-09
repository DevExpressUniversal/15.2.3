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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Core;
using System;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Controls.Primitives {
	public enum ItemDisplayMode { Default, Content, ContentAndGlyph, Glyph }
	public class veButtonBase : veContentContainer, IClickableControl, ICommandSource {
		#region static
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty GlyphAlignmentProperty;
		static readonly DependencyPropertyKey IsContentActuallyVisiblePropertyKey;
		public static readonly DependencyProperty IsContentActuallyVisibleProperty;
		static readonly DependencyPropertyKey IsGlyphActuallyVisiblePropertyKey;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsGlyphActuallyVisibleProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty HasCommandProperty;
		static readonly DependencyPropertyKey HasCommandPropertyKey;
		public static readonly DependencyProperty GlyphSpaceProperty;
		static veButtonBase() {
			DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(ItemDisplayMode), typeof(veButtonBase), new PropertyMetadata(ItemDisplayMode.Default, new PropertyChangedCallback(OnDisplayModeChanged)));
			GlyphAlignmentProperty = DependencyProperty.Register("GlyphAlignment", typeof(Dock), typeof(veButtonBase), new UIPropertyMetadata(Dock.Left));
			IsGlyphActuallyVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsGlyphActuallyVisible", typeof(bool), typeof(veButtonBase), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsGlyphActuallyVisibleChanged), new CoerceValueCallback(OnCoerceIsGlyphActuallyVisible)));
			IsGlyphActuallyVisibleProperty = IsGlyphActuallyVisiblePropertyKey.DependencyProperty;
			IsContentActuallyVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsContentActuallyVisible", typeof(bool), typeof(veButtonBase), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsContentActuallyVisibleChanged), new CoerceValueCallback(OnCoerceIsContentActuallyVisible)));
			IsContentActuallyVisibleProperty = IsContentActuallyVisiblePropertyKey.DependencyProperty;
			GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), typeof(veButtonBase));
			var dProp = new DependencyPropertyRegistrator<veButtonBase>();
			dProp.Register("Command", ref CommandProperty, (ICommand)null,
				(d, e) => ((veButtonBase)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue));
			dProp.Register("CommandParameter", ref CommandParameterProperty, (object)null);
			dProp.Register("CommandTarget", ref CommandTargetProperty, (IInputElement)null);
			dProp.RegisterReadonly("HasCommand", ref HasCommandPropertyKey, ref HasCommandProperty, false);
			dProp.Register("GlyphSpace", ref GlyphSpaceProperty, 0d);
		}
		private static void OnDisplayModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			veButtonBase tileNavigatorItem = o as veButtonBase;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnDisplayModeChanged((ItemDisplayMode)e.OldValue, (ItemDisplayMode)e.NewValue);
		}
		private static object OnCoerceIsContentActuallyVisible(DependencyObject o, object value) {
			veButtonBase tileNavigatorItem = o as veButtonBase;
			if(tileNavigatorItem != null)
				return tileNavigatorItem.OnCoerceIsContentActuallyVisible((bool)value);
			else
				return value;
		}
		private static void OnIsContentActuallyVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			veButtonBase tileNavigatorItem = o as veButtonBase;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnIsContentActuallyVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static object OnCoerceIsGlyphActuallyVisible(DependencyObject o, object value) {
			veButtonBase tileNavigatorItem = o as veButtonBase;
			if(tileNavigatorItem != null)
				return tileNavigatorItem.OnCoerceIsGlyphActuallyVisible((bool)value);
			else
				return value;
		}
		private static void OnIsGlyphActuallyVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			veButtonBase tileNavigatorItem = o as veButtonBase;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnIsGlyphActuallyVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		protected veButtonBase() {
		}
		protected virtual void OnDisplayModeChanged(ItemDisplayMode oldValue, ItemDisplayMode newValue) {
			CoerceValue(IsContentActuallyVisibleProperty);
			CoerceValue(IsGlyphActuallyVisibleProperty);
		}
		protected virtual bool OnCoerceIsContentActuallyVisible(bool value) {
			return DisplayMode != ItemDisplayMode.Glyph;
		}
		protected virtual void OnIsContentActuallyVisibleChanged(bool oldValue, bool newValue) { }
		protected virtual bool OnCoerceIsGlyphActuallyVisible(bool value) {
			return DisplayMode != ItemDisplayMode.Content;
		}
		protected virtual void OnIsGlyphActuallyVisibleChanged(bool oldValue, bool newValue) { }
		protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue) {
			SetValue(HasCommandPropertyKey, newValue != null);
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
		public new ButtonBaseController Controller { get { return (ButtonBaseController)base.Controller; } }
		protected override ControlControllerBase CreateController() {
			return new ButtonBaseController(this);
		}
		protected virtual void InvokeCommand() {
			CommandHelper.ExecuteCommand(this);
		}
		protected virtual void OnClick() {
			InvokeCommand();
		}
		bool _CanExecute = true;
		private bool CanExecute {
			get { return _CanExecute; }
			set {
				if(value != _CanExecute) {
					_CanExecute = value;
					CoerceValue(IsEnabledProperty);
				}
			}
		}
		protected override bool IsEnabledCore {
			get { return base.IsEnabledCore && CanExecute; }
		}
		public event EventHandler Click {
			add { Controller.Click += value; }
			remove { Controller.Click -= value; }
		}
		public ItemDisplayMode DisplayMode {
			get { return (ItemDisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public Dock GlyphAlignment {
			get { return (Dock)GetValue(GlyphAlignmentProperty); }
			set { SetValue(GlyphAlignmentProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public bool IsContentActuallyVisible {
			get { return (bool)GetValue(IsContentActuallyVisibleProperty); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		public bool HasCommand {
			get { return (bool)GetValue(HasCommandProperty); }
		}
		public double GlyphSpace {
			get { return (double)GetValue(GlyphSpaceProperty); }
			set { SetValue(GlyphSpaceProperty, value); }
		}
		#region IClickableControl Members
		void IClickableControl.OnClick() {
			OnClick();
		}
		#endregion
		#region IClickable Members
		event EventHandler IClickable.Click {
			add { Click += value; }
			remove { Click -= value; }
		}
		#endregion
		public class ButtonBaseController : ClickableController {
			public ButtonBaseController(IControl control)
				: base(control) {
				CaptureMouseOnDown = true;
			}
		}
	}
}
