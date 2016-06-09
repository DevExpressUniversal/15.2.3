#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public class PdfInteractiveFormFieldTextState {
		public const double DefaultFontSize = 12;
		readonly PdfSetTextFontCommand fontCommand;
		readonly IList<PdfCommand> commandsToFill;
		readonly double characterSpacing = 0;
		readonly double wordSpacing = 0;
		readonly double horizontalScaling = 100;
		public PdfSetTextFontCommand FontCommand { get { return fontCommand; } }
		public double CharacterSpacing { get { return characterSpacing; } }
		public double WordSpacing { get { return wordSpacing; } }
		public double HorizontalScaling { get { return horizontalScaling; } }
		public double FontSize { get { return fontCommand == null ? DefaultFontSize : fontCommand.FontSize; } }
		public PdfInteractiveFormFieldTextState(PdfInteractiveFormField formField) {
			commandsToFill = new List<PdfCommand>();
			IEnumerable<PdfCommand> commands = GetAppearanceCommandsInheritable(formField);
			if (commands != null)
				foreach (PdfCommand appearanceCommand in commands) {
					PdfSetWordSpacingCommand wordSpacingCommand = appearanceCommand as PdfSetWordSpacingCommand;
					if (wordSpacingCommand != null)
						wordSpacing = wordSpacingCommand.WordSpacing;
					PdfSetCharacterSpacingCommand characterSpacingCommand = appearanceCommand as PdfSetCharacterSpacingCommand;
					if (characterSpacingCommand != null)
						this.characterSpacing = characterSpacingCommand.CharacterSpacing;
					PdfSetTextHorizontalScalingCommand horizontalScalingCommand = appearanceCommand as PdfSetTextHorizontalScalingCommand;
					if (horizontalScalingCommand != null)
						horizontalScaling = horizontalScalingCommand.HorizontalScaling;
					PdfSetTextFontCommand fontCommand = appearanceCommand as PdfSetTextFontCommand;
					if (fontCommand != null)
						this.fontCommand = fontCommand;
					else
						commandsToFill.Add(appearanceCommand);
				}
			if (this.fontCommand == null || this.fontCommand.Font == null) {
				PdfAnnotationAppearances appearance = formField.Widget == null ? null : formField.Widget.Appearance;
				IEnumerable<PdfCommand> widgetCommands = appearance == null || appearance.Normal == null || appearance.Normal.DefaultForm == null ? null : appearance.Normal.DefaultForm.Commands;
				if (widgetCommands != null)
					this.fontCommand = FindSetTextFontCommand(widgetCommands);
			}
		}
		public void FillCommands(IList<PdfCommand> commands) {
			foreach (PdfCommand appearanceCommand in commandsToFill)
				commands.Add(appearanceCommand);
		}
		protected IEnumerable<PdfCommand> GetAppearanceCommandsInheritable(PdfInteractiveFormField formField) {
			if (formField == null)
				return null;
			IEnumerable<PdfCommand> appearanceCommands = formField.AppearanceCommands;
			if (appearanceCommands != null)
				return appearanceCommands;
			PdfInteractiveFormField parentField = formField.Parent;
			return parentField == null && formField.Form != null ? formField.Form.DefaultAppearanceCommands : GetAppearanceCommandsInheritable(parentField);
		}
		PdfSetTextFontCommand FindSetTextFontCommand(IEnumerable<PdfCommand> commands) {
			foreach (PdfCommand command in commands) {
				PdfSetTextFontCommand fontCommand = command as PdfSetTextFontCommand;
				if (fontCommand != null)
					return fontCommand;
				PdfMarkedContentCommand markedContent = command as PdfMarkedContentCommand;
				if (markedContent != null) {
					PdfSetTextFontCommand markedFontCommand = FindSetTextFontCommand(markedContent.Children);
					if (markedFontCommand != null)
						return markedFontCommand;
				}
			}
			return null;
		}
	}
}
