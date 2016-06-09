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
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region ThreeDFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ThreeDFormat : IMsoObject {
		void IncrementRotationX(float Increment);
		void IncrementRotationY(float Increment);
		void ResetRotation();
		void SetThreeDFormat(MsoPresetThreeDFormat PresetThreeDFormat);
		void SetExtrusionDirection(MsoPresetExtrusionDirection PresetExtrusionDirection);
		float Depth { get; set; }
		ColorFormat ExtrusionColor { get; }
		MsoExtrusionColorType ExtrusionColorType { get; set; }
		MsoTriState Perspective { get; set; }
		MsoPresetExtrusionDirection PresetExtrusionDirection { get; }
		MsoPresetLightingDirection PresetLightingDirection { get; set; }
		MsoPresetLightingSoftness PresetLightingSoftness { get; set; }
		MsoPresetMaterial PresetMaterial { get; set; }
		MsoPresetThreeDFormat PresetThreeDFormat { get; }
		float RotationX { get; set; }
		float RotationY { get; set; }
		MsoTriState Visible { get; set; }
		void SetPresetCamera(MsoPresetCamera PresetCamera);
		void IncrementRotationZ(float Increment);
		void IncrementRotationHorizontal(float Increment);
		void IncrementRotationVertical(float Increment);
		MsoLightRigType PresetLighting { get; set; }
		float Z { get; set; }
		MsoBevelType BevelTopType { get; set; }
		float BevelTopInset { get; set; }
		float BevelTopDepth { get; set; }
		MsoBevelType BevelBottomType { get; set; }
		float BevelBottomInset { get; set; }
		float BevelBottomDepth { get; set; }
		MsoPresetCamera PresetCamera { get; }
		float RotationZ { get; set; }
		float ContourWidth { get; set; }
		ColorFormat ContourColor { get; }
		float FieldOfView { get; set; }
		MsoTriState ProjectText { get; set; }
		float LightAngle { get; set; }
	}
	#endregion
}
