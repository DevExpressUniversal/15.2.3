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
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	public class SplitterControl : BaseSplitterControl {
		#region static
		public static readonly DependencyProperty LayoutItemProperty;
		static SplitterControl() {
			var dProp = new DependencyPropertyRegistrator<SplitterControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((SplitterControl)dObj).OnLayoutItemChanged((BaseLayoutItem)e.NewValue));
		}
		#endregion
		public SplitterControl()
			: base(null) {
			SplitterWidth = LayoutSplitter.LayoutSplitterWidth;
			SplitterHeight = LayoutSplitter.LayoutSplitterHeight;
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		protected override Grid GetParentGrid() {
			return LayoutHelper.FindParentObject<Grid>(this);
		}
		protected override int GetCurrent(bool isColumns) {
			DependencyObject parent = LayoutItem ?? (DependencyObject)this;
			int current = (int)parent.GetValue(isColumns ? Grid.ColumnProperty : Grid.RowProperty);
			return current;
		}
		protected override int GetNext(int current) {
			if(LayoutGroup != null && LayoutItem != null) {
				int index = LayoutGroup.Items.IndexOf(LayoutItem);
				for(int i = index; i < LayoutGroup.Items.Count; i++) {
					BaseLayoutItem nextItem = LayoutGroup.Items[i];
					if(IsResizableItem(nextItem, IsHorizontal))
						return LayoutGroup.ItemsInternal.IndexOf(nextItem);
				}
			}
			return current;
		}
		protected override int GetPrev(int current) {
			if(LayoutGroup != null && LayoutItem != null) {
				int index = LayoutGroup.Items.IndexOf(LayoutItem);
				for(int i = index; i >= 0; i--) {
					BaseLayoutItem prevItem = LayoutGroup.Items[i];
					if(IsResizableItem(prevItem, IsHorizontal))
						return LayoutGroup.ItemsInternal.IndexOf(prevItem);
				}
			}
			return current;
		}
		const string HorizontalRootElementName = "PART_HorizontalRootElement";
		const string VerticalRootElementName = "PART_VerticalRootElement";
		protected FrameworkElement PartHorizontalRootElement { get; set; }
		protected FrameworkElement PartVerticalRootElement { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DockLayoutManager.Ensure(this);
			PartHorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			PartVerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplateParts();
			if(LayoutGroup != null) {
				if(!IsActivated) Activate();
				bool horizontal = LayoutGroup.Orientation == Orientation.Horizontal;
				InitSplitThumb(horizontal);
			}
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item) {
			if(item != null)
				LayoutGroup = item.Parent;
			else LayoutGroup = null;
		}
		protected override void OnOrientationChanged(Orientation orientation) {
			base.OnOrientationChanged(orientation);
			UpdateTemplateParts();
		}
		void UpdateTemplateParts() {
			if(PartHorizontalRootElement != null)
				PartHorizontalRootElement.Visibility = VisibilityHelper.Convert(Orientation == Orientation.Vertical);
			if(PartVerticalRootElement != null)
				PartVerticalRootElement.Visibility = VisibilityHelper.Convert(Orientation == Orientation.Horizontal);
		}
	}
}
