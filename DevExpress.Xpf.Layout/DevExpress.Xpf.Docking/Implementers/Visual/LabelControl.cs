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
namespace DevExpress.Xpf.Docking.VisualElements {
	public class LabelControl : psvHeaderedContentControl {
		#region static
		static LabelControl() {
			var dProp = new DependencyPropertyRegistrator<LabelControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public LabelControl() {
			LayoutUpdated += OnLayoutUpdated;
		}
		protected override void OnDispose() {
			LayoutUpdated -= OnLayoutUpdated;
			ClearValue(DockLayoutManager.DockLayoutManagerProperty);
			ClearValue(DockLayoutManager.LayoutItemProperty);
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
		protected UIElement PartBorder { get; private set; }
		protected LabelControlCaptionPresenter PartCaption { get; private set; }
		protected LabelControlContentPresenter PartControl { get; private set; }
		protected LabelItem Item { get { return LayoutItem as LabelItem; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartCaption != null && !LayoutItemsHelper.IsTemplateChild(PartCaption, this))
				PartCaption.Dispose();
			if(PartControl != null && !LayoutItemsHelper.IsTemplateChild(PartControl, this))
				PartControl.Dispose();
			PartBorder = GetTemplateChild("PART_Border") as UIElement;
			PartCaption = GetTemplateChild("PART_Caption") as LabelControlCaptionPresenter;
			PartControl = GetTemplateChild("PART_Content") as LabelControlContentPresenter;
			if(PartControl != null) {
				PartControl.EnsureOwner(this);
			}
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
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			if(IsDisposing) return;
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			if(item != null) item.LayoutSize = value;
		}
	}
	public class LabelControlCaptionPresenter : psvContentPresenter {
	}
	public class LabelControlContentPresenter : BasePanePresenter<LabelControl, LabelItem> {
		class DefaultContentTemplateSelector : DataTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				LabelControlContentPresenter presenter = container as LabelControlContentPresenter;
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
		protected override bool CanSelectTemplate(LabelItem item) {
			return _DefaultContentTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LabelItem item) {
			return _DefaultContentTemplateSelector.SelectTemplate(item, this);
		}
		protected override LabelItem ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as LabelItem ?? base.ConvertToLogicalItem(content);
		}
	}
}
