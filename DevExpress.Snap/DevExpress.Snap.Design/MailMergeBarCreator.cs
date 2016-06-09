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
using DevExpress.Snap.Extensions.UI;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands;
namespace DevExpress.Snap.Design {
	public class MailMergeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(DataToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(MailMergeBar); } }
		public override XtraBars.Bar CreateBar() { return new MailMergeBar(); }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new MailMergeBarItemBuilder(); }
		public override XtraBars.Commands.Ribbon.CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new MailMergeRibbonPageGroup(); }
		public override XtraBars.Commands.Ribbon.CommandBasedRibbonPage CreateRibbonPageInstance() { return new MailMergeRibbonPage(); }
	}
	public class MailMergeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<XtraBars.BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new MailMergeDataSourceItem());
			items.Add(new CommandBarCheckItem() { SnapCommand = SnapCommand.MailMergeFilters });
			items.Add(new CommandBarCheckItem() { SnapCommand = SnapCommand.MailMergeSorting });
		}
	}
}
