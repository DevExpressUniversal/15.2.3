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
	public sealed class RadarDiagram2D : CircularDiagram2D {
		public static readonly DependencyProperty AxisXProperty = DependencyPropertyManager.Register("AxisX", typeof(RadarAxisX2D), typeof(RadarDiagram2D), new PropertyMetadata(AxisXPropertyChanged));
		public static readonly DependencyProperty AxisYProperty = DependencyPropertyManager.Register("AxisY", typeof(RadarAxisY2D), typeof(RadarDiagram2D), new PropertyMetadata(AxisYPropertyChanged));
		[
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public RadarAxisX2D AxisX {
			get { return (RadarAxisX2D)GetValue(AxisXProperty); }
			set { SetValue(AxisXProperty, value); }
		}
		[
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public RadarAxisY2D AxisY {
			get { return (RadarAxisY2D)GetValue(AxisYProperty); }
			set { SetValue(AxisYProperty, value); }
		}
		static void AxisXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarDiagram2D diagram = d as RadarDiagram2D;
			if (diagram != null) {
				RadarAxisX2D axis = e.NewValue as RadarAxisX2D;
				if (axis == null) {
					diagram.actualAxisX = new RadarAxisX2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisX);
				}
				else
					diagram.actualAxisX = axis;
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems);
		}
		static void AxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarDiagram2D diagram = d as RadarDiagram2D;
			if (diagram != null) {
				RadarAxisY2D axis = e.NewValue as RadarAxisY2D;
				if (axis == null) {
					diagram.actualAxisY = new RadarAxisY2D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisY);
				}
				else
					diagram.actualAxisY = axis;
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.UpdateXYDiagram2DItems);
		}
		RadarAxisX2D actualAxisX;
		RadarAxisY2D actualAxisY;
		protected internal override CircularAxisX2D AxisXImpl { get { return actualAxisX; } }
		protected internal override CircularAxisY2D AxisYImpl { get { return actualAxisY; } }
		protected internal override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.RadarView; } }
		[Category(Categories.Elements)]
		public RadarAxisX2D ActualAxisX {
			get { return actualAxisX; }
		}
		[Category(Categories.Elements)]
		public RadarAxisY2D ActualAxisY {
			get { return actualAxisY; }
		}
		public RadarDiagram2D() : base() {
			DefaultStyleKey = typeof(RadarDiagram2D);
			actualAxisX = new RadarAxisX2D();
			actualAxisX.ChangeOwner(null, this);
			actualAxisY = new RadarAxisY2D();
			actualAxisY.ChangeOwner(null, this);
			EndInit();
		}
	}
}
