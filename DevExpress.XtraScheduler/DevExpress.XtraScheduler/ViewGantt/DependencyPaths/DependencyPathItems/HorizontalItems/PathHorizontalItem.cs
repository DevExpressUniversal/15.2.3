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
	public class PathHorizontalItem : PathHorizontalItemBase {
		PathHorizontalItemType type;
		public PathHorizontalItem(Point start, Point end, IntersectionObjectsInfo intersectionObjects, PathHorizontalItemType type, Rectangle availableBounds, GanttAppointmentViewInfoCollection pathOwners)
			: base(start, end, intersectionObjects, availableBounds, pathOwners) {
			this.type = type;
		}
		public PathHorizontalItemType Type { get { return type; } }
		protected override bool ValidateCore(Rectangle intersection) {
			int newPosition;
			if (Type == PathHorizontalItemType.MoveUp) {
				newPosition = Math.Max(intersection.Top, AvailableBounds.Top);
				XtraSchedulerDebug.Assert(newPosition < Start.Y);
			} else {
				newPosition = Math.Min(intersection.Bottom, AvailableBounds.Bottom);
				XtraSchedulerDebug.Assert(newPosition > Start.Y);
			}
			SetStart(new Point(Start.X, newPosition));
			SetEnd(new Point(End.X, newPosition));
			return false;
		}
		protected override bool CanValidate() {
			if (Type == PathHorizontalItemType.MoveUp)
				return Start.Y > AvailableBounds.Top;
			return Start.Y < AvailableBounds.Bottom;
		}
		public override PathItemBase Clone() {
			return new PathHorizontalItem(Start, End, IntersectionObjects, Type, AvailableBounds, PathOwners);
		}
	}
}
