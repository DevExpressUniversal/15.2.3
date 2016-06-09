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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Core.Native {
	public interface IChrome {
		void AddChild(FrameworkElement element);
		void RemoveChild(FrameworkElement element);		
		FrameworkRenderElementContext Root { get; }
		void InvalidateMeasure();
		void InvalidateArrange();
		void InvalidateVisual();
		void GoToState(string stateName);
	}
	public class Chrome : ContentPresenter, IChrome, IElementOwner {
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty FontSizeProperty;
		public static readonly DependencyProperty FontFamilyProperty;
		public static readonly DependencyProperty FontStretchProperty;
		public static readonly DependencyProperty FontStyleProperty;
		public static readonly DependencyProperty FontWeightProperty;
		static readonly DataTemplate emptyTemplate;
		static Chrome() {
			Type ownerType = typeof(Chrome);
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			var brush = new SolidColorBrush(Colors.Black);
			brush.Freeze();
			emptyTemplate = Net45Detector.IsNet45() ? null : new DataTemplate().Do(x => x.Seal());
			ForegroundProperty = Control.ForegroundProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(brush, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).ForegroundChanged((Brush)args.NewValue)));
			FontSizeProperty = Control.FontSizeProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(Control.FontSizeProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).UpdateSubTree()));
			FontFamilyProperty = Control.FontFamilyProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(Control.FontFamilyProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).UpdateSubTree()));
			FontStretchProperty = Control.FontStretchProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(Control.FontStretchProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).UpdateSubTree()));
			FontStyleProperty = Control.FontStyleProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(Control.FontStyleProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).UpdateSubTree()));
			FontWeightProperty = Control.FontWeightProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(Control.FontWeightProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, (o, args) => ((Chrome)o).UpdateSubTree()));			
			RecognizesAccessKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((Chrome)d).OnRecognizesAccessKeyChanged((bool)e.OldValue, (bool)e.NewValue)));
			ContentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((Chrome)d).OnContentChanged(e.OldValue, e.NewValue)));
			FlowDirectionProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(FlowDirection.LeftToRight, ChromeSlave.GetDefaultOptions(FlowDirectionProperty), (o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.FlowDirectionPropertyKey)));
			UseLayoutRoundingProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false, ChromeSlave.GetDefaultOptions(UseLayoutRoundingProperty), (o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.UseLayoutRoundingPropertyKey)));
			ThemeManager.TreeWalkerProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, ChromeSlave.GetDefaultOptions(ThemeManager.TreeWalkerProperty), (o, args) => ((Chrome)o).InvalidateContext()));
		}
		readonly ChromeSlave slave;
		object renderContent;
		RenderTemplate renderTemplate;
		RenderTemplateSelector renderTemplateSelector = RenderTemplateSelector.Default;
		bool recognizesAccessKey = true;
		object content = null;
		string contentStringFormat = null;				
		public new object Content {
			get { return content; }
			set { base.Content = value; }
		}
		public new bool RecognizesAccessKey {
			get { return recognizesAccessKey; }
			set { base.RecognizesAccessKey = value; }
		}
		public new string ContentStringFormat {
			get { return contentStringFormat; }
			set { base.ContentStringFormat = value; }
		}				
		public Chrome() {			
			slave = new ChromeSlave(this, true, InitializeContext, ReleaseContext);
		}		
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
		public FontStretch FontStretch {
			get { return (FontStretch)GetValue(FontStretchProperty); }
			set { SetValue(FontStretchProperty, value); }
		}
		public FontStyle FontStyle {
			get { return (FontStyle)GetValue(FontStyleProperty); }
			set { SetValue(FontStyleProperty, value); }
		}
		public FontWeight FontWeight {
			get { return (FontWeight)GetValue(FontWeightProperty); }
			set { SetValue(FontWeightProperty, value); }
		}
		public RenderTemplate RenderTemplate {
			get { return renderTemplate; }
			set {
				renderTemplate = value;
				InvalidateContext();
			}
		}
		public RenderTemplateSelector RenderTemplateSelector {
			get { return renderTemplateSelector; }
			set {
				if (renderTemplateSelector == value)
					return;
				renderTemplateSelector = value ?? RenderTemplateSelector.Default;
				InvalidateContext();
			}
		}		
		public object RenderContent {
			get { return renderContent; }
			set {
				object oldContent = renderContent;
				renderContent = value;
				if (!object.Equals(oldContent, renderContent))
					InvalidateContext();
				RenderContentChanged(oldContent, renderContent);
			}
		}
		protected virtual void RenderContentChanged(object oldContent, object newContent) {
			InvalidateMeasureAndVisual();
		}		
		protected internal void InvalidateMeasureAndVisual() {
			InvalidateMeasure();
			InvalidateVisual();
		}		
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, ElementHost.LogicalChildren); } }
		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + ElementHost.ChildrenCount; } }
		public Visual RealChild { get { return base.VisualChildrenCount > 0 ? GetVisualChild(0) : null; } }
		protected override Visual GetVisualChild(int index) {
			int childrenCount = base.VisualChildrenCount;
			if (childrenCount == 0)
				return ElementHost.GetChild(index);
			return index < childrenCount ? base.GetVisualChild(index) : ElementHost.GetChild(index - childrenCount);
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (visualAdded != null && visualAdded == RealChild)
				InvalidateContext();
		}
		protected virtual void ForegroundChanged(Brush newValue) {
			UpdateSubTree();
		}		
		bool? useDefaultStringTemplate;
