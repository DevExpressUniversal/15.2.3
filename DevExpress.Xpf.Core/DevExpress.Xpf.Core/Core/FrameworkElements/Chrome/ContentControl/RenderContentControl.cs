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
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class RenderControl : FrameworkRenderElement {
		static readonly RenderDecorator CpTemplate = new RenderDecorator();
#if DEBUGTEST
		[DevExpress.Xpf.Editors.Tests.IgnoreFREChecker]
#endif
		public bool UsePropagatedIsMouseOver { get; set; }
		public RenderTemplate RenderTemplate { get; set; }
		public RenderTemplateSelector RenderTemplateSelector { get; set; }
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderControlContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			var ccContext = (RenderControlContext)context;
			var innerNamescope = new Namescope((Namescope)context.Namescope);
			innerNamescope.RootElement = context;
			ccContext.InnerNamescope = innerNamescope;
		}
		protected override void PreApplyTemplate(FrameworkRenderElementContext context) {
			base.PreApplyTemplate(context);
			var ccContext = (RenderControlContext)context;
			if (ccContext.Context != null)
				return;
			var tSelector = ccContext.RenderTemplateSelector ?? RenderTemplateSelector;
			var template = ccContext.RenderTemplate ?? RenderTemplate;
			var renderTemplate = tSelector != null ? tSelector.SelectTemplate(ccContext.ElementHost.Parent, ccContext) : template;
			ccContext.ActualRenderTemplate = renderTemplate;
			if (renderTemplate == null)
				return;
			var innerNamescope = ccContext.InnerNamescope;
			var cpContext = (RenderDecoratorContext)CpTemplate.CreateContext(innerNamescope, innerNamescope);
			ccContext.AddChild(cpContext);
			var inner = ccContext.ActualRenderTemplate.CreateContext(innerNamescope, innerNamescope, ccContext.DataContext);
			ccContext.ActualRenderTemplate.InitializeTemplate(inner, innerNamescope, innerNamescope, innerNamescope);
			cpContext.AddChild(inner);
			ccContext.UpdateStates();
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			return LayoutProvider.GridInstance.MeasureOverride(availableSize, context);
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			return LayoutProvider.GridInstance.ArrangeOverride(finalSize, context);
		}
	}
	[ContentProperty("Content")]
	public class RenderContentControl : RenderControl {
		public const string ContentPresenterName = "PART_ContentPresenter";
		protected static readonly RenderTemplate defaultTemplate;
		RenderTemplateSelector renderContentTemplateSelector;
		RenderTemplate renderContentTemplate;
		HorizontalAlignment hca;
		VerticalAlignment vca;
		Thickness padding;
		DataTemplate contentTemplate;
		DataTemplateSelector contentTemplateSelector;
		bool preferRenderTemplate = true;
		public bool PreferRenderTemplate {
			get { return preferRenderTemplate; }
			set { SetProperty(ref preferRenderTemplate, value); }
		}
		public RenderTemplateSelector RenderContentTemplateSelector {
			get { return renderContentTemplateSelector; }
			set { SetProperty(ref renderContentTemplateSelector, value); }
		}
		public RenderTemplate RenderContentTemplate {
			get { return renderContentTemplate; }
			set { SetProperty(ref renderContentTemplate, value); }
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set { SetProperty(ref contentTemplate, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return contentTemplateSelector; }
			set { SetProperty(ref contentTemplateSelector, value); }
		}
		public HorizontalAlignment HorizontalContentAlignment {
			get { return hca; }
			set { SetProperty(ref hca, value); }
		}
		public VerticalAlignment VerticalContentAlignment {
			get { return vca; }
			set { SetProperty(ref vca, value); }
		}
		public Thickness Padding {
			get { return padding; }
			set { SetProperty(ref padding, value); }
		}
		public object Content { get; set; }
		static RenderContentControl() {
			defaultTemplate = new RenderTemplate() { RenderTree = new RenderContentPresenter() { Name = RenderContentControl.ContentPresenterName } };
		}
		public RenderContentControl() {
			RenderTemplate = defaultTemplate;
			VerticalContentAlignment = VerticalAlignment.Center;
			HorizontalContentAlignment = HorizontalAlignment.Center;
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderContentControlContext(this) { };
		}		
		protected override void PreApplyTemplate(FrameworkRenderElementContext context) {
			var ccContext = (RenderContentControlContext)context;
			bool assignValues = ((RenderControlContext)context).Context == null;
			base.PreApplyTemplate(context);
			if (!assignValues)
				return;
			var cpContext = GetContentPresenter(ccContext);
			if (cpContext != null) {
				cpContext.VerticalAlignment = ccContext.VerticalContentAlignment ?? VerticalContentAlignment;
				cpContext.HorizontalAlignment = ccContext.HorizontalContentAlignment ?? HorizontalContentAlignment;
				cpContext.Content = ccContext.Content ?? Content;
				cpContext.RenderTemplate = RenderContentTemplate;
				cpContext.RenderTemplateSelector = RenderContentTemplateSelector;
				cpContext.ContentTemplate = ccContext.ContentTemplate ?? ContentTemplate;
				cpContext.ContentTemplateSelector = ccContext.ContentTemplateSelector ?? ContentTemplateSelector;
				cpContext.PreferRenderTemplate = ccContext.PreferRenderTemplate ?? PreferRenderTemplate;
			}
			var paddingTargetContext = GetPaddingTarget(ccContext);
			if (paddingTargetContext != null) {
				paddingTargetContext.Margin = ccContext.Padding.HasValue ? ccContext.Padding.Value : Padding;
			}
		}
		protected RenderContentPresenterContext GetContentPresenter(RenderControlContext context) {
			return (RenderContentPresenterContext)context.InnerNamescope.GetElement(RenderContentControl.ContentPresenterName);
		}
		protected virtual FrameworkRenderElementContext GetPaddingTarget(RenderControlContext context) {
			return GetContentPresenter(context);
		}
	}
	public class RenderContentPresenter : RenderDecorator {
		bool allowVisualTree = true;
		bool preferRenderTemplate = true;
		public bool PreferRenderTemplate {
			get { return preferRenderTemplate; }
			set { SetProperty(ref preferRenderTemplate, value); }
		}
		public bool AllowVisualTree {
			get { return allowVisualTree; }
			set { SetProperty(ref allowVisualTree, value); }
		}
		public RenderContentPresenter() { }
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderContentPresenterContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			var ccContext = (RenderContentPresenterContext)context;
			var innerNamescope = new Namescope((Namescope)context.Namescope);
			innerNamescope.RootElement = context;
			ccContext.InnerNamescope = innerNamescope;
		}
		protected override void PreApplyTemplate(FrameworkRenderElementContext context) {
			var cpContext = (RenderContentPresenterContext)context;
			var assignValues = cpContext.Context == null;				
			base.PreApplyTemplate(context);
			if (!assignValues)
				return;
			bool useTemplate;
			DataTemplate dTemplate;
			bool? forceVisual;
			cpContext.ActualRenderTemplate = GetActualRenderTemplate(cpContext, out useTemplate, out forceVisual, out dTemplate);
			cpContext.DefaultContentTemplate = dTemplate;
			if (cpContext.ActualRenderTemplate == null) {
				if (useTemplate)
					return;
				var frec = cpContext.Content as FrameworkRenderElementContext;
				if (frec == null) {
					var fre = cpContext.Content as FrameworkRenderElement;
					if (fre != null)
						frec = fre.CreateContext(cpContext.InnerNamescope, cpContext.InnerNamescope);
				} if (frec != null) {
					cpContext.AddChild(frec);					
				}
				return;
			}
			if (cpContext.InnerNamescope.GetElement(RenderContentControl.ContentPresenterName) as RenderRealContentPresenterContext != null)
				cpContext.HasRealPresenter = true;
			var innerNamescope = cpContext.InnerNamescope;
			var inner = cpContext.ActualRenderTemplate.CreateContext(innerNamescope, innerNamescope, cpContext.Content);
			cpContext.ActualRenderTemplate.InitializeTemplate(inner, innerNamescope, innerNamescope, innerNamescope);
			cpContext.AddChild(inner);
			cpContext.PropagateProperties();
		}
		public bool ShouldUpdateTemplate(RenderContentPresenterContext context, object oldValue, object newValue) {
			if (context.InnerNamescope == null)
				return true;
			bool useTemplate;
			bool? forceVisual;
			DataTemplate defaultContentTemplate;
			RenderTemplate actualRenderTemplate;
			actualRenderTemplate = GetActualRenderTemplate(context, out useTemplate, out forceVisual, out defaultContentTemplate);
			if (object.ReferenceEquals(oldValue, newValue) || forceVisual == true && context.DefaultContentTemplate == defaultContentTemplate && context.ActualRenderTemplate == actualRenderTemplate)
				return false;
			return true;
		}
		protected virtual RenderTemplate GetActualRenderTemplate(RenderContentPresenterContext cpContext, out bool useTemplate, out bool? forceVisual, out DataTemplate dataTemplate) {
			dataTemplate = null;
			forceVisual = null;
			useTemplate = cpContext.RenderTemplate != null || cpContext.RenderTemplateSelector != null;
			var useVisualTemplate = cpContext.ContentTemplate != null || cpContext.ContentTemplateSelector != null;
			var chrome = (IChrome)cpContext.ElementHost.Parent;
			var content = cpContext.Content;			
			var renderTemplate = cpContext.RenderTemplateSelector != null ? cpContext.RenderTemplateSelector.SelectTemplate((FrameworkElement)chrome, content) : cpContext.RenderTemplate;
			if (useVisualTemplate) {
				dataTemplate = cpContext.ContentTemplate;
				if (dataTemplate == null && cpContext.ContentTemplateSelector != null)
					dataTemplate = cpContext.ContentTemplateSelector.SelectTemplate(content, chrome as DependencyObject);
				if (dataTemplate != null && !(cpContext.PreferRenderTemplate ?? PreferRenderTemplate))
					useTemplate = false;
			}
			if (!useTemplate) {
				var allowTree = cpContext.AllowVisualTree.HasValue ? cpContext.AllowVisualTree.Value : AllowVisualTree;
				if (useVisualTemplate)
					forceVisual = true;
				else
					forceVisual = allowTree ? (bool?)null : (bool?)false;
				return DefaultRenderTemplateSelector.SelectTemplate((FrameworkElement)chrome, content, forceVisual, ref dataTemplate);
			}			
			dataTemplate = null;
			return renderTemplate;
		}		
	}
	public class RenderContentPresenterContext : RenderDecoratorContext {
		RenderTemplate actualRenderTemplate;
		RenderTemplate renderTemplate;
		RenderTemplateSelector renderTemplateSelector;
		DataTemplate contentTemplate;
		DataTemplate defaultContentTemplate;
		DataTemplateSelector contentTemplateSelector;
		bool hasRealPresenter;
		bool? allowVisualTree;
		bool? preferRenderTemplate;
		object content;		
		protected override int RenderChildrenCount { get { return Context != null ? 1 : 0; } }
		protected override FrameworkRenderElementContext GetRenderChild(int index) {
			return Context;
		}
		public new RenderContentPresenter Factory { get { return (RenderContentPresenter)base.Factory; } }
		public RenderRealContentPresenterContext ContentPresenter {
			get { return (RenderRealContentPresenterContext)InnerNamescope.GetElement(RenderContentControl.ContentPresenterName); }
		}
		public bool? PreferRenderTemplate {
			get { return preferRenderTemplate; }
			set { SetProperty(ref preferRenderTemplate, value, FREInvalidateOptions.UpdateLayout); }
		}
		public bool? AllowVisualTree {
			get { return allowVisualTree; }
			set { SetProperty(ref allowVisualTree, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public RenderTemplate ActualRenderTemplate { 
			get{ return actualRenderTemplate;}
			set { SetProperty(ref actualRenderTemplate, value, FREInvalidateOptions.None); }
		}
		public object Content {
			get { return content; }
			set { SetProperty(ref content, value, FREInvalidateOptions.None, null, ContentChanged); }
		}		
		public RenderTemplateSelector RenderTemplateSelector {
			get { return renderTemplateSelector; }
			set { SetProperty(ref renderTemplateSelector, value, FREInvalidateOptions.UpdateSubTree); }
		}		
		public RenderTemplate RenderTemplate {
			get { return renderTemplate; }
			set { SetProperty(ref renderTemplate, value, FREInvalidateOptions.UpdateSubTree); }
		}		
		public DataTemplateSelector ContentTemplateSelector {
			get { return contentTemplateSelector; }
			set { SetProperty(ref contentTemplateSelector, value, HasRealPresenter ? FREInvalidateOptions.None : FREInvalidateOptions.UpdateSubTree, ContentTemplateSelectorChanged); }
		}
		protected internal DataTemplate DefaultContentTemplate {
			get { return defaultContentTemplate; }
			set { SetProperty(ref defaultContentTemplate, value, FREInvalidateOptions.None, ContentTemplateChanged); }
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set { SetProperty(ref contentTemplate, value, HasRealPresenter ? FREInvalidateOptions.None : FREInvalidateOptions.UpdateSubTree, ContentTemplateChanged); }
		}
		public DataTemplate ActualContentTemplate {
			get { return ContentTemplate ?? DefaultContentTemplate; }
		}
		public bool HasRealPresenter {
			get { return hasRealPresenter; }
			set { SetProperty(ref hasRealPresenter, value, FREInvalidateOptions.None); }
		}				
		public FrameworkRenderElementContext Context { get; private set; }
		public Namescope InnerNamescope { get; internal set; }
		public RenderContentPresenterContext(RenderContentPresenter factory)
			: base(factory) {
		}
		public bool ShouldUpdateTemplate(object oldValue, object newValue) {
			return Factory.ShouldUpdateTemplate(this, oldValue, newValue);
		}
		protected override void ResetTemplatesAndVisualsInternal() {			
			base.ResetTemplatesAndVisualsInternal();
			var freContext = Context;
			freContext.Do(x => x.Release());
			Context = null;
			ActualRenderTemplate.Do(x => x.ReleaseTemplate(this, InnerNamescope, InnerNamescope));
			ActualRenderTemplate = null;
		}
		public override void Release() {
			ResetTemplatesAndVisualsInternal();
			base.Release();			
		}
		public override void AddChild(FrameworkRenderElementContext child) {
			base.AddChild(child);
			Context = child;
		}
		public virtual void PropagateProperties() {			
			ContentChanged(Content, Content);
			ContentTemplateSelectorChanged();
			ContentTemplateChanged();
		}
		protected virtual void ContentChanged(object oldValue, object newValue) {
			if (ShouldUpdateTemplate(oldValue, newValue))
				UpdateLayout(FREInvalidateOptions.UpdateSubTree);
			DataContext = Content;
			ContentPresenter.Do(x => x.Content = Content);			
		}	 
		protected virtual void ContentTemplateSelectorChanged() {
			ContentPresenter.Do(x => x.ContentTemplateSelector = ContentTemplateSelector);
		}
		protected virtual void ContentTemplateChanged() {
			ContentPresenter.Do(x => x.ContentTemplate = ActualContentTemplate);
		}
		protected virtual void ForegroundChanged() {
			ContentPresenter.Do(x => x.Foreground = Foreground);
		}
		protected override void ResetValueCore(string propertyName) {
			if (Equals("Content", propertyName))
				ResetContent();
			else
				base.ResetValueCore(propertyName);
		}
		protected virtual void ResetContent() {
			var ownerControlContext = Namescope.With(x => x.RootElement) as RenderContentControlContext;
			if (ownerControlContext != null) {
				Content = ownerControlContext.Content ?? ((RenderContentControl)ownerControlContext.Factory).Content;
			}
		}
		protected override void IsTouchEnabledChanged(bool oldValue, bool newValue) {
			base.IsTouchEnabledChanged(oldValue, newValue);
			UpdateTouchState(InnerNamescope, newValue);
		}		
	}
	public sealed class DefaultRenderTemplateSelector : RenderTemplateSelector {		
		static readonly Lazy<DefaultRenderTemplateSelector> instance = new Lazy<DefaultRenderTemplateSelector>(() => new DefaultRenderTemplateSelector());
		static readonly Func<object, bool> isXmlNode;
		static DefaultRenderTemplateSelector Instance {
			get { return instance.Value; }
		}
		static DefaultRenderTemplateSelector() {
			if (Net45Detector.IsNet45()) {
				isXmlNode = DevExpress.Xpf.Core.Internal.ReflectionHelper.CreateInstanceMethodHandler<Func<object, bool>>(
				null,
				"IsXmlNode",
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic,
				typeof(ContentPresenter).Assembly.GetType("MS.Internal.SystemXmlHelper"));
			} else {
				isXmlNode = (arg) => arg is System.Xml.XmlNode;
			}
		}
		DefaultRenderTemplateSelector() { }
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {			
			if (content is FrameworkRenderElementContext || content is FrameworkRenderElement)
				return null;
			if (Equals(content, null))
				return null;
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			if (content is UIElement)
				return resourceProvider.GetRealContentPresenterTemplate(chrome);
			return resourceProvider.GetTextContentPresenterTemplate(chrome);
		}
		public static RenderTemplate SelectTemplate(FrameworkElement chrome, object content, bool? forceVisual, ref DataTemplate dataTemplate) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			if (forceVisual.HasValue) {
				return forceVisual.Value ? resourceProvider.GetRealContentPresenterTemplate(chrome) : Instance.SelectTemplate(chrome, content);
			} else {
				if (dataTemplate != null)
					return resourceProvider.GetRealContentPresenterTemplate(chrome);				
				dataTemplate = DefaultTemplateSelector.Instance.SelectTemplate(content, chrome);
				if (dataTemplate != null && dataTemplate.DataType != null || chrome is Chrome && ((Chrome)chrome).UseDefaultStringTemplate || isXmlNode(content))
					return resourceProvider.GetRealContentPresenterTemplate(chrome);
				dataTemplate = null;
			}
			return Instance.SelectTemplate(chrome, content);
		}
	}
}
