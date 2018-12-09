using System;
using System.Diagnostics;

namespace Tv7Playlist.Parser
{
    [DebuggerDisplay("ParsedTrack-{Id}(Name:{Name}, Url:{Url})")]
    public class ParsedTrack
    {
        public ParsedTrack(int id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public int Id { get; }

        public string Name { get; }

        public string Url { get; }

        protected bool Equals(ParsedTrack other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ParsedTrack) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Url)}: {Url}";
        }
    }
}