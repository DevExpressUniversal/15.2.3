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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.Model;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseEFContextPage : DataSourceWizardPage, IChooseEFContextPageView {
		public static ChooseEFContextPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseEFContextPage(model));
		}
		protected ChooseEFContextPage(DataSourceWizardModelBase model) : base(model) { }
		public virtual IEnumerable<string> Contexts { get; protected set; }
		void IChooseEFContextPageView.RefreshContextList(IEnumerable<IContainerInfo> enumerable) {
			Contexts = enumerable.Select(x => x.FullName).ToList().AsReadOnly();
		}
		public virtual string ContextName { get; set; }
		protected void OnContextNameChanged() {
			if(contextNameChanged != null)
				contextNameChanged(this, new ContextNameChangedEventArgs(ContextName));
		}
		EventHandler<ContextNameChangedEventArgs> contextNameChanged;
		event EventHandler<ContextNameChangedEventArgs> IChooseEFContextPageView.ContextNameChanged {
			add { contextNameChanged += value; }
			remove { contextNameChanged -= value; }
		}
		public void BrowseForAssembly() {
			model.Parameters.DoWithOpenFileDialogService(dialog => {
				dialog.Filter = "Assemblies (*.dll)|*.dll|Executable (*.exe)|*.exe";
				if(dialog.ShowDialog())
					browseForAssembly.Do(x => x(this, new BrowseForAssemblyEventArgs(Path.Combine(dialog.File.DirectoryName, dialog.File.Name))));
			});
		}
		EventHandler<BrowseForAssemblyEventArgs> browseForAssembly;
		event EventHandler<BrowseForAssemblyEventArgs> IChooseEFContextPageView.BrowseForAssembly {
			add { browseForAssembly += value; }
			remove { browseForAssembly -= value; }
		}
	}
}
