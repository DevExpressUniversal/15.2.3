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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Compatibility.System.Drawing;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.Xpf.Collections;
using System.Windows.Media;
using DevExpress.XtraPrinting.Stubs;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	public interface IPrintingSystem {
		string Version { get; }
		IImagesContainer Images { get; }
		int AutoFitToPagesWidth { get; set; }
		IBrick CreateBrick(string typeName);
		event ChangeEventHandler BeforeChange;
		event ChangeEventHandler AfterChange;
		ITextBrick CreateTextBrick();
		IImageBrick CreateImageBrick();
		IPanelBrick CreatePanelBrick();
		IRichTextBrick CreateRichTextBrick();
		IProgressBarBrick CreateProgressBarBrick();
		ITrackBarBrick CreateTrackBarBrick();
		void InsertPageBreak(float pos);
#if !SL && !DXPORTABLE
		void SetCommandVisibility(PrintingSystemCommand command, bool visible);
#endif
	}
	public interface IVisualBrick : IBaseBrick {
		Color BackColor { get; set; }
		Color BorderColor { get; set; }
		BrickBorderStyle BorderStyle { get; set; }
		float BorderWidth { get; set; }
		BorderDashStyle BorderDashStyle { get; set;}
		PaddingInfo Padding { get; set; }
		BorderSide Sides { get; set; }
		BrickStyle Style { get; set; }
		bool SeparableHorz { get; set; }
		bool SeparableVert { get; set; }
		bool Separable { get; set; }
		string TextValueFormatString { get; set; }
		object TextValue { get; set; }
		string Text { get; set; }
		bool UseTextAsDefaultHint { get; set; }
	}
	public interface ITextBrick : IVisualBrick {
		Font Font { get; set; }
		Color ForeColor { get; set; }
		HorzAlignment HorzAlignment { get; set; }
		BrickStringFormat StringFormat { get; set; }
		VertAlignment VertAlignment { get; set; }
		DefaultBoolean XlsExportNativeFormat { get; set; }
	}
	public interface IRichTextBrick : IVisualBrick {
		string RtfText { get; set; }
		Font BaseFont { get; set; }
		int InfiniteHeight { get; }
		int EffectiveHeight { get; }
		IList GetChildBricks();
	}
	public interface IPanelBrick : IVisualBrick {
		IList Bricks { get; }
	}
	public interface IImageBrick : IVisualBrick {
		Image Image { get; set; }
		ImageSizeMode SizeMode { get; set; }
	}
	public interface IImagesContainer {
		bool ContainsImage(object key);
		Image GetImageByKey(object key);
		Image GetImage(object key, Image image);
	}
	public interface IProgressBarBrick : IVisualBrick {
		Color ForeColor { get; set; }
		int Position { get; set; }
	}
	public interface ITrackBarBrick : IVisualBrick {
		Color ForeColor { get; set; }
		int Position { get; set; }
		int Minimum { get; }
		int Maximum { get; }
	}
}
