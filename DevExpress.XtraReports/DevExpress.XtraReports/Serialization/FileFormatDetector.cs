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
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Serialization {
	public class FileFormatDetector {
		public static FileFormatDetector CreateXmlDetector() {
			return new FileFormatDetector("<?xml", 30);
		}
		public static FileFormatDetector CreateSoapDetector() {
			return new FileFormatDetector(@"<SOAP-ENV:ENVELOPE", 1000);
		}
		string prefix;
		int headerLength;
		public FileFormatDetector(string prefix, int headerLength) {
			this.prefix = prefix;
			this.headerLength = headerLength;
		}
		public bool FormatExists(byte[] bytes) {
			return FormatExistsCore(BytesToString(bytes));
		}
		string BytesToString(byte[] bytes) {
			return Encoding.UTF8.GetString(bytes, 0, ValidateLength(bytes.Length));
		}
		public bool FormatExists(Stream stream) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			byte[] bytes = new byte[headerLength];
			stream.Read(bytes, 0, ValidateLength(stream.Length));
			return FormatExists(bytes);
		}
		public bool FormatExists(string s) {
			return FormatExistsCore(s.Substring(0, ValidateLength(s.Length)));
		}
		int ValidateLength(long value) {
			return (int)Math.Min(headerLength, value);
		}
		bool FormatExistsCore(string s) {
			return s.IndexOf(prefix, 0, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
