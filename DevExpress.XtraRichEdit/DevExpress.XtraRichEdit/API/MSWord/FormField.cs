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
	#region FormField
	[GeneratedCode("Suppress FxCop check", "")]
	public interface FormField : IWordObject {
		WdFieldType Type { get; }
		string Name { get; set; }
		string EntryMacro { get; set; }
		string ExitMacro { get; set; }
		bool OwnHelp { get; set; }
		bool OwnStatus { get; set; }
		string HelpText { get; set; }
		string StatusText { get; set; }
		bool Enabled { get; set; }
		string Result { get; set; }
		TextInput TextInput { get; }
		CheckBox CheckBox { get; }
		DropDown DropDown { get; }
		FormField Next { get; }
		FormField Previous { get; }
		bool CalculateOnExit { get; set; }
		Range Range { get; }
		void Select();
		void Copy();
		void Cut();
		void Delete();
	}
	#endregion
	#region FormFields
	[GeneratedCode("Suppress FxCop check", "")]
	public interface FormFields : IWordObject, IEnumerable {
		int Count { get; }
		bool Shaded { get; set; }
		FormField this[object Index] { get; } 
		FormField Add(Range Range, WdFieldType Type);
	}
	#endregion
}
