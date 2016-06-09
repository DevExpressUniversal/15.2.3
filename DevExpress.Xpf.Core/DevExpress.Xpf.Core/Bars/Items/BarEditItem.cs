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
using DevExpress.Utils.Serializing;
using System.Windows;
using DevExpress.Xpf.Editors.Settings;
using System.ComponentModel;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class BarEditItem : BarItem {
		#region static
		public static readonly RoutedEvent EditValueChangedEvent;
		public static readonly DependencyProperty EditSettingsProperty;
		public static readonly DependencyProperty EditWidthProperty;
		public static readonly DependencyProperty EditHeightProperty;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty ClosePopupOnChangingEditValueProperty;
		public static readonly DependencyProperty BarEditItemProperty;
		public static readonly DependencyProperty Content2Property;
		public static readonly DependencyProperty Content2TemplateProperty;
		public static readonly DependencyProperty EditStyleProperty;
		public static readonly DependencyProperty EditTemplateProperty;
		public static readonly DependencyProperty EditHorizontalAlignmentProperty;								
		public static readonly DependencyProperty ShowInVerticalBarProperty;										
		static BarEditItem() {
			EditHorizontalAlignmentProperty = DependencyPropertyManager.Register("EditHorizontalAlignment", typeof(HorizontalAlignment?), typeof(BarEditItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnEditHorizontalAlignmentPropertyChanged)));
			EditSettingsProperty = DependencyPropertyManager.Register("EditSettings", typeof(BaseEditSettings), typeof(BarEditItem), new FrameworkPropertyMetadata(null, OnEditSettingsChanged));
			EditWidthProperty = DependencyPropertyManager.Register("EditWidth", typeof(double?), typeof(BarEditItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnWidthPropertyChanged)));
			EditHeightProperty = DependencyPropertyManager.Register("EditHeight", typeof(double?), typeof(BarEditItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnHeightPropertyChanged)));
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool), typeof(BarEditItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsReadOnlyPropertyChanged)));
			ClosePopupOnChangingEditValueProperty = DependencyPropertyManager.Register("ClosePopupOnChangingEditValue", typeof(bool), typeof(BarEditItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(CloseOwnerMenuOnChangeEditValuePropertyChanged)));
			EditValueChangedEvent = EventManager.RegisterRoutedEvent("EditValueChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(BarEditItem));
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), typeof(BarEditItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValuePropertyChanged), null, false));
			BarEditItemProperty = DependencyPropertyManager.RegisterAttached("BarEditItem", typeof(BarEditItem), typeof(BarEditItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			EditStyleProperty = DependencyPropertyManager.Register("EditStyle", typeof(Style), typeof(BarEditItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditStylePropertyChanged)));
			Content2Property = DependencyPropertyManager.Register("Content2", typeof(object), typeof(BarEditItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarEditItem)d).OnContent2Changed()));
			Content2TemplateProperty = DependencyPropertyManager.Register("Content2Template", typeof(DataTemplate), typeof(BarEditItem), new FrameworkPropertyMetadata(null, OnContent2TemplatePropertyChanged));
			EditTemplateProperty = DependencyPropertyManager.Register("EditTemplate", typeof(DataTemplate), typeof(BarEditItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnEditTemplatePropertyChanged)));
			ShowInVerticalBarProperty = DependencyPropertyManager.Register("ShowInVerticalBar", typeof(DevExpress.Utils.DefaultBoolean), typeof(BarEditItem), new FrameworkPropertyMetadata(DevExpress.Utils.DefaultBoolean.Default, (d, e) => ((BarEditItem)d).OnShowInVerticalBarChanged((DevExpress.Utils.DefaultBoolean)e.OldValue)));
		}
		public static void SetBarEditItem(DependencyObject d, BarEditItem item) {
			d.SetValue(BarEditItemProperty, item);
		}
		public static BarEditItem GetBarEditItem(DependencyObject d) { return (BarEditItem)d.GetValue(BarEditItemProperty); }
		protected static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)d).OnIsReadOnlyChanged(e);
		}
		protected static void CloseOwnerMenuOnChangeEditValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnCloseOwnerMenuOnChangeEditValueChanged(e);
		}
		protected static void OnContent2TemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)d).OnContent2TemplateChanged(e);
		}
		protected static void OnEditValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnEditValueChanged();
		}
		protected static void OnWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnWidthChanged(e);
		}
		protected static void OnHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnHeightChanged(e);
		}
		protected static void OnEditSettingsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnEditSettingsChanged(e);
		}
		protected static void OnEditStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)obj).OnEditStyleChanged(e);
		}
		protected static void OnEditTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)d).OnEditTemplateChanged((DataTemplate)e.OldValue);
		}
		protected static void OnEditHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItem)d).OnEditHorizontalAlignmentChanged((HorizontalAlignment?)e.OldValue);
		}
		#endregion
		#region dep props
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarEditItemContent2"),
#endif
	  TypeConverter(typeof(ObjectConverter))]
		public object Content2 {
			get { return GetValue(Content2Property); }
			set { SetValue(Content2Property, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemContent2Template")]
#endif
		public DataTemplate Content2Template {
			get { return (DataTemplate)GetValue(Content2TemplateProperty); }
			set { SetValue(Content2TemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemClosePopupOnChangingEditValue")]
#endif
		public bool ClosePopupOnChangingEditValue {
			get { return (bool)GetValue(ClosePopupOnChangingEditValueProperty); }
			set { SetValue(ClosePopupOnChangingEditValueProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemEditSettings")]
#endif
		public BaseEditSettings EditSettings {
			get { return (BaseEditSettings)GetValue(EditSettingsProperty); }
			set { SetValue(EditSettingsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemEditHorizontalAlignment")]
#endif
		public HorizontalAlignment? EditHorizontalAlignment {
			get { return (HorizontalAlignment?)GetValue(EditHorizontalAlignmentProperty); }
			set { SetValue(EditHorizontalAlignmentProperty, value); }
		}
		public DevExpress.Utils.DefaultBoolean ShowInVerticalBar {
			get { return (DevExpress.Utils.DefaultBoolean)GetValue(ShowInVerticalBarProperty); }
			set { SetValue(ShowInVerticalBarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemEditWidth")]
#endif
		public double? EditWidth {
			get { return (double?)GetValue(EditWidthProperty); }
			set { SetValue(EditWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemEditHeight")]
#endif
		public double? EditHeight {
			get { return (double?)GetValue(EditHeightProperty); }
			set { SetValue(EditHeightProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemEditStyle")]
#endif
		public Style EditStyle {
			get { return (Style)GetValue(EditStyleProperty); }
			set { SetValue(EditStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemIsReadOnly")]
#endif
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public DataTemplate EditTemplate {
			get { return (DataTemplate)GetValue(EditTemplateProperty); }
			set { SetValue(EditTemplateProperty, value); }
		}
		#endregion
		readonly PostponedAction addEditSettingsAction;
		public BarEditItem() {
			addEditSettingsAction = new PostponedAction(() => !IsInitialized);
		}
		protected virtual void OnCloseOwnerMenuOnChangeEditValueChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.UpdateEditIsReadOnlyState());
		}
		protected virtual void OnShowInVerticalBarChanged(DevExpress.Utils.DefaultBoolean oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.UpdateShowInVerticalBar());
		}
		protected virtual void OnEditSettingsChanged(DependencyPropertyChangedEventArgs e) {
			BaseEditSettings oldSettings = e.OldValue as BaseEditSettings;
			BaseEditSettings newSettings = e.NewValue as BaseEditSettings;
			if(oldSettings != null) {
				ClearDataContext(oldSettings);
				oldSettings.DataContext = null;
				BarEditItem.SetBarEditItem(oldSettings, null);				
				RemoveEditSettingsFromLogicalTree(oldSettings);
			}
			if(newSettings != null) {
				SetDataContext(newSettings);
				BarEditItem.SetBarEditItem(newSettings, this);				
				AddEditSettingsToLogicalTree(newSettings);
			}
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.RecreateEdit());
		}		
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			addEditSettingsAction.Perform();
		}
		protected virtual void RemoveEditSettingsFromLogicalTree(BaseEditSettings settings) {			
			addEditSettingsAction.PerformPostpone(() => {
				if (settings != null && LogicalTreeHelper.GetParent(settings) == this)
					RemoveLogicalChild(settings);
			});
		}		
		protected virtual void AddEditSettingsToLogicalTree(BaseEditSettings settings) {
			addEditSettingsAction.PerformPostpone(() => {
				if (settings != null && LogicalTreeHelper.GetParent(settings) == null)
					AddLogicalChild(settings);
			});
		}
		protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnDataContextChanged(sender, e);
			SetDataContext(EditSettings);
		}
		private void OnWidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks<BarEditItemLink>(l => l.UpdateActualEditSize());
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditWidthChanged());
		}
		private void OnHeightChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks<BarEditItemLink>(l => l.UpdateActualEditSize());
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditHeightChanged());
		}
		private void OnEditStyleChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditStyleChanged());
		}
		protected virtual void OnEditTemplateChanged(DataTemplate oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.UpdateActualEditTemplate());
		}
		protected virtual void OnEditHorizontalAlignmentChanged(HorizontalAlignment? oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.UpdateActualHorizontalEditAlignment());
		}
		private void OnContent2Changed() {
			ExecuteActionOnLinks<BarEditItemLink>(l => l.UpdateActualContent2());
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceContent2Changed());
		}
		protected virtual void OnContent2TemplateChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceContent2TemplateChanged());
		}
		public event RoutedEventHandler EditValueChanged {
			add { this.AddHandler(EditValueChangedEvent, value); }
			remove { this.RemoveHandler(EditValueChangedEvent, value); }
		}
		protected internal virtual void SyncEditValue(BarEditItemLinkControl linkControl) {
			EditValue = linkControl.GetActualEditValue();
		}
		protected internal virtual void OnEditValueChanged() {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.SyncEditValue());
			RaiseEditValueChanged();
		}
		protected virtual void RaiseEditValueChanged() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = BarEditItem.EditValueChangedEvent });
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarEditItemEditValue"),
#endif
 TypeConverter(typeof(ObjectConverter))]
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(EditSettings.If(x => x.Parent == this)));				
			}
		}
	}
}
