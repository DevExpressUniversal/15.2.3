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
using System.Windows.Controls;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Caption", Type = typeof(ControlItemCaptionPresenter))]
	[TemplatePart(Name = "PART_Control", Type = typeof(ControlItemControlPresenter))]
	public class ControlItem : psvHeaderedContentControl {
		#region static
		static ControlItem() {
			var dProp = new DependencyPropertyRegistrator<ControlItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public ControlItem() {
			LayoutUpdated += OnLayoutUpdated;
		}
		protected override void OnDispose() {
			LayoutUpdated -= OnLayoutUpdated;
			if(PartCaption != null) {
				PartCaption.Dispose();
				PartCaption = null;
			}
			if(PartControl != null) {
				PartControl.Dispose();
				PartControl = null;
			}
			base.OnDispose();
		}
		public ControlItemCaptionPresenter PartCaption { get; private set; }
		public ControlItemControlPresenter PartControl { get; private set; }
		protected FrameworkElement PartBorder { get; private set; }
		protected LayoutControlItem Item { get { 
			return LayoutItem as LayoutControlItem; } 
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartBorder = GetTemplateChild("PART_Border") as FrameworkElement;
			if(PartCaption != null && !LayoutItemsHelper.IsTemplateChild(PartCaption, this))
				PartCaption.Dispose();
			PartCaption = GetTemplateChild("PART_Caption") as ControlItemCaptionPresenter;
			if(PartCaption != null) {
				PartCaption.EnsureOwner(this);
			}
			if(PartControl != null && !LayoutItemsHelper.IsTemplateChild(PartControl, this))
				PartControl.Dispose();
			PartControl = GetTemplateChild("PART_Control") as ControlItemControlPresenter;
			if(PartControl != null) {
				PartControl.EnsureOwner(this);
			}
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			if(IsDisposing) return;
			if(Item != null) Item.LayoutSize = value;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateDesiredSize(DesiredSize);
		}
		void UpdateDesiredSize(Size desiredSize) {
			if(Item != null && !Item.HasDesiredSize) {
				if(PartBorder != null)
					desiredSize = PartBorder.DesiredSize;
				Item.DesiredSizeInternal = desiredSize;
			}
		}
	}
	public abstract class ControlItemElementPresenter :
		BasePanePresenter<ControlItem, LayoutControlItem> {
		protected override LayoutControlItem ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as LayoutControlItem ?? base.ConvertToLogicalItem(content);
		}
	}
	public class ControlItemCaptionPresenter : ControlItemElementPresenter {
		class DefaultCaptionTemplateSelector : DataTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				ControlItemCaptionPresenter presenter = container as ControlItemCaptionPresenter;
				if(presenter != null && presenter.Owner != null) {
					return presenter.Owner.HeaderTemplate;
				}
				return null;
			}
		}
		DataTemplateSelector defaultCaptionTemplateSelector;
		DataTemplateSelector _DefaultCaptionTemplateSelector {
			get {
				if(defaultCaptionTemplateSelector == null)
					defaultCaptionTemplateSelector = new DefaultCaptionTemplateSelector();
				return defaultCaptionTemplateSelector;
			}
		}
		protected override bool CanSelectTemplate(LayoutControlItem item) {
			return _DefaultCaptionTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LayoutControlItem item) {
			return _DefaultCaptionTemplateSelector.SelectTemplate(item, this);
		}
	}
	public class ControlItemControlPresenter : ControlItemElementPresenter {
		class DefaultContentTemplateSelector : DataTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				ControlItemControlPresenter presenter = container as ControlItemControlPresenter;
				if(presenter != null && presenter.Owner != null) {
					return presenter.Owner.ContentTemplate;
				}
				return null;
			}
		}
		DataTemplateSelector defaultContentTemplateSelector;
		DataTemplateSelector _DefaultContentTemplateSelector {
			get {
				if(defaultContentTemplateSelector == null)
					defaultContentTemplateSelector = new DefaultContentTemplateSelector();
				return defaultContentTemplateSelector;
			}
		}
		protected override void OnDispose() {
			if(PartControl != null) {
				PartControl.Dispose();
				PartControl = null;
			}
			base.OnDispose();
		}
		protected override bool CanSelectTemplate(LayoutControlItem item) {
			return _DefaultContentTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LayoutControlItem item) {
			return _DefaultContentTemplateSelector.SelectTemplate(item, this);
		}
		public UIElementPresenter PartControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartControl != null && !LayoutItemsHelper.IsTemplateChild(PartControl, this))
				PartControl.Dispose();
			PartControl = LayoutItemsHelper.GetTemplateChild<UIElementPresenter>(this);
		}
	}
}
