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
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseObjectMemberPage : ChooseObjectPageBase<ObjectMember, IEnumerable<ObjectMember>>, IChooseObjectMemberPageView {
		public static ChooseObjectMemberPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseObjectMemberPage(model));
		}
		protected ChooseObjectMemberPage(DataSourceWizardModelBase model) : base(x => x.Highlighted, x => x, model) { }
		readonly Lazy<IEnumerable<BooleanViewModel<SinglePropertyViewModel<bool>>>> options = BooleanViewModel.CreateList(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToObject), DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToMember), false, _ => SinglePropertyViewModel<bool>.Create(true));
		public IEnumerable<BooleanViewModel<SinglePropertyViewModel<bool>>> Options { get { return options.Value; } }
		[RaiseChanged]
		public virtual bool IsStaticType { get; set; }
		void IChooseObjectMemberPageView.Initialize(IEnumerable<ObjectMember> items, bool staticType, bool showAll) {
			IsStaticType = staticType;
			SetData(items, showAll);
			Options.First().Properties.Value = !staticType;
			IsStaticType = staticType;
		}
	}
}
