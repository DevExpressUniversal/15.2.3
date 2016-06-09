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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Diagram.Core.Native;
using DevExpress.Utils;
using WinCursor = System.Windows.Forms.Cursor;
using WinCursors = System.Windows.Forms.Cursors;
namespace DevExpress.Diagram.Core {
	public interface IMouseArgs {
		Point Position { get; }
		ModifierKeys Modifiers { get; }
		void Capture();
		void Release();
	}
	public interface IMouseButtonArgs : IMouseArgs {
		int ClickCount { get; }
		MouseButton ChangedButton { get; }
	}
	public class InputResult {
		public readonly InputState State;
		public readonly bool Handled;
		public InputResult(InputState state, bool handled) {
			this.State = state;
			this.Handled = handled;
		}
	}
	public static class InputStateExtensions {
		public static InputResult AsHandled(this InputState state) {
			return new InputResult(state, true);
		}
		public static InputResult AsUnhandled(this InputState state) {
			return new InputResult(state, false);
		}
	}
	public interface IInputElement {
		InputState CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs);
		InputState CreateItemToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs, ItemTool tool);
		InputState CreateConnectorToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs);
		DiagramCursor GetPointerToolCursor(IMouseArgs mouse);
		DiagramCursor GetItemToolCursor(IDiagramControl diagram, IMouseArgs mouse);
		DiagramCursor GetConnectorToolCursor(IDiagramControl diagram, IMouseArgs mouse);
	}
	public sealed class NullInputElement : IInputElement {
		public static readonly IInputElement Instance = new NullInputElement();
		NullInputElement() { }
		InputState IInputElement.CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			throw new NotSupportedException();
		}
		InputState IInputElement.CreateItemToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs, ItemTool tool) {
			throw new NotSupportedException();
		}
		InputState IInputElement.CreateConnectorToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			throw new NotSupportedException();
		}
		DiagramCursor IInputElement.GetPointerToolCursor(IMouseArgs mouse) {
			return DiagramCursor.Default;
		}
		DiagramCursor IInputElement.GetItemToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return DiagramCursor.Default;
		}
		DiagramCursor IInputElement.GetConnectorToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return DiagramCursor.Default;
		}
	}
	public abstract class InputState {
		readonly DiagramCursor cursor;
		public InputState(DiagramCursor cursor) {
			this.cursor = cursor;
		}
		public DiagramCursor Cursor { get { return cursor; } }
		public abstract InputResult HandleMouseUp(IInputElement item, IMouseButtonArgs mouseArgs);
		public abstract InputResult HandleMouseDown(IInputElement item, IMouseButtonArgs mouseArgs);
		public abstract InputResult HandleMouseMove(IInputElement item, IMouseArgs mouseArgs);
		public abstract InputResult HandleEscape(IMouseArgs mouse);
		public abstract InputResult HandleMouseLeave(IMouseArgs mouse);
		public abstract InputResult HandleModifiersChanged(IMouseArgs mouseArgs);
	}
	public static class IMouseArgsExtensions {
		public static bool IsSnappingEnabled(this IMouseArgs mouse) {
			return (mouse.Modifiers & ModifierKeys.Alt) == 0;
		}
		public static bool IsCloneSelectionModifierPressed(this IMouseArgs mouse) {
			return (mouse.Modifiers & ModifierKeys.Control) > 0;
		}
		public static bool IsSelectionModifierPressed(this IMouseArgs mouse) {
			return ((mouse.Modifiers & ModifierKeys.Control) > 0) ||
				((mouse.Modifiers & ModifierKeys.Shift) > 0);
		}
		public static void SelectItemWithModifier(this IDiagramControl diagram, IDiagramItem item, IMouseArgs args) {
			diagram.SelectItem(item, addToSelection: args.IsSelectionModifierPressed());
		}
		public static void UnselectItemWithModifier(this IDiagramControl diagram, IMouseArgs args, IDiagramItem item) {
			if(!args.IsSelectionModifierPressed()) {
				diagram.Selection().ClearSelectionOrMakePrimarySelection(item);
			}
			else if(item != null) {
				diagram.UnselectItem(item);
			}
		}
		public static bool IsPrimaryButton(this IMouseButtonArgs args) {
			return args.ChangedButton == MouseButton.Left || args.ChangedButton == MouseButton.Right;
		}
		public static bool IsDoubleClick(this IMouseButtonArgs args) {
			return args.ClickCount == 2;
		}
		public static bool IsRightButton(this IMouseButtonArgs args) {
			return args.ChangedButton == MouseButton.Right;
		}
		public static void ShowPopupMenuIfRightButton(this IDiagramControl diagram, IMouseButtonArgs args) {
			if(args.IsRightButton())
				diagram.ShowPopupMenu();
		}
	}
	public static class DefaultImages {
		public static System.Drawing.Image RotateIcon = ResourceImageHelper.CreateImageFromResources("Images.RotateIcon.png", typeof(DefaultImages));
	}
	public enum DiagramCursor {
		Default,
		No,
		Add,
		Copy,
		Move,
		DrawSelection,
		DrawItem,
		SizeWE,
		SizeNS,
		SizeNESW,
		SizeNWSE,
		HoverParameter,
		ChangeParameter,
		HoverRotation,
		Rotation,
		HoverConnectorPoint,
		MoveConnectorPoint,
		HoverFreeConnectorSource,
		HoverConnectedConnectorSource
	}
	public static partial class DiagramCursorExtensions {
		static Cursor GetCursor(string name) {
			return new Cursor(GetCursorStream(name));
		}
		static WinCursor GetWinCursor(string name) {
			IntPtr handle = Win32.CreateCursor(GetCursorStream(name));
			if(handle == IntPtr.Zero) {
				throw new ArgumentException("name");
			}
			return new WinCursor(handle);
		}
		const string cursorsPath = "DevExpress.Diagram.Core.Images.Cursors.";
		static Stream GetCursorStream(string name) {
			return typeof(IDiagramControl).Assembly.GetManifestResourceStream(cursorsPath + name);
		}
		public static Cursor ToCursor(this DiagramCursor cursor) {
			switch(cursor) {
			case DiagramCursor.Default:
				return ArrowCursor;
			case DiagramCursor.No:
				return NoCursor;
			case DiagramCursor.Add:
				return CopyCursor;
			case DiagramCursor.Copy:
				return CopyCursor;
			case DiagramCursor.Move:
				return MoveCursor;
			case DiagramCursor.DrawSelection:
				return CrossCursor;
			case DiagramCursor.DrawItem:
				return CrossCursor;
			case DiagramCursor.SizeWE:
				return SizeWECursor;
			case DiagramCursor.SizeNS:
				return SizeNSCursor;
			case DiagramCursor.SizeNESW:
				return SizeNESWCursor;
			case DiagramCursor.SizeNWSE:
				return SizeNWSECursor;
			case DiagramCursor.HoverParameter:
				return SizeAllCursor;
			case DiagramCursor.ChangeParameter:
				return CrossCursor;
			case DiagramCursor.HoverRotation:
				return HoverRotationCursor;
			case DiagramCursor.Rotation:
				return RotationCursor;
			case DiagramCursor.HoverConnectorPoint:
				return SizeAllCursor;
			case DiagramCursor.MoveConnectorPoint:
				return CrossCursor;
			case DiagramCursor.HoverFreeConnectorSource:
				return DrawFreeConnectorCursor;
			case DiagramCursor.HoverConnectedConnectorSource:
				return DrawConnectedConnectorCursor;
			default:
				throw new InvalidOperationException();
			}
		}
		public static WinCursor ToWinCursor(this DiagramCursor cursor) {
			switch(cursor) {
				case DiagramCursor.Default:
					return WinCursors.Default;
				case DiagramCursor.No:
					return NoWinCursor;
				case DiagramCursor.Add:
				case DiagramCursor.Copy:
					return CopyWinCursor;
				case DiagramCursor.Move:
					return MoveWinCursor;
				case DiagramCursor.DrawSelection:
				case DiagramCursor.DrawItem:
					return CrossWinCursor;
				case DiagramCursor.SizeWE:
					return SizeWEWinCursor;
				case DiagramCursor.SizeNS:
					return SizeNSWinCursor;
				case DiagramCursor.SizeNESW:
					return SizeNESWWinCursor;
				case DiagramCursor.SizeNWSE:
					return SizeNWSEWinCursor;
				case DiagramCursor.HoverParameter:
					return SizeAllWinCursor;
				case DiagramCursor.ChangeParameter:
					return CrossWinCursor;
				case DiagramCursor.HoverRotation:
					return HoverRotationWinCursor;
				case DiagramCursor.Rotation:
					return RotationWinCursor;
				case DiagramCursor.HoverConnectorPoint:
					return SizeAllWinCursor;
				case DiagramCursor.MoveConnectorPoint:
					return CrossWinCursor;
				case DiagramCursor.HoverFreeConnectorSource:
					return DrawFreeConnectorWinCursor;
				case DiagramCursor.HoverConnectedConnectorSource:
					return DrawConnectedConnectorWinCursor;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
