using System;

namespace OpenIddict.CouchDB
{
    internal static class Check
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> when obj is null.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="name">The name of the object parameter.</param>
        /// <exception cref="ArgumentNullException"/>
        internal static void NotNull(object obj, string name)
        {
            if (obj is null) throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when obj is null but
        /// it came from a search operation meaming there was no object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="name">The name of the object parameter.</param>
        /// <param name="message">The message to pass to the exception.</param>
        /// <exception cref="ArgumentException"/>
        internal static void Found(object obj, string name, string message)
        {
            if (obj is null) throw new ArgumentException(message, name);
        }
    }
}