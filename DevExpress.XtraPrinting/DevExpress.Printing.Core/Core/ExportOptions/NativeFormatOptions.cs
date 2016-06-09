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
using DevExpress.Data;
namespace DevExpress.XtraPrinting {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.NativeFormatOptions")]
	public class NativeFormatOptions : ExportOptionsBase {
		const bool CompressedDefaultValue = true;
		const bool ShowOptionsBeforeSaveDefaultValue = false;
		bool compressed = CompressedDefaultValue;
		bool showOptionsBeforeSave = ShowOptionsBeforeSaveDefaultValue;
		protected internal override bool IsMultiplePaged {
			get { return true; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("NativeFormatOptionsCompressed"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.NativeFormatOptions.Compressed"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(CompressedDefaultValue),
		XtraSerializableProperty,
		]
		public bool Compressed { get { return compressed; } set { compressed = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("NativeFormatOptionsShowOptionsBeforeSave"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.NativeFormatOptions.ShowOptionsBeforeSave"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(ShowOptionsBeforeSaveDefaultValue),
		XtraSerializableProperty,
		]
		public bool ShowOptionsBeforeSave { get { return showOptionsBeforeSave; } set { showOptionsBeforeSave = value; } }
		protected internal override bool UseActionAfterExportAndSaveModeValue { get { return false; } }
		public NativeFormatOptions() { }
		NativeFormatOptions(NativeFormatOptions source)
			: base(source) {
		}
		protected internal override ExportOptionsBase CloneOptions() {
			return new NativeFormatOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			NativeFormatOptions nativeFormatSource = (NativeFormatOptions)source;
			Compressed = nativeFormatSource.Compressed;
			ShowOptionsBeforeSave = nativeFormatSource.ShowOptionsBeforeSave;
		}
		protected internal override bool GetShowOptionsBeforeExport(bool defaultValue) {
			return ShowOptionsBeforeSave;
		}
		protected internal override bool ShouldSerialize() {
			return compressed != CompressedDefaultValue || showOptionsBeforeSave != ShowOptionsBeforeSaveDefaultValue;
		}
	}
}
