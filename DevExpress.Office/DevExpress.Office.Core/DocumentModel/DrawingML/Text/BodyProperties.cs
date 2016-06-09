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
namespace DevExpress.Office.DrawingML {
	#region DrawingTextAnchoringType
	public enum DrawingTextAnchoringType {
		Bottom = 1,
		Center = 2,
		Distributed = 3,
		Justified = 4,
		Top = 5
	}
	#endregion
	#region DrawingTextHorizontalOverflowType
	public enum DrawingTextHorizontalOverflowType {
		Clip = 0,
		Overflow = 1
	}
	#endregion
	#region DrawingTextVerticalOverflowType
	public enum DrawingTextVerticalOverflowType {
		Clip = 0,
		Ellipsis = 1,
		Overflow = 2
	}
	#endregion
	#region DrawingTextWrappingType
	public enum DrawingTextWrappingType {
		None = 0,
		Square = 1
	}
	#endregion
	#region DrawingTextVerticalTextType
	public enum DrawingTextVerticalTextType {
		EastAsianVertical = 0,
		Horizontal = 1,
		MongolianVertical = 2,
		Vertical = 3,
		Vertical270 = 4,
		WordArtVertical = 5,
		VerticalWordArtRightToLeft = 6
	}
	#endregion
}
