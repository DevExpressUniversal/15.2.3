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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms {
	public static class TabsFormHelper {
		public static int GetIndexFromAlignment(TabAlignmentType alignment) {
			switch(alignment) {
				case TabAlignmentType.Center:
					return 2;
				case TabAlignmentType.Decimal:
					return 1;
				case TabAlignmentType.Left:
					return 0;
				case TabAlignmentType.Right:
					return 3;
				default:
					return -1;
			}
		}
		public static TabAlignmentType GetAlignmentFromIndex(int indx) {
			switch(indx) {
				case 0:
					return TabAlignmentType.Left;
				case 1:
					return TabAlignmentType.Decimal;
				case 2:
					return TabAlignmentType.Center;
				case 3:
					return TabAlignmentType.Right;
				default:
					return TabAlignmentType.Left;
			}
		}
		public static int GetIndexFromLeader(TabLeaderType tabLeaderType) {
			switch(tabLeaderType) {
				case TabLeaderType.None:
					return 0;
				case TabLeaderType.Dots:
					return 1;
				case TabLeaderType.MiddleDots:
					return 2;
				case TabLeaderType.Hyphens:
					return 3;
				case TabLeaderType.Underline:
					return 4;
				case TabLeaderType.ThickLine:
					return 5;
				case TabLeaderType.EqualSign:
					return 6;
			}
			return 0;
		}
		public static TabLeaderType GetLeaderFromIndex(int index) {
			switch(index) {
				case 0:
					return TabLeaderType.None;
				case 1:
					return TabLeaderType.Dots;
				case 2:
					return TabLeaderType.MiddleDots;
				case 3:
					return TabLeaderType.Hyphens;
				case 4:
					return TabLeaderType.Underline;
				case 5:
					return TabLeaderType.ThickLine;
				case 6:
					return TabLeaderType.EqualSign;
				default:
					return TabLeaderType.None;
			}
		}
	}
}
