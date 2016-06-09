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
using System.IO;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfFontStorage : PdfDisposableObject {
		readonly Dictionary<PdfFont, PdfFontRegistrationData> dictionary = new Dictionary<PdfFont, PdfFontRegistrationData>();
		readonly string fontFolderName;
		PdfFont lastRegisteredFont;
		PdfFontRegistrationData lastRegisteredFontData;
		public PdfFontStorage() {
			fontFolderName = PdfTempFolder.Create();
		}
		public void DeleteFont(PdfFont font) {
			Unregister();
			PdfFontRegistrationData registrationData;
			if (dictionary.TryGetValue(font, out registrationData)) {
				dictionary.Remove(font);
				PdfFontRegistrator registrator = registrationData.Registrator;
				if (registrator != null)
					registrator.Dispose();
			}
		}
		public void Unregister() {
			if (lastRegisteredFontData != null) {
				PdfFontRegistrator lastUsedRegistrator = lastRegisteredFontData.Registrator;
				if (lastUsedRegistrator != null)
					lastUsedRegistrator.Unregister();
				lastRegisteredFontData = null;
			}
			lastRegisteredFont = null;
		}
		public void Clear() {
			Unregister();
			foreach (PdfFontRegistrationData data in dictionary.Values) {
				PdfFontRegistrator registrator = data.Registrator;
				if (registrator != null)
					registrator.Dispose();
			}
			dictionary.Clear();
		}
		internal PdfFontRegistrationData Register(PdfFont font) {
			if (Object.ReferenceEquals(font, lastRegisteredFont))
				return lastRegisteredFontData;
			Unregister();
			if (dictionary.TryGetValue(font, out lastRegisteredFontData)) {
				PdfFontRegistrator registrator = lastRegisteredFontData.Registrator;
				if (registrator != null)
					registrator.Register();
				lastRegisteredFont = font;
				return lastRegisteredFontData;
			}
			lastRegisteredFont = font;
			PdfFontRegistrator newRegistrator = PdfFontRegistrator.Create(font, fontFolderName);
			if (newRegistrator != null) {
				lastRegisteredFontData = newRegistrator.Register();
				if (lastRegisteredFontData.Registrator == null)
					newRegistrator.Dispose();
			}
			else
				lastRegisteredFontData = new PdfFontRegistrationData(font.FontName, 0,
					PdfFontRegistrator.GetWeight(font), PdfFontRegistrator.IsItalic(font), PdfFontRegistrator.GetPitchAndFamily(font), false, null, font is PdfType3Font);
			dictionary.Add(font, lastRegisteredFontData);
			return lastRegisteredFontData;
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				Clear();
			try {
				Directory.Delete(fontFolderName, true);
			}
			catch {
			}
		}
	}
}
