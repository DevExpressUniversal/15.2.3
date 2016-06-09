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
	public class ChooseObjectBindingModePage<TModel> : WizardPageBase<IChooseObjectBindingModePageView, TModel> where TModel : IObjectDataSourceModel {
		readonly OperationMode operationMode;
		ConstructorInfo defaultConstructorInfo;
		public ChooseObjectBindingModePage(IChooseObjectBindingModePageView view, OperationMode operationMode) : base(view) {
			this.operationMode = operationMode;
		}
		#region Overrides of WizardPageBase<IChooseObjectBindingModePageView,TModel>
		public override void Begin() {
			View.SchemaOnly = Model.ObjectConstructor == null;
			View.Changed += ViewOnChanged;
		}
		public override void Commit() {
			View.Changed -= ViewOnChanged;
			if(View.SchemaOnly) {
				Model.ObjectConstructor = null;
				Model.CtorParameters = null;
			}
			else {
				if(Model.ObjectConstructor != null)
					return;
				Model.ObjectConstructor = DefaultConstructorInfo;
				Model.CtorParameters = null;
			}
		}
		public override Type GetNextPageType() {
			Type objType = Model.ObjectType.ResolveType();
			ConstructorInfo ctor = GetResult();
			return ObjectBindingPagesRouter.AfterChooseObjectBindingModePage<TModel>(objType, ctor, operationMode);
		}
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return GetNextPageType() == null; } }
		#endregion
		void ViewOnChanged(object sender, EventArgs eventArgs) { RaiseChanged(); }
		protected ConstructorInfo GetResult() {
			return View.SchemaOnly
				? null
				: (Model.ObjectConstructor ?? DefaultConstructorInfo);
		}
		protected ConstructorInfo DefaultConstructorInfo {
			get {
				if(defaultConstructorInfo == null) {
					ConstructorInfo[] infos = Model.ObjectType.ResolveType().GetConstructors().OrderBy(info => info.GetParameters().Length).ToArray();
					defaultConstructorInfo =
						infos.FirstOrDefault(
							info => info.GetCustomAttributes(typeof(HighlightedMemberAttribute), false).Any()) ??
						infos.FirstOrDefault();
				}
				return defaultConstructorInfo;
			}
		}
	}
}
