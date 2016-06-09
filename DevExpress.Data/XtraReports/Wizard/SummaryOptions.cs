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

#if SL
using DevExpress.Xpf.Collection.Specialized;
#else
using System.Collections.Specialized;
#endif
namespace DevExpress.Data.XtraReports.Wizard {
	public class SummaryOptions {
		#region Fields & Properties
		BitVector32 vector;
		public SummaryOptionFlags Flags {
			get {
				return (SummaryOptionFlags)vector.Data;
			}
		}
		public bool Sum {
			get {
				return vector[(int)SummaryOptionFlags.Sum];
			}
			set {
				vector[(int)SummaryOptionFlags.Sum] = value;
			}
		}
		public bool Avg {
			get {
				return vector[(int)SummaryOptionFlags.Avg];
			}
			set {
				vector[(int)SummaryOptionFlags.Avg] = value;
			}
		}
		public bool Min {
			get {
				return vector[(int)SummaryOptionFlags.Min];
			}
			set {
				vector[(int)SummaryOptionFlags.Min] = value;
			}
		}
		public bool Max {
			get {
				return vector[(int)SummaryOptionFlags.Max];
			}
			set {
				vector[(int)SummaryOptionFlags.Max] = value;
			}
		}
		public bool Count {
			get {
				return vector[(int)SummaryOptionFlags.Count];
			}
			set {
				vector[(int)SummaryOptionFlags.Count] = value;
			}
		}
		#endregion
		public SummaryOptions(SummaryOptionFlags flags) {
			vector = new BitVector32((int)flags);
		}
	}
}
