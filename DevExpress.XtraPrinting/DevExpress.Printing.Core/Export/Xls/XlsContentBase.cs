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
using System.IO;
using System.Text;
namespace DevExpress.XtraExport.Xls {
	#region IXlsContent
	public interface IXlsContent {
		void Read(XlsReader reader, int size);
		void Write(BinaryWriter writer);
		int GetSize();
		FutureRecordHeaderBase RecordHeader { get; }
	}
	#endregion
	#region XlsContentBase
	public abstract class XlsContentBase : IXlsContent {
		const string invalidFileMessage = "Invalid Xls file";
		public abstract void Read(XlsReader reader, int size);
		public abstract void Write(BinaryWriter writer);
		public abstract int GetSize();
		public virtual FutureRecordHeaderBase RecordHeader { get { return null; } }
		protected void ThrowInvalidXlsFile() {
			throw new Exception(invalidFileMessage);
		}
		protected void ThrowInvalidXlsFile(string reason) {
			throw new Exception(string.Format("{0}: {1}", invalidFileMessage, reason));
		}
		protected void CheckValue(int value, int minValue, int maxValue) {
			if(value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format("Value out of range {0}...{1}", minValue, maxValue));
		}
		protected void CheckValue(int value, int minValue, int maxValue, string name) {
			if(value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		protected void CheckValue(long value, long minValue, long maxValue, string name) {
			if(value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		protected void CheckValue(double value, double minValue, double maxValue, string name) {
			if(value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		protected void CheckLength(string value, int maxLength, string name) {
			if(!string.IsNullOrEmpty(value) && value.Length > maxLength)
				throw new ArgumentException(string.Format("{0}: number of characters in this string MUST be less than or equal to {1}", name, maxLength));
		}
		protected int ValueInRange(int value, int minValue, int maxValue) {
			if(value < minValue)
				value = minValue;
			if(value > maxValue)
				value = maxValue;
			return value;
		}
	}
	#endregion
}
