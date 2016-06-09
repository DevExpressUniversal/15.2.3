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

using System.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Gesture;
namespace DevExpress.XtraPivotGrid {
	class PivotGestureHelper : GestureHelper {
		Point start;
		Point end;
		public PivotGestureHelper(IPivotGestureClient owner)
		   : base(owner) {
		}
		public new IPivotGestureClient Owner { get { return (IPivotGestureClient)base.Owner; } }
		public Point Start { get { return start; } }
		public Point End { get { return end; } }
		protected override void OnGidBegin(ref NativeMethods.GESTUREINFO gi) {
			base.OnGidBegin(ref gi);
			this.start = Info.Start.Point;
		}
		protected override void GidPressAndTap(ref NativeMethods.GESTUREINFO gi) {
			base.GidPressAndTap(ref gi);
			this.end = Info.Start.Point;
			Owner.OnTwoFingerSelection(Start, End);
		}
		protected override void GidTwoFingerTap(ref NativeMethods.GESTUREINFO gi) {
			base.GidTwoFingerTap(ref gi);
			this.end = GetEnd(Info.Current.Point);
			Owner.OnTwoFingerSelection(Start, End);
		}
		Point GetEnd(Point middle) {
			int x = start.X + (middle.X - start.X) * 2;
			int y = start.Y + (middle.Y - start.Y) * 2;
			return new Point(x, y);
		}
	}
	interface IPivotGestureClient : IGestureClient {
		void OnTwoFingerSelection(Point start, Point end);
	}
}
