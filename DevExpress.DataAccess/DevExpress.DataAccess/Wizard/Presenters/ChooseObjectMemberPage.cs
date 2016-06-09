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
using System.Linq;
using System.Reflection;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseObjectMemberPage<TModel> : WizardPageBase<IChooseObjectMemberPageView, TModel> where TModel : IObjectDataSourceModel {
		readonly IWizardRunnerContext context;
		readonly OperationMode operationMode;
		public ChooseObjectMemberPage(IChooseObjectMemberPageView view, IWizardRunnerContext context, OperationMode operationMode) : base(view) {
			this.context = context;
			this.operationMode = operationMode;
		}
		#region Overrides of WizardPageBase<IChooseObjectMemberPageView,TModel>
		public override Type GetNextPageType() {
			Type objType = Model.ObjectType.ResolveType();
			ObjectMember member = GetResult();
			ConstructorInfo ctor = member == Model.ObjectMember ? Model.ObjectConstructor : null;
			return ObjectBindingPagesRouter.AfterChooseObjectMemberPage<TModel>(objType, member, ctor, operationMode);
		}
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return GetNextPageType() == null; } }
		public override void Begin() {
			Type type = Model.ObjectType.ResolveType();
			object instance = ObjectDataSourceFillHelper.CreateInstanceForSchema(type);
			ObjectMember[] members = ObjectDataSourceFillHelper.GetItemMembers(instance).ToArray();
			View.Initialize(members, type.IsAbstract && type.IsSealed, Model.ShowAllState.HasFlag(ShowAllState.Members));
			View.Changed += View_Changed;
			View.Result =
				members.FirstOrDefault(item => ObjectMember.EqualityComparer.Equals(item, Model.ObjectMember));
		}
		public override void Commit() {
			ObjectMember member = GetResult();
			Model.ObjectMember = member;
			if(member == null || !member.HasParams)
				Model.MemberParameters = null;
			View.Changed -= View_Changed;
			if(View.ShowAll)
				Model.ShowAllState |= ShowAllState.Members;
			else
				Model.ShowAllState &= ~ShowAllState.Members;
		}
		public override bool Validate(out string errorMessage) {
			ObjectMember result = GetResult();
			if (result != null) {
				errorMessage = ObjectDataSourceFillHelper.ValidateResultType(result.ReturnType);
				if (errorMessage != null) {
					ShowMessage(errorMessage);
					return false;
				}
			}
			Type type = Model.ObjectType.ResolveType();
			object instance = ObjectDataSourceFillHelper.CreateInstanceForSchema(type);
			object data = ObjectDataSourceFillHelper.BrowseForSchema(instance, result);
			Model.DataSchema = ObjectDataSourceFillHelper.CreateTypedList(data, type.Name);
			return base.Validate(out errorMessage);
		}
		#endregion
		void View_Changed(object sender, EventArgs e) { RaiseChanged(); }
		protected ObjectMember GetResult() { return View.Result; }
		public virtual void ShowMessage(string message) { context.ShowMessage(message); }
	}
}
