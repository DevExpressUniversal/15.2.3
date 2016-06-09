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
	public class ChooseObjectConstructorPage<TModel> : WizardPageBase<IChooseObjectConstructorPageView, TModel> where TModel : IObjectDataSourceModel {
		readonly OperationMode operationMode;
		ConstructorInfo[] constructors;
		ConstructorViewInfo[] constructorViewInfos;
		public ChooseObjectConstructorPage(IChooseObjectConstructorPageView view, OperationMode operationMode) : base(view) {
			this.operationMode = operationMode;
		}
		#region Overrides of WizardPageBase<IChooseObjectConstructorPageView,TModel>
		public override void Begin() {
			constructors = Model.ObjectType.ResolveType().GetConstructors();
			if(Model.ObjectConstructor != null && !constructors.Contains(Model.ObjectConstructor)) {
				Model.ObjectConstructor = constructors.FirstOrDefault(ci => ci.Equals(Model.ObjectConstructor));
			}
			ConstructorViewInfo selected = null;
			constructorViewInfos = constructors.Select(
				ci => {
					ConstructorViewInfo item = new ConstructorViewInfo(
						ci.GetCustomAttributes(typeof(HighlightedMemberAttribute), false).Length > 0,
						new ParametersViewInfo(ci.GetParameters()));
					if(ci == Model.ObjectConstructor)
						selected = item;
					return item;
				}).ToArray();
			View.Initialize(constructorViewInfos, Model.ShowAllState.HasFlag(ShowAllState.Constructors));
			View.Result = selected;
			View.Changed += View_Changed;
		}
		public override void Commit() {
			ConstructorInfo constructorInfo = GetResult();
			if(Model.ObjectConstructor != constructorInfo) {
				Model.ObjectConstructor = constructorInfo;
				Model.CtorParameters = null;
			}
			if(View.ShowAll)
				Model.ShowAllState |= ShowAllState.Constructors;
			else
				Model.ShowAllState &= ~ShowAllState.Constructors;
			View.Changed -= View_Changed;
		}
		public override Type GetNextPageType() {
			ConstructorInfo ctor = GetResult();
			return ObjectBindingPagesRouter.AfterChooseObjectConstructorPage<TModel>(ctor, operationMode);
		}
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return GetNextPageType() == null; } }
		#endregion
		protected ConstructorInfo GetResult() {
			ConstructorViewInfo viewResult = View.Result;
			if(viewResult == null)
				return null;
			int index = constructorViewInfos.ToList().IndexOf(viewResult);
			if(index < 0)
				return null;
			return constructors[index];
		}
		void View_Changed(object sender, EventArgs e) { RaiseChanged(); }
	}
}
