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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
#if !DXWINDOW
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core.Internal;
#endif
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Reflection.Emit;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
#if !DXWINDOW
	[Serializable]
	public class XbapNotSupportedException : ApplicationException {
		internal const string Text = @"Starting from v2011 vol 2, XBAP applications are not supported. Instead, we recommend that you use ClickOnce deployment (the most preferable way) or migrate your application to Silverlight. For more information, please refer to http://go.devexpress.com/SupportXBAP.aspx

If you still want to continue using DevExpress controls in XBAP applications, set the OptionsXBAP.SuppressNotSupportedException property to True.";
		public XbapNotSupportedException(string message)
			: base(message) {
		}
		[System.Security.SecuritySafeCritical]
		public XbapNotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) 
			: base(info, context) {
		}
	}
	public static class OptionsXBAP {
		public static bool SuppressNotSupportedException { get; set; }
	}
#endif
	[Flags]
	public enum Side {
		Left = 0x0, 
		Top = 0x1, 
		Right = 0x2, 
		Bottom = 0x4,
		LeftRight = Left | Right,
		TopBottom = Top | Bottom,
		All = LeftRight | TopBottom,
	}
	public static class SideExtentions {
		public static HorizontalAlignment GetHorizontalAlignment(this Side side) {
			if (side.GetOrientation() == Orientation.Vertical)
				return side.IsStart() ? HorizontalAlignment.Left : HorizontalAlignment.Right;
			else
				return HorizontalAlignment.Stretch;
		}
		public static VerticalAlignment GetVerticalAlignment(this Side side) {
			if (side.GetOrientation() == Orientation.Horizontal)
				return side.IsStart() ? VerticalAlignment.Top : VerticalAlignment.Bottom;
			else
				return VerticalAlignment.Stretch;
		}
		public static Side GetOppositeSide(this Side side) {
			switch (side) {
				case Side.Left:
					return Side.Right;
				case Side.Top:
					return Side.Bottom;
				case Side.Right:
					return Side.Left;
				case Side.Bottom:
					return Side.Top;
				default:
					throw new Exception();
			}
		}
		public static Orientation GetOrientation(this Side side) {
			return side == Side.Left || side == Side.Right ? Orientation.Vertical : Orientation.Horizontal;
		}
		public static bool IsStart(this Side side) {
			return side == Side.Left || side == Side.Top;
		}
		public static bool IsEnd(this Side side) {
			return side == Side.Right || side == Side.Bottom;
		}
	}
