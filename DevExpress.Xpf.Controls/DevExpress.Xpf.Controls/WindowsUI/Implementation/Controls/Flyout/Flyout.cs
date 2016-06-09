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

using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Internal.Flyout;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
namespace DevExpress.Xpf.WindowsUI {
	[ContentProperty("Content")]
	public class Flyout : FlyoutBase {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty CloseOnCommandExecuteProperty;
		static Flyout() {
			var dProp = new DependencyPropertyRegistrator<Flyout>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Content", ref ContentProperty, (object)null, OnContentChanged);
			dProp.Register("ContentTemplate", ref ContentTemplateProperty, (DataTemplate)null);
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.Register("Command", ref CommandProperty, (ICommand)null);
			dProp.Register("CommandParameter", ref CommandParameterProperty, (object)null);
			dProp.Register("CommandTarget", ref CommandTargetProperty, (IInputElement)null);
			dProp.Register("CloseOnCommandExecute", ref CloseOnCommandExecuteProperty, true);
		}
		private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Flyout)d).OnContentChanged(e.OldValue, e.NewValue);
		}
		#endregion
		public Flyout() { }
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			if(oldValue != null) RemoveLogicalChild(oldValue);
			if(newValue != null) AddLogicalChild(newValue);
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
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
		public bool CloseOnCommandExecute {
			get { return (bool)GetValue(CloseOnCommandExecuteProperty); }
			set { SetValue(CloseOnCommandExecuteProperty, value); }
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return Content != null ? new object[] { Content }.GetEnumerator() : base.LogicalChildren;
			}
		}
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class FlyoutContentPresenter : ContentControl, ICommandSource {
		#region static
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty HasCommandProperty;
		static readonly DependencyPropertyKey HasCommandPropertyKey;
		static FlyoutContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<FlyoutContentPresenter>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Command", ref CommandProperty, (ICommand)null,
				(d, e) => ((FlyoutContentPresenter)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue));
			dProp.Register("CommandParameter", ref CommandParameterProperty, (object)null);
			dProp.Register("CommandTarget", ref CommandTargetProperty, (IInputElement)null);
			dProp.RegisterReadonly("HasCommand", ref HasCommandPropertyKey, ref HasCommandProperty, false);
		}
		#endregion
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
		public FlyoutContentPresenter() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(FlyoutContentPresenter);
#endif
		}
		protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue) {
			SetValue(HasCommandPropertyKey, newValue != null);
		}
		FlyoutCommandButton PartCommandButton;
		public override void OnApplyTemplate() {
			if(PartCommandButton != null) WindowsUI.Flyout.SetFlyout(PartCommandButton, null);
			base.OnApplyTemplate();
			PartCommandButton = GetTemplateChild("PART_CommandButton") as FlyoutCommandButton;
			if(PartCommandButton != null) WindowsUI.Flyout.SetFlyout(PartCommandButton, WindowsUI.Flyout.GetFlyout(this));
		}
	}
	public class FlyoutCommandButton : veButtonBase {
		public FlyoutCommandButton() {
			DefaultStyleKey = typeof(FlyoutCommandButton);
		}
		protected override void InvokeCommand() {
			base.InvokeCommand();
			WindowsUI.Flyout flyout = FlyoutBase.GetFlyout(this) as WindowsUI.Flyout;
			if(flyout != null && flyout.CloseOnCommandExecute) flyout.IsOpen = false;
		}
	}
}
