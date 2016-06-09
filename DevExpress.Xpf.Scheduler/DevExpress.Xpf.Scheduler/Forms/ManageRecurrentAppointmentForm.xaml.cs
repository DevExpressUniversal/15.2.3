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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class ManageRecurrentAppointmentControl : UserControl {
		public ManageRecurrentAppointmentControl() {
			InitializeComponent();
#if DEBUGTEST
			Loaded += new RoutedEventHandler(AskRecurrentAppointmentControl_Loaded);
#endif
		}
#if DEBUGTEST
		void AskRecurrentAppointmentControl_Loaded(object sender, RoutedEventArgs e) {
			if(DevExpress.Xpf.Scheduler.Tests.TestFormHelper.IsTest) {
#if !SL
				DevExpress.Xpf.Scheduler.Tests.TestWindowService service = new DevExpress.Xpf.Scheduler.Tests.TestWindowService();
				if(!DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(this).Contains(service))
					DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(this).Add(service);
#endif
				DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Add(DataContext, this);
				DevExpress.Xpf.Scheduler.Tests.TestFormHelper.TestDelegate(this, DataContext);
			}
		}
#endif
	}
}
