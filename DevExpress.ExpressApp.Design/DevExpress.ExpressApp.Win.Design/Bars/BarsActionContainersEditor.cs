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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public class BarsActionContainersEditor : BaseActionContainersEditor {
		private const int BarLinkActionControlContainerImageIndex = 0;
		protected override void InitializeDesigner() {
			BarControlsList.Manager = Manager;
			ActionsUiTree.ImageList = ResourceImageHelper.CreateImageListFromResources("DevExpress.ExpressApp.Win.Design.Images.BarLinkActionControlContainer.png", typeof(XafBarAndDockingDesigner).Assembly, new Size(16, 16));
			base.InitializeDesigner();
		}
		protected override void CreateTreeNode(string caption, object container) {
			CreateTreeNode(caption, container, BarLinkActionControlContainerImageIndex);
		}
		protected override BarItems BarManagerItems {
			get { return Manager.Items; }
		}
		protected override IList<IActionControlContainer> ActionContainers {
			get { return Manager.ActionContainers; }
		}
		protected override IContainer FormContainer {
			get { return Manager.Container; }
		}
		public XafBarManagerV2 Manager {
			get {
				if(EditingObject == null) return null;
				return EditingObject as XafBarManagerV2;
			}
		}
	}
}
