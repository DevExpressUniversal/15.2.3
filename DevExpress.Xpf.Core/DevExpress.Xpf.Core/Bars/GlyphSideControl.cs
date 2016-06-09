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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Bars;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class GlyphSidePanel: Panel {			 
		public static readonly DependencyProperty SubMenuProperty =
			 DependencyPropertyManager.Register("SubMenu", typeof(SubMenuBarControl), typeof(GlyphSidePanel), new FrameworkPropertyMetadata(null, (d, e) => ((GlyphSidePanel)d).OnSubMenuChanged(e.OldValue as SubMenuBarControl)));					 
		public SubMenuBarControl SubMenu {
			get { return (SubMenuBarControl)GetValue(SubMenuProperty); }
			set { SetValue(SubMenuProperty, value); }
		}		
		public PopupMenuBase Popup { get { return SubMenu != null ? SubMenu.Popup : null; } }
		protected virtual void OnSubMenuChanged(SubMenuBarControl oldValue) {
			if(SubMenu != null)
				SubMenu.GlyphSidePanel = this;
		}
		protected override Size MeasureOverride(Size availableSize) {
			foreach(UIElement child in Children)
				child.Measure(SizeHelper.Infinite);
			return new Size();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(glyphSidePanelInfo == null) return new Size();
			double left = 0d;
			for(int i = 0; i < splitters.Count; i++) {
				left += desiredColumnWidth;
				splitters[i].Arrange(new Rect(left, 0, splitters[i].DesiredSize.Width, arrangeHeight));
				left += splitters[i].DesiredSize.Width;
			}
			for(int i = 0; i < panelControls.Count; i++) {
				panelControls[i].Arrange(glyphSidePanelInfo[i]);
			}
			return new Size(arrangeWidth, arrangeHeight);
		}
		private static bool IsNotNanOrInfinity(double value) {
			return Double.IsNaN(value) || Double.IsInfinity(value);
		}
		internal Size MeasureSplitter(double maxHeight, PopupMenuColumnSplitter splitter) {
			Children.Add(splitter);
			splitter.ApplyTemplate();
			splitter.Measure(new Size(double.PositiveInfinity, maxHeight));
			Size dSize = splitter.DesiredSize;
			Children.Remove(splitter);
			return dSize;
		}
		double arrangeWidth = 0d;
		double arrangeHeight = 0d;
		double desiredColumnWidth = 0d;
		List<RectEx> glyphSidePanelInfo = null;
		DevExpress.Xpf.Bars.Native.WeakList<UIElement> splitters = new Native.WeakList<UIElement>();
		DevExpress.Xpf.Bars.Native.WeakList<UIElement> panelControls = new Native.WeakList<UIElement>();
		internal void Update(double arrangeWidth, double arrangeHeight, int desiredColumnsCount, List<RectEx> glyphSidePanelInfo, double desiredColumnWidth) {
			this.arrangeWidth = arrangeWidth;
			this.arrangeHeight = arrangeHeight;
			this.desiredColumnWidth = desiredColumnWidth;
			this.glyphSidePanelInfo = glyphSidePanelInfo;
			splitters.Clear();
			panelControls.Clear();
			Children.Clear();
			for(int i = 0; i < desiredColumnsCount-1; i++) {
				var splitter = new PopupMenuColumnSplitter();
				Children.Add(splitter);
				splitters.Add(splitter);				
			}
			for(int i = 0; i < glyphSidePanelInfo.Count; i++) {
				var glyphSideControl = new GlyphSideControl() { ItemPosition = glyphSidePanelInfo[i].VerticalPosition };
				Children.Add(glyphSideControl);
				panelControls.Add(glyphSideControl);
			}
		}
	}
	public class PopupMenuColumnSplitter : Control {
		static PopupMenuColumnSplitter() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupMenuColumnSplitter), new FrameworkPropertyMetadata(typeof(PopupMenuColumnSplitter)));
		}
		public PopupMenuColumnSplitter() {
		}
	}	
	public class GlyphSideControl: Control {
		public static readonly DependencyProperty ItemPositionProperty =
			DependencyPropertyManager.Register("ItemPosition", typeof(VerticalAlignment), typeof(GlyphSideControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, (d, e) => ((GlyphSideControl)d).OnItemPositionChanged((VerticalAlignment)e.OldValue)));
		public VerticalAlignment ItemPosition {
			get { return (VerticalAlignment)GetValue(ItemPositionProperty); }
			set { SetValue(ItemPositionProperty, value); }
		}
		protected internal double VerticalOffset { get; set; }
		public GlyphSideControl() {
			DefaultStyleKey = typeof(GlyphSideControl);
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			OnItemPositionChanged(VerticalAlignment.Center);
		}
		protected virtual void OnItemPositionChanged(VerticalAlignment oldValue) {
			VisualStateManager.GoToState(this, ItemPosition.ToString(), false);
		}
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
	}
	public class ReversedPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			Size resultSize = new Size(0d, 0d);
			for (int i = System.Windows.Media.VisualTreeHelper.GetChildrenCount(this) - 1; i >= 0; i--) {
				UIElement vChild = System.Windows.Media.VisualTreeHelper.GetChild(this, i) as UIElement;
				if (vChild != null) {
					vChild.Measure(availableSize);
					resultSize.Height = Math.Max(resultSize.Height, vChild.DesiredSize.Height);
					resultSize.Width = Math.Max(resultSize.Width, vChild.DesiredSize.Width);
				}
			}
			return resultSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			for (int i = System.Windows.Media.VisualTreeHelper.GetChildrenCount(this) - 1; i >= 0; i--) {
				UIElement vChild = System.Windows.Media.VisualTreeHelper.GetChild(this, i) as UIElement;
				vChild.Arrange(new Rect(new Point(0, 0), finalSize));
			}
			return finalSize;
		}
	}
}
