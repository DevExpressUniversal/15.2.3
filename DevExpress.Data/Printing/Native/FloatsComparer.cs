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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public class FloatsComparer {
		static FloatsComparer() {
			Default = new FloatsComparer(0.001);
		}
		public static FloatsComparer Default;
		double epsilon;
		protected FloatsComparer(double epsilon) {
			this.epsilon = epsilon;
		}
		public bool FirstEqualsSecond(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon) == 0;
		}
		public bool FirstLessSecond(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon) < 0;
		}
		public bool FirstLessOrEqualSecond(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon) <= 0;
		}
		public bool FirstGreaterSecond(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon) > 0;
		}
		public bool FirstGreaterOrEqualSecond(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon) >= 0;
		}
		public bool FirstGreaterSecondLessThird(double first, double second, double third) {
			return FirstGreaterSecond(first, second) && FirstLessSecond(first, third);
		}
		public bool SizeFEquals(SizeF size1, SizeF size2) {
			return ComparingUtils.CompareDoubles(size1.Width, size2.Width, epsilon) == 0 && ComparingUtils.CompareDoubles(size1.Height, size2.Height, epsilon) == 0;
		}
		public bool RectangleIsEmpty(RectangleF rect) {
			return FirstGreaterSecond(rect.Width, 0) ? (FirstLessSecond(rect.Height, 0) || FirstEqualsSecond(rect.Height, 0)) : true;
		}
		public int CompareDoubles(double first, double second) {
			return ComparingUtils.CompareDoubles(first, second, epsilon);
		}
	}
}
