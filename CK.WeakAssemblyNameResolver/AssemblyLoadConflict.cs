using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CK.Core
{
    /// <summary>
    /// Captures an assembly load conflict. See <see cref="WeakAssemblyNameResolver"/>.
    /// </summary>
    public class AssemblyLoadConflict
    {
        /// <summary>
        /// Initalizes a new <see cref="AssemblyLoadConflict"/>.
        /// </summary>
        /// <param name="t">The date time of the conflict. Must be in <see cref="DateTimeKind.Utc"/>.</param>
        /// <param name="requesting">The requesting assembly name. Can be null.</param>
        /// <param name="wanted">The wanted assembly name. Can not be null.</param>
        /// <param name="resolved">The resolved assembly name. Can be be null.</param>
        /// <param name="installCount">Current number of installed hook.</param>
        public AssemblyLoadConflict( DateTime t, AssemblyName requesting, AssemblyName wanted, AssemblyName resolved, int installCount )
        {
            if( t.Kind != DateTimeKind.Utc ) throw new ArgumentException();
            if( wanted == null ) throw new ArgumentNullException( nameof( wanted ) );
            ConflictTimeUtc = t;
            Requesting = requesting;
            Wanted = wanted;
            Resolved = resolved;
            InstallCount = installCount;
        }

        /// <summary>
        /// Gets the number of active <see cref="WeakAssemblyNameResolver.Install"/> at the time when this
        /// conflict was captured.
        /// This should be used with care since in multi-threading environment Install/Uninstall are
        /// not "embedded" into each other.
        /// You should use this (to avoid reporting the same conflict more than once for instance) only
        /// when no parrallel activities can interfere and you have full control of the Install/Unistall scoping.
        /// </summary>
        public int InstallCount { get; }

        /// <summary>
        /// Gets the date and time in UTC of the conflict.
        /// </summary>
        public DateTime ConflictTimeUtc { get; }

        /// <summary>
        /// Gets the name of the assembly that requesting the wanted assembly.
        /// May be null (see <see cref="ResolveEventArgs.RequestingAssembly"/>).
        /// </summary>
        public AssemblyName Requesting { get; }

        /// <summary>
        /// Gets the wanted assembly name.
        /// This is never null.
        /// </summary>
        public AssemblyName Wanted { get; }

        /// <summary>
        /// Gets the resolved assembly name.
        /// Null if assembly has not been resolved.
        /// </summary>
        public AssemblyName Resolved { get; }

        /// <summary>
        /// Overridden to return a one-line descriptive string with all the data.
        /// </summary>
        /// <returns>A descriptive string.</returns>
        public override string ToString()
        {
            string success = Resolved != null ? "Sucess" : "Failed";
            string wanted = Wanted.FullName;
            string requesting = Requesting != null ? "requested by " + Requesting.FullName : "no requesting assembly";
            string resolved = Resolved != null ? Resolved.FullName : "(null)";
            return $"{success}: '{wanted}' => '{resolved}' ({requesting})";
        }
    }
}
