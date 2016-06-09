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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class PathItem : PathItemBase {
		IntersectionObjectsInfo intersectionObjects;
		Point start;
		Point end;
		readonly Rectangle availableBounds;
		bool skipValidation;
		readonly GanttAppointmentViewInfoCollection pathOwners;
		protected PathItem(Point start, Point end, IntersectionObjectsInfo intersectionObjects, Rectangle availableBounds, GanttAppointmentViewInfoCollection pathOwners) {
			Guard.ArgumentNotNull(intersectionObjects, "intersectionObjects");
			this.intersectionObjects = intersectionObjects;
			this.start = start;
			this.end = end;
			this.availableBounds = availableBounds;
			this.pathOwners = pathOwners;
		}
		protected internal IntersectionObjectsInfo IntersectionObjects { get { return intersectionObjects; } set { intersectionObjects = value; } }
		public override Point Start { get { return start; } }
		public override Point End { get { return end; } }
		protected internal Rectangle AvailableBounds { get { return availableBounds; } }
		protected internal bool SkipValidation { get { return skipValidation; } set { skipValidation = value; } }
		public GanttAppointmentViewInfoCollection PathOwners { get { return pathOwners; } }
		public virtual bool Validate(RoundElementType roundType) {
			if (Start == End)
				return true;
			if (SkipValidation)
				return true;
			if (!CanValidate())
				return true;
			Rectangle intersection = IntersectionObjects.CalculateIntersections(Start, End, PathOwners, roundType);
			if (intersection == Rectangle.Empty)
				return true;
			return ValidateCore(intersection);
		}
		public override void Inverse() {
			Point oldStart = Start;
			this.start = End;
			this.end = oldStart;
		}
		protected abstract bool ValidateCore(Rectangle intersection);
		protected abstract bool CanValidate();
		protected internal override void SetStart(Point value) {
			this.start = value;
		}
		protected internal override void SetEnd(Point value) {
			this.end = value;
		}
	}
}
