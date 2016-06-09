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
	#region Frameset
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Frameset : IWordObject, IEnumerable {
		Frameset ParentFrameset { get; }
		WdFramesetType Type { get; }
		WdFramesetSizeType WidthType { get; set; }
		WdFramesetSizeType HeightType { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		int ChildFramesetCount { get; }
		Frameset this[int Index] { get; }
		float FramesetBorderWidth { get; set; }
		WdColor FramesetBorderColor { get; set; }
		WdScrollbarType FrameScrollbarType { get; set; }
		bool FrameResizable { get; set; }
		string FrameName { get; set; }
		bool FrameDisplayBorders { get; set; }
		string FrameDefaultURL { get; set; }
		bool FrameLinkToFile { get; set; }
		Frameset AddNewFrame(WdFramesetNewFrameLocation Where);
		void Delete();
	}
	#endregion
	#region WdFramesetType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFramesetType {
		wdFramesetTypeFrameset,
		wdFramesetTypeFrame
	}
	#endregion
	#region WdFramesetSizeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFramesetSizeType {
		wdFramesetSizeTypePercent,
		wdFramesetSizeTypeFixed,
		wdFramesetSizeTypeRelative
	}
	#endregion
	#region WdScrollbarType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdScrollbarType {
		wdScrollbarTypeAuto,
		wdScrollbarTypeYes,
		wdScrollbarTypeNo
	}
	#endregion
	#region WdFramesetNewFrameLocation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFramesetNewFrameLocation {
		wdFramesetNewFrameAbove,
		wdFramesetNewFrameBelow,
		wdFramesetNewFrameRight,
		wdFramesetNewFrameLeft
	}
	#endregion
}
