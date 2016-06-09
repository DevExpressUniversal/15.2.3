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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
#if SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Printing {
	public class CursorService : ICursorService {
		readonly CursorContainer cursorContainer;
		bool isServiceBlocked;
		bool isCursorChangingSuppressed;
		string blockId;
		public CursorService(CursorContainer cursorContainer) {
			Guard.ArgumentNotNull(cursorContainer, "cursorContainer");
			this.cursorContainer = cursorContainer;
		}
		#region ICursorService Members
		public bool SetCursor(FrameworkElement control, Cursor cursor) {
			return SetCursor(control, cursor, string.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetCursor(FrameworkElement control, Cursor cursor, string id) {
			if(isServiceBlocked && blockId != id || isCursorChangingSuppressed)
				return false;
			Guard.ArgumentNotNull(cursor, "cursor");
			cursorContainer.Visibility = Visibility.Collapsed;
			cursorContainer.CustomCursor = null;
			control.Cursor = cursor;
			return true;
		}
		public bool SetCursor(FrameworkElement control, CustomCursor customCursor) {
			return SetCursor(control, customCursor, string.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetCursor(FrameworkElement control, CustomCursor customCursor, string id) {
			if(isServiceBlocked && blockId != id || isCursorChangingSuppressed)
				return false;
			control.Cursor = Cursors.None;
			cursorContainer.CustomCursor = customCursor;
			cursorContainer.Visibility = Visibility.Visible;
			return true;
		}
		public bool SetCursorPosition(Point relativePosition, FrameworkElement relativeTo) {
			return SetCursorPosition(relativePosition, relativeTo, string.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetCursorPosition(Point relativePosition, FrameworkElement relativeTo, string id) {
			if(isServiceBlocked && blockId != id || isCursorChangingSuppressed)
				return false;
			if(cursorContainer.CustomCursor == null || cursorContainer.Visibility == Visibility.Collapsed)
				throw new InvalidOperationException();
			new OnLoadedScheduler().Schedule(() => {
				Point position = relativeTo.TranslatePoint(relativePosition, cursorContainer);
				cursorContainer.CursorPosition = position;
			}, relativeTo);
			return true;
		}
		public bool HideCustomCursor() {
			return HideCustomCursor(string.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HideCustomCursor(string id) {
			if(isServiceBlocked && blockId != id || isCursorChangingSuppressed)
				return false;
			cursorContainer.Visibility = Visibility.Collapsed;
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetSuppressCursorChanging(bool value) {
			if(isServiceBlocked)
				return false;
			isCursorChangingSuppressed = value;
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool BlockService(string id) {
			Guard.ArgumentIsNotNullOrEmpty(id, "id");
			if(isServiceBlocked)
				return false;
			else {
				blockId = id;
				isServiceBlocked = true;
				isCursorChangingSuppressed = false;
				return true;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool UnblockService(string id) {
			Guard.ArgumentIsNotNullOrEmpty(id, "id");
			if(!isServiceBlocked)
				return false;
			else {
				if(blockId != id)
					return false;
				else {
					blockId = string.Empty;
					isServiceBlocked = false;
					return true;
				}
			}
		}
		#endregion
	}
}
