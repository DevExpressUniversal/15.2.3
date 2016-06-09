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
using DevExpress.Utils;
using System.IO;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class SectionPropertiesWriter : IDisposable {
		BinaryWriter writer;
		SectionPropertiesHelper sectionHelper;
		public SectionPropertiesWriter(MemoryStream memoryStream) {
			Guard.ArgumentNotNull(memoryStream, "memoryStream");
			this.writer = new BinaryWriter(memoryStream);
			this.sectionHelper = new SectionPropertiesHelper();
		}
		public SectionPropertiesHelper SectionHelper { get { return this.sectionHelper; } }
		public void WriteSection(int characterPosition, Section section) {
			byte[] grpprl = GetSectionGroupPropertiesModifiers(section);
			if (grpprl.Length == 0) {
				sectionHelper.AddEntry(characterPosition, -1);
			}
			else {
				int sepxOffset = (int)writer.BaseStream.Position;
				writer.Write((ushort)grpprl.Length);
				writer.Write(grpprl);
				sectionHelper.AddEntry(characterPosition, sepxOffset);
			}
		}
		public void Finish(int lastPosition) {
			sectionHelper.AddLastPosition(lastPosition);
		}
		protected byte[] GetSectionGroupPropertiesModifiers(Section section) {
			using (MemoryStream output = new MemoryStream()) {
				DocSectionPropertiesActions actions = new DocSectionPropertiesActions(output, section);
				actions.CreateSectionPropertiesModifiers();
				return output.ToArray();
			}
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
}
