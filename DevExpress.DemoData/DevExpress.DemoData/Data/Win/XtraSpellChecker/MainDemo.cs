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
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_XtraSpellChecker_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"XtraSpellChecker Features",
					group: "About",
					type: "DevExpress.XtraSpellChecker.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "CheckRichEditControl",
					displayName: @"RichEditControl",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CheckRichEditControl",
					description: @"This demo illustrates the integration of spell checker into the RichEditControl.
In this demo, click Spelling to reveal spelling mistakes in the document. Additional options include the language selection, as well as the capability to check spelling on the fly, as long as you type.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CheckRichEditControl.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CheckRichEditControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "CheckTextBox",
					displayName: @"TextBox",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CheckTextBox",
					description: @"This demo illustrates how to force the SpellChecker component to check the text currently being edited in the standard Windows Forms TextBox control. For this, you should simply call the SpellChecker.Check method and pass the TextBox as a parameter. In this demo you're able to enter any text into the TextBox below. Note that although you may check the suggested text, it would be better to enter some misspelled words into the text box, to see how the SpellChecker works. Then press F7 or click the ""Check Spelling..."" button, and the SpellChecker starts checking the text.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CheckTextBox.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CheckTextBox.vb"
					}
				),
				new SimpleModule(demo,
					name: "CheckRichTextBox",
					displayName: @"RichTextBox",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CheckRichTextBox",
					description: @"This demo illustrates how to force the SpellChecker component to check the text currently being edited in the standard Windows Forms RichTextBox control. For this, you should simply call the SpellChecker.Check method and pass the RichTextBox as a parameter. In this demo you're able to enter any text into the RichTextBox below. Note that although you may check the suggested text, it would be better to enter some misspelled words into the text box, to see how the SpellChecker works. Then press F7 or click the ""Check Spelling..."" button, and the SpellChecker starts checking the text.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CheckRichTextBox.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CheckRichTextBox.vb"
					}
				),
				new SimpleModule(demo,
					name: "CheckMemoEdit",
					displayName: @"MemoEdit",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CheckMemoEdit",
					description: @"This demo illustrates how to force the SpellChecker component to check the text currently being edited in the DevExpress XtraEditors MemoEdit control. For this, you should simply call the SpellChecker.Check method and pass the MemoEdit as a parameter. In this demo you're able to enter any text into the MemoEdit below. Note that although you may check the suggested text,it would be better to enter some misspelled words into the memo edit, to see how the SpellChecker works. Then press F7 or click the ""Check Spelling..."" button, and the SpellChecker starts checking the text.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CheckMemoEdit.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CheckMemoEdit.vb"
					}
				),
				new SimpleModule(demo,
					name: "CheckContainer",
					displayName: @"XtraGrid",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CheckContainer",
					description: @"This demo illustrates how to force the SpellChecker component to check the text displayed in GridControl cells (shipped with the DevExpress XtraGrid suite). For this, you should simply call the SpellChecker.Check method and pass the grid's cell as a parameter. In this demo you may, for instance, change the text displayed in the grid's Notes column (to enter some misspelled words into it) and then press F7, and the SpellChecker starts checking the text into this cell.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CheckContainer.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CheckContainer.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultipleEditors",
					displayName: @"Multiple Editors",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.MultipleEditors",
					description: @"This demo illustrates how to force the SpellChecker component to check the text within several controls on a control container. For this, the SpellChecker.CheckContainer method is called, and each control is passed through the spell check according to its tab order. In this demo you're able to enter any text into the controls displayed below. Note that it would be better to enter some misspelled words, to see how the SpellChecker works. Then press F7 or click the ""Check Spelling..."" button and the SpellChecker starts checking the text within all the controls onto the form.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\MultipleEditors.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\MultipleEditors.vb"
					}
				),
				new SimpleModule(demo,
					name: "DifferentSettings",
					displayName: @"Different Settings",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.DifferentSettings",
					description: @"This demo illustrates how to force the SpellChecker component to check the text within several controls on a control container. For this, the SpellChecker.CheckContainer method is called, and each control is passed through the spell check according to its tab order. In this demo you're able to enter any text into the controls displayed below. Note that it would be better to enter some misspelled words, to see how the SpellChecker works. Then press F7 or click the ""Check Spelling..."" button and the SpellChecker starts checking the text within all the controls onto the form.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\DifferentSettings.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\DifferentSettings.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomDictionary",
					displayName: @"Custom Dictionary",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.CustomDictionary",
					description: @"This demo illustrates how the SpellChecker uses the words provided from the custom dictionary. The SpellChecker internally merges standard and custom dictionaries before starting the check, so the words from both types of dictionaries are considered valid. In this demo it's recommended that you add several words definitely missed in common vocabularies to the custom dictionary (the text box on the left), and check the text containing those words in the right text box by pressing F7 or clicking the ""Check Spelling..."" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\CustomDictionary.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\CustomDictionary.vb"
					}
				),
				new SimpleModule(demo,
					name: "HunspellDictionaries",
					displayName: @"Hunspell Dictionaries",
					group: "Spelling",
					type: "DevExpress.XtraSpellChecker.Demos.HunspellDictionaries",
					description: @"Hunspell spellchecking engine is designed for languages with rich morphology and complex word compounding or character encoding. Its advantages include morphological analysis, Unicode support and better performance. This demo shows how Hunspell dictionaries can be used to check spelling of text containing words in three different languages. Our implementation makes use of third-party Hunspell dictionaries available online.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpellCheckerMainDemo\Modules\HunspellDictionaries.cs",
						@"\WinForms\VB\SpellCheckerMainDemo\Modules\HunspellDictionaries.vb"
					}
				)
			};
		}
	}
}
