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
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Diagram.Native;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using System.Collections.ObjectModel;
using DevExpress.Diagram.Core;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using DevExpress.Mvvm.UI.Native;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Diagram.Core.Themes;
using DependencyPropertyHelper = System.Windows.DependencyPropertyHelper;
using System.IO;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.Diagram.Core.Base;
namespace DevExpress.Xpf.Diagram {
	public abstract partial class DiagramItem : Control, IDiagramItem, IXtraSupportDeserializeCollectionItem {
		static DiagramItem() {
			DependencyPropertyRegistrator<DiagramItem>.New()
				.AddOwner(x => x.TextAlignment, out TextAlignmentProperty, TextBlock.TextAlignmentProperty, (TextAlignment)TextBlock.TextAlignmentProperty.DefaultMetadata.DefaultValue)
				.OverrideMetadata(BackgroundProperty, Brushes.Transparent)
				.OverrideMetadata<Style>(StyleProperty, x => { }, (x, y) => x.CoerceStyle(y))
			;
			WidthProperty.OverrideMetadata(typeof(DiagramItem),
				new FrameworkPropertyMetadata((o, e) => ((DiagramItem)o).Do(x => x.OnSizeChanged(new Size((double)e.OldValue, x.Height)))));
			HeightProperty.OverrideMetadata(typeof(DiagramItem),
				new FrameworkPropertyMetadata((o, e) => ((DiagramItem)o).Do(x => x.OnSizeChanged(new Size(x.Width, (double)e.OldValue)))));
		}
		public static readonly DependencyProperty TextDecorationsProperty =
			DependencyProperty.RegisterAttached("TextDecorations", typeof(TextDecorationCollection), typeof(DiagramItem), new FrameworkPropertyMetadata(default(TextDecorationCollection), FrameworkPropertyMetadataOptions.Inherits, OnTextDecorationsChanged));
		static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var value = (TextDecorationCollection)e.NewValue;
			(d as TextBlock).Do(x => x.TextDecorations = value);
			(d as TextBox).Do(x => x.TextDecorations = value);
			BaseEditHelper.SetTextDecorations(d, value);
		}
		public TextDecorationCollection TextDecorations {
			get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}
		public TextAlignment TextAlignment {
			get { return (TextAlignment)GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
		}
		public static readonly DependencyProperty TextAlignmentProperty;
		void OnSelectionLayerChanged() { }
		ItemTraits itemTraits;
		protected DiagramItem() {
			Controller = CreateItemController();
			DiagramInput.SetInputElementFactory(this, Controller.CreateInputElement);
			itemTraits = new ItemTraits(this);
			iNestedItems = new Lazy<IList<IDiagramItem>>(() => new DevExpress.Utils.CastList<IDiagramItem, DiagramItem>(NestedItems));
			SizeChanged += (o, e) => OnActualSizeChanged();
		}
		protected virtual DiagramItemController CreateItemController() {
			return new DiagramItemController(this);
		}
		protected virtual void OnActualSizeChanged() {
			Controller.OnActualSizeChanged();
		}
		protected DiagramItemCollection CreateDiagramItemCollection() {
			return new DiagramItemCollection(this);
		}
		[Browsable(false)]
		public new Brush Background {
			get { return base.Background; }
			set { base.Background = value; }
		}
		[Browsable(false)]
		public new Brush Foreground {
			get { return base.Foreground; }
			set { base.Foreground = value; }
		}
		internal Rect Bounds {
			get { return Controller.Bounds; }
			set { Controller.Bounds = value; }
		}
		internal Point ToItemPoint(Point diagramPoint) {
			return diagramPoint.OffsetPoint(this.ActualDiagramBounds().Location.InvertPoint());
		}
		public DiagramControl GetDiagram() {
			return (DiagramControl)this.GetRootDiagram();
		}
		void OnPositionChanged(Point oldValue) {
			this.SetPosition(Position);
			Controller.OnPositionChanged(oldValue);
		}
		protected void OnSizeChanged(Size oldValue) {
			Controller.OnSizeChanged(oldValue);
		}
		static readonly IList<DiagramItem> EmptyReadOnlyList = DiagramItemController.GetEmptyItems<DiagramItem>();
		protected internal virtual IList<DiagramItem> NestedItems { get { return EmptyReadOnlyList; } }
		protected virtual IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			var ownerType = typeof(DiagramItem);
			return new [] {
				DependencyPropertyDescriptor.FromProperty(AngleProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(PositionProperty, ownerType),
			}.Concat(GetNonBrowsablePropertiesWrappers(new[] {
				DependencyPropertyDescriptor.FromProperty(BackgroundProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanDeleteProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanResizeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanMoveProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanCopyProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanRotateProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanSelectProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(IsTabStopProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(AnchorsProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanSnapToThisItemProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(CanSnapToOtherItemsProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(FontStretchProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(FontWeightProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(FontStyleProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(ForegroundProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(VerticalContentAlignmentProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(TextAlignmentProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(TextDecorationsProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(ThemeStyleIdProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(WeightProperty, ownerType),
			}))
			.Concat(Controller.GetProxyDescriptors(x => ((DiagramItem)x).itemTraits))
			.Concat(Controller.GetFontTraitsProperties())
			.Concat(Controller.GetColorProperties())
			;
		}
		internal static IEnumerable<PropertyDescriptor> GetNonBrowsablePropertiesWrappers(IEnumerable<PropertyDescriptor> properties) {
#if DEBUGTEST
			return properties;
#else
			return properties.Select(pd => pd.AddAttributes(new BrowsableAttribute(false)));
#endif
		}
		protected internal virtual IFontTraits GetFontTraits() {
			return null;
		}
		internal void OnAngleChanged() {
			Controller.OnAngleChanged();
			UpdateRotateTransform();
		}
		protected void UpdateRotateTransform() {
			this.SetRotateTransform(Angle, this.GetRotationCenter());
		}
		public override string ToString() {
			var result = string.Format("{0}: {1}", GetType().Name, Bounds);
			if(!string.IsNullOrEmpty(Name))
				result += ", Name: " + Name;
			return result;
		}
		#region Themes
		Style[] customStyles;
		Style CoerceStyle(Style originalValue) {
			if (customStyles == null)
				return originalValue;
			return DiagramThemeHelper.CreateMergedStyle(originalValue, customStyles);
		}
		internal void SetActualStyleId(DiagramItemStyleId styleId) {
			if (Controller.GetDefaultStyleId() == styleId)
				ClearValue(ThemeStyleIdProperty);
			else
				ThemeStyleId = styleId;
		}
		#endregion
		#region IDiagramItem
		readonly Lazy<IList<IDiagramItem>> iNestedItems;
		public readonly DiagramItemController Controller;
		IList<IDiagramItem> IDiagramItem.NestedItems { get { return iNestedItems.Value; } }
		DiagramItemController IDiagramItem.Controller { get { return Controller; } }
		bool IDiagramItem.IsSelected { get { return IsSelected; } set { IsSelected = value; } }
		Size IDiagramItem.Size { get { return this.GetSize(); } set { this.SetSize(value); } }
		Size IDiagramItem.MinSize { get { return this.GetMinSize(); } }
		Size IDiagramItem.ActualSize { get { return this.GetSize().CoerceNaNSize(this.GetActualSize()).CoerceMinSize(this.GetMinSize()); } }
		IEnumerable<PropertyDescriptor> IDiagramItem.GetEditableProperties() {
			return GetEditablePropertiesCore();
		}
		void IDiagramItem.SetCustomStyles(IDiagramItemStyle[] styles) {
			customStyles = styles.Select(style => (style as DXDiagramItemStyle).With(x => x.Style)).ToArray();
			CoerceValue(StyleProperty);
		}
		PropertyDescriptor IDiagramItem.GetRealProperty(PropertyDescriptor pd) {
			var sizeProperty = ExpressionHelper.GetProperty((ItemTraits x) => x.Size);
			if(pd.Name ==  sizeProperty.Name) { 
				return ProxyPropertyDescriptor.Create(sizeProperty, (DiagramItem x) => x.itemTraits);
			}
			return TypeDescriptor.GetProperties(this)[pd.Name] ?? pd;
		}
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return DiagramItemController.CreateCollectionItem(propertyName, e);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			DiagramItemController.SetIndexCollectionItem(propertyName, e);
		}
		#endregion
		#endregion
	}
	public class ItemTraits {
		readonly DiagramItem item;
		public ItemTraits(DiagramItem item) {
			this.item = item;
		}
		public Size Size {
			get { return item.GetSize(); }
			set { item.SetSize(value); }
		}
		public bool ShouldSerializeSize() {
			return new[] { DiagramItem.WidthProperty, DiagramItem.HeightProperty }
				.Select(x => DependencyPropertyDescriptor.FromProperty(x, typeof(DiagramItem)))
				.Any(x => x.ShouldSerializeValue(item));
		}
	}
	public static class TextProperties {
		public static TextAlignment GetTextAlignment(DependencyObject obj) {
			return (TextAlignment)obj.GetValue(TextAlignmentProperty);
		}
		public static void SetTextAlignment(DependencyObject obj, TextAlignment value) {
			obj.SetValue(TextAlignmentProperty, value);
		}
		public static readonly DependencyProperty TextAlignmentProperty =
			DependencyProperty.RegisterAttached("TextAlignment", typeof(TextAlignment), typeof(TextProperties),
				new FrameworkPropertyMetadata(TextBlock.TextAlignmentProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, OnChanged)
			);
		static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TextBlock.SetTextAlignment(d, (TextAlignment)e.NewValue);
		}
	}
}
