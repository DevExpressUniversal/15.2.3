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
	#region SmartTag
	[GeneratedCode("Suppress FxCop check", "")]
	public interface SmartTag : IWordObject {
		string Name { get; }
		string XML { get; }
		Range Range { get; }
		string DownloadURL { get; }
		CustomProperties Properties { get; }
		void Select();
		void Delete();
		SmartTagActions SmartTagActions { get; }
		XMLNode XMLNode { get; }
	}
	#endregion
	#region SmartTags
	[GeneratedCode("Suppress FxCop check", "")]
	public interface SmartTags : IWordObject, IEnumerable {
		int Count { get; }
		SmartTag this[object Index] { get; } 
		SmartTag Add(string Name, ref object Range, ref object Properties);
		SmartTags SmartTagsByType(string Name);
	}
	#endregion
	#region SmartTagAction
	[GeneratedCode("Suppress FxCop check", "")]
	public interface SmartTagAction : IWordObject {
		string Name { get; }
		void Execute();
		WdSmartTagControlType Type { get; }
		bool PresentInPane { get; }
		bool ExpandHelp { get; set; }
		bool CheckboxState { get; set; }
		string TextboxText { get; set; }
		int ListSelection { get; set; }
		int RadioGroupSelection { get; set; }
		bool ExpandDocumentFragment { get; set; }
		object ActiveXControl { get; }
	}
	#endregion
	#region SmartTagActions
	[GeneratedCode("Suppress FxCop check", "")]
	public interface SmartTagActions : IWordObject, IEnumerable {
		int Count { get; }
		SmartTagAction this[object Index] { get; } 
		void ReloadActions();
	}
	#endregion
	#region CustomProperty
	[GeneratedCode("Suppress FxCop check", "")]
	public interface CustomProperty : IWordObject {
		string Name { get; }
		string Value { get; set; }
		void Delete();
	}
	#endregion
	#region CustomProperties
	[GeneratedCode("Suppress FxCop check", "")]
	public interface CustomProperties : IWordObject, IEnumerable {
		int Count { get; }
		CustomProperty this[object Index] { get; } 
		CustomProperty Add(string Name, string Value);
	}
	#endregion
	#region WdSmartTagControlType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSmartTagControlType {
		wdControlActiveX = 13,
		wdControlButton = 6,
		wdControlCheckbox = 9,
		wdControlCombo = 12,
		wdControlDocumentFragment = 14,
		wdControlDocumentFragmentURL = 15,
		wdControlHelp = 3,
		wdControlHelpURL = 4,
		wdControlImage = 8,
		wdControlLabel = 7,
		wdControlLink = 2,
		wdControlListbox = 11,
		wdControlRadioGroup = 0x10,
		wdControlSeparator = 5,
		wdControlSmartTag = 1,
		wdControlTextbox = 10
	}
	#endregion
}
