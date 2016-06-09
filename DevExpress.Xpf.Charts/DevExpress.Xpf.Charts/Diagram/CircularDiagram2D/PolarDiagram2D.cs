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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public sealed class PolarDiagram2D : CircularDiagram2D {
		public static readonly DependencyProperty AxisXProperty = DependencyPropertyManager.Register("AxisX", typeof(PolarAxisX2D), typeof(PolarDiagram2D), new PropertyMetadata(AxisXPropertyChanged));
		public static readonly DependencyProperty AxisYProperty = DependencyPropertyManager.Register("AxisY", typeof(PolarAxisY2D), typeof(PolarDiagram2D), new PropertyMetadata(AxisYPropertyChanged));
		[
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public PolarAxisX2D AxisX {
			get { return (PolarAxisX2D)GetValue(AxisXProperty); }
			set { SetValue(AxisXProperty, value); }
		}
		[
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public PolarAxisY2D AxisY {
			get { return (PolarAxisY2D)GetValue(AxisYProperty); }
			set { SetValue(AxisYProperty, value); }
		}
		static void AxisXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PolarDiagram2D diagram = d as PolarDiagram2D;
			if (diagram != null) {
				PolarAxisX2D axis = e.NewValue as PolarAxisX2D;
				if (axis == null) {
					diagram.actualAxisX = new PolarAxisX2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisX);
				}
				else
					diagram.actualAxisX = axis;
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems);
		}
		static void AxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PolarDiagram2D diagram = d as PolarDiagram2D;
			if (diagram != null) {
				PolarAxisY2D axis = e.NewValue as PolarAxisY2D;
				if (axis == null) {
					diagram.actualAxisY = new PolarAxisY2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisY);
				}
				else
					diagram.actualAxisY = axis;
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems);
		}
		PolarAxisX2D actualAxisX;
		PolarAxisY2D actualAxisY;
		protected internal override CircularAxisX2D AxisXImpl { get { return actualAxisX; } }
		protected internal override CircularAxisY2D AxisYImpl { get { return actualAxisY; } }
		protected internal override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.PolarView; } }
		[Category(Categories.Elements)]
		public PolarAxisX2D ActualAxisX {
			get { return actualAxisX; }
		}
		[Category(Categories.Elements)]
		public PolarAxisY2D ActualAxisY {
			get { return actualAxisY; }
		}
		public PolarDiagram2D()
			: base() {
			DefaultStyleKey = typeof(PolarDiagram2D);
			actualAxisX = new PolarAxisX2D();
			actualAxisX.ChangeOwner(null, this);
			actualAxisY = new PolarAxisY2D();
			actualAxisY.ChangeOwner(null, this);
			EndInit();
		}
	}
}
