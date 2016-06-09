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

using System.Windows.Markup;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Style")]
	public class StatedStyleSelectorState {
		public BorderState State { get; set; }
		public bool IsDefault { get; set; }
		private Style style;
		public Style Style {
			get { return style; }
			set {
				if (value == style) return;
				style = value;
				style.Do(x => x.Seal());
			}
		}	 
	}
	[ContentProperty("States")]
	public class StatedStyleSelector {
		public Style SelectStyle(BorderState state) {
			StatedStyleSelectorState result = null;
			foreach(StatedStyleSelectorState st in States) {
				if(st.IsDefault && result == null) {
					result = st;
				} else if(st.State == state) {
					result = st;
				}
			}
			if(result == null)
				return null;
			else
				return result.Style;
		}
		public ObservableCollectionCore<StatedStyleSelectorState> States { get; protected set; }
		public StatedStyleSelector() {
			States = new ObservableCollectionCore<StatedStyleSelectorState>();			
		}
	}
}
