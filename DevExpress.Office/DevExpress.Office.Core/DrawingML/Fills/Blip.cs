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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	#region CompressionState
	public enum CompressionState { 
		Email,
		HighQualityPrinting,
		None,
		Print,
		Screen
	}
	#endregion
	#region Blip
	public class DrawingBlip : ICloneable<DrawingBlip>, ISupportsCopyFrom<DrawingBlip>, IDrawingBullet {
		#region Fields
		readonly InvalidateProxy innerParent;
		readonly DrawingEffectCollection effects;
		bool embedded = true;
		string link = String.Empty;
		CompressionState compressionState;
		OfficeImage image;
		#endregion
		public DrawingBlip(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			innerParent = new InvalidateProxy();
			effects = new DrawingEffectCollection(documentModel);
			compressionState = CompressionState.None;
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return effects.DocumentModel; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public DrawingEffectCollection Effects { get { return effects; } }
		public bool IsEmpty { get { return Embedded && Effects.Count == 0 && CompressionState == CompressionState.None && String.IsNullOrEmpty(Link); } }
		#region Embedded
		public bool Embedded { 
			get { return embedded; } 
			protected set {
				if (embedded != value)
					ApplyHistoryItem(new DrawingBlipEmbeddedChangeHistoryItem(this, embedded, value));
			} 
		}
		public void SetEmbeddedCore(bool value) {
			embedded = value;
		}
		#endregion
		#region Link
		public string Link {
			get { return link; }
			protected set {
				if (link != value)
					ApplyHistoryItem(new DrawingBlipLinkChangeHistoryItem(this, link, value));
			}
		}
		public void SetLinkCore(string value) {
			link = value;
		}
		#endregion
		#region CompressionState
		public CompressionState CompressionState {
			get { return compressionState; }
			set {
				if (CompressionState != value)
					ApplyHistoryItem(new DrawingBlipCompressionStateHistoryItem(this, compressionState, value));
			}
		}
		public void SetCompressionStateCore(CompressionState value) {
			compressionState = value;
		}
		#endregion
		#region Image
		public OfficeImage Image {
			get { return image; }
			set {
				if (!OfficeImage.Equals(image, value))
					ApplyHistoryItem(new DrawingBlipImageChangeHistoryItem(this, image, value));
			}
		}
		public void SetImageCore(OfficeImage value) {
			this.image = value;
			this.innerParent.Invalidate();
		}
		#endregion
		#endregion
		#region SetExternal
		public void SetExternal(string link) {
			SetExternalCore(false, link);
		}
		void SetExternalCore(bool embedded, string link) {
			DocumentModel.History.BeginTransaction();
			Embedded = embedded;
			Link = link;
			DocumentModel.History.EndTransaction();
		}
		#endregion 
		#region SetEmbedded
		public void SetEmbedded() {
			SetExternalCore(true, String.Empty);
		}
		#endregion 
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region ICloneable<DrawingBlip> Members
		public DrawingBlip Clone() {
			DrawingBlip result = new DrawingBlip(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingBlip> Members
		public void CopyFrom(DrawingBlip value) {
			link = value.Link;
			embedded = value.Embedded;
			compressionState = value.compressionState;
			effects.CopyFrom(value.effects);
			if (value.Image == null)
				Image = null;
			else
				Image = value.Image.Clone(DocumentModel);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingBlip other = obj as DrawingBlip;
			if (obj == null)
				return false;
			return 
				effects.Equals(other.effects) && 
				compressionState == other.compressionState &&
				embedded == other.embedded &&
				StringExtensions.CompareInvariantCultureIgnoreCase(link, other.link) == 0 &&
				OfficeImage.Equals(this.image, other.image);
		}
		public override int GetHashCode() {
			return 
				GetType().GetHashCode() ^ effects.GetHashCode() ^ 
				compressionState.GetHashCode() ^ embedded.GetHashCode() ^ 
				link.GetHashCode() ^ image.GetHashCode();
		}
		#endregion
		#region IDrawingBullet
		public DrawingBulletType Type { get { return DrawingBulletType.Common; } }
		public IDrawingBullet CloneTo(IDocumentModel documentModel) {
			DrawingBlip result = new DrawingBlip(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
}
