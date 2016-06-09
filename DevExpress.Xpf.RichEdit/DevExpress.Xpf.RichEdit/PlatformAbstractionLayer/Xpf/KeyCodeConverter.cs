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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.Services.Implementation;
using DevExpress.XtraRichEdit.Keyboard;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
#if SL
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = DevExpress.Data.KeyPressEventArgs;
using PlatformIndependentKeys = DevExpress.Data.Keys;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using DevExpress.Xpf.Core.Native;
#else
using PlatformIndependentKeys = System.Windows.Forms.Keys;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentKeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public delegate ImeController ObtainImeControllerHandler();
	public class KeyCodeConverter : TextBox {
		bool innerIsFocused;
		Canvas canvas;
		protected internal bool InnerIsFocused { get { return innerIsFocused; } }
		public Control Owner { get; set; }
		internal ImeController ImeController { get; set; }
		protected RichEditControl RichEditControl { get { return ImeController != null ? ImeController.Control : null; } }
		public KeyCodeConverter() {
			TextChanged += OnKeyCodeConverterTextChanged;
#if !SL
			TextCompositionManager.AddTextInputStartHandler(this, OnTextInputStart);
			TextCompositionManager.AddTextInputUpdateHandler(this, OnTextInputUpdate);
#endif
#if SL
			TabNavigation = KeyboardNavigationMode.Cycle;
			AcceptsReturn = true;
#else
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
#endif
		}
		public event EventHandler<PlatformIndependentKeyPressEventArgs> KeyPress;
		public new event KeyEventHandler KeyDown;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.canvas = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<Canvas>(this);
		}
		protected void RaiseKeyPress(char keyChar) {
			if (KeyPress == null)
				return;
			PlatformIndependentKeyPressEventArgs args = new PlatformIndependentKeyPressEventArgs(keyChar);
			KeyPress(this, args);
		}
		void SetTextSilent(string newText) {
			TextChanged -= OnKeyCodeConverterTextChanged;
			Text = newText;
			TextChanged += OnKeyCodeConverterTextChanged;
		}
		internal void ForceTextChanged() {
			OnKeyCodeConverterTextChanged(null, null);
			FlushPendingTextInput();
		}
#if SL
		protected override void OnKeyDown(KeyEventArgs e) {
			if (ImeController == null)
				return;
			ModifierKeys modifiers = System.Windows.Input.Keyboard.Modifiers;
			if(ImeController.IsActive && e.Key == Key.Z && modifiers == ModifierKeys.Control) {
				e.Handled = true;
				return;
			}
			HandleKeyDown(e);
			base.OnKeyDown(e);
		}
#else
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			HandleKeyDown(e);
			base.OnPreviewKeyDown(e);
		}
#endif
		protected internal virtual void HandleKeyDown(KeyEventArgs e) {
			if (ImeController == null)
				return;
			ImeController.UpdateCaretPosition(SelectionStart, SelectionLength);
#if SL
			OfficeClipboard.LastEventArgs = e;
#endif
			if (KeyDown != null && !ImeController.IsActive)
				KeyDown(this, e);
			ModifierKeys modifiers = System.Windows.Input.Keyboard.Modifiers;
			if (((modifiers == ModifierKeys.Control
#if SL
 || modifiers == ModifierKeys.Apple
#endif
) &&
				(e.Key == Key.V || e.Key == Key.C || e.Key == Key.Z || e.Key == Key.X || e.Key == Key.Insert)) ||
			   (modifiers == ModifierKeys.Shift && (e.Key == Key.Insert || e.Key == Key.Delete)))
				e.Handled = true;
			if (e.Key == Key.Tab || e.Key == Key.Insert
#if !SL
 || e.ImeProcessedKey == Key.Insert
#endif
)
				e.Handled = true;
			ImeController.OnKeyDown(e, modifiers);
		}
