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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ColorSchemeIndex
	public enum ColorSchemeIndex {
		Accent1,
		Accent2,
		Accent3,
		Accent4,
		Accent5,
		Accent6,
		Dark1,
		Dark2,
		Light1,
		Light2,
		FollowedHyperlink,
		Hyperlink
	}
	#endregion
	public class ColorMapOverride : ICloneable<ColorMapOverride>, ISupportsCopyFrom<ColorMapOverride> {
		#region Fields
		const int mapSize = 12;
		const int indexBackground1 = 0;
		const int indexBackground2 = 1;
		const int indexText1 = 2;
		const int indexText2 = 3;
		const int indexAccent1 = 4;
		const int indexAccent2 = 5;
		const int indexAccent3 = 6;
		const int indexAccent4 = 7;
		const int indexAccent5 = 8;
		const int indexAccent6 = 9;
		const int indexHyperlink = 10;
		const int indexFollowedHyperlink = 11;
		IChart parent;
		ColorSchemeIndex[] innerMap;
		bool isDefined;
		#endregion
		public ColorMapOverride(IChart parent) {
			this.parent = parent;
			this.innerMap = new ColorSchemeIndex[mapSize];
			Initialize();
		}
		void Initialize() {
			this.innerMap[indexBackground1] = ColorSchemeIndex.Light1;
			this.innerMap[indexBackground2] = ColorSchemeIndex.Light2;
			this.innerMap[indexText1] = ColorSchemeIndex.Dark1;
			this.innerMap[indexText2] = ColorSchemeIndex.Dark2;
			this.innerMap[indexAccent1] = ColorSchemeIndex.Accent1;
			this.innerMap[indexAccent2] = ColorSchemeIndex.Accent2;
			this.innerMap[indexAccent3] = ColorSchemeIndex.Accent3;
			this.innerMap[indexAccent4] = ColorSchemeIndex.Accent4;
			this.innerMap[indexAccent5] = ColorSchemeIndex.Accent5;
			this.innerMap[indexAccent6] = ColorSchemeIndex.Accent6;
			this.innerMap[indexHyperlink] = ColorSchemeIndex.Hyperlink;
			this.innerMap[indexFollowedHyperlink] = ColorSchemeIndex.FollowedHyperlink;
			this.isDefined = false;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public ColorSchemeIndex Background1 {
			get { return this.innerMap[indexBackground1]; }
			set {
				if(Background1 == value)
					return;
				SetMapEntry(indexBackground1, value);
			}
		}
		public ColorSchemeIndex Background2 {
			get { return this.innerMap[indexBackground2]; }
			set {
				if(Background2 == value)
					return;
				SetMapEntry(indexBackground2, value);
			}
		}
		public ColorSchemeIndex Text1 {
			get { return this.innerMap[indexText1]; }
			set {
				if(Text1 == value)
					return;
				SetMapEntry(indexText1, value);
			}
		}
		public ColorSchemeIndex Text2 {
			get { return this.innerMap[indexText2]; }
			set {
				if(Text2 == value)
					return;
				SetMapEntry(indexText2, value);
			}
		}
		public ColorSchemeIndex Accent1 {
			get { return this.innerMap[indexAccent1]; }
			set {
				if(Accent1 == value)
					return;
				SetMapEntry(indexAccent1, value);
			}
		}
		public ColorSchemeIndex Accent2 {
			get { return this.innerMap[indexAccent2]; }
			set {
				if(Accent2 == value)
					return;
				SetMapEntry(indexAccent2, value);
			}
		}
		public ColorSchemeIndex Accent3 {
			get { return this.innerMap[indexAccent3]; }
			set {
				if(Accent3 == value)
					return;
				SetMapEntry(indexAccent3, value);
			}
		}
		public ColorSchemeIndex Accent4 {
			get { return this.innerMap[indexAccent4]; }
			set {
				if(Accent4 == value)
					return;
				SetMapEntry(indexAccent4, value);
			}
		}
		public ColorSchemeIndex Accent5 {
			get { return this.innerMap[indexAccent5]; }
			set {
				if(Accent5 == value)
					return;
				SetMapEntry(indexAccent5, value);
			}
		}
		public ColorSchemeIndex Accent6 {
			get { return this.innerMap[indexAccent6]; }
			set {
				if(Accent6 == value)
					return;
				SetMapEntry(indexAccent6, value);
			}
		}
		public ColorSchemeIndex Hyperlink {
			get { return this.innerMap[indexHyperlink]; }
			set {
				if(Hyperlink == value)
					return;
				SetMapEntry(indexHyperlink, value);
			}
		}
		public ColorSchemeIndex FollowedHyperlink {
			get { return this.innerMap[indexFollowedHyperlink]; }
			set {
				if(FollowedHyperlink == value)
					return;
				SetMapEntry(indexFollowedHyperlink, value);
			}
		}
		public bool IsDefined {
			get { return isDefined; }
			set {
				if(isDefined == value)
					return;
				SetIsDefined(value);
			}
		}
		#endregion
		void SetMapEntry(int index, ColorSchemeIndex value) {
			if(index < 0 || index >= mapSize)
				return;
			ColorMapOverridePropertyChangedHistoryItem historyItem = new ColorMapOverridePropertyChangedHistoryItem(DocumentModel, this, index, this.innerMap[index], value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetMapEntryCore(int index, ColorSchemeIndex value) {
			if(index < 0 || index >= mapSize)
				return;
			this.innerMap[index] = value;
			if (IsDefined)
				Parent.Invalidate();
		}
		void SetIsDefined(bool value) {
			ColorMapOverrideIsDefinedPropertyChangedHistoryItem historyItem = new ColorMapOverrideIsDefinedPropertyChangedHistoryItem(DocumentModel, this, isDefined, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetIsDefinedCore(bool value) {
			this.isDefined = value;
			Parent.Invalidate();
		}
		#region ICloneable<ColorMapOverride> Members
		public ColorMapOverride Clone() {
			ColorMapOverride result = new ColorMapOverride(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ColorMapOverride> Members
		public void CopyFrom(ColorMapOverride value) {
			Guard.ArgumentNotNull(value, "value");
			for(int i = 0; i < mapSize; i++)
				this.innerMap[i] = value.innerMap[i];
		}
		#endregion
	}
}
