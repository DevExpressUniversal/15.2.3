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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public interface IReportDesignerDocumentViewSource {
		ReportDesignerDocument Document { get; }
		event EventHandler DocumentChanged;
	}
	public sealed class ReportDesignerWindowDataContext : BindableBase, IDisposable {
		public ReportDesignerWindowDataContext(ReportDesigner designer) {
			Designer = designer;
		}
		public void Dispose() {
			Designer = null;
		}
		ICommand onWindowClosedCommand;
		public ICommand OnWindowClosedCommand {
			get {
				if(onWindowClosedCommand == null)
					onWindowClosedCommand = new DelegateCommand(() => Dispose(), false);
				return onWindowClosedCommand;
			}
		}
		ReportDesigner designer;
		public ReportDesigner Designer {
			get { return designer; }
			set { SetProperty(ref designer, value, () => Designer); }
		}
	}
	public class ReportDesignerWindowBehaviorsTemplate : ContentControl {
		public ReportDesignerWindowBehaviorsTemplate() {
			var closedEventToCommand = new EventToCommand() { EventName = "Closed" };
			BindingOperations.SetBinding(closedEventToCommand, EventToCommand.CommandProperty, new Binding(ExpressionHelper.GetPropertyName((ReportDesignerWindowDataContext x) => x.OnWindowClosedCommand)));
			Content = closedEventToCommand;
		}
	}
	public class ReportDesignerWindowViewTemplate : ContentControl {
		public ReportDesignerWindowViewTemplate () {
			HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
			VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
			IsTabStop = false;
			Focusable = false;
			SetBinding(ContentProperty, new Binding(ExpressionHelper.GetPropertyName((ReportDesignerWindowDataContext x) => x.Designer)));
		}
	}
}
