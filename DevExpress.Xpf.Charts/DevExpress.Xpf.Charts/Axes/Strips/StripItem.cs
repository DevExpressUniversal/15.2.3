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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class StripItem : NotifyPropertyChangedObject {
		const double minStripWidth = 1.0;
		double offset;
		double thickness;
		double lenghth;
		List<Rect> selectionGeometry;
		readonly Strip strip;
		internal Strip Strip {
			get { return strip; }
		}
		internal StripPresentation Presentation {
			get;
			set;
		}
		public double Offset {
			get { return offset; }
			set {
				offset = value;
				OnPropertyChanged("Offset");
			}
		}
		public double Thickness {
			get { return thickness; }
			set {
				thickness = value;
				OnPropertyChanged("Thickness");
			}
		}
		public double Length {
			get { return lenghth; }
			set {
				lenghth = value;
				OnPropertyChanged("Length");
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
		internal StripItem(Strip strip) {
			this.strip = strip;
		}
		internal void UpdateLayout(Rect viewport, Rect? lastMappingBounds) {
			Axis2D axis = Strip.Axis;
			if (axis == null || !Strip.GetActualVisible()) {
				Offset = 0;
				Thickness = 0;
				Length = 0;
				SelectionGeometry = new List<Rect>();
			}
			else {
				double minValue = ((IStrip)strip).MinLimit.Value;
				double maxValue = ((IStrip)strip).MaxLimit.Value;
				if (!Double.IsNaN(minValue) && !Double.IsNaN(maxValue)) {
					Transformation transformation = ((IAxisData)axis).AxisScaleTypeMap.Transformation;
					IAxisMapping axisMapping = axis.CreateMapping(viewport);
					double near = axisMapping.GetRoundedClampedAxisValue(transformation.TransformForward(minValue));
					double far = axisMapping.GetRoundedClampedAxisValue(transformation.TransformForward(maxValue));
					Render2DHelper.CorrectBounds(ref near, ref far);
					Render2DHelper.CorrectBoundsByMinDistance(ref near, ref far, minStripWidth);
					Offset = Math.Min(near, far);
					Thickness = Math.Abs(far - near);
				}
				else {
					Offset = 0.0;
					Thickness = 0.0;
				}
				Length = axis.IsVertical ? viewport.Width : viewport.Height;
				double selectionLength = Length - 2;
				SelectionGeometry = new List<Rect>() {new Rect(Offset, 1, Thickness, selectionLength > 0 ? selectionLength : 0)};
			}
		}
	}
}
