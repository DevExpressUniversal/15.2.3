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
namespace DevExpress.XtraRichEdit.API.Word {
	#region WrapFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface WrapFormat : IWordObject {
		WdWrapType Type { get; set; }
		WdWrapSideType Side { get; set; }
		float DistanceTop { get; set; }
		float DistanceBottom { get; set; }
		float DistanceLeft { get; set; }
		float DistanceRight { get; set; }
		int AllowOverlap { get; set; }
	}
	#endregion
	#region WdWrapSideType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdWrapSideType {
		wdWrapBoth,
		wdWrapLeft,
		wdWrapRight,
		wdWrapLargest
	}
	#endregion
	#region WdWrapType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdWrapType {
		wdWrapBehind = 5,
		wdWrapFront = 3,
		wdWrapInline = 7,
		wdWrapNone = 3,
		wdWrapSquare = 0,
		wdWrapThrough = 2,
		wdWrapTight = 1,
		wdWrapTopBottom = 4
	}
	#endregion
}
