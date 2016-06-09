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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Gauges.Native {
	public abstract class ScaleLayout {
		readonly Rect initialBounds;
		public Rect InitialBounds { get { return initialBounds; } }
		public abstract Geometry Clip { get; }
		public abstract bool IsEmpty { get; }
		public ScaleLayout(Rect initialBounds) {
			this.initialBounds = initialBounds;
		}
	}
	public abstract class ScaleMapping {
		readonly Scale scale;
		readonly ScaleLayout layout;
		protected double ValuesRange { get { return Scale.ValuesRange; } }
		public Scale Scale { get { return scale; } }
		public ScaleLayout Layout { get { return layout; } }
		public ScaleMapping(Scale scale, ScaleLayout layout) {
			this.scale = scale;
			this.layout = layout;
		}
		protected abstract double GetAngleByPoint(Point point);
		public double GetAngleByPercent(double percent) {
			return GetAngleByPoint(GetPointByPercent(percent));
		}
		public double GetAngleByValue(double value, bool clamp) {
			if (clamp)
				return GetAngleByPoint(GetPointByValue(Scale.GetLimitedValue(value)));
			else
				return GetAngleByPoint(GetPointByValue(value));
		}
		public double GetAngleByValue(double value) {
			return GetAngleByValue(value, false);
		}
		public Point GetPointByValue(double value, bool clamp) {
			if (clamp)
				return GetPointByPercent(Scale.GetLimitedValueInPercent(value));
			else
				return GetPointByPercent(Scale.GetValueInPercent(value));
		}
		public Point GetPointByValue(double value) {
			return GetPointByValue(value, false);
		}
		public Point GetPointByValue(double value, double offset, bool clamp) {
			if (clamp)
				return GetPointByPercent(Scale.GetValueInPercent(Scale.GetLimitedValue(value)), offset);
			else
				return GetPointByPercent(Scale.GetValueInPercent(value), offset);
		}
		public Point GetPointByValue(double value, double offset) {
			return GetPointByValue(value, offset, false);
		}
		public abstract Point GetPointByPercent(double percent);
		public abstract Point GetPointByPercent(double percent, double offset);
		public abstract double? GetValueByPoint(Point point);
	}
}
