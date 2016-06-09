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
using System.CodeDom.Compiler;
using System.Collections;
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Hyperlink
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Hyperlink : IWordObject {
		string Name { get; }
		string AddressOld { get; }
		MsoHyperlinkType Type { get; }
		Range Range { get; }
		Shape Shape { get; }
		string SubAddressOld { get; }
		bool ExtraInfoRequired { get; }
		void Delete();
		void Follow(ref object NewWindow, ref object AddHistory, ref object ExtraInfo, ref object Method, ref object HeaderInfo);
		void AddToFavorites();
		void CreateNewDocument(string FileName, bool EditNow, bool Overwrite);
		string Address { get; set; }
		string SubAddress { get; set; }
		string EmailSubject { get; set; }
		string ScreenTip { get; set; }
		string TextToDisplay { get; set; }
		string Target { get; set; }
	}
	#endregion
	#region Hyperlinks
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Hyperlinks : IWordObject, IEnumerable {
		int Count { get; }
		Hyperlink this[object Index] { get; } 
		Hyperlink Add(object Anchor, ref object Address, ref object SubAddress, ref object ScreenTip, ref object TextToDisplay, ref object Target);
	}
	#endregion
}
