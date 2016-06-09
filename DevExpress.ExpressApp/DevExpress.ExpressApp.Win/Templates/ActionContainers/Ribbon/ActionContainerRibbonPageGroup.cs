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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class ActionContainersRibbonPageGroup : RibbonPageGroup {
		private List<ActionContainerBarItem> actionContainers = new List<ActionContainerBarItem>();
		private List<XafBarLinkContainerItem> barLinkContainers = new List<XafBarLinkContainerItem>();
		protected override void Dispose(bool disposing) {
			actionContainers.Clear();
			barLinkContainers.Clear();
			base.Dispose(disposing);
		}
		public ActionContainersRibbonPageGroup(string caption)
			: base(caption) {
			Name = caption;
			AllowTextClipping = false;
			ToolbarContentButtonLink.Item.Hint = caption;
			ShowCaptionButton = false;
		}
		public void RegisterBarLinkContainer(XafBarLinkContainerItem barLinkContainerItem) {
			if(!barLinkContainers.Contains(barLinkContainerItem)) {
				barLinkContainers.Add(barLinkContainerItem);
				if(barLinkContainerItem is ActionContainerBarItem) {
					actionContainers.Add((ActionContainerBarItem)barLinkContainerItem);
				}
			}
		}
		public IList<XafBarLinkContainerItem> BarLinkContainers {
			get { return barLinkContainers; }
		}
		public IList<ActionContainerBarItem> ActionContainers {
			get { return actionContainers; }
		}
	}
}
