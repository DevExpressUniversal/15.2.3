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

using DevExpress.Xpf.Scheduler.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Scheduler.UI {
	public class ChangeLabelBarItem : ColorablePopupMenuBasedBarItem {
		Size colorIndicatorSize = new Size(10, 10);
		Thickness colorIndicatorMarging = new Thickness(3, 3, 0, 0);
		Size colorIndicatorLargeSize = new Size(22, 22);
		Thickness colorIndicatorLargeMarging = new Thickness(5, 5, 0, 0);
		internal override Thickness ColorIndicatorLargeMarging { get { return colorIndicatorLargeMarging; } }
		internal override Size ColorIndicatorLargeSize { get { return colorIndicatorLargeSize; } }
		internal override Thickness ColorIndicatorMarging { get { return colorIndicatorMarging; } }
		internal override Size ColorIndicatorSize { get { return colorIndicatorSize; } }
		protected override int GetItemCount() {
			return SchedulerControl.Storage.AppointmentStorage.Labels.Count;
		}
		protected override SchedulerUICommand CreateItemCommand(int i) {
			return new ChangeAppointmentLabelUICommand(SchedulerControl.Storage.AppointmentStorage.Labels.GetByIndex(i), i);
		}
	}
}
