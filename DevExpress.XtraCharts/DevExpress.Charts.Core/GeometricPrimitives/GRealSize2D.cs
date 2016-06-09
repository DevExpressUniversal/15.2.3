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

namespace DevExpress.Charts.Native {
	public struct GRealSize2D {
		public static bool operator ==(GRealSize2D size1, GRealSize2D size2) {
			return size1.width == size2.width && size1.height == size2.height;
		}
		public static bool operator !=(GRealSize2D size1, GRealSize2D size2) {
			return !(size1 == size2);
		}
		public static GRealSize2D Empty { get { return new GRealSize2D(double.NaN, double.NaN); } }
		double width;
		double height;
		public double Width { get { return width; } set { width = value; } }
		public double Height { get { return height; } set { height = value; } }
		public bool IsEmpty { get { return !GeometricUtils.IsValidDouble(width) || !GeometricUtils.IsValidDouble(height); } }
		public GRealSize2D(double width, double height) {
			this.width = width;
			this.height = height;
		}
		public override bool Equals(object obj) {
			return (obj is GRealSize2D) && (this == (GRealSize2D)obj || IsEmpty && ((GRealSize2D)obj).IsEmpty);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("Width: {0}; Height: {1}", width, height);
		}
	}
}
