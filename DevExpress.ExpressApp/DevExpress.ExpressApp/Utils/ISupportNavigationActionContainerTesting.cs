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
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Utils {
	public interface ISupportNavigationActionContainerTesting : IActionContainer {
		bool IsItemControlVisible(ChoiceActionItem item);
		int GetGroupCount();
		string GetGroupControlCaption(ChoiceActionItem groupItem);
		int GetGroupChildControlCount(ChoiceActionItem groupItem);
		string GetChildControlCaption(ChoiceActionItem item);
		bool GetChildControlEnabled(ChoiceActionItem item);
		bool GetChildControlVisible(ChoiceActionItem item);
		bool IsGroupExpanded(ChoiceActionItem item);
		string GetSelectedItemCaption();
		INavigationControlTestable NavigationControl { get;}
	}
	public interface INavigationControlTestable : INavigationControl {
		bool IsItemEnabled(DevExpress.ExpressApp.Actions.ChoiceActionItem item);
		bool IsItemVisible(ChoiceActionItem item);
		bool IsGroupExpanded(ChoiceActionItem item);
		int GetSubItemsCount(ChoiceActionItem item);
		string GetItemCaption(ChoiceActionItem item);
		int GetGroupCount();
		int GetSubGroupCount(ChoiceActionItem item);
		string GetSelectedItemCaption();
		string GetItemToolTip(ChoiceActionItem item);
	}
}
