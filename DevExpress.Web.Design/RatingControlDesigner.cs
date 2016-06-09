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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.Design;
using System.ComponentModel.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxRatingControlDesigner : ASPxWebControlDesigner {
		ASPxRatingControl ratingControl;
		ASPxRatingControl RatingControl { get { return ratingControl; } }
		public override void Initialize(System.ComponentModel.IComponent component) {
			this.ratingControl = (ASPxRatingControl)component;
			base.Initialize(component);
		}
		protected override ASPxWebControlDesignerActionList  CreateCommonActionList() {
			return new ASPxRatingControlDesignerActionList(this);
		}
	}
	public class ASPxRatingControlDesignerActionList : ASPxWebControlDesignerActionList {
		ASPxRatingControlDesigner designer;
		public ASPxRatingControlDesignerActionList(ASPxRatingControlDesigner designer)
		: base(designer) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionMethodItem(this, "OpenSetImageMapPropertyHelp",
				StringResources.RatingControl_OpenSetImageMapPropertyHelpActionItem,
				StringResources.RatingControl_OpenSetImageMapPropertyHelpActionItemDescription, true));
			return collection;
		}
		protected void OpenSetImageMapPropertyHelp() {
			ShowHelpFromUrl("#AspNet/CustomDocument6618");
		}
	}
}
