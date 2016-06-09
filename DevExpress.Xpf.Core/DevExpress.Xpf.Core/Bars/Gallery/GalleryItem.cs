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
using System.Windows.Media;
using System.ComponentModel;
using System;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Caption")]
	public class GalleryItem : FrameworkContentElement, ICommandSource {
		#region static               
		private static readonly object clickEventHandler;
		private static readonly object checkedEventHandler;
		private static readonly object uncheckedEventHandler;
		private static readonly object hoverEventHandler;
		private static readonly object leaveEventHandler;
		private static readonly object enterEventHandler;
		private static readonly object commandCanExecuteEventHandler;
		public static readonly DependencyProperty HintProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty IsCheckedProperty;			
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty DescriptionProperty;
		public static readonly DependencyProperty HoverGlyphProperty;
		public static readonly DependencyProperty GroupProperty;
		protected internal static readonly DependencyPropertyKey GroupPropertyKey;
		public static readonly DependencyProperty SuperTipProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		static GalleryItem() {
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(GalleryItem), new FrameworkPropertyMetadata(null));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(object), typeof(GalleryItem), new FrameworkPropertyMetadata(null, (d, e) => ((GalleryItem)d).OnCaptionChanged(e.OldValue, e.NewValue)));
			DescriptionProperty = DependencyPropertyManager.Register("Description", typeof(object), typeof(GalleryItem), new FrameworkPropertyMetadata(null, (d, e) => ((GalleryItem)d).OnDescriptionChanged(e.OldValue, e.NewValue)));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(true));
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));
			HoverGlyphProperty = DependencyPropertyManager.Register("HoverGlyph", typeof(ImageSource), typeof(GalleryItem), new FrameworkPropertyMetadata(null));
			HintProperty = DependencyPropertyManager.Register("Hint", typeof(object), typeof(GalleryItem), new FrameworkPropertyMetadata(null));
			GroupPropertyKey = DependencyPropertyManager.RegisterReadOnly("Group", typeof(GalleryItemGroup), typeof(GalleryItem), new FrameworkPropertyMetadata(null, (d,e)=>((GalleryItem)d).OnGroupChanged(e.OldValue as GalleryItemGroup)));
			GroupProperty = GroupPropertyKey.DependencyProperty;
			SuperTipProperty = DependencyPropertyManager.Register("SuperTip", typeof(SuperTip), typeof(GalleryItem), new FrameworkPropertyMetadata(null));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(GalleryItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandPropertyChanged)));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), typeof(GalleryItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandTargetPropertyChanged)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(GalleryItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandParameterPropertyChanged)));
			clickEventHandler = new object();
			checkedEventHandler = new object();
			uncheckedEventHandler = new object();
			hoverEventHandler = new object();
			leaveEventHandler = new object();
			enterEventHandler = new object();
			commandCanExecuteEventHandler = new object();
		}
		protected static void OnIsCheckedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryItem)obj).OnIsCheckedChanged((bool)e.OldValue);
		}
		protected static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryItem)d).CommandChanged((ICommand)e.OldValue);
		}
		protected static void OnCommandTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryItem)d).CommandTargetChanged((IInputElement)e.OldValue);
		}
		protected static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryItem)d).CommandParameterChanged(e.OldValue);
		}
		#endregion
		#region dep prop
		[TypeConverter(typeof(ObjectConverter)),
#if !SL
	DevExpressXpfCoreLocalizedDescription("GalleryItemHint")
#else
	Description("")
#endif
]
		public object Hint {
			get { return (object)GetValue(HintProperty); }
			set { SetValue(HintProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemHoverGlyph")]
#endif
		public ImageSource HoverGlyph {
			get { return (ImageSource)GetValue(HoverGlyphProperty); }
			set { SetValue(HoverGlyphProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemIsChecked")]
#endif
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyph")]
#endif
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		[TypeConverter(typeof(ObjectConverter)),
#if !SL
	DevExpressXpfCoreLocalizedDescription("GalleryItemCaption")
#else
	Description("")
#endif
]
		public object Caption {
			get { return (object)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		[TypeConverter(typeof(ObjectConverter)),
#if !SL
	DevExpressXpfCoreLocalizedDescription("GalleryItemDescription")
#else
	Description("")
#endif
]
		public object Description {
			get { return (object)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroup")]
#endif
		public GalleryItemGroup Group {
			get { return (GalleryItemGroup)GetValue(GroupProperty); }
			protected internal set { this.SetValue(GroupPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemSuperTip")]
#endif
		public SuperTip SuperTip {
			get { return (SuperTip)GetValue(SuperTipProperty); }
			set { SetValue(SuperTipProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCommand")]
#endif
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCommandTarget")]
#endif
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCommandParameter")]
#endif
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		#endregion
		EventHandlerList events;
		internal event EventHandler CaptionChanged;
		internal event EventHandler DescriptionChanged;
		protected EventHandlerList Events {
			get {
				if(events == null) events = new EventHandlerList();
				return events;
			}
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		protected internal GalleryItem ClonedFrom { get; set; }
		protected internal object DataItem { get; set; }
		#region events
		public event EventHandler Checked {
			add { Events.AddHandler(checkedEventHandler, value); }
			remove { Events.RemoveHandler(checkedEventHandler, value); }
		}
		public event EventHandler Unchecked {
			add { Events.AddHandler(uncheckedEventHandler, value); }
			remove { Events.RemoveHandler(uncheckedEventHandler, value); }
		}
		public event EventHandler Click {
			add { Events.AddHandler(clickEventHandler, value); }
			remove { Events.RemoveHandler(clickEventHandler, value); }
		}
		public event EventHandler Hover {
			add { Events.AddHandler(hoverEventHandler, value); }
			remove { Events.RemoveHandler(hoverEventHandler, value); }
		}
		public event EventHandler Leave {
			add { Events.AddHandler(leaveEventHandler, value); }
			remove { Events.RemoveHandler(leaveEventHandler, value); }
		}
		public event EventHandler Enter {
			add { Events.AddHandler(enterEventHandler, value); }
			remove { Events.RemoveHandler(enterEventHandler, value); }
		}
		public event EventHandler CommandCanExecuteChanged {
			add { Events.AddHandler(commandCanExecuteEventHandler, value); }
			remove { Events.RemoveHandler(commandCanExecuteEventHandler, value); }
		}
		private EventHandler commandCanExecuteChangedEventHandler;
		protected internal virtual void OnChecked() {
			if(Group != null && Group.Gallery != null) {
				switch(Group.Gallery.ItemCheckMode) {
					case GalleryItemCheckMode.Single:
						Group.Gallery.UncheckAllItems(this);
						break;
					case GalleryItemCheckMode.SingleInGroup:
						Group.UncheckAllItems(this);
						break;
				}
			}
			RaiseEventByHandler(checkedEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null) Group.Gallery.OnItemChecked(new GalleryItemEventArgs(this));
		}
		protected internal virtual void OnUnchecked() {
			RaiseEventByHandler(uncheckedEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null) Group.Gallery.OnItemUnchecked(new GalleryItemEventArgs(this));
		}
		protected internal virtual void OnClick(GalleryItemControl itemControl) {
			RaiseEventByHandler(clickEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null) Group.Gallery.OnItemClick(new GalleryItemEventArgs(this, itemControl));
			ExecuteCommand();
		}
		protected internal virtual void OnHover(GalleryItemControl itemControl) {
			RaiseEventByHandler(hoverEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null) Group.Gallery.OnItemHover(new GalleryItemEventArgs(this, itemControl));
		}
		protected internal virtual void OnLeave(GalleryItemControl itemControl) {
			RaiseEventByHandler(leaveEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null) Group.Gallery.OnItemLeave(new GalleryItemEventArgs(this, itemControl));
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		protected internal virtual void OnEnter(GalleryItemControl itemControl) {
			RaiseEventByHandler(enterEventHandler, EventArgs.Empty);
			if(Group != null && Group.Gallery != null)
				Group.Gallery.OnItemEnter(new GalleryItemEventArgs(this, itemControl));
		}
		#endregion
		void CloneEvent(object eventHandler, GalleryItem targetObject) {
			EventHandler baseEventHandler = Events[eventHandler] as EventHandler;
			EventHandler targetEventHandler = targetObject.Events[eventHandler] as EventHandler;
			if(targetEventHandler == null && baseEventHandler != null) targetObject.Events.AddHandler(eventHandler, baseEventHandler);
		}
		protected virtual void OnIsCheckedChanged(bool oldValue) {
			if(Group == null || Group.Gallery == null)
				return;
			if(IsChecked)
				OnChecked();
			else
				OnUnchecked();
		}
		protected virtual void CommandChanged(ICommand oldValue) {
			UnhookCommand(oldValue);
			HookCommand(Command);
			RaiseEventByHandler(commandCanExecuteEventHandler, EventArgs.Empty);
		}
		protected virtual void CommandTargetChanged(IInputElement oldValue) {
			RaiseEventByHandler(commandCanExecuteEventHandler, EventArgs.Empty);
		}
		protected virtual void CommandParameterChanged(object oldValue) {
			RaiseEventByHandler(commandCanExecuteEventHandler, EventArgs.Empty);
		}
		protected virtual void OnGroupChanged(GalleryItemGroup oldGroup) {
			if(Group != null && Group.Gallery != null && IsChecked) {
				Group.Gallery.UpdateCheckedItems();
			}
		}
		protected virtual void OnCaptionChanged(object oldValue, object newValue) {
			if(CaptionChanged != null) {
				CaptionChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCommandCanExecuteChanged(object sender, EventArgs e) {
			RaiseEventByHandler(commandCanExecuteEventHandler, EventArgs.Empty);
		}
		protected virtual void OnDescriptionChanged(object oldValue, object newValue) {
			if(DescriptionChanged != null) {
				DescriptionChanged(this, EventArgs.Empty);
			}
		}
		protected internal virtual bool GetCommandCanExecute() {
			if(Command == null)
				return true;
			RoutedCommand routedCommand = Command as RoutedCommand;
			if(routedCommand != null)
				return routedCommand.CanExecute(CommandParameter, CommandTarget ?? ItemControl);
			return Command.CanExecute(CommandParameter);
		}
		protected virtual void HookCommand(ICommand command) {
			if(command == null)
				return;
			commandCanExecuteChangedEventHandler = new EventHandler(OnCommandCanExecuteChanged);
			command.CanExecuteChanged += commandCanExecuteChangedEventHandler;
		}
		protected virtual void UnhookCommand(ICommand command) {
			if(command == null)
				return;
			if(commandCanExecuteEventHandler != null)
				command.CanExecuteChanged -= commandCanExecuteChangedEventHandler;
			commandCanExecuteChangedEventHandler = null;
		}
		protected virtual void ExecuteCommand() {
			if(Command == null)
				return;
			RoutedCommand routedCommand = Command as RoutedCommand;
			if(routedCommand != null) {
				routedCommand.Execute(CommandParameter, CommandTarget ?? ItemControl);
				return;
			}
			Command.Execute(CommandParameter);
		}
		protected internal GalleryItemControl ItemControl {
			get {
				return
					Group != null &&
					Group.GroupControl != null &&
					Group.GroupControl.ItemContainerGenerator != null
					? Group.GroupControl.ItemContainerGenerator.ContainerFromItem(this) as GalleryItemControl
					: null;
			}
		}
		public GalleryItem CloneWithEvents() {
			GalleryItem clone = CloneWithoutEvents();
			CloneEvent(checkedEventHandler, clone);
			CloneEvent(uncheckedEventHandler, clone);
			CloneEvent(clickEventHandler, clone);
			CloneEvent(hoverEventHandler, clone);
			CloneEvent(leaveEventHandler, clone);
			CloneEvent(enterEventHandler, clone);
			clone.ClonedFrom = this;
			return clone;
		}
		public GalleryItem CloneWithoutEvents() {
			GalleryItem clone = new GalleryItem();
			clone.Glyph = Glyph;
			clone.IsChecked = IsChecked;
			clone.IsVisible = IsVisible;
			clone.Caption = Caption;
			clone.Description = Description;
			clone.HoverGlyph = HoverGlyph;
			clone.Hint = Hint;
			clone.SuperTip = SuperTip;
			clone.ClonedFrom = this;
			return clone;
		}
	}
}
