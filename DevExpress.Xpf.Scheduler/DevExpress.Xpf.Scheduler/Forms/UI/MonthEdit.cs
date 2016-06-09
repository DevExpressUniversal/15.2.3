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
using DevExpress.Xpf.Scheduler.UI;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Drawing;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]	
	public class MonthEdit : FixedSourceComboBoxEdit {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Month {
			get {
				object val = base.EditValue;
				if(IsNullValue(val))
					return 1; 
				else
					return (int)val;
			}
			set {
				base.EditValue = value;
			}
		}	 
		protected virtual bool IsNullValue(object editValue) {
			return ((editValue == null) || (editValue is DBNull));
		}
		protected override Editors.Settings.BaseEditSettings CreateEditorSettings() {
			return new MonthEditSettings();
		}
	}
	public class MonthEditSettings : FixedSourceComboBoxEditSettings {
		protected override void PopulateItems() {
			Items.Clear();
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			for (int i = 1; i <= 12; i++)
				Items.Add(new NamedElement(i, dtfi.GetMonthName(i)));
		}
	}
}