#if !DXWINDOW
	public class LockableCollection<T> : ObservableCollection<T> {
		private bool _IsChanged;
		private int _UpdateLockCount;
		public void Assign(IList<T> source) {
			if (Equals(source))
				return;
			BeginUpdate();
			try {
				Clear();
				foreach (T item in source)
					Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public void BeginUpdate() {
			if (!IsUpdateLocked)
				_IsChanged = false;
			_UpdateLockCount++;
		}
		public void EndUpdate() {
			if (!IsUpdateLocked)
				return;
			_UpdateLockCount--;
			if (!IsUpdateLocked && _IsChanged)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public override bool Equals(object obj) {
			if (obj is IList<T>) {
				var list = (IList<T>)obj;
				if (Count != list.Count)
					return false;
				else {
					for (int i = 0; i < Count; i++)
						if (!this[i].Equals(list[i]))
							return false;
					return true;
				}
			}
			else
				return base.Equals(obj);
		}
		public void ForEach(Action<T> action) {
			for (int i = 0; i < Count; i++)
				action(this[i]);
		}
		public T Find(Predicate<T> match) {
			if (match == null)
				return default(T);
			int count = Count;
			for (int i = 0; i < count; i++) {
				T item = this[i];
				if (match(item))
					return item;
			}
			return default(T);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public bool IsUpdateLocked { get { return _UpdateLockCount > 0; } }
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (IsUpdateLocked)
				_IsChanged = true;
			else
				base.OnCollectionChanged(e);
		}
		protected override void SetItem(int index, T item) {
			if (!this[index].Equals(item))
				base.SetItem(index, item);
		}
	}
	public class FrameworkElements : LockableCollection<FrameworkElement> { }
	public class ElementPool<T> : IEnumerable<T> where T : FrameworkElement, new() {
		private Style _ItemStyle;
		private List<T> UsedItems = new List<T>();
		public ElementPool(Panel container) {
			Container = container;
			Items = new List<T>();
		}
		public T Add() {
			var result = GetFirstUnusedItem();
			if (result == null) {
				result = CreateItem();
				result.Tag = this;
				Items.Add(result);
				Container.Children.Add(result);
			}
			MarkItemAsUsed(result);
			return result;
		}
		public void DeleteUnusedItems() {
			for (int i = Items.Count - 1; i >= 0; i--) {
				T item = Items[i];
				if (!IsItemUsed(item)) {
					DeleteItem(item);
					item.Tag = null;
					Container.Children.Remove(item);
					Items.RemoveAt(i);
				}
			}
		}
		public int IndexOf(T item) {
			return Items.IndexOf(item);
		}
		public bool IsItem(UIElement element) {
			return element is T && ((T)element).Tag == this;
		}
		public void MarkItemsAsUnused() {
			foreach (var item in Items)
				MarkItemAsUnused(item);
		}
		public T this[int index] { get { return Items[index]; } }
		public Panel Container { get; private set; }
		public int Count { get { return Items.Count; } }
		public Style ItemStyle {
			get { return _ItemStyle; }
			set {
				if (ItemStyle == value)
					return;
				_ItemStyle = value;
				OnItemStyleChanged();
			}
		}
		protected virtual T CreateItem() {
			var result = new T();
			if (ItemStyle != null)
				result.SetValueIfNotDefault(FrameworkElement.StyleProperty, ItemStyle);
			return result;
		}
		protected virtual void DeleteItem(T item) {
		}
		protected virtual bool IsItemInTree(T item) {
			return item.Parent != null;
		}
		protected T GetFirstUnusedItem() {
			foreach(var item in Items)
				if(!IsItemUsed(item)) {
					if(!IsItemInTree(item))
						Container.Children.Add(item);
					return item;
				}
			return null;
		}
		protected bool IsItemUsed(T item) {
			return UsedItems.Contains(item);
		}
		protected void MarkItemAsUnused(T item) {
			UsedItems.Remove(item);
		}
		protected void MarkItemAsUsed(T item) {
			UsedItems.Add(item);
		}
		protected virtual void OnItemStyleChanged() {
			foreach (T item in Items)
				item.SetValueIfNotDefault(FrameworkElement.StyleProperty, ItemStyle);
		}
		protected List<T> Items { get; private set; }
		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			return Items.GetEnumerator();
		}
		#endregion IEnumerable
		#region IEnumerable<T>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return Items.GetEnumerator();
		}
		#endregion IEnumerable<T>
	}
	public class BindingContainer {
		public BindingBase Binding { get; private set; }
		public BindingContainer(BindingBase binding) {
			this.Binding = binding;
		}
	}
	public interface INotifyVisibilityChanged {
		void OnVisibilityChanged();
	}
	public static class FrameworkContentElementHelper {
		static Action<FrameworkContentElement, DependencyPropertyChangedEventArgs> baseOnPropertyChanged;
		static Func<FrameworkElement, FrameworkTemplate> getTemplateInternal;
		static Func<DependencyPropertyChangedEventArgs, bool> getIsAValueChange;
		static Func<DependencyPropertyChangedEventArgs, bool> getIsASubPropertyChange;
		static Action<FrameworkContentElement, DependencyObject> setTemplatedParent;
		static Func<FrameworkContentElement, int> getTemplateChildIndex;
		static Action<FrameworkContentElement, int> setTemplateChildIndex;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static DependencyProperty dskProperty;		
		static FrameworkContentElementHelper() {
			baseOnPropertyChanged = ReflectionHelper.CreateInstanceMethodHandler<FrameworkContentElement, Action<FrameworkContentElement, DependencyPropertyChangedEventArgs>>(
				null,
				"OnPropertyChanged",
				BindingFlags.Instance | BindingFlags.NonPublic,
				callVirtIfNeeded: false);
			getIsAValueChange = ReflectionHelper.CreateInstanceMethodHandler<DependencyPropertyChangedEventArgs, Func<DependencyPropertyChangedEventArgs, bool>>(
				default(DependencyPropertyChangedEventArgs),
				"get_IsAValueChange",
				BindingFlags.Instance | BindingFlags.NonPublic);
			getIsASubPropertyChange = ReflectionHelper.CreateInstanceMethodHandler<DependencyPropertyChangedEventArgs, Func<DependencyPropertyChangedEventArgs, bool>>(
				default(DependencyPropertyChangedEventArgs),
				"get_IsASubPropertyChange",
				BindingFlags.Instance | BindingFlags.NonPublic);
			dskProperty = typeof(FrameworkContentElement).GetField("DefaultStyleKeyProperty", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as DependencyProperty;
			getTemplateInternal = ReflectionHelper.CreateInstanceMethodHandler<FrameworkElement, Func<FrameworkElement, FrameworkTemplate>>(
				null,
				"get_TemplateInternal",
				BindingFlags.Instance | BindingFlags.NonPublic);
			setTemplatedParent = ReflectionHelper.CreateFieldSetter<FrameworkContentElement, DependencyObject>(typeof(FrameworkContentElement), "_templatedParent", BindingFlags.Instance | BindingFlags.NonPublic);
			getTemplateChildIndex = ReflectionHelper.CreateInstanceMethodHandler<FrameworkContentElement, Func<FrameworkContentElement, int>>(
			   null,
			   "get_TemplateChildIndex",
			   BindingFlags.Instance | BindingFlags.NonPublic);
			setTemplateChildIndex = ReflectionHelper.CreateInstanceMethodHandler<FrameworkContentElement, Action<FrameworkContentElement, int>>(
			   null,
			   "set_TemplateChildIndex",
			   BindingFlags.Instance | BindingFlags.NonPublic);
		}
		public static void SafeOnPropertyChanged(this FrameworkContentElement element, DependencyPropertyChangedEventArgs e) {
			bool shouldResetTemplatedParent = false;
			int templateChildIndex = -1;
			FrameworkElement feTemplatedParent = null;
			if (getIsAValueChange(e) || getIsASubPropertyChange(e)) {
				DependencyProperty dp = e.Property;
				if (dp != FrameworkContentElement.StyleProperty && dp != dskProperty) {
					feTemplatedParent = element.TemplatedParent as FrameworkElement;
					if (feTemplatedParent != null && getTemplateInternal(feTemplatedParent) == null) {
						shouldResetTemplatedParent = true;
						setTemplatedParent(element, null);
						templateChildIndex = getTemplateChildIndex(element);
						setTemplateChildIndex(element, -1);
					}
				}
			}
			baseOnPropertyChanged(element, e);
			if (shouldResetTemplatedParent) {
				setTemplateChildIndex(element, templateChildIndex);
				setTemplatedParent(element, feTemplatedParent);
			}
		}
	}
	public static class FrameworkElementHelper {
		public static readonly DependencyProperty IsMouseOverOverrideProperty =
			DependencyProperty.RegisterAttached("IsMouseOverOverride", typeof(bool), typeof(FrameworkElementHelper),
				new PropertyMetadata((o, e) => OnIsMouseOverOverrideChanged((FrameworkElement)o)));
		public static readonly DependencyProperty EnableIsMouseOverOverrideProperty =
			DependencyProperty.RegisterAttached("EnableIsMouseOverOverride", typeof(bool), typeof(FrameworkElementHelper),
				new PropertyMetadata((o, e) => OnEnableIsMouseOverOverrideChanged((FrameworkElement)o, (bool)e.NewValue)));
		public static readonly DependencyProperty ClipCornerRadiusProperty =
			DependencyProperty.RegisterAttached("ClipCornerRadius", typeof(double), typeof(FrameworkElementHelper),
				new PropertyMetadata((o, e) => OnClipCornerRadiusChanged((FrameworkElement)o)));
		public static readonly DependencyProperty IsClippedProperty =
			DependencyProperty.RegisterAttached("IsClipped", typeof(bool), typeof(FrameworkElementHelper),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var element = o as FrameworkElement;
						if (element == null)
							return;
						if ((bool)e.NewValue)
							element.SizeChanged += OnElementSizeChanged;
						else
							element.SizeChanged -= OnElementSizeChanged;
					}));
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(FrameworkElementHelper),
				new PropertyMetadata(true, (o, e) => ((FrameworkElement)o).SetVisible((bool)e.NewValue)));
		public static object GetToolTip(FrameworkElement element) {
			return element.ToolTip;
		}
		public static void SetToolTip(FrameworkElement element, object value) {
			element.ToolTip = value;
		}
		public static void SetIsLoaded(FrameworkElement element, bool isLoaded) {
		}
		public static bool GetIsLoaded(FrameworkElement element) {
			return element.IsLoaded;
		}
		public static void SetIsLoaded(FrameworkContentElement element, bool isLoaded) {
		}
		public static bool GetIsLoaded(FrameworkContentElement element) {
			return element.IsLoaded;
		}
		public static void SetAllowDrop(FrameworkElement element, bool allowDrop) {
			element.AllowDrop = allowDrop;
		}
		public static bool GetAllowDrop(FrameworkElement element) {
			return element.AllowDrop;
		}
		public static double GetClipCornerRadius(FrameworkElement element) {
			return (double)element.GetValue(ClipCornerRadiusProperty);
		}
		public static void SetClipCornerRadius(FrameworkElement element, double value) {
			element.SetValue(ClipCornerRadiusProperty, value);
		}
		public static bool GetIsClipped(FrameworkElement element) {
			return (bool)element.GetValue(IsClippedProperty);
		}
		public static void SetIsClipped(FrameworkElement element, bool value) {
			element.SetValue(IsClippedProperty, value);
		}
		public static bool GetIsMouseOverOverride(FrameworkElement element) {
			return (bool)element.GetValue(IsMouseOverOverrideProperty);
		}
		public static void SetIsMouseOverOverride(FrameworkElement element, bool value) {
			element.SetValue(IsMouseOverOverrideProperty, value);
		}
		public static bool GetEnableIsMouseOverOverride(FrameworkElement element) {
			return (bool)element.GetValue(EnableIsMouseOverOverrideProperty);
		}
		public static void SetEnableIsMouseOverOverride(FrameworkElement element, bool value) {
			element.SetValue(EnableIsMouseOverOverrideProperty, value);
		}
		public static bool GetIsVisible(FrameworkElement element) {
			return element.GetVisible();
		}
		public static void SetIsVisible(FrameworkElement element, bool value) {
			element.SetValue(IsVisibleProperty, value);
		}
		private static void OnClipCornerRadiusChanged(FrameworkElement element) {
			element.SizeChanged -= ElementClipCornerRadiusSizeChanged;
			element.SizeChanged += ElementClipCornerRadiusSizeChanged;
			ElementClipCornerRadiusSizeChanged(element, null);
		}
		static void ElementClipCornerRadiusSizeChanged(object sender, SizeChangedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			double radius = GetClipCornerRadius(element);
			element.Clip = new RectangleGeometry { Rect = RectHelper.New(new Size(element.ActualWidth, element.ActualHeight)), RadiusX = radius, RadiusY = radius };
		}
		private static void OnElementSizeChanged(object sender, SizeChangedEventArgs e) {
			((FrameworkElement)sender).ClipToBounds();
		}
		private static void OnEnableIsMouseOverOverrideChanged(FrameworkElement frameworkElement, bool value) {
			MouseEventHandler enter = new MouseEventHandler((d, e) => { frameworkElement.SetValue(IsMouseOverOverrideProperty, true); });
			MouseEventHandler leave = new MouseEventHandler((d, e) => { frameworkElement.SetValue(IsMouseOverOverrideProperty, false); });
			if(value) {
				frameworkElement.MouseEnter +=enter;
				frameworkElement.MouseLeave += leave;
			}
			else {
				frameworkElement.MouseEnter -= enter;
				frameworkElement.MouseLeave -= leave;
			}
		}
		private static void OnIsMouseOverOverrideChanged(FrameworkElement frameworkElement) {
		}
