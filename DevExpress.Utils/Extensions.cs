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
using System.Drawing;
using DevExpress.Utils.Win.Hook;
using System.Windows.Forms;
namespace DevExpress.Utils.Extensions {
	public static class UtilsExtensions {
		internal static IntPtr LParamFromPoint(this HookManager.POINT pt) {
			Point point = new Point(pt.X, pt.Y);
			return LParamFromPoint(point);
		}
		public static IntPtr LParamFromPoint(this Point pt) {
			ushort x = unchecked((ushort)(short)pt.X);
			ushort y = unchecked((ushort)(short)pt.Y);
			return new IntPtr(unchecked((y << 16) | x));
		}
		public static Point PointFromLParam(this IntPtr lParam) {
			long value = lParam.ToInt64();
			short x = unchecked((short)(value & 0xFFFF));
			short y = unchecked((short)(((value & 0xFFFF0000) >> 16)));
			return new Point(x, y);
		}
		[Obsolete("Use EnumAllChildren"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static void EnumAllChildrens(this Control control, ChildControlsVisitor visitor) {
			EnumAllChildren(control, visitor);
		}
		public static void EnumAllChildren(this Control control, ChildControlsVisitor visitor) {
			EnumAllChildrenCore(control, visitor);
		}
		public delegate void ChildControlsVisitor(Control control);
		static void EnumAllChildrenCore(Control parent, ChildControlsVisitor visitor) {
			foreach(Control control in parent.Controls) {
				visitor(control);
				EnumAllChildrenCore(control, visitor);
			}
		}
		public static int GetBorderSize(this Form form) {
			return (form.Width - form.ClientSize.Width) / 2;
		}
		public static int GetCaptionHeight(this Form form) {
			return form.Height - form.ClientSize.Height - form.GetBorderSize();
		}
	}
}
