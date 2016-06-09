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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
namespace DevExpress.Xpf.Editors.Helpers {
	public class CheckedListEditValueConverter : IValueConverter {
		bool isCheckedEditor = true;
		public bool IsCheckedEditor { get { return isCheckedEditor; } set { isCheckedEditor = value; } }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!IsCheckedEditor) {
				if(value is IEnumerable)
					return ((IEnumerable)value).Cast<object>().FirstOrDefault();
				return value;
			}
			if(value is IEnumerable)
				return ((IEnumerable)value).Cast<object>().ToList();
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			IEnumerable<Type> interfaces = targetType.GetInterfaces();
			if(targetType.IsInterface)
				interfaces = interfaces.Union(targetType.Yield());
			if(interfaces == null)
				return value;
			var itemType = interfaces.FirstOrDefault(x => x.Name.Contains("IEnumerable") && x.IsGenericType)
				.Return(x => x.GetGenericArguments().FirstOrDefault(), () => null);
			if(itemType == null)
				return value;
			return Converter.ConvertToIEnumerable(value, itemType);
		}
		static class Converter {
			static MethodInfo castMethod;
			static MethodInfo convertToIEnumerableMethod;
			static Converter() {
				castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
				convertToIEnumerableMethod = typeof(Converter).GetMethod("ConvertToIEnumerable", new Type[] { typeof(object) });
			}
			static T Convert<T>(object value) {
				if(object.ReferenceEquals(null, value))
					return default(T);
				if(typeof(T).IsAssignableFrom(value.GetType()))
					return (T)value;
				return (T)System.Convert.ChangeType(value, typeof(T));
			}
			public static IEnumerable ConvertToIEnumerable(object value, Type t) {
				return (IEnumerable)convertToIEnumerableMethod.MakeGenericMethod(t).Invoke(null, new object[] { value });
			}
			public static IEnumerable<T> ConvertToIEnumerable<T>(object value) {
				IEnumerable<T> genericEnumerable = value as IEnumerable<T>;
				if(genericEnumerable != null)
					return genericEnumerable;
				IEnumerable enumerable = value as IEnumerable;
				if(enumerable != null)
					return Cast<T>(enumerable);
				return ConvertToArray<T>(value);
			}
			static T[] ConvertToArray<T>(object value) {
				T[] arr = value as T[];
				if(arr != null)
					return arr;
				object[] objArr = value as object[];
				if(objArr != null)
					return Array.ConvertAll(objArr, (e) => Convert<T>(e));
				IEnumerable<T> genericEnumerable = value as IEnumerable<T>;
				if(genericEnumerable != null)
					return genericEnumerable.ToArray();
				IEnumerable enumerable = value as IEnumerable;
				if(enumerable != null)
					return Cast<T>(enumerable).ToArray();
				if(value is T)
					return new T[] { (T)value };
				return new T[] { };
			}
			static IEnumerable<T> Cast<T>(IEnumerable value) {
				return (IEnumerable<T>)castMethod.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { value });
			}
		}
	}
	public class CheckedListEditValueConverterExtension : MarkupExtension {
		bool isCheckedEditor = true;
		public bool IsCheckedEditor { get { return isCheckedEditor; } set { isCheckedEditor = value; } }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new CheckedListEditValueConverter() { IsCheckedEditor = IsCheckedEditor };
		}
	}
	public class BooleanChoiceEditorBehavior : Behavior<BaseEdit> {
		public static readonly DependencyProperty DefaultNameProperty = DependencyProperty.Register("DefaultName", typeof(string), typeof(BooleanChoiceEditorBehavior), new PropertyMetadata(null, (d, e) => ((BooleanChoiceEditorBehavior)d).UpdateItemsSource()));
		public static readonly DependencyProperty TrueNameProperty = DependencyProperty.Register("TrueName", typeof(string), typeof(BooleanChoiceEditorBehavior), new PropertyMetadata(null, (d, e) => ((BooleanChoiceEditorBehavior)d).UpdateItemsSource()));
		public static readonly DependencyProperty FalseNameProperty = DependencyProperty.Register("FalseName", typeof(string), typeof(BooleanChoiceEditorBehavior), new PropertyMetadata(null, (d, e) => ((BooleanChoiceEditorBehavior)d).UpdateItemsSource()));
		public string DefaultName { get { return (string)GetValue(DefaultNameProperty); } set { SetValue(DefaultNameProperty, value); } }
		public string TrueName { get { return (string)GetValue(TrueNameProperty); } set { SetValue(TrueNameProperty, value); } }
		public string FalseName { get { return (string)GetValue(FalseNameProperty); } set { SetValue(FalseNameProperty, value); } }
		protected override void OnAttached() {
			base.OnAttached();
			TypeDescriptor.GetProperties(AssociatedObject)["ValueMember"].SetValue(AssociatedObject, "Value");
			TypeDescriptor.GetProperties(AssociatedObject)["DisplayMember"].SetValue(AssociatedObject, "Name");
			UpdateItemsSource();
		}
		void UpdateItemsSource() {
			if(AssociatedObject == null) return;
			List<BooleanInfo> source = new List<BooleanInfo>();
			source.Add(new BooleanInfo() { Value = null, Name = DefaultName });
			source.Add(new BooleanInfo() { Value = true, Name = TrueName });
			source.Add(new BooleanInfo() { Value = false, Name = FalseName });
			TypeDescriptor.GetProperties(AssociatedObject)["ItemsSource"].SetValue(AssociatedObject, source);
		}
		class BooleanInfo {
			public string Name { get; set; }
			public bool? Value { get; set; }
		}
	}
	public class FilterListEditorBehavior : Behavior<BaseEdit> {
		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(null, (d, e) => ((FilterListEditorBehavior)d).UpdateEditorItemsSource()));
		public static readonly DependencyProperty ValueMemberProperty = DependencyProperty.Register("ValueMember", typeof(string), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(null, (d, e) => ((FilterListEditorBehavior)d).UpdateEditorItemsSource()));
		public static readonly DependencyProperty DisplayMemberProperty = DependencyProperty.Register("DisplayMember", typeof(string), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(null, (d, e) => ((FilterListEditorBehavior)d).UpdateEditorItemsSource()));
		public static readonly DependencyProperty UseFlagsProperty = DependencyProperty.Register("UseFlags", typeof(bool), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(false, (d,e) => ((FilterListEditorBehavior)d).UpdateEditorStyleSettings()));
		public static readonly DependencyProperty UseSelectAllProperty = DependencyProperty.Register("UseSelectAll", typeof(bool), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(false, (d, e) => ((FilterListEditorBehavior)d).UpdateEditorUseSelectAll()));
		public static readonly DependencyProperty SelectAllNameProperty = DependencyProperty.Register("SelectAllName", typeof(string), typeof(FilterListEditorBehavior), 
			new PropertyMetadata("All", (d, e) => ((FilterListEditorBehavior)d).UpdateEditorStyleSettings()));
		public static readonly DependencyProperty UseTokenStyleProperty = DependencyProperty.Register("UseTokenStyle", typeof(bool), typeof(FilterListEditorBehavior), 
			new PropertyMetadata(false, (d, e) => ((FilterListEditorBehavior)d).UpdateEditorStyleSettings()));
		public object ItemsSource { get { return (object)GetValue(ItemsSourceProperty); } set { SetValue(ItemsSourceProperty, value); } }
		public string ValueMember { get { return (string)GetValue(ValueMemberProperty); } set { SetValue(ValueMemberProperty, value); } }
		public string DisplayMember { get { return (string)GetValue(DisplayMemberProperty); } set { SetValue(DisplayMemberProperty, value); } }
		public bool UseFlags { get { return (bool)GetValue(UseFlagsProperty); } set { SetValue(UseFlagsProperty, value); } }
		public bool UseSelectAll { get { return (bool)GetValue(UseSelectAllProperty); } set { SetValue(UseSelectAllProperty, value); } }
		public string SelectAllName { get { return (string)GetValue(SelectAllNameProperty); } set { SetValue(SelectAllNameProperty, value); } }
		public bool UseTokenStyle { get { return (bool)GetValue(UseTokenStyleProperty); } set { SetValue(UseTokenStyleProperty, value); } }
		public BindingBase RadioEditValueBinding {
			get { return radioEditValueBinding; }
			set { radioEditValueBinding = value; UpdateEditorEditValue(); }
		}
		public BindingBase CheckedEditValueBinding {
			get { return checkedEditValueBinding; }
			set { checkedEditValueBinding = value; UpdateEditorEditValue(); }
		}
		bool shouldSetItemsSource = false;
		BindingBase radioEditValueBinding;
		BindingBase checkedEditValueBinding;
		protected override void OnAttached() {
			base.OnAttached();
			if(shouldSetItemsSource) UpdateEditorItemsSource();
			UpdateEditorStyleSettings();
			UpdateEditorUseSelectAll();
			UpdateEditorEditValue();
		}
		void UpdateEditorEditValue() {
			if(AssociatedObject == null || RadioEditValueBinding == null || CheckedEditValueBinding == null) return;
			if(UseFlags)
				AssociatedObject.SetBinding(BaseEdit.EditValueProperty, CheckedEditValueBinding);
			else AssociatedObject.SetBinding(BaseEdit.EditValueProperty, RadioEditValueBinding);
		}
		void UpdateEditorItemsSource() {
			shouldSetItemsSource = true;
			if(AssociatedObject == null) return;
			TypeDescriptor.GetProperties(AssociatedObject)["ItemsSource"].Do(x => x.SetValue(AssociatedObject, ItemsSource));
			TypeDescriptor.GetProperties(AssociatedObject)["ValueMember"].Do(x => x.SetValue(AssociatedObject, ValueMember));
			TypeDescriptor.GetProperties(AssociatedObject)["DisplayMember"].Do(x => x.SetValue(AssociatedObject, DisplayMember));
		}
		void UpdateEditorStyleSettings() {
			(AssociatedObject as ListBoxEdit).Do(x => x.StyleSettings = GetListBoxStyleSettings());
			(AssociatedObject as ComboBoxEdit).Do(x => x.StyleSettings = GetComboBoxStyleSettings());
			UpdateEditorEditValue();
		}
		void UpdateEditorUseSelectAll() {
			(AssociatedObject as ListBoxEdit).Do(x => x.ShowCustomItems = UseSelectAll);
			(AssociatedObject as ComboBoxEdit).Do(x => x.ShowCustomItems = UseSelectAll);
		}
		BaseEditStyleSettings GetListBoxStyleSettings() {
			if(UseFlags)
				return new SelectAllCheckedListBoxEditStyleSettings() { SelectAllName = SelectAllName };
			else return new SelectAllRadioListBoxEditStyleSettings() { SelectAllName = SelectAllName };
		}
		BaseEditStyleSettings GetComboBoxStyleSettings() {
			if(UseTokenStyle)
				return new TokenComboBoxStyleSettings() { NewTokenPosition = NewTokenPosition.Far };
			if(UseFlags)
				return new SelectAllCheckedComboBoxStyleSettings() { SelectAllName = SelectAllName };
			else return new ComboBoxStyleSettings();
		}
		#region StyleSettings
		class SelectAllCheckedListBoxEditStyleSettings : CheckedListBoxEditStyleSettings {
			public static readonly DependencyProperty SelectAllNameProperty = DependencyProperty.Register("SelectAllName", typeof(string), typeof(SelectAllCheckedListBoxEditStyleSettings), new PropertyMetadata("All"));
			public string SelectAllName { get { return (string)GetValue(SelectAllNameProperty); } set { SetValue(SelectAllNameProperty, value); } }
			protected internal override IEnumerable<CustomItem> GetCustomItems(ListBoxEdit editor) {
				return new List<CustomItem>() { new SelectAllItem() { DisplayText = SelectAllName } };
			}
			protected override bool ShowCustomItemInternal(ListBoxEdit editor) {
				return true;
			}
		}
		class SelectAllRadioListBoxEditStyleSettings : RadioListBoxEditStyleSettings {
			public static readonly DependencyProperty SelectAllNameProperty = SelectAllCheckedListBoxEditStyleSettings.SelectAllNameProperty.AddOwner(typeof(RadioListBoxEditStyleSettings));
			public string SelectAllName { get { return (string)GetValue(SelectAllNameProperty); } set { SetValue(SelectAllNameProperty, value); } }
			protected internal override IEnumerable<CustomItem> GetCustomItems(ListBoxEdit editor) {
				return new List<CustomItem>() { new CustomItem() { DisplayText = SelectAllName, EditValue = null } };
			}
			protected override bool ShowCustomItemInternal(ListBoxEdit editor) {
				return true;
			}
		}
		class SelectAllCheckedComboBoxStyleSettings : CheckedComboBoxStyleSettings {
			public static readonly DependencyProperty SelectAllNameProperty = SelectAllCheckedListBoxEditStyleSettings.SelectAllNameProperty.AddOwner(typeof(SelectAllCheckedComboBoxStyleSettings));
			public string SelectAllName { get { return (string)GetValue(SelectAllNameProperty); } set { SetValue(SelectAllNameProperty, value); } }
			protected internal override IEnumerable<CustomItem> GetCustomItems(LookUpEditBase editor) {
				return new List<CustomItem>() { new SelectAllItem() { DisplayText = SelectAllName } };
			}
			protected override bool ShowCustomItemInternal(LookUpEditBase editor) {
				return true;
			}
		}
		#endregion StyleSettings
	}
	public class RightClickEditorBehavior : Behavior<BaseEdit> {
		public static readonly DependencyProperty IsRightClickEditorBehaviorEnabledProperty =
			DependencyProperty.RegisterAttached("IsRightClickEditorBehaviorEnabled", typeof(bool), typeof(RightClickEditorBehavior), new PropertyMetadata(true, OnIsRightClickEditorBehaviorEnabledChanged));
		public static bool GetIsRightClickEditorBehaviorEnabled(BaseEdit obj) { return (bool)obj.GetValue(IsRightClickEditorBehaviorEnabledProperty); }
		public static void SetIsRightClickEditorBehaviorEnabled(BaseEdit obj, bool value) { obj.SetValue(IsRightClickEditorBehaviorEnabledProperty, value); }
		static void OnIsRightClickEditorBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BaseEdit editor = (BaseEdit)d;
			if((bool)e.NewValue) {
				Interaction.GetBehaviors(editor).OfType<RightClickEditorBehavior>()
					.FirstOrDefault().Do(x => Interaction.GetBehaviors(editor).Remove(x));
			} else Interaction.GetBehaviors(editor).Add(new RightClickEditorBehavior() { IsRightClickEnabled = false });
		}
		public static readonly DependencyProperty IsRightClickEnabledProperty = DependencyProperty.Register("IsRightClickEnabled", typeof(bool), typeof(RightClickEditorBehavior), new PropertyMetadata(true));
		public bool IsRightClickEnabled { get { return (bool)GetValue(IsRightClickEnabledProperty); } set { SetValue(IsRightClickEnabledProperty, value); } }
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.PreviewMouseRightButtonDown += OnEditorPreviewMouseRightButtonDown;
		}
		protected override void OnDetaching() {
			AssociatedObject.PreviewMouseRightButtonDown -= OnEditorPreviewMouseRightButtonDown;
			base.OnDetaching();
		}
		void OnEditorPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			if(!IsRightClickEnabled)
				e.Handled = true;
		}
	}
}
