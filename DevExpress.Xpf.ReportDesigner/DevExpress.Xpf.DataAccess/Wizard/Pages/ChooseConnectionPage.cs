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
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseConnectionPage : DataSourceWizardPage, IChooseConnectionPageView {
		public static ChooseConnectionPage Create(IEnumerable<SqlDataConnection> connections, DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseConnectionPage(connections, model));
		}
		protected ChooseConnectionPage(IEnumerable<SqlDataConnection> connections, DataSourceWizardModelBase model)
			: base(model) {
			this.connections = connections.Select(x => x.Name);
			ExistingConnectionName = Connections.FirstOrDefault();
		}
		[RaiseChanged]
		public virtual bool ShouldCreateNewConnection { get; set; }
		readonly Lazy<IEnumerable<BooleanViewModel>> options = BooleanViewModel.CreateList(
			DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnection_SpecifyCustomConnection),
			DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseConnection_ChooseExistingConnection), 
			true);
		public IEnumerable<BooleanViewModel> Options { get { return options.Value; } }
		[RaiseChanged]
		public virtual string ExistingConnectionName { get; set; }
		void IChooseConnectionPageView.SetSelectedConnection(INamedItem connection) {
			if(connection == null) return;
			ExistingConnectionName = connection.With(x => x.Name);
		}
		readonly IEnumerable<string> connections;
		public IEnumerable<string> Connections { get { return connections; } }
	}
}
