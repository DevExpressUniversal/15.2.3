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
namespace DevExpress.Xpf.Carousel {
	public class ItemTransfer {
		bool isActual = true;
		internal bool IsActual {
			get { return isActual; }
			set { isActual = value; }
		}
		IList<Range> ranges;
		public ItemTransfer(IList<Range> ranges) {
			this.ranges = ranges;
		}
		public double Distance {
			get {
				double distance;
				distance = 0;
				foreach(Range range in ranges) {
					distance += range.Distance;
				}
				return distance;
			}
		}
		public double GetPosition(double ratio) {
			if(!IsActual)
				return 0;
			double goneDistance = ratio * Distance;
			foreach(Range range in ranges) {
				if(Math.Abs(goneDistance) <= Math.Abs(range.Distance))
					return range.StartPosition + goneDistance;
				goneDistance -= range.Distance;
			}
			return 0d;
		}
		public void Truncate(double position) {
			List<Range> newRanges = new List<Range>();
			bool addLeftRanges = false;
			foreach(Range range in ranges) {
				if(addLeftRanges) {
					newRanges.Add(range);
					continue;
				}
				if(Distance > 0 && range.StartPosition <= position && position <= range.FinalPosition
					|| Distance < 0 && range.FinalPosition <= position && position <= range.StartPosition) {
					newRanges.Add(new Range(position, range.FinalPosition));
					addLeftRanges = true;
				}
			}
			ranges = newRanges;
		}
	}
}
