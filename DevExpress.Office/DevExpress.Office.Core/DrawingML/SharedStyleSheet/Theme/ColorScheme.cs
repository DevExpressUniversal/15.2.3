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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Drawing {
	#region ThemeDrawingColorCollection
	public class ThemeDrawingColorCollection {
		#region Static Members
		static Dictionary<SchemeColorValues, ThemeColorIndex> CreateSchemeColorValuesToThemeColorIndexTranslationTable() {
			Dictionary<SchemeColorValues, ThemeColorIndex> result = new Dictionary<SchemeColorValues, ThemeColorIndex>();
			result.Add(SchemeColorValues.Background1, ThemeColorIndex.Light1);
			result.Add(SchemeColorValues.Background2, ThemeColorIndex.Light2);
			result.Add(SchemeColorValues.Text1, ThemeColorIndex.Dark1);
			result.Add(SchemeColorValues.Text2, ThemeColorIndex.Dark2);
			result.Add(SchemeColorValues.Light1, ThemeColorIndex.Light1);
			result.Add(SchemeColorValues.Light2, ThemeColorIndex.Light2);
			result.Add(SchemeColorValues.Dark1, ThemeColorIndex.Dark1);
			result.Add(SchemeColorValues.Dark2, ThemeColorIndex.Dark2);
			result.Add(SchemeColorValues.Accent1, ThemeColorIndex.Accent1);
			result.Add(SchemeColorValues.Accent2, ThemeColorIndex.Accent2);
			result.Add(SchemeColorValues.Accent3, ThemeColorIndex.Accent3);
			result.Add(SchemeColorValues.Accent4, ThemeColorIndex.Accent4);
			result.Add(SchemeColorValues.Accent5, ThemeColorIndex.Accent5);
			result.Add(SchemeColorValues.Accent6, ThemeColorIndex.Accent6);
			result.Add(SchemeColorValues.Hyperlink, ThemeColorIndex.Hyperlink);
			result.Add(SchemeColorValues.FollowedHyperlink, ThemeColorIndex.FollowedHyperlink);
			return result;
		}
		#endregion
		#region Fields
		readonly IDocumentModel documentModel;
		readonly Dictionary<ThemeColorIndex, DrawingColor> innerCollection;
		readonly Dictionary<SchemeColorValues, ThemeColorIndex> schemeColorValuesToThemeColorIndexTranslationTable = CreateSchemeColorValuesToThemeColorIndexTranslationTable();
		string name;
		#endregion
		public ThemeDrawingColorCollection(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.name = String.Empty;
			this.innerCollection = new Dictionary<ThemeColorIndex, DrawingColor>();
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public DrawingColor Light1 { get { return TryGetDrawingColor(ThemeColorIndex.Light1); } set { SetDrawingColor(ThemeColorIndex.Light1, value); } }
		public DrawingColor Light2 { get { return TryGetDrawingColor(ThemeColorIndex.Light2); } set { SetDrawingColor(ThemeColorIndex.Light2, value); } }
		public DrawingColor Dark1 { get { return TryGetDrawingColor(ThemeColorIndex.Dark1); } set { SetDrawingColor(ThemeColorIndex.Dark1, value); } }
		public DrawingColor Dark2 { get { return TryGetDrawingColor(ThemeColorIndex.Dark2); } set { SetDrawingColor(ThemeColorIndex.Dark2, value); } }
		public DrawingColor Accent1 { get { return TryGetDrawingColor(ThemeColorIndex.Accent1); } set { SetDrawingColor(ThemeColorIndex.Accent1, value); } }
		public DrawingColor Accent2 { get { return TryGetDrawingColor(ThemeColorIndex.Accent2); } set { SetDrawingColor(ThemeColorIndex.Accent2, value); } }
		public DrawingColor Accent3 { get { return TryGetDrawingColor(ThemeColorIndex.Accent3); } set { SetDrawingColor(ThemeColorIndex.Accent3, value); } }
		public DrawingColor Accent4 { get { return TryGetDrawingColor(ThemeColorIndex.Accent4); } set { SetDrawingColor(ThemeColorIndex.Accent4, value); } }
		public DrawingColor Accent5 { get { return TryGetDrawingColor(ThemeColorIndex.Accent5); } set { SetDrawingColor(ThemeColorIndex.Accent5, value); } }
		public DrawingColor Accent6 { get { return TryGetDrawingColor(ThemeColorIndex.Accent6); } set { SetDrawingColor(ThemeColorIndex.Accent6, value); } }
		public DrawingColor Hyperlink { get { return TryGetDrawingColor(ThemeColorIndex.Hyperlink); } set { SetDrawingColor(ThemeColorIndex.Hyperlink, value); } }
		public DrawingColor FollowedHyperlink { get { return TryGetDrawingColor(ThemeColorIndex.FollowedHyperlink); } set { SetDrawingColor(ThemeColorIndex.FollowedHyperlink, value); } }
		public bool IsValidate { get { return CheckValidation(); } }
		public Dictionary<SchemeColorValues, ThemeColorIndex> SchemeColorValuesToThemeColorIndexTranslationTable { get { return schemeColorValuesToThemeColorIndexTranslationTable; } }
		#endregion
		public Color GetColor(ThemeColorIndex key) {
			DrawingColor drawingColor = TryGetDrawingColor(key);
			if (drawingColor == null) {
				Exceptions.ThrowInternalException();
			}
			return drawingColor.FinalColor;
		}
		public Color GetColor(SchemeColorValues value) {
			return GetColor(schemeColorValuesToThemeColorIndexTranslationTable[value]);
		}
		internal void CopyFrom(ThemeDrawingColorCollection sourceObj) {
			Clear();
			name = sourceObj.name;
			foreach (KeyValuePair<ThemeColorIndex, DrawingColor> source in sourceObj.innerCollection) 
				innerCollection.Add(source.Key, GetColor(source.Value, sourceObj.documentModel));
		}
		DrawingColor GetColor(DrawingColor sourceColor, IDocumentModel sourceDocumentModel) {
			if (Object.ReferenceEquals(sourceDocumentModel, documentModel))
				return sourceColor;
			return sourceColor.CloneTo(documentModel);
		}
		internal void Clear() {
			name = String.Empty;
			innerCollection.Clear();
		}
		bool CheckValidation() {
			return
				name != null &&
				innerCollection.Count == 12 &&
				innerCollection.ContainsKey(ThemeColorIndex.Dark1) &&
				innerCollection.ContainsKey(ThemeColorIndex.Light1) &&
				innerCollection.ContainsKey(ThemeColorIndex.Dark2) &&
				innerCollection.ContainsKey(ThemeColorIndex.Light2) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent1) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent2) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent3) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent4) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent5) &&
				innerCollection.ContainsKey(ThemeColorIndex.Accent6) &&
				innerCollection.ContainsKey(ThemeColorIndex.Hyperlink) &&
				innerCollection.ContainsKey(ThemeColorIndex.FollowedHyperlink);
		}
		public void SetDrawingColor(ThemeColorIndex index, DrawingColor value) {
			Guard.ArgumentNotNull(value, "DrawingColor");
			if (innerCollection.ContainsKey(index))
				innerCollection[index] = value;
			else
				innerCollection.Add(index, value);
		}
		internal void SetColor(ThemeColorIndex index, Color value) {
			SetDrawingColor(index, CreateDrawingColor(value));
		}
		internal void SetColor(ThemeColorIndex index, SystemColorValues value) {
			SetDrawingColor(index, CreateDrawingColor(value));
		}
		DrawingColor CreateDrawingColor(SystemColorValues value) {
			return DrawingColor.Create(documentModel, DrawingColorModelInfo.CreateSystem(value));
		}
		DrawingColor CreateDrawingColor(Color value) {
			return DrawingColor.Create(documentModel, DrawingColorModelInfo.CreateARGB(value));
		}
		DrawingColor TryGetDrawingColor(ThemeColorIndex index) {
			if (innerCollection.ContainsKey(index))
				return innerCollection[index];
			return null;
		}
	}
	#endregion
}
