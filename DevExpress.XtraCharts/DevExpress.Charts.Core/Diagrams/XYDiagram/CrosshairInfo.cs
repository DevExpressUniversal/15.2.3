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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.Charts.Native {
	public enum CrosshairSnapModeCore {
		NearestArgument,
		NearestValue
	}
	public enum CrosshairLabelModeCore {
		ShowForEachSeries,
		ShowForNearestSeries,
		ShowCommonForAllSeries,
	}
	public enum PointPositionInSeries {
		Left,
		Center,
		Right
	}
	public class CrosshairAxisInfo {
		readonly string text;
		readonly GRealPoint2D anchorPoint;
		readonly GRealSize2D maxSize;
		readonly IAxisData axis;
		readonly object value;
		GRealSize2D size;
		public string Text { get { return text; } }
		public GRealPoint2D AnchorPoint { get { return anchorPoint; } }
		public IAxisData Axis { get { return axis; } }
		public GRealSize2D Size { get { return size; } set { size = value; } }
		public GRealSize2D MaxSize { get { return maxSize; } }
		public GRealRect2D Bounds {
			get {
				GRealSize2D actualSize = new GRealSize2D(Math.Min(maxSize.Width, size.Width), Math.Min(maxSize.Height, size.Height));
				return new GRealRect2D(anchorPoint.X - actualSize.Width / 2, anchorPoint.Y - actualSize.Height / 2, actualSize.Width, actualSize.Height);
			}
		}
		public object Value { get { return value; } }
		public CrosshairAxisInfo(string text, GRealPoint2D anchorPoint, GRealSize2D maxSize, IAxisData axis, object value) {
			this.text = text;
			this.anchorPoint = anchorPoint;
			this.maxSize = maxSize;
			this.axis = axis;
			this.value = value;
		}
	}
	public class CrosshairLine {
		readonly GRealLine2D line;
		readonly bool isValueLine;
		public GRealLine2D Line { get { return line; } }
		public bool IsValueLine { get { return isValueLine; } }
		public CrosshairLine(GRealLine2D line, bool isValueLine) {
			this.line = line;
			this.isValueLine = isValueLine;
		}
	}
}
