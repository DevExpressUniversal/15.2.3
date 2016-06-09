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
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Core {
	public enum ScrollDirection { None, Left, Top, Right, Bottom }
	public enum ScrollKind { SmallStep, LargeStep, ExactPosition }
	public delegate void ScrollingEventHandler(object sender, ScrollKind kind);
	public class ScrollParams {
		private double _Min;
		private double _Max;
		private double _PageSize;
		private double _Position;
		private double _LargeStep;
		private double _SmallStep;
		public void Assign(ScrollParams value) {
			Min = value.Min;
			Max = value.Max;
			PageSize = value.PageSize;
			Position = value.Position;
			LargeStep = value.LargeStep;
			SmallStep = value.SmallStep;
		}
		public void AssignTo(ScrollBar scrollBar) {
			scrollBar.Minimum = Min;
			scrollBar.Maximum = MaxPosition;
			scrollBar.ViewportSize = PageSize;
			scrollBar.Value = Position;
			scrollBar.SmallChange = SmallStep;
			scrollBar.LargeChange = LargeStep;
		}
		public double CorrectPosition(double position) {
			return Math.Max(Min, Math.Min(position, MaxPosition));
		}
		public void DoSmallStep(bool forward) {
			DoScroll(ScrollKind.SmallStep,
				delegate {
					if(forward)
						Position += SmallStep;
					else
						Position -= SmallStep;
				});
		}
		public void DoLargeStep(bool forward) {
			DoScroll(ScrollKind.LargeStep,
				delegate {
					if(forward)
						Position += LargeStep;
					else
						Position -= LargeStep;
				});
		}
		public void Scroll(double position, bool isRelative) {
			DoScroll(ScrollKind.ExactPosition,
				delegate {
					if(isRelative)
						RelativePosition = position;
					else
						Position = position;
				});
		}
		public double Min {
			get { return _Min; }
			set {
				if(_Min != value) {
					_Min = value;
					DoChanged();
				}
			}
		}
		public double Max {
			get { return _Max; }
			set {
				if(_Max != value) {
					_Max = value;
					DoChanged();
				}
			}
		}
		public double PageSize {
			get { return _PageSize; }
			set {
				if(_PageSize != value) {
					_PageSize = value;
					DoChanged();
				}
			}
		}
		public double Position {
			get { return CorrectPosition(_Position); }
			set {
				value = Math.Max(Min, value);
				value = Math.Min(value, Max);   
				if(_Position != value) {
					_Position = value;
					DoChanged();
				}
			}
		}
		public double SmallStep {
			get {
				if(_SmallStep == 0)
					return Math.Ceiling(PageSize / 10);
				else
					return _SmallStep;
			}
			set { _SmallStep = value; }
		}
		public double LargeStep {
			get {
				if(_LargeStep == 0)
					return PageSize;
				else
					return _LargeStep;
			}
			set { _LargeStep = value; }
		}
		public bool Enabled {
			get { return Max - Min - PageSize >= 1; }  
		}
		public double MaxPosition {
			get { return Math.Max(Min, Max - PageSize); }
		}
		public double RelativePageSize {
			get { return PageSize / (Max - Min); }
		}
		public double RelativePosition {
			get { return (Position - Min) / (Max - Min); }
			set { Position = Math.Round(Min + value * (Max - Min)); }
		}
		public event Action<ScrollParams> Change;
		public event ScrollingEventHandler Scrolling;
		protected delegate void ScrollProcedure();
		protected virtual void DoChanged() {
			if(Change != null)
				Change(this);
		}
		protected void DoScroll(ScrollKind kind, ScrollProcedure scroll) {
			var oldPosition = Position;
			scroll();
			if(Position != oldPosition)
				DoScrolled(kind);
		}
		protected virtual void DoScrolled(ScrollKind kind) {
			if(Scrolling != null)
				Scrolling(this, kind);
		}
	}
}
