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
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ConfigureEFConnectionStringPage : DataSourceWizardPage, IConfigureEFConnectionStringPageView {
		public static ConfigureEFConnectionStringPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ConfigureEFConnectionStringPage(model));
		}
		protected ConfigureEFConnectionStringPage(DataSourceWizardModelBase model) : base(model) { }
		bool canSaveToStorage;
		public bool CanSaveToStorage { get { return canSaveToStorage; } }
		[RaiseChanged]
		public virtual bool UseDefaultConnectionString { get; set; }
		[RaiseChanged]
		public virtual bool UseSaveConnection { get; set; }
		[RaiseChanged]
		public virtual string TextConnectionName { get; set; }
		[RaiseChanged]
		public virtual string TextConnectionString { get; set; }
		readonly Lazy<IEnumerable<BooleanViewModel>> options = BooleanViewModel.CreateList(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageEFConnectionProperties_DefaultConnection), DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageEFConnectionProperties_CustomConnection), true);
		public IEnumerable<BooleanViewModel> Options { get { return options.Value; } }
		bool IConfigureEFConnectionStringPageView.ShouldSaveConnectionString {
			get { return !UseDefaultConnectionString && UseSaveConnection; }
			set {
				if(!UseDefaultConnectionString)
					UseSaveConnection = value;
			}
		}
		void IConfigureEFConnectionStringPageView.SetCanSaveToStorage(bool value) {
			this.canSaveToStorage = value;
			if(!UseDefaultConnectionString)
				UseSaveConnection = CanSaveToStorage;
		}
		string IConfigureEFConnectionStringPageView.ConnectionName {
			get { return UseDefaultConnectionString ? string.Empty : TextConnectionName; }
			set {
				if(!UseDefaultConnectionString)
					TextConnectionName = value;
			}
		}
		string IConfigureEFConnectionStringPageView.ConnectionString {
			get { return UseDefaultConnectionString ? string.Empty : TextConnectionString; }
			set {
				if(!UseDefaultConnectionString)
					TextConnectionString = value;
			}
		}
	}
}
