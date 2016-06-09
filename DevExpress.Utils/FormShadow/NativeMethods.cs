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

namespace DevExpress.Utils.FormShadow.Helpers {
	public class NativeHelper {
		public const int WM_DISPLAYCHANGE = 126;
		public const int WM_NCXBUTTONDOWN = 171;
		public const int WM_NCXBUTTONDBLCLK = 173;
		public const int WS_EX_NOACTIVATE = 0x08000000;
		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_EX_TOOLWINDOW = 0x00000080;
		public const int WS_EX_TRANSPARENT = 0x00000020;
		public const int WS_VISIBLE = 0x10000000;
		public const int WS_MINIMIZE = 0x20000000;
		public const int WS_CHILDWINDOW = 0x40000000;
		public const int WS_CLIPCHILDREN = 0x02000000;
		public const int WS_DISABLED = 0x08000000;
		public const int WS_POPUP = int.MinValue;
		#region SetWindowLong
		public const int GWL_WNDPROC = -4;
		public const int GWL_HWNDPARENT = -8;
		#endregion
		public const int WA_ACTIVE = 1;
		public const int WA_CLICKACTIVE = 2;
		internal static int LOWORD(System.IntPtr ptr) {
			return GetLowWord(GetInt(ptr));
		}
		internal static int HIWORD(System.IntPtr ptr) {
			return GetHighWord(GetInt(ptr));
		}
		public static System.Drawing.Point GetPoint(System.IntPtr ptr) {
			return new System.Drawing.Point(GetInt(ptr));
		}
		internal static int GetInt(System.IntPtr ptr) {
			return System.IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
		}
		internal static int GetXlParam(int lParam) {
			return GetLowWord(lParam);
		}
		internal static int GetYlParam(int lParam) {
			return GetHighWord(lParam);
		}
		internal static int GetLowWord(int value) {
			return (value & 0xffff);
		}
		internal static int GetHighWord(int value) {
			return ((value >> 0x10) & 0xffff);
		}
	}
}
