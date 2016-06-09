﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region SpreadsheetToolsCommandGroup (abstract class)
	public abstract class SpreadsheetToolsCommandGroup : SpreadsheetCommand {
		protected SpreadsheetToolsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = DocumentServer.IsEditable && IsToolsActive();
		}
		protected abstract bool IsToolsActive();
	}
	#endregion
	#region ToolsPictureCommandGroup
	public class ToolsPictureCommandGroup : SpreadsheetToolsCommandGroup {
		public ToolsPictureCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ToolsPictureCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		protected override bool IsToolsActive() {
			List<int> drawingIndexes = ActiveSheet.Selection.SelectedDrawingIndexes;
			if (drawingIndexes.Count == 0)
				return false;
			DrawingObjectsCollection drawings = ActiveSheet.DrawingObjects;
			foreach (int index in drawingIndexes) {
				if (drawings[index].DrawingType != DrawingObjectType.Picture)
					return false;
			}
			return true;
		}
	}
	#endregion
}
