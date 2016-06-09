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

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.Xpf.Core.Design {
	class DXContextMenuProvider : ContextMenuProvider {
		MenuAction AboutItem { get; set; }
		public DXContextMenuProvider() {
			AboutItem = new MenuAction("About...");
			AboutItem.Execute += OnAboutItemExecute;
			Items.Add(AboutItem);
			UpdateItemStatus += OnUpdateItemStatus;
		}
#if SL
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			ModelItem primary = e.Selection.PrimarySelection;
			AboutItem.Visible = CheckIsDXType(primary.ItemType);
		}
		bool CheckIsDXType(Type type) {
			return type.Assembly.FullName.StartsWith("DevExpress");
		}
		void OnAboutItemExecute(object sender, Microsoft.Windows.Design.Interaction.MenuActionEventArgs e) {
			DevExpress.Xpf.About.ShowAbout(DevExpress.Utils.About.ProductKind.DXperienceSliverlight);
		}
#else
		Type VisibleType { get; set; }
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			if (!CheckIsDXType(e.Selection.PrimarySelection.ItemType)) {
				AboutItem.Visible = false;
				return;
			}
			VisibleType = GetVisibleType(e.Selection.PrimarySelection);
			AboutItem.Visible = VisibleType != null;
		}
		Type GetVisibleType(ModelItem modelItem) {
			try {
				if (modelItem == null)
					return null;
				Type currentType = modelItem.ItemType;
						return currentType;
			} catch {
				return null;
			}
		}
		bool CheckIsDXType(Type type) {
			return type.Assembly.FullName.StartsWith("DevExpress");
		}
		void OnAboutItemExecute(object sender, Microsoft.Windows.Design.Interaction.MenuActionEventArgs e) {
			About.ShowAbout(VisibleType);
		}
#endif
	}
}
