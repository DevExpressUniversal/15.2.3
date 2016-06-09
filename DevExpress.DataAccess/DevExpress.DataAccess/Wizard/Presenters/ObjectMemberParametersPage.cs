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
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ObjectMemberParametersPage<TModel> : WizardPageBase<IObjectMemberParametersPageView, TModel> where TModel : IObjectDataSourceModel {
		readonly OperationMode operationMode;
		public ObjectMemberParametersPage(IObjectMemberParametersPageView view, OperationMode operationMode) : base(view) {
			this.operationMode = operationMode;
		}
		#region Overrides of WizardPageBase<IObjectMemberParametersPageView,TModel>
		public override void Begin() {
			ObjectBinding.Parameter[] parameters = Model.ObjectMember.GetParameters(Model.MemberParameters).ToArray();
			View.Initialize(parameters);
		}
		public override void Commit() { Model.MemberParameters = View.GetParameters().Select(ObjectBinding.Parameter.FromIParameter).ToArray(); }
		public override Type GetNextPageType() {
			Type objType = Model.ObjectType.ResolveType();
			ObjectMember member = Model.ObjectMember;
			ConstructorInfo ctor = Model.ObjectConstructor;
			return ObjectBindingPagesRouter.AfterObjectMemberParametersPage<TModel>(objType, member, ctor, operationMode);
		}
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return GetNextPageType() == null; } }
		#endregion
	}
}
