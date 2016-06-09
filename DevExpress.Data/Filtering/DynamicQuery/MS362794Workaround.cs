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

using DevExpress.Compatibility.System;
using System;
namespace DevExpress.Data.Linq.Helpers {
	public static class SummaryWorkaroundForMS362794 {
		public static readonly Type[] TypesArray = new Type[] {null,
			typeof(MS362794_C1<>),
			typeof(MS362794_C2<,>),
			typeof(MS362794_C3<,,>),
			typeof(MS362794_C4<,,,>),
			typeof(MS362794_C5<,,,,>),
			typeof(MS362794_C6<,,,,,>),
			typeof(MS362794_C7<,,,,,,>),
			typeof(MS362794_C8<,,,,,,,>),
			typeof(MS362794_C9<,,,,,,,,>),
			typeof(MS362794_C10<,,,,,,,,,>),
			typeof(MS362794_C11<,,,,,,,,,,>),
			typeof(MS362794_C12<,,,,,,,,,,,>),
			typeof(MS362794_C13<,,,,,,,,,,,,>),
			typeof(MS362794_C14<,,,,,,,,,,,,,>),
			typeof(MS362794_C15<,,,,,,,,,,,,,,>),
			typeof(MS362794_C16<,,,,,,,,,,,,,,,>),
			typeof(MS362794_C17<,,,,,,,,,,,,,,,,>),
		};
	}
	[Serializable]
	public abstract class MS362794 {
		public readonly object[] Container;
		protected MS362794(int size) { Container = new object[size]; }
	}
	public class MS362794_C1<T0>: MS362794 {
		public MS362794_C1() : base(1) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
	}
	public class MS362794_C2<T0, T1>: MS362794 {
		public MS362794_C2() : base(2) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
	}
	public class MS362794_C3<T0, T1, T2>: MS362794 {
		public MS362794_C3() : base(3) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
	}
	public class MS362794_C4<T0, T1, T2, T3>: MS362794 {
		public MS362794_C4() : base(4) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
	}
	public class MS362794_C5<T0, T1, T2, T3, T4>: MS362794 {
		public MS362794_C5() : base(5) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
	}
	public class MS362794_C6<T0, T1, T2, T3, T4, T5>: MS362794 {
		public MS362794_C6() : base(6) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
	}
	public class MS362794_C7<T0, T1, T2, T3, T4, T5, T6>: MS362794 {
		public MS362794_C7() : base(7) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
	}
	public class MS362794_C8<T0, T1, T2, T3, T4, T5, T6, T7>: MS362794 {
		public MS362794_C8() : base(8) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
	}
	public class MS362794_C9<T0, T1, T2, T3, T4, T5, T6, T7, T8>: MS362794 {
		public MS362794_C9() : base(9) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
	}
	public class MS362794_C10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>: MS362794 {
		public MS362794_C10() : base(10) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
	}
	public class MS362794_C11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>: MS362794 {
		public MS362794_C11() : base(11) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
	}
	public class MS362794_C12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>: MS362794 {
		public MS362794_C12() : base(12) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
	}
	public class MS362794_C13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>: MS362794 {
		public MS362794_C13() : base(13) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
		public T12 P12 { get { return (T12)Container[12]; } set { Container[12] = value; } }
	}
	public class MS362794_C14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>: MS362794 {
		public MS362794_C14() : base(14) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
		public T12 P12 { get { return (T12)Container[12]; } set { Container[12] = value; } }
		public T13 P13 { get { return (T13)Container[13]; } set { Container[13] = value; } }
	}
	public class MS362794_C15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>: MS362794 {
		public MS362794_C15() : base(15) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
		public T12 P12 { get { return (T12)Container[12]; } set { Container[12] = value; } }
		public T13 P13 { get { return (T13)Container[13]; } set { Container[13] = value; } }
		public T14 P14 { get { return (T14)Container[14]; } set { Container[14] = value; } }
	}
	public class MS362794_C16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>: MS362794 {
		public MS362794_C16() : base(16) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
		public T12 P12 { get { return (T12)Container[12]; } set { Container[12] = value; } }
		public T13 P13 { get { return (T13)Container[13]; } set { Container[13] = value; } }
		public T14 P14 { get { return (T14)Container[14]; } set { Container[14] = value; } }
		public T15 P15 { get { return (T15)Container[15]; } set { Container[15] = value; } }
	}
	public class MS362794_C17<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>: MS362794 {
		public MS362794_C17() : base(17) { }
		public T0 P0 { get { return (T0)Container[0]; } set { Container[0] = value; } }
		public T1 P1 { get { return (T1)Container[1]; } set { Container[1] = value; } }
		public T2 P2 { get { return (T2)Container[2]; } set { Container[2] = value; } }
		public T3 P3 { get { return (T3)Container[3]; } set { Container[3] = value; } }
		public T4 P4 { get { return (T4)Container[4]; } set { Container[4] = value; } }
		public T5 P5 { get { return (T5)Container[5]; } set { Container[5] = value; } }
		public T6 P6 { get { return (T6)Container[6]; } set { Container[6] = value; } }
		public T7 P7 { get { return (T7)Container[7]; } set { Container[7] = value; } }
		public T8 P8 { get { return (T8)Container[8]; } set { Container[8] = value; } }
		public T9 P9 { get { return (T9)Container[9]; } set { Container[9] = value; } }
		public T10 P10 { get { return (T10)Container[10]; } set { Container[10] = value; } }
		public T11 P11 { get { return (T11)Container[11]; } set { Container[11] = value; } }
		public T12 P12 { get { return (T12)Container[12]; } set { Container[12] = value; } }
		public T13 P13 { get { return (T13)Container[13]; } set { Container[13] = value; } }
		public T14 P14 { get { return (T14)Container[14]; } set { Container[14] = value; } }
		public T15 P15 { get { return (T15)Container[15]; } set { Container[15] = value; } }
		public T16 P16 { get { return (T16)Container[16]; } set { Container[16] = value; } }
	}
}
