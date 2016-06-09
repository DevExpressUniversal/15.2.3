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
using System.Windows.Forms;
using System.Collections;
using System.Security;
using DevExpress.Data.Helpers;
using System.Security.Permissions;
namespace DevExpress.XtraPrinting.Native {
	public class CursorStorage {
		static Stack<Cursor> cursors = new Stack<Cursor>();
		static object SyncRoot { get { return ((ICollection)cursors).SyncRoot; } }
		static bool? isSafeSubWindowsGranted;
		public static bool IsSafeSubWindowsGranted {
			get {
				if(!isSafeSubWindowsGranted.HasValue)
					isSafeSubWindowsGranted = SecurityHelper.IsPermissionGranted(new UIPermission(UIPermissionWindow.SafeSubWindows));
				return isSafeSubWindowsGranted.Value;
			}
		}
		public static void SetCursor(Cursor cursor) {
			lock(SyncRoot) {
				cursors.Push(Cursor.Current);
				SetCurrentCursor(cursor);
			}
		}
		public static void RestoreCursor() {
			lock(SyncRoot) {
				System.Diagnostics.Debug.Assert(cursors.Count > 0);
				SetCurrentCursor(cursors.Pop());
			}
		}
		public static void Clear() {
			lock(SyncRoot)
				cursors.Clear();
		}
		static void SetCurrentCursor(Cursor cursor) {
			if(!PSNativeMethods.AspIsRunning && IsSafeSubWindowsGranted)
				Cursor.Current = cursor;
		}
	}
}
