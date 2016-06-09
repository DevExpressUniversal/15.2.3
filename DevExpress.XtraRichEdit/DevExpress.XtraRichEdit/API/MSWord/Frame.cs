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
	#region Frame
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Frame : IWordObject {
		WdFrameSizeRule HeightRule { get; set; }
		WdFrameSizeRule WidthRule { get; set; }
		float HorizontalDistanceFromText { get; set; }
		float Height { get; set; }
		float HorizontalPosition { get; set; }
		bool LockAnchor { get; set; }
		WdRelativeHorizontalPosition RelativeHorizontalPosition { get; set; }
		WdRelativeVerticalPosition RelativeVerticalPosition { get; set; }
		float VerticalDistanceFromText { get; set; }
		float VerticalPosition { get; set; }
		float Width { get; set; }
		bool TextWrap { get; set; }
		Shading Shading { get; }
		Borders Borders { get; set; }
		Range Range { get; }
		void Delete();
		void Select();
		void Copy();
		void Cut();
	}
	#endregion
	#region Frames
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Frames : IWordObject, IEnumerable {
		int Count { get; }
		Frame this[int Index] { get; }
		Frame Add(Range Range);
		void Delete();
	}
	#endregion
	#region WdFrameSizeRule
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFrameSizeRule {
		wdFrameAuto,
		wdFrameAtLeast,
		wdFrameExact
	}
	#endregion
}
