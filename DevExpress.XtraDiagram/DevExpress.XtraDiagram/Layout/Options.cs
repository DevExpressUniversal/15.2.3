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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraDiagram.Layout {
	public class DiagramOptionsLayout : OptionsLayoutBase {
		bool storeAllOptions;
		public DiagramOptionsLayout() {
			this.storeAllOptions = true;
		}
		[DefaultValue(true), NotifyParentProperty(true)]
		public bool StoreAllOptions {
			get { return storeAllOptions; }
			set {
				if(StoreAllOptions == value) return;
				bool prevValue = StoreAllOptions;
				storeAllOptions = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAllOptions", prevValue, StoreAllOptions));
			}
		}
		public override void Assign(BaseOptions options) {
			DiagramOptionsLayout other = (DiagramOptionsLayout)options;
			BeginUpdate();
			try {
				base.Assign(options);
				this.storeAllOptions = other.StoreAllOptions;
			}
			finally {
				EndUpdate();
			}
		}
		public override void Reset() {
			BeginUpdate();
			try {
				base.Reset();
			}
			finally {
				EndUpdate();
			}
		}
	}
}
