#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	[DXToolboxItem(false)]
	public abstract class DashboardBackstageButton : BackstageViewButtonItem {
		IServiceProvider serviceProvider;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DashboardDesigner DashboardDesigner { get { return null; } set { ServiceProvider = value; } }
		public IServiceProvider ServiceProvider { get { return serviceProvider; } set { serviceProvider = value; } }
		protected abstract DashboardCommandId CommandId { get; }
		protected virtual ICommandUIState UIState { get { return null; } }
		protected override void Execute() {
			if(serviceProvider != null) {
				IDashboardCommandService commandService = serviceProvider.GetService<IDashboardCommandService>();
				if (commandService != null)
					commandService.ExecuteCommand(CommandId, UIState);
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (ServiceProvider != null)
					ServiceProvider = null;
			}
			base.Dispose(disposing);
		}
	}
}
