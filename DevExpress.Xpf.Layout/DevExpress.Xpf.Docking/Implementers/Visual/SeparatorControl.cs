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
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	public class SeparatorControl : psvContentControl {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		static SeparatorControl() {
			var dProp = new DependencyPropertyRegistrator<SeparatorControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((SeparatorControl)dObj).OnOrientationChanged((Orientation)e.NewValue));
		}
		#endregion
		public SeparatorControl() {
		}
		protected override void OnDispose() {
			ClearValue(DockLayoutManager.DockLayoutManagerProperty);
			ClearValue(DockLayoutManager.LayoutItemProperty);
			base.OnDispose();
		}
		const string HorizontalRootElementName = "PART_HorizontalRootElement";
		const string VerticalRootElementName = "PART_VerticalRootElement";
		protected FrameworkElement PartHorizontalRootElement { get; set; }
		protected FrameworkElement PartVerticalRootElement { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartHorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			PartVerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplateParts();
		}
		public void OnOrientationChanged(Orientation orientation) {
			UpdateTemplateParts();
		}
		void UpdateTemplateParts() {
			if(PartHorizontalRootElement != null)
				PartHorizontalRootElement.Visibility = VisibilityHelper.Convert(Orientation == Orientation.Vertical);
			if(PartVerticalRootElement != null)
				PartVerticalRootElement.Visibility = VisibilityHelper.Convert(Orientation == Orientation.Horizontal);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
}
