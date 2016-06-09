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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public enum FREInheritedPropertyValueSource {
		Default = 0x1, Inherited = 0x2, Local = 0x4, Coerced = 0x8
	}
	public struct FREInheritedPropertyRecord {
		public object DefaultValue { get; set; }
		public Action<FrameworkRenderElementContext, object, object> PropertyChangedCallback { get; set; }
		public Func<FrameworkRenderElementContext, object, FREInheritedPropertyValueSource, object> CoerceValueCallback { get; set; }
	}
	public class FREInheritedPropertyValue {
		public static readonly object unsetValue = null;
		readonly FREInheritedPropertyRecord record;
		readonly FREInheritedProperties owner;
		readonly int key;
		object cachedCoercedValue;
		object cachedInheritedValue;
		object cachedLocalValue;
		object cachedValue;
		public FREInheritedPropertyValue(FREInheritedPropertyRecord record, FREInheritedProperties owner, int key) {
			this.record = record;
			this.owner = owner;
			this.cachedCoercedValue = record.DefaultValue;
			this.cachedInheritedValue = record.DefaultValue;
			this.cachedLocalValue = record.DefaultValue;
			this.cachedValue = record.DefaultValue;
			this.key = key;
			ValueSource = FREInheritedPropertyValueSource.Default;
		}
		public FREInheritedPropertyValueSource ValueSource { get; set; }
		public int Key { get { return key; } }
		public bool IsDefaultValue { get { return Equals(record.DefaultValue, GetValue()); } }
		public object GetValue() { return cachedValue; }
		public void SetValue(object value) { SetCurrentValue(value, FREInheritedPropertyValueSource.Local); }
		public void ClearValue() { SetCurrentValue(unsetValue, FREInheritedPropertyValueSource.Default); }
		public void SetCurrentValue(object value, FREInheritedPropertyValueSource valueSource) {
			var oldValue = cachedValue;
			if (Equals(unsetValue, value)) {
				cachedLocalValue = record.DefaultValue;
				valueSource = FREInheritedPropertyValueSource.Default;
			}
			if (valueSource.HasFlag(FREInheritedPropertyValueSource.Default)) {
				cachedValue = cachedLocalValue;
			}
			if (valueSource.HasFlag(FREInheritedPropertyValueSource.Inherited)) {
				cachedInheritedValue = value;
				cachedValue = cachedInheritedValue;
			}
			if (valueSource.HasFlag(FREInheritedPropertyValueSource.Local)) {
				cachedLocalValue = value;
				cachedValue = cachedLocalValue;
			}
			if (record.CoerceValueCallback != null) {
				cachedCoercedValue = record.CoerceValueCallback(owner.Context, value, valueSource);
				if (!Equals(cachedCoercedValue, value)) {
					valueSource |= FREInheritedPropertyValueSource.Coerced;
				}
				cachedValue = cachedCoercedValue;
			}
			ValueSource = valueSource;
			ValueChanged(oldValue, cachedValue, valueSource);
		}
		void ValueChanged(object oldValue, object newValue, FREInheritedPropertyValueSource newValueSource) {
			if (!Equals(oldValue, newValue)) {
				if (record.PropertyChangedCallback != null)
					record.PropertyChangedCallback(owner.Context, oldValue, newValue);
			}
			PropagateValue(newValueSource);
		}
		public void CoerceValue() {
			object value = unsetValue;
			if (ValueSource.HasFlag(FREInheritedPropertyValueSource.Default) || ValueSource.HasFlag(FREInheritedPropertyValueSource.Local))
				value = cachedLocalValue;
			if (ValueSource.HasFlag(FREInheritedPropertyValueSource.Inherited))
				value = cachedInheritedValue;
			SetCurrentValue(value, ValueSource);
		}
		void PropagateValue(FREInheritedPropertyValueSource valueSource) {
			if (valueSource.HasFlag(FREInheritedPropertyValueSource.Default)) {
				var parent = owner.Context.Parent;
				if (parent != null) {
					var parentValueSource = parent.InheritedProperties.GetValueSource(Key);
					if (!parentValueSource.HasFlag(FREInheritedPropertyValueSource.Default) || parentValueSource.HasFlag(FREInheritedPropertyValueSource.Coerced)) {
						SetCurrentValue(parent.InheritedProperties.GetValue(Key), FREInheritedPropertyValueSource.Inherited);
						return;
					}
				}
			}
			var context = (IFrameworkRenderElementContext)owner.Context;
			if (context == null)
				return;
			for (int i = 0; i < context.RenderChildrenCount; i++) {
				var child = context.GetRenderChild(i);
				var childValueSource = child.InheritedProperties.GetValueSource(Key);
				if (childValueSource.HasFlag(FREInheritedPropertyValueSource.Local))
					continue;
				child.InheritedProperties.SetCurrentValue(Key, GetValue(), FREInheritedPropertyValueSource.Inherited);
			}
		}
	}
	public class FREInheritedProperties {
		#region static
		static int lastAllocated = 0;
		static readonly List<FREInheritedPropertyRecord> records = new List<FREInheritedPropertyRecord>(50);
		static FREInheritedProperties() {
		}
		public static int Register(object defaultValue = null, Action<FrameworkRenderElementContext, object, object> propertyChangedCallback = null, Func<FrameworkRenderElementContext, object, FREInheritedPropertyValueSource, object> coerceValueCallback = null) {
			records.Add(new FREInheritedPropertyRecord()
			{
				DefaultValue = defaultValue,
				PropertyChangedCallback = propertyChangedCallback,
				CoerceValueCallback = coerceValueCallback
			});
			return lastAllocated++;
		}
		#endregion
		readonly FREInheritedPropertyValue[] values;
		public FrameworkRenderElementContext Context { get; private set; }
		public FREInheritedProperties(FrameworkRenderElementContext context) {
			this.Context = context;
			values = new FREInheritedPropertyValue[lastAllocated];
		}
		public void SetValue(int key, object value) {
			SetCurrentValue(key, value, FREInheritedPropertyValueSource.Local);
		}
		public object GetValue(int key) {
			var container = values[key];
			if (container == null)
				return records[key].DefaultValue;
			return container.GetValue();
		}
		public void CoerceValue(int key) {
			var container = values[key];
			if (container == null) {
				container = new FREInheritedPropertyValue(records[key], this, key);
				values[key] = container;
			}
			container.CoerceValue();
		}
		public void CoerceValues() {
			var cParent = Context.Parent;
			if (cParent == null)
				return;
			var parentValues = cParent.InheritedProperties.values;
			for (int i = 0; i < records.Count; i++) {
				var pValue = parentValues[i];
				if (pValue != null && !pValue.IsDefaultValue)
					CoerceValue(i);
			}
		}
		public FREInheritedPropertyValueSource GetValueSource(int key) {
			var container = values[key];
			if (container == null)
				return FREInheritedPropertyValueSource.Default;
			return container.ValueSource;
		}
		public void SetCurrentValue(int key, object value, FREInheritedPropertyValueSource source) {
			var container = values[key];
			if (container == null) {
				if (FREInheritedPropertyValue.unsetValue == value)
					return;
				container = new FREInheritedPropertyValue(records[key], this, key);
				values[key] = container;
			}
			container.SetCurrentValue(value, source);
		}
	}
	[Flags]
	public enum FREInvalidateOptions {
		None = 0x1,
		AffectsMeasure = 0x2,
		AffectsArrange = 0x4,
		AffectsVisual = 0x8,
		AffectsRenderCaches = 0x10,
		AffectsGeneralCaches = 0x20,
		AffectsChildrenCaches = 0x40,
		AffectsOpacity = 0x80,
		AffectsMeasureAndVisual = AffectsMeasure | AffectsVisual,
		UpdateVisual = AffectsArrange | AffectsVisual | AffectsRenderCaches,
		UpdateLayout = AffectsMeasureAndVisual | AffectsRenderCaches,
		UpdateSubTree = UpdateLayout | AffectsGeneralCaches | AffectsChildrenCaches,
	}
	public abstract class FrameworkRenderElement {
		static readonly object locker = new object();
		public static double DpiScaleX {
			get { return ScreenHelper.ScaleX; }
		}
		public static double DpiScaleY {
			get { return DpiScaleX; }
		}
		protected static double RoundLayoutValue(double value, double dpiScale) {
			double d = value;
			if (!dpiScale.AreClose(1.0)) {
				d = Math.Round(value * dpiScale) / dpiScale;
				if (double.IsNaN(d) || double.IsInfinity(d) || d.AreClose(double.MaxValue))
					d = value;
			}
			else
				d = Math.Round(d);
			return d;
		}
		protected static Size RoundLayoutSize(Size size, double dpiScaleX, double dpiScaleY) {
			return new Size(RoundLayoutValue(size.Width, dpiScaleX), RoundLayoutValue(size.Height, dpiScaleY));
		}
		protected static Rect RoundLayoutRect(Rect rect, double dpiScaleX, double dpiScaleY) {
			return new Rect(RoundLayoutValue(rect.X, dpiScaleX), RoundLayoutValue(rect.Y, dpiScaleY), RoundLayoutValue(rect.Width, dpiScaleX), RoundLayoutValue(rect.Height, dpiScaleY));
		}
		#region internal classes
		struct MinMax {
			public readonly double MinWidth;
			public readonly double MaxWidth;
			public readonly double MinHeight;
			public readonly double MaxHeight;
			internal MinMax(FrameworkRenderElementContext context) {
				FrameworkRenderElement e = context.Factory;
				MaxHeight = context.MaxHeight.HasValue ? Math.Min(e.MaxHeight, context.MaxHeight.Value) : e.MaxHeight;
				MinHeight = context.MinHeight.HasValue ? Math.Max(e.MinHeight, context.MinHeight.Value) : e.MinHeight;
				double height = context.Height ?? e.Height;
				MaxHeight = Math.Max(Math.Min(double.IsNaN(height) ? double.PositiveInfinity : height, MaxHeight), MinHeight);
				MinHeight = Math.Max(Math.Min(MaxHeight, double.IsNaN(height) ? 0d : height), MinHeight);
				MaxWidth = context.MaxWidth.HasValue ? Math.Min(e.MaxWidth, context.MaxWidth.Value) : e.MaxWidth;
				MinWidth = context.MinWidth.HasValue ? Math.Max(e.MinWidth, context.MinWidth.Value) : e.MinWidth;
				double width = context.Width ?? e.Width;
				MaxWidth = Math.Max(Math.Min(double.IsNaN(width) ? double.PositiveInfinity : width, MaxWidth), MinWidth);
				MinWidth = Math.Max(Math.Min(MaxWidth, double.IsNaN(width) ? 0d : width), MinWidth);
			}
		}
		#endregion
		bool? useLayoutRounding;
		double width;
		double height;
		double minWidth;
		double maxWidth;
		double minHeight;
		double maxHeight;
		Thickness margin;
		string name;
		HorizontalAlignment ha;
		VerticalAlignment va;
		Visibility vi;
		Dock? dock;
		FlowDirection? fd;
		double opacity = 1d;
		bool showBounds;
		bool shouldCalcDpiAwareThickness = true;
		Brush foreground;
		bool clipToBounds;
		bool contentSpecificClipToBounds;
		public FrameworkRenderElement Parent { get; internal set; }
		public Brush Foreground {
			get { return foreground; }
			set { SetProperty(ref foreground, value); }
		}
		public FlowDirection? FlowDirection {
			get { return fd; }
			set { SetProperty(ref fd, value); }
		}
		public bool ShowBounds {
			get { return showBounds; }
			set { SetProperty(ref showBounds, value); }
		}
		public bool? UseLayoutRounding {
			get { return useLayoutRounding; }
			set { SetProperty(ref useLayoutRounding, value); }
		}
		public bool ClipToBounds {
			get { return clipToBounds; }
			set { SetProperty(ref clipToBounds, value); }
		}
		public bool ContentSpecificClipToBounds {
			get { return contentSpecificClipToBounds; }
			set { SetProperty(ref contentSpecificClipToBounds, value); }
		}
		public double Opacity {
			get { return opacity; }
			set { SetProperty(ref opacity, Math.Max(Math.Min(value, 1d), 0d)); }
		}
		public double Width {
			get { return width; }
			set { SetProperty(ref width, value); }
		}
		public double Height {
			get { return height; }
			set { SetProperty(ref height, value); }
		}
		public double MinWidth {
			get { return minWidth; }
			set { SetProperty(ref minWidth, value); }
		}
		public double MaxWidth {
			get { return maxWidth; }
			set { SetProperty(ref maxWidth, value); }
		}
		public double MinHeight {
			get { return minHeight; }
			set { SetProperty(ref minHeight, value); }
		}
		public double MaxHeight {
			get { return maxHeight; }
			set { SetProperty(ref maxHeight, value); }
		}
		public Thickness Margin {
			get { return margin; }
			set { SetProperty(ref margin, value); }
		}
		public string Name {
			get { return name; }
			set { SetProperty(ref name, value); }
		}
		public HorizontalAlignment HorizontalAlignment {
			get { return ha; }
			set { SetProperty(ref ha, value); }
		}
		public VerticalAlignment VerticalAlignment {
			get { return va; }
			set { SetProperty(ref va, value); }
		}
		public Visibility Visibility {
			get { return vi; }
			set { SetProperty(ref vi, value); }
		}
		public Dock? Dock {
			get { return dock; }
			set { SetProperty(ref dock, value); }
		}
		public bool ShouldCalcDpiAwareThickness {
			get { return shouldCalcDpiAwareThickness; }
			set { SetProperty(ref shouldCalcDpiAwareThickness, value); }
		}
		public string ThemeInfo { get; set; }
		bool isFreezed;
		protected FrameworkRenderElement() {
			Width = double.NaN;
			Height = double.NaN;
			MaxWidth = double.PositiveInfinity;
			MaxHeight = double.PositiveInfinity;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}
		public void Measure(Size availableSize, FrameworkRenderElementContext context) {
			ApplyTemplate(context);
			MeasureCore(availableSize, context);
			if (double.IsInfinity(context.DesiredSize.Width) || double.IsInfinity(context.DesiredSize.Height))
				throw new ArgumentException("desiredSize has infinity member");
		}
		void MeasureCore(Size availableSize, FrameworkRenderElementContext context) {
			var visibility = CalcVisibility(context);
			if (visibility == Visibility.Collapsed) {
				context.DesiredSize = new Size(0, 0);
				return;
			}
			bool useLayoutRounding = context.UseLayoutRounding;
			Thickness margin = CalcDpiAwareThickness(context, context.Margin ?? Margin);
			double marginWidth = margin.Left + margin.Right;
			double marginHeight = margin.Top + margin.Bottom;
			Size size = new Size(Math.Max(availableSize.Width - marginWidth, 0d), Math.Max(availableSize.Height - marginHeight, 0));
			MinMax minMax = new MinMax(context);
			size.Width = Math.Max(minMax.MinWidth, Math.Min(size.Width, minMax.MaxWidth));
			size.Height = Math.Max(minMax.MinHeight, Math.Min(size.Height, minMax.MaxHeight));
			if (context.UseLayoutRounding)
				size = RoundLayoutSize(size, DpiScaleX, DpiScaleY);
			Size desiredSize = MeasureOverride(size, context);
			desiredSize = new Size(Math.Max(desiredSize.Width, minMax.MinWidth), Math.Max(desiredSize.Height, minMax.MinHeight));
			Size unclippedDesiredSize = desiredSize;
			bool clipped = false;
			if (desiredSize.Width > minMax.MaxWidth) {
				desiredSize.Width = minMax.MaxWidth;
				clipped = true;
			}
			if (desiredSize.Height > minMax.MaxHeight) {
				desiredSize.Height = minMax.MaxHeight;
				clipped = true;
			}
			double clippedDesiredWidth = desiredSize.Width + marginWidth;
			double clippedDesiredHeight = desiredSize.Height + marginHeight;
			if (clippedDesiredWidth > availableSize.Width) {
				clippedDesiredWidth = availableSize.Width;
				clipped = true;
			}
			if (clippedDesiredHeight > availableSize.Height) {
				clippedDesiredHeight = availableSize.Height;
				clipped = true;
			}
			if (useLayoutRounding) {
				clippedDesiredWidth = RoundLayoutValue(clippedDesiredWidth, ScreenHelper.ScaleX);
				clippedDesiredHeight = RoundLayoutValue(clippedDesiredHeight, ScreenHelper.ScaleX);
			}
			if (clipped || clippedDesiredWidth < 0 || clippedDesiredHeight < 0)
				context.UnclippedDesiredSize = unclippedDesiredSize;
			else
				context.UnclippedDesiredSize = null;
			context.DesiredSize = new Size(Math.Max(0, clippedDesiredWidth), Math.Max(0, clippedDesiredHeight));
		}
		protected Thickness CalcDpiAwareThickness(FrameworkRenderElementContext context, Thickness thickness) {
			return ShouldCalcDpiAwareThickness && context.UseLayoutRounding ? thickness.Multiply(ScreenHelper.DpiThicknessCorrection) : thickness;
		}
		public void ApplyTemplate(FrameworkRenderElementContext context) {
			var visibility = context.Visibility ?? Visibility;
			if (visibility == Visibility.Collapsed)
				return;
			PreApplyTemplate(context);
		}
		protected virtual void PreApplyTemplate(FrameworkRenderElementContext context) {
			context.PreApplyTemplate();
		}
		public void Arrange(Rect finalRect, FrameworkRenderElementContext context) {
			ArrangeCore(finalRect, context);
		}
		void ArrangeCore(Rect finalRect, FrameworkRenderElementContext context) {
			var visibility = CalcVisibility(context);
			if (visibility == Visibility.Collapsed) {
				context.RenderSize = new Size(0, 0);
				context.VisualOffset = new Vector(0, 0);
				return;
			}
			context.NeedsClipBounds = false;
			HorizontalAlignment horizontalAlignment = context.HorizontalAlignment ?? HorizontalAlignment;
			VerticalAlignment verticalAlignment = context.VerticalAlignment ?? VerticalAlignment;
			Size arrangeSize = finalRect.Size;
			Thickness margin = CalcDpiAwareThickness(context, context.Margin ?? Margin);
			double marginWidth = margin.Left + margin.Right;
			double marginHeight = margin.Top + margin.Bottom;
			arrangeSize.Width = Math.Max(0.0, arrangeSize.Width - marginWidth);
			arrangeSize.Height = Math.Max(0.0, arrangeSize.Height - marginHeight);
			Size unclippedDesiredSize = !ContentSpecificClipToBounds && context.UnclippedDesiredSize != null ? context.UnclippedDesiredSize.Value : new Size(Math.Max(0d, context.DesiredSize.Width - marginWidth), Math.Max(0d, context.DesiredSize.Height - marginHeight));
			if (arrangeSize.Width.LessThan(unclippedDesiredSize.Width)) {
				context.NeedsClipBounds = true;
				arrangeSize.Width = unclippedDesiredSize.Width;
			}
			if (arrangeSize.Height.LessThan(unclippedDesiredSize.Height)) {
				context.NeedsClipBounds = true;
				arrangeSize.Height = unclippedDesiredSize.Height;
			}
			if (horizontalAlignment != HorizontalAlignment.Stretch)
				arrangeSize.Width = unclippedDesiredSize.Width;
			if (verticalAlignment != VerticalAlignment.Stretch)
				arrangeSize.Height = unclippedDesiredSize.Height;
			MinMax minMax = new MinMax(context);
			double effectiveMaxWidth = Math.Max(unclippedDesiredSize.Width, minMax.MaxWidth);
			if (effectiveMaxWidth.LessThan(arrangeSize.Width)) {
				context.NeedsClipBounds = true;
				arrangeSize.Width = effectiveMaxWidth;
			}
			double effectiveMaxHeight = Math.Max(unclippedDesiredSize.Height, minMax.MaxHeight);
			if (effectiveMaxHeight.LessThan(arrangeSize.Height)) {
				context.NeedsClipBounds = true;
				arrangeSize.Height = effectiveMaxHeight;
			}
			if (context.UseLayoutRounding)
				arrangeSize = RoundLayoutSize(arrangeSize, DpiScaleX, DpiScaleY);
			Size oldRenderSize = context.RenderSize;
			if (oldRenderSize != arrangeSize)
				context.ResetSizeSpecificCaches();
			Size innerInkSize = ArrangeOverride(arrangeSize, context);
			context.RenderSize = innerInkSize;
			if (context.UseLayoutRounding)
				context.RenderSize = RoundLayoutSize(innerInkSize, DpiScaleX, DpiScaleY);
			Size clippedInkSize = new Size(Math.Min(innerInkSize.Width, minMax.MaxWidth), Math.Min(innerInkSize.Height, minMax.MaxHeight));
			if (context.UseLayoutRounding)
				clippedInkSize = RoundLayoutSize(clippedInkSize, DpiScaleX, DpiScaleY);
			context.NeedsClipBounds |= clippedInkSize.Width.LessThan(innerInkSize.Width) || clippedInkSize.Height.LessThan(innerInkSize.Height);
			Size clientSize = new Size(Math.Max(0.0, finalRect.Width - marginWidth), Math.Max(0.0, finalRect.Height - marginHeight));
			if (context.UseLayoutRounding) {
				clientSize = RoundLayoutSize(clientSize, DpiScaleX, DpiScaleY);
			}
			context.NeedsClipBounds |= clientSize.Width.LessThan(clippedInkSize.Width) || clientSize.Height.LessThan(clippedInkSize.Height);
			Vector alignmentOffset = this.ComputeAlignmentOffset(clientSize, clippedInkSize, horizontalAlignment, verticalAlignment);
			if (context.UseLayoutRounding) {
				alignmentOffset.X = RoundLayoutValue(alignmentOffset.X + finalRect.X + margin.Left, DpiScaleX);
				alignmentOffset.Y = RoundLayoutValue(alignmentOffset.Y + finalRect.Y + margin.Top, DpiScaleY);
			}
			else {
				alignmentOffset.X += finalRect.X + margin.Left;
				alignmentOffset.Y += finalRect.Y + margin.Top;
			}
			context.VisualOffset = alignmentOffset;
			context.LayoutClip = GetLayoutClip(finalRect.Size, context);
		}
		Vector ComputeAlignmentOffset(Size clientSize, Size inkSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) {
			Vector vector = new Vector();
			if (horizontalAlignment == HorizontalAlignment.Stretch && inkSize.Width > clientSize.Width)
				horizontalAlignment = HorizontalAlignment.Left;
			if (verticalAlignment == VerticalAlignment.Stretch && inkSize.Height > clientSize.Height)
				verticalAlignment = VerticalAlignment.Top;
			vector.X = horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Stretch
				? (clientSize.Width - inkSize.Width) * 0.5
				: (horizontalAlignment != HorizontalAlignment.Right ? 0.0 : clientSize.Width - inkSize.Width);
			vector.Y = verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Stretch
				? (clientSize.Height - inkSize.Height) * 0.5
				: (verticalAlignment != VerticalAlignment.Bottom ? 0.0 : clientSize.Height - inkSize.Height);
			return vector;
		}
		public void Render(DrawingContext dc, FrameworkRenderElementContext context) {
			RenderCore(dc, context);
		}
		protected virtual Geometry GetLayoutClip(Size layoutSlotSize, FrameworkRenderElementContext context) {
			bool useLayoutRounding = context.UseLayoutRounding;
			bool needsClipBounds = context.NeedsClipBounds;
			bool clipToBounds = ClipToBounds;
			if (needsClipBounds || clipToBounds) {
				MinMax mm = new MinMax(context);
				Size inkSize = context.RenderSize;
				double maxWidthClip = (double.IsPositiveInfinity(mm.MaxWidth) ? inkSize.Width : mm.MaxWidth);
				double maxHeightClip = (double.IsPositiveInfinity(mm.MaxHeight) ? inkSize.Height : mm.MaxHeight);
				bool needToClipLocally = clipToBounds || maxWidthClip.LessThan(inkSize.Width) || maxHeightClip.LessThan(inkSize.Height);
				inkSize.Width = Math.Min(inkSize.Width, mm.MaxWidth);
				inkSize.Height = Math.Min(inkSize.Height, mm.MaxHeight);
				Rect inkRectTransformed = new Rect();
				Thickness margin = CalcDpiAwareThickness(context, context.Margin ?? Margin);
				double marginWidth = margin.Left + margin.Right;
				double marginHeight = margin.Top + margin.Bottom;
				Size clippingSize = new Size(Math.Max(0, layoutSlotSize.Width - marginWidth), Math.Max(0, layoutSlotSize.Height - marginHeight));
				bool needToClipSlot = clipToBounds || clippingSize.Width.LessThan(inkSize.Width) || clippingSize.Height.LessThan(inkSize.Height) || CalcNeedToClipSlot(inkSize, context);
				Transform rtlMirror = GetFlowDirectionTransform(context);
				if (needToClipLocally && !needToClipSlot) {
					Rect clipRect = new Rect(0, 0, maxWidthClip, maxHeightClip);
					if (useLayoutRounding)
						clipRect = RoundLayoutRect(clipRect, DpiScaleX, DpiScaleY);
					RectangleGeometry localClip = new RectangleGeometry(clipRect);
					if (rtlMirror != null)
						localClip.Transform = rtlMirror;
					return localClip;
				}
				if (needToClipSlot) {
					HorizontalAlignment horizontalAlignment = context.HorizontalAlignment ?? HorizontalAlignment;
					VerticalAlignment verticalAlignment = context.VerticalAlignment ?? VerticalAlignment;
					Vector offset = ComputeAlignmentOffset(clippingSize, inkSize, horizontalAlignment, verticalAlignment);
					Rect slotRect = new Rect(-offset.X + inkRectTransformed.X, -offset.Y + inkRectTransformed.Y, clippingSize.Width, clippingSize.Height);
					if (useLayoutRounding)
						slotRect = RoundLayoutRect(slotRect, DpiScaleX, DpiScaleY);
					if (needToClipLocally) 
					{
						Rect localRect = new Rect(0, 0, maxWidthClip, maxHeightClip);
						if (useLayoutRounding) {
							localRect = RoundLayoutRect(localRect, DpiScaleX, DpiScaleY);
						}
						slotRect.Intersect(localRect);
					}
					RectangleGeometry combinedClip = new RectangleGeometry(slotRect);
					if (rtlMirror != null)
						combinedClip.Transform = rtlMirror;
					return combinedClip;
				}
				return null;
			}
			return GetLayoutClipBase(layoutSlotSize, context);
		}
		protected virtual bool CalcNeedToClipSlot(Size inkSize, FrameworkRenderElementContext context) {
			return false;
		}
		protected virtual Geometry GetLayoutClipBase(Size layoutSlotSize, FrameworkRenderElementContext context) {
			if (ClipToBounds) {
				RectangleGeometry rect = new RectangleGeometry(new Rect(context.RenderSize));
				rect.Freeze();
				return rect;
			}
			return null;
		}
		Transform GetFlowDirectionTransform(FrameworkRenderElementContext context) {
			if (ShouldApplyMirrorTransform(context))
				return new MatrixTransform(-1.0, 0.0, 0.0, 1.0, context.RenderSize.Width, 0.0);
			return null;
		}
		bool ShouldApplyMirrorTransform(FrameworkRenderElementContext context) {
			FlowDirection thisFlowDirection = context.FlowDirection;
			FlowDirection parentFlowDirection = System.Windows.FlowDirection.LeftToRight;
			if (context.Parent != null)
				parentFlowDirection = context.Parent.FlowDirection;
			return ApplyMirrorTransform(parentFlowDirection, thisFlowDirection);
		}
		bool ApplyMirrorTransform(FlowDirection parentFD, FlowDirection thisFD) {
			return ((parentFD == System.Windows.FlowDirection.LeftToRight && thisFD == System.Windows.FlowDirection.RightToLeft) ||
					(parentFD == System.Windows.FlowDirection.RightToLeft && thisFD == System.Windows.FlowDirection.LeftToRight));
		}
		void RenderCore(DrawingContext dc, FrameworkRenderElementContext context) {
			var visibility = CalcRenderVisibility(context);
			if (visibility == Visibility.Collapsed)
				return;
			IFrameworkRenderElementContext c = context;
			bool shouldUseTransform = context.ShouldUseTransform();
			if (shouldUseTransform) {
				context.UpdateRenderTransform();
				dc.PushTransform(context.RenderTransform);
			}
			bool shouldUseClipping = ShouldUseLayoutClip(context);
			if (shouldUseClipping)
				dc.PushClip(context.LayoutClip);
			double opacity = context.ActualOpacity;
			bool shouldUseOpacity = visibility == Visibility.Visible && !opacity.AreClose(1d);
			if (shouldUseOpacity)
				dc.PushOpacity(opacity);
			if (visibility == Visibility.Visible)
				RenderOverride(dc, context);
			for (int i = 0; i < c.RenderChildrenCount; i++) {
				var child = c.GetRenderChild(i);
				child.Render(dc);
			}
			if (shouldUseOpacity)
				dc.Pop();
			if (shouldUseClipping)
				dc.Pop();
			if (shouldUseTransform)
				dc.Pop();
		}
		bool ShouldUseLayoutClip(FrameworkRenderElementContext context) {
			return (context.NeedsClipBounds || ClipToBounds) && context.LayoutClip != null;
		}
		Visibility CalcRenderVisibility(FrameworkRenderElementContext context) {
			Visibility visibility = CalcParentVisibility(context);
			Visibility currentVisibility = CalcVisibility(context);
			if (visibility == Visibility.Visible)
				return currentVisibility;
			if (visibility == Visibility.Hidden)
				return currentVisibility == Visibility.Visible ? Visibility.Hidden : currentVisibility;
			return currentVisibility;
		}
		Visibility CalcParentVisibility(FrameworkRenderElementContext context) {
			FrameworkRenderElementContext parent = context.Parent;
			while (parent != null) {
				var visibility = CalcVisibility(parent);
				if (visibility != Visibility.Visible)
					return visibility;
				parent = parent.Parent;
			}
			return Visibility.Visible;
		}
		Visibility CalcVisibility(FrameworkRenderElementContext context) {
			return context.Visibility ?? context.Factory.Visibility;
		}
		protected virtual Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			return availableSize;
		}
		protected virtual Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			return finalSize;
		}
		protected virtual void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			var frec = (FrameworkRenderElementContext)context;
			var showBounds = frec.ShowBounds ?? ShowBounds;
			if (showBounds)
				dc.DrawRectangle(null, new Pen(Brushes.Red, 1d), new Rect(new Point(0, 0), context.RenderSize));
		}
		protected internal FrameworkRenderElementContext CreateContext(INamescope namescope, IElementHost elementHost) {
			FreezeIfNeeded();
			var context = CreateContextInstance();
			context.Namescope = namescope;
			context.ElementHost = elementHost;
			context.Foreground = Foreground;
			if (FlowDirection.HasValue)
				context.FlowDirection = FlowDirection.Value;
			if (UseLayoutRounding.HasValue)
				context.UseLayoutRounding = UseLayoutRounding.Value;
			InitializeContext(context);
			namescope.RegisterElement(context);
			return context;
		}
		void FreezeIfNeeded() {
			if (isFreezed)
				return;
			lock (locker) {
				if (isFreezed)
					return;
				Name = string.IsNullOrEmpty(Name) ? Guid.NewGuid().ToString() : Name;
				isFreezed = true;
			}
		}
		protected virtual FrameworkRenderElementContext CreateContextInstance() {
			return new FrameworkRenderElementContext(this);
		}
		protected virtual void InitializeContext(FrameworkRenderElementContext context) {
		}
		protected void SetProperty<T>(ref T container, T value) {
			if (isFreezed)
				throw new ArgumentException("already frozen");
			if (object.Equals(container, value))
				return;
			container = value;
		}		
	}
}
