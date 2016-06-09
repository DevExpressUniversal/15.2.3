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
using System.Reflection;
using System.Windows.Forms;
using System.IO;
namespace DevExpress.Utils
{
	public sealed class CursorsHelper {
		internal static Cursor LoadFromResource(string resourceName) {
			return LoadFromResource(resourceName, System.Reflection.Assembly.GetExecutingAssembly());
		}
		public static Cursor LoadFromResource(string resourceName, Assembly assembly) {
			using(Stream stream = assembly.GetManifestResourceStream(resourceName)) {
				try {
					return new Cursor(stream);
				} catch {
					return Cursors.Default;
				}
			}
		}
	}
	public sealed class DragCursors {
		public static readonly Cursor MoveCursor;
		public static readonly Cursor HandCursor;
		public static readonly Cursor HandDragCursor;
		public static readonly Cursor ZoomInCursor;
		public static readonly Cursor ZoomOutCursor;
		public static readonly Cursor ZoomLimitCursor;
		public static readonly Cursor RotateCursor;
		public static readonly Cursor RotateDragCursor;
		static DragCursors() {
			const string cursorsPath = "DevExpress.Utils.Cursors.Cursor{0}.cur";
			MoveCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "Move"));
			HandCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "Hand"));
			HandDragCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "HandDrag"));
			ZoomInCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "ZoomIn"));
			ZoomOutCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "ZoomOut"));
			ZoomLimitCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "ZoomLimit"));
			RotateCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "Rotate"));
			RotateDragCursor = CursorsHelper.LoadFromResource(String.Format(cursorsPath, "RotateDrag"));
		}
		DragCursors() {
		}
	}
}
