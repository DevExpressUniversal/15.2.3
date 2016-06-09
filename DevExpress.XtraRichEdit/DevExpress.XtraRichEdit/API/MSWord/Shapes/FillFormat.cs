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
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region FillFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface FillFormat : IWordObject {
		ColorFormat BackColor { get; }
		ColorFormat ForeColor { get; }
		MsoGradientColorType GradientColorType { get; }
		float GradientDegree { get; }
		MsoGradientStyle GradientStyle { get; }
		int GradientVariant { get; }
		MsoPatternType Pattern { get; }
		MsoPresetGradientType PresetGradientType { get; }
		MsoPresetTexture PresetTexture { get; }
		string TextureName { get; }
		MsoTextureType TextureType { get; }
		float Transparency { get; set; }
		MsoFillType Type { get; }
		MsoTriState Visible { get; set; }
		void Background();
		void OneColorGradient(MsoGradientStyle Style, int Variant, float Degree);
		void Patterned(MsoPatternType Pattern);
		void PresetGradient(MsoGradientStyle Style, int Variant, MsoPresetGradientType PresetGradientType);
		void PresetTextured(MsoPresetTexture PresetTexture);
		void Solid();
		void TwoColorGradient(MsoGradientStyle Style, int Variant);
		void UserPicture(string PictureFile);
		void UserTextured(string TextureFile);
	}
	#endregion
}
