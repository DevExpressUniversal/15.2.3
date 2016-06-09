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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Design;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public class XafBarAndDockingDesigner : BarAndDockingDesigner {
		Image navBarGroupImage;
		ImageList navBarItemsImages;
		protected override void CreateBarGroups() {
			base.CreateBarGroups();
			DesignerGroup actionControlsGroup = Groups.Add("XAF Action Controls", "XAF Action Containers and Controls.", navBarGroupImage, true);
			actionControlsGroup.Add("Action Containers", "Customize Action containers.", typeof(BarsActionContainersEditor), null, navBarItemsImages.Images[0], null);
			actionControlsGroup.Add("Simple / Popup Action controls", "Customize Action controls for SimpleAction and PopupWindowShowAction.", typeof(SimpleActionControlsEditor), null, navBarItemsImages.Images[1], null);
			actionControlsGroup.Add("Single Choice Action controls", "Customize Action controls for SingleChoiceAction.", typeof(SingleChoiceActionControlsEditor), null, navBarItemsImages.Images[2], null);
			actionControlsGroup.Add("Parametrized Action controls", "Customize Action controls for ParametrizedAction.", typeof(ParametrizedActionControlsEditor), null, navBarItemsImages.Images[3], null);
		}
		public XafBarAndDockingDesigner(object component) : base(component) {
			navBarGroupImage = ResourceImageHelper.CreateImageFromResources("DevExpress.ExpressApp.Win.Design.Images.NavBarGroupIcon.png", typeof(XafBarAndDockingDesigner).Assembly);
			navBarItemsImages = ResourceImageHelper.CreateImageListFromResources("DevExpress.ExpressApp.Win.Design.Images.NavBarItemsIcons.png", typeof(XafBarAndDockingDesigner).Assembly, new Size(16, 16));
		}
	}
}
