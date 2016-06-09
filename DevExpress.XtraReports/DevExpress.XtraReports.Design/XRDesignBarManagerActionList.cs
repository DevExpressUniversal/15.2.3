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
using DevExpress.XtraPrinting.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Design {
	public class XRDesignBarManagerActionList : SpecializedBarManagerActionList {
		protected override string HeaderText { get { return "XtraReport"; } 
		}
		public XRDesignBarManagerActionList(SpecializedBarManagerDesigner designer) : base(designer) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = base.GetSortedActionItems();
			IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host.FirstComponent<XRDesignDockManager>() == null)
				result.Add(new DesignerActionMethodItem(this, "CreateToolWindows", "Create Tool Windows", "Toolbar", false));
			return result;
		}
		void CreateToolWindows() {
			AddComponent(new XRDesignDockManager());
		}
		void AddComponent(Component comp) {
			IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			host.AddToContainer(comp, true);
			DesignerActionUIService serv = host.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(serv != null)
				serv.Refresh(Component);
		}
	}
}
