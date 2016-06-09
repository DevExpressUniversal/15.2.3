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

using DevExpress.Mvvm;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Localization;
using System.Windows;
using System.Windows.Input;
using System;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	public class ReportInfoViewModel {
		ReportManagerServiceViewModel viewModel;
		public ReportInfo Info { get; private set; }
		public ReportInfoViewModel(ReportManagerServiceViewModel viewModel, ReportInfo info) {
			this.viewModel = viewModel;
			Info = info;
			ShowCommand = new DelegateCommand(OnShowReport);
			RenameCommand = new DelegateCommand(OnRenameReport);
			DeleteCommand = new DelegateCommand(OnDeleteReport);
			PreviewCommand = new DelegateCommand(OnPreviewReport);
		}
		public ICommand ShowCommand { get; private set; }
		public ICommand RenameCommand { get; private set; }
		public ICommand DeleteCommand { get; private set; }
		public ICommand PreviewCommand { get; private set; }
		Window OwnerWindow {
			get {
				return Window.GetWindow(viewModel.service.AssociatedObject);
			}
		}
		void OnDeleteReport() {
			if(viewModel.ShowReportNameDialog(OwnerWindow, "DeleteDialogTemplate", Info.Name ?? "", 
				PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_DeleteDialogCaption)) == null)
				return;
			viewModel.service.RemoveReport(Info);
		}
		void OnRenameReport() {
			var name = viewModel.ShowReportNameDialog(OwnerWindow, "RenameDialogTemplate", Info.Name,
				PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_RenameDialogCaption));
			if(name == null)
				return;
			Info.Name = name;
			viewModel.service.UpdateReportInfo(Info);
		}
		void OnShowReport() {
			viewModel.EditReportCommand.Execute(Info);
		}
		void OnPreviewReport() {
			viewModel.ShowReportPreviewCommand.Execute(Info);
		}
	}
}
