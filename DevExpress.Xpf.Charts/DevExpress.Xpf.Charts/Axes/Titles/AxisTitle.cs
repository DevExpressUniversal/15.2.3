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

using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum TitleAlignment {
		Near = 0,
		Center = 1,
		Far = 2
	}
	public class AxisTitle : AxisElementTitleBase, ISupportVisibilityControlElement {
		public static readonly DependencyProperty AlignmentProperty = DependencyPropertyManager.Register("Alignment",
			typeof(TitleAlignment), typeof(AxisTitle), new PropertyMetadata(TitleAlignment.Center, ChartElementHelper.UpdateWithClearDiagramCache));
		static AxisTitle() {
			FlowDirectionProperty.OverrideMetadata(typeof(AxisTitle), new FrameworkPropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
			bool? newValue = null; 
			VisibleProperty.OverrideMetadata(typeof(AxisTitle), new FrameworkPropertyMetadata(newValue));
		}
		bool autoLayoutVisible = true;
		Rect bounds;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisTitleAlignment"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public TitleAlignment Alignment {
			get { return (TitleAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		internal TitleAlignment ActualAlignment {
			get {
				TitleAlignment alignment = Alignment;
				Axis axis = Axis;
				if (axis != null && axis.IsReversed)
					switch (alignment) {
						case TitleAlignment.Near:
							return TitleAlignment.Far;
						case TitleAlignment.Far:
							return TitleAlignment.Near;
					}
				return alignment;
			}
		}
		internal override bool ActualVisible {
			get {
				if (Visible.HasValue)
					return Visible.Value;
				return autoLayoutVisible;
			}
		}		
		internal Rect Bounds {
			set {
				if (!value.IsEmpty && value.Height != 0 && value.Height != 0)
					bounds = value;
			}
		}
		protected internal override Axis Axis { get { return ((IOwnedElement)this).Owner as Axis; } }
		protected internal override object HitTestableElement { get { return this; } }
		protected internal override bool ShouldRotate {
			get {
				Axis axis = Axis;
				return axis != null && axis.ShouldRotateTitle;
			}
		}
		protected internal override double RotateAngle { get { return Axis.ActualAlignment == AxisAlignment.Far ? 90 : -90; } }
		public AxisTitle() {
			DefaultStyleKey = typeof(AxisTitle);
		}
		#region ISupportVisibilityControlElement implementation
		int ISupportVisibilityControlElement.Priority {
			get {
				if (Axis == null)
					return 100;
				if (Axis is AxisX2D || Axis is SecondaryAxisX2D)
					return (int)ChartElementVisibilityPriority.AxisXTitle;
				if (Axis is AxisY2D || Axis is SecondaryAxisY2D)
					return (int)ChartElementVisibilityPriority.AxisYTitle;
				return 100;
			}
		}
		bool ISupportVisibilityControlElement.Visible {
			get {
				if (!this.Visible.HasValue)
					return autoLayoutVisible;
				return this.Visible.Value;
			}
			set {
				if (!this.Visible.HasValue)
					autoLayoutVisible = value;
			}
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds {
			get {
				if (bounds != null && !bounds.IsEmpty)
					return new GRealRect2D(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
				return GRealRect2D.Empty;
			}
		}
		AxisTitlePresentation FindAxisTitlePresentation(XYDiagram2D diagram, AxisTitle axisTitle) {
			return LayoutHelper.FindElement(diagram,
				element => (element is AxisTitlePresentation) && ((AxisTitlePresentation)element).Title == axisTitle) as AxisTitlePresentation;
		}
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get {
				if (Axis == null)
					return VisibilityElementOrientation.Inside;
				if (!Axis.IsVertical)
					return VisibilityElementOrientation.Horizontal;
				return VisibilityElementOrientation.Vertical;
			}
		}		
		#endregion
	}
}
