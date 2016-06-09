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
using System.Reflection;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraDiagram.Bars {
	public struct DiagramCommandId : IConvertToInt<DiagramCommandId>, IEquatable<DiagramCommandId> {
		public static readonly DiagramCommandId None = CreateCommand();
		#region Document
		public static readonly DiagramCommandId Open = CreateCommand();
		public static readonly DiagramCommandId Save = CreateCommand();
		public static readonly DiagramCommandId SaveAs = CreateCommand();
		public static readonly DiagramCommandId Undo = CreateCommand();
		public static readonly DiagramCommandId Redo = CreateCommand();
		#endregion
		#region File
		public static readonly DiagramCommandId Paste = CreateCommand();
		public static readonly DiagramCommandId Cut = CreateCommand();
		public static readonly DiagramCommandId Copy = CreateCommand();
		#endregion
		#region Font
		public static readonly DiagramCommandId Font = CreateCommand();
		public static readonly DiagramCommandId FontSize = CreateCommand();
		public static readonly DiagramCommandId FontSizeIncrease = CreateCommand();
		public static readonly DiagramCommandId FontSizeDecrease = CreateCommand();
		public static readonly DiagramCommandId FontBold = CreateCommand();
		public static readonly DiagramCommandId FontItalic = CreateCommand();
		public static readonly DiagramCommandId FontUnderline = CreateCommand();
		public static readonly DiagramCommandId FontStrikethrough = CreateCommand();
		public static readonly DiagramCommandId FontColor = CreateCommand();
		public static readonly DiagramCommandId FontBackColor = CreateCommand();
		#endregion
		#region Paragraph 
		public static readonly DiagramCommandId AlignTop = CreateCommand();
		public static readonly DiagramCommandId AlignMiddle = CreateCommand();
		public static readonly DiagramCommandId AlignBottom = CreateCommand();
		public static readonly DiagramCommandId AlignLeft = CreateCommand();
		public static readonly DiagramCommandId AlignCenter = CreateCommand();
		public static readonly DiagramCommandId AlignRight = CreateCommand();
		public static readonly DiagramCommandId Justify = CreateCommand();
		#endregion
		#region Tools
		public static readonly DiagramCommandId PointerTool = CreateCommand();
		public static readonly DiagramCommandId ConnectorTool = CreateCommand();
		public static readonly DiagramCommandId ShapeTool = CreateCommand();
		public static readonly DiagramCommandId RectangleTool = CreateCommand();
		public static readonly DiagramCommandId EllipseTool = CreateCommand();
		public static readonly DiagramCommandId RightTriangleTool = CreateCommand();
		public static readonly DiagramCommandId HexagonTool = CreateCommand();
		#endregion
		#region Arrange
		public static readonly DiagramCommandId BringForward = CreateCommand();
		public static readonly DiagramCommandId BringToFront = CreateCommand();
		public static readonly DiagramCommandId SendBackward = CreateCommand();
		public static readonly DiagramCommandId SendToBack = CreateCommand();
		#endregion
		#region Show
		public static readonly DiagramCommandId ShowRuler = CreateCommand();
		public static readonly DiagramCommandId ShowGrid = CreateCommand();
		#endregion
		#region Appearance
		public static readonly DiagramCommandId SkinGallery = CreateCommand();
		#endregion
		static int lastCommandId = -1;
		readonly int value;
		public DiagramCommandId(int value) {
			this.value = value;
		}
		static DiagramCommandId CreateCommand() { return new DiagramCommandId(++lastCommandId); }
		public bool Equals(DiagramCommandId other) {
			return value == other.value;
		}
		public override bool Equals(object obj) {
			return ((obj is DiagramCommandId) && value == ((DiagramCommandId)obj).value);
		}
		public override int GetHashCode() {
			return value.GetHashCode();
		}
		int IConvertToInt<DiagramCommandId>.ToInt() { return value; }
		DiagramCommandId IConvertToInt<DiagramCommandId>.FromInt(int value) { return new DiagramCommandId(value); }
	}
}
