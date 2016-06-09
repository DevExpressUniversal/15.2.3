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
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCategoryHeaderControl : RibbonCaptionControl {
		#region static
		public static readonly DependencyProperty CategoryProperty;
		public static readonly DependencyProperty RightIndentProperty;
		public static readonly DependencyProperty IsInRibbonWindowProperty;
		protected internal static readonly DependencyPropertyKey IsInRibbonWindowPropertyKey;
		public static readonly DependencyProperty IsAeroModeProperty;
		public static readonly DependencyProperty AeroTemplateProperty;
		public static readonly DependencyProperty HeaderVisibilityProperty;								
		protected static readonly DependencyPropertyKey SizeInfoPropertyKey;
		public static readonly DependencyProperty SizeInfoProperty;			   
		static RibbonPageCategoryHeaderControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(typeof(RibbonPageCategoryHeaderControl)));
			IsAeroModeProperty = DependencyPropertyManager.Register("IsAeroMode", typeof(bool), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
			AeroTemplateProperty = DependencyPropertyManager.Register("AeroTemplate", typeof(ControlTemplate), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroTemplatePropertyChanged)));
			CategoryProperty = DependencyPropertyManager.Register("Category", typeof(RibbonPageCategoryBase), typeof(RibbonPageCategoryHeaderControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPageCategoryPropertyChanged)));
			RightIndentProperty = DependencyPropertyManager.Register("RightIndent", typeof(double), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnRightIndentPropertyChanged)));
			IsInRibbonWindowPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsInRibbonWindow", typeof(bool), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(false));
			IsInRibbonWindowProperty = IsInRibbonWindowPropertyKey.DependencyProperty;
			HeaderVisibilityProperty = DependencyPropertyManager.Register("HeaderVisibility", typeof(Visibility), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(Visibility.Visible, (d, e) => ((RibbonPageCategoryHeaderControl)d).OnHeaderVisibilityChanged((Visibility)e.OldValue)));
			SizeInfoPropertyKey = DependencyPropertyManager.RegisterReadOnly("SizeInfo", typeof(RibbonPageCategoryHeaderInfo), typeof(RibbonPageCategoryHeaderControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonPageCategoryHeaderControl)d).OnSizeInfoChanged((RibbonPageCategoryHeaderInfo)e.OldValue)));
			SizeInfoProperty = SizeInfoPropertyKey.DependencyProperty;
		}
		static protected void OnRightIndentPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryHeaderControl)o).OnRightIndentChanged((double)e.OldValue);
		}
		protected static void OnPageCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryHeaderControl)d).OnPageCategoryChanged(e.OldValue as RibbonPageCategoryBase);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryHeaderControl)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		protected static void OnAeroTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryHeaderControl)d).OnAeroTemplateChanged((ControlTemplate)e.OldValue);
		}
		#endregion
		#region dep props
		public RibbonPageCategoryBase Category {
			get { return (RibbonPageCategoryBase)GetValue(CategoryProperty); }
			set { SetValue(CategoryProperty, value); }
		}
		public Visibility HeaderVisibility {
			get { return (Visibility)GetValue(HeaderVisibilityProperty); }
			set { SetValue(HeaderVisibilityProperty, value); }
		}
		public double RightIndent {
			get { return (double)GetValue(RightIndentProperty); }
			set { SetValue(RightIndentProperty, value); }
		}
		public bool IsInRibbonWindow {
			get { return (bool)GetValue(IsInRibbonWindowProperty); }
			protected internal set { this.SetValue(IsInRibbonWindowPropertyKey, value); }
		}
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public ControlTemplate AeroTemplate {
			get { return (ControlTemplate)GetValue(AeroTemplateProperty); }
			set { SetValue(AeroTemplateProperty, value); }
		}
		public RibbonPageCategoryHeaderInfo SizeInfo {
			get { return (RibbonPageCategoryHeaderInfo)GetValue(SizeInfoProperty); }
			protected internal set { this.SetValue(SizeInfoPropertyKey, value); }
		}		
		#endregion
		public RibbonPageCategoryHeaderControl() {
			SetBinding(IsAeroModeProperty, new Binding("Category.Ribbon.IsAeroMode") { Source = this });
		}
		protected virtual void OnRightIndentChanged(double oldValue) {
			if(Margin.Right == oldValue)
				return;
			Margin = new Thickness(Margin.Left, Margin.Top, RightIndent, Margin.Bottom);
		}
		protected ContentControl SelectedContent { get; private set; }
		protected ContentControl NormalContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (NormalContent != null) {
				NormalContent.SizeChanged -= OnContentSizeChanged;
			}
			if (SelectedContent != null) {
				SelectedContent.SizeChanged -= OnContentSizeChanged;
			}
			NormalContent = GetTemplateChild("PART_NormalContent") as ContentControl;
			SelectedContent = GetTemplateChild("PART_SelectedContent") as ContentControl;
			if (NormalContent != null) {
				NormalContent.SizeChanged += OnContentSizeChanged;
			}
			if (SelectedContent != null) {
				SelectedContent.SizeChanged += OnContentSizeChanged;
			}
		}
		void OnContentSizeChanged(object sender, SizeChangedEventArgs e) { }
		protected override Size MeasureOverride(Size constraint) {
			Size desiredSize = base.MeasureOverride(constraint);
			UpdateSizeInfo();
			return desiredSize;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if (Category == null || Category.Ribbon == null || Category.Ribbon.IsBackStageViewOpen)
				return;
			var pageToSelect = Category.ActualPagesCore.FirstOrDefault(page => page.ActualIsVisible);
			if (pageToSelect != null) {
				Category.Ribbon.SetCurrentValue(RibbonControl.SelectedPageProperty, pageToSelect);
				if (Category.Ribbon.IsMinimizedRibbonCollapsed)
					Category.Ribbon.ExpandMinimizedRibbon();
			}
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateTemplate();
		}
		protected virtual void OnAeroTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(IsAeroMode)
				Template = AeroTemplate;
		}
		protected virtual void UpdateVisibility() { }
		protected virtual void OnHeaderVisibilityChanged(Visibility oldValue) {
			UpdateVisibility();
		}
		protected virtual void OnPageCategoryChanged(RibbonPageCategoryBase oldValue) {
			if(oldValue != null)
				oldValue.CategoryHeaderControls.Remove(this);
			if(Category != null) {
				Category.CategoryHeaderControls.Add(this);
			}
			UpdateVisibility();
		}
		protected internal virtual void UpdateSizeInfo(bool isasync = true) {
			var ribbon = Ribbon ?? RibbonControl.GetRibbon(this);
			SizeInfo = new RibbonPageCategoryHeaderInfo();
			SizeInfo.DesiredWidth = BestWidth;
			SizeInfo.MaxWidth = MaxDesiredSize.Width;
			SizeInfo.MinWidth = BestDesiredWidth;
		}
		protected virtual void OnSizeInfoChanged(RibbonPageCategoryHeaderInfo oldValue) { }
	}
}
