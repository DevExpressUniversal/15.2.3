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
namespace DevExpress.Office.Drawing {
	#region StyleMatrixElementType (enum)
	public enum StyleMatrixElementType {
		None,
		Subtle,
		Moderate,
		Intense
	}
	#endregion
	#region ThemeFormatScheme
	public class ThemeFormatScheme {
		#region Fields
		readonly List<IDrawingFill> backgroundFillStyleList = new List<IDrawingFill>();
		readonly List<IDrawingFill> fillStyleList = new List<IDrawingFill>();
		readonly List<Outline> lineStyleList = new List<Outline>();
		readonly List<DrawingEffectStyle> effectStyleList = new List<DrawingEffectStyle>();
		string name = String.Empty;
		#endregion
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public List<IDrawingFill> BackgroundFillStyleList { get { return backgroundFillStyleList; } }
		public List<IDrawingFill> FillStyleList { get { return fillStyleList; } }
		public List<Outline> LineStyleList { get { return lineStyleList; } }
		public List<DrawingEffectStyle> EffectStyleList { get { return effectStyleList; } }
		public bool IsValidate { get { return CheckValidation(); } }
		#endregion
		public Outline GetOutline(StyleMatrixElementType type) {
			return GetElement(type, lineStyleList);
		}
		public IDrawingFill GetFill(StyleMatrixElementType type) {
			return GetElement(type, fillStyleList);
		}
		public DrawingEffectStyle GetEffectStyle(StyleMatrixElementType type) {
			return GetElement(type, effectStyleList);
		}
		T GetElement<T>(StyleMatrixElementType type, List<T> items) where T : class {
			if (type == StyleMatrixElementType.Subtle)
				return items[0];
			if (type == StyleMatrixElementType.Moderate)
				return items[1];
			if (type == StyleMatrixElementType.Intense)
				return items[2];
			return null;
		}
		bool CheckValidation() {
			return
				backgroundFillStyleList.Count >= 3 &&
				fillStyleList.Count >= 3 &&
				lineStyleList.Count >= 3 &&
				effectStyleList.Count >= 3;
		}
		protected internal void CopyFrom(IDocumentModel targetModel, IOfficeTheme sourceTheme) {
			Clear();
			name = sourceTheme.FormatScheme.name;
			CopyFrom(targetModel, backgroundFillStyleList, sourceTheme.FormatScheme.backgroundFillStyleList);
			CopyFrom(targetModel, fillStyleList, sourceTheme.FormatScheme.fillStyleList);
			CopyFrom(targetModel, lineStyleList, sourceTheme.FormatScheme.lineStyleList);
			CopyFrom(targetModel, effectStyleList, sourceTheme.FormatScheme.effectStyleList);
		}
		void CopyFrom(IDocumentModel targetModel, List<IDrawingFill> targetList, List<IDrawingFill> sourceList) {
			int count = sourceList.Count;
			for (int i = 0; i < count; i++)
				targetList.Add(sourceList[i].CloneTo(targetModel));
		}
		void CopyFrom(IDocumentModel targetModel, List<Outline> targetList, List<Outline> sourceList) {
			int count = sourceList.Count;
			for (int i = 0; i < count; i++)
				targetList.Add(sourceList[i].CloneTo(targetModel));
		}
		void CopyFrom(IDocumentModel targetModel, List<DrawingEffectStyle> targetList, List<DrawingEffectStyle> sourceList) {
			int count = sourceList.Count;
			for (int i = 0; i < count; i++)
				targetList.Add(sourceList[i].CloneTo(targetModel));
		}
		protected internal void Clear() {
			name = String.Empty;
			backgroundFillStyleList.Clear();
			fillStyleList.Clear();
			lineStyleList.Clear();
			effectStyleList.Clear();
		}
	}
	#endregion
}
