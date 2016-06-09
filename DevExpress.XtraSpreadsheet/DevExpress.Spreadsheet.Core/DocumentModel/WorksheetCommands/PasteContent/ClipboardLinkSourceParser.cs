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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public struct ClipboardLinkParsedInfo {
		public string FilePath { get; set; }
		public string RangeReference { get; set; }
	}
	public static class ClipboardLinkSourceParser {
		public static ClipboardLinkParsedInfo Parse(byte[] linkSourceBytes) {
			ClipboardLinkParsedInfo result = new ClipboardLinkParsedInfo();
			if (linkSourceBytes.Length == 0)
				return result;
			const int startOfSizeDosFilePath = 38;
			const int startOfDosFilePath = startOfSizeDosFilePath + 4;
			int sizeDosFilePath = BitConverter.ToInt32(linkSourceBytes, startOfSizeDosFilePath);
			int startOfSizeFilePath = startOfDosFilePath + sizeDosFilePath + 24;
			int sizeFilePathData = BitConverter.ToInt32(linkSourceBytes, startOfSizeFilePath);
			int startOfClsid3 = startOfSizeFilePath +4  + sizeFilePathData;
			if (sizeFilePathData == 0) {
				result.FilePath = String.Empty;
			}
			else {
				int innerStartOfSizeFilePathData = startOfSizeFilePath + 4;
				int innerSizeFilePathData = BitConverter.ToInt32(linkSourceBytes, innerStartOfSizeFilePathData);
				int startOfFilePathData = innerStartOfSizeFilePathData + 2 + 4;
				result.FilePath = ExtractString(linkSourceBytes, startOfFilePathData, innerSizeFilePathData, Encoding.Unicode, false);
			}
			int startOfSizeRcReference = startOfClsid3 + 16 + 6;
			int sizeRcReference = BitConverter.ToInt32(linkSourceBytes, startOfSizeRcReference);
			int startOfRcReference = startOfSizeRcReference + 4;
			result.RangeReference = ExtractString(linkSourceBytes, startOfRcReference, sizeRcReference, Encoding.ASCII, true);
			return result;
		}
		static string ExtractString(byte[] bytes, int start, int size, Encoding encoding, bool whithoutTerminatedNull) {
			int correctedSize = size;
			if (whithoutTerminatedNull)
				correctedSize = size - 1;
			byte[] resultBytes = new byte[correctedSize];
			Array.Copy(bytes, start, resultBytes, 0, correctedSize);
			return encoding.GetString(resultBytes, 0, correctedSize);
		}
	}
}
