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
namespace DevExpress.Office.Forms {
	#region InsertSymbolViewModel (abstract class)
	public abstract class InsertSymbolViewModel : ViewModelBase {
		#region Fields
		char unicodeChar = ' ';
		string fontName = String.Empty;
		bool modelessBehavior;
		#endregion
		#region Properties
		public char UnicodeChar {
			get { return unicodeChar; }
			set {
				if (UnicodeChar == value)
					return;
				this.unicodeChar = value;
				OnPropertyChanged("UnicodeChar");
			}
		}
		public string FontName {
			get { return fontName; }
			set {
				if (FontName == value)
					return;
				this.fontName = value;
				OnPropertyChanged("FontName");
			}
		}
		public bool ModelessBehavior {
			get { return modelessBehavior; }
			set {
				if (ModelessBehavior == value)
					return;
				this.modelessBehavior = value;
				OnPropertyChanged("ModelessBehavior");
			}
		}
		#endregion
		public abstract void ApplyChanges();
	}
	#endregion
}
