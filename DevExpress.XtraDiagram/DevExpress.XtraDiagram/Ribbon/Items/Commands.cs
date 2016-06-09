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
using System.Drawing;
using DevExpress.Diagram.Core;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Bars {
	#region Document
	public class DiagramOpenDocumentCommand : DiagramCommand {
		public DiagramOpenDocumentCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.LoadDocument();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Open; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_Open; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_Open_Description; } }
	}
	public class DiagramSaveDocumentCommand : DiagramCommand {
		public DiagramSaveDocumentCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.Commands().Execute(DiagramCommandsBase.SaveDocumentCommand);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Save; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_Save; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_Save_Description; } }
	}
	public class DiagramSaveDocumentAsCommand : DiagramCommand {
		public DiagramSaveDocumentAsCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.Commands().Execute(DiagramCommandsBase.SaveDocumentAsCommand);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.SaveAs; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_SaveAs; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_SaveAs_Description; } }
	}
	public class DiagramUndoCommand : DiagramCommand {
		public DiagramUndoCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if(state != null) {
				state.Enabled = Diagram.CanUndo;
			}
			base.UpdateUIStateCore(state);
		}
		protected override void ExecuteInternal() {
			Diagram.Undo();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Undo; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Menu_Undo_Button; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Menu_Undo_Button_Description; } }
	}
	public class DiagramRedoCommand : DiagramCommand {
		public DiagramRedoCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if(state != null) {
				state.Enabled = Diagram.CanRedo;
			}
			base.UpdateUIStateCore(state);
		}
		protected override void ExecuteInternal() {
			Diagram.Redo();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Redo; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Menu_Redo_Button; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Menu_Redo_Button_Description; } }
	}
	#endregion
	#region Clipboard
	public class DiagramPasteCommand : DiagramCommand {
		public DiagramPasteCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.Paste();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Paste; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_Paste; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_Paste_Description; } }
	}
	public class DiagramCutCommand : DiagramCommand {
		public DiagramCutCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.Cut();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Cut; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_Cut; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_Cut_Description; } }
	}
	public class DiagramCopyCommand : DiagramCommand {
		public DiagramCopyCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			Diagram.Copy();
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Paste; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.DiagramCommand_Copy; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.DiagramCommand_Copy_Description; } }
	}
	#endregion
	#region Font
	public class DiagramFontCommand : DiagramCommand {
		public DiagramFontCommand(DiagramControl control)
			: base(control){
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<string> itemState = state as IValueBasedCommandUIState<string>;
			if(itemState != null) {
				DiagramItem item = Diagram.PrimarySelection();
				if(item != null) {
					itemState.Value = item.Appearance.Font.Name;
				}
				else {
					itemState.Value = AppearanceObject.DefaultFont.Name;
				}
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			IValueBasedCommandUIState<string> itemState = state as IValueBasedCommandUIState<string>;
			if(Diagram.ContainsSelection() && itemState != null) {
				Diagram.ForEachSelectedItem(item => item.Appearance.Update(fontFamily: itemState.Value));
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.Font; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_Font; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_Font_Description; } }
	}
	public class DiagramFontSizeCommand : DiagramCommand {
		public DiagramFontSizeCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<int?> itemState = state as IValueBasedCommandUIState<int?>;
			if(itemState != null) {
				DiagramItem item = Diagram.PrimarySelection();
				if(item != null) {
					itemState.Value = (int)(item.Appearance.Font.Size * 2);
				}
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			IValueBasedCommandUIState<int?> itemState = state as IValueBasedCommandUIState<int?>;
			if(Diagram.ContainsSelection() && itemState != null) {
				Diagram.ForEachSelectedItem(item => item.Appearance.Update(fontSize: itemState.Value / 2.0));
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontSize; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontSize; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontSize_Description; } }
	}
	public class DiagramFontSizeIncreaseCommand : DiagramCommand {
		public DiagramFontSizeIncreaseCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			if(Diagram.ContainsSelection()) {
				Diagram.ForEachSelectedItem(item => item.Appearance.Update(fontSize: IncreaseFontSize(item.Appearance.Font.Size)));
			}
		}
		protected int IncreaseFontSize(float fontSize) {
			return FontSizeCollection.Instance.CalculateNextFontSize((int)fontSize);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontSizeIncrease; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontSizeIncrease; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontSizeIncrease_Description; } }
	}
	public class DiagramFontSizeDecreaseCommand : DiagramCommand {
		public DiagramFontSizeDecreaseCommand(DiagramControl control)
			: base(control) {
		}
		protected override void ExecuteInternal() {
			if(Diagram.ContainsSelection()) {
				Diagram.ForEachSelectedItem(item => item.Appearance.Update(fontSize: DecreaseFontSize(item.Appearance.Font.Size)));
			}
		}
		protected int DecreaseFontSize(float fontSize) {
			return FontSizeCollection.Instance.CalculatePreviousFontSize((int)fontSize);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontSizeDecrease; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontSizeDecrease; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontSizeDecrease_Description; } }
	}
	public abstract class DiagramAppearanceCommandBase : DiagramCommand {
		protected DiagramAppearanceCommandBase(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if(state != null) {
				DiagramItem item = Diagram.PrimarySelection();
				if(item != null) {
					state.Checked = CalcIsChecked(item);
				}
				else {
					state.Checked = false;
				}
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			if(Diagram.ContainsSelection() && state != null) {
				Diagram.ForEachSelectedItem(item => UpdateItemAppearance(item, state));
			}
			base.ForceExecute(state);
		}
		protected abstract bool CalcIsChecked(DiagramItem item );
		protected abstract void UpdateItemAppearance(DiagramItem item, ICommandUIState state);
	}
	public abstract class DiagramFontStyleCommandBase : DiagramAppearanceCommandBase {
		protected DiagramFontStyleCommandBase(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.Font.Style.HasFlag(FontStyle);
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			item.Appearance.Update(fontStyle: GetFontStyle(state, item));
		}
		protected FontStyle? GetFontStyle(ICommandUIState state, DiagramItem item) {
			FontStyle style = item.Appearance.Font.Style;
			if(state.Checked) {
				return (style & ~FontStyle);
			}
			return (style | FontStyle);
		}
		protected abstract FontStyle FontStyle { get; }
	}
	public class DiagramFontBoldCommand : DiagramFontStyleCommandBase {
		public DiagramFontBoldCommand(DiagramControl control)
			: base(control) {
		}
		protected override FontStyle FontStyle { get { return FontStyle.Bold; } }
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontBold; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontBold; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontBold_Description; } }
	}
	public class DiagramFontItalicCommand : DiagramFontStyleCommandBase {
		public DiagramFontItalicCommand(DiagramControl control)
			: base(control) {
		}
		protected override FontStyle FontStyle { get { return FontStyle.Italic; } }
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontItalic; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontItalic; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontItalic_Description; } }
	}
	public class DiagramFontUnderlineCommand : DiagramFontStyleCommandBase {
		public DiagramFontUnderlineCommand(DiagramControl control)
			: base(control) {
		}
		protected override FontStyle FontStyle { get { return FontStyle.Underline; } }
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontUnderline; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontUnderline; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontUnderline_Description; } }
	}
	public class DiagramFontStrikethroughCommand : DiagramFontStyleCommandBase {
		public DiagramFontStrikethroughCommand(DiagramControl control)
			: base(control) {
		}
		protected override FontStyle FontStyle { get { return FontStyle.Strikeout; } }
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontStrikethrough; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontStrikethrough; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontStrikethrough_Description; } }
	}
	public abstract class DiagramFontColorCommandBase : DiagramCommand {
		public DiagramFontColorCommandBase(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ColorChangeItemUIState itemState = state as ColorChangeItemUIState;
			if(itemState != null && itemState.Color.IsEmpty) {
				DiagramItem item = Diagram.PrimarySelection();
				if(item != null) itemState.Color = GetColor(item);
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			IValueBasedCommandUIState<Color> itemState = state as IValueBasedCommandUIState<Color>;
			if(Diagram.ContainsSelection() && itemState != null) {
				Diagram.ForEachSelectedItem(item => SetColor(item, itemState.Value));
			}
		}
		protected abstract Color GetColor(DiagramItem item);
		protected abstract void SetColor(DiagramItem item, Color color);
	}
	public class DiagramFontColorCommand : DiagramFontColorCommandBase {
		public DiagramFontColorCommand(DiagramControl control)
			: base(control) {
		}
		protected override Color GetColor(DiagramItem item) {
			return item.Appearance.ForeColor;
		}
		protected override void SetColor(DiagramItem item, Color color) {
			item.Appearance.ForeColor = color;
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontColor; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontColor; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontColor_Description; } }
	}
	public class DiagramFontBackColorCommand : DiagramFontColorCommandBase {
		public DiagramFontBackColorCommand(DiagramControl control)
			: base(control) {
		}
		protected override Color GetColor(DiagramItem item) {
			return item.Appearance.BackColor;
		}
		protected override void SetColor(DiagramItem item, Color color) {
			item.Appearance.BackColor = color;
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.FontBackColor; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Font_FontBackgroundColor; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Font_FontBackgroundColor_Description; } }
	}
	#endregion
	#region Paragraph
	public class DiagramParagraphAlignTopCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignTopCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.VAlignment == VertAlignment.Top;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.VAlignment = VertAlignment.Top;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignTop; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignTop; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignTop_Description; } }
	}
	public class DiagramParagraphAlignMiddleCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignMiddleCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.VAlignment == VertAlignment.Center || item.Appearance.TextOptions.VAlignment == VertAlignment.Default;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.VAlignment = VertAlignment.Default;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignMiddle; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignMiddle; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignMiddle_Description; } }
	}
	public class DiagramParagraphAlignBottomCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignBottomCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.VAlignment == VertAlignment.Bottom;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignBottom; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignBottom; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignBottom_Description; } }
	}
	public class DiagramParagraphAlignLeftCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignLeftCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.HAlignment == HorzAlignment.Near;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignLeft; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignLeft; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignLeft_Description; } }
	}
	public class DiagramParagraphAlignCenterCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignCenterCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.HAlignment == HorzAlignment.Default || item.Appearance.TextOptions.HAlignment == HorzAlignment.Center;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.HAlignment = HorzAlignment.Default;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignCenter; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignCenter; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignCenter_Description; } }
	}
	public class DiagramParagraphAlignRightCommand : DiagramAppearanceCommandBase {
		public DiagramParagraphAlignRightCommand(DiagramControl control)
			: base(control) {
		}
		protected override bool CalcIsChecked(DiagramItem item) {
			return item.Appearance.TextOptions.HAlignment == HorzAlignment.Far;
		}
		protected override void UpdateItemAppearance(DiagramItem item, ICommandUIState state) {
			if(!state.Checked) {
				item.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.AlignRight; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Paragraph_AlignRight; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Paragraph_AlignRight_Description; } }
	}
	#endregion
	#region Tools
	public class DiagramPointerToolCommand : DiagramCommand {
		public DiagramPointerToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = (Diagram.OptionsBehavior.ActiveTool is PointerTool);
		}
		public override void ForceExecute(ICommandUIState state) {
			if(!state.Checked) {
				Diagram.OptionsBehavior.ActiveTool = DiagramController.DefaultTool;
			}
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.PointerTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Tools_Pointer; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Pointer_Description; } }
	}
	public class DiagramConnectorToolCommand : DiagramCommand {
		public DiagramConnectorToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = (Diagram.OptionsBehavior.ActiveTool is ConnectorTool);
		}
		public override void ForceExecute(ICommandUIState state) {
			if(!state.Checked) {
				Diagram.OptionsBehavior.ActiveTool = new ConnectorTool();
			}
			base.ForceExecute(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.ConnectorTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Tools_Connector; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Connector_Description; } }
	}
	public class DiagramShapeToolSelectionCommand : DiagramCommand {
		public DiagramShapeToolSelectionCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			CheckDropDownButtonItemUIState itemState = state as CheckDropDownButtonItemUIState;
			if(itemState != null) {
				ShapeTool tool = Diagram.OptionsBehavior.ActiveTool as ShapeTool;
				if(tool != null && Array.Exists(ShapeTools, shape => tool.Shape == shape)) {
					itemState.Down = true;
				}
				else {
					itemState.Down = false;
				}
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<bool> itemState = state as IValueBasedCommandUIState<bool>;
			if(itemState != null) {
				if(!itemState.Value) Diagram.OptionsBehavior.ActiveTool = DiagramController.DefaultTool;
			}
			base.ForceExecute(state);
		}
		static readonly ShapeDescription[] ShapeTools = new ShapeDescription[] {
			BasicShapes.Rectangle,
			BasicShapes.Ellipse,
			BasicShapes.RightTriangle,
			BasicShapes.Hexagon
		};
		public override DiagramCommandId Id { get { return DiagramCommandId.ShapeTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Tools_Text; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Text_Description; } }
	}
	public abstract class DiagramShapeToolCommandBase : DiagramCommand {
		public DiagramShapeToolCommandBase(DiagramControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Diagram.OptionsBehavior.ActiveTool = new ShapeTool(Shape);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = GetIsChecked();
		}
		protected bool GetIsChecked() {
			if(Diagram == null) return false;
			ShapeTool shapeTool = Diagram.OptionsBehavior.ActiveTool as ShapeTool;
			return shapeTool != null && ReferenceEquals(shapeTool.Shape, Shape);
		}
		protected abstract ShapeDescription Shape { get; }
	}
	public class DiagramRectangleToolCommand : DiagramShapeToolCommandBase {
		public DiagramRectangleToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override ShapeDescription Shape { get { return BasicShapes.Rectangle; } }
		public override DiagramCommandId Id { get { return DiagramCommandId.RectangleTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.BasicShapes_Rectangle_Name; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Rectangle_Description; } }
	}
	public class DiagramEllipseToolCommand : DiagramShapeToolCommandBase {
		public DiagramEllipseToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override ShapeDescription Shape { get { return BasicShapes.Ellipse; } }
		public override DiagramCommandId Id { get { return DiagramCommandId.EllipseTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.BasicShapes_Ellipse_Name; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Ellipse_Description; } }
	}
	public class DiagramRightTriangleToolCommand : DiagramShapeToolCommandBase {
		public DiagramRightTriangleToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override ShapeDescription Shape { get { return BasicShapes.RightTriangle; } }
		public override DiagramCommandId Id { get { return DiagramCommandId.RightTriangleTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.BasicShapes_RightTriangle_Name; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_RightTriangle_Description; } }
	}
	public class DiagramHexagonToolCommand : DiagramShapeToolCommandBase {
		public DiagramHexagonToolCommand(DiagramControl control)
			: base(control) {
		}
		protected override ShapeDescription Shape { get { return BasicShapes.Hexagon; } }
		public override DiagramCommandId Id { get { return DiagramCommandId.HexagonTool; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.BasicShapes_Hexagon_Name; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Tools_Hexagon_Description; } }
	}
	#endregion
	#region Arrange
	public class DiagramBringToFrontCommand : DiagramCommand {
		public DiagramBringToFrontCommand(DiagramControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Diagram.Commands().Execute(DiagramCommandsBase.BringToFrontCommand);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Diagram.Commands().CanExecute(DiagramCommandsBase.BringToFrontCommand, null);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.BringToFront; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Layout_BringToFront; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Layout_BringToFront; } }
	}
	public class DiagramSendToBackCommand : DiagramCommand {
		public DiagramSendToBackCommand(DiagramControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Diagram.Commands().Execute(DiagramCommandsBase.SendToBackCommand);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Diagram.Commands().CanExecute(DiagramCommandsBase.SendToBackCommand, null);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.SendToBack; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Layout_SendToBack; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Layout_SendToBack; } }
	}
	public class DiagramBringForwardCommand : DiagramCommand {
		public DiagramBringForwardCommand(DiagramControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Diagram.Commands().Execute(DiagramCommandsBase.BringForwardCommand);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.BringForward; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Layout_BringForward; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Layout_BringForward; } }
	}
	public class DiagramSendBackwardCommand : DiagramCommand {
		public DiagramSendBackwardCommand(DiagramControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Diagram.Commands().Execute(DiagramCommandsBase.SendBackwardCommand);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.SendBackward; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Layout_SendBackward; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Layout_SendBackward; } }
	}
	#endregion
	#region Show
	public class DiagramShowRulerCommand : DiagramCommand {
		public DiagramShowRulerCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if(state != null) {
				state.Checked = Diagram.OptionsView.DrawRulers;
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			if(state != null) {
				Diagram.OptionsView.DrawRulers = !state.Checked;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.ShowRuler; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Show_Ruler; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Show_Ruler_Description; } }
	}
	public class DiagramShowGridCommand : DiagramCommand {
		public DiagramShowGridCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if(state != null) {
				state.Checked = Diagram.OptionsView.DrawGrid;
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			if(state != null) {
				Diagram.OptionsView.DrawGrid = !state.Checked;
			}
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.ShowGrid; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.Show_Grid; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.Show_Grid_Description; } }
	}
	#endregion
	#region Appearance
	public class DiagramSkinGalleryCommand : DiagramCommand {
		public DiagramSkinGalleryCommand(DiagramControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandsRestriction(state, Diagram.OptionsBehavior.SkinGallery);
		}
		public override DiagramCommandId Id { get { return DiagramCommandId.SkinGallery; } }
		public override DiagramControlStringId MenuCaptionStringId { get { return DiagramControlStringId.String_None; } }
		public override DiagramControlStringId DescriptionStringId { get { return DiagramControlStringId.String_None; } }
	}
	#endregion
}
