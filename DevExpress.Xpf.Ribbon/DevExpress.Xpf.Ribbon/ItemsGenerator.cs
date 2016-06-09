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
using System.Linq;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.Xpf.Ribbon.Native {
	class PagesGenerator : ICommandGroupsGenerator {
		readonly RibbonPageCategoryBase category;
		public PagesGenerator(RibbonPageCategoryBase category) {
			this.category = category;
		}
		ICommandSubGroupsGenerator ICommandGroupsGenerator.CreateGroup(string groupName) {
			string caption = groupName ?? "Home";
			RibbonPage page = category.Pages.FindOrCreateNew(x => object.Equals(x.Caption, caption), () => new RibbonPage() { Caption = caption });
			return new PageGroupsGenerator(page);
		}
	}
	class PageGroupsGenerator : ICommandSubGroupsGenerator {
		readonly RibbonPage page;
		public PageGroupsGenerator(RibbonPage page) {
			this.page = page;
		}
		ICommandsGenerator ICommandSubGroupsGenerator.CreateSubGroup(string groupName) {
			RibbonPageGroup group = page.Groups.FindOrCreateNew(x => x.Caption == groupName || (string.IsNullOrEmpty(x.Caption) && string.IsNullOrEmpty(groupName)), () => new RibbonPageGroup() { Caption = groupName });
			return new BarItemsGenerator(group, ImageType.Colored);
		}
	}
}
