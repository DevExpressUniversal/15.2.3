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
	#region LineFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface LineFormat : IWordObject {
		ColorFormat BackColor { get; }
		MsoArrowheadLength BeginArrowheadLength { get; set; }
		MsoArrowheadStyle BeginArrowheadStyle { get; set; }
		MsoArrowheadWidth BeginArrowheadWidth { get; set; }
		MsoLineDashStyle DashStyle { get; set; }
		MsoArrowheadLength EndArrowheadLength { get; set; }
		MsoArrowheadStyle EndArrowheadStyle { get; set; }
		MsoArrowheadWidth EndArrowheadWidth { get; set; }
		ColorFormat ForeColor { get; }
		MsoPatternType Pattern { get; set; }
		MsoLineStyle Style { get; set; }
		float Transparency { get; set; }
		MsoTriState Visible { get; set; }
		float Weight { get; set; }
		MsoTriState InsetPen { get; set; }
	}
	#endregion
}
