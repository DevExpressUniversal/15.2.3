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

using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public sealed class StatusMessagesHelper {
		private BarLinksHolder barLinksHolder;
		private ICollection<BarItem> statusMessageItems;
		public StatusMessagesHelper(BarLinksHolder barLinksHolder) {
			Guard.ArgumentNotNull(barLinksHolder, "barLinksHolder");
			this.barLinksHolder = barLinksHolder;
			statusMessageItems = new List<BarItem>();
		}
		public void SetMessages(IEnumerable<string> messages) {
			foreach(BarItem barItem in statusMessageItems) {
				barItem.Dispose();
			}
			statusMessageItems.Clear();
			int i = 0;
			foreach(string message in messages) {
				BarStaticItem item = new BarStaticItem();
				item.Name = "StatusBarItem" + i++;
				item.Caption = message;
				item.Visibility = BarItemVisibility.OnlyInRuntime;
				item.AutoSize = BarStaticItemSize.Content;
				item.MergeType = BarMenuMerge.Replace;
				barLinksHolder.ItemLinks.Add(item);
				statusMessageItems.Add(item);
			}
		}
	}
}
