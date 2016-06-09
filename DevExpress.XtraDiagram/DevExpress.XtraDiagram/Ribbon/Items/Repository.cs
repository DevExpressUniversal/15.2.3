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
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraDiagram.Bars {
	public class DiagramBarCommandRepository : DiagramCommandRepositoryCore {
		protected internal DiagramBarCommandRepository(DiagramControl diagram) : base(diagram) {
			AddCommandConstructor(DiagramCommandId.Open, typeof(DiagramOpenDocumentCommand));
			AddCommandConstructor(DiagramCommandId.Save, typeof(DiagramSaveDocumentCommand));
			AddCommandConstructor(DiagramCommandId.SaveAs, typeof(DiagramSaveDocumentAsCommand));
			AddCommandConstructor(DiagramCommandId.Undo, typeof(DiagramUndoCommand));
			AddCommandConstructor(DiagramCommandId.Redo, typeof(DiagramRedoCommand));
			AddCommandConstructor(DiagramCommandId.Paste, typeof(DiagramPasteCommand));
			AddCommandConstructor(DiagramCommandId.Cut, typeof(DiagramCutCommand));
			AddCommandConstructor(DiagramCommandId.Copy, typeof(DiagramCopyCommand));
			AddCommandConstructor(DiagramCommandId.Font, typeof(DiagramFontCommand));
			AddCommandConstructor(DiagramCommandId.FontSize, typeof(DiagramFontSizeCommand));
			AddCommandConstructor(DiagramCommandId.FontSizeIncrease, typeof(DiagramFontSizeIncreaseCommand));
			AddCommandConstructor(DiagramCommandId.FontSizeDecrease, typeof(DiagramFontSizeDecreaseCommand));
			AddCommandConstructor(DiagramCommandId.FontBold, typeof(DiagramFontBoldCommand));
			AddCommandConstructor(DiagramCommandId.FontItalic, typeof(DiagramFontItalicCommand));
			AddCommandConstructor(DiagramCommandId.FontUnderline, typeof(DiagramFontUnderlineCommand));
			AddCommandConstructor(DiagramCommandId.FontStrikethrough, typeof(DiagramFontStrikethroughCommand));
			AddCommandConstructor(DiagramCommandId.FontColor, typeof(DiagramFontColorCommand));
			AddCommandConstructor(DiagramCommandId.FontBackColor, typeof(DiagramFontBackColorCommand));
			AddCommandConstructor(DiagramCommandId.AlignTop, typeof(DiagramParagraphAlignTopCommand));
			AddCommandConstructor(DiagramCommandId.AlignMiddle, typeof(DiagramParagraphAlignMiddleCommand));
			AddCommandConstructor(DiagramCommandId.AlignBottom, typeof(DiagramParagraphAlignBottomCommand));
			AddCommandConstructor(DiagramCommandId.AlignLeft, typeof(DiagramParagraphAlignLeftCommand));
			AddCommandConstructor(DiagramCommandId.AlignCenter, typeof(DiagramParagraphAlignCenterCommand));
			AddCommandConstructor(DiagramCommandId.AlignRight, typeof(DiagramParagraphAlignRightCommand));
			AddCommandConstructor(DiagramCommandId.PointerTool, typeof(DiagramPointerToolCommand));
			AddCommandConstructor(DiagramCommandId.ConnectorTool, typeof(DiagramConnectorToolCommand));
			AddCommandConstructor(DiagramCommandId.ShapeTool, typeof(DiagramShapeToolSelectionCommand));
			AddCommandConstructor(DiagramCommandId.RectangleTool, typeof(DiagramRectangleToolCommand));
			AddCommandConstructor(DiagramCommandId.EllipseTool, typeof(DiagramEllipseToolCommand));
			AddCommandConstructor(DiagramCommandId.RightTriangleTool, typeof(DiagramRightTriangleToolCommand));
			AddCommandConstructor(DiagramCommandId.HexagonTool, typeof(DiagramHexagonToolCommand));
			AddCommandConstructor(DiagramCommandId.SendBackward, typeof(DiagramSendBackwardCommand));
			AddCommandConstructor(DiagramCommandId.SendToBack, typeof(DiagramSendToBackCommand));
			AddCommandConstructor(DiagramCommandId.BringForward, typeof(DiagramBringForwardCommand));
			AddCommandConstructor(DiagramCommandId.BringToFront, typeof(DiagramBringToFrontCommand));
			AddCommandConstructor(DiagramCommandId.ShowRuler, typeof(DiagramShowRulerCommand));
			AddCommandConstructor(DiagramCommandId.ShowGrid, typeof(DiagramShowGridCommand));
			AddCommandConstructor(DiagramCommandId.SkinGallery, typeof(DiagramSkinGalleryCommand));
		}
	}
	public class DiagramCommandRepositoryCore {
		readonly DiagramControl diagram;
		readonly Dictionary<DiagramCommandId, ConstructorInfo> commands = new Dictionary<DiagramCommandId, ConstructorInfo>();
		public DiagramCommandRepositoryCore(DiagramControl diagram) {
			this.diagram = diagram;
		}
		protected void AddCommandConstructor(DiagramCommandId commandId, Type commandType) {
			ConstructorInfo ci = commandType.GetConstructor(new Type[] { Diagram.GetType() });
			if(ci == null) throw new Exception(string.Format(DiagramControlLocalizer.GetString(DiagramControlStringId.MessageAddCommandConstructorError), commandType));
			commands.Add(commandId, ci);
		}
		protected internal Command CreateCommand(DiagramCommandId id) {
			ConstructorInfo ci;
			return (commands.TryGetValue(id, out ci) && ci != null) ? ci.Invoke(new object[] { Diagram }) as Command : null;
		}
		public DiagramControl Diagram { get { return diagram; } }
	}
}
