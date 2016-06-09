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
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Content", Type = typeof(TabbedPaneContentPresenter))]
	public class TabbedPane : psvContentControl {
		#region static
		public static readonly DependencyProperty TabbedTemplateProperty;
		static TabbedPane() {
			var dProp = new DependencyPropertyRegistrator<TabbedPane>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("TabbedTemplate", ref TabbedTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public IUIElement GetRootUIScope() {
			return LayoutItem != null ? LayoutItem.GetRootUIScope() : null;
		}
		public TabbedPane() {
		}
#if DEBUGTEST
		internal BaseLayoutItem SelectedItem {
			get { return ((PanelTabContainer)PartContent.PartItemsContainer).SelectedItem; }
			set { ((PanelTabContainer)PartContent.PartItemsContainer).SelectedItem = value; }
		}
		internal int SelectedIndex {
			get { return ((PanelTabContainer)PartContent.PartItemsContainer).SelectedIndex; }
			set { ((PanelTabContainer)PartContent.PartItemsContainer).SelectedIndex = value; }
		}
#endif
		protected override void OnDispose() {
			ClearValue(TabbedTemplateProperty);
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			base.OnDispose();
		}
		public DataTemplate TabbedTemplate {
			get { return (DataTemplate)GetValue(TabbedTemplateProperty); }
			set { SetValue(TabbedTemplateProperty, value); }
		}
		public TabbedPaneContentPresenter PartContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = GetTemplateChild("PART_Content") as TabbedPaneContentPresenter;
			if(PartContent != null) {
				PartContent.EnsureOwner(this);
#if SILVERLIGHT//B233300
				PartContent.SetBinding(TabbedPaneContentPresenter.ContentProperty, new System.Windows.Data.Binding("Content") { Source = this });
#endif
			}
		}
	}
	[TemplatePart(Name = "PART_ItemsControl", Type = typeof(psvItemsControl))]
	public class TabbedPaneContentPresenter : BasePanePresenter<TabbedPane, TabbedGroup> {
		protected override void OnDispose() {
			if(PartItemsContainer != null) {
				PartItemsContainer.Dispose();
				PartItemsContainer = null;
			}
			base.OnDispose();
		}
		protected override bool CanSelectTemplate(TabbedGroup group) {
			return group.ActualGroupTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(TabbedGroup group) {
			return group.ActualGroupTemplateSelector.SelectTemplate(group, this);
		}
		public psvItemsControl PartItemsContainer { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartItemsContainer = GetTemplateChild("PART_ItemsControl") as psvItemsControl;
		}
		protected override TabbedGroup ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as TabbedGroup ?? base.ConvertToLogicalItem(content);
		}
	}
	public class PanelTabContainer : LayoutTabControl {
		#region static
		static PanelTabContainer() {
			var dProp = new DependencyPropertyRegistrator<PanelTabContainer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public PanelTabContainer() {
		}
		protected override psvSelectorItem CreateSelectorItem() {
			return new TabbedPaneItem();
		}
		protected override IView GetView(DockLayoutManager container) {
			TabbedPane documentPane = null;
			TabbedPaneContentPresenter presenter = TemplatedParent as TabbedPaneContentPresenter;
			if(presenter != null) documentPane = presenter.Owner;
			return (documentPane != null) ? container.GetView(documentPane.GetRootUIScope()) : null;
		}
	}
	public class TabHeadersPanel : BaseHeadersPanel {
		public static readonly DependencyProperty AllowChildrenMeasureProperty;
		static TabHeadersPanel() {
			var dProp = new DependencyPropertyRegistrator<TabHeadersPanel>();
			dProp.Register("AllowChildrenMeasure", ref AllowChildrenMeasureProperty, true,
				(dObj, e) => ((TabHeadersPanel)dObj).OnShowLastChildChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		public bool AllowChildrenMeasure {
			get { return (bool)GetValue(AllowChildrenMeasureProperty); }
			set { SetValue(AllowChildrenMeasureProperty, value); }
		}
		TabbedPaneHeaderLayoutCalculator calculatorCore;
		TabbedPaneHeaderLayoutCalculator Calculator {
			get {
				if(calculatorCore == null) calculatorCore = new TabbedPaneHeaderLayoutCalculator();
				return calculatorCore;
			}
		}
		protected virtual void OnShowLastChildChanged(bool oldValue, bool newValue) {
			Calculator.ShowLastChild = newValue;
		}
		protected override ITabHeaderLayoutResult Measure(ITabHeaderLayoutCalculator calculator, ITabHeaderLayoutOptions options) {
			if(!AllowChildrenMeasure) {
				Calculator.InnerCalculator = calculator;
				calculator = Calculator;
			}
			return base.Measure(calculator, options);
		}
		class TabbedPaneHeaderLayoutCalculator : ITabHeaderLayoutCalculator {
			static ITabHeaderLayoutResult EmptyLayoutResult = new EmptyTabHeaderLayoutResult();
			public ITabHeaderLayoutCalculator InnerCalculator { get; set; }
			public bool ShowLastChild { get; set; }
			public TabbedPaneHeaderLayoutCalculator() {
			}
			public TabbedPaneHeaderLayoutCalculator(ITabHeaderLayoutCalculator innerCalculator) {
				InnerCalculator = innerCalculator;
			}
			#region ITabHeaderLayoutCalculator Members
			public ITabHeaderLayoutResult Calc(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
				var result = InnerCalculator.Calc(headers, options);
				if(!ShowLastChild && headers.Length == 1) {
					headers[0].IsVisible = false;
					result = EmptyLayoutResult;
				}
				return result;
			}
			#endregion
			class EmptyTabHeaderLayoutResult : ITabHeaderLayoutResult {
				#region ITabHeaderLayoutResult Members
				public bool HasScroll { get { return false; } }
				public Rect[] Headers { get { return new Rect[0]; } }
				public bool IsEmpty { get { return true; } }
				public IScrollResult ScrollResult { get { return null; } }
				public Size Size { get { return new Size(); } }
				#endregion
			}
		}
	}
}
