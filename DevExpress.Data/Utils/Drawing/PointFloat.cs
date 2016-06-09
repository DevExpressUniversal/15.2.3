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
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
namespace DevExpress.Utils {
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	[TypeConverter(typeof(PointFloatConverter))]
	public struct PointFloat {
		public static readonly PointFloat Empty = new PointFloat();
		private float x;
		private float y;
		public PointFloat(float x, float y) {
			this.x = x;
			this.y = y;
		}
		public PointFloat(PointF pt) : this(pt.X, pt.Y) {
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return ((this.x == 0f) && (this.y == 0f)); }
		}
		[TypeConverter(typeof(SingleTypeConverter))]
		[DXDisplayNameIgnore]
		public float X {
			get { return this.x; }
			set { this.x = value; }
		}
		[TypeConverter(typeof(SingleTypeConverter))]
		[DXDisplayNameIgnore]
		public float Y {
			get { return this.y; }
			set { this.y = value; }
		}
		public static PointFloat operator +(PointFloat pt, Size sz) {
			return new PointFloat(pt.X + sz.Width, pt.Y + sz.Height);
		}
		public static PointFloat operator -(PointFloat pt, Size sz) {
			return new PointFloat(pt.X - sz.Width, pt.Y - sz.Height);
		}
		public static bool operator ==(PointFloat left, PointFloat right) {
			return ((left.X == right.X) && (left.Y == right.Y));
		}
		public static bool operator !=(PointFloat left, PointFloat right) {
			return !(left == right);
		}
		public static implicit operator PointF(PointFloat point) {
			return new PointF((float)point.X, (float)point.Y);
		}
		public override bool Equals(object obj) {
			if(!(obj is PointFloat))
				return false;
			PointFloat point = (PointFloat)obj;
			return point.X == this.X && point.Y == this.Y && point.GetType().Equals(base.GetType());
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("{{X={0}, Y={1}}}", this.x, this.y);
		}
		public void Offset(float dx, float dy) {
			this.X += dx;
			this.Y += dy;
		}
	}
}
