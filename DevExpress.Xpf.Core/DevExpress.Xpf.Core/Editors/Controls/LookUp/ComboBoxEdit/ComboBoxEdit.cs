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
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ContentProperty("Items")]
#if !SL
	[DataAccessMetadata("All", SupportedProcessingModes = "GridLookup", EnableBindingToEnum = true)]
#endif
	public class ComboBoxEdit : LookUpEditBase, ISelectorEdit {
		public static readonly DependencyProperty ShowCustomItemsProperty;
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		internal protected static readonly DependencyProperty ApplyImageTemplateToSelectedItemProperty;
		static ComboBoxEdit() {
			Type ownerType = typeof(ComboBoxEdit);
			PopupMaxHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));
			ShowCustomItemsProperty = DependencyPropertyManager.Register("ShowCustomItems", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ComboBoxEdit)d).ShowCustomItemsChanged((bool?)e.NewValue)));
			ApplyImageTemplateToSelectedItemProperty = DependencyPropertyManager.Register("ApplyImageTemplateToSelectedItem", typeof(bool), ownerType, new UIPropertyMetadata(false));
		}
		public ComboBoxEdit() {
			this.SetDefaultStyleKey(typeof(ComboBoxEdit));
#if SL
			PopupMaxHeight = GetDefaultPopupMaxHeight();
#endif
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ComboBoxEditItems"),
#endif
		Bindable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Common Properties")]
		public ListItemCollection Items { get { return Settings.Items; } }
#if !SL
		public ObservableCollection<GroupStyle> GroupStyle { get { return Settings.GroupStyle; } }
#else
		[TypeConverter(typeof(NullableConverter<bool>))]
#endif
		[Category("Behavior")]
		public bool? ShowCustomItems {
			get { return (bool?)GetValue(ShowCustomItemsProperty); }
			set { SetValue(ShowCustomItemsProperty, value); }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ApplyImageTemplateToSelectedItem {
			get { return (bool)GetValue(ApplyImageTemplateToSelectedItemProperty); }
			protected set { this.SetValue(ApplyImageTemplateToSelectedItemProperty, value); }
		}
		protected internal new ComboBoxEditSettings Settings { get { return base.Settings as ComboBoxEditSettings; } }
		protected override EditStrategyBase CreateEditStrategy() {
			return new ComboBoxEditStrategy(this);
		}
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new ComboBoxStyleSettings();
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ComboBoxEditAutomationPeer(this);
		}
		protected internal override FrameworkElement PopupElement {
			get { return ListBox as FrameworkElement; }
		}
		protected virtual void ShowCustomItemsChanged(bool? value) {
			EditStrategy.ShowCustomItemsChanged(value);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if (IsPopupOpen)
				(ListBox as PopupListBox).Do(x => x.SetEditBoxMousePosition());
		}
#if DEBUGTEST
		public
#else
		internal protected
#endif
 ISelectorEditInnerListBox ListBox { get { return EditStrategy.GetVisualClient().InnerEditor as ISelectorEditInnerListBox; } }
		ObservableCollection<GroupStyle> ISelectorEdit.GroupStyle { get { return GroupStyle; } }
		SelectionEventMode ISelectorEdit.SelectionEventMode { get { return SelectionEventMode.MouseDown; } }
		ISelectionProvider ISelectorEdit.SelectionProvider { get { return new SelectionProvider(ListBox); } }
		ListItemCollection ISelectorEdit.Items { get { return Items; } }
		internal new ComboBoxEditStrategy EditStrategy {
			get { return base.EditStrategy as ComboBoxEditStrategy; }
			set { base.EditStrategy = value; }
		}
		protected override VisualClientOwner CreateVisualClient() {
			return new ListBoxVisualClientOwner(this);
		}
		protected override void ItemsSourceChanged(object itemsSource) {
			base.ItemsSourceChanged(itemsSource);
			ApplyImageTemplateToSelectedItem = EnumItemsSource.IsEnumItemsSource(itemsSource);
		}
	}
	[DXToolboxBrowsable(false)]
	public class SelectAllItemCheckEdit : CheckEdit {
		new CheckEditStrategy EditStrategy { get { return (CheckEditStrategy)base.EditStrategy; } }
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) { }
		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) { }
	}
	internal class SelectedItemValueRenderer {
		UIElement element;
		readonly LookUpEditBase edit;
		readonly Locker renderLocker = new Locker();
		public SelectedItemValueRenderer(LookUpEditBase edit) {
			this.edit = edit;
		}
		public UIElement Element {
			get { return element; }
			set {
				if (Element == value)
					return;
#if !SL
				if (Element != null)
					Element.LayoutUpdated -= OnElementLayoutUpdated;
#endif
				element = value;
#if !SL
				if (Element != null)
					Element.LayoutUpdated += OnElementLayoutUpdated;
#endif
			}
		}
#if !SL
		protected virtual void OnElementLayoutUpdated(object sender, EventArgs e) {
			Rectangle selectedItemValue = (Rectangle)edit.SelectedItemValue;
			if (selectedItemValue == null)
				return;
			selectedItemValue.Width = Element.RenderSize.Width;
			selectedItemValue.Height = Element.RenderSize.Height;
			VisualBrush fill = (VisualBrush)selectedItemValue.Fill;
			fill.Viewbox = new Rect(Element.RenderSize);
			fill.Viewport = new Rect(Element.RenderSize);
		}
#endif
		public void Render() {
			renderLocker.DoLockedActionIfNotLocked(RenderCore);
		}
		private void RenderCore() {
			if (Element == null)
				return;
			Transform transform = null;
			FlowDirection flowDirection = GetElementFlowDirection();
			if (flowDirection == FlowDirection.RightToLeft)
				transform = new MatrixTransform { Matrix = new Matrix(-1.0, 0.0, 0.0, 1.0, Element.RenderSize.Width, 0.0) };
#if !SL
			VisualBrush brush = new VisualBrush(Element) { Stretch = Stretch.None, ViewboxUnits = BrushMappingMode.Absolute, ViewportUnits = BrushMappingMode.Absolute };
			brush.Viewbox = new Rect(Element.RenderSize);
			brush.Viewport = new Rect(Element.RenderSize);
			brush.Transform = transform;
#else
			edit.SelectedItemValue = Element;
			Element.UpdateLayout();
			ImageBrush brush = new ImageBrush();
			WriteableBitmap bitmap = new WriteableBitmap((int)Element.RenderSize.Width, (int)Element.RenderSize.Height);
			bitmap.Render(element, transform);
			bitmap.Invalidate();
			brush.ImageSource = bitmap;
#endif
			edit.SelectedItemValue = new Rectangle { Fill = brush, Width = Element.RenderSize.Width, Height = Element.RenderSize.Height };
		}
		FlowDirection GetElementFlowDirection() {
			FlowDirection elementFlowDirection = (FlowDirection)Element.GetValue(FrameworkElement.FlowDirectionProperty);
			DependencyObject parent = VisualTreeHelper.GetParent(element);
			return (parent == null) ? elementFlowDirection : ((FlowDirection)parent.GetValue(FrameworkElement.FlowDirectionProperty));
		}
	}
	public class EnumItemsSource : MarkupExtension {
		public EnumItemsSource() {
			SplitNames = true;
			AllowImages = true;
		}
		public Type EnumType { get; set; }
		public bool UseNumericEnumValue { get; set; }
		public bool SplitNames { get; set; }
		public IValueConverter NameConverter { get; set; }
		public EnumMembersSortMode SortMode { get; set; }
		public bool AllowImages { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return DevExpress.Xpf.Core.EnumHelper.GetEnumSource(EnumType, UseNumericEnumValue, NameConverter, SplitNames, SortMode, AllowImages);
		}
		public static bool IsEnumItemsSource(object itemsSource) {
			return itemsSource is IEnumerable<EnumMemberInfo>;
		}
		public static void SetupEnumItemsSource(object itemsSource, Action setupCallback) {
			if (IsEnumItemsSource(itemsSource))
				setupCallback();
		}
	}
}
namespace DevExpress.Xpf.Editors.Native {
	public class LookUpEditEnumItem<T> {
		public string Text { get; set; }
		public T Value { get; set; }
	}
}
