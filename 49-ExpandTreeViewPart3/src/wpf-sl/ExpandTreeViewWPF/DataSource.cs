using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Markup;

namespace ExpandTreeViewWPF
{
    /// <summary>
    /// Represents an item contained in a level of a Linnaean taxonomy.
    /// </summary>
    [ContentProperty("Subclasses")]
    public abstract class Taxonomy
    {
        /// <summary>
        /// Gets the name of the TaxonomicRank.
        /// </summary>
        public string Rank
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets or sets the classification of the item being ranked.
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// Gets the subclasses of of the item being ranked.
        /// </summary>
        public Collection<Taxonomy> Subclasses { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TaxonomicItem class.
        /// </summary>
        protected Taxonomy()
        {
            Subclasses = new Collection<Taxonomy>();
        }

        /// <summary>
        /// Get a string representation of the TaxonomicItem.
        /// </summary>
        /// <returns>String representation of the TaxonomicItem.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", Rank, Classification);
        }
    }

    /// <summary>
    /// Represents a Domain in a Linnaean taxonomy.
    /// </summary>
    public sealed class Domain : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Kingdom in a Linnaean taxonomy.
    /// </summary>
    public sealed class Kingdom : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Class in a Linnaean taxonomy.
    /// </summary>
    public sealed class Class : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Family in a Linnaean taxonomy.
    /// </summary>
    public sealed class Family : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Genus in a Linnaean taxonomy.
    /// </summary>
    public sealed class Genus : Taxonomy
    {
    }

    /// <summary>
    /// Represents an Order in a Linnaean taxonomy.
    /// </summary>
    public sealed class Order : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Phylum in a Linnaean taxonomy.
    /// </summary>
    public sealed class Phylum : Taxonomy
    {
    }

    /// <summary>
    /// Represents a Species in a Linnaean taxonomy.
    /// </summary>
    public sealed class Species : Taxonomy
    {
    }
}
