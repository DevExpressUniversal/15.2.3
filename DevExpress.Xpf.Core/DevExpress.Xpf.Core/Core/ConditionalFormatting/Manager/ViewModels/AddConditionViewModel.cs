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
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public interface IConditionEditor {
		BaseEditUnit Edit();
		void Init(BaseEditUnit unit);
		bool CanInit(BaseEditUnit unit);
		bool Validate();
		string Description { get; }
	}
	public abstract class ConditionViewModelBase : ManagerViewModelBase, IConditionEditor {
		protected BaseEditUnit source;
		protected ConditionViewModelBase(IDialogContext context)
			: base(context) {
			ViewModels = CreateChildViewModels();
			SelectedViewModel = ViewModels.First();
		}
		public IEnumerable<IConditionEditor> ViewModels { get; private set; }
		public virtual IConditionEditor SelectedViewModel { get; set; }
		protected abstract IEnumerable<IConditionEditor> CreateChildViewModels();
		protected void OnSelectedViewModelChanging(IConditionEditor newValue) {
			source.Do(x => newValue.Init(x));
		}
		BaseEditUnit IConditionEditor.Edit() {
			return SelectedViewModel.Edit();
		}
		void IConditionEditor.Init(BaseEditUnit unit) {
			source = unit;
			IConditionEditor oldViewModel = SelectedViewModel;
			SelectedViewModel = ViewModels.FirstOrDefault(x => x.CanInit(unit)) ?? ViewModels.First();
			if(SelectedViewModel == oldViewModel)
				SelectedViewModel.Init(source);
		}
		bool IConditionEditor.CanInit(BaseEditUnit unit) {
			return ViewModels.Any(x => x.CanInit(unit));
		}
		bool IConditionEditor.Validate() {
			return SelectedViewModel.Validate();
		}
	}
	public class AddConditionViewModel : ConditionViewModelBase {
		public static Func<IDialogContext, AddConditionViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new AddConditionViewModel(x)); } }
		protected AddConditionViewModel(IDialogContext context) : base(context) { }
		protected override IEnumerable<IConditionEditor> CreateChildViewModels() {
			return new IConditionEditor[] { ValueBasedViewModel.Factory(Context),
				ContainViewModel.Factory(Context),
				TopBottomViewModel.Factory(Context),
				AboveBelowViewModel.Factory(Context),
				FormulaViewModel.Factory(Context) };
		}
		public override string Description { get { return GetLocalizedString(source == null ? ConditionalFormattingStringId.ConditionalFormatting_Manager_NewFormattingRule : ConditionalFormattingStringId.ConditionalFormatting_Manager_EditFormattingRule); } }
	}
}
