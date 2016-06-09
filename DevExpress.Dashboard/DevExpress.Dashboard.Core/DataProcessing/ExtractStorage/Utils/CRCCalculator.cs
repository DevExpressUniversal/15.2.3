#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DevExpress.DashboardCommon.DataProcessing {
	[CLSCompliant(false)]
	public static class CRCAlgorithm {
		static uint[] crcTable = new uint[256];
		static bool crcTableComputed;
		public static void MakeCRCTable() {
			uint reg;
			for (int i = 0; i < 256; i++) {
				reg = (uint)i;
				for (int j = 0; j < 8; j++) {
					if ((reg & 1) != 0)
						reg = 0xedb88320 ^ (reg >> 1);
					else
						reg = reg >> 1;
				}
				crcTable[i] = reg;
			}
			crcTableComputed = true;
		}
		static public uint GetCRC(byte[] buf) {
			uint c = 0xffffffff;
			int i = 0;
			if (!crcTableComputed)
				MakeCRCTable();
			for (int j = 0; j < buf.Length; j++) {
				c = crcTable[(c ^ (byte)buf[i]) & 0xff] ^ (c >> 8);
				i++;
			}
			return c ^ 0xffffffff;
		}
		static public uint GetCRC(ByteBuffer buf) {
			uint c = 0xffffffff;
			int i = 0;
			if (!crcTableComputed)
				MakeCRCTable();
			for (int j = 0; j < buf.Count; j++) {
				c = crcTable[(c ^ buf[i]) & 0xff] ^ (c >> 8);
				i++;
			}
			return c ^ 0xffffffff;
		}
	}
}
