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

using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseEFDataMemberPage : DataSourceWizardPage, IChooseEFDataMemberPageView {
		public static ChooseEFDataMemberPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseEFDataMemberPage(model));
		}
		public virtual IEnumerable<EFDataMemberViewModelBase> DataMembers { get; protected set; }
		public virtual EFDataMemberViewModelBase SelectedDataMember { get; set; }
		public string DataMember {
			get { return SelectedDataMember.Return(x=> x.DataMember, ()=> null); }
		}
		public bool StoredProcChosen {
			get { return SelectedDataMember.Return(x=> x is EFDBStoredProcedureViewModel, ()=> false); }
		}
		public event EventHandler DataMemberChanged;
		protected ChooseEFDataMemberPage(DataSourceWizardModelBase model) : base(model) { }
		public void Initialize(IEnumerable<DBTable> tables, IEnumerable<DBStoredProcedure> procedures, string dataMember) {
			var tableViewModels = tables.Return(t => t.Select(x => new EFDBTableViewModel(x)), () => Enumerable.Empty<EFDataMemberViewModelBase>());
			var procViewModels = procedures.Return(p => p.Select(x => new EFDBStoredProcedureViewModel(x)), () => Enumerable.Empty<EFDataMemberViewModelBase>());
			DataMembers = tableViewModels.Concat(procViewModels).ToArray();
			SelectedDataMember = DataMembers.FirstOrDefault(x => x.DataMember == dataMember) ?? DataMembers.FirstOrDefault();
		}
		protected void OnSelectedDataMemberChanged() {
			DataMemberChanged.Do(x => x(this, EventArgs.Empty));
		}
	}
}
