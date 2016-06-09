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
using System.IO;
using System.Text;
namespace DevExpress.Office.Utils {
	#region ChunkedStringBuilderWriter
	public class ChunkedStringBuilderWriter : TextWriter {
		static readonly Encoding unicodeEncoding = new UnicodeEncoding(false, false);
		readonly ChunkedStringBuilder stringBuilder;
		Encoding encoding = unicodeEncoding;
		bool isOpen = true;
		public ChunkedStringBuilderWriter(ChunkedStringBuilder stringBuilder) {
			this.stringBuilder = stringBuilder;
		}
		public override Encoding Encoding { get { return encoding; } }
		public void SetEncoding(Encoding encoding) {
			this.encoding = encoding;
		}
		protected override void Dispose(bool disposing) {
			isOpen = false;
			base.Dispose(disposing);
		}
#if !DXRESTRICTED
		public override void Close() {
			Dispose(true);
		}
#endif
		public virtual ChunkedStringBuilder GetStringBuilder() {
			return stringBuilder;
		}
		public override string ToString() {
			return stringBuilder.ToString();
		}
		public override void Write(char value) {
			if (isOpen)
				stringBuilder.Append(value);
			else
				ThrowWriterClosedException();
		}
		public override void Write(string value) {
			if (isOpen)
				stringBuilder.Append(value);
			else
				ThrowWriterClosedException();
		}
		public override void Write(char[] buffer, int index, int count) {
			if (isOpen)
				stringBuilder.Append(buffer, index, count);
			else
				ThrowWriterClosedException();
		}
		void ThrowWriterClosedException() {
			Exceptions.ThrowInvalidOperationException("writer is closed");
		}
	}
#endregion
}
