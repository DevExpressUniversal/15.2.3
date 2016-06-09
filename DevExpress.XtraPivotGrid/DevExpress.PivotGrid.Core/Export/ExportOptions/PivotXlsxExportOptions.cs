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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.PivotGrid.Export;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public abstract class PivotXlsxExportOptionsBase : XlsxExportOptionsEx, IPivotGridExportOptions {
		DefaultBoolean exportRowAreaHeaders;
		DefaultBoolean exportColumnAreaHeaders;
		DefaultBoolean exportDataAreaHeaders;
		DefaultBoolean exportFilterAreaHeaders;
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ExportRowAreaHeaders { get { return exportRowAreaHeaders; } set { exportRowAreaHeaders = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ExportColumnAreaHeaders { get { return exportColumnAreaHeaders; } set { exportColumnAreaHeaders = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ExportDataAreaHeaders { get { return exportDataAreaHeaders; } set { exportDataAreaHeaders = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ExportFilterAreaHeaders { get { return exportFilterAreaHeaders; } set { exportFilterAreaHeaders = value; } }
		protected PivotXlsxExportOptionsBase() {
			Init();
		}
		protected PivotXlsxExportOptionsBase(TextExportMode mode)
			: base(mode) {
			Init();
		}
		void Init() {
			ExportRowAreaHeaders = DefaultBoolean.Default;
			ExportColumnAreaHeaders = DefaultBoolean.Default;
			ExportDataAreaHeaders = DefaultBoolean.Default;
			ExportFilterAreaHeaders = DefaultBoolean.Default;
		}
		protected abstract void RaiseCustomizeCellEvent(CustomizePivotCellEventArgsCore e);
		void IPivotGridExportOptions.RaiseCustomizeCellEvent(CustomizePivotCellEventArgsCore e) {
			RaiseCustomizeCellEvent(e);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			PivotXlsxExportOptionsBase options = source as PivotXlsxExportOptionsBase;
			if(options != null) {
				ExportRowAreaHeaders = options.ExportRowAreaHeaders;
				ExportColumnAreaHeaders = options.ExportColumnAreaHeaders;
				ExportDataAreaHeaders = options.ExportDataAreaHeaders;
				ExportFilterAreaHeaders = options.ExportFilterAreaHeaders;
			}
		}
	}
}
