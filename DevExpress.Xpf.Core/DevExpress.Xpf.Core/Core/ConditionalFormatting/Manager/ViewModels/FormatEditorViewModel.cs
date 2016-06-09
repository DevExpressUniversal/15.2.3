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
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public abstract class FormatEditorOwnerViewModel : ManagerViewModelBase, IConditionEditor {
		public virtual Format Format { get; set; }
		protected FormatEditorOwnerViewModel(IDialogContext context) : base(context) { }
		public virtual IDialogService DialogService { get { return null; } }
		BaseEditUnit IConditionEditor.Edit() {
			var unit = CreateEditUnit();
			AddChanges(unit);
			return unit;
		}
		void IConditionEditor.Init(BaseEditUnit unit) {
			(unit as ConditionEditUnit).Do(x => InitCore(x));
		}
		bool IConditionEditor.CanInit(BaseEditUnit unit) {
			return (unit as ConditionEditUnit).If(x => CanInitCore(x)) != null;
		}
		bool IConditionEditor.Validate() {
			return ValidateExpression();
		}
		protected virtual ConditionEditUnit CreateEditUnit() {
			return new ConditionEditUnit();
		}
		protected virtual void AddChanges(ConditionEditUnit unit) {
			unit.Format = Format;
		}
		protected virtual void InitCore(ConditionEditUnit unit) {
			Format = unit.Format;
		}
		protected abstract bool CanInitCore(ConditionEditUnit unit);
		protected virtual bool ValidateExpression() {
			return true;
		}
		public void ShowFormatEditor() {
			var viewModel = CreateFormatViewModel();
			if(ManagerHelper.ShowDialog(viewModel, viewModel.Description, DialogService))
				Format = viewModel.CreateFormat();
		}
		FormatEditorViewModel CreateFormatViewModel() {
			var viewModel = FormatEditorViewModel.Factory(Context);
			viewModel.Init(Format);
			return viewModel;
		}
	}
}
