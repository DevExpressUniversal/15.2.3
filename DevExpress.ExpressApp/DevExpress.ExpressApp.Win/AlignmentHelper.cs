#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win {
	public class AlignmentHelper {
		public static ContentAlignment GetAlignment(ISupportControlAlignment supportControlAlignment) {
			if(supportControlAlignment != null) {
				return GetAlignment(supportControlAlignment.HorizontalAlign, supportControlAlignment.VerticalAlign);
			}
			return ContentAlignment.TopLeft;
		}
		public static ContentAlignment GetAlignment(StaticHorizontalAlign hAlign, StaticVerticalAlign vAlign) {
			ContentAlignment result = ContentAlignment.TopLeft;
			switch(vAlign) {
				case StaticVerticalAlign.NotSet:
				case StaticVerticalAlign.Top:
					switch(hAlign) {
						case StaticHorizontalAlign.NotSet:
						case StaticHorizontalAlign.Left:
							result = ContentAlignment.TopLeft;
							break;
						case StaticHorizontalAlign.Center:
							result = ContentAlignment.TopCenter;
							break;
						case StaticHorizontalAlign.Right:
							result = ContentAlignment.TopRight;
							break;
					}
					break;
				case StaticVerticalAlign.Bottom:
					switch(hAlign) {
						case StaticHorizontalAlign.NotSet:
						case StaticHorizontalAlign.Left:
							result = ContentAlignment.BottomLeft;
							break;
						case StaticHorizontalAlign.Center:
							result = ContentAlignment.BottomCenter;
							break;
						case StaticHorizontalAlign.Right:
							result = ContentAlignment.BottomRight;
							break;
					}
					break;
				case StaticVerticalAlign.Middle:
					switch(hAlign) {
						case StaticHorizontalAlign.NotSet:
						case StaticHorizontalAlign.Left:
							result = ContentAlignment.MiddleLeft;
							break;
						case StaticHorizontalAlign.Center:
							result = ContentAlignment.MiddleCenter;
							break;
						case StaticHorizontalAlign.Right:
							result = ContentAlignment.MiddleRight;
							break;
					}
					break;
			}
			return result;
		}
	}
}
