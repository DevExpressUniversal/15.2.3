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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum LabelRenderMode {
		RectangleConnectedToCenter,
		Rectangle,
		CustomShape
	}
	public class SeriesLabel : ChartTextElement, ICustomTypeDescriptor {
		public static readonly new DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(SeriesLabel), new PropertyMetadata(false, VisiblePropertyChanged));
		public static readonly DependencyProperty ItemsProperty = DependencyPropertyManager.Register("Items",
			typeof(ObservableCollection<SeriesLabelItem>), typeof(SeriesLabel));
		public static readonly DependencyProperty IndentProperty = DependencyPropertyManager.Register("Indent",
			typeof(int), typeof(SeriesLabel), new PropertyMetadata(10, ChartElementHelper.UpdateWithClearDiagramCache), ValidateIndent);
		public static readonly DependencyProperty RenderModeProperty = DependencyPropertyManager.Register("RenderMode",
			typeof(LabelRenderMode), typeof(SeriesLabel), new PropertyMetadata(LabelRenderMode.RectangleConnectedToCenter, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ConnectorVisibleProperty = DependencyPropertyManager.Register("ConnectorVisible",
			typeof(bool), typeof(SeriesLabel), new PropertyMetadata(true, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ConnectorThicknessProperty = DependencyPropertyManager.Register("ConnectorThickness",
			typeof(int), typeof(SeriesLabel), new PropertyMetadata(1, ChartElementHelper.UpdateWithClearDiagramCache), ValidateConnectorThickness);
		public static readonly DependencyProperty ResolveOverlappingModeProperty = DependencyPropertyManager.Register("ResolveOverlappingMode",
			typeof(ResolveOverlappingMode), typeof(SeriesLabel), new PropertyMetadata(ResolveOverlappingMode.None, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty TextPatternProperty = DependencyPropertyManager.Register("TextPattern",
			typeof(string), typeof(SeriesLabel), new PropertyMetadata(String.Empty, TextPatternPropertyChanged));
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesLabel label = d as SeriesLabel;
			if (label != null && label.Series != null)
				label.Series.LabelsVisibility = (bool)e.NewValue;
		}
		static void TextPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesLabel label = d as SeriesLabel;
			if (label != null && !label.changeLocker)
				ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public ObservableCollection<SeriesLabelItem> Items {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelIndent"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Indent {
			get { return (int)GetValue(IndentProperty); }
			set { SetValue(IndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelRenderMode"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public LabelRenderMode RenderMode {
			get { return (LabelRenderMode)GetValue(RenderModeProperty); }
			set { SetValue(RenderModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelConnectorVisible"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool ConnectorVisible {
			get { return (bool)GetValue(ConnectorVisibleProperty); }
			set { SetValue(ConnectorVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelConnectorThickness"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int ConnectorThickness {
			get { return (int)GetValue(ConnectorThicknessProperty); }
			set { SetValue(ConnectorThicknessProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelResolveOverlappingMode"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ResolveOverlappingMode ResolveOverlappingMode {
			get { return (ResolveOverlappingMode)GetValue(ResolveOverlappingModeProperty); }
			set { SetValue(ResolveOverlappingModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesLabelTextPattern"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string TextPattern {
			get { return (string)GetValue(TextPatternProperty); }
			set { SetValue(TextPatternProperty, value); }
		}
		static bool ValidateIndent(object value) {
			return (int)value >= 0;
		}
		static bool ValidateConnectorThickness(object connectorThickness) {
			return (int)connectorThickness >= 0;
		}
		internal Series Series { get { return ((IOwnedElement)this).Owner as Series; } }
		[
		Obsolete(ObsoleteMessages.SeriesLabelVisibleProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new bool Visible {
			get { return Series != null ? Series.LabelsVisibility : (bool)GetValue(VisibleProperty); }
			set {
				if (Series != null)
					Series.LabelsVisibility = value;
				else
					SetValue(VisibleProperty, value);
			}
		}
		IDataLabelFormatter formatter;
		bool changeLocker = false;
		internal IDataLabelFormatter Formatter {
			get { return formatter; }
			set { formatter = value; }
		}
		static SeriesLabel() {
			FlowDirectionProperty.OverrideMetadata(typeof(SeriesLabel), new FrameworkPropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		}
		public SeriesLabel() {
			DefaultStyleKey = typeof(SeriesLabel);
		}
		#region ICustomTypeDescriptor implementation
		string ICustomTypeDescriptor.GetClassName() {
			return TypeDescriptor.GetClassName(this, true);
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return TypeDescriptor.GetComponentName(this, true);
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		bool IsPropertySupported(PropertyDescriptor property) {
			if (property.Name == ResolveOverlappingModeProperty.Name && !Series.LabelsResolveOverlappingSupported)
				return false;
			if (property.Name == ConnectorVisibleProperty.Name && !Series.LabelConnectorSupported)
				return false;
			if (property.Name == ConnectorThicknessProperty.Name && !Series.LabelConnectorSupported)
				return false;
			return true;
		}
		PropertyDescriptorCollection GetFilteredProperties(PropertyDescriptorCollection properties) {
			if (Series == null)
				return properties;
			List<PropertyDescriptor> filteredProperties = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor property in properties)
				if (IsPropertySupported(property))
					filteredProperties.Add(property);
			return new PropertyDescriptorCollection(filteredProperties.ToArray());
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return GetFilteredProperties(TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return GetFilteredProperties(TypeDescriptor.GetProperties(this, attributes, true));
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
		void UpdateConnectorItems(ObservableCollection<SeriesLabelItem> labelItems) {
			foreach (SeriesLabelItem labelItem in labelItems) {
				bool connectorItemVisible = (Series != null && Series.IsLabelConnectorItemVisible);
				labelItem.ConnectorItem = new SeriesLabelConnectorItem(labelItem);
				labelItem.ShowConnector = connectorItemVisible;
			}
		}
		internal void UpdateItems() {
			List<SeriesPointItem> pointItems = Series != null ? Series.GetPointItemsForLabels() : null;
			ObservableCollection<SeriesLabelItem> items = Items;
			if (items == null || pointItems == null || items.Count != pointItems.Count)
				items = new ObservableCollection<SeriesLabelItem>();
			if (pointItems != null) {
				for (int i = 0; i < pointItems.Count; i++) {
					if (items.Count <= i)
						items.Add(new SeriesLabelItem(this));
					items[i].PointItem = pointItems[i];
					pointItems[i].LabelItem = items[i];
				}
			}
			UpdateConnectorItems(items);
			Items = items;
		}
		internal void Assign(SeriesLabel label) {
			if (label != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, label, StyleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, VisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, ElementTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, ConnectorVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, ConnectorThicknessProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, IndentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, ForegroundProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, FontFamilyProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, FontSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, FontStretchProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, FontStyleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, FontWeightProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, RenderModeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, ResolveOverlappingModeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, label, TextPatternProperty);
			}
		}
		protected internal override void ChangeOwner(IChartElement oldOwner, IChartElement newOwner) {
			base.ChangeOwner(oldOwner, newOwner);
			if (Series != null && (bool)GetValue(VisibleProperty) == true)
				Series.LabelsVisibility = true;
		}
		internal void UpdateTextPattern(string pattern) {
			changeLocker = true;
			TextPattern = pattern;
			changeLocker = false;
		}
	}
}
