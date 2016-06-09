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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public delegate void ChangeFieldAction<U>(InstructionController controller, U newValue);
	public interface IFieldChanger {
		void ApplyNewValue<U>(ChangeFieldAction<U> changeFieldAction, U newValue);
		void ApplyNewValue(Action<InstructionController> changeFieldAction);
	}
	public class FieldChanger : IFieldChanger {
		readonly IFieldInfo fieldInfo;
		readonly IParsedInfoProvider parsedInfoProvider;
		public FieldChanger(IFieldInfo fieldInfo, IParsedInfoProvider parsedInfoProvider) {
			this.fieldInfo = fieldInfo;
			this.parsedInfoProvider = parsedInfoProvider;
		}
		SnapPieceTable PieceTable { get { return fieldInfo.PieceTable; } }
		Field Field { get { return fieldInfo.Field; } }
		public void ApplyNewValue<U>(ChangeFieldAction<U> changeFieldAction, U newValue) {
			InstructionController controller = BeginUpdateFieldCode();
			try {
				changeFieldAction(controller, newValue);
			} finally {
				EndUpdateFieldCode(controller);
			}
		}
		public void ApplyNewValue(Action<InstructionController> changeFieldAction) {
			InstructionController controller = BeginUpdateFieldCode();
			try {
				changeFieldAction(controller);
			}
			finally {
				EndUpdateFieldCode(controller);
			}
		}
		InstructionController CreateInstructionController() {
			CalculatedFieldBase parsedInfo = parsedInfoProvider.GetParsedInfo();
			if (parsedInfo == null)
				return null;
			return new InstructionController(PieceTable, parsedInfo, Field);
		}
		InstructionController BeginUpdateFieldCode() {
			return CreateInstructionController();
		}
		void EndUpdateFieldCode(InstructionController controller) {
			PieceTable.DocumentModel.BeginUpdate();
			try {
				controller.ApplyDeferredActions();
				parsedInfoProvider.InvalidateParsedInfo();
			} finally {
				PieceTable.DocumentModel.EndUpdate();
			}
		}
	}
}
