#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using DevExpress.Pdf.Native;
using System;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfTransitionStyle.R)]
	public enum PdfTransitionStyle { Split, Blinds, Box, Wipe, Dissolve, Glitter, R, Fly, Push, Cover, Uncover, Fade }
	[PdfDefaultField(PdfTransitionDimension.Horizontal)]
	public enum PdfTransitionDimension {
		[PdfFieldName("H")]
		Horizontal,
		[PdfFieldName("V")]
		Vertical
	}
	[PdfDefaultField(PdfMotionDirection.Inward)]
	public enum PdfMotionDirection {
		[PdfFieldName("I")]
		Inward,
		[PdfFieldName("O")]
		Outward
	}
	public enum PdfTransitionDirection { None, LeftToRight, BottomToTop, RightToLeft, TopToBottom, TopLeftToBottomRight }
	public class PdfPagePresentation {
		const string dictionaryName = "Trans";
		const string transitionStyleKey = "S";
		const string durationKey = "D";
		const string dimensionKey = "Dm";
		const string motionDirectionKey = "M";
		const string transitionDirectionKey = "Di";
		const string scaleKey = "SS";
		const string isRectAndOpaqueKey = "B";
		readonly PdfTransitionStyle transitionStyle;
		readonly double duration;
		readonly PdfTransitionDimension dimension;
		readonly PdfMotionDirection motionDirection;
		readonly PdfTransitionDirection transitionDirection;
		readonly PdfRange changesScale;
		readonly bool isRectAndOpaque;
		public PdfTransitionStyle TransitionStyle { get { return transitionStyle; } }
		public double Duration { get { return duration; } }
		public PdfTransitionDimension Dimension { get { return dimension; } }
		public PdfMotionDirection MotionDirection { get { return motionDirection; } }
		public PdfTransitionDirection TransitionDirection { get { return transitionDirection; } }
		public PdfRange ChangesScale { get { return changesScale; } }
		public bool IsRectAndOpaque { get { return isRectAndOpaque; } }
		internal PdfPagePresentation(PdfReaderDictionary dictionary) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != dictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			duration = dictionary.GetNumber(durationKey) ?? 1;
			transitionStyle = dictionary.GetEnumName<PdfTransitionStyle>(transitionStyleKey);
			dimension = dictionary.GetEnumName<PdfTransitionDimension>(dimensionKey);
			motionDirection = dictionary.GetEnumName<PdfMotionDirection>(motionDirectionKey);
			transitionDirection = GetTransitionDirection(dictionary);
			double scaleBorder = dictionary.GetNumber(scaleKey) ?? 1.0;
			if (motionDirection == PdfMotionDirection.Inward)
				changesScale = new PdfRange(scaleBorder, 1.0);
			if (motionDirection == PdfMotionDirection.Outward)
				changesScale = new PdfRange(1.0, scaleBorder);
			isRectAndOpaque = dictionary.GetBoolean(isRectAndOpaqueKey) ?? false;
		}
		internal PdfDictionary Write() {
			PdfWriterDictionary result = new PdfWriterDictionary(null);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryName));
			result.AddEnumName(transitionStyleKey, transitionStyle);
			result.Add(durationKey, duration, 1.0);
			result.AddEnumName(dimensionKey, dimension);
			result.AddEnumName(motionDirectionKey, motionDirection);
			if (transitionDirection != PdfTransitionDirection.LeftToRight)
				result.Add(transitionDirectionKey, TransitionDirectionToWritableObject());
			if (motionDirection == PdfMotionDirection.Inward)
				result.Add(scaleKey, changesScale.Min);
			if (motionDirection == PdfMotionDirection.Outward)
				result.Add(scaleKey, changesScale.Max);
			result.Add(isRectAndOpaqueKey, isRectAndOpaque, false);
			return result;
		}
		object TransitionDirectionToWritableObject() {
			switch (transitionDirection) {
				case PdfTransitionDirection.None: return new PdfName("None");
				case PdfTransitionDirection.BottomToTop: return 90;
				case PdfTransitionDirection.RightToLeft: return 180;
				case PdfTransitionDirection.TopToBottom: return 270;
				case PdfTransitionDirection.TopLeftToBottomRight: return 317;
				case PdfTransitionDirection.LeftToRight:
				default: return 0;
			}
		}
		PdfTransitionDirection GetTransitionDirection(PdfReaderDictionary dictionary) {
			object value = GetDictionaryObject(dictionary, transitionDirectionKey);
			if (value == null)
				return PdfTransitionDirection.LeftToRight;
			PdfName name = value as PdfName;
			if (name != null) {
				if (name.Name != "None")
					PdfDocumentReader.ThrowIncorrectDataException();
				return PdfTransitionDirection.None;
			}
			double doubleValue = PdfDocumentReader.ConvertToDouble(value);
			switch ((int)doubleValue) {
				case 0: return PdfTransitionDirection.LeftToRight;
				case 90: return PdfTransitionDirection.BottomToTop;
				case 180: return PdfTransitionDirection.RightToLeft;
				case 270: return PdfTransitionDirection.TopToBottom;
				case 315: return PdfTransitionDirection.TopLeftToBottomRight;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return PdfTransitionDirection.None;
		}
		object GetDictionaryObject(PdfReaderDictionary dictionary, string key) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				value = dictionary.Objects.GetObjectData(reference.Number);
			return value;
		}
	}
}
