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

namespace DevExpress.Xpf.Diagram {
using DevExpress.Mvvm;
using DevExpress.Diagram.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Text;
	partial class DiagramCommands {
		public ICommand Cancel { get; private set; }
		public ICommand Empty { get; private set; }
		public ICommand Undo { get; private set; }
		public ICommand Redo { get; private set; }
		public ICommand Delete { get; private set; }
		public ICommand Copy { get; private set; }
		public ICommand Paste { get; private set; }
		public ICommand Cut { get; private set; }
		public ICommand Edit { get; private set; }
		public ICommand IncreaseFontSize { get; private set; }
		public ICommand DecreaseFontSize { get; private set; }
		public ICommand ToggleFontBold { get; private set; }
		public ICommand ToggleFontItalic { get; private set; }
		public ICommand ToggleFontUnderline { get; private set; }
		public ICommand ToggleFontStrikethrough { get; private set; }
		public ICommand ShowPopupMenu { get; private set; }
		public ICommand<DiagramTool> StartDragTool { get; private set; }
		public ICommand<DiagramTool> StartDragToolAlternate { get; private set; }
		public ICommand<DiagramTool> UseTool { get; private set; }
		public ICommand ZoomIn { get; private set; }
		public ICommand ZoomOut { get; private set; }
		public ICommand<Double> SetZoom { get; private set; }
		public ICommand MoveLeft { get; private set; }
		public ICommand MoveUp { get; private set; }
		public ICommand MoveRight { get; private set; }
		public ICommand MoveDown { get; private set; }
		public ICommand MoveLeftNoSnap { get; private set; }
		public ICommand MoveUpNoSnap { get; private set; }
		public ICommand MoveRightNoSnap { get; private set; }
		public ICommand MoveDownNoSnap { get; private set; }
		public ICommand BringToFront { get; private set; }
		public ICommand SendToBack { get; private set; }
		public ICommand BringForward { get; private set; }
		public ICommand SendBackward { get; private set; }
		public ICommand SelectNextItem { get; private set; }
		public ICommand SelectPrevItem { get; private set; }
		public ICommand SelectAll { get; private set; }
		public ICommand FocusNextControl { get; private set; }
		public ICommand FocusPrevControl { get; private set; }
		public ICommand SaveDocument { get; private set; }
		public ICommand SaveDocumentAs { get; private set; }
		public ICommand LoadDocument { get; private set; }
		public ICommand NewDocument { get; private set; }
		public ICommand ApplyLastBackgroundColor { get; private set; }
		public ICommand ApplyLastForegroundColor { get; private set; }
		public ICommand ApplyLastStrokeColor { get; private set; }
		public ICommand<String> TreeLayout { get; private set; }
		public ICommand DisplayItemProperties { get; private set; }
		public ICommand SetPageSize { get; private set; }
		public ICommand<ConnectorType> ChangeConnectorType { get; private set; }
		public DiagramCommands(DiagramControl diagram) 
			: base(diagram) {
			Cancel = CreateCommand(CancelCommand);
			Empty = CreateCommand(EmptyCommand);
			Undo = CreateCommand(UndoCommand);
			Redo = CreateCommand(RedoCommand);
			Delete = CreateCommand(DeleteCommand);
			Copy = CreateCommand(CopyCommand);
			Paste = CreateCommand(PasteCommand);
			Cut = CreateCommand(CutCommand);
			Edit = CreateCommand(EditCommand);
			IncreaseFontSize = CreateCommand(IncreaseFontSizeCommand);
			DecreaseFontSize = CreateCommand(DecreaseFontSizeCommand);
			ToggleFontBold = CreateCommand(ToggleFontBoldCommand);
			ToggleFontItalic = CreateCommand(ToggleFontItalicCommand);
			ToggleFontUnderline = CreateCommand(ToggleFontUnderlineCommand);
			ToggleFontStrikethrough = CreateCommand(ToggleFontStrikethroughCommand);
			ShowPopupMenu = CreateCommand(ShowPopupMenuCommand);
			StartDragTool = CreateCommand(StartDragToolCommand);
			StartDragToolAlternate = CreateCommand(StartDragToolAlternateCommand);
			UseTool = CreateCommand(UseToolCommand);
			ZoomIn = CreateCommand(ZoomInCommand);
			ZoomOut = CreateCommand(ZoomOutCommand);
			SetZoom = CreateCommand(SetZoomCommand);
			MoveLeft = CreateCommand(MoveLeftCommand);
			MoveUp = CreateCommand(MoveUpCommand);
			MoveRight = CreateCommand(MoveRightCommand);
			MoveDown = CreateCommand(MoveDownCommand);
			MoveLeftNoSnap = CreateCommand(MoveLeftNoSnapCommand);
			MoveUpNoSnap = CreateCommand(MoveUpNoSnapCommand);
			MoveRightNoSnap = CreateCommand(MoveRightNoSnapCommand);
			MoveDownNoSnap = CreateCommand(MoveDownNoSnapCommand);
			BringToFront = CreateCommand(BringToFrontCommand);
			SendToBack = CreateCommand(SendToBackCommand);
			BringForward = CreateCommand(BringForwardCommand);
			SendBackward = CreateCommand(SendBackwardCommand);
			SelectNextItem = CreateCommand(SelectNextItemCommand);
			SelectPrevItem = CreateCommand(SelectPrevItemCommand);
			SelectAll = CreateCommand(SelectAllCommand);
			FocusNextControl = CreateCommand(FocusNextControlCommand);
			FocusPrevControl = CreateCommand(FocusPrevControlCommand);
			SaveDocument = CreateCommand(SaveDocumentCommand);
			SaveDocumentAs = CreateCommand(SaveDocumentAsCommand);
			LoadDocument = CreateCommand(LoadDocumentCommand);
			NewDocument = CreateCommand(NewDocumentCommand);
			ApplyLastBackgroundColor = CreateCommand(ApplyLastBackgroundColorCommand);
			ApplyLastForegroundColor = CreateCommand(ApplyLastForegroundColorCommand);
			ApplyLastStrokeColor = CreateCommand(ApplyLastStrokeColorCommand);
			TreeLayout = CreateCommand(TreeLayoutCommand);
			DisplayItemProperties = CreateCommand(DisplayItemPropertiesCommand);
			SetPageSize = CreateCommand(SetPageSizeCommand);
			ChangeConnectorType = CreateCommand(ChangeConnectorTypeCommand);
		}
	}
}
