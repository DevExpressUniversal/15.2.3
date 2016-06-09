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
using System.Drawing.Printing;
using System.Text;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.Drawing.Printing;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPrinting.Native {
#if !DXPORTABLE
	[TypeConverter(typeof(MarginsFConverter))]
#endif
	public class MarginsF : ICloneable {
		#region static
		static float scale = 3;
		public static int ToHundredths(float val) {
			return Convert.ToInt32(val / scale);
		}
		public static float FromHundredths(float val) {
			return val * scale;
		}
		public static Margins ToMargins(MarginsF value) {
			return new Margins(ToHundredths(value.Left), ToHundredths(value.Right), ToHundredths(value.Top), ToHundredths(value.Bottom));
		}
#endregion
		float left;
		float right;
		float top;
		float bottom;
		public float Left {
			get { return left; }
			set { left = value; }
		}
		public float Right {
			get { return right; }
			set { right = value; }
		}
		public float Top {
			get { return top; }
			set { top = value; }
		}
		public float Bottom {
			get { return bottom; }
			set { bottom = value; }
		}
		public Margins Margins {
			get { 
				return ToMargins(this);
			}
		}
		public MarginsF() {
		}
		public MarginsF(float left, float right, float top, float bottom) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		public MarginsF(Margins margins) {
			this.left = FromHundredths(margins.Left);
			this.right = FromHundredths(margins.Right);
			this.top = FromHundredths(margins.Top);
			this.bottom = FromHundredths(margins.Bottom);
		}
		public Margins Round() {
			return new Margins(Convert.ToInt32(left), Convert.ToInt32(right), Convert.ToInt32(top), Convert.ToInt32(bottom));
		}
		public object Clone() {
			return new MarginsF(Left, Right, Top, Bottom);
		}
		public override bool Equals(object obj) {
			MarginsF margins = obj as MarginsF;
			return margins != null && 
				left == margins.left && 
				right == margins.right &&
				top == margins.top &&
				bottom == margins.bottom;
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode((int)left, (int)right, (int)top, (int)bottom);
		}
	}
}
