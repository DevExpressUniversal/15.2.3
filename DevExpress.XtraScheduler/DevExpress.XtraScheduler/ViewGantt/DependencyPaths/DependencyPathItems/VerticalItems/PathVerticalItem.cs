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

using DevExpress.XtraScheduler.Internal.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class PathVerticalItem : PathVerticalItemBase {
		PathVerticalItemType type;
		public PathVerticalItem(Point start, Point end, IntersectionObjectsInfo intersectionObjects, PathVerticalItemType type, Rectangle availableBounds, GanttAppointmentViewInfoCollection pathOwners)
			: base(start, end, intersectionObjects, availableBounds, pathOwners) {
			this.type = type;
		}
		public PathVerticalItemType Type { get { return type; } set { type = value; } }
		public override PathItemBase Clone() {
			return new PathVerticalItem(Start, End, IntersectionObjects, Type, AvailableBounds, PathOwners);
		}
		protected override bool ValidateCore(Rectangle intersection) {
			int newPosition;
			if (type == PathVerticalItemType.MoveLeft) {
				newPosition = Math.Max(intersection.Left, AvailableBounds.Left);
				XtraSchedulerDebug.Assert(newPosition < Start.X);
			} else {
				newPosition = Math.Min(intersection.Right, AvailableBounds.Right);
				XtraSchedulerDebug.Assert(newPosition > Start.X);
			}
			SetStart(new Point(newPosition, Start.Y));
			SetEnd(new Point(newPosition, End.Y));
			return false;
		}
		protected override bool CanValidate() {
			if (Type == PathVerticalItemType.MoveLeft)
				return Start.X > AvailableBounds.Left;
			return Start.X < AvailableBounds.Right;
		}
	}
}
