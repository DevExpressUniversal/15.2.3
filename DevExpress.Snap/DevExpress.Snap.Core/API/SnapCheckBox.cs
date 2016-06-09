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
	using System.Windows.Forms;
	public interface SnapCheckBox : SnapSingleListItemEntity {
		CheckState State { get; set; }				  
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Snap.Core.API;
	using System.Windows.Forms;
	using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
	using DevExpress.Snap.Core.Fields;
	public class NativeSnapCheckBox : NativeSnapSingleListItemEntity, SnapCheckBox {
		CheckState state;
		public NativeSnapCheckBox(SnapNativeDocument document, ApiField field) : base(document, field) { }
		public NativeSnapCheckBox(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNCheckBoxField parsedField = GetParsedField<SNCheckBoxField>();
			this.state = parsedField.CheckState;
		}
		#region SnapCheckBox Members
		public CheckState State {
			get {
				return state;
			}
			set {
				EnsureUpdateBegan();
				if(state == value)
					return;
				Controller.SetSwitch(SNCheckBoxField.CheckStateSwitch, SNCheckBoxField.checkStateDictionary[value]);
				state = value;
			}
		}
		#endregion
	}
}
