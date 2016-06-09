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
	[TemplatePart(Name = "PART_SplitThumb", Type = typeof(UIElement))]
	public class BaseSplitterControl : BaseSplitterItem {
		#region static
		public static readonly DependencyProperty IsDragDropOverProperty;
		static BaseSplitterControl() {
			var dProp = new DependencyPropertyRegistrator<BaseSplitterControl>();
			dProp.Register("IsDragDropOver", ref IsDragDropOverProperty, false);
		}
		#endregion static
		protected UIElement PartSplitThumb { get; private set; }
		public override void Deactivate() {
			DockLayoutManager.Release(this);
			base.Deactivate();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartSplitThumb = GetTemplateChild("PART_SplitThumb") as UIElement;
		}
		protected double SplitterWidth;
		protected double SplitterHeight;
		public void InitSplitThumb(bool horizontal) {
			if(PartSplitThumb != null) {
				PartSplitThumb.SetValue(horizontal ? WidthProperty : HeightProperty,
						horizontal ? SplitterWidth : SplitterHeight
					);
				PartSplitThumb.ClearValue(horizontal ? HeightProperty : WidthProperty);
			}
		}
		public bool IsDragDropOver {
			get { return (bool)GetValue(IsDragDropOverProperty); }
			set { SetValue(IsDragDropOverProperty, value); }
		}
		public BaseSplitterControl(LayoutGroup group) {
			LayoutGroup = group;
			psvPanel.SetZIndex(this, 1);
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			SplitterWidth = DockLayoutManagerParameters.DockingItemIntervalHorz;
			SplitterHeight = DockLayoutManagerParameters.DockingItemIntervalVert;
		}
	}
	public class Splitter : BaseSplitterControl, IUIElement {
		#region static
		static Splitter() {
			var dProp = new DependencyPropertyRegistrator<Splitter>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public Splitter(LayoutGroup group) : base(group) {
		}
		protected override IResizeCalculator ResolveResizeCalculator() {
			return System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) ? base.ResolveResizeCalculator() :
				new RecursiveResizeCalculator() { Orientation = Orientation };
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion
	}
}
