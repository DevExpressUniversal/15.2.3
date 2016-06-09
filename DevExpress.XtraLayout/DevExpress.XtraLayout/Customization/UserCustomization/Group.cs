﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	class Group : InteractionMethod {
		public Group(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.Button;
			Image = UCIconsHelper.GroupIcon;
			Text = "Group";
			Category = Customization.Category.Group;
		}
		public override void Execute() {
			Owner.GetOwner().BeginUpdate();
			List<BaseLayoutItem> selItems = Owner.GetSelection();		   
			foreach(BaseLayoutItem item in selItems) {
				item.Parent.CreateGroupForSelectedItems();	
			 }
			Owner.GetOwner().EndUpdate();
		}
		public override bool CanExecute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			if(selItems.Count < 1 ) return false;
			foreach(BaseLayoutItem item in selItems) {
				if(item.Parent == null) return false;
				if(!item.Parent.CanGroupSelectedItems) return false;
			}
			return true;
		}
	}
}
