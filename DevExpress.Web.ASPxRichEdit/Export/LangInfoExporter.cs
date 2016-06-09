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
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class LangInfoExporter {
		public static Hashtable ToHashtable(DevExpress.XtraRichEdit.Model.LangInfo info) {
			Hashtable result = new Hashtable();
			result.Add("0", info.Latin != null ? info.Latin.ToString() : null);
			result.Add("1", info.Bidi != null ? info.Bidi.ToString() : null);
			result.Add("2", info.EastAsia != null ? info.EastAsia.ToString() : null);
			return result;
		}
		public static DevExpress.XtraRichEdit.Model.LangInfo FromHashtable(Hashtable source) {
			string latinString = (string)source[((int)JSONLangInfoProperty.Latin).ToString()];
			string bidiString = (string)source[((int)JSONLangInfoProperty.Bidi).ToString()];
			string eastAsiaString = (string)source[((int)JSONLangInfoProperty.EastAsia).ToString()];
			CultureInfo latin = !string.IsNullOrEmpty(latinString) ? new CultureInfo(latinString) : null;
			CultureInfo bidi = !string.IsNullOrEmpty(bidiString) ? new CultureInfo(bidiString) : null;
			CultureInfo eastAsia = !string.IsNullOrEmpty(eastAsiaString) ? new CultureInfo(eastAsiaString) : null;
			return new LangInfo(latin, bidi, eastAsia);
		}
	}
}
