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

using DevExpress.XtraRichEdit.Mouse;
using DevExpress.Snap.Core.Native.MouseHandler;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office.PInvoke;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using DevExpress.Data.Utils;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Native {
	public class WinFormsSnapMouseHandlerStrategyFactory : WinFormsRichEditMouseHandlerStrategyFactory {
		public override RichEditMouseHandlerStrategy CreateMouseHandlerStrategy(RichEditMouseHandler mouseHandler) {
			SnapMouseHandler snapMouseHandler = mouseHandler as SnapMouseHandler;
			if (snapMouseHandler != null)
				return new WinFormsSnapMouseHandlerStrategy(snapMouseHandler);
			else
				return base.CreateMouseHandlerStrategy(mouseHandler);
		}
		public override DragContentMouseHandlerStateBaseStrategy CreateDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state) {
			return new WinFormsSnapDragContentMouseHandlerStateBaseStrategy(state);
		}
	}
	public class WinFormsSnapDragContentMouseHandlerStateBaseStrategy : WinFormsDragContentMouseHandlerStateBaseStrategy {
		public WinFormsSnapDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state)
			: base(state) {
		}
		protected override DragCaretVisualizer CreateCaretVisualizer() {
			return new WinFormsSnapDragCaretVisualizer((SnapControl)Control);
		}
	}
	public class WinFormsSnapMouseHandlerStrategy : WinFormsRichEditMouseHandlerStrategy {
		public WinFormsSnapMouseHandlerStrategy(SnapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new SnapControl Control { get { return (SnapControl)base.Control; } }
	}
	#region WinFormsSnapDragCaretVisualizer
	public class WinFormsSnapDragCaretVisualizer : WinFormsDragCaretVisualizer, IDragCaretGiveFeedbackSupport {
		#region Inner classes
		abstract class DragCaretControl : Control {
			readonly static string ImageNamePrefix = "DevExpress.Snap.Images";
			readonly Image image;
			protected DragCaretControl() {
				this.image = CreateBitmapFromResources(typeof(InnerSnapControl).Assembly);
				Size = Image.Size;
				SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
				BackColor = Color.Transparent;
			}
			protected abstract string ImageName { get; }
			public Image Image { get { return image; } }
			Bitmap CreateBitmapFromResources(Assembly asm) {
				Stream stream = asm.GetManifestResourceStream(String.Format("{0}.{1}.png", ImageNamePrefix, ImageName));
				return (Bitmap)ImageTool.ImageFromStream(stream);
			}
			protected override void OnPaint(PaintEventArgs e) {
				e.Graphics.DrawImage(Image, new Point(0, 0));
				base.OnPaint(e);
			}
			protected override void OnVisibleChanged(EventArgs e) {
				if (!Visible && Parent != null) {
					Parent.Invalidate(Bounds);
					Parent.Update();
				}
				base.OnVisibleChanged(e);
			}
		}
		class InsertSNTextCaretControl : DragCaretControl {
			protected override string ImageName { get { return "InsertSNTextDragCaretHint"; } }
		}
		class InsertSNListCaretControl : DragCaretControl {
			protected override string ImageName { get { return "InsertSNListDragCaretHint"; } }
		}
		class InsertMasterDetailCaretControl : DragCaretControl {
			protected override string ImageName { get { return "InsertMasterDetailDragCaretHint"; } }
		}
		#endregion
		DragCaretType caretType;
		DragCaretControl caretControl;
		public WinFormsSnapDragCaretVisualizer(SnapControl control)
			: base(control) {
		}
		DragCaretControl CaretControl { get { return caretControl; } }
		DragCaretType CaretType { get { return caretType; } }
		#region GiveFeedback event
		EventHandler<DragCaretGiveFeedbackEventArgs> onGiveFeedback;
		public event EventHandler<DragCaretGiveFeedbackEventArgs> GiveFeedback { add { onGiveFeedback += value; } remove { onGiveFeedback -= value; } }
		protected void RaiseGiveFeedback(DragCaretGiveFeedbackEventArgs args) {
			EventHandler<DragCaretGiveFeedbackEventArgs> handler = onGiveFeedback;
			if (handler != null)
				handler(this, args);
		}
		#endregion
		public override void ShowCaret(DocumentLogPosition caretLogPosition) {
			base.ShowCaret(caretLogPosition);
			DragCaretType newCaretType = GetDragCaretType(caretLogPosition);
			if (CaretType != newCaretType) {
				this.caretType = newCaretType;
				ChangeDragCaretControl();
			}
			if (CaretControl != null) {
				CaretControl.Location = CalculateCaretControlLocation();
				CaretControl.Show();
			}
		}
		DragCaretType GetDragCaretType(DocumentLogPosition caretLogPosition) {
			DragCaretGiveFeedbackEventArgs args = new DragCaretGiveFeedbackEventArgs(caretLogPosition);
			RaiseGiveFeedback(args);
			return args.CaretType;
		}
		Point CalculateCaretControlLocation() {
			Rectangle bounds = Caret.Bounds;
			return new Point((bounds.Right - this.caretControl.Width), bounds.Bottom);
		}
		void ChangeDragCaretControl() {
			switch (CaretType) {
				case DragCaretType.InsertList:
					SetDragCaretControl(new InsertSNListCaretControl());
					break;
				case DragCaretType.InsertField:
					SetDragCaretControl(new InsertSNTextCaretControl());
					break;
				case DragCaretType.InsertMasterDetail:
					SetDragCaretControl(new InsertMasterDetailCaretControl());
					break;
				default:
					SetDragCaretControl(null);
					break;
			}
		}
		void SetDragCaretControl(DragCaretControl caretControl) {
			if (CaretControl != null)
				WinControl.Controls.Remove(CaretControl);
			this.caretControl = caretControl;
			WinControl.Controls.Add(CaretControl);
		}
		public override void HideCaret(DocumentLogPosition caretLogPosition) {
			if (CaretControl != null)
				CaretControl.Hide();
			base.HideCaret(caretLogPosition);
		}
		public override void Finish() {
			if (CaretControl != null)
				WinControl.Controls.Remove(CaretControl);
			base.Finish();
		}
	}
	#endregion
}
