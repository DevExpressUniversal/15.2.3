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
using System.Linq;
using System.Text;
namespace DevExpress.XtraDiagram.Utils {
	public static class MayBe {
		public static TR Select<TI, TR>(this TI input, Func<TI, TR> evaluator)
			where TI : class
			where TR : class {
			if(input == null)
				return null;
			return evaluator(input);
		}
		public static TI DoIfNotNull<TI>(this TI input, Action<TI> action) where TI : class {
			if(input == null)
				return null;
			action(input);
			return input;
		}
		public static void ForEach<TI>(this IEnumerable<TI> items, Action<TI> action) {
			foreach(var item in items) {
				action(item);
			}
		}
		public static TI DoIfTrue<TI>(this TI input, Func<TI, bool> condition, Action<TI> action) where TI : class {
			if(input == null) return null;
			if(condition(input))
				action(input);
			return input;
		}
		public static TR Return<TI, TR>(this TI input, Func<TI, TR> evaluator, Func<TR> fallback)
			where TI : class
			where TR : struct {
			if(input == null)
				return fallback();
			return evaluator(input);
		}
	}
}
