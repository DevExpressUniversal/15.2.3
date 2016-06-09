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

using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates {
	public partial class PopupForm : PopupFormBase {
		protected override XafLayoutControl BottomPanel {
			get { return bottomPanel; }
		}
		protected override PanelControl ViewSitePanel {
			get { return viewSitePanel; }
		}
		protected override ViewSiteManager ViewSiteManager {
			get { return viewSiteManager; }
		}
		protected override FormStateModelSynchronizer FormStateModelSynchronizer {
			get { return formStateModelSynchronizer; }
		}
		public override ICollection<IActionContainer> GetContainers() {
			return actionContainersManager.GetContainers();
		}
		public override IActionContainer DefaultContainer {
			get { return actionContainersManager.DefaultContainer; }
		}
		public ButtonsContainer ButtonsContainer {
			get { return buttonsContainer; }
		}
		public ButtonsContainer DiagnosticContainer {
			get { return diagnosticContainer; }
		}
		public override object ViewSiteControl {
			get { return viewSitePanel; }
		}
		public override DevExpress.XtraBars.BarManager BarManager {
			get { return xafBarManager; }
		}
		public PopupForm() {
			InitializeComponent();
		}
	}
}
