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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class WebParagraphPropertiesModifier {
		public void ModifyParagraphProperties(ParagraphProperties properties, object newValue, bool newUse) {
			var baseInfo = properties.GetInfoForModification();
			var info = baseInfo.GetInfoForModification();
			var options = baseInfo.GetOptionsForModification();
			ApplyNewValue(info, newValue);
			ApplyNewUseValue(options, newUse);
			baseInfo.ReplaceInfo(info, options);
			properties.ReplaceInfo(baseInfo, ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType));
		}
		protected abstract ParagraphFormattingChangeType ParagraphFormattingChangeType { get; }
		public abstract ParagraphFormattingOptions.Mask Mask { get; }
		protected ParagraphFormattingOptions.Mask GetNewMask(ParagraphFormattingOptions.Mask oldMask, bool useValue) {
			if(useValue)
				return oldMask | Mask;
			else
				return oldMask & ~Mask;
		}
		public abstract void ApplyNewValue(ParagraphFormattingInfo info, object newValue);
		public void ApplyNewUseValue(ParagraphFormattingOptions options, bool useValue) {
			options.Value = GetNewMask(options.Value, useValue);
		}
	}
	public abstract class WebParagraphPropertiesModifier<T> : WebParagraphPropertiesModifier {
		protected abstract void ModifyParagraphFormatting(ParagraphFormattingInfo info, T value);
		protected virtual T GetNewValue(object value) { return (T)value; }
		public override void ApplyNewValue(ParagraphFormattingInfo info, object newValue) {
			T value = GetNewValue(newValue);
			ModifyParagraphFormatting(info, value);
		}
	}
	public abstract class WebParagraphPropertiesFloatModifier : WebParagraphPropertiesModifier<float> {
		protected override float GetNewValue(object obj) {
			return Convert.ToSingle(obj);
		}
	}
	public abstract class WebParagraphPropertiesColorModifier : WebParagraphPropertiesModifier<Color> {
		protected override Color GetNewValue(object obj) {
			return PropertiesHelper.GetColorFromArgb((int)obj);
		}
	}
	public abstract class WebParagraphPropertiesBorderInfoModifier : WebParagraphPropertiesModifier<BorderInfo> {
		protected override BorderInfo GetNewValue(object obj) {
			Hashtable ht = (Hashtable)obj;
			BorderInfo result = new BorderInfo();
			result.Style = (BorderLineStyle)ht["style"];
			result.Color = Color.FromArgb((int)ht["color"]);
			result.Width = (int)ht["width"];
			result.Offset = (int)ht["offset"];
			result.Frame = Convert.ToBoolean(ht["offset"]);
			result.Shadow = Convert.ToBoolean(ht["shadow"]);
			return result;
		}
	}
	public class WebParagraphPropertiesAlignmentModifier : WebParagraphPropertiesModifier<ParagraphAlignment> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseAlignment; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, ParagraphAlignment value) {
			info.Alignment = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.Alignment; }
		}
	}
	public class WebParagraphPropertiesFirstLineIndentModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseFirstLineIndent; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.FirstLineIndent = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.FirstLineIndent; }
		}
	}
	public class WebParagraphPropertiesFirstLineIndentTypeModifier : WebParagraphPropertiesModifier<ParagraphFirstLineIndent> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseFirstLineIndent; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, ParagraphFirstLineIndent value) {
			info.FirstLineIndentType = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.FirstLineIndentType; }
		}
	}
	public class WebParagraphPropertiesLeftIndentModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseLeftIndent; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.LeftIndent = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.LeftIndent; }
		}
	}
	public class WebParagraphPropertiesLineSpacingModifier : WebParagraphPropertiesFloatModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseLineSpacing; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, float value) {
			info.LineSpacing = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.LineSpacing; }
		}
	}
	public class WebParagraphPropertiesLineSpacingTypeModifier : WebParagraphPropertiesModifier<ParagraphLineSpacing> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseLineSpacing; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, ParagraphLineSpacing value) {
			info.LineSpacingType = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.LineSpacingType; }
		}
	}
	public class WebParagraphPropertiesRightIndentModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseRightIndent; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.RightIndent = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.RightIndent; }
		}
	}
	public class WebParagraphPropertiesSpacingBeforeModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseSpacingBefore; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.SpacingBefore = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.SpacingBefore; }
		}
	}
	public class WebParagraphPropertiesSpacingAfterModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseSpacingAfter; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.SpacingAfter = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.SpacingAfter; }
		}
	}
	public class WebParagraphPropertiesSuppressHyphenationModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseSuppressHyphenation; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.SuppressHyphenation = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.SuppressHyphenation; }
		}
	}
	public class WebParagraphPropertiesSuppressLineNumbersModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseSuppressLineNumbers; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.SuppressLineNumbers = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.SuppressLineNumbers; }
		}
	}
	public class WebParagraphPropertiesContextualSpacingModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseContextualSpacing; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.ContextualSpacing = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.ContextualSpacing; }
		}
	}
	public class WebParagraphPropertiesPageBreakBeforeModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UsePageBreakBefore; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.PageBreakBefore = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.PageBreakBefore; }
		}
	}
	public class WebParagraphPropertiesBeforeAutoSpacingModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseBeforeAutoSpacing; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.BeforeAutoSpacing = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.BeforeAutoSpacing; }
		}
	}
	public class WebParagraphPropertiesAfterAutoSpacingModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseAfterAutoSpacing; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.AfterAutoSpacing = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.AfterAutoSpacing; }
		}
	}
	public class WebParagraphPropertiesKeepWithNextModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseKeepWithNext; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.KeepWithNext = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.KeepWithNext; }
		}
	}
	public class WebParagraphPropertiesKeepLinesTogetherModifier : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseKeepLinesTogether; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.KeepLinesTogether = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.KeepLinesTogether; }
		}
	}
	public class WebParagraphPropertiesWidowOrphanControl : WebParagraphPropertiesModifier<bool> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseWidowOrphanControl; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, bool value) {
			info.WidowOrphanControl = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.WidowOrphanControl; }
		}
	}
	public class WebParagraphPropertiesOutlineLevelModifier : WebParagraphPropertiesModifier<int> {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseOutlineLevel; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, int value) {
			info.OutlineLevel = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.OutlineLevel; }
		}
	}
	public class WebParagraphPropertiesBackColorModifier : WebParagraphPropertiesColorModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseBackColor; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, Color value) {
			info.BackColor = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.BackColor; }
		}
	}
	public class WebParagraphPropertiesLeftBorderModifier : WebParagraphPropertiesBorderInfoModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseLeftBorder; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, BorderInfo value) {
			info.LeftBorder = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.LeftBorder; }
		}
	}
	public class WebParagraphPropertiesRightBorderModifier : WebParagraphPropertiesBorderInfoModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseRightBorder; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, BorderInfo value) {
			info.RightBorder = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.RightBorder; }
		}
	}
	public class WebParagraphPropertiesTopBorderModifier : WebParagraphPropertiesBorderInfoModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseTopBorder; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, BorderInfo value) {
			info.TopBorder = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.TopBorder; }
		}
	}
	public class WebParagraphPropertiesBottomBorderModifier : WebParagraphPropertiesBorderInfoModifier {
		public override ParagraphFormattingOptions.Mask Mask { get { return ParagraphFormattingOptions.Mask.UseBottomBorder; } }
		protected override void ModifyParagraphFormatting(ParagraphFormattingInfo info, BorderInfo value) {
			info.BottomBorder = value;
		}
		protected override ParagraphFormattingChangeType ParagraphFormattingChangeType {
			get { return XtraRichEdit.Model.ParagraphFormattingChangeType.BottomBorder; }
		}
	}
	public static class PropertiesHelper {
		public static Color GetColorFromArgb(int argb) {
			if(argb == Color.Empty.ToArgb())
				return Color.Empty;
			else if(argb == Color.Transparent.ToArgb())
					return Color.Transparent;
				else
					return Color.FromArgb(argb);
		}
	}
}