#if DEBUGTEST
		public static bool? forcedUseDefaultStringTemplate;
#endif
		protected internal bool UseDefaultStringTemplate {
			get {
#if DEBUGTEST
				if (forcedUseDefaultStringTemplate.HasValue)
					return forcedUseDefaultStringTemplate.Value;
#endif
				return (useDefaultStringTemplate ?? (useDefaultStringTemplate =
				  !String.IsNullOrEmpty(ContentStringFormat)
				  || RecognizesAccessKey && (Content as string).If(x => x.IndexOf('_') > -1).ReturnSuccess())).Value;
			}
		}
		protected virtual void UpdateInnerPresenters() {
			useDefaultStringTemplate = null;
			UpdateSubTree();
		}
		protected virtual void UpdateSubTree() {
			slave.UpdateSubTree();
		}
		protected void InvalidateContext() {
			slave.InvalidateContext();
		}
		public INamescope Namescope { get { return slave.Namescope; } }
		public IElementHost ElementHost { get { return slave.ElementHost; } }
		FrameworkRenderElementContext IChrome.Root { get { return slave.Root; } }		
		void IChrome.GoToState(string stateName) { slave.GoToState(stateName); }
		void IChrome.AddChild(FrameworkElement element) {
			AddLogicalChild(element);
			AddVisualChild(element);
		}
		void IChrome.RemoveChild(FrameworkElement element) {
			RemoveLogicalChild(element);
			RemoveVisualChild(element);
		}
		protected virtual FrameworkRenderElementContext InitializeContext() { return slave.CreateContext(RenderTemplateSelector.SelectTemplate(this, RenderContent)); }
		protected virtual void ReleaseContext(FrameworkRenderElementContext context) { slave.ReleaseContext(context); }
		protected override Size MeasureOverride(Size availableSize) {
			return slave.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return slave.ArrangeOverride(finalSize);
		}
		protected override void OnRender(DrawingContext dc) {
			slave.OnRender(dc);
		}
		protected override void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat) { contentStringFormat = newContentStringFormat; UpdateInnerPresenters(); }
		protected virtual void OnRecognizesAccessKeyChanged(bool oldValue, bool newValue) { recognizesAccessKey = newValue; UpdateInnerPresenters(); }
		protected virtual void OnContentChanged(object oldValue, object newValue) { content = newValue; UpdateInnerPresenters(); }
		protected override DataTemplate ChooseTemplate() {
			return emptyTemplate;
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		FrameworkElement IElementOwner.Child { get { return null; }}
	}	
	public class ChromeSlave {		
		public static void CoerceValue(IChrome chrome, int propertyKey) {
			if (chrome.Root == null)
				return;
			chrome.Root.CoerceValue(propertyKey);
		}
		public static FrameworkPropertyMetadataOptions GetDefaultOptions(DependencyProperty property) {
			var result = FrameworkPropertyMetadataOptions.None;
			var defaultMetadata = property.DefaultMetadata as FrameworkPropertyMetadata;
			if (defaultMetadata == null)
				return result;
			if (defaultMetadata.AffectsMeasure) {
				result |= FrameworkPropertyMetadataOptions.AffectsMeasure;
			}
			if (defaultMetadata.AffectsArrange) {
				result |= FrameworkPropertyMetadataOptions.AffectsArrange;
			}
			if (defaultMetadata.AffectsParentMeasure) {
				result |= FrameworkPropertyMetadataOptions.AffectsParentMeasure;
			}
			if (defaultMetadata.AffectsParentArrange) {
				result |= FrameworkPropertyMetadataOptions.AffectsParentArrange;
			}
			if (defaultMetadata.AffectsRender) {
				result |= FrameworkPropertyMetadataOptions.AffectsRender;
			}
			if (defaultMetadata.Inherits) {
				result |= FrameworkPropertyMetadataOptions.Inherits;
			}
			if (defaultMetadata.OverridesInheritanceBehavior) {
				result |= FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior;
			}
			if (defaultMetadata.IsNotDataBindable) {
				result |= FrameworkPropertyMetadataOptions.NotDataBindable;
			}
			if (defaultMetadata.BindsTwoWayByDefault) {
				result |= FrameworkPropertyMetadataOptions.BindsTwoWayByDefault;
			}
			if (defaultMetadata.Journal) {
				result |= FrameworkPropertyMetadataOptions.Journal;
			}
			if (defaultMetadata.SubPropertiesDoNotAffectRender) {
				result |= FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender;
			}
			return result;
		}
#region fields\props
		readonly FrameworkElement feOwner;
		readonly IChrome iChromeOwner;
		readonly Namescope namescope;
		bool contextUnloaded;
		bool contextInvalid;
		FrameworkRenderElementContext context;
		FrameworkElement FrameworkElement { get { return feOwner; } }
		IChrome IChrome { get { return iChromeOwner; } }
		readonly Func<FrameworkRenderElementContext> initializeContext;
		readonly Action<FrameworkRenderElementContext> releaseContext;
		readonly Func<bool> validateTemplate;
		readonly Func<Size, Size> baseMeasure;
		readonly Func<Size, Size> baseArrange;
		readonly bool destroyContextOnUnloaded;
		RenderTemplate template;
		public INamescope Namescope { get { return namescope; } }
		public IElementHost ElementHost { get { return namescope; } }
		public FrameworkRenderElementContext Root { get { return context; } }	 
		public RenderTemplate Template { get { return template; } }
		bool? isEnabled;
		public bool? IsEnabled {
			get { return isEnabled; }
			set {
				if (isEnabled == value)
					return;
				isEnabled = value;
				OnIsEnabledChanged();
			}
		}
		protected virtual void OnIsEnabledChanged() {
			if (baseMeasure == null || baseArrange == null)
				throw new InvalidOperationException();
			if (!IsEnabled == false) {
				feOwner.SetBypassLayoutPolicies(false);
			}
			if (IsEnabled == true)
				feOwner.SetBypassLayoutPolicies(true);
			DestroyContext();
			contextUnloaded = true;
			feOwner.InvalidateMeasure();
		}
		#endregion
		public ChromeSlave(IChrome owner, bool propagateMouseEvents, Func<FrameworkRenderElementContext> initializeContext, Action<FrameworkRenderElementContext> releaseContext, 
			Func<bool> validateTemplate = null, Func<Size, Size> baseMeasure = null, Func<Size, Size> baseArrange = null, bool destroyContextOnUnloaded = true) {
			this.feOwner = owner as FrameworkElement;
			this.iChromeOwner = owner;
			this.namescope = new Namescope(owner);
			this.initializeContext = initializeContext;
			this.releaseContext = releaseContext;
			this.validateTemplate = validateTemplate;
			this.baseMeasure = baseMeasure;
			this.baseArrange = baseArrange;
			this.destroyContextOnUnloaded = destroyContextOnUnloaded;
			SubscribeEvents();
			if (propagateMouseEvents)
				SubscribeMouseEvents();
		}
#region events
		void SubscribeEvents() {
			FrameworkElement.Unloaded += ChromeUnloaded;
			FrameworkElement.Loaded += ChromeLoaded;
			FrameworkElement.IsEnabledChanged += ChromeIsEnabledChanged;
		}
		void ChromeLoaded(object sender, RoutedEventArgs e) {
			if (contextUnloaded)
				InvalidateContext();
		}
		void ChromeUnloaded(object sender, RoutedEventArgs e) {
			if (destroyContextOnUnloaded)
				DestroyContext();
			else
				InvalidateContext();
			contextUnloaded = true;
		}
		void ChromeIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CoerceValue(IChrome, FrameworkRenderElementContext.IsEnabledPropertyKey);
		}
