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
		static List<Module> Create_DXSpellChecker_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "HunspellDictionaries",
					displayName: @"Hunspell Dictionaries",
					group: "Dictionaries",
					type: "SpellCheckerDemo.HunspellDictionaries",
					shortDescription: @"Evaluate our implementation of Hunspell spell checking engine.",
					description: @"
                        <Paragraph>
                        Hunspell spellchecking engine is designed for languages with rich morphology and complex word compounding or character encoding. Its advantages include morphological analysis, Unicode support and better performance.
                        </Paragraph>
                        <Paragraph>
                        This demo shows how Hunspell dictionaries can be used to check spelling of text containing words in three different languages. Our implementation makes use of third-party Hunspell dictionaries available online.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Spell Check Algorithms",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8944"),
						new WpfModuleLink(
							title: "Dictionaries",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8945"),
						new WpfModuleLink(
							title: "HunspellDictionary",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#CoreLibraries/clsDevExpressXtraSpellCheckerHunspellDictionarytopic")
					}
				),
				new WpfModule(demo,
					name: "CheckAsYouType",
					displayName: @"Check As You Type",
					group: "Spelling Mode",
					type: "SpellCheckerDemo.CheckAsYouType",
					shortDescription: @"This demo illustrates use of the Check As You Type option across target languages.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of the Check As You Type option. Spelling within a document is checked immediately after a word is written. When the spell checker locates a spelling error, the misspelled word is marked with a wavy line.
                        </Paragraph>
                        <Paragraph>
                        Use the Options panel to specify line style, color and other common settings. Note that each editor in the container can include its own settings. Click Reset to Container Settings to clear individual editor settings.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.V142,	
					links: new[] {
						new WpfModuleLink(
							title: "Spell Check Algorithms",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8944"),
						new WpfModuleLink(
							title: "Dictionaries",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8945"),
						new WpfModuleLink(
							title: "SpellChecker.SpellCheckMode Property",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/DevExpressXpfSpellCheckerSpellChecker_SpellCheckModetopic")
					}
				),
				new WpfModule(demo,
					name: "DataGrid",
					displayName: @"DevExpress GridControl",
					group: "Supported Controls",
					type: "SpellCheckerDemo.DataGrid",
					shortDescription: @"Check cell editors in the DXGrid control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to force the DXSpellChecker to check the text contained within the cells of the DXGrid. Modify the text in the Notes column and click the Check Spelling button to see spell checker in action.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RichTextBox",
					displayName: @"Standard RichTextBox",
					group: "Supported Controls",
					type: "SpellCheckerDemo.RichTextBox",
					shortDescription: @"Check the content of a WPF RichTextBox control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to force the DXSpellChecker component to check the text currently being edited in the System.Windows.Controls.RichTextBox control. In this demo you're able to enter any text into the RichTextBox to see how the spell checker works.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TextBox",
					displayName: @"Standard TextBox",
					group: "Supported Controls",
					type: "SpellCheckerDemo.TextBox",
					shortDescription: @"Check the content of a standard TextBox control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to force the DXSpellChecker to check the text currently edited in the System.Windows.Controls.TextBox control. In this demo you're able to enter any text into the TextBox above. Note that although you may check the suggested text, it would be better to enter some misspelled words into the text box, to see how the spell checker works.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Spell Check Algorithms",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8944"),
						new WpfModuleLink(
							title: "Dictionaries",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8945")
					}
				),
				new WpfModule(demo,
					name: "TextEdit",
					displayName: @"DevExpress TextEdit",
					group: "Supported Controls",
					type: "SpellCheckerDemo.TextEdit",
					shortDescription: @"Check the content of the TextEdit control that ships with the DXEditors Library.",
					description: @"
                        <Paragraph>
                        This demo shows that the DXSpellChecker can be used to check text within the TextEdit control - a text editor that ships with the DXEditors Library.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Spell Check Algorithms",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8944"),
						new WpfModuleLink(
							title: "Dictionaries",
							type: WpfModuleLinkType.Documentation,
							url: "http://help.devexpress.com/#WPF/CustomDocument8945")
					}
				),
				new WpfModule(demo,
					name: "DXRichEdit",
					displayName: @"DevExpress RichEditControl",
					group: "Supported Controls",
					type: "SpellCheckerDemo.DXRichEdit",
					shortDescription: @"Check the DXRichEdit control using built-in support for DXSpellChecker.",
					description: @"
                        <Paragraph>
                        This demo shows that the DXRichEdit control can use DXSpellChecker to check its text content. The check-as-you-type functionality is available due to DXSpellChecker support built into the DXRichEdit control.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ContainerWithControls",
					displayName: @"Container Control",
					group: "Supported Controls",
					type: "SpellCheckerDemo.ContainerWithControls",
					shortDescription: @"Check multiple controls in  a container.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to force the DXSpellChecker to check the text within several controls in a container. For this, the SpellChecker.CheckContainer method is called, and each control is passed through the spell check.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
