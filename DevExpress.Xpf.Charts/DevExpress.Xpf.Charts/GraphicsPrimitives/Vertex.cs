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

using System.Windows;
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public struct Vertex {
		public static Vertex Offset(Vertex vertex, double dx, double dy, double dz) {
			vertex.Offset(dx, dy, dz);
			return vertex;
		}
		Point3D position;
		Vector3D normal;
		Point textureCoord;
		public Point3D Position { get { return position; } }
		public Vector3D Normal { get { return normal; } }
		public Point TextureCoord { get { return textureCoord; } }
		public Vertex(Point3D position, Vector3D normal, Point textureCoord) {
			this.position = position;
			this.normal = normal;
			this.textureCoord = textureCoord;
		}
		public Vertex(Point3D position)
			: this(position, new Vector3D(), new Point()) {
		}
		public Vertex(Point3D position, Vector3D normal)
			: this(position, normal, new Point()) {
		}
		public Vertex(Point3D position, Point textureCoord)
			: this(position, new Vector3D(), textureCoord) {
		}
		public void Offset(double dx, double dy, double dz) {
			position = MathUtils.Offset(position, dx, dy, dz);
		}
	}
}
