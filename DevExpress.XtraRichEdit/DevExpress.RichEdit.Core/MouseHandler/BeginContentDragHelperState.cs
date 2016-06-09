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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	#region BeginContentDragHelperState
	public class BeginContentDragHelperState : RichEditBeginMouseDragHelperState {
		bool resetSelectionOnMouseUp = true;
		public BeginContentDragHelperState(RichEditMouseHandler mouseHandler, MouseHandlerState dragState, Point point)
			: base(mouseHandler, dragState, point) {
		}
		public bool ResetSelectionOnMouseUp { get { return resetSelectionOnMouseUp; } set { resetSelectionOnMouseUp = value; } }
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if (ResetSelectionOnMouseUp) {
				RichEditMouseHandler handler = (RichEditMouseHandler)MouseHandler;
				PlaceCaretToPhysicalPointCommand command = new PlaceCaretToPhysicalPointCommand2(handler.Control);
				command.PhysicalPoint = new Point(e.X, e.Y);
				handler.Control.InnerControl.DocumentModel.Selection.ClearMultiSelection();
				command.ExecuteCore();
			}
		}
	}
	#endregion
	#region RichEditBeginMouseDragHelperState
	public class RichEditBeginMouseDragHelperState : BeginMouseDragHelperState {
		Point initialPoint;
		public RichEditBeginMouseDragHelperState(RichEditMouseHandler mouseHandler, MouseHandlerState dragState, Point point)
			: base(mouseHandler, dragState, point) {
				this.initialPoint = point;
		}
		DocumentModel DocumentModel { get { return ((RichEditMouseHandler)MouseHandler).Control.InnerControl.DocumentModel; } }
		public override void Start() {
			base.Start();
			DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
		}
		public override void Finish() {
			base.Finish();
			DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
		}
		void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			IEndDocumentUpdateHandler handler = DragState as IEndDocumentUpdateHandler;
			if (handler != null)
				handler.HandleEndDocumentUpdate(e);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			MouseHandler.SwitchStateCore(DragState, initialPoint);
			DragState.OnMouseMove(e);
		}
	}
	#endregion
}
