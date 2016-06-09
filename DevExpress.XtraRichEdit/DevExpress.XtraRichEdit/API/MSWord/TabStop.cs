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
	#region TabStop
	[GeneratedCode("Suppress FxCop check", "")]
	public interface TabStop : IWordObject {
		WdTabAlignment Alignment { get; set; }
		WdTabLeader Leader { get; set; }
		float Position { get; set; }
		bool CustomTab { get; }
		TabStop Next { get; }
		TabStop Previous { get; }
		void Clear();
	}
	#endregion
	#region TabStops
	[GeneratedCode("Suppress FxCop check", "")]
	public interface TabStops : IWordObject, IEnumerable {
		int Count { get; }
		TabStop this[object Index] { get; } 
		TabStop Add(float Position, ref object Alignment, ref object Leader);
		void ClearAll();
		TabStop Before(float Position);
		TabStop After(float Position);
	}
	#endregion
	#region WdTabLeader
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTabLeader {
		wdTabLeaderSpaces,
		wdTabLeaderDots,
		wdTabLeaderDashes,
		wdTabLeaderLines,
		wdTabLeaderHeavy,
		wdTabLeaderMiddleDot
	}
	#endregion
	#region WdTabAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTabAlignment {
		wdAlignTabBar = 4,
		wdAlignTabCenter = 1,
		wdAlignTabDecimal = 3,
		wdAlignTabLeft = 0,
		wdAlignTabList = 6,
		wdAlignTabRight = 2
	}
	#endregion
}
