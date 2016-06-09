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
using System.Collections;
using System.Drawing;
namespace DevExpress.XtraPrinting.Shape.Native {
	public class ShapeCommandCollection : CollectionBase {
		Type[] allowedTypes;
		public ShapeCommandBase this[int index] {
			get { return (ShapeCommandBase)List[index]; }
		}
		public ShapeCommandCollection()
			: this(new Type[] { }) {
		}
		protected ShapeCommandCollection(Type[] allowedTypes) {
			this.allowedTypes = allowedTypes;
		}
		public void AddLine(PointF startPoint, PointF endPoint) {
			Add(new ShapeLineCommand(startPoint, endPoint));
		}
		public void AddBezier(PointF startPoint, PointF startControlPoint, PointF endControlPoint, PointF endPoint) {
			Add(new ShapeBezierCommand(startPoint, startControlPoint, endControlPoint, endPoint));
		}
		public void Iterate(IShapeCommandVisitor visitor) {
			foreach(ShapeCommandBase command in this) {
				command.Accept(visitor);
			}
		}
		public void Add(ShapeCommandBase command) {
			if(allowedTypes.Length > 0 && ((IList)allowedTypes).IndexOf(command.GetType()) == -1)
				throw new ArgumentException();
			List.Add(command);
		}
		public void AddRange(ShapeCommandCollection commands) {
			foreach(ShapeCommandBase command in commands) {
				Add(command);
			}
		}
		public void ScaleAt(PointF scaleCenter, float scaleFactorX, float scaleFactorY) {
			Iterate(new ShapeCommandsScaler(scaleCenter, scaleFactorX, scaleFactorY));
		}
		public void RotateAt(PointF rotateCenter, int angle) {
			Iterate(new ShapeCommandsRotator(rotateCenter, angle));
		}
		public void Offset(PointF offset) {
			Iterate(new ShapeCommandsOfsetter(offset));
		}
		public CriticalPointsCalculator GetCriticalPointsCalculator() {
			CriticalPointsCalculator criticalPointsCalculator = new CriticalPointsCalculator();
			Iterate(criticalPointsCalculator);
			return criticalPointsCalculator;
		}
	}
	public class ShapeLineCommandCollection : ShapeCommandCollection {
		public ShapeLineCommandCollection()
			: base(new Type[] { typeof(ShapeLineCommand) }) {
		}
	}
	public class ShapeBezierCommandCollection : ShapeCommandCollection {
		public ShapeBezierCommandCollection()
			: base(new Type[] { typeof(ShapeBezierCommand) }) {
		}
	}
	public class ShapePointsCommandCollection : ShapeCommandCollection {
		public ShapePointsCommandCollection()
			: base(new Type[] { typeof(ShapeLineCommand), typeof(ShapeBezierCommand) }) {
		}
	}
	public class ShapePathCommandCollection : ShapeCommandCollection {
		public ShapePathCommandCollection()
			: base(new Type[] { typeof(ShapeFillPathCommand), typeof(ShapeDrawPathCommand) }) {
		}
	}
}
