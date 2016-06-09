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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class LineStrip : List<GRealPoint2D>, IGeometryStrip {
		public bool IsEmpty { get { return Count < 2; } }
		public LineStrip() : base() {
		}
		public LineStrip(int pointsCount) : base(pointsCount) {
		}
		public LineStrip(IList<GRealPoint2D> points) : base(points) {
		}
		public void AddUniquePoint(GRealPoint2D point) {
			int count = Count;
			if (count == 0 || point != this[count - 1])
				Add(point);
		}
		public void AddStepToPoint(GRealPoint2D point, bool invertedStep) {
			int count = Count;
			if (count > 0) {
				GRealPoint2D previousPoint = this[count - 1];
				if (invertedStep)
					Add(new GRealPoint2D(previousPoint.X, point.Y));
				else
					Add(new GRealPoint2D(point.X, previousPoint.Y));
			}
			Add(point);
		}
		public virtual LineStrip CreateInstance() {
			return new LineStrip();
		}
		public virtual void CompleteFilling() {
		}
		public virtual LineStrip CreateUniqueStrip() {
			LineStrip uniqueStrip = new LineStrip();
			int lastIndex = -1;
			foreach (GRealPoint2D point in this)
				if (lastIndex < 0 || uniqueStrip[lastIndex] != point) {
					uniqueStrip.Add(point);
					lastIndex++;
				}
			return uniqueStrip;
		}
		public virtual void Extend(GRealPoint2D point, bool toLeft) {
			if (toLeft)
				Insert(0, point);
			else
				Add(point);
		}
		public virtual LineStrip ExtractSubStrip(int startIndex, int endIndex) {
			LineStrip strip = CreateInstance();
			for (int i = startIndex; i <= endIndex; i++)
				strip.Add(this[i]);
			return strip;
		}
		public virtual void Substiture(int startIndex, int endIndex, LineStrip lineStrip) {
			RemoveRange(startIndex, endIndex - startIndex + 1);
			InsertRange(startIndex, lineStrip);
		}
	}
}
