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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraLayout;
namespace DevExpress.XtraLayout.Customization
{
	class GetParent: InteractionMethod {
		public GetParent(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.Button;
			Text = "Select parent";
			Image = UCIconsHelper.ParentIcon;
		}
		public override void Execute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			LayoutGroup root = Owner.GetOwner().RootGroup;
			root.StartChangeSelection(); 
			root.ClearSelection();
			LayoutGroup group  = selItems[0].Parent;
			if(group is LayoutControlGroup) {
				group.Selected = false;
				if(group.ParentTabbedGroup != null) {
					group.ParentTabbedGroup.Selected = true;
				} else group.Selected = true;
			} else group.Selected = true;
			root.EndChangeSelection();
		}
		public override bool CanExecute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			if(selItems.Count <= 0) return false;
			BaseLayoutItem parent = selItems[0].Parent;
			foreach(BaseLayoutItem item in selItems) {
				if(item.Parent != parent) return false;
			}
			if(selItems[0].Parent == null) return false;
			return true;
		}	  
	}
}
