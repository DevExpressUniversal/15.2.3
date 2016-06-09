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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseDataSourceNamePage<TModel> : WizardPageBase<IChooseDataSourceNamePageView, TModel> where TModel : IDataSourceModel {
		IDataSourceNameCreationService DataSourceNameCreator { get; set; }
		public ChooseDataSourceNamePage(IChooseDataSourceNamePageView view, IDataSourceNameCreationService dataSourceNameCreator)
			: base(view) {
			DataSourceNameCreator = dataSourceNameCreator;
		}
		public override bool FinishEnabled { get { return false; } }
		public override bool MoveNextEnabled { get { return true; } }
		public override void Begin() {
			if(!string.IsNullOrEmpty(Model.DataSourceName))
				View.DataSourceName = Model.DataSourceName;
			else
				View.DataSourceName = DataSourceNameCreator.CreateName();
		}
		public override void Commit() {
			Model.DataSourceName = View.DataSourceName;
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			if(DataSourceNameCreator.ValidateName(View.DataSourceName))
				return true;
			View.ShowErrorMessage();
			return false;
		}
		public override Type GetNextPageType() {
			return typeof(ChooseDataSourceTypePage<TModel>);
		}
	}
}
