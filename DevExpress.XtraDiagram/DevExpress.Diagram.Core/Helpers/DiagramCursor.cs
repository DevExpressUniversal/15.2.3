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

using System.Windows;
using System.Windows.Input;
using WinCursor = System.Windows.Forms.Cursor;
using WinCursors = System.Windows.Forms.Cursors;
namespace DevExpress.Diagram.Core {
public static partial class DiagramCursorExtensions {
	static readonly Cursor ArrowCursor = GetCursor("Arrow.cur");
	static readonly WinCursor ArrowWinCursor = GetWinCursor("Arrow.cur");
	static readonly Cursor CopyCursor = GetCursor("Copy.cur");
	static readonly WinCursor CopyWinCursor = GetWinCursor("Copy.cur");
	static readonly Cursor CrossCursor = GetCursor("Cross.cur");
	static readonly WinCursor CrossWinCursor = GetWinCursor("Cross.cur");
	static readonly Cursor DrawConnectedConnectorCursor = GetCursor("DrawConnectedConnector.cur");
	static readonly WinCursor DrawConnectedConnectorWinCursor = GetWinCursor("DrawConnectedConnector.cur");
	static readonly Cursor DrawFreeConnectorCursor = GetCursor("DrawFreeConnector.cur");
	static readonly WinCursor DrawFreeConnectorWinCursor = GetWinCursor("DrawFreeConnector.cur");
	static readonly Cursor HoverRotationCursor = GetCursor("HoverRotation.cur");
	static readonly WinCursor HoverRotationWinCursor = GetWinCursor("HoverRotation.cur");
	static readonly Cursor MoveCursor = GetCursor("Move.cur");
	static readonly WinCursor MoveWinCursor = GetWinCursor("Move.cur");
	static readonly Cursor NoCursor = GetCursor("No.cur");
	static readonly WinCursor NoWinCursor = GetWinCursor("No.cur");
	static readonly Cursor RotationCursor = GetCursor("Rotation.cur");
	static readonly WinCursor RotationWinCursor = GetWinCursor("Rotation.cur");
	static readonly Cursor SizeAllCursor = GetCursor("SizeAll.cur");
	static readonly WinCursor SizeAllWinCursor = GetWinCursor("SizeAll.cur");
	static readonly Cursor SizeNESWCursor = GetCursor("SizeNESW.cur");
	static readonly WinCursor SizeNESWWinCursor = GetWinCursor("SizeNESW.cur");
	static readonly Cursor SizeNSCursor = GetCursor("SizeNS.cur");
	static readonly WinCursor SizeNSWinCursor = GetWinCursor("SizeNS.cur");
	static readonly Cursor SizeNWSECursor = GetCursor("SizeNWSE.cur");
	static readonly WinCursor SizeNWSEWinCursor = GetWinCursor("SizeNWSE.cur");
	static readonly Cursor SizeWECursor = GetCursor("SizeWE.cur");
	static readonly WinCursor SizeWEWinCursor = GetWinCursor("SizeWE.cur");
}
}
