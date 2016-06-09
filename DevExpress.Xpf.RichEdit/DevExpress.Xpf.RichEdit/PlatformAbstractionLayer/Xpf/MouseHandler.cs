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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
#if SL
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIndependentDataObject = DevExpress.Utils.DataObject;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformImage = System.Windows.Controls.Image;
#else
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformImage = System.Windows.Controls.Image;
#endif
namespace DevExpress.XtraRichEdit.Mouse {
	public class XpfDragCaretVisualizer : DragCaretVisualizer {
		readonly RichEditControl control;
		XpfRichEditCaret caret;
		bool caretVisible;
		public XpfDragCaretVisualizer(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		public override void ShowCaret(DocumentLogPosition caretLogPosition) {
			Control.HoverMenuCalculator.CloseCurrentHover();
			if (!caretVisible)
				CreateCaret(caretLogPosition);
			else
				UpdateCaretPosition(caretLogPosition);			
		}
		public override void HideCaret(DocumentLogPosition caretLogPosition) {
			if (!caretVisible)
				return;
			Control.Surface.Children.Remove(caret);
			caretVisible = false;
		}
		protected void CreateCaret(DocumentLogPosition caretLogPosition) {
			caret = new XpfRichEditDragCaret();
			if (!UpdateCaretPosition(caretLogPosition))
				return;
			caretVisible = true;
			Control.Surface.Children.Add(caret);
		}
		bool UpdateCaretPosition(DocumentLogPosition caretLogPosition) {
			DragCaretPosition caretPosition = Control.ActiveView.CaretPosition.CreateDragCaretPosition(); 
			caretPosition.SetLogPosition(caretLogPosition);
			if (!caretPosition.Update(DocumentLayoutDetailsLevel.Character)) return false;
			if (caretPosition.PageViewInfo == null) return false;
			if (!Control.IsCaretVisible ) return false;
			if (Control.ReadOnly && !Control.ShowCaretInReadOnly) return false;
			Rectangle pageViewInfoCaretBounds = caretPosition.PageViewInfo.ClientBounds;
			var pos = caretPosition.CalculateCaretBounds();
			pos.Y = (int)(pos.Y * Control.ActiveView.ZoomFactor);
			pos.X = (int)(pos.X * Control.ActiveView.ZoomFactor);
			pos.X = (int)(pos.X - Control.ActiveView.HorizontalScrollController.GetPhysicalLeftInvisibleWidth());
			pos.Y = (int)(pos.Y + pageViewInfoCaretBounds.Y);
			pos.X = (int)(pos.X + pageViewInfoCaretBounds.X);
			DocumentLayoutUnitConverter unitConverter = Control.DocumentModel.LayoutUnitConverter;
			caret.SetValue(System.Windows.Controls.Canvas.TopProperty, (double)unitConverter.LayoutUnitsToPixelsF(pos.Y));
			caret.SetValue(System.Windows.Controls.Canvas.LeftProperty, (double)unitConverter.LayoutUnitsToPixelsF(pos.X));
			caret.Width = unitConverter.LayoutUnitsToPixelsF(pos.Width * Control.ActiveView.ZoomFactor);
			caret.Height = unitConverter.LayoutUnitsToPixelsF(pos.Height * Control.ActiveView.ZoomFactor);
			return true;
		}
		public override void Finish() {
			base.Finish();
			HideCaret(DocumentLogPosition.Zero);
		}
	}
	public delegate void MouseWheelEventHandlerEx(MouseWheelEventArgsEx e);
	public class MouseWheelEventArgsEx : EventArgs {
		double delta;
		public MouseWheelEventArgsEx() : this(0) { }
		public MouseWheelEventArgsEx(double delta) {
			this.delta = delta;
		}
		public double Delta { get { return delta; } }
	}
}
