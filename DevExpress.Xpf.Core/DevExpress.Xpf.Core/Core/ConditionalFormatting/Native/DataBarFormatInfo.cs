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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.GridData;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public class DataBarFormatInfo {
		public static DataBarFormatInfo AddIcon(DataBarFormatInfo info, ImageSource icon, VerticalAlignment verticalAlignment) {
			var result = new DataBarFormatInfo();
			if(info != null) {
				result.Format = info.Format;
				result.ZeroPosition = info.ZeroPosition;
				result.ValuePosition = info.ValuePosition;
			}
			AssignIcon(result, icon, verticalAlignment);
			return result;
		}
		public static DataBarFormatInfo AddDataBarFormatInfo(DataBarFormatInfo info, DataBarFormat format, double zeroPosition, double value) {
			var result = new DataBarFormatInfo();
			AssignDataBarFormatInfo(result, format, zeroPosition, value);
			if(info != null)
				AssignIcon(result, info.Icon, info.IconVerticalAlignment);
			return result;
		}
		static void AssignDataBarFormatInfo(DataBarFormatInfo result, DataBarFormat format, double zeroPosition, double value) {
			result.Format = (DataBarFormat)format.GetCurrentValueAsFrozen();
			result.ZeroPosition = zeroPosition;
			result.ValuePosition = value;
		}
		static void AssignIcon(DataBarFormatInfo result, ImageSource icon, VerticalAlignment verticalAlignment) {
			result.Icon = icon;
			result.IconVerticalAlignment = verticalAlignment;
		}
		public DataBarFormat Format { get; private set; }
		public double ZeroPosition { get; private set; }
		public double ValuePosition { get; private set; }
		public ImageSource Icon { get; private set; }
		public VerticalAlignment IconVerticalAlignment { get; private set; }
		public DataBarFormatInfo() { }
#if DEBUGTEST
		public DataBarFormatInfo(DataBarFormat format, double zeroPosition, double value) {
			AssignDataBarFormatInfo(this, format, zeroPosition, value);
		}
#endif
		public override bool Equals(object obj) {
			var other = obj as DataBarFormatInfo;
			return other != null && object.ReferenceEquals(other.Format, Format) && other.ZeroPosition == ZeroPosition && other.ValuePosition == ValuePosition && object.ReferenceEquals(other.Icon, Icon) && other.IconVerticalAlignment == IconVerticalAlignment;
		}
		public override int GetHashCode() {
			int hashCode = ZeroPosition.GetHashCode() ^ ValuePosition.GetHashCode() ^ IconVerticalAlignment.GetHashCode();
			if(Format != null)
				hashCode ^= Format.GetHashCode();
			if(Icon != null)
				hashCode ^= Icon.GetHashCode();
			return hashCode;
		}
	}
}
