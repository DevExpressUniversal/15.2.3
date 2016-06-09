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
using System.CodeDom.Compiler;
namespace DevExpress.XtraRichEdit.API.Word {
	#region WdContinue
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdContinue {
		wdContinueDisabled,
		wdResetList,
		wdContinueList
	}
	#endregion
	#region WdListType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdListType {
		wdListNoNumbering,
		wdListListNumOnly,
		wdListBullet,
		wdListSimpleNumbering,
		wdListOutlineNumbering,
		wdListMixedNumbering,
		wdListPictureBullet
	}
	#endregion
	#region ListFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ListFormat : IWordObject {
		int ListLevelNumber { get; set; }
		List List { get; }
		ListTemplate ListTemplate { get; }
		int ListValue { get; }
		bool SingleList { get; }
		bool SingleListTemplate { get; }
		WdListType ListType { get; }
		string ListString { get; }
		WdContinue CanContinuePreviousList(ListTemplate ListTemplate);
		void RemoveNumbers(ref object NumberType);
		void ConvertNumbersToText(ref object NumberType);
		int CountNumberedItems(ref object NumberType, ref object Level);
		void ApplyBulletDefaultOld();
		void ApplyNumberDefaultOld();
		void ApplyOutlineNumberDefaultOld();
		void ApplyListTemplateOld(ListTemplate ListTemplate, ref object ContinuePreviousList, ref object ApplyTo);
		void ListOutdent();
		void ListIndent();
		void ApplyBulletDefault(ref object DefaultListBehavior);
		void ApplyNumberDefault(ref object DefaultListBehavior);
		void ApplyOutlineNumberDefault(ref object DefaultListBehavior);
		void ApplyListTemplate(ListTemplate ListTemplate, ref object ContinuePreviousList, ref object ApplyTo, ref object DefaultListBehavior);
		InlineShape ListPictureBullet { get; }
		void ApplyListTemplateWithLevel(ListTemplate ListTemplate, ref object ContinuePreviousList, ref object ApplyTo, ref object DefaultListBehavior, ref object ApplyLevel);
	}
	#endregion
}
