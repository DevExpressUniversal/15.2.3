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
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region HexContentDestination (abstract class)
	public abstract class HexContentDestination : DestinationBase {
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		protected HexContentDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		#region Read hex data
		int val;
		bool firstPosition = true;
		protected override void ProcessCharCore(char ch) {
			if (ch == ' ')
				return;
			int hex = RtfImporter.HexToInt(ch);
			if (firstPosition)
				val = hex << 4;
			else {
				val += hex;
				ProcessBinChar((char)val);
				val = 0;
			}
			firstPosition = !firstPosition;
		}
		#endregion
	}
	#endregion
	#region HexByteArrayDestination (abstract class)
	public abstract class HexByteArrayDestination : HexContentDestination {
		List<byte> value = new List<byte>();
		protected HexByteArrayDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		public List<byte> Value { get { return value; } }
		protected override DestinationBase CreateClone() {
			HexByteArrayDestination clone = CreateEmptyClone();
			clone.value = value;
			return clone;
		}
		protected override void ProcessBinCharCore(char ch) {
			value.Add((byte)ch);
		}
		protected internal abstract HexByteArrayDestination CreateEmptyClone();
	}
	#endregion
	#region HexStreamDestination (abstract class)
	public abstract class HexStreamDestination : HexContentDestination {
		MemoryStream value = new MemoryStream();
		protected HexStreamDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		public MemoryStream Value { get { return value; } }
		protected override DestinationBase CreateClone() {
			HexStreamDestination clone = CreateEmptyClone();
			clone.value = value;
			return clone;
		}
		protected override void ProcessBinCharCore(char ch) {
			value.WriteByte((byte)ch);
		}
		protected internal abstract HexStreamDestination CreateEmptyClone();
	}
	#endregion
}
