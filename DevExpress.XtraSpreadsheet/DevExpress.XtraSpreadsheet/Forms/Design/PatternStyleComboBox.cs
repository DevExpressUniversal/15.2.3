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

using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Export.Xl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[
	UserRepositoryItem("RegisterPatternStyleEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemPatternStyle : RepositoryItemImageComboBox {
		internal static string InternalEditorTypeName { get { return typeof(PatternStyleImageComboBoxEdit).Name; } }
		public static void RegisterPatternStyleEdit() {
			EditorClassInfo info = EditorRegistrationInfo.Default.Editors[InternalEditorTypeName];
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraSpreadsheet.Bitmaps256.SpreadsheetControl.bmp", System.Reflection.Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(PatternStyleImageComboBoxEdit).Name, typeof(PatternStyleImageComboBoxEdit), typeof(RepositoryItemPatternStyle), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new PatternStylePainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
	}
	[DXToolboxItem(false)]
	public partial class PatternStyleImageComboBoxEdit : ImageComboBoxEdit {
		static PatternStyleImageComboBoxEdit() {
			RepositoryItemPatternStyle.RegisterPatternStyleEdit();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemPatternStyle Properties { get { return base.Properties as RepositoryItemPatternStyle; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new PatternStyleComboBoxEditListBoxForm(this);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	public static class PatternStyleComboBoxEditColor {
		public static Color DrawComboBoxEditColor { get; set; }
	}
	public class PatternStyleComboBoxEditListBoxForm : PopupImageComboBoxEditListBoxForm {
		public PatternStyleComboBoxEditListBoxForm(ComboBoxEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupListBox CreateListBox() {
			return new PatternStyleComboBoxEditListBox(this);
		}
		public override int CalcMinimumComboWidth() {
			return OwnerEdit.Width;
		}
	}
	public class PatternStyleComboBoxEditListBox : PopupImageComboBoxEditListBox {
		public PatternStyleComboBoxEditListBox(PopupListBoxForm ownerForm)
			: base(ownerForm) {
			this.ColumnWidth = 25;
			this.MultiColumn = true;
		}
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new PatternStyleEditPopupImageComboBoxEditListBoxViewInfo(this);
		}
		public new PatternStyleEditPopupImageComboBoxEditListBoxViewInfo ViewInfo { get { return (PatternStyleEditPopupImageComboBoxEditListBoxViewInfo)base.ViewInfo; } }
	}
	public class PatternStylePainter : ImageComboBoxEditPainter {
		#region Fields
		HatchStyle hatchStyle;
		readonly Dictionary<XlPatternType, HatchStyle> brushHatchStyles;
		#endregion
		public PatternStylePainter() {
			this.brushHatchStyles = CreateBrushHatchStyleTable();
		}
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			this.brushHatchStyles.TryGetValue(GetPatternType(info.ViewInfo.DisplayText), out hatchStyle);
			if (GetPatternType(info.ViewInfo.DisplayText) != XlPatternType.None) {
				if (GetPatternType(info.ViewInfo.DisplayText) != XlPatternType.Solid)
					info.Graphics.FillRectangle(new HatchBrush(hatchStyle, PatternStyleComboBoxEditColor.DrawComboBoxEditColor, Color.Empty), info.Bounds);
				else
					info.Graphics.FillRectangle(new SolidBrush(Color.Empty), info.Bounds);
			}
		}
		XlPatternType GetPatternType(string patternType) {
			if (String.IsNullOrEmpty(patternType)) return XlPatternType.None;
			return (XlPatternType)Enum.Parse(typeof(XlPatternType), patternType);
		}
		protected internal Dictionary<XlPatternType, HatchStyle> CreateBrushHatchStyleTable() {
			Dictionary<XlPatternType, HatchStyle> result = new Dictionary<XlPatternType, HatchStyle>();
			result.Add(XlPatternType.None, HatchStyle.Percent05); 
			result.Add(XlPatternType.Solid, HatchStyle.Percent90); 
			result.Add(XlPatternType.MediumGray, HatchStyle.Percent50);
			result.Add(XlPatternType.DarkGray, HatchStyle.Percent70);
			result.Add(XlPatternType.LightGray, HatchStyle.Percent25);
			result.Add(XlPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(XlPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(XlPatternType.DarkDown, HatchStyle.DarkDownwardDiagonal);
			result.Add(XlPatternType.DarkUp, HatchStyle.DarkUpwardDiagonal);
			result.Add(XlPatternType.DarkGrid, HatchStyle.SmallCheckerBoard);
			result.Add(XlPatternType.DarkTrellis, HatchStyle.Trellis);
			result.Add(XlPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(XlPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(XlPatternType.LightDown, HatchStyle.LightDownwardDiagonal);
			result.Add(XlPatternType.LightUp, HatchStyle.LightUpwardDiagonal);
			result.Add(XlPatternType.LightGrid, HatchStyle.SmallGrid);
			result.Add(XlPatternType.LightTrellis, HatchStyle.Percent30);
			result.Add(XlPatternType.Gray125, HatchStyle.Percent20);
			result.Add(XlPatternType.Gray0625, HatchStyle.Percent10);
			return result;
		}
	}
	public class PatternStyleEditPopupImageComboBoxEditListBoxViewInfo : PopupImageComboBoxEditListBoxViewInfo {
		public PatternStyleEditPopupImageComboBoxEditListBoxViewInfo(PopupImageComboBoxEditListBox owner)
			: base(owner) {
		}
		public new ListBoxItemObjectInfoArgs ListBoxItemInfoArgs { get { return base.ListBoxItemInfoArgs; } }
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if (IsSkinnedHighlightingEnabled)
				return new PatternStyleEditPopupImageListBoxSkinItemPainter();
			return new PatternStyleEditPopupImageListBoxItemPainter();
		}
	}
	public class PatternStyleEditPopupImageListBoxSkinItemPainter : PopupImageListBoxSkinItemPainter {
		#region Fields
		HatchStyle hatchStyle;
		HatchBrush hatchBrush;
		readonly Dictionary<XlPatternType, HatchStyle> brushHatchStyles;
		#endregion
		public PatternStyleEditPopupImageListBoxSkinItemPainter() {
			this.brushHatchStyles = CreateBrushHatchStyleTable();
		}
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			this.brushHatchStyles.TryGetValue(GetPatternType(e.ItemText), out hatchStyle);
			hatchBrush = GetPatternType(e.ItemText) != XlPatternType.Solid ? new HatchBrush(hatchStyle, Color.Black, Color.Empty) : new HatchBrush(hatchStyle, Color.Empty, Color.Empty);
			e.Graphics.FillRectangle(hatchBrush, e.TextRect);
		}
		XlPatternType GetPatternType(string patternType) {
			return (XlPatternType)Enum.Parse(typeof(XlPatternType), patternType);
		}
		protected internal Dictionary<XlPatternType, HatchStyle> CreateBrushHatchStyleTable() {
			Dictionary<XlPatternType, HatchStyle> result = new Dictionary<XlPatternType, HatchStyle>();
			result.Add(XlPatternType.None, HatchStyle.Percent05); 
			result.Add(XlPatternType.Solid, HatchStyle.Percent90); 
			result.Add(XlPatternType.MediumGray, HatchStyle.Percent50);
			result.Add(XlPatternType.DarkGray, HatchStyle.Percent70);
			result.Add(XlPatternType.LightGray, HatchStyle.Percent25);
			result.Add(XlPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(XlPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(XlPatternType.DarkDown, HatchStyle.DarkDownwardDiagonal);
			result.Add(XlPatternType.DarkUp, HatchStyle.DarkUpwardDiagonal);
			result.Add(XlPatternType.DarkGrid, HatchStyle.SmallCheckerBoard);
			result.Add(XlPatternType.DarkTrellis, HatchStyle.Trellis);
			result.Add(XlPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(XlPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(XlPatternType.LightDown, HatchStyle.LightDownwardDiagonal);
			result.Add(XlPatternType.LightUp, HatchStyle.LightUpwardDiagonal);
			result.Add(XlPatternType.LightGrid, HatchStyle.SmallGrid);
			result.Add(XlPatternType.LightTrellis, HatchStyle.Percent30);
			result.Add(XlPatternType.Gray125, HatchStyle.Percent20);
			result.Add(XlPatternType.Gray0625, HatchStyle.Percent10);
			return result;
		}
	}
	public class PatternStyleEditPopupImageListBoxItemPainter : PopupImageListBoxItemPainter {
		#region Fields
		HatchStyle hatchStyle;
		HatchBrush hatchBrush;
		readonly Dictionary<XlPatternType, HatchStyle> brushHatchStyles;
		#endregion
		public PatternStyleEditPopupImageListBoxItemPainter() {
			this.brushHatchStyles = CreateBrushHatchStyleTable();
		}
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			this.brushHatchStyles.TryGetValue(GetPatternType(e.ItemText), out hatchStyle);
			hatchBrush = GetPatternType(e.ItemText) != XlPatternType.Solid ? new HatchBrush(hatchStyle, Color.Black, Color.Empty) : new HatchBrush(hatchStyle, Color.Empty, Color.Empty);
			e.Graphics.FillRectangle(hatchBrush, e.TextRect);
		}
		XlPatternType GetPatternType(string patternType) {
			return (XlPatternType)Enum.Parse(typeof(XlPatternType), patternType);
		}
		protected internal Dictionary<XlPatternType, HatchStyle> CreateBrushHatchStyleTable() {
			Dictionary<XlPatternType, HatchStyle> result = new Dictionary<XlPatternType, HatchStyle>();
			result.Add(XlPatternType.None, HatchStyle.Percent05); 
			result.Add(XlPatternType.Solid, HatchStyle.Percent90); 
			result.Add(XlPatternType.MediumGray, HatchStyle.Percent50);
			result.Add(XlPatternType.DarkGray, HatchStyle.Percent70);
			result.Add(XlPatternType.LightGray, HatchStyle.Percent25);
			result.Add(XlPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(XlPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(XlPatternType.DarkDown, HatchStyle.DarkDownwardDiagonal);
			result.Add(XlPatternType.DarkUp, HatchStyle.DarkUpwardDiagonal);
			result.Add(XlPatternType.DarkGrid, HatchStyle.SmallCheckerBoard);
			result.Add(XlPatternType.DarkTrellis, HatchStyle.Trellis);
			result.Add(XlPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(XlPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(XlPatternType.LightDown, HatchStyle.LightDownwardDiagonal);
			result.Add(XlPatternType.LightUp, HatchStyle.LightUpwardDiagonal);
			result.Add(XlPatternType.LightGrid, HatchStyle.SmallGrid);
			result.Add(XlPatternType.LightTrellis, HatchStyle.Percent30);
			result.Add(XlPatternType.Gray125, HatchStyle.Percent20);
			result.Add(XlPatternType.Gray0625, HatchStyle.Percent10);
			return result;
		}
	}
}