#if !DXWINDOW
		static readonly Action<FrameworkElement, bool> SetBypassLayoutPoliciesHandler;
		static FrameworkElementHelper() {
			SetBypassLayoutPoliciesHandler = ReflectionHelper.CreateInstanceMethodHandler<FrameworkElement, Action<FrameworkElement, bool>>(
				null, "set_BypassLayoutPolicies", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		public static void SetBypassLayoutPolicies(this FrameworkElement element, bool value) {
			SetBypassLayoutPoliciesHandler(element, value);
		}
#endif
	}
	public static class GraphicsHelper {
		public static PathFigure CreateRectFigure(Rect rect) {
			var result = new PathFigure { IsClosed = true, StartPoint = rect.TopLeft() };
			var segment = new PolyLineSegment();
			segment.Points.Add(rect.TopRight());
			segment.Points.Add(rect.BottomRight());
			segment.Points.Add(rect.BottomLeft());
			result.Segments.Add(segment);
			return result;
		}
	}
	public class FontLayoutHelper {
		string[] FontProp = { "FontFamily", "FontSize", "FontStretch", "FontStyle", "FontWeight", "Foreground" };
		Dictionary<string, object> FontValues = new Dictionary<string, object>();
		public FontLayoutHelper() { }
		public FontLayoutHelper(object cc) {
			Assign(cc);
		}
		public void SetFont(object tb) {
			if (tb == null) return;
			foreach (PropertyInfo pi in tb.GetType().GetProperties()) {
				if (FontValues.ContainsKey(pi.Name)) tb.GetType().GetProperty(pi.Name).SetValue(tb, FontValues[pi.Name], null);
			}
		}
		void Assign(object cc) {
			if (cc == null) return;
			foreach (PropertyInfo pi in cc.GetType().GetProperties()) {
				if (FontProp.Contains(pi.Name)) FontValues.Add(pi.Name, pi.GetValue(cc, null));
			}
		}
	}
	public delegate void ValueChangedEventHandler<T>(object sender, ValueChangedEventArgs<T> e);
	public delegate void RoutedValueChangedEventHandler<T>(object sender, RoutedValueChangedEventArgs<T> args);
	public delegate void RoutedValueChangingEventHandler<T>(object sender, RoutedValueChangingEventArgs<T> args);
	public class RoutedValueChangedEventArgs<T> : RoutedEventArgs {
		public RoutedValueChangedEventArgs(RoutedEvent routedEvent, T oldValue, T newValue) : base(routedEvent) {
			NewValue = newValue;
			OldValue = oldValue;
		}
		public RoutedValueChangedEventArgs(T oldValue, T newValue) : this(null, oldValue, newValue) { }
		public T OldValue { get; private set; }
		public T NewValue { get; private set; }
	}
	public class RoutedValueChangingEventArgs<T> : CancelRoutedEventArgs {
		public RoutedValueChangingEventArgs(RoutedEvent routedEvent, T oldValue, T newValue) : base(routedEvent) {
			NewValue = newValue;
			OldValue = oldValue;
		}
		public RoutedValueChangingEventArgs(T oldValue, T newValue) : this(null, oldValue, newValue) { }
		public T OldValue { get; private set; }
		public T NewValue { get; private set; }
	}
	public class ValueChangedEventArgs<T> : EventArgs {
		private T _NewValue;
		private T _OldValue;
		public ValueChangedEventArgs(T oldValue, T newValue) {
			_OldValue = oldValue;
			_NewValue = newValue;
		}
		public T NewValue { get { return _NewValue; } }
		public T OldValue { get { return _OldValue; } }
	}
	public static class UriHelper {
		public static Uri GetUri(string dllName, string relativeFilePath) {
			return new Uri(string.Format("/{0}{1};component/{2}", dllName, AssemblyInfo.VSuffix, relativeFilePath), UriKind.RelativeOrAbsolute);
		}
	}
	public static class Keyboard2 {
		public static bool IsAltPressed {
			get { return (Keyboard.Modifiers & ModifierKeys.Alt) != 0; }
		}
		public static bool IsControlPressed {
			get {
				return (Keyboard.Modifiers & ModifierKeys.Control) != 0;
			}
		}
		public static bool IsShiftPressed {
			get {
				return (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
			}
		}
	}
	public enum CursorType { 
		AppStarting,
		Arrow,
		ArrowCD,
		Cross,
		Hand,
		Help,
		IBeam,
		No,
		None,
		Pen,
		ScrollAll,
		ScrollE,
		ScrollN,
		ScrollNE,
		ScrollNS,
		ScrollNW,
		ScrollS,
		ScrollSE,
		ScrollSW,
		ScrollW,
		ScrollWE,
		SizeAll,
		SizeNESW,
		SizeNS,
		SizeNWSE,
		SizeWE,
		UpArrow,
		Wait
	}
	public class CursorHelper : DependencyObject {
		public static readonly DependencyProperty CursorProperty;
		static CursorHelper() { 
			CursorProperty = DependencyProperty.RegisterAttached("Cursor", typeof(CursorType), typeof(CursorHelper), new PropertyMetadata(CursorType.Arrow, new PropertyChangedCallback(OnCursorPropertyChanged)));
		}
		public static void SetCursor(DependencyObject d, CursorType value) { d.SetValue(CursorProperty, value); }
		public static CursorType GetCursor(DependencyObject d) { return (CursorType)d.GetValue(CursorProperty); }
		protected static void OnCursorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CursorType tp = (CursorType)e.NewValue;
			FrameworkElement elem = d as FrameworkElement;
			if(elem == null)
				return;
			switch(tp) { 
				case CursorType.Arrow: elem.Cursor = Cursors.Arrow; break;
				case CursorType.None: elem.Cursor = Cursors.None; break;
				case CursorType.SizeNESW: elem.Cursor = Cursors.SizeNESW; break;
				case CursorType.SizeNS: elem.Cursor = Cursors.SizeNS; break;
				case CursorType.SizeNWSE: elem.Cursor = Cursors.SizeNWSE; break;
				case CursorType.SizeWE: elem.Cursor = Cursors.SizeWE; break;
				case CursorType.Wait: elem.Cursor = Cursors.Wait; break;
				case CursorType.Hand: elem.Cursor = Cursors.Hand; break;
				case CursorType.IBeam: elem.Cursor = Cursors.IBeam; break;
				case CursorType.AppStarting: elem.Cursor = Cursors.AppStarting; break;
				case CursorType.ArrowCD: elem.Cursor = Cursors.ArrowCD; break;
				case CursorType.Cross: elem.Cursor = Cursors.Cross; break;
				case CursorType.Help: elem.Cursor = Cursors.Help; break;
				case CursorType.No: elem.Cursor = Cursors.No; break;
				case CursorType.Pen: elem.Cursor = Cursors.Pen; break;
				case CursorType.ScrollAll: elem.Cursor = Cursors.ScrollAll; break;
				case CursorType.ScrollE: elem.Cursor = Cursors.ScrollE; break;
				case CursorType.ScrollN: elem.Cursor = Cursors.ScrollN; break;
				case CursorType.ScrollNE: elem.Cursor = Cursors.ScrollNE; break;
				case CursorType.ScrollNS: elem.Cursor = Cursors.ScrollNS; break;
				case CursorType.ScrollNW: elem.Cursor = Cursors.ScrollNW; break;
				case CursorType.ScrollS: elem.Cursor = Cursors.ScrollS; break;
				case CursorType.ScrollSE: elem.Cursor = Cursors.ScrollSE; break;
				case CursorType.ScrollSW: elem.Cursor = Cursors.ScrollSW; break;
				case CursorType.ScrollW: elem.Cursor = Cursors.ScrollW; break;
				case CursorType.ScrollWE: elem.Cursor = Cursors.ScrollWE; break;
				case CursorType.SizeAll: elem.Cursor = Cursors.SizeAll; break;
				case CursorType.UpArrow: elem.Cursor = Cursors.UpArrow; break;
			}
		}
	}
	public static class RootVisualHelper {
		static UIElement rootVisual = null;
		public static void UpdateRootVisual(DependencyObject dependencyObject) {
			rootVisual = Window.GetWindow(dependencyObject);
			if(rootVisual == null)
				rootVisual = LayoutHelper.FindRoot(dependencyObject) as UIElement;
		}
		public static UIElement RootVisual {
			get {
				return rootVisual;
			}
			private set {
				rootVisual = value;
			}
		}
		public static event EventHandler OnClick;
		public static void SubscribeOnClick() {
			UnsubscribeOnClick();
			SubscribeOnClickCore();
		}
		public static void UnsubscribeOnClick() {
			if(RootVisual == null)
				return;
			if(rootVisual is Window) {
				((Window)RootVisual).Activated -= OnRootVisualActivated;
				((Window)RootVisual).Deactivated -= OnRootVisualDeactivated;
				((Window)RootVisual).LocationChanged -= OnRootVisualLocationChanged;
				((Window)RootVisual).SizeChanged -= OnRootVisualSizeChanged;
				((Window)RootVisual).StateChanged -= OnRootVisualStateChanged;
			}
			RootVisual.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDownHandler));
			RootVisual.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDownHandler));
		}
		static void SubscribeOnClickCore() {
			if(RootVisual == null)
				return;
			if(rootVisual is Window) {
				((Window)RootVisual).Activated += OnRootVisualActivated;
				((Window)RootVisual).Deactivated += OnRootVisualDeactivated;
				((Window)RootVisual).LocationChanged += OnRootVisualLocationChanged;
				((Window)RootVisual).SizeChanged += OnRootVisualSizeChanged;
				((Window)RootVisual).StateChanged += OnRootVisualStateChanged;
			}
			RootVisual.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDownHandler), true);
			RootVisual.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDownHandler), true);
		}
		static void OnRootVisualActivated(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void OnRootVisualDeactivated(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void OnRootVisualLocationChanged(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void OnRootVisualSizeChanged(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void OnRootVisualStateChanged(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void MouseLeftButtonDownHandler(object sender, EventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void KeyDownHandler(object sender, KeyEventArgs e) {
			RaiseClickEvent(sender, e);
		}
		static void RaiseClickEvent(object sender, EventArgs e) {
			if(OnClick != null) {
				OnClick(sender, e);
			}
		}
	}
	public class Wpf2SLOptions : DependencyObject {
		public static bool GetAllowProcessNode(DependencyObject obj) {
			return (bool)obj.GetValue(AllowProcessNodeProperty);
		}
		public static void SetAllowProcessNode(object obj, bool value) {
			if(obj is DependencyObject)
				((DependencyObject)obj).SetValue(AllowProcessNodeProperty, value);
		}
		public static readonly DependencyProperty AllowProcessNodeProperty =
			DependencyProperty.RegisterAttached("AllowProcessNode", typeof(bool), typeof(Wpf2SLOptions), new UIPropertyMetadata(default(bool)));
		public static string GetTag(DependencyObject obj) {
			return (string)obj.GetValue(TagProperty);
		}
		public static void SetTag(DependencyObject obj, string value) {
			obj.SetValue(TagProperty, value);
		}
		public static readonly DependencyProperty TagProperty =
			DependencyProperty.RegisterAttached("Tag", typeof(string), typeof(Wpf2SLOptions), new PropertyMetadata(String.Empty, TagChanged));
		public static void TagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine("Resource Dictionary loaded:" + e.NewValue.ToString());
		}
	}
	public class TransparentPopup {
		private double _HorizontalOffset;
		private bool _IsOpen;
		private double _VerticalOffset;
		public void MakeVisible() {
			MakeVisible(null, null);
		}
		public void MakeVisible(Point? offset) {
			MakeVisible(offset, null);
		}
		public void MakeVisible(Point? offset, Rect? clearZone) {
			if (!offset.HasValue)
				offset = new Point(HorizontalOffset, VerticalOffset);
			offset = PopupExtensions.MakeVisible(PlacementTarget, offset.Value, clearZone, Child);
			HorizontalOffset = offset.Value.X;
			VerticalOffset = offset.Value.Y;
		}
		public void SetOffset(Point offset) {
			HorizontalOffset = offset.X;
			VerticalOffset = offset.Y;
		}
		public void UpdatePosition() {
			if (!IsOpen || PlacementTarget == null)
				return;
			HorizontalOffset += 1;
			HorizontalOffset -= 1;
		}
		public UIElement Child { get; set; }
		public double HorizontalOffset {
			get { return _HorizontalOffset; }
			set {
				if (HorizontalOffset == value)
					return;
				_HorizontalOffset = value;
				UpdatePopupPosition();
				UpdatePositionInHost();
			}
		}
		public bool IsOpen {
			get { return _IsOpen; }
			set {
				if (IsOpen == value)
					return;
				_IsOpen = value;
				if (IsOpen) {
					if (!BrowserInteropHelper.IsBrowserHosted || !ShowInHost())
					ShowPopup();
				}
				else
					if (!HideInHost())
					HidePopup();
			}
		}
		public UIElement PlacementTarget { get; set; }
		public Popup Popup { get; private set; }
		public double VerticalOffset {
			get { return _VerticalOffset; }
			set {
				if (VerticalOffset == value)
					return;
				_VerticalOffset = value;
				UpdatePopupPosition();
				UpdatePositionInHost();
			}
		}
		private void ShowPopup() {
			Popup = new Popup();
			Popup.AllowsTransparency = true;
			Popup.Placement = PlacementMode.Relative;
			Popup.PlacementTarget = PlacementTarget;
			Popup.UseLayoutRounding = true;
			Popup.Unloaded += (o, e) => IsOpen = false;
			Popup.Child = Child;
			Popup.IsOpen = true;
			UpdatePopupPosition();
		}
		private void HidePopup() {
			Popup.Child = null;
			Popup.IsOpen = false;
			Popup = null;
		}
		private void UpdatePopupPosition() {
			if (Popup == null)
				return;
			double horizontalOffset = HorizontalOffset;
			if (SystemParameters.MenuDropAlignment && Popup.Child != null)
				horizontalOffset += Popup.Child.DesiredSize.Width;
			Popup.HorizontalOffset = horizontalOffset;
			Popup.VerticalOffset = VerticalOffset;
		}
		private bool ShowInHost() {
			var page = Application.Current.MainWindow.Content as Page;
			if (page != null) {
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(page);
				if (adornerLayer != null) {
					Host = new PopupAdorner(page);
					adornerLayer.Add(Host);
					Host.Child = Child;
					UpdatePositionInHost();
					return true;
				}
			}
			return false;
		}
		private bool HideInHost() {
			if (Host == null)
				return false;
			Host.Child = null;
			((AdornerLayer)Host.Parent).Remove(Host);
			Host = null;
			return true;
		}
		private void UpdatePositionInHost() {
			if (Host == null)
				return;
			var p = new Point(HorizontalOffset, VerticalOffset);
			if (PlacementTarget != null)
				p = PlacementTarget.TranslatePoint(p, Host);
			else
				p = Host.MapPointFromScreen(p);
			Host.ChildPosition = p;
		}
		private PopupAdorner Host { get; set; }
		private class PopupAdorner : Adorner {
			static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(PopupAdorner));
			static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(PopupAdorner));
			private UIElement _Child;
			private Point _ChildPosition;
			public PopupAdorner(UIElement adornedElement) : base(adornedElement) {
				object rootVisual = Application.Current.MainWindow.Content;
				SetBinding(FontFamilyProperty, new Binding("FontFamily") { Source = rootVisual });
				SetBinding(FontSizeProperty, new Binding("FontSize") { Source = rootVisual });
				Container = new Canvas();
				AddVisualChild(Container);
			}
			public UIElement Child {
				get { return _Child; }
				set {
					if (Child == value)
						return;
					if (Child != null)
						Container.Children.Remove(Child);
					_Child = value;
					if (Child != null)
						Container.Children.Add(Child);
					UpdateChildPosition();
				}
			}
			public Point ChildPosition {
				get { return _ChildPosition; }
				set {
					if (ChildPosition == value)
						return;
					_ChildPosition = value;
					UpdateChildPosition();
				}
			}
			protected override Size ArrangeOverride(Size finalSize) {
				Container.Arrange(new Rect(finalSize));
				return base.ArrangeOverride(finalSize);
			}
			protected override Visual GetVisualChild(int index) {
				return Container;
			}
			protected override Size MeasureOverride(Size constraint) {
				Container.Measure(constraint);
				return base.MeasureOverride(constraint);
			}
			protected void UpdateChildPosition() {
				if (Child == null)
					return;
				Canvas.SetLeft(Child, ChildPosition.X);
				Canvas.SetTop(Child, ChildPosition.Y);
			}
			protected Canvas Container { get; private set; }
			protected override int VisualChildrenCount { get { return Container != null ? 1 : 0; } }
		}
	}
	public interface IDXDomainDataSourceSupport {
		bool SupportDomainDataSource { get; set; }
		bool ShowLoadingPanel { get; set; }
	}
	public interface IDXFilterable {
		CriteriaOperator FilterCriteria { get; set; }
		CriteriaOperator SearchFilterCriteria { get; }
		bool IsFilterEnabled { get; set; }
		event RoutedEventHandler FilterChanged;
	}
}
namespace DevExpress.Xpf.Core.Native {
	public class NamePropertyChangeListener : DependencyObject {
#if DEBUGTEST
		public
#endif
		static bool IsInDesignMode = false;
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(NamePropertyChangeListener), new PropertyMetadata(null, (d, e) => ((NamePropertyChangeListener)d).OnNameChanged()));
		readonly Action nameChangedCallback;
		public static NamePropertyChangeListener CreateDesignTimeOnly(DependencyObject source, Action nameChangedCallback) {
			return new NamePropertyChangeListener(source, nameChangedCallback, true);
		}
		public static NamePropertyChangeListener Create(DependencyObject source, Action nameChangedCallback) {
			return new NamePropertyChangeListener(source, nameChangedCallback, false);
		}
		NamePropertyChangeListener(DependencyObject source, Action nameChangedCallback, bool designTimeOnly) {
			if(designTimeOnly && !DesignerProperties.GetIsInDesignMode(source) && !IsInDesignMode)
				return;
			BindingOperations.SetBinding(this, NameProperty, new Binding("Name") { Source = source });
			this.nameChangedCallback = nameChangedCallback;
		}
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		void OnNameChanged() {
			if(nameChangedCallback != null)
				nameChangedCallback();
		}
	}
	public class PropertyChangeListener : DependencyObject {
		public static readonly DependencyProperty FakeProperty = DependencyProperty.Register("Fake", typeof(object), typeof(PropertyChangeListener), new PropertyMetadata(null, (d, e) => ((PropertyChangeListener)d).OnFakeChanged()));
		readonly Action<object> changedCallback;
		public static PropertyChangeListener Create(Binding binding, Action<object> changedCallback) {
			return new PropertyChangeListener(binding, changedCallback);
		}
		PropertyChangeListener(Binding binding, Action<object> changedCallback) {
			BindingOperations.SetBinding(this, FakeProperty, binding);
			this.changedCallback = changedCallback;
		}
		public object Fake {
			get { return GetValue(FakeProperty); }
			set { SetValue(FakeProperty, value); }
		}
		void OnFakeChanged() {
			if(changedCallback != null)
				changedCallback(Fake);
		}
	}
	public class PropertyChangeListener<T> : DependencyObject {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty FakeProperty = DependencyProperty.Register("Fake", typeof(T), typeof(PropertyChangeListener<T>), new PropertyMetadata(default(T), (d, e) => ((PropertyChangeListener<T>)d).OnFakeChanged()));
		readonly Action<T> changedCallback;
		public static PropertyChangeListener<T> Create(Binding binding, Action<T> changedCallback) {
			return new PropertyChangeListener<T>(binding, changedCallback);
		}
		PropertyChangeListener(Binding binding, Action<T> changedCallback) {
			BindingOperations.SetBinding(this, FakeProperty, binding);
			this.changedCallback = changedCallback;
		}
		public int ChangeCount { get; private set; }
		public T Fake {
			get { return (T)GetValue(FakeProperty); }
			set { SetValue(FakeProperty, value); }
		}
		void OnFakeChanged() {
			ChangeCount++;
			if(changedCallback != null)
				changedCallback(Fake);
		}
	}
#endif
}
