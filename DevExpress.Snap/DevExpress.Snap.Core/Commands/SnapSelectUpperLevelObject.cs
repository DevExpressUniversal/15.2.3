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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Commands {
	public class SnapSelectUpperLevelObjectCommand : SelectUpperLevelObjectCommand {
		public SnapSelectUpperLevelObjectCommand(IRichEditControl control)
			: base(control) {
		}
		bool baseEnabled;
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!state.Enabled)
				state.Enabled = IsChartSelected();
			baseEnabled = state.Enabled;
			state.Enabled = true;
		}
		protected internal override void ExecuteCore() {
			if (baseEnabled)
				base.ExecuteCore();
			else {
				((InnerSnapControl)InnerControl).DrawTemplateDecorators = false;
				Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.AllDocument);
			}
		}
		protected internal override bool CanResetCurrentObjectSelection() {
			return base.CanResetCurrentObjectSelection() || IsChartSelected();
		}
		bool IsChartSelected() {
			SnapDocumentModel model = (SnapDocumentModel)DocumentModel;
			return model.Selection.Length > 0 && FieldsHelper.GetParsedInfo(model) is SNChartField;
		}
	}
}
