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
using System.ComponentModel.Design;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public class ToolTipService {
		#region static
		public static readonly ToolTipService NullToolTipService = new NullToolTipService(null);
		public static readonly Control NullView = new Control();
		public static ToolTipService GetInstance(IServiceProvider serviceProvider) {
			return (ToolTipService)ServiceHelper.GetServiceInstance(serviceProvider, typeof(ToolTipService), NullToolTipService);
		}
		public static ToolTipControlInfo GetListToolTipInfo(Control control, System.Drawing.Point point, System.Drawing.Rectangle bounds, string text, object toolTipObject) {
			if(bounds.Contains(point)) {
				ToolTipControlInfo tooltipInfo = new ToolTipControlInfo(toolTipObject, text, true, ToolTipIconType.None);
				tooltipInfo.ToolTipLocation = ToolTipLocation.Fixed;
				tooltipInfo.ToolTipPosition = control.PointToScreen(new System.Drawing.Point(point.X, bounds.Bottom));
				tooltipInfo.ToolTipType = ToolTipType.SuperTip;
				return tooltipInfo;
			}
			return null;
		}
		#endregion
		IServiceProvider serviceProvider;
		XtraReport rootReport;
		string toolTipText;
		Control View { 
			get {
				IBandViewInfoService bandViewInfoService = (IBandViewInfoService)serviceProvider.GetService(typeof(IBandViewInfoService));
				return bandViewInfoService != null ? bandViewInfoService.View : NullView;
			}
		}
		public bool IsToolTipActive {
			get { return !String.IsNullOrEmpty(toolTipText) && rootReport != null && rootReport.DesignerOptions.ShowDesignerHints; }
		}
		public string ToolTipText {
			get { return toolTipText; }
		}
		public ToolTipService(IServiceProvider serviceProvider) {
			ToolTipController.DefaultController.AutoPopDelay = 10000;
			this.serviceProvider = serviceProvider;
			if(serviceProvider != null) {
				IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(designerHost != null)
					rootReport = designerHost.RootComponent as XtraReport;
			}
		}
		public virtual void SetToolTip(string toolTipText) {
			this.toolTipText = toolTipText;
		}
		public virtual void ShowHint(string hint) {
			if(String.IsNullOrEmpty(hint))
				HideHint();
			else {
				ToolTipController.DefaultController.ShowHint(hint, System.Windows.Forms.Control.MousePosition);
			}
		}
		public virtual void HideHint() {
			ToolTipController.DefaultController.HideHint();
		}
	}
	public class NullToolTipService : ToolTipService {
		public NullToolTipService(IServiceProvider serviceProvider) : base(serviceProvider) { 
		}
		public override void SetToolTip(string toolTipText) {
		}
		public override void ShowHint(string hint) {
		}
		public override void HideHint() {
		}
	}
}
