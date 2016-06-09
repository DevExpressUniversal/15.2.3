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
using DevExpress.XtraPrinting.BarCode;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.BarCode {
	public abstract class BarCode2DGenerator : BarCodeGeneratorBase {
		string text = string.Empty;
		byte[] binaryData = new byte[] { };
		RectangleF textBarcodeBounds = RectangleF.Empty;
		protected string Text { get { return text; } set { text = value; } }
		protected byte[] BinaryData { get { return binaryData; } set { binaryData = value; } }
		protected object Data { get { return TextCompactionMode() ? (object)ProcessText(text) : (object)binaryData; } }
		protected abstract IPatternProcessor PatternProcessor { get; }
		protected abstract bool IsSquareBarcode { get; }
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		public BarCode2DGenerator() {
		}
		public BarCode2DGenerator(BarCode2DGenerator source)
			: base(source) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Update(string text, byte[] binaryData) {
			if(TextCompactionMode() && text == null) throw new ArgumentException("text");
			if(BinaryCompactionMode() && binaryData == null) throw new ArgumentException("binaryData");
			this.text = text;
			this.binaryData = binaryData;
			RefreshPatternProcessor();
		}
		protected abstract bool TextCompactionMode();
		protected abstract bool BinaryCompactionMode();
		protected virtual string ProcessText(string text) {
			return text;
		}
		protected void RefreshPatternProcessor() {
			PatternProcessor.RefreshPattern(Data);
		}
		protected override char[] PrepareText(string text) {
			string validCharset = GetValidCharSet();
			string newText = string.Empty;
			foreach(char c in text)
				if(validCharset.Contains(c.ToString()))
					newText += c;
			return newText.ToCharArray();
		}
		protected override Hashtable GetPatternTable() {
			return new Hashtable();
		}
		protected override void DrawBarCode(IGraphicsBase gr, System.Drawing.RectangleF barBounds, System.Drawing.RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			RefreshPatternProcessor();
			gr.FillRectangle(gr.GetBrush(data.Style.BackColor), barBounds.Left, barBounds.Top, ((List<bool>)Pattern[0]).Count * xModule, Pattern.Count * yModule);
			int y = 0;
			foreach(List<bool> rowPattern in Pattern) {
				for(int x = 0; x < rowPattern.Count; x++)
					if(rowPattern[x])
						gr.FillRectangle(gr.GetBrush(data.Style.ForeColor), barBounds.Left + x * xModule, barBounds.Top + y * yModule, xModule, yModule);
				y++;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			return (float)((double)((List<bool>)pattern[0]).Count * module);
		}
		protected override float CalcBarCodeHeight(ArrayList pattern, double module) {
			return (float)((double)pattern.Count * module);
		}
		protected override double CalcAutoModuleX(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			if(!IsSquareBarcode)
				return base.CalcAutoModuleX(data, clientBounds, gr);
			double xModule = base.CalcAutoModuleX(data, clientBounds, gr);
			double yModule = base.CalcAutoModuleY(data, clientBounds, gr);
			return Math.Min(xModule, yModule);
		}
		protected override double CalcAutoModuleY(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			if(!IsSquareBarcode)
				return base.CalcAutoModuleY(data, clientBounds, gr);
			return CalcAutoModuleX(data, clientBounds, gr);
		}
		protected override void JustifyBarcodeBounds(IBarCodeData data, ref float barCodeWidth, ref float barCodeHeight, ref RectangleF barBounds) {
			if(!IsSquareBarcode)
				base.JustifyBarcodeBounds(data, ref barCodeWidth, ref barCodeHeight, ref barBounds);
		}
		protected override RectangleF AlignBarcodeBounds(RectangleF barcodeBounds, float width, float height, TextAlignment align) {
			if(!IsSquareBarcode)
				return base.AlignBarcodeBounds(barcodeBounds, width, height, align);
			textBarcodeBounds = barcodeBounds;
			barcodeBounds = base.AlignBarcodeBounds(barcodeBounds, width, height, align);
			barcodeBounds = AlignVerticalBarcodeBound(barcodeBounds, height, align);
			return barcodeBounds;
		}
		protected override RectangleF AlignTextBounds(IBarCodeData data, RectangleF barBounds, RectangleF textBounds) {
			if(!IsSquareBarcode)
				return base.AlignTextBounds(data, barBounds, textBounds);
			textBounds = base.AlignTextBounds(data, textBarcodeBounds, textBounds);
			return textBounds;
		}
		protected override ArrayList MakeBarCodePattern(string text) {
			if(BarCodeData != null) {
				bool needRefresh = false;
				if(BarCodeData.Text != Text) {
					Text = BarCodeData.Text;
					needRefresh = true;
				}
				BarCodeBrick brick = BarCodeData as BarCodeBrick;
				if(brick != null && brick.BinaryData != BinaryData) {
					BinaryData = brick.BinaryData;
					needRefresh = true;
				}
				if(needRefresh)
					RefreshPatternProcessor();
			} else if(Text != text) {
				Text = text;
				RefreshPatternProcessor();
			}
			return PatternProcessor.Pattern;
		}
	}
	public interface IPatternProcessor {
		ArrayList Pattern { get; }
		void RefreshPattern(object data);
		void Assign(IPatternProcessor source);
	}
}
