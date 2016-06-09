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

using DevExpress.Data.Camera.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
namespace DevExpress.Data.Camera {
	[SecuritySafeCritical]
	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	internal class AMMediaType : IDisposable {
		public Guid MajorType;
		public Guid SubType;
		[MarshalAs(UnmanagedType.Bool)]
		public bool FixedSizeSamples = true;
		[MarshalAs(UnmanagedType.Bool)]
		public bool TemporalCompression;
		public int SampleSize = 1;
		public Guid FormatType;
		public IntPtr UnkPtr;
		public int FormatSize;
		public IntPtr FormatPtr;
		~AMMediaType() {
			this.Dispose(false);
		}
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(this.FormatSize != 0) Marshal.FreeCoTaskMem(this.FormatPtr);
			if(this.UnkPtr != IntPtr.Zero) Marshal.Release(this.UnkPtr);
		}
	}
	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	internal struct VideoInfoHeader {
		public RECT SrcRect;
		public RECT TargetRect;
		public int BitRate;
		public int BitErrorRate;
		public long AverageTimePerFrame;
		public BitmapInfoHeader BmiHeader;
	}
	[ComVisible(false), StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct BitmapInfoHeader {
		public int Size;
		public int Width;
		public int Height;
		public short Planes;
		public short BitCount;
		public int Compression;
		public int ImageSize;
		public int XPelsPerMeter;
		public int YPelsPerMeter;
		public int ColorsUsed;
		public int ColorsImportant;
	}
	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	internal struct RECT {
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}
	[ComVisible(false), StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	internal class PinInfo {
		public IBaseFilter Filter;
		public PinDirection Direction;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Name;
	}
	[ComVisible(false)]
	public enum PinDirection {
		Input,
		Output
	}
	[ComVisible(false)]
	internal class MediaTypes {
		public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid Text = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid Stream = new Guid(0xE436EB83, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
	}
	[ComVisible(false)]
	internal class MediaSubTypes {
		public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);
		public static readonly Guid RGB1 = new Guid(0xE436EB78, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB4 = new Guid(0xE436EB79, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB8 = new Guid(0xE436EB7A, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB565 = new Guid(0xE436EB7B, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB555 = new Guid(0xE436EB7C, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB24 = new Guid(0xE436Eb7D, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid RGB32 = new Guid(0xE436EB7E, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid Avi = new Guid(0xE436EB88, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
		public static readonly Guid Asf = new Guid(0x3DB80F90, 0x9412, 0x11D1, 0xAD, 0xED, 0x00, 0x00, 0xF8, 0x75, 0x4B, 0x99);
	}
	[ComImport, Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
	internal class CaptureGraphBuilder2 { }
	internal enum VideoProcAmpProperty {
		Brightness,
		Contrast,
		Hue,
		Saturation,
		Sharpness,
		Gamma,
		ColorEnable,
		WhiteBalance,
		BacklightCompensation,
		Gain
	}
	[Flags]
	internal enum VideoProcAmpFlags {
		None = 0,
		Auto = 1,
		Manual = 2
	}
	[Flags]
	internal enum AMRenderExFlags {
		None = 0,
		RenderToExistingRenderers = 1
	}
	[Flags]
	public enum NotifyFlags {
		None = 0,
		NoNotify = 1
	}
	public enum EventCode {
		Complete = 1,
		UserAbort,
		ErrorAbort,
		Time,
		Repaint,
		StErrStopped,
		StErrStPlaying,
		ErrorStPlaying,
		PaletteChanged,
		VideoSizeChanged,
		QualityChange,
		ShuttingDown,
		ClockChanged,
		Paused,
		OpeningFile = 16,
		BufferingData,
		FullScreenLost,
		Activate,
		NeedRestart,
		WindowDestroyed,
		DisplayChanged,
		Starvation,
		OleEvent,
		NotifyWindow,
		StreamControlStopped,
		StreamControlStarted,
		EndOfSegment,
		SegmentStarted,
		LengthChanged,
		DeviceLost,
		SampleNeeded,
		ProcessingLatency,
		SampleLatency,
		ScrubTime,
		StepComplete,
		SkipFrames,
		TimeCodeAvailable = 48,
		ExtDeviceModeChange,
		StateChange,
		PleaseReOpen = 64,
		Status,
		MarkerHit,
		LoadStatus,
		FileClosed,
		ErrorAbortEx,
		EOSSoon,
		ContentPropertyChanged,
		BandwidthChange,
		VideoFrameReady,
		GraphChanged = 80,
		ClockUnset,
		VMRRenderDeviceSet = 83,
		VMRSurfaceFlipped,
		VMRReconnectionFailed,
		PreprocessComplete,
		CodecApiEvent,
		DvdDomainChange = 257,
		DvdTitleChange,
		DvdChapterStart,
		DvdAudioStreamChange,
		DvdSubPicictureStreamChange,
		DvdAngleChange,
		DvdButtonChange,
		DvdValidUopsChange,
		DvdStillOn,
		DvdStillOff,
		DvdCurrentTime,
		DvdError,
		DvdWarning,
		DvdChapterAutoStop,
		DvdNoFpPgc,
		DvdPlaybackRateChange,
		DvdParentalLevelChange,
		DvdPlaybackStopped,
		DvdAnglesAvailable,
		DvdPlayPeriodAutoStop,
		DvdButtonAutoActivated,
		DvdCmdStart,
		DvdCmdEnd,
		DvdDiscEjected,
		DvdDiscInserted,
		DvdCurrentHmsfTime,
		DvdKaraokeMode,
		DvdProgramCellChange,
		DvdTitleSetChange,
		DvdProgramChainChange,
		DvdVOBU_Offset,
		DvdVOBU_Timestamp,
		DvdGPRM_Change,
		DvdSPRM_Change,
		DvdBeginNavigationCommands,
		DvdNavigationCommand,
		SNDDEVInError = 512,
		SNDDEVOutError,
		WMTIndexEvent = 593,
		WMTEvent,
		Built = 768,
		Unbuilt,
		StreamBufferTimeHole = 806,
		StreamBufferStaleDataRead,
		StreamBufferStaleFileDeleted,
		StreamBufferContentBecomingStale,
		StreamBufferWriteFailure,
		StreamBufferReadFailure,
		StreamBufferRateChanged
	}
	[StructLayout(LayoutKind.Explicit)]
	internal class DsGuid {
		[FieldOffset(0)]
		private Guid guid;
		public static readonly DsGuid Empty = Guid.Empty;
		public DsGuid() {
			this.guid = Guid.Empty;
		}
		public DsGuid(string g) {
			this.guid = new Guid(g);
		}
		public DsGuid(Guid g) {
			this.guid = g;
		}
		public override string ToString() {
			return this.guid.ToString();
		}
		public string ToString(string format) {
			return this.guid.ToString(format);
		}
		public override int GetHashCode() {
			return this.guid.GetHashCode();
		}
		public static implicit operator Guid(DsGuid g) {
			return g.guid;
		}
		public static implicit operator DsGuid(Guid g) {
			return new DsGuid(g);
		}
		public Guid ToGuid() {
			return this.guid;
		}
		public static DsGuid FromGuid(Guid g) {
			return new DsGuid(g);
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	internal class DsLong {
		private long Value;
		public DsLong(long Value) {
			this.Value = Value;
		}
		public override string ToString() {
			return this.Value.ToString();
		}
		public override int GetHashCode() {
			return this.Value.GetHashCode();
		}
		public static implicit operator long(DsLong l) {
			return l.Value;
		}
		public static implicit operator DsLong(long l) {
			return new DsLong(l);
		}
		public long ToInt64() {
			return this.Value;
		}
		public static DsLong FromInt64(long l) {
			return new DsLong(l);
		}
	}
	internal static class PinCategory {
		public static readonly Guid Capture = new Guid(4218176129u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid Preview = new Guid(4218176130u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid AnalogVideoIn = new Guid(4218176131u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid VBI = new Guid(4218176132u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid VideoPort = new Guid(4218176133u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid NABTS = new Guid(4218176134u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid EDS = new Guid(4218176135u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid TeleText = new Guid(4218176136u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid CC = new Guid(4218176137u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid Still = new Guid(4218176138u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid TimeCode = new Guid(4218176139u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
		public static readonly Guid VideoPortVBI = new Guid(4218176140u, 851, 4561, 144, 95, 0, 0, 192, 204, 22, 186);
	}
}
