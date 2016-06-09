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
using System.Runtime.InteropServices;
namespace DevExpress.Pdf.Interop {
	[StructLayout(LayoutKind.Sequential)]
	struct GCP_RESULTS {
		int structSize;
		[MarshalAs(UnmanagedType.LPTStr)]
		string outString;
		IntPtr order;
		IntPtr dx;
		IntPtr caretPos;
		IntPtr cls;
		IntPtr glyphs;
		uint glyphCount;
		int maxFit;
		public int StructSize {
			get { return structSize; }
			set { structSize = value; }
		}
		public string OutString {
			get { return outString; }
			set { outString = value; }
		}
		public IntPtr Order {
			get { return order; }
			set { order = value; }
		}
		public IntPtr Dx {
			get { return dx; }
			set { dx = value; }
		}
		public IntPtr CaretPos {
			get { return caretPos; }
			set { caretPos = value; }
		}
		public IntPtr Cls {
			get { return cls; }
			set { cls = value; }
		}
		public IntPtr Glyphs {
			get { return glyphs; }
			set { glyphs = value; }
		}
		public uint GlyphCount {
			get { return glyphCount; }
			set { glyphCount = value; }
		}
		public int MaxFit {
			get { return maxFit; }
			set { maxFit = value; }
		}
	}
}
