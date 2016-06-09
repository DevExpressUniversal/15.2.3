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
	#region ShapeNode
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ShapeNode : IMsoObject {
		MsoEditingType EditingType { get; }
		object Points { get; }
		MsoSegmentType SegmentType { get; }
	}
	#endregion
	#region ShapeNodes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ShapeNodes : IMsoObject, IEnumerable {
		int Count { get; }
		ShapeNode Item(object Index);
		void Delete(int Index);
		void Insert(int Index, MsoSegmentType SegmentType, MsoEditingType EditingType, float X1, float Y1, float X2, float Y2, float X3, float Y3);
		void SetEditingType(int Index, MsoEditingType EditingType);
		void SetPosition(int Index, float X1, float Y1);
		void SetSegmentType(int Index, MsoSegmentType SegmentType);
	}
	#endregion
}
