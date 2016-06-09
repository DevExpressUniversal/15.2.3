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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.ExpressApp.Win.Templates.Ribbon {
	public class XafRibbonRegistrationInfo : RibbonRegistrationInfo {
		BarItemInfoCollection items;
		public XafRibbonRegistrationInfo(RibbonControl ribbon) {
			Ribbon = ribbon;
		}
		public RibbonControl Ribbon { get; private set; }
		public override string PageCategoryName {
			get { return null; }
		}
		public override Type PageCategoryType {
			get { return null; }
		}
		public override string PageGroupName {
			get { return null; }
		}
		public override Type PageGroupType {
			get { return null; }
		}
		public override string PageName {
			get { return null; }
		}
		public override Type PageType {
			get { return null; }
		}
		public override BarItemInfoCollection ItemInfoCollection {
			get {
				if(items == null) {
					items = new BarItemInfoCollection(Ribbon.GetController().PaintStyle);
					items.Add(new BarItemInfo("BarLinkContainerExItem", "Inplace Link Container", 4, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(Ribbon.GetController().PaintStyle), true, true));
				}
				return items;
			}
		}
	}
}
