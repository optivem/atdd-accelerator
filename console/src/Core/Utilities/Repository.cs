using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public class Repository
    {
        public Repository(string owner, string name)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner cannot be null or whitespace.", nameof(owner));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            Owner = owner;
            Name = name;
        }

        public string Owner { get; }
        public string Name { get; }

        public string Path => $"{Owner}/{Name}";
    }
}
