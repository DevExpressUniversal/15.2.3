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
namespace DevExpress.XtraCharts.Native {
	public struct Vertex {
		public static implicit operator DiagramPoint(Vertex vertex) {
			return vertex.point;
		}
		DiagramPoint point;
		DiagramVector normal;
		Color color;
		public DiagramPoint Point { get { return point; } set { point = value; } }
		public DiagramVector Normal { get { return normal; } set { normal = value; } }
		public Color Color { get { return color; } set { color = value; } }
		public double X { get { return point.X; } set { point.X = value; } }
		public double Y { get { return point.Y; } set { point.Y = value; } }
		public double Z { get { return point.Z; } set { point.Z = value; } }
		public Vertex(DiagramPoint point, DiagramVector normal, Color color) {
			this.point = point;
			this.normal = normal;
			this.color = color;
		}
		public Vertex(DiagramPoint point, DiagramVector normal) : this(point, normal, Color.Black) { 
		}
		public void Offset(double dx, double dy, double dz) {
			point = DiagramPoint.Offset(point, dx, dy, dz);
		}
	}
}
