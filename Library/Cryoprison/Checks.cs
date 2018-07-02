﻿using System;
using System.Collections.Generic;

namespace Cryoprison
{
    /// <summary>
    /// A check is an id-value pair, identifying both the jailbreak and the
    /// variant which is basically the parameter sent to the inspector.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// The identifier of the check, which will be used to create the
        /// identifier / jailbreak ID.  CheckId could e.g. be FOO while the
        /// identifier / jailbreak ID could e.g. be derived to be
        /// FILE_FOO_SHOULD_BE_PRESENT.
        /// </summary>
        public string CheckId { get; set; }

        /// <summary>
        /// The value, or parameter used when instantiating the jailbreak
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// A FluentApi factory for building up a collection of checks to be performed
    /// when performing a jailbreak test.
    /// </summary>
    public class Checks
    {
        /// <summary>
        /// The list of checks in this fluent factory
        /// </summary>
        private List<Check> checks { get; set; } = new List<Check>();

        /// <summary>
        /// The root paths to search.  When a check is added, a version with
        /// each root path is used.  If there is a seperator required, then the
        /// caller must either always include it in the root paths, or always
        /// include it in the added values.
        /// </summary>
        private List<string> rootPaths { get; set; } = new List<string>(new[] { "" });

        /// <summary>
        /// Adds the specified roots to our root paths collection.
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="roots">The roots to add</param>
        public Checks AddRoots(params string[] roots)
        {
            this.rootPaths.AddRange(roots);

            return this;
        }

        /// <summary>
        /// Add an individual check to the factory
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="checkId">The check ID.</param>
        /// <param name="val">The value, or parameter used when instantiating the inspector.</param>
        public Checks Add(string checkId, string val)
        {
            foreach (var root in this.rootPaths)
            {
                this.checks.Add(new Check { CheckId = checkId, Value = root + val });
            }

            return this;
        }

        /// <summary>
        /// Add a collection of checks to the factory
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="checkId">The check ID.</param>
        /// <param name="val">The values, or parameters used when instantiating the inspectors.</param>
        public Checks Add(string checkId, params string[] vals)
        {
            foreach (var val in vals)
            {
                this.Add(checkId, val);
            }

            return this;
        }

        /// <summary>
        /// Gets the instantiated, initialized inspectors that have been declared
        /// via Add statements.
        /// </summary>
        public IEnumerable<IInspector> GetInspectors<T>()
            where T : IInspector, new()
        {
            var retval = new List<IInspector>();

            foreach (var check in checks)
            {
                retval.Add(new T().Init(check.CheckId.ToUpperInvariant(), check.Value));
            }

            return retval;
        }
    }
}
