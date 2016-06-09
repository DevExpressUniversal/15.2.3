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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region IDrawingBullet
	public interface IDrawingBullet {
		DrawingBulletType Type { get; }
		IDrawingBullet CloneTo(IDocumentModel documentModel);
		void Visit(IDrawingBulletVisitor visitor);
	}
	#endregion
	#region DrawingBulletType
	public enum DrawingBulletType {
		Automatic, 
		Color, 
		Size, 
		Typeface, 
		Common
	}
	#endregion
	#region IDrawingBulletVisitor
	public interface IDrawingBulletVisitor {
		void Visit(DrawingBulletAutoNumbered bullet);
		void Visit(DrawingBulletCharacter bullet);
		void Visit(DrawingBlip bullet);
		void Visit(DrawingColor bullet);
		void Visit(DrawingTextFont bullet);
		void Visit(DrawingBulletSizePercentage bullet);
		void Visit(DrawingBulletSizePoints bullet);
		void VisitNoBullets();
		void VisitBulletColorFollowText();
		void VisitBulletTypefaceFollowText();
		void VisitBulletSizeFollowText();
	}
	#endregion
	#region DrawingBullet (Automatic, NoBullets, TypefaceFollowText, ColorFollowText, SizeFollowText)
	public sealed class DrawingBullet : IDrawingBullet {
		public static DrawingBullet Automatic = new DrawingBullet(DrawingBulletType.Automatic);
		public static DrawingBullet NoBullets = new DrawingBullet(DrawingBulletType.Common);
		public static DrawingBullet TypefaceFollowText = new DrawingBullet(DrawingBulletType.Typeface);
		public static DrawingBullet ColorFollowText = new DrawingBullet(DrawingBulletType.Color);
		public static DrawingBullet SizeFollowText = new DrawingBullet(DrawingBulletType.Size);
		DrawingBulletType type;
		DrawingBullet(DrawingBulletType type) {
			this.type = type;
		}
		public DrawingBulletType Type { get { return type; } }
		public void Visit(IDrawingBulletVisitor visitor) {
			if (type == DrawingBulletType.Common)
				visitor.VisitNoBullets();
			if (type == DrawingBulletType.Typeface)
				visitor.VisitBulletTypefaceFollowText();
			if (type == DrawingBulletType.Color)
				visitor.VisitBulletColorFollowText();
			if (type == DrawingBulletType.Size)
				visitor.VisitBulletSizeFollowText();
		}
		public override bool Equals(object obj) {
			DrawingBullet bullet = obj as DrawingBullet;
			return bullet != null && type == bullet.type;
		}
		public override int GetHashCode() {
			return (int)type;
		}
		public IDrawingBullet CloneTo(IDocumentModel documentModel) {
			if (type == DrawingBulletType.Common)
				return NoBullets;
			if (type == DrawingBulletType.Typeface)
				return TypefaceFollowText;
			if (type == DrawingBulletType.Color)
				return ColorFollowText;
			if (type == DrawingBulletType.Size)
				return SizeFollowText;
			return Automatic;
		}
	}
	#endregion
	#region DrawingBulletAutoNumbered
	public class DrawingBulletAutoNumbered : IDrawingBullet {
		#region Fields
		public const short DefaultStartAtValue = 1;
		readonly DrawingTextAutoNumberSchemeType schemeType;
		readonly short startAt;
		#endregion
		public DrawingBulletAutoNumbered(DrawingTextAutoNumberSchemeType schemeType, short startAt) {
			DrawingValueChecker.CheckTextBulletStartAtNumValue(startAt);
			this.schemeType = schemeType;
			this.startAt = startAt;
		}
		#region Properties
		public DrawingTextAutoNumberSchemeType SchemeType { get { return schemeType; } }
		public DrawingBulletType Type { get { return DrawingBulletType.Common; } }
		public short StartAt { get { return startAt; } }
		#endregion
		public void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			DrawingBulletAutoNumbered bullet = obj as DrawingBulletAutoNumbered;
			return bullet != null && schemeType == bullet.schemeType && startAt == bullet.startAt;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ (int)schemeType ^ startAt;
		}
		public IDrawingBullet CloneTo(IDocumentModel documentModel) {
			return new DrawingBulletAutoNumbered(schemeType, startAt);
		}
	}
	#region DrawingTextAutoNumberSchemeType
	public enum DrawingTextAutoNumberSchemeType {
		AlphaLcParenBoth, 
		AlphaUcParenBoth, 
		AlphaLcParenR, 
		AlphaUcParenR,
		AlphaLcPeriod,
		AlphaUcPeriod,
		ArabicParenBoth,
		ArabicParenR,
		ArabicPeriod,
		ArabicPlain,
		RomanLcParenBoth,
		RomanUcParenBoth,
		RomanLcParenR,
		RomanUcParenR,
		RomanLcPeriod,
		RomanUcPeriod,
		CircleNumDbPlain,
		CircleNumWdBlackPlain,
		CircleNumWdWhitePlain,
		ArabicDbPeriod,
		ArabicDbPlain,
		Ea1ChsPeriod,
		Ea1ChsPlain,
		Ea1ChtPeriod,
		Ea1ChtPlain,
		Ea1JpnChsDbPeriod,
		Ea1JpnKorPlain,
		Ea1JpnKorPeriod,
		Arabic1Minus,
		Arabic2Minus,
		Hebrew2Minus,
		ThaiAlphaPeriod,
		ThaiAlphaParenR,
		ThaiAlphaParenBoth,
		ThaiNumPeriod,
		ThaiNumParenR,
		ThaiNumParenBoth,
		HindiAlphaPeriod,
		HindiNumPeriod,
		HindiNumParenR,
		HindiAlpha1Period,
	}
	#endregion
	#endregion
	#region DrawingBulletCharacter
	public class DrawingBulletCharacter : IDrawingBullet {
		readonly string character;
		public DrawingBulletCharacter(string character) {
			Guard.ArgumentNotNull(character, "character");
			this.character = character;
		}
		public string Character { get { return character; } }
		public DrawingBulletType Type { get { return DrawingBulletType.Common; } }
		public void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			DrawingBulletCharacter bullet = obj as DrawingBulletCharacter;
			return bullet != null && character.Equals(bullet.character);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ character.GetHashCode();
		}
		public IDrawingBullet CloneTo(IDocumentModel documentModel) {
			return new DrawingBulletCharacter(character);
		}
	}
	#endregion
	#region DrawingBulletSizeBase
	public abstract class DrawingBulletSizeBase : IDrawingBullet {
		int value;
		protected DrawingBulletSizeBase(int value) {
			this.value = value;
		}
		#region Properties
		public DrawingBulletType Type { get { return DrawingBulletType.Size; } }
		public int Value { get { return value; } }
		#endregion
		public override bool Equals(object obj) {
			DrawingBulletSizeBase bullet = GetDrawingBulletSize(obj);
			return bullet != null && value == bullet.value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ value;
		}
		protected abstract DrawingBulletSizeBase GetDrawingBulletSize(object obj);
		public abstract void Visit(IDrawingBulletVisitor visitor);
		public abstract IDrawingBullet CloneTo(IDocumentModel documentModel);
	}
	#endregion
	#region DrawingBulletSizePercentage
	public class DrawingBulletSizePercentage : DrawingBulletSizeBase {
		public DrawingBulletSizePercentage(int value)
			: base(value) {
		}
		public override void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		protected override DrawingBulletSizeBase GetDrawingBulletSize(object obj) {
			return obj as DrawingBulletSizePercentage;
		}
		public override IDrawingBullet CloneTo(IDocumentModel documentModel) {
			return new DrawingBulletSizePercentage(Value);
		}
	}
	#endregion
	#region DrawingBulletSizePoints
	public class DrawingBulletSizePoints : DrawingBulletSizeBase {
		public DrawingBulletSizePoints(int value)
			: base(value) {
		}
		public override void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		protected override DrawingBulletSizeBase GetDrawingBulletSize(object obj) {
			return obj as DrawingBulletSizePoints;
		}
		public override IDrawingBullet CloneTo(IDocumentModel documentModel) {
			return new DrawingBulletSizePoints(Value);
		}
	}
	#endregion
}
