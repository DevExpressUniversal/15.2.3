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
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using System.Collections;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.Windows.Data;
using DevExpress.Xpf.Editors;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.PropertyGrid {
	public class RowControl : RowControlBase, IChrome, ICommandSource {
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty CanExpandProperty;
		public static readonly DependencyProperty HeaderShowModeProperty;
		public static readonly DependencyProperty ShouldHighlightValueProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public event EventHandler IsExpandedChanged;		
		static RowControl() {
			var ownerType = typeof(RowControl);
			HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(RowControl), new PropertyMetadata(null));
			ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(RowControl), new PropertyMetadata(null));
			ShouldHighlightValueProperty = DependencyProperty.Register("ShouldHighlightValue", typeof(bool), typeof(RowControl), new PropertyMetadata(false));
			HeaderProperty = DependencyPropertyManager.Register("Header", typeof(string), typeof(RowControl), new FrameworkPropertyMetadata(null));
			CanExpandProperty = DependencyProperty.Register("CanExpand", typeof(bool), typeof(RowControl), new PropertyMetadata(false));
			HeaderShowModeProperty = DependencyProperty.Register("HeaderShowMode", typeof(HeaderShowMode), typeof(RowControl), new PropertyMetadata(HeaderShowMode.Left));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(typeof(RowControl)));
			FrameworkElement.FlowDirectionProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.FlowDirectionPropertyKey)));
			FrameworkElement.UseLayoutRoundingProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.UseLayoutRoundingPropertyKey)));
			ThemeManager.TreeWalkerProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((RowControl)d).OnTreeWalkerChanged((ThemeTreeWalker)e.OldValue, (ThemeTreeWalker)e.NewValue)));
		}
		readonly ChromeSlave slave;
		RenderTemplate renderTemplate;
		IPropertyGridResizingStrategy resizingStrategy;
		public event EventHandler HeaderPercentChanged;
		public RowControl() {
			slave = new ChromeSlave(this, true, InitializeContext, ReleaseContext, ValidateTemplate, base.MeasureOverride, base.ArrangeOverride, false);
			slave.IsEnabled = true;
		}
		#region ICommandSource
		ICommand command;
		object commandParameter;
		ICommand ICommandSource.Command {
			get { return command; }
		}
		object ICommandSource.CommandParameter {
			get { return commandParameter ?? this; }
		}
		IInputElement ICommandSource.CommandTarget {
			get { return this; }
		}
		#endregion ICommandSource
		#region render properties        
		public RenderTemplate RenderTemplate {
			get { return renderTemplate; }
			set {
				if (renderTemplate == value)
					return;
				var oldValue = renderTemplate;
				renderTemplate = value;
				OnRenderTemplateChanged(oldValue, value);
			}
		}
		protected RenderTemplate ActualRenderTemplate { get; set; }
		#endregion //render properties
		#region chrome implementation
		protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) {
			base.OnTemplateChanged(oldTemplate, newTemplate);
			slave.IsEnabled = newTemplate == null;
		}
		protected virtual bool UpdateActualRenderTemplate() {
			try {
				return Equals(renderTemplate, ActualRenderTemplate);
			} finally {
				ActualRenderTemplate = renderTemplate;
			}
		}
		protected virtual void ReleaseContext(FrameworkRenderElementContext obj) {
			slave.ReleaseContext(obj);
			OnApplyRenderTemplate();
		}
		protected void InvalidateContext() { slave.InvalidateContext(); }
		public FrameworkRenderElementContext Root { get { return slave.Root; } }
		protected IElementHost ElementHost { get { return slave.ElementHost; } }
		protected INamescope Namescope { get { return slave.Namescope; } }
		void IChrome.AddChild(FrameworkElement element) {
			AddLogicalChild(element);
			AddVisualChild(element);
		}
		void IChrome.RemoveChild(FrameworkElement element) {
			RemoveLogicalChild(element);
			RemoveVisualChild(element);
		}
		void IChrome.GoToState(string stateName) {
			slave.GoToState(stateName);
		}
		protected override void OnRowDataChanged(RowDataBase oldValue) {
			UpdateCommandAndParameter();
			base.OnRowDataChanged(oldValue);
			DetachFromDefinition(RowData == null);
			if (RowData != null) {
				Header = RowData.With(x => x.Header);
				HeaderShowMode = RowData.Definition.Return(x => x.HeaderShowMode, () => HeaderShowMode.Left);
				SetBinding(CanExpandProperty, new Binding() { Source = RowData, Path = new PropertyPath(RowDataBase.CanExpandProperty) });
				AttachToDefinition();
			}
			InvalidateContext();
		}
		private void UpdateCommandAndParameter() {
			command = null;
			commandParameter = null;
			if (RowData != null) {
				var pdefinition = RowData.Definition as PropertyDefinition;
				if (pdefinition != null) {
					command = pdefinition.Command;
					commandParameter = pdefinition.CommandParameter;
				}
			}
		}
		PropertyDefinitionBase definition;
		void DetachFromDefinition(bool clearProperties) {			
			if (definition == null)
				return;
			definition.NotifyPropertyChanged -= PrivateOnDefinitionPropertyChanged;			
			if (clearProperties) {				
				ClearDefinitionProperties();
			}				
		}		
		void AttachToDefinition() {
			definition = RowData.Definition;			
			if (definition == null)
				return;
			definition.NotifyPropertyChanged += PrivateOnDefinitionPropertyChanged;
			AssignDefinitionProperties();
		}
		void PrivateOnDefinitionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			OnDefinitionPropertyChanged(e.PropertyName);
		}
		void AssignDefinitionProperties() {
			Template = definition.RowTemplate;
			HeaderTemplateSelector = definition.ActualHeaderTemplateSelector;
			ContentTemplateSelector = definition.ActualContentTemplateSelector;
		}
		void ClearDefinitionProperties() {
			Template = null;
			HeaderTemplateSelector = null;
			ContentTemplateSelector = null;
		}		
		protected virtual void OnDefinitionPropertyChanged(string propertyName) {
			if(propertyName == "RowTemplate") {
				Template = definition.RowTemplate;
				return;
			}
			if (propertyName == "ActualHeaderTemplateSelector") {
				HeaderTemplateSelector = definition.ActualHeaderTemplateSelector;
				return;
			}
			if (propertyName == "ActualContentTemplateSelector") {
				ContentTemplateSelector = definition.ActualContentTemplateSelector;
				return;
			}			
		}
		protected override void OnOwnerViewChanged(PropertyGridView oldValue, PropertyGridView newValue) {
			base.OnOwnerViewChanged(oldValue, newValue);
			ResizingStrategy.HeaderPercentChanged -= OnHeaderPercentChanged;
			ResizingStrategy.ResetCurrentRow(this);
			ResizingStrategy = newValue.With(x => x.ResizingStrategy);
			ResizingStrategy.HeaderPercentChanged += OnHeaderPercentChanged;
			UpdateRenderResourceProviderDependentProperties();
		}
		void OnHeaderPercentChanged(object sender, EventArgs e) {
			if (HeaderPercentChanged != null)
				HeaderPercentChanged(this, EventArgs.Empty);
		}
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, ElementHost.LogicalChildren); } }
		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + ElementHost.ChildrenCount; } }
		protected override Visual GetVisualChild(int index) {
			int childrenCount = base.VisualChildrenCount;
			if (childrenCount == 0)
				return ElementHost.GetChild(index);
			return index < childrenCount ? base.GetVisualChild(index) : ElementHost.GetChild(index - childrenCount);
		}
		protected override Size MeasureOverride(Size constraint) { return slave.MeasureOverride(constraint); }
		protected override Size ArrangeOverride(Size arrangeBounds) { return slave.ArrangeOverride(arrangeBounds); }
		protected override void OnRender(DrawingContext drawingContext) { slave.OnRender(drawingContext); }
		protected virtual void OnRenderTemplateChanged(RenderTemplate oldValue, RenderTemplate newValue) {
			InvalidateContext();
		}
		protected virtual void OnTreeWalkerChanged(ThemeTreeWalker oldValue, ThemeTreeWalker newValue) {
			ChromeSlave.CoerceValue(this, FrameworkRenderElementContext.IsTouchEnabledPropertyKey);
			UpdateRenderResourceProviderDependentProperties();
		}
		protected virtual bool ValidateTemplate() {
			if (UpdateActualRenderTemplate()) {
				return true;
			}
			return false;
		}
		protected virtual FrameworkRenderElementContext InitializeContext() {
			UpdateActualRenderTemplate();
			var result = slave.CreateContext(ActualRenderTemplate);
			OnApplyRenderTemplate();
			return result;
		}
		protected bool UseChrome { get { return slave.IsEnabled == true; } }
		#endregion //chrome implementation
		#region chrome logic
		protected virtual void UpdateRenderResourceProviderDependentProperties() {
			if (OwnerView == null)
				return;
			UpdateRenderTemplate();
			UpdateRealMinHeight();
		}
		protected FrameworkRenderElementContext PART_Root { get; private set; }
		protected virtual void OnApplyRenderTemplate() {
			PART_Root = Namescope.GetElement("PART_Root");
			UpdateRenderMinHeight();
		}
		protected override void OnIsExpandedChanged(bool oldValue) {
			base.OnIsExpandedChanged(oldValue);
			if (IsExpandedChanged != null)
				IsExpandedChanged(this, EventArgs.Empty);
		}
		#endregion //chrome logic
		#region wpf logic
		protected virtual void UpdateRealMinHeight() {
			MinHeight = OwnerView.GetResourceProvider().GetRowMinHeight();
		}
		#endregion //wpf logic        
		public IPropertyGridResizingStrategy ResizingStrategy {
			get { return resizingStrategy ?? NullPropertyGridResizingStrategy.Instance; }
			set { resizingStrategy = value; }
		}
		public bool ShouldHighlightValue {
			get { return (bool)GetValue(ShouldHighlightValueProperty); }
			set { SetValue(ShouldHighlightValueProperty, value); }
		}
		public HeaderShowMode HeaderShowMode {
			get { return (HeaderShowMode)GetValue(HeaderShowModeProperty); }
			set { SetValue(HeaderShowModeProperty, value); }
		}
		public string Header {
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		public bool CanExpand {
			get { return (bool)GetValue(CanExpandProperty); }
			set { SetValue(CanExpandProperty, value); }
		}
		protected internal override FrameworkElement MenuPlacementTarget {
			get { return this; }
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			ResizingStrategy.SetCurrentRow(this);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			ResizingStrategy.ResetCurrentRow(this);
		}
		protected virtual void UpdateRenderMinHeight() {
			if (PART_Root == null)
				return;
			PART_Root.MinHeight = OwnerView.GetResourceProvider().GetRowMinHeight();
		}		
		protected virtual void UpdateRenderTemplate() {
			RenderTemplate = OwnerView.GetResourceProvider().GetRowTemplate();
		}
		public void OnCommandButtonClick(object commandButton) {
			if (OwnerView != null) {
				OwnerView.MenuHelper.ShowCommandsMenu(RowData, commandButton);
			}
		}
		public void OnCollectionButtonClick(object collectionButton) {
			if (OwnerView != null) {
				OwnerView.CollectionHelper.OnCollectionButtonClick(RowData, collectionButton);
			}			
		}
	}
}
