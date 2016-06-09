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

namespace DevExpress.Xpf.Charts.RangeControlClient.Native {
	class RangeClientState {
		object defaultStart;
		object defaultEnd;
		object sparklineStart;
		object sparklineEnd;
		object rangeControlStart;
		object rangeControlEnd;
		object selectedStart;
		object selectedEnd;
		object visibleStart;
		object visibleEnd;
		bool isSetSparklineStart;
		bool isSetSparklineEnd;
		bool isSetWholeStart;
		bool isSetWholeEnd;
		bool isSetSelectedStart;
		bool isSetSelectedEnd;
		bool isSetVisibleStart;
		bool isSetVisibleEnd;
		internal object SparklineStart {
			get { return sparklineStart; }
			set {
				sparklineStart = value;
				isSetSparklineStart = true;
			}
		}
		internal object SparklineEnd {
			get { return sparklineEnd; }
			set {
				sparklineEnd = value;
				isSetSparklineEnd = true;
			}
		}
		internal object WholeStart {
			get { return isSetWholeStart ? rangeControlStart : ActualStart; }
			set {
				rangeControlStart = value;
				isSetWholeStart = true;
			}
		}
		internal object WholeEnd {
			get { return isSetWholeEnd ? rangeControlEnd : ActualEnd; }
			set {
				rangeControlEnd = value;
				isSetWholeEnd = true;
			}
		}
		internal object SelectedStart {
			get { return isSetSelectedStart ? selectedStart : ActualStart; }
			set {
				selectedStart = value;
				isSetSelectedStart = true;
			}
		}
		internal object SelectedEnd {
			get { return isSetSelectedEnd ? selectedEnd : ActualEnd; }
			set {
				selectedEnd = value;
				isSetSelectedEnd = true;
			}
		}
		internal object VisibleStart {
			get { return isSetVisibleStart ? visibleStart : ActualStart; }
			set {
				visibleStart = value;
				isSetVisibleStart = true;
			}
		}
		internal object VisibleEnd {
			get { return isSetVisibleEnd ? visibleEnd : ActualEnd; }
			set {
				visibleEnd = value;
				isSetVisibleEnd = true;
			}
		}
		internal bool IsRangeSet {
			get {
				return isSetWholeStart && isSetWholeEnd || isSetSparklineStart && isSetSparklineEnd;
			}
		}
		public object ActualStart { get { return isSetWholeStart ? WholeStart : isSetSparklineStart ? SparklineStart : defaultStart; } }
		public object ActualEnd { get { return isSetWholeEnd ? WholeEnd : isSetSparklineEnd ? SparklineEnd : defaultEnd; } }
		internal void SetDefaultRange(object start, object end) {
			defaultStart = start;
			defaultEnd = end;
		}
	}
}
