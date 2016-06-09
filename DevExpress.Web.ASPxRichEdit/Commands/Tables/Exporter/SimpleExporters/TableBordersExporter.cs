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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public enum JSONTableBordersProperty {
		TopBorder = 0,
		LeftBorder = 1,
		RightBorder = 2,
		BottomBorder = 3,
		InsideHorizontalBorder = 4,
		InsideVerticalBorder = 5
	}
	public static class TableBordersExporter {
		public static Hashtable ToHashtable(TableBorders from) {
			Hashtable result = new Hashtable();
			result.Add((int)JSONTableBordersProperty.TopBorder, BorderBaseExporter.ToHashtable(from.TopBorder));
			result.Add((int)JSONTableBordersProperty.LeftBorder, BorderBaseExporter.ToHashtable(from.LeftBorder));
			result.Add((int)JSONTableBordersProperty.RightBorder, BorderBaseExporter.ToHashtable(from.RightBorder));
			result.Add((int)JSONTableBordersProperty.BottomBorder, BorderBaseExporter.ToHashtable(from.BottomBorder));
			result.Add((int)JSONTableBordersProperty.InsideHorizontalBorder, BorderBaseExporter.ToHashtable(from.InsideHorizontalBorder));
			result.Add((int)JSONTableBordersProperty.InsideVerticalBorder, BorderBaseExporter.ToHashtable(from.InsideVerticalBorder));
			return result;
		}
		public static void FromHashtable(Hashtable from, TableBorders to) {
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.TopBorder).ToString()], to.TopBorder);
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.LeftBorder).ToString()], to.LeftBorder);
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.RightBorder).ToString()], to.RightBorder);
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.BottomBorder).ToString()], to.BottomBorder);
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.InsideHorizontalBorder).ToString()], to.InsideHorizontalBorder);
			BorderBaseExporter.FromHashtable((Hashtable)from[((int)JSONTableBordersProperty.InsideVerticalBorder).ToString()], to.InsideVerticalBorder);
		}
	}
}