#endregion
#region mouse events
		void SubscribeMouseEvents() {
			FrameworkElement.MouseDown += OnMouseDown;
			FrameworkElement.MouseUp += OnMouseUp;
			FrameworkElement.PreviewMouseDown += OnPreviewMouseDown;
			FrameworkElement.PreviewMouseUp += OnPreviewMouseUp;
			FrameworkElement.MouseEnter += OnMouseEnter;
			FrameworkElement.MouseLeave += OnMouseLeave;
			FrameworkElement.MouseMove += OnMouseMove;
		}
		void OnMouseDown(object sender, MouseButtonEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.MouseDown); }
		void OnMouseUp(object sender, MouseButtonEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.MouseUp); }
		void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.PreviewMouseDown); }
		void OnPreviewMouseUp(object sender, MouseButtonEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.PreviewMouseUp); }
		void OnMouseEnter(object sender, MouseEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.MouseEnter); }
		void OnMouseLeave(object sender, MouseEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.MouseLeave); }
		void OnMouseMove(object sender, MouseEventArgs e) { IChrome.PropagateEvent(e, RenderEvents.MouseMove); }
#endregion
		public void InvalidateContext() {
			if (validateTemplate != null)
				contextInvalid = true;
			else
				DestroyContext();
			InvalidateMeasureAndVisual();
		}
		public void GoToState(string stateName) {
			((Namescope)Namescope).GoToState(stateName);
		}
		public virtual void UpdateSubTree() {
			context.Do(x => x.UpdateLayout(FREInvalidateOptions.UpdateLayout | FREInvalidateOptions.AffectsChildrenCaches));
		}
		public Size MeasureOverride(Size availableSize) {
			if (IsEnabled == false)
				return baseMeasure(availableSize);
			if (contextInvalid && validateTemplate != null) {
				if (!validateTemplate())
					DestroyContext();
			}
			contextInvalid = false;
			if (Context == null)
				return new Size(0, 0);
			Context.Measure(availableSize);
			return Context.DesiredSize;
		}
		public Size ArrangeOverride(Size finalSize) {
			if (IsEnabled == false)
				return baseArrange(finalSize);
			if (context == null)
				return new Size(0, 0);
			context.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			return finalSize;
		}
		public void OnRender(DrawingContext dc) {
			if (context == null) {
				if (IsEnabled != false)
					InvalidateMeasureAndVisual();
				return;
			}
			context.Render(dc);
		}		
		FrameworkRenderElementContext CreateContext() {
			if (context != null)
				releaseContext(context);
			return initializeContext();					   
		}
		public FrameworkRenderElementContext CreateContext(RenderTemplate template) {
			this.template = template;
			var result = template.With(x => x.CreateContext(Namescope, ElementHost, ElementHost.TemplatedParent ?? ElementHost.Parent));
			SetRoot(result);
			template.Do(x => x.InitializeTemplate(context, Namescope, ElementHost, namescope));
			return result;
		}
		public void SetRoot(FrameworkRenderElementContext context) {
			((Namescope)Namescope).RootElement = context;
		}		
		public void ReleaseContext(FrameworkRenderElementContext context) {
			context.Do(x => x.Release());
			SetRoot(null);
			Template.Do(x => x.ReleaseTemplate(context, Namescope, ElementHost));
			template = null;
		}
		void DestroyContext() {
			if (context != null)
				releaseContext(context);
			context = null;
		}
		void CoerceValues(FrameworkRenderElementContext c) {
			if (FrameworkElement.FlowDirection == FlowDirection.RightToLeft)
				c.CoerceValue(FrameworkRenderElementContext.FlowDirectionPropertyKey);
			if (!FrameworkElement.IsEnabled)
				c.CoerceValue(FrameworkRenderElementContext.IsEnabledPropertyKey);
			if (GetIsTouchEnabled(FrameworkElement))
				c.CoerceValue(FrameworkRenderElementContext.IsTouchEnabledPropertyKey);
			if (FrameworkElement.UseLayoutRounding)
				c.CoerceValue(FrameworkRenderElementContext.UseLayoutRoundingPropertyKey);
		}
		bool GetIsTouchEnabled(FrameworkElement frameworkElement) {
			var walker = ThemeManager.GetTreeWalker(frameworkElement);
			if (walker == null)
				return false;
			return walker.IsTouch;
		}
		void InvalidateMeasureAndVisual() {
			FrameworkElement.InvalidateMeasure();
			FrameworkElement.InvalidateVisual();
		}
		FrameworkRenderElementContext Context { get { return context ?? (context = CreateContext()).Do(CoerceValues); } }					
	}
	public class MergedEnumerator : IEnumerator {
		readonly IEnumerator<IEnumerator> sourceEnumerator;
		public MergedEnumerator(params IEnumerator[] args) {
			sourceEnumerator = args.Where(x => x != null).ToList().GetEnumerator();
		}
		public object Current {
			get { return sourceEnumerator.Current.With(x => x.Current); }
		}
		public void Dispose() {
			sourceEnumerator.Dispose();
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (sourceEnumerator.Current == null)
				sourceEnumerator.MoveNext();
			if (sourceEnumerator.Current == null)
				return false;
			if (!sourceEnumerator.Current.MoveNext()) {
				sourceEnumerator.MoveNext();
				if (sourceEnumerator.Current == null)
					return false;
			}
			else
				return true;
			return sourceEnumerator.Current.MoveNext();
		}
		public void Reset() {
			sourceEnumerator.Reset();
			while (sourceEnumerator.MoveNext())
				sourceEnumerator.Current.Reset();
			sourceEnumerator.Reset();
		}
	}
	public class MergedEnumerator<T> : IEnumerator<T> {
		readonly IEnumerator<IEnumerator<T>> sourceEnumerator;
		public MergedEnumerator(params IEnumerator<T>[] args) {
			sourceEnumerator = args.Where(x => x != null).ToList().GetEnumerator();
		}
		public T Current {
			get { return sourceEnumerator.Current.Current; }
		}
		public void Dispose() {
			sourceEnumerator.Dispose();
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (sourceEnumerator.Current == null)
				sourceEnumerator.MoveNext();
			if (sourceEnumerator.Current == null)
				return false;
			if (!sourceEnumerator.Current.MoveNext()) {
				sourceEnumerator.MoveNext();
				if (sourceEnumerator.Current == null)
					return false;
			}
			else
				return true;
			return sourceEnumerator.Current.MoveNext();
		}
		public void Reset() {
			sourceEnumerator.Reset();
			while (sourceEnumerator.MoveNext())
				sourceEnumerator.Current.Reset();
			sourceEnumerator.Reset();
		}
	}
}
