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
namespace DevExpress.XtraLayout.Customization {
	public class EmptySpace : InteractionMethod {
		public EmptySpace(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.Button;
			Image = UCIconsHelper.AddIcon;
			Text = "Create empty space";
		}
		public override void Execute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			bool OneGroup = true;
			if(selItems.Count == 1) {
				if(selItems[0] != Owner.GetOwner().RootGroup) {
					selItems[0].Parent.AddItem(new EmptySpaceItem(), selItems[0], DevExpress.XtraLayout.Utils.InsertType.Bottom);
				} else Owner.GetOwner().RootGroup.AddItem(new EmptySpaceItem(), selItems[0], DevExpress.XtraLayout.Utils.InsertType.Bottom);
				return;
			}
			for(int i = 1; i < selItems.Count; i++) {
				if(selItems[i - 1].Owner != selItems[i].Owner) { OneGroup = false; }
			}
			if(!OneGroup) {
				Owner.GetOwner().RootGroup.AddItem(new EmptySpaceItem());
			} else selItems[0].Parent.AddItem(new EmptySpaceItem());
		}
	}
}
