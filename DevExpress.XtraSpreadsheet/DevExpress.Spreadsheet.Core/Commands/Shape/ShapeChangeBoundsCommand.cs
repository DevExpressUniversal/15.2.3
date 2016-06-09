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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShapeChangeBoundsCommand
	public class ShapeChangeBoundsCommand : SpreadsheetDrawingCommand {
		public ShapeChangeBoundsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ShapeChangeBounds; } }
		public Rectangle Bounds { get; set; } 
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.Move; } }
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<Rectangle>();
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<Rectangle> valueBasedState = state as IValueBasedCommandUIState<Rectangle>;
			if (valueBasedState != null) {
				this.Bounds = valueBasedState.Value;
				base.ForceExecute(state);
			}
		}
		protected override void ApplyCommandRestriction(ICommandUIState state) {
			base.ApplyCommandRestriction(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.BehaviorOptions.Drawing.Resize);
		}
		protected internal override void ExecuteCore() {
			DocumentLayout layout = InnerControl.DesignDocumentLayout;
			if (layout != null)
				base.ExecuteCore();
		}
		protected internal override void ModifyDrawing(IDrawingObject drawing) {
			DrawingBox drawingBox = LookupDrawingBox(drawing);
			if (drawingBox == null)
				return;
			Rectangle initialBounds = drawingBox.Bounds;
			ActiveSheet.SetDrawingSizeIndependent(drawing.IndexInCollection, Bounds.Size);
			ActiveSheet.MoveDrawingInLayoutUnits(drawing.IndexInCollection, Bounds.X - initialBounds.X, Bounds.Y - initialBounds.Y);
		}
		DrawingBox LookupDrawingBox(IDrawingObject drawing) {
			DocumentLayout layout = InnerControl.DesignDocumentLayout;
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			List<DrawingBox> boxes = lastPage.DrawingBoxes;
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				if (boxes[i].DrawingIndex == drawing.IndexInCollection)
					return boxes[i];
			return null;
		}
	}
	#endregion
}
