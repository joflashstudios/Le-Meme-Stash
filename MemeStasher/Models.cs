using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemeStasher
{
    public class MemeContext : DbContext
    {
        public DbSet<Meme> Memes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MemeTag> MemeTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=memes.db");
        }
    }

    public class Meme
    {
        public Meme() => Tags = new JoinCollectionFacade<Tag, Meme, MemeTag>(this, MemeTags);
        public int Id { get; set; }
        public string Sha { get; set; }
        public string MimeType { get; set; }
        public int ShareCount { get; set; }
        public User Author { get; set; }

        private ICollection<MemeTag> MemeTags { get; set; }
        [NotMapped]
        public ICollection<Tag> Tags { get; }
    }

    public class Tag
    {
        public Tag() => Memes = new JoinCollectionFacade<Meme, Tag, MemeTag>(this, PostTags);
        public int Id { get; set; }
        public string Name { get; set; }
        private ICollection<MemeTag> PostTags { get; } = new List<MemeTag>();

        [NotMapped]
        public ICollection<Meme> Memes { get; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }

    public class MemeTag : IJoinEntity<Meme>, IJoinEntity<Tag>
    {
        public int Id { get; set; }
        public int MemeId { get; set; }
        public Meme Meme { get; set; }
        Meme IJoinEntity<Meme>.Navigation
        {
            get => Meme;
            set => Meme = value;
        }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
        Tag IJoinEntity<Tag>.Navigation
        {
            get => Tag;
            set => Tag = value;
        }
    }

    public class JoinCollectionFacade<TEntity, TOtherEntity, TJoinEntity>
    : ICollection<TEntity>
    where TJoinEntity : IJoinEntity<TEntity>, IJoinEntity<TOtherEntity>, new()
    {
        private readonly TOtherEntity _ownerEntity;
        private readonly ICollection<TJoinEntity> _collection;

        public JoinCollectionFacade(
            TOtherEntity ownerEntity,
            ICollection<TJoinEntity> collection)
        {
            _ownerEntity = ownerEntity;
            _collection = collection;
        }

        public IEnumerator<TEntity> GetEnumerator()
            => _collection.Select(e => ((IJoinEntity<TEntity>)e).Navigation).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinEntity<TEntity>)entity).Navigation = item;
            ((IJoinEntity<TOtherEntity>)entity).Navigation = _ownerEntity;
            _collection.Add(entity);
        }

        public void Clear()
            => _collection.Clear();

        public bool Contains(TEntity item)
            => _collection.Any(e => Equals(item, e));

        public void CopyTo(TEntity[] array, int arrayIndex)
            => this.ToList().CopyTo(array, arrayIndex);

        public bool Remove(TEntity item)
            => _collection.Remove(
                _collection.FirstOrDefault(e => Equals(item, e)));

        public int Count
            => _collection.Count;

        public bool IsReadOnly
            => _collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e)
            => Equals(((IJoinEntity<TEntity>)e).Navigation, item);
    }
}