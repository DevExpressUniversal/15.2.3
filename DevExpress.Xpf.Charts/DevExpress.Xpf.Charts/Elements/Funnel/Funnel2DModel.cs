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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class Funnel2DKind : PredefinedElementKind {
		internal static readonly List<Funnel2DKind> List = new List<Funnel2DKind>();
		static Funnel2DKind() {
			Add(typeof(Funnel2DModel));
		}
		static void Add(Type type) {
			List.Add(new Funnel2DKind(type, ((Funnel2DModel)Activator.CreateInstance(type)).ModelName));
		}
		public Funnel2DKind(Type type, string name)
			: base(type, name) {
		}
	}
	public class Funnel2DModel : PointModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Funnel2DModelModelName")]
#endif
		public virtual string ModelName { get { return "Simple"; } }
		protected virtual Funnel2DModel CreateObjectForClone() {
			return new Funnel2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SimpleFunnel2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DModel();
		}
		protected internal Funnel2DModel CloneModel() {
			Funnel2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
		protected virtual void Assign(Funnel2DModel model) { }
	}
	public class SimpleFunnel2DModelControl : PredefinedPie2DModelControl, INotifyPropertyChanged {
		public static readonly DependencyProperty BorderProperty = DependencyPropertyManager.Register("Border",
			typeof(SeriesBorder), typeof(SimpleFunnel2DModelControl), new PropertyMetadata(null, BorderChanged));
		public static readonly DependencyProperty PointGeometryProperty = DependencyPropertyManager.Register("PointGeometry",
			typeof(Geometry), typeof(SimpleFunnel2DModelControl), new PropertyMetadata(null));
		public static readonly DependencyProperty BorderGeometryProperty = DependencyPropertyManager.Register("BorderGeometry",
			typeof(Geometry), typeof(SimpleFunnel2DModelControl), new PropertyMetadata(null));
		private static void BorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SimpleFunnel2DModelControl modelControl = d as SimpleFunnel2DModelControl;
			if (modelControl != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				modelControl.ActualBorder = newBorder;
			}
		}
		double opacity;
		SolidColorBrush defaultBrush = new SolidColorBrush(Colors.Black);
		SeriesBorder actualBorder;
		[
			Category(Categories.Presentation)
		]
		public SeriesBorder Border {
			get { return (SeriesBorder)GetValue(BorderProperty); }
			set { SetValue(BorderProperty, value); }
		}
		[
			Category(Categories.Presentation)
		]
		public Geometry PointGeometry {
			get { return (Geometry)GetValue(PointGeometryProperty); }
			set { SetValue(PointGeometryProperty, value); }
		}
		[
			Category(Categories.Presentation)
		]
		public Geometry BorderGeometry {
			get { return (Geometry)GetValue(BorderGeometryProperty); }
			set { SetValue(BorderGeometryProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public double ActualOpacity {
			get { return opacity;  }
			set {
				if (opacity != value) {
					opacity = value;
					if (PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("ActualBorder"));
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SeriesBorder ActualBorder {
			get { return actualBorder; }
			set {
				SetActualBorder(value);
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("ActualBorder"));
			}
		}
		public SimpleFunnel2DModelControl() {
			DefaultStyleKey = typeof(SimpleFunnel2DModelControl);
		}
		void SetActualBorder(SeriesBorder value) {
			if (value != null) {
				actualBorder = new SeriesBorder();
				if (value.Brush != null) {
					actualBorder.Brush = value.Brush;
					ActualOpacity = 1;
				}
				else {
					actualBorder.Brush = defaultBrush;
					ActualOpacity = 0.2;
				}
				if (value.LineStyle != null)
					actualBorder.LineStyle = new LineStyle() {
						DashCap = value.LineStyle.DashCap,
						DashStyle = value.LineStyle.DashStyle.Clone(),
						LineJoin = value.LineStyle.LineJoin,
						MiterLimit = value.LineStyle.MiterLimit,
						Thickness = value.LineStyle.Thickness * 2,
					};
			}
			else
				actualBorder = null;
		}
		internal override void SetPointItemBinding(SeriesPointItem pointItem) {
			base.SetPointItemBinding(pointItem);
			if (pointItem != null) {
				FunnelSeries2D funnelSeries = pointItem.Series as FunnelSeries2D;
				if (funnelSeries != null)
					SetBinding(BorderProperty, new Binding("ActualBorder") { Source = funnelSeries });
			}
			else {
				ClearValue(BorderProperty);
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		internal void SetGeometries(Geometry pointGeometry, Geometry borderGeometry) {
			PointGeometry = pointGeometry;
			BorderGeometry = borderGeometry;
		}
	} 
}
