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

using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
namespace DevExpress.Snap.Core.API {
	using System;
	using DevExpress.XtraPrinting;
	using DevExpress.XtraPrinting.BarCode;
	public interface SnapBarCode : SnapSingleListItemEntity {
		string Data { get; set; }					   
		bool ShowData { get; set; }					 
		int Module { get; set; }						
		bool AutoModule { get; set; }				   
		BarCodeOrientation Orientation { get; set; }	
		TextAlignment Alignment { get; set; }		   
		TextAlignment TextAlignment { get; set; }	   
		BarCodeGeneratorBase GetGenerator();
		void SetGenerator(BarCodeGeneratorBase value);
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using DevExpress.Snap.Core;
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.Snap.Core.Native;
	using DevExpress.XtraPrinting;
	using DevExpress.XtraPrinting.BarCode;
	public class NativeSnapBarCode : NativeSnapSingleListItemEntity, SnapBarCode {
		string data;
		bool showData;
		int module;
		bool autoModule;
		BarCodeOrientation orientation;
		TextAlignment alignment;
		TextAlignment textAlignment;
		BarCodeGeneratorBase generator;
		public NativeSnapBarCode(SnapNativeDocument document, ApiField field) : base(document, field) { }
		public NativeSnapBarCode(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNBarCodeField parsedField = GetParsedField<SNBarCodeField>();
			this.data = parsedField.Data;
			if (this.data == null) this.data = String.Empty;
			this.showData = parsedField.ShowText;
			this.module = parsedField.Module;
			this.autoModule = parsedField.AutoModule;
			this.orientation = parsedField.Orientation;
			this.alignment = parsedField.Alignment;
			this.textAlignment = parsedField.TextAlignment;
			this.generator = null;
		}
		#region SnapBarCode Members
		public string Data {
			get {
				return data;
			}
			set {
				EnsureUpdateBegan();
				if(String.Equals(data, value))
					return;
				Controller.SetSwitch(SNBarCodeField.BarCodeDataSwitch, value);
				data = value;
			}
		}
		public bool ShowData {
			get {
				return showData;
			}
			set {
				EnsureUpdateBegan();
				if(showData == value)
					return;
				Controller.SetSwitch(SNBarCodeField.BarCodeShowTextSwitch, Convert.ToString(value));
				showData = value;
			}
		}
		public int Module {
			get {
				return module;
			}
			set {
				EnsureUpdateBegan();
				if(module == value)
					return;
				if(value == BarCodeRunObject.DefaultModule)
					Controller.RemoveSwitch(SNBarCodeField.BarCodeModuleSwitch);
				else
					Controller.SetSwitch(SNBarCodeField.BarCodeModuleSwitch, Convert.ToString(value));
				module = value;
			}
		}
		public bool AutoModule {
			get {
				return autoModule;
			}
			set {
				EnsureUpdateBegan();
				if(autoModule == value)
					return;
				Controller.SetSwitch(SNBarCodeField.BarCodeAutoModuleSwitch, Convert.ToString(value));
				autoModule = value;
			}
		}
		public BarCodeOrientation Orientation {
			get {
				return orientation;
			}
			set {
				EnsureUpdateBegan();
				if(orientation == value)
					return;
				if(value == BarCodeRunObject.DefaultOrientation)
					Controller.RemoveSwitch(SNBarCodeField.BarCodeOrientationSwitch);
				else
					Controller.SetSwitch(SNBarCodeField.BarCodeOrientationSwitch, Convert.ToString(value));
				orientation = value;
			}
		}
		public TextAlignment Alignment {
			get {
				return alignment;
			}
			set {
				EnsureUpdateBegan();
				if(alignment == value)
					return;
				Controller.SetSwitch(SNBarCodeField.BarCodeAlignmentSwitch, Convert.ToString(value));
				alignment = value;
			}
		}
		public TextAlignment TextAlignment {
			get {
				return textAlignment;
			}
			set {
				EnsureUpdateBegan();
				if(textAlignment == value)
					return;
				Controller.SetSwitch(SNBarCodeField.BarCodeTextAlignmentSwitch, Convert.ToString(value));
				textAlignment = value;
			}
		}
		public BarCodeGeneratorBase GetGenerator() {
			if(this.generator == null)
				return CloneGenerator(GetParsedField<SNBarCodeField>().BarCodeGenerator);
			return CloneGenerator(generator);
		}
		public void SetGenerator(BarCodeGeneratorBase value) {
			EnsureUpdateBegan();
			this.generator = CloneGenerator(value);
			Controller.SetSwitch(SNBarCodeField.BarCodeGeneratorSwitch, SNBarCodeHelper.GetGeneratorBase64String(this.generator));
		}
		static BarCodeGeneratorBase CloneGenerator(BarCodeGeneratorBase generator) { return ((ICloneable)generator).Clone() as BarCodeGeneratorBase; }
		#endregion
	}
}
