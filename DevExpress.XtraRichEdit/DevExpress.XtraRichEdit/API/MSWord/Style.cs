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
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Style
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Style : IWordObject {
		string NameLocal { get; set; }
		object BaseStyle { get; set; }
		string Description { get; }
		WdStyleType Type { get; }
		bool BuiltIn { get; }
		object NextParagraphStyle { get; set; }
		bool InUse { get; }
		Shading Shading { get; }
		Borders Borders { get; set; }
		ParagraphFormat ParagraphFormat { get; set; }
		Font Font { get; set; }
		Frame Frame { get; }
		WdLanguageID LanguageID { get; set; }
		bool AutomaticallyUpdate { get; set; }
		ListTemplate ListTemplate { get; }
		int ListLevelNumber { get; }
		WdLanguageID LanguageIDFarEast { get; set; }
		bool Hidden { get; set; }
		void Delete();
		void LinkToListTemplate(ListTemplate ListTemplate, ref object ListLevelNumber);
		int NoProofing { get; set; }
		object LinkStyle { get; set; }
		bool Visibility { get; set; }
		bool NoSpaceBetweenParagraphsOfSameStyle { get; set; }
		TableStyle Table { get; }
		bool Locked { get; set; }
		int Priority { get; set; }
		bool UnhideWhenUsed { get; set; }
		bool QuickStyle { get; set; }
		bool Linked { get; }
	}
	#endregion
	#region Styles
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Styles : IWordObject, IEnumerable {
		int Count { get; }
		Style this[object Index] { get; } 
		Style Add(string Name, ref object Type);
	}
	#endregion
	#region WdStyleType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdStyleType {
		wdStyleTypeCharacter = 2,
		wdStyleTypeLinked = 6,
		wdStyleTypeList = 4,
		wdStyleTypeParagraph = 1,
		wdStyleTypeParagraphOnly = 5,
		wdStyleTypeTable = 3
	}
	#endregion
}
