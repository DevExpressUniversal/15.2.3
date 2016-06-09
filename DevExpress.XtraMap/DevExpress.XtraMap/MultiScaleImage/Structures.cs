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
namespace DevExpress.XtraMap.Native {
	public struct TileIndex {
		static readonly TileIndex invalid = new TileIndex(int.MinValue, int.MinValue, int.MinValue);
		public static TileIndex Invalid { get { return invalid; } }
		readonly int x;
		readonly int y;
		readonly int level;
		public int X { get { return x; } }
		public int Y { get { return y; }  }
		public int Level { get { return level; } }
		public static bool operator ==(TileIndex a, TileIndex b) {
			return a.Equals(b);
		}
		public static bool operator !=(TileIndex a, TileIndex b) {
			return !a.Equals(b);
		}
		public static bool IsInvalid(TileIndex index) {
			return index == Invalid;
		}
		public TileIndex(int x, int y, int level) {
			this.x = x;
			this.y = y;
			this.level = level;
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return String.Format("TileIndex({0}:{1}:{2})", x, y, level);
		}
	}
	public struct TileRange {
		readonly TileIndex min;
		readonly TileIndex max;
		public TileIndex Min { get { return min; } }
		public TileIndex Max { get { return max; } }
		public int Width { get { return max.X - min.X; } }
		public int Height { get { return max.Y - min.Y; } }
		public int Area { get { return Width * Height; } }
		public static bool operator ==(TileRange a, TileRange b) {
			return a.Equals(b);
		}
		public static bool operator !=(TileRange a, TileRange b) {
			return !a.Equals(b);
		}
		public TileRange(TileIndex min, TileIndex max) {
			this.min = min;
			this.max = max;
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return String.Format("TileRange(x({0}),y({1}),l({2}) - x({3}),y({4}),l({5}))", min.X, min.Y, min.Level, max.X, max.Y, max.Level);
		}
	}
	public struct IntSize {
		readonly int width;
		readonly int height;
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public int Area { get { return Width * Height; } }
		public static bool operator ==(IntSize a, IntSize b) {
			return a.Equals(b);
		}
		public static bool operator !=(IntSize a, IntSize b) {
			return !a.Equals(b);
		}
		public IntSize(int width, int height) {
			this.width = width;
			this.height = height;
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return String.Format("IntSize({0},{1})", width, height);
		}
	}
}
