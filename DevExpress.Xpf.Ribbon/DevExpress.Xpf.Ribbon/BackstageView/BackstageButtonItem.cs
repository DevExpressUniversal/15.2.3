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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	public class BackstageButtonItem : BackstageItem {
		#region static
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty GlyphContainerStyleProperty;
		public static readonly DependencyProperty GlyphStyleProperty;
		static BackstageButtonItem() {
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(BackstageButtonItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGlyphPropertyChanged)));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(BackstageButtonItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandPropertyChanged)));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), typeof(BackstageButtonItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandTargetPropertyChanged)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(BackstageButtonItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandParameterPropertyChanged)));
			GlyphContainerStyleProperty = DependencyPropertyManager.Register("GlyphContainerStyle", typeof(Style), typeof(BackstageButtonItem), new FrameworkPropertyMetadata(null));
			GlyphStyleProperty = DependencyPropertyManager.Register("GlyphStyle", typeof(Style), typeof(BackstageButtonItem), new FrameworkPropertyMetadata(null));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BackstageButtonItem), typeof(BackstageButtonItemControlAutomationPeer), owner => new BackstageButtonItemControlAutomationPeer((BackstageButtonItem)owner));
		}
		protected static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageButtonItem)d).OnGlyphChanged(e.OldValue as ImageSource);
		}
		protected static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageButtonItem)d).CommandChanged((ICommand)e.OldValue);
		}
		protected static void OnCommandTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageButtonItem)d).CommandTargetChanged((IInputElement)e.OldValue);
		}
		protected static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageButtonItem)d).CommandParameterChanged(e.OldValue);
		}
		#endregion
		#region dep props
		[Category("Common")]
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		[Category("Common")]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		[Category("Common")]
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		[Browsable(false)]
		public Style GlyphContainerStyle {
			get { return (Style)GetValue(GlyphContainerStyleProperty); }
			set { SetValue(GlyphContainerStyleProperty, value); }
		}
		[Browsable(false)]
		public Style GlyphStyle {
			get { return (Style)GetValue(GlyphStyleProperty); }
			set { SetValue(GlyphStyleProperty, value); }
		}
		#endregion
		#region events
		#endregion
		public BackstageButtonItem() {
			DefaultStyleKey = typeof(BackstageButtonItem);
			if(this.IsInDesignTool()) {
				Content = "Button";
			}
		}
		protected internal FrameworkElement IconContainer;
		EventHandler CommandCanExecuteChangedEventHandler; 
		protected virtual void OnGlyphChanged(ImageSource oldValue) {
		}
		protected virtual void CommandChanged(ICommand oldValue) {
			UnhookCommand(oldValue);
			HookCommand(Command);
			UpdateActualIsEnabled();
		}
		protected virtual void CommandTargetChanged(IInputElement oldValue) {
			UpdateActualIsEnabled();
		}
		protected virtual void CommandParameterChanged(object oldValue) {
			UpdateActualIsEnabled();
		}
		protected virtual void OnCommandCanExecuteChanged(object sender, EventArgs e) {
			UpdateActualIsEnabled();
		}
		protected virtual bool GetCommandCanExecute() {
			if(Command == null)
				return true;
			RoutedCommand routedCommand = Command as RoutedCommand;
			if(routedCommand != null)
				return routedCommand.CanExecute(CommandParameter, CommandTarget);
			return Command.CanExecute(CommandParameter);
		}
		protected virtual void HookCommand(ICommand command) {
			if(command == null)
				return;
			CommandCanExecuteChangedEventHandler = new EventHandler(OnCommandCanExecuteChanged);
			command.CanExecuteChanged += CommandCanExecuteChangedEventHandler;
		}
		protected virtual void UnhookCommand(ICommand command) {
			if(command == null)
				return;
			command.CanExecuteChanged -= OnCommandCanExecuteChanged;
			CommandCanExecuteChangedEventHandler = null;
		}
		protected virtual void ExecuteCommand() {
			if(Command == null)
				return;
			RoutedCommand routedCommand = Command as RoutedCommand;
			if(routedCommand != null) {
				routedCommand.Execute(CommandParameter, CommandTarget);
				return;
			}
			Command.Execute(CommandParameter);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void UpdateActualIsEnabled() {		 
			ActualIsEnabled = IsEnabled && GetCommandCanExecute();
		}
		protected internal override void OnClick() {
			base.OnClick();
			if (ActualIsEnabled) {
				Dispatcher.BeginInvoke(new Action(() => ExecuteCommand()), null);
				if (Backstage != null)
					Backstage.Close();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IconContainer = GetTemplateChild("PART_Icon") as FrameworkElement;
		}
	}
}
