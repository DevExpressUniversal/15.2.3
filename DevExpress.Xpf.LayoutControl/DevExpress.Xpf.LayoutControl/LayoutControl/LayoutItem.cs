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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.LayoutControl.Serialization;
#if !SILVERLIGHT
using System.Collections;
#endif
namespace DevExpress.Xpf.LayoutControl {
	public enum LayoutItemLabelPosition { Left, Top }
	[ContentProperty("Content")]
	[StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(LayoutItemLabel))]
	[TemplatePart(Name = LabelElementName, Type = typeof(LayoutItemLabel))]
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
#if !SILVERLIGHT
#endif
	public class LayoutItem : ControlBase, ILayoutControlCustomizableItem, ISerializableItem {
		#region Dependency Properties
		public static readonly DependencyProperty AddColonToLabelProperty =
			DependencyProperty.Register("AddColonToLabel", typeof(bool), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnAddColonToLabelChanged()));
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(UIElement), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnContentChanged((UIElement)e.OldValue, (UIElement)e.NewValue)));
		public static readonly DependencyProperty ElementSpaceProperty =
			DependencyProperty.Register("ElementSpace", typeof(double), typeof(LayoutItem), null);
		public static readonly DependencyProperty IsRequiredProperty =
			DependencyProperty.Register("IsRequired", typeof(bool), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnIsRequiredChanged()));
		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(object), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnLabelChanged(e.OldValue)));
		public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
			DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(LayoutItem),
				new PropertyMetadata(HorizontalAlignment.Left));
		public static readonly DependencyProperty LabelVerticalAlignmentProperty =
			DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(LayoutItem),
				new PropertyMetadata(VerticalAlignment.Center));
		public static readonly DependencyProperty LabelPositionProperty =
			DependencyProperty.Register("LabelPosition", typeof(LayoutItemLabelPosition), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnLabelPositionChanged((LayoutItemLabelPosition)e.OldValue)));
		public static readonly DependencyProperty LabelStyleProperty =
			DependencyProperty.Register("LabelStyle", typeof(Style), typeof(LayoutItem),
				new PropertyMetadata((o, e) => ((LayoutItem)o).OnLabelStyleChanged()));
		public static readonly DependencyProperty LabelTemplateProperty =
			DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(LayoutItem), null);
		public static readonly DependencyProperty ActualLabelProperty =
			DependencyProperty.Register("ActualLabel", typeof(object), typeof(LayoutItem), null);
		public static readonly DependencyProperty CalculatedLabelVisibilityProperty =
			DependencyProperty.Register("CalculatedLabelVisibility", typeof(Visibility), typeof(LayoutItem), null);
		public static readonly DependencyProperty IsActuallyRequiredProperty =
			DependencyProperty.Register("IsActuallyRequired", typeof(bool), typeof(LayoutItem), null);
		#endregion Dependency Properties
		static LayoutItem() {
			DXSerializer.SerializationProviderProperty.OverrideMetadata(typeof(LayoutItem), new UIPropertyMetadata(new LayoutControlSerializationProviderBase()));
		}
		public LayoutItem() {
			DefaultStyleKey = typeof(LayoutItem);
			UpdateActualLabel();
			CalculateLabelVisibility();
			UpdateIsActuallyRequired();
			SerializableItem.SetTypeName(this, this.GetType().Name);
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemActualLabel")]
#endif
		public object ActualLabel {
			get { return (object)GetValue(ActualLabelProperty); }
			private set { SetValue(ActualLabelProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemAddColonToLabel")]
#endif
		public bool AddColonToLabel {
			get { return (bool)GetValue(AddColonToLabelProperty); }
			set { SetValue(AddColonToLabelProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemContent")]
#endif
		public UIElement Content {
			get { return (UIElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemElementSpace")]
#endif
		public double ElementSpace {
			get { return (double)GetValue(ElementSpaceProperty); }
			set { SetValue(ElementSpaceProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemIsActuallyRequired")]
#endif
		public bool IsActuallyRequired {
			get { return (bool)GetValue(IsActuallyRequiredProperty); }
			private set { SetValue(IsActuallyRequiredProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemIsLabelVisible")]
#endif
		public bool IsLabelVisible { get { return GetIsLabelVisible(Label); } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemIsRequired")]
#endif
		public bool IsRequired {
			get { return (bool)GetValue(IsRequiredProperty); }
			set { SetValue(IsRequiredProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabel")]
#endif
		[XtraSerializableProperty]
		public object Label {
			get { return GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabelHorizontalAlignment")]
#endif
		public HorizontalAlignment LabelHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
			set { SetValue(LabelHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabelVerticalAlignment")]
#endif
		public VerticalAlignment LabelVerticalAlignment {
			get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
			set { SetValue(LabelVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabelPosition")]
#endif
		public LayoutItemLabelPosition LabelPosition {
			get { return (LayoutItemLabelPosition)GetValue(LabelPositionProperty); }
			set { SetValue(LabelPositionProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabelStyle")]
#endif
		public Style LabelStyle {
			get { return (Style)GetValue(LabelStyleProperty); }
			set { SetValue(LabelStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutItemLabelTemplate")]
#endif
		public DataTemplate LabelTemplate {
			get { return (DataTemplate)GetValue(LabelTemplateProperty); }
			set { SetValue(LabelTemplateProperty, value); }
		}
		#region Template
		const string LabelElementName = "LabelElement";
		public override void OnApplyTemplate() {
			if (LabelElement != null) {
				LabelElement.DesiredWidthChanged = null;
				LabelElement.ClearValue(LayoutItemLabel.StyleProperty);
			}
			base.OnApplyTemplate();
			LabelElement = GetTemplateChild(LabelElementName) as LayoutItemLabel;
			UpdateLabelElementStyle();
			if (LabelElement != null)
				LabelElement.DesiredWidthChanged = OnLabelWidthChanged;
		}
		protected LayoutItemLabel LabelElement { get; private set; }
		private void UpdateLabelElementStyle() {
			if (LabelElement == null)
				return;
			if (LabelStyle != null)
				LabelElement.Style = LabelStyle;
			else
				LabelElement.ClearValue(LayoutItemLabel.StyleProperty);
		}
		#endregion Template
		#region Layout
		protected override Size MeasureOverride(Size availableSize) {
			AvailableSize = availableSize;
			return base.MeasureOverride(availableSize);
		}
		protected Size AvailableSize { get; private set; }
		#endregion Layout
		#region XML Storage
		protected internal virtual void ReadCustomizablePropertiesFromXML(XmlReader xml) {
			this.ReadPropertyFromXML(xml, LabelProperty, "Label", typeof(object));
		}
		protected internal virtual void WriteCustomizablePropertiesToXML(XmlWriter xml) {
			this.WritePropertyToXML(xml, LabelProperty, "Label");
		}
		#endregion XML Storage
		protected void CalculateLabelVisibility() {
			SetValue(CalculatedLabelVisibilityProperty, IsLabelVisible ? Visibility.Visible : Visibility.Collapsed);
		}
		protected virtual object GetActualLabel() {
			var result = Label as string;
			if (result == null)
				return Label;
			if (!string.IsNullOrEmpty(result) && AddColonToLabel)
				result += ":";
			return result;
		}
		protected virtual bool GetIsActuallyRequired() {
			return IsRequired;
		}
		protected virtual bool GetIsLabelVisible(object label) {
			return label != null && (!(label is string) || !string.IsNullOrEmpty((string)label));
		}
		protected internal virtual FrameworkElement GetSelectableElement(Point p) {
			if (Content != null && Content.GetVisible() && ((FrameworkElement)Content).GetBounds(this).Contains(p))
				return (FrameworkElement)Content;
			else
				return this;
		}
		protected virtual void OnContentChanged(UIElement oldValue, UIElement newValue) {
#if !SILVERLIGHT
			RemoveLogicalChild(oldValue);
			AddLogicalChild(newValue);
#endif
		}
		protected virtual void OnAddColonToLabelChanged() {
			UpdateActualLabel();
		}
		protected virtual void OnIsRequiredChanged() {
			UpdateIsActuallyRequired();
		}
		protected virtual void OnLabelChanged(object oldValue) {
#if !SILVERLIGHT
			RemoveLogicalChild(oldValue);
			AddLogicalChild(Label);
#endif
			UpdateActualLabel();
			CalculateLabelVisibility();
		}
		protected virtual void OnLabelPositionChanged(LayoutItemLabelPosition oldValue) {
			LabelWidth = double.NaN;
		}
		protected virtual void OnLabelStyleChanged() {
			UpdateLabelElementStyle();
		}
		protected virtual void OnLabelWidthChanged() {
			var parent = Parent as ILayoutGroup;
			if (parent != null)
				((ILayoutGroup)parent).LayoutItemLabelWidthChanged(this);
		}
		protected void UpdateActualLabel() {
			ActualLabel = GetActualLabel();
		}
		protected void UpdateIsActuallyRequired() {
			IsActuallyRequired = GetIsActuallyRequired();
		}
		protected internal double LabelWidth {
			get { return LabelElement != null ? LabelElement.DesiredWidth : 0; }
			set {
				if (LabelElement == null)
					return;
				double maxValue = double.PositiveInfinity;
				double actualWidth = Margin.Left + ActualWidth + Margin.Right;
				if (double.IsInfinity(AvailableSize.Width) && actualWidth > DesiredSize.Width)
					maxValue = actualWidth - (DesiredSize.Width - LabelElement.DesiredSize.Width);
				LabelElement.CustomWidth = value <= maxValue ? value : double.NaN;
			}
		}
#if !SILVERLIGHT
		protected override IEnumerator LogicalChildren {
			get {
				var logicalChildren = new List<object>();
				if (Content != null)
					logicalChildren.Add(Content);
				if (Label != null)
					logicalChildren.Add(Label);
				return logicalChildren.GetEnumerator();
			}
		}
#endif
		#region ILayoutControlCustomizableItem
		FrameworkElement ILayoutControlCustomizableItem.AddNewItem() {
			return null;
		}
		bool ILayoutControlCustomizableItem.CanAddNewItems { get { return false; } }
		bool ILayoutControlCustomizableItem.HasHeader { get { return true; } }
		object ILayoutControlCustomizableItem.Header {
			get { return Label; }
			set { Label = value; }
		}
		bool ILayoutControlCustomizableItem.IsLocked { get { return false; } }
		#endregion ILayoutControlCustomizableItem
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.LayoutItemAutomationPeer(this);
		}
		#endregion
		#region ISerializableItem Members
		FrameworkElement ISerializableItem.Element {
			get { return this; }
		}
		#endregion
		#region ISerializableCollectionItem Members
		string ISerializableCollectionItem.Name {
			get { return Name; }
			set { Name = value; }
		}
		string ISerializableCollectionItem.TypeName {
			get { return SerializableItem.GetTypeName(this); }
		}
		string ISerializableCollectionItem.ParentName {
			get { return SerializableItem.GetParentName(this); }
			set { SerializableItem.SetParentName(this, value); }
		}
		string ISerializableCollectionItem.ParentCollectionName {
			get { return SerializableItem.GetParentCollectionName(this); }
			set { SerializableItem.SetParentCollectionName(this, value); }
		}
		#endregion
	}
	public class LayoutItems : List<LayoutItem> { }
	public class LayoutItemPanel : Panel {
		#region Dependency Properties
		public static readonly DependencyProperty ElementSpaceProperty =
			DependencyProperty.Register("ElementSpace", typeof(double), typeof(LayoutItemPanel),
				new PropertyMetadata((o, e) => ((LayoutItemPanel)o).OnElementSpaceChanged()));
		public static readonly DependencyProperty LabelPositionProperty =
			DependencyProperty.Register("LabelPosition", typeof(LayoutItemLabelPosition), typeof(LayoutItemPanel),
				new PropertyMetadata((o, e) => ((LayoutItemPanel)o).OnLabelPositionChanged()));
		#endregion Dependency Properties
		private UIElement _Content;
		private UIElement _Label;
		public UIElement Content {
			get { return _Content; }
			set {
				if (Content == value)
					return;
				if (Content != null)
					Children.Remove(Content);
				_Content = value;
				if (Content != null)
					Children.Add(Content);
			}
		}
		public double ElementSpace {
			get { return (double)GetValue(ElementSpaceProperty); }
			set { SetValue(ElementSpaceProperty, value); }
		}
		public UIElement Label {
			get { return _Label; }
			set {
				if (Label == value)
					return;
				if (Label != null)
					Children.Remove(Label);
				_Label = value;
				if (Label != null)
					Children.Add(Label);
			}
		}
		public LayoutItemLabelPosition LabelPosition {
			get { return (LayoutItemLabelPosition)GetValue(LabelPositionProperty); }
			set { SetValue(LabelPositionProperty, value); }
		}
		#region Layout
		protected override Size MeasureOverride(Size availableSize) {
			if (Label == null || Content == null)
				return new Size(0, 0);
			Label.Measure(GetLabelAvailableSize(availableSize));
			Content.Measure(GetContentAvailableSize(availableSize));
			return GetDesiredSize();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (Label == null || Content == null)
				return base.ArrangeOverride(finalSize);
			Rect bounds = RectHelper.New(finalSize);
			Label.Arrange(GetLabelBounds(bounds));
			Content.Arrange(GetContentBounds(bounds));
			return base.ArrangeOverride(finalSize);
		}
		protected virtual Size GetContentAvailableSize(Size availableSize) {
			Size result = availableSize;
			SetLength(ref result, Math.Max(0, GetLength(result) - (GetLength(Label.DesiredSize) + ActualElementSpace)));
			return result;
		}
		protected virtual Size GetLabelAvailableSize(Size availableSize) {
			Size result = availableSize;
			SetLength(ref result, double.PositiveInfinity);
			return result;
		}
		protected virtual Size GetDesiredSize() {
			var result = new Size();
			SetLength(ref result, GetLength(Label.DesiredSize) + ActualElementSpace + GetLength(Content.DesiredSize));
			SetWidth(ref result, Math.Max(GetWidth(Label.DesiredSize), GetWidth(Content.DesiredSize)));
			return result;
		}
		protected virtual Rect GetContentBounds(Rect bounds) {
			Rect result = bounds;
			double offset = GetLength(Label.DesiredSize) + ActualElementSpace;
			SetOffset(ref result, offset);
			SetLength(ref result, Math.Max(0, GetLength(result) - offset));
			return result;
		}
		protected virtual Rect GetLabelBounds(Rect bounds) {
			Rect result = bounds;
			SetLength(ref result, GetLength(Label.DesiredSize));
			return result;
		}
		protected double GetLength(Size size) {
			return Orientation == Orientation.Horizontal ? size.Width : size.Height;
		}
		protected double GetLength(Rect rect) {
			return Orientation == Orientation.Horizontal ? rect.Width : rect.Height;
		}
		protected double GetWidth(Size size) {
			return Orientation == Orientation.Horizontal ? size.Height : size.Width;
		}
		protected void SetOffset(ref Rect rect, double value) {
			if (Orientation == Orientation.Horizontal)
				rect.X = value;
			else
				rect.Y = value;
		}
		protected void SetLength(ref Size size, double value) {
			if (Orientation == Orientation.Horizontal)
				size.Width = value;
			else
				size.Height = value;
		}
		protected void SetLength(ref Rect rect, double value) {
			if (Orientation == Orientation.Horizontal)
				rect.Width = value;
			else
				rect.Height = value;
		}
		protected void SetWidth(ref Size size, double value) {
			if (Orientation == Orientation.Horizontal)
				size.Height = value;
			else
				size.Width = value;
		}
		protected double ActualElementSpace { get { return Label != null && Label.GetVisible() ? ElementSpace : 0; } }
		protected Orientation Orientation {
			get { return LabelPosition == LayoutItemLabelPosition.Left ? Orientation.Horizontal : Orientation.Vertical; }
		}
		#endregion Layout
		protected virtual void OnElementSpaceChanged() {
			InvalidateMeasure();
		}
		protected virtual void OnLabelPositionChanged() {
			InvalidateMeasure();
		}
	}
	[TemplateVisualState(GroupName = RequiredStates, Name = NotRequiredState)]
	[TemplateVisualState(GroupName = RequiredStates, Name = RequiredState)]
	public class LayoutItemLabel : ContentControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty IsRequiredProperty =
			DependencyProperty.Register("IsRequired", typeof(bool), typeof(LayoutItemLabel),
				new PropertyMetadata((o, e) => ((LayoutItemLabel)o).OnIsRequiredChanged()));
		#endregion Dependency Properties
		private double _CustomWidth = double.NaN;
		private double _DesiredWidth = double.NaN;
		public LayoutItemLabel() {
			DefaultStyleKey = typeof(LayoutItemLabel);
		}
		public double CustomWidth {
			get { return _CustomWidth; }
			set {
				if (CustomWidth.Equals(value))
					return;
				_CustomWidth = value;
				InvalidateMeasure();
			}
		}
		public double DesiredWidth {
			get { return _DesiredWidth; }
			private set {
				if (DesiredWidth == value)
					return;
				double prevDesiredWidth = _DesiredWidth;
				_DesiredWidth = value;
				if (!double.IsNaN(prevDesiredWidth))
					OnDesiredWidthChanged();
			}
		}
		public Action DesiredWidthChanged;
		public bool IsRequired {
			get { return (bool)GetValue(IsRequiredProperty); }
			set { SetValue(IsRequiredProperty, value); }
		}
		#region Template
		const string RequiredStates = "RequiredStates";
		const string NotRequiredState = "NotRequired";
		const string RequiredState = "Required";
		#endregion Template
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			Size result = base.OnMeasure(availableSize);
			DesiredWidth = UIElementExtensions.GetRoundedSize(result.Width);
			if (!double.IsNaN(CustomWidth))
				result.Width = CustomWidth;
			return result;
		}
		#endregion Layout
		protected virtual void OnDesiredWidthChanged() {
			if (DesiredWidthChanged != null)
				DesiredWidthChanged();
		}
		protected virtual void OnIsRequiredChanged() {
			UpdateState(false);
		}
		protected override void UpdateState(bool useTransitions) {
			base.UpdateState(useTransitions);
			GoToState(IsRequired ? RequiredState : NotRequiredState, useTransitions);
		}
#if !SILVERLIGHT
		protected override bool IsContentInLogicalTree { get { return false; } }
#endif
	}
	public static class LayoutItemExtensions {
		public static LayoutItem GetLayoutItem(this FrameworkElement content, FrameworkElement root = null, bool explicitOnly = true) {
			LayoutItem result = content.FindElementByTypeInParents<LayoutItem>(root);
			return result != null && (!explicitOnly || result.Content == content) ? result : null;
		}
	}
}
