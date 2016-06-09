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
using DevExpress.Utils.Controls;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraGauges.Core.Printing;
namespace DevExpress.XtraGauges.Win.Printing {
	public class GaugeOptionsPrint : BaseOptions, IXtraSupportShouldSerialize {
		const PrintSizeMode defaultSizeMode = PrintSizeMode.None;
		PrintSizeMode sizeModeCore;
		GaugeControl gaugeControlCore;
		internal GaugeOptionsPrint(GaugeControl gaugeControl) {
			this.sizeModeCore = defaultSizeMode;
			this.gaugeControlCore = gaugeControl;
		}
		public PrintSizeMode SizeMode {
			get { return sizeModeCore; }
			set {
				if(sizeModeCore == value) return;
				sizeModeCore = value;
				UpdatePrinterSizeMode();
			}
		}
		#region XtraSerializing
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			if(propertyName == "SizeMode") return ShouldSerializeSizeMode();
			return true;
		}
		#endregion
		#region ShouldSerialize
		bool ShouldSerializeSizeMode() {
			return SizeMode != defaultSizeMode;
		}
		internal new bool ShouldSerialize() {
			return ShouldSerializeSizeMode();
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			GaugeOptionsPrint printOptions = options as GaugeOptionsPrint;
			if(printOptions != null) {
				SizeMode = printOptions.SizeMode;
			}
		}
		public override string ToString() {
			return "(OptionsPrint)";
		}
		void UpdatePrinterSizeMode() {
			if (gaugeControlCore != null && gaugeControlCore.Printer != null) 
				gaugeControlCore.Printer.SizeMode = sizeModeCore;
		}
	}
}
