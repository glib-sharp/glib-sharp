// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[StructLayout(LayoutKind.Sequential)]
	public partial struct SourceCallbackFuncs : IEquatable<SourceCallbackFuncs> {


		public static GLib.SourceCallbackFuncs Zero = new GLib.SourceCallbackFuncs ();

		public static GLib.SourceCallbackFuncs New(IntPtr raw) {
			if (raw == IntPtr.Zero)
				return GLib.SourceCallbackFuncs.Zero;
			return (GLib.SourceCallbackFuncs) Marshal.PtrToStructure (raw, typeof (GLib.SourceCallbackFuncs));
		}

		public bool Equals (SourceCallbackFuncs other)
		{
			return true;
		}

		public override bool Equals (object other)
		{
			return other is SourceCallbackFuncs && Equals ((SourceCallbackFuncs) other);
		}

		public override int GetHashCode ()
		{
			return this.GetType().FullName.GetHashCode();
		}

		private static GLib.GType GType {
			get { return GLib.GType.Pointer; }
		}
#endregion
	}
}
