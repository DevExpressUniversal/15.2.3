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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Diagram.Core;
using DevExpress.Internal;
using DevExpress.Utils;
namespace DevExpress.Diagram.Core {
	public abstract class DiagramCommandBase {
	}
	public class DiagramCommand<T> : DiagramCommandBase {
	}
	public class DiagramCommand : DiagramCommandBase {
	}
	public abstract class DiagramCommandsBase : ImmutableObject {
		protected class CommandHandler {
			public readonly Action<object, IDiagramControl, IMouseArgs> Action;
			public readonly Func<object, IDiagramControl, bool> CanExecute;
			public readonly DiagramCommandBase Command;
			public CommandHandler(DiagramCommandBase command, Action<object, IDiagramControl, IMouseArgs> action, Func<object, IDiagramControl, bool> canExecute) {
				this.Action = action;
				this.CanExecute = canExecute;
				this.Command = command;
			}
		}
		protected class CommandHandlersBase {
			class HandlersRegistrator : IHandlersRegistrator {
				readonly CommandHandlersBase handlers;
				public HandlersRegistrator(CommandHandlersBase handlers) {
					this.handlers = handlers;
				}
				void IHandlersRegistrator.RegisterHandlerCore(DiagramCommandBase command, Action<object, IDiagramControl, IMouseArgs> handler, Func<object, IDiagramControl, bool> canExecute) {
					var commandHandler = new CommandHandler(command, handler, (canExecute ?? ((param, x) => true)));
					if (handlers.handlers.ContainsKey(command))
						throw new InvalidOperationException();
					handlers.handlers[command] = commandHandler;
				}
			}
			readonly Dictionary<DiagramCommandBase, CommandHandler> handlers = new Dictionary<DiagramCommandBase, CommandHandler>();
			public CommandHandlersBase(Action<IHandlersRegistrator> registerHandlers) {
				registerHandlers(new HandlersRegistrator(this));
			}
			public CommandHandler GetCommandHandler(DiagramCommandBase command) {
				return handlers.GetValueOrDefault(command);
			}
			protected IEnumerable<DiagramCommandBase> DiagramCommands {
				get { return handlers.Keys; }
			}
		}
		protected sealed class CommandHandlers : CommandHandlersBase, ICommandHandlers {
			readonly IDiagramControl diagram;
			public CommandHandlers(Action<IHandlersRegistrator> registerHandlers, IDiagramControl diagram, bool updateCommandsOnRequerySuggested)
				: base(registerHandlers) {
				this.diagram = diagram;
				if (updateCommandsOnRequerySuggested)
					CommandManager.RequerySuggested += OnRequerySuggested;
			}
			void OnRequerySuggested(object sender, EventArgs e) {
				UpdateCommands();
			}
			void ICommandHandlers.Destroy() {
				diagram.Commands().handlersList.Remove(this);
				CommandManager.RequerySuggested -= OnRequerySuggested;
				UpdateCommands();
			}
			void UpdateCommands() {
				diagram.UpdateCommands(DiagramCommands);
			}
		}
		public static readonly DiagramCommand CancelCommand = new DiagramCommand();
		public static readonly DiagramCommand EmptyCommand = new DiagramCommand();
		public static readonly DiagramCommand UndoCommand = new DiagramCommand();
		public static readonly DiagramCommand RedoCommand = new DiagramCommand();
		public static readonly DiagramCommand DeleteCommand = new DiagramCommand();
		public static readonly DiagramCommand CopyCommand = new DiagramCommand();
		public static readonly DiagramCommand PasteCommand = new DiagramCommand();
		public static readonly DiagramCommand CutCommand = new DiagramCommand();
		public static readonly DiagramCommand EditCommand = new DiagramCommand();
		public static readonly DiagramCommand IncreaseFontSizeCommand = new DiagramCommand();
		public static readonly DiagramCommand DecreaseFontSizeCommand = new DiagramCommand();
		public static readonly DiagramCommand ToggleFontBoldCommand = new DiagramCommand();
		public static readonly DiagramCommand ToggleFontItalicCommand = new DiagramCommand();
		public static readonly DiagramCommand ToggleFontUnderlineCommand = new DiagramCommand();
		public static readonly DiagramCommand ToggleFontStrikethroughCommand = new DiagramCommand();
		public static readonly DiagramCommand ShowPopupMenuCommand = new DiagramCommand();
		public static readonly DiagramCommand<DiagramTool> StartDragToolCommand = new DiagramCommand<DiagramTool>();
		public static readonly DiagramCommand<DiagramTool> StartDragToolAlternateCommand = new DiagramCommand<DiagramTool>();
		public static readonly DiagramCommand<DiagramTool> UseToolCommand = new DiagramCommand<DiagramTool>();
		public static readonly DiagramCommand ZoomInCommand = new DiagramCommand();
		public static readonly DiagramCommand ZoomOutCommand = new DiagramCommand();
		public static readonly DiagramCommand<double> SetZoomCommand = new DiagramCommand<double>();
		public static readonly DiagramCommand MoveLeftCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveUpCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveRightCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveDownCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveLeftNoSnapCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveUpNoSnapCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveRightNoSnapCommand = new DiagramCommand();
		public static readonly DiagramCommand MoveDownNoSnapCommand = new DiagramCommand();
		public static readonly DiagramCommand BringToFrontCommand = new DiagramCommand();
		public static readonly DiagramCommand SendToBackCommand = new DiagramCommand();
		public static readonly DiagramCommand BringForwardCommand = new DiagramCommand();
		public static readonly DiagramCommand SendBackwardCommand = new DiagramCommand();
		public static readonly DiagramCommand SelectNextItemCommand = new DiagramCommand();
		public static readonly DiagramCommand SelectPrevItemCommand = new DiagramCommand();
		public static readonly DiagramCommand SelectAllCommand = new DiagramCommand();
		public static readonly DiagramCommand FocusNextControlCommand = new DiagramCommand();
		public static readonly DiagramCommand FocusPrevControlCommand = new DiagramCommand();
		public static readonly DiagramCommand SaveDocumentCommand = new DiagramCommand();
		public static readonly DiagramCommand SaveDocumentAsCommand = new DiagramCommand();
		public static readonly DiagramCommand LoadDocumentCommand = new DiagramCommand();
		public static readonly DiagramCommand NewDocumentCommand = new DiagramCommand();
		public static readonly DiagramCommand ApplyLastBackgroundColorCommand = new DiagramCommand();
		public static readonly DiagramCommand ApplyLastForegroundColorCommand = new DiagramCommand();
		public static readonly DiagramCommand ApplyLastStrokeColorCommand = new DiagramCommand();
		public static readonly DiagramCommand<string> TreeLayoutCommand = new DiagramCommand<string>();
		public static readonly DiagramCommand DisplayItemPropertiesCommand = new DiagramCommand();
		public static readonly DiagramCommand SetPageSizeCommand = new DiagramCommand();
		public static readonly DiagramCommand<ConnectorType> ChangeConnectorTypeCommand = new DiagramCommand<ConnectorType>();
		public static IEnumerable<DiagramCommandBase> SelectionCommands {
			get {
				yield return DeleteCommand;
				yield return CutCommand;
				yield return CopyCommand;
				yield return BringToFrontCommand;
				yield return BringForwardCommand;
				yield return SendToBackCommand;
				yield return SendBackwardCommand;
			}
		}
		public static IEnumerable<DiagramCommandBase> ZoomCommands {
			get {
				yield return ZoomInCommand;
				yield return ZoomOutCommand;
			}
		}
		public static IEnumerable<DiagramCommandBase> UndoRedoCommands {
			get {
				yield return UndoCommand;
				yield return RedoCommand;
			}
		}
		protected static readonly CommandHandlersBase DefaultHandlers = new CommandHandlersBase(registrator => {
			registrator.RegisterHandler(CancelCommand, (x, args) => {
				if (x.Controller.IsActiveInputState())
					x.Controller.EscapeState(args);
				else
					x.ClearSelection();
			}, canExecute: null, defaultStateOnly: false);
			registrator.RegisterHandler(UndoCommand, x => x.UndoManager().Undo(), x => x.UndoManager().CanUndo());
			registrator.RegisterHandler(RedoCommand, x => x.UndoManager().Redo(), x => x.UndoManager().CanRedo());
			registrator.RegisterHandler(DeleteCommand, x => x.DeleteSelectedItems(), x => x.CanDeleteSelectedItems());
			registrator.RegisterHandler(CopyCommand, x => x.CopySelectedItems(), x => x.CanCopySelectedItems());
			registrator.RegisterHandler(CutCommand, x => x.CutSelectedItems(), x => x.CanCopySelectedItems() && x.CanDeleteSelectedItems());
			registrator.RegisterHandler(PasteCommand, x => x.Paste());
			registrator.RegisterHandler(EditCommand, x => x.StartEditPrimarySelection(), x => x.CanEditPrimarySelection());
			registrator.RegisterHandler(IncreaseFontSizeCommand, x => x.IncreaseSelectionFontSize());
			registrator.RegisterHandler(DecreaseFontSizeCommand, x => x.DecreaseSelectionFontSize());
			registrator.RegisterHandler(ToggleFontBoldCommand, diagram => diagram.ToggleFontTraitsProperty(x => x.IsFontBold));
			registrator.RegisterHandler(ToggleFontItalicCommand, diagram => diagram.ToggleFontTraitsProperty(x => x.IsFontItalic));
			registrator.RegisterHandler(ToggleFontUnderlineCommand, diagram => diagram.ToggleFontTraitsProperty(x => x.IsFontUnderline));
			registrator.RegisterHandler(ToggleFontStrikethroughCommand, diagram => diagram.ToggleFontTraitsProperty(x => x.IsFontStrikethrough));
			registrator.RegisterHandler(SaveDocumentCommand, diagram => diagram.SaveDocument());
			registrator.RegisterHandler(SaveDocumentAsCommand, diagram => diagram.SaveDocument(overwrite: true));
			registrator.RegisterHandler(LoadDocumentCommand, diagram => diagram.LoadDocument());
			registrator.RegisterHandler(NewDocumentCommand, diagram => diagram.LoadDocument(createNew: true));
			registrator.RegisterHandler(ShowPopupMenuCommand, x => x.ShowPopupMenu(DiagramMenuPlacement.PrimarySelection));
			Action<DiagramCommand<DiagramTool>, MouseButton> registerStartDragCommand = (command, button) =>
				registrator.RegisterHandler(command, (tool, diag, args) => diag.Controller.UpdateInputState(state => tool.StartDrag(state, diag, args, button).AsHandled()));
			registerStartDragCommand(StartDragToolCommand, MouseButton.Left);
			registerStartDragCommand(StartDragToolAlternateCommand, MouseButton.Right);
			registrator.RegisterHandler(UseToolCommand, (tool, diag, args) => tool.DefaultAction(diag));
			registrator.RegisterHandler(ZoomInCommand, x => x.ZoomIn(), x => x.ZoomFactor < DiagramController.MaxZoomFactor);
			registrator.RegisterHandler(ZoomOutCommand, x => x.ZoomOut(), x => x.ZoomFactor > DiagramController.MinZoomFactor);
			registrator.RegisterHandler(SetZoomCommand, (zoomFactor, x, args) => x.SetZoom((double)zoomFactor), null);
			registrator.RegisterHandler(MoveLeftCommand, x => x.MoveSelection(Direction.Left));
			registrator.RegisterHandler(MoveUpCommand, x => x.MoveSelection(Direction.Up));
			registrator.RegisterHandler(MoveRightCommand, x => x.MoveSelection(Direction.Right));
			registrator.RegisterHandler(MoveDownCommand, x => x.MoveSelection(Direction.Down));
			registrator.RegisterHandler(MoveLeftNoSnapCommand, x => x.MoveSelectionNoSnap(Direction.Left));
			registrator.RegisterHandler(MoveUpNoSnapCommand, x => x.MoveSelectionNoSnap(Direction.Up));
			registrator.RegisterHandler(MoveRightNoSnapCommand, x => x.MoveSelectionNoSnap(Direction.Right));
			registrator.RegisterHandler(MoveDownNoSnapCommand, x => x.MoveSelectionNoSnap(Direction.Down));
			Func<IDiagramControl, bool> canChangeZOrder = x => x.CanChangeZOrder();
			registrator.RegisterHandler(BringToFrontCommand, x => x.BringSelectionToFront(), canChangeZOrder);
			registrator.RegisterHandler(BringForwardCommand, x => x.BringSelectionForward(), canChangeZOrder);
			registrator.RegisterHandler(SendToBackCommand, x => x.SendSelectionToBack(), canChangeZOrder);
			registrator.RegisterHandler(SendBackwardCommand, x => x.SendSelectionBackward(), canChangeZOrder);
			registrator.RegisterHandler(SelectNextItemCommand, x => x.MoveSelection(LogicalDirection.Forward));
			registrator.RegisterHandler(SelectPrevItemCommand, x => x.MoveSelection(LogicalDirection.Backward));
			registrator.RegisterHandler(SelectAllCommand, x => x.SelectAll());
			registrator.RegisterHandler(FocusNextControlCommand, x => x.MoveFocus(LogicalDirection.Forward));
			registrator.RegisterHandler(FocusPrevControlCommand, x => x.MoveFocus(LogicalDirection.Backward));
			registrator.RegisterHandler(ChangeConnectorTypeCommand, (type, x, args) => x.ChangeConnectorType(type));
			registrator.RegisterHandler(ApplyLastBackgroundColorCommand, x => x.ApplyLastBackgroundColor());
			registrator.RegisterHandler(ApplyLastForegroundColorCommand, x => x.ApplyLastForegroundColor());
			registrator.RegisterHandler(ApplyLastStrokeColorCommand, x => x.ApplyLastStrokeColor());
			registrator.RegisterHandler(TreeLayoutCommand, (s, x, args) => x.LayoutTreeDiagram(s));
			registrator.RegisterHandler(DisplayItemPropertiesCommand, x => x.DisplayItemProperties());
			registrator.RegisterHandler(SetPageSizeCommand, x => x.SetPageSize());
	});
		public static DiagramCommand GetKeyDownCommand(Key key, ModifierKeys modifers) {
			bool isControlPressed = modifers == ModifierKeys.Control;
			bool isShiftPressed = modifers == ModifierKeys.Shift;
			bool isControlShiftPressed = modifers == (ModifierKeys.Control | ModifierKeys.Shift);
			if (key == Key.Escape)
				return CancelCommand;
			if (key == Key.Delete)
				return DeleteCommand;
			if (isControlPressed) {
				if (key == Key.C)
					return CopyCommand;
				if (key == Key.V)
					return PasteCommand;
				if (key == Key.X)
					return CutCommand;
				if (key == Key.A)
					return SelectAllCommand;
			}
			if (key == Key.Z && isControlPressed)
				return UndoCommand;
			if ((key == Key.Y && isControlPressed) || (key == Key.Z && isControlShiftPressed))
				return RedoCommand;
			if (key == Key.F2)
				return EditCommand;
			if (key == Key.Left)
				return isControlPressed ? MoveLeftNoSnapCommand : MoveLeftCommand;
			if (key == Key.Up)
				return isControlPressed ? MoveUpNoSnapCommand : MoveUpCommand;
			if (key == Key.Right)
				return isControlPressed ? MoveRightNoSnapCommand : MoveRightCommand;
			if (key == Key.Down)
				return isControlPressed ? MoveDownNoSnapCommand : MoveDownCommand;
			if (key == Key.OemPeriod && isControlShiftPressed)
				return IncreaseFontSizeCommand;
			if (key == Key.OemComma && isControlShiftPressed)
				return DecreaseFontSizeCommand;
			if (key == Key.B && isControlPressed)
				return ToggleFontBoldCommand;
			if (key == Key.U && isControlPressed)
				return ToggleFontUnderlineCommand;
			if (key == Key.I && isControlPressed)
				return ToggleFontItalicCommand;
			if (key == Key.F && isControlShiftPressed)
				return BringToFrontCommand;
			if (key == Key.B && isControlShiftPressed)
				return SendToBackCommand;
			if (key == Key.Tab) {
				if (isControlPressed || isControlShiftPressed)
					return isControlShiftPressed ? FocusPrevControlCommand : FocusNextControlCommand;
				else
					return isShiftPressed ? SelectPrevItemCommand : SelectNextItemCommand;
			}
			return EmptyCommand;
		}
		public static DiagramCommand GetKeyUpCommand(Key key, ModifierKeys modifers) {
			if (key == Key.Apps)
				return ShowPopupMenuCommand;
			return EmptyCommand;
		}
		protected readonly IDiagramControl diagram;
		readonly List<CommandHandlersBase> handlersList = new List<CommandHandlersBase>();
		public DiagramCommandsBase(IDiagramControl diagram) {
			this.diagram = diagram;
			RegisterHandlers(DefaultHandlers);
		}
		public ICommandHandlers RegisterHandlers(Action<IHandlersRegistrator> registerHandlers, bool updateCommandsOnRequerySuggested = false) {
			var handlers = new CommandHandlers(registerHandlers, diagram, updateCommandsOnRequerySuggested);
			RegisterHandlers(handlers);
			return handlers;
		}
		void RegisterHandlers(CommandHandlersBase handlers) {
			handlersList.Add(handlers);
		}
		public virtual void Execute(DiagramCommandBase command, IMouseArgs args = null, object parameter = null) {
			GetCommandHandler(command).Do(x => x.Action(parameter, diagram, args ?? diagram.Controller.CreatePlatformMouseArgs()));
		}
		public bool CanExecute(DiagramCommandBase command, object parameter) {
			var handler = GetCommandHandler(command);
			if (handler == null)
				return false;
			return handler.CanExecute(parameter, diagram);
		}
		CommandHandler GetCommandHandler(DiagramCommandBase command) {
			return handlersList.Reverse<CommandHandlersBase>().Select(x => x.GetCommandHandler(command)).FirstOrDefault(x => x != null);
		}
#if DEBUGTEST
		public IMouseArgs ForcedMouseArgsForTests { get; set; }
#endif
		protected IMouseArgs Args {
			get {
				IMouseArgs args = null;
#if DEBUGTEST
				args = ForcedMouseArgsForTests;
#endif
				return args;
			}
		}
		public abstract void UpdateCommands(IEnumerable<DiagramCommandBase> commands);
	}
	public interface IHandlersRegistrator {
		void RegisterHandlerCore(DiagramCommandBase command, Action<object, IDiagramControl, IMouseArgs> handler, Func<object, IDiagramControl, bool> canExecute);
	}
	public static class HandlersRegistratorExtensions {
		public static void RegisterHandler(this IHandlersRegistrator registrator, DiagramCommandBase command, Action<object, IDiagramControl, IMouseArgs> handler, Func<object, IDiagramControl, bool> canExecute = null, bool defaultStateOnly = true) {
			Action<object, IDiagramControl, IMouseArgs> actualHandler = (param, x, args) => {
				if (!defaultStateOnly || !x.Controller.IsActiveInputState())
					handler(param, x, args);
			};
			registrator.RegisterHandlerCore(command, actualHandler, canExecute);
		}
		public static void RegisterHandler(this IHandlersRegistrator registrator, DiagramCommand command, Action<IDiagramControl> handler, Func<IDiagramControl, bool> canExecute = null, bool defaultStateOnly = true) {
			registrator.RegisterHandler(command, (x, args) => handler(x), canExecute, defaultStateOnly);
		}
		public static void RegisterHandler(this IHandlersRegistrator registrator, DiagramCommand command, Action<IDiagramControl, IMouseArgs> handler, Func<IDiagramControl, bool> canExecute = null, bool defaultStateOnly = true) {
			Func<object, IDiagramControl, bool> actualCanExecute = (param, x) => canExecute != null ? canExecute(x) : true;
			registrator.RegisterHandler(command, (param, x, arg) => handler(x, arg), actualCanExecute, defaultStateOnly);
		}
		public static void RegisterHandler<T>(this IHandlersRegistrator registrator, DiagramCommand<T> command, Action<T, IDiagramControl, IMouseArgs> handler) {
			registrator.RegisterHandler(command, (param, x, arg) => handler((T)param, x, arg), null, defaultStateOnly: true);
		}
		public static void RegisterHandler(this IHandlersRegistrator registrator, DiagramCommand command, Action handler, Func<bool> canExecute = null) {
			Func<object, IDiagramControl, bool> actualCanExecute = (param, x) => canExecute != null ? canExecute() : true;
			registrator.RegisterHandler(command, (param, x, arg) => handler(), actualCanExecute, defaultStateOnly: true);
		}
	}
	public interface ICommandHandlers {
		void Destroy();
	}
}
