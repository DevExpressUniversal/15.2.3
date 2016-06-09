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
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Editors.RangeControl {
	public enum RangeControlClientRegionType {
		Nothing,
		ItemInterval,
		GroupInterval
	}
	public class RangeControlClientHitTestResult {
		public static readonly RangeControlClientHitTestResult Nothing = new RangeControlClientHitTestResult(RangeControlClientRegionType.Nothing);
		public RangeControlClientRegionType RegionType { get; private set; }
		public object RegionStart { get; private set; }
		public object RegionEnd { get; private set; }
		public RangeControlClientHitTestResult(RangeControlClientRegionType regionType, object start = null, object end = null) {
			RegionType = regionType;
			RegionStart = start;
			RegionEnd = end;
		}
	}
	public interface IRangeControlClient {
		bool GrayOutNonSelectedRange { get;}
		bool AllowThumbs { get; }
		bool SnapSelectionToGrid { get; }
		bool SetVisibleRange(object visibleStart, object visibleEnd, Size viewportSize);
		bool SetSelectionRange(object selectionStart, object selectionEnd, Size viewportSize, bool isSnapped = true);
		bool SetRange(object start, object end, Size viewportSize);
		void Invalidate(Size viewportSize);
		event EventHandler<LayoutChangedEventArgs> LayoutChanged;
		bool ConvergeThumbsOnZoomingOut { get; }
		object Start { get; }
		object End { get; }
		object SelectionStart { get; }
		object SelectionEnd { get; }
		object VisibleStart { get; }
		object VisibleEnd { get; }
		Rect ClientBounds { get; }
		double GetComparableValue(object realValue);
		object GetRealValue(double comparable);
		string FormatText(object value);
		object GetSnappedValue(object value, bool isLeft);
		RangeControlClientHitTestResult HitTest(Point point);
	}
	public enum LayoutChangedType {
		Layout,
		Data
	}
	public class LayoutChangedEventArgs : EventArgs {
		public object Start { get; private set; }
		public object End { get; private set; }
		public LayoutChangedType ChangeType { get; private set; }
		public LayoutChangedEventArgs(LayoutChangedType changeType, object start = null, object end = null) {
			Start = start;
			End = end;
			ChangeType = changeType;
		}
	}
}
