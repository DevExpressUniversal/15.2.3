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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using System.ComponentModel;
using DevExpress.XtraRichEdit;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeRectangularObjectScaleCommand
	public class ChangeRectangularObjectScaleCommand : RectangularObjectCommandBase<int> {
		#region Fields
		int scaleX = 100;
		int scaleY = 100;
		#endregion
		public ChangeRectangularObjectScaleCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeRectangularObjectScaleCommandScaleX")]
#endif
		public int ScaleX { get { return scaleX; } set { scaleX = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeRectangularObjectScaleCommandScaleY")]
#endif
		public int ScaleY { get { return scaleY; } set { scaleY = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeRectangularObjectScaleCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeRectangularObjectScaleCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override RectangularObjectPropertyModifier<int> CreateModifier(ICommandUIState state) {
			return new RectangularObjectScaleModifier(ScaleX, ScaleY);
		}
	}
	#endregion
	#region ChangeRectangularObjectSizeCommand
	public class ChangeRectangularObjectSizeCommand : RectangularObjectCommandBase<Size> {
		#region Fields
		int width = 100;
		int height = 100;
		#endregion
		public ChangeRectangularObjectSizeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override RectangularObjectPropertyModifier<Size> CreateModifier(ICommandUIState state) {
			return new RectangularObjectSizeModifier(new Size(Width, Height));
		}
	}
	#endregion
	#region ChangeInlinePictureScaleCommand
	public class ChangeInlinePictureScaleCommand : ChangeRectangularObjectScaleCommand {
		public ChangeInlinePictureScaleCommand(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
	#region ChangeInlinePictureSizeCommand
	public class ChangeInlinePictureSizeCommand : ChangeRectangularObjectSizeCommand {
		public ChangeInlinePictureSizeCommand(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
}
