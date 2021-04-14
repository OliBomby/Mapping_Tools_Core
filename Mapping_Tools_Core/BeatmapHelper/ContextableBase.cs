﻿using System;
using System.Collections.Generic;
using Mapping_Tools_Core.BeatmapHelper.Contexts;

namespace Mapping_Tools_Core.BeatmapHelper {
    public abstract class ContextableBase {
        /// <summary>
        /// Additional properties.
        /// The objects always have the type of their key.
        /// </summary>
        protected Dictionary<Type, IContext> contexts;

        protected ContextableBase() {
            contexts = new Dictionary<Type, IContext>();
        }

        /// <summary>
        /// Checks whether this object has the context with type T.
        /// </summary>
        /// <typeparam name="T">The type to check the context of.</typeparam>
        /// <returns>Whether the context object with type T exists in this object.</returns>
        public bool HasContext<T>() where T : IContext {
            return contexts.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Gets the context with type T.
        /// </summary>
        /// <typeparam name="T">The type to get the context of.</typeparam>
        /// <exception cref="KeyNotFoundException">If the context does not exist in this hit object.</exception>
        /// <returns>The context object with type T.</returns>
        public T GetContext<T>() where T : IContext {
            return (T)contexts[typeof(T)];
        }

        /// <summary>
        /// Tries to get the context with type T.
        /// </summary>
        /// <param name="context">The found context with type T.</param>
        /// <typeparam name="T">The type to get the context of.</typeparam>
        /// <returns>Whether the context exists in this hit object.</returns>
        public bool TryGetContext<T>(out T context) where T : IContext {
            if (contexts.TryGetValue(typeof(T), out var context2)) {
                context = (T)context2;
                return true;
            }

            context = default;
            return false;
        }

        /// <summary>
        /// Sets the context object of type T.
        /// </summary>
        /// <typeparam name="T">The context type to set.</typeparam>
        /// <param name="context">The context object to store in this hit object.</param>
        public void SetContext<T>(T context) where T : IContext {
            contexts[typeof(T)] = context;
        }

        /// <summary>
        /// Removes the context of type T from the hit object.
        /// </summary>
        /// <typeparam name="T">The type to remove the context of.</typeparam>
        /// <returns>Whether a context was removed.</returns>
        public bool RemoveContext<T>() where T : IContext {
            return RemoveContext(typeof(T));
        }

        /// <summary>
        /// Removes the context of type T from the hit object.
        /// </summary>
        /// <param name="t">The type to remove the context of.</param>
        /// <returns>Whether a context was removed.</returns>
        public bool RemoveContext(Type t) {
            return contexts.Remove(t);
        }
    }
}