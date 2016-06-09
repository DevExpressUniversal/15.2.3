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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region IStrikeoutPainter
	public interface IStrikeoutPainter : IPatternLinePainter<StrikeoutType> {
		void DrawStrikeout(StrikeoutSingle Strikeout, RectangleF bounds, Color color);
		void DrawStrikeout(StrikeoutDouble Strikeout, RectangleF bounds, Color color);
	}
	#endregion
	#region Strikeout (abstract class)
	public abstract class Strikeout : PatternLine<StrikeoutType> {
		public override void Draw(IPatternLinePainter<StrikeoutType> painter, RectangleF bounds, Color color) {
			IStrikeoutPainter strikeoutPainter = painter as IStrikeoutPainter;
			if (strikeoutPainter != null)
				Draw(strikeoutPainter, bounds, color);
		}
		public abstract void Draw(IStrikeoutPainter painter, RectangleF bounds, Color color);
	}
	#endregion
	#region StrikeoutCollection
	public class StrikeoutCollection : List<Strikeout> {
	}
	#endregion
	#region StrikeoutRepository
	public class StrikeoutRepository : PatternLineRepository<StrikeoutType, Strikeout, StrikeoutCollection> {
		protected override void PopulateRepository() {
			RegisterPatternLine(new StrikeoutSingle());
			RegisterPatternLine(new StrikeoutDouble());
		}
	}
	#endregion
	#region StrikeoutType
	public enum StrikeoutType {
		None = 0,
		Single = 1,
		Double = 2,
	}
	#endregion
	#region StrikeoutSingle
	public class StrikeoutSingle : Strikeout {
		public override StrikeoutType Id { get { return StrikeoutType.Single; } }
		public override void Draw(IStrikeoutPainter painter, RectangleF bounds, Color color) {
			painter.DrawStrikeout(this, bounds, color);
		}
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return new Rectangle(r.X, r.Y, r.Width, thickness);
		}
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return lineBounds.Height / 2;
		}
	}
	#endregion
	#region StrikeoutDouble
	public class StrikeoutDouble : Strikeout {
		public override StrikeoutType Id { get { return StrikeoutType.Double; } }
		public override void Draw(IStrikeoutPainter painter, RectangleF bounds, Color color) {
			painter.DrawStrikeout(this, bounds, color);
		}
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return new Rectangle(r.X, r.Y - 3 * thickness / 2, r.Width, 3 * thickness);
		}
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return 0;
		}
	}
	#endregion
}
