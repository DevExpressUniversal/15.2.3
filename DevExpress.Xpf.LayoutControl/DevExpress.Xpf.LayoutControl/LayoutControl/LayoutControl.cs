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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.LayoutControl.Serialization;
namespace DevExpress.Xpf.LayoutControl {
	public interface ILayoutControlModel : ILayoutGroupModel {
	}
	public interface ILayoutControl : ILayoutGroup, ILayoutControlModel {
		void ControlAdded(FrameworkElement control);
		void ControlRemoved(FrameworkElement control);
		void ControlVisibilityChanged(FrameworkElement control);
		LayoutGroup CreateGroup();
		void DeleteAvailableItem(FrameworkElement item);
		FrameworkElement FindControl(string id);
		string GetID(FrameworkElement control);
		void SetID(FrameworkElement control, string id);
		Style GetPartStyle(LayoutGroupPartStyle style);
		void InitCustomizationController();
		void InitNewElement(FrameworkElement element);
		bool MakeControlVisible(FrameworkElement control);
		void ModelChanged(LayoutControlModelChangedEventArgs args);
		void TabClicked(ILayoutGroup group, FrameworkElement selectedTabChild);
		void ReadElementFromXML(XmlReader xml, FrameworkElement element);
		void WriteElementToXML(XmlWriter xml, FrameworkElement element);
		void WriteToXML(XmlWriter xml);
		bool AllowItemSizing { get; }
		FrameworkElements AvailableItems { get; }
		FrameworkElements VisibleAvailableItems { get; }
	}
	public class LayoutControlInitNewElementEventArgs : EventArgs {
		public LayoutControlInitNewElementEventArgs(FrameworkElement element) {
			Element = element;
		}
		public FrameworkElement Element { get; private set; }
	}
	public class LayoutControlReadElementFromXMLEventArgs : EventArgs {
		public LayoutControlReadElementFromXMLEventArgs(XmlReader xml, FrameworkElement element) {
			Xml = xml;
			Element = element;
		}
		public FrameworkElement Element { get; private set; }
		public XmlReader Xml { get; private set; }
	}
	public class LayoutControlWriteElementToXMLEventArgs : EventArgs {
		public LayoutControlWriteElementToXMLEventArgs(XmlWriter xml, FrameworkElement element) {
			Xml = xml;
			Element = element;
		}
		public FrameworkElement Element { get; private set; }
		public XmlWriter Xml { get; private set; }
	}
#if !SILVERLIGHT
#endif
	[StyleTypedProperty(Property = "CustomizationControlStyle", StyleTargetType = typeof(LayoutControlCustomizationControl))]
	[StyleTypedProperty(Property = "ItemCustomizationToolbarStyle", StyleTargetType = typeof(LayoutItemCustomizationToolbar))]
	[StyleTypedProperty(Property = "ItemInsertionPointIndicatorStyle", StyleTargetType = typeof(LayoutItemDragAndDropInsertionPointIndicator))]
	[StyleTypedProperty(Property = "ItemParentIndicatorStyle", StyleTargetType = typeof(LayoutItemParentIndicator))]
	[StyleTypedProperty(Property = "ItemSelectionIndicatorStyle", StyleTargetType = typeof(LayoutItemSelectionIndicator))]
	[StyleTypedProperty(Property = "ItemSizerStyle", StyleTargetType = typeof(ElementSizer))]
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class LayoutControl : LayoutGroup, ILayoutControl {
		#region Dependency Properties
		public static readonly DependencyProperty ActualAllowAvailableItemsDuringCustomizationProperty =
			DependencyProperty.Register("ActualAllowAvailableItemsDuringCustomization", typeof(bool), typeof(LayoutControl), null);
		public static readonly DependencyProperty AllowAvailableItemsDuringCustomizationProperty =
			DependencyProperty.Register("AllowAvailableItemsDuringCustomization", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(true, (o, e) => ((LayoutControl)o).OnAllowAvailableItemsDuringCustomizationChanged()));
		public static readonly DependencyProperty AllowItemMovingDuringCustomizationProperty =
			DependencyProperty.Register("AllowItemMovingDuringCustomization", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(true, (o, e) => ((LayoutControl)o).OnAllowItemMovingDuringCustomizationChanged()));
		public static readonly DependencyProperty AllowItemRenamingDuringCustomizationProperty =
			DependencyProperty.Register("AllowItemRenamingDuringCustomization", typeof(bool), typeof(LayoutControl), new PropertyMetadata(true));
		public static readonly DependencyProperty AllowItemSizingProperty =
			DependencyProperty.Register("AllowItemSizing", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(true, (o, e) => ((LayoutControl)o).OnAllowItemSizingChanged()));
		public static readonly DependencyProperty AllowItemSizingDuringCustomizationProperty =
			DependencyProperty.Register("AllowItemSizingDuringCustomization", typeof(bool), typeof(LayoutControl), new PropertyMetadata(true));
		public static readonly DependencyProperty AllowNewItemsDuringCustomizationProperty =
			DependencyProperty.Register("AllowNewItemsDuringCustomization", typeof(bool), typeof(LayoutControl), new PropertyMetadata(true));
		public static readonly DependencyProperty AllowHorizontalSizingProperty =
			DependencyProperty.RegisterAttached("AllowHorizontalSizing", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty AllowVerticalSizingProperty =
			DependencyProperty.RegisterAttached("AllowVerticalSizing", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty CustomizationControlStyleProperty =
			DependencyProperty.Register("CustomizationControlStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((ILayoutControl)o).InitCustomizationController()));
		public static readonly DependencyProperty CustomizationLabelProperty =
			DependencyProperty.RegisterAttached("CustomizationLabel", typeof(object), typeof(LayoutControl), null);
		public static readonly DependencyProperty IsCustomizationProperty =
			DependencyProperty.Register("IsCustomization", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						if (!o.IsInDesignTool())
							((LayoutControl)o).Controller.IsCustomization = (bool)e.NewValue;
					}));
		private static readonly DependencyProperty IsUserDefinedProperty =
			DependencyProperty.RegisterAttached("IsUserDefined", typeof(bool), typeof(LayoutControl), null);
		public static readonly DependencyProperty ItemCustomizationToolbarStyleProperty =
			DependencyProperty.Register("ItemCustomizationToolbarStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((ILayoutControl)o).InitCustomizationController()));
		public static readonly DependencyProperty ItemInsertionPointIndicatorStyleProperty =
			DependencyProperty.Register("ItemInsertionPointIndicatorStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((LayoutControl)o).Controller.ItemInsertionPointIndicatorStyle = (Style)e.NewValue));
		public static readonly DependencyProperty ItemParentIndicatorStyleProperty =
			DependencyProperty.Register("ItemParentIndicatorStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((ILayoutControl)o).InitCustomizationController()));
		public static readonly DependencyProperty ItemSelectionIndicatorStyleProperty =
			DependencyProperty.Register("ItemSelectionIndicatorStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((ILayoutControl)o).InitCustomizationController()));
		public static readonly DependencyProperty ItemSizerStyleProperty =
			DependencyProperty.Register("ItemSizerStyle", typeof(Style), typeof(LayoutControl),
				new PropertyMetadata((o, e) => ((LayoutControl)o).SetItemSizerStyle((Style)e.NewValue)));
		public static readonly DependencyProperty LayoutUriProperty =
			DependencyProperty.Register("LayoutUri", typeof(Uri), typeof(LayoutControl),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutControl)o;
						if(control.IsLoaded)
							control.OnLayoutUriChanged();
						else
							control._IsLayoutUriChanged = true;
					}));
		public static readonly DependencyProperty MovingItemPlaceHolderBrushProperty =
			DependencyProperty.Register("MovingItemPlaceHolderBrush", typeof(Brush), typeof(LayoutControl),
				new PropertyMetadata(DefaultMovingItemPlaceHolderBrush));
		public static readonly DependencyProperty StretchContentHorizontallyProperty =
			DependencyProperty.Register("StretchContentHorizontally", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(true, (o, e) => ((LayoutControl)o).OnStretchContentChanged()));
		public static readonly DependencyProperty StretchContentVerticallyProperty =
			DependencyProperty.Register("StretchContentVertically", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(true, (o, e) => ((LayoutControl)o).OnStretchContentChanged()));
		public static readonly DependencyProperty TabHeaderProperty =
			DependencyProperty.RegisterAttached("TabHeader", typeof(object), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty TabHeaderTemplateProperty =
			DependencyProperty.RegisterAttached("TabHeaderTemplate", typeof(DataTemplate), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty UseDesiredWidthAsMaxWidthProperty =
			DependencyProperty.RegisterAttached("UseDesiredWidthAsMaxWidth", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty UseDesiredHeightAsMaxHeightProperty =
			DependencyProperty.RegisterAttached("UseDesiredHeightAsMaxHeight", typeof(bool), typeof(LayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static bool GetAllowHorizontalSizing(UIElement element) {
			return (bool)element.GetValue(AllowHorizontalSizingProperty);
		}
		public static void SetAllowHorizontalSizing(UIElement element, bool value) {
			element.SetValue(AllowHorizontalSizingProperty, value);
		}
		public static bool GetAllowVerticalSizing(UIElement element) {
			return (bool)element.GetValue(AllowVerticalSizingProperty);
		}
		public static void SetAllowVerticalSizing(UIElement element, bool value) {
			element.SetValue(AllowVerticalSizingProperty, value);
		}
		public static object GetCustomizationLabel(UIElement element) {
			return (object)element.GetValue(CustomizationLabelProperty);
		}
		public static void SetCustomizationLabel(UIElement element, object value) {
			element.SetValue(CustomizationLabelProperty, value);
		}
		public static bool GetIsUserDefined(UIElement element) {
			return (bool)element.GetValue(IsUserDefinedProperty);
		}
		public static void SetIsUserDefined(UIElement element, bool value) {
			if (!element.IsInDesignTool())
				element.SetValue(IsUserDefinedProperty, value);
		}
		public static object GetTabHeader(UIElement element) {
			return element.GetValue(TabHeaderProperty);
		}
		public static void SetTabHeader(UIElement element, object value) {
			element.SetValue(TabHeaderProperty, value);
		}
		public static DataTemplate GetTabHeaderTemplate(UIElement element) {
			return (DataTemplate)element.GetValue(TabHeaderTemplateProperty);
		}
		public static void SetTabHeaderTemplate(UIElement element, DataTemplate value) {
			element.SetValue(TabHeaderTemplateProperty, value);
		}
		public static bool GetUseDesiredWidthAsMaxWidth(UIElement element) {
			return (bool)element.GetValue(UseDesiredWidthAsMaxWidthProperty);
		}
		public static void SetUseDesiredWidthAsMaxWidth(UIElement element, bool value) {
			element.SetValue(UseDesiredWidthAsMaxWidthProperty, value);
		}
		public static bool GetUseDesiredHeightAsMaxHeight(UIElement element) {
			return (bool)element.GetValue(UseDesiredHeightAsMaxHeightProperty);
		}
		public static void SetUseDesiredHeightAsMaxHeight(UIElement element, bool value) {
			element.SetValue(UseDesiredHeightAsMaxHeightProperty, value);
		}
		#endregion Dependency Properties
		#region Internal ID
		private static readonly DependencyProperty IDProperty = DependencyProperty.RegisterAttached("ID", typeof(int), typeof(LayoutControl), null);
		private int _LastID;
		private string GetID(FrameworkElement control) {
			if(string.IsNullOrEmpty(control.Name))
				return ((int)control.GetValue(IDProperty)).ToString();
			else
				return control.Name;
		}
		private void SetID(FrameworkElement control) {
			_LastID++;
			control.SetValue(IDProperty, _LastID);
		}
		private void SetID(FrameworkElement control, string id) {
			int internalID;
			if (int.TryParse(id, out internalID)) {
				control.SetValue(IDProperty, internalID);
				if (internalID > _LastID)
					_LastID = internalID;
			}
		}
		private void UpdateID(FrameworkElement control) {
			if (!control.IsPropertyAssigned(IDProperty))
				SetID(control);
			else {
				var id = (int)control.GetValue(IDProperty);
				if (id > _LastID)
					SetID(control);
			}
			if (control.IsLayoutGroup())
				foreach (FrameworkElement child in ((ILayoutGroup)control).GetLogicalChildren(false))
					UpdateID(child);
		}
		private FrameworkElement FindControl(string id, FrameworkElement exception = null) {
			int internalID;
			if (int.TryParse(id, out internalID)) {
				FrameworkElement result = FindControl(AvailableItems, id, exception);
				if (result == null)
					result = FindControl(this, id, exception);
				return result;
			}
			else
				return (FrameworkElement)FindName(id);
		}
		private FrameworkElement FindControl(ILayoutGroup group, string id, FrameworkElement exception) {
			return FindControl(group.GetLogicalChildren(false), id, exception);
		}
		private FrameworkElement FindControl(FrameworkElements controls, string id, FrameworkElement exception) {
			foreach (FrameworkElement control in controls) {
				if (control != exception && GetID(control) == id)
					return control;
				if (control.IsLayoutGroup()) {
					FrameworkElement result = FindControl((ILayoutGroup)control, id, exception);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		#endregion Internal ID
#if !SILVERLIGHT
		static LayoutControl() {
			PaddingProperty.OverrideMetadata(typeof(LayoutControl), new PropertyMetadata(new Thickness(DefaultPadding)));
			DXSerializer.SerializationIDDefaultProperty.OverrideMetadata(typeof(LayoutControl), new UIPropertyMetadata(Serialization.SerializationController.DefaultID));			
		}
#endif
		public static string GetCustomizationDefaultLabel(UIElement element) {
			string result = null;
			if (element is ContentControl)
				result = ((ContentControl)element).Content as string;
			else
				if (element is ContentControlBase)
					result = ((ContentControlBase)element).Content as string;
				else
					if (element is LayoutGroup)
						result = ((LayoutGroup)element).Header as string;
					else
						if (element is LayoutItem) {
							result = ((LayoutItem)element).Label as string;
							if (!string.IsNullOrEmpty(result))
								result = result.TrimEnd(':', ' ');
						}
			if (string.IsNullOrEmpty(result))
				result = "[" + element.GetType().Name + "]";
			return result;
		}
		private FrameworkElements _AvailableItems;
		private bool _IsLayoutUriChanged;
		public LayoutControl() {
			_AvailableItems = new FrameworkElements();
			AvailableItems.CollectionChanged += (o, e) => OnAvailableItemsChanged(e);
			VisibleAvailableItems = new FrameworkElements();
			AvailableItemsContainer = new ItemsContainer { Visibility = Visibility.Collapsed };
			AvailableItemsContainer.ChildVisibilityChanged = delegate(FrameworkElement child) {
				if (child.GetVisible())
					VisibleAvailableItems.Add(child);
				else
					VisibleAvailableItems.Remove(child);
			};
#if SILVERLIGHT
			Children.Add(AvailableItemsContainer);
#endif
			UpdateActualAllowAvailableItemsDuringCustomization();
			serializationControllerCore = CreateSerializationController();			
		}
		public override void ReadFromXML(XmlReader xml) {
			base.ReadFromXML(xml);
			OptimizeLayout(true);
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowAvailableItemsDuringCustomization")]
#endif
		public bool AllowAvailableItemsDuringCustomization {
			get { return (bool)GetValue(AllowAvailableItemsDuringCustomizationProperty); }
			set { SetValue(AllowAvailableItemsDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowItemMovingDuringCustomization")]
#endif
		public bool AllowItemMovingDuringCustomization {
			get { return (bool)GetValue(AllowItemMovingDuringCustomizationProperty); }
			set { SetValue(AllowItemMovingDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowItemRenamingDuringCustomization")]
#endif
		public bool AllowItemRenamingDuringCustomization {
			get { return (bool)GetValue(AllowItemRenamingDuringCustomizationProperty); }
			set { SetValue(AllowItemRenamingDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowItemSizing")]
#endif
		public bool AllowItemSizing {
			get { return (bool)GetValue(AllowItemSizingProperty); }
			set { SetValue(AllowItemSizingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowItemSizingDuringCustomization")]
#endif
		public bool AllowItemSizingDuringCustomization {
			get { return (bool)GetValue(AllowItemSizingDuringCustomizationProperty); }
			set { SetValue(AllowItemSizingDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAllowNewItemsDuringCustomization")]
#endif
		public bool AllowNewItemsDuringCustomization {
			get { return (bool)GetValue(AllowNewItemsDuringCustomizationProperty); }
			set { SetValue(AllowNewItemsDuringCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlAvailableItems")]
#endif
		public FrameworkElements AvailableItems { get { return _AvailableItems; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlCustomizationControlStyle")]
#endif
		public Style CustomizationControlStyle {
			get { return (Style)GetValue(CustomizationControlStyleProperty); }
			set { SetValue(CustomizationControlStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlController")]
#endif
		public new LayoutControlController Controller { get { return (LayoutControlController)base.Controller; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlIsCustomization")]
#endif
		public new bool IsCustomization {
			get { return (bool)GetValue(IsCustomizationProperty); }
			set { SetValue(IsCustomizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlIsRoot")]
#endif
		public override bool IsRoot { get { return true; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemCustomizationToolbarStyle")]
#endif
		public Style ItemCustomizationToolbarStyle {
			get { return (Style)GetValue(ItemCustomizationToolbarStyleProperty); }
			set { SetValue(ItemCustomizationToolbarStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemInsertionPointIndicatorStyle")]
#endif
		public Style ItemInsertionPointIndicatorStyle {
			get { return (Style)GetValue(ItemInsertionPointIndicatorStyleProperty); }
			set { SetValue(ItemInsertionPointIndicatorStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemLabelsAlignment")]
#endif
		public override LayoutItemLabelsAlignment ItemLabelsAlignment {
			get { return LayoutItemLabelsAlignment.Local; }
			set { }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemParentIndicatorStyle")]
#endif
		public Style ItemParentIndicatorStyle {
			get { return (Style)GetValue(ItemParentIndicatorStyleProperty); }
			set { SetValue(ItemParentIndicatorStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemSelectionIndicatorStyle")]
#endif
		public Style ItemSelectionIndicatorStyle {
			get { return (Style)GetValue(ItemSelectionIndicatorStyleProperty); }
			set { SetValue(ItemSelectionIndicatorStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlItemSizerStyle")]
#endif
		public new Style ItemSizerStyle {
			get { return (Style)GetValue(ItemSizerStyleProperty); }
			set { SetValue(ItemSizerStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlLayoutUri")]
#endif
		public Uri LayoutUri {
			get { return (Uri)GetValue(LayoutUriProperty); }
			set { SetValue(LayoutUriProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlMovingItemPlaceHolderBrush")]
#endif
		public Brush MovingItemPlaceHolderBrush {
			get { return (Brush)GetValue(MovingItemPlaceHolderBrushProperty); }
			set { SetValue(MovingItemPlaceHolderBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlStretchContentHorizontally")]
#endif
		public bool StretchContentHorizontally {
			get { return (bool)GetValue(StretchContentHorizontallyProperty); }
			set { SetValue(StretchContentHorizontallyProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlStretchContentVertically")]
#endif
		public bool StretchContentVertically {
			get { return (bool)GetValue(StretchContentVerticallyProperty); }
			set { SetValue(StretchContentVerticallyProperty, value); }
		}
		public event EventHandler<LayoutControlInitNewElementEventArgs> InitNewElement;
		public event EventHandler IsCustomizationChanged {
			add { Controller.IsCustomizationChanged += value; }
			remove { Controller.IsCustomizationChanged -= value; }
		}
		public event EventHandler<LayoutControlReadElementFromXMLEventArgs> ReadElementFromXML;
		public event EventHandler<LayoutControlWriteElementToXMLEventArgs> WriteElementToXML;
		#region Children
		protected override FrameworkElement GetChildContainer(FrameworkElement child) {
			return child.GetLayoutItem(this, false) ?? base.GetChildContainer(child);
		}
		protected override IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in base.GetInternalElements())
				yield return element;
			yield return AvailableItemsContainer;
		}
		protected virtual void OnControlAdded(FrameworkElement control) {
			UpdateID(control);
			AvailableItems.Remove(control);
		}
		protected virtual void OnControlRemoved(FrameworkElement control) {
		}
		#endregion Children
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			if(!StretchContentHorizontally)
				availableSize.Width = double.PositiveInfinity;
			if(!StretchContentVertically)
				availableSize.Height = double.PositiveInfinity;
			return base.OnMeasure(availableSize);
		}
		protected override Size OnArrange(Rect bounds) {
			var result = bounds.Size();
			if(!StretchContentHorizontally || bounds.Width < OriginalDesiredSize.Width)
				bounds.Width = OriginalDesiredSize.Width;
			if(!StretchContentVertically || bounds.Height < OriginalDesiredSize.Height)
				bounds.Height = OriginalDesiredSize.Height;
			base.OnArrange(bounds);
			return result;
		}
		#endregion Layout
		#region GroupBox
		protected override bool HasGroupBox { get { return false; } }
		#endregion GroupBox
		#region Tabs
		protected override bool HasTabs { get { return false; } }
		#endregion Tabs
		#region XML Storage
		const string AvailableItemsXMLNodeName = "AvailableItems";
		protected override void AddChildFromXML(IList children, FrameworkElement element, int index) {
			if (children == AvailableItems && children.Contains(element))
				return;
			base.AddChildFromXML(children, element, index);
		}
		protected override FrameworkElement ReadChildFromXML(XmlReader xml, IList children, int index) {
			if (xml.Name == AvailableItemsXMLNodeName) {
				FrameworkElement lastChild;
				ReadChildrenFromXML(AvailableItems, xml, 0, out lastChild);
				return null;
			}
			else
				return base.ReadChildFromXML(xml, children, index);
		}
		protected override void WriteToXMLCore(XmlWriter xml) {
			base.WriteToXMLCore(xml);
			xml.WriteStartElement(AvailableItemsXMLNodeName);
			WriteChildrenToXML(AvailableItems, xml);
			xml.WriteEndElement();
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new LayoutControlController(this);
		}
		protected override Type GetGroupType() {
			return typeof(LayoutGroup);
		}
#if SILVERLIGHT
		protected override Thickness GetDefaultPadding() {
			return new Thickness(DefaultPadding);
		}
#endif
		protected virtual void InitCustomizationController(LayoutControlCustomizationController controller) {
			controller.CustomizationControlStyle = CustomizationControlStyle;
			controller.ItemCustomizationToolbarStyle = ItemCustomizationToolbarStyle;
			controller.ItemParentIndicatorStyle = ItemParentIndicatorStyle;
			controller.ItemSelectionIndicatorStyle = ItemSelectionIndicatorStyle;
		}
		protected virtual void OnAllowAvailableItemsDuringCustomizationChanged() {
			UpdateActualAllowAvailableItemsDuringCustomization();
		}
		protected virtual void OnAllowItemMovingDuringCustomizationChanged() {
			UpdateActualAllowAvailableItemsDuringCustomization();
		}
		protected virtual void OnAvailableItemsChanged(NotifyCollectionChangedEventArgs args) {
			IList items;
			if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Replace)
				items = args.NewItems;
			else
				if (args.Action == NotifyCollectionChangedAction.Reset)
					items = AvailableItems;
				else
					items = null;
			if (items != null)
				foreach (FrameworkElement item in items) {
					item.SetParent(null);
					UpdateID(item);
				}
			SynchronizeWithAvailableItems(VisibleAvailableItems, args, item => item.GetVisible());
			SynchronizeWithAvailableItems(AvailableItemsContainer.Children, args);
		}
#if !SILVERLIGHT
		protected override void OnInitialized(EventArgs e) {
			Children.Add(AvailableItemsContainer);
			base.OnInitialized(e);
		}
#endif
		protected virtual void OnInitNewElement(FrameworkElement element) {
			SetIsUserDefined(element, true);
			if (InitNewElement != null)
				InitNewElement(this, new LayoutControlInitNewElementEventArgs(element));
		}
		protected override void OnItemLabelsAlignmentChanged() {
		}
		protected override void OnLayoutUpdated() {
			if (Controller.IsCustomization)
				Controller.CustomizationController.CheckSelectedElementsAreInVisualTree();
			base.OnLayoutUpdated();
		}
		protected virtual void OnLayoutUriChanged() {
			if(LayoutUri == null)
				return;
			var resource = Application.GetResourceStream(LayoutUri);
			if(resource != null)
				ReadFromXML(XmlReader.Create(resource.Stream));
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if(_IsLayoutUriChanged) {
				_IsLayoutUriChanged = false;
				OnLayoutUriChanged();
			}
		}
		protected override void OnPartStyleChanged(LayoutGroupPartStyle style) {
			((ILayoutControl)this).UpdatePartStyle(style);
		}
		protected virtual void OnStretchContentChanged() {
			Changed();
		}
		protected virtual void OnReadElementFromXML(XmlReader xml, FrameworkElement element) {
			if (ReadElementFromXML != null)
				ReadElementFromXML(this, new LayoutControlReadElementFromXMLEventArgs(xml, element));
		}
		protected virtual void OnWriteElementToXML(XmlWriter xml, FrameworkElement element) {
			if (WriteElementToXML != null)
				WriteElementToXML(this, new LayoutControlWriteElementToXMLEventArgs(xml, element));
		}
		protected void SynchronizeWithAvailableItems(IList items, NotifyCollectionChangedEventArgs args, Predicate<FrameworkElement> isItemAllowed = null) {
			Action<IList> AddItems = delegate(IList newItems) {
				foreach (FrameworkElement item in newItems)
					if (isItemAllowed == null || isItemAllowed(item))
						items.Add(item);
			};
			Action<IList> RemoveItems = delegate(IList oldItems) {
				foreach (FrameworkElement item in oldItems)
					items.Remove(item);
			};
			switch (args.Action) {
				case NotifyCollectionChangedAction.Add:
					AddItems(args.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemoveItems(args.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					RemoveItems(args.OldItems);
					AddItems(args.NewItems);
					break;
				default:
					items.Clear();
					AddItems(AvailableItems);
					break;
			}
		}
		protected override bool IsBorderless { get { return false; } }
		protected FrameworkElements VisibleAvailableItems { get; private set; }
		private void SetItemSizerStyle(Style value) {
			base.ItemSizerStyle = value;
		}
		private void UpdateActualAllowAvailableItemsDuringCustomization() {
			SetValue(ActualAllowAvailableItemsDuringCustomizationProperty,
				AllowItemMovingDuringCustomization && AllowAvailableItemsDuringCustomization);
		}
		private ItemsContainer AvailableItemsContainer { get; set; }
		internal class ItemsContainer : PanelBase {
			public Action<FrameworkElement> ChildVisibilityChanged { get; set; }
			protected static DependencyProperty ChildVisibilityListener = RegisterChildPropertyListener("Visibility", typeof(ItemsContainer));
			protected override void OnChildPropertyChanged(FrameworkElement child, DependencyProperty propertyListener, object oldValue, object newValue) {
				base.OnChildPropertyChanged(child, propertyListener, oldValue, newValue);
				if (propertyListener == ChildVisibilityListener &&
					newValue != null &&	 
					!newValue.Equals(oldValue))
					OnChildVisibilityChanged(child);
			}
			protected virtual void OnChildVisibilityChanged(FrameworkElement child) {
				if (ChildVisibilityChanged != null)
					ChildVisibilityChanged(child);
			}
			protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
				base.OnVisualChildrenChanged(visualAdded, visualRemoved);
				if (visualAdded is FrameworkElement)
					AttachChildPropertyListener((FrameworkElement)visualAdded, "Visibility", ChildVisibilityListener);
				if (visualRemoved is FrameworkElement)
					DetachChildPropertyListener((FrameworkElement)visualRemoved, ChildVisibilityListener);
			}
		}
		#region ILayoutControlBase
		bool ILayoutControlBase.AllowItemMoving { get { return this.IsInDesignTool() || AllowItemMovingDuringCustomization; } }
		#endregion ILayoutControlBase
		#region ILayoutGroup
		Rect ILayoutGroup.ChildAreaBounds { get { return ChildrenBounds; } }
		bool ILayoutGroup.IsCustomization {
			get { return base.IsCustomization; }
			set {
				base.IsCustomization = value;
				if (!this.IsInDesignTool() && IsCustomization != value)	
					IsCustomization = value;
			}
		}
		#endregion ILayoutGroup
		#region ILayoutControl
		void ILayoutControl.ControlAdded(FrameworkElement control) {
			OnControlAdded(control);
		}
		void ILayoutControl.ControlRemoved(FrameworkElement control) {
			OnControlRemoved(control);
		}
		void ILayoutControl.ControlVisibilityChanged(FrameworkElement control) {
			Controller.OnControlVisibilityChanged(control);
		}
		void ILayoutControl.DeleteAvailableItem(FrameworkElement item) {
			if (item.IsLayoutGroup())
				((ILayoutGroup)item).MoveNonUserDefinedChildrenToAvailableItems();
			AvailableItems.Remove(item);
		}
		FrameworkElement ILayoutControl.FindControl(string id) {
			return FindControl(id);
		}
		string ILayoutControl.GetID(FrameworkElement control) {
			return GetID(control);
		}
		void ILayoutControl.SetID(FrameworkElement control, string id) {
			SetID(control, id);
		}
		Style ILayoutControl.GetPartStyle(LayoutGroupPartStyle style) {
			return GetPartStyle(style);
		}
		void ILayoutControl.InitCustomizationController() {
			if (Controller.IsCustomization)
				InitCustomizationController(Controller.CustomizationController);
		}
		void ILayoutControl.InitNewElement(FrameworkElement element) {
			OnInitNewElement(element);
		}
		bool ILayoutControl.MakeControlVisible(FrameworkElement control) {
			if (!control.GetVisible())
				return false;
			LayoutItem layoutItem = control.GetLayoutItem();
			if (layoutItem != null)
				control = layoutItem;
			do {
				var parent = control.Parent as ILayoutGroup;
				if (parent == null)
					break;
				if (parent.MakeChildVisible(control))
					if (parent.IsRoot)
						return true;
					else
						control = parent.Control;
				else
					break;
			} while (true);
			return false;
		}
		void ILayoutControl.ModelChanged(LayoutControlModelChangedEventArgs args) {
			Controller.OnModelChanged(args);
		}
		void ILayoutControl.TabClicked(ILayoutGroup group, FrameworkElement selectedTabChild) {
			Controller.OnTabClicked(group, selectedTabChild);
		}
		void ILayoutControl.ReadElementFromXML(XmlReader xml, FrameworkElement element) {
			OnReadElementFromXML(xml, element);
		}
		void ILayoutControl.WriteElementToXML(XmlWriter xml, FrameworkElement element) {
			OnWriteElementToXML(xml, element);
		}
		FrameworkElements ILayoutControl.VisibleAvailableItems { get { return VisibleAvailableItems; } }
		#endregion ILayoutControl
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.LayoutControlAutomationPeer(this);
		}
		#endregion
		#region Serialization
		ISerializationController serializationControllerCore;
		protected virtual ISerializationController CreateSerializationController() {
			return new SerializationController(this);
		}
		protected internal ISerializationController SerializationController {
			get { return serializationControllerCore; }
		}
		void SaveLayoutToStream(Stream stream) {
			SerializationController.SaveLayout(stream);
		}
		void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(stream);
		}
		void SaveLayoutToXml(string path) {
			SerializationController.SaveLayout(path);
		}
		void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(path);
		}
		void RestoreLayoutCore(object path) {
			SerializationController.RestoreLayout(path);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false)]
		public SerializableItemCollection Items {
			get { return SerializationController.Items; }
			set { SerializationController.Items = value; }
		}
		#endregion Serialization
	}
	public abstract class LayoutControlModelChangedEventArgs : EventArgs {
		public string ChangeDescription { get; protected set; }
	}
	public class LayoutControlModelStructureChangedEventArgs : LayoutControlModelChangedEventArgs {
		public LayoutControlModelStructureChangedEventArgs(string changeDescription, FrameworkElement element) {
			ChangeDescription = changeDescription;
			Element = element;
		}
		public FrameworkElement Element { get; private set; }
	}
	public class LayoutControlModelPropertyChangedEventArgs : LayoutControlModelChangedEventArgs {
		public LayoutControlModelPropertyChangedEventArgs(DependencyObject obj, string propertyName, DependencyProperty property) {
			Object = obj;
			PropertyName = propertyName;
			Property = property;
			ChangeDescription = "Change " + PropertyName;
		}
		public DependencyObject Object { get; private set; }
		public DependencyProperty Property { get; private set; }
		public string PropertyName { get; private set; }
	}
	public class LayoutControlController : LayoutGroupController {
		public LayoutControlController(ILayoutControl control) : base(control) { }
		public override IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in base.GetInternalElements())
				yield return element;
			if (CustomizationController != null)
				foreach (UIElement element in CustomizationController.GetInternalElements())
					yield return element;
		}
		public override FrameworkElement GetItem(Point p, bool ignoreLayoutGroups, bool ignoreLocking) {
			if (!ignoreLocking && ILayoutGroup.IsLocked)
				return null;
			else
				return base.GetItem(p, ignoreLayoutGroups, ignoreLocking);
		}
		public override void OnMeasure(Size availableSize) {
			base.OnMeasure(availableSize);
			if (IsCustomization)
				CustomizationController.OnMeasure(availableSize);
		}
		public override void OnArrange(Size finalSize) {
			base.OnArrange(finalSize);
			if (IsCustomization)
				CustomizationController.OnArrange(finalSize);
		}
		public new ILayoutControl ILayoutControl { get { return base.ILayoutControl as ILayoutControl; } }
		protected internal virtual void OnControlVisibilityChanged(FrameworkElement control) {
			if (IsCustomization)
				CustomizationController.OnControlVisibilityChanged(control);
		}
		protected override void OnLayoutUpdated() {
			base.OnLayoutUpdated();
			if (IsCustomization && Control.IsInVisualTree())
				CustomizationController.OnLayoutUpdated();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if (IsCustomization)
				CustomizationController.OnLoaded();
		}
		protected internal virtual void OnTabClicked(ILayoutGroup group, FrameworkElement selectedTabChild) {
			if (IsCustomization)
				CustomizationController.OnTabClicked(group, selectedTabChild);
		}
		protected override void OnUnloaded() {
			base.OnUnloaded();
			IsCustomization = false;
		}
		protected override bool NeedsUnloadedEvent { get { return IsCustomization; } }
		#region Keyboard and Mouse Handling
		protected override void OnKeyDown(DXKeyEventArgs e) {
			base.OnKeyDown(e);
			if (!Control.IsInDesignTool() && e.Key == Key.F2 && Keyboard2.IsControlPressed && Keyboard2.IsShiftPressed) {
				IsCustomization = !IsCustomization;
				e.Handled = true;
				return;
			}
			if (IsCustomization)
				CustomizationController.OnKeyDown(e);
		}
		protected override void OnMouseMove(DXMouseEventArgs e) {
			base.OnMouseMove(e);
			if (IsCustomization)
				CustomizationController.OnMouseMove(e);
		}
		protected override void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (IsCustomization)
				CustomizationController.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			bool wasMouseLeftButtonDown = IsMouseLeftButtonDown;
			base.OnMouseLeftButtonUp(e);
			if (e != null && IsCustomization && wasMouseLeftButtonDown)
				CustomizationController.OnMouseLeftButtonUp(e);
		}
		protected virtual void OnMouseRightButtonDown(DXMouseButtonEventArgs e) {
			if (IsCustomization)
				CustomizationController.OnMouseRightButtonDown(e);
		}
		#endregion Keyboard and Mouse Handling
		#region Scrolling
		public override bool IsScrollable() {
			return true;
		}
		#endregion Scrolling
		#region Customization
		private bool _IsCustomization;
		public LayoutControlCustomizationController CustomizationController { get; private set; }
		public bool IsCustomization {
			get { return _IsCustomization; }
			set {
				if (IsCustomization == value)
					return;
				_IsCustomization = value;
				if (IsCustomization) {
					CustomizationController = CreateCustomizationController();
					CustomizationController.BeginCustomization();
					ILayoutControl.InitCustomizationController();
				}
				ILayoutControl.IsCustomization = IsCustomization;
				OnIsCustomizationChanged();
				if (!IsCustomization) {
					CustomizationController.EndCustomization();
					CustomizationController = null;
				}
			}
		}
		public event EventHandler IsCustomizationChanged;
		protected virtual LayoutControlCustomizationController CreateCustomizationController() {
			return new LayoutControlCustomizationController(this);
		}
		protected virtual void OnIsCustomizationChanged() {
			if (IsCustomizationChanged != null)
				IsCustomizationChanged(Control, EventArgs.Empty);
		}
		#endregion Customization
		#region Drag&Drop
		public virtual void DropElement(FrameworkElement element, LayoutItemInsertionPoint insertionPoint, LayoutItemInsertionKind insertionKind) {
			var group = (ILayoutGroup)(insertionPoint.IsInternalInsertion ? insertionPoint.Element : insertionPoint.Element.Parent);
			group.InsertElement(element, insertionPoint, insertionKind);
			ILayoutGroup.OptimizeLayout(true);
			Control.InvalidateMeasure();
			if (!element.IsInVisualTree())
				element = null;
			if (IsCustomization)
				CustomizationController.OnDropElement(element);
			OnModelChanged(new LayoutControlModelStructureChangedEventArgs("Drag & Drop", element));
		}
		public LayoutItemInsertionInfo GetInsertionInfo(FrameworkElement parentedElement, Point p) {
			FrameworkElement destinationItem = GetItem(p, false, Control.IsInDesignTool());
			if (destinationItem == null)
				return new LayoutItemInsertionInfo();
			var destinationGroup = destinationItem as ILayoutGroup;
			var parentGroup = destinationItem.Parent as ILayoutGroup;
			p = Control.MapPoint(p, destinationItem);
			LayoutItemInsertionKind insertionKind;
			if (destinationItem == Control)
				insertionKind = LayoutItemInsertionKind.None;
			else
				insertionKind = parentGroup.GetInsertionKind(destinationItem, p);
			if (destinationItem == Control || destinationItem.IsLayoutGroup() && insertionKind == LayoutItemInsertionKind.None)
				return destinationGroup.GetInsertionInfoForEmptyArea(parentedElement, p);
			else {
				LayoutItemInsertionPoint insertionPoint = parentGroup.GetInsertionPoint(parentedElement, destinationItem, insertionKind, p);
				return new LayoutItemInsertionInfo(destinationItem, insertionKind, insertionPoint);
			}
		}
		public void StartItemDragAndDrop(FrameworkElement item, MouseEventArgs mouseEventArgs, FrameworkElement source) {
			DragAndDropController controller;
			if (WantsItemDragAndDrop(new Point(0, 0), p => item, out controller)) {
				Point startDragPoint = mouseEventArgs.GetPosition(source);
				((LayoutItemDragAndDropController)controller).StartDragRelativePoint =
					new Point(startDragPoint.X / source.ActualWidth, startDragPoint.Y / source.ActualHeight);
				DragAndDropController = controller;
				StartDragAndDrop(mouseEventArgs.GetPosition(Control));
			}
		}
		public Style ItemInsertionPointIndicatorStyle { get; set; }
		protected override bool CanItemDragAndDrop() {
			return IsCustomization && ILayoutControl.AllowItemMoving;
		}
		protected override void StartDragAndDrop(Point p) {
			if (IsCustomization)
				CustomizationController.OnStartDragAndDrop();
			base.StartDragAndDrop(p);
		}
		protected override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			if (IsCustomization)
				CustomizationController.OnEndDragAndDrop(accept);
		}
		protected override DragAndDropController CreateItemDragAndDropControler(Point startDragPoint, FrameworkElement dragControl) {
			return new LayoutItemDragAndDropController(this, startDragPoint, dragControl);
		}
		#endregion
		#region Design-time Support
		public void DesignTimeKeyDown(DXKeyEventArgs e) {
			if (ForwardDesignTimeInput)
				ProcessKeyDown(e);
		}
		public void DesignTimeKeyUp(DXKeyEventArgs e) {
			if (ForwardDesignTimeInput)
				ProcessKeyUp(e);
		}
		public void DesignTimeMouseCaptureCancelled() {
			if (ForwardDesignTimeInput)
				OnMouseCaptureCancelled();
		}
		public void DesignTimeMouseEnter(DXMouseEventArgs e) {
			if (ForwardDesignTimeInput)
				OnMouseEnter(e);
		}
		public void DesignTimeMouseLeave(DXMouseEventArgs e) {
			if (ForwardDesignTimeInput)
				OnMouseLeave(e);
		}
		public void DesignTimeMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			if (!ForwardDesignTimeInput)
				CustomizationController.ProcessSelection(Control, true);
			FrameworkElement element = CustomizationController.GetSelectableItem(e.GetPosition(Control));
			if (element.IsLayoutGroup() && ((ILayoutGroup)element).DesignTimeClick(e)) {
				e.Handled = true;
				return;
			}
			if (ForwardDesignTimeInput)
				OnMouseLeftButtonDown(e);
		}
		public void DesignTimeMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			if (ForwardDesignTimeInput)
				OnMouseLeftButtonUp(e);
		}
		public void DesignTimeMouseMove(DXMouseEventArgs e) {
			if (ForwardDesignTimeInput)
				OnMouseMove(e);
		}
		public void DesignTimeMouseRightButtonDown(DXMouseButtonEventArgs e) {
			if (ForwardDesignTimeInput)
				OnMouseRightButtonDown(e);
		}
		public event EventHandler<LayoutControlModelChangedEventArgs> ModelChanged;
		protected internal virtual void OnModelChanged(LayoutControlModelChangedEventArgs args) {
			if (Control.IsInDesignTool() && IsCustomization) {
				var propertyChangedArgs = args as LayoutControlModelPropertyChangedEventArgs;
				if (propertyChangedArgs != null && propertyChangedArgs.Property == LayoutGroup.IsCollapsedProperty &&
					((LayoutGroup)propertyChangedArgs.Object).IsCollapsed)
					CustomizationController.OnGroupCollapsed((ILayoutGroup)propertyChangedArgs.Object);
			}
			if (ModelChanged != null)
				ModelChanged(Control, args);
		}
		protected virtual bool ForwardDesignTimeInput { get { return true; } }
		#endregion Design-time Support
	}
	public class LayoutItemDragAndDropController : LayoutItemDragAndDropControllerBase {
		private ILayoutGroup _DestinationGroup;
		private int _DestinationGroupTabIndex = -1;
		private LayoutItemInsertionInfo _InsertionInfo;
		public LayoutItemDragAndDropController(Controller controller, Point startDragPoint, FrameworkElement dragControl)
			: base(controller, startDragPoint, dragControl) {
		}
		public override void StartDragAndDrop(Point p) {
			InitializeIndicator();
			base.StartDragAndDrop(p);
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			InsertionInfo = Controller.GetInsertionInfo(DragControlPlaceHolder, p);
			if (DestinationGroup != null)
				DestinationGroupTabIndex = DestinationGroup.GetTabIndex(Controller.Control.MapPoint(p, null));
		}
		public override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			FinalizeIndicator();
			DestinationGroup = null;
			if (accept && InsertionInfo.InsertionPoint != null)
				Controller.DropElement(DragControl, InsertionInfo.InsertionPoint, InsertionInfo.InsertionKind);
		}
		public override void OnArrange(Size finalSize) {
			base.OnArrange(finalSize);
			InsertionInfo = new LayoutItemInsertionInfo();
		}
		protected virtual void OnDestinationGroupChanged() {
			DestinationGroupTabIndex = -1;
			CheckDelayTimer(ref _GroupExpandingDelayTimer, GroupExpandingDelay, IsGroupExpandingDelayNeeded, OnGroupExpandingDelayExpired);
		}
		protected virtual void OnDestinationGroupTabIndexChanged() {
			CheckDelayTimer(ref _TabSelectionDelayTimer, TabSelectionDelay, IsTabSelectionDelayNeeded, OnTabSelectionDelayExpired);
		}
		protected virtual void OnInsertionInfoChanged() {
			Indicator.InsertionInfo = InsertionInfo;
			Indicator.SetVisible(InsertionInfo.DestinationItem != null);
			DestinationGroup = InsertionInfo.DestinationItem.IsLayoutGroup() ? (ILayoutGroup)InsertionInfo.DestinationItem : null;
		}
		protected new LayoutControlController Controller { get { return (LayoutControlController)base.Controller; } }
		protected ILayoutGroup DestinationGroup {
			get { return _DestinationGroup; }
			set {
				if (DestinationGroup == value)
					return;
				_DestinationGroup = value;
				OnDestinationGroupChanged();
			}
		}
		protected int DestinationGroupTabIndex {
			get { return _DestinationGroupTabIndex; }
			set {
				if (DestinationGroupTabIndex == value)
					return;
				_DestinationGroupTabIndex = value;
				OnDestinationGroupTabIndexChanged();
			}
		}
		protected LayoutItemInsertionInfo InsertionInfo {
			get { return _InsertionInfo; }
			set {
				if (InsertionInfo.Equals(value))
					return;
				_InsertionInfo = value;
				OnInsertionInfoChanged();
			}
		}
		#region Indicator
		protected virtual LayoutItemDragAndDropIndicator CreateIndicator() {
			return new LayoutItemDragAndDropIndicator(DragControlPlaceHolder);
		}
		protected void InitializeIndicator() {
			Indicator = CreateIndicator();
			Indicator.InsertionPointIndicatorStyle = Controller.ItemInsertionPointIndicatorStyle;
			Indicator.Visibility = Visibility.Collapsed;
			Controller.CustomizationController.CustomizationCanvas.Children.Add(Indicator);
		}
		protected void FinalizeIndicator() {
			Controller.CustomizationController.CustomizationCanvas.Children.Remove(Indicator);
			Indicator = null;
		}
		protected LayoutItemDragAndDropIndicator Indicator { get; private set; }
		#endregion
		#region Group Expanding Delay
		public static int GroupExpandingDelay = 1000;   
		private Storyboard _GroupExpandingDelayTimer;
		protected virtual bool IsGroupExpandingDelayNeeded() {
			return DestinationGroup != null && DestinationGroup.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible;
		}
		protected virtual void OnGroupExpandingDelayExpired() {
			DestinationGroup.IsCollapsed = false;
		}
		#endregion Group Expanding Delay
		#region Tab Selection Delay
		public static int TabSelectionDelay = 750;   
		private Storyboard _TabSelectionDelayTimer;
		protected virtual bool IsTabSelectionDelayNeeded() {
			return DestinationGroup != null && DestinationGroupTabIndex != -1 && DestinationGroup.SelectedTabIndex != DestinationGroupTabIndex;
		}
		protected virtual void OnTabSelectionDelayExpired() {
			DestinationGroup.SelectedTabIndex = DestinationGroupTabIndex;
		}
		#endregion Tab Selection Delay
	}
	public class LayoutItemDragAndDropIndicator : PanelBase {
		private LayoutItemInsertionInfo _InsertionInfo;
		public LayoutItemDragAndDropIndicator(FrameworkElement parentedDragControl) {
			ParentedDragControl = parentedDragControl;
			ShowingStoryboard = CreateShowingStoryboard();
		}
		public LayoutItemInsertionInfo InsertionInfo {
			get { return _InsertionInfo; }
			set {
				if (InsertionInfo.Equals(value))
					return;
				LayoutItemInsertionInfo oldValue = InsertionInfo;
				_InsertionInfo = value;
				OnInsertionInfoChanged(oldValue);
			}
		}
		public Style InsertionPointIndicatorStyle { get; set; }
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			foreach(UIElement child in Children)
				child.Measure(SizeHelper.Infinite);
			return base.OnMeasure(availableSize);
		}
		protected override Size OnArrange(Rect bounds) {
			for (int i = 0; i < Children.Count; i++) {
				var insertionPointIndicator = (LayoutItemDragAndDropInsertionPointIndicator)Children[i];
				Rect insertionPointIndicatorBounds = GetInsertionPointIndicatorBounds(
					DestinationItemGroup.GetInsertionPointZoneBounds(InsertionInfo.DestinationItem, InsertionInfo.InsertionKind, i, Children.Count),
					GetInsertionPointBounds(insertionPointIndicator.InsertionPoint));
				insertionPointIndicator.Arrange(insertionPointIndicatorBounds);
			}
			return base.OnArrange(bounds);
		}
		protected Rect GetInsertionPointBounds(LayoutItemInsertionPoint insertionPoint) {
			if (insertionPoint.Element.IsLayoutGroup() || insertionPoint.Element == InsertionInfo.DestinationItem.GetLayoutControl())
				return ((ILayoutGroup)insertionPoint.Element).GetInsertionPointBounds(insertionPoint.IsInternalInsertion, InsertionInfo.DestinationItem);
			else
				return insertionPoint.Element.GetBounds(InsertionInfo.DestinationItem);
		}
		protected virtual Rect GetInsertionPointIndicatorBounds(Rect insertionPointZoneBounds, Rect insertionPointBounds) {
			Rect result = insertionPointZoneBounds;
			Side? insertionSide = InsertionInfo.InsertionKind.GetSide();
			if (insertionSide != null)
				if (insertionSide.Value.GetOrientation() == Orientation.Horizontal) {
					result.X = insertionPointBounds.X;
					result.Width = insertionPointBounds.Width;
				}
				else {
					result.Y = insertionPointBounds.Y;
					result.Height = insertionPointBounds.Height;
				}
			return result;
		}
		#endregion Layout
		protected LayoutItemInsertionPoints GetInsertionPoints() {
			var result = new LayoutItemInsertionPoints();
			if (DestinationItemGroup != null)
				DestinationItemGroup.GetInsertionPoints(ParentedDragControl, InsertionInfo.DestinationItem, InsertionInfo.DestinationItem,
					InsertionInfo.InsertionKind, result);
			return result;
		}
		protected void ActivateInsertionPointIndicator(LayoutItemInsertionPoint oldInsertionPoint, LayoutItemInsertionPoint newInsertionPoint) {
			LayoutItemDragAndDropInsertionPointIndicator insertionPointIndicator = GetInsertionPointIndicator(oldInsertionPoint);
			if (insertionPointIndicator != null)
				insertionPointIndicator.IsActive = false;
			insertionPointIndicator = GetInsertionPointIndicator(newInsertionPoint);
			if (insertionPointIndicator != null)
				insertionPointIndicator.IsActive = true;
		}
		protected virtual LayoutItemDragAndDropInsertionPointIndicator CreateInsertionPointIndicator(LayoutItemInsertionPoint insertionPoint) {
			return new LayoutItemDragAndDropInsertionPointIndicator(insertionPoint, InsertionInfo.InsertionKind);
		}
		protected void CreateInsertionPointIndicators() {
			Children.Clear();
			foreach (var insertionPoint in GetInsertionPoints()) {
				LayoutItemDragAndDropInsertionPointIndicator indicator = CreateInsertionPointIndicator(insertionPoint);
				indicator.Style = InsertionPointIndicatorStyle;
				Children.Add(indicator);
			}
		}
		protected LayoutItemDragAndDropInsertionPointIndicator GetInsertionPointIndicator(LayoutItemInsertionPoint insertionPoint) {
			foreach (LayoutItemDragAndDropInsertionPointIndicator child in Children)
				if (child.InsertionPoint.Equals(insertionPoint))
					return child;
			return null;
		}
		protected void UpdateInsertionPointIndicators(LayoutItemInsertionPoint oldInsertionPoint) {
			if(ShowingStoryboard != null)
				ShowingStoryboard.Stop();
			ActivateInsertionPointIndicator(oldInsertionPoint, null);
			CreateInsertionPointIndicators();
			if(ShowingStoryboard != null)
				ShowingStoryboard.Begin();
		}
		protected virtual void OnInsertionInfoChanged(LayoutItemInsertionInfo oldInsertionInfo) {
			if (InsertionInfo.DestinationItem != oldInsertionInfo.DestinationItem && InsertionInfo.DestinationItem != null)
				this.SetBounds(InsertionInfo.DestinationItem.GetBounds((FrameworkElement)Parent));
			if (InsertionInfo.DestinationItem != oldInsertionInfo.DestinationItem || InsertionInfo.InsertionKind != oldInsertionInfo.InsertionKind) {
				UpdateInsertionPointIndicators(oldInsertionInfo.InsertionPoint);
				oldInsertionInfo.InsertionPoint = null;
			}
			if (InsertionInfo.InsertionPoint != oldInsertionInfo.InsertionPoint)
				ActivateInsertionPointIndicator(oldInsertionInfo.InsertionPoint, InsertionInfo.InsertionPoint);
		}
		protected virtual Storyboard CreateShowingStoryboard() {
			var animation = new DoubleAnimation();
			Storyboard.SetTarget(animation, this);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
			animation.From = 0;
			animation.To = 1;
			animation.Duration = TimeSpan.FromMilliseconds(250);
			var result = new Storyboard();
			result.Children.Add(animation);
			return result;
		}
		protected ILayoutGroup DestinationItemGroup {
			get { return InsertionInfo.DestinationItem != null ? InsertionInfo.DestinationItem.Parent as ILayoutGroup : null; }
		}
		protected FrameworkElement ParentedDragControl { get; private set; }
		protected Storyboard ShowingStoryboard { get; private set; }
	}
	public class LayoutItemDragAndDropInsertionPointIndicator : ControlBase {
		private bool _IsActive;
		public LayoutItemDragAndDropInsertionPointIndicator() {
			DefaultStyleKey = typeof(LayoutItemDragAndDropInsertionPointIndicator);
		}
		public LayoutItemDragAndDropInsertionPointIndicator(LayoutItemInsertionPoint insertionPoint, LayoutItemInsertionKind insertionKind) {
			DefaultStyleKey = typeof(LayoutItemDragAndDropInsertionPointIndicator);
			InsertionPoint = insertionPoint;
			InsertionKind = insertionKind;
		}
		public LayoutItemInsertionKind InsertionKind { get; private set; }
		public LayoutItemInsertionPoint InsertionPoint { get; private set; }
		public bool IsActive {
			get { return _IsActive; }
			set {
				if(_IsActive == value)
					return;
				_IsActive = value;
				OnIsActiveChanged();
			}
		}
		protected virtual void OnIsActiveChanged() {
			UpdateState(true);
		}
		protected override void UpdateState(bool useTransitions) {
			GoToState(IsActive ? "MouseOver" : "Normal", useTransitions);
		}
	}
}
