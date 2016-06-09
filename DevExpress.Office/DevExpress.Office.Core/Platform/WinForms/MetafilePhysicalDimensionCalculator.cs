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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using DevExpress.Office.PInvoke;
namespace DevExpress.Office.Utils {
	#region MetafilePhysicalDimensionCalculator
	public class MetafilePhysicalDimensionCalculator {
		Win32.MapMode mapMode;
		Size windowExtent;
		public Size Calculate(Metafile metafile, byte[] bytes) {
			this.windowExtent = Size.Empty;
			this.mapMode = Win32.MapMode.HighMetric;
			Size result;
			IntPtr hmf = Win32.SetMetaFileBitsEx(bytes.Length, bytes);
			if (hmf != IntPtr.Zero) {
				result = CalculateWmfPhysicalDimension(hmf, metafile);
				Win32.DeleteMetaFile(hmf);
			}
			else {
				Win32.METAFILEPICT mfp = new Win32.METAFILEPICT(Win32.MapMode.Anisotropic, -1, -1);
				IntPtr hEmf = Win32.SetWinMetaFileBits(bytes.Length, bytes, IntPtr.Zero, ref mfp);
				if (hEmf == IntPtr.Zero)
					hEmf = Win32.SetEnhMetaFileBits(bytes.Length, bytes);
				if (hEmf != IntPtr.Zero) {
					result = CalculateEmfPhysicalDimension(hEmf, metafile);
					MetafileHelper.DeleteMetafileHandle(hEmf);
				}
				else
					result = Size.Round(metafile.PhysicalDimension);
			}
			return result;
		}
		Size CalculateWmfPhysicalDimension(IntPtr hmf, Metafile metafile) {
			Win32.EnumMetaFile(IntPtr.Zero, hmf, EnumMetafileCallback, IntPtr.Zero);
			Size result = windowExtent;
			if (result == Size.Empty)
				result = metafile.Size;
			if (mapMode == Win32.MapMode.HighMetric)
				return result;
			return ConvertLogicalUnitsToHundredthsOfMillimeter(result);
		}
		Size CalculateEmfPhysicalDimension(IntPtr hEmf, Metafile metafile) {
			Win32.RECT rect = new Win32.RECT();
			Win32.EnumEnhMetaFile(IntPtr.Zero, hEmf, EnumEnhMetafileCallback, IntPtr.Zero, ref rect);
			Size result = windowExtent;
			if (result == Size.Empty)
				result = metafile.Size;
			return ConvertLogicalUnitsToHundredthsOfMillimeter(result);
		}
		Size ConvertLogicalUnitsToHundredthsOfMillimeter(Size size) {
			size.Width = (int)Math.Round(2540.0f * Math.Abs(size.Width) / MetafileHelper.MetafileResolution);
			size.Height = (int)Math.Round(2540.0f * Math.Abs(size.Height) / MetafileHelper.MetafileResolution);
			return size;
		}
		[SecuritySafeCritical]
		int EnumMetafileCallback(IntPtr hdc, IntPtr handleTable, IntPtr metafileRecord, int objectCount, IntPtr clientData) {
			int recordType = Marshal.ReadInt16(metafileRecord, 4);
			EmfPlusRecordType emfRecordType = EmfPlusRecordType.WmfRecordBase + recordType;
			if (emfRecordType == EmfPlusRecordType.WmfSetWindowExt) {
				int height = Marshal.ReadInt16(metafileRecord, 6);
				int width = Marshal.ReadInt16(metafileRecord, 8);
				this.windowExtent = new Size(width, height);
			}
			else if (emfRecordType == EmfPlusRecordType.WmfSetMapMode)
				this.mapMode = (Win32.MapMode)Marshal.ReadInt16(metafileRecord, 6);
			return 1;
		}
		[SecuritySafeCritical]
		int EnumEnhMetafileCallback(IntPtr hdc, IntPtr handleTable, IntPtr metafileRecord, int objectCount, IntPtr clientData) {
			EmfPlusRecordType recordType = (EmfPlusRecordType)Marshal.ReadInt32(metafileRecord);
			if (recordType == EmfPlusRecordType.EmfSetWindowExtEx) {
				int width = Marshal.ReadInt32(metafileRecord, 8);
				int height = Marshal.ReadInt32(metafileRecord, 12);
				this.windowExtent = new Size(width, height);
			}
			else if (recordType == EmfPlusRecordType.WmfSetMapMode)
				this.mapMode = (Win32.MapMode)Marshal.ReadInt32(metafileRecord, 8);
			return 1;
		}
	}
	#endregion
}
