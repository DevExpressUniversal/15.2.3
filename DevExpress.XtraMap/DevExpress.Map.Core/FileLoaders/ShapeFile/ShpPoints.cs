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
namespace DevExpress.Map.Native {
	public struct ShpPoint {
		double x;
		double y;
		public double X {
			get { return x; }
			set { x = value; }
		}
		public double Y {
			get { return y; }
			set { y = value; }
		}
	}
	public struct ShpPointM {
		double x;
		double y;
		double m;
		public double M {
			get { return m; }
			set { m = value; }
		}
		public double X {
			get { return x; }
			set { x = value; }
		}
		public double Y {
			get { return y; }
			set { y = value; }
		}
		public ShpPointM(ShpPoint point) {
			x = point.X;
			y = point.Y;
			m = 0;
		}
	}
	public struct ShpPointZ {
		double x;
		double y;
		double z;
		double m;
		public double M {
			get { return m; }
			set { m = value; }
		}
		public double Z {
			get { return z; }
			set { z = value; }
		}
		public double X {
			get { return x; }
			set { x = value; }
		}
		public double Y {
			get { return y; }
			set { y = value; }
		}
		public ShpPointZ(ShpPoint point) {
			x = point.X;
			y = point.Y;
			z = 0;
			m = 0;
		}
	}
}
