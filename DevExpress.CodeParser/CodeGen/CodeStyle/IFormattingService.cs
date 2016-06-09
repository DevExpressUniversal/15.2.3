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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public interface IFormattingService
  {
	FormattingRuleCollection GetRules(ParserLanguageID languageId);
	void SetDefaults(ParserLanguageID languageId);
	bool Enabled(ParserLanguageID languageId, string optionName);
	int GetIntValue(ParserLanguageID languageId, string optionName);
	string GetStringValue(ParserLanguageID languageId, string optionName);
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Save(ParserLanguageID languageId);
	FormattingRuleCollection this[ParserLanguageID languageId] { get; }
	bool Format(SourceFile file, SourceRange range);
		[EditorBrowsable(EditorBrowsableState.Never)]
		void FormatRange(SourceFile file, SourceRange range);
	IRuleChanger GetRuleChanger(ParserLanguageID language);
  }
  public interface IRuleChanger : IDisposable
  {
	bool AddRuleValue(string ruleName, object newValue);
  }
}
