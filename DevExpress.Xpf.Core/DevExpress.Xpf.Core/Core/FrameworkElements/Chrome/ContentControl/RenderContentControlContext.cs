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

using System.Windows;
using DevExpress.Mvvm.Native;
using System.Windows.Controls;
using System;
namespace DevExpress.Xpf.Core.Native {
	public class RenderControlContext : FrameworkRenderElementContext {
		RenderTemplate renderTemplate;
		RenderTemplateSelector renderTemplateSelector;
		bool isPressed, isMouseOver;
		bool isFocused;
		public RenderTemplate ActualRenderTemplate { get; internal set; }
		protected override int RenderChildrenCount { get { return Context != null ? 1 : 0; } }
		protected override FrameworkRenderElementContext GetRenderChild(int index) {
			return Context;
		}		
		public RenderTemplate RenderTemplate {
			get { return renderTemplate; }
			set { SetProperty(ref renderTemplate, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public RenderTemplateSelector RenderTemplateSelector {
			get { return renderTemplateSelector; }
			set { SetProperty(ref renderTemplateSelector, value, FREInvalidateOptions.UpdateSubTree); }
		}
		#region behavior
		public bool IsFocused {
			get { return isFocused; }
			set { SetProperty(ref isFocused, value, FREInvalidateOptions.None, UpdateFocusState); }
		}		
		public bool IsMouseOver {
			get { return isMouseOver; }
			set { SetProperty(ref isMouseOver, value, FREInvalidateOptions.None, UpdateCommonState); }
		}
		public bool IsPressed {
			get { return isPressed; }
			set { SetProperty(ref isPressed, value, FREInvalidateOptions.None, UpdateCommonState); }
		}
		#endregion     
		public FrameworkRenderElementContext Context { get; private set; }
		public Namescope InnerNamescope { get; internal set; }
		public RenderControlContext(RenderControl factory)
			: base(factory) {
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
		public virtual void UpdateStates() {
			UpdateCommonState();
			UpdateFocusState();
		}
		protected virtual void UpdateCommonState() {
			string stateName = "Normal";
			if (IsMouseOver)
				stateName = "MouseOver";
			if (IsPressed)
				stateName = "Pressed";
			if (!IsEnabled)
				stateName = "Disabled";
			GoToState(stateName);
		}
		public virtual void GoToState(string stateName) {
			InnerNamescope.GoToState(stateName);
		}
		protected virtual void UpdateFocusState() {
			GoToState(IsFocused ? "Focused" : "Unfocused");
		}
		protected override void OnIsEnabledChanged(bool oldValue, bool newValue) {
			base.OnIsEnabledChanged(oldValue, newValue);
			UpdateCommonState();
		}
		protected override void OnIsMouseOverCoreChanged() {
			base.OnIsMouseOverCoreChanged();
			if (!((RenderControl)Factory).UsePropagatedIsMouseOver)
				return;
			IsMouseOver = IsMouseOverCore;
		}
	}
	public class RenderContentControlContext : RenderControlContext {
		RenderTemplateSelector renderContentTemplateSelector;
		RenderTemplate renderContentTemplate;			 
		object content;
		DataTemplate contentTemplate;
		DataTemplateSelector contentTemplateSelector;
		HorizontalAlignment? hca;
		VerticalAlignment? vca;
		Thickness? padding;
		bool? preferRenderTemplate;
		public bool? PreferRenderTemplate {
			get { return preferRenderTemplate; }
			set { SetProperty(ref preferRenderTemplate, value, FREInvalidateOptions.UpdateLayout); }
		}
		public HorizontalAlignment? HorizontalContentAlignment {
			get { return hca; }
			set { SetProperty(ref hca, value, FREInvalidateOptions.UpdateLayout, HorizontalContentAlignmentChanged);  }
		}		
		public VerticalAlignment? VerticalContentAlignment {
			get { return vca; }
			set { SetProperty(ref vca, value, FREInvalidateOptions.UpdateLayout, VerticalContentAlignmentChanged); }
		}		
		public object Content {
			get { return content; }
			set { SetProperty(ref content, value, FREInvalidateOptions.None, null, OnContentChanged); }
		}
		public RenderTemplateSelector RenderContentTemplateSelector {
			get { return renderContentTemplateSelector; }
			set { SetProperty(ref renderContentTemplateSelector, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public RenderTemplate RenderContentTemplate {
			get { return renderContentTemplate; }
			set { SetProperty(ref renderContentTemplate, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return contentTemplateSelector; }
			set { SetProperty(ref contentTemplateSelector, value, FREInvalidateOptions.None, ContentTemplateSelectorChanged); }
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set { SetProperty(ref contentTemplate, value, FREInvalidateOptions.None, ContentTemplateChanged); }
		}
		public Thickness? Padding {
			get { return padding; }
			set { SetProperty(ref padding, value, FREInvalidateOptions.None, PaddingChanged); }
		}
		public new RenderContentControl Factory { get { return (RenderContentControl)base.Factory; } }
		protected RenderContentPresenterContext ContentPresenter {
			get { return (RenderContentPresenterContext)InnerNamescope.GetElement("PART_ContentPresenter"); }
		}
		public RenderContentControlContext(RenderContentControl factory)
			: base(factory) {
		}
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			var presenter = ContentPresenter;
			if (presenter == null) {
				UpdateLayout(FREInvalidateOptions.UpdateSubTree);
				return;
			}
			if (presenter.ShouldUpdateTemplate(oldValue, newValue))
				UpdateLayout(FREInvalidateOptions.UpdateSubTree);
			else
				presenter.Content = Content ?? Factory.Content;
		}
		protected virtual void HorizontalContentAlignmentChanged() {			
			ContentPresenter.Do(x => x.HorizontalAlignment = HorizontalContentAlignment.HasValue ? HorizontalContentAlignment.Value : System.Windows.HorizontalAlignment.Stretch);
		}
		protected virtual void VerticalContentAlignmentChanged() {
			ContentPresenter.Do(x => x.VerticalAlignment = VerticalContentAlignment.HasValue ? VerticalContentAlignment.Value : System.Windows.VerticalAlignment.Stretch);
		}		
		protected virtual void ContentTemplateSelectorChanged() {
			ContentPresenter.Do(x => x.ContentTemplateSelector = ContentTemplateSelector);
		}
		protected virtual void ContentTemplateChanged() {
			ContentPresenter.Do(x => x.ContentTemplate = ContentTemplate);
		}
		protected virtual void PaddingChanged() {
			ContentPresenter.Do(x => x.Margin = Padding);
		}
		public override void UpdateStates() {
			base.UpdateStates();
			UpdateContentState();
		}
		protected virtual void UpdateContentState() {
			if (ContentPresenter == null)
				return;
			bool hasContent = Content != null;
			GoToState(hasContent ? "HasContent" : "HasNoContent");
		}
		protected override void IsTouchEnabledChanged(bool oldValue, bool newValue) {
			base.IsTouchEnabledChanged(oldValue, newValue);
			UpdateTouchState(InnerNamescope, newValue);
		}
	}
}
