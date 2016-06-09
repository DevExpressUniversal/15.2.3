#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using DevExpress.Pdf.Native;
using System.Collections.Generic;
namespace DevExpress.Pdf {
	public class PdfRunLengthDecodeFilter : PdfFilter {
		internal const string Name = "RunLengthDecode";
		internal const string ShortName = "RL";
		protected internal override string FilterName { get { return Name; } }
		internal PdfRunLengthDecodeFilter() {
		}
		protected internal override byte[] Decode(byte[] data) {
			List<byte> result = new List<byte>();
			int state = 1;
			int count = 0;
			foreach (byte b in data) {
				switch (state) {
					case 1:
						if (b == 128)
							return result.ToArray();
						if (b <= 127) {
							state = 2;
							count = b + 1;
						}
						else {
							state = 3;
							count = 257 - b;
						}
						break;
					case 2:
						count--;
						result.Add(b);
						if (count == 0)
							state = 1;
						break;
					case 3:
						state = 1;
						for (int i = 0; i < count; i++)
							result.Add(b);
						break;
				}
			}
			return result.ToArray();
		}
	}
}
