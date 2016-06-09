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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core;
namespace DevExpress.Xpf.Diagram {
	public abstract class RulerScaleBase : Control {
		public static readonly DependencyProperty ZoomProperty;
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty LocationProperty;
		public static readonly DependencyProperty ViewportProperty;
		public static readonly DependencyProperty MeasureUnitProperty;
		static RulerScaleBase() {
			var registrator = DependencyPropertyRegistrator<RulerScaleBase>.New();
			registrator
				.AddOwner(x => x.Zoom, out ZoomProperty, Ruler.ZoomProperty, 1, x => x.UpdatePadding())
				.AddOwner(x => x.Offset, out OffsetProperty, Ruler.OffsetProperty, default(Point), x => x.UpdatePadding())
				.AddOwner(x => x.Location, out LocationProperty, Ruler.LocationProperty, default(Point), x => x.UpdatePadding())
				.AddOwner(x => x.Viewport, out ViewportProperty, Ruler.ViewportProperty, default(Size), x => x.UpdatePadding())
				.Register(d => d.MeasureUnit, out MeasureUnitProperty, MeasureUnits.Pixels)
				.OverrideMetadata(WidthProperty, x => x.UpdatePadding())
				.OverrideMetadata(HeightProperty, x => x.UpdatePadding())
				;
		}
		protected readonly Orientation orientation;
		public RulerScaleBase(Orientation orientation, DiagramControl diagram) {
			this.orientation = orientation;
			this.SetBinding(MeasureUnitProperty, new Binding(DiagramControl.MeasureUnitProperty.Name) { Source = diagram });
		}
		public double Zoom {
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		public Point Offset {
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public Point Location {
			get { return (Point)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public Size Viewport {
			get { return (Size)GetValue(ViewportProperty); }
			set { SetValue(ViewportProperty, value); }
		}
		public MeasureUnit MeasureUnit {
			get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		protected void UpdatePadding() {
			if(!double.IsNaN(orientation.GetSize(this))) {
				var nearPadding = GetNearPadding();
				var farPadding = GetFarPadding();
				Padding = orientation.MakeThickness(nearPadding, farPadding);
				orientation.SetMinSize(this, Math.Max(0, orientation.GetSize(this) - farPadding));
			}
		}
		protected virtual double GetFarPadding() {
			return 0;
		}
		protected virtual double GetNearPadding() {
			return 0;
		}
	}
}
