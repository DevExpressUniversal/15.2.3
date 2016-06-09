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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.ComponentModel;
using DevExpress.XtraBars.Commands.Internal;
using System.Drawing;
namespace DevExpress.XtraDiagram.Bars {
	#region Document
	public class DiagramOpenDocumentBarItem : DiagramCommandBarButtonItem {
		public DiagramOpenDocumentBarItem() {
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("open;Colored;Size32x32");
		}
		protected override bool AddToApplicationMenu { get { return true; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Open; } }
	}
	public class DiagramSaveDocumentBarItem : DiagramCommandBarButtonItem {
		public DiagramSaveDocumentBarItem() {
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("save;Colored;Size32x32");
		}
		protected override bool AddToApplicationMenu { get { return true; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Save; } }
	}
	public class DiagramSaveDocumentAsBarItem : DiagramCommandBarButtonItem {
		public DiagramSaveDocumentAsBarItem() {
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("SaveAs;Colored;Size32x32");
		}
		protected override bool AddToApplicationMenu { get { return true; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.SaveAs; } }
	}
	public class DiagramUndoBarItem : DiagramCommandBarButtonItem {
		public DiagramUndoBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("Undo");
		}
		protected override Image LoadDefaultLargeImage() {
			return ImageUtils.LoadLargeImage("Undo");
		}
		protected override bool AddToQuickAccessToolbar { get { return true; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Undo; } }
	}
	public class DiagramRedoBarItem : DiagramCommandBarButtonItem {
		public DiagramRedoBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("Redo");
		}
		protected override Image LoadDefaultLargeImage() {
			return ImageUtils.LoadLargeImage("Redo");
		}
		protected override bool AddToQuickAccessToolbar { get { return true; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Redo; } }
	}
	#endregion
	#region Appearance
	public class DiagramSkinGalleryBarItem : DiagramCommandSkinGalleryBarItem {
		protected override void OnControlChanged() {
			base.OnControlChanged();
			if(Diagram != null && !Diagram.IsDesignMode) SkinHelper.InitSkinGallery(this);
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.SkinGallery; } }
	}
	#endregion
	#region Clipboard
	public class DiagramPasteBarItem : DiagramCommandBarButtonItem {
		public DiagramPasteBarItem() {
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("paste;Colored;Size32x32");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Paste; } }
	}
	public class DiagramCutBarItem : DiagramCommandBarButtonItem {
		public DiagramCutBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("cut;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Cut; } }
	}
	public class DiagramCopyBarItem : DiagramCommandBarButtonItem {
		public DiagramCopyBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("copy;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Copy; } }
	}
	#endregion
	#region Font
	public class DiagramFontNameEditItem : DiagramCommandBarEditItem<string>, IBarButtonGroupMember {
		const int defaultWidth = 130;
		public DiagramFontNameEditItem() {
			Width = defaultWidth;
		}
		protected override RepositoryItem CreateEdit() {
			return new RepositoryItemFontEdit();
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		object IBarButtonGroupMember.ButtonGroupTag { get { return GroupId.FontGroupId; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.Font; } }
	}
	public class DiagramFontSizeBarItem : DiagramCommandBarEditItem<int?>, IBarButtonGroupMember {
		public DiagramFontSizeBarItem() {
		}
		protected override ICommandUIState CreateCommandUIState(Command command) {
			int editValue = 0;
			DefaultValueBasedCommandUIState<int?> value = new DefaultValueBasedCommandUIState<int?>();
			if(EditValue != null) {
				FontSizeEditUtils.TryGetHalfSizeValue(EditValue.ToString(), out editValue);
			}
			value.Value = editValue;
			return value;
		}
		protected override BarEditItemUIState<int?> CreateBarEditItemUIState() {
			return new FontSizeEditItemUIState(this);
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemDiagramFontSizeEdit edit = new RepositoryItemDiagramFontSizeEdit();
			if(Control != null)
				edit.Diagram = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemDiagramFontSizeEdit edit = (RepositoryItemDiagramFontSizeEdit)Edit;
			if(edit != null)
				edit.Diagram = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			string text = string.Empty;
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			e.Cancel = !FontSizeEditUtils.IsFontSizeValid(edit.EditValue, out text);
			if(e.Cancel) {
				edit.ErrorText = text;
			}
		}
		public object ButtonGroupTag { get { return GroupId.FontGroupId; } }
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontSize; } }
	}
	public class DiagramFontSizeIncreaseBarItem : DiagramCommandBarButtonItem, IBarButtonGroupMember {
		public DiagramFontSizeIncreaseBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("fontsizeincrease;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontSizeIncrease; } }
		public object ButtonGroupTag { get { return GroupId.FontGroupId; } }
	}
	public class DiagramFontSizeDecreaseBarItem : DiagramCommandBarButtonItem, IBarButtonGroupMember {
		public DiagramFontSizeDecreaseBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("fontsizedecrease;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontSizeDecrease; } }
		public object ButtonGroupTag { get { return GroupId.FontGroupId; } }
	}
	public class DiagramFontBoldStyleBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramFontBoldStyleBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("bold;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontBold; } }
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
	}
	public class DiagramFontItalicStyleBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramFontItalicStyleBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("italic;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontItalic; } }
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
	}
	public class DiagramFontUnderlineStyleBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramFontUnderlineStyleBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("underline;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontUnderline; } }
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
	}
	public class DiagramFontStrikethroughStyleBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramFontStrikethroughStyleBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("strikeout;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontStrikethrough; } }
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
	}
	public class DiagramFontColorBarItem : DiagramColorChangeItemBase, IBarButtonGroupMember, IBeginGroupSupport {
		public DiagramFontColorBarItem() {
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontColor; } }
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("FontColor");
		}
		protected override Image LoadDefaultLargeImage() {
			return base.LoadDefaultLargeImage();
		}
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
		public bool BeginGroup { get { return true; }  set { } }
	}
	public class DiagramFontBackColorBarItem : DiagramColorChangeItemBase, IBarButtonGroupMember, IBeginGroupSupport {
		public DiagramFontBackColorBarItem() {
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.FontBackColor; } }
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("Highlight");
		}
		protected override Image LoadDefaultLargeImage() {
			return base.LoadDefaultLargeImage();
		}
		public object ButtonGroupTag { get { return GroupId.FontStyleGroupId; } }
		public bool BeginGroup { get { return true; } set { } }
	}
	#endregion
	#region Paragraph
	public class DiagramAlignTopBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignTopBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("AlignTop");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignTop; } }
		public object ButtonGroupTag { get { return GroupId.VertAlignmentGroupId; } }
	}
	public class DiagramAlignMiddleBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignMiddleBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("AlignCenter");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignMiddle; } }
		public object ButtonGroupTag { get { return GroupId.VertAlignmentGroupId; } }
	}
	public class DiagramAlignBottomBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignBottomBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("AlignBottom");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignBottom; } }
		public object ButtonGroupTag { get { return GroupId.VertAlignmentGroupId; } }
	}
	public class DiagramAlignLeftBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignLeftBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("alignleft;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignLeft; } }
		public object ButtonGroupTag { get { return GroupId.HorzAlignmentGroupId; } }
	}
	public class DiagramAlignCenterBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignCenterBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("aligncenter;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignCenter; } }
		public object ButtonGroupTag { get { return GroupId.HorzAlignmentGroupId; } }
	}
	public class DiagramAlignRightBarItem : DiagramCommandBarCheckItem, IBarButtonGroupMember {
		public DiagramAlignRightBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
		protected override DxImageUri GetImageUri() {
			return new DxImageUri("alignright;Colored;Size16x16");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.AlignRight; } }
		public object ButtonGroupTag { get { return GroupId.HorzAlignmentGroupId; } }
	}
	#endregion
	#region Tools
	public class DiagramPointerToolBarItem : DiagramCommandBarCheckItem {
		public DiagramPointerToolBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("Select");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.PointerTool; } }
	}
	public class DiagramConnectorToolBarItem : DiagramCommandBarCheckItem {
		public DiagramConnectorToolBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("Connector");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.ConnectorTool; } }
	}
	public class DiagramShapeToolSelectionBarItem : DiagramCommandCheckDropDownButtonItem {
		public DiagramShapeToolSelectionBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.ShapeTool; } }
	}
	public class DiagramRectangleToolBarItem : DiagramPopupMenuCommandBarCheckItem {
		public DiagramRectangleToolBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("RectangleTool");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.RectangleTool; } }
	}
	public class DiagramEllipseToolBarItem : DiagramPopupMenuCommandBarCheckItem {
		public DiagramEllipseToolBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("EllipseTool");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.EllipseTool; } }
	}
	public class DiagramRightTriangleToolBarItem : DiagramPopupMenuCommandBarCheckItem {
		public DiagramRightTriangleToolBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("RightTriangleTool");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.RightTriangleTool; } }
	}
	public class DiagramHexagonToolBarItem : DiagramPopupMenuCommandBarCheckItem {
		public DiagramHexagonToolBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("HexagonTool");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.HexagonTool; } }
	}
	#endregion
	#region Arrange
	public class DiagramBringToFrontBarItem : DiagramCommandDropDownButtonItem {
		public DiagramBringToFrontBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("BringToFront");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.BringToFront; } }
	}
	public class DiagramSendToBackBarItem : DiagramCommandDropDownButtonItem {
		public DiagramSendToBackBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("SendToBack");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.SendToBack; } }
	}
	public class DiagramBringForwardPopupMenuBarItem : DiagramPopupMenuCommandBarButtonItem {
		public DiagramBringForwardPopupMenuBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("BringForward");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.BringForward; } }
	}
	public class DiagramBringToFrontPopupMenuBarItem : DiagramPopupMenuCommandBarButtonItem {
		public DiagramBringToFrontPopupMenuBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("BringToFront");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.BringToFront; } }
	}
	public class DiagramSendBackwardPopupMenuBarItem : DiagramPopupMenuCommandBarButtonItem {
		public DiagramSendBackwardPopupMenuBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("SendBackward");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.SendBackward; } }
	}
	public class DiagramSendToBackPopupMenuBarItem : DiagramPopupMenuCommandBarButtonItem {
		public DiagramSendToBackPopupMenuBarItem() {
		}
		protected override Image LoadDefaultImage() {
			return ImageUtils.LoadImage("SendToBack");
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.SendToBack; } }
	}
	#endregion
	#region Show 
	public class DiagramShowRulerCheckItem : DiagramCommandBarCheckItem {
		public DiagramShowRulerCheckItem() {
			CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.ShowRuler; } }
	}
	public class DiagramShowGridCheckItem : DiagramCommandBarCheckItem {
		public DiagramShowGridCheckItem() {
			CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			RibbonStyle = RibbonItemStyles.SmallWithText;
		}
		protected override DiagramCommandId CommandId { get { return DiagramCommandId.ShowGrid; } }
	}
	#endregion
}
