#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.XtraLayout;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using DevExpress.XtraTab;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
	internal class XtraLayoutTabControl : TestControlInterfaceImplementerBase<LayoutGroup>, IControlAct, IControlEnabled {
		public XtraLayoutTabControl(LayoutGroup control)
			: base(control) {
		}
		public bool Enabled {
			get { return control.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never; } 
		}
		#region IControlAct Members
		public void Act(string value) {
			control.ParentTabbedGroup.SelectedTabPage = control;
		}
		#endregion
	}
	internal class XtraTabPageControl : TestControlInterfaceImplementerBase<XtraTabPage>, IControlAct, IControlEnabled {
		public XtraTabPageControl(XtraTabPage control)
			: base(control) {
		}
		public void Act(string value) {
			((XtraTabControl)(control.Parent)).SelectedTabPage = control;
		}
		public bool Enabled {
			get { return ((XtraTabControl)(control.Parent)).Enabled; }
		}
	}
}
