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
namespace DevExpress.Charts.Native {
	public class StripRange {
		readonly GRealPoint2D top;
		readonly GRealPoint2D bottom;
		public GRealPoint2D Top { get { return top; } }
		public GRealPoint2D Bottom { get { return bottom; } }
		public StripRange(GRealPoint2D top, GRealPoint2D bottom) {
			this.top = top;
			this.bottom = bottom;
		}
	}
	public class RangeStrip : IGeometryStrip {
		LineStrip topStrip;
		LineStrip bottomStrip;
		public int Count { 
			get { 
				EnsureStips();
				return Math.Min(topStrip.Count, bottomStrip.Count); 
			} 
		}
		public bool IsEmpty { get { return Count < 2; } }
		public LineStrip TopStrip {
			get {
				EnsureStips();
				return topStrip;
			}
			set { topStrip = value; } 
		}
		public LineStrip BottomStrip {
			get {
				EnsureStips();
				return bottomStrip;
			}
			set { bottomStrip = value; } 
		}
		public RangeStrip() {
		}
		protected virtual LineStrip CreateBorderStrip() {
			return new LineStrip();
		}
		protected void EnsureStips() {
			if (topStrip == null)
				topStrip = CreateBorderStrip();
			if (bottomStrip == null)
				bottomStrip = CreateBorderStrip();
		}
		public virtual RangeStrip CreateInstance() {
			return new RangeStrip();
		}
		public virtual void CompleteFilling() {
			EnsureStips();
			topStrip.CompleteFilling();
			bottomStrip.CompleteFilling();
		}
		public void Add(StripRange range) {
			EnsureStips();
			topStrip.Add(range.Top);
			bottomStrip.Add(range.Bottom);
		}
	}
}
