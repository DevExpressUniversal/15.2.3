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

namespace DevExpress.Charts.Native {
	public class FibonacciLine {
		readonly double level;
		GRealPoint2D start;
		GRealPoint2D end;
		GRealPoint2D screenStart;
		GRealPoint2D screenEnd;
		public double Level { get { return level; } }
		public GRealPoint2D Start { get { return start; } set { start = value; } }
		public GRealPoint2D End   { get { return end;}	set { end   = value; } }
		public GRealPoint2D ScreenStart { get { return screenStart; } set { screenStart = value; } }
		public GRealPoint2D ScreenEnd   { get { return screenEnd; }   set { screenEnd   = value; } }
		public FibonacciLine(double level, GRealPoint2D start, GRealPoint2D end) {
			this.level = level;
			this.start = start;
			this.end = end;
		}
		public FibonacciLine(double level, double startX, double startY, double endX, double endY) {
			this.level = level;
			this.start = new GRealPoint2D(startX,startY);
			this.end = new GRealPoint2D(endX, endY);
		}
		public FibonacciLine(double level, GRealPoint2D startPoint, double endPointX, double endPointY) {
			this.level = level;
			this.start = startPoint;
			this.end = new GRealPoint2D(endPointX, endPointY);
		}
	}
}
