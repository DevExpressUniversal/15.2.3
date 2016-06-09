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
	#region HorizontalLineFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface HorizontalLineFormat : IWordObject {
		float PercentWidth { get; set; }
		bool NoShade { get; set; }
		WdHorizontalLineAlignment Alignment { get; set; }
		WdHorizontalLineWidthType WidthType { get; set; }
	}
	#endregion
	#region WdHorizontalLineAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdHorizontalLineAlignment {
		wdHorizontalLineAlignLeft,
		wdHorizontalLineAlignCenter,
		wdHorizontalLineAlignRight
	}
	#endregion
	#region WdHorizontalLineWidthType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdHorizontalLineWidthType {
		wdHorizontalLineFixedWidth = -2,
		wdHorizontalLinePercentWidth = -1
	}
	#endregion
}
