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

using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Scheduler.UI
{
	public partial class AppointmentRibbonForm : UserControl
	{
		public AppointmentRibbonForm()
		{
			InitializeComponent();
		#if DEBUGTEST
			Loaded +=new RoutedEventHandler(AppointmentForm_Loaded);
#endif
			if (!DesignerProperties.GetIsInDesignMode(this)) 
				AddItemLinksToRibbon();
		}
		void AddItemLinksToRibbon() {
			this.ribbonControl.ToolbarItemLinks.Add(this.btnSave);
			this.ribbonControl.ToolbarItemLinks.Add(this.btnUndo);
			this.ribbonControl.ToolbarItemLinks.Add(this.btnRedo);
			this.ribbonControl.ToolbarItemLinks.Add(this.btnPrevious);
			this.ribbonControl.ToolbarItemLinks.Add(this.btnNext);
			this.ribbonControl.ToolbarItemLinks.Add(this.toolbarBtnDelete);
			this.groupActions.ItemLinks.Add(btnSaveAndClose);
			this.groupActions.ItemLinks.Add(btnDelete);
			this.groupOptions.ItemLinks.Add(barLabel);
			this.groupOptions.ItemLinks.Add(barStatus);
			this.groupOptions.ItemLinks.Add(new BarEditItem());
			this.groupOptions.ItemLinks.Add(barResources);
			this.groupOptions.ItemLinks.Add(barResource);
			this.groupOptions.ItemLinks.Add(barReminder);
			this.groupOptions.ItemLinks.Add(btnRecurrence);
			this.groupOptions.ItemLinks.Add(btnTimeZones);
		}
#if DEBUGTEST
		void AppointmentForm_Loaded(object sender, RoutedEventArgs e) {
			if(DevExpress.Xpf.Scheduler.Tests.TestFormHelper.IsTest)
				DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Add(DataContext, this);
		}
#endif        
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			subjectEdit.Focus();
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
			subjectEdit.Focus();
		}
	}
}
