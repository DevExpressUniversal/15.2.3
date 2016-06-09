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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraPrinting;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using System.ComponentModel;
using DevExpress.Snap.Localization;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class SNImageActionList : FieldActionList<SNImageField> {
		public SNImageActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ImageSmartTagItem_Sizing")]
		public ImageSizeMode Sizing {
			get {
				return ParsedInfo.Sizing;
			}
			set {
				if (Sizing == value)
					return;
				ApplyNewValue((controller, newSizing) => controller.SetSwitch(SNImageField.ImageSizeModeSwitch, SNImageField.GetImageSizeModeString(newSizing)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ImageSmartTagItem_UpdateMode")]
		public UpdateMergeImageFieldMode UpdateMode {
			get {
				return ParsedInfo.UpdateMode;
			}
			set {
				if (UpdateMode == value)
					return;
				ApplyNewValue(UpdateImageSizeInfo, value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "UpdateMode", "UpdateMode");
			AddPropertyItem(actionItems, "Sizing", "Sizing");
		}
		protected internal void UpdateImageSizeInfo(InstructionController controller, UpdateMergeImageFieldMode newMode) {
			controller.SetSwitch(SNImageField.UpdateModeSwitch, SNImageField.GetUpdateModeString(newMode));
			SNImageFieldController imageController = new SNImageFieldController(controller);
			imageController.SetImageSizeInfo(newMode);
		}
	}
}
