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

using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public class ReportDesignerCommands : ImmutableObject {
		readonly DelegateCommand openDocumentCommand;
		readonly DelegateCommand exitCommand;
		readonly DelegateCommand<XtraReport> newDocumentCommand;
		public ReportDesignerCommands(ReportDesigner designer) {
			openDocumentCommand = new DelegateCommand(() => designer.OpenDocument(), false);
			exitCommand = new DelegateCommand(() => designer.Exit(), false);
			newDocumentCommand = new DelegateCommand<XtraReport>(x => designer.NewDocument(x), false);
		}
		public ICommand OpenDocument { get { return openDocumentCommand; } }
		public ICommand Exit { get { return exitCommand; } }
		public ICommand<XtraReport> NewDocument { get { return newDocumentCommand; } }
	}
}
