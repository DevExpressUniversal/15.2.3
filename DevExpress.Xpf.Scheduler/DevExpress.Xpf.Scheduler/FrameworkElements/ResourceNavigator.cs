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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ResourceNavigatorControl : Control, INotifyPropertyChanged, IVisualElement {
		public static readonly DependencyProperty SchedulerControlProperty;
		public static readonly DependencyProperty FilteredResourcesCountProperty;
		public static readonly DependencyProperty ResourcesPerPageProperty;
		public static readonly DependencyProperty FirstVisibleResourceIndexProperty;
		Dictionary<SchedulerCommandId, SchedulerUICommand> commands;
		#region ButtonFirstStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonFirstStyle")]
#endif
public Style ButtonFirstStyle { get { return (Style)GetValue(ButtonFirstStyleProperty); } set { SetValue(ButtonFirstStyleProperty, value); } }
		public static readonly DependencyProperty ButtonFirstStyleProperty = CreateButtonFirstStyleProperty();
		static DependencyProperty CreateButtonFirstStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonFirstStyle", null);
		}
		#endregion
		#region ButtonPrevPageStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonPrevPageStyle")]
#endif
public Style ButtonPrevPageStyle { get { return (Style)GetValue(ButtonPrevPageStyleProperty); } set { SetValue(ButtonPrevPageStyleProperty, value); } }
		public static readonly DependencyProperty ButtonPrevPageStyleProperty = CreateButtonPrevPageStyleProperty();
		static DependencyProperty CreateButtonPrevPageStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonPrevPageStyle", null);
		}
		#endregion
		#region ButtonPrevStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonPrevStyle")]
#endif
public Style ButtonPrevStyle { get { return (Style)GetValue(ButtonPrevStyleProperty); } set { SetValue(ButtonPrevStyleProperty, value); } }
		public static readonly DependencyProperty ButtonPrevStyleProperty = CreateButtonPrevStyleProperty();
		static DependencyProperty CreateButtonPrevStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonPrevStyle", null);
		}
		#endregion
		#region ButtonNextStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonNextStyle")]
#endif
public Style ButtonNextStyle { get { return (Style)GetValue(ButtonNextStyleProperty); } set { SetValue(ButtonNextStyleProperty, value); } }
		public static readonly DependencyProperty ButtonNextStyleProperty = CreateButtonNextStyleProperty();
		static DependencyProperty CreateButtonNextStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonNextStyle", null);
		}
		#endregion
		#region ButtonNextPageStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonNextPageStyle")]
#endif
public Style ButtonNextPageStyle { get { return (Style)GetValue(ButtonNextPageStyleProperty); } set { SetValue(ButtonNextPageStyleProperty, value); } }
		public static readonly DependencyProperty ButtonNextPageStyleProperty = CreateButtonNextPageStyleProperty();
		static DependencyProperty CreateButtonNextPageStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonNextPageStyle", null);
		}
		#endregion
		#region ButtonLastStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonLastStyle")]
#endif
public Style ButtonLastStyle { get { return (Style)GetValue(ButtonLastStyleProperty); } set { SetValue(ButtonLastStyleProperty, value); } }
		public static readonly DependencyProperty ButtonLastStyleProperty = CreateButtonLastStyleProperty();
		static DependencyProperty CreateButtonLastStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonLastStyle", null);
		}
		#endregion
		#region ButtonIncCountStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonIncCountStyle")]
#endif
public Style ButtonIncCountStyle { get { return (Style)GetValue(ButtonIncCountStyleProperty); } set { SetValue(ButtonIncCountStyleProperty, value); } }
		public static readonly DependencyProperty ButtonIncCountStyleProperty = CreateButtonIncCountStyleProperty();
		static DependencyProperty CreateButtonIncCountStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonIncCountStyle", null);
		}
		#endregion
		#region ButtonDecCountStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlButtonDecCountStyle")]
