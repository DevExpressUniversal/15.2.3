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
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using DevExpress.Utils.Design;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Design {
	#region PaintStyleNameConverter
	public class PaintStyleNameConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SchedulerControl control = context.Instance as SchedulerControl;
			StringCollection names = new StringCollection();
			names.Add(SchedulerControl.DefaultPaintStyleName);
			if (control != null) {
				foreach (SchedulerPaintStyle paintStyle in control.PaintStyles) 
					names.Add(paintStyle.Name);
			}
			return new StandardValuesCollection(names);
		}
	}
	#endregion
	#region TimeScaleCollectionEditor
	public class TimeScaleCollectionEditor : DXCollectionEditor {
		public TimeScaleCollectionEditor(Type type) : base(type) {
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(TimeScaleFixedInterval) };
		}
	}
	#endregion
}
