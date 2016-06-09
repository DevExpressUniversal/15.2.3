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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShapeMoveAndResizeCommand
	public class ShapeMoveAndResizeCommand : SpreadsheetDrawingCommand {
		#region Fields
		MoveAndResizeShapeInfo info;
		#endregion
		public ShapeMoveAndResizeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ShapeMoveAndResize; } }
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.Move; } }
		public override void ForceExecute(ICommandUIState state) {
			DefaultValueBasedCommandUIState<MoveAndResizeShapeInfo> valueBasedState = state as DefaultValueBasedCommandUIState<MoveAndResizeShapeInfo>;
			if (valueBasedState == null)
				return;
			this.info = valueBasedState.Value;
			base.ForceExecute(state);
		}
		protected override void ApplyCommandRestriction(ICommandUIState state) {
			base.ApplyCommandRestriction(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.BehaviorOptions.Drawing.Resize);
		}
		protected internal override void ModifyDrawing(IDrawingObject drawing) {
			int drawingIndex = drawing.IndexInCollection;
			ActiveSheet.MoveDrawingInPixels(drawingIndex, info.OffsetX, info.OffsetY);
			ActiveSheet.SetDrawingSizeIndependent(drawingIndex, info.NewWidth, info.NewHeight);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<MoveAndResizeShapeInfo>();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region MoveAndResizeShapeInfo
	public class MoveAndResizeShapeInfo {
		#region Fields
		int offsetX;
		int offsetY;
		int newWidth;
		int newHeight;
		#endregion
		public MoveAndResizeShapeInfo() {
		}
		public MoveAndResizeShapeInfo(int offsetX, int offsetY, int newWidth, int newHeight) {
			this.offsetX = offsetX;
			this.offsetY = offsetY;
			this.newWidth = newWidth;
			this.newHeight = newHeight;
		}
		#region Properties
		public int OffsetX { get { return offsetX; } set { offsetX = value; } }
		public int OffsetY { get { return offsetY; } set { offsetY = value; } }
		public int NewWidth { get { return newWidth; } set { newWidth = value; } }
		public int NewHeight { get { return newHeight; } set { newHeight = value; } }
		#endregion
	}
	#endregion
}
