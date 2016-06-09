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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core {
	[TypeConverter(typeof(ArrowDescriptionTypeConverter))]
	public class ArrowDescription {
		#region DEBUGTEST
#if DEBUGTEST
		public static Point GetConnectionPointForTests(double width, double height, Shapes.ArrowConnectorKind kind) {
			return GetConnectionPoint(width, height, kind);
		}
#endif
		#endregion
		#region static
		public static ArrowDescription Create(string id, Func<string> getName, Shapes.ArrowTemplate template) {
			return new ArrowDescription(id, getName, (width, height) => template.GetShape(new Size(width, height)), (w, h) => GetConnectionPoint(w, h, template.ConnectorKind));
		}
		static Point GetConnectionPoint(double width, double height, Shapes.ArrowConnectorKind kind) {
			switch(kind) {
				case Shapes.ArrowConnectorKind.Left:
					return new Point(0, height / 2);
				case Shapes.ArrowConnectorKind.Right:
					return new Point(width, height / 2);
				case Shapes.ArrowConnectorKind.Center:
					return new Point(width / 2, height / 2);
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
		readonly string idCore;
		readonly Func<string> getName;
		readonly Func<double, double, ShapeGeometry> getShapeDelegate;
		readonly Func<double, double, Point> getConnectionPointDelegate;
		public string Id { get { return idCore; } }
		public string Name { get { return getName(); } }
		public ArrowDescription(string id, Func<string> getName, Func<double, double, ShapeGeometry> getShapeDelegate, Func<double, double, Point> getConnectionPointDelegate) {
			this.getName = getName;
			this.idCore = id;
			this.getShapeDelegate = getShapeDelegate;
			this.getConnectionPointDelegate = getConnectionPointDelegate;
		}
		public ShapeGeometry GetShapePoints(double width, double height) {
			return getShapeDelegate(width, height);
		}
		public Point GetConnectionPoint(double width, double height) {
			return getConnectionPointDelegate(width, height);
		}
		public override string ToString() {
			return Name;
		}
	}
	public static class ConnectionBridgeFactory {
		public static ShapeGeometry CreateBowBridge(Size size, Point position, double angle) {
			SweepDirection direction = GetBowArcSweepDirection(angle);
			return new ShapeGeometry(
				LineSegment.Create(-size.Width / 2, 0),
				ArcSegment.Create(size.Width / 2, 0, new Size(size.Width / 2, size.Height), direction)
			).Rotate(angle).Offset(position);
		}
		static SweepDirection GetBowArcSweepDirection(double angle) {
			double angleRad = angle * Math.PI / 180.0;
			double value = MathHelper.RemoveBias(Math.Cos(angleRad), 0d);
			if (value == 0d)
				value = Math.Sin(angleRad);
			return Math.Sign(value) > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
		}
	}
	public static partial class ArrowDescriptions {
		public static IEnumerable<ArrowDescription> Arrows { get { return arrowsCore.Values; } }
		static readonly Dictionary<string, ArrowDescription> arrowsCore = new Dictionary<string, ArrowDescription>();
		static ArrowDescriptions() {
			DiagramToolboxRegistrator.LoadArrows();
		}
		public static ArrowDescription RegisterArrow(string id, ArrowDescription arrow) {
			arrowsCore.Add(id, arrow);
			return arrow;
		}
		public static ArrowDescription GetArrow(string id) {
			ArrowDescription arrow = null;
			if(arrowsCore.TryGetValue(id, out arrow))
				return arrow;
			return null;
		}
	}
}
