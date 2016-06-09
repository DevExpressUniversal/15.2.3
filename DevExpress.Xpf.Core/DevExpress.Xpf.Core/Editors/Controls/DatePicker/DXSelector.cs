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

using DevExpress.Data.Access;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors {
	public class DXSelector : DXItemsControl {
		public static readonly DependencyProperty SelectedIndexProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedValueProperty;
		public static readonly DependencyProperty SelectedValuePathProperty;
		static DXSelector() {
			Type ownerType = typeof(DXSelector);
			SelectedIndexProperty = DependencyPropertyManager.Register("SelectedIndex", typeof(int), ownerType,
				new FrameworkPropertyMetadata(InvalidHandle,
					(d, e) => ((DXSelector)d).SelectedIndexChanged((int)e.NewValue),
					(d, value) => ((DXSelector)d).CoerceSelectedIndex((int)value)));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null,
					(d, e) => ((DXSelector)d).SelectedItemChanged(e.NewValue),
					(d, value) => ((DXSelector)d).CoerceSelectedItem(value)));
			SelectedValueProperty = DependencyPropertyManager.Register("SelectedValue", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null,
					(d, e) => ((DXSelector)d).SelectedValueChanged(e.NewValue),
					(d, value) => ((DXSelector)d).CoerceSelectedValue(value)));
			SelectedValuePathProperty = DependencyPropertyManager.Register("SelectedValuePath", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty,
					(d, e) => ((DXSelector)d).SelectedValuePathChanged((string)e.NewValue),
					(d, value) => ((DXSelector)d).CoerceSelectedValuePath((string)value)));
		}
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public object SelectedValue {
			get { return GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}
		public string SelectedValuePath {
			get { return (string)GetValue(SelectedValuePathProperty); }
			set { SetValue(SelectedValuePathProperty, value); }
		}
		public event EventHandler SelectionChanged;
		readonly Locker coercionLocker = new Locker();
		PropertyDescriptor DataAccessDescriptor { get; set; }
		public DXSelector() {
			DataAccessDescriptor = new SelectorPropertyDescriptor(string.Empty);
		}
		protected virtual int CoerceSelectedIndex(int value) {
			if (!IsInitialized || coercionLocker.IsLocked)
				return value;
			int index = CanGenerateItem(value) ? value : InvalidHandle;
			coercionLocker.DoLockedActionIfNotLocked(() => {
				object item = GetItem(index);
				SelectedItem = item;
				SelectedValue = DataAccessDescriptor.GetValue(item);
			});
			return index;
		}
		protected virtual void SelectedIndexChanged(int newValue) {
		}
		protected override void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			base.OnScrollChanged(sender, e);
		}
		protected override void OnViewChanged(object sender, ViewChangedEventArgs e) {
			base.OnViewChanged(sender, e);
			if (!IsLoaded)
				return;
			if (!e.IsIntermediate) {
				double offset = Panel.Offset;
				SelectedIndex = Panel.IndexCalculator.LogicalOffsetToIndex(offset, GetItemsCount(), Panel.IsLooped);
			}
		}
		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
			this.CoerceValue(SelectedIndexProperty);
		}
		void RaiseSelectionChanged() {
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}
		protected virtual void SelectedItemChanged(object newValue) {
		}
		protected virtual object CoerceSelectedItem(object value) {
			if (coercionLocker.IsLocked)
				return value;
			int index = GetIndex(value);
			object item = GetItem(index);
			coercionLocker.DoLockedActionIfNotLocked(() => {
				SelectedIndex = index;
				SelectedValue = DataAccessDescriptor.GetValue(item);
			});
			return item;
		}
		protected virtual void SelectedValueChanged(object value) {
			RaiseSelectionChanged();
		}
		protected virtual object CoerceSelectedValue(object value) {
			if (coercionLocker.IsLocked)
				return value;
			int index = GetIndex(value, x => object.Equals(DataAccessDescriptor.GetValue(x), value));
			coercionLocker.DoLockedActionIfNotLocked(() => {
				SelectedIndex = index;
				SelectedItem = GetItem(index);
			});
			return index >= 0 ? value : null;
		}
		protected virtual string CoerceSelectedValuePath(string path) {
			DataAccessDescriptor = new SelectorPropertyDescriptor(path);
			if (!IsInitialized)
				return path;
			coercionLocker.DoLockedActionIfNotLocked(() => {
				int index = SelectedIndex;
				object item = GetItem(index);
				SelectedItem = item;
				SelectedValue = DataAccessDescriptor.GetValue(item);
			});
			return path;
		}
		protected virtual void SelectedValuePathChanged(string path) {
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			BringToView();
		}
		protected virtual void BringToView() {
		}
	}
	public class SelectorPropertyDescriptor : PropertyDescriptor {
		readonly Dictionary<Type, PropertyDescriptor> descriptors = new Dictionary<Type, PropertyDescriptor>();
		readonly bool returnComponent;
		string Path { get; set; }
		public SelectorPropertyDescriptor(string path)
			: base(string.IsNullOrEmpty(path) ? "empty" : path, null) {
			returnComponent = string.IsNullOrEmpty(path);
			Path = path;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(object); }
		}
		public override object GetValue(object component) {
			if (component == null || returnComponent)
				return component;
			PropertyDescriptor descr;
			Type type = component.GetType();
			if (!descriptors.TryGetValue(type, out descr)) {
				descr = new ComplexPropertyDescriptorReflection(component, Path);
				descriptors.Add(type, descr);
			}
			return descr.GetValue(component);
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object obj) {
			return false;
		}
	}
}
