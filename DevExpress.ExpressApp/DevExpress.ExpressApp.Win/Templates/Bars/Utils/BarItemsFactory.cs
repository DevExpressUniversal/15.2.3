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
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.Bars.Utils {
	public class BarItemsFactory {
		public static int DefaultControlWidth = 150;
		private BarManager barManager;
		public BarItemsFactory(BarManager barManager) {
			this.barManager = barManager;
		}
		public BarButtonItem CreateButtonItem() {
			BarButtonItem barItem = new BarButtonItem();
			barItem.Manager = barManager;
			return barItem;
		}
		public BarEditItem CreateEditItem(Type valueType, bool canClear) {
			BarEditItem editItem = new BarEditItem();
			editItem.Manager = barManager;
			editItem.Edit = ParametrizedActionItemControlFactory.CreateRepositoryItem(valueType, canClear);
			editItem.Width = DefaultControlWidth;
			return editItem;
		}
		public BarEditItem CreateImageComboBoxEditItem() {
			BarEditItem editItem = new BarEditItem();
			editItem.Manager = barManager;
			editItem.Edit = new RepositoryItemImageComboBox();
			editItem.Width = DefaultControlWidth;
			return editItem;
		}
	}
}
