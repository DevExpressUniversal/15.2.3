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
using System.Collections;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public static class BorderInfoExporter {		
		public static Hashtable ToHashtable(DevExpress.XtraRichEdit.Model.BorderInfo info) {
			Hashtable result = new Hashtable();
			result.Add("0", (int)info.Style);			
			result.Add("1", info.Color.ToArgb());			
			result.Add("2", info.Width);			
			result.Add("3", info.Offset);			
			result.Add("4", info.Frame);			
			result.Add("5", info.Shadow);			
			return result;
		}
		public static DevExpress.XtraRichEdit.Model.BorderInfo FromHashtable(Hashtable source) {
			DevExpress.XtraRichEdit.Model.BorderInfo info = new DevExpress.XtraRichEdit.Model.BorderInfo();
			info.Style = (DevExpress.XtraRichEdit.Model.BorderLineStyle)source["0"];
			info.Color = DevExpress.Web.ASPxRichEdit.Internal.PropertiesHelper.GetColorFromArgb((int)source["1"]);
			info.Width = Convert.ToInt32(source["2"]);
			info.Offset = Convert.ToInt32(source["3"]);
			info.Frame = Convert.ToBoolean(source["4"]);
			info.Shadow = Convert.ToBoolean(source["5"]);
			return info;
		}
	}
}