#endif
public Style ButtonDecCountStyle { get { return (Style)GetValue(ButtonDecCountStyleProperty); } set { SetValue(ButtonDecCountStyleProperty, value); } }
		public static readonly DependencyProperty ButtonDecCountStyleProperty = CreateButtonDecCountStyleProperty();
		static DependencyProperty CreateButtonDecCountStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, Style>("ButtonDecCountStyle", null);
		}
		#endregion
		protected internal Dictionary<SchedulerCommandId, SchedulerUICommand> Commands { get { return commands; } }
		static ResourceNavigatorControl() {
			SchedulerControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue), null);
			FilteredResourcesCountProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, double>("FilteredResourcesCount", 0, (d, e) => d.OnFilteredResourcesCountChanged(e.OldValue, e.NewValue), null);
			ResourcesPerPageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, double>("ResourcesPerPage", 0, (d, e) => d.OnResourcesPerPageChanged(e.OldValue, e.NewValue), null);
			FirstVisibleResourceIndexProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, double>("FirstVisibleResourceIndex", 0, (d, e) => d.OnFirstVisibleResourceIndexChanged(e.OldValue, e.NewValue), null);
		}
		public ResourceNavigatorControl() {
			DefaultStyleKey = typeof(ResourceNavigatorControl);
			this.commands = new Dictionary<SchedulerCommandId, SchedulerUICommand>();
			RegisterCommands();
			this.Loaded += OnLoaded;
			this.Unloaded += OnUnloaded;
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlMaximum")]
#endif
		public double Maximum { get { return CalcMaximum(); } }
		private double CalcMaximum() {
			return Math.Max(0, FilteredResourcesCount - ResourcesPerPage);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlViewportSize")]
#endif
		public double ViewportSize { get { return CalcViewPortSize(); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlScrollBarEnabled")]
#endif
		public bool ScrollBarEnabled { get { return Maximum > 0; } }
		private double CalcViewPortSize() {
			return ResourcesPerPage;
		}
		private void OnFirstVisibleResourceIndexChanged(double oldVal, double newVal) {
			if (SchedulerControl == null)
				return;
			double delta = newVal - oldVal;
			int oldIndex = SchedulerControl.ActiveView.InnerView.FirstVisibleResourceIndex;
			int newIndex = (int)(delta > 0 ? Math.Floor(newVal) : Math.Ceiling(newVal));
			if (newIndex != oldIndex)
				SchedulerControl.ActiveView.InnerView.FirstVisibleResourceIndex = newIndex;
		}
		private void OnResourcesPerPageChanged(double oldVal, double newVal) {
			RaiseOnPropertyChange("Maximum");
			RaiseOnPropertyChange("ViewportSize");
			RaiseOnPropertyChange("ScrollBarEnabled");
		}
		private void OnFilteredResourcesCountChanged(double oldVal, double newVal) {
			RaiseOnPropertyChange("Maximum");
			RaiseOnPropertyChange("ViewportSize");
			RaiseOnPropertyChange("ScrollBarEnabled");
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeControlEvents(SchedulerControl);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeControlEvents(SchedulerControl);
			SubscribeControlEvents(SchedulerControl);
			if (SchedulerControl != null)
				InitScrollBar(SchedulerControl);
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaiseOnPropertyChange(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region Properties
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlSchedulerControl")]
#endif
public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlFilteredResourcesCount")]
#endif
public double FilteredResourcesCount {
			get { return (double)GetValue(FilteredResourcesCountProperty); }
			set { SetValue(FilteredResourcesCountProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlResourcesPerPage")]
#endif
public double ResourcesPerPage {
			get { return (double)GetValue(ResourcesPerPageProperty); }
			set { SetValue(ResourcesPerPageProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlFirstVisibleResourceIndex")]
#endif
public double FirstVisibleResourceIndex {
			get { return (double)GetValue(FirstVisibleResourceIndexProperty); }
			set { SetValue(FirstVisibleResourceIndexProperty, value); }
		}
		#endregion
		#region Commands Properties
		#region CommandFirst
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandFirst")]
#endif
public SchedulerUICommand CommandFirst {
			get { return (SchedulerUICommand)GetValue(CommandFirstProperty); }
			set { SetValue(CommandFirstProperty, value); }
		}
		public static readonly DependencyProperty CommandFirstProperty = CreateCommandFirstProperty();
		static DependencyProperty CreateCommandFirstProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandFirst", null);
		}
		#endregion
		#region CommandPrevPage
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandPrevPage")]
#endif
public SchedulerUICommand CommandPrevPage {
			get { return (SchedulerUICommand)GetValue(CommandPrevPageProperty); }
			set { SetValue(CommandPrevPageProperty, value); }
		}
		public static readonly DependencyProperty CommandPrevPageProperty = CreateCommandPrevPageProperty();
		static DependencyProperty CreateCommandPrevPageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandPrevPage", null);
		}
		#endregion
		#region CommandPrev
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandPrev")]
#endif
public SchedulerUICommand CommandPrev {
			get { return (SchedulerUICommand)GetValue(CommandPrevProperty); }
			set { SetValue(CommandPrevProperty, value); }
		}
		public static readonly DependencyProperty CommandPrevProperty = CreateCommandPrevProperty();
		static DependencyProperty CreateCommandPrevProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandPrev", null);
		}
		#endregion
		#region CommandNext
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandNext")]
#endif
public SchedulerUICommand CommandNext {
			get { return (SchedulerUICommand)GetValue(CommandNextProperty); }
			set { SetValue(CommandNextProperty, value); }
		}
		public static readonly DependencyProperty CommandNextProperty = CreateCommandNextProperty();
		static DependencyProperty CreateCommandNextProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandNext", null);
		}
		#endregion
		#region CommandNextPage
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandNextPage")]
#endif
public SchedulerUICommand CommandNextPage {
			get { return (SchedulerUICommand)GetValue(CommandNextPageProperty); }
			set { SetValue(CommandNextPageProperty, value); }
		}
		public static readonly DependencyProperty CommandNextPageProperty = CreateCommandNextPageProperty();
		static DependencyProperty CreateCommandNextPageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandNextPage", null);
		}
		#endregion
		#region CommandLast
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandLast")]
#endif
public SchedulerUICommand CommandLast {
			get { return (SchedulerUICommand)GetValue(CommandLastProperty); }
			set { SetValue(CommandLastProperty, value); }
		}
		public static readonly DependencyProperty CommandLastProperty = CreateCommandLastProperty();
		static DependencyProperty CreateCommandLastProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandLast", null);
		}
		#endregion
		#region CommandIncCount
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandIncCount")]
#endif
public SchedulerUICommand CommandIncCount {
			get { return (SchedulerUICommand)GetValue(CommandIncCountProperty); }
			set { SetValue(CommandIncCountProperty, value); }
		}
		public static readonly DependencyProperty CommandIncCountProperty = CreateCommandIncCountProperty();
		static DependencyProperty CreateCommandIncCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandIncCount", null);
		}
		#endregion
		#region CommandDecCount
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceNavigatorControlCommandDecCount")]
#endif
public SchedulerUICommand CommandDecCount {
			get { return (SchedulerUICommand)GetValue(CommandDecCountProperty); }
			set { SetValue(CommandDecCountProperty, value); }
		}
		public static readonly DependencyProperty CommandDecCountProperty = CreateCommandDecCountProperty();
		static DependencyProperty CreateCommandDecCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorControl, SchedulerUICommand>("CommandDecCount", null);
		}
		#endregion
		#endregion
		void OnSchedulerControlChanged(SchedulerControl oldControl, SchedulerControl newControl) {
			UnsubscribeControlEvents(oldControl);
			InitScrollBar(newControl);
			SubscribeControlEvents(newControl);
		}
		void InitScrollBar(SchedulerControl control) {
			if (control == null)
				return;
			SchedulerViewBase view = control.ActiveView;
			ResourcesPerPage = view.InnerView.ActualResourcesPerPage;
			FirstVisibleResourceIndex = view.InnerView.FirstVisibleResourceIndex;
			FilteredResourcesCount = view.FilteredResources.Count;
		}
		protected void SubscribeControlEvents(SchedulerControl control) {
			if (control == null)
				return;
			SchedulerControl.ResourceScrollBarValueChanged += new EventHandler(OnSchedulerControlResourceScrollBarValueChanged);
			SchedulerControl.ActiveView.InnerView.Changed += new SchedulerControlStateChangedEventHandler(InnerView_Changed);
		}
		protected void UnsubscribeControlEvents(SchedulerControl control) {
			if (control == null)
				return;
			control.ResourceScrollBarValueChanged -= new EventHandler(OnSchedulerControlResourceScrollBarValueChanged);
			control.ActiveView.InnerView.Changed -= new SchedulerControlStateChangedEventHandler(InnerView_Changed);
		}
		void InnerView_Changed(object sender, XtraScheduler.Native.SchedulerControlStateChangedEventArgs e) {
			if (e.Change == SchedulerControlChangeType.ResourcesPerPageChanged)
				ResourcesPerPage = SchedulerControl.ActiveView.InnerView.ResourcesPerPage;
			if (e.Change == SchedulerControlChangeType.FirstVisibleResourceIndexChanged)
				FirstVisibleResourceIndex = SchedulerControl.ActiveView.InnerView.FirstVisibleResourceIndex;
			InvalidateCommandCanExecuteChanged();
		}
		void FilteredResources_CollectionChanged(object sender, CollectionChangedEventArgs<Resource> e) {
			FilteredResourcesCount = SchedulerControl.ActiveView.InnerView.FilteredResources.Count;
		}
		void OnSchedulerControlResourceScrollBarValueChanged(object sender, EventArgs e) {			
			InitScrollBar(SchedulerControl);
			InvalidateCommandCanExecuteChanged();
		}
		protected internal virtual void RegisterCommands() {
			CommandFirst = CreateCommand(SchedulerCommandId.NavigateFirstResource);
			CommandPrevPage = CreateCommand(SchedulerCommandId.NavigateResourcePageBackward);
			CommandPrev = CreateCommand(SchedulerCommandId.NavigatePrevResource);
			CommandNext = CreateCommand(SchedulerCommandId.NavigateNextResource);
			CommandNextPage = CreateCommand(SchedulerCommandId.NavigateResourcePageForward);
			CommandLast = CreateCommand(SchedulerCommandId.NavigateLastResource);
			CommandIncCount = CreateCommand(SchedulerCommandId.IncrementResourcePerPageCount);
			CommandDecCount = CreateCommand(SchedulerCommandId.DecrementResourcePerPageCount);
	   }
		protected internal virtual SchedulerUICommand CreateCommand(SchedulerCommandId id) {
			SchedulerUICommand result = SchedulerUICommand.CreateInstance(id, false);
			Commands.Add(result.CommandId, result);
			return result;
		}
		protected internal virtual void InvalidateCommandCanExecuteChanged() {
			foreach (KeyValuePair<SchedulerCommandId, SchedulerUICommand> item in Commands) {
				SchedulerUICommand command = item.Value;
				command.RaiseCanExecuteChanged();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InvalidateCommandCanExecuteChanged();
		}
	}
}
