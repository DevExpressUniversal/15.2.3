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
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class ConstantLineItem : NotifyPropertyChangedObject {
		Visibility visibility;
		double offset;
		double length;
		DoubleCollection dashArray;
		double dashOffset;
		List<Rect> selectionGeometry;
		readonly ConstantLine constantLine;		
		internal bool IsVisibleForLayout { get { return !Double.IsNaN(constantLine.InternalValue); } }
		public ConstantLine ConstantLine { get { return constantLine; } }
		public Visibility Visibility {
			get { return visibility; }
			set { 
				visibility = value;
				OnPropertyChanged("Visibility");
			}
		}
		public double Offset {
			get { return offset; }
			set {
				offset = value;
				OnPropertyChanged("Offset");
			}
		}
		public double Length {
			get { return length; }
			set {
				length = value;
				OnPropertyChanged("Length");
			}
		}
		public DoubleCollection DashArray {
			get { return dashArray; }
			set {
				dashArray = value;
				OnPropertyChanged("DashArray");
			}
		}
		public double DashOffset {
			get { return dashOffset; }
			set {
				dashOffset = value;
				OnPropertyChanged("DashOffset");
			}
		}
		public List<Rect> SelectionGeometry {
			get { return selectionGeometry; }
			set {
				if (selectionGeometry != value) {
					selectionGeometry = value;
					OnPropertyChanged("SelectionGeometry");
				}
			}
		}
		internal ConstantLineItem(ConstantLine constantLine) {
			this.constantLine = constantLine;
		}
		RectangleGeometry CalcHittestableGeometry(Rect viewport, Rect? lastMappingBounds) {
			Rect bounds;
			int thickness = constantLine.ActualLineStyle.Thickness;
			int inflateValue = MathUtils.isEvenNumber(thickness) ? 6 : 5;
			if (constantLine.Axis.IsVertical) {
				Point location = new Point() {
					X = 0,
					Y = viewport.Height - Offset - 3 - thickness / 2
				};
				Size size = new Size(viewport.Width, thickness + inflateValue);
				bounds = new Rect(location, size);
			} else {
				Point location = new Point() {
					X = Offset - 3 - thickness / 2,
					Y = 0
				};
				Size size = new Size(thickness + inflateValue, viewport.Height);
				bounds = new Rect(location, size);
			}			
			if (lastMappingBounds.HasValue)
				bounds = new Rect(new Point(lastMappingBounds.Value.X + bounds.X, lastMappingBounds.Value.Y + bounds.Y), new Size(bounds.Width, bounds.Height));
			return new RectangleGeometry() { Rect = bounds };
		}
		List<Rect> CalcSelectionGeometry(Rect viewport) {
			List<Rect> result = new List<Rect>();
			int thickness = constantLine.ActualLineStyle.Thickness;
			int inflateValue = MathUtils.isEvenNumber(thickness) ? 6 : 5;
			Point location = new Point() {
				X = Offset - 3 - thickness / 2,
				Y = 1
			};
			double selectionLength = Length - 2;
			Size size = new Size(thickness + inflateValue, selectionLength > 0 ? selectionLength : 0);
			result.Add(new Rect(location, size));
			return result;
		}
		internal void UpdateLayout(Rect viewport, Rect? lastMappingBounds) {
			if (IsVisibleForLayout) {
				Axis2D axis = constantLine.Axis;
				IAxisMapping mapping = axis.CreateMapping(viewport);
				double axisValue = mapping.GetAxisValue(constantLine.InternalValue);
				Offset = Render2DHelper.CorrectLinePosition(axisValue, constantLine.ActualLineStyle.Thickness);
				Length = constantLine.Axis.IsVertical ? viewport.Width : viewport.Height;
				SelectionGeometry = CalcSelectionGeometry(viewport);
			}
			else {
				Offset = 0;
				Length = 0;
				SelectionGeometry = new List<Rect>();
			}
			LineStyle style = constantLine.ActualLineStyle;
			DashStyle dashStyle = style.DashStyle;
			if (dashStyle == null) {
				DashArray = CommonUtils.CloneDoubleCollection(null);
				DashOffset = 0;
			}
			else {
				DashArray = CommonUtils.CloneDoubleCollection(dashStyle.Dashes);
				DashOffset = dashStyle.Offset;
			}
		}
	}
}
