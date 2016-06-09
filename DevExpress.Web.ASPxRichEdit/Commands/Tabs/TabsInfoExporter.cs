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
	public enum JSONTabInfoProperty {
		Alignment = 0,
		LeaderType = 1,
		Position = 2,
		Deleted = 3,
		IsDefault = 4
	}
	public static class TabInfoExporter {
		public static Hashtable ToHashtable(TabInfo from) {
			Hashtable result = new Hashtable();
			TabInfoExporter.FillHashtable(result, from);
			return result;
		}
		public static void FillHashtable(Hashtable result, TabInfo from) {
			result.Add((int)JSONTabInfoProperty.Alignment, (int)from.Alignment);
			result.Add((int)JSONTabInfoProperty.LeaderType, (int)from.Leader);
			result.Add((int)JSONTabInfoProperty.Position, from.Position);
			result.Add((int)JSONTabInfoProperty.Deleted, from.Deleted);
			result.Add((int)JSONTabInfoProperty.IsDefault, from.IsDefault);
		}
		public static TabInfo FromHashtable(Hashtable from) {
			if((bool)from[((int)JSONTabInfoProperty.IsDefault).ToString()])
				return new DefaultTabInfo((int)from[((int)JSONTabInfoProperty.Position).ToString()]);
			else
				return new TabInfo((int)from[((int)JSONTabInfoProperty.Position).ToString()],
					(TabAlignmentType)from[((int)JSONTabInfoProperty.Alignment).ToString()],
					(TabLeaderType)from[((int)JSONTabInfoProperty.LeaderType).ToString()],
					(bool)from[((int)JSONTabInfoProperty.Deleted).ToString()]);
		}
	}
}
