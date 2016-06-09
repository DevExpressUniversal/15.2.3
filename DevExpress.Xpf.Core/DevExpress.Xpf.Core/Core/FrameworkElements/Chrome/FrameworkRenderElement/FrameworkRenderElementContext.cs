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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public interface IFrameworkRenderElementContext {
		Size DesiredSize { get; }
		Size RenderSize { get; }
		Rect RenderRect { get; }
		Vector VisualOffset { get; }
		object DataContext { get; set; }
		int RenderChildrenCount { get; }
		void SetValue(string propertyName, object value);
		object GetValue(string propertyName);
		FrameworkRenderElementContext GetRenderChild(int index);
		void OnMouseDown(MouseRenderEventArgs args);
		void OnMouseUp(MouseRenderEventArgs args);
		void OnMouseMove(MouseRenderEventArgs args);
		void OnMouseEnter(MouseRenderEventArgs args);
		void OnMouseLeave(MouseRenderEventArgs args);
		void OnPreviewMouseDown(MouseRenderEventArgs args);
		void OnPreviewMouseUp(MouseRenderEventArgs args);
	}
	public class FrameworkRenderElementContext : IFrameworkRenderElementContext, ISupportInitialize {
		public static readonly int DataContextPropertyKey;
		public static readonly int ForegroundPropertyKey;
		public static readonly int IsEnabledPropertyKey;
		public static readonly int IsTouchEnabledPropertyKey;
		public static readonly int UseLayoutRoundingPropertyKey;
		public static readonly int FlowDirectionPropertyKey;
		protected internal FREInheritedProperties InheritedProperties;
		Visibility? visibility;
		double? opacity;
		Size renderSize;
		HorizontalAlignment? ha;
		Transform transform;
		Point? transformOrigin;
		VerticalAlignment? va;
		Thickness? margin;
		bool isMouseOverCore;
		bool? showBounds;
		double? minHeight, height, maxHeight, minWidth, width, maxWidth;
		readonly Locker isInSupportInitialize = new Locker();
		bool isMeasureValid = true;
		bool isArrangeValid = true;
		Dock? dock;
		static FrameworkRenderElementContext() {
			UseLayoutRoundingPropertyKey = FREInheritedProperties.Register(false, coerceValueCallback: (frec, v, vs) => frec.OnUseLayoutRoundingChangingCore((bool)v, vs));
			FlowDirectionPropertyKey = FREInheritedProperties.Register(FlowDirection.LeftToRight, (frec, ov, nv) => frec.OnFlowDirectionChanged((FlowDirection)ov, (FlowDirection)nv), (frec, v, vs) => frec.OnFlowDirectionChangingCore((FlowDirection)v, vs));
			IsTouchEnabledPropertyKey = FREInheritedProperties.Register(false, (frec, ov, nv) => frec.IsTouchEnabledChanged((bool)ov, (bool)nv), (frec, v, vs) => frec.OnIsTouchEnabledChangingCore((bool)v));
			IsEnabledPropertyKey = FREInheritedProperties.Register(true, (frec, ov, nv) => frec.OnIsEnabledChanged((bool)ov, (bool)nv), (frec, v, vs) => frec.OnIsEnabledChangingCore((bool)v));
			DataContextPropertyKey = FREInheritedProperties.Register(propertyChangedCallback: (frec, ov, nv) => frec.ResetTriggers());
			ForegroundPropertyKey = FREInheritedProperties.Register(propertyChangedCallback: (frec, ov, nv) => frec.OnForegroundChanged(ov, nv));
		}				
		public FlowDirection FlowDirection {
			get { return (FlowDirection)InheritedProperties.GetValue(FlowDirectionPropertyKey); }
			set { InheritedProperties.SetValue(FlowDirectionPropertyKey, value); }
		}
		public bool IsMouseOverCore {
			get { return isMouseOverCore; }
			set { SetProperty(ref isMouseOverCore, value, FREInvalidateOptions.None, OnIsMouseOverCoreChanged); }
		}
		public Transform Transform {
			get { return transform; }
			set { SetProperty(ref transform, value==null ? null : (Transform)value.GetAsFrozen(), FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public Point? TransformOrigin {
			get { return transformOrigin; }
			set { SetProperty(ref transformOrigin, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public Visibility? Visibility {
			get { return visibility; }
			set { SetProperty(ref visibility, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public double? Opacity {
			get { return opacity; }
			set { SetProperty(ref opacity, value, FREInvalidateOptions.AffectsChildrenCaches | FREInvalidateOptions.AffectsVisual | FREInvalidateOptions.AffectsOpacity); }
		}
		protected internal double ActualOpacity {
			get { return opacity ?? Factory.Opacity; }
		}
		public object DataContext {
			get { return InheritedProperties.GetValue(DataContextPropertyKey); }
			set { InheritedProperties.SetValue(DataContextPropertyKey, value); }
		}
		public Brush Foreground {
			get { return (Brush)InheritedProperties.GetValue(ForegroundPropertyKey); }
			set { InheritedProperties.SetValue(ForegroundPropertyKey, value); }
		}
		public bool IsEnabled {
			get { return (bool)InheritedProperties.GetValue(IsEnabledPropertyKey); }
			set { InheritedProperties.SetValue(IsEnabledPropertyKey, value); }
		}
		public bool IsTouchEnabled {
			get { return (bool)InheritedProperties.GetValue(IsTouchEnabledPropertyKey); }
			protected internal set { InheritedProperties.SetValue(IsTouchEnabledPropertyKey, value); }
		}
		public Dock? Dock {
			get { return dock; }
			set { SetProperty(ref dock, value); }
		}
		public HorizontalAlignment? HorizontalAlignment {
			get { return ha; }
			set { SetProperty(ref ha, value); }
		}
		public VerticalAlignment? VerticalAlignment {
			get { return va; }
			set { SetProperty(ref va, value); }
		}
		public Thickness? Margin {
			get { return margin; }
			set { SetProperty(ref margin, value); }
		}
		public bool? ShowBounds {
			get { return showBounds; }
			set { SetProperty(ref showBounds, value); }
		}
		public bool UseLayoutRounding {
			get { return (bool)InheritedProperties.GetValue(UseLayoutRoundingPropertyKey); }
			set { InheritedProperties.SetValue(UseLayoutRoundingPropertyKey, value); }
		}
		public double? MinHeight {
			get { return minHeight; }
			set { SetProperty(ref minHeight, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public double? Height {
			get { return height; }
			set { SetProperty(ref height, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public double? MaxHeight {
			get { return maxHeight; }
			set { SetProperty(ref maxHeight, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public double? MinWidth {
			get { return minWidth; }
			set { SetProperty(ref minWidth, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public double? Width {
			get { return width; }
			set { SetProperty(ref width, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public double? MaxWidth {
			get { return maxWidth; }
			set { SetProperty(ref maxWidth, value, FREInvalidateOptions.AffectsMeasureAndVisual); }
		}
		public bool IsArrangeValid { get { return isArrangeValid; } }
		public bool IsInSupportInitialize { get { return isInSupportInitialize.IsLocked; } }
		public virtual bool AttachToRoot { get { return false; } }
		public bool IsAttachedToRoot { get; set; }
		public Transform RenderTransform { get; protected set; }		
		public Geometry LayoutClip { get; internal set; }
		public string Name { get { return Factory.Name; } }
		public Size DesiredSize { get; internal set; }
		public Size? UnclippedDesiredSize { get; internal set; }
		public Size RenderSize {
			get { return renderSize; }
			set { SetProperty(ref renderSize, value, FREInvalidateOptions.None, RenderSizeChanged); }
		}
		public Vector VisualOffset { get; internal set; }
		public bool NeedsClipBounds { get; internal set; }
		public Rect RenderRect { get { return new Rect(VisualOffset.X, VisualOffset.Y, RenderSize.Width, RenderSize.Height); } }
		public FrameworkRenderElement Factory { get; private set; }
		public string ThemeInfo { get { return Factory.ThemeInfo; } }
		public FrameworkRenderElementContext(FrameworkRenderElement factory) {
			Factory = factory;
			InheritedProperties = new FREInheritedProperties(this);
		}
		public void PreApplyTemplate() {
			if (IsInSupportInitialize)
				return;
			SetIsMeasureValid();
			SetIsArrangeValid();
		}
		public void Measure(Size availableSize) {
			Factory.Measure(availableSize, this);
		}
		public void Arrange(Rect finalRect) {
			Factory.Arrange(finalRect, this);
		}
		public void Render(DrawingContext dc) {
			Factory.Render(dc, this);
		}
		public void InvalidateMeasure() {
			ElementHost.InvalidateMeasure();
		}
		public void InvalidateArrange() {
			ElementHost.InvalidateArrange();
		}
		public void InvalidateVisual() {
			ElementHost.InvalidateVisual();
		}		
		FrameworkRenderElementContext IFrameworkRenderElementContext.GetRenderChild(int index) {
			return GetRenderChild(index);
		}
		int IFrameworkRenderElementContext.RenderChildrenCount { get { return RenderChildrenCount; } }
		protected virtual int RenderChildrenCount { get { return 0; } }
		public INamescope Namescope { get; internal set; }
		public IElementHost ElementHost { get; internal set; }
		public FrameworkRenderElementContext Parent { get; private set; }
		protected virtual FrameworkRenderElementContext GetRenderChild(int index) {
			return null;
		}
		public virtual void AddChild(FrameworkRenderElementContext child) {
			child.Parent = this;
			child.InheritedProperties.CoerceValues();
		}
		public virtual void RemoveChild(FrameworkRenderElementContext child) {
			child.Release();
			child.Parent = null;
		}
		protected internal virtual void AttachToVisualTree(FrameworkElement root) {
		}
		protected internal virtual void DetachFromVisualTree(FrameworkElement root) {
		}		
		protected void SetProperty<TProperty>(ref TProperty container, TProperty value, FREInvalidateOptions invalidateOptions = FREInvalidateOptions.UpdateLayout, Action action = null, Action<TProperty, TProperty> parametrizedAction = null) {
			if (object.Equals(container, value))
				return;
			var oldValue = container;
			container = value;
			UpdateLayout(invalidateOptions);
			if (parametrizedAction != null)
				parametrizedAction(oldValue, value);
			action.Do(x => x());
		}
		public void UpdateLayout(FREInvalidateOptions invalidateOptions = (FREInvalidateOptions.AffectsMeasure | FREInvalidateOptions.AffectsVisual | FREInvalidateOptions.AffectsRenderCaches)) {
			if (invalidateOptions == FREInvalidateOptions.None)
				return;
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsMeasure))
				ElementHost.InvalidateMeasure();
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsArrange))
				ElementHost.InvalidateArrange();
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsVisual))
				ElementHost.InvalidateVisual();
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsRenderCaches)) {
				SetIsArrangeInvalid();
				ResetRenderCaches();
			}
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsGeneralCaches)) {
				SetIsMeasureInvalid();
				ResetTemplatesAndVisuals();
			}
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsChildrenCaches)) {
				for (int i = 0; i < RenderChildrenCount; i++) {
					var child = GetRenderChild(i);
					child.UpdateLayout(invalidateOptions);
				}
			}
			if (invalidateOptions.HasFlag(FREInvalidateOptions.AffectsOpacity)) {
				UpdateOpacity();
			}
		}
		public virtual bool ShouldUseTransform() {
			useMirrorTransform = ShouldUseMirrorTransform();
			return !VisualOffset.X.AreClose(0d) || !VisualOffset.Y.AreClose(0d) || useMirrorTransform || Transform!=null;
		}
		public virtual bool ShouldUseMirrorTransform() {
			var actualFd = FlowDirection;
			var parentFd = Parent == null ? GetChromeFlowDirectionIfRoot() : Parent.FlowDirection;
			return Parent != null && actualFd != parentFd;
		}
		protected virtual void UpdateOpacity() { }
		bool useMirrorTransform = false;
		public virtual void UpdateRenderTransform() {
			Transform transform = null;
			if (useMirrorTransform)
				transform = new MatrixTransform(new Matrix(-1, 0, 0, 1, RenderSize.Width + VisualOffset.X, VisualOffset.Y));
			else
				transform = new MatrixTransform(new Matrix(1, 0, 0, 1, VisualOffset.X, VisualOffset.Y));
			transform.Freeze();
			if (Transform != null) {
				TransformGroup group = new TransformGroup();				
				var origin = TransformOrigin ?? new Point();
				var hasOrigin = origin.X != 0d || origin.Y != 0d;
				if (hasOrigin) {
					TranslateTransform tt = new TranslateTransform(-RenderSize.Width * origin.X, -RenderSize.Height * origin.Y);
					tt.Freeze();
					group.Children.Add(tt);
				}
				group.Children.Add(Transform);
				if (hasOrigin) {
					TranslateTransform tt = new TranslateTransform(RenderSize.Width * origin.X, RenderSize.Height * origin.Y);
					tt.Freeze();
					group.Children.Add(tt);
				}
				group.Children.Add(transform);
				group.Freeze();
				transform = group;
			}
			RenderTransform = transform;
		}
		void ResetTriggers() {
			if (Namescope.Triggers == null)
				return;
			foreach (var trigger in Namescope.Triggers)
				trigger.Reset();
		}
		protected virtual void RenderSizeChanged() { }
		void ResetRenderCaches() {
			if (isInSupportInitialize)
				return;
			ResetRenderCachesInternal();
		}
		protected virtual void ResetRenderCachesInternal() {
		}
		void ResetTemplatesAndVisuals() {
			if (isInSupportInitialize)
				return;
			ResetTemplatesAndVisualsInternal();
		}
		protected virtual void ResetTemplatesAndVisualsInternal() {
			if (!AttachToRoot)
				Namescope.RemoveChild(this);
			else
				Namescope.AddChild(this);
		}
		protected internal virtual void ResetSizeSpecificCaches() {
			LayoutClip = null;
			RenderTransform = null;
		}
		public virtual void Release() {
			for (int i = 0; i < RenderChildrenCount; i++) {
				var child = GetRenderChild(i);
				child.Release();
			}
			Namescope.ReleaseElement(this);
			Namescope = null;
			ElementHost = null;
		}
		public void SetValue(string propertyName, object value) {
			SetValueOverride(propertyName, value);
		}
		readonly Locker resetValueLocker = new Locker();
		public void ResetValue(string propertyName) {
			resetValueLocker.DoLockedActionIfNotLocked(() => ResetValueOverride(propertyName));
		}
		public object GetValue(string propertyName) {
			return GetValueOverride(propertyName);
		}
		protected virtual void SetValueOverride(string propertyName, object value) {
			if (object.Equals(GetValue(propertyName), value))
				return;
			RenderTriggerHelper.SetConvertedValue(this, propertyName, value);
		}
		protected virtual void ResetValueOverride(string propertyName) {
			bool hasMatch = false;
			foreach (var trigger in Namescope.Triggers) {
				if (!trigger.Matches(this, propertyName))
					continue;
				if (trigger is SettableRenderTriggerContextBase && !((SettableRenderTriggerContextBase)trigger).IsValid())
					continue;
				hasMatch = true;
				trigger.Invalidate();
			}
			if (!hasMatch)
				ResetValueCore(propertyName);
		}
		protected virtual void ResetValueCore(string propertyName) {
			RenderTriggerHelper.SetConvertedValue(this, propertyName, null);
		}
		protected virtual object GetValueOverride(string propertyName) {
			return RenderTriggerHelper.GetValue(this, propertyName);
		}
		protected virtual void OnForegroundChanged(object oldValue, object newValue) { }
		bool OnIsEnabledChangingCore(bool value) {
			value = OnIsEnabledChanging(value);
			if (Parent != null) {
				if (!Parent.IsEnabled)
					value = false;
			}
			else {
				var chrome = ElementHost.Parent as IChrome;
				if (chrome != null) {
					if (Equals(this, chrome.Root) && !ElementHost.Parent.IsEnabled)
						value = false;
				}
			}
			return value;
		}
		private object OnIsTouchEnabledChangingCore(bool value) {
			value = OnIsTouchEnabledChanging(value);
			if (Parent != null) {
				if (Parent.IsTouchEnabled)
					value = true;
			} else {
				var chrome = ElementHost.Parent as IChrome;
				if (chrome != null) {
					if (Equals(this, chrome.Root)) {
						var walker = ThemeManager.GetTreeWalker(ElementHost.Parent);
						if (walker != null && walker.IsTouch)
							value = true;
					}						
				}
			}
			return value;		  
		}
		protected virtual bool OnIsTouchEnabledChanging(bool value) { return value; }
		protected virtual bool OnIsEnabledChanging(bool value) { return value; }
		protected virtual void OnIsEnabledChanged(bool oldValue, bool newValue) { }
		FlowDirection OnFlowDirectionChangingCore(FlowDirection value, FREInheritedPropertyValueSource valueSource) {
			value = OnFlowDirectionChanging(value);
			if (value == FlowDirection.LeftToRight && !valueSource.HasFlag(FREInheritedPropertyValueSource.Local) && Parent == null) {
				value = GetChromeFlowDirectionIfRoot();
			}
			return value;
		}
		object OnUseLayoutRoundingChangingCore(bool value, FREInheritedPropertyValueSource valueSource) {
			if (valueSource.HasFlag(FREInheritedPropertyValueSource.Local) || Parent != null)
				return value;
			var chrome = ElementHost.Parent as IChrome;
			if (chrome == null || !Equals(chrome.Root, this))
				return false;
			return ElementHost.Parent.UseLayoutRounding;
		}
		private FlowDirection GetChromeFlowDirectionIfRoot() {
			var chrome = ElementHost.Parent as IChrome;
			if (chrome != null) {
				if (Equals(this, chrome.Root) && ElementHost.Parent.FlowDirection == System.Windows.FlowDirection.RightToLeft)
					return System.Windows.FlowDirection.RightToLeft;
			}
			return FlowDirection.LeftToRight;
		}
		protected virtual FlowDirection OnFlowDirectionChanging(FlowDirection value) { return value; }
		protected virtual void OnFlowDirectionChanged(FlowDirection oldValue, FlowDirection newValue) {
			UpdateLayout(FREInvalidateOptions.AffectsVisual);
		}
		public void CoerceValue(int propertyKey) {
			InheritedProperties.CoerceValue(propertyKey);
		}
		public virtual bool HitTest(Point point) {
			return new Rect(RenderSize).Contains(point);
		}
		void IFrameworkRenderElementContext.OnMouseMove(MouseRenderEventArgs args) { OnMouseMove(args); }
		void IFrameworkRenderElementContext.OnMouseEnter(MouseRenderEventArgs args) { IsMouseOverCore = true; OnMouseEnter(args); }
		void IFrameworkRenderElementContext.OnMouseLeave(MouseRenderEventArgs args) { IsMouseOverCore = false; OnMouseLeave(args); }
		void IFrameworkRenderElementContext.OnMouseDown(MouseRenderEventArgs args) { OnMouseDown(args); }
		void IFrameworkRenderElementContext.OnMouseUp(MouseRenderEventArgs args) { OnMouseUp(args); }
		void IFrameworkRenderElementContext.OnPreviewMouseDown(MouseRenderEventArgs args) { OnPreviewMouseDown(args); }
		void IFrameworkRenderElementContext.OnPreviewMouseUp(MouseRenderEventArgs args) { OnPreviewMouseUp(args); }
		protected virtual void OnIsMouseOverCoreChanged() { }
		protected virtual void OnMouseMove(MouseRenderEventArgs args) { }
		protected virtual void OnMouseEnter(MouseRenderEventArgs args) { }
		protected virtual void OnMouseLeave(MouseRenderEventArgs args) { }
		protected virtual void OnMouseDown(MouseRenderEventArgs args) { }
		protected virtual void OnMouseUp(MouseRenderEventArgs args) { }
		protected virtual void OnPreviewMouseDown(MouseRenderEventArgs args) { }
		protected virtual void OnPreviewMouseUp(MouseRenderEventArgs args) { }
		public void BeginInit() {
			if (!isInSupportInitialize.IsLocked)
				BeginInitInternal();
			isInSupportInitialize.Lock();
			for (int i = 0; i < RenderChildrenCount; i++) {
				ISupportInitialize child = GetRenderChild(i);
				child.BeginInit();
			}
		}
		protected virtual void BeginInitInternal() {
		}
		public void EndInit() {
			isInSupportInitialize.Unlock();
			for (int i = 0; i < RenderChildrenCount; i++) {
				ISupportInitialize child = GetRenderChild(i);
				child.EndInit();
			}
			if (!isInSupportInitialize.IsLocked)
				EndInitInternal();
		}
		protected virtual void EndInitInternal() {
			if (!isMeasureValid)
				ResetTemplatesAndVisuals();
			if (!isArrangeValid)
				ResetRenderCaches();
		}
		void SetIsMeasureValid() {
			isMeasureValid = true;
		}
		void SetIsMeasureInvalid() {
			isMeasureValid = false;
		}
		void SetIsArrangeValid() {
			isArrangeValid = true;
		}
		void SetIsArrangeInvalid() {
			isArrangeValid = false;
		}
		protected virtual void IsTouchEnabledChanged(bool oldValue, bool newValue) {
			UpdateTouchState(Namescope, newValue);
		}
		protected void UpdateTouchState(INamescope scope, bool newValue) {
			scope.GoToState(newValue ? "Touch" : "NonTouch");
		}
	}
}
