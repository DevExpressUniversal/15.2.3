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

namespace DevExpress.Snap.Core.API {
	using System;
	using System.ComponentModel;
	public interface SnapHyperlink : SnapSingleListItemEntity {
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the DataFieldName property instead.")]
		string FieldName { get; set; }				  
		string ScreenTip { get; set; }				  
		string Target { get; set; }					 
		string DisplayField { get; set; }			   
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Snap.Core.API;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.Snap.Core.Fields;
	using System;
	using System.ComponentModel;
	public class NativeSnapHyperlink : NativeSnapSingleListItemEntity, SnapHyperlink {
		string screenTip;
		string target;
		string displayField;
		public NativeSnapHyperlink(SnapNativeDocument document, Field field) : base(document, field) { }
		public NativeSnapHyperlink(SnapSubDocument subDocument, SnapNativeDocument document, Field field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNHyperlinkField parsedField = GetParsedField<SNHyperlinkField>();
			this.screenTip = parsedField.ScreenTip;
			this.target = parsedField.Target;
			this.displayField = parsedField.DisplayFieldName;
		}
		#region SnapHyperlink Members
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		public string FieldName { get { return DataFieldName; } set { DataFieldName = value; } }
		public string ScreenTip {
			get {
				return screenTip;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(screenTip, value))
					return;
				Controller.SetSwitch(SNHyperlinkField.ScreenTipSwitch, value);
				screenTip = value;
			}
		}
		public string Target {
			get {
				return target;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(target, value))
					return;
				Controller.SetSwitch(SNHyperlinkField.TargetSwitch, value);
				target = value;
			}
		}
		public string DisplayField {
			get {
				return displayField;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(displayField, value))
					return;
				Controller.SetSwitch(SNHyperlinkField.DisplayFieldSwitch, value);
				displayField = value;
			}
		}
		#endregion
	}
}
