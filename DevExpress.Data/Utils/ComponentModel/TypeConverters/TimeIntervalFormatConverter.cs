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
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
namespace DevExpress.Utils.Design {
	public class TimeIntervalFormatConverter : DateTimeFormatConverter {
		const string DefaultTimeIntervalPattern = "{0:D} - {1:D}";
		const string TimeIntervalStartFormatPattern = "{{0:{0}}}";
		const string TimeIntervalEndFormatPattern = "{{1:{0}}}";
		const string TimeIntervalFormatPattern = TimeIntervalStartFormatPattern + " - " + TimeIntervalEndFormatPattern;
		protected override string DefaultString { get { return DefaultTimeIntervalPattern; } }
		protected internal override StringCollection GetDateTimeFormats(ITypeDescriptorContext context) {
			StringCollection formats = new StringCollection();
			if(context != null) {
				string[] formatInfos = new DateTimeFormatInfo().GetAllDateTimePatterns();
				for(int i = 0; i < formatInfos.Length; i++) {
					string info = formatInfos[i];
					formats.Add(BuildTimeIntervalFormat(TimeIntervalStartFormatPattern, info));
					formats.Add(BuildTimeIntervalFormat(TimeIntervalEndFormatPattern, info));
					formats.Add(BuildTimeIntervalFormat(TimeIntervalFormatPattern, info));
				}
			}
			return formats;
		}
		protected virtual string BuildTimeIntervalFormat(string pattern, string formatInfo) {
			return String.Format(pattern, formatInfo);
		}
	}
}
