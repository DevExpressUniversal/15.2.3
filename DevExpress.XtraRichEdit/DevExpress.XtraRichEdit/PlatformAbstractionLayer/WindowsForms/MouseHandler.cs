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
using System.Drawing;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Windows.Forms;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentGiveFeedbackEventArgs = System.Windows.Forms.GiveFeedbackEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using System.Drawing.Imaging;
#else
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentGiveFeedbackEventArgs = DevExpress.Utils.GiveFeedbackEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
#if SL4
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIndependentDataObject = System.Windows.DataObject;
#else
using PlatformIndependentIDataObject = DevExpress.Utils.IDataObject;
using PlatformIndependentDataObject = DevExpress.Utils.DataObject;
#endif
using DevExpress.XtraRichEdit.Drawing;
using System.Windows.Threading;
#endif
namespace DevExpress.XtraRichEdit.Mouse {
	#region WinFormsDragCaretVisualizer
	public class WinFormsDragCaretVisualizer : DragCaretVisualizer {
		readonly RichEditControl control;
		public WinFormsDragCaretVisualizer(RichEditControl control) {
			this.control = control;
		}
		protected RichEditControl WinControl { get { return control; } }
		protected DragCaret Caret { get { return WinControl.DragCaret; } }
		RichEditControlPainter Painter { get { return control.Painter; } }
		public override void Start() {
			WinControl.CreateDragCaret();
			base.Start();
		}
		public override void Finish() {
			WinControl.DestroyDragCaret();
			base.Finish();
		}
		public override void ShowCaret(DocumentLogPosition caretLogPosition) {
			if (Caret.IsHidden) {
				DrawCaret(caretLogPosition);
				Caret.IsHidden = false;
			}
		}
		public override void HideCaret(DocumentLogPosition caretLogPosition) {
			if (!Caret.IsHidden) {
				DrawCaret(caretLogPosition);
				Caret.IsHidden = true;
			}
		}
		void DrawCaret(DocumentLogPosition caretLogPosition) {
			Caret.SetLogPosition(caretLogPosition);
			Painter.DrawDragCaret();
		}
	}
	#endregion
	#region RichEditOfficeScroller
	public class RichEditOfficeScroller : OfficeScroller {
		readonly IRichEditControl control;
		public RichEditOfficeScroller(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public IRichEditControl Control { get { return control; } }
		protected override bool AllowHScroll { get { return false; } }
		protected override void OnVScroll(int delta) {
			base.OnVScroll(delta);
			ScrollVerticallyByPixelOffsetCommand command = new ScrollVerticallyByPixelOffsetCommand(Control);
			command.PixelOffset = 5 * delta;
			command.Execute();
		}
	}
	#endregion
}