#if SL
		protected override void OnTextInputStart(TextCompositionEventArgs e) {
			base.OnTextInputStart(e);
#else
		protected void OnTextInputStart(object sender, TextCompositionEventArgs e) {
#endif
			if (ImeController == null)
				return;
			ImeController.OnTextInputStart(e.TextComposition.CompositionText);
		}
#if SL
		protected override void OnTextInputUpdate(TextCompositionEventArgs e) {
			base.OnTextInputUpdate(e);
#else
		protected void OnTextInputUpdate(object sender, TextCompositionEventArgs e) {
#endif
			if (ImeController == null)
				return;
			ImeController.OnTextInputUpdate(e.TextComposition.CompositionText);
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			if (ImeController == null)
				return;
			ImeController.OnTextInput(e.TextComposition.CompositionText);
			if (ImeController.IsActive)
				ImeController.Close();
			else
				FlushPendingTextInput();
		}
		void OnKeyCodeConverterTextChanged(object sender, TextChangedEventArgs e) {
			if (ImeController == null)
				return;
			ImeController.OnTextChanged(Text);
			ImeController.UpdateCaretPosition(SelectionStart, SelectionLength);
			SetPosition();
		}
		void SetPosition() {
			if (this.canvas == null || ImeController == null || !ImeController.IsActive)
				return;
			RichEditView view = ImeController.Control.ActiveView;
			PieceTable pieceTable = view.DocumentModel.ActivePieceTable;
			DevExpress.XtraRichEdit.Layout.DocumentLayoutPosition caretPosition = view.CaretPosition.LayoutPosition;
			DocumentLogPosition rowStartPos = caretPosition.IsValid(XtraRichEdit.Layout.DocumentLayoutDetailsLevel.Row) ? caretPosition.Row.GetFirstPosition(pieceTable).LogPosition : DocumentLogPosition.Zero;
			DocumentLogPosition logPosition = Algorithms.Max(ImeController.CompositionInfo.StartPos, rowStartPos);
			DevExpress.XtraRichEdit.Layout.DocumentLayoutPosition pos = view.DocumentLayout.CreateLayoutPosition(pieceTable, logPosition, caretPosition.Page.PageIndex);
			if (!pos.Update(view.DocumentLayout.Pages, DevExpress.XtraRichEdit.Layout.DocumentLayoutDetailsLevel.Character))
				return;
			PageViewInfo pageViewInfo = view.LookupPageViewInfoByPage(pos.Page);
			if (pageViewInfo == null)
				return;
			System.Drawing.Rectangle bounds = pos.Character.Bounds;
			System.Drawing.Rectangle physicalRectangle = view.CreatePhysicalRectangle(pageViewInfo, bounds);
			System.Drawing.Rectangle controlRectangle = view.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(physicalRectangle, ImeController.Control.DpiX, ImeController.Control.DpiY);
			GeneralTransform transform = ImeController.Control.TransformToVisual(this.canvas);
			System.Windows.Point location = transform.Transform(new Point(controlRectangle.X, controlRectangle.Y + controlRectangle.Height + 10));
			Canvas.SetLeft(this, location.X);
			Canvas.SetTop(this, location.Y);
		}
		protected internal virtual void FlushPendingTextInput() {
			if (ImeController == null)
				return;
			if (ImeController.IsActive)
				return;
			string text = Text;
			if (String.IsNullOrEmpty(text))
				return;
			System.Diagnostics.Debug.Assert(text.Length >= 1);
			if (!ImeController.IsCanceled)
				ProcessMultipleChars(text);
			SetTextSilent("");
			ImeController.Reset();
		}
		bool ShouldAcceptCharacter(char ch) {
			return ch != '\r' && ch != '\n';
		}
		void ProcessSingleChar(char ch) {
			if (ShouldAcceptCharacter(ch))
				RaiseKeyPress(ch);
		}
		void ProcessMultipleChars(string text) {
			if (RichEditControl != null)
				RichEditControl.BeginProcessMultipleKeyPress();
			try {
				foreach (char c in text)
					ProcessSingleChar(c);
			}
			finally {
				if (RichEditControl != null)
					RichEditControl.EndProcessMultipleKeyPress();
			}
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			innerIsFocused = true;
			base.OnGotKeyboardFocus(e);
		}
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			innerIsFocused = false;
			base.OnLostKeyboardFocus(e);
		}
	}
}
