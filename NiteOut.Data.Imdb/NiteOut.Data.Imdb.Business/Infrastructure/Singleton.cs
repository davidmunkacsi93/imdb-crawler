using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Infrastructure
{
    /// <summary>
    /// Generic class, that enforces the realization of the single design pattern.
    /// </summary>
    /// <typeparam name="T">Class that realizes the singleton design pattern.</typeparam>
    public class Singleton<T>
        where T : class
    {
        /// <summary>
        /// Instance of the class T. />
        /// </summary>
        private static readonly Lazy<T> InstanceValue
            = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the instance of the class.
        /// </summary>
        public static T Instance
        {
            get { return InstanceValue.Value; }
        }
    }
}
