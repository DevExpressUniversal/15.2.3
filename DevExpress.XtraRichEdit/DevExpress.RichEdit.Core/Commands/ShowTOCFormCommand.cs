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

using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	public class ShowTOCFormCommand: RichEditSelectionCommand {
		Field field;
		public ShowTOCFormCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTOCFormCommandId")]
#endif
		public override RichEditCommandId Id {
			get {
				return RichEditCommandId.ShowTOCForm;
			}
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTOCFormCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId {
			get { return Localization.XtraRichEditStringId.MenuCmd_EditTOC;
			}
		}
		protected internal override void ExecuteCore() {
			Control.ShowTOCForm(field);
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			FieldController controller = new FieldController();
			TocField tocField;
			field = controller.FindFieldBySelection<TocField>(DocumentModel.Selection, out tocField);
			state.Enabled = (field != null) && CheckIsContentEditable();
			state.Visible = field != null;
		}
		protected internal override bool CanChangePosition(Model.DocumentModelPosition pos) {
			return false;
		}
		protected internal override bool TryToKeepCaretX {
			get { return false; }
		}
		protected internal override bool TreatStartPositionAsCurrent {
			get { return false; }
		}
		protected internal override bool ExtendSelection {
			get { return false; }
		}
		protected internal override Layout.DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel {
			get { return Layout.DocumentLayoutDetailsLevel.None; }
		}
		protected internal override Model.DocumentLogPosition ChangePosition(Model.DocumentModelPosition pos) {
			return pos.LogPosition;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTOCFormCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId {
			get { throw new NotImplementedException(); }
		}
	}
}
